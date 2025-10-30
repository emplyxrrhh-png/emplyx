Imports Robotics.Security.Base

Namespace Visits

    ''' <summary>
    ''' Business Class with database context on Visists
    ''' </summary>
    Public Class roVisitsState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            SqlError

        End Enum

        Private intResult As ResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Visits", "VisitsService")
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Visits", "VisitsService", _IDPassport)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Visits", "VisitsService", _IDPassport, _Context)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visits", "VisitsService", _IDPassport, , _ClientAddress)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visits", "VisitsService", _IDPassport, _Context)
            Me.ClientAddress = _ClientAddress
            Me.intResult = ResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ResultEnum)
                Me.intResult = value
                If Me.intResult <> ResultEnum.NoError And Me.intResult <> ResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace