Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum BudgetView
        <EnumMember()> Definition
        <EnumMember()> Planification
    End Enum

    Public Enum BudgetDetailLevel
        <EnumMember()> Daily
        <EnumMember()> Mode
        <EnumMember()> Hour
    End Enum

    <DataContract()>
    Public Enum BudgetStatusEnum
        <EnumMember()> OK
        <EnumMember()> KO
        <EnumMember()> WARNING
    End Enum

    <DataContract()>
    Public Enum ProductiveUnitStatusOnDayEnum
        <EnumMember()> Ok
        <EnumMember()> NoPlanned
    End Enum

    <DataContract()>
    Public Enum BudgetRowState
        <EnumMember()> NoChangeRow
        <EnumMember()> NewRow
        <EnumMember()> UpdateRow
        <EnumMember()> DeleteRow
    End Enum

    <DataContract()>
    Public Enum BudgetErrorResultDayEnum
        <EnumMember()> NoContract
        <EnumMember()> FreezingDate
        <EnumMember()> PermissionDenied
    End Enum

    ''' <summary>
    ''' Representa un presupuesto
    ''' </summary>
    <DataContract>
    Public Class roBudgetResponse
        <DataMember>
        Public Property Budget As roBudget

        <DataMember>
        Public Property oState As roWsState

        <DataMember>
        Public Property BudgetResult As roBudgetResult

    End Class

    ''' <summary>
    ''' Representa el estado de los empleados de un nodo
    ''' </summary>
    <DataContract>
    Public Class roCurrentStatusEmployeesSummaryResponse
        <DataMember>
        Public Property NodeStatus As roCurrentStatusEmployeesSummary

        <DataMember>
        Public Property oState As roWsState

    End Class

    <DataContract>
    Public Class roBudgetEmployeeAvailableForPositionResponse
        <DataMember>
        Public Property Employees As roBudgetEmployeeAvailableForPosition()

        <DataMember>
        Public Property oState As roWsState

    End Class

    <DataContract>
    Public Class roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse
        <DataMember>
        Public Property Employees As roBudgetEmployeeAvailableForPosition()

        <DataMember>
        Public Property NodeStatus As roCurrentStatusEmployeesSummary

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roStandarResponse

        Public Sub New()
            Me.oState = New roWsState
            Me.Result = False
        End Sub

        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roEmployeeAvailableForNodeResponse
        <DataMember>
        Public Property AvailableEmployees As roEmployeeAvailableForNode()

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roBudgetResult
        Protected _intStatus As BudgetStatusEnum
        Protected _lstBudgetDataResult As roBudgetDataDayError()

        <DataMember()>
        Public Property Status As BudgetStatusEnum
            Get
                Return _intStatus
            End Get
            Set(value As BudgetStatusEnum)
                _intStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property BudgetDataResult As roBudgetDataDayError()
            Get
                Return _lstBudgetDataResult
            End Get
            Set(value As roBudgetDataDayError())
                _lstBudgetDataResult = value
            End Set
        End Property

        Public Sub New()
            _intStatus = BudgetStatusEnum.OK
            _lstBudgetDataResult = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roBudget
        Protected _lstBudgetHeader As roBudgetHeader
        Protected _lstBudgetData As roBudgetRow()
        Protected _dateFirstDay As Date
        Protected _dateLastDay As Date
        Protected _dateFreezingDate As Date

        <DataMember()>
        Public Property BudgetHeader As roBudgetHeader
            Get
                Return _lstBudgetHeader
            End Get
            Set(value As roBudgetHeader)
                _lstBudgetHeader = value
            End Set
        End Property

        <DataMember()>
        Public Property BudgetData As roBudgetRow()
            Get
                Return _lstBudgetData
            End Get
            Set(value As roBudgetRow())
                _lstBudgetData = value
            End Set
        End Property

        <DataMember()>
        Public Property FirstDay As Date
            Get
                Return _dateFirstDay
            End Get
            Set(value As Date)
                _dateFirstDay = value
            End Set
        End Property

        <DataMember()>
        Public Property LastDay As Date
            Get
                Return _dateLastDay
            End Get
            Set(value As Date)
                _dateLastDay = value
            End Set
        End Property

        <DataMember()>
        Public Property FreezingDate As Date
            Get
                Return _dateFreezingDate
            End Get
            Set(value As Date)
                _dateFreezingDate = value
            End Set
        End Property

        Public Sub New()
            _lstBudgetHeader = Nothing
            _lstBudgetData = Nothing
            _dateFirstDay = Nothing
            _dateLastDay = Nothing
            _dateFreezingDate = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roBudgetHeader

        Protected _ProductiveUnitHeaderData As roBudgetHeaderCell
        Protected _lstPeriodHeaderData As roBudgetHeaderCell()

        <DataMember()>
        Public Property ProductiveUnitHeaderData As roBudgetHeaderCell
            Get
                Return _ProductiveUnitHeaderData
            End Get
            Set(value As roBudgetHeaderCell)
                _ProductiveUnitHeaderData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodHeaderData As roBudgetHeaderCell()
            Get
                Return _lstPeriodHeaderData
            End Get
            Set(value As roBudgetHeaderCell())
                _lstPeriodHeaderData = value
            End Set
        End Property

        Public Sub New()
            _ProductiveUnitHeaderData = Nothing
            _lstPeriodHeaderData = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roBudgetHeaderCell
        Protected _strRow1 As String
        Protected _strRow2 As String
        Protected _strBackColor As String
        Protected _bolFeastDay As Boolean

        <DataMember()>
        Public Property Row1Text As String
            Get
                Return _strRow1
            End Get
            Set(value As String)
                _strRow1 = value
            End Set
        End Property

        <DataMember()>
        Public Property Row2Text As String
            Get
                Return _strRow2
            End Get
            Set(value As String)
                _strRow2 = value
            End Set
        End Property

        <DataMember()>
        Public Property BackColor As String
            Get
                Return _strBackColor
            End Get
            Set(value As String)
                _strBackColor = value
            End Set
        End Property

        <DataMember()>
        Public Property FeastDay As Boolean
            Get
                Return _bolFeastDay
            End Get
            Set(value As Boolean)
                _bolFeastDay = value
            End Set
        End Property

        Public Sub New()
            _strRow1 = String.Empty
            _strRow2 = String.Empty
            _strBackColor = String.Empty
            _bolFeastDay = False
        End Sub

    End Class

    <DataContract()>
    Public Class roBudgetRow

        Protected _ProductiveUnitData As roBudgetRowProductiveUnitData
        Protected _PeriodData As roBudgetRowPeriodData
        Protected _Pos As Integer
        Protected _RowState As BudgetRowState

        Public Sub New()
            _ProductiveUnitData = Nothing
            _PeriodData = Nothing
            _Pos = 0
            _RowState = BudgetRowState.NoChangeRow
        End Sub

        <DataMember()>
        Public Property ProductiveUnitData As roBudgetRowProductiveUnitData
            Get
                Return _ProductiveUnitData
            End Get
            Set(value As roBudgetRowProductiveUnitData)
                _ProductiveUnitData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodData As roBudgetRowPeriodData
            Get
                Return _PeriodData
            End Get
            Set(value As roBudgetRowPeriodData)
                _PeriodData = value
            End Set
        End Property

        <DataMember()>
        Public Property Pos As Integer
            Get
                Return _Pos
            End Get
            Set(value As Integer)
                _Pos = value
            End Set
        End Property

        <DataMember()>
        Public Property RowState As BudgetRowState
            Get
                Return _RowState
            End Get
            Set(value As BudgetRowState)
                _RowState = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roBudgetRowProductiveUnitData
        Protected _ProductiveUnit As roProductiveUnit
        Protected _intIDNode As Integer
        Protected _strNodeName As String
        Protected _intPermission As Integer

        Public Sub New()
            _ProductiveUnit = Nothing
            _strNodeName = String.Empty
            _intIDNode = -1
            _intPermission = 0
        End Sub

        <DataMember()>
        Public Property ProductiveUnit As roProductiveUnit
            Get
                Return _ProductiveUnit
            End Get
            Set(value As roProductiveUnit)
                _ProductiveUnit = value
            End Set
        End Property

        <DataMember()>
        Public Property IDNode As Integer
            Get
                Return _intIDNode
            End Get
            Set(value As Integer)
                _intIDNode = value
            End Set
        End Property

        <DataMember()>
        Public Property NodeName As String
            Get
                Return _strNodeName
            End Get
            Set(value As String)
                _strNodeName = value
            End Set
        End Property

        <DataMember()>
        Public Property Permission As Integer
            Get
                Return _intPermission
            End Get
            Set(value As Integer)
                _intPermission = value
            End Set
        End Property

    End Class

    ''' <summary>
    ''' Representa toda la información sobre la planificación de una unidad productiva para un periodo de fechas
    ''' </summary>
    <DataContract()>
    Public Class roBudgetRowPeriodData

        ''' <summary>Infomación de planificación de la unidad productiva para un día</summary>
        Protected _lstDayData As roBudgetRowDayData()

        Public Sub New()
            _lstDayData = Nothing
        End Sub

        ''' <summary>
        ''' Devuelve o establece un array de elementos con la información diaria de planificación de una unidad productiva. Cada elemento contiene la información de un día del periodo de fechas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DayData As roBudgetRowDayData()
            Get
                Return _lstDayData
            End Get
            Set(value As roBudgetRowDayData())
                _lstDayData = value
            End Set
        End Property

    End Class

    ''' <summary>
    ''' Información de planificación de un día para una unidad productiva
    ''' </summary>
    <DataContract()>
    Public Class roBudgetRowDayData
        Protected _Date As Date
        Protected _HasChanged As Boolean
        Protected _ProductiveUnitMode As roProductiveUnitMode
        Protected _ProductiveUnitStatus As ProductiveUnitStatusOnDayEnum
        Protected _canBeModified As Boolean

        ''' <summary>
        ''' Fecha a la que se refiere el resto de la información de planificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PlanDate As Date
            Get
                Return _Date
            End Get
            Set(value As Date)
                _Date = value
            End Set
        End Property

        <DataMember()>
        Public Property HasChanged As Boolean
            Get
                Return _HasChanged
            End Get
            Set(value As Boolean)
                _HasChanged = value
            End Set
        End Property

        <DataMember()>
        Public Property ProductiveUnitMode As roProductiveUnitMode
            Get
                Return _ProductiveUnitMode
            End Get
            Set(value As roProductiveUnitMode)
                _ProductiveUnitMode = value
            End Set
        End Property
        <DataMember()>
        Public Property ProductiveUnitStatus As ProductiveUnitStatusOnDayEnum
            Get
                Return _ProductiveUnitStatus
            End Get
            Set(value As ProductiveUnitStatusOnDayEnum)
                _ProductiveUnitStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property CanBeModified As Boolean
            Get
                Return _canBeModified
            End Get
            Set(value As Boolean)
                _canBeModified = value
            End Set
        End Property

        Public Sub New()
            _Date = Date.Now.Date
            _HasChanged = False
            _ProductiveUnitMode = Nothing
            _ProductiveUnitStatus = ProductiveUnitStatusOnDayEnum.Ok
            _canBeModified = False

        End Sub

    End Class

    <DataContract()>
    Public Class roBudgetDataDayError
        Protected _intIDProductiveUnit As Integer
        Protected _Date As Date
        Protected _ErrorCode As BudgetErrorResultDayEnum
        Protected _Errortext As String

        <DataMember()>
        Public Property IDProductiveUnit As Integer
            Get
                Return _intIDProductiveUnit
            End Get
            Set(value As Integer)
                _intIDProductiveUnit = value
            End Set
        End Property
        <DataMember()>
        Public Property ErrorDate As Date
            Get
                Return _Date
            End Get
            Set(value As Date)
                _Date = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorCode As BudgetErrorResultDayEnum
            Get
                Return _ErrorCode
            End Get
            Set(value As BudgetErrorResultDayEnum)
                _ErrorCode = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorText As String
            Get
                Return _Errortext
            End Get
            Set(value As String)
                _Errortext = value
            End Set
        End Property

        Public Sub New()
            _intIDProductiveUnit = 0
            _Date = Now.Date
            '_ErrorCode = BudgetErrorResultDayEnum.NoContract
        End Sub

    End Class

    <DataContract()>
    Public Class roBudgetEmployeeAvailableForPosition
        Protected _intIDEmployee As Integer
        Protected _strEmployeeName As String
        Protected _intIDAssignment As Integer
        Protected _strAssignmentName As String
        Protected _intIDShift As Integer
        Protected _strShiftName As String
        Protected _intIDGroup As Integer
        Protected _strFullGroupName As String
        Protected _lngCost As Double
        Protected _Indictments As roCalendarScheduleIndictment()
        Protected _TotalIndictments As Integer

        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return _intIDEmployee
            End Get
            Set(value As Integer)
                _intIDEmployee = value
            End Set
        End Property
        <DataMember()>
        Public Property EmployeeName As String
            Get
                Return _strEmployeeName
            End Get
            Set(value As String)
                _strEmployeeName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDAssignment As Integer
            Get
                Return _intIDAssignment
            End Get
            Set(value As Integer)
                _intIDAssignment = value
            End Set
        End Property

        <DataMember()>
        Public Property AssignmentName As String
            Get
                Return _strAssignmentName
            End Get
            Set(value As String)
                _strAssignmentName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDShift As Integer
            Get
                Return _intIDShift
            End Get
            Set(value As Integer)
                _intIDShift = value
            End Set
        End Property

        <DataMember()>
        Public Property ShiftName As String
            Get
                Return _strShiftName
            End Get
            Set(value As String)
                _strShiftName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDGroup As Integer
            Get
                Return _intIDGroup
            End Get
            Set(value As Integer)
                _intIDGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property FullGroupName As String
            Get
                Return _strFullGroupName
            End Get
            Set(value As String)
                _strFullGroupName = value
            End Set
        End Property

        <DataMember()>
        Public Property Cost As Double
            Get
                Return _lngCost
            End Get
            Set(value As Double)
                _lngCost = value
            End Set
        End Property

        <DataMember()>
        Public Property Indictments As roCalendarScheduleIndictment()
            Get
                Return _Indictments
            End Get
            Set(value As roCalendarScheduleIndictment())
                _Indictments = value
            End Set
        End Property

        <DataMember()>
        Public Property TotalIndictments As Integer
            Get
                Return _TotalIndictments
            End Get
            Set(value As Integer)
                _TotalIndictments = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary
        Protected _BudgetEmployeeAvailableForPositions As roBudgetEmployeeAvailableForPosition()
        Protected _CurrentStatusEmployeesSummary As roCurrentStatusEmployeesSummary

        <DataMember()>
        Public Property BudgetEmployeeAvailableForPositions As roBudgetEmployeeAvailableForPosition()
            Get
                Return _BudgetEmployeeAvailableForPositions
            End Get
            Set(value As roBudgetEmployeeAvailableForPosition())
                _BudgetEmployeeAvailableForPositions = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentStatusEmployeesSummary As roCurrentStatusEmployeesSummary
            Get
                Return _CurrentStatusEmployeesSummary
            End Get
            Set(value As roCurrentStatusEmployeesSummary)
                _CurrentStatusEmployeesSummary = value
            End Set
        End Property

        Public Sub New()
            _CurrentStatusEmployeesSummary = Nothing
            _BudgetEmployeeAvailableForPositions = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roEmployeeAvailableForNode
        Protected _BudgetDate As Date
        Protected _BudgetEmployeeAvailableForNode As roBudgetEmployeeAvailableForPosition()

        <DataMember()>
        Public Property BudgetDate As Date
            Get
                Return _BudgetDate
            End Get
            Set(value As Date)
                _BudgetDate = value
            End Set
        End Property

        <DataMember()>
        Public Property BudgetEmployeeAvailableForNode As roBudgetEmployeeAvailableForPosition()
            Get
                Return _BudgetEmployeeAvailableForNode
            End Get
            Set(value As roBudgetEmployeeAvailableForPosition())
                _BudgetEmployeeAvailableForNode = value
            End Set
        End Property

        Public Sub New()
            _BudgetEmployeeAvailableForNode = Nothing
            _BudgetDate = Nothing
        End Sub

    End Class

End Namespace