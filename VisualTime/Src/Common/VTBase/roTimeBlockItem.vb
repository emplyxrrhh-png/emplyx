Imports Robotics.VTBase.roTypes

Public Class roTimeBlockItem

    Private mMoveID As Object
    Private mPeriod As New roTimePeriod
    Private mInReader As Object
    Private mOutReader As Object
    Private mInCause As Object
    Private mOutCause As Object
    Private mMoveShiftDate As New roTime
    Private mTimeValue As roTime
    Private mType As roBlockType
    Private mTimeZone As Object
    Private mTag As Object
    Private mIDCenter As Double
    Private mDefaultCenter As Boolean

    Public Enum roBlockType
        roBTAny = 0
        roBTAnyAttendance = 1
        roBTAnyAbsence = 2
        roBTAnyOvertime = 3

        roBTWorking = 1001
        roBTOverworking = 1010
        roBTAbsence = 1011
        roBTLateArrival = 1020
        roBTUnexpectedBreak = 1021
        roBTEarlyLeave = 1022
        roBTFlexibleOverworking = 1030
        roBTFlexibleUnderworking = 1031
        roBTBreak = 1040
        roBTOvertimeBreak = 1041
        roBTUndertimeBreak = 1042

        roBTDailyOverworking = 1050
        roBTDailyUnderworking = 1051
        roBTComplementary = 2000
    End Enum

    Public Enum roManualEntryCauseType
        roInWorking = 1
        roOutWorking = 2
        roInAbsence = 3
        roOutAbsence = 4
    End Enum

    Public Function DebugText() As String
        '
        ' Funcion para debugar, devuelve los datos de este objeto en texto legible
        '
        Dim st As String = ""

        Select Case BlockType
            Case roBlockType.roBTAny : st = "Any??   "
            Case roBlockType.roBTAnyAttendance : st = "AnyATT??"
            Case roBlockType.roBTAnyAbsence : st = "AnyABS??"
            Case roBlockType.roBTAnyOvertime : st = "AnyOVW??"

            Case roBlockType.roBTBreak : st = "Break   "
            Case roBlockType.roBTAbsence : st = "[ABSEN!]"
            Case roBlockType.roBTEarlyLeave : st = "[E.OUT!]"
            Case roBlockType.roBTFlexibleOverworking : st = "[FLEX.+]"
            Case roBlockType.roBTFlexibleUnderworking : st = "[FLEX.-]"
            Case roBlockType.roBTLateArrival : st = "[L.IN! ]"
            Case roBlockType.roBTOvertimeBreak : st = "[BREAK+]"
            Case roBlockType.roBTOverworking : st = "Extra   "
            Case roBlockType.roBTUndertimeBreak : st = "[BREAK-]"
            Case roBlockType.roBTUnexpectedBreak : st = "[U.BRK!]"
            Case roBlockType.roBTWorking : st = "Working "
            Case roBlockType.roBTComplementary : st = "Complementary "

            Case roBlockType.roBTDailyOverworking : st = "[TOTAL+]"
            Case roBlockType.roBTDailyUnderworking : st = "[TOTAL+]"
        End Select

        DebugText = "Type:" & st & vbTab & Period.DebugText & vbTab & "TimeValue:" & TimeValue.Value

    End Function

    Public Property Tag() As Object
        Get
            Return mTag
        End Get
        Set(ByVal value As Object)
            mTag = value
        End Set
    End Property

    Public Property IDCenter() As Object
        Get
            Return mIDCenter
        End Get
        Set(ByVal value As Object)
            mIDCenter = value
        End Set
    End Property

    Public Property DefaultCenter() As Object
        Get
            Return mDefaultCenter
        End Get
        Set(ByVal value As Object)
            mDefaultCenter = value
        End Set
    End Property

    Public Property TimeValue() As roTime
        '
        ' Devuelve el valor de tiempo en formato de VB
        '
        Get

            If mTimeValue Is Nothing Then
                TimeValue = mPeriod.PeriodTime
            Else
                ' Devuelve el tiempo

                ' Comprueba que no exceda el máximo del intervalo
                If mTimeValue.VBNumericValue > mPeriod.PeriodTime.VBNumericValue Then
                    ' Si el tiempo de la incidencia en el bloque es superior al tiempo que nos ha quedado ahora
                    '  recortamos dicho tiempo.
                    mTimeValue = mPeriod.PeriodTime
                End If

                TimeValue = mTimeValue
            End If

        End Get
        '
        ' Inicializa con el tiempo indicado
        '
        Set(ByVal value As roTime)
            If value.VBNumericValue > mPeriod.PeriodTime.VBNumericValue Then
                mTimeValue = mPeriod.PeriodTime
            Else
                If Year(value.Value) = 1 Then
                    Dim x As New DateTime(1899, 12, 30)
                    x = x.AddHours(CDate(value.Value).Hour).AddMinutes(CDate(value.Value).Minute)
                    value = Any2Time(x)
                End If
                mTimeValue = value
            End If
        End Set
    End Property

    Public Property InReader() As Object
        Get
            Return mInReader
        End Get
        Set(ByVal value As Object)
            mInReader = value
        End Set
    End Property

    Public Property OutReader() As Object
        Get
            Return mOutReader
        End Get
        Set(ByVal value As Object)
            mOutReader = value
        End Set
    End Property

    Public Property InCause() As Object
        Get
            Return mInCause
        End Get
        Set(ByVal value As Object)
            mInCause = value
        End Set
    End Property

    Public Property MoveShiftDate() As roTime
        Get
            Return mMoveShiftDate
        End Get
        Set(ByVal value As roTime)
            mMoveShiftDate = value
        End Set
    End Property

    Public Property OutCause() As Object
        Get
            Return mOutCause
        End Get
        Set(ByVal value As Object)
            mOutCause = value
        End Set
    End Property

    Public Property Period() As roTimePeriod
        Get
            Return mPeriod
        End Get
        Set(ByVal value As roTimePeriod)
            mPeriod = value
        End Set
    End Property

    Public Property BlockType() As roBlockType
        Get
            Return mType
        End Get
        Set(ByVal value As roBlockType)
            mType = value
        End Set
    End Property

    Public Sub LoadFromMove(ByVal Move As roMoveItem)
        '
        ' Inicializa bloque a partir de un movimiento.
        '

        mMoveID = Move.ID
        'mPeriod = Move.Period
        Dim tmpPeriod As New roTimePeriod
        tmpPeriod.Begin = Move.Period.Begin
        tmpPeriod.Finish = Move.Period.Finish
        mPeriod = tmpPeriod

        mInReader = Move.InReader
        mOutReader = Move.OutReader
        mInCause = Move.InCause
        mOutCause = Move.OutCause
        mMoveShiftDate = Move.ShiftDate

        mType = roBlockType.roBTAny

    End Sub

    Public Function Clone() As roTimeBlockItem
        '
        ' Devuelve una copia exacta de este bloque
        '

        Dim aNewTimeBlock As roTimeBlockItem

        aNewTimeBlock = New roTimeBlockItem

        aNewTimeBlock.Period.Begin = Any2Time(mPeriod.Begin.Value)
        aNewTimeBlock.Period.Finish = Any2Time(mPeriod.Finish.Value)

        aNewTimeBlock.InReader = mInReader
        aNewTimeBlock.OutReader = mOutReader
        aNewTimeBlock.InCause = mInCause
        aNewTimeBlock.OutCause = mOutCause
        If mMoveShiftDate IsNot Nothing AndAlso mMoveShiftDate.Value IsNot Nothing Then
            aNewTimeBlock.MoveShiftDate = Any2Time(mMoveShiftDate.Value)
        End If

        aNewTimeBlock.BlockType = mType

        Clone = aNewTimeBlock

    End Function

    Public Function Copy(ByVal TimeBlockItem As roTimeBlockItem) As roTimeBlockItem
        'Llena el Item con una copia de los datos del item que se pasa por parametro

        mPeriod.Begin = Any2Time(TimeBlockItem.Period.Begin.Value)
        mPeriod.Finish = Any2Time(TimeBlockItem.Period.Finish.Value)
        mInReader = TimeBlockItem.InReader
        mOutReader = TimeBlockItem.OutReader
        mInCause = TimeBlockItem.InCause
        mOutCause = TimeBlockItem.OutCause

        mMoveShiftDate = Any2Time(TimeBlockItem.MoveShiftDate)

        mType = TimeBlockItem.BlockType

        Copy = Me

    End Function

    Public Function IsWorkingTime() As Boolean
        '
        ' Segun el tipo de la capa horaria devuelve si las horas son trabajadas o no
        '

        Select Case mType
            Case roBlockType.roBTAnyAttendance,
                roBlockType.roBTAnyOvertime,
                roBlockType.roBTDailyOverworking,
                roBlockType.roBTFlexibleOverworking,
                roBlockType.roBTOverworking,
                roBlockType.roBTUndertimeBreak,
                roBlockType.roBTWorking,
                roBlockType.roBTComplementary
                IsWorkingTime = True
            Case Else
                IsWorkingTime = False
        End Select

    End Function

    Public Property TimeZone() As Object
        Get
            Return mTimeZone
        End Get
        Set(ByVal value As Object)
            mTimeZone = value
        End Set
    End Property

    Public Function ValidatesFilter(ByVal FilterType As roBlockType) As Boolean
        '
        ' Devuelve True si la capa es del tipo indicado o bien:
        '  Se especifica roBTAny como filtro.
        '  Se especifica roBTAnyAbsence y la capa es de un tipo de ausencia cualquiera.
        '  Se especifica roBTAnyAttendance y la capa es de un tipo de presencia cualquiera.
        '
        ValidatesFilter =
            (FilterType = roBlockType.roBTAnyAbsence And Not IsWorkingTime()) Or
            (FilterType = roBlockType.roBTAnyAttendance And IsWorkingTime()) Or
            (FilterType = roBlockType.roBTAny) Or
            (mType = FilterType) Or
            (FilterType = roBlockType.roBTAnyOvertime And mType = roBlockType.roBTDailyOverworking) Or
            (FilterType = roBlockType.roBTAnyOvertime And mType = roBlockType.roBTFlexibleOverworking) Or
            (FilterType = roBlockType.roBTAnyOvertime And mType = roBlockType.roBTOverworking)

    End Function

End Class