Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Move

    Public Class roMoveState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As MoveResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Move", "MovesService")
            Me.intResult = MoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Move", "MovesService", _IDPassport)
            Me.intResult = MoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Move", "MovesService", _IDPassport, _Context)
            Me.intResult = MoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Move", "MovesService", _IDPassport, , _ClientAddress)
            Me.intResult = MoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Move", "MovesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = MoveResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As MoveResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As MoveResultEnum)
                Me.intResult = value
                If Me.intResult <> MoveResultEnum.NoError And Me.intResult <> MoveResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = MoveResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = MoveResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = MoveResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace