Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace LabAgree

    <DataContract()>
    <Serializable()>
    Public Class roStartupValue

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intID As Integer

        Private intIDConcept As Integer
        Private intOriginalIDConcept As Integer
        Private strName As String

        Private intCalculatedType As Integer
        Private strScalingUserField As String
        Private lstScalingFieldValues As Generic.List(Of roScalingValues)
        Private strScalingCoefficientUserField As String

        Private oStartValueType As LabAgreeValueType = LabAgreeValueType.None
        Private dblStartValue As Double
        Private strStartUserField As roUserField

        Private intStartValueBaseType As LabAgreeValueTypeBase = LabAgreeValueTypeBase.DirectValue
        Private dblStartValueBase As Double
        Private strStartUserFieldBase As roUserField

        Private intTotalPeriodBaseType As LabAgreeValueTypeBase = LabAgreeValueTypeBase.DirectValue
        Private dblTotalPeriodBase As Double
        Private strStartUserFieldTotalPeriodBase As roUserField

        Private intAccruedValueType As LabAgreeValueTypeBase = LabAgreeValueTypeBase.DirectValue
        Private dblAccruedValue As Double
        Private strStartUserFieldAccruedValue As roUserField

        Private intRoundingType As Integer = 0

        Private bolNewContractException As Boolean
        Private lstNewContractExceptionCriteria As Generic.List(Of roUserFieldCondition)

        Private oMaximumValueType As LabAgreeValueType = LabAgreeValueType.None
        Private dblMaximumValue As Double
        Private strMaximumUserField As roUserField

        Private oMinimumValueType As LabAgreeValueType = LabAgreeValueType.None
        Private dblMinimumValue As Double
        Private strMinimumUserField As roUserField

        Private bolApplyEndCustomPeriod As Boolean
        Private strEndCustomPeriodUserField As roUserField
        Public Const roNullDate = "1/1/2079"

        Private oExpiration As roStartupValueExpirationRule
        Private oEnjoyment As roStartupValueEnjoymentRule


        Public Sub New()
            Me.oState = New roLabAgreeState
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roLabAgreeState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roLabAgreeState)
                Me.oState = value
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
        Public Property IDConcept() As Integer
            Get
                Return Me.intIDConcept
            End Get
            Set(ByVal value As Integer)
                Me.intIDConcept = value
            End Set
        End Property
        <DataMember()>
        Public Property CalculatedType() As Integer
            Get
                Return Me.intCalculatedType
            End Get
            Set(ByVal value As Integer)
                Me.intCalculatedType = value
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember()>
        Public Property ScalingFieldValues() As Generic.List(Of roScalingValues)
            Get
                Return Me.lstScalingFieldValues
            End Get
            Set(ByVal value As Generic.List(Of roScalingValues))
                Me.lstScalingFieldValues = value
            End Set
        End Property

        <DataMember()>
        Public Property ScalingUserField() As String
            Get
                Return Me.strScalingUserField
            End Get
            Set(ByVal value As String)
                Me.strScalingUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property ScalingCoefficientUserField() As String
            Get
                Return Me.strScalingCoefficientUserField
            End Get
            Set(ByVal value As String)
                Me.strScalingCoefficientUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property StartValueType() As LabAgreeValueType
            Get
                Return Me.oStartValueType
            End Get
            Set(ByVal value As LabAgreeValueType)
                Me.oStartValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property StartValue() As Double
            Get
                Return Me.dblStartValue
            End Get
            Set(ByVal value As Double)
                Me.dblStartValue = value
            End Set
        End Property

        <DataMember()>
        Public Property StartValueBaseType() As LabAgreeValueTypeBase
            Get
                Return Me.intStartValueBaseType
            End Get
            Set(ByVal value As LabAgreeValueTypeBase)
                Me.intStartValueBaseType = value
            End Set
        End Property

        <DataMember()>
        Public Property TotalPeriodBaseType() As LabAgreeValueTypeBase
            Get
                Return Me.intTotalPeriodBaseType
            End Get
            Set(ByVal value As LabAgreeValueTypeBase)
                Me.intTotalPeriodBaseType = value
            End Set
        End Property

        <DataMember()>
        Public Property AccruedValueType() As LabAgreeValueTypeBase
            Get
                Return Me.intAccruedValueType
            End Get
            Set(ByVal value As LabAgreeValueTypeBase)
                Me.intAccruedValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property StartValueBase() As Double
            Get
                Return Me.dblStartValueBase
            End Get
            Set(ByVal value As Double)
                Me.dblStartValueBase = value
            End Set
        End Property

        <DataMember()>
        Public Property TotalPeriodBase() As Double
            Get
                Return Me.dblTotalPeriodBase
            End Get
            Set(ByVal value As Double)
                Me.dblTotalPeriodBase = value
            End Set
        End Property

        <DataMember()>
        Public Property AccruedValue() As Double
            Get
                Return Me.dblAccruedValue
            End Get
            Set(ByVal value As Double)
                Me.dblAccruedValue = value
            End Set
        End Property

        <DataMember()>
        Public Property StartUserField() As roUserField
            Get
                Return Me.strStartUserField
            End Get
            Set(ByVal value As roUserField)
                Me.strStartUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property StartUserFieldBase() As roUserField
            Get
                Return Me.strStartUserFieldBase
            End Get
            Set(ByVal value As roUserField)
                Me.strStartUserFieldBase = value
            End Set
        End Property

        <DataMember()>
        Public Property StartUserFieldTotalPeriodBase() As roUserField
            Get
                Return Me.strStartUserFieldTotalPeriodBase
            End Get
            Set(ByVal value As roUserField)
                Me.strStartUserFieldTotalPeriodBase = value
            End Set
        End Property

        <DataMember()>
        Public Property StartUserFieldAccruedValue() As roUserField
            Get
                Return Me.strStartUserFieldAccruedValue
            End Get
            Set(ByVal value As roUserField)
                Me.strStartUserFieldAccruedValue = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumValueType() As LabAgreeValueType
            Get
                Return Me.oMaximumValueType
            End Get
            Set(ByVal value As LabAgreeValueType)
                Me.oMaximumValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumValue() As Double
            Get
                Return Me.dblMaximumValue
            End Get
            Set(ByVal value As Double)
                Me.dblMaximumValue = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumUserField() As roUserField
            Get
                Return Me.strMaximumUserField
            End Get
            Set(ByVal value As roUserField)
                Me.strMaximumUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property MinimumValueType() As LabAgreeValueType
            Get
                Return Me.oMinimumValueType
            End Get
            Set(ByVal value As LabAgreeValueType)
                Me.oMinimumValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property MinimumValue() As Double
            Get
                Return Me.dblMinimumValue
            End Get
            Set(ByVal value As Double)
                Me.dblMinimumValue = value
            End Set
        End Property

        <DataMember()>
        Public Property MinimumUserField() As roUserField
            Get
                Return Me.strMinimumUserField
            End Get
            Set(ByVal value As roUserField)
                Me.strMinimumUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property ApplyEndCustomPeriod() As Boolean
            Get
                Return Me.bolApplyEndCustomPeriod
            End Get
            Set(ByVal value As Boolean)
                Me.bolApplyEndCustomPeriod = value
            End Set
        End Property

        <DataMember()>
        Public Property EndCustomPeriodUserField() As roUserField
            Get
                Return Me.strEndCustomPeriodUserField
            End Get
            Set(ByVal value As roUserField)
                Me.strEndCustomPeriodUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property OriginalIDConcept() As Integer
            Get
                Return Me.intOriginalIDConcept
            End Get
            Set(ByVal value As Integer)
                Me.intOriginalIDConcept = value
            End Set
        End Property

        <DataMember()>
        Public Property RoundingType() As Integer
            Get
                Return Me.intRoundingType
            End Get
            Set(ByVal value As Integer)
                Me.intRoundingType = value
            End Set
        End Property

        <DataMember()>
        Public Property NewContractException() As Boolean
            Get
                Return Me.bolNewContractException
            End Get
            Set(ByVal value As Boolean)
                Me.bolNewContractException = value
            End Set
        End Property

        <DataMember()>
        Public Property NewContractExceptionCriteria() As Generic.List(Of roUserFieldCondition)
            Get
                Return Me.lstNewContractExceptionCriteria
            End Get
            Set(ByVal value As Generic.List(Of roUserFieldCondition))
                Me.lstNewContractExceptionCriteria = value
            End Set
        End Property

        <DataMember()>
        Public Property Expiration() As roStartupValueExpirationRule
            Get
                Return Me.oExpiration
            End Get
            Set(ByVal value As roStartupValueExpirationRule)
                Me.oExpiration = value
            End Set
        End Property

        <DataMember()>
        Public Property Enjoyment() As roStartupValueEnjoymentRule
            Get
                Return Me.oEnjoyment
            End Get
            Set(ByVal value As roStartupValueEnjoymentRule)
                Me.oEnjoyment = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM StartupValues " &
                                       "WHERE IDStartupValue = " & Me.intID.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)
                    intID = Any2Integer(oRow("IDStartupValue"))
                    intOriginalIDConcept = Any2Integer(oRow("IDConcept"))
                    intIDConcept = Any2Integer(oRow("IDConcept"))
                    strName = Any2String(oRow("Name"))

                    strScalingUserField = Any2String(oRow("ScalingUserField"))
                    strScalingCoefficientUserField = Any2String(oRow("ScalingCoefficientUserField"))
                    lstScalingFieldValues = New Generic.List(Of roScalingValues)
                    Dim strStartUpFieldValues = Any2String(oRow("ScalingValues")).Split(New Char() {"@"c}, StringSplitOptions.RemoveEmptyEntries)

                    For Each value As String In strStartUpFieldValues
                        lstScalingFieldValues.Add(New roScalingValues(value.Split("#")(0), value.Split("#")(1)))
                    Next

                    intCalculatedType = Any2Integer(oRow("CalculatedType"))

                    If oRow("StartValueType") IsNot DBNull.Value Then
                        Select Case oRow("StartValueType")
                            Case 0 'None
                                oStartValueType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oStartValueType = LabAgreeValueType.DirectValue
                                dblStartValue = Any2Double(roTypes.Any2String(oRow("StartValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oStartValueType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFStartup As New roUserField(oUFState, oRow("StartValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    StartUserField = oUFStartup
                                End If
                            Case 3 'CalculatedValue
                                oStartValueType = LabAgreeValueType.CalculatedValue

                                ' Valor Base
                                If Any2Integer(oRow("StartValueBaseType")) = 0 Then
                                    ' Directo
                                    StartValueBaseType = LabAgreeValueTypeBase.DirectValue
                                    dblStartValueBase = Any2Double(roTypes.Any2String(oRow("StartValueBase")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Else
                                    ' Campo ficha
                                    StartValueBaseType = LabAgreeValueTypeBase.UserField
                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFStartup As New roUserField(oUFState, oRow("StartValueBase"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        StartUserFieldBase = oUFStartup
                                    End If
                                End If

                                ' Total Base
                                If Any2Integer(oRow("TotalPeriodBaseType")) = 0 Then
                                    ' Directo
                                    TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue
                                    dblTotalPeriodBase = Any2Double(roTypes.Any2String(oRow("TotalPeriodBase")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Else
                                    ' Campo ficha
                                    TotalPeriodBaseType = LabAgreeValueTypeBase.UserField
                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFStartup As New roUserField(oUFState, oRow("TotalPeriodBase"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        StartUserFieldTotalPeriodBase = oUFStartup
                                    End If
                                End If

                                ' Valor Devengado
                                If Any2Integer(oRow("AccruedValueType")) = 0 Then
                                    ' Directo
                                    AccruedValueType = LabAgreeValueTypeBase.DirectValue
                                    dblAccruedValue = Any2Double(roTypes.Any2String(oRow("AccruedValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Else
                                    ' Campo ficha
                                    AccruedValueType = LabAgreeValueTypeBase.UserField
                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFStartup As New roUserField(oUFState, oRow("AccruedValue"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        StartUserFieldAccruedValue = oUFStartup
                                    End If
                                End If

                                ' Redondeo
                                intRoundingType = Any2Integer(oRow("RoundingType"))

                                bolApplyEndCustomPeriod = False

                                If Not IsDBNull(oRow("ApplyEndCustomPeriod")) AndAlso Any2Boolean(oRow("ApplyEndCustomPeriod")) Then
                                    bolApplyEndCustomPeriod = True
                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oEndCustomPeriodUserField As New roUserField(oUFState, oRow("EndCustomPeriodUserField"), Types.EmployeeField, False, False)
                                    EndCustomPeriodUserField = oEndCustomPeriodUserField
                                End If

                        End Select

                        If roTypes.Any2Integer(oRow("StartValueType")) = 1 OrElse roTypes.Any2Integer(oRow("StartValueType")) = 2 OrElse roTypes.Any2Integer(oRow("StartValueType")) = 3 Then
                            bolNewContractException = Any2Boolean(oRow("NewContractException"))

                            Dim oUserFieldState As New roUserFieldState
                            roBusinessState.CopyTo(Me.oState, oUserFieldState)

                            lstNewContractExceptionCriteria = roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("NewContractExceptionCondition")), oUserFieldState, False)
                        Else
                            bolNewContractException = False
                            lstNewContractExceptionCriteria = New Generic.List(Of roUserFieldCondition)
                        End If
                    Else
                        oStartValueType = LabAgreeValueType.None
                    End If

                    If oRow("MaximumValueType") IsNot DBNull.Value Then
                        Select Case oRow("MaximumValueType")
                            Case 0 'None
                                oMaximumValueType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oMaximumValueType = LabAgreeValueType.DirectValue
                                dblMaximumValue = Any2Double(roTypes.Any2String(oRow("MaximumValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oMaximumValueType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFMaximum As New roUserField(oUFState, oRow("MaximumValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    MaximumUserField = oUFMaximum
                                End If

                                MaximumUserField = oUFMaximum
                        End Select
                    Else
                        oMaximumValueType = LabAgreeValueType.None
                    End If

                    If oRow("MinimumValueType") IsNot DBNull.Value Then
                        Select Case oRow("MinimumValueType")
                            Case 0 'None
                                oMinimumValueType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oMinimumValueType = LabAgreeValueType.DirectValue
                                dblMinimumValue = Any2Double(roTypes.Any2String(oRow("MinimumValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oMinimumValueType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFMinimum As New roUserField(oUFState, oRow("MinimumValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    MinimumUserField = oUFMinimum
                                End If

                                MinimumUserField = oUFMinimum
                        End Select
                    Else
                        oMinimumValueType = LabAgreeValueType.None
                    End If

                    ' Caducidad
                    If oStartValueType <> LabAgreeValueType.None AndAlso oRow("ExpirationValue") IsNot DBNull.Value Then
                        Me.Expiration = New roStartupValueExpirationRule
                        Expiration.Unit = Any2Integer(oRow("ExpirationUnit"))
                        Expiration.ExpireAfter = Any2Integer(oRow("ExpirationValue"))
                    End If

                    ' Disfrute
                    If oStartValueType <> LabAgreeValueType.None AndAlso oRow("StartEnjoymentValue") IsNot DBNull.Value Then
                        Me.Enjoyment = New roStartupValueEnjoymentRule
                        Enjoyment.Unit = roTypes.Any2Integer(oRow("EnjoymentUnit"))
                        Enjoyment.StartAfter = roTypes.Any2Integer(oRow("StartEnjoymentValue"))
                    End If

                    ' Auditar lectura
                    If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tStartupValues, Me.strName, tbParameters, -1)
                        End If

                    End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roStartupValue:: Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal IDLabAgree As Integer) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = LabAgreeResultEnum.StartupValueNameInvalid
                    bolRet = False
                End If

                If bolRet Then
                    ' Debe insertar un concepto
                    If Me.IDConcept < 1 Then
                        oState.Result = LabAgreeResultEnum.StartupValueIDConceptEmpty
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso Me.StartValueType = LabAgreeValueType.UserField Then
                    If Me.StartUserField Is Nothing OrElse Me.StartUserField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.StartupValueStartUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso Me.MaximumValueType = LabAgreeValueType.UserField Then
                    If Me.MaximumUserField Is Nothing OrElse Me.MaximumUserField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.StartupValueMaximumUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso Me.MinimumValueType = LabAgreeValueType.UserField Then
                    If Me.MinimumUserField Is Nothing OrElse Me.MinimumUserField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.StartupValueMinimumUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso Me.MaximumValueType = LabAgreeValueType.None And Me.MinimumValueType = LabAgreeValueType.None And Me.StartValueType = LabAgreeValueType.None Then
                    oState.Result = LabAgreeResultEnum.StartupValueNoUserFieldsSelected
                    bolRet = False
                End If

                If bolRet AndAlso Me.ApplyEndCustomPeriod Then
                    If Me.EndCustomPeriodUserField Is Nothing OrElse Me.EndCustomPeriodUserField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.StartupValueStartUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso Me.IDConcept > 0 AndAlso Me.StartValueType <> LabAgreeValueType.None Then
                    ' En el caso que el saldo sea por contrato y con caducidad no se pueden definir valores iniciales
                    ' al ser incompatible con el tipo de saldo que es
                    Dim strDefaultQuery As String = Any2String(ExecuteScalar("@SELECT# DefaultQuery FROM Concepts WHERE [ID] = " & Me.IDConcept))
                    Dim bApplyExpiredHours As Boolean = Any2Boolean(ExecuteScalar("@SELECT# ApplyExpiredHours FROM Concepts WHERE [ID] = " & Me.IDConcept))

                    If strDefaultQuery = "C" AndAlso bApplyExpiredHours Then
                        oState.Result = LabAgreeResultEnum.LabAgreeRulesConceptIncompatibleAction
                        bolRet = False
                        Return bolRet
                    End If
                End If

                If bolRet AndAlso Me.StartValueType = LabAgreeValueType.CalculatedValue Then
                    ' Validamos los campos de la ficha, si es del tipo valor calculado
                    If Me.StartValueBaseType = LabAgreeValueTypeBase.UserField Then
                        If Me.StartUserFieldBase Is Nothing OrElse Me.StartUserFieldBase.FieldName = "" Then
                            oState.Result = LabAgreeResultEnum.StartupValueStartUserFieldEmpty
                            bolRet = False
                        End If
                    End If

                    If Me.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField Then
                        If Me.StartUserFieldTotalPeriodBase Is Nothing OrElse Me.StartUserFieldTotalPeriodBase.FieldName = "" Then
                            oState.Result = LabAgreeResultEnum.StartupValueStartUserFieldEmpty
                            bolRet = False
                        End If
                    End If

                    If Me.AccruedValueType = LabAgreeValueTypeBase.UserField Then
                        If Me.StartUserFieldAccruedValue Is Nothing OrElse Me.StartUserFieldAccruedValue.FieldName = "" Then
                            oState.Result = LabAgreeResultEnum.StartupValueStartUserFieldEmpty
                            bolRet = False
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roStartupValue::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal idLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate(idLabAgree) Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oOldStartupValue As roStartupValue = Nothing

                    Dim tb As New DataTable("StartupValues")
                    Dim strSQL As String = "@SELECT# * FROM StartupValues " &
                                           "WHERE IDStartupValue = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                    Else
                        oOldStartupValue = New roStartupValue(Me.intID, Me.oState, False) ' Obtengo una còpia del StartupValue actual, para poder recalcular
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = strName
                    oRow("StartValueType") = oStartValueType
                    oRow("IDConcept") = Me.IDConcept

                    oRow("StartValue") = ""

                    oRow("StartValueBaseType") = LabAgreeValueTypeBase.DirectValue
                    oRow("StartValueBase") = "0"

                    oRow("TotalPeriodBaseType") = LabAgreeValueTypeBase.DirectValue
                    oRow("TotalPeriodBase") = "0"

                    oRow("AccruedValueType") = LabAgreeValueTypeBase.DirectValue
                    oRow("AccruedValue") = "0"

                    oRow("RoundingType") = 0
                    oRow("CalculatedType") = 0
                    oRow("ScalingUserField") = ""
                    oRow("ScalingCoefficientUserField") = ""

                    oRow("EndCustomPeriodUserField") = ""
                    oRow("ApplyEndCustomPeriod") = False

                    Select Case oStartValueType
                        Case LabAgreeValueType.None
                            oRow("StartValue") = ""
                        Case LabAgreeValueType.DirectValue
                            oRow("StartValue") = dblStartValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Case LabAgreeValueType.UserField
                            oRow("StartValue") = strStartUserField.FieldName.ToString
                        Case LabAgreeValueType.CalculatedValue

                            oRow("StartValueBaseType") = StartValueBaseType
                            If StartValueBaseType = LabAgreeValueTypeBase.DirectValue Then
                                oRow("StartValueBase") = dblStartValueBase.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                            Else
                                oRow("StartValueBase") = strStartUserFieldBase.FieldName.ToString
                            End If

                            oRow("TotalPeriodBaseType") = TotalPeriodBaseType
                            If TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue Then
                                oRow("TotalPeriodBase") = dblTotalPeriodBase.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                            Else
                                oRow("TotalPeriodBase") = strStartUserFieldTotalPeriodBase.FieldName.ToString
                            End If

                            oRow("AccruedValueType") = AccruedValueType
                            If AccruedValueType = LabAgreeValueTypeBase.DirectValue Then
                                oRow("AccruedValue") = dblAccruedValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                            Else
                                oRow("AccruedValue") = strStartUserFieldAccruedValue.FieldName.ToString
                            End If

                            oRow("RoundingType") = intRoundingType

                            oRow("ApplyEndCustomPeriod") = bolApplyEndCustomPeriod
                            If bolApplyEndCustomPeriod Then
                                oRow("EndCustomPeriodUserField") = strEndCustomPeriodUserField.FieldName.ToString
                            Else
                                oRow("EndCustomPeriodUserField") = ""
                            End If

                            oRow("ScalingUserField") = strScalingUserField
                            oRow("ScalingCoefficientUserField") = strScalingCoefficientUserField

                            Dim scalingValues As String = ""
                            If lstScalingFieldValues.Count > 0 Then
                                For Each value In lstScalingFieldValues
                                    scalingValues &= value.UserField + "#" + value.AccumValue + "@"
                                Next
                                scalingValues = scalingValues.Remove(scalingValues.Length - 1)
                            End If
                            oRow("ScalingValues") = scalingValues
                            oRow("CalculatedType") = intCalculatedType
                    End Select

                    If oStartValueType = LabAgreeValueType.DirectValue OrElse oStartValueType = LabAgreeValueType.UserField OrElse oStartValueType = LabAgreeValueType.CalculatedValue Then
                        oRow("NewContractException") = Me.NewContractException
                        If Me.NewContractExceptionCriteria IsNot Nothing AndAlso Me.NewContractExceptionCriteria.Count > 0 Then
                            If Me.NewContractExceptionCriteria.Count > 1 Then NewContractExceptionCriteria.RemoveRange(0, NewContractExceptionCriteria.Count - 1)
                            oRow("NewContractExceptionCondition") = Replace(roUserFieldCondition.GetXml(NewContractExceptionCriteria), "'", "''")
                        Else
                            oRow("NewContractExceptionCondition") = DBNull.Value
                        End If
                    End If

                    oRow("MaximumValueType") = oMaximumValueType
                    Select Case oMaximumValueType
                        Case LabAgreeValueType.None
                            oRow("MaximumValue") = ""
                        Case LabAgreeValueType.DirectValue
                            oRow("MaximumValue") = dblMaximumValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Case LabAgreeValueType.UserField
                            oRow("MaximumValue") = strMaximumUserField.FieldName.ToString
                    End Select

                    oRow("MinimumValueType") = oMinimumValueType
                    Select Case oMinimumValueType
                        Case LabAgreeValueType.None
                            oRow("MinimumValue") = ""
                        Case LabAgreeValueType.DirectValue
                            oRow("MinimumValue") = dblMinimumValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Case LabAgreeValueType.UserField
                            oRow("MinimumValue") = strMinimumUserField.FieldName.ToString
                    End Select

                    ' Caducidad
                    oRow("ExpirationValue") = DBNull.Value
                    oRow("ExpirationUnit") = DBNull.Value
                    If Expiration IsNot Nothing AndAlso Expiration.ExpireAfter > 0 AndAlso oStartValueType <> LabAgreeValueType.None Then
                        oRow("ExpirationUnit") = Expiration.Unit
                        oRow("ExpirationValue") = Expiration.ExpireAfter
                    End If

                    ' Disfrute
                    oRow("StartEnjoymentValue") = DBNull.Value
                    oRow("EnjoymentUnit") = DBNull.Value
                    If Enjoyment IsNot Nothing AndAlso Enjoyment.StartAfter > 0 AndAlso oStartValueType <> LabAgreeValueType.None Then
                        oRow("StartEnjoymentValue") = Enjoyment.StartAfter
                        oRow("EnjoymentUnit") = Enjoyment.Unit
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    If Me.intID <= 0 Then
                        Dim tmpStartupValues As DataTable = CreateDataTable("@SELECT# TOP 1 [IDStartupValue] FROM StartupValues " &
                                                              "ORDER BY [IDStartupValue] DESC", )
                        If tmpStartupValues IsNot Nothing AndAlso tmpStartupValues.Rows.Count = 1 Then
                            Me.intID = tmpStartupValues.Rows(0)("IDStartupValue")
                        End If
                    End If

                    oAuditDataNew = oRow
                    bolRet = True

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tStartupValues, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roStartupValue::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::Save")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra los valores iniciales del acumulado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed(IDLabAgree) Then

                    ' Recalculamos  los empleados y dias que sean necesarios
                    bolRet = Me.Recalculate(Nothing, , , False)

                    If bolRet Then
                        Dim DelQuerys() As String = {"@DELETE# FROM StartupValues WHERE IDStartupValue = " & Me.intID.ToString}
                        For n As Integer = 0 To DelQuerys.Length - 1
                            If Not ExecuteSql(DelQuerys(n)) Then
                                oState.Result = LabAgreeResultEnum.ConnectionError
                                Exit For
                            End If
                        Next

                        bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tStartupValues, Me.strName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roStartupValue::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si el valor inicial se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed(ByVal IDLabAgree As Integer) As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                Dim strUseConcept As String = ""

                ' Reglas de acumulados
                ' Verifica que el acumulado no se esté usando en ninguna regla de acumulados
                'strSQL = "@SELECT# name, Definition FROM AccrualsRules "
                'tb = CreateDataTable(strSQL, )
                'If tb IsNot Nothing Then
                '    strUseConcept = ""
                '    For Each oRow As DataRow In tb.Rows
                '        If Any2String(oRow("Definition")).Trim <> "" Then
                '            ' Compruebo si el IDConcept esta dentro de la definicion de la regla de acumulados
                '            oCollection = New roCollection(oRow("Definition"))
                '            If Any2Integer(oCollection.Item("MainAccrual")) = Me.ID Then
                '                strUseConcept &= ", " & oRow("Name")
                '            End If
                '        End If
                '    Next
                '    If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                '    If strUseConcept <> "" Then
                '        oState.Result = conceptResultEnum.UsedByAccrualRules
                '        bolIsUsed = True
                '    End If
                'End If

                'Dim strUsedLabAgree As String = ""
                'If Not bolIsUsed Then
                '    ' Convenios - Valores iniciales
                '    ' Verifica que el valor inicial no se esté usando en ningún convenio
                '    strSQL = "@SELECT# LabAgree.Name From LabAGree, LabAgreeStartupValues Where LabAgree.ID = IdLabAgree And LabAgreeStartupValues.IDConcept = " & Me.intIDConcept & " AND LabAgree.ID =" & IDLabAgree
                '    tb = CreateDataTable(strSQL, )
                '    If tb IsNot Nothing Then
                '        strUsedLabAgree = ""
                '        For Each oRow As DataRow In tb.Rows
                '            ' Guardo el nombre de todos los empleados que lo usan
                '            strUsedLabAgree &= "," & oRow("Name")
                '        Next
                '        If strUsedLabAgree <> "" Then strUsedLabAgree = strUsedLabAgree.Substring(1)
                '        If strUsedLabAgree <> "" Then
                '            oState.Result = LabAgreeResultEnum.StartupValueUsedInLabAgree
                '            bolIsUsed = True
                '        End If
                '    End If
                'End If

                'If Not bolIsUsed Then
                '    ' Grupos de acumulados
                '    ' Verifica que no existan grupos de acumulados que usen el concepto
                '    strSQL = "@SELECT# SysRoReportGroups.Name " & _
                '             "FROM SysRoReportGroups " & _
                '                    "INNER JOIN SysRoReportGroupConcepts ON " & _
                '                              "SysRoReportGroupConcepts.IDReportGroup = SysRoReportGroups.ID AND " & _
                '                              "SysRoReportGroupConcepts.IDConcept = " & Me.ID.ToString
                '    tb = CreateDataTable(strSQL, )

                '    If tb IsNot Nothing Then
                '        strUseConcept = ""
                '        For Each oRow As DataRow In tb.Rows
                '            ' Guardo el nombre de todos los grupos de acumulados que lo usan
                '            strUseConcept &= ", " & oRow("Name")
                '        Next
                '        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)

                '        If strUseConcept <> "" Then
                '            oState.Result = conceptResultEnum.UsedByReportsGroups
                '            bolIsUsed = True
                '        End If
                '    End If
                'End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roStartupValue::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Actualiza el estado de la 'DailySchedule' de los empleados relacionados a través del convenio y notifica el proceso de recálculo CONCEPTS al servidor.
        ''' </summary>
        ''' <param name="oOldStartupValue">Configuración anterior del valor inicial. Necesario para determinar que valores han cambiado. Si es Nothing, se considera que se tiene que recalcular.</param>
        ''' <param name="_IDEmployee">Opcional. Si se indica, solo actualiza el estado del empleado indicado y se notifica el proceso de recálculo DAILYCAUSES al servidor.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Recalculate(ByVal oOldStartupValue As roStartupValue, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifDate As Date = Nothing, Optional ByVal bolRunTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Verificamos si es necesario recalcular en función de los cambios
                Dim bolMustRecalcStartValue As Boolean = True
                Dim bolMustRecalcMaximumValue As Boolean = True
                Dim bolMustRecalcMinimumValue As Boolean = True

                If oOldStartupValue IsNot Nothing Then
                    bolMustRecalcStartValue = (oOldStartupValue.IDConcept <> Me.IDConcept)
                    bolMustRecalcMaximumValue = (oOldStartupValue.IDConcept <> Me.IDConcept)
                    bolMustRecalcMinimumValue = (oOldStartupValue.IDConcept <> Me.IDConcept)

                    Dim strOldValue As String = ""
                    Dim strValue As String = ""
                    If Not bolMustRecalcStartValue Then
                        If oOldStartupValue.StartValueType = LabAgreeValueType.UserField AndAlso
                           oOldStartupValue.StartUserField IsNot Nothing Then strOldValue = oOldStartupValue.StartUserField.FieldName
                        If Me.StartValueType = LabAgreeValueType.UserField AndAlso
                           Me.StartUserField IsNot Nothing Then strValue = Me.StartUserField.FieldName
                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.DirectValue Then strOldValue = oOldStartupValue.StartValue
                        If Me.StartValueType = LabAgreeValueType.DirectValue Then strValue = Me.StartValue
                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    ' Valor calculado
                    ' Campos de la ficha
                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.StartValueBaseType = LabAgreeValueTypeBase.UserField AndAlso
                           oOldStartupValue.StartUserFieldBase IsNot Nothing Then strOldValue = oOldStartupValue.StartUserFieldBase.FieldName

                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.StartValueBaseType = LabAgreeValueTypeBase.UserField AndAlso
                           Me.StartUserFieldBase IsNot Nothing Then strValue = Me.StartUserFieldBase.FieldName

                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField AndAlso
                           oOldStartupValue.StartUserFieldTotalPeriodBase IsNot Nothing Then strOldValue = oOldStartupValue.StartUserFieldTotalPeriodBase.FieldName

                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField AndAlso
                           Me.StartUserFieldTotalPeriodBase IsNot Nothing Then strValue = Me.StartUserFieldTotalPeriodBase.FieldName

                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.AccruedValueType = LabAgreeValueTypeBase.UserField AndAlso
                           oOldStartupValue.StartUserFieldAccruedValue IsNot Nothing Then strOldValue = oOldStartupValue.StartUserFieldAccruedValue.FieldName

                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.AccruedValueType = LabAgreeValueTypeBase.UserField AndAlso
                           Me.StartUserFieldAccruedValue IsNot Nothing Then strValue = Me.StartUserFieldAccruedValue.FieldName

                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    ' Valores directos
                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.StartValueBaseType = LabAgreeValueTypeBase.DirectValue Then strOldValue = oOldStartupValue.StartValueBase
                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.StartValueBaseType = LabAgreeValueTypeBase.DirectValue Then strValue = Me.StartValueBase
                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue Then strOldValue = oOldStartupValue.TotalPeriodBase
                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue Then strValue = Me.TotalPeriodBase
                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue AndAlso oOldStartupValue.AccruedValueType = LabAgreeValueTypeBase.DirectValue Then strOldValue = oOldStartupValue.AccruedValue
                        If Me.StartValueType = LabAgreeValueType.CalculatedValue AndAlso Me.AccruedValueType = LabAgreeValueTypeBase.DirectValue Then strValue = Me.AccruedValue

                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    ' Redondeo
                    If Not bolMustRecalcStartValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.StartValueType = LabAgreeValueType.CalculatedValue Then strOldValue = oOldStartupValue.RoundingType
                        If Me.StartValueType = LabAgreeValueType.CalculatedValue Then strValue = Me.RoundingType
                        bolMustRecalcStartValue = (strOldValue <> strValue)
                    End If

                    ' Fecha final de periodo
                    If Not bolMustRecalcStartValue Then
                        bolMustRecalcStartValue = (oOldStartupValue.ApplyEndCustomPeriod <> Me.ApplyEndCustomPeriod)
                        If Not bolMustRecalcStartValue Then
                            If oOldStartupValue.ApplyEndCustomPeriod AndAlso Me.ApplyEndCustomPeriod Then
                                If oOldStartupValue.EndCustomPeriodUserField.FieldName <> Me.EndCustomPeriodUserField.FieldName Then
                                    bolMustRecalcStartValue = True
                                End If
                            End If
                        End If
                    End If

                    ' Excepciones , arrastre valores nuevos contratos
                    If Not bolMustRecalcStartValue Then
                        If oOldStartupValue.NewContractException <> Me.NewContractException Then
                            bolMustRecalcStartValue = True
                        End If

                        If Not bolMustRecalcStartValue Then
                            If oOldStartupValue.NewContractException AndAlso Me.NewContractException Then
                                If oOldStartupValue.NewContractExceptionCriteria IsNot Nothing AndAlso
                                        oOldStartupValue.NewContractExceptionCriteria.Count > 0 AndAlso
                                            Me.NewContractExceptionCriteria IsNot Nothing AndAlso Me.NewContractExceptionCriteria.Count > 0 Then
                                    If oOldStartupValue.NewContractExceptionCriteria(0).UserField.FieldName <> Me.NewContractExceptionCriteria(0).UserField.FieldName Then
                                        bolMustRecalcStartValue = True
                                    End If
                                    If oOldStartupValue.NewContractExceptionCriteria(0).Value <> Me.NewContractExceptionCriteria(0).Value Then
                                        bolMustRecalcStartValue = True
                                    End If
                                    If oOldStartupValue.NewContractExceptionCriteria(0).Compare <> Me.NewContractExceptionCriteria(0).Compare Then
                                        bolMustRecalcStartValue = True
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If Not bolMustRecalcStartValue Then
                        ' Verificamos valores iniciales en base a escalado. El resto de la validación debería ser similar ...
                        bolMustRecalcStartValue = (oOldStartupValue.ToString <> Me.ToString)
                    End If

                    ' Maximos
                    If Not bolMustRecalcMaximumValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.MaximumValueType = LabAgreeValueType.UserField AndAlso
                           oOldStartupValue.MaximumUserField IsNot Nothing Then strOldValue = oOldStartupValue.MaximumUserField.FieldName
                        If Me.MaximumValueType = LabAgreeValueType.UserField AndAlso
                           Me.MaximumUserField IsNot Nothing Then strValue = Me.MaximumUserField.FieldName
                        bolMustRecalcMaximumValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcMaximumValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.MaximumValueType = LabAgreeValueType.DirectValue Then strOldValue = oOldStartupValue.MaximumValue
                        If Me.MaximumValueType = LabAgreeValueType.DirectValue Then strValue = Me.MaximumValue
                        bolMustRecalcMaximumValue = (strOldValue <> strValue)
                    End If

                    ' Mínimos
                    If Not bolMustRecalcMinimumValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.MinimumValueType = LabAgreeValueType.UserField AndAlso
                           oOldStartupValue.MinimumUserField IsNot Nothing Then strOldValue = oOldStartupValue.MinimumUserField.FieldName
                        If Me.MinimumValueType = LabAgreeValueType.UserField AndAlso
                           Me.MinimumUserField IsNot Nothing Then strValue = Me.MinimumUserField.FieldName
                        bolMustRecalcMinimumValue = (strOldValue <> strValue)
                    End If

                    If Not bolMustRecalcMinimumValue Then
                        strOldValue = ""
                        strValue = ""
                        If oOldStartupValue.MinimumValueType = LabAgreeValueType.DirectValue Then strOldValue = oOldStartupValue.MinimumValue
                        If Me.MinimumValueType = LabAgreeValueType.DirectValue Then strValue = Me.MinimumValue
                        bolMustRecalcMinimumValue = (strOldValue <> strValue)
                    End If

                End If

                If bolMustRecalcStartValue OrElse bolMustRecalcMaximumValue OrElse bolMustRecalcMinimumValue Then

                    ' Obtenemos la fecha de congelación
                    Dim xFreezingDate As New Date(1900, 1, 1)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                        xFreezingDate = CDate(oParameters.Parameter(Parameters.FirstDate))
                    End If

                    Dim strSQL As String

                    ' Buscamos los empleados que tengan asignado el valor inicial en alguno de sus convenios.
                    strSQL = "@SELECT#  EmployeeContracts.IDEmployee, EmployeeContracts.BeginDate, EmployeeContracts.EndDate, EmployeeContracts.IDContract " &
                             "FROM EmployeeContracts with (nolock) "
                    strSQL &= "INNER JOIN LabAgreeStartupValues with (nolock) " &
                                "ON EmployeeContracts.IDLabAgree = LabAgreeStartupValues.IDLabAgree " &
                                "WHERE LabAgreeStartupValues.IDStartupValue = " & Me.ID.ToString

                    If _IDEmployee <> -1 Then
                        strSQL &= " AND  EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                    End If
                    strSQL &= " Order by  EmployeeContracts.IDEmployee ,  EmployeeContracts.BeginDate"
                    Dim tbEmployees As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                    bolRet = True

                    Dim strEmployees As String = "-1"

                    If tbEmployees.Rows.Count > 0 Then

                        If bolMustRecalcStartValue OrElse bolMustRecalcMaximumValue OrElse bolMustRecalcMinimumValue Then

                            Dim xRecalcDate As Date

                            'Para cada empleado marcamos para recalcular todas las fechas
                            ' posteriores a la fecha de congelación que sean inicio de año/mes/contrato

                            Dim oParams As New roParameters("OPTIONS", True)
                            Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                            Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                            Dim intWeekIniDay As Integer = oParams.Parameter(Parameters.WeekPeriod)
                            If intMonthIniDay = 0 Then intMonthIniDay = 1
                            If intYearIniMonth = 0 Then intYearIniMonth = 1

                            Dim strDefaultQuery As String = Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery,'')  FROM CONCEPTS WHERE ID=" & Me.IDConcept.ToString))

                            bolRet = True

                            For Each oEmployeeRow As DataRow In tbEmployees.Rows
                                xRecalcDate = roBusinessSupport.GetEmployeeLockDatetoApply(oEmployeeRow("IDEmployee"), False, Me.oState)
                                xRecalcDate = xRecalcDate.AddDays(1)
                                If _ModifDate <> Nothing Then
                                    If xRecalcDate < _ModifDate Then xRecalcDate = _ModifDate
                                End If

                                If xRecalcDate < oEmployeeRow("BeginDate") Then
                                    ' Solo es necesario recalcular a partir del incio de contrato , antes es innecesario
                                    xRecalcDate = oEmployeeRow("BeginDate")
                                End If

                                ' Obtenemos los inicios de año/mes en funcion del tipo de saldo a partir de la fecha de inicio de recalculo
                                Dim StartupDays As roCollection = IsStartupDay(xRecalcDate, intYearIniMonth, intMonthIniDay, intWeekIniDay, strDefaultQuery, Any2Integer(oEmployeeRow("IDEmployee")))

                                ' Nos guardamos los empleados afectados
                                strEmployees += "," & oEmployeeRow("IDEmployee")

                                If bolMustRecalcStartValue Then
                                    ' Para cada inicio de contrato actualizamos el status
                                    If oEmployeeRow("BeginDate") >= xRecalcDate Then
                                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                                 "WHERE Status >= 65 AND IDEmployee = " & oEmployeeRow("IDEmployee") & " AND " &
                                                       "Date = " & Any2Time(oEmployeeRow("BeginDate")).SQLSmallDateTime

                                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                        If bolRet Then
                                            strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                                     "@SELECT# Employees.ID, " & Any2Time(oEmployeeRow("BeginDate")).SQLSmallDateTime & ", 65 " &
                                                     "FROM Employees " &
                                                     "WHERE Employees.ID = " & oEmployeeRow("IDEmployee") & " AND " &
                                                           "Employees.ID NOT IN " &
                                                           "(@SELECT# DS.IDEmployee " &
                                                            "FROM DailySchedule DS " &
                                                            "WHERE Date = " & Any2Time(oEmployeeRow("BeginDate")).SQLSmallDateTime & ")"
                                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                                        End If
                                        If Not bolRet Then Exit For

                                    End If

                                    If bolRet Then
                                        ' Para cada inicio de mes/año actualziamos el status
                                        For i As Integer = 1 To StartupDays.Count
                                            ' Si la fecha esta dentro del contrato activo
                                            If oEmployeeRow("BeginDate") <= StartupDays(i) And oEmployeeRow("EndDate") >= StartupDays(i) And StartupDays(i) >= xRecalcDate Then
                                                ' Actualizamos el Status de la tabla DailySchedule a 65 para cada una de la fechas a recalcular
                                                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                                         "WHERE Status >= 65 AND IDEmployee = " & oEmployeeRow("IDEmployee") & " AND " &
                                                               "Date = " & Any2Time(StartupDays(i)).SQLSmallDateTime

                                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                                If bolRet Then
                                                    strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                                             "@SELECT# Employees.ID, " & Any2Time(StartupDays(i)).SQLSmallDateTime & ", 65 " &
                                                             "FROM Employees " &
                                                             "WHERE Employees.ID = " & oEmployeeRow("IDEmployee") & " AND " &
                                                                   "Employees.ID NOT IN " &
                                                                   "(@SELECT# DS.IDEmployee " &
                                                                    "FROM DailySchedule DS " &
                                                                    "WHERE Date = " & Any2Time(StartupDays(i)).SQLSmallDateTime & ")"
                                                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                                                End If
                                                If Not bolRet Then Exit For

                                            End If
                                        Next

                                    End If

                                End If

                                If bolRet AndAlso (bolMustRecalcMaximumValue OrElse bolMustRecalcMinimumValue) Then

                                    ' Eliminamos todas las alertas que haya de este empleado en su contrato actual para ese saldo
                                    strSQL = "@DELETE# FROM sysroUserTasks " &
                                             "WHERE ResolverValue1 = '" & oEmployeeRow("IDEmployee").ToString & "' AND " &
                                                   "ResolverValue3 ='" & Any2String(oEmployeeRow("IDContract")).Replace("'", "''") & "' AND " &
                                                   "ResolverValue2 ='" & Me.IDConcept.ToString & "' "
                                    If bolMustRecalcMaximumValue Then
                                        bolRet = ExecuteSqlWithoutTimeOut(strSQL & " AND ResolverURL = 'FN:\\ExceededMaxValue'")
                                    End If

                                    If bolMustRecalcMinimumValue Then
                                        bolRet = ExecuteSqlWithoutTimeOut(strSQL & " AND ResolverURL = 'FN:\\ExceededMinValue'")
                                    End If

                                    ' Recalculamos la fecha de hoy
                                    ' Si la fecha esta dentro del contrato activo
                                    If oEmployeeRow("BeginDate") <= Now.Date And oEmployeeRow("EndDate") >= Now.Date And Now.Date >= xRecalcDate And bolRet Then
                                        ' Actualizamos el Status de la tabla DailySchedule a 65
                                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                                 "WHERE Status >= 65 AND IDEmployee = " & oEmployeeRow("IDEmployee") & " AND " &
                                                       "Date = " & Any2Time(Now.Date).SQLSmallDateTime

                                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                        If bolRet Then
                                            strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                                     "@SELECT# Employees.ID, " & Any2Time(Now.Date).SQLSmallDateTime & ", 65 " &
                                                     "FROM Employees " &
                                                     "WHERE Employees.ID = " & oEmployeeRow("IDEmployee") & " AND " &
                                                           "Employees.ID NOT IN " &
                                                           "(@SELECT# DS.IDEmployee " &
                                                            "FROM DailySchedule DS " &
                                                            "WHERE Date = " & Any2Time(Now.Date).SQLSmallDateTime & ")"
                                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                                        End If
                                        If Not bolRet Then Exit For
                                    End If
                                End If

                            Next
                            If bolMustRecalcStartValue AndAlso bolRet Then
                                ' Eliminamos todos los saldos iniciales futuros del saldo indicado en el valor inicial a partir de la fecha inidcada de los empleados del convenio
                                ExecuteSqlWithoutTimeOut("@DELETE# FROM DailyAccruals WHERE IDEmployee IN(" & strEmployees & ") AND IDConcept = " & Me.IDConcept.ToString & " AND Date >=" & Any2Time(xRecalcDate).SQLSmallDateTime & " AND StartupValue = 1 AND CarryOver = 1" & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailyAccruals.IDEmployee)  ")
                            End If

                            If Not bolRet Then
                                oState.Result = LabAgreeResultEnum.SqlError
                            End If
                        End If
                    Else ' No hay ningún contrato con un convenio asignado que contenga este valor inicial
                        ' ...
                    End If
                Else ' No es necesario recalcular nada
                    bolRet = True
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roStartupValue::Recalculate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::Recalculate")
            End Try

            Return bolRet

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim sRes As String = String.Empty
            Try
                sRes = Me.ScalingUserField & Me.ScalingCoefficientUserField
                For Each oScalingValue In Me.ScalingFieldValues
                    sRes = sRes & oScalingValue.UserField & oScalingValue.AccumValue
                Next
                If Me.Expiration IsNot Nothing Then
                    sRes = sRes & Me.Expiration.ExpireAfter.ToString & Me.Expiration.Unit.ToString
                End If
                If Me.Enjoyment IsNot Nothing Then
                    sRes = sRes & Me.Enjoyment.StartAfter.ToString & Me.Enjoyment.Unit.ToString
                End If
            Catch ex As Exception
            End Try
            Return sRes
        End Function

        Private Function IsStartupDay(ByVal xRecalcDate As Date, ByVal intYearIniMonth As Integer, ByVal intMonthIniDay As Integer, ByVal intWeekIniDay As Integer, ByVal strDefaultQuery As String, ByVal IDEmployee As Integer) As roCollection
            '
            ' Obtenemos el siguiente inicio de año/mes/contrato/año laboral
            '
            Dim bolRet As New roCollection
            Dim dbContractAnnualizedPeriods As DataTable = Nothing

            Try

                ' A partir de esta fecha marcamos todos los inicios de año/mes hasta la fecha de hoy
                Dim xActualDate As Date
                xActualDate = Any2Time(System.DateTime.Now).DateOnly

                Dim xDay As Date

                xDay = xRecalcDate

                While xDay <= xActualDate
                    Dim xBeginDate As DateTime
                    Dim xBeginYear As DateTime
                    If xDay.Month > intYearIniMonth Then
                        xBeginYear = New DateTime(xDay.Year, intYearIniMonth, intMonthIniDay)
                    ElseIf xDay.Month = intYearIniMonth And xDay.Day >= intMonthIniDay Then
                        xBeginYear = New DateTime(xDay.Year, intYearIniMonth, intMonthIniDay)
                    Else
                        xBeginYear = New DateTime(xDay.Year - 1, intYearIniMonth, intMonthIniDay)
                    End If

                    Dim xBeginMonth As DateTime
                    If xDay.Day > intMonthIniDay Then
                        'Si el dia es posterior al inicio del periodo (mismo mes)
                        xBeginMonth = New Date(xDay.Year, xDay.Month, intMonthIniDay)
                    ElseIf xDay.Day < intMonthIniDay Then
                        'Si el dia es anterior al inicio del periodo (mes anterior)
                        xBeginMonth = New Date(xDay.AddMonths(-1).Year, xDay.AddMonths(-1).Month, intMonthIniDay)
                    Else
                        'Si es el mismo dia
                        xBeginMonth = xDay
                    End If

                    If strDefaultQuery = "Y" Then
                        xBeginDate = xBeginYear
                    ElseIf strDefaultQuery = "M" Then
                        xBeginDate = xBeginMonth
                    ElseIf strDefaultQuery = "W" Then
                        'Semanal
                        Dim iDayOfWeek As Integer = xDay.DayOfWeek
                        If iDayOfWeek = 0 Then iDayOfWeek = 7
                        If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7
                        xBeginDate = xDay.AddDays(intWeekIniDay - iDayOfWeek)
                    End If

                    ' Comprobamos si la fecha es la de inicio de año/mes
                    If Any2Time(xDay).NumericValue = Any2Time(xBeginDate).NumericValue Then
                        bolRet.Add(bolRet.Count + 1, xDay)
                    End If
                    xDay = xDay.AddDays(1)
                End While

                ' Contrato Anualizado (lo hago fuera del bucle de días, porque es innecesario y costoso)
                If strDefaultQuery = "L" Then
                    Dim strSQL = $"@SELECT# BeginPeriod , EndPeriod From dbo.sysfnEmployeesAnnualWorkPeriods({IDEmployee})  
                                   WHERE BeginPeriod >= {roTypes.Any2Time(xRecalcDate).SQLSmallDateTime} 
                                   ORDER BY BeginPeriod ASC"
                    dbContractAnnualizedPeriods = CreateDataTable(strSQL)
                    If dbContractAnnualizedPeriods IsNot Nothing AndAlso dbContractAnnualizedPeriods.Rows.Count > 0 Then
                        For Each oRow As DataRow In dbContractAnnualizedPeriods.Rows
                            If oRow("BeginPeriod") <= xActualDate Then bolRet.Add(bolRet.Count + 1, Any2Time(oRow("BeginPeriod")).Value)
                        Next
                    End If
                End If


            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roStartupValue::IsStartupDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roStartupValue::IsStartupDay")
            Finally
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract()>
    <Serializable()>
    Public Class roStartupValueExpirationRule
        Private intExpiration As Integer = 0
        Private eExpirationUnit As LabAgreeStartupValueExpirationUnit = LabAgreeStartupValueExpirationUnit.Day

        <DataMember()>
        Public Property ExpireAfter() As Integer
            Get
                Return Me.intExpiration
            End Get
            Set(ByVal value As Integer)
                Me.intExpiration = value
            End Set
        End Property

        <DataMember()>
        Public Property Unit() As LabAgreeStartupValueExpirationUnit
            Get
                Return Me.eExpirationUnit
            End Get
            Set(ByVal value As LabAgreeStartupValueExpirationUnit)
                Me.eExpirationUnit = value
            End Set
        End Property

    End Class

    <DataContract()>
    <Serializable()>
    Public Class roStartupValueEnjoymentRule
        Private intStartEnjoyment As Integer = 0
        Private eEnjoymentUnit As LabAgreeStartupValueEnjoymentUnit = LabAgreeStartupValueEnjoymentUnit.Day

        <DataMember()>
        Public Property StartAfter() As Integer
            Get
                Return Me.intStartEnjoyment
            End Get
            Set(ByVal value As Integer)
                Me.intStartEnjoyment = value
            End Set
        End Property

        <DataMember()>
        Public Property Unit() As LabAgreeStartupValueEnjoymentUnit
            Get
                Return Me.eEnjoymentUnit
            End Get
            Set(ByVal value As LabAgreeStartupValueEnjoymentUnit)
                Me.eEnjoymentUnit = value
            End Set
        End Property
    End Class


End Namespace