Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTCalendar

    Public Class roCalendarRowHourDataState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CalendarRowHourDataResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService")
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport)
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context)
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, , _ClientAddress)
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CalendarRowHourDataResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CalendarRowHourDataResultEnum)
                Me.intResult = value
                If Me.intResult <> CalendarRowHourDataResultEnum.NoError And Me.intResult <> CalendarRowHourDataResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("CalendarRowHourDataResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = CalendarRowHourDataResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarRowHourDataResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarRowHourDataResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace