Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum AccrualsAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> AccrualExportKey
        <EnumMember> AccrualShortName
        <EnumMember> AccrualDate
        <EnumMember> AccrualValue
        <EnumMember> ImportPrimaryKey
    End Enum

    Public Enum AccrualsCriteriaAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> BeginPeriod
        <EnumMember> EndPeriod
        <EnumMember> ImportPrimaryKey
        <EnumMember> AtDate
    End Enum

    Public Enum TaskAccrualsCriteriaAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> BeginPeriod
        <EnumMember> EndPeriod
        <EnumMember> ImportPrimaryKey
        <EnumMember> Project
        <EnumMember> Task
    End Enum

    Public Interface IDatalinkAccrualCriteria

        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    Public Interface IDatalinkTaskAccrualCriteria

        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandarAccrual
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property AccrualExportKey As String ' Codigo exportacion
        <DataMember>
        Public Property AccrualShortName As String 'Nombre corto
        <DataMember>
        Public Property AccrualDate As Date 'Fecha del acumulado
        <DataMember>
        Public Property AccrualValue As Double 'Valor del acumulado

    End Class

    <DataContract>
    Public Class roDatalinkStandarAccrualCriteria
        Implements IDatalinkAccrualCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property StartAccrualPeriod As Date 'FECHA INICIO PERIODO
        <DataMember>
        Public Property EndAccrualPeriod As Date 'FECHA FINAL PERIODO

        <DataMember>
        Public Property AtDateAccrual As Date 'Obener valor dels saldo a la fecha indicada

        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkAccrualCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(AccrualsCriteriaAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(AccrualsCriteriaAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(AccrualsCriteriaAsciiColumns.NIF) = CInt(AccrualsCriteriaAsciiColumns.NIF)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(AccrualsCriteriaAsciiColumns.ImportPrimaryKey) = CInt(AccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(AccrualsCriteriaAsciiColumns.NIF_Letter) = CInt(AccrualsCriteriaAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(AccrualsCriteriaAsciiColumns.BeginPeriod) = CInt(AccrualsCriteriaAsciiColumns.BeginPeriod)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.BeginPeriod)) = StartAccrualPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(AccrualsCriteriaAsciiColumns.EndPeriod) = CInt(AccrualsCriteriaAsciiColumns.EndPeriod)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.EndPeriod)) = EndAccrualPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(AccrualsCriteriaAsciiColumns.AtDate) = CInt(AccrualsCriteriaAsciiColumns.AtDate)
                ColumnsVal(CInt(AccrualsCriteriaAsciiColumns.AtDate)) = AtDateAccrual.ToString("yyyy/MM/dd")

                bolRet = True

                ' Ya no es obligatorio filtrar por empleado
                'If ColumnsVal(AccrualsCriteriaAsciiColumns.NIF) = "" AndAlso ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(AccrualsCriteriaAsciiColumns.BeginPeriod) = "" Then bolRet = False
                If ColumnsVal(AccrualsCriteriaAsciiColumns.EndPeriod) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roDatalinkStandarAccrualResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Accruals As Generic.List(Of roDatalinkStandarAccrual)
    End Class

    <DataContract>
    Public Class roDatalinkStandarTaskAccrual
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property Project As String ' Proyecto
        <DataMember>
        Public Property Task As String 'Tarea
        <DataMember>
        Public Property AccrualDate As Date 'Fecha del acumulado
        <DataMember>
        Public Property AccrualValue As Double 'Valor del acumulado

    End Class

    <DataContract>
    Public Class roDatalinkStandarTaskAccrualResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Accruals As Generic.List(Of roDatalinkStandarTaskAccrual)
    End Class

    <DataContract>
    Public Class roDatalinkStandarTaskAccrualCriteria
        Implements IDatalinkTaskAccrualCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property StartAccrualPeriod As Date 'FECHA INICIO PERIODO
        <DataMember>
        Public Property EndAccrualPeriod As Date 'FECHA FINAL PERIODO

        <DataMember>
        Public Property Proyecto As String 'proyecto

        <DataMember>
        Public Property Tarea As String 'Tarea

        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkTaskAccrualCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(TaskAccrualsCriteriaAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(TaskAccrualsCriteriaAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.NIF) = CInt(TaskAccrualsCriteriaAsciiColumns.NIF)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey) = CInt(TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.NIF_Letter) = CInt(TaskAccrualsCriteriaAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.BeginPeriod) = CInt(TaskAccrualsCriteriaAsciiColumns.BeginPeriod)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.BeginPeriod)) = StartAccrualPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.EndPeriod) = CInt(TaskAccrualsCriteriaAsciiColumns.EndPeriod)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.EndPeriod)) = EndAccrualPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.Project) = CInt(TaskAccrualsCriteriaAsciiColumns.Project)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.Project)) = Proyecto

                ColumnsPos(TaskAccrualsCriteriaAsciiColumns.Task) = CInt(TaskAccrualsCriteriaAsciiColumns.Task)
                ColumnsVal(CInt(TaskAccrualsCriteriaAsciiColumns.Task)) = Tarea

                bolRet = True

                ' Ya no es obligatorio filtrar por empleado
                'If ColumnsVal(AccrualsCriteriaAsciiColumns.NIF) = "" AndAlso ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey) = "" Then bolRet = False
                If ColumnsVal(TaskAccrualsCriteriaAsciiColumns.BeginPeriod) = "" Then bolRet = False
                If ColumnsVal(TaskAccrualsCriteriaAsciiColumns.EndPeriod) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class


End Namespace