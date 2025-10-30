Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTBots

    Public Class roBotRuleState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BotRuleResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roBotRule", "BotsService")
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roBotRule", "BotsService", _IDPassport)
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roBotRule", "BotsService", _IDPassport, _Context)
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBotRule", "BotsService", _IDPassport, , _ClientAddress)
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBotRule", "BotsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BotRuleResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BotRuleResultEnum)
                Me.intResult = value
                If Me.intResult <> BotRuleResultEnum.NoError And Me.intResult <> BotRuleResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("BotRuleResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BotRuleResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BotRuleResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BotRuleResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace