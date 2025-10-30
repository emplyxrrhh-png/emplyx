Option Strict On

Namespace VTDiagCore.QueryService

    Public Class Parameter
        Public Property Name As String
        Public Property Description As String
        Public Property Type As ParameterType

        Public Function CalculateDBType() As DbType
            Select Case Type
                Case ParameterType.Integer
                    Return DbType.Int32
                Case ParameterType.Decimal
                    Return DbType.Decimal
                Case ParameterType.Date
                    Return DbType.Date
                Case ParameterType.Time
                    Return DbType.Time
                Case ParameterType.Text
                    Return DbType.String
                Case Else
                    Throw New NotImplementedException("The current Type don't have a conversion defined.")
            End Select
        End Function

    End Class

End Namespace