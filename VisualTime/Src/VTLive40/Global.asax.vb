Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Mvc
Imports System.Web.Optimization
Imports System.Web.Routing
Imports DevExpress.Web
Imports DevExpress.XtraReports.Native
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.VTLiveMvC.Support

Public Class Global_asax
    Inherits roBaseGlobalAsax

    Private Shared _updateSessionExculdeMethods = {"UserTasksCheck", "ashx"}

#Region "Base"
    Protected Overrides Function GetLoggedInPassportFromSession() As roPassportTicket
        Dim oPassportTicket As roPassportTicket = Nothing
        Try
            oPassportTicket = WLHelperWeb.CurrentPassport(True)
        Catch ex As Exception
            oPassportTicket = Nothing
        End Try

        Return oPassportTicket
    End Function

    Protected Overrides Function GetLoggedInPassportIdFromSession() As Integer
        Dim idPassport As Integer = Nothing
        Try
            idPassport = If(WLHelperWeb.CurrentPassport(True)?.ID, -1)
        Catch ex As Exception
            idPassport = -1
        End Try

        Return idPassport
    End Function
    Private Function OnValidateCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslPolicyErrors As Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
#End Region


#Region "Global asax events"
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.VTLive, roLiveQueueTypes.vtlive)

        Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf OnValidateCertificate

        MvcHandler.DisableMvcResponseHeader = True

        DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles)
        ASPxWebControl.BackwardCompatibility.DataControlAllowReadUnlistedFieldsFromClientApiDefaultValue = True
        ASPxWebControl.BackwardCompatibility.DataControlAllowReadUnexposedColumnsFromClientApiDefaultValue = True
        DevExpress.Data.Helpers.ServerModeCore.DefaultForceCaseInsensitiveForAnySource = True
        DevExpress.Utils.AzureCompatibility.Enable = True
        DevExpress.XtraReports.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.ReadOnly
        DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.ReadOnly
        DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions
        DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.ReadOnly

        DevExpress.XtraReports.Web.ReportDesigner.DefaultReportDesignerContainer.EnableCustomSql()
        DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(New roReportStorage)

        'DevExpress.Utils.DeserializationSettings.RegisterTrustedAssembly("DevExpress.Web.v23.1, Version=23.1.5.0, Culture=neutral, PublicKeyToken=B88D1754D700E49A")
        'DevExpress.Utils.DeserializationSettings.RegisterTrustedAssembly("DevExpress.Web.ASPxPivotGrid.v23.1, Version=23.1.5.0, Culture=neutral, PublicKeyToken=B88D1754D700E49A")

        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(System.Web.UI.WebControls.WebColorConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataTextColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataImageColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataComboBoxColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataDateColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewCommandColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataImageColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataCheckColumn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Web.GridViewDataProgressBarColumn))


        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(DevExpress.Data.PivotGrid.PivotErrorValue))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.Report))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportCategory))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportExecution))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportParameter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportPassport))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportPermissions))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportPlannedDestination))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportPlannedExecution))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ReportPlannedExecution.ReportPlannedExecutionComparer))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.userFieldIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.passportIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.employeesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.betweenYearAndMonthSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.causeIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.causesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.conceptGroupsSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.conceptIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.conceptsSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterProfileTypesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterSelectorCausesRegistroJL))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterSelectorConceptsRegistroJL))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterValuesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.formatSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.functionCall))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.incidenceIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.incidencesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.shiftsSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.holidaysSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.taskIdentifier))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.tasksSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.terminalsSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.userFieldIdentifierConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.userFieldsSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.userFieldsSelectorRadioBtn))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.viewAndFormatSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.accessTypeSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.yearAndMonthSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.zonesSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.projectsVSLSelector))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.betweenYearAndMonthSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.causeIdentifierTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.CausesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ConceptGroupsSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.conceptIdentifierTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ConceptSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.EmployeesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterProfileTypesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterSelectorCausesRegistroJLTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterSelectorConceptsRegistroJLTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.filterValuesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.formatSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.FunctionCallTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.incidenceIdentifierTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.IncidencesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.PassportIdentifierTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ShiftsSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.HolidaysSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.TaskIdentifierTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.TasksSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.TerminalsSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.UserFieldsSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.UserFieldsSelectorRadioBtnTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.viewAndFormatSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.accessTypeSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.yearAndMonthSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.ZonesSelectorTypeConverter))
        DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(GetType(Robotics.Base.projectsVSLSelectorTypeConverter))

        SerializationService.RegisterSerializer(CustomDataSerializer.Name, New CustomDataSerializer())

        'Se desencadena al iniciar la aplicación
        ModelBinders.Binders.DefaultBinder = New Mvc.DevExpressEditorsBinder()
        AreaRegistration.RegisterAllAreas()
        RegisterGlobalFilters(GlobalFilters.Filters)
        RegisterRoutes(RouteTable.Routes)

    End Sub

    Sub Application_PreSendRequestHeaders()
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
    End Sub

    Protected Sub Application_AcquireRequestState(sender As Object, e As EventArgs)
        Dim absolutePath As String = HttpContext.Current.Request.Url.AbsolutePath
        If absolutePath.Contains(".png") OrElse absolutePath.Contains(".gif") OrElse
            absolutePath.Contains(".jpeg") OrElse absolutePath.Contains(".jpg") OrElse
            absolutePath.Contains(".js") OrElse absolutePath.Contains(".css") OrElse
            absolutePath.Contains(".eot") OrElse absolutePath.Contains(".svg") OrElse
            absolutePath.Contains(".woff") OrElse absolutePath.Contains(".otf") OrElse
            absolutePath.Contains(".ttf") Then
            Return
        End If

        Me.OnApplicationBeginRequest()

        Dim bUpdateSession As Boolean = HttpContext.Current.Request.Url.PathAndQuery.Contains("aspx")
        If bUpdateSession Then
            For Each oExcludedFunction In _updateSessionExculdeMethods
                If HttpContext.Current.Request.Url.PathAndQuery.Contains(oExcludedFunction) Then bUpdateSession = False
            Next
        End If

        If HttpContext.Current.Session IsNot Nothing AndAlso CompanyId <> String.Empty AndAlso bUpdateSession Then
            If API.SecurityServiceMethods.GetPassportTicketBySessionID(Nothing, WLHelperWeb.CurrentPassportID, WLHelperWeb.CurrentPassportAuthMethod) IsNot Nothing AndAlso roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NoError Then
                API.SecurityServiceMethods.UpdateLastAccessTimeMVC(Nothing)
            End If
        End If



        If AuthValidations.GetCompanyWithSessionInitiated() <> String.Empty AndAlso
            Not HttpContext.Current.Request.Url.AbsolutePath.Contains("/LoginWeb.aspx") AndAlso
            Not HttpContext.Current.Request.Url.AbsolutePath.Contains("/Base/Ooops") AndAlso
            SessionHelper.GetCurrentPassportSessionID(WLHelperWeb.CurrentPassportID, roAppSource.VTLive) <> HttpContext.Current.Session.SessionID Then

            WLHelperWeb.CurrentPassport = Nothing

            Response.Redirect("~/Base/Ooops.aspx")
        End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        'Dim oCN As roBaseConnection = Robotics.DataLayer.roCacheManager.GetInstance().GetConnection()

        'If oCN IsNot Nothing AndAlso oCN.IsOpen() Then
        '    Robotics.DataLayer.roCacheManager.GetInstance().RemoveCurrentConnection()
        'End If

    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        Me.OnApplicationEndRequest()
    End Sub

#End Region

#Region "MVC config"
    Public Shared Sub RegisterGlobalFilters(ByVal filters As GlobalFilterCollection)
        filters.Add(New HandleErrorAttribute())
    End Sub

    Public Shared Sub RegisterRoutes(routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.IgnoreRoute("{resource}.aspx/{*pathInfo}")

        routes.MapPageRoute(
            routeName:="Auth",
            routeUrl:="Auth/{clientId}",
            physicalFile:="~/Auth/VTLiveAuth.aspx"
        )

        routes.MapPageRoute(
            routeName:="AuthCheck",
            routeUrl:="AuthCheck/{clientId}",
            physicalFile:="~/Auth/VTCheckAuth.aspx"
        )

        routes.MapPageRoute(
            routeName:="PortalAuth",
            routeUrl:="PortalAuth/{clientId}",
            physicalFile:="~/Auth/VTPortalAuth.aspx"
        )

        routes.MapPageRoute(
            routeName:="biocertificate",
            routeUrl:="biocertificate/{clientId}/{certificateName}",
            physicalFile:="~/Documents/downloadFile.aspx"
        )

        routes.MapRoute(
            name:="DEX",
            url:="DEX/{id}/{action}",
            defaults:=New With {.controller = "DEX", .action = "Index", .id = UrlParameter.Optional}
        )

        routes.MapRoute(
            name:="Default",
            url:="{controller}/{action}/{id}",
            defaults:=New With {.controller = "Home", .action = "Index", .id = UrlParameter.Optional}
        )


    End Sub
#End Region
End Class