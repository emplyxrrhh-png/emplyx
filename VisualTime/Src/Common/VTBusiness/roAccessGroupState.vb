Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace AccessGroup

    <DataContract()>
    Public Class roAccessGroupState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AccessGroupResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.AccessGroup", "AccessGroupsService")
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.AccessGroup", "AccessGroupsService", _IDPassport)
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.AccessGroup", "AccessGroupsService", _IDPassport, _Context)
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessGroup", "AccessGroupsService", _IDPassport, , _ClientAddress)
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessGroup", "AccessGroupsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As AccessGroupResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AccessGroupResultEnum)
                Me.intResult = value
                If Me.intResult <> AccessGroupResultEnum.NoError And Me.intResult <> AccessGroupResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = AccessGroupResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessGroupResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessGroupResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace