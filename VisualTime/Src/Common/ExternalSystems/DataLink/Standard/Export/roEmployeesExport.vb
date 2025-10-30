Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink



    Public Class roEmployeesExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileEmployeeASCII(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                arrFile = ExportEmployees(intIDExport, employeeTempTableName, BeginDate, EndDate, oExcelProfileName, ProfileExportBody.FileTypeExport.typ_ASCII, DelimiterChar, StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
                If (arrFile Is Nothing OrElse (arrFile IsNot Nothing AndAlso arrFile.Length = 0)) AndAlso Me.State.Result = DataLinkResultEnum.NoError Then arrFile = System.Text.Encoding.Unicode.GetBytes(" ")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileEmployeeASCII")

            End Try

            Return arrFile
        End Function

        Public Function ExportProfileEmployeeEXCEL(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelproFileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportEmployees(intIDExport, employeeTempTableName, BeginDate, EndDate, oExcelproFileName, ExcelType, "", StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileEmployeeEXCEL")

            End Try

            Return arrFile
        End Function

        Private Function ExportEmployees(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim bContinue As Boolean = False

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportEmployees.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim ExcelProfile As ExcelExport = Nothing
                If Not oExcelProfileBytes Is Nothing Then
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile IsNot Nothing AndAlso ExcelProfile.FileIsOK Then
                        ' Exporta tareas
                        arrFile = ProlEmployees(employeeTempTableName, BeginDate, EndDate, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, msgLog, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes)
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportEmployees.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportEmployees")

            End Try

            Return arrFile
        End Function

        Private Function ProlEmployees(ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByRef msgLog As String,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
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
                OutputFileName = "employees#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                ' Determina la hoja correspondiente a tareas
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

                Dim myExcelProfile As ProfileExportEmployees

                ' Determina el tipo de lanzamiento
                If BeginDate <> #12:00:00 AM# Then
                    ' Crea la plantilla para lanzamiento manual
                    myExcelProfile = New ProfileExportEmployees(employeeTempTableName, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)
                Else
                    ' Crea la plantilla para lanzamiento automático
                    myExcelProfile = New ProfileExportEmployees(employeeTempTableName, OutputFileName, OutputFileType, DelimiterChar, StartCalculDay, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)
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

                    ' Si el último esta USR*, debo añadir tantas columnas como campos hay en la ficha de empleado
                    Dim sNextValue As String = ExcelProfile.GetCellValue(Row, 1, idSheet)
                    If sNextValue.ToUpper = "USR*" Then
                        Dim strSQL As String = "@SELECT# FieldName, FieldType, History FROM sysroUserFields WHERE Type = 0 AND Used = 1 ORDER BY FieldName ASC"
                        Dim dtUsrFields As New DataTable
                        dtUsrFields = CreateDataTable(strSQL)
                        If dtUsrFields.Rows.Count > 0 Then
                            For Each usrRow As DataRow In dtUsrFields.Rows
                                NomField = New ProfileExportFields("USR_" & usrRow("FieldName"), Format, Lenght, IntegerLenght, DecimalLenght, Type, Padding, "USR_" & usrRow("FieldName"))
                                OutputFields.Add(NomField)
                                ' Miro si tiene histórico
                                If roTypes.Any2Boolean(usrRow("History")) Then
                                    NomField = New ProfileExportFields("DAT_" & usrRow("FieldName"), Format, Lenght, IntegerLenght, DecimalLenght, Type, Padding, "USR_" & usrRow("FieldName") & "_#FECHAINICIO#")
                                    OutputFields.Add(NomField)
                                End If
                            Next
                        End If
                    Else
                        OutputFields.Add(NomField)
                    End If

                    Row += 1
                    Field = ExcelProfile.GetCellValue(Row, 1, idSheet)
                End While

                ' Añade la lista de campos a la plantilla
                myExcelProfile.Profile.Fields = OutputFields

                ' Informo roturas configuradas
                myExcelProfile.BreakByContracts = False
                myExcelProfile.BreakByMobilities = False
                myExcelProfile.BreakByUsrFieldsValues = False
                Field = ExcelProfile.GetCellValue(1, 11, idSheet)
                If Field.ToUpper = "ROTURAS" Then
                    myExcelProfile.BreakByContracts = ExcelProfile.GetCellValue(1, 12, idSheet).ToString.ToUpper.Contains("C")
                    myExcelProfile.BreakByMobilities = ExcelProfile.GetCellValue(1, 12, idSheet).ToString.ToUpper.Contains("M")
                    myExcelProfile.BreakByUsrFieldsValues = ExcelProfile.GetCellValue(1, 12, idSheet).ToString.ToUpper.Contains("F")
                End If

                ' Exporta las tareas
                If myExcelProfile.ExportProfile(msgLog, Me.State) Then
                    ' Lee el fichero temporal para convertirlo en array de bytes
                    If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                        arrFile = myExcelProfile.Profile.MemoryStreamWriter.ToArray()
                    Else
                        arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ProlEmployees")

            End Try

            ' Borra el fichero temporal
            Try
                If OutputFileName <> "" Then
                    Azure.RoAzureSupport.DeleteFileFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                End If
            Catch ex As Exception

            End Try

            Return arrFile

        End Function

    End Class
End Namespace