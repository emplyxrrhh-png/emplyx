Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTDocuments

    Public Class roDocumentState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As DocumentResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roDocument", "DocumentService")
            Me.intResult = DocumentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roDocument", "DocumentService", _IDPassport)
            Me.intResult = DocumentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roDocument", "DocumentService", _IDPassport, _Context)
            Me.intResult = DocumentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDocument", "DocumentService", _IDPassport, , _ClientAddress)
            Me.intResult = DocumentResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roDocument", "DocumentService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = DocumentResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As DocumentResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As DocumentResultEnum)
                Me.intResult = value
                If Me.intResult <> DocumentResultEnum.NoError And Me.intResult <> DocumentResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("DocumentResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = DocumentResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DocumentResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = DocumentResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace