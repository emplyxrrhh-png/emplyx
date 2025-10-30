Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports SwaggerWcf.Attributes

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum DailyCauseExcelColumns
        <EnumMember> Contract
        <EnumMember> CauseDate
        <EnumMember> Cause
        <EnumMember> Value
        <EnumMember> NIF
        <EnumMember> IDEmployee
        <EnumMember> ImportPrimaryKey
        <EnumMember> NIF_Letter
    End Enum

    Public Enum DailyCauseAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> BeginPeriod
        <EnumMember> EndPeriod
        <EnumMember> ImportPrimaryKey
        <EnumMember> Timestamp
        <EnumMember> AddRelatedIncidence
        <EnumMember> ShowChangesInPeriod

    End Enum

    Public Interface IDatalinkDailyCause

        Function GetDailyCauseColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    Public Interface IDatalinkDailyCauseCriteria
        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkDailyCause
        Implements IDatalinkDailyCause
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Contract short name")>
        <DataMember>
        Public Property ShortCauseName As String

        <SwaggerWcfProperty(Required:=True, Default:="2021-11-30T21:30:42.233Z", Description:="Date of the DailyCause")>
        <DataMember>
        Public Property CauseDate As Date

        <SwaggerWcfProperty(Required:=True, Description:="Desired value for the DailyCause")>
        <DataMember>
        Public Property Value As String

        Public Function GetDailyCauseColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkDailyCause.GetDailyCauseColumnsDefinition
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(DailyCauseExcelColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(DailyCauseExcelColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(DailyCauseExcelColumns.NIF) = CInt(DailyCauseExcelColumns.NIF)
                ColumnsVal(CInt(DailyCauseExcelColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(DailyCauseExcelColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(DailyCauseExcelColumns.ImportPrimaryKey) = CInt(DailyCauseExcelColumns.ImportPrimaryKey)
                ColumnsVal(CInt(DailyCauseExcelColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(DailyCauseExcelColumns.NIF_Letter) = CInt(DailyCauseExcelColumns.NIF_Letter)
                ColumnsVal(CInt(DailyCauseExcelColumns.NIF_Letter)) = String.Empty

                ColumnsPos(DailyCauseExcelColumns.Cause) = CInt(DailyCauseExcelColumns.Cause)
                ColumnsVal(CInt(DailyCauseExcelColumns.Cause)) = ShortCauseName

                ColumnsPos(DailyCauseExcelColumns.CauseDate) = CInt(DailyCauseExcelColumns.CauseDate)
                ColumnsVal(CInt(DailyCauseExcelColumns.CauseDate)) = CauseDate.ToString("yyyy/MM/dd")

                ColumnsPos(DailyCauseExcelColumns.Value) = CInt(DailyCauseExcelColumns.Value)
                ColumnsVal(CInt(DailyCauseExcelColumns.Value)) = Value

                bolRet = True
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        <DataContract>
        Public Class roDatalinkStandarDailyCauseResponse

            <DataMember(Order:=0)>
            <Description("Result code that indicates the request status")>
            Public Property ResultCode As ReturnCode

            <DataMember(Order:=1)>
            <SwaggerWcfProperty(Required:=True, Default:="", Description:="Result from Rest api request")>
            <Description("Result from Rest api request")>
            Public Property ResultDetails As String

            <DataMember(Order:=2)>
            <SwaggerWcfProperty(Required:=True, Default:="", Description:="Result cause with updated or created data")>
            <Description("Result cause with updated or created data")>
            Public Property ResultDailyCause As roDailyCause

        End Class

    End Class

    <DataContract>
    Public Class roDatalinkStandarCauseResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Causes As Generic.List(Of roDatalinkStandarDailyCause)
    End Class

    <DataContract>
    Public Class roDatalinkStandarDailyCauseCriteria
        Implements IDatalinkDailyCauseCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA

        <DataMember>
        Public Property StartCausePeriod As Date 'FECHA INICIO PERIODO
        <DataMember>
        Public Property EndCausePeriod As Date 'FECHA FINAL PERIODO
        <DataMember>
        Public Property Timestamp As Date 'TIMESTAMP EN QUE SE HA CALCULADO EL DÍA

        <DataMember>
        Public Property AddRelatedIncidence As Boolean 'INDICA SI DEBEMOS INCLUIR LA INCIDENCIA RELACIONADA

        <DataMember>
        Public Property ShowChangesInPeriod As Boolean 'Devuelve los cambios realizados en el periodo



        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkDailyCauseCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(DailyCauseAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(DailyCauseAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(DailyCauseAsciiColumns.NIF) = CInt(DailyCauseAsciiColumns.NIF)
                ColumnsVal(CInt(DailyCauseAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(DailyCauseAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(DailyCauseAsciiColumns.ImportPrimaryKey) = CInt(DailyCauseAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(DailyCauseAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(DailyCauseAsciiColumns.NIF_Letter) = CInt(DailyCauseAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(DailyCauseAsciiColumns.NIF_Letter)) = String.Empty

                If Not ShowChangesInPeriod Then

                    ColumnsPos(DailyCauseAsciiColumns.BeginPeriod) = CInt(DailyCauseAsciiColumns.BeginPeriod)
                    ColumnsVal(CInt(DailyCauseAsciiColumns.BeginPeriod)) = StartCausePeriod.ToString("yyyy/MM/dd")

                    ColumnsPos(DailyCauseAsciiColumns.EndPeriod) = CInt(DailyCauseAsciiColumns.EndPeriod)
                    ColumnsVal(CInt(DailyCauseAsciiColumns.EndPeriod)) = EndCausePeriod.ToString("yyyy/MM/dd")
                Else
                    ColumnsPos(DailyCauseAsciiColumns.BeginPeriod) = CInt(DailyCauseAsciiColumns.BeginPeriod)
                    ColumnsVal(CInt(DailyCauseAsciiColumns.BeginPeriod)) = StartCausePeriod.ToString("yyyy/MM/dd HH:mm:ss")

                    ColumnsPos(DailyCauseAsciiColumns.EndPeriod) = CInt(DailyCauseAsciiColumns.EndPeriod)
                    ColumnsVal(CInt(DailyCauseAsciiColumns.EndPeriod)) = EndCausePeriod.ToString("yyyy/MM/dd HH:mm:ss")
                End If

                ColumnsPos(DailyCauseAsciiColumns.Timestamp) = CInt(DailyCauseAsciiColumns.Timestamp)
                ColumnsVal(CInt(DailyCauseAsciiColumns.Timestamp)) = Timestamp.ToString()

                ColumnsPos(DailyCauseAsciiColumns.AddRelatedIncidence) = CInt(DailyCauseAsciiColumns.AddRelatedIncidence)
                ColumnsVal(CInt(DailyCauseAsciiColumns.AddRelatedIncidence)) = IIf(AddRelatedIncidence, "1", "0")

                ColumnsPos(DailyCauseAsciiColumns.ShowChangesInPeriod) = CInt(DailyCauseAsciiColumns.ShowChangesInPeriod)
                ColumnsVal(CInt(DailyCauseAsciiColumns.ShowChangesInPeriod)) = IIf(ShowChangesInPeriod, "1", "0")


                bolRet = True

                If ColumnsVal(DailyCauseAsciiColumns.BeginPeriod) = "" AndAlso ColumnsVal(DailyCauseAsciiColumns.Timestamp) = "" Then bolRet = False
                If ColumnsVal(DailyCauseAsciiColumns.EndPeriod) = "" AndAlso ColumnsVal(DailyCauseAsciiColumns.Timestamp) = "" Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roDatalinkStandarDailyCause
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA            
        <DataMember>
        Public Property CauseShortName As String 'Nombre corto
        <DataMember>
        Public Property CauseDate As Date 'Fecha del acumulado
        <DataMember>
        Public Property CauseValue As Double 'Valor del acumulado
        <DataMember>
        Public Property Manual As Boolean 'Manual
        <DataMember>
        Public Property Incidence As Double 'Incidencia relacionada
        <DataMember>
        Public Property IncidenceData As roDatalinkStandarDailyIncidence ' Datos Incidencia relacionada
        <DataMember>
        Public Property CauseEquivalenceCode As String 'Código de equivalencia de la justificación

    End Class

    <DataContract>
    Public Class roDatalinkStandarDailyIncidence
        <DataMember>
        Public Property Incidence As Double 'ID Incidencia
        <DataMember>
        Public Property IncidenceDate As Date 'Fecha de la incidencia
        <DataMember>
        Public Property IncidenceType As Integer 'Tipo de incidencia
        <DataMember>
        Public Property IncidenceBeginTime As Date 'Fecha/hora inicial del periodo de la incidencia
        <DataMember>
        Public Property IncidenceEndTime As Date 'Fecha/hora final del periodo de la incidencia
        <DataMember>
        Public Property IncidenceZone As String 'Nombre de la franja horaria de la incidencia
        <DataMember>
        Public Property IncidenceValue As Double 'Valor de la incidencia

    End Class
End Namespace