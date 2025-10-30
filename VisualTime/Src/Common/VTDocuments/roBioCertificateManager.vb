Imports System.ComponentModel
Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Web
Imports DevExpress.Utils.MVVM.Internal.ILReader
Imports DevExpress.XtraRichEdit
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.ValidateID
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports ServiceApi
Imports Robotics.Mail

Namespace VTDocuments

    Public Class roBioCertificateManager
        Private oState As roDocumentState = Nothing


#Region "Constructores"

        Public Sub New()
            oState = New roDocumentState()
        End Sub

        Public Sub New(ByVal _State As roDocumentState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        Public Function GenerateCertificate(cInfo As roCompanyInfo) As Boolean
            Try



                Dim certificateBytes = RoAzureSupport.GetCommonTemplateBytesFromAzure("bioCertificate.docx", roLiveQueueTypes.datalink)
                If certificateBytes Is Nothing OrElse certificateBytes.Length = 0 Then Return False

                Dim certificateCulture As CultureInfo = Me.oState.Language.GetLanguageCulture
                Dim certificateName As String = Guid.NewGuid().ToString()
                Dim certificateURL As String = $"{roConstants.VTLiveDefaultURL}biocertificate/{cInfo.code}/{certificateName}"


                Using stream As New MemoryStream(certificateBytes)
                    Using wordProcessor As New RichEditDocumentServer
                        wordProcessor.LoadDocument(stream, DocumentFormat.OpenXml)

                        Dim searchRegex As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("\[Cliente\]")
                        Dim count As Integer = wordProcessor.Document.ReplaceAll(searchRegex, cInfo.name)

                        If count = 0 Then
                            roLog.GetInstance.logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::Client tag not found")
                            Return False
                        End If

                        searchRegex = New System.Text.RegularExpressions.Regex("\[dd mmm yyyy\]")
                        count = wordProcessor.Document.ReplaceAll(searchRegex, DateTime.Now.ToString("D", certificateCulture))

                        If count = 0 Then
                            roLog.GetInstance.logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::Date tag not found")
                            Return False
                        End If

                        searchRegex = New System.Text.RegularExpressions.Regex("\[hh:mm\]")
                        count = wordProcessor.Document.ReplaceAll(searchRegex, DateTime.Now.ToString("t", certificateCulture))

                        If count = 0 Then
                            roLog.GetInstance.logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::Time tag not found")
                            Return False
                        End If


                        searchRegex = New System.Text.RegularExpressions.Regex("\[url\]")
                        count = wordProcessor.Document.ReplaceAll(searchRegex, certificateURL)

                        If count = 0 Then
                            roLog.GetInstance.logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::url tag not found")
                            Return False
                        End If


                        Using outputstream As New MemoryStream()
                            wordProcessor.ExportToPdf(outputstream)

                            Dim visualtimeDocumentManager As New roDocumentManager(oState)
                            Dim bioCertificate As New roDocument()
                            With bioCertificate
                                .Id = -1
                                .Title = certificateName
                                .IdEmployee = -1
                                .IdCompany = -1
                                .DocumentTemplate = visualtimeDocumentManager.LoadSystemTemplate(DocumentScope.BioCertificate)
                                .DocumentType = ".pdf"
                                .Document = outputstream.ToArray()
                                .DeliveredDate = DateTime.Now
                                .DeliveryChannel = "VisualTimeLive"
                                .DeliveredBy = roPassportManager.GetPassportTicket(oState.IDPassport).Name
                                .Status = DocumentStatus.Validated
                                .LastStatusChange = Date.Now
                                .BeginDate = New Date(1900, 1, 1)
                                .EndDate = New Date(2079, 1, 1)
                                .Remarks = certificateURL
                            End With

                            Dim bolret As Boolean = visualtimeDocumentManager.SaveDocument(bioCertificate, True)
                            If bolret Then
                                Try
                                    ' Enviamos el certificado de borrado a la cuenta configurada
                                    Dim oCompanyConfigurationRepository As New roConfigRepository()
                                    Dim emailCache As roAzureConfig = oCompanyConfigurationRepository.GetConfigParameter(roConfigParameter.email)

                                    Dim oConf As roMailConfig = DirectCast(Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(emailCache.value, GetType(roMailConfig)), roMailConfig)
                                    Dim oSmtpServer = Robotics.Mail.SendMail.GetInstance(oConf)
                                    Dim sourceMail = oConf.MailAccount
                                    Dim DestinationMail As roAzureConfig = oCompanyConfigurationRepository.GetConfigParameter(roConfigParameter.accountbiometriccertificate)

                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roBioCertificateManager::GenerateCertificate::Start sending email ...")
                                    Dim strRet = Mail.SendMail.SendMail(DestinationMail.value, "Certificado de borrado de datos biométricos " & cInfo.name, "Se adjunta certificado de borrado del cliente " & cInfo.name & ", realizado el " & DateTime.Now.ToString("dd-MM-yyyy HH:mm", certificateCulture), "bioCertificate.pdf", bioCertificate.Document, oSmtpServer, sourceMail, Robotics.Azure.RoAzureSupport.GetCompanyName())
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roBioCertificateManager::GenerateCertificate::email sent")

                                    If strRet.ToUpper <> "OK" Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::Sendmail {strRet}")
                                    End If

                                Catch ex As Exception
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate:: Sendmail{ex.Message}", ex)
                                End Try

                            End If

                            Return bolret

                        End Using
                    End Using
                End Using

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"roBioCertificateManager::GenerateCertificate::{ex.Message}", ex)
                Return False
            End Try

            Return True
        End Function
#End Region



    End Class

End Namespace