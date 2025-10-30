Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Punch

    Public Class roPunchState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As PunchResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Punch", "PunchService")
            Me.intResult = PunchResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Punch", "PunchService", _IDPassport)
            Me.intResult = PunchResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Punch", "PunchService", _IDPassport, _Context)
            Me.intResult = PunchResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Punch", "PunchService", _IDPassport, , _ClientAddress)
            Me.intResult = PunchResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Punch", "PunchService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = PunchResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As PunchResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As PunchResultEnum)
                Me.intResult = value
                If Me.intResult <> PunchResultEnum.NoError And Me.intResult <> PunchResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = PunchResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = PunchResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = PunchResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace