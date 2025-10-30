Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Hosting
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTPortal.VTPortal
Imports Robotics.Base.VTTerminals
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class TerminalSvcx

#Region "Webservice Constants"

    Private Shared SERVER_VERSION = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

    Private cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

#End Region

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function KeepAlive() As Boolean
        Return True
    End Function

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza el inicio del inicio de sesión del empleado especificado en los parametros en el sistema de VisualTime
    ''' </summary>
    ''' <param name="usr">Nombre de usuario del empleado</param>
    ''' <param name="pwd">Contraseña del empleado</param>
    ''' <param name="language">Idioma de inicio de sesión. Valores permitiods (ESP/CAT/ENG/POR)</param>
    ''' <param name="appVersion">Versión de la aplicación para iniciar el protocolo de comunicaciones</param>
    ''' <param name="validationCode">Código de validación subministrado por el empleado para iniciar sesión si se ha configurado visualtime para requerir este nivel de validación.</param>
    ''' <param name="timeZone">Código OLSON que describe la zona horaria desde donde se realiza la conexión del portal.</param>
    ''' <param name="accessFromApp">Incidca si el usuario esta realizando login mediante una aplicación.</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Initialize() As TerminalConfig
        Dim oState = New roTerminalsState(-1)
        Dim lrret As New TerminalConfig
        Dim idTerminal As Integer = -1

        Try
            roBaseState.SetSessionSmall(oState, -1, roAppSource.mx9, "")
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
                Select Case Global_asax.TerminalResult
                    Case TerminalBaseResultEnum.InvalidRegistrationCode
                        oState.Result = roTerminalsState.ResultEnum.InvalidRegistrationCode
                    Case TerminalBaseResultEnum.SecurityTokenNotValid
                        oState.Result = roTerminalsState.ResultEnum.SecurityTokenNotValid
                    Case TerminalBaseResultEnum.TerminalDoesNotExists
                        oState.Result = roTerminalsState.ResultEnum.TerminalDoesNotExists
                        ' Creamos la tarea de registro
                        Dim oTerminalManager As roTerminalsManager = New roTerminalsManager(oState)
                        oTerminalManager.CreateUnregisteredAlert(strAuthToken, Global_asax.ApplicationSource.ToString, oState.ClientAddress.Split("#")(0))
                    Case TerminalBaseResultEnum.ServerStopped
                        oState.Result = roTerminalsState.ResultEnum.ServerStopped
                    Case Else
                        oState.Result = roTerminalsState.ResultEnum.Exception
                End Select
            Else
                'TODO: Borrado de tarea de terminal sin registrar, por si desde pantalla falla #84729. Estudiar antes si puede tener afectación en rendimiento.
                Dim strConfig As String = HttpContext.Current.Request.Params("Config")
                Dim oConfig() As roTerminalSyncParameter = {}
                If strConfig IsNot Nothing AndAlso strConfig <> String.Empty Then oConfig = JSONHelper.DeserializeNewtonSoft(strConfig, GetType(roTerminalSyncParameter()))

                idTerminal = Global_asax.TerminalIdentity.ID
                lrret.SecurityToken = Robotics.VTBase.HashCheckSum.CalculateString(Global_asax.TerminalSecurityToken, Algorithm.MD5)
                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/BackgroundPlus.png")

                If oTerminalManager.Init(oConfig, fileName) Then
                    lrret.Config = oTerminalManager.GetConfig(-1, fileName)
                    oState.Result = roTerminalsState.ResultEnum.NoError
                End If
            End If
        Catch ex As Exception
            oState.Result = roTerminalsState.ResultEnum.Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::Login")
        End Try

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, idTerminal)
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Init(ByVal timestamp As String) As TerminalConfig
        Dim oState = New roTerminalsState(-1)
        Dim lrret As New TerminalConfig
        Dim idTerminal As Integer = -1

        Try
            roBaseState.SetSessionSmall(oState, -1, roAppSource.mx9, "")
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
                Select Case Global_asax.TerminalResult
                    Case TerminalBaseResultEnum.InvalidRegistrationCode
                        oState.Result = roTerminalsState.ResultEnum.InvalidRegistrationCode
                    Case TerminalBaseResultEnum.SecurityTokenNotValid
                        oState.Result = roTerminalsState.ResultEnum.SecurityTokenNotValid
                    Case TerminalBaseResultEnum.TerminalDoesNotExists
                        oState.Result = roTerminalsState.ResultEnum.TerminalDoesNotExists
                        ' Creamos la tarea de registro
                        Dim oTerminalManager As roTerminalsManager = New roTerminalsManager(oState)
                        oTerminalManager.CreateUnregisteredAlert(strAuthToken, Global_asax.ApplicationSource.ToString, oState.ClientAddress.Split("#")(0))
                    Case TerminalBaseResultEnum.ServerStopped
                        oState.Result = roTerminalsState.ResultEnum.ServerStopped
                    Case Else
                        oState.Result = roTerminalsState.ResultEnum.Exception
                End Select
            Else
                idTerminal = Global_asax.TerminalIdentity.ID
                lrret.SecurityToken = Robotics.VTBase.HashCheckSum.CalculateString(Global_asax.TerminalSecurityToken, Algorithm.MD5)
                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/backgroundDefault.png")

                If oTerminalManager.Init({}, fileName) Then
                    lrret.Config = oTerminalManager.GetConfig(-1, fileName)
                    oState.Result = roTerminalsState.ResultEnum.NoError
                End If
            End If
        Catch ex As Exception
            oState.Result = roTerminalsState.ResultEnum.Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::Login")
        End Try

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, idTerminal)
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTasks() As roTerminalSyncData
        Dim lrret As New roTerminalSyncData
        Dim oState = New roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                    lrret = oTerminalManager.GetTask()
                Catch ex As Exception
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::GetTasks")
                End Try
            End If
        End If

        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetTasks() As TerminalStdResponse
        Dim lrret As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim setTasksText As String = HttpContext.Current.Request.Params("oSyncData")

                    Dim oTasks As roTerminalSyncData = New roTerminalSyncData
                    oTasks = JSONHelper.DeserializeNewtonSoft(setTasksText, oTasks.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                    lrret = oTerminalManager.SetTasks(Global_asax.TerminalIdentity.ID, oTasks)
                Catch ex As Exception
                    lrret.Result = False
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SetTasks")
                End Try
            End If
        End If
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function TasksResult() As TerminalStdResponse
        Dim lrret As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim strParamText As String = HttpContext.Current.Request.Params("tasksResult")

                    Dim oTasksResults As roTasksResult() = {}
                    oTasksResults = JSONHelper.DeserializeNewtonSoft(strParamText, oTasksResults.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                    lrret = oTerminalManager.ProcessTasksResult(Global_asax.TerminalIdentity.ID, oTasksResults)
                Catch ex As Exception
                    lrret.Result = False
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::TasksResult")
                End Try
            End If
        End If

        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    '<OperationContract()>
    '<WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    'Public Function Identify(iID As Integer, sCredential As String, sPWD As String, Method As AuthenticationMethod, Action As TerminalAction, dPunchDateTime As Date, dLastAttPunchOnTerminalDateTime As Date, dLastAttPunchOnTerminalAction As EmployeeAttStatus) As roTerminalEmployeeStatus
    '    Dim retEmployeeStatus As New roTerminalEmployeeStatus
    '    Dim oState = New roTerminalsState(-1)

    '    If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
    '        Select Case Global_asax.TerminalResult
    '            Case TerminalBaseResultEnum.ServerStopped
    '                oState.Result = roTerminalsState.ResultEnum.ServerStopped
    '            Case Else
    '                oState.Result = roTerminalsState.ResultEnum.Exception
    '        End Select
    '    Else
    '        If Not Global_asax.LoggedIn Then
    '            oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
    '        Else
    '            Try
    '                oState.Result = roTerminalsState.ResultEnum.NoError
    '                Dim iIDEmployee As Integer = 0
    '                'cambio mi state genérico a un estado especifico

    '                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
    '                iIDEmployee = oTerminalManager.Identify(Global_asax.TerminalIdentity.ID, sCredential, sPWD, Method, oState)

    '                If iIDEmployee > 0 Then
    '                    'Recupero la información adicional a mostrar en el terminal en función de la acción seleccionada por el empleado antes de identificarse
    '                    retEmployeeStatus = oTerminalManager.GetEmployeeStatusOnTerminal(iIDEmployee, Action, dPunchDateTime, Global_asax.TerminalIdentity, oState, dLastAttPunchOnTerminalDateTime, dLastAttPunchOnTerminalAction)
    '                End If
    '            Catch ex As Exception
    '                oState.Result = roTerminalsState.ResultEnum.Exception
    '                Dim oLogState As New roBusinessState("Common.BaseState", "")
    '                oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::Identify")
    '            End Try
    '        End If
    '    End If

    '    'Crear el response genérico
    '    Dim newGState As New roWsTerminalState
    '    roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
    '    retEmployeeStatus.Status = newGState

    '    Return retEmployeeStatus
    'End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SavePunch() As TerminalStdResponse
        Dim retSavePunch As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch

                    Dim strParamText As String = HttpContext.Current.Request.Params("oPunch")

                    Dim oPunch As New roTerminalPunch()
                    oPunch = JSONHelper.DeserializeNewtonSoft(strParamText, oPunch.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)

                    Select Case oPunch.Command
                        Case PunchCommand.Add
                            If oTerminalManager.SavePunch(oPunch) Then oState.Result = roTerminalsState.ResultEnum.NoError
                        Case PunchCommand.Delete
                            If oTerminalManager.DeletePunch(oPunch) Then oState.Result = roTerminalsState.ResultEnum.NoError
                        Case PunchCommand.Update

                    End Select

                    ' Si se mostraron mensajes de empleado, los marco como leídos
                    If oState.Result = roTerminalsState.ResultEnum.NoError AndAlso Not oPunch.PunchData.ReadMessages Is Nothing AndAlso oPunch.PunchData.ReadMessages.Length > 0 Then
                        oTerminalManager.SetMessagesAsRead(oPunch.PunchData.ReadMessages)
                    End If
                Catch ex As Exception
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SavePunch")
                End Try
            End If
        End If

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        retSavePunch.Status = newGState

        Return retSavePunch
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ProcessPunch() As roTerminalInteractivePunch
        Dim retInteractivePunch As New roTerminalInteractivePunch
        Dim oState = New roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    oState.Result = roTerminalsState.ResultEnum.ErrorProcessingPunch

                    Dim strParamText As String = HttpContext.Current.Request.Params("oInteractivePunch")

                    Dim oCurrentPunch As New roTerminalInteractivePunch
                    oCurrentPunch = JSONHelper.DeserializeNewtonSoft(strParamText, oCurrentPunch.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)

                    ' Si el terminal no identificó al empleado, lo intento ahora ...
                    If oCurrentPunch.Punch.IDEmployee <= 0 Then
                        If oCurrentPunch.Punch.Method = AuthenticationMethod.Card Then
                            oCurrentPunch.Punch.Credential = Convert.ToInt64(oCurrentPunch.Punch.Credential, 16).ToString
                        End If

                        oCurrentPunch.Punch.IDEmployee = oTerminalManager.Identify(oCurrentPunch.Punch.Credential, oCurrentPunch.Punch.PIN, oCurrentPunch.Punch.Method, oState)
                    End If

                    If oCurrentPunch.Punch.IDEmployee <= 0 Then
                        oState.Result = roTerminalsState.ResultEnum.EmployeeNotIdentified
                        retInteractivePunch = oCurrentPunch
                        retInteractivePunch.EmployeeStatus.EmployeeName = ""
                        retInteractivePunch.EmployeeStatus.ServerDate = Now
                    Else
                        oState = New roTerminalsState(roPassportManager.GetPassportTicket(oCurrentPunch.Punch.IDEmployee, LoadType.Employee).ID)
                        oTerminalManager = New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
                        retInteractivePunch.Punch.IDEmployee = oCurrentPunch.Punch.IDEmployee
                        retInteractivePunch = oTerminalManager.ProcessPunch(oCurrentPunch)
                    End If
                Catch ex As Exception
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SavePunch")
                End Try
            End If
        End If

        ' Si algo falló, paso el texto para que se muestre por pantalla
        If oState.Result <> roTerminalsState.ResultEnum.NoError Then
            If oState.Result = roTerminalsState.ResultEnum.Exception Then
                retInteractivePunch.Display.WorkArea = oState.Language.Translate("ResultEnum.Exception", "")
            Else
                retInteractivePunch.Display.WorkArea = oState.ErrorText
            End If
        End If

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        retInteractivePunch.Status = newGState

        Return retInteractivePunch
    End Function

    ''' <summary>
    ''' TO BE CONTINUED -- NO BORRAR --
    ''' Gestión de fichajes desde servidor
    ''' </summary>
    ''' <returns></returns>
    '<OperationContract()>
    '<WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    'Private Function ProcessServerDrivenPunch() As roTerminalInteractivePunch
    '    Dim retInteractivePunch As New roTerminalInteractivePunch
    '    Dim oState = New roTerminalsState(-1)

    '    If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
    '        Select Case Global_asax.TerminalResult
    '            Case TerminalBaseResultEnum.ServerStopped
    '                oState.Result = roTerminalsState.ResultEnum.ServerStopped
    '            Case Else
    '                oState.Result = roTerminalsState.ResultEnum.Exception
    '        End Select
    '    Else
    '        If Not Global_asax.LoggedIn Then
    '            oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
    '        Else
    '            Try
    '                oState.Result = roTerminalsState.ResultEnum.NoError

    '                Dim strParamText As String = HttpContext.Current.Request.Params("oInteractivePunch")

    '                Dim oCurrentPunch As roTerminalPunch
    '                oCurrentPunch = Global_asax.CurrentPunch
    '                Dim oInteractivePunch As New roTerminalInteractivePunch
    '                oInteractivePunch = JSONHelper.DeserializeNewtonSoft(strParamText, oInteractivePunch.GetType())

    '                Dim iIDEmployee As Integer = 0
    '                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)

    '                ' Si el terminal no identificó al empleado, lo hago ahora ...
    '                iIDEmployee = oInteractivePunch.Punch.IDEmployee
    '                If iIDEmployee <= 0 Then
    '                    iIDEmployee = oTerminalManager.Identify(Global_asax.TerminalIdentity.ID, oCurrentPunch.Credential, oCurrentPunch.PIN, oCurrentPunch.Method, oState)
    '                End If

    '                If iIDEmployee > 0 AndAlso oInteractivePunch.Command = InteractivePunchCommand.Punch Then
    '                    'Recupero la información adicional a mostrar en el terminal en función de la acción seleccionada por el empleado antes de identificarse
    '                    oInteractivePunch.EmployeeStatus = VTTerminals.roTerminalsManager.GetEmployeeStatusOnTerminal(iIDEmployee, oInteractivePunch.Punch.Action, oInteractivePunch.Punch.PunchDateTime, Global_asax.TerminalIdentity, oState)
    '                End If

    '                retInteractivePunch = oTerminalManager.ProcessServerDrivenPunch(oInteractivePunch, oCurrentPunch)
    '                Global_asax.CurrentPunch = oCurrentPunch

    '            Catch ex As Exception
    '                oState.Result = roTerminalsState.ResultEnum.Exception
    '                Dim oLogState As New roBusinessState("Common.BaseState", "")
    '                oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SavePunch")
    '            End Try
    '        End If
    '    End If

    '    'Crear el response genérico
    '    Dim newGState As New roWsTerminalState
    '    roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
    '    retInteractivePunch.State = newGState

    '    Return retInteractivePunch
    'End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function bugreport() As TerminalConfig
        Dim oState = New roTerminalsState(-1)
        Dim lrret As New TerminalConfig
        Dim idTerminal As Integer = -1

        Try
            'do nothing deprecated
        Catch ex As Exception
            oState.Result = roTerminalsState.ResultEnum.Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::Login")
        End Try

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, idTerminal)
        lrret.Status = newGState

        Return lrret
    End Function

End Class