Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace AccessPeriod

    <DataContract()>
    Public Class roAccessPeriodDaily

#Region "Declarations - Constructor"

        Private oState As roAccessPeriodState

        Private intID As Integer
        Private iDayofWeek As eWeekDay
        Private dBeginTime As Date
        Private dEndTime As Date

        Public Sub New()
            Me.oState = New roAccessPeriodState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _DayofWeek As eWeekDay, ByVal _BeginTime As Date, ByVal _EndTime As Date, ByVal _State As roAccessPeriodState)
            Me.oState = _State
            Me.intID = _ID
            Me.iDayofWeek = _DayofWeek
            Me.dBeginTime = _BeginTime
            Me.dEndTime = _EndTime
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAccessPeriodState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessPeriodState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property IDAccessPeriod() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember()>
        Public Property DayofWeek() As eWeekDay
            Get
                Return Me.iDayofWeek
            End Get
            Set(ByVal value As eWeekDay)
                Me.iDayofWeek = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginTime() As Date
            Get
                Return Me.dBeginTime
            End Get
            Set(ByVal value As Date)
                Me.dBeginTime = value
            End Set
        End Property

        <DataMember()>
        Public Property EndTime() As Date
            Get
                Return Me.dEndTime
            End Get
            Set(ByVal value As Date)
                Me.dEndTime = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.GetDescription
            End Get
            Set(ByVal value As String)
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("AccessPeriodDaily")
                Dim strSQL As String = "@SELECT# * FROM AccessPeriodDaily WHERE IDAccessPeriod = " & Me.intID.ToString & " And DayofWeek = " & Me.iDayofWeek & " And BeginTime = " & roTypes.Any2Time(Me.dBeginTime).SQLDateTime &
                                        " And EndTime = " & roTypes.Any2Time(Me.dEndTime).SQLDateTime & " ORDER BY DayofWeek"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDAccessPeriod") = Me.IDAccessPeriod
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("DayofWeek") = Me.iDayofWeek
                oRow("BeginTime") = Me.dBeginTime
                oRow("EndTime") = Me.dEndTime

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
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessZone, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodDaily::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodDaily::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim DeleteQuerys() As String = {"@DELETE# FROM AccessPeriodDaily WHERE IDAccessPeriod = " & Me.intID.ToString & " And DayofWeek = " & Me.iDayofWeek & " And BeginTime = " & roTypes.Any2Time(Me.dBeginTime).SQLSmallDateTime & " And EndTime = " & roTypes.Any2Time(Me.dEndTime).SQLSmallDateTime}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessPeriodDaily, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriods::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriods::Delete")
            End Try

            Return bolRet

        End Function

        Public Function GetDescription() As String
            Try
                Dim strDescription As String = ""
                Dim strSufix1 As String = ""
                Dim strSufix2 As String = ""
                Dim strResult As String = ""

                strDescription = oState.Language.Translate("AccessPeriods.DayofWeek." & Me.DayofWeek.ToString, "")
                strDescription &= oState.Language.Translate("AccessPeriods.Colon", "")

                If Me.dBeginTime.Hour = 0 And Me.dBeginTime.Minute = 0 And Me.dEndTime.Hour = 0 And Me.dEndTime.Minute = 0 Then
                    strSufix1 = oState.Language.Translate("AccessPeriods.Sufix1.NoAccess", "")
                    strSufix2 = oState.Language.Translate("AccessPeriods.Sufix2.NoAccess", "")
                Else
                    strSufix1 = oState.Language.Translate("AccessPeriods.Sufix1.Access", "")
                    strSufix2 = oState.Language.Translate("AccessPeriods.Sufix2.Access", "")
                End If

                If Me.dBeginTime.Hour = 0 And Me.dBeginTime.Minute = 0 And Me.dEndTime.Hour = 0 And Me.dEndTime.Minute = 0 Then
                    strResult = strDescription & " " & strSufix1 & " " & strSufix2
                Else
                    strResult = strDescription & " " & strSufix1 & " " & Format(Me.BeginTime, "H:mm") & " " & strSufix2 & " " & Format(Me.EndTime, "H:mm")
                End If

                Return strResult
            Catch ex As Exception
                Return ex.ToString & " " & ex.StackTrace
            End Try
        End Function

#Region "Helper methods"

        Public Shared Function GetAccessPeriodsDailyList(ByVal _ID As Integer, ByVal _State As roAccessPeriodState) As Generic.List(Of roAccessPeriodDaily)

            Dim oRet As New Generic.List(Of roAccessPeriodDaily)

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriodDaily Where IDAccessPeriod = " & _ID & " ORDER BY DayofWeek"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAccessPeriodDaily As roAccessPeriodDaily = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAccessPeriodDaily = New roAccessPeriodDaily(oRow("IDAccessPeriod"), oRow("DayofWeek"), oRow("BeginTime"), oRow("EndTime"), _State)
                        oRet.Add(oAccessPeriodDaily)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriodDaily::GetAccessPeriodsDailyList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriodDaily::GetAccessPeriodsDailyList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessPeriodsDailyDataTable(ByVal _ID As Integer, ByVal _State As roAccessPeriodState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriodDaily Where IDAccessPeriod = " & _ID & " ORDER BY DayofWeek"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriodDaily::GetAccessPeriodsDailyDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriodDaily::GetAccessPeriodsDailyDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace