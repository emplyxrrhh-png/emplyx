Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace AdvancedParameter

    <DataContract()>
    Public Class roAdvancedParameterState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AdvancedParameterResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.AdvancedParameter", "AdvancedParameterService")
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.AdvancedParameter", "AdvancedParameterService", _IDPassport)
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.AdvancedParameter", "AdvancedParameterService", _IDPassport, _Context)
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AdvancedParameter", "AdvancedParameterService", _IDPassport, , _ClientAddress)
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AdvancedParameter", "AdvancedParameterService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property Result() As AdvancedParameterResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AdvancedParameterResultEnum)
                Me.intResult = value
                If Me.intResult <> AdvancedParameterResultEnum.NoError And Me.intResult <> AdvancedParameterResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = AdvancedParameterResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AdvancedParameterResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AdvancedParameterResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace