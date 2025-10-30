Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa una posicion del modo de la unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModePositionStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de posiciones del modo de la unidadad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModePositionListResponse
        <DataMember>
        Public Property ProductiveUnitPositions As roProductiveUnitModePosition()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una posicion del modo de la unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModePositionResponse
        <DataMember>
        Public Property ProductiveUnitMode As roProductiveUnitModePosition

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roProductiveUnitModePosition
        Protected lngID As Long
        Protected lngIDProductiveUnitMode As Long
        Protected dblQuantity As Double
        Protected bolIsExpandable As Boolean
        Protected oShiftData As roCalendarRowShiftData
        Protected oShiftHourData As roCalendarRowHourData()
        Protected oAssignmentData As roCalendarAssignmentCellData
        Protected oEmployeesData As roProductiveUnitModePositionEmployeeData()
        Protected dblCoverage As Double
        Protected _Alerts As roCalendarRowDayAlerts

        Public Sub New()
            lngID = -1
            lngIDProductiveUnitMode = -1
            dblQuantity = 0
            bolIsExpandable = False
            oShiftData = Nothing
            oShiftHourData = {}
            oAssignmentData = Nothing
            oEmployeesData = Nothing
            dblCoverage = 0
            _Alerts = Nothing
        End Sub

        ''' <summary>
        ''' Información resumen de estado de la posicion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Alerts As roCalendarRowDayAlerts
            Get
                Return _Alerts
            End Get
            Set(value As roCalendarRowDayAlerts)
                _Alerts = value
            End Set
        End Property

        ''' <summary>
        ''' Identificador único de la posicion en el modo de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID As Long
            Get
                Return lngID
            End Get
            Set(value As Long)
                lngID = value
            End Set
        End Property
        ''' <summary>
        ''' Identificador del modo de la unidad productiva a la que está asignada la posicion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDProductiveUnitMode As Long
            Get
                Return lngIDProductiveUnitMode
            End Get
            Set(value As Long)
                lngIDProductiveUnitMode = value
            End Set
        End Property

        ''' <summary>
        ''' Cantidad de recursos en la posicion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Quantity As Double
            Get
                Return dblQuantity
            End Get
            Set(value As Double)
                dblQuantity = value
            End Set
        End Property
        ''' <summary>
        ''' indica si se puede ampliar la cantidad de recursos inicial
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsExpandable As Boolean
            Get
                Return bolIsExpandable
            End Get
            Set(value As Boolean)
                bolIsExpandable = value
            End Set
        End Property

        ''' <summary>
        ''' indica la informacion del horario asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftData As roCalendarRowShiftData
            Get
                Return oShiftData
            End Get
            Set(value As roCalendarRowShiftData)
                oShiftData = value
            End Set
        End Property

        <DataMember()>
        Public Property ShiftHourData As roCalendarRowHourData()
            Get
                Return oShiftHourData
            End Get
            Set(value As roCalendarRowHourData())
                oShiftHourData = value
            End Set
        End Property

        ''' <summary>
        ''' indica la informacion del puesto asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AssignmentData As roCalendarAssignmentCellData
            Get
                Return oAssignmentData
            End Get
            Set(value As roCalendarAssignmentCellData)
                oAssignmentData = value
            End Set
        End Property

        ''' <summary>
        ''' indica la informacion de los empleados asignados a la posicion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeesData As roProductiveUnitModePositionEmployeeData()
            Get
                Return oEmployeesData
            End Get
            Set(value As roProductiveUnitModePositionEmployeeData())
                oEmployeesData = value
            End Set
        End Property

        ''' <summary>
        ''' Cobertura de la posicion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Coverage As Double
            Get
                Return dblCoverage
            End Get
            Set(value As Double)
                dblCoverage = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roProductiveUnitModePositionEmployeeData
        Protected intIDEmployee As Integer
        Protected strEmployeeName As String
        Protected oShiftData As roCalendarRowShiftData
        Protected _Alerts As roCalendarRowDayAlerts
        Protected _lstHourData As roCalendarRowHourData() ' Para vista diaria (una para cada media hora)

        Public Sub New()
            intIDEmployee = -1
            strEmployeeName = String.Empty
            oShiftData = Nothing
            _Alerts = Nothing
            _lstHourData = Nothing
        End Sub

        <DataMember()>
        Public Property HourData As roCalendarRowHourData()
            Get
                Return _lstHourData
            End Get
            Set(value As roCalendarRowHourData())
                _lstHourData = value
            End Set
        End Property

        ''' <summary>
        ''' Información resumen de estado de un empleado para un día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Alerts As roCalendarRowDayAlerts
            Get
                Return _Alerts
            End Get
            Set(value As roCalendarRowDayAlerts)
                _Alerts = value
            End Set
        End Property

        ''' <summary>
        ''' Identificador del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return intIDEmployee
            End Get
            Set(value As Integer)
                intIDEmployee = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del Empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeName As String
            Get
                Return strEmployeeName
            End Get
            Set(value As String)
                strEmployeeName = value
            End Set
        End Property
        ''' <summary>
        ''' indica la informacion del horario asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftData As roCalendarRowShiftData
            Get
                Return oShiftData
            End Get
            Set(value As roCalendarRowShiftData)
                oShiftData = value
            End Set
        End Property
    End Class

End Namespace