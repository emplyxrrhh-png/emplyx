Imports System.Runtime.Serialization
Imports Robotics.Security.Base

Namespace Zone

    <DataContract()>
    Public Class roZoneState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As DTOs.ZoneResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Zone", "ZonesService")
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Zone", "ZonesService", _IDPassport)
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Zone", "ZonesService", _IDPassport, _Context)
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Zone", "ZonesService", _IDPassport, , _ClientAddress)
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Zone", "ZonesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As DTOs.ZoneResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As DTOs.ZoneResultEnum)
                Me.intResult = value
                If Me.intResult <> DTOs.ZoneResultEnum.NoError And Me.intResult <> DTOs.ZoneResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = DTOs.ZoneResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DTOs.ZoneResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DTOs.ZoneResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace