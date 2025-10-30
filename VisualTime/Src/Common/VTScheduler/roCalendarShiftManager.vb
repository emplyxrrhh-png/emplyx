Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTCalendar

    Public Class roCalendarShiftManager

        Private oState As roCalendarShiftState = Nothing

        Public Sub New()
            Me.oState = New roCalendarShiftState()
        End Sub

        Public Sub New(ByVal _State As roCalendarShiftState)
            Me.oState = _State
        End Sub

#Region "Helper function"

        Public Function GetShiftDefinition(ByVal intIDShift As Integer, Optional ForceLoadRigidDataLayers As Boolean = False) As roCalendarShift

            Dim oCalendarShift As roCalendarShift = Nothing

            oState.UpdateStateInfo()

            Dim bolRet As Boolean = True

            Try

                Dim oShiftState As New roShiftState
                roBusinessState.CopyTo(oState, oShiftState)

                Dim strSQL As String = "@SELECT# Shifts.ID, Shifts.Name as Name, Shifts.Color, Shifts.ShortName, Shifts.ShiftType, Shifts.IsFloating, " &
                                        "Shifts.StartFloating, 0 as ShiftLayers, 0 as AllowComplementary1, 0 as AllowComplementary2,0 as AllowModifyIniHour1, 0 as AllowModifyIniHour2 , 0 as AllowModifyDuration1, " &
                                        "0 as AllowModifyDuration2,0 as IDLayer1, 0 as IDLayer2, Shifts.StartLimit, Shifts.EndLimit, " &
                                        "Shifts.ExpectedWorkingHours, Shifts.AreWorkingDays, isnull(Shifts.BreakHours,0) as BreakHours , Shifts.AllowComplementary, Shifts.AllowFloatingData, (@SELECT# count(*) from ShiftAssignments where ShiftAssignments.IDShift = Shifts.ID) as Assignments, Shifts.AdvancedParameters "

                strSQL &= " FROM Shifts  "

                If intIDShift <> -1 Then
                    strSQL &= " WHERE (Shifts.ID = " & intIDShift & ") AND (ISNULL(Shifts.TypeShift, '') = '') "
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, "Shifts")

                If tb IsNot Nothing AndAlso tb.Rows IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    oCalendarShift = New roCalendarShift()

                    For Each oRow As DataRow In tb.Rows
                        ' Para cada horario , revisamos si tenemos que añadir datos adicionales
                        If roTypes.Any2Boolean(oRow("AllowComplementary")) Or roTypes.Any2Boolean(oRow("AllowFloatingData")) Or ForceLoadRigidDataLayers Then
                            Dim intCountLayers As Integer = 0

                            Dim oShift As New roShift()
                            oShift.ID = oRow("ID")
                            oShift.Load()
                            If oShift IsNot Nothing AndAlso oShift.Layers IsNot Nothing Then
                                Dim oBreakLayers As New List(Of roBreakLayerDefinition)
                                For Each oLayer As roShiftLayer In oShift.Layers
                                    ' Para cada franja rigida del horario
                                    Dim bolExistData As Boolean = False

                                    If oLayer.LayerType = roLayerTypes.roLTMandatory And ForceLoadRigidDataLayers Then bolExistData = True
                                    If oLayer.LayerType = roLayerTypes.roLTMandatory And roTypes.Any2Boolean(oRow("AllowComplementary")) Then bolExistData = True
                                    If oLayer.LayerType = roLayerTypes.roLTMandatory And oLayer.Data.Exists("AllowModifyIniHour") Then bolExistData = True
                                    If oLayer.LayerType = roLayerTypes.roLTMandatory And oLayer.Data.Exists("AllowModifyDuration") Then bolExistData = True

                                    If bolExistData Then
                                        intCountLayers += 1
                                        If intCountLayers = 1 Then
                                            oCalendarShift.IDLayer1 = oLayer.ID
                                            If oLayer.Data.Exists("AllowModifyIniHour") Then oCalendarShift.AllowModifyIniHour1 = True
                                            If oLayer.Data.Exists("AllowModifyDuration") Then oCalendarShift.AllowModifyDuration1 = True
                                            If roTypes.Any2Boolean(oRow("AllowComplementary")) Then oCalendarShift.AllowComplementary1 = True
                                            If oLayer.Data.Exists("Begin") Then oCalendarShift.StartLayer1 = roTypes.Any2DateTime(oLayer.Data("Begin"))
                                            If oLayer.Data.Exists("Finish") Then oCalendarShift.EndLayer1 = roTypes.Any2DateTime(oLayer.Data("Finish"))
                                            If Not oLayer.Data.Exists("FloatingFinishMinutes") Then oCalendarShift.HasLayer1FixedEnd = True
                                        Else
                                            oCalendarShift.IDLayer2 = oLayer.ID
                                            If oLayer.Data.Exists("AllowModifyIniHour") Then oCalendarShift.AllowModifyIniHour2 = True
                                            If oLayer.Data.Exists("AllowModifyDuration") Then oCalendarShift.AllowModifyDuration2 = True
                                            If roTypes.Any2Boolean(oRow("AllowComplementary")) Then oCalendarShift.AllowComplementary2 = True
                                            If oLayer.Data.Exists("Begin") Then oCalendarShift.StartLayer2 = roTypes.Any2DateTime(oLayer.Data("Begin"))
                                            If oLayer.Data.Exists("Finish") Then oCalendarShift.EndLayer2 = roTypes.Any2DateTime(oLayer.Data("Finish"))
                                            If Not oLayer.Data.Exists("FloatingFinishMinutes") Then oCalendarShift.HasLayer2FixedEnd = True
                                        End If
                                    End If

                                    If oLayer.LayerType = roLayerTypes.roLTBreak Then
                                        oBreakLayers.Add(New roBreakLayerDefinition() With {
                                            .Start = roTypes.Any2DateTime(oLayer.Data("Begin")),
                                            .Finish = roTypes.Any2DateTime(oLayer.Data("Finish"))
                                                        })
                                    End If

                                Next

                                oCalendarShift.BreakLayers = oBreakLayers.ToArray
                            End If
                            oCalendarShift.CountLayers = intCountLayers
                        Else
                            oCalendarShift.CountLayers = 0
                            oCalendarShift.IDLayer1 = 0
                            oCalendarShift.IDLayer2 = 0
                            oCalendarShift.AllowComplementary1 = False
                            oCalendarShift.AllowComplementary2 = False
                            oCalendarShift.AllowModifyIniHour1 = False
                            oCalendarShift.AllowModifyIniHour2 = False
                            oCalendarShift.AllowModifyDuration1 = False
                            oCalendarShift.AllowModifyDuration2 = False
                            oCalendarShift.BreakLayers = {}
                        End If

                        oCalendarShift.IDShift = intIDShift
                        oCalendarShift.Name = roTypes.Any2String(oRow("Name"))
                        oCalendarShift.Color = roCalendarRowPeriodDataManager.HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRow("Color"))))
                        oCalendarShift.ShortName = roTypes.Any2String(oRow("ShortName"))
                        oCalendarShift.AllowFloating = roTypes.Any2Boolean(oRow("AllowFloatingData"))
                        oCalendarShift.AllowComplementary = roTypes.Any2Boolean(oRow("AllowComplementary"))
                        oCalendarShift.WorkingHours = roTypes.Any2Time(roTypes.Any2Double(oRow("ExpectedWorkingHours"))).Minutes
                        oCalendarShift.IsFloating = roTypes.Any2Boolean(oRow("IsFloating"))
                        oCalendarShift.BreakHours = roTypes.Any2Time(roTypes.Any2Double(oRow("BreakHours"))).Minutes
                        oCalendarShift.AdvancedParameters = roCalendarRowPeriodDataManager.LoadShiftAdvancedParatemers(roTypes.Any2String(oRow("AdvancedParameters")), New roCalendarRowPeriodDataState(oState.IDPassport))

                        If oCalendarShift.IsFloating Then
                            oCalendarShift.Type = ShiftTypeEnum.NormalFloating
                            If Not IsDBNull(oRow("StartFloating")) Then oCalendarShift.StartFloating = roTypes.Any2DateTime(oRow("StartFloating"))
                        Else
                            If roTypes.Any2Integer(oRow("ShiftType")) = 1 Then
                                oCalendarShift.Type = ShiftTypeEnum.Normal
                                If Not IsDBNull(oRow("StartLimit")) Then oCalendarShift.StartFloating = roTypes.Any2DateTime(oRow("StartLimit"))
                                If Not IsDBNull(oRow("EndLimit")) Then oCalendarShift.EndFloating = roTypes.Any2DateTime(oRow("EndLimit"))
                            Else
                                oCalendarShift.Type = IIf(roTypes.Any2Boolean(oRow("AreWorkingDays")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                            End If
                        End If

                        If roTypes.Any2Double(oRow("Assignments")) > 0 Then
                            ' Obtenemos la informacion de los puestos asignados al horario
                            Dim lstAssignments As Generic.List(Of roShiftAssignment) = roShiftAssignment.GetShiftAssignments(oRow("ID"), oShiftState, False)
                            If Not lstAssignments Is Nothing AndAlso lstAssignments.Count > 0 Then
                                Dim _Assignments As New List(Of roCalendarShiftAssignmentData)
                                For Each oShiftAssignment As roShiftAssignment In lstAssignments
                                    Dim oCalendarShiftAss As New roCalendarShiftAssignmentData
                                    oCalendarShiftAss.Cover = oShiftAssignment.Coverage
                                    oCalendarShiftAss.IDAssig = oShiftAssignment.IDAssignment
                                    _Assignments.Add(oCalendarShiftAss)
                                Next
                                oCalendarShift.Assignments = _Assignments.ToArray
                            End If
                        End If

                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarShiftManager::GetShiftDefinition")
                oCalendarShift = Nothing
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarShiftManager::GetShiftDefinition")
                oCalendarShift = Nothing
                bolRet = False
            Finally

            End Try

            Return oCalendarShift

        End Function

#End Region

    End Class

End Namespace