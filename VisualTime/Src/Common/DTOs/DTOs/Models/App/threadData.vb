Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Class roThreadData

        Public Property AppName As String
        Public Property DefaultLogLevel As String
        Public Property DefaultTraceLevel As String
        Public Property PoolName As String
        Public Property DBConnectionString As String
        Public Property ReadDBConnectionString As String
        Public Property Company As String
        Public Property License As String
        Public Property RequestGUID As String
        Public Property SystemPassportID As String

    End Class

    Public Class BaseTaskResult
        Public Property Result As Boolean
        Public Property Description As String
    End Class

End Namespace