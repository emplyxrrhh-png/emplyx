Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace AuditState

    Public Class wscAuditState
        Inherits VTBase.Extensions.roState

#Region "Declarations - Constructor"

        Private intResult As AuditResultEnum

        Public Sub New()
            MyBase.New(-1, "AuditService.Audit")
            Me.intResult = AuditResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New(_IDPassport, "AuditService.Audit")
            Me.intResult = AuditResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As AuditResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As AuditResultEnum)
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
                    Me.oLanguage.SetLanguageReference("AuditService", GetPassport().Language.Key)
                Else
                    Me.oLanguage.SetLanguageReference("AuditService", "ESP")
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
            Me.intResult = AuditResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AuditResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = AuditResultEnum.Exception
        End Sub

        Public Shadows Function Audit(ByVal _Action As VTBase.Audit.Action, ByVal _ObjectType As VTBase.Audit.ObjectType, ByVal _ObjectName As String, ByVal _Parameters As DataTable, ByVal _SessionID As Integer) As Integer

            Dim strUserName As String = ""
            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(Me.IDPassport)
            If oPassport IsNot Nothing Then
                strUserName = oPassport.Name
            End If

            Return MyBase.Audit(strUserName, _Action, _ObjectType, _ObjectName, _Parameters, _SessionID)

        End Function

#End Region

    End Class

End Namespace