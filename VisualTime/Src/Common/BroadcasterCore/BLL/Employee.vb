Imports System.Drawing

Namespace BusinessLogicLayer

    Public Class Employee

        Private _EmployeeID As Integer
        Private _EmployeeName As String
        Private _EmployeeImage As Image
        Private _EmployeeImageCRC As String = String.Empty
        Private _EmployeeLanguageKey As String
        Private _NewEmployee As Boolean
        Private _PIN As String
        Private _MergeMethod As Integer
        Private _AllowedCauses As String
        Private _IsOnline As Boolean = False
        Private _ConsentRequired As Boolean = False

        Public Property EmployeeID() As Integer
            Get
                Return Me._EmployeeID
            End Get
            Set(ByVal value As Integer)
                Me._EmployeeID = value
            End Set
        End Property

        Public Property EmployeeName() As String
            Get
                Return Me._EmployeeName
            End Get
            Set(ByVal value As String)
                Me._EmployeeName = value
            End Set
        End Property

        Public Property EmployeeImage() As Image
            Get
                Return Me._EmployeeImage
            End Get
            Set(ByVal value As Image)
                Me._EmployeeImage = value
            End Set
        End Property

        Public Property EmployeeImageCRC() As String
            Get
                Return Me._EmployeeImageCRC
            End Get
            Set(ByVal value As String)
                Me._EmployeeImageCRC = value
            End Set
        End Property

        Public Property EmployeeLanguageKey() As String
            Get
                Return Me._EmployeeLanguageKey
            End Get
            Set(ByVal value As String)
                Me._EmployeeLanguageKey = value
            End Set
        End Property

        Public Property NewEmployee() As String
            Get
                Return Me._NewEmployee
            End Get
            Set(ByVal value As String)
                Me._NewEmployee = value
            End Set
        End Property

        Public Property PIN() As String
            Get
                Return Me._PIN
            End Get
            Set(ByVal value As String)
                Me._PIN = value
            End Set
        End Property

        Public Property MergeMethod() As Integer
            Get
                Return Me._MergeMethod
            End Get
            Set(ByVal value As Integer)
                Me._MergeMethod = value
            End Set
        End Property

        Public Property AllowedCauses() As String
            Get
                Return _AllowedCauses
            End Get
            Set(ByVal value As String)
                _AllowedCauses = value
            End Set
        End Property

        ''' <summary>
        ''' Indica si cuando fiche este empleado en terminal mx8+ se debe consultar al servidor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsOnline() As Boolean
            Get
                Return _IsOnline
            End Get
            Set(ByVal value As Boolean)
                _IsOnline = value
            End Set
        End Property

        Public Property ConsentRequired() As Boolean
            Get
                Return _ConsentRequired
            End Get
            Set(ByVal value As Boolean)
                _ConsentRequired = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me._EmployeeID.ToString.PadLeft(5, "0") + BCGlobal.KeyDBField + Me._EmployeeName.ToString
        End Function

    End Class

End Namespace