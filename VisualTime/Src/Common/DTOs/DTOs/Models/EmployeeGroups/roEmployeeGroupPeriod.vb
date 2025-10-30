Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class EmployeeGroupPeriod
        Private _IDEmployee As Integer
        Private _IDGroup As Integer
        Private _BeginDate As Date
        Private _EndDate As Date

        <DataMember>
        Public Property IDEmployee As Integer
            Get
                Return _IDEmployee
            End Get
            Set(value As Integer)
                _IDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property IDGroup As Integer
            Get
                Return _IDGroup
            End Get
            Set(value As Integer)
                _IDGroup = value
            End Set
        End Property

        <DataMember>
        Public Property BeginDate As Date
            Get
                Return _BeginDate
            End Get
            Set(value As Date)
                _BeginDate = value
            End Set
        End Property

        <DataMember>
        Public Property EndDate As Date
            Get
                Return _EndDate
            End Get
            Set(value As Date)
                _EndDate = value
            End Set
        End Property

        Public Sub New(idEmployee As Integer, idGroup As Integer, dBeginDate As Date, dEndDate As Date)
            _IDEmployee = idEmployee
            _IDGroup = idGroup
            _BeginDate = dBeginDate
            _EndDate = dEndDate
        End Sub

    End Class

End Namespace