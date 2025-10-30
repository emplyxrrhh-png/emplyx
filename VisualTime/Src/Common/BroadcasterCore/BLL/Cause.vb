Namespace BusinessLogicLayer

    Public Class Cause

        Private _CauseID As Integer
        Private _CauseName As String
        Private _ReaderInputCode As Integer
        Private _WorkingType As Boolean

        Public Property CauseID() As Integer
            Get
                Return Me._CauseID
            End Get
            Set(ByVal value As Integer)
                Me._CauseID = value
            End Set
        End Property

        Public Property CauseName() As String
            Get
                Return Me._CauseName
            End Get
            Set(ByVal value As String)
                Me._CauseName = value
            End Set
        End Property

        Public Property ReaderInputCode() As Integer
            Get
                Return Me._ReaderInputCode
            End Get
            Set(ByVal value As Integer)
                Me._ReaderInputCode = value
            End Set
        End Property

        Public Property WorkingType() As Boolean
            Get
                Return Me._WorkingType
            End Get
            Set(ByVal value As Boolean)
                Me._WorkingType = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me._CauseID.ToString.PadLeft(5, "0") + BCGlobal.KeyDBField + Me._CauseName.ToString + BCGlobal.KeyDBField + Me._ReaderInputCode.ToString + BCGlobal.KeyDBField + Me._WorkingType.ToString
        End Function

    End Class

End Namespace