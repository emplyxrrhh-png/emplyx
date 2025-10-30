Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Causes_WebUserForms_frmDocumentTrace
    Inherits UserControlBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then

            cmbLabAgree.Items.Add(Me.Language.Translate("AnyLabAgree", DefaultScope), "0")
            Dim tb As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)
            If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                For Each dRow As DataRow In tb.Rows
                    cmbLabAgree.Items.Add(dRow("Name"), dRow("ID"))
                Next
            End If
            cmbLabAgree.SelectedIndex = 0

            cmbDocument.Items.Add("", "")

            Dim oselectingList As List(Of roDocumentTemplate) = DocumentsServiceMethods.GetDocumentTemplateListbyType(DocumentType.Employee, Page, False)

            If oselectingList IsNot Nothing Then
                oselectingList = oselectingList.FindAll(Function(x) x.Scope = DocumentScope.LeaveOrPermission OrElse x.Scope = DocumentScope.CauseNote)

                Me.cmbDocument.Items.Clear()
                For Each oDocument In oselectingList
                    Me.cmbDocument.Items.Add(oDocument.Name, oDocument.Id & "_" & CInt(oDocument.Scope))
                Next
            End If

            'tb = API.DocumentsAbsencesServiceMethods.GetDocumentsAbsences(Me.Page)
            'If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
            '    For Each dRow As DataRow In tb.Rows
            '        cmbDocument.Items.Add(dRow("Name"), dRow("ID"))
            '    Next
            'End If
            cmbDocument.SelectedIndex = 0

            cmbDocumentFirstTime.Items.Add(Me.Language.Translate("DocumentFirstTime.Start", "DocumentAbsences"), "0")
            cmbDocumentFirstTime.Items.Add(Me.Language.Translate("DocumentFirstTime.End", "DocumentAbsences"), "1")
            cmbDocumentFirstTime.Items.Add(Me.Language.Translate("DocumentFirstTime.AfterStart", "DocumentAbsences"), "2")
            cmbDocumentFirstTime.Items.Add(Me.Language.Translate("DocumentFirstTime.AfterEnded", "DocumentAbsences"), "3")
            cmbDocumentFirstTime.SelectedIndex = 0

            'cmbDayWeekMonth.Items.Add(Me.Language.Translate("DocumentNextTime.Day", "DocumentAbsences"), "0")
            'cmbDayWeekMonth.Items.Add(Me.Language.Translate("DocumentNextTime.Week", "DocumentAbsences"), "1")
            'cmbDayWeekMonth.Items.Add(Me.Language.Translate("DocumentNextTime.Month", "DocumentAbsences"), "2")
            'cmbDayWeekMonth.SelectedIndex = 0

            'cmbBeginEndNext.Items.Add(Me.Language.Translate("DocumentNextTime.Ini", "DocumentAbsences"), "0")
            'cmbBeginEndNext.Items.Add(Me.Language.Translate("DocumentNextTime.Fin", "DocumentAbsences"), "1")
            'cmbBeginEndNext.SelectedIndex = 0

        End If

    End Sub

End Class