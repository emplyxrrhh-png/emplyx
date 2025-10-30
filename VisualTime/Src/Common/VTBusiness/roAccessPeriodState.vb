Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace AccessPeriod

    Public Class roAccessPeriodState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As AccessPeriodResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.AccessPeriod", "AccessPeriodsService")
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.AccessPeriod", "AccessPeriodsService", _IDPassport)
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.AccessPeriod", "AccessPeriodsService", _IDPassport, _Context)
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessPeriod", "AccessPeriodsService", _IDPassport, , _ClientAddress)
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.AccessPeriod", "AccessPeriodsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As AccessPeriodResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AccessPeriodResultEnum)
                Me.intResult = value
                If Me.intResult <> AccessPeriodResultEnum.NoError And Me.intResult <> AccessPeriodResultEnum.Exception Then
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
            Me.intResult = AccessPeriodResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessPeriodResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AccessPeriodResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace