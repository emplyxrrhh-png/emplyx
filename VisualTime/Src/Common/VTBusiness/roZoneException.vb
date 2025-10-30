Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Zone

    <DataContract()>
    Public Class roZoneException

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private intIDZone As Integer
        Private dExceptionDate As Date

        Public Sub New()
            Me.oState = New roZoneState()
            Me.intIDZone = -1
        End Sub

        Public Sub New(ByVal _IDZone As Integer, ByVal _ExceptionDate As Date, ByVal _State As roZoneState)
            Me.oState = _State
            Me.intIDZone = _IDZone
            Me.dExceptionDate = _ExceptionDate
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
        Public Property ExceptionDate() As Date
            Get
                Return Me.dExceptionDate
            End Get
            Set(ByVal value As Date)
                Me.dExceptionDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Save() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("ZonesException")
                Dim strSQL As String = "@SELECT# * FROM ZonesException WHERE IDZone = " & Me.intIDZone.ToString & " And ExceptionDate = " & roTypes.Any2Time(Me.dExceptionDate).SQLSmallDateTime
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

                oRow("ExceptionDate") = Me.dExceptionDate

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)
                bolRet = True

                oAuditDataNew = oRow
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZoneException::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZoneException::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim DeleteQuerys() As String = {"@DELETE# FROM ZonesException WHERE IDZone = " & Me.intIDZone.ToString & " And ExceptionDate = " & roTypes.Any2Time(Me.dExceptionDate).SQLSmallDateTime}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZoneException::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZoneException::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetZonesExceptionList(ByVal IDZone As Integer, ByVal _State As roZoneState) As Generic.List(Of roZoneException)

            Dim oRet As New Generic.List(Of roZoneException)

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonesException Where IDZone = " & IDZone & "  ORDER BY ExceptionDate DESC"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oZoneException As roZoneException = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oZoneException = New roZoneException(oRow("IDZone"), oRow("ExceptionDate"), _State)
                        oRet.Add(oZoneException)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZoneException::GetZonesExceptionList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneException::GetZonesExceptionList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetZonesExceptionDataTable(ByVal IDZone As Integer, ByVal _State As roZoneState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonesException Where IDZone = " & IDZone & " ORDER BY ExceptionDate DESC"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZoneException::GetZonesExceptionDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneException::GetZonesExceptionDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function SaveZoneExceptions(ByVal _IDZone As Integer, ByVal _Exceptions As Generic.List(Of roZoneException), ByVal _State As roZoneState) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim xExclude As New Generic.List(Of Date)

                bolRet = True

                If _Exceptions IsNot Nothing AndAlso _Exceptions.Count > 0 Then
                    For Each oException As roZoneException In _Exceptions
                        bolRet = oException.Save()
                        xExclude.Add(oException.ExceptionDate)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    Dim strSQL As String = "@DELETE# FROM ZonesException WHERE IDZone = " & _IDZone.ToString
                    If xExclude.Count > 0 Then
                        For Each xDate As Date In xExclude
                            strSQL &= " AND ExceptionDate <> " & roTypes.Any2Time(xDate).SQLSmallDateTime
                        Next
                    End If
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZoneException::SaveZoneExceptions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneException::SaveZoneExceptions")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteZoneExceptions(ByVal _IDZone As Integer, ByVal _State As roZoneState, Optional ByVal ExcludeList As Generic.List(Of Date) = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM ZonesException WHERE IDZone = " & _IDZone.ToString
                If ExcludeList IsNot Nothing AndAlso ExcludeList.Count > 0 Then
                    For Each xExceptionDate As Date In ExcludeList
                        strSQL &= " AND ExceptionDate <> " & roTypes.Any2Time(xExceptionDate).SQLSmallDateTime
                    Next
                End If
                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZoneException::DeleteZoneExceptions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneException::DeleteZoneExceptions")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace