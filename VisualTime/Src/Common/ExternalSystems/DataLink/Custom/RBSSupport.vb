Imports System
Imports System.Data
Imports System.Xml
Imports System.Collections
Imports DocumentFormat.OpenXml.Office2010.PowerPoint

Namespace DataLink

#Region "RBS Management"
    Public Class AccrualCellValueByLine
        Public Shared Function GetValue(scriptName As String, ByRef CamposExportados As Object, ByVal dtRegistrosAExportar As DataTable, ByVal oEmployeeRow As DataRow, ByVal oAccrualRow As DataRow, ByVal dtSaldos As DataTable, ByVal FechaInicial As Date, ByVal FechaFinal As Date) As String
            Dim result As String = String.Empty

            Try
                Select Case scriptName.ToLower
                    Case "FIAT_INFOTIPO_LINE".ToLower
                        result = DataLink.ExportFunctions.FIAT_InfoTipo_Line(CamposExportados, dtRegistrosAExportar, oEmployeeRow, oAccrualRow, dtSaldos, FechaInicial, FechaFinal)
                    Case "LimitaSaldoTrabajadas8".ToLower
                        Dim iColumnaSaldoHorasTrabajadas As Integer = 8
                        Try
                            If CDbl(CamposExportados(iColumnaSaldoHorasTrabajadas).Value) > 8 Then
                                CamposExportados(iColumnaSaldoHorasTrabajadas).Value = 8
                            End If
                            result = ""
                        Catch ex As Exception
                            result = "Error limitando saldo"
                        End Try
                    Case "MultiplicaSaldos".ToLower
                        Dim iColumnaSaldo As Integer = 2
                        Try
                            CamposExportados(iColumnaSaldo).Value = CamposExportados(iColumnaSaldo).Value * 3600
                            CamposExportados(iColumnaSaldo + 1).Value = CamposExportados(iColumnaSaldo + 1).Value * 3600
                            CamposExportados(iColumnaSaldo + 2).Value = CamposExportados(iColumnaSaldo + 2).Value * 3600
                        Catch ex As Exception
                            result = "Error pasando saldo a minutos"
                        End Try
                    Case Else
                        result = "RBS not supported"
                End Select

            Catch ex As Exception
                result = "Exception getting value"
            End Try
            Return result
        End Function
    End Class

    Public Class AccrualCellValueByHead
        Private CamposDeLaFicha As DataTable = Nothing
        Private Contratos As DataTable = Nothing
        Private Niveles As DataTable = Nothing
        Private Mobilidades As DataTable = Nothing
        Private Autorizaciones As DataTable = Nothing

        Private Function ObtenCampoDeLaFicha(ByVal strCampo As String) As String
            Return ExportFunctions.ObtenCampoDeLaFicha(strCampo, CamposDeLaFicha)
        End Function

        Private Function ContratoActual() As DataRow
            Return ExportFunctions.ObtenUltimoContrato(Contratos)
        End Function

        Private Function MobilidadActual() As DataRow
            Return ExportFunctions.ObtenUltimaMobilidad(Mobilidades)
        End Function

        Private Function GrupoActual() As DataRow
            Return Niveles.Rows(Niveles.Rows.Count - 1)
        End Function

        Public Function GetValue(scriptName As String, ByRef CamposExportados As Object, ByVal dtRegistrosAExportar As DataTable, ByVal Empleado As DataRow, ByVal UserFields As DataTable, ByVal Contracts As DataTable, ByVal Groups As DataTable, ByVal Movilities As DataTable, ByVal Authorizations As DataTable, ByVal RoturaFechaInicial As Date, ByVal RoturaFechaFinal As Date, ByVal FechaInicial As Date, ByVal FechaFinal As Date, ByVal NombrePlantillaExcel As String) As String
            Dim result As String = String.Empty

            Try
                CamposDeLaFicha = UserFields
                Contratos = Contracts
                Niveles = Groups
                Mobilidades = Movilities
                Autorizaciones = Authorizations

                Select Case scriptName.ToLower
                    Case "FIAT_INFOTIPO_HEAD".ToLower
                        result = DataLink.ExportFunctions.FIAT_InfoTipo(Empleado, dtRegistrosAExportar, RoturaFechaInicial, RoturaFechaFinal, FechaInicial, FechaFinal, NombrePlantillaExcel)
                    Case "LEAR_PAGOS_HEAD".ToLower
                        result = DataLink.ExportFunctions.LEAR_Pagos(Empleado, dtRegistrosAExportar, RoturaFechaInicial, RoturaFechaFinal, FechaInicial, FechaFinal, NombrePlantillaExcel)
                    Case Else
                        result = "RBS not supported"
                End Select
            Catch ex As Exception
                result = "Exception getting value"
            End Try

            Return result
        End Function
    End Class

    Public Class CellValueFromCode
        Private CamposDeLaFicha As DataTable = Nothing
        Private Contratos As DataTable = Nothing
        Private Niveles As DataTable = Nothing
        Private Mobilidades As DataTable = Nothing
        Private Autorizaciones As DataTable = Nothing

        Private Function ObtenCampoDeLaFicha(ByVal strCampo As String) As String
            Return ExportFunctions.ObtenCampoDeLaFicha(strCampo, CamposDeLaFicha)
        End Function

        Private Function ContratoActual() As DataRow
            Return ExportFunctions.ObtenUltimoContrato(Contratos)
        End Function

        Private Function MobilidadActual() As DataRow
            Return ExportFunctions.ObtenUltimaMobilidad(Mobilidades)
        End Function

        Private Function GrupoActual() As DataRow
            Return Niveles.Rows(Niveles.Rows.Count - 1)
        End Function

        Public Function GetValue(ByVal scriptName As String, ByRef CamposExportados As Object, ByVal dtRegistrosAExportar As DataTable, ByVal Empleado As DataRow, ByVal UserFields As DataTable, ByVal Contracts As DataTable, ByVal Groups As DataTable, ByVal Movilities As DataTable, ByVal Authorizations As DataTable, ByVal FechaInicial As Date, ByVal FechaFinal As Date) As String
            Dim result As String = String.Empty
            Try
                CamposDeLaFicha = UserFields
                Contratos = Contracts
                Niveles = Groups
                Mobilidades = Movilities
                Autorizaciones = Authorizations

                Select Case scriptName.ToLower
                    Case "plantCodeFiat".ToLower
                        Try
                            result = DataLink.ExportFunctions.FIAT_PlantCode(Niveles.Rows(0)("Name"))
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "sectionFiat".ToLower
                        Try
                            Dim section As String = Niveles.Rows(6)("Name")
                            If section <> String.Empty Then
                                If section.Contains(" ") Then
                                    result = section.Substring(0, section.IndexOf(" "))
                                Else
                                    result = section
                                End If
                                While result.Length < 5
                                    result = $"0{result}"
                                End While
                            Else
                                result = ""
                            End If
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "workAccrualFiat".ToLower
                        Try
                            result = DataLink.ExportFunctions.FIAT_CalculateISSEAccrualValue(Empleado, FechaInicial, FechaFinal, Niveles.Rows(0)("Name"))
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "yearmonthFiat".ToLower
                        result = Date.Now.ToString("MMyyyy")
                    Case "codigoFiat".ToLower
                        Try
                            Dim section As String = Niveles.Rows(Niveles.Rows.Count - 1)("Name")
                            If section <> String.Empty Then
                                If section.Contains(" ") Then
                                    result = section.Substring(0, section.IndexOf(" "))
                                Else
                                    result = section
                                End If
                            Else
                                result = ""
                            End If
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "codigoDescFiat".ToLower
                        Try
                            Dim section As String = Niveles.Rows(Niveles.Rows.Count - 1)("Name")
                            If section <> String.Empty Then
                                result = section.Substring(section.IndexOf(" ") + 1)
                            Else
                                result = ""
                            End If
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "cuentaFiat".ToLower
                        Try
                            Dim gActual As String = (GrupoActual("DescriptionGroup")).ToString()
                            If gActual <> String.Empty Then
                                gActual = gActual.Split(";")(0)
                                If gActual.Contains("#") Then
                                    gActual = gActual.Split("#")(1)
                                Else
                                    gActual = ""
                                End If
                            End If
                            result = gActual
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case "inicioPuestoFiat".ToLower
                        Try
                            result = CDate(MobilidadActual("BeginDate")).ToString("dd/MM/yyyy")
                        Catch ex As Exception
                            result = ""
                        End Try
                    Case Else
                        result = "RBS not supported"
                End Select
            Catch ex As Exception
                result = "Exception getting value"
            End Try

            Return result
        End Function
    End Class


#End Region

End Namespace