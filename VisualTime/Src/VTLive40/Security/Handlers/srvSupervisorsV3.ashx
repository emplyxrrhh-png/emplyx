<%@ WebHandler Language="VB" Class="srvSupervisorsV3" %>

Imports System
Imports System.Data
Imports Robotics.VTBase
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.UsersAdmin
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base

Public Class srvSupervisorsV3
    Inherits handlerBase

    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"
    Private employeePermission As Permission = Permission.None

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission("Administration.Security")
        Me.employeePermission = Me.GetFeaturePermission("Employees.IdentifyMethods")

        Dim empActions As String() = {"resetPassport", "SendUsername"}

        If (Me.oPermission > Permission.None) OrElse
                (empActions.ToList().Contains(context.Request("action")) AndAlso employeePermission > Permission.None) Then

            Select Case context.Request("action")
                Case "getPassportTab" ' Retorna un passport (Tab Superior)
                    Me.LoadPassportDataTab()
                Case "chgName" 'Cambia el Nom de l'usuari seleccionat
                    Me.changePassportName()
                Case "deletePassport" 'Elimina un passport
                    Me.DeletePassport()
                Case "movePassport" ' Mueve el passport de grupo
                    Me.MovePassport()
                Case "resetPassport"
                    Me.ResetPassport()
                Case "restoreCegidID"
                    Me.RestoreCegidID()
                Case "SendUsername"
                    Me.SendUserName()
                Case "SetFeaturePermission"
                    Me.SetFeaturePermission()
                Case "SetDefaultFeaturePermission"
                    Me.SetFeaturePermission(True)
                Case "checkPermission"
                    Me.CheckPermission()
                Case "checkPermission"
                    Me.CheckPermission()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
            End Select

        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Dim strResponse As String = "MESSAGE" &
                              "TitleKey=CheckPermission.Denied.Title&" +
                              "DescriptionKey=CheckPermission.Denied.Description&" +
                              "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                              "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & Robotics.Web.Base.WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Response.Write(strResponse)
        End If
    End Sub

#Region "Methods"

    ''' <summary>
    ''' Carrega l'usuari per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadPassportDataTab()
        Try

            Dim oCurrentPassport As roPassport

            If Me.oPermission > Permission.None Then

                If Request("ID") <> "source" Then

                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport)

                    If oCurrentPassport Is Nothing Then Exit Sub
                    Me.strActiveTab = Request("aTab")

                    Dim oMainDiv As New HtmlGenericControl("div")

                    Dim oImageDiv As New HtmlGenericControl("div")
                    oImageDiv.Attributes("class") = "blackRibbonIcon"

                    Dim img As HtmlImage = New HtmlImage
                    Select Case oCurrentPassport.GroupType
                        Case "U"
                            img.Src = "Images/PassportGroup_80.png"
                        Case "E"
                            img.Src = "Images/PassportEmployee_80.png"
                        Case ""
                            img.Src = "Images/Passport_80.png"
                    End Select
                    img.Height = 80
                    oImageDiv.Controls.Add(img)

                    Dim oTextDiv As New HtmlGenericControl("div")
                    oTextDiv.Attributes("class") = "blackRibbonDescription"
                    oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameSupervisor"" class=""NameText"">  " & oCurrentPassport.Name & "</span></div>"

                    Dim oButtonsDiv As New HtmlGenericControl("div")
                    oButtonsDiv.Attributes("class") = "blackRibbonButtons"
                    oButtonsDiv.Controls.Add(Me.CreateTabs(oCurrentPassport))

                    oMainDiv.Controls.Add(oImageDiv)
                    oMainDiv.Controls.Add(oTextDiv)
                    oMainDiv.Controls.Add(oButtonsDiv)

                    Dim sw As New IO.StringWriter
                    Dim htw As New HtmlTextWriter(sw)
                    oMainDiv.RenderControl(htw)

                    Response.Write(sw.ToString)

                Else
                    Dim oMainDiv As New HtmlGenericControl("div")

                    Dim oImageDiv As New HtmlGenericControl("div")
                    oImageDiv.Attributes("class") = "blackRibbonIcon"

                    Dim img As HtmlImage = New HtmlImage
                    img.Src = "Images/AccessGroups80.png"
                    oImageDiv.Controls.Add(img)

                    Dim oTextDiv As New HtmlGenericControl("div")
                    oTextDiv.Attributes("class") = "blackRibbonDescription"

                    Dim oButtonsDiv As New HtmlGenericControl("div")
                    oButtonsDiv.Attributes("class") = "blackRibbonButtons"

                    oMainDiv.Controls.Add(oImageDiv)
                    oMainDiv.Controls.Add(oTextDiv)
                    oMainDiv.Controls.Add(oButtonsDiv)

                    Dim sw As New IO.StringWriter
                    Dim htw As New HtmlTextWriter(sw)
                    oMainDiv.RenderControl(htw)

                    Response.Write(sw.ToString)
                End If

            End If

        Catch ex As Exception
            Response.Write("MESSAGE" & _
                           "TitleKey=SaveName.Error.Text&" & _
                           "DescriptionText=" & ex.ToString & "&" & _
                           "Option1TextKey=SaveName.Error.Option1Text&" & _
                           "Option1DescriptionKey=SaveName.Error.Option1Description&" & _
                           "Option1OnClickScript=HideMsgBoxForm(); return false;&" & _
                           "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon))

        End Try
    End Sub

    ''' <summary>
    ''' Cambia el nom del passport
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changePassportName()
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write Then
                ' Obtenemos el passport a modificar
                oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport)
                If oCurrentPassport IsNot Nothing Then
                    Dim NouName As String = Request("NewName")
                    If NouName = "" Then
                        strErrorInfo = Me.Language.Translate("NameInvalid", Me.DefaultScope)
                    Else
                        oCurrentPassport.Name = NouName
                        If Not API.UserAdminServiceMethods.SavePassport(Nothing, oCurrentPassport, False) Then
                            strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                        End If
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try
        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" & _
                          "TitleKey=SaveName.Error.Text&" & _
                          "DescriptionText=" & strErrorInfo & "&" & _
                          "Option1TextKey=SaveName.Error.Option1Text&" & _
                          "Option1DescriptionKey=SaveName.Error.Option1Description&" & _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" & _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If
        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\Security\Passports\Supervisors", Robotics.Web.Base.WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Supervisors")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Genera els botons de la dreta (General, ...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateTabs(ByVal oCurrentPassport As roPassport) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_General", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(1)
        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_IdentifyMethods", Me.Language.Translate("tabIdentifyMethods", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(2)
        oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_AllowedApplications", Me.Language.Translate("tabAllowedApplications", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(2))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        '================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell
        hCellGen.Attributes("valign") = "top"

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        '================================

        ReDim Preserve oTabButtons(3)
        oTabButtons(3) = CreateNewHtmlAnchor("TABBUTTON_RESTRICTIONS", Me.Language.Translate("tabRestrictions", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(3))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        Dim intNotifierTab As Integer = 4

        Dim iNextTab As Integer = 4
        ReDim Preserve oTabButtons(iNextTab)
        oTabButtons(iNextTab) = CreateNewHtmlAnchor("TABBUTTON_NOTIFICATIONS", Me.Language.Translate("tabNotifications", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTab))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        intNotifierTab = iNextTab

        iNextTab = iNextTab + 1

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs('TABBUTTON_General');")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeTabs('TABBUTTON_IdentifyMethods');")
        oTabButtons(2).Attributes.Add("OnClick", "javascript: changeTabs('TABBUTTON_AllowedApplications');")
        oTabButtons(3).Attributes.Add("OnClick", "javascript: changeTabs('TABBUTTON_RESTRICTIONS');")
        oTabButtons(intNotifierTab).Attributes.Add("OnClick", "javascript: changeTabs('TABBUTTON_NOTIFICATIONS');")

        Select Case Me.strActiveTab
            Case "TABBUTTON_General"
                oTabButtons(0).Attributes("class") = "bTab-active"
            Case "TABBUTTON_IdentifyMethods"
                oTabButtons(1).Attributes("class") = "bTab-active"
            Case "TABBUTTON_AllowedApplications"
                oTabButtons(2).Attributes("class") = "bTab-active"
            Case "TABBUTTON_RESTRICTIONS"
                oTabButtons(3).Attributes("class") = "bTab-active"
            Case "TABBUTTON_NOTIFICATIONS"
                oTabButtons(intNotifierTab).Attributes("class") = "bTab-active"
        End Select

        Return hTableGen ' Retorna el HTMLTable
    End Function

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Borra el passport seleccionado
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeletePassport()
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            If Me.oPermission >= Permission.Admin Then
                oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, True)
                If oCurrentPassport IsNot Nothing Then

                    Dim hasCommuniques = API.CommuniqueServiceMethods.GetCommuniquesByCreator(Nothing, Request("ID"))

                    If hasCommuniques.Count > 0 Then
                        strErrorInfo = "No se puede eliminar el supervisor porque tiene comunicados con él como remitente. Por favor, gestione dichos comunicados antes de eliminar el supervisor"
                    End If

                    If strErrorInfo = "" Then
                        'Si el passport es de un empleado no se permite su borrado desde la pantalla de seguridad, solo desde la de empleados.
                        If oCurrentPassport.GroupType.ToUpper.Trim = "E" Then
                            oCurrentPassport.IsSupervisor = False

                            If Not API.UserAdminServiceMethods.SavePassport(Nothing, oCurrentPassport) Then
                                strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                            End If
                        Else
                            If Not API.UserAdminServiceMethods.DeletePassportByID(Nothing, Request("ID")) Then
                                strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                            End If
                        End If
                    End If

                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try
        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" & _
                          "TitleKey=RemovePassport.Error.Title&" + _
                          "DescriptionText=" + strErrorInfo + "&" + _
                          "Option1TextKey=RemovePassport.Error.Option1Text&" + _
                          "Option1DescriptionKey=RemovePassport.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If
        Response.Write(strResponse)
    End Sub

    Private Sub RestoreCegidID()
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Dim strOkInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write OrElse Me.employeePermission >= Permission.Write Then

                Dim oPassport As roPassport = Nothing
                If Request("PassportType") = "U" Then
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Passport)
                Else
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Employee, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Employee)
                End If

                If oCurrentPassport IsNot Nothing Then

                    If Robotics.Web.Base.WLHelperWeb.CurrentPassport.ID <> oCurrentPassport.ID Then
                        Dim oPassportAuthenticationMethods As roPassportAuthenticationMethodsRow

                        Try
                            If Not oPassport Is Nothing Then
                                oPassportAuthenticationMethods = oPassport.AuthenticationMethods.IntegratedSecurityRow
                                If oPassportAuthenticationMethods IsNot Nothing Then

                                    With oPassportAuthenticationMethods
                                        .RowState = RowState.UpdateRow
                                        .Credential = "\temp_" & Guid.NewGuid.ToString("N").ToLower
                                    End With

                                    oPassport.AuthenticationMethods.IntegratedSecurityRow = oPassportAuthenticationMethods

                                    Dim opassportManager As New roPassportManager
                                    opassportManager.Save(oPassport)

                                End If
                            End If
                        Catch ex As Exception
                            strErrorInfo = ex.Message
                        End Try

                        If strErrorInfo = "" Then
                            strErrorInfo = "OK"
                            strOkInfo = Me.Language.Translate("SavePassport.InfoPwd.ResetDescription", "Supervisors")

                        End If
                    Else
                        strErrorInfo = Me.Language.Translate("ResetPassword.MySelf.Error", Me.DefaultScope)
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If

            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try

        If strErrorInfo = "OK" Then
            strResponse = "OK"
        Else
            If strErrorInfo <> "" Then
                strResponse = "MESSAGE" &
                              "TitleKey=MovePassport.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=MovePassport.Error.Option1Text&" +
                              "Option1DescriptionKey=MovePassport.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
        End If

        Response.Write(strResponse)
    End Sub

    Private Sub ResetPassport()
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Dim strOkInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write OrElse Me.employeePermission >= Permission.Write Then

                Dim oPassport As roPassport = Nothing
                If Request("PassportType") = "U" Then
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Passport)
                Else
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Employee, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Employee)
                End If

                If oCurrentPassport IsNot Nothing Then

                    If Robotics.Web.Base.WLHelperWeb.CurrentPassport.ID <> oCurrentPassport.ID Then
                        Dim oPassportAuthenticationMethods As roPassportAuthenticationMethodsRow
                        Dim strAux As String
                        Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))

                        Try
                            If Not oPassport Is Nothing Then
                                oPassportAuthenticationMethods = oPassport.AuthenticationMethods.PasswordRow
                                If oPassportAuthenticationMethods IsNot Nothing Then

                                    If Not oPassportAuthenticationMethods.Credential.Contains("\") Then 'Usuario no se valida mediante Active Directory

                                        Dim r As New Random()
                                        Dim strCode As String = ""
                                        For i As Integer = 1 To 10
                                            strCode &= CStr(r.Next(0, 9))
                                        Next
                                        strAux = oPassport.Name.PadRight(51) & oPassportAuthenticationMethods.Credential & " - " & strCode & Environment.NewLine
                                        oPassportAuthenticationMethods.Password = CryptographyHelper.EncryptWithMD5(strCode)
                                        oPassportAuthenticationMethods.RowState = RowState.UpdateRow
                                        oPassportAuthenticationMethods.LastUpdatePassword = New Date(1900, 1, 1)
                                        oPassportAuthenticationMethods.InvalidAccessAttemps = 0
                                        oPassportAuthenticationMethods.LastDateInvalidAccessAttempted = Nothing

                                        Dim opassportManager As New roPassportManager
                                        opassportManager.Save(oPassport)
                                        AuthHelper.SetPassportKeyValidated(oPassport.ID, False, "", False)

                                        'Solo enviamos el mail si el servicio esta activo
                                        If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1 Then
                                            Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric , Parameters) VALUES (1903, " & Request("ID") & ", " & oPassport.ID & ",'" & CryptographyHelper.Encrypt(strCode) & "')")
                                        End If

                                    End If

                                End If
                            End If
                        Catch ex As Exception
                            strErrorInfo = ex.Message
                        End Try

                        If strErrorInfo = "" Then
                            strErrorInfo = "OK"
                            strOkInfo = Me.Language.Translate("SavePassport.InfoPwd.ResetDescription", "Supervisors")

                        End If
                    Else
                        strErrorInfo = Me.Language.Translate("ResetPassword.MySelf.Error", Me.DefaultScope)
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If

            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try

        If strErrorInfo = "OK" Then
            strResponse = "INFOMSG" &
                          "TitleKey=SavePassport.InfoPwd.Title&" +
                          "DescriptionText=" + strOkInfo + "&" +
                          "Option1TextKey=SavePassport.InfoPwd.Option1Text&" +
                          "Option1DescriptionKey=SavePassport.InfoPwd.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.InformationIcon)
        Else
            If strErrorInfo <> "" Then
                strResponse = "MESSAGE" &
                              "TitleKey=MovePassport.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=MovePassport.Error.Option1Text&" +
                              "Option1DescriptionKey=MovePassport.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
        End If

        Response.Write(strResponse)
    End Sub

    Private Sub SendUserName()
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Dim strOkInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write OrElse Me.employeePermission >= Permission.Write Then

                Dim oPassport As roPassport = Nothing
                If Request("PassportType") = "U" Then
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Passport)
                Else
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Employee, False)
                    oPassport = roPassportManager.GetPassport(Request("ID"), LoadType.Employee)
                End If

                If oCurrentPassport IsNot Nothing Then

                    If Robotics.Web.Base.WLHelperWeb.CurrentPassport.ID <> oCurrentPassport.ID Then
                        Dim oPassportAuthenticationMethods As roPassportAuthenticationMethodsRow
                        Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))

                        Try
                            If Not oPassport Is Nothing Then
                                oPassportAuthenticationMethods = oPassport.AuthenticationMethods.PasswordRow
                                If oPassportAuthenticationMethods IsNot Nothing Then
                                    If Not oPassportAuthenticationMethods.Credential.Contains("\") Then 'Usuario no se valida mediante Active Directory

                                        'Solo enviamos el mail si el servicio esta activo
                                        If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1 Then
                                            Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric , Parameters) VALUES (1905, " & Request("ID") & ", " & oPassport.ID & ",'" & oPassportAuthenticationMethods.Credential & "')")
                                        End If

                                    End If

                                End If
                            End If
                        Catch ex As Exception
                            strErrorInfo = ex.Message
                        End Try

                        If strErrorInfo = "" Then
                            strErrorInfo = "OK"
                            strOkInfo = Me.Language.Translate("SendUsername.InfoUsername.SendUsernameDescription", "Supervisors")

                        End If
                    Else
                        strErrorInfo = Me.Language.Translate("SendUsername.MySelf.Error", Me.DefaultScope)
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If

            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try

        If strErrorInfo = "OK" Then
            strResponse = "INFOMSG" &
                          "TitleKey=SendUsername.InfoUsername.Title&" +
                          "DescriptionText=" + strOkInfo + "&" +
                          "Option1TextKey=SendUsername.InfoUsername.Option1Text&" +
                          "Option1DescriptionKey=SendUsername.InfoUsername.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.InformationIcon)
        Else
            If strErrorInfo <> "" Then
                strResponse = "MESSAGE" &
                              "TitleKey=SendUsername.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=SendUsername.Error.Option1Text&" +
                              "Option1DescriptionKey=SendUsername.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
        End If

        Response.Write(strResponse)
    End Sub

    Private Sub MovePassport()

        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write Then

                oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, False)
                If oCurrentPassport IsNot Nothing Then

                    Dim belongsToAdmin As Boolean = API.SecurityServiceMethods.GetPassportBelongsToAdminGroup(Nothing, oCurrentPassport.ID)
                    Dim finalBelongsToAdmin As Boolean = False

                    If Request("newParent") <> "source" Then
                        Dim destGroup As Integer = roTypes.Any2Integer(Request("newParent"))
                        finalBelongsToAdmin = destGroup = 3 Or API.SecurityServiceMethods.GetPassportBelongsToAdminGroup(Nothing, destGroup)
                    End If

                    If belongsToAdmin AndAlso finalBelongsToAdmin = False Then
                        If GetCurrentPassportsInGroup(3) = 1 Then
                            strErrorInfo = Me.Language.Translate("MoveLastPassport.Error", Me.DefaultScope)
                        End If
                    End If

                    If strErrorInfo = "" Then
                        If Request("newParent") = "source" Then
                            oCurrentPassport.IDParentPassport = Nothing
                        Else

                            If oCurrentPassport.GroupType = "E" And Not oCurrentPassport.IDUser.HasValue Then
                                'crear user
                                If Not API.UserAdminServiceMethods.CreateUserOfPassport(Nothing, oCurrentPassport) Then
                                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                                End If
                            End If

                            oCurrentPassport.IDParentPassport = Request("newParent")

                        End If
                    End If

                    If strErrorInfo = "" Then
                        If Not API.UserAdminServiceMethods.SavePassport(Nothing, oCurrentPassport, False) Then
                            strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                        End If
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If

            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try
        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" & _
                          "TitleKey=MovePassport.Error.Title&" + _
                          "DescriptionText=" + strErrorInfo + "&" + _
                          "Option1TextKey=MovePassport.Error.Option1Text&" + _
                          "Option1DescriptionKey=MovePassport.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If
        Response.Write(strResponse)
    End Sub

    Private Sub SetFeaturePermission(Optional ByVal bolDefault As Boolean = False)

        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""

        Try

            Dim oCurrentPassport As roPassport

            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write Then

                oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Request("ID"), LoadType.Passport, True)
                If oCurrentPassport IsNot Nothing Then

                    Dim intIDFeature As Integer = Request("IDFeature")
                    Dim strFeatureType As String = "U"
                    If Request("FeatureType") <> Nothing Then strFeatureType = Request("FeatureType")
                    Dim Permission As Permission
                    If Not bolDefault Then
                        Permission = System.Enum.Parse(GetType(Permission), Request("Permission"))
                    End If

                    'Comprobar si es el grupo "Administradores" y en caso de que se anule algun permiso no permitirlo
                    If oCurrentPassport.ID = 3 And Permission = Permission.None Then
                        strErrorInfo = Me.Language.Translate("CheckGroupAdm.Denied.Message", Me.DefaultScope) & " " & oCurrentPassport.Description
                    Else

                        Dim oFeaturesChanged As List(Of Feature) = Nothing

                        Dim bolRet As Boolean
                        If Not bolDefault Then
                            bolRet = API.UserAdminServiceMethods.SetFeaturePermission(Nothing, oCurrentPassport.ID, intIDFeature, strFeatureType, Permission, oFeaturesChanged)
                        Else
                            bolRet = API.UserAdminServiceMethods.SetDefaultFeaturePermission(Nothing, oCurrentPassport.ID, intIDFeature, strFeatureType, oFeaturesChanged)
                        End If
                        If bolRet Then

                            strResponse = "OK"
                            If oFeaturesChanged IsNot Nothing Then
                                Dim strFeaturesChanged As String = ""
                                Dim strFeaturesChangedNamesForAudit As String = ""
                                For Each oFeature As Feature In oFeaturesChanged
                                    strFeaturesChanged &= "~" & oFeature.ID & "*" & CType(oFeature.EditedValue, Permission).ToString() & "*" & CType(oFeature.InheritedValue, Permission).ToString()
                                Next
                                strFeaturesChanged = strFeaturesChanged.Substring(1)
                                strResponse &= strFeaturesChanged

                                'Auditoría
                                Dim lstAuditParameterNames As New List(Of String)
                                Dim lstAuditParameterValues As New List(Of String)
                                lstAuditParameterNames.Add("{PassportName}")
                                lstAuditParameterValues.Add(oCurrentPassport.Name)
                                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tPassportPermissionsOverFeatures, oCurrentPassport.Name, lstAuditParameterNames, lstAuditParameterValues, Nothing)
                            End If

                        Else
                            strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                        End If
                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try

        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" & _
                          "TitleKey=SetFeaturePermission.Error.Title&" + _
                          "DescriptionText=" + strErrorInfo + "&" + _
                          "Option1TextKey=SetFeaturePermission.Error.Option1Text&" + _
                          "Option1DescriptionKey=SetFeaturePermission.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If

        Response.Write(strResponse)

    End Sub

    Private Sub CheckPermission()

        Dim strResponse As String = "OK"

        If Not Me.HasFeaturePermission("Administration.Security", Permission.Read) Then
            strResponse = "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)

        End If

        Response.Write(strResponse)

    End Sub

    Private Function GetCurrentPassportsInGroup(ByVal IdPassport As Integer) As Integer
        Dim usersCount As Integer = 0
        Dim oPassports As DataTable = API.UserAdminServiceMethods.GetPassportsByParentLite(Nothing, IdPassport, "")
        If oPassports IsNot Nothing Then usersCount = usersCount + oPassports.Rows.Count

        oPassports = API.UserAdminServiceMethods.GetPassportsByParentLite(Nothing, IdPassport, "E")
        If oPassports IsNot Nothing Then usersCount = usersCount + oPassports.Rows.Count

        oPassports = API.UserAdminServiceMethods.GetPassportsByParentLite(Nothing, IdPassport, "U")

        For Each oPass As DataRow In oPassports.Rows
            usersCount = usersCount + GetCurrentPassportsInGroup(roTypes.Any2Integer(oPass("ID")))
        Next

        Return usersCount
    End Function
#End Region

End Class