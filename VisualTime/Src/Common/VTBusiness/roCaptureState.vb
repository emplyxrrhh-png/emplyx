Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Capture

    Public Class roCaptureState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CaptureResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Capture", "CapturesService")
            Me.intResult = CaptureResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Capture", "CapturesService", _IDPassport)
            Me.intResult = CaptureResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Capture", "CapturesService", _IDPassport, _Context)
            Me.intResult = CaptureResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Capture", "CapturesService", _IDPassport, , _ClientAddress)
            Me.intResult = CaptureResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Capture", "CapturesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CaptureResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CaptureResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CaptureResultEnum)
                Me.intResult = value
                If Me.intResult <> CaptureResultEnum.NoError And Me.intResult <> CaptureResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CaptureResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CaptureResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CaptureResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace