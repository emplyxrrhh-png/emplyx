Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Wizards_SelectTaskFieldsWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmFinish
    End Enum

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get

            Dim oFrames As New Generic.List(Of Frame)
            If Me.hdnFrames.Value = "" Then
                oFrames.Add(Frame.frmFinish)
            Else

                For Each strItem As String In Me.hdnFrames.Value.Split("*")
                    oFrames.Add(strItem)
                Next

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            Me.hdnFrames.Value = ""
            Me.hdnFramesOnlyClient.Value = ""
            If value IsNot Nothing Then
                For Each oItem As Frame In value
                    Me.hdnFrames.Value &= "*" & oItem
                    Select Case oItem
                        Case Else
                            Me.hdnFramesOnlyClient.Value &= "*0"
                    End Select
                Next
                If Me.hdnFrames.Value <> "" Then Me.hdnFrames.Value = Me.hdnFrames.Value.Substring(1)
                If Me.hdnFramesOnlyClient.Value <> "" Then Me.hdnFramesOnlyClient.Value = Me.hdnFramesOnlyClient.Value.Substring(1)
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            If Me.GetFeaturePermission("Tasks.TemplatesDefinition") > Permission.None Then
                Me.Frames = Nothing
                Me.Frames = Me.Frames
                Me.oActiveFrame = Frame.frmFinish
                Dim parentID As Integer = roTypes.Any2Integer(Request("ParentId"))
                Dim isProject As Boolean = roTypes.Any2Boolean(Request("IsProject"))
                Dim selectedNodes As String = roTypes.Any2String(Request("SelectedNodes"))
                Dim objectId As Integer = roTypes.Any2Integer(Request("ID"))
                loadAvailableFields(objectId, isProject, selectedNodes, parentID)
            Else
                WLHelperWeb.RedirectAccessDenied(True)
            End If
        Else

            If Me.Request("action") = "CheckFrame" Then
            Else

                Me.oActiveFrame = Me.FrameByIndex(Me.hdnActiveFrame.Value)
                Dim oDiv As HtmlControl
                For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                    oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                    If n = Me.FrameIndex(Me.oActiveFrame) Then
                        oDiv.Style("display") = "block"
                    Else
                        oDiv.Style("display") = "none"
                    End If
                Next

            End If

        End If

    End Sub

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckFrame(Me.oActiveFrame) Then
            Dim oTaskFields As New Generic.List(Of Object)

            For Each cNode As TreeNode In Me.treeTaskFields.Nodes
                If (cNode.Checked And cNode.Value <> "-1") Then
                    Dim oEmpField As TaskFieldTemplateStructField
                    Dim oEmp As New Generic.List(Of TaskFieldTemplateStructField)

                    oEmpField = New TaskFieldTemplateStructField
                    oEmpField.attname = "idtasktemplatefield"
                    oEmpField.value = cNode.Value
                    oEmp.Add(oEmpField)

                    oEmpField = New TaskFieldTemplateStructField
                    oEmpField.attname = "name"
                    oEmpField.value = cNode.Text
                    oEmp.Add(oEmpField)

                    oTaskFields.Add(oEmp)
                End If
            Next

            Dim selectedFields As Integer = 0
            Dim strJSON As String = "{rows : [ "
            For Each oObj As Object In oTaskFields
                selectedFields = selectedFields + 1
                strJSON &= " {fields:"
                Dim oEmpFld As Generic.List(Of TaskFieldTemplateStructField)
                oEmpFld = CType(oObj, Generic.List(Of TaskFieldTemplateStructField))
                strJSON &= roJSONHelper.Serialize(oEmpFld) & "} ,"
            Next
            strJSON = strJSON.Substring(0, Len(strJSON) - 2) & "]}"

            If selectedFields > 0 Then
                hdnParams_PageBase.Value = strJSON
            Else
                hdnParams_PageBase.Value = ""
            End If

            Me.MustRefresh = "1"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
        End If

    End Sub

    Protected Sub btClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btClose.Click
        hdnParams_PageBase.Value = ""
        Me.MustRefresh = "0"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
    End Sub

#End Region

#Region "Methods"

    Private Sub loadAvailableFields(ByVal IdObject As Integer, ByVal isProject As Boolean, ByVal selectedNodes As String, ByVal parentId As Integer)
        Me.treeTaskFields.Nodes.Clear()
        Dim oFields As DataTable = Nothing
        If (isProject) Then
            oFields = API.UserFieldServiceMethods.GetAvailableProjectTemplateFieldsDataSet(Me.Page, IdObject)
        Else
            oFields = API.UserFieldServiceMethods.GetAvailableTaskTemplateFieldsDataSet(Me.Page, IdObject)
            If (IdObject <= 0) Then
                Dim parentFields As DataTable = API.UserFieldServiceMethods.GetProjectTemplateFieldsDataSet(Me.Page, parentId)
                For Each cRow As DataRow In parentFields.Rows
                    Dim maxCount As Integer = oFields.Rows.Count
                    For index As Integer = 0 To maxCount - 1
                        If (roTypes.Any2Integer(oFields.Rows(index)("IDField")) = roTypes.Any2Integer(cRow("IDField"))) Then
                            oFields.Rows.RemoveAt(index)
                            maxCount = maxCount - 1
                            Exit For
                        End If
                    Next
                Next
            End If
        End If

        Dim sNodes As New Generic.List(Of String)
        sNodes.AddRange(selectedNodes.Split(","))

        Dim countFields As Integer = 0
        For Each oRow As DataRow In oFields.Rows
            Dim idField As String = roTypes.Any2String(oRow("IDField"))
            If Not sNodes.Contains(idField) Then
                Dim oNode As TreeNode = New TreeNode(roTypes.Any2String(oRow("FieldName")), idField)
                Me.treeTaskFields.Nodes.Add(oNode)
                countFields = countFields + 1
            End If
        Next

        If countFields = 0 Then
            Dim oNode As TreeNode = New TreeNode(Me.Language.Translate("SelectNewFields.NoFieldsAvailable", DefaultScope), "-1")
            oNode.Checked = True
            Me.treeTaskFields.Nodes.Add(oNode)
            Me.treeTaskFields.Enabled = False
            Me.btEnd.Visible = False
        End If

    End Sub

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = 0
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                intRet = n
                Exit For
            End If
        Next
        Return intRet
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal Frame As Frame) As Boolean

        Dim strMsg As String = ""
        Dim bolRet As Boolean = True

        Select Case Frame
            Case Wizards_SelectTaskFieldsWizard.Frame.frmFinish
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg
        End Select

        Return bolRet

    End Function

#End Region

End Class

Public Class TaskFieldTemplateStructField
    Public attname As String
    Public value As String
End Class