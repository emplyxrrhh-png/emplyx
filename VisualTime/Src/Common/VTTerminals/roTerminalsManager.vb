Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.AccessControl
Imports System.Text.RegularExpressions
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Comms.Base
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roLog

Namespace VTTerminals

    Enum IdTypeOnPINIdentification
        IDEmployee
        UserField
    End Enum

    Public Class roTerminalsManager
        Protected oState As roTerminalsState = Nothing
        Protected mCurrentEmployee As roTerminalEmployee
        Protected mCurrentTask As roTerminalsSyncTasks
        Protected mTerminalSyncParameter As roTerminalSyncParameter
        Protected mTerminalSyncEmployeeSyncData As roTerminalEmployeeSyncData
        Protected mTerminalCardSyncData As roTerminalCardSyncData
        Protected mTerminalFingerSyncData As roTerminalFingerSyncData
        Protected mTerminalCauseSyncData As roTerminalCauseSyncData
        Protected mTerminalEmployeePhotoSyncData As roTerminalEmployeePhotoSyncData
        Protected mTerminalDocumentSyncData As roTerminalDocumentSyncData
        Protected mTerminalAccessAuthorizationSyncData As roTerminalAccessAuthorizationSyncData
        Protected mTerminalTimeZoneSyncData As roTerminalTimezoneSyncData
        Protected mTerminalSirensSyncData As roTerminalSirensSyncData
        Protected iTasksInMessage As Integer = 0
        Protected mTerminal As Terminal.roTerminal
        Protected bBackgroundLoaded As Boolean

        Public ReadOnly Property State As roTerminalsState
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roTerminalsState)
            Me.oState = _State
            Me.bBackgroundLoaded = False
        End Sub

        Public Sub New(ByVal _State As roTerminalsState, oTerminal As Terminal.roTerminal)
            Me.oState = _State
            Me.mTerminal = oTerminal
            Me.bBackgroundLoaded = False
        End Sub

        Public Function Identify(sCredential As String, sPWD As String, Method As AuthenticationMethod, ByRef oState As roTerminalsState) As Integer
            Dim identifiedEmployeeId As Integer = -1
            Try
                Dim oEmployeeState = New Employee.roEmployeeState()

                oState.Result = roTerminalsState.ResultEnum.NoError

                Select Case Method
                    Case AuthenticationMethod.Biometry
                        identifiedEmployeeId = roTypes.Any2Integer(sCredential)
                    Case AuthenticationMethod.Card
                        ' Descarto lecturas de tarejetas con menos de 3 bytes significativos
                        If roTypes.Any2Integer(sCredential) < 65536 Then
                            oState.Result = roTerminalsState.ResultEnum.CardValueTooShort
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::Identify: Credential " & sCredential & " .Card too short. Stop checking!.")
                        Else
                            ' El terminal nos envía código real de tarjeta. Pero en BBDD puede estar almacenado con codificación. Trato este último caso antas de recuperar el empleado.
                            identifiedEmployeeId = roTerminalEmployee.GetIDEmployeeFromCardCredential(sCredential)
                            If identifiedEmployeeId = -1 Then
                                oState.Result = roTerminalsState.ResultEnum.EmployeeNotIdentified
                            End If
                        End If
                    Case AuthenticationMethod.Pin
                        ' Obtenemos PIN para su validación
                        Dim pinIdentificationIdType As IdTypeOnPINIdentification = IdTypeOnPINIdentification.IDEmployee
                        Dim identifiedEmployeeByUserfield As Integer = -1
                        If Me.mTerminal.Type.ToUpper = "TIME GATE" Then
                            ' Si estamos identificándonos en Time Gate ...
                            Dim timegateConf As TimegateConfiguration
                            Dim advancedParameter = New AdvancedParameter.roAdvancedParameter("Timegate.Identification.CustomUserFieldId", New AdvancedParameter.roAdvancedParameterState())
                            If advancedParameter.Value.Trim <> String.Empty Then
                                ' ... y está definido el parámetro de identificador por campo de la ficha
                                timegateConf = roJSONHelper.DeserializeNewtonSoft(advancedParameter.Value.Trim, GetType(TimegateConfiguration))
                                If timegateConf IsNot Nothing AndAlso timegateConf.CustomUserFieldEnabled Then
                                    ' ... y está habilitado campo de la ficha como identificador, tratamos de recuperar el identificador de empleado asociado.
                                    pinIdentificationIdType = IdTypeOnPINIdentification.UserField
                                    ' No aceptamos identificadores por defecto (cadena vacía y 0)
                                    If sCredential.Trim.Length > 0 AndAlso sCredential.Trim <> "0" Then
                                        Dim employeeTable As DataTable = roEmployeeUserField.GetIDEmployeesFromUserFieldIdAndValue(timegateConf.UserFieldId, sCredential.Trim, Now.Date, New roUserFieldState(oState.IDPassport))
                                        If employeeTable IsNot Nothing AndAlso employeeTable.Rows.Count > 0 Then identifiedEmployeeByUserfield = employeeTable.Rows(0).Item("idemployee")
                                    End If
                                End If
                            End If
                        End If

                        If (pinIdentificationIdType = IdTypeOnPINIdentification.IDEmployee AndAlso roTypes.Any2Integer(sCredential) > 0) OrElse (pinIdentificationIdType = IdTypeOnPINIdentification.UserField AndAlso identifiedEmployeeByUserfield > 0) Then
                            Dim tb As DataTable = VTBusiness.Common.roBusinessSupport.GetEmployeesByIDForPIN(If(pinIdentificationIdType = IdTypeOnPINIdentification.IDEmployee, roTypes.Any2Integer(sCredential), identifiedEmployeeByUserfield), "", "", oEmployeeState)
                            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                                Dim row As DataRow = tb.Rows(0)
                                If Not IsDBNull(row("PinNumber")) Then
                                    If row("PinNumber").ToString = sPWD AndAlso roTypes.Any2Boolean(row("Enabled")) Then
                                        Select Case pinIdentificationIdType
                                            Case IdTypeOnPINIdentification.IDEmployee
                                                identifiedEmployeeId = roTypes.Any2Integer(sCredential)
                                            Case IdTypeOnPINIdentification.UserField
                                                identifiedEmployeeId = identifiedEmployeeByUserfield
                                        End Select
                                    Else
                                        identifiedEmployeeId = -1
                                        oState.Result = roTerminalsState.ResultEnum.InvalidPassword
                                    End If
                                End If
                            End If
                        End If
                    Case AuthenticationMethod.NFC
                        Dim tb As DataTable = VTBusiness.Common.roBusinessSupport.GetIDEmployeeByNFC(sCredential, "", "", oEmployeeState)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            Dim oRowCard As DataRow = tb.Rows(0)
                            If Not IsDBNull(oRowCard("NFCcode")) Then
                                identifiedEmployeeId = roTypes.Any2Integer(oRowCard("IDEmployee"))
                            Else
                                oState.Result = roTerminalsState.ResultEnum.EmployeeNotIdentified
                            End If
                        Else
                            oState.Result = roTerminalsState.ResultEnum.EmployeeNotIdentified
                        End If
                    Case AuthenticationMethod.Plate
                        oState.Result = roTerminalsState.ResultEnum.NonSupportedPunchMethod
                    Case AuthenticationMethod.Password
                        oState.Result = roTerminalsState.ResultEnum.NonSupportedPunchMethod
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::Identify")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Identify:DBException identifying punch from terminal. Credential " & sCredential, ex)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::Identify")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Identify:Exception identifying punch from terminal. Credential " & sCredential, ex)
            End Try
            Return identifiedEmployeeId
        End Function

        Public Function Init(oConfig() As roTerminalSyncParameter, Optional strBackgroundFileName As String = "") As Boolean
            Try
                ' Incorporo información proporcionada por el terminal
                For Each oConfigItem As roTerminalSyncParameter In oConfig
                    Select Case oConfigItem.Name
                        Case "Version"
                            mTerminal.UpdateFirmVersion(oConfigItem.Value)
                        Case "DataVersion"
                            ' Si no hay datos en el terminal (DataVersion = 1-1-1900) provoco una programación de cero.
                            If roTypes.Any2DateTime(oConfigItem.Value) = roTerminalsHelper.NULLDATE Then
                                If Not roTerminalsHelper.ForceFullTerminalSync(mTerminal.ID, oState) Then
                                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::Init:Error forcing terminal full sync")
                                End If
                            End If
                        Case "TerminalBackground"
                            If strBackgroundFileName <> String.Empty AndAlso File.Exists(strBackgroundFileName) Then
                                Dim fileStream As New FileStream(strBackgroundFileName, FileMode.Open, FileAccess.Read)
                                Dim ImageData As Byte()

                                ImageData = New Byte(fileStream.Length - 1) {}
                                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                                fileStream.Close()

                                If VTBase.CryptographyHelper.EncryptWithMD5(Convert.ToBase64String(ImageData)) = oConfigItem.Value Then
                                    Me.bBackgroundLoaded = True
                                End If
                            End If
                        Case "Model"
                            mTerminal.UpdateModel(oConfigItem.Value.ToUpper)
                        Case "LocalIP"
                            mTerminal.UpdateLocation(oConfigItem.Value)
                    End Select
                Next

                ' Guardo toda la configuración a cascoporro
                mTerminal.UpdateOther(String.Join(";", oConfig.Select(Function(x) x.Name & ":" & x.Value).ToArray))
                mTerminal.UpdateStatus(True, True)
                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::Init:Terminal initialized")
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::Init:Error initializing terminal")
                Return False
            End Try
            Return True
        End Function

        Public Function GetConfig(Optional iIDTask As Integer = -1, Optional strBackgroundFileName As String = "", Optional sTaskData As String = "") As roTerminalSyncParameter()
            Dim oRet As roTerminalSyncParameter() = {}
            Dim iCurrentElements As Integer = 0

            ' Refresco información del terminal
            mTerminal.Load()

            ' 0.- Modo de fichaje
            Dim sReaderMode As String = ""
            sReaderMode = mTerminal.ReaderByID(1).Mode

            If Not mTerminal.ReaderByID(1) Is Nothing AndAlso (mTerminal.ReaderByID(1).InteractionAction = InteractionAction.E OrElse mTerminal.ReaderByID(1).InteractionAction = InteractionAction.S OrElse mTerminal.ReaderByID(1).InteractionAction = InteractionAction.X) Then
                sReaderMode = sReaderMode.Replace("TA", "TA" & mTerminal.ReaderByID(1).InteractionAction.ToString)
                sReaderMode = sReaderMode.Replace("ACCTAX", "ACCTA")
            Else
                ' Accesos integrados con presencia, más presencia en el sentido contrario al de accesos
                If mTerminal.Type.ToUpper = DTOs.TerminalModel.mx9.ToString.ToUpper AndAlso sReaderMode.StartsWith("ACCTA") AndAlso mTerminal.ReaderByID(1).InteractionAction = InteractionAction.ES Then
                    'Calculo el sentido adicional
                    If Zone.roZone.GetIsWorkingZone(mTerminal.ReaderByID(1).IDZone, Nothing) Then
                        sReaderMode = sReaderMode.Replace("ACCTA", "ACCTAS")
                    Else
                        sReaderMode = sReaderMode.Replace("ACCTA", "ACCTAE")
                    End If
                End If
            End If

            ' 1 .- Comportamiento
            iCurrentElements += 1
            Array.Resize(oRet, iCurrentElements)
            oRet(iCurrentElements - 1) = New roTerminalSyncParameter("Mode", sReaderMode)
            oRet(iCurrentElements - 1).IDTask = iIDTask

            ' 2 .- Modo de validación
            ' 2.1.- Comprobamos si hay control de antipasspack
            If mTerminal.ReaderByID(1).ValidationMode <> ValidationMode.ServerLocal AndAlso mTerminal.ReaderByID(1).CheckAPB Then
                mTerminal.ReaderByID(1).ValidationMode = ValidationMode.LocalServerPesimistic
            End If

            Dim sValidationMode As String = String.Empty
            Select Case mTerminal.ReaderByID(1).ValidationMode
                Case ValidationMode.Local
                    sValidationMode = "L"
                Case ValidationMode.LocalServer
                    sValidationMode = "LS"
                Case ValidationMode.LocalServerPesimistic
                    sValidationMode = "LSP"
                Case ValidationMode.Server
                    sValidationMode = "S"
                Case ValidationMode.ServerLocal
                    sValidationMode = "SL"
                Case Else
                    sValidationMode = "L"
            End Select
            iCurrentElements += 1
            Array.Resize(oRet, iCurrentElements)
            oRet(iCurrentElements - 1) = New roTerminalSyncParameter("ValidationMode", sValidationMode)
            oRet(iCurrentElements - 1).IDTask = iIDTask

            ' 3.- Tiempo de apertura de relé
            iCurrentElements += 1
            Array.Resize(oRet, iCurrentElements)
            oRet(iCurrentElements - 1) = New roTerminalSyncParameter("Relay1OnTime", IIf(mTerminal.ReaderByID(1).Output.HasValue AndAlso mTerminal.ReaderByID(1).Output = 1, mTerminal.ReaderByID(1).Duration, 0))
            oRet(iCurrentElements - 1).IDTask = iIDTask

            ' 4.- Background imagen de fondo del terminal
            If Not Me.bBackgroundLoaded AndAlso strBackgroundFileName <> String.Empty AndAlso File.Exists(strBackgroundFileName) Then
                Dim fileStream As New FileStream(strBackgroundFileName, FileMode.Open, FileAccess.Read)
                Dim ImageData As Byte()

                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                iCurrentElements += 1
                Array.Resize(oRet, iCurrentElements)
                oRet(iCurrentElements - 1) = New roTerminalSyncParameter("TerminalBackground", Convert.ToBase64String(ImageData))
                oRet(iCurrentElements - 1).IDTask = iIDTask
            End If

            ' 5.- Foto al fichar
            iCurrentElements += 1
            Array.Resize(oRet, iCurrentElements)
            Dim bTakeFoto As Boolean = False
            Dim oConfData As New roCollection(roTypes.Any2String(mTerminal.ConfData))
            bTakeFoto = roTypes.Any2Boolean(oConfData.Item("CaptureImage"))
            oRet(iCurrentElements - 1) = New roTerminalSyncParameter("TakePunchPhoto", bTakeFoto.ToString.ToLower)
            oRet(iCurrentElements - 1).IDTask = iIDTask

            '6.- Comportamiento Offline de Comedor
            iCurrentElements += 1
            Array.Resize(oRet, iCurrentElements)
            Dim oAdvParam = New AdvancedParameter.roAdvancedParameter("AllowDiningTurnSelection", New AdvancedParameter.roAdvancedParameterState())
            oRet(iCurrentElements - 1) = New roTerminalSyncParameter("AllowDiningTurnSelection", roTypes.Any2Boolean(oAdvParam.Value.ToLower.Trim).ToString.ToLower)

            ' Carga de parámetros dinámicos (parámetros que no están definidos en VisualTime
            ' Sólo traspaso parámetros con nombre mx9DB:
            Dim ds As New LocalDataSet
            Dim dsLocalData As New LocalDataSet
            Dim oTbl As LocalDataSet.TerminalConfigDataTable = dsLocalData.TerminalConfig
            Dim oRow As LocalDataSet.TerminalConfigRow
            Dim res As String = ""
            Dim sParamName As String
            Dim sParamValue As String

            Try

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        sParamName = oRow.Name
                        sParamValue = oRow.Value
                        If sParamName.StartsWith("mx9DB:") Then
                            sParamName = sParamName.Replace("mx9DB:", "")
                            oRet(iCurrentElements - 1) = New roTerminalSyncParameter(sParamName, sParamValue)
                            oRet(iCurrentElements - 1).IDTask = iIDTask
                        End If
                    Next
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetConfig:Exception processing advanced parameters for terminal", ex)
            End Try

            Return oRet
        End Function

        Public Function GetTask() As roTerminalSyncData
            Dim lret As New roTerminalSyncData
            Dim iTasksInMessage As Integer
            Dim bAbortSync As Boolean = False
            Dim iIDTerminal As Integer

            Try
                iIDTerminal = mTerminal.ID

                lret.Type = TerminalDataType.None
                lret.Action = TerminalDataAction.Add
                lret.Cards = {}
                lret.Causes = {}
                lret.ConfigParameters = {}
                lret.Documents = {}
                lret.Employees = {}
                lret.Fingers = {}
                lret.Photos = {}
                lret.Sirens = {}
                lret.AccessAuthorizations = {}
                lret.TimeZones = {}

                Dim oCurrentTask As roTerminalsSyncTasks
                oCurrentTask = New roTerminalsSyncTasks(iIDTerminal)
                Dim eLastTaskType As New roTerminalsSyncTasks.SyncActions
                Dim bExit As Boolean = False
                Dim bCallBroadcaster As Boolean = False

                oCurrentTask.LoadNext()
                If oCurrentTask.Task <> roTerminalsSyncTasks.SyncActions.none Then
                    mTerminal.UpdateLastUpdate(True)
                    ' Si hay alguna tarea atascada, lo advierto ahora
                    If oCurrentTask.Retries > 10 Then
                        roTerminalsHelper.CreateUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED", "TerminalSyncStopped.Title", mTerminal.ID, oState)
                    Else
                        roTerminalsHelper.DelUserTaskGeneric("USERTASK:\\TERMINAL_SYNC_STOPPED", mTerminal.ID, oState)
                    End If
                    eLastTaskType = oCurrentTask.Task
                    iTasksInMessage = 0

                    While (Not bExit) AndAlso eLastTaskType = oCurrentTask.Task
                        ' Traducimos tarea a comandos según el protocolo de la centralita
                        If LoadTaskData(lret, oCurrentTask, bExit, bCallBroadcaster) Then
                            oCurrentTask.WorkingEx()
                        Else
                            'Algo va mal. Reinicio la sincronización del terminal
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:Error retrieving data for task id " & oCurrentTask.ID.ToString)
                            oCurrentTask.WillBeRetried()
                            oCurrentTask.ResetAllWithDelay(10)
                            bAbortSync = True
                            Exit While
                        End If
                        If Not bExit Then oCurrentTask.LoadNext()
                    End While
                    If bAbortSync Then
                        lret = New roTerminalSyncData
                        bAbortSync = False
                    Else
                        Try
                            If lret.Type <> TerminalDataType.None Then
                                For Each sLogLine As String In DebugText(lret)
                                    Select Case lret.Action
                                        Case TerminalDataAction.Add
                                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:Adding data to terminal: Detail: " & sLogLine)
                                        Case TerminalDataAction.Delete
                                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:Deleting data from terminal: Detail: " & sLogLine)
                                    End Select
                                Next
                            End If
                        Catch ex As Exception
                        End Try
                        If bCallBroadcaster Then
                            roTerminalsHelper.CallBroadcaster(oState, iIDTerminal)
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:Calling broadcaster for terminal")
                        End If
                    End If
                Else
                    ' Reseteo posibles tareas que hayan quedado marcadas como enviadas y en las que haya habido un error ...
                    ' roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:No pending tasks for terminal")
                    oCurrentTask.ResetAll()
                    ' El terminal está conectado
                    mTerminal.UpdateStatus(True, True)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTask")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:DbException: ", ex)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTask")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::GetTask:Exception: ", ex)
            End Try
            Return lret
        End Function

        Public Function SetTasks(iIDTerminal As Integer, oTasks As roTerminalSyncData) As TerminalStdResponse
            Dim lret As New TerminalStdResponse
            Dim bTaskResult As Boolean = True
            Dim oTaskResult As roTasksResult = Nothing
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                lret.Result = True

                Select Case oTasks.Type
                    Case TerminalDataType.Fingers
                        For Each oFingerData As roTerminalFingerSyncData In oTasks.Fingers
                            Select Case oTasks.Action
                                Case TerminalDataAction.Add
                                    bTaskResult = AddEmployeeFinger(oFingerData)
                                Case TerminalDataAction.Delete
                                    bTaskResult = DelEmployeeFinger(oFingerData)
                            End Select

                            ' Añado detalle del resulatado de la huella para que el terminal pueda borrarla si procede
                            oTaskResult = New roTasksResult
                            oTaskResult.IDTask = oFingerData.IDEmployee * 10 + oFingerData.IDFinger
                            oTaskResult.Code = IIf(bTaskResult, 0, 1)
                            Array.Resize(lret.TaskResult, lret.TaskResult.Length + 1)
                            ' Si hubo algún error en alguna tarea, marco la respuesta global como errónea
                            If lret.Result AndAlso Not bTaskResult Then lret.Result = False
                            lret.TaskResult(lret.TaskResult.Length - 1) = oTaskResult
                        Next
                    Case TerminalDataType.Cards
                        'TODO: Cuando en el terminal se puedan registrar tarjetas ...
                        lret.Result = True
                    Case TerminalDataType.Employees
                        'TODO: Cuando en el terminal se puedan registrar PIN ...
                        lret.Result = True
                    Case TerminalDataType.EmployeePhotos
                        'TODO: Cuando en el terminal se puedan registrar fotos ...
                        lret.Result = True
                    Case Else
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::SetTasks")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SetTasks:DbException: ", ex)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::SetTasks")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SetTasks:Exception: ", ex)
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, True)
            End Try

            Return lret
        End Function

        Public Function ProcessTasksResult(iIDTerminal As Integer, oTasksResults As roTasksResult()) As TerminalStdResponse
            Dim lret As New TerminalStdResponse
            Try

                Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(iIDTerminal)
                For Each oTaskResult As roTasksResult In oTasksResults
                    If oTaskResult.IDTask > 0 Then
                        oTask.Load(oTaskResult.IDTask)
                        If oTaskResult.Result Then
                            ' Fue bien. La elimino
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::ProcessCommandResult::SyncTask completed for terminal. (commandresultcode: " + oTaskResult.Code.ToString + ", taskdetail: " & oTask.ToString & ")")
                            oTask.DoneEx(oTaskResult.IDTask)
                        Else
                            ' Algo falló en el terminal al procesar la tarea
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::ProcessCommandResult::SyncTask failed (commandresultcode: " + oTaskResult.Code.ToString + ", taskdetail: " & oTask.ToString & " ). Reset sync process for terminal")
                            oTask.ResetAllWithDelay(10)
                        End If
                    End If
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessTasksResult")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessTasksResult:DbException: ", ex)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessTasksResult")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessTasksResult:Exception: ", ex)
            End Try
            Return lret
        End Function

        Public Function SavePunch(oTerminalPunch As roTerminalPunch) As Boolean
            Dim lret As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim bUnknownPunchType As Boolean = False

            Try
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                Dim oPunch As Punch.roPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, -1, oPunchState)
                Dim oExistingPunch As New DataTable

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                oState.Result = roTerminalsState.ResultEnum.NoError

                oPunch.IDTerminal = mTerminal.ID
                oPunch.IDReader = 1
                oPunch.IDZone = mTerminal.ReaderByID(1).IDZone
                oPunch.DateTime = oTerminalPunch.PunchDateTime
                oPunch.TypeData = 0
                oPunch.Capture = Nothing

                If oTerminalPunch.TimeZoneName <> String.Empty Then
                    oPunch.TimeZone = oTerminalPunch.TimeZoneName
                End If

                If mTerminal.Type = "Time Gate" Then
                    If oTerminalPunch.Method = CInt(AuthenticationMethod.Pin) Then
                        oPunch.VerificationType = VerificationType.ID
                    Else
                        oPunch.VerificationType = VerificationType.NFC
                    End If
                End If


                If oTerminalPunch.Photo IsNot Nothing AndAlso oTerminalPunch.Photo.Length > 0 Then
                    Try
                        Dim ms As IO.MemoryStream
                        ms = New IO.MemoryStream(Convert.FromBase64String(oTerminalPunch.Photo))
                        Dim bmp As Drawing.Bitmap = New Drawing.Bitmap(ms)
                        oPunch.Capture = bmp
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Unexpected error recovering punch photo por punch: (IDEmployee: " + oPunch.IDEmployee.ToString + ", DateTime: " + oPunch.DateTime.ToString + "," & oTerminalPunch.Photo & ")")
                    End Try
                End If

                ' Si el terminal no reconoció al empleado, y se fichó con una tarjeata no asignada, y no se trataba de fichaje de accesos, guardo el fichaje de la tarjeta para poder asignarla despueés desde el asistente ...
                If oTerminalPunch.IDEmployee <= 0 AndAlso oTerminalPunch.Method = AuthenticationMethod.Card Then

                    Dim sCredential = Convert.ToInt64(oTerminalPunch.Credential, 16).ToString
                    oTerminalPunch.IDEmployee = Identify(sCredential, oTerminalPunch.PIN, AuthenticationMethod.Card, oState)

                    If oTerminalPunch.IDEmployee > 0 Then
                        ' Es un fichaje por ejemplo de presencia con tarjeta, en terminal sin autorización. No guardo la tarjeta porque luego aparecería en el asistente de asignación y podría acabar generando un fichaje de prsencia ...
                        ' Guardo el Idemployee a modo de log en Field4
                        If oTerminalPunch.Action = "AIC" Then
                            oPunch.IDEmployee = oTerminalPunch.IDEmployee
                        Else
                            oPunch.Field4 = oTerminalPunch.IDEmployee
                        End If
                        oPunch.InvalidType = InvalidTypeEnum.NRDR_
                    Else
                        oPunch.IDCredential = roTerminalsHelper.ConvertCardForVTDatabase(sCredential, oState)
                        oPunch.Type = PunchTypeEnum._IN
                    End If
                End If

                If oTerminalPunch.IDEmployee > 0 Then
                    Select Case oTerminalPunch.Action
                        Case "E"
                            oPunch.Type = PunchTypeEnum._IN
                            If oTerminalPunch.PunchData.AttendanceData IsNot Nothing Then oPunch.TypeData = oTerminalPunch.PunchData.AttendanceData.IdCause
                        Case "S"
                            oPunch.Type = PunchTypeEnum._OUT
                            If oTerminalPunch.PunchData.AttendanceData IsNot Nothing Then oPunch.TypeData = oTerminalPunch.PunchData.AttendanceData.IdCause
                        Case "X", "SM"
                            oPunch.Type = PunchTypeEnum._AUTO
                            If oTerminalPunch.PunchData.AttendanceData IsNot Nothing Then oPunch.TypeData = oTerminalPunch.PunchData.AttendanceData.IdCause
                        Case "L"
                            oPunch.Type = PunchTypeEnum._L
                            If oTerminalPunch.PunchData.AccessAndAttendanceData IsNot Nothing Then oPunch.TypeData = oTerminalPunch.PunchData.AccessAndAttendanceData.IdCause
                        Case "AIC", "AIR"
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NRDR_
                        Case "AIT"
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NTIME_
                        Case "AIS"
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NSRV
                        Case "AID"
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NOHP_
                        Case "AIAPB"
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NAPB
                        Case "AV"
                            oPunch.Type = PunchTypeEnum._AV
                        Case "D"
                            oPunch.Type = PunchTypeEnum._DR
                            If oTerminalPunch.PunchData.DinnerData IsNot Nothing AndAlso oTerminalPunch.PunchData.DinnerData.IdTurn <> -1 Then
                                oPunch.TypeData = oTerminalPunch.PunchData.DinnerData.IdTurn
                                oPunch.InvalidType = Nothing
                                If oTerminalPunch.PunchData.DinnerData.Result <> DinnerPunchResultType.TurnValid Then
                                    Select Case oTerminalPunch.PunchData.DinnerData.Result
                                        Case DinnerPunchResultType.NoTurn
                                            oPunch.InvalidType = InvalidTypeEnum.NTIME_
                                        Case DinnerPunchResultType.TurnWithPunch
                                            oPunch.InvalidType = InvalidTypeEnum.NRPT_
                                        Case Else
                                            oPunch.InvalidType = InvalidTypeEnum.NERR_
                                    End Select

                                Else
                                    ' Si es necesario, imprimo el tiquet de comedor
                                    Try
                                        Dim sPrintOnFilePath As String = String.Empty
                                        Dim sCustomDesign As String = String.Empty
                                        Dim hasCustomDesign As Boolean = False
                                        mTerminal.AdvancedParameters.TryGetValue("DinnerPrintOnFile", sPrintOnFilePath)
                                        mTerminal.AdvancedParameters.TryGetValue("DinnerPrintCustomDesign", sCustomDesign)
                                        Dim reader = mTerminal.ReaderByID(1)
                                        If reader IsNot Nothing AndAlso reader.InteractiveConfig IsNot Nothing Then
                                            Dim printerName As String = reader.InteractiveConfig.PrinterName

                                            If Not String.IsNullOrEmpty(printerName) Then
                                                Dim folder As String = "diner"

                                                Dim oCustomFolderName As String = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "DiningTicketFolder"))
                                                If Not String.IsNullOrEmpty(oCustomFolderName) Then folder = oCustomFolderName.Trim.ToLower

                                                If printerName <> "cloud" Then
                                                    folder = $"{folder}/{printerName}"
                                                End If

                                                folder = folder.Trim.ToLower()

                                                hasCustomDesign = (sCustomDesign IsNot Nothing AndAlso sCustomDesign.ToUpper = "TRUE")
                                                roTerminalsHelper.PrintTicketOnFileOnAzure(oState, folder, hasCustomDesign, oTerminalPunch, reader.InteractiveConfig.PrinterText)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" + mTerminal.ToString + "::SavePunch:Error:Error printing ticket.", ex)
                                    End Try
                                End If
                            Else
                                ' El terminal no recibió la respuesta del servidor
                                ' Calculo ahora ...
                                oPunch.TypeDetails = "Offline punch"
                                Dim oIP As New roTerminalInteractivePunch
                                oIP.Punch = oTerminalPunch
                                If Not CheckDinnerPunch(oIP) Then
                                    oState.Result = roTerminalsState.ResultEnum.ErrorCheckingDinnerTurn
                                Else
                                    oPunch.TypeData = oIP.Punch.PunchData.DinnerData.IdTurn
                                    Select Case oIP.Punch.PunchData.DinnerData.Result
                                        Case DinnerPunchResultType.TurnValid
                                            oPunch.InvalidType = InvalidTypeEnum.NDEF_
                                        Case DinnerPunchResultType.NoTurn
                                            oPunch.InvalidType = InvalidTypeEnum.NTIME_
                                        Case DinnerPunchResultType.TurnWithPunch
                                            oPunch.InvalidType = InvalidTypeEnum.NRPT_
                                        Case Else
                                            oPunch.InvalidType = InvalidTypeEnum.NERR_
                                    End Select
                                End If
                            End If

                            ' Si hay que guardar una salida, lo hago ahora
                            If oTerminalPunch.PunchData.DinnerData.SaveAttOut Then
                                If oPunch.InvalidType Is Nothing OrElse oPunch.InvalidType = InvalidTypeEnum.NDEF_ Then
                                    'Guardamos una salida de presencia si toca
                                    Dim oAttPunchState As Punch.roPunchState = New Punch.roPunchState
                                    Dim mAttExitPunch As Punch.roPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, -1, oAttPunchState)
                                    mAttExitPunch.IDTerminal = oPunch.IDTerminal
                                    mAttExitPunch.IDReader = 1
                                    mAttExitPunch.IDZone = oPunch.IDZone
                                    mAttExitPunch.DateTime = oTerminalPunch.PunchDateTime.AddSeconds(-1)
                                    mAttExitPunch.Capture = oPunch.Capture
                                    mAttExitPunch.Type = PunchTypeEnum._OUT
                                    mAttExitPunch.ActualType = PunchTypeEnum._OUT
                                    If mAttExitPunch.Save() Then
                                        roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch:Attendance out punch saved on dinner in (IDEmployee: " + oPunch.IDEmployee.ToString + ", DateTime: " + oPunch.DateTime.ToString + ")")
                                    Else
                                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch:Error saving att punch out for dinner punch in")
                                    End If
                                End If
                            End If
                        Case "C"
                            oPunch.Type = PunchTypeEnum._CENTER
                            If Not oTerminalPunch.PunchData.CostCenterData Is Nothing Then oPunch.TypeData = oTerminalPunch.PunchData.CostCenterData.IdCostCenter
                        Case "T"
                            ' ProductiV
                            If Not SaveProductivPunch(oTerminalPunch) Then
                                oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                                lret = False
                            Else
                                lret = True
                            End If
                        Case Else
                            bUnknownPunchType = True
                            roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch: Saving punches of type " & oTerminalPunch.Action & " COMMING SOON !!!!!!!!!!!!!!!!!!!!!!!!!!")
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch: Unexpected type punch " & oTerminalPunch.Action & ". Saving anyway...")
                    End Select

                    'No se guarda un fichaje si ya existe otro con la misma hora:min:seg para el mismo empleado desde el mismo terminal y del mismo tipo
                    Dim strFilter As String = ""
                    strFilter = "IDEmployee = " + oPunch.IDEmployee.ToString + " AND DateTime = " + roTypes.Any2Time(oPunch.DateTime).SQLDateTime + " AND IDTerminal = " + oPunch.IDTerminal.ToString
                    oExistingPunch = Robotics.Base.VTBusiness.Punch.roPunch.GetPunches(strFilter, oPunchState)
                End If

                If oTerminalPunch.Action <> "T" AndAlso Not bUnknownPunchType Then
                    If oExistingPunch.Rows.Count = 0 Then
                        If oPunch.Save() Then
                            'Guardamos el ActualType despues del Save por si fuese automatico
                            oTerminalPunch.ActualType = oPunch.ActualType
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch::Punch saved: " & roTerminalsHelper.TerminalPunchToString(oTerminalPunch, mTerminal.ID))
                            lret = True
                        Else
                            oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch::Error saving punch: " & roTerminalsHelper.TerminalPunchToString(oTerminalPunch, mTerminal.ID))
                            lret = False
                        End If
                    Else
                        lret = True
                        roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch::Punch duplicated. It will not be saved: " & roTerminalsHelper.TerminalPunchToString(oTerminalPunch, mTerminal.ID))
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch:DbException: ", ex)
                oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                lret = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::SavePunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch
                lret = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, lret)
            End Try

            Return lret
        End Function

        Private Function SaveProductivPunch(oTerminalPunch As roTerminalPunch) As Boolean
            Try
                Dim bSave As Boolean = False
                Dim oCurrentTask As roTerminalProductivTask = Nothing
                Dim oNextTask As roTerminalProductivTask = Nothing
                Dim oCurrentTaskBusinessPunch As Punch.roPunch = Nothing
                Dim oNextBusinessPunch As Punch.roPunch = Nothing
                Dim oCurrentBusinessTask As Task.roTask = Nothing
                Dim oNextBusinessTask As Task.roTask = Nothing

                oCurrentTask = oTerminalPunch.PunchData.ProductivData.CurrentTask
                oNextTask = oTerminalPunch.PunchData.ProductivData.NextTask

                ' Tratamiento de la tarea que estaba realizando ...
                If Not oCurrentTask Is Nothing AndAlso oCurrentTask.Id > -1 Then
                    oCurrentBusinessTask = New Task.roTask(oCurrentTask.Id, New Task.roTaskState(oState.IDPassport))
                End If
                If Not oNextTask Is Nothing AndAlso oNextTask.Id > -1 Then
                    oNextBusinessTask = New Task.roTask(oNextTask.Id, New Task.roTaskState(oState.IDPassport))
                End If

                Select Case oTerminalPunch.PunchData.ProductivData.PunchAction
                    Case ActionTypes.tNone
                    Case ActionTypes.aBegin
                    Case ActionTypes.tChange
                        ' Si tenía que informar campos de la ficha, los guardo ahora
                        ' Los campos de tipo "Al cambiar" se guardan en el fichaje
                        If Not oCurrentTask.RequiredUserFields Is Nothing AndAlso oCurrentTask.RequiredUserFields.ToList.FindAll(Function(x) x.OnAction = ActionTypes.tChange).Count > 0 Then
                            Dim aFieldValues As String() = GetTaskFieldsNormalizedValuesForPunch(oCurrentTask)
                            Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                            oCurrentTaskBusinessPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, -1, oPunchState)
                            oCurrentTaskBusinessPunch.LoadNewBEGIN(oTerminalPunch.PunchDateTime, mTerminal.ID, oCurrentBusinessTask, , , , , , , , , , aFieldValues(0), aFieldValues(1), aFieldValues(2), aFieldValues(3), aFieldValues(4), aFieldValues(5))

                            If Not oCurrentTaskBusinessPunch.Save() Then
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch:Exception: " + oCurrentTask.Name + " and employee " + oTerminalPunch.IDEmployee.ToString)
                            End If
                        End If
                    Case ActionTypes.tComplete
                        ' Cambio el estado de la tarea que acabo de completar
                        oCurrentBusinessTask.Status = TaskStatusEnum._PENDING
                        oCurrentBusinessTask.EndDate = DateTime.Now
                        oCurrentBusinessTask.UpdateStatusDate = DateTime.Now
                        oCurrentBusinessTask.IDEmployeeUpdateStatus = oTerminalPunch.IDEmployee
                        If oCurrentBusinessTask.Save() Then
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch::Task " + oCurrentTask.Name + " completed by employee " + oTerminalPunch.IDEmployee.ToString + ". Approval pending!")
                            ' Guardo los valores de la ficha, si se informaron y son de tipo "Al completar"
                            Dim oState As New Task.roTaskState

                            Dim aFieldValues As String() = GetTaskFieldsNormalizedValuesForTask(oCurrentTask)

                            If Not Task.roTask.SaveTaskFieldsFromPunch(oCurrentBusinessTask.ID, aFieldValues(0), aFieldValues(1), aFieldValues(2), roTypes.Any2Double(aFieldValues(3)), roTypes.Any2Double(aFieldValues(4)), roTypes.Any2Double(aFieldValues(5)), oState) Then
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch::Error saving atributes on completion of task " + oCurrentTask.Name + " and employee " + oTerminalPunch.IDEmployee.ToString)
                            End If
                        Else
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch:Exception: " + oCurrentTask.Name + " could not be saved. Description: " + oCurrentBusinessTask.State.Result.ToString)
                        End If

                        ' Ahora si hay campos de la ficha de tipo "Al cambiar", los guardo en un nuevo fichaje para la tarea
                        If Not oCurrentTask.RequiredUserFields Is Nothing AndAlso oCurrentTask.RequiredUserFields.ToList.FindAll(Function(x) x.OnAction = ActionTypes.tChange).Count > 0 Then
                            Dim aFieldValues As String() = GetTaskFieldsNormalizedValuesForPunch(oCurrentTask)
                            Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                            oCurrentTaskBusinessPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, -1, oPunchState)

                            oCurrentTaskBusinessPunch.LoadNewBEGIN(oTerminalPunch.PunchDateTime, mTerminal.ID, oCurrentBusinessTask, , , , , , , , , , aFieldValues(0), aFieldValues(1), aFieldValues(2), roTypes.Any2Double(aFieldValues(3)), roTypes.Any2Double(aFieldValues(4)), roTypes.Any2Double(aFieldValues(5)))

                            If Not oCurrentTaskBusinessPunch.Save() Then
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch:Exception: " + oCurrentTask.Name)
                            End If
                        End If

                        ' Si se ha completado una tarea y no se ha seleccionado otra, se guarda "Sin tarea" como siguiente.
                        If oNextTask Is Nothing OrElse oNextBusinessTask Is Nothing OrElse oNextTask.Id <= 0 Then
                            oNextBusinessTask = New Task.roTask()
                            oNextBusinessTask.ID = 0
                            oNextBusinessTask.Load()
                        End If
                End Select

                ' ... Tratamiento de la tarea que voy a realizar
                If Not oNextTask Is Nothing AndAlso oNextTask.Id > -1 Then
                    'Han seleccionado una tarea nueva la iniciamos
                    Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                    oNextBusinessPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, -1, oPunchState)
                    oNextBusinessPunch.LoadNewBEGIN(oTerminalPunch.PunchDateTime, mTerminal.ID, oNextBusinessTask)
                    If oNextBusinessPunch.Save() Then
                        bSave = True
                        roLog.GetInstance().logMessage(EventType.roDebug, "TerminalProductivPunch::Save::Start working on " + oNextTask.Name)

                        ' Si estoy iniciando una tarea, guardo los campos de la ficha
                        ' Guardo los valores de la ficha, si se informaron
                        Dim oState As New Task.roTaskState

                        Dim aFieldValues As String() = GetTaskFieldsNormalizedValuesForTask(oNextTask)

                        If Not Task.roTask.SaveTaskFieldsFromPunch(oNextBusinessTask.ID, aFieldValues(0), aFieldValues(1), aFieldValues(2), roTypes.Any2Double(aFieldValues(3)), roTypes.Any2Double(aFieldValues(4)), roTypes.Any2Double(aFieldValues(5)), oState) Then
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::SaveProductivPunch::Error saving task atributes for task " + oNextTask.Name + " and employee " + oTerminalPunch.IDEmployee.ToString)
                        End If
                    Else
                        roLog.GetInstance().logMessage(EventType.roDebug, "TerminalProductivPunch::Save::Error creating starting task punch for " + oNextTask.Name)
                    End If
                End If
                Return bSave
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "TerminalProductivPunch::Save::Error:", ex)
                Return False
            End Try
        End Function

        Private Function GetTaskFieldsNormalizedValuesForPunch(oCurrentTask As roTerminalProductivTask) As String()
            Dim aFieldValues As String() = {"", "", "", "-1", "-1", "-1"}
            If Not oCurrentTask.RequiredUserFields Is Nothing Then
                aFieldValues(0) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 1) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 1).Value), 1), "")
                aFieldValues(1) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 2) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 2).Value), 2), "")
                aFieldValues(2) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 3) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 3).Value), 3), "")
                aFieldValues(3) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 4) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 4).Value), 4), "-1")
                aFieldValues(4) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 5) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 5).Value), 5), "-1")
                aFieldValues(5) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 6) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnPunch(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) x.OnAction = ActionTypes.tChange AndAlso x.Id = 6).Value), 6), "-1")
            End If

            Return aFieldValues
        End Function

        Private Function GetTaskFieldValueIfItShouldBeSavedOnPunch(ByVal sValue As String, ByVal iFieldIndex As Integer) As String
            Try
                'Sólo se guarda en el fichaje los campos de tipo "Informar al cambiar". El resto se guarda en la tarea
                If GetTaskFieldType(iFieldIndex) = ActionTypes.tChange Then
                    If sValue Is Nothing Then
                        ' No informo
                        If iFieldIndex = 1 Or iFieldIndex = 2 Or iFieldIndex = 3 Then
                            'Campos string. Valor por defecto ""
                            Return ""
                        Else
                            ' Campos numéricos. Valor por defecto -1
                            Return "-1"
                        End If
                    Else
                        Return roTypes.Any2String(sValue)
                    End If
                Else
                    'No debo informar el campo
                    If iFieldIndex = 1 Or iFieldIndex = 2 Or iFieldIndex = 3 Then
                        'Campos string. Valor por defecto ""
                        Return ""
                    Else
                        ' Campos numéricos. Valor por defecto -1
                        Return "-1"
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalManager::GetTaskFieldValueIfItShouldBeSavedOnPunch::Error:FieldIndex - " + iFieldIndex.ToString + " Value - " + sValue, ex)
                Return ""
            End Try
        End Function

        Private Function GetTaskFieldsNormalizedValuesForTask(oCurrentTask As roTerminalProductivTask) As String()
            Dim aFieldValues As String() = {"", "", "", "-1", "-1", "-1"}
            If Not oCurrentTask.RequiredUserFields Is Nothing Then
                aFieldValues(0) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 1) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 1).Value), 1), "")
                aFieldValues(1) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 2) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 2).Value), 2), "")
                aFieldValues(2) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 3) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 3).Value), 3), "")
                aFieldValues(3) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 4) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 4).Value), 4), "-1")
                aFieldValues(4) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 5) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 5).Value), 5), "-1")
                aFieldValues(5) = If(Not oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 6) Is Nothing, GetTaskFieldValueIfItShouldBeSavedOnTask(roTypes.Any2String(oCurrentTask.RequiredUserFields.ToList.FirstOrDefault(Function(x) (x.OnAction = ActionTypes.aBegin OrElse x.OnAction = ActionTypes.tComplete) AndAlso x.Id = 6).Value), 6), "-1")
            End If

            Return aFieldValues
        End Function

        Private Function GetTaskFieldValueIfItShouldBeSavedOnTask(ByVal sValue As String, ByVal iFieldIndex As Integer) As String
            Try
                'Sólo se guarda en el fichaje los campos de tipo "Informar al iniciar o Informar al completar". El resto se guarda en la tarea
                Dim oTaskType As ActionTypes
                oTaskType = GetTaskFieldType(iFieldIndex)
                If oTaskType = ActionTypes.aBegin OrElse oTaskType = ActionTypes.tComplete Then
                    'Debo informar el campo
                    If sValue Is Nothing Then
                        ' No informo
                        If iFieldIndex = 1 Or iFieldIndex = 2 Or iFieldIndex = 3 Then
                            'Campos string. Valor por defecto ""
                            Return ""
                        Else
                            ' Campos numéricos. Valor por defecto -1
                            Return "-1"
                        End If
                    Else
                        Return roTypes.Any2String(sValue)
                    End If
                Else
                    'No debo informar el campo
                    If iFieldIndex = 1 Or iFieldIndex = 2 Or iFieldIndex = 3 Then
                        'Campos string. Valor por defecto ""
                        Return ""
                    Else
                        ' Campos numéricos. Valor por defecto -1
                        Return "-1"
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalManager::GetTaskFieldValueIfItShouldBeSavedOnTask::Error:FieldIndex - " + iFieldIndex.ToString + " Value - " + sValue, ex)
                Return ""
            End Try
        End Function

        Private Function GetTaskFieldType(ByVal iIndex As Integer) As ActionTypes
            Try
                Dim oFieldDefinition As New roTaskFieldDefinition(New roTaskFieldState, iIndex)
                Return oFieldDefinition.Action
            Catch ex As Exception
                Return ActionTypes.tNone
                roLog.GetInstance().logMessage(EventType.roError, "TerminalProductivPunch::GetTaskFieldType: ", ex)
            End Try
        End Function

        Public Function DeletePunch(oTerminalPunch As roTerminalPunch) As Boolean
            Dim lret As Boolean = False
            Try
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                Dim oPunch As Punch.roPunch
                Dim oExistingPunch As New DataTable

                ' Buscamos el fichaje
                Dim iActualType As Integer = 0
                Select Case oTerminalPunch.Action
                    Case "E"
                        iActualType = 1
                    Case "S"
                        iActualType = 2
                End Select
                Dim strFilter As String = ""
                strFilter = "IDEmployee = " + oTerminalPunch.IDEmployee.ToString + " AND DateTime = " + roTypes.Any2Time(oTerminalPunch.PunchDateTime).SQLDateTime + " AND IDTerminal = " + mTerminal.ID.ToString + " AND ActualType = " & iActualType.ToString
                oExistingPunch = Robotics.Base.VTBusiness.Punch.roPunch.GetPunches(strFilter, oPunchState)

                If oExistingPunch.Rows.Count = 1 Then
                    oPunch = New Punch.roPunch(oTerminalPunch.IDEmployee, oExistingPunch.Rows(0)("ID"), oPunchState)
                    If Not oPunch.Delete Then
                        roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::DeletePunch::Unable to delete punch: " & roTerminalsHelper.TerminalPunchToString(oTerminalPunch, mTerminal.ID))
                        lret = False
                    End If
                Else
                    lret = True
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::DeletePunch::Punch to be deleted not found: " & roTerminalsHelper.TerminalPunchToString(oTerminalPunch, mTerminal.ID))
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.Type & "::DeletePunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::DeletePunch:DbException: ", ex)
                oState.Result = roTerminalsState.ResultEnum.ErrorDeletingPunch
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.Type & "::DeletePunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::DeletePunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.ErrorDeletingPunch
            End Try
            Return lret
        End Function

        Public Function SetMessagesAsRead(ByVal aMessagesId() As Integer) As Boolean
            Dim oRet As Boolean = False
            Try
                oRet = roTerminalEmployee.SetEmployeeMSGByID(aMessagesId)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::SetMessagesAsRead")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessPunch:DbException: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
                oRet = False
            End Try
            Return oRet
        End Function

        Public Function GetLastPunchEmployeeLanguage() As String
            If mCurrentEmployee IsNot Nothing Then
                Return mCurrentEmployee.Language
            Else
                Dim systemState As New roSecurityState()
                Dim langKey As String = systemState.GetLanguageKey()

                Return langKey
            End If
        End Function

        Public Function ProcessPunch(oCurrentPunch As roTerminalInteractivePunch) As roTerminalInteractivePunch
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If oCurrentPunch.Command = InteractivePunchCommand.Punch Then
                    'Recupero la información adicional a mostrar en el terminal en función de la acción seleccionada por el empleado antes de identificarse
                    LoadEmployeeStatusOnTerminal(oCurrentPunch, mTerminal, oState)
                End If

                mCurrentEmployee = New roTerminalEmployee(Me.oState)
                mCurrentEmployee.LoadEmployee(oCurrentPunch.Punch.IDEmployee)

                If oState.Result = roTerminalsState.ResultEnum.NoError Then
                    Select Case oCurrentPunch.Command
                        Case InteractivePunchCommand.Punch
                            Select Case oCurrentPunch.Punch.Action
                                Case "X", "SM"
                                    oCurrentPunch = ProcessSmartAttendancePunch(oCurrentPunch)
                                Case "E", "S"
                                    ' DO NOTHING
                                Case "L", "AV"
                                    ' Accesos permitidos por el terminal
                                    ' Modo pesimístico
                                    Dim sValidationResult As String = "AV"

                                    If mTerminal.ReaderByID(1).EmployeePermit(mCurrentEmployee.ID, True, True, sValidationResult) Then
                                        If roTerminalPunchHelper.CheckAPBIfNeeded(mTerminal, oCurrentPunch, oState) Then
                                            If mTerminal.ReaderByID(1).Mode.Contains("ACCTA") Then
                                                oCurrentPunch.Punch.Action = "L"
                                            Else
                                                oCurrentPunch.Punch.Action = "AV"
                                            End If
                                        Else
                                            ' Deniego acceso
                                            oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.AccessDeniedByServer", Nothing,, mCurrentEmployee.Language)
                                            oCurrentPunch.Punch.Action = "AIAPB"
                                        End If
                                    Else
                                        oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.AccessDeniedByServer", Nothing,, mCurrentEmployee.Language)
                                        oCurrentPunch.Punch.Action = "AIS"
                                    End If
                                Case "AIC", "AIR", "AIT"
                                    ' Accesos denegados por el terminal
                                    ' Modo optimista
                                    Dim sValidationResult As String = oCurrentPunch.Punch.Action
                                    If mTerminal.ReaderByID(1).EmployeePermit(mCurrentEmployee.ID, True, True, sValidationResult) Then
                                        If roTerminalPunchHelper.CheckAPBIfNeeded(mTerminal, oCurrentPunch, oState) Then
                                            oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.AccessAllowedByServer", Nothing,, mCurrentEmployee.Language)
                                            If mTerminal.ReaderByID(1).Mode.Contains("ACCTA") Then
                                                oCurrentPunch.Punch.Action = "L"
                                            Else
                                                oCurrentPunch.Punch.Action = "AV"
                                            End If
                                        Else
                                            ' Deniego acceso por APB
                                            oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.AccessDeniedByServer", Nothing,, mCurrentEmployee.Language)
                                            oCurrentPunch.Punch.Action = "AIAPB"
                                        End If
                                    End If
                                Case "D"
                                    ' Comedor
                                    oCurrentPunch = ProcessDinnerPunch(oCurrentPunch)
                                Case "T"
                                    ' Productiv
                                    oCurrentPunch = ProcessProductivPunch(oCurrentPunch)
                                Case "C"
                                    ' Centros de Coste
                                Case Else
                                    oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.UnknownBehaviour", Nothing,, mCurrentEmployee.Language)
                            End Select

                        Case InteractivePunchCommand.Display
                            ' Actuo según la orden dada por pantalla
                            Select Case oCurrentPunch.Display.Response.ToUpper
                                Case "DELETELASTIN"

                                Case "DELETELASTOUT"

                            End Select
                    End Select

                ElseIf oState.Result = roTerminalsState.ResultEnum.NoPermissionForThisAction AndAlso (oCurrentPunch.Punch.Action.StartsWith("AV") OrElse oCurrentPunch.Punch.Action.StartsWith("AI")) Then
                    oCurrentPunch.Display.WorkArea = roTerminalTextHelper.Translate("ProcessPunch.AccessDeniedByServer", Nothing,, mCurrentEmployee.Language)
                    oCurrentPunch.Punch.Action = "AIS"
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessPunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessPunch:DbException: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::Process")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessPunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = roTerminalsState.ResultEnum.NoError)
            End Try

            Return oCurrentPunch
        End Function

        Public Function ProcessSmartAttendancePunch(oCurrentPunch As roTerminalInteractivePunch) As roTerminalInteractivePunch
            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                'Solo miramos si es un empleado valido.
                If Not (mTerminal.ReaderByID(1).Mode = "ACC" OrElse mTerminal.ReaderByID(1).Mode = "ACCTA") And Not mTerminal.ReaderByID(1).EmployeePermit(oCurrentPunch.Punch.IDEmployee, False, False, False) Then
                    oState.Result = roTerminalsState.ResultEnum.NoPermissionForThisAction
                    Return oCurrentPunch
                End If

                ' Cargo justificaciones que se pueden fichar
                Dim lCauses As New List(Of roTerminalListItem)
                lCauses = roTerminalEmployee.GetEmployeeAllowedCausesForPunch(oCurrentPunch.Punch.IDEmployee)

                Dim olAvailableCauses As New List(Of roTerminalCause)
                For Each oItem As roTerminalListItem In lCauses
                    olAvailableCauses.Add(New roTerminalCause(oItem.Id, oItem.Text))
                Next
                oCurrentPunch.EmployeeStatus.AttendanceStatus.AvailableCauses = olAvailableCauses.ToArray
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessSmartAttendancePunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessSmartAttendancePunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
            End Try

            Return oCurrentPunch
        End Function

        Public Function ProcessDinnerPunch(oCurrentPunch As roTerminalInteractivePunch) As roTerminalInteractivePunch
            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                ' Verifico si aplica guardar salida de presencia en función de cuando se fichó la última salida.
                Dim sCourtesySeconds As String = String.Empty
                Dim sPrintOnFilePath As String = String.Empty
                Dim sCustomDesign As String = String.Empty
                Dim hasCustomDesign As Boolean = False
                oCurrentPunch.Punch.PunchData.DinnerData.SaveAttOut = False
                If mTerminal.ReaderByID(1).Mode.ToUpper.IndexOf("TA") >= 0 Then
                    mTerminal.AdvancedParameters.TryGetValue("AttOutOnDinnerIn", sCourtesySeconds)
                    If roTypes.Any2Integer(sCourtesySeconds) > 0 Then
                        oCurrentPunch.Punch.PunchData.DinnerData.SaveAttOut = True
                        If oCurrentPunch.EmployeeStatus.AttendanceStatus.AttendanceStatus = EmployeeAttStatus.Outside Then
                            ' Si lo último es una salida, sólo guardo una nueva si hace más de 60 segundos ...
                            oCurrentPunch.Punch.PunchData.DinnerData.SaveAttOut = (DateDiff("s", oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchDateTime, oCurrentPunch.Punch.PunchDateTime) > roTypes.Any2Integer(sCourtesySeconds))
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessDinnerPunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessDinnerPunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
            End Try

            Return oCurrentPunch
        End Function

        Public Function ProcessProductivPunch(oCurrentPunch As roTerminalInteractivePunch) As roTerminalInteractivePunch
            Try
                oState.Result = roTerminalsState.ResultEnum.NoError

                ' TODO: Do Something or Delete
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::ProcessProductivPunch")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::ProcessProductivPunch:Exception: ", ex)
                oState.Result = roTerminalsState.ResultEnum.Exception
            End Try

            Return oCurrentPunch
        End Function

        Public Function LoadTaskRequiredFields(ByRef oTask As roTerminalProductivTask) As Boolean
            Dim oRet As Boolean = False

            Try
                Dim fieldsList = New Generic.List(Of roTerminalProductivTaskUserField)

                ' Obtenemos los campos asignados a la tarea
                Dim taskFieldsList As Generic.List(Of roTaskField) = roTaskField.GetTaskFieldsList(oTask.Id, Nothing)

                ' Cargo la tarea
                Dim selectedTask As Task.roTask = New Task.roTask(oTask.Id, Nothing)

                Dim bExcludeFieldOnBegin As Boolean = False

                'Si la tarea ya está inciada (esto no debería pasar), ya habrán informado los campos, por tanto los omito

                bExcludeFieldOnBegin = (selectedTask.ID > 0 AndAlso selectedTask.StartDate.HasValue)

                For Each oTaskField As roTaskField In taskFieldsList
                    Dim newField As New roTerminalProductivTaskUserField
                    newField.Id = oTaskField.IDField
                    newField.OnAction = oTaskField.Action
                    newField.Name = oTaskField.FieldName
                    newField.Value = String.Empty
                    newField.ValuesList = oTaskField.ListValues.ToArray

                    If Not bExcludeFieldOnBegin OrElse oTaskField.Action <> ActionTypes.aBegin Then
                        fieldsList.Add(newField)
                    End If
                Next

                If fieldsList.Count > 0 Then
                    oTask.RequiredUserFields = fieldsList.ToArray()
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::LoadTaskRequiredFields : ", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function CheckDinnerPunch(ByRef oDinnerPunch As roTerminalInteractivePunch) As Boolean
            Dim oRet As Boolean = False
            Dim oDinnerPunchData As New roDinnerPunchData

            Try

                Dim sSQL As String = ""
                Dim dt As DataTable

                oState.Result = roTerminalsState.ResultEnum.NoDinnerTurnAvailable
                oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.NoTurn

                'Comprobamos si ahora hay turnos para fichar
                sSQL = "@SELECT# * from DiningRoomTurns "
                sSQL += " where CONVERT(VARCHAR(8)," + roTypes.Any2Time(oDinnerPunch.Punch.PunchDateTime).SQLDateTime + ",108)"
                sSQL += " between begintime And endtime"
                sSQL += " and daysOfWeek like '" + "1".PadLeft(IIf(oDinnerPunch.Punch.PunchDateTime.DayOfWeek = DayOfWeek.Sunday, 7, oDinnerPunch.Punch.PunchDateTime.DayOfWeek), "_").PadRight(7, "_") + "'"
                dt = CreateDataTable(sSQL)

                ' Miro si debo permitir más de un acceso por turno
                Dim bAllowMultipleDinnerInTurn As Boolean = False
                Dim sMultipleDinnerInTurnDefinition As String = String.Empty
                mTerminal.AdvancedParameters.TryGetValue("AllowMultipleDinnerInTurn", sMultipleDinnerInTurnDefinition)

                If sMultipleDinnerInTurnDefinition.Length > 0 AndAlso sMultipleDinnerInTurnDefinition.Split("=").Length > 0 Then
                    Dim sFieldName As String = String.Empty
                    Dim sFieldValue As String = String.Empty
                    sFieldName = sMultipleDinnerInTurnDefinition.Split("=")(0)
                    sFieldValue = sMultipleDinnerInTurnDefinition.Split("=")(1)
                    ' Miro si la ficha tiene el campo y valor informado
                    Dim oEmployeeState As New roUserFieldState
                    Dim usrField As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oDinnerPunch.Punch.IDEmployee, sFieldName, Now.Date, oEmployeeState)
                    If usrField IsNot Nothing AndAlso usrField.FieldValue IsNot Nothing Then
                        bAllowMultipleDinnerInTurn = usrField.FieldValue.ToString.ToUpper = sFieldValue.ToUpper
                    End If
                End If

                Dim sAllowDiningTurnSelection As String = String.Empty
                Dim bAllowDiningTurnSelection As Boolean = False
                mTerminal.AdvancedParameters.TryGetValue("AllowDiningTurnSelection", sAllowDiningTurnSelection)
                If sAllowDiningTurnSelection.Length > 0 Then
                    bAllowDiningTurnSelection = roTypes.Any2Boolean(sAllowDiningTurnSelection)
                End If

                Dim lAvailableTurns As New List(Of roTerminalDiningTurn)
                For Each orow As DataRow In dt.Rows
                    'Miramos los turnos
                    Dim oColection As roCollection
                    Dim iIDTurn As Integer = 0
                    Dim sTurnName As String = String.Empty
                    oColection = New roCollection(roTypes.Any2String(orow.Item("EmployeeSelection")))

                    iIDTurn = roTypes.Any2Integer(orow.Item("ID"))
                    sTurnName = roTypes.Any2String(orow.Item("Name"))

                    oDinnerPunchData.IdTurn = iIDTurn
                    oDinnerPunch.EmployeeStatus.DinnerStatus.IdTurn = iIDTurn
                    oDinnerPunch.EmployeeStatus.DinnerStatus.TurnName = sTurnName

                    If roTypes.Any2String(oColection.Item("prmUserFields", roCollection.roSearchMode.roByKey)).Length > 0 Then
                        Dim FieldName As String = ""
                        Dim FieldValue As String = ""

                        FieldName = roTypes.Any2String(oColection.Item("prmUserFields", roCollection.roSearchMode.roByKey)).Split("~")(0)
                        FieldValue = roTypes.Any2String(oColection.Item("prmUserFields", roCollection.roSearchMode.roByKey)).Split("~")(1)

                        Dim oEmployeeState As New roUserFieldState
                        Dim usrField As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oDinnerPunch.Punch.IDEmployee, FieldName, oDinnerPunch.Punch.PunchDateTime.Date, oEmployeeState)
                        If usrField IsNot Nothing AndAlso usrField.FieldValue IsNot Nothing Then
                            'Si el campo es valido
                            If usrField.FieldValue.ToString.ToUpper = FieldValue.ToUpper Then
                                'Buscamos si ya tiene un acceso al turno
                                If bAllowMultipleDinnerInTurn OrElse (Not ExistDinnerPunchInTurn(oDinnerPunch.Punch, roTypes.Any2Time(orow.Item("BeginTime")).Value, roTypes.Any2Time(orow.Item("EndTime")).Value, iIDTurn)) Then
                                    'Si no encontramos acceso es que podemos fichar
                                    oState.Result = roTerminalsState.ResultEnum.DinnerTurnValid
                                    oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.TurnValid
                                    lAvailableTurns.Add(New roTerminalDiningTurn With {.Id = iIDTurn, .Name = sTurnName})
                                    If Not bAllowDiningTurnSelection Then Exit For
                                Else
                                    If lAvailableTurns.Count = 0 OrElse Not bAllowDiningTurnSelection Then
                                        oState.Result = roTerminalsState.ResultEnum.OnlyOneDinnerPerTurn
                                        oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.TurnWithPunch
                                    End If
                                End If
                            End If

                            oColection = Nothing
                            usrField = Nothing
                        Else
                            If lAvailableTurns.Count = 0 OrElse Not bAllowDiningTurnSelection Then
                                oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.NoTurn
                            End If
                        End If
                    Else
                        'Todos los empleados
                        If bAllowMultipleDinnerInTurn OrElse (Not ExistDinnerPunchInTurn(oDinnerPunch.Punch, roTypes.Any2Time(orow.Item("BeginTime")).Value, roTypes.Any2Time(orow.Item("EndTime")).Value, iIDTurn)) Then
                            'Si no encontramos marcaje es que podemos fichar
                            oState.Result = roTerminalsState.ResultEnum.DinnerTurnValid
                            oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.TurnValid
                            lAvailableTurns.Add(New roTerminalDiningTurn With {.Id = iIDTurn, .Name = sTurnName})
                            If Not bAllowDiningTurnSelection Then Exit For
                        Else
                            If lAvailableTurns.Count = 0 OrElse Not bAllowDiningTurnSelection Then
                                oState.Result = roTerminalsState.ResultEnum.OnlyOneDinnerPerTurn
                                oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.TurnWithPunch
                            End If
                        End If
                    End If
                Next

                oDinnerPunch.EmployeeStatus.DinnerStatus.AvailableTurns = lAvailableTurns.ToArray
                oDinnerPunchData.Result = oDinnerPunch.EmployeeStatus.DinnerStatus.Result
                oDinnerPunch.EmployeeStatus.DinnerStatus.Allowed = (oState.Result = roTerminalsState.ResultEnum.DinnerTurnValid)

                If oState.Result = roTerminalsState.ResultEnum.DinnerTurnValid Then
                    oDinnerPunch.EmployeeStatus.DinnerStatus.Allowed = True
                    oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.TurnValid
                    oState.Result = roTerminalsState.ResultEnum.NoError
                End If

                oDinnerPunch.Punch.PunchData.DinnerData = oDinnerPunchData

                oRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::CheckDinnerPunch : ", ex)
                oState.Result = roTerminalsState.ResultEnum.ErrorCheckingDinnerTurn
                oDinnerPunch.EmployeeStatus.DinnerStatus.Result = DinnerPunchResultType.NoTurn
                oRet = False
            End Try

            Return oRet
        End Function

        Private Function ExistDinnerPunchInTurn(ByVal oDinnerPunch As roTerminalPunch, ByVal BeginDate As DateTime, ByVal EndDate As DateTime, ByVal idTurn As Integer) As Boolean
            Try
                Dim sSQL As String
                sSQL = "@SELECT# top 1 idEmployee from Punches"
                sSQL += " where idEmployee=" + oDinnerPunch.IDEmployee.ToString
                sSQL += " and Type=10 and InvalidType is null"
                sSQL += " and datetime between " + roTypes.Any2Time(oDinnerPunch.PunchDateTime.Date + BeginDate.TimeOfDay).SQLSmallDateTime
                sSQL += " and " + roTypes.Any2Time(oDinnerPunch.PunchDateTime.Date + EndDate.TimeOfDay).SQLSmallDateTime
                sSQL += " and TypeData = " + idTurn.ToString

                'Si encuentra un valor es que hay marcaje
                Dim ires As Integer = 0
                ires = roTypes.Any2Integer(ExecuteScalar(sSQL))
                Return ires > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::ExistDinnerPunchInTurn:Error:", ex)
                Return False
            End Try
        End Function

        Public Function LoadNextAvailableTasks(ByRef oCurrentPunch As roTerminalInteractivePunch) As Boolean
            Dim oRet As Boolean = False

            Try
                'Dim oTasksList As New Generic.List(Of Task.roTask)
                Dim oTerminalTasks As New Generic.List(Of roTerminalProductivTask)
                Dim bProbablyMoreThan20 As Boolean = False
                Dim taskState As New Task.roTaskState

                oCurrentPunch.EmployeeStatus.ProductiveStatus.AvailableTasks = {}

                'oTasksList = Punch.roPunch.GetAllowTasksByEmployeeOnPunchWithPattern(oCurrentPunch.Punch.IDEmployee, Nothing, bProbablyMoreThan20)

                Dim oPunchState As New Punch.roPunchState
                roBusinessState.CopyTo(oState, oPunchState)
                Dim tbEmployeeTasks As DataTable = Punch.roPunch.GetAllowTasksByEmployeeOnPunchV2(oCurrentPunch.Punch.IDEmployee, oPunchState, False, -1)

                ' Añadimos la tarea Sin Tarea (si el empleado no está actualmente en Sin Tarea)
                If Not oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask Is Nothing AndAlso oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask.Id > 0 Then
                    Dim noTask As New VTBusiness.Task.roTask(0, taskState)
                    If Not noTask Is Nothing Then
                        Dim oData As New roTerminalProductivTask
                        oData.Id = noTask.ID
                        oData.Name = noTask.Name
                        oData.Project = ""
                        oData.BarCode = ""
                        oData.RequiredUserFields = {}
                        oData.CanBeCompleted = False
                        oTerminalTasks.Add(oData)
                    End If
                End If

                For Each oTask As DataRow In tbEmployeeTasks.Rows
                    Dim oData As New roTerminalProductivTask
                    oData.Id = oTask("ID")
                    oData.Name = oTask("Name")
                    LoadTaskRequiredFields(oData)
                    oData.Project = oTask("Project")
                    oData.BarCode = oTask("BarCodeStr")
                    oData.CanBeCompleted = roTypes.Any2Integer(oTask("ActuallyWorking")) > 1
                    If oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask Is Nothing OrElse oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask.Id <> oData.Id Then
                        ' No añado la tarea en curso
                        oTerminalTasks.Add(oData)
                    End If
                Next

                If oTerminalTasks.Count > 0 Then
                    oCurrentPunch.EmployeeStatus.ProductiveStatus.AvailableTasks = oTerminalTasks.ToArray
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::LoadNextAvailableTasks : ", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function LoadNextAvailableCostcenters(ByRef oAvailableCostCentersForEmployee As roTerminalCostCenter(), ByVal idEmployee As Integer, ByRef bIsFixed As Boolean) As Boolean
            Dim oRet As Boolean = False

            Try
                Dim oTable As DataTable
                Dim ret As New List(Of roTerminalCostCenter)
                Dim sSQL As String = String.Empty

                sSQL = "@SELECT# BusinessCenters.ID, BusinessCenters.Name FROM TerminalReaders " &
                       "INNER JOIN BusinessCenters ON BusinessCenters.ID = TerminalReaders.IDCostCenter " &
                       "WHERE Mode LIKE '%CO%' AND TerminalReaders.ID= 1 AND IDTerminal = " & mTerminal.ID.ToString & " " &
                       "AND BusinessCenters.Status = 1 "

                oTable = CreateDataTable(sSQL)

                Dim oCostCenter As roTerminalCostCenter
                If oTable.Rows.Count > 0 Then
                    oCostCenter = New roTerminalCostCenter
                    oCostCenter.Id = roTypes.Any2Integer(oTable.Rows(0)("ID"))
                    oCostCenter.Name = roTypes.Any2String(oTable.Rows(0)("Name"))
                    ret.Add(oCostCenter)
                    bIsFixed = True
                Else
                    bIsFixed = False
                    ' Busco los asignados al empleado
                    Dim oCostCencerState As New BusinessCenter.roBusinessCenterState
                    ' Si no tiene centros de coste asignados, no puede fichar aunque su grupo tenga uno por defecto ...
                    If BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oCostCencerState, idEmployee, False, True, mTerminal.ReaderByID(1).IDZone).Rows.Count > 0 Then
                        ' Si tiene centros de coste asignados, puede fichar por estos o por el asignado a por el que su grupo tenga por defecto ...
                        oTable = New DataTable
                        oTable = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oCostCencerState, idEmployee, True, True, mTerminal.ReaderByID(1).IDZone)
                        If oTable.Rows.Count > 0 Then
                            For Each oRow As DataRow In oTable.Rows
                                oCostCenter = New roTerminalCostCenter
                                oCostCenter.Id = roTypes.Any2Integer(oRow.Item("IDCenter"))
                                oCostCenter.Name = roTypes.Any2String(oRow.Item("Name").ToString)
                                ret.Add(oCostCenter)
                            Next
                        End If
                    End If
                End If

                oAvailableCostCentersForEmployee = ret.ToArray

                oRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::LoadNextAvailableTasks : ", ex)
                oRet = False
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' TO BE CONTINUED -- NO BORRAR --
        ''' Gestión de fichajes desde servidor
        ''' </summary>
        ''' <param name="oIncomingTerminalPunch"></param>
        ''' <param name="oCurrentPunch"></param>
        ''' <returns></returns>
        'Public Function ProcessServerDrivenPunch(oIncomingTerminalPunch As roTerminalInteractivePunch, ByRef oCurrentPunch As roTerminalPunch) As roTerminalInteractivePunch
        '    Dim oResponse As roTerminalInteractivePunch = New roTerminalInteractivePunch(InteractivePunchCommand.Display)
        '    Dim oLogic As roTerminalLogicOnline = New roTerminalLogicOnline(Me.mTerminal)
        '    Try
        '        Select Case oIncomingTerminalPunch.Command
        '            Case InteractivePunchCommand.Punch
        '                ' Si tengo en caché un fichaje en curso, lo guardo ahora
        '                If Not oCurrentPunch Is Nothing Then
        '                    If Not Me.SavePunch(oCurrentPunch) Then
        '                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Process:ProcessPunch: Unable to save current punch")
        '                    End If
        '                End If

        '                ' Creo fichaje entrante
        '                oCurrentPunch = New roTerminalPunch
        '                oCurrentPunch = oIncomingTerminalPunch.Punch

        '                ' Cargo información adicional
        '                'If Not roTerminalPunchHelper.LoadPunchExtraInfo(oCurrentPunch, oIncomingTerminalPunch, Me.oState) Then
        '                '    roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Process:ProcessPunch: Unable to load addicional punch info")
        '                'End If

        '                oResponse = oLogic.STATE_OUT_PRE(oIncomingTerminalPunch, oCurrentPunch)

        '            Case InteractivePunchCommand.Display
        '                Select Case oCurrentPunch.PunchState
        '                    Case roTerminalLogicOnline.ePunchState.IDLE.ToString

        '                    Case roTerminalLogicOnline.ePunchState.PRE.ToString
        '                        oResponse = oLogic.STATE_IN_PRE(oIncomingTerminalPunch, oCurrentPunch)

        '                    Case roTerminalLogicOnline.ePunchState.INIT.ToString

        '                    Case roTerminalLogicOnline.ePunchState.PRE_INV
        '                        oResponse = oLogic.STATE_OUT_IDLE(oIncomingTerminalPunch, oCurrentPunch)
        '                End Select
        '                ' Si el estado del fichaje es IDLE, guardo y elimino de caché
        '                If Not oCurrentPunch Is Nothing AndAlso oCurrentPunch.PunchState = roTerminalLogicOnline.ePunchState.IDLE.ToString Then
        '                    If Not Me.SavePunch(oCurrentPunch) Then
        '                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Process:ProcessPunch: Unable to save current punch")
        '                    Else
        '                        oCurrentPunch = Nothing
        '                    End If
        '                End If
        '        End Select

        '    Catch ex As DbException
        '        oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::Process")
        '        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Process:DbException: ", ex)
        '    Catch ex As Exception
        '        oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::Process")
        '        roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::Process:Exception: ", ex)
        '    End Try

        '    Return oResponse
        'End Function

        Public Sub LoadEmployeeStatusOnTerminal(ByRef oCurrentPunch As roTerminalInteractivePunch, oTerminal As Terminal.roTerminal, ByRef oState As roTerminalsState, Optional dLastAttPunchOnTerminalDateTime As Date = Nothing, Optional dLastAttPunchOnTerminalAction As EmployeeAttStatus = DTOs.EmployeeAttStatus.Unknown)
            Dim bCloseTransaction As Boolean = False

            Try
                oCurrentPunch.EmployeeStatus = New roTerminalEmployeeStatus

                Dim idEmployeePassport As Integer = roPassportManager.GetPassportTicket(oCurrentPunch.Punch.IDEmployee, LoadType.Employee).ID

                Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(oCurrentPunch.Punch.IDEmployee, New Employee.roEmployeeState)

                If Not oEmployee Is Nothing Then
                    oCurrentPunch.EmployeeStatus.EmployeeName = oEmployee.Name
                    oCurrentPunch.Display.UserInfo = oEmployee.Name
                    If Not oEmployee.Image Is Nothing Then
                        oCurrentPunch.EmployeeStatus.EmployeePhoto = Convert.ToBase64String(oEmployee.Image)
                    End If
                Else
                    oState.Result = roTerminalsState.ResultEnum.ErroLoadingEmployeeData
                    Exit Sub
                End If

                oCurrentPunch.EmployeeStatus.ServerDate = oCurrentPunch.Punch.PunchDateTime

                ' Verificamos si las comunicaciones están habilitadas
                If Not PunchesEnabled() Then
                    oState.Result = roTerminalsState.ResultEnum.CommsDisabled
                    Exit Sub
                End If

                ' Verificamos que el empleado puede fichar en el terminal (por restricción de accesos)
                Dim bAdvancedAccessMode As Boolean = False
                Dim pAdvancedAccessMode As New Robotics.Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", Nothing)
                Dim bIsAccessWithTimeZoneCheck As Boolean = False
                bIsAccessWithTimeZoneCheck = (oCurrentPunch.Punch.Action = "L" OrElse oCurrentPunch.Punch.Action = "AV" OrElse oCurrentPunch.Punch.Action = "AIC" OrElse oCurrentPunch.Punch.Action = "AIR" OrElse oCurrentPunch.Punch.Action = "AIT")
                If Not pAdvancedAccessMode Is Nothing Then bAdvancedAccessMode = roTypes.Any2String(pAdvancedAccessMode.Value) = "1"
                If Not oTerminal Is Nothing AndAlso Not oTerminal.ReaderByID(1) Is Nothing AndAlso Not oTerminal.ReaderByID(1).EmployeePermit(oCurrentPunch.Punch.IDEmployee, False, bAdvancedAccessMode, Nothing, bIsAccessWithTimeZoneCheck) Then
                    oState.Result = roTerminalsState.ResultEnum.NoPermissionForThisAction
                    Exit Sub
                End If

                ' Cargamos mensajes para empleado, si los hay ...
                oCurrentPunch.EmployeeStatus.EmployeeMessages = roTerminalEmployee.GetEmployeeMessages(oCurrentPunch.Punch.IDEmployee, roLog.GetInstance())

                ' Cargamos información de contexto en función de la acción
                ' Para Smart, Productiv, Comedor y Centros de Coste necesito saber el estado de presencia
                Dim oPunchStatus As PunchStatus
                Dim oLastPunch As New Punch.roPunch
                Dim oPresenceMinutes As Integer
                Dim oLastPunchDateTime As Date = New Date(1900, 1, 1)
                If oCurrentPunch.Punch.Action = "SM" OrElse oCurrentPunch.Punch.Action = "C" OrElse oCurrentPunch.Punch.Action = "T" OrElse oCurrentPunch.Punch.Action = "X" OrElse oCurrentPunch.Punch.Action = "E" OrElse oCurrentPunch.Punch.Action = "S" OrElse oCurrentPunch.Punch.Action = "D" Then
                    oCurrentPunch.EmployeeStatus.AttendanceStatus.AttendanceStatus = VTBusiness.Scheduler.roScheduler.GetPresenceStatusEx(oEmployee.ID, DateTime.Now, oPunchStatus, oLastPunchDateTime, oLastPunch, oPresenceMinutes, New Employee.roEmployeeState)
                    Select Case oLastPunch.ActualType
                        Case 1
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchAction = "E"
                        Case 2
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchAction = "S"
                    End Select
                    If Not oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchDateTime.HasValue Then
                        oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchDateTime = New DateTime
                    End If
                    oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchDateTime = oLastPunchDateTime
                    oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchCause = oLastPunch.TypeData
                End If

                Select Case oCurrentPunch.Punch.Action
                    Case "X", "E", "S"
                        ' No se necesitan datos adicionales
                    Case "SM"
                        ' Cargo información adicional
                        If Not roTerminalPunchHelper.LoadSmartPunchExtraInfo(oCurrentPunch, Me.oState) Then
                            oState.Result = roTerminalsState.ResultEnum.ErroLoadingEmployeeData
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::Process:ProcessPunch: Unable to load addicional punch info")
                            Exit Sub
                        End If
                    Case "C"
                        Dim bCanPunchCostCenterWhileInAttOut = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "CanPunchCostCenterWhileInAttOut"))

                        ' Sólo puedo fichar Centros de Coste si estoy dentro. Puede ocurrir (en terminales físicos), que el terminal tenga algún fichaje que aún no se haya descargado
                        If Not bCanPunchCostCenterWhileInAttOut AndAlso oCurrentPunch.EmployeeStatus.AttendanceStatus.AttendanceStatus = EmployeeAttStatus.Outside Then
                            oState.Result = roTerminalsState.ResultEnum.ShouldBeInForThisAction
                            Exit Sub
                        End If

                        Dim idCostCenter As Integer = -1

                        oCurrentPunch.EmployeeStatus.CostsStatus.CostCenterName = BusinessCenter.roBusinessCenter.GetEmployeeWorkingCostCenter(New BusinessCenter.roBusinessCenterState(-1), oCurrentPunch.Punch.IDEmployee, idCostCenter)
                        oCurrentPunch.EmployeeStatus.CostsStatus.CostCenterId = idCostCenter

                        ' Cargo posibles centros de coste a fichar
                        Dim bIsFixed As Boolean = False
                        If Not LoadNextAvailableCostcenters(oCurrentPunch.EmployeeStatus.CostsStatus.AvailableCostCenters, oCurrentPunch.Punch.IDEmployee, bIsFixed) Then
                            oState.Result = roTerminalsState.ResultEnum.ErrorLoadingAvailableCostCentersForEmployee
                            Exit Sub
                        End If
                        If bIsFixed AndAlso Not oCurrentPunch.EmployeeStatus.CostsStatus.AvailableCostCenters Is Nothing AndAlso oCurrentPunch.EmployeeStatus.CostsStatus.AvailableCostCenters.Length = 1 Then
                            oCurrentPunch.Punch.PunchData.CostCenterData.IdCostCenter = oCurrentPunch.EmployeeStatus.CostsStatus.AvailableCostCenters(0).Id
                        End If
                    Case "D"
                        ' Comedor
                        If Not CheckDinnerPunch(oCurrentPunch) Then
                            oState.Result = roTerminalsState.ResultEnum.ErrorCheckingDinnerTurn
                            Exit Sub
                        End If
                    Case "Q"
                        ' Portal del Empleado
                        ' 0.- Cargar todos los permisos
                        ' 1.- ...
                    Case "T"
                        'Sólo puedo fichar ProductiV si estoy dentro
                        If oCurrentPunch.EmployeeStatus.AttendanceStatus.AttendanceStatus = EmployeeAttStatus.Outside Then
                            oState.Result = roTerminalsState.ResultEnum.ShouldBeInForThisAction
                            Exit Sub
                        End If

                        oCurrentPunch.EmployeeStatus.ProductiveStatus.HasCompletePermission = (WLHelper.GetFeaturePermission(idEmployeePassport, "TaskPunches.Complete", "E") >= Permission.Write)
                        oCurrentPunch.EmployeeStatus.ProductiveStatus.ProductiVEnabled = (WLHelper.GetFeaturePermission(idEmployeePassport, "TaskPunches.Punches", "E") >= Permission.Write)
                        If Not oEmployee.Type = "J" OrElse Not oCurrentPunch.EmployeeStatus.ProductiveStatus.ProductiVEnabled Then
                            oState.Result = roTerminalsState.ResultEnum.NoPermissionForThisAction
                            Exit Sub
                        End If

                        Dim oTaskPunch As New Punch.roPunch()
                        oTaskPunch.IDEmployee = oCurrentPunch.Punch.IDEmployee
                        Dim lastTaskPunchID As Integer
                        Dim lastTaskPunchDate As DateTime = DateTime.Now
                        Dim oTask As Task.roTask = Nothing

                        oTaskPunch.GetLastPunchTask(PunchTypeEnum._TASK, lastTaskPunchDate, lastTaskPunchID)
                        oTask = Task.roTask.GetLastTaskByEmployee(oCurrentPunch.Punch.IDEmployee, New Task.roTaskState)

                        If lastTaskPunchID <> -1 AndAlso Not oTask Is Nothing Then
                            oCurrentPunch.EmployeeStatus.ProductiveStatus.LastTaskDate = lastTaskPunchDate
                            oCurrentPunch.EmployeeStatus.ProductiveStatus.TaskName = oTask.Project & ":" & oTask.Name

                            ' 0.- Cargamos información de la tarea actual
                            oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask = New roTerminalProductivTask
                            With oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask
                                .Id = oTask.ID
                                .Project = oTask.Project
                                .Name = oTask.Name
                                .CanBeCompleted = oTask.ID > 0 AndAlso roTypes.Any2Integer(Task.roTask.GetEmployeesWorkingInTask(oTask.ID, New Task.roTaskState)) <= 1
                            End With

                            ' 1.- Cargamos campos de la ficha de la tarea actual por si se deben informar cuando el empleado indique una acción
                            If Not LoadTaskRequiredFields(oCurrentPunch.Punch.PunchData.ProductivData.CurrentTask) Then
                                oState.Result = roTerminalsState.ResultEnum.ErrorLoadingTasksUserFields
                                Exit Sub
                            End If
                        Else
                            oCurrentPunch.EmployeeStatus.ProductiveStatus.TaskName = ""
                        End If

                        ' 2.- Cargamos lista de nuevas tareas
                        If Not LoadNextAvailableTasks(oCurrentPunch) Then
                            oState.Result = roTerminalsState.ResultEnum.ErrorLoadingAvailableTasksForEmployee
                            Exit Sub
                        End If

                        ' 3.- Reflejo método de fichaje
                        Dim iTasksPunchMethod As Integer = 0
                        Dim oAdvParam = New AdvancedParameter.roAdvancedParameter("TasksPunchMethod", New AdvancedParameter.roAdvancedParameterState())

                        Select Case roTypes.Any2Integer(oAdvParam.Value.ToLower.Trim)
                            Case 2
                                oCurrentPunch.EmployeeStatus.ProductiveStatus.TaskPunchMethod = TerminalProductivTaskPunchMethod.Number
                            Case Else
                                oCurrentPunch.EmployeeStatus.ProductiveStatus.TaskPunchMethod = TerminalProductivTaskPunchMethod.List
                        End Select

                End Select
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTTerminals::GetEmployeeStatusOnTerminal")
                oState.Result = roTerminalsState.ResultEnum.Exception
            End Try
        End Sub

        Public Shared Function PunchesEnabled() As Boolean
            Dim bolRet As Boolean = False
            Try
                Dim oParameters As New roParameters("OPTIONS")
                Dim sOffline As String
                sOffline = roTypes.Any2String(oParameters.Parameter(Parameters.CommsOffLine))
                bolRet = (sOffline <> "1")
                Return bolRet
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTTerminals::PunchesEnabled")
                Return False
            End Try
        End Function

        Public Function LoadTaskData(ByRef oTerminalSyncData As roTerminalSyncData, oSyncTask As roTerminalsSyncTasks, ByRef bMaxTasksReached As Boolean, ByRef bCallBroadcaster As Boolean) As Boolean
            Dim bRet As Boolean = True
            Try
                If oSyncTask.IDEmployee > 0 Then
                    ' Cargo información de empleado por si es precisa
                    mCurrentEmployee = New roTerminalEmployee(oState, "RXFFNG")
                    Dim bEmployeeLoaded As Boolean = False
                    Dim bEmployeeInfoRequired As Boolean = True
                    Dim bEmployeeExists As Boolean = True
                    bEmployeeInfoRequired = Not (oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployeetimezones OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delbio OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delcard OrElse oSyncTask.Task = roTerminalsSyncTasks.SyncActions.delemployee)
                    If bEmployeeInfoRequired Then
                        bEmployeeExists = mCurrentEmployee.EmployeeExists(oSyncTask.IDEmployee)
                        bEmployeeLoaded = mCurrentEmployee.LoadEmployee(oSyncTask.IDEmployee, True)
                    End If

                    If bEmployeeLoaded OrElse Not bEmployeeInfoRequired Then
                        Select Case oSyncTask.Task
                            Case roTerminalsSyncTasks.SyncActions.addemployee
                                oTerminalSyncData.Type = TerminalDataType.Employees
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                mTerminalSyncEmployeeSyncData = New roTerminalEmployeeSyncData
                                mTerminalSyncEmployeeSyncData.IDTask = oSyncTask.ID
                                mTerminalSyncEmployeeSyncData.IDEmployee = oSyncTask.IDEmployee
                                mTerminalSyncEmployeeSyncData.Name = mCurrentEmployee.Name
                                mTerminalSyncEmployeeSyncData.PIN = mCurrentEmployee.PIN
                                mTerminalSyncEmployeeSyncData.AllowedCauses = oSyncTask.TaskData
                                mTerminalSyncEmployeeSyncData.ConsentRequired = False
                                mTerminalSyncEmployeeSyncData.IsOnline = mCurrentEmployee.IsOnline
                                mTerminalSyncEmployeeSyncData.Language = mCurrentEmployee.Language
                                Array.Resize(oTerminalSyncData.Employees, oTerminalSyncData.Employees.Length + 1)
                                oTerminalSyncData.Employees(oTerminalSyncData.Employees.Length - 1) = mTerminalSyncEmployeeSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                                ' TODO: Log
                            Case roTerminalsSyncTasks.SyncActions.addcard
                                oTerminalSyncData.Type = TerminalDataType.Cards
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                mTerminalCardSyncData = New roTerminalCardSyncData
                                mTerminalCardSyncData.IDTask = oSyncTask.ID
                                mTerminalCardSyncData.IDEmployee = oSyncTask.IDEmployee
                                mTerminalCardSyncData.IDCard = roTerminalsHelper.ConvertCardForTerminal(mCurrentEmployee.Card.ToString, oState)
                                Array.Resize(oTerminalSyncData.Cards, oTerminalSyncData.Cards.Length + 1)
                                oTerminalSyncData.Cards(oTerminalSyncData.Cards.Length - 1) = mTerminalCardSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                            Case roTerminalsSyncTasks.SyncActions.addbio
                                If mCurrentEmployee.BioData(oSyncTask.IDFinger) IsNot Nothing Then
                                    oTerminalSyncData.Type = TerminalDataType.Fingers
                                    oTerminalSyncData.IDTask = Nothing
                                    oTerminalSyncData.Action = TerminalDataAction.Add
                                    mTerminalFingerSyncData = New roTerminalFingerSyncData
                                    mTerminalFingerSyncData.IDTask = oSyncTask.ID
                                    mTerminalFingerSyncData.IDEmployee = oSyncTask.IDEmployee
                                    mTerminalFingerSyncData.IDFinger = oSyncTask.IDFinger
                                    mTerminalFingerSyncData.FingerData = System.Text.Encoding.UTF8.GetChars(mCurrentEmployee.BioData(oSyncTask.IDFinger))
                                    mTerminalFingerSyncData.TimeStamp = mCurrentEmployee.BioTimeStamp(oSyncTask.IDFinger)
                                    Array.Resize(oTerminalSyncData.Fingers, oTerminalSyncData.Fingers.Length + 1)
                                    oTerminalSyncData.Fingers(oTerminalSyncData.Fingers.Length - 1) = mTerminalFingerSyncData
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 5)
                                Else
                                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: No bio information for employee " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addphoto
                                If mCurrentEmployee.Photo IsNot Nothing Then
                                    oTerminalSyncData.Type = TerminalDataType.EmployeePhotos
                                    oTerminalSyncData.IDTask = Nothing
                                    oTerminalSyncData.Action = TerminalDataAction.Add
                                    mTerminalEmployeePhotoSyncData = New roTerminalEmployeePhotoSyncData
                                    mTerminalEmployeePhotoSyncData.IDTask = oSyncTask.ID
                                    mTerminalEmployeePhotoSyncData.IDEmployee = oSyncTask.IDEmployee
                                    mTerminalEmployeePhotoSyncData.PhotoData = Convert.ToBase64String(roTerminalsHelper.resizeImage(mCurrentEmployee.Photo, 140, 140))
                                    Array.Resize(oTerminalSyncData.Photos, oTerminalSyncData.Photos.Length + 1)
                                    oTerminalSyncData.Photos(oTerminalSyncData.Photos.Length - 1) = mTerminalEmployeePhotoSyncData
                                    iTasksInMessage = iTasksInMessage + 1
                                    bMaxTasksReached = (iTasksInMessage = 15)
                                Else
                                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: No photo information for employee " + oSyncTask.IDEmployee.ToString + ". task: " + oSyncTask.ToString + ". Ignoring")
                                    oSyncTask.DoneEx(oSyncTask.ID)
                                End If
                            Case roTerminalsSyncTasks.SyncActions.addemployeegroup
                                oTerminalSyncData.Type = TerminalDataType.AccessAuthorizations
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                mTerminalAccessAuthorizationSyncData = New roTerminalAccessAuthorizationSyncData
                                mTerminalAccessAuthorizationSyncData.IDTask = oSyncTask.ID
                                mTerminalAccessAuthorizationSyncData.IDEmployee = oSyncTask.IDEmployee
                                mTerminalAccessAuthorizationSyncData.IDAuthorization = oSyncTask.IDFinger
                                Array.Resize(oTerminalSyncData.AccessAuthorizations, oTerminalSyncData.AccessAuthorizations.Length + 1)
                                oTerminalSyncData.AccessAuthorizations(oTerminalSyncData.AccessAuthorizations.Length - 1) = mTerminalAccessAuthorizationSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                            Case roTerminalsSyncTasks.SyncActions.delbio
                                oTerminalSyncData.Type = TerminalDataType.Fingers
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalFingerSyncData = New roTerminalFingerSyncData
                                mTerminalFingerSyncData.IDTask = oSyncTask.ID
                                mTerminalFingerSyncData.IDEmployee = oSyncTask.IDEmployee
                                mTerminalFingerSyncData.IDFinger = oSyncTask.IDFinger
                                mTerminalFingerSyncData.TimeStamp = New Date(1970, 1, 1)
                                Array.Resize(oTerminalSyncData.Fingers, oTerminalSyncData.Fingers.Length + 1)
                                oTerminalSyncData.Fingers(oTerminalSyncData.Fingers.Length - 1) = mTerminalFingerSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                            Case roTerminalsSyncTasks.SyncActions.delcard
                                oTerminalSyncData.Type = TerminalDataType.Cards
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalCardSyncData = New roTerminalCardSyncData
                                mTerminalCardSyncData.IDTask = oSyncTask.ID
                                mTerminalCardSyncData.IDEmployee = oSyncTask.IDEmployee
                                Array.Resize(oTerminalSyncData.Cards, oTerminalSyncData.Cards.Length + 1)
                                oTerminalSyncData.Cards(oTerminalSyncData.Cards.Length - 1) = mTerminalCardSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                            Case roTerminalsSyncTasks.SyncActions.delemployee
                                oTerminalSyncData.Type = TerminalDataType.Employees
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalSyncEmployeeSyncData = New roTerminalEmployeeSyncData
                                mTerminalSyncEmployeeSyncData.IDTask = oSyncTask.ID
                                mTerminalSyncEmployeeSyncData.IDEmployee = oSyncTask.IDEmployee
                                Array.Resize(oTerminalSyncData.Employees, oTerminalSyncData.Employees.Length + 1)
                                oTerminalSyncData.Employees(oTerminalSyncData.Employees.Length - 1) = mTerminalSyncEmployeeSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                            Case roTerminalsSyncTasks.SyncActions.delphoto
                                oTerminalSyncData.Type = TerminalDataType.EmployeePhotos
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalEmployeePhotoSyncData = New roTerminalEmployeePhotoSyncData
                                mTerminalEmployeePhotoSyncData.IDTask = oSyncTask.ID
                                mTerminalEmployeePhotoSyncData.IDEmployee = oSyncTask.IDEmployee
                                Array.Resize(oTerminalSyncData.Photos, oTerminalSyncData.Photos.Length + 1)
                                oTerminalSyncData.Photos(oTerminalSyncData.Photos.Length - 1) = mTerminalEmployeePhotoSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                            Case roTerminalsSyncTasks.SyncActions.delemployeegroup
                                oTerminalSyncData.Type = TerminalDataType.AccessAuthorizations
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalAccessAuthorizationSyncData = New roTerminalAccessAuthorizationSyncData
                                mTerminalAccessAuthorizationSyncData.IDTask = oSyncTask.ID
                                mTerminalAccessAuthorizationSyncData.IDEmployee = oSyncTask.IDEmployee
                                mTerminalAccessAuthorizationSyncData.IDAuthorization = oSyncTask.IDFinger
                                Array.Resize(oTerminalSyncData.AccessAuthorizations, oTerminalSyncData.AccessAuthorizations.Length + 1)
                                oTerminalSyncData.AccessAuthorizations(oTerminalSyncData.AccessAuthorizations.Length - 1) = mTerminalAccessAuthorizationSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 15)
                            Case roTerminalsSyncTasks.SyncActions.adddocument
                                oTerminalSyncData.Type = TerminalDataType.Documents
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                ' Recojo todos los incumplimientos de documentación del empleado
                                GetEmployeeTerminalDocuments(oTerminalSyncData.Documents, oSyncTask.IDEmployee, oSyncTask.ID)
                                bMaxTasksReached = True
                            Case roTerminalsSyncTasks.SyncActions.deldocument
                                oTerminalSyncData.Type = TerminalDataType.Documents
                                oTerminalSyncData.IDTask = Nothing
                                oTerminalSyncData.Action = TerminalDataAction.Delete
                                mTerminalDocumentSyncData = New roTerminalDocumentSyncData
                                mTerminalDocumentSyncData.IDTask = oSyncTask.ID
                                mTerminalDocumentSyncData.IDEmployee = oSyncTask.IDEmployee
                                Array.Resize(oTerminalSyncData.Documents, oTerminalSyncData.Documents.Length + 1)
                                oTerminalSyncData.Documents(oTerminalSyncData.Documents.Length - 1) = mTerminalDocumentSyncData
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 20)
                            Case Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: Unknown or unexpected task ( " + oSyncTask.ToString + ")")
                                oSyncTask.DoneEx(oSyncTask.ID)
                                Return True
                        End Select
                    Else
                        If bEmployeeInfoRequired And Not bEmployeeExists Then
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: Employee " + oSyncTask.IDEmployee.ToString + " does not exist!. Task: " + oSyncTask.ToString + " ignored")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            Return True
                        Else
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: Unable to load employee data. Employee  " + oSyncTask.IDEmployee.ToString + " Task: " + oSyncTask.ToString)
                            ' Retraso todas las tareas relativas a este empleado 1 minuto, para que el resto se procesen
                            oSyncTask.DelayEmployeeTasks(60)
                            ' Si no hay otras tareas en este mensaje, envío uno de ok ...
                            Return True
                        End If
                        bMaxTasksReached = True
                    End If
                Else
                    ' Tareas sin emplaado
                    Select Case oSyncTask.Task
                        Case roTerminalsSyncTasks.SyncActions.setterminalconfig
                            If oSyncTask.TaskData.Length > 0 Then
                                oTerminalSyncData.Type = TerminalDataType.Config
                                oTerminalSyncData.IDTask = oSyncTask.ID
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                oTerminalSyncData.ConfigParameters = Me.GetConfig(oSyncTask.ID, , oSyncTask.TaskData)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 1)
                            Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: No config parameter specified!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.updatefirmware
                            oTerminalSyncData.Type = TerminalDataType.Config
                            oTerminalSyncData.IDTask = oSyncTask.ID
                            oTerminalSyncData.Action = TerminalDataAction.Add
                            Dim oParameter As New roTerminalSyncParameter("FirmwareVersion", "Latest")
                            oParameter.IDTask = oSyncTask.ID
                            oTerminalSyncData.ConfigParameters = {oParameter}
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.setsirens
                            If oSyncTask.TaskData.Length > 0 Then
                                oTerminalSyncData.Type = TerminalDataType.Sirens
                                oTerminalSyncData.IDTask = oSyncTask.ID
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                GetTerminalSirens(oTerminalSyncData.Sirens, oSyncTask.TaskData, oSyncTask.ID)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 1)
                            Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: No sirens data specified!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.delsirens
                            oTerminalSyncData.Type = TerminalDataType.Sirens
                            oTerminalSyncData.IDTask = oSyncTask.ID
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalSirensSyncData = New roTerminalSirensSyncData
                            mTerminalSirensSyncData.IDTask = oSyncTask.ID
                            Array.Resize(oTerminalSyncData.Sirens, oTerminalSyncData.Sirens.Length + 1)
                            oTerminalSyncData.Sirens(oTerminalSyncData.Sirens.Length - 1) = mTerminalSirensSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.delalldocuments
                            oTerminalSyncData.Type = TerminalDataType.Documents
                            oTerminalSyncData.IDTask = oSyncTask.ID
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalDocumentSyncData = New roTerminalDocumentSyncData
                            mTerminalDocumentSyncData.IDTask = oSyncTask.ID
                            mTerminalDocumentSyncData.IDEmployee = 0
                            Array.Resize(oTerminalSyncData.Documents, oTerminalSyncData.Documents.Length + 1)
                            oTerminalSyncData.Documents(oTerminalSyncData.Documents.Length - 1) = mTerminalDocumentSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.setcauses
                            If oSyncTask.TaskData.Length > 0 Then
                                oTerminalSyncData.Type = TerminalDataType.Causes
                                oTerminalSyncData.IDTask = oSyncTask.ID
                                oTerminalSyncData.Action = TerminalDataAction.Add
                                GetTerminalCauses(oTerminalSyncData.Causes, oSyncTask.TaskData, oSyncTask.ID)
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 1)
                            Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: No causes data specified!. (task=" + oSyncTask.ToString + "). Ignoring!")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.getterminalconfig
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: getterminalconfig COMMING SOON !!!!!!!!!!!!!!!!!!!!!!!!!!")
                        Case roTerminalsSyncTasks.SyncActions.delallemployees
                            oTerminalSyncData.Type = TerminalDataType.Employees
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalSyncEmployeeSyncData = New roTerminalEmployeeSyncData
                            mTerminalSyncEmployeeSyncData.IDTask = oSyncTask.ID
                            mTerminalSyncEmployeeSyncData.IDEmployee = 0
                            Array.Resize(oTerminalSyncData.Employees, oTerminalSyncData.Employees.Length + 1)
                            oTerminalSyncData.Employees(oTerminalSyncData.Employees.Length - 1) = mTerminalSyncEmployeeSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.delallemployeegroup
                            oTerminalSyncData.Type = TerminalDataType.AccessAuthorizations
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalAccessAuthorizationSyncData = New roTerminalAccessAuthorizationSyncData
                            mTerminalAccessAuthorizationSyncData.IDTask = oSyncTask.ID
                            mTerminalAccessAuthorizationSyncData.IDEmployee = 0
                            Array.Resize(oTerminalSyncData.AccessAuthorizations, oTerminalSyncData.AccessAuthorizations.Length + 1)
                            oTerminalSyncData.AccessAuthorizations(oTerminalSyncData.AccessAuthorizations.Length - 1) = mTerminalAccessAuthorizationSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.delallcards
                            oTerminalSyncData.Type = TerminalDataType.Cards
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalCardSyncData = New roTerminalCardSyncData
                            mTerminalCardSyncData.IDTask = oSyncTask.ID
                            mTerminalCardSyncData.IDEmployee = 0
                            Array.Resize(oTerminalSyncData.Cards, oTerminalSyncData.Cards.Length + 1)
                            oTerminalSyncData.Cards(oTerminalSyncData.Cards.Length - 1) = mTerminalCardSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.delallbios
                            oTerminalSyncData.Type = TerminalDataType.Fingers
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalFingerSyncData = New roTerminalFingerSyncData
                            mTerminalFingerSyncData.IDTask = oSyncTask.ID
                            mTerminalFingerSyncData.IDEmployee = 0
                            mTerminalFingerSyncData.IDFinger = oSyncTask.IDFinger
                            mTerminalFingerSyncData.TimeStamp = New Date(1970, 1, 1)
                            Array.Resize(oTerminalSyncData.Fingers, oTerminalSyncData.Fingers.Length + 1)
                            oTerminalSyncData.Fingers(oTerminalSyncData.Fingers.Length - 1) = mTerminalFingerSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.refreshallbios
                            If roTerminalsHelper.GetVersionCode(mTerminal.FirmVersion) > 20005 Then
                                oTerminalSyncData.Type = TerminalDataType.Device
                                oTerminalSyncData.IDTask = oSyncTask.ID
                                oTerminalSyncData.Action = TerminalDataAction.Refresh
                                iTasksInMessage = iTasksInMessage + 1
                                bMaxTasksReached = (iTasksInMessage = 1)
                            Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: Terminal " + mTerminal.ToString + " does not support refreshallbios. Ignoring")
                                oSyncTask.DoneEx(oSyncTask.ID)
                            End If
                        Case roTerminalsSyncTasks.SyncActions.delallphotos
                            oTerminalSyncData.Type = TerminalDataType.EmployeePhotos
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalEmployeePhotoSyncData = New roTerminalEmployeePhotoSyncData
                            mTerminalEmployeePhotoSyncData.IDTask = oSyncTask.ID
                            mTerminalEmployeePhotoSyncData.IDEmployee = 0
                            Array.Resize(oTerminalSyncData.Photos, oTerminalSyncData.Photos.Length + 1)
                            oTerminalSyncData.Photos(oTerminalSyncData.Photos.Length - 1) = mTerminalEmployeePhotoSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.addtimezone
                            oTerminalSyncData.Type = TerminalDataType.TimeZones
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Add
                            mTerminalTimeZoneSyncData = New roTerminalTimezoneSyncData
                            GetTerminalTimeZone(mTerminalTimeZoneSyncData, oSyncTask.TaskData, oSyncTask.ID)
                            Array.Resize(oTerminalSyncData.TimeZones, oTerminalSyncData.TimeZones.Length + 1)
                            oTerminalSyncData.TimeZones(oTerminalSyncData.TimeZones.Length - 1) = mTerminalTimeZoneSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 15)
                        Case roTerminalsSyncTasks.SyncActions.delalltimezones
                            oTerminalSyncData.Type = TerminalDataType.TimeZones
                            oTerminalSyncData.IDTask = Nothing
                            oTerminalSyncData.Action = TerminalDataAction.Delete
                            mTerminalTimeZoneSyncData = New roTerminalTimezoneSyncData
                            mTerminalTimeZoneSyncData.IDTask = oSyncTask.ID
                            mTerminalTimeZoneSyncData.IDAuthorization = 0
                            Array.Resize(oTerminalSyncData.TimeZones, oTerminalSyncData.TimeZones.Length + 1)
                            oTerminalSyncData.TimeZones(oTerminalSyncData.TimeZones.Length - 1) = mTerminalTimeZoneSyncData
                            iTasksInMessage = iTasksInMessage + 1
                            bMaxTasksReached = (iTasksInMessage = 1)
                        Case roTerminalsSyncTasks.SyncActions.getallemployees
                        Case roTerminalsSyncTasks.SyncActions.getallemployeeaccesslevel
                        Case roTerminalsSyncTasks.SyncActions.getalltimezones
                        Case roTerminalsSyncTasks.SyncActions.getallfingerprints
                        Case roTerminalsSyncTasks.SyncActions.delallemployeetimezones
                        Case roTerminalsSyncTasks.SyncActions.reboot
                        Case roTerminalsSyncTasks.SyncActions.cleardata
                        Case roTerminalsSyncTasks.SyncActions.check
                        Case roTerminalsSyncTasks.SyncActions.log
                        Case roTerminalsSyncTasks.SyncActions.info
                        Case Else
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData: Unknown or unexpected task ( " + oSyncTask.ToString + ")")
                            oSyncTask.DoneEx(oSyncTask.ID)
                            Return True
                    End Select
                End If
                If mCurrentEmployee IsNot Nothing Then mCurrentEmployee = Nothing
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData for task ( " + oSyncTask.ToString + " ). DBError:", ex)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::LoadTaskData for task ( " + oSyncTask.ToString + " ). Error:", ex)
            End Try
            Return bRet
        End Function

        Public Function CreateUnregisteredAlert(UID As String, sTerminalType As String, sTerminalLocation As String) As Boolean
            Dim lRet As Boolean = False
            Try
                roTerminalsHelper.CreateUserTask(UID, sTerminalType, sTerminalLocation, oState)
                lRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager:CreateUnregisteredAlert")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager:CreateUnregisteredAlert:Exception: ", ex)
            End Try
            Return lRet
        End Function

        Public Function DeleteUnregisteredAlert(UID As String) As Boolean
            Dim lRet As Boolean = False
            Try
                roTerminalsHelper.DelUserTask(UID, oState)
                lRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager:DeleteUnregisteredAlert")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager:DeleteUnregisteredAlert:Exception: ", ex)
            End Try
            Return lRet
        End Function

        Public Function GetTerminalCauses(ByRef aCauses As roTerminalCauseSyncData(), sTaskData As String, iTaskID As Integer) As Boolean
            Dim lRet As Boolean = False
            Dim oCauseSyncData As roTerminalCauseSyncData = Nothing
            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.CausesDataTable = dsLocalData.Causes
                Dim oRow As LocalDataSet.CausesRow
                Dim res As String = ""

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        oCauseSyncData = New roTerminalCauseSyncData
                        oCauseSyncData.IDTask = iTaskID
                        oCauseSyncData.IDCause = oRow.IDCause
                        oCauseSyncData.Name = oRow.Name
                        Array.Resize(aCauses, aCauses.Length + 1)
                        aCauses(aCauses.Length - 1) = oCauseSyncData
                    Next

                End If
                lRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalCauses")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalCauses::Error getting causes info for task data ( " + sTaskData + " ). Error:", ex)
                lRet = False
            End Try
            Return lRet
        End Function

        Public Function GetTerminalSirens(ByRef aSirens As roTerminalSirensSyncData(), sTaskData As String, iTaskID As Integer) As Boolean
            Dim lRet As Boolean = False
            Dim oSirenSyncData As roTerminalSirensSyncData = Nothing
            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.SirensDataTable = dsLocalData.Sirens
                Dim oRow As LocalDataSet.SirensRow
                Dim res As String = ""

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    For Each oRow In oTbl.Rows
                        oSirenSyncData = New roTerminalSirensSyncData
                        oSirenSyncData.IDTask = iTaskID
                        oSirenSyncData.DayOf = oRow.DayOf
                        oSirenSyncData.StartDate = oRow.StartDate
                        oSirenSyncData.Relay = oRow.Relay
                        oSirenSyncData.Duration = oRow.Duration
                        Array.Resize(aSirens, aSirens.Length + 1)
                        aSirens(aSirens.Length - 1) = oSirenSyncData
                    Next

                End If
                lRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalSirens")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalSirens::Error getting sirens info for task data ( " + sTaskData + " ). Error:", ex)
                lRet = False
            End Try
            Return lRet
        End Function

        Public Function GetEmployeeTerminalDocuments(ByRef aDocument As roTerminalDocumentSyncData(), ByVal IDEmployee As Integer, ByVal iTaskID As Integer) As Boolean
            Dim bRet As Boolean = False
            Dim tbRet As New DataTable
            Dim strQuery As String = String.Empty
            Dim oDocumentData As roTerminalDocumentSyncData = Nothing
            Try

                Try
                    strQuery = "@SELECT# sepd.IDEmployee, tr.ID as IDReader, dt.Name, gr.Name CompanyName  from sysrovwEmployeePRLDocumentaionFaults sepd " &
                            "Left join TerminalReaders tr on tr.IDZone = sepd.idzone " &
                            "inner join DocumentTemplates dt on dt.Id = sepd.templateid " &
                            "left join Groups gr on gr.ID = sepd.idcompany " &
                            "where tr.idterminal = " & mTerminal.ID.ToString & " " &
                            "and sepd.accessvalidation = " & Robotics.Base.DTOs.DocumentAccessValidation.AccessDenied & " " &
                            "and sepd.IDEmployee = " & IDEmployee.ToString
                    tbRet = CreateDataTable(strQuery)
                Catch ex As Data.Common.DbException
                    roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::GetTerminalDocuments: Unexpected error: " + ex.ToString)
                Catch ex As Exception
                    roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::GetTerminalDocuments: Unexpected error: " + ex.ToString)
                End Try

                For Each dr As DataRow In tbRet.Rows
                    oDocumentData = New roTerminalDocumentSyncData
                    oDocumentData.IDTask = iTaskID
                    oDocumentData.IDEmployee = roTypes.Any2Integer(dr("IDEmployee"))
                    oDocumentData.Name = roTypes.Any2String(dr("Name"))
                    oDocumentData.BeginDate = DateSerial(2000, 1, 1)
                    oDocumentData.EndDate = DateSerial(2000, 1, 1)
                    oDocumentData.Company = roTypes.Any2String(dr("CompanyName"))
                    oDocumentData.DenyAccess = True
                    Array.Resize(aDocument, aDocument.Length + 1)
                    aDocument(aDocument.Length - 1) = oDocumentData
                Next

                bRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalSirens")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::GetTerminalDocuments: Unexpected error: " + ex.ToString)
                bRet = False
            End Try

            Return bRet

        End Function

        Public Function GetTerminalTimeZone(ByRef oTerminalTimeZone As roTerminalTimezoneSyncData, sTaskData As String, iTaskID As Integer) As Boolean
            Dim lRet As Boolean = False

            Try
                'Cargamos definiticion a partir del XML.
                Dim ds As New LocalDataSet
                Dim dsLocalData As New LocalDataSet
                Dim oTbl As LocalDataSet.TimeZonesDataTable = dsLocalData.TimeZones
                Dim oRow As LocalDataSet.TimeZonesRow
                Dim res As String = ""

                If sTaskData <> "" Then
                    Dim sTaskDataXML As New System.IO.StringReader(sTaskData)
                    oTbl.ReadXml(sTaskDataXML)
                    If oTbl.Rows.Count = 1 Then
                        oRow = oTbl.Rows(0)
                        oTerminalTimeZone.IDTask = iTaskID
                        oTerminalTimeZone.IDAuthorization = oRow.IDGroup
                        oTerminalTimeZone.DayOf = oRow.DayOf
                        oTerminalTimeZone.StartTime = If(oRow.BeginTime.Year = 1, oRow.BeginTime.AddYears(1970), oRow.BeginTime)
                        oTerminalTimeZone.EndTime = If(oRow.EndTime.Year = 1, oRow.EndTime.AddYears(1970), oRow.EndTime)
                    End If
                End If
                lRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalTimeZone")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::GetTerminalTimeZone::Error getting causes info for task data ( " + sTaskData + " ). Error:", ex)
                lRet = False
            End Try
            Return lRet
        End Function

        Private Function AddEmployeeFinger(oFingerData As roTerminalFingerSyncData) As Boolean
            Dim bRet As Boolean = False
            Dim bLaunchBroadcaster As Boolean = False

            Try
                If oFingerData.IDFinger = -1 Then
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:Added a fingerprint in terminal " & mTerminal.ID.ToString & " for employee " & oFingerData.IDEmployee.ToString & " with IdFinger = -1 at TimeStamp = " & oFingerData.TimeStamp.ToShortDateString & ". Don't know what to do. Ignoring!")
                    Return True
                End If

                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:Added a fingerprint for employee " & oFingerData.IDEmployee.ToString & " IdFinger = " & oFingerData.IDFinger.ToString & " TimeStamp = " & oFingerData.TimeStamp.ToShortDateString & " FPLength = " & oFingerData.FingerData.Length.ToString & " FP=" & oFingerData.FingerData.Substring(0, 20) & "... ")
                mCurrentEmployee = New roTerminalEmployee(oState, "RXFFNG")
                If mCurrentEmployee.LoadEmployee(oFingerData.IDEmployee) Then
                    If mCurrentEmployee.HasBio(oFingerData.IDFinger) Then
                        If mCurrentEmployee.BioTimeStamp(oFingerData.IDFinger) <= oFingerData.TimeStamp Then
                            ' La huella del terminal es más reciente que la de la BBDD. Guardo la recibida.
                            mCurrentEmployee.BioData(oFingerData.IDFinger) = System.Text.Encoding.UTF8.GetBytes(oFingerData.FingerData)
                            mCurrentEmployee.BioTimeStamp(oFingerData.IDFinger) = oFingerData.TimeStamp
                            If mCurrentEmployee.SaveFingerOnDBLive(oFingerData.IDFinger, mTerminal.ID) Then
                                bLaunchBroadcaster = True
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Finger changed for employee " + oFingerData.IDEmployee.ToString + ", finger id " + oFingerData.IDFinger.ToString)
                                If Not mCurrentEmployee.AllowBio Then
                                    'Si no tiene permisos para fichar por huella la borramos del terminal
                                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Employee does not allow bio. Finger will be erased")
                                    roTerminalsHelper.CallBroadcaster(oState, mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, oFingerData.IDEmployee, True, oFingerData.IDFinger)
                                End If
                            Else
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:Unable to save finger on database")
                            End If
                        Else
                            ' La huella de la BBDD es más reciente
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Employee does not allow bio. Finger will be erased")
                                roTerminalsHelper.CallBroadcaster(oState, mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, oFingerData.IDEmployee, True, oFingerData.IDFinger)
                                bLaunchBroadcaster = True
                            Else
                                'Si tiene una fecha anterior generamos tarea para que vuelva a subir la huella
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Employee has newer finger on database. Finger on terminal will be updated")
                                roTerminalsHelper.CallBroadcaster(oState, mTerminal.ID, roTerminalsSyncTasks.SyncActions.addbio, oFingerData.IDEmployee, True, oFingerData.IDFinger)
                                bLaunchBroadcaster = True
                            End If
                        End If
                    Else
                        'Si la bbdd no tiene una huella guardamos la nueva
                        mCurrentEmployee.BioData(oFingerData.IDFinger) = System.Text.Encoding.UTF8.GetBytes(oFingerData.FingerData)
                        mCurrentEmployee.BioTimeStamp(oFingerData.IDFinger) = oFingerData.TimeStamp
                        If mCurrentEmployee.SaveFingerOnDBLive(oFingerData.IDFinger, mTerminal.ID) Then
                            bLaunchBroadcaster = True
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Finger saved for employee " + oFingerData.IDEmployee.ToString + ", finger id " + oFingerData.IDFinger.ToString)
                            If Not mCurrentEmployee.AllowBio Then
                                'Si no tiene permisos para fichar por huella la borramos del terminal
                                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:" + mTerminal.ToString + ":Employee does not allow bio. Finger will be erased")
                                roTerminalsHelper.CallBroadcaster(oState, mTerminal.ID, roTerminalsSyncTasks.SyncActions.delbio, oFingerData.IDEmployee, True, oFingerData.IDFinger)
                            End If
                        Else
                            roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:Unable to save finger on database")
                        End If
                    End If
                Else
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::AddEmployeeFinger:Unable to load employee " + oFingerData.IDEmployee.ToString + " in order to add a fingerprint enrolled in terminal  " + mTerminal.ID.ToString)
                End If

                If bLaunchBroadcaster Then
                    roTerminalsHelper.CallBroadcaster(oState)
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::AddEmployeeFinger:Calling broadcaster for all terminals ")
                End If

                bRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::AddEmployeeFinger")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::AddEmployeeFinger:Error:", ex)
                bRet = False
            End Try
            Return bRet
        End Function

        Private Function DelEmployeeFinger(oFingerData As roTerminalFingerSyncData) As Boolean
            Dim bRet As Boolean = False
            Dim bLaunchBroadcaster As Boolean = False
            Try
                roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::DelEmployeeFinger:Deleted a fingerprint for employee " & oFingerData.IDEmployee.ToString & ". FP=" + If(oFingerData.FingerData.Length > 20, oFingerData.FingerData.Substring(0, 20), ""))
                mCurrentEmployee = New roTerminalEmployee(oState, "RXFFNG")
                If mCurrentEmployee.LoadEmployee(oFingerData.IDEmployee) Then
                    'Si la bbdd tiene una huella eliminamos
                    mCurrentEmployee.BioData(oFingerData.IDFinger) = Array.CreateInstance(GetType(Byte), 0)
                    mCurrentEmployee.BioTimeStamp(oFingerData.IDFinger) = oFingerData.TimeStamp
                    mCurrentEmployee.SaveFingerOnDBLive(oFingerData.IDFinger, mTerminal.ID)
                    bLaunchBroadcaster = True
                Else
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalManager::DelEmployeeFinger:Unable to load employee " + oFingerData.IDEmployee.ToString + " in order to delete a fingerprint in terminal  " + mTerminal.ID.ToString)
                End If

                If bLaunchBroadcaster Then
                    roTerminalsHelper.CallBroadcaster(oState)
                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalsManager::" & mTerminal.ToString & "::DelEmployeeFinger:Calling broadcaster for all terminals ")
                End If

                bRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalsManager::" & mTerminal.ToString & "::DelEmployeeFinger")
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalsManager::" & mTerminal.ToString & "::DelEmployeeFinger:Error:", ex)
                bRet = False
            End Try
            Return bRet
        End Function

        Private Function DebugText(oSyncData As roTerminalSyncData) As List(Of String)
            Dim lRet As New List(Of String)
            Select Case oSyncData.Type
                Case TerminalDataType.Employees
                    If oSyncData.Employees.Count = 1 AndAlso oSyncData.Employees(0).IDEmployee = 0 Then
                        ' Borrado de todos los empleados
                        lRet.Add("All employees")
                    Else
                        For Each oEmpSyncData As roTerminalEmployeeSyncData In oSyncData.Employees
                            lRet.Add("IdEmpleado=" & oEmpSyncData.IDEmployee & " Empleado=" & oEmpSyncData.Name & " Idioma=" & oEmpSyncData.Language & " Justificaciones=" & oEmpSyncData.AllowedCauses & " Online=" & If(oEmpSyncData.IsOnline, "Si", "No") & " PIN=" & oEmpSyncData.PIN)
                        Next
                    End If
                Case TerminalDataType.EmployeePhotos
                    If oSyncData.Photos.Count = 1 AndAlso oSyncData.Photos(0).IDEmployee = 0 Then
                        ' Borrado de todos los elementos del tipo foto
                        lRet.Add("All employee photos")
                    Else
                        For Each oEmpSyncData As roTerminalEmployeeSyncData In oSyncData.Employees
                            lRet.Add("IdEmpleado=" & oEmpSyncData.IDEmployee & " Empleado=" & oEmpSyncData.Name & " Idioma=" & oEmpSyncData.Language & " Justificaciones=" & oEmpSyncData.AllowedCauses & " Online=" & If(oEmpSyncData.IsOnline, "Si", "No") & " PIN=" & oEmpSyncData.PIN)
                        Next
                    End If
                Case TerminalDataType.Causes
                    For Each oCauseSyncData As roTerminalCauseSyncData In oSyncData.Causes
                        lRet.Add("IdJustificación=" & oCauseSyncData.IDCause & " Nombre=" & oCauseSyncData.Name)
                    Next
            End Select
            Return lRet
        End Function

    End Class

#Region "Objetos Business"

    ' TODO: Esto debería ser un DTO + Manager
    Public Class roTerminalEmployee
        Private _EmployeeBus As New Employee.roEmployee
        Private _EmpState As Employee.roEmployeeState = New Employee.roEmployeeState
        Private _ID As Integer
        Private _Name As String = ""
        Private _Card As String = ""
        Private _PIN As String = ""
        Private _AllowCard As Boolean
        Private _AllowPIN As Boolean
        Private _AllowBio As Boolean
        Private _Finger(10) As Boolean
        Private _FingerData(10)() As Byte
        Private _FingerDateStamp(10) As DateTime
        Private _FingerType As String
        Private _Photo As Drawing.Image
        Private _language As String = ""
        Private mMerge As Generic.List(Of AuthenticationMethodMerge)

        Private _AllowProductiVPunch As Boolean = False
        Private _AllowProductivTasksComplete As Boolean = True
        Private _AllowPortal As Boolean = False
        Private _AllowQueryMoves As Boolean = False
        Private _AllowQueryPlanification As Boolean = False
        Private _AllowQueryAccruals As Boolean = False
        Private _AllowRequestExternalJob As Boolean = False
        Private _AllowRequestForgottenPunches As Boolean = False
        Private _AllowRequestVacations As Boolean = False
        Private _AllowRequestPlanification As Boolean = False
        Private _AllowRequestHoursAbsence As Boolean = False
        Private _AllowRequestDaysAbsence As Boolean = False

        Private _IsOnline As Boolean = False
        Private _AllowedCauses As String = "*"

        Private _Request As roTerminalRequest

        Private NULL_DATE As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        Private oState As roTerminalsState

        Public Sub New(ByVal oTerminalsState As roTerminalsState, Optional ByVal sFingerType As String = "")
            oState = oTerminalsState
            _FingerType = sFingerType
        End Sub

        Public Sub New(ByVal IDEmployee As Integer, ByRef Log As roLog, ByVal oTerminalsState As roTerminalsState, Optional ByVal sFingerType As String = "", Optional ByVal bIsOnline As Boolean = False, Optional ByVal bConsentRequired As Boolean = False)
            oState = oTerminalsState
            _ID = IDEmployee
            _Request = New roTerminalRequest(IDEmployee, oState)
            _FingerType = sFingerType
            _IsOnline = bIsOnline
        End Sub

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal Value As Integer)
                _ID = Value
                _Request.IDEmployee = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Card() As String
            Get
                Return _Card
            End Get
            Set(ByVal Value As String)
                _Card = Value
            End Set
        End Property

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(ByVal value As String)
                _PIN = value
            End Set
        End Property

        Public Property AllowCard() As Boolean
            Get
                Return _AllowCard
            End Get
            Set(ByVal Value As Boolean)
                _AllowCard = Value
            End Set
        End Property

        Public Property AllowBio() As Boolean
            Get
                Return _AllowBio
            End Get
            Set(ByVal Value As Boolean)
                _AllowBio = Value
            End Set
        End Property

        Public Property Request() As roTerminalRequest
            Get
                Return _Request
            End Get
            Set(ByVal value As roTerminalRequest)
                _Request = value
            End Set
        End Property

        Public ReadOnly Property AllowProductiv() As Boolean
            Get
                Return roParametersHelper.AllowProductiv(oState) And _EmployeeBus.Type = "J" And _AllowProductiVPunch
            End Get
        End Property

        Public ReadOnly Property AllowProductivTasksComplete() As Boolean
            Get
                Return _AllowProductivTasksComplete
            End Get
        End Property

        Public ReadOnly Property AllowOHP() As Boolean
            Get
                Try
                    Return _EmployeeBus.RiskControlled
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public ReadOnly Property AllowMyData() As Boolean
            Get
                Return _AllowQueryAccruals Or _AllowQueryMoves Or _AllowQueryPlanification Or _AllowRequestExternalJob Or _AllowRequestVacations
            End Get
        End Property

        Public ReadOnly Property AllowPortal() As Boolean
            Get
                Return _AllowQueryAccruals Or _AllowQueryMoves Or _AllowQueryPlanification Or _AllowRequestPlanification Or _AllowRequestVacations Or _AllowRequestDaysAbsence Or _AllowRequestHoursAbsence 'Or _AllowRequestExternalJob
            End Get
        End Property

        Public ReadOnly Property AllowQueryMoves() As Boolean
            Get
                Return _AllowQueryMoves
            End Get
        End Property

        Public ReadOnly Property AllowQueryAccruals() As Boolean
            Get
                Return _AllowQueryAccruals
            End Get
        End Property

        Public ReadOnly Property AllowRequestForgottenPunches() As Boolean
            Get
                Return _AllowRequestForgottenPunches
            End Get
        End Property

        Public ReadOnly Property AllowQueryPlanification() As Boolean
            Get
                Return _AllowQueryPlanification
            End Get
        End Property

        Public ReadOnly Property AllowRequestVacations() As Boolean
            Get
                Return _AllowRequestVacations
            End Get
        End Property

        Public ReadOnly Property AllowRequestPlanification() As Boolean
            Get
                Return _AllowRequestPlanification
            End Get
        End Property

        Public ReadOnly Property AllowRequestHoursAbsence() As Boolean
            Get
                Return _AllowRequestHoursAbsence
            End Get
        End Property

        Public ReadOnly Property AllowRequestDaysAbsence() As Boolean
            Get
                Return _AllowRequestDaysAbsence
            End Get
        End Property

        Public ReadOnly Property AllowRequestExternalJob() As Boolean
            Get
                Return _AllowRequestExternalJob
            End Get
        End Property

        Public ReadOnly Property Photo() As Drawing.Image
            Get
                Try
                    'Comparamos las dos fotos, si es igual a la estandar no la enviamos.
                    Dim ms1 As IO.MemoryStream = New IO.MemoryStream
                    Dim ms2 As IO.MemoryStream = New IO.MemoryStream
                    _Photo.Save(ms2, Drawing.Imaging.ImageFormat.Bmp)
                    My.Resources.Employee256.Save(ms1, Drawing.Imaging.ImageFormat.Bmp)
                    Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider
                    If System.Text.Encoding.UTF8.GetString(md5.ComputeHash(ms1)) <> System.Text.Encoding.UTF8.GetString(md5.ComputeHash(ms2)) Then
                        Return _Photo
                    Else
                        Return Nothing
                    End If
                Catch ex As Exception
                    Return Nothing
                End Try

            End Get
        End Property

        Public Property Language() As String
            Get
                Return _language
            End Get
            Set(ByVal value As String)
                _language = value
            End Set
        End Property

        Public ReadOnly Property HasBio(ByVal Index As Byte) As Boolean
            Get
                Try
                    Return _Finger(Index)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public Property BioData(ByVal Index As Byte) As Byte()
            Get
                If _Finger(Index) Then
                    Return _FingerData(Index)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Byte())
                _FingerData(Index) = value
                _FingerDateStamp(Index) = Now
            End Set
        End Property

        Public Property BioTimeStamp(ByVal Index As Byte) As DateTime
            Get
                Return roTypes.Any2DateTime(_FingerDateStamp(Index))
            End Get
            Set(ByVal value As DateTime)
                _FingerDateStamp(Index) = value
            End Set
        End Property

        Public Property IsOnline() As Boolean
            Get
                Return _IsOnline
            End Get
            Set(ByVal value As Boolean)
                _IsOnline = value
            End Set
        End Property

        Public Property AllowedCauses() As String
            Get
                Return _AllowedCauses
            End Get
            Set(ByVal value As String)
                _AllowedCauses = value
            End Set
        End Property

        Public Function LoadEmployee(ByVal IDEmployee As Integer, Optional bAvoidCauses As Boolean = False) As Boolean
            Dim oEmployeesState As Employee.roEmployeeState = Nothing

            If IDEmployee <= 0 Then Return False

            Try

                'Si es un empleado valido lo carga
                oEmployeesState = New Employee.roEmployeeState
                _EmployeeBus = Employee.roEmployee.GetEmployee(IDEmployee, oEmployeesState)
                _Name = _EmployeeBus.Name

                ' Carga imagen empleado
                If _EmployeeBus.Image IsNot Nothing Then
                    Try
                        Dim ms As MemoryStream = New MemoryStream(_EmployeeBus.Image)
                        _Photo = CType(Image.FromStream(ms), Bitmap)
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(EventType.roDebug, "VTerminals::LoadEmployee : error on load employee image, Employee '" & Me.ID & "', error: " & ex.Message)
                    End Try
                Else
                    _Photo = My.Resources.Employee256
                End If

                Dim oPassport As roPassport = roPassportManager.GetPassport(IDEmployee, LoadType.Employee)
                _ID = IDEmployee
                'Cargo objeto request para el uso del Portal
                _Request = New roTerminalRequest(_ID, oState)
                If oPassport IsNot Nothing Then
                    Try
                        If oPassport.AuthenticationMethods.PinRow IsNot Nothing AndAlso
                           oPassport.AuthenticationMethods.PinRow.Enabled Then
                            _PIN = Robotics.VTBase.roConversions.Any2String(oPassport.AuthenticationMethods.PinRow.Password)
                            _AllowPIN = True
                        End If
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::LoadLive:LoadPin:" & _ID.ToString & ": ", ex)
                    End Try

                    Try
                        If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Any() AndAlso
                           oPassport.AuthenticationMethods.CardRows(0).Enabled Then
                            _Card = oPassport.AuthenticationMethods.CardRows(0).Credential.ToString
                            If _Card <> "0" AndAlso _Card.Length > 0 Then
                                ' Miramos si existe un valor en la tabla CardAliases
                                Dim strSQL As String = "@SELECT# RealValue FROM CardAliases WHERE IDCard = " & _Card.ToString
                                Dim tbCardAliases As DataTable = CreateDataTable(strSQL)
                                If tbCardAliases IsNot Nothing AndAlso tbCardAliases.Rows.Count > 0 Then
                                    _Card = roTypes.Any2String(tbCardAliases.Rows(0).Item("RealValue"))
                                End If
                            End If

                            ' Tratamiento de tarjeta
                            Select Case roParametersHelper.CardType
                                Case roParametersHelper.eCardType.HID
                                    'Tratamiento HID. Sólo terminales ZK
                                    _Card = roTerminalsHelper.ConvertCardForTerminal(_Card, oState)
                            End Select

                            _AllowCard = True
                        End If
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::LoadLive:LoadCard:" & _ID.ToString & ": ", ex)
                    End Try

                    If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Any Then
                        'Si es un terminal de huella las carga
                        ' Si el tipo de huella no está informado se trata del tipo por defecto (huella de ANVIZ -> RXA100)
                        If _FingerType = "" Then _FingerType = "RXA100"

                        For Each oBioRow As roPassportAuthenticationMethodsRow In oPassport.AuthenticationMethods.BiometricRows
                            If oBioRow IsNot Nothing AndAlso oBioRow.Enabled Then
                                _AllowBio = True
                                Select Case oBioRow.Version
                                    Case _FingerType
                                        ' Biometría dedo
                                        _Finger(oBioRow.BiometricID) = oBioRow.BiometricData.Length > 0
                                        _FingerData(oBioRow.BiometricID) = oBioRow.BiometricData
                                        _FingerDateStamp(oBioRow.BiometricID) = oBioRow.TimeStamp
                                End Select
                            End If
                        Next
                    End If

                    _language = oPassport.Language.Key

                    'Obtenemos los permisos
                    Try
                        _AllowQueryMoves = WLHelper.GetFeaturePermission(oPassport.ID, "Punches.Query", "E") >= Permission.Read
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowProductiVPunch = (WLHelper.GetFeaturePermission(oPassport.ID, "TaskPunches.Punches", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowProductivTasksComplete = (WLHelper.GetFeaturePermission(oPassport.ID, "TaskPunches.Complete", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowQueryPlanification = (WLHelper.GetFeaturePermission(oPassport.ID, "Planification.Query", "E") >= Permission.Read)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowQueryAccruals = (WLHelper.GetFeaturePermission(oPassport.ID, "Totals.Query", "E") >= Permission.Read)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestForgottenPunches = (WLHelper.GetFeaturePermission(oPassport.ID, "Punches.Requests.Forgotten", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestExternalJob = (WLHelper.GetFeaturePermission(oPassport.ID, "Punches.Requests.ExternalParts", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestVacations = (WLHelper.GetFeaturePermission(oPassport.ID, "Planification.Requests.Vacations", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestPlanification = (WLHelper.GetFeaturePermission(oPassport.ID, "Planification.Requests.ShiftChange", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestDaysAbsence = (WLHelper.GetFeaturePermission(oPassport.ID, "Planification.Requests.PlannedAbsence", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try
                    Try
                        _AllowRequestHoursAbsence = (WLHelper.GetFeaturePermission(oPassport.ID, "Planification.Requests.PlannedCause", "E") >= Permission.Write)
                    Catch ex As Exception
                    End Try

                    ' Cargo las justificaciones que puede fichar por terminal
                    Try
                        _AllowedCauses = String.Empty
                        If Not bAvoidCauses Then
                            _AllowedCauses = String.Join(",", GetEmployeeAllowedCausesForPunch(_ID).Select(Function(y) y.Id))
                        End If

                        If Not SomeCausesDependsOnEmployee() Then
                            ' Todos los empleados pueden usar las mismas justificaciones
                            _AllowedCauses = ""
                        Else
                            ' Algunos empleados podrán fichar unas justificaciones y otros otras, en función de su ficha
                            ' Indico un - 1 para señalar que no tiene acceso a ninguna justificación
                            If _AllowedCauses.Trim.Length = 0 Then _AllowedCauses = "-1"
                        End If
                    Catch ex As Exception
                    End Try

                    ' Por último cargo si cuando el empleado ficha se debe consultar al terminal (sólo se usa con terminales mx8+)
                    Try
                        _IsOnline = roTerminalMessage.PendingTerminalMessages(_ID, oEmployeesState)
                    Catch ex As Exception
                    End Try

                    Return True
                Else
                    'No tiene contrato activo
                    Return False
                End If
                oPassport = Nothing
                Return False
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::LoadLive:" + _ID.ToString + ": ", ex)
                Return False
            End Try

        End Function

        Public Function PresenceToday(ByVal PunchDate As DateTime) As Integer
            Dim iMinutes As Integer = VTBusiness.Scheduler.roScheduler.PresenceMinutes(_ID, PunchDate, _EmpState)
            Try
                Dim oPunch As Robotics.Base.VTBusiness.Punch.roPunch = VTBusiness.Punch.roPunch.GetLastPunchPres(_ID, _EmpState)
                If oPunch.ActualType = PunchTypeEnum._IN Then
                    iMinutes += DateDiff(DateInterval.Minute, oPunch.DateTime.Value.AddSeconds(-1 * oPunch.DateTime.Value.Second), PunchDate.AddSeconds(-1 * PunchDate.Second))
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::PresenceToday:Error:", ex)
                Return 0
            End Try
            Return iMinutes
        End Function

        Public Function PresenceDetails(ByVal BeginDate As DateTime, ByVal EndDate As DateTime) As DataTable
            Return VTBusiness.Scheduler.roScheduler.PresenceDetail(_ID, BeginDate, EndDate, _EmpState)
        End Function

        Public Function AccrualsQueryLive(ByVal CurrentDate As DateTime) As DataTable
            Dim dt As DataTable = New DataTable
            Try
                Dim oState = New Employee.roEmployeeState(-1)
                dt = VTBusiness.Concept.roConcept.GetAccrualsQuery(_ID, CurrentDate, 1, oState)
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::AccrualsQueryLive:Error:", ex)
            End Try
            Return dt
        End Function

        Public Function HasActiveContract() As Boolean
            Try
                Dim sSQL As String = "@SELECT# IDEmployee from sysrosubvwCurrentEmployeePeriod where idemployee=" + _ID.ToString

                Return Robotics.VTBase.roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "clsEmployeeCustom::HasActiveContract:Error:", ex)
                Return False
            End Try
        End Function


        Public Function EmployeeExists(ByVal ID As Integer) As Integer
            Try
                Dim sSQL As String = "@SELECT# count(*) from Employees where id =" + ID.ToString

                Return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::EmployeeExists:Error:", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Obtiene detalle de las solicitudes del empleado
        ''' </summary>
        ''' <param name="showAll"></param>
        ''' <param name="dateStart"></param>
        ''' <param name="dateEnd"></param>
        ''' <param name="filter"></param>
        ''' <param name="orderBy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EmployeeRequests(ByVal showAll As Boolean, ByVal dateStart As Date, ByVal dateEnd As Date, ByVal filter As String, ByVal orderBy As String) As roTerminalRequestsList
            Dim lrret As New roTerminalRequestsList

            Try

                Dim strOrderBy As String = orderBy
                Dim strArrStatus() As String = {"pending", "ongoing", "accepted", "denied"}
                Dim strArrRequestType() As String = {"", "UserFieldsChange", "ForbiddenPunch", "JustifyPunch", "ExternalWorkResumePart", "ChangeShift", "VacationsOrPermissions", "PlannedAbsences", "ExchangeShiftBetweenEmployees", "PlannedCauses", "ForbiddenTaskPunch"}

                'Dim resultFilter As String = RequestHelper.CheckResultFilter(showAll, sDate, eDate, filter)
                Dim resultFilter As String = roTerminalRequestHelper.CheckResultFilter(showAll, dateStart, dateEnd, filter)

                Dim oRequestState As New Requests.roRequestState
                Dim dtblQuery As DataTable = Requests.roRequest.GetRequestsByEmployee(Me.ID, resultFilter, strOrderBy, oRequestState)

                Dim curRequestList As New Generic.List(Of roTerminalRequestDetail)
                If dtblQuery IsNot Nothing Then

                    For Each dRow As DataRow In dtblQuery.Rows
                        Dim curRequest = New roTerminalRequestDetail

                        Dim strNameRequest As String = ""
                        Dim strIcoStatus As String = "pending"
                        Dim strNameStatus As String = "pending"

                        curRequest.Name = "RequestName." & strArrRequestType(dRow("RequestType"))
                        curRequest.NameStatus = "RequestStatus." & strArrStatus(dRow("Status"))
                        curRequest.RequestType = strArrRequestType(dRow("RequestType"))
                        curRequest.NotReaded = roTypes.Any2Boolean(dRow("NotReaded"))
                        curRequest.RequestDate = dRow("RequestDate")
                        curRequest.Id = dRow("ID")
                        curRequest.Title = "ShowDetail"
                        If Not IsDBNull(dRow("FieldName")) Then curRequest.RequestedFieldName = dRow("FieldName")

                        curRequest.ObjectDateStart = NULL_DATE
                        If Not IsDBNull(dRow("Date1")) Then curRequest.ObjectDateStart = dRow("Date1")

                        Dim endDate As DateTime = NULL_DATE
                        If Not IsDBNull(dRow("Date2")) Then endDate = dRow("Date2")
                        If (endDate <> NULL_DATE) Then
                            curRequest.ObjectDateEnd = dRow("Date2")
                            If curRequest.RequestType <> "VacationsOrPermissions" Then
                                curRequest.ObjectDateDuration = (endDate.DayOfYear - curRequest.ObjectDateStart.DayOfYear) + 1
                            Else
                                Dim oShift As Shift.roShift
                                Dim oShiftState As New Shift.roShiftState
                                oShift = New Shift.roShift(roTypes.Any2Integer(dRow("IDSHift")), oShiftState, False)
                                If Not oShift.AreWorkingDays Then
                                    curRequest.ObjectDateDuration = (endDate.DayOfYear - curRequest.ObjectDateStart.DayOfYear) + 1
                                Else
                                    curRequest.ObjectDateDuration = VTBusiness.Common.roBusinessSupport.LaboralDaysInPeriod(Me.ID, curRequest.ObjectDateStart, endDate, _EmpState)
                                End If
                            End If
                        End If
                        If Not IsDBNull(dRow("FromTime")) Then curRequest.ObjectHourStart = dRow("FromTime")
                        If Not IsDBNull(dRow("ToTime")) Then curRequest.ObjectHourEnd = dRow("ToTime")
                        If Not IsDBNull(dRow("IDShift")) Then
                            Dim state As New Shift.roShiftState
                            Dim shift As New Shift.roShift(dRow("IDShift"), state)
                            curRequest.RequestedShiftName = shift.Name
                        End If
                        If Not IsDBNull(dRow("IDCause")) Then
                            Dim state As New Cause.roCauseState
                            Dim cause As New Cause.roCause(dRow("IDCause"), state)
                            curRequest.RequestedCauseName = cause.Name
                        End If
                        If Not IsDBNull(dRow("Hours")) Then curRequest.RequestedHours = dRow("Hours")
                        curRequestList.Add(curRequest)
                    Next
                End If
                lrret.Requests = curRequestList.ToArray()
            Catch ex As Exception
                lrret.Status = -4
            End Try

            Return lrret
        End Function

        Public Function EmployeeCalendarInfo(ByVal iShiftID As Integer) As roTerminalHolidaysInfo
            Dim lrret As New roTerminalHolidaysInfo

            Try
                Dim oShiftState As New Shift.roShiftState

                Dim vacInfoShifts As New Generic.List(Of roTerminalHolidaysResumeShifts)
                'For Each cRow As DataRow In tbShifts.Rows
                Dim cInfoShift As New roTerminalHolidaysResumeShifts()

                Dim VacationsResumeHeaders() As String = {"VacationsResumeHeader.Done",
                                                      "VacationsResumeHeader.Pending",
                                                      "VacationsResumeHeader.Lasting",
                                                      "VacationsResumeHeader.Disponible"}
                Dim VacationsResumeValue() As Double = {0, 0, 0, 0, 0, 0}

                ' Obtenemos información resumen días vacaciones
                Dim iDisponible As Double = 0
                VTBusiness.Common.roBusinessSupport.VacationsResumeQuery(Me.ID, iShiftID, Now, Nothing, Nothing, Now, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), iDisponible, _EmpState, VacationsResumeValue(4), VacationsResumeValue(5))
                VacationsResumeValue(3) = iDisponible - VacationsResumeValue(2) - VacationsResumeValue(1) - VacationsResumeValue(4) - VacationsResumeValue(5)

                cInfoShift.VacationsResumeHeaders = VacationsResumeHeaders
                cInfoShift.VacationsResumeValue = VacationsResumeValue
                Dim oShift As Shift.roShift
                oShift = New Shift.roShift(iShiftID, oShiftState, False)
                cInfoShift.VacationsShiftName = oShift.Name
                oShift = Nothing
                oShiftState = Nothing

                vacInfoShifts.Add(cInfoShift)

                lrret.Shifts = vacInfoShifts.ToArray()
                lrret.Status = 0
            Catch ex As Exception
                lrret.Status = -4
            End Try

            Return lrret
        End Function

        Public Function SomeCausesDependsOnEmployee() As Boolean
            Dim iCount As Integer = 0
            Try
                Dim sSQL As String
                sSQL = "@SELECT# count(*)  FROM Causes " &
                       "WHERE [ID] > 0 AND AllowInputFromReader = 1 AND InputPermissions = 2"
                iCount = roTypes.Any2Integer(ExecuteScalar(sSQL))

                Return iCount > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "TerminalMX7::SomeCausesDependsOnEmployee: Unexpected error: ", ex)
                Return False
            End Try
        End Function

        Public Function SaveFingerOnDBLive(ByVal IDFinger As Byte, ByVal iIdTerminal As Integer) As Boolean
            Try
                Dim oPassport As roPassport = roPassportManager.GetPassport(_ID, LoadType.Employee)
                If oPassport IsNot Nothing Then
                    Dim oPassportManager As roPassportManager = New roPassportManager
                    Dim oMethod As roPassportAuthenticationMethodsRow = Nothing

                    If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                        oMethod = oPassport.AuthenticationMethods.BiometricRows.ToList.Find(Function(x) x.Version = _FingerType AndAlso x.BiometricID = IDFinger)
                    End If

                    If oMethod IsNot Nothing Then
                        'ACTUALIZO HUELLA EXISTENTE

                        oMethod.BiometricData = _FingerData(IDFinger)
                        oMethod.TimeStamp = _FingerDateStamp(IDFinger)
                        oMethod.BiometricTerminalId = iIdTerminal
                        oMethod.RowState = RowState.UpdateRow

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"roTerminalsManager::SaveFingerOnDBLive:Update finger in employee({_ID},{IDFinger})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsManager::SaveFingerOnDBLive:Error updating finger in employee({_ID},{IDFinger})")
                            Return False
                        End If

                        Return True
                    Else
                        ' NUEVA HUELLA
                        oMethod = New roPassportAuthenticationMethodsRow
                        oMethod.IDPassport = oPassport.ID
                        oMethod.Method = AuthenticationMethod.Biometry
                        oMethod.Credential = String.Empty
                        oMethod.Password = String.Empty
                        oMethod.BiometricID = IDFinger
                        oMethod.BiometricTerminalId = iIdTerminal
                        oMethod.BiometricAlgorithm = String.Empty
                        oMethod.BiometricData = _FingerData(IDFinger)
                        oMethod.TimeStamp = _FingerDateStamp(IDFinger)
                        oMethod.Enabled = True
                        oMethod.Version = _FingerType
                        oMethod.RowState = RowState.NewRow
                        Dim oBiometricRow As List(Of roPassportAuthenticationMethodsRow) = New List(Of roPassportAuthenticationMethodsRow)
                        oBiometricRow.Add(oMethod)
                        If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                            Dim oCurrentBiometricRows As New List(Of roPassportAuthenticationMethodsRow)
                            oCurrentBiometricRows = oPassport.AuthenticationMethods.BiometricRows.ToList
                            oPassport.AuthenticationMethods.BiometricRows = oCurrentBiometricRows.Append(oMethod).ToArray
                        Else
                            oPassport.AuthenticationMethods.BiometricRows = oBiometricRow.ToArray
                        End If

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"roTerminalsManager::SaveFingerOnDBLive:Save finger in employee({_ID},{IDFinger})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsManager::SaveFingerOnDBLive:Error saving finger in employee({_ID},{IDFinger})")
                            Return False
                        End If

                        Return True
                    End If

                    roLog.GetInstance().logMessage(EventType.roDebug, "roTerminalEmployee::SaveFingerOnDBLive:Save finger in employee(" + _ID.ToString + "," + IDFinger.ToString + ")")
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::SaveFingerOnDBLive : ", ex)
                Return False
            End Try
        End Function

        Public Function CreateRequestHolidays(ByVal BeginDate As DateTime, ByVal EndDate As DateTime, Optional ByVal HolidayShift As Integer = 0) As Boolean
            Try
                Return CreateRequestHolidays_VTLive(BeginDate, EndDate, HolidayShift)
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::CreateRequestHolidays : ", ex)
                Return False
            End Try
        End Function

        Public Function CreateRequestHolidays_VTLive(ByVal BeginDate As DateTime, ByVal EndDate As DateTime, ByVal HolidayShift As Integer) As Boolean
            Try
                Dim req As Requests.roRequest
                Dim reqstate As New Requests.roRequestState
                req = New Requests.roRequest()
                req.RequestType = eRequestType.VacationsOrPermissions
                req.IDEmployee = _ID
                req.IDShift = HolidayShift
                req.Date1 = BeginDate
                req.Date2 = EndDate
                req.RequestDate = Now
                req.Comments = "Solicitud de vacaciones"
                If req.Save Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::CreateRequestHolidays_VT: ", ex)
                Return False
            End Try
        End Function

        Public Function CreateExternalWork(ByVal WorkDate As DateTime) As Boolean
            Try
                Dim req As Requests.roRequest
                Dim reqstate As New Requests.roRequestState
                req = New Requests.roRequest()
                req.RequestType = eRequestType.ExternalWorkResumePart
                req.IDEmployee = _ID
                req.RequestDate = Now
                req.Comments = oState.Language.Translate("RequestExternalJob", "Solicitud de trabajo externo.")
                If req.Save Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::CreateRequestHolidays : ", ex)
                Return False
            End Try
        End Function

        Public Sub ProcessString(ByRef Text As String)
            Try
                If Text.IndexOf("{Employee.") >= 0 Then
                    Text = Text.Replace("${Employee.Name}", _Name)
                    Text = Text.Replace("${Employee.ID}", _ID)
                    Text = Text.Replace("${Employee.Card}", _Card)
                    Text = Text.Replace("${Employee.PresenceTime}", roTerminalTextHelper.Minutes2StringLong(PresenceToday(Now), oState))
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::ProcessString : ", ex)
            End Try
        End Sub

        Public Shared Function GetEmployeeMessages(ByVal idEmployee As Integer, oLog As roLog, Optional ByRef sIds As String = "") As roEmployeeMessage()
            Dim aRet() As roEmployeeMessage = {}

            Try
                Dim i As Byte

                Dim strSQL As String
                ' Recupero hasta 3 mensajes
                strSQL = "@SELECT# Schedule,LastTimeShown,Message,Id" &
                         " FROM EmployeeTerminalMessages" &
                         " WHERE IDEmployee=" + idEmployee.ToString

                Dim cmd As DataTable = CreateDataTable(strSQL)
                Dim oEmpMsg As roEmployeeMessage = Nothing

                If cmd.Rows.Count > 0 Then
                    For i = 0 To cmd.Rows.Count - 1
                        If Not IsDBNull(cmd.Rows(i).Item("Schedule")) Then
                            If GetNextEmployeeMessage_MustShow(idEmployee, cmd.Rows(i).Item("Schedule"), cmd.Rows(i).Item("LastTimeShown")) Then
                                Try
                                    oEmpMsg = New roEmployeeMessage
                                    Array.Resize(aRet, aRet.Length + 1)
                                    oEmpMsg.ID = roTypes.Any2Integer(cmd.Rows(i).Item("ID"))
                                    oEmpMsg.Message = roTypes.Any2String(cmd.Rows(i).Item("Message"))
                                    aRet(aRet.Length - 1) = oEmpMsg
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                    Next
                    Return aRet
                End If
                Return {}
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::GetEmployeeMessages:" + idEmployee.ToString + ": ", ex)
                Return {}
            End Try

        End Function

        Public Shared Function SetEmployeeMSGByID(ByVal aMessageIds As Integer()) As Boolean
            Try
                'actualiza tabla
                Dim sSQL As String
                sSQL = "@UPDATE# EmployeeTerminalMessages set LastTimeShown = getdate() where ID in ( " & String.Join(",", aMessageIds) & ")"
                ExecuteSql(sSQL)
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::SetEmployeeMSGByID:" + String.Join(",", aMessageIds) + ": ", ex)
                Return False
            End Try
        End Function

        Private Shared Function GetNextEmployeeMessage_MustShow(ByVal idEmployee As Integer, ByVal Schedule As String, ByVal LastTimeShown As Object) As Boolean
            '
            ' Determina si debemos mostrar un mensaje o no
            '
            Dim oRet As Boolean = False
            Try
                Dim Params() As String
                Dim MonthDiff As Integer
                Dim FirstExecDate As New roTime ' Guarda el mes y el año de la primera vez que se debió ejecutar la regla (el día no es significativo)
                Dim iDayOfMonth As Integer
                ' Obtiene parametros
                Params = Split(Schedule, "@")

                ' Mira el tipo de schedule
                Select Case Params(0)
                    Case "1" ' Una vez. No tiene en cuenta la fecha, se mostrará en el próximo marcaje
                        If IsDBNull(LastTimeShown) Then oRet = True
                    Case "D" ' Cada X dias
                        If IsDBNull(LastTimeShown) Then
                            oRet = True
                        ElseIf DateDiff("d", LastTimeShown, Now) >= roTypes.Any2Long(Params(1)) Then
                            oRet = True
                        End If

                    Case "S" ' Cada cierto dia de la semana
                        If Weekday(Now, vbMonday) = roTypes.Any2Long(Params(1)) Then
                            If IsDBNull(LastTimeShown) Then
                                oRet = True
                            ElseIf roTypes.Any2Time(LastTimeShown).DateOnly <> roTypes.Any2Time(Now).DateOnly Then
                                oRet = True
                            End If
                        End If

                    Case "M" ' Cada cierto dia del mes
                        If Params(1) = "DM" Then
                            ' Dia numero tal cada tantos meses
                            If Day(Now) = roTypes.Any2Long(Params(2)) Then
                                If IsDBNull(LastTimeShown) Then
                                    ' Primera vez
                                    oRet = True
                                Else
                                    ' Otras veces, mira que hayan pasado n meses
                                    MonthDiff = Month(Now) - Month(LastTimeShown) + 12
                                    If MonthDiff < 0 Then MonthDiff = MonthDiff + 12
                                    If MonthDiff Mod roTypes.Any2Long(Params(3)) = 0 Then
                                        If roTypes.Any2Time(LastTimeShown).DateOnly <> roTypes.Any2Time(Now).DateOnly Then
                                            oRet = True
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            ' Cada n día de la semana de cada m meses (tercer jueves de cada 3 meses,etc)
                            If Params(1) = "DS" Then

                                ' Calculamos el día del més al que corresponden los datos
                                iDayOfMonth = Day(GetNthWeekdayOfMonth(Now, roTypes.Any2Long(Params(2)), roTypes.Any2Long(Params(3))))

                                'Calculo la diferencia en meses entre la primera ejecución y la fecha de la tarea
                                MonthDiff = (Year(roTypes.Any2Time(LastTimeShown).DateOnly) - Year(Now)) * 12 _
                                             + Month(roTypes.Any2Time(LastTimeShown).DateOnly) - Month(Now)

                                'Finalmente comprobamos si es el día y el mes concreto para lanzar el cálculo de la regla
                                If IsDBNull(LastTimeShown) Then
                                    If (Day(Now) = iDayOfMonth) Then
                                        oRet = True
                                    End If
                                Else
                                    If (Day(Now) = iDayOfMonth) And MonthDiff Mod roTypes.Any2Long(Params(4)) = 0 And (Day(LastTimeShown) <> iDayOfMonth) Then
                                        oRet = True
                                    End If
                                End If
                            End If
                        End If

                    Case "A" ' Cada cierto dia del año
                        ' Miro primero si la fecha me la pasan directamente o referente a campo de la ficha
                        Dim iDay As Integer = 0
                        Dim iMonth As Integer = 0
                        Dim dRefDate As Date = Date.MinValue
                        If Params.Count > 1 AndAlso Not IsNumeric(Params(1)) Then
                            ' Campo de la ficha
                            Dim oEmployeeState = New VTUserFields.UserFields.roUserFieldState()
                            Dim oEmpUserField As New VTUserFields.UserFields.roEmployeeUserField(oEmployeeState)
                            oEmpUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, Params(1).Trim, Now, oEmployeeState)
                            If Not oEmpUserField Is Nothing AndAlso Not oEmpUserField.FieldValue Is Nothing Then
                                dRefDate = roTypes.Any2DateTime(oEmpUserField.FieldValue)
                                iDay = dRefDate.Day
                                iMonth = dRefDate.Month
                            End If
                        Else
                            iDay = roTypes.Any2Long(Params(1))
                            iMonth = roTypes.Any2Long(Params(2))
                        End If

                        If Day(Now) = iDay And Month(Now) = iMonth Then
                            'Miramos si hay que mostrarlo en un cierto periodo de horas del día
                            Dim dStartHour As DateTime = Nothing
                            Dim dEndHour As DateTime = Nothing

                            If Params.Count > 3 Then
                                dStartHour = New DateTime(Now.Year, Now.Month, Now.Day, CDate(Params(3)).Hour, CDate(Params(3)).Minute, 0)
                            End If
                            If Params.Count > 4 Then
                                dEndHour = New DateTime(Now.Year, Now.Month, Now.Day, CDate(Params(4)).Hour, CDate(Params(4)).Minute, 0)
                            End If

                            ' Si se informa periodo de horas se deben informar las dos
                            If Not dStartHour = Nothing AndAlso Not dEndHour = Nothing Then
                                If IsDBNull(LastTimeShown) AndAlso Now >= dStartHour AndAlso Now <= dEndHour Then
                                    oRet = True
                                ElseIf IsDBNull(LastTimeShown) AndAlso Now >= dStartHour Then
                                    ' Pasó la hora en que debía mostrarlo. Aunque no lo haya mostrado, no lo muestro nunca más
                                    ' TODO: Lo marco como mostrado?. Si lo marco pierdo la info de que no lo mostré. Si no lo marco, lo procesaré siempre.
                                    oRet = False
                                End If
                            Else
                                If IsDBNull(LastTimeShown) Then
                                    oRet = True
                                ElseIf roTypes.Any2Time(LastTimeShown).DateOnly <> roTypes.Any2Time(Now).DateOnly Then
                                    oRet = True
                                End If
                            End If
                        End If
                    Case Else
                        oRet = False
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(EventType.roError, "roTerminalEmployee::GetNextEmployeeMessage_MustShow:" + idEmployee.ToString + ": ", ex)
                oRet = False
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Recupera un string con los id's de justificaciones permitidos por terminal para un empleado dado
        ''' </summary>
        ''' <param name="_IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeeAllowedCausesForPunch(ByVal _IDEmployee As Integer) As List(Of roTerminalListItem)
            Dim lResult As New List(Of roTerminalListItem)
            Dim oDataSet As DataSet = Nothing
            Dim oCauseState As New Cause.roCauseState
            Dim oData As System.Data.DataTable = Nothing

            Try
                oData = Cause.roCause.GetCausesByEmployeeInputPermissions(_IDEmployee, oCauseState)
                For Each row As System.Data.DataRow In oData.Rows
                    lResult.Add(New roTerminalListItem(row.Item("ID").ToString, row.Item("Name").ToString))
                Next
                Return lResult
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTTerminals::roTerminalEmployee::GetEmployeeAllowedCausesForPunch")
                roLog.GetInstance().logMessage(EventType.roError, "VTTerminals::roTerminalEmployee::GetEmployeeAllowedCausesForPunch: Unexpected error: ", ex)
            Finally
                If Not oData Is Nothing Then oData.Dispose()
            End Try
            Return Nothing
        End Function

        Public Shared Function GetNthWeekdayOfMonth(ByVal pdate As Date, ByVal iNthWeekDay As Integer, ByVal iWeekDay As Integer) As Date
            '
            '   Calcula el enésimo día de la semana de un mes dado (ejemplo: 3er martes del mes --> iNthWeekDay=3, iWeekDay)
            '

            Dim iWeekdayFirstDayOfMonth As Integer

            ' ¿Qué día de la semana es el primer día del mes?
            iWeekdayFirstDayOfMonth = Weekday(GetFirstDayOfMonth(pdate), vbMonday)

            ' Calculamos qué día del mes que corresponde
            If iWeekdayFirstDayOfMonth > iWeekDay Then
                GetNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * iNthWeekDay, GetFirstDayOfMonth(pdate))
            Else
                GetNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * (iNthWeekDay - 1), GetFirstDayOfMonth(pdate))
            End If

        End Function

        Public Shared Function GetFirstDayOfMonth(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '
            GetFirstDayOfMonth = "1/" & DatePart("m", pdate) - pDif & "/" & DatePart("yyyy", pdate)

        End Function

        Public Shared Function GetIDEmployeeFromCardCredential(sCredential As String) As Integer
            Dim lRet As Integer = 0
            Try
                Dim sCardInDB As String = String.Empty
                sCardInDB = roTerminalsHelper.ConvertCardForVTDatabase(sCredential, New roTerminalsState(-1))
                Dim oEmpState As New Employee.roEmployeeState(-1)
                ' Ahora busco en BBDD, teniendo en cuenta el tipo de codificación ...
                Select Case roParametersHelper.CardCode
                    Case roParametersHelper.eCardCode.Numeric
                        lRet = VTBusiness.Common.roBusinessSupport.GetEmployeeIDFromCardIDHex(Hex(sCardInDB), oEmpState)
                    Case Else
                        lRet = VTBusiness.Common.roBusinessSupport.GetEmployeeIDFromCardID(sCardInDB, oEmpState)
                End Select
            Catch ex As Exception
                lRet = -1
            End Try
            Return lRet
        End Function

    End Class

#End Region

#Region "Helpers"

    Public Class roTerminalPunchHelper

        Public Shared Function LoadSmartPunchExtraInfo(oCurrentPunch As roTerminalInteractivePunch, oState As roTerminalsState) As Boolean
            Dim oRet As Boolean = True
            Try
                Dim iMaxHours As Integer = 0
                Dim iMaxRepeatedIn As Integer = 0
                Dim iMaxRepeatedOut As Integer = 0
                Dim oLastPunchDateTime As Date?

                oLastPunchDateTime = oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchDateTime

                iMaxHours = roParametersHelper.MovMaxHours(oState)
                iMaxRepeatedIn = roParametersHelper.PunchPeriodRTIn(oState)
                iMaxRepeatedOut = roParametersHelper.PunchPeriodRTOut(oState)
                Dim bTimeSinceFirstIn As Boolean = roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "Terminals.TimeSinceFirstPunchOfDay"))

                ' Vemos si es el primer fichaje de presencia del día
                If oLastPunchDateTime.HasValue Then
                    If iMaxHours > 0 Then
                        oCurrentPunch.EmployeeStatus.AttendanceStatus.FirstDayPunch = Math.Abs(oCurrentPunch.Punch.PunchDateTime.Subtract(oLastPunchDateTime.Value).TotalHours) > iMaxHours
                    Else
                        oCurrentPunch.EmployeeStatus.AttendanceStatus.FirstDayPunch = oCurrentPunch.Punch.PunchDateTime.Date <> oLastPunchDateTime.Value.Date
                    End If
                Else
                    oCurrentPunch.EmployeeStatus.AttendanceStatus.FirstDayPunch = True
                End If

                Dim minutesWorkedDay As Integer = 0
                'Calculamos las horas trabajadas desde el primer fichaje del dia al ultimo fichaje, incluido el actual fichaje, como se hacia en el mx8
                If bTimeSinceFirstIn AndAlso Not oCurrentPunch.EmployeeStatus.AttendanceStatus.FirstDayPunch Then
                    'Dim sSQL = $"@SELECT# Value FROM [sysrovwDailyEfectiveWorkingHours] 
                    '             WHERE IDEmployee = {oCurrentPunch.Punch.IDEmployee} AND Date = {roTypes.Any2Time(oCurrentPunch.Punch.PunchDateTime.Date).SQLDateTime}"
                    Dim sSQL = $"@SELECT# Value FROM sysrfnDailyEfectiveWorkingHours({oCurrentPunch.Punch.IDEmployee}, {roTypes.Any2Time(oCurrentPunch.Punch.PunchDateTime.Date).SQLDateTime})"
                    minutesWorkedDay = roTypes.Any2Double(ExecuteScalar(sSQL)) * 60

                    ' En las salidas debemos sumar el tiempo entre la última entrada al fichaje en curso (Salida)
                    If oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchAction = "E" Then
                        Dim minutesDifferenceNow = oCurrentPunch.Punch.PunchDateTime.Subtract(oLastPunchDateTime.Value).TotalMinutes
                        minutesWorkedDay += minutesDifferenceNow
                    End If

                    oCurrentPunch.EmployeeStatus.AttendanceStatus.MinutesSinceFirstPunch = minutesWorkedDay
                End If

                ' Vemos tipo de fichaje y minutos desde el último fichaje
                oCurrentPunch.EmployeeStatus.AttendanceStatus.MinutesFromLastPunch = 0
                If oLastPunchDateTime.HasValue Then
                    Dim dLimit As Double = 0
                    If oCurrentPunch.EmployeeStatus.AttendanceStatus.AttendanceStatus = EmployeeAttStatus.Inside Then
                        dLimit = Math.Abs(oCurrentPunch.Punch.PunchDateTime.Subtract(oLastPunchDateTime.Value).TotalMinutes)
                        If dLimit <> 0 AndAlso dLimit < iMaxRepeatedIn Then
                            'Entrada repetida
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.RepeatIn
                        Else
                            'Es una salida
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.AttOut
                            'Quitamos segundos a la baja para que coincida con criterio de saldos
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.MinutesFromLastPunch = oCurrentPunch.Punch.PunchDateTime.AddSeconds(-1 * oCurrentPunch.Punch.PunchDateTime.Second).Subtract(oLastPunchDateTime.Value.AddSeconds(-1 * oLastPunchDateTime.Value.Second)).TotalMinutes
                        End If
                    Else
                        If oCurrentPunch.EmployeeStatus.AttendanceStatus.LastPunchAction = "E" AndAlso iMaxHours > 0 AndAlso Math.Abs(oCurrentPunch.Punch.PunchDateTime.Subtract(oLastPunchDateTime.Value).TotalHours) > iMaxHours Then
                            ' Está fuera, aunque mi último fichaje fue una entrada, luego se olvidó una salida
                            oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.Incomplete
                        Else
                            dLimit = Math.Abs(oCurrentPunch.Punch.PunchDateTime.Subtract(oLastPunchDateTime.Value).TotalMinutes)
                            If dLimit <> 0 AndAlso dLimit < iMaxRepeatedOut Then
                                oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.RepeatOut
                            Else
                                oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.AttIn
                                'Quitamos segundos a la baja para que coincida con criterio de saldos
                                oCurrentPunch.EmployeeStatus.AttendanceStatus.MinutesFromLastPunch = oCurrentPunch.Punch.PunchDateTime.AddSeconds(-1 * oCurrentPunch.Punch.PunchDateTime.Second).Subtract(oLastPunchDateTime.Value.AddSeconds(-1 * oLastPunchDateTime.Value.Second)).TotalMinutes
                            End If
                        End If
                    End If
                Else
                    ' Primer fichaje de la jistoria para este empleado
                    oCurrentPunch.EmployeeStatus.AttendanceStatus.PunchType = PunchType.AttIn
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalPunchHelper::LoadPunchExtraInfo::Error: ", ex)
            End Try
            Return oRet
        End Function

        Public Shared Function CheckAPBIfNeeded(mTerminal As Terminal.roTerminal, oCurrentPunch As roTerminalInteractivePunch, oState As roTerminalsState) As Boolean
            Dim bAllowAccess As Boolean = True

            Try
                ' Validación APB si corresponde
                If mTerminal.ReaderByID(1).CheckAPB Then
                    Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                    Dim oLastPunchAccessInfo As Punch.roPunch = New Punch.roPunch(oCurrentPunch.Punch.IDEmployee, -1, oPunchState)
                    Dim iLastAPBControlledZone As Integer = -1
                    Dim iLastAccessMoveID As Integer = -1
                    Dim dLastAccessMoveDateTime As Date
                    Dim iLastAccessMoveStatus As Integer

                    oLastPunchAccessInfo.GetLastAccPunchInfo(iLastAccessMoveStatus, dLastAccessMoveDateTime, iLastAccessMoveID, iLastAPBControlledZone, mTerminal.ReaderByID(1).APBControlledZones)

                    If oPunchState.Result = PunchResultEnum.NoError Then
                        bAllowAccess = (mTerminal.ReaderByID(1).IDZone <> iLastAPBControlledZone) OrElse
                                            Now.Subtract(dLastAccessMoveDateTime).TotalMinutes > mTerminal.ReaderByID(1).APBControlledMinutes OrElse
                                            Now.Subtract(dLastAccessMoveDateTime).TotalSeconds < mTerminal.ReaderByID(1).APBCourtesyTime
                    Else
                        bAllowAccess = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalPunchHelper::CheckAPBIfNeeded::Error:" & oPunchState.ErrorDetail)
                    End If

                End If
            Catch ex As Exception
                bAllowAccess = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalPunchHelper::CheckAPBIfNeeded::Error checking APB. Access will be denied:", ex)
            End Try

            Return bAllowAccess
        End Function

    End Class

    Public Class roParametersHelper

        Public Enum eCardCode
            None = -1
            Hex = 0
            Numeric = 1
            Robotics = 2
        End Enum

        Public Enum eCardType
            None = -1
            Unique = 1
            Mifare = 2
            HID = 3
        End Enum

        Public Shared ReadOnly Property AllowProductiv(oState As roTerminalsState) As Boolean
            Get
                Dim oLicense As New roServerLicense
                Return oLicense.FeatureIsInstalled("Feature\Productiv")
            End Get
        End Property

        Public Shared ReadOnly Property PunchPeriodRTIn(oState As roTerminalsState) As Integer
            Get
                Return roTypes.Any2Integer(DataLayer.roCacheManager.GetInstance().GetParametersCache(Azure.RoAzureSupport.GetCompanyName, Parameters.PunchPeriodRTIn))
            End Get
        End Property

        Public Shared ReadOnly Property PunchPeriodRTOut(oState As roTerminalsState) As Integer
            Get
                Return roTypes.Any2Integer(DataLayer.roCacheManager.GetInstance().GetParametersCache(Azure.RoAzureSupport.GetCompanyName, Parameters.PunchPeriodRTOut))
            End Get
        End Property

        Public Shared ReadOnly Property MovMaxHours(ostate As roTerminalsState) As Integer
            Get
                Return roTypes.Any2Integer(DataLayer.roCacheManager.GetInstance().GetParametersCache(Azure.RoAzureSupport.GetCompanyName, Parameters.MovMaxHours))
            End Get
        End Property

        Public Shared ReadOnly Property CardType() As eCardType
            Get
                Dim sCardType As String = String.Empty
                Dim oRet As eCardType = eCardType.Unique

                ' Tipo de tarjeta
                sCardType = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "Terminals.CardReaderType")
                Select Case sCardType.ToLower.Trim
                    Case "1", "unique"
                        oRet = eCardType.Unique
                    Case "2", "mifare"
                        oRet = eCardType.Mifare
                    Case "3", "hid"
                        oRet = eCardType.HID
                End Select

                Return oRet
            End Get
        End Property

        Public Shared ReadOnly Property CardCode() As eCardCode
            Get
                Dim sCardCode As String
                Dim oRet As eCardCode = eCardCode.Robotics
                Try
                    sCardCode = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "Terminals.CardCodification")

                    Select Case sCardCode.ToLower.Trim
                        Case "0", "hex"
                            oRet = eCardCode.Hex
                        Case "1", "numeric"
                            oRet = eCardCode.Numeric
                        Case "2", "robotics"
                            oRet = eCardCode.Robotics
                    End Select
                Catch ex As Exception
                    oRet = eCardCode.Robotics
                End Try
                Return oRet
            End Get
        End Property

    End Class

    Public Class roTerminalsHelper
        Public Const NULLDATE = "1900-01-01 00:00:00"
        Private oBroadcasterThread As Threading.Thread

        Public Shared Function TerminalPunchToString(oTerminalPunch As roTerminalPunch, iIDTerminal As Integer) As String
            Dim lRet As String = String.Empty
            Try
                lRet = "IDEmployee=" & oTerminalPunch.IDEmployee.ToString & " Action=" & oTerminalPunch.Action & " DateTime=" & oTerminalPunch.PunchDateTime.ToString & " IDTerminal=" & iIDTerminal.ToString & " Credential=" & roTypes.Any2String(oTerminalPunch.Credential) & " Method=" & oTerminalPunch.Method.ToString
            Catch ex As Exception
                lRet = "Error parsing punch data to show it in log"
            End Try
            Return lRet
        End Function

        Public Shared Sub CallBroadcaster(oState As roTerminalsState, Optional ByVal IDTerminal As Integer = 0, Optional ByVal Task As roTerminalsSyncTasks.SyncActions = roTerminalsSyncTasks.SyncActions.none, Optional ByVal IDEmployee As Integer = 0, Optional ByVal OnlyTask As Boolean = True, Optional ByVal IDFinger As Integer = 0)
            Try
                Dim roConnector As New roConnector()
                Dim oCollection As New roCollection
                If IDTerminal > 0 Then
                    oCollection.Add("Command", "ON_ADD_TASK")
                    oCollection.Add("IDTerminal", IDTerminal.ToString)
                    oCollection.Add("TerminalsTask", Task.ToString)
                    oCollection.Add("IDEmployees", IDEmployee.ToString)
                    oCollection.Add("OnlyTask", OnlyTask.ToString)
                    oCollection.Add("IDFinger", IDFinger.ToString)
                End If
                roConnector.InitTask(TasksType.BROADCASTER, oCollection)
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "mdPublic::CallBroadcaster::Terminal " + IDTerminal.ToString + "::Call broadcaster(" + Task.ToString + "," + IDEmployee.ToString + "," + OnlyTask.ToString + ")")
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::CallBroadcaster::Terminal " + IDTerminal.ToString + "::Error::", ex)
            End Try
        End Sub

        Public Shared Sub CreateUserTask(UID As String, sType As String, sLocation As String, oState As roTerminalsState)
            Dim oUsrState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\TERMINAL_NOTREGISTERED" & UID, oUsrState)
            If oTaskExist.Message = "" Then
                Dim oTask As New VTBusiness.UserTask.roUserTask()
                With oTask
                    .ID = "USERTASK:\\TERMINAL_NOTREGISTERED" & UID
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(UID) : oState.Language.AddUserToken(sType) : oState.Language.AddUserToken(sLocation)
                    Dim arrList As New ArrayList
                    .Message = oState.Language.Translate("TerminalNotRegistered.Title", "")
                    oState.Language.ClearUserTokens()
                    .DateCreated = Now
                    .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                    .ResolverURL = "FN:\\TERMINAL_NotRegistered"
                    .ResolverVariable1 = "UID" : .ResolverValue1 = UID
                    .ResolverVariable2 = "Type" : .ResolverValue2 = sType
                    .ResolverVariable3 = "Location" : .ResolverValue3 = sLocation
                    .Save()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::CreateUserTask: Terminal " & sType & " with UID" & UID & " at " & sLocation & ":User task created.")
                End With
            End If
        End Sub

        Public Shared Sub DelUserTask(UID As String, oState As roTerminalsState)
            Dim oUsrState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\TERMINAL_NOTREGISTERED" & UID, oUsrState)

            'Si existe la tarea la borramos
            If oTaskExist.Message <> "" Then
                oTaskExist.Delete()
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::DelUserTask: Terminal with UID" & UID & ": User task deleted because the terminal is already registered")
            End If
        End Sub

        Public Shared Sub CreateUserTaskGeneric(sTaskID As String, sLangTag As String, iIDTerminal As Integer, oState As roTerminalsState)
            Dim oUsrTaskState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & iIDTerminal.ToString, oUsrTaskState)
            If oTaskExist.Message = "" Then
                Dim oTask As New VTBusiness.UserTask.roUserTask()
                With oTask
                    .ID = sTaskID & iIDTerminal.ToString
                    .Message = sLangTag + "¬" + iIDTerminal.ToString
                    .DateCreated = Now
                    .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                    .ResolverURL = ""
                    .Save()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::CreateUserTaskGeneric:" & iIDTerminal.ToString & ":User task created.")
                End With
            End If
        End Sub

        Public Shared Sub DelUserTaskGeneric(sTaskID As String, iIDTerminal As Integer, oState As roTerminalsState)
            Try
                Dim oUsrTaskState As New VTBusiness.UserTask.roUserTaskState()
                Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & iIDTerminal.ToString, oUsrTaskState)
                'Si existe la tarea la borramos
                If oTaskExist.Message <> "" Then
                    oTaskExist.Delete()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::DelUserTaskGeneric:" & iIDTerminal.ToString & ":User task deleted because the terminal registered yet")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalsHelper::DelUserTask:Terminal " + iIDTerminal.ToString + " :Error:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Convierte un código de tarjeta guardado en VT para enviarlo al terminal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        Public Shared Function ConvertCardForTerminal(_VTCard As String, oState As roTerminalsState) As String
            Try
                Select Case roParametersHelper.CardType
                    Case roParametersHelper.eCardType.HID
                        ' Tarjeta HID. En BBDD se guarda como un número octal, incluyendo bits de paridad ...
                        Return RemoveHIDParityBits(_VTCard, oState)
                    Case roParametersHelper.eCardType.Mifare
                        ' Tarjeta MiFare. En BBDD es guarda con codificación Robotics
                        Return roTerminalsHelper.DecodeRoboticsCard(_VTCard, oState, 16)
                    Case roParametersHelper.eCardType.Unique
                        Select Case roParametersHelper.CardCode
                            Case roParametersHelper.eCardCode.Robotics
                                ' OJO: Los terminales Android, a diferencia del resto de terminales de ZK con tarjeta Unique, por defecto leen 4 bytes y no 3, y se pueden configurar para que lean 5
                                ' Return roTerminalsHelper.DecodeRoboticsCard(_VTCard, oState, 12)
                                Return roTerminalsHelper.DecodeRoboticsCard(_VTCard, oState, 16)
                            Case roParametersHelper.eCardCode.Numeric
                                ' Tarjeta UNIQUE, sin codificación Robotics. Simplemente corto a 3 bytes (6 caracteres HEXA)
                                ' OJO: Los terminales Android, a diferencia del resto de terminales de ZK con tarjeta Unique, leen 4 bytes y no 3
                                'Return roTerminalsHelper.CutCard(_VTCard, 6, oState)
                                Return roTerminalsHelper.CutCard(_VTCard, 8, oState)
                            Case Else
                                ' Trato como si fuera Robotics
                                'Return roTerminalsHelper.DecodeRoboticsCard(_VTCard, oState, 12)
                                Return roTerminalsHelper.DecodeRoboticsCard(_VTCard, oState, 16)
                        End Select
                        ' Tarjeta UNIQUE, con codificación Robotics
                    Case Else
                        Return roTerminalsHelper.CutCard(_VTCard, 6, oState)
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::ConvertCardForTerminal::Error:(IDCard:" + _VTCard + "):", ex)
                Return _VTCard
            End Try

        End Function

        ''' <summary>
        ''' Convierte un código de tarjeta guardado en VT para enviarlo al terminal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertCardForVTDatabase(_VTCard As String, oState As roTerminalsState) As String
            Dim oRet As String = ""
            Try
                Select Case roParametersHelper.CardType
                    Case roParametersHelper.eCardType.HID
                        ' Tarjeta HID.
                        oRet = roTerminalsHelper.AddHIDParityBits(_VTCard, oState)
                    Case roParametersHelper.eCardType.Mifare
                        ' Tarjeta MiFare. En BBDD es guarda con codificación Robotics
                        oRet = roTerminalsHelper.EncodeRoboticsCard(_VTCard, oState)
                    Case roParametersHelper.eCardType.Unique
                        Select Case roParametersHelper.CardCode
                            Case roParametersHelper.eCardCode.Robotics
                                oRet = roTerminalsHelper.EncodeRoboticsCard(_VTCard, oState)
                            Case roParametersHelper.eCardCode.Numeric
                                oRet = _VTCard
                            Case Else
                                oRet = roTerminalsHelper.EncodeRoboticsCard(_VTCard, oState)
                        End Select
                    Case Else
                        oRet = _VTCard
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::ConvertCardForDatabase::Error:(IDCard:" + _VTCard + "):", ex)
                oRet = _VTCard
            End Try
            Return oRet
        End Function

        Public Shared Function RemoveHIDParityBits(ByVal _VTCard As String, oState As roTerminalsState) As String
            Try
                ' A partir de un número en octal, devuelve un código de tarjeta válido para programar terminales (número en decimal resultante de eliminar los bits de paridad y el bit 27)

                Dim iDecimalIDCard As Long
                Dim sBinaryIDCard As String
                Dim sBinaryIDCardWithoutParity As String

                ' Verifico si el número que me han pasado es octal. Si no, seguramente se trata de un código de tarjeta sin asignar
                Try
                    iDecimalIDCard = Convert.ToInt32(_VTCard, 8)
                Catch ex As Exception
                    ' No es octal. No es una tarjeta válida
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. It should be octal")
                    Return ""
                End Try

                ' Obtengo el binario
                sBinaryIDCard = Convert.ToString(iDecimalIDCard, 2)

                ' Elimino los bits 1, 26 y 27 de derecha a izquierda (No valido que sean correctos)
                If Len(sBinaryIDCard) <> 27 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. Should be 27 bits long")
                    Return ""
                Else
                    sBinaryIDCardWithoutParity = Mid$(sBinaryIDCard, 3, 24)
                    Return Convert.ToInt32(sBinaryIDCardWithoutParity, 2).ToString
                End If
                Return ""
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::ConvertCardForTerminal::Error:(IDCard:" + _VTCard + "):", ex)
                Return ""
            End Try
        End Function

        Public Shared Function AddHIDParityBits(ByVal _VTCard As String, oState As roTerminalsState) As String
            Try
                ' A partir de un número entregado por un lector HID (24 bits), genera el número Octal que se va a guardar en BBDD, que es el mismo
                ' que lee un lector HID estandar Wiegand 26 bits (los terminales suelen procesar la lectura y quitan los bits de paridad)

                Dim sBinaryIDCard As String
                Dim sBinaryIDCardWithoutParity As String
                Dim sOddParityBit As String
                Dim sEvenParityBit As String
                Dim iCount As Integer

                iCount = 0

                ' Paso a binario
                sBinaryIDCardWithoutParity = Convert.ToString(Convert.ToInt32(_VTCard), 2)
                ' Completo hasta 24 con 0 por la izquierda
                sBinaryIDCardWithoutParity = sBinaryIDCardWithoutParity.PadLeft(24, "0")

                ' Añado el bit de paridad impar por la derecha
                ' - Cuento el número de veces que aparece el 1 en los doce dígitos de la derecha (menos significativos)
                '   - Si sale un número par, el bit de paridad es 1
                '   - Si sale un número impar, el bit de paridad es 0

                If sBinaryIDCardWithoutParity.Substring(12, 12).Replace("0", "").Length Mod 2 = 0 Then
                    sOddParityBit = "1"
                Else
                    sOddParityBit = "0"
                End If

                ' Añado el bit de paridad par por la izquierda
                ' - Cuento el número de veces que aparece el 1 en los dígitos a la izquierda de los doce dígitos menos significativos
                '   - Si sale un número par, el bit de paridad es 0
                '   - Si sale un número imapr, el bit de paridad es 1
                If sBinaryIDCardWithoutParity.Substring(0, 12).Replace("0", "").Length Mod 2 = 0 Then
                    sEvenParityBit = "0"
                Else
                    sEvenParityBit = "1"
                End If

                ' Añado el bit fijo "1" por la izquierda
                sBinaryIDCard = "1" & sEvenParityBit + sBinaryIDCardWithoutParity + sOddParityBit

                ' Valido que el número en binario tiene 27 dígitos
                If sBinaryIDCard.Length <> 27 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. Should be 27 bits long")
                    Return ""
                End If

                Return Convert.ToString(Convert.ToInt32(sBinaryIDCard, 2), 8)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::AddHIDParityBits::Error:(IDCard:" + _VTCard + "):" & ex.Message)
                Return ""
            End Try
        End Function

        Public Shared Function DecodeRoboticsCard(ByVal IDCard As String, oState As roTerminalsState, Optional ByVal MaxLen As Byte = 16) As Long
            Dim sIDCard As String = ""
            Try
                Dim stmp As String

                If IDCard.Trim <> "" Then
                    If IDCard.Length > MaxLen Then
                        stmp = Right(IDCard, MaxLen)
                    Else
                        stmp = IDCard
                    End If
                    While stmp.Length >= 2
                        If Convert.ToString(Integer.Parse(Right(stmp, 2)), 16).Length > 1 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "mdPublic::DecodeRoboticsCard::Warning: IDCard " + IDCard + " is invalid.")
                            Return 0
                        End If
                        sIDCard = Convert.ToString(Integer.Parse(Right(stmp, 2)), 16) + sIDCard
                        stmp = stmp.Substring(0, stmp.Length - 2)
                    End While
                    If stmp.Length > 0 Then sIDCard = Convert.ToString(Integer.Parse(stmp), 16) + sIDCard
                    sIDCard = Convert.ToInt64(sIDCard, 16).ToString
                End If

                Return roTypes.Any2Long(sIDCard)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::DecodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
                Return 0
            End Try
        End Function

        Public Shared Function EncodeRoboticsCard(ByVal IDCard As String, oState As roTerminalsState) As Long
            Dim sIDCard As String = ""
            Dim tmp As String = ""
            Try

                tmp = Convert.ToString(Long.Parse(roConversions.Any2Double(IDCard)), 16)
                While tmp.Length > 0
                    sIDCard += IIf(Convert.ToInt16(tmp.Substring(0, 1), 16) > 9, "", "0") + Convert.ToInt16(tmp.Substring(0, 1), 16).ToString
                    tmp = tmp.Substring(1)
                End While
                Return roTypes.Any2Long(sIDCard)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::EncodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
                Return 0
            End Try

        End Function

        Public Shared Function CutCard(ByVal Card As String, ByVal ByteLenght As Byte, oState As roTerminalsState) As Long
            Try
                Dim tmp As String = ""
                'Convertimos a hex
                tmp = Convert.ToString(Long.Parse(roConversions.Any2Double(Card)), 16)
                If tmp.Length > ByteLenght Then
                    'Cortamos los bytes sobrantes
                    tmp = tmp.Substring(tmp.Length - ByteLenght)
                    'Devolvemos el numero cortado
                    Return Convert.ToInt32(tmp, 16)
                Else
                    Return Long.Parse(Card)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "mdPublic::CutCard::Error:(IDCard:" + Card + "):", ex)
                Return 0
            End Try
        End Function

        Public Shared Function resizeImage(ByVal Img As Drawing.Image, ByVal Width As Integer, ByVal Height As Integer) As Byte()
            Try
                Dim ms As IO.MemoryStream = New IO.MemoryStream
                Dim thumb As New Drawing.Bitmap(Width, Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(thumb)
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                g.DrawImage(Img, New Drawing.Rectangle(0, 0, Width, Height), New Drawing.Rectangle(0, 0, Img.Width, Img.Height), Drawing.GraphicsUnit.Pixel)
                g.Dispose()
                thumb.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
                Img.Dispose()
                thumb.Dispose()

                Return ms.ToArray
            Catch ex As Exception
                Return Array.CreateInstance(GetType(Byte), 0)
            End Try
        End Function

        Public Shared Function ForceFullTerminalSync(iIDTerminal As Integer, oState As roTerminalsState) As Boolean
            Dim bRet As Boolean = False
            Try

                Dim sSQL As String = "@DELETE# from [dbo].[TerminalsSyncBiometricData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncCardsData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncEmployeesData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncConfigData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncSirensData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncCausesData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncAccessData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncGroupsData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncDocumentsData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                sSQL = "@DELETE# from [dbo].[TerminalsSyncTimeZonesData] where terminalid = " & iIDTerminal
                bRet = Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL)

                roTerminalsHelper.CallBroadcaster(oState, iIDTerminal)
                bRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalsHelper::ForceFullTerminalSync:Terminal " & iIDTerminal & ":Unable to delete employee files and force Broadcaster")
            End Try
            Return bRet
        End Function

        Public Shared Sub PrintTicketOnFileOnAzure(oState As roTerminalsState, folder As String, Optional ByVal hasCustomDesign As Boolean = False, Optional oDinerPunch As roTerminalPunch = Nothing, Optional printerText As String = Nothing)
            Try
                Dim sLine As String = String.Empty
                Dim sLineLog As String = String.Empty
                Dim iIdEmployee As Integer = 0
                Dim sEmployeeName As String = String.Empty
                Dim sPunchDate As String = String.Empty
                Dim sPunchTime As String = String.Empty
                Dim sTurnName As String = String.Empty
                Dim sGroupName As String = String.Empty
                Dim oEmployee As Employee.roEmployee = Nothing

                If oDinerPunch IsNot Nothing Then
                    sPunchDate = oDinerPunch.PunchDateTime.ToShortDateString
                    sPunchTime = oDinerPunch.PunchDateTime.ToShortTimeString
                    iIdEmployee = oDinerPunch.IDEmployee
                    sTurnName = New DiningRoom.roDiningRoomTurn(oDinerPunch.PunchData.DinnerData.IdTurn, New DiningRoom.roDiningRoomState()).Name
                    oEmployee = Employee.roEmployee.GetEmployee(oDinerPunch.IDEmployee, New Employee.roEmployeeState)
                End If

                If oEmployee IsNot Nothing Then
                    sEmployeeName = oEmployee.Name
                    sGroupName = oEmployee.GroupName
                End If

                Dim ms As New IO.MemoryStream
                Dim sw As New IO.StreamWriter(ms)

                If String.IsNullOrWhiteSpace(printerText) Then
                    sLine = "{$24}" & sEmployeeName
                    sLineLog = sLineLog & " - " & sLine
                    sw.WriteLine(sLine)
                    sw.Flush()
                    If hasCustomDesign Then
                        sLine = "{$16} "
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                    End If
                    sLine = "{$25}" & sPunchDate & " " & sPunchTime
                    sLineLog = sLineLog & " - " & sLine
                    sw.WriteLine(sLine)
                    sw.Flush()
                    If hasCustomDesign Then
                        sLine = "{$16} "
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                    End If
                    sLine = "{$20}" & sTurnName
                    sLineLog = sLineLog & " - " & sLine
                    sw.WriteLine(sLine)
                    sw.Flush()
                    sLine = ""
                    If sLine.Length > 0 Then
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                    End If
                    sLine = ""
                    If sLine.Length > 0 Then
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                    End If
                    If hasCustomDesign Then
                        sLine = "{$16} "
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                        sLine = "{$16} "
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                        sLine = "{$16}-- "
                        sLineLog = sLineLog & " - " & sLine
                        sw.WriteLine(sLine)
                        sw.Flush()
                    End If
                Else
                    printerText = Replace(printerText, "{GroupName}", sGroupName)
                    printerText = Replace(printerText, "{NameEmployee}", sEmployeeName)
                    printerText = Replace(printerText, "{IdEmployee}", iIdEmployee)
                    printerText = Replace(printerText, "{TurnName}", sTurnName)
                    printerText = Replace(printerText, "{PunchDate}", sPunchDate)
                    printerText = Replace(printerText, "{PunchTime}", sPunchTime)

                    'Obtener valor de los campos de la ficha
                    Dim pattern As String = "\{USR_[^}]+\}"
                    Dim regex As New System.Text.RegularExpressions.Regex(pattern)
                    Dim matches As MatchCollection = regex.Matches(printerText)
                    Dim oPassport As roPassport = roPassportManager.GetPassport(iIdEmployee, LoadType.Employee)
                    For Each match As Match In matches
                        Dim resultString = Replace(Replace(match.Value, "{USR_", ""), "}", "")
                        Dim field As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(iIdEmployee, resultString, DateAndTime.Now, New VTUserFields.UserFields.roUserFieldState(oPassport.ID), False)
                        If field IsNot Nothing Then
                            printerText = Replace(printerText, match.Value, field.FieldValue)
                        End If
                    Next
                    sw.WriteLine(printerText)
                    sw.Flush()
                End If

                ms.Position = 0
                ' Escribo blob
                If Not Azure.RoAzureSupport.SaveFileOnCompanyContainer(ms, "DinnerPunchInfo_" & Now.ToString("yyyyMMdd-HHmmss") & ".txt", folder, roLiveQueueTypes.dinner, True) Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::PrintTicketOnFileOnAzure: Unable to save ticket !!")
                End If

                sw.Close()
                sw.Dispose()

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsHelper::PrintTicketOnFileOnAzure:Save line (" + sLineLog + ")")
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalsHelper::PrintTicketOnFileOnAzure:Error:", ex)
            End Try
        End Sub

        Public Shared Function GetVersionCode(ByVal _Version As String) As Integer
            Dim retVersionCode As Integer = 20005

            Try
                Dim v1, v2, v3 As Integer
                v1 = roTypes.Any2Integer(_Version.Split(".")(0))
                v2 = roTypes.Any2Integer(_Version.Split(".")(1))
                v3 = roTypes.Any2Integer(_Version.Split(".")(2))
                'Construyo una cadena con el código versión, formado por la concatenación de los tres números de versión, cada uno completado a dos posiciones con ceros por la izquierda
                Dim sVersion As String = String.Format("{0:00}{1:00}{2:00}", v1, v2, v3)

                retVersionCode = roTypes.Any2Integer(sVersion)
            Catch ex As Exception
                ' Version 2.0.5
                retVersionCode = 20005
            End Try

            Return retVersionCode
        End Function

    End Class

    Public Class roTerminalTextHelper

        Public Shared Function Minutes2StringLong(ByVal intMinutes As Integer, oState As roTerminalsState) As String
            Dim strRet As String = ""
            Try

                If (intMinutes \ 60) > 0 Then strRet += (intMinutes \ 60).ToString + " hrs "
                If (intMinutes Mod 60) > 0 Then strRet += (intMinutes Mod 60).ToString + " min"
                If strRet = "" Then
                    strRet = "un instante"
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTextHelper::Minutes2StringLong::Error:", ex)
            End Try
            Return strRet
        End Function

        Public Shared Function Minutes2StringLong(ByVal Value As DateTime, oState As roTerminalsState) As String
            Dim strRet As String = ""
            Try

                strRet += Value.Hour.ToString + " hrs "
                strRet += Value.Minute.ToString + " min"
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTextHelper::Minutes2StringLong::Error:", ex)
            End Try
            Return strRet
        End Function

        Public Shared Function Minutes2StringShort(ByVal intMinutes As Integer, oState As roTerminalsState) As String
            Try

                If intMinutes > 600 Then
                    Return (intMinutes \ 60).ToString & ":" & Format(intMinutes Mod 60, "00")
                Else
                    Return Format(intMinutes \ 60, "00") & ":" & Format(intMinutes Mod 60, "00")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTextHelper::Minutes2StringShort::Error:", ex)
                Return ""
            End Try
        End Function

        Public Shared Function MonthName(ByVal intMonth As Integer, oState As roTerminalsState) As String
            Dim strRet As String = ""
            Try

                Select Case intMonth
                    Case 1 : strRet = "January"
                    Case 2 : strRet = "February"
                    Case 3 : strRet = "March"
                    Case 4 : strRet = "April"
                    Case 5 : strRet = "May"
                    Case 6 : strRet = "June"
                    Case 7 : strRet = "July"
                    Case 8 : strRet = "August"
                    Case 9 : strRet = "September"
                    Case 10 : strRet = "October"
                    Case 11 : strRet = "November"
                    Case 12 : strRet = "December"
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTextHelper::MonthName::Error:", ex)
            End Try
            Return strRet
        End Function

        ''' <summary>
        ''' Sustituye carácteres por sus equivalentes sin diacríticos (acentos, ...). Ojo: cambia ñ por n y ç por c
        ''' </summary>
        ''' <param name="strIn"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Normalize(strIn As String) As String
            Dim reg As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9 * . \xF1 \xD1]")
            Return reg.Replace(strIn.Normalize(System.Text.NormalizationForm.FormD), "")
        End Function

        Public Shared Function Translate(ByVal strKey As String, Optional ByVal oParamList As ArrayList = Nothing, Optional ByVal strFileReference As String = "TerminalsManager", Optional ByVal strLanguageKey As String = "ESP", Optional bInvalidateCache As Boolean = False) As String
            Static oLanguage As Robotics.VTBase.roLanguage = Nothing

            If oLanguage Is Nothing OrElse bInvalidateCache Then
                oLanguage = New Robotics.VTBase.roLanguage
            End If

            oLanguage.ClearUserTokens()
            If oParamList IsNot Nothing Then
                For i As Integer = 0 To oParamList.Count - 1
                    oLanguage.AddUserToken(oParamList(i))
                Next
            End If

            oLanguage.SetLanguageReference(strFileReference, strLanguageKey)
            Return oLanguage.Translate(strKey, "")
        End Function

    End Class

#End Region

End Namespace