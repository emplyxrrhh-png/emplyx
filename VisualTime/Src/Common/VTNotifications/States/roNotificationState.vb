Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.base

Namespace Notifications

    <XmlType(Namespace:="http://localhost", TypeName:="roNotificationState")>
    <DataContract()>
    Public Class roNotificationState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As NotificationResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Notification", "NotificationsService")
            Me.intResult = NotificationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Notification", "NotificationsService", _IDPassport)
            Me.intResult = NotificationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Notification", "NotificationsService", _IDPassport, _Context)
            Me.intResult = NotificationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Notification", "NotificationsService", _IDPassport, , _ClientAddress)
            Me.intResult = NotificationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Notification", "NotificationsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = NotificationResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As NotificationResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As NotificationResultEnum)
                Me.intResult = value
                If Me.intResult <> NotificationResultEnum.NoError And Me.intResult <> NotificationResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = NotificationResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = NotificationResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = NotificationResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace