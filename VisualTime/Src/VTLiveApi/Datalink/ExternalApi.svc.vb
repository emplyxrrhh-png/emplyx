Imports System.ServiceModel.Activation
Imports Robotics.Base.DTOs
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Required)>
Public Class ExternalApi
    Implements IExternalApi

    Public Function CreateOrUpdateEmployee(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oEmployee As roDatalinkStandarEmployee) As Integer Implements IExternalApi.CreateOrUpdateEmployee
        Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError

        Try

            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                Return Core.DTOs.ReturnCode._MissingCompanyName
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                iResultCode = Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmp As Integer = 0
                externAccessInstance.CreateOrUpdateEmployee(oEmployee, iResultCode, iNewEmp)
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            iResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return iResultCode
    End Function

    Public Function CreateOrUpdateAbsence(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oAbsence As roDatalinkStandarAbsence) As Integer Implements IExternalApi.CreateOrUpdateAbsence
        Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError

        Try

            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                Return Core.DTOs.ReturnCode._MissingCompanyName
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                iResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateAbsence(oAbsence, iResultCode)
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            iResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return iResultCode
    End Function

    Public Function CreateOrUpdateHolidays(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oHolidays As roDatalinkStandarHolidays) As Integer Implements IExternalApi.CreateOrUpdateHolidays
        Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                Return Core.DTOs.ReturnCode._MissingCompanyName
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                iResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateHolidays(oHolidays, iResultCode)
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            iResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return iResultCode
    End Function

    Public Function CreateOrUpdateCalendar(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oCalendar As roDatalinkStandarCalendar) As Integer Implements IExternalApi.CreateOrUpdateCalendar
        Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                Return Core.DTOs.ReturnCode._MissingCompanyName
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                iResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateCalendar(oCalendar, iResultCode)
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            iResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return iResultCode
    End Function

    Public Function GetAbsences(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oAbsenceCriteria As roDatalinkStandarAbsenceCriteria) As roDatalinkStandarAbsenceResponse Implements IExternalApi.GetAbsences
        Dim oDatalinkStandarAbsenceResponse As New roDatalinkStandarAbsenceResponse

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandarAbsenceResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetAbsences(oAbsenceCriteria, oDatalinkStandarAbsenceResponse)
            Else
                oDatalinkStandarAbsenceResponse.ResultCode = iResultCode
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oDatalinkStandarAbsenceResponse
    End Function

    Public Function GetAccruals(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oAccrualCriteria As roDatalinkStandarAccrualCriteria) As roDatalinkStandarAccrualResponse Implements IExternalApi.GetAccruals
        Dim oDatalinkStandarAccrualResponse As New roDatalinkStandarAccrualResponse

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandarAccrualResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetAccruals(oAccrualCriteria, oDatalinkStandarAccrualResponse)
            Else
                oDatalinkStandarAccrualResponse.ResultCode = iResultCode
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oDatalinkStandarAccrualResponse
    End Function

    Public Function CreateOrUpdateDocument(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oDocument As roDatalinkStandardDocument) As roDatalinkStandarDocumentResponse Implements IExternalApi.CreateOrUpdateDocument
        Dim oDatalinkStandarDocumentResponse As New roDatalinkStandarDocumentResponse
        Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
        Dim sResultMessage As String = String.Empty

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandarDocumentResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandarDocumentResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                iResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateDocument(oDocument, UserName, iResultCode, sResultMessage)
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            iResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        oDatalinkStandarDocumentResponse.ResultDetails = sResultMessage
        oDatalinkStandarDocumentResponse.ResultCode = iResultCode
        Return oDatalinkStandarDocumentResponse
    End Function

    Public Function GetPunches(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal timestamp As DateTime) As roDatalinkStandardPunchResponse Implements IExternalApi.GetPunches
        Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandardPunchResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunches(PunchFilterType.ByTimeStamp, timestamp, timestamp.Date, timestamp.Date, oDatalinkStandardPunchResponse)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultCode
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oDatalinkStandardPunchResponse
    End Function

    Public Function GetPunchesBetweenDates(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal StartDate As Date, ByVal EndDate As Date) As roDatalinkStandardPunchResponse Implements IExternalApi.GetPunchesBetweenDates
        Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandardPunchResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunches(PunchFilterType.ByDatePeriod, StartDate, StartDate, EndDate, oDatalinkStandardPunchResponse)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultCode
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oDatalinkStandardPunchResponse
    End Function

    Public Function AddPunches(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oPunchList As Generic.List(Of roDatalinkStandardPunch)) As roDatalinkStandardPunchResponse Implements IExternalApi.AddPunches
        Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

        Try
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                CompanyCode = RoboticsExternAccess.GetCompanyFromUserName(UserName)
            End If

            If CompanyCode = String.Empty Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MissingCompanyName
                Return oDatalinkStandardPunchResponse
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iResultCode As Integer = Core.DTOs.ReturnCode._UnknownError
            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResultCode) Then
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.AddPunches(oPunchList, oDatalinkStandardPunchResponse)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultCode
            End If
            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oDatalinkStandardPunchResponse
    End Function

End Class