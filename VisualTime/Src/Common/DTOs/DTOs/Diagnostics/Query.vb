Option Strict On

Namespace Robotics.Base.DTOs.VTDiag

    Public Class Query
        Public Property Id As Integer
        Public Property Name As String
        Public Property Description As String
        Public Property Parameters As List(Of Parameter)
    End Class

End Namespace