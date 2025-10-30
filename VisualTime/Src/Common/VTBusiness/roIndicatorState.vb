Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace Indicator

    <DataContract()>
    Public Class roIndicatorState

        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As IndicatorResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Indicator", "IndicatorService")
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Indicator", "IndicatorService", _IDPassport)
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Indicator", "IndicatorService", _IDPassport, _Context)
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Indicator", "IndicatorService", _IDPassport, , _ClientAddress)
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Indicator", "IndicatorService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As IndicatorResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As IndicatorResultEnum)
                Me.intResult = value
                If Me.intResult <> IndicatorResultEnum.NoError And Me.intResult <> IndicatorResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = IndicatorResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = IndicatorResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = IndicatorResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace