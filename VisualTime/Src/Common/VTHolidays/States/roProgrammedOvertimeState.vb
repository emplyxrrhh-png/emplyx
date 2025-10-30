Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace VTHolidays

    Public Class roProgrammedOvertimeState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As OvertimeResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roProgrammedOvertime", "ProgrammedOvertimeService")
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roProgrammedOvertime", "ProgrammedOvertimeService", _IDPassport)
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roProgrammedOvertime", "ProgrammedOvertimeService", _IDPassport, _Context)
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProgrammedOvertime", "ProgrammedOvertimeService", _IDPassport, , _ClientAddress)
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProgrammedOvertime", "ProgrammedOvertimeService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As OvertimeResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As OvertimeResultEnum)
                Me.intResult = value
                If Me.intResult <> OvertimeResultEnum.NoError And Me.intResult <> OvertimeResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("OvertimeResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = OvertimeResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = OvertimeResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = OvertimeResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace