Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Analytics.Manager

    Public Class roAnalyticState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AnalyticsResultEnum

        Public Sub New()
            MyBase.New("Business.Report", "ReportsService")
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Business.Report", "ReportsService", _IDPassport)
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Business.Report", "ReportsService", _IDPassport, _Context)
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Business.Report", "ReportsService", _IDPassport, , _ClientAddress)
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Business.Report", "ReportsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As AnalyticsResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AnalyticsResultEnum)
                Me.intResult = value
                If Me.intResult <> AnalyticsResultEnum.NoError And Me.intResult <> AnalyticsResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, String.Empty)
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = AnalyticsResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AnalyticsResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AnalyticsResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace