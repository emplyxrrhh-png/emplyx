Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTCalendar

    Public Class roCalendarShiftState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CalendarShiftResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService")
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport)
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context)
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, , _ClientAddress)
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CalendarShiftResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CalendarShiftResultEnum)
                Me.intResult = value
                If Me.intResult <> CalendarShiftResultEnum.NoError And Me.intResult <> CalendarShiftResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("CalendarShiftResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CalendarShiftResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarShiftResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarShiftResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace