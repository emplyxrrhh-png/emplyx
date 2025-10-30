Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace AccessMove

    Public Class roAccessMoveState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AccessMoveResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.AccessMove", "AccessMovesService")
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.AccessMove", "AccessMovesService", _IDPassport)
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.AccessMove", "AccessMovesService", _IDPassport, _Context)
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessMove", "AccessMovesService", _IDPassport, , _ClientAddress)
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessMove", "AccessMovesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As AccessMoveResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AccessMoveResultEnum)
                Me.intResult = value
                If Me.intResult <> AccessMoveResultEnum.NoError And Me.intResult <> AccessMoveResultEnum.Exception Then
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
            Me.intResult = AccessMoveResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessMoveResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessMoveResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace