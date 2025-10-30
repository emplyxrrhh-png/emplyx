Imports Robotics.Base.DTOs

Imports Robotics.Security.Base

Namespace DataLink

    Public Class roDataLinkGuideState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As DataLinkGuideResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roDatalinkGuide", "DatalinkService")
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roDatalinkGuide", "DatalinkService", _IDPassport)
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roDatalinkGuide", "DatalinkService", _IDPassport, _Context)
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDatalinkGuide", "DatalinkService", _IDPassport, , _ClientAddress)
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDatalinkGuide", "DatalinkService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As DataLinkGuideResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As DataLinkGuideResultEnum)
                Me.intResult = value
                If Me.intResult <> DataLinkGuideResultEnum.NoError And Me.intResult <> DataLinkGuideResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("DataLinkGuideResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = DataLinkGuideResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DataLinkGuideResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DataLinkGuideResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace