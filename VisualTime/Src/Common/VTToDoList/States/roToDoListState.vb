Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTToDoLists

    Public Class roToDoListState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ToDoListResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roToDoList", "ToDoListService")
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roToDoList", "ToDoListService", _IDPassport)
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roToDoList", "ToDoListService", _IDPassport, _Context)
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roToDoList", "ToDoListService", _IDPassport, , _ClientAddress)
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roToDoList", "ToDoListService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ToDoListResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ToDoListResultEnum)
                Me.intResult = value
                If Me.intResult <> ToDoListResultEnum.NoError And Me.intResult <> ToDoListResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ToDoListResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ToDoListResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ToDoListResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ToDoListResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace