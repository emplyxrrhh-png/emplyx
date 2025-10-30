Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTChannels

    Public Class roLogBookState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As MessageResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roLogBook", "ChannelsService")
            Me.intResult = MessageResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roLogBook", "ChannelsService", _IDPassport)
            Me.intResult = MessageResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roLogBook", "ChannelsService", _IDPassport, _Context)
            Me.intResult = MessageResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roLogBook", "ChannelsService", _IDPassport, , _ClientAddress)
            Me.intResult = MessageResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roLogBook", "ChannelsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = MessageResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As LogBookResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As LogBookResultEnum)
                Me.intResult = value
                If Me.intResult <> LogBookResultEnum.NoError And Me.intResult <> LogBookResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("LogBookResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = LogBookResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = LogBookResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = MessageResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace