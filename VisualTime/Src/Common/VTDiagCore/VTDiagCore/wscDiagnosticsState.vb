Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTDiagCore

    Public Class wscDiagnosticsState
        Inherits roState

        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            FileAccessError
            ProcessAccessError
        End Enum

#Region "Declarations - Constructor"

        Private intResult As ResultEnum

        Public Sub New()
            MyBase.New(-1, "Diagnostics.Audit")
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New(_IDPassport, "Diagnostics.Audit")
            Me.intResult = ResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ResultEnum)
                Me.intResult = value
            End Set
        End Property

#End Region



#Region "Base methods"
        Public Overrides Sub LoadLanguage()
            If Me.oLanguage Is Nothing Then
                Me.oLanguage = New roLanguage()
                Dim oPassport As roPassportTicket = Me.GetPassport()
                If oPassport IsNot Nothing Then
                    Me.oLanguage.SetLanguageReference("Diagnostics", GetPassport().Language.Key)
                Else
                    Me.oLanguage.SetLanguageReference("Diagnostics", "ESP")
                End If
            End If
        End Sub
        Public Overrides Sub LoadPassport()

            Try
                Me.oPassport = VTBase.roConstants.GetLoggedInPassportTicket()

                If Me.oPassport Is Nothing Then
                    Me.oPassport = roPassportManager.GetPassportTicket(Me.IDPassport, LoadType.Passport)

                    If Me.oPassport Is Nothing Then
                        Me.oPassport = New roPassportTicket With {
                            .Name = "Unkown",
                            .AuthCredential = "unkown",
                            .Language = New roPassportLanguage With {
                                    .Culture = "es-ES",
                                    .ID = 1,
                                    .Installed = True,
                                    .Key = "ESP",
                                    .ParametersXml = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ExtLanguage"" type=""8"">sp</Item><Item key=""ExtDatePickerFormat"" type=""8"">d/m/Y</Item><Item key=""ExtDatePickerStartDay"" type=""8"">1</Item></roCollection>"
                                }
                        }
                    End If

                End If
            Catch
            End Try

        End Sub

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace