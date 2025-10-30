Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace AccessPeriod

    <DataContract()>
    Public Class roAccessPeriodHolidays

#Region "Declarations - Constructor"

        Private oState As roAccessPeriodState

        Private intID As Integer
        Private iDay As Integer
        Private iMonth As Integer
        Private dBeginTime As Date
        Private dEndTime As Date

        Public Sub New()
            Me.oState = New roAccessPeriodState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Day As Integer, ByVal _Month As Integer, ByVal _BeginTime As Date, ByVal _EndTime As Date, ByVal _State As roAccessPeriodState)
            Me.oState = _State
            Me.intID = _ID
            Me.iDay = _Day
            Me.iMonth = _Month
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
        Public Property Day() As Integer
            Get
                Return Me.iDay
            End Get
            Set(ByVal value As Integer)
                Me.iDay = value
            End Set
        End Property

        <DataMember()>
        Public Property Month() As Integer
            Get
                Return Me.iMonth
            End Get
            Set(ByVal value As Integer)
                Me.iMonth = value
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

                Dim tb As New DataTable("AccessPeriodHolidays")
                Dim strSQL As String = "@SELECT# * FROM AccessPeriodHolidays WHERE IDAccessPeriod = " & Me.intID.ToString & " And Day = " & Me.iDay & " And Month = " & Me.iMonth & "  And BeginTime = " & roTypes.Any2Time(Me.dBeginTime).SQLDateTime &
                                        " And EndTime = " & roTypes.Any2Time(Me.dEndTime).SQLDateTime
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

                oRow("Day") = Me.iDay
                oRow("Month") = Me.iMonth
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
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessPeriodHolidays, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodHolidays::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodHolidays::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim DeleteQuerys() As String = {"@DELETE# FROM AccessPeriodHolidays WHERE IDAccessPeriod = " & Me.intID.ToString & " And Day = " & Me.iDay & " And Month = " & Me.iMonth & " And Begintime = " & roTypes.Any2Time(Me.dBeginTime).SQLSmallDateTime & " And EndTime = " & roTypes.Any2Time(Me.dEndTime).SQLSmallDateTime}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessPeriodHolidays, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodHolidays::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriodHolidays::Delete")
            End Try

            Return bolRet

        End Function

        Public Function GetDescription() As String
            Try
                Dim strResult As String = ""
                Dim strDescription As String = ""
                Dim strSufix1 As String = ""
                Dim strSufix2 As String = ""
                Dim oInfo As Globalization.DateTimeFormatInfo = Globalization.CultureInfo.CurrentCulture.DateTimeFormat

                strDescription = oState.Language.Translate("AccessPeriods.TheDay", "")

                If Me.iMonth <> 0 Then
                    ' un dia concreto
                    If (oInfo.ShortDatePattern.ToUpper.StartsWith("M")) Then
                        strDescription &= " " & Me.iMonth.ToString & "/" & Me.iDay.ToString
                    Else
                        strDescription &= " " & Me.iDay.ToString & "/" & Me.iMonth.ToString
                    End If
                Else
                    ' el dia del evento
                    strDescription &= " " & oState.Language.Translate("AccessPeriods.EventDay", "")
                End If

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

        Public Shared Function GetAccessPeriodHolidaysList(ByVal _ID As Integer, ByVal _State As roAccessPeriodState) As Generic.List(Of roAccessPeriodHolidays)

            Dim oRet As New Generic.List(Of roAccessPeriodHolidays)

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriodHolidays Where IDAccessPeriod = " & _ID

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAccessPeriodHoliday As roAccessPeriodHolidays = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAccessPeriodHoliday = New roAccessPeriodHolidays(oRow("IDAccessPeriod"), oRow("Day"), oRow("Month"), oRow("BeginTime"), oRow("EndTime"), _State)
                        oRet.Add(oAccessPeriodHoliday)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriodHolidays::GetAccessPeriodHolidaysList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriodHolidays::GetAccessPeriodHolidaysList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessPeriodHolidaysDataTable(ByVal _ID As Integer, ByVal _State As roAccessPeriodState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriodHolidays Where IDAccessPeriod = " & _ID

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriodHolidays::GetAccessPeriodHolidaysDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriodHolidays::GetAccessPeriodHolidaysDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace