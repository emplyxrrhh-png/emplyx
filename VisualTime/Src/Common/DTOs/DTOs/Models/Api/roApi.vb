''' <summary>
''' DTOs de la API rest de enlace con Visualtime
''' </summary>
'''
Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum HolidayType_Enum

        <Description("Holiday for days")>
        <EnumMember> Days

        <Description("Holiday for hours")>
        <EnumMember> Hours

    End Enum

    Public Enum AbsenceType_Enum

        <Description("Absence for days")>
        <EnumMember> Days

        <Description("Absence for hours")>
        <EnumMember> Hours

    End Enum

    Public Enum DocumentStatusType_Enum

        <Description("Document pending of validation")>
        <EnumMember> Pending

        <Description("A supervisor has approved the document")>
        <EnumMember> Validated

        <Description("The document has expired")>
        <EnumMember> Expired

        <Description("The document has been rejected")>
        <EnumMember> Rejected

        <Description("The document has been invalidated")>
        <EnumMember> Invalidated

    End Enum

    <DataContract(Name:="roGroup")>
    <Description("Organizational structure of the company")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Organizational structure of the company, with the groups organized hierarchically, optionally including information from the users of each group")>
    Public Class roGroup

        <DataMember>
        <Description("Group name")>
        <SwaggerWcfProperty(Required:=True, Default:="Administration", Description:="Group name")>
        Public Property Name As String

        <DataMember>
        <Description("Full path of the group separated by //")>
        <SwaggerWcfProperty(Required:=True, Default:="1 // 2", Description:="Group codes chain of the full route")>
        Public Property FullPath As String

        <DataMember>
        <Description("Unique group Id")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique group identifier")>
        Public Property Id As String

        <DataMember(Name:="Childs")>
        <Description("List of groups ")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of groups that depend directly on this")>
        Public Property Childs As roGroup()

        <DataMember(Name:="Employees")>
        <Description("List of users inside the group")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of users who are directly in the group")>
        Public Property Employees As roEmployeeOnGroup()

    End Class

    <DataContract(Name:="roEmployeeOnGroup")>
    <Description("Minimum information of a user to show in group structure")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Minimum information of a user to show in group structure")>
    Public Class roEmployeeOnGroup

        <DataMember(Name:="Name")>
        <Description("Name of the user")>
        <SwaggerWcfProperty(Required:=True, Default:="John Smith", Description:="User name")>
        Public Property Name As String

        <DataMember(Name:="Id")>
        <Description("Id of the user")>
        <SwaggerWcfProperty(Required:=True, Default:="12345678A", Description:="Identification document")>
        Public Property Id As String

        <DataMember(Name:="InContract")>
        <Description("Active contract or not")>
        <SwaggerWcfProperty(Required:=True, Default:="true", Description:="Indicates if the user has active contract today")>
        Public Property InContract As Boolean

    End Class

    <DataContract(Name:="roAbsence")>
    <Description("User absence")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Detail regarding the planned absence of a user")>
    Public Class roAbsence

        <DataMember(Name:="EmployeeId")>
        <Description("Unique user identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique absence identifier")>
        Public Property EmployeeId As String

        <DataMember>
        <Description("Date Start of absence")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start Date of the absence")>
        Public Property BeginDate As roWCFDate

        <DataMember>
        <Description("End date of the absence. Report January 1, 2079 when the end date is unknown")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="End Date of the absence")>
        Public Property EndDate As roWCFDate

        <DataMember>
        <Description("Maximum number of days of absence, when your end date is unknown")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Maximum number of absence days when EndDate is not specified.")>
        Public Property MaxDays As Nullable(Of Integer)

        <DataMember>
        <Description("Short name of the justification of the absence in Visualtime")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Short name of the justification of the absence in Visualtime")>
        Public Property CauseShortName As String

        <DataMember>
        <Description("Export key of the justification of absence")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Export key of the justification of absence")>
        Public Property CauseExportKey As String

        <DataMember>
        <Description("Time at the beginning of the absence for hour absences. For day absences, report January 1, 2079")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Time at the beginning of the absence for hour absences. For day absences, report January 1, 2079")>
        Public Property BeginHour As roWCFDate

        <DataMember>
        <Description("Time to end the absence for hour absences. For day absences, report January 1, 2079")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Time to end the absence for hour absences. For day absences, report January 1, 2079")>
        Public Property EndHour As roWCFDate

        <DataMember>
        <Description("Duration of daily absence for hour absences. For day absences, report January 1, 2079")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Duration of daily absence for hour absences. For day absences, report January 1, 2079")>
        Public Property Duration As roWCFDate

        <DataMember>
        <Description("Operation to be done with the absence (only to inform absences in Visualtime, not to recover them)")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Operation to be done with the absence (only to inform absences in Visualtime, not to recover them)")>
        Public Property Action As String

        <DataMember>
        <Description("Unique identifier of absence")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique identifier of absence. It must be taken into account for deletion operations of an absence)")>
        Public Property Id As Integer

        <DataMember>
        <Description("Date of creation or last modification or deletion of the absence. For absences created in previous and unmodified Visualtime versions, this date is 1900-01-01")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Date of creation or last modification or deletion of the absence. For absences created in previous and unmodified Visualtime versions, this date is 1900-01-01")>
        Public Property Timestamp As roWCFDate

        <DataMember>
        <Description("Name of the justification of absence")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Name of the justification of the absence")>
        Public Property CauseName As String

    End Class

    <DataContract(Name:="usuario")>
    <Description("Information related to a user. General data, contracts, tab...")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a user. General data, contracts...")>
    Public Class roEmployee

        <DataMember>
        <Description("Name of the employee")>
        <SwaggerWcfProperty(Required:=True, Default:="John Smith", Description:="Name and Surname of the User")>
        Public Property Name As String

        <DataMember>
        <Description("ID of the employee")>
        <SwaggerWcfProperty(Required:=True, Default:="12", Description:="Unique user identifier")>
        Public Property ID As String

        <DataMember>
        <Description("List of available authentifications of the employee")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of methods with which the user can identify")>
        Public Property AuthenticationMethods As roAuthentication() '

        <DataMember>
        <Description("List of employee contracts")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of user contracts")>
        Public Property Contracts As roContract()

        <DataMember>
        <Description("List of employee mobilities")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of assignments to user tree groups")>
        Public Property Mobilities As roMobility()

        <DataMember>
        <Description("List of employee contracts")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Fields of the user's file with the different values")>
        Public Property Fields As roEmployeeField()

    End Class

    <DataContract(Name:="Documento")>
    <Description("Information related to a user document")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a user document")>
    Public Class roDocument

        <DataMember>
        <Description("Unique identifier of the user to which the document is associated")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique user identifier")>
        Public Property EmployeeID As String

        <DataMember>
        <Description("Name of the folder in which the document will be saved in the Document Manager. The indicated folder must exist in the Visualtime Document Manager, and this must be of type General user documentation")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Folder name")>
        Public Property DocumentType As String

        <DataMember>
        <Description("Content Of the document, coded in base64")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Document content (Base64)")>
        Public Property DocumentData As String

        <DataMember>
        <Description("Document title")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Document title")>
        Public Property DocumentTitle As String

        <DataMember>
        <Description("Extension of the document")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Document extension")>
        Public Property DocumentExtension As String

        <DataMember>
        <Description("Notes of the document")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Document notes")>
        Public Property DocumentRemarks As String

        <DataMember>
        <Description("ID of the group to which the company document belongs. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="ID of the group to which the company document belongs. Readonly")>
        Public Property CompanyID As String

        <DataMember>
        <Description("Absence information in documents related to an absence. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Absence information in documents related to an absence. Readonly")>
        Public Property AbsenceInfo As roDocumentAbsenceInfo

        <DataMember>
        <Description("Date the document was delivered. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Date the document was delivered")>
        Public Property DeliveryDate As roWCFDate

        <DataMember>
        <Description("Channel the document was delivered. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Channel the document was delivered. Readonly")>
        Public Property DeliveryChannel As String

        <DataMember>
        <Description("Name of the user who creates the document. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Credential of the user who delivers the document. Readonly")>
        Public Property DeliveredBy As String

        <DataMember>
        <Description("Timestamp of the last document update. If UpdateType parameter is setted it refers to timestamp of this type of update. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Timestamp of the last document update. If UpdateType parameter is setted it refers to timestamp of this type of update. Readonly")>
        Public Property Timestamp As roWCFDate

        <DataMember>
        <Description("Document status info. Readonly")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Document status info. Readonly")>
        Public Property DocumentStatusInfo As roDocumentStatusInfo

        <DataMember>
        <Description("Unique ID assigned to the document for tracking or reference.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Document external ID")>
        Public Property DocumentExternalID As String

    End Class

    <DataContract(Name:="roDocumentAbsenceInfo")>
    <Description("Minimum information of a absence to show in documents related with absence")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Minimum information of a absence to show in documents related with absence")>
    Public Class roDocumentAbsenceInfo

        <DataMember(Name:="IdAbsence")>
        <Description("ID of the absence related to the document")>
        <SwaggerWcfProperty(Required:=False, Description:="ID of the absence related to the document")>
        Public Property Id As String

        <DataMember(Name:="AbsenceType")>
        <Description("Type of absence. It can be for days or hours.")>
        <SwaggerWcfProperty(Required:=False, Description:="Type of absence. It can be for days or hours.")>
        Public Property AbsenceType As String

    End Class

    <DataContract(Name:="roDocumentStatusInfo")>
    <Description("Minimum information of a document status to show in documents")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Minimum information of a document status to show in documents")>
    Public Class roDocumentStatusInfo

        <DataMember(Name:="Type")>
        <Description("Type Of Status. It can be Pending, Validated, Expired, Rejected, Invalidated")>
        <SwaggerWcfProperty(Required:=False, Description:="Type Of Status. It can be Pending, Validated, Expired, Rejected, Invalidated")>
        Public Property Type As String

        <DataMember(Name:="SupervisorName")>
        <Description("Name of the supervisor who mades the last status change")>
        <SwaggerWcfProperty(Required:=False, Description:="Name of the supervisor who mades the last status change")>
        Public Property SupervisorName As String

        <DataMember(Name:="LastStatusChangeDate")>
        <Description("Date of last document status change")>
        <SwaggerWcfProperty(Required:=False, Description:="Date of last document status change")>
        Public Property LastStatusChangeDate As roWCFDate

        <DataMember(Name:="DocumentBeginDate")>
        <Description("Document validity start date")>
        <SwaggerWcfProperty(Required:=False, Description:="Document validity start date")>
        Public Property DocumentBeginDate As roWCFDate

        <DataMember(Name:="DocumentEndDate")>
        <Description("Document expiration date")>
        <SwaggerWcfProperty(Required:=False, Description:="Document expiration date")>
        Public Property DocumentEndDate As roWCFDate

        <DataMember(Name:="SignedDate")>
        <Description("Document signed date")>
        <SwaggerWcfProperty(Required:=False, Description:="Document signed date")>
        Public Property DocumentSignedDate As roWCFDate

    End Class


    <DataContract(Name:="Document action result")>
    <Description("Result of a document deletion action")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Result of a document deletion action")>
    Public Class roDocumentResponse
        <DataMember>
        <Description("Response code")>
        Public Property Status As Integer

        <DataMember>
        <Description("Response text")>
        Public Property Text As String

    End Class

    <DataContract(Name:="Field")>
    <Description("Field of the user's file with the value assigned to a specific date")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Field of the user's file with the value assigned to a specific date")>
    Public Class roEmployeeField

        <DataMember>
        <Description("Field Name")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Field name")>
        Public Property FieldName As String

        <DataMember>
        <Description("Value of the field")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Field value")>
        Public Property FieldValue As String

        <DataMember>
        <Description("Date from which the value is assigned")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Field date")>
        Public Property FieldValueDate As roWCFDate

    End Class

    <DataContract(Name:="Photo")>
    <Description("Employee photo")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Employee's photo")>
    Public Class roEmployeePhoto

        <DataMember>
        <Description("Unique identifier of the user to which the document is associated")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique user identifier")>
        Public Property EmployeeID As String

        <DataMember>
        <Description("Employee's photo")>
        <SwaggerWcfProperty(Required:=True, Default:="iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABHNCSVQICAgIfAhki...", Description:="Employee photo on base64 format")>
        Public Property Photo As String

    End Class

    <DataContract(Name:="Contract")>
    <Description("Contract of a user where the period is indicated in which it is active and the Agreement assigned to that contract")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Contract of a user where the period is indicated in which it is active and the Agreement assigned to that contract")>
    Public Class roContract

        <DataMember>
        <Description("Unique contract identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Contract ID")>
        Public Property IDContract As String

        <DataMember>
        <Description("Date on which the contract starts")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start date")>
        Public Property BeginDate As roWCFDate

        <DataMember>
        <Description("Date on which the contract ends")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="End date")>
        Public Property EndDate As roWCFDate

        <DataMember>
        <Description("Identifier of the Agreement assigned to the Contract")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Agreement ID")>
        Public Property LabAgree As String

    End Class

    <DataContract(Name:="Mobility")>
    <Description("Assignment of a user to a group or department in a specific period of dates")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Assignment of a user to a group or department in a specific period of dates")>
    Public Class roMobility

        <DataMember>
        <Description("Unique group identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Group ID")>
        Public Property IDGroup As String

        <DataMember>
        <Description("Date on which it starts the assignment to the group")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start date")>
        Public Property BeginDate As roWCFDate

        <DataMember>
        <Description("Date on which the assignment completes the group")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="End date")>
        Public Property EndDate As roWCFDate

    End Class

    <DataContract(Name:="Authentication")>
    <Description("Method with which the user can identify on Visualtime")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Assignment of a user to a group or department in a specific period of dates")>
    Public Class roAuthentication

        <DataMember>
        <Description("Type of authentication (login, card number)")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Authentification type")>
        Public Property Method As AuthenticationMethod

        <DataMember>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Authentification value")>
        <Description("Authentication value")>
        Public Property Credential As String

    End Class

    <DataContract(Name:="Holidays")>
    <Description("Information related to a user's holidays. Indicating the days or hours planned on vacation and the assigned reason")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a user's holidays. Indicating the days or hours planned on vacation and the assigned reason")>
    Public Class roHoliday

        <DataMember>
        <Description("Unique user identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Employee ID")>
        Public Property IDEmployee As String

        <DataMember>
        <Description("Type of vacation")>
        Public Property HolidayType As HolidayType_Enum

        <DataMember>
        <Description("Planned date")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Planned date")>
        Public Property PlannedDate As roWCFDate

        <DataMember>
        <Description("Reason identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Reason ID")>
        Public Property IDReason As String

        <DataMember>
        <Description("Holiday identifier for hours")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Reason ID")>
        Public Property IdHoursHoliday As String

        <DataMember>
        <Description("Start time")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start date")>
        Public Property BeginTime As roWCFDate

        <DataMember>
        <Description("Ending time")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="End date")>
        Public Property EndTime As roWCFDate

        <DataMember>
        <Description("Elimination time")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="The duration or time period for elimination or removal")>
        Public Property TimeStamp As roWCFDate

        <DataMember>
        <Description("Duration")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="The duration or time period for elimination or removal")>
        Public Property Duration As Double

        <DataMember>
        <Description("Apply throughout the day")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Apply throughout the day")>
        Public Property ApplyAllDay As Boolean

        <DataMember>
        <Description("In set operations, action to be done on vacation registration (I, U, D) (INSERT, UPDATE, DELETE). In get operations D for deleted, CRU for the rest")>
        <SwaggerWcfProperty(Required:=True, Default:="(I, U, D)", Description:="In set operations, action to be done on vacation registration (I, U, D) (INSERT, UPDATE, DELETE). In get operations D for deleted, CRU for the rest")>
        Public Property Action As String 'TODO: Enum

        <DataMember>
        <Description("Reason name")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Reason Name")>
        Public Property ReasonName As String

    End Class

    Public Enum TelecommutingType_Enum

        <Description("Presence")>
        <EnumMember> Presence

        <Description("Telecommuting")>
        <EnumMember> Telecommuting

        <Description("Optional")>
        <EnumMember> TelecommutingOptional

    End Enum

    <DataContract(Name:="Calendar")>
    <Description("Information related to the planning of a user. Indicating the assigned date and time")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the planning of a user. Indicating the assigned date and time")>
    Public Class roCalendar

        <DataMember>
        <Description("Unique user identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1234567", Description:="User identity document. It is used as a secondary identifier if the user is not located by the UniqueEmployeeID")>
        Public Property IDEmployee As String

        <DataMember>
        <Description("Planned date")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Day that the shift has to be applied")>
        Public Property PlannedDate As roWCFDate

        <DataMember>
        <Description("Schedule identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="HMO", Description:="Short name that identifies the shift that will be planned")>
        Public Property IDShift As String

        <DataMember>
        <Description("Minimum allowed start shift hour")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Minimum allowed start shift hour")>
        Public Property StartHour As roWCFDate

        <DataMember>
        <Description("Maximum allowed end shift hour")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Maximum allowed end shift hour")>
        Public Property EndHour As roWCFDate

        <DataMember>
        <Description("Shift Layer definition")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Layer definitions used on createorupdatecalendar to set start hour and related shift info and in getcalendar to recover sheduled layers")>
        Public Property ShiftLayerDefinition As roShiftLayerDefinition()

        <DataMember>
        <Description("Can telecommute")>
        <SwaggerWcfProperty(Required:=False, Default:="false", Description:="Indicates if the employee can telecommute")>
        Public Property CanTelecommute As Boolean

        <DataMember>
        <Description("Telecommuting status")>
        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Telecommute status. (0:Presence, 1:Telecommuting, 2:Optional)")>
        Public Property TelecommutingStatus As TelecommutingType_Enum

        <DataMember>
        <Description("Telecommuting forced")>
        <SwaggerWcfProperty(Required:=False, Default:="false", Description:="If true telecommute condition has been modified by a supervisor")>
        Public Property TelecommuteForced As Boolean

        <DataMember>
        <Description("BreakLayers")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Layer definitions used on createorupdatecalendar to set start hour and related shift info")>
        Public Property BreakLayers As roBreakLayerDefinitionResponse()

        <DataMember>
        <Description("Base Schedule Identifier. Only returned when the planned shift is a holiday shift")>
        <SwaggerWcfProperty(Required:=True, Default:="HMO", Description:="Short name that identifies the shift that will be planned as base shift. Readonly. Only returned when the planned shift is a holiday shift.")>
        Public Property IDShiftBase As String

        <DataMember>
        <Description("Shift Base Layer definition. Only returned when the planned shift is a holiday shift")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Layer definitions of the base shift. Readonly. Only returned when the planned shift is a holiday shift.")>
        Public Property ShiftBaseLayerDefinition As roShiftLayerDefinition()

        <DataMember>
        <Description("Planned start hour on base shift. Readonly. Only returned when the planned shift is a holiday shift")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Planned start hour on base shift. Readonly. Only returned when the planned shift is a holiday shift")>
        Public Property StartBasePlanned As roWCFDate

        <DataMember>
        <Description("Planned end hour on base shift. Readonly. Only returned when the planned shift is a holiday shift")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Planned start hour on base shift. Readonly. Only returned when the planned shift is a holiday shift")>
        Public Property EndBasePlanned As roWCFDate

    End Class

    <DataContract(Name:="roShiftLayerDefinition")>
    <Description("Information related to the shift used. If nothing default shift information is used. Only the first two layers are taken.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the shift used. If nothing default shift information is used. Only the first two layers are taken.")>
    Public Class roShiftLayerDefinition

        <DataMember>
        <Description("Hour in which the layer starts. Format is HH:mm")>
        <SwaggerWcfProperty(Required:=True, Default:="08:00", Description:="Hour in which the layer starts. Format is HH:mm")>
        Public Property StartTime As String

        <DataMember>
        <Description("Used to indicate if the layer starts the same, next or before day referencing the day we are planning")>
        <SwaggerWcfProperty(Required:=False, Default:="roDayInfo.CurrentDay", Description:="Used to indicate if the layer starts the same, before or next day referencing the day we are planning. 0 = Current, 1 = Before, 2 = Next")>
        Public Property StartDay As roDayInfo

        <DataMember>
        <Description("Ordinary hours that the layer will have. Format is HH:mm for creation/modification and minutes for reading")>
        <SwaggerWcfProperty(Required:=True, Default:="06:00", Description:="Ordinary hours that the layer will have. Format is HH:mm for creation/modification and minutes for reading")>
        Public Property OrdinaryHours As String

        <DataMember>
        <Description("Complementary hours that the layer will have. Format is HH:mm for creation/modification and minutes for reading")>
        <SwaggerWcfProperty(Required:=False, Default:="00:00", Description:="Complementary hours that the layer will have. Format is HH:mm for creation/modification and minutes for reading")>
        Public Property ComplemntaryHours As String

    End Class

    <DataContract(Name:="roBreakLayerDefinitionResponse")>
    <Description("Information related to the breaks of the shift used. This information is only for read. You can't create or update this breaks layers with the API.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the breaks of the shift used. This information is only for read. You can't create or update this breaks layers with the API.")>
    Public Class roBreakLayerDefinitionResponse

        <DataMember>
        <Description("Hour in which the break layer starts. Format is HH:mm")>
        <SwaggerWcfProperty(Required:=True, Default:="08:00", Description:="Hour in which the break layer starts. Format is HH:mm")>
        Public Property Start As roWCFDate

        <DataMember>
        <Description("Hour in which the break layer ends. Format is HH:mm")>
        <SwaggerWcfProperty(Required:=True, Default:="09:00", Description:="Hour in which the break layer ends. Format is HH:mm")>
        Public Property Finish As roWCFDate

    End Class

    <DataContract(Name:="roDayInfo")>
    <Description("Possible values related to the day that is beeing planned")>
    Public Enum roDayInfo
        CurrentDay = 0
        DayBefore = 1
        DayAfter = 2
    End Enum

    <DataContract(Name:="Eroneous calendar")>
    <Description("Information related to the planning of a user who has not been able to plan. ")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the planning of a user. Indicating the assigned date and time")>
    Public Class roCalendarResponse

        <DataMember>
        <Description("Calendar")>
        Public Property oCalendar As roCalendar

        <DataMember>
        <Description("Response code")>
        Public Property Status As Integer

        <DataMember>
        <Description("Response text")>
        Public Property Text As String

    End Class

    <DataContract(Name:="Signing")>
    <Description("Information related to the signing of a user.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the signing of a user.")>
    Public Class roPunch

        <DataMember>
        <Description("Unique punch identifier")>
        Public Property ID As String

        <DataMember>
        <Description("Unique user identifier")>
        Public Property IDEmployee As String

        <DataMember>
        <Description("Type of signing. For insertion, only type 1 (input), 2 (output) and 3 (automatic) are supported")>
        Public Property Type As PunchTypeEnum

        <DataMember>
        <Description("Type of signing calculated by Visualtime when the type indicated in the insert is 3 (automatic). It is not necessary in insertion operations")>
        Public Property ActualType As PunchTypeEnum

        <DataMember>
        <Description("Date and time of assigned signing")>
        Public Property ShiftDate As roWCFDate

        <DataMember>
        <Description("Date and time of signing")>
        Public Property DateTime As roWCFDate

        <DataMember>
        <Description("Terminal identifier")>
        Public Property IDTerminal As Integer

        <DataMember>
        <Description("Identifier of the justification associated with the signing when its type is 1, 2 or 3")>
        Public Property TypeData As String

        <DataMember>
        <Description("Latitude and length of signing, if you have")>
        Public Property GPS As String

        <DataMember>
        <Description("Extended chain type information")>
        Public Property Field1 As String

        <DataMember>
        <Description("Extended chain type information")>
        Public Property Field2 As String

        <DataMember>
        <Description("Extended chain type information")>
        Public Property Field3 As String

        <DataMember>
        <Description("Extended information of numerical type")>
        Public Property Field4 As Nullable(Of Double)

        <DataMember>
        <Description("Extended information of numerical type")>
        Public Property Field5 As Nullable(Of Double)

        <DataMember>
        <Description("Extended information of numerical type")>
        Public Property Field6 As Nullable(Of Double)

        <DataMember>
        <Description("In responses, indicates the return code of the saving operation of the signing in question")>
        Public Property ResultCode As Integer

        <DataMember>
        <Description("In responses, extended information about the result of the saving operation of the signing in question")>
        Public Property ResultDescription As String

        <DataMember>
        <Description("Date of creation or modification of the signing")>
        Public Property Timestamp As roWCFDate

    End Class

    <DataContract(Name:="Punch selection criteria")>
    <Description("Information related about punch to update")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related about the punch to update.")>
    Public Class roPunchCriteria

        <DataMember>
        <Description("Unique punch identifier")>
        <SwaggerWcfProperty(Required:=True, Description:="Unique punch identifier")>
        Public Property ID As String

        <DataMember>
        <Description("New Date and time for the punch. If the value is empty no changes will be updated.")>
        <SwaggerWcfProperty(Required:=False, Description:="New Date and time for the punch")>
        Public Property DateTimeToUpdate As roWCFDate

        <DataMember>
        <Description("Type of signing. For update, only type 1 (input), 2 (output) and 3 (automatic) are supported. If the value is empty no changes will be updated.")>
        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Type of signing. For update, only type 1 (input), 2 (output) and 3 (automatic) are supported")>
        Public Property Type As PunchTypeEnum

    End Class

    <DataContract(Name:="Punch to delete selection criteria")>
    <Description("Information related to identify the punches to delete")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related about punches to delete")>
    Public Class roPunchToDeleteCriteria

        <DataMember>
        <Description("Unique punch identifier")>
        <SwaggerWcfProperty(Required:=True, Description:="Unique punch identifier")>
        Public Property ID As String

    End Class

    <DataContract(Name:="Non-added signing")>
    <Description("Information related to a signing that could not be incorporated ")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a signing that could not be incorporated")>
    Public Class roPunchesResponse

        <DataMember>
        <Description("Signing")>
        Public Property oPunch As roPunch

        <DataMember>
        <Description("Response code")>
        Public Property Status As Integer

        <DataMember>
        <Description("Response text")>
        Public Property Text As String

    End Class

    <DataContract(Name:="Balances")>
    <Description("Information related to the balance of a user for a day.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the balance of a user for a day.")>
    Public Class roAccrual

        <DataMember>
        <Description("Unique user identifier")>
        Public Property IDEmployee As String 'ImportKey

        <DataMember>
        <Description("Export code of the balance")>
        Public Property AccrualExportKey As String ' Codigo exportacion

        <DataMember>
        <Description("Short name of the balance")>
        Public Property AccrualShortName As String 'Nombre corto

        <DataMember>
        <Description("Date of the balance generated")>
        Public Property AccrualDate As roWCFDate 'Fecha del acumulado

        <DataMember>
        <Description("Value of the balance generated for the user in the day")>
        Public Property AccrualValue As Double 'Valor del acumulado

    End Class

    <DataContract(Name:="Tasks")>
    <Description("Information related to the balance of a user for a day on a task.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the balance of a user for a day on a task.")>
    Public Class roTaskAccrual

        <DataMember>
        <Description("Unique user identifier")>
        Public Property IDEmployee As String 'ImportKey

        <DataMember>
        <Description("Project of the task")>
        Public Property Project As String ' Proyecto

        <DataMember>
        <Description("Task")>
        Public Property Task As String 'Tarea

        <DataMember>
        <Description("Date of the balance generated")>
        Public Property AccrualDate As roWCFDate 'Fecha del acumulado

        <DataMember>
        <Description("Value of the balance generated for the user in the day")>
        Public Property AccrualValue As Double 'Valor del acumulado

    End Class

    <DataContract(Name:="Schedule")>
    <Description("Information related to the definition of schedule.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the definition of schedule.")>
    Public Class roShift

        <DataMember>
        <Description("Unique schedule identifier")>
        Public Property IDShift As String

        <DataMember>
        <Description("Unique identifier of the assigned schedule group")>
        Public Property IDGroup As String

        <DataMember>
        <Description("Name of the schedule")>
        Public Property Name As String

        <DataMember>
        <Description("Theoretical working hours")>
        Public Property ExpectedWorkingHours As Double

        <DataMember>
        <Description("Start of the schedule")>
        Public Property StartLimit As roWCFDate

        <DataMember>
        <Description("Final of the schedule")>
        Public Property EndLimit As roWCFDate

        <DataMember>
        <Description("Type of time")>
        Public Property Type As ShiftType

        <DataMember>
        <Description("Color of the shift represented in hex")>
        Public Property Color As String

    End Class

    <DataContract(Name:="Festive Template")>
    <Description("Information related to a holiday template.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a holiday template.")>
    Public Class roPublicHoliday

        <DataMember>
        <Description("Festive Template Identifier")>
        Public Property ID As String

        <DataMember>
        <Description("List of festive containing the template")>
        Public Property PublicHolidaysDetails As roPublicHolidayDate()

    End Class

    <DataContract(Name:="Festive date of a template")>
    <Description("Information related to a festive date of a template")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a festive date of a template")>
    Public Class roPublicHolidayDate

        <DataMember>
        <Description("Festive date")>
        Public Property PublicHolidayDate As roWCFDate

        <DataMember>
        <Description("Description")>
        Public Property Description As String

    End Class

    <DataContract(Name:="CALENDAR SELECTION CRITERIA")>
    <Description("Information related to the filters used to obtain the users' calendar")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the filters used to obtain the users' calendar")>
    Public Class roCalendarCriteria

        <DataMember>
        <Description("List of unique user identifiers")>
        Public Property IDEmployees As String()

        <DataMember>
        <Description("Initial date of the period")>
        Public Property StartDate As roWCFDate

        <DataMember>
        <Description("End date of the period")>
        Public Property EndDate As roWCFDate

    End Class

    <DataContract(Name:="LockDate info")>
    <Description("Information related to the lock date")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the lock date")>
    Public Class roLockDateResponse

        <DataMember>
        <Description("Lock date")>
        Public Property LockDate As roWCFDate

    End Class

    <DataContract(Name:="Requests")>
    <Description("Information related to the requests of a user.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the requests of a user.")>
    Public Class roRequest

        <DataMember>
        <Description("Unique user identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Unique user identifier")>
        Public Property IDEmployee As String 'ImportKey

        <DataMember>
        <Description("Type of the request")>
        Public Property RequestType As eRequestType ' Tipo 'TODO: Revisar este nombre, en otros sitios está como Type

        <DataMember>
        <Description("Date of the request generated")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Date of the request generated")>
        Public Property RequestDate As roWCFDate 'Fecha de creación de la solicitud

        <DataMember>
        <Description("Status of the request")>
        Public Property Status As eRequestStatus 'Estado de la solicitud

        <DataMember>
        <Description("Min date requested")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Min date requested")>
        Public Property MinDate As roWCFDate 'Date1    'TODO: As Nullable(Of XXX) ??

        <DataMember>
        <Description("Max date requested")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Max date requested")>
        Public Property MaxDate As roWCFDate 'Date2

        <DataMember>
        <Description("Cause identifier related with the request")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Cause identifier related with the request")>
        Public Property IDCause As String ' Identificador justificación (el de exportación, no el interno)

        <DataMember>
        <Description("Shift identifier")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Shift identifier")>
        Public Property IDShift As String 'Identificador del horario

        <DataMember>
        <Description("Comments related with the request")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Comments related with the request")>
        Public Property Comments As String 'Comments

        <DataMember>
        <Description("Field Name of the employee")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Field Name of the requested value")>
        Public Property FieldName As String 'FieldName

        <DataMember>
        <Description("Field Value of the Field Name")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Field Value of the Field Name requested")>
        Public Property FieldValue As String 'FieldValue

        <DataMember>
        <Description("Hours")>
        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Hours")>
        Public Property Hours As String 'Hours

        <DataMember>
        <Description("Employee exchange identifier")>
        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Employee exchange identifier")>
        Public Property IDEmployeeExchange As String 'Identificador del EmployeeExchange

        <DataMember>
        <Description("Start Hour requested for the shift")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start Hour requested for the shift")>
        Public Property StartShift As roWCFDate 'StartShift

        <DataMember>
        <Description("FromTime")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="FromTime")>
        Public Property FromTime As roWCFDate 'FromTime

        <DataMember>
        <Description("ToTime")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="ToTime")>
        Public Property ToTime As roWCFDate 'ToTime

        <DataMember>
        <Description("Cost Center Identifier")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Cost Center Identifier")>
        Public Property IDCenter As String 'Identificador del centro de coste

        <DataMember>
        <Description("Days requested")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Days requested")>
        Public Property RequestDays As List(Of roRequestDayStandard) 'Días de vacaciones

        <DataMember>
        <Description("Approvals history")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Approvals history")>
        Public Property Approvals As List(Of roRequestApprovalStandard) 'Aprobaciones

    End Class

    <DataContract(Name:="Requests")>
    <Description("Information related to the requests days of a request.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the requests days of a request.")>
    Public Class roRequestDayStandard

        <DataMember>
        <Description("Date of the day added to the request. Only applies to request that can select days individually")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Date of the day added to the request. Only applies to request that can select days individually")>
        Public Property RequestDate As roWCFDate 'Fecha de creación de la solicitud

        <DataMember>
        <Description("When true request day applies to whole day. If not information saved to fromtime/totime")>
        <SwaggerWcfProperty(Required:=False, Default:="false", Description:="When true request day applies to whole day. If not information saved to fromtime/totime")>
        Public Property AllDay As Boolean 'Si la solicitud es para todo el día o no

        <DataMember>
        <Description("Start hour of the request related to request day period")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start hour of the request related to request day period")>
        Public Property FromTime As roWCFDate 'FromTime

        <DataMember>
        <Description("End hour of the request related to request day period")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Start hour of the request related to request day period")>
        Public Property ToTime As roWCFDate 'ToTime

        <DataMember>
        <Description("Min duration requested between start and end hour")>
        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Min duration requested between start and end hour")>
        Public Property Duration As Double 'Duration

        <DataMember>
        <Description("Punch type related to an external work request request")>
        Public Property ActualType As PunchTypeEnum 'ActualType

        <DataMember>
        <Description("Cause identifier related to an external work request")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Cause identifier related to an external work request")>
        Public Property IDCause As String ' Identificador justificación (el de exportación, no el interno)

        <DataMember>
        <Description("Comments added by user related to an external work request")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Comments added by user related to an external work request")>
        Public Property Comments As String 'Comments

    End Class

    <DataContract(Name:="Requests")>
    <Description("Information related to the request history.")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the request history.")>
    Public Class roRequestApprovalStandard

        <DataMember>
        <Description("Datetime of the request action")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Datetime of the request action")>
        Public Property ApprovalDateTime As roWCFDate 'Fecha de creación de la solicitud

        <DataMember>
        <Description("Comments added by the supervisor")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Comments added by the supervisor")>
        Public Property Comments As String 'Comments

        <DataMember>
        <Description("Identifier of the supervisor that has done the action")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Identifier of the supervisor that has done the action")>
        Public Property IDPassport As Integer 'IDPassport

        <DataMember>
        <Description("Status of the request after the supervisor approval/refuse")>
        Public Property Status As eRequestStatus 'Status

        <DataMember>
        <Description("Approval level of the passport that has done the approval")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Approval level of the passport that has done the approval")>
        Public Property StatusLevel As Integer 'StatusLevel

    End Class

    <DataContract(Name:="terminalConfiguration")>
    <Description("Information related to a terminal's configuration")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Information related to a terminal's configuration")>
    Public Class roTerminalConfiguration
        Public Sub New()
            Me.AuthorizedEmployees = {}
            Me.AccessPeriods = {}
        End Sub

        <DataMember>
        <Description("List of IDs of employees authorized's on terminal")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of IDs of employees authorized's on terminal")>
        Public Property AuthorizedEmployees As roEmployeeOnAccessAuthorization() '

        <DataMember>
        <Description("List of access periods")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="List of access periods")>
        Public Property AccessPeriods As roAccessPeriod()
    End Class

    <DataContract(Name:="Access period")>
    <Description("Terminal's access period")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Access period in which users can access a terminal. Indicates the day of the week in which users can access and the period of time during which they can do so")>
    Public Class roAccessPeriod

        <DataMember>
        <Description("Day of the week which users can access the terminal")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Day of week (1 = Monday - 7 = Sunday)")>
        Public Property DayOf As String

        <DataMember>
        <Description("Time when the access period begins")>
        <SwaggerWcfProperty(Required:=True, Default:="00:00", Description:="Hour in which the access period starts. Format is HH:mm")>
        Public Property BeginTime As String

        <DataMember>
        <Description("Time when the access period ends")>
        <SwaggerWcfProperty(Required:=True, Default:="00:00", Description:="Hour in which the access period ends. Format is HH:mm")>
        Public Property EndTime As String

    End Class

    <DataContract(Name:="roEmployeeOnAccessAuthorization")>
    <Description("Minimum information of a user to show in access authorizations")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Minimum information of a user to show in access authorizations")>
    Public Class roEmployeeOnAccessAuthorization

        <DataMember(Name:="Id")>
        <Description("User identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="12", Description:="User identifier")>
        Public Property Id As String

    End Class

    <DataContract(Name:="roDailyCause")>
    <Description("Daily Cause")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="An user Daily Cause")>
    Public Class roDailyCause

        <DataMember(Name:="EmployeeId")>
        <Description("Unique user identifier")>
        <SwaggerWcfProperty(Required:=True, Default:="1", Description:="Unique user identifier")>
        Public Property EmployeeId As String

        <DataMember>
        <Description("Short name of the cause in Visualtime")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Short name of the cause in Visualtime")>
        Public Property CauseShortName As String

        <DataMember>
        <Description("Date of the cause.")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Date of the DailyCause")>
        Public Property DateTime As roWCFDate

        <DataMember>
        <Description("Value desired for the DailyCause")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Value desired for the DailyCause. The value is in minutes. If the value is zero the daily cause will be removed if it exists and is manual")>
        Public Property Value As String

        <DataMember>
        <Description("Indicates whether the cause was created manually or by the system. Cannot be false.")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Indicates whether the cause was created manually or by the system. Cannot be false.")>
        Public Property Manual As String

        <DataMember>
        <Description("Incidence related with the cause.This field is read-only.")>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Incidence related with the cause. This field is read-only.")>
        Public Property IDIncidence As String


        <DataMember>
        <Description("Data of the incidence.")>
        <SwaggerWcfProperty(Required:=True, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Data of the DailyIncidence")>
        Public Property Incidence As roDailyIncidence

        <DataMember>
        <Description("Cause equivalence code.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Cause equivalence code. This field is read-only.")>
        Public Property CauseEquivalenceCode As String


    End Class


    <DataContract(Name:="roDailyIncidence")>
    <Description("Daily Incidence")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="An user Daily Incidence")>
    Public Class roDailyIncidence
        <DataMember>
        <Description("Incidence related with the cause.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Incidence related with the cause. This field is read-only.")>
        Public Property IDIncidence As String

        <DataMember>
        <Description("Date of the incidence.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="yyyy-MM-dd HH:mm:ss zz", Description:="Date of the DailyIncidence. This field is read-only.")>
        Public Property DateTime As roWCFDate

        <DataMember>
        <Description("Incidence type.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Incidence type. This field is read-only.")>
        Public Property IncidenceType As String

        <DataMember>
        <Description("Begin time of incidence.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Begin time of incidence. This field is read-only.")>
        Public Property BeginTime As roWCFDate

        <DataMember>
        <Description("End time of incidence.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="End time of incidence. This field is read-only.")>
        Public Property EndTime As roWCFDate

        <DataMember>
        <Description("Zone of the incidence.This field is read-only.")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Incidence zone. This field is read-only.")>
        Public Property Zone As String

        <DataMember>
        <Description("Value for the DailyIncidence")>
        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Value for the DailyIncidence. The value is in minutes. This field is read-only.")>
        Public Property Value As String


    End Class


    <DataContract(Name:="API Protection Data")>
    <Description("Data protection configuration")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Configure data protection properties for API operations")>
    Public Class roAPIProtectionData

        <DataMember>
        <Description("When true, employee contracts can not be modified if relevant data should be removed as a result")>
        <SwaggerWcfProperty(Required:=False, Default:="false", Description:="When true, employee contracts can not be modified if relevant data should be removed as a result")>
        Public Property ContractData As Boolean

    End Class

End Namespace