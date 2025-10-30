Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roFilterListSelectorsrv
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

            If sFilter <> String.Empty Then
                sFilter = "%" & sFilter & "%"
            End If

            Dim tbTask As DataTable = API.TasksServiceMethods.GetTasksByName(Me.Page, sFilter)
            For Each row As DataRow In tbTask.Rows

                strAux &= "{fields:[{ field: 'id', value: '" & roTypes.Any2Integer(row("ID")) & "' }, " &
                                   "{ field: 'name', value: '" & roTypes.Any2String(row("Name")).Replace(Environment.NewLine, "").Replace("'", "\'") & "'}]},"
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