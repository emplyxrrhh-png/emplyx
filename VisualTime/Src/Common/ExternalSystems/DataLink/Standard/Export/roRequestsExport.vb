Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Namespace DataLink
    Public Class roRequestsExport
        Inherits roDataLinkExport


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileRequestsEXCEL(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer,
                                                 Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportProfileRequests(intIDExport, EmployeesFilter, BeginDate, EndDate, oExcelProfileName, ExcelType, "", StartCalculDay, IntervalMinutes, Field1, Field2, Field3, Field4, Field5, Field6)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileRequestsEXCEL")

            End Try

            Return arrFile
        End Function

        Private Function ExportProfileRequests(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer,
                                                 Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportDailyCauses.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)
                If ExcelProfile.FileIsOK Then
                    arrFile = RequestsWrite(intIDExport, EmployeesFilter, BeginDate, EndDate, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, IntervalMinutes, msgLog, Field1, Field2, Field3, Field4, Field5, Field6)
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportDailyCauses.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileRequests")

            End Try

            Return arrFile
        End Function

        Private Function RequestsWrite(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer, ByRef msgLog As String,
                                                 Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing) As Byte()
            Dim bolRet As Boolean = False
            Dim arrFile As Byte() = Nothing
            Dim OutputFileName As String = ""

            msgLog = ""

            Try

                Dim Ext As String = ""

                ' Determina la extensión del fichero de salida
                Select Case OutputFileType
                    Case ProfileExportBody.FileTypeExport.typ_2003 : Ext = "xls"
                    Case ProfileExportBody.FileTypeExport.typ_2007 : Ext = "xlsx"
                    Case ProfileExportBody.FileTypeExport.typ_ASCII : Ext = "txt"
                End Select

                ' Crea el fichero temporal
                OutputFileName = CreateTempFile("requests", Ext, Me.State)

                ' Determina la hoja correspondiente a fichajes
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

                Dim myExcelProfile As ProfileExportRequests

                ' Lee los campos de la plantilla
                Dim Field As String = ""
                Dim Format As String = ""
                Dim Lenght As Integer = 0
                Dim IntegerLenght As Integer = 0
                Dim DecimalLenght As Integer = 0
                Dim Type As ProfileExportFields.Type
                Dim Padding As Integer = 0
                Dim Head As String = ""
                Dim Row As Integer = 2

                ' Obtenemos la lista de justificaciones
                Dim strCausesFilter As String = ""
                Dim idSheetCauses As Integer = ExcelProfile.GetIDSheet("Hoja2")
                If idSheetCauses <> -1 Then
                    Row = 2
                    Field = ExcelProfile.GetCellValue(Row, 1, idSheetCauses)

                    While Field <> ""
                        If strCausesFilter.Length = 0 Then
                            strCausesFilter = "'" & Trim(Field) & "'"
                        Else
                            strCausesFilter += ",'" & Trim(Field) & "'"
                        End If
                        Row += 1
                        Field = ExcelProfile.GetCellValue(Row, 1, idSheetCauses)
                    End While
                End If

                Row = 5

                ' Crea la plantilla para lanzamiento manual
                myExcelProfile = New ProfileExportRequests(EmployeesFilter, strCausesFilter, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)

                Dim OutputFields As New List(Of ProfileExportFields)

                Field = ExcelProfile.GetCellValue(Row, 1, idSheet)

                While Field <> ""
                    Format = ExcelProfile.GetCellValue(Row, 2, idSheet)
                    Lenght = roTypes.Any2Integer(ExcelProfile.GetCellValue(Row, 3, idSheet))
                    IntegerLenght = roTypes.Any2Integer(ExcelProfile.GetCellValue(Row, 4, idSheet))
                    DecimalLenght = roTypes.Any2Integer(ExcelProfile.GetCellValue(Row, 5, idSheet))
                    Select Case ExcelProfile.GetCellValue(Row, 6, idSheet).ToString.ToUpper
                        Case "N" : Type = ProfileExportFields.Type.typ_Numeric
                        Case "T" : Type = ProfileExportFields.Type.typ_Text
                        Case "D" : Type = ProfileExportFields.Type.typ_Date
                        Case Else : Type = ProfileExportFields.Type.typ_Text
                    End Select
                    Padding = roTypes.Any2Integer(ExcelProfile.GetCellValue(Row, 7, idSheet))
                    Head = ExcelProfile.GetCellValue(Row, 8, idSheet)

                    ' Crea el campo
                    Dim NomField As New ProfileExportFields(Field, Format, Lenght, IntegerLenght, DecimalLenght, Type, Padding, Head)

                    ' Lo añade a la lista de campos
                    OutputFields.Add(NomField)

                    Row += 1
                    Field = ExcelProfile.GetCellValue(Row, 1, idSheet)
                End While

                ' Añade la lista de campos a la plantilla
                myExcelProfile.Profile.Fields = OutputFields

                ' Exporta solicitudes
                If myExcelProfile.ExportProfile() = True Then
                    ' Lee el fichero temporal para convertirlo en array de bytes
                    arrFile = IO.File.ReadAllBytes(OutputFileName)
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::DailyRequestsWrite")

            End Try

            ' Borra el fichero temporal
            Try
                If OutputFileName <> "" Then IO.File.Delete(OutputFileName)
            Catch ex As Exception

            End Try

            Return arrFile

        End Function

    End Class

End Namespace