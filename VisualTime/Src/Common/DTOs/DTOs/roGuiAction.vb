Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roGuiAction
        Protected _strIDPath As String
        Protected _strIDGuiPath As String
        Protected _strLanguageTag As String
        Protected _text As String
        Protected _strAfterFunction As String
        Protected _strCssClass As String
        Protected _iSection As Integer
        Protected _iOrder As Integer
        Protected _bAppearsOnPopup As Boolean

        Public Sub New()
            _strIDPath = String.Empty
            _strIDGuiPath = String.Empty
            _strLanguageTag = String.Empty
            _strAfterFunction = String.Empty
            _strCssClass = String.Empty
            _iSection = -1
            _iOrder = -1
            _bAppearsOnPopup = False
        End Sub

        <DataMember()>
        Public Property IDPath As String
            Get
                Return _strIDPath
            End Get
            Set(value As String)
                _strIDPath = value
            End Set
        End Property

        <DataMember()>
        Public Property IDGuiPath As String
            Get
                Return _strIDGuiPath
            End Get
            Set(value As String)
                _strIDGuiPath = value
            End Set
        End Property

        <DataMember()>
        Public Property LanguageTag As String
            Get
                Return _strLanguageTag
            End Get
            Set(value As String)
                _strLanguageTag = value
            End Set
        End Property

        <DataMember()>
        Public Property AfterFunction As String
            Get
                Return _strAfterFunction
            End Get
            Set(value As String)
                _strAfterFunction = value
            End Set
        End Property

        <DataMember()>
        Public Property CssClass As String
            Get
                Return _strCssClass
            End Get
            Set(value As String)
                _strCssClass = value
            End Set
        End Property

        <DataMember()>
        Public Property Section As Integer
            Get
                Return _iSection
            End Get
            Set(value As Integer)
                _iSection = value
            End Set
        End Property

        <DataMember()>
        Public Property Order As Integer
            Get
                Return _iOrder
            End Get
            Set(value As Integer)
                _iOrder = value
            End Set
        End Property

        <DataMember()>
        Public Property AppearsOnPopup As Boolean
            Get
                Return _bAppearsOnPopup
            End Get
            Set(value As Boolean)
                _bAppearsOnPopup = value
            End Set
        End Property

        <DataMember()>
        Public Property Text As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
            End Set
        End Property

    End Class

End Namespace