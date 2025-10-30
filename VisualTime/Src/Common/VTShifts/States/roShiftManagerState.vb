Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Shifts

    Public Class roShiftManagerState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ShiftResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Shift", "ShiftsService")
            Me.intResult = ShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Shift", "ShiftsService", _IDPassport)
            Me.intResult = ShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Shift", "ShiftsService", _IDPassport, _Context)
            Me.intResult = ShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Shift", "ShiftsService", _IDPassport, , _ClientAddress)
            Me.intResult = ShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Shift", "ShiftsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ShiftResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ShiftResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ShiftResultEnum)
                Me.intResult = value
                If Me.intResult <> ShiftResultEnum.NoError And Me.intResult <> ShiftResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ShiftResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ShiftResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ShiftResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace