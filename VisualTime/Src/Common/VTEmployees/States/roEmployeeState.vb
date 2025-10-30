Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Employee

    Public Class roEmployeeState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As EmployeeResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Employee", "EmployeeService")
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Employee", "EmployeeService", _IDPassport)
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Employee", "EmployeeService", _IDPassport, _Context)
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Employee", "EmployeeService", _IDPassport, , _ClientAddress)
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Employee", "EmployeeService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As EmployeeResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As EmployeeResultEnum)
                Me.intResult = value
                If Me.intResult <> EmployeeResultEnum.NoError And Me.intResult <> EmployeeResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = EmployeeResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = EmployeeResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = EmployeeResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace