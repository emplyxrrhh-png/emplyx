Public Class clsEmployeeData

    Private _ID As Integer = 0
    Private _Name As String = ""
    Private _PIN As String = ""
    Private _AllowCard As Boolean = True
    Private _AllowBio As Boolean = True
    Private _AllowCardBio As Boolean = False
    Private _AllowPIN As Boolean = True
    Private _OHPDocuments As List(Of Database.clsDocumentData) = New List(Of Database.clsDocumentData)
    Private _HasOHPInvalid As Boolean = False
    Private _HasOHPWarning As Boolean = False

    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal Value As Integer)
            _ID = Value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal Value As String)
            _Name = Value
        End Set
    End Property

    Public ReadOnly Property Photo() As String
        Get
            Return _ID.ToString
        End Get
    End Property

    Public Property PIN() As String
        Get
            Return _PIN
        End Get
        Set(ByVal Value As String)
            _PIN = Value
        End Set
    End Property

    Public Property AllowCard() As Boolean
        Get
            Return _AllowCard
        End Get
        Set(ByVal Value As Boolean)
            _AllowCard = Value
        End Set
    End Property

    Public Property AllowBio() As Boolean
        Get
            Return _AllowBio
        End Get
        Set(ByVal Value As Boolean)
            _AllowBio = Value
        End Set
    End Property

    Public Property AllowCardBio() As Boolean
        Get
            Return _AllowCardBio
        End Get
        Set(ByVal Value As Boolean)
            _AllowCardBio = Value
        End Set
    End Property

    Public Property AllowPIN() As Boolean
        Get
            Return _AllowPIN
        End Get
        Set(ByVal Value As Boolean)
            _AllowPIN = Value
        End Set
    End Property

    Public Property OHPDocument() As List(Of Database.clsDocumentData)
        Get
            Return _OHPDocuments
        End Get
        Set(ByVal value As List(Of Database.clsDocumentData))
            _OHPDocuments = value
            OHP_Check()
        End Set
    End Property

    Public ReadOnly Property HasOHPInvalid() As Boolean
        Get
            Return _HasOHPInvalid
        End Get
    End Property

    Public ReadOnly Property HasOHPWarning() As Boolean
        Get
            Return _HasOHPWarning
        End Get
    End Property

    Public Sub New()

    End Sub

    Private Sub OHP_Check()
        Try
            _HasOHPInvalid = False
            _HasOHPWarning = False
            If _ID > 0 Then
                For Each data As Database.clsDocumentData In _OHPDocuments
                    If Not data.Valid Then
                        If data.DenyAccess Then
                            _HasOHPInvalid = True
                        Else
                            _HasOHPWarning = True
                        End If
                    End If
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub


End Class
