Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa la petición del calendario de un empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeeDayInfo

        Public Sub New()
            DayInfo = Nothing
            Status = 0
        End Sub

        ''' <summary>
        ''' Objeto calendario con la definición del horario que tiene aplicado cada día
        ''' </summary>
        <DataMember()>
        Public Property DayInfo As roCalendarRowPeriodData

        ''' <summary>
        ''' Objeto calendario con la definición del horario que tiene aplicado cada día
        ''' </summary>
        <DataMember()>
        Public Property Forecasts As EmployeeForecast()

        ''' <summary>
        ''' Estado del calendario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Detalle de una baja de un empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeeForecast
        ''' <summary>
        ''' Fecha inicio de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property BeginDate As DateTime
        ''' <summary>
        ''' Identificador de ausencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ForecastID As Integer

        ''' <summary>
        ''' Identificador de ausencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ForecastType As String
        ''' <summary>
        ''' Fecha fin de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FinishDate As DateTime

        ''' <summary>
        ''' Nombre completo de la justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Cause As String

        ''' <summary>
        ''' Identificador de la justificación de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Identificador del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployee As Integer
        ''' <summary>
        ''' Descripción extendida de la información solicitada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DocAlerts As DocumentAlert()
        ''' <summary>
        ''' Lista de alertas de documentación de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String

        <DataMember()>
        Public Property ForecastDetail As String

        ''' <summary>
        ''' Indica si la previsión soporta documentación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasDocuments As Boolean
    End Class

    ''' <summary>
    ''' Representa la petición del calendario de un empleado
    ''' </summary>
    <DataContract()>
    Public Class CurrentCapacity

        Public Sub New()
            capacities = Nothing
            Status = 0
        End Sub

        ''' <summary>
        ''' Objeto calendario con la definición del horario que tiene aplicado cada día
        ''' </summary>
        <DataMember()>
        Public Property capacities As roCalendarCapacities

        ''' <summary>
        ''' Estado del calendario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Representa la petición del calendario de un empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeeCalendar

        Public Sub New()
            oCalendar = Nothing
            Status = 0
        End Sub

        ''' <summary>
        ''' Objeto calendario con la definición del horario que tiene aplicado cada día
        ''' </summary>
        <DataMember()>
        Public Property oCalendar As roCalendarRowPeriodData

        ''' <summary>
        ''' Estado del calendario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

    End Class

    ''' <summary>
    ''' Objeto representante del fichaje de un empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeePunch

        Public Sub New()
            ID = -1
            Type = -1
            ActualType = -1
            TypeData = -1
            RelatedInfo = String.Empty
            DateTime = New DateTime(1900, 1, 1)
            LocationZone = String.Empty

            FullAddress = String.Empty
            EmployeeName = String.Empty
            ZoneName = String.Empty
            TerminalName = String.Empty
            InTelecommute = String.Empty

            Field1 = String.Empty
            Field2 = String.Empty
            Field3 = String.Empty

            Field4 = -1
            Field5 = -1
            Field6 = -1
        End Sub

        ''' <summary>
        ''' Identificador del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID As Integer

        ''' <summary>
        ''' Tipo del fichaje (1:Entrada, 2:Salida, 3:Automatica, 7:Accesos con Presencia, 4:Tareas, 13: centros de coste)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Type As Integer

        ''' <summary>
        ''' Tipo calculado de fichaje (1:Entrada, 2:Salida, 3:Automatica, 7:Accesos con Presencia, 4:Tareas, 13: centros de coste)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ActualType As Integer

        ''' <summary>
        ''' Identificador de la tarea, centro de costo o justificación en función del tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TypeData As Integer

        ''' <summary>
        ''' Nombre de la tarea, centro de costo o justificación en función del tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RelatedInfo As String

        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Población donde se ha realizado el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LocationZone As String

        ''' <summary>
        ''' Dirección completa del fichjae
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FullAddress As String

        ''' <summary>
        ''' Nombre del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeName As String

        ''' <summary>
        ''' Nombre de la zona del terminal asignada en Visualtime
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ZoneName As String

        ''' <summary>
        ''' Nombre del terminal del fichaje en Visualtime
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TerminalName As String

        <DataMember()>
        Public Property InTelecommute As String

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field1 As String

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field2 As String

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field3 As String

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field4 As Double

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field5 As Double

        ''' <summary>
        ''' Campo con información adicional del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field6 As Double

        ''' <summary>
        ''' Nombre de la justificación solicitada por el empleado si el fichaje dispone de una solicitud de justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestedTypeData As String
    End Class

    ''' <summary>
    ''' Representa un listdo de fichajes
    ''' </summary>
    <DataContract()>
    Public Class EmployeePunches

        Public Sub New()
            Punches = {}
            Status = 0
        End Sub

        ''' <summary>
        ''' Array de objectos de tipo fichaje con los datos seleccionados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Punches As EmployeePunch()

        ''' <summary>
        ''' Estado del calendario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

    End Class

    ''' <summary>
    ''' Representa una agrupación de saldos
    ''' </summary>
    <DataContract()>
    Public Class EmployeeAccrualGroup

        Public Sub New()
            key = String.Empty
            items = {}
        End Sub

        ''' <summary>
        ''' Clave para agrupar saldos, los valores posibles són(daily, week, month, year)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property key As String

        ''' <summary>
        ''' Listado de saldos con los valores en el periodo que especifica la clave
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property items As EmployeeAccrualValue()
    End Class

    ''' <summary>
    ''' Representa el valor de un saldo para un empleado en un periodo especifico
    ''' </summary>
    <DataContract()>
    Public Class EmployeeAccrualValue

        Public Sub New()
            IdAccrual = -1
            Name = String.Empty
            Value = "00:00"
        End Sub

        ''' <summary>
        ''' Identificador del saldo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdAccrual As Long

        ''' <summary>
        ''' Nombre del saldo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Valor del saldo en formato horario HH:mm
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Value As String
    End Class

    ''' <summary>
    ''' Listado de los saldos de un empleado
    ''' </summary>
    Public Class AccrualsSummary
        <DataMember()>
        Public Property ScheduleSummary As roEmployeeAccrualsSummary

        <DataMember()>
        Public Property ProductiVSummary As roEmployeeAccrualsProductivSummary

        <DataMember()>
        Public Property HolidaysSummary As roEmployeeHolidaysSummary

        ''' <summary>
        ''' Estado de los saldos obtenidos
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Listado de los saldos de un empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeeAccruals

        Public Sub New()
            ScheduleAccruals = {}
            TaskAccruals = {}
            Status = 0
        End Sub

        ''' <summary>
        ''' Listado de los saldos de presencia de un empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ScheduleAccruals As EmployeeAccrualGroup()

        ''' <summary>
        ''' Listado de los saldos de tareas de un empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskAccruals As EmployeeAccrualGroup()

        ''' <summary>
        ''' Estado de los saldos obtenidos
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

    End Class

End Namespace