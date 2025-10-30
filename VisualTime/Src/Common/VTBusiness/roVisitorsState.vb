Imports Robotics.Base.VTBusiness.Visits
Imports Robotics.Security.Base

Namespace Visitors

    ''' <summary>
    ''' Business Class with database context on Visitors
    ''' </summary>
    Public Class roVisitorsState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            SqlError

        End Enum

        Private intResult As roVisitorsState.ResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Visitors", "VisitorsService")
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Visitors", "VisitorsService", _IDPassport)
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Visitors", "VisitorsService", _IDPassport, _Context)
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visitors", "VisitorsService", _IDPassport, , _ClientAddress)
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visitors", "VisitorsService", _IDPassport, _Context)
            Me.ClientAddress = _ClientAddress
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As roVisitorsState.ResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As roVisitorsState.ResultEnum)
                Me.intResult = value
                If Me.intResult <> roVisitorsState.ResultEnum.NoError And Me.intResult <> roVisitsState.ResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = roVisitorsState.ResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = roVisitorsState.ResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = roVisitorsState.ResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace