Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace UserTask

    <DataContract>
    Public Class roUserTaskState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As UserTaskResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.UserTask", "UserTasksService")
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.UserTask", "UserTasksService", _IDPassport)
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.UserTask", "UserTasksService", _IDPassport, _Context)
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.UserTask", "UserTasksService", _IDPassport, , _ClientAddress)
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.UserTask", "UserTasksService", _IDPassport, _Context)
            Me.ClientAddress = _ClientAddress
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property Result() As UserTaskResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As UserTaskResultEnum)
                Me.intResult = value
                If Me.intResult <> UserTaskResultEnum.NoError And Me.intResult <> UserTaskResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = UserTaskResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = UserTaskResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = UserTaskResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace