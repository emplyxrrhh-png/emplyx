Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink
    Public Class roCostCenterExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileCostCentersASCII(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal CausesFilter As String, ByVal CentersFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                arrFile = ExportCostCenters(intIDExport, tmpEmployeeFilterTable, CausesFilter, CentersFilter, BeginDate, EndDate, oExcelProfileName, ProfileExportBody.FileTypeExport.typ_ASCII, DelimiterChar, StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
                If (arrFile Is Nothing OrElse (arrFile IsNot Nothing AndAlso arrFile.Length = 0)) AndAlso Me.State.Result = DataLinkResultEnum.NoError Then arrFile = System.Text.Encoding.Unicode.GetBytes(" ")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileCostCentersASCII")

            End Try

            Return arrFile
        End Function

        Public Function ExportProfileCostCentersEXCEL(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal CausesFilter As String, ByVal CentersFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportCostCenters(intIDExport, tmpEmployeeFilterTable, CausesFilter, CentersFilter, BeginDate, EndDate, oExcelProfileName, ExcelType, "", StartCalculDay, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileCostCentersEXCEL")

            End Try

            Return arrFile
        End Function

        Private Function ExportCostCenters(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal CausesFilter As String, ByVal CentersFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim bContinue As Boolean = False

            Try
                strlogevent = ""

                'oState = Me.state

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportCostCenters.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim ExcelProfile As ExcelExport = Nothing

                If oExcelProfileBytes IsNot Nothing Then
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile IsNot Nothing AndAlso ExcelProfile.FileIsOK Then
                        ' Exporta centros de coste
                        arrFile = CostCentersWrite(intIDExport, tmpEmployeeFilterTable, CausesFilter, CentersFilter, BeginDate, EndDate, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, msgLog,
                                            Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes)
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportCostCentersLogEvent.Finish", "") & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportCostCenters")

            End Try

            Return arrFile
        End Function

        Private Function CostCentersWrite(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal CausesFilter As String, ByVal CentersFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByRef msgLog As String,
                                                          Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
            Dim bolRet As Boolean = False
            Dim arrFile As Byte() = Nothing
            Dim OutputFileName As String = ""

            Try

                Dim Ext As String = ""

                ' Determina la extensión del fichero de salida
                Select Case OutputFileType
                    Case ProfileExportBody.FileTypeExport.typ_2003 : Ext = "xls"
                    Case ProfileExportBody.FileTypeExport.typ_2007 : Ext = "xlsx"
                    Case ProfileExportBody.FileTypeExport.typ_ASCII : Ext = "txt"
                End Select

                ' Crea el fichero temporal
                OutputFileName = "CostCenters#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & "." & Ext

                ' Determina la hoja correspondiente a Saldos
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")
                ' Determina la hoja correspondiente a flitros de centros o justificaciones
                Dim idSheetFilter As Integer = ExcelProfile.GetIDSheet("Filtros")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

                'la plantilla tiene filtros
                If idSheetFilter <> -1 Then
                    'recupero los nombres cortos de las justificaciones por las que se quiere filtrar
                    Dim causesFilterNames = ExcelProfile.GetCellValue(2, 1, idSheetFilter).ToString()
                    If (Not String.IsNullOrEmpty(causesFilterNames)) Then
                        Dim sSql = "@SELECT# id from Causes where ShortName in ('" & causesFilterNames.Replace(",", "','") & "') "
                        Dim dtCauses = CreateDataTable(sSql)
                        If (dtCauses IsNot Nothing AndAlso dtCauses.Rows.Count > 0) Then
                            For Each causeRow As DataRow In dtCauses.Rows
                                CausesFilter += Any2String(causeRow("id")) & ","
                            Next
                            CausesFilter = CausesFilter.Substring(0, CausesFilter.Length - 1)
                        End If
                    End If

                    'recupero los nombres de los centros de coste a filtrar
                    Dim centersFilterNames = ExcelProfile.GetCellValue(2, 2, idSheetFilter).ToString()
                    If (Not String.IsNullOrEmpty(centersFilterNames)) Then
                        Dim sSql = "@SELECT# id from BusinessCenters where Name in ('" & centersFilterNames.Replace(",", "','") & "') "
                        Dim dtCauses = CreateDataTable(sSql)
                        If (dtCauses IsNot Nothing AndAlso dtCauses.Rows.Count > 0) Then
                            For Each causeRow As DataRow In dtCauses.Rows
                                CentersFilter += Any2String(causeRow("id")) & ","
                            Next
                            CentersFilter = CentersFilter.Substring(0, CentersFilter.Length - 1)
                        End If
                    End If
                End If

                Dim myCostCentersProfile As ProfileExportCostCenters
                Dim Field As String = ""

                ' Determinamos si tenemos que redondear el valor de las justificaciones
                Dim bolRoundCauses As Boolean = False
                Field = roTypes.Any2String(ExcelProfile.GetCellValue(3, 11, idSheet)).Trim
                If Field.ToUpper = "REDONDEAR_JUSTIFICACIONES" Then
                    bolRoundCauses = True
                End If

                ' Determina el tipo de lanzamiento
                If BeginDate <> #12:00:00 AM# Then
                    ' Crea la plantilla para lanzamiento manual
                    myCostCentersProfile = New ProfileExportCostCenters(tmpEmployeeFilterTable, CausesFilter, CentersFilter, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6, bolRoundCauses)
                Else
                    ' Crea la plantilla para lanzamiento automático
                    myCostCentersProfile = New ProfileExportCostCenters(tmpEmployeeFilterTable, CausesFilter, CentersFilter, OutputFileName, OutputFileType, DelimiterChar, StartCalculDay, Me.State, Field1, Field2, Field3, Field4, Field5, Field6, bolRoundCauses)
                End If

                ' Lee los campos de la plantilla
                Dim Format As String = ""
                Dim Lenght As Integer = 0
                Dim IntegerLenght As Integer = 0
                Dim DecimalLenght As Integer = 0
                Dim Type As ProfileExportFields.Type
                Dim Padding As Integer = 0
                Dim Head As String = ""
                Dim Row As Integer = 0

                Dim OutputFields As New List(Of ProfileExportFields)

                ' Lee los diferentes campos de la plantilla
                Row = 5
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
                myCostCentersProfile.Profile.Fields = OutputFields

                ' Exporta centros
                If myCostCentersProfile.ExportProfile Then
                    ' Lee el fichero temporal para convertirlo en array de bytes
                    If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                        arrFile = myCostCentersProfile.Profile.MemoryStreamWriter.ToArray()
                    Else
                        arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::CostCentersWrite")

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