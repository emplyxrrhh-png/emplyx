Imports System.IO
Imports Google.Cloud.Translation.V2
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class roGoogleParser

    Public Shared Sub translateAllJSONUsingGoogleTo(currentPath As String, toLang As String)
        Dim jsonPathFiles As String = currentPath & "Resources\JsonFiles\i18n\"
        Dim jsonDestPathFiles As String = currentPath & "Resources\JsonFiles\i18n\"


        Dim oGoogleTranslator As New roGoogleParser()



    End Sub

    Private Function gtranslate(ByVal inputtext As String, ByVal tolangid As String) As String
        Dim step4 As String
        Dim keyDesLang As String = String.Empty

        Select Case tolangid
            Case "ESP"
                keyDesLang = "es-ES"
            Case "CAT"
                keyDesLang = "ca-ES"
            Case "ENG"
                keyDesLang = "en-US"
            Case "GAL"
                keyDesLang = "gl-ES"
            Case "EKR"
                keyDesLang = "eu-ES"
            Case "ITA"
                keyDesLang = "it-IT"
            Case "FRA"
                keyDesLang = "fr-FR"
            Case "POR"
                keyDesLang = "pt-PT"
            Case "SLK"
                keyDesLang = "sk-SK"
            Case Else
                Console.WriteLine($"Idioma no reconocido {tolangid}")
        End Select


        If keyDesLang <> String.Empty Then
            Try
                Dim oClient As TranslationClient = TranslationClient.CreateFromApiKey("AIzaSyDEu7Qo0HTVOFAw3xDla35s_wMhvY7qiGw")
                Dim oResult As TranslationResult = oClient.TranslateText(inputtext, keyDesLang, "es-ES")
                step4 = oResult.TranslatedText
            Catch ex As Exception
                step4 = inputtext
            End Try
        Else
            step4 = inputtext
        End If
        Return step4
    End Function

End Class
