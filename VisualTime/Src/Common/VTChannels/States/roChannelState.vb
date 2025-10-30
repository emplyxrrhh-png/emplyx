Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTChannels

    Public Class roChannelState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ChannelResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roChannel", "ChannelsService")
            Me.intResult = ChannelResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roChannel", "ChannelsService", _IDPassport)
            Me.intResult = ChannelResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roChannel", "ChannelsService", _IDPassport, _Context)
            Me.intResult = ChannelResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roChannel", "ChannelsService", _IDPassport, , _ClientAddress)
            Me.intResult = ChannelResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roChannel", "ChannelsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ChannelResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ChannelResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ChannelResultEnum)
                Me.intResult = value
                If Me.intResult <> ChannelResultEnum.NoError And Me.intResult <> ChannelResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ChannelResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ChannelResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ChannelResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ChannelResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace