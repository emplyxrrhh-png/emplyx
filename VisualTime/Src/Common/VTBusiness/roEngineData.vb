Imports Robotics.Base.VTBusiness.Move
Imports Robotics.VTBase

Public Class roEngineData

    '==================================================================
    ' DATOS DE LA FECHA PROCESADA
    '
    ' La detección de horarios y movimientos para una fecha y empleado
    '  concretos utiliza bastantes datos que se encapsulan en esta
    '  clase para que resulte más fácil de programar y utilizar.
    '
    '==================================================================
    Public Const Yesterday = 0
    Public Const Today = 1
    Public Const Tomorrow = 2

    ' Datos generales
    Public ProcDate As New roTime
    Public Employee As Object

    ' Detalles de ayer, hoy y mañana (respecto a la fecha de proceso)
    Private mShiftIDs(Yesterday To Tomorrow) As Integer   ' IDs de horarios
    Private mUsedShiftID(Yesterday To Tomorrow) As Integer        ' IDs de horarios usados
    Private mUsedFloatingTime(Yesterday To Tomorrow) As Double   ' Horas de inicio de los horarios usados
    Private mFloatingTime(Yesterday To Tomorrow) As Double   ' Horas de inicio de los horarios usados
    Private mUsedFlexibleStartTime(Yesterday To Tomorrow) As Double   ' Horas de inicio de los horarios starter usados
    Private mUsedFlexibleEndTime(Yesterday To Tomorrow) As Double   ' Horas de final de los horarios starter usados

    Private mFlexibleStartTime(Yesterday To Tomorrow) As Double   ' Horas de inicio de los horarios starter
    Private mFlexibleEndTime(Yesterday To Tomorrow) As Double   ' Horas final de los horarios starter
    Private mFlexibleName(Yesterday To Tomorrow) As String   ' Nombre de los horarios starter
    Private mFlexibleColor(Yesterday To Tomorrow) As String   ' Color de los horarios starter

    Private mSelectedShiftID(Yesterday To Tomorrow) As Integer    ' IDs de horarios seleccionados
    Private mSelectedFloatingTime(Yesterday To Tomorrow) As Double   ' Horas de inicio de los horarios seleccionados
    Private mSelectedFlexibleStartTime(Yesterday To Tomorrow) As Double    ' Horas de inicio de los horarios starter seleccionados
    Private mSelectedFlexibleEndTime(Yesterday To Tomorrow) As Double    ' Horas de final de los horarios starter seleccionados
    Private mSelectedFlexibleName(Yesterday To Tomorrow) As String    ' Nombre de los horarios starter seleccionados
    Private mSelectedFlexibleColor(Yesterday To Tomorrow) As String    ' Color de los horarios starter seleccionados

    Private mShiftCount(Yesterday To Tomorrow) As Integer         ' Numero de horarios disponibles
    Private mShiftMajor(Yesterday To Tomorrow) As Integer         ' Indice más grande de horario (1-4)

    Private mLimits(Yesterday To Tomorrow) As roTimePeriod         ' Limites horarios de ayer,hoy y mañana

    ' Movimientos de ayer,hoy y mañana
    Public Moves As New roMoveList
    Public TimeBlockList As New roTimeBlockList

    ' Puntuación
    Public BestMixedScore As Double

    ' Flags de arrastre de proceso
    Public bMustProcessYesterday As Boolean
    Public bMustProcessTomorrow As Boolean

    Public Property ShiftID(ByVal RelativeDay As Integer) As Integer
        Get
            Return mShiftIDs(RelativeDay)
        End Get
        Set(ByVal ID As Integer)
            mShiftIDs(RelativeDay) = ID
        End Set
    End Property

    Public Function IsValidShiftID(ByVal RelativeDay As Integer, ByVal RelativeDayIsFixed As Boolean, ByVal FixedID As Integer) As Boolean
        '
        ' Devuelve True si hay un ID de horario
        '
        If RelativeDayIsFixed Then
            Return (mShiftIDs(RelativeDay) = FixedID)
        Else
            Return (mShiftIDs(RelativeDay) <> 0)
        End If

    End Function

    Public Property UsedShiftID(ByVal RelativeDay As Integer) As Integer
        Get
            Return mUsedShiftID(RelativeDay)
        End Get
        Set(ByVal ID As Integer)
            mUsedShiftID(RelativeDay) = ID
        End Set

    End Property

    Public Property UsedFloatingTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mUsedFloatingTime(RelativeDay)
        End Get
        Set(ByVal NewUsedFloatingTime As Double)
            mUsedFloatingTime(RelativeDay) = NewUsedFloatingTime
        End Set

    End Property

    Public Property UsedFlexibleStartTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mUsedFlexibleStartTime(RelativeDay)
        End Get
        Set(ByVal NewUsedFlexibleStartTime As Double)
            mUsedFlexibleStartTime(RelativeDay) = NewUsedFlexibleStartTime
        End Set

    End Property

    Public Property UsedFlexibleEndTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mUsedFlexibleEndTime(RelativeDay)
        End Get
        Set(ByVal NewUsedFlexibleEndTime As Double)
            mUsedFlexibleEndTime(RelativeDay) = NewUsedFlexibleEndTime
        End Set

    End Property

    Public Property FloatingTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mFloatingTime(RelativeDay)
        End Get
        Set(ByVal NewFloatingTime As Double)
            mFloatingTime(RelativeDay) = NewFloatingTime
        End Set

    End Property

    Public Property FlexibleStartTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mFlexibleStartTime(RelativeDay)
        End Get
        Set(ByVal NewFlexibleStartTime As Double)
            mFlexibleStartTime(RelativeDay) = NewFlexibleStartTime
        End Set

    End Property

    Public Property FlexibleEndTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mFlexibleEndTime(RelativeDay)
        End Get
        Set(ByVal NewFlexibleEndTime As Double)
            mFlexibleEndTime(RelativeDay) = NewFlexibleEndTime
        End Set

    End Property

    Public Property FlexibleColor(ByVal RelativeDay As Integer) As Double
        Get
            Return mFlexibleColor(RelativeDay)
        End Get
        Set(ByVal NewFlexibleColor As Double)
            mFlexibleColor(RelativeDay) = NewFlexibleColor
        End Set

    End Property

    Public Property FlexibleName(ByVal RelativeDay As Integer) As String
        Get
            Return mFlexibleName(RelativeDay)
        End Get
        Set(ByVal NewFlexibleName As String)
            mFlexibleName(RelativeDay) = NewFlexibleName
        End Set

    End Property

    Public Property SelectedShiftID(ByVal RelativeDay As Integer) As Integer
        Get
            Return mSelectedShiftID(RelativeDay)
        End Get
        Set(ByVal ID As Integer)
            mSelectedShiftID(RelativeDay) = ID
        End Set
    End Property

    Public Property SelectedFloatingTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mSelectedFloatingTime(RelativeDay)
        End Get
        Set(ByVal NewSelectedFloatingTime As Double)
            mSelectedFloatingTime(RelativeDay) = NewSelectedFloatingTime
        End Set

    End Property

    Public Property SelectedFlexibleStartTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mSelectedFlexibleStartTime(RelativeDay)
        End Get
        Set(ByVal NewSelectedFlexibleStartTime As Double)
            mSelectedFlexibleStartTime(RelativeDay) = NewSelectedFlexibleStartTime
        End Set

    End Property

    Public Property SelectedFlexibleEndTime(ByVal RelativeDay As Integer) As Double
        Get
            Return mSelectedFlexibleEndTime(RelativeDay)
        End Get
        Set(ByVal NewSelectedFlexibleEndTime As Double)
            mSelectedFlexibleEndTime(RelativeDay) = NewSelectedFlexibleEndTime
        End Set

    End Property

    Public Property SelectedFlexibleColor(ByVal RelativeDay As Integer) As Double
        Get
            Return mSelectedFlexibleColor(RelativeDay)
        End Get
        Set(ByVal NewSelectedFlexibleColor As Double)
            SelectedFlexibleColor(RelativeDay) = NewSelectedFlexibleColor
        End Set

    End Property

    Public Property SelectedFlexibleName(ByVal RelativeDay As Integer) As String
        Get
            Return mSelectedFlexibleName(RelativeDay)
        End Get
        Set(ByVal NewSelectedFlexibleName As String)
            mSelectedFlexibleName(RelativeDay) = NewSelectedFlexibleName
        End Set

    End Property

    Public Property Limits(ByVal RelativeDay As Integer) As roTimePeriod
        Get
            Return mLimits(RelativeDay)
        End Get
        Set(ByVal NewLimits As roTimePeriod)
            mLimits(RelativeDay) = NewLimits
        End Set
    End Property



End Class