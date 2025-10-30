Imports System.Data.Common
Imports System.Text
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper

Namespace Visitors

    Public Class roVisitors

#Region "Declarations - Constructor"

        Public Const TypePunchIn As String = "IN"
        Public Const TypePunchOut As String = "OUT"

        Private oState As roVisitorsState

        Public Sub New()
            Me.oState = New roVisitorsState()
        End Sub

        Public Sub New(ByVal _State As roVisitorsState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _ID As String, ByVal _State As roVisitorsState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roVisitorsState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roVisitorsState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Update status visit
        ''' </summary>
        ''' <returns></returns>
        Public Function InsertPunch(ByVal VisitId As Guid, TypePunch As String) As Boolean

            Dim vResult As Boolean = True
            Dim vQuery As New StringBuilder

            Try
                'create connection

                'create query with parameters
                vQuery.Append("@INSERT# INTO Visit_Visitor_Punch(IDVisit,IDVisitor,DatePunch,Action) ")
                vQuery.Append("(@SELECT# @VisitId,vt.IDVisitor,getdate(),@Action from Visit_Visitor as vt where vt.IDVisit=@VisitId)")
                Dim vParameters = New List(Of CommandParameter) From
                {
                    New CommandParameter("@VisitId", CommandParameter.ParameterType.tString, VisitId.ToString()),
                    New CommandParameter("@Action", CommandParameter.ParameterType.tString, TypePunch)
                }
                'execute query
                vResult = ExecuteSql(vQuery.ToString(), vParameters)
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roVisitors::InsertPunch")
                vResult = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roVisitors::InsertPunch")
                vResult = False
            Finally

                vQuery.Clear()
            End Try

            Return vResult
        End Function

#End Region

    End Class

End Namespace