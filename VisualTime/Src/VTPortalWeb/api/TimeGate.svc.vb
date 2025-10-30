Imports System.Drawing
Imports System.IO
Imports System.Security.Principal
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Hosting
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTPortal
Imports Robotics.Base.VTPortal.VTPortal
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTServiceApi
Imports Robotics.Base.VTSurveys
Imports Robotics.Base.VTTerminals
Imports Robotics.Base.VTToDoLists
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports VTLiveApi

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<CustomErrorBehavior>
Public Class TimeGate

#Region "Webservice Constants"

    Private Shared SERVER_VERSION = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

    Private cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

#End Region

    ''' <summary>
    ''' Esta función responde a un método POST del servidor.
    ''' Identifica el Terminal timegate, obtenemos su estado y devolvemos la configuración del timegate.
    ''' </summary>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Initialize() As roGenericResponse(Of DTOs.Timegate)
        Dim lrret As roGenericResponse(Of DTOs.Timegate) = New roGenericResponse(Of DTOs.Timegate)

        Try
            lrret.Status = ErrorCodes.OK

            Dim terminalState As New roTerminalState
            roBaseState.SetSessionSmall(terminalState, -1, Global_asax.ApplicationSource, "")

            lrret = TerminalsHelper.GetTimeGateConfiguration(Global_asax.TimeGateSerial, terminalState)

            If lrret.Status = ErrorCodes.OK Then
                Dim apkVersion As String = HttpContext.Current.Request.Params("apkVersion")
                TerminalsHelper.SetTimeGateStatus(Global_asax.TimeGateSerial, apkVersion, terminalState)
            End If
        Catch ex As Exception
            lrret = New roGenericResponse(Of DTOs.Timegate)
            lrret.Value = Nothing
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TimeGate::GetSharedPortalConfiguration")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Esta función responde a un método POST del servidor.
    ''' Devolvemos la configuración de fondo de pantalla de Time Gate
    ''' </summary>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetBackground() As roGenericResponse(Of String)
        Dim lrret As roGenericResponse(Of String) = New roGenericResponse(Of String)

        Try
            lrret.Status = ErrorCodes.OK

            Dim terminalState As New roTerminalState
            roBaseState.SetSessionSmall(terminalState, -1, Global_asax.ApplicationSource, "")

            lrret = TerminalsHelper.GetTimeGateBackgroundConfiguration(Global_asax.TimeGateSerial, terminalState)

            If lrret.Status = ErrorCodes.OK Then
                Dim apkVersion As String = HttpContext.Current.Request.Params("apkVersion")
                TerminalsHelper.SetTimeGateStatus(Global_asax.TimeGateSerial, apkVersion, terminalState)
            End If
        Catch ex As Exception
            lrret = New roGenericResponse(Of String)
            lrret.Value = Nothing
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TimeGate::GetSharedPortalConfiguration")
        End Try

        Return lrret
    End Function


    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Identify() As TimegateResponse
        Dim lrret As New TimegateResponse

        Try
            lrret.Status = ErrorCodes.OK
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")


            Dim id As String = HttpContext.Current.Request.Params("id")
            Dim pin As String = HttpContext.Current.Request.Params("pin")
            Dim nfc As String = HttpContext.Current.Request.Params("nfc")
            Dim accessFromApp As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("accessFromApp"))
            Dim timezone As String = HttpContext.Current.Request.Params("timeZone")

            If (String.IsNullOrEmpty(id) AndAlso String.IsNullOrEmpty(pin) AndAlso String.IsNullOrEmpty(nfc)) Then
                lrret.Status = ErrorCodes.BAD_CREDENTIALS
                Return lrret
            ElseIf Global_asax.TerminalIdentity Is Nothing Then
                lrret.Status = ErrorCodes.NO_LIVE_PORTAL
                Return lrret
            End If


            Dim employeeCredential As String
            Dim employeePassword As String
            Dim identifyMethod As AuthenticationMethod

            If String.IsNullOrEmpty(nfc) Then
                employeeCredential = id
                employeePassword = pin
                identifyMethod = AuthenticationMethod.Pin
            Else
                employeeCredential = nfc
                employeePassword = ""
                identifyMethod = AuthenticationMethod.NFC
            End If

            Dim oState As New roTerminalsState()
            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            Dim secState As New roSecurityState()
            roBaseState.SetSessionSmall(secState, -1, Global_asax.ApplicationSource, "")

            Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)
            Dim idEmployee As Integer = oTerminalManager.Identify(employeeCredential, employeePassword, identifyMethod, oState)

            If idEmployee = -1 Then
                lrret.Status = ErrorCodes.BAD_CREDENTIALS
                Return lrret
            Else
                Dim terminalBehaviour As ScopeMode = Global_asax.TerminalIdentity.Readers(0).ScopeMode
                lrret.TimeGateMode = terminalBehaviour

                Select Case terminalBehaviour
                    Case ScopeMode.EIP
                        lrret.LoginInfo = VTPortal.SecurityHelper.doTimeGateLogin(idEmployee, identifyMethod, strAuthToken, SERVER_VERSION, accessFromApp, Global_asax.ApplicationSource, secState)

                        If lrret.LoginInfo.Status = ErrorCodes.OK Then
                            Robotics.Web.Base.CookieSession.CreateAuthenticationCookie((lrret.LoginInfo.Status = ErrorCodes.OK), lrret.LoginInfo.Token, StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
                        End If

                    Case ScopeMode.TA
                        Dim zoneInfo As TimeZoneInfo
                        If timezone <> String.Empty Then
                            zoneInfo = Robotics.VTBase.roSupport.OlsonTimeZoneToTimeZoneInfo(timezone)
                            If zoneInfo Is Nothing Then
                                zoneInfo = TimeZoneInfo.Local
                            End If
                        Else
                            zoneInfo = TimeZoneInfo.Local
                        End If

                        Dim newPunch As New roTerminalInteractivePunch With {
                            .Command = InteractivePunchCommand.Punch,
                            .Punch = New roTerminalPunch With {
                                .IDEmployee = idEmployee,
                                .PunchDateTime = VTPortal.StatusHelper.GetCurrentTerminalDatetime(Global_asax.TerminalIdentity, zoneInfo),
                                .Method = identifyMethod,
                                .PIN = If(identifyMethod = AuthenticationMethod.Pin, pin, ""),
                                .Credential = If(identifyMethod = AuthenticationMethod.Pin, id, nfc),
                                .Action = "X",
                                .PunchData = New roTerminalPunchData,
                                .TimeZoneName = zoneInfo.Id
                                }
                            }

                        Dim resultPunch As roTerminalInteractivePunch = oTerminalManager.ProcessPunch(newPunch)
                        Dim savedPunch As Boolean = oTerminalManager.SavePunch(resultPunch.Punch)

                        If savedPunch Then
                            Select Case resultPunch.Punch.ActualType
                                Case PunchTypeEnum._IN
                                    resultPunch.Display.StringValue = roTerminalTextHelper.Translate("ProcessPunch.Welcome", Nothing,, oTerminalManager.GetLastPunchEmployeeLanguage())
                                Case PunchTypeEnum._OUT
                                    resultPunch.Display.StringValue = roTerminalTextHelper.Translate("ProcessPunch.Bye", Nothing,, oTerminalManager.GetLastPunchEmployeeLanguage())
                            End Select

                            lrret.PunchInfo = resultPunch
                            lrret.Status = ErrorCodes.OK
                        Else
                            lrret.Status = ErrorCodes.BAD_CREDENTIALS
                        End If

                    Case ScopeMode.CO
                        Dim zoneInfo As TimeZoneInfo
                        If timezone <> String.Empty Then
                            zoneInfo = Robotics.VTBase.roSupport.OlsonTimeZoneToTimeZoneInfo(timezone)
                            If zoneInfo Is Nothing Then
                                zoneInfo = TimeZoneInfo.Local
                            End If
                        Else
                            zoneInfo = TimeZoneInfo.Local
                        End If

                        Dim idCostCenter = Global_asax.TerminalIdentity.Readers(0).IDCostCenter

                        Dim newPunch As New roTerminalInteractivePunch With {
                            .Command = InteractivePunchCommand.Punch,
                            .Punch = New roTerminalPunch With {
                                .IDEmployee = idEmployee,
                                .PunchDateTime = VTPortal.StatusHelper.GetCurrentTerminalDatetime(Global_asax.TerminalIdentity, zoneInfo),
                                .Method = identifyMethod,
                                .PIN = If(identifyMethod = AuthenticationMethod.Pin, pin, ""),
                                .Credential = If(identifyMethod = AuthenticationMethod.Pin, id, nfc),
                                .Action = "C",
                                .PunchData = New roTerminalPunchData With {
                                    .CostCenterData = New roCostCenterPunchData With {
                                        .IdCostCenter = idCostCenter
                                    }
                                },
                                .TimeZoneName = zoneInfo.Id
                            }
                        }

                        Dim resultPunch As roTerminalInteractivePunch = oTerminalManager.ProcessPunch(newPunch)
                        If oTerminalManager.State.Result <> roTerminalsState.ResultEnum.NoError Then
                            Select Case oTerminalManager.State.Result
                                Case roTerminalsState.ResultEnum.ShouldBeInForThisAction
                                    lrret.Status = ErrorCodes.PUNCH_ERROR_NO_SEQUENCE
                                Case Else
                                    lrret.Status = ErrorCodes.BAD_CREDENTIALS
                            End Select
                        Else
                            Dim savedPunch As Boolean = oTerminalManager.SavePunch(resultPunch.Punch)
                            If savedPunch Then
                                resultPunch.Display.StringValue = roTerminalTextHelper.Translate("ProcessPunch.costcenter", Nothing,, oTerminalManager.GetLastPunchEmployeeLanguage())

                                lrret.PunchInfo = resultPunch
                                lrret.Status = ErrorCodes.OK
                            Else
                                lrret.Status = ErrorCodes.BAD_CREDENTIALS
                            End If
                        End If

                    Case Else
                        lrret.Status = ErrorCodes.BAD_CREDENTIALS
                        Return lrret
                End Select
            End If



        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::TimeGate::GetSharedPortalConfiguration")
        End Try

        Return lrret
    End Function

End Class