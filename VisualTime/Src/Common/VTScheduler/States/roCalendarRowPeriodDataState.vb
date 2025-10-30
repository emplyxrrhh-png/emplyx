Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTCalendar

    Public Class roCalendarRowPeriodDataState
        Inherits roBusinessState

        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            NoContract
            InvalidData
        End Enum

#Region "Declarations - Constructor"

        Private intResult As ResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService")
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, , _ClientAddress)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCalendar", "CalendarService", _IDPassport, _Context, _ClientAddress)
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

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

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