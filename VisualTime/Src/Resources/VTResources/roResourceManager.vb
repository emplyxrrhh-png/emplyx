Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Text

Public Class roResourceManager

    Public Sub New()

    End Sub

    Public Function GetString(ByVal strFileName As String, ByVal strKey As String, ByVal sDefaultText As String) As String
        Dim ret As String
        Try
            Dim translateKey As String = strKey
            If Not strKey.StartsWith("RES_") Then translateKey = $"RES_{strKey}"

            Dim rm As New ResourceManager("VTResources." & strFileName, Assembly.GetExecutingAssembly())
            ret = rm.GetString(translateKey)
        Catch ex As MissingManifestResourceException
            ret = GetCommonString(strFileName, strKey, sDefaultText)
        Catch ex As Exception
            WriteMissingLanguageTag(strFileName, strKey, "es-ES", sDefaultText)
            ret = "NotFound"
        End Try

        If ret.ToLower = "(noentry)" Then
            WriteMissingLanguageTag(strFileName, strKey, "es-ES", sDefaultText)
        End If

        ret = ret.Replace("\\", "\")

        Return ret
    End Function

    Public Function GetCommonString(ByVal strFileName As String, ByVal strKey As String, ByVal sDefaultText As String) As String
        Dim ret As String
        Try
            Dim rm As New ResourceManager("VTResources." & strFileName, Assembly.GetExecutingAssembly())
            ret = rm.GetString(strKey)
        Catch ex As MissingManifestResourceException
            ret = GetDefaultTranslation(strFileName, strKey, sDefaultText)
        Catch ex As Exception
            WriteMissingLanguageTag(strFileName, strKey, "es-ES", sDefaultText)
            ret = "NotFound"
        End Try

        If ret.ToLower = "(noentry)" Then
            WriteMissingLanguageTag(strFileName, strKey, "es-ES", sDefaultText)
        End If

        ret = ret.Replace("\\", "\")

        Return ret
    End Function


    Private Function GetDefaultTranslation(strFileName As String, strKey As String, ByVal sDefaultText As String, Optional ByVal culture As CultureInfo = Nothing) As String
        Dim ret As String

        If culture Is Nothing Then culture = Threading.Thread.CurrentThread.CurrentCulture

        WriteMissingLanguageTag(strFileName, strKey, culture.Name, sDefaultText)

        Try
            If culture.Name <> "es-ES" Then
                Dim translateKey As String = strKey
                If Not strKey.StartsWith("RES_") Then translateKey = $"RES_{strKey}"

                Dim rm As New ResourceManager("VTResources." & strFileName, Assembly.GetExecutingAssembly())
                ret = rm.GetString(translateKey, New CultureInfo("es-ES"))
            Else
                ret = "NotFound"
            End If
        Catch ex As MissingManifestResourceException
            Try
                Dim rm As New ResourceManager("VTResources." & strFileName, Assembly.GetExecutingAssembly())
                ret = rm.GetString(strKey, New CultureInfo("es-ES"))
            Catch ex2 As Exception
                WriteMissingLanguageTag(strFileName, strKey, "es-ES", sDefaultText)
                ret = "NotFound"
            End Try
        Catch ex As Exception
            ret = "NotFound"
        End Try



        Return ret
    End Function

    Private Sub WriteMissingLanguageTag(ByVal strFileName As String, ByVal strKey As String, ByVal strCulture As String, ByVal sDefaultText As String)

#If DEBUG Then
        Dim bAlreadyExists As Boolean = False
        If Not strKey.StartsWith("RES_") Then strKey = $"RES_{strKey}"

        Try
            Dim assemblyLocation As String = System.Reflection.Assembly.GetExecutingAssembly().EscapedCodeBase.Replace("file:///", "")
            Dim directory As String = System.IO.Path.GetDirectoryName(assemblyLocation)

            Dim strFile As String
            Dim iSourceIndex As Integer = directory.ToLower.IndexOf("\src\")

            If iSourceIndex >= 0 Then
                strFile = $"{directory.Substring(0, iSourceIndex)}\Src\Resources\language_missing_tags.txt"
            Else
                strFile = "c:\temp\language_missing_tags.txt"
            End If

            Dim bFileExists As Boolean = File.Exists(strFile)
            If bFileExists Then
                Dim fileReader As String() = File.ReadAllLines(strFile)

                For Each sLine In fileReader
                    If sLine.ToLower() = $"Resource file:{strCulture}::{strFileName}::{strKey}::{sDefaultText}".ToLower() Then
                        bAlreadyExists = True
                        Exit For
                    End If
                Next
            End If

            If Not bAlreadyExists Then
                Using sw As New StreamWriter(File.Open(strFile, IIf(bFileExists, FileMode.Append, FileMode.OpenOrCreate)))
                    sw.WriteLine($"Resource file:{strCulture}::{strFileName}::{strKey}::{sDefaultText}")
                End Using
            End If
        Finally
        End Try


        Try

            Dim assemblyLocation As String = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "")
            Dim directory As String = System.IO.Path.GetDirectoryName(assemblyLocation)

            Dim resxFilePath As String = String.Empty
            Dim iSourceIndex As Integer = directory.ToLower.IndexOf("\src\")

            If iSourceIndex >= 0 Then
                resxFilePath = $"{directory.Substring(0, iSourceIndex)}\Src\Resources\ResXFiles\"
            End If

            Dim customLangTag As String = String.Empty
            Select Case strCulture
                Case "es-ES"
                    customLangTag &= "es"
            End Select

            If customLangTag <> String.Empty Then

                resxFilePath &= $"{strFileName}.resx"

                ' Create a ResXResourceReader for the existing file.
                Dim rr As New ResXResourceReader(resxFilePath)
                ' Create a ResXResourceWriter for the new file.
                Dim rw As New ResXResourceWriter(resxFilePath & ".temp")

                ' Iterate through the resources and copy them into the new file.
                Dim bAddNewEntry As Boolean = True
                For Each dict As DictionaryEntry In rr
                    If dict.Key.ToString.ToLower = strKey.ToLower Then bAddNewEntry = False
                    rw.AddResource(dict.Key.ToString(), dict.Value)
                Next

                If strCulture <> "es-ES" Then sDefaultText = String.Empty

                ' Add the new resource.
                If bAddNewEntry Then rw.AddResource(strKey, sDefaultText)

                rr.Close()
                rw.Close()

                ' Delete the original file and rename the new file.
                File.Delete(resxFilePath)
                File.Move(resxFilePath & ".temp", resxFilePath)
            End If

        Catch ex As Exception

        End Try

#End If
    End Sub

End Class