Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_roDestinationSelectorV2
    Inherits PageBase

    Public Property AvailableSupervisors(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim oLst = ViewState("frmAddDestination_AvailableSupervisors")

            If oLst Is Nothing OrElse bolReload Then
                oLst = API.UserAdminServiceMethods.GetSupervisorPassports(Me.Page)
                ViewState("frmAddDestination_AvailableSupervisors") = oLst
            End If
            Return oLst
        End Get
        Set(value As DataTable)
            ViewState("frmAddDestination_AvailableSupervisors") = value
        End Set
    End Property

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.hdnLngSupervisorTextSelected.Value = Me.oLanguage.Translate("hdnLngSupervisorTextSelected", "Scope")
        Me.hdnLngSupervisorsTextSelected.Value = Me.oLanguage.Translate("hdnLngSupervisorsTextSelected", "Scope")
        Me.hdnLngNoDocument.Value = Me.oLanguage.Translate("hdnLngNoDocument", "Scope")
        Me.hdnLngToDocument.Value = Me.oLanguage.Translate("hdnLngToDocument", "Scope")
        Me.hdnEmployeeField.Value = Me.oLanguage.Translate("hdnEmployeeField", "Scope")
        Me.hdnNoHardcodedField.Value = Me.oLanguage.Translate("hdnNoHardcodedField", "Scope")

        If Not Me.IsPostBack Then
            AvailableSupervisors = Nothing
            Me.tbAvailableSupervisors.Items.Clear()
            Dim tmpSupervisors = AvailableSupervisors.Rows
            If tmpSupervisors IsNot Nothing Then
                For Each oVal As DataRow In tmpSupervisors
                    Me.tbAvailableSupervisors.Items.Add(oVal("Name"), oVal("ID"))
                Next

            End If

            Dim documentTypes = DocumentsServiceMethods.GetTemplateDocumentsList(True, DocumentScope.EmployeeContract, Me.Page, True)

            For Each documentType In documentTypes
                cmbEmployeeDocumentTemplate.Items.Add(documentType.Name, documentType.Id)
            Next

        End If

    End Sub

    Private Sub Base_WebUserControls_roDestinationSelectorV2_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("frmAddDestinationV2", "~/Base/Scripts/frmAddDestinationV2.js")
    End Sub

End Class