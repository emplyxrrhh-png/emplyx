Imports System.Drawing

Public Class roShift

    Private xShiftdate As DateTime
    Private intID As Integer
    Private strName As String
    Private oColor As Color
    Private strShortName As String

    Private oAlterShifts As roShift()

    Public Sub New(ByVal _Shiftdate As DateTime, ByVal _ID As Integer, ByVal _Name As String, ByVal _ShortName As String, ByVal _Color As Integer)

        Me.xShiftdate = _Shiftdate
        Me.intID = _ID
        Me.strName = _Name
        Me.strShortName = _ShortName
        'oColor = Color.FromArgb(_Color)

        Dim r, g, b As Byte

        r = _Color And 255
        g = (_Color \ 256) And 255
        b = (_Color \ 65536) And 255

        oColor = Color.FromArgb(r, g, b)

        ReDim Me.oAlterShifts(-1)

    End Sub

    Public ReadOnly Property Shiftdate() As DateTime
        Get
            Return Me.xShiftdate
        End Get
    End Property

    Public ReadOnly Property ID() As Integer
        Get
            Return Me.intID
        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return Me.strName
        End Get
    End Property

    Public ReadOnly Property ShortName() As String
        Get
            Return Me.strShortName
        End Get
    End Property

    Public ReadOnly Property Color() As Color
        Get
            Return Me.oColor
        End Get
    End Property

    Public ReadOnly Property AlterShifts() As roShift()
        Get
            Return Me.oAlterShifts
        End Get
    End Property

    Public Sub AddAlterShift(ByVal _Shiftdate As DateTime, ByVal _ID As Integer, ByVal _Name As String, ByVal _ShortName As String, ByVal _Color As Integer)

        ReDim Preserve Me.oAlterShifts(Me.oAlterShifts.Length)

        Me.oAlterShifts(Me.oAlterShifts.Length - 1) = New roShift(_Shiftdate, _ID, _Name, _ShortName, _Color)

    End Sub

End Class