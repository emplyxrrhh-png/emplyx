Namespace Robotics.Base.DTOs

    Public Class roFTPConnection

        Public Sub New(ByVal client As String, ByVal ftpInfo As FTPInfo, ByVal id As String, ByVal storageFolder As String, ByVal action As String)
            Me.client = client
            Me.ftps = ftpInfo
            Me.Id = id
            Me.storageFolder = storageFolder
            Me.action = action
        End Sub

        Public Property client As String
        Public Property ftps As FTPInfo
        Public Property Id As String
        Public Property storageFolder As String
        Public Property action As String
    End Class

    Public Class FTPInfo

        Public Sub New(ByVal server As String, ByVal user As String, ByVal password As String, ByVal secretKeyId As String, ByVal folder As String)
            Me.server = server
            Me.user = user
            Me.password = password
            Me.secretKeyId = secretKeyId
            Me.folder = folder
        End Sub

        Public Property server As String
        Public Property user As String
        Public Property password As String
        Public Property secretKeyId As String
        Public Property folder As String
    End Class

End Namespace