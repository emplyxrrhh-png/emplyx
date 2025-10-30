Imports System.Data.Common
Imports System.Text
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper

Namespace Visits

    ''' <summary>
    ''' Business Class for manage Visists
    ''' </summary>
    Public Class roVisits

#Region "Declarations - Constructor"

        Public Const ConfigurationZoneKey As String = "vst_ZoneValue"
        Public Const ConfigurationCardNumberKey As String = "vst_CardNumberField"

        Public Const StatusVisitScheduled As Integer = 0
        Public Const StatusVisitActive As Integer = 1
        Public Const StatusVisitCompleted As Integer = 2
        Public Const StatusVisitNotPresented As Integer = 3

        Public Const StatusDelete As Integer = 0

        Private oState As roVisitsState

        Public Sub New()
            Me.oState = New roVisitsState()
        End Sub

        Public Sub New(ByVal _State As roVisitsState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _ID As String, ByVal _State As roVisitsState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roVisitsState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roVisitsState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Update status visit
        ''' </summary>
        ''' <returns></returns>
        Public Function UpdateStatus(ByVal VisitId As Guid, Status As Integer) As Boolean

            Dim vResult As Boolean = True

            Try
                'create connection

                'create query with parameters
                Dim vQuery = "@UPDATE# Visit set Status=@status, Modified=getdate() where IDVisit=@visitId"
                Dim vParameters = New List(Of CommandParameter) From
                {
                    New CommandParameter("@status", CommandParameter.ParameterType.tInt, Status),
                    New CommandParameter("@visitId", CommandParameter.ParameterType.tString, VisitId.ToString())
                }
                'execute query
                vResult = ExecuteSql(vQuery, vParameters)
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roVisits::UpdateStatus")
                vResult = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roVisits::UpdateStatus")
                vResult = False
            Finally

            End Try

            Return vResult
        End Function

        ''' <summary>
        ''' Get id zone for autoclose visits
        ''' </summary>
        ''' <returns></returns>
        Public Function FindByEmployeeId(ByVal IdEmployee As Integer) As Guid

            Dim vResultVisitGuid As Guid = Guid.Empty
            Dim vResult As String
            Dim vCardNumberConfiguration As String
            Dim vQuery As New StringBuilder

            Try
                'create connection

                'get car number configuration
                vCardNumberConfiguration = GetConfiguration(ConfigurationCardNumberKey)

                'create query with parameters
                vQuery.Append("@SELECT# top 1 v.idVisit from [Visit] as v ")
                vQuery.Append("inner join [Visit_Fields_Value] as vfv on v.IDVisit=vfv.IDVisit and v.Deleted=@deleted and v.Status=@status ")
                vQuery.Append("inner join [Visit_Fields] as vf on vfv.IDField=vf.IDField and vf.Name=@cardNumberField ")
                vQuery.Append("inner join sysroPassports_AuthenticationMethods as pa on vfv.Value=pa.[Credential] ")
                vQuery.Append("inner join sysroPassports as p on pa.IDPassport=p.ID and p.IDEmployee=@EmployeeId ")
                vQuery.Append("order by v.BeginDate desc")

                Dim vParameters = New List(Of CommandParameter) From
                {
                    New CommandParameter("@deleted", CommandParameter.ParameterType.tInt, StatusDelete),
                    New CommandParameter("@status", CommandParameter.ParameterType.tInt, StatusVisitActive),
                    New CommandParameter("@cardNumberField", CommandParameter.ParameterType.tString, vCardNumberConfiguration),
                    New CommandParameter("@EmployeeId", CommandParameter.ParameterType.tInt, IdEmployee)
                }

                'execute query & get result
                vResult = ExecuteScalar(vQuery.ToString(), vParameters)
                If (Not vResult Is Nothing) Then
                    vResultVisitGuid = Guid.Parse(vResult.ToString())
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roVisits::FindByEmployeeId")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roVisits::FindByEmployeeId")
            Finally

                vQuery.Clear()
            End Try

            Return vResultVisitGuid
        End Function

        ''' <summary>
        ''' Get id zone for autoclose visits
        ''' </summary>
        ''' <returns></returns>
        Public Function GetConfiguration(ByVal Configuration As String) As String

            Dim vResult As String
            Dim vResultConfiguration As String = Nothing

            Try
                'create connection

                'create query with parameters
                Dim vQuery = "@SELECT# Value from sysroLiveAdvancedParameters where ParameterName=@parametername"
                Dim vParameters = New List(Of CommandParameter) From
                {
                    New CommandParameter("@parametername", CommandParameter.ParameterType.tString, Configuration)
                }
                'execute query
                vResult = ExecuteScalar(vQuery, vParameters)
                If (Not vResult Is Nothing) Then
                    vResultConfiguration = vResult
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roVisits::GetConfiguration")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roVisits::GetConfiguration")
            Finally

            End Try

            Return vResultConfiguration
        End Function

#End Region

    End Class

End Namespace