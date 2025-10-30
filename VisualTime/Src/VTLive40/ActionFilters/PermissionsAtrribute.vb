Imports System.Web.Mvc
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

''' <summary>
''' Attribute to check the required permissions in requests.
''' </summary>
Public Class PermissionsAtrribute
    Inherits ActionFilterAttribute

    Public FeatureAlias As String
    Public Permission As Permission
    Public FeatureType As String = "U"

    Private ReadOnly AccessDeniedRedirectUrl As String = String.Format("/{0}/Base/AccessDenied.aspx", Configuration.RootUrl)

    ''' <summary>
    ''' Executed method in actions with permissions required.
    ''' </summary>
    ''' <param name="filterContext">Executed context (controller/action)</param>
    Public Overrides Sub OnActionExecuting(ByVal filterContext As ActionExecutingContext)
        Dim actionName As String = filterContext.ActionDescriptor.ActionName
        Dim controllerName As String = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName

        If Not CheckAccessRight(actionName, controllerName) Then
            filterContext.Result = New RedirectResult(AccessDeniedRedirectUrl)
        Else
            API.SecurityServiceMethods.UpdateLastAccessTimeMVC(Nothing)
            MyBase.OnActionExecuting(filterContext)
        End If
    End Sub

    ''' <summary>
    ''' Validate the right permissions
    ''' </summary>
    ''' <param name="Action">Action executed</param>
    ''' <param name="Controller">Controller executed</param>
    ''' <returns></returns>
    Private Function CheckAccessRight(ByVal Action As String, ByVal Controller As String) As Boolean
        If HttpContext.Current.Session IsNot Nothing Then

            If Me.Permission = Permission.None Then
                If Action.ToUpper.StartsWith("SAVE") Then
                    Me.Permission = Permission.Write
                ElseIf Action.ToUpper.StartsWith("DELETE") Then
                    Me.Permission = Permission.Admin
                Else
                    Me.Permission = Permission.Read
                End If
            End If

            'Dim oRequestsTypeSecurity As List(Of roRequestTypeSecurity) = API.RequestServiceMethods.GetRequestTypeSecurityListAll(Nothing)
            'Return oRequestsTypeSecurity.Any(Function(security) Me.HasFeaturePermission(security.SupervisorFeatureAlias, Permission.Read))

            Dim bHasPermission = True

            If Not Me.FeatureAlias.Contains(",") Then
                If Me.FeatureAlias.Trim() <> String.Empty Then bHasPermission = Me.HasFeaturePermission(Me.FeatureAlias, Me.Permission, Me.FeatureType)
            Else
                Dim features As String() = Me.FeatureAlias.Split(",")

                If features.Length > 0 AndAlso features(0).Trim() <> String.Empty Then
                    bHasPermission = False
                End If

                For Each oFeature In features
                    If oFeature.Trim() <> String.Empty Then
                        bHasPermission = bHasPermission OrElse Me.HasFeaturePermission(oFeature, Me.Permission, Me.FeatureType)
                    End If
                Next
            End If

            Return bHasPermission
        Else
            Return False
        End If
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="strFeatureAlias"></param>
    ''' <param name="oPermission"></param>
    ''' <param name="strFeatureType"></param>
    ''' <returns></returns>
    Protected Function HasFeaturePermission(ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType, oPermission)
    End Function

End Class