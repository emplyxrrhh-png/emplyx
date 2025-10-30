Imports Robotics.Base.DTOs

Namespace Base

    Public Class roSecurityCategoryState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As SecurityResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roSecurityCategory", "SecurityService")
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roSecurityCategory", "SecurityService", _IDPassport)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roSecurityCategory", "SecurityService", _IDPassport, _Context)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roSecurityCategory", "SecurityService", _IDPassport, , _ClientAddress)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roSecurityCategory", "SecurityService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As SecurityResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As SecurityResultEnum)
                Me.intResult = value
                If Me.intResult <> SecurityResultEnum.NoError AndAlso Me.intResult <> SecurityResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("SecurityResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SecurityResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SecurityResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace