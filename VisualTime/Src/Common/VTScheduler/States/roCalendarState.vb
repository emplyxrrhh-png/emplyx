Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTCalendar

    Public Class roCalendarState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CalendarV2ResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService")
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport)
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context)
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, , _ClientAddress)
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CalendarV2ResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CalendarV2ResultEnum)
                Me.intResult = value
                If Me.intResult <> CalendarV2ResultEnum.NoError And Me.intResult <> CalendarV2ResultEnum.Exception Then
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
            Me.intResult = CalendarV2ResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarV2ResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = CalendarV2ResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace