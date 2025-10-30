Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTChannels

    Public Class roConversationState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ConversationResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roConversation", "ChannelsService")
            Me.intResult = ConversationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roConversation", "ChannelsService", _IDPassport)
            Me.intResult = ConversationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roConversation", "ChannelsService", _IDPassport, _Context)
            Me.intResult = ConversationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roConversation", "ChannelsService", _IDPassport, , _ClientAddress)
            Me.intResult = ConversationResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roConversation", "ChannelsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ConversationResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ConversationResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ConversationResultEnum)
                Me.intResult = value
                If Me.intResult <> ConversationResultEnum.NoError And Me.intResult <> ConversationResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ConversationResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ConversationResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ConversationResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ConversationResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace