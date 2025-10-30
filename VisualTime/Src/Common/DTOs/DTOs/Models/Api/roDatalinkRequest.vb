Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum RequestCriteriaAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> BeginPeriod
        <EnumMember> EndPeriod
        <EnumMember> ImportPrimaryKey
        <EnumMember> Type
    End Enum

    Public Interface IDatalinkRequestCriteria

        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandarRequest
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property RequestType As String ' Tipo 'TODO: Revisar este nombre, en otros sitios está como Type
        <DataMember>
        Public Property RequestDate As DateTime 'Fecha de creación de la solicitud
        <DataMember>
        Public Property Status As String 'Estado de la solicitud
        <DataMember>
        Public Property Date1 As DateTime 'Date1    'TODO: As Nullable(Of XXX) ??
        <DataMember>
        Public Property Date2 As DateTime 'Date2
        <DataMember>
        Public Property IDCause As String ' Identificador justificación (el de exportación, no el interno)
        <DataMember>
        Public Property IDShift As String 'Identificador del horario
        <DataMember>
        Public Property Comments As String 'Comments
        <DataMember>
        Public Property FieldName As String 'FieldName
        <DataMember>
        Public Property FieldValue As String 'FieldValue
        <DataMember>
        Public Property Hours As String 'Hours
        <DataMember>
        Public Property IDEmployeeExchange As String 'Identificador del EmployeeExchange
        <DataMember>
        Public Property StartShift As DateTime 'StartShift
        <DataMember>
        Public Property FromTime As DateTime 'FromTime
        <DataMember>
        Public Property ToTime As DateTime 'ToTime
        <DataMember>
        Public Property IDCenter As String 'Identificador del centro de coste
        <DataMember>
        Public Property HolidaysDays As List(Of roRequestDayStandard) 'Días de vacaciones
        <DataMember>
        Public Property Approvals As List(Of roRequestApprovalStandard) 'Aprobaciones

    End Class

    <DataContract>
    Public Class roDatalinkStandarRequestResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Requests As Generic.List(Of roDatalinkStandarRequest)
    End Class

    <DataContract>
    Public Class roDatalinkStandarRequestCriteria
        Implements IDatalinkRequestCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property StartPeriod As Date 'FECHA INICIO PERIODO
        <DataMember>
        Public Property EndPeriod As Date 'FECHA FINAL PERIODO

        <DataMember>
        Public Property Tipo As String 'tipo solicitud

        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkRequestCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(RequestCriteriaAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(RequestCriteriaAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(RequestCriteriaAsciiColumns.NIF) = CInt(RequestCriteriaAsciiColumns.NIF)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(RequestCriteriaAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(RequestCriteriaAsciiColumns.ImportPrimaryKey) = CInt(RequestCriteriaAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(RequestCriteriaAsciiColumns.NIF_Letter) = CInt(RequestCriteriaAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(RequestCriteriaAsciiColumns.BeginPeriod) = CInt(RequestCriteriaAsciiColumns.BeginPeriod)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.BeginPeriod)) = StartPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(RequestCriteriaAsciiColumns.EndPeriod) = CInt(RequestCriteriaAsciiColumns.EndPeriod)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.EndPeriod)) = EndPeriod.ToString("yyyy/MM/dd")

                ColumnsPos(RequestCriteriaAsciiColumns.Type) = CInt(RequestCriteriaAsciiColumns.Type)
                ColumnsVal(CInt(RequestCriteriaAsciiColumns.Type)) = Tipo

                bolRet = True

                If ColumnsVal(RequestCriteriaAsciiColumns.BeginPeriod) = "" Then bolRet = False
                If ColumnsVal(RequestCriteriaAsciiColumns.EndPeriod) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

End Namespace