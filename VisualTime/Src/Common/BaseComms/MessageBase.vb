Public MustInherit Class CMessageBase

#Region "Declarations - Constructor"

    Private strDriver As String

    Public Sub New(ByVal _Driver As String)
        Me.strDriver = _Driver
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property Driver() As String
        Get
            Return Me.strDriver
        End Get
    End Property

    Public Overridable ReadOnly Property IsCorrect() As Boolean
        Get
        End Get
    End Property

#End Region

#Region "Methods"

    Public MustOverride Function Parse(ByRef bInput() As Byte) As Boolean

    Public MustOverride Function ToDebugInfo() As String

    Public MustOverride Function ToBytes() As Byte()

#End Region

End Class