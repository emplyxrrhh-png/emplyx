Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Contract

    Public Class roContractState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ContractsResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Contract", "ContractsService")
            Me.intResult = ContractsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Contract", "ContractsService", _IDPassport)
            Me.intResult = ContractsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Contract", "ContractsService", _IDPassport, _Context)
            Me.intResult = ContractsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Contract", "ContractsService", _IDPassport, , _ClientAddress)
            Me.intResult = ContractsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Contract", "ContractsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ContractsResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ContractsResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ContractsResultEnum)
                Me.intResult = value
                If Me.intResult <> ContractsResultEnum.NoError And Me.intResult <> ContractsResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ContractsResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ContractsResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ContractsResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace