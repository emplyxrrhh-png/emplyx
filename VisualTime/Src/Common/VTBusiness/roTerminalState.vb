Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

Namespace Terminal

    Public Class roTerminalState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As TerminalBaseResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Terminal", "TerminalsService")
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Terminal", "TerminalsService", _IDPassport)
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Terminal", "TerminalsService", _IDPassport, _Context)
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Terminal", "TerminalsService", _IDPassport, , _ClientAddress)
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Terminal", "TerminalsService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As TerminalBaseResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As TerminalBaseResultEnum)
                Me.intResult = value
                If Me.intResult <> TerminalBaseResultEnum.NoError And Me.intResult <> TerminalBaseResultEnum.Exception Then
                    If Me.ReturnCode.Length > 0 Then
                        Me.ErrorText = Me.Language.Translate("ErrorCodeMessage." & Me.ReturnCode, "")
                    Else
                        Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                    End If
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = TerminalBaseResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TerminalBaseResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = TerminalBaseResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace