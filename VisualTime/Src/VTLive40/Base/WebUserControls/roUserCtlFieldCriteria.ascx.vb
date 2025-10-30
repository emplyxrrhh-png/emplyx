Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.WebControls

Partial Class WebUserControls_roUserCtlFieldCriteria
    Inherits UserControlBase

    'Language Tags
    Private CriteriaEquals As String

    Private CriteriaDifferent As String
    Private CriteriaStartsWith As String
    Private CriteriaContains As String
    Private CriteriaNoContains As String
    Private CriteriaMajor As String
    Private CriteriaMajorOrEquals As String
    Private CriteriaMinor As String
    Private CriteriaMinorOrEquals As String
    Private CriteriaTheValue As String
    Private CriteriaTheDate As String
    Private CriteriaTheDateActual As String
    Private CriteriaTheDateOfJustification As String
    Private CriteriaTheTime As String
    Private CriteriaTheTimeActual As String
    Private CriteriaTheTimeOfJustification As String
    Private CriteriaTheValues As String
    Private CriteriaThePeriod As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            IsScriptManagerInParent()
            LoadCombos()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadCombos()
        Try
            'Criteris de busqueda
            '/----------------------------------------------------------------------
            CriteriaEquals = Me.Language.Translate("Criteria.Equals", Me.DefaultScope)
            CriteriaDifferent = Me.Language.Translate("Criteria.Different", Me.DefaultScope)
            CriteriaStartsWith = Me.Language.Translate("Criteria.StartWith", Me.DefaultScope)
            CriteriaContains = Me.Language.Translate("Criteria.Contains", Me.DefaultScope)
            CriteriaNoContains = Me.Language.Translate("Criteria.NoContains", Me.DefaultScope)

            CriteriaMajor = Me.Language.Translate("Criteria.Major", Me.DefaultScope)
            CriteriaMajorOrEquals = Me.Language.Translate("Criteria.MajorOrEquals", Me.DefaultScope)
            CriteriaMinor = Me.Language.Translate("Criteria.Minor", Me.DefaultScope)
            CriteriaMinorOrEquals = Me.Language.Translate("Criteria.MinorOrEquals", Me.DefaultScope)

            CriteriaTheValue = Me.Language.Translate("Criteria.TheValue", Me.DefaultScope)
            CriteriaTheDate = Me.Language.Translate("Criteria.TheDate", Me.DefaultScope)
            CriteriaTheDateOfJustification = Me.Language.Translate("Criteria.TheDateOfJustification", Me.DefaultScope)
            CriteriaTheTime = Me.Language.Translate("Criteria.TheTime", Me.DefaultScope)
            CriteriaTheTimeOfJustification = Me.Language.Translate("Criteria.TheTimeOfJustification", Me.DefaultScope)
            CriteriaTheValues = Me.Language.Translate("Criteria.TheValues", Me.DefaultScope)
            CriteriaThePeriod = Me.Language.Translate("Criteria.ThePeriod", Me.DefaultScope)

            cmbVisibilityCriteria1.ClearItems()
            Dim dtblUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
            Dim dRows() As DataRow = dtblUserFields.Select("", "FieldName")

            For Each dRow As DataRow In dRows
                If dRow("Used") Then
                    cmbVisibilityCriteria1.AddItem(dRow("FieldName"), dRow("FieldName") & "*|*" & dRow("FieldType"), "roUFC_chkCombosVisibility('" & Me.ClientID & "','" & cmbVisibilityCriteria1.ClientID & "'); hasChanges(true);")
                End If
            Next
        Catch e As Exception
            Response.Write(e.Message & " " & e.StackTrace)
        End Try
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roUserFieldCriteria", "~/Base/Scripts/roUserFieldCriteria.js", Me.Parent.Page)

        Return True
    End Function

    Public Sub loadValuesCriteria(ByVal oCondition As Object)
        Try
            'Readers
            CriteriaEquals = Me.Language.Keyword("Criteria.Equals")
            CriteriaDifferent = Me.Language.Keyword("Criteria.Different")
            CriteriaStartsWith = Me.Language.Keyword("Criteria.StartWith")
            CriteriaContains = Me.Language.Keyword("Criteria.Contains")
            CriteriaNoContains = Me.Language.Keyword("Criteria.NoContains")
            CriteriaMajor = Me.Language.Keyword("Criteria.Major")
            CriteriaMajorOrEquals = Me.Language.Keyword("Criteria.MajorOrEquals")
            CriteriaMinor = Me.Language.Keyword("Criteria.Minor")
            CriteriaMinorOrEquals = Me.Language.Keyword("Criteria.MinorOrEquals")
            CriteriaTheValue = Me.Language.Keyword("Criteria.TheValue")
            CriteriaTheDate = Me.Language.Keyword("Criteria.TheDate")
            CriteriaTheDateActual = Me.Language.Keyword("Criteria.TheDateActual")
            CriteriaTheTime = Me.Language.Keyword("Criteria.TheTime")
            CriteriaTheTimeActual = Me.Language.Keyword("Criteria.TheTimeActual")
            CriteriaTheValues = Me.Language.Keyword("Criteria.TheValues")
            CriteriaThePeriod = Me.Language.Keyword("Criteria.ThePeriod")

            'Controls
            Dim cmbVisibilityCriteria1 As roComboBox = Me.cmbVisibilityCriteria1
            Dim cmbVisibilityCriteria2 As roComboBox = Me.cmbVisibilityCriteria2
            Dim cmbVisibilityCriteria3 As roComboBox = Me.cmbVisibilityCriteria3
            'Texte valor
            Dim txtVisibilityValue As HtmlInputText = Me.txtVisibilityValue
            'Decimal valor
            Dim decVisibilityValue As HtmlInputText = Me.decVisibilityValue
            'Numeric valor
            Dim numVisibilityValue As HtmlInputText = Me.numVisibilityValue
            'Hora
            Dim mskVisibilityValueTime As HtmlInputText = Me.mskVisibilityValueTime
            'Data
            Dim mskVisibilityValue As HtmlInputText = Me.mskVisibilityValue
            'Combo valors
            Dim cmbVisibilityValue As roComboBox = Me.cmbVisibilityValue

            'Periodes de data
            Dim dtBegin As HtmlInputText = Me.dtBegin
            Dim dtEnd As HtmlInputText = Me.dtEnd

            'Periodes de hora
            Dim tBegin As HtmlInputText = Me.tBegin
            Dim tEnd As HtmlInputText = Me.tEnd

            Dim oPanelText As Panel = Me.panVValueTextBox
            Dim oPanelNumber As Panel = Me.panVValueNumericBox
            Dim oPanelDecimal As Panel = Me.panVValueDecimalBox
            Dim oPanelCombo As Panel = Me.panVValueComboBox
            Dim oPanelDate As Panel = Me.panVValueMaskTextBox
            Dim oPanelTime As Panel = Me.panVValueMaskTextBoxTime
            Dim oPanelPeriod As Panel = Me.panVValuePeriods
            Dim oPanelPeriodTime As Panel = Me.panVValueTimePeriods

            cmbVisibilityCriteria1.SelectedText = oCondition.UserField.FieldName
            cmbVisibilityCriteria1.SelectedValue = oCondition.UserField.FieldName & "*|*" & oCondition.UserField.FieldType
            'combo criteri 1
            Dim fieldTypeInt As Integer = roTypes.Any2Integer(oCondition.UserField.FieldType)
            Select Case fieldTypeInt
                Case FieldTypes.tText
                    cmbVisibilityCriteria2.ClearItems()
                    cmbVisibilityCriteria2.AddItem(CriteriaEquals, "Equals", "roUFC_checkCombo2('" & Me.ClientID & "','Equals'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaDifferent, "Different", "roUFC_checkCombo2('" & Me.ClientID & "','Different'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaStartsWith, "StartsWith", "roUFC_checkCombo2('" & Me.ClientID & "','StartsWith'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaContains, "Contains", "roUFC_checkCombo2('" & Me.ClientID & "','Contains'); hasChanges(true);")
                Case FieldTypes.tNumeric, FieldTypes.tDate, FieldTypes.tDecimal, FieldTypes.tTime 'Numeric (1), Data (2), Decimal (3), Hora (4)
                    cmbVisibilityCriteria2.ClearItems()
                    cmbVisibilityCriteria2.AddItem(CriteriaEquals, "Equals", "roUFC_checkCombo2('" & Me.ClientID & "','Equals'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaMajor, "Major", "roUFC_checkCombo2('" & Me.ClientID & "','Major'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaMajorOrEquals, "MajorOrEquals", "roUFC_checkCombo2('" & Me.ClientID & "','MajorOrEquals'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaMinor, "Minor", "roUFC_checkCombo2('" & Me.ClientID & "','Minor'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaMinorOrEquals, "MinorOrEquals", "roUFC_checkCombo2('" & Me.ClientID & "','MinorOrEquals'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaDifferent, "Different", "roUFC_checkCombo2('" & Me.ClientID & "','Different'); hasChanges(true);")
                Case FieldTypes.tList 'Llista de valors
                    cmbVisibilityCriteria2.ClearItems()
                    cmbVisibilityCriteria2.AddItem(CriteriaContains, "Contains", "roUFC_checkCombo2('" & Me.ClientID & "','Contains'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaNoContains, "NoContains", "roUFC_checkCombo2('" & Me.ClientID & "','NoContains'); hasChanges(true);")
                Case FieldTypes.tDatePeriod, FieldTypes.tTimePeriod  'Periodes de data / hora
                    cmbVisibilityCriteria2.ClearItems()
                    cmbVisibilityCriteria2.AddItem(CriteriaEquals, "Equals", "roUFC_checkCombo2('" & Me.ClientID & "','Equals'); hasChanges(true);")
                    cmbVisibilityCriteria2.AddItem(CriteriaContains, "Contains", "roUFC_checkCombo2('" & Me.ClientID & "','Contains'); hasChanges(true);")
            End Select

            Dim intConditionCompare As Integer = roTypes.Any2Integer(oCondition.Compare)
            Select Case intConditionCompare
                Case CompareType.Contains
                    cmbVisibilityCriteria2.SelectedValue = "Contains"
                Case CompareType.Distinct
                    cmbVisibilityCriteria2.SelectedValue = "Different"
                Case CompareType.EndWidth
                    cmbVisibilityCriteria2.SelectedValue = "EndWith"
                Case CompareType.Equal
                    cmbVisibilityCriteria2.SelectedValue = "Equals"
                Case CompareType.Major
                    cmbVisibilityCriteria2.SelectedValue = "Major"
                Case CompareType.MajorEqual
                    cmbVisibilityCriteria2.SelectedValue = "MajorOrEquals"
                Case CompareType.Minor
                    cmbVisibilityCriteria2.SelectedValue = "Minor"
                Case CompareType.MinorEqual
                    cmbVisibilityCriteria2.SelectedValue = "MinorOrEquals"
                Case CompareType.NotContains
                    cmbVisibilityCriteria2.SelectedValue = "NoContains"
                Case CompareType.StartWith
                    cmbVisibilityCriteria2.SelectedValue = "StartsWith"
            End Select

            'combo criteri 2
            Select Case fieldTypeInt
                Case FieldTypes.tText, FieldTypes.tNumeric, FieldTypes.tDecimal  'Texte (0), Numeric (1), Decimal (3)
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaTheValue, "TheValue", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheValue'); hasChanges(true);")
                Case FieldTypes.tDate 'Data (2)
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaTheDate, "TheDate", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheDate'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheDateActual, "TheDateActual", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheDateActual'); hasChanges(true);")
                Case FieldTypes.tTime  'Hora (4)
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaTheTime, "TheTime", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheTime'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheTimeActual, "TheTimeActual", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheTimeActual'); hasChanges(true);")
                Case FieldTypes.tList  'Llista de valors
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaTheValue, "TheValue", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheValue'); hasChanges(true);")
                Case FieldTypes.tDatePeriod  'Periodes de data
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaThePeriod, "ThePeriod", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','ThePeriod'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheDate, "TheDate", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheDate'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheDateActual, "TheDateActual", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheDateActual'); hasChanges(true);")
                Case FieldTypes.tTimePeriod 'Periodes de hora
                    cmbVisibilityCriteria3.ClearItems()
                    cmbVisibilityCriteria3.AddItem(CriteriaThePeriod, "ThePeriod", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','ThePeriod'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheTime, "TheTime", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheTime'); hasChanges(true);")
                    cmbVisibilityCriteria3.AddItem(CriteriaTheTimeActual, "TheTimeActual", "roUFC_checkCombo3('" & Me.ClientID & "','" & oCondition.UserField.FieldType & "','TheTimeActual'); hasChanges(true);")
            End Select

            If oCondition.ValueType = CompareValueType.DirectValue Then 'Si es un valor directe
                Select Case fieldTypeInt
                    Case FieldTypes.tText, FieldTypes.tNumeric, FieldTypes.tDecimal, FieldTypes.tList
                        cmbVisibilityCriteria3.SelectedValue = "TheValue"
                    Case FieldTypes.tDate
                        cmbVisibilityCriteria3.SelectedValue = "TheDate"
                    Case FieldTypes.tTime
                        cmbVisibilityCriteria3.SelectedValue = "TheTime"
                    Case FieldTypes.tDatePeriod
                        cmbVisibilityCriteria3.SelectedValue = "ThePeriod"
                    Case FieldTypes.tTimePeriod
                        cmbVisibilityCriteria3.SelectedValue = "ThePeriod"
                End Select
            Else
                Select Case fieldTypeInt
                    Case FieldTypes.tTime
                        cmbVisibilityCriteria3.SelectedValue = "TheTimeActual"
                    Case FieldTypes.tDate
                        cmbVisibilityCriteria3.SelectedValue = "TheDateActual"
                    Case FieldTypes.tDatePeriod
                        cmbVisibilityCriteria3.SelectedValue = "TheDateActual"
                    Case FieldTypes.tTimePeriod
                        cmbVisibilityCriteria3.SelectedValue = "TheTimeActual"
                End Select
            End If

            oPanelText.Style("display") = "none"
            oPanelNumber.Style("display") = "none"
            oPanelDecimal.Style("display") = "none"
            oPanelCombo.Style("display") = "none"
            oPanelDate.Style("display") = "none"
            oPanelTime.Style("display") = "none"
            oPanelPeriod.Style("display") = "none"
            oPanelPeriodTime.Style("display") = "none"

            Select Case fieldTypeInt
                Case FieldTypes.tText 'Texte (0)
                    oPanelText.Style("display") = ""
                    txtVisibilityValue.Value = roTypes.Any2String(oCondition.Value)
                Case FieldTypes.tNumeric 'Numeric
                    oPanelNumber.Style("display") = ""
                    numVisibilityValue.Value = roTypes.Any2String(oCondition.Value)
                Case FieldTypes.tDecimal 'Decimal
                    oPanelDecimal.Style("display") = ""
                    decVisibilityValue.Value = roTypes.Any2String(oCondition.Value)
                Case FieldTypes.tDate 'Data (2)
                    If cmbVisibilityCriteria3.SelectedValue = "TheDateOfJustification" Or cmbVisibilityCriteria3.SelectedValue = "TheDateActual" Then
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "TheDate" Then
                        oPanelDate.Style("display") = ""
                        mskVisibilityValue.Value = roTypes.Any2String(oCondition.Value)
                    End If
                Case FieldTypes.tTime  'Hora (4)
                    If cmbVisibilityCriteria3.SelectedValue = "TheTimeOfJustification" Or cmbVisibilityCriteria3.SelectedValue = "TheTimeActual" Then
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "TheTime" Then
                        oPanelTime.Style("display") = ""
                        mskVisibilityValueTime.Value = roTypes.Any2String(oCondition.Value)
                    End If
                Case FieldTypes.tList 'Llista de valors
                    oPanelCombo.Style("display") = ""
                    loadComboListValues(oCondition.UserField.FieldName, cmbVisibilityValue)
                    cmbVisibilityValue.SelectedValue = roTypes.Any2String(oCondition.Value)
                Case FieldTypes.tDatePeriod 'Periodes de data
                    If cmbVisibilityCriteria3.SelectedValue = "TheDateOfJustification" Or cmbVisibilityCriteria3.SelectedValue = "TheDateActual" Then
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "TheDate" Then
                        oPanelDate.Style("display") = ""
                        mskVisibilityValue.Value = roTypes.Any2String(oCondition.Value)
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "ThePeriod" Then
                        Dim arrDate() As String = roTypes.Any2String(oCondition.Value).Split("*")
                        If arrDate.Length = 1 Then
                            cmbVisibilityCriteria3.SelectedValue = "TheDate"
                            oPanelDate.Style("display") = ""
                            Dim aDate() As String = oCondition.Value.ToString.Split("/")
                            Dim dDate As Date = New Date(aDate(0), aDate(1), aDate(2))

                            mskVisibilityValue.Value = Format(dDate, HelperWeb.GetShortDateFormat)
                        Else
                            oPanelPeriod.Style("display") = ""
                            Dim strBegin As String = roTypes.Any2String(oCondition.Value).Split("*")(0)
                            Dim aBegin() As String = strBegin.Split("/")
                            Dim dBegin As Date = New Date(aBegin(0), aBegin(1), aBegin(2))

                            Dim strEnd As String = roTypes.Any2String(oCondition.Value).Split("*")(1)
                            Dim aEnd() As String = strEnd.Split("/")
                            Dim dEnd As Date = New Date(aEnd(0), aEnd(1), aEnd(2))

                            dtBegin.Value = Format(dBegin, HelperWeb.GetShortDateFormat)
                            dtEnd.Value = Format(dEnd, HelperWeb.GetShortDateFormat)
                        End If
                    End If
                Case FieldTypes.tTimePeriod 'Periodes de hora
                    If cmbVisibilityCriteria3.SelectedValue = "TheTimeOfJustification" Or cmbVisibilityCriteria3.SelectedValue = "TheTimeActual" Then
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "TheTime" Then
                        oPanelTime.Style("display") = ""
                        mskVisibilityValueTime.Value = roTypes.Any2String(oCondition.Value)
                    ElseIf cmbVisibilityCriteria3.SelectedValue = "ThePeriod" Then
                        Dim arrTime() As String = roTypes.Any2String(oCondition.Value).Split("*")
                        If arrTime.Length = 1 Then
                            cmbVisibilityCriteria3.SelectedValue = "TheTime"
                            oPanelTime.Style("display") = ""
                            mskVisibilityValueTime.Value = oCondition.Value
                        Else
                            oPanelPeriodTime.Style("display") = ""
                            tBegin.Value = arrTime(0)
                            tEnd.Value = arrTime(1)
                        End If
                    End If
            End Select
        Catch ex As Exception
            Response.Write(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Sub loadComboListValues(ByVal uField As String, ByRef cmb As Robotics.WebControls.roComboBox)
        Try
            Dim oUserField As roUserField
            oUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, uField, Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField, False, False)

            cmb.ClearItems()
            For Each strValue As String In oUserField.ListValues
                cmb.AddItem(strValue, strValue, "hasChanges(true);")
            Next
        Catch ex As Exception
        End Try
    End Sub

End Class