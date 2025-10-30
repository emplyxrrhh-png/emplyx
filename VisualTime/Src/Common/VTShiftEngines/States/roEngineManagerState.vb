Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace VTShiftEngines

    Public Class roEngineState
        Inherits roBusinessState
        Implements IResultHandler

#Region "Declarations - Constructor"

        Private intResult As EngineResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roEngine", "ConceptsService")
            Me.intResult = EngineResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roEngine", "ConceptsService", _IDPassport)
            Me.intResult = EngineResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roEngine", "ConceptsService", _IDPassport, _Context)
            Me.intResult = EngineResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roEngine", "ConceptsService", _IDPassport, , _ClientAddress)
            Me.intResult = EngineResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roEngine", "ConceptsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = EngineResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As EngineResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As EngineResultEnum)
                Me.intResult = value
                If Me.intResult <> EngineResultEnum.NoError And Me.intResult <> EngineResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("EngineResultEnum." & Me.intResult.ToString, "")
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