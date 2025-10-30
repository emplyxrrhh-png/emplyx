Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Report

    Public Class roReportState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CrystalReportsResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Report", "ReportsService")
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Report", "ReportsService", _IDPassport)
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Report", "ReportsService", _IDPassport, _Context)
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Report", "ReportsService", _IDPassport, , _ClientAddress)
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Report", "ReportsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CrystalReportsResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CrystalReportsResultEnum)
                Me.intResult = value
                If Me.intResult <> CrystalReportsResultEnum.NoError And Me.intResult <> CrystalReportsResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CrystalReportsResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CrystalReportsResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CrystalReportsResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace