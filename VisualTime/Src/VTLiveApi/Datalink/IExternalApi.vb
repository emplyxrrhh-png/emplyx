Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

<ServiceContract()>
Public Interface IExternalApi

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function CreateOrUpdateEmployee(CompanyCode As String, UserName As String, UserPwd As String, oEmployee As roDatalinkStandarEmployee) As Integer

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function CreateOrUpdateDocument(CompanyCode As String, UserName As String, UserPwd As String, oDocument As roDatalinkStandardDocument) As roDatalinkStandarDocumentResponse

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function GetAccruals(CompanyCode As String, UserName As String, UserPwd As String, oAccrualCriteria As roDatalinkStandarAccrualCriteria) As roDatalinkStandarAccrualResponse

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function GetAbsences(CompanyCode As String, UserName As String, UserPwd As String, oAbsenceCriteria As roDatalinkStandarAbsenceCriteria) As roDatalinkStandarAbsenceResponse

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function CreateOrUpdateCalendar(CompanyCode As String, UserName As String, UserPwd As String, oCalendar As roDatalinkStandarCalendar) As Integer

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function CreateOrUpdateAbsence(CompanyCode As String, UserName As String, UserPwd As String, oAbsence As roDatalinkStandarAbsence) As Integer

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function CreateOrUpdateHolidays(CompanyCode As String, UserName As String, UserPwd As String, oHolidays As roDatalinkStandarHolidays) As Integer

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function GetPunches(CompanyCode As String, UserName As String, UserPwd As String, timestamp As DateTime) As roDatalinkStandardPunchResponse

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function GetPunchesBetweenDates(CompanyCode As String, UserName As String, UserPwd As String, StartDate As Date, ByVal EndDate As Date) As roDatalinkStandardPunchResponse

    <OperationContract>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Function AddPunches(CompanyCode As String, UserName As String, UserPwd As String, oPunchList As Generic.List(Of roDatalinkStandardPunch)) As roDatalinkStandardPunchResponse

End Interface