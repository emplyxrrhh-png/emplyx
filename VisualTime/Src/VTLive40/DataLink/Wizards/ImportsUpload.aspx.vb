Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Wizards_ImportsUpload
    Inherits PageBase

    Private idImport As Integer = 0
    Private separator As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)
        'TODO: falta añadir control de permisos para usuario no autorizado

        Dim strFileTypes As String = "111"
        Dim intSelectedIndex As Integer = 0
        Dim isSelected = False

        If Request("FileTypes") IsNot Nothing AndAlso Request("FileTypes") <> "" Then
            strFileTypes = Request("FileTypes")
        End If

        If Request("IdImport") IsNot Nothing AndAlso Request("IdImport") <> "" Then
            Me.idImport = Request("IdImport")
        End If

        If Request("Separator") IsNot Nothing AndAlso Request("Separator") <> "" Then
            Me.separator = Request("Separator")
        End If

        If Not Me.IsPostBack Then

            If Request("isBusiness") IsNot Nothing AndAlso Request("isBusiness") <> "" Then
                hdnIsBusiness.Value = roTypes.Any2String(Request("isBusiness"))
            End If

            txtSeparator.Text = Me.separator

            For n As Integer = 0 To strFileTypes.Length - 1
                If Me.rdList.Items.Count > n Then
                    Me.rdList.Items(n).Enabled = (strFileTypes.Substring(n, 1) = "1")
                    If Me.rdList.Items(n).Enabled Then
                        If Not isSelected Then
                            Me.rdList.Items(n).Selected = (strFileTypes.Substring(n, 1) = "1")
                            isSelected = True
                            intSelectedIndex = n
                        End If
                    End If
                End If
            Next

            Dim isVis As Boolean
            Select Case intSelectedIndex
                Case 0, 2 'Excel , XML
                    isVis = False
                Case 1 'ASCII
                    isVis = True
            End Select

            lblSchema.Visible = True
            fileSchema.Visible = True

            If Me.idImport = 3 Then
                lblSchema.Style("display") = "none"  'lblSchema.Visible lblSchema.Visible = False
                fileSchema.Style("display") = "none"  'fileSchema.VisiblefileSchema.Visible = False
                trSeparator.Style("display") = "none"  'trSeparator.VisiblefileSchema.Visible = False
            Else

                If isVis Then
                    lblSchema.Style("display") = ""
                    fileSchema.Style("display") = ""
                    trSeparator.Style("display") = ""
                Else
                    If Me.idImport = 20 AndAlso API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Employees.DataLink.Imports.SageMurano", "U", Permission.Write) Then
                        lstAvailableCustomTemplates.Visible = True
                        lstAvailableCustomTemplates.Items.Clear()
                        lstAvailableCustomTemplates.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Template.Nothing", DefaultScope), "-1"))
                        lstAvailableCustomTemplates.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Template.SageMurano", DefaultScope), "8"))
                        lblSelectTemplate.Text = Language.Translate("Template.Select", DefaultScope)
                        lblSelectTemplate.Visible = True
                    End If
                    lblSchema.Style("display") = "none"
                    fileSchema.Style("display") = "none"
                    trSeparator.Style("display") = "none"
                    trSeparator.Style("display") = "none"
                End If
            End If

            lblSeparator.Visible = hdnIsBusiness.Value = "1" '1 DataLinkBusiness(Importar / Exportar) - 0 DataLink (Enlace de Datos)
            txtSeparator.Visible = hdnIsBusiness.Value = "1"

        End If

    End Sub

    Protected Sub rdList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdList.SelectedIndexChanged
        'Dim isVis As Boolean = False
        'Select Case rdList.SelectedIndex
        '    Case 0, 2 'Excel , XML
        '        isVis = False
        '    Case 1 'ASCII
        '        isVis = True
        'End Select

        'lblSchema.Visible = isVis
        'fileSchema.Visible = isVis
        'lblSeparator.Visible = isVis
        'txtSeparator.Visible = isVis
    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim strMsg As String = ""
        Dim fileType As Integer = 0
        'Control d'errors en la carrega
        If rdList.SelectedValue = "TEXT" Then
            If Me.idImport <> 3 Then
                If Not fileSchema.HasFile Then
                    strMsg = Language.Translate("FileASCIISchema.NotFound", DefaultScope)
                End If
            End If
        End If

        If Not fileOrig.HasFile Then
            strMsg = Language.Translate("FileImport.NotFound", DefaultScope)
        End If

        If strMsg <> "" Then
            Me.lblMsg.Text = strMsg
            Me.lblMsg.Visible = True
        Else

            Session.Remove("ImportFileType")
            Session.Add("ImportFileType", rdList.SelectedValue)

            Session.Remove("ImportFileOrig")
            Session.Add("ImportFileOrig", fileOrig.FileBytes)

            Session.Remove("ImportFileOrigName")
            Session.Add("ImportFileOrigName", fileOrig.FileName)

            Session.Remove("ImportFileSchema")
            If Me.idImport = 1 Or Me.idImport = 10 Or Me.idImport = 22 Or Me.idImport = 20 Then
                Session.Add("ImportFileSchema", fileSchema.FileBytes)
            End If

            Session.Remove("IDImportTemplate")
            If Me.idImport = 20 Then
                If lstAvailableCustomTemplates.SelectedItem IsNot Nothing AndAlso lstAvailableCustomTemplates.SelectedItem.Value <> -1 Then
                    Session.Add("IDImportTemplate", lstAvailableCustomTemplates.SelectedItem.Value)
                Else
                    Session.Add("IDImportTemplate", 1)
                End If
            End If

            Session.Remove("ImportSeparator")
            Session.Add("ImportSeparator", txtSeparator.Text)

            If rdList.SelectedValue = "EXCEL" Then
                fileType = 1
            ElseIf rdList.SelectedValue = "TEXT" Then
                fileType = 2
            ElseIf rdList.SelectedValue = "XML" Then
                fileType = 3
            End If

            Session.Remove("FileType")
            Session.Add("FileType", fileType)

            Me.Controls.Clear()
            Me.Response.Write("<script language=""javascript"">parent.ContinueSend();</script>")

        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "window-script", "hidePopupLoader();", True)

    End Sub

End Class