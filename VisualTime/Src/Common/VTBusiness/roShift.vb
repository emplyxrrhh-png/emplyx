Imports System.Data.Common
Imports System.Math
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base
Imports Robotics.Base.VTSelectorManager

Namespace Shift

    <DataContract(),
    KnownType(GetType(roShiftLayer))>
    Public Class roShift

#Region "Declarations - Constructor"

        Private oState As roShiftState

        Private intID As Integer
        Private bolNewShift As Integer
        Private strName As String
        Private strDescription As String
        Private intColor As Integer
        Private sngExpectedWorkingHours As Single
        Private bolIsObsolete As Boolean
        Private bolIsTemplate As Boolean
        Private datStartLimit As DateTime
        Private datEndLimit As DateTime
        Private bolManualLimit As Boolean
        Private strShortName As String
        Private strTypeShift As String
        Private intIDGroup As Integer
        Private bolWebVisible As Boolean
        Private bolWebLaboral As Boolean
        Private intIDConceptBalance As Integer
        Private intIDConceptRequestNextYear As Integer
        Private intIDCauseHolidays As Integer
        Private bolAreWorkingDays As Boolean
        Private arrayLayers As New ArrayList
        Private strAdvancedParameters As String
        Private eShiftType As ShiftType     ' Tipo de horario
        Private xStartFloating As Nullable(Of DateTime) ' Hora de inicio del horario flotante por defecto (Día del horario: 1899/12/30 HH:mm, Día anterior: 1899/12/29 HH:mm, Día posterior: 1899/12/31 HH:mm)
        Private isFlexible As Boolean
        Private isMandatory As Boolean
        Private isComplementary As Boolean

        Private bolEnableNotifyExit As Boolean
        Private bolEnableNotifyBeforeStart As Boolean
        Private bolEnableNotifyAfterStart As Boolean
        Private intNotifyEmployeeExitAt As Integer
        Private intNotifyEmployeeBeforeAt As Integer
        Private intNotifyEmployeeAfterAt As Integer
        Private bolEnableCompleteExit As Boolean
        Private intCompleteExitAt As Integer
        Private intWhoToNotifyBefore As Integer
        Private intWhoToNotifyAfter As Integer

        Private intIDCenter As Integer
        Private bolApplyCenterOnAbsence As Boolean
        Private bolAllowComplementary As Boolean
        Private sngBreakHours As Single
        Private bolAllowFloatingData As Boolean

        Private bolAllowComplementary1 As Boolean = False
        Private bolAllowModifyIniHour1 As Boolean = False
        Private bolAllowModifyDuration1 As Boolean = False
        Private bolAllowModifyIniHour2 As Boolean = False
        Private bolAllowModifyDuration2 As Boolean = False
        Private intLayersCount = 0
        Private strExportName As String

        Private intTypeHolidayValue As HolidayValueType

        Private dblHolidayValue As Double

        Private datStartDate As Nullable(Of Date)

        Private oSimpleRules As Generic.List(Of roShiftRule)

        Private oDailyRules As Generic.List(Of roShiftDailyRule)

        Private oTimeZones As Generic.List(Of roShiftTimeZone)

        Private oAssignments As Generic.List(Of roShiftAssignment)

        Private intVisibilityPermissions As Integer = 1 'Por defecto nadie puede ver horarios nuevos desde Portal
        Private lstVisibilityCriteria As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition) 'List(Of roCauseCriteria)
        Private visibilityCollectivesValue As String
        Private dblDailyFactor As Double
        Private oPunchesPattern As roShiftPunchesPattern

        Public Sub New()
            Me.oState = New roShiftState()
            Me.oSimpleRules = New Generic.List(Of roShiftRule)
            Me.oDailyRules = New Generic.List(Of roShiftDailyRule)
            Me.oTimeZones = New Generic.List(Of roShiftTimeZone)
            Me.oAssignments = New Generic.List(Of roShiftAssignment)
            Me.ID = -1
            datStartDate = Nothing
            isFlexible = False
            isMandatory = False
            isComplementary = False
            intTypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value
            dblDailyFactor = 1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.oSimpleRules = New Generic.List(Of roShiftRule)
            Me.oDailyRules = New Generic.List(Of roShiftDailyRule)

            Me.oTimeZones = New Generic.List(Of roShiftTimeZone)
            Me.oAssignments = New Generic.List(Of roShiftAssignment)
            Me.ID = _ID
            datStartDate = Nothing
            isFlexible = False
            isMandatory = False
            isComplementary = False
            intTypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                ' Actualizamos el IDShift de los datos relacionados
                If Me.arrayLayers IsNot Nothing Then
                    For Each oLayer As roShiftLayer In arrayLayers
                        oLayer.IDShift = intID
                    Next
                End If
                If Me.oSimpleRules IsNot Nothing Then
                    For Each oRule As roShiftRule In Me.oSimpleRules
                        oRule.IDShift = intID
                    Next
                End If

                If Me.oDailyRules IsNot Nothing Then
                    For Each oRule As roShiftDailyRule In Me.oDailyRules
                        oRule.IDShift = intID
                    Next
                End If

                If Me.oTimeZones IsNot Nothing Then
                    For Each oTimeZone As roShiftTimeZone In Me.oTimeZones
                        oTimeZone.IDShift = intID
                    Next
                End If
                If Me.oAssignments IsNot Nothing Then
                    For Each oShiftAssignment As roShiftAssignment In Me.oAssignments
                        oShiftAssignment.IDShift = intID
                    Next
                End If
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property Color() As Integer
            Get
                Return intColor
            End Get
            Set(ByVal value As Integer)
                intColor = value
            End Set
        End Property

        <DataMember()>
        Public Property ExpectedWorkingHours() As Single
            Get
                Return sngExpectedWorkingHours
            End Get
            Set(ByVal value As Single)
                sngExpectedWorkingHours = value
            End Set
        End Property

        <DataMember()>
        Public Property IsObsolete() As Boolean
            Get
                Return bolIsObsolete
            End Get
            Set(ByVal value As Boolean)
                bolIsObsolete = value
            End Set
        End Property
        <DataMember()>
        Public Property IsTemplate() As Boolean
            Get
                Return bolIsTemplate
            End Get
            Set(ByVal value As Boolean)
                bolIsTemplate = value
            End Set
        End Property
        <DataMember()>
        Public Property StartLimit() As DateTime
            Get
                Return datStartLimit
            End Get
            Set(ByVal value As DateTime)
                datStartLimit = value
            End Set
        End Property
        <DataMember()>
        Public Property EndLimit() As DateTime
            Get
                Return datEndLimit
            End Get
            Set(ByVal value As DateTime)
                datEndLimit = value
            End Set
        End Property
        <DataMember()>
        Public Property ManualLimit() As Boolean
            Get
                Return bolManualLimit
            End Get
            Set(ByVal value As Boolean)
                bolManualLimit = value
            End Set
        End Property
        <DataMember()>
        Public Property ShortName() As String
            Get
                Return strShortName
            End Get
            Set(ByVal value As String)
                strShortName = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeShift() As String
            Get
                Return strTypeShift
            End Get
            Set(ByVal value As String)
                strTypeShift = value
            End Set
        End Property
        <DataMember()>
        Public Property IDGroup() As Integer
            Get
                Return intIDGroup
            End Get
            Set(ByVal value As Integer)
                intIDGroup = value
            End Set
        End Property
        <DataMember()>
        Public Property IDCenter() As Integer
            Get
                Return intIDCenter
            End Get
            Set(ByVal value As Integer)
                intIDCenter = value
            End Set
        End Property
        <DataMember()>
        Public Property WebVisible() As Boolean
            Get
                Return bolWebVisible
            End Get
            Set(ByVal value As Boolean)
                bolWebVisible = value
            End Set
        End Property
        <DataMember()>
        Public Property ApplyCenterOnAbsence() As Boolean
            Get
                Return bolApplyCenterOnAbsence
            End Get
            Set(ByVal value As Boolean)
                bolApplyCenterOnAbsence = value
            End Set
        End Property
        <DataMember()>
        Public Property WebLaboral() As Boolean
            Get
                Return bolWebLaboral
            End Get
            Set(ByVal value As Boolean)
                bolWebLaboral = value
            End Set
        End Property
        <DataMember()>
        Public Property IDConceptBalance() As Integer
            Get
                Return intIDConceptBalance
            End Get
            Set(ByVal value As Integer)
                intIDConceptBalance = value
            End Set
        End Property

        Public Property IDConceptRequestNextYear() As Integer
            Get
                Return intIDConceptRequestNextYear
            End Get
            Set(ByVal value As Integer)
                intIDConceptRequestNextYear = value
            End Set
        End Property
        <DataMember()>
        Public Property IDCauseHolidays() As Integer
            Get
                Return intIDCauseHolidays
            End Get
            Set(ByVal value As Integer)
                intIDCauseHolidays = value
            End Set
        End Property
        <DataMember()>
        Public Property AllowComplementary() As Boolean
            Get
                Return bolAllowComplementary
            End Get
            Set(ByVal value As Boolean)
                bolAllowComplementary = value
            End Set
        End Property
        <DataMember()>
        Public Property AreWorkingDays() As Boolean
            Get
                Return bolAreWorkingDays
            End Get
            Set(ByVal value As Boolean)
                bolAreWorkingDays = value
            End Set
        End Property
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
        Public Property StartDate() As Nullable(Of Date)
            Get
                Return datStartDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                datStartDate = value
            End Set
        End Property

        <DataMember()>
        <XmlArray("Layers"), XmlArrayItem("roShiftLayer", GetType(roShiftLayer))>
        Public Property Layers() As ArrayList
            Get
                Return arrayLayers
            End Get
            Set(ByVal value As ArrayList)
                arrayLayers = value
            End Set
        End Property
        <DataMember()>
        Public Property TimeZones() As Generic.List(Of roShiftTimeZone)
            Get
                Return oTimeZones
            End Get
            Set(ByVal value As Generic.List(Of roShiftTimeZone))
                oTimeZones = value
            End Set
        End Property

        <DataMember()>
        Public Property SimpleRules() As Generic.List(Of roShiftRule)
            Get
                Return Me.oSimpleRules
            End Get
            Set(ByVal value As Generic.List(Of roShiftRule))
                Me.oSimpleRules = value
            End Set
        End Property

        <DataMember()>
        Public Property DailyRules() As Generic.List(Of roShiftDailyRule)
            Get
                Return Me.oDailyRules
            End Get
            Set(ByVal value As Generic.List(Of roShiftDailyRule))
                Me.oDailyRules = value
            End Set
        End Property
        <DataMember()>
        Public Property AdvancedParameters() As String
            Get
                Return Me.strAdvancedParameters
            End Get
            Set(ByVal value As String)
                Me.strAdvancedParameters = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de horario
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ShiftType() As ShiftType
            Get
                Return Me.eShiftType
            End Get
            Set(ByVal value As ShiftType)
                Me.eShiftType = value
            End Set
        End Property

        ''' <summary>
        ''' Hora de inicio del horario flotante. (Día del horario: 1899/12/30 HH:mm, Día anterior: 1899/12/29 HH:mm, Día posterior: 1899/12/31 HH:mm)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StartFloating() As Nullable(Of DateTime)
            Get
                Return Me.xStartFloating
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xStartFloating = value
            End Set
        End Property
        <DataMember()>
        Public Property Assignments() As Generic.List(Of roShiftAssignment)
            Get
                Return Me.oAssignments
            End Get
            Set(ByVal value As Generic.List(Of roShiftAssignment))
                Me.oAssignments = value
            End Set
        End Property
        <DataMember()>
        Public Property VisibilityPermissions() As Integer
            Get
                Return intVisibilityPermissions
            End Get
            Set(ByVal value As Integer)
                intVisibilityPermissions = value
            End Set
        End Property
        <DataMember()>
        Public Property VisibilityCriteria() As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Get
                Return Me.lstVisibilityCriteria
            End Get
            Set(ByVal value As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition))
                Me.lstVisibilityCriteria = value
            End Set
        End Property

        <DataMember()>
        Public Property VisibilityCollectives() As String
            Get
                Return Me.visibilityCollectivesValue
            End Get
            Set(ByVal value As String)
                Me.visibilityCollectivesValue = value
            End Set
        End Property

        <DataMember()>
        Public Property BreakHours() As Single
            Get
                Return sngBreakHours
            End Get
            Set(ByVal value As Single)
                sngBreakHours = value
            End Set
        End Property
        <DataMember()>
        Public Property AllowFloatingData() As Boolean
            Get
                Return bolAllowFloatingData
            End Get
            Set(ByVal value As Boolean)
                bolAllowFloatingData = value
            End Set
        End Property
        <DataMember()>
        Public Property ExportName() As String
            Get
                Return strExportName
            End Get
            Set(ByVal value As String)
                strExportName = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeHolidayValue() As HolidayValueType
            Get
                Return intTypeHolidayValue
            End Get
            Set(ByVal value As HolidayValueType)
                intTypeHolidayValue = value
            End Set
        End Property
        <DataMember()>
        Public Property HolidayValue() As Double
            Get
                Return dblHolidayValue
            End Get
            Set(ByVal value As Double)
                dblHolidayValue = value
            End Set
        End Property

        <DataMember()>
        Public Property DailyFactor() As Double
            Get
                Return dblDailyFactor
            End Get
            Set(ByVal value As Double)
                dblDailyFactor = value
            End Set
        End Property

        <DataMember()>
        Public Property EnableNotifyExit() As Boolean
            Get
                Return bolEnableNotifyExit
            End Get
            Set(ByVal value As Boolean)
                bolEnableNotifyExit = value
            End Set
        End Property

        <DataMember()>
        Public Property EnableNotifyBeforeStart() As Boolean
            Get
                Return bolEnableNotifyBeforeStart
            End Get
            Set(ByVal value As Boolean)
                bolEnableNotifyBeforeStart = value
            End Set
        End Property
        <DataMember()>
        Public Property EnableNotifyAfterStart() As Boolean
            Get
                Return bolEnableNotifyAfterStart
            End Get
            Set(ByVal value As Boolean)
                bolEnableNotifyAfterStart = value
            End Set
        End Property
        <DataMember()>
        Public Property CompleteExitAt() As Integer
            Get
                Return intCompleteExitAt
            End Get
            Set(ByVal value As Integer)
                intCompleteExitAt = value
            End Set
        End Property

        <DataMember()>
        Public Property WhoToNotifyBefore() As Integer
            Get
                Return intWhoToNotifyBefore
            End Get
            Set(ByVal value As Integer)
                intWhoToNotifyBefore = value
            End Set
        End Property

        <DataMember()>
        Public Property WhoToNotifyAfter() As Integer
            Get
                Return intWhoToNotifyAfter
            End Get
            Set(ByVal value As Integer)
                intWhoToNotifyAfter = value
            End Set
        End Property

        <DataMember()>
        Public Property EnableCompleteExit() As Boolean
            Get
                Return bolEnableCompleteExit
            End Get
            Set(ByVal value As Boolean)
                bolEnableCompleteExit = value
            End Set
        End Property

        <DataMember()>
        Public Property NotifyEmployeeExitAt() As Integer
            Get
                Return intNotifyEmployeeExitAt
            End Get
            Set(ByVal value As Integer)
                intNotifyEmployeeExitAt = value
            End Set
        End Property

        <DataMember()>
        Public Property NotifyEmployeeBeforeAt() As Integer
            Get
                Return intNotifyEmployeeBeforeAt
            End Get
            Set(ByVal value As Integer)
                intNotifyEmployeeBeforeAt = value
            End Set
        End Property
        <DataMember()>
        Public Property NotifyEmployeeAfterAt() As Integer
            Get
                Return intNotifyEmployeeAfterAt
            End Get
            Set(ByVal value As Integer)
                intNotifyEmployeeAfterAt = value
            End Set
        End Property

        Public Property PunchesPattern As roShiftPunchesPattern
            Get
                Return oPunchesPattern
            End Get
            Set(value As roShiftPunchesPattern)
                oPunchesPattern = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)
            Dim strQuery As String
            Dim oDataset As Data.DataSet
            Dim oDatareader As Data.Common.DbDataReader

            If Me.ID = -1 Then Exit Sub

            strQuery = " @SELECT# * from Shifts "
            strQuery = strQuery & " Where ID = " & Me.ID

            Try

                oDataset = CreateDataSet(strQuery)

                If oDataset IsNot Nothing Then
                    oDatareader = oDataset.CreateDataReader()

                    If oDatareader IsNot Nothing Then
                        If oDatareader.HasRows Then
                            If oDatareader.Read() Then

                                Me.intID = oDatareader("ID")
                                Me.strName = oDatareader("Name")
                                Me.Description = Any2String(oDatareader("Description"))
                                Me.intColor = IIf(Not IsDBNull(oDatareader("Color")), oDatareader("Color"), 0)
                                Me.sngExpectedWorkingHours = oDatareader("ExpectedWorkingHours")
                                Me.bolIsObsolete = oDatareader("IsObsolete")
                                Me.bolIsTemplate = oDatareader("IsTemplate")
                                Me.datStartLimit = oDatareader("StartLimit")
                                Me.datEndLimit = oDatareader("EndLimit")
                                Me.bolManualLimit = oDatareader("ManualLimit")
                                Me.strShortName = IIf(Not IsDBNull(oDatareader("ShortName")), oDatareader("ShortName"), "")
                                Me.strTypeShift = IIf(Not IsDBNull(oDatareader("TypeShift")), oDatareader("TypeShift"), "")
                                Me.intIDGroup = oDatareader("IDGroup")
                                Me.bolWebVisible = oDatareader("WebVisible")
                                Me.bolWebLaboral = IIf(Not IsDBNull(oDatareader("WebLaboral")), oDatareader("WebLaboral"), False)
                                Me.intIDConceptBalance = IIf(Not IsDBNull(oDatareader("IDConceptBalance")), oDatareader("IDConceptBalance"), 0)
                                Me.intIDConceptRequestNextYear = IIf(Not IsDBNull(oDatareader("IDConceptRequestNextYear")), oDatareader("IDConceptRequestNextYear"), 0)
                                Me.intIDCauseHolidays = IIf(Not IsDBNull(oDatareader("IDCauseHolidays")), oDatareader("IDCauseHolidays"), 0)
                                Me.bolAreWorkingDays = IIf(Not IsDBNull(oDatareader("AreWorkingDays")), oDatareader("AreWorkingDays"), True)
                                Me.strAdvancedParameters = IIf(Not IsDBNull(oDatareader("AdvancedParameters")), oDatareader("AdvancedParameters"), "")
                                Me.bolEnableNotifyExit = roTypes.Any2Boolean(oDatareader("EnableNotifyExit"))
                                Me.bolEnableNotifyBeforeStart = roTypes.Any2Boolean(oDatareader("EnableNotifyBefore"))
                                Me.bolEnableNotifyAfterStart = roTypes.Any2Boolean(oDatareader("EnableNotifyAfter"))
                                Me.intCompleteExitAt = roTypes.Any2Integer(oDatareader("CompleteExitAt"))

                                Me.bolEnableCompleteExit = roTypes.Any2Boolean(oDatareader("EnableCompleteExit"))
                                Me.intNotifyEmployeeExitAt = roTypes.Any2Integer(oDatareader("NotifyEmployeeExitAt"))
                                Me.intNotifyEmployeeAfterAt = roTypes.Any2Integer(oDatareader("NotifyEmployeeAfterAt"))
                                Me.intNotifyEmployeeBeforeAt = roTypes.Any2Integer(oDatareader("NotifyEmployeeBeforeAt"))
                                Me.intWhoToNotifyAfter = roTypes.Any2Integer(oDatareader("WhoToNotifyAfter"))
                                Me.intWhoToNotifyBefore = roTypes.Any2Integer(oDatareader("WhoToNotifyBefore"))

                                Me.intIDCenter = IIf(IsDBNull(oDatareader("IDCenter")), 0, oDatareader("IDCenter"))
                                Me.bolApplyCenterOnAbsence = IIf(IsDBNull(oDatareader("ApplyCenterOnAbsence")), False, oDatareader("ApplyCenterOnAbsence"))
                                bolAllowComplementary = IIf(IsDBNull(oDatareader("AllowComplementary")), False, oDatareader("AllowComplementary"))
                                sngBreakHours = IIf(IsDBNull(oDatareader("BreakHours")), 0, oDatareader("BreakHours"))
                                bolAllowFloatingData = IIf(IsDBNull(oDatareader("AllowFloatingData")), False, oDatareader("AllowFloatingData"))
                                strExportName = IIf(Not IsDBNull(oDatareader("Export")), oDatareader("Export"), "")
                                intTypeHolidayValue = IIf(Not IsDBNull(oDatareader("TypeHolidayValue")), oDatareader("TypeHolidayValue"), HolidayValueType.ExpectedWorkingHours_Value)

                                dblHolidayValue = IIf(Not IsDBNull(oDatareader("HolidayValue")), oDatareader("HolidayValue"), 0)

                                dblDailyFactor = IIf(Not IsDBNull(oDatareader("DailyFactor")), oDatareader("DailyFactor"), 1)

                                Dim oLicense As New roServerLicense
                                Dim bMultipleShifts As Boolean = oLicense.FeatureIsInstalled("Feature\MultipleShifts")

                                '->Dim bolIsFloating As Boolean = (Me.bolMultipleShifts And Any2Boolean(oDatareader("IsFloating")))
                                Dim bolIsFloating As Boolean = (bMultipleShifts And Any2Boolean(oDatareader("IsFloating")))
                                Select Case Any2Integer(oDatareader("ShiftType"))
                                    Case 0, 1
                                        If bolIsFloating Then
                                            Me.eShiftType = ShiftType.NormalFloating
                                        Else
                                            Me.eShiftType = ShiftType.Normal
                                        End If

                                    Case 2
                                        Me.eShiftType = ShiftType.Vacations
                                End Select

                                If Not IsDBNull(oDatareader("StartFloating")) Then
                                    Me.xStartFloating = oDatareader("StartFloating")
                                Else
                                    Me.xStartFloating = Nothing
                                End If

                                LoadLayers(Me.intID)

                                Me.oSimpleRules = roShiftRule.GetShiftRultes(Me.intID, ShiftRuleType.Simple, Me.oState)

                                Me.oDailyRules = roShiftRule.GetDailyShiftRules(Me.intID, Me.oState)

                                Me.oTimeZones = roShiftTimeZone.GetShiftTimeZones(Me.intID, Me.oState)

                                Me.oAssignments = roShiftAssignment.GetShiftAssignments(Me.intID, Me.oState)

                                Me.VisibilityPermissions = roTypes.Any2Integer(oDatareader("VisibilityPermissions"))
                                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                                roBusinessState.CopyTo(Me.oState, oUserFieldState)
                                If Me.VisibilityPermissions = 2 Then
                                    Me.lstVisibilityCriteria = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oDatareader("VisibilityCriteria")), oUserFieldState, False)
                                End If
                                If Me.VisibilityPermissions = 3 Then
                                    Me.visibilityCollectivesValue = roTypes.Any2String(oDatareader("VisibilityCriteria"))
                                End If
                                roBusinessState.CopyTo(oUserFieldState, Me.oState)

                                ' Patrón de fichajes para Declaración de la Jornada
                                Me.oPunchesPattern = LoadPunchesPattern(Me.intID, Me.oState)

                                ' Auditamos consulta horario
                                If bAudit Then
                                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                    oState.AddAuditParameter(tbParameters, "{ShiftName}", Me.Name, "", 1)
                                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShift, Me.Name, tbParameters, -1)
                                End If

                            End If
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::Load")
            Finally

            End Try

        End Sub

        Public Sub LoadGeneral(ByVal _IDShift As Integer)
            Dim bolRet As Boolean = True
            Dim strQuery As String
            Dim oDataset As Data.DataTable

            Me.intID = -1

            strQuery = " @SELECT# * from Shifts "
            strQuery = strQuery & " Where ID = " & _IDShift.ToString

            Try
                oDataset = CreateDataTable(strQuery, "")

                If oDataset IsNot Nothing Then
                    If oDataset.Rows.Count > 0 Then
                        Me.intID = oDataset.Rows(0)("ID")
                        Me.strName = oDataset.Rows(0)("Name")
                        Me.Description = oDataset.Rows(0)("Description")
                        Me.intColor = IIf(Not IsDBNull(oDataset.Rows(0)("Color")), oDataset.Rows(0)("Color"), 0)
                        Me.sngExpectedWorkingHours = oDataset.Rows(0)("ExpectedWorkingHours")
                        Me.bolIsObsolete = oDataset.Rows(0)("IsObsolete")
                        Me.bolIsTemplate = oDataset.Rows(0)("IsTemplate")
                        Me.datStartLimit = oDataset.Rows(0)("StartLimit")
                        Me.datEndLimit = oDataset.Rows(0)("EndLimit")
                        Me.bolManualLimit = oDataset.Rows(0)("ManualLimit")
                        Me.strShortName = IIf(Not IsDBNull(oDataset.Rows(0)("ShortName")), oDataset.Rows(0)("ShortName"), "")
                        Me.strTypeShift = IIf(Not IsDBNull(oDataset.Rows(0)("TypeShift")), oDataset.Rows(0)("TypeShift"), "")
                        Me.intIDGroup = oDataset.Rows(0)("IDGroup")
                        Me.bolWebVisible = oDataset.Rows(0)("WebVisible")
                        Me.bolWebLaboral = IIf(Not IsDBNull(oDataset.Rows(0)("WebLaboral")), oDataset.Rows(0)("WebLaboral"), False)
                        Me.intIDConceptBalance = IIf(Not IsDBNull(oDataset.Rows(0)("IDConceptBalance")), oDataset.Rows(0)("IDConceptBalance"), 0)
                        Me.intIDConceptRequestNextYear = IIf(Not IsDBNull(oDataset.Rows(0)("IDConceptRequestNextYear")), oDataset.Rows(0)("IDConceptRequestNextYear"), 0)
                        Me.intIDCauseHolidays = IIf(Not IsDBNull(oDataset.Rows(0)("IDCauseHolidays")), oDataset.Rows(0)("IDCauseHolidays"), 0)
                        Me.bolAreWorkingDays = IIf(Not IsDBNull(oDataset.Rows(0)("AreWorkingDays")), oDataset.Rows(0)("AreWorkingDays"), True)
                        Me.strAdvancedParameters = IIf(Not IsDBNull(oDataset.Rows(0)("AdvancedParameters")), oDataset.Rows(0)("AdvancedParameters"), "")

                        Me.bolEnableCompleteExit = roTypes.Any2Boolean(oDataset.Rows(0)("EnableCompleteExit"))
                        Me.intCompleteExitAt = roTypes.Any2Integer(oDataset.Rows(0)("CompleteExitAt"))

                        Me.bolEnableNotifyExit = roTypes.Any2Boolean(oDataset.Rows(0)("EnableNotifyExit"))
                        Me.intNotifyEmployeeExitAt = roTypes.Any2Integer(oDataset.Rows(0)("NotifyEmployeeExitAt"))

                        bolAllowComplementary = IIf(IsDBNull(oDataset.Rows(0)("AllowComplementary")), False, oDataset.Rows(0)("AllowComplementary"))

                        Me.intIDCenter = IIf(IsDBNull(oDataset.Rows(0)("IDCenter")), 0, oDataset.Rows(0)("IDCenter"))
                        Me.bolApplyCenterOnAbsence = IIf(IsDBNull(oDataset.Rows(0)("ApplyCenterOnAbsence")), False, oDataset.Rows(0)("ApplyCenterOnAbsence"))

                        strExportName = IIf(Not IsDBNull(oDataset.Rows(0)("Export")), oDataset.Rows(0)("Export"), "")

                        intTypeHolidayValue = IIf(Not IsDBNull(oDataset.Rows(0)("TypeHolidayValue")), oDataset.Rows(0)("TypeHolidayValue"), HolidayValueType.ExpectedWorkingHours_Value)

                        dblHolidayValue = IIf(Not IsDBNull(oDataset.Rows(0)("HolidayValue")), oDataset.Rows(0)("HolidayValue"), 0)

                        dblDailyFactor = IIf(Not IsDBNull(oDataset.Rows(0)("DailyFactor")), oDataset.Rows(0)("DailyFactor"), 1)

                        Dim bolIsFloating As Boolean = Any2Boolean(oDataset.Rows(0)("IsFloating"))
                        Select Case Any2Integer(oDataset.Rows(0)("ShiftType"))
                            Case 0, 1
                                If bolIsFloating Then
                                    Me.eShiftType = ShiftType.NormalFloating
                                Else
                                    Me.eShiftType = ShiftType.Normal
                                End If

                            Case 2
                                Me.eShiftType = ShiftType.Vacations
                        End Select

                        If Not IsDBNull(oDataset.Rows(0)("StartFloating")) Then
                            Me.xStartFloating = oDataset.Rows(0)("StartFloating")
                        Else
                            Me.xStartFloating = Nothing
                        End If

                        sngBreakHours = IIf(IsDBNull(oDataset.Rows(0)("BreakHours")), 0, oDataset.Rows(0)("BreakHours"))
                        bolAllowFloatingData = IIf(IsDBNull(oDataset.Rows(0)("AllowFloatingData")), False, oDataset.Rows(0)("AllowFloatingData"))

                        If bolAllowFloatingData Or bolAllowComplementary Then
                            ' Si el horario tiene datos flotantes o complementarias, debemos cargar las franjas del horario
                            Me.LoadLayers(Me.intID, False)
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::LoadGeneral")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::LoadGeneral")
            Finally

            End Try

        End Sub

        Private Sub LoadLayers(ByVal ShiftID As Integer, Optional ByVal bAudit As Boolean = False)
            Dim tb As DataTable = Nothing
            Dim oLayer As roShiftLayer = Nothing
            Dim oLayerChild As roShiftLayer = Nothing
            Dim strShiftLayerTypes As String = ""

            Try
                oState.UpdateStateInfo()

                Dim strSQL As String = " @SELECT# * from sysroShiftsLayers Where (IDShift = " & ShiftID & " And ParentLayerID IS NULL) OR (IDShift = " & ShiftID & " And ParentLayerID = 0) Order by IDType "
                tb = CreateDataTable(strSQL, "sysroShiftsLayers")

                For Each dRow As DataRow In tb.Rows
                    oLayer = New roShiftLayer(oState)

                    oLayer.ID = dRow("ID")
                    oLayer.IDShift = dRow("IDShift")
                    oLayer.LayerType = dRow("IDType")
                    oLayer.ParentID = IIf(IsDBNull(dRow("ParentLayerID")), -1, dRow("ParentLayerID"))
                    oLayer.ParseLayerData(dRow("Definition"))

                    Dim tbChilds As DataTable = CreateDataTable(" @SELECT# * from sysroShiftsLayers Where IDShift = " & ShiftID & " And ParentLayerID = " & dRow("ID") & " Order by IDType ", "sysroShiftLayersChilds")
                    If tbChilds.Rows.Count > 0 Then
                        oLayer.ChildLayers = New Generic.List(Of roShiftLayer)
                        'Fem consulta per carregar els layers fills
                        For Each dRowChild As DataRow In tbChilds.Rows
                            oLayerChild = New roShiftLayer(oState)
                            oLayerChild.ID = dRowChild("ID")
                            oLayerChild.IDShift = dRowChild("IDShift")
                            oLayerChild.LayerType = dRowChild("IDType")
                            oLayerChild.ParentID = IIf(IsDBNull(dRowChild("ParentLayerID")), -1, dRowChild("ParentLayerID"))
                            oLayerChild.ParseLayerData(dRowChild("Definition"))

                            oLayer.ChildLayers.Add(oLayerChild)

                            strShiftLayerTypes &= IIf(strShiftLayerTypes <> "", ",", "") & System.Enum.GetName(GetType(roLayerTypes), oLayerChild.LayerType)
                        Next
                    Else
                        oLayer.ChildLayers = Nothing
                    End If

                    arrayLayers.Add(oLayer)

                    strShiftLayerTypes &= IIf(strShiftLayerTypes <> "", ",", "") & System.Enum.GetName(GetType(roLayerTypes), oLayer.LayerType)
                Next

                If tb.Rows.Count > 0 And bAudit Then
                    ' Auditamos consulta múltiple capas horario
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ShiftLayerTypes}", strShiftLayerTypes, "", 1)
                    oState.AddAuditParameter(tbParameters, "{ShiftName}", Me.Name, "", 1)
                    oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tShiftLayer, Me.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::LoadLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::LoadLayers")
            Finally

            End Try
        End Sub

        Private Shared Function HasPermissionOverBusinessGroup(ByVal oState As roShiftState, ByVal idPassport As Integer, ByVal strBusinessGroup As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ID IN(@SELECT# isnull(IDGroupFeature,-1) from sysroPassports WHERE id = " & oState.IDPassport & " ) "
                Dim oRet As System.Data.DataTable = CreateDataTable(strQuery, "")
                If oRet IsNot Nothing Then
                    If oRet.Rows.Count > 0 Then
                        If roTypes.Any2String(oRet.Rows(0)("BusinessGroupList")) = String.Empty Then
                            bolRet = True
                        Else
                            bolRet = roTypes.Any2String(oRet.Rows(0)("BusinessGroupList")).Contains(strBusinessGroup)
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::HasPermissionOverBusinessGroup")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function ShiftIsAllowed(ByVal oState As roShiftState, ByVal intIDShift As Integer, Optional ByVal intIDShift2 As Integer = -1, Optional ByVal intIDShift3 As Integer = -1,
                                                Optional ByVal intIDShift4 As Integer = -1) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim sSQL As String = "@SELECT# ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID " &
                                     "WHERE (Shifts.ID = " & intIDShift & ") " &
                                     IIf(intIDShift2 > 0, " OR (Shifts.ID = " & intIDShift2 & ") ", "") &
                                     IIf(intIDShift3 > 0, " OR (Shifts.ID = " & intIDShift3 & ") ", "") &
                                     IIf(intIDShift4 > 0, " OR (Shifts.ID = " & intIDShift4 & ") ", "")
                Dim tb As DataTable = CreateDataTable(sSQL, "")
                If tb.Rows.Count > 0 Then
                    For Each rw As DataRow In tb.Rows
                        If roTypes.Any2String(rw("BusinessGroup")) <> "" Then
                            If Not HasPermissionOverBusinessGroup(oState, oState.IDPassport, roTypes.Any2String(rw("BusinessGroup"))) Then
                                bolRet = False
                                oState.Result = ShiftResultEnum.ShiftWithoutPermission
                                Exit For
                            End If
                        End If
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::ShiftIsAllowed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ShiftIsAllowed")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetBusinessGroupList(ByVal oState As roShiftState, ByVal idPassport As Integer) As String
            Dim strRet As String = String.Empty

            Try
                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ID IN(@SELECT# isnull(IDGroupFeature,-1) from sysroPassports WHERE id = " & oState.IDPassport & " ) "
                Dim oRet As System.Data.DataTable = CreateDataTable(strQuery)
                If oRet IsNot Nothing Then
                    If oRet.Rows.Count > 0 Then
                        Dim arrAux() As String = roTypes.Any2String(oRet.Rows(0)("BusinessGroupList")).Split(";")
                        For Each item As String In arrAux
                            If item.Trim() <> String.Empty Then
                                strRet &= "'" & item.Trim().Replace("'", "''") & "',"
                            End If
                        Next
                        If strRet.Length > 0 Then strRet = strRet.Substring(0, strRet.Length() - 1)
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetBusinessGroupList")
            Finally

            End Try

            Return strRet

        End Function

        Public Function Shifts(ByVal intIDGroup As Integer, Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal _IDAssignment As Integer = -1, Optional ByVal includeStarer As Boolean = False, Optional ByVal addIsRigidInfo As Boolean = False, Optional ByVal addNotifiyAtInfo As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String = "@SELECT# CAST(id AS nvarchar(200)) AS id, Name, Color, ShortName, ExpectedWorkingHours, isnull(AllowFloatingData,0) as AllowFloatingData , ShiftType, IsFloating, StartFloating, (@SELECT# Count(*) from ShiftAssignments WHERE IDShift = Shifts.ID) as Assignments, IdConceptBalance "
                If IncludeObsoletes Then strSQL &= ", IsObsolete "
                If addIsRigidInfo Then strSQL &= ", case when (@select# count(*) from sysroShiftsLayers where IDShift = Shifts.ID and IDType in (1100,1050)) > 0 then 1 else 0 end as IsRigid "
                If addNotifiyAtInfo Then strSQL &= ",case when EnableNotifyExit = 1 then NotifyEmployeeExitAt else null end as NotifyEmployeeExitAt "
                strSQL &= ", IDGroup "

                strSQL &= " FROM Shifts with (nolock) WHERE ID>0 AND IsTemplate = 0 "

                If Not IncludeObsoletes Then
                    strSQL &= " AND IsObsolete = 0 "
                End If

                If intIDGroup <> -1 Then
                    strSQL &= " AND IdGroup = " & intIDGroup.ToString & " "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') "

                If _IDAssignment > 0 Then
                    strSQL &= " AND ID IN (@SELECT# ShiftAssignments.IDShift FROM ShiftAssignments WHERE ShiftAssignments.IDAssignment = " & _IDAssignment.ToString & ") "
                End If


                ' Para Starter (horarios flotantes personalizados
                If includeStarer Then

                    strSQL &= " UNION "
                    strSQL &= " @SELECT# ShiftName1 As id, " &
                                      " ShiftName1 As NAME, " &
                                      " ShiftColor1 As color, " &
                                      " ShiftName1 As shortname, " &
                                      " ExpectedWorkingHours, " &
                                      " 0 As allowfloatingdata , " &
                                      " 1 ShiftType, " &
                                      " 0 isfloating, " &
                                      " NULL StartFloating, " &
                                      " 0 AS IdConceptBalance, " &
                                      " 0 As assignments "

                    If IncludeObsoletes Then strSQL &= ", 0 AS IsObsolete "
                    If addIsRigidInfo Then strSQL &= ", 1 as IsRigid "
                    If addNotifiyAtInfo Then strSQL &= ", NotifyEmployeeExitAt "
                    strSQL &= " FROM DailySchedule with (nolock) " &
                                " WHERE ExpectedWorkingHours IS NOT NULL AND ShiftName1 IS NOT NULL " &
                                " GROUP BY ShiftName1, ShiftColor1, ExpectedWorkingHours "

                End If

                strSQL &= " ORDER BY Name"

                tb = CreateDataTable(strSQL, "Shifts")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::Shifts")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::Shifts")
            End Try

            Return tb

        End Function

        Public Function ShiftsPlanification(ByVal intIDGroup As Integer, Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal _IDAssignment As Integer = -1, Optional ByVal isPortal As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                '->Dim strCateg As String = GetCategoriesEnabledList()
                Dim strBusinessGroups As String = GetBusinessGroupList(oState, oState.IDPassport)

                Dim strSQL As String = "@SELECT# Shifts.ID, isnull(ShiftGroups.Name,'') as GroupName, Shifts.Name as Name, Shifts.Color, Shifts.ShortName, Shifts.ShiftType, Shifts.IsFloating, Shifts.StartFloating," &
                                       "(@SELECT# Count(*) from ShiftAssignments WHERE ShiftAssignments.IDShift = Shifts.ID) AS Assignments, Shifts.StartLimit, Shifts.ExpectedWorkingHours, Shifts.AreWorkingDays, Shifts.AllowComplementary, Shifts.AllowFloatingData, Shifts.AdvancedParameters, Shifts.EndLimit "

                If isPortal Then
                    strSQL = "@SELECT# Shifts.ID, Shifts.Name as Name, Shifts.ShiftType "
                End If

                If IncludeObsoletes Then strSQL &= ", Shifts.IsObsolete "

                strSQL &= " FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID WHERE (Shifts.ID > 0) AND (Shifts.IsTemplate = 0) "

                If strBusinessGroups <> String.Empty Then
                    strSQL &= " AND ((Shifts.IDGroup = 0) OR (ISNULL(ShiftGroups.BusinessGroup, '') IN (" & strBusinessGroups & "))) "
                End If

                If IncludeObsoletes = False Then
                    strSQL &= " AND (Shifts.IsObsolete = 0) "
                End If

                If intIDGroup <> -1 Then
                    strSQL &= " AND (Shifts.IdGroup = " & intIDGroup.ToString & ") "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') "

                If _IDAssignment > 0 Then
                    strSQL &= " AND (Shifts.ID IN (@SELECT# ShiftAssignments.IDShift FROM ShiftAssignments WHERE ShiftAssignments.IDAssignment = " & _IDAssignment.ToString & "))"
                End If

                strSQL &= " ORDER BY isnull(ShiftGroups.Name,'') + ' ' + Shifts.Name"

                tb = CreateDataTable(strSQL, "Shifts")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift:: ShiftsPlanification")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ShiftsPlanification")
            End Try

            Return tb

        End Function

        Public Function Schemas(Optional ByVal IncludeObsoletes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try
                Dim strSQL As String = "@SELECT# ID, Name, Color, ShortName "
                If IncludeObsoletes Then strSQL &= ", IsObsolete "
                strSQL &= " FROM Shifts " &
                        "WHERE ID>0 AND IsTemplate = 1 "

                If IncludeObsoletes = False Then
                    strSQL &= " AND IsObsolete = 0 "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') "

                strSQL &= "ORDER BY Name"

                tb = CreateDataTable(strSQL, "Schemas")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::Schemas")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::Schemas")
            End Try

            Return tb

        End Function

        Public Function SchemasPlanification(Optional ByVal IncludeObsoletes As Boolean = False) As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                '->Dim strCateg As String = GetCategoriesEnabledList()
                Dim strBusinessGroups As String = GetBusinessGroupList(oState, oState.IDPassport)

                Dim strSQL As String = "@SELECT# Shifts.ID, Shifts.Name, Shifts.Color, Shifts.ShortName "

                If IncludeObsoletes Then strSQL &= ", Shifts.IsObsolete "

                strSQL &= " FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID WHERE (Shifts.ID > 0) AND (Shifts.IsTemplate = 1) "

                If strBusinessGroups <> String.Empty Then
                    strSQL &= " AND ((Shifts.IDGroup = 0) OR (ISNULL(ShiftGroups.BusinessGroup, '') IN (" & strBusinessGroups & "))) "
                End If

                If IncludeObsoletes = False Then
                    strSQL &= " AND (Shifts.IsObsolete = 0) "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') "

                strSQL &= " ORDER BY Shifts.Name "

                tb = CreateDataTable(strSQL, "Schemas")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::SchemasPlanification")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::SchemasPlanification")
            End Try

            Return tb

        End Function

        Public Function ShiftGroups() As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String = "@SELECT# ID, Name FROM ShiftGroups ORDER BY Name"
                tb = CreateDataTable(strSQL, "ShiftsGroups")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::ShiftGroups")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ShiftGroups")
            End Try

            Return tb

        End Function

        Public Function ShiftGroupsPlanification() As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                '->Dim strCateg As String = GetCategoriesEnabledList()
                Dim strBusinessGroups As String = GetBusinessGroupList(oState, oState.IDPassport)

                Dim strSQL As String

                If strBusinessGroups = String.Empty Then
                    strSQL = "@SELECT# ID, Name FROM ShiftGroups ORDER BY Name"
                Else
                    strSQL = "@SELECT# ID, Name FROM ShiftGroups WHERE (ISNULL(ShiftGroups.BusinessGroup, '') IN (" & strBusinessGroups & ")) OR (ID=0) ORDER BY Name"
                End If

                tb = CreateDataTable(strSQL, "ShiftsGroups")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::ShiftGroupsPlanification")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ShiftGroupsPlanification")
            End Try

            Return tb

        End Function

        Public Sub AddLayer(ByVal NewLayer As roShiftLayer)
            Try
                If arrayLayers.Count > 0 Then
                    For Each oArr As roShiftLayer In arrayLayers
                        If oArr.ID = NewLayer.ID Then
                            Me.oState.Result = ShiftResultEnum.ShiftLayer_LayerIDExistent
                            Exit Sub
                        End If
                    Next
                End If

                arrayLayers.Add(NewLayer)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::AddLayer")
            End Try
        End Sub

        Public Sub ClearLayers()
            arrayLayers.Clear()
        End Sub

#Region "Save"

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal bolCheckVacationsEmpty As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = ShiftResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim NewID As Integer
                Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)
                Dim oSupport As roSupport
                If oPassport IsNot Nothing Then
                    oSupport = New roSupport(oPassport.Language.Key)
                Else
                    oSupport = New roSupport()
                End If
                Dim oLanguage As New roLanguage()
                oLanguage.SetLanguageReference("LiveShifts", oSupport.ActiveLanguage)

                If Me.Validate(bolCheckVacationsEmpty) Then

                    Dim oShiftOld As DataRow = Nothing
                    Dim oShiftNew As DataRow = Nothing
                    Dim strQueryRow As String

                    Dim intIDShiftOld As Integer = Me.intID

                    strQueryRow = "@SELECT# * " &
                                  "FROM Shifts WHERE [ID] = " & Me.intID
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "Shifts")
                    If tbAuditOld.Rows.Count = 1 Then oShiftOld = tbAuditOld.Rows(0)

                    ' Si es un horario de vacaciones eliminamos la información no necesaria
                    If Me.eShiftType = ShiftType.Vacations Then
                        Me.Layers.Clear()
                        Me.sngExpectedWorkingHours = 0
                        Me.oTimeZones.Clear()
                    End If

                    ' Si es un horario nuevo genero el ID
                    ' y genera reglas de justificación automáticas
                    If intID = -1 Then
                        bolNewShift = True
                        NewID = GetNextShiftID()
                        If NewID = -1 Then
                            Me.oState.Result = ShiftResultEnum.ErrorGeneratingNewID
                        End If
                        Me.ID = NewID
                    Else
                        bolNewShift = False
                        If datStartDate.HasValue AndAlso datStartDate <> Date.MinValue Then
                            ' Si nos han indicado una fecha de modificación generamos una copia del horario antiguo
                            NewID = GetNextShiftID()
                            If NewID = -1 Then
                                Me.oState.Result = ShiftResultEnum.ErrorGeneratingNewID
                            Else
                                If Not MarkShiftAsObsolete(Me.intID) Then
                                    Me.oState.Result = ShiftResultEnum.ErrorOnMarkShiftAsObsolete
                                Else
                                    Me.oState.Language.ClearUserTokens()
                                    Me.oState.Language.AddUserToken(strName)
                                    Dim _Name As String = Me.oState.Language.Translate("Shifts.ShiftSave.Obsolete", "")
                                    Me.oState.Language.ClearUserTokens()
                                    If Not ReplaceShiftNameToObsolete(intID, _Name) Then
                                        Me.oState.Result = ShiftResultEnum.ErrorOnChangeObsoleteName
                                    Else
                                        If Not ReplaceExportName(intID, Me.strExportName) Then
                                            Me.oState.Result = ShiftResultEnum.ErrorOnChangeExportName
                                        Else
                                            Me.ID = NewID
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If

                    CalculateLimits()

                    If oState.Result <> ShiftResultEnum.NoError Or oState.ErrorNumber <> 0 Then
                        bolRet = Me.SaveData()
                        If Not bolRet Then oState.Result = ShiftResultEnum.ErrorSavingShiftData
                    End If

                    'Revisamos reglas de planificación para sustituir, si aplica, los id's de horarios usados.
                    If bolRet AndAlso intIDShiftOld <> Me.intID Then
                        Try
                            If bolRet Then
                                Dim tTemp As New DataTable
                                Dim strSQL As String = ""
                                Dim sDefinition As String = String.Empty
                                Dim sOldCurrentShiftsDef As String = String.Empty
                                Dim sNewCurrentShiftsDef As String = String.Empty
                                Dim sOldPreviousShiftsDef As String = String.Empty
                                Dim sNewPreviousShiftsDef As String = String.Empty
                                Dim sOldCurrentShiftsIds As String = String.Empty
                                Dim sNewCurrentShiftsIds As String = String.Empty
                                Dim sOldPreviousShiftsIds As String = String.Empty
                                Dim sNewPreviousShiftsIds As String = String.Empty
                                Dim tempList As New List(Of String)
                                strSQL = "@SELECT# id, definition from ScheduleRules where Definition like '%""CurrentDayShifts""%'"
                                tTemp = CreateDataTable(strSQL, "Rules")
                                For Each oRow As DataRow In tTemp.Rows
                                    sDefinition = oRow("Definition")
                                    If sDefinition.IndexOf("""CurrentDayShifts""") > -1 Then
                                        sOldCurrentShiftsDef = sDefinition.Substring(sDefinition.IndexOf("""CurrentDayShifts"""), sDefinition.Substring(sDefinition.IndexOf("""CurrentDayShifts""")).IndexOf("]") + 1)
                                        sNewCurrentShiftsDef = sOldCurrentShiftsDef
                                        sOldCurrentShiftsIds = sOldCurrentShiftsDef.Substring(sOldCurrentShiftsDef.IndexOf("[") + 1, sOldCurrentShiftsDef.Length - 2 - sOldCurrentShiftsDef.IndexOf("["))
                                        If sOldCurrentShiftsIds.Split(",").Contains(intIDShiftOld) Then
                                            tempList = sOldCurrentShiftsIds.Split(",").ToList()
                                            tempList.Remove(intIDShiftOld)
                                            tempList.Add(intID)
                                            sNewCurrentShiftsIds = String.Join(",", tempList.ToArray)
                                            sNewCurrentShiftsDef = sOldCurrentShiftsDef.Replace(sOldCurrentShiftsIds, sNewCurrentShiftsIds)
                                        End If
                                        If sNewCurrentShiftsDef <> sOldCurrentShiftsDef Then
                                            strSQL = "@UPDATE# ScheduleRules set definition = REPLACE(definition,'" & sOldCurrentShiftsDef & "','" & sNewCurrentShiftsDef & "') where ID = " & oRow("ID")
                                            bolRet = ExecuteSql(strSQL)
                                        End If
                                    End If

                                    tempList = New List(Of String)
                                    If sDefinition.IndexOf("""PreviousDayShifts""") > -1 Then
                                        sOldPreviousShiftsDef = sDefinition.Substring(sDefinition.IndexOf("""PreviousDayShifts"""), sDefinition.Substring(sDefinition.IndexOf("""PreviousDayShifts""")).IndexOf("]") + 1)
                                        sNewPreviousShiftsDef = sOldPreviousShiftsDef
                                        sOldPreviousShiftsIds = sOldPreviousShiftsDef.Substring(sOldPreviousShiftsDef.IndexOf("[") + 1, sOldPreviousShiftsDef.Length - 2 - sOldPreviousShiftsDef.IndexOf("["))
                                        If sOldPreviousShiftsIds.Split(",").Contains(intIDShiftOld) Then
                                            tempList = sOldPreviousShiftsIds.Split(",").ToList()
                                            tempList.Remove(intIDShiftOld)
                                            tempList.Add(intID)
                                            sNewPreviousShiftsIds = String.Join(",", tempList.ToArray)
                                            sNewPreviousShiftsDef = sOldPreviousShiftsDef.Replace(sOldPreviousShiftsIds, sNewPreviousShiftsIds)
                                        End If
                                        If bolRet AndAlso sNewPreviousShiftsDef <> sOldPreviousShiftsDef Then
                                            strSQL = "@UPDATE# ScheduleRules set definition = REPLACE(definition,'" & sOldPreviousShiftsDef & "','" & sNewPreviousShiftsDef & "') where ID = " & oRow("ID")
                                            bolRet = ExecuteSql(strSQL)
                                        End If
                                    End If
                                Next
                            End If
                        Catch ex As Exception
                            Me.oState.UpdateStateInfo(ex, "roShift::ReAssignShift: Error tring to update schedule rules definition on changing shift id " & intIDShiftOld.ToString & " to " & intID.ToString)
                        End Try
                    End If

                    If bolRet AndAlso datStartDate.HasValue Then
                        bolRet = Me.ReAssignShift(intIDShiftOld, Me.intID, datStartDate.Value)
                    End If

                    If bolRet AndAlso bAudit Then
                        ' Auditamos modificación horario
                        strQueryRow = "@SELECT# * " &
                                      "FROM Shifts WHERE [ID] = " & Me.intID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "Shifts")
                        If tbAuditNew.Rows.Count = 1 Then oShiftNew = tbAuditNew.Rows(0)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditFieldsValues(tbParameters, oShiftNew, oShiftOld)
                        Dim oAuditAction As Audit.Action = IIf(oShiftOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oShiftNew("Name")
                        ElseIf oShiftOld("Name") <> oShiftNew("Name") Then
                            strObjectName = oShiftOld("Name") & " -> " & oShiftNew("Name")
                        Else
                            strObjectName = oShiftNew("Name")
                        End If
                        If datStartDate.HasValue Then oState.AddAuditParameter(tbParameters, "{StartDate}", datStartDate.Value, "", 1)
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tShift, strObjectName, tbParameters, -1)

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShift::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.InsertOrUpdate.ToString)

                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.SHIFTS, oParamsAux)

                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::Save::Could not send cache update")
            End Try

            Return bolRet

        End Function

        Private Function SaveData() As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim tb As New DataTable("Shifts")
                Dim strSQL As String = "@SELECT# * FROM Shifts WHERE [ID] = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("ID") = Me.intID
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("Name") = Me.strName
                oRow("Description") = Me.strDescription
                oRow("Color") = Me.intColor
                oRow("ExpectedWorkingHours") = Me.sngExpectedWorkingHours
                oRow("IsObsolete") = Me.bolIsObsolete
                oRow("IsTemplate") = Me.bolIsTemplate
                oRow("StartLimit") = Me.datStartLimit ''New DateTime(1899, 12, 30, Me.datStartLimit.Hour, Me.datStartLimit.Minute, Me.datStartLimit.Second)
                oRow("EndLimit") = Me.datEndLimit ''New DateTime(1899, 12, 30, Me.datEndLimit.Hour, Me.datEndLimit.Minute, Me.datEndLimit.Second)
                oRow("ManualLimit") = Me.bolManualLimit
                oRow("ShortName") = Me.ShortName
                oRow("TypeShift") = Me.TypeShift
                oRow("IDGroup") = Me.intIDGroup
                oRow("WebVisible") = Me.bolWebVisible
                oRow("WebLaboral") = Me.bolWebLaboral
                oRow("IDConceptBalance") = Me.intIDConceptBalance
                oRow("IDConceptRequestNextYear") = Me.intIDConceptRequestNextYear
                oRow("IDCauseHolidays") = Me.intIDCauseHolidays

                oRow("IDCenter") = Me.intIDCenter
                oRow("ApplyCenterOnAbsence") = Me.bolApplyCenterOnAbsence
                oRow("AllowComplementary") = Me.bolAllowComplementary
                oRow("BreakHours") = sngBreakHours
                oRow("AllowFloatingData") = bolAllowFloatingData
                oRow("Export") = strExportName
                oRow("TypeHolidayValue") = intTypeHolidayValue
                oRow("HolidayValue") = dblHolidayValue

                oRow("AreWorkingDays") = Me.bolAreWorkingDays
                oRow("AdvancedParameters") = Me.strAdvancedParameters

                oRow("EnableCompleteExit") = Me.bolEnableCompleteExit
                oRow("CompleteExitAt") = Me.intCompleteExitAt
                oRow("EnableNotifyExit") = Me.bolEnableNotifyExit
                oRow("NotifyEmployeeExitAt") = Me.intNotifyEmployeeExitAt

                oRow("WhoToNotifyBefore") = Me.intWhoToNotifyBefore
                oRow("WhoToNotifyAfter") = Me.intWhoToNotifyAfter
                oRow("NotifyEmployeeBeforeAt") = Me.intNotifyEmployeeBeforeAt
                oRow("NotifyEmployeeAfterAt") = Me.intNotifyEmployeeAfterAt
                oRow("EnableNotifyBefore") = Me.bolEnableNotifyBeforeStart
                oRow("EnableNotifyAfter") = Me.bolEnableNotifyAfterStart

                Select Case Me.eShiftType
                    Case ShiftType.Normal
                        oRow("ShiftType") = 1
                        oRow("IsFloating") = False
                    Case ShiftType.Vacations
                        oRow("ShiftType") = 2
                        oRow("IsFloating") = False
                    Case ShiftType.NormalFloating
                        oRow("ShiftType") = 1
                        oRow("IsFloating") = True
                End Select
                If Me.eShiftType = ShiftType.NormalFloating And Me.xStartFloating.HasValue Then
                    oRow("StartFloating") = Me.xStartFloating.Value
                Else
                    oRow("StartFloating") = DBNull.Value
                End If

                oRow("VisibilityPermissions") = Me.VisibilityPermissions
                If Me.VisibilityCriteria IsNot Nothing Then
                    If VisibilityCriteria.Count > 1 Then
                        VisibilityCriteria.RemoveRange(0, VisibilityCriteria.Count - 1)
                    End If
                    oRow("VisibilityCriteria") = Replace(VTUserFields.UserFields.roUserFieldCondition.GetXml(VisibilityCriteria), "'", "''")
                Else
                    If Me.VisibilityCollectives IsNot Nothing Then
                        oRow("VisibilityCriteria") = Me.VisibilityCollectives
                    Else
                        oRow("VisibilityCriteria") = DBNull.Value
                    End If
                End If


                oRow("DailyFactor") = dblDailyFactor

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                ' Actualizamos las reglas
                bolRet = roShiftRule.DeleteShiftRules(Me.ID, ShiftRuleType.Simple, Me.State, False)
                If bolRet Then
                    bolRet = roShiftRule.DeleteShiftRules(Me.ID, ShiftRuleType.Daily, Me.State, False)
                End If

                If bolRet Then
                    If Me.oSimpleRules IsNot Nothing AndAlso Me.oSimpleRules.Count > 0 Then
                        For Each oRule As roShiftRule In Me.oSimpleRules
                            oRule.ID = -1
                        Next
                        If bolRet Then
                            bolRet = roShiftRule.SaveShiftRules(Me.oSimpleRules, ShiftRuleType.Simple, Me.oState, False)
                        End If
                    Else
                        'bolRet = roShiftRule.DeleteShiftRules(Me.ID, ShiftRuleType.Simple, Me.State, Me.oTransaction, False)
                    End If

                    If Me.oDailyRules IsNot Nothing AndAlso Me.oDailyRules.Count > 0 Then
                        For Each oRule As roShiftDailyRule In Me.oDailyRules
                            oRule.ID = -1
                        Next
                        If bolRet Then
                            bolRet = roShiftRule.SaveShiftDailyRules(Me.oDailyRules, Me.oState, False)
                        End If
                    Else
                        'bolRet = roShiftRule.DeleteShiftRules(Me.ID, ShiftRuleType.Daily, Me.State, Me.oTransaction, False)
                    End If
                End If

                If bolRet Then
                    ' Actualizamos las zonas horarias
                    If Me.oTimeZones IsNot Nothing AndAlso Me.oTimeZones.Count > 0 Then
                        ' Verificamos el campo IsBlocked en función de si el horario es flotante o no.
                        For Each oTimeZone As roShiftTimeZone In Me.oTimeZones
                            oTimeZone.IsBlocked = IIf(Me.eShiftType = ShiftType.NormalFloating, oTimeZone.IsBlocked, True)
                        Next
                        'Eliminamos las zonas y las volvemos a cargar
                        bolRet = roShiftTimeZone.DeleteShiftTimeZones(Me.intID, Me.oState)
                        If bolRet Then
                            bolRet = roShiftTimeZone.SaveShiftTimeZones(Me.oTimeZones, Me.oState, False)
                        End If
                    Else
                        bolRet = roShiftTimeZone.DeleteShiftTimeZones(Me.intID, Me.oState)
                    End If
                End If

                If bolRet Then
                    bolRet = roShiftAssignment.SaveShiftAssignments(Me.intID, Me.oAssignments, Me.oState, True)
                End If

                If bolRet Then
                    'Borramos todas las capas existentes del horario actual
                    bolRet = DeleteShiftsLayers(False)

                    If bolRet Then
                        ' Guardar los datos de las layers que componen el horario
                        If arrayLayers.Count > 0 Then
                            bolRet = SaveLayers(False)
                        End If
                    End If
                End If

                If bolRet AndAlso Me.PunchesPattern IsNot Nothing AndAlso Me.PunchesPattern.Punches IsNot Nothing Then
                    'Borramos los fichajes del patrón que puedan existir
                    bolRet = DeleteShiftPunchesPatern(Me.intID, Me.oState)
                    'Permistimos el patrón que llega desde pantalla
                    If bolRet AndAlso Me.PunchesPattern.Punches.Count > 0 Then
                        bolRet = SavePunchesPattern(Me.PunchesPattern, Me.intID, Me.oState)
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::SaveData")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal bolCheckVacationsEmpty As Boolean) As Boolean

            Dim bolRet As Boolean = True

            Try
                'Existen muchos clientes que tienen horarios con nombre repetido y al modificar obliga a cambiar algo que en ese momento no es posible
                ' Compruebo que el nombre no este en uso
                If ExistExportName() OrElse Me.ExportName = String.Empty Then
                    Me.oState.Result = ShiftResultEnum.ExportNameAlreadyExist
                    bolRet = False
                End If
                Dim isUsed = roShift.IsShiftPlanned(Me.intID, Me.oState, Robotics.Base.DTOs.ActionShiftType.AllShift)
                Dim eShiftAllowFloating As Boolean = False
                If Me.intID <> -1 AndAlso bolRet Then
                    ' Verifica si se ha modificado el parámetro de tipo de horario.
                    ' Si el horario ya ha sido planificado no se podrá modificar el parámetro.
                    Dim strSQL As String = "@SELECT# * " &
                                           "FROM Shifts WHERE [ID] = " & Me.intID
                    Dim tbShiftOld As DataTable = CreateDataTable(strSQL, "Shifts")
                    If tbShiftOld IsNot Nothing AndAlso tbShiftOld.Rows.Count = 1 Then

                        Dim eShiftTypeOld As ShiftType = ShiftType.Normal
                        Dim eShiftAllowComplementaryOld As Boolean = Any2Boolean(tbShiftOld.Rows(0).Item("AllowComplementary"))
                        eShiftAllowFloating = Any2Boolean(tbShiftOld.Rows(0).Item("AllowFloatingData"))
                        Dim eBreakHours As Single = Any2Time(tbShiftOld.Rows(0).Item("BreakHours")).NumericValue()
                        Select Case Any2Integer(tbShiftOld.Rows(0).Item("ShiftType"))
                            Case 0, 1

                                Dim oLicense As New roServerLicense
                                Dim bMultipleShifts As Boolean = oLicense.FeatureIsInstalled("Feature\MultipleShifts")

                                '->If Me.bolMultipleShifts And Any2Boolean(tbShiftOld.Rows(0).Item("IsFloating")) Then
                                If bMultipleShifts And Any2Boolean(tbShiftOld.Rows(0).Item("IsFloating")) Then
                                    eShiftTypeOld = ShiftType.NormalFloating
                                Else
                                    eShiftTypeOld = ShiftType.Normal
                                End If
                            Case 2
                                eShiftTypeOld = ShiftType.Vacations
                        End Select

                        'Validar si se cambio el tipo de horario.
                        'Validar si se cambio el check de admitir complementarias y validar si existe planificación.
                        If ((eShiftTypeOld <> Me.eShiftType) OrElse
                            (eShiftAllowComplementaryOld <> bolAllowComplementary) OrElse
                            (eShiftAllowFloating <> bolAllowFloatingData)) Then
                            If isUsed Then
                                Me.oState.Result = ShiftResultEnum.TypeLockedShiftPlanned
                                bolRet = False
                            End If
                        End If
                        If ((Not eBreakHours.Equals(sngBreakHours))) Then ' AndAlso bolAllowComplementary AndAlso bolAllowFloatingData) Then
                            If isUsed Then
                                Me.oState.Result = ShiftResultEnum.TypeLockedShiftPlannedBreakHours
                                bolRet = False
                            End If
                        End If

                    End If
                End If

                If bolRet And bolCheckVacationsEmpty And Me.eShiftType = ShiftType.Vacations Then

                    ' Si es un horario de vacaciones o permiso, verificamos que no tenga franjas, horas teóricas a zero, ...
                    If (Me.Layers IsNot Nothing AndAlso Me.Layers.Count > 0) OrElse
                           Me.sngExpectedWorkingHours > 0 OrElse
                           (Me.oTimeZones IsNot Nothing AndAlso Me.oTimeZones.Count > 0) Then

                        Me.oState.Result = ShiftResultEnum.VacationsShiftNotEmpty
                        bolRet = False

                    End If

                End If

                'Validamos que si es un horario de vacaciones y tiene saldos asignados, estos són del mismo tipo
                If bolRet And Me.eShiftType = ShiftType.Vacations Then

                    Dim actualConcept As roConcept = Nothing
                    Dim nextConcept As roConcept = Nothing

                    If Me.IDConceptBalance > 0 AndAlso Me.IDConceptRequestNextYear > 0 Then
                        actualConcept = New roConcept(Me.IDConceptBalance, New roConceptState(Me.State.IDPassport), False)
                        nextConcept = New roConcept(Me.IDConceptRequestNextYear, New roConceptState(Me.State.IDPassport), False)

                        If actualConcept IsNot Nothing AndAlso nextConcept IsNot Nothing Then
                            If actualConcept.DefaultQuery <> nextConcept.DefaultQuery Then
                                Me.oState.Result = ShiftResultEnum.HolidayConceptsQueryNotEqual
                                bolRet = False
                            End If
                        End If

                    End If
                End If

                If bolRet And Me.eShiftType = ShiftType.NormalFloating And Not Me.xStartFloating.HasValue Then
                    Me.oState.Result = ShiftResultEnum.StartFloatingRequired
                    bolRet = False
                End If

                If bolRet And bolAllowFloatingData Then
                    ' SI el horario tiene datos flotantes , tiene que tener horas teoricas obligadas
                    If sngExpectedWorkingHours <= 0 Then
                        Me.oState.Result = ShiftResultEnum.InvalidExpectedWorkingHours
                        bolRet = False
                    End If
                End If

                ' Validamos las layers flotantes
                If bolRet Then
                    bolRet = Me.ValidateFloatingDataLayers(Me.intID, eShiftAllowFloating)
                End If

                ' Validamos las layers
                If bolRet Then
                    Dim intErrID As Integer
                    bolRet = Me.ValidateLayers(intErrID, isUsed)
                End If

                ' Validamos las zonas horarias (datos)
                If bolRet Then
                    bolRet = roShiftTimeZone.ValidateShiftTimeZones(Me.oTimeZones, Me.oState)
                End If

                ' Validamos los puestos asignados
                If bolRet Then
                    bolRet = roShiftAssignment.ValidateShiftAssignments(Me.intID, Me.oAssignments, Me.oState)
                End If

                ' Valida que no exista la sobreposición de zonas
                If bolRet Then
                    If Not ValidateTimeZones() Then
                        Me.oState.Result = ShiftResultEnum.TimeZone_OverlappedZones
                        bolRet = False
                    End If
                End If

                ' Validamos patrón de fichajes si lo hay
                If bolRet AndAlso Me.PunchesPattern IsNot Nothing AndAlso Me.PunchesPattern.Punches IsNot Nothing AndAlso Me.PunchesPattern.Punches.Count > 0 Then
                    Dim oDailyRecordPunchList As New List(Of roDailyRecordPunch)
                    Dim strCRC As String = String.Empty
                    oDailyRecordPunchList = Me.PunchesPattern.Punches.ToList.ConvertAll(AddressOf XShiftPatternPunchToDailyRecordPunch)
                    Select Case Punch.roDailyRecordPunchHelper.CheckDailyRecordPunches(oDailyRecordPunchList.ToArray, strCRC)
                        Case DailyRecordPunchesResultEnum.Exception
                            oState.Result = ShiftResultEnum.Exception
                            bolRet = False
                        Case DailyRecordPunchesResultEnum.InvalidSequence
                            oState.Result = ShiftResultEnum.PatternPunchesBadSequence
                            bolRet = False
                        Case DailyRecordPunchesResultEnum.PunchesNumberShouldBeEven
                            oState.Result = ShiftResultEnum.PatternPunchesShouldBeEven
                            bolRet = False
                        Case DailyRecordPunchesResultEnum.PunchesOverlaped
                            oState.Result = ShiftResultEnum.PatternPunchesOverlaped
                            bolRet = False
                        Case DailyRecordPunchesResultEnum.PunchRepeated
                            oState.Result = ShiftResultEnum.PatternHasRepeatedPunch
                            bolRet = False
                    End Select
                End If

                If Me.VisibilityPermissions = 2 Then
                    If Me.VisibilityCriteria.Count = 0 Then
                        Me.oState.Result = ShiftResultEnum.UserFieldEmpty
                        Return False
                    End If
                Else
                    Me.VisibilityCriteria = Nothing
                End If

                If Me.VisibilityPermissions = 3 Then
                    If Me.VisibilityCollectives = String.Empty Then
                        Me.oState.Result = ShiftResultEnum.CollectivesEmpty
                        Return False
                    End If
                Else
                    Me.VisibilityCollectives = Nothing
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        ''' <summary>
        '''  Valida que no exista la sobreposición de zonas
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ValidateTimeZones() As Boolean
            Dim bolRet As Boolean = True
            Dim sTimeZonesList As New Generic.List(Of roShiftTimeZone)
            Try
                For Each sTimeZone As roShiftTimeZone In oTimeZones
                    'Comprueba si alguna de las zonas se solapa con las otras
                    If Any2Time(GetDateTimeInPeriod(sTimeZonesList, sTimeZone.BeginTime, sTimeZone.EndTime)).VBNumericValue = 0 Then
                        'No existe solapamiento, añade a la lista de bloques
                        sTimeZonesList.Add(New roShiftTimeZone(Me.ID, -1, sTimeZone.BeginTime, sTimeZone.EndTime, sTimeZone.IsBlocked, oState))
                    Else
                        bolRet = False
                        Me.oState.Result = ShiftResultEnum.TimeZone_OverlappedZones
                        Exit For
                    End If
                Next

                sTimeZonesList = Nothing
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::ValidateTimeZones")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Cuenta el total de tiempo que se encuentra dentro de los limites del bloque especificado,
        ''' comprobando unicamente los bloques del tipo indicado.
        ''' </summary>
        ''' <param name="TimeZones"></param>
        ''' <param name="PeriodBegin"></param>
        ''' <param name="PeriodFinish"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDateTimeInPeriod(ByRef TimeZones As Generic.List(Of roShiftTimeZone), ByVal PeriodBegin As DateTime, ByVal PeriodFinish As DateTime) As Date
            Dim lRet As Double

            Dim LayerBegin As Double
            Dim LayerFinish As Double

            Dim Begin As Double
            Dim Finish As Double

            Try
                LayerBegin = Any2Time(PeriodBegin).VBNumericValue
                LayerFinish = Any2Time(PeriodFinish).VBNumericValue

                lRet = 0
                For Each oTimeZone As roShiftTimeZone In TimeZones
                    With oTimeZone
                        Begin = Math.Max(LayerBegin, Any2Time(.BeginTime).VBNumericValue)
                        Finish = Math.Min(LayerFinish, Any2Time(.EndTime).VBNumericValue)
                        If Finish > Begin Then
                            ' Añade tiempo al total calculado hasta ahora, hasta el máximo que permita el
                            '  timevalue del bloque interseccionado.
                            Dim timeValue As Date = Date.FromOADate(Any2Time(.EndTime).VBNumericValue - Any2Time(.BeginTime).VBNumericValue)
                            lRet = lRet + Min(Finish - Begin, Any2Time(timeValue).VBNumericValue)
                        End If
                    End With
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::GetDateTimeInPeriod")
            End Try

            Return Date.FromOADate(lRet)

        End Function

        Public Function ValidateLayers(ByRef ErrID As Integer, ByVal isShiftUsed As Boolean) As Boolean
            Try
                'Posem els Layers en un array per fer comprobacions sense ChildLayers
                Dim arrValidates As New ArrayList
                Dim oldLayers = 0
                For Each oLayer As roShiftLayer In arrayLayers
                    arrValidates.Add(oLayer)
                    If oLayer.ChildLayers IsNot Nothing Then
                        For Each oLayerChild As roShiftLayer In oLayer.ChildLayers
                            arrValidates.Add(oLayerChild)
                        Next
                    End If
                    Select Case (oLayer.LayerType)
                        Case roLayerTypes.roLTMandatory, roLayerTypes.roLTBreak
                            oldLayers += 1
                    End Select
                Next
                If (Not intLayersCount.Equals(oldLayers) AndAlso isShiftUsed AndAlso (Me.ShiftType = ShiftType.Normal AndAlso (Me.AllowComplementary OrElse Me.AllowFloatingData))) Then
                    Me.oState.Result = ShiftResultEnum.ShiftLayerCount
                End If

                ErrID = -1

                ' Flexibles
                If Not ValidateFlexibleLayers(arrValidates, ErrID) Then Return False
                ' Rigidas
                If Not ValidateMandatoryLayers(arrValidates, ErrID, isShiftUsed) Then Return False
                ' Descansos
                If Not ValidateBreakLayers(arrValidates, ErrID) Then Return False
                ' Tiempos abonados
                If Not ValidatePaidLayers(arrValidates, ErrID) Then Return False
                ' Filtros
                If Not ValidateFilters(arrValidates, ErrID) Then Return False
                ' Si llegamos aqui, es que es valido
                Return True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateLayers")
            End Try
        End Function

        Private Function ValidateFloatingDataLayers(ByVal ShiftID As Integer, ByVal eShiftAllowFloating As Boolean) As Boolean
            Dim tb As DataTable = Nothing
            Dim oLayer As roShiftLayer = Nothing
            Dim oLayerChild As roShiftLayer = Nothing
            Dim strShiftLayerTypes As String = ""
            Dim arrayLayersOld As New ArrayList
            Dim intCountLayers As Integer = 0
            Dim intCountLayersMandatory As Integer = 0
            Dim finalCountLayers As Integer = 0
            Dim bolRet As Boolean = True
            Try
                Dim strSQL As String = " @SELECT# * from sysroShiftsLayers Where (IDShift = " & ShiftID & " And ParentLayerID IS NULL) OR (IDShift = " & ShiftID & " And ParentLayerID = 0) Order by IDType "
                tb = CreateDataTable(strSQL, "sysroShiftsLayers")

                For Each dRow As DataRow In tb.Rows
                    oLayer = New roShiftLayer(oState)

                    oLayer.ID = dRow("ID")
                    oLayer.IDShift = dRow("IDShift")
                    oLayer.LayerType = dRow("IDType")
                    oLayer.ParentID = IIf(IsDBNull(dRow("ParentLayerID")), -1, dRow("ParentLayerID"))
                    oLayer.ParseLayerData(dRow("Definition"))

                    Dim tbChilds As DataTable = CreateDataTable(" @SELECT# * from sysroShiftsLayers Where IDShift = " & ShiftID & " And ParentLayerID = " & dRow("ID") & " Order by IDType ", "sysroShiftLayersChilds")
                    If tbChilds.Rows.Count > 0 Then
                        oLayer.ChildLayers = New Generic.List(Of roShiftLayer)
                        'Fem consulta per carregar els layers fills
                        For Each dRowChild As DataRow In tbChilds.Rows
                            oLayerChild = New roShiftLayer(oState)
                            oLayerChild.ID = dRowChild("ID")
                            oLayerChild.IDShift = dRowChild("IDShift")
                            oLayerChild.LayerType = dRowChild("IDType")
                            oLayerChild.ParentID = IIf(IsDBNull(dRowChild("ParentLayerID")), -1, dRowChild("ParentLayerID"))
                            oLayerChild.ParseLayerData(dRowChild("Definition"))
                            oLayer.ChildLayers.Add(oLayerChild)
                            strShiftLayerTypes &= IIf(strShiftLayerTypes <> "", ",", "") & [Enum].GetName(GetType(roLayerTypes), oLayerChild.LayerType)
                        Next
                    Else
                        oLayer.ChildLayers = Nothing
                    End If

                    Dim bolExistData As Boolean = False

                    If oLayer.LayerType = roLayerTypes.roLTMandatory And oLayer.Data.Exists("AllowModifyIniHour") Then bolExistData = True
                    If oLayer.LayerType = roLayerTypes.roLTMandatory And oLayer.Data.Exists("AllowModifyDuration") Then bolExistData = True
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then intCountLayersMandatory += 1
                    intCountLayers += 1

                    'If (intCountLayersMandatory > 2) Then Me.oState.Result = ShiftResultEnum.ShiftLayerNumber

                    If bolExistData Then
                        If intCountLayers = 1 Then
                            If oLayer.Data.Exists("AllowModifyIniHour") Then bolAllowModifyIniHour1 = True
                            If oLayer.Data.Exists("AllowModifyDuration") Then bolAllowModifyDuration1 = True
                        Else
                            If oLayer.Data.Exists("AllowModifyIniHour") Then bolAllowModifyIniHour2 = True
                            If oLayer.Data.Exists("AllowModifyDuration") Then bolAllowModifyDuration2 = True
                        End If
                    End If
                    Select Case (oLayer.LayerType)
                        Case roLayerTypes.roLTMandatory, roLayerTypes.roLTBreak
                            finalCountLayers += 1
                    End Select
                Next
                intLayersCount = finalCountLayers
                If Me.oState.Result <> ShiftResultEnum.NoError Then bolRet = False
                Return bolRet
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateFloatingDataLayers")
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Valida periodos flexibles
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ValidateFlexibleLayers(ByVal arrayValidates As ArrayList, ByRef ErrID As Integer) As Boolean
            Try
                '
                ' Valida periodos de flexibles
                '

                Dim bolRet As Boolean = True

                ' Comprueba hora de inicio y fin, tiempo máximo, mínimo
                For Each oLayer As roShiftLayer In arrayValidates
                    If oLayer.LayerType = roLayerTypes.roLTWorking Then
                        isFlexible = True
                        If Not IsDate(oLayer.Data("Begin")) Then
                            Me.oState.Result = ShiftResultEnum.FlexibleLayer_InvalidBegin
                        ElseIf Not IsDate(oLayer.Data("Finish")) Then
                            Me.oState.Result = ShiftResultEnum.FlexibleLayer_InvalidFinish
                        ElseIf DateDiff("n", oLayer.Data("Begin"), oLayer.Data("Finish")) <= 0 Then
                            Me.oState.Result = ShiftResultEnum.FlexibleLayer_InvalidPeriod
                        End If
                    End If

                    If Me.oState.Result <> ShiftResultEnum.NoError Then
                        ErrID = oLayer.ID
                        bolRet = False
                        Exit For
                    End If
                Next

                If Me.oState.Result <> ShiftResultEnum.NoError Then
                    bolRet = False
                    Return bolRet
                End If

                For oA As Integer = 0 To arrayValidates.Count - 1
                    Dim oLayerA As roShiftLayer = arrayValidates(oA)
                    If oLayerA.LayerType = roLayerTypes.roLTWorking Then
                        For oB As Integer = oA + 1 To arrayValidates.Count - 1
                            Dim oLayerB As roShiftLayer = arrayValidates(oB)
                            If oLayerB.LayerType = roLayerTypes.roLTWorking Then
                                If DateDiff("n", oLayerA.Data("Begin"), oLayerB.Data("Finish")) >= 0 And
                                DateDiff("n", oLayerB.Data("Begin"), oLayerA.Data("Finish")) >= 0 Then
                                    'Se solapan!
                                    Me.oState.Result = ShiftResultEnum.FlexibleLayer_Collision
                                    ErrID = oLayerB.ID
                                    Return False
                                End If
                            End If
                        Next
                    End If
                Next

                For oA As Integer = 0 To arrayValidates.Count - 1
                    Dim oLayerA As roShiftLayer = arrayValidates(oA)
                    If oLayerA.LayerType = roLayerTypes.roLTWorking Then
                        For oB As Integer = oA + 1 To arrayValidates.Count - 1
                            Dim oLayerB As roShiftLayer = arrayValidates(oB)
                            If oLayerB.LayerType = roLayerTypes.roLTMandatory Then
                                If DateDiff("n", oLayerB.Data("Begin"), oLayerA.Data("Begin")) > 0 And
                                DateDiff("n", oLayerA.Data("Finish"), oLayerB.Data("Finish")) > 0 Then
                                    'La franja esta contenida dentro de una fraja rígida
                                    Me.oState.Result = ShiftResultEnum.FlexibleLayer_LayerInsideMandatory
                                    ErrID = oLayerB.ID
                                    Return False
                                End If
                            End If
                        Next
                    End If
                Next

                Return (Me.oState.Result = ShiftResultEnum.NoError)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateFlexibleLayers")
                Return False
            End Try
        End Function

        Private Function ValidateMandatoryLayers(ByVal arrayValidates As ArrayList, ByRef ErrID As Integer, ByVal isShiftUsed As Boolean) As Boolean
            Try
                Dim layerCount = 0
                For Each oLayer As roShiftLayer In arrayValidates
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        layerCount += 1
                        If Not IsDate(oLayer.Data("Begin")) Then
                            Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidBegin
                        ElseIf Not IsDate(oLayer.Data("Finish")) Then
                            Me.oState.Result = ShiftResultEnum.Mandatory_InvalidFinish
                        ElseIf DateDiff("n", oLayer.Data("Begin"), oLayer.Data("Finish")) <= 0 Then
                            Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidPeriod
                        ElseIf (Me.AllowComplementary) Then
                            If (isFlexible) Then Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidFlexibleLayer
                        ElseIf (oLayer.Data.Exists("AllowModifyIniHour") AndAlso Any2Boolean(oLayer.Data("AllowModifyIniHour"))) Then
                            If (isFlexible) Then Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidFlexibleLayer
                        ElseIf (oLayer.Data.Exists("AllowModifyDuration") AndAlso Any2Boolean(oLayer.Data("AllowModifyDuration"))) Then
                            If (isFlexible) Then Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidFlexibleLayer
                        End If

                        If Me.oState.Result = ShiftResultEnum.NoError Then
                            ' Comprueba inicios/finales flotantes
                            If oLayer.Data.Exists("FloatingBeginUpto") Then
                                ' Hay inicio flotante, comprueba
                                If DateDiff("n", oLayer.Data("Begin"), oLayer.Data("FloatingBeginUpto")) <= 0 Then
                                    Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidFloatingBegin
                                Else
                                    If Not oLayer.Data.Exists("FloatingFinishMinutes") Then
                                        ' No hay salida flotante
                                        If DateDiff("n", oLayer.Data("FloatingBeginUpto"), oLayer.Data("Finish")) <= 0 Then
                                            Me.oState.Result = ShiftResultEnum.MandatoryLayer_InvalidFloatingBegin
                                        End If
                                    Else
                                        ' Hay salida flotante, comprueba coherencia con salida real
                                        If DateDiff("n", oLayer.Data("FloatingBeginUpto"), oLayer.Data("Finish")) <> oLayer.Data("FloatingFinishMinutes") Then
                                            Me.oState.Result = ShiftResultEnum.MandatoryLayer_FinishDoesNotMatchFloatingFinish
                                        End If
                                    End If
                                End If
                                'Esta comprabación ya no es necesaria por horas complementarias.
                                'Else
                                '    ' No hay inicio flotante, no puede haber final flotante
                                '    If oLayer.Data.Exists("FloatingFinishMinutes") Then
                                '        Me.oState.Result = ShiftResultEnum.MandatoryLayer_FloatingFinish_Without_FloatingBegin
                                '    End If

                            End If
                        End If
                        If layerCount.Equals(1) Then
                            Dim modifyIni = oLayer.Data.Exists("AllowModifyIniHour") = bolAllowModifyIniHour1
                            Dim modifyDuration = oLayer.Data.Exists("AllowModifyDuration") = bolAllowModifyDuration1
                            If (Not modifyIni AndAlso isShiftUsed) Then Me.oState.Result = ShiftResultEnum.TypeLockedShiftFloatingData
                            If (Not modifyDuration AndAlso isShiftUsed) Then Me.oState.Result = ShiftResultEnum.TypeLockedShiftFloatingData
                        Else
                            Dim modifyIni = oLayer.Data.Exists("AllowModifyIniHour") = bolAllowModifyIniHour2
                            Dim modifyDuration = oLayer.Data.Exists("AllowModifyDuration") = bolAllowModifyDuration2
                            If (Not modifyIni AndAlso isShiftUsed) Then Me.oState.Result = ShiftResultEnum.TypeLockedShiftFloatingData
                            If (Not modifyDuration AndAlso isShiftUsed) Then Me.oState.Result = ShiftResultEnum.TypeLockedShiftFloatingData
                        End If
                    End If

                    If Me.oState.Result <> ShiftResultEnum.NoError Then
                        ErrID = oLayer.ID
                        Return False
                    End If
                Next

                For oA As Integer = 0 To arrayValidates.Count - 1
                    Dim oLayerA As roShiftLayer = arrayValidates(oA)
                    If oLayerA.LayerType = roLayerTypes.roLTMandatory Then
                        For oB As Integer = oA + 1 To arrayValidates.Count - 1
                            Dim oLayerB As roShiftLayer = arrayValidates(oB)
                            If oLayerB.LayerType = roLayerTypes.roLTMandatory Then
                                If DateDiff(DateInterval.Minute, oLayerA.Data("Begin"), oLayerB.Data("Finish")) >= 0 And
                                DateDiff(DateInterval.Minute, oLayerB.Data("Begin"), oLayerA.Data("Finish")) >= 0 Then
                                    'Se solapan!
                                    Me.oState.Result = ShiftResultEnum.MandatoryLayer_Collision
                                    ErrID = oLayerB.ID
                                    Return False
                                End If
                            End If
                        Next
                    End If
                Next

                If layerCount > 2 AndAlso (Me.ShiftType = ShiftType.Normal AndAlso (Me.AllowComplementary OrElse Me.AllowFloatingData)) Then
                    Me.oState.Result = ShiftResultEnum.ShiftLayerNumber
                End If

                Return (Me.oState.Result = ShiftResultEnum.NoError)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateMandatoryLayers")
                Return False
            End Try
        End Function

        Private Function ValidateParameterMinBreak(ByRef Data As roCollection) As Boolean
            '
            ' Subfuncion de validar que comprueba que el parametro "MinBreakTime" y "MinBreakAction"
            ' exista y tenga un valor valido.
            '
            Try
                If Data("MinBreakTime") Is Nothing Then Return True

                If Not IsDate(Data("MinBreakTime")) Then Return False
                Select Case Data("MinBreakAction")
                    Case "CreateIncidence"
                    Case "SubstractAttendanceTime"
                    Case Else : Return False
                End Select
                Return True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateParameterMinBreak")
                Return False
            End Try
        End Function

        Private Function ValidateParameterMaxBreak(ByRef Data As roCollection) As Boolean
            '
            ' Subfuncion de validar que comprueba que el parametro "MaxBreakTime"
            ' exista y tenga un valor valido.
            '
            Try
                If Data("MaxBreakTime") Is Nothing Then Return True

                If Not IsDate(Data("MaxBreakTime")) Then Return False
                Return True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateParameterMaxBreak")
                Return False
            End Try
        End Function

        Private Function ValidateBreakLayers(ByVal arrayValidates As ArrayList, ByRef ErrID As Integer) As Boolean
            Try
                For oA As Integer = 0 To arrayValidates.Count - 1
                    Dim oLayer As roShiftLayer = arrayValidates(oA)

                    If oLayer.LayerType = roLayerTypes.roLTBreak Then
                        If Not IsDate(oLayer.Data("Begin")) Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_InvalidBegin
                        ElseIf Not IsDate(oLayer.Data("Finish")) Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_InvalidFinish
                        ElseIf DateDiff("n", oLayer.Data("Begin"), oLayer.Data("Finish")) <= 0 Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_InvalidPeriod
                        ElseIf Not ValidateParameterMinBreak(oLayer.Data) Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_InvalidMinBreak
                        ElseIf Not ValidateParameterMaxBreak(oLayer.Data) Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_InvalidMaxBreak
                        End If

                        If Me.oState.Result <> ShiftResultEnum.NoError Then
                            ErrID = oLayer.ID
                            Exit For
                        End If

                        ' Comprueba que exista una capa de Rigidas que englobe a esta
                        Dim enGlobeMandatory As Boolean = False

                        For Each oLayerA As roShiftLayer In arrayValidates
                            If oLayerA.LayerType = roLayerTypes.roLTMandatory Then
                                ' Comprueba estas Rigidas a ver si engloban al descanso
                                If Day(oLayer.Data("Begin")) = 1 And oLayer.Data.Exists(oLayer.XmlKey(roXmlLayerKeys.RealBegin)) And oLayer.Data.Exists(oLayer.XmlKey(roXmlLayerKeys.RealFinish)) Then
                                    Dim tmpHour As DateTime = roTypes.Any2DateTime(oLayer.Data(oLayer.XmlKey(roXmlLayerKeys.RealBegin)))
                                    Dim checkDate As New DateTime(tmpHour.Year, tmpHour.Month, roTypes.Any2DateTime(oLayerA.Data("Begin")).Day, tmpHour.Hour, tmpHour.Minute, tmpHour.Second)
                                    tmpHour = roTypes.Any2DateTime(oLayer.Data(oLayer.XmlKey(roXmlLayerKeys.RealFinish)))
                                    Dim checkDateFinish As New DateTime(tmpHour.Year, tmpHour.Month, roTypes.Any2DateTime(oLayerA.Data("Begin")).Day, tmpHour.Hour, tmpHour.Minute, tmpHour.Second)

                                    If DateDiff(DateInterval.Minute, oLayerA.Data("Begin"), checkDate) >= 0 And DateDiff(DateInterval.Minute, checkDateFinish, oLayerA.Data("Finish")) >= 0 Then
                                        enGlobeMandatory = True
                                        Exit For
                                    End If
                                Else
                                    If DateDiff(DateInterval.Minute, oLayerA.Data("Begin"), oLayer.Data("Begin")) >= 0 And DateDiff(DateInterval.Minute, oLayer.Data("Finish"), oLayerA.Data("Finish")) >= 0 Then
                                        enGlobeMandatory = True
                                        Exit For
                                    End If
                                End If

                            End If
                        Next

                        If enGlobeMandatory = False Then
                            Me.oState.Result = ShiftResultEnum.BreakLayer_NoParentMandatoryLayer
                            ErrID = oLayer.ID
                            Exit For
                        End If

                        ' Comprueba que no se solape con otro descanso
                        For oB As Integer = oA + 1 To arrayValidates.Count - 1
                            Dim oLayerB As roShiftLayer = arrayValidates(oB)
                            If oLayerB.LayerType = roLayerTypes.roLTBreak Then
                                ' Comprueba si los dos descansos se solapan
                                If DateDiff(DateInterval.Minute, oLayerB.Data("Begin"), oLayer.Data("Finish")) >= 0 And
                                DateDiff(DateInterval.Minute, oLayer.Data("Begin"), oLayerB.Data("Finish")) >= 0 Then
                                    Me.oState.Result = ShiftResultEnum.BreakLayer_Collision
                                    ErrID = oLayerB.ID
                                    Exit For
                                End If
                            End If
                        Next

                        If Me.oState.Result <> ShiftResultEnum.NoError Then Exit For
                    End If
                Next

                Return (Me.oState.Result = ShiftResultEnum.NoError)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateBreakLayers")
            End Try
        End Function

        Private Function ValidateParameterTarget(ByVal Data As Object) As Boolean
            '
            ' Subfuncion de validar que comprueba que el parametro "Target" exista y tenga un
            '  valor valido.
            '
            Try
                Select Case Data
                    Case DTOs.roIncidenceType.roITWorking,
                        DTOs.roIncidenceType.roITOverworking,
                        DTOs.roIncidenceType.roITAbsence,
                        DTOs.roIncidenceType.roITLateArrival,
                        DTOs.roIncidenceType.roITUnexpectedBreak,
                        DTOs.roIncidenceType.roITEarlyLeave,
                        DTOs.roIncidenceType.roITFlexibleOverworking,
                        DTOs.roIncidenceType.roITFlexibleUnderworking,
                        DTOs.roIncidenceType.roITBreak,
                        DTOs.roIncidenceType.roITOvertimeBreak,
                        DTOs.roIncidenceType.roITUndertimeBreak
                        Return True
                    Case Else
                        Return False
                End Select
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateParameterTarget")
                Return False
            End Try
        End Function

        Private Function ValidatePaidLayers(ByVal arrayValidates As ArrayList, ByRef ErrID As Integer) As Boolean
            Try
                '
                ' Valida periodos de abonos de descanso
                '
                For Each oLayer As roShiftLayer In arrayValidates
                    If oLayer.LayerType = roLayerTypes.roLTPaidTime Then
                        ' Comprueba hora de inicio y fin, tiempo máximo, mínimo y acciones
                        If Not IsDate(oLayer.Data("Begin")) Then
                            Me.oState.Result = ShiftResultEnum.PaidLayer_InvalidBegin
                        ElseIf Not IsDate(oLayer.Data("Finish")) Then
                            Me.oState.Result = ShiftResultEnum.PaidLayer_InvalidFinish
                        ElseIf DateDiff(DateInterval.Minute, oLayer.Data("Begin"), oLayer.Data("Finish")) <= 0 Then
                            Me.oState.Result = ShiftResultEnum.PaidLayer_InvalidPeriod
                        ElseIf Not ValidateParameterTarget(oLayer.Data("Target")) Then
                            Me.oState.Result = ShiftResultEnum.PaidLayer_InvalidTarget
                        ElseIf Not IsDate(oLayer.Data("Value")) Then
                            Me.oState.Result = ShiftResultEnum.PaidLayer_InvalidValue
                        Else
                            ' Comprueba que exista una capa de descanso que cubra a esta
                            Dim existBreak As Boolean = False
                            For Each oLayerA As roShiftLayer In arrayValidates
                                If oLayerA.LayerType = roLayerTypes.roLTBreak Then
                                    ' Comprueba este descanso a ver si cubre nuestra capa
                                    If DateDiff(DateInterval.Minute, oLayerA.Data("Begin"), oLayer.Data("Begin")) >= 0 And
                                    DateDiff(DateInterval.Minute, oLayer.Data("Finish"), oLayerA.Data("Finish")) >= 0 Then
                                        existBreak = True
                                        Exit For
                                    End If
                                End If
                            Next

                            If existBreak = False Then
                                Me.oState.Result = ShiftResultEnum.PaidLayer_NoParentBreakLayer
                            End If
                        End If

                        If Me.oState.Result <> ShiftResultEnum.NoError Then
                            ErrID = oLayer.ID
                            Exit For
                        End If
                    End If
                Next

                Return (Me.oState.Result = ShiftResultEnum.NoError)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidatePaidLayers")
                Return False
            End Try
        End Function

        Private Function ValidateParameterFilterAction(ByVal Data As Object) As Boolean
            '
            ' Subfuncion de validar que comprueba que el parametro "Action" exista y tenga un
            '  valor valido.
            '
            Try
                If IsDBNull(Data) Then Data = ""

                Select Case Data
                    Case roShiftLayer.roFilterTreatAsWork, roShiftLayer.roFilterIgnore, roShiftLayer.roFilterGenerateIncidence, roShiftLayer.roFilterGenerateOvertime
                        Return True
                    Case Else
                        Return False
                End Select
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateParameterFilterAction")
                Return False
            End Try
        End Function

        Private Function ValidateFilters(ByVal arrayValidates As ArrayList, ByRef ErrID As Integer) As Boolean
            Try
                '
                ' Subfunción de IsValid que comprueba los filtros.
                '

                For Each oLayer As roShiftLayer In arrayValidates
                    If oLayer.LayerType = roLayerTypes.roLTGroupFilter Or oLayer.LayerType = roLayerTypes.roLTUnitFilter Then
                        If Not ValidateParameterFilterAction(oLayer.Data("Action")) Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidAction
                        ElseIf Not ValidateParameterTarget(oLayer.Data("Target")) Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidTarget
                        ElseIf Not IsDate(oLayer.Data("Value")) Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidValue
                        End If
                    End If

                    If oLayer.LayerType = roLayerTypes.roLTUnitFilter Then
                        If Not IsDate(oLayer.Data("Begin")) Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidBegin
                        ElseIf Not IsDate(oLayer.Data("Finish")) Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidFinish
                        ElseIf DateDiff(DateInterval.Minute, oLayer.Data("Begin"), oLayer.Data("Finish")) <= 0 Then
                            Me.oState.Result = ShiftResultEnum.Filter_InvalidPeriod
                        End If
                    End If

                    If Me.oState.Result <> ShiftResultEnum.NoError Then
                        ErrID = oLayer.ID
                        Exit For
                    End If
                Next

                Return (Me.oState.Result = ShiftResultEnum.NoError)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ValidateFilters")
                Return False
            End Try
        End Function

        Private Function SaveLayers(Optional ByVal bAudit As Boolean = False) As Boolean
            ' Recorro los layers para guardarlos uno a uno

            Dim bolRet As Boolean = False

            Try
                Dim intLayerCounter As Integer = 0

                For Each oLayer As roShiftLayer In arrayLayers

                    'intLayerCounter = intLayerCounter + 1
                    bolRet = oLayer.Save(intLayerCounter, bAudit)

                    'Si hi ha un error, sortim
                    If bolRet = False Then
                        Exit For
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::SaveLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::SaveLayers")
            End Try

            Return bolRet

        End Function

        Private Function DeleteShiftsLayers(Optional ByVal bAudit As Boolean = False) As Boolean
            'Borramos todas las capas existentes del horario actual

            Dim bolRet As Boolean = False

            Try

                Dim tb As New DataTable("ShiftsLayers")
                Dim strSQL As String = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & Me.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim intLayerID As Integer
                Dim strLayerType As String

                bolRet = True

                For Each oRow As DataRow In tb.Rows

                    intLayerID = oRow("ID")
                    strLayerType = System.Enum.GetName(GetType(roLayerTypes), oRow("IDType"))

                    oRow.Delete()

                    ' Auditamos borrado capa horario
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ShiftLayerType}", strLayerType, "", 1)
                        oState.AddAuditParameter(tbParameters, "{ShiftName}", Me.Name, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tShiftLayer, intLayerID & " " & strLayerType & " (" & Me.Name & ")", tbParameters, -1)
                    End If

                Next

                da.Update(tb)

                ''Dim strSql As String = "@DELETE# sysroShiftsLayers WHERE IDShift = " & Me.ID
                ''bolRet = ExecuteSql(strSql, Me.Connection, Me.oTransaction)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::DeleteShiftsLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::DeleteShiftsLayers")
            End Try

            Return bolRet

        End Function

        Private Sub CalculateLimits()

            ' Si se establecen manualmente, no toca nada
            If Me.ManualLimit Then Exit Sub

            If Me.Layers.Count = 0 Then
                'Me.StartLimit = roTypes.Any2Time("00:00").Value
                'Me.EndLimit = roTypes.Any2Time("23:59").Value
                Me.StartLimit = New Date(1899, 12, 30, 0, 0, 0)
                Me.EndLimit = New Date(1899, 12, 30, 23, 59, 0)
            Else

                Dim xAux As DateTime
                Dim xMin As DateTime ' NewDateTime(1899, 12, 30, 23, 59, 0)
                Dim xMax As DateTime ' New DateTime(1899, 12, 30, 0, 0, 0)

                Dim bolIni As Boolean = True
                For Each oLayer As roShiftLayer In Me.Layers
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Or
                        oLayer.LayerType = roLayerTypes.roLTWorking Then

                        xAux = roTypes.Any2DateTime(oLayer.Data.Item("Begin"))
                        If (xAux.Year = 1) Then
                            xAux = New Date(1899, 12, 30, xAux.Hour, xAux.Minute, xAux.Second)
                        End If
                        'xAux = Any2Time(oLayer.Data.Item("Begin")).Value
                        If bolIni Then
                            xMin = xAux
                        ElseIf xAux < xMin Then
                            xMin = xAux
                        End If

                        xAux = roTypes.Any2DateTime(oLayer.Data.Item("Finish"))
                        If (xAux.Year = 1) Then
                            xAux = New Date(1899, 12, 30, xAux.Hour, xAux.Minute, xAux.Second)
                        End If
                        If bolIni Then
                            xMax = xAux
                        ElseIf xAux > xMax Then
                            xMax = xAux
                        End If

                        bolIni = False

                        ''datBeginDate = CDate(oLayer.Data.Item("Begin"))
                        ''datFinishDate = CDate(oLayer.Data.Item("Finish"))

                        ''If Me.StartLimit > datBeginDate Then
                        ''    Me.StartLimit = datBeginDate
                        ''End If

                        ''If Me.EndLimit < datFinishDate Then
                        ''    Me.EndLimit = datFinishDate
                        ''End If

                    End If
                Next

                Me.StartLimit = xMin
                Me.EndLimit = xMax

                If Me.StartLimit = Date.MinValue Then
                    Me.StartLimit = New Date(1899, 12, 30, 0, 0, 0)
                End If

                If Me.EndLimit = Date.MinValue Then
                    Me.EndLimit = New Date(1899, 12, 30, 23, 59, 0)
                End If

            End If

            Me.ManualLimit = False

        End Sub

        Private Function ReplaceShiftNameToObsolete(ByVal ID As Integer, ByVal NewName As String) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSql As String
                strSql = " @UPDATE# Shifts "
                strSql &= " Set Name = '" & NewName & "' "
                strSql &= " Where ID = " & ID

                bolRet = ExecuteSql(strSql)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            End Try

            Return bolRet

        End Function

        Private Function MarkShiftAsObsolete(ByVal ID As Integer) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSql As String
                strSql = "@UPDATE# Shifts "
                strSql = strSql & " Set IsObsolete = 1 "
                strSql = strSql & " Where ID = " & ID

                bolRet = ExecuteSql(strSql)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::MarkShiftAsObsolete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::MarkShiftAsObsolete")
            End Try

            Return bolRet

        End Function

        Private Function GetNextShiftID() As Integer

            Dim intRet As Integer = -1

            Try

                Dim strSql As String = "@SELECT# Max(ID) as counter From Shifts"
                Dim tb As DataTable = CreateDataTable(strSql)

                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Counter")) Then
                        intRet = tb.Rows(0)("Counter") + 1
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::GetNextShiftID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetNextShiftID")
            End Try

            Return intRet

        End Function

        Private Function ExistName() As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# ID FROM Shifts WHERE (ID <> " & Me.intID & ") AND (IsTemplate = 0) AND " &
                                       "((Name = '" & Me.strName & "') OR (ShortName = '" & Me.strShortName & "'))"
                Dim tb As DataTable = CreateDataTable(strSQL)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::ExistName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ExistName")
            End Try

            Return bolRet

        End Function

        Private Function ExistExportName(Optional exportName As String = "", Optional idShiftValidate As Integer = 0) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim exportNameToValidate = If(String.IsNullOrEmpty(exportName), Me.strExportName, exportName)
                Dim shiftToValidate = If(idShiftValidate.Equals(0), Me.intID, idShiftValidate)
                Dim strSQL As String = "@SELECT# ID FROM Shifts WHERE (ID <> " & shiftToValidate & ") AND (IsTemplate = 0) AND " &
                                       "(Export = '" & exportNameToValidate & "')"
                Dim tb As DataTable = CreateDataTable(strSQL)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::ExistExportName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ExistExportName")
            End Try

            Return bolRet

        End Function

        Private Function ReAssignShift(ByVal OldShiftID As Integer, ByVal NewShiftID As Integer, ByVal FromDate As Date) As Boolean
            '
            ' Reasigna los horarios de todos los empleados de OldShiftID a NewShiftID
            '  para las fechas iguales o superiores FromDate. Si no se especifica FromDate
            '  se reasignarán los horarios para todas las fechas
            '
            Dim bolRet As Boolean = False

            Try
                Dim freezeDate As Date = roParameters.GetFirstDate().AddDays(1)
                If FromDate = Date.MinValue Then FromDate = freezeDate

                Dim strSQL As String
                Dim updatePrevious As Boolean = False
                updatePrevious = (OldShiftID <> NewShiftID)

                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift1 =" & NewShiftID & ", Status = 0, [GUID] = '' " &
                         "WHERE IDShift1 = " & OldShiftID &
                                " AND Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                If bolRet AndAlso updatePrevious Then
                    strSQL = $"@UPDATE# DailySchedule WITH (ROWLOCK) SET IDPreviousShift ={NewShiftID}, Status = 0, [GUID] = '' 
                              WHERE IDPreviousShift = {OldShiftID}
                                   AND Date >= {Any2Time(FromDate).SQLSmallDateTime} AND Date >= {Any2Time(freezeDate).SQLSmallDateTime} AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                ' No hace falta aActualizar el puesto asignado en función de la nueva lista de puestos del horario, ya que no dejamos borrar puestos que ya estén asignados a la DailySchedule
                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK)  SET IDShift2 =" & NewShiftID & ", Status = 0, [GUID] = '' " &
                             "WHERE IDShift2 = " & OldShiftID &
                                   " AND Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    'If SQLExecute(sSQL, aConnection) = -1 Then Err.Raise(12364, "SQLExecute Update failed")
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK)  SET IDShift3 =" & NewShiftID & ", Status = 0, [GUID] = '' " &
                             "WHERE IDShift3 = " & OldShiftID &
                                    " AND Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    'If SQLExecute(sSQL, aConnection) = -1 Then Err.Raise(12364, "SQLExecute Update failed")
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK)  SET IDShift4 =" & NewShiftID & ", Status = 0, [GUID] = '' " &
                             "WHERE IDShift4 = " & OldShiftID &
                                   " AND Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    'If SQLExecute(sSQL, aConnection) = -1 Then Err.Raise(12364, "SQLExecute Update failed")
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK)  SET IDShiftBase =" & NewShiftID & ", Status = 0, [GUID] = '' " &
                             "WHERE IDShiftBase = " & OldShiftID &
                                   " AND Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    'If SQLExecute(sSQL, aConnection) = -1 Then Err.Raise(12364, "SQLExecute Update failed")
                End If

                If bolRet Then
                    strSQL = "@UPDATE# ProductiveUnit_Mode_Positions SET IDShift =" & NewShiftID &
                             "WHERE IDShift = " & OldShiftID
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                    If bolRet Then
                        strSQL = "@UPDATE# DailyBudget_Positions SET IDShift =" & NewShiftID &
                             "WHERE IDShift = " & OldShiftID &
                                   " AND  IDDailyBudget IN(@SELECT# id from DailyBudgets where  Date >= " & Any2Time(FromDate).SQLSmallDateTime & " AND Date >= " & Any2Time(freezeDate).SQLSmallDateTime & ")"
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShift::ReAssignShift")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::ReAssignShift")
            End Try

            Return bolRet

        End Function

        Private Function ReplaceExportName(ByVal ID As Integer, ByVal oldExportName As String) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim newExportName = String.Empty
                Dim tmpName As String = String.Empty

                For index As Integer = 1 To 9999
                    tmpName = index.ToString
                    tmpName = "O" & tmpName.PadLeft(4, "0")
                    If Not ExistExportName(tmpName) Then
                        newExportName = tmpName
                        Exit For
                    End If

                Next

                If (newExportName <> String.Empty) Then
                    Dim strSql As String
                    strSql = " @UPDATE# Shifts "
                    strSql &= " Set Export = '" & newExportName & "' "
                    strSql &= " Where ID = " & ID

                    bolRet = ExecuteSql(strSql)
                Else
                    bolRet = False
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Delete"

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim strArray As New ArrayList
            Dim strSql As String

            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift1 = NULL, IDPreviousShift = NULL, Status = 0, [GUID] = '' WHERE IDShift1 = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift2 = NULL, Status = 0, [GUID] = '' WHERE IDShift2 = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift3 = NULL, Status = 0, [GUID] = '' WHERE IDShift3 = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift4 = NULL, Status = 0, [GUID] = '' WHERE IDShift4 = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShiftUsed = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDPreviousShift = NULL, Status = 0, [GUID] = '' WHERE IDPreviousShift = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDAssignment = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IsCovered = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET OldIDAssignment = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET OldIDShift = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDEmployeeCovered = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)

            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift1=NULL, IDShiftUsed=NULL, IDShiftBase=NULL, Status=0, [GUID] = '' , IsHolidays=0, StartShiftBase=NULL, IDAssignmentBase=NULL WHERE IDShiftBase = " & intID)

            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShiftBase = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET StartShiftBase = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IDAssignmentBase = NULL, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)
            strArray.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET IsHolidays = 0, Status = 0, [GUID] = '' WHERE IDShiftUsed = " & intID)

            ''strArray.Add("@DELETE# DailySchedule WHERE IDShift1 IS NULL and IDShift2 IS NULL and IDShift3 IS NULL and IDShift4 IS NULL and IdShiftUsed IS NULL")
            strArray.Add("@DELETE# sysroShiftsLayers WHERE IDSHIFT= " & intID)
            strArray.Add("@DELETE# sysroShiftTimeZones WHERE IDSHIFT= " & intID)
            strArray.Add("@DELETE# sysroShiftsCausesRules WHERE IDSHIFT= " & intID)
            strArray.Add("@DELETE# wtRequest WHERE ShiftID= " & intID)
            strArray.Add("@DELETE# ShiftAssignments WHERE IDShift= " & intID)
            strArray.Add("@DELETE# Shifts WHERE ID= " & intID)

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Verifico si el horario está planificado en algún día anterior a la fecha de cierre. De ser así, no lo permito
                'Dim freezeDate As Date = Support.roLiveSupport.GetFreezeDate()
                strSql = "@SELECT# count(*) from dailyschedule where (idshift1 = " & intID & " OR idshift2 = " & intID
                strSql = strSql & " OR idshift3 = " & intID & " OR idshift4 = " & intID & " OR idshiftUsed = " & intID
                strSql = strSql & " OR OldIDShift = " & intID & " OR IDShiftBase = " & intID & ") AND cast(Date as date) <= (@SELECT# cast(LockDate as date) from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                bolRet = (Any2Double(ExecuteScalar(strSql)) = 0)

                If Not bolRet Then oState.Result = ShiftResultEnum.ShiftPlannedInFreezeDate

                If bolRet Then
                    ' Verifico que no este asignado a ningun presupuesto ni modo
                    strSql = "@SELECT# COUNT(*) FROM ProductiveUnit_Mode_Positions WHERE IDShift = " & intID
                    If Any2Integer(ExecuteScalar(strSql)) > 0 Then
                        bolRet = False
                    End If

                    If bolRet Then
                        strSql = "@SELECT# COUNT(*) FROM DailyBudget_Positions WHERE IDShift = " & intID
                        If Any2Integer(ExecuteScalar(strSql)) > 0 Then
                            bolRet = False
                        End If
                    End If

                    If Not bolRet Then oState.Result = ShiftResultEnum.ShiftAssignmentAssigned
                End If

                If bolRet Then
                    ' Verifico que el horario no se este utilizando en las reglas de solicitudes
                    strSql = "@SELECT# COUNT(*) FROM RequestsRules WHERE IDRequestType in(6,8) AND ',' + CONVERT(xml, RequestsRules.definition).value('(/roCollection/Item[@key=""IDReasons""]/text())[1]', 'varchar(MAX)') + ',' LIKE '%," & intID & ",%'"
                    If Any2Integer(ExecuteScalar(strSql)) > 0 Then
                        bolRet = False
                        oState.Result = ShiftResultEnum.ShiftInRule
                    End If
                End If

                If bolRet Then
                    ' Verifico que el horario no se este utilizando en las reglas diarias de horarios
                    strSql = "@SELECT# COUNT(*) FROM sysroShiftsCausesRules WHERE RuleType = 3 AND ',' + CONVERT(xml, sysroShiftsCausesRules.definition).value('(/roCollection/Item[@key=""PreviousShiftValidationRule""]/text())[1]', 'varchar(MAX)') + ',' LIKE '%," & intID & ",%'"
                    If Any2Integer(ExecuteScalar(strSql)) > 0 Then
                        bolRet = False
                        oState.Result = ShiftResultEnum.ShiftInDailyRule
                    End If
                End If


                If bolRet Then
                    ' Borramos patrón de fichajes si lo hay
                    bolRet = DeleteShiftPunchesPatern(Me.intID, Me.oState)
                End If

                If bolRet Then
                    For Each strSql In strArray
                        bolRet = ExecuteSqlWithoutTimeOut(strSql)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos borrado horario
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ShiftName}", Me.Name, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tShift, Me.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::DeleteShift")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::DeleteShift")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.Delete.ToString)

                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.SHIFTS, oParamsAux)

                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::Delete::Could not send cache update")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetShiftOldestDate(ByVal _IDShift As Integer, ByVal oState As roShiftState) As Date
            'Devuelve la fecha más antigua de las que son usadas por este horario
            Dim xRet As Date = New Date(1900, 1, 1)

            Try
                Dim strSQL As String

                'Construimos la sentencia SQL
                strSQL = $"WITH RankedShifts AS (
                                                    @SELECT# 
                                                        Date,
                                                        ROW_NUMBER() OVER (PARTITION BY IDShift1, IDShift2, IDShift3, IDShift4 ORDER BY Date) AS RowNum
                                                    FROM DailySchedule
                                                    WHERE {_IDShift} IN (IDShift1, IDShift2, IDShift3, IDShift4)
                                                )
                        @SELECT# MIN(Date) AS Minimo
                        FROM RankedShifts
                        WHERE RowNum = 1"

                strSQL &= DataLayer.SQLServerHint.GetSQLHint(DataLayer.SQLServerHint.SelectHinted.GetShiftOldestDate)

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count = 1 AndAlso Not IsDBNull(tb.Rows(0).Item(0)) Then
                    xRet = CDate(tb.Rows(0).Item(0))
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::GetShiftOldestDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetShiftOldestDate")
            Finally

            End Try

            Return xRet

        End Function

        Public Shared Function SaveTimeZone(ByVal _State As roShiftState) As Boolean
            Try
            Catch ex As Exception

            End Try
        End Function

        ''' <summary>
        ''' Devuelve las zonas horarias definidas.
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>DataTable: ID, Name, Description</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTimeZones(ByVal _State As roShiftState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String

                strSQL = "@SELECT# * FROM TimeZones ORDER BY ID"
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShift::GetTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShift::GetTimeZones")
            Finally

            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Devuelve el nombre de un horario
        ''' </summary>
        ''' <param name="iIDShift"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetName(ByVal iIDShift As Integer, ByVal _State As roShiftState) As String
            Dim sRet As String = ""

            Try
                Dim strSQL As String

                strSQL = "@SELECT# Name FROM Shifts WHERE ID = " & iIDShift.ToString
                sRet = ExecuteScalar(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShift::GetName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShift::GetName")
            Finally

            End Try

            Return sRet

        End Function

        Public Shared Function ProcessString(ByVal TextCtrl As String, ByVal _State As roShiftState) As String

            Dim TxtObj As String

            'Busca el nuevo texto en el fichero de configuración
            TxtObj = _State.Language.Translate(TextCtrl, "")
            'TxtObj = INIRead(GetPath("Language") & "\FormsReportsCR" & ActiveLanguage & ".LNG", "Language", TextCtrl, "")

            'Si no existe lo añade con el texto actual
            If Trim(TxtObj) = "" Then
                'INIWrite(GetPath("Language") & "\FormsReportsCR" & ActiveLanguage & ".LNG", "Language", TextCtrl, "#")
            End If

            ' Retorna el valor traducido
            Return TxtObj

        End Function

        Public Shared Function GetIDShiftsCausesRules(ByVal _State As roShiftState) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Try

                Dim strSQL As String = "@SELECT# ID FROM Shifts " &
                                       "WHERE TypeShift IS NULL AND IsObsolete = 0 AND IsTemplate = 0 AND ID > 0 " &
                                       "ORDER BY ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(oRow("ID"))
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShift::GetIDShiftsCausesRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShift::GetIDShiftsCausesRules")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Crea una copia de un horario existente.
        ''' </summary>
        ''' <param name="_IDSourceShift">Código del horario del que se quiere realizar la copia</param>
        ''' <param name="_NewName">Nombre del nuevo horario creado. Si no se informa, se utiliza el tag de idioma 'Shifts.ShiftSave.Copy' para generar el nuevo nombre (copia de ...).</param>
        ''' <param name="_State"></param>
        ''' <returns>Nuevo horario creado</returns>
        ''' <remarks></remarks>
        Public Shared Function CopyShift(ByVal _IDSourceShift As Integer, ByVal _NewName As String, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As roShift

            Dim oRet As roShift = Nothing

            Try

                oRet = New roShift(_IDSourceShift, _State, False)

                oRet.ID = -1

                Dim a As New Random()
                oRet.ShortName = a.Next(100, 999).ToString

                If _NewName = "" Then
                    _State.Language.ClearUserTokens()
                    _State.Language.AddUserToken(oRet.Name)
                    _NewName = _State.Language.Translate("Shifts.ShiftSave.Copy", "")
                    _State.Language.ClearUserTokens()
                End If
                oRet.Name = _NewName
                oRet.ExportName = oRet.GenerateExportName(oRet.ExportName)

                'En caso de ser de tipo Vacaciones y tener asignado un saldo de tipo año laboral, eliminamos el saldo asignado ya que no pueden haber 2 horarios con el mismo saldo de tipo año laboral asignado
                Dim conceptState = New roConceptState(-1)
                Dim oConcept As roConcept = New roConcept(CInt(oRet.IDConceptBalance), conceptState, False)
                If oRet.ShiftType = ShiftType.Vacations AndAlso oConcept.Load() AndAlso oConcept.DefaultQuery = "L" Then
                    oRet.IDConceptBalance = 0
                End If

                If Not oRet.Save(bAudit) Then
                    oRet = Nothing
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShift::CopyShift")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShift::CopyShift")
            End Try

            Return oRet

        End Function

        Private Function GenerateExportName(ByVal oldExportName As String) As String

            Dim newExportName = String.Empty
            Dim bolCloseTransAction As Boolean = False
            Try
                Dim tmpName As String = String.Empty

                For index As Integer = 1 To 9999
                    tmpName = index.ToString
                    tmpName = "C" & tmpName.PadLeft(4, "0")
                    If Not ExistExportName(tmpName) Then
                        newExportName = tmpName
                        Exit For
                    End If

                Next

                Return newExportName
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::ReplaceShiftNameToObsolete")
            End Try

            Return newExportName

        End Function

        Public Shared Function IsShiftPlanned(ByVal _IDShift As Integer, ByVal _State As roShiftState, Optional ByVal _ShiftType As Robotics.Base.DTOs.ActionShiftType = Robotics.Base.DTOs.ActionShiftType.AllShift) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String
                strSQL = "@SELECT# * FROM DailySchedule with (nolock) " &
                         "WHERE "
                Select Case _ShiftType
                    Case Robotics.Base.DTOs.ActionShiftType.PrimaryShift
                        strSQL &= "IDShift1 = " & _IDShift.ToString
                    Case Robotics.Base.DTOs.ActionShiftType.AlterShift
                        strSQL &= "(IDShift2 = " & _IDShift.ToString & " OR IDShift3 = " & _IDShift.ToString & " OR IDShift4 = " & _IDShift.ToString & ") "
                    Case Robotics.Base.DTOs.ActionShiftType.AllShift
                        strSQL &= "(IDShift1 = " & _IDShift.ToString & " OR IDShift2 = " & _IDShift.ToString & " OR IDShift3 = " & _IDShift.ToString & " OR IDShift4 = " & _IDShift.ToString & ") "
                End Select
                strSQL &= "ORDER BY [Date]"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    bolRet = (tb.Rows.Count > 0)
                End If

                If Not bolRet Then
                    ' Verificamos que este asignado a alguna posicion de presupuesto
                    strSQL = "@SELECT# COUNT(*) FROM ProductiveUnit_Mode_Positions WHERE IDShift = " & _IDShift.ToString
                    If Any2Integer(ExecuteScalar(strSQL)) > 0 Then bolRet = True

                    If Not bolRet Then
                        strSQL = "@SELECT# COUNT(*) FROM DailyBudget_Positions WHERE IDShift = " & _IDShift.ToString
                        If Any2Integer(ExecuteScalar(strSQL)) > 0 Then bolRet = True
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShift::IsShiftPlanned")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShift::IsShiftPlanned")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetShiftsByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal oState As roShiftState, Optional ByVal intIDGroup As Integer = -1,
                                                                        Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal _IDAssignment As Integer = -1,
                                                                        Optional ByVal isPortal As Boolean = False, Optional bOnlyControlledByContractAnnualizedConcept As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try
                Dim bolShiftPermission As Boolean = True
                Dim strSQLFilter As String = ""

                Dim strSQL As String = $"@SELECT#   Shifts.ID, 
                                                    isnull(ShiftGroups.Name,'') as GroupName, 
                                                    Shifts.Name as Name, 
                                                    Shifts.Color, 
                                                    Shifts.ShortName, 
                                                    Shifts.ShiftType, 
                                                    Shifts.IsFloating, 
                                                    Shifts.StartFloating, 
                                                    Shifts.VisibilityPermissions, 
                                                    Shifts.VisibilityCriteria, 
                                                    (@SELECT# Count(*) from ShiftAssignments WITH (NOLOCK) WHERE ShiftAssignments.IDShift = Shifts.ID) AS Assignments,
                                                    Concepts.DefaultQuery "

                If isPortal Then
                    strSQL = $"@SELECT#     Shifts.ID, 
                                            Shifts.Name as Name, 
                                            Shifts.ShiftType, 
                                            Shifts.VisibilityPermissions, 
                                            Shifts.VisibilityCriteria, 
                                            Shifts.IsFloating, 
                                            Shifts.AllowComplementary, 
                                            Shifts.AllowFloatingData,   
                                            Shifts.ExpectedWorkingHours,
                                            Concepts.DefaultQuery "
                End If

                If IncludeObsoletes Then strSQL &= ", Shifts.IsObsolete "

                strSQL &= $"    FROM Shifts WITH (NOLOCK) 
                                LEFT OUTER JOIN ShiftGroups WITH (NOLOCK) ON Shifts.IDGroup = ShiftGroups.ID 
                                LEFT OUTER JOIN Concepts ON Concepts.ID = Shifts.IDConceptBalance 
                                WHERE ISNULL(Shifts.VisibilityPermissions,0) IN (0, 2, 3) 
                                  AND (Shifts.ID > 0) 
                                  AND (Shifts.IsTemplate = 0) "

                If Not IncludeObsoletes Then
                    strSQL &= " AND (Shifts.IsObsolete = 0) "
                End If

                If intIDGroup <> -1 Then
                    strSQL &= " AND (Shifts.IdGroup = " & intIDGroup.ToString & ") "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') "

                If bOnlyControlledByContractAnnualizedConcept Then
                    strSQL &= " AND (Concepts.DefaultQuery = 'L') "
                End If

                If _IDAssignment > 0 Then
                    strSQL &= " AND (Shifts.ID IN (@SELECT# ShiftAssignments.IDShift FROM ShiftAssignments WITH (NOLOCK) WHERE ShiftAssignments.IDAssignment = " & _IDAssignment.ToString & "))"
                End If

                strSQL &= " ORDER BY ISNULL(ShiftGroups.Name,'') + ' ' + Shifts.Name"

                oRet = CreateDataTable(strSQL, )

                ' Miramos si hay horarios con visibilidad limitada por colectivos (VisibilityPermissions = 3) para prepapar el filtrado
                Dim collectiveLimitedShiftsDataView As DataView = oRet.DefaultView
                collectiveLimitedShiftsDataView.RowFilter = "VisibilityPermissions = 3"

                Dim employeeCollectivesMembershipTable As New DataTable
                Dim selectorManager As New roSelectorManager

                If collectiveLimitedShiftsDataView.Count > 0 Then
                    Dim uniqueCollectiveIds As HashSet(Of Integer) = collectiveLimitedShiftsDataView.Cast(Of DataRowView)().Select(Function(row) roTypes.Any2String(row("VisibilityCriteria"))).Where(Function(criteria) Not String.IsNullOrEmpty(criteria)).SelectMany(Function(criteria) criteria.Split(","c)).Select(Function(idStr) Integer.Parse(idStr.Trim())).ToHashSet()
                    employeeCollectivesMembershipTable = selectorManager.GetCollectiveEmployees(uniqueCollectiveIds.ToList, intIDEmployee, Now.Date, Now.Date)
                End If

                If intIDEmployee <> -1 Then

                    For Each oRow As DataRow In oRet.Rows
                        bolShiftPermission = True

                        If roTypes.Any2Integer(oRow("VisibilityPermissions")) = 2 Then
                            If oRow("VisibilityCriteria") IsNot Nothing Then
                                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                                roBusinessState.CopyTo(oState, oUserFieldState)
                                Dim lst As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition) = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("VisibilityCriteria")), oUserFieldState, False)
                                If lst IsNot Nothing AndAlso lst.Count > 0 Then
                                    strSQLFilter = ""
                                    For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In lst
                                        strSQLFilter = oCondition.GetFilter(intIDEmployee)
                                        If strSQLFilter <> String.Empty Then
                                            strSQLFilter = " AND " & strSQLFilter
                                        End If
                                    Next
                                    If strSQLFilter <> String.Empty Then
                                        strSQLFilter = "Employees.ID  = " & intIDEmployee.ToString & strSQLFilter
                                        Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", oState)
                                        bolShiftPermission = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                                    End If
                                End If
                            End If
                        ElseIf roTypes.Any2Integer(oRow("VisibilityPermissions")) = 3 Then
                            Dim collectiveIds As String = roTypes.Any2String(oRow("VisibilityCriteria"))
                            If collectiveIds <> String.Empty Then
                                Dim filteredViewAux = employeeCollectivesMembershipTable.DefaultView
                                filteredViewAux.RowFilter = $"IsInCollective = True AND IDCollective IN ({collectiveIds})"

                                If filteredViewAux Is Nothing OrElse filteredViewAux.ToTable.Rows.Count = 0 Then
                                    bolShiftPermission = False
                                End If
                            End If
                        End If

                        If Not bolShiftPermission Then oRow.Delete()
                    Next
                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::GetShiftsByEmployeeVisibilityPermissions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetShiftsByEmployeeVisibilityPermissions")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de horarios de vacaciones
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetHolidaysShifts(ByVal oState As roShiftState, Optional ByVal intIDGroup As Integer = -1,
                                                 Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal OrderByGroup As Boolean = True, Optional bOnlyControlledByContractAnnualizedConcept As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strBusinessGroups As String = GetBusinessGroupList(oState, oState.IDPassport)

                Dim strSQL As String = $"@SELECT#   Shifts.ID, 
                                                    isnull(ShiftGroups.Name,'') as GroupName, 
                                                    Shifts.Name as Name, 
                                                    Shifts.Description as Description, 
                                                    Shifts.Color, 
                                                    Shifts.ShortName, 
                                                    Shifts.ShiftType, 
                                                    Shifts.IsFloating, 
                                                    Shifts.StartFloating,  
                                                    Shifts.VisibilityPermissions, 
                                                    Shifts.VisibilityCriteria, 
                                                    (@SELECT# Count(*) from ShiftAssignments WHERE ShiftAssignments.IDShift = Shifts.ID) AS Assignments,
                                                    Concepts.DefaultQuery"

                If IncludeObsoletes Then strSQL &= ", Shifts.IsObsolete "

                strSQL &= $" FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID 
                             LEFT OUTER JOIN Concepts ON Concepts.ID = Shifts.IDConceptBalance 
                             WHERE (Shifts.ID > 0) AND (Shifts.IsTemplate = 0) "

                If strBusinessGroups <> String.Empty Then
                    strSQL &= " AND ((Shifts.IDGroup = 0) OR (ISNULL(ShiftGroups.BusinessGroup, '') IN (" & strBusinessGroups & "))) "
                End If

                If Not IncludeObsoletes Then
                    strSQL &= " AND (Shifts.IsObsolete = 0) "
                End If

                If intIDGroup <> -1 Then
                    strSQL &= " AND (Shifts.IdGroup = " & intIDGroup.ToString & ") "
                End If

                strSQL = strSQL & " AND (ISNULL(Shifts.TypeShift, '') = '') AND (ISNULL(Shifts.ShiftType, -1) = 2) "

                If bOnlyControlledByContractAnnualizedConcept Then
                    strSQL &= " AND (Concepts.DefaultQuery = 'L') "
                End If

                If OrderByGroup Then
                    strSQL &= " ORDER BY ISNULL(ShiftGroups.Name,'') + ' ' + Shifts.Name"
                Else
                    strSQL &= " ORDER BY Shifts.Name"
                End If
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::GetShiftsByEmployeeVisibilityPermissions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetShiftsByEmployeeVisibilityPermissions")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el ID del horario de vacaciones con el nombre solicitado
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetImportHolidaysShiftID(ByVal oState As roShiftState, ByVal shiftName As String) As Integer

            Dim oRet As Integer = Nothing

            Try

                Dim bolShiftPermission As Boolean = True
                Dim strSQLFilter As String = ""

                Dim strSQL As String = "@SELECT# TOP 1 Shifts.ID "
                strSQL &= " FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID WHERE (Shifts.ID > 0) AND (Shifts.IsTemplate = 0) "
                strSQL &= " AND (ISNULL(Shifts.TypeShift, '') = '') AND (ISNULL(Shifts.ShiftType, -1) = 2) "
                strSQL &= " AND Shifts.Name ='" & shiftName & "'"

                strSQL &= " ORDER BY ISNULL(ShiftGroups.Name,'') + ' ' + Shifts.Name"

                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::GetImportHolidaysShiftID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetImportHolidaysShiftID")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function LoadPunchesPattern(idShift As Integer, ByVal oState As roShiftState) As roShiftPunchesPattern
            Dim oShiftPunchesPattern As roShiftPunchesPattern = New roShiftPunchesPattern
            Dim strSQL As String
            Dim lShiftPatternPunches As New List(Of roShiftPunchesPatternItem)
            Dim bolContinue As Boolean = False

            Try

                strSQL = "@SELECT# DateTime, PunchType FROM ShiftsPunchesPattern WHERE IDShift = " & idShift.ToString & " ORDER BY DateTime ASC	"

                Dim oPunch As roShiftPunchesPatternItem
                Dim tbPattern As DataTable = Nothing
                Dim dPunchDateTime As DateTime = DateTime.MinValue
                tbPattern = CreateDataTable(strSQL)

                If tbPattern IsNot Nothing AndAlso tbPattern.Rows.Count > 0 Then
                    For Each oPatternRow As DataRow In tbPattern.Rows
                        oPunch = New roShiftPunchesPatternItem
                        oPunch.DateTime = roTypes.Any2DateTime(oPatternRow("DateTime"))
                        oPunch.Type = PunchTypeEnum._IN
                        If roTypes.Any2Integer(oPatternRow("PunchType")) = 2 Then oPunch.Type = PunchTypeEnum._OUT
                        lShiftPatternPunches.Add(oPunch)
                    Next
                End If

                oShiftPunchesPattern.Punches = lShiftPatternPunches.ToArray
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::LoadPunchesPattern")
            End Try

            Return oShiftPunchesPattern
        End Function

        Public Shared Function SavePunchesPattern(oPunchesPattern As roShiftPunchesPattern, idShift As Integer, ByVal oState As roShiftState) As Boolean
            Dim bRet As Boolean = True
            Dim strSQL As String
            Dim lShiftPatternPunches As List(Of roShiftPunchesPatternItem)

            Try

                lShiftPatternPunches = oPunchesPattern.Punches.ToList

                For Each oPunch As roShiftPunchesPatternItem In lShiftPatternPunches
                    strSQL = "@INSERT# INTO ShiftsPunchesPattern (IDShift, DateTime, PunchType) VALUES (" &
                             idShift.ToString & ", " &
                             roTypes.Any2Time(oPunch.DateTime).SQLDateTime & ", " &
                             oPunch.Type &
                             ")"
                    If Not ExecuteSql(strSQL) Then
                        bRet = False
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bRet = False
                oState.UpdateStateInfo(ex, "roShift::SavePunchesPattern")
            End Try

            Return bRet
        End Function

        Public Shared Function DeleteShiftPunchesPatern(idShift As Integer, ByVal oState As roShiftState) As Boolean
            Try
                Dim strSQL As String = "@DELETE# ShiftsPunchesPattern WHERE IDShift = " & idShift.ToString
                Return ExecuteSql(strSQL)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::DeleteShiftPunchesPatern")
                Return False
            End Try
        End Function

        Private Shared Function XShiftPatternPunchToDailyRecordPunch(oShiftPatternPunch As roShiftPunchesPatternItem) As roDailyRecordPunch
            Dim oRet As roDailyRecordPunch
            Try
                oRet = New roDailyRecordPunch

                oRet.Type = oShiftPatternPunch.Type
                oRet.DateTime = oShiftPatternPunch.DateTime
            Catch ex As Exception
                oRet = Nothing
            End Try
            Return oRet
        End Function

        Public Shared Function IsShiftFlexible(ByVal idShift As Integer, ByVal oState As roShiftState) As Boolean
            Dim bRet = False
            Try
                bRet = roTypes.Any2Boolean(ExecuteScalar("@SELECT# CASE " &
                                                                          " WHEN " &
                                                                          " SUM(CASE WHEN IDType = 1000 THEN 1 ELSE 0 END) = 1 " &
                                                                          " AND SUM(CASE WHEN IDType IN (1100, 1200, 1300, 1400) THEN 1 ELSE 0 END) = 0 " &
                                                                          " THEN 1 " &
                                                                          " ELSE 0 " &
                                                                          " END AS Flexible " &
                                                                          " FROM " &
                                                                          " sysroShiftsLayers " &
                                                                          " WHERE " &
                                                                          " IDShift = " & idShift))
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::IsShiftFlexible")
            End Try

            Return bRet
        End Function

#End Region

        Public Function GetLayers() As String

            'Franjas Horarias

            Dim strRet As String = ""
            Try

                Dim mCollection As New roCollection
                Dim mParentCollection As New roCollection

                If Me.Layers IsNot Nothing Then

                    Dim oParentLayer As roShiftLayer = Nothing

                    For Each oLayer As roShiftLayer In Me.Layers

                        If strRet <> "" Then
                            strRet &= vbCrLf
                        End If
                        strRet &= oLayer.ParseLayer()

                    Next

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShift::GetLayers")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::GetLayers")
            End Try

            Return strRet

        End Function

        Public Function GetZones() As String

            'Zonas Horarias
            Dim strRet As String = ""

            Try

                If Me.oTimeZones IsNot Nothing Then

                    For Each oZone As roShiftTimeZone In Me.oTimeZones

                        strRet &= oZone.Name & vbNewLine

                        strRet &= roShift.ProcessString("CRUFLCOM.Shifts.From", Me.oState) & " " & Any2Time(oZone.BeginTime).TimeOnly
                        If Any2Time(oZone.BeginTime).VBNumericValue < 0 Then
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.BeforeDay", Me.oState)
                        ElseIf Any2Time(oZone.BeginTime).VBNumericValue > 1 Then
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.AfterDay", Me.oState)
                        Else
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.Day", Me.oState)
                        End If

                        strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.To", Me.oState) & " " & Any2Time(oZone.EndTime).TimeOnly
                        If Any2Time(oZone.EndTime).VBNumericValue < 0 Then
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.BeforeDay", Me.oState)
                        ElseIf Any2Time(oZone.EndTime).VBNumericValue > 1 Then
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.AfterDay", Me.oState)
                        Else
                            strRet &= " " & roShift.ProcessString("CRUFLCOM.Shifts.Day", Me.oState)
                        End If
                        strRet &= vbNewLine & vbNewLine

                    Next

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShift::GetZones")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::GetZones")
            End Try

            Return strRet

        End Function

        Public Function GetRules() As String

            Dim strRet As String = ""

            Try

                If Me.oSimpleRules IsNot Nothing Then

                    Dim LastType As ShiftRuleType = ShiftRuleType.Simple
                    Dim bolFirst As Boolean = True

                    For Each oRule As roShiftRule In Me.oSimpleRules

                        If bolFirst OrElse oRule.Type <> LastType Then

                            bolFirst = False
                            LastType = oRule.Type
                            Select Case oRule.Type
                                Case ShiftRuleType.Simple
                                    ' ...
                                Case ShiftRuleType.Adv
                                    ' ...
                                Case ShiftRuleType.Bonus
                                    strRet &= IIf(Len(strRet) > 0, vbNewLine, "") & roShift.ProcessString("CRUFLCOM.Shifts.BasicRules", Me.oState) & vbNewLine
                            End Select

                        End If

                        strRet &= vbTab & oRule.Description() & vbNewLine

                    Next

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShift::GetRules")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShift::GetRules")
            End Try

            Return strRet

        End Function

        Private Function AssignCausesRules(ByVal _IDShift As Integer) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String

                strSQL = "@DELETE# FROM sysroShiftsCausesRules " &
                         "WHERE IDShift = " & Me.intID.ToString
                If ExecuteSql(strSQL) Then

                    strSQL = "@INSERT# INTO sysroShiftsCausesRules (IDShift, ID, Definition, RuleType) " &
                             "@SELECT# " & Me.intID.ToString & ", ID, Definition, RuleType " &
                             "FROM sysroShiftsCausesRules " &
                             "WHERE IDShift = " & _IDShift.ToString
                    bolRet = ExecuteSql(strSQL)

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::AssignCausesRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::AssignCausesRules")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve el nombre del horario indicando la hora de inicio del flotante.
        ''' </summary>
        ''' <param name="_StartFloating"></param>
        ''' <returns></returns>
        ''' <remarks>Si el horario no es flotante, devuelve el nombre del horario.</remarks>
        Public Function FloatingShiftName(ByVal _StartFloating As DateTime) As String

            Dim strRet As String = Me.Name

            If Me.eShiftType = ShiftType.NormalFloating Then

                strRet = "[" & Format(_StartFloating, "HH:mm")
                If _StartFloating < New DateTime(1899, 12, 30, 0, 0, 0) Then
                    ' Día anterior al del horario
                    strRet &= "-"
                ElseIf _StartFloating > New DateTime(1899, 12, 30, 23, 59, 59) Then
                    ' Día posterior al del horario
                    strRet &= "+"
                End If
                strRet &= "] " & Me.Name

            End If

            Return strRet

        End Function

        Public Function GetExpectedWorkingHoursByFloatingShift(ByVal IDEmployee As Integer, ByVal Ejercicio As Integer, ByVal IDShift As String) As Double
            Dim oRet As Double = 0

            Try

                Dim strSQL As String = "@SELECT# SUM(isnull((case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end), 0)) as Total FROM DailySchedule, Shifts WHERE DailySchedule.IDShift1 = Shifts.ID AND IDShift1 = " & IDShift &
                                            " AND IDEmployee = " & IDEmployee & " AND (YEAR(Date) = " & Ejercicio & ")  "

                oRet = Any2Double(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByFloatingShift")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByFloatingShift")
            Finally

            End Try

            Return oRet
        End Function

        Public Function GetExpectedWorkingHoursByShiftBase(ByVal IDEmployee As Integer, ByVal Ejercicio As Integer, ByVal IDShift As String) As Double
            Dim oRet As Double = 0

            Try

                Dim strSQL As String = "@SELECT# SUM(isnull((case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end), 0)) as Total FROM DailySchedule, Shifts WHERE IDShift1 = " & IDShift &
                                            " AND Shifts.ID = DailySchedule.IDShiftBase AND IDEmployee = " & IDEmployee & " AND (YEAR(Date) = " & Ejercicio & ") AND IDShiftBase IS NOT NULL "

                oRet = Any2Double(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByShiftBase")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByShiftBase")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetExpectedWorkingHoursForEmployeeOnDate(ByVal IDEmployee As Integer, ByVal dDate As Date) As Double
            Dim oRet As Double = 0

            Try

                Dim strSQL As String = "@SELECT# SUM(isnull((case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end), 0)) as Total " &
                                       " FROM DailySchedule WITH (NOLOCK) " &
                                       " INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime &
                                            " AND IDEmployee = " & IDEmployee & " AND IDShift1 IS NOT NULL "

                oRet = Any2Double(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByShiftBase")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetExpectedWorkingHoursByShiftBase")
            End Try

            Return oRet

        End Function

        Public Function GetShiftsTotalsByEmployee(ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer, Optional ByVal IdContract As String = "") As DataSet
            Dim dsRet As DataSet = Nothing

            Try

                Dim dsShiftsTotals As New DataSet
                Dim tbShiftsTotals As New DataTable("ShiftsTotals")

                With tbShiftsTotals
                    .Columns.Add(New DataColumn("Name", GetType(String)))
                    .Columns.Add(New DataColumn("ShortName", GetType(String)))
                    .Columns.Add(New DataColumn("Color", GetType(String)))
                    .Columns.Add(New DataColumn("Quantity", GetType(Integer)))
                    .Columns.Add(New DataColumn("Hours", GetType(Double)))
                    .Columns.Add(New DataColumn("HoursString", GetType(String)))
                End With

                Dim SQL As String
                If IdContract = String.Empty Then
                    SQL = $"@SELECT# COALESCE(t.IDShift,s.Name) as IDShift, t.Total, COALESCE(s.Name, t.IDShift) as Name,COALESCE(s.ShortName, t.IDShift) as ShortName,COALESCE(s.Color, t.ShiftColor1) as Color,COALESCE(s.ShiftType, 1) as ShiftType,COALESCE(s.AllowFloatingData, 0) as AllowFloatingData,COALESCE(s.ExpectedWorkingHours , t.ExpectedWorkingHours) as ExpectedWorkingHours from (
                                @SELECT# ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) AS IDShift, ShiftColor1,(case when IsHolidays=1 then 0 else 1 end) as ExpectedWorkingHours, COUNT(*) AS Total  
                                                FROM DailySchedule 
                                            WHERE IDShift1 IS NOT NULL AND YEAR(Date)= {Ejercicio} AND IDEmployee ={IDEmployee}
                                            GROUP BY ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1),ShiftColor1,case when IsHolidays=1 then 0 else 1 end
                                )t 
                                left Join Shifts s ON TRY_CAST(t.IDShift AS INT) = s.ID 
                                Order by COALESCE(s.Name, t.IDShift)"
                Else
                    SQL = $"@SELECT# COALESCE(t.IDShift,s.Name) as IDShift, t.Total, COALESCE(s.Name, t.IDShift) as Name,COALESCE(s.ShortName, t.IDShift) as ShortName,COALESCE(s.Color, t.ShiftColor1) as Color,COALESCE(s.ShiftType, 1) as ShiftType,COALESCE(s.AllowFloatingData, 0) as AllowFloatingData,COALESCE(s.ExpectedWorkingHours , t.ExpectedWorkingHours) as ExpectedWorkingHours from (
                                @SELECT# ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) AS IDShift, ShiftColor1,(case when IsHolidays=1 then 0 else 1 end) as ExpectedWorkingHours, COUNT(*) AS Total  
	                                FROM DailySchedule
		                                INNER JOIN EmployeeContracts ON DailySchedule.IDEmployee = EmployeeContracts.IDEmployee AND 
					                                DailySchedule.Date >= EmployeeContracts.BeginDate AND DailySchedule.Date <= EmployeeContracts.EndDate 
                                    WHERE DailySchedule.IDShift1 IS NOT NULL AND IDContract = '{IdContract}' and DailySchedule.IDEmployee = {IDEmployee} and YEAR(DailySchedule.Date) = {Ejercicio}
                                    GROUP BY ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) ,ShiftColor1,case when IsHolidays=1 then 0 else 1 end
                                )t 
                                left Join Shifts s ON TRY_CAST(t.IDShift AS INT) = s.ID 
                                Order by COALESCE(s.Name, t.IDShift)"
                End If

                Dim tbEmployeeShifts As DataTable = CreateDataTable(SQL)


                Dim NumHoras As Double = 0
                Dim oShiftRow As DataRow
                For Each oRow As DataRow In tbEmployeeShifts.Rows
                    oShiftRow = tbShiftsTotals.NewRow
                    oShiftRow("Name") = oRow("Name")
                    oShiftRow("shortName") = oRow("ShortName")
                    oShiftRow("Color") = oRow("Color")
                    oShiftRow("Quantity") = roTypes.Any2Integer(oRow("Total"))

                    If Any2Double(oRow("ShiftType")) <> 2 Then
                        If Not Any2Boolean(oRow("AllowFloatingData")) And Not roTypes.Any2String(oRow("IDShift")).StartsWith("U") Then
                            NumHoras = Math.Round(oShiftRow("Quantity") * roTypes.Any2Double(oRow("ExpectedWorkingHours")), 2)
                        Else
                            NumHoras = Math.Round(GetExpectedWorkingHoursByFloatingShift(IDEmployee, Ejercicio, roTypes.Any2String(oRow("IDShift"))), 2)
                        End If
                    Else
                        NumHoras = Math.Round(GetExpectedWorkingHoursByShiftBase(IDEmployee, Ejercicio, roTypes.Any2String(oRow("IDShift"))), 2)
                    End If

                    oShiftRow("Hours") = NumHoras
                    oShiftRow("HoursString") = roConversions.ConvertHoursToTime(System.Math.Abs(CDbl(oShiftRow("Hours"))))

                    tbShiftsTotals.Rows.Add(oShiftRow)
                Next

                tbShiftsTotals.AcceptChanges()
                dsShiftsTotals.Tables.Add(tbShiftsTotals)

                dsRet = dsShiftsTotals


                'Dim tbShifts As DataTable = Shifts(IDGroup, True,, True)
                'If tbShifts IsNot Nothing Then

                '    Dim NumHoras As Double = 0

                '    'obtener veces asignado
                '    Dim SQL As String
                '    If IdContract = String.Empty Then
                '        SQL = $"@SELECT# t.IDShift, t.Total, s.Name,s.ShortName,s.Color from (
                '                @SELECT# ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) AS IDShift, COUNT(*) AS Total  
                '                                FROM DailySchedule 
                '                            WHERE IDShift1 IS NOT NULL AND YEAR(Date)= {Ejercicio} AND IDEmployee ={IDEmployee}
                '                            GROUP BY ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1)
                '                )t 
                '                Inner Join Shifts s on t.IDShift = s.ID
                '                Order by s.Name"
                '    Else
                '        SQL = "@SELECT# ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) AS IDShift1, COUNT(*) AS Total  FROM DailySchedule " &
                '              "INNER JOIN EmployeeContracts ON DailySchedule.IDEmployee = EmployeeContracts.IDEmployee AND " &
                '              "DailySchedule.Date >= EmployeeContracts.BeginDate AND DailySchedule.Date <= EmployeeContracts.EndDate " &
                '              "WHERE DailySchedule.IDShift1 IS NOT NULL AND IDContract = '" & IdContract & "' " &
                '              "GROUP BY DailySchedule.IDEmployee, YEAR(DailySchedule.Date), ISNULL(DailySchedule.ShiftName1,DailySchedule.IDShift1) " &
                '              "HAVING (DailySchedule.IDEmployee = " & IDEmployee & ") AND (YEAR(DailySchedule.Date) = " & Ejercicio & ")"
                '    End If

                '    Dim tbShiftsAssigned As DataTable = CreateDataTable(SQL)
                '    tbShiftsAssigned.PrimaryKey = New DataColumn() {tbShiftsAssigned.Columns("IDShift1")}
                '    Dim dvShiftsAssigned As DataView = tbShiftsAssigned.DefaultView
                '    dvShiftsAssigned.Sort = "IDShift1"

                '    Dim oShiftRow As DataRow
                '    For Each oRow As DataRow In tbShifts.Rows
                '        oShiftRow = tbShiftsTotals.NewRow
                '        oShiftRow("Name") = oRow("Name")
                '        oShiftRow("shortName") = oRow("ShortName")
                '        oShiftRow("Color") = oRow("Color")

                '        dvShiftsAssigned.RowFilter = "(IdShift1 = '" & roTypes.Any2String(oRow("ID")) & "')"
                '        If dvShiftsAssigned.Count = 0 Then
                '            oShiftRow(" ") = 0
                '            oShiftRow("Hours") = 0
                '            oShiftRow("HoursString") = "00:00"
                '        Else
                '            oShiftRow("Quantity") = roTypes.Any2Integer(dvShiftsAssigned(0).Row("Total"))

                '            If Any2Double(oRow("ShiftType")) <> 2 Then
                '                If Not Any2Boolean(oRow("AllowFloatingData")) Then
                '                    NumHoras = Math.Round(oShiftRow("Quantity") * roTypes.Any2Double(oRow("ExpectedWorkingHours")), 2)
                '                Else
                '                    NumHoras = Math.Round(GetExpectedWorkingHoursByFloatingShift(IDEmployee, Ejercicio, roTypes.Any2String(oRow("ID"))), 2)
                '                End If
                '            Else
                '                NumHoras = Math.Round(GetExpectedWorkingHoursByShiftBase(IDEmployee, Ejercicio, roTypes.Any2String(oRow("ID"))), 2)
                '            End If

                '            oShiftRow("Hours") = NumHoras
                '            oShiftRow("HoursString") = roConversions.ConvertHoursToTime(System.Math.Abs(CDbl(oShiftRow("Hours"))))
                '        End If

                '        tbShiftsTotals.Rows.Add(oShiftRow)
                '    Next

                '    tbShiftsTotals.AcceptChanges()
                '    dsShiftsTotals.Tables.Add(tbShiftsTotals)

                '    dsRet = dsShiftsTotals

                'End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetShiftsTotalsByEmployee")
            End Try

            Return dsRet

        End Function
        ''' <summary>
        ''' Devuelve la lista de nombres cortos existentes
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetExistingShortNamesAndExport(ByVal oState As roShiftState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = $"@SELECT#   Shifts.ShortName, Shifts.Export"

                strSQL &= $" FROM Shifts"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::GetExistingShortNames")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::GetExistingShortNames")
            End Try

            Return oRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function IsHolidays(ByVal intIdShift As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String
                strQuery = "@SELECT# ShiftType FROM Shifts WHERE ID = " & intIdShift
                Dim shiftType As Integer = Any2Integer(ExecuteScalar(strQuery))
                If shiftType = 2 Then bolRet = True
            Catch ex As Exception
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function GetShiftIdByExportKey(ByVal sKey As String) As Integer

            Dim oRet As Integer = -1

            Try

                Dim strSQL As String
                If sKey.Trim = String.Empty Then Return oRet

                strSQL = "@SELECT# ID FROM Shifts WHERE Export = '" & sKey & "'"
                oRet = Robotics.VTBase.roTypes.Any2Integer(ExecuteScalar(strSQL))
                If oRet = 0 Then oRet = -1
            Catch ex As Exception
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetShiftExportKeyById(ByVal iID As Integer) As String

            Dim oRet As String = String.Empty

            Try

                Dim strSQL As String
                If iID < 1 Then Return oRet

                strSQL = "@SELECT# Export FROM Shifts WHERE Id = " & iID.ToString
                oRet = Robotics.VTBase.roTypes.Any2String(ExecuteScalar(strSQL))
            Catch ex As Exception
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function Recalculate(ByVal IDShift As Integer, ByVal _IDEmployee As Integer, ByVal _ModifDate As Date, ByVal oState As roShiftState) As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Obtenemos la fecha de inicio del periodo de calculo
                Dim xFirstDate As Date = _ModifDate

                ' Miramos si la fecha obtenida está dentro de periodo de congelación.
                'If xFirstDate <= xFreezingDate Then xFirstDate = xFreezingDate.AddDays(1)

                Dim strSQL As String = ""

                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                         "WHERE STATUS>65 AND IDShift1 = " & IDShift &
                                " AND Date >= " & Any2Time(xFirstDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " &
                                " AND IDEmployee= " & _IDEmployee

                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                             "WHERE STATUS>65 AND IDShift2 = " & IDShift &
                                " AND Date >= " & Any2Time(xFirstDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " &
                                " AND IDEmployee= " & _IDEmployee
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                             "WHERE STATUS>65 AND IDShift3 = " & IDShift &
                                " AND Date >= " & Any2Time(xFirstDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " &
                                " AND IDEmployee= " & _IDEmployee
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                             "WHERE STATUS>65 AND IDShift4 = " & IDShift &
                                " AND Date >= " & Any2Time(xFirstDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " &
                                " AND IDEmployee= " & _IDEmployee
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                             "WHERE STATUS>65 AND IDShiftBase = " & IDShift &
                                " AND Date >= " & Any2Time(xFirstDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  " &
                                " AND IDEmployee= " & _IDEmployee
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShift::Recalculate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::Recalculate")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetBeginHourByShiftDX(ByVal IDEmployee As Long, ByVal IDShift As Integer, ByVal DayDate As String, ByVal Layer As Integer) As String

            Dim strShiftInfo As String = String.Empty
            Dim Result As New roCollection
            Dim dayShift As Integer

            Dim oDataset As Data.DataSet
            Dim oDatareader As Data.Common.DbDataReader

            Try

                Dim sSQL As String
                sSQL = "@select# s.ShiftType, s.IsFloating, s.StartLimit, ds.StartShift1, s.AllowFloatingData, s.AllowComplementary, ds.LayersDefinition"
                sSQL = sSQL & " from DailySchedule ds "
                sSQL = sSQL & " inner join Shifts s on ISNULL(ds.IDShiftUsed, ds.IDShift1) = s.ID "
                sSQL = sSQL & " WHERE IDEmployee=" & IDEmployee
                sSQL = sSQL & " AND Date = " & Any2Time(DayDate).SQLSmallDateTime

                oDataset = CreateDataSet(sSQL)

                If oDataset IsNot Nothing Then
                    oDatareader = oDataset.CreateDataReader()
                    If oDatareader IsNot Nothing Then
                        If oDatareader.HasRows Then
                            If oDatareader.Read() Then
                                Select Case oDatareader("ShiftType")
                                    'Si es de vacaciones no se imprime nada
                                    Case 2
                                        strShiftInfo = ""
                                    'Si es normal
                                    Case 1
                                        'si es flotante
                                        If (oDatareader("IsFloating") = True) Then
                                            If (Layer <> 2) Then
                                                dayShift = Day(Any2Time(oDatareader("StartShift1")).DateOnly)
                                                strShiftInfo = GetSignedBeginShift(dayShift, Any2Time(oDatareader("StartShift1")).TimeOnly)
                                            End If
                                        Else
                                            Select Case True
                                                Case oDatareader("AllowFloatingData"), oDatareader("AllowComplementary")
                                                    Dim ordinaryHours As String = ""
                                                    Dim complementaryHours As String = ""
                                                    Dim beginLayer As String = ""

                                                    Result.LoadXMLString(Any2String(oDatareader("LayersDefinition")))
                                                    If (Layer = 1) Then
                                                        dayShift = Day(Any2Time(Result.Item("LayerFloatingBeginTime_1")).DateOnly)
                                                        beginLayer = GetSignedBeginShift(dayShift, Any2Time(Result.Item("LayerFloatingBeginTime_1")).TimeOnly)
                                                        ordinaryHours = Any2String(Result.Item("LayerOrdinaryHours_1")) & ":00 "
                                                        complementaryHours = Any2String(Result.Item("LayerComplementaryHours_1")) & ":00 "
                                                    ElseIf (Layer = 2) Then
                                                        dayShift = Day(Any2Time(Result.Item("LayerFloatingBeginTime_2")).DateOnly)
                                                        beginLayer = GetSignedBeginShift(dayShift, Any2Time(Result.Item("LayerFloatingBeginTime_2")).TimeOnly)
                                                        ordinaryHours = Any2String(Result.Item("LayerOrdinaryHours_2")) & ":00 "
                                                        complementaryHours = Any2String(Result.Item("LayerComplementaryHours_2")) & ":00 "
                                                    End If
                                                    If (beginLayer <> "") Then
                                                        strShiftInfo = beginLayer & vbCrLf & "HO: " & ordinaryHours & vbCrLf & "HC: " & complementaryHours
                                                    Else
                                                        strShiftInfo = ""
                                                    End If

                                                Case Else
                                                    If (Layer <> 2) Then
                                                        dayShift = Day(Any2Time(oDatareader("StartLimit")).DateOnly)
                                                        strShiftInfo = GetSignedBeginShift(dayShift, Any2Time(oDatareader("StartLimit")).TimeOnly)
                                                    End If
                                            End Select
                                        End If
                                End Select
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try
            Return strShiftInfo
        End Function

        Private Shared Function GetSignedBeginShift(ByVal day As Integer, ByVal hour As String) As String
            Dim sRet As String = ""
            If (hour <> "0:00:00") Then
                Select Case day
                    Case 29
                        sRet = "E:-" & hour
                    Case 30
                        sRet = "E:" & hour
                    Case 31
                        sRet = "E:+" & hour
                End Select
            End If

            Return sRet

        End Function

        Public Shared Function GetLocalizedShiftTime(ByVal time As Date, ByVal refDate As Date) As Date
            Dim dRet As Date

            Try
                Select Case time.Day
                    Case 29
                        dRet = refDate.AddDays(-1) + time.TimeOfDay
                    Case 30
                        dRet = refDate + time.TimeOfDay
                    Case 31
                        dRet = refDate.AddDays(1) + time.TimeOfDay
                End Select
            Catch ex As Exception
                dRet = refDate + time.TimeOfDay
            End Try

            Return dRet

        End Function
#End Region

    End Class

    <DataContract()>
    Public Class roShiftGroup

#Region "Declarations - Constructor"

        Private oState As roShiftState

        Private intID As Integer
        Private strName As String
        Private strBusinessGroup As String

        Private _Audit As Boolean = False

        Public Sub New()
            Me.oState = New roShiftState
            Me.ID = 0
        End Sub

        Public Sub New(ByVal _State As roShiftState)
            Me.oState = _State
            Me.ID = 0
        End Sub

        Public Sub New(ByVal _IDShiftGroup As Integer, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False)
            Me._Audit = bAudit
            Me.oState = _State
            Me.ID = _IDShiftGroup
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property
        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property
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
        Public Property BusinessGroup() As String
            Get
                Return strBusinessGroup
            End Get
            Set(ByVal value As String)
                strBusinessGroup = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            If Me.intID <= 0 Then

                Me.strName = ""
            Else

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM ShiftGroups WHERE [ID] = " & Me.ID.ToString)
                    If tb.Rows.Count > 0 Then

                        Me.strName = tb.Rows(0).Item("Name")
                        Me.strBusinessGroup = roTypes.Any2String(tb.Rows(0).Item("BusinessGroup"))

                        ' Auditamos consulta grupo
                        If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{ShiftGroupName}", Me.Name, "", 1)
                            oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShiftGroup, Me.Name, tbParameters, -1)
                        End If

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roShiftGroup::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roShiftGroup::Load")
                End Try

            End If

        End Sub

        Public Function ValidateShiftGroup() As Boolean
            Dim bolRet As Boolean = False

            If Me.Name = "" Then
                oState.Result = ShiftResultEnum.ShiftGroupNameRequired
                bolRet = False
            Else
                Try

                    Dim strSQL As String = "@SELECT# ID FROM ShiftGroups WHERE Name = @Name AND ID <> @ID"
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    AddParameter(cmd, "@ID", DbType.Int32).Value = Me.intID
                    AddParameter(cmd, "@Name", DbType.String, 50).Value = Me.strName
                    Dim x As Object = cmd.ExecuteScalar()
                    bolRet = x Is Nothing
                Catch ex As DbException
                    oState.UpdateStateInfo(ex, "roShiftGroup::ValidateShiftGroup")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roShiftGroup::ValidateShiftGroup")
                End Try

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strQueryRow As String = ""
                Dim oShiftGroupOld As DataRow = Nothing
                Dim oShiftGroupNew As DataRow = Nothing

                If ValidateShiftGroup() Then

                    strQueryRow = "@SELECT# ID, Name " &
                                  "FROM ShiftGroups WHERE [ID] = " & Me.intID
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "ShiftGroups")
                    If tbAuditOld.Rows.Count = 1 Then oShiftGroupOld = tbAuditOld.Rows(0)

                    Dim tbGroup As New DataTable("ShiftGroups")
                    Dim strSQL As String = "@SELECT# * FROM ShiftGroups WHERE [ID] = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbGroup)

                    Dim oRow As DataRow = Nothing
                    If Me.intID <= 0 Then
                        oRow = tbGroup.NewRow
                        oRow("ID") = Me.GetNextIDShiftGroup()
                    ElseIf tbGroup.Rows.Count = 1 Then
                        oRow = tbGroup.Rows(0)
                    End If

                    oRow("Name") = Me.strName
                    oRow("BusinessGroup") = Me.strBusinessGroup

                    If Me.intID <= 0 Then
                        tbGroup.Rows.Add(oRow)
                    End If

                    da.Update(tbGroup)

                    If Me.intID <= 0 Then
                        Me.intID = oRow("ID")
                    End If

                    If bAudit Then
                        strQueryRow = "@SELECT# ID, Name " &
                                      "FROM ShiftGroups WHERE [ID] = " & Me.intID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "ShiftGroups")
                        If tbAuditNew.Rows.Count = 1 Then oShiftGroupNew = tbAuditNew.Rows(0)

                        ' Insertar registro auditoria
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Dim oAuditAction As Audit.Action = IIf(oShiftGroupOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        oState.AddAuditFieldsValues(tbParameters, oShiftGroupNew, oShiftGroupOld)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oShiftGroupNew("Name")
                        ElseIf oShiftGroupOld("Name") <> oShiftGroupNew("Name") Then
                            strObjectName = oShiftGroupOld("Name") & " -> " & oShiftGroupNew("Name")
                        Else
                            strObjectName = oShiftGroupNew("Name")
                        End If
                        oState.Audit(oAuditAction, Audit.ObjectType.tShiftGroup, strObjectName, tbParameters, -1)
                    End If

                    bolRet = True

                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roShiftGroup::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShiftGroup::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Me.oState.UpdateStateInfo()

            Try

                Dim dSet As DataSet = roShiftGroup.GetShiftsFromGroup(Me.ID, oState)
                If dSet IsNot Nothing Then
                    If dSet.Tables(0).Rows.Count = 0 Then
                        Dim strSql As String = "@DELETE# ShiftGroups WHERE ID = " & Me.ID
                        bolRet = ExecuteSql(strSql)

                        If bolRet And bAudit Then
                            ' Auditamos borrado horario
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{ShiftGroupName}", Me.Name, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tShiftGroup, Me.Name, tbParameters, -1)
                        End If
                    Else
                        oState.Result = ShiftResultEnum.ShiftGroupNotEmpty
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftGroup::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftGroup::Delete")
            End Try

            Return bolRet

        End Function

        Private Function GetNextIDShiftGroup() As Integer

            ' Busca el siguiente ID de grupo.
            Dim intRet As Integer = 1

            Dim strQuery As String = " @SELECT# Max(ID) as Contador From ShiftGroups "
            Dim tb As DataTable = CreateDataTable(strQuery)
            If tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item("Contador")) Then
                    intRet = tb.Rows(0).Item("Contador") + 1
                End If
            End If

            Return intRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un dataset con los grupos disponibles
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetShiftGroups(ByVal oState As roShiftState) As System.Data.DataSet
            Dim oDataset As System.Data.DataSet

            Try
                Dim strQuery As String = "@SELECT# * From ShiftGroups"
                oDataset = CreateDataSet(strQuery, "ShiftGroups")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftGroup::GetShiftGroups")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftGroup::GetShiftGroups")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        ''' <summary>
        ''' Devuelve un dataset con los horarios que pertenecen al grupo pasado por parámetro
        ''' </summary>
        ''' <param name="IDGroup">ID del grupo a recuperar los horarios</param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetShiftsFromGroup(ByVal IDGroup As Integer, ByRef oState As roShiftState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# * from Shifts "
                If (IDGroup <> -1) Then strQuery = strQuery & " Where IDGroup = " & IDGroup
                strQuery = strQuery & " Order By Name "

                oRet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftGroup::GetShiftsFromGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftGroup::GetShiftsFromGroup")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un datatable con los BusinessGroup de los grupos de horarios
        ''' </summary>
        Public Shared Function GetBusinessGroupFromShiftGroups(ByRef oState As roShiftState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroup FROM ShiftGroups GROUP BY BusinessGroup HAVING (BusinessGroup <> '')"
                oRet = CreateDataSet(strQuery)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftGroup::GetBusinessGroupFromShiftGroups")
            End Try

            Return oRet

        End Function

        Public Shared Function BusinessGroupListInUse(ByRef oState As roShiftState, ByVal strBusinessGroup As String, ByVal idGroup As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String
                strQuery = "@SELECT# ID FROM ShiftGroups WHERE (BusinessGroup = '" & strBusinessGroup & "') AND (ID <> " & idGroup & ")"
                Dim oRet As System.Data.DataTable = CreateDataTable(strQuery)
                If oRet IsNot Nothing Then
                    If oRet.Rows.Count > 0 Then
                        bolRet = True
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShift::BusinessGroupListInUse")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace