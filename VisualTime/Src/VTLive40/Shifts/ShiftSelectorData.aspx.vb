Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Web.Base

Partial Class ShiftsSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private strParent As String = ""
    Private strParentType As String = ""
    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconShift As String = "ShiftIco.png"
    Private strIconShiftOld As String = "ShiftIco-disabled.png"
    Private strIconShiftGrp As String = "ShiftGroup.png"

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
            Me.strImagesPath = "../../images/ShiftSelector"
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

        If ViewState("ShiftSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = ViewState("ShiftSelector_Selection")
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

                LoadShiftsTree()

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
                            strPath = Me.GetShiftGroupPath(Me.intIDParent)
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

    Private Function GetShiftGroupPath(ByVal intIDShift As Integer) As String

        Dim strRet As String = ""

        ' Obtener el grupo actual del empleado
        Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Nothing, intIDShift, False)
        If roWsUserManagement.SessionObject.States.ShiftState.Result = ShiftResultEnum.NoError Then

            If oShift IsNot Nothing Then
                ' Obtener el path del grupo del horario
                strRet = "/A" & oShift.IDGroup & "/B" & oShift.ID
            End If

        End If

        Return strRet

    End Function

    Private Sub LoadShiftsTree()

        If intIDParent.HasValue Then
            Dim dTbl As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page, intIDParent, True)
            If dTbl.Rows.Count > 0 Then
                Dim oRows() As DataRow = dTbl.Select("", "IsObsolete ASC")

                For Each dRow As DataRow In oRows
                    Dim strIco As String = strIconShift

                    If dRow("IsObsolete") = True Then
                        strIco = strIconShiftOld
                    End If

                    strArrayNodes &= "{ 'id':'B" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                     "'leaf': true, " &
                                     "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                Next
            End If
        Else
            ' Obtenemos los grupos
            Dim dTblGroup As DataTable = API.ShiftServiceMethods.GetShiftGroups(Me.Page)
            For Each dRGroup As DataRow In dTblGroup.Rows
                If dRGroup("ID").ToString <> "0" Then ' Si no es el Grup GENERAL
                    strArrayNodes &= "{ 'id':'A" & dRGroup("ID") & "', 'text':'" & dRGroup("Name").Replace("'", "&#39;") & "', " &
                                             "'leaf': false, " &
                                             "'icon': '" & Me.strImagesPath & strIconShiftGrp & "'"
                    strArrayNodes &= "},"
                End If
            Next

            ' Cargamos solo los del Grupo 0 en la raiz (General)
            Dim dTbl As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page, 0, True)

            If dTbl.Rows.Count > 0 Then
                Dim oRows() As DataRow = dTbl.Select("", "IsObsolete ASC")

                For Each dRow As DataRow In oRows
                    Dim strIco As String = strIconShift

                    If dRow("IsObsolete") = True Then
                        strIco = strIconShiftOld
                    End If

                    strArrayNodes &= "{ 'id':'B" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
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