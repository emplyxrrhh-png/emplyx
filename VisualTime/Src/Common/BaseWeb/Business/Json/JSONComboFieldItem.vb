Imports System.Runtime.Serialization

<DataContract()>
Public Class JSONComboFieldItem

    Private m_Text As String
    Private m_Value As String
    Private m_JSFunction As String

    Public Sub New()
        m_Text = ""
        m_Value = ""
        m_JSFunction = ""
    End Sub

    Public Sub New(ByVal xText As String, ByVal xValue As String, ByVal xJSFunction As String)
        m_Text = xText
        m_Value = xValue
        m_JSFunction = xJSFunction
    End Sub

    <DataMember()>
    Public Property Text() As String
        Get
            Return m_Text
        End Get
        Set(ByVal value As String)
            m_Text = value
        End Set
    End Property

    <DataMember()>
    Public Property [Value]() As String
        Get
            Return m_Value
        End Get
        Set(ByVal value As String)
            m_Value = value
        End Set
    End Property

    <DataMember()>
    Public Property JSFunction() As String
        Get
            Return m_JSFunction
        End Get
        Set(ByVal value As String)
            m_JSFunction = value
        End Set
    End Property

    Public Function toJSON() As String
        Dim strJSON As String = ""

        If m_Value = "" Then
            m_Value = m_Text
        End If
        Return m_Text.Replace("'", "\'") & "~*~" & m_Value.Replace("'", "\'") & "~*~" & m_JSFunction.Replace("'", "\'")
    End Function

End Class