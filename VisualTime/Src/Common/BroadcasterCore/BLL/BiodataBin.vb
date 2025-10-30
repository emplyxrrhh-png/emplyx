Namespace BusinessLogicLayer

    ''' <summary>
    ''' Objeto para modelar distintos tipos de versiones de biometría: Huellas, caras, palmas de la mano
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BiodataBin

        Private _EmployeeID As Integer
        Private _FingerID As String
        Private _FingerData() As Byte
        Private _NewFinger As Boolean
        Private _Version As String = String.Empty

        ''' <summary>
        ''' Identificador del usuario
        ''' </summary>
        ''' <value>Identificador del usuario</value>
        ''' <returns>Identificador del usuario</returns>
        ''' <remarks></remarks>
        Property EmployeeID() As Integer
            Get
                Return _EmployeeID
            End Get
            Set(ByVal value As Integer)
                _EmployeeID = value
            End Set
        End Property

        Property NewFinger() As Boolean
            Get
                Return _NewFinger
            End Get
            Set(ByVal value As Boolean)
                _NewFinger = value
            End Set
        End Property

        ''' <summary>
        ''' Identificador de dedo del usuario
        ''' </summary>
        ''' <value>Identificador de dedo del usuario</value>
        ''' <returns>Identificador de dedo del usuario</returns>
        ''' <remarks></remarks>
        Property FingerID() As Integer
            Get
                Return _FingerID
            End Get
            Set(ByVal value As Integer)
                _FingerID = value
            End Set
        End Property

        ''' <summary>
        ''' Estructura de la huella
        ''' </summary>
        ''' <value>Estructura de la huella</value>
        ''' <returns>Estructura de la huella</returns>
        ''' <remarks></remarks>
        Property FingerData() As Byte()
            Get
                Return _FingerData
            End Get
            Set(ByVal value As Byte())
                _FingerData = value
            End Set
        End Property

        ''' <summary>
        ''' Versión de biometría (para incluir terminales con huella, cara, palma de la mano, ...
        ''' </summary>
        ''' <returns></returns>
        Property Version As String
            Get
                Return _Version
            End Get
            Set(ByVal value As String)
                _Version = value
            End Set
        End Property

        ''' <summary>
        ''' Devuelve los datos a enviar al terminal independiente mente si es una alta o baja.
        ''' </summary>
        ''' <returns>Datos del mensaje al terminal</returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return _EmployeeID.ToString + ";" + _FingerID.ToString + ";" + _Version.ToString
        End Function

    End Class

End Namespace