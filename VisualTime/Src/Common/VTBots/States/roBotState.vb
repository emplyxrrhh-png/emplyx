Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTBots

    Public Class roBotState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BotResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roBot", "BotsService")
            Me.intResult = BotResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roBot", "BotsService", _IDPassport)
            Me.intResult = BotResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roBot", "BotsService", _IDPassport, _Context)
            Me.intResult = BotResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBot", "BotsService", _IDPassport, , _ClientAddress)
            Me.intResult = BotResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBot", "BotsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BotResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BotResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BotResultEnum)
                Me.intResult = value
                If Me.intResult <> BotResultEnum.NoError And Me.intResult <> BotResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("BotResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BotResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BotResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BotResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace