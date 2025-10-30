Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Zone

    Public Enum eWeekDay
        <EnumMember> Monday = 1
        <EnumMember> Tuesday = 2
        <EnumMember> Wednesday = 3
        <EnumMember> Thursday = 4
        <EnumMember> Friday = 5
        <EnumMember> Saturday = 6
        <EnumMember> Sunday = 7
    End Enum

    <DataContract()>
    Public Class roZoneInactivity

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private intIDZone As Integer
        Private iWeekDay As eWeekDay
        Private dBegin As Date
        Private dEnd As Date

        Public Sub New()
            Me.oState = New roZoneState()
            Me.intIDZone = -1
        End Sub

        Public Sub New(ByVal _IDZone As Integer, ByVal _WeekDay As eWeekDay, ByVal _Begin As Date, ByVal _End As Date, ByVal _State As roZoneState)
            Me.oState = _State
            Me.intIDZone = _IDZone
            Me.iWeekDay = _WeekDay
            Me.dBegin = _Begin
            Me.dEnd = _End
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roZoneState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roZoneState)
                Me.oState = value
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
        Public Property WeekDay() As eWeekDay
            Get
                Return Me.iWeekDay
            End Get
            Set(ByVal value As eWeekDay)
                Me.iWeekDay = value
            End Set
        End Property

        <DataMember()>
        Public Property Begin() As Date
            Get
                Return Me.dBegin
            End Get
            Set(ByVal value As Date)
                Me.dBegin = value
            End Set
        End Property

        <DataMember()>
        Public Property [End]() As Date
            Get
                Return Me.dEnd
            End Get
            Set(ByVal value As Date)
                Me.dEnd = value
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

                Dim tb As New DataTable("Zones")
                Dim strSQL As String = "@SELECT# * FROM ZonesInactivity WHERE IDZone = " & Me.intIDZone.ToString & " And WeekDay = " & Me.iWeekDay & " And [Begin] = " & roTypes.Any2Time(Me.dBegin).SQLSmallDateTime &
                                        " ORDER BY WeekDay"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDZone") = Me.IDZone
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("WeekDay") = Me.iWeekDay
                oRow("Begin") = Me.dBegin
                oRow("End") = Me.dEnd

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)
                bolRet = True

                oAuditDataNew = oRow

                If bAudit Then
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String = ""
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tZoneInactivity, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZoneInactivity::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZoneInactivity::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try
                Dim DeleteQuerys As String() = {"@DELETE# FROM ZonesInactivity WHERE IDZone = " & Me.intIDZone.ToString & " And WeekDay = " & Me.iWeekDay & " And [Begin] = " & roTypes.Any2Time(Me.dBegin).SQLSmallDateTime & " And [End] = " & roTypes.Any2Time(Me.dEnd).SQLSmallDateTime}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tZoneInactivity, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZoneInactivity::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZoneInactivity::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetZonesInactivityList(ByVal _IDZone As Integer, ByVal _State As roZoneState) As Generic.List(Of roZoneInactivity)

            Dim oRet As New Generic.List(Of roZoneInactivity)

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonesInactivity Where IDZone = " & _IDZone & " ORDER BY WeekDay"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oZoneInactivity As roZoneInactivity = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oZoneInactivity = New roZoneInactivity(oRow("IDZone"), oRow("WeekDay"), oRow("Begin"), oRow("End"), _State)
                        oRet.Add(oZoneInactivity)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZoneInactivity::GetZonesInactivityList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneInactivity::GetZonesInactivityList")
            End Try

            Return oRet

        End Function

        Public Shared Function GetZonesInactivityDataTable(ByVal _IDZone As Integer, ByVal _State As roZoneState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonesInactivity Where IDZone = " & _IDZone & " ORDER BY WeekDay"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZoneInactivity::GetZonesInactivityDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneInactivity::GetZonesInactivityDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function SaveZoneInactivities(ByVal _IDZone As Integer, ByVal _Inactivities As Generic.List(Of roZoneInactivity), ByVal _State As roZoneState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ExcludeWeekDay As New Generic.List(Of Integer)
                Dim ExcludeBegin As New Generic.List(Of Date)

                bolRet = True

                If _Inactivities IsNot Nothing AndAlso _Inactivities.Count > 0 Then
                    For Each oInactivity As roZoneInactivity In _Inactivities
                        bolRet = oInactivity.Save()
                        ExcludeWeekDay.Add(oInactivity.WeekDay)
                        ExcludeBegin.Add(oInactivity.Begin)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    Dim strSQL As String = "@DELETE# FROM ZonesInactivity WHERE IDZone = " & _IDZone.ToString
                    If ExcludeWeekDay.Count > 0 AndAlso ExcludeWeekDay.Count = ExcludeBegin.Count Then
                        For n As Integer = 0 To ExcludeWeekDay.Count - 1
                            strSQL &= " AND NOT ([WeekDay] = " & ExcludeWeekDay(n) & " AND [Begin] = " & roTypes.Any2Time(ExcludeBegin(n)).SQLSmallDateTime & ")"
                        Next
                    End If
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZoneInactivity::SaveZoneInactivities")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneInactivity::SaveZoneInactivities")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace