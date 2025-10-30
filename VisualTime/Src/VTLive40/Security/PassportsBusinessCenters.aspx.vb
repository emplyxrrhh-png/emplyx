Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Security_PassportsBusinessCenters
    Inherits PageBase

    Private IdPassport As Integer
    Private Seleccion As String

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmBusinessCenters.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me.Page, roTypes.Any2Integer(Request.Params("idPassport")), LoadType.Passport)

            Me.ViewState("IdPassport") = oPassport.ID
            Me.IdPassport = oPassport.ID

            Dim tbBusinessCenter As DataTable = API.TasksServiceMethods.GetBusinessCenters(Me.Page, False)
            If tbBusinessCenter IsNot Nothing Then
                InitList(tbBusinessCenter)

                Dim arrBusinessCenter() As Integer = API.TasksServiceMethods.GetBusinessCenterByPassport(Me.Page, IdPassport, False)
                If arrBusinessCenter IsNot Nothing Then
                    LoadList(arrBusinessCenter)
                End If
            End If
        End If

    End Sub

    Private Sub InitList(ByVal tbData As DataTable)
        'Dim strImagen = "~/Security/Images/Features/Access_InheritedRead.png"
        Dim oNode As TreeNode
        Dim strText As String
        Me.treeBusinessCenters.Nodes.Clear()
        For Each oRow As DataRow In tbData.Rows
            strText = oRow("Name")
            oNode = New TreeNode(roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("ID")))
            Me.treeBusinessCenters.Nodes.Add(oNode)
        Next
    End Sub

    Private Sub LoadList(ByVal arrBusinessCenter() As Integer)
        Dim allChecked As Boolean = True
        Dim noneChecked As Boolean = True
        If arrBusinessCenter.Length > 0 Then
            For Each oNode As TreeNode In Me.treeBusinessCenters.Nodes
                If Array.IndexOf(arrBusinessCenter, roTypes.Any2Integer(oNode.Value)) < 0 Then
                    oNode.Checked = False
                    allChecked = False
                Else
                    oNode.Checked = True
                    noneChecked = False
                End If
            Next
        End If
    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        Me.MustRefresh = "7"
        Me.CanClose = True

        Dim lstBusinessCenter As New Generic.List(Of Integer)
        For Each oNode As TreeNode In Me.treeBusinessCenters.Nodes
            If oNode.Checked Then
                lstBusinessCenter.Add(oNode.Value)
            End If
        Next

        Me.IdPassport = Me.ViewState("IdPassport")

        Dim bRetorna As Boolean = API.TasksServiceMethods.SaveBusinessCenterByPassport(Me.Page, IdPassport, lstBusinessCenter.ToArray(), False)

    End Sub

End Class