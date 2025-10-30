Imports Microsoft.QualityTools.Testing.Fakes
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTHolidays
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase.Extensions

Public Class CalendarHelper

    Public Sub New()
    End Sub

    Public Function LoadCellsByCalendar(ByVal idMainShift As Integer, idShiftBase As Integer, mainShiftIsHoliday As Boolean, Optional layerStartTime As Date? = Nothing) As DataTable

        Base.VTCalendar.Fakes.ShimroCalendarRowPeriodDataManager.LoadCellsByCalendarDateTimeDateTimeInt32Int32Int32roParametersCalendarViewCalendarDetailLevelroConceptroConceptroConceptroCalendarRowPeriodDataStateRefBooleanHashtableRefDateTimeDateTimeBooleanDataTableDataTableRefroEmployeeStateroContractStateroProgrammed =
                        Function(ByVal _FirstDay As DateTime, _LastDay As DateTime, _IDEmployee As Integer, _IDGroup As Integer, oPermission As Integer,
                                 ByVal oParameters As roParameters, ByVal _typeView As CalendarView, ByVal _detailLevel As CalendarDetailLevel, ByVal oConceptNormalWork As Concept.roConcept, ByVal oConceptAbsence As Concept.roConcept,
                                                   ByVal oConceptOverWorking As Concept.roConcept, ByRef _State As roCalendarRowPeriodDataState, ByVal bolLicenseHRScheduling As Boolean,
                            ByRef oShiftCache As Hashtable, ByVal _BeginGroupDate As DateTime, ByVal _EndGroupDate As DateTime, ByVal _IsEmployeePortal As Boolean,
                                             ByVal tbCauses As DataTable, ByRef tbProgrammedAbsences As DataTable, ByVal oEmployeeState As Employee.roEmployeeState, ByVal oContractState As Contract.roContractState, ByVal oProgrammedAbsState As Absence.roProgrammedAbsenceState, ByVal oProgrammedOvertimesState As roProgrammedOvertimeState,
                                            ByVal oProgrammedHolidaysState As roProgrammedHolidayState, ByVal oProgrammedOvertimeyManager As roProgrammedOvertimeManager, ByVal oProgrammedHolidayManager As roProgrammedHolidayManager,
                                 oSchedulerRemarks As Scheduler.roSchedulerRemarks, bLoadSeatingCapacity As Boolean, bLoadAlerts As Boolean, ByVal sVTEdition As String)
                            Dim calendarInfo As roCalendarRowPeriodData = New roCalendarRowPeriodData
                            Dim dayData As roCalendarRowDayData() = New roCalendarRowDayData(0) {}
                            Dim dayDataInfo = New roCalendarRowDayData
                            dayDataInfo.PlanDate = New Date(2025, 1, 1)
                            Dim mainShift As roCalendarRowShiftData = New roCalendarRowShiftData()
                            mainShift.ID = idMainShift
                            If (mainShiftIsHoliday) Then
                                mainShift.Type = ShiftTypeEnum.Holiday_Working
                            Else
                                mainShift.Type = ShiftTypeEnum.Normal
                            End If
                            dayDataInfo.MainShift = mainShift
                            Dim baseShift As roCalendarRowShiftData = New roCalendarRowShiftData()
                            baseShift.ID = idShiftBase
                            baseShift.StartHour = New DateTime(2025, 1, 1, 10, 0, 0)
                            baseShift.EndHour = New DateTime(2025, 1, 1, 18, 0, 0)
                            Dim layersDefinitionArray As roCalendarShiftLayersDefinition() = New roCalendarShiftLayersDefinition(1) {}
                            Dim layersDefinition As roCalendarShiftLayersDefinition = New roCalendarShiftLayersDefinition()
                            layersDefinition.LayerDuration = 8
                            layersDefinition.LayerID = 1
                            layersDefinition.LayerOrdinaryHours = "8"
                            layersDefinition.LayerComplementaryHours = "1"
                            layersDefinition.ExistLayerDuration = True
                            layersDefinition.ExistLayerStartTime = False
                            If layerStartTime IsNot Nothing Then
                                layersDefinition.LayerStartTime = layerStartTime
                            End If
                            layersDefinitionArray(0) = layersDefinition
                            layersDefinitionArray(1) = layersDefinition
                            baseShift.ShiftLayersDefinition = layersDefinitionArray
                            dayDataInfo.ShiftBase = baseShift
                            dayDataInfo.CanTelecommute = False
                            dayData(0) = dayDataInfo
                            calendarInfo.DayData = dayData
                            Return calendarInfo
                        End Function
    End Function

End Class
