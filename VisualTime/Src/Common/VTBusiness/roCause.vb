Imports System.Data.Common
Imports System.Math
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace Cause

    <DataContract>
    Public Class roCause

#Region "Declarations - constructor"

        Private oState As roCauseState

        Private intID As Integer
        Private strName As String
        Private intRoundingBy As Decimal = 1
        Private intCostFactor As Decimal = 1
        Private strRoundingType As String
        Private bolAllowInputFromReader As Boolean
        Private intReaderInputcode As Integer
        Private bolWorkingType As Boolean
        Private strDescription As String
        Private intColor As Integer
        Private strShortName As String
        Private bolStartsProgrammedAbsence As Boolean
        Private intMaxProgrammedAbsence As Integer
        Private intAbsenceMandatoryDays As Integer
        Private bolRoundingByDailyScope As Boolean
        Private bolApplyAbsenceOnHolidays As Boolean
        Private intCauseType As Integer
        Private bolPunchCloseProgrammedAbsence As Boolean
        Private intVisibilityPermissions As Integer
        Private lstVisibilityCriteria As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition) 'List(Of roCauseCriteria)
        Private intInputPermissions As Integer
        Private lstInputCriteria As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition) ' List(Of roCauseCriteria)
        Private bolApplyJustifyPeriod As Boolean
        Private intJustifyPeriodStart As Nullable(Of Integer)
        Private intJustifyPeriodEnd As Nullable(Of Integer)
        Private intJustifyPeriodType As Nullable(Of Integer)
        Private strExport As String
        Private bExternalWork As Boolean
        Private intIDConceptBalance As Integer
        Private bIsHoliday As Boolean
        Private bDayType As Boolean
        Private bCustomType As Boolean
        Private strRequestAvailability As String
        Private intIDCategory As CategoryType
        Private intMinLevelOfAuthority As Integer
        Private intApprovedAtLevel As Integer

        Private dMaxTimeToForecast As Decimal = 0

        Private intAutomaticEquivalenceType As Integer
        Private oAutomaticEquivalenceCriteria As New roAutomaticEquivalenceCriteria
        Private intAutomaticEquivalenceIDCause As Integer

        Private strBusinessCenter As String

        Private lstDocuments As Generic.List(Of roCauseDocument)
        Private bolTraceDocumentsAbsences As Boolean
        Private intAbsence_MaxDays As Nullable(Of Integer)
        Private datAbsence_BetweenMax As Nullable(Of DateTime)
        Private datAbsence_BetweenMin As Nullable(Of DateTime)
        Private datAbsence_DurationMax As Nullable(Of DateTime)
        Private datAbsence_DurationMin As Nullable(Of DateTime)

        Private bolApplyWorkDaysOnConcept As Boolean = False

        Private _Audit As Boolean = False

        Private Const MAX_NUMBER_OF_CAUSES_IN_EXPRESS As Byte = 35

        Public Sub New()
            Me.oState = New roCauseState
            Me.ID = -1
            bolApplyWorkDaysOnConcept = False
        End Sub

        Public Sub New(ByVal _IDCause As Integer, ByVal _State As roCauseState, Optional ByVal bAudit As Boolean = False)
            AutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType
            Me.oAutomaticEquivalenceCriteria = New roAutomaticEquivalenceCriteria

            bolApplyWorkDaysOnConcept = False

            _Audit = bAudit
            Me.oState = _State
            Me.intID = _IDCause
            Me.Load(_Audit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roCauseState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roCauseState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
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
        Public Property RoundingBy() As Decimal
            Get
                Return intRoundingBy
            End Get
            Set(ByVal value As Decimal)
                intRoundingBy = value
            End Set
        End Property

        <DataMember>
        Public Property CostFactor() As Decimal
            Get
                Return intCostFactor
            End Get
            Set(ByVal value As Decimal)
                intCostFactor = value
            End Set
        End Property

        <DataMember>
        Public Property MaxTimeToForecast() As Decimal
            Get
                Return dMaxTimeToForecast
            End Get
            Set(ByVal value As Decimal)
                dMaxTimeToForecast = value
            End Set
        End Property

        <DataMember>
        Public Property RoundingType() As eRoundingType
            Get
                Select Case strRoundingType
                    Case "+"
                        Return eRoundingType.Round_UP
                    Case "-"
                        Return eRoundingType.Round_Down
                    Case "~"
                        Return eRoundingType.Round_Near
                End Select
            End Get
            Set(ByVal value As eRoundingType)
                Select Case value
                    Case eRoundingType.Round_UP
                        strRoundingType = "+"
                    Case eRoundingType.Round_Down
                        strRoundingType = "-"
                    Case eRoundingType.Round_Near
                        strRoundingType = "~"
                End Select
            End Set
        End Property

        <DataMember>
        Public Property StringRoundingType() As String
            Get
                Return strRoundingType
            End Get
            Set(ByVal value As String)
                strRoundingType = value
            End Set
        End Property

        <DataMember>
        Public Property AllowInputFromReader() As Boolean
            Get
                Return bolAllowInputFromReader
            End Get
            Set(ByVal value As Boolean)
                bolAllowInputFromReader = value
            End Set
        End Property

        <DataMember>
        Public Property ReaderInputcode() As Integer
            Get
                Return intReaderInputcode
            End Get
            Set(ByVal value As Integer)
                intReaderInputcode = value
            End Set
        End Property

        <DataMember>
        Public Property WorkingType() As Boolean
            Get
                Return bolWorkingType
            End Get
            Set(ByVal value As Boolean)
                bolWorkingType = value
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
        Public Property ShortName() As String
            Get
                Return strShortName
            End Get
            Set(ByVal value As String)
                strShortName = value
            End Set
        End Property

        <DataMember>
        Public Property StartsProgrammedAbsence() As Boolean
            Get
                Return bolStartsProgrammedAbsence
            End Get
            Set(ByVal value As Boolean)
                bolStartsProgrammedAbsence = value
            End Set
        End Property

        <DataMember>
        Public Property MaxProgrammedAbsence() As Integer
            Get
                Return intMaxProgrammedAbsence
            End Get
            Set(ByVal value As Integer)
                intMaxProgrammedAbsence = value
            End Set
        End Property

        <DataMember>
        Public Property AbsenceMandatoryDays() As Integer
            Get
                Return intAbsenceMandatoryDays
            End Get
            Set(ByVal value As Integer)
                intAbsenceMandatoryDays = value
            End Set
        End Property

        <DataMember>
        Public Property RoundingByDailyScope() As Boolean
            Get
                Return bolRoundingByDailyScope
            End Get
            Set(ByVal value As Boolean)
                bolRoundingByDailyScope = value
            End Set
        End Property

        <DataMember>
        Public Property CauseType() As eCauseType
            Get
                Return intCauseType
            End Get
            Set(ByVal value As eCauseType)
                intCauseType = value
            End Set
        End Property

        <DataMember>
        Public Property PunchCloseProgrammedAbsence() As Boolean
            Get
                Return bolPunchCloseProgrammedAbsence
            End Get
            Set(ByVal value As Boolean)
                bolPunchCloseProgrammedAbsence = value
            End Set
        End Property

        <DataMember>
        Public Property VisibilityPermissions() As Integer
            Get
                Return intVisibilityPermissions
            End Get
            Set(ByVal value As Integer)
                intVisibilityPermissions = value
            End Set
        End Property

        <DataMember>
        Public Property VisibilityCriteria() As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Get
                Return Me.lstVisibilityCriteria
            End Get
            Set(ByVal value As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition))
                Me.lstVisibilityCriteria = value
            End Set
        End Property

        <DataMember>
        Public Property InputPermissions() As Integer
            Get
                Return intInputPermissions
            End Get
            Set(ByVal value As Integer)
                intInputPermissions = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticEquivalenceType() As eAutomaticEquivalenceType
            Get
                Return intAutomaticEquivalenceType
            End Get
            Set(ByVal value As eAutomaticEquivalenceType)
                intAutomaticEquivalenceType = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticEquivalenceCriteria() As roAutomaticEquivalenceCriteria
            Get
                Return oAutomaticEquivalenceCriteria
            End Get
            Set(ByVal value As roAutomaticEquivalenceCriteria)
                oAutomaticEquivalenceCriteria = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticEquivalenceIDCause() As Integer
            Get
                Return Me.intAutomaticEquivalenceIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intAutomaticEquivalenceIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property InputCriteria() As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
            Get
                Return Me.lstInputCriteria
            End Get
            Set(ByVal value As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition))
                Me.lstInputCriteria = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyJustifyPeriod() As Boolean
            Get
                Return bolApplyJustifyPeriod
            End Get
            Set(ByVal value As Boolean)
                bolApplyJustifyPeriod = value
            End Set
        End Property

        <DataMember>
        Public Property JustifyPeriodStart() As Nullable(Of Integer)
            Get
                Return intJustifyPeriodStart
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intJustifyPeriodStart = value
            End Set
        End Property

        <DataMember>
        Public Property JustifyPeriodEnd() As Nullable(Of Integer)
            Get
                Return intJustifyPeriodEnd
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intJustifyPeriodEnd = value
            End Set
        End Property

        <DataMember>
        Public Property JustifyPeriodType() As Nullable(Of eJustifyPeriodType)
            Get
                Return intJustifyPeriodType
            End Get
            Set(ByVal value As Nullable(Of eJustifyPeriodType))
                intJustifyPeriodType = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyAbsenceOnHolidays() As Boolean
            Get
                Return bolApplyAbsenceOnHolidays
            End Get
            Set(ByVal value As Boolean)
                bolApplyAbsenceOnHolidays = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyWorkDaysOnConcept() As Boolean
            Get
                Return bolApplyWorkDaysOnConcept
            End Get
            Set(ByVal value As Boolean)
                bolApplyWorkDaysOnConcept = value
            End Set
        End Property

        <DataMember>
        Public Property Documents() As Generic.List(Of roCauseDocument)
            Get
                Return Me.lstDocuments
            End Get
            Set(ByVal value As Generic.List(Of roCauseDocument))
                Me.lstDocuments = value
            End Set
        End Property

        <DataMember>
        Public Property Absence_MaxDays() As Nullable(Of Integer)
            Get
                Return Me.intAbsence_MaxDays
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intAbsence_MaxDays = value
            End Set
        End Property

        <DataMember>
        Public Property Absence_BetweenMax() As Nullable(Of DateTime)
            Get
                Return Me.datAbsence_BetweenMax
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.datAbsence_BetweenMax = value
            End Set
        End Property

        <DataMember>
        Public Property Absence_BetweenMin() As Nullable(Of DateTime)
            Get
                Return Me.datAbsence_BetweenMin
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.datAbsence_BetweenMin = value
            End Set
        End Property

        <DataMember>
        Public Property Absence_DurationMax() As Nullable(Of DateTime)
            Get
                Return Me.datAbsence_DurationMax
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.datAbsence_DurationMax = value
            End Set
        End Property

        <DataMember>
        Public Property Absence_DurationMin() As Nullable(Of DateTime)
            Get
                Return Me.datAbsence_DurationMin
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.datAbsence_DurationMin = value
            End Set
        End Property

        <DataMember>
        Public Property TraceDocumentsAbsences() As Boolean
            Get
                Return Me.bolTraceDocumentsAbsences
            End Get
            Set(ByVal value As Boolean)
                Me.bolTraceDocumentsAbsences = value
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
        Public Property BusinessCenter() As String
            Get
                Return strBusinessCenter
            End Get
            Set(ByVal value As String)
                strBusinessCenter = value
            End Set
        End Property

        <DataMember>
        Public Property ExternalWork() As Boolean
            Get
                Return bExternalWork
            End Get
            Set(ByVal value As Boolean)
                bExternalWork = value
            End Set
        End Property

        <DataMember>
        Public Property IDConceptBalance() As Integer
            Get
                Return intIDConceptBalance
            End Get
            Set(ByVal value As Integer)
                intIDConceptBalance = value
            End Set
        End Property

        <DataMember>
        Public Property IsHoliday() As Boolean
            Get
                Return bIsHoliday
            End Get
            Set(ByVal value As Boolean)
                bIsHoliday = value
            End Set
        End Property

        <DataMember>
        Public Property DayType() As Boolean
            Get
                Return bDayType
            End Get
            Set(ByVal value As Boolean)
                bDayType = value
            End Set
        End Property

        <DataMember>
        Public Property CustomType() As Boolean
            Get
                Return bCustomType
            End Get
            Set(ByVal value As Boolean)
                bCustomType = value
            End Set
        End Property

        <DataMember>
        Public Property RequestAvailability() As String
            Get
                Return strRequestAvailability
            End Get
            Set(ByVal value As String)
                strRequestAvailability = value
            End Set
        End Property
        <DataMember>
        Public Property IDCategory() As CategoryType
            Get
                Return intIDCategory
            End Get
            Set(ByVal value As CategoryType)
                intIDCategory = value
            End Set
        End Property

        <DataMember>
        Public Property MinLevelOfAuthority() As Integer
            Get
                Return intMinLevelOfAuthority
            End Get
            Set(ByVal value As Integer)
                intMinLevelOfAuthority = value
            End Set
        End Property

        <DataMember>
        Public Property ApprovedAtLevel() As Integer
            Get
                Return intApprovedAtLevel
            End Get
            Set(ByVal value As Integer)
                intApprovedAtLevel = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            Try

                Dim strSQL As String
                strSQL = " @SELECT# * from Causes  Where "

                If Me.ReaderInputcode > 0 And Me.ID = -1 Then
                    strSQL = strSQL & " ReaderInputCode=" & Me.ReaderInputcode
                Else
                    strSQL = strSQL & " ID=" & Me.ID
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intID = oRow("ID")
                    Me.Name = oRow("Name")
                    Me.RoundingBy = oRow("RoundingBy")

                    Me.CostFactor = oRow("CostFactor")

                    Me.MaxTimeToForecast = oRow("MaxTimeToForecast")

                    Select Case oRow("RoundingType")
                        Case "+"
                            Me.RoundingType = eRoundingType.Round_UP
                        Case "-"
                            Me.RoundingType = eRoundingType.Round_Down
                        Case "~"
                            Me.RoundingType = eRoundingType.Round_Near
                    End Select
                    Me.AllowInputFromReader = oRow("AllowInputFromReader")
                    If Not IsDBNull(oRow("ReaderInputcode")) Then
                        Me.ReaderInputcode = oRow("ReaderInputcode")
                    Else
                        Me.ReaderInputcode = 0
                    End If
                    Me.WorkingType = oRow("WorkingType")
                    Me.Description = oRow("Description")
                    Me.Color = oRow("Color")
                    Me.ShortName = oRow("ShortName")
                    Me.StartsProgrammedAbsence = oRow("StartsProgrammedAbsence")
                    If Not IsDBNull(oRow("MaxProgrammedAbsenceDays")) Then
                        Me.MaxProgrammedAbsence = oRow("MaxProgrammedAbsenceDays")
                    Else
                        Me.MaxProgrammedAbsence = 0
                    End If

                    If Not IsDBNull(oRow("AbsenceMandatoryDays")) Then
                        Me.AbsenceMandatoryDays = oRow("AbsenceMandatoryDays")
                    Else
                        Me.AbsenceMandatoryDays = -1
                    End If

                    Me.RoundingByDailyScope = oRow("RoundingByDailyScope")
                    If Not IsDBNull(oRow("ApplyAbsenceOnHolidays")) Then
                        Me.ApplyAbsenceOnHolidays = oRow("ApplyAbsenceOnHolidays")
                    Else
                        Me.ApplyAbsenceOnHolidays = False
                    End If
                    Me.CauseType = oRow("CauseType")
                    Me.PunchCloseProgrammedAbsence = oRow("PunchCloseProgrammedAbsence")

                    Me.VisibilityPermissions = oRow("VisibilityPermissions")
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Me.lstVisibilityCriteria = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("VisibilityCriteria")), oUserFieldState, False)
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)

                    Me.InputPermissions = oRow("InputPermissions")
                    oUserFieldState = New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Me.lstInputCriteria = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("InputCriteria")), oUserFieldState, False)
                    roBusinessState.CopyTo(oUserFieldState, Me.oState)

                    Me.ApplyJustifyPeriod = oRow("ApplyJustifyPeriod")
                    If Not IsDBNull(oRow("JustifyPeriodStart")) Then
                        Me.JustifyPeriodStart = oRow("JustifyPeriodStart")
                    End If
                    If Not IsDBNull(oRow("JustifyPeriodEnd")) Then
                        Me.JustifyPeriodEnd = oRow("JustifyPeriodEnd")
                    End If
                    If Not IsDBNull(oRow("JustifyPeriodType")) Then
                        If oRow("JustifyPeriodType") = True Then
                            Me.JustifyPeriodType = eJustifyPeriodType.JustifyPeriod
                        Else
                            Me.JustifyPeriodType = eJustifyPeriodType.DontJustify
                        End If

                    End If

                    If Not IsDBNull(oRow("Export")) Then
                        Me.strExport = oRow("Export")
                    End If

                    If Not IsDBNull(oRow("ExternalWork")) Then
                        Me.ExternalWork = oRow("ExternalWork")
                    End If

                    If Not IsDBNull(oRow("IsHoliday")) Then
                        Me.IsHoliday = roTypes.Any2Boolean(oRow("IsHoliday"))
                    End If

                    If Not IsDBNull(oRow("DayType")) Then
                        Me.DayType = roTypes.Any2Boolean(oRow("DayType"))
                    End If

                    If Not IsDBNull(oRow("CustomType")) Then
                        Me.CustomType = roTypes.Any2Boolean(oRow("CustomType"))
                    End If

                    If Not IsDBNull(oRow("BusinessGroup")) Then
                        Me.strBusinessCenter = oRow("BusinessGroup")
                    End If

                    Me.intIDConceptBalance = IIf(Not IsDBNull(oRow("IDConceptBalance")), oRow("IDConceptBalance"), 0)

                    Me.TraceDocumentsAbsences = roTypes.Any2Boolean(oRow("TraceDocumentsAbsences"))

                    Try
                        Dim oCollection As New roCollection(roTypes.Any2String(oRow("DefaultValuesAbsences")))

                        If Not oCollection("MaxDays") Is Nothing Then Me.intAbsence_MaxDays = roTypes.Any2Integer(oCollection("MaxDays"))

                        If Not oCollection("BetweenMax") Is Nothing Then Me.datAbsence_BetweenMax = roTypes.Any2DateTime(oCollection("BetweenMax"))
                        If Not oCollection("BetweenMin") Is Nothing Then Me.datAbsence_BetweenMin = roTypes.Any2DateTime(oCollection("BetweenMin"))

                        If Not oCollection("DurationMax") Is Nothing Then Me.datAbsence_DurationMax = roTypes.Any2DateTime(oCollection("DurationMax"))
                        If Not oCollection("DurationMin") Is Nothing Then Me.datAbsence_DurationMin = roTypes.Any2DateTime(oCollection("DurationMin"))
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Load")
                    End Try

                    Me.lstDocuments = roCauseDocument.GetDocumentsByIdCause(Me.ID, oState)

                    Me.AutomaticEquivalenceType = roTypes.Any2Integer(oRow("AutomaticEquivalenceType"))
                    Me.AutomaticEquivalenceIDCause = roTypes.Any2Integer(oRow("AutomaticEquivalenceIDCause"))

                    If Me.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType Then
                        Me.oAutomaticEquivalenceCriteria = New roAutomaticEquivalenceCriteria
                        Me.oAutomaticEquivalenceCriteria.AutomaticEquivalenceType = Me.AutomaticEquivalenceType
                        Me.oAutomaticEquivalenceCriteria.LoadFromXml(roTypes.Any2String(oRow("AutomaticEquivalenceCriteria")))
                    End If

                    If Not IsDBNull(oRow("RequestAvailability")) Then
                        Me.RequestAvailability = roTypes.Any2String(oRow("RequestAvailability"))
                    Else
                        Me.RequestAvailability = "-1"
                    End If

                    Me.ApplyWorkDaysOnConcept = False
                    If Not IsDBNull(oRow("ApplyWorkDaysOnConcept")) Then
                        Me.ApplyWorkDaysOnConcept = roTypes.Any2Boolean(oRow("ApplyWorkDaysOnConcept"))
                    End If

                    If Not IsDBNull(oRow("MinLevelOfAuthority")) Then
                        Me.MinLevelOfAuthority = roTypes.Any2Integer(oRow("MinLevelOfAuthority"))
                    Else
                        Me.MinLevelOfAuthority = 11
                    End If

                    If Not IsDBNull(oRow("ApprovedAtLevel")) Then
                        Me.ApprovedAtLevel = roTypes.Any2Integer(oRow("ApprovedAtLevel"))
                    Else
                        Me.ApprovedAtLevel = 1
                    End If

                    If Not IsDBNull(oRow("IDCategory")) Then
                        Me.IDCategory = roTypes.Any2Integer(oRow("IDCategory"))
                    Else
                        Me.IDCategory = 6
                    End If

                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", IIf(Me.strName Is Nothing, "", Me.strName), "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCause, IIf(Me.strName Is Nothing, "", Me.strName), tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::Load")
            Finally

            End Try

        End Sub

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = CauseResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim bolIsNew As Boolean = False
                Dim bolIsUsed As Boolean = False
                Dim bolHasRecalculateChanges As Boolean = False
                Dim bolHasRecalculateRequests As Boolean = False

                Dim intIDCause As Integer = -1
                Dim strQuery As String = String.Empty

                Dim oOldCause As roCause = Nothing

                If ValidateCause() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim strDefaultValuesAbsences As String = String.Empty
                    Try
                        Dim oCollection As New roCollection()
                        If Me.intAbsence_MaxDays.HasValue Then oCollection.Add("MaxDays", Me.Absence_MaxDays)
                        If Me.datAbsence_BetweenMax.HasValue Then oCollection.Add("BetweenMax", Me.Absence_BetweenMax)
                        If Me.datAbsence_BetweenMin.HasValue Then oCollection.Add("BetweenMin", Me.Absence_BetweenMin)
                        If Me.datAbsence_DurationMax.HasValue Then oCollection.Add("DurationMax", Me.Absence_DurationMax)
                        If Me.datAbsence_DurationMin.HasValue Then oCollection.Add("DurationMin", Me.Absence_DurationMin)

                        strDefaultValuesAbsences = oCollection.XML()
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Save")
                    End Try

                    If Me.ID = -1 Then
                        bolIsNew = True

                        Do
                            intIDCause = GetNextIDCause()
                            strQuery = " @INSERT# INTO Causes "
                            strQuery = strQuery & " ( ID, Name, RoundingBy, RoundingType, "
                            strQuery = strQuery & " AllowInputFromReader, ReaderInputCode, WorkingType, "
                            strQuery = strQuery & " Description, Color, ShortName, "
                            strQuery = strQuery & " StartsProgrammedAbsence, MaxProgrammedAbsenceDays, RoundingByDailyScope, "
                            strQuery = strQuery & " ApplyAbsenceOnHolidays, CauseType, PunchCloseProgrammedAbsence, "
                            strQuery = strQuery & " VisibilityPermissions, VisibilityCriteria, InputPermissions, "
                            strQuery = strQuery & " InputCriteria, ApplyJustifyPeriod, JustifyPeriodStart,"
                            strQuery = strQuery & " JustifyPeriodEnd, JustifyPeriodType, DefaultValuesAbsences, "
                            strQuery = strQuery & " TraceDocumentsAbsences, Export, CostFactor, IDConceptBalance, ExternalWork, IsHoliday, DayType, CustomType, "
                            strQuery = strQuery & " AutomaticEquivalenceType, AutomaticEquivalenceCriteria, AutomaticEquivalenceIDCause, MaxTimeToForecast,BusinessGroup, AbsenceMandatoryDays, RequestAvailability, ApplyWorkDaysOnConcept,MinLevelOfAuthority,ApprovedAtLevel,IDCategory) "
                            strQuery = strQuery & " Values "
                            strQuery = strQuery & " ( " & intIDCause & ", "
                            strQuery = strQuery & " '" & Me.Name.Replace("'", "''") & "' , " & Replace(Me.RoundingBy.ToString, ",", ".") & ", '" & Me.StringRoundingType & "' "
                            strQuery = strQuery & ", " & Abs(CInt(Me.AllowInputFromReader)) & ", " & Me.ReaderInputcode & ", " & Abs(CInt(Me.WorkingType))
                            strQuery = strQuery & ", '" & Me.Description.Replace("'", "''") & "', " & Me.Color & ", '" & Me.ShortName.Replace("'", "''") & "' "
                            strQuery = strQuery & ", " & Abs(CInt(Me.StartsProgrammedAbsence))
                            strQuery = strQuery & ", " & Me.MaxProgrammedAbsence
                            strQuery = strQuery & ", " & Abs(CInt(Me.RoundingByDailyScope))
                            strQuery = strQuery & ", " & Abs(CInt(ApplyAbsenceOnHolidays))
                            strQuery = strQuery & ", " & CauseType
                            strQuery = strQuery & ", " & Abs(CInt(PunchCloseProgrammedAbsence))
                            strQuery = strQuery & ", " & VisibilityPermissions

                            If Me.VisibilityCriteria IsNot Nothing Then
                                If Me.VisibilityCriteria.Count > 1 Then
                                    Me.VisibilityCriteria.RemoveRange(0, VisibilityCriteria.Count - 1)
                                End If
                                strQuery &= ",'" & Replace(VTUserFields.UserFields.roUserFieldCondition.GetXml(VisibilityCriteria), "'", "''") & "'"
                            Else
                                strQuery = strQuery & ", NULL"
                            End If
                            strQuery = strQuery & ", " & InputPermissions
                            If Me.InputCriteria IsNot Nothing Then
                                strQuery &= ",'" & Replace(VTUserFields.UserFields.roUserFieldCondition.GetXml(Me.lstInputCriteria), "'", "''") & "'"
                            Else
                                strQuery = strQuery & ", NULL"
                            End If

                            strQuery = strQuery & ", " & Abs(CInt(ApplyJustifyPeriod))
                            If Me.JustifyPeriodStart.HasValue Then
                                strQuery = strQuery & ", " & Me.JustifyPeriodStart.ToString
                            Else
                                strQuery = strQuery & ", NULL"
                            End If
                            If Me.JustifyPeriodEnd.HasValue Then
                                strQuery = strQuery & ", " & Me.JustifyPeriodEnd.ToString
                            Else
                                strQuery = strQuery & ", NULL"
                            End If

                            strQuery = strQuery & ", " & CInt(JustifyPeriodType)
                            strQuery = strQuery & ", '" & strDefaultValuesAbsences & "'"
                            strQuery = strQuery & ", " & Abs(CInt(Me.TraceDocumentsAbsences))

                            If Me.strExport <> "" Then
                                strQuery = strQuery & ", '" & Me.strExport & "'"
                            Else
                                strQuery = strQuery & ", NULL"
                            End If

                            strQuery = strQuery & ", " & Replace(Me.CostFactor.ToString, ",", ".")
                            strQuery = strQuery & ", " & Abs(roTypes.Any2Integer(Me.IDConceptBalance))
                            strQuery = strQuery & ", " & Abs(CInt(Me.ExternalWork))
                            strQuery = strQuery & ", " & Abs(CInt(Me.IsHoliday))
                            strQuery = strQuery & ", " & Abs(CInt(Me.DayType))
                            strQuery = strQuery & ", " & Abs(CInt(Me.CustomType))
                            strQuery = strQuery & ", " & Abs(CInt(Me.AutomaticEquivalenceType))

                            If Me.oAutomaticEquivalenceCriteria IsNot Nothing Then
                                strQuery &= ",'" & Replace(oAutomaticEquivalenceCriteria.GetXml, "'", "''") & "'"
                            Else
                                strQuery = strQuery & ", NULL"
                            End If

                            strQuery = strQuery & ", " & IIf(Me.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType, Me.AutomaticEquivalenceIDCause, 0)

                            strQuery = strQuery & ", " & Replace(Me.MaxTimeToForecast.ToString, ",", ".")

                            If Me.strBusinessCenter <> "" Then
                                strQuery = strQuery & ", '" & Me.strBusinessCenter & "'"
                            Else
                                strQuery = strQuery & ", NULL"
                            End If

                            strQuery = strQuery & ", " & Me.intAbsenceMandatoryDays

                            strQuery = strQuery & ", " & Me.RequestAvailability

                            strQuery = strQuery & ", " & Abs(CInt(Me.ApplyWorkDaysOnConcept))

                            strQuery = strQuery & ", " & Me.MinLevelOfAuthority

                            strQuery = strQuery & ", " & Me.ApprovedAtLevel

                            strQuery = strQuery & ", " & Me.IDCategory

                            strQuery = strQuery & " ) "

                            'Documents
                            If Me.lstDocuments IsNot Nothing AndAlso Me.lstDocuments.Count > 0 Then

                                For Each doc As roCauseDocument In Me.Documents
                                    doc.IDCause = Me.intID
                                Next

                                roCauseDocument.SaveDocumentsByIdCause(Me.ID, Me.lstDocuments, oState)
                            Else
                                roCauseDocument.DeleteDocumentsByIdCause(Me.ID, oState)
                            End If

                            Try
                                If ExecuteSql(strQuery) Then
                                    oState.Result = CauseResultEnum.NoError
                                    Me.ID = intIDCause
                                    bolRet = True
                                Else
                                    oState.Result = CauseResultEnum.ConnectionError
                                End If
                                oState.ErrorText = ""
                            Catch ex As System.Data.Common.DbException
                                oState.UpdateStateInfo(ex, "roCause::Save")
                            Catch ex As Exception
                                oState.UpdateStateInfo(ex, "roCause::Save")
                            End Try
                        Loop Until InStr(oState.ErrorText, "PRIMARY KEY") = 0
                    Else

                        bolIsUsed = Me.IsUsed()

                        ' En el caso que sea de tipo días
                        ' hay que revisar si esta activo la equivalencia
                        If Not bolIsUsed Then
                            If CauseUsedWithEquivalenceCause() <> String.Empty Then bolIsUsed = True
                        End If

                        Dim tb As New DataTable("Causes")
                        Dim strSQL As String = "@SELECT# * FROM Causes " &
                                               "WHERE ID = " & Me.ID.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                        Else
                            oRow = tb.Rows(0)
                            bolHasRecalculateChanges = Me.HasRecalculateChanges(oRow)
                            bolHasRecalculateRequests = Me.HasRecalculateRequests(oRow)
                            oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        End If

                        oOldCause = New roCause(Me.ID, Me.oState, False)

                        intIDCause = Me.ID
                        strQuery = " @UPDATE# Causes "
                        strQuery = strQuery & " set Name = N'" & Me.Name.Replace("'", "''") & "' "
                        strQuery = strQuery & " , RoundingBy = " & Replace(Me.RoundingBy.ToString, ",", ".")
                        strQuery = strQuery & ", RoundingType = '" & Me.StringRoundingType & "' "
                        strQuery = strQuery & ", AllowInputFromReader = " & Abs(CInt(Me.AllowInputFromReader))
                        strQuery = strQuery & ", ReaderInputcode = " & Abs(CInt(Me.ReaderInputcode))
                        strQuery = strQuery & ", WorkingType = " & Abs(CInt(Me.WorkingType))
                        strQuery = strQuery & ", Description = '" & Me.Description.Replace("'", "''") & "'"
                        strQuery = strQuery & ", Color = " & Me.Color
                        strQuery = strQuery & ", ShortName = '" & Me.ShortName.Replace("'", "''") & "' "
                        strQuery = strQuery & ", StartsProgrammedAbsence = " & Abs(CInt(Me.StartsProgrammedAbsence))
                        strQuery = strQuery & ", MaxProgrammedAbsenceDays = " & Me.MaxProgrammedAbsence
                        strQuery = strQuery & ", AbsenceMandatoryDays = " & Me.AbsenceMandatoryDays
                        strQuery = strQuery & ", RoundingByDailyScope = " & Abs(CInt(Me.RoundingByDailyScope))
                        strQuery = strQuery & ", ApplyAbsenceOnHolidays = " & Abs(CInt(ApplyAbsenceOnHolidays))
                        strQuery = strQuery & ", CauseType = " & CInt(CauseType)
                        strQuery = strQuery & ", PunchCloseProgrammedAbsence = " & Abs(CInt(PunchCloseProgrammedAbsence))
                        strQuery = strQuery & ", VisibilityPermissions = " & CInt(VisibilityPermissions)

                        If VisibilityCriteria IsNot Nothing Then
                            If VisibilityCriteria.Count > 1 Then
                                VisibilityCriteria.RemoveRange(0, VisibilityCriteria.Count - 1)
                            End If
                            strQuery &= ", VisibilityCriteria = '" & Replace(VTUserFields.UserFields.roUserFieldCondition.GetXml(VisibilityCriteria), "'", "''") & "'"
                        Else
                            strQuery = strQuery & ", VisibilityCriteria = NULL"
                        End If
                        strQuery = strQuery & ", InputPermissions = " & CInt(InputPermissions)
                        If InputCriteria IsNot Nothing Then
                            strQuery &= ", InputCriteria = '" & Replace(VTUserFields.UserFields.roUserFieldCondition.GetXml(Me.lstInputCriteria), "'", "''") & "'"
                        Else
                            strQuery = strQuery & ", InputCriteria = NULL"
                        End If
                        strQuery = strQuery & ", ApplyJustifyPeriod = " & Abs(CInt(ApplyJustifyPeriod))
                        If Me.JustifyPeriodStart.HasValue Then
                            strQuery = strQuery & ", JustifyPeriodStart = " & CInt(Me.JustifyPeriodStart.ToString)
                        Else
                            strQuery = strQuery & ", JustifyPeriodStart = NULL"
                        End If

                        If Me.JustifyPeriodEnd.HasValue Then
                            strQuery = strQuery & ", JustifyPeriodEnd = " & CInt(Me.JustifyPeriodEnd.ToString)
                        Else
                            strQuery = strQuery & ", JustifyPeriodEnd = NULL"
                        End If

                        If Me.strExport <> "" Then
                            strQuery = strQuery & ", Export = '" & Me.strExport & "'"
                        Else
                            strQuery = strQuery & ", Export = NULL"
                        End If

                        If Me.strBusinessCenter <> "" Then
                            strQuery = strQuery & ", BusinessGroup = '" & Me.strBusinessCenter & "'"
                        Else
                            strQuery = strQuery & ", BusinessGroup = NULL"
                        End If

                        strQuery = strQuery & ", JustifyPeriodType = " & CInt(JustifyPeriodType)
                        strQuery = strQuery & ", DefaultValuesAbsences = '" & strDefaultValuesAbsences & "'"
                        strQuery = strQuery & ", TraceDocumentsAbsences = " & Abs(CInt(Me.TraceDocumentsAbsences))

                        strQuery = strQuery & ", CostFactor = " & Replace(Me.CostFactor.ToString, ",", ".")
                        strQuery = strQuery & ", MaxTimeToForecast = " & Replace(Me.MaxTimeToForecast.ToString, ",", ".")

                        strQuery = strQuery & ", ExternalWork = " & Abs(CInt(Me.ExternalWork))
                        strQuery = strQuery & ", IsHoliday = " & Abs(CInt(Me.IsHoliday))
                        strQuery = strQuery & ", DayType = " & Abs(CInt(Me.DayType))
                        strQuery = strQuery & ", CustomType = " & Abs(CInt(Me.CustomType))

                        strQuery = strQuery & ", AutomaticEquivalenceType = " & Abs(CInt(Me.AutomaticEquivalenceType))

                        If Me.oAutomaticEquivalenceCriteria IsNot Nothing Then
                            strQuery = strQuery & ", AutomaticEquivalenceCriteria = '" & Replace(oAutomaticEquivalenceCriteria.GetXml, "'", "''") & "'"
                        Else
                            strQuery = strQuery & ", AutomaticEquivalenceCriteria = NULL "
                        End If

                        If Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType Then
                            strQuery = strQuery & ", AutomaticEquivalenceIDCause = 0 "
                        Else
                            strQuery = strQuery & ", AutomaticEquivalenceIDCause = " & Abs(CInt(Me.AutomaticEquivalenceIDCause))
                        End If

                        strQuery = strQuery & ", IDConceptBalance = " & Me.intIDConceptBalance
                        strQuery = strQuery & ", RequestAvailability ='" & Me.RequestAvailability & "'"
                        strQuery = strQuery & ", ApplyWorkDaysOnConcept=" & Abs(CInt(Me.ApplyWorkDaysOnConcept))
                        strQuery = strQuery & ", MinLevelOfAuthority = " & Me.MinLevelOfAuthority
                        strQuery = strQuery & ", ApprovedAtLevel = " & Me.ApprovedAtLevel
                        strQuery = strQuery & ", IDCategory = " & Me.IDCategory

                        strQuery = strQuery & " Where Id = " & Me.ID

                        'Documents
                        If Me.Documents IsNot Nothing AndAlso Me.Documents.Count > 0 Then
                            roCauseDocument.SaveDocumentsByIdCause(Me.ID, Me.Documents, oState)
                        Else
                            roCauseDocument.DeleteDocumentsByIdCause(Me.ID, oState)
                        End If

                        Try
                            If ExecuteSql(strQuery) Then
                                oState.Result = CauseResultEnum.NoError
                                bolRet = True
                            Else
                                oState.Result = CauseResultEnum.ConnectionError
                            End If
                        Catch ex As Data.Common.DbException
                            oState.UpdateStateInfo(ex, "roCause::Save")
                        Catch ex As Exception
                            oState.UpdateStateInfo(ex, "roCause::Save")
                        End Try
                    End If

                    ' ESPECIAL: TorrasPapel
                    If bolRet Then
                        Try
                            Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                            If roTypes.Any2String(customization) = "SARROT" Then
                                ' Marco para envío a Meta4. Sea de cuando sea la ausencia, marco el día de hoy para que se envíe ya. De todos modos, se envían todas desde cero ...
                                strQuery = "@UPDATE# CAUSES SET SEND=0"
                                ExecuteSql(strQuery)
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    Dim tbNew As New DataTable("Causes")
                    Dim strSQLNEw As String = "@SELECT# * FROM Causes " &
                                           "WHERE ID = " & intIDCause
                    Dim cmdNew As DbCommand = CreateCommand(strSQLNEw)
                    Dim daNew As DbDataAdapter = CreateDataAdapter(cmdNew, True)
                    daNew.Fill(tbNew)

                    If tbNew.Rows.Count = 0 Then
                    Else
                        oAuditDataNew = Extensions.roAudit.CloneRow(tbNew.Rows(0))
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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tCause, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet Then

                        ' Notificamos al servidor
                        'Update a 0 dels registres de dailyschedule afectats + init task detector + actual
                        If bolRet AndAlso Not bolIsNew AndAlso bolIsUsed AndAlso bolHasRecalculateChanges Then
                            bolRet = UpdateDailySchedule(oOldCause)
                        End If

                        If bolRet AndAlso Not bolIsNew AndAlso bolHasRecalculateRequests Then
                            ' lanzamos la tarea de recalculo de estado de solicitudes en caso necesario
                            Dim oStateTask As New roLiveTaskState(-1)

                            Dim oParameters As New roCollection
                            oParameters.Add("IDCause", intIDCause)
                            oParameters.Add("IDRequestType", eRequestType.None)

                            roLiveTask.CreateLiveTask(roLiveTaskTypes.RecalculateRequestStatus, oParameters, oStateTask)

                        End If

                    End If

                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roCause::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCause::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.InsertOrUpdate.ToString)

                    roConnector.InitTask(TasksType.CAUSES, oParamsAux)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCause::Save::Could not send cache update")
            End Try

            Return bolRet

        End Function

        Private Function UpdateDailySchedule(Optional ByVal oOldCause As roCause = Nothing) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sqlFreezeDate As String = roTypes.Any2Time(roParameters.GetFirstDate()).SQLSmallDateTime

                Dim strObjectIDs As String = String.Empty
                Dim strSQL As String = String.Empty

                If CauseUsedInShift(strObjectIDs) <> String.Empty Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                              FROM DailySchedule
	                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
                              WHERE (DailySchedule.IDShiftUsed IS NOT NULL
                                    AND DailySchedule.IDShiftUsed IN (" & strObjectIDs & ")
                                    )
                                AND DailySchedule.Date > @date
                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet AndAlso CauseUsedinAccrual(strObjectIDs) <> String.Empty Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                              WHERE EXISTS (
                                            @SELECT#
					                            1 AS ExistReg
				                            FROM DailyAccruals
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE DailyAccruals.IDConcept IN(" & strObjectIDs & ")
					                            AND DailySchedule.Date > @date
					                            AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
					                            AND DailyAccruals.Date = DailySchedule.Date
                                                AND DailyAccruals.IDEmployee = DailySchedule.IDEmployee
			                               )"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet AndAlso CauseUsedinCauseLimits(strObjectIDs) <> String.Empty Then
                    strSQL = "@DECLARE#  @date SMALLDATETIME = " & sqlFreezeDate & "

                             @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                             FROM DailySchedule
                             WHERE EXISTS (
				                            @SELECT#
					                            1 AS ExistReg
				                            FROM DailyCauses
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE DailyCauses.IDCause in (" & strObjectIDs & ")
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailyCauses.Date = DailySchedule.Date
                                                AND DailyCauses.IDEmployee = DailySchedule.IDEmployee
                                          )"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet AndAlso CauseUsedinCauses() <> String.Empty Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                              WHERE EXISTS (
				                            @SELECT#
					                            1 AS ExistReg
				                            FROM DailyCauses
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE DailyCauses.IDCause = " & Me.ID & "
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailyCauses.Date = DailySchedule.Date
				                                AND DailyCauses.IDEmployee = DailySchedule.IDEmployee
			                               )"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet AndAlso CauseUsedWithEquivalenceCause() <> String.Empty Then
                    strSQL = "@DECLARE#  @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0 , [GUID] = ''
                              FROM DailySchedule
                              WHERE EXISTS (
				                            @SELECT#
					                            1 AS ExistReg
				                            FROM DailyCauses
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE DailyCauses.IDCause = " & Me.AutomaticEquivalenceIDCause & "
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailyCauses.Date = DailySchedule.Date
				                                AND DailyCauses.IDEmployee = DailySchedule.IDEmployee
                                           )"

                    bolRet = ExecuteSql(strSQL)

                    If bolRet AndAlso Me.ID > 0 Then
                        If Not oOldCause Is Nothing Then
                            If oOldCause.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType AndAlso oOldCause.intAutomaticEquivalenceIDCause > 0 Then
                                strSQL = "@DECLARE#  @date SMALLDATETIME = " & sqlFreezeDate & "

                                          @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                                        DailySchedule.Status = 0, [GUID] = ''
                                          FROM DailySchedule
                                          WHERE EXISTS (
				                                        @SELECT#
					                                        1 AS ExistReg
				                                        FROM DailyCauses
					                                        INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                                        WHERE IDCause = " & oOldCause.AutomaticEquivalenceIDCause & "
				                                            AND DailySchedule.Date > @date
				                                            AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                            AND DailyCauses.Date = DailySchedule.Date
				                                            AND DailyCauses.IDEmployee = DailySchedule.IDEmployee
			                                           )"

                                bolRet = ExecuteSql(strSQL)
                            End If
                        End If
                    End If
                End If

                If bolRet Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                              FROM DailySchedule
                              WHERE EXISTS (
				                            @SELECT#
					                            1 As ExistsReg
				                            FROM ProgrammedAbsences
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE ProgrammedAbsences.IDCause = " & Me.ID & "
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.IDEmployee = ProgrammedAbsences.IDEmployee
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailySchedule.Date BETWEEN ProgrammedAbsences.BeginDate
				                                AND ISNULL(ProgrammedAbsences.FinishDate, DATEADD(D, ProgrammedAbsences.maxlastingdays, ProgrammedAbsences.BeginDate))
			                               )"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                             @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                             FROM DailySchedule
                             WHERE EXISTS (
				                            @SELECT#
					                            1 As ExistsReg
				                            FROM ProgrammedCauses
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE ProgrammedCauses.IDCause = " & Me.ID & "
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.IDEmployee = ProgrammedCauses.IDEmployee
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailySchedule.Date BETWEEN ProgrammedCauses.Date AND ISNULL(ProgrammedCauses.FinishDate, GETDATE())
                                          )"

                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet Then
                    strSQL = "@DECLARE# @date SMALLDATETIME = " & sqlFreezeDate & "

                              @UPDATE# DailySchedule WITH (ROWLOCK) SET
	                            DailySchedule.Status = 0, [GUID] = ''
                              FROM DailySchedule
                              WHERE EXISTS (
				                            @SELECT#
					                            1 As ExistsReg
				                            FROM Punches
					                            INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee
				                            WHERE Punches.TypeData = " & Me.ID & "
				                                AND DailySchedule.Date > @date
				                                AND DailySchedule.IDEmployee = Punches.IDEmployee
				                                AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate
				                                AND DailySchedule.Date = Punches.ShiftDate
				                                AND Punches.ActualType IN (1,2)
			                             )"

                    bolRet = ExecuteSql(strSQL)
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::UpdateDailySchedule")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::UpdateDailySchedule")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function IsUsed() As Boolean
            Dim bolRet As Boolean = False

            Dim strErrorMessage As String

            ' Miramos si la causa esta siendo usada en algun horario
            strErrorMessage = CauseUsedInShift()
            If strErrorMessage <> "" Then
                oState.Language.ClearUserTokens()
                oState.Language.AddUserToken(strErrorMessage)
                oState.Result = CauseResultEnum.CauseUsedInShift
                oState.Language.ClearUserTokens()
                bolRet = True
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siedo usada en algun acumulado
                strErrorMessage = CauseUsedinAccrual()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInAccrual
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siedo usada en algun acumulado
                strErrorMessage = CauseUsedinCauses()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedinCauses
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siedo usada en algun definicion de equivalencia
                strErrorMessage = CauseUsedinEquivalenceData()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInEquivalenceData
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siedo usada en alguna ausencia prolongada
                strErrorMessage = CauseUsedInProgrammedAbsence()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInProgrammedAbsence
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siendo usada en alguna Incidencia Prevista
                strErrorMessage = CauseUsedInProgrammedCause()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInProgrammedCause
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siendo usada en alguna prevision de vacaciones por horas
                strErrorMessage = CauseUsedInProgrammedHoliday()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInProgrammedHoliday
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siendo usada en alguna prevision de horas de exceso
                strErrorMessage = CauseUsedInProgrammedOvertime()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInProgrammedOvertime
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa se usó en algún fichaje con incidencia por terminal
                strErrorMessage = CauseUsedInPunchWithIncidence()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInPunchWithIncidence
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa se usó en alguna solicitud
                If Me.CauseUsedInRequest() Then
                    oState.Result = CauseResultEnum.CauseUsedInRequest
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siendo usada en limites por justificacion
                strErrorMessage = CauseUsedinCauseLimits()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInAccrual
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                ' Miramos si la causa esta siendo usada en las reglas de solicitudes
                strErrorMessage = CauseUsedinRequestsRules()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInRequestRule
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If Not bolRet Then
                strErrorMessage = CauseUsedInLabAgree()
                If strErrorMessage <> "" Then
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strErrorMessage)
                    oState.Result = CauseResultEnum.CauseUsedInLabAgree
                    oState.Language.ClearUserTokens()
                    bolRet = True
                End If
            End If

            If bolRet Then
                If oState.ErrorText.Length > 1100 Then
                    oState.ErrorText = oState.ErrorText.Substring(0, 1097) & "..."
                End If
            End If

            Return bolRet

        End Function

        Public Function HasRecalculateChanges(ByVal oOldRow As DataRow) As Boolean

            Dim bHasRecalculateChanges As Boolean = False

            Try

                Dim oOldCause As roCause = New roCause(oOldRow("ID"), Me.oState, False)

                If Me.WorkingType <> oOldCause.WorkingType Then bHasRecalculateChanges = True

                If Me.RoundingType <> oOldCause.RoundingType Then bHasRecalculateChanges = True
                If Me.RoundingBy <> oOldCause.RoundingBy Then bHasRecalculateChanges = True
                If Me.RoundingByDailyScope <> oOldCause.RoundingByDailyScope Then bHasRecalculateChanges = True

                If Me.StartsProgrammedAbsence <> oOldCause.StartsProgrammedAbsence Then bHasRecalculateChanges = True
                If Me.ApplyAbsenceOnHolidays <> oOldCause.ApplyAbsenceOnHolidays Then bHasRecalculateChanges = True

                If Me.ApplyJustifyPeriod <> oOldCause.ApplyJustifyPeriod Then bHasRecalculateChanges = True
                If Me.JustifyPeriodStart <> oOldCause.JustifyPeriodStart Then bHasRecalculateChanges = True
                If Me.JustifyPeriodEnd <> oOldCause.JustifyPeriodEnd Then bHasRecalculateChanges = True
                If Me.JustifyPeriodType <> oOldCause.JustifyPeriodType Then bHasRecalculateChanges = True

                If Me.ExternalWork <> oOldCause.ExternalWork Then bHasRecalculateChanges = True

                If Me.AutomaticEquivalenceType <> oOldCause.AutomaticEquivalenceType Then
                    bHasRecalculateChanges = True
                End If

                If Me.AutomaticEquivalenceIDCause <> oOldCause.AutomaticEquivalenceIDCause Then
                    bHasRecalculateChanges = True
                End If

                If Me.oAutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType And oOldCause.oAutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType Then
                    If Me.oAutomaticEquivalenceCriteria.FactorValue <> oOldCause.oAutomaticEquivalenceCriteria.FactorValue Then
                        bHasRecalculateChanges = True
                    End If
                End If

                If Me.oAutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType And oOldCause.oAutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType Then
                    If Me.oAutomaticEquivalenceCriteria.UserField.FieldName <> oOldCause.oAutomaticEquivalenceCriteria.UserField.FieldName Then
                        bHasRecalculateChanges = True
                    End If
                End If

                If Me.MaxTimeToForecast <> oOldCause.MaxTimeToForecast Then
                    bHasRecalculateChanges = True
                End If
            Catch ex As Exception

            End Try

            Return bHasRecalculateChanges

        End Function

        Public Function HasRecalculateRequests(ByVal oOldRow As DataRow) As Boolean

            Dim bHasRecalculateChanges As Boolean = False

            Try

                Dim oOldCause As roCause = New roCause(oOldRow("ID"), Me.oState, False)

                If Me.IDCategory <> oOldCause.IDCategory Then bHasRecalculateChanges = True

                If Me.MinLevelOfAuthority <> oOldCause.MinLevelOfAuthority Then bHasRecalculateChanges = True
            Catch ex As Exception

            End Try

            Return bHasRecalculateChanges

        End Function

        Public Function ValidateCause() As Boolean

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing

            ' El nombre no puede estar en blanco
            If Me.Name = "" Then
                oState.Result = CauseResultEnum.InvalidName
                Return False
            End If

            ' El nombre corto no puede estar en blanco
            If Me.ShortName = "" Then
                oState.Result = CauseResultEnum.InvalidShortName
                Return False
            End If

            ' Comprueba intervalo de fechas
            If Me.JustifyPeriodStart.HasValue And Me.JustifyPeriodEnd.HasValue Then
                If Me.JustifyPeriodStart > Me.JustifyPeriodEnd Then
                    oState.Result = CauseResultEnum.InvalidJustifyPeriodDates
                    Return False
                End If
            End If

            ' Si hay documentos de seguimiento, la justificación es de tipo ausencia
            If Me.lstDocuments IsNot Nothing AndAlso Me.lstDocuments.Count > 0 AndAlso (Me.CustomType OrElse Me.DayType) Then
                oState.Result = CauseResultEnum.TypeDoesNotAllowDocumentTracking
                Return False
            End If

            ' El nombre no puede existir en la bdd para otra justificación
            strQuery = " @SELECT# * From Causes "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " And name = '" & Me.Name.Replace("'", "''") & "' "

            oDataTable = CreateDataTable(strQuery)
            If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = CauseResultEnum.NameAlreadyExist
                Return False
            End If

            ' El nombre corto no puede existir en la bdd para otra justificación
            strQuery = " @SELECT# * From Causes "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " AND ShortName = '" & Me.ShortName.Replace("'", "''") & "' "

            oDataTable = CreateDataTable(strQuery)
            If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = CauseResultEnum.ShortNameAlreadeyExist
                Return False
            End If

            ' RoundingBy no puede ser negativo
            If Me.RoundingBy <= 0 Then
                oState.Result = CauseResultEnum.InvalidRoundingBy
                Return False
            End If

            ' ReaderInputCode no puede ser negativo
            If Me.ReaderInputcode < 0 Or Me.ReaderInputcode > 255 Then
                oState.Result = CauseResultEnum.InvalidReaderInputCode
                Return False
            End If

            ' MaxProgrammedAbsenece no puede ser negativo
            If Me.MaxProgrammedAbsence < 0 Then
                oState.Result = CauseResultEnum.InvalidMaxProgrammedAbsence
                Return False
            End If

            'No puede existir El nombre no puede existir en la bdd para otra justificación
            If Me.ReaderInputcode > 0 Then
                strQuery = " @SELECT# * From Causes "
                strQuery = strQuery & " Where ReaderInputCode = " & Me.ReaderInputcode
                strQuery = strQuery & " And Id <> " & Me.ID

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then
                    ' Si la select me ha devuelto es que alguien tiene el mismo código de justificación
                    oState.Result = CauseResultEnum.ReaderInputCodeExistent
                    Return False
                End If
            End If

            If Me.VisibilityPermissions = 2 Then
                If Me.VisibilityCriteria.Count = 0 Then
                    Me.oState.Result = CauseResultEnum.UserFieldEmpty
                    Return False
                End If
            Else
                Me.VisibilityCriteria = Nothing
            End If

            If Me.InputPermissions = 2 Then
                If Me.InputCriteria.Count = 0 Then
                    Me.oState.Result = CauseResultEnum.UserFieldEmpty
                    Return False
                End If
            Else
                Me.InputCriteria = Nothing
            End If

            If Me.IDConceptBalance > 0 And Me.IsHoliday Then
                ' Validamos que el saldo sea anual o mensual y de tipo horas
                strQuery = " @SELECT# count(*) From Concepts "
                strQuery = strQuery & " Where ID = " & Me.IDConceptBalance
                strQuery = strQuery & " And (DefaultQuery = 'Y' or DefaultQuery = 'M') and IDType='H' "
                Dim Total As Long = ExecuteScalar(strQuery)
                If Total = 0 Then
                    ' Si la select no me ha devuelto es que el saldo no es anual
                    oState.Result = CauseResultEnum.ConceptBalanceAnnualQueryRequired
                    Return False
                End If
            End If

            ' Si es una modificacion de la justificacion
            If Me.ID <> -1 Then
                ' En el caso que haya cambiado de tipo vacaciones a otro tipo, revisamos que no se este haciendo servir en previsiones de vacaciones por horas
                Dim _Cause As New roCause(Me.ID, Me.State)
                If Not _Cause Is Nothing Then
                    If _Cause.IsHoliday And Not Me.IsHoliday Then
                        ' Buscamos dicha justificacion en las vacaciones por horas
                        strQuery = CauseUsedInProgrammedHoliday()
                        If strQuery.Length > 0 Then
                            oState.Result = CauseResultEnum.CauseUsedInProgrammedHoliday
                            Return False
                        End If
                    End If

                    Dim bolChangeType As Boolean = False
                    ' En el caso que se cambie de un tipo a otro (horas/dias/personalizado) y ya se este usando, no dejamos cambiar de tipo
                    If (_Cause.CustomType <> Me.CustomType) Then
                        bolChangeType = True
                    End If

                    If (_Cause.DayType <> Me.DayType) Then
                        bolChangeType = True
                    End If

                    If bolChangeType Then
                        If Me.IsUsed Then
                            oState.Result = CauseResultEnum.ChangeTypeNotAllowed
                            Return False
                        End If
                    End If
                End If

                ' No se puede modificar el tipo de la justificacion HORAS TEORICAS, NO JUSTIFICADO, HORAS TRABAJADAS
                If Me.ID = 4 Or Me.ID = 0 Or Me.ID = 1 Then
                    If Me.IsHoliday Or Me.CustomType Or Me.DayType Then
                        oState.Result = CauseResultEnum.CauseUsedInAccrual
                        Return False
                    End If
                End If

            End If

            ' Validamos equivalencias en caso de justificaciones de tipo dias
            If Me.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType Then
                If Me.AutomaticEquivalenceType <> eAutomaticEquivalenceType.ExpectedWorkingHoursType AndAlso Me.AutomaticEquivalenceCriteria Is Nothing Then
                    oState.Result = CauseResultEnum.AutomaticEquivalenceDataInvalid
                    Return False
                ElseIf Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType Then
                    If Me.AutomaticEquivalenceCriteria.FactorValue = 0 Then
                        oState.Result = CauseResultEnum.AutomaticEquivalenceDataInvalid
                        Return False
                    End If
                ElseIf Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType Then
                    If Me.AutomaticEquivalenceCriteria.UserField Is Nothing OrElse Me.AutomaticEquivalenceCriteria.UserField.FieldName.Length = 0 Then
                        oState.Result = CauseResultEnum.AutomaticEquivalenceDataInvalid
                        Return False
                    End If
                End If

                If Me.AutomaticEquivalenceIDCause = 0 Then
                    oState.Result = CauseResultEnum.AutomaticEquivalenceDataInvalid
                    Return False
                End If
            Else
                Me.AutomaticEquivalenceCriteria = New roAutomaticEquivalenceCriteria
                Me.AutomaticEquivalenceIDCause = 0
            End If

            'OBTENER LICENCIA DE EXPRES PARA COMPROBAR SI PUEDE REALIZAR LA ACCION QUE REQUIERE LA VALIDACION
            Try
                Dim oServerLicense As New roServerLicense
                If oServerLicense.FeatureIsInstalled("Version\LiveExpress") Then
                    strQuery = "@SELECT# Count(*) FROM Causes"
                    Dim Total As Long = ExecuteScalar(strQuery)
                    If Me.ID <= 0 Then
                        Total = Total + 1
                    End If
                    If Total > MAX_NUMBER_OF_CAUSES_IN_EXPRESS Then
                        oState.Result = CauseResultEnum.NumberOfCausesExceeded
                        Return False
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::ValidateCause")
            End Try

            Return True

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim strQuery As String

            If Me.ID = 4 Or Me.ID = 0 Or Me.ID = 1 Then
                ' No se puede borrar la justificacion de horas teoricas ni NO Justificado, ni horas trabajadas
                oState.Result = CauseResultEnum.CauseUsedInAccrual
                bolRet = False
                Return bolRet
                Exit Function
            End If

            If Not Me.IsUsed() Then

                Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                strQuery = "@DELETE# DailyCauses WHERE IDCause = " & Me.ID
                Try
                    If Not ExecuteSql(strQuery) Then
                        oState.Result = CauseResultEnum.ConnectionError
                        oState.ErrorText = ""
                        bolRet = False
                    Else
                        bolRet = True
                    End If
                Catch ex As System.Data.Common.DbException
                    oState.UpdateStateInfo(ex, "roCause::Delete")
                    bolRet = False
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roCause::Delete")
                    bolRet = False
                End Try

                If bolRet Then

                    'Borramos las relaciones entre la justificacion y las incidencias diarias que la usan
                    strQuery = "@DELETE# ConceptCauses WHERE IDCause = " & Me.ID
                    Try
                        If Not ExecuteSql(strQuery) Then
                            oState.Result = CauseResultEnum.ConnectionError
                            oState.ErrorText = ""
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    Catch ex As Data.Common.DbException
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    End Try

                End If

                If bolRet Then

                    ' BORRO la asociacion de documentos
                    strQuery = "@DELETE# CausesDocuments WHERE IDCause = " & Me.ID
                    Try
                        If Not ExecuteSql(strQuery) Then
                            oState.Result = CauseResultEnum.ConnectionError
                            oState.ErrorText = ""
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    Catch ex As Data.Common.DbException
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    End Try

                End If

                If bolRet Then

                    ' BORRO la justificacion
                    strQuery = "@DELETE# Causes WHERE ID = " & Me.ID
                    Try
                        If Not ExecuteSql(strQuery) Then
                            oState.Result = CauseResultEnum.ConnectionError
                            oState.ErrorText = ""
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    Catch ex As Data.Common.DbException
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Delete")
                        bolRet = False
                    End Try

                End If

                ' ESPECIAL: TorrasPapel
                If bolRet Then
                    Try
                        Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                        If roTypes.Any2String(customization) = "SARROT" Then
                            ' Marco para envío a Meta4. Sea de cuando sea la ausencia, marco el día de hoy para que se envíe ya. De todos modos, se envían todas desde cero ...
                            strQuery = "@UPDATE# CAUSES SET SEND=0"
                            ExecuteSql(strQuery)
                        End If
                    Catch ex As Exception
                    End Try
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCause, Me.strName, Nothing, -1)
                End If

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            Else
                bolRet = False
            End If

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.Delete.ToString)

                    roConnector.InitTask(TasksType.CAUSES, oParamsAux)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCause::Delete::Could not send cache update")
            End Try

            Return bolRet

        End Function

        Public Function Causes(Optional ByVal strWhere As String = "", Optional ByVal bolBusinessGroupFilter As Boolean = False) As DataTable

            Dim tbCauses As Data.DataTable = Nothing
            Dim strQuery As String

            Dim strBusiness As String = roCause.GetBusinessGroupList(Me.oState)

            strQuery = " @SELECT# * FROM Causes "
            If strWhere <> "" Then
                strQuery &= "WHERE " & strWhere
                If bolBusinessGroupFilter Then
                    If strBusiness <> String.Empty Then
                        strQuery &= " AND (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                    End If
                End If
            Else
                If bolBusinessGroupFilter Then
                    If strBusiness <> String.Empty Then
                        strQuery &= " WHERE (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                    End If
                End If
            End If
            strQuery &= " ORDER BY Name"

            Try
                tbCauses = CreateDataTable(strQuery)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::Causes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::Causes")
            End Try

            Return tbCauses

        End Function

        Public Function CausesShortList(Optional ByVal bolBusinessGroupFilter As Boolean = False) As DataTable

            Dim tbCauses As Data.DataTable = Nothing
            Dim strQuery As String

            strQuery = "@SELECT# ID, Name, IsHoliday, WorkingType, ExternalWork, DayType, CustomType from Causes"

            Dim strBusiness As String = roCause.GetBusinessGroupList(Me.oState)
            If bolBusinessGroupFilter Then
                If strBusiness <> String.Empty Then
                    strQuery &= " WHERE (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                End If
            End If
            strQuery &= " ORDER BY Name"
            Try
                tbCauses = CreateDataTable(strQuery)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CausesShortList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CausesShortList")
            End Try

            Return tbCauses

        End Function

        Public Function CausesShortListByRequestType(ByVal eRequestType As eCauseRequest, Optional ByVal bolBusinessGroupFilter As Boolean = False) As DataTable

            Dim tbCauses As Data.DataTable = Nothing
            Dim strQuery As String

            strQuery = "@SELECT# ID, Name, IsHoliday, WorkingType, ExternalWork, DayType, CustomType from Causes WHERE 1=1 "

            Dim strBusiness As String = roCause.GetBusinessGroupList(Me.oState)
            If bolBusinessGroupFilter Then
                If strBusiness <> String.Empty Then
                    strQuery &= " AND (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                End If
            End If

            Dim strFilter As String = String.Empty

            Select Case eRequestType
                Case eCauseRequest.All
                    strFilter = " "
                Case eCauseRequest.ExternalWork
                    strFilter = " AND IsHoliday = 0 AND (WorkingType = 1 OR ExternalWork = 1) AND DayType = 0 AND CustomType=0 AND RequestAvailability <> '-2' AND (RequestAvailability = '-1' OR RequestAvailability = '4' OR RequestAvailability like '4,%' OR RequestAvailability like '%,4')"
                Case eCauseRequest.ProgrammedAbsence
                    strFilter = " AND IsHoliday = 0 AND WorkingType = 0 AND ExternalWork = 0 AND DayType = 0 AND CustomType=0 AND RequestAvailability <> '-2' AND (RequestAvailability = '-1' OR RequestAvailability like '%7%')"
                Case eCauseRequest.ProgrammedCause
                    strFilter = " AND IsHoliday = 0 AND WorkingType = 0 AND ExternalWork = 0 AND DayType = 0 AND CustomType=0 AND RequestAvailability <> '-2' AND (RequestAvailability = '-1' OR RequestAvailability like '%9%')"
                Case eCauseRequest.ProgrammedOvertime
                    strFilter = " AND IsHoliday = 0 AND WorkingType = 1 AND DayType = 0 AND CustomType=0 AND RequestAvailability <> '-2' AND (RequestAvailability = '-1' OR RequestAvailability like '%14%')"
                Case eCauseRequest.PlannedHolidays
                    strFilter = " AND IsHoliday = 1 AND WorkingType = 0 AND ExternalWork = 0 AND DayType = 0 AND CustomType=0 AND RequestAvailability <> '-2' AND (RequestAvailability = '-1' OR RequestAvailability like '%13%')"
                Case eCauseRequest.Leaves
                    strQuery = "@SELECT#  distinct Causes.ID, Causes.Name, Causes.IsHoliday, Causes.WorkingType, Causes.ExternalWork, Causes.DayType, Causes.CustomType FROM Causes " &
                                    " inner join CausesDocumentsTracking on Causes.ID = CausesDocumentsTracking.IDCause " &
                                    " inner join DocumentTemplates on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id " &
                                    " WHERE causes.ID <> 0 and DocumentTemplates.Scope = 3 AND RequestAvailability <> '-2' "
            End Select

            strQuery = strQuery & strFilter

            strQuery &= " ORDER BY Name"
            Try
                tbCauses = CreateDataTable(strQuery)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CausesShortList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CausesShortList")
            End Try

            Return tbCauses

        End Function

        Public Function CauseUsedInPunchWithIncidence() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Employees.ID, Employees.Name FROM Employees, Punches"
            strQuery = strQuery & " WHERE Employees.ID = Punches.IDEmployee"
            strQuery = strQuery & " AND (ActualType IN(1,2)) AND (Punches.TypeData = " & Me.ID & ")"
            strQuery = strQuery & " GROUP BY Employees.ID, Employees.Name"

            Try
                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInPunchWithIncidence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInPunchWithIncidence")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedWithEquivalenceCause() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing

            Try

                If Me.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType AndAlso Me.intAutomaticEquivalenceIDCause > 0 Then

                    strQuery = "@SELECT# count(*) AS Count FROM DailyCauses"
                    strQuery = strQuery & " Where DailyCauses.IDCause = " & Me.intAutomaticEquivalenceIDCause

                    Dim oCount As Long = roTypes.Any2Long(ExecuteScalar(strQuery))
                    If oCount > 0 Then strRet = oCount.ToString()

                End If

                If strRet = String.Empty AndAlso Me.ID > 0 Then
                    Dim oOldCause As roCause = New roCause(Me.ID, Me.oState, False)
                    If Not oOldCause Is Nothing Then
                        If oOldCause.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType AndAlso oOldCause.intAutomaticEquivalenceIDCause > 0 Then
                            strQuery = "@SELECT# count(*) AS Count FROM DailyCauses"
                            strQuery = strQuery & " Where DailyCauses.IDCause = " & oOldCause.intAutomaticEquivalenceIDCause

                            Dim oCount As Long = roTypes.Any2Long(ExecuteScalar(strQuery))
                            If oCount > 0 Then strRet = oCount.ToString()
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedWithEquivalenceCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedWithEquivalenceCause")
            End Try

            Return strRet
        End Function

        Public Function CauseUsedInProgrammedCause() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Employees.ID, Employees.Name FROM Employees, ProgrammedCauses"
            strQuery = strQuery & " WHERE Employees.ID = ProgrammedCauses.IDEmployee"
            strQuery = strQuery & " AND ProgrammedCauses.IDCause = " & Me.ID
            strQuery = strQuery & " GROUP BY Employees.ID, Employees.Name"

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedCause")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedInProgrammedOvertime() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Employees.ID, Employees.Name FROM Employees, ProgrammedOvertimes"
            strQuery = strQuery & " WHERE Employees.ID = ProgrammedOvertimes.IDEmployee"
            strQuery = strQuery & " AND ProgrammedOvertimes.IDCause = " & Me.ID
            strQuery = strQuery & " GROUP BY Employees.ID, Employees.Name"

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedOvertime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedOvertime")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedInProgrammedHoliday() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Employees.ID, Employees.Name FROM Employees, ProgrammedHolidays"
            strQuery = strQuery & " WHERE Employees.ID = ProgrammedHolidays.IDEmployee"
            strQuery = strQuery & " AND ProgrammedHolidays.IDCause = " & Me.ID
            strQuery = strQuery & " GROUP BY Employees.ID, Employees.Name"

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedHoliday")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedHoliday")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedInProgrammedAbsence() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Employees.ID, Employees.Name FROM Employees, ProgrammedAbsences"
            strQuery = strQuery & " WHERE Employees.ID = ProgrammedAbsences.IDEmployee"
            strQuery = strQuery & " AND ProgrammedAbsences.IDCause = " & Me.ID
            strQuery = strQuery & " GROUP BY Employees.ID, Employees.Name"

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInProgrammedAbsence")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedinCauses() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing

            strQuery = "@SELECT# count(*) AS Count FROM DailyCauses"
            strQuery = strQuery & " Where DailyCauses.IDCause = " & Me.ID

            Try

                Dim oCount As Long = roTypes.Any2Long(ExecuteScalar(strQuery))
                If oCount > 0 Then strRet = oCount.ToString()
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinAccrual")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinAccrual")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedinEquivalenceData() As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing

            strQuery = "@SELECT# count(*) AS Count FROM Causes"
            strQuery = strQuery & " Where AutomaticEquivalenceIDCause = " & Me.ID

            Try

                Dim oCount As Long = roTypes.Any2Long(ExecuteScalar(strQuery))
                If oCount > 0 Then strRet = oCount.ToString()
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinEquivalenceData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinEquivalenceData")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedinAccrual(Optional ByRef strIds As String = "") As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Concepts.ID, Concepts.Name FROM ConceptCauses"
            strQuery = strQuery & " INNER JOIN Concepts ON ConceptCauses.IDConcept = Concepts.ID"
            strQuery = strQuery & " Where ConceptCauses.IDCause = " & Me.ID

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                            strIds = strIds & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                        strIds = strIds & DataRowView("ID")
                    Next
                End If

                strQuery = "@SELECT# Concepts.ID, Concepts.Name FROM Concepts "
                strQuery = strQuery & " Where (Concepts.AutomaticAccrualIDCause=" & Me.ID & " OR Concepts.ExpiredIDCause=" & Me.ID & ") "
                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                            strIds = strIds & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                        strIds = strIds & DataRowView("ID")
                    Next
                End If

                strQuery = "@SELECT# Concepts.ID, Concepts.Name FROM Concepts "
                strQuery = strQuery & " Where AutomaticAccrualType <> " & eAutomaticAccrualType.DeactivatedType
                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)
                    For Each DataRowView In oDataView
                        Dim oConcept As New Concept.roConcept(DataRowView("ID"), New Concept.roConceptState(-1))
                        If oConcept IsNot Nothing Then
                            If oConcept.AutomaticAccrualCriteria IsNot Nothing AndAlso oConcept.AutomaticAccrualCriteria.Causes.Contains(Me.ID) Then
                                If strRet <> String.Empty Then
                                    strRet = strRet & ","
                                    strIds = strIds & ","
                                End If
                                strRet = strRet & DataRowView("Name")
                                strIds = strIds & DataRowView("ID")
                            End If
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinAccrual")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinAccrual")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedinCauseLimits(Optional ByRef strIds As String = "") As String

            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# Causes.ID, Causes.Name FROM CauseLimitValues"
            strQuery = strQuery & " INNER JOIN Causes ON CauseLimitValues.IDCause = Causes.ID"
            strQuery = strQuery & " Where CauseLimitValues.IDCause = " & Me.ID

            Try

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                            strIds = strIds & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                        strIds = strIds & DataRowView("ID")
                    Next
                End If

                strQuery = "@SELECT# Causes.ID, Causes.Name FROM CauseLimitValues"
                strQuery = strQuery & " INNER JOIN Causes ON CauseLimitValues.IDExcessCause = Causes.ID"
                strQuery = strQuery & " Where CauseLimitValues.IDExcessCause = " & Me.ID

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                            strIds = strIds & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                        strIds = strIds & DataRowView("ID")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinCauseLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinCauseLimits")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedinRequestsRules(Optional ByRef strIds As String = "") As String

            Dim strRet As String = ""

            Dim oDataTable As DataTable = Nothing

            Try
                ' Obtenemos las reglas del mismo convenio
                Dim LabAgreeRequestRules As Generic.List(Of LabAgree.roRequestRule) = LabAgree.roRequestRule.GetRequestsRules("IDRequestType IN(7,13)", New LabAgree.roLabAgreeState(-1), False)
                If LabAgreeRequestRules IsNot Nothing AndAlso LabAgreeRequestRules.Count > 0 Then
                    For Each oReqRule As LabAgree.roRequestRule In LabAgreeRequestRules
                        If oReqRule.Definition IsNot Nothing AndAlso oReqRule.Definition.IDReasons.Count > 0 Then
                            If oReqRule.Definition.IDReasons.FindAll(Function(o) oReqRule.Definition.IDReasons.Contains(Me.ID)).Count > 0 Then
                                If strRet <> String.Empty Then
                                    strRet = strRet & ","
                                    strIds = strIds & ","
                                End If
                                strRet = strRet & oReqRule.Name
                                strIds = strIds & oReqRule.IDRule
                            End If
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinRequestsRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedinRequestsRules")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedInShift(Optional ByRef strIds As String = "") As String
            ' Funcion que devuelve los horarios que usan una causa
            Dim strRet As String = ""

            Dim strQuery As String
            Dim oDataTable As DataTable = Nothing

            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            Try
                strQuery = "@SELECT# Shifts.Name, sysroShiftsCausesRules.Definition, Shifts.ID"
                strQuery = strQuery & " FROM Shifts INNER JOIN sysroShiftsCausesRules ON Shifts.ID = sysroShiftsCausesRules.IDShift"
                strQuery = strQuery & " Where Definition like '%key=""Cause"" type=""%%"">" & Me.ID & "</Item>%'"

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                            strIds = strIds & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                        strIds = strIds & DataRowView("ID")
                    Next
                End If

                ' Obtenemos los horaros que tienen reglas diarias que usen dicha justificacion
                strQuery = "@SELECT# distinct Shifts.Name,  Shifts.ID"
                strQuery = strQuery & " FROM Shifts INNER JOIN sysroShiftsCausesRules ON Shifts.ID = sysroShiftsCausesRules.IDShift"
                strQuery = strQuery & " Where RuleType=3"

                oDataTable = CreateDataTable(strQuery)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    Dim oShiftState As New Shift.roShiftState(-1)
                    Dim mExistCause As Boolean = False
                    For Each DataRowView In oDataView
                        mExistCause = False
                        Dim oShiftDailyRules As Generic.List(Of roShiftDailyRule) = Shift.roShiftRule.GetDailyShiftRules(roTypes.Any2Integer(DataRowView("ID")), oShiftState)
                        If oShiftDailyRules IsNot Nothing AndAlso oShiftDailyRules.Count > 0 Then
                            For Each oDailyRule As roShiftDailyRule In oShiftDailyRules
                                If oDailyRule.Conditions IsNot Nothing AndAlso oDailyRule.Conditions.Count > 0 Then
                                    For Each oShiftDailyRuleCondition As roShiftDailyRuleCondition In oDailyRule.Conditions
                                        If oShiftDailyRuleCondition.CompareCauses IsNot Nothing AndAlso oShiftDailyRuleCondition.CompareCauses.Count > 0 Then
                                            For Each oShiftDailyRuleConditionCause As roShiftDailyRuleConditionCause In oShiftDailyRuleCondition.CompareCauses
                                                If oShiftDailyRuleConditionCause.IDCause = Me.ID Then mExistCause = True
                                            Next
                                        End If

                                        If oShiftDailyRuleCondition.ConditionCauses IsNot Nothing AndAlso oShiftDailyRuleCondition.ConditionCauses.Count > 0 Then
                                            For Each oShiftDailyRuleConditionCause As roShiftDailyRuleConditionCause In oShiftDailyRuleCondition.ConditionCauses
                                                If oShiftDailyRuleConditionCause.IDCause = Me.ID Then mExistCause = True
                                            Next
                                        End If
                                    Next
                                End If

                                If oDailyRule.Actions IsNot Nothing AndAlso oDailyRule.Actions.Count > 0 Then
                                    For Each oShiftDailyRuleAction As roShiftDailyRuleAction In oDailyRule.Actions
                                        If oShiftDailyRuleAction.Action = RuleAction.CarryOver Then
                                            If oShiftDailyRuleAction.CarryOverIDCauseFrom = Me.ID Then mExistCause = True
                                            If oShiftDailyRuleAction.CarryOverIDCauseTo = Me.ID Then mExistCause = True
                                        Else
                                            If oShiftDailyRuleAction.PlusIDCause = Me.ID Then mExistCause = True
                                        End If
                                        If oShiftDailyRuleAction.Action = RuleAction.CarryOverSingle Then
                                            If roTypes.Any2Integer(oShiftDailyRuleAction.CarryOverSingleCause) = Me.ID Then mExistCause = True
                                            For Each mCause In oShiftDailyRuleAction.ActionCauses
                                                If mCause.IDCause = Me.ID Then mExistCause = True
                                                If mCause.IDCause2 = Me.ID Then mExistCause = True
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                        End If

                        If mExistCause Then
                            If strRet <> String.Empty Then
                                strRet = strRet & ","
                                strIds = strIds & ","
                            End If

                            strRet = strRet & DataRowView("Name")
                            strIds = strIds & DataRowView("ID")
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInShift")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInShift")
            End Try

            Return strRet

        End Function

        Public Function CauseUsedInRequest() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# COUNT(*) FROM Requests WHERE IDCause = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    bolRet = (roTypes.Any2Integer(tb.Rows(0).Item(0)) > 0)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInRequest")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInRequest")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetNextIDCause() As Integer
            ' Recupera el siguiente codigo de employee a usar

            Dim strQuery As String
            Dim intNextID As Integer

            intNextID = -1

            strQuery = " @SELECT# Max(ID) as Contador From Causes "

            Try
                Dim tb As DataTable = CreateDataTable(strQuery)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    intNextID = roTypes.Any2Integer(tb.Rows(0).Item(0)) + 1
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::GetNextIDCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::GetNextIDCause")
            End Try

            Return intNextID

        End Function

        Public Function CauseUsedInLabAgree() As String
            Dim oDataTable As DataTable = Nothing
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView
            Dim strRet As String = ""

            Dim strSQL As String = "@SELECT# Name FROM LabAgree WHERE" &
                    "(',' + RTRIM(ExtraHoursIDCauseSimples) + ',') LIKE '%,' + CAST(" + Me.ID.ToString + " as varchar) + ',%' OR" &
                    "(',' + RTRIM(ExtraHoursIDCauseDoubles) + ',') LIKE '%,' + CAST(" + Me.ID.ToString + " as varchar) + ',%' OR" &
                    "(',' + RTRIM(ExtraHoursIDCauseTriples) + ',') LIKE '%,' + CAST(" + Me.ID.ToString + " as varchar) + ',%'"
            Try
                oDataTable = CreateDataTable(strSQL)
                If oDataTable IsNot Nothing Then
                    oDataView = New Data.DataView(oDataTable)

                    For Each DataRowView In oDataView
                        If strRet <> String.Empty Then
                            strRet = strRet & ","
                        End If
                        strRet = strRet & DataRowView("Name")
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInLabAgree")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInLabAgree")
            Finally

            End Try

            Return strRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un datatable con los BusinessGroup de los grupos de horarios
        ''' </summary>
        Public Shared Function GetBusinessGroupFromCauseGroups(ByRef oState As roCauseState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroup FROM Causes GROUP BY BusinessGroup HAVING (BusinessGroup <> '')"
                oRet = CreateDataSet(strQuery)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::GetBusinessGroupFromCauseGroups")
            End Try

            Return oRet

        End Function

        Private Shared Function GetBusinessGroupList(ByVal oState As roCauseState) As String
            Dim strRet As String = String.Empty
            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ID IN(@SELECT# isnull(IDGroupFeature,-1) from sysroPassports WHERE id = " & oState.IDPassport & " ) "
                Dim dtBusinessGroups As System.Data.DataTable = CreateDataTable(strQuery)
                If dtBusinessGroups IsNot Nothing AndAlso dtBusinessGroups.Rows.Count > 0 Then

                    Dim arrAux() As String = roTypes.Any2String(dtBusinessGroups.Rows(0)("BusinessGroupList")).Split(";")
                    For Each item As String In arrAux
                        If item.Trim() <> String.Empty Then
                            strRet &= "'" & item.Trim().Replace("'", "''") & "',"
                        End If
                    Next
                    If strRet.Length > 0 Then strRet = strRet.Substring(0, strRet.Length() - 1)

                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::GetBusinessGroupList")
            End Try

            Return strRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de justificaciones a las que tiene acceso de fichaje (pestaña fichajes de la pantalla de definición de justificaciones) el empleado indicado.
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCausesByEmployeeInputPermissions(ByVal intIDEmployee As Integer, ByVal _State As roCauseState, Optional ByVal bolBusinessGroupFilter As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# * from ( " &
                                        "@SELECT# Causes.*, ROW_NUMBER() OVER(PARTITION BY causes.id ORDER BY causes.id DESC) AS rownum " &
                                        "From Causes " &
                                        "Left outer join CausesDocumentsTracking on Causes.ID = CausesDocumentsTracking.IDCause  " &
                                        "Left outer join DocumentTemplates on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id  " &
                                        "WHERE InputPermissions In (0, 2)  " &
                                        "And causes.ID <> 0  " &
                                        "And (isnull(DocumentTemplates.Scope,-1) <>3) "
                Dim strBusiness As String = roCause.GetBusinessGroupList(_State)
                If bolBusinessGroupFilter Then
                    If strBusiness <> String.Empty Then
                        strSQL &= " AND (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                    End If
                End If
                strSQL &= ") aux " &
                          "where aux.rownum = 1 " &
                          "order by aux.Name  "

                oRet = CreateDataTable(strSQL, )
                For Each oRow As DataRow In oRet.Rows

                    bolCausePermission = True

                    If oRow("InputPermissions") = 2 Then

                        oCause = New roCause(oRow("ID"), _State, False)
                        If oCause.InputCriteria IsNot Nothing AndAlso oCause.InputCriteria.Count > 0 Then
                            Dim strSQLFilter As String = ""
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oCause.InputCriteria
                                strSQLFilter = oCondition.GetFilter(intIDEmployee)
                                If strSQLFilter <> String.Empty Then
                                    strSQLFilter = " AND " & strSQLFilter
                                End If
                            Next
                            If strSQLFilter <> String.Empty Then
                                strSQLFilter = "Employees.ID = " & intIDEmployee.ToString & strSQLFilter
                                Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", _State)
                                bolCausePermission = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                            End If
                        End If
                    End If

                    If Not bolCausePermission Then oRow.Delete()

                Next

                oRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeCausesByInputPermissions(ByVal _State As roCauseState,
                                                                   Optional ByVal bolBusinessGroupFilter As Boolean = False, Optional lEmployees As List(Of Integer) = Nothing) As Dictionary(Of Integer, List(Of String))
            Dim dEmployeeAllowedCauses As Dictionary(Of Integer, List(Of String)) = New Dictionary(Of Integer, List(Of String))

            Try

                ' 1.- Justificaciones que son visibles por empleado, sea porque las ven todos los empleados, o por que dependen de una condición sobre campo de la ficha
                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# * from ( " &
                                        "@SELECT# Causes.*, ROW_NUMBER() OVER(PARTITION BY causes.id ORDER BY causes.id DESC) AS rownum " &
                                        "From Causes " &
                                        "Left outer join CausesDocumentsTracking on Causes.ID = CausesDocumentsTracking.IDCause  " &
                                        "Left outer join DocumentTemplates on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id  " &
                                        "WHERE InputPermissions In (0, 2)  " &
                                        "And causes.ID <> 0  " &
                                        "And (isnull(DocumentTemplates.Scope,-1) <>3) "
                Dim strBusiness As String = roCause.GetBusinessGroupList(_State)
                If bolBusinessGroupFilter Then
                    If strBusiness <> String.Empty Then
                        strSQL &= " AND (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                    End If
                End If
                strSQL &= ") aux " &
                          "where aux.rownum = 1 " &
                          "order by aux.Name  "

                Dim tCauses As DataTable
                Dim tbCauseEmployees As DataTable
                tCauses = CreateDataTable(strSQL)
                For Each oCauseRow As DataRow In tCauses.Rows
                    oCause = New roCause(oCauseRow("ID"), _State, False)
                    If oCauseRow("InputPermissions") = 2 Then

                        If oCause.InputCriteria IsNot Nothing AndAlso oCause.InputCriteria.Count > 0 Then
                            Dim strSQLFilter As String = ""
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oCause.InputCriteria
                                strSQLFilter = oCondition.GetFilter(-1)
                                If strSQLFilter <> String.Empty Then
                                    strSQLFilter = " AND " & strSQLFilter
                                End If
                            Next

                            If strSQLFilter.StartsWith(" AND") Then strSQLFilter = strSQLFilter.Remove(0, 4)

                            If strSQLFilter <> String.Empty Then
                                tbCauseEmployees = roBusinessSupport.GetEmployees(strSQLFilter, "", "", _State)
                                If Not tbCauseEmployees Is Nothing AndAlso tbCauseEmployees.Rows.Count > 0 Then
                                    'Recorro todos los empleados que tienen visibildiad de esta justificación y los añado al diccionario resultante
                                    Dim iIDEmployee As Integer
                                    For Each oEmployeeRow As DataRow In tbCauseEmployees.Rows
                                        iIDEmployee = roTypes.Any2Integer(oEmployeeRow("IDEmployee"))
                                        If Not dEmployeeAllowedCauses.ContainsKey(iIDEmployee) Then
                                            dEmployeeAllowedCauses.Add(iIDEmployee, New List(Of String)())
                                        End If
                                        dEmployeeAllowedCauses(iIDEmployee).Add(roTypes.Any2String(oCause.ID))
                                    Next
                                End If
                            End If
                        End If
                    ElseIf oCauseRow("InputPermissions") = 0 Then
                        For Each iIDEmployee As Integer In lEmployees
                            If Not dEmployeeAllowedCauses.ContainsKey(iIDEmployee) Then
                                dEmployeeAllowedCauses.Add(iIDEmployee, New List(Of String)())
                            End If
                            dEmployeeAllowedCauses(iIDEmployee).Add(roTypes.Any2String(oCause.ID))
                        Next
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetEmployeeCausesByInputPermissions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetEmployeeCausesByInputPermissions")
            Finally

            End Try

            Return dEmployeeAllowedCauses

        End Function

        ''' <summary>
        ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLeaveCausesByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal _State As roCauseState) As DataTable
            Return GetCausesForRequestByEmployee(intIDEmployee, eCauseRequest.Leaves, _State)
        End Function

        ''' <summary>
        ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCausesByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal _State As roCauseState) As DataTable
            Return GetCausesForRequestByEmployee(intIDEmployee, eCauseRequest.All, _State)
        End Function

        ''' <summary>
        ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
        ''' </summary>
        ''' <param name="intIDEmployee">Código del empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCausesForRequestByEmployee(ByVal intIDEmployee As Integer, ByVal eRequestType As eCauseRequest, ByVal _State As roCauseState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True
                Dim bolIsLeaveCause As Boolean = False

                Dim idCurrentLabAgree As Integer = 0

                Dim strSQL As String = "@SELECT# * from ( " &
                                        "@SELECT# Causes.*, ROW_NUMBER() OVER(PARTITION BY causes.id ORDER BY causes.id DESC) AS rownum " &
                                        "From Causes  " &
                                        "left outer join CausesDocumentsTracking on Causes.ID = CausesDocumentsTracking.IDCause   " &
                                        "left outer join DocumentTemplates on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id   " &
                                        "WHERE VisibilityPermissions In (0, 2)   " &
                                        "And causes.ID <> 0   " &
                                        "And (isnull(DocumentTemplates.Scope,-1) <>3) " &
                                        ") aux " &
                                        "where aux.rownum=1 "

                Dim strFilter As String = String.Empty

                Select Case eRequestType
                    Case eCauseRequest.All
                        strFilter = " "
                    Case eCauseRequest.ExternalWork
                        strFilter = " AND aux.IsHoliday = 0 AND (aux.WorkingType = 1 OR aux.ExternalWork = 1) AND aux.DayType = 0 AND aux.CustomType=0 AND (aux.RequestAvailability = '-1' OR aux.RequestAvailability = '4' OR aux.RequestAvailability like '4,%' OR aux.RequestAvailability like '%,4')"
                    Case eCauseRequest.ProgrammedAbsence
                        strFilter = " AND aux.IsHoliday = 0 AND aux.WorkingType = 0 AND aux.ExternalWork = 0 AND aux.DayType = 0 AND aux.CustomType=0 AND (aux.RequestAvailability = '-1' OR aux.RequestAvailability like '%7%')"
                    Case eCauseRequest.ProgrammedCause
                        strFilter = " AND aux.IsHoliday = 0 AND aux.WorkingType = 0 AND aux.ExternalWork = 0 AND aux.DayType = 0 AND aux.CustomType=0 AND (aux.RequestAvailability = '-1' OR aux.RequestAvailability like '%9%')"
                    Case eCauseRequest.ProgrammedOvertime
                        strFilter = " AND aux.IsHoliday = 0 AND aux.WorkingType = 1 AND aux.DayType = 0 AND aux.CustomType=0 AND (aux.RequestAvailability = '-1' OR aux.RequestAvailability like '%14%')"
                    Case eCauseRequest.PlannedHolidays
                        strFilter = " AND aux.IsHoliday = 1 AND aux.WorkingType = 0 AND aux.ExternalWork = 0 AND aux.DayType = 0 AND aux.CustomType=0 AND (aux.RequestAvailability = '-1' OR aux.RequestAvailability like '%13%')"
                    Case eCauseRequest.Leaves
                        strSQL = "@SELECT#  distinct Causes.Id,Causes.Name,Causes.VisibilityPermissions FROM Causes " &
                                    " inner join CausesDocumentsTracking on Causes.ID = CausesDocumentsTracking.IDCause " &
                                    " inner join DocumentTemplates on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id " &
                                    " WHERE VisibilityPermissions IN (0, 2) AND causes.ID <> 0 and DocumentTemplates.Scope = 3 "
                End Select

                strSQL = strSQL & strFilter

                oRet = CreateDataTable(strSQL, )

                For Each oRow As DataRow In oRet.Rows

                    bolCausePermission = True

                    If oRow("VisibilityPermissions") = 2 Then

                        oCause = New roCause(oRow("ID"), _State, False)
                        If oCause.VisibilityCriteria IsNot Nothing AndAlso oCause.VisibilityCriteria.Count > 0 Then
                            Dim strSQLFilter As String = ""
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oCause.VisibilityCriteria
                                strSQLFilter = oCondition.GetFilter(intIDEmployee)
                                If strSQLFilter <> String.Empty Then
                                    strSQLFilter = " AND " & strSQLFilter
                                End If
                            Next
                            If strSQLFilter <> String.Empty Then
                                strSQLFilter = "Employees.ID  = " & intIDEmployee.ToString & strSQLFilter
                                Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", _State)
                                bolCausePermission = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                            End If
                        End If
                    End If

                    If Not bolCausePermission Then oRow.Delete()

                Next

                oRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeVisibilityPermissions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeVisibilityPermissions")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el id de la justificación asociada al nombre solicitado
        ''' </summary>
        ''' <param name="strCauseName">Nombre de la justificación</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCauseByName(ByVal strCauseName As String, ByVal _State As roCauseState) As Integer

            Dim oRet As Integer = Nothing

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# TOP 1 ID FROM Causes WHERE Name = '" & strCauseName & "'"
                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCauseNameByID(id As Integer) As String
            Dim oRet As String = "?"
            Try
                Dim strSQL As String = "@SELECT# Name FROM Causes WHERE ID = " & id.ToString
                oRet = roTypes.Any2String(ExecuteScalar(strSQL))
            Catch ex As Exception
            End Try
            Return oRet
        End Function

        Public Shared Function GetCauseByShortName(ByVal strCauseShortName As String, ByVal _State As roCauseState) As Integer

            Dim oRet As Integer = -1

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# TOP 1 ID FROM Causes WHERE ShortName = '" & strCauseShortName & "'"
                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCausesByEmployeeInputPermissions")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCauseByAdvancedCode(ByVal advancedCode As String, ByVal _State As roCauseState) As Integer
            Dim oRet As Integer = -1

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# TOP 1 ID FROM Causes WHERE Description like '%ID=" & advancedCode & ";%'"
                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCauseByInputCode")
                oRet = "0"
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCauseByInputCode")
                oRet = "0"
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetCauseByInputCode(ByVal inputCode As String, ByVal _State As roCauseState) As Integer

            Dim oRet As Integer = -1

            Try

                Dim oCause As roCause = Nothing
                Dim bolCausePermission As Boolean = True

                Dim strSQL As String = "@SELECT# TOP 1 ID FROM Causes WHERE ReaderInputCode = " & inputCode
                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetCauseByInputCode")
                oRet = "0"
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetCauseByInputCode")
                oRet = "0"
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCausesListWithData(ByVal _State As roCauseState, Optional ByVal bAudit As Boolean = False) As roCollection

            Dim oRet As New roCollection

            Dim dTbl As DataTable

            Try

                Dim strSQL As String = "@SELECT# ID FROM Causes  Order By ID"

                dTbl = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each dRow As DataRow In dTbl.Rows
                        oRet.Add(dRow("ID"), New roCause(dRow("ID"), _State, bAudit))
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCause::GetConceptsListWithData")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCause::GetConceptsListWithData")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#Region "Causes Employee Helper"

        Public Shared Function GetContractAnnualizedCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                               Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing
            Try
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = Nothing
                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, intIDEmployee, xDate, roContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                Dim strSQL As String
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat , isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DC.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualCauses")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetAnualCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                               Optional idCause As Integer = 0, Optional ByVal Last As Boolean = False) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1
                If intYearIniMonth = 0 Then intYearIniMonth = 1

                Dim xBeginPeriod As Date
                If xDate.Month > intYearIniMonth Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                ElseIf xDate.Month = intYearIniMonth And xDate.Day >= intMonthIniDay Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                Else
                    xBeginPeriod = New DateTime(xDate.Year - 1, intYearIniMonth, intMonthIniDay)
                End If

                If (Last = True) Then
                    xBeginPeriod = xBeginPeriod.AddYears(-1)
                    xDate = xBeginPeriod.AddYears(1).AddDays(-1)
                End If

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat , isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DC.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualCauses")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetMonthCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                               Optional idCause As Integer = 0, Optional ByVal Last As Boolean = False) As DataTable
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

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat , isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DC.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthCauses")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetContractCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                  Optional idCause As Integer = 0) As DataTable
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
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat, isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DC.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try

                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetContractCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetContractCauses")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetWeekCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                              Optional idCause As Integer = 0) As DataTable
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

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat , isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DC.Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekCauses")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetDailyCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                               Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim myDate = New DateTime(xDate.Year, xDate.Month, xDate.Day, 0, 0, 0, 0)

                Dim strSQL As String
                strSQL = "@SELECT# CA.ID AS IDCause, CA.Name, ISNULL(SUM(DC.VALUE),0) AS Total, CA.WorkingType, '' AS TotalFormat , isnull(CA.DayType, 0) as DayType, isnull(CA.CustomType, 0) as CustomType "
                strSQL &= " FROM Causes CA "

                strSQL &= " INNER JOIN DailyCauses DC ON CA.ID = DC.IDCause "

                strSQL &= " WHERE ((DC.IDEmployee = " & intIDEmployee.ToString & " AND "
                strSQL &= " DC.Date = " & roTypes.Any2Time(myDate).SQLSmallDateTime & ") OR "
                strSQL &= " DC.IDEmployee IS NULL) "

                If idCause > 0 Then
                    strSQL &= " AND CA.ID = " & idCause.ToString
                End If

                strSQL &= " GROUP BY CA.ID, CA.NAME, CA.WorkingType, CA.DayType, CA.CustomType "

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Boolean(oRow("DayType")) Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If

                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyCauses")
            Finally

            End Try

            Return tb
        End Function

#End Region

        Public Class roCauseCriteria

            Public Enum eCriteriaCondition
                <EnumMember> Equal
                <EnumMember> NoEqual
                <EnumMember> Major
                <EnumMember> MajorOrEqual
                <EnumMember> Minor
                <EnumMember> MinorOrEqual
                <EnumMember> StartsWith
                <EnumMember> Contains
                <EnumMember> NoContains
            End Enum

            Public Enum eCriteriaValueType
                <EnumMember> StringValueType
                <EnumMember> NumericValueType
                <EnumMember> DateValueType
                <EnumMember> DatePeriodValueType
                <EnumMember> TimeValueType
                <EnumMember> TimePeriodValueType
                <EnumMember> CauseDateValueType
                <EnumMember> CauseTimeValueType
            End Enum

            Private oCollection As roCollection

            Public Sub New()
                oCollection = New roCollection
                oCollection.Add("UserField", "")
                oCollection.Add("Comp", "")
                oCollection.Add("ValueType", "")
                oCollection.Add("Value", Nothing)
            End Sub

            Public Sub New(ByVal oCollectionCriteria As roCollection)
                AddCriteria(oCollectionCriteria("UserField"), oCollectionCriteria("Comp"), oCollectionCriteria("ValueType"), oCollectionCriteria("Value"))
            End Sub

            Public Sub New(ByVal UserField As String, ByVal Comp As String, ByVal ValueType As String, ByVal [Value] As Object)
                AddCriteria(UserField, Comp, ValueType, [Value])
            End Sub

            <DataMember>
            Public Property UserField() As String
                Get
                    If oCollection("UserField") Is Nothing Then
                        Return ""
                    Else
                        Return oCollection("UserField").ToString
                    End If
                End Get
                Set(ByVal value As String)
                    oCollection("UserField") = value
                End Set
            End Property

            <DataMember>
            Public Property Comp() As eCriteriaCondition
                Get
                    If oCollection("Comp") Is Nothing Then
                        Return eCriteriaCondition.Equal
                    Else
                        Select Case oCollection("Comp").ToString
                            Case Is = "="
                                Return eCriteriaCondition.Equal
                            Case Is = "<>"
                                Return eCriteriaCondition.NoEqual
                            Case Is = ">"
                                Return eCriteriaCondition.Major
                            Case Is = ">="
                                Return eCriteriaCondition.MajorOrEqual
                            Case Is = "<"
                                Return eCriteriaCondition.Minor
                            Case Is = "<="
                                Return eCriteriaCondition.MinorOrEqual
                            Case Is = "*"
                                Return eCriteriaCondition.StartsWith
                            Case Is = "**"
                                Return eCriteriaCondition.Contains
                            Case Is = "!**"
                                Return eCriteriaCondition.NoContains
                            Case Else
                                Return eCriteriaCondition.Equal
                        End Select
                    End If
                End Get
                Set(ByVal value As eCriteriaCondition)
                    Select Case value
                        Case eCriteriaCondition.Equal
                            oCollection("Comp") = "="
                        Case eCriteriaCondition.NoEqual
                            oCollection("Comp") = "<>"
                        Case eCriteriaCondition.Major
                            oCollection("Comp") = ">"
                        Case eCriteriaCondition.MajorOrEqual
                            oCollection("Comp") = ">="
                        Case eCriteriaCondition.Minor
                            oCollection("Comp") = "<"
                        Case eCriteriaCondition.MinorOrEqual
                            oCollection("Comp") = "<="
                        Case eCriteriaCondition.StartsWith
                            oCollection("Comp") = "*"
                        Case eCriteriaCondition.Contains
                            oCollection("Comp") = "**"
                        Case eCriteriaCondition.NoContains
                            oCollection("Comp") = "!**"
                    End Select
                End Set
            End Property

            <DataMember>
            Public Property ValueType() As eCriteriaValueType
                Get
                    If oCollection("ValueType") Is Nothing Then
                        Return eCriteriaValueType.StringValueType
                    Else
                        Select Case oCollection("ValueType").ToString
                            Case Is = "@String"
                                Return eCriteriaValueType.StringValueType
                            Case Is = "@Numeric"
                                Return eCriteriaValueType.NumericValueType
                            Case Is = "@Date"
                                Return eCriteriaValueType.DateValueType
                            Case Is = "@DatePeriod"
                                Return eCriteriaValueType.DatePeriodValueType
                            Case Is = "@Time"
                                Return eCriteriaValueType.TimeValueType
                            Case Is = "@TimePeriod"
                                Return eCriteriaValueType.TimePeriodValueType
                            Case Is = "@CauseDate"
                                Return eCriteriaValueType.CauseDateValueType
                            Case Is = "@CauseTime"
                                Return eCriteriaValueType.CauseTimeValueType
                            Case Else
                                Return eCriteriaValueType.StringValueType
                        End Select
                    End If
                End Get
                Set(ByVal value As eCriteriaValueType)
                    Select Case value
                        Case eCriteriaValueType.StringValueType
                            oCollection("ValueType") = "@String"
                        Case eCriteriaValueType.NumericValueType
                            oCollection("ValueType") = "@Numeric"
                        Case eCriteriaValueType.DateValueType
                            oCollection("ValueType") = "@Date"
                        Case eCriteriaValueType.DatePeriodValueType
                            oCollection("ValueType") = "@DatePeriod"
                        Case eCriteriaValueType.TimeValueType
                            oCollection("ValueType") = "@Time"
                        Case eCriteriaValueType.TimePeriodValueType
                            oCollection("ValueType") = "@TimePeriod"
                        Case eCriteriaValueType.CauseDateValueType
                            oCollection("ValueType") = "@CauseDate"
                        Case eCriteriaValueType.CauseTimeValueType
                            oCollection("ValueType") = "@CauseTime"
                    End Select
                End Set
            End Property

            <DataMember>
            Public Property [Value]() As Object
                Get
                    If oCollection("Value") Is Nothing Then
                        Return Nothing
                    Else
                        Return oCollection("Value").ToString
                    End If
                End Get
                Set(ByVal value As Object)
                    oCollection("Value") = value
                End Set
            End Property

            Public Sub AddCriteria(ByVal UserField As String, ByVal Comp As String, ByVal ValueType As String, ByVal [Value] As Object)
                oCollection = New roCollection
                oCollection.Add("UserField", UserField)
                oCollection.Add("Comp", Comp)
                oCollection.Add("ValueType", ValueType)
                oCollection.Add("Value", [Value])
            End Sub

            Public Function RetRoCollection() As roCollection
                Return oCollection
            End Function

        End Class

    End Class

    Public Class roAutomaticEquivalenceCriteria

#Region "Declarations - Constructor"

        Private intAutomaticEquivalenceType As eAutomaticEquivalenceType
        Private oUserField As VTUserFields.UserFields.roUserField
        Private dblFactorValue As Double

        Public Sub New()
            Me.intAutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType
            Me.oUserField = Nothing
        End Sub

        Public Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then
                ' Añadimos la composición a la colección
                Dim oDefinition As New roCollection(strXml)

                If oDefinition.Exists("FactorValue") And Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType Then
                    Me.FactorValue = oDefinition("FactorValue")
                End If

                If oDefinition.Exists("FactorField") And Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType Then
                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(-1)
                    Me.oUserField = New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("FactorField"), UserFieldsTypes.Types.EmployeeField, False)
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
        Public Property FactorValue() As Double
            Get
                Return Me.dblFactorValue
            End Get
            Set(ByVal value As Double)
                Me.dblFactorValue = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticEquivalenceType() As eAutomaticEquivalenceType
            Get
                Return Me.intAutomaticEquivalenceType
            End Get
            Set(ByVal value As eAutomaticEquivalenceType)
                Me.intAutomaticEquivalenceType = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String
            Dim oAutomaticEquivalenceCriteria As New roCollection
            If Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType Then
                oAutomaticEquivalenceCriteria.Add("FactorValue", Me.FactorValue)
            ElseIf Me.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType Then
                If Not oUserField Is Nothing Then
                    oAutomaticEquivalenceCriteria.Add("FactorField", Me.oUserField.FieldName)
                End If
            End If
            Return oAutomaticEquivalenceCriteria.XML

        End Function

#End Region

    End Class

End Namespace