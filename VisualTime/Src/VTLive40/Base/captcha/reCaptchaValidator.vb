Imports Newtonsoft.Json
Imports Robotics.VTBase

Public Class ReCaptchaValidator

    Public Shared Function Validate(ByVal EncodedResponse As String) As Boolean

        Dim client = New System.Net.WebClient()

        Dim PrivateKey As String = "6LfSKR4pAAAAANqZZ3Ouygr9rwbE8HFIj10Oo-rs"

        Dim GoogleReply = client.DownloadString(String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse))

        Dim captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ReCaptchaValidator)(GoogleReply)

        Return roTypes.Any2Boolean(captchaResponse.Success)
    End Function

    <JsonProperty("success")>
    Public Property Success() As String
        Get
            Return m_Success
        End Get
        Set(value As String)
            m_Success = value
        End Set
    End Property

    Private m_Success As String

    <JsonProperty("error-codes")>
    Public Property ErrorCodes() As List(Of String)
        Get
            Return m_ErrorCodes
        End Get
        Set(value As List(Of String))
            m_ErrorCodes = value
        End Set
    End Property

    Private m_ErrorCodes As List(Of String)

End Class