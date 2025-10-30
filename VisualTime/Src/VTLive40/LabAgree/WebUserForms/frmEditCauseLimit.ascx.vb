Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmEditCauseLimit
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="idtype")>
        Public idtype As String

    End Class

#Region "Helper private methods"

    Private Property LabAgreedCausesLimitData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roLabAgreeCauseLimitValues)
        Get

            Dim tbCauses As Generic.List(Of roLabAgreeCauseLimitValues) = Session("LabAgree_LabAgreedCasuesLimitData")

            If bolReload Or tbCauses Is Nothing Then
                Dim oList As New Generic.List(Of roLabAgreeCauseLimitValues)
                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me.Page, Session("LabAgree_SelectedID"), True)
                    For Each oLabObject As roLabAgreeCauseLimitValues In oCurrentLabAgree.LabAgreeCauseLimitValues

                        If oLabObject.EndDate = New Date(2079, 1, 1) Then
                            oLabObject.EndDate = Nothing
                        End If

                        oList.Add(oLabObject)
                    Next

                End If
                tbCauses = oList
                Session("LabAgree_LabAgreedCasuesLimitData") = oList

            End If
            Return tbCauses

        End Get
        Set(ByVal value As Generic.List(Of roLabAgreeCauseLimitValues))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedCasuesLimitData") = value
            Else
                Session("LabAgree_LabAgreedCasuesLimitData") = Nothing
            End If
        End Set
    End Property

    Private Function getMinStartupID() As Integer
        Dim newId As Integer = 0

        Dim oList As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            Dim index As Integer = 0
            For Each oElem As roLabAgreeCauseLimitValues In oList
                If index = 0 Then
                    newId = oElem.IDCauseLimitValue
                Else
                    If oElem.IDCauseLimitValue < newId Then
                        newId = oElem.IDCauseLimitValue
                    End If
                End If
                index = index + 1
            Next
        End If

        If newId >= 0 Then
            Return -2
        Else
            Return newId - 1
        End If
    End Function

    Private Function ExistsCauseInList(ByVal oId As Integer, ByVal oIdCause As Integer, ByVal beginDate As Date, ByVal endDate As Date) As Boolean
        Dim bExists As Boolean = False

        Dim calcEndDate As Date = New Date(2079, 1, 1, 0, 0, 0)
        If endDate > Date.MinValue Then calcEndDate = endDate

        Dim oList As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeCauseLimitValues In oList
                Dim assignedEndDate As Date = New Date(2079, 1, 1, 0, 0, 0)
                If oElem.EndDate > Date.MinValue Then assignedEndDate = oElem.EndDate

                If oElem.IDCauseLimitValue <> oId AndAlso oElem.CauseLimitValue.IDCause = oIdCause Then
                    If (oElem.BeginDate <= beginDate AndAlso assignedEndDate > beginDate) Or
                        (oElem.BeginDate <= calcEndDate AndAlso assignedEndDate >= calcEndDate) Or
                        (oElem.BeginDate > beginDate AndAlso assignedEndDate < calcEndDate) Then
                        bExists = True
                        Exit For
                    End If
                End If
            Next
        End If

        Return bExists
    End Function

    Private Function getCauseLimit(ByVal oId As Integer) As roLabAgreeCauseLimitValues
        Dim oObject As roLabAgreeCauseLimitValues = Nothing

        Dim oList As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeCauseLimitValues In oList
                If oElem.IDCauseLimitValue = oId Then
                    oObject = roSupport.DeepClone(Of roLabAgreeCauseLimitValues)(oElem)

                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function setCauseLimit(ByVal oObject As roLabAgreeCauseLimitValues, ByVal bIsNew As Boolean) As Boolean
        Dim bResult As Boolean = False

        Dim oList As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData()

        If Not bIsNew Then
            Dim index As Integer = 0
            If oList IsNot Nothing AndAlso oList.Count > 0 Then
                For Each oElem As roLabAgreeCauseLimitValues In oList
                    If oElem.IDCauseLimitValue = oObject.IDCauseLimitValue Then
                        oList(index) = oObject
                        bResult = True
                        LabAgreedCausesLimitData = oList
                        Exit For
                    End If
                    index = index + 1
                Next
            End If
        Else
            If oList Is Nothing Then oList = New Generic.List(Of roLabAgreeCauseLimitValues)
            oList.Add(oObject)
            LabAgreedCausesLimitData = oList
        End If

        Return bResult
    End Function

#End Region

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadCombos()

            Me.hdnDif0.Value = Me.Language.Keyword("Dif.Value")
            Me.hdnDif1.Value = Me.Language.Keyword("Dif.Value")
            Me.hdnDif2.Value = Me.Language.Keyword("Dif.Value")
        End If
    End Sub

    Private Sub LoadCombos()
        'Si tenim valor inicial, busquem concept i carreguem els combos
        cmbAnnualValue.Items.Clear()
        cmbMonthlyValue.Items.Clear()
        Dim dtblUF As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3,4) AND Used = 1", False)
        For Each dRowUF As DataRow In dtblUF.Rows
            cmbAnnualValue.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmbMonthlyValue.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
        Next

        cmbIDCause.Items.Clear()
        cmbCauseExcess.Items.Clear()
        cmbIDCause.ValueType = GetType(Integer)
        cmbCauseExcess.ValueType = GetType(Integer)
        Dim dtCauses As DataTable = CausesServiceMethods.GetCauses(Me.Page)
        For Each dtCausesRow As DataRow In dtCauses.Rows
            cmbIDCause.Items.Add(dtCausesRow("Name"), dtCausesRow("ID"))
            cmbCauseExcess.Items.Add(dtCausesRow("Name"), dtCausesRow("ID"))
        Next

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCauseLimitCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "GETCAUSELIMITVALUE"
                LoadLabAgreeCauseLimit(oParameters)
            Case "SAVECAUSELIMITVALUE"
                SaveLabCauseLimit(oParameters)
            Case Else
                ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select
    End Sub

    Private Sub LoadLabAgreeCauseLimit(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eCauseLimitValue As roLabAgreeCauseLimitValues = Nothing)
        Dim oCurrentLabAgreeCauseLimit As roLabAgreeCauseLimitValues = Nothing
        Dim oCurrentCauseLimit As New roCauseLimitValue
        Dim result As String = "OK"
        Try

            If eCauseLimitValue IsNot Nothing Then
                oCurrentLabAgreeCauseLimit = eCauseLimitValue
            Else
                If oParameters.ID = "-1" Then
                    oCurrentLabAgreeCauseLimit = New roLabAgreeCauseLimitValues
                    oCurrentLabAgreeCauseLimit.IDCauseLimitValue = getMinStartupID()

                    oCurrentLabAgreeCauseLimit.IDLabAgree = oParameters.IDLabAgree
                    oCurrentLabAgreeCauseLimit.CauseLimitValue = oCurrentCauseLimit
                    oCurrentCauseLimit.Name = String.Empty
                    oCurrentCauseLimit.IDCause = 0
                    oCurrentCauseLimit.IDCauseLimitValue = 0
                Else
                    oCurrentLabAgreeCauseLimit = getCauseLimit(oParameters.ID)
                    oCurrentCauseLimit = oCurrentLabAgreeCauseLimit.CauseLimitValue
                End If
            End If

            If oCurrentLabAgreeCauseLimit Is Nothing Then Exit Sub

            Me.txtName.Text = oCurrentCauseLimit.Name
            If oCurrentLabAgreeCauseLimit.BeginDate = DateTime.MinValue Then
                txtInitialDate.Date = Date.Now.Date
            Else
                txtInitialDate.Date = oCurrentLabAgreeCauseLimit.BeginDate
            End If

            If oCurrentLabAgreeCauseLimit.EndDate = DateTime.MinValue Or oCurrentLabAgreeCauseLimit.EndDate = New DateTime(2079, 1, 1, 0, 0, 0) Then
                txtEndDate.Value = Nothing
            Else
                txtEndDate.Date = oCurrentLabAgreeCauseLimit.EndDate
            End If

            cmbIDCause.SelectedItem = cmbIDCause.Items.FindByValue(oCurrentCauseLimit.IDCause)
            cmbCauseExcess.SelectedItem = cmbCauseExcess.Items.FindByValue(oCurrentCauseLimit.IDExcessCause)

            If oCurrentLabAgreeCauseLimit.IDCauseLimitValue > 0 Then
                cmbIDCause.Enabled = False
            End If

            cmbAnnualValue.SelectedItem = Nothing
            cmbMonthlyValue.SelectedItem = Nothing
            Me.txtAnnualValue.Text = String.Empty
            Me.txtMonthlyValue.Text = String.Empty

            Dim txtMaxVF As String = ""
            Dim txtMinVF As String = ""
            If oCurrentCauseLimit.MaximumAnnualField IsNot Nothing Then txtMaxVF = oCurrentCauseLimit.MaximumAnnualField.FieldName
            If oCurrentCauseLimit.MaximumMonthlyField IsNot Nothing Then txtMinVF = oCurrentCauseLimit.MaximumMonthlyField.FieldName

            If oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.None Then
                Me.optAnnual.Checked = False
            ElseIf oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.DirectValue Then
                Me.optAnnual.Checked = True
                Me.rbAnnualFix.Checked = True
                Me.rbAnnualUF.Checked = False
                Me.txtAnnualValue.Text = oCurrentCauseLimit.MaximumAnnualValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
            ElseIf oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.UserField Then
                Me.optAnnual.Checked = True
                Me.rbAnnualFix.Checked = False
                Me.rbAnnualUF.Checked = True
                cmbAnnualValue.SelectedItem = cmbAnnualValue.Items.FindByValue(txtMaxVF)
            End If

            If oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.None Then
                Me.optMonthly.Checked = False
            ElseIf oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.DirectValue Then
                Me.optMonthly.Checked = True
                Me.rbMonthlyFix.Checked = True
                Me.rbMonthlyUF.Checked = False
                Me.txtMonthlyValue.Text = oCurrentCauseLimit.MaximumMonthlyValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
            ElseIf oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.UserField Then
                Me.optMonthly.Checked = True
                Me.rbMonthlyFix.Checked = False
                Me.rbMonthlyUF.Checked = True
                cmbMonthlyValue.SelectedItem = cmbMonthlyValue.Items.FindByValue(txtMinVF)
            End If
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETCAUSELIMITVALUE")
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentCauseLimit.Name)
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Sub SaveLabCauseLimit(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")

        Dim oCurrentLabAgreeCauseLimit As roLabAgreeCauseLimitValues = Nothing
        Dim oCurrentCauseLimit As New roCauseLimitValue
        Dim bInclompleteData As Boolean = False

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then
                oCurrentLabAgreeCauseLimit = New roLabAgreeCauseLimitValues
                oCurrentLabAgreeCauseLimit.IDCauseLimitValue = getMinStartupID()

                oCurrentLabAgreeCauseLimit.IDLabAgree = oParameters.IDLabAgree
                oCurrentLabAgreeCauseLimit.CauseLimitValue = oCurrentCauseLimit
                oCurrentCauseLimit.Name = String.Empty
                oCurrentCauseLimit.IDCause = 0
                oCurrentCauseLimit.IDCauseLimitValue = 0
            Else
                oCurrentLabAgreeCauseLimit = getCauseLimit(oParameters.ID)
                oCurrentCauseLimit = oCurrentLabAgreeCauseLimit.CauseLimitValue
            End If

            If oCurrentLabAgreeCauseLimit Is Nothing Then Exit Sub

            If oParameters.ID < 0 Then
                oCurrentCauseLimit.OriginalIDCause = 0
            Else
                oCurrentCauseLimit.OriginalIDCause = oParameters.ID
            End If

            oCurrentCauseLimit.Name = txtName.Text

            oCurrentLabAgreeCauseLimit.BeginDate = txtInitialDate.Date.Date

            If txtEndDate.Value Is Nothing Then
                oCurrentLabAgreeCauseLimit.EndDate = Nothing
            Else
                oCurrentLabAgreeCauseLimit.EndDate = txtEndDate.Date.Date
            End If

            oCurrentCauseLimit.IDCause = cmbIDCause.SelectedItem.Value
            oCurrentCauseLimit.IDExcessCause = cmbCauseExcess.SelectedItem.Value

            If Not bInclompleteData Then
                If Me.optAnnual.Checked Then
                    If rbAnnualUF.Checked Then
                        oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.UserField
                        If cmbAnnualValue.SelectedItem IsNot Nothing Then
                            Dim oMaxUF As New roUserField
                            oMaxUF.FieldName = cmbAnnualValue.SelectedItem.Value
                            oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                            oCurrentCauseLimit.MaximumAnnualField = oMaxUF
                        Else
                            oCurrentCauseLimit.MaximumAnnualField = Nothing
                            bInclompleteData = True
                        End If
                    ElseIf rbAnnualFix.Checked Then
                        If txtAnnualValue.Value IsNot Nothing And txtAnnualValue.Text <> String.Empty Then
                            oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.DirectValue
                            oCurrentCauseLimit.MaximumAnnualValue = roTypes.Any2Double(txtAnnualValue.Text)
                        Else
                            bInclompleteData = True
                        End If
                    Else
                        bInclompleteData = True
                    End If
                Else
                    oCurrentCauseLimit.MaximumAnnualValueType = LabAgreeValueType.None
                    oCurrentCauseLimit.MaximumAnnualValue = Nothing
                End If
            End If

            If Not bInclompleteData Then
                If Me.optMonthly.Checked Then
                    If rbMonthlyUF.Checked Then
                        oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.UserField
                        If cmbMonthlyValue.SelectedItem IsNot Nothing Then
                            Dim oMaxUF As New roUserField()
                            oMaxUF.FieldName = cmbMonthlyValue.SelectedItem.Value
                            oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                            oCurrentCauseLimit.MaximumMonthlyField = oMaxUF
                        Else
                            bInclompleteData = True
                            oCurrentCauseLimit.MaximumMonthlyField = Nothing
                        End If
                    ElseIf rbMonthlyFix.Checked Then
                        If txtMonthlyValue.Value IsNot Nothing And txtMonthlyValue.Text <> String.Empty Then
                            oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.DirectValue
                            oCurrentCauseLimit.MaximumMonthlyValue = roTypes.Any2Double(txtMonthlyValue.Text)
                        Else
                            bInclompleteData = True
                        End If
                    Else
                        bInclompleteData = True
                    End If
                Else
                    oCurrentCauseLimit.MaximumMonthlyType = LabAgreeValueType.None
                    oCurrentCauseLimit.MaximumMonthlyValue = Nothing
                End If
            End If

            If cmbCauseExcess.SelectedItem.Value = cmbIDCause.SelectedItem.Value Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.MustBeDifferent", Me.DefaultScope))
            End If

            If Not Me.optAnnual.Checked AndAlso Not Me.optMonthly.Checked Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.NoSelection", Me.DefaultScope))
            End If

            'modificar el startup value de la variable de sesion
            If bInclompleteData Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.FieldsEmpty", Me.DefaultScope))
            End If

            If ExistsCauseInList(oCurrentLabAgreeCauseLimit.IDCauseLimitValue, oCurrentCauseLimit.IDCause, oCurrentLabAgreeCauseLimit.BeginDate, oCurrentLabAgreeCauseLimit.EndDate) Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.AlreadyExists", Me.DefaultScope))
            End If

            If rError.Error = False Then
                setCauseLimit(oCurrentLabAgreeCauseLimit, bolIsNew)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVECAUSELIMITVALUE")
            ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentCauseLimit.Name)

            If rError.Error = False Then
                ASPxCauseLimitCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                ASPxCauseLimitCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCauseLimitCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

End Class