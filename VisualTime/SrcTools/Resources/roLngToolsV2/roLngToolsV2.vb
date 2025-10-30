
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module roLngToolsV2

    Sub Main()
        Try
            If My.Application.CommandLineArgs.Count > 0 Then

                Dim currentPath As String = IO.Directory.GetParent(IO.Directory.GetCurrentDirectory()).Parent.FullName & "\Src\"
                Dim oWorkMode As String = My.Application.CommandLineArgs(0)

                Select Case oWorkMode
                    Case "JSON"
                        Console.WriteLine("Iniciando copia de ficheros JSON a sus carpetas destino")
                        roOrincoFileParser.ExportFilesToVT(currentPath)
                    Case "API"
                        Console.WriteLine("Conectando a api orinoco para subir los ficheros de recursos en formato ZIP")
                    Case "ORINOCO"
                        Console.WriteLine("Iniciando proceso de preparación de ficheros para integración orinoco")
                        roOrincoFileParser.createOrinocoFileFormat(currentPath)
                    Case "GOOGLE"
                        If My.Application.CommandLineArgs.Count > 1 Then
                            Console.WriteLine($"Iniciando proceso de traducción de ficheros JSON a {My.Application.CommandLineArgs(1)}")
                            roGoogleParser.translateAllJSONUsingGoogleTo(currentPath, My.Application.CommandLineArgs(1))
                        Else
                            Console.WriteLine($"Parametros incorrectos para traducir un idioma mediante google (GOOGLE CAT/ENG/GAL/EKR/ITA/FRA/POR)")
                        End If

                    Case Else
                        Console.WriteLine("Modos disponibles. JSON / API")
                End Select

            Else
                If My.Application.CommandLineArgs.Count = 0 Then
                    Console.WriteLine("Debes indicar el modo de trabajo. Modos disponibles. JSON / API / ORINOCO / GOOGLE")
                Else
                    Console.WriteLine("Demasiados parámetros. Sólo acepto el id del terminal a programar")
                End If
            End If
        Catch ex As Exception
            Console.WriteLine($"Algo inesperado ha pasado {ex.Message} {vbNewLine} {ex.StackTrace}")
        End Try

    End Sub




End Module
