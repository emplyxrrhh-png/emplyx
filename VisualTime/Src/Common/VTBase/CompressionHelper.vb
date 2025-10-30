Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text

Public Class CompressionHelper
    ' Método para comprimir un string usando GZip y convertirlo a Base64
    Public Shared Function CompressString(text As String) As String
        Dim inputBytes As Byte() = Encoding.UTF8.GetBytes(text)
        Using outputStream As New MemoryStream()
            Using gZipStream As New GZipStream(outputStream, CompressionMode.Compress)
                gZipStream.Write(inputBytes, 0, inputBytes.Length)
            End Using
            Return Convert.ToBase64String(outputStream.ToArray())
        End Using
    End Function

    ' Método para descomprimir un string comprimido en Base64 usando GZip
    Public Shared Function DecompressString(compressedText As String) As String
        Dim inputBytes As Byte() = Convert.FromBase64String(compressedText)
        Using inputStream As New MemoryStream(inputBytes)
            Using gZipStream As New GZipStream(inputStream, CompressionMode.Decompress)
                Using outputStream As New MemoryStream()
                    gZipStream.CopyTo(outputStream)
                    Return Encoding.UTF8.GetString(outputStream.ToArray())
                End Using
            End Using
        End Using
    End Function
End Class
