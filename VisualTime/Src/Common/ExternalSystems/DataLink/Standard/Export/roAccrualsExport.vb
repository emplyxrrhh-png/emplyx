Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roAccrualsExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileAccrualsASCII(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal idConceptGroup As Integer, ByVal oExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idExportTemplate As Integer = 0, Optional bIsAuto As Boolean = False, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                arrFile = ExportAccruals(intIDExport, employeeTempTableName, BeginDate, EndDate, idConceptGroup, oExcelProfileName, ProfileExportBody.FileTypeExport.typ_ASCII, DelimiterChar, StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idExportTemplate, bIsAuto, idSchedule)
                If (arrFile Is Nothing OrElse (arrFile IsNot Nothing AndAlso arrFile.Length = 0)) AndAlso Me.State.Result = DataLinkResultEnum.NoError Then arrFile = System.Text.Encoding.Unicode.GetBytes(" ")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportAccrualsASCII")
            End Try

            Return arrFile
        End Function

        Public Function ExportProfileAccrualsEXCEL(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal idConceptGroup As Integer, ByVal oExcelProfileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idExportTemplate As Integer = 0, Optional bIsAuto As Boolean = False, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportAccruals(intIDExport, employeeTempTableName, BeginDate, EndDate, idConceptGroup, oExcelProfileName, ExcelType, "", StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idExportTemplate, bIsAuto, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportAccrualsEXCEL")
            End Try

            Return arrFile
        End Function

        Private Function ExportAccruals(ByVal intIDExport As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal idConceptGroup As Integer, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idExportTemplate As Integer = 0, Optional bIsAuto As Boolean = False, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim sPostProcessExcelMacro As String = ""
            Dim sPostProcessExcelMacroParameters As String = ""
            Dim bContinue As Boolean = False

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportAccruals.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                ' Diferenciamos si estamos en Cloud
                Dim strExcelFileName As String = String.Empty
                Dim ExcelProfile As ExcelExport = Nothing
                If oExcelProfileBytes IsNot Nothing Then
                    strExcelFileName = oExcelFileName
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile.FileIsOK Then
                        ' Exporta saldos
                        arrFile = AccrualsWriteIE(intIDExport, idExportTemplate, employeeTempTableName, BeginDate, EndDate, idConceptGroup, strExcelFileName, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, sPostProcessExcelMacro, msgLog,
                                            Field1, Field2, Field3, Field4, Field5, Field6, sPostProcessExcelMacroParameters, oExcelProfileBytes, bIsAuto)
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportAccruals.LogEvent.Finish", "") & vbNewLine

                ' Graba el resultado
                roDataLinkExport.SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportAccruals")
            End Try

            Return arrFile
        End Function

        Private Function AccrualsWriteIE(ByVal intIDExport As Integer, ByVal intIDTemplate As Integer, ByVal employeeTempTableName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal idConceptGroup As Integer, ByVal strTemplateFileName As String, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByRef sPostProcessExcelMacro As String, ByRef msgLog As String,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional ByRef sPostProcessExcelMacroParameters As String = "", Optional oExcelProfileBytes As Byte() = Nothing, Optional bIsAuto As Boolean = False) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim OutputFileName As String = ""

            Try

                Dim Ext As String = "xlsx"

                ' Crea el fichero temporal
                OutputFileName = "accruals#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & If(OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII, ".txt", ".xlsx")

                ' Determina la hoja correspondiente a Saldos
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                Else
                    Dim myAccrualsProfile As ProfileExportConcepts

                    myAccrualsProfile = New ProfileExportConcepts(employeeTempTableName, strTemplateFileName, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)

                    ' Lee los campos de la plantilla
                    Dim Field As String = ""
                    Dim Format As String = ""
                    Dim Lenght As Integer = 0
                    Dim IntegerLenght As Integer = 0
                    Dim DecimalLenght As Integer = 0
                    Dim Type As ProfileExportFields.Type
                    Dim Padding As Integer = 0
                    Dim Head As String = ""
                    Dim Row As Integer = 0

                    Dim OutputFields As New List(Of ProfileExportFields)

                    ' Cargamos parámetros avanzados si los hay. TODO: Pasar el resto de parámetros avanzados a esta estructura
                    myAccrualsProfile.Profile.AdvancedParameters = GetProfileAdvancedParameters(ExcelProfile, idSheet)

                    ' Determina si debe filtrar los saldos
                    If Not bIsAuto Then
                        ' En manual ese parámetro viene de la pantalla.
                        myAccrualsProfile.AccrualsFilteredBy = idConceptGroup
                    Else
                        ' En automático está en la plantilla
                        Field = ExcelProfile.GetCellValue(7, 11, idSheet)
                        If Field.ToUpper = "GRUPO_SALDOS" Then
                            myAccrualsProfile.AccrualsFilteredBy = ExecuteScalar("@SELECT# ID from sysroReportGroups where ltrim(rtrim(upper(Name))) = '" & roTypes.Any2String(ExcelProfile.GetCellValue(7, 12, idSheet)).Trim.ToUpper & "'")
                        End If
                    End If

                    ' Recuerpa registro de cabecera
                    Field = ExcelProfile.GetCellValue(8, 11, idSheet)
                    If Field.ToUpper = "HEADER" Then
                        myAccrualsProfile.HeaderLine = roTypes.Any2String(ExcelProfile.GetCellValue(8, 12, idSheet))
                        myAccrualsProfile.SortRowsOffset = 1
                    End If


                    ' Determina si debe roturar
                    Field = ExcelProfile.GetCellValue(1, 11, idSheet)
                    If Field.ToUpper = "ROTURAR_POR_USR" Then
                        myAccrualsProfile.BreakingBy = ExcelProfile.GetCellValue(1, 12, idSheet)
                    End If

                    ' Determina si se deben exportar valores con 0
                    Field = ExcelProfile.GetCellValue(2, 11, idSheet)
                    If Field.ToUpper.Trim = "EXPORT_ZERO_VALUES" Then
                        Dim strValue As String = roTypes.Any2String(ExcelProfile.GetCellValue(2, 12, idSheet)).Trim
                        If strValue <> String.Empty Then myAccrualsProfile.ExportZeroValues = IIf(strValue = "1", True, False)
                    End If

                    myAccrualsProfile.EspGuideName = roTypes.Any2String(ExcelProfile.GetCellValue(1, 10, idSheet)).Trim

                    ' Vemos si se debe incluir detalle de empleados, o se mostrará a nivel de departamento
                    Field = ExcelProfile.GetCellValue(3, 11, idSheet)
                    If Field.ToUpper = "SOLO_DEPARTAMENTOS" Then
                        Dim strValue As String = roTypes.Any2String(ExcelProfile.GetCellValue(3, 12, idSheet)).Trim
                        Try
                            myAccrualsProfile.OnlyDepartamentsOnLevel = Integer.Parse(strValue)
                        Catch ex As Exception
                            myAccrualsProfile.OnlyDepartamentsOnLevel = -1
                        End Try
                    End If

                    ' Recuperamos noombre de script rbs de postproceso para Excel
                    sPostProcessExcelMacro = ExcelProfile.GetCellValue(4, 13, idSheet)

                    ' Periodo de exportación para exportaciones automáticas
                    myAccrualsProfile.PeriodPattern = ExcelProfile.GetCellValue(6, 13, idSheet)

                    ' Vemos si debemos limitar los empleados seleccionados a aquellos que finalizaron contrato ayer
                    myAccrualsProfile.ContractTerminations = myAccrualsProfile.Profile.AdvancedParameters.ContainsKey("FINIQUITO")
                    If myAccrualsProfile.ContractTerminations Then
                        Dim iDays As Integer
                        iDays = roTypes.Any2Integer(myAccrualsProfile.Profile.AdvancedParameters("FINIQUITO"))
                        myAccrualsProfile.ContractTerminationCheckDaysInAdvance = If(iDays <> 0, iDays, 1)
                    End If

                    ' Vemos si se trata de envío automático de nómina a PNET (se envían sólo datos de saldos de contratos en vigor a fecha de ejecución)
                    myAccrualsProfile.OnlyActiveContractsData = myAccrualsProfile.Profile.AdvancedParameters.ContainsKey("CONTRATOS_EN_VIGOR")

                    ' Determinamos si debemos exportar la información de fichajes de comedor
                    myAccrualsProfile.IncludeDining = myAccrualsProfile.Profile.AdvancedParameters.ContainsKey("INCLUDE_DINING")

                    ' Lee los diferentes campos de la plantilla
                    Row = 5
                    Field = ExcelProfile.GetCellValue(Row, 1, idSheet).Trim()

                    While Field <> ""
                        Format = ExcelProfile.GetCellValue(Row, 2, idSheet).Trim()
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
                        Head = ExcelProfile.GetCellValue(Row, 8, idSheet).Trim()

                        ' Crea el campo
                        Dim NomField As New ProfileExportFields(Field, Format, Lenght, IntegerLenght, DecimalLenght, Type, Padding, Head)

                        ' Lo añade a la lista de campos
                        OutputFields.Add(NomField)

                        If Not myAccrualsProfile.IncludePunches AndAlso Field.ToUpper.StartsWith("FICHAJE_") Then myAccrualsProfile.IncludePunches = True
                        If Not myAccrualsProfile.IncludeSchedule AndAlso Field.ToUpper.StartsWith("HORARIO_") Then myAccrualsProfile.IncludeSchedule = True
                        If Not myAccrualsProfile.IncludeEmployeeAccrualPeriods AndAlso (Field.ToUpper.Contains("_INICIOPERIODO") OrElse Field.ToUpper.Contains("_FINPERIODO")) Then myAccrualsProfile.IncludeEmployeeAccrualPeriods = True

                        ' Ordenación. Por el momento una única columna. Handle With Care !!!!
                        Dim sOrderKey As String
                        sOrderKey = ExcelProfile.GetCellValue(Row, 9, idSheet).Trim()
                        If (sOrderKey = "A" OrElse sOrderKey = "D") AndAlso OutputFields IsNot Nothing Then
                            myAccrualsProfile.SortColumnIndex = OutputFields.Count
                            myAccrualsProfile.SortAscending = (sOrderKey = "A")
                        End If

                        Row += 1
                        Field = ExcelProfile.GetCellValue(Row, 1, idSheet).Trim()
                    End While

                    ' Añade la lista de campos a la plantilla
                    myAccrualsProfile.Profile.Fields = OutputFields

                    'Hasta aquí se ha cargado el 'schema' del report. Ahora vamos a rellenarlo de datos.
                    If myAccrualsProfile.OnlyDepartamentsOnLevel < 0 Then
                        ' Exporta saldos
                        If myAccrualsProfile.ExportProfileIE(intIDExport, strTemplateFileName, bIsAuto) Then
                            ' Lee el fichero temporal para convertirlo en array de bytes
                            If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                                arrFile = myAccrualsProfile.Profile.MemoryStreamWriter.ToArray()
                            Else
                                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                            End If

                        End If
                    Else
                        ' Exporta saldos sólo departamentos
                        If myAccrualsProfile.ExportProfileOnlyDepartments Then
                            ' Lee el fichero temporal para convertirlo en array de bytes
                            If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                                arrFile = myAccrualsProfile.Profile.MemoryStreamWriter.ToArray()
                            Else
                                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::AccrualsWriteIE")
            End Try

            ' Borra el fichero temporal
            Try
                If OutputFileName <> "" Then
                    Azure.RoAzureSupport.DeleteFileFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::AccrualsWriteIE:Exception deleteing temp file")
            End Try

            Return arrFile

        End Function

        Private Function GetProfileAdvancedParameters(ExcelProfile As ExcelExport, idSheet As Integer) As Dictionary(Of String, String)
            Dim Field As String = String.Empty
            Dim FieldValue As String = String.Empty
            Dim Row As Integer = 1
            Dim oRet As New Dictionary(Of String, String)(StringComparer.InvariantCultureIgnoreCase)

            Try
                Field = ExcelProfile.GetCellValue(Row, 11, idSheet).Trim()
                While Row < 20
                    If Field <> "" Then
                        FieldValue = roTypes.Any2String(ExcelProfile.GetCellValue(Row, 12, idSheet).Trim()).Trim
                        If Not oRet.ContainsKey(Field) Then oRet.Add(Field, FieldValue)
                    End If
                    Row += 1
                    Field = ExcelProfile.GetCellValue(Row, 11, idSheet).Trim()
                End While
            Catch ex As Exception
            End Try

            Return oRet
        End Function
    End Class
End Namespace
