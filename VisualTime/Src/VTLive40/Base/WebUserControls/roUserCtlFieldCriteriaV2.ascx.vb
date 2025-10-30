Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserControls_roUserCtlFieldCriteriaV2
    Inherits UserControlBase

    'Language Tags
    Private _CriteriaEquals As String

    Private _CriteriaDifferent As String
    Private _CriteriaStartsWith As String
    Private _CriteriaContains As String
    Private _CriteriaNoContains As String
    Private _CriteriaMajor As String
    Private _CriteriaMajorOrEquals As String
    Private _CriteriaMinor As String
    Private _CriteriaMinorOrEquals As String
    Private _CriteriaTheValue As String
    Private _CriteriaTheDate As String
    Private _CriteriaTheDateActual As String
    Private _CriteriaTheDateOfJustification As String
    Private _CriteriaTheTime As String
    Private _CriteriaTheTimeActual As String
    Private _CriteriaTheTimeOfJustification As String
    Private _CriteriaTheValues As String
    Private _CriteriaThePeriod As String
    Private _prefix As String = ""
    Private _strUserFieldsTypeFilter As String = ""

    Public ReadOnly Property CriteriaEquals As String
        Get
            Return _CriteriaEquals
        End Get
    End Property

    Public ReadOnly Property CriteriaMinorOrEquals As String
        Get
            Return _CriteriaMinorOrEquals
        End Get
    End Property

    Public ReadOnly Property CriteriaTheValue As String
        Get
            Return _CriteriaTheValue
        End Get
    End Property

    Public ReadOnly Property CriteriaTheDate As String
        Get
            Return _CriteriaTheDate
        End Get
    End Property

    Public ReadOnly Property CriteriaTheDateActual As String
        Get
            Return _CriteriaTheDateActual
        End Get
    End Property

    Public ReadOnly Property CriteriaTheDateOfJustification As String
        Get
            Return _CriteriaTheDateOfJustification
        End Get
    End Property

    Public ReadOnly Property CriteriaTheTime As String
        Get
            Return _CriteriaTheTime
        End Get
    End Property

    Public ReadOnly Property CriteriaTheTimeActual As String
        Get
            Return _CriteriaTheTimeActual
        End Get
    End Property

    Public ReadOnly Property CriteriaTheTimeOfJustification As String
        Get
            Return _CriteriaTheTimeOfJustification
        End Get
    End Property

    Public ReadOnly Property CriteriaTheValues As String
        Get
            Return _CriteriaTheValues
        End Get
    End Property

    Public ReadOnly Property CriteriaThePeriod As String
        Get
            Return _CriteriaThePeriod
        End Get
    End Property

    Public ReadOnly Property CriteriaDifferent As String
        Get
            Return _CriteriaDifferent
        End Get
    End Property

    Public ReadOnly Property CriteriaStartsWith As String
        Get
            Return _CriteriaStartsWith
        End Get
    End Property

    Public ReadOnly Property CriteriaContains As String
        Get
            Return _CriteriaContains
        End Get
    End Property

    Public ReadOnly Property CriteriaNoContains As String
        Get
            Return _CriteriaNoContains
        End Get
    End Property

    Public ReadOnly Property CriteriaMajor As String
        Get
            Return _CriteriaMajor
        End Get
    End Property

    Public ReadOnly Property CriteriaMajorOrEquals As String
        Get
            Return _CriteriaMajorOrEquals
        End Get
    End Property

    Public ReadOnly Property CriteriaMinor As String
        Get
            Return _CriteriaMinor
        End Get
    End Property

    Public Property Prefix As String
        Get
            Return _prefix
        End Get
        Set(value As String)
            _prefix = value
        End Set
    End Property

    Public ReadOnly Property FieldName As String
        Get
            Try
                Dim value As String = roTypes.Any2String(Me.cmbCriteria1.SelectedItem.Value)

                value = value.Substring(0, value.IndexOf("*|*"))

                Return value
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property Type As Integer
        Get
            Try
                Dim value As String = roTypes.Any2String(Me.cmbCriteria1.SelectedItem.Value)

                value = value.Substring(value.IndexOf("*|*") + 3)

                Return roTypes.Any2Integer(value)
            Catch ex As Exception
                Return -1
            End Try
        End Get
    End Property

    Public ReadOnly Property criteria1 As String
        Get
            Try
                If Me.cmbCriteria2.Text <> String.Empty AndAlso Me.cmbCriteria2.Items.Count = 0 Then
                    Me.LoadCombo2(Type)
                    For Each item As DevExpress.Web.ListEditItem In Me.cmbCriteria2.Items
                        If item.Text = Me.cmbCriteria2.Text Then
                            Return item.Value
                        End If
                    Next
                End If

                Return Me.cmbCriteria2.Value
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property criteria2 As String
        Get
            Try
                If Me.cmbCriteria3.Text <> String.Empty AndAlso Me.cmbCriteria3.Items.Count = 0 Then
                    Me.LoadCombo3(Type)
                    For Each item As DevExpress.Web.ListEditItem In Me.cmbCriteria3.Items
                        If item.Text = Me.cmbCriteria3.Text Then
                            Return item.Value
                        End If
                    Next
                End If

                Return Me.cmbCriteria3.Value
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property OConditionValue As roUserFieldCondition
        Get
            Dim oCondition As New roUserFieldCondition

            oCondition.UserField = New roUserField
            oCondition.UserField.FieldName = Me.FieldName
            oCondition.UserField.FieldType = Me.Type

            Select Case Me.criteria1.ToUpper
                Case "EQUALS"
                    oCondition.Compare = CompareType.Equal
                Case "DIFFERENT"
                    oCondition.Compare = CompareType.Distinct
                Case "STARTSWITH"
                    oCondition.Compare = CompareType.StartWith
                Case "CONTAINS"
                    oCondition.Compare = CompareType.Contains
                Case "NOCONTAINS"
                    oCondition.Compare = CompareType.NotContains
                Case "MAJOR"
                    oCondition.Compare = CompareType.Major
                Case "MAJOROREQUALS"
                    oCondition.Compare = CompareType.MajorEqual
                Case "MINOR"
                    oCondition.Compare = CompareType.Minor
                Case "MINOROREQUALS"
                    oCondition.Compare = CompareType.MinorEqual
            End Select

            Select Case Me.criteria2.ToUpper
                Case "THETIMEACTUAL", "THEDATEACTUAL"
                    oCondition.ValueType = CompareValueType.CurrentDate
                Case Else
                    oCondition.ValueType = CompareValueType.DirectValue
            End Select

            oCondition.Value = FilterValue()
            Return oCondition
        End Get

    End Property

    Public Property FieldTypesFilter As String
        Get
            If Me._strUserFieldsTypeFilter = String.Empty Then
                Me._strUserFieldsTypeFilter = " Used <> 0"
            End If

            Return Me._strUserFieldsTypeFilter

        End Get
        Set(value As String)
            Me._strUserFieldsTypeFilter = "FieldType In(" & value & ") And Used <> 0"
            'LoadComboUserFields()
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Me.IsPostBack Then
                LoadCombos()
                SetClientIDs()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SetClientIDs()
        Me.cmbCriteria1.ClientInstanceName = Me.cmbCriteria1.ClientID & "Client"
        Me.cmbCriteria2.ClientInstanceName = Me.cmbCriteria2.ClientID & "Client"
        Me.cmbCriteria3.ClientInstanceName = Me.cmbCriteria3.ClientID & "Client"
        Me.cmbVisibilityValue.ClientInstanceName = Me.cmbVisibilityValue.ClientID & "Client"
    End Sub

    Private Sub LoadCombos()
        Try
            'Criteris de busqueda
            '/----------------------------------------------------------------------
            _CriteriaEquals = Me.Language.Keyword("Criteria.Equals")
            _CriteriaDifferent = Me.Language.Keyword("Criteria.Different")
            _CriteriaStartsWith = Me.Language.Keyword("Criteria.StartWith")
            _CriteriaContains = Me.Language.Keyword("Criteria.Contains")
            _CriteriaNoContains = Me.Language.Keyword("Criteria.NoContains")

            _CriteriaMajor = Me.Language.Keyword("Criteria.Major")
            _CriteriaMajorOrEquals = Me.Language.Keyword("Criteria.MajorOrEquals")
            _CriteriaMinor = Me.Language.Keyword("Criteria.Minor")
            _CriteriaMinorOrEquals = Me.Language.Keyword("Criteria.MinorOrEquals")

            _CriteriaTheValue = Me.Language.Keyword("Criteria.TheValue")
            _CriteriaTheDate = Me.Language.Keyword("Criteria.TheDate")
            _CriteriaTheDateOfJustification = Me.Language.Keyword("Criteria.TheDateOfJustification")
            _CriteriaTheTime = Me.Language.Keyword("Criteria.TheTime")
            _CriteriaTheTimeOfJustification = Me.Language.Keyword("Criteria.TheTimeOfJustification")
            _CriteriaTheValues = Me.Language.Keyword("Criteria.TheValues")
            _CriteriaThePeriod = Me.Language.Keyword("Criteria.ThePeriod")

            LoadComboUserFields()

            Me.cmbCriteria1.ClientSideEvents.SelectedIndexChanged = "function(s,e){chkComboField(s,e,'" & Me.Prefix & "');valueChanged();}"
            Me.cmbCriteria2.ClientSideEvents.SelectedIndexChanged = "function(s,e){checkCombo2(s,e,'" & Me.Prefix & "');valueChanged();}"
            Me.cmbCriteria3.ClientSideEvents.SelectedIndexChanged = "function(s,e){checkCombo3(s,e,true,'" & Me.Prefix & "');valueChanged();}"
            Me.cmbVisibilityValue.ClientSideEvents.SelectedIndexChanged = "function(s,e){selectFieldListValue(s,'" & Me.Prefix & "');valueChanged();}"
        Catch e As Exception
            Response.Write(e.Message & " " & e.StackTrace)
        End Try
    End Sub

    Private Sub LoadComboUserFields()

        'Obtener Campos de la ficha del empleado
        Dim dtblUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Nothing, Types.EmployeeField, FieldTypesFilter, False)
        Dim dRows() As DataRow = dtblUserFields.Select("", "FieldName")

        'Combo de la ficha de la pestaña Visibilidad
        Me.cmbCriteria1.Items.Clear()
        For Each dRow As DataRow In dRows
            Me.cmbCriteria1.Items.Add(dRow("FieldName"), dRow("FieldName") & "*|*" & dRow("FieldType"))
        Next
    End Sub

    Private Sub LoadCombo2(ByVal fieldType As FieldTypes)
        Me.cmbCriteria2.Items.Clear()

        If CInt(fieldType) = 0 Then
            Me.cmbCriteria2.Items.Add(Me.CriteriaEquals, "Equals")
            Me.cmbCriteria2.Items.Add(Me.CriteriaDifferent, "Different")
            Me.cmbCriteria2.Items.Add(Me.CriteriaStartsWith, "StartsWith")
            Me.cmbCriteria2.Items.Add(Me.CriteriaContains, "Contains")
        ElseIf CInt(fieldType) = 1 Or CInt(fieldType) = 2 Or CInt(fieldType) = 3 Or CInt(fieldType) = 4 Then
            Me.cmbCriteria2.Items.Add(Me.CriteriaEquals, "Equals")
            Me.cmbCriteria2.Items.Add(Me.CriteriaMajor, "Major")
            Me.cmbCriteria2.Items.Add(Me.CriteriaMajorOrEquals, "MajorOrEquals")
            Me.cmbCriteria2.Items.Add(Me.CriteriaMinor, "Minor")
            Me.cmbCriteria2.Items.Add(Me.CriteriaMinorOrEquals, "MinorOrEquals")
            Me.cmbCriteria2.Items.Add(Me.CriteriaDifferent, "Different")
        ElseIf CInt(fieldType) = 5 Then
            Me.cmbCriteria2.Items.Add(Me.CriteriaContains, "Contains")
            Me.cmbCriteria2.Items.Add(Me.CriteriaNoContains, "NoContains")
        ElseIf CInt(fieldType) = 6 Or CInt(fieldType) = 7 Then
            Me.cmbCriteria2.Items.Add(Me.CriteriaEquals, "Equals")
            Me.cmbCriteria2.Items.Add(Me.CriteriaContains, "Contains")
        End If

    End Sub

    Private Sub LoadCombo3(ByVal fieldType As FieldTypes)
        Me.cmbCriteria3.Items.Clear()

        If CInt(fieldType) = 0 Or CInt(fieldType) = 1 Or CInt(fieldType) = 3 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheValue, "TheValue")
        ElseIf CInt(fieldType) = 2 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheDate, "TheDate")
        ElseIf CInt(fieldType) = 4 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheTime, "TheTime")
        ElseIf CInt(fieldType) = 5 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheValue, "TheValue")
        ElseIf CInt(fieldType) = 6 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaThePeriod, "ThePeriod")
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheDate, "TheDate")
        ElseIf CInt(fieldType) = 7 Then
            Me.cmbCriteria3.Items.Add(Me.CriteriaThePeriod, "ThePeriod")
            Me.cmbCriteria3.Items.Add(Me.CriteriaTheTime, "TheTime")
        End If
    End Sub

    Public Function Validate() As Boolean
        Dim bReturn As Boolean = False

        If FieldName <> String.Empty AndAlso Type <> -1 AndAlso criteria1 <> String.Empty AndAlso criteria2 <> String.Empty Then
            bReturn = True
        End If

        Return bReturn
    End Function

    Public Function ClearFilterValue() As Boolean
        Try
            LoadCombos()
            Me.cmbCriteria1.SelectedItem = Nothing
            Me.cmbCriteria2.SelectedItem = Nothing
            Me.cmbCriteria3.SelectedItem = Nothing
            Me.cmbVisibilityValue.SelectedItem = Nothing

            Me.cmbCriteria2.Items.Clear()
            Me.cmbCriteria3.Items.Clear()
            Me.cmbVisibilityValue.Items.Clear()

            Me.panVValueComboBox.Style("display") = "none"
            Me.panVValueMaskTextBox.Style("display") = "none"
            Me.panVValueMaskTextBoxTime.Style("display") = "none"
            Me.panVValuePeriods.Style("display") = "none"
            Me.panVValueTextBox.Style("display") = "none"
            Me.panVValueTimePeriods.Style("display") = "none"

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function SetFilterValue(ByVal oFilter As roUserFieldCondition) As String
        Dim bResult As String = False

        Try
            If oFilter IsNot Nothing Then
                LoadCombos()
                Me.cmbCriteria1.SelectedItem = Me.cmbCriteria1.Items.FindByText(oFilter.UserField.FieldName)

                bResult = roTypes.Any2String(Me.cmbCriteria1.SelectedIndex) & ","

                Dim strCriteria2 As String = ""

                LoadCombo2(oFilter.UserField.FieldType)
                Select Case oFilter.Compare
                    Case CompareType.Contains
                        strCriteria2 = "Contains"
                    Case CompareType.Distinct
                        strCriteria2 = "Different"
                    Case CompareType.EndWidth
                        ' strCriteria2 ="Equals")
                    Case CompareType.Equal
                        strCriteria2 = "Equals"
                    Case CompareType.Major
                        strCriteria2 = "Major"
                    Case CompareType.MajorEqual
                        strCriteria2 = "MajorOrEquals"
                    Case CompareType.Minor
                        strCriteria2 = "Minor"
                    Case CompareType.MinorEqual
                        strCriteria2 = "MinorOrEquals"
                    Case CompareType.NotContains
                        strCriteria2 = "NoContains"
                    Case CompareType.StartWith
                        strCriteria2 = "StartsWith"
                End Select

                Me.cmbCriteria2.SelectedItem = Me.cmbCriteria2.Items.FindByValue(strCriteria2)

                bResult = bResult & roTypes.Any2String(Me.cmbCriteria2.SelectedIndex) & ","

                LoadCombo3(oFilter.UserField.FieldType)
                Dim strCriteria3 As String = ""

                If CInt(oFilter.UserField.FieldType) = 0 Or CInt(oFilter.UserField.FieldType) = 1 Or CInt(oFilter.UserField.FieldType) = 3 Then
                    strCriteria3 = "TheValue"
                ElseIf CInt(oFilter.UserField.FieldType) = 2 Then
                    strCriteria3 = "TheDate"
                ElseIf CInt(oFilter.UserField.FieldType) = 4 Then
                    strCriteria3 = "TheTime"
                ElseIf CInt(oFilter.UserField.FieldType) = 5 Then
                    strCriteria3 = "TheValue"
                ElseIf CInt(oFilter.UserField.FieldType) = 6 Then
                    Select Case oFilter.ValueType
                        Case CompareValueType.CurrentDate
                            strCriteria3 = "TheDate"
                        Case CompareValueType.DirectValue
                            strCriteria3 = "ThePeriod"
                    End Select
                ElseIf CInt(oFilter.UserField.FieldType) = 7 Then
                    Select Case oFilter.ValueType
                        Case CompareValueType.CurrentDate
                            strCriteria3 = "TheTime"
                        Case CompareValueType.DirectValue
                            strCriteria3 = "ThePeriod"
                    End Select
                End If

                Me.cmbCriteria3.SelectedItem = Me.cmbCriteria3.Items.FindByValue(strCriteria3)

                bResult = bResult & roTypes.Any2String(Me.cmbCriteria3.SelectedIndex) & ","

                Select Case oFilter.UserField.FieldType
                    Case FieldTypes.tText 'Texte (0)
                        panVValueTextBox.Style("display") = ""
                        Me.txtVisibilityValue.Text = oFilter.Value
                    Case FieldTypes.tNumeric 'Numeric
                        panVValueTextBox.Style("display") = ""
                        Me.txtVisibilityValue.Text = oFilter.Value
                    Case FieldTypes.tDecimal 'Decimal
                        panVValueTextBox.Style("display") = ""
                        Me.txtVisibilityValue.Text = oFilter.Value
                    Case FieldTypes.tDate 'Data (2)
                        If strCriteria3 = "TheDate" Then
                            panVValueMaskTextBox.Style("display") = ""
                            mskVisibilityValueDate.Date = CDate(oFilter.Value)
                        End If
                    Case FieldTypes.tTime  'Hora (4)
                        If strCriteria3 = "TheTime" Then
                            panVValueMaskTextBoxTime.Style("display") = ""
                            mskVisibilityValueTime.Text = oFilter.Value
                        End If
                    Case FieldTypes.tList 'Llista de valors
                        panVValueComboBox.Style("display") = ""
                        Dim oUserField As roUserField
                        oUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, oFilter.UserField.FieldName, Types.EmployeeField, False, False)
                        Me.cmbVisibilityValue.Value = Nothing
                        Me.cmbVisibilityValue.Items.Clear()
                        For Each strValue As String In oUserField.ListValues
                            Me.cmbVisibilityValue.Items.Add(strValue)
                        Next
                        cmbVisibilityValue.SelectedItem = cmbVisibilityValue.Items.FindByValue(oFilter.Value)

                        bResult = bResult & roTypes.Any2String(Me.cmbVisibilityValue.SelectedIndex)
                    Case FieldTypes.tDatePeriod 'Periodes de data
                        If strCriteria3 = "TheDateOfJustification" Or strCriteria3 = "TheDateActual" Then
                        ElseIf strCriteria3 = "TheDate" Then
                            panVValueMaskTextBox.Style("display") = ""
                            mskVisibilityValueDate.Date = CDate(oFilter.Value)
                        ElseIf strCriteria3 = "ThePeriod" Then
                            panVValuePeriods.Style("display") = ""
                            mskDatePeriod1.Date = CDate(oFilter.Value.Split("*")(0))
                            mskDatePeriod2.Date = CDate(oFilter.Value.Split("*")(1))
                        End If
                    Case FieldTypes.tTimePeriod 'Periodes de hora
                        If strCriteria3 = "TheTimeOfJustification" Or strCriteria3 = "TheTimeActual" Then
                        ElseIf strCriteria3 = "TheTime" Then
                            panVValueMaskTextBoxTime.Style("display") = ""
                            mskVisibilityValueTime.Text = oFilter.Value
                        ElseIf strCriteria3 = "ThePeriod" Then
                            panVValueTimePeriods.Style("display") = ""
                            mskTimePeriod1.Text = oFilter.Value.Split("*")(0)
                            mskTimePeriod2.Text = oFilter.Value.Split("*")(1)
                        End If
                End Select
            End If
        Catch ex As Exception
            bResult = ""
        End Try

        Return bResult
    End Function

    Public Function IsValidFilter() As Boolean
        Dim isValid As Boolean = True

        If FieldName = String.Empty Or Type = -1 Or criteria1 = String.Empty Or criteria2 = String.Empty Then
            isValid = False
        End If

        Return isValid
    End Function

    Private Function FilterValue() As String
        Dim _filterValue As String = ""

        Select Case Type
            Case FieldTypes.tText 'Texte (0)
                _filterValue = Me.txtVisibilityValue.Text
            Case FieldTypes.tNumeric 'Numeric
                _filterValue = Me.txtVisibilityValue.Text
            Case FieldTypes.tDecimal 'Decimal
                _filterValue = Me.txtVisibilityValue.Text
            Case FieldTypes.tDate 'Data (2)
                If criteria2 = "TheDate" Then
                    _filterValue = Format(mskVisibilityValueDate.Date, HelperWeb.GetShortDateFormat())
                End If
            Case FieldTypes.tTime  'Hora (4)
                If criteria2 = "TheTime" Then
                    _filterValue = Left(mskVisibilityValueTime.Text, 4) & ":" & Right(mskVisibilityValueTime.Text, 2)
                End If
            Case FieldTypes.tList 'Llista de valors
                _filterValue = Me.cmbVisibilityValue.Text
                'If Me.cmbCriteria1.Text <> String.Empty AndAlso Me.cmbVisibilityValue.Items.Count = 0 Then
                '    Dim oUserField As UserFieldService.roUserField
                '    oUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, Me.cmbCriteria1.Text, UserFieldService.Types.EmployeeField, False)
                '    Me.cmbVisibilityValue.Value = Nothing
                '    Me.cmbVisibilityValue.Items.Clear()
                '    For Each strValue As String In oUserField.ListValues
                '        Me.cmbVisibilityValue.Items.Add(strValue)
                '    Next
                'End If
                'For Each item As DevExpress.Web.ListEditItem In Me.cmbVisibilityValue.Items
                '    If item.Text = Me.cmbVisibilityValue.Text Then
                '        _filterValue = item.Value
                '    End If
                'Next
            Case FieldTypes.tDatePeriod 'Periodes de data
                If criteria2 = "TheDateOfJustification" Or criteria2 = "TheDateActual" Then
                ElseIf criteria2 = "TheDate" Then
                    _filterValue = Format(mskVisibilityValueDate.Date, HelperWeb.GetShortDateFormat())
                ElseIf criteria2 = "ThePeriod" Then
                    _filterValue = Format(mskDatePeriod1.Date, HelperWeb.GetShortDateFormat()) & "*" & Format(mskDatePeriod2.Date, HelperWeb.GetShortDateFormat())
                End If
            Case FieldTypes.tTimePeriod 'Periodes de hora
                If criteria2 = "TheTimeOfJustification" Or criteria2 = "TheTimeActual" Then
                ElseIf criteria2 = "TheTime" Then
                    _filterValue = mskVisibilityValueTime.Text
                ElseIf criteria2 = "ThePeriod" Then
                    _filterValue = mskTimePeriod1.Text & "*" & mskTimePeriod2.Text
                End If
        End Select

        Return _filterValue
    End Function

    Protected Sub cmbVisibilityValue_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles cmbVisibilityValue.Callback

        If e.Parameter <> String.Empty AndAlso e.Parameter.StartsWith("LOADFIELDS") Then
            Dim parameters As String() = e.Parameter.Split("#")

            Dim oUserField As roUserField
            oUserField = API.UserFieldServiceMethods.GetUserField(Me.Page, parameters(1), Types.EmployeeField, False, False)

            Dim strOutput As String = ""

            Me.cmbVisibilityValue.Value = Nothing
            Me.cmbVisibilityValue.Items.Clear()
            For Each strValue As String In oUserField.ListValues
                Me.cmbVisibilityValue.Items.Add(strValue)
            Next

        End If

    End Sub

End Class