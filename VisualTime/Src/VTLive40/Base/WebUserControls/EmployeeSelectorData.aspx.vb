Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_get_nodes
    Inherits NoCachePageBase

    Private strAction As String = "TreeData"

    Private strUserField As String = ""

    Private strParent As String = ""
    Private strParentType As String = ""
    Private strIconGroup As String = "Grupos-16x16.Gif"
    Private strIconCompany As String = "Company_16.png"

    Private strFeatureAlias As String = "" ' Nombre funcionalidad para aplicar seguridad (Employees, Calendar)
    Private strFeatureType As String = "U"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterCurrent As Boolean = True
    Private bolFilterMovility As Boolean = True
    Private bolFilterOld As Boolean = True
    Private bolFilterFuture As Boolean = True
    Private bolFilterUserFields As Boolean = False

    Private strFilterUserFields As String = ""

    Private strFieldFindColumn As String = ""
    Private strFieldFindValue As String = ""

    Private lstSelection As New ArrayList

    Private ReloadGroups As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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
            Me.strImagesPath = "../images/EmployeeSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        Me.strFeatureAlias = Request("FeatureAlias")
        Me.strFeatureType = Request("FeatureType")

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node").Length > 1 AndAlso Me.Request("node") <> "null" AndAlso Me.Request("node") <> "undefined" Then
            Me.strParentType = Me.Request("node").Substring(0, 1)
            Me.strParent = Me.Request("node").Substring(1)
            If Me.strParentType = "C" Then
                If Me.strParent.Split("~").Length > 1 Then Me.strParent = Me.strParent.Split("~")(1)
            End If
        End If

        'recargar grupos de empleados de la BBDD en lugar de optenerlos de la Application
        Me.ReloadGroups = roTypes.Any2Boolean(Me.Request("ReloadGroups"))

        ' Obtenemos configuración de los filtros des de parámetros url
        'TODO: Modificat per poder passar de dues maneres els filtres
        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterCurrent = True Else Me.bolFilterCurrent = False
            If Filters.Substring(1, 1) = "1" Then Me.bolFilterMovility = True Else Me.bolFilterMovility = False
            If Filters.Substring(2, 1) = "1" Then Me.bolFilterOld = True Else Me.bolFilterOld = False
            If Filters.Substring(3, 1) = "1" Then Me.bolFilterFuture = True Else Me.bolFilterFuture = False
            If Filters.Substring(4, 1) = "1" Then Me.bolFilterUserFields = True Else Me.bolFilterUserFields = False
        Else
            Filters = Request.Params("FilterCurrent")
            If Filters IsNot Nothing AndAlso Filters.Length = 1 Then
                Me.bolFilterCurrent = (Filters = "1")
            End If
            Filters = Request.Params("FilterMovility")
            If Filters IsNot Nothing AndAlso Filters.Length = 1 Then
                Me.bolFilterMovility = (Filters = "1")
            End If
            Filters = Request.Params("FilterOld")
            If Filters IsNot Nothing AndAlso Filters.Length = 1 Then
                Me.bolFilterOld = (Filters = "1")
            End If
            Filters = Request.Params("FilterFuture")
            If Filters IsNot Nothing AndAlso Filters.Length = 1 Then
                Me.bolFilterFuture = (Filters = "1")
            End If
        End If
        If Me.bolFilterUserFields Then
            Filters = Request.Params("FilterUserFields")
            If Filters IsNot Nothing Then
                Filters = HttpUtility.UrlDecode(Filters)
                Filters = StringDecodeControlChars(Filters)
                Me.strFilterUserFields = ParseFilterUserFields(Filters)
            End If
        End If

        Dim UserField As String = Request.Params("UserField")
        If UserField IsNot Nothing Then
            Me.strUserField = UserField
        End If

        Dim FieldFind As String = Request.Params("FieldFindColumn")
        If FieldFind IsNot Nothing Then
            Me.strFieldFindColumn = FieldFind
        End If
        FieldFind = Request.Params("FieldFindValue")
        If FieldFind IsNot Nothing Then
            Me.strFieldFindValue = FieldFind
        End If

        If Session("EmployeeSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = Session("EmployeeSelector_Selection")
            If strSelection <> "" Then
                For Each s As String In strSelection.Split(",")
                    Me.lstSelection.Add(s.Trim)
                Next
            Else
                Me.lstSelection.Clear()
            End If
        End If

        'Filtro personalizado al definir el control
        Dim strFiltro As String = roTypes.Any2String(Request.Params("FilterFixed"))
        If strFiltro <> String.Empty Then
            strFiltro = strFiltro.Replace("""", "'")
            strFiltro = StringDecodeControlChars(strFiltro)
            If Me.strFilterUserFields <> String.Empty Then
                Me.strFilterUserFields = Me.strFilterUserFields & " AND (" & strFiltro & ")"
            Else
                Me.strFilterUserFields = strFiltro
            End If
        End If

        Select Case Me.strAction
            Case "TreeData" ' Obtiene los nodos del árbol del nivel indicado (strParent)

                If Not Me.bolMultiSelect Then
                    LoadEmployeesTree()
                Else
                    LoadEmployeesTreeAll()
                End If

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                'Me.Response.Write("[{ 'id': 1, 'text': 'A folder Node', 'leaf': false }," & _
                '                   "{ 'id': 2, 'text': 'A leaf Node', 'leaf': true }]")
                If Me.strArrayNodes = "" Then Me.strArrayNodes = "[]"
                Me.Response.Write(Me.strArrayNodes)

            Case "FieldFindData"

                LoadEmployeesSearch()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                If Me.strArrayNodes = "" Then Me.strArrayNodes = "[]"
                Me.Response.Write(Me.strArrayNodes)

            Case "getSelectionPath" ' Obtiene la ruta del empleado seleccionado (en strParent).

                Me.Controls.Clear()

                Me.Response.Clear()
                Me.Response.ContentType = "text/plain"

                Dim strPath As String = ""

                If Me.strUserField = "" Then
                    ' Buscamos la ruta del grupo del empleado

                    If Me.strParent <> "" Then
                        Select Case Me.strParentType
                            Case "A"
                                strPath = Me.GetGroupPath(Me.strParent)
                            Case "B"
                                strPath = Me.GetEmployeeGroupPath(Me.strParent)
                        End Select
                    End If
                    'Select Case Me.strParent.Substring(0, 1)
                    '    Case "A"
                    '        strPath = Me.GetGroupPath(Me.strParent.Substring(1))
                    '    Case "B"
                    '        strPath = Me.GetEmployeeGroupPath(Me.strParent.Substring(1))
                    'End Select
                Else
                    If strParent <> "" Then
                        ' Buscamos la ruta en función del campo de la ficha (strUserField)
                        Dim dsUF As DataSet = EmployeeServiceMethods.GetUserFieldsDataset(Me.Page, Me.strParent)
                        If dsUF IsNot Nothing AndAlso dsUF.Tables.Count > 0 Then
                            Dim oRow() As DataRow = dsUF.Tables(0).Select("FieldName='" & Me.strUserField & "'")
                            If oRow.Length = 1 Then
                                If Not IsDBNull(oRow(0)("Value")) Then
                                    strPath = oRow(0)("Value")
                                End If
                            End If
                        End If
                    End If
                    If strPath <> "" Then
                        'strPath = "/C" & strPath.Replace("\", "/C")
                        strPath = strPath.Replace("\", "/C")
                        Dim Paths() As String = strPath.Split("/C")
                        If Paths.Length > 1 Then
                            Dim strPath2 As String = ""
                            strPath2 = "/C" & Paths(0)
                            For n As Integer = 1 To Paths.Length - 1
                                Paths(n) = Paths(n).Substring(1)
                                strPath2 &= "/C" & Paths(n - 1) & "#sep1#" & Paths(n)
                                Paths(n) = Paths(n - 1) & "#sep1#" & Paths(n)
                            Next
                            strPath = strPath2
                        Else
                            strPath = "/C" & strPath
                        End If

                    End If

                End If

                If Me.strUserField = "" Then
                    strPath = "/source" & strPath & "/" & Me.strParentType & Me.strParent
                Else
                    strPath = "/source" & strPath & "/B" & Me.strParent

                End If

                Me.Response.Write(strPath)

            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")

        End Select

    End Sub

    Public Sub LoadEmployeesTree()
        ' Funcion que carga el dataset con el árbol de empleados.

        Dim bolShowChilds As Boolean = True
        Dim x As New StringBuilder(strArrayNodes)

        If Me.strParent = "" Then

            If Me.strUserField = "" Then

                bolShowChilds = False

                Dim oDataTable As DataTable = HelperSession.GetEmployeeGroupsFromSession(Me.strFeatureAlias, Me.strFeatureType, ReloadGroups)
                If oDataTable IsNot Nothing Then
                    Dim oDataView As System.Data.DataView = New System.Data.DataView(oDataTable)
                    'oDataView.RowFilter = "Convert([ID], 'System.String') = Path"
                    oDataView.Sort = "Level DESC, Name"


                    Dim lNodePath As New List(Of String)
                    For Each oDataviewRow As Data.DataRowView In oDataView
                        lNodePath.Add(roTypes.Any2String(oDataviewRow("Path")))
                    Next

                    For Each oDataviewRow As Data.DataRowView In oDataView

                        Dim sParentNodePath As String = roTypes.Any2String(oDataviewRow("Path"))
                        If sParentNodePath.EndsWith($"\{oDataviewRow("ID")}") Then sParentNodePath = sParentNodePath.Substring(0, sParentNodePath.Length - oDataviewRow("ID").ToString.Length - 1)

                        If Not lNodePath.Contains(sParentNodePath) OrElse roTypes.Any2String(oDataviewRow("Path")).IndexOf("\") = -1 Then
                            Dim strIcon As String = Me.strIconGroup
                            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultiCompany") AndAlso roTypes.Any2String(oDataviewRow("Path")).IndexOf("\") = -1 Then strIcon = Me.strIconCompany
                            Dim strParentNode As String = String.Empty

                            If (roTypes.Any2String(oDataviewRow("FullGroupName")).IndexOf("\") >= 0) Then
                                Dim sDepNames As String() = roTypes.Any2String(oDataviewRow("Path")).Split("\")
                                Dim parentId As Integer = roTypes.Any2Integer(sDepNames(sDepNames.Length - 2))
                                Dim parentGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, parentId, False)
                                If parentGroup IsNot Nothing Then
                                    strParentNode = $"({IIf(parentGroup.Export <> String.Empty, parentGroup.Export, parentGroup.ID.ToString)} - {parentGroup.Name.Trim})"
                                End If
                            End If

                            x.Append("{ 'id':'A" & oDataviewRow("ID") & "', 'text':'" & $"{oDataviewRow("Name")} {strParentNode}".Replace("'", "&#39;") & "', 'leaf':false, 'icon': '" & Me.strImagesPath & strIcon & "'")
                            If Me.bolMultiSelect Then
                                x.Append(", 'checked2' : false")
                                If Me.lstSelection.Contains("A" & oDataviewRow("ID")) Then
                                    x.Append(", 'checked': true, 'expanded': true")
                                Else
                                    x.Append(", 'checked': false")
                                End If
                            End If
                            x.Append("},")
                        End If
                    Next
                    If Not Me.bolOnlyGroups AndAlso Me.strFeatureAlias <> String.Empty Then bolShowChilds = True
                End If
            Else

                Dim oDataTable As DataTable = UserFieldServiceMethods.GetUserFieldValues(Me, Me.strUserField, Types.EmployeeField, "", "\", Me.strFilterUserFields)
                If oDataTable IsNot Nothing Then


                    Dim oFieldDefinition As roUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, Me.strUserField.Replace("Usr_", ""), Types.EmployeeField, False, False)

                    ' Ordeno la lista que me ha devuelto el webservice y la ordeno por path
                    Dim oDataView As System.Data.DataView = New System.Data.DataView(oDataTable)
                    oDataView.Sort = "Path"

                    For Each oDataviewRow In oDataView
                        x.Append("{ 'id':'C" & Me.FilterSpecialChars(Me.strUserField) & "~" & oDataviewRow("Path").Replace("'", "#sep3#").Replace("\", "#sep1#") &
                                         "', 'text':'" & Me.FilterSpecialChars(FormatUserFieldValue(oFieldDefinition.FieldType, CStr(oDataviewRow("Name")))) & "', 'leaf':false, 'icon': '" & Me.strImagesPath & Me.strIconGroup & "'")
                        If Me.bolMultiSelect Then
                            x.Append(", 'checked2' : false")
                            If Me.lstSelection.Contains("C" & oDataviewRow("Path")) Then
                                x.Append(", 'checked': true, 'expanded': true")
                            Else
                                x.Append(", 'checked': false")
                            End If
                        End If
                        x.Append("},")
                    Next

                End If

            End If
        Else

            If Me.strUserField = "" Then
                Dim oDataTable As DataTable = HelperSession.GetEmployeeGroupsFromSession(Me.strFeatureAlias, Me.strFeatureType, Me.ReloadGroups)
                If oDataTable IsNot Nothing Then
                    Dim oDataView As System.Data.DataView = New System.Data.DataView(oDataTable)
                    oDataView.RowFilter = "Path LIKE '%\" & Me.strParent & "\' + Convert([ID], 'System.String') OR " &
                                          "Path = '" & Me.strParent & "\' + Convert([ID], 'System.String')"
                    oDataView.Sort = "Name"
                    For Each oDataviewRow In oDataView
                        x.Append("{ 'id':'A" & oDataviewRow("ID") & "', 'text':'" & Me.FilterSpecialChars(CStr(oDataviewRow("Name"))) & "', 'leaf':false, 'icon': '" & Me.strImagesPath & Me.strIconGroup & "'")
                        If Me.bolMultiSelect Then
                            x.Append(", 'checked2' : false")
                            If Me.lstSelection.Contains("A" & oDataviewRow("ID")) Then
                                x.Append(", 'checked': true, 'expanded': true")
                            Else
                                x.Append(", 'checked': false")
                            End If
                        End If
                        x.Append("},")
                    Next
                End If
            Else

                Me.strParent = Me.strParent.Replace("#sep1#", "\")
                Me.strParent = Me.strParent.Replace("#sep2#", "/")
                Me.strParent = Me.strParent.Replace("#sep3#", "'")

                Dim tb As DataTable = UserFieldServiceMethods.GetUserFieldValues(Me, Me.strUserField, Types.EmployeeField, Me.strParent, "\", Me.strFilterUserFields)
                Dim oDataView As System.Data.DataView = New System.Data.DataView(tb)
                oDataView.Sort = "Name"
                Dim strId As String
                For Each oDataviewRow In oDataView
                    strId = oDataviewRow("Path")
                    strId = strId.Replace("\", "#sep1#")
                    strId = strId.Replace("/", "#sep2#")
                    strId = strId.Replace("'", "#sep3#")
                    x.Append("{ 'id':'C" & Me.FilterSpecialChars(Me.strUserField) & "~" & strId & "', 'text':'" & Me.FilterSpecialChars(CStr(oDataviewRow("Name"))) &
                                     "', 'leaf':false, 'icon': '" & Me.strImagesPath & Me.strIconGroup & "'")
                    If Me.bolMultiSelect Then
                        x.Append(", 'checked2' : false")
                        If Me.lstSelection.Contains("C" & oDataviewRow("Path")) Then
                            x.Append(", 'checked': true, 'expanded': true")
                        Else
                            x.Append(", 'checked': false")
                        End If
                    End If
                    x.Append("},")
                Next

            End If

        End If

        If Not Me.bolOnlyGroups And bolShowChilds Then

            Dim tbEmployees As DataTable

            If Me.strUserField = "" Then
                tbEmployees = EmployeeGroupsServiceMethods.GetEmployeesFromGroupWithType(Me, roTypes.Any2Integer(Me.strParent), Me.strFeatureAlias, Me.strFilterUserFields, Me.strFeatureType)
            Else
                tbEmployees = UserFieldServiceMethods.GetEmployeesFromUserFieldWithType(Me, Me.strUserField, Me.strParent, Me.strFilterUserFields, Me.strFeatureAlias, Me.strFeatureType)
            End If

            If tbEmployees IsNot Nothing Then

                Dim oDataView As System.Data.DataView = New Data.DataView(tbEmployees)
                oDataView.Sort = "EmployeeName"

                If oDataView.Count > 0 Then
                    Dim bolFilter As Boolean = False
                    Dim strIcon As String = ""
                    For Each oDataviewRow In oDataView

                        bolFilter = (Me.bolFilterCurrent And oDataviewRow("Type") = 1 Or
                                     Me.bolFilterMovility And oDataviewRow("Type") = 2 Or
                                     Me.bolFilterOld And oDataviewRow("Type") = 3 Or
                                     Me.bolFilterFuture And oDataviewRow("Type") = 4)

                        Dim typeEmployee As Integer = roTypes.Any2Integer(oDataviewRow("Type"))
                        Select Case typeEmployee
                            Case 1
                                strIcon = "Empleado-16x16.gif"
                            Case 2
                                strIcon = "Empleado-Move-16x16.gif"
                            Case 3
                                strIcon = "Empleado-Remove-16x16.GIF"
                            Case 4
                                strIcon = "Empleado-Add-16x16.GIF"
                            Case Else
                                strIcon = "Empleado-16x16.gif"
                        End Select

                        If bolFilter Then
                            x.Append("{ 'id':'B" & oDataviewRow("IDEmployee") & "', 'text':'" & Me.FilterSpecialChars(CStr(oDataviewRow("EmployeeName"))) & "', 'leaf':true, 'icon': '" & Me.strImagesPath & strIcon & "'")
                            If Me.bolMultiSelect Then
                                x.Append(", 'checked2' : false")
                                If Me.lstSelection.Contains("B" & oDataviewRow("IDEmployee")) Then
                                    x.Append(", 'checked': true")
                                Else
                                    x.Append(", 'checked': false")
                                End If
                            End If
                            x.Append("},")
                        End If

                    Next
                End If

            End If

        End If
        Me.strArrayNodes = x.ToString
        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

    Public Sub LoadEmployeesTreeAll()

        If Me.strUserField = "" Then
            strArrayNodes = ""
            Dim x As New StringBuilder(strArrayNodes)

            Dim lstGroups As Generic.List(Of roGroupTree) = EmployeeGroupsServiceMethods.GetTree(Me, Me.strFilterUserFields, Me.strFeatureAlias, Me.strFeatureType)
            If lstGroups IsNot Nothing Then

                If lstGroups.Count > 0 Then

                    Dim strIcon As String = Me.strIconGroup
                    If HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultiCompany") Then strIcon = Me.strIconCompany
                    For Each oGroup As roGroupTree In lstGroups

                        x.Append("{ 'id':'A" & oGroup.ID & "', 'text':'" & Me.FilterSpecialChars(CStr(oGroup.Name)) & "', 'leaf':false, 'icon': '" & Me.strImagesPath & strIcon & "'")
                        If Me.bolMultiSelect Then
                            x.Append(", 'checked2' : false")
                            If Me.lstSelection.Contains("A" & oGroup.ID) Then
                                x.Append(", 'checked': true, 'expanded': true")
                            Else
                                x.Append(", 'checked': false")
                            End If
                        End If
                        x.Append(", 'children': " & Me.GetTreeLevel(oGroup))
                        x.Append("},")

                    Next

                End If

            End If
            strArrayNodes = x.ToString
        Else

            ' Crear árbol en función de un campo de la ficha
            ' ...

        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

    Private Function GetTreeLevel(ByVal oGroup As roGroupTree) As String

        Dim strRet As String = ""
        Dim x As New StringBuilder(strRet)

        If oGroup.ChildrenGroups IsNot Nothing Then

            For Each oChildGroup As roGroupTree In oGroup.ChildrenGroups

                x.Append("{ 'id':'A" & oChildGroup.ID & "', 'text':'" & Me.FilterSpecialChars(CStr(oChildGroup.Name)) & "', 'leaf':false, 'icon': '" & Me.strImagesPath & Me.strIconGroup & "'")
                If Me.bolMultiSelect Then
                    x.Append(", 'checked2' : false")
                    If Me.lstSelection.Contains("A" & oChildGroup.ID) Then
                        x.Append(", 'checked': true, 'expanded': true")
                    Else
                        x.Append(", 'checked': false")
                    End If
                End If
                x.Append(", 'children': ")
                x.Append(Me.GetTreeLevel(oChildGroup))
                x.Append("},")

            Next

        End If

        If oGroup.Employees IsNot Nothing Then

            Dim bolFilter As Boolean = False
            Dim strIcon As String = ""

            For Each oEmployee As roEmployeeTree In oGroup.Employees

                bolFilter = (Me.bolFilterCurrent And oEmployee.Type = 1 Or
                             Me.bolFilterMovility And oEmployee.Type = 2 Or
                             Me.bolFilterOld And oEmployee.Type = 3 Or
                             Me.bolFilterFuture And oEmployee.Type = 4)

                Select Case oEmployee.Type
                    Case 1
                        strIcon = "Empleado-16x16.gif"
                    Case 2
                        strIcon = "Empleado-Move-16x16.gif"
                    Case 3
                        strIcon = "Empleado-Remove-16x16.GIF"
                    Case 4
                        strIcon = "Empleado-Add-16x16.GIF"
                    Case Else
                        strIcon = "Empleado-16x16.gif"
                End Select

                If bolFilter Then
                    x.Append("{ 'id':'B" & oEmployee.ID & "', 'text':'" & Me.FilterSpecialChars(CStr(oEmployee.Name)) & "', 'leaf':true, 'icon': '" & Me.strImagesPath & strIcon & "'")
                    If Me.bolMultiSelect Then
                        x.Append(", 'checked2' : false")
                        If Me.lstSelection.Contains("B" & oEmployee.ID) Then
                            x.Append(", 'checked': true")
                        Else
                            x.Append(", 'checked': false")
                        End If
                    End If
                    x.Append("},")
                End If

            Next

        End If

        strRet = x.ToString

        If strRet <> "" Then
            strRet = "[" & strRet.Substring(0, strRet.Length - 1) & "]"
        Else
            strRet = "[]"
        End If

        Return strRet

    End Function

    Private Function GetMyParentGroup(ByVal Path As String) As String
        ' Busca el que tiene que ser padre del grupo pasado por parámetro
        ' Ej: Path = 1/2 ---> GetMyParentGroup = 1
        ' Ej: Path = 1/2/3 ---> GetMyParentGroup = 2

        Dim PosSlash As Integer
        Dim PosSlash2 As Integer
        Dim ParentPath As String

        PosSlash = InStrRev(Path, "\")

        If PosSlash > 0 Then
            PosSlash2 = InStrRev(Path, "\", PosSlash - 1)
            If PosSlash2 > 0 Then
                ParentPath = Mid(Path, PosSlash2 + 1, PosSlash - PosSlash2 - 1)
            Else
                ParentPath = Mid(Path, 1, PosSlash - PosSlash2 - 1)
            End If
            Return ParentPath
        Else
            Return Path
        End If

    End Function

    Private Function GetEmployeeGroupPath(ByVal intIDEmployee As Integer) As String

        Dim strRet As String = ""

        ' Obtener el grupo actual del empleado
        Dim oMobility As roMobility = EmployeeServiceMethods.GetCurrentMobility(Nothing, intIDEmployee)
        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoError Then

            If oMobility IsNot Nothing Then

                ' Obtener el path del grupo del empleado
                Dim oGroup As roGroup = EmployeeGroupsServiceMethods.GetGroup(Nothing, oMobility.IdGroup, False)
                If roWsUserManagement.SessionObject.States.EmployeeState.Result = GroupResultEnum.NoError Then
                    If oGroup.Path.Split("\").Length >= 1 Then
                        For n As Integer = 0 To oGroup.Path.Split("\").Length - 1
                            strRet &= "A" & oGroup.Path.Split("\")(n) & "/"
                        Next
                        If strRet <> "" Then strRet = "/" & strRet.Substring(0, strRet.Length - 1)
                    End If
                End If

            End If

        End If

        Return strRet

    End Function

    Private Function GetGroupPath(ByVal intIDGroup As Integer) As String

        Dim strRet As String = ""

        ' Obtener el path del grupo
        Dim oGroup As roGroup = EmployeeGroupsServiceMethods.GetGroup(Nothing, intIDGroup, False)
        If roWsUserManagement.SessionObject.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
            If oGroup.Path.Split("\").Length > 1 Then
                For n As Integer = 0 To oGroup.Path.Split("\").Length - 2
                    strRet &= "A" & oGroup.Path.Split("\")(n) & "/"
                Next
                If strRet <> "" Then strRet = "/" & strRet.Substring(0, strRet.Length - 1)
            End If
        End If

        Return strRet

    End Function

    Private Sub LoadEmployeesSearch()

        Dim tbEmployees As DataTable = Nothing
        Dim x As New StringBuilder(strArrayNodes)

        Dim strTextoABuscar As String = Me.strFieldFindValue.Trim
        Dim IdEmp As Integer = 0

        Select Case Me.strFieldFindColumn.ToLower
            Case "idemployee"
                If Not Integer.TryParse(strTextoABuscar.Replace("?", "").Replace("'", "").Replace("*", "").Replace("%", "").Trim(), IdEmp) Then
                    IdEmp = -1
                End If
            Case "advfilter"
                If strFieldFindValue.IndexOf("}") < 0 Then
                    strTextoABuscar = "{" & strTextoABuscar & "}"
                End If
            Case "employeename", "idcontract", "idcard", "plate"
                strTextoABuscar = strTextoABuscar.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
                If Not strTextoABuscar.StartsWith("%") Then strTextoABuscar = "%" & strTextoABuscar
                If Not strTextoABuscar.EndsWith("%") Then strTextoABuscar = strTextoABuscar & "%"
            Case Else
                strTextoABuscar = strTextoABuscar.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
                If Not strTextoABuscar.StartsWith("%") Then strTextoABuscar = "%" & strTextoABuscar
                If Not strTextoABuscar.EndsWith("%") Then strTextoABuscar = strTextoABuscar & "%"
        End Select

        Select Case Me.strFieldFindColumn.ToLower
            Case "employeename"
                tbEmployees = EmployeeServiceMethods.GetEmployeesByName(Nothing, strTextoABuscar, Me.strFilterUserFields, Me.strFeatureAlias)
            Case "idcontract"
                tbEmployees = EmployeeServiceMethods.GetEmployeesByIDContract(Nothing, strTextoABuscar, Me.strFilterUserFields, Me.strFeatureAlias)
            Case "idcard"
                tbEmployees = EmployeeServiceMethods.GetEmployeesByIDCard(Nothing, strTextoABuscar, Me.strFilterUserFields, Me.strFeatureAlias)
            Case "plate"
                tbEmployees = EmployeeServiceMethods.GetEmployeesByPlate(Nothing, strTextoABuscar, Me.strFilterUserFields, Me.strFeatureAlias)
            Case "idemployee"
                If IdEmp > 0 Then
                    tbEmployees = EmployeeServiceMethods.GetEmployeesById(Nothing, IdEmp, Me.strFeatureAlias)
                End If
            Case "advfilter"
                tbEmployees = EmployeeServiceMethods.GetEmployeesByAdvancedFilter(Nothing, strTextoABuscar, Me.strFilterUserFields, Me.strFeatureAlias)
            Case Else
                Dim strWhere As String = String.Empty
                If Me.strFieldFindColumn <> "" Then
                    strWhere = Me.strFieldFindColumn & " LIKE '" & strTextoABuscar & "'"
                End If
                tbEmployees = API.EmployeeServiceMethods.GetAllEmployees(Nothing, strWhere, Me.strFeatureAlias)
        End Select

        If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then

            Dim oDataView As System.Data.DataView = New Data.DataView(tbEmployees)
            oDataView.Sort = "EmployeeName"

            Dim bolFilter As Boolean = False
            Dim strIcon As String = ""
            For Each oDataviewRow As DataRowView In oDataView

                bolFilter = (Me.bolFilterCurrent And oDataviewRow("Type") = 1 Or
                             Me.bolFilterMovility And oDataviewRow("Type") = 2 Or
                             Me.bolFilterOld And oDataviewRow("Type") = 3 Or
                             Me.bolFilterFuture And oDataviewRow("Type") = 4)

                Dim intEmployeeType As Integer = roTypes.Any2Integer(oDataviewRow("Type"))
                Select Case intEmployeeType
                    Case 1
                        strIcon = "Empleado-16x16.gif"
                    Case 2
                        strIcon = "Empleado-Move-16x16.gif"
                    Case 3
                        strIcon = "Empleado-Remove-16x16.GIF"
                    Case 4
                        strIcon = "Empleado-Add-16x16.GIF"
                    Case Else
                        strIcon = "Empleado-16x16.gif"
                End Select

                If bolFilter Then
                    Dim colName = Me.strFieldFindColumn
                    If Me.strFieldFindColumn.ToLower = "advfilter" Then
                        colName = "employeename"
                    End If
                    x.Append("{ 'id':'B" & oDataviewRow("IDEmployee") & "', 'text':'" &
                                  Me.FilterSpecialChars(roTypes.Any2String(oDataviewRow(colName))) & "', 'leaf':true, 'icon': '" & Me.strImagesPath & strIcon & "'")
                    If Me.bolMultiSelect Then
                        x.Append(", 'checked2' : false")
                        If Me.lstSelection.Contains("B" & oDataviewRow("IDEmployee")) Then
                            x.Append(", 'checked': true")
                        Else
                            x.Append(", 'checked': false")
                        End If
                    End If
                    x.Append("},")
                End If

            Next

        End If
        strArrayNodes = x.ToString

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

    Private Function ParseFilterUserFields(ByVal _Filter As String) As String

        Dim strRet As String = ""

        If _Filter <> "" Then
            _Filter = HttpUtility.UrlDecode(_Filter)

            Dim FilterList As New Generic.List(Of roUserFieldCondition)()
            Dim FilterListCondition As New Generic.List(Of String)()

            Dim strFieldName As String = ""
            Dim arrFilters As String() = _Filter.Split(Chr(127))
            Dim arrParams As String()

            Dim oUserFieldCondition As roUserFieldCondition = Nothing

            For Each str As String In arrFilters
                If str.Trim <> "" Then

                    oUserFieldCondition = New roUserFieldCondition

                    arrParams = str.Split("~")

                    'Obtener el nombre del campo
                    strFieldName = arrParams(0).Split("|")(0)
                    If strFieldName.ToUpper.StartsWith("USR_") Then strFieldName = strFieldName.Substring(4)

                    'Obtener el tipo de campo
                    oUserFieldCondition.UserField = New roUserField
                    oUserFieldCondition.UserField.FieldName = strFieldName
                    oUserFieldCondition.UserField.Type = Types.EmployeeField
                    oUserFieldCondition.UserField.FieldType = arrParams(0).Split("|")(1).Substring(1, 1)

                    Select Case arrParams(1)
                        Case "=" : oUserFieldCondition.Compare = CompareType.Equal
                        Case "<>" : oUserFieldCondition.Compare = CompareType.Distinct
                        Case ">" : oUserFieldCondition.Compare = CompareType.Major
                        Case ">=" : oUserFieldCondition.Compare = CompareType.MajorEqual
                        Case "<" : oUserFieldCondition.Compare = CompareType.Minor
                        Case "<=" : oUserFieldCondition.Compare = CompareType.MinorEqual
                        Case "*" : oUserFieldCondition.Compare = CompareType.StartWith
                        Case "**" : oUserFieldCondition.Compare = CompareType.Contains
                    End Select
                    oUserFieldCondition.ValueType = CompareValueType.DirectValue

                    'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                    If arrParams(2).StartsWith("(") AndAlso arrParams(2).EndsWith(")") Then
                        oUserFieldCondition.Value = arrParams(2).Substring(1, arrParams(2).Length - 2)
                    Else
                        oUserFieldCondition.Value = arrParams(2)
                    End If

                    FilterList.Add(oUserFieldCondition)
                    FilterListCondition.Add(arrParams(3))

                End If
            Next

            If FilterList.Count > 0 Then
                strRet = UserFieldServiceMethods.GetUserFieldConditionFilterGlobal(Me.Page, FilterList, FilterListCondition)
            End If

        End If

        Return strRet

    End Function

    Private Function FilterSpecialChars(ByVal strValue As String) As String
        Return strValue.Replace(";", "&#45;").Replace("'", "&#39;").Replace("\", "&#92;")
    End Function

    Private Function FormatUserFieldValue(ufType As FieldTypes, ufValue As String) As String
        Dim formattedValue As String = String.Empty
        Select Case ufType
            Case FieldTypes.tDecimal
                If formattedValue <> "" Then
                    formattedValue = ufValue.Replace(".", HelperWeb.GetDecimalDigitFormat())
                End If
            Case FieldTypes.tDate
                If ufValue <> "" Then
                    formattedValue = Format(roTypes.Any2DateTime(ufValue), HelperWeb.GetShortDateFormat)
                End If

            Case FieldTypes.tTime
                If ufValue <> "" Then
                    formattedValue = roConversions.ConvertHoursToTime(roTypes.Any2Double(ufValue.Replace(".", HelperWeb.GetDecimalDigitFormat())))
                End If

            Case FieldTypes.tDatePeriod
                Dim xDate As DateTime
                If ufValue.Split("*")(0) <> "" AndAlso ufValue.Split("*")(0).Length = 10 Then
                    xDate = roTypes.CreateDateTime(ufValue.Split("*")(0).Substring(0, 4), ufValue.Split("*")(0).Substring(5, 2), ufValue.Split("*")(0).Substring(8, 2))
                    formattedValue = Format(xDate, HelperWeb.GetShortDateFormat)
                End If

                If ufValue.Split("*").Length > 1 AndAlso ufValue.Split("*")(1).Length = 10 Then
                    xDate = roTypes.CreateDateTime(ufValue.Split("*")(1).Substring(0, 4), ufValue.Split("*")(1).Substring(5, 2), ufValue.Split("*")(1).Substring(8, 2))
                    formattedValue &= formattedValue & " - " & Format(xDate, HelperWeb.GetShortDateFormat)
                End If

            Case FieldTypes.tTimePeriod
                Dim xDate As DateTime
                If ufValue.Split("*")(0) <> "" AndAlso ufValue.Split("*")(0).Length = 5 Then
                    xDate = roTypes.CreateDateTime(1900, 1, 1, ufValue.Split("*")(0).Substring(0, 2), ufValue.Split("*")(0).Substring(3, 2), 0)
                    formattedValue = Format(xDate, HelperWeb.GetShortTimeFormat)
                End If
                If ufValue.Split("*").Length > 1 AndAlso ufValue.Split("*")(1).Length = 5 Then
                    xDate = roTypes.CreateDateTime(1900, 1, 1, ufValue.Split("*")(1).Substring(0, 2), ufValue.Split("*")(1).Substring(3, 2), 0)
                    formattedValue &= formattedValue & " - " & Format(xDate, HelperWeb.GetShortTimeFormat)
                End If
            Case Else
                formattedValue = ufValue
        End Select

        Return formattedValue
    End Function

    Private Function StringEncodeControlChars(ByVal sInput As String) As String
        '
        ' Cambia caracteres de control de un string por tokens.
        '  Llamando de nuevo a la funcion StringDecodeControlChars se obtiene de nuevo el
        '  string original.

        Dim sOutput As String
        Dim I As Integer

        sOutput = sInput
        For I = 1 To 31
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        For I = 60 To 62
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        For I = 123 To 255
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        StringEncodeControlChars = sOutput

    End Function

    Private Function StringDecodeControlChars(ByVal sInput As String) As String
        '
        ' Descodifica un string codificado previamente con StringEncodeControlChars.
        '

        Dim sOutput As String
        Dim I As Integer

        sOutput = sInput
        For I = 1 To 31
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next
        For I = 60 To 62
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next
        For I = 123 To 255
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next

        StringDecodeControlChars = sOutput

    End Function

End Class