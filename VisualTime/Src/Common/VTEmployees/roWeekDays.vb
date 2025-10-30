Imports System.Runtime.Serialization

Namespace LabAgree

    <DataContract()>
    <Serializable()>
    Public Class roWeekDays

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intID As Integer
        Private intWeek As String

        Private strMonday As String
        Private strTuesday As String
        Private strWednesday As String
        Private strThursday As String
        Private strFriday As String
        Private strSaturday As String
        Private strSunday As String

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember()>
        Public Property Day1() As String
            Get
                Return Me.strMonday
            End Get
            Set(ByVal value As String)
                Me.strMonday = value
            End Set
        End Property

        <DataMember()>
        Public Property Week() As String
            Get
                Return Me.intWeek
            End Get
            Set(ByVal value As String)
                Me.intWeek = value
            End Set
        End Property

        <DataMember()>
        Public Property Day2() As String
            Get
                Return Me.strTuesday
            End Get
            Set(ByVal value As String)
                Me.strTuesday = value
            End Set
        End Property

        <DataMember()>
        Public Property Day3() As String
            Get
                Return Me.strWednesday
            End Get
            Set(ByVal value As String)
                Me.strWednesday = value
            End Set
        End Property

        <DataMember()>
        Public Property Day4() As String
            Get
                Return Me.strThursday
            End Get
            Set(ByVal value As String)
                Me.strThursday = value
            End Set
        End Property
        <DataMember()>
        Public Property Day5() As String
            Get
                Return Me.strFriday
            End Get
            Set(ByVal value As String)
                Me.strFriday = value
            End Set
        End Property

        <DataMember()>
        Public Property Day6() As String
            Get
                Return Me.strSaturday
            End Get
            Set(ByVal value As String)
                Me.strSaturday = value
            End Set
        End Property

        <DataMember()>
        Public Property Day0() As String
            Get
                Return Me.strSunday
            End Get
            Set(ByVal value As String)
                Me.strSunday = value
            End Set
        End Property

#End Region

    End Class

End Namespace