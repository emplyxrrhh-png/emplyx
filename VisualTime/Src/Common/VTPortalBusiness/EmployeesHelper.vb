Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Web.Hosting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTServiceApi
Imports Robotics.Base.VTSurveys
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class EmployeesHelper

        Public Shared Function GetSupervisedEmployees(ByVal oPassport As roPassportTicket, ByVal oEmpState As Employee.roEmployeeState) As EmployeeList
            Dim lrret As New EmployeeList

            Try
                lrret.Status = ErrorCodes.OK
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

                Dim empsList As New Generic.List(Of EmployeeInfo)

                Dim dtEmployees As DataTable = Common.roBusinessSupport.GetEmployees("CurrentEmployee=1", "Calendar", "U", oEmpState, False)

                If dtEmployees IsNot Nothing AndAlso dtEmployees.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtEmployees.Rows
                        empsList.Add(New EmployeeInfo With {
                            .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                            .Name = roTypes.Any2String(oRow("EmployeeName")),
                            .Group = roTypes.Any2String(oRow("GroupName"))
                            })
                    Next
                End If

                lrret.Employees = empsList.ToArray()
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Employees = {}

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetSupervisedEmployees")
            End Try

            Return lrret
        End Function

        Public Shared Function CurrentEmployeesZoneStatus(ByVal currentZone As Integer, ByVal oEmpState As Employee.roEmployeeState) As EmployeeList
            ' Devuelve los datos del empleado con el código pasado por parámetro
            Dim lrret As New EmployeeList

            Try
                lrret.Status = ErrorCodes.OK
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

                Try
                    lrret.Description = New Zone.roZone(currentZone, New Zone.roZoneState).Name
                Catch ex As Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::CurrentEmployeesZoneStatus::MissingDescription")
                End Try

                Dim empsList As New Generic.List(Of EmployeeInfo)

                Dim dtEmployees As DataTable = Common.roBusinessSupport.GetCurrentEmployeesZoneStatus("IDZone=" & currentZone.ToString, oEmpState, True)

                If dtEmployees IsNot Nothing AndAlso dtEmployees.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtEmployees.Rows
                        'Solo mostramos empleados con fichaje antes de 48 horas.

                        If roTypes.Any2DateTime(oRow("DateTime")).AddDays(2) < DateTime.Today() Then
                        Else
                            empsList.Add(New EmployeeInfo With {
                        .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                        .Name = roTypes.Any2String(oRow("EmployeeName")),
                        .Group = roTypes.Any2String(oRow("IDZone")),
                        .Image = EmployeesHelper.LoadEmployeeImage(oRow("Image"), oEmpState)
                        })
                        End If
                    Next
                End If

                lrret.Employees = empsList.ToArray()
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Employees = {}

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::CurrentEmployeesZoneStatus")
            End Try

            Return lrret

        End Function

        Public Shared Function LoadEmployeeImage(ByVal objImage As Object, ByVal oEmpState As Employee.roEmployeeState) As String
            Dim strImage As String = String.Empty
            Try
                Dim ImageData As Byte()

                If Not IsDBNull(objImage) AndAlso objImage IsNot Nothing Then
                    ImageData = CType(objImage, Byte())
                    ImageData = MakeThumbnail(ImageData, 120, 120)
                    strImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

                End If
            Catch ex As Exception
                strImage = String.Empty
            End Try
            Return strImage
        End Function

        Public Shared Function MakeThumbnail(ByVal myImage As Byte(), ByVal thumbWidth As Integer, ByVal thumbHeight As Integer) As Byte()
            Using ms As MemoryStream = New MemoryStream()

                Using thumbnail As Image = Image.FromStream(New MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, Nothing, New IntPtr())
                    thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                    Return ms.ToArray()
                End Using
            End Using
        End Function

        Public Shared Function GetEmployeesOnWorkCenter(ByVal oPassport As roPassportTicket, idEmployee As Integer, ByVal refDate As Date, ByVal oEmpState As VTCalendar.roCalendarRowPeriodDataState) As EmployeeList
            Dim lrret As New EmployeeList

            Try
                lrret.Status = ErrorCodes.OK
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

                Dim empsList As New Generic.List(Of EmployeeInfo)

                Dim oCalendarState As New VTCalendar.roCalendarState(oPassport.ID)
                Dim dtEmployees As DataTable = VTCalendar.roCalendarManager.LoadEmployeesOnWorkCenter(idEmployee, refDate, oCalendarState)

                If dtEmployees IsNot Nothing AndAlso dtEmployees.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtEmployees.Rows
                        empsList.Add(New EmployeeInfo With {
                            .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                            .Name = roTypes.Any2String(oRow("EmployeeName")),
                            .Group = roTypes.Any2String(oRow("GroupName")),
                            .Image = LoadEmployeeImage(oRow("EmployeeImage"), New Employee.roEmployeeState(oPassport.ID))
                            })
                    Next
                End If

                lrret.Employees = empsList.ToArray()
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Employees = {}

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeesOnWorkCenter")
            End Try

            Return lrret
        End Function

        Public Shared Function CheckTelecommutingChange(ByVal idEmployee As Integer, ByVal dDate As Date, ByVal type As TelecommutingTypeEnum, ByVal impersonating As Boolean, ByVal oState As Employee.roEmployeeState) As DTOs.TelecommutingCheckChangeResult
            Dim eRet As TelecommutingCheckChangeResult = TelecommutingCheckChangeResult._Direct

            Try
                Dim oCalendarState As New VTCalendar.roCalendarState(oState.IDPassport)
                Dim oCalManager As New VTCalendar.roCalendarManager(oCalendarState)

                eRet = oCalManager.CheckTelecommutingChange(idEmployee, dDate, type)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::CheckTelecommutingChange")
            End Try

            Return eRet
        End Function

        Public Shared Function GetEmployeeAvailableChannels(ByVal idEmployee As Integer, ByVal oState As roChannelState) As roGenericResponse(Of roChannel())
            Dim lrret As New roGenericResponse(Of roChannel())
            Try
                Dim oChannelManager As New roChannelManager(oState)

                lrret.Value = oChannelManager.GetAllChannels(idEmployee, False).ToArray
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeAvailableChannels")
                lrret.Value = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeAvailableConversationsByChannel(ByVal idEmployee As Integer, ByVal idChannel As Integer, ByVal oState As roConversationState) As roGenericResponse(Of roConversation())
            Dim lrret As New roGenericResponse(Of roConversation())
            Try
                Dim oConversationManager As New roConversationManager(oState)

                lrret.Value = oConversationManager.GetAllChannelConversations(idChannel, idEmployee).ToArray
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeAvailableConversationsByChannel")
                lrret.Value = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function CreateConversation(ByVal idEmployee As Integer, ByVal idChannel As Integer, ByVal sTitle As String, ByVal sMessage As String, ByVal isAnonymous As Integer, ByVal oState As roConversationState) As roGenericResponse(Of roConversation)
            Dim lrret As New roGenericResponse(Of roConversation)
            Try
                Dim oConversationManager As New roConversationManager(oState)

                Dim oConversation As New roConversation()
                oConversation.CreatedBy = idEmployee
                oConversation.CreatedOn = Now
                oConversation.LastStatusChangeOn = Now
                oConversation.LastMessageTimestamp = Now
                oConversation.Title = sTitle
                oConversation.IsAnonymous = isAnonymous
                Dim oChannelManager = New roChannelManager()
                oConversation.Channel = oChannelManager.GetChannel(idChannel, False, True, idEmployee)

                Dim oMessage As New roMessage()
                oMessage.Body = sMessage
                oMessage.IsAnonymous = isAnonymous
                oMessage.CreatedBy = idEmployee
                oMessage.CreatedOn = Now
                oMessage.Conversation = oConversation

                If (oConversationManager.CreateConversation(oConversation, oMessage, True)) Then
                    lrret.Value = oConversation
                    lrret.Status = ErrorCodes.OK
                Else
                    'No se ha podido crear la conversación
                    lrret.Value = Nothing
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeAvailableConversationsByChannel")
                lrret.Value = Nothing
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeMessagesByConversation(ByVal idEmployee As Integer, ByVal idConversation As Integer, ByVal oState As roMessageState) As roGenericResponse(Of roMessage())
            Dim lrret As New roGenericResponse(Of roMessage())
            Try
                Dim oMessageManager As New roMessageManager(oState)

                lrret.Value = oMessageManager.GetAllConversationMessages(idConversation, idEmployee, False).ToArray
                oMessageManager.SetAllConversationMessagesRead(idConversation, idEmployee)
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeMessagesByConversation")
                lrret.Value = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function CreateMessage(ByVal idEmployee As Integer, ByVal idConversation As Integer, ByVal sMessage As String, ByVal oState As roMessageState) As roGenericResponse(Of roMessage)
            Dim lrret As New roGenericResponse(Of roMessage)
            Try
                Dim oMessageManager As New roMessageManager(oState)

                Dim oConversationManager = New roConversationManager()
                Dim oConversation As roConversation = oConversationManager.GetConversation(idConversation, idEmployee)

                Dim oMessage As New roMessage()
                oMessage.Body = sMessage
                oMessage.IsAnonymous = oConversation.IsAnonymous
                oMessage.CreatedBy = idEmployee
                oMessage.CreatedOn = Now
                oMessage.Conversation = oConversation

                If (oMessageManager.CreateMessage(oMessage, True)) Then
                    lrret.Value = oMessage
                    lrret.Status = ErrorCodes.OK
                Else
                    'No se ha podido crear el mensaje
                    lrret.Value = Nothing
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::CreateMessage")
                lrret.Value = Nothing
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetDEXUrl(ByVal oState As roServiceApiManagerState) As roGenericResponse(Of String)
            Dim lrret As New roGenericResponse(Of String)
            Try
                Dim oApiManager = New roServiceApiManager(oState)

                lrret.Value = oApiManager.GetDEXurl()
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetDEXUrl")
                lrret.Value = Nothing
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function


        Public Shared Function GetEmployeeCommuniques(ByVal idEmployee As Integer) As EmployeeCommuniques
            Dim lrret As New EmployeeCommuniques
            Try

                Dim oParameter As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.VTPortalApiVersion.ToString(), New AdvancedParameter.roAdvancedParameterState)
                Dim ApiVersion = roTypes.Any2Integer(oParameter.Value)

                If ApiVersion > 10 Then

                    Dim oCommuniqueManager As New roCommuniqueManager()
                    lrret.Communiques = oCommuniqueManager.GetAllEmployeeCommuniquesWithStatus(idEmployee, False, False).ToArray

                    For Each oCommunique In lrret.Communiques
                        oCommunique.Communique.Message = oCommunique.Communique.Message.Replace(vbCr, "<br>").Replace(vbLf, "<br>")
                    Next
                End If


                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetDEXUrl")
                lrret.Communiques = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeSurveys(ByVal idEmployee As Integer) As EmployeeSurveys
            Dim lrret As New EmployeeSurveys
            Try

                Dim oParameter As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.VTPortalApiVersion.ToString(), New AdvancedParameter.roAdvancedParameterState)
                Dim ApiVersion = roTypes.Any2Integer(oParameter.Value)

                If ApiVersion > 14 Then
                    Try
                        Dim oSurveyManager As New roSurveyManager()
                        Dim surveys = oSurveyManager.GetAllSurveys(idEmployee, False)

                        If surveys IsNot Nothing AndAlso surveys.Count > 0 Then
                            lrret.Surveys = surveys.ToArray
                        Else
                            lrret.Surveys = {}
                        End If
                    Catch ex As Exception
                        lrret.Surveys = {}
                    End Try
                Else
                    lrret.Surveys = {}
                End If


                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetDEXUrl")
                lrret.Surveys = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeTelecommuting(ByVal idEmployee As Integer) As EmployeeTelecommuting
            Dim lrret As New EmployeeTelecommuting
            Try

                ' Datos de teletrabajo relativos al día de hoy
                Dim bEmployeeHasTelecommuteAgreementOnDate As Boolean = False
                Dim sEmployeeTelecommuteMandatoryDays As String = String.Empty
                Dim sEmployeeTelecommuteOptionalDays As String = String.Empty
                Dim iEmployeeTelecommuteMaxDays As Integer = 0
                Dim iEmployeeTelecommuteMaxPercentage As Integer = 0
                Dim iEmployeeTelecommutePeriodType As Integer = 0

                Employee.roEmployee.GetEmployeeTelecommutingDataOnDate(Now.Date, idEmployee, New Employee.roEmployeeState(-1), bEmployeeHasTelecommuteAgreementOnDate, sEmployeeTelecommuteMandatoryDays, sEmployeeTelecommuteOptionalDays, iEmployeeTelecommuteMaxDays, iEmployeeTelecommuteMaxPercentage, iEmployeeTelecommutePeriodType)

                lrret.Telecommute.Telecommuting = bEmployeeHasTelecommuteAgreementOnDate
                lrret.Telecommute.TelecommutingDays = sEmployeeTelecommuteMandatoryDays.Trim

                lrret.Telecommute.TelecommutingExpected = False
                lrret.Telecommute.WorkCenterName = ""


                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetDEXUrl")
                lrret.Status = ErrorCodes.GENERAL_ERROR

            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeConceptsSummaryHolidays(ByVal idEmployee As Integer, ByVal idShift As Integer, ByVal idPassport As Integer) As roGenericResponse(Of List(Of roHolidayConceptsSummary))
            Dim lrret As New roGenericResponse(Of List(Of roHolidayConceptsSummary))
            Try
                Dim oConceptState As New roConceptState(idPassport)
                lrret.Value = roConcept.GetHolidaysConceptsSummaryByShift(oConceptState, idEmployee, idShift)
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeConceptsSummaryHolidays")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeConceptsDetailHolidays(ByVal idEmployee As Integer, ByVal idShift As Integer) As roGenericResponse(Of List(Of roHolidayConceptsDetail))
            Dim lrret As New roGenericResponse(Of List(Of roHolidayConceptsDetail))
            Try
                Dim oConceptState As New roConceptState()
                Dim tbHolidaysConceptsDetail As DataTable = roConcept.GetHolidaysConceptsSummaryByEmployee(oConceptState, idEmployee, idShift)

                Dim holidayConceptsDetailList As New List(Of roHolidayConceptsDetail)()

                If tbHolidaysConceptsDetail IsNot Nothing AndAlso tbHolidaysConceptsDetail.Rows.Count = 1 Then

                    Dim available As New roHolidayConceptsDetail With {
                        .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("Available")),
                        .DayType = "Available"
            }
                    holidayConceptsDetailList.Add(available)

                    Dim approvalPendingDays As New roHolidayConceptsDetail With {
                        .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("ApprovalPending")),
                        .DayType = "ApprovalPending"
            }
                    holidayConceptsDetailList.Add(approvalPendingDays)

                    Dim approvedNotEnjoyed As New roHolidayConceptsDetail With {
                        .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("ApprovedNotEnjoyed")),
                        .DayType = "ApprovedNotEnjoyed"
            }
                    holidayConceptsDetailList.Add(approvedNotEnjoyed)

                    Dim willExpires As New roHolidayConceptsDetail With {
    .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("WillExpires")),
    .DayType = "WillExpires"
}
                    holidayConceptsDetailList.Add(willExpires)

                    Dim canNotEnjoyYet As New roHolidayConceptsDetail With {
    .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("CanNotEnjoyYet")),
    .DayType = "CanNotEnjoyYet"
}
                    holidayConceptsDetailList.Add(canNotEnjoyYet)

                    Dim expectedAccrual As New roHolidayConceptsDetail With {
    .NumberOfDays = roTypes.Any2Double(tbHolidaysConceptsDetail(0)("ExpectedAccrual")),
    .DayType = "ExpectedAccrual"
}
                    holidayConceptsDetailList.Add(expectedAccrual)

                End If

                lrret.Value = holidayConceptsDetailList
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::EmployeesHelper::GetEmployeeConceptsDetailHolidays")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function


    End Class

End Namespace