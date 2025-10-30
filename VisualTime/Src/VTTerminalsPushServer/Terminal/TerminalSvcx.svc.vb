Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Hosting
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
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

            If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
                Select Case WebApiApplication.TerminalResult
                    Case TerminalBaseResultEnum.InvalidRegistrationCode
                        oState.Result = roTerminalsState.ResultEnum.InvalidRegistrationCode
                    Case TerminalBaseResultEnum.SecurityTokenNotValid
                        oState.Result = roTerminalsState.ResultEnum.SecurityTokenNotValid
                    Case TerminalBaseResultEnum.TerminalDoesNotExists
                        oState.Result = roTerminalsState.ResultEnum.TerminalDoesNotExists
                        ' Creamos la tarea de registro
                        Dim oTerminalManager As roTerminalsManager = New roTerminalsManager(oState)
                        oTerminalManager.CreateUnregisteredAlert(strAuthToken, WebApiApplication.ApplicationSource.ToString, oState.ClientAddress.Split("#")(0))
                    Case TerminalBaseResultEnum.ServerStopped
                        oState.Result = roTerminalsState.ResultEnum.ServerStopped
                    Case Else
                        oState.Result = roTerminalsState.ResultEnum.Exception
                End Select
            Else
                'TODO: Borrado de tarea de terminal sin registrar, por si desde pantalla falla #84729. Estudiar antes si puede tener afectación en rendimiento.
                Dim strConfig As String = HttpContext.Current.Request.Params("Config")
                Dim oConfig() As roTerminalSyncParameter = {}
                If strConfig IsNot Nothing AndAlso strConfig <> String.Empty Then oConfig = roJSONHelper.DeserializeNewtonSoft(strConfig, GetType(roTerminalSyncParameter()))

                idTerminal = WebApiApplication.TerminalIdentity.ID
                lrret.SecurityToken = Robotics.VTBase.HashCheckSum.CalculateString(WebApiApplication.TerminalSecurityToken, Algorithm.MD5)
                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
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

            If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
                Select Case WebApiApplication.TerminalResult
                    Case TerminalBaseResultEnum.InvalidRegistrationCode
                        oState.Result = roTerminalsState.ResultEnum.InvalidRegistrationCode
                    Case TerminalBaseResultEnum.SecurityTokenNotValid
                        oState.Result = roTerminalsState.ResultEnum.SecurityTokenNotValid
                    Case TerminalBaseResultEnum.TerminalDoesNotExists
                        oState.Result = roTerminalsState.ResultEnum.TerminalDoesNotExists
                        ' Creamos la tarea de registro
                        Dim oTerminalManager As roTerminalsManager = New roTerminalsManager(oState)
                        oTerminalManager.CreateUnregisteredAlert(strAuthToken, WebApiApplication.ApplicationSource.ToString, oState.ClientAddress.Split("#")(0))
                    Case TerminalBaseResultEnum.ServerStopped
                        oState.Result = roTerminalsState.ResultEnum.ServerStopped
                    Case Else
                        oState.Result = roTerminalsState.ResultEnum.Exception
                End Select
            Else
                idTerminal = WebApiApplication.TerminalIdentity.ID
                lrret.SecurityToken = Robotics.VTBase.HashCheckSum.CalculateString(WebApiApplication.TerminalSecurityToken, Algorithm.MD5)
                Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
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

        If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case WebApiApplication.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not WebApiApplication.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
                    lrret = oTerminalManager.GetTask()
                Catch ex As Exception
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::GetTasks")
                End Try
            End If
        End If

        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(WebApiApplication.LoggedIn, WebApiApplication.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetTasks() As TerminalStdResponse
        Dim lrret As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case WebApiApplication.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not WebApiApplication.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim setTasksText As String = HttpContext.Current.Request.Params("oSyncData")

                    Dim oTasks As roTerminalSyncData = New roTerminalSyncData
                    oTasks = roJSONHelper.DeserializeNewtonSoft(setTasksText, oTasks.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
                    lrret = oTerminalManager.SetTasks(WebApiApplication.TerminalIdentity.ID, oTasks)
                Catch ex As Exception
                    lrret.Result = False
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SetTasks")
                End Try
            End If
        End If
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(WebApiApplication.LoggedIn, WebApiApplication.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function TasksResult() As TerminalStdResponse
        Dim lrret As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case WebApiApplication.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not WebApiApplication.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    Dim strParamText As String = HttpContext.Current.Request.Params("tasksResult")

                    Dim oTasksResults As roTasksResult() = {}
                    oTasksResults = roJSONHelper.DeserializeNewtonSoft(strParamText, oTasksResults.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
                    lrret = oTerminalManager.ProcessTasksResult(WebApiApplication.TerminalIdentity.ID, oTasksResults)
                Catch ex As Exception
                    lrret.Result = False
                    oState.Result = roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::TasksResult")
                End Try
            End If
        End If

        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(WebApiApplication.LoggedIn, WebApiApplication.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SavePunch() As TerminalStdResponse
        Dim retSavePunch As New TerminalStdResponse
        Dim oState = New roTerminalsState(-1)

        If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case WebApiApplication.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not WebApiApplication.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    oState.Result = roTerminalsState.ResultEnum.ErrorSavingPunch

                    Dim strParamText As String = HttpContext.Current.Request.Params("oPunch")

                    Dim oPunch As New roTerminalPunch()
                    oPunch = roJSONHelper.DeserializeNewtonSoft(strParamText, oPunch.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)

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
        roWsStateManager.CopyTo(oState, newGState, If(WebApiApplication.LoggedIn, WebApiApplication.TerminalIdentity.ID, -1))
        retSavePunch.Status = newGState

        Return retSavePunch
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ProcessPunch() As roTerminalInteractivePunch
        Dim retInteractivePunch As New roTerminalInteractivePunch
        Dim oState = New roTerminalsState(-1)

        If WebApiApplication.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case WebApiApplication.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not WebApiApplication.LoggedIn Then
                oState.Result = roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    oState.Result = roTerminalsState.ResultEnum.ErrorProcessingPunch

                    Dim strParamText As String = HttpContext.Current.Request.Params("oInteractivePunch")

                    Dim oCurrentPunch As New roTerminalInteractivePunch
                    oCurrentPunch = roJSONHelper.DeserializeNewtonSoft(strParamText, oCurrentPunch.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)

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
                        oTerminalManager = New VTTerminals.roTerminalsManager(oState, WebApiApplication.TerminalIdentity)
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
        roWsStateManager.CopyTo(oState, newGState, If(WebApiApplication.LoggedIn, WebApiApplication.TerminalIdentity.ID, -1))
        retInteractivePunch.Status = newGState

        Return retInteractivePunch
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function bugreport() As TerminalConfig
        Dim oState = New roTerminalsState(-1)
        Dim lrret As New TerminalConfig
        Dim idTerminal As Integer = -1

        Try
            'do nothing. Deprecated
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