Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Web.Base

Partial Class TaskTemplateSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private strParent As String = ""
    Private strParentType As String = ""
    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconTaskTemplate As String = "TaskTemplateIco.png"
    Private strIconProject As String = "ProjectIco.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterShiftEmployee As Boolean = True

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
            Me.strImagesPath = "../../images/TaskTemplateSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node").Length > 1 AndAlso Me.Request("node") <> "null" AndAlso Me.Request("node") <> "undefined" Then
            Me.strParentType = Me.Request("node").Substring(0, 1)
            Me.strParent = Me.Request("node").Substring(1)
            'End If
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
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterShiftEmployee = True Else Me.bolFilterShiftEmployee = False
        End If

        If ViewState("TaskTemplateSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("TaskTemplateSelector_Selection")
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

                LoadTaskTemplatesTree()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                If Me.strArrayNodes = "" Then Me.strArrayNodes = "[]"
                Me.Response.Write(Me.strArrayNodes)
            Case "getSelectionPath"
                Me.Controls.Clear()

                Me.Response.Clear()
                Me.Response.ContentType = "text/plain"

                Dim strPath As String = ""

                ' Buscamos la ruta del grupo del empleado
                If Me.strParent <> "" Then
                    Select Case Me.strParentType
                        Case "A"
                            strPath = "/A" & Me.strParent
                        Case "B"
                            strPath = Me.GetTaskTemplatePath(Me.intIDParent)
                    End Select
                End If
                Me.Response.Write("/source" & strPath)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Function GetTaskTemplatePath(ByVal intIDTaskTemplate As Integer) As String

        Dim strRet As String = ""

        ' Obtener el grupo actual del empleado
        Dim oTaskTemplate As roTaskTemplate = API.TaskTemplatesServiceMethods.GetTaskTemplate(Nothing, intIDTaskTemplate, False)
        If roWsUserManagement.SessionObject.States.TaskTemplateState.Result = TaskResultEnum.NoError Then

            If oTaskTemplate IsNot Nothing Then
                ' Obtener el path del grupo del horario
                strRet = "/A" & oTaskTemplate.IDProject & "/B" & oTaskTemplate.ID
            End If

        End If

        Return strRet

    End Function

    Private Sub LoadTaskTemplatesTree()

        If intIDParent.HasValue Then
            Dim dTbl As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(intIDParent, "", Me.Page, "", False)
            If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                For Each dRow As DataRow In dTbl.Rows
                    Dim strIco As String = strIconTaskTemplate

                    strArrayNodes &= "{ 'id':'B" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("\", "\\").Replace("'", "&#39;") & "', " &
                                     "'leaf': true, " &
                                     "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                Next
            End If
        Else
            ' Obtenemos los grupos
            Dim dTblGroup As DataTable = API.TaskTemplatesServiceMethods.GetProjectTemplatesDataTable(Me.Page, "", False)
            For Each dRGroup As DataRow In dTblGroup.Rows
                If dRGroup("ID").ToString <> "0" Then ' Si no es el Grup GENERAL
                    strArrayNodes &= "{ 'id':'A" & dRGroup("ID") & "', 'text':'" & dRGroup("Project").Replace("\", "\\").Replace("'", "&#39;") & "', " &
                                             "'leaf': false, " &
                                             "'icon': '" & Me.strImagesPath & strIconProject & "'"
                    strArrayNodes &= "},"
                End If
            Next
        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

#End Region

End Class