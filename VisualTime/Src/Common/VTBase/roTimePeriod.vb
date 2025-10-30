Public Class roTimePeriod

    '==================================================================
    ' roTIMEPERIOD
    '
    ' Tipo de datos que contiene periodos de tiempo que incluyen:
    '       Inicio del periodo.
    '       Fin del periodo.
    '       Tiempo en el periodo (que no tiene porque ser el total).
    '
    '==================================================================

    Private mBegin As New roTime
    Private mFinish As New roTime

    Public Sub Clear()
        '
        ' Incializa clase
        '
        mBegin.Clear()
        mBegin.Clear()

    End Sub

    Public Function Clone() As roTimePeriod
        '
        ' Devuelve una copia de este objeto
        '
        Dim Cloned As New roTimePeriod

        Cloned.Begin.Value = Begin.Value
        Cloned.Finish.Value = Finish.Value

        Clone = Cloned

    End Function

    Public Function DebugText() As String
        '
        ' Funcion para debugar, devuelve los datos de este objeto en texto legible
        '
        DebugText = "[" & Begin.Value & "->" & Finish.Value & "]"

    End Function

    Public Function IsValid() As Boolean
        '
        ' Devuelve True si el periodo es valido
        '

        IsValid = (mBegin.IsValid And mFinish.IsValid)

    End Function

    Public Function Minutes() As Long
        '
        ' Devuelve los minutos contenidos
        '
        Dim auxMinutes As Long = 0
        If Not IsValid() Then
            Err.Raise(1350, "roTimePeriod", "Period must be initialized first.")
        Else
            auxMinutes = GetAny("n")
        End If
        Minutes = auxMinutes

    End Function

    Public Function PeriodTime() As roTime
        '
        ' Devuelve el intervalo de tiempo dentro del periodo
        '
        Dim myResult As New roTime

        If Not IsValid() Then
            Err.Raise(1350, "roTimePeriod", "Period must be initialized first.")
        Else
            myResult.Value = Date.FromOADate(mFinish.VBNumericValue - mBegin.VBNumericValue)
        End If

        PeriodTime = myResult

    End Function

    Public Function Seconds() As Long
        '
        ' Devuelve los segundos contenidos
        '
        Dim mySeconds As Long = 0
        If Not IsValid() Then
            Err.Raise(1350, "roTimePeriod", "Period must be initialized first.")
        Else
            mySeconds = GetAny("s")
        End If

        Seconds = mySeconds

    End Function

    Public Function Hours() As Long
        '
        ' Devuelve las horas contenidos
        '
        Dim myHours As Long
        If Not IsValid() Then
            Err.Raise(1350, "roTimePeriod", "Period must be initialized first.")
        Else
            myHours = GetAny("h")
        End If
        Hours = myHours

    End Function

    Public Function Days() As Long
        '
        ' Devuelve los dias contenidos
        '
        Dim myDays As Long = 0
        If Not IsValid() Then
            Err.Raise(1350, "roTimePeriod", "Period must be initialized first.")
        Else
            myDays = GetAny("d")
        End If
        Days = myDays

    End Function

    Private Function GetAny(ByVal Units As String) As Long
        '
        ' Devuelve cualquier unidad contenida en el periodo
        '
        GetAny = DateDiff(Units, mBegin.Value, mFinish.Value)

    End Function

    Public Property Begin() As roTime
        '
        ' Devuelve el valor de tiempo en formato de VB
        '
        Get
            Return mBegin
        End Get
        '
        ' Inicializa con el tiempo indicado
        '
        Set(ByVal value As roTime)
            mBegin = value
        End Set
    End Property

    Public Property Finish() As roTime
        '
        ' Devuelve el valor de tiempo en formato de VB
        '
        Get
            Return mFinish
        End Get
        '
        ' Inicializa con el tiempo indicado
        '
        Set(ByVal value As roTime)
            mFinish = value
        End Set
    End Property

End Class