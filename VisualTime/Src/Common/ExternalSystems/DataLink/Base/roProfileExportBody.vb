Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink
    Public Class ProfileExportBody

        Public Enum FileTypeExport
            typ_ASCII
            typ_2003
            typ_2007
        End Enum

        Private oState As roDataLinkState
        Private mDelimitedChar As String = ""
        Private mOutputFileName As String = ""
        Private mOutputFileType As FileTypeExport = Nothing
        Private mObjStreamWriter As System.IO.FileStream
        Private mMemoryStreamWriter As MemoryStream
        Private mExcelFile As ExcelExport = Nothing
        Private mExcelRow As Integer = 2
        Private mFields As New List(Of ProfileExportFields)
        Private mAdvancedParameters As New Dictionary(Of String, String)(StringComparer.InvariantCultureIgnoreCase)

        Public Sub New(OutputFileName As String, OutputFileType As FileTypeExport, ByVal DelimitedChar As String, ByVal _State As roDataLinkState)
            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mOutputFileName = OutputFileName
            mDelimitedChar = DelimitedChar
            mOutputFileType = OutputFileType
        End Sub

        Public Property CurrentExcelRow() As Integer
            Get
                Return Me.mExcelRow
            End Get
            Set(value As Integer)
                Me.mExcelRow = value
            End Set
        End Property

        Public ReadOnly Property OutputFileName() As String
            Get
                Return Me.mOutputFileName
            End Get
        End Property

        Public Property Fields() As List(Of ProfileExportFields)
            Get
                Return Me.mFields
            End Get
            Set(ByVal value As List(Of ProfileExportFields))
                Me.mFields = value
            End Set
        End Property

        Public Property ObjStreamWriter As System.IO.FileStream
            Get
                Return Me.mObjStreamWriter
            End Get
            Set(ByVal value As System.IO.FileStream)
                Me.mObjStreamWriter = value
            End Set
        End Property

        Public Property MemoryStreamWriter As MemoryStream
            Get
                Return Me.mMemoryStreamWriter
            End Get
            Set(ByVal value As MemoryStream)
                Me.mMemoryStreamWriter = value
            End Set
        End Property

        Public Property DelimitedChar As String
            Get
                Return Me.mDelimitedChar
            End Get
            Set(ByVal value As String)
                Me.mDelimitedChar = value
            End Set
        End Property

        Public Property AdvancedParameters As Dictionary(Of String, String)
            Get
                Return Me.mAdvancedParameters
            End Get
            Set(value As Dictionary(Of String, String))
                Me.mAdvancedParameters = value
            End Set
        End Property

        Public Function CreateLine() As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Graba la línea
                Select Case mOutputFileType
                    Case FileTypeExport.typ_ASCII
                        bolRet = CreateAsciiLine()
                    Case FileTypeExport.typ_2003, FileTypeExport.typ_2007
                        bolRet = CreateExcelLine(True)
                End Select
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateLine")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function CreateSheet(name As String) As Boolean
            Select Case mOutputFileType
                Case FileTypeExport.typ_2003, FileTypeExport.typ_2007
                    Return mExcelFile.AddWorkSheet(name)
                Case Else
                    Return True
            End Select
        End Function

        Public Function CreateShiftLines(lstShifts As List(Of Shift.roShift)) As Boolean
            If Me.mOutputFileType = FileTypeExport.typ_ASCII Then Return True

            Dim bolRet = False
            mExcelRow = 1
            Try
                For Each shiftData As Shift.roShift In lstShifts
                    Dim cellCount = 1
                    mExcelFile.SetCellValue(mExcelRow, cellCount, shiftData.ID)
                    cellCount += 1
                    mExcelFile.SetCellValue(mExcelRow, cellCount, shiftData.ExportName)
                    cellCount += 1
                    mExcelFile.SetCellValue(mExcelRow, cellCount, shiftData.ShortName)
                    cellCount += 1
                    mExcelFile.SetCellValue(mExcelRow, cellCount, shiftData.Name)
                    cellCount += 1
                    mExcelFile.SetCellValue(mExcelRow, cellCount, shiftData.ExpectedWorkingHours)
                    cellCount += 1
                    For Each oMandatoryLayer As Shift.roShiftLayer In shiftData.Layers.Cast(Of Shift.roShiftLayer)().ToList().Where(Function(l) l.LayerType.Equals(roLayerTypes.roLTMandatory))
                        mExcelFile.SetCellValue(mExcelRow, cellCount, oMandatoryLayer.Data("Begin"), "dd/MM/yyyy HH:mm")
                        cellCount += 1
                        mExcelFile.SetCellValue(mExcelRow, cellCount, oMandatoryLayer.Data("Finish"), "dd/MM/yyyy HH:mm")
                        cellCount += 1
                    Next
                    mExcelRow += 1
                    bolRet = True
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateExcelLine")
            End Try
            Return bolRet
        End Function

        Public Function CreateHeaderA3(ByVal strHeaderA3 As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Graba la línea del excel

                If StringItemsCount(strHeaderA3, "⌂") = 21 Then
                    ' Linea 1
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 0, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 1, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 2, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 2
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 3, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 4, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 5, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 3
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 6, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 7, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 8, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 4
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 9, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 10, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 11, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 5
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 12, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 13, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 14, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 6
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 15, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 16, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 17, "⌂"))

                    Me.mExcelRow += 1

                    ' Linea 7
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 3, String2Item(strHeaderA3, 18, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 4, String2Item(strHeaderA3, 19, "⌂"))
                    Me.mExcelFile.SetCellValue(Me.mExcelRow, 5, String2Item(strHeaderA3, 20, "⌂"))

                    Me.mExcelRow += 1

                End If

                bolRet = True
            Catch ex As Exception
            End Try

            Return bolRet

        End Function

        Public Sub SortByColumnIndex(iSortColumnIndex As Integer, bSortAscending As Boolean, Optional iRowOffset As Integer = 0)
            Select Case mOutputFileType
                Case FileTypeExport.typ_ASCII
                    'do nothing on ascii. order is not possible
                Case FileTypeExport.typ_2003, FileTypeExport.typ_2007
                    Dim sStats As New SpreadsheetLight.SLWorksheetStatistics
                    sStats = Me.mExcelFile.GetWorksheetStatistics
                    Dim iStartColumnIndex As Integer = sStats.StartColumnIndex
                    Dim iStartRowIndex As Integer = sStats.StartRowIndex + 1 + iRowOffset ' Sumo uno para no incluir la cabecera en la ordenación
                    Dim iEndColumnIndex As Integer = sStats.EndColumnIndex
                    Dim iEndRowIndex As Integer = sStats.EndRowIndex

                    Try
                        Me.mExcelFile.Sort(iStartRowIndex, iStartColumnIndex, iEndRowIndex, iEndColumnIndex, iSortColumnIndex, bSortAscending)
                    Catch ex As Exception
                    End Try
            End Select
        End Sub

        Private Function CreateAsciiLine() As Boolean
            Dim bolRet As Boolean = False
            Dim i As Integer = 0

            Try
                Dim str As String = ""
                For i = 0 To Me.mFields.Count - 1
                    ' Asigna el caracter delimitador
                    If Me.mDelimitedChar <> "" AndAlso str <> "" Then str += Me.mDelimitedChar

                    str += Me.mFields(i).ValueFormated
                Next

                ' Graba la línea
                'Me.mObjStreamWriter.WriteLine(str)

                Dim b() As Byte = System.Text.Encoding.Default.GetBytes(str & vbNewLine)
                Me.mMemoryStreamWriter.Write(b, 0, b.Length)

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateAsciiLine")
            End Try

            Return bolRet
        End Function

        Private Function CreateExcelLine(ByVal IncreaseRowLine As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim i As Integer = 0

            Try
                ' Graba la línea del excel
                For i = 0 To Me.mFields.Count - 1
                    'Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, Me.mFields(i).ValueFormated)
                    Select Case Me.mFields(i).DataType
                        Case ProfileExportFields.Type.typ_Numeric
                            'Trato el caso de haber obtenido una división por cero
                            If Me.mFields(i).Value.ToString = "DIV0" Then
                                Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, "DIV0")
                            Else
                                Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, roTypes.Any2Double(Me.mFields(i).ValueFormated))
                            End If
                        Case ProfileExportFields.Type.typ_Date
                            If Me.mFields(i).ValueFormated.Trim() <> String.Empty Then
                                Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, roTypes.Any2DateTime(Me.mFields(i).Value), mFields(i).DataFormat)
                            Else
                                Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, "")
                            End If
                        Case Else
                            'Formato texto. Si es un campo de lista, no se le aplica formato (porque Excel se vuelve loco)
                            Me.mExcelFile.SetCellValue(Me.mExcelRow, 1 + i, Me.mFields(i).ValueFormated, IIf(Not Me.mFields(i).GetValueFromList, mFields(i).DataFormat, ""))
                    End Select
                Next

                ' Incrementa el registro de la hoja
                If IncreaseRowLine Then Me.mExcelRow += 1

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateExcelLine")
            End Try

            Return bolRet
        End Function

        Public Function GetExcelCells(ByVal IncreaseRowLine As Boolean) As List(Of KeyValuePair(Of Integer, Object))
            Dim bolRet As Boolean = False
            Dim row As New List(Of KeyValuePair(Of Integer, Object))
            Dim value As Object

            Try
                ' Graba la línea del excel
                For i = 0 To Me.mFields.Count - 1
                    Select Case Me.mFields(i).DataType
                        Case ProfileExportFields.Type.typ_Numeric
                            If Me.mFields(i).Value.ToString = "DIV0" Then
                                value = "DIV0"
                            Else
                                value = roTypes.Any2Double(Me.mFields(i).ValueFormated)
                            End If
                        Case ProfileExportFields.Type.typ_Date

                            If Me.mFields(i).ValueFormated.Trim() <> String.Empty Then
                                value = roTypes.Any2DateTime(Me.mFields(i).Value).ToString(mFields(i).DataFormat)
                            Else
                                value = ""
                            End If
                        Case Else
                            value = Me.mFields(i).ValueFormated.ToString()
                    End Select

                    row.Add(New KeyValuePair(Of Integer, Object)(i + 1, value))
                Next

                If IncreaseRowLine Then Me.mExcelRow += 1

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateExcelLine")
            End Try
            Return row
        End Function

        'Public Function FileOpen(Optional oStrm As StreamWriter = Nothing) As Boolean
        Public Function FileOpen(Optional oStrm As System.IO.FileStream = Nothing, Optional strHeader As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Si ya viene con un fichero asignado lo usa y sale
                If Not IsNothing(oStrm) Then
                    Return True
                End If

                ' Crea el fichero
                Select Case mOutputFileType
                    Case FileTypeExport.typ_ASCII
                        'Me.mObjStreamWriter = New StreamWriter(mOutputFileName)
                        Me.mMemoryStreamWriter = New MemoryStream()
                    Case FileTypeExport.typ_2003, FileTypeExport.typ_2007
                        ' Crea el fichero excel
                        If mOutputFileType = FileTypeExport.typ_2003 Then
                            Me.mExcelFile = New ExcelExport(ExcelExport.ExcelVersion.exc_2003, mOutputFileName)
                        Else
                            Me.mExcelFile = New ExcelExport(ExcelExport.ExcelVersion.exc_2007, {}, mOutputFileName)
                        End If

                        Me.mExcelRow = 1
                        ' EN caso necesario crea la cabecera de A3
                        If strHeader.Length > 0 Then
                            Me.CreateHeaderA3(strHeader)
                        End If

                        ' Crea la cabecera
                        CreateExcelHead()
                End Select

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:FileOpen")
            End Try

            Return bolRet
        End Function

        Private Sub CreateExcelHead()
            Try
                Dim i As Integer = 0

                ' Crea la cabecera del excel
                For i = 0 To Me.mFields.Count - 1
                    Me.mExcelFile.SetCellValue(mExcelRow, i + 1, Me.mFields(i).Head)
                Next i
                Me.mExcelRow += 1
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:CreateExcelHead")
            End Try
        End Sub

        Public Function FileClose() As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Cierra el fichero
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDataLinkExport::ProfileExportBody: FileClose " & mOutputFileType.ToString)
                Select Case mOutputFileType
                    Case FileTypeExport.typ_ASCII
                        If Not IsNothing(Me.mMemoryStreamWriter) Then
                            Me.mMemoryStreamWriter.Close()
                        End If
                    Case FileTypeExport.typ_2003, FileTypeExport.typ_2007
                        If Not IsNothing(Me.mExcelFile) Then
                            Me.mExcelFile.AutoFitAllColumns()
                            Me.mExcelFile.SaveFile()
                        End If
                End Select

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:FileClose")
            End Try

            Return bolRet
        End Function

        Public Function FieldExists(ByVal FieldName As String) As Boolean
            Dim bolExists As Boolean = False

            Try
                FieldName = FieldName.ToUpper

                ' Crea la cabecera del excel
                For i As Integer = 0 To Me.mFields.Count - 1
                    If FieldName = Me.Fields(i).Source.ToUpper Then
                        bolExists = True
                        Exit For
                    End If
                Next i
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportBody:FieldExists")
            End Try

            Return bolExists
        End Function

    End Class

End Namespace