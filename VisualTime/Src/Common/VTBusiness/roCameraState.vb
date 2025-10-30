Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Camera

    <DataContract()>
    Public Class roCameraState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CameraResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Camera", "CamerasService")
            Me.intResult = CameraResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Camera", "CamerasService", _IDPassport)
            Me.intResult = CameraResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Camera", "CamerasService", _IDPassport, _Context)
            Me.intResult = CameraResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Camera", "CamerasService", _IDPassport, , _ClientAddress)
            Me.intResult = CameraResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Camera", "CamerasService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CameraResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As CameraResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CameraResultEnum)
                Me.intResult = value
                If Me.intResult <> CameraResultEnum.NoError And Me.intResult <> CameraResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CameraResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CameraResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CameraResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace