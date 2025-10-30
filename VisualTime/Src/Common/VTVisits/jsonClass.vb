Imports System.Runtime.Serialization

Namespace VTVisits

    <DataContract>
    Public Class roUser
        <DataMember>
        Public token As [String]
        <DataMember()>
        Public userid As Integer
        <DataMember()>
        Public language As String
        <DataMember>
        Public status As Long
        <DataMember>
        Public hasaccess As Integer
        <DataMember>
        Public hascreate As Integer
        <DataMember>
        Public hascreateEmployee As Integer
        <DataMember>
        Public haschange As Integer
        <DataMember>
        Public hasfields As Integer
        <DataMember>
        Public idemployee As Integer
        <DataMember>
        Public showLegalText As Boolean

    End Class

    <DataContract>
    Public Class Lastchange
        <DataMember>
        Public lastdate As New DateTime(1970, 1, 1)
        <DataMember>
        Public result As String = "NoError"
    End Class

    <DataContract>
    Public Class BackgroundPhoto
        <DataMember>
        Public url As String = ""
        <DataMember>
        Public base64 As String = ""
        <DataMember>
        Public result As String = "NoError"
    End Class

    <DataContract()>
    Public Class SSO_Windows_Data
        <DataMember()>
        Public SSOEnabled As Boolean

        <DataMember()>
        Public SSOUser As String

        <DataMember()>
        Public SSOLoggedIn As Boolean

        <DataMember()>
        Public Status As Long
    End Class

    <DataContract()>
    Public Class IsAlive
        <DataMember()>
        Public ServerVersion As String

        <DataMember()>
        Public Status As Long
    End Class

End Namespace