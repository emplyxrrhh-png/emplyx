Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Web.Base

Partial Class TerminalSelectorData
    Inherits PageBase

#Region "Declarations"

    Private strAction As String = "TreeData"
    Private strIDParent As String = ""

    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconTerminalList As String = "TermList.png"
    Private strIconTerminal As String = "terminal16.png"
    Private strIconTerminal_active As String = "terminal-active.png"
    Private strIconTerminal_inactive As String = "terminal-inactive.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterTerminalEmployee As Boolean = True

    Private strUserField As String = ""
    Private lstSelection As New ArrayList

    Private Const FeatureAlias As String = "Terminals"
    Private Const FeatureAliasStatus As String = "Terminals.StatusInfo"
    Private Const FeatureAliasDefinition As String = "Terminals.Definition"
    Private oPermission As Permission
    Private oPermissionStatus As Permission
    Private oPermissionDefinition As Permission

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Controls.Clear()
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermissionStatus = Me.GetFeaturePermission(FeatureAliasStatus)
        Me.oPermissionDefinition = Me.GetFeaturePermission(FeatureAliasDefinition)

        If oPermission = Permission.None Then Exit Sub

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
            Me.strImagesPath = "../../images/TerminalSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" Then
            strIDParent = Me.Request("node")
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterTerminalEmployee = True Else Me.bolFilterTerminalEmployee = False
        End If

        If ViewState("TerminalSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("TerminalSelector_Selection")
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

                LoadTerminalsTree()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                If Me.strArrayNodes = "" Then Me.strArrayNodes = "[]"
                Me.Response.Write(Me.strArrayNodes)
            Case "getSelectionPath" ' Obtiene la ruta del empleado seleccionado (en strParent).

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"

                Dim strPath As String = ""

                If Me.strUserField = "" Then
                    ' Buscamos la ruta de la zona
                    'TODO: en un futur arreglar per buscar per idparent
                    'strPath = Me.GetZoneGroupPath(Me.strIDParent)
                End If
                If Me.strIDParent.StartsWith("A") Then
                    strPath = "/source/A1"
                Else
                    strPath = "/source/A1/" & Me.strIDParent
                End If

                Me.Response.Write(strPath)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadTerminalsTree()
        ' Obtenemos los grupos
        If strIDParent <> "" Then
            Dim TList As roTerminalList = API.TerminalServiceMethods.GetTerminals(Me.Page)
            For Each oTerm As roTerminal In TList.Terminals
                Dim strIco As String = ""
                If oPermissionStatus = Permission.None Then
                    strIco = strIconTerminal
                Else
                    If oTerm.LastStatus.ToUpper = "OK" Then
                        'strIco = strIconTerminal_active
                        strIco = strIconTerminal
                    Else
                        'strIco = strIconTerminal_inactive
                        strIco = strIconTerminal
                    End If
                End If

                If Not oTerm.Enabled Then
                    strIco = strIconTerminal_inactive
                End If

                If oPermissionDefinition > Permission.None Then
                    strArrayNodes &= "{ 'id':'B" & oTerm.ID & "', 'text':'" & oTerm.ID & " - " & oTerm.Description.Replace("'", "&#39;") & "', " &
                                     "'leaf': true, " &
                                     "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                End If
            Next
        Else
            strArrayNodes &= "{ 'id':'A1', 'text':'" & Me.Language.Keyword("Terminals") & "', " &
                     "'leaf': false, " &
                     "'icon': '" & Me.strImagesPath & strIconTerminalList & "'"
            strArrayNodes &= "},"
        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

#End Region

End Class