Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base
Imports Robotics.Base.VTBusiness.Cause
Imports DocumentFormat.OpenXml.VariantTypes
Imports System.Web
Imports System.Web.UI

Namespace Concept

    <DataContract>
    Public Class roConcept

#Region "Declarations - Constructor"

        Private oState As roConceptState

        Private intID As Integer
        Private strName As String = ""
        Private strDescription As String
        Private intColor As Integer
        Private strIDType As String
        Private strShortName As String
        Private datBeginDate As DateTime
        Private datFinishDate As DateTime
        Private bolViewInEmployees As Boolean
        Private bolViewInTerminals As Nullable(Of Boolean)
        Private bolViewInPays As Nullable(Of Boolean)
        Private bolFixedPay As Nullable(Of Boolean)
        Private dblPayValue As Nullable(Of Double)
        Private strUsedField As String
        Private dblRoundingBy As Nullable(Of Double)
        Private strExport As String
        Private strDefaultQuery As String
        Private bolIsExported As Nullable(Of Boolean)
        Private bolIsIntervaled As Nullable(Of Boolean)
        Private dblRoundConceptBy As Nullable(Of Double)
        Private oRoundConveptType As eRoundingType
        Private bolIsAbsentiism As Nullable(Of Boolean)
        Private bolAbsentiismRewarded As Nullable(Of Boolean)
        Private bolIsAccrualWork As Nullable(Of Boolean)
        Private oComposition As New Generic.List(Of roConceptComposition)

        Private intEmployeesPermission As Integer                                          ' Quién puede consultar el saldo: 0- Todos, 1- Nadie, 2- Según criterio en campo de la ficha
        Private lstEmployeesConditions As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)   ' Lista de condiciones según campo de la ficha

        Private intAutomaticAccrualType As eAutomaticAccrualType
        Private oAutomaticAccrualCriteria As New roAutomaticAccrualCriteria
        Private intAutomaticAccrualIDCause As Integer

        Private bolCustomType As Nullable(Of Boolean)

        Private bApplyOnHolidaysRequest As Nullable(Of Boolean)

        Private bolApplyExpiredHours As Nullable(Of Boolean)
        Private iDailyRecordMargin As Nullable(Of Integer)

        Private bAutoApproveRequestsDR As Boolean

        Private oExpiredHoursCriteria As New roExpiredHoursCriteria
        Private intExpiredIDCause As Integer
        Private dblValue As Double
        Private dblPositiveValue As Double
        Private dblNegativeValue As Double

        Private Const MAX_NUMBER_OF_CONCEPTS_IN_EXPRESS As Byte = 39

        Public Sub New()
            Me.oState = New roConceptState
            Me.ID = -1
            Me.intEmployeesPermission = 0
            Me.lstEmployeesConditions = New Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Me.intAutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
            Me.intAutomaticAccrualIDCause = 0
            Me.intExpiredIDCause = 0
            Me.oAutomaticAccrualCriteria = New roAutomaticAccrualCriteria
            Me.oExpiredHoursCriteria = New roExpiredHoursCriteria
            Me.oComposition = New List(Of roConceptComposition)

        End Sub

        Public Sub New(ByVal _IDConcept As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.ID = _IDConcept
            Me.intEmployeesPermission = 0
            Me.intAutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
            Me.intAutomaticAccrualIDCause = 0
            Me.intExpiredIDCause = 0
            Me.oAutomaticAccrualCriteria = New roAutomaticAccrualCriteria
            Me.lstEmployeesConditions = New Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Me.oExpiredHoursCriteria = New roExpiredHoursCriteria
            Me.oComposition = New List(Of roConceptComposition)
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roConceptState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roConceptState)
                Me.oState = value
                If Me.oComposition IsNot Nothing Then
                    For Each oComposition As roConceptComposition In Me.oComposition
                        oComposition.State = Me.oState
                    Next
                End If
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                If Me.oComposition IsNot Nothing Then
                    For Each oComp As roConceptComposition In Me.oComposition
                        oComp.IDConcept = Me.intID
                    Next
                End If
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        <DataMember>
        Public Property Color() As Integer
            Get
                Return intColor
            End Get
            Set(ByVal value As Integer)
                intColor = value
            End Set
        End Property

        <DataMember>
        Public Property IDType() As String
            Get
                Return strIDType
            End Get
            Set(ByVal value As String)
                strIDType = value
            End Set
        End Property

        <DataMember>
        Public Property ShortName() As String
            Get
                Return strShortName
            End Get
            Set(ByVal value As String)
                strShortName = value
            End Set
        End Property

        <DataMember>
        Public Property BeginDate() As DateTime
            Get
                Return datBeginDate
            End Get
            Set(ByVal value As DateTime)
                datBeginDate = value
            End Set
        End Property

        <DataMember>
        Public Property Value() As Double
            Get
                Return dblValue
            End Get
            Set(ByVal value As Double)
                dblValue = value
            End Set
        End Property

        <DataMember>
        Public Property FinishDate() As DateTime
            Get
                Return datFinishDate
            End Get
            Set(ByVal value As DateTime)
                datFinishDate = value
            End Set
        End Property

        <DataMember>
        Public Property ViewInEmployees() As Boolean
            Get
                Return bolViewInEmployees
            End Get
            Set(ByVal value As Boolean)
                bolViewInEmployees = value
            End Set
        End Property

        <DataMember>
        Public Property ViewInTerminals() As Nullable(Of Boolean)
            Get
                Return bolViewInTerminals
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolViewInTerminals = value
            End Set
        End Property

        <DataMember>
        Public Property ViewInPays() As Nullable(Of Boolean)
            Get
                Return bolViewInPays
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolViewInPays = value
            End Set
        End Property

        <DataMember>
        Public Property FixedPay() As Nullable(Of Boolean)
            Get
                Return bolFixedPay
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolFixedPay = value
            End Set
        End Property

        <DataMember>
        Public Property PayValue() As Nullable(Of Double)
            Get
                Return dblPayValue
            End Get
            Set(ByVal value As Nullable(Of Double))
                dblPayValue = value
            End Set
        End Property

        <DataMember>
        Public Property UsedField() As String
            Get
                Return strUsedField
            End Get
            Set(ByVal value As String)
                strUsedField = value
            End Set
        End Property

        <DataMember>
        Public Property RoundingBy() As Nullable(Of Double)
            Get
                Return dblRoundingBy
            End Get
            Set(ByVal value As Nullable(Of Double))
                dblRoundingBy = value
            End Set
        End Property

        <DataMember>
        Public Property Export() As String
            Get
                Return strExport
            End Get
            Set(ByVal value As String)
                strExport = value
            End Set
        End Property

        <DataMember>
        Public Property DefaultQuery() As String
            Get
                Return strDefaultQuery
            End Get
            Set(ByVal value As String)
                strDefaultQuery = value
            End Set
        End Property

        <DataMember>
        Public Property IsExported() As Nullable(Of Boolean)
            Get
                Return bolIsExported
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolIsExported = value
            End Set
        End Property

        <DataMember>
        Public Property IsIntervaled() As Nullable(Of Boolean)
            Get
                Return bolIsIntervaled
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolIsIntervaled = value
            End Set
        End Property

        <DataMember>
        Public Property RoundConceptBy() As Nullable(Of Double)
            Get
                Return dblRoundConceptBy
            End Get
            Set(ByVal value As Nullable(Of Double))
                dblRoundConceptBy = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticAccrualType() As eAutomaticAccrualType
            Get
                Return intAutomaticAccrualType
            End Get
            Set(ByVal value As eAutomaticAccrualType)
                intAutomaticAccrualType = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticAccrualCriteria() As roAutomaticAccrualCriteria
            Get
                Return oAutomaticAccrualCriteria
            End Get
            Set(ByVal value As roAutomaticAccrualCriteria)
                oAutomaticAccrualCriteria = value
            End Set
        End Property

        <DataMember>
        Public Property ExpiredHoursCriteria() As roExpiredHoursCriteria
            Get
                Return oExpiredHoursCriteria
            End Get
            Set(ByVal value As roExpiredHoursCriteria)
                oExpiredHoursCriteria = value
            End Set
        End Property

        <DataMember>
        Public Property RoundConveptType() As eRoundingType
            Get
                Return oRoundConveptType
            End Get
            Set(ByVal value As eRoundingType)
                oRoundConveptType = value
            End Set
        End Property

        <DataMember>
        Public Property IsAbsentiism() As Nullable(Of Boolean)
            Get
                Return bolIsAbsentiism
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolIsAbsentiism = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyExpiredHours() As Nullable(Of Boolean)
            Get
                Return bolApplyExpiredHours
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolApplyExpiredHours = value
            End Set
        End Property

        <DataMember>
        Public Property AbsentiismRewarded() As Nullable(Of Boolean)
            Get
                Return bolAbsentiismRewarded
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolAbsentiismRewarded = value
            End Set
        End Property

        <DataMember>
        Public Property IsAccrualWork() As Nullable(Of Boolean)
            Get
                Return bolIsAccrualWork
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolIsAccrualWork = value
            End Set
        End Property

        <DataMember>
        Public Property CustomType() As Nullable(Of Boolean)
            Get
                Return bolCustomType
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolCustomType = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyOnHolidaysRequest() As Nullable(Of Boolean)
            Get
                Return bApplyOnHolidaysRequest
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bApplyOnHolidaysRequest = value
            End Set
        End Property

        <DataMember>
        Public Property DailyRecordMargin() As Nullable(Of Integer)
            Get
                Return iDailyRecordMargin
            End Get
            Set(ByVal value As Nullable(Of Integer))
                iDailyRecordMargin = value
            End Set
        End Property

        <DataMember>
        Public Property AutoApproveRequestsDR() As Boolean
            Get
                Return bAutoApproveRequestsDR
            End Get
            Set(ByVal value As Boolean)
                bAutoApproveRequestsDR = value
            End Set
        End Property

        <DataMember>
        Public Property Composition() As Generic.List(Of roConceptComposition)
            Get
                Return Me.oComposition
            End Get
            Set(ByVal value As Generic.List(Of roConceptComposition))
                Me.oComposition = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeesPremission() As Integer
            Get
                Return Me.intEmployeesPermission
            End Get
            Set(ByVal value As Integer)
                Me.intEmployeesPermission = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeesConditions() As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Get
                Return Me.lstEmployeesConditions
            End Get
            Set(ByVal value As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition))
                Me.lstEmployeesConditions = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticAccrualIDCause() As Integer
            Get
                Return Me.intAutomaticAccrualIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intAutomaticAccrualIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property ExpiredIDCause() As Integer
            Get
                Return Me.intExpiredIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intExpiredIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property PositiveValue() As Double
            Get
                Return Me.dblPositiveValue
            End Get
            Set(ByVal value As Double)
                Me.dblPositiveValue = value
            End Set
        End Property

        <DataMember>
        Public Property NegativeValue() As Double
            Get
                Return Me.dblNegativeValue
            End Get
            Set(ByVal value As Double)
                Me.dblNegativeValue = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Concepts WHERE ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.intColor = IIf(Not IsDBNull(oRow("Color")), oRow("Color"), 0)
                    Me.strIDType = Any2String(oRow("IDType"))
                    Me.strShortName = Any2String(oRow("ShortName"))
                    Me.datBeginDate = oRow("BeginDate")
                    Me.datFinishDate = oRow("FinishDate")
                    Me.bolViewInEmployees = oRow("ViewInEmployees")
                    If Not IsDBNull(oRow("ViewInTerminals")) Then
                        Me.bolViewInTerminals = oRow("ViewInTerminals")
                    End If
                    If Not IsDBNull(oRow("ViewInPays")) Then
                        Me.bolViewInPays = oRow("ViewInPays")
                    End If
                    If Not IsDBNull(oRow("FixedPay")) Then
                        Me.bolFixedPay = oRow("FixedPay")
                    End If
                    If Not IsDBNull(oRow("PayValue")) Then
                        Me.dblPayValue = CDbl(oRow("PayValue"))
                    End If
                    Me.strUsedField = Any2String(oRow("UsedField"))
                    If Not IsDBNull(oRow("RoundingBy")) Then
                        Me.dblRoundingBy = CDbl(oRow("RoundingBy"))
                    End If
                    If Not IsDBNull(oRow("Export")) Then
                        Me.strExport = oRow("Export")
                    End If
                    Me.strDefaultQuery = Any2String(oRow("DefaultQuery"))
                    If Not IsDBNull(oRow("IsExported")) Then
                        Me.bolIsExported = oRow("IsExported")
                    End If
                    If Not IsDBNull(oRow("IsIntervaled")) Then
                        Me.bolIsIntervaled = oRow("IsIntervaled")
                    End If
                    If Not IsDBNull(oRow("RoundConceptBy")) Then
                        Me.dblRoundConceptBy = CDbl(oRow("RoundConceptBy"))
                    End If
                    Select Case Any2String(oRow("RoundConceptType"))
                        Case "+"
                            Me.oRoundConveptType = eRoundingType.Round_UP
                        Case "~"
                            Me.oRoundConveptType = eRoundingType.Round_Near
                        Case "-"
                            Me.oRoundConveptType = eRoundingType.Round_Down
                    End Select
                    If Not IsDBNull(oRow("IsAbsentiism")) Then
                        Me.bolIsAbsentiism = oRow("IsAbsentiism")
                    End If
                    If Not IsDBNull(oRow("AbsentiismRewarded")) Then
                        Me.bolAbsentiismRewarded = oRow("AbsentiismRewarded")
                    End If
                    If Not IsDBNull(oRow("IsAccrualWork")) Then
                        Me.bolIsAccrualWork = oRow("IsAccrualWork")
                    End If
                    Me.intEmployeesPermission = Any2Integer(oRow("EmployeesPermission"))

                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Me.lstEmployeesConditions = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(Any2String(oRow("EmployeesCriteria")), oUserFieldState, False)
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)

                    Me.oComposition = roConceptComposition.GetConceptCompositions(Me.ID, Me.oState, bAudit)

                    Me.ApplyExpiredHours = False
                    If Not IsDBNull(oRow("ApplyExpiredHours")) Then
                        Me.ApplyExpiredHours = oRow("ApplyExpiredHours")
                    End If

                    Me.ExpiredIDCause = Any2Integer(oRow("ExpiredIDCause"))
                    If Me.ApplyExpiredHours Then
                        Me.oExpiredHoursCriteria = New roExpiredHoursCriteria
                        Me.oExpiredHoursCriteria.LoadFromXml(Any2String(oRow("ExpiredHoursCriteria")))
                    End If

                    Me.AutomaticAccrualType = Any2Integer(oRow("AutomaticAccrualType"))
                    Me.AutomaticAccrualIDCause = Any2Integer(oRow("AutomaticAccrualIDCause"))
                    If Me.AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType Then
                        Me.oAutomaticAccrualCriteria = New roAutomaticAccrualCriteria
                        Me.oAutomaticAccrualCriteria.AutomaticAccrualType = Me.AutomaticAccrualType
                        Me.oAutomaticAccrualCriteria.LoadFromXml(Any2String(oRow("AutomaticAccrualCriteria")))
                    End If

                    If Not IsDBNull(oRow("CustomType")) Then
                        Me.bolCustomType = oRow("CustomType")
                    End If

                    If Not IsDBNull(oRow("ApplyOnHolidaysRequest")) Then
                        Me.bApplyOnHolidaysRequest = oRow("ApplyOnHolidaysRequest")
                    End If

                    If Not IsDBNull(oRow("DailyRecordMargin")) Then
                        Me.iDailyRecordMargin = oRow("DailyRecordMargin")
                    End If

                    If Not IsDBNull(oRow("AutoApproveRequestsDR")) Then
                        Me.bAutoApproveRequestsDR = oRow("AutoApproveRequestsDR")
                    End If

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tConcept, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roConcept::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Load")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nuevo acumulado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM Concepts"
            Dim tb As DataTable = CreateDataTable(strSQL)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True
            Dim NumReg As Long = 0
            Dim strSQL As String

            Try
                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = ConceptResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                If bolRet And bolCheckNames Then
                    ' Compuebo que el nombre no exista
                    strSQL = "@SELECT# Count(*) FROM Concepts WHERE Name = '" & Me.Name & "' AND ID <> " & Me.ID.ToString
                    NumReg = ExecuteScalar(strSQL)
                    If NumReg > 0 Then
                        oState.Result = ConceptResultEnum.NameAlreadyExist
                        bolRet = False
                    End If
                End If

                If bolRet And bolCheckNames Then
                    ' Compruebo que la abreviatura no exista
                    strSQL = "@SELECT# Count(*) FROM Concepts WHERE ShortName = '" & Me.ShortName & "' AND ID <> " & Me.ID.ToString & " AND FinishDate >= getdate()"
                    NumReg = ExecuteScalar(strSQL)
                    If NumReg > 0 Then
                        oState.Result = ConceptResultEnum.ShortNameAlreadyExist
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    ' Compruebo que el intervalo de fechas sea el correcto
                    If Me.BeginDate > Me.FinishDate Then
                        oState.Result = ConceptResultEnum.InvalidDateInterval
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    ' Compruebo que no haya seleccionados cuatro concepts para terminal
                    If Me.ViewInTerminals.HasValue AndAlso Me.ViewInTerminals Then
                        strSQL = "@SELECT# Count(*) FROM Concepts WHERE ViewInTerminals = 1 AND Id <> " & Me.ID.ToString
                        NumReg = ExecuteScalar(strSQL)
                        If NumReg >= 4 Then
                            oState.Result = ConceptResultEnum.TooManyTerminals
                            bolRet = False
                        End If
                    End If
                End If

                If bolRet Then
                    ' Compruebo que me hayan informado el campo de userfield
                    If Me.ViewInPays.HasValue AndAlso Me.ViewInPays.Value Then
                        If Not Me.FixedPay.HasValue OrElse Not Me.FixedPay.Value Then
                            If Me.UsedField = "" Then
                                oState.Result = ConceptResultEnum.InvalidUsedField
                                bolRet = False
                            End If
                        End If
                    End If
                End If

                If bolRet Then
                    ' Compruebo la composición del acumulado
                    bolRet = roConceptComposition.ValidateConceptCompositions(Me.oComposition, Me.oState)
                End If

                If bolRet Then
                    ' Compruebo que no se encuentre marcado en otro acumulado el saldo de horas trabajadas
                    If Me.IsAccrualWork = True Then
                        strSQL = "@SELECT# Count(*) FROM Concepts WHERE IsAccrualWork = 1 AND Id <> " & Me.ID.ToString
                        NumReg = ExecuteScalar(strSQL)
                        If NumReg > 0 Then
                            oState.Result = ConceptResultEnum.AccrualWorkExists
                            bolRet = False
                        End If
                    End If
                End If

                ' Compruebo la definicion de los devengos
                If bolRet Then
                    If Me.CustomType.HasValue AndAlso Me.CustomType Then
                        Me.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                    End If

                    If Me.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType Then
                        Me.AutomaticAccrualIDCause = 0
                        Me.AutomaticAccrualCriteria = New roAutomaticAccrualCriteria
                    Else
                        If Me.AutomaticAccrualCriteria Is Nothing Then
                            bolRet = False
                        ElseIf Me.AutomaticAccrualCriteria.FactorType = eFactorType.UserField Then
                            If Me.AutomaticAccrualCriteria.UserField Is Nothing OrElse Me.AutomaticAccrualCriteria.UserField.FieldName.Length = 0 Then
                                bolRet = False
                            End If
                        End If

                        If Me.AutomaticAccrualIDCause = 0 Then
                            bolRet = False
                        Else
                            If Me.oComposition IsNot Nothing AndAlso Me.oComposition.Count > 0 Then
                                For Each oComposition As roConceptComposition In Me.oComposition
                                    If oComposition.IDType = CompositionType.Cause AndAlso oComposition.IDCause = Me.AutomaticAccrualIDCause Then bolRet = False
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If

                        If Me.AutomaticAccrualType = eAutomaticAccrualType.DaysType And Me.IDType = "H" Then
                            bolRet = False
                        End If

                        If bolRet Then
                            If Me.AutomaticAccrualType = eAutomaticAccrualType.HoursType Then
                                If Me.AutomaticAccrualCriteria.Causes Is Nothing OrElse Me.AutomaticAccrualCriteria.Causes.Count = 0 Then
                                    bolRet = False
                                End If
                            Else
                                If Me.AutomaticAccrualCriteria.TypeAccrualDay = eAccrualDayType.SomeDays And ((Me.AutomaticAccrualCriteria.Shifts Is Nothing OrElse Me.AutomaticAccrualCriteria.Shifts.Count = 0) And (Me.AutomaticAccrualCriteria.Causes Is Nothing OrElse Me.AutomaticAccrualCriteria.Causes.Count = 0)) Then
                                    bolRet = False
                                End If

                            End If
                        End If
                    End If

                    If Not bolRet Then oState.Result = ConceptResultEnum.AutomaticAccrualCriteriaDataError
                End If

                If bolRet Then
                    ' Verificamos si es se han marcado caducidades con un saldo que no sea por contrato
                    If bolApplyExpiredHours.HasValue AndAlso bolApplyExpiredHours Then
                        If Me.DefaultQuery <> "C" OrElse Me.ExpiredIDCause = 0 Then
                            oState.Result = ConceptResultEnum.ExpiredHoursDataError
                            bolRet = False
                        End If
                        If bolRet AndAlso Me.ExpiredHoursCriteria IsNot Nothing AndAlso Me.ExpiredHoursCriteria.oValue <= 0 Then
                            ' Se debe indicar un valor
                            oState.Result = ConceptResultEnum.ExpiredHoursDataError
                            bolRet = False
                        End If

                        If bolRet AndAlso Me.ID > 0 Then
                            ' en el caso que no sea nuevo
                            ' Debemos revisar que no hay valores inciales con un saldo marcado con caducidades
                            If Me.DefaultQuery = "C" Then
                                strSQL = "@SELECT# Count(*) FROM StartupValues WHERE IDConcept = " & Me.ID.ToString & " and isnull(StartValueType,0) > 0"
                                NumReg = ExecuteScalar(strSQL)
                                If NumReg > 0 Then
                                    oState.Result = ConceptResultEnum.ExpiredHoursDataError
                                    bolRet = False
                                End If
                            End If

                            If bolRet Then
                                ' Debemos revisar que no haya reglas de arrastres con este saldo inidcando la opcion MOVER
                                strSQL = "set arithabort on; " &
                                            "@SELECT# Count(*)  " &
                                            " FROM AccrualsRules "
                                strSQL &= " INNER JOIN LabAgreeAccrualsRules ON AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules "
                                strSQL &= " WHERE  CONVERT(xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""MainAccrual"")]/text())[1]', 'nvarchar(max)') = " & Me.ID.ToString
                                strSQL &= " and  CONVERT(xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""Action"")]/text())[1]', 'nvarchar(max)') = 0"
                                NumReg = ExecuteScalar(strSQL)
                                If NumReg > 0 Then
                                    oState.Result = ConceptResultEnum.ExpiredHoursDataError
                                    bolRet = False
                                End If

                            End If

                        End If

                        ' En el caso que el saldo tenga caducidad no se puede redondear, forzamos que no tengan redondeo
                        Me.RoundConceptBy = 1
                        Me.RoundConveptType = eRoundingType.Round_Near
                    Else
                        Me.ExpiredIDCause = 0
                        Me.ExpiredHoursCriteria = New roExpiredHoursCriteria
                    End If
                End If

                If bolRet AndAlso Me.ApplyOnHolidaysRequest.HasValue AndAlso Me.ApplyOnHolidaysRequest Then
                    ' Solo se puede marcar la opcion de saldo de solicitudes en el caso de saldos anuales, por contrato o mesuales (por horas o días)
                    If Not (Me.DefaultQuery = "Y" OrElse Me.DefaultQuery = "L" OrElse Me.DefaultQuery = "C" OrElse Me.DefaultQuery = "M") Then
                        oState.Result = ConceptResultEnum.DefaultQueryMustBeAnnual
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    Try
                        'OBTENER LICENCIA DE EXPRES PARA COMPROBAR SI PUEDE REALIZAR LA ACCION QUE REQUIERE LA VALIDACION
                        Dim oLicense As New roServerLicense
                        If oLicense.FeatureIsInstalled("Version\LiveExpress") Then
                            strSQL = "@SELECT# Count(*) FROM Concepts WHERE FinishDate >= " & Any2Time(DateTime.Today).SQLSmallDateTime
                            Dim Total As Long = ExecuteScalar(strSQL)
                            If Me.ID <= 0 Then
                                Total = Total + 1
                            End If
                            If Total > MAX_NUMBER_OF_CONCEPTS_IN_EXPRESS Then
                                oState.Result = ConceptResultEnum.NumberOfConceptsExceeded
                                bolRet = False
                            End If
                        End If
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roConcept::Validate")
                    End Try
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal xClosingDate As Date, ByVal bolDefinitionHasChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.ID > 0 Then
                    Dim oOldConcept As New roConcept(Me.ID, Me.oState)

                    If oOldConcept.IsAccrualWork.HasValue And oOldConcept.IsAccrualWork.Value = True Then
                        oOldConcept.IsAccrualWork = False
                    End If

                    ' Marca actual como obsoleto y lo guarda
                    Me.oState.Language.ClearUserTokens()
                    Me.oState.Language.AddUserToken(Any2String(oOldConcept.Name))
                    Dim _Name As String = Me.oState.Language.Translate("SaveFromData.OldNamePrefix", "")
                    oOldConcept.Name = _Name
                    oOldConcept.FinishDate = xClosingDate.AddDays(-1)
                    Dim n As Integer = 1
                    While Not oOldConcept.Validate()
                        If Me.oState.Result = ConceptResultEnum.NameAlreadyExist Then
                            oOldConcept.Name = _Name & n.ToString
                            Me.oState.Result = ConceptResultEnum.NoError
                        Else
                            If Me.oState.Result = ConceptResultEnum.ShortNameAlreadyExist Then
                                oOldConcept.ShortName = oOldConcept.ShortName.Substring(0, 2) & n.ToString
                                Me.oState.Result = ConceptResultEnum.NoError
                            Else
                                Exit While
                            End If
                        End If
                        n += 1
                    End While
                    bolRet = oOldConcept.Save(False)

                    If bolRet Then
                        'Quitamos el ID del concepto para generar un ID nuevo
                        Me.ID = -1
                        ' Actualizamos la nueva fecha de inicio del acumulado
                        Me.BeginDate = xClosingDate.Date

                        ' Grabamos el nuevo concepto
                        bolRet = Me.Save(bolDefinitionHasChanged, False, bAudit)

                        If bolRet Then
                            Dim oldID As Integer = oOldConcept.ID
                            Dim newID As Integer = Me.ID

                            bolRet = UpdateConceptUsedInServer(oldID, newID)
                        End If

                    End If
                Else
                    'Quitamos el ID del concepto para generar un ID nuevo
                    Me.ID = -1
                    ' Actualizamos la nueva fecha de inicio del acumulado
                    ' Me.BeginDate = xClosingDate

                    ' Grabamos el nuevo concepto
                    bolRet = Me.Save(bolDefinitionHasChanged, False, bAudit)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal bolDefinitionHasChanged As Boolean, Optional ByVal bolCheckNames As Boolean = True, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Dim bolNewConcept As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = ConceptResultEnum.XSSvalidationError
                    Return False
                End If

                Dim oConceptold As roConcept = Nothing
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate(bolCheckNames) Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Concepts")
                    Dim strSQL As String = "@SELECT# * FROM Concepts " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        Me.ID = oRow("ID")
                        oRow("Category") = ""
                        bolNewConcept = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)

                        ' En caso que sea una modificacion de un saldo ya existente
                        ' nos guardamos la composicion antigua
                        oConceptold = New roConcept(Me.ID, Me.State)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("Color") = Me.intColor
                    oRow("IDType") = Me.strIDType
                    oRow("ShortName") = Me.strShortName
                    oRow("BeginDate") = Me.datBeginDate
                    oRow("FinishDate") = Me.datFinishDate
                    oRow("ViewInEmployees") = Me.bolViewInEmployees
                    If Me.bolViewInTerminals.HasValue Then
                        oRow("ViewInTerminals") = Me.bolViewInTerminals
                    Else
                        oRow("ViewInTerminals") = DBNull.Value
                    End If
                    If Me.bolViewInPays.HasValue Then
                        oRow("ViewInPays") = Me.bolViewInPays
                    Else
                        oRow("ViewInPays") = DBNull.Value
                    End If
                    If Me.bolFixedPay.HasValue Then
                        oRow("FixedPay") = Me.bolFixedPay
                    Else
                        oRow("FixedPay") = DBNull.Value
                    End If
                    If Me.dblPayValue.HasValue Then
                        oRow("PayValue") = Me.dblPayValue
                    Else
                        oRow("PayValue") = DBNull.Value
                    End If
                    oRow("UsedField") = Any2String(Me.strUsedField)
                    If Me.dblRoundingBy.HasValue Then
                        oRow("RoundingBy") = Me.dblRoundingBy
                    Else
                        oRow("RoundingBy") = DBNull.Value
                    End If
                    If Me.strExport <> "" Then
                        oRow("Export") = Me.strExport
                    Else
                        oRow("Export") = DBNull.Value
                    End If
                    oRow("DefaultQuery") = Any2String(Me.strDefaultQuery)
                    If Me.bolIsExported.HasValue Then
                        oRow("IsExported") = Me.bolIsExported
                    Else
                        oRow("IsExported") = DBNull.Value
                    End If
                    If Me.bolIsIntervaled.HasValue Then
                        oRow("IsIntervaled") = Me.bolIsIntervaled
                    Else
                        oRow("IsIntervaled") = DBNull.Value
                    End If
                    If Me.dblRoundConceptBy.HasValue Then
                        oRow("RoundConceptBy") = Me.dblRoundConceptBy
                    Else
                        oRow("RoundConceptBy") = DBNull.Value
                    End If
                    Select Case Me.oRoundConveptType
                        Case eRoundingType.Round_UP
                            oRow("RoundConceptType") = "+"
                        Case eRoundingType.Round_Near
                            oRow("RoundConceptType") = "~"
                        Case eRoundingType.Round_Down
                            oRow("RoundConceptType") = "-"
                    End Select
                    If Me.bolIsAbsentiism.HasValue Then
                        oRow("IsAbsentiism") = Me.bolIsAbsentiism
                    Else
                        oRow("IsAbsentiism") = DBNull.Value
                    End If
                    If Me.bolAbsentiismRewarded.HasValue Then
                        oRow("AbsentiismRewarded") = Me.bolAbsentiismRewarded
                    Else
                        oRow("AbsentiismRewarded") = DBNull.Value
                    End If

                    If Me.bolIsAccrualWork.HasValue Then
                        oRow("IsAccrualWork") = Me.bolIsAccrualWork
                    Else
                        oRow("IsAccrualWork") = DBNull.Value
                    End If

                    If Me.bolCustomType.HasValue Then
                        oRow("CustomType") = Me.bolCustomType
                        If Me.bolCustomType Then
                            Me.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType

                        End If
                    Else
                        oRow("CustomType") = DBNull.Value
                    End If

                    If Me.bApplyOnHolidaysRequest.HasValue Then
                        oRow("ApplyOnHolidaysRequest") = Me.bApplyOnHolidaysRequest
                    Else
                        oRow("ApplyOnHolidaysRequest") = 0
                    End If

                    If Me.iDailyRecordMargin IsNot Nothing Then
                        oRow("DailyRecordMargin") = Me.iDailyRecordMargin
                    Else
                        oRow("DailyRecordMargin") = DBNull.Value
                    End If

                    If Me.bAutoApproveRequestsDR Then
                        oRow("AutoApproveRequestsDR") = 1
                    Else
                        oRow("AutoApproveRequestsDR") = 0
                    End If

                    oRow("EmployeesPermission") = Me.intEmployeesPermission
                    oRow("EmployeesCriteria") = VTUserFields.UserFields.roUserFieldCondition.GetXml(Me.lstEmployeesConditions)

                    oRow("AutomaticAccrualType") = Me.AutomaticAccrualType
                    oRow("AutomaticAccrualIDCause") = IIf(AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType, Me.AutomaticAccrualIDCause, 0)

                    oRow("AutomaticAccrualCriteria") = Me.oAutomaticAccrualCriteria.GetXml

                    If Me.ApplyExpiredHours.HasValue AndAlso Me.ApplyExpiredHours Then
                        oRow("ApplyExpiredHours") = True
                        oRow("ExpiredIDCause") = Me.ExpiredIDCause
                    Else
                        oRow("ApplyExpiredHours") = False
                        oRow("ExpiredIDCause") = 0

                    End If

                    oRow("ExpiredHoursCriteria") = Me.ExpiredHoursCriteria.GetXml

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    bolRet = True

                    ' En caso que sea una modificacion de un saldo ya existente
                    ' nos guardamos la composicion antigua
                    If Not bolNewConcept Then
                        ' Comprobamos si han cambiado el tipo de saldo
                        If oRow("IDType") <> oAuditDataOld("IDType") Then
                            bolDefinitionHasChanged = True
                        End If
                    End If

                    ' Actualizamos la composición del acumulado
                    If Me.oComposition IsNot Nothing AndAlso Me.oComposition.Count > 0 Then
                        bolRet = roConceptComposition.SaveConceptCompositions(Me.oComposition, Me.oState)
                    Else
                        bolRet = roConceptComposition.DeleteConceptCompositions(Me.ID, Me.State)
                    End If

                    If Me.DailyRecordMargin IsNot Nothing Then
                        bolRet = ResetDailyRecordConcepts(Me.State)
                    End If

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tConcept, strObjectName, tbAuditParameters, -1)
                    End If

                End If

                If bolRet Then
                    bolRet = Me.Recalculate(bolDefinitionHasChanged, , , oConceptold)

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.InsertOrUpdate.ToString)
                    ' Notificamos el cambio al servidor
                    roConnector.InitTask(TasksType.CONCEPTS, oParamsAux)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roConcept::Save::Could not send cache update")
            End Try

            Return bolRet

        End Function

        Public Function ResetDailyRecordConcepts(ByVal _State As roConceptState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@UPDATE# Concepts SET DailyRecordMargin = NULL, AutoApproveRequestsDR = 0 where DailyRecordMargin IS NOT NULL AND ID <> " & Me.ID

                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::ResetDailyRecordConcepts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::ResetDailyRecordConcepts")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra el acumulado siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then

                    'Borramos las relaciones entre el concepto que vamos a borrar y sus causas (composición)
                    bolRet = roConceptComposition.DeleteConceptCompositions(Me.ID, Me.oState)
                    If bolRet Then
                        Dim oConceptold As roConcept = Nothing
                        oConceptold = New roConcept(Me.ID, Me.State)

                        ' Hay que recalcular los dias del periodo del saldo
                        bolRet = Me.Recalculate(True, , , oConceptold)

                        If bolRet Then
                            bolRet = False

                            'Borramos los registro relacionados de arrastres
                            'Borramos la relacion entre concepto y limite anual
                            'Borramos el concepto
                            Dim DelQuerys() As String = {"@DELETE# FROM DailyAccruals WHERE IDConcept = " & Me.ID.ToString,
                                                     "@DELETE# FROM EmployeeConceptCarryOvers WHERE IDConcept = " & Me.ID.ToString,
                                                     "@DELETE# FROM EmployeeConceptAnnualLimits WHERE IDConcept = " & Me.ID.ToString,
                                                     "@DELETE# FROM Concepts WHERE ID = " & Me.ID.ToString}
                            For n As Integer = 0 To DelQuerys.Length - 1
                                If Not ExecuteSqlWithoutTimeOut(DelQuerys(n)) Then
                                    oState.Result = ConceptResultEnum.ConnectionError
                                    Exit For
                                End If
                            Next

                            bolRet = (oState.Result = ConceptResultEnum.NoError)
                        End If

                        If bolRet And bAudit Then
                            ' Auditamos
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tConcept, Me.strName, Nothing, -1)
                        End If

                        If bolRet Then

                        End If
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.Delete.ToString)
                    ' Notificamos el cambio al servidor
                    roConnector.InitTask(TasksType.CONCEPTS, oParamsAux)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roConcept::Save::Could not send cache update")
            End Try

            Return bolRet

        End Function

        Public Function UpdateConceptUsedInServer(ByVal iOldID As Integer, ByVal iNewID As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@UPDATE# SysRoReportGroupConcepts SET IDConcept = " & iNewID & " WHERE SysRoReportGroupConcepts.IDConcept = " & iOldID.ToString

                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::UpdateConceptUsedInServer")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::UpdateConceptUsedInServer")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Verifica si el acumulado es está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean
            Dim bolIsUsed As Boolean = False

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim oCollection As roCollection
                Dim strUseConcept As String = ""

                ' Reglas de acumulados
                ' Verifica que el acumulado no se esté usando en ninguna regla de acumulados
                strSQL = "@SELECT# name, Definition FROM AccrualsRules "
                tb = CreateDataTable(strSQL)
                If tb IsNot Nothing Then
                    strUseConcept = ""
                    For Each oRow As DataRow In tb.Rows
                        If Any2String(oRow("Definition")).Trim <> "" Then
                            ' Compruebo si el IDConcept esta dentro de la definicion de la regla de acumulados
                            oCollection = New roCollection(oRow("Definition"))
                            If Any2Integer(oCollection.Item("MainAccrual")) = Me.ID Then
                                strUseConcept &= ", " & oRow("Name")
                            End If
                        End If
                    Next
                    If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                    If strUseConcept <> "" Then
                        oState.Result = ConceptResultEnum.UsedByAccrualRules
                        bolIsUsed = True
                    End If
                End If

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado a ningun empleado
                    strSQL = "@SELECT# Indicators.Name From Indicators, Concepts Where (Concepts.ID = Indicators.IDFirstConcept OR Concepts.ID = Indicators.IDSecondConcept) And Concepts.ID = " & Me.intID
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUseConcept = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los empleados que lo usan
                            strUseConcept &= "," & oRow("Name")
                        Next
                        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                        If strUseConcept <> "" Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(strUseConcept)
                            oState.Result = ConceptResultEnum.UsedByInddicators
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Límites anuales
                    ' Verifica que el acumulado no se esté usando en los límites anuales
                    strSQL = "@SELECT# idConcept, Employees.Name " &
                             "FROM EmployeeConceptAnnualLimits ECAL, Employees " &
                             "WHERE ECAL.IDEmployee = Employees.ID AND " &
                                   "ECAL.IDConcept = " & Me.ID.ToString & " " &
                             "ORDER BY Employees.Name "
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUseConcept = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los empleados que lo usan
                            strUseConcept &= "," & oRow("Name")
                        Next
                        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                        If strUseConcept <> "" Then
                            oState.Result = ConceptResultEnum.UsedByEmployeeConceptAnnualLimits
                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Grupos de acumulados
                    ' Verifica que no existan grupos de acumulados que usen el concepto
                    strSQL = "@SELECT# SysRoReportGroups.Name " &
                             "FROM SysRoReportGroups " &
                                    "INNER JOIN SysRoReportGroupConcepts ON " &
                                              "SysRoReportGroupConcepts.IDReportGroup = SysRoReportGroups.ID AND " &
                                              "SysRoReportGroupConcepts.IDConcept = " & Me.ID.ToString
                    tb = CreateDataTable(strSQL)

                    If tb IsNot Nothing Then
                        strUseConcept = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los grupos de acumulados que lo usan
                            strUseConcept &= ", " & oRow("Name")
                        Next
                        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)

                        If strUseConcept <> "" Then
                            oState.Result = ConceptResultEnum.UsedByReportsGroups
                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Saldos de vacaciones/permisos por horas
                    ' Verifica que el acumulado no se esté usando en las justificaciones de vacaciiones
                    strSQL = "@SELECT# Name " &
                             "FROM Causes  " &
                             "WHERE IDConceptBalance= " & Me.ID.ToString & " " &
                             "ORDER BY Name "
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUseConcept = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los empleados que lo usan
                            strUseConcept &= "," & oRow("Name")
                        Next
                        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                        If strUseConcept <> "" Then
                            oState.Result = ConceptResultEnum.UsedByCausesonProgrammedHolidays
                            bolIsUsed = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::IsUsed")
            End Try

            Return bolIsUsed

        End Function

        Public Function Recalculate(ByVal bolDefinitionHasChanged As Boolean, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifDate As Date = Nothing, Optional ByVal oConceptOld As roConcept = Nothing) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Obtenemos la fecha de congelación
                Dim xFreezingDate As Date = New Date(1900, 1, 1)
                Dim oParameters As New roParameters("OPTIONS", True)
                If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing Then
                    If IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                        xFreezingDate = oParameters.Parameter(Parameters.FirstDate)
                    End If
                End If

                ' Obtenemos la fecha de inicio del periodo del saldo
                Dim xFirstDate As Date = Me.BeginDate.Date

                ' Si se informa de fecha de modificación, miramos si es posterior al inicio del periodo del saldo. Nos quedamos con la más grande.
                If _ModifDate <> Nothing Then
                    If xFirstDate < _ModifDate Then xFirstDate = _ModifDate
                End If

                ' Miramos si la fecha obtenida está dentro de periodo de congelación.
                If xFirstDate <= xFreezingDate Then xFirstDate = xFreezingDate.AddDays(1)

                Dim strSQL As String = ""

                ' Solo borramos los registros que no esten dentro del periodo, en el caso que se modifique únicamente la definicion del saldo
                If _IDEmployee = -1 Then
                    strSQL = "@DELETE# DailyAccruals WHERE (NOT Date BETWEEN " &
                            Any2Time(xFirstDate).SQLSmallDateTime & " AND " &
                            Any2Time(Me.FinishDate.Date).SQLSmallDateTime & ") AND " &
                            "IDConcept=" & Me.ID.ToString & " AND Date>" & Any2Time(xFreezingDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailyAccruals.IDEmployee)  "

                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                Else
                    bolRet = True
                End If

                If bolRet Then
                    Dim lstIDCauses As New Generic.List(Of Integer)
                    Dim lstIDShifts As New Generic.List(Of Integer)
                    Dim lstIDCauseProgrammedAbsences As New Generic.List(Of Integer)

                    Dim strFilterComposite As String = ""

                    If bolDefinitionHasChanged And bolRet Then
                        If Not oConceptOld Is Nothing Then
                            If oConceptOld.AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType Then
                                ' Eliminamos todos los registros generados por el devengo automatico
                                ' de la definicion anterior, en caso que la antiguo definicion lo generase
                                strSQL = "@DELETE# DailyCauses WHERE ((Date BETWEEN " & Any2Time(xFirstDate).SQLSmallDateTime & " AND " &
                                       Any2Time(Me.FinishDate.Date).SQLSmallDateTime & ") AND " &
                                       "Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                                       "Date > " & Any2Time(xFreezingDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailyCauses.IDEmployee)  " & ")"
                                If _IDEmployee <> -1 Then
                                    strSQL &= " AND (IDEmployee = " & _IDEmployee.ToString & ") "
                                End If
                                strSQL &= " AND IDCause=" & oConceptOld.AutomaticAccrualIDCause.ToString & " AND IDRelatedIncidence = 0  AND AccruedRule=1"
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                lstIDCauses.Add(oConceptOld.AutomaticAccrualIDCause)
                                If oConceptOld.oAutomaticAccrualCriteria.Causes IsNot Nothing Then
                                    For i As Integer = 0 To oConceptOld.oAutomaticAccrualCriteria.Causes.Count - 1
                                        lstIDCauses.Add(oConceptOld.oAutomaticAccrualCriteria.Causes(i))
                                    Next
                                End If
                                If oConceptOld.oAutomaticAccrualCriteria.Shifts IsNot Nothing Then
                                    For i As Integer = 0 To oConceptOld.oAutomaticAccrualCriteria.Shifts.Count - 1
                                        lstIDShifts.Add(oConceptOld.oAutomaticAccrualCriteria.Shifts(i))
                                    Next
                                End If

                                If oConceptOld.AutomaticAccrualCriteria.AutomaticAccrualType = eAutomaticAccrualType.DaysType Then
                                    ' Debemos recalcular todos los dias del periodo en el caso que el devengo se genere cada cada dia
                                    strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailySchedule DS WHERE DailySchedule.IDEmployee = DS.IDEmployee AND DailySchedule.Date = DS.Date  ))   "
                                End If
                            End If

                            If oConceptOld.ApplyExpiredHours.HasValue AndAlso oConceptOld.ApplyExpiredHours Then
                                ' Eliminamos todos los registros generados por la caducidad del saldo
                                ' de la definicion anterior, en caso que la antiguo definicion lo generase
                                strSQL = "@DELETE# DailyCauses WHERE ((Date BETWEEN " & Any2Time(xFirstDate).SQLSmallDateTime & " AND " &
                                       Any2Time(Me.FinishDate.Date).SQLSmallDateTime & ") AND " &
                                       "Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                                       "Date > " & Any2Time(xFreezingDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailyCauses.IDEmployee)  " & ")"
                                If _IDEmployee <> -1 Then
                                    strSQL &= " AND (IDEmployee = " & _IDEmployee.ToString & ") "
                                End If
                                strSQL &= " AND IDCause=" & oConceptOld.ExpiredIDCause.ToString & " AND AccrualsRules=1"
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                lstIDCauses.Add(oConceptOld.ExpiredIDCause)

                            End If
                        End If
                    End If

                    If bolDefinitionHasChanged And bolRet Then

                        ' Obtenemos IDs de las justificaciones de la composición
                        ' Obtenemos IDs de los Horarios de la composición
                        ' Obtenemos IDs de las justificaciones de las previsiones de ausencias por dias
                        ' de la definicion antigua y de la nueva

                        ' Antigua definicion
                        If Not oConceptOld Is Nothing Then
                            For Each oAuxcomposition As roConceptComposition In oConceptOld.oComposition
                                Select Case oAuxcomposition.IDType
                                    Case CompositionType.Cause
                                        lstIDCauses.Add(oAuxcomposition.IDCause)
                                    Case CompositionType.Shift
                                        lstIDShifts.Add(oAuxcomposition.IDShift)
                                    Case CompositionType.Absence
                                        lstIDCauseProgrammedAbsences.Add(oAuxcomposition.IDCause)
                                End Select
                            Next

                        End If

                        ' Nueva definicion
                        If Not Me.oComposition Is Nothing Then
                            For Each oAuxcomposition As roConceptComposition In Me.oComposition
                                Select Case oAuxcomposition.IDType
                                    Case CompositionType.Cause
                                        lstIDCauses.Add(oAuxcomposition.IDCause)
                                    Case CompositionType.Shift
                                        lstIDShifts.Add(oAuxcomposition.IDShift)
                                    Case CompositionType.Absence
                                        lstIDCauseProgrammedAbsences.Add(oAuxcomposition.IDCause)
                                End Select
                            Next
                        End If

                        If Me.AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType Then
                            lstIDCauses.Add(Me.AutomaticAccrualIDCause)
                            If Me.oAutomaticAccrualCriteria.Causes IsNot Nothing Then
                                For i As Integer = 0 To Me.oAutomaticAccrualCriteria.Causes.Count - 1
                                    lstIDCauses.Add(Me.oAutomaticAccrualCriteria.Causes(i))
                                Next
                            End If
                            If Me.oAutomaticAccrualCriteria.Shifts IsNot Nothing Then
                                For i As Integer = 0 To Me.oAutomaticAccrualCriteria.Shifts.Count - 1
                                    lstIDShifts.Add(Me.oAutomaticAccrualCriteria.Shifts(i))
                                Next
                            End If

                            If Me.AutomaticAccrualCriteria.AutomaticAccrualType = eAutomaticAccrualType.DaysType Then
                                If strFilterComposite.Length > 0 Then
                                    strFilterComposite = strFilterComposite & " OR "
                                End If

                                ' Debemos recalcular todos los dias del periodo en el caso que el devengo se genere cada cada dia
                                strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailySchedule DS WHERE DailySchedule.IDEmployee = DS.IDEmployee AND DailySchedule.Date = DS.Date  ))   "
                            End If
                        End If

                        If Me.ApplyExpiredHours.HasValue AndAlso Me.ApplyExpiredHours Then
                            lstIDCauses.Add(Me.AutomaticAccrualIDCause)
                        End If

                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                 "SET Status = 65, [GUID] = '' " &
                                 "WHERE STATUS>65 AND ((Date BETWEEN " & Any2Time(xFirstDate).SQLSmallDateTime & " AND " &
                                                          Any2Time(Me.FinishDate.Date).SQLSmallDateTime & ") AND " &
                                       "Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                                       "Date > " & Any2Time(xFreezingDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " & ")"

                        If _IDEmployee <> -1 Then
                            strSQL &= " AND (IDEmployee = " & _IDEmployee.ToString & ") "
                        End If

                        Dim IDs As String = ""

                        Dim bolTeoric As Boolean = False
                        If Not lstIDCauses Is Nothing AndAlso lstIDCauses.Count > 0 Then
                            IDs = "-11"
                            For Each ID As Integer In lstIDCauses
                                ' Si se aplican horas teoricas lo marcamos
                                If ID = 4 Then
                                    bolTeoric = True
                                End If
                                IDs &= "," & ID.ToString
                            Next
                            If strFilterComposite.Length > 0 Then
                                strFilterComposite = strFilterComposite & " OR "
                            End If

                            strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailyCauses WHERE DailySchedule.IDEmployee = IDEmployee AND DailySchedule.Date = Date AND IDCause IN(" & IDs & ")  )) "
                        End If

                        If bolTeoric Then
                            If strFilterComposite.Length > 0 Then
                                strFilterComposite = strFilterComposite & " OR "
                            End If
                            strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailySchedule DS WHERE DailySchedule.IDEmployee = DS.IDEmployee AND DailySchedule.Date = DS.Date AND (DS.IDShiftUsed in(@SELECT# id from shifts where shifts.ExpectedWorkingHours > 0) or DS.IDShiftBase IN(@SELECT# id from shifts where shifts.ExpectedWorkingHours > 0)) ))   "
                        End If

                        If Not lstIDShifts Is Nothing AndAlso lstIDShifts.Count > 0 Then
                            IDs = "-11"
                            For Each ID As Integer In lstIDShifts
                                IDs &= "," & ID.ToString
                            Next

                            If strFilterComposite.Length > 0 Then
                                strFilterComposite = strFilterComposite & " OR "
                            End If
                            strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailySchedule DS with (nolock) WHERE DailySchedule.IDEmployee = DS.IDEmployee AND DailySchedule.Date = DS.Date AND (DS.IDShiftUsed in(" & IDs & ") or DS.IDShiftBase IN(" & IDs & ")) ))   "
                        End If

                        If Not lstIDCauseProgrammedAbsences Is Nothing AndAlso lstIDCauseProgrammedAbsences.Count > 0 Then
                            IDs = "-11"
                            For Each ID As Integer In lstIDCauseProgrammedAbsences
                                IDs &= "," & ID.ToString
                            Next

                            If strFilterComposite.Length > 0 Then
                                strFilterComposite = strFilterComposite & " OR "
                            End If
                            strFilterComposite &= "  ( EXISTS  (@SELECT# 1 as ExistRec FROM ProgrammedAbsences WHERE DailySchedule.IDEmployee = IDEmployee AND DailySchedule.Date >= BeginDate and DailySchedule.Date <=  (CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) AND IDCause in(" & IDs & ") ))  "

                        End If

                        If strFilterComposite.Length > 0 Then
                            ' Solo marcamos para recalcular si existe o existía composicion en el saldo

                            ' Añadimos además para recalcular los días en los que actualmente se habia generado el saldo
                            strFilterComposite &= " OR  ( EXISTS  (@SELECT# 1 as ExistRec FROM DailyAccruals with (nolock) WHERE DailySchedule.IDEmployee = IDEmployee AND DailySchedule.Date  = Date AND IDConcept =" & Me.ID.ToString & " ))  "

                            strSQL &= " AND ( " & strFilterComposite & " )"

                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        Else
                            bolRet = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Recalculate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Recalculate")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetConceptByShortName(ByVal strShortName As String, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False) As roConcept
            Dim oRet As roConcept = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID from Concepts where ShortName = '" & strShortName & "'"

                Dim iConceptID As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If iConceptID > 0 Then
                    oRet = New roConcept(iConceptID, _State, bAudit)
                Else
                    oRet = Nothing
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConceptList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConceptList")
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetConceptList(ByVal _State As roConceptState, ByVal filterBusinessGroups As Boolean) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# distinct Concepts.ID, Concepts.Name from Concepts"

                If filterBusinessGroups Then
                    Dim dtFilter As DataTable = roConceptGroup.GetConceptGroups(_State, True)

                    If dtFilter IsNot Nothing AndAlso dtFilter.Rows.Count > 0 Then
                        Dim strIDs As String = "-1,"
                        For Each oRow As DataRow In dtFilter.Rows
                            strIDs &= oRow("ID") & ","
                        Next
                        strIDs = strIDs.Substring(0, strIDs.Length - 1)

                        strSQL &= " LEFT JOIN sysroReportGroupConcepts ON (sysroReportGroupConcepts.IDConcept = Concepts.ID)"
                        strSQL &= " WHERE sysroReportGroupConcepts.IDReportGroup IN (" & strIDs & ") OR sysroReportGroupConcepts.IDReportGroup is null"
                    End If
                End If

                strSQL &= " Order By Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConceptList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConceptList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptListLite(ByVal _State As roConceptState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# distinct Concepts.ID, Concepts.Name from Concepts Order By Name"

                oRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConceptListLite")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConceptListLite")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConcepts(ByVal _State As roConceptState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from Concepts WHERE IDType = 'O' AND ApplyOnHolidaysRequest = 1 Order By Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConcepts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConcepts")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetConcepts(ByVal _State As roConceptState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from Concepts Order By Name"

                oRet = CreateDataTable(strSQL, )

                'If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                '    ' Auditamos consulta masiva
                '    Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                '    Dim strAuditName As String = ""
                '    For Each oRow As DataRow In oRet.Rows
                '        strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("IDContract")
                '    Next
                '    Audit.roAudit.AddParameter(tbAuditParameters, "{IDContracts}", strAuditName, "", 1)
                '    Dim strEmployeeName As String = GetEmployeeName(IDEmployee, _State)
                '    Audit.roAudit.AddParameter(tbAuditParameters, "{EmployeeName}", strEmployeeName, "", 1)
                '    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tEmployeeContract, strAuditName & " (" & strEmployeeName & ")", tbAuditParameters, -1)
                'End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConcepts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConcepts")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la fecha más antigua calculada de un acumulado. Si no hay ninguna fecha calculada, devuelve nothing.
        ''' </summary>
        ''' <param name="_IDConcept">Código del acumulado</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetConceptOldestDate(ByVal _IDConcept As Integer, ByVal _State As roConceptState) As Nullable(Of Date)

            Dim oRet As Nullable(Of Date) = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# MIN(Date) From DailyAccruals Where IDConcept = " & _IDConcept.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        oRet = tb.Rows(0).Item(0)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConceptOldestDate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConceptOldestDate")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un dataset con el id y nombre de todos los saldos
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetConceptDataset(ByVal _State As roConceptState) As DataTable

            Dim dsRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID, Name, FinishDate from Concepts"
                dsRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::GetConceptDataset")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::GetConceptDataset")
            Finally

            End Try

            Return dsRet

        End Function

        Public Shared Function GetConceptsListWithData(ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roConcept)

            Dim oRet As New Generic.List(Of roConcept)

            Dim dTbl As DataTable

            Try

                Dim strSQL As String = "@SELECT# ID FROM Concepts  Order By ID"

                dTbl = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each dRow As DataRow In dTbl.Rows
                        oRet.Add(New roConcept(dRow("ID"), _State, bAudit))
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetConceptsListWithData")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetConceptsListWithData")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConceptsSummaryByShift(ByVal _State As roConceptState, ByVal idEmployee As Integer, ByVal idShift As Integer) As List(Of roHolidayConceptsSummary)

            Dim oRet As New List(Of roHolidayConceptsSummary)

            Try

                Dim tb As DataTable = roConcept.GetHolidaysConceptsDetailByEmployee(_State, idShift, idEmployee)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim index As Integer = 0
                    For Each oRow As DataRow In tb.Rows
                        Dim aux As roHolidayConceptsSummary = New roHolidayConceptsSummary
                        aux.ID = index
                        aux.TransactionDate = roTypes.Any2String(oRow("Date"))
                        aux.Detail = _State.Language.Translate($"roConcepts.HolidayMovementType.{roTypes.Any2String(oRow("ValueType"))}", String.Empty)
                        aux.Days = Math.Abs(oRow("AccrualDayValue"))
                        aux.Total = oRow("AccrualValue")
                        aux.TransactionDateOrder = oRow("RowOrder")
                        oRet.Add(aux)
                        index += 1
                    Next
                End If

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsSummaryByShift")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsSummaryByShift")
            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConceptsSummaryByEmployee(ByVal _State As roConceptState, ByVal idEmployee As Integer, ByVal idShift As Integer) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim _EmpState = New Employee.roEmployeeState(_State.IDPassport)

                Dim intCurrentAccrual As Double = 0
                Dim intApprovalPending As Double = 0
                Dim intApprovedNotEnjoyied As Double = 0
                Dim intWillExpires As Integer = 0
                Dim intCanNotEnjoyYet As Integer = 0
                Dim intAlreadyEnjoyed As Double = 0
                Dim intExpectedAccrual As Double = 0

                Common.roBusinessSupport.VacationsResumeQuery(idEmployee, idShift, Now.Date, Nothing, Nothing, Now.Date, intAlreadyEnjoyed, intApprovalPending, intApprovedNotEnjoyied, intCurrentAccrual, _EmpState, intWillExpires, intCanNotEnjoyYet)

                intExpectedAccrual = intCurrentAccrual - intWillExpires - intCanNotEnjoyYet - intApprovedNotEnjoyied - intApprovalPending

                oRet = New DataTable()
                oRet.Columns.Add("Available")
                oRet.Columns.Add("ApprovalPending")
                oRet.Columns.Add("ApprovedNotEnjoyed")
                oRet.Columns.Add("WillExpires")
                oRet.Columns.Add("CanNotEnjoyYet")
                oRet.Columns.Add("ExpectedAccrual")
                oRet.Rows.Add(intCurrentAccrual, intApprovalPending, intApprovedNotEnjoyied, intWillExpires, intCanNotEnjoyYet, intExpectedAccrual)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsSummaryByEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsSummaryByEmployee")
            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConceptsDetailByEmployee(ByVal _State As roConceptState, ByVal iSelectedShiftID As Integer, ByVal idEmployee As Integer) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try
                ' Recuperamos el saldo que controla las vacaciones
                Dim sType As String = String.Empty
                Dim strSQL As String = $"@SELECT# DefaultQuery FROM Shifts 
                                        INNER JOIN Concepts ON Concepts.ID = Shifts.IdConceptBalance
                                        WHERE Shifts.Id = {iSelectedShiftID}"
                sType = UCase(roTypes.Any2String(ExecuteScalar(strSQL)))

                Dim iSummaryType As SummaryType = SummaryType.ContractAnnualized
                Select Case sType
                    Case "Y"
                        iSummaryType = SummaryType.Anual
                    Case "M"
                        iSummaryType = SummaryType.Mensual
                    Case "W"
                        iSummaryType = SummaryType.Semanal
                    Case "L"
                        iSummaryType = SummaryType.ContractAnnualized
                    Case Else
                        iSummaryType = SummaryType.Contrato
                End Select

                Dim lstDates As List(Of DateTime)
                Dim oContractState = New Contract.roContractState(_State.IDPassport)
                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(iSummaryType, idEmployee, Now.Date, oContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                Dim strCultureName As String = System.Globalization.CultureInfo.CurrentCulture.TextInfo.CultureName
                Dim strShortDatePattern As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

                strSQL = $"@SELECT# ValueType,
		                            CAST(Date AS DATE) Date,
		                            CAST(EndDate AS DATE) EndDate,
                                    AccrualDayValue,
	                                FORMAT(BeginPeriod,'{strShortDatePattern}','{strCultureName}') + ' - ' + FORMAT(EndPeriod,'{strShortDatePattern}','{strCultureName}') AS Section,
	                                DATEDIFF(YEAR, FirstDateAtCompany, CAST(Date AS DATE)) AS YearsAtCompany,
	                                CASE WHEN ValueType = 'StartupValue' AND  AccrualDayValue > 0 THEN ISNULL(CAST(StartEnjoymentDate AS DATE), CAST(Date AS DATE)) END HolidaysAvailabilityDate,
	                                CASE WHEN ValueType = 'StartupValue' THEN  CAST(AccrualDayValue AS INT) END EarnedHolidays,
	                                CASE WHEN ValueType = 'StartupValue' THEN  CAST(ExpiredDate AS DATE) END HolidaysExpirationDate,
	                                CASE WHEN ValueType = 'DailyValue' THEN  -1 * CAST(AccrualDayValue AS INT) END AlreadyTakenHollidays,
	                                CASE WHEN ValueType = 'ExpiredOrCarryOver' THEN  -1 * CAST(AccrualDayValue AS INT) END ExpiredHolidays,
	                                CAST(SUM(AccrualDayValue) OVER (ORDER BY Date ASC, CASE WHEN ValueType = 'DailyValue' THEN 1 WHEN ValueType = 'ExpiredOrCarryOver' THEN 2 ELSE 3 END ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS INT) AS AccrualValue,
		                            CASE WHEN ValueType = 'DailyValue' THEN 1 WHEN ValueType = 'ExpiredOrCarryOver' THEN 2 ELSE 3 END AS RowOrder
                            FROM (
                            @SELECT# SAD.IDEmployee, 
                                    SAD.IDConcept, 
	                                ValueType, 
	                                SAD.Date, 
	                                EndDate, 
	                                ExpiredDate, 
	                                StartEnjoymentDate, 
	                                SAD.Value AS AccrualDayValue, 
	                                BeginPeriod, 
	                                EndPeriod,
                                    CASE WHEN TRY_CONVERT(datetime,CONVERT(VARCHAR(50),EUFV.Value),111) IS NOT NULL THEN TRY_CONVERT(datetime,CONVERT(VARCHAR(50),EUFV.Value),111) ELSE TRY_CONVERT(datetime,CONVERT(VARCHAR(50),EUFV.Value),103) END AS FirstDateAtCompany
                            FROM sysrovwAccruals_DailyAccrualsGrouped SAD
                            CROSS JOIN Shifts
                            LEFT JOIN StartupValues SV ON SV.IDConcept = SAD.IDConcept
                            INNER JOIN LabAgreeStartupValues LASV ON SV.IDStartupValue = LASV.IDStartupValue
                            AND LASV.IDLabAgree IN (@SELECT# IDLabAgree FROM EmployeeContracts WHERE EmployeeContracts.IDEmployee = SAD.IDEmployee and EmployeeContracts.BeginDate <= {roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime}  and EmployeeContracts.EndDate >= {roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime} )
                            LEFT JOIN EmployeeUserFieldValues EUFV ON EUFV.IDEmployee = SAD.IDEmployee AND FieldName = SV.ScalingUserField
                            WHERE SAD.Date >= {roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime}
                                AND Shifts.Id = {iSelectedShiftID}
                                AND SAD.IDEmployee = {idEmployee}
                                AND SAD.IDConcept = Shifts.IDConceptBalance
                            ) AUX
                            ORDER BY AUX.Date ASC, RowOrder ASC" ' <--- ATENCIÓN !!!!: NO SE PUEDE CAMBIAR ESTA ORDENACIÓN. AFECTA A LA ACUMULACIÓN (ACCRUALVALUE)

                oRet = CreateDataTable(strSQL, )


            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsDetailByEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConcept::GetHolidaysConceptsDetailByEmployee")

            End Try

            Return oRet

        End Function
#End Region

#End Region

#Region "Accruals Employee Helper"

        Public Shared Function GetDailyAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal filterBusinessGroups As Boolean,
                                                Optional ByVal bolAddMaxValue As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)
                Dim myDate = New DateTime(xDate.Year, xDate.Month, xDate.Day, 0, 0, 0, 0)

                Dim strSQL As String
                strSQL = "@SELECT# DailyAccruals.IDConcept, Concepts.Name, DailyAccruals.Value, Concepts.IDType, DailyAccruals.CarryOver, '' AS ValueFormat , 0.0 as MaxValue  "
                strSQL &= "FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID "
                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ")"
                strSQL &= "WHERE DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND DailyAccruals.Date = " & Any2Time(myDate).SQLSmallDateTime
                If intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                tb = CreateDataTable(strSQL,)

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then

                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("ValueFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Value")))
                                Catch ex As Exception
                                    oRow("ValueFormat") = "00:00"
                                End Try

                            Case "O"
                                oRow("ValueFormat") = Format(CDbl(oRow("Value")), "##0.000")

                            Case Else
                                oRow("ValueFormat") = Format(CDate(Any2Time(CDbl(oRow("Value"))).Value), "HH:mm")

                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date = " & Any2Time(myDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else

                        oRow.Delete()

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetDailyTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim myDate = New DateTime(xDate.Year, xDate.Month, xDate.Day, 0, 0, 0, 0)
                Dim strSQL As String
                strSQL = "@SELECT# DailyTaskAccruals.IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Value, '' AS ValueFormat " &
                         "FROM DailyTaskAccruals INNER JOIN Tasks " &
                                    "ON DailyTaskAccruals.IDTask = Tasks.ID " &
                               " WHERE DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                               "DailyTaskAccruals.Date = " & Any2Time(myDate).SQLSmallDateTime

                strSQL &= " GROUP By DailyTaskAccruals.IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows
                    Try
                        oRow("ValueFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Value")))
                    Catch ex As Exception
                        oRow("ValueFormat") = "00:00"
                    End Try
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetAnualAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                                Optional ByVal _intIDConcept As Integer = 0, Optional ByVal filterBusinessGroups As Boolean = False, Optional ByVal bolAddMaxValue As Boolean = False, Optional ByVal Last As Boolean = False, Optional ByVal bIncludeZeroes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1
                If intYearIniMonth = 0 Then intYearIniMonth = 1

                Dim xBeginPeriod As DateTime
                If xDate.Month > intYearIniMonth Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                ElseIf xDate.Month = intYearIniMonth And xDate.Day >= intMonthIniDay Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                Else
                    xBeginPeriod = New DateTime(xDate.Year - 1, intYearIniMonth, intMonthIniDay)
                End If

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                If (Last = True) Then
                    xBeginPeriod = xBeginPeriod.AddYears(-1)
                    xDate = xBeginPeriod.AddYears(1).AddDays(-1)
                End If

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)

                Dim strSQL As String
                strSQL = "@SELECT# Concepts.ID AS IDConcept, Concepts.Name, ISNULL(SUM(DailyAccruals.Value), 0) as Total, Concepts.IDType, '' AS TotalFormat, Concepts.DefaultQuery, 0.0 as MaxValue, Concepts.ShortName "
                strSQL &= " FROM Concepts "

                If Not bIncludeZeroes Then
                    strSQL &= "INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                Else
                    strSQL &= "LEFT JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                End If

                strSQL &= " WHERE ((DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DailyAccruals.IDEmployee IS NULL) AND DefaultQuery = 'Y'"

                If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                If _intIDConcept > 0 Then
                    strSQL &= " AND Concepts.ID = " & _intIDConcept.ToString
                End If

                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ") "

                strSQL &= " GROUP By Concepts.ID, Concepts.Name, Concepts.IDType, Concepts.DefaultQuery, Concepts.ShortName "

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then

                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                                Catch ex As Exception
                                    oRow("TotalFormat") = "00:00"
                                End Try

                            Case "O"
                                oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")

                            Case Else
                                oRow("TotalFormat") = Format(CDate(Any2Time(CDbl(oRow("Total"))).Value), "HH:mm")

                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else

                        oRow.Delete()

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetContractAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                                   Optional ByVal _intIDConcept As Integer = 0, Optional ByVal filterBusinessGroups As Boolean = False, Optional ByVal bolAddMaxValue As Boolean = False, Optional ByVal bIncludeZeroes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim xBeginPeriod As DateTime

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                Else
                    xBeginPeriod = New DateTime(1900, 1, 1, 0, 0, 0)
                    xDate = New DateTime(1900, 1, 1, 0, 0, 0)
                End If

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)

                Dim strSQL As String
                strSQL = "@SELECT# Concepts.ID AS IDConcept, Concepts.Name, ISNULL(SUM(DailyAccruals.Value), 0) as Total, Concepts.IDType, '' AS TotalFormat, Concepts.DefaultQuery, 0.0 as MaxValue , Concepts.ShortName "
                strSQL &= " FROM Concepts "

                If Not bIncludeZeroes Then
                    strSQL &= " INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                Else
                    strSQL &= " LEFT JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                End If

                strSQL &= " WHERE ((DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DailyAccruals.IDEmployee IS NULL) AND DefaultQuery = 'C' "

                If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                If _intIDConcept > 0 Then
                    strSQL &= " AND Concepts.ID = " & _intIDConcept.ToString
                End If

                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ") "

                strSQL &= " GROUP By Concepts.ID, Concepts.Name, Concepts.IDType, Concepts.DefaultQuery, Concepts.ShortName "

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then

                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                                Catch ex As Exception
                                    oRow("TotalFormat") = "00:00"
                                End Try

                            Case "O"
                                oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")

                            Case Else
                                oRow("TotalFormat") = Format(CDate(Any2Time(CDbl(oRow("Total"))).Value), "HH:mm")

                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else

                        oRow.Delete()

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetContractAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetContractAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetContractAnnualizedAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                                   Optional ByVal _intIDConcept As Integer = 0, Optional ByVal filterBusinessGroups As Boolean = False, Optional ByVal bolAddMaxValue As Boolean = False, Optional ByVal bIncludeZeroes As Boolean = False) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = Nothing
                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, intIDEmployee, xDate, roContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)

                Dim strSQL As String
                strSQL = "@SELECT# Concepts.ID AS IDConcept, Concepts.Name, ISNULL(SUM(DailyAccruals.Value), 0) as Total, Concepts.IDType, '' AS TotalFormat, Concepts.DefaultQuery, 0.0 as MaxValue , Concepts.ShortName, '' As YearWorkPeriod "
                strSQL &= " FROM Concepts "

                If Not bIncludeZeroes Then
                    strSQL &= " INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                Else
                    strSQL &= " LEFT JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                End If

                strSQL &= " WHERE ((DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DailyAccruals.IDEmployee IS NULL) AND DefaultQuery = 'L' "

                If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                If _intIDConcept > 0 Then
                    strSQL &= " AND Concepts.ID = " & _intIDConcept.ToString
                End If

                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ") "

                strSQL &= " GROUP By Concepts.ID, Concepts.Name, Concepts.IDType, Concepts.DefaultQuery, Concepts.ShortName "

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If _OnlyViewInTerminals OrElse intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then
                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                                Catch ex As Exception
                                    oRow("TotalFormat") = "00:00"
                                End Try
                            Case "O"
                                oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                            Case Else
                                oRow("TotalFormat") = Format(CDate(Any2Time(CDbl(oRow("Total"))).Value), "HH:mm")
                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else
                        oRow.Delete()
                    End If
                Next
                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnnualWorkAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnnualWorkAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetContractAnnualizedTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = Nothing
                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, intIDEmployee, xDate, roContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total,  '' AS TotalFormat  " &
                         "FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ")) AND " &
                                 "DailyTaskAccruals.IDTask IN (@SELECT# DISTINCT IDTASK FROM DailyTaskAccruals WHERE IDEmployee = " & intIDEmployee.ToString & " AND Date >=" & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND Date <=" & Any2Time(xDate).SQLSmallDateTime & ")"
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next
                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetAnualTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                               Optional ByVal Last As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = Nothing
                If Last Then
                    lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.LastYear, intIDEmployee, xDate, roContractState)
                Else
                    lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, intIDEmployee, xDate, roContractState)
                End If

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total,  '' AS TotalFormat  " &
                         "FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ")) AND " &
                                 "DailyTaskAccruals.IDTask IN (@SELECT# DISTINCT IDTASK FROM DailyTaskAccruals WHERE IDEmployee = " & intIDEmployee.ToString & " AND Date >=" & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND Date <=" & Any2Time(xDate).SQLSmallDateTime & ")"
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next
                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                               Optional ByVal Last As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try
                Dim oParams As New roParameters("OPTIONS", True)

                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1
                If intYearIniMonth = 0 Then intYearIniMonth = 1

                Dim xBeginPeriod As DateTime
                If xDate.Month > intYearIniMonth Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                ElseIf xDate.Month = intYearIniMonth And xDate.Day >= intMonthIniDay Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                Else
                    xBeginPeriod = New DateTime(xDate.Year - 1, intYearIniMonth, intMonthIniDay)
                End If

                If Last Then
                    xBeginPeriod = xBeginPeriod.AddYears(-1)
                    xDate = xBeginPeriod.AddYears(1).AddDays(-1)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total,  '' AS TotalFormat  " &
                         "FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ")) AND " &
                                 "DailyTaskAccruals.IDTask IN (@SELECT# DISTINCT IDTASK FROM DailyTaskAccruals WHERE IDEmployee = " & intIDEmployee.ToString & " AND Date >=" & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND Date <=" & Any2Time(xDate).SQLSmallDateTime & ")"
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next
                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetMonthAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                                Optional ByVal filterBusinessGroups As Boolean = False, Optional ByVal bolAddMaxValue As Boolean = False, Optional ByVal Last As Boolean = False, Optional ByVal bIncludeZeroes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1

                Dim xBeginPeriod As DateTime
                If xDate.Day > intMonthIniDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    xBeginPeriod = New Date(xDate.Year, xDate.Month, intMonthIniDay)
                ElseIf xDate.Day < intMonthIniDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    xBeginPeriod = New Date(xDate.AddMonths(-1).Year, xDate.AddMonths(-1).Month, intMonthIniDay)
                Else
                    'Si es el mismo dia
                    xBeginPeriod = xDate
                End If

                If Last Then
                    xBeginPeriod = xBeginPeriod.AddMonths(-1)
                    xDate = xBeginPeriod.AddMonths(1).AddDays(-1)
                End If

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)

                Dim strSQL As String
                strSQL = "@SELECT# Concepts.ID AS IDConcept, Concepts.Name, ISNULL(SUM(DailyAccruals.Value), 0) as Total, Concepts.IDType, '' AS TotalFormat, Concepts.DefaultQuery , 0.0 as MaxValue, Concepts.ShortName  "
                strSQL &= " FROM Concepts "

                If Not bIncludeZeroes Then
                    strSQL &= " INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                Else
                    strSQL &= " LEFT JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                End If

                strSQL &= " WHERE ISNULL(Concepts.FinishDate, " & Any2Time(New Date(2079, 1, 1)).SQLSmallDateTime & ") > " & Any2Time(xDate).SQLSmallDateTime & " AND "
                strSQL &= " ((DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND "
                strSQL &= " DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DailyAccruals.IDEmployee IS NULL) AND "
                strSQL &= " DefaultQuery = 'M' "
                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ") "

                If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                strSQL &= " GROUP By Concepts.ID, Concepts.Name, Concepts.IDType, Concepts.DefaultQuery, Concepts.ShortName "

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then

                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                                Catch ex As Exception
                                    oRow("TotalFormat") = "00:00"
                                End Try

                            Case "O"
                                oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")

                            Case Else
                                oRow("TotalFormat") = Format(CDate(Any2Time(CDbl(oRow("Total"))).Value), "HH:mm")

                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else

                        oRow.Delete()

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetWeekAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                               Optional ByVal filterBusinessGroups As Boolean = False, Optional ByVal bolAddMaxValue As Boolean = False, Optional ByVal bIncludeZeroes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing
            Try
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intWeekIniDay As Integer = oParams.Parameter(Parameters.WeekPeriod)
                'intWeekIniDay = 0 -> Domingo, intWeekIniDay = 1 -> Lunes

                Dim xBeginPeriod As DateTime
                Dim iDayOfWeek As Integer = xDate.DayOfWeek
                If iDayOfWeek = 0 Then iDayOfWeek = 7
                If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7
                xBeginPeriod = xDate.AddDays(intWeekIniDay - iDayOfWeek)

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los saldos, se tendrá en cuenta la configuración de permisos del saldo.
                Dim intIDPassportEmployee As Integer = -1
                If oState.ActivePassportType(intIDPassportEmployee) <> "E" Then
                    intIDPassportEmployee = -1
                End If

                Dim oConcepGrouptState As New Concept.roConceptState(-1)
                roBusinessState.CopyTo(oState, oConcepGrouptState)

                Dim strSQL As String
                strSQL = "@SELECT# Concepts.ID AS IDConcept, Concepts.Name, ISNULL(SUM(DailyAccruals.Value), 0) as Total, Concepts.IDType, '' AS TotalFormat, Concepts.DefaultQuery, 0.0 as MaxValue, Concepts.ShortName  "
                strSQL &= " FROM Concepts "

                If Not bIncludeZeroes Then
                    strSQL &= " INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                Else
                    strSQL &= " LEFT JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "
                End If

                strSQL &= " WHERE ISNULL(Concepts.FinishDate, " & Any2Time(New Date(2079, 1, 1)).SQLSmallDateTime & ") > " & Any2Time(xDate).SQLSmallDateTime & " AND "
                strSQL &= " ((DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND "
                strSQL &= " DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DailyAccruals.IDEmployee IS NULL) AND "
                strSQL &= " DefaultQuery = 'W' "
                If filterBusinessGroups Then strSQL &= " AND Concepts.ID IN(" & Concept.roConceptGroup.GetConceptIDsByBusinessGroupsPermissions(oConcepGrouptState) & ") "

                If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then
                    strSQL &= " AND ISNULL(Concepts.EmployeesPermission, 0) IN (0,2)"
                End If
                strSQL &= " GROUP By Concepts.ID, Concepts.Name, Concepts.IDType, Concepts.DefaultQuery, Concepts.ShortName "

                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState
                roBusinessState.CopyTo(oState, oConceptState)
                Dim strSQLFilter As String = ""

                Dim bolAddAccrual As Boolean = True

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows

                    bolAddAccrual = True

                    If _OnlyViewInTerminals Or intIDPassportEmployee = intIDEmployee Then

                        ' Verificamos si el empleado asociado al passport puede consultar este saldo
                        oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState, False)
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter = " AND " & oCondition.GetFilter(intIDEmployee)
                            Next
                            strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                            Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                            bolAddAccrual = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                        End If

                    End If

                    If bolAddAccrual Then

                        Select Case oRow("IDType")
                            Case "H"
                                Try
                                    oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                                Catch ex As Exception
                                    oRow("TotalFormat") = "00:00"
                                End Try

                            Case "O"
                                oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")

                            Case Else
                                oRow("TotalFormat") = Format(CDate(Any2Time(CDbl(oRow("Total"))).Value), "HH:mm")

                        End Select

                        If bolAddMaxValue Then
                            ' Añadimos la suma de los valores positivos del saldo
                            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals WHERE IDEmployee=" & intIDEmployee.ToString
                            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime
                            strSQL &= " AND DailyAccruals.IDConcept = " & oRow("IDConcept")
                            strSQL &= " AND DailyAccruals.Value > 0.0"

                            oRow("MaxValue") = Any2Double(ExecuteScalar(strSQL))
                        End If
                    Else
                        oRow.Delete()
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetMonthTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False,
                                                    Optional ByVal Last As Boolean = False) As DataTable
            Dim tb As DataTable = Nothing
            Try
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1

                Dim xBeginPeriod As DateTime
                If xDate.Day > intMonthIniDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    xBeginPeriod = New Date(xDate.Year, xDate.Month, intMonthIniDay)
                ElseIf xDate.Day < intMonthIniDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    xBeginPeriod = New Date(xDate.AddMonths(-1).Year, xDate.AddMonths(-1).Month, intMonthIniDay)
                Else
                    'Si es el mismo dia
                    xBeginPeriod = xDate
                End If

                If (Last = True) Then
                    xBeginPeriod = xBeginPeriod.AddMonths(-1)
                    xDate = xBeginPeriod.AddMonths(1).AddDays(-1)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total, '' AS TotalFormat " &
                         " FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") ) "
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetContractTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim xBeginPeriod As DateTime
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                Else
                    xBeginPeriod = New DateTime(1900, 1, 1, 0, 0, 0)
                    xDate = New DateTime(1900, 1, 1, 0, 0, 0)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total, '' AS TotalFormat " &
                         " FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") ) "
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetContractTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetContractTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetWeekTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, Optional ByVal _OnlyViewInTerminals As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim oParams As New roParameters("OPTIONS", True)
                Dim intWeekIniDay As Integer = oParams.Parameter(Parameters.WeekPeriod)
                If intWeekIniDay = 0 Then intWeekIniDay = 1

                Dim xBeginPeriod As DateTime
                Dim iDayOfWeek As Integer = xDate.DayOfWeek
                If iDayOfWeek = 0 Then iDayOfWeek = 7
                If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7
                xBeginPeriod = xDate.AddDays(intWeekIniDay - iDayOfWeek)

                Dim strSQL As String
                strSQL = "@SELECT# Tasks.ID AS IDTask, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total, '' AS TotalFormat " &
                         " FROM Tasks INNER JOIN DailyTaskAccruals ON Tasks.ID = DailyTaskAccruals.IDTask " &
                               " WHERE ((DailyTaskAccruals.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                 "DailyTaskAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND " &
                                 "DailyTaskAccruals.Date <= " & Any2Time(xDate).SQLSmallDateTime & ") ) "
                strSQL &= " GROUP By Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    Try
                        oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                    Catch ex As Exception
                        oRow("TotalFormat") = "00:00"
                    End Try
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekTaskAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekTaskAccruals")
            Finally

            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados mensuales y anuales de un empleado y una fecha.<br/>
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado), DefaultQuery (tipo de acumulado, Y:anual, M:Mensual)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_CurrentDate">Fecha actual</param>
        ''' <param name="_OnlyViewInTerminals">Mostrar sólo los acumulados que se tienen que mostrar en los terminales</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat, DefaultQuery</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAccrualsQuery(ByVal _IDEmployee As Integer, ByVal _CurrentDate As Date, ByVal _OnlyViewInTerminals As Boolean, ByVal _State As roBusinessState) As DataTable

            Dim oRet As DataTable

            oRet = GetMonthAccruals(_IDEmployee, _CurrentDate, _State, _OnlyViewInTerminals)
            If oRet IsNot Nothing Then

                Dim tbAnual As DataTable = GetAnualAccruals(_IDEmployee, _CurrentDate, _State, _OnlyViewInTerminals)
                If tbAnual IsNot Nothing Then
                    oRet.BeginLoadData()
                    For Each oRow As DataRow In tbAnual.Rows
                        oRet.LoadDataRow(oRow.ItemArray, True)
                    Next
                    oRet.EndLoadData()
                    If oRet IsNot Nothing Then
                        Dim tbWeek As DataTable = GetWeekAccruals(_IDEmployee, _CurrentDate, _State, _OnlyViewInTerminals)
                        If tbWeek IsNot Nothing Then
                            oRet.BeginLoadData()
                            For Each oRow As DataRow In tbWeek.Rows
                                oRet.LoadDataRow(oRow.ItemArray, True)
                            Next
                            oRet.EndLoadData()
                        End If
                    End If
                    If oRet IsNot Nothing Then
                        Dim tbContract As DataTable = GetContractAccruals(_IDEmployee, _CurrentDate, _State, _OnlyViewInTerminals)
                        If tbContract IsNot Nothing Then
                            oRet.BeginLoadData()
                            For Each oRow As DataRow In tbContract.Rows
                                oRet.LoadDataRow(oRow.ItemArray, True)
                            Next
                            oRet.EndLoadData()
                        End If
                    End If

                End If

            End If

            Return oRet

        End Function

        Public Shared Function GetAccrualValueOnDate(ByVal intIDEmployee As Integer, ByVal oParams As roParameters, ByVal xDate As Date, ByVal bolOnlyDayValue As Boolean, ByVal oConcept As Concept.roConcept, ByRef oState As roBusinessState, Optional ByRef lstDates As Generic.List(Of DateTime) = Nothing) As Double

            Dim dblRet As Double = 0
            Dim bolRet As Boolean = False

            Try
                Dim xBeginPeriod As DateTime

                If oConcept IsNot Nothing AndAlso oConcept.ID > 0 Then
                    ' TODO: Toda la parte de cálculo del periodo correspondiente al tipo de saldo debería eliminarse
                    '       y delegarse en la función VTBusiness.Common.roBusinessSupport.GetPeriodDatesByContractAtDate


                    ' Obtenemos el valor del Saldo a la fecha indicada
                    Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                    Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                    Dim intWeekIniday As Integer = oParams.Parameter(Parameters.WeekPeriod)
                    If intMonthIniDay = 0 Then intMonthIniDay = 1
                    If intYearIniMonth = 0 Then intYearIniMonth = 1

                    Dim roContractState = New Contract.roContractState(-1)

                    If lstDates Is Nothing OrElse lstDates.Count = 0 Then lstDates = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)

                    ' Si el saldo es anual
                    If oConcept.DefaultQuery = "Y" Then
                        If xDate.Month > intYearIniMonth Then
                            xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                        ElseIf xDate.Month = intYearIniMonth And xDate.Day >= intMonthIniDay Then
                            xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                        Else
                            xBeginPeriod = New DateTime(xDate.Year - 1, intYearIniMonth, intMonthIniDay)
                        End If
                    ElseIf oConcept.DefaultQuery = "M" Then
                        'Si es mensual
                        If xDate.Day > intMonthIniDay Then
                            'Si el dia es posterior al inicio del periodo (mismo mes)
                            xBeginPeriod = New Date(xDate.Year, xDate.Month, intMonthIniDay)
                        ElseIf xDate.Day < intMonthIniDay Then
                            'Si el dia es anterior al inicio del periodo (mes anterior)
                            xBeginPeriod = New Date(xDate.AddMonths(-1).Year, xDate.AddMonths(-1).Month, intMonthIniDay)
                        Else
                            'Si es el mismo dia
                            xBeginPeriod = xDate
                        End If
                    ElseIf oConcept.DefaultQuery = "W" Then
                        ' Si es semanal
                        Dim iDayOfWeek As Integer = xDate.DayOfWeek
                        If iDayOfWeek = 0 Then iDayOfWeek = 7
                        If intWeekIniday > iDayOfWeek Then intWeekIniday = intWeekIniday - 7
                        xBeginPeriod = xDate.AddDays(intWeekIniday - iDayOfWeek)
                    Else
                        ' Si es por contrato, o por contrato anualizado, ya hemos definido el periodo del contrato previamente no hay que hacer ningun otro filtro
                        ' Si no hay contrato, no mostramos nada
                        If lstDates.Count = 0 Then Return 0
                    End If

                    If lstDates.Count > 0 Then
                        If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                        If xDate > lstDates(1) Then xDate = lstDates(1)
                    Else
                        ' en el caso que en la fecha indicada no tenga concrato en vigor
                        ' el valor del saldo siempre será 0
                        Return 0
                    End If

                    Dim strSQL As String
                    strSQL = "@SELECT# ISNULL(SUM(DailyAccruals.Value), 0) as Total "
                    strSQL &= " FROM Concepts "
                    strSQL &= "INNER JOIN DailyAccruals ON Concepts.ID = DailyAccruals.IDConcept "

                    strSQL &= " WHERE DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND "
                    strSQL &= " DailyAccruals.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime
                    If bolOnlyDayValue Then
                        strSQL &= " AND DailyAccruals.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime
                    End If

                    strSQL &= " AND Concepts.ID = " & oConcept.ID.ToString
                    dblRet = roTypes.Any2Double(ExecuteScalar(strSQL))
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAccrualValueOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAccrualValueOnDate")
            Finally

            End Try

            Return dblRet
        End Function

        Public Shared Function GetAccrualValueByDefaultQueryDX(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal IDConcept As Integer, ByVal strWhere As String) As Double
            Dim dblRet As Double = 0

            Try

                Dim roContractState = New Contract.roContractState(-1)
                Dim lstDates As Generic.List(Of DateTime)
                Dim iSummaryType As Integer
                ' Calculamos día de inicio y fin de acumulado, en función del tipo de saldo
                Dim sSQL As String = "@SELECT# DefaultQuery FROM Concepts WHERE Id = " & IDConcept.ToString
                Dim sType As String = UCase(roTypes.Any2String(ExecuteScalar(sSQL)))

                Select Case sType
                    Case "Y"
                        iSummaryType = SummaryType.Anual
                    Case "M"
                        iSummaryType = SummaryType.Mensual
                    Case "W"
                        iSummaryType = SummaryType.Semanal
                    Case "L"
                        iSummaryType = SummaryType.ContractAnnualized
                    Case Else
                        iSummaryType = SummaryType.Contrato
                End Select

                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(iSummaryType, intIDEmployee, xDate, roContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                sSQL = "@SELECT# sum(value) from ( @SELECT# DailyAccruals.value, DailyAccruals.Date, Concepts.DefaultQuery, " &
           " (@SELECT# top 1 beginDate from EmployeeContracts where IDEmployee = " & intIDEmployee.ToString & " AND BeginDate <= " & Any2Time(xDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xDate).SQLSmallDateTime & " order by BeginDate DESC) as BeginContract, " &
           " (@SELECT# top 1 EndDate from EmployeeContracts where IDEmployee = " & intIDEmployee.ToString & " AND BeginDate <= " & Any2Time(xDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xDate).SQLSmallDateTime & " order by BeginDate DESC) as EndContract " &
           " FROM DailyAccruals with (nolock) inner join Concepts with (nolock) on DailyAccruals.IDConcept = Concepts.ID " &
           " WHERE DailyAccruals.IDEmployee = " & intIDEmployee.ToString & " AND IDConcept = " & IDConcept.ToString & ") accrualsContract " &
           " Where BeginContract is not null and EndContract is not null and accrualsContract.Date between BeginContract and EndContract " &
           " and accrualsContract.Date between " & Any2Time(xBeginPeriod).SQLSmallDateTime & " and " & Any2Time(xDate).SQLSmallDateTime

                ' Obtenemos el valor del Saldo a la fecha indicada
                dblRet = roTypes.Any2Double(ExecuteScalar(sSQL))
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return dblRet
        End Function

        Public Shared Function GetAccrualValueOnDateDX(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal IDConcept As Integer, ByVal strWhere As String) As Double
            Dim dblRet As Double = 0

            Try
                ' Obtenemos el valor del Saldo a la fecha indicada
                Dim strSQL As String

                strSQL = "@SELECT# ISNULL(SUM(DailyAccruals.Value), 0) as Total "
                strSQL &= " FROM Concepts with (nolock) "
                strSQL &= "INNER JOIN DailyAccruals with (nolock) ON Concepts.ID = DailyAccruals.IDConcept "

                strSQL &= " WHERE DailyAccruals.IDEmployee = " & intIDEmployee.ToString
                strSQL &= " AND DailyAccruals.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime
                strSQL &= " AND Concepts.ID = " & IDConcept.ToString

                If strWhere.Length > 0 Then strSQL &= " AND " & strWhere

                dblRet = roTypes.Any2Double(ExecuteScalar(strSQL))
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return dblRet
        End Function

        Public Shared Function GetAccrualValueOnDateForDailyRecordCheck(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef dMargin As Integer) As Double

            Dim dblRet As Double = 0

            Try

                ' Obtenemos el valor del Saldo a la fecha indicada
                Dim strSQL As String

                strSQL = "@SELECT# ISNULL(SUM(DailyAccruals.Value), 0) as Total, ISNULL(MAX(Concepts.DailyRecordMargin),-1) AS Margin "
                strSQL &= " FROM Concepts with (nolock) "
                strSQL &= "INNER JOIN DailyAccruals with (nolock) ON Concepts.ID = DailyAccruals.IDConcept "
                strSQL &= " WHERE DailyAccruals.StartupValue = 0 AND DailyAccruals.CarryOver = 0 AND DailyAccruals.IDEmployee = " & intIDEmployee.ToString
                strSQL &= " AND DailyAccruals.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime
                strSQL &= " AND ISNULL(Concepts.DailyRecordMargin,-1) >= 0"

                Dim dTable As DataTable
                dTable = CreateDataTable(strSQL)
                If Not dTable Is Nothing AndAlso dTable.Rows.Count = 1 Then
                    dblRet = roTypes.Any2Double(dTable.Rows(0)("Total"))
                    dMargin = roTypes.Any2Integer(dTable.Rows(0)("Margin"))
                End If

                dblRet = roTypes.Any2Double(ExecuteScalar(strSQL))
            Catch ex As DbException
            Catch ex As Exception
            End Try

            Return dblRet
        End Function

        Public Shared Function GetExpectedWorkingHoursHolidaysDX(ByVal intIDEmployee As Integer, ByVal IDShift As Integer, ByVal startDate As Date, ByVal endDate As Date) As Double

            Dim dblRet As Double = 0

            Try

                Dim strSQL As String

                strSQL = "@SELECT# ISNULL(ShiftType, 0) FROM Shifts WHERE ID = " + IDShift.ToString

                If Any2Integer(ExecuteScalar(strSQL)).Equals(2) Then
                    strSQL = "@SELECT# SUM(isnull((case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end), 0)) as Total " +
                        "FROM DailySchedule, Shifts WHERE IDShift1 = " & IDShift & " AND Shifts.ID = DailySchedule.IDShiftBase AND IDEmployee = " & intIDEmployee & " AND Date>=" & roTypes.Any2Time(startDate).SQLSmallDateTime & " AND Date<=" & roTypes.Any2Time(endDate).SQLSmallDateTime & " AND IDShiftBase IS NOT NULL "
                Else
                    strSQL = "@SELECT# SUM(isnull((case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end), 0)) as Total " +
                        "FROM DailySchedule, Shifts WHERE IDShift1 = " & IDShift & " AND Shifts.ID = DailySchedule.IDShift1 AND IDEmployee = " & intIDEmployee & " AND Date>=" & roTypes.Any2Time(startDate).SQLSmallDateTime & " AND Date<=" & roTypes.Any2Time(endDate).SQLSmallDateTime
                End If

                dblRet = roTypes.Any2Double(ExecuteScalar(strSQL))
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return dblRet
        End Function

        Public Shared Function TimeFormat(ByVal Hours As Double, Optional IDConcept As Double = -1) As String
            Dim strRet As String = Hours.ToString

            Dim ConceptType As String = ""
            Try

                If IDConcept <= 0 Then
                    ConceptType = "H"
                Else
                    ConceptType = roTypes.Any2String(ExecuteScalar("@SELECT#  isnull(IDType,'H') as IDType  FROM Concepts WHERE id=" & IDConcept.ToString))
                End If

                If ConceptType = "H" Then
                    strRet = roConversions.ConvertHoursToTime(Hours)
                Else
                    strRet = Format$(Hours, "###0.00")
                End If
            Catch ex As DbException
            Catch ex As Exception
                If ex.Message = "Ya hay un DataReader abierto asociado a este Command, debe cerrarlo primero." Then
                    strRet = roConversions.ConvertHoursToTime(Hours)

                End If
            Finally

            End Try

            Return strRet

        End Function

        Private Shared Function HexConverter(c As System.Drawing.Color) As String
            Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
        End Function

        Public Shared Function ColorHexFormat(ByVal Color As Integer) As String

            Dim auxColor As Drawing.Color = Drawing.ColorTranslator.FromWin32(Color)
            Return HexConverter(auxColor)

        End Function

        ''' <summary>
        ''' Copia las reglas de acumulados de un empleado a otro. Opcionalmente se puede informar si substituir las reglas actuales o no.
        ''' </summary>
        ''' <param name="intIDSourceEmployee">Código del empleado origen.</param>
        ''' <param name="intIDDestinationEmployee">Código del empleado destino.</param>
        ''' <param name="bolDescartarReglasActuales">Indica si substituir las reglas actuales del empleado destino.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyAccrualRules(ByVal intIDSourceEmployee As Integer, ByVal intIDDestinationEmployee As Integer, ByVal bolDescartarReglasActuales As Boolean, ByRef _State As roBusinessState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                If bolDescartarReglasActuales Then
                    'Elimino las reglas que pudiera tener asignadas en empleado destino
                    strSQL = "@DELETE# EmployeeAccrualsRules where IdEmployee = " + intIDDestinationEmployee.ToString
                    ExecuteSql(strSQL)
                End If

                Dim intIdAccrualRule As Integer
                Dim intPosition As Integer
                Dim ruleAlreadyExists As Boolean = False

                ' Seleccionamos todas las reglas de acumulados que tiene asignadas el empleado origen
                strSQL = "@SELECT# IdAccrualsRules, Position from EmployeeAccrualsRules where IdEmployee = " + intIDSourceEmployee.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        intIdAccrualRule = Any2Integer(oRow("IdAccrualsRules"))
                        intPosition = Any2Integer(oRow("Position"))
                        If Not bolDescartarReglasActuales Then
                            'Compruebo si el empleado origen ya tenía esa regla. Si es así, no hago nada.
                            strSQL = "@SELECT# count(*) from EmployeeAccrualsRules where IdAccrualsRules = " + intIdAccrualRule.ToString
                            strSQL = strSQL + " and IdEmployee = " + intIDDestinationEmployee.ToString

                            ruleAlreadyExists = (Any2Integer(ExecuteScalar(strSQL)) > 0)
                        End If

                        If Not ruleAlreadyExists Then
                            strSQL = "@INSERT# INTO EmployeeAccrualsRules values (" + intIDDestinationEmployee.ToString + "," + intIdAccrualRule.ToString + "," + intPosition.ToString + ")"
                            ExecuteSql(strSQL)
                        End If

                    Next

                    bolRet = True

                End If

                If bolRet Then
                    ' Notificamos los cambios al servidor
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::CopyAccrualRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::CopyAccrualRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Cópia los valores de los límites anuales de un empleado a otro.
        ''' </summary>
        ''' <param name="intIDEmployeeSource"></param>
        ''' <param name="intIDEmployeeDestination"></param>
        ''' <param name="bolDiscardAnnualLimits"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyConceptAnnualLimits(ByVal intIDEmployeeSource As Integer, ByVal intIDEmployeeDestination As Integer, ByVal bolDiscardAnnualLimits As Boolean, ByVal _State As roBusinessState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String
                Dim bolOverwriteAll As Boolean

                Dim dblStartUpValue As Nullable(Of Double)
                Dim dblMaxValue As Nullable(Of Double)
                Dim dblMinValue As Nullable(Of Double)

                '* Obtenemos los limites definidos en el empleado origen.
                ' Sólo considero los ya aplicados o los aplicados en el año en curso
                '* Bucle principal
                strSQL = "@SELECT# * FROM EmployeeConceptAnnualLimits WHERE IDEmployee = " & intIDEmployeeSource.ToString
                ' PENDIENTE sSQL = sSQL & " LastUpdated "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then

                    If tb.Rows.Count > 0 Then

                        For Each oRow As DataRow In tb.Rows
                            bolOverwriteAll = True
                            '* Verificar si debemos machacar los límites coincidentes
                            If Not bolDiscardAnnualLimits Then
                                strSQL = "@SELECT# count(*) FROM EmployeeConceptAnnualLimits WHERE IDEmployee = " & intIDEmployeeDestination.ToString &
                                         " AND IDConcept =" & oRow("IDConcept") & " AND IDYear=" & oRow("IDYear")

                                If Any2Long(ExecuteScalar(strSQL)) > 0 Then bolOverwriteAll = False
                            End If

                            '* Preparamos el proceso por cada límite encontrado
                            If bolOverwriteAll Then

                                dblStartUpValue = Nothing
                                If Not IsDBNull(oRow("StartUpValue")) Then dblStartUpValue = oRow("StartUpValue")
                                dblMaxValue = Nothing
                                If Not IsDBNull(oRow("MaxValue")) Then dblMaxValue = oRow("MaxValue")
                                dblMinValue = Nothing
                                If Not IsDBNull(oRow("MinValue")) Then dblMinValue = oRow("MinValue")

                                bolRet = InsertAnnualLimits(intIDEmployeeDestination, oRow("IDConcept"), oRow("IDYear"), dblStartUpValue, dblMaxValue, dblMinValue, _State)
                                If Not bolRet Then Exit For
                            Else
                                bolRet = True
                            End If
                        Next
                    Else
                        bolRet = True
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::CopyConceptAnnualLimits")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::CopyConceptAnnualLimits")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Inserta los valores para el límite especificado
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="intIDConcept">Código del acumulado</param>
        ''' <param name="intYear">Año</param>
        ''' <param name="dblStartUpValue">Valor inicial del límite</param>
        ''' <param name="dblMaxValue">Valor màximo del límite</param>
        ''' <param name="dblMinValue">Valor mínimo del límite</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InsertAnnualLimits(ByVal intIDEmployee As Integer, ByVal intIDConcept As Integer, ByVal intYear As Integer, ByVal dblStartUpValue As Nullable(Of Double),
                                           ByVal dblMaxValue As Nullable(Of Double), ByVal dblMinValue As Nullable(Of Double), ByVal _State As roBusinessState) As Boolean
            '
            ' Se guardarán los valores límites y valores iniciales que aún no hayan sido aplicados, y también
            ' aquellos que hayan sido aplicados en el año en curso. Si el empleado destino ya tuviera
            ' esa definición de acumulado - año, entonces la respetaré o la sobreescribiré, según se me
            ' haya indicado desde la pantalla
            '
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                '* Elimiar posibles registros anteriores
                strSQL = "@DELETE# FROM EmployeeconceptAnnualLimits WHERE " &
                         " IDEmployee = " & intIDEmployee.ToString &
                         " AND IDConcept = " & intIDConcept.ToString &
                         " AND IDYear= " & intYear.ToString

                ExecuteSql(strSQL)

                '* Insertamos el nuevo registro
                strSQL = "@INSERT# INTO EmployeeConceptAnnualLimits (IDEmployee, IDConcept, StartUpValue, MaxValue, MinValue, LastupdatedYear, IDYear, LastUpdated) " &
                         "Values(" & intIDEmployee.ToString & ", " & intIDConcept.ToString & ", " & IIf(Not dblStartUpValue.HasValue, "NULL", dblStartUpValue.Value.ToString) & ", " &
                                 IIf(Not dblMaxValue.HasValue, "NULL", dblMaxValue.Value.ToString) & ", " & IIf(Not dblMinValue.HasValue, "NULL", dblMinValue.Value.ToString) & ", 0, " & intYear.ToString & ", NULL)"
                ExecuteSql(strSQL)

                '* Actualizar el proceso de límites anuales
                Dim xBeginDate As DateTime = roParameters.GetBeginYearDate(intYear) '  BeginYear(vsYear)
                Dim xEndYear As DateTime = xBeginDate.AddYears(1) ' DateAdd("yyyy", 1, sBeginDate)

                '* Eliminar los registros con startupvalue del año y concepto correspondiente
                strSQL = "@DELETE# FROM DailyAccruals WHERE IDemployee = " & intIDEmployee.ToString & " AND IDconcept=" & intIDConcept.ToString &
                         " AND StartUpValue = 1 " &
                         " AND Date >=" & Any2Time(xBeginDate).SQLSmallDateTime & " AND Date <" & Any2Time(xEndYear).SQLSmallDateTime
                ExecuteSql(strSQL)

                ' Ejecutar el cálculo en caso de que su ejecución fuese anterior al año introducido
                If DateDiff(DateInterval.Day, xBeginDate, Now.Date) > 0 Then
                    bolRet = ManualSingleDay_AnualLimits(intYear, intIDConcept, intIDEmployee, _State)
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::InsertAnnualLimits")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::InsertAnnualLimits")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Graba los límites anuales manualmente, para simular el proceso de acumulados.
        ''' </summary>
        ''' <param name="intYear">Año</param>
        ''' <param name="intIDConcept">Código de acumulado</param>
        ''' <param name="intIDEmployee">Código de empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ManualSingleDay_AnualLimits(ByVal intYear As Integer, ByVal intIDConcept As Integer, ByVal intIDEmployee As Integer, ByVal _State As roBusinessState) As Boolean
            ' Grabamos manualmente los datos para simular al proceso de acumulados
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                strSQL = "@SELECT# * FROM EmployeeConceptAnnualLimits WHERE IDEmployee=" & intIDEmployee.ToString &
                         " AND IDConcept = " & intIDConcept.ToString & " AND IDYear = " & intYear.ToString & " AND StartupValue IS NOT NULL"

                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then
                    ' Recorre la lista de valores iniciales a establecer
                    For Each oRow As DataRow In tb.Rows
                        ' Añade el valor anual inicial que se indica, siempre y cuando no exista ya
                        If Any2Long(oRow("LastupdatedYear")) = 0 Then
                            ' El valor inicial no se ha aplicado ningún año
                            strSQL = "@INSERT# INTO DailyAccruals(IDEmployee,IDConcept,Date,Value,CarryOver,StartupValue) " &
                                     "VALUES (" & intIDEmployee.ToString & ", " &
                                              intIDConcept.ToString & ", " & Any2Time(Now.Date).SQLSmallDateTime & ", " &
                                              CStr(oRow("StartupValue")).Replace(",", ".") & ",1,1)"
                            ExecuteSql(strSQL)
                            'If SQLExecute(Sql, m_Connection) = -1 Then Err.Raise(9432, "SQLExecute step 3 failed inserting accrual")

                            ' Actualizo el campo LastUpdatedYear con el valor del año siguiente (el año en que se ha creado el acumulado inicial
                            strSQL = "@UPDATE# EmployeeConceptAnnualLimits set LastUpdatedYear = " & intYear.ToString & ", LastUpdated = " & Any2Time(Now.Date).SQLSmallDateTime &
                                     " WHERE IDEmployee = " & intIDEmployee.ToString & " and IDConcept = " & intIDConcept.ToString & " AND IDYear = " & intYear.ToString
                            ExecuteSql(strSQL)
                            'If SQLExecute(Sql, m_Connection) = -1 Then Err.Raise(9432, "SQLExecute step 3 failed updating EmployeeConceptAnnualLimits")
                        End If
                    Next
                    bolRet = True
                Else
                    'gServerConn.LogMessage(roError, "ManualProccesAccruals: Error calculating StartupValues")
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::ManualSingleDay_AnualLimits")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::ManualSingleDay_AnualLimits")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract>
    Public Class roAutomaticAccrualCriteria

#Region "Declarations - Constructor"

        Private intAutomaticAccrualType As eAutomaticAccrualType
        Private oFactorType As eFactorType
        Private oUserField As VTUserFields.UserFields.roUserField
        Private dblFactorValue As Double
        Private oTypeAccrualDay As eAccrualDayType
        Private intTotalCauses As Integer
        Private intTotalShifts As Integer
        Private lstCauses As New Generic.List(Of Integer)
        Private lstShifts As New Generic.List(Of Integer)

        Public Sub New()
            Me.intAutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
            Me.oFactorType = eFactorType.DirectValue
            Me.oUserField = Nothing
            Me.oTypeAccrualDay = eAccrualDayType.AllDays
            lstCauses = New Generic.List(Of Integer)
            lstShifts = New Generic.List(Of Integer)
            Me.Shifts = New Generic.List(Of Integer)
            Me.Causes = New Generic.List(Of Integer)

        End Sub

        Public Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then

                ' Añadimos la composición a la colección
                Dim oDefinition As New roCollection(strXml)

                If oDefinition.Exists("FactorType") Then
                    Me.FactorType = oDefinition("FactorType")
                End If

                If oDefinition.Exists("FactorValue") And Me.FactorType = eFactorType.DirectValue Then
                    Me.FactorValue = oDefinition("FactorValue")
                End If

                If oDefinition.Exists("FactorField") And Me.FactorType = eFactorType.UserField Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(-1)

                    Me.oUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("FactorField"), UserFieldsTypes.Types.EmployeeField, False)
                End If

                If oDefinition.Exists("TypeAccrualDay") Then
                    Me.oTypeAccrualDay = oDefinition("TypeAccrualDay")
                End If

                If oDefinition.Exists("TotalCauses") Then
                    Me.TotalCauses = oDefinition("TotalCauses")
                End If

                If oDefinition.Exists("TotalShifts") Then
                    Me.TotalShifts = oDefinition("TotalShifts")
                End If

                If Me.TotalCauses > 0 Then
                    For i As Integer = 1 To Me.TotalCauses
                        If oDefinition.Exists("CauseCriteria_" & i.ToString) Then
                            Me.lstCauses.Add(Any2Integer(oDefinition("CauseCriteria_" & i.ToString)))
                        End If
                    Next
                End If

                If Me.TotalShifts > 0 Then
                    For i As Integer = 1 To Me.TotalShifts
                        If oDefinition.Exists("ShiftCriteria_" & i.ToString) Then
                            Me.lstShifts.Add(Any2Integer(oDefinition("ShiftCriteria_" & i.ToString)))
                        End If
                    Next
                End If

            End If

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property UserField() As VTUserFields.UserFields.roUserField
            Get
                Return Me.oUserField
            End Get
            Set(ByVal value As VTUserFields.UserFields.roUserField)
                Me.oUserField = value
            End Set
        End Property

        <DataMember>
        Public Property TypeAccrualDay() As eAccrualDayType
            Get
                Return Me.oTypeAccrualDay
            End Get
            Set(ByVal value As eAccrualDayType)
                Me.oTypeAccrualDay = value
            End Set
        End Property

        <DataMember>
        Public Property FactorType() As eFactorType
            Get
                Return Me.oFactorType
            End Get
            Set(ByVal value As eFactorType)
                Me.oFactorType = value
            End Set
        End Property

        <DataMember>
        Public Property FactorValue() As Double
            Get
                Return Me.dblFactorValue
            End Get
            Set(ByVal value As Double)
                Me.dblFactorValue = value
            End Set
        End Property

        <DataMember>
        Public Property TotalCauses() As Integer
            Get
                Return Me.intTotalCauses
            End Get
            Set(ByVal value As Integer)
                Me.intTotalCauses = value
            End Set
        End Property

        <DataMember>
        Public Property TotalShifts() As Integer
            Get
                Return Me.intTotalShifts
            End Get
            Set(ByVal value As Integer)
                Me.intTotalShifts = value
            End Set
        End Property

        <DataMember>
        Public Property Shifts() As Generic.List(Of Integer)
            Get
                Return Me.lstShifts
            End Get
            Set(ByVal value As Generic.List(Of Integer))
                Me.lstShifts = value
            End Set
        End Property

        <DataMember>
        Public Property Causes() As Generic.List(Of Integer)
            Get
                Return Me.lstCauses
            End Get
            Set(ByVal value As Generic.List(Of Integer))
                Me.lstCauses = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticAccrualType() As eAutomaticAccrualType
            Get
                Return Me.intAutomaticAccrualType
            End Get
            Set(ByVal value As eAutomaticAccrualType)
                Me.intAutomaticAccrualType = value
            End Set
        End Property

#End Region

#Region "Methods"

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.lstCauses Is Nothing Then
                Me.lstCauses = New Generic.List(Of Integer)
            End If

            If Me.lstShifts Is Nothing Then
                Me.lstShifts = New Generic.List(Of Integer)
            End If
        End Sub

        Public Function GetXml() As String
            Dim oAutomaticAccrualCriteria As New roCollection
            If Me.AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType Then
                oAutomaticAccrualCriteria.Add("FactorType", Me.FactorType)
                oAutomaticAccrualCriteria.Add("FactorValue", Me.FactorValue)
                If Not oUserField Is Nothing Then
                    oAutomaticAccrualCriteria.Add("FactorField", Me.oUserField.FieldName)
                End If
                oAutomaticAccrualCriteria.Add("TypeAccrualDay", Me.oTypeAccrualDay)
                oAutomaticAccrualCriteria.Add("TotalCauses", Me.TotalCauses)
                oAutomaticAccrualCriteria.Add("TotalShifts", Me.TotalShifts)

                If Me.Causes IsNot Nothing AndAlso Me.Causes.Count > 0 Then
                    For i As Integer = 0 To Me.TotalCauses - 1
                        oAutomaticAccrualCriteria.Add("CauseCriteria_" & (i + 1).ToString, Me.Causes(i))
                    Next
                End If

                If Me.Shifts IsNot Nothing AndAlso Me.Shifts.Count > 0 Then
                    For i As Integer = 0 To Me.TotalShifts - 1
                        oAutomaticAccrualCriteria.Add("ShiftCriteria_" & (i + 1).ToString, Me.Shifts(i))
                    Next
                End If
            End If

            Return oAutomaticAccrualCriteria.XML

        End Function

#End Region

    End Class

    <DataContract>
    Public Class roExpiredHoursCriteria

#Region "Declarations - Constructor"

        Private intExpiredHoursType As eExpiredHoursType
        Private lstLabAgreementsAffected As New Generic.List(Of Integer)
        Private dblValue As Double

        Public Sub New()
            Me.intExpiredHoursType = eExpiredHoursType.NotExpired
            Me.lstLabAgreementsAffected = New List(Of Integer)
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property oValue() As Double
            Get
                Return Me.dblValue
            End Get
            Set(ByVal value As Double)
                Me.dblValue = value
            End Set
        End Property

        <DataMember>
        Public Property ExpiredHoursType() As eExpiredHoursType
            Get
                Return Me.intExpiredHoursType
            End Get
            Set(ByVal value As eExpiredHoursType)
                Me.intExpiredHoursType = value
            End Set
        End Property


#End Region

#Region "Methods"
        Public Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then
                ' Añadimos la composición a la colección
                Dim oDefinition As New roCollection(strXml)

                If oDefinition.Exists("Value") Then
                    Me.oValue = oDefinition("Value")
                End If

                If oDefinition.Exists("ExpiredHoursType") Then
                    Me.ExpiredHoursType = Any2Integer(oDefinition("ExpiredHoursType"))
                End If
            End If

        End Sub

        Public Function GetXml() As String
            Dim oExpiredHoursTypeCriteria As New roCollection
            oExpiredHoursTypeCriteria.Add("Value", Me.oValue)
            oExpiredHoursTypeCriteria.Add("ExpiredHoursType", Me.ExpiredHoursType)
            Return oExpiredHoursTypeCriteria.XML
        End Function

#End Region

    End Class


End Namespace