Public Class roPushServerHelper

    Public Function GetPushRawMessageFromHttpRequest(request As HttpRequestBase) As String
        Dim strIncomingMessage As String = String.Empty
        Try
            Dim SN As String = request.Params("SN")
            ' Monto mensaje recibido para reutilizar Driver de comunicaciones DriverZKPush2
            ' Cabecera
            Dim strMethod As String = request.HttpMethod
            Dim strRequestRawURL As String = request.RawUrl
            Dim strHttpVersion As String = request.Params("SERVER_PROTOCOL")
            Dim strAllRaw As String = request.Params("ALL_RAW")

            ' Contenido
            Dim httpInputStream As System.IO.Stream
            Dim strMessageContent As String

            httpInputStream = request.InputStream
            Dim httpInputBytes(httpInputStream.Length) As Byte
            httpInputStream.Read(httpInputBytes, 0, httpInputStream.Length - 1)
            strMessageContent = Text.Encoding.Default.GetString(httpInputBytes)

            strIncomingMessage = strMethod & " " & strRequestRawURL & " " & strHttpVersion
            strIncomingMessage = strIncomingMessage & vbCrLf & strAllRaw
            strIncomingMessage = strIncomingMessage & vbCrLf & strMessageContent
        Catch ex As Exception
            strIncomingMessage = String.Empty
        End Try
        Return strIncomingMessage
    End Function

End Class