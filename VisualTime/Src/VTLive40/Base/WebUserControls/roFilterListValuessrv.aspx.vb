Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roFilterListValuessrv
    Inherits SrvPageBase

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

            Dim intIDField As Integer = roTypes.Any2Integer(Request("IDField"))

            If sFilter <> String.Empty Then
                sFilter = "%" & sFilter & "%"
            End If

            Dim oFieldTask As roTaskFieldDefinition

            Dim oData As New Generic.List(Of String)
            oFieldTask = API.UserFieldServiceMethods.GetTaskField(Me.Page, intIDField, False)
            If Not oFieldTask Is Nothing Then
                If Not oFieldTask.ListValues Is Nothing Then
                    Dim i As Integer = 1
                    If oFieldTask.Type = FieldTypes.tNumeric Then
                        For Each strValue As String In oFieldTask.ListValues
                            strAux &= "{fields:[{ field: 'id', value: '" & i & "' }, " &
                       "{ field: 'name', value: '" & roTypes.Any2Double(strValue.Replace(".", HelperWeb.GetDecimalDigitFormat())).ToString("F2") & "'}]},"
                            i = i + 1
                        Next
                    Else
                        For Each strValue As String In oFieldTask.ListValues
                            strAux &= "{fields:[{ field: 'id', value: '" & i & "' }, " &
                       "{ field: 'name', value: '" & strValue.Replace("'", "\'") & "'}]},"
                            i = i + 1
                        Next
                    End If

                End If
            End If

            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Context.Response.Write(rError.toJSON)
        End Try

    End Sub

End Class