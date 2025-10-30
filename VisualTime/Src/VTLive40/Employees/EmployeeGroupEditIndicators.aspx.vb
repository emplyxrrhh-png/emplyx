Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class EmplyeeGroupIndicators
    Inherits PageBase

    Private GroupId As Integer
    Private Seleccion As String

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            Me.ViewState("GroupId") = roTypes.Any2Integer(Request.Params("GroupId"))
            Me.GroupId = roTypes.Any2Integer(Request.Params("GroupId"))

            Dim tbIndicators As DataTable = API.IndicatorsServiceMethods.GetIndicatorsDataTable(IndicatorsType.Attendance, Me.Page, "", False)
            If tbIndicators IsNot Nothing Then
                InitList(tbIndicators)

                Dim tbGroupIndicators As DataTable = API.EmployeeGroupsServiceMethods.GetIndicatorsDataset(Me.Page, Me.GroupId).Tables(0)
                If tbGroupIndicators IsNot Nothing Then
                    LoadList(tbGroupIndicators)
                End If
            End If
        End If

    End Sub

    Private Sub InitList(ByVal tbData As DataTable)
        Dim oNode As TreeNode
        Dim strText As String
        Me.treeGroupIndicators.Nodes.Clear()
        For Each oRow As DataRow In tbData.Rows
            strText = oRow("Name")
            oNode = New TreeNode(roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("ID")))
            Me.treeGroupIndicators.Nodes.Add(oNode)
        Next
    End Sub

    Private Sub LoadList(ByVal tbGroupIndicators As DataTable)
        Dim indicatorsIDs As Generic.List(Of Integer) = New Generic.List(Of Integer)
        For Each cRow As DataRow In tbGroupIndicators.Rows
            indicatorsIDs.Add(roTypes.Any2Integer(cRow("IDIndicator")))
        Next
        If tbGroupIndicators.Rows.Count > 0 Then
            For Each oNode As TreeNode In Me.treeGroupIndicators.Nodes
                If Array.IndexOf(indicatorsIDs.ToArray, roTypes.Any2Integer(oNode.Value)) < 0 Then
                    oNode.Checked = False
                Else
                    oNode.Checked = True
                End If
            Next
        End If
    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        Me.MustRefresh = "7"
        Me.CanClose = True

        Dim lstIndicatorsIds As New Generic.List(Of Integer)
        For Each oNode As TreeNode In Me.treeGroupIndicators.Nodes
            If oNode.Checked Then
                lstIndicatorsIds.Add(oNode.Value)
            End If
        Next

        Me.GroupId = Me.ViewState("GroupId")

        Dim bRetorna As Boolean = API.EmployeeGroupsServiceMethods.SaveIndicators(Me.Page, Me.GroupId, lstIndicatorsIds, False)

    End Sub

End Class