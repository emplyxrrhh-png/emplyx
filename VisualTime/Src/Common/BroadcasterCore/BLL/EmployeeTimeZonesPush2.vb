Namespace BusinessLogicLayer

    Public Class EmployeeTimeZonesPush2

        Private _TZs As String
        Private _EmployeeID As Integer

        Public Sub New()
        End Sub

        Property TZs() As String
            Get
                Return _TZs
            End Get
            Set(ByVal value As String)
                _TZs = value
            End Set
        End Property

        Property EmployeeID() As Integer
            Get
                Return _EmployeeID
            End Get
            Set(ByVal value As Integer)
                _EmployeeID = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return _TZs.PadLeft(32, "0") + BCGlobal.KeyDBField + _EmployeeID.ToString.PadLeft(5, "0")
        End Function

    End Class

End Namespace