Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class PassportsAuthorizations
    Inherits PageBase

    Private IdPassport As Integer
    Private Seleccion As String

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmAuthorization.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me.Page, roTypes.Any2Integer(Request.Params("idPassport")), LoadType.Passport)

            Me.ViewState("IdPassport") = oPassport.IDParentPassport
            Me.IdPassport = oPassport.IDParentPassport

            Dim dTAccessGroups As DataTable = AccessGroupServiceMethods.GetAccessGroups(Me.Page)
            If dTAccessGroups IsNot Nothing Then
                InitList(dTAccessGroups)

                Dim arrAccessGroups() As Integer = AccessGroupServiceMethods.GetAccessGroupsByPassport(Me.Page, IdPassport)
                If arrAccessGroups IsNot Nothing Then
                    LoadList(arrAccessGroups)
                End If
            End If
        End If
    End Sub

    Private Sub InitList(ByVal tbData As DataTable)
        'Dim strImagen = "~/Security/Images/Features/Access_InheritedRead.png"
        Dim oNode As TreeNode
        Dim strText As String
        Me.treeAccessGroups.Nodes.Clear()
        For Each oRow As DataRow In tbData.Rows
            strText = oRow("Name")
            oNode = New TreeNode(roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("ID")))
            Me.treeAccessGroups.Nodes.Add(oNode)
        Next
    End Sub

    Private Sub LoadList(ByVal arrAccessGroups() As Integer)
        Dim allChecked As Boolean = True
        Dim noneChecked As Boolean = True
        If arrAccessGroups.Length > 0 Then
            For Each oNode As TreeNode In Me.treeAccessGroups.Nodes
                If Array.IndexOf(arrAccessGroups, roTypes.Any2Integer(oNode.Value)) < 0 Then
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

        Dim lstAccessGroups As New Generic.List(Of Integer)
        For Each oNode As TreeNode In Me.treeAccessGroups.Nodes
            If oNode.Checked Then
                lstAccessGroups.Add(oNode.Value)
            End If
        Next

        Me.IdPassport = Me.ViewState("IdPassport")

        Dim bRetorna As Boolean = AccessGroupServiceMethods.SaveAccessGroupByPassport(Me.Page, IdPassport, lstAccessGroups.ToArray(), False)

    End Sub

End Class