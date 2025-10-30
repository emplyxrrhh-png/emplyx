Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Assignment

    Public Class roAssignmentState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AssignmentResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Assignment", "AssignmentService")
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Assignment", "AssignmentService", _IDPassport)
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Assignment", "AssignmentService", _IDPassport, _Context)
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Assignment", "AssignmentService", _IDPassport, , _ClientAddress)
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Assignment", "AssignmentService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As AssignmentResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AssignmentResultEnum)
                Me.intResult = value
                If Me.intResult <> AssignmentResultEnum.NoError And Me.intResult <> AssignmentResultEnum.Exception Then
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
            Me.intResult = AssignmentResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AssignmentResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AssignmentResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace