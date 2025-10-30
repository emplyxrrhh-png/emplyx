Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTSurveys

    Public Class roSurveyState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As SurveyResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roSurvey", "SurveyService")
            Me.intResult = SurveyResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roSurvey", "SurveyService", _IDPassport)
            Me.intResult = SurveyResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roSurvey", "SurveyService", _IDPassport, _Context)
            Me.intResult = SurveyResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roSurvey", "SurveyService", _IDPassport, , _ClientAddress)
            Me.intResult = SurveyResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roSurvey", "SurveyService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = SurveyResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As SurveyResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As SurveyResultEnum)
                Me.intResult = value
                If Me.intResult <> SurveyResultEnum.NoError And Me.intResult <> SurveyResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("SurveyResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = SurveyResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SurveyResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SurveyResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace