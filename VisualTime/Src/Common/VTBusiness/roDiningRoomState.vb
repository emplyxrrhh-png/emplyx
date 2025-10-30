Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace DiningRoom

    <DataContract()>
    Public Class roDiningRoomState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As DiningRoomResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.DiningRoom", "DiningRoomService")
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.DiningRoom", "DiningRoomService", _IDPassport)
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.DiningRoom", "DiningRoomService", _IDPassport, _Context)
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.DiningRoom", "DiningRoomService", _IDPassport, , _ClientAddress)
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.DiningRoom", "DiningRoomService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As DiningRoomResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As DiningRoomResultEnum)
                Me.intResult = value
                If Me.intResult <> DiningRoomResultEnum.NoError And Me.intResult <> DiningRoomResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = DiningRoomResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DiningRoomResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DiningRoomResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace