Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roCollective

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Historial de definiciones del colectivo
        ''' </summary>
        <DataMember>
        Public Property HistoryEntries As List(Of roCollectiveDefinition)

        ''' <summary>
        ''' Fecha y hora en que se creó esta definición
        ''' </summary>
        <DataMember>
        Public Property CreatedDate As DateTime

        ''' <summary>
        ''' Usuario que creó esta definición
        ''' </summary>
        <DataMember>
        Public Property CreatedBy As String

        ''' <summary>
        ''' Éstado de edición del registro, para gestión de pantalla
        ''' </summary>
        <DataMember>
        Public Property EditionStatus As ItemEditionStatus

        Public Sub New()
            Id = -1
            Name = String.Empty
            Description = String.Empty
            CreatedDate = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            HistoryEntries = New List(Of roCollectiveDefinition)()
            EditionStatus = ItemEditionStatus.NotEdited
        End Sub
    End Class


    <DataContract>
    <Serializable>
    Public Class roCollectiveDefinition
        ''' <summary>
        ''' Identificador único de la definición histórica
        ''' </summary>
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Identificador del colectivo al que pertenece esta definición
        ''' </summary>
        <DataMember>
        Public Property CollectiveId As Integer

        ''' <summary>
        ''' Definición del colectivo en este punto de la historia
        ''' </summary>
        <DataMember>
        Public Property Definition As String

        ''' <summary>
        ''' Expresión de filtro utilizada en esta definición
        ''' </summary>
        <DataMember>
        Public Property FilterExpression As String

        <DataMember>
        Public Property BeginDate As Date

        <DataMember>
        Public Property EndDate As Date?

        ''' <summary>
        ''' Fecha y hora en que se creó esta definición
        ''' </summary>
        <DataMember>
        Public Property ModifiedDate As DateTime

        ''' <summary>
        ''' Usuario que creó esta definición
        ''' </summary>
        <DataMember>
        Public Property ModifiedBy As String

        ''' <summary>
        ''' Éstado de edición del registro, para gestión de pantalla
        ''' </summary>
        <DataMember>
        Public Property EditionStatus As ItemEditionStatus

        Public Sub New()
            Id = -1
            Description = String.Empty
            CollectiveId = -1
            Definition = String.Empty
            FilterExpression = String.Empty
            BeginDate = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            EndDate = Date.Now.Date
            ModifiedDate = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            ModifiedBy = String.Empty
            EditionStatus = ItemEditionStatus.NotEdited
        End Sub
    End Class


    Public Class Comparison
        Public Property Operand1 As String
        Public Property ComparisonOperator As String
        Public Property Operand2 As String

        Public Sub New(operand1 As String, comparisonOperator As String, operand2 As String)
            Me.Operand1 = operand1
            Me.ComparisonOperator = comparisonOperator
            Me.Operand2 = operand2
        End Sub
    End Class
End Namespace


