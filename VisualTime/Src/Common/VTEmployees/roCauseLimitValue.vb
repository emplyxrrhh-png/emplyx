Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base
Imports Robotics.VTBase.Extensions

Namespace LabAgree

    <DataContract()>
    <Serializable>
    Public Class roCauseLimitValue

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intIDCauseLimitValue As Integer

        Private intIDCause As Integer
        Private intOriginalIDCause As Integer
        Private strName As String

        Private intIDExcessCause As Integer

        Private oMaximumAnnualValueType As LabAgreeValueType = LabAgreeValueType.None
        Private dblMaximumAnnualValue As Double
        Private strMaximumAnnualValueField As roUserField

        Private oMaximumMonthlyType As LabAgreeValueType = LabAgreeValueType.None
        Private dblMaximumMonthlyValue As Double
        Private strMaximumMonthlyField As roUserField

        Public Sub New()
            Me.oState = New roLabAgreeState
            Me.intIDCauseLimitValue = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intIDCauseLimitValue = _ID
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
        Public Property IDCauseLimitValue() As Integer
            Get
                Return Me.intIDCauseLimitValue
            End Get
            Set(ByVal value As Integer)
                Me.intIDCauseLimitValue = value
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
        Public Property IDExcessCause() As Integer
            Get
                Return Me.intIDExcessCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDExcessCause = value
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
        Public Property MaximumAnnualValueType() As LabAgreeValueType
            Get
                Return Me.oMaximumAnnualValueType
            End Get
            Set(ByVal value As LabAgreeValueType)
                Me.oMaximumAnnualValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumAnnualValue() As Double
            Get
                Return Me.dblMaximumAnnualValue
            End Get
            Set(ByVal value As Double)
                Me.dblMaximumAnnualValue = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumAnnualField() As roUserField
            Get
                Return Me.strMaximumAnnualValueField
            End Get
            Set(ByVal value As roUserField)
                Me.strMaximumAnnualValueField = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumMonthlyType() As LabAgreeValueType
            Get
                Return Me.oMaximumMonthlyType
            End Get
            Set(ByVal value As LabAgreeValueType)
                Me.oMaximumMonthlyType = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumMonthlyValue() As Double
            Get
                Return Me.dblMaximumMonthlyValue
            End Get
            Set(ByVal value As Double)
                Me.dblMaximumMonthlyValue = value
            End Set
        End Property

        <DataMember()>
        Public Property MaximumMonthlyField() As roUserField
            Get
                Return Me.strMaximumMonthlyField
            End Get
            Set(ByVal value As roUserField)
                Me.strMaximumMonthlyField = value
            End Set
        End Property

        <DataMember()>
        Public Property OriginalIDCause() As Integer
            Get
                Return Me.intOriginalIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intOriginalIDCause = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM CauseLimitValues " &
                                       "WHERE IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)
                    intIDCauseLimitValue = Any2Integer(oRow("IDCauseLimitValue"))
                    intOriginalIDCause = Any2Integer(oRow("IDCause"))
                    IDCause = Any2Integer(oRow("IDCause"))
                    strName = Any2String(oRow("Name"))
                    IDExcessCause = Any2Integer(oRow("IDExcessCause"))

                    If oRow("MaximumAnnualValueType") IsNot DBNull.Value Then
                        Select Case oRow("MaximumAnnualValueType")
                            Case 0 'None
                                oMaximumAnnualValueType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oMaximumAnnualValueType = LabAgreeValueType.DirectValue
                                dblMaximumAnnualValue = Any2Double(roTypes.Any2String(oRow("MaximumAnnualValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oMaximumAnnualValueType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFStartup As New roUserField(oUFState, oRow("MaximumAnnualValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    strMaximumAnnualValueField = oUFStartup
                                End If
                        End Select
                    Else
                        oMaximumAnnualValueType = LabAgreeValueType.None
                    End If

                    If oRow("MaximumMonthlyType") IsNot DBNull.Value Then
                        Select Case oRow("MaximumMonthlyType")
                            Case 0 'None
                                oMaximumMonthlyType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oMaximumMonthlyType = LabAgreeValueType.DirectValue
                                dblMaximumMonthlyValue = Any2Double(roTypes.Any2String(oRow("MaximumMonthlyValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oMaximumMonthlyType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFMaximumMonth As New roUserField(oUFState, oRow("MaximumMonthlyValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    strMaximumMonthlyField = oUFMaximumMonth
                                End If
                        End Select
                    Else
                        oMaximumMonthlyType = LabAgreeValueType.None
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCauseLimitValue, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal IDLabAgree As Integer) As Boolean

            Dim bolRet As Boolean = True

            Try

                Return bolRet
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Validate")
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

                    Dim oOldCauseLimitValue As roCauseLimitValue = Nothing

                    Dim tb As New DataTable("CauseLimitValues")
                    Dim strSQL As String = "@SELECT# * FROM CauseLimitValues " &
                                           "WHERE IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                    Else
                        oOldCauseLimitValue = New roCauseLimitValue(Me.intIDCauseLimitValue, Me.oState, False) ' Obtengo una còpia del StartupValue actual, para poder recalcular
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("IDCause") = Me.IDCause
                    oRow("IDExcessCause") = Me.IDExcessCause

                    oRow("Name") = strName
                    oRow("MaximumAnnualValueType") = oMaximumAnnualValueType

                    Select Case oMaximumAnnualValueType
                        Case LabAgreeValueType.None
                            oRow("MaximumAnnualValue") = ""
                        Case LabAgreeValueType.DirectValue
                            oRow("MaximumAnnualValue") = dblMaximumAnnualValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Case LabAgreeValueType.UserField
                            oRow("MaximumAnnualValue") = strMaximumAnnualValueField.FieldName.ToString
                    End Select

                    oRow("MaximumMonthlyType") = oMaximumMonthlyType
                    Select Case oMaximumMonthlyType
                        Case LabAgreeValueType.None
                            oRow("MaximumMonthlyValue") = ""
                        Case LabAgreeValueType.DirectValue
                            oRow("MaximumMonthlyValue") = dblMaximumMonthlyValue.ToString.Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Case LabAgreeValueType.UserField
                            oRow("MaximumMonthlyValue") = strMaximumMonthlyField.FieldName.ToString
                    End Select

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If

                    da.Update(tb)

                    If Me.intIDCauseLimitValue <= 0 Then
                        Dim tmpStartupValues As DataTable = CreateDataTable("@SELECT# top 1 IDCauseLimitValue  FROM CauseLimitValues Order by IDCauseLimitValue desc ", )
                        If tmpStartupValues IsNot Nothing AndAlso tmpStartupValues.Rows.Count = 1 Then
                            Me.intIDCauseLimitValue = Any2Integer(tmpStartupValues.Rows(0)("IDCauseLimitValue"))
                        End If
                    End If

                    bolRet = True

                    oAuditDataNew = oRow

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tCauseLimitValue, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Save")
            Finally

            End Try

            Return bolRet

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

        ''' <summary>
        ''' Borra los valores iniciales del acumulado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Not Me.IsUsed(IDLabAgree) Then

                    bolRet = False

                    Dim DelQuerys() As String = {"@DELETE# FROM CauseLimitValues WHERE IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = LabAgreeResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                    If bolRet Then
                        ' Notificamos el cambio al servidor
                        bolRet = Me.Recalculate(Nothing)
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCauseLimitValue, Me.strName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Delete")
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

                Dim strSQL As String
                Dim tb As DataTable
                Dim strUseConcept As String = ""

                Dim strUsedLabAgree As String = ""
                If Not bolIsUsed Then
                    ' Convenios - Valores iniciales
                    ' Verifica que la justificacion no se esté usando en el mismo convenio por otro limite
                    strSQL = "@SELECT# la.Name From LabAGree la inner join LabAgreeCauseLimitValues lacv on la.ID = lacv.IDLabAgree inner join CauseLimitValues clv on clv.IDCauseLimitValue = lacv.IDCauseLimitValue Where clv.IDCause =" & Me.intIDCause & " AND la.ID =" & IDLabAgree
                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        strUsedLabAgree = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los empleados que lo usan
                            strUsedLabAgree &= "," & oRow("Name")
                        Next
                        If strUsedLabAgree <> "" Then strUsedLabAgree = strUsedLabAgree.Substring(1)
                        If strUsedLabAgree <> "" Then
                            oState.Result = LabAgreeResultEnum.StartupValueUsedInLabAgree
                            bolIsUsed = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::IsUsed")
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
        Public Function Recalculate(ByVal oOldStartupValue As roCauseLimitValue, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifDate As Date = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Recalculate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseLimitValue::Recalculate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace