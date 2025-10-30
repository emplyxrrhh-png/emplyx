Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase

Namespace DataLink


    Public Class roApiAccess
        Inherits roDataLinkApi

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function GetTerminalDateTimeById(ByRef oTerminalDateTime As DateTime, ByVal terminalID As String, ByRef strErrorMsg As String, ByRef returnCode As RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bolRet As Boolean = False
            Try
                returnCode = Core.DTOs.ReturnCode._OK
                Dim oTerminal As New Terminal.roTerminal(terminalID, New Terminal.roTerminalState())
                If terminalID > 0 Then
                    oTerminalDateTime = oTerminal.GetCurrentDateTime()
                Else
                    returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._TerminalNotExist
                    strErrorMsg = "Terminal not found"
                    bolRet = False
                End If

            Catch ex As Exception
                returnCode = Core.DTOs.ReturnCode._UnknownError
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetTerminalDateTimeById")
                oTerminalDateTime = Nothing
            End Try

            Return bolRet
        End Function


        Public Function GetTerminalConfigurationById(ByRef oTerminalConfiguration As roTerminalConfiguration, ByVal terminalID As String, ByRef strErrorMsg As String, ByRef returnCode As RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bolRet As Boolean = False

            Try
                returnCode = Core.DTOs.ReturnCode._OK
                ' Comprobamos que el tipo de terminal existe y es compatible
                Dim strSQL As String = "@SELECT# Partners FROM Terminals AS T INNER JOIN sysroReaderTemplates AS RT ON RT.Type = T.Type WHERE T.ID = " & terminalID
                Dim oTerminalsDT As DataTable = DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(strSQL)
                If oTerminalsDT IsNot Nothing AndAlso oTerminalsDT.Rows.Count > 0 Then
                    If oTerminalsDT.Select("Partners = True").Count > 0 Then
                        ' Obtenemos el campo identificador del empleado
                        Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                        If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                        strSQL = "@SELECT# euf.Value from TerminalsSyncEmployeesData as ted inner join employees as e on e.ID = ted.EmployeeId inner join EmployeeUserFieldValues as euf on euf.IDEmployee = ted.EmployeeId and euf.FieldName = '" & strImportPrimaryKeyUserField & "' where TerminalId = " & terminalID

                        ' Cargamos lista de empleados autorizados
                        Dim oEmployeesDT As DataTable = DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(strSQL)
                        If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                            Dim tbEmployees As List(Of roEmployeeOnAccessAuthorization) = New List(Of roEmployeeOnAccessAuthorization)()
                            For Each oEmployeesRow As DataRow In oEmployeesDT.Rows
                                Dim oAuthorizedEmployee As New RoboticsExternAccess.roEmployeeOnAccessAuthorization()
                                oAuthorizedEmployee.Id = oEmployeesRow("Value")
                                tbEmployees.Add(oAuthorizedEmployee)
                            Next
                            oTerminalConfiguration.AuthorizedEmployees = tbEmployees.ToArray
                        End If

                        strSQL = "@SELECT# DayOf, BeginTime, EndTime from TerminalsSyncTimeZonesData  as tzd where BeginTime IS NOT NULL AND ENDTIME IS NOT NULL AND DAYOF IS NOT NULL AND TerminalId = " & terminalID

                        ' Cargamos lista de períodos de acceso
                        Dim oAccessPeriodDT As DataTable = DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(strSQL)
                        If oAccessPeriodDT IsNot Nothing AndAlso oAccessPeriodDT.Rows.Count > 0 Then
                            Dim tbAccessPeriods As List(Of roAccessPeriod) = New List(Of roAccessPeriod)()
                            For Each oAccessPeriodRow As DataRow In oAccessPeriodDT.Rows
                                Dim oAccessPeriod As New RoboticsExternAccess.roAccessPeriod
                                oAccessPeriod.BeginTime = roTypes.Any2DateTime(oAccessPeriodRow("BeginTime")).ToShortTimeString()
                                oAccessPeriod.EndTime = roTypes.Any2DateTime(oAccessPeriodRow("EndTime")).ToShortTimeString()
                                oAccessPeriod.DayOf = roTypes.Any2Integer(oAccessPeriodRow("DayOf"))
                                tbAccessPeriods.Add(oAccessPeriod)
                            Next
                            oTerminalConfiguration.AccessPeriods = tbAccessPeriods.ToArray
                        End If
                        bolRet = True
                    Else
                        returnCode = Core.DTOs.ReturnCode._NotCompatibleTerminal
                        oTerminalConfiguration = Nothing
                    End If
                Else
                    returnCode = Core.DTOs.ReturnCode._TerminalNotExist
                    oTerminalConfiguration = Nothing
                End If
            Catch ex As Exception
                returnCode = Core.DTOs.ReturnCode._UnknownError
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetTerminalConfigurationById")
                oTerminalConfiguration = Nothing
            End Try

            Return bolRet
        End Function

    End Class


End Namespace