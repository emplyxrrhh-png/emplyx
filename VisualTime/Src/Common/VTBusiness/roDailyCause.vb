Imports System.Data.Common
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Cause

    Public Class roDailyCause

#Region "Properties - Constructor"

        Private oState As roCauseState

        Private intIDEmployee As Integer
        Private xDate As Date
        Private intIDRelatedIncidence As Integer
        Private intIDCause As Integer
        Private oCause As roCause

        Private dblValue As Double
        Private bolManual As Nullable(Of Boolean)
        Private intCauseUser As Nullable(Of Integer)
        Private intCauseUserType As Nullable(Of Integer)

        Private intAccrualsRules As Integer

        Private intDailyRule As Integer

        Private intAccruedRule As Integer

        Private bIsMirrorTable As Boolean

        Private intIDCenter As Nullable(Of Integer)
        Private bolDefaultCenter As Nullable(Of Boolean)
        Private bolManualCenter As Nullable(Of Boolean)

        Private bolIsNotReliable As Nullable(Of Boolean)

        Public Sub New()
            Me.oState = New roCauseState
            ' Valores por defecto
            Me.intIDEmployee = -1
            Me.dblValue = 0
            Me.bolManual = True
            Me.intCauseUser = 1
            Me.intCauseUserType = 1
            Me.bolIsNotReliable = False
            Me.intIDCenter = 0
            Me.bolDefaultCenter = True
            Me.bolManualCenter = False

            Me.bIsMirrorTable = False
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal _IDRelatedIncidence As Integer, ByVal _IDCause As Integer, ByVal _AccrualsRules As Integer, ByVal _IDCenter As Integer, ByVal _DailyRule As Integer, ByVal _AccruedRule As Integer, ByVal _State As roCauseState, Optional ByVal bIsMirror As Boolean = False)
            Me.oState = _State
            Me.intIDEmployee = _IDEmployee
            Me.xDate = _Date
            Me.intIDRelatedIncidence = _IDRelatedIncidence
            Me.intIDCause = _IDCause
            Me.intAccrualsRules = _AccrualsRules
            Me.intDailyRule = _DailyRule
            Me.intAccruedRule = _AccruedRule
            Me.intIDCenter = _IDCenter
            Me.bIsMirrorTable = bIsMirror
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
        End Property

        Public ReadOnly Property Date_() As Date
            Get
                Return Me.xDate
            End Get
        End Property

        Public ReadOnly Property IDRelatedIncidence() As Integer
            Get
                Return Me.intIDRelatedIncidence
            End Get
        End Property

        Public ReadOnly Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
        End Property

        Public Property Value() As Double
            Get
                Return Me.dblValue
            End Get
            Set(ByVal value As Double)
                Me.dblValue = value
            End Set
        End Property

        Public Property Manual() As Nullable(Of Boolean)
            Get
                Return Me.bolManual
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolManual = value
            End Set
        End Property

        Public Property DefaultCenter() As Nullable(Of Boolean)
            Get
                Return Me.bolDefaultCenter
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolDefaultCenter = value
            End Set
        End Property

        Public Property ManualCenter() As Nullable(Of Boolean)
            Get
                Return Me.bolManualCenter
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolManualCenter = value
            End Set
        End Property

        Public Property CauseUser() As Nullable(Of Integer)
            Get
                Return Me.intCauseUser
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intCauseUser = value
            End Set
        End Property

        Public Property IDCenter() As Nullable(Of Integer)
            Get
                Return Me.intIDCenter
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDCenter = value
            End Set
        End Property

        Public Property CauseUserType() As Nullable(Of Integer)
            Get
                Return Me.intCauseUserType
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intCauseUserType = value
            End Set
        End Property

        Public ReadOnly Property AccrualsRules() As Integer
            Get
                Return Me.intAccrualsRules
            End Get
        End Property

        Public ReadOnly Property DailyRule() As Integer
            Get
                Return Me.intDailyRule
            End Get
        End Property

        Public ReadOnly Property AccruedRule() As Integer
            Get
                Return Me.intAccruedRule
            End Get
        End Property

        Public Property IsNotReliable() As Nullable(Of Boolean)
            Get
                Return Me.bolIsNotReliable
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolIsNotReliable = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Function SQLWhere(Optional ByVal _Manual As Nullable(Of Boolean) = Nothing, Optional ByVal _CheckUser As Boolean = False) As String
            Dim strTableName As String = IIf(Not Me.bIsMirrorTable, "DailyCauses", "DailyCauses_MIRROR")

            Dim strWhere As String = strTableName & ".IDEmployee = " & Me.IDEmployee & " And " &
                   strTableName & ".Date = " & Any2Time(Me.Date_).SQLSmallDateTime & " AND " &
                   strTableName & ".IDRelatedIncidence = " & Me.IDRelatedIncidence & " AND " &
                   strTableName & ".IDCause = " & Me.IDCause & " AND " &
                   strTableName & ".AccrualsRules = " & Me.AccrualsRules & " AND " &
                   strTableName & ".DailyRule = " & Me.DailyRule & " AND " &
                   strTableName & ".AccruedRule = " & Me.AccruedRule

            If Me.IDCenter.HasValue Then
                strWhere = strWhere & " AND " & strTableName & ".IDCenter = " & Me.IDCenter
            End If

            If _Manual.HasValue Then
                strWhere = strWhere & " AND " & strTableName & ".Manual = " & IIf(_Manual.Value, "1", "0")
            End If

            If _CheckUser Then
                strWhere = strWhere & " AND " & strTableName & ".CauseUser IS NULL"
            End If

            Return strWhere
        End Function

        Private Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM " & IIf(Not Me.bIsMirrorTable, "DailyCauses", "DailyCauses_MIRROR") & " " &
                                       "WHERE " & Me.SQLWhere()
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Me.dblValue = oRow("Value")
                    If Not IsDBNull(oRow("Manual")) Then Me.bolManual = oRow("Manual")
                    If Not IsDBNull(oRow("CauseUser")) Then Me.intCauseUser = CInt(oRow("CauseUser"))
                    If Not IsDBNull(oRow("CauseUserType")) Then Me.intCauseUserType = CInt(oRow("CauseUserType"))
                    If Not IsDBNull(oRow("IsNotReliable")) Then Me.bolIsNotReliable = oRow("IsNotReliable")
                    If Not IsDBNull(oRow("IDCenter")) Then Me.intIDCenter = CInt(oRow("IDCenter"))
                    If Not IsDBNull(oRow("DefaultCenter")) Then Me.bolDefaultCenter = oRow("DefaultCenter")
                    If Not IsDBNull(oRow("ManualCenter")) Then Me.bolManualCenter = oRow("ManualCenter")

                    Me.oCause = New roCause(Me.intIDCause, Me.oState, False)
                    bolRet = True
                Else
                    ' Valores por defecto
                    Me.dblValue = 0
                    Me.bolManual = True
                    Me.intCauseUser = 1
                    Me.intCauseUserType = 1
                    Me.bolIsNotReliable = False
                    Me.intIDCenter = 0
                    Me.bolDefaultCenter = True
                    Me.bolManualCenter = False

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyCause:Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCause:Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function LoadWithParams(ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal _IDCause As Integer, ByVal _Manual As Boolean, Optional ByVal _CheckUser As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.intIDEmployee = _IDEmployee
                Me.xDate = _Date
                Me.intIDCause = _IDCause
                Me.bolManual = _Manual
                Me.intAccrualsRules = 0
                Me.intDailyRule = 0
                Me.intAccruedRule = 0
                Me.intIDRelatedIncidence = 0
                Me.CauseUser = Nothing
                Me.CauseUserType = Nothing

                Dim oBusinessCenterState As New BusinessCenter.roBusinessCenterState(-1)
                Dim intIDefaultCenter = BusinessCenter.roBusinessCenter.GetEmployeeDefaultBusinessCenter(oBusinessCenterState, _IDEmployee, _Date)
                Me.IDCenter = intIDefaultCenter

                Dim strSQL As String = "@SELECT# Value FROM " & IIf(Not Me.bIsMirrorTable, "DailyCauses", "DailyCauses_MIRROR") & " WHERE " & Me.SQLWhere(_Manual, _CheckUser)
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Me.dblValue = oRow("Value")
                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyCause:Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCause:Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal bolNotify As Boolean = False, Optional ByVal Action As String = "") As Boolean

            Dim bolRet As Boolean = False

            Dim oAuditDataOld As DataRow = Nothing
            Dim oAuditDataNew As DataRow = Nothing

            Try

                Dim tb As New DataTable("DailyCauses")
                Dim strSQL As String = "@SELECT# * FROM " & IIf(Not Me.bIsMirrorTable, "DailyCauses", "DailyCauses_MIRROR") & " WHERE " & Me.SQLWhere()
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("IDEmployee") = Me.intIDEmployee
                oRow("Date") = Me.xDate
                oRow("IDRelatedIncidence") = Me.intIDRelatedIncidence
                oRow("IDCause") = Me.intIDCause
                Try
                    oRow("Value") = Math.Round(Me.dblValue, 6)
                Catch ex As Exception
                    oRow("Value") = Me.dblValue
                End Try
                If Me.bolManual.HasValue Then oRow("Manual") = Me.bolManual.Value
                If Me.intCauseUser.HasValue Then oRow("CauseUser") = Me.intCauseUser.Value
                If Me.intCauseUserType.HasValue Then oRow("CauseUserType") = Me.intCauseUserType.Value
                oRow("AccrualsRules") = Me.intAccrualsRules

                oRow("DailyRule") = Me.intDailyRule

                oRow("AccruedRule") = Me.intAccruedRule

                If Me.bolIsNotReliable.HasValue Then oRow("IsNotReliable") = Me.bolIsNotReliable.Value

                If Me.intIDCenter.HasValue Then
                    oRow("IDCenter") = Me.intIDCenter.Value
                Else
                    ' intentamos asignar el centro de coste por defecto del empleado
                    Dim oBusinessCenterState As New BusinessCenter.roBusinessCenterState(-1)
                    Dim intIDefaultCenter = BusinessCenter.roBusinessCenter.GetEmployeeDefaultBusinessCenter(oBusinessCenterState, intIDEmployee, Me.xDate)
                    oRow("IDCenter") = intIDefaultCenter
                End If

                If Me.bolDefaultCenter.HasValue Then oRow("DefaultCenter") = Me.bolDefaultCenter.Value
                If Me.bolManualCenter.HasValue Then oRow("ManualCenter") = Me.bolManualCenter.Value

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True

                If bolRet AndAlso bolNotify AndAlso Not bIsMirrorTable Then
                    roConnector.InitTask(TasksType.DAILYCAUSES)
                End If

                If bolRet AndAlso bAudit Then
                    oAuditDataNew = oRow
                    bolRet = False
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                    If Action <> "" Then
                        Select Case Action
                            Case "Added"
                                oAuditAction = Audit.Action.aInsert
                            Case "Modified"
                                oAuditAction = Audit.Action.aUpdate
                        End Select
                    End If

                    Dim strObjectName As String = ""

                    'Recupero nombre de la justificación
                    Dim oCauseState As New VTBusiness.Cause.roCauseState
                    Dim oCause As New VTBusiness.Cause.roCause(Me.intIDCause, oCauseState)
                    Dim sCauseName As String = " (" & oCause.Name & ")"
                    oState.AddAuditParameter(tbAuditParameters, "{CauseName}", sCauseName, "", 1)
                    strObjectName = oCause.Name

                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDailyCause, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCause:Save")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim sSql As String = "@DELETE# FROM " & IIf(Not Me.bIsMirrorTable, "DailyCauses", "DailyCauses_MIRROR") & " WHERE " & Me.SQLWhere()
                bolRet = ExecuteSql(sSql)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyCause::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCause::Delete")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

    Public Class roDailyCauseList

#Region "Declarations - Constructors"

        Private oState As roCauseState

        Private Items As ArrayList

        Public Sub New()

            Me.oState = New roCauseState
            Me.Items = New ArrayList

        End Sub

        Public Sub New(ByVal _State As roCauseState)

            Me.oState = _State
            Me.Items = New ArrayList

        End Sub

#End Region

#Region "Properties"

        <XmlArray("DailyCauses"), XmlArrayItem("roDailyCause", GetType(roDailyCause))>
        Public Property DailyCauses() As ArrayList
            Get
                Return Me.Items
            End Get
            Set(ByVal value As ArrayList)
                Me.Items = value
            End Set
        End Property

        Public ReadOnly Property State() As roCauseState
            Get
                Return Me.oState
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try
                For Each oMove As roDailyCause In Me.DailyCauses
                    bolRet = oMove.Save
                    If Not bolRet Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDailyCauseList::Save")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal tbDailyCauses As DataTable, ByVal bolUpdateStatus As Boolean,
                             Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim oDailyCause As roDailyCause

                ' Borrar las incidencias para el empleado y fecha
                Dim strSQL As String = "@DELETE# FROM DailyCauses WHERE IDEmployee = " & intIDEmployee & " AND Date = " & Any2Time(xDate).SQLSmallDateTime
                If ExecuteSql(strSQL) Then

                    For Each oRow As DataRow In tbDailyCauses.Rows
                        Select Case oRow.RowState
                            Case DataRowState.Added, DataRowState.Modified, DataRowState.Unchanged
                                oDailyCause = New roDailyCause(oRow("IDEmployee"), oRow("Date"), Any2Integer(oRow("IDRelatedIncidence")), Any2Integer(oRow("IDCause")), Any2Integer(oRow("AccrualsRules")), Any2Integer(oRow("IDCenter")), Any2Integer(oRow("DailyRule")), Any2Integer(oRow("AccruedRule")), Me.oState)
                                oDailyCause.Value += Any2Double(oRow("Value"))
                                If Not IsDBNull(oRow("Manual")) Then oDailyCause.Manual = oRow("Manual")
                                If Not IsDBNull(oRow("CauseUser")) Then oDailyCause.CauseUser = CInt(oRow("CauseUser"))
                                If Not IsDBNull(oRow("CauseUserType")) Then oDailyCause.CauseUserType = CInt(oRow("CauseUserType"))
                                If Not IsDBNull(oRow("IsNotReliable")) Then oDailyCause.IsNotReliable = oRow("IsNotReliable")

                                If Not IsDBNull(oRow("IDCenter")) Then oDailyCause.IDCenter = Any2Integer(oRow("IDCenter"))
                                If Not IsDBNull(oRow("DefaultCenter")) Then oDailyCause.DefaultCenter = oRow("DefaultCenter")
                                If Not IsDBNull(oRow("ManualCenter")) Then oDailyCause.ManualCenter = oRow("ManualCenter")

                                Dim _Audit = bAudit
                                If _Audit Then
                                    If oRow.RowState = DataRowState.Unchanged Then
                                        _Audit = False
                                    End If
                                End If

                                bolRet = oDailyCause.Save(_Audit, , oRow.RowState.ToString)
                        End Select
                    Next

                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status=65, [GUID] = '' WHERE IDEmployee = " & intIDEmployee & " AND Date=" & Any2Time(xDate).SQLSmallDateTime
                    If ExecuteSql(strSQL) Then
                        roConnector.InitTask(TasksType.DAILYCAUSES)
                        bolRet = True
                    Else
                        Dim olog As New VTBase.roLog("roDailyCauseList::Save")
                        olog.logMessage(roLog.EventType.roDebug, "ERROR al ejecutarSQL: " & strSQL)
                    End If
                Else
                    Dim olog As New VTBase.roLog("roDailyCauseList::Save")
                    olog.logMessage(roLog.EventType.roDebug, "ERROR al ejecutar SQL: " & strSQL)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCauseList::Save")
            End Try

            Return bolRet

        End Function

        Public Function SaveMassCauses(ByVal strRows As String(), ByRef oCauseState As roCauseState, ByRef oLiveTask As roLiveTask, ByRef strError As String,
                             Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim oDailyCause As New roDailyCause

                Dim strAction As String = ""
                Dim intIDEmployee As Integer = 0
                Dim xDate As DateTime
                Dim intIDRelatedIncidence As Integer = 0
                Dim intIDCause As Integer = 0
                Dim dblValue As Double = 0
                Dim intIDCauseOld As Integer = 0
                Dim DefaultCenter As Boolean = True
                Dim IDCenter As Integer = 0
                Dim ManualCenter As Boolean = False

                Dim totalActions As Integer = strRows.Length
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Integer = 100 / totalActions

                ' Primero eliminamos los registros que han cambiado
                For Each sRow As String In strRows
                    Dim strFields() As String = sRow.Split("@")
                    If strFields.Length = 10 Then
                        strAction = strFields(0)
                        intIDEmployee = roTypes.Any2Integer(strFields(1))
                        xDate = DateTime.Parse(strFields(2))
                        intIDRelatedIncidence = roTypes.Any2Integer(strFields(3))
                        intIDCause = roTypes.Any2Integer(strFields(4))
                        dblValue = Any2Double(strFields(5).Replace(".", roConversions.GetDecimalDigitFormat))
                        intIDCauseOld = roTypes.Any2Integer(strFields(6))

                        IDCenter = roTypes.Any2Integer(strFields(8))

                        If Robotics.Security.WLHelper.HasFeaturePermissionByEmployeeOnDate(oCauseState.IDPassport, "Calendar.JustifyIncidences", Permission.Write, intIDEmployee, xDate.Date) Then
                            If strAction = "M" Then
                                oDailyCause = New Cause.roDailyCause(intIDEmployee, xDate, intIDRelatedIncidence, intIDCauseOld, 0, IDCenter, 0, 0, oCauseState)
                                If Not oDailyCause Is Nothing Then
                                    If Not oDailyCause.Delete Then
                                        strError += oState.ErrorText
                                    End If
                                End If
                            End If
                        Else
                            Dim oEmpState As New Employee.roEmployeeState(oCauseState.IDPassport)
                            oEmpState.Result = EmployeeResultEnum.AccessDenied
                            strError = strError & vbNewLine & roBusinessSupport.GetEmployeeName(intIDEmployee, oEmpState) & "(" & xDate.ToString(oState.Language.GetShortDateFormat) & "): " & oEmpState.ErrorText
                        End If

                    End If
                Next

                ' Luego añadimos todo los registros nuevos
                For Each sRow As String In strRows
                    Dim strFields() As String = sRow.Split("@")
                    If strFields.Length = 10 Then
                        strAction = strFields(0)
                        intIDEmployee = roTypes.Any2Integer(strFields(1))
                        xDate = DateTime.Parse(strFields(2))
                        intIDRelatedIncidence = roTypes.Any2Integer(strFields(3))
                        intIDCause = roTypes.Any2Integer(strFields(4))
                        dblValue = Any2Double(strFields(5).Replace(".", roConversions.GetDecimalDigitFormat))
                        intIDCauseOld = roTypes.Any2Integer(strFields(6))

                        DefaultCenter = IIf(roTypes.Any2Integer(strFields(7)) = 1, True, False)
                        IDCenter = roTypes.Any2Integer(strFields(8))
                        ManualCenter = IIf(roTypes.Any2Integer(strFields(9)) = 1, True, False)

                        If Robotics.Security.WLHelper.HasFeaturePermissionByEmployeeOnDate(oCauseState.IDPassport, "Calendar.JustifyIncidences", Permission.Write, intIDEmployee, xDate.Date) Then
                            roBusinessState.CopyTo(oCauseState, Me.oState)

                            oDailyCause = New roDailyCause(intIDEmployee, xDate, intIDRelatedIncidence, intIDCause, 0, IDCenter, 0, 0, Me.oState)
                            oDailyCause.Value += dblValue
                            oDailyCause.Manual = 1
                            oDailyCause.CauseUser = 1
                            oDailyCause.CauseUserType = 1
                            oDailyCause.IsNotReliable = False
                            oDailyCause.DefaultCenter = DefaultCenter
                            oDailyCause.IDCenter = IDCenter
                            oDailyCause.ManualCenter = ManualCenter

                            If oDailyCause.Save(bAudit) Then
                                Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status=65, [GUID] = '' WHERE IDEmployee = " & intIDEmployee & " AND Date=" & Any2Time(xDate).SQLSmallDateTime
                                If Not ExecuteSql(strSQL) Then
                                    Dim olog As New roLog("roDailyCauseList::SaveMassCauses")
                                    olog.logMessage(roLog.EventType.roDebug, "Error executing SQL Command: " & strSQL)
                                    strError += "Error executing SQL Command: " & strSQL
                                End If
                            End If
                        End If

                    End If
                    oLiveTask.Progress = oLiveTask.Progress + stepProgress
                    oLiveTask.Save()
                Next

                If strRows.Length > 0 Then roConnector.InitTask(TasksType.DAILYCAUSES)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCauseList::SaveMassCauses")
                strError += ex.Message
            End Try

            Return bolRet

        End Function

        Public Function SaveMassAssignCenters(ByVal strRows As String(), ByRef oCauseState As roCauseState, ByRef oLiveTask As roLiveTask, ByRef strError As String,
                     Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim bIsMirror As Boolean = roBusinessSupport.GetCustomizationCode() = "taif"

                Dim oDailyCause As New roDailyCause

                Dim strAction As String = ""
                Dim intIDEmployee As Integer = 0
                Dim xDate As DateTime
                Dim intIDRelatedIncidence As Integer = 0
                Dim intIDCause As Integer = 0
                Dim dblValue As Double = 0
                Dim intIDCauseOld As Integer = 0
                Dim DefaultCenter As Boolean = True
                Dim IDCenter As Integer = 0
                Dim IDCenterOld As Integer = 0
                Dim ManualCenter As Boolean = False

                Dim AccrualRule As Integer = 0
                Dim DailyRule As Integer = 0

                Dim AccruedRule As Integer = 0

                Dim totalActions As Integer = strRows.Length
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Integer = 100 / totalActions

                ' Primero eliminamos los registros que han cambiado
                For Each sRow As String In strRows
                    Dim strFields As String() = sRow.Split("@")
                    If strFields.Length = 13 Then
                        strAction = strFields(0)
                        intIDEmployee = roTypes.Any2Integer(strFields(1))
                        xDate = DateTime.Parse(strFields(2))
                        intIDRelatedIncidence = roTypes.Any2Integer(strFields(3))
                        intIDCause = roTypes.Any2Integer(strFields(4))
                        dblValue = Any2Double(strFields(5).Replace(".", roConversions.GetDecimalDigitFormat))
                        IDCenter = roTypes.Any2Integer(strFields(7))
                        IDCenterOld = roTypes.Any2Integer(strFields(9))
                        AccrualRule = roTypes.Any2Integer(strFields(10))
                        DailyRule = roTypes.Any2Integer(strFields(11))
                        AccruedRule = roTypes.Any2Integer(strFields(12))

                        If strAction = "M" Then
                            oDailyCause = New Cause.roDailyCause(intIDEmployee, xDate, intIDRelatedIncidence, intIDCause, AccrualRule, IDCenterOld, DailyRule, AccruedRule, oCauseState, bIsMirror)
                            If oDailyCause IsNot Nothing Then
                                bolRet = oDailyCause.Delete
                                If Not bolRet Then
                                    strError += oState.ErrorText
                                End If
                            End If
                        End If
                    End If
                Next

                ' Luego añadimos todo los registros nuevos
                For Each sRow As String In strRows
                    Dim strFields As String() = sRow.Split("@")
                    If strFields.Length = 13 Then
                        strAction = strFields(0)
                        intIDEmployee = roTypes.Any2Integer(strFields(1))
                        xDate = DateTime.Parse(strFields(2))
                        intIDRelatedIncidence = roTypes.Any2Integer(strFields(3))
                        intIDCause = roTypes.Any2Integer(strFields(4))
                        dblValue = Any2Double(strFields(5).Replace(".", roConversions.GetDecimalDigitFormat))
                        DefaultCenter = IIf(roTypes.Any2Integer(strFields(6)) = 1, True, False)
                        IDCenter = roTypes.Any2Integer(strFields(7))
                        ManualCenter = IIf(roTypes.Any2Integer(strFields(8)) = 1, True, False)
                        AccrualRule = roTypes.Any2Integer(strFields(10))
                        DailyRule = roTypes.Any2Integer(strFields(11))
                        AccruedRule = roTypes.Any2Integer(strFields(12))

                        oDailyCause = New roDailyCause(intIDEmployee, xDate, intIDRelatedIncidence, intIDCause, AccrualRule, IDCenter, DailyRule, AccruedRule, Me.oState, bIsMirror)
                        oDailyCause.Value += dblValue
                        oDailyCause.Manual = 1
                        oDailyCause.CauseUser = 1
                        oDailyCause.CauseUserType = 1
                        oDailyCause.IsNotReliable = False
                        oDailyCause.DefaultCenter = DefaultCenter
                        oDailyCause.IDCenter = IDCenter
                        oDailyCause.ManualCenter = ManualCenter

                        bolRet = oDailyCause.Save(bAudit)

                    End If
                    oLiveTask.Progress = oLiveTask.Progress + stepProgress
                    oLiveTask.Save()
                Next

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyCauseList::SaveMassAssignCenters")
                strError += ex.Message
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal ds As DataSet, ByRef oState As roCauseState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ds.Tables.Contains("DailyCauses") Then

                    Dim tb As DataTable = ds.Tables("DailyCauses")
                    Dim oDailyCause As roDailyCause

                    For Each oRow As DataRow In tb.Rows
                        oDailyCause = New roDailyCause(Any2Integer(oRow("IDEmployee")),
                                                       oRow("Date"),
                                                       Any2Integer(oRow("IDRelatedIncidence")),
                                                       Any2Integer(oRow("IDCause")),
                                                       Any2Integer(oRow("AccrualsRules")), Any2Integer(oRow("IDCenter")), Any2Integer(oRow("DailyRule")), Any2Integer(oRow("AccruedRule")), oState)
                        With oDailyCause
                            If Not IsDBNull(oRow("Value")) Then .Value = oRow("Value")
                            If Not IsDBNull(oRow("Manual")) Then .Manual = oRow("Manual")
                            If Not IsDBNull(oRow("CauseUser")) Then .CauseUser = oRow("CauseUser")
                            If Not IsDBNull(oRow("CauseUserType")) Then .CauseUserType = oRow("CauseUserType")
                            If Not IsDBNull(oRow("IsNotReliable")) Then .IsNotReliable = oRow("IsNotReliable")

                            If Not IsDBNull(oRow("IDCenter")) Then oDailyCause.IDCenter = oRow("IDCenter")
                            If Not IsDBNull(oRow("DefaultCenter")) Then oDailyCause.DefaultCenter = oRow("DefaultCenter")
                            If Not IsDBNull(oRow("ManualCenter")) Then oDailyCause.ManualCenter = oRow("ManualCenter")

                        End With
                        Me.DailyCauses.Add(oDailyCause)
                    Next

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyCauseList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCauseList:LoadData")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace