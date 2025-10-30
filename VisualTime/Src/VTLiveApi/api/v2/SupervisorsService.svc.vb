Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/SupervisorsService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class SupervisorsService_v2
    Implements ISupervisorsService_v2

    <SwaggerWcfTag("Supervisors")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function CreateOrUpdateSupervisor(ByVal Token As String, ByVal supervisor As roSupervisor, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roSupervisor) Implements ISupervisorsService_v2.CreateOrUpdateSupervisor
        Dim oWSResponse As roWSResponse(Of roSupervisor) = New roWSResponse(Of roSupervisor)
        Try
            ' WhatEver needed (see other methods for examples)

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus))

            ' ...

            If externAccessInstance.CreateOrUpdateSupervisor(supervisor) Then

                oWSResponse.Value = supervisor
                oWSResponse.Status = Core.DTOs.ReturnCode._OK
                oWSResponse.Text = "Supervisor created or updated successfully."
            Else
                oWSResponse.Value = Nothing
                oWSResponse.Status = Convert.ToInt32(resultStatus)
                oWSResponse.Text = externAccessInstance.ReturnCode.ToString()
            End If

            ' ...

            ' WhatEver needed (see other methods for examples)


        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Supervisors")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetAllSupervisors(ByVal Token As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of List(Of roSupervisor)) Implements ISupervisorsService_v2.GetAllSupervisors
        Dim oWSResponse As roWSResponse(Of List(Of roSupervisor)) = New roWSResponse(Of List(Of roSupervisor))
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty

            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If
            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError

                Dim oSupervisorsResponse = New Generic.List(Of roSupervisor)
                Dim resultText As String = String.Empty

                If externAccessInstance.GetAllSupervisors(oSupervisorsResponse, resultStatus, resultText) Then
                    oWSResponse.Value = oSupervisorsResponse
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = oWSResponse.Status.ToString()
                Else
                    oWSResponse.Value = Nothing
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = oWSResponse.Status.ToString()
                End If
            Else
                oWSResponse.Value = Nothing
                oWSResponse.Status = Convert.ToInt32(resultStatus)
                oWSResponse.Text = resultStatus.ToString()
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""

        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Supervisors")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetRoles(ByVal Token As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of List(Of roRole)) Implements ISupervisorsService_v2.GetRoles
        Dim oWSResponse As New roWSResponse(Of List(Of roRole)) With {
        .Status = Core.DTOs.ReturnCode._UnknownError,
        .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty

            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))
            Dim oRoles As New List(Of roRole)

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                Dim success As Boolean = externAccessInstance.GetRoles(oRoles)

                If success AndAlso externAccessInstance.ReturnCode = Core.DTOs.ReturnCode._OK Then
                    oWSResponse.Value = oRoles
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = resultStatus.ToString()
                Else
                    oWSResponse.Value = Nothing
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = resultStatus.ToString()
                End If
            Else
                oWSResponse.Value = Nothing
                oWSResponse.Status = Convert.ToInt32(resultStatus)
                oWSResponse.Text = resultStatus.ToString()
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""

        Catch ex As Exception
            oWSResponse.Status = Convert.ToInt32(Core.DTOs.ReturnCode._UnknownError)
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString()
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Supervisors")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetCategories(ByVal Token As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of List(Of roCategory)) Implements ISupervisorsService_v2.GetCategories
        Dim oWSResponse As New roWSResponse(Of List(Of roCategory)) With {
        .Status = Core.DTOs.ReturnCode._UnknownError,
        .Text = String.Empty
    }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty

            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))
            Dim oCategories As New List(Of roCategory)

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                Dim success As Boolean = externAccessInstance.GetCategories(oCategories)
                If success AndAlso externAccessInstance.ReturnCode = Core.DTOs.ReturnCode._OK Then
                    oWSResponse.Value = oCategories
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = resultStatus.ToString()
                Else
                    oWSResponse.Value = Nothing
                    oWSResponse.Status = Convert.ToInt32(resultStatus)
                    oWSResponse.Text = resultStatus.ToString()
                End If
            Else
                oWSResponse.Value = Nothing
                oWSResponse.Status = Convert.ToInt32(resultStatus)
                oWSResponse.Text = resultStatus.ToString()
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""

        Catch ex As Exception
            oWSResponse.Status = Convert.ToInt32(Core.DTOs.ReturnCode._UnknownError)
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString()
        End Try

        Return oWSResponse
    End Function

End Class