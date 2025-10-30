Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roUserFieldCriteria
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case Request("action")
            Case "getUserFieldList" ' Retorna una llista de parametres pel combo
                Me.Controls.Clear()
                getUserFieldList()
        End Select

    End Sub

    Private Sub getUserFieldList()
        Try
            Dim oFieldName As String = Request("fieldName")
            Dim oUserField As roUserField
            oUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, oFieldName, Types.EmployeeField, False, False)

            Dim strOutput As String = ""

            For Each strValue As String In oUserField.ListValues
                strOutput &= "'" & strValue & "',"
            Next
            strOutput = strOutput.Substring(0, Len(strOutput) - 1)

            Response.Write(strOutput)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message.ToString)
        End Try
    End Sub

End Class