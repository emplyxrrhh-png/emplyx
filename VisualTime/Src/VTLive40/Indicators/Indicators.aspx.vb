Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Indicator
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Indicators_Indicators
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private Const FeatureAlias As String = "KPI.Definition"
    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("IndicatorsV2", "~/Indicators/Scripts/IndicatorsV2.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\KPIs") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesIndicators.TreeCaption = Me.Language.Translate("TreeCaptionIndicators", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        dateFormatValue.Value = HelperWeb.GetShortDateFormat

        Dim dTbl As Generic.List(Of roIndicator) = API.IndicatorsServiceMethods.GetIndicators(IndicatorsType.Attendance, Me.Page, "Name", False)
        If dTbl.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        If Not IsPostBack AndAlso Not IsCallback Then
            LoadDefaultData()
        End If

    End Sub

    Protected Sub LoadDefaultData()
        Dim conceptsList As DataTable = API.ConceptsServiceMethods.GetConcepts(Me.Page)
        Dim query = From c In conceptsList.AsEnumerable
                    Order By c.Field(Of String)("Name") Ascending
        Dim dtOrderer As DataTable = query.CopyToDataTable

        cmbPercentageFrom.Items.Clear()
        cmbPercentageTo.Items.Clear()
        cmbOperations.Items.Clear()
        cmbPercentageFrom.ValueType = GetType(Integer)
        cmbPercentageTo.ValueType = GetType(Integer)
        cmbOperations.ValueType = GetType(String)

        For Each dRow As DataRow In dtOrderer.Rows
            If Not dRow("ID") Is DBNull.Value Then
                cmbPercentageFrom.Items.Add(dRow("Name"), dRow("ID"))
                cmbPercentageTo.Items.Add(dRow("Name"), dRow("ID"))
            End If
        Next

        cmbOperations.Items.Add(Me.Language.Translate("Tendence.MajorEqual", DefaultScope), IndicatorCompareType.MajorEqual.ToString())
        cmbOperations.Items.Add(Me.Language.Translate("Tendence.MinorEqual", DefaultScope), IndicatorCompareType.MinorEqual.ToString())
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "GETINDICATOR"
                LoadIndicator(oParameters)
            Case "SAVEINDICATOR"
                SaveIndicator(oParameters)
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

        'Mostra el TAB seleccionat
        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadIndicator(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eIndicator As roIndicator = Nothing)
        Dim oCurrentIndicator As roIndicator = Nothing
        Dim result As String = "OK"
        Try

            If eIndicator IsNot Nothing Then
                oCurrentIndicator = eIndicator
            Else
                If oParameters.ID = "-1" Then
                    oCurrentIndicator = New roIndicator
                Else
                    oCurrentIndicator = API.IndicatorsServiceMethods.GetIndicator(Me, oParameters.ID, True)
                End If
            End If

            If oCurrentIndicator Is Nothing Then Exit Sub

            txtName.Text = oCurrentIndicator.Name
            txtDescription.Text = oCurrentIndicator.Description

            cmbPercentageFrom.SelectedItem = cmbPercentageFrom.Items.FindByValue(oCurrentIndicator.IDFirstConcept)
            cmbPercentageTo.SelectedItem = cmbPercentageTo.Items.FindByValue(oCurrentIndicator.IDSecondConcept)

            If String.IsNullOrEmpty(oCurrentIndicator.Condition) Then
                cmbOperations.SelectedItem = cmbOperations.Items.FindByValue(IndicatorCompareType.MajorEqual.ToString())
            Else
                cmbOperations.SelectedItem = cmbOperations.Items.FindByValue(oCurrentIndicator.Condition.ToString())
            End If

            If oCurrentIndicator.DesiredValue.HasValue Then
                txtDesiredValue.Value = oCurrentIndicator.DesiredValue
                'oJSONFields.Add(New JSONFieldItem("DesiredValue", oCurrentIndicator.DesiredValue.ToString.Replace(HelperWeb.GetDecimalDigitFormat, "."), New String() {txtDesiredValue.ClientID}, roJSON.JSONType.Number_JSON, Nothing, oDisable))
            Else
                txtDesiredValue.Value = 0
            End If

            If oCurrentIndicator.LimitValue.HasValue Then
                txtLimitValue.Value = oCurrentIndicator.LimitValue
                'oJSONFields.Add(New JSONFieldItem("LimitValue", oCurrentIndicator.LimitValue.ToString.Replace(HelperWeb.GetDecimalDigitFormat, "."), New String() {txtLimitValue.ClientID}, roJSON.JSONType.Number_JSON, Nothing, oDisable))
            Else
                txtLimitValue.Value = 0
            End If
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETINDICATOR")
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentIndicator.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Sub SaveIndicator(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentIndicator As roIndicator = Nothing

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If Me.oPermission = Permission.Write And bolIsNew Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            End If

            If rError.Error = False Then
                If bolIsNew Then
                    oCurrentIndicator = New roIndicator
                    oCurrentIndicator.ID = -1
                Else
                    oCurrentIndicator = API.IndicatorsServiceMethods.GetIndicator(Me, oParameters.ID, False)
                End If

                If oCurrentIndicator Is Nothing Then Exit Sub

                oCurrentIndicator.Name = txtName.Text

                oCurrentIndicator.Description = txtDescription.Text

                If cmbPercentageFrom.SelectedItem Is Nothing Then
                    oCurrentIndicator.IDFirstConcept = -1
                Else
                    oCurrentIndicator.IDFirstConcept = cmbPercentageFrom.SelectedItem.Value
                End If

                If cmbPercentageTo.SelectedItem Is Nothing Then
                    oCurrentIndicator.IDSecondConcept = -1
                Else
                    oCurrentIndicator.IDSecondConcept = cmbPercentageTo.SelectedItem.Value
                End If

                oCurrentIndicator.Condition = cmbOperations.SelectedItem.Value

                oCurrentIndicator.DesiredValue = CDbl(roTypes.Any2String(txtDesiredValue.Value).Replace(".", HelperWeb.GetDecimalDigitFormat))

                oCurrentIndicator.LimitValue = CDbl(roTypes.Any2String(txtLimitValue.Value).Replace(".", HelperWeb.GetDecimalDigitFormat))

                If API.IndicatorsServiceMethods.SaveIndicator(IndicatorsType.Attendance, Me, oCurrentIndicator, True) = True Then
                    Dim treePath As String = "/source/" & oCurrentIndicator.ID
                    HelperWeb.roSelector_SetSelection(oCurrentIndicator.ID.ToString, treePath, "ctl00_contentMainBody_roTreesIndicators")
                    rError = New roJSON.JSONError(False, "OK:" & oCurrentIndicator.ID)
                Else
                    rError = New roJSON.JSONError(True, API.IndicatorsServiceMethods.LastErrorText)
                End If

            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            If rError.Error = False Then
                LoadIndicator(oParameters, oCurrentIndicator)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
            Else
                LoadIndicator(oParameters, oCurrentIndicator)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEINDICATOR"
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

End Class