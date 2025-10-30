Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class OptionsBusinessCenterField
    Inherits PageBase

    Private Const FeatureAlias As String = "BusinessCenters.Definition"

#Region "Declarations"

    Private oPermission As Permission = Permission.None

#End Region

#Region "Properties"

    Private Property BusinessCenterFieldData() As roBusinessCenterFieldDefinition
        Get
            Dim oData As roBusinessCenterFieldDefinition = Session("BusinessCenterField_BusinessCenterFieldData")
            If oData Is Nothing Then

                Dim intTaskID As String = Request.Params("BusinessCenterFieldID")
                If intTaskID Is Nothing Then intTaskID = 1

                oData = New roBusinessCenterFieldDefinition
                If intTaskID <> 0 Then
                    Dim oRows() As DataRow = Me.BusinessCenterFieldsData.Select("ID = " & intTaskID)
                    If oRows.Length = 1 Then
                        With oData
                            .ID = oRows(0).Item("ID")
                            .Name = oRows(0).Item("Name")

                        End With
                    End If
                End If

                Session("BusinessCenterField_BusinessCenterFieldData") = oData

            End If
            Return oData
        End Get
        Set(ByVal value As roBusinessCenterFieldDefinition)
            Session("BusinessCenterField_BusinessCenterFieldData") = value
        End Set
    End Property

    Private ReadOnly Property BusinessCenterFieldDataChanged() As Boolean
        Get
            Dim oData As roBusinessCenterFieldDefinition = Session("BusinessCenterField_BusinessCenterFieldData")
            Return (oData Is Nothing)
        End Get
    End Property

    Private Property BusinessCenterFieldsData() As DataTable
        Get
            Dim tb As DataTable = Nothing
            tb = Session("ConfigurationOptions_BusinessCenterFieldsData")
            Return tb
        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_BusinessCenterFieldsData") = value

        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("OpenWindow", "~/Base/Scripts/OpenWindow.js", , True)
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", , True)

        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("roBusinessCenterFieldCriteria", "~/Base/Scripts/roUserFieldCriteria.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Base/ext-3.4.0/resources/css/ext-all.css")

        ' Si el passport actual no tiene permisso de lectura, redirigimos a página de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then

            Me.BusinessCenterFieldData = Nothing

        End If

        Me.UpdateData()

        If Not Me.IsPostBack Then
            Me.SetPermissions()
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.oPermission >= Permission.Write Then

            If Me.ValidateData() Then
                Dim strListValues As String = ""

                With Me.BusinessCenterFieldData
                    .Name = Me.txtName.Value

                End With

                Dim bolSaved As Boolean = False

                Dim tbBusinessCenterFields As DataTable = Me.BusinessCenterFieldsData
                If tbBusinessCenterFields IsNot Nothing Then
                    Dim oRows() As DataRow = tbBusinessCenterFields.Select("ID = " & Me.BusinessCenterFieldData.ID)
                    Dim oUserFieldRow As DataRow
                    oUserFieldRow = oRows(0)
                    With Me.BusinessCenterFieldData
                        oUserFieldRow("ID") = .ID
                        oUserFieldRow("Name") = .Name

                    End With
                    Me.BusinessCenterFieldsData = tbBusinessCenterFields
                    bolSaved = True
                End If

                Me.CanClose = bolSaved
                Me.MustRefresh = IIf(bolSaved, "1", "0")
                If bolSaved Then
                    ' Para forzar que se recargen los datos en la pantalla opciones de presencia
                    'Session("AttendanceOptions_BusinessCenterFieldsData") = Nothing
                End If

            End If

        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Select Case strButtonKey
            Case "DeleteCategory.Answer.Yes"

                'API.ReportServiceMethods.DeleteProfile(Me, Me.cmbProfiles_Value.Value)
                'Me.LoadCategories()

            Case "DeleteListValue.Answer.Yes"

        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub UpdateData()

        Dim intBusinessCenterFieldID As String = roTypes.Any2String(Request.Params("BusinessCenterFieldID"))
        Dim fl As roBusinessCenterFieldDefinition = API.UserFieldServiceMethods.GetBusinessCenterField(Me, intBusinessCenterFieldID, False)
        If Me.BusinessCenterFieldDataChanged Then
            With Me.BusinessCenterFieldData
                Me.txtName.Value = .Name

            End With
        End If

    End Sub

    Private Function ValidateData() As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        If Not Robotics.DataLayer.roSupport.IsXSSSafe(Me.txtName.Value) Then
            strMsg = Me.Language.Translate("Validate.XSSError", Me.DefaultScope)
            Me.txtName.Focus()
        Else
            If Me.txtName.Value.Trim = "" Then
                strMsg = Me.Language.Translate("Validate.InvalidName", Me.DefaultScope)
                Me.txtName.Focus()
            Else
                Dim oRows(-1) As DataRow
                oRows = Me.BusinessCenterFieldsData.Select("Name='" & Replace(Me.txtName.Value, "'", "''") & "' AND ID <> " & Me.BusinessCenterFieldData.ID)
                If oRows.Length > 0 Then
                    strMsg = Me.Language.Translate("Validate.FieldExist", Me.DefaultScope)
                    Me.txtName.Focus()
                End If
            End If
        End If


        lblError.Text = ""
        lblError.Visible = False

        If strMsg <> "" Then
            lblError.Text = strMsg
            lblError.Visible = True
            Me.updError.Update()
            bolRet = False
        End If

        Return bolRet

    End Function

    Private Sub SetPermissions()

        If Me.oPermission < Permission.Write Then

            Me.DisableControls()

            Me.btAccept.Style("display") = "none"
            Me.btCancel.Text = Me.Language.Keyword("Button.Close")

        End If

    End Sub

#End Region

End Class