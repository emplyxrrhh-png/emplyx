Imports DevExpress.Web
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class WebUserForms_frmFilterBusinessCenters
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="state")>
        Public BcState As String

        <Runtime.Serialization.DataMember(Name:="name")>
        Public BcName As String

        <Runtime.Serialization.DataMember(Name:="fieldFilters")>
        Public BcFieldFilters As List(Of ObjectCallbackRequestItem)

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequestItem

        <Runtime.Serialization.DataMember(Name:="FieldName")>
        Public FieldId As String

        <Runtime.Serialization.DataMember(Name:="CriteriaContains")>
        Public CriteriaContains As String

        <Runtime.Serialization.DataMember(Name:="Value")>
        Public Value As String

        <Runtime.Serialization.DataMember(Name:="Condition")>
        Public Condition As String

    End Class

    ReadOnly _lstCriteria = New List(Of String())

    Property Activate() As Boolean
    Property State() As Boolean
    Property NameBc() As String
    Property BcFieldsFilter() As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            LoadCriteria()
            LoadData()
            Activate = False
            State = False
            NameBc = String.Empty
            BcFieldsFilter = String.Empty
            LoadFilter()
        End If

    End Sub

    Private Sub LoadCriteria()
        _lstCriteria.Add({"Criteria.None", " "})
        _lstCriteria.Add({"Criteria.Equal", "="})
        _lstCriteria.Add({"Criteria.Major", ">"})
        _lstCriteria.Add({"Criteria.MajororEqual", ">="})
        _lstCriteria.Add({"Criteria.Minor", "<"})
        _lstCriteria.Add({"Criteria.MinororEqual", "<="})
        _lstCriteria.Add({"Criteria.Different", "<>"})
        _lstCriteria.Add({"Criteria.StartsWith", "*"})
        _lstCriteria.Add({"Criteria.Contains", "*X*"})
    End Sub

    Private Sub LoadData()
        ClearCombos()
        ControlsProperties()

        cmbState.Items.Add(Language.Translate("BusinessCenterState.Active", DefaultScope), 1)
        cmbState.Items.Add(Language.Translate("BusinessCenterState.Inactive", DefaultScope), 0)
        rblAndOr1.Items.Add(Language.Translate("BusinessCenterState.And", DefaultScope), "AND")
        rblAndOr1.Items.Add(Language.Translate("BusinessCenterState.Or", DefaultScope), "OR")
        rblAndOr2.Items.Add(Language.Translate("BusinessCenterState.And", DefaultScope), "AND")
        rblAndOr2.Items.Add(Language.Translate("BusinessCenterState.Or", DefaultScope), "OR")
        rblAndOr3.Items.Add(Language.Translate("BusinessCenterState.And", DefaultScope), "AND")
        rblAndOr3.Items.Add(Language.Translate("BusinessCenterState.Or", DefaultScope), "OR")
        rblAndOr4.Items.Add(Language.Translate("BusinessCenterState.And", DefaultScope), "AND")
        rblAndOr4.Items.Add(Language.Translate("BusinessCenterState.Or", DefaultScope), "OR")

        Dim businessCenterFields As DataTable = API.UserFieldServiceMethods.GetBusinessCenterFields(Page, Types.TaskField)

        For Each dRowUf As DataRow In businessCenterFields.Rows
            cmbBCFieldsValues1.Items.Add(dRowUf("Name"), dRowUf("ID"))
            cmbBCFieldsValues2.Items.Add(dRowUf("Name"), dRowUf("ID"))
            cmbBCFieldsValues3.Items.Add(dRowUf("Name"), dRowUf("ID"))
            cmbBCFieldsValues4.Items.Add(dRowUf("Name"), dRowUf("ID"))
            cmbBCFieldsValues5.Items.Add(dRowUf("Name"), dRowUf("ID"))
        Next
        For Each criteria As String() In _lstCriteria
            cmbBCCriteria1.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
            cmbBCCriteria2.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
            cmbBCCriteria3.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
            cmbBCCriteria4.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
            cmbBCCriteria5.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
        Next

    End Sub

    Private Sub ControlsProperties()

        rblAndOr1.ControlStyle.BorderStyle = BorderStyle.None
        rblAndOr2.ControlStyle.BorderStyle = BorderStyle.None
        rblAndOr3.ControlStyle.BorderStyle = BorderStyle.None
        rblAndOr4.ControlStyle.BorderStyle = BorderStyle.None
        cmbBCFieldsValues1.ValueType = GetType(Integer)
        cmbBCFieldsValues2.ValueType = GetType(Integer)
        cmbBCFieldsValues3.ValueType = GetType(Integer)
        cmbBCFieldsValues4.ValueType = GetType(Integer)
        rblAndOr1.ValueType = GetType(String)
        rblAndOr2.ValueType = GetType(String)
        rblAndOr3.ValueType = GetType(String)
        rblAndOr4.ValueType = GetType(String)
        cmbState.ValueType = GetType(Integer)
        cmbState.Items.Add("", Nothing)
        cmbBCFieldsValues1.Items.Add("", Nothing)
        cmbBCFieldsValues2.Items.Add("", Nothing)
        cmbBCFieldsValues3.Items.Add("", Nothing)
        cmbBCFieldsValues4.Items.Add("", Nothing)
        cmbBCFieldsValues5.Items.Add("", Nothing)
    End Sub

    Protected Sub ASPxBusinessCentersCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxBusinessCentersCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())
        Dim strWhere = String.Empty
        If (Not String.IsNullOrEmpty(oParameters.BcState)) Then strWhere = "Status = " & oParameters.BcState
        If (Not String.IsNullOrEmpty(oParameters.BcName)) Then
            If (String.IsNullOrEmpty(strWhere)) Then
                strWhere = "Name like '%" & oParameters.BcName & "%'"
            Else
                strWhere = strWhere & " AND Name like '%" & oParameters.BcName & "%'"
            End If
        End If
        Dim strWhereFields = "("
        Dim moreThanOne = False
        If (oParameters.BcFieldFilters IsNot Nothing AndAlso oParameters.BcFieldFilters.Count > 0) Then
            For Each filter As ObjectCallbackRequestItem In oParameters.BcFieldFilters
                If (Not String.IsNullOrEmpty(filter.Value)) Then
                    If (filter.CriteriaContains.Contains("*")) Then
                        If (filter.CriteriaContains.Equals("*X*")) Then
                            strWhereFields = strWhereFields & " Field" & filter.FieldId & " like '%" & filter.Value & "%'"
                        Else
                            strWhereFields = strWhereFields & " Field" & filter.FieldId & " like '" & filter.Value & "%'"
                        End If
                    Else
                        strWhereFields = strWhereFields & " Field" & filter.FieldId & " " & filter.CriteriaContains & " " & filter.Value
                    End If
                Else
                    If (moreThanOne) Then
                        strWhereFields = strWhereFields.Substring(0, strWhereFields.Length - 3)
                    End If
                End If
                If (String.IsNullOrEmpty(filter.Condition)) Then Exit For
                strWhereFields = strWhereFields & " " & filter.Condition
                moreThanOne = True
            Next
        End If
        strWhereFields = strWhereFields & ")"
        If (Not strWhereFields.Trim().Equals("()")) Then
            If (Not String.IsNullOrEmpty(strWhere)) Then
                strWhere = strWhere & " AND " & strWhereFields
            Else
                strWhere = strWhereFields
            End If
        End If

        Session.Add("TreeFilter", If(String.IsNullOrEmpty(strWhere), String.Empty, strWhere & "|" & strParameter))

        'WLHelperWeb.RedirectToUrl("~/Tasks/BusinessCentersSelectorData.aspx?OnlyGroups=0&ImagesPath=/Base/WebUserControls/../../Tasks/images/BusinessCentersSelector&Filters=11110&FilterUserFields=&FilterFixed=U&FeatureAlias=&FeatureType=U&ReloadGroups=false&MultiSelect=0?BcFilter=dsdsd")

    End Sub

    Private Sub ClearCombos()
        cmbState.Items.Clear()
        cmbBCFieldsValues1.Items.Clear()
        cmbBCFieldsValues2.Items.Clear()
        cmbBCFieldsValues3.Items.Clear()
        cmbBCFieldsValues4.Items.Clear()
        cmbBCFieldsValues5.Items.Clear()
        cmbBCCriteria1.Items.Clear()
        cmbBCCriteria2.Items.Clear()
        cmbBCCriteria3.Items.Clear()
        cmbBCCriteria4.Items.Clear()
        cmbBCCriteria5.Items.Clear()
    End Sub

    Private Sub LoadFilter()
        If (Session("TreeFilter") IsNot Nothing) Then
            Dim strSession = Session("TreeFilter").ToString()
            If (Not String.IsNullOrEmpty(strSession)) Then
                Dim strFilterValues = strSession.Split("|")(1)
                Dim oParameters As New ObjectCallbackRequest()
                oParameters = roJSONHelper.Deserialize(strFilterValues, oParameters.GetType())
                If (Not String.IsNullOrEmpty(oParameters.BcState)) Then
                    cmbState.Value = Integer.Parse(oParameters.BcState)
                Else
                    cmbState.SelectedIndex = -1
                End If
                txtBusinessCenterName.Text = If(Not String.IsNullOrEmpty(oParameters.BcName), oParameters.BcName, String.Empty)
                Dim countAux = 1
                For Each filter As ObjectCallbackRequestItem In oParameters.BcFieldFilters
                    Select Case countAux
                        Case 1
                            LoadFilterData(cmbBCFieldsValues1, cmbBCCriteria1, txtValue1, rblAndOr1, filter)
                        Case 2
                            LoadFilterData(cmbBCFieldsValues2, cmbBCCriteria2, txtValue2, rblAndOr2, filter)
                        Case 3
                            LoadFilterData(cmbBCFieldsValues3, cmbBCCriteria3, txtValue3, rblAndOr3, filter)
                        Case 4
                            LoadFilterData(cmbBCFieldsValues4, cmbBCCriteria4, txtValue4, rblAndOr4, filter)
                        Case 5
                            LoadFilterData(cmbBCFieldsValues5, cmbBCCriteria5, txtValue5, Nothing, filter)

                    End Select
                    countAux += 1
                Next
            End If
        End If
    End Sub

    Private Shared Sub LoadFilterData(cmbField As ASPxComboBox, cmbCriteria As ASPxComboBox, txtValue As ASPxTextBox, rblOperators As ASPxRadioButtonList, filterData As ObjectCallbackRequestItem)
        If (Not String.IsNullOrEmpty(filterData.FieldId)) Then
            cmbField.Value = Integer.Parse(filterData.FieldId)
        Else
            cmbField.SelectedIndex = -1
        End If
        If (Not String.IsNullOrEmpty(filterData.CriteriaContains)) Then
            cmbCriteria.Value = filterData.CriteriaContains
        Else
            cmbCriteria.SelectedIndex = -1
        End If
        If (Not String.IsNullOrEmpty(filterData.Condition)) Then
            rblOperators.Value = filterData.Condition
        Else
            If (rblOperators IsNot Nothing) Then rblOperators.SelectedIndex = -1
        End If
        txtValue.Text = If(Not String.IsNullOrEmpty(filterData.Value), filterData.Value, String.Empty)

    End Sub

End Class