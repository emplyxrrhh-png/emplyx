Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Namespace DataLink

    Public Class roTaskExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileTasksASCII(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing,
                                                Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                arrFile = ExportTasks(intIDExport, tmpEmployeeFilterTable, BeginDate, EndDate, oExcelProfileName, ProfileExportBody.FileTypeExport.typ_ASCII, DelimiterChar, StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
                If (arrFile Is Nothing OrElse arrFile.Length = 0) AndAlso Me.State.Result = DataLinkResultEnum.NoError Then arrFile = System.Text.Encoding.Unicode.GetBytes(" ")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileTasksASCII")

            End Try

            Return arrFile
        End Function

        Public Function ExportProfileTasksEXCEL(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelproFileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing,
                                                Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportTasks(intIDExport, tmpEmployeeFilterTable, BeginDate, EndDate, oExcelproFileName, ExcelType, "", StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportTasksEXCEL")
            End Try

            Return arrFile
        End Function

        Private Function ExportTasks(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport,
                                     ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing,
                                     Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim bContinue As Boolean = False

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportTasks.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim ExcelProfile As ExcelExport = Nothing

                If oExcelProfileBytes IsNot Nothing Then
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile IsNot Nothing AndAlso ExcelProfile.FileIsOK Then
                        ' Exporta tareas
                        arrFile = ProlTasks(tmpEmployeeFilterTable, BeginDate, EndDate, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, msgLog, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes)
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportTasks.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportTasks")

            End Try

            Return arrFile
        End Function

        Private Function ProlTasks(ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String,
                                   ByVal StartCalculDay As Integer, ByRef msgLog As String, Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing,
                                   Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
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
                OutputFileName = "tasks#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & "." & Ext

                ' Determina la hoja correspondiente a tareas
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")
                ' Determina la hoja correspondiente a flitros de tareas
                Dim idSheetFilter As Integer = ExcelProfile.GetIDSheet("Filtros")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

                Dim myExcelProfile As ProfileExportTasks

                ' Determina el tipo de lanzamiento
                If BeginDate <> #12:00:00 AM# Then
                    ' Crea la plantilla para lanzamiento manual
                    myExcelProfile = New ProfileExportTasks(tmpEmployeeFilterTable, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)
                Else
                    ' Crea la plantilla para lanzamiento automático
                    myExcelProfile = New ProfileExportTasks(tmpEmployeeFilterTable, OutputFileName, OutputFileType, DelimiterChar, StartCalculDay, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)
                End If

                ' Lee los campos de la plantilla
                Dim Field As String = ""
                Dim Format As String = ""
                Dim Lenght As Integer = 0
                Dim IntegerLenght As Integer = 0
                Dim DecimalLenght As Integer = 0
                Dim Type As ProfileExportFields.Type
                Dim Padding As Integer = 0
                Dim Head As String = ""
                Dim Row As Integer = 5

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
                Dim lstFilters As New List(Of String)
                If idSheetFilter <> -1 Then
                    Dim strTaskFilter = String.Empty
                    Dim filterColumn = True
                    Dim rowCount = 1

                    While filterColumn
                        Dim filterValue = String.Empty
                        Dim columnName = ExcelProfile.GetCellValue(1, rowCount, idSheetFilter).ToString()
                        If (Not String.IsNullOrEmpty(columnName)) Then
                            Dim columnValue = String.Empty
                            If (columnName.ToUpper().Contains("FECHA")) Then
                                Dim excelFilter As String = ExcelProfile.GetCellValue(2, rowCount, idSheetFilter).ToString()
                                If (excelFilter.Contains("-")) Then
                                    Dim dateValues() = excelFilter.Split("-")
                                    columnValue = Convert.ToDateTime(dateValues(0).Trim()).ToShortDateString() & " - " & Convert.ToDateTime(dateValues(1).Trim()).ToShortDateString()
                                Else
                                    columnValue = Convert.ToDateTime(ExcelProfile.GetCellValue(2, rowCount, idSheetFilter, True)).ToShortDateString()
                                End If
                            Else
                                columnValue = ExcelProfile.GetCellValue(2, rowCount, idSheetFilter).ToString()
                            End If

                            If (Not String.IsNullOrEmpty(columnValue)) Then
                                filterValue = myExcelProfile.SetDataFilters(columnName, columnValue)
                                lstFilters.Add(filterValue)
                            End If
                        Else
                            filterColumn = False
                        End If
                        rowCount += 1
                    End While
                End If
                ' Añade la lista de campos a la plantilla
                myExcelProfile.Profile.Fields = OutputFields
                myExcelProfile.LstFilters = lstFilters
                ' Exporta las tareas
                If myExcelProfile.ExportProfile(msgLog, Me.State) = True Then
                    ' Lee el fichero temporal para convertirlo en array de bytes
                    If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                        arrFile = myExcelProfile.Profile.MemoryStreamWriter.ToArray()
                    Else
                        arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ProlTasks")

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
