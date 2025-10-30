Imports Robotics.VTBase.roTypes

Public Class roTime

    '==================================================================
    ' roTIME
    '
    ' Tipo de datos que contiene tiempo con funciones extendidas.
    '
    '==================================================================

    Private mVal As Object
    Private mPrecission As Object

    Public Sub New()
        '
        ' Incializa clase
        '
        mPrecission = Nothing

    End Sub

    Public Sub New(ByVal xValue As DateTime)

        mPrecission = Nothing
        Me.Value = xValue

    End Sub

    Public Function SQLDateTime() As String
        '
        ' Devuelve un string con el DateTime del valor
        '  listo para usar en una sentencia SQL.
        '
        Dim strRet As String = ""

        If Not IsValid() Then
            Err.Raise(1250, "roTime", "Time must be initialized first.")
        Else
            strRet = "CONVERT(DATETIME,'" & Format(CDate(mVal), "yyyy/MM/dd") & " " &
                     Format$(Hour(mVal), "00") & ":" &
                     Format$(Minute(mVal), "00") & ":" &
                     Format$(Second(mVal), "00") & " ',120)"
        End If

        Return strRet

    End Function

    Public Function SQLSmallDateTime() As String
        '
        ' Devuelve un string con el DateTime del valor
        '  listo para usar en una sentencia SQL.
        '
        Dim strRet As String = ""
        If Not IsValid() Then
            Err.Raise(1250, "roTime", "Time must be initialized first.")
        Else
            strRet = "CONVERT(SMALLDATETIME,'" & Format(CDate(mVal), "yyyy/MM/dd") & " " &
                Format$(Hour(mVal), "00") & ":" &
                Format$(Minute(mVal), "00") & " ',120)"
        End If

        Return strRet

    End Function

    Public Function Add(ByVal Value As Object, Optional ByVal Interval As String = "") As roTime
        '
        ' Suma el tiempo indicado al actual.
        '  El tiempo indicado puede ser un roTime, un date/time o bien
        '  un numero de minutos, segundos... en este caso es necesario indicar
        '  el tipo de intervalo.
        '
        Dim myResult As New roTime

        If Not IsValid() Then
            Err.Raise(1250, "roTime", "Time must be initialized first.")
        Else
            If TypeOf Value Is roTime Then
                myResult.Value = DateTimeAdd(mVal, Value.Value, "n")
            ElseIf IsDate(Value) Then
                myResult.Value = DateTimeAdd(mVal, Value, "n")
            ElseIf IsNumeric(Value) Then
                If Interval = "" Then
                    myResult.Value = Date.FromOADate(VBNumericValue() + Value)
                Else
                    myResult.Value = DateAdd(Interval, Value, mVal)
                End If
            Else
                Err.Raise(1259, "roTime", "Add: Invalid value.")
            End If
        End If

        Add = myResult

    End Function

    Public Function Substract(ByVal Value As Object, Optional ByVal Interval As String = "") As roTime
        '
        ' Resta el tiempo indicado al actual.
        '  El tiempo indicado puede ser un roTime, un date/time o bien
        '  un numero de minutos, segundos... en este caso es necesario indicar
        '  el tipo de intervalo.
        '
        Dim myResult As New roTime

        If Not IsValid() Then
            Err.Raise(1250, "roTime", "Time must be initialized first.")
        Else
            If TypeOf Value Is roTime Then
                myResult.Value = DateTimeSubstract(mVal, Value.Value, "n")
            ElseIf IsDate(Value) Then
                myResult.Value = DateTimeSubstract(mVal, Value, "n")
            ElseIf IsNumeric(Value) Then
                If Interval = "" Then
                    ' Numeric in VB format
                    myResult.Value = Date.FromOADate(VBNumericValue() - Value)
                Else
                    myResult.Value = DateAdd(Interval, -Value, mVal)
                End If
            Else
                Err.Raise(1269, "roTime", "Substract: Invalid value.")
            End If
        End If

        Substract = myResult

    End Function

    Public Sub Clear()
        '
        ' Inicializa el valor
        '
        mVal = Nothing
        mPrecission = Nothing

    End Sub

    Public Function Precission() As Object
        '
        ' Devuelve la precision del valor almacenado
        '

        ' Recalcula precision si es necesario
        '  (esto solo se hace despues de modificar el valor)

        If mPrecission Is Nothing Then
            If mVal Is Nothing Then
                mPrecission = "s"
            ElseIf Second(mVal) <> 0 Then
                mPrecission = "s"
            ElseIf Minute(mVal) <> 0 Then
                mPrecission = "n"
            ElseIf Hour(mVal) <> 0 Then
                mPrecission = "h"
            Else
                mPrecission = "d"
            End If
        End If

        Precission = mPrecission

    End Function

    Public Function DateOnly() As Object
        '
        ' Devuelve solo la fecha, sin horas minutos ni segundos
        '
        DateOnly = DateTime2Date(mVal)

    End Function

    Public Function TimeOnly() As Object
        '
        ' Devuelve solo el tiempo, sin fecha
        '
        TimeOnly = DateTime2Time(mVal)

    End Function

    Private Function GetAny(ByVal Units As String) As Long
        '
        ' Devuelve el numero de minutos, segundos, horas, lo que se quiera.
        '
        If IsValid() Then
            GetAny = DateDiff(Units, "00:00", mVal)
        Else
            GetAny = 0
        End If

    End Function

    Public Function IsValid() As Boolean
        '
        ' Devuelve True si el valor almacenado es válido.
        '
        IsValid = IsDate(mVal)

    End Function

    Public Function Minutes() As Long
        '
        ' Devuelve el numero de minutos
        '
        Minutes = GetAny("n")

    End Function

    Public Function Hours() As Long
        '
        ' Devuelve el numero de horas
        '
        Hours = GetAny("h")

    End Function

    Public Function NumericValue(Optional EngineData As Boolean = False) As Double
        '
        ' Devuelve un numero con horas y centesimas de horas
        '
        If IsValid() Then
            Dim mDateDiff As Double = DateDiff("d", "00:00", CDate(mVal).Date)

            'If EngineData Then
            If (CDate(mVal).Year > 20 AndAlso CDate(mVal).Year < 1920) OrElse EngineData Then
                mDateDiff = DateDiff("d", "1899/12/30", CDate(mVal).Date)
            End If

            NumericValue = (Second(mVal) / 3600) + (Minute(mVal) / 60) + Hour(mVal) + mDateDiff * 24
        Else
            NumericValue = 0
        End If

    End Function

    Public Function VBNumericValue() As Double
        '
        ' Devuelve un numero en formato double reconocido como fecha por VisualBasic
        '
        If IsValid() Then
            VBNumericValue = Any2Double(mVal)
        Else
            VBNumericValue = 0
        End If

    End Function

    Public Function Seconds() As Long
        '
        ' Devuelve el numero de segundos
        '
        Seconds = GetAny("s")

    End Function

    Public Function Days() As Long
        '
        ' Devuelve el numero de dias
        '
        Days = GetAny("d")

    End Function

    Public Property Value() As Object
        '
        ' Devuelve el valor de tiempo en formato de VB
        '
        Get
            Return mVal
        End Get
        '
        ' Inicializa con el tiempo indicado
        '
        Set(ByVal value As Object)

            If Not IsDate(value) Then Err.Raise(1257, "roTime", "Invalid value.")

            mVal = Format$(CDate(value), "yyyy/MM/dd HH:mm:ss")
            mPrecission = Nothing

        End Set
    End Property

    Public ReadOnly Property ValueDateTime() As DateTime
        Get
            Return New DateTime(Year(mVal), Month(mVal), Day(mVal), Hour(mVal), Minute(mVal), Second(mVal))
        End Get
    End Property

    Public Function CrystalDateTime() As String
        '
        ' Devuelve un string con el DateTime del valor
        '  listo para usar en una sentencia Crystal.
        '
        Dim strRet As String = ""

        If Not IsValid() Then
            Err.Raise(1250, "roTime", "Time must be initialized first.")
        Else
            strRet = "DateTime (" & Format$(Year(mVal), "00") & ", " &
            Format$(Month(mVal), "00") & ", " &
            Format$(Day(mVal), "00") & ", " &
            Format$(Hour(mVal), "00") & ", " &
            Format$(Minute(mVal), "00") & ", " &
            Format$(Second(mVal), "00") & ")"
        End If

        Return strRet

    End Function

    Public Function RoundTime(ByVal TotalMinutes As Double, ByVal RoundingType As String, ByVal RoundingBy As Integer) As Double
        ' Redondea un tiempo dado en minutos

        Dim RoundedMinutes As Double
        Dim Divergence As Double
        Dim Base As Double

        Try
            Divergence = TotalMinutes Mod RoundingBy
            Base = TotalMinutes - Divergence

            Select Case RoundingType
                Case "~"   ' Por aproximación
                    RoundedMinutes = Base
                    If Divergence > (RoundingBy / 2) Then RoundedMinutes = RoundedMinutes + RoundingBy

                Case "+"   ' Por exceso
                    RoundedMinutes = Base
                    If Divergence > 0 Then RoundedMinutes = RoundedMinutes + RoundingBy

                Case "-"   ' Por defecto
                    RoundedMinutes = Base
            End Select
        Catch ex As Exception
        End Try

        Return RoundedMinutes

    End Function

End Class