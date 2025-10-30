Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IScheduleService

    <OperationContract>
    <SwaggerWcfPath("Obtiene las vacaciones por días u horas de un usuario en un periodo", "Obtiene las vacaciones por días u horas de un usuario en un periodo")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetHolidays?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}&EmployeeID={EmployeeID}&StartDate={StartDate}&EndDate={EndDate}&Criteria={Criteria}")>
    Function GetHolidays(
                         <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Identificador del usuario del que se quieren recuperar las vacaciones", ParameterType:=GetType(String))>
                        EmployeeID As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Criterio con el que se recuperan las vacaciones. Por defecto, se recuperan aquellas cuyas fechas estén comprendidas en el periodo indicado. Si informamos ""timestamp"", se recuperan las vacaciones que hayan sido modificadas/eliminadas en el periodo indicado", ParameterType:=GetType(String))>
                        Optional Criteria As String = ""
                        ) As roWSResponse(Of roHoliday())

    <OperationContract>
    <SwaggerWcfPath("Crea o actualiza un día de vacaciones de un usuario", "Crea o actualiza un día de vacaciones de un usuario. Las propiedades del día u horas de vacaciones se indican en el objeto oHolidays ")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateHolidays?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function CreateOrUpdateHolidays(
                                   <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   CompanyCode As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                                   UserName As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                                   UserPwd As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el registro de vacaciones", ParameterType:=GetType(roHoliday))>
                                   oHolidays As roHoliday) As roWSResponse(Of roHoliday())

    <OperationContract>
    <SwaggerWcfPath("Crea o actualiza la planificación de un día para un usuario", "Crea o actualiza la planificación de un día para un usuario. Los detalles de la planificación se indican en el objeto oCalendar")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateCalendar?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function CreateOrUpdateCalendar(
                                   <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   CompanyCode As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                                   UserName As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                                   UserPwd As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el registro de planificación", ParameterType:=GetType(roCalendar))>
                                    oCalendar As roCalendar) As roWSResponse(Of roCalendar())

    <OperationContract>
    <SwaggerWcfPath("Crea o actualiza la planificación de uno o varios días de usuarios", "Crea o actualiza la planificación de uno o varios días de usuarios. La planificación corresponde a uno o varios usuarios y para uno o varios días. Los detalles se proporcionan en una lista de objetos oCalendar. Se procesan un máximo 50 registros")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateCalendarBatch?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function CreateOrUpdateCalendarBatch(
                                   <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   CompanyCode As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                                   UserName As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                                   UserPwd As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el array de registros de planificación", ParameterType:=GetType(roCalendar()))>
                                    oCalendars As roCalendar()) As roWSResponse(Of roCalendarResponse())

    <OperationContract>
    <SwaggerWcfPath("Obtiene la lista de horarios", "Obtiene la lista de horarios")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetShifts?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function GetShifts(
                        <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String
                        ) As roWSResponse(Of roShift())

    <OperationContract>
    <SwaggerWcfPath("Obtiene las lista de plantillas de festivos", "Obtiene las lista de plantillas de festivos")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPublicHolidays?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function GetPublicHolidays(
                         <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String
                        ) As roWSResponse(Of roPublicHoliday())

    <OperationContract>
    <SwaggerWcfPath("Obtiene la planificación para uno o varios usuarios y un periodo", "Obtiene la planificación para uno o varios usuarios y un periodo. Los usuarios y periodo se indican en el objeto oCalendarCriteria. Se devuelven como máximo 50 usuarios y 366 días.")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetCalendar?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function GetCalendar(
                         <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela los criterios de selección del calendario", ParameterType:=GetType(roCalendarCriteria))>
                        oCalendarCriteria As roCalendarCriteria) As roWSResponse(Of roCalendar())

End Interface