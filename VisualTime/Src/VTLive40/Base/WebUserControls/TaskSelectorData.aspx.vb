Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_TaskSelectorData
    Inherits NoCachePageBase

    <Runtime.Serialization.DataContract()>
    Private Class roTreeTaskItem

        <Runtime.Serialization.DataMember(Name:="id")>
        Public Id As String

        <Runtime.Serialization.DataMember(Name:="type")>
        Public TypeItem As String

        <Runtime.Serialization.DataMember(Name:="icon")>
        Public Icon As String

        <Runtime.Serialization.DataMember(Name:="name")>
        Public Name As String

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strAction As String = roTypes.Any2String(Request.Params("action"))
        If strAction <> String.Empty Then

            Select Case strAction
                Case "FieldFindData"

                    LoadTasksSearch()

                Case "LoadInitialData"

                    LoadInitialData()

                Case Else
                    Me.Response.Clear()
                    Me.Response.ContentType = "text/html"
                    Me.Response.Write("/source/")

            End Select
        Else
            Me.Response.Clear()
            Me.Response.ContentType = "text/html"
            Me.Response.Write("")
        End If

    End Sub

    Private Sub LoadInitialData()

        Dim strTaskValues As String = roTypes.Any2String(Context.Request("TaskValues"))
        Dim strProjectValues As String = roTypes.Any2String(Context.Request("ProjectValues"))

        Dim oTreeTaskItem As roTreeTaskItem = Nothing
        Dim ListaItems As New Generic.List(Of roTreeTaskItem)()

        Dim strAux As String = String.Empty

        If strTaskValues <> String.Empty Then
            Dim tmpLista As String() = strTaskValues.Split(",")
            Dim strText As String
            For Each strID As String In tmpLista
                Try
                    strText = TasksServiceMethods.GetNameTask(Me, strID)
                    If strText <> String.Empty Then
                        oTreeTaskItem = New roTreeTaskItem()
                        oTreeTaskItem.Id = FilterSpecialChars(strID)
                        oTreeTaskItem.Name = FilterSpecialChars(strText)
                        oTreeTaskItem.Icon = "<img src=\'" & ResolveUrl("~/Base/Images/TaskSelector/task16.png") & "\'/>"
                        oTreeTaskItem.TypeItem = "task"
                        ListaItems.Add(oTreeTaskItem)
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TaskSelectorData::LoadInitialData::Could not parse taskID", ex)
                End Try


            Next
        End If

        If strProjectValues <> String.Empty Then
            Dim tmpLista As String() = strProjectValues.Split(",")
            Dim tbProjects As DataTable
            For Each strID As String In tmpLista
                Try
                    tbProjects = TasksServiceMethods.GetProjectsByName(Me, strID)
                    If tbProjects IsNot Nothing AndAlso tbProjects.Rows.Count > 0 Then
                        Dim oDataView As System.Data.DataView = New Data.DataView(tbProjects)
                        oDataView.RowFilter = "Name = '" & strID & "'"
                        If oDataView.Count > 0 Then
                            oTreeTaskItem = New roTreeTaskItem()
                            oTreeTaskItem.Id = FilterSpecialChars(strID)
                            oTreeTaskItem.Name = FilterSpecialChars(oDataView(0)("Name"))
                            oTreeTaskItem.Icon = "<img src=\'" & ResolveUrl("~/Base/Images/TaskSelector/project16.png") & "\'/>"
                            oTreeTaskItem.TypeItem = "project"
                            ListaItems.Add(oTreeTaskItem)
                        End If
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TaskSelectorData::LoadInitialData::Could not parse projectID", ex)
                End Try

            Next
        End If

        If ListaItems.Count > 0 Then
            For Each item As roTreeTaskItem In ListaItems

                strAux &= "{fields:[{ field: 'id', value: '" & item.Id & "' }, " &
                                   "{ field: 'type', value: '" & item.TypeItem & "' }, " &
                                   "{ field: 'icon', value: '" & item.Icon & "' }, " &
                                   "{ field: 'name', value: '" & item.Name.Replace("'", "\'") & "'}]},"
            Next
            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Else
            Me.Response.Clear()
            Me.Response.ContentType = "text/html"
            Me.Response.Write("")
        End If

    End Sub

    Private Sub LoadTasksSearch()

        Dim strFieldFindColumn As String = ""
        Dim strFieldFindValue As String = ""
        Dim strArrayNodes As String = ""
        Dim strImagePath As String = ResolveUrl("~/Base/Images/TaskSelector/")

        Dim FieldFind As String = roTypes.Any2String(Request.Params("FieldFindColumn"))
        If FieldFind <> String.Empty Then
            strFieldFindColumn = FieldFind
        End If

        FieldFind = roTypes.Any2String(Request.Params("FieldFindValue"))
        If FieldFind <> String.Empty Then
            strFieldFindValue = FieldFind
        End If

        Dim tbItems As DataTable = Nothing

        Dim strTextoABuscar As String = strFieldFindValue.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
        If Not strTextoABuscar.StartsWith("%") Then strTextoABuscar = "%" & strTextoABuscar
        If Not strTextoABuscar.EndsWith("%") Then strTextoABuscar = strTextoABuscar & "%"

        Select Case strFieldFindColumn.ToLower
            Case "task"
                tbItems = TasksServiceMethods.GetTasksByName(Nothing, strTextoABuscar)
            Case "project"
                tbItems = TasksServiceMethods.GetProjectsByName(Nothing, strTextoABuscar)
            Case Else
                Dim strWhere As String = String.Empty
                If strFieldFindColumn <> "" Then
                    strWhere = strFieldFindColumn & " LIKE '" & strTextoABuscar
                End If
                tbItems = TasksServiceMethods.GetTasksByName(Nothing, strTextoABuscar)
        End Select

        If tbItems IsNot Nothing AndAlso tbItems.Rows.Count > 0 Then

            Dim oDataView As System.Data.DataView = New Data.DataView(tbItems)
            oDataView.Sort = "Name"

            If strFieldFindColumn.ToLower = "project" Then

                For Each oDataviewRow As Data.DataRowView In oDataView
                    strArrayNodes &= "{ 'id':'" & oDataviewRow("Name") & "', 'text':'" &
                                      Me.FilterSpecialChars(roTypes.Any2String(oDataviewRow("Name"))) & "', 'leaf':true, 'icon': '" & strImagePath & "Project16.png" & "'},"
                Next
            Else

                For Each oDataviewRow As Data.DataRowView In oDataView
                    strArrayNodes &= "{ 'id':'" & oDataviewRow("ID") & "', 'text':'" &
                                      Me.FilterSpecialChars(roTypes.Any2String(oDataviewRow("Name"))) & "', 'leaf':true, 'icon': '" & strImagePath & "Task16.png" & "'},"
                Next
            End If

        End If

        If strArrayNodes <> "" Then
            strArrayNodes = "[" & strArrayNodes.Substring(0, strArrayNodes.Length - 1) & "]"
        End If

        Me.Response.Clear()
        Me.Response.ContentType = "text/html"
        If strArrayNodes = "" Then strArrayNodes = "[]"
        Me.Response.Write(strArrayNodes)

    End Sub

    Private Function FilterSpecialChars(ByVal strValue As String) As String
        Return strValue.Replace("'", "&#39;").Replace("\", "&#92;")
    End Function

End Class