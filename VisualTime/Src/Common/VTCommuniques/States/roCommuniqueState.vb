Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTCommuniques

    Public Class roCommuniqueState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CommuniqueResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCommunique", "CommuniqueService")
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCommunique", "CommuniqueService", _IDPassport)
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCommunique", "CommuniqueService", _IDPassport, _Context)
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCommunique", "CommuniqueService", _IDPassport, , _ClientAddress)
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCommunique", "CommuniqueService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CommuniqueResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CommuniqueResultEnum)
                Me.intResult = value
                If Me.intResult <> CommuniqueResultEnum.NoError And Me.intResult <> CommuniqueResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("CommuniqueResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CommuniqueResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CommuniqueResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CommuniqueResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace