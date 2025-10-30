Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Namespace DataLink


    Public Class roSqlExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfileFreeExcel(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal sExcelProfileName As String, ByVal sDataSourceFile As String, ByRef oState As roDataLinkState, Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = ProfileExportBody.FileTypeExport.typ_2007

                arrFile = ExportFree(intIDExport, EmployeesFilter, BeginDate, EndDate, sExcelProfileName, sDataSourceFile)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ExportProfilePunchesExEXCEL")

            End Try

            Return arrFile
        End Function

        Private Function ExportFree(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal sDataSourceFile As String) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportPunches.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)
                If ExcelProfile.FileIsOK Then
                    arrFile = FreeWrite(intIDExport, EmployeesFilter, BeginDate, EndDate, ExcelProfile, sDataSourceFile)
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportPunches.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportPunchesEx")

            End Try

            Return arrFile
        End Function

        Private Function FreeWrite(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef ExcelProfile As ExcelExport, ByVal sDataSourceFile As String) As Byte()
            Dim bolRet As Boolean = False
            Dim arrFile As Byte() = Nothing
            Dim OutputFileName As String = ""

            Try

                Dim Ext As String = "xlsx"

                ' Crea el fichero temporal
                OutputFileName = CreateTempFile("free", Ext, Me.State)

                Dim myExcelProfile As ProfileExportFree

                ' Crea la plantilla para lanzamiento manual
                myExcelProfile = New ProfileExportFree(EmployeesFilter, OutputFileName, BeginDate, EndDate, sDataSourceFile, Me.State)

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

                ' Determina la hoja correspondiente a Saldos
                Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")

                ' Si no la encuentra no hace nada
                If idSheet = -1 Then
                    ' msgLog = Me.state.Language.Translate("ResultEnum.Sheet1NotFound", "") & vbNewLine
                    Exit Try
                End If

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

                ' Exporta fichajes
                If myExcelProfile.ExportProfile() = True Then
                    ' Lee el fichero temporal para convertirlo en array de bytes
                    arrFile = IO.File.ReadAllBytes(OutputFileName)
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::FreeWrite")

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