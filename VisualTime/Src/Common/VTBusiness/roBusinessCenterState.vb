Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace BusinessCenter

    Public Class roBusinessCenterState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BusinessCenterResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.BusinessCenter", "TaskService")
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.BusinessCenter", "TaskService", _IDPassport)
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.BusinessCenter", "TaskService", _IDPassport, _Context)
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.BusinessCenter", "TaskService", _IDPassport, , _ClientAddress)
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.BusinessCenter", "TaskService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BusinessCenterResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BusinessCenterResultEnum)
                Me.intResult = value
                If Me.intResult <> BusinessCenterResultEnum.NoError And Me.intResult <> BusinessCenterResultEnum.Exception Then
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
            Me.intResult = BusinessCenterResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BusinessCenterResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BusinessCenterResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace