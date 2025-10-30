Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Business

    Public Class wscState
        Inherits roState

#Region "Declarations / Constructor"

        Private intResult As WscResultEnum

        Public Sub New()
            MyBase.New(-1, "VTBusiness.Security")
            Me.intResult = WscResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New(_IDPassport, "VTBusiness.Security")
            Me.intResult = WscResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New(_IDPassport, "VTBusiness.Security", _Context)
            Me.intResult = WscResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As WscResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As WscResultEnum)
                Me.intResult = value
                If Me.intResult <> WscResultEnum.NoError And Me.intResult <> WscResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property
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

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = WscResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = WscResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = WscResultEnum.Exception
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