Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    ''' <summary>
    ''' Enumeración usada en importación de supervisores vía ficheros Excel (no estándar y deprecada frente a API REST)
    ''' </summary>
    Public Enum SupervisorColumns
        GroupName
        ResponsibleType
        ResponsibleId
    End Enum


    <DataContract(Name:="Category")>
    <Description("Request category information")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Defines a category overview, including authorization and supervision levels.")>
    Public Class roCategory

        <DataMember(Name:="CategoryID")>
        <Description("Alphanumeric identifier for the category")>
        <SwaggerWcfProperty(Required:=True, Default:="CAT_01", Description:="Category's alphanumeric ID")>
        Public Property CategoryID As String

        <DataMember(Name:="Name")>
        <Description("Name of the category")>
        <SwaggerWcfProperty(Required:=True, Default:="Default Category", Description:="Name of the category")>
        Public Property Name As String

    End Class

    <DataContract(Name:="Role")>
    <Description("Role information")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Defines a role overview, including its external ID and name.")>
    Public Class roRole

        <DataMember(Name:="RoleID")>
        <Description("Alphanumeric identifier for the role")>
        <SwaggerWcfProperty(Required:=True, Default:="ROLE_01", Description:="Role's alphanumeric ID")>
        Public Property ExternalId As String

        <DataMember(Name:="Name")>
        <Description("Name of the role")>
        <SwaggerWcfProperty(Required:=True, Default:="Administrator", Description:="Name of the role")>
        Public Property Name As String

    End Class

    <DataContract(Name:="SupervisorCategory")>
    <Description("Category information for a supervisor")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Defines a category supervised by a supervisor, including authorization and supervision levels.")>
    Public Class roSupervisorCategory

        <DataMember(Name:="CategoryID")>
        <Description("Alphanumeric identifier for the category")>
        <SwaggerWcfProperty(Required:=True, Default:="CAT_01", Description:="Category's alphanumeric ID")>
        Public Property CategoryID As String

        <DataMember(Name:="AuthorizationLevel")>
        <Description("Authorization level for the category")>
        <SwaggerWcfProperty(Required:=True, Default:="ReadWrite", Description:="Category's authorization level")>
        Public Property AuthorizationLevel As String

        <DataMember(Name:="MinimumSupervisionLevel")>
        <Description("Minimum level to start supervising this category")>
        <SwaggerWcfProperty(Required:=True, Default:="Level1", Description:="Minimum supervision level for the category")>
        Public Property MinimumSupervisionLevel As String

    End Class

    <DataContract(Name:="Supervisor")>
    <Description("Supervisor data")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Visualtime Supervisor data")>
    Public Class roSupervisor

        <DataMember(Name:="Name")>
        <Description("Name of the supervisor")>
        <SwaggerWcfProperty(Required:=True, Default:="John Doe", Description:="Supervisor's full name")>
        Public Property Name As String

        <DataMember(Name:="Description")>
        <Description("Description of the supervisor role or profile")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Supervisor's description")>
        Public Property Description As String

        <DataMember(Name:="RoleID")>
        <Description("Alphanumeric identifier for the supervisor's role")>
        <SwaggerWcfProperty(Required:=True, Default:="SUPERVISOR_ROLE_01", Description:="Supervisor's role ID")>
        Public Property RoleID As String

        <DataMember(Name:="Language")>
        <Description("Supervisor's preferred language code (e.g., en, es)")>
        <SwaggerWcfProperty(Required:=False, Default:="es", Description:="Supervisor's language preference")>
        Public Property Language As String

        <DataMember(Name:="ValidityStartDate")>
        <Description("Date from which the supervisor profile is valid")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start date of supervisor's validity period")>
        Public Property ValidityStartDate As roWCFDate

        <DataMember(Name:="ValidityEndDate")>
        <Description("Date until which the supervisor profile is valid")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="End date of supervisor's validity period")>
        Public Property ValidityEndDate As roWCFDate

        <DataMember(Name:="IsActive")>
        <Description("Indicates if the supervisor profile is currently active")>
        <SwaggerWcfProperty(Required:=True, Default:="True", Description:="Indicates if supervisor is active")>
        Public Property IsActive As Boolean

        <DataMember(Name:="Email")>
        <Description("Supervisor's email address")>
        <SwaggerWcfProperty(Required:=False, Default:="supervisor@example.com", Description:="Supervisor's email")>
        Public Property Email As String

        <DataMember(Name:="UserName")>
        <Description("Supervisor's username for system access")>
        <SwaggerWcfProperty(Required:=True, Default:="johndoe_supervisor", Description:="Supervisor's username")>
        Public Property UserName As String

        <DataMember(Name:="SupervisedGroupIDs")>
        <Description("List of alphanumeric IDs of groups supervised by this supervisor")>
        <SwaggerWcfProperty(Required:=False, Description:="List of supervised group IDs")>
        Public Property SupervisedGroupIDs As String()

        <DataMember(Name:="SupervisedEmployeeIDs")>
        <Description("List of alphanumeric IDs of employees supervised by this supervisor")>
        <SwaggerWcfProperty(Required:=False, Description:="List of supervised employee IDs")>
        Public Property SupervisedEmployeeIDs As String()

        <DataMember(Name:="EmployeeExceptionIDs")>
        <Description("List of alphanumeric IDs of exceptions related to employees under this supervisor")>
        <SwaggerWcfProperty(Required:=False, Description:="List of employee exception IDs")>
        Public Property EmployeeExceptionIDs As String()

        <DataMember(Name:="Categories")>
        <Description("List of categories managed or relevant to this supervisor, including authorization and supervision levels")>
        <SwaggerWcfProperty(Required:=False, Description:="List of categories with their respective authorization and supervision levels")>
        Public Property Categories As roSupervisorCategory()

    End Class



End Namespace