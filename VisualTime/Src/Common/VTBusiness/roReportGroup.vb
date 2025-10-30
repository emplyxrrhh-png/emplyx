Namespace Concept

    Public Class roReportGroup
        Dim intID As Integer
        Dim strName As String
        Dim oConcepts As New ArrayList

        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        Public Property Concepts() As ArrayList
            Get
                Return oConcepts
            End Get
            Set(ByVal value As ArrayList)
                oConcepts = value
            End Set
        End Property

    End Class

    Public Class roReportGroupsConcepts
        Dim IntIDReportGroup As Integer
        Dim intIDConcept As Integer
        Dim intPosition As Integer

        Public Property IDReportGroup() As Integer
            Get
                Return IntIDReportGroup
            End Get
            Set(ByVal value As Integer)
                IntIDReportGroup = value
            End Set
        End Property

        Public Property IDConcept() As Integer
            Get
                Return intIDConcept
            End Get
            Set(ByVal value As Integer)
                intIDConcept = value
            End Set
        End Property

        Public Property Position() As Integer
            Get
                Return intPosition
            End Get
            Set(ByVal value As Integer)
                intPosition = value
            End Set
        End Property

    End Class

End Namespace