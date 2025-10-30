Imports Robotics.Comms.Base
Imports Robotics.Comms.DriverZKPush2
Imports Robotics.Comms.DriverZKPush2.BusinesProtocol
Imports Robotics.VTBase

Namespace VTTerminals

    Public Class roPushTerminalManager
        'Inherits TerminalLogicZKPush2

        Public oState As roTerminalsState = Nothing
        Protected mTerminalLogic As TerminalLogicZKPush2
        Protected mTerminal As TerminalZKPush2

        Public ReadOnly Property State As roTerminalsState
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roTerminalsState, ByRef oTerminalLogic As TerminalLogicZKPush2)
            Me.mTerminalLogic = oTerminalLogic
            Me.mTerminal = oTerminalLogic.mTerminal
            Me.oState = _State
        End Sub

        Public Function ProcessMessage(oIncomingMessage As MessageZKPush2) As MessageZKPush2
            Dim oResponseMessage As MessageZKPush2 = Nothing
            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                ' En función del mensaje, respondo
                Select Case oIncomingMessage.Command
                    Case MessageZKPush2.msgCommand.init
                        roTrace.GetInstance.InitTraceEvent()
                        mTerminalLogic.bTerminalConnected = True
                        mTerminalLogic.bForceInitSession = False
                        mTerminalLogic.bGetSecurityOptions = True
                        'Conexión inicial. Sólo se produce cuando se reinicia la centralita.
                        'Si necesito reinicializar la lógica, lo hago ahora
                        If mTerminalLogic.NeedReInitialization Then
                            mTerminalLogic.ReInitialize()
                        Else
                            ' Marco para que si se vuelve a pasar por el init (lo que implica que el terminal ha abortado la comunciación, por desconexión de cable, apagado, ...) se reinicialice el driver
                            mTerminalLogic.NeedReInitialization = True
                        End If

                        ' En IIS los terminales ZK se registran en Azure, luego no se puede dar el caso
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.init, , True, oIncomingMessage.data_init.HttpVersion)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.Stamp, mTerminal.PunchTimeStatmp)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.OpStamp, mTerminal.OperTimeStatmp)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.PhotoStamp, "0")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.TransFlag, "TransFlag=TransData AttLog	OpLog	AttPhoto	EnrollUser	ChgUser	EnrollFP	ChgFP	FACE")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.ErrorDelay, "30")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.Delay, mTerminal.RequestDelaySeconds)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.TimeZone, mTerminal.TimeZone.ToString)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.TransInterval, "1")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.SyncTime, "60")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.Realtime, "1")
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.ServerVer, clsPushProtocol.PUSH_SERVER_VER)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.ATTLOGStamp, mTerminal.PunchTimeStatmp)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.OPERLOGStamp, mTerminal.OperTimeStatmp)
                        oResponseMessage.data_initresponse.setValue(MsgData_InitResponse.dataParameters.ATTPHOTOStamp, "0")
                        oResponseMessage.data_initresponse.Status = MsgData.dataStatus.success
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"roPushTerminalManager:ProcessMessage:{mTerminal.ToString}:Terminal is properly registered and is successfully initialized. RequestDelay {mTerminal.RequestDelaySeconds} seconds.Enjoy !!!")
                        mTerminalLogic.DelUserTask()
                        ' Marcamos que se debe pedir la versión de firmware (y resto de info del terminal), y establecer el usuario admin
                        mTerminalLogic.bGetFirmversion = True
                        mTerminalLogic.bSetAdminUser = True
                        roTrace.GetInstance.AddTraceEvent("Init command processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.punch
                        ' Evento
                        ' Si es un fichaje, lo guardo
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        mTerminalLogic.bError = False
                        mTerminalLogic.bSavePunch = True
                        SyncLock Me
                            If oIncomingMessage.data_punches.Punches.Count > 0 Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Punches received: (" + oIncomingMessage.data_punches.toString + ")")
                                For Each oPunch As MsgData_Table_punch In oIncomingMessage.data_punches.Punches
                                    If oPunch.IsPunch Then
                                        Dim oCurrentPunch As clsTerminalPunch = Nothing
                                        If mTerminalLogic.LoadCurrentDataIIS(oPunch, mTerminalLogic.bSavePunch, oCurrentPunch) Then
                                            If mTerminalLogic.bSavePunch AndAlso Not oCurrentPunch.SavePunch() Then
                                                mTerminalLogic.bError = True
                                            End If
                                        Else
                                            mTerminalLogic.bError = True
                                        End If
                                    Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Event Ignored: (" + oIncomingMessage.data_punches.toString + ")")
                                    End If
                                Next
                            Else
                                ' El mensaje de fichajes llegó vacío o no se pudo cargar ... No actualizo Timestamp para que vuelva a caer
                                mTerminalLogic.bError = True
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Unable to process punches message: (" + oIncomingMessage.data_punches.HttpContent + ")")
                            End If

                            ' Si todo fue correcto, actualizo el timestamp de fichajes y respondo
                            If Not mTerminalLogic.bError Then
                                mTerminal.UpdateAttlogStamp(oIncomingMessage.data_punches.PunchStamp)
                                oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_punches.HttpVersion)
                                oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            Else
                                ' Hubo un error. No respondo para forzar que  se vuelva a enviar ...
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error processing punch: (" + oIncomingMessage.data_punches.toString + ")")
                                oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                            End If
                        End SyncLock
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.operation
                        ' Estado
                        roTrace.GetInstance.InitTraceEvent()
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        Dim bResult As Boolean = False
                        bResult = mTerminalLogic.ProcessOperationLog(oIncomingMessage.data_operation)
                        If bResult Then mTerminal.UpdateOperStamp(oIncomingMessage.data_operation.OpStamp)
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_operation.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roTrace.GetInstance.AddTraceEvent("Operation command processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.biodata
                        ' Estado
                        roTrace.GetInstance.InitTraceEvent()
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        Dim bResult As Boolean = False
                        bResult = mTerminalLogic.ProcessBiodataLog(oIncomingMessage.data_biodata)
                        If bResult Then mTerminal.UpdateOperStamp(oIncomingMessage.data_biodata.OpStamp)
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_biodata.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roTrace.GetInstance.AddTraceEvent("Biodata command processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.queryresult
                        ' Resultado de una consulta
                        ' Marco comando
                        roTrace.GetInstance.InitTraceEvent()
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        Try
                            mTerminalLogic.ProcessCommandResult(oIncomingMessage.data_queryresult.CommandID)
                        Catch ex As Exception
                        End Try
                        ' Respondo OK
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_queryresult.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roTrace.GetInstance.AddTraceEvent("Query result command processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.getrequest
                        Dim bAbortSync As Boolean = False
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        ' Recupero comandos
                        Try
                            ' Lo primero es asegurar que el terminal tiene un empleado admin correcto, y que no hay cambios relevantes en configuración general del terminal
                            If mTerminalLogic.bRunningWithoutAdmin OrElse (Not mTerminalLogic.bSynchronizing AndAlso (mTerminalLogic.bConfigChangesPending OrElse mTerminalLogic.bSetAdminUser OrElse Math.Abs(Now.Subtract(mTerminalLogic.dlastTimeSyncLog).TotalMinutes) > 60)) Then
                                ' Programo usuario administrador por si han hecho algún cambio fraudulento
                                roTrace.GetInstance.InitTraceEvent()
                                oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                oResponseMessage.data_table.addCommand(-9999, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERINFO, True)
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, mTerminal.AdminID)
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Name, "Admin")
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Passwd, mTerminal.AdminPWD)
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Card, "[0000000000]")
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Grp, "0")
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZ, "0")
                                oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pri, "14")
                                If mTerminalLogic.bConfigChangesPending Then
                                    ' Cosas que hacer una vez cuando hay cambio de configuración en config.dat. Este cambio se puede generar en BroadcasterNET
                                    If mTerminal.RDRIsAttendance(1) Then
                                        oResponseMessage.data_table.addCommand(-9998, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.AccGroup, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.ID, "1")
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZ, "0032|0000|0000")
                                    End If
                                End If
                                ' La hora se actualiza en cada mensaje intercambiado. Para no llenar los logs, dejo log sólo una vez por minuto ...
                                mTerminalLogic.dlastTimeSyncLog = Now
                                mTerminalLogic.bSetAdminUser = False
                                mTerminalLogic.bConfigChangesPending = False
                                mTerminalLogic.bRunningWithoutAdmin = False
                                roTrace.GetInstance.AddTraceEvent("Admin user command added")
                                Return oResponseMessage
                            Else
                                mTerminalLogic.DelUserTask()
                                mTerminalLogic.oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                                Dim eLastTaskType As New roTerminalsSyncTasks.SyncActions
                                Dim bExit As Boolean = False
                                Dim bCallBroadcaster As Boolean = False

                                mTerminalLogic.oCurrentTask.LoadNext()

                                If mTerminalLogic.oCurrentTask.Task <> roTerminalsSyncTasks.SyncActions.none Then
                                    ' Si hay alguna tarea atascada, lo advierto ahora
                                    mTerminalLogic.bSynchronizing = True
                                    If mTerminalLogic.oCurrentTask.Retries > 10 Then
                                        mTerminalLogic.CreateUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED", "TerminalSyncStopped.Title")
                                    Else
                                        mTerminalLogic.DelUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED")
                                    End If
                                    eLastTaskType = mTerminalLogic.oCurrentTask.Task
                                    oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                    mTerminalLogic.iTasksInMessage = 0
                                    mTerminalLogic.params = Array.CreateInstance(GetType(String), 100)
                                    mTerminalLogic.iEvent = 0
                                    While (Not bExit) AndAlso eLastTaskType = mTerminalLogic.oCurrentTask.Task
                                        ' Traducimos tarea a comandos según el protocolo de la centralita
                                        If mTerminalLogic.GetNextCommand(oResponseMessage, mTerminalLogic.oCurrentTask, oIncomingMessage.data_getrequest.HttpVersion, bExit, bCallBroadcaster, True) Then
                                            mTerminalLogic.oCurrentTask.WorkingEx()
                                        Else
                                            'Algo va mal. Reinicio la sincronización del terminal
                                            mTerminalLogic.oCurrentTask.WillBeRetried()
                                            mTerminalLogic.oCurrentTask.ResetAllWithDelay(10)
                                            bAbortSync = True
                                            Exit While
                                        End If
                                        If Not bExit Then
                                            mTerminalLogic.oCurrentTask.LoadNext()
                                        End If
                                    End While

                                    If bAbortSync Then
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                        mTerminalLogic.iEvent = 0
                                        bAbortSync = False
                                    End If
                                    roTrace.GetInstance.InitTraceEvent()
                                    mTerminal.UpdateLastUpdate()
                                    roTrace.GetInstance.AddTraceEvent("Terminal last update updated")
                                    If mTerminalLogic.iEvent > 0 Then
                                        Dim sDetail As String = vbCr & String.Join(vbCr, mTerminalLogic.params.ToList.FindAll(Function(x) x IsNot Nothing AndAlso x.ToString.Length > 0).ToArray)
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Update info on terminal -> " & [Enum].GetName(GetType(CTerminalLogicBase.mxCInbioEventID), mTerminalLogic.iEvent) & sDetail)
                                    Else
                                        Dim sDetail As String = String.Join(vbCr, mTerminalLogic.params.ToList.FindAll(Function(x) x IsNot Nothing AndAlso x.ToString.Length > 0).ToArray)
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Update info on terminal -> " & sDetail)
                                    End If
                                    ' Sea como sea, marco todas las tareas como no enviadas. En teoria, la respuesta del terminal debe llegar antes de que volvamos a pasar por aquí. Si no llegara
                                    ' la respuesta, de esta manera se volverán a enviar. Si no hago esto y el mensaje no llega al terminal o este no lo procesa porque llegó más allá de su timeout
                                    ' la tarea quedaría como deleteonconfirm, y no se reporocesaría hasta que la comunicación se restableciera
                                    roTrace.GetInstance.InitTraceEvent()
                                    mTerminalLogic.oCurrentTask.ResetAll()
                                    roTrace.GetInstance.AddTraceEvent("All terminal tasks reseted")
                                    If bCallBroadcaster Then
                                        CallBroadcaster(mTerminal.ID)
                                    End If
                                    Return oResponseMessage
                                Else
                                    mTerminalLogic.bSynchronizing = False
                                    mTerminal.UpdateStatus(True)
                                    ' No hay comandos. Miro si debo ejecutar alguna tarea especial
                                    'If mTerminal.FirmVersion.Trim.Length = 0 OrElse bGetFirmversion Then
                                    If mTerminalLogic.bGetFirmversion Then
                                        ' Si no tengo informada la versión de firmware, la pido ahora
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_table.addCommand(mTerminalLogic.oCurrentTask.ID, msgdata_table_command.dataCommands.INFO, MessageZKPush2.msgTables.none, True)
                                        ' Si es un rx1, pido resto de parámetros de configuración
                                        If mTerminal.Model = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                            oResponseMessage.data_table.addCommand(mTerminalLogic.oCurrentTask.ID, msgdata_table_command.dataCommands.GET_OPTION, MessageZKPush2.msgTables.none, True)
                                            oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, mTerminalLogic.GetTerminalConfigForRMA)
                                        End If
                                        mTerminalLogic.bGetFirmversion = False
                                    ElseIf mTerminalLogic.bGetSecurityOptions AndAlso Math.Abs(Now.Subtract(mTerminalLogic.dLastTerminalMenuEntered).TotalMinutes) >= 2 Then
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_table.addCommand(mTerminalLogic.oCurrentTask.ID, msgdata_table_command.dataCommands.GET_OPTION, MessageZKPush2.msgTables.none, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, mTerminalLogic.GetTerminalSecurityConfig)
                                        mTerminalLogic.bGetSecurityOptions = False
                                    ElseIf mTerminal.IsInDST <> mTerminal.LastInDSTStatus OrElse mTerminalLogic.bRebootPending Then
                                        'Requiere reinicio por cambio de DST
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_table.addCommand(mTerminalLogic.oCurrentTask.ID, msgdata_table_command.dataCommands.REBOOT, MessageZKPush2.msgTables.none, True)
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ": Terminal will be rebooted")
                                    ElseIf mTerminalLogic.bForceInitSession Then
                                        ' Forzamos
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                                    ElseIf mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper AndAlso mTerminal.NextRebootTime <> DateTime.MinValue AndAlso Now.Subtract(mTerminal.NextRebootTime).TotalSeconds > 5 Then
                                        ' Tengo un reinicio programado pendiente para un terminal rx1
                                        mTerminal.NextRebootTime = mTerminal.NextRebootTime.AddDays(1)
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ": Scheduled reboot.")
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_table.addCommand(mTerminalLogic.oCurrentTask.ID, msgdata_table_command.dataCommands.REBOOT, MessageZKPush2.msgTables.none, True)
                                    Else
                                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    End If
                                    Return oResponseMessage
                                End If
                            End If

                            If mTerminalLogic.oCurrentTask IsNot Nothing Then mTerminalLogic.oCurrentTask = Nothing
                        Catch ex As Exception
                            ' Si todo falla, respondo para no perder la secuencia del terminal
                            oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                            oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error:", ex)
                            Return oResponseMessage
                        End Try
                    Case MessageZKPush2.msgCommand.commandresult
                        ' Resultado de un comando
                        roTrace.GetInstance.InitTraceEvent()
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        If Not oIncomingMessage.data_commandresult.Results Is Nothing AndAlso oIncomingMessage.data_commandresult.Results.Count > 0 Then
                            mTerminalLogic.ProcessCommandResult(oIncomingMessage.data_commandresult.Results)
                        End If
                        ' Si era un comando de INFO, proceso los parámetros recibidos
                        If Not oIncomingMessage.data_commandresult.DeviceParameters Is Nothing AndAlso oIncomingMessage.data_commandresult.DeviceParameters.Count > 0 Then
                            mTerminalLogic.ProcessDeviceParameters(oIncomingMessage.data_commandresult.DeviceParameters)
                        End If
                        ' Respuesta
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_commandresult.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roTrace.GetInstance.AddTraceEvent("Terminal command result processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.punchphoto
                        roTrace.GetInstance.InitTraceEvent()
                        ' Fotografía tomada en un fichaje ya registrado
                        If Not clsTerminalPunch.SavePhoto(oIncomingMessage.data_punchphoto.Photo, oIncomingMessage.data_punchphoto.PIN, oIncomingMessage.data_punchphoto.PunchDateTime, roLog.GetInstance()) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ": Unable to save photo for punch: idemployee = " & oIncomingMessage.data_punchphoto.PIN)
                        End If
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_punchphoto.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roTrace.GetInstance.AddTraceEvent("Punch photo command processed")
                        Return oResponseMessage
                    Case MessageZKPush2.msgCommand.noprocessedtable
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, oIncomingMessage.data_nonprocessedtable.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ": Received options table non supported yet. We will response OK. Have luck!")
                        Return oResponseMessage
                    Case Else
                        oResponseMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ": Unknown message received " & oIncomingMessage.Command & ". We will response OK. Have luck!")
                        Return oResponseMessage
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPushTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error:", ex)
                oResponseMessage = Nothing
                oState.Result = roTerminalsState.ResultEnum.Exception
            End Try

            Return oResponseMessage
        End Function

    End Class

End Namespace