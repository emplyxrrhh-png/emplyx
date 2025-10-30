Namespace BusinessLogicLayer

    Public Class TimeZoneZKPush2

        Private _ID As Integer
        Private _BeginTime(10) As Date
        Private _EndTime(10) As Date

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

        Public ReadOnly Property BeginTime(ByVal Index As eDay) As Date
            Get
                Return _BeginTime(Index - 1)
            End Get
        End Property

        Public ReadOnly Property EndTime(ByVal Index As eDay) As Date
            Get
                Return _EndTime(Index - 1)
            End Get
        End Property

        Public Sub New()
            Dim i As Byte
            For i = 0 To 9
                _BeginTime(i) = Date.Parse("00:01")
                _EndTime(i) = Date.Parse("00:00")
            Next
        End Sub

        Public Sub add(ByVal weekday As eDay, ByVal BeginTime As Date, ByVal EndTime As Date)
            _BeginTime(weekday - 1) = BeginTime
            _EndTime(weekday - 1) = EndTime
        End Sub

        Public Function Exist(ByVal weekday As eDay, ByVal BeginTime As Date, ByVal EndTime As Date) As Boolean
            If (_BeginTime(weekday - 1).Equals(BeginTime) AndAlso _EndTime(weekday - 1).Equals(EndTime)) Then
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

        Public Function Remove(iDayOfWeek As Byte) As Boolean
            Try
                _BeginTime(iDayOfWeek - 1) = Date.Parse("00:01")
                _EndTime(iDayOfWeek - 1) = Date.Parse("00:00")
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim i As Byte
            Dim oTz As TimeZoneZKPush2
            Try
                oTz = CType(obj, TimeZoneZKPush2)
                For i = 0 To 9
                    If Not ((_BeginTime(i).Equals(oTz.BeginTime(i)) AndAlso _EndTime(i).Equals(oTz.EndTime(i)))) Then
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