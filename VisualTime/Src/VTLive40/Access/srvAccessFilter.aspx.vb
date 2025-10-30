Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class srvAccessFilter
    Inherits PageBase

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.HasFeaturePermission("Access.Zones.Supervision", Permission.Read) Then
            Select Case Request("action")
                Case "getXGridsJSON" 'Carrega del grid de composicions
                    Me.Controls.Clear()
                    Me.CreateGridsJSON()
            End Select
        Else
            'Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Me.Controls.Clear()
            Dim strResponse As String = "MESSAGE" &
                          "TitleKey=CheckPermission.Denied.Title&" +
                          "DescriptionKey=CheckPermission.Denied.Description&" +
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)
        End If
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Carrega dels 2 grids (Exceptions / Periods)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateGridsJSON()
        Try
            Dim oJGException As New Generic.List(Of Object)
            Dim oJFExceptions As Generic.List(Of JSONFieldItem)

            Dim strJSONGroups As String = ""

            If Request("ID") = "-1" Then Exit Sub

            Dim strBegin As Date = roTypes.Any2Time(Request("DateBegin")).Value
            Dim strEnd As String = Request("DateEnd")
            Dim strHBegin As String = Request("HourBegin")
            Dim strHEnd As String = Request("HourEnd")

            Dim strEmployees As String = Request("Employees")
            Dim strFilterTree As String = roTypes.Any2String(Request("FilterTree"))
            Dim strFilterTreeUser As String = roTypes.Any2String(Request("FilterTreeUser"))

            Dim strZones As String = Request("Zones")

            'Valors ordenacio grid
            Dim strSortedField As String = Request("SortField")
            Dim strOrderType As String = Request("SortOrder")
            '##Dim oType As AccessMoveService.eSortType
            '##If strOrderType = "ASC" Then oType = AccessMoveService.eSortType.Ascendend Else oType = AccessMoveService.eSortType.Descended

            Dim dBegin As DateTime = strBegin & " " & strHBegin
            Dim dEnd As DateTime = strEnd & " " & strHEnd

            Dim oEmployees As New Generic.List(Of Integer)
            Dim oArrEmp() As String

            Dim ListGroupsAux As New List(Of Integer)
            Dim ListEmpsAux As List(Of Integer) = Nothing

            If strEmployees = "ALL" Then
                oArrEmp = Nothing
            Else
                If strEmployees <> "" Then
                    oArrEmp = strEmployees.Split(",")
                    For n = 0 To oArrEmp.Length - 1

                        If oArrEmp(n).StartsWith("A") Then 'grupos
                            ListGroupsAux.Add(oArrEmp(n).Substring(1))
                        End If

                        If Not oArrEmp(n).StartsWith("A") And Not oArrEmp(n).StartsWith("B") Then
                            oEmployees.Add(CInt(oArrEmp(n).Replace("B", "")))
                        Else
                            If oArrEmp(n).StartsWith("B") Then
                                oEmployees.Add(CInt(oArrEmp(n).Replace("B", "")))
                            End If
                        End If
                    Next

                    If ListGroupsAux.Count > 0 Then
                        ListEmpsAux = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me.Page, ListGroupsAux.ToArray, "Access", "U",
                                                                                                                           strFilterTree, strFilterTreeUser)
                        oEmployees.AddRange(ListEmpsAux)
                    End If

                End If
            End If

            Dim oZones As New Generic.List(Of Integer)
            Dim oArrZone() As String
            Dim advancedAccess = HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1")

            Dim dTblZones As DataTable

            If strZones = "ALL" Then
                If (advancedAccess) Then
                    dTblZones = ZoneServiceMethods.GetZones(Me.Page, WLHelperWeb.CurrentPassportID)
                    If (dTblZones IsNot Nothing AndAlso dTblZones.Rows.Count > 0) Then
                        For Each row As DataRow In dTblZones.Rows
                            If (row("ID") <> 1) Then oZones.Add(row("ID"))
                        Next
                    End If
                Else
                    oArrZone = Nothing
                End If
            Else
                If strZones <> "" Then
                    oArrZone = strZones.Split(",")
                    For n = 0 To oArrZone.Length - 1
                        If Not oArrZone(n).StartsWith("A") And Not oArrZone(n).StartsWith("B") Then
                            oZones.Add(CInt(oArrZone(n).Replace("B", "")))
                        Else
                            If oArrZone(n).StartsWith("B") Then
                                oZones.Add(CInt(oArrZone(n).Replace("B", "")))
                            End If
                        End If
                    Next
                End If
            End If

            Dim dTbl As DataTable = Nothing

            Dim strTypes As String = ""
            Dim strOrderBy As String = "DESC"

            Dim strEmployeesAux As String = "-1"
            Dim strZonesAux As String = "-1"

            Dim Index As Integer
            For Index = 0 To oEmployees.Count - 1
                strEmployeesAux &= "," & oEmployees(Index)
            Next
            For Index = 0 To oZones.Count - 1
                strZonesAux &= "," & oZones(Index)
            Next

            If strOrderType = "ASC" Then
                strOrderBy = "ASC"
            End If

            strTypes = roTypes.Any2Double(PunchTypeEnum._L) & "," & roTypes.Any2Double(PunchTypeEnum._AV)

            If strSortedField = "" Then
                strSortedField = " Punches.DateTime " & strOrderBy & ", Punches.ID " & strOrderBy
            Else
                strSortedField = "  " & strSortedField & " " & strOrderBy
            End If

            If strEmployees = "ALL" And strZones = "ALL" Then
                '## dTbl = API.AccessMoveServiceMethods.GetAccessMovesDataTable(Me.Page, dBegin, dEnd, "Access", Nothing, Nothing, "A", Nothing, strSortedField, oType)
                If (advancedAccess) Then
                    dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, , , , , strZonesAux, , strTypes, , , , strSortedField)
                Else
                    dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, , , , , , , strTypes, , , , strSortedField)
                End If

            ElseIf strEmployees = "ALL" And strZones <> "ALL" Then
                '## dTbl = API.AccessMoveServiceMethods.GetAccessMovesDataTable(Me.Page, dBegin, dEnd, "Access", Nothing, Nothing, "A", oZones, strSortedField, oType)
                dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, , , , , strZonesAux, , strTypes, , , , strSortedField)
            ElseIf strEmployees <> "ALL" And strZones <> "ALL" Then
                '## dTbl = API.AccessMoveServiceMethods.GetAccessMovesDataTable(Me.Page, dBegin, dEnd, "Access", oEmployees, Nothing, "A", oZones, strSortedField, oType)
                dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, strEmployeesAux, , , , strZonesAux, , strTypes, , , , strSortedField)
            ElseIf strEmployees <> "ALL" And strZones = "ALL" Then
                '## dTbl = API.AccessMoveServiceMethods.GetAccessMovesDataTable(Me.Page, dBegin, dEnd, "Access", oEmployees, Nothing, "A", Nothing, strSortedField, oType)
                If (advancedAccess) Then
                    dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, strEmployeesAux, , , , strZonesAux, , strTypes, , , , strSortedField)
                Else
                    dTbl = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, , , dBegin, dEnd, strEmployeesAux, , , , , , strTypes, , , , strSortedField)
                End If
            End If

            If dTbl IsNot Nothing Then
                For Each dRow As DataRow In dTbl.Rows
                    oJFExceptions = New Generic.List(Of JSONFieldItem)

                    oJFExceptions.Add(New JSONFieldItem("ID", dRow("ID"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFExceptions.Add(New JSONFieldItem("ZoneName", roTypes.Any2String(dRow("ZoneName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFExceptions.Add(New JSONFieldItem("IDEmployee", dRow("IDEmployee"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFExceptions.Add(New JSONFieldItem("EmployeeName", roTypes.Any2String(dRow("EmployeeName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFExceptions.Add(New JSONFieldItem("DateTime", dRow("DateTime"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '## oJFExceptions.Add(New JSONFieldItem("IDReader", roTypes.Any2String(dRow("IDReader")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '## If dRow("IDCapture") Is DBNull.Value Then
                    '##     oJFExceptions.Add(New JSONFieldItem("IDCapture", "<span style='display: block; height: 20px;'>&nbsp;</span>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '## Else
                    '##     oJFExceptions.Add(New JSONFieldItem("IDCapture", "<a href='javascript:void(0);' class='btnShowCapture' onclick='showCapture(""" & dRow("IDCapture") & """); return false;'>&nbsp;</a>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '## End If

                    oJFExceptions.Add(New JSONFieldItem("IDTerminal", roTypes.Any2String(dRow("IDTerminal")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFExceptions.Add(New JSONFieldItem("IDCapture", "<span style='display: block; height: 20px;'>&nbsp;</span>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJGException.Add(oJFExceptions)
                Next
            End If

            If oJGException.Count > 0 Then
                strJSONGroups = "{ ""access"": ["
                For Each oObj As Object In oJGException
                    Dim strJSONText As String = ""
                    strJSONText &= "{ ""fields"": "
                    strJSONText &= roJSONHelper.Serialize(oObj)
                    strJSONText &= " } ,"
                    strJSONGroups &= strJSONText
                Next
                If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
                strJSONGroups &= "]}"
            Else
                strJSONGroups = ""
            End If

            Response.Write(strJSONGroups)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

#End Region

End Class