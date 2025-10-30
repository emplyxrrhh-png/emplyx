Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTWebLinks

    Public Class roWebLinksManagerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As WebLinksResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.CostCenter", "CostCenterService")
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.CostCenter", "CostCenterService", _IDPassport)
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.CostCenter", "CostCenterService", _IDPassport, _Context)
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.CostCenter", "CostCenterService", _IDPassport, , _ClientAddress)
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.CostCenter", "CostCenterService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property Result() As WebLinksResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As WebLinksResultEnum)
                Me.intResult = value
                If Me.intResult <> WebLinksResultEnum.NoError And Me.intResult <> WebLinksResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = WebLinksResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = WebLinksResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = WebLinksResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace