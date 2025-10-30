''' <summary>
''' Occurs when a functionalities check query is missing or not correctly formed.
''' </summary>
''' <remarks>An example of correct format is
''' U:Tasks.Manage=Read OR E:Tasks.Manage=Read</remarks>
Public Class FeaturesFormatException
    Inherits Exception

    Public Sub New(ByVal functionalities As String)
        MyBase.New(GetMessage(functionalities))
    End Sub

    Private Shared Function GetMessage(ByVal functionalities As String) As String
        If functionalities Is Nothing OrElse functionalities.Trim().Length = 0 Then
            Return "The functionalities query is missing. An example of correct format is 'U:Tasks.Manage=Read OR E:Tasks.Manage=Read', or 'U'."
        Else
            Return "The functionalities query is not correctly formed: " + functionalities + ".  An example of correct format is 'U:Tasks.Manage=Read OR E:Tasks.Manage=Read', or 'U'."
        End If
    End Function

End Class