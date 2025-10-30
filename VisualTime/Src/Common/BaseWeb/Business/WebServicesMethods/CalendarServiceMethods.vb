Imports System.Reflection
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class CalendarServiceMethods

#Region "SecurityNode"

        Public Shared Function GetCalendarCoverage(ByVal oPage As System.Web.UI.Page, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strEmployeeFilter As String, ByVal strAssignmentFilter As String) As roCalendarCoverageDay()

            Dim oRet As Robotics.Base.DTOs.roCalendarCoverageDay() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarPeriodCoverageState

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObj As roCalendarCoverageDaysResponse = VTLiveApi.CalendarMethods.GetCalendarCoverage(xFirstDay, xLastDay, strEmployeeFilter, oState, strAssignmentFilter)
                oRet = oTmpObj.CalendarCoverageDays

                oSession.States.CalendarPeriodCoverageState = oTmpObj.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CalendarPeriodCoverageState.Result <> CalendarPeriodCoverageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarPeriodCoverageState)
                End If
            Catch ex As Exception
                oRet = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-068")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCalendar(ByVal oPage As System.Web.UI.Page, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strEmployeeFilter As String, ByVal typeView As CalendarView, ByVal calendarDetail As CalendarDetailLevel, ByVal bLoadChilds As Boolean, ByVal strAssignmentFilter As String, ByVal bolShiftData As Boolean, ByVal bolShowIndictments As Boolean, ByVal bolShowPunches As Boolean, ByVal bLoadSeatingCapacity As Boolean) As roCalendar

            Dim oRet As roCalendar = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObj As roCalendarResponse = VTLiveApi.CalendarMethods.GetCalenar(xFirstDay, xLastDay, strEmployeeFilter, typeView, calendarDetail, bLoadChilds, oState, strAssignmentFilter, bolShiftData, bolShowIndictments, bolShowPunches, bLoadSeatingCapacity)
                oRet = oTmpObj.Calendar

                oSession.States.CalendarV2State = oTmpObj.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendar
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-069")
            End Try

            Return oRet

        End Function

        Public Shared Function AddIndictmentsToCalendar(ByVal oPage As System.Web.UI.Page, ByVal oCalendar As roCalendar) As roCalendarIndictmentResult

            Dim oRet As roCalendarIndictmentResult = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObj As roCalendarIndictmentResultResponse = VTLiveApi.CalendarMethods.AddIndictmentsToCalendar(oCalendar, oState)
                oRet = oTmpObj.CalendarIndictment

                oSession.States.CalendarV2State = oTmpObj.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendarIndictmentResult
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-070")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveCalendar(ByVal oPage As System.Web.UI.Page, ByVal oCalendar As roCalendar) As roCalendarResult

            Dim oRet As roCalendarResult = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObj As roCalendarResponse = VTLiveApi.CalendarMethods.SaveCalenar(oCalendar, oState)
                oRet = oTmpObj.CalendarResult

                oSession.States.CalendarV2State = oTmpObj.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendarResult
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-071")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCalendarDayData(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal IdGroup As Integer, ByVal xDate As DateTime, ByVal iTypeView As Integer, detailLevel As CalendarDetailLevel, ByVal bLoadPunches As Boolean, Optional bLoadSeatingCapacity As Boolean = False) As roCalendarRowDayData

            Dim oRet As roCalendarRowDayData = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObject As roCalendarRowDayDataResponse = VTLiveApi.CalendarMethods.GetCalendarDayData(IDEmployee, IdGroup, xDate, iTypeView, detailLevel, bLoadPunches, oState, bLoadSeatingCapacity)
                oRet = oTmpObject.CalendarDay

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendarRowDayData
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-072")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCalendarDayHourData(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal IdGroup As Integer, ByVal IdShift As Integer, ByVal xStartFloating As Date, ByVal xDate As DateTime, ByVal calendarDetail As CalendarDetailLevel, ByVal oCalendarRowShiftData As roCalendarRowShiftData) As roCalendarRowHourData()

            Dim oRet As roCalendarRowHourData() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObject As roCalendarRowHourDataResponse = VTLiveApi.CalendarMethods.GetCalendarDayHourData(IDEmployee, IdGroup, xDate, IdShift, xStartFloating, calendarDetail, oCalendarRowShiftData, oState)
                oRet = oTmpObject.CalendarRowHourData

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-073")
            End Try

            Return oRet

        End Function

        Public Shared Function ExportCalendarToExcel(ByVal oPage As System.Web.UI.Page, ByVal oCalendar As roCalendar) As Byte()

            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObject As roCalendarFile = VTLiveApi.CalendarMethods.ExportCalendarToExcel(oCalendar, oState)
                oRet = oTmpObject.XlsByteArray

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-074")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCalendarFromExcel(ByVal oPage As System.Web.UI.Page, ByVal excelFile As Byte(), ByVal strEmployees As String, ByVal dStartDate As Date, ByVal dEndDate As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyHolidays As Boolean, ByVal bolCopyAlternatives As Boolean,
                                         ByVal bolKeepHolidays As Boolean, ByVal bolKeepLockedDay As Boolean, ByRef strFileNameError As String, ByRef oCalResult As roCalendarResult, ByVal strAssignments As String, ByVal bLoadChilds As Boolean, ByVal bExcelType As Boolean) As roCalendar

            Dim oRet As roCalendar = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObject As roExcelCalendar = VTLiveApi.CalendarMethods.GetCalendarFromExcel(excelFile, strEmployees, dStartDate, dEndDate, bolCopyMainShifts, bolCopyHolidays, False, bolKeepHolidays, bolKeepLockedDay, oState, strAssignments, bLoadChilds, bExcelType)
                oRet = oTmpObject.Calendar
                oCalResult = oTmpObject.CalendarResult
                strFileNameError = oTmpObject.FileNameError

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendar

                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-075")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftDefinition(ByVal oPage As System.Web.UI.Page, ByVal idShift As Integer) As roCalendarShift

            Dim oRet As roCalendarShift = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarShiftV2State

            WebServiceHelper.SetState(oState)

            Try

                Dim oTmpObject As roCalendarShiftResponse = VTLiveApi.CalendarMethods.GetShiftDefinition(idShift, oState)
                oRet = oTmpObject.CalendarShift

                oSession.States.CalendarShiftV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession

                If oRet Is Nothing OrElse oSession.States.CalendarShiftV2State.Result <> CalendarShiftResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarShiftV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendarShift
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-076")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCalendarConfiguration(ByVal oPage As System.Web.UI.Page) As roCalendarPassportConfig
            Dim oRet As roCalendarPassportConfig = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State
            WebServiceHelper.SetState(oState)
            Try
                Dim oTmpObject As roCalendarPassportConfigResponse = VTLiveApi.CalendarMethods.GetCalendarConfig(oState)
                oRet = oTmpObject.CalendarPassportConfig

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession
                If oRet Is Nothing OrElse oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = New roCalendarPassportConfig
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-077")
            End Try
            Return oRet
        End Function

        Public Shared Function SaveCalendarConfiguration(ByVal oPage As System.Web.UI.Page, ByVal oRemarksConfig As roCalendarPassportConfig) As Boolean
            Dim oRet As Boolean = True
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State
            WebServiceHelper.SetState(oState)
            Try
                Dim oTmpObject As roStandarResponse = VTLiveApi.CalendarMethods.SaveCalendarConfig(oRemarksConfig, oState)
                oRet = oTmpObject.Result

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = False
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-078")
            End Try
            Return oRet
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function HasAlternativeShifts(ByVal oPage As System.Web.UI.Page) As Boolean
            Dim oRet As Boolean = True
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State
            WebServiceHelper.SetState(oState)
            Try
                Dim oTmpObject As roStandarResponse = VTLiveApi.CalendarMethods.HasAlternativeShifts(oState)
                oRet = oTmpObject.Result

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = False
                Dim translator = New roLanguageWeb()
                Dim oTmpState As New roWsState With {
                .Result = 1,
                .ErrorText = translator.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable", WLHelperWeb.CurrentLanguage) + MethodBase.GetCurrentMethod().Name
            }
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-020")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteAlternativeShifts(ByVal oPage As System.Web.UI.Page) As Boolean
            Dim oRet As Boolean = True
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CalendarV2State
            WebServiceHelper.SetState(oState)
            Try
                Dim oTmpObject As roStandarResponse = VTLiveApi.CalendarMethods.DeleteAlternativeShifts(oState)
                oRet = oTmpObject.Result

                oSession.States.CalendarV2State = oTmpObject.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.CalendarV2State.Result <> CalendarV2ResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CalendarV2State)
                End If
            Catch ex As Exception
                oRet = False
                Dim translator = New roLanguageWeb()
                Dim oTmpState As New roWsState With {
                .Result = 1,
                .ErrorText = translator.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable", WLHelperWeb.CurrentLanguage) + MethodBase.GetCurrentMethod().Name
            }
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-020")
            End Try
            Return oRet
        End Function

    End Class

End Namespace