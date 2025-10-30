Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs

Namespace VTLiveTasks

    <DataContract()>
    Public Class roLiveTaskState
        Inherits roState

#Region "Declarations - Constructor"

        Private intResult As LiveTasksResultEnum

        Public Sub New(Optional ByVal idPassport As Integer = -1)
            MyBase.New(idPassport, "VTBusiness.roLiveTask")

            Me.intResult = LiveTasksResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Result() As LiveTasksResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As LiveTasksResultEnum)
                Me.intResult = value
                If Me.intResult <> LiveTasksResultEnum.NoError AndAlso Me.intResult <> LiveTasksResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = LiveTasksResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = LiveTasksResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = LiveTasksResultEnum.Exception
        End Sub

        Public Overrides Sub LoadPassport()
            Me.oPassport = VTBase.roConstants.GetLoggedInPassportTicket()

            If Me.oPassport Is Nothing Then
                Me.oPassport = New roPassportTicket() With {
                .ID = VTBase.roConstants.GetSystemUserId(),
                .Name = "System",
                .AuthCredential = "System",
                .Language = New roPassportLanguage With {
                        .Culture = "es-ES",
                        .ID = 1,
                        .Installed = True,
                        .Key = "ESP",
                        .ParametersXml = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ExtLanguage"" type=""8"">sp</Item><Item key=""ExtDatePickerFormat"" type=""8"">d/m/Y</Item><Item key=""ExtDatePickerStartDay"" type=""8"">1</Item></roCollection>"
                    }
                }
            End If

        End Sub
        Public Overrides Sub LoadLanguage()

            If Me.oLanguage Is Nothing Then
                Me.oLanguage = New roLanguage()
                Me.oLanguage.SetLanguageReference("LiveTasksService", "ESP")
            End If

        End Sub

#End Region

    End Class

End Namespace