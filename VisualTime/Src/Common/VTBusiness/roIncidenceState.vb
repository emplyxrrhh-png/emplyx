Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Incidence

    Public Class roIncidenceState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As IncidenceResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Incidence", "IncidencesService")
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Incidence", "IncidencesService", _IDPassport)
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Incidence", "IncidencesService", _IDPassport, _Context)
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Incidence", "IncidencesService", _IDPassport, , _ClientAddress)
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Incidence", "IncidencesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As IncidenceResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As IncidenceResultEnum)
                Me.intResult = value
                If Me.intResult <> IncidenceResultEnum.NoError And Me.intResult <> IncidenceResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = IncidenceResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = IncidenceResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = IncidenceResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace