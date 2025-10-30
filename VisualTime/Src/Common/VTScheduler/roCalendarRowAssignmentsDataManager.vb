Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.VTBase

Namespace VTCalendar

    Public Class roCalendarRowAssignmentsDataManager
        Private oState As roCalendarRowAssignmentsDataState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowAssignmentsDataState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowAssignmentsDataState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Shared Function LoadAssignments(ByVal intIDEmployee As Integer, ByRef _State As roCalendarRowAssignmentsDataState) As roCalendarAssignmentData()
            '
            ' Obtenemos los puestos que puede cubrir el empleado
            '

            Dim oRet As New Generic.List(Of roCalendarAssignmentData)

            Dim bolRet As Boolean = False

            Try

                Dim oStateEmployeeAssignments As New Employee.roEmployeeState()
                Dim oAssignments As DataTable = Employee.roEmployeeAssignment.GetEmployeeAssignmentsDataTable(intIDEmployee, oStateEmployeeAssignments)

                If oAssignments IsNot Nothing AndAlso oAssignments.Rows.Count > 0 Then
                    Dim oRows As DataRow() = oAssignments.Select("", "ShortName ASC")
                    If Not oRows Is Nothing Then
                        For Each orow As DataRow In oRows
                            Dim oCalendarAssignmentData As New roCalendarAssignmentData
                            oCalendarAssignmentData.ID = roTypes.Any2Double(orow("IDAssignment"))
                            oCalendarAssignmentData.Suit = roTypes.Any2Double(orow("Suitability"))
                            oCalendarAssignmentData.Name = roTypes.Any2String(orow("Name"))
                            oCalendarAssignmentData.ShortName = roTypes.Any2String(orow("ShortName"))
                            oCalendarAssignmentData.Color = roCalendarRowPeriodDataManager.HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(orow("Color"))))
                            oRet.Add(oCalendarAssignmentData)
                        Next
                    End If
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roCalendarRowAssignmentsDataManager::Load")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowAssignmentsDataManager::Load")
            Finally

            End Try

            Return oRet.ToArray

        End Function

#End Region

    End Class

End Namespace