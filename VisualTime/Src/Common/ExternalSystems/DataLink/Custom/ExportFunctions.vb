Imports SpreadsheetLight

Namespace DataLink
    Public Class ExportFunctions

        Public Shared Function ObtenCampoDeLaFicha(ByVal strCampo As String, ByVal dtEmployeeFields As DataTable) As String
            If dtEmployeeFields IsNot Nothing Then
                Dim dRows As DataRow() = dtEmployeeFields.Select("FieldName = '" & strCampo & "'")
                If dRows.Length > 0 Then Return dRows(0)("value") Else Return ""
            Else
                Return ""
            End If
        End Function

        Public Shared Function ObtenUltimoContrato(ByVal dtContracts As DataTable) As DataRow
            If dtContracts IsNot Nothing Then
                Dim dRows As DataRow() = dtContracts.Select("BeginDate <= #" & Date.Now.ToString("yyyy/MM/dd") & "# and EndDate >= #" & Date.Now.ToString("yyyy/MM/dd") & "#")
                If dRows.Length > 0 Then Return dRows(0) Else Return dtContracts.Rows(0)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function ObtenUltimaMobilidad(ByVal dtMobilities As DataTable) As DataRow
            If dtMobilities IsNot Nothing Then
                Dim dRows As DataRow() = dtMobilities.Select("BeginDate <= #" & Date.Now.ToString("yyyy/MM/dd") & "# and EndDate >= #" & Date.Now.ToString("yyyy/MM/dd") & "#")
                If dRows.Length > 0 Then Return dRows(0) Else Return dtMobilities.Rows(0)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function VISUALTIME_PUNCHES_Line(ByRef ExportedFieldsObj As Object, dtRegistrosAExportar As DataTable, drEmployee As DataRow, drAccrual As DataRow, dtConcepts As DataTable, BeginDate As Date, EndDate As Date, Obj As Object) As String
            Return VisualTime.VisualTimeSystem.VISUALTIME_PUNCHES_Line(ExportedFieldsObj, dtRegistrosAExportar, drEmployee, drAccrual, dtConcepts, BeginDate, EndDate, Obj)
        End Function

#Region "FIAT"
        Public Shared Function FIAT_InfoTipo(ByVal drEmployee As DataRow, ByVal RegistrosAExportar As DataTable, ByVal BeginDateBreak As Date, ByVal EndDateBreak As Date, ByVal BeginDatePer As Date, ByVal EndDatePer As Date, ByVal strExcelFileName As String) As String
            Return Fiat.FiatSystem.FIAT_InfoTipo(drEmployee, RegistrosAExportar, BeginDateBreak, EndDateBreak, BeginDatePer, EndDatePer, strExcelFileName)
        End Function

        Public Shared Function FIAT_InfoTipo_Line(ByRef ExportedFieldsObj As Object, ByVal dtRegistrosAExportar As DataTable, ByVal drEmployee As DataRow, ByVal drAccrual As DataRow, ByVal dtConcepts As DataTable, ByVal BeginDate As Date, ByVal EndDate As Date) As String
            Return Fiat.FiatSystem.FIAT_InfoTipo_Line(ExportedFieldsObj, dtRegistrosAExportar, drEmployee, drAccrual, dtConcepts, BeginDate, EndDate)
        End Function

        Public Shared Function FIAT_CalculateISSEAccrualValue(ByVal drEmployee As DataRow, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Nivel0 As String) As String
            Return Fiat.FiatSystem.FIAT_CalculateISSEAccrualValue(drEmployee, BeginDate, EndDate, Nivel0)
        End Function

        Public Shared Function FIAT_PlantCode(ByVal Nivel0 As String) As String
            Return Fiat.FiatSystem.FIAT_PlantCode(Nivel0)
        End Function

        Public Shared Function FIAT_SIMENU_Line(ByRef ExportedFieldsObj As Object, dtRegistrosAExportar As DataTable, drEmployee As DataRow, drAccrual As DataRow, dtConcepts As DataTable, BeginDate As Date, EndDate As Date, Obj As Object) As String
            Return Fiat.FiatSystem.FIAT_SIMENU_Line(ExportedFieldsObj, dtRegistrosAExportar, drEmployee, drAccrual, dtConcepts, BeginDate, EndDate)
        End Function
        Public Shared Function FIAT_ABSENTISMO_Line(ByRef ExportedFieldsObj As Object, dtRegistrosAExportar As DataTable, drEmployee As DataRow, drAccrual As DataRow, dtConcepts As DataTable, BeginDate As Date, EndDate As Date, Obj As Object) As String
            Return Fiat.FiatSystem.FIAT_ABSENTISMO_Line(ExportedFieldsObj, dtRegistrosAExportar, drEmployee, drAccrual, dtConcepts, BeginDate, EndDate)
        End Function
#End Region
#Region "LEAR"
        Public Shared Function LEAR_Pagos(ByVal drEmployee As DataRow, ByVal RegistrosAExportar As DataTable, ByVal BeginDateBreak As Date, ByVal EndDateBreak As Date, ByVal BeginDatePer As Date, ByVal EndDatePer As Date, ByVal strExcelFileName As String) As String
            Return Lear.LearSystem.LEAR_Pagos(drEmployee, RegistrosAExportar, BeginDateBreak, EndDateBreak, BeginDatePer, EndDatePer, strExcelFileName)
        End Function
#End Region

    End Class
End Namespace

Namespace ExcelHelper
    Public Class ExcelHelper
        Public Shared Function GetValue(ifila As Integer, icolumna As Integer, ByRef oExcel As SpreadsheetLight.SLDocument) As String
            Dim slStyle As New SpreadsheetLight.SLStyle
            slStyle = oExcel.GetCellStyle(ifila, icolumna)
            Select Case oExcel.GetCells(ifila)(icolumna).DataType
                Case DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString
                    Return oExcel.GetCellValueAsString(ifila, icolumna).TrimEnd
                Case DocumentFormat.OpenXml.Spreadsheet.CellValues.Number
                    If oExcel.GetCells(ifila)(icolumna).CellText IsNot Nothing AndAlso oExcel.GetCells(ifila)(icolumna).CellText = "" Then
                        Return oExcel.GetCellValueAsString(ifila, icolumna).TrimEnd
                    Else
                        Select Case GetDataType(slStyle.FormatCode)
                            Case "Date"
                                Return oExcel.GetCellValueAsDateTime(ifila, icolumna).ToString("dd/MM/yyyy")
                            Case "Time"
                                Return oExcel.GetCellValueAsDateTime(ifila, icolumna).ToString("HH:mm")
                            Case "Numeric"
                                Return oExcel.GetCellValueAsDouble(ifila, icolumna).ToString
                            Case Else
                                Return oExcel.GetCellValueAsDouble(ifila, icolumna).ToString
                        End Select
                    End If
                Case DocumentFormat.OpenXml.Spreadsheet.CellValues.Date
                    Return oExcel.GetCellValueAsDateTime(ifila, icolumna).ToString("dd/MM/yyyy")
                Case DocumentFormat.OpenXml.Spreadsheet.CellValues.String
                    Return oExcel.GetSharedStringItems(oExcel.GetCells(ifila)(icolumna).NumericValue).InnerText
                Case Else
                    Return oExcel.GetSharedStringItems(oExcel.GetCells(ifila)(icolumna).NumericValue).InnerText
            End Select
        End Function

        Public Shared Function GetDataType(formatCode As String) As String
            If (formatCode.Contains("[$-409]") OrElse formatCode.Contains("[$-F800]") OrElse formatCode.Contains("m/d") OrElse formatCode.Contains("M/d") OrElse formatCode.Contains("d/m") OrElse formatCode.Contains("d/M") OrElse (formatCode.Contains("mm") AndAlso formatCode.Contains("dd")) OrElse (formatCode.Contains("MM") AndAlso formatCode.Contains("dd"))) Then
                Return "Date"
            ElseIf (formatCode.Contains("[$-F400]") OrElse formatCode.Contains("h:mm") OrElse formatCode.Contains("mm:ss")) Then
                Return "Time"
            ElseIf (formatCode.Contains("#,##0.0")) Then
                Return "Currency"
            ElseIf (formatCode.Last() = "%") Then
                Return "Percentage"
            ElseIf (formatCode.IndexOf("0") = 0) Then
                Return "Numeric"
            Else
                Return "String"
            End If
        End Function
    End Class
End Namespace

