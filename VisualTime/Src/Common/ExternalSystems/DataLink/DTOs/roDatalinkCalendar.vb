Imports System.Runtime.Serialization
Imports Robotics.VTBase

Namespace DataLink.RoboticsExternAccess

    <DataContract>
    Public Class roDatalinkStandarCalendar
        Implements IDatalinkCalendar
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property ShiftKey As String 'Codigo de equivalencia del horario
        <DataMember>
        Public Property PlanDate As Date ' Fecha planificada

        <DataMember>
        Public Property ShiftLayerDefinition As roShiftLayerDefinition() ' Fecha planificada
        <DataMember>
        Public Property CanTelecommute As Boolean ' Puede teletrabajar
        <DataMember>
        Public Property TelecommutingStatus As TelecommutingType_Enum ' Estado de teletrabajo
        <DataMember>
        Public Property TelecommuteForced As Boolean ' Estado forzado por supervisor

        Public Function GetEmployeeColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkCalendar.GetEmployeeColumnsDefinition
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(CalendarAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(CalendarAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(CalendarAsciiColumns.NIF) = CInt(CalendarAsciiColumns.NIF)
                ColumnsVal(CInt(CalendarAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(CalendarAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(CalendarAsciiColumns.ImportPrimaryKey) = CInt(CalendarAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(CalendarAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(CalendarAsciiColumns.NIF_Letter) = CInt(CalendarAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(CalendarAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(CalendarAsciiColumns.ShiftKey) = CInt(CalendarAsciiColumns.ShiftKey)
                ColumnsVal(CInt(CalendarAsciiColumns.ShiftKey)) = ShiftKey

                ColumnsPos(CalendarAsciiColumns.PlanDate) = CInt(CalendarAsciiColumns.PlanDate)
                ColumnsVal(CInt(CalendarAsciiColumns.PlanDate)) = PlanDate.ToString("yyyy/MM/dd")

                ColumnsPos(CalendarAsciiColumns.Layer1StartTime) = CInt(CalendarAsciiColumns.Layer1StartTime)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer1StartTime)) = String.Empty

                ColumnsPos(CalendarAsciiColumns.Layer1EndTime) = CInt(CalendarAsciiColumns.Layer1EndTime)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer1EndTime)) = String.Empty

                ColumnsPos(CalendarAsciiColumns.Layer1OrdinaryHours) = CInt(CalendarAsciiColumns.Layer1OrdinaryHours)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer1OrdinaryHours)) = "0"

                ColumnsPos(CalendarAsciiColumns.Layer1ComplementaryHours) = CInt(CalendarAsciiColumns.Layer1ComplementaryHours)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer1ComplementaryHours)) = "0"

                ColumnsPos(CalendarAsciiColumns.Layer2StartTime) = CInt(CalendarAsciiColumns.Layer2StartTime)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer2StartTime)) = String.Empty

                ColumnsPos(CalendarAsciiColumns.Layer2EndTime) = CInt(CalendarAsciiColumns.Layer2EndTime)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer2EndTime)) = String.Empty

                ColumnsPos(CalendarAsciiColumns.Layer2OrdinaryHours) = CInt(CalendarAsciiColumns.Layer2OrdinaryHours)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer2OrdinaryHours)) = "0"

                ColumnsPos(CalendarAsciiColumns.Layer2ComplementaryHours) = CInt(CalendarAsciiColumns.Layer2ComplementaryHours)
                ColumnsVal(CInt(CalendarAsciiColumns.Layer2ComplementaryHours)) = "0"

                Dim iIndex As Integer = 0
                If ShiftLayerDefinition IsNot Nothing Then
                    For Each oLayer As roShiftLayerDefinition In ShiftLayerDefinition
                        If oLayer.StartTime = String.Empty OrElse oLayer.OrdinaryHours = String.Empty Then Exit For

                        Dim oStartDate As String = "1899/12/30 " & oLayer.StartTime
                        Dim oEndDate As String = String.Empty

                        If oLayer.StartDay = roDayInfo.DayAfter Then
                            oStartDate = "1899/12/31 " & oLayer.StartTime
                        ElseIf oLayer.StartDay = roDayInfo.DayBefore Then
                            oStartDate = "1899/12/29 " & oLayer.StartTime
                        End If

                        Dim oOrdinayHours As Integer = "0"
                        If oLayer.OrdinaryHours <> String.Empty Then
                            oOrdinayHours = roTypes.Any2Time("0001/01/01 " & oLayer.OrdinaryHours).Minutes.ToString
                        End If

                        Dim oComplentaryHours As String = "0"
                        If oLayer.ComplemntaryHours <> String.Empty Then
                            oComplentaryHours = roTypes.Any2Time("0001/01/01 " & oLayer.ComplemntaryHours).Minutes.ToString
                        End If

                        oEndDate = roTypes.Any2Time(oStartDate).Add(oOrdinayHours + oComplentaryHours, "N").Value

                        If iIndex = 0 Then
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer1StartTime)) = oStartDate
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer1EndTime)) = oEndDate
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer1OrdinaryHours)) = oOrdinayHours
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer1ComplementaryHours)) = oComplentaryHours
                        Else
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer2StartTime)) = oStartDate
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer2EndTime)) = oEndDate
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer2OrdinaryHours)) = oOrdinayHours
                            ColumnsVal(CInt(CalendarAsciiColumns.Layer2ComplementaryHours)) = oComplentaryHours
                        End If

                        iIndex += 1
                    Next
                End If

                ColumnsPos(CalendarAsciiColumns.CanTelecommute) = CInt(CalendarAsciiColumns.CanTelecommute)
                ColumnsVal(CInt(CalendarAsciiColumns.CanTelecommute)) = IIf(CanTelecommute, "1", "0")

                ColumnsPos(CalendarAsciiColumns.TelecommutingStatus) = CInt(CalendarAsciiColumns.TelecommutingStatus)
                ColumnsVal(CInt(CalendarAsciiColumns.TelecommutingStatus)) = TelecommutingStatus

                ColumnsPos(CalendarAsciiColumns.TelecommuteForced) = CInt(CalendarAsciiColumns.TelecommuteForced)
                ColumnsVal(CInt(CalendarAsciiColumns.TelecommuteForced)) = IIf(TelecommuteForced, "1", "0")

                bolRet = True

                If ColumnsVal(CalendarAsciiColumns.NIF) = "" AndAlso ColumnsVal(CalendarAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(CalendarAsciiColumns.ShiftKey) = "" Then bolRet = False
                If ColumnsVal(CalendarAsciiColumns.PlanDate) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

End Namespace