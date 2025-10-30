Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace Task

    Public Class roTaskState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As TaskResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Task", "TaskService")
            Me.intResult = TaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport)
            Me.intResult = TaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, _Context)
            Me.intResult = TaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, , _ClientAddress)
            Me.intResult = TaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Task", "TaskService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = TaskResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As TaskResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As TaskResultEnum)
                Me.intResult = value
                If Me.intResult <> TaskResultEnum.NoError And Me.intResult <> TaskResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = TaskResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TaskResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TaskResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace