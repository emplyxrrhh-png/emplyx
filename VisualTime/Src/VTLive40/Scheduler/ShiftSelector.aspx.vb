Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class ShiftSelector
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"
    Private intIDEmployee As Integer
    Private intIDShift As Integer

    Private oPermission As Permission

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Dim IDEmployee As String = Request.Params("IDEmployee")
        If IDEmployee IsNot Nothing AndAlso IDEmployee.Length > 0 Then
            Me.intIDEmployee = roTypes.Any2Integer(IDEmployee)
        End If

        Dim IDShift As String = Request.Params("IDShift")
        If IDShift IsNot Nothing AndAlso IDShift.Length > 0 Then
            Me.intIDShift = roTypes.Any2Integer(IDShift)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            If Not Me.IsPostBack Then

                Me.LoadData()

                Me.SelectShift(Me.intIDShift)

            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btAccept.Click

        Dim bolCorrect As Boolean = False
        If treeShifts.SelectedNode IsNot Nothing AndAlso Me.treeShifts.SelectedNode.ChildNodes.Count = 0 AndAlso treeShifts.SelectedNode.Value.IndexOf("*") > 0 Then
            hdnIDShiftSelected.Value = treeShifts.SelectedNode.Value.Split("*")(0)
            hdnStartShift.Value = treeShifts.SelectedNode.Value.Split("*")(1)
            hdnShowAssignmentSelector.Value = "0"
            Dim oAssignments As List(Of roAssignment) = API.SchedulerServiceMethods.GetEmployeAndShiftAssignments(Me, Me.intIDEmployee, Me.hdnIDShiftSelected.Value)
            If oAssignments IsNot Nothing Then
                If oAssignments.Count = 1 Then
                    hdnIDAssignmentSelected.Value = oAssignments(0).ID
                ElseIf oAssignments.Count > 0 Then
                    hdnShowAssignmentSelector.Value = "1"
                    hdnIDAssignmentSelected.Value = "-1"
                    cmbAssignmentSelectorNew.DataSource = oAssignments
                    cmbAssignmentSelectorNew.ValueField = "ID"
                    cmbAssignmentSelectorNew.TextField = "Name"
                    cmbAssignmentSelectorNew.DataBind()
                End If
            End If

            bolCorrect = True

        End If

        If bolCorrect Then
            If Me.hdnStartShift.Value = "" And Me.hdnShowAssignmentSelector.Value = "0" Then Me.hdnCanClose.Value = "1"
        Else
            Me.hdnCanClose.Value = "0"
            Me.lblError.Text = Me.Language.Translate("InvalidSelection.Message", Me.DefaultScope)
            Me.lblError.Visible = True
        End If

    End Sub

#End Region

#Region "Methods"

    Public Sub LoadData()

        Dim tbGroups As DataTable = API.ShiftServiceMethods.GetShiftGroupsPlanification(Me)

        Dim tbShifts As DataTable

        Dim oGroupNode As TreeNode
        Dim oShiftNode As TreeNode
        Dim strName As String
        For Each oGroup As DataRow In tbGroups.Rows

            If oGroup("ID") = 0 Then
                strName = Me.Language.Translate("Shifts.ShiftsMainGroup.literal", Me.DefaultScope)
            ElseIf Not IsDBNull(oGroup("Name")) Then
                strName = oGroup("Name")
            Else
                strName = ""
            End If
            oGroupNode = New TreeNode(strName, oGroup("ID"), "Images/TABLE_PROPERTIES_CLOCK_16.GIF")

            tbShifts = API.ShiftServiceMethods.GetShiftsPlanification(Me, oGroup("ID"))

            Dim strID As String = ""
            For Each oShift As DataRow In tbShifts.Rows

                If Not roTypes.Any2Boolean(oShift("AllowComplementary")) AndAlso Not roTypes.Any2Boolean(oShift("AllowFloatingData")) AndAlso Not roTypes.Any2String(oShift("AdvancedParameters")) = "Starter=[1]" Then
                    strID = oShift("ID") & "*"
                    If API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultipleShifts") Then
                        If roTypes.Any2Boolean(oShift("IsFloating")) Then
                            If Not IsDBNull(oShift("StartFloating")) Then
                                strID &= Format(CDate(oShift("StartFloating")), "yyyyMMddHHmm")
                            Else
                                strID &= "189912300000"
                            End If
                        End If
                    End If
                    oShiftNode = New TreeNode(oShift("Name"), strID, "Images/TABLE_PROPERTIES_CLOCK_16.GIF")
                    oGroupNode.ChildNodes.Add(oShiftNode)
                End If

            Next

            If tbShifts.Rows.Count > 0 Then
                Me.treeShifts.Nodes.Add(oGroupNode)
            End If
        Next

        Me.cmbStartFloating.ClearItems()
        Me.cmbStartFloating.AddItem(Me.Language.Translate("StartFloating.ShiftDay", "Shift"), "1", "")
        Me.cmbStartFloating.AddItem(Me.Language.Translate("StartFloating.ShiftAfter", "Shift"), "2", "")
        Me.cmbStartFloating.AddItem(Me.Language.Translate("StartFloating.ShiftBefore", "Shift"), "0", "")

    End Sub

    Private Sub SelectShift(ByVal _IDShift As Integer)

        Dim bolSelected As Boolean = False

        For Each oNode As TreeNode In Me.treeShifts.Nodes
            For Each oChild As TreeNode In oNode.ChildNodes
                If oChild.Value.Split("*")(0) = _IDShift Then
                    oChild.Selected = True
                    bolSelected = True
                    Exit For
                End If
            Next
            If bolSelected Then Exit For
        Next

    End Sub

    ''#Region "WebServices methods"

    ''    Private Function GetShiftGroups() As DataTable

    ''        Dim oRet As DataTable = Nothing

    ''        If Me.oShiftService Is Nothing Then Me.oShiftService = WebServices.ShiftService
    ''        Dim oState As New ShiftService.roShiftState
    ''        oState.IDPassport = WLHelperWeb.CurrentPassport.ID

    ''        Try

    ''            Dim ds As DataSet = Me.oShiftService.GetShiftGroups(oState)
    ''            If oState.Result = CostCenterService.ResultEnum.NoError Then
    ''                If ds.Tables.Count > 0 Then
    ''                    oRet = ds.Tables(0)
    ''                End If
    ''            Else
    ''                ' Mostrar el error
    ''                HelperWeb.ShowError(Me, oState)
    ''            End If

    ''        Catch ex As Exception
    ''            ' Mostrar la exception
    ''            HelperWeb.ShowMessage(Me, ex.Message)
    ''        End Try

    ''        Return oRet

    ''    End Function

    ''    Private Function GetShifts(ByVal intIDShiftGroup As Integer) As DataTable

    ''        Dim oRet As DataTable = Nothing

    ''        If Me.oShiftService Is Nothing Then Me.oShiftService = WebServices.ShiftService
    ''        Dim oState As New ShiftService.roShiftState
    ''        oState.IDPassport = WLHelperWeb.CurrentPassport.ID

    ''        Try

    ''            Dim ds As DataSet = Me.oShiftService.GetShifts(intIDShiftGroup, oState, True)
    ''            If oState.Result = CostCenterService.ResultEnum.NoError Then
    ''                If ds.Tables.Count > 0 Then
    ''                    oRet = ds.Tables(0)
    ''                End If
    ''            Else
    ''                ' Mostrar el error
    ''                HelperWeb.ShowError(Me, oState)
    ''            End If

    ''        Catch ex As Exception
    ''            ' Mostrar la exception
    ''            HelperWeb.ShowMessage(Me, ex.Message)
    ''        End Try

    ''        Return oRet

    ''    End Function

    ''#End Region

#End Region

End Class