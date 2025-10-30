Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Public Class roSDKState
    Inherits roBusinessState

#Region "Declarations - Constructor"

    Private intResult As SDKResultEnum

    Public Sub New()
        MyBase.New("Robotics.Base.DTOs.roSDK", "SDKService")
        Me.intResult = SDKResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer)
        MyBase.New("Robotics.Base.DTOs.roSDK", "SDKService", _IDPassport)
        Me.intResult = SDKResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
        MyBase.New("Robotics.Base.DTOs.roSDK", "SDKService", _IDPassport, _Context)
        Me.intResult = SDKResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
        MyBase.New("Robotics.Base.DTOs.roSDK", "SDKService", _IDPassport, , _ClientAddress)
        Me.intResult = SDKResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
        MyBase.New("Robotics.Base.DTOs.roSDK", "SDKService", _IDPassport, _Context, _ClientAddress)
        Me.intResult = SDKResultEnum.NoError
    End Sub

#End Region

#Region "Properties"

    Public Property Result() As SDKResultEnum
        Get
            Return Me.intResult
        End Get
        Set(ByVal value As SDKResultEnum)
            Me.intResult = value
            If Me.intResult <> SDKResultEnum.NoError And Me.intResult <> SDKResultEnum.Exception Then
                Me.ErrorText = Me.Language.Translate("SDKResultEnum." & Me.intResult.ToString, "")
            End If
        End Set
    End Property

#End Region

#Region "Methods"

    'Public Overrides Sub UpdateAccessTime()

    'End Sub

    Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
        MyBase.UpdateStateInfo(_Context)
        Me.intResult = SDKResultEnum.NoError
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = SDKResultEnum.Exception
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = SDKResultEnum.Exception
    End Sub

#End Region

End Class