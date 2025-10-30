Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roFilterTaskTemplateSelectorsrv
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case roTypes.Any2String(Request("action"))
            Case "newItemInGrid"
                newItemInGrid()
        End Select

    End Sub

    Private Sub newItemInGrid()

        Try

            Dim strAux As String = ""
            Dim sFilter As String = roTypes.Any2String(Request("KeyFilter"))
            Dim acProject As String = roTypes.Any2String(Request("acProject"))

            Dim projectID As Integer = roTypes.Any2Integer(acProject)
            Dim currentTask As Integer = roTypes.Any2Integer(sFilter)

            Dim tbTask As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(projectID, "", Me.Page, "", False)
            For Each row As DataRow In tbTask.Rows
                If (roTypes.Any2Integer(row("ID")) <> currentTask) Then
                    strAux &= "{fields:[{ field: 'id', value: '" & roTypes.Any2Integer(row("ID")) & "' }, " &
                                       "{ field: 'name', value: '" & roTypes.Any2String(row("Name")).Replace("\", "\\").Replace("'", "\'") & "'}]},"
                End If
            Next
            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Context.Response.Write(rError.toJSON)
        End Try

    End Sub

End Class