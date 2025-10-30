Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/PunchesService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class PunchesService_v2
    Implements IPunchesService_v2

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetPunches(ByVal Token As String, ByVal Timestamp As DateTime, Optional ByVal CompanyCode As String = "", Optional ByVal EmployeeID As String = "") As roWSResponse(Of roPunch()) Implements IPunchesService_v2.GetPunches
        Dim oWSResponse As New roWSResponse(Of roPunch()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            If EmployeeID Is Nothing Then EmployeeID = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oPunches As New List(Of roPunch)
            Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunchesEx(DTOs.PunchFilterType.ByTimeStamp, Timestamp, Timestamp.Date, Timestamp.Date, oDatalinkStandardPunchResponse, EmployeeID)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultStatus
                iResultText = iResultStatus.ToString
            End If

            If oDatalinkStandardPunchResponse IsNot Nothing Then
                If oDatalinkStandardPunchResponse.Punches IsNot Nothing Then oWSResponse.Value = oDatalinkStandardPunchResponse.Punches.ConvertAll(AddressOf XStandardPunchToPunchConverter).ToArray
            End If

            oWSResponse.Status = oDatalinkStandardPunchResponse.ResultCode
            oWSResponse.Text = iResultText

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetPunchesBetweenDates(ByVal Token As String, ByVal StartDate As Date, ByVal EndDate As Date, Optional ByVal CompanyCode As String = "", Optional ByVal EmployeeID As String = "") As roWSResponse(Of roPunch()) Implements IPunchesService_v2.GetPunchesBetweenDates
        Dim oWSResponse As New roWSResponse(Of roPunch()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            If EmployeeID Is Nothing Then EmployeeID = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oPunches As New List(Of roPunch)
            Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunches(DTOs.PunchFilterType.ByDatePeriod, StartDate, StartDate, EndDate, oDatalinkStandardPunchResponse, EmployeeID)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultStatus
                iResultText = iResultStatus.ToString
            End If

            If oDatalinkStandardPunchResponse IsNot Nothing Then
                If oDatalinkStandardPunchResponse.Punches IsNot Nothing Then oWSResponse.Value = oDatalinkStandardPunchResponse.Punches.ConvertAll(AddressOf XStandardPunchToPunchConverter).ToArray
            End If

            oWSResponse.Status = oDatalinkStandardPunchResponse.ResultCode
            oWSResponse.Text = iResultText

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function AddPunches(ByVal Token As String, ByVal lPunches As roPunch(), Optional ByVal CompanyCode As String = "") As roWSResponse(Of roPunchesResponse()) Implements IPunchesService_v2.AddPunches
        Dim oWSResponse As New roWSResponse(Of roPunchesResponse()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim lPunchesDatalink As New List(Of roDatalinkStandardPunch)
            Dim lstStandardPunchesResponse As New roDatalinkStandardPunchResponse
            Dim lstPunchesResponse As New List(Of roPunchesResponse)

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                lPunchesDatalink = lPunches.ToList.ConvertAll(AddressOf XPunchToStandardPunchConverter)
                externAccessInstance.AddPunches(lPunchesDatalink, lstStandardPunchesResponse)
            End If

            If lstStandardPunchesResponse.PunchesListError IsNot Nothing Then
                oWSResponse.Value = lstStandardPunchesResponse.PunchesListError.ConvertAll(AddressOf XStandardPunchToPunchResponseConverter).ToArray
            Else
                oWSResponse.Value = {}
            End If

            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function UpdatePunches(ByVal Token As String, ByVal lPunchesCriteria As roPunchCriteria(), Optional ByVal CompanyCode As String = "") As roWSResponse(Of List(Of roPunchesResponse)) Implements IPunchesService_v2.UpdatePunches
        Dim oWSResponse = New roWSResponse(Of List(Of roPunchesResponse)) With {
                .Status = Core.DTOs.ReturnCode._UnknownError,
                .Text = String.Empty,
                .Value = New List(Of roPunchesResponse)
            }
        Dim bContinueUpdate As Boolean = True
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                Dim lPunchesDatalink As New List(Of roDatalinkStandardPunch)
                Dim lstStandardPunchesResponse As New roDatalinkStandardPunchResponse
                Dim lstPunchesResponse As New List(Of roPunchesResponse)

                Dim oPunchErrorResponse As New roDatalinkStandardPunch()

                Dim lIDPunches As New List(Of String)
                For Each oPunchToUpdateCriteria In lPunchesCriteria
                    lIDPunches.Add(oPunchToUpdateCriteria.ID)
                Next
                externAccessInstance.GetPunchesWithIDEx(lIDPunches, lstStandardPunchesResponse)
                lPunchesDatalink = lstStandardPunchesResponse.Punches

                'Obtenemos los punches que no se han encontrado
                Dim notFound = lIDPunches.Select(Of String)(Function(x) x).Except(lPunchesDatalink.Select(Of String)(Function(x) x.ID))
                For Each id In notFound
                    oPunchErrorResponse.ResultCode = Core.DTOs.ReturnCode._PunchNotFound
                    oPunchErrorResponse.ResultDescription = Core.DTOs.ReturnCode._PunchNotFound.ToString + " - ID: " + id.ToString
                    addErrorToList(oPunchErrorResponse, lstStandardPunchesResponse)
                Next

                'Actualizamos los datos del punch
                If lPunchesDatalink IsNot Nothing AndAlso lPunchesDatalink.Count > 0 Then
                    externAccessInstance.UpdatePunches(lPunchesDatalink, lstStandardPunchesResponse, lPunchesCriteria)
                End If

                If lstStandardPunchesResponse.PunchesListError IsNot Nothing Then
                    oWSResponse.Value.AddRange(lstStandardPunchesResponse.PunchesListError.ConvertAll(AddressOf XStandardPunchToPunchResponseConverter))
                End If
            End If
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function DeletePunches(ByVal Token As String, ByVal lPunchesToDeleteCriteria As roPunchToDeleteCriteria(), Optional ByVal CompanyCode As String = "") As roWSResponse(Of List(Of roPunchesResponse)) Implements IPunchesService_v2.DeletePunches
        Dim oWSResponse = New roWSResponse(Of List(Of roPunchesResponse)) With {
                .Status = Core.DTOs.ReturnCode._UnknownError,
                .Text = String.Empty,
                .Value = New List(Of roPunchesResponse)
            }
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                Dim lPunchesDatalink As New List(Of roDatalinkStandardPunch)
                Dim lstStandardPunchesResponse As New roDatalinkStandardPunchResponse
                Dim oPunchErrorResponse As New roDatalinkStandardPunch()
                Dim lIDPunches As New List(Of String)
                For Each oPunchToDeleteCriteria In lPunchesToDeleteCriteria
                    lIDPunches.Add(oPunchToDeleteCriteria.ID)
                Next
                externAccessInstance.GetPunchesWithIDEx(lIDPunches, lstStandardPunchesResponse)
                lPunchesDatalink = lstStandardPunchesResponse.Punches

                'Obtenemos los punches que no se han encontrado
                Dim notFound = lIDPunches.Select(Of String)(Function(x) x).Except(lPunchesDatalink.Select(Of String)(Function(x) x.ID))
                For Each id In notFound
                    oPunchErrorResponse.ResultCode = Core.DTOs.ReturnCode._PunchNotFound
                    oPunchErrorResponse.ResultDescription = Core.DTOs.ReturnCode._PunchNotFound.ToString + " - ID: " + id.ToString
                    addErrorToList(oPunchErrorResponse, lstStandardPunchesResponse)
                Next

                'Eliminamos los fichajes encontrados en BD
                If lPunchesDatalink IsNot Nothing AndAlso lPunchesDatalink.Count > 0 Then
                    externAccessInstance.DeletePunches(lPunchesDatalink, lstStandardPunchesResponse)
                End If

                If lstStandardPunchesResponse.PunchesListError IsNot Nothing Then
                    oWSResponse.Value.AddRange(lstStandardPunchesResponse.PunchesListError.ConvertAll(AddressOf XStandardPunchToPunchResponseConverter))
                End If

            End If
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    Private Sub addErrorToList(ByRef oPunchErrorResponse As roDatalinkStandardPunch, ByRef lstStandardPunchesResponse As roDatalinkStandardPunchResponse)
        If lstStandardPunchesResponse.PunchesListError Is Nothing Then
            lstStandardPunchesResponse.PunchesListError = New List(Of roDatalinkStandardPunch)
        End If
        lstStandardPunchesResponse.PunchesListError.Add(oPunchErrorResponse)
    End Sub

    Private Function XStandardPunchToPunchConverter(oPunch As roDatalinkStandardPunch) As roPunch
        Dim oRet As roPunch
        Try
            oRet = New roPunch
            oRet.ID = oPunch.ID
            oRet.IDEmployee = oPunch.UniqueEmployeeID
            oRet.Type = oPunch.Type
            oRet.ActualType = oPunch.ActualType
            oRet.ShiftDate = New Robotics.VTBase.roWCFDate(oPunch.ShiftDate)
            oRet.DateTime = New Robotics.VTBase.roWCFDate(oPunch.DateTime)
            oRet.IDTerminal = oPunch.IDTerminal
            oRet.TypeData = oPunch.TypeData
            oRet.GPS = oPunch.GPS
            If Not oPunch.Field1 Is Nothing Then oRet.Field1 = oPunch.Field1
            If Not oPunch.Field2 Is Nothing Then oRet.Field2 = oPunch.Field2
            If Not oPunch.Field3 Is Nothing Then oRet.Field3 = oPunch.Field3
            If Not oPunch.Field4 Is Nothing Then oRet.Field4 = oPunch.Field4
            If Not oPunch.Field5 Is Nothing Then oRet.Field5 = oPunch.Field5
            If Not oPunch.Field6 Is Nothing Then oRet.Field6 = oPunch.Field6
            oRet.Timestamp = New Robotics.VTBase.roWCFDate(oPunch.Timestamp)
            oRet.ResultCode = oPunch.ResultCode
            oRet.ResultDescription = oPunch.ResultDescription
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Private Function XPunchToStandardPunchConverter(oPunch As roPunch) As roDatalinkStandardPunch
        Dim oRet As roDatalinkStandardPunch
        Try
            oRet = New roDatalinkStandardPunch
            oRet.ID = oPunch.ID
            oRet.Type = oPunch.Type
            oRet.ActualType = oPunch.ActualType
            oRet.DateTime = oPunch.DateTime.Data
            oRet.Field1 = oPunch.Field1
            oRet.Field2 = oPunch.Field2
            oRet.Field3 = oPunch.Field3
            oRet.Field4 = oPunch.Field4
            oRet.Field5 = oPunch.Field5
            oRet.Field6 = oPunch.Field6
            oRet.GPS = oPunch.GPS
            oRet.UniqueEmployeeID = oPunch.IDEmployee
            oRet.IDTerminal = oPunch.IDTerminal
            If Not oPunch.Timestamp Is Nothing Then oRet.Timestamp = oPunch.Timestamp.Data
            oRet.TypeData = oPunch.TypeData
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Private Function XStandardPunchToPunchResponseConverter(oPunch As roDatalinkStandardPunch) As roPunchesResponse
        Dim oRet As roPunchesResponse
        Try
            oRet = New roPunchesResponse
            If oPunch IsNot Nothing Then
                oRet.oPunch = XStandardPunchToPunchConverter(oPunch)
            Else
                oRet.oPunch = Nothing
            End If
            oRet.Status = oPunch.ResultCode
            oRet.Text = oPunch.ResultDescription
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class