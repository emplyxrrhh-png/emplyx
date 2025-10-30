Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Tasks_srvTasks
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"
    Private oPermission As Permission

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Me.Controls.Clear()
            Select Case Request("action")

                'Case "saveXTask" ' Graba la tarea
                '    Me.Controls.Clear()
                '    SaveTaskDataX()

                Case "deleteXTask" 'Elimina la tarea
                    Me.Controls.Clear()
                    DeleteXTask()
                Case "getEmployeesSelected"
                    Me.Controls.Clear()
                    GetEmployeesSelected()
                Case "getTaskFieldList" ' Retorna la lista de valores
                    Me.Controls.Clear()
                    GetListFieldValues()
            End Select

        End If

    End Sub

    Private Sub GetEmployeesSelected()
        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                Dim rError As roJSON.JSONError
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim strNodesSelected As String = roTypes.Any2String(Request("NodesSelected"))

            If strNodesSelected <> String.Empty Then

                Dim strEmployees As String = ""
                Dim strGroups As String = ""

                'A8,B122,B127,A9,B129,B140,B133,B147,A7
                Dim arrNodes = strNodesSelected.Split(",")
                For Each node As String In arrNodes
                    If node.StartsWith("B") Then
                        strEmployees &= node.Substring(1) & ","
                    ElseIf node.StartsWith("A") Then
                        strGroups &= node.Substring(1) & ","
                    End If
                Next

                If strEmployees.Length > 0 Then strEmployees = strEmployees.Substring(0, strEmployees.Length() - 1)
                If strGroups.Length > 0 Then strGroups = strGroups.Substring(0, strGroups.Length() - 1)

                Dim nEmployees As Double = TasksServiceMethods.GetEmployeesFromTask(Me, 0, strEmployees, strGroups)
                Dim rOK As New roJSON.JSONError(False, "OK:" & nEmployees.ToString)
                Response.Write(rOK.toJSON)
            Else
                Dim rOK As New roJSON.JSONError(False, "OK:0")
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub GetListFieldValues()
        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                Dim rError As roJSON.JSONError
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim FieldID As Integer = roTypes.Any2Integer(Request("fieldListID"))

            If FieldID > 0 Then

                Dim oFieldTask As New roTaskFieldDefinition
                Dim oData As New Generic.List(Of String)

                Dim strOutput As String = ""

                oFieldTask = API.UserFieldServiceMethods.GetTaskField(Me.Page, FieldID, False)
                If Not oFieldTask Is Nothing Then
                    If Not oFieldTask.ListValues Is Nothing Then
                        For Each strValue As String In oFieldTask.ListValues
                            strOutput &= "'" & strValue.Replace("'", "") & "',"
                        Next
                    End If
                End If

                If strOutput.Length > 0 Then
                    strOutput = strOutput.Substring(0, Len(strOutput) - 1)
                End If

                Response.Write(strOutput)
            Else
                Response.Write("")
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina la tarea
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteXTask()
        Try

            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim IdTask As Integer = roTypes.Any2Integer(Request("ID"))
            If IdTask < 0 Then Exit Sub

            If TasksServiceMethods.DeleteTaskByID(Me, IdTask, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.TaskState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Copia una tarea
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CopyXTask()
        Try

            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim IdTask As Integer = roTypes.Any2Integer(Request("ID"))
            If IdTask < 0 Then Exit Sub

            Dim IdTaskNew As Integer = TasksServiceMethods.CopyTask(Me, IdTask)

            If IdTaskNew = -1 Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.TaskState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK:" & IdTaskNew)
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

End Class