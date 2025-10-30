Imports System.Runtime.Serialization
Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.VTBase.Audit

<Serializable()>
<DataContract>
Public MustInherit Class roState
    Inherits roBaseState

    Protected Property oPassport As roPassportTicket = Nothing
    Protected Property oLanguage As roLanguage = Nothing

    Public ReadOnly Property Language As roLanguage
        Get
            If (Me.oLanguage Is Nothing) Then Me.LoadLanguage()

            Return Me.oLanguage
        End Get
    End Property


    Public MustOverride Sub LoadPassport()
    Public MustOverride Sub LoadLanguage()

    Public Sub New(ByVal idPassport As Integer, ByVal strClassNameLog As String, Optional ByVal context As System.Web.HttpContext = Nothing, Optional ByVal clientAddress As String = "", Optional ByVal sessionID As String = "")
        MyBase.New(idPassport, strClassNameLog, context, clientAddress, sessionID)

        ' Si el passport es -1, asigno al usuario de sistema 'System' que es admin
        If Me.IDPassport = -1 Then
            Me.IDPassport = roConstants.GetSystemUserId()

            If Me.IDPassport < 1 Then
                Try
                    Dim strSQL As String = "@SELECT# ID FROM sysroPassports WHERE Description = '@@ROBOTICS@@System'"
                    Me.IDPassport = roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL))
                Catch ex As Exception
                    Me.IDPassport = -2
                End Try

                If Me.IDPassport > 0 Then
                    If roConstants.IsMultitenantServiceEnabled() Then
                        Thread.GetDomain().SetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString(), Me.IDPassport)
                    Else
                        roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.SystemPassportID, Me.IDPassport)
                    End If
                End If



            End If
        End If
    End Sub

#Region "Methods"

    Public Function Audit(ByVal action As Action, ByVal objectType As ObjectType,
                                      ByVal objectName As String, ByVal parameters As DataTable,
                                      ByVal sessionID As Integer) As Boolean

        Return Me.Audit("", action, objectType, objectName, parameters, sessionID)

    End Function

    Public Function GetPassportName() As String
        Dim strValue As String = String.Empty
        If Me.IDPassport > 0 Then
            If oPassport Is Nothing Then
                Me.LoadPassport()
            End If
            If oPassport IsNot Nothing Then
                strValue = oPassport.Name
            Else
                strValue = Me.GetPassportName(Me.IDPassport)
            End If
        End If
        Return strValue
    End Function

    Public Function GetPassport() As roPassportTicket

        Me.oPassport = VTBase.roConstants.GetLoggedInPassportTicket()

        If oPassport Is Nothing Then
            Me.LoadPassport()

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
        Return oPassport
    End Function

    Public Function GetLanguageKey() As String

        Dim strLanguageKey As String

        Dim oPassport As roPassportTicket = Me.GetPassport
        If oPassport Is Nothing Then
            Dim oSettings As New roSettings()
            strLanguageKey = oSettings.GetVTSetting(eKeys.DefaultLanguage)
        Else
            strLanguageKey = oPassport.Language.Key
        End If

        Return strLanguageKey

    End Function

    'Overrides
    Public Function Audit(ByVal userName As String,
                                      ByVal action As Action, ByVal objectType As ObjectType,
                                      ByVal objectName As String, ByVal parameters As DataTable,
                                      ByVal sessionID As Integer) As Boolean

        Dim bolRet As Boolean = False

        Try

            userName = GetPassportName()

            Dim oAudit As roAudit
            If Me.ClientAddress <> "" Then
                oAudit = New roAudit(ClientAddress, userName, action, objectType, objectName, parameters, sessionID)
            ElseIf Context IsNot Nothing Then ' Se ha informado el contexto (HttpContext) para poder determinar la localización del cliente (ip)
                oAudit = New roAudit(Context.Request, userName, action, objectType, objectName, parameters, sessionID)
            Else ' No se ha informado el context. Se especifica com localización del cliente el nombre de usuario activo
                oAudit = New roAudit(My.User.Name, userName, action, objectType, objectName, parameters, sessionID)
            End If

            bolRet = oAudit.SaveAudit()
        Catch ex As System.Data.Common.DbException
            Me.UpdateStateInfo(ex, "roBaseState::Audit")
        Catch ex As Exception
            Me.UpdateStateInfo(ex, "roBaseState::Audit")
        End Try

        Return bolRet

    End Function

    Private Function GetPassportName(ByVal idPassport As Integer) As String
        Dim strValue As String = String.Empty

        Try

            Dim strSQL As String = "@SELECT# Name FROM sysroPassports WHERE ID = " & idPassport
            strValue = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL))
        Catch ex As Exception
            'do nothing, just return empty string
            strValue = "unknown"
        End Try

        Return strValue
    End Function

    'Overrides
    Public Function CreateAuditParameters() As DataTable
        Return roAudit.CreateParametersTable
    End Function

    'Overrides
    Public Sub AddAuditParameter(ByVal tb As DataTable, ByVal strName As String, ByVal strValue As String, ByVal strParent As String, ByVal intPriority As Integer)
        roAudit.AddParameter(tb, strName, strValue, strParent, intPriority)
    End Sub

    'Overrides
    Public Sub AddAuditFieldsValues(ByVal tb As DataTable, ByVal oRow As DataRow, Optional ByVal oRowOld As DataRow = Nothing)
        roAudit.AddFieldsValues(tb, oRow, oRowOld)
    End Sub

#End Region

End Class