Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace LabAgree

    Public Class roLabAgreeManagerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As LabAgreeResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.LabAgree", "LabAgreeService")
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.LabAgree", "LabAgreeService", _IDPassport)
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.LabAgree", "LabAgreeService", _IDPassport, _Context)
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.LabAgree", "LabAgreeService", _IDPassport, , _ClientAddress)
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.LabAgree", "LabAgreeService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As LabAgreeResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As LabAgreeResultEnum)
                Me.intResult = value
                If Me.intResult <> LabAgreeResultEnum.NoError And Me.intResult <> LabAgreeResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = LabAgreeResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = LabAgreeResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = LabAgreeResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace