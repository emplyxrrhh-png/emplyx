Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmEditLabAgreeRule
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequestLabAgreeRule

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

#Region "Helper private methods"

    Private Property LabAgreedRulesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roLabAgreeAccrualRule)
        Get

            Dim tbCauses As Generic.List(Of roLabAgreeAccrualRule) = Session("LabAgree_LabAgreedRulesData")

            If bolReload Or tbCauses Is Nothing Then
                Dim oList As New Generic.List(Of roLabAgreeAccrualRule)
                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me.Page, Session("LabAgree_SelectedID"), True)
                    For Each oLabObject As roLabAgreeAccrualRule In oCurrentLabAgree.LabAgreeAccrualRules

                        If oLabObject.EndDate = New Date(2079, 1, 1) Then
                            oLabObject.EndDate = Nothing
                        End If

                        oList.Add(oLabObject)
                    Next

                End If
                tbCauses = oList
                Session("LabAgree_LabAgreedRulesData") = oList

            End If
            Return tbCauses

        End Get
        Set(ByVal value As Generic.List(Of roLabAgreeAccrualRule))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedRulesData") = value
            Else
                Session("LabAgree_LabAgreedRulesData") = Nothing
            End If
        End Set
    End Property

    Private Function getAccrualRule(ByVal oId As Integer) As roLabAgreeAccrualRule
        Dim oObject As roLabAgreeAccrualRule = Nothing

        Dim oList As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeAccrualRule In oList
                If oElem.IDAccrualRule = oId Then
                    oObject = roSupport.DeepClone(Of roLabAgreeAccrualRule)(oElem)

                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function getMinAccrualID() As Integer
        Dim newId As Integer = 0

        Dim oList As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            Dim index As Integer = 0
            For Each oElem As roLabAgreeAccrualRule In oList
                If index = 0 Then
                    newId = oElem.IDAccrualRule
                Else
                    If oElem.IDAccrualRule < newId Then
                        newId = oElem.IDAccrualRule
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

    Private Function setAccrualRule(ByVal oObject As roLabAgreeAccrualRule, ByVal bIsNew As Boolean) As Boolean
        Dim bResult As Boolean = False

        Dim oList As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData()

        If Not bIsNew Then
            Dim index As Integer = 0
            If oList IsNot Nothing AndAlso oList.Count > 0 Then
                For Each oElem As roLabAgreeAccrualRule In oList
                    If oElem.IDAccrualRule = oObject.IDAccrualRule Then
                        oList(index) = oObject
                        bResult = True
                        LabAgreedRulesData = oList
                        Exit For
                    End If
                    index = index + 1
                Next
            End If
        Else
            If oList Is Nothing Then oList = New Generic.List(Of roLabAgreeAccrualRule)
            oList.Add(oObject)
            LabAgreedRulesData = oList
        End If

        Return bResult
    End Function

    Private Function ExistsNameInList(ByVal oId As Integer, ByVal strName As String) As Boolean
        Dim bExists As Boolean = False

        Dim oList As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeAccrualRule In oList
                If oElem.IDAccrualRule <> oId AndAlso oElem.LabAgreeRule.Name = strName Then
                    bExists = True
                    Exit For
                End If
            Next
        End If

        Return bExists
    End Function

#End Region

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadCombos()
        End If
    End Sub

    Protected Sub ASPxRuleCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxRuleCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequestLabAgreeRule()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "GETLABAGREERULE"
                LoadLabAgreeRule(oParameters)
            Case "SAVELABAGREERULE"
                SaveLabAgreeRule(oParameters)
            Case Else
                ASPxRuleCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxRuleCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxRuleCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

        'Mostra el TAB seleccionat
        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadLabAgreeRule(ByVal oParameters As ObjectCallbackRequestLabAgreeRule, Optional ByVal eLabAgreeRule As roLabAgreeRule = Nothing)
        Dim oCurrentLabAgreeRule As roLabAgreeRule = Nothing
        Dim oCurrentLabAgree As roLabAgreeAccrualRule = Nothing
        Dim result As String = "OK"
        Try

            If eLabAgreeRule IsNot Nothing Then
                oCurrentLabAgreeRule = eLabAgreeRule
            Else
                If oParameters.ID = "-1" Then
                    oCurrentLabAgree = New roLabAgreeAccrualRule

                    oCurrentLabAgree.IDAccrualRule = getMinAccrualID()

                    oCurrentLabAgreeRule = New roLabAgreeRule
                    oCurrentLabAgreeRule.Name = String.Empty
                    oCurrentLabAgreeRule.Description = String.Empty

                    oCurrentLabAgree.IDLabAgree = oParameters.IDLabAgree
                    oCurrentLabAgree.LabAgreeRule = oCurrentLabAgreeRule
                Else
                    oCurrentLabAgree = getAccrualRule(oParameters.ID)
                    oCurrentLabAgreeRule = oCurrentLabAgree.LabAgreeRule

                End If
            End If

            If oCurrentLabAgreeRule Is Nothing Or oCurrentLabAgree Is Nothing Then Exit Sub

            'LoadCombos()

            If oCurrentLabAgree.BeginDate = DateTime.MinValue Then
                txtInitialDate.Date = Date.Now.Date
            Else
                txtInitialDate.Date = oCurrentLabAgree.BeginDate
            End If

            If oCurrentLabAgree.EndDate = DateTime.MinValue Or oCurrentLabAgree.EndDate = New DateTime(2079, 1, 1, 0, 0, 0) Then
                txtEndDate.Value = Nothing
            Else
                txtEndDate.Date = oCurrentLabAgree.EndDate
            End If

            Dim IDType As String = ""
            Dim intMainAccrual As Integer = -1
            Dim intComparation As Integer = -1
            Dim intSubAccrual As Integer = -1

            Dim oValueType As LabAgreeRuleDefinitionValueType
            Dim strValueTime As Double = 0
            Dim strValueOnce As Double = 0
            Dim strValueUserFieldH As String = ""
            Dim strValueUserFieldO As String = ""
            Dim intValueIDConcept As Integer = -1

            Dim intAction As Integer = -1
            Dim intDif As Integer = -1
            Dim strUntilTime As Double = 0
            Dim strUntilOnce As Double = 0
            Dim strUntilUserFieldH As String = ""
            Dim strUntilUserFieldO As String = ""

            Dim intDestiAccrual As Integer = -1

            If oCurrentLabAgreeRule.Definition IsNot Nothing Then
                intMainAccrual = oCurrentLabAgreeRule.Definition.MainAccrual
                Dim oConceptMA As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, intMainAccrual, False)
                IDType = oConceptMA.IDType

                oValueType = oCurrentLabAgreeRule.Definition.ValueType
                If oConceptMA.IDType = "H" Then 'Tiempo
                    strValueTime = oCurrentLabAgreeRule.Definition.Value
                    If oCurrentLabAgreeRule.Definition.ValueUserField IsNot Nothing Then
                        strValueUserFieldH = oCurrentLabAgreeRule.Definition.ValueUserField.FieldName
                    End If
                Else 'Veces
                    strValueOnce = oCurrentLabAgreeRule.Definition.Value
                    If oCurrentLabAgreeRule.Definition.ValueUserField IsNot Nothing Then
                        strValueUserFieldO = oCurrentLabAgreeRule.Definition.ValueUserField.FieldName
                    End If
                End If
                intValueIDConcept = oCurrentLabAgreeRule.Definition.ValueIDConcept

                intComparation = oCurrentLabAgreeRule.Definition.Comparation
                intAction = oCurrentLabAgreeRule.Definition.Action

                intDif = oCurrentLabAgreeRule.Definition.Dif
                If oConceptMA.IDType = "H" Then 'Tiempo
                    strUntilTime = oCurrentLabAgreeRule.Definition.Until
                    If oCurrentLabAgreeRule.Definition.UntilUserField IsNot Nothing Then
                        strUntilUserFieldH = oCurrentLabAgreeRule.Definition.UntilUserField.FieldName
                    End If
                Else 'Veces
                    strUntilOnce = oCurrentLabAgreeRule.Definition.Until
                    If oCurrentLabAgreeRule.Definition.UntilUserField IsNot Nothing Then
                        strUntilUserFieldO = oCurrentLabAgreeRule.Definition.UntilUserField.FieldName
                    End If
                End If

                intDestiAccrual = oCurrentLabAgreeRule.Definition.DestiAccrual
            End If

            If oCurrentLabAgreeRule.Name IsNot Nothing Then txtName.Text = oCurrentLabAgreeRule.Name
            If oCurrentLabAgreeRule.Description IsNot Nothing Then txtDescription.Text = oCurrentLabAgreeRule.Description

            hdnIDType.Value = IDType

            cmbMainAccrual.SelectedItem = Nothing
            For Each oItem As DevExpress.Web.ListEditItem In cmbMainAccrual.Items
                If roTypes.Any2String(oItem.Value).StartsWith(intMainAccrual & "_") Then
                    cmbMainAccrual.SelectedItem = oItem
                    Exit For
                End If
            Next

            cmbComparation.SelectedItem = cmbComparation.Items.FindByValue(intComparation)
            cmbValueTypes.SelectedItem = cmbValueTypes.Items.FindByValue(oValueType.ToString())
            txtValueTime.Text = strValueTime
            txtValueOnce.Text = strValueOnce
            cmbValueUserFieldsH.SelectedItem = cmbValueUserFieldsH.Items.FindByValue(strValueUserFieldH)
            cmbValueUserFieldsO.SelectedItem = cmbValueUserFieldsO.Items.FindByValue(strValueUserFieldO)
            cmbValueConcepts.SelectedItem = cmbValueConcepts.Items.FindByValue(intValueIDConcept)

            cmbAction.SelectedItem = cmbAction.Items.FindByValue(intAction)
            cmbDif.SelectedItem = cmbDif.Items.FindByValue(intDif)

            txtUntilTime.Text = strUntilTime
            txtUntilOnce.Text = strUntilOnce

            cmbUntilUserFieldsH.SelectedItem = cmbUntilUserFieldsH.Items.FindByValue(strUntilUserFieldH)
            cmbUntilUserFieldsO.SelectedItem = cmbUntilUserFieldsO.Items.FindByValue(strUntilUserFieldO)

            cmbDestiCause.SelectedItem = cmbDestiCause.Items.FindByValue(intDestiAccrual)

            Me.optSchedule1.SetValue(oCurrentLabAgreeRule.Schedule)
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETLABAGREERULE")
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentLabAgreeRule.Name)
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Sub SaveLabAgreeRule(ByVal oParameters As ObjectCallbackRequestLabAgreeRule)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentLabAgreeRule As roLabAgreeRule = Nothing
        Dim oCurrentLabAgree As roLabAgreeAccrualRule = Nothing
        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then
                oCurrentLabAgree = New roLabAgreeAccrualRule
                oCurrentLabAgreeRule = New roLabAgreeRule
                oCurrentLabAgree.IDAccrualRule = getMinAccrualID()

                oCurrentLabAgreeRule.Name = String.Empty
                oCurrentLabAgreeRule.Description = String.Empty

                oCurrentLabAgree.IDLabAgree = oParameters.IDLabAgree
                oCurrentLabAgree.LabAgreeRule = oCurrentLabAgreeRule
            Else
                oCurrentLabAgree = getAccrualRule(oParameters.ID)
                oCurrentLabAgreeRule = oCurrentLabAgree.LabAgreeRule
            End If

            If oCurrentLabAgreeRule Is Nothing Or oCurrentLabAgree Is Nothing Then Exit Sub

            oCurrentLabAgreeRule.Name = txtName.Text
            oCurrentLabAgreeRule.Description = txtDescription.Text

            oCurrentLabAgree.BeginDate = txtInitialDate.Date.Date

            If txtEndDate.Value Is Nothing Then
                oCurrentLabAgree.EndDate = Nothing
            Else
                oCurrentLabAgree.EndDate = txtEndDate.Date.Date
            End If

            oCurrentLabAgreeRule.Schedule = Me.optSchedule1.GetValue()

            'LoadCombos()

            oCurrentLabAgreeRule.Definition = New roLabAgreeRuleDefinition
            oCurrentLabAgreeRule.Definition.MainAccrual = roTypes.Any2Integer(roTypes.Any2String(cmbMainAccrual.SelectedItem.Value).Split("_")(0))
            oCurrentLabAgreeRule.Definition.Comparation = cmbComparation.SelectedItem.Value

            Select Case cmbValueTypes.SelectedItem.Value.ToString.ToUpper
                Case "CONCEPTVALUE"
                    oCurrentLabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.ConceptValue
                Case "DIRECTVALUE"
                    oCurrentLabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.DirectValue
                Case "USERFIELDVALUE"
                    oCurrentLabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.UserFieldValue
            End Select

            If hdnIDType.Value = "H" Then
                oCurrentLabAgreeRule.Definition.Value = txtValueTime.Text

                oCurrentLabAgreeRule.Definition.ValueUserField = New roUserField
                oCurrentLabAgreeRule.Definition.ValueUserField.Type = UserFieldsTypes.Types.EmployeeField
                If cmbValueUserFieldsH.SelectedItem IsNot Nothing Then
                    oCurrentLabAgreeRule.Definition.ValueUserField.FieldName = cmbValueUserFieldsH.SelectedItem.Value
                Else
                    oCurrentLabAgreeRule.Definition.ValueUserField.FieldName = Nothing
                End If

                oCurrentLabAgreeRule.Definition.Until = txtUntilTime.Text

                oCurrentLabAgreeRule.Definition.UntilUserField = New roUserField
                oCurrentLabAgreeRule.Definition.UntilUserField.Type = UserFieldsTypes.Types.EmployeeField
                If cmbUntilUserFieldsH.SelectedItem IsNot Nothing Then
                    oCurrentLabAgreeRule.Definition.UntilUserField.FieldName = cmbUntilUserFieldsH.SelectedItem.Value
                Else
                    oCurrentLabAgreeRule.Definition.UntilUserField.FieldName = Nothing
                End If
            Else
                oCurrentLabAgreeRule.Definition.Value = txtValueOnce.Text

                oCurrentLabAgreeRule.Definition.ValueUserField = New roUserField
                oCurrentLabAgreeRule.Definition.ValueUserField.Type = UserFieldsTypes.Types.EmployeeField
                If cmbValueUserFieldsO.SelectedItem IsNot Nothing Then
                    oCurrentLabAgreeRule.Definition.ValueUserField.FieldName = cmbValueUserFieldsO.SelectedItem.Value
                Else
                    oCurrentLabAgreeRule.Definition.ValueUserField.FieldName = Nothing
                End If

                oCurrentLabAgreeRule.Definition.Until = txtUntilOnce.Text

                oCurrentLabAgreeRule.Definition.UntilUserField = New roUserField
                oCurrentLabAgreeRule.Definition.UntilUserField.Type = UserFieldsTypes.Types.EmployeeField
                If cmbUntilUserFieldsO.SelectedItem IsNot Nothing Then
                    oCurrentLabAgreeRule.Definition.UntilUserField.FieldName = cmbUntilUserFieldsO.SelectedItem.Value
                Else
                    oCurrentLabAgreeRule.Definition.UntilUserField.FieldName = Nothing
                End If

            End If
            If cmbValueConcepts.SelectedItem IsNot Nothing Then
                oCurrentLabAgreeRule.Definition.ValueIDConcept = cmbValueConcepts.SelectedItem.Value
            Else
                oCurrentLabAgreeRule.Definition.ValueIDConcept = 0
            End If

            oCurrentLabAgreeRule.Definition.Action = cmbAction.SelectedItem.Value
            oCurrentLabAgreeRule.Definition.Dif = cmbDif.SelectedItem.Value
            oCurrentLabAgreeRule.Definition.DestiAccrual = cmbDestiCause.SelectedItem.Value

            If ExistsNameInList(oCurrentLabAgree.IDAccrualRule, oCurrentLabAgree.LabAgreeRule.Name) Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.AlreadyExists", Me.DefaultScope))
            End If

            If rError.Error = False Then
                setAccrualRule(oCurrentLabAgree, bolIsNew)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVELABAGREERULE")
            ASPxRuleCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentLabAgreeRule.Name)

            If rError.Error = False Then
                ASPxRuleCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxRuleCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                ASPxRuleCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxRuleCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

    Private Sub LoadCombos()
        Dim dTblConcepts As DataTable = API.ConceptsServiceMethods.GetConcepts(Me.Page)
        Me.cmbMainAccrual.Items.Clear()
        Me.cmbValueTypes.Items.Clear()
        Me.cmbValueConcepts.Items.Clear()

        ' Lleno selector tipos valores (valor directo, campo de la ficha y acumulado)
        For Each ValueType As LabAgreeRuleDefinitionValueType In System.Enum.GetValues(GetType(LabAgreeRuleDefinitionValueType))
            cmbValueTypes.Items.Add(Me.Language.Translate("ValueType." & System.Enum.GetName(ValueType.GetType, ValueType), Me.DefaultScope), ValueType)
        Next

        'TODO: Obsolets es tenen d'ensenyar?
        cmbMainAccrual.ValueType = GetType(String)
        cmbValueConcepts.ValueType = GetType(Integer)
        For Each dRowConcept As DataRow In dTblConcepts.Rows
            'No hay arrastres de saldos de tipo contrato anualiazdo
            If roTypes.Any2String(dRowConcept("DefaultQuery")) <> "L" Then
                cmbMainAccrual.Items.Add(dRowConcept("Name"), dRowConcept("ID") & "_" & dRowConcept("IDType") & "_" & dRowConcept("DefaultQuery"))
                cmbValueConcepts.Items.Add(dRowConcept("Name"), dRowConcept("ID"))
            End If
        Next

        Dim strComparations As String() = System.Enum.GetNames(GetType(LabAgreeRuleDefinitionComparation))
        Dim strValueComp As Integer() = System.Enum.GetValues(GetType(LabAgreeRuleDefinitionComparation))

        cmbComparation.ValueType = GetType(Integer)
        For n As Integer = LBound(strComparations) To UBound(strComparations)
            cmbComparation.Items.Add(Me.Language.Keyword("Comparation." & strComparations(n)), strValueComp(n))
        Next

        Dim strActions As String() = System.Enum.GetNames(GetType(LabAgreeRuleDefinitionAction))
        Dim strValueAction As Integer() = System.Enum.GetValues(GetType(LabAgreeRuleDefinitionAction))

        cmbAction.ValueType = GetType(Integer)
        For n As Integer = LBound(strActions) To UBound(strActions)
            cmbAction.Items.Add(Me.Language.Keyword("Action." & strActions(n)), strValueAction(n))
        Next

        Dim strDifs As String() = System.Enum.GetNames(GetType(LabAgreeRuleDefinitionDif))
        Dim strValueDifs As Integer() = System.Enum.GetValues(GetType(LabAgreeRuleDefinitionDif))

        cmbDif.ValueType = GetType(Integer)
        For n As Integer = LBound(strDifs) To UBound(strDifs)
            cmbDif.Items.Add(Me.Language.Keyword("Dif." & strDifs(n)), strValueDifs(n))
        Next

        Dim dTblCauses As DataTable = CausesServiceMethods.GetCauses(Me.Page)
        Me.cmbDestiCause.Items.Clear()
        cmbDestiCause.ValueType = GetType(Integer)
        For Each dRowCause As DataRow In dTblCauses.Rows
            cmbDestiCause.Items.Add(dRowCause("Name"), dRowCause("ID")) ', "")
        Next

        Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3,4) AND Used=1", False)
        For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
            If oRow("FieldType") = 1 Or oRow("FieldType") = 3 Then ' Tipo numérico o decimal
                cmbValueUserFieldsO.Items.Add(oRow("FieldName"), oRow("FieldName"))
                cmbUntilUserFieldsO.Items.Add(oRow("FieldName"), oRow("FieldName"))
            ElseIf oRow("FieldType") = 4 Then ' Tipo hora
                cmbValueUserFieldsH.Items.Add(oRow("FieldName"), oRow("FieldName"))
                cmbUntilUserFieldsH.Items.Add(oRow("FieldName"), oRow("FieldName"))
            End If
        Next
    End Sub

End Class