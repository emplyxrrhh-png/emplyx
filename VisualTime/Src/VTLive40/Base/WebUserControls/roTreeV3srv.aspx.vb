Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTSelectorManager
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_roTreesV3srv
    Inherits NoCachePageBase

    Private oTreeV3 As roTreeV3Item = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case Request("action")
            Case "newItemInGrid"
                newItemInGridExt()

            Case "newChildrenInGrid"
                newChildrenInGridExt()
        End Select

    End Sub

    Private Function FilterSpecialChars(ByVal strValue As String) As String
        Return strValue.Replace("'", "´")
    End Function

    Private Sub newChildrenInGridExt()

        Try

            Dim strValues As String = roTypes.Any2String(Context.Request("ID"))
            Dim strRows As String = roTypes.Any2String(Context.Request("rows"))
            Dim strFeature As String = roTypes.Any2String(Context.Request("feature"))
            Dim strFilter As String = roTypes.Any2String(Context.Request("filter"))
            Dim strFilterUser As String = roTypes.Any2String(Context.Request("filterUser"))
            Dim OnlyGroups As String = roTypes.Any2String(Context.Request("OnlyGroups"))
            Dim ListaItems As New Generic.List(Of roTreeV3Item)()
            If strRows <> String.Empty Then
                ListaItems = roJSONHelper.Deserialize(strRows, ListaItems.GetType())
            End If

            Dim bAgregarItem As Boolean = False
            Dim strAux As String = String.Empty

            Dim tmpLista As String() = Nothing

            If strValues.ToLower().StartsWith("c") Then
                tmpLista = {strValues}
            Else
                tmpLista = strValues.Split(",")
            End If

            If strFeature.Trim.ToLower() = "tasks" Then strFeature = ""

            For Each strID As String In tmpLista

                oTreeV3 = Nothing

                If strID.StartsWith("A") Then 'Grupo



                    'GRUPOS DE EMPLEADOS
                    Dim tb As DataTable = API.EmployeeGroupsServiceMethods.GetChildGroups(Me, strID.Substring(1), strFeature)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each rw As DataRow In tb.Rows

                            oTreeV3 = New roTreeV3Item With {
                                .Id = "A" & roTypes.Any2Integer(rw("ID")),
                                .Nombre = FilterSpecialChars(roTypes.Any2String(rw("Name"))),
                                .Tipo = "G",
                                .Ruta = ">" & roTypes.Any2String(rw("Path")).Replace("\", "#") & "#",
                                .NombreFull = FilterSpecialChars(roTypes.Any2String(rw("FullGroupName")).Replace(" \ ", "-"))
                            }

                            bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                            ListaItems.RemoveAll(AddressOf EliminarInferiores)

                            If bAgregarItem Then
                                ListaItems.Add(oTreeV3)
                            End If

                        Next
                    End If

                    'EMPLEADOS SUELTOS
                    If OnlyGroups <> "1" Then
                        Dim lstIdEmployees As Generic.List(Of Integer) = EmployeeGroupsServiceMethods.GetEmployeeListFromGroupNORecursiveInDates(Me, New Integer() {strID.Substring(1)}, strFeature, "U", strFilter, strFilterUser, DateTime.Today, DateTime.Today)
                        If lstIdEmployees IsNot Nothing AndAlso lstIdEmployees.Count > 0 Then
                            Dim strWhere As String = "ID IN (" & String.Join(",", lstIdEmployees.ToArray()) & ")"
                            Dim lstEmployees As DataTable = EmployeeServiceMethods.GetAllEmployees(Me.Page, strWhere, strFeature)
                            If lstEmployees IsNot Nothing AndAlso lstEmployees.Rows.Count > 0 Then
                                For Each item As DataRow In lstEmployees.Rows
                                    oTreeV3 = New roTreeV3Item With {
                                        .Id = "B" & item("IDEmployee"),
                                        .Nombre = FilterSpecialChars(item("EmployeeName")),
                                        .Tipo = "E",
                                        .Ruta = ">" & item("Path").Replace("\", "#") & "#",
                                        .NombreFull = FilterSpecialChars(item("FullGroupName").Replace(" \ ", "-") & "-" & item("EmployeeName"))
                                    }
                                    bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                                    ListaItems.RemoveAll(AddressOf EliminarInferiores)

                                    If bAgregarItem Then
                                        ListaItems.Add(oTreeV3)
                                    End If

                                Next
                            End If
                        End If
                    End If
                ElseIf strID.StartsWith("C") Then 'Agrupacion de la ficha 'Ejemplo: CUsr_Grupo~1
                    Dim strUserField As String = strID.Substring(1).Split("~")(0)
                    Dim strUserFieldValue As String = strID.Substring(1).Split("~")(1)

                    If strFilterUser <> String.Empty Then
                        strFilterUser = roSelectorManager.BuildUserFieldsWhere(strFilterUser)
                    End If
                    Dim tbEmployees As DataTable = API.UserFieldServiceMethods.GetEmployeesFromUserFieldWithType(Me, strUserField, strUserFieldValue, strFilterUser, "Employees")
                    If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then

                        Dim bolFilterCurrent As Boolean = True
                        Dim bolFilterMovility As Boolean = True
                        Dim bolFilterOld As Boolean = True
                        Dim bolFilterFuture As Boolean = True
                        Dim bolFilterUserFields As Boolean = False

                        If strFilter <> "" Then
                            If strFilter.Substring(0, 1) = "1" Then bolFilterCurrent = True Else bolFilterCurrent = False
                            If strFilter.Substring(1, 1) = "1" Then bolFilterMovility = True Else bolFilterMovility = False
                            If strFilter.Substring(2, 1) = "1" Then bolFilterOld = True Else bolFilterOld = False
                            If strFilter.Substring(3, 1) = "1" Then bolFilterFuture = True Else bolFilterFuture = False
                            If strFilter.Substring(4, 1) = "1" Then bolFilterUserFields = True Else bolFilterUserFields = False
                        End If

                        Dim oDataView As System.Data.DataView = New Data.DataView(tbEmployees)
                        oDataView.Sort = "EmployeeName"

                        If oDataView.Count > 0 Then
                            Dim bolFilter As Boolean = False
                            For Each oDataviewRow In oDataView

                                bolFilter = (bolFilterCurrent And oDataviewRow("Type") = 1 Or
                                             bolFilterMovility And oDataviewRow("Type") = 2 Or
                                             bolFilterOld And oDataviewRow("Type") = 3 Or
                                             bolFilterFuture And oDataviewRow("Type") = 4)

                                If bolFilter Then

                                    oTreeV3 = New roTreeV3Item With {
                                        .Id = "B" & oDataviewRow("IDEmployee"),
                                        .Nombre = FilterSpecialChars(oDataviewRow("EmployeeName")),
                                        .Tipo = "E",
                                        .Ruta = ">" & oDataviewRow("Path").Replace("\", "#") & "#",
                                        .NombreFull = FilterSpecialChars(oDataviewRow("FullGroupName").Replace(" \ ", "-") & "-" & oDataviewRow("EmployeeName"))
                                    }

                                    bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                                    ListaItems.RemoveAll(AddressOf EliminarInferiores)

                                    If bAgregarItem Then
                                        ListaItems.Add(oTreeV3)
                                    End If

                                End If

                            Next
                        End If
                    End If
                End If

            Next

            'Dim LstTemp = From l In ListaItems Select l Order By l.NombreFull
            Dim LstTemp = From l In ListaItems Select l

            Dim x As New StringBuilder(strAux)

            If LstTemp.Any() Then

                For Each item As roTreeV3Item In LstTemp

                    x.Append("{fields:[{ field: 'id', value: '" & item.Id & "' }, " &
                                       "{ field: 'tipo', value: '" & item.Tipo & "' }, " &
                                       "{ field: 'nombre', value: '" & item.Nombre.Replace("'", "\'") & "' }, " &
                                       "{ field: 'nombrefull', value: '" & item.NombreFull.Replace("'", "\'") & "' }, " &
                                       "{ field: 'ruta', value: '" & item.Ruta & "'}]},")
                Next
            End If

            strAux = x.ToString

            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Context.Response.Write(rError.toJSON)
        End Try

    End Sub

    Private Sub newItemInGridExt()

        Try

            Dim strValues As String = roTypes.Any2String(Context.Request("ID"))
            Dim strRows As String = roTypes.Any2String(Context.Request("rows"))
            Dim strFeature As String = roTypes.Any2String(Context.Request("feature"))
            Dim strFilter As String = roTypes.Any2String(Context.Request("filter"))
            Dim strFilterUser As String = roTypes.Any2String(Context.Request("filterUser"))

            Dim ListaItems As New Generic.List(Of roTreeV3Item)()
            If strRows <> String.Empty Then
                ListaItems = roJSONHelper.Deserialize(strRows, ListaItems.GetType())
            End If

            Dim bAgregarItem As Boolean = False
            Dim strAux As String = String.Empty

            Dim tmpLista As String() = Nothing

            If strValues.ToLower().StartsWith("c") Then
                tmpLista = {strValues}
            Else
                tmpLista = strValues.Split(",")
            End If

            If strFeature.Trim.ToLower() = "tasks" Then strFeature = ""
            For Each strID As String In tmpLista

                oTreeV3 = Nothing

                If strID.StartsWith("A") Then 'Grupo
                    Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, strID.Substring(1), False)
                    If oGroup IsNot Nothing AndAlso oGroup.Name IsNot Nothing AndAlso oGroup.Name.Length > 0 Then

                        oTreeV3 = New roTreeV3Item With {
                            .Id = strID,
                            .Nombre = FilterSpecialChars(oGroup.Name),
                            .Tipo = "G",
                            .Ruta = ">" & oGroup.Path.Replace("\", "#") & "#",
                            .NombreFull = FilterSpecialChars(oGroup.FullGroupName.Replace(" \ ", "-"))
                        }

                        bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                        ListaItems.RemoveAll(AddressOf EliminarInferiores)

                        If bAgregarItem Then
                            ListaItems.Add(oTreeV3)
                        End If

                    End If

                ElseIf strID.StartsWith("B") Then 'Empleado
                    Dim strWhere As String = "ID IN (" & strID.Substring(1) & ")"
                    Dim lstEmployees As DataTable = EmployeeServiceMethods.GetAllEmployees(Me.Page, strWhere, strFeature)

                    If lstEmployees IsNot Nothing AndAlso lstEmployees.Rows.Count > 0 Then

                        oTreeV3 = New roTreeV3Item With {
                            .Id = "B" & lstEmployees.Rows(0)("IDEmployee"),
                            .Nombre = FilterSpecialChars(lstEmployees.Rows(0)("EmployeeName")),
                            .Tipo = "E",
                            .Ruta = ">" & lstEmployees.Rows(0)("Path").Replace("\", "#") & "#",
                            .NombreFull = FilterSpecialChars(lstEmployees.Rows(0)("FullGroupName").Replace(" \ ", "-") & "-" & lstEmployees.Rows(0)("EmployeeName"))
                        }

                        bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                        ListaItems.RemoveAll(AddressOf EliminarInferiores)

                        If bAgregarItem Then
                            ListaItems.Add(oTreeV3)
                        End If

                    End If

                ElseIf strID.StartsWith("C") Then 'Agrupacion de la ficha 'Ejemplo: CUsr_Grupo~1
                    Dim strUserField As String = strID.Substring(1).Split("~")(0)
                    Dim strUserFieldValue As String = strID.Substring(1).Split("~")(1).Replace("#sep3#", "'").Replace("#sep1#", "\")

                    If strFilterUser <> String.Empty Then
                        strFilterUser = roSelectorManager.BuildUserFieldsWhere(strFilterUser)
                    End If

                    Dim tbEmployees As DataTable = API.UserFieldServiceMethods.GetEmployeesFromUserFieldWithType(Me, strUserField, strUserFieldValue, strFilterUser, strFeature)
                    If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then

                        Dim bolFilterCurrent As Boolean = True
                        Dim bolFilterMovility As Boolean = True
                        Dim bolFilterOld As Boolean = True
                        Dim bolFilterFuture As Boolean = True
                        Dim bolFilterUserFields As Boolean = False

                        If strFilter <> "" Then
                            If strFilter.Substring(0, 1) = "1" Then bolFilterCurrent = True Else bolFilterCurrent = False
                            If strFilter.Substring(1, 1) = "1" Then bolFilterMovility = True Else bolFilterMovility = False
                            If strFilter.Substring(2, 1) = "1" Then bolFilterOld = True Else bolFilterOld = False
                            If strFilter.Substring(3, 1) = "1" Then bolFilterFuture = True Else bolFilterFuture = False
                            If strFilter.Substring(4, 1) = "1" Then bolFilterUserFields = True Else bolFilterUserFields = False
                        End If

                        Dim oDataView As System.Data.DataView = New Data.DataView(tbEmployees)
                        oDataView.Sort = "EmployeeName"

                        If oDataView.Count > 0 Then
                            Dim bolFilter As Boolean = False
                            For Each oDataviewRow In oDataView

                                bolFilter = (bolFilterCurrent And oDataviewRow("Type") = 1 Or
                                             bolFilterMovility And oDataviewRow("Type") = 2 Or
                                             bolFilterOld And oDataviewRow("Type") = 3 Or
                                             bolFilterFuture And oDataviewRow("Type") = 4)

                                If bolFilter Then

                                    oTreeV3 = New roTreeV3Item With {
                                        .Id = "B" & oDataviewRow("IDEmployee"),
                                        .Nombre = FilterSpecialChars(oDataviewRow("EmployeeName")),
                                        .Tipo = "E",
                                        .Ruta = ">" & oDataviewRow("Path").Replace("\", "#") & "#",
                                        .NombreFull = FilterSpecialChars(oDataviewRow("FullGroupName").Replace(" \ ", "-") & "-" & oDataviewRow("EmployeeName"))
                                    }

                                    bAgregarItem = ListaItems.TrueForAll(AddressOf TodosSonInferiores)
                                    ListaItems.RemoveAll(AddressOf EliminarInferiores)

                                    If bAgregarItem Then
                                        ListaItems.Add(oTreeV3)
                                    End If

                                End If

                            Next
                        End If
                    End If
                End If

            Next

            'Dim LstTemp = From l In ListaItems Select l Order By l.NombreFull
            Dim LstTemp = From l In ListaItems Select l

            Dim x As New StringBuilder(strAux)

            If LstTemp.Any() Then

                For Each item As roTreeV3Item In LstTemp

                    x.Append("{fields:[{ field: 'id', value: '" & item.Id & "' }, " &
                                       "{ field: 'tipo', value: '" & item.Tipo & "' }, " &
                                       "{ field: 'nombre', value: '" & item.Nombre & "' }, " &
                                       "{ field: 'nombrefull', value: '" & item.NombreFull & "' }, " &
                                       "{ field: 'ruta', value: '" & item.Ruta & "'}]},")
                Next
            End If

            strAux = x.ToString
            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Context.Response.Write(rError.toJSON)
        End Try

    End Sub

    Private Function TodosSonInferiores(ByVal Item As roTreeV3Item) As Boolean

        If oTreeV3 Is Nothing Then Return True

        If oTreeV3.Tipo = "G" Then 'Grupo
            If Item.Tipo = "G" AndAlso oTreeV3.Ruta.Contains(Item.Ruta) Then Return False 'el grupo de la lista es inferior al grupo nuevo

        ElseIf oTreeV3.Tipo = "E" Then 'Empleado
            If Item.Tipo = "E" AndAlso oTreeV3.Id = Item.Id Then Return False 'el empleado de la lista y el nuevo es el mismo
        End If

        Return True

    End Function

    Private Function EliminarInferiores(ByVal Item As roTreeV3Item) As Boolean

        If oTreeV3 Is Nothing Then Return False

        If oTreeV3.Tipo = "G" Then 'Grupo
            If Item.Tipo = "G" AndAlso Item.Ruta.Contains(oTreeV3.Ruta) AndAlso oTreeV3.Id <> Item.Id Then Return True 'el grupo de la lista es inferior al grupo nuevo
        End If

        Return False

    End Function

End Class

Public Class Comparer_roTreeV3Item
    Implements IComparer(Of roTreeV3Item)

    Public Function Compare(ByVal x As roTreeV3Item, ByVal y As roTreeV3Item) As Integer Implements System.Collections.Generic.IComparer(Of roTreeV3Item).Compare
        Return String.Compare(x.NombreFull, y.NombreFull)
    End Function

End Class