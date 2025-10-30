Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Causes

    Public Class roCauseManagerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CauseResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Cause", "CausesService")
            Me.intResult = CauseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Cause", "CausesService", _IDPassport)
            Me.intResult = CauseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Cause", "CausesService", _IDPassport, _Context)
            Me.intResult = CauseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Cause", "CausesService", _IDPassport, , _ClientAddress)
            Me.intResult = CauseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Cause", "CausesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CauseResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property Result() As CauseResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CauseResultEnum)
                Me.intResult = value
                If Me.intResult <> CauseResultEnum.NoError And Me.intResult <> CauseResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CauseResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CauseResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CauseResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace