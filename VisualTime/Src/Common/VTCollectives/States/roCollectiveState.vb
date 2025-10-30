Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace VTCollectives

    Public Class roCollectiveState
        Inherits roBusinessState
        Implements IResultHandler

#Region "Declarations - Constructor"

        Private intResult As CollectiveResult

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roCollective", "CollectivesService")
            Me.intResult = CollectiveResult.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roCollective", "CollectivesService", _IDPassport)
            Me.intResult = CollectiveResult.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roCollective", "CollectivesService", _IDPassport, _Context)
            Me.intResult = CollectiveResult.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCollective", "CollectivesService", _IDPassport, , _ClientAddress)
            Me.intResult = CollectiveResult.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roCollective", "CollectivesService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = CollectiveResult.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As CollectiveResult
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As CollectiveResult)
                Me.intResult = value
                If Me.intResult <> CollectiveResult.NoError AndAlso Me.intResult <> CollectiveResult.Exception Then
                    Me.ErrorText = Me.Language.Translate("CollectiveResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"
        Public Sub SetExceptionResult() Implements IResultHandler.SetExceptionResult
            Me.intResult = EngineResultEnum.Exception
        End Sub

        Public Sub ClearResult() Implements IResultHandler.ClearResult
            Me.intResult = EngineResultEnum.NoError
        End Sub

#End Region

    End Class

End Namespace