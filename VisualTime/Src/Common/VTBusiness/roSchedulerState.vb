Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace Scheduler

    <DataContract()>
    Public Class roSchedulerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As SchedulerResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Scheduler", "SchedulerService")
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Scheduler", "SchedulerService", _IDPassport)
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Scheduler", "SchedulerService", _IDPassport, _Context)
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Scheduler", "SchedulerService", _IDPassport, , _ClientAddress)
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Scheduler", "SchedulerService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As SchedulerResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As SchedulerResultEnum)
                Me.intResult = value
                If Me.intResult <> SchedulerResultEnum.NoError And Me.intResult <> SchedulerResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = SchedulerResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SchedulerResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SchedulerResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace