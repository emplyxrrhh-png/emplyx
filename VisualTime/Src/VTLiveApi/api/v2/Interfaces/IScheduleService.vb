Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IScheduleService_v2

    <OperationContract>
    <SwaggerWcfPath("Get the holiday for days or hours of a user in a period ", " gets the holiday for days or hours of a user in a period")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetHolidays?CompanyCode={CompanyCode}&Token={Token}&EmployeeID={EmployeeID}&StartDate={StartDate}&EndDate={EndDate}&Criteria={Criteria}")>
    Function GetHolidays(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user who wants to recover the holidays", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Criterion with which vacations recover. By default, those whose dates are included in the indicated period are recovered. If we inform "" Timestamp "", vacations that have been modified / eliminated in the indicated period are recovered", ParameterType:=GetType(String))>
                        Optional Criteria As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = ""
                        ) As roWSResponse(Of roHoliday())

    <OperationContract>
    <SwaggerWcfPath("Create or update a day of a user's vacation ", " Create or update a day of a user's vacation. The properties of the day or holiday hours are indicated on the OHOLIDAYS object ")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateHolidays?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateHolidays(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Object modeling vacation log", ParameterType:=GetType(roHoliday))>
                                   oHolidays As roHoliday,
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional CompanyCode As String = "") As roWSResponse(Of roHoliday())

    <OperationContract>
    <SwaggerWcfPath("Create or update a day planning for a user, "" creates or updates a day planning for a user. The details of the planning are indicated in the Ocalendar object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateCalendar?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateCalendar(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Object that models the planning record", ParameterType:=GetType(roCalendar))>
                                   oCalendar As roCalendar,
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional CompanyCode As String = "") As roWSResponse(Of roCalendar())

    <OperationContract>
    <SwaggerWcfPath("Create or update the planning of one or more days of users, "" creates or updates the planning of one or more days of users. Planning corresponds to one or more users and for one or more days. Details are provided on a list of Ocalendar objects. A maximum of 50 records are processed")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateCalendarBatch?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateCalendarBatch(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Object that models the array of planning records", ParameterType:=GetType(roCalendar()))>
                                   oCalendars As roCalendar(),
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional CompanyCode As String = "") As roWSResponse(Of roCalendarResponse())

    <OperationContract>
    <SwaggerWcfPath("Update the lock date")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="UpdateLockDate?CompanyCode={CompanyCode}&Token={Token}&LockDate={LockDate}")>
    Function UpdateLockDate(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Lock date value", ParameterType:=GetType(Date))>
                                   LockDate As Date,
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional CompanyCode As String = "") As roWSResponse(Of roLockDateResponse)

    <OperationContract>
    <SwaggerWcfPath("Get the list of schedules ", " Get the list of schedules")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetShifts?CompanyCode={CompanyCode}&Token={Token}&ShiftID={ShiftID}")>
    Function GetShifts(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the shift who wants to recover definition", ParameterType:=GetType(String))>
                        Optional ShiftID As String = ""
                        ) As roWSResponse(Of roShift())

    <OperationContract>
    <SwaggerWcfPath("Get the list of holiday templates ", " Get the list of holiday templates")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPublicHolidays?CompanyCode={CompanyCode}&Token={Token}")>
    Function GetPublicHolidays(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = ""
                        ) As roWSResponse(Of roPublicHoliday())

    <OperationContract>
    <SwaggerWcfPath("Get planning for one or more users and a period ", " obtains planning for one or more users and a period. Users and period are indicated in the Ocalendarcriteria object. A maximum of 50 users and 366 days are returned.")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetCalendar?CompanyCode={CompanyCode}&Token={Token}&LoadScheduledLayers={LoadScheduledLayers}")>
    Function GetCalendar(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Object that models the schedule selection criteria", ParameterType:=GetType(roCalendarCriteria))>
                        oCalendarCriteria As roCalendarCriteria,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="True for recover shcduled layer info", ParameterType:=GetType(Boolean))>
                        Optional LoadScheduledLayers As Boolean = False) As roWSResponse(Of roCalendar())

    <OperationContract>
    <SwaggerWcfPath("Get planning- moidified after timestamp for one or more users and a period ", " obtains planning moidified after timestamp for one or more users and a period. Users and period are indicated in the Ocalendarcriteria object. A maximum of 50 users and 366 days are returned.")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetCalendarByTimestamp?CompanyCode={CompanyCode}&Token={Token}&Timestamp={Timestamp}&LoadScheduledLayers={LoadScheduledLayers}")>
    Function GetCalendarByTimestamp(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Object that models the schedule selection criteria", ParameterType:=GetType(roCalendarCriteria))>
                        oCalendarCriteria As roCalendarCriteria,
                        <SwaggerWcfParameter(Required:=True, Description:="Date and time from which users are obtained", ParameterType:=GetType(DateTime))>
                        Timestamp As DateTime,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="True for recover shcduled layer info", ParameterType:=GetType(Boolean))>
                        Optional LoadScheduledLayers As Boolean = False) As roWSResponse(Of roCalendar())

    <OperationContract>
    <SwaggerWcfPath("Get the lock date that is applied")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetLockDate?CompanyCode={CompanyCode}&Token={Token}")>
    Function GetLockDate(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = ""
                        ) As roWSResponse(Of roLockDateResponse)

End Interface