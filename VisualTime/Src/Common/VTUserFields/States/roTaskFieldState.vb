Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace UserFields

    Public Class roTaskFieldState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As TaskFieldResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Group", "GroupService")
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport)
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, _Context)
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, , _ClientAddress)
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As TaskFieldResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As TaskFieldResultEnum)
                Me.intResult = value
                If Me.intResult <> TaskFieldResultEnum.NoError And Me.intResult <> TaskFieldResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = TaskFieldResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TaskFieldResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TaskFieldResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace