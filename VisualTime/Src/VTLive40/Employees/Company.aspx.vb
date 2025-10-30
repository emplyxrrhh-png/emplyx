Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Company
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")

        InsertExtraJavascript("Documents", "~/Documents/Scripts/Documents.js")
        InsertExtraJavascript("DocumentTemplate", "~/Documents/Scripts/DocumentTemplate.js")
        InsertExtraJavascript("GridDocuments", "~/Documents/Scripts/GridDocuments.js")
    End Sub

    'Dim aux = WLHelperWeb.MainMenu.List.ToList.Find(Function(x) x.Path = "Portal\Company").Childs.List 'toList '<-- No funciona

    'Dim companyElements As New List(Of Robotics.Web.Base.PortalBaseSvc.wscMenuElement)

    'For Each o As Object In aux
    '            companyElements.Add(o)

    '        Next
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack AndAlso Not IsCallback Then
            LoadCompanyDataTab()

            Dim companyElements As New List(Of System.Object)

            companyElements.AddRange(WLHelperWeb.MainMenu.List.ToArray.ToList.Find(Function(x) x.Path = "Portal\Company").Childs.List.ToArray)

            Me.btnGoToGroups.JSProperties.Add("cpDestinationURL", String.Format("/{0}/" & companyElements.Find(Function(x) x.Path.StartsWith("Portal\Company\Groups")).URL, Configuration.RootUrl))


            Dim curElem = companyElements.Find(Function(x) x.Path.StartsWith("Portal\Company\Collectives"))
            If curElem IsNot Nothing Then
                Me.btnGoToCollectives.JSProperties.Add("cpDestinationURL", String.Format("/{0}/" & curElem.URL, Robotics.Web.Base.Configuration.RootUrl))
            Else
                Me.btnGoToCollectives.JSProperties.Add("cpDestinationURL", "")
            End If

            curElem = companyElements.Find(Function(x) x.Path.StartsWith("Portal\Company\Passports"))

            curElem = companyElements.Find(Function(x) x.Path.StartsWith("Portal\Company\SecurityFunctions"))
            If curElem IsNot Nothing Then
                Me.btnGoToRolesV3.JSProperties.Add("cpDestinationURL", String.Format("/{0}/" & curElem.URL, Robotics.Web.Base.Configuration.RootUrl))
            Else
                Me.btnGoToRolesV3.JSProperties.Add("cpDestinationURL", "")
            End If

            curElem = companyElements.Find(Function(x) x.Path.StartsWith("Portal\Company\AdvSupervisors"))
            If curElem IsNot Nothing Then
                Me.btnGoToSupervisorsV3.JSProperties.Add("cpDestinationURL", String.Format("/{0}/" & curElem.URL, Robotics.Web.Base.Configuration.RootUrl))
            Else
                Me.btnGoToSupervisorsV3.JSProperties.Add("cpDestinationURL", "")
            End If

        End If

        If Not API.LicenseServiceMethods.FeatureIsInstalled("Feature\Collectives") Then
            Me.btnGoToCollectives.Visible = False
            Me.txtDisabledCollectives.InnerHtml = Me.Language.Translate("collectivesnotinstalled", Me.DefaultScope)
        End If

        Me.companySecurityModeV3.Style("display") = ""

        Me.companyGroupsDiv.Attributes("class") = "descriptionCompanyDiv descriptionMaxWidth"
        Me.companySecurityModeV3.Attributes("class") = "descriptionCompanyDiv descriptionMaxWidth"

    End Sub

    Private Sub LoadCompanyDataTab()
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Company", WLHelperWeb.CurrentPassportID)
            Dim destDiv As HtmlGenericControl = roTools.BuildCentralBar(guiActions, -1, Me.Language, Me.DefaultScope, "",, roTools.ToolBarDirection.Horizontal)
            Me.tbButtons.Controls.Clear()
            Me.tbButtons.Controls.Add(destDiv)
        Catch ex As Exception
            Me.tbButtons.InnerHtml = ex.Message.ToString
        End Try
    End Sub

End Class