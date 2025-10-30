Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Newtonsoft.Json
Imports Robotics.DataLayer
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports SwiftExcel
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Security.Base
Imports Robotics.Base.VTBusiness.DiningRoom
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Azure
Imports System.Activities.Expressions
Imports Robotics.Base.VTSelectorManager
Imports System.Text

Namespace DataLink



    Public Class roCausesExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileDailyCausesASCII(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                arrFile = ExportDailyCauses(intIDExport, tmpEmployeeFilterTable, BeginDate, EndDate, oExcelProfileName, ProfileExportBody.FileTypeExport.typ_ASCII, DelimiterChar, StartCalculDay, IntervalMinutes, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
                If (arrFile Is Nothing OrElse (arrFile IsNot Nothing AndAlso arrFile.Length = 0)) AndAlso Me.State.Result = DataLinkResultEnum.NoError Then arrFile = System.Text.Encoding.Unicode.GetBytes(" ")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileDailyCausesASCII")
            End Try

            Return arrFile
        End Function

        Public Function ExportProfileDailyCausesEXCEL(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelProfileName As String, ByVal OutputFileIsExcel2003 As Boolean, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)

                arrFile = ExportDailyCauses(intIDExport, tmpEmployeeFilterTable, BeginDate, EndDate, oExcelProfileName, ExcelType, "", StartCalculDay, IntervalMinutes, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfileDailyCausesEXCEL")
            End Try

            Return arrFile
        End Function

        Private Function ExportDailyCauses(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim bContinue As Boolean = False

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportDailyCauses.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                ' Diferenciamos si estamos en Cloud
                Dim strExcelFileName As String = String.Empty
                Dim ExcelProfile As ExcelExport = Nothing

                If Not oExcelProfileBytes Is Nothing Then
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile.FileIsOK Then
                        arrFile = DailyCausesWrite(intIDExport, tmpEmployeeFilterTable, BeginDate, EndDate, ExcelProfile, OutputFileType, DelimiterChar, StartCalculDay, IntervalMinutes, msgLog, Field1, Field2, Field3, Field4, Field5, Field6, oExcelProfileBytes)
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportDailyCauses.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportDailyCauses")

            End Try

            Return arrFile
        End Function

        Private Function DailyCausesWrite(ByVal intIDExport As Integer, ByVal tmpEmployeeFilterTable As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal IntervalMinutes As Integer, ByRef msgLog As String,
                                             Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
            Dim bolRet As Boolean = False
            Dim arrFile As Byte() = Nothing
            Dim OutputFileName As String = ""

            msgLog = ""

            Try

                Dim Ext As String = "xlsx"

                ' Determina la extensión del fichero de salida
                Select Case OutputFileType
                    Case ProfileExportBody.FileTypeExport.typ_2003 : Ext = "xls"
                    Case ProfileExportBody.FileTypeExport.typ_2007 : Ext = "xlsx"
                    Case ProfileExportBody.FileTypeExport.typ_ASCII : Ext = "txt"
                End Select

                ' Crea el fichero temporal
                OutputFileName = "dailycauses#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                ' Determina la hoja correspondiente a fichajes
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    msgLog = Me.State.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

                Dim myExcelProfile As ProfileExportDailyCauses

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
                myExcelProfile = New ProfileExportDailyCauses(tmpEmployeeFilterTable, strCausesFilter, OutputFileName, OutputFileType, DelimiterChar, BeginDate, EndDate, Me.State, Field1, Field2, Field3, Field4, Field5, Field6)

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

                ' Exporta justificaciones
                If intIDExport < 20000 Then
                    ' Estándar. Teóricamente no existe, pero algún cliente la debe tener como IDN. La mantenemos ...
                    If myExcelProfile.ExportProfile() = True Then
                        ' Lee el fichero temporal para convertirlo en array de bytes
                        arrFile = IO.File.ReadAllBytes(OutputFileName)
                    End If
                Else
                    ' Módulo Import/Export
                    If myExcelProfile.ExportProfileIE() = True Then
                        If OutputFileType = ProfileExportBody.FileTypeExport.typ_ASCII Then
                            arrFile = myExcelProfile.Profile.MemoryStreamWriter.ToArray()
                        Else
                            arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(OutputFileName, DTOs.roLiveQueueTypes.datalink)
                        End If
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::DailyCausesWrite")

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