Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class License
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.Security"
    Private maxConcurrent As Int32 = 50

    Private Property SolutionsData(Optional ByVal bolReload As Boolean = False) As roLicenseSolution()
        Get
            Dim solutions As Object = Session("Licenses_SolutionsData")

            If bolReload OrElse solutions Is Nothing Then
                Dim licInfoFile As String = IO.Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Resources/licenseInfo.xml")
                solutions = API.LicenseServiceMethods.GetLicenseInstalledSolutionStatus(licInfoFile)

                Session("Licenses_SolutionsData") = solutions
            End If
            Return solutions
        End Get
        Set(value As roLicenseSolution())
            Session("Licenses_SolutionsData") = value
        End Set
    End Property

    Private Property ModulesData(Optional ByVal bolReload As Boolean = False) As roLicenseModule()
        Get
            Dim modules As Object = Session("Licenses_ModulesData")

            If bolReload OrElse modules Is Nothing Then
                Dim licInfoFile As String = IO.Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Resources/licenseInfo.xml")
                modules = API.LicenseServiceMethods.GetLicenseInstalledModulesStatus(licInfoFile)

                Session("Licenses_ModulesData") = modules
            End If
            Return modules
        End Get
        Set(value As roLicenseModule())
            Session("Licenses_ModulesData") = value
        End Set
    End Property

    Public Property BusinessTools As Object

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Options", "~/Options/Scripts/Options.js")
        Me.InsertExtraJavascript("frmAddRoute", "~/Options/Scripts/frmAddRoute.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("License", "~/Security/Scripts/License.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Si el passport actual no tiene permisso de lectura, rediriguimos a pàgina de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Dim oLicense As New Robotics.VTBase.Extensions.roServerLicense
        Dim intMaxConcurrentSessions As Integer
        Dim licInfoFile As String = IO.Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Resources/licenseInfo.xml")
        Dim objRet = API.LicenseServiceMethods.GetLicenseMaxConcurrentSessions(licInfoFile)
        If Not IsNothing(objRet) Then

            If Not Integer.TryParse(objRet.ToString, intMaxConcurrentSessions) Then
                intMaxConcurrentSessions = 0
            End If
            maxConcurrent = intMaxConcurrentSessions
            Me.maxConcurrentUsers.Value = intMaxConcurrentSessions
        End If

        If Not Me.IsPostBack Then
            CreateColumnsGrids()
            BindGrids(True)
            Me.SetPermissions()
        Else
            BindGrids(False)
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub CreateColumnsGrids()
        CreateColumnsSolutionGrid()
        CreateColumnsModulesGrid()
    End Sub

    Private Sub BindGrids(ByVal bolReload As Boolean)
        Me.GridSolutions.DataSource = Me.SolutionsData(bolReload)
        Me.GridSolutions.DataBind()

        Me.GridModules.DataSource = Me.ModulesData(bolReload)
        Me.GridModules.DataBind()
    End Sub

    Private Sub CreateColumnsSolutionGrid()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnImage As GridViewDataImageColumn

        Dim VisibleIndex As Integer = 0

        Me.GridSolutions.Columns.Clear()
        Me.GridSolutions.KeyFieldName = "LanguageTag"
        Me.GridSolutions.SettingsText.EmptyDataRow = " "
        Me.GridSolutions.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridSolutions.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "LanguageTag"
        GridColumn.FieldName = "LanguageTag"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSolutions.Columns.Add(GridColumn)

        'IsAvailable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IsAvailable"
        GridColumn.FieldName = "IsAvailable"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSolutions.Columns.Add(GridColumn)

        'IsCorrect
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IsCorrect"
        GridColumn.FieldName = "IsCorrect"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSolutions.Columns.Add(GridColumn)

        'Nombre
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Solution", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.GridSolutions.Columns.Add(GridColumn)

        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("GridSolutions.Column.Status", DefaultScope)
        GridColumnImage.FieldName = "Status"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 16
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 32
        GridColumnImage.PropertiesImage.ImageWidth = 32
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "{0}"
        Me.GridSolutions.Columns.Add(GridColumnImage)

        'Object Name
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.MissingModules", DefaultScope)
        GridColumn.FieldName = "MissingModules"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.Width = 30
        Me.GridSolutions.Columns.Add(GridColumn)

        'Object Name
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.LicenseEmployees", DefaultScope)
        GridColumn.FieldName = "LicenseEmployees"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.Width = 30
        Me.GridSolutions.Columns.Add(GridColumn)

        'Object Name
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.AvailableLicenseEmployees", DefaultScope)
        GridColumn.FieldName = "AvailableLicenseEmployees"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.Width = 30
        Me.GridSolutions.Columns.Add(GridColumn)
    End Sub

    Private Sub CreateColumnsModulesGrid()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnImage As GridViewDataImageColumn

        Dim VisibleIndex As Integer = 0

        Me.GridModules.Columns.Clear()
        Me.GridModules.KeyFieldName = "LanguageTag"
        Me.GridModules.SettingsText.EmptyDataRow = " "
        Me.GridModules.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridModules.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "LanguageTag"
        GridColumn.FieldName = "LanguageTag"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridModules.Columns.Add(GridColumn)

        'IsAvailable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IsAvailable"
        GridColumn.FieldName = "IsAvailable"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSolutions.Columns.Add(GridColumn)

        'IsCorrect
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IsCorrect"
        GridColumn.FieldName = "IsCorrect"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSolutions.Columns.Add(GridColumn)

        'Nombre
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Solution", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.GridModules.Columns.Add(GridColumn)

        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("GridSolutions.Column.Status", DefaultScope)
        GridColumnImage.FieldName = "Status"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 16
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 32
        GridColumnImage.PropertiesImage.ImageWidth = 32
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "{0}"
        Me.GridModules.Columns.Add(GridColumnImage)

        'Object Name
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.MissingModules", DefaultScope)
        GridColumn.FieldName = "MissingModules"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.Width = 30
        Me.GridModules.Columns.Add(GridColumn)
    End Sub

    Protected Sub GridModules_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridModules.CustomUnboundColumnData, GridSolutions.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "Status"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IsAvailable") IsNot System.DBNull.Value AndAlso e.GetListSourceFieldValue("IsCorrect") IsNot System.DBNull.Value Then
                        Dim bIsCorrect As Boolean = roTypes.Any2Boolean(e.GetListSourceFieldValue("IsCorrect"))
                        Dim bIsAvailable As Boolean = roTypes.Any2Boolean(e.GetListSourceFieldValue("IsAvailable"))
                        e.Value = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")

                        If bIsCorrect AndAlso bIsAvailable Then
                            e.Value = "Images/ico_ok.png"
                        ElseIf Not bIsCorrect AndAlso bIsAvailable Then
                            e.Value = "Images/ico_ko.png"
                        Else
                            e.Value = "Images/ico_ok_dis.png"
                        End If

                    End If
                End If
            Case "Name"
                If Not IsDBNull(e.GetListSourceFieldValue("LanguageTag")) Then
                    e.Value = Me.Language.Translate("LicenceType." & e.GetListSourceFieldValue("LanguageTag"), Me.DefaultScope)
                End If

        End Select

    End Sub

    Private Sub SetPermissions()
        If Me.oPermission = Permission.None Then
            Me.TABBUTTON_LicenceInfo.Style("display") = "none"
            Me.tbLicenceInfo.Visible = False
            Me.tbConcurrencyInfo.Visible = False
        ElseIf Me.oPermission < Permission.Write Then
            Me.DisableControls(Me.tbLicenceInfo.Controls)
            Me.DisableControls(Me.tbConcurrencyInfo.Controls)
        End If
    End Sub

    Private Sub CallbackSession_Callback(source As Object, e As CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "LOADGRAPHDATA"
                Dim concurrencyInfo = SecurityV2ServiceMethods.GetConcurrencyInfo(Me.Page, False)

                If concurrencyInfo IsNot Nothing Then

                    For Each concurrency As Robotics.Base.DTOs.ConcurrencyInfo In concurrencyInfo.ConcurrencyInfoValues

                        ' concurrency.Datetime = FormatDateTime(concurrency.Datetime, DateFormat.ShortDate)

                        If concurrency.Max = 1 Then
                            concurrency.Max = maxConcurrent + 1
                        End If
                    Next

                    CallbackSession.JSProperties.Add("cpGraphInfo", roJSONHelper.SerializeNewtonSoft(concurrencyInfo.ConcurrencyInfoValues))
                End If
        End Select
    End Sub

#End Region

End Class