Public Class roMoveItem
    Private mID As Double

    Private mPeriod As New roTimePeriod

    Private mInReader As Long
    Private mOutReader As Long
    Private mInCause As Long
    Private mOutCause As Long

    Private mShiftDate As New roTime

    Public Function Clone() As roMoveItem
        '
        ' Devuelve una copia de este objeto
        '
        Dim ClonedMove As New roMoveItem

        ClonedMove.ID = ID
        ClonedMove.InCause = InCause
        ClonedMove.InReader = InReader
        ClonedMove.OutCause = OutCause
        ClonedMove.OutReader = OutReader
        ClonedMove.Period = Period.Clone
        ClonedMove.ShiftDate = ShiftDate

        Clone = ClonedMove

    End Function

    Public Property InReader() As Long
        Get
            Return mInReader
        End Get
        Set(ByVal value As Long)
            mInReader = value
        End Set
    End Property

    Public Property ID() As Double
        Get
            Return mID
        End Get
        Set(ByVal value As Double)
            mID = value
        End Set
    End Property

    Public Property OutReader() As Long
        Get
            Return mOutReader
        End Get
        Set(ByVal value As Long)
            mOutReader = value
        End Set
    End Property

    Public Property InCause() As Long
        Get
            Return mInCause
        End Get
        Set(ByVal value As Long)
            mInCause = value
        End Set
    End Property

    Public Property ShiftDate() As roTime
        Get
            Return mShiftDate
        End Get
        Set(ByVal value As roTime)
            mShiftDate = value
        End Set
    End Property

    Public Property OutCause() As Long
        Get
            Return mOutCause
        End Get
        Set(ByVal value As Long)
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

    '    Public Function LoadFromActiveDataset(ByRef PositionedDataset As roDataset, ByRef LogHandle As roConnector) As Boolean
    '        '
    '        ' Carga datos del movimiento de un dataset posicionado.
    '        '   Devuelve True si ha cargado, False si hay algun error.
    '        '

    '        On Error GoTo catch

    'mID = PositionedDataset.Collect("ID")
    '        mInCause = Any2Long(PositionedDataset.Collect("InIDCause"))
    '        mOutCause = Any2Long(PositionedDataset.Collect("OutIDCause"))
    '        mInReader = Any2Long(PositionedDataset.Collect("InIDReader"))
    '        mOutReader = Any2Long(PositionedDataset.Collect("OutIDReader"))
    'Set mShiftDate = Any2Time(PositionedDataset.Collect("ShiftDate"))

    'Set mPeriod = New roTimePeriod
    'If Not IsNull(PositionedDataset.Collect("InDateTime")) Then Set mPeriod.Begin = Any2Time(PositionedDataset.Collect("InDateTime"))
    'If Not IsNull(PositionedDataset.Collect("OutDateTime")) Then Set mPeriod.Finish = Any2Time(PositionedDataset.Collect("OutDateTime"))

    '' Valida
    '            LoadFromActiveDataset = True

    '        Finally
    '        Exit Function

    '        Catch
    '        LogHandle.LogMessage roDebug, "Error loading move details. (Error " & Err() & ": " & Error$ & ")"
    '    LoadFromActiveDataset = False
    '        mID = Null
    '        Resume finally

    'End Function

End Class