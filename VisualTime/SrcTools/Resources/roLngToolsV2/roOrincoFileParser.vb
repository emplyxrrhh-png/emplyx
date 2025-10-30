Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class roOrincoFileParser

#Region "Prepare files for orinoco(OBSOLETE)"
    Public Shared Sub createOrinocoFileFormat(currentPath As String)
        Dim jsonPathFiles As String = currentPath & "Resources\JsonFiles\"
        Dim jsonDestPathFiles As String = currentPath & "Resources\JsonFiles\i18n\"

        createSingleObjectFile($"{jsonPathFiles}Devextreme\dx.all.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})
        createSingleObjectFile($"{jsonPathFiles}Flexmonster\genius.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})
        createSingleObjectFile($"{jsonPathFiles}SurveyJS\surveyanalyticsjs.lang.json", {"es"}, jsonDestPathFiles, {"es-ES"})
        createSingleObjectFile($"{jsonPathFiles}SurveyJS\surveyjs.lang.json", {"es"}, jsonDestPathFiles, {"es-ES"})
        createSingleObjectFile($"{jsonPathFiles}VTLive\dx.errors.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})
        createSingleObjectFile($"{jsonPathFiles}VTLive\dx.vtlive.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})
        createSingleObjectFile($"{jsonPathFiles}VTPortal\vtportal.i18n.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})
        createSingleObjectFile($"{jsonPathFiles}Visits\visits.lang.json", {"ca", "en", "es", "eu", "fr", "gl", "it", "pt"}, jsonDestPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT"})

    End Sub

    Private Shared Sub createSingleObjectFile(sourcePath As String, sourceLanguages() As String, destPath As String, destPathLang() As String)

        Dim cIndex As Integer = 0
        Dim jsonParser As New roJsonParser()

        For Each strLang As String In sourceLanguages

            Dim cFilePath As String = sourcePath.Replace(".lang.", $".{strLang}.")

            If File.Exists(cFilePath) Then
                Dim cFile As FileInfo = New FileInfo(cFilePath)
                Dim sDestJsonFile As String = destPath & destPathLang(cIndex) & "\" & cFile.Name.Replace($".{strLang}.", ".lang.")

                Dim encoding As Encoding = New UTF8Encoding(False)
                Dim destJson As String = jsonParser.FlattenJsonFile(cFilePath)

                IO.File.WriteAllText(sDestJsonFile, destJson, encoding)
            End If

            cIndex += 1
        Next

    End Sub
#End Region

#Region "Export files to VTI"

    Public Shared Sub ExportFilesToVT(executionPath As String)
        Dim jsonPathFiles As String = executionPath & "Resources\JsonFiles\i18n\"
        Dim resxPathFiles As String = executionPath & "Resources\ResXFiles\"

        Console.WriteLine($" ... Ordenando los ficheros JSON origen")
        orderSourceFiles(jsonPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT", "sk-SK"})
        Console.WriteLine($" ... Copiando los ficheros JSON a sus respectivos proyectos")
        moveToSolutionFiles(jsonPathFiles, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT", "sk-SK"}, {"ca", "en", "es", "eu", "fr", "gl", "it", "pt", "sk"}, executionPath)
        Console.WriteLine($" ... Copiando los ficheros RESX a sus respectivos proyectos")
        moveRexToSolutionFiles(resxPathFiles, {"ca", "en", "es", "eu", "fr", "gl", "it", "pt", "sk"}, {"ca-ES", "en-US", "es-ES", "eu-ES", "fr-FR", "gl-ES", "it-IT", "pt-PT", "sk-SK"}, executionPath)
    End Sub

    Private Shared Sub orderSourceFiles(jsonPathFiles As String, availableEncodings() As String)

        For Each strLang As String In availableEncodings
            Dim cPath As String = jsonPathFiles & strLang

            Dim cFiles As String() = Directory.GetFiles(cPath, "*.json")

            For Each cFile As String In cFiles
                Dim encoding As Encoding = New UTF8Encoding(False) ' roHelper.GetFileEncoding(cFile)
                Dim json As String = File.ReadAllText(cFile)
                Dim jObject As JObject = JObject.Parse(json)

                Dim sortedJObject As JObject = New JObject(jObject.Properties().OrderBy(Function(prop) prop.Name))

                ' Convertir el JObject ordenado en una cadena JSON
                Dim sortedJson As String = sortedJObject.ToString()

                ' Escribir la cadena JSON ordenada de nuevo en el archivo
                File.WriteAllText(cFile, sortedJson, encoding)

            Next
        Next

    End Sub

    Private Shared Sub moveToSolutionFiles(jsonPathFiles As String, availableEncodings() As String, destinationLngTags() As String, executionPath As String)

        Dim cIndex As Integer = 0
        For Each strLang As String In availableEncodings
            Dim cPath As String = jsonPathFiles & strLang


            parseFile($"{cPath}\dx.all.lang.json", destinationLngTags(cIndex),
                        {$"{executionPath}VTLive40\Base\globalize\localization\",
                        $"{executionPath}VTPortalWeb\2\js\localization\",
                        $"{executionPath}LiveVisits\js\localization\"})

            parseFile($"{cPath}\dx.errors.lang.json", destinationLngTags(cIndex),
                        {$"{executionPath}VTLive40\Base\globalize\localization\"})

            parseFile($"{cPath}\dx.vtlive.lang.json", destinationLngTags(cIndex),
                        {$"{executionPath}VTLive40\Base\globalize\localization\"})


            parseFile($"{cPath}\genius.lang.json", destinationLngTags(cIndex), {$"{executionPath}VTLive40\Base\flexmonster\locale\"}, "genius.", "")

            parseFile($"{cPath}\surveyanalyticsjs.lang.json", destinationLngTags(cIndex), {$"{executionPath}VTLive40\Base\surveyjs\locale\"})
            parseFile($"{cPath}\surveyjs.lang.json", destinationLngTags(cIndex), {$"{executionPath}VTLive40\Base\surveyjs\locale\"})


            parseFile($"{cPath}\vtportal.i18n.lang.json", destinationLngTags(cIndex), {$"{executionPath}VTPortalWeb\2\js\localization\"})

            parseFile($"{cPath}\visits.lang.json", destinationLngTags(cIndex), {$"{executionPath}LiveVisits\locales\"}, "visits", "translation")


            cIndex += 1
        Next

    End Sub

    Private Shared Sub moveRexToSolutionFiles(resXFilesPath As String, availableEncodings() As String, destinationLngTags() As String, executionPath As String)

        Dim cIndex As Integer = 0
        Dim sourceFolder As New DirectoryInfo(resXFilesPath)
        Dim resxVTPathFiles As String = executionPath & "Resources\VTResources"

        For Each strLang As String In availableEncodings
            Dim cultureFiles As FileInfo() = sourceFolder.GetFiles("*." & strLang & ".*")
            Dim newLngTag As String = destinationLngTags(cIndex)
            Dim extendedKey As String = GetKeyForTag(strLang)

            If strLang = "es" Then
                cultureFiles = sourceFolder.GetFiles("*.resx")

                cultureFiles = cultureFiles.ToList.FindAll(Function(x) x.Name.Count(Function(c) c = ".") = 1).ToArray

            End If

            If newLngTag = String.Empty OrElse extendedKey = String.Empty Then Exit For

            For Each cFile As FileInfo In cultureFiles

                Dim sContent As String = IO.File.ReadAllText(cFile.FullName)
                Dim encoding As Encoding = roHelper.GetFileEncoding(cFile.FullName)

                Dim destinationFileName As String = cFile.Name.Replace($".{strLang}.", $".{newLngTag}.")

                If strLang = "es" Then
                    destinationFileName = cFile.Name.Replace(".resx", $".{newLngTag}.resx")
                End If

                IO.File.WriteAllText(resxVTPathFiles & $"\{extendedKey}\" & destinationFileName, sContent, encoding)
            Next
            cIndex += 1
        Next

    End Sub

    Private Shared Function GetKeyForTag(strLang As String) As String
        Select Case strLang
            Case "ca"
                Return "CAT"
            Case "en"
                Return "ENG"
            Case "es"
                Return "ESP"
            Case "eu"
                Return "EKR"
            Case "fr"
                Return "FRA"
            Case "gl"
                Return "GAL"
            Case "it"
                Return "ITA"
            Case "pt"
                Return "POR"
            Case "sk"
                Return "SLK"
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Shared Sub parseFile(sourceFile As String, destinationLngTag As String, destinationFolders As String(), Optional ByVal sourcePatternMatch As String = "", Optional ByVal destinationPatternMatch As String = "")

        Dim jsonParser As New roJsonParser()
        If IO.File.Exists(sourceFile) Then

            Dim encoding As Encoding = roHelper.GetFileEncoding(sourceFile)
            Dim jsonContent As String = jsonParser.UnflattenJsonFile(sourceFile)

            Dim cFile As FileInfo = New FileInfo(sourceFile)

            Dim newFileName As String = cFile.Name.Replace(".lang.", $".{destinationLngTag}.")
            If sourcePatternMatch <> String.Empty Then
                newFileName = newFileName.Replace(sourcePatternMatch, destinationPatternMatch)
            End If

            jsonContent = jsonContent.Replace("""lng"": {", $"""{destinationLngTag}"": {"{"}")

            For Each destinationFolder As String In destinationFolders
                IO.File.WriteAllText(destinationFolder & newFileName, jsonContent, encoding)
                'Console.WriteLine($" ... ... File'{newFileName}' copied to '{destinationFolder & newFileName}'")
            Next
        Else
            Console.WriteLine($" ... ... No se ha encontrado el fichero: {sourceFile}")
        End If
    End Sub

#End Region

End Class
