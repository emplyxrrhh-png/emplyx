Imports System.Data.Common
Imports System.IO
Imports DocumentFormat.OpenXml.Packaging
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Mail
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roDataLinkImport
        Inherits roExternalSystemBase

#Region "Declarations and Constructors"

        Protected bolIs2007 As Nullable(Of Boolean)
        Protected strFileNameExcel As String
        Protected bolIsFileOKExcel As Boolean
        Protected bolDeleteFileExcel As Boolean

        Protected bolIsFileOKAscii As Boolean
        Protected strFileNameAscii As String
        Protected bolDeleteFileAscii As Boolean

        Protected bolIsFileOKXML As Boolean
        Protected strFileNameXML As String
        Protected bolDeleteFileXML As Boolean

        Protected BookSpreadsheetLight As SpreadsheetLight.SLDocument   'Componente para versiones >= 2007
        Protected BookExcelLibrary As ExcelLibrary.SpreadSheet.Workbook 'Componente para versiones <= 2003
        Protected ActiveSheetExcelLibrary As Integer

        Protected oAsciiReader As IO.StreamReader
        Protected oAsciiData As Byte()

        Protected oXMLReader As Xml.XmlDocument

        Protected dateImportFileCreationDate As DateTime = Now.Date
        Protected intIDImportGuide As Integer = 0
        Protected mIsAutomaticProcess As Boolean = False

        Public Property ImportFileCreationDateTime As DateTime
            Get
                Return dateImportFileCreationDate
            End Get
            Set(value As DateTime)
                dateImportFileCreationDate = value
            End Set
        End Property

        Public Property IDImportGuide As Integer
            Get
                Return intIDImportGuide
            End Get
            Set(value As Integer)
                intIDImportGuide = value
            End Set
        End Property

        Public Property IsAutomaticProcess As Boolean
            Get
                Return mIsAutomaticProcess
            End Get
            Set(value As Boolean)
                mIsAutomaticProcess = value
            End Set
        End Property

        ''' <summary>
        ''' Constructor Base
        ''' </summary>
        ''' <param name="_State"></param>
        Public Sub New(Optional ByVal _State As roDataLinkState = Nothing)
            MyBase.New(_State)

            Me.bolIsFileOKAscii = False
            Me.strFileNameAscii = String.Empty
            Me.bolDeleteFileAscii = False

            Me.bolIsFileOKExcel = False
            Me.strFileNameExcel = String.Empty
            Me.bolDeleteFileExcel = False



            bolIsFileOKXML = False
            strFileNameXML = String.Empty
            bolDeleteFileXML = False

            ' Cargo parámetro avanzado para personalizaciones

        End Sub

        Public Sub New(ByVal ImportType As eImportType, ByVal oImportFile As Byte(), Optional ByVal _State As roDataLinkState = Nothing)
            Me.New(_State)
            Try
                Select Case ImportType
                    Case eImportType.IsCustom
                    Case eImportType.IsAsciiType
                    Case eImportType.IsExcelType
                        Me.bolDeleteFileExcel = True
                        CheckXLSIs2007Version(oImportFile)
                    Case eImportType.IsXMLType
                End Select
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::New")
            End Try
        End Sub

        ''' <summary>
        ''' CONSTRUCTOR DE FICHERO ASCII O FICHERO EXCEL O FICHERO XML
        ''' </summary>
        ''' <param name="ImportType"></param>
        ''' <param name="FileNameAsciiOExcelOXML"></param>
        ''' <param name="_State"></param>
        Public Sub New(ByVal ImportType As eImportType, ByVal FileNameAsciiOExcelOXML As String, Optional ByVal _State As roDataLinkState = Nothing)
            Me.New(_State)
            Try

                Select Case ImportType
                    Case eImportType.IsCustom
                        Me.bolIsFileOKAscii = False

                        Me.bolIsFileOKExcel = False
                        Me.bolIs2007 = False

                        Me.bolIsFileOKXML = False

                        Me.bolDeleteFileAscii = False
                        Me.bolDeleteFileExcel = False
                    Case eImportType.IsAsciiType
                        Me.strFileNameAscii = FileNameAsciiOExcelOXML
                        CheckAsciiFile()

                    Case eImportType.IsExcelType
                        Me.strFileNameExcel = FileNameAsciiOExcelOXML
                        CheckXLSIs2007Version()

                    Case eImportType.IsXMLType
                        Me.strFileNameXML = FileNameAsciiOExcelOXML
                        CheckXMLFile()

                End Select
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::New")
            End Try
        End Sub

        ''' <summary>
        ''' CONSTRUCTOR DE FICHERO ASCII Y FICHERO EXCEL
        ''' </summary>
        ''' <param name="FileNameAscii"></param>
        ''' <param name="FileNameExcel"></param>
        ''' <param name="_State"></param>
        Public Sub New(ByVal FileNameAscii As String, ByVal FileNameExcel As String, Optional ByVal _State As roDataLinkState = Nothing)
            Me.New(_State)
            Try
                Me.strFileNameAscii = FileNameAscii
                CheckAsciiFile()

                Me.strFileNameExcel = FileNameExcel
                CheckXLSIs2007Version()
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::New")
            End Try
        End Sub

#End Region

#Region "EXCEL FUNCTIONS"

        Private Enum CellType
            Time_Format
            Date_Format
            Currency_Format
            Percentage_Format
            Numeric_Format
            String_Format
        End Enum

        Private Function GetExcelCellDataType(ByVal cellStyle As SpreadsheetLight.SLStyle) As CellType

            If cellStyle.FormatCode.Contains("h:mm") OrElse cellStyle.FormatCode.Contains("mm:ss") Then
                Return CellType.Time_Format
            ElseIf cellStyle.FormatCode.Contains("[$-409]") OrElse cellStyle.FormatCode.Contains("[$-F800]") OrElse cellStyle.FormatCode.Contains("m") OrElse cellStyle.FormatCode.Contains("d") OrElse cellStyle.FormatCode.Contains("y") Then
                Return CellType.Date_Format
            ElseIf cellStyle.FormatCode.Contains("#,##0.0") Then
                Return CellType.Currency_Format
            ElseIf cellStyle.FormatCode.EndsWith("%") Then
                Return CellType.Percentage_Format
            ElseIf cellStyle.FormatCode.IndexOf("0") = 0 Then
                Return CellType.Numeric_Format
            Else
                Return CellType.String_Format
            End If
        End Function

        Public Function GetSheetsCount() As Integer
            If Me.bolIs2007 Then
                Return BookSpreadsheetLight.GetSheetNames().Count
            Else
                Return BookExcelLibrary.Worksheets.Count
            End If
        End Function

        Public Sub SetActiveSheet(ByVal NumSheet)
            If Me.bolIs2007 Then
                BookSpreadsheetLight.SelectWorksheet(BookSpreadsheetLight.GetSheetNames()(0))
            Else
                ActiveSheetExcelLibrary = NumSheet
            End If
        End Sub

        Public Function CheckColumnsExcel(ByVal ColInicial As Integer) As Integer
            Dim intCol As Integer = ColInicial

            Try
                While GetCellValueWithoutFormat(0, intCol) <> ""
                    If CellFormatIsOK(0, intCol) = False Then
                        Exit Try
                    End If

                    intCol += 1
                End While

                intCol = -1
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CheckColumns")
            End Try

            Return intCol
        End Function

        Public Function CountLinesExcel(ByRef pintOrigen As Integer, ByRef pintFinal As Integer, Optional ByVal IncreaseInitialRows As Integer = 0) As Integer
            Dim intRet As Integer = -1
            Dim intRow As Integer = 0

            Try
                'recorremos la hoja hasta econtrar un blanco
                pintOrigen = 1 + IncreaseInitialRows
                intRow = pintOrigen

                While GetCellValueWithoutFormat(intRow, 0) <> ""
                    intRow += 1
                End While

                pintFinal = intRow - 1
                intRet = intRow - pintOrigen
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CountLinesExcel")
            End Try

            Return intRet

        End Function

        Public Function GetCellValue(ByVal RowIndex As Integer, ByVal ColIndex As Integer, Optional ByVal Upper As Boolean = False) As String
            Dim strValue As String = String.Empty

            If Me.bolIs2007 Then

                RowIndex += 1
                ColIndex += 1

                Try
                    Dim cellStyle As SpreadsheetLight.SLStyle = BookSpreadsheetLight.GetCellStyle(RowIndex, ColIndex)
                    Select Case GetExcelCellDataType(cellStyle)
                        Case CellType.Date_Format
                            strValue = Format(BookSpreadsheetLight.GetCellValueAsDateTime(RowIndex, ColIndex, BookSpreadsheetLight.GetCellStyle(RowIndex, ColIndex).FormatCode.Replace("mm", "MM")), "yyyy/MM/dd").Trim
                            If strValue = "1900/01/01" Then strValue = BookSpreadsheetLight.GetCellValueAsString(RowIndex, ColIndex).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
                        Case CellType.Numeric_Format
                            strValue = BookSpreadsheetLight.GetCellValueAsString(RowIndex, ColIndex).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim().Replace(".", roConversions.GetDecimalDigitFormat())
                        Case Else
                            strValue = BookSpreadsheetLight.GetCellValueAsString(RowIndex, ColIndex).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
                    End Select
                Catch ex As Exception
                    strValue = BookSpreadsheetLight.GetCellValueAsString(RowIndex, ColIndex).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
                End Try
            Else
                Dim bolFormatDate As Boolean = False
                Try
                    Dim strformatcode = BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Format.FormatString.ToUpper
                    If BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Format.FormatType = ExcelLibrary.SpreadSheet.CellFormatType.Date Or BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Format.FormatType = ExcelLibrary.SpreadSheet.CellFormatType.DateTime Or strformatcode.Contains("M") Or strformatcode.Contains("D") Or strformatcode.Contains("Y") Then
                        bolFormatDate = True
                    End If
                Catch ex As Exception
                End Try
                If Not bolFormatDate Then
                    strValue = roTypes.Any2String(BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Value).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
                Else
                    strValue = Format(BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).DateTimeValue, "yyyy/MM/dd").Trim
                End If
            End If

            Return IIf(Upper, strValue.ToUpper, strValue)
        End Function

        Public Function GetCellValueWithoutFormat(ByVal RowIndex As Integer, ByVal ColIndex As Integer) As String
            Dim strValue As String = String.Empty

            If Me.bolIs2007 Then
                strValue = BookSpreadsheetLight.GetCellValueAsString(RowIndex + 1, ColIndex + 1).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
            Else
                strValue = roTypes.Any2String(BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Value).Replace(Environment.NewLine, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
            End If

            Return strValue
        End Function

        Public Function CellFormatIsOK(ByVal RowIndex As Integer, ByVal ColIndex As Integer, Optional ByVal Upper As Boolean = False) As Boolean
            Dim bolCellFormatIsOK As Boolean = True

            If Me.bolIs2007 Then
                Dim strformatCode As String = ""

                Try
                    strformatCode = BookSpreadsheetLight.GetCellStyle(RowIndex + 1, ColIndex + 1).FormatCode.ToUpper
                Catch ex As Exception
                    strformatCode = ""
                End Try

                If (strformatCode.Contains("[$-409]") Or strformatCode.Contains("[$-F800]") Or strformatCode.Contains("M") Or strformatCode.Contains("D") Or strformatCode.Contains("Y")) Then
                    bolCellFormatIsOK = False
                End If
            Else
                Try
                    If BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Format.FormatType = ExcelLibrary.SpreadSheet.CellFormatType.Date Or BookExcelLibrary.Worksheets(Me.ActiveSheetExcelLibrary).Cells(RowIndex, ColIndex).Format.FormatType = ExcelLibrary.SpreadSheet.CellFormatType.DateTime Then
                        bolCellFormatIsOK = False
                    End If
                Catch ex As Exception
                End Try
            End If

            Return bolCellFormatIsOK
        End Function

#End Region

#Region "ASCII FUNCTION"

        Protected Function CountLinesAscii() As Integer
            Dim intRet As Integer = 0

            Try

                While Not oAsciiReader.EndOfStream
                    oAsciiReader.ReadLine()
                    intRet += 1
                End While
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CountLinesAscii")
            Finally
                If Not IsNothing(oAsciiReader) Then oAsciiReader.Close()
                CheckAsciiFile()
            End Try

            Return intRet

        End Function

#End Region

#Region "General Helper Methods"

        Public Enum eImportGuide
            Employees = 1
            Scheduling = 2
            DailyCoverage = 3
            Groups = 4
            Taks = 5
            Assignments = 6
            Holidays = 7
            ProgrammedCauses = 8
            ProgrammedAbsencesA = 9
            CalendarAbsencesASCII = 10
            DailyCauses = 11
            VSL_WorkSheets = 12
            ImportBusinessCenter = 13
            ImportPlanningV2 = 14
            EmployeePhoto = 15
        End Enum

        Protected Sub CheckXLSIs2007Version()
            Dim oFile As IO.FileInfo = New IO.FileInfo(Me.strFileNameExcel)
            If oFile.Length > 0 Then
                Try
                    Me.BookSpreadsheetLight = New SpreadsheetLight.SLDocument(Me.strFileNameExcel)
                    Me.bolIsFileOKExcel = True
                    Me.bolIs2007 = True
                Catch ex As Exception
                    Try
                        Me.BookExcelLibrary = ExcelLibrary.SpreadSheet.Workbook.Load(Me.strFileNameExcel)
                        Me.bolIsFileOKExcel = True
                        Me.bolIs2007 = False
                        Me.ActiveSheetExcelLibrary = 0
                    Catch ex2 As Exception
                        'Me.State.UpdateStateInfo(ex2, "roDataLinkImport::CheckXLSIs2007Version")
                    End Try
                End Try
            End If
        End Sub

        Protected Sub CheckXLSIs2007Version(ByVal oBytes As Byte())
            If oBytes.Length > 0 Then
                Try
                    Me.BookSpreadsheetLight = New SpreadsheetLight.SLDocument(New MemoryStream(oBytes))
                    If BookSpreadsheetLight.GetSheetNames.Count > 0 Then
                        Me.bolIsFileOKExcel = True
                        Me.bolIs2007 = True
                    End If
                Catch ex As Exception
                    Try
                        Me.BookExcelLibrary = ExcelLibrary.SpreadSheet.Workbook.Load(New MemoryStream(oBytes))
                        Me.bolIsFileOKExcel = True
                        Me.bolIs2007 = False
                        Me.ActiveSheetExcelLibrary = 0
                    Catch ex2 As Exception
                    End Try
                End Try
            End If
        End Sub

        Protected Sub CheckAsciiFile()
            Dim oFile As IO.FileInfo = New IO.FileInfo(Me.strFileNameAscii)
            If oFile.Length > 0 Then
                Try
                    Dim oFileEncoding As System.Text.Encoding = VTBase.roLanguageFileEncoding.DetectTextFileEncoding(Me.strFileNameAscii)
                    Me.oAsciiReader = New IO.StreamReader(Me.strFileNameAscii, oFileEncoding)
                    Me.bolIsFileOKAscii = True
                Catch ex As Exception
                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::CheckAsciiFile")
                End Try
            End If
        End Sub

        Protected Sub CheckXMLFile()
            Dim oFile As IO.FileInfo = New IO.FileInfo(Me.strFileNameXML)
            If oFile.Length > 0 Then
                Try
                    Me.oXMLReader = New Xml.XmlDocument()
                    Me.oXMLReader.Load(Me.strFileNameXML)
                    Me.bolIsFileOKXML = True
                Catch ex As Exception
                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::CheckXMLFile")
                End Try
            End If
        End Sub

        Public Function SaveImportLog(ByVal intIDImport As Integer, ByVal strLogMessage As String, Optional ByVal IDPassport As Integer = -1, Optional bAvoidNotification As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                Dim strName As String = ""

                strSQL = "@SELECT# Name from ImportGuides WHERE ID=" & intIDImport
                strName = roTypes.Any2String(ExecuteScalar(strSQL))
                If Not String.IsNullOrEmpty(strName) Then
                    'Insertamos la información de la importación realziada
                    strSQL = "@UPDATE# ImportGuides Set LastLog='" & strLogMessage.Replace(",", ".").Replace("'", "''") & "' WHERE ID=" & intIDImport
                    bolRet = ExecuteSql(strSQL)

                    Dim oAuditState As New AuditState.wscAuditState(IDPassport)

                    Dim lstAuditParameterNames As New List(Of String)
                    Dim lstAuditParameterValues As New List(Of String)

                    lstAuditParameterNames.Add("{ImportResult}")
                    lstAuditParameterValues.Add(strLogMessage)

                    Support.roLiveSupport.Audit(VTBase.Audit.Action.aExecuted, VTBase.Audit.ObjectType.tDataLinkResult, strName, lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                    ' En el caso que exista alguna notificacion de importación ejecutada, generamos las alertas necesarias
                    Dim oNotificationState As New Notifications.roNotificationState(-1)
                    Dim oImportDataExecutedNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 61 And Activated=1", oNotificationState,, True)

                    If Not bAvoidNotification AndAlso oImportDataExecutedNotifications IsNot Nothing AndAlso oImportDataExecutedNotifications.Count > 0 Then
                        For Each oNotification As Notifications.roNotification In oImportDataExecutedNotifications
                            strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime, Parameters ) VALUES " &
                                                                "(" & oNotification.ID.ToString & ", " & intIDImport & "," & roTypes.Any2Time(Now).SQLDateTime & ",'" & strLogMessage.Replace(",", ".").Replace("'", "''") & "')"
                            ExecuteSql(strSQL)
                        Next
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::SaveImportLog")
            Finally

            End Try

            Return bolRet

        End Function


#End Region

    End Class

End Namespace