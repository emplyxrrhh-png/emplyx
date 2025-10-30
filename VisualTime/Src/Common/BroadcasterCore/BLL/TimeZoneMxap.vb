Namespace BusinessLogicLayer

    Public Class TimeZoneMxap

        Private _ID As Integer
        Private _BeginTime1(10) As Date
        Private _EndTime1(10) As Date
        Private _BeginTime2(10) As Date
        Private _EndTime2(10) As Date
        Private _BeginTime3(10) As Date
        Private _EndTime3(10) As Date
        Private _LastSlotUsed(10) As Byte

        Public Enum eDay
            monday = 1
            tuesday = 2
            wednesday = 3
            thursday = 4
            friday = 5
            sarturday = 6
            sunday = 7
            holiday1 = 8
            holiday2 = 9
            holiday3 = 10
        End Enum

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Public ReadOnly Property BeginTime1(ByVal Index As eDay) As Date
            Get
                Return _BeginTime1(Index - 1)
            End Get
        End Property

        Public ReadOnly Property EndTime1(ByVal Index As eDay) As Date
            Get
                Return _EndTime1(Index - 1)
            End Get
        End Property

        Public ReadOnly Property BeginTime2(ByVal Index As eDay) As Date
            Get
                Return _BeginTime2(Index - 1)
            End Get
        End Property

        Public ReadOnly Property EndTime2(ByVal Index As eDay) As Date
            Get
                Return _EndTime2(Index - 1)
            End Get
        End Property

        Public ReadOnly Property BeginTime3(ByVal Index As eDay) As Date
            Get
                Return _BeginTime3(Index - 1)
            End Get
        End Property

        Public ReadOnly Property EndTime3(ByVal Index As eDay) As Date
            Get
                Return _EndTime3(Index - 1)
            End Get
        End Property

        Public Sub New()
            Dim i As Byte
            For i = 0 To 9
                _BeginTime1(i) = Date.Parse("00:01")
                _EndTime1(i) = Date.Parse("00:00")
                _BeginTime2(i) = Date.Parse("00:01")
                _EndTime2(i) = Date.Parse("00:00")
                _BeginTime3(i) = Date.Parse("00:01")
                _EndTime3(i) = Date.Parse("00:00")
                _LastSlotUsed(i) = 0
            Next
        End Sub

        Public Sub add(ByVal weekday As eDay, ByVal BeginTime As Date, ByVal EndTime As Date)
            Dim slot As Byte
            slot = _LastSlotUsed(weekday - 1)
            Select Case slot
                Case 0
                    _BeginTime1(weekday - 1) = BeginTime
                    _EndTime1(weekday - 1) = EndTime
                    _LastSlotUsed(weekday - 1) = 1
                Case 1
                    _BeginTime2(weekday - 1) = BeginTime
                    _EndTime2(weekday - 1) = EndTime
                    _LastSlotUsed(weekday - 1) = 2
                Case 2
                    _BeginTime3(weekday - 1) = BeginTime
                    _EndTime3(weekday - 1) = EndTime
                    _LastSlotUsed(weekday - 1) = 3
                Case Else
                    ' No cabe ...
            End Select
        End Sub

        Public Function SlotsInDay(ByVal weekday As eDay) As Byte
            Return _LastSlotUsed(weekday - 1)
        End Function

        Public Function Exist(ByVal weekday As eDay, ByVal BeginTime As Date, ByVal EndTime As Date) As Boolean
            If (_BeginTime1(weekday - 1).Equals(BeginTime) AndAlso _EndTime1(weekday - 1).Equals(EndTime)) OrElse (_BeginTime2(weekday - 1).Equals(BeginTime) AndAlso _EndTime2(weekday - 1).Equals(EndTime)) OrElse (_BeginTime3(weekday - 1).Equals(BeginTime) AndAlso _EndTime3(weekday - 1).Equals(EndTime)) Then
                Return True
            End If
            Return False
        End Function

        Public Overrides Function ToString() As String
            'Dim sTMP As String = ""
            'Dim i As Byte
            'sTMP = _TzID.ToString.PadLeft(5, "0") + BCGlobal.KeyDBField
            'For i = 0 To 6
            '    sTMP += _BeginTime(i).Hour.ToString.PadRight(2, "0") + _BeginTime(i).Minute.ToString.PadRight(2, "0")
            '    sTMP += _EndTime(i).Hour.ToString.PadRight(2, "0") + _EndTime(i).Minute.ToString.PadRight(2, "0")
            'Next
            'Return sTMP
            Return "..."
        End Function

        Public Function Remove(idTimeZone As Integer, iDayOfWeek As Byte) As Boolean
            Try
                _BeginTime1(iDayOfWeek - 1) = Date.Parse("00:01")
                _EndTime1(iDayOfWeek - 1) = Date.Parse("00:00")
                _BeginTime2(iDayOfWeek - 1) = Date.Parse("00:01")
                _EndTime2(iDayOfWeek - 1) = Date.Parse("00:00")
                _BeginTime3(iDayOfWeek - 1) = Date.Parse("00:01")
                _EndTime3(iDayOfWeek - 1) = Date.Parse("00:00")
                _LastSlotUsed(iDayOfWeek - 1) = 0
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim i As Byte
            Dim oTz As TimeZoneMxap
            Try
                oTz = CType(obj, TimeZoneMxap)
                For i = 0 To 9
                    If Not ((_BeginTime1(i).Equals(oTz.BeginTime1(i)) AndAlso _EndTime1(i).Equals(oTz.EndTime1(i))) AndAlso (_BeginTime2(i).Equals(oTz.BeginTime2(i)) AndAlso _EndTime2(i).Equals(oTz.EndTime2(i))) AndAlso (_BeginTime3(i).Equals(oTz.BeginTime3(i)) AndAlso _EndTime3(i).Equals(oTz.EndTime3(i)))) Then
                        Return False
                    End If
                Next
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class

End Namespace