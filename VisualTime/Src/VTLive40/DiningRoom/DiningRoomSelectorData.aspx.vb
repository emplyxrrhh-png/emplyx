Imports Robotics.Web.Base

Partial Class DiningRoomSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private strIDParent As String = ""
    Private strIconDiningRoom As String = "DiningRoom_16.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterDiningRoom As Boolean = True

    Private lstSelection As New ArrayList

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Obtengo el parámetro de la acción a realizar
        Dim Action As String = Request.Params("action")
        If Action IsNot Nothing Then
            Me.strAction = Action
        End If

        ' Lectura parámetros página
        Dim OnlyGroups As String = Request.Params("OnlyGroups")
        If OnlyGroups IsNot Nothing AndAlso OnlyGroups.Length = 1 Then
            Me.bolOnlyGroups = (OnlyGroups = "1")
        End If

        Dim MultiSelect As String = Request.Params("MultiSelect")
        If MultiSelect IsNot Nothing AndAlso MultiSelect.Length = 1 Then
            Me.bolMultiSelect = (MultiSelect = "1")
        End If

        Dim ImagesPath As String = Request.Params("ImagesPath")
        If ImagesPath IsNot Nothing Then
            Me.strImagesPath = ImagesPath
        Else
            Me.strImagesPath = "../../images/DiningRoomSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" Then
            strIDParent = Me.Request("node")
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterDiningRoom = True Else Me.bolFilterDiningRoom = False
        End If

        If Session("DiningRoomSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = Session("DiningRoomSelector_Selection")
            If strSelection <> "" Then
                For Each s As String In strSelection.Split(",")
                    Me.lstSelection.Add(s.Trim)
                Next
            Else
                Me.lstSelection.Clear()
            End If
        End If

        Select Case Me.strAction
            Case "TreeData" ' Obtiene los nodos del árbol del nivel indicado (strParent)

                LoadDiningRoomTree()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write(Me.strArrayNodes)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadDiningRoomTree()

        Dim dTbl As DataTable = API.DiningRoomServiceMethods.GetDiningRoomTurns(Me.Page)

        If dTbl.Rows.Count > 0 Then
            For Each dRow As DataRow In dTbl.Rows
                'If "A" & dRow("IDParent") = strIDParent Then
                strArrayNodes &= "{ 'id':'" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                 "'leaf': true, " &
                                 "'icon': '" & Me.strImagesPath & strIconDiningRoom & "'"
                strArrayNodes &= "},"
                'End If
            Next
        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

#End Region

End Class