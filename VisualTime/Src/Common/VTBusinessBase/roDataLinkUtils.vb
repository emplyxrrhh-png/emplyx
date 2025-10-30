Imports System.IO
Imports Ionic.Zip
Imports Robotics.Azure
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports SpreadsheetLight

#Region "ExcelExport Support Class"

Public Class ExcelExport

    Public Enum ExcelVersion
        exc_2003
        exc_2007
    End Enum

    Private BookSpreadsheetLight As SpreadsheetLight.SLDocument   'Componente para versiones >= 2007
    Private BookExcelLibrary As ExcelLibrary.SpreadSheet.Workbook 'Componente para versiones <= 2003

    Private mbolIs2007 As Boolean
    Private mPathName As String


    Public Sub New(ByVal ExcelType As ExcelVersion, ByVal PathName As String)
        ' Asigna variables iniciales
        mbolIs2007 = (ExcelType = ExcelVersion.exc_2007)
        mPathName = PathName

        ' Crea objeto en función de la versión excel
        If mbolIs2007 Then
            BookSpreadsheetLight = New SpreadsheetLight.SLDocument
        Else
            BookExcelLibrary = New ExcelLibrary.SpreadSheet.Workbook
            Dim wrkSht As New ExcelLibrary.SpreadSheet.Worksheet("Hoja1")
            BookExcelLibrary.Worksheets.Add(wrkSht)
        End If
    End Sub

    Public Sub New(ByVal ExcelType As ExcelVersion, ByVal oBytes As Byte(), ByVal OutputFileName As String)
        ' Asigna variables iniciales
        mbolIs2007 = (ExcelType = ExcelVersion.exc_2007)
        BookSpreadsheetLight = New SpreadsheetLight.SLDocument
        mPathName = OutputFileName
    End Sub

    Public Sub New(ByVal PathName As String)
        ' Asigna variables iniciales
        mPathName = PathName

        ' Comprueba si es 2007
        Try
            BookSpreadsheetLight = New SpreadsheetLight.SLDocument(PathName)
            If BookSpreadsheetLight.GetSheetNames.Count > 0 Then mbolIs2007 = True
        Catch ex As Exception

        End Try

        ' Comprueba si es 97
        Try
            If mbolIs2007 = False Then
                BookExcelLibrary = ExcelLibrary.SpreadSheet.Workbook.Load(PathName)
                If BookExcelLibrary.Worksheets.Count > 0 Then mbolIs2007 = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub New(ByVal oBytes As Byte())
        ' Comprueba si es 2007
        Try
            BookSpreadsheetLight = New SpreadsheetLight.SLDocument(New MemoryStream(oBytes))
            If BookSpreadsheetLight.GetSheetNames.Count > 0 Then mbolIs2007 = True
        Catch ex As Exception

        End Try

        ' Comprueba si es 97
        Try
            If mbolIs2007 = False Then
                BookExcelLibrary = ExcelLibrary.SpreadSheet.Workbook.Load(New MemoryStream(oBytes))
                If BookExcelLibrary.Worksheets.Count > 0 Then mbolIs2007 = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function FileIsOK() As Boolean
        Try
            Return Not (IsNothing(BookSpreadsheetLight) And IsNothing(BookExcelLibrary))
        Catch ex As Exception
            Return False
        End Try
    End Function

#Region "Cell management"

    Public Sub SetCellValue(ByVal row As Integer, ByVal col As Integer, ByVal Value As Object, Optional ByVal Format As String = "", Optional ByVal HorAlig As DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues = -1, Optional ByVal BorderVisible As Boolean = False, Optional ByVal TextRotation As Integer = -1, Optional FontSize As Integer = -1, Optional FontName As String = "", Optional FontBold As Boolean = False, Optional ByVal BorderType As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin, Optional ByVal VerAlig As DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues = -1)
        Try
            If IsDBNull(Value) Then Return

            If mbolIs2007 = True Then

                Dim styleBehaviour As String = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Export.FormatColumns")
                If Format = String.Empty AndAlso (String.IsNullOrEmpty(styleBehaviour) OrElse roTypes.Any2Boolean(styleBehaviour)) Then Format = GetStyleByValueType(Value)

                BookSpreadsheetLight.SetCellValue(row, col, Value)

                ' Formatea la celda si es necesario
                Dim style As New SLStyle
                If Format <> "" Then style.FormatCode = Format
                If HorAlig <> -1 Then style.Alignment.Horizontal = HorAlig
                If VerAlig <> -1 Then style.Alignment.Vertical = VerAlig
                If BorderVisible = True Then
                    style.Border.BottomBorder.BorderStyle = BorderType
                    style.Border.TopBorder.BorderStyle = BorderType
                    style.Border.LeftBorder.BorderStyle = BorderType
                    style.Border.RightBorder.BorderStyle = BorderType
                End If

                If TextRotation <> -1 Then style.Alignment.TextRotation = TextRotation
                If FontName <> "" Then style.Font.FontName = FontName
                If FontSize <> -1 Then style.Font.FontSize = FontSize
                If FontBold Then style.Font.Bold = True

                BookSpreadsheetLight.SetCellStyle(row, col, style)
            Else
                BookExcelLibrary.Worksheets(0).Cells(row - 1, col - 1) = New ExcelLibrary.SpreadSheet.Cell(Value, Format)
            End If
        Catch ex As Exception
            Console.Write(ex.Message)
        End Try
    End Sub

    Public Function GetStyleByValueType(valor As Object) As String
        Dim cellStyle As String = String.Empty

        If valor Is Nothing Then Return cellStyle

        Dim tipo = valor.GetType()

        If tipo = GetType(Integer) OrElse tipo = GetType(Long) Then
            cellStyle = "0"
        ElseIf tipo = GetType(Decimal) OrElse tipo = GetType(Double) OrElse tipo = GetType(Single) Then
            cellStyle = "0.00"
        ElseIf tipo = GetType(DateTime) Then
            cellStyle = "yyyy-mm-dd hh:mm"
        ElseIf tipo = GetType(Boolean) Then
            cellStyle = "General" ' sin formato específico, pero podrías poner "TRUE/FALSE"
        Else
            cellStyle = "@" ' texto
        End If

        Return cellStyle
    End Function


    Public Sub SetBorder(ByVal row As Integer, ByVal col As Integer, ByVal BottomBorder As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues, ByVal TopBorder As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues, ByVal LeftBorder As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues, ByVal RightBorder As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues)
        Try
            If mbolIs2007 = True Then
                ' Define bordes
                Dim style As New SLStyle

                If BottomBorder Then style.Border.BottomBorder.BorderStyle = BottomBorder
                If TopBorder Then style.Border.TopBorder.BorderStyle = TopBorder
                If LeftBorder Then style.Border.LeftBorder.BorderStyle = LeftBorder
                If RightBorder Then style.Border.RightBorder.BorderStyle = RightBorder

                BookSpreadsheetLight.SetCellStyle(row, col, style)
            Else

            End If
        Catch ex As Exception
            Console.Write(ex.Message)
        End Try
    End Sub

    Public Function GetWorksheetStatistics() As SpreadsheetLight.SLWorksheetStatistics
        Return BookSpreadsheetLight.GetWorksheetStatistics
    End Function

    Public Function GetCellValue(ByVal row As Integer, ByVal col As Integer, Optional idSheet As Integer = 0, Optional isDate As Boolean = False) As Object
        Try
            If mbolIs2007 = True Then
                ' Selecciona la hoja
                BookSpreadsheetLight.SelectWorksheet(BookSpreadsheetLight.GetSheetNames.Item(idSheet))

                ' Asigna el valor a la celda
                If (isDate) Then
                    Return BookSpreadsheetLight.GetCellValueAsDateTime(row, col)
                Else
                    Return BookSpreadsheetLight.GetCellValueAsString(row, col)
                End If
            Else
                Dim value As Object = BookExcelLibrary.Worksheets(idSheet).Cells(row - 1, col - 1).Value
                If value = Nothing Then value = ""
                Return value
            End If
        Catch ex As Exception
            Console.Write(ex.Message)

        End Try

        Return ""
    End Function

    Public Sub CreateBox(ByVal IniRow As Integer, ByVal IniCol As Integer, ByVal FinRow As Integer, ByVal FinCol As Integer, ByVal BorderStyle As DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues)
        Try

            If mbolIs2007 = True Then
                Dim r As Integer = 0
                Dim c As Integer = 0

                For r = IniRow To FinRow
                    Dim style As New SLStyle

                    For c = IniCol To FinCol
                        If r = IniRow Then style.Border.TopBorder.BorderStyle = BorderStyle
                        If r = FinRow Then style.Border.BottomBorder.BorderStyle = BorderStyle
                        If c = IniCol Then style.Border.LeftBorder.BorderStyle = BorderStyle
                        If c = FinCol Then style.Border.RightBorder.BorderStyle = BorderStyle

                        BookSpreadsheetLight.SetCellStyle(r, c, style)
                    Next c
                Next r
            Else

            End If
        Catch ex As Exception
            Console.Write(ex.Message)

        End Try
    End Sub

    Public Sub AutoFitColumn(ByVal Col As Integer, ByVal MaxSize As Integer)
        If mbolIs2007 = True Then
            BookSpreadsheetLight.AutoFitColumn(Col, MaxSize)
        Else

        End If
    End Sub

    Public Sub AutoFitAllColumns()
        If mbolIs2007 = True Then
            Dim iTotalColumns As Integer = BookSpreadsheetLight.GetWorksheetStatistics.EndColumnIndex
            For i As Integer = 1 To iTotalColumns
                BookSpreadsheetLight.AutoFitColumn(i)
            Next
        End If
    End Sub

    Public Sub ColumnSize(ByVal Col As Integer, ColWidth As Double, ColHeight As Double)
        If mbolIs2007 = True Then
            If ColWidth Then BookSpreadsheetLight.SetColumnWidth(Col, ColWidth)
            If ColHeight Then BookSpreadsheetLight.SetRowHeight(Col, ColHeight)
        Else

        End If
    End Sub

    Public Function GetIDSheet(ByVal SheetName As String) As Integer
        Dim IDSheet As Integer = -1
        Dim i As Integer = 0

        SheetName = SheetName.ToUpper

        If mbolIs2007 = True Then
            For i = 0 To BookSpreadsheetLight.GetSheetNames.Count - 1
                If SheetName = BookSpreadsheetLight.GetSheetNames.Item(i).ToUpper Then
                    IDSheet = i
                    Exit For
                End If
            Next
        Else
            For i = 0 To BookExcelLibrary.Worksheets.Count - 1
                If SheetName = BookExcelLibrary.Worksheets(i).Name.ToUpper Then
                    IDSheet = i
                    Exit For
                End If
            Next
        End If

        Return IDSheet

    End Function

    Public Sub SetPattern(ByVal row As Integer, ByVal Col As Integer, ByVal BackColor As System.Drawing.Color, Optional ForeColor As System.Drawing.Color = Nothing)
        Try
            If mbolIs2007 = True Then
                Dim style As New SLStyle

                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, BackColor, System.Drawing.Color.Blue)
                If Not IsNothing(ForeColor) Then style.Font.FontColor = ForeColor
                BookSpreadsheetLight.SetCellStyle(row, Col, style)
            Else
                ' NO FUNCIONA  http://code.google.com/p/excellibrary/issues/detail?id=49&q=backcolor&colspec=ID%20Type%20Status%20Priority%20ReportedBy%20Owner%20Summary%20Opened
                BookExcelLibrary.Worksheets(0).Cells(row - 1, Col - 1).Style = New ExcelLibrary.SpreadSheet.CellStyle
                BookExcelLibrary.Worksheets(0).Cells(row - 1, Col - 1).Style.BackColor = System.Drawing.Color.Red
            End If
        Catch ex As Exception
            Console.Write(ex.Message)

        End Try
    End Sub

    Public Sub MergeCells(ByVal IniRow As Integer, ByVal IniCol As Integer, ByVal EndRow As Integer, ByVal EndCol As Integer)
        Try
            If mbolIs2007 = True Then
                BookSpreadsheetLight.MergeWorksheetCells(IniRow, IniCol, EndRow, EndCol)
            Else

            End If
        Catch ex As Exception
            Console.Write(ex.Message)

        End Try
    End Sub

    Public Sub SaveFile()
        Try
            Dim ostream As New MemoryStream
            BookSpreadsheetLight.SaveAs(ostream)
            ' Guardamos en Cloud
            Azure.RoAzureSupport.SaveFileOnAzure(ostream.ToArray(), mPathName, DTOs.roLiveQueueTypes.datalink)
        Catch ex As Exception
            'do nothing
        End Try
    End Sub

    Public Function GetColumnLetter(ByVal iCol As Integer) As String
        Dim z As Double
        Dim l As String

        'Funciona unicamente para las primeras 702 columnas (ZZ)
        Dim Letra() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}

        z = (iCol - 1) Mod 26 : l = Letra(z)
        If iCol > 26 Then
            z = (iCol - 1) / 26
            z = Int(z)
            If z > 0 Then l = Letra(z - 1) & l
        End If

        Return l
    End Function

    Public Function AddWorkSheet(name As String) As Boolean
        Return BookSpreadsheetLight.AddWorksheet(name)
    End Function

    Public Function RenameSheet(oldName As String, newName As String) As Boolean
        Return BookSpreadsheetLight.RenameWorksheet(oldName, newName)
    End Function

    Public Function RenameSheet(id As Integer, newName As String) As Boolean
        If BookSpreadsheetLight.GetSheetNames.Count >= id Then
            Return BookSpreadsheetLight.RenameWorksheet(BookSpreadsheetLight.GetSheetNames.Item(id), newName)
        Else
            Return False
        End If
    End Function

    Public Function SelectWorksheet(sheetName As String) As Boolean
        Return BookSpreadsheetLight.SelectWorksheet(sheetName)
    End Function

    Public Sub Sort(iStartRowIndex As Integer, iStartColumnIndex As Integer, iEndRowIndex As Integer, iEndColumnIndex As Integer, iSortByColumnIndex As Integer, bSortAscending As Boolean)
        BookSpreadsheetLight.Sort(iStartRowIndex, iStartColumnIndex, iEndRowIndex, iEndColumnIndex, iSortByColumnIndex, bSortAscending)
    End Sub

#End Region

End Class

#End Region

#Region "ZipExport Support Class"

Public Class ZipExport

    Public zipExportFile As ZipFile

    Private mPathName As String

    Public Sub New(ByVal PathName As String)
        mPathName = PathName

        zipExportFile = New Ionic.Zip.ZipFile(PathName)
    End Sub

    Public Sub SaveFile()
        Try
            Dim ostream As New MemoryStream
            zipExportFile.Save(ostream)
            ' Guardamos en Cloud
            Azure.RoAzureSupport.SaveFileOnAzure(ostream.ToArray(), mPathName, DTOs.roLiveQueueTypes.datalink)
        Catch ex As Exception

        End Try
    End Sub

End Class

#End Region