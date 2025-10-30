Imports System.Security.Principal
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class PortalSvcx

#Region "Webservice Constants"

    Private Shared SERVER_VERSION = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

    Private cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

#End Region

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
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function isAdfsActive() As StdServerVersion
        Dim oState As New roSecurityState(0, HttpContext.Current)
        Dim lrret As New StdServerVersion

        Try
            Dim oCn As Robotics.DataLayer.roBaseConnection = Robotics.DataLayer.roCacheManager.GetInstance.GetConnection
            If oCn Is Nothing OrElse Not oCn.IsInitialized() Then
                lrret.Status = ErrorCodes.GENERAL_ERROR
            Else

                Dim oAdfs As New AdvancedParameter.roAdvancedParameter("ADFSEnabled", New AdvancedParameter.roAdvancedParameterState())

                If (oAdfs.State.Result = AdvancedParameterResultEnum.NoError) Then
                    Dim oApiVersion As New AdvancedParameter.roAdvancedParameter("VTPortalApiVersion", New AdvancedParameter.roAdvancedParameterState())
                    Dim oDefaultVersion As New AdvancedParameter.roAdvancedParameter("VTPortal.DefaultVersion", New AdvancedParameter.roAdvancedParameterState())
                    Dim oParamSSOType As New AdvancedParameter.roAdvancedParameter("SSOType", New AdvancedParameter.roAdvancedParameterState())
                    Dim oVTPortalMixModeEnabled As New AdvancedParameter.roAdvancedParameter("VisualTime.SSO.VTPortalMixedAuthEnabled", New AdvancedParameter.roAdvancedParameterState())
                    Dim oSSOConigVersion As New AdvancedParameter.roAdvancedParameter("VisualTime.SSO.ConfigurationVersion", New AdvancedParameter.roAdvancedParameterState())

                    If oParamSSOType.Value = "SSO" Then
                        Try
                            Dim user As WindowsPrincipal = CType(HttpContext.Current.User, WindowsPrincipal)
                            lrret.SSOServerEnabled = True
                            lrret.Result = False
                            lrret.SSOmixedModeEnabled = False
                            lrret.SSOUserLoggedIn = user.Identity.IsAuthenticated
                            lrret.SSOUserName = user.Identity.Name
                            lrret.SSOVersion = 1
                        Catch ex As Exception
                            lrret.SSOServerEnabled = False
                            lrret.Result = False
                            lrret.SSOmixedModeEnabled = False
                            lrret.SSOUserLoggedIn = False
                            lrret.SSOUserName = ""
                            lrret.SSOVersion = 0
                        End Try
                    Else
                        lrret.SSOServerEnabled = False
                        lrret.SSOUserLoggedIn = False
                        lrret.SSOUserName = ""
                        lrret.SSOmixedModeEnabled = roTypes.Any2Boolean(oVTPortalMixModeEnabled.Value)
                        lrret.Result = roTypes.Any2Boolean(oAdfs.Value)
                        lrret.SSOVersion = roTypes.Any2Integer(oSSOConigVersion.Value)
                    End If

                    lrret.ApiVersion = roTypes.Any2Integer(oApiVersion.Value)
                    lrret.DefaultVersion = roTypes.Any2String(oDefaultVersion.Value)

                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::isAdfsActive")
        End Try

        Return lrret
    End Function

End Class