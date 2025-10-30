Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class SecurityFunctionsSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconNode As String = "SecurityFunctions16.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterPassportEmployee As Boolean = True

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
            Me.strImagesPath = "../../images/SecurityFunctionsSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" AndAlso IsNumeric(Me.Request("node")) Then
            Me.intIDParent = CInt(Me.Request("node"))
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            'linea original-> ppr If Filters.Substring(0, 1) = "1" Then Me.bolFilterPassportEmployee = True Else Me.bolFilterPassportEmployee = False
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterPassportEmployee = False Else Me.bolFilterPassportEmployee = True
        End If

        If ViewState("SecurityFunctions_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("SecurityFunctions_Selection")
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

                LoadPassportsTree()

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

    Private Sub LoadPassportsTree()

        ' Verificamos que el passport activo como mínimo tiene permisos de lectura
        If API.SecurityServiceMethods.HasPermissionOverFeature(Me, "Administration.Security", "U", Permission.Read) Then

            Dim oSecurityFuncions As roGroupFeature() = API.SecurityChartServiceMethods.GetGroupFeatures(Me.Page)
            ' Obtenemos los grupos

            If oSecurityFuncions IsNot Nothing AndAlso oSecurityFuncions.Length > 0 Then
                For Each oFeature As roGroupFeature In oSecurityFuncions
                    strArrayNodes &= "{ 'id':'" & oFeature.ID & "', 'text':'" & oFeature.Name.Replace("'", "&#39;") & "', " &
                                     "'leaf':true, 'icon': '" & Me.strImagesPath & Me.strIconNode & "','draggable':false"
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