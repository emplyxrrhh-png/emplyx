Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.Security.Base

Namespace VTPortal

    Public Class CostCenterHelper

        Public Shared Function SaveCostCenterPunch(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo, ByVal costCenterId As Integer, ByVal latitude As Double, ByVal longitude As Double, ByVal identifier As String,
                                                   ByVal locationZone As String, ByVal fullAddress As String, ByVal punchImage As System.Drawing.Image, ByVal reliable As Boolean, Optional punchDate As Nullable(Of DateTime) = Nothing, Optional isApp As Boolean = False, Optional comments As String = Nothing, Optional notReliableCause As String = Nothing) As StdResponse

            Dim bSaved As New StdResponse

            Try
                Dim oReqState As New Requests.roRequestState(-1)
                If oPassport IsNot Nothing Then oReqState = New Requests.roRequestState(oPassport.ID)

                Dim oPermList As New PermissionList
                If oPassport IsNot Nothing Then oPermList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPassport Is Nothing OrElse oPermList.Punch.CostCenterPunch Then
                    bSaved.Result = True
                    bSaved.Status = ErrorCodes.OK

                    Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)

                    Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState())
                    Dim dServerDateTime As Date = DateTime.Now
                    Dim oTerminal As Terminal.roTerminal = Nothing

                    If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                        dServerDateTime = oTerminals(0).GetCurrentDateTime()
                        oTerminal = oTerminals(0)
                    End If

                    'Recuperamos los valores de estado del empleado desde el servidor (hora, etc.)
                    Dim serverDatetime As DateTime = VTPortal.StatusHelper.GetCurrentTerminalDatetime(oTerminal, timeZone)

                    If punchDate.HasValue Then
                        serverDatetime = punchDate
                    End If

                    Dim oPunch As Punch.roPunch = Nothing
                    Dim oPunchStatus As PunchStatus

                    bSaved.Result = Punch.roPunch.DoCostCenterPunch(oEmployee.ID, serverDatetime, oTerminal.ID, costCenterId, punchImage, oPunch, oPunchStatus, reliable, New Punch.roPunchState(),
                                                       latitude, longitude, locationZone, fullAddress, timeZone.Id, isApp, comments, notReliableCause)
                Else
                    bSaved.Result = False
                    bSaved.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                bSaved.Result = False
                bSaved.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CostCenterHelper::SaveCostCenterPunch")
            End Try

            Return bSaved

        End Function

    End Class

End Namespace