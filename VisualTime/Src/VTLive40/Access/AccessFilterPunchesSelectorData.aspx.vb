Imports Robotics.Web.Base

Public Class AccessFilterPunchesSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private strIDParent As String = ""
    Private strIconAccessZones As String = "AccessZones_16.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterConceptAccessZones As Boolean = True

    Private strUserField As String = ""
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
            Me.strImagesPath = "../../images/AccessZonesSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" Then
            strIDParent = Me.Request("node")
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterConceptAccessZones = True Else Me.bolFilterConceptAccessZones = False
        End If

        If ViewState("AccessFilterPunchesSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("AccessFilterPunchesSelector_Selection")
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

                LoadAccessZonesTree()

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
                Dim intPID As Integer = retParentZoneID()
                If Me.strIDParent.StartsWith("A") Then
                    strPath = "/source/A" & intPID.ToString
                Else
                    strPath = "/source/A" & intPID.ToString & "/" & Me.strIDParent
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

    Private Sub LoadAccessZonesTree()

        Dim advancedAccess As Boolean = HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1")

        Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page, If(advancedAccess, WLHelperWeb.CurrentPassportID, 0))

        If strIDParent <> "" Then
            If dTbl.Rows.Count > 0 Then
                For Each dRow As DataRow In dTbl.Rows
                    If "A" & dRow("IDParent") = strIDParent Then
                        strArrayNodes &= "{ 'id':'B" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                         "'leaf': true, " &
                                         "'icon': '" & Me.strImagesPath & strIconAccessZones & "'"
                        strArrayNodes &= "},"
                    End If
                Next
            End If
        Else
            If dTbl.Rows.Count > 0 Then

                For Each dRow As DataRow In dTbl.Rows
                    If dRow("IDParent") Is DBNull.Value Then
                        strArrayNodes &= "{ 'id':'A" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                         "'leaf': false, " &
                                         "'icon': '" & Me.strImagesPath & strIconAccessZones & "'"
                        strArrayNodes &= "},"
                        Exit For
                    End If
                Next
            End If
        End If

        If Me.strArrayNodes <> "" Then
            Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
        End If

    End Sub

    Private Function retParentZoneID() As Integer
        Dim intReturn As Integer = 1

        Try
            Dim advancedAccess As Boolean = HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1")

            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page, If(advancedAccess, WLHelperWeb.CurrentPassportID, 0))

            Dim dRows() As DataRow = dTbl.Select("IDParent IS NULL")
            If dRows.Length > 0 Then
                intReturn = dRows(0)("ID")
            End If
        Catch ex As Exception
        End Try

        Return intReturn
    End Function

#End Region

End Class