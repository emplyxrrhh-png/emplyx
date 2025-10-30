Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class roServiceApiManagerState
    Inherits roBaseState

    Private oLanguage As roLanguage = Nothing

#Region "Declarations - Constructor"

    Private intResult As ServiceApiResultEnum

    Public Sub New(Optional ByVal _Context As System.Web.HttpContext = Nothing, Optional ByVal _ClientAddress As String = "")
        MyBase.New(-2, "AppInsights", _Context, _ClientAddress)
        Me.intResult = ServiceApiResultEnum.NoError
    End Sub

#End Region

#Region "Properties"

    <DataMember>
    Public Property Language() As roLanguage
        Get
            If Me.oLanguage Is Nothing Then
                Dim sLangKey As String = "ESP"
                Me.oLanguage = New roLanguage()
                Me.oLanguage.SetLanguageReference("ServiceApiService", sLangKey)
            End If
            Return Me.oLanguage
        End Get
        Set(value As roLanguage)

        End Set
    End Property

    Public Property Result() As ServiceApiResultEnum
        Get
            Return Me.intResult
        End Get
        Set(ByVal value As ServiceApiResultEnum)
            Me.intResult = value
            If Me.intResult <> ServiceApiResultEnum.NoError And Me.intResult <> ServiceApiResultEnum.Exception Then
                Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
            End If
        End Set
    End Property

#End Region

#Region "Methods"

    Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
        MyBase.UpdateStateInfo(_Context)
        Me.intResult = ServiceApiResultEnum.NoError
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = ServiceApiResultEnum.Exception
    End Sub

    Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
        MyBase.UpdateStateInfo(Ex, strUbication)
        Me.intResult = ServiceApiResultEnum.Exception
    End Sub

#End Region

End Class