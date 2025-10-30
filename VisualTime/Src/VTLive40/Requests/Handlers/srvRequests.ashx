<%@ WebHandler Language="VB" Class="srvRequests" %>

Imports System
Imports System.Web
Imports Robotics.Web.Base
Imports Robotics.VTBase
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTRequests.Requests

Public Class srvRequests
    Inherits handlerBase

    Private intActiveTab As Integer = 0

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "srvRequests"

        Select Case context.Request("action")
            Case "getSupervisorsPending"
                If Me.CheckPermissionsAny Then
                    LoadRequestSupervisorPending(roTypes.Any2Integer(Request("IDRequest")))
                End If
            Case "getBarButtons"
                GetBarButtons()

        End Select
    End Sub

#Region "Methods"
    Private Sub GetBarButtons()
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Requests\Requests", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, "", Me.Language, Me.DefaultScope, "Requests")

            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub LoadRequestSupervisorPending(ByVal intIDRequest As Integer)
        Try
            If intIDRequest > 0 Then
                Dim oRequestSupervisors As String = API.RequestServiceMethods.GetRequestPendingSupervisors(Nothing, intIDRequest, True)

                ' Mostrar información del siguiente supervisor que tiene que cursar la solicitud
                If oRequestSupervisors <> "" Then
                    Dim oMsgParams As New Generic.List(Of String)
                    oMsgParams.Add(oRequestSupervisors)
                    Dim translatedStr As String = Me.Language.Translate("Request.NextApprovalInfo", Me.DefaultScope, oMsgParams)
                    Response.Clear()
                    Response.Write(translatedStr)
                    Response.Flush()
                Else
                    Dim translatedStr As String = Me.Language.Translate("Request.AlreadyApproved", Me.DefaultScope)
                    Response.Clear()
                    Response.Write(translatedStr)
                    Response.Flush()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function CheckPermissionsAny() As Boolean

        ' Verificar permisos
        Dim bolHasPermission As Boolean = False
        Dim oRequestsTypeSecurity As Generic.List(Of roRequestTypeSecurity) = API.RequestServiceMethods.GetRequestTypeSecurityListAll(Nothing)
        For Each oRequestTypeSecurity As roRequestTypeSecurity In oRequestsTypeSecurity
            If Me.HasFeaturePermission(oRequestTypeSecurity.SupervisorFeatureName, Permission.Read) Then
                bolHasPermission = True
                Exit For
            End If
        Next

        Return bolHasPermission

    End Function

#End Region


End Class