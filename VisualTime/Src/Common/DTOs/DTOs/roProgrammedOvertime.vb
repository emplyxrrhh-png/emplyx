Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa una prevision de horas de exceso
    ''' </summary>
    <DataContract>
    Public Class roProgrammedOvertimeStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de horas de exceso
    ''' </summary>
    <DataContract>
    Public Class roProgrammedOvertimeListResponse
        <DataMember>
        Public Property ProgrammedOvertimes As roProgrammedOvertime()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una prevision de horas de exceso
    ''' </summary>
    <DataContract>
    Public Class roProgrammedOvertimeResponse
        <DataMember>
        Public Property ProgrammedOvertime As roProgrammedOvertime

        <DataMember>
        Public Property oState As roWsState
    End Class

    <Serializable>
    <DataContract>
    Public Class roProgrammedOvertime

        Public Sub New()
            Me.BeginTime = New DateTime(1899, 12, 30, 0, 0, 0)
            Me.EndTime = New DateTime(1899, 12, 30, 0, 0, 0)
        End Sub

        <DataMember>
        Public Property ID As Long

        <DataMember>
        Public Property IDEmployee As Integer

        <DataMember>
        Public Property ProgrammedBeginDate As Date

        <DataMember>
        Public Property ProgrammedEndDate As Date

        <DataMember>
        Public Property IDCause As Integer

        <DataMember>
        Public Property RequestId As Nullable(Of Integer)

        <DataMember>
        Public Property Duration As Double

        <DataMember>
        Public Property Description As String

        <DataMember>
        Public Property BeginTime As DateTime

        <DataMember>
        Public Property EndTime As DateTime

        <DataMember>
        Public Property MinDuration As Double

        <DataMember>
        Public Property HasDocuments As Boolean

        <DataMember>
        Public Property DocumentsDelivered As Boolean

    End Class

End Namespace