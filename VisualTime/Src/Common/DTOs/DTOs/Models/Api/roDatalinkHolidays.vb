Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum HolidaysAsciiColumns
        Action
        NIF
        NIF_Letter
        ShiftKey
        PlanDate
        ImportPrimaryKey
    End Enum

    Public Interface IDatalinkHolidays

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandarHolidays
        Implements IDatalinkHolidays
        <DataMember>
        Public Property Action As String 'Action
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property ShiftKey As String 'Codigo de equivalencia del horario
        <DataMember>
        Public Property PlanDate As Date 'FECHA INICIO

        Public Function GetEmployeeColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkHolidays.GetEmployeeColumnsDefinition
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(HolidaysAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(HolidaysAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(HolidaysAsciiColumns.Action) = CInt(HolidaysAsciiColumns.Action)
                ColumnsVal(CInt(HolidaysAsciiColumns.Action)) = If(Action = String.Empty, "I", Action)

                ColumnsPos(HolidaysAsciiColumns.NIF) = CInt(HolidaysAsciiColumns.NIF)
                ColumnsVal(CInt(HolidaysAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(HolidaysAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(HolidaysAsciiColumns.ImportPrimaryKey) = CInt(HolidaysAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(HolidaysAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(HolidaysAsciiColumns.NIF_Letter) = CInt(HolidaysAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(HolidaysAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(HolidaysAsciiColumns.ShiftKey) = CInt(HolidaysAsciiColumns.ShiftKey)
                ColumnsVal(CInt(HolidaysAsciiColumns.ShiftKey)) = ShiftKey

                ColumnsPos(HolidaysAsciiColumns.PlanDate) = CInt(HolidaysAsciiColumns.PlanDate)
                ColumnsVal(CInt(HolidaysAsciiColumns.PlanDate)) = PlanDate.ToString("yyyy/MM/dd")

                bolRet = True

                If ColumnsVal(HolidaysAsciiColumns.NIF) = "" AndAlso ColumnsVal(HolidaysAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(HolidaysAsciiColumns.ShiftKey) = "" Then bolRet = False
                If ColumnsVal(HolidaysAsciiColumns.PlanDate) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

End Namespace