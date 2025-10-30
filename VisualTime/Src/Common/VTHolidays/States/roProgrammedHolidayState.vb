Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTHolidays

    Public Class roProgrammedHolidayState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As HolidayResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roProgrammedHoliday", "ProgrammedHolidayService")
            Me.intResult = HolidayResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roProgrammedHoliday", "ProgrammedHolidayService", _IDPassport)
            Me.intResult = HolidayResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roProgrammedHoliday", "ProgrammedHolidayService", _IDPassport, _Context)
            Me.intResult = HolidayResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProgrammedHoliday", "ProgrammedHolidayService", _IDPassport, , _ClientAddress)
            Me.intResult = HolidayResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProgrammedHoliday", "ProgrammedHolidayService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = HolidayResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As HolidayResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As HolidayResultEnum)
                Me.intResult = value
                If Me.intResult <> HolidayResultEnum.NoError And Me.intResult <> HolidayResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("HolidayResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = HolidayResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = HolidayResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = HolidayResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace