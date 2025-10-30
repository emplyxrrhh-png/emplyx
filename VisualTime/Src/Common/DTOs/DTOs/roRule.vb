Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Estado de la solicitud
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eRuleType
        ' 0 = Deprecated - Bonus
        ' 1 = Deprecated - Adv

        ''' <summary>
        ''' Regas de justificación (sysroShiftsCausesRules type 2)
        ''' </summary>
        <EnumMember> Justification = 2

        ''' <summary>
        ''' Regas diaria (sysroShiftsCausesRules type 3)
        ''' </summary>
        <EnumMember> Daily = 3

        ''' <summary>
        ''' Regas de arrastre (AccrualsRules)
        ''' </summary>
        <EnumMember> Accruals = 4

        ''' <summary>
        ''' Regas de Bots (BotRules)
        ''' </summary>
        <EnumMember> Bots = 5

        ''' <summary>
        ''' Regas de Solicitudes (RequestRules)
        ''' </summary>
        <EnumMember> Requests = 6

        ''' <summary>
        ''' Regas de Planificación (ScheduleRules)
        ''' </summary>
        <EnumMember> Schedule = 7
    End Enum

    ''' <summary>
    ''' Tipos de cambios introducidos en un registro histórico de regla
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eRuleChangeType
        ''' <summary>
        ''' Cambio en definición de la regla
        ''' </summary>
        <EnumMember> Definition = 0
        ''' <summary>
        ''' Cambio en la la composición de horarios de los que forma parte la regla
        ''' </summary>
        <EnumMember> Composition = 1
        ''' <summary>
        ''' Cambio en la definición y también en la composición de los grupos de reglas a los que pertenece la regla
        ''' </summary>
        <EnumMember> DefinitionAndComposition = 2

    End Enum

    <DataContract>
    Public Class roRule

        ''' <summary>
        ''' Identificador único de la regla
        ''' </summary>
        <DataMember>
        Public Property Id As Integer

        ''' <summary>
        ''' Nombre de la regla
        ''' </summary>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Descripción de la regla
        ''' </summary>
        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Tipo de regla
        ''' </summary>
        <DataMember>
        Public Property Type As eRuleType

        ''' <summary>
        ''' Identificador del grupo al que pertenece la regla (opcional)
        ''' </summary>
        <DataMember>
        Public Property GroupId As Integer?

        ''' <summary>
        ''' Lista de tags asociados a la regla
        ''' </summary>
        <DataMember>
        Public Property Tags As List(Of String)

        ''' <summary>
        ''' Historial de cambios para las propiedades historificables
        ''' </summary>
        <DataMember>
        Public Property RuleDefinitions As List(Of roRuleDefinition)

        ''' <summary>
        ''' Fecha en la que se creó la regla
        ''' </summary>
        <DataMember>
        Public Property CreatedDate As Date

        ''' <summary>
        ''' Fecha en la que se realizó el último cambio en la regla
        ''' </summary>
        <DataMember>
        Public Property ModifiedDate As Date

        ''' <summary>
        ''' Éstado de edición del registro, para gestión de pantalla
        ''' </summary>
        <DataMember>
        Public Property EditionStatus As ItemEditionStatus


        Public Sub New()
            Id = -1
            Name = String.Empty
            Description = String.Empty
            Type = eRuleType.Justification
            GroupId = Nothing
            Tags = New List(Of String)()
            CreatedDate = Now.Date
            ModifiedDate = Now.Date
            RuleDefinitions = New List(Of roRuleDefinition)()
        End Sub

    End Class

    ''' <summary>
    ''' Representa un registro histórico para propiedades de reglas
    ''' </summary>
    <DataContract>
    Public Class roRuleDefinition
        ''' <summary>
        ''' Identificador único del registro histórico
        ''' </summary>
        <DataMember>
        Public Property Id As Integer

        ''' <summary>
        ''' Descripción de la regla
        ''' </summary>
        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Identificador de la regla a la que pertenece este registro histórico
        ''' </summary>
        <DataMember>
        Public Property IdRule As Integer

        ''' <summary>
        ''' Contexto de empleados al que se aplica el grupo de reglas
        ''' </summary>
        <DataMember>
        Public Property EmployeeContext As roSelectorFilter

        ''' <summary>
        ''' Horarios a los que aplica el grupo de reglas
        ''' </summary>
        <DataMember>
        Public Property Shifts As List(Of roShiftItem)

        ''' <summary>
        ''' Condiciones que debe cumplir la regla
        ''' </summary>
        <DataMember>
        Public Property Conditions As List(Of roRuleCondition)

        ''' <summary>
        ''' Acciones que se ejecutarán si se cumplen las condiciones
        ''' </summary>
        <DataMember>
        Public Property Actions As List(Of roRuleAction)

        ''' <summary>
        ''' Definición de regla en XML
        ''' </summary>
        <DataMember>
        Public Property XmlDefinition As String

        ''' <summary>
        ''' Usuario que realizó el cambio
        ''' </summary>
        <DataMember>
        Public Property ModifiedBy As String

        ''' <summary>
        ''' Fecha en la que se realizó el cambio
        ''' </summary>
        <DataMember>
        Public Property ModifiedDate As Date

        ''' <summary>
        ''' Fecha desde la que aplica este registro histórico
        ''' </summary>
        <DataMember>
        Public Property EffectiveFrom As Date

        ''' <summary>
        ''' Fecha hasta la que aplica este registro histórico (no persistida en BBDD)
        ''' </summary>
        <DataMember>
        Public Property EffectiveUntil As Date?

        ''' <summary>
        ''' Éstado de edición del registro, para gestión de pantalla
        ''' </summary>
        <DataMember>
        Public Property EditionStatus As ItemEditionStatus

        <DataMember>
        Public Property ChangeType As eRuleChangeType

        ''' <summary>
        ''' Identificador del contexto de empleados al que se aplica el grupo de reglas
        ''' </summary>

        ''' <summary>
        ''' Constructor por defecto
        ''' </summary>
        Public Sub New()
            Id = -1
            IdRule = -1
            EffectiveFrom = Date.UtcNow
            EffectiveUntil = Nothing
            ModifiedBy = String.Empty
            ModifiedDate = Date.UtcNow
            EmployeeContext = Nothing
            Shifts = New List(Of roShiftItem)()
            Conditions = New List(Of roRuleCondition)()
            Actions = New List(Of roRuleAction)()
            XmlDefinition = String.Empty
            EditionStatus = ItemEditionStatus.NotEdited
        End Sub
    End Class

    ''' <summary>
    ''' Para uso en pantalla donde se requiere sólo Id y Name
    ''' </summary>
    <DataContract()>
    Public Class roShiftItem
        <DataMember>
        Public Property IdShift As Integer
        <DataMember>
        Public Property ShiftName As String

        <DataMember>
        Public Property IdShiftGroup As Integer

        <DataMember>
        Public Property ShiftGroupName As String

        <DataMember>
        Public Property IdRule As Integer

        <DataMember>
        Public Property Order As Integer

        <DataMember>
        Public Property EditionStatus As ItemEditionStatus

    End Class

    ''' <summary>
    ''' Representa un grupo de reglas
    ''' </summary>
    <DataContract>
    Public Class roRuleChangeHistory

        ''' <summary>
        ''' Nombre del grupo
        ''' </summary>
        <DataMember>
        Public Property Path As String

        ''' <summary>
        ''' Acción realizada
        ''' </summary>
        <DataMember>
        Public Property Action As ItemEditionStatus

        ''' <summary>
        ''' Constructor por defecto
        ''' </summary>
        Public Sub New()
            Path = String.Empty
            Action = ItemEditionStatus.NotEdited
        End Sub

    End Class

    ''' <summary>
    ''' Representa un grupo de reglas
    ''' </summary>
    <DataContract>
    Public Class roRulesGroup

        ''' <summary>
        ''' Identificador único del grupo
        ''' </summary>
        <DataMember>
        Public Property Id As Integer

        ''' <summary>
        ''' Nombre del grupo
        ''' </summary>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Lista de reglas asociadas al horario. El orden es relevante. El motor las cogerá en el mimsmo orden.
        ''' </summary>
        <DataMember>
        Public Property Rules As List(Of roRule)

        ''' <summary>
        ''' Éstado de edición del registro, para gestión de pantalla
        ''' </summary>
        <DataMember>
        Public Property EditionStatus As ItemEditionStatus

        ''' <summary>
        ''' Constructor por defecto
        ''' </summary>
        Public Sub New()
            Id = -1
            Name = String.Empty
            Rules = New List(Of roRule)
            EditionStatus = ItemEditionStatus.NotEdited
        End Sub

    End Class

    ''' <summary>
    ''' Clase base de la que deben heredar todas las clase que modelen condiciones de una regla
    ''' </summary>
    <DataContract>
    <KnownType(GetType(roDummyRuleCondition))>
    Public MustInherit Class roRuleCondition
    End Class

    ''' <summary>
    ''' Clase base de la que deben heredar todas las clase que modelen acciones de una regla
    ''' </summary>
    <DataContract>
    <KnownType(GetType(roDummyRuleAction))>
    Public MustInherit Class roRuleAction
    End Class

    ''' <summary>
    ''' Clase para condiciones de un nuevo tipo de regla. Heredan de roRuleCondition
    ''' </summary>
    <DataContract>
    Public Class roDummyRuleCondition
        Inherits roRuleCondition

        Public Sub New()
            Throw New NotImplementedException("Dummy rule condition")
        End Sub
    End Class

    ''' <summary>
    ''' Clase para acciones de un nuevo tipo de regla. Heredan de roRuleAction
    ''' </summary>
    <DataContract>
    Public Class roDummyRuleAction
        Inherits roRuleAction
        Public Sub New()
            Throw New NotImplementedException("Dummy rule action")
        End Sub
    End Class

    ''' <summary>
    ''' Clase con los filtros disponibles para filtrar los grupos de reglas
    ''' </summary>
    ''' <remarks></remarks>
    ''' </summary>
    <DataContract>
    Public Class RulesGroupFilter
        ''' <summary>
        ''' Nombre del grupo de reglas o reglas con un 'contains'
        ''' </summary>
        <DataMember>
        Public Property Name As String
    End Class
End Namespace