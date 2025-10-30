Imports System.Security.Cryptography
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserForms_frmEditStartupValue
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

        <Runtime.Serialization.DataMember(Name:="acumValues")>
        Public AcumValues As roScalingValues()

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="idtype")>
        Public idtype As String

    End Class

#Region "Helper private methods"

    Private Property LabAgreeStartUpValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roStartupValue)
        Get

            Dim tbValues As Generic.List(Of roStartupValue) = Session("LabAgree_LabAgreedStartupValues")

            If bolReload OrElse tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roStartupValue)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me.Page, Session("LabAgree_SelectedID"), True)
                    oList.AddRange(oCurrentLabAgree.StartupValues())
                End If

                tbValues = oList
                Session("LabAgree_LabAgreedStartupValues") = tbValues

            End If
            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roStartupValue))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedStartupValues") = value
            Else
                Session("LabAgree_LabAgreedStartupValues") = Nothing
            End If
        End Set
    End Property

    Private Function getMinStartupID() As Integer
        Dim newId As Integer = 0

        Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            Dim index As Integer = 0
            For Each oElem As roStartupValue In oList
                If index = 0 Then
                    newId = oElem.ID
                Else
                    If oElem.ID < newId Then
                        newId = oElem.ID
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

    Private Function ExistsConceptInList(ByVal oId As Integer, ByVal oIdConcept As Integer) As Boolean
        Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roStartupValue In oList
                If oElem.ID <> oId AndAlso oElem.IDConcept = oIdConcept Then
                    Return True
                End If
            Next
        End If

        Return False
    End Function

    Private Function getStartupValue(ByVal oId As Integer) As roStartupValue
        Dim oObject As roStartupValue = Nothing

        Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roStartupValue In oList
                If oElem.ID = oId Then
                    oObject = roSupport.DeepClone(Of roStartupValue)(oElem)

                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function setStartupValue(ByVal oObject As roStartupValue, ByVal bIsNew As Boolean) As Boolean
        Dim bResult As Boolean = False

        Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

        If Not bIsNew Then
            Dim index As Integer = 0
            If oList IsNot Nothing AndAlso oList.Count > 0 Then
                For Each oElem As roStartupValue In oList
                    If oElem.ID = oObject.ID Then
                        oList(index) = oObject
                        bResult = True
                        LabAgreeStartUpValuesData = oList
                        Exit For
                    End If
                    index = index + 1
                Next
            End If
        Else
            If oList Is Nothing Then oList = New Generic.List(Of roStartupValue)
            oList.Add(oObject)
            LabAgreeStartUpValuesData = oList
        End If

        Return bResult
    End Function

#End Region

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadDefaultCombos()

            Me.hdnDif0.Value = Me.Language.Keyword("Dif.Value")
            Me.hdnDif1.Value = Me.Language.Keyword("Dif.Value")
            Me.hdnDif2.Value = Me.Language.Keyword("Dif.Value")
        End If
    End Sub

    Private Sub LoadDefaultCombos()
        Dim dTblConcepts As DataTable = API.ConceptsServiceMethods.GetConcepts(Me.Page)
        Me.cmbIDConcept.Items.Clear()

        For Each dRowConcept As DataRow In dTblConcepts.Rows
            cmbIDConcept.Items.Add(dRowConcept("Name"), dRowConcept("ID") & "_" & dRowConcept("IDType") & "_" & dRowConcept("DefaultQuery")) ', "reloadCombos('" & dRowConcept("IDType") & "');")
        Next

        cmbRoundType.Items.Clear()
        cmbRoundType.ValueType = GetType(Integer)
        cmbRoundType.Items.Add(Me.Language.Translate("RoundType.None", Me.DefaultScope), 0) ', "reloadCombos('" & dRowConcept("IDType") & "');")
        cmbRoundType.Items.Add(Me.Language.Translate("RoundType.Up", Me.DefaultScope), 1) ', "reloadCombos('" & dRowConcept("IDType") & "');")
        cmbRoundType.Items.Add(Me.Language.Translate("RoundType.Down", Me.DefaultScope), 2) ', "reloadCombos('" & dRowConcept("IDType") & "');")
        cmbRoundType.Items.Add(Me.Language.Translate("RoundType.Aprox", Me.DefaultScope), 3) ', "reloadCombos('" & dRowConcept("IDType") & "');")

        'Si tenim valor inicial, busquem concept i carreguem els combos
        cmbStartValue.Items.Clear()
        cmbMaximumValue.Items.Clear()
        cmbMinimumValue.Items.Clear()
        cmbAntUserField.Items.Clear()
        cmbCoeField.Items.Clear()
        cmbEndCustomPeriod.Items.Clear()

        cmbAutomaticAccrualUF.Items.Clear()
        cmbBaseHoursUF.Items.Clear()
        cmdTotalHoursUF.Items.Clear()

        ckEndCustomPeriod.Checked = False
        ckCoe.Checked = False

        Dim dtblUF As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "Used = 1", False)
        For Each dRowUF As DataRow In dtblUF.Rows

            cmbMaximumValue.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmbMinimumValue.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmbBaseHoursUF.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmdTotalHoursUF.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmbAutomaticAccrualUF.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
            cmbAntUserField.Items.Add(dRowUF("FieldName"), dRowUF("FieldName"))
        Next

        Dim dtblCOE As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType in (1,3,4) and Used = 1", False)
        For Each dRowCOE As DataRow In dtblCOE.Rows
            cmbCoeField.Items.Add(dRowCOE("FieldName"), dRowCOE("FieldName"))
            cmbStartValue.Items.Add(dRowCOE("FieldName"), dRowCOE("FieldName"))
        Next

        Dim dtblEndPeriod As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType = 2 and Used = 1", False)
        For Each dRowEP As DataRow In dtblEndPeriod.Rows
            cmbEndCustomPeriod.Items.Add(dRowEP("FieldName"), dRowEP("FieldName"))
        Next

        cmbExpirationPeriodType.Items.Clear()
        cmbExpirationPeriodType.ValueType = GetType(Integer)
        cmbExpirationPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Day", DefaultScope), 0))
        cmbExpirationPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Week", DefaultScope), 1))

        cmbEnjoymentPeriodType.Items.Clear()
        cmbEnjoymentPeriodType.ValueType = GetType(Integer)
        cmbEnjoymentPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Day", DefaultScope), 0))
        cmbEnjoymentPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Week", DefaultScope), 1))
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxStartupValueCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Action
            Case "GETLABAGREESTARTUP"
                LoadLabAgreeStartup(oParameters)
            Case "SAVELABAGREESTARTUP"
                SaveLabAgreeStartup(oParameters)
            Case Else
                ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select
    End Sub

    Private Sub LoadLabAgreeStartup(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eStartupValue As roStartupValue = Nothing)
        Dim oCurrentStartupValue As roStartupValue = Nothing
        Dim result As String = "OK"
        Try

            If eStartupValue IsNot Nothing Then
                oCurrentStartupValue = eStartupValue
            Else
                If oParameters.ID = "-1" Then
                    oCurrentStartupValue = New roStartupValue
                    oCurrentStartupValue.ID = getMinStartupID()
                Else
                    oCurrentStartupValue = getStartupValue(oParameters.ID)
                End If
            End If

            If oCurrentStartupValue Is Nothing Then Return

            Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentStartupValue.IDConcept, False)

            txtName.Text = oCurrentStartupValue.Name

            If oConcept IsNot Nothing Then hdnIDType.Value = oConcept.IDType
            If oConcept IsNot Nothing Then hdnDefaultQuery.Value = oConcept.DefaultQuery

            For Each oItem As DevExpress.Web.ListEditItem In cmbIDConcept.Items
                If oItem.Value.ToString.StartsWith(oCurrentStartupValue.IDConcept & "_") Then
                    cmbIDConcept.SelectedItem = oItem
                End If
            Next

            If oCurrentStartupValue.ID > 0 Then
                cmbIDConcept.Enabled = False
            End If

            cmbStartValue.SelectedItem = Nothing
            cmbMaximumValue.SelectedItem = Nothing
            cmbMinimumValue.SelectedItem = Nothing
            Me.txtStartValue.Text = String.Empty
            Me.txtMaximumValue.Text = String.Empty
            Me.txtMinimumValue.Text = String.Empty

            Dim txtStartVF As String = ""
            Dim txtMaxVF As String = ""
            Dim txtMinVF As String = ""
            If oCurrentStartupValue.StartUserField IsNot Nothing Then txtStartVF = oCurrentStartupValue.StartUserField.FieldName
            If oCurrentStartupValue.MaximumUserField IsNot Nothing Then txtMaxVF = oCurrentStartupValue.MaximumUserField.FieldName
            If oCurrentStartupValue.MinimumUserField IsNot Nothing Then txtMinVF = oCurrentStartupValue.MinimumUserField.FieldName

            Me.txtAutomaticAccrualFix.Text = "1.0"
            Me.txtTotalHoursFix.Text = "0.0"
            Me.txtBaseHoursFix.Text = "0.0"
            Me.cmbAutomaticAccrualUF.SelectedItem = Nothing
            Me.cmdTotalHoursUF.SelectedItem = Nothing
            Me.cmbBaseHoursUF.SelectedItem = Nothing
            Me.cmbRoundType.SelectedIndex = 0
            Me.cmbAntUserField.SelectedIndex = 0
            Me.cmbCoeField.SelectedIndex = 0
            Me.cmbEndCustomPeriod.SelectedItem = Nothing

            Me.ckCoe.Checked = False
            Me.ckEndCustomPeriod.Checked = False

            If oCurrentStartupValue.StartValueType = LabAgreeValueType.None Then
                Me.optInitializaWith.Checked = False
                Me.rbAutomaticAccrualFix.Checked = True
                Me.rbTotalHoursFix.Checked = True
                Me.rbBaseHoursFix.Checked = True
                Me.chkYear.Checked = False
                Me.chkAnt.Checked = False
            ElseIf oCurrentStartupValue.StartValueType = LabAgreeValueType.DirectValue Then
                Me.optInitializaWith.Checked = True
                Me.rbStartValueFix.Checked = True
                Me.rbStartValueUF.Checked = False
                Me.rbCalculatedStartupValue.Checked = False
                Me.chkYear.Checked = False
                Me.chkAnt.Checked = False
                Me.txtStartValue.Text = oCurrentStartupValue.StartValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
            ElseIf oCurrentStartupValue.StartValueType = LabAgreeValueType.UserField Then
                Me.optInitializaWith.Checked = True
                Me.rbStartValueFix.Checked = False
                Me.rbStartValueUF.Checked = True
                Me.rbCalculatedStartupValue.Checked = False
                Me.chkYear.Checked = False
                Me.chkAnt.Checked = False
                cmbStartValue.SelectedItem = cmbStartValue.Items.FindByValue(txtStartVF)
            ElseIf oCurrentStartupValue.StartValueType = LabAgreeValueType.CalculatedValue Then
                Me.optInitializaWith.Checked = True
                Me.rbStartValueFix.Checked = False
                Me.rbStartValueUF.Checked = False
                Me.rbCalculatedStartupValue.Checked = True

                If oCurrentStartupValue.AccruedValueType = LabAgreeValueTypeBase.DirectValue Then
                    Me.rbAutomaticAccrualUF.Checked = False
                    Me.rbAutomaticAccrualFix.Checked = True

                    Me.txtAutomaticAccrualFix.Text = oCurrentStartupValue.AccruedValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
                ElseIf oCurrentStartupValue.AccruedValueType = LabAgreeValueTypeBase.UserField Then
                    Me.rbAutomaticAccrualUF.Checked = True
                    Me.rbAutomaticAccrualFix.Checked = False

                    Dim selField As String = If(oCurrentStartupValue.StartUserFieldAccruedValue Is Nothing, "", oCurrentStartupValue.StartUserFieldAccruedValue.FieldName)
                    Me.cmbAutomaticAccrualUF.SelectedItem = Me.cmbAutomaticAccrualUF.Items.FindByValue(selField)
                End If

                If oCurrentStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue Then
                    Me.rbTotalHoursUF.Checked = False
                    Me.rbTotalHoursFix.Checked = True

                    Me.txtTotalHoursFix.Text = oCurrentStartupValue.TotalPeriodBase.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
                ElseIf oCurrentStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField Then
                    Me.rbTotalHoursUF.Checked = True
                    Me.rbTotalHoursFix.Checked = False

                    Dim selField As String = If(oCurrentStartupValue.StartUserFieldTotalPeriodBase Is Nothing, "", oCurrentStartupValue.StartUserFieldTotalPeriodBase.FieldName)
                    Me.cmdTotalHoursUF.SelectedItem = Me.cmdTotalHoursUF.Items.FindByValue(selField)
                End If

                If oCurrentStartupValue.StartValueBaseType = LabAgreeValueTypeBase.DirectValue Then
                    Me.rbBaseHoursUF.Checked = False
                    Me.rbBaseHoursFix.Checked = True
                    Me.txtBaseHoursFix.Text = oCurrentStartupValue.StartValueBase.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
                ElseIf oCurrentStartupValue.StartValueBaseType = LabAgreeValueTypeBase.UserField Then
                    Me.rbBaseHoursUF.Checked = True
                    Me.rbBaseHoursFix.Checked = False
                    Dim selField As String = If(oCurrentStartupValue.StartUserFieldBase Is Nothing, "", oCurrentStartupValue.StartUserFieldBase.FieldName)
                    Me.cmbBaseHoursUF.SelectedItem = Me.cmbBaseHoursUF.Items.FindByValue(selField)
                End If

                If Not String.IsNullOrEmpty(oCurrentStartupValue.ScalingUserField) Then
                    Me.chkAnt.Checked = True
                Else
                    Me.chkAnt.Checked = False
                End If

                If Not String.IsNullOrEmpty(oCurrentStartupValue.ScalingCoefficientUserField) Then
                    Me.ckCoe.Checked = True
                Else
                    Me.ckCoe.Checked = False
                End If

                If Not (oCurrentStartupValue.EndCustomPeriodUserField Is Nothing) Then
                    If Not String.IsNullOrEmpty(oCurrentStartupValue.EndCustomPeriodUserField.FieldName) Then
                        Me.ckEndCustomPeriod.Checked = True
                        cmbEndCustomPeriod.SelectedItem = Me.cmbEndCustomPeriod.Items.FindByValue(oCurrentStartupValue.EndCustomPeriodUserField.FieldName)
                    Else
                        Me.ckEndCustomPeriod.Checked = False
                        Me.cmbEndCustomPeriod.SelectedIndex = -1
                    End If
                Else
                    Me.ckEndCustomPeriod.Checked = False
                    Me.cmbEndCustomPeriod.SelectedIndex = -1
                End If
                If oCurrentStartupValue.CalculatedType = 1 Then
                    Me.chkYear.Checked = True
                Else
                    Me.chkYear.Checked = False
                End If

                If Not String.IsNullOrEmpty(oCurrentStartupValue.ScalingUserField) Then
                    cmbAntUserField.SelectedItem = cmbAntUserField.Items.FindByValue(oCurrentStartupValue.ScalingUserField)
                End If

                If Not String.IsNullOrEmpty(oCurrentStartupValue.ScalingCoefficientUserField) Then
                    cmbCoeField.SelectedItem = cmbCoeField.Items.FindByValue(oCurrentStartupValue.ScalingCoefficientUserField)
                End If

                Me.cmbRoundType.SelectedItem = Me.cmbRoundType.Items.FindByValue(oCurrentStartupValue.RoundingType)
            End If

            Me.contractExceptionCriteria.ClearFilterValue()
            If oCurrentStartupValue.StartValueType <> LabAgreeValueType.None Then
                Me.ckStartContractException.Checked = oCurrentStartupValue.NewContractException
                If oCurrentStartupValue.NewContractExceptionCriteria IsNot Nothing AndAlso oCurrentStartupValue.NewContractExceptionCriteria.Count > 0 Then
                    Me.contractExceptionCriteria.SetFilterValue(oCurrentStartupValue.NewContractExceptionCriteria(0))
                End If
            End If

            If oCurrentStartupValue.MaximumValueType = LabAgreeValueType.None Then
                Me.optAlertWith.Checked = False
            ElseIf oCurrentStartupValue.MaximumValueType = LabAgreeValueType.DirectValue Then
                Me.optAlertWith.Checked = True
                Me.rbAlertValueFix.Checked = True
                Me.rbAlertValueUF.Checked = False
                Me.txtMaximumValue.Text = oCurrentStartupValue.MaximumValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
            ElseIf oCurrentStartupValue.MaximumValueType = LabAgreeValueType.UserField Then
                Me.optAlertWith.Checked = True
                Me.rbAlertValueFix.Checked = False
                Me.rbAlertValueUF.Checked = True
                cmbMaximumValue.SelectedItem = cmbMaximumValue.Items.FindByValue(txtMaxVF)
            End If

            If oCurrentStartupValue.MinimumValueType = LabAgreeValueType.None Then
                Me.optAlertMin.Checked = False
            ElseIf oCurrentStartupValue.MinimumValueType = LabAgreeValueType.DirectValue Then
                Me.optAlertMin.Checked = True
                Me.rbAlertMInValueFix.Checked = True
                Me.rbAlertMInValueUF.Checked = False
                Me.txtMinimumValue.Text = oCurrentStartupValue.MinimumValue.ToString().Replace(".", HelperWeb.GetDecimalDigitFormat())
            ElseIf oCurrentStartupValue.MinimumValueType = LabAgreeValueType.UserField Then
                Me.optAlertMin.Checked = True
                Me.rbAlertMInValueFix.Checked = False
                Me.rbAlertMInValueUF.Checked = True
                cmbMinimumValue.SelectedItem = cmbMinimumValue.Items.FindByValue(txtMinVF)
            End If



            Me.txtExpirationPeriodValue.Value = 0
            Me.cmbExpirationPeriodType.SelectedItem = Me.cmbExpirationPeriodType.Items(0)
            Me.txtEnjoymentPeriodValue.Value = 0
            Me.cmbEnjoymentPeriodType.SelectedItem = Me.cmbEnjoymentPeriodType.Items(0)
            Me.optAccrualExpiration.Checked = False
            Me.optAccrualEnjoyment.Checked = False


            If oConcept.DefaultQuery = "L" Then
                If oCurrentStartupValue.Expiration IsNot Nothing Then
                    Me.optAccrualExpiration.Checked = True
                    Me.txtExpirationPeriodValue.Value = oCurrentStartupValue.Expiration.ExpireAfter
                    Me.cmbExpirationPeriodType.SelectedItem = Me.cmbExpirationPeriodType.Items.FindByValue(CInt(oCurrentStartupValue.Expiration.Unit))
                End If

                If oCurrentStartupValue.Enjoyment IsNot Nothing Then
                    Me.optAccrualEnjoyment.Checked = True
                    Me.txtEnjoymentPeriodValue.Value = oCurrentStartupValue.Enjoyment.StartAfter
                    Me.cmbEnjoymentPeriodType.SelectedItem = Me.cmbEnjoymentPeriodType.Items.FindByValue(CInt(oCurrentStartupValue.Enjoyment.Unit))
                End If
            End If

            ' oCurrentStartupValueArray = oCurrentStartupValue.UserFieldValues.Split(New Char() {"@"c}, StringSplitOptions.RemoveEmptyEntries)
        Catch ex As Exception
            result = "KO"
        Finally

            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETLABAGREESTARTUP")
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpNameRO", If(oCurrentStartupValue?.Name, String.Empty))
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpValues", If(oCurrentStartupValue?.ScalingFieldValues, New List(Of roScalingValues)))
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Sub SaveLabAgreeStartup(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentStartupValue As roStartupValue = Nothing
        Dim bInclompleteData As Boolean = False

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then
                oCurrentStartupValue = New roStartupValue
                oCurrentStartupValue.ID = getMinStartupID()
            Else
                oCurrentStartupValue = getStartupValue(oParameters.ID)
            End If

            If oCurrentStartupValue Is Nothing Then Return

            oCurrentStartupValue.ScalingFieldValues = oParameters.AcumValues.ToList

            If Me.chkAnt.Checked AndAlso oCurrentStartupValue.ScalingFieldValues.Count = 0 Then
                bInclompleteData = True
            End If

            Try
                oCurrentStartupValue.IDConcept = roTypes.Any2Integer(cmbIDConcept.SelectedItem.Value.ToString.Split("_"c)(0))
            Catch ex As Exception
                bInclompleteData = True
            End Try

            If oParameters.ID < 0 Then
                oCurrentStartupValue.OriginalIDConcept = 0
            Else
                oCurrentStartupValue.OriginalIDConcept = oParameters.ID
            End If

            oCurrentStartupValue.Name = txtName.Text

            Dim bEnjoymentFieldsValid As Boolean = True

            If optAccrualEnjoyment.Checked AndAlso optAccrualExpiration.Checked AndAlso roTypes.Any2Integer(Me.txtExpirationPeriodValue.Text) < roTypes.Any2Integer(Me.txtEnjoymentPeriodValue.Text) Then
                bEnjoymentFieldsValid = False
            End If

            If bEnjoymentFieldsValid Then
                If Not bInclompleteData Then

                    oCurrentStartupValue.ScalingUserField = ""
                    oCurrentStartupValue.ScalingCoefficientUserField = ""

                    If Me.optInitializaWith.Checked Then


                        If Me.optAccrualExpiration.Checked Then
                            oCurrentStartupValue.Expiration = New roStartupValueExpirationRule With {
                                .ExpireAfter = roTypes.Any2Integer(Me.txtExpirationPeriodValue.Text),
                                .Unit = roTypes.Any2Integer(Me.cmbExpirationPeriodType.SelectedItem.Value)
                            }
                        Else
                            oCurrentStartupValue.Expiration = Nothing
                        End If

                        If Me.optAccrualEnjoyment.Checked Then
                            oCurrentStartupValue.Enjoyment = New roStartupValueEnjoymentRule With {
                                .StartAfter = roTypes.Any2Integer(Me.txtEnjoymentPeriodValue.Text),
                                .Unit = roTypes.Any2Integer(Me.cmbEnjoymentPeriodType.SelectedItem.Value)
                            }
                        Else
                            oCurrentStartupValue.Enjoyment = Nothing
                        End If


                        If rbStartValueUF.Checked Then
                            oCurrentStartupValue.StartValueType = LabAgreeValueType.UserField
                            If cmbStartValue.SelectedItem IsNot Nothing Then
                                Dim oMaxUF As New roUserField()
                                oMaxUF.FieldName = cmbStartValue.SelectedItem.Value
                                oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                oCurrentStartupValue.StartUserField = oMaxUF
                            Else
                                oCurrentStartupValue.StartUserField = Nothing
                                bInclompleteData = True
                            End If
                        ElseIf rbStartValueFix.Checked Then
                            If txtStartValue.Value IsNot Nothing AndAlso txtStartValue.Text <> String.Empty Then
                                oCurrentStartupValue.StartValueType = LabAgreeValueType.DirectValue
                                oCurrentStartupValue.StartValue = roTypes.Any2Double(txtStartValue.Text)
                            Else
                                bInclompleteData = True
                            End If
                        ElseIf rbCalculatedStartupValue.Checked Then
                            oCurrentStartupValue.StartValueType = LabAgreeValueType.CalculatedValue

                            If rbBaseHoursUF.Checked Then
                                oCurrentStartupValue.StartValueBaseType = LabAgreeValueTypeBase.UserField
                                If cmbBaseHoursUF.SelectedItem IsNot Nothing Then
                                    Dim oMaxUF As New roUserField()
                                    oMaxUF.FieldName = cmbBaseHoursUF.SelectedItem.Value
                                    oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                    oCurrentStartupValue.StartUserFieldBase = oMaxUF
                                Else
                                    oCurrentStartupValue.StartUserFieldBase = Nothing
                                    bInclompleteData = True
                                End If
                            ElseIf rbBaseHoursFix.Checked Then

                                If txtStartValue.Value IsNot Nothing AndAlso txtStartValue.Text <> String.Empty Then
                                    oCurrentStartupValue.StartValueBaseType = LabAgreeValueTypeBase.DirectValue
                                    oCurrentStartupValue.StartValueBase = roTypes.Any2Double(txtBaseHoursFix.Text)
                                Else
                                    bInclompleteData = True
                                End If
                            End If

                            If rbTotalHoursUF.Checked Then
                                oCurrentStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField
                                If cmdTotalHoursUF.SelectedItem IsNot Nothing Then
                                    Dim oMaxUF As New roUserField()
                                    oMaxUF.FieldName = cmdTotalHoursUF.SelectedItem.Value
                                    oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                    oCurrentStartupValue.StartUserFieldTotalPeriodBase = oMaxUF
                                Else
                                    oCurrentStartupValue.StartUserFieldTotalPeriodBase = Nothing
                                    bInclompleteData = True
                                End If
                            ElseIf rbTotalHoursFix.Checked Then

                                If txtStartValue.Value IsNot Nothing AndAlso txtStartValue.Text <> String.Empty Then
                                    oCurrentStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue
                                    oCurrentStartupValue.TotalPeriodBase = roTypes.Any2Double(txtTotalHoursFix.Text)
                                Else
                                    bInclompleteData = True
                                End If
                            End If

                            If rbAutomaticAccrualUF.Checked Then
                                oCurrentStartupValue.AccruedValueType = LabAgreeValueTypeBase.UserField
                                If cmbAutomaticAccrualUF.SelectedItem IsNot Nothing Then
                                    Dim oMaxUF As New roUserField()
                                    oMaxUF.FieldName = cmbAutomaticAccrualUF.SelectedItem.Value
                                    oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                    oCurrentStartupValue.StartUserFieldAccruedValue = oMaxUF
                                Else
                                    oCurrentStartupValue.StartUserFieldAccruedValue = Nothing
                                    bInclompleteData = True
                                End If
                            ElseIf rbAutomaticAccrualFix.Checked Then

                                If txtStartValue.Value IsNot Nothing AndAlso txtStartValue.Text <> String.Empty Then
                                    oCurrentStartupValue.AccruedValueType = LabAgreeValueTypeBase.DirectValue
                                    oCurrentStartupValue.AccruedValue = roTypes.Any2Double(txtAutomaticAccrualFix.Text)
                                Else
                                    bInclompleteData = True
                                End If
                            End If

                            If cmbRoundType.SelectedItem IsNot Nothing Then
                                oCurrentStartupValue.RoundingType = cmbRoundType.SelectedItem.Value
                            Else
                                oCurrentStartupValue.RoundingType = 0
                            End If

                            'oCurrentStartupValue.StartUpFieldValues = oParameters.AcumValues
                            If chkAnt.Checked Then

                                oCurrentStartupValue.ScalingUserField = cmbAntUserField.SelectedItem.Value

                            End If

                            If ckCoe.Checked Then
                                oCurrentStartupValue.ScalingCoefficientUserField = cmbCoeField.SelectedItem.Value
                            End If

                            If ckEndCustomPeriod.Checked Then
                                If cmbEndCustomPeriod.SelectedItem Is Nothing Then
                                    bInclompleteData = True
                                Else
                                    oCurrentStartupValue.ApplyEndCustomPeriod = True
                                    Dim oEndUF As New roUserField()
                                    oEndUF.FieldName = cmbEndCustomPeriod.SelectedItem.Value
                                    oEndUF.Type = UserFieldsTypes.Types.EmployeeField
                                    oCurrentStartupValue.EndCustomPeriodUserField = oEndUF
                                End If
                            Else
                                oCurrentStartupValue.ApplyEndCustomPeriod = False
                                oCurrentStartupValue.EndCustomPeriodUserField = Nothing
                            End If

                            If chkYear.Checked Then
                                oCurrentStartupValue.CalculatedType = 1
                            Else
                                oCurrentStartupValue.CalculatedType = 0
                            End If
                        Else
                            bInclompleteData = True
                        End If

                        If rbStartValueUF.Checked OrElse rbStartValueFix.Checked OrElse rbCalculatedStartupValue.Checked Then
                            If Me.ckStartContractException.Checked Then
                                If contractExceptionCriteria.IsValidFilter Then
                                    Dim xList As New Generic.List(Of roUserFieldCondition)
                                    xList.Add(contractExceptionCriteria.OConditionValue)
                                    oCurrentStartupValue.NewContractExceptionCriteria = xList
                                Else
                                    bInclompleteData = True
                                End If
                            Else
                                oCurrentStartupValue.NewContractExceptionCriteria = New List(Of roUserFieldCondition)
                            End If

                            oCurrentStartupValue.NewContractException = Me.ckStartContractException.Checked
                        End If
                    Else
                        oCurrentStartupValue.StartValueType = LabAgreeValueType.None
                        oCurrentStartupValue.StartValue = Nothing
                    End If
                End If

                If Not bInclompleteData Then
                    If Me.optAlertWith.Checked Then
                        If rbAlertValueUF.Checked Then
                            oCurrentStartupValue.MaximumValueType = LabAgreeValueType.UserField
                            If cmbMaximumValue.SelectedItem IsNot Nothing Then
                                Dim oMaxUF As New roUserField()
                                oMaxUF.FieldName = cmbMaximumValue.SelectedItem.Value
                                oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                oCurrentStartupValue.MaximumUserField = oMaxUF
                            Else
                                bInclompleteData = True
                                oCurrentStartupValue.MaximumUserField = Nothing
                            End If
                        ElseIf rbAlertValueFix.Checked Then
                            If txtMaximumValue.Value IsNot Nothing AndAlso txtMaximumValue.Text <> String.Empty Then
                                oCurrentStartupValue.MaximumValueType = LabAgreeValueType.DirectValue
                                oCurrentStartupValue.MaximumValue = roTypes.Any2Double(txtMaximumValue.Text)
                            Else
                                bInclompleteData = True
                            End If
                        Else
                            bInclompleteData = True
                        End If
                    Else
                        oCurrentStartupValue.MaximumValueType = LabAgreeValueType.None
                        oCurrentStartupValue.MaximumValue = Nothing
                    End If
                End If

                If Not bInclompleteData Then
                    If Me.optAlertMin.Checked Then
                        If rbAlertMInValueUF.Checked Then
                            oCurrentStartupValue.MinimumValueType = LabAgreeValueType.UserField
                            If cmbMinimumValue.SelectedItem IsNot Nothing Then
                                Dim oMaxUF As New roUserField()
                                oMaxUF.FieldName = cmbMinimumValue.SelectedItem.Value
                                oMaxUF.Type = UserFieldsTypes.Types.EmployeeField
                                oCurrentStartupValue.MinimumUserField = oMaxUF
                            Else
                                bInclompleteData = True
                                oCurrentStartupValue.MinimumUserField = Nothing
                            End If
                        ElseIf rbAlertMInValueFix.Checked Then
                            If txtMinimumValue.Value IsNot Nothing AndAlso txtMinimumValue.Text <> String.Empty Then
                                oCurrentStartupValue.MinimumValueType = LabAgreeValueType.DirectValue
                                oCurrentStartupValue.MinimumValue = roTypes.Any2Double(txtMinimumValue.Text)
                            Else
                                bInclompleteData = True
                            End If
                        Else
                            bInclompleteData = True
                        End If
                    Else
                        oCurrentStartupValue.MinimumValueType = LabAgreeValueType.None
                        oCurrentStartupValue.MinimumValue = Nothing
                    End If
                End If
            End If

            If Not Me.optAlertMin.Checked AndAlso Not Me.optAlertWith.Checked AndAlso Not Me.optInitializaWith.Checked Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.NoSelection", Me.DefaultScope))
            End If

            'modificar el startup value de la variable de sesion
            If Not rError.Error AndAlso bInclompleteData Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.FieldsEmpty", Me.DefaultScope))
            End If

            If Not rError.Error AndAlso Not bEnjoymentFieldsValid Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.EnjoymentFields", Me.DefaultScope))
            End If

            If ExistsConceptInList(oCurrentStartupValue.ID, oCurrentStartupValue.IDConcept) Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.AlreadyExists", Me.DefaultScope))
            End If

            If Not rError.Error Then
                setStartupValue(oCurrentStartupValue, bolIsNew)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVELABAGREESTARTUP")
            ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpNameRO", If(oCurrentStartupValue?.Name, String.Empty))

            If Not rError.Error Then
                ASPxStartupValueCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                ASPxStartupValueCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxStartupValueCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

End Class