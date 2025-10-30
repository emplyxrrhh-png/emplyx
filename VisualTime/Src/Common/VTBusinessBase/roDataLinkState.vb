Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

<DataContract>
Public Class roDataLinkState
    Inherits roBusinessState

#Region "Declarations - Constructor"

    Private intResult As DataLinkResultEnum

    Public Sub New()
        MyBase.New("VTBusiness.DataLink", "DataLinkService")
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer)
        MyBase.New("VTBusiness.DataLink", "DataLinkService", _IDPassport)
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
        MyBase.New("VTBusiness.DataLink", "DataLinkService", _IDPassport, _Context)
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
        MyBase.New("VTBusiness.DataLink", "DataLinkService", _IDPassport, , _ClientAddress)
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
        MyBase.New("VTBusiness.DataLink", "DataLinkService", _IDPassport, _Context, _ClientAddress)
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

#End Region

#Region "Properties"

    <IgnoreDataMember>
    Public Property Result() As DataLinkResultEnum
        Get
            Return Me.intResult
        End Get
        Set(ByVal value As DataLinkResultEnum)
            Me.intResult = value
            If Me.intResult <> DataLinkResultEnum.NoError And Me.intResult <> DataLinkResultEnum.Exception Then
                Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
            End If
        End Set
    End Property

#End Region

#Region "Methods"

    Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
        MyBase.UpdateStateInfo(_Context)
        Me.intResult = DataLinkResultEnum.NoError
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = DataLinkResultEnum.Exception
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = DataLinkResultEnum.Exception
    End Sub

#End Region

End Class