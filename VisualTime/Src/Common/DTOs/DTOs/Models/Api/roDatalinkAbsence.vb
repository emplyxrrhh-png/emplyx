Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum AbsencesAsciiColumns
        <EnumMember> Action
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> ShortCause
        <EnumMember> BeginDate
        <EnumMember> EndDate
        <EnumMember> BeginHour
        <EnumMember> EndHour
        <EnumMember> Duration
        <EnumMember> MaxDays
        <EnumMember> ImportPrimaryKey
        <EnumMember> ExportCause
    End Enum

    Public Enum AbsencesCriteriaAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> BeginPeriod
        <EnumMember> EndPeriod
        <EnumMember> ImportPrimaryKey
        <EnumMember> ShowChangesInPeriod
    End Enum

    Public Interface IDatalinkAbsence

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    Public Interface IDatalinkAbsenceCriteria

        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandarAbsence
        Implements IDatalinkAbsence
        <DataMember>
        Public Property Action As String 'Action
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property CauseShortName As String 'NOMBRE CORT
        <DataMember>
        Public Property StartAbsenceDate As Date 'FECHA INICIO
        <DataMember>
        Public Property EndAbsenceDate As Nullable(Of Date) 'FECHA FINAL
        <DataMember>
        Public Property MaxDays As Nullable(Of Integer) 'MAXIMOS DIAS

        <DataMember>
        Public Property BeginHour As Nullable(Of Date) 'HORA INICIO
        <DataMember>
        Public Property EndHour As Nullable(Of Date) 'HORA FINAL

        <DataMember>
        Public Property Duration As Nullable(Of Date) 'DURACION

        <DataMember>
        Public Property CauseExportKey As String ' Codigo exportacion

        <DataMember>
        Public Property TimeStamp As Date 'FECHA INICIO

        <DataMember>
        Public Property AbsenceId As String

        <DataMember>
        Public Property CauseName As String ' Nombre justificacion

        Public Function GetEmployeeColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkAbsence.GetEmployeeColumnsDefinition
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(AbsencesAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(AbsencesAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(AbsencesAsciiColumns.Action) = CInt(AbsencesAsciiColumns.Action)
                ColumnsVal(CInt(AbsencesAsciiColumns.Action)) = If(Action = String.Empty, "I", Action)

                ColumnsPos(AbsencesAsciiColumns.NIF) = CInt(AbsencesAsciiColumns.NIF)
                ColumnsVal(CInt(AbsencesAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(AbsencesAsciiColumns.ImportPrimaryKey) = CInt(AbsencesAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(AbsencesAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(AbsencesAsciiColumns.NIF_Letter) = CInt(AbsencesAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(AbsencesAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(AbsencesAsciiColumns.ShortCause) = CInt(AbsencesAsciiColumns.ShortCause)
                ColumnsVal(CInt(AbsencesAsciiColumns.ShortCause)) = CauseShortName

                ColumnsPos(AbsencesAsciiColumns.ExportCause) = CInt(AbsencesAsciiColumns.ExportCause)
                ColumnsVal(CInt(AbsencesAsciiColumns.ExportCause)) = CauseExportKey

                ColumnsPos(AbsencesAsciiColumns.BeginDate) = CInt(AbsencesAsciiColumns.BeginDate)
                ColumnsVal(CInt(AbsencesAsciiColumns.BeginDate)) = StartAbsenceDate.ToString("yyyy/MM/dd")

                ColumnsPos(AbsencesAsciiColumns.EndDate) = CInt(AbsencesAsciiColumns.EndDate)
                If EndAbsenceDate.HasValue AndAlso EndAbsenceDate.Value <> Date.MinValue Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.EndDate)) = EndAbsenceDate.Value.ToString("yyyy/MM/dd")
                Else
                    ColumnsVal(CInt(AbsencesAsciiColumns.EndDate)) = String.Empty
                End If

                ColumnsPos(AbsencesAsciiColumns.Duration) = CInt(AbsencesAsciiColumns.Duration)
                If Duration.HasValue AndAlso Duration.Value <> Date.MinValue Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.Duration)) = Duration.Value.ToString("HH:mm")
                Else
                    ColumnsVal(CInt(AbsencesAsciiColumns.Duration)) = String.Empty
                End If

                ColumnsPos(AbsencesAsciiColumns.BeginHour) = CInt(AbsencesAsciiColumns.BeginHour)
                If BeginHour.HasValue AndAlso BeginHour.Value <> Date.MinValue Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.BeginHour)) = BeginHour.Value.ToString("HH:mm")
                Else
                    ColumnsVal(CInt(AbsencesAsciiColumns.BeginHour)) = String.Empty
                End If

                ColumnsPos(AbsencesAsciiColumns.EndHour) = CInt(AbsencesAsciiColumns.EndHour)
                If EndHour.HasValue AndAlso EndHour.Value <> Date.MinValue Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.EndHour)) = EndHour.Value.ToString("HH:mm")
                Else
                    ColumnsVal(CInt(AbsencesAsciiColumns.EndHour)) = String.Empty
                End If

                ColumnsPos(AbsencesAsciiColumns.MaxDays) = CInt(AbsencesAsciiColumns.MaxDays)
                If MaxDays.HasValue Then
                    ColumnsVal(CInt(AbsencesAsciiColumns.MaxDays)) = MaxDays
                Else
                    ColumnsVal(CInt(AbsencesAsciiColumns.MaxDays)) = If(ColumnsVal(CInt(AbsencesAsciiColumns.EndDate)) = String.Empty, 60, (EndAbsenceDate.Value - StartAbsenceDate).TotalDays)
                End If

                bolRet = True

                If ColumnsVal(AbsencesAsciiColumns.NIF) = "" AndAlso ColumnsVal(AbsencesAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(AbsencesAsciiColumns.ShortCause) = "" AndAlso ColumnsVal(AbsencesAsciiColumns.ExportCause) = "" Then bolRet = False
                If ColumnsVal(AbsencesAsciiColumns.BeginDate) = "" Then bolRet = False

                If ColumnsVal(AbsencesAsciiColumns.Duration) <> "" Or ColumnsVal(AbsencesAsciiColumns.BeginHour) <> "" Or ColumnsVal(AbsencesAsciiColumns.EndHour) <> "" Then
                    ' Ausencia por horas
                    If ColumnsVal(AbsencesAsciiColumns.BeginHour) = "" Then bolRet = False
                    If ColumnsVal(AbsencesAsciiColumns.EndHour) = "" Then bolRet = False
                    If ColumnsVal(AbsencesAsciiColumns.Duration) = "" Then bolRet = False

                    ' No puede ser por dias y horas a la vez
                    If ColumnsVal(AbsencesAsciiColumns.EndDate) <> "" And ColumnsVal(AbsencesAsciiColumns.BeginDate) <> ColumnsVal(AbsencesAsciiColumns.EndDate) Then bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roDatalinkStandarAbsenceCriteria
        Implements IDatalinkAbsenceCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property StartAbsencePeriod As Date 'FECHA INICIO PERIODO
        <DataMember>
        Public Property EndAbsencePeriod As Date 'FECHA FINAL PERIODO
        <DataMember>
        Public Property ShowChangesInPeriod As Boolean 'Devuelve los cambios realizados en el periodo

        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkAbsenceCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(AbsencesCriteriaAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(AbsencesCriteriaAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(AbsencesCriteriaAsciiColumns.NIF) = CInt(AbsencesCriteriaAsciiColumns.NIF)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.NIF)) = NifEmpleado

                ColumnsPos(AbsencesCriteriaAsciiColumns.ShowChangesInPeriod) = CInt(AbsencesCriteriaAsciiColumns.ShowChangesInPeriod)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.ShowChangesInPeriod)) = IIf(ShowChangesInPeriod, "1", "0")

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(AbsencesCriteriaAsciiColumns.ImportPrimaryKey) = CInt(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(AbsencesCriteriaAsciiColumns.NIF_Letter) = CInt(AbsencesCriteriaAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(AbsencesCriteriaAsciiColumns.BeginPeriod) = CInt(AbsencesCriteriaAsciiColumns.BeginPeriod)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.BeginPeriod)) = StartAbsencePeriod.ToString("yyyy/MM/dd")

                ColumnsPos(AbsencesCriteriaAsciiColumns.EndPeriod) = CInt(AbsencesCriteriaAsciiColumns.EndPeriod)
                ColumnsVal(CInt(AbsencesCriteriaAsciiColumns.EndPeriod)) = EndAbsencePeriod.ToString("yyyy/MM/dd")

                bolRet = True

                'No es obligatorio identificar al empleado
                'If ColumnsVal(AbsencesCriteriaAsciiColumns.NIF) = "" AndAlso ColumnsVal(AbsencesCriteriaAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(AbsencesCriteriaAsciiColumns.BeginPeriod) = "" Then bolRet = False
                If ColumnsVal(AbsencesCriteriaAsciiColumns.EndPeriod) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roDatalinkStandarAbsenceResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Absences As Generic.List(Of roDatalinkStandarAbsence)
    End Class

End Namespace