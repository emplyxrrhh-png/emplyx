Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Namespace Base


    <Serializable()>
    Public Class roSecurityState
        Inherits Robotics.VTBase.Extensions.roState

#Region "Declarations / Constructor"

        Private intResult As SecurityResultEnum

        Private intAuthenticateAttempts As Integer
        Private strLastCredentialAttempt As String

        Public Sub New()
            MyBase.New(-1, "Security")
            Me.intResult = SecurityResultEnum.NoError
            Me.intAuthenticateAttempts = 0
            Me.strLastCredentialAttempt = ""
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New(_IDPassport, "Security")
            Me.intResult = SecurityResultEnum.NoError
            Me.intAuthenticateAttempts = 0
            Me.strLastCredentialAttempt = ""
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New(_IDPassport, "Security", _Context)
            Me.intResult = SecurityResultEnum.NoError
            Me.intAuthenticateAttempts = 0
            Me.strLastCredentialAttempt = ""
        End Sub

#End Region

#Region "Base methods"
        Public Overrides Sub LoadLanguage()
            If Me.oLanguage Is Nothing Then
                Me.oLanguage = New roLanguage()
                Dim oPassport As roPassportTicket = Me.GetPassport()
                If oPassport IsNot Nothing Then
                    Me.oLanguage.SetLanguageReference("SecurityService", GetPassport().Language.Key)
                Else
                    Me.oLanguage.SetLanguageReference("SecurityService", "ESP")
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

#Region "Properties"

        Public Property Result() As SecurityResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As SecurityResultEnum)
                Me.intResult = value
                If Me.intResult <> SecurityResultEnum.NoError And Me.intResult <> SecurityResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

        Public Property AuthenticateAttempts() As Integer
            Get
                Return Me.intAuthenticateAttempts
            End Get
            Set(ByVal value As Integer)
                Me.intAuthenticateAttempts = value
            End Set
        End Property

        Public Property LastCredentialAttempt() As String
            Get
                Return Me.strLastCredentialAttempt
            End Get
            Set(ByVal value As String)
                Me.strLastCredentialAttempt = value
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = SecurityResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SecurityResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = SecurityResultEnum.Exception
        End Sub

        Public Shadows Function Audit(ByVal _Action As Audit.Action, ByVal _ObjectType As Audit.ObjectType, ByVal _ObjectName As String, ByVal _Parameters As DataTable, ByVal _SessionID As Integer) As Boolean

            Dim strUserName As String = ""
            Dim oPassport As roPassportTicket = GetPassport()
            If oPassport IsNot Nothing Then
                strUserName = oPassport.Name
            End If

            Return MyBase.Audit(strUserName, _Action, _ObjectType, _ObjectName, _Parameters, _SessionID)

        End Function



#End Region

    End Class
End Namespace