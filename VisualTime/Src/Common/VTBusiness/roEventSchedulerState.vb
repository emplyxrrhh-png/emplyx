Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace EventScheduler

    Public Class roEventSchedulerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As EventSchedulerResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Event", "EventsService")
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Event", "EventsService", _IDPassport)
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Event", "EventsService", _IDPassport, _Context)
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Event", "EventsService", _IDPassport, , _ClientAddress)
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Event", "EventsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As EventSchedulerResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As EventSchedulerResultEnum)
                Me.intResult = value
                If Me.intResult <> EventSchedulerResultEnum.NoError And Me.intResult <> EventSchedulerResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = EventSchedulerResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = EventSchedulerResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = EventSchedulerResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace