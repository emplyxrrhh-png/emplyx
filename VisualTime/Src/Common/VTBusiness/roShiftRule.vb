Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Shift

    <DataContract()>
    Public Class roShiftRule

#Region "Declarations - Constructor"

        Private oState As roShiftState

        Private intIDShift As Integer
        Private intID As Integer

        Private oType As ShiftRuleType

        Private intIDIncidence As Integer   'Incidence
        Private intIDZone As Integer        'Zone
        Private eConditionValueType As eShiftRuleValueType
        Private xFromTime As Nullable(Of DateTime) 'FromTime
        Private xToTime As Nullable(Of DateTime) 'ToTime
        Private oFromValueUserField As VTUserFields.UserFields.roUserField = Nothing
        Private oToValueUserField As VTUserFields.UserFields.roUserField = Nothing
        Private oBetweenValueUserField As VTUserFields.UserFields.roUserField = Nothing

        Private intIDCause As Integer       'Cause
        Private eActionValueType As eShiftRuleValueType
        Private xMaxTime As Nullable(Of DateTime)  'MaxTime
        Private oMaxValueUserField As VTUserFields.UserFields.roUserField = Nothing
        Private sAux As String = String.Empty

        Public Sub New()
            Me.oState = New roShiftState
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _ID As Integer, ByVal _State As roShiftState)
            Me.oState = _State
            Me.intIDShift = _IDShift
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _ID As Integer, ByVal strDefinitionXml As String, ByVal _State As roShiftState)
            Me.oState = _State
            Me.intIDShift = _IDShift
            Me.intID = _ID
            Me.LoadFromXml(strDefinitionXml)
        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _ID As Integer, ByVal ruleType As ShiftRuleType, ByVal strDefinitionXml As String, ByVal _State As roShiftState)
            Me.oState = _State
            Me.intIDShift = _IDShift
            Me.intID = _ID
            Me.Type = ruleType
            Me.LoadFromXml(strDefinitionXml)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roShiftState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roShiftState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDShift() As Integer
            Get
                Return Me.intIDShift
            End Get
            Set(ByVal value As Integer)
                Me.intIDShift = value
            End Set
        End Property
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property
        <DataMember()>
        Public Property Type() As ShiftRuleType
            Get
                Return Me.oType
            End Get
            Set(ByVal value As ShiftRuleType)
                Me.oType = value
            End Set
        End Property
        <DataMember()>
        Public Property IDIncidence() As Integer
            Get
                Return Me.intIDIncidence
            End Get
            Set(ByVal value As Integer)
                Me.intIDIncidence = value
            End Set
        End Property
        <DataMember()>
        Public Property IDZone() As Integer
            Get
                Return Me.intIDZone
            End Get
            Set(ByVal value As Integer)
                Me.intIDZone = value
            End Set
        End Property
        <DataMember()>
        Public Property ConditionValueType() As eShiftRuleValueType
            Get
                Return Me.eConditionValueType
            End Get
            Set(ByVal value As eShiftRuleValueType)
                Me.eConditionValueType = value
            End Set
        End Property
        <DataMember()>
        Public Property FromTime() As Nullable(Of DateTime)
            Get
                Return Me.xFromTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xFromTime = value
            End Set
        End Property
        <DataMember()>
        Public Property ToTime() As Nullable(Of DateTime)
            Get
                Return Me.xToTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xToTime = value
            End Set
        End Property
        <DataMember()>
        Public Property FromValueUserFieldName() As String
            Get
                If Me.oFromValueUserField IsNot Nothing Then
                    Return Me.oFromValueUserField.FieldName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    Me.oFromValueUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, value, Types.EmployeeField, False)
                Else
                    Me.oFromValueUserField = Nothing
                End If
            End Set
        End Property
        <DataMember()>
        Public Property ToValueUserFieldName() As String
            Get
                If Me.oToValueUserField IsNot Nothing Then
                    Return Me.oToValueUserField.FieldName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    Me.oToValueUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, value, Types.EmployeeField, False)
                Else
                    Me.oToValueUserField = Nothing
                End If
            End Set
        End Property
        <DataMember()>
        Public Property BetweenValueUserFieldName() As String
            Get
                If Me.oBetweenValueUserField IsNot Nothing Then
                    Return Me.oBetweenValueUserField.FieldName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    Me.oBetweenValueUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, value, Types.EmployeeField, False)
                Else
                    Me.oBetweenValueUserField = Nothing
                End If
            End Set
        End Property
        <DataMember()>
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property
        <DataMember()>
        Public Property ActionValueType() As eShiftRuleValueType
            Get
                Return Me.eActionValueType
            End Get
            Set(ByVal value As eShiftRuleValueType)
                Me.eActionValueType = value
            End Set
        End Property
        <DataMember()>
        Public Property MaxTime() As Nullable(Of DateTime)
            Get
                Return Me.xMaxTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xMaxTime = value
            End Set
        End Property
        <DataMember()>
        Public Property MaxValueUserFieldName() As String
            Get
                If Me.oMaxValueUserField IsNot Nothing Then
                    Return Me.oMaxValueUserField.FieldName
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    Me.oMaxValueUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, value, Types.EmployeeField, False)
                Else
                    Me.oMaxValueUserField = Nothing
                End If
            End Set
        End Property
        <DataMember()>
        Public Property Aux() As String
            Get
                Return Me.sAux
            End Get
            Set(ByVal value As String)
                Me.sAux = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & Me.intIDShift.ToString & " AND ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.oType = Any2Integer(oRow("RuleType"))

                    Dim strXml As String = Any2String(oRow("Definition"))

                    If strXml <> "" Then
                        ' Añadimos la composición a la colección
                        Me.LoadFromXml(strXml)
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShiftRule, "", tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftRule::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftRule::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then

                ' Añadimos la composición a la colección
                Dim oDefinition As New roCollection(strXml)

                If oDefinition.Exists("Incidence") Then
                    Me.intIDIncidence = oDefinition("Incidence")
                End If

                If oDefinition.Exists("Zone") Then
                    Me.intIDZone = oDefinition("Zone")
                End If

                Me.eConditionValueType = eShiftRuleValueType.DirectValue
                If oDefinition.Exists("ConditionValueType") Then
                    Me.eConditionValueType = oDefinition("ConditionValueType")
                End If

                If oDefinition.Exists("FromTime") Then
                    Me.xFromTime = CDate(Any2Time(CDate(oDefinition("FromTime"))).Value)
                End If
                If oDefinition.Exists("ToTime") Then
                    Me.xToTime = CDate(Any2Time(CDate(oDefinition("ToTime"))).Value)
                End If

                Me.oFromValueUserField = Nothing
                If oDefinition.Exists("FromValueUserField") AndAlso oDefinition("FromValueUserField") <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Dim oUserField As New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("FromValueUserField"), Types.EmployeeField, False, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oFromValueUserField = oUserField
                    End If
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)
                End If
                Me.oToValueUserField = Nothing
                If oDefinition.Exists("ToValueUserField") AndAlso oDefinition("ToValueUserField") <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Dim oUserField As New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("ToValueUserField"), Types.EmployeeField, False, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oToValueUserField = oUserField
                    End If
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)
                End If
                Me.oBetweenValueUserField = Nothing
                If oDefinition.Exists("BetweenValueUserField") AndAlso oDefinition("BetweenValueUserField") <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Dim oUserField As New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("BetweenValueUserField"), Types.EmployeeField, False, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oBetweenValueUserField = oUserField
                    End If
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)
                End If

                If oDefinition.Exists("Cause") Then
                    Me.intIDCause = oDefinition("Cause")
                End If

                Me.eActionValueType = eShiftRuleValueType.DirectValue
                If oDefinition.Exists("ActionValueType") Then
                    Me.eActionValueType = oDefinition("ActionValueType")
                End If

                If oDefinition.Exists("MaxTime") Then
                    Me.xMaxTime = CDate(Any2Time(CDate(oDefinition("MaxTime"))).Value)
                End If

                Me.oMaxValueUserField = Nothing
                If oDefinition.Exists("MaxValueUserField") AndAlso oDefinition("MaxValueUserField") <> "" Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Dim oUserField As New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("MaxValueUserField"), Types.EmployeeField, False, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oMaxValueUserField = oUserField
                    End If
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)
                End If

            End If

        End Sub

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try
                If Me.intIDIncidence <= 0 Then
                    Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidIncidence
                    bolRet = False
                End If

                'If bolRet AndAlso Me.intIDZone <> 0 Then
                '    Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidZone
                '    bolRet = False
                'End If

                If bolRet AndAlso Me.intIDCause = 0 Then
                    Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidCause
                    bolRet = False
                End If

                If bolRet And Me.eConditionValueType = eShiftRuleValueType.UserFieldValue And Me.oFromValueUserField Is Nothing And Me.oToValueUserField Is Nothing And Me.oBetweenValueUserField Is Nothing Then
                    Me.oState.Result = ShiftResultEnum.ShiftRule_ConditionUserFieldRequired
                    bolRet = False
                End If
                If bolRet And Me.eConditionValueType = eShiftRuleValueType.UserFieldValue Then
                    If (Me.oFromValueUserField IsNot Nothing Or Me.oToValueUserField IsNot Nothing) And Me.oBetweenValueUserField IsNot Nothing Then
                        Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidConditionUserFields
                        bolRet = False
                    End If
                End If
                If bolRet And Me.eConditionValueType = eShiftRuleValueType.UserFieldValue Then
                    If Me.oFromValueUserField IsNot Nothing Then
                        bolRet = (Me.oFromValueUserField.FieldType = FieldTypes.tTime)
                        If Not bolRet Then Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidFromUserFieldType
                    End If
                    If bolRet And Me.oToValueUserField IsNot Nothing Then
                        bolRet = (Me.oToValueUserField.FieldType = FieldTypes.tTime)
                        If Not bolRet Then Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidToUserFieldType
                    End If
                    If bolRet And Me.oBetweenValueUserField IsNot Nothing Then
                        bolRet = (Me.oBetweenValueUserField.FieldType = FieldTypes.tTimePeriod)
                        If Not bolRet Then Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidBetweenUserFieldType
                    End If
                End If

                If bolRet And Me.eActionValueType = eShiftRuleValueType.UserFieldValue And Me.oMaxValueUserField Is Nothing Then
                    Me.oState.Result = ShiftResultEnum.ShiftRule_ActionUserFieldRequired
                    bolRet = False
                End If
                If bolRet And Me.eActionValueType = eShiftRuleValueType.UserFieldValue And Me.oMaxValueUserField IsNot Nothing Then
                    bolRet = (Me.oMaxValueUserField.FieldType = FieldTypes.tTime)
                    If Not bolRet Then Me.oState.Result = ShiftResultEnum.ShiftRule_InvalidActionUserFieldType
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftRule::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftRule::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("sysroShiftsCausesRules")
                    Dim strSQL As String = "@SELECT# * FROM sysroShiftsCausesRules " &
                                           "WHERE IDShift = " & Me.intIDShift.ToString & " AND ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDShift") = Me.intIDShift
                        If Me.intID <= 0 Then
                            Me.intID = Me.GetNextID()
                        End If
                        oRow("ID") = Me.intID
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("RuleType") = Me.oType
                    oRow("Definition") = Me.GetXml()

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String = ""
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tShiftRule, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftRule::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftRule::Save")
            End Try

            Return bolRet

        End Function

        Public Function GetXml() As String

            Dim oDefinition As New roCollection

            oDefinition.Add("Incidence", Me.intIDIncidence)
            oDefinition.Add("Zone", Me.intIDZone)
            oDefinition.Add("ConditionValueType", Me.eConditionValueType)
            If Me.xFromTime.HasValue Then
                oDefinition.Add("FromTime", Any2Time(Me.xFromTime.Value).TimeOnly)
            End If
            If Me.xToTime.HasValue Then
                oDefinition.Add("ToTime", Any2Time(Me.xToTime.Value).TimeOnly)
            End If
            If Me.oFromValueUserField IsNot Nothing Then
                oDefinition.Add("FromValueUserField", Me.oFromValueUserField.FieldName)
            Else
                oDefinition.Add("FromValueUserField", "")
            End If
            If Me.oToValueUserField IsNot Nothing Then
                oDefinition.Add("ToValueUserField", Me.oToValueUserField.FieldName)
            Else
                oDefinition.Add("ToValueUserField", "")
            End If
            If Me.oBetweenValueUserField IsNot Nothing Then
                oDefinition.Add("BetweenValueUserField", Me.oBetweenValueUserField.FieldName)
            Else
                oDefinition.Add("BetweenValueUserField", "")
            End If

            oDefinition.Add("Cause", Me.intIDCause)
            oDefinition.Add("ActionValueType", Me.eActionValueType)
            If Me.xMaxTime.HasValue Then
                oDefinition.Add("MaxTime", Any2Time(Me.xMaxTime.Value).TimeOnly)
            End If
            If Me.oMaxValueUserField IsNot Nothing Then
                oDefinition.Add("MaxValueUserField", Me.oMaxValueUserField.FieldName)
            Else
                oDefinition.Add("MaxValueUserField", "")
            End If

            Return oDefinition.XML

        End Function

        Public Function GetNextID() As Integer

            Dim intRet As Integer = 1

            Try

                Dim strSQL As String = "@SELECT# MAX(ID) FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & Me.intIDShift.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    intRet = Any2Integer(tb.Rows(0).Item(0)) + 1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftRule::GetNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftRule::GetNextID")
            End Try

            Return intRet

        End Function

        Public Function Description() As String
            Dim sText As String = ""

            Try

                'Incidencia
                If Me.intIDIncidence Then
                    sText &= Me.State.Language.Translate("docShift.ParseRule.IifThereIsA", "") & " " & Me.State.Language.Keyword("Incidence." & Me.IDIncidence) & " "
                End If

                'Zona
                If Me.intIDZone = -1 Then ' Es cualquier zona
                    sText &= Me.State.Language.Translate("docShift.ParseRule.InAnyTimeZone", "") & " "
                Else
                    sText &= Me.State.Language.Translate("docShift.ParseRule.In", "") & " " & ExecuteScalar("@SELECT# Name from TimeZones WHERE ID = " & Me.intIDZone) & " "
                End If

                Dim oParams As New ArrayList
                Dim strMessageKey As String = ""
                Select Case Me.eConditionValueType
                    Case eShiftRuleValueType.DirectValue

                        If Me.FromTime.HasValue And Me.ToTime.HasValue Then
                            If FromTime.Value.Hour = 0 And FromTime.Value.Minute = 0 Then
                                If ToTime.Value.Hour = 23 And ToTime.Value.Minute = 59 Then
                                    strMessageKey = "docShift.ParseRule.OfAnyDuration"
                                    'sText &= Me.State.Language.Translate("docShift.ParseRule.OfAnyDuration", "") & " "
                                Else
                                    oParams.Add(Format(ToTime.Value, "HH:mm"))
                                    strMessageKey = "docShift.ParseRule.LesserOrEqualThan"
                                    'sText &= Me.State.Language.Translate("docShift.ParseRule.LesserOrEqualThan", "") & " " & Format(ToTime.Value, "HH:mm") & " "
                                End If
                            Else
                                If ToTime.Value.Hour = 23 And ToTime.Value.Minute = 59 Then
                                    oParams.Add(Format(FromTime.Value, "HH:mm"))
                                    strMessageKey = "docShift.ParseRule.GraterOrEquelThan"
                                    'sText &= Me.State.Language.Translate("docShift.ParseRule.GraterOrEquelThan", "") & " " & Format(FromTime.Value, "HH:mm") & " "
                                Else
                                    oParams.Add(Format(FromTime.Value, "HH:mm:"))
                                    oParams.Add(Format(ToTime.Value, "HH:mm"))
                                    strMessageKey = "docShift.ParseRule.Between"
                                    'sText &= Me.State.Language.Translate("docShift.ParseRule.Between", "") & " " & Format(FromTime.Value, "HH:mm:") & " " & Me.State.Language.Translate("docShift.ParseRule.And", "") & " " & Format(ToTime.Value, "HH:mm") & " "
                                End If
                            End If
                        End If

                    Case eShiftRuleValueType.UserFieldValue

                        If Me.oFromValueUserField IsNot Nothing Or Me.oToValueUserField IsNot Nothing Then
                            If Me.oFromValueUserField Is Nothing Then
                                oParams.Add(Me.oToValueUserField.FieldName)
                                strMessageKey = "docShift.ParseRule.UserField.LesserOrEqualThan"
                            Else
                                oParams.Add(Me.oFromValueUserField.FieldName)
                                strMessageKey = "docShift.ParseRule.UserField.GraterOrEquelThan"
                            End If
                        ElseIf Me.oBetweenValueUserField IsNot Nothing Then
                            oParams.Add(Me.oBetweenValueUserField.FieldName)
                            strMessageKey = "docShift.ParseRule.UserField.Between"
                        End If

                End Select
                If strMessageKey <> "" Then
                    Me.oState.Language.ClearUserTokens()
                    If oParams IsNot Nothing Then
                        For i As Integer = 0 To oParams.Count - 1
                            Me.oState.Language.AddUserToken(oParams(i))
                        Next
                    End If
                    sText &= Me.oState.Language.Translate(strMessageKey, "") & " "
                End If

                sText &= Me.State.Language.Translate("docShift.ParseRule.InWillBeConsideredAs", "") & " " & ExecuteScalar("@SELECT# Name from Causes WHERE ID = " & Me.intIDCause) & " "

                oParams = New ArrayList
                strMessageKey = ""
                Select Case Me.eActionValueType

                    Case eShiftRuleValueType.DirectValue

                        If Me.MaxTime.HasValue Then
                            If Me.MaxTime.Value.Hour = 23 And Me.MaxTime.Value.Minute = 59 Then
                                strMessageKey = "docShift.ParseRule.AllTheTime"
                                'sText &= Me.State.Language.Translate("docShift.ParseRule.AllTheTime", "")
                            Else
                                oParams.Add(Format(MaxTime.Value, "HH:mm"))
                                strMessageKey = "docShift.ParseRule.Since"
                                'sText &= Me.State.Language.Translate("docShift.ParseRule.Since", "") & " " & Format(MaxTime.Value, "HH:mm")
                            End If
                        End If

                    Case eShiftRuleValueType.UserFieldValue

                        If Me.oMaxValueUserField IsNot Nothing Then
                            oParams.Add(Me.oMaxValueUserField.FieldName)
                            strMessageKey = "docShift.ParseRule.UserField.Since"
                        End If

                End Select
                If strMessageKey <> "" Then
                    Me.oState.Language.ClearUserTokens()
                    If oParams IsNot Nothing Then
                        For i As Integer = 0 To oParams.Count - 1
                            Me.oState.Language.AddUserToken(oParams(i))
                        Next
                    End If
                    sText &= Me.oState.Language.Translate(strMessageKey, "") & " "
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftRule::Description")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftRule::Description")
            Finally

            End Try

            Return sText.Trim()

        End Function

#Region "Helper methods"

        Public Shared Function LoadDailyShiftRule(ByVal _IDShift As Integer, ByVal _ID As Integer, strXml As String, ByVal _State As roShiftState) As roShiftDailyRule

            Dim oRet As New roShiftDailyRule

            Try

                oRet.IDShift = _IDShift
                oRet.ID = _ID

                If strXml <> "" Then

                    ' Añadimos la composición a la colección
                    Dim oDefinition As New roCollection(strXml)

                    If oDefinition.Exists("Name") Then
                        oRet.Name = oDefinition("Name")
                    End If

                    If oDefinition.Exists("Description") Then
                        oRet.Description = oDefinition("Description")
                    End If

                    If oDefinition.Exists("DayValidationRule") Then
                        oRet.DayValidationRule = Any2Integer(oDefinition("DayValidationRule"))
                    End If

                    If oDefinition.Exists("PreviousShiftValidationRule") Then
                        oRet.PreviousShiftValidationRule = roTypes.Any2String(oDefinition("PreviousShiftValidationRule")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                    End If

                    If oDefinition.Exists("ApplyScheduleValidationRule") Then
                        oRet.ApplyScheduleValidationRule = Any2Integer(oDefinition("ApplyScheduleValidationRule"))
                    End If

                    If oDefinition.Exists("ScheduleRulesValidationRule") Then
                        oRet.ScheduleRulesValidationRule = roTypes.Any2String(oDefinition("ScheduleRulesValidationRule")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                    End If

                    ' Agregamos todas las condiciones
                    If oDefinition.Exists("TotalConditions") Then
                        Dim dblTotalConditions As Double = oDefinition("TotalConditions")
                        For i As Integer = 1 To dblTotalConditions
                            Dim oShiftDailyRuleCondition As New roShiftDailyRuleCondition

                            If oDefinition.Exists("ConditionCauses_" & i.ToString) Then
                                Dim strConditionCause As String = oDefinition("ConditionCauses_" & i.ToString)
                                If strConditionCause.Length > 0 Then
                                    For Each strCause As String In strConditionCause.Split(",")
                                        Dim oShiftDailyRuleConditionCause As New roShiftDailyRuleConditionCause
                                        oShiftDailyRuleConditionCause.IDCause = Any2Double(strCause.Split("_")(0))
                                        oShiftDailyRuleConditionCause.Operation = Any2Integer(strCause.Split("_")(1))
                                        oShiftDailyRuleConditionCause.Name = Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleConditionCause.IDCause))
                                        oShiftDailyRuleCondition.ConditionCauses.Add(oShiftDailyRuleConditionCause)
                                    Next
                                End If
                            End If

                            If oDefinition.Exists("ConditionTimeZones_" & i.ToString) Then
                                Dim strConditionZone As String = oDefinition("ConditionTimeZones_" & i.ToString)
                                If strConditionZone.Length > 0 Then
                                    For Each iZones As Integer In strConditionZone.Split(",")
                                        Dim oShiftDailyRuleConditionTimeZone As New roShiftDailyRuleConditionTimeZone
                                        oShiftDailyRuleConditionTimeZone.IDTimeZone = iZones
                                        If oShiftDailyRuleConditionTimeZone.IDTimeZone <> -1 Then
                                            oShiftDailyRuleConditionTimeZone.Name = Any2String(ExecuteScalar("@SELECT# Name from TimeZones where id=" & oShiftDailyRuleConditionTimeZone.IDTimeZone))
                                        Else
                                            oShiftDailyRuleConditionTimeZone.Name = _State.Language.Translate("CRUFLCOM.Shifts.All", "")
                                        End If

                                        oShiftDailyRuleCondition.ConditionTimeZones.Add(oShiftDailyRuleConditionTimeZone)
                                    Next
                                End If
                            End If

                            If oDefinition.Exists("Compare_" & i.ToString) Then
                                oShiftDailyRuleCondition.Compare = Any2Integer(oDefinition("Compare_" & i.ToString))
                            End If

                            If oDefinition.Exists("Type_" & i.ToString) Then
                                oShiftDailyRuleCondition.Type = Any2Integer(oDefinition("Type_" & i.ToString))
                            End If

                            If oDefinition.Exists("FromValue_" & i.ToString) Then
                                oShiftDailyRuleCondition.FromValue = Any2String(oDefinition("FromValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("ToValue_" & i.ToString) Then
                                oShiftDailyRuleCondition.ToValue = Any2String(oDefinition("ToValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("UserField_" & i.ToString) Then
                                oShiftDailyRuleCondition.UserField = Any2String(oDefinition("UserField_" & i.ToString))
                            End If

                            If oDefinition.Exists("CompareCauses_" & i.ToString) Then
                                Dim strConditionCause As String = oDefinition("CompareCauses_" & i.ToString)
                                If strConditionCause.Length > 0 Then
                                    For Each strCause As String In strConditionCause.Split(",")
                                        Dim oShiftDailyRuleConditionCause As New roShiftDailyRuleConditionCause
                                        oShiftDailyRuleConditionCause.IDCause = Any2Double(strCause.Split("_")(0))
                                        oShiftDailyRuleConditionCause.Operation = Any2Integer(strCause.Split("_")(1))
                                        oShiftDailyRuleConditionCause.Name = Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleConditionCause.IDCause))
                                        oShiftDailyRuleCondition.CompareCauses.Add(oShiftDailyRuleConditionCause)
                                    Next
                                End If
                            End If

                            If oDefinition.Exists("CompareTimeZones_" & i.ToString) Then
                                Dim strConditionZone As String = oDefinition("CompareTimeZones_" & i.ToString)
                                If strConditionZone.Length > 0 Then
                                    For Each iZones As Integer In strConditionZone.Split(",")
                                        Dim oShiftDailyRuleConditionTimeZone As New roShiftDailyRuleConditionTimeZone
                                        oShiftDailyRuleConditionTimeZone.IDTimeZone = iZones
                                        If oShiftDailyRuleConditionTimeZone.IDTimeZone <> -1 Then
                                            oShiftDailyRuleConditionTimeZone.Name = Any2String(ExecuteScalar("@SELECT# Name from TimeZones where id=" & oShiftDailyRuleConditionTimeZone.IDTimeZone))
                                        Else
                                            oShiftDailyRuleConditionTimeZone.Name = _State.Language.Translate("CRUFLCOM.Shifts.All", "")
                                        End If

                                        oShiftDailyRuleCondition.CompareTimeZones.Add(oShiftDailyRuleConditionTimeZone)
                                    Next
                                End If
                            End If

                            ' Añadimos la condicion
                            oRet.Conditions.Add(oShiftDailyRuleCondition)

                        Next
                    End If

                    ' Agregamos todas las acciones
                    If oDefinition.Exists("TotalActions") Then
                        Dim dblTotalActions As Double = oDefinition("TotalActions")
                        For i As Integer = 1 To dblTotalActions
                            Dim oShiftDailyRuleAction As New roShiftDailyRuleAction
                            If oDefinition.Exists("Action_" & i.ToString) Then
                                oShiftDailyRuleAction.Action = Any2Integer(oDefinition("Action_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverAction_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverAction = Any2Integer(oDefinition("CarryOverAction_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverDirectValue_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverDirectValue = Any2String(oDefinition("CarryOverDirectValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverUserFieldValue_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverUserFieldValue = Any2String(oDefinition("CarryOverUserFieldValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverConditionPart_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverConditionPart = Any2Integer(oDefinition("CarryOverConditionPart_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverConditionNumber_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverConditionNumber = Any2Integer(oDefinition("CarryOverConditionNumber_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverActionResult_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverActionResult = Any2Integer(oDefinition("CarryOverActionResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverDirectValueResult_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverDirectValueResult = Any2String(oDefinition("CarryOverDirectValueResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverUserFieldValueResult_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverUserFieldValueResult = Any2String(oDefinition("CarryOverUserFieldValueResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverConditionPartResult_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverConditionPartResult = Any2Integer(oDefinition("CarryOverConditionPartResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverConditionNumberResult_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverConditionNumberResult = Any2Integer(oDefinition("CarryOverConditionNumberResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverIDCauseFrom_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverIDCauseFrom = Any2Integer(oDefinition("CarryOverIDCauseFrom_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverIDCauseTo_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverIDCauseTo = Any2Integer(oDefinition("CarryOverIDCauseTo_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusIDCause_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusIDCause = Any2Integer(oDefinition("PlusIDCause_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusAction_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusAction = Any2Integer(oDefinition("PlusAction_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusDirectValue_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusDirectValue = Any2String(oDefinition("PlusDirectValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusUserFieldValue_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusUserFieldValue = Any2String(oDefinition("PlusUserFieldValue_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusConditionPart_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusConditionPart = Any2Integer(oDefinition("PlusConditionPart_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusConditionNumber_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusConditionNumber = Any2Integer(oDefinition("PlusConditionNumber_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusActionResult_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusActionResult = Any2Integer(oDefinition("PlusActionResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusDirectValueResult_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusDirectValueResult = Any2String(oDefinition("PlusDirectValueResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusUserFieldValueResult_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusUserFieldValueResult = Any2String(oDefinition("PlusUserFieldValueResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusConditionPartResult_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusConditionPartResult = Any2Integer(oDefinition("PlusConditionPartResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusConditionNumberResult_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusConditionNumberResult = Any2Integer(oDefinition("PlusConditionNumberResult_" & i.ToString))
                            End If

                            If oDefinition.Exists("PlusActionSign_" & i.ToString) Then
                                oShiftDailyRuleAction.PlusActionSign = Any2Integer(oDefinition("PlusActionSign_" & i.ToString))
                            End If

                            If oDefinition.Exists("CarryOverSingleCause_" & i.ToString) Then
                                oShiftDailyRuleAction.CarryOverSingleCause = Any2Integer(oDefinition("CarryOverSingleCause_" & i.ToString))
                            End If

                            If oDefinition.Exists("ActionCauses_" & i.ToString) Then
                                Dim strActionCause As String = oDefinition("ActionCauses_" & i.ToString)
                                If strActionCause.Length > 0 Then
                                    For Each strCause As String In strActionCause.Split(",")
                                        Dim oShiftDailyRuleActionCause As New roShiftDailyRuleActionCause
                                        oShiftDailyRuleActionCause.IDCause = Any2Double(strCause.Split("_")(0))
                                        oShiftDailyRuleActionCause.IDCause2 = Any2Integer(strCause.Split("_")(1))
                                        oShiftDailyRuleActionCause.Name = Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleActionCause.IDCause))
                                        oShiftDailyRuleActionCause.Name2 = Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleActionCause.IDCause2))
                                        oShiftDailyRuleAction.ActionCauses.Add(oShiftDailyRuleActionCause)
                                    Next
                                End If
                            End If

                            ' Añadimos la accion
                            oRet.Actions.Add(oShiftDailyRuleAction)

                        Next
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::LoadDailyShiftRule")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::LoadDailyShiftRule")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetDailyShiftRules(ByVal _IDShift As Integer, ByVal _State As roShiftState) As Generic.List(Of roShiftDailyRule)

            Dim oRet As New Generic.List(Of roShiftDailyRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & _IDShift.ToString & " AND " &
                                             "RuleType = 3 " &
                                       "ORDER BY ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each oRow As DataRow In tb.Rows
                    Dim oShiftDailyRule As New roShiftDailyRule
                    oShiftDailyRule = LoadDailyShiftRule(oRow("IDShift"), oRow("ID"), Any2String(oRow("Definition")), _State)
                    If Not oShiftDailyRule Is Nothing Then
                        oShiftDailyRule.Priority = oRet.Count + 1
                        oRet.Add(oShiftDailyRule)
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::GetDailyShiftRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::GetDailyShiftRules")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftRultes(ByVal _IDShift As Integer, ByVal _RuleType As ShiftRuleType, ByVal _State As roShiftState) As Generic.List(Of roShiftRule)

            Dim oRet As New Generic.List(Of roShiftRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & _IDShift.ToString & " AND " &
                                             "RuleType = " & CStr(_RuleType) & " " &
                                       "ORDER BY ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each oRow As DataRow In tb.Rows
                    oRet.Add(New roShiftRule(oRow("IDShift"), oRow("ID"), Any2Integer(oRow("RuleType")), Any2String(oRow("Definition")), _State))
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::GetShiftRultes")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::GetShiftRultes")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveShiftDailyRules(ByVal oRules As Generic.List(Of roShiftDailyRule), ByVal _State As roShiftState,
                                              Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strIDShifts As String = ""
                Dim strIDs As String = ""
                If oRules IsNot Nothing Then
                    bolRet = True
                    For Each oRule As roShiftDailyRule In oRules
                        bolRet = SaveShiftDailyRule(oRule, _State, bAudit)
                        If Not bolRet Then Exit For
                        strIDShifts &= "," & oRule.IDShift.ToString
                        strIDs &= "," & oRule.ID.ToString
                    Next
                    If strIDShifts <> "" Then strIDShifts = strIDShifts.Substring(1)
                    If strIDs <> "" Then strIDs = strIDs.Substring(1)
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las reglas de la tabla que no esten en la lista
                    If strIDShifts <> "" Then
                        Dim strSQL As String = "@DELETE# FROM sysroShiftsCausesRules " &
                                               "WHERE IDShift IN (" & strIDShifts & ") AND ID NOT IN (" & strIDs & ") AND " &
                                                     "RuleType = 3"
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftDailyRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftDailyRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveShiftDailyRule(ByVal oRule As roShiftDailyRule, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ValidateShiftDailyRule(oRule, _State) Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("sysroShiftsCausesRules")
                    Dim strSQL As String = "@SELECT# * FROM sysroShiftsCausesRules " &
                                               "WHERE IDShift = " & oRule.IDShift.ToString & " AND ID = " & oRule.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    Dim _Rule As New roShiftRule
                    _Rule.IDShift = oRule.IDShift

                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDShift") = oRule.IDShift
                        If oRule.ID <= 0 Then
                            oRule.ID = _Rule.GetNextID()
                        End If
                        oRow("ID") = oRule.ID
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("RuleType") = ShiftRuleType.Daily
                    oRow("Definition") = roShiftRule.GetShiftDailyRuleXml(oRule, _State)

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True
                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String = ""
                        bolRet = _State.Audit(oAuditAction, Audit.ObjectType.tShiftRule, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftDailyRule")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftDailyRule")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveShiftRules(ByVal oRules As Generic.List(Of roShiftRule), ByVal _RuleType As ShiftRuleType, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strIDShifts As String = ""
                Dim strIDs As String = ""
                If oRules IsNot Nothing Then
                    bolRet = True
                    For Each oRule As roShiftRule In oRules
                        oRule.Type = _RuleType
                        bolRet = oRule.Save(bAudit)
                        If Not bolRet Then Exit For
                        strIDShifts &= "," & oRule.IDShift.ToString
                        strIDs &= "," & oRule.ID.ToString
                    Next
                    If strIDShifts <> "" Then strIDShifts = strIDShifts.Substring(1)
                    If strIDs <> "" Then strIDs = strIDs.Substring(1)
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las reglas de la tabla que no esten en la lista
                    If strIDShifts <> "" Then
                        Dim strSQL As String = "@DELETE# FROM sysroShiftsCausesRules " &
                                               "WHERE IDShift IN (" & strIDShifts & ") AND ID NOT IN (" & strIDs & ") AND " &
                                                     "RuleType = " & CStr(_RuleType)
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::SaveShiftRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteShiftRules(ByVal _IDShift As Integer, ByVal _RuleType As ShiftRuleType, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM sysroShiftsCausesRules WHERE IDShift = " & _IDShift.ToString & " AND RuleType = " & CStr(_RuleType)
                bolRet = ExecuteSql(strSQL)

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = _State.Audit(Audit.Action.aDelete, Audit.ObjectType.tShiftRule, "", Nothing, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::DeleteShiftRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::DeleteShiftRules")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateShiftRules(ByVal oRules As Generic.List(Of roShiftRule), ByVal _State As roShiftState) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Verificar que
                If oRules.Count > 0 Then
                    For Each oRule As roShiftRule In oRules
                        bolRet = oRule.Validate()
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::ValidateShiftRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::ValidateShiftRules")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateShiftDailyRule(ByVal oRule As roShiftDailyRule, ByVal _State As roShiftState) As Boolean

            Dim bolRet As Boolean = True

            Try

                If oRule.Name.Length = 0 Then
                    _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                    bolRet = False
                End If

                If bolRet And (oRule.Conditions Is Nothing OrElse oRule.Conditions.Count = 0) Then
                    _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                    bolRet = False
                End If

                If bolRet And (oRule.Actions Is Nothing OrElse oRule.Actions.Count = 0) Then
                    _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                    bolRet = False
                End If

                If bolRet Then
                    For Each oShiftDailyRuleCondition As roShiftDailyRuleCondition In oRule.Conditions
                        If bolRet Then
                            If oShiftDailyRuleCondition.ConditionCauses Is Nothing OrElse oShiftDailyRuleCondition.ConditionCauses.Count = 0 Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If

                            If bolRet And oShiftDailyRuleCondition.ConditionTimeZones Is Nothing OrElse oShiftDailyRuleCondition.ConditionTimeZones.Count = 0 Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If

                            ' Si el valor a comparar es un campo
                            If bolRet And oShiftDailyRuleCondition.Type = DailyConditionValueType.UserField And oShiftDailyRuleCondition.UserField.Length = 0 Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If

                            ' Si el valor a comparar son justificaciones
                            If bolRet And oShiftDailyRuleCondition.Type = DailyConditionValueType.ID And (oShiftDailyRuleCondition.CompareCauses Is Nothing OrElse oShiftDailyRuleCondition.CompareCauses.Count = 0) Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If

                            ' Si el valor a comparar son justificaciones
                            If bolRet And oShiftDailyRuleCondition.Type = DailyConditionValueType.ID And (oShiftDailyRuleCondition.CompareTimeZones Is Nothing OrElse oShiftDailyRuleCondition.CompareTimeZones.Count = 0) Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If
                        End If
                    Next
                End If

                If bolRet Then
                    For Each oShiftDailyRuleAction As roShiftDailyRuleAction In oRule.Actions
                        If bolRet Then
                            If oShiftDailyRuleAction.CarryOverAction = DailyConditionValueType.UserField And oShiftDailyRuleAction.CarryOverUserFieldValue.Length = 0 Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If
                            If bolRet And oShiftDailyRuleAction.CarryOverActionResult = DailyConditionValueType.UserField And oShiftDailyRuleAction.CarryOverUserFieldValueResult.Length = 0 Then
                                _State.Result = ShiftResultEnum.ShiftRule_InvalidData
                                bolRet = False
                            End If
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::ValidateShiftDailyRule")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::ValidateShiftDailyRule")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function GetShiftDailyRuleXml(ByVal oRule As roShiftDailyRule, ByVal _State As roShiftState) As String
            Dim oDefinition As New roCollection

            Try

                ' Nombre
                oDefinition.Add("Name", oRule.Name)

                ' Descripcion
                oDefinition.Add("Description", oRule.Description)

                ' Dia de validacion de la regla
                oDefinition.Add("DayValidationRule", oRule.DayValidationRule)

                ' Horario planificado anteriormente para validacion de la regla
                oDefinition.Add("PreviousShiftValidationRule", String.Join(",", oRule.PreviousShiftValidationRule))

                ' Tipo de regla
                oDefinition.Add("RuleType", "3")

                ' Tipo de validación para descanso entre jornadas
                oDefinition.Add("ApplyScheduleValidationRule", oRule.ApplyScheduleValidationRule)

                ' Reglas de planificación de tipo descanso
                oDefinition.Add("ScheduleRulesValidationRule", String.Join(",", oRule.ScheduleRulesValidationRule))

                ' Guardamos los datos de las condiciones
                oDefinition.Add("TotalConditions", oRule.Conditions.Count)

                ' Para cada condicion guardamos sus parametros
                For i As Integer = 1 To oRule.Conditions.Count
                    ' Guardamos las justificaciones(parte 1)
                    Dim strConditionCauses As String = ""
                    For x As Integer = 1 To oRule.Conditions(i - 1).ConditionCauses.Count
                        strConditionCauses += "," & oRule.Conditions(i - 1).ConditionCauses(x - 1).IDCause.ToString & "_" & oRule.Conditions(i - 1).ConditionCauses(x - 1).Operation
                    Next
                    If strConditionCauses.Length > 0 Then strConditionCauses = strConditionCauses.Substring(1)
                    oDefinition.Add("ConditionCauses_" & i.ToString, strConditionCauses)

                    ' Guardamos las zonas horarias(parte 1)
                    Dim strConditionTimeZones As String = ""
                    For x As Integer = 1 To oRule.Conditions(i - 1).ConditionTimeZones.Count
                        strConditionTimeZones += "," & oRule.Conditions(i - 1).ConditionTimeZones(x - 1).IDTimeZone.ToString
                    Next
                    If strConditionTimeZones.Length > 0 Then strConditionTimeZones = strConditionTimeZones.Substring(1)
                    oDefinition.Add("ConditionTimeZones_" & i.ToString, strConditionTimeZones)

                    oDefinition.Add("Compare_" & i.ToString, oRule.Conditions(i - 1).Compare)
                    oDefinition.Add("Type_" & i.ToString, oRule.Conditions(i - 1).Type)
                    oDefinition.Add("FromValue_" & i.ToString, oRule.Conditions(i - 1).FromValue)
                    oDefinition.Add("ToValue_" & i.ToString, oRule.Conditions(i - 1).ToValue)
                    oDefinition.Add("UserField_" & i.ToString, oRule.Conditions(i - 1).UserField)

                    ' Guardamos las justificaciones(parte 2)
                    strConditionCauses = ""
                    For x As Integer = 1 To oRule.Conditions(i - 1).CompareCauses.Count
                        strConditionCauses += "," & oRule.Conditions(i - 1).CompareCauses(x - 1).IDCause.ToString & "_" & oRule.Conditions(i - 1).CompareCauses(x - 1).Operation
                    Next
                    If strConditionCauses.Length > 0 Then strConditionCauses = strConditionCauses.Substring(1)
                    oDefinition.Add("CompareCauses_" & i.ToString, strConditionCauses)

                    ' Guardamos las zonas horarias(parte 2)
                    strConditionTimeZones = ""
                    For x As Integer = 1 To oRule.Conditions(i - 1).CompareTimeZones.Count
                        strConditionTimeZones += "," & oRule.Conditions(i - 1).CompareTimeZones(x - 1).IDTimeZone.ToString
                    Next
                    If strConditionTimeZones.Length > 0 Then strConditionTimeZones = strConditionTimeZones.Substring(1)
                    oDefinition.Add("CompareTimeZones_" & i.ToString, strConditionTimeZones)

                Next

                ' Para cada accion guardamos sus parametros
                oDefinition.Add("TotalActions", oRule.Actions.Count)

                For i As Integer = 1 To oRule.Actions.Count
                    oDefinition.Add("Action_" & i.ToString, oRule.Actions(i - 1).Action)
                    oDefinition.Add("CarryOverAction_" & i.ToString, oRule.Actions(i - 1).CarryOverAction)
                    oDefinition.Add("CarryOverDirectValue_" & i.ToString, oRule.Actions(i - 1).CarryOverDirectValue)
                    oDefinition.Add("CarryOverUserFieldValue_" & i.ToString, oRule.Actions(i - 1).CarryOverUserFieldValue)
                    oDefinition.Add("CarryOverConditionPart_" & i.ToString, oRule.Actions(i - 1).CarryOverConditionPart)
                    oDefinition.Add("CarryOverConditionNumber_" & i.ToString, oRule.Actions(i - 1).CarryOverConditionNumber)
                    oDefinition.Add("CarryOverActionResult_" & i.ToString, oRule.Actions(i - 1).CarryOverActionResult)
                    oDefinition.Add("CarryOverDirectValueResult_" & i.ToString, oRule.Actions(i - 1).CarryOverDirectValueResult)
                    oDefinition.Add("CarryOverUserFieldValueResult_" & i.ToString, oRule.Actions(i - 1).CarryOverUserFieldValueResult)
                    oDefinition.Add("CarryOverConditionPartResult_" & i.ToString, oRule.Actions(i - 1).CarryOverConditionPartResult)
                    oDefinition.Add("CarryOverConditionNumberResult_" & i.ToString, oRule.Actions(i - 1).CarryOverConditionNumberResult)
                    oDefinition.Add("CarryOverIDCauseFrom_" & i.ToString, oRule.Actions(i - 1).CarryOverIDCauseFrom)
                    oDefinition.Add("CarryOverIDCauseTo_" & i.ToString, oRule.Actions(i - 1).CarryOverIDCauseTo)
                    oDefinition.Add("PlusIDCause_" & i.ToString, oRule.Actions(i - 1).PlusIDCause)
                    oDefinition.Add("PlusAction_" & i.ToString, oRule.Actions(i - 1).PlusAction)
                    oDefinition.Add("PlusDirectValue_" & i.ToString, oRule.Actions(i - 1).PlusDirectValue)
                    oDefinition.Add("PlusUserFieldValue_" & i.ToString, oRule.Actions(i - 1).PlusUserFieldValue)
                    oDefinition.Add("PlusConditionPart_" & i.ToString, oRule.Actions(i - 1).PlusConditionPart)
                    oDefinition.Add("PlusConditionNumber_" & i.ToString, oRule.Actions(i - 1).PlusConditionNumber)
                    oDefinition.Add("PlusActionResult_" & i.ToString, oRule.Actions(i - 1).PlusActionResult)
                    oDefinition.Add("PlusDirectValueResult_" & i.ToString, oRule.Actions(i - 1).PlusDirectValueResult)
                    oDefinition.Add("PlusUserFieldValueResult_" & i.ToString, oRule.Actions(i - 1).PlusUserFieldValueResult)
                    oDefinition.Add("PlusConditionPartResult_" & i.ToString, oRule.Actions(i - 1).PlusConditionPartResult)
                    oDefinition.Add("PlusConditionNumberResult_" & i.ToString, oRule.Actions(i - 1).PlusConditionNumberResult)
                    oDefinition.Add("PlusActionSign_" & i.ToString, oRule.Actions(i - 1).PlusActionSign)
                    oDefinition.Add("CarryOverSingleCause_" & i.ToString, oRule.Actions(i - 1).CarryOverSingleCause)

                    Dim strActionCauses As String = ""
                    For x As Integer = 1 To oRule.Actions(i - 1).ActionCauses.Count
                        strActionCauses += "," & oRule.Actions(i - 1).ActionCauses(x - 1).IDCause.ToString & "_" & oRule.Actions(i - 1).ActionCauses(x - 1).IDCause2
                    Next
                    If strActionCauses.Length > 0 Then strActionCauses = strActionCauses.Substring(1)
                    oDefinition.Add("ActionCauses_" & i.ToString, strActionCauses)

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftRule::GetShiftDailyRuleXml")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftRule::GetShiftDailyRuleXml")
            Finally
            End Try

            Return oDefinition.XML

        End Function

#End Region

#End Region

    End Class

End Namespace