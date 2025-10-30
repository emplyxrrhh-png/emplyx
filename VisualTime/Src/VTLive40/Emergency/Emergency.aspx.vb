Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Emergency_Emergency
    Inherits PageBase

#Region "Properties"

    Public ReadOnly Property SecretKey() As String
        Get
            Try
                If HttpContext.Current.Session("SecretKey") Is Nothing Then
                    Dim tmp As String = roTypes.Any2String(API.SecurityServiceMethods.GetEmergencyReportKey(Me))
                    If Not String.IsNullOrEmpty(tmp) Then
                        HttpContext.Current.Session("SecretKey") = tmp
                    Else
                        HttpContext.Current.Session("SecretKey") = String.Empty
                    End If
                End If
                Return HttpContext.Current.Session("SecretKey")
            Catch ex As Exception
                HttpContext.Current.Session("SecretKey") = String.Empty
                Return HttpContext.Current.Session("SecretKey")
            End Try
        End Get
    End Property

    Private ReadOnly Property ReportSchedulersList(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roEmergencySchedule)
        Get
            Try
                Dim lstReportScheduler As Generic.List(Of roEmergencySchedule) = HttpContext.Current.Session("Emergency_ReportScheduler")
                If bolReload Then
                    Dim tmpReport As Robotics.Base.Report = API.ReportServiceMethods.GetEmergencyReport(Me.Page, False)
                    lstReportScheduler = New List(Of roEmergencySchedule)
                    If tmpReport IsNot Nothing Then
                        For Each oExecution As Robotics.Base.ReportPlannedExecution In tmpReport.PlannedExecutionsList
                            Dim jsonObject As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(oExecution.ViewFields)
                            Dim schRepName As String = CType(jsonObject, Newtonsoft.Json.Linq.JObject)("description") 'Nombre del report scheduled

                            Dim oReport As New roEmergencySchedule With {
                                .Name = schRepName,
                                .ProfileName = "",
                                .ID = oExecution.Id,
                                .ReportName = tmpReport.Name
                                }
                            lstReportScheduler.Add(oReport)
                        Next
                    End If

                    HttpContext.Current.Session("Emergency_ReportScheduler") = lstReportScheduler
                End If
                Return lstReportScheduler
            Catch ex As Exception
                HttpContext.Current.Session("Emergency_ReportScheduler") = New Generic.List(Of roEmergencySchedule)
                Return HttpContext.Current.Session("Emergency_ReportScheduler")
            End Try
        End Get
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("jQuery", "~/Base/jquery/jquery-3.7.1.min.js", , True)
        Me.InsertExtraJavascript("Emergency", "~/Emergency/Scripts/Emergency.js")
    End Sub

    Protected Sub GetReportList()

        trButtonLanzar.Style.Add("display", "")
        trGrid.Style.Add("display", "")
        trSetCompany.Style.Add("display", "none")
        trCompany.Style.Add("display", "none")

        If Me.SecretKey = String.Empty Then
            trWithKey.Style.Add("display", "none")
            trWithoutKey.Style.Add("display", "")
            trKey.Style.Add("display", "none")
        Else
            trWithKey.Style.Add("display", "")
            trWithoutKey.Style.Add("display", "none")
            trKey.Style.Add("display", "")
        End If

        GridReports.DataSource = ReportSchedulersList(True)
        GridReports.DataBind()

        lblIPText.Text = roBaseState.GetClientAddress(Me.Page.Request)

        'Columnas del grid
        GridReports.Columns(0).Caption = Me.Language.Translate("IsSelected", Me.DefaultScope.ToLowerInvariant()) '"Selecc"
        GridReports.Columns("ID").Visible = False
        GridReports.Columns("Name").Caption = Me.Language.Translate("Name", Me.DefaultScope.ToLowerInvariant()) 'Nombre
        GridReports.Columns("ProfileName").Caption = Me.Language.Translate("ProfileName", Me.DefaultScope.ToLowerInvariant()) '"Perfil"
        GridReports.Columns("ReportName").Caption = Me.Language.Translate("ReportName", Me.DefaultScope.ToLowerInvariant()) '"Informe"

        GridReports.DataSource = ReportSchedulersList()
        GridReports.DataBind()
    End Sub

    Protected Sub btnSetCompany_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSetCompany.Click

        If txtCompany.Value <> "" AndAlso (AuthValidations.GetCompanyWithSessionInitiated() = String.Empty OrElse AuthValidations.GetCompanyWithSessionInitiated() = txtCompany.Value.ToLower().Trim) Then
            Dim sourceCompany As String = roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId"))
            Try
                HttpContext.Current.Session("roMultiCompanyId") = txtCompany.Value
                GetReportList()
            Catch ex As Exception
                'do nothing
            End Try
            HttpContext.Current.Session("roMultiCompanyId") = sourceCompany
        End If

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then
            'comprobar si esta activo el uso de la pantalla

            trCompany.Style.Add("display", "")

            trWithKey.Style.Add("display", "none")
            trWithoutKey.Style.Add("display", "none")
            trKey.Style.Add("display", "none")
            trButtonLanzar.Style.Add("display", "none")
            trGrid.Style.Add("display", "none")
            trSetCompany.Style.Add("display", "")

        End If

        If AuthValidations.GetCompanyWithSessionInitiated() <> String.Empty Then
            txtCompany.Value = AuthValidations.GetCompanyWithSessionInitiated()
            txtCompany.Disabled = True
        End If

    End Sub

End Class