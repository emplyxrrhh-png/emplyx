Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace VTDailyRecord

    Public Class roDailyRecordState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As CommuniqueResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roDailyRecord", "DailyRecordService")
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roDailyRecord", "DailyRecordService", _IDPassport)
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roDailyRecord", "DailyRecordService", _IDPassport, _Context)
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDailyRecord", "DailyRecordService", _IDPassport, , _ClientAddress)
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDailyRecord", "DailyRecordService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As DailyRecordResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As DailyRecordResultEnum)
                Me.intResult = value
                If Me.intResult <> DailyRecordResultEnum.NoError And Me.intResult <> DailyRecordResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("DailyRecordResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = DailyRecordResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DailyRecordResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DailyRecordResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace