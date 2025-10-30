Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTRequests
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class LeavesHelper

        Public Shared Function GetEmployeeLeaves(ByVal oPassportTicket As roPassportTicket, ByVal showAll As Boolean, ByVal dateStart As String, ByVal dateEnd As String, ByRef oAbsState As Absence.roProgrammedAbsenceState) As LeavesList
            Dim lrret As New LeavesList
            Try
                lrret.Status = ErrorCodes.OK

                Dim dtAbsences As DataTable = Absence.roProgrammedAbsence.GetLeaves(oPassportTicket.IDEmployee, showAll, dateStart, dateEnd, oAbsState)

                If dtAbsences IsNot Nothing AndAlso dtAbsences.Rows.Count > 0 Then
                    Dim oTmpList As New Generic.List(Of Leave)

                    Dim oDocState As New roDocumentState()
                    roBusinessState.CopyTo(oAbsState, oDocState)
                    Dim oDocumentManager As New roDocumentManager(oDocState)

                    For Each oRow As DataRow In dtAbsences.Rows
                        Dim oTmpLeave As New Leave

                        With oTmpLeave
                            .BeginDate = roTypes.Any2DateTime(oRow("BeginDate"))
                            .FinishDate = roTypes.Any2DateTime(oRow("RealFinishDate"))
                            .IdCause = roTypes.Any2Integer(oRow("IDCause"))
                            .IdEmployee = roTypes.Any2Integer(oRow("IDEmployee"))
                            .Description = roTypes.Any2String(oRow("Description"))
                            .Cause = roTypes.Any2String(oRow("CauseName"))
                            .AbsenceID = roTypes.Any2Integer(oRow("AbsenceID"))
                            'Obtengo los documentos faltantes para empleados
                            .DocAlerts = oDocumentManager.GetForecastDocumentationFaultAlerts(.IdEmployee, DocumentType.Employee, .AbsenceID, ForecastType.Leave, False)
                        End With

                        oTmpList.Add(oTmpLeave)
                    Next

                    lrret.Leaves = oTmpList.ToArray
                Else
                    lrret.Leaves = {}
                End If
            Catch ex As Exception
                lrret.Leaves = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::LeavesHelper::GetEmployeeLeaves")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveLeave(ByVal oPassportTicket As roPassportTicket, ByVal fromDate As String, ByVal toDate As String, ByVal idCause As Integer, ByVal idTemplateDocument As Integer, ByVal oFile As Web.HttpPostedFile, ByVal requestObs As String, ByRef oReqState As Requests.roRequestState) As StdResponse
            Dim lrret As New StdResponse
            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = True

                Dim oAbsenceState As New Absence.roProgrammedAbsenceState
                roBusinessState.CopyTo(oReqState, oAbsenceState)

                Dim oCauseState As New Cause.roCauseState
                roBusinessState.CopyTo(oReqState, oCauseState)

                Dim oCause As New Cause.roCause(idCause, oCauseState)

                If idCause <= 0 Then
                    lrret.Status = ErrorCodes.REQUEST_ERROR_CauseRequired
                    lrret.Result = False
                Else
                    Dim dFromDate As Date = DateTime.ParseExact(fromDate, "yyyy-MM-dd", Nothing)
                    Dim oAbsence As New Absence.roProgrammedAbsence(oPassportTicket.IDEmployee, dFromDate, oAbsenceState)
                    Dim differenceBetweenLeaveAndNextAbsence As Integer = -1
                    Dim lastValidEndDate As Date

                    If oAbsence.Load(False) Then
                        If oCause.StartsProgrammedAbsence AndAlso oCause.MaxProgrammedAbsence > 0 Then
                            oAbsence.MaxLastingDays = oCause.MaxProgrammedAbsence
                        Else
                            oAbsence.MaxLastingDays = 100
                        End If

                        lastValidEndDate = Absence.roProgrammedAbsence.GetLastValidEnddate(oPassportTicket.IDEmployee, dFromDate, oAbsence.MaxLastingDays, oAbsenceState)
                        If lastValidEndDate <> Nothing Then
                            differenceBetweenLeaveAndNextAbsence = (roTypes.Any2Integer(DateDiff(DateInterval.Day, roTypes.Any2DateTime(oAbsence.BeginDate), lastValidEndDate)) + 1)
                        End If

                        If differenceBetweenLeaveAndNextAbsence <> -1 AndAlso differenceBetweenLeaveAndNextAbsence < oAbsence.MaxLastingDays Then
                            oAbsence.MaxLastingDays = differenceBetweenLeaveAndNextAbsence
                        End If

                        oAbsence.IDCause = idCause
                        oAbsence.Description = requestObs
                        lrret.Result = oAbsence.Save()
                        If Not lrret.Result Then
                            lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesError

                            Select Case oAbsence.State.Result
                                Case ProgrammedAbsencesResultEnum.InvalidDateInterval
                                    lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                                Case ProgrammedAbsencesResultEnum.AnotherExistInDateInterval, ProgrammedAbsencesResultEnum.AnotherOvertimeExistInDate, ProgrammedAbsencesResultEnum.AnotherHolidayExistInDate
                                    lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesOverlapped
                            End Select
                        Else
                            If idTemplateDocument > 0 AndAlso oFile IsNot Nothing Then
                                Dim oDocState As New VTDocuments.roDocumentState(-1)
                                roBusinessState.CopyTo(oReqState, oDocState)

                                lrret = DocumentsHelper.UploadDocument(oPassportTicket, oFile, requestObs, idTemplateDocument, oAbsence.IdAbsence, "", ForecastType.Leave, oDocState)
                            End If
                        End If
                    Else
                        lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesError
                    End If
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::LeavesHelper::SaveLeave")
            End Try

            Return lrret
        End Function

        Public Shared Function UploadLeaveFinishDate(ByVal idProgrammedAbcense As Integer, ByVal endDate As DateTime, ByRef oReqState As Requests.roRequestState) As StdResponse
            Dim lrret As New StdResponse
            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = True

                'Recupero la ausencia
                Dim oAbsenceState As New Absence.roProgrammedAbsenceState
                roBusinessState.CopyTo(oReqState, oAbsenceState)

                Dim oAbsence As New Absence.roProgrammedAbsence(idProgrammedAbcense, oAbsenceState)

                If oAbsence.Load(False) Then
                    oAbsence.FinishDate = endDate
                    lrret.Result = oAbsence.Save()
                    If Not lrret.Result Then
                        lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesError

                        Select Case oAbsence.State.Result
                            Case ProgrammedAbsencesResultEnum.InvalidDateInterval
                                lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                            Case ProgrammedAbsencesResultEnum.AnotherExistInDateInterval, ProgrammedAbsencesResultEnum.AnotherOvertimeExistInDate, ProgrammedAbsencesResultEnum.AnotherHolidayExistInDate
                                lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesOverlapped
                        End Select
                    End If
                Else
                    lrret.Status = ErrorCodes.REQUEST_ERROR_PlannedAbsencesError
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::LeavesHelper::UploadLeaveFinishDate")
            End Try

            Return lrret
        End Function

    End Class

End Namespace