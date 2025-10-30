Imports Robotics.Comms.Base
Imports Robotics.Comms.DrivermxS
Imports Robotics.Comms.DrivermxS.BusinesProtocol
Imports Robotics.VTBase

Namespace VTTerminals

    ''' <summary>
    ''' Manager de la lógica de centralitas mxS
    ''' </summary>
    Public Class roInBioTerminalManager

        Public oState As roTerminalsState = Nothing
        Protected mTerminalLogic As TerminalLogicMxS
        Protected mTerminal As TerminalMxS

        Public ReadOnly Property State As roTerminalsState
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roTerminalsState, ByRef oTerminalLogic As TerminalLogicMxS)
            Me.mTerminalLogic = oTerminalLogic
            Me.mTerminal = oTerminalLogic.mTerminal
            Me.oState = _State
        End Sub

        Public Function ProcessMessage(oIncomingMessage As MessageMxS) As MessageMxS
            Dim oResponseMessage As MessageMxS = Nothing

            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                ' En función del mensaje, respondo
                Select Case oIncomingMessage.Command
                    Case MessageMxS.msgCommand.init
                        mTerminalLogic.bTerminalConnected = True
                        'Conexión inicial. Sólo se produce cuando se reinicia la centralita.
                        'Si necesito reinicializar la lógica, lo hago ahora
                        If mTerminalLogic.NeedReInitialization Then
                            mTerminalLogic.ReInitialize()
                        Else
                            ' Marco para que si se vuelve a pasar por el init (lo que implica que el terminal ha abortado la comunciación, por desconexión de cable, apagado, ...) se reinicialice el driver
                            mTerminalLogic.NeedReInitialization = True
                        End If
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.init, , True, oIncomingMessage.data_init.HttpVersion)

                        'Centralitas mxS 2024 (requieren estos parámetros para realizar el handshake de registro)
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.registry, "ok")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.RegistryCode, mTerminal.RegistrationCode.Substring(1, 10))

                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.ServerVersion, clsPushProtocol.PUSH_SERVER_VER)
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.ServerName, "VisualTimeLive")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.PushVersion, clsPushProtocol.PUSH_LIB_VER)
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.ErrorDelay, "60")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.RequestDelay, mTerminal.RequestDelaySeconds.ToString)
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.TransTimes, "00:00" + vbTab + "14:00")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.TransInterval, "1")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.TransTables, "User" + vbTab + "Transaction")
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.Realtime, "1")
                        mTerminalLogic.mCurrentSessionID = Helper.GetSessionID
                        oResponseMessage.data_initresponse.setValue(MsgData_ConfigResponse.dataParameters.SessionID, mTerminalLogic.mCurrentSessionID)
                        oResponseMessage.data_initresponse.Status = MsgData.dataStatus.success
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.register
                        mTerminalLogic.bTerminalConnected = True
                        'Solicitud de registro. Reinicio de centralita, servidor de comunicaciones o restablecimiento de la comunicación
                        mTerminalLogic.CheckTerminalReaders(CInt(oIncomingMessage.data_register.getValue(MsgData_Register.dataParameters.LockCount)))
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.register, , True, oIncomingMessage.data_register.HttpVersion)
                        oResponseMessage.data_registerresponse.Status = MsgData.dataStatus.success
                        oResponseMessage.data_registerresponse.setValue(MsgData_RegisterResponse.dataParameters.registrycode, mTerminal.RegistrationCode.Substring(1, 10))
                        mTerminalLogic.mRegistryCode = mTerminal.RegistrationCode.Substring(1, 10)
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager:ProcessMessage:" + mTerminal.ToString + ":Terminal Registered !!!")
                        mTerminal.UpdateFirmVer(oIncomingMessage.data_register.getValue(MsgData_Register.dataParameters.FirmVer))
                        ' Marco para recoger info de red
                        mTerminalLogic.bGetNetworkConfig = True
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.getpushconfig
                        mTerminalLogic.bTerminalConnected = True
                        ' Solicitud de parámetros
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.getpushconfig, , True, oIncomingMessage.data_config.HttpVersion)
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.ServerVersion, clsPushProtocol.PUSH_SERVER_VER)
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.ServerName, "VisualTimeLive")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.PushVersion, clsPushProtocol.PUSH_LIB_VER)
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.ErrorDelay, "60")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.RequestDelay, mTerminal.RequestDelaySeconds.ToString)
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.TransTimes, "00:00" + vbTab + "14:00")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.TransInterval, "1")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.TransTables, "User" + vbTab + "Transaction")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.Realtime, "1")
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.TimeoutSec, "10")
                        mTerminalLogic.mCurrentSessionID = Helper.GetSessionID
                        oResponseMessage.data_configresponse.setValue(MsgData_ConfigResponse.dataParameters.SessionID, mTerminalLogic.mCurrentSessionID)
                        ' Valido sesión
                        '''If oIncomingMessage.data_config.Token <> clsPushProtocol.GetCommunicationToken(mTerminalLogic.mRegistryCode, oIncomingMessage.data_config.SN, oIncomingMessage.data_config.Timestamp) Then
                        '''    ' Sesión inválida. No respondo (así lo hace la demos de ZK, aunque debería devolver un mensaje con cabecera 406 según sdk y otros mensajes de la demo
                        '''    VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Invalid session detected(" + mTerminalLogic.mRegistryCode + "," + oIncomingMessage.data_config.SN + "," + oIncomingMessage.data_config.Timestamp + "): Token should be:" + oIncomingMessage.data_config.Token + " Communication will be restarted.")
                        '''    Return Nothing
                        '''End If
                        oResponseMessage.data_configresponse.Status = MsgData.dataStatus.success
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager:ProcessMessage:" + mTerminal.ToString + ":Protocol Configured !!!")
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.logevent
                        ' Evento
                        ' Si es un fichaje, lo guardo
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        mTerminalLogic.bError = False
                        mTerminalLogic.bSavePunch = True

                        ' Aquí se podría/debería validar sesión. Como lo hago en el Getrequest, que se recibe cada 10 segundos aprox. de momento no validamos para no comprometer los fichajes
                        SyncLock Me
                            If oIncomingMessage.data_event.IsPunch Then
                                Dim oCurrentPunch As clsTerminalPunch = Nothing
                                If mTerminalLogic.LoadCurrentDataIIS(oIncomingMessage, mTerminalLogic.bSavePunch, oCurrentPunch) Then
                                    If mTerminalLogic.bSavePunch AndAlso Not oCurrentPunch.SavePunch(mTerminalLogic.mCardType) Then
                                        ' El fichaje se guardó en fichero entries.vtr
                                        mTerminalLogic.bError = True
                                    End If
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error loading punch data. Punch message was: " & oIncomingMessage.data_event.HttpContent)
                                    mTerminalLogic.bError = True
                                End If
                            Else
                                Select Case oIncomingMessage.data_event.LogEvent
                                    Case 105
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Device offline: (" + oIncomingMessage.data_event.toString + ") ")
                                    Case 214
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Device online: (" + oIncomingMessage.data_event.toString + ")")
                                    Case 206
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Device started: (" + oIncomingMessage.data_event.toString + ")")
                                    Case Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Event Ignored: (" + oIncomingMessage.data_event.toString + ")")
                                End Select
                            End If

                            ' Si no se pudo guardar el fichaje, no respondo, para que el terminal lo vuelva a enviar.
                            If Not mTerminalLogic.bError Then
                                oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_event.HttpVersion)
                                oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                Return oResponseMessage
                            Else
                                ' Hubo un error. No respondo para forzar que  se vuelva a enviar ...
                                ' OJO: Si no respondo, ES PROBABLE QUE LA CENTRALITA NO VUELVA A ENVIAR EL FICHAJE.FUE UN ERROR PRODUCIDO EN LA CENTRALITA MXC, ARREGLADO, PERO APARENTEMENTE NO SE PROPAGÓ A INBIOPRO
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error processing punch: (" + oIncomingMessage.data_event.toString + ")")
                                oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                            End If
                        End SyncLock
                    Case MessageMxS.msgCommand.state
                        ' Estado
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_state.HttpVersion)
                        ' Valido sesión
                        '''If oIncomingMessage.data_state.Token <> clsPushProtocol.GetCommunicationToken(mTerminalLogic.mRegistryCode, oIncomingMessage.data_state.SN, mTerminalLogic.mCurrentSessionID) Then
                        '''    ' Sesión inválida. Respondo con sesión inválida (406)
                        '''    roLog.GetInstance().logMessage(roLog.EventType.roError, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Invalid session detected(" + mTerminalLogic.mRegistryCode + "," + oIncomingMessage.data_state.SN + "," + mTerminalLogic.mCurrentSessionID + "): Token should be:" + oIncomingMessage.data_state.Token + " Communication will be restarted.")
                        '''    oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                        '''Else
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        '''End If
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.queryresult
                        ' Resultado de una consulta
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        Try
                            mTerminalLogic.ProcessCommandResult(oIncomingMessage.data_queryresult.CommandID)
                        Catch ex As Exception
                        End Try
                        ' Recojo información si la hay
                        If oIncomingMessage.data_queryresult.DeviceParameters IsNot Nothing AndAlso oIncomingMessage.data_queryresult.DeviceParameters.Count > 0 Then
                            mTerminalLogic.ProcessDeviceParameters(oIncomingMessage.data_queryresult.DeviceParameters)
                        End If

                        ' Respondo OK
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_queryresult.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.getrequest
                        Dim bAbortSync As Boolean = False
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True

                        ' Recupero comandos
                        Try
                            ' Valido sesión
                            ''''If oIncomingMessage.data_getrequest.Token <> clsPushProtocol.GetCommunicationToken(mTerminalLogic.mRegistryCode, oIncomingMessage.data_getrequest.SN, mTerminalLogic.mCurrentSessionID) Then
                            ''''    ' Sesión inválida. Devuelvo un mensaje con cabecera 406 según sdk
                            ''''    roLog.GetInstance().logMessage(roLog.EventType.roError, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Invalid session detected(" + mTerminalLogic.mRegistryCode + "," + oIncomingMessage.data_getrequest.SN + "," + mTerminalLogic.mCurrentSessionID + "): Token should be:" + oIncomingMessage.data_getrequest.Token + " Communication will be restarted.")
                            ''''    oResponseMessage = New MessageMxC(MessageMxC.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                            ''''    oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                            ''''    Return oResponseMessage
                            ''''End If

                            ' TODO: Si estoy esperando la respuesta de alguna tarea, no envío ninguna más. Sólo envío otras del mismo tipo
                            Dim bWaitingForSomeResponses As Boolean = False
                            If Not bWaitingForSomeResponses Then
                                mTerminalLogic.oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                                Dim eLastTaskType As New roTerminalsSyncTasks.SyncActions
                                Dim bExit As Boolean = False
                                Dim bCallBroadcaster As Boolean = False
                                mTerminalLogic.oCurrentTask.LoadNext()
                                If mTerminal.ZKInbioHeartBeatControl AndAlso mTerminalLogic.oCurrentTask.Task <> roTerminalsSyncTasks.SyncActions.none AndAlso mTerminal.RequestDelaySeconds = mTerminal.DefaultRequestDelaySeconds Then
                                    'Prototipo de ajuste de heartbeat
                                    'Si esta es la primera tarea después de un tiempo sin tareas, ajusto el heartbeat a 2
                                    mTerminal.RequestDelaySeconds = 2
                                    'Fuerzo reinicio del establecimiento de la comunicación para que el tiempo entre solicitudes sea corto.
                                    oResponseMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                    oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                                    Return oResponseMessage
                                End If

                                If mTerminalLogic.oCurrentTask.Task <> roTerminalsSyncTasks.SyncActions.none Then
                                    ' Si hay alguna tarea atascada, lo advierto ahora
                                    If mTerminalLogic.oCurrentTask.Retries > 10 Then
                                        mTerminalLogic.CreateUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED", "TerminalSyncStopped.Title")
                                    Else
                                        mTerminalLogic.DelUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED")
                                    End If

                                    eLastTaskType = mTerminalLogic.oCurrentTask.Task
                                    oResponseMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                    mTerminalLogic.iTasksInMessage = 0
                                    While (Not bExit) AndAlso eLastTaskType = mTerminalLogic.oCurrentTask.Task
                                        ' Traducimos tarea a comandos según el protocolo de la centralita
                                        If mTerminalLogic.GetNextCommand(oResponseMessage, mTerminalLogic.oCurrentTask, oIncomingMessage.data_getrequest.HttpVersion, bExit, bCallBroadcaster) Then
                                            mTerminalLogic.oCurrentTask.WorkingEx()
                                        Else
                                            'Algo va mal. Reinicio la sincronización del terminal
                                            mTerminalLogic.oCurrentTask.WillBeRetried()
                                            mTerminalLogic.oCurrentTask.ResetAllWithDelay(10)
                                            bAbortSync = True
                                            Exit While
                                        End If
                                        If Not bExit Then mTerminalLogic.oCurrentTask.LoadNext()
                                    End While
                                    If bAbortSync Then
                                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                        bAbortSync = False
                                    End If
                                    ' Sea como sea, marco todas las tareas como no enviadas. En teoria, la respuesta del terminal debe llegar antes de que volvamos a pasar por aquí. Si no llegara
                                    ' la respuesta, de esta manera se volverán a enviar. Si no hago esto y el mensaje no llega al terminal o no este no lo procesa porque llegó más allá de su timeout
                                    ' la tarea quedaría como deleteonconfirm, y no se reporocesaría hasta que la comunicación se restableciera
                                    mTerminalLogic.oCurrentTask.ResetAll()
                                    If bCallBroadcaster Then
                                        CallBroadcaster(mTerminal.ID)
                                    End If
                                    mTerminal.UpdateLastUpdate()
                                    Return oResponseMessage
                                Else
                                    ' No hay comandos.
                                    mTerminal.UpdateStatus(True)
                                    ' Si no recogí parámetros de red, lo hago ahora
                                    If mTerminalLogic.bGetNetworkConfig Then
                                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        oResponseMessage.data_table.addCommand(-2, msgdata_table_command.dataCommands.GET_OPTIONS, MessageMxS.msgTables.none, True)
                                        Dim sNetworkParams As String = "IPAddress,GATEIPAddress,NetMask,WebServerURL,WebServerIP"
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sNetworkParams)
                                        mTerminalLogic.bGetNetworkConfig = False
                                    ElseIf Math.Abs(Now.Subtract(mTerminalLogic.dlastTimeSync).TotalSeconds) > 900 Then
                                        ' Devuelvo sincronización de hora si hace más de 15 minutos que no lo hago ...
                                        mTerminal.RefreshDLSTConfig()

                                        'Sincronizo hora
                                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, oIncomingMessage.data_getrequest.HttpVersion)
                                        ' Si no paso el parámetro MachineTZ, la centralita sólo funcionaría en uso horario España. Pero si lo paso, hay centralitas que no actualizan la hora (por error en firmware ZK)
                                        oResponseMessage.data_table.addCommand(-1, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MachineTZ, mTerminal.TimeZone)
                                        oResponseMessage.data_table.addCommand(-7, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.DateTime, clsPushProtocol.ConvertDateTime(Date.UtcNow))

                                        ' Añado configuración de DLST (estríctamente no seria necesario, ya que se hace en cada setterminalconfig)
                                        oResponseMessage.data_table.addCommand(-2, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DSTOn=1")
                                        oResponseMessage.data_table.addCommand(-3, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DLSTMode=1")

                                        oResponseMessage.data_table.addCommand(-4, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.DSTSetting, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                                        oResponseMessage.data_table.addCommand(-5, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.DSTSetting, True)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Year, Now.Year.ToString)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.StartTime, mTerminal.DaylightSavingTime)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.EndTime, mTerminal.StandardTime)
                                        oResponseMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters._Loop, "1")

                                        mTerminalLogic.dlastTimeSync = Now
                                    Else
                                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                        If mTerminal.ZKInbioHeartBeatControl AndAlso mTerminal.RequestDelaySeconds <> mTerminal.DefaultRequestDelaySeconds Then
                                            'Restablezco tiempo entre solicitudes al valor por defecto
                                            mTerminal.RequestDelaySeconds = mTerminal.DefaultRequestDelaySeconds
                                            'Para que aplique, fuerzo reinicio del establecimiento de la comunicación
                                            oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                                        Else
                                            oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                        End If
                                    End If
                                    Return oResponseMessage
                                End If
                            Else
                                oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                                oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                Return oResponseMessage
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error:", ex)
                            oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_getrequest.HttpVersion)
                            oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.session_invalid
                            Return oResponseMessage
                        Finally
                            If mTerminalLogic.oCurrentTask IsNot Nothing Then mTerminalLogic.oCurrentTask = Nothing
                        End Try
                    Case MessageMxS.msgCommand.synctime
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.synctime, MessageMxS.msgTables.none, True, oIncomingMessage.data_synctime.HttpVersion)
                        oResponseMessage.data_synctimeresponse.setValue(MsgData_SyncTimeResponse.dataParameters.DateTime, clsPushProtocol.ConvertDateTime(Date.UtcNow))
                        oResponseMessage.data_synctimeresponse.setValue(MsgData_SyncTimeResponse.dataParameters.ServerTz, mTerminal.ServerTimeZone)
                        Return oResponseMessage
                    Case MessageMxS.msgCommand.commandresult
                        ' Resultado de un comando
                        mTerminalLogic.dlastReceived = Now
                        mTerminalLogic.bTerminalConnected = True
                        mTerminalLogic.ProcessCommandResult(oIncomingMessage.data_commandresult.Results)
                        ' Respuesta
                        oResponseMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, oIncomingMessage.data_commandresult.HttpVersion)
                        oResponseMessage.data_genericresponse.Status = MsgData.dataStatus.success
                        Return oResponseMessage
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roInBioTerminalManager::ProcessMessage:" + mTerminal.ToString + ":Error:", ex)
            End Try

            Return oResponseMessage
        End Function

    End Class

End Namespace