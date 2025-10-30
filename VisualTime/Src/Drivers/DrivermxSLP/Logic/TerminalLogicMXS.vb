Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Comms.Base
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class TerminalLogicMxS
        Inherits CTerminalLogicBase

#Region "Constantes de los mensajes"

        Protected Const RightBtn_OK = "ok"

#End Region

#Region "Variables Protected"

        Public mTerminal As TerminalMxS = New TerminalMxS("mxS")
        Public mLastId As Integer = 0
        Public oParent As Object
        Public dLastSend As DateTime
        Public dlastReceived As DateTime
        Public dlastConnectionStatusChanged As DateTime = Now
        Public dlastConfigChangesCheck As DateTime = Now
        Public bConfigChangesPending As Boolean = False
        Public LastMessage As MessageMxS
        Public mCurrentEmployee As clsTerminalEmployee
        Public mCurrentPunch As clsTerminalPunch
        Public mSendMessage As MessageMxS
        Public _RegistryRoot As String
        Public oSettings As roSettings = New roSettings
        Public mConfigPath As String
        Public oCurrentTask As roTerminalsSyncTasks
        Public iTasksInMessage As Integer = 0
        Public bNeedReInitialization As Boolean = False
        Public oCheckConnectionThread As Thread
        Public bTerminalConnected As Boolean = False
        Public bForceInitSession As Boolean = False
        Public dlastTimeSync As DateTime
        Public bError As Boolean = False
        Public bSavePunch As Boolean = True
        Public Language As New Robotics.VTBase.roLanguage()
        Public mRegistryCode As String = String.Empty
        Public mCurrentSessionID As String = String.Empty
        Public dLastRebootCheck As DateTime = Now
        Public bGetNetworkConfig As Boolean = False
        Public dlastTimeSyncLog As DateTime
        Public bRunningOnIIS As Boolean = False
        Public mCardType As clsParameters.eCardType = clsParameters.eCardType.Unique
        Public mMifareCardBytesRead As Integer = 4
        Public mCardCodificationToSend As String = String.Empty

        'Gestión de log de eventos
        Public sDetail As String = ""

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
            MyBase.New("MXS", "online")
            dlastTimeSyncLog = Now.AddMinutes(-15)
            mTerminal = New TerminalMxS("online", oTerminal)
            dlastTimeSyncLog = Now.AddMinutes(-15)
            bRunningOnIIS = True
        End Sub

        Public Overrides Function Initialize(strSN As String, strIP As String, strPort As String, strModel As String) As Boolean
            Try

                Dim bResponse As Boolean = False

                ' Informamos parámetros de terminal
                mTerminal.SN = strSN
                mTerminal.IP = strIP
                mTerminal.Port = strPort
                mTerminal.Model = strModel

                Dim sCardType = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("MT.PUSHTerminals.CardType", Nothing, False))
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

                ' Surprisingly, lenght of cards can be different in different terminals when reader is mifare. Let's check now ...
                mMifareCardBytesRead = 4
                If mCardType = clsParameters.eCardType.Mifare OrElse mCardType = clsParameters.eCardType.MifareNumeric Then
                    ' By default, mxS mifare reads 3 bytes ...
                    If mTerminal.Model.ToUpper = "MXS" Then
                        mMifareCardBytesRead = 3
                    End If

                    ' But with ZK devices you never know ...
                    Dim sMifareCardBytesRead = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("MT.PUSHTerminals.MifareCardBytesReadOnMxS", Nothing, False))
                    If sMifareCardBytesRead <> "" Then
                        mMifareCardBytesRead = roTypes.Any2Integer(sMifareCardBytesRead)
                    End If
                End If

                ' ... really never
                Dim cardCodificationToSend = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("MT.PUSHTerminals.CardCodification", Nothing, False))
                If cardCodificationToSend <> "" Then
                    mCardCodificationToSend = cardCodificationToSend
                End If

                ' Aquí llega la info de número de lectores. Doy de alta el terminal si no lo estaba ya
                If mTerminal.SN.Trim <> "" Then
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::Initialize:" & mTerminal.ToString & ": Terminal accepted.")
                    bResponse = CheckTerminalDB(mTerminal.SN)
                    'En IIS no se requiere registro
                    If Not mTerminal.CheckRegistrationCode Then
                        mTerminal.ByPassRegitrationCode()
                    End If
                    MyBase.intTerminalID = mTerminal.ID
                    MyBase.strTerminalLocation = mTerminal.IP
                    MyBase.strTerminalType = mTerminal.TerminalType
                    If bResponse Then
                        oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                        oCurrentTask.ResetAll()
                        mTerminal.UpdateStatus(True)
                        Me.NeedReInitialization = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::Initialize:" + mTerminal.ToString + ":Terminal identified")
                    End If
                    Return bResponse
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::Initialize:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        Public Function ReInitialize() As Boolean
            Try
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::ReInitialize:Driver" + MyBase.Driver + "(" +
                                System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion +
                                ") reinicialized: " +
                                "Terminal: " + mTerminal.ToString + ". Communication restablished!")
                oCurrentTask = New roTerminalsSyncTasks(mTerminal.ID)
                oCurrentTask.ResetAll()
                Me.NeedReInitialization = False
                mTerminal.LoadRegistrationCode()
                'mTerminal.UpdateStatus(True)
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::ReInitialize:" + mTerminal.ToString + ":Error:", ex)
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
                        Dim oTerminalTmp As roTerminal = roTerminal.GetTerminalBySerialNumber(mTerminal.DBTerminal.SerialNumber, oTermState)
                        If mTerminal.TimeZoneName <> oTerminalTmp.TimeZoneName Then
                            Me.bForceInitSession = True
                        End If
                        mTerminal.Load()
                        bConfigChangesPending = True
                        roTerminal.ClearForceConfig(mTerminal.ID, oTermState)
                    End If

                    dlastConfigChangesCheck = Now
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::CheckConnectionStatusOnIIS:" + mTerminal.ToString + ":Error:", ex)
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
            Return Nothing
        End Function

        Public Overrides Sub ResumeStart()

        End Sub

        Public Overrides Sub TerminalTask(_IDEmployee As Integer, _Action As CTerminalLogicBase.TerminalTaskAction, Optional _IDTerminal As Integer = -1)

        End Sub

        ''' <summary>
        ''' Cargamos datos del fichaje
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function LoadCurrentData(ByVal Message As MessageMxS, ByRef bSavePunch As Boolean) As Boolean
            Try
                'En principio guardo el fichaje
                bSavePunch = True

                'Cargamos en memoria los datos del empleado y del marajes
                mCurrentEmployee = New clsTerminalEmployee()
                'Si se identificó al empleado
                If Message.data_event.PIN > 0 Then
                    mCurrentEmployee.Load(Message.data_event.PIN)
                    mCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, Message.data_event.PIN, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)

                    If mCurrentEmployee.ID > 0 Then
                        mCurrentPunch.IDEmployee = mCurrentEmployee.ID
                        If mCurrentPunch.Card(mCardType).Length = 0 Then mCurrentPunch.Card(mCardType) = mCurrentEmployee.Card
                        mCurrentPunch.Load()
                    End If

                    Return True
                Else
                    If Message.data_event.PIN = 0 AndAlso Message.data_event.VerifyType = 4 Then
                        'Algunas centralitas NO ENVIAN EL PIN en fichajes correctos con tarjeta !!!!!!!!!!!!
                        mCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, Message.data_event.PIN, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)
                        mCurrentEmployee.Load(0, mCurrentPunch.Card(mCardType))
                        If mCurrentEmployee.ID > 0 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::LoadCurrentData:" + mTerminal.ToString + ":Empleado identificado " & mCurrentEmployee.ID.ToString)
                            mCurrentPunch.IDEmployee = mCurrentEmployee.ID
                            If mCurrentPunch.Card(mCardType).Length = 0 Then mCurrentPunch.Card(mCardType) = mCurrentEmployee.Card
                            mCurrentPunch.Load()
                        End If

                        Return True
                    Else
                        ' Si se fichó con tarjeta, se trata de una tarjeta no asignada.
                        If Message.data_event.VerifyType = "4" Then
                            mCurrentEmployee.Load(-1)
                            mCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, 0, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)
                        Else
                            ' No se identificó al empleado, y no fichó con tarjeta. No guardo el fichaje
                            bSavePunch = False
                        End If
                        Return True
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::LoadCurrentData:" + mTerminal.ToString + ":Error:", ex)
                bSavePunch = False
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Cargamos datos del fichaje
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function LoadCurrentDataIIS(ByVal Message As MessageMxS, ByRef bSavePunch As Boolean, ByRef oCurrentPunch As clsTerminalPunch) As Boolean
            Try
                'En principio guardo el fichaje
                bSavePunch = True

                'Cargamos en memoria los datos del empleado y del marajes
                mCurrentEmployee = New clsTerminalEmployee()
                'Si se identificó al empleado
                If Message.data_event.PIN > 0 Then
                    mCurrentEmployee.Load(Message.data_event.PIN)
                    oCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, Message.data_event.PIN, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)

                    If mCurrentEmployee.ID > 0 Then
                        oCurrentPunch.IDEmployee = mCurrentEmployee.ID
                        If oCurrentPunch.Card(mCardType).Length = 0 Then oCurrentPunch.Card(mCardType) = mCurrentEmployee.Card
                        oCurrentPunch.Load()
                    End If

                    Return True
                Else
                    If Message.data_event.PIN = 0 AndAlso Message.data_event.VerifyType = 4 Then
                        'Algunas centralitas NO ENVIAN EL PIN en fichajes correctos con tarjeta !!!!!!!!!!!!
                        oCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, Message.data_event.PIN, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)
                        mCurrentEmployee.Load(0, oCurrentPunch.Card(mCardType))
                        If mCurrentEmployee.ID > 0 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::LoadCurrentData:" + mTerminal.ToString + ":Empleado identificado " & mCurrentEmployee.ID.ToString)
                            oCurrentPunch.IDEmployee = mCurrentEmployee.ID
                            If oCurrentPunch.Card(mCardType).Length = 0 Then oCurrentPunch.Card(mCardType) = mCurrentEmployee.Card
                            oCurrentPunch.Load()
                        End If

                        Return True
                    Else
                        ' Si se fichó con tarjeta, se trata de una tarjeta no asignada.
                        If Message.data_event.VerifyType = "4" Then
                            mCurrentEmployee.Load(-1)
                            oCurrentPunch = New clsTerminalPunch(mTerminal, Message.data_event.PunchDateTime, 0, Message.data_event.Card, Message.data_event.Reader, Message.data_event.Action, 0, Nothing)
                        Else
                            ' No se identificó al empleado, y no fichó con tarjeta. No guardo el fichaje
                            bSavePunch = False
                        End If
                        Return True
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::LoadCurrentData:" + mTerminal.ToString + ":Error:", ex)
                bSavePunch = False
                Return False
            End Try
        End Function

        Protected Overridable Function CheckTerminalValid(ByVal oMessage As MessageMxS) As Boolean
            Try
                Return oMessage.SN <> ""
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::CheckTerminalValid:" + mTerminal.ToString + ":Error:", ex)
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
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::CheckTerminalDB:" & mTerminal.ToString & ": Checking if terminal is registered.")
                If mTerminal.Exist Then
                    mTerminal.Model = "mxS"
                    Return mTerminal.Load
                Else
                    If mTerminal.CreateNew() Then
                        If mTerminal.Load() Then
                            'Hemos creado el terminal. Me aseguro que se programa desde cero
                            If Not bRunningOnIIS Then
                                My.Computer.FileSystem.DeleteDirectory(mTerminal.PathFiles, FileIO.DeleteDirectoryOption.DeleteAllContents)
                                CallBroadcaster(mTerminal.ID)
                            Else
                                mTerminal.ForceFullDataSync()
                                CallBroadcaster(mTerminal.ID)
                            End If

                            Return True
                        End If
                    End If
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::CheckTerminalDB:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Comprueba si hay que crear algún reader más (el terminal se crea por defecto con dos, pero hasta el mensaje de registro no se sabe cuantos hay realmente en la centralita
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function CheckTerminalReaders(iReaders As Byte) As Boolean
            Try
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalLogic" + MyBase.Driver + "::CheckTerminalDB:" & mTerminal.ToString & ": Checking number of readers.")
                If mTerminal.Exist Then
                    'If iReaders > 2 Then iReaders = 2
                    If iReaders <> mTerminal.RDRCount Then
                        mTerminal.AddReaders(mTerminal.RDRCount, iReaders)
                        Return mTerminal.Load
                    Else
                        Return True
                    End If
                Else
                    If mTerminal.CreateNew(iReaders) Then
                        If mTerminal.Load() Then
                            'Hemos creado el terminal. Me aseguro que se programa desde cero
                            If Not bRunningOnIIS Then
                                My.Computer.FileSystem.DeleteDirectory(mTerminal.PathFiles, FileIO.DeleteDirectoryOption.DeleteAllContents)
                                CallBroadcaster(mTerminal.ID)
                            Else
                                mTerminal.ForceFullDataSync()
                                CallBroadcaster(mTerminal.ID)
                            End If
                            Return True
                        End If
                    End If
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::CheckTerminalReaders:" + mTerminal.ToString + ":Error:", ex)
                Return False
            End Try

        End Function

        Public Overrides Function ParseMessage(ByRef bInput() As Byte) As Robotics.Comms.Base.CMessageBase
            Try
                If bInput.Length > 0 Then
                    Return New MessageMxS(bInput)
                Else
                    Return New MessageMxS
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogic" + MyBase.Driver + "::ParseMessage:" + mTerminal.ToString + ":Error:", ex)
                Return New MessageMxS
            End Try
        End Function

        Protected Sub Send(ByVal oMessage As MessageMxS)
            Try
                Try
                    'Hacemos una pequeña parada porque un envio inmediato no funciona
                    System.Threading.Thread.Sleep(300)
                    dLastSend = Now
                    '                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, oMessage.toString)
                    MyBase.RaiseOnSendMessageEx(oMessage, mTerminal.SN, Me, False)
                    mSendMessage = Nothing
                    'Historial de los dos ultimos mensaje
                    LastMessage = oMessage
                Catch ex As Exception
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicMxS::Send:" + mTerminal.ToString + ": Error: '" + ex.Message + "'", ex)
                End Try
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::Send:" + mTerminal.ToString + ":Error:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Traduce una o varias acciones de sincronización a comandos para INBIO
        ''' </summary>
        ''' <param name="oMessage"></param>
        ''' <remarks></remarks>
        Public Function GetNextCommand(ByRef oMessage As MessageMxS, oSyncTask As roTerminalsSyncTasks, sHttpVersion As String, ByRef bMaxTasksReached As Boolean, ByRef bCallBroadcaster As Boolean) As Boolean
            Try
                ' Cargo datos del empleado
                mCurrentEmployee = New clsTerminalEmployee()
                If oSyncTask.IDEmployee > 0 Then
                    ' Cargo información de empleado por si es precisa
                    Dim bEmployeeLoaded As Boolean = False
                    Dim bEmployeeInfoRequired As Boolean = True
                    Dim bEmployeeExists As Boolean = True
                    bEmployeeInfoRequired = Not (oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delbio OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delcard OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployee)
                    If bEmployeeInfoRequired Then
                        bEmployeeExists = mCurrentEmployee.EmployeeExists(oSyncTask.IDEmployee)
                        bEmployeeLoaded = mCurrentEmployee.Load(oSyncTask.IDEmployee)
                    End If
                    If bEmployeeLoaded OrElse Not bEmployeeInfoRequired Then
                        Select Case oSyncTask.Task
                            Case roTerminalsSyncTasks.SyncActions.addemployee, roTerminalsSyncTasks.SyncActions.addcard
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                ' Nuevo protocolo. Primero tengo que borrar, por si se trata de un cambio
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.mulcarduser, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)

                                ' Información general. Aquí la tarjeta no se pasa (porque va en otra tabla)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.user, True)
                                Dim cardString As String = mdPublic.ConvertCardForTerminal(mCurrentEmployee.Card.ToString, mCardType,, mMifareCardBytesRead)
                                If mCardCodificationToSend.Trim <> String.Empty Then
                                    Select Case mCardCodificationToSend.ToUpper
                                        Case "HEX"
                                            cardString = Hex(cardString).ToLower
                                        Case "ROTATEDHEX"
                                            cardString = clsPushProtocol.ConvertCard(cardString)
                                    End Select
                                End If

                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.CardNo, cardString) ' La tarjeta no la trata en la tabla user ... pues con el nuevo firmware si
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Password, mCurrentEmployee.PIN)

                                ' Nuevo protocolo InBio Pro para pasar la tarjeta
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.mulcarduser, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)

                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.CardNo, cardString)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.LossCardFlag, "0")
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.CardType, "0")

                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 5)
                            Case roTerminalsSyncTasks.SyncActions.addbio
                                ' Por el momento, no se envían huellas a centralitas InBio por decisión de producto
                                'If oMessage Is Nothing Then oMessage = New MessageMxC(MessageMxC.msgCommand.command, MessageMxC.msgTables.commands, True, sHttpVersion)
                                'oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxC.msgTables.templatev10, True)
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Size, "0")
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.FingerID, oSyncTask.IDFinger)
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Template, New String(System.Text.Encoding.UTF8.GetChars(mCurrentEmployee.BioData(oCurrentTask.IDFinger))))
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Valid, "1")
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Resverd, "")
                                'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.EndTag, "")
                                'iTasksInMessage = iTasksInMessage + 1
                                'bMaxTasksReached = (iTasksInMessage = 5)
                                ''Log
                                'Try
                                '    params(iTasksInMessage) = mCurrentEmployee.Name + ";" + oSyncTask.IDFinger.ToString
                                '    iEvent = CTerminalLogicBase.mxCInbioEventID.AddBio
                                'Catch ex As Exception
                                'End Try
                                oSyncTask.DoneEx(oSyncTask.ID)
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetNextCommand: Fingerprint not present on device. Ignoring task = " + oSyncTask.ToString)
                            Case roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel
                                ' Para cada TimeZone y Empleado, programa por qué puertas tiene acceso concedido
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.userauthorize, True)
                                Dim sLogInfo As String = ""
                                Dim sEmployeeAccessLevelDef As String = GetEmployeeAccessLevelDefinition(oSyncTask.TaskData, sLogInfo)
                                If sEmployeeAccessLevelDef <> "" Then
                                    oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sEmployeeAccessLevelDef)
                                Else
                                    'No pude recuperar la definición del timezone. Debo forzar reprogramación de Timezones
                                    DeleteTimeZoneFile()
                                    bCallBroadcaster = True
                                    oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                                    oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetCommandsFromTask: Unable to load timezone definition!. (task=" + oSyncTask.ToString + ")")
                                    Return True 'No la reseteo porque no es tratable, y encualquier caso Broadcaster la volverá a generar
                                End If
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 5)
                            Case roTerminalsSyncTasks.SyncActions.delbio
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.templatev10, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.FingerID, oSyncTask.IDFinger)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 10)
                            Case roTerminalsSyncTasks.SyncActions.delcard
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.user, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.CardNo, "")
                                ' Nuevo protocolo. Primero tengo que borrar, por si se trata de un cambio
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.mulcarduser, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 10)
                            Case roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.userauthorize, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.AuthorizeTimezoneId, oSyncTask.Parameter1)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 10)
                            Case roTerminalsSyncTasks.SyncActions.delemployee
                                If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.user, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                ' Nuevo protocolo. Primero tengo que borrar, por si se trata de un cambio
                                oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.mulcarduser, True)
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Pin, oSyncTask.IDEmployee)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 10)
                            Case Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetCommandsFromTask: Unknown or unexpected task ( " + oSyncTask.ToString + "). Ignoring ...")
                                oSyncTask.DoneEx(oSyncTask.ID)
                                oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                Return True
                        End Select
                    Else
                        If bEmployeeInfoRequired And Not bEmployeeExists Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetNextCommand: Employee " + oSyncTask.IDEmployee.ToString + " does not exist!. Task: " + oSyncTask.ToString + " ignored")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            If iTasksInMessage = 0 Then
                                oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            End If
                            Return True
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetNextCommand: Unable to load employee data. Employee  " + oSyncTask.IDEmployee.ToString + " Task: " + oSyncTask.Task.ToString + " EmployeeID" + oSyncTask.IDEmployee.ToString)
                            ' Retraso todas las tareas relativas a este empleado 1 minuto, para que el resto se procesen
                            oSyncTask.DelayEmployeeTasks(60)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            If iTasksInMessage = 0 Then
                                oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
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
                            mTerminal.RefreshDLSTConfig()
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                            Dim sTerminalConfigParams As String = GetTerminalConfigParams(oSyncTask.TaskData, False)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTerminalConfigParams)

                            ' Añado configuración de DLST
                            oMessage.data_table.addCommand(-2, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DSTOn=1")
                            oMessage.data_table.addCommand(-3, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "DLSTMode=1")

                            oMessage.data_table.addCommand(-4, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.DSTSetting, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                            oMessage.data_table.addCommand(-5, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.DSTSetting, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Year, Now.Year.ToString)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.StartTime, mTerminal.DaylightSavingTime)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.EndTime, mTerminal.StandardTime)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters._Loop, "1")

                            ' Y sincronizo hora
                            oMessage.data_table.addCommand(-1, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.MachineTZ, mTerminal.TimeZone)
                            oMessage.data_table.addCommand(-7, msgdata_table_command.dataCommands.SET_OPTIONS, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.DateTime, clsPushProtocol.ConvertDateTime(Date.UtcNow))
                        Case roTerminalsSyncTasks.SyncActions.getterminalconfig
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.GET_OPTIONS, MessageMxS.msgTables.none, True)
                            Dim sTerminalConfigParams As String = GetTerminalConfigParams(oSyncTask.TaskData, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTerminalConfigParams)
                        Case roTerminalsSyncTasks.SyncActions.getnetworkinfo
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.GET_OPTIONS, MessageMxS.msgTables.none, True)
                            Dim sNetworkParams As String = "IPAddress,GATEIPAddress,DNSAddress,NetMask,IsSupportDNS,WebServerURL,WebServerIP"
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sNetworkParams)
                        Case roTerminalsSyncTasks.SyncActions.delallbios
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.templatev10, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                        Case roTerminalsSyncTasks.SyncActions.delallcards
                            ' TODO: De momento no se pueden borrar todas las tarjetas en un sólo comando sin borrar todos los empleados
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.user, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.Group, "0")
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.CardNo, "")
                        Case roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.userauthorize, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                        Case roTerminalsSyncTasks.SyncActions.delalltimezones
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.timezone, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                        Case roTerminalsSyncTasks.SyncActions.delallemployees
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.user, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.All, "")
                        Case roTerminalsSyncTasks.SyncActions.addtimezone
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.timezone, True)
                            Dim sLogDetail As String = ""
                            Dim sTimeZoneDef As String = GetTimeZoneDefinition(oSyncTask.TaskData, sLogDetail)
                            If sTimeZoneDef <> "" Then
                                oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, sTimeZoneDef)
                            Else
                                'No pude recuperar la definición del timezone. Debo forzar reprogramación de Timezones
                                DeleteTimeZoneFile()
                                bCallBroadcaster = True
                                oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetCommandsFromTask: Unable to load timezone definition!. (task=" + oSyncTask.ToString + ")")
                                Return True 'No la reseteo porque no es tratable, y encualquier caso Broadcaster la volverá a generar
                            End If
                            ' Añadimos la zona 999999 para asignar el periodo de validez
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_UPDATE, MessageMxS.msgTables.timezone, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, GetReadersValidTimezone)
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 2)
                        Case roTerminalsSyncTasks.SyncActions.deltimezone
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_DELETE, MessageMxS.msgTables.timezone, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.TimezoneId, oSyncTask.Parameter1)
                            iTasksInMessage = iTasksInMessage + 1
                            'bMaxTasksReached = (iTasksInMessage = 15)
                            bMaxTasksReached = (iTasksInMessage = 10)
                        Case roTerminalsSyncTasks.SyncActions.getallemployees
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "DSTSetting,fielddesc=*, filter =*")
                            'oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "mulcarduser,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getallemployeeaccesslevel
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "userauthorize,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getallholidays
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "holiday,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getalltimezones
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "timezone,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getalltransactions
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "transaction,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getnewtransactions
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "transaction,fielddesc=*, filter =NewRecord")
                        Case roTerminalsSyncTasks.SyncActions.getallfingerprints
                            'TODO: Tratar esta tarea. Mientras tanto, simplemente respondo ok
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "templatev10,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.getalltransactions
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.DATA_QUERY, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.tablename, "transaction,fielddesc=*, filter =*")
                        Case roTerminalsSyncTasks.SyncActions.reboot
                            If oMessage Is Nothing Then oMessage = New MessageMxS(MessageMxS.msgCommand.command, MessageMxS.msgTables.commands, True, sHttpVersion)
                            oMessage.data_table.addCommand(oCurrentTask.ID, msgdata_table_command.dataCommands.CONTROL_DEVICE, MessageMxS.msgTables.none, True)
                            oMessage.data_table.addCommandParameters(msgdata_table_command.dataParameters.none, "03000000")
                        Case Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::GetCommandsFromTask: Unknown or unexpected task ( " + oSyncTask.ToString + ")")
                            oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                            oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                            Return False
                    End Select
                End If
                If Not mCurrentEmployee Is Nothing Then mCurrentEmployee = Nothing
                Return True
            Catch ex As Exception
                oMessage = New MessageMxS(MessageMxS.msgCommand.genericresponse, , True, sHttpVersion)
                oMessage.data_genericresponse.Status = MsgData.dataStatus.success
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::GetCommandsFromTask for task ( " + oSyncTask.ToString + " ). Error:", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Obtiene una cadena con la definición del timezone cuya definición se guardó en la tarea de sincronización
        ''' </summary>
        ''' <param name="sTaskData"></param>
        ''' <remarks></remarks>
        Protected Function GetTimeZoneDefinition(sTaskData As String, ByRef sLogData As String) As String
            Try
                'Cargamos definiticion a partir del XML
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TimeZonesMxaDataTable = dsLocalData.TimeZonesMxa
                Dim oRow As LocalDataSet.TimeZonesMxaRow
                Dim tmp As String = ""
                Dim tmpLog As String = ""
                Dim idTimeZone As Integer = 0

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        idTimeZone = oRow.IDTimeZone
                        Select Case oRow.IdDayOrHol
                            Case 1
                                'Lunes
                                If tmp <> "" Then tmp += vbTab
                                tmp += "MonTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "MonTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "MonTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 2
                                'Martes
                                If tmp <> "" Then tmp += vbTab
                                tmp += "TueTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "TueTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "TueTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 3
                                'Miercoles
                                If tmp <> "" Then tmp += vbTab
                                tmp += "WedTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "WedTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "WedTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 4
                                'Jueves
                                If tmp <> "" Then tmp += vbTab
                                tmp += "ThuTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "ThuTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "ThuTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 5
                                'Viernes
                                If tmp <> "" Then tmp += vbTab
                                tmp += "FriTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "FriTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "FriTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 6
                                'Sábado
                                If tmp <> "" Then tmp += vbTab
                                tmp += "SatTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "SatTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "SatTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 7
                                'Domingo
                                If tmp <> "" Then tmp += vbTab
                                tmp += "SunTime1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmpLog += ";" + oRow.BeginTime1.ToShortTimeString + ";" + oRow.EndTime1.ToShortTimeString
                                tmp += vbTab + "SunTime2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmpLog += ";" + oRow.BeginTime2.ToShortTimeString + ";" + oRow.EndTime2.ToShortTimeString
                                tmp += vbTab + "SunTime3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                                tmpLog += ";" + oRow.BeginTime3.ToShortTimeString + ";" + oRow.EndTime3.ToShortTimeString
                            Case 8
                                'Festivo1
                                If tmp <> "" Then tmp += vbTab
                                tmp += "Hol1Time1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmp += vbTab + "Hol1Time2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmp += vbTab + "Hol1Time3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                            Case 9
                                'Festivo2
                                If tmp <> "" Then tmp += vbTab
                                tmp += "Hol2Time1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmp += vbTab + "Hol2Time2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmp += vbTab + "Hol2Time3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                            Case 10
                                'Festivo3
                                If tmp <> "" Then tmp += vbTab
                                tmp += "Hol3Time1=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime1, oRow.EndTime1).ToString
                                tmp += vbTab + "Hol3Time2=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime2, oRow.EndTime2).ToString
                                tmp += vbTab + "Hol3Time3=" + clsPushProtocol.FormatTimeZoneSlot(oRow.BeginTime3, oRow.EndTime3).ToString
                        End Select
                    Next
                    tmpLog = idTimeZone.ToString + tmpLog
                    sLogData = tmpLog
                    'Finalmente añado el ID de timezone
                    Return "TimezoneId=" + idTimeZone.ToString + vbTab + tmp
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::GetTimeZoneDefinition::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Devuelve la zona horaria que define cuándo están activos los lectores. Por defecto siempre, todos los lectores.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetReadersValidTimezone() As String
            ' Esta zona horaria debe existeir, y estar asignada a los parámetros Door1ValidTZ, Door2ValidTZ, ...
            'DATA UPDATE timezone TimezoneId=999999	SunTime1=2359	SunTime2=0	SunTime3=0	MonTime1=2359	MonTime2=0	MonTime3=0	TueTime1=2359	TueTime2=0	TueTime3=0	WedTime1=2359	WedTime2=0	WedTime3=0	ThuTime1=2359	ThuTime2=0	ThuTime3=0	FriTime1=2359	FriTime2=0	FriTime3=0	SatTime1=2359	SatTime2=0	SatTime3=0	Hol1Time1=2359	Hol1Time2=0	Hol1Time3=0	Hol2Time1=2359	Hol2Time2=0	Hol2Time3=0	Hol3Time1=2359	Hol3Time2=0	Hol3Time3=0
            Return "TimezoneId=999999	SunTime1=2359	SunTime2=0	SunTime3=0	MonTime1=2359	MonTime2=0	MonTime3=0	TueTime1=2359	TueTime2=0	TueTime3=0	WedTime1=2359	WedTime2=0	WedTime3=0	ThuTime1=2359	ThuTime2=0	ThuTime3=0	FriTime1=2359	FriTime2=0	FriTime3=0	SatTime1=2359	SatTime2=0	SatTime3=0	Hol1Time1=2359	Hol1Time2=0	Hol1Time3=0	Hol2Time1=2359	Hol2Time2=0	Hol2Time3=0	Hol3Time1=2359	Hol3Time2=0	Hol3Time3=0"
        End Function

        Protected Function GetTerminalConfigParams(sTaskData As String, bIsGet As Boolean) As String
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
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::GetTerminalConfigParams::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Obtiene una cadena con la definición del permiso de acceso del empleado
        ''' </summary>
        ''' <param name="sTaskData"></param>
        ''' <remarks></remarks>
        Protected Function GetEmployeeAccessLevelDefinition(sTaskData As String, ByRef sLogInfo As String) As String
            Try
                'Cargamos definiticion a partir del XML
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.EmployeeAccesLevelMxaDataTable = dsLocalData.EmployeeAccesLevelMxa
                Dim oRow As LocalDataSet.EmployeeAccesLevelMxaRow
                Dim tmp As String = ""
                Dim idTimeZone As Integer = 0
                Dim idEmployee As Integer = 0
                Dim iAuthorizedDoors As Byte = 0

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        idEmployee = oRow.IDEmployee
                        idTimeZone = oRow.IdTimezone
                        Try
                            iAuthorizedDoors = IIf(oRow.Door1, 1, 0) + IIf(oRow.Door2, 1, 0) * 2 + IIf(oRow.Door3, 1, 0) * 4 + IIf(oRow.Door4, 1, 0) * 8
                        Catch ex As Exception
                        End Try
                    Next
                    'Info para log
                    sLogInfo = idEmployee.ToString + "," + idTimeZone.ToString + "," + iAuthorizedDoors.ToString
                    'Finalmente añado el ID de timezone
                    Return "AuthorizeDoorId=" & iAuthorizedDoors.ToString & vbTab & "AuthorizeTimezoneId=" & idTimeZone.ToString & vbTab & "Pin=" & idEmployee.ToString
                Else
                    Return ""
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::GetEmployeeAccessLevelDefinition::Error getting info for task data ( " + sTaskData + " ). Error:", ex)
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Procesa el resultado de los comandos
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ProcessCommandResult(oCommandResults As ArrayList)
            Try
                For Each cmdRes As CommandResult In oCommandResults
                    If CInt(cmdRes.ID) > 0 Then
                        Dim oTask As Base.roTerminalsSyncTasks = New roTerminalsSyncTasks(mTerminal.ID)
                        Dim lTaskId As Long = Math.Truncate(cmdRes.ID / 10)
                        If cmdRes.ReturnCode >= 0 Then
                            ' Correcto
                            oTask.DoneEx(lTaskId)
                            oTask.LoadNext()
                            If oTask.Task = roTerminalsSyncTasks.SyncActions.none Then
                                DelUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED")
                            End If
                        Else
                            If cmdRes.CMD = "DATA QUERY" Then
                                ' Aunque el resultado no sea correcto, los comandos de consulta los elimino. Serán casos realizados en actuacones de Soporte.
                                oTask.DoneEx(lTaskId)
                            Else
                                ' Error. Reseteo para que se vuelva a enviar
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::ProcessCommandResult::SyncTask failed (id: " + lTaskId.ToString + ", commandresultcode:" + cmdRes.ReturnCode.ToString + " ). Reset sync process for terminal " + mTerminal.ID.ToString)
                                oTask.ResetAllWithDelay(10)
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::ProcessCommandResult:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Procesa el resultado de los comandos
        ''' </summary>
        ''' <remarks></remarks>
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
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::ProcessCommandResult:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Sub ProcessDeviceParameters(oDeviceparameters As Dictionary(Of String, String))
            Try
                Dim sOther As String = String.Empty
                Dim IsSupportDNS As String = String.Empty
                sOther = "LocalIP:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.IPAddress))
                sOther = sOther & ";Gateway:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.GATEIPAddress))
                sOther = sOther & ";SubNetMask:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.NetMask))
                IsSupportDNS = "1"
                If IsSupportDNS = "1" Then
                    ' Función DNS
                    sOther = sOther & ";ServerURL:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerURL)).Split(":")(0)
                    sOther = sOther & ";ServerPort:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerURL)).Split(":")(1)
                Else
                    ' Función IP
                    sOther = sOther & ";ServerURL:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerIP))
                    sOther = sOther & ";ServerPort:" & oDeviceparameters(System.Enum.GetName(GetType(clsPushProtocol.dataTerminalConfigParameters), clsPushProtocol.dataTerminalConfigParameters.WebServerIP))
                End If

                mTerminal.UpdateOther(sOther)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::ProcessDeviceParameters:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub CreateUserTask()
            Try
                Dim oState As New UserTask.roUserTaskState()
                Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString, oState)
                If oTaskExist.Message = "" Then
                    Dim oTask As New UserTask.roUserTask()
                    With oTask
                        .ID = "USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString
                        Language.ClearUserTokens()
                        Me.Language.AddUserToken(mTerminal.ID.ToString) : Me.Language.AddUserToken(mTerminal.SN.ToString) : Me.Language.AddUserToken(Me.strTerminalLocation)
                        Dim arrList As New ArrayList
                        .Message = Me.Language.Translate("TerminalNotRegistered.Title", "")
                        Me.Language.ClearUserTokens()
                        .DateCreated = Now
                        .TaskType = UserTask.TaskType.UserTaskRepair
                        .ResolverURL = "FN:\\mxC_NotRegistered"
                        .ResolverVariable1 = "TerminalID" : .ResolverValue1 = mTerminal.ID.ToString
                        .ResolverVariable2 = "Serial" : .ResolverValue2 = mTerminal.SN.ToString
                        .ResolverVariable3 = "Location" : .ResolverValue3 = mTerminal.IP
                        .Save()
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::CreateUserTask:" & mTerminal.ToString & ":User task created.")
                    End With
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::CreateUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub DelUserTask()
            Try
                Dim oState As New UserTask.roUserTaskState()
                Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\MXC_NOTREGISTERED" & mTerminal.ID.ToString, oState)
                'Si existe la tarea la borramos
                If oTaskExist.Message <> "" Then
                    oTaskExist.Delete()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::DelUserTask:" & mTerminal.ToString & ":User task deleted because the terminal registered yet")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::DelUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Overridable Sub CreateUserTaskGeneric(sTaskID As String, sLangTag As String)
            Dim oState As New UserTask.roUserTaskState()
            Dim oTaskExist As New UserTask.roUserTask(sTaskID & mTerminal.ID.ToString, oState)
            If oTaskExist.Message = "" Then
                Dim oTask As New UserTask.roUserTask()
                With oTask
                    .ID = sTaskID & mTerminal.ID.ToString
                    .Message = sLangTag + "¬" + mTerminal.ID.ToString
                    .DateCreated = Now
                    .TaskType = UserTask.TaskType.UserTaskRepair
                    .ResolverURL = ""
                    .Save()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::CreateUserTaskGeneric:" & mTerminal.ToString & ":User task created.")
                End With
            End If
        End Sub

        Public Overridable Sub DelUserTaskGeneric(sTaskID As String)
            Try
                Dim oState As New UserTask.roUserTaskState()
                Dim oTaskExist As New UserTask.roUserTask(sTaskID & mTerminal.ID.ToString, oState)
                'Si existe la tarea la borramos
                If oTaskExist.Message <> "" Then
                    oTaskExist.Delete()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicMxS::DelUserTaskGeneric:" & mTerminal.ToString & ":User task deleted because the terminal registered yet")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::DelUserTask:Terminal " + mTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        Public Sub DeleteTimeZoneFile()
            Dim sql As String = $"@DELETE# FROM TerminalsSyncTimeZonesInbioData WHERE TerminalId = {Me.TerminalID}"
            Try
                AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicMxS::DeleteTimeZoneFile: Unexpected error: ", Ex)
            End Try
        End Sub

    End Class

End Namespace