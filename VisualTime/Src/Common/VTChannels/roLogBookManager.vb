Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Audit
Imports Robotics.VTBase.Extensions
Imports System.Data.Common

Namespace VTChannels

    Public Class roLogBookManager

        Private oState As roLogBookState = Nothing

        Public ReadOnly Property State As roLogBookState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roLogBookState()
        End Sub

        Public Sub New(ByVal _State As roLogBookState)
            oState = _State
        End Sub

#End Region

        ''' <summary>
        ''' Recupera el libro de registro de una denuncia
        ''' </summary>
        ''' <param name="complaintRef"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetComplaintLog(complaintRef As String, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)
            Dim retLogBook As List(Of roMessage) = New List(Of roMessage)()
            Dim retLogBookStr As List(Of String) = Nothing

            Try
                oState.Result = LogBookResultEnum.NoError

                ' Comprobamos permisos de administración sobre el canal de denuncias

                Dim iPermissionOverComplaintChannels As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, "Employees.Complaints", "U")
                ' Consultores no pueden acceder
                If roPassportManager.IsRoboticsUserOrConsultant(Me.oState.IDPassport) Then
                    iPermissionOverComplaintChannels = 0
                End If

                If iPermissionOverComplaintChannels = 9 Then
                    retLogBookStr = GetAzureLogBookByReference(complaintRef).ToList()
                    Dim sCompanyEncryptionKey As String = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                    For Each oLogInfo As String In retLogBookStr
                        Dim oLog As roMessage = VTBase.roJSONHelper.DeserializeNewtonSoft(oLogInfo, GetType(roMessage))
                        Dim sBody = CryptographyHelper.DecryptEx(oLog.Body, sCompanyEncryptionKey)
                        oLog.Body = sBody
                        retLogBook.Add(oLog)
                    Next
                Else
                    oState.Result = LogBookResultEnum.NoPermission
                End If

                If (oState.Result = LogBookResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{ComplaintRef}", complaintRef, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLogBook, complaintRef, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = LogBookResultEnum.ErrorRecoveringLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::GetLogBook")
            Catch ex As Exception
                oState.Result = LogBookResultEnum.ErrorRecoveringLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::GetLogBook")
            End Try
            Return retLogBook
        End Function

        Private Function GetAzureLogBookByReference(ByVal xComplaintRef As String, Optional ByVal bAudit As Boolean = False) As String()
            Dim logBook As String() = New String() {}
            Try
                oState.Result = LogBookResultEnum.NoError
                logBook = RoAzureSupport.GetAzureLogBookByReference(xComplaintRef)
                If (oState.Result = LogBookResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLogBook, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = LogBookResultEnum.ErrorRecoveringLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::GetAzureLogBookByReference")
            Catch ex As Exception
                oState.Result = LogBookResultEnum.ErrorRecoveringLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::GetAzureLogBookByReference")
            End Try
            Return logBook
        End Function

        ''' <summary>
        ''' Almacena información en el libro de registro de una denuncia
        ''' </summary>
        ''' <param name="complaintRef"></param>
        ''' <param name="message"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>

        Public Function SaveLogBook(message As roMessage, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim ret As Boolean = False

            Try
                oState.Result = LogBookResultEnum.NoError

                Dim storedMessage As roMessage = DirectCast(message.Clone(), roMessage)
                Dim storedConversation As roConversation = DirectCast(message.Conversation.Clone(), roConversation)
                storedConversation.ExtraData = Nothing
                storedConversation.Channel = Nothing
                storedMessage.Conversation = storedConversation
                ret = SaveLogBookOnAzure(storedMessage)

                If (oState.Result = LogBookResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tLogBook, "", tbParameters, -1)
                End If
                If Not ret Then
                    oState.Result = LogBookResultEnum.ErrorSavingLogBook
                End If
            Catch ex As DbException
                oState.Result = LogBookResultEnum.ErrorSavingLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::SaveLogBook")
            Catch ex As Exception
                oState.Result = LogBookResultEnum.ErrorSavingLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::SaveLogBook")
            End Try
            Return ret
        End Function

        Private Function SaveLogBookOnAzure(ByVal message As roMessage, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Try
                oState.Result = LogBookResultEnum.NoError
                Dim strLogInfo As String = VTBase.roJSONHelper.SerializeNewtonSoft(message)
                bolRet = Robotics.Azure.RoAzureSupport.AddLogBookLineToTable(message.Conversation.ReferenceNumber, strLogInfo)
                If (oState.Result = LogBookResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLogBook, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = LogBookResultEnum.ErrorSavingLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::SaveLogBookOnAzure")
            Catch ex As Exception
                oState.Result = LogBookResultEnum.ErrorSavingLogBook
                Me.oState.UpdateStateInfo(ex, "roLogBookManager::SaveLogBookOnAzure")
            End Try
            Return bolRet
        End Function

    End Class

End Namespace