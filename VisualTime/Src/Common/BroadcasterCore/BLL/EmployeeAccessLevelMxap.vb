Namespace BusinessLogicLayer

    Public Class EmployeeAccessLevelMxap

        Private _IDEmployee As Integer
        Private _IDTimeZone As Integer
        Private _Door1 As Boolean = False
        Private _Door2 As Boolean = False
        Private _Door3 As Boolean = False
        Private _Door4 As Boolean = False
        Private _NewEmployeeAccesLevel As Boolean = False

        Public Property IDEmployee As Integer
            Get
                Return _IDEmployee
            End Get
            Set(value As Integer)
                _IDEmployee = value
            End Set
        End Property
        Public Property IDTimeZone As Integer
            Get
                Return _IDTimeZone
            End Get
            Set(value As Integer)
                _IDTimeZone = value
            End Set
        End Property

        Public Property Door1 As Boolean
            Get
                Return _Door1
            End Get
            Set(value As Boolean)
                _Door1 = value
            End Set
        End Property

        Public Property Door2 As Boolean
            Get
                Return _Door2
            End Get
            Set(value As Boolean)
                _Door2 = value
            End Set
        End Property

        Public Property Door3 As Boolean
            Get
                Return _Door3
            End Get
            Set(value As Boolean)
                _Door3 = value
            End Set
        End Property

        Public Property Door4 As Boolean
            Get
                Return _Door4
            End Get
            Set(value As Boolean)
                _Door4 = value
            End Set
        End Property

        Public Property NewEmployeeAccesLevel() As Boolean
            Get
                Return _NewEmployeeAccesLevel
            End Get
            Set(ByVal value As Boolean)
                _NewEmployeeAccesLevel = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return "Employee: " + _IDEmployee.ToString + ",IDAccessperiod: " + _IDTimeZone.ToString + ",Door1: " + IIf(_Door1, " Allowed ", " Denied ") + ",Door2: " + IIf(_Door2, " Allowed ", " Denied ") + ",Door3: " + IIf(_Door3, " Allowed ", " Denied ") + ",Door4: " + IIf(_Door4, " Allowed ", " Denied ")
        End Function

    End Class

End Namespace