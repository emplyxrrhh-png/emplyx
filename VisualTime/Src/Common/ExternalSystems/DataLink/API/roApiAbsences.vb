Imports Robotics.DataLayer
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Security.Base
Imports Robotics.Base.VTSelectorManager

Namespace DataLink

    Public Class roApiAbsences
        Inherits roDataLinkApi

        Protected ReadOnly Property ImportEngine As roCalendarImport
            Get
                Return CType(Me.oDataImport, roCalendarImport)
            End Get
        End Property

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roCalendarImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub


        Public Function CreateOrUpdateAbsence(ByVal oAbsenceData As RoboticsExternAccess.IDatalinkAbsence, ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                bolRet = oAbsenceData.GetEmployeeColumnsDefinition(ColumnsVal, ColumnsPos)

                If bolRet Then
                    Dim tbCauses As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, Export from causes")

                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration) = "" Then
                        ' Graba una ausencia programada
                        Dim oAbsenceStateResult As New Absence.roProgrammedAbsenceState
                        bolRet = Me.ImportEngine.ProcessProgrammedAbsence(tbCauses, ColumnsVal, strErrorMsg, 1, oAbsenceStateResult)
                        Me.State.Result = Me.ImportEngine.State.Result

                        If Not bolRet Then
                            If oAbsenceStateResult.Result = ProgrammedAbsencesResultEnum.NoError Then
                                Select Case Me.State.Result
                                    Case DataLinkResultEnum.InvalidEmployee
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidEmployee
                                    Case DataLinkResultEnum.InvalidCause
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidCause
                                    Case DataLinkResultEnum.FormatColumnIsWrong
                                        iReturnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                                    Case DataLinkResultEnum.InvalidData
                                        iReturnCode = Core.DTOs.ReturnCode._AbenceNotFound
                                    Case Else
                                        iReturnCode = Core.DTOs.ReturnCode._UnknownError
                                End Select
                            Else
                                Select Case oAbsenceStateResult.Result
                                    Case ProgrammedAbsencesResultEnum.AnotherExistInDateInterval, ProgrammedAbsencesResultEnum.AnotherHolidayExistInDate,
                                         ProgrammedAbsencesResultEnum.AnotherOvertimeExistInDate
                                        iReturnCode = Core.DTOs.ReturnCode._AbsenceOverlapping
                                    Case ProgrammedAbsencesResultEnum.InFreezeDate
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidCloseDate
                                    Case ProgrammedAbsencesResultEnum.DateOutOfContract
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidContract
                                    Case ProgrammedAbsencesResultEnum.InvalidDateInterval
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidPeriod
                                    Case ProgrammedAbsencesResultEnum.NotAllowedCause
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidCause
                                    Case Else
                                        iReturnCode = Core.DTOs.ReturnCode._UnknownError
                                End Select
                            End If
                        Else
                            iReturnCode = Core.DTOs.ReturnCode._OK
                        End If
                    Else
                        ' Graba una ausencia por horas
                        Dim oAbsenceStateResult As New Incidence.roProgrammedCauseState
                        bolRet = Me.ImportEngine.ProcessProgrammedCause(tbCauses, ColumnsVal, strErrorMsg, 1, oAbsenceStateResult)
                        Me.State.Result = Me.ImportEngine.State.Result

                        If Not bolRet Then
                            If oAbsenceStateResult.Result = ProgrammedAbsencesResultEnum.NoError Then
                                Select Case Me.State.Result
                                    Case DataLinkResultEnum.InvalidEmployee
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidEmployee
                                    Case DataLinkResultEnum.InvalidCause
                                        iReturnCode = Core.DTOs.ReturnCode._InvalidCause
                                    Case DataLinkResultEnum.FormatColumnIsWrong
                                        iReturnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                                    Case DataLinkResultEnum.InvalidData
                                        iReturnCode = Core.DTOs.ReturnCode._AbenceNotFound
                                    Case Else
                                        iReturnCode = Core.DTOs.ReturnCode._UnknownError
                                End Select
                            Else

                                Select Case oAbsenceStateResult.Result
                                    Case ProgrammedCausesResultEnum.AnotherAbsenceExistInDate, ProgrammedCausesResultEnum.AnotherHolidayExistInDate,
                                     ProgrammedCausesResultEnum.AnotherOvertimeinDate, ProgrammedCausesResultEnum.AnotherExistInDate
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._AbsenceOverlapping
                                    Case ProgrammedCausesResultEnum.InFreezeDate
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidCloseDate
                                    Case ProgrammedCausesResultEnum.DateOutOfContract
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidContract
                                    Case ProgrammedCausesResultEnum.InvalidDate, ProgrammedCausesResultEnum.InvalidDateTimeInterval, ProgrammedCausesResultEnum.InvalidDuration
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPeriod
                                    Case ProgrammedCausesResultEnum.NotAllowedCause
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidCause
                                    Case Else
                                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                                End Select
                            End If
                        Else
                            iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                        End If
                    End If

                    If bolRet Then
                        Dim oContext As New roCollection
                        oContext.Add("Employee.ID", -1)
                        oContext.Add("Date", Now.Date)
                        Extensions.roConnector.InitTask(TasksType.MOVES, oContext)
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid employee object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateEmployee")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function GetAbsences(ByVal oAbsenceCriteria As RoboticsExternAccess.IDatalinkAbsenceCriteria, ByRef lAbsences As Generic.List(Of RoboticsExternAccess.roDatalinkStandarAbsence), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False
            Dim bForAllEmployees As Boolean = False

            Try
                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                lAbsences = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarAbsence)
                bolRet = oAbsenceCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

                If bolRet Then

                    bForAllEmployees = (roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

                    Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RoboticsExternAccess.AbsencesCriteriaAsciiColumns.ImportPrimaryKey, RoboticsExternAccess.AbsencesCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                    If idEmployee > 0 OrElse bForAllEmployees Then

                        ' Obtenemos las justificaciones que debemos exportar
                        Dim tbCauses As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, isnull(Export,'0') as ExportKey from causes where isnull(Export,'0') <> '0' ")

                        Dim beginDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesCriteriaAsciiColumns.BeginPeriod))
                        Dim endDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesCriteriaAsciiColumns.EndPeriod))

                        ' Obtenemos las previsiones de ausencia por dia del empleado en el periodo indicado
                        Dim aState As New Absence.roProgrammedAbsenceState
                        Dim absencesDt As DataTable
                        If Not roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.AbsencesCriteriaAsciiColumns.ShowChangesInPeriod)) Then
                            absencesDt = Absence.roProgrammedAbsence.GetProgrammedAbsences(idEmployee, beginDate, endDate, aState)
                        Else
                            absencesDt = Absence.roProgrammedAbsence.GetProgrammedAbsencesByTimeStamp(idEmployee, beginDate, endDate, aState)
                        End If

                        If absencesDt IsNot Nothing AndAlso absencesDt.Rows.Count > 0 Then
                            Dim rw() As DataRow = Nothing
                            Dim sCauseExportKey As String = String.Empty
                            Dim sCauseShortName As String = String.Empty
                            Dim sCauseName As String = String.Empty
                            Dim oDatalinkStandarAbsence As roDatalinkStandarAbsence
                            For Each oRow As DataRow In absencesDt.Rows
                                sCauseExportKey = String.Empty
                                sCauseShortName = String.Empty
                                sCauseName = String.Empty
                                If Any2String(oRow("Action")) <> "D" Then
                                    rw = tbCauses.Select("ID=" & oRow("IDCause"))
                                    If rw.Length > 0 Then
                                        sCauseExportKey = Any2String(rw(0)("ExportKey"))
                                        sCauseShortName = Any2String(rw(0)("ShortName"))
                                        sCauseName = Any2String(rw(0)("Name"))
                                    End If
                                End If
                                If sCauseExportKey.Length > 0 OrElse Any2String(oRow("Action")) = "D" Then
                                    oDatalinkStandarAbsence = New roDatalinkStandarAbsence
                                    oDatalinkStandarAbsence.AbsenceId = Any2String(oRow("AbsenceId"))
                                    oDatalinkStandarAbsence.Action = Any2String(oRow("Action"))
                                    If Not IsDBNull(oRow("Timestamp")) Then
                                        oDatalinkStandarAbsence.TimeStamp = oRow("Timestamp")
                                    End If
                                    If Not bForAllEmployees Then
                                        oDatalinkStandarAbsence.NifEmpleado = ColumnsVal(AbsencesCriteriaAsciiColumns.NIF)
                                        oDatalinkStandarAbsence.UniqueEmployeeID = ColumnsVal(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)
                                    Else
                                        oDatalinkStandarAbsence.NifEmpleado = Any2String(oRow("NIF"))
                                        oDatalinkStandarAbsence.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                    End If
                                    oDatalinkStandarAbsence.CauseExportKey = sCauseExportKey
                                    oDatalinkStandarAbsence.CauseShortName = sCauseShortName
                                    oDatalinkStandarAbsence.CauseName = sCauseName

                                    If Not IsDBNull(oRow("BeginDate")) Then
                                        oDatalinkStandarAbsence.StartAbsenceDate = oRow("BeginDate")
                                    End If
                                    If Not IsDBNull(oRow("FinishDate")) Then
                                        oDatalinkStandarAbsence.EndAbsenceDate = oRow("FinishDate")
                                    End If
                                    oDatalinkStandarAbsence.MaxDays = Any2Integer(oRow("MaxLastingDays"))

                                    ' Añadimos la prevision de ausencia por dia a la lista
                                    lAbsences.Add(oDatalinkStandarAbsence)
                                End If
                            Next
                        End If

                        ' Obtenemos las previsiones de ausencia por horas del empleado en el periodo indicado
                        Dim oProgCauseState As New Incidence.roProgrammedCauseState()

                        Dim dtProgCauses As DataTable = New DataTable
                        If Not roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.AbsencesCriteriaAsciiColumns.ShowChangesInPeriod)) Then
                            dtProgCauses = Incidence.roProgrammedCause.GetProgrammedCauses(idEmployee, beginDate, endDate, oProgCauseState)
                        Else
                            dtProgCauses = Incidence.roProgrammedCause.GetProgrammedCausesByTimestamp(idEmployee, beginDate, endDate, oProgCauseState)
                        End If

                        If dtProgCauses IsNot Nothing AndAlso dtProgCauses.Rows.Count > 0 Then
                            Dim rw() As DataRow = Nothing
                            Dim sCauseExportKey As String = String.Empty
                            Dim sCauseShortName As String = String.Empty
                            Dim sCauseName As String = String.Empty
                            Dim oDatalinkStandarAbsence As roDatalinkStandarAbsence
                            For Each oRow As DataRow In dtProgCauses.Rows
                                sCauseExportKey = String.Empty
                                sCauseShortName = String.Empty
                                If Any2String(oRow("Action")) <> "D" Then
                                    rw = tbCauses.Select("ID=" & oRow("IDCause"))
                                    If rw.Length > 0 Then
                                        sCauseExportKey = Any2String(rw(0)("ExportKey"))
                                        sCauseShortName = Any2String(rw(0)("ShortName"))
                                        sCauseName = Any2String(rw(0)("Name"))
                                    End If
                                End If
                                If sCauseExportKey.Length > 0 OrElse Any2String(oRow("Action")) = "D" Then
                                    oDatalinkStandarAbsence = New roDatalinkStandarAbsence
                                    oDatalinkStandarAbsence.AbsenceId = Any2String(oRow("AbsenceId"))
                                    oDatalinkStandarAbsence.Action = Any2String(oRow("Action"))
                                    oDatalinkStandarAbsence.TimeStamp = Any2DateTime(oRow("Timestamp"))
                                    If Not bForAllEmployees Then
                                        oDatalinkStandarAbsence.NifEmpleado = ColumnsVal(AbsencesCriteriaAsciiColumns.NIF)
                                        oDatalinkStandarAbsence.UniqueEmployeeID = ColumnsVal(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)
                                    Else
                                        oDatalinkStandarAbsence.NifEmpleado = Any2String(oRow("NIF"))
                                        oDatalinkStandarAbsence.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                    End If

                                    oDatalinkStandarAbsence.CauseExportKey = sCauseExportKey
                                    oDatalinkStandarAbsence.CauseShortName = sCauseShortName
                                    oDatalinkStandarAbsence.CauseName = sCauseName

                                    If Not IsDBNull(oRow("Date")) Then
                                        oDatalinkStandarAbsence.StartAbsenceDate = oRow("Date")
                                    End If

                                    If Not IsDBNull(oRow("FinishDate")) Then
                                        oDatalinkStandarAbsence.EndAbsenceDate = oRow("FinishDate")
                                    End If

                                    If Not IsDBNull(oRow("BeginTime")) Then
                                        oDatalinkStandarAbsence.BeginHour = oRow("BeginTime")
                                    End If

                                    If Not IsDBNull(oRow("EndTime")) Then
                                        oDatalinkStandarAbsence.EndHour = oRow("EndTime")
                                    End If

                                    If Not IsDBNull(oRow("duration")) Then
                                        oDatalinkStandarAbsence.Duration = Any2DateTime(roConversions.ConvertHoursToTime(oRow("duration")))
                                    End If
                                    ' Añadimos la prevision de ausencia por horas a la lista
                                    lAbsences.Add(oDatalinkStandarAbsence)
                                End If
                            Next
                        End If

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        bolRet = False
                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid absence object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetAbsences")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

    End Class

End Namespace