Imports System.Web.Mvc
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

''' <summary>
''' Attribute to check the required permissions in requests.
''' </summary>
Public Class LoggedInAtrribute
    Inherits ActionFilterAttribute

    Public Requiered As Boolean

    Private ReadOnly AccessDeniedRedirectUrl As String = String.Format("/{0}/Base/AccessDenied.aspx", Configuration.RootUrl)

    ''' <summary>
    ''' Executed method in actions with permissions required.
    ''' </summary>
    ''' <param name="filterContext">Executed context (controller/action)</param>
    Public Overrides Sub OnActionExecuting(ByVal filterContext As ActionExecutingContext)
        Dim actionName As String = filterContext.ActionDescriptor.ActionName
        Dim controllerName As String = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName

        If Not HasAccess(actionName, controllerName) Then
            filterContext.Result = New RedirectResult(AccessDeniedRedirectUrl)
        Else
            MyBase.OnActionExecuting(filterContext)
        End If
    End Sub

    ''' <summary>
    ''' Validate the right permissions
    ''' </summary>
    ''' <param name="Action">Action executed</param>
    ''' <param name="Controller">Controller executed</param>
    ''' <returns></returns>
    Private Function HasAccess(ByVal Action As String, ByVal Controller As String) As Boolean
        If HttpContext.Current.Session IsNot Nothing Then

            If Me.Requiered Then

                If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return True
            End If
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