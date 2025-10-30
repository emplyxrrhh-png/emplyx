<%@ WebHandler Language="VB" Class="srvSecurityOptions" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base


Public Class srvSecurityOptions
    Inherits handlerBase

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission("Administration.SecurityOptions")

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getXSecurityOptions" ' Retorna un passport (Tab Superior)
                    Me.GetXSecurityOptions(context.Request("ID"))
                Case "newSecurityOptions"
                    Me.GetXSecurityOptions("-1")
                Case "saveXSecurityOptions"
                    Me.SaveXSecurityOptions(context.Request("ID"))
            End Select
        Else
            Dim strResponse As String = "MESSAGE" & _
                          "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            context.Response.Write(strResponse)
        End If
    End Sub

    Private Sub GetXSecurityOptions(ByVal oID As String)
        Dim oSecurityOptions As roSecurityOptions = API.SecurityOptionsServiceMethods.GetSecurityOptions(Nothing, oID, True)

        If oSecurityOptions Is Nothing Then Exit Sub

        'Check Permissions
        Dim disControls As Boolean = False
        If Me.oPermission < Permission.Write Then
            disControls = True
        End If

        Try
            Dim oJSONFields As New Generic.List(Of JSONFieldItem)

            oJSONFields.Add(New JSONFieldItem("ID", oSecurityOptions.Passport, New String() {}, roJSON.JSONType.None_JSON, Nothing, disControls))
            'oJSONFields.Add(New JSONFieldItem("TXTPASSWORDSECURITY", oSecurityOptions.PasswordSecurityLevel, New String() {"ctl00_contentMainBody_groupPassword_txtPasswordSecurity"}, roJSON.JSONType.Text_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("TXTHISTORICPASSWORD", oSecurityOptions.PreviousPasswordsStored, New String() {"ctl00_contentMainBody_groupPassword_txtHistoricPassword"}, roJSON.JSONType.Text_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("TXTPASSWORDOUTDATED", oSecurityOptions.DaysPasswordExpired, New String() {"ctl00_contentMainBody_groupPassword_txtPasswordOutdated"}, roJSON.JSONType.Text_JSON, Nothing, disControls))

            oJSONFields.Add(New JSONFieldItem("TXTTEMPORALYBLOCK", oSecurityOptions.AccessAttempsTemporaryBlock, New String() {"ctl00_contentMainBody_groupBlock_txtBlock1Attempts"}, roJSON.JSONType.Text_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("TXTPERMANENTBLOCK", oSecurityOptions.AccessAttempsPermanentBlock, New String() {"ctl00_contentMainBody_groupBlock_txtBlock2Attempts"}, roJSON.JSONType.Text_JSON, Nothing, disControls))

            oJSONFields.Add(New JSONFieldItem("CKREQUIEREKEY", IIf(oSecurityOptions.RequestValidationCode = True, "true", "false"), New String() {"ctl00_contentMainBody_groupAccessRestrictions_ckRequiereKey"}, roJSON.JSONType.OptionCheck_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("TXTSAVEKEYTIME", oSecurityOptions.SaveAuthorizedPointDays, New String() {"ctl00_contentMainBody_groupAccessRestrictions_ckRequiereKey_txtSaveKeyTime"}, roJSON.JSONType.Text_JSON, Nothing, disControls Or oSecurityOptions.RequestValidationCode = False))
            oJSONFields.Add(New JSONFieldItem("TXTACCESSDIFERENTIPS", oSecurityOptions.DifferentAccessPointExceeded, New String() {"ctl00_contentMainBody_groupAccessRestrictions_ckRequiereKey_txtAccessDiferentIps"}, roJSON.JSONType.Text_JSON, Nothing, disControls Or oSecurityOptions.RequestValidationCode = False))

            oJSONFields.Add(New JSONFieldItem("CKSAMEVERSIONAPP", IIf(oSecurityOptions.OnlySameVersionApp = True, "true", "false"), New String() {"ctl00_contentMainBody_groupAccessRestrictions_chkVersion"}, roJSON.JSONType.OptionCheck_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("CKALLOWEDIPS", IIf(oSecurityOptions.OnlyAllowedAdress <> "", "true", "false"), New String() {"ctl00_contentMainBody_groupAccessRestrictions_ChkRestrictedIP"}, roJSON.JSONType.OptionCheck_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("TXTALLOWEDIPS", oSecurityOptions.OnlyAllowedAdress, New String() {"ctl00_contentMainBody_groupAccessRestrictions_txtAllowedIPs"}, roJSON.JSONType.Text_JSON, Nothing, disControls))

            oJSONFields.Add(New JSONFieldItem("CKBLOCKPORTAL", IIf(oSecurityOptions.BlockAccessVTPortal = True, "true", "false"), New String() {"ctl00_contentMainBody_groupAccessBlock_ckBlockPortal"}, roJSON.JSONType.CheckBox_JSON, Nothing, disControls))
            oJSONFields.Add(New JSONFieldItem("CKBLOCKDESKTOP", IIf(oSecurityOptions.BlockAccessVTDesktop = True, "true", "false"), New String() {"ctl00_contentMainBody_groupAccessBlock_ckBlockDesktop"}, roJSON.JSONType.CheckBox_JSON, Nothing, disControls))

            oJSONFields.Add(New JSONFieldItem("CKREMOTEACCESS", IIf(oSecurityOptions.EnabledAccessSupportRobotics = True, "true", "false"), New String() {"ctl00_contentMainBody_groupRemote_ckRemoteAccess"}, roJSON.JSONType.CheckBox_JSON, Nothing, disControls))

            Dim strIPsGrid As String = String.Empty

            Dim allowedIPs As String() = oSecurityOptions.OnlyAllowedAdress.Split("#")
            Dim ipId As Integer = 1
            For Each oIp As String In allowedIPs
                If oIp <> String.Empty then
                    strIPsGrid &= "{fields:[{ field: 'id', value: '" & ipId & "' }, " & _
                                       "{ field: 'value', value: '" & oIp & "' }]},"
                    ipId = ipId + 1
                End If
            Next

            If strIPsGrid <> String.Empty Then
                strIPsGrid = "[" & strIPsGrid.Substring(0, strIPsGrid.Length - 1) & "]"
            Else
                strIPsGrid = "[]"
            End If

            Response.ContentType = "application/json"
            Dim strResponse As String = roJSONHelper.Serialize(oJSONFields)
            Response.Write("[" & strResponse & "],[" & strIPsGrid & "]")


        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub SaveXSecurityOptions(ByVal oID As String)
        Try

            If Me.oPermission < Permission.Write Then
                Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim oSecurityOptions As roSecurityOptions = Nothing
            Dim IsNegative As Boolean = False
            Dim bolIsNew As Boolean = False
            If oID = "-1" Then
                oSecurityOptions = New roSecurityOptions()
                oSecurityOptions.Passport = WLHelperWeb.CurrentPassport.ID
            Else
                oSecurityOptions = API.SecurityOptionsServiceMethods.GetSecurityOptions(Nothing, oID, False)
            End If

            If oSecurityOptions Is Nothing Then Exit Sub
            Dim bolRestrictedIP As Boolean = False
            For Each oParms As String In Request.Params
                If oParms Is Nothing Then Continue For
                Select Case oParms.ToUpper
                    'Case "TXTPASSWORDSECURITY"
                    '    oSecurityOptions.PasswordSecurityLevel = Request("TXTPASSWORDSECURITY")
                    Case "TXTHISTORICPASSWORD"
                        oSecurityOptions.PreviousPasswordsStored = roTypes.Any2Integer(Request("TXTHISTORICPASSWORD"))
                        If oSecurityOptions.PreviousPasswordsStored < 0 Then oSecurityOptions.PreviousPasswordsStored = 0
                    Case "TXTPASSWORDOUTDATED"
                        oSecurityOptions.DaysPasswordExpired = roTypes.Any2Integer(Request("TXTPASSWORDOUTDATED"))
                        If oSecurityOptions.DaysPasswordExpired < 0 Then oSecurityOptions.DaysPasswordExpired = 0
                    Case "TXTTEMPORALYBLOCK"
                        oSecurityOptions.AccessAttempsTemporaryBlock = roTypes.Any2Integer(Request("TXTTEMPORALYBLOCK"))
                        If oSecurityOptions.AccessAttempsTemporaryBlock < 0 Then oSecurityOptions.AccessAttempsTemporaryBlock = 0
                    Case "TXTPERMANENTBLOCK"
                        oSecurityOptions.AccessAttempsPermanentBlock = roTypes.Any2Integer(Request("TXTPERMANENTBLOCK"))
                        If oSecurityOptions.AccessAttempsPermanentBlock < 0 Then oSecurityOptions.AccessAttempsPermanentBlock = 0
                    Case "CKREQUIEREKEY"
                        oSecurityOptions.RequestValidationCode = Request("CKREQUIEREKEY")
                    Case "TXTSAVEKEYTIME"
                        oSecurityOptions.SaveAuthorizedPointDays = Request("TXTSAVEKEYTIME")
                    Case "TXTACCESSDIFERENTIPS"
                        oSecurityOptions.DifferentAccessPointExceeded = Request("TXTACCESSDIFERENTIPS")
                    Case "CKSAMEVERSIONAPP"
                        oSecurityOptions.OnlySameVersionApp = Request("CKSAMEVERSIONAPP")
                    Case "TXTALLOWEDIPS"
                        Dim allowedIPS As String = Request("TXTALLOWEDIPS")
                        If allowedIPS.StartsWith(",") Then
                            allowedIPS = allowedIPS.Substring(1)
                        End If
                        oSecurityOptions.OnlyAllowedAdress = allowedIPS
                    Case "CKBLOCKPORTAL"
                        oSecurityOptions.BlockAccessVTPortal = Request("CKBLOCKPORTAL")
                    Case "CKBLOCKDESKTOP"
                        oSecurityOptions.BlockAccessVTDesktop = Request("CKBLOCKDESKTOP")
                    Case "CKREMOTEACCESS"
                        oSecurityOptions.EnabledAccessSupportRobotics = Request("CKREMOTEACCESS")
                    Case "CKALLOWEDIPS"
                        bolRestrictedIP = roTypes.Any2Boolean(Request("CKALLOWEDIPS"))
                End Select
            Next

            If bolRestrictedIP = False Then
                oSecurityOptions.OnlyAllowedAdress = ""
            End If

            'Guardar SecurityOptions
            If API.SecurityOptionsServiceMethods.SaveSecurityOptions(Nothing, oSecurityOptions, True) Then
                Dim rOK As New roJSON.JSONError(False, "OK:" & oSecurityOptions.Passport)
                Response.Write(rOK.toJSON)
            Else
                Dim rError As New roJSON.JSONError(True, API.SecurityOptionsServiceMethods.LastErrorText)
                Response.Write(rError.toJSON)
            End If

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

End Class