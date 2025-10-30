Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Security_PassportsBusinesGroups
    Inherits PageBase

    Private IdPassport As Integer
    Private Seleccion As String

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmBusinessGroup.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me.Page, roTypes.Any2Integer(Request.Params("idPassport")), LoadType.Passport)

            Me.ViewState("IdPassport") = oPassport.IDParentPassport
            Me.IdPassport = oPassport.IDParentPassport

            Me.ViewState("Seleccion") = roTypes.Any2Integer(Request.Params("seleccion"))
            Me.Seleccion = roTypes.Any2String(Request.Params("seleccion"))

            Dim strImagen = "~/Security/Images/Features/Access_InheritedRead.png"

            Dim tbShiftsBusinessGroup As DataTable = API.ShiftServiceMethods.GetBusinessGroupFromShiftGroups(Me)

            Dim tbCausesBusinessGroup As DataTable = CausesServiceMethods.GetBusinessGroupFromCauseGroups(Me)

            Dim tbConceptsGroupsBusinessGroup As DataTable = API.ConceptsServiceMethods.GetBusinessGroupFromConceptGroups(Me)

            InitList(tbShiftsBusinessGroup, tbCausesBusinessGroup, tbConceptsGroupsBusinessGroup, "BusinessGroup", "BusinessGroup", strImagen)
            LoadList(Me.Seleccion.Trim())

        End If

    End Sub

    Private Sub InitList(ByVal tbShiftsData As DataTable, ByVal tbCausesData As DataTable, ByVal tbConceptGroupsData As DataTable, ByVal strIDField As String, ByVal strNameField As String, ByVal strImage As String)

        Dim oNode As TreeNode
        Dim strText As String
        Me.treeConcepts.Nodes.Clear()

        Dim lstAddedItems As New Generic.List(Of String)

        For Each oRow As DataRow In tbShiftsData.Rows
            strText = oRow(strNameField)
            If Not lstAddedItems.Contains(strText) Then
                oNode = New TreeNode(strText, strText, strImage)
                Me.treeConcepts.Nodes.Add(oNode)
                lstAddedItems.Add(strText)
            End If
        Next

        For Each oRow As DataRow In tbCausesData.Rows
            strText = oRow(strNameField)
            If Not lstAddedItems.Contains(strText) Then
                oNode = New TreeNode(strText, strText, strImage)
                Me.treeConcepts.Nodes.Add(oNode)
                lstAddedItems.Add(strText)
            End If
        Next

        For Each oRow As DataRow In tbConceptGroupsData.Rows
            strText = oRow(strNameField)
            If Not lstAddedItems.Contains(strText) Then
                oNode = New TreeNode(strText, strText, strImage)
                Me.treeConcepts.Nodes.Add(oNode)
                lstAddedItems.Add(strText)
            End If
        Next

    End Sub

    Private Sub LoadList(ByVal strValues As String)
        If strValues.Length > 0 Then

            Dim arrValues As Generic.List(Of String) = New Generic.List(Of String)(strValues.Split(New Char() {";"}, System.StringSplitOptions.RemoveEmptyEntries))
            For n As Integer = 0 To arrValues.Count - 1
                arrValues(n) = arrValues(n).Trim()
            Next
            If arrValues.Count > 0 Then
                For Each oNode As TreeNode In Me.treeConcepts.Nodes
                    If arrValues.Contains(oNode.Value) Then
                        oNode.Checked = True
                    Else
                        oNode.Checked = False
                    End If
                Next
            End If
        End If
    End Sub

    Private Function GetList() As String

        Dim sValue As String = ""

        For Each oNode As TreeNode In Me.treeConcepts.Nodes
            If oNode.Checked Then
                sValue = sValue & oNode.Value & ";"
            End If
        Next
        If sValue.Length > 0 Then sValue = sValue.Substring(0, sValue.Length() - 1)

        Return sValue

    End Function

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        'If Me.oPermission >= Permission.Write Then

        'Dim strErrorInfo As String = ""

        'If Me.MediosAccesoLocal.SaveData(Me.intIDEmployee) Then
        'If Me.cnIdentifyMethods.SaveData(strErrorInfo) Then

        Me.MustRefresh = "7"
        Me.CanClose = True

        Dim strSeleccion As String = GetList()

        Me.IdPassport = Me.ViewState("IdPassport")

        Dim bRetorna As Boolean = True

        'Else
        'HelperWeb.ShowMessage(Me.Page, strErrorInfo)
        'End If

        'Else
        'WLHelperWeb.RedirectAccessDenied(True)
        'End If

    End Sub

End Class