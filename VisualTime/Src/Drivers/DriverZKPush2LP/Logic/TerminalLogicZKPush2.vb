Imports System.Text
Imports System.Threading
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Comms.Base
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace BusinesProtocol

    Public Class TerminalLogicZKPush2
        Inherits CTerminalLogicBase

#Region "Constantes de los mensajes"

        Protected Const RightBtn_OK = "ok"

#End Region

#Region "Variables Protected"

        Public mTerminal As TerminalZKPush2
        Public mLastId As Integer = 0
        Public oParent As Object
        Public dLastSend As DateTime
        Public dlastReceived As DateTime
        Public dlastConnectionStatusChanged As DateTime = Now
        Public dlastConfigChangesCheck As DateTime = Now
        Public LastMessage As MessageZKPush2
        Public mCurrentEmployee As clsTerminalEmployee
        Public mCurrentPunch As clsTerminalPunch
        Public mSendMessage As MessageZKPush2
        Public _RegistryRoot As String
        Public oSettings As roSettings = New roSettings
        Public mConfigPath As String
        Public oCurrentTask As roTerminalsSyncTasks
        Public iTasksInMessage As Integer = 0
        Public bNeedReInitialization As Boolean = False
        Public oCheckConnectionThread As Thread
        Public bTerminalConnected As Boolean = False
        Public dlastTimeSyncLog As DateTime
        Public bError As Boolean = False
        Public bSavePunch As Boolean = True
        Public bTerminalInDST As Boolean = False
        Public Language As New VTBase.roLanguage()
        Public bGetFirmversion As Boolean = False
        Public bGetSecurityOptions As Boolean = False
        Public bSetAdminUser As Boolean = False
        Public bSynchronizing As Boolean = False
        Public bRebootPending As Boolean = False
        Public bConfigChangesPending As Boolean = False
        Public bRunningWithoutAdmin As Boolean = False
        Public bForceInitSession As Boolean = False
        Public dLastTerminalMenuEntered As DateTime = Now.AddMinutes(-2)

        'Gestión de log de eventos
        Public params() As String = Array.CreateInstance(GetType(String), 100)
        Public iEvent As CTerminalLogicBase.mxEventID
        Public sDetail As String = ""

        Public hEmployeeCache As New Hashtable
        Public dTasksCadence As New Dictionary(Of String, Integer)
        Public iMessagesDelay As Integer = 15
        Public mCardType As clsParameters.eCardType = clsParameters.eCardType.Unique

        Public mUniqueCardBytesRead As Integer = 3

#End Region

        Public Property NeedReInitialization As Boolean
            Get
                Return bNeedReInitialization
            End Get
            Set(value As Boolean)
                bNeedReInitialization = value
            End Set
        End Property

        Public Sub New(oTerminal As roTerminal)
            MyBase.New("ZKPush2", "online")
            dlastTimeSyncLog = Now.AddMinutes(-15)
            mTerminal = New TerminalZKPush2("online", oTerminal, roLog.GetInstance())
            dlastTimeSyncLog = Now.AddMinutes(-15)
        End Sub

        Public Overrides Function Initialize(strSN As String, strIP As String, strPort As String, strModel As String) As Boolean
            Try

                roTrace.GetInstance.InitTraceEvent()

                Dim bResponse As Boolean = False

                If mTerminal.DBTerminal Is Nothing Then
                    mTerminal.DBTerminal = Terminal.roTerminal.GetTerminalBySerialNumber(strSN, New roTerminalState())
                End If

                ' Informamos parámetros de terminal
                mTerminal.SN = strSN
                mTerminal.IP = strIP
                mTerminal.Port = strPort
                ' Cargo modelo de terminal, para diferenciar de momento entre rxCP y rxCeP
                mTerminal.Model = strModel

                'Cargo parámetros de CommsDriversList. En MT, sólo las tarjetas, y son parámetros avanzados
                Dim sCardType = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("MT.PUSHTerminals.CardType", Nothing))
                If sCardType = "" Then sCardType = "Unique"

                Select Case sCardType.ToLower
                    Case "1", "unique"
                        mCardType = clsParameters.eCardType.Unique
                    Case "2", "mifare"
                        mCardType = clsParameters.eCardType.Mifare
                    Case "3", "hid"
                        mCardType = clsParameters.eCardType.HID
                    Case "4", "uniquenumeric"
                        mCardType = clsParameters.eCardType.UniqueNumeric
                    Case "5", "mifarenumeric"
                        mCardType = clsParameters.eCardType.MifareNumeric
                    Case Else
                        mCardType = clsParameters.eCardType.Unique
                End Select

                'El rxTe debe leer 4 bytes, el resto 3. Solo para Unique y UniqueNumeric
                'Anulado!. Vuelve a leer 3
                If (mCardType = clsParameters.eCardType.UniqueNumeric OrElse mCardType = clsParameters.eCardType.Unique) AndAlso mTerminal.Model.ToUpper = "RXTE" Then
                    mUniqueCardBytesRead = 3
                End If

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::Initialize:Driver" + MyBase.Driver + "(" +
                                System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion +
                                ") inicialized: " +
                                "Terminal: " + mTerminal.ToString)

                ' Aquí llega la info de número de lectores. Doy de alta el terminal si no lo estaba ya
                If mTerminal.SN.Trim <> "" Then
                    mConfigPath = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" + mTerminal.ID.ToString + "\config.dat"
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::Initialize:" + mTerminal.ToString + ":Terminal accepted")
                    bResponse = CheckTerminalDB(mTerminal.SN)
                    MyBase.intTerminalID = mTerminal.ID
                    MyBase.strTerminalLocation = mTerminal.IP
                    MyBase.strTerminalType = mTerminal.TerminalType
                    If bResponse Then
                        oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                        oCurrentTask.ResetAll()
                        mTerminal.UpdateStatus(True)
                        Me.NeedReInitialization = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::Initialize:" + mTerminal.ToString + ":Terminal identified")
                    End If
                    Return bResponse
                Else
                    Return False
                End If


            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::Initialize:" + mTerminal.ToString + ":Error:", ex)
                Return False
            Finally
                roTrace.GetInstance.AddTraceEvent("TerminalLogicZKPUSH initialized")
            End Try
        End Function

        Public Function ReInitialize() As Boolean
            Try
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::ReInitialize:Driver" + MyBase.Driver + "(" +
                                System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion +
                                ") reinicialized: " +
                                "Terminal: " + mTerminal.ToString + ". Communication restablished!")
                oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                oCurrentTask.ResetAll()
                Me.NeedReInitialization = False
                mTerminal.LoadRegistrationCode()
                mTerminal.UpdateStatus(True)
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::ReInitialize:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        Public Sub CheckConnectionStatusOnIIS()
            Try
                'If Math.Abs(Now.Subtract(dlastReceived).TotalSeconds) > 90 Then
                '    ' Llevo 120 segundos sin recibir mensajes ... (no bajar de 30, porque ese es tiempo entre getrequest en algunos terminales....)
                '    If bTerminalConnected Then
                '        mTerminal.UpdateStatus(False)
                '        dlastConnectionStatusChanged = Now
                '    End If
                '    bTerminalConnected = False
                'Else
                '    If (Not bTerminalConnected) OrElse Math.Abs(Now.Subtract(dlastConnectionStatusChanged).TotalSeconds) > 20 Then
                '        mTerminal.UpdateStatus(True)
                '        dlastConnectionStatusChanged = Now
                '    End If
                '    bTerminalConnected = True
                'End If

                ' Si cambio el comportamiento del terminal, lo recargo ahora
                If bTerminalConnected AndAlso Math.Abs(Now.Subtract(dlastConfigChangesCheck).TotalSeconds) > 300 Then
                    If mTerminal.DBTerminal IsNot Nothing AndAlso roTerminal.GetForceConfig(mTerminal.DBTerminal.SerialNumber, New roTerminalState(-1)) Then
                        Dim oTermState = New roTerminalState(-1)
                        If mTerminal.TimeZoneName <> mdPublic.GetZKPUSHTerminalTimeZoneName(mTerminal.DBTerminal.SerialNumber) OrElse mdPublic.GetZKPUSHTerminalsRequestDelay <> mTerminal.RequestDelaySeconds Then
                            Me.bForceInitSession = True
                        End If
                        mTerminal.Load()
                        bConfigChangesPending = True
                        roTerminal.ClearForceConfig(mTerminal.ID, oTermState)
                    End If

                    dlastConfigChangesCheck = Now
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::CheckConnectionStatusOnIIS:" + mTerminal.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Overrides Sub Abort()

        End Sub

        Public Overrides Sub Close()

        End Sub

        Public Overrides Function GetInitMessage() As CMessageBase
            Return Nothing
        End Function

        Public Overrides Sub Interrupt()

        End Sub

        Public Overrides Function ModeDebugInfo() As String
            Return String.Empty
        End Function

        Public Overrides Sub ResumeStart()

        End Sub

        Public Overrides Sub TerminalTask(_IDEmployee As Integer, _Action As CTerminalLogicBase.TerminalTaskAction, Optional _IDTerminal As Integer = -1)

        End Sub

        Public Overrides Function ParseMessage(ByRef bInput() As Byte) As Robotics.Comms.Base.CMessageBase
            Try
                If bInput.Length > 0 Then
                    Return New MessageZKPush2(bInput)
                Else
                    Return New MessageZKPush2
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::ParseMessage:" + mTerminal.ToString + ":Error:", ex)
                Return New MessageZKPush2
            End Try
        End Function

        ''' <summary>
        ''' Cargamos datos del fichaje
        ''' </summary>
        ''' <param name="oPunch"></param>
        ''' <param name="bSavePunch"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function LoadCurrentData(ByVal oPunch As MsgData_Table_punch, ByRef bSavePunch As Boolean) As Boolean
            Try
                'En principio guardo el fichaje
                bSavePunch = True

                'Cargamos en memoria los datos del empleado y del marajes
                mCurrentEmployee = New clsTerminalEmployee()

                'Si se identificó al empleado
                If oPunch.PIN > 0 Then
                    mCurrentEmployee.Load(oPunch.PIN)
                    mCurrentPunch = New clsTerminalPunch(mTerminal, oPunch.PunchDateTime, oPunch.PIN, "", 1, oPunch.PunchStatus, roTypes.Any2Integer(oPunch.WorkCode), oPunch.VerifyMode, oPunch.WearingMask, oPunch.Temperature, Nothing)

                    If mCurrentEmployee.ID > 0 Then
                        mCurrentPunch.IDEmployee = mCurrentEmployee.ID
                        If mCurrentPunch.Card.Length = 0 Then mCurrentPunch.Card = mCurrentEmployee.Card
                        mCurrentPunch.Load()
                    End If

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::LoadCurrentData:" + mTerminal.ToString + ":Punch received:" & oPunch.ToLogInfo)

                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::LoadCurrentData:Exception on terminal " + mTerminal.ToString + ":Error:", ex)
                bSavePunch = False
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Cargamos datos del fichaje
        ''' </summary>
        ''' <param name="oPunch"></param>
        ''' <param name="bSavePunch"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function LoadCurrentDataIIS(ByVal oPunch As MsgData_Table_punch, ByRef bSavePunch As Boolean, ByRef oCurrentPunch As clsTerminalPunch) As Boolean
            Try
                'En principio guardo el fichaje
                bSavePunch = True

                'Cargamos en memoria los datos del empleado y del marajes
                mCurrentEmployee = New clsTerminalEmployee()

                'Si se identificó al empleado
                If oPunch.PIN > 0 Then
                    mCurrentEmployee.Load(oPunch.PIN)
                    oCurrentPunch = New clsTerminalPunch(mTerminal, oPunch.PunchDateTime, oPunch.PIN, "", 1, oPunch.PunchStatus, roTypes.Any2Integer(oPunch.WorkCode), oPunch.VerifyMode, oPunch.WearingMask, oPunch.Temperature, Nothing)
                    If mCurrentEmployee.ID > 0 Then
                        oCurrentPunch.IDEmployee = mCurrentEmployee.ID
                        If oCurrentPunch.Card.Length = 0 Then oCurrentPunch.Card = mCurrentEmployee.Card
                        oCurrentPunch.Load()
                    End If

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::LoadCurrentDataIIS:" + mTerminal.ToString + ":Punch received:" & oPunch.ToLogInfo)

                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::LoadCurrentDataIIS:Exception on terminal " + mTerminal.ToString + ":Error:", ex)
                bSavePunch = False
                Return False
            Finally
                roTrace.GetInstance.AddTraceEvent("Currrent punch data loaded")
            End Try
        End Function

        Protected Overridable Function CheckTerminalValid(ByVal oMessage As MessageZKPush2) As Boolean
            Try
                Return oMessage.SN <> ""
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::CheckTerminalValid:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Comprueba si el terminal existe y esta bien registrado en la base de datos.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function CheckTerminalDB(Optional ByVal DBTerminalSN As String = "") As Boolean
            Try
                If mTerminal.Exist Then
                    Return mTerminal.Load
                Else
                    If mTerminal.CreateNew() Then
                        If mTerminal.Load() Then
                            'Hemos creado el terminal. Me aseguro que se programa desde cero
                            mTerminal.ForceFullDataSync()
                            CallBroadcaster(mTerminal.ID)
                            Return True
                        End If
                    End If
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2" + MyBase.Driver + "::CheckTerminalDB:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        Protected Sub Send(ByVal oMessage As MessageZKPush2)
            Try
                Try
                    'Hacemos una pequeña parada porque un envio inmediato no funciona
                    System.Threading.Thread.Sleep(300)
                    dLastSend = Now
                    ' Ajuste de hora de terminal si está en otro uso horario que el servidor
                    'If mTerminal.IsInDifferentTimeZone Then oMessage.MessageTimeStamp = mTerminal.CurrentDatetime
                    oMessage.MessageTimeStamp = mTerminal.CurrentDatetime
                    MyBase.RaiseOnSendMessageEx(oMessage, mTerminal.SN, Me, False)
                    mSendMessage = Nothing
                    'Historial de los dos ultimos mensaje
                    LastMessage = oMessage
#If DEBUG Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::Send:Message:" + vbCrLf + oMessage.toString)
#End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Send:" + mTerminal.ToString + ": Error: '" + ex.Message + "'")
                End Try
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Send:" + mTerminal.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Function ProcessOperationLog(ByVal oOperLog As MsgData_Operation) As Boolean
            ' Proceso los distintos mensajes recibidos en el log de operaciones
            Dim bLaunchBroadcaster As Boolean = False

            Try
                mCurrentEmployee = New clsTerminalEmployee()
                For Each oEmployee As MsgData_Table_employee In oOperLog.Employees

                Next

                For Each oOperation As MsgData_Table_operation In oOperLog.OperationLog
                    ' Log genérico
                    If [Enum].IsDefined(GetType(clsPushProtocol.dataOperations), oOperation.OperationID) Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + " reports operation: " & oOperation.Tostring)
                    End If
                    Select Case oOperation.OperationID
                        Case clsPushProtocol.dataOperations.PowerOff
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + " was shutdown by user " + oOperation.AdminID + " at " + oOperation.OperationTime.ToShortDateString)
                        Case clsPushProtocol.dataOperations.PowerOn
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + " powered on by user " + oOperation.AdminID + " at " + oOperation.OperationTime.ToShortDateString)
                        Case clsPushProtocol.dataOperations.VerificationFail
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Verification fail on terminal " + mTerminal.ID.ToString + " for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToShortDateString)
                        Case clsPushProtocol.dataOperations.Alarm
                            Dim eAlarmCode As clsPushProtocol.dataAlarmCodes = oOperation.OperationParameter1
                            Dim strAlarmDesc As String
                            If IsNumeric(oOperation.OperationParameter1) Then
                                strAlarmDesc = eAlarmCode.ToString
                            Else
                                strAlarmDesc = oOperation.OperationParameter1
                            End If
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": Alarm " + strAlarmDesc + " received at " + oOperation.OperationTime.ToString)
                        Case clsPushProtocol.dataOperations.EnterMenu
                            dLastTerminalMenuEntered = Now
                            bGetSecurityOptions = True
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " entered admin menu at " + oOperation.OperationTime.ToString)
                        Case clsPushProtocol.dataOperations.ChangeSetting
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed settings at " + oOperation.OperationTime.ToString + ". Parameter: " + oOperation.OperationParameter1 + " value: " + oOperation.OperationParameter2)
                        Case clsPushProtocol.dataOperations.ModifyUserName
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed some data on user " + oOperation.OperationParameter2 + " at " + oOperation.OperationTime.ToString)
                        Case clsPushProtocol.dataOperations.AddUser
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " added user " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString + ". User will be deleted!")
                            If mTerminal.Model <> "RXCEP" Then
                                ' Se ha añadido un empleado directamente en el terminal. Fuerzo reprogramación del mismo.
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: User " + oOperation.OperationParameter2 + "added on terminal " + mTerminal.ID.ToString + ". User will be deleted")
                                Dim idNewEmployeeId As Long
                                idNewEmployeeId = roTypes.Any2Long(oOperation.OperationParameter1)
                                If idNewEmployeeId > 0 Then
                                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delemployee, roTypes.Any2Long(idNewEmployeeId), True)
                                End If
                                Dim oAudit As New roAudit()
                                oAudit.Action = Audit.Action.aInsert
                                oAudit.ClientLocation = "Terminal ID " & mTerminal.ID
                                Dim oEmployee As VTEmployees.Employee.roEmployee
                                oEmployee = VTEmployees.Employee.roEmployee.GetEmployee(idNewEmployeeId, New VTEmployees.Employee.roEmployeeState(-1))
                                If Not oEmployee Is Nothing Then
                                    oAudit.ObjectName = oEmployee.Name
                                Else
                                    oAudit.ObjectName = "Unknown"
                                End If
                                oAudit.ObjectType = Audit.ObjectType.tEmployeeOnTerminal
                                oAudit.UserName = "System"
                                oAudit.SaveAudit()
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. User " + oOperation.OperationParameter2 + "added on terminal. Forcing terminal full reset for terminal " + mTerminal.ID.ToString)
                            End If
                        Case clsPushProtocol.dataOperations.AddFinger
                            If mTerminal.Model <> "RXCEP" AndAlso mTerminal.Model <> "RX1" Then
                                ' Se registró una huella. La busco en el array de huellas
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " added a fingerprint for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString + ". FPSize=" + oOperation.OperationParameter3 + ". FP=" + oOperation.OperationParameter2)
                                For Each oFinger As MsgData_Table_finger In oOperLog.Fingers
                                    If oOperation.OperationParameter1 = oFinger.PIN AndAlso oOperation.OperationParameter3 = oFinger.FID Then
                                        oFinger.TimeStamp = oOperation.OperationTime
                                        oFinger.EnrollerID = oOperation.AdminID
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " added a fingerprint for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString + ". FPSize=" + oOperation.OperationParameter3 + ". FP=" + oOperation.OperationParameter2)
                                        If Not ProcessOperationLog_AddFinger(oFinger, bLaunchBroadcaster) Then
                                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ". Error processing finger " + oFinger.FID + " for employee " + oFinger.PIN)
                                        End If
                                        Exit For
                                    End If
                                Next
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": Operation ignored beacuse terminal does not allow this operation. User " + oOperation.AdminID + " added a fingerprint for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString + ". FPSize=" + oOperation.OperationParameter3 + ". FP=" + oOperation.OperationParameter2)
                            End If
                        Case clsPushProtocol.dataOperations.AddCard
                            If mTerminal.Model <> "RXCEP" AndAlso mTerminal.Model <> "RX1" Then
                                Dim sCard As String = ""
                                If Not ProcessOperationLog_Card(oOperation, bLaunchBroadcaster, sCard) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ". Error processing card " + sCard + " for employee " + oOperation.OperationParameter4)
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. Card added on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.AddPassword
                            If mTerminal.Model <> "RXCEP" Then
                                For Each oEmployee As MsgData_Table_employee In oOperLog.Employees
                                    If oEmployee.PIN = oOperation.OperationParameter1 Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " added PIN " + oEmployee.Password + " for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString)
                                        If Not ProcessOperationLog_Password(oEmployee, oOperation, bLaunchBroadcaster) Then
                                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ". Error processing password " + oEmployee.Password + " for employee " + oEmployee.PIN)
                                        End If
                                        Exit For
                                    End If
                                Next
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. Password added on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.DelUser, clsPushProtocol.dataOperations.DelCard, clsPushProtocol.dataOperations.DelPassword, clsPushProtocol.dataOperations.DeletePersonnelPermissions
                            If mTerminal.Model <> "RXCEP" Then
                                Dim idEmployee As Integer = roTypes.Any2Long(oOperation.OperationParameter1)
                                If idEmployee <> 0 AndAlso mTerminal.EmployeeHasPermit(idEmployee) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: User/Card/Pwd for employee " + oOperation.OperationParameter1 + " deleted on terminal " + mTerminal.ID.ToString + ". It will be added again")
                                    mTerminal.DeleteEmployeeOnterminal(idEmployee)
                                    mdPublic.CallBroadcaster(mTerminal.ID, , idEmployee)
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. Employee deleted on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.DelFinger
                            If mTerminal.Model <> "RXCEP" Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " deleted finger " + " - " + " for employee " + oOperation.OperationParameter1 + " at " + oOperation.OperationTime.ToString + ". Finger will be erased from database!")
                                If Not ProcessOperationLog_DelFinger(roTypes.Any2Integer(oOperation.OperationParameter1), roTypes.Any2Integer(oOperation.OperationParameter3), oOperation.OperationTime, bLaunchBroadcaster) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ". Error deleting finger " + oOperation.OperationParameter3 + " for employee " + oOperation.OperationParameter1)
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. Finger deleted on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.PurgeData
                            ' Si han borrado todo, vuelvo a programar el terminal de cero
                            If oOperation.OperationParameter1 = "0" AndAlso (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper = "RX1") Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: All data deleted on terminal " + mTerminal.ID.ToString + ". Terminal will be programmed from scratch")
                                mTerminal.ForceFullDataSync()
                                mdPublic.CallBroadcaster(mTerminal.ID)
                            End If
                        Case clsPushProtocol.dataOperations.SetTime
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed terminal time at " + oOperation.OperationTime.ToString + ". It will be synchornized back in less than a minute!")
                        Case clsPushProtocol.dataOperations.FactorySetting
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " set factory setings for terminal. Time " + oOperation.OperationTime.ToString + ". Some puches and everything else could be lost!")
                        Case clsPushProtocol.dataOperations.DeleteAttRecords
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " deleted al punches on terminal. Time " + oOperation.OperationTime.ToString + ". Some puches could be lost!")
                        Case clsPushProtocol.dataOperations.CleanAdministrationPrivilege
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " cleaned administration privileges at " + oOperation.OperationTime.ToString + ". They will be reestablished back in less than a minute!")
                        Case clsPushProtocol.dataOperations.ChgAccControl
                            If mTerminal.Model <> "RXCEP" Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed terminal access control parameters at " + oOperation.OperationTime.ToString)
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. AccControl changed on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.ChgUserControl
                            If mTerminal.Model <> "RXCEP" Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed terminal user control parameters at " + oOperation.OperationTime.ToString)
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. UserControl changed on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.ChgTimeControl
                            If mTerminal.Model <> "RXCEP" Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " changed terminal time control parameters at " + oOperation.OperationTime.ToString)
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Operation ignored beacuse terminal does not allow this operation. TimeControl changed on terminal")
                            End If
                        Case clsPushProtocol.dataOperations.DoorSensorClosed
                            ' Señal de cierre de relé tras apertura
                            ' IDN El Pozo. Para ciertos terminales, los AV los guardo como AI hasta que recibo esta señal.
                            Try
                                Dim customization As String = Common.roBusinessSupport.GetCustomizationCode().ToUpper()
                                If roTypes.Any2String(customization) = "OZOPLE" Then
                                    Dim sACCTerminalsToCheckRelayClose As String = String.Empty
                                    sACCTerminalsToCheckRelayClose = roTypes.Any2String(Common.AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("ACCTerminalsToCheckRelayClose", Nothing))
                                    ' 1.- El terminal es uno de los configurados ...
                                    If sACCTerminalsToCheckRelayClose.Trim.Length > 0 AndAlso sACCTerminalsToCheckRelayClose.Split(",").Contains(mTerminal.ID.ToString) Then
                                        Dim strSQL As String = String.Empty
                                        strSQL = "@SELECT# TOP(1) ID FROM Punches WHERE IDterminal = " & mTerminal.ID.ToString & " AND Type = 6 AND InvalidType = 20 AND DateTime BETWEEN " & roTypes.Any2Time(oOperation.OperationTime.AddSeconds(-1 * mTerminal.RDROpenTime(1))).SQLDateTime & " AND " & roTypes.Any2Time(oOperation.OperationTime).SQLDateTime & " ORDER BY DateTime DESC"
                                        Dim iID As Integer = 0
                                        iID = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                                        If iID > 0 Then
                                            Dim bolRet As Boolean = False
                                            strSQL = "@UPDATE# Punches SET Type = 5, ActualType = 5, InvalidType = NULL, Field1 = 'Completed at " & oOperation.OperationTime.ToString & "' WHERE ID = " & iID.ToString
                                            DataLayer.AccessHelper.ExecuteSql(strSQL)
                                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog::One punch in last " & mTerminal.RDROpenTime(1).ToString & " seconds completed on close relay signal detected for terminal " & mTerminal.ToString)
                                        End If
                                    End If
                                End If
                            Catch ex As Exception
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog::Unknownerror checking DoorSensor signal. Skipped", ex)
                            End Try
                        Case Else
                            If [Enum].IsDefined(GetType(clsPushProtocol.dataOperations), oOperation.OperationID) Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " & mTerminal.ID.ToString & ": Unknown operation event: " & oOperation.Tostring + " at " & oOperation.OperationTime & ". Parameters (1 to 4): " & oOperation.OperationParameter1 & "," & oOperation.OperationParameter2 & "," & oOperation.OperationParameter3 & "," & oOperation.OperationParameter4)
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " & mTerminal.ID.ToString & ": Unknown operation event: " & oOperation.OperationID.ToString & " at " & oOperation.OperationTime & ". Parameters (1 to 4): " & oOperation.OperationParameter1 & "," & oOperation.OperationParameter2 & "," & oOperation.OperationParameter3 & "," & oOperation.OperationParameter4)
                            End If
                    End Select
                Next

                If mTerminal.Model = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper Then
                    ' Proceso huellas de manera independiente (en F22 pueden llegar por separado de los OPLOGs correspondientes (ID=6)
                    For Each oFinger As MsgData_Table_finger In oOperLog.Fingers
                        oFinger.TimeStamp = Now
                        oFinger.EnrollerID = mTerminal.AdminID
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User 9999 added a fingerprint for employee " + oFinger.PIN + " at " + Now.ToShortDateString + ". FPSize=" + oFinger.Size + ". FP=" + oFinger.TMP)
                        If Not ProcessOperationLog_AddFinger(oFinger, bLaunchBroadcaster) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ". Error processing finger " + oFinger.FID + " for employee " + oFinger.PIN)
                        End If
                    Next
                End If

                If mTerminal.Model = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                    ' Terminales compatibles con lector de tarjetas: rx1, rxFL, rxFP
                    ' Proceso tarjetas de manera independiente (en F22 pueden llegar por separado de los OPLOGs correspondientes (ID=8)
                    For Each oEmployee As MsgData_Table_employee In oOperLog.Employees
                        Dim sCard As String = ""
                        If Not ProcessOperationLog_Cardv2(oEmployee, oOperLog, bLaunchBroadcaster, sCard) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roError, $"TerminalLogicZKPush2::ProcessOperationLog:: Terminal {mTerminal.ID.ToString}. Error processing card {sCard} for employee ID {oEmployee.PIN}")
                        End If
                    Next
                End If

                'Finalmente, si es necesario, lanzo Broadcaster
                If bLaunchBroadcaster Then CallBroadcaster()

                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessBiodataLog(ByVal oBiodataLog As MsgData_Biodata) As Boolean
            ' Proceso los distintos mensajes recibidos en el log de biodata (información de biometría unificada
            Dim bLaunchBroadcaster As Boolean = False

            Try
                mCurrentEmployee = New clsTerminalEmployee()

                ' Proceso caras
                For Each oBiodata As MsgData_Table_biodata In oBiodataLog.Bioelements
                    oBiodata.TimeStamp = Now
                    Select Case oBiodata.Type
                        Case "9"
                            ' Es una cara
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog:: Terminal " + mTerminal.ID.ToString + ": User 9999 added a face for employee " + oBiodata.PIN + " at " + Now.ToShortDateString + ". FaceTMP=" + oBiodata.TMP)
                        Case "8"
                            ' Es una palma de la mano
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog:: Terminal " + mTerminal.ID.ToString + ": User 9999 added a face for employee " + oBiodata.PIN + " at " + Now.ToShortDateString + ". FaceTMP=" + oBiodata.TMP)
                    End Select
                    If Not ProcessBiodataLog_AddBiodata(oBiodata, bLaunchBroadcaster) Then
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessBiodataLog:: Terminal " + mTerminal.ID.ToString + ". Error processing face " + oBiodata.Index + " for employee " + oBiodata.PIN)
                    End If
                Next

                'Finalmente, si es necesario, lanzo Broadcaster
                If bLaunchBroadcaster Then CallBroadcaster()

                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessOperationLog_Password(oEmployee As MsgData_Table_employee, oOperation As MsgData_Table_operation, ByRef bLaunchBroadcaster As Boolean) As Boolean
            Try
                If mCurrentEmployee.Load(oOperation.OperationParameter1) Then
                    mCurrentEmployee.PINDateStamp = oOperation.OperationTime
                    If mCurrentEmployee.Card.Trim <> "" Then
                        If mCurrentEmployee.SavePIN(oEmployee.Password) Then
                            bLaunchBroadcaster = True
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Password updated!.")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Error updating password. Password on terminal will be restored!.")
                            mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addemployee, mCurrentEmployee.ID)
                        End If
                    Else
                        ' Verifico que si tiene password asignado, la fecha es posterior a la que estoy recibiendo ahora. Si no, debo volver a programar el password del empleado.
                        If mCurrentEmployee.PINDateStamp < oOperation.OperationTime Then
                            If mCurrentEmployee.SavePIN(oEmployee.Password) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Password updated!.")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Error updating password. Password on terminal will be deleted!.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addemployee, mCurrentEmployee.ID)
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Employee has a newer password assigned. Password discarted!.")
                            mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addemployee, mCurrentEmployee.ID)
                        End If
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Error loading employee info. Password on terminal will be restored!.")
                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addemployee, mCurrentEmployee.ID)
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Password:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessOperationLog_Card(oOperation As MsgData_Table_operation, ByRef bLaunchBroadcaster As Boolean, ByRef sCardNumber As String) As Boolean
            Try
                ' Recupero código de tarjeta
                Dim lCardNumber As Long = 0
                Dim sCard As String
                lCardNumber = roTypes.Any2Long(oOperation.OperationParameter1 * 16 * 16 * 16 * 16) + roTypes.Any2Long(oOperation.OperationParameter2)
                sCardNumber = lCardNumber.ToString

                sCard = mdPublic.ConvertCardForDatabase(lCardNumber.ToString, mCardType)

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Terminal " + mTerminal.ID.ToString + ": User " + oOperation.AdminID + " added card " + lCardNumber.ToString + " for employee " + oOperation.OperationParameter4 + " at " + oOperation.OperationTime.ToString)
                ' Verifico que la tarjeta no está y aen uso, y que el empleado tiene permiso para fichar por tarjeta
                If Not roPassportManager.CredentialExists(sCard, AuthenticationMethod.Card, "", 0) Then
                    If mCurrentEmployee.Load(oOperation.OperationParameter4) Then
                        If mCurrentEmployee.Card.Trim = "" Then
                            If mCurrentEmployee.SaveCard(sCard) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Card updated!.")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Error updating card. Card on terminal will be restored!.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                            End If
                        Else
                            ' Verifico que si tiene tarjeta asignada, la fecha es posterior a la que estoy recibiendo ahora. Si no, debo volver a programar la tarjeta del empleado.
                            If mCurrentEmployee.CardDateStamp < oOperation.OperationTime Then
                                If mCurrentEmployee.SaveCard(sCard) Then
                                    bLaunchBroadcaster = True
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Card updated!.")
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Error updating card. Card on terminal will be deleted!.")
                                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Employee has a newer card assigned. Card discarted!.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                            End If
                        End If
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Error loading employee info. Card on terminal will be restored!.")
                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                    End If
                Else
                    ' Se debe volver a enviar la tarjeta del empleado al terminal
                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Card already assigned to another employee. Card discarted!.")
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Card:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessOperationLog_Cardv2(oEmployee As MsgData_Table_employee, oOperLog As MsgData_Operation, ByRef bLaunchBroadcaster As Boolean, ByRef sCard As String) As Boolean
            Try
                ' Recupero código de tarjeta
                sCard = mdPublic.ConvertCardForDatabase(oEmployee.Card, mCardType)

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Terminal " + mTerminal.ID.ToString + ": User admin added card " + sCard + " for employee " + oEmployee.PIN)
                ' Verifico que la tarjeta no está y aen uso, y que el empleado tiene permiso para fichar por tarjeta
                If Not roPassportManager.CredentialExists(sCard, AuthenticationMethod.Card, "", 0) Then
                    If mCurrentEmployee.Load(oEmployee.PIN) Then
                        If mCurrentEmployee.Card.Trim = "" Then
                            If mCurrentEmployee.SaveCard(sCard) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Card updated!.")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Error updating card. Card on terminal will be restored!.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                            End If
                        Else
                            ' Verifico que si tiene tarjeta asignada, la fecha es posterior a la que estoy recibiendo ahora. Si no, debo volver a programar la tarjeta del empleado.
                            ' TODO: Debo poder recupera la fecha y hora en que se grabó la tarjeta
                            If mCurrentEmployee.SaveCard(sCard) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Card updated!.")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Error updating card. Card on terminal will be deleted!.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                            End If
                        End If
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Error loading employee info. Card on terminal will be restored!.")
                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                    End If
                Else
                    ' Se debe volver a enviar la tarjeta del empleado al terminal
                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addcard, mCurrentEmployee.ID)
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Card already assigned to another employee. Card discarted!.")
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Cardv2:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessOperationLog_DelFinger(iEmployeeId As Integer, iIDFinger As Byte, dTimeStamp As DateTime, ByRef bLaunchBroadcaster As Boolean) As Boolean
            Try
                If mCurrentEmployee.Load(iEmployeeId) Then
                    If mCurrentEmployee.BioTimeStamp(iIDFinger) < dTimeStamp Then
                        'Si la bbdd tiene una huella eliminamos anterior
                        mCurrentEmployee.BioData(iIDFinger) = Array.CreateInstance(GetType(Byte), 0)
                        mCurrentEmployee.BioTimeStamp(iIDFinger) = dTimeStamp
                        mCurrentEmployee.SaveFinger(iIDFinger, mTerminal.ID)
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_DelFinger:" + mTerminal.ToString + ":Finger deleted for employee " & mCurrentEmployee.Name & " and Finger id " & iIDFinger.ToString)
                        bLaunchBroadcaster = True
                    Else
                        'Si es mas antigua la volvemos a subir al terminal
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_DelFinger:" + mTerminal.ToString + ":Finger deleted has a newer version on database for employee " & mCurrentEmployee.Name & ". Finger will be sent to terminal again")

                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addbio, iEmployeeId, True)
                    End If
                End If

                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_DelFinger:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessOperationLog_AddFinger(oFinger As MsgData_Table_finger, ByRef bLaunchBroadcaster As Boolean) As Boolean
            Try
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:: Terminal " + mTerminal.ID.ToString + ": User " + oFinger.EnrollerID.ToString + " added a fingerprint for employee " + oFinger.PIN + ". FPSize=" + oFinger.Size + ". FP=" + oFinger.TMP)
                If mCurrentEmployee.Load(oFinger.PIN) Then
                    If mCurrentEmployee.HasBio(oFinger.FID) Then
                        If mCurrentEmployee.BioTimeStamp(oFinger.FID) < oFinger.TimeStamp Then
                            ' La huella del terminal es más reciente que la de la BBDD. Guardo la recibida.
                            mCurrentEmployee.BioData(oFinger.FID) = Encoding.UTF8.GetBytes(oFinger.TMP)
                            mCurrentEmployee.BioTimeStamp(oFinger.FID) = oFinger.TimeStamp
                            If mCurrentEmployee.SaveFinger(oFinger.FID, mTerminal.ID) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Finger changed for employee " + oFinger.PIN + ", finger id " + oFinger.FID)
                                If Not mCurrentEmployee.AllowBio Then
                                    'Si no tiene permisos para fichar por huella la borramos del terminal
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. Finger will be erased from this device")
                                    mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, mCurrentEmployee.ID, True, oFinger.FID)
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Erorr saving biometric info for employee " & mCurrentEmployee.Name)
                            End If
                        Else
                            ' La huella de la BBDD es más reciente
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. Finger will be erased from this device")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, mCurrentEmployee.ID, True, oFinger.FID)
                                bLaunchBroadcaster = True
                            Else
                                'Si tiene una fecha anterior generamos tarea para que vuelva a subir la huella
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Employee has the latest biometric information on database.")
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addbio, mCurrentEmployee.ID, True, oFinger.FID)
                                bLaunchBroadcaster = True
                            End If
                        End If
                    Else
                        'Si la bbdd no tiene una huella guardamos la nueva
                        mCurrentEmployee.BioData(oFinger.FID) = Encoding.UTF8.GetBytes(oFinger.TMP)
                        mCurrentEmployee.BioTimeStamp(oFinger.FID) = oFinger.TimeStamp
                        If mCurrentEmployee.SaveFinger(oFinger.FID, mTerminal.ID) Then
                            bLaunchBroadcaster = True
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Finger changed for employee " + oFinger.PIN + ", finger id " + oFinger.FID)
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. Finger will be erased from this device")
                                'XAVI mTerminal.DeleteBiometricFile()
                                mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, mCurrentEmployee.ID, True, oFinger.FID)
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + $":Finger for employee {mCurrentEmployee.Name} could not be saved")
                        End If
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog:: Terminal " + mTerminal.ID.ToString + ": User " + oFinger.EnrollerID.ToString + " added a fingerprint for employee " + oFinger.PIN + ". FPSize=" + oFinger.Size + ". FP=" + oFinger.TMP + ". Error loading employee info. Finger discarted!")
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessOperationLog_Finger:: Error:", ex)
                Return False
            End Try
        End Function

        Public Function ProcessBiodataLog_AddBiodata(oBio As MsgData_Table_biodata, ByRef bLaunchBroadcaster As Boolean) As Boolean
            Try
                Dim sBioTypeForLog As String = String.Empty
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:: Terminal " + mTerminal.ID.ToString + ": User 9999 added a biodata for employee " + oBio.PIN + ". FaceTMP=" + oBio.TMP)
                If mCurrentEmployee.Load(oBio.PIN) Then
                    ' En función del tipo de biometría del que se trate
                    Dim dBioTimeStamp As Date
                    Dim bBioAlreadyRegistered As Boolean
                    Select Case oBio.Type
                        Case "9"
                            dBioTimeStamp = mCurrentEmployee.BioFaceTimeStamp(oBio.Index)
                            bBioAlreadyRegistered = mCurrentEmployee.HasFace(oBio.Index)
                            sBioTypeForLog = "face"
                        Case "8"
                            dBioTimeStamp = mCurrentEmployee.BioPalmTimeStamp(oBio.Index)
                            bBioAlreadyRegistered = mCurrentEmployee.HasPalm(oBio.Index)
                            sBioTypeForLog = "palm"
                    End Select

                    If bBioAlreadyRegistered Then
                        If dBioTimeStamp < oBio.TimeStamp Then
                            ' La huella del terminal es más reciente que la de la BBDD. Guardo la recibida.
                            Select Case oBio.Type
                                Case "9"
                                    mCurrentEmployee.BioFaceData(oBio.Index) = Encoding.UTF8.GetBytes(oBio.TMP)
                                    mCurrentEmployee.BioFaceTimeStamp(oBio.Index) = oBio.TimeStamp
                                Case "8"
                                    mCurrentEmployee.BioPalmData(oBio.Index) = Encoding.UTF8.GetBytes(oBio.TMP)
                                    mCurrentEmployee.BioPalmTimeStamp(oBio.Index) = oBio.TimeStamp
                            End Select

                            If mCurrentEmployee.SaveBiodata(oBio.Index, oBio.Type, oBio.MajorVer + "." + oBio.MinorVer, mTerminal.ID) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":" & sBioTypeForLog & " changed for employee " + oBio.PIN + ", face id " + oBio.Index)
                                If Not mCurrentEmployee.AllowBio Then
                                    'Si no tiene permisos para fichar por huella la borramos del terminal
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. " & sBioTypeForLog & " will be erased from this device")
                                    Select Case oBio.Type
                                        Case "9"
                                            mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodataface, mCurrentEmployee.ID, True, oBio.Index)
                                        Case "8"
                                            mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodatapalm, mCurrentEmployee.ID, True, oBio.Index)
                                    End Select
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + $":Face for employee {mCurrentEmployee.Name} could not be saved")
                            End If
                        Else
                            ' La cara de la BBDD es más reciente
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. " & sBioTypeForLog & " will be erased from this device")
                                Select Case oBio.Type
                                    Case "9"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodataface, mCurrentEmployee.ID, True, oBio.Index)
                                    Case "8"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodatapalm, mCurrentEmployee.ID, True, oBio.Index)
                                End Select
                                bLaunchBroadcaster = True
                            Else
                                'Si tiene una fecha anterior generamos tarea para que vuelva a subir la huella
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":Employee has the latest biometric information on database.")
                                Select Case oBio.Type
                                    Case "9"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addbiodataface, mCurrentEmployee.ID, True, oBio.Index)
                                    Case "8"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.addbiodatapalm, mCurrentEmployee.ID, True, oBio.Index)
                                End Select
                                bLaunchBroadcaster = True
                            End If
                        End If
                    Else
                        'Si la bbdd no tiene una cara guardamos la nueva
                        Select Case oBio.Type
                            Case "9"
                                mCurrentEmployee.BioFaceData(oBio.Index) = Encoding.UTF8.GetBytes(oBio.TMP)
                                mCurrentEmployee.BioFaceTimeStamp(oBio.Index) = oBio.TimeStamp
                            Case "8"
                                mCurrentEmployee.BioPalmData(oBio.Index) = Encoding.UTF8.GetBytes(oBio.TMP)
                                mCurrentEmployee.BioPalmTimeStamp(oBio.Index) = oBio.TimeStamp
                        End Select

                        If mCurrentEmployee.SaveBiodata(oBio.Index, oBio.Type, oBio.MajorVer + "." + oBio.MinorVer, mTerminal.ID) Then
                            bLaunchBroadcaster = True
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":" & sBioTypeForLog & " changed for employee " + oBio.PIN + ", face id " + oBio.Index)
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:" + mTerminal.ToString + ":Employee has no permission for using biometric identification. " & sBioTypeForLog & " will be erased from this device")
                                Select Case oBio.Type
                                    Case "9"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodataface, mCurrentEmployee.ID, True, oBio.Index)
                                    Case "8"
                                        mdPublic.CallBroadcaster(mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbiodatapalm, mCurrentEmployee.ID, True, oBio.Index)
                                End Select
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessOperationLog_AddFinger:" + mTerminal.ToString + $":Face for employee {mCurrentEmployee.Name} could not be saved")
                        End If
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:: Terminal " + mTerminal.ID.ToString + ": User 9999 added a fingerprint for employee " + oBio.PIN + ". " & sBioTypeForLog & "TMP =" + oBio.TMP + ". Error loading employee info. Face discarted!")
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessBiodataLog_AddBiodata:: Error:", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Traduce una o varias acciones de sincronización a comandos para INBIO
        ''' </summary>
        ''' <param name="oMessage"></param>
        ''' <remarks></remarks>
        Public Function GetNextCommand(ByRef oMessage As MessageZKPush2, oSyncTask As roTerminalsSyncTasks, sHttpVersion As String, ByRef bMaxTasksReached As Boolean, ByRef bCallBroadcaster As Boolean, Optional bIsIIS As Boolean = False) As Boolean

            Try
                roTrace.GetInstance.InitTraceEvent()

                mCurrentEmployee = New clsTerminalEmployee()

                If oSyncTask.IDEmployee > 0 Then
                    ' Cargo información de empleado por si es precisa
                    Dim bEmployeeLoaded As Boolean = False
                    Dim bEmployeeInfoRequired As Boolean = True
                    Dim bEmployeeExists As Boolean = True
                    bEmployeeInfoRequired = Not (oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployeetimezones OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delbio OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delcard OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployee)
                    If bEmployeeInfoRequired Then
                        ' Cargo datos del empleado si aún no los tengo
                        bEmployeeExists = mCurrentEmployee.EmployeeExists(oSyncTask.IDEmployee)
                        bEmployeeLoaded = mCurrentEmployee.Load(oSyncTask.IDEmployee)
                    End If
                    If bEmployeeLoaded OrElse Not bEmployeeInfoRequired Then
                        Select Case oSyncTask.Task
                            Case roTerminalsSyncTasks.SyncActions.addemployee, roTerminalsSyncTasks.SyncActions.addcard
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERINFO, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                If mTerminal.Model.ToUpper <> "RXCEP" Then
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Name, mdPublic.NormalizeText(mCurrentEmployee.Name))
                                Else
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Name, HashCheckSum.CalculateString(mCurrentEmployee.Name, Algorithm.MD5).Substring(0, 10))
                                End If
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Passwd, mCurrentEmployee.PIN)
                                Dim sCard As String
                                sCard = mdPublic.ConvertCardForTerminal(mCurrentEmployee.Card.ToString, mCardType, mUniqueCardBytesRead)
                                If mCardType = clsParameters.eCardType.Mifare AndAlso mCurrentEmployee.Card.Length > 28 Then
                                    ' Compatibilidad con tarjeta PassVigo (MiFare Plus de 10 bytes)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Card, sCard)
                                Else
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Card, clsPushProtocol.ConvertCard(sCard))
                                End If

                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Grp, "1") 'Empleados al grupo 1 por defecto
                                ' Marcamos que el empleado no usa los timezones de su grupo de acceso, sino los suyos propios
                                If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse
                                    mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZ, "1|0032|0000|0000")
                                Else
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZ, "0")
                                End If
                                iTasksInMessage = iTasksInMessage + 1
                                If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("employee") Then
                                    bMaxTasksReached = (iTasksInMessage = dTasksCadence("employee"))
                                Else
                                    bMaxTasksReached = (iTasksInMessage = 15)
                                End If

                                'Log
                                Try
                                    If oSyncTask.Task = roTerminalsSyncTasks.SyncActions.addcard Then
                                        params(iTasksInMessage) = mCurrentEmployee.Name + ";" + sCard
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.AddCard
                                    Else
                                        params(iTasksInMessage) = oSyncTask.IDEmployee.ToString + ";" + mCurrentEmployee.Name
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.AddEmployee
                                    End If
                                Catch ex As Exception
                                End Try
                            Case roTerminalsSyncTasks.SyncActions.addbio
                                If mCurrentEmployee.BioData(oCurrentTask.IDFinger) IsNot Nothing AndAlso mTerminal.Model.ToUpper <> eTerminalModel.rxTe.ToString.ToUpper Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.FINGERTMP, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.FID, oSyncTask.IDFinger)

                                    Dim messageByte As Byte()
                                    messageByte = Helper.Decode(mCurrentEmployee.BioData(oCurrentTask.IDFinger))

                                    Dim tmpstr As String = New String(System.Text.Encoding.UTF8.GetChars(messageByte))
                                    ' Compacto huella
                                    Try
                                        tmpstr = Convert.ToBase64String(Convert.FromBase64String(tmpstr))
                                    Catch ex As Exception
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Exception packaging finger " & oSyncTask.IDFinger.ToString & " for employee " + oSyncTask.IDEmployee.ToString + ". We will try without packing!")
                                    End Try

                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Size, tmpstr.Length.ToString)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TMP, tmpstr)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Valid, "1")
                                    iTasksInMessage = iTasksInMessage + 1
                                    If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("bio") Then
                                        bMaxTasksReached = (iTasksInMessage = dTasksCadence("bio"))
                                    Else
                                        bMaxTasksReached = (iTasksInMessage = 5)
                                    End If
                                    'Log
                                    Try
                                        params(iTasksInMessage) = mCurrentEmployee.Name + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.AddBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No bio information for employee " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addbiodataface
                                If mCurrentEmployee.BioFaceData(oCurrentTask.IDFinger) IsNot Nothing AndAlso (mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper) Then
                                    Dim sMajor As String = "58"
                                    Dim sMinor As String = "0"
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.BIODATA, True)
                                    oMessage.data_table.addCommandParametersEx("Pin", oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.No, "0")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Index, oSyncTask.IDFinger)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Valid, "1")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Duress, "0")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "9")
                                    If mCurrentEmployee.BioFaceAlgorithmVersion(oCurrentTask.IDFinger).Contains(".") Then
                                        sMajor = mCurrentEmployee.BioFaceAlgorithmVersion(oCurrentTask.IDFinger).Split(".")(0)
                                        sMinor = mCurrentEmployee.BioFaceAlgorithmVersion(oCurrentTask.IDFinger).Split(".")(1)
                                    End If
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MajorVer, sMajor)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MinorVer, sMinor)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Format, "0")

                                    Dim tmpstr As String = New String(System.Text.Encoding.UTF8.GetChars(mCurrentEmployee.BioFaceData(oCurrentTask.IDFinger)))
                                    ' Compacto huella
                                    Try
                                        tmpstr = Convert.ToBase64String(Decode(Convert.FromBase64String(tmpstr)))
                                    Catch ex As Exception
                                        'roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Exception packaging face " & oSyncTask.IDFinger.ToString & " for employee " + oSyncTask.IDEmployee.ToString + ". We will try without packing!")
                                    End Try

                                    oMessage.data_table.addCommandParametersEx("Tmp", tmpstr)

                                    iTasksInMessage = iTasksInMessage + 1
                                    If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("biodataface") Then
                                        bMaxTasksReached = (iTasksInMessage = dTasksCadence("biodataface"))
                                    Else
                                        bMaxTasksReached = (iTasksInMessage = 5)
                                    End If

                                    'Log
                                    Try
                                        params(iTasksInMessage) = mCurrentEmployee.Name + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.AddBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No bio information for employee " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addbiodatapalm
                                If mCurrentEmployee.BioPalmData(oCurrentTask.IDFinger) IsNot Nothing AndAlso (mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper) Then
                                    Dim sMajor As String = "12"
                                    Dim sMinor As String = "0"
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.BIODATA, True)
                                    oMessage.data_table.addCommandParametersEx("Pin", oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.No, "0")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Index, oSyncTask.IDFinger)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Valid, "1")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Duress, "0")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "8")
                                    If mCurrentEmployee.BioPalmAlgorithmVersion(oCurrentTask.IDFinger).Contains(".") Then
                                        sMajor = mCurrentEmployee.BioPalmAlgorithmVersion(oCurrentTask.IDFinger).Split(".")(0)
                                        sMinor = mCurrentEmployee.BioPalmAlgorithmVersion(oCurrentTask.IDFinger).Split(".")(1)
                                    End If
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MajorVer, sMajor)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MinorVer, sMinor)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Format, "0")

                                    Dim tmpstr As String = New String(System.Text.Encoding.UTF8.GetChars(mCurrentEmployee.BioPalmData(oCurrentTask.IDFinger)))
                                    ' Compacto huella
                                    Try
                                        tmpstr = Convert.ToBase64String(Decode(Convert.FromBase64String(tmpstr)))
                                    Catch ex As Exception
                                        'roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Exception packaging palm " & oSyncTask.IDFinger.ToString & " for employee " + oSyncTask.IDEmployee.ToString + ". We will try without packing!")
                                    End Try

                                    oMessage.data_table.addCommandParametersEx("Tmp", tmpstr)

                                    iTasksInMessage = iTasksInMessage + 1
                                    If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("biodatapalm") Then
                                        bMaxTasksReached = (iTasksInMessage = dTasksCadence("biodatapalm"))
                                    Else
                                        bMaxTasksReached = (iTasksInMessage = 5)
                                    End If
                                    'Log
                                    Try
                                        params(iTasksInMessage) = mCurrentEmployee.Name + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.AddBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No bio information for employee " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addphoto
                                If mCurrentEmployee.EmployeeBus.EmployeeImage IsNot Nothing AndAlso mTerminal.Model.ToUpper <> eTerminalModel.rxFe.ToString.ToUpper AndAlso mTerminal.Model.ToUpper <> eTerminalModel.rxTe.ToString.ToUpper Then
                                    Dim sPictureBase64 As String = String.Empty
                                    sPictureBase64 = Convert.ToBase64String(roTypes.Image2Bytes(roSupport.FixedSize(mCurrentEmployee.EmployeeBus.EmployeeImage, 200, 200)))
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERPIC, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Size, sPictureBase64.Length)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Content, sPictureBase64)
                                    iTasksInMessage = iTasksInMessage + 1
                                    If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("photo") Then
                                        bMaxTasksReached = (iTasksInMessage = dTasksCadence("photo"))
                                    Else
                                        bMaxTasksReached = (iTasksInMessage = 5)
                                    End If
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No photo found for employee " + oSyncTask.IDEmployee.ToString + " or device does not support employee photo. Task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addemployeetimezones
                                Dim sLogInfo As String = ""
                                Dim sEmployeeAccessLevelDef As String = oSyncTask.TaskData.ToString
                                If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERINFO, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    Dim TZ1 As String = "1"
                                    Dim TZ2 As String = "0"
                                    Dim TZ3 As String = "0"
                                    TZ1 = CInt(sEmployeeAccessLevelDef.Split(";")(0)).ToString("X").PadLeft(4, "0")
                                    TZ2 = CInt(sEmployeeAccessLevelDef.Split(";")(1)).ToString("X").PadLeft(4, "0")
                                    TZ3 = CInt(sEmployeeAccessLevelDef.Split(";")(2)).ToString("X").PadLeft(4, "0")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZ, "1|" & TZ1 & "|" & TZ2 & "|" & TZ3)
                                    ' Para F22, los TZ de los empleados se pasan con la definición del empleado
                                    ' donde el formato es "<grupo=0/empleado=1>|<TZ1 en HEXA con 4 dígitos>|<TZ2 en HEXA con 4 dígitos>|<TZ3 en HEXA con 4 dígitos>"
                                    iTasksInMessage = iTasksInMessage + 1
                                    If (mTerminal.Model.ToUpper.StartsWith("RXF") OrElse mTerminal.Model.ToUpper.StartsWith("RX1")) AndAlso dTasksCadence.ContainsKey("employeetimezones") Then
                                        bMaxTasksReached = (iTasksInMessage = dTasksCadence("employeetimezones"))
                                    Else
                                        bMaxTasksReached = (iTasksInMessage = 15)
                                    End If
                                Else
                                    ' Terminales rxC y rxCe PUSH
                                    ' Para cada TimeZone y Empleado, programa por qué puertas tiene acceso concedido
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SetUserTZStr, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZs, sEmployeeAccessLevelDef)
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 10)
                                End If
                                'Log
                                Try
                                    params(iTasksInMessage) = mCurrentEmployee.Name + ";" + sLogInfo.Split(",")(1) + ";" + sLogInfo.Split(",")(2)
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.AddEmployeeAccessLevel
                                Catch ex As Exception
                                End Try
                            Case roTerminalsSyncTasks.SyncActions.delbio
                                If mTerminal.Model.ToUpper <> eTerminalModel.rxTe.ToString.ToUpper Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.FINGERTMP, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.FID, oSyncTask.IDFinger)
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 20)
                                    'Log
                                    Try
                                        params(iTasksInMessage) = oSyncTask.IDEmployee.ToString + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.DelBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Task not compliant for rxTe device: " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.delbiodataface
                                Dim ZKMola As Boolean = False
                                If ZKMola Then
                                    ' TODO ZK: Si borras una cara de un empleado, LAS BORRAS TODAS, Y TAMBIÉN TODAS LAS MANOS !!!!!!!. De momento, no envío borrado de caras ni manos, porque normalmente siempre van con un delete del propio empeelado, y eso lo borra todo.
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.BIODATA, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "9")
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 20)
                                    'Log
                                    Try
                                        params(iTasksInMessage) = oSyncTask.IDEmployee.ToString + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.DelBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Task: " + oSyncTask.ToString + " ignored. If you delete an employee face, you delete all due to protocolo bug!")
                                    If oMessage Is Nothing Then
                                        oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                        oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    End If
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 20)
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                    Return True
                                End If
                            Case roTerminalsSyncTasks.SyncActions.delbiodatapalm
                                Dim ZKMola As Boolean = False
                                If ZKMola Then
                                    ' Si borras una cara de un empleado, LAS BORRAS TODAS, Y TAMBIÉN TODAS LAS MANOS !!!!!!!. De momento, no envío borrado de caras ni manos, porque normalmente siempre van con un delete del propio empeelado, y eso lo borra todo.
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.BIODATA, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "8")
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.No, oSyncTask.IDFinger)
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 20)
                                    'Log
                                    Try
                                        params(iTasksInMessage) = oSyncTask.IDEmployee.ToString + ";" + oSyncTask.IDFinger.ToString
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.DelBio
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Task: " + oSyncTask.ToString + " ignored. If you delete an employee palm, you delete all due to protocolo bug!")
                                    If oMessage Is Nothing Then
                                        oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                        oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    End If
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 20)
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                    Return True
                                End If
                            Case roTerminalsSyncTasks.SyncActions.delphoto
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.USERPIC, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                            Case roTerminalsSyncTasks.SyncActions.delcard
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERINFO, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Card, "[0000000000]")
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                                'Log
                                Try
                                    params(iTasksInMessage) = oSyncTask.IDEmployee.ToString
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.DelCard
                                Catch ex As Exception
                                End Try
                            Case roTerminalsSyncTasks.SyncActions.delemployeetimezones
                                If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    ' La instrucción DelUserTZStr no funciona en el firmware de ZK. A la espera de corrección, asignamos TZ 0;0;0
                                    'oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DelUserTZStr, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.USERINFO, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZs, "1|0|0|0")
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 15)
                                Else
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    ' La instrucción DelUserTZStr no funciona en el firmware de ZK. A la espera de corrección, asignamos TZ 0;0;0
                                    'oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DelUserTZStr, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SetUserTZStr, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TZs, "0;0;0")
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 15)
                                End If

                                'Log
                                Try
                                    params(iTasksInMessage) = "Employee " & oSyncTask.IDEmployee.ToString & ";" + oSyncTask.Parameter1.ToString
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.DelEmployeeAccessLevel
                                Catch ex As Exception
                                End Try
                            Case roTerminalsSyncTasks.SyncActions.delemployee
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.USERINFO, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, oSyncTask.IDEmployee)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                                'Log
                                Try
                                    params(iTasksInMessage) = oSyncTask.IDEmployee.ToString
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.DelEmployee
                                Catch ex As Exception
                                End Try
                            Case Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetCommandsFromTask: Unknown or unexpected task ( " + oSyncTask.ToString + ")")
                                oSyncTask.DoneEx(oSyncTask.ID)
                                oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                Return True
                        End Select
                    Else
                        If bEmployeeInfoRequired AndAlso Not bEmployeeExists Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Employee " + oSyncTask.IDEmployee.ToString + " does not exist!. Task: " + oSyncTask.ToString + " ignored")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            If iTasksInMessage = 0 Then
                                oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            End If
                            Return True
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Unable to load employee data. Employee  " + oSyncTask.IDEmployee.ToString + " Task: " + oSyncTask.Task.ToString + " EmployeeID" + oSyncTask.IDEmployee.ToString)
                            ' Retraso todas las tareas relativas a este empleado 1 minuto, para que el resto se procesen
                            oSyncTask.DelayEmployeeTasks(60)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            If iTasksInMessage = 0 Then
                                oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            End If
                            Return True
                        End If
                        bMaxTasksReached = True
                    End If
                Else
                    ' Tareas sin emplaado
                    Select Case oSyncTask.Task
                        Case roTerminalsSyncTasks.SyncActions.setterminalconfig
                            If oSyncTask.TaskData.Length > 0 Then
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                Dim sDetailForLog As String
                                sDetailForLog = GetTerminalConfigParams(oSyncTask.TaskData, oMessage)
                                'Añado parámetro fijo para que en caso de quedar sin espacio para fichajes, borre los 500 más antiguos
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DelRecord=500")
                                ' Clave de comunicaciones para acceso desde apps de ZK
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                Dim scommskey As String = "COMKey=951753"
#If DEBUG Then
                                scommskey = "COMKey=0"
#End If
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, scommskey)

                                ' Tamaño de paquetes de fichajes. En Multitenant, indicamos al terminal que envie los fichajes de 1 en 1 para evitar cuellos de botella en servidor
                                '                                 En entornos monotenant esto nunca se ha desmostrado necesario.
                                If bIsIIS Then
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "PushRecordCount=1")
                                End If

                                'Y configuración de hora (de momento sólo en protocolo sobre IIS).
                                'TODO: Pendiente hacerlo de manera correcta, como en PushTerminalManager. Envío el GMT en los mensajes, en Init el TZ que toca, y aquí parámetros de DLST.
                                '      Para esto hay que asegurar que todos los terminales (rxC y rxCe) funcionan con este protocolo
                                If bIsIIS Then
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DLSTMode=" & mTerminal.DLSTMode.ToString)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DaylightSavingTimeOn=" & mTerminal.DaylightSavingTimeOn.ToString)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DaylightSavingTime=" & mTerminal.DaylightSavingTime.ToString)
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "StandardTime=" & mTerminal.StandardTime.ToString)
                                Else
                                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DaylightSavingTimeOn=0")
                                End If

                                'Log
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"TerminalLogicZKPush2::GetNextCommand::Task setterminalconfig for terminal:{vbCrLf}sDetailForLog")
                                Try
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.Config
                                    params(0) = [Enum].GetName(GetType(CTerminalLogicBase.mxCInbioEventID), iEvent) & vbCr & sDetailForLog
                                    iEvent = 0
                                Catch ex As Exception
                                End Try
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No config parameter specified!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.setsirens
                            If oSyncTask.TaskData.Length > 0 Then
                                If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    Dim sDetailForLog As String
                                    sDetailForLog = GetTerminalSirensAlt1(oSyncTask.TaskData, oMessage)
                                    'Log
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"TerminalLogicZKPush2::GetNextCommand::Task setsirens for terminal:{vbCrLf}sDetailForLog")
                                    Try
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.Sirens
                                        params(0) = [Enum].GetName(GetType(CTerminalLogicBase.mxCInbioEventID), iEvent) & vbCr & sDetailForLog
                                        iEvent = 0
                                    Catch ex As Exception
                                    End Try
                                Else
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    Dim sDetailForLog As String
                                    sDetailForLog = GetTerminalSirens(oSyncTask.TaskData, oMessage)
                                    'Log
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"TerminalLogicZKPush2::GetNextCommand::Task setsirens for terminal:{vbCrLf}sDetailForLog")
                                    Try
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.Sirens
                                        params(0) = [Enum].GetName(GetType(CTerminalLogicBase.mxCInbioEventID), iEvent) & vbCr & sDetailForLog
                                        iEvent = 0
                                    Catch ex As Exception
                                    End Try
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No sirens data!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.setcauses
                            If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper _
                                                                                             OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper _
                                                                                             OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper _
                                                                                             OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper _
                                                                                             OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                If oSyncTask.TaskData.Length > 0 Then
                                    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                    Dim sDetailForLog As String
                                    sDetailForLog = GetTerminalCauses(oSyncTask.TaskData, oMessage)
                                    'Log
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"TerminalLogicZKPush2::GetNextCommand::Task setcauses for terminal:{vbCrLf}sDetailForLog")
                                    Try
                                        iEvent = CTerminalLogicBase.mxCInbioEventID.Config
                                        params(0) = [Enum].GetName(GetType(CTerminalLogicBase.mxCInbioEventID), iEvent) & vbCr & sDetailForLog
                                        iEvent = 0
                                    Catch ex As Exception
                                    End Try
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: No causes data!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Else
                                ' Incompatible
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Only available on rx1!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.getterminalconfig
                            'Solo rx1
                            If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.GET_OPTION, MessageZKPush2.msgTables.none, True)
                                Dim sTerminalConfigParams As String = GetTerminalConfigParamValues(oSyncTask.TaskData, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTerminalConfigParams)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.delallemployees
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.USERINFO, True)
                            iEvent = CTerminalLogicBase.mxCInbioEventID.ResetEmployees
                        Case roTerminalsSyncTasks.SyncActions.delallcauses
                            If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.WORKCODE, True)
                                iEvent = CTerminalLogicBase.mxCInbioEventID.Config
                            Else
                                ' Incompatible
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Only available on rx1!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.addtimezone
                            If mTerminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFP.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFL.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFPTD.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxFe.ToString.ToUpper OrElse mTerminal.Model.ToUpper = eTerminalModel.rxTe.ToString.ToUpper Then
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.none, True)
                                Dim sLogDetail As String = ""
                                Dim sTimeZoneDef As String = GetTimeZoneDefinitionAlt1(oSyncTask.TaskData, sLogDetail)
                                If sTimeZoneDef <> "" Then
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTimeZoneDef)
                                Else
                                    'No pude recuperar la definición del timezone. Debo forzar reprogramación de Timezones
                                    mTerminal.DeleteTimeZoneFile()
                                    bCallBroadcaster = True
                                    oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                    oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetCommandsFromTask: Unable to load timezone definition!. (task=" + oSyncTask.ToString + ")")
                                    Return True 'No la reseteo porque no es tratable, y encualquier caso Broadcaster la volverá a generar
                                End If
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                                Try
                                    params(iTasksInMessage) = sLogDetail
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.AddTimePeriod
                                Catch ex As Exception
                                End Try
                            Else
                                If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SetTZInfo, MessageZKPush2.msgTables.none, True)
                                Dim sLogDetail As String = ""
                                Dim sTimeZoneDef As String = GetTimeZoneDefinition(oSyncTask.TaskData, sLogDetail)
                                If sTimeZoneDef <> "" Then
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTimeZoneDef)
                                Else
                                    'No pude recuperar la definición del timezone. Debo forzar reprogramación de Timezones
                                    mTerminal.DeleteTimeZoneFile()
                                    bCallBroadcaster = True
                                    oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                                    oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetCommandsFromTask: Unable to load timezone definition!. (task=" + oSyncTask.ToString + ")")
                                    Return True 'No la reseteo porque no es tratable, y encualquier caso Broadcaster la volverá a generar
                                End If
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                                Try
                                    params(iTasksInMessage) = sLogDetail
                                    iEvent = CTerminalLogicBase.mxCInbioEventID.AddTimePeriod
                                Catch ex As Exception
                                End Try
                            End If
                        Case roTerminalsSyncTasks.SyncActions.deltimezone
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SetTZInfo, MessageZKPush2.msgTables.none, True)
                            Dim sLogDetail As String = ""
                            Dim sTimeZoneDef As String = ResetTimeZoneDefinition(oSyncTask.Parameter1)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTimeZoneDef)
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 15)
                            'Log
                            Try
                                params(iTasksInMessage) = oSyncTask.Parameter1.ToString
                                iEvent = CTerminalLogicBase.mxCInbioEventID.DelTimePeriod
                            Catch ex As Exception
                            End Try
                        Case roTerminalsSyncTasks.SyncActions.getallemployees
                            'TODO: No funciona el protocolo de ZK. De momento las ignoro (respondo ok, y la borro) ...
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageZKPush2.msgTables.USERINFO, True)
                            'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, "")
                        Case roTerminalsSyncTasks.SyncActions.getallemployeeaccesslevel, roTerminalsSyncTasks.SyncActions.getalltimezones, roTerminalsSyncTasks.SyncActions.getallfingerprints, roTerminalsSyncTasks.SyncActions.delallcards, roTerminalsSyncTasks.SyncActions.getterminalconfig
                            'TODO: No funciona el protocolo de ZK. De momento las ignoro (respondo ok, y la borro) ...
                            'If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            'oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageZKPush2.msgTables.USERINFO, True)
                            'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, "")
                            oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                            oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Task not supported!. (task=" + oSyncTask.ToString + "). Ignoring!")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            Return True
                            'Case roTerminalsSyncTasks.SyncActions.getallemployeeaccesslevel
                            '    'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            '    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            '    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageZKPush2.msgTables.none, True)
                            '    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "userauthorize,fielddesc=*, filter =*")
                            'Case roTerminalsSyncTasks.SyncActions.getalltimezones
                            '    'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            '    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            '    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageZKPush2.msgTables.none, True)
                            '    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "timezone,fielddesc=*, filter =*")
                            'Case roTerminalsSyncTasks.SyncActions.getallfingerprints
                            '    'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            '    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            '    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageZKPush2.msgTables.FINGERTMP, True)
                            '    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.PIN, "500")
                            '    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.FID, "0")
                        Case roTerminalsSyncTasks.SyncActions.delallbios
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.FINGERTMP, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                            iEvent = CTerminalLogicBase.mxCInbioEventID.ResetBio
                            'Case roTerminalsSyncTasks.SyncActions.delallcards
                            '    ' TODO: De momento no se pueden borrar todas las tarjetas en un sólo comando sin borrar todos los empleados. Mando un mensaje que no devuelva error
                            '...
                            '    iEvent = CTerminalLogicBase.mxCInbioEventID.ResetCards
                        Case roTerminalsSyncTasks.SyncActions.delallbiodataface
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.BIODATA, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "9")
                            iEvent = CTerminalLogicBase.mxCInbioEventID.ResetBio
                        Case roTerminalsSyncTasks.SyncActions.delallbiodatapalm
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.BIODATA, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Type, "8")
                            iEvent = CTerminalLogicBase.mxCInbioEventID.ResetBio
                        'Case roTerminalsSyncTasks.SyncActions.delallphotos
                        '    El borrado de todas las fotos no está soportado, pero no es necesario. Al borrar todos los empleados, se borran todas las fotos
                        '    If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                        '    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.USERPIC, True)
                        '    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                        Case roTerminalsSyncTasks.SyncActions.delallemployeetimezones
                            ' Situación anómala. Fuerzo programación de cero
                            mTerminal.ForceFullDataSync()
                            bCallBroadcaster = True
                            oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                            oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Unexpected task!. (task=" + oSyncTask.ToString + "). Forcing employees reset!")
                            oSyncTask.DoneEx(oSyncTask.ID) 'Math.Truncate(oCommandID / 10))
                            Return True 'No la reseteo porque no es tratable, y encualquier caso Broadcaster la volverá a generar
                        Case roTerminalsSyncTasks.SyncActions.delalltimezones
                            ' No es necesaria. Siempre se generarán los 50 periodos
                            oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                            oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Task delalltimezones ignored!. Al 50 timezones will be updated")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            Return True
                        Case roTerminalsSyncTasks.SyncActions.shell
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SHELL, MessageZKPush2.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, oSyncTask.TaskData.Replace("#", Chr(34)))
                        Case roTerminalsSyncTasks.SyncActions.reboot
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.REBOOT, MessageZKPush2.msgTables.none, True)
                        Case roTerminalsSyncTasks.SyncActions.cleardata
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.CLEAR_DATA, MessageZKPush2.msgTables.none, True)
                        Case roTerminalsSyncTasks.SyncActions.check
                            ' Fuerza que el terminal vuelva a enviar un mensaje de init, y por tanto vuelva a pedir los parámetros de configuración
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.CHECK, MessageZKPush2.msgTables.none, True)
                        Case roTerminalsSyncTasks.SyncActions.log
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.LOG, MessageZKPush2.msgTables.none, True)
                        Case roTerminalsSyncTasks.SyncActions.info
                            If oMessage Is Nothing Then oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.command, MessageZKPush2.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.INFO, MessageZKPush2.msgTables.none, True)
                        Case Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetCommandsFromTask: Unknown or unexpected task ( " + oSyncTask.ToString + ")")
                            oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                            oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            oSyncTask.DoneEx(oSyncTask.ID)
                            Return True
                    End Select
                End If
                If mCurrentEmployee IsNot Nothing Then mCurrentEmployee = Nothing
                Return True
            Catch ex As Exception
                oMessage = New MessageZKPush2(MessageZKPush2.msgCommand.genericresponse, , True, sHttpVersion)
                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetCommandsFromTask for task ( " + oSyncTask.ToString + " ). Error:", ex)
                Return False
            Finally
                roTrace.GetInstance.AddTraceEvent($"{oSyncTask.Task} task prepared to send")
            End Try
        End Function

        ''' <summary>
        ''' Obtiene una cadena con la definición del timezone cuya definición se guardó en la tarea de sincronización
        ''' </summary>
        ''' <param name="sTaskData"></param>
        ''' <remarks></remarks>
        Public Function GetTimeZoneDefinition(sTaskData As String, ByRef sLogData As String) As String
            Try
                'Cargamos definiticion a partir del XML
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TimeZonesZKPush2DataTable = dsLocalData.TimeZonesZKPush2
                Dim tmp As String = ""
                Dim tmpLog As String = ""
                Dim idTimeZone As Integer = 0

                If sTaskData <> "" Then
                    ' Obtendremos definición del timezone, con el formato 'TZIndex=20	TZ=07:00-12:00;00:00-23:59;00:00-23:59;00:00-23:59;00:00-23:59;00:00-23:59;00:00-23:59	TZMode=1
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    idTimeZone = oTbl.Rows(6)("IdTimeZone")
                    tmp += roTypes.Any2DateTime(oTbl.Rows(6)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(6)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(0)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(0)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(1)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(1)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(2)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(2)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(3)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(3)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(4)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(4)("EndTime")).ToString("HH:mm") + "-"
                    tmp += roTypes.Any2DateTime(oTbl.Rows(5)("BeginTime")).ToString("HH:mm") + ":" + roTypes.Any2DateTime(oTbl.Rows(5)("EndTime")).ToString("HH:mm")

                    tmpLog = idTimeZone.ToString + tmp
                    sLogData = tmpLog
                    'Finalmente añado el ID de timezone
                    Return "TZIndex=" + idTimeZone.ToString + vbTab + "TZ=" + tmp + vbTab + "TZMode=1"
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTimeZoneDefinition::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Obtiene una cadena con la definición del timezone cuya definición se guardó en la tarea de sincronización para terminales F22
        ''' </summary>
        ''' <param name="sTaskData"></param>
        ''' <remarks></remarks>
        Public Function GetTimeZoneDefinitionAlt1(sTaskData As String, ByRef sLogData As String) As String
            Try
                'Cargamos definiticion a partir del XML
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TimeZonesZKPush2DataTable = dsLocalData.TimeZonesZKPush2
                Dim tmp As String = ""
                Dim tmpLog As String = ""
                Dim idTimeZone As Integer = 0

                If sTaskData <> "" Then
                    ' Formato de definición: DATA UPDATE AccTimeZone UID=1	SunStart=1200	SunEnd=2300	MonStart=0	MonEnd=1200	TuesStart=0	TuesEnd=1300	WedStart=0	WedEnd=1200	ThursStart=0	ThursEnd=1200	FriStart=0	FriEnd=1200	SatStart=0	SatEnd=1200
                    ' DATA UPDATE AccGroup ID=1	Verify=1	ValidHoliday=0	TZ1=1	TZ2=1	TZ3=3
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    idTimeZone = oTbl.Rows(6)("IdTimeZone")
                    tmp = "AccTimeZone UID=" & idTimeZone.ToString
                    tmp += vbTab + "SunStart=" + roTypes.Any2DateTime(oTbl.Rows(6)("BeginTime")).ToString("HHmm") + vbTab + "SunEnd=" + roTypes.Any2DateTime(oTbl.Rows(6)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "MonStart=" + roTypes.Any2DateTime(oTbl.Rows(0)("BeginTime")).ToString("HHmm") + vbTab + "MonEnd=" + roTypes.Any2DateTime(oTbl.Rows(0)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "TuesStart=" + roTypes.Any2DateTime(oTbl.Rows(1)("BeginTime")).ToString("HHmm") + vbTab + "TuesEnd=" + roTypes.Any2DateTime(oTbl.Rows(1)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "WedStart=" + roTypes.Any2DateTime(oTbl.Rows(2)("BeginTime")).ToString("HHmm") + vbTab + "WedEnd=" + roTypes.Any2DateTime(oTbl.Rows(2)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "ThursStart=" + roTypes.Any2DateTime(oTbl.Rows(3)("BeginTime")).ToString("HHmm") + vbTab + "ThursEnd=" + roTypes.Any2DateTime(oTbl.Rows(3)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "FriStart=" + roTypes.Any2DateTime(oTbl.Rows(4)("BeginTime")).ToString("HHmm") + vbTab + "FriEnd=" + roTypes.Any2DateTime(oTbl.Rows(4)("EndTime")).ToString("HHmm")
                    tmp += vbTab + "SatStart=" + roTypes.Any2DateTime(oTbl.Rows(5)("BeginTime")).ToString("HHmm") + vbTab + "SatEnd=" + roTypes.Any2DateTime(oTbl.Rows(5)("EndTime")).ToString("HHmm")

                    tmpLog = tmp
                    sLogData = tmpLog
                    'Finalmente añado el ID de timezone
                    Return tmp
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTimeZoneDefinitionAlt1::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Obtiene definición de timezone sin acceso
        ''' </summary>
        ''' <param name="iIdTimeZone"></param>
        ''' <remarks></remarks>
        Public Function ResetTimeZoneDefinition(iIdTimeZone As Integer) As String
            Try
                Return "TZIndex=" + iIdTimeZone.ToString + vbTab + "TZ=22:00-00:00;22:00-00:00;22:00-00:00;22:00-00:00;22:00-00:00;22:00-00:00;22:00-00:00	TZMode=1"
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ResetTimeZoneDefinition:: Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Devuelve la zona horaria que define cuándo están activos los lectores. Por defecto siempre, todos los lectores.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetReadersValidTimezone() As String
            ' Esta zona horaria debe existeir, y estar asignada a los parámetros Door1ValidTZ, Door2ValidTZ, ...
            'DATA UPDATE timezone TimezoneId=999999	SunTime1=2359	SunTime2=0	SunTime3=0	MonTime1=2359	MonTime2=0	MonTime3=0	TueTime1=2359	TueTime2=0	TueTime3=0	WedTime1=2359	WedTime2=0	WedTime3=0	ThuTime1=2359	ThuTime2=0	ThuTime3=0	FriTime1=2359	FriTime2=0	FriTime3=0	SatTime1=2359	SatTime2=0	SatTime3=0	Hol1Time1=2359	Hol1Time2=0	Hol1Time3=0	Hol2Time1=2359	Hol2Time2=0	Hol2Time3=0	Hol3Time1=2359	Hol3Time2=0	Hol3Time3=0
            Return "TimezoneId=999999	SunTime1=2359	SunTime2=0	SunTime3=0	MonTime1=2359	MonTime2=0	MonTime3=0	TueTime1=2359	TueTime2=0	TueTime3=0	WedTime1=2359	WedTime2=0	WedTime3=0	ThuTime1=2359	ThuTime2=0	ThuTime3=0	FriTime1=2359	FriTime2=0	FriTime3=0	SatTime1=2359	SatTime2=0	SatTime3=0	Hol1Time1=2359	Hol1Time2=0	Hol1Time3=0	Hol2Time1=2359	Hol2Time2=0	Hol2Time3=0	Hol3Time1=2359	Hol3Time2=0	Hol3Time3=0"
        End Function

        Public Function GetTerminalConfigParams(sTaskData As String, oMessage As MessageZKPush2) As String
            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TerminalConfigDataTable = dsLocalData.TerminalConfig
                Dim oRow As LocalDataSet.TerminalConfigRow
                Dim res As String = ""
                Dim sParamName As String
                Dim sZKParamName As String
                Dim sParamValue As String

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        sParamName = oRow.Name
                        sParamValue = oRow.Value
                        ' Traducimos nombre de parámetro
                        Select Case sParamName
                            Case "RDR1OpenTime"
                                sZKParamName = "Door1Drivertime"
                            Case "RDR2OpenTime"
                                sZKParamName = "Door2Drivertime"
                            Case "RDR3OpenTime"
                                sZKParamName = "Door3Drivertime"
                            Case "RDR4OpenTime"
                                sZKParamName = "Door4Drivertime"
                            Case Else
                                sZKParamName = sParamName
                        End Select

                        If res = "" Then
                            res = sZKParamName + "=" + sParamValue
                        Else
                            res += " / " + sZKParamName + "=" + sParamValue
                        End If
                        oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sZKParamName + "=" + sParamValue)
                    Next
                    Return res
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalConfigParams::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalConfigParamValues(sTaskData As String, bIsGet As Boolean) As String
            Try
                'Cargamos definiticion a partir del XML
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TerminalConfigDataTable = dsLocalData.TerminalConfig
                Dim oRow As LocalDataSet.TerminalConfigRow
                Dim res As String = ""
                Dim sParamName As String
                Dim sZKParamName As String
                Dim sParamValue As String

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        sParamName = oRow.Name
                        sParamValue = oRow.Value
                        ' Traducimos nombre de parámetro
                        Select Case sParamName
                            Case "RDR1OpenTime"
                                sZKParamName = "Door1Drivertime"
                            Case "RDR2OpenTime"
                                sZKParamName = "Door2Drivertime"
                            Case "RDR3OpenTime"
                                sZKParamName = "Door3Drivertime"
                            Case "RDR4OpenTime"
                                sZKParamName = "Door4Drivertime"
                            Case Else
                                sZKParamName = sParamName
                        End Select
                        If res = "" Then
                            If bIsGet Then
                                res = sZKParamName
                            Else
                                res = sZKParamName + "=" + sParamValue
                            End If
                        Else
                            If bIsGet Then
                                res += "," + sZKParamName
                            Else
                                res += "," + sZKParamName + "=" + sParamValue
                            End If
                        End If
                    Next

                    Return res
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxCInbio::GetTerminalConfigParams::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalConfigForRMA() As String
            Dim res As String = String.Empty
            Try
                res = "IPAddress,GATEIPAddress,DNS,NetMask,WebServerURLModel,ICLOCKSVRURL,WebServerIP,WebServerPort,WirelessSSID,WifiOn"
                Return res
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalConfigForRMA::Error getting info for task data ( " + res + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalSecurityConfig() As String
            Dim res As String = String.Empty
            Try
                res = "IRTempThreshold,IRTempLowThreshold,EnalbeIRTempDetection,EnableNormalIRTempPass,EnalbeMaskDetection,EnableWearMaskPass,BiometricVersion" 'OJO: los "enalbe" no se deben corregir. Los chinos son así!
                Return res
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalSecurityConfig::Error getting info for task data ( " + res + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalSirens(sTaskData As String, ByRef oMessage As MessageZKPush2) As String
            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.SirensDataTable = dsLocalData.Sirens
                Dim oRow As LocalDataSet.SirensRow
                Dim res As String = ""
                Dim sZKWDAutoAlarm As String = "WDAutoAlarm"
                Dim sZKNullAlarm As String = "6204"
                Dim iAlarmDayIndex As Integer = 0
                Dim iAlarmDayTimeIndex As Integer = 6204
                Dim iWeekDaySiren As Integer = -1
                Dim iLastDay As Integer = -1
                Dim iSirenDuration As Integer = 150

                If sTaskData <> "" Then
                    ' Primero borro todas las sirenas
                    ' Hay 12 por día
                    For i = 1 To 84
                        oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, i = 1)
                        oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sZKWDAutoAlarm & i.ToString & "=" & sZKNullAlarm)
                    Next

                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    iWeekDaySiren = -1
                    For Each oRow In oTbl.Rows
                        ' DayOf 1 = Lunes -> ZK: 1-12 Dom, 13-24 Lun, 25-36 Mar, 37-48 Mier
                        If oRow.DayOf <> iLastDay Then
                            iLastDay = oRow.DayOf
                            iWeekDaySiren = 1
                        Else
                            iWeekDaySiren = iWeekDaySiren + 1
                        End If

                        ' Cada día tiene un máximo de 12 sirenas. Si hay más las ignoro y aviso por log.
                        If iWeekDaySiren <= 12 Then
                            If oRow.DayOf = 7 Then
                                iAlarmDayIndex = iWeekDaySiren
                            Else
                                iAlarmDayIndex = oRow.DayOf * 12 + iWeekDaySiren
                            End If
                            iAlarmDayTimeIndex = oRow.StartDate.Hour * 256 + oRow.StartDate.Minute
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, False)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sZKWDAutoAlarm & iAlarmDayIndex.ToString & "=" & iAlarmDayTimeIndex.ToString)
                            If res = "" Then
                                res = sZKWDAutoAlarm & iAlarmDayIndex.ToString + "=" + oRow.StartDate.ToString("t") & "(" & iAlarmDayTimeIndex.ToString & ")"
                            Else
                                res += sZKWDAutoAlarm & iAlarmDayIndex.ToString + "=" + oRow.StartDate.ToString("t") & "(" & iAlarmDayTimeIndex.ToString & ")"
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalSirens::More than 12 sirens for dayweek " & iLastDay & ". Some sirens will be ignored")
                        End If

                        ' Duración. Nos quedamos con el último (aunque en pantalla se pueda configurar uno para cada sirena)
                        iSirenDuration = oRow.Duration

                    Next

                    ' Los terminales no aceptan duración inferior a 3 segundos. Si se programa menos, la sirena no suena ...
                    If iSirenDuration < 3 Then iSirenDuration = 3

                    ' Parámetros comunes para habilitar sirenas
                    ' TOD: Que los configuren a mano. Resulta que en terminales rxC con franja naranaja los parámetros no son exactamente los mismos que en los clásicos, y alguno parece interferir ...
                    If oTbl.Rows.Count > 0 Then
                        ''    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        ''    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.AADelay.ToString & "=0")
                        ''    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        ''    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.FPOpenRelay.ToString & "=0")
                        ''    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        ''    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.AutoOpenRelay.ToString & "=1")
                        ''    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        ''    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.WiegandOpenDoor.ToString & "=0")
                        ' Tiempo de sirena (aunque parece que el terminal no hace caso ...)
                        oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                        oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.AutoOpenRelayTimes.ToString & "=" & iSirenDuration.ToString)
                    End If
                    Return res
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalConfigParams::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalSirensAlt1(sTaskData As String, ByRef oMessage As MessageZKPush2) As String
            Try
                'Sirenas para terminales rx1
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.SirensDataTable = dsLocalData.Sirens
                Dim oRow As LocalDataSet.SirensRow
                Dim res As String = ""
                Dim iSirenId As Integer = 0
                Dim iSirenDuration As Integer = 3
                Dim iWeekDay As Integer = 1

                If sTaskData <> "" Then
                    ' Parámetros comunes para activar sirenas por relé
                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.isSupportAlarmExt.ToString & "=1")

                    ' Habilito detección de dedo vivo
                    'oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTION, MessageZKPush2.msgTables.none, True)
                    'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, clsPushProtocol.dataTerminalConfigParameters.FakeFingerFunOn.ToString & "=1")

                    ' Primero borro todas las sirenas
                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.BELLINFO, True)

                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)

                    For Each oRow In oTbl.Rows
                        ' Formato a enviar: DATA UPDATE BellInfo ID=1	Valid=1	Time=1026	WavIndex=1	Times=3	Way=0	Volume=30	Sun=1	Mon=1	Tue=1	Wed=0	Thu=0	Fri=0	Sat=0	ExtbellDelay=10
                        iWeekDay = oRow.DayOf
                        If oRow.DayOf = 7 Then iWeekDay = 0
                        oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.BELLINFO, False)
                        oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "ID=" & iSirenId.ToString & vbTab & "Valid=1" & vbTab & "Time=" & oRow.StartDate.Hour.ToString.PadLeft(2, "0") & oRow.StartDate.Minute.ToString.PadLeft(2, "0") & vbTab & "WavIndex=1" & vbTab & "Times=5" & vbTab & "Way=1" & vbTab & "Volume=0" & vbTab & "Sun=" & IIf(iWeekDay = 0, "1", "0") & vbTab & "Mon=" & IIf(iWeekDay = 1, "1", "0") & vbTab & "Tue=" & IIf(iWeekDay = 2, "1", "0") & vbTab & "Wed=" & IIf(iWeekDay = 3, "1", "0") & vbTab & "Thu=" & IIf(iWeekDay = 4, "1", "0") & vbTab & "Fri=" & IIf(iWeekDay = 5, "1", "0") & vbTab & "Sat=" & IIf(iWeekDay = 6, "1", "0") & vbTab & "ExtbellDelay=" & oRow.Duration.ToString)

                        iSirenId = iSirenId + 1

                        ' Duración. Nos quedamos con el último (aunque en pantalla se pueda configurar uno para cada sirena)
                        iSirenDuration = oRow.Duration
                    Next

                    ' Parámetros comunes para habilitar sirenas
                    Return res
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalSirensAlt1::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        Public Function GetTerminalCauses(sTaskData As String, ByRef oMessage As MessageZKPush2) As String
            Dim lRet As String = String.Empty
            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.CausesDataTable = dsLocalData.Causes
                Dim oRow As LocalDataSet.CausesRow
                Dim res As String = ""

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageZKPush2.msgTables.WORKCODE, True)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageZKPush2.msgTables.WORKCODE, True)
                        oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "PIN=" & oRow.IDCause & vbTab & "CODE=" & oRow.ReaderInputCode & vbTab & "NAME=" & oRow.Name)
                        'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "Code|" & oRow.ReaderInputCode & "," & "Name|" & oRow.Name)
                    Next
                End If
                lRet = ""
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::GetTerminalCauses::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                lRet = ""
            End Try
            Return lRet
        End Function

        Public Sub ProcessCommandResult(oCommandResults As ArrayList)
            Try
                Dim oTask As Base.roTerminalsSyncTasks = New roTerminalsSyncTasks(mTerminal.ID)
                If oCommandResults.Count > 0 Then
                    For Each cmdRes As CommandResult In oCommandResults
                        'roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::ProcessCommandResult::Command = " & cmdRes.CMD)
                        If CInt(cmdRes.ID) > 0 Then
                            Dim lTaskId As Long = Math.Truncate(cmdRes.ID / 10)
                            oTask.Load(lTaskId)
                            If cmdRes.ReturnCode >= 0 Then
                                ' El terminal proceso correctamente el comando
                                oTask.DoneEx(lTaskId)
                                If oTask.Task = roTerminalsSyncTasks.SyncActions.delallemployees Then
                                    bRunningWithoutAdmin = True
                                End If
                            Else
                                If cmdRes.CMD.StartsWith("DATA QUERY") Then
                                    ' Aunque el resultado no sea correcto, los comandos de consulta los elimino. Serán casos realizados en actuaciones de Soporte.
                                    oTask.DoneEx(lTaskId)
                                Else
                                    Select Case cmdRes.ReturnCode
                                        Case -10
                                            ' Asignación a un empleado que no existe. No hay empleado, es un error. La borro.
                                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask failed because employee does not exist on terminal (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Ignoring")
                                            oTask.DoneEx(lTaskId)
                                        Case -3
                                            Select Case oTask.Task
                                                Case roTerminalsSyncTasks.SyncActions.delbio
                                                    ' TODO: Esto es un error del protocolo de ZK. De momento ignoro este error, porque es un borrado de huella que funcionó, o bien de un empleado que no existía, pero devuelve -3
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask " + oTask.Task.ToString + " presumably succeeded (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Deleting as done!")
                                                    oTask.DoneEx(lTaskId)
                                                Case roTerminalsSyncTasks.SyncActions.addbio
                                                    ' La huella no parece correcta. Es un registro de huella borrada en algún momento
                                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Fingerprint seems corrupted!. (task=" + oTask.ToString + "). Ignoring!")
                                                    oTask.DoneEx(lTaskId)
                                                    ' TODO: Crear una tarea para avisar que hay una huella que no se ha programado correctamente
                                                Case Else
                                                    ' Error. Reseteo para que se vuelva a enviar
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask failed (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Reset sync process for terminal " + mTerminal.ID.ToString)
                                                    'oTask.ResetAllWithDelay(10)
                                                    oTask.ResetAll()
                                            End Select
                                        Case -1
                                            Select Case oTask.Task
                                                Case roTerminalsSyncTasks.SyncActions.addemployeetimezones, roTerminalsSyncTasks.SyncActions.delallemployeetimezones
                                                    ' TODO: Esto es un error del protocolo de ZK. De momento ignoro este error, porque es un borrado o asignación de TZ a un empleado que no existe, pero devuelve -1
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask " + oTask.Task.ToString + " for an unexistent employee (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Deleting as done!")
                                                    oTask.DoneEx(lTaskId)
                                                Case roTerminalsSyncTasks.SyncActions.setterminalconfig
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask " + oTask.Task.ToString + " not supported config parameter specified (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Deleting as done!")
                                                    oTask.DoneEx(lTaskId)
                                                Case roTerminalsSyncTasks.SyncActions.delemployee
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask " + oTask.Task.ToString + " for an unexistent employee (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Deleting as done!")
                                                    oTask.DoneEx(lTaskId)
                                                Case Else
                                                    ' Error. Reseteo para que se vuelva a enviar
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask failed (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Reset sync process for terminal " + mTerminal.ID.ToString)
                                                    'oTask.ResetAllWithDelay(10)
                                                    oTask.ResetAll()
                                            End Select
                                        Case -9, -11, -12
                                            Select Case oTask.Task
                                                Case roTerminalsSyncTasks.SyncActions.addbio
                                                    ' La huella no parece correcta. Es un registro de huella borrada en algún momento
                                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::GetNextCommand: Fingerprint void or invalid format!. (task=" + oTask.ToString + "). Ignoring!")
                                                    oTask.DoneEx(lTaskId)
                                            End Select
                                        Case Else
                                            ' Error. Reseteo para que se vuelva a enviar
                                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult::SyncTask failed (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Reset sync process for terminal " + mTerminal.ID.ToString)
                                            'oTask.ResetAllWithDelay(10)
                                            oTask.ResetAll()
                                    End Select
                                End If
                            End If
                            ' Si no quedan tareas, borro la alerta de sincronización bloqueada, si existe ...
                            oTask.LoadNext()
                            If oTask.Task = roTerminalsSyncTasks.SyncActions.none Then
                                DelUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED")
                            End If
                        End If
                        'Comandos que no vienen de terminalsynctasks
                        If CInt(cmdRes.ID) = 0 Then
                            If cmdRes.CMD.StartsWith("REBOOT") Then
                                ' Si fue un reinicio, actualizo el estado de DST en BBDD porque estoy seguro que el cambio tendrá efecto
                                If cmdRes.ReturnCode >= 0 Then
                                    mTerminal.UpdateDSTState(mTerminal.IsInDST)
                                    Me.bRebootPending = False
                                End If
                            End If
                        End If
                    Next
                Else
                    ' Llego una respuesta a comando, pero sin resultados !!!!!!. El terminal puede estar "cuajado". Retraso todas las tareas 10 segundos.
                    ' TODO: Cuando esto ha ocurrido, no se recupera hasta que se reinicia. De momento no lo hacemos
                    '       Pero podríamos cargar la última tarea enviada, y si lleva muchos reintentos, reiniciar el terminal.
                    ' --- LO QUITAMOS. LOS rxF lo hacen con asiduidad, y hace que las tareas puedan irse a futuro varios minutos !!!
                    'roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult:Terminal " + mTerminal.ToString + " : Response with no results !!!!! ")
                    'oTask.ResetAllWithDelay(10)
                End If

                If oTask IsNot Nothing Then oTask = Nothing
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Sub ProcessCommandResult(oCommandID As Integer)
            Try
                Dim oTask As Base.roTerminalsSyncTasks = New roTerminalsSyncTasks(mTerminal.ID)
                If oCommandID >= 0 Then
                    ' Correcto
                    oTask.Done(Math.Truncate(oCommandID / 10))
                Else
                    ' Error. No borro
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessCommandResult:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Sub ProcessDeviceParameters(oDeviceparameters As Dictionary(Of String, String))
            Try
                ' Función para recuperar info del terminal
                ' En rxC y rxCe PUSH toda la info se devuelve como respuesta a INFO
                ' En rx1, la versión de firmware viene en respuesta a INFO, pero el resto, viene en respuesta a un GET OPTION ....
                ' Versión de firmware
                Dim sKey As String = String.Empty
                Dim sFirmVersion As String = String.Empty
                Dim sUserCount As String = String.Empty
                Dim sFaceCount As String = String.Empty
                Dim sPalmCount As String = String.Empty
                Dim sFingerCount As String = String.Empty
                Dim sWifiOn As String = String.Empty
                Dim bUpdateOthers As Boolean = False
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.FWVersion)
                If oDeviceparameters.ContainsKey(sKey) Then sFirmVersion = oDeviceparameters(sKey)
                If sFirmVersion.Trim.Length > 0 Then
                    mTerminal.UpdateFirmVersion(sFirmVersion)
                End If

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.UserCount)
                If oDeviceparameters.ContainsKey(sKey) Then sUserCount = oDeviceparameters(sKey)
                If sUserCount.Trim.Length > 0 Then
                    mTerminal.UpdateUserCount(sUserCount)
                End If

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.FaceCount)
                If oDeviceparameters.ContainsKey(sKey) Then sFaceCount = oDeviceparameters(sKey)
                If sFaceCount.Trim.Length > 0 Then
                    mTerminal.UpdateFaceCount(sFaceCount)
                End If

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.PvCount)
                If oDeviceparameters.ContainsKey(sKey) Then sPalmCount = oDeviceparameters(sKey)
                If sPalmCount.Trim.Length > 0 Then
                    mTerminal.UpdatePalmCount(sPalmCount)
                End If

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.FPCount)
                If oDeviceparameters.ContainsKey(sKey) Then sFingerCount = oDeviceparameters(sKey)
                If sFingerCount.Trim.Length > 0 Then
                    mTerminal.UpdateFingerCount(sFingerCount)
                End If

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WifiOn)
                If oDeviceparameters.ContainsKey(sKey) Then sWifiOn = oDeviceparameters(sKey)
                If sWifiOn.Trim.Length > 0 Then
                    mTerminal.UpdateWifiOn(sWifiOn.Trim.Chars(0))
                End If

                Dim sOther As String = String.Empty
                If mTerminal.Other.Contains("InDST:True") OrElse mTerminal.Other.Contains("InDST=True") Then sOther = "InDST:True"
                If mTerminal.Other.Contains("InDST:False") OrElse mTerminal.Other.Contains("InDST=False") Then sOther = "InDST:False"

                Dim sLocalIP As String = String.Empty
                Dim sGateway As String = String.Empty
                Dim sDNS As String = String.Empty
                Dim sSubNetMask As String = String.Empty
                Dim WebServerURLModel As String = String.Empty
                Dim sICLOCKSVRURL As String = String.Empty
                Dim sWebServerIP As String = String.Empty
                Dim sWebServerPort As String = String.Empty

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.IPAddress)
                If oDeviceparameters.ContainsKey(sKey) Then sLocalIP = oDeviceparameters(sKey) : bUpdateOthers = True
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.GATEIPAddress)
                If oDeviceparameters.ContainsKey(sKey) Then sGateway = oDeviceparameters(sKey) : bUpdateOthers = True
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.DNS)
                If oDeviceparameters.ContainsKey(sKey) Then sDNS = oDeviceparameters(sKey) : bUpdateOthers = True
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.NetMask)
                If oDeviceparameters.ContainsKey(sKey) Then sSubNetMask = oDeviceparameters(sKey) : bUpdateOthers = True

                sOther = sOther & ";" & "LocalIP:" & sLocalIP
                sOther = sOther & ";Gateway:" & sGateway
                sOther = sOther & ";DNS:" & sDNS
                sOther = sOther & ";SubNetMask:" & sSubNetMask

                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerURLModel)
                If oDeviceparameters.ContainsKey(sKey) Then WebServerURLModel = oDeviceparameters(sKey)
                If WebServerURLModel = "1" Then
                    ' Función DNS
                    sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.ICLOCKSVRURL)
                    If oDeviceparameters.ContainsKey(sKey) Then sICLOCKSVRURL = oDeviceparameters(sKey)
                    sOther = sOther & ";ServerURL:" & sICLOCKSVRURL
                Else
                    ' Función IP
                    sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerIP)
                    If oDeviceparameters.ContainsKey(sKey) Then sWebServerIP = oDeviceparameters(sKey)
                    sOther = sOther & ";ServerIP:" & sWebServerIP
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerPort)
                If oDeviceparameters.ContainsKey(sKey) Then sWebServerPort = oDeviceparameters(sKey)
                sOther = sOther & ";ServerPort:" & sWebServerPort

                If bUpdateOthers Then
                    mTerminal.UpdateOther(sOther)
                    mTerminal.Other = sOther
                End If

                ' Detección de temperatura y máscara
                Dim oSecurityOptions As New clsSecurityOptions
                Dim bUpdateSecurityOptions As Boolean = False
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.IRTempThreshold)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.IRTempThreshold = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.IRTempLowThreshold)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.IRTempLowThreshold = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.EnalbeIRTempDetection)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.EnalbeIRTempDetection = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.EnableNormalIRTempPass)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.EnableNormalIRTempPass = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.EnalbeMaskDetection)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.EnalbeMaskDetection = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.EnableWearMaskPass)
                If oDeviceparameters.ContainsKey(sKey) AndAlso IsNumeric(oDeviceparameters(sKey)) Then
                    oSecurityOptions.EnableWearMaskPass = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If
                sKey = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.BiometricVersion)
                If oDeviceparameters.ContainsKey(sKey) Then
                    oSecurityOptions.BiometricVersion = oDeviceparameters(sKey)
                    bUpdateSecurityOptions = True
                End If

                'Serializamos
                If bUpdateSecurityOptions Then
                    Dim strOsecurityOptionsDefinition As String = String.Empty
                    strOsecurityOptionsDefinition = roJSONHelper.Serialize(oSecurityOptions)
                    mTerminal.UpdateSecurityOptions(strOsecurityOptionsDefinition)
                    mTerminal.SecurityOptionsDefinition = strOsecurityOptionsDefinition
                End If

                ' Tratamiento de resultado de comandos SHELL
                Try
                    Dim sKeyCMD = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.CMD)
                    Dim sKeyID = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.ID)
                    If oDeviceparameters.ContainsKey(sKeyCMD) AndAlso oDeviceparameters(sKeyCMD) = "Shell" AndAlso oDeviceparameters.ContainsKey(sKeyID) Then
                        Dim oTask As Base.roTerminalsSyncTasks = New roTerminalsSyncTasks(mTerminal.ID)
                        Dim lTaskId As Long = Math.Truncate(oDeviceparameters(sKeyID) / 10)
                        oTask.Load(lTaskId)
                        'Si hay Content en la respuesta al comando SHELL, la guardo ahora.
                        Dim sKeyContent = System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.Content)
                        If oDeviceparameters.ContainsKey(sKeyContent) Then
                            mTerminal.SaveShellCommandResult(mTerminal.ID, oTask.Task.ToString, oTask.TaskData, Now, oDeviceparameters(sKeyContent))
                        Else
                            mTerminal.SaveShellCommandResult(mTerminal.ID, oTask.Task.ToString, oTask.TaskData, Now, String.Empty)
                        End If
                        oTask.DoneEx(lTaskId)
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessDeviceParameters:Terminal " + mTerminal.ToString + " :Error recovering shell command response.", ex)
                End Try

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessDeviceParameters:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub CreateUserTask()
            Try
                Dim oState As New VTBusiness.UserTask.roUserTaskState()
                Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString, oState)
                If oTaskExist.Message = "" Then
                    Dim oTask As New VTBusiness.UserTask.roUserTask()
                    With oTask
                        .ID = "USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString
                        Language.ClearUserTokens()
                        Me.Language.AddUserToken(mTerminal.ID.ToString) : Me.Language.AddUserToken(mTerminal.SN.ToString) : Me.Language.AddUserToken(Me.strTerminalLocation) : Me.Language.AddUserToken(mTerminal.TerminalType)
                        Dim arrList As New ArrayList
                        .Message = Me.Language.Translate("TerminalNotRegistered.Title", "")
                        Me.Language.ClearUserTokens()
                        .DateCreated = Now
                        .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                        .ResolverURL = "FN:\\mxC_NotRegistered"
                        .ResolverVariable1 = "TerminalID" : .ResolverValue1 = mTerminal.ID.ToString
                        .ResolverVariable2 = "Serial" : .ResolverValue2 = mTerminal.SN.ToString
                        .ResolverVariable3 = "Location" : .ResolverValue3 = mTerminal.IP
                        .Save()
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::CreateUserTask:" & mTerminal.ToString & ":User task created.")
                    End With
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::CreateUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub DelUserTask()
            Try
                Dim oState As New VTBusiness.UserTask.roUserTaskState()
                Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString, oState)
                'Si existe la tarea la borramos
                If oTaskExist.Message <> "" Then
                    oTaskExist.Delete()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::DelUserTask:" & mTerminal.ToString & ":User task deleted because the terminal registered yet")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::DelUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub CreateUserTaskGeneric(sTaskID As String, sLangTag As String)
            Dim oState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & mTerminal.ID.ToString, oState)
            If oTaskExist.Message = "" Then
                Dim oTask As New VTBusiness.UserTask.roUserTask()
                With oTask
                    .ID = sTaskID & mTerminal.ID.ToString
                    .Message = sLangTag + "¬" + mTerminal.ID.ToString
                    .DateCreated = Now
                    .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                    .ResolverURL = ""
                    .Save()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::CreateUserTaskGeneric:" & mTerminal.ToString & ":User task created.")
                End With
            End If
        End Sub

        Public Overridable Sub DelUserTaskGeneric(sTaskID As String)
            Try
                Dim oState As New VTBusiness.UserTask.roUserTaskState()
                Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & mTerminal.ID.ToString, oState)
                'Si existe la tarea la borramos
                If oTaskExist.Message <> "" Then
                    oTaskExist.Delete()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::DelUserTaskGeneric:" & mTerminal.ToString & ":User task deleted because the terminal registered yet")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::DelUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Function Decode(ByVal packet As Byte()) As Byte()
            Dim i = packet.Length - 1
            Dim iZeroes As Integer = 0

            While packet(i) = 0
                i -= 1
                iZeroes = iZeroes + 1
            End While

            ' Compacto si hay más de 50 leading ceros
            If iZeroes > 50 Then
                Dim temp = New Byte(i + 1 - 1 + 50) {}
                Array.Copy(packet, temp, i + 1 + 50)
                Return temp
            Else
                Return packet
            End If
        End Function

    End Class

End Namespace