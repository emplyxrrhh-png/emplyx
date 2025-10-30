Imports Robotics.Web.Base

Partial Class SchemaSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconSchema As String = "SchemaIco.png"
    Private strIconSchemaOld As String = "SchemaIco-disabled.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterSchema As Boolean = True

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
            Me.strImagesPath = "../../images/SchemaSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" AndAlso Me.Request("node") <> "null" Then
            If Me.Request("node").StartsWith("A") Then
                Me.intIDParent = CInt(Me.Request("node").Substring(1))
            Else
                If IsNumeric(Me.Request("node").Substring(1)) Then
                    Me.intIDParent = CInt(Me.Request("node").Substring(1))
                End If
            End If
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterSchema = True Else Me.bolFilterSchema = False
        End If

        If ViewState("SchemaSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("SchemaSelector_Selection")
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

                LoadSchemasTree()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                If Me.strArrayNodes = "" Then Me.strArrayNodes = "[]"
                Me.Response.Write(Me.strArrayNodes)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadSchemasTree()

        If intIDParent.HasValue Then
            Dim dTbl As DataTable = API.ShiftServiceMethods.GetSchemas(Me.Page, True)
            If dTbl.Rows.Count > 0 Then
                For Each dRow As DataRow In dTbl.Rows
                    Dim strIco As String = strIconSchema

                    strArrayNodes &= "{ 'id':'" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                     "'leaf': true, " &
                                     "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                Next
            End If
        Else
            ' Cargamos solo los del Grupo 0 en la raiz (General)
            Dim dTbl As DataTable = API.ShiftServiceMethods.GetSchemas(Me.Page, True)

            If dTbl.Rows.Count > 0 Then
                For Each dRow As DataRow In dTbl.Rows
                    Dim strIco As String = strIconSchema

                    strArrayNodes &= "{ 'id':'" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                     "'leaf': true, " &
                                     "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                Next
            End If
        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

#End Region

End Class