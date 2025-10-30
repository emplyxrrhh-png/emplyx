Imports System.Web.Mvc
Imports DevExpress.CodeParser
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports ServiceApi
Imports VTLive40.Controllers.Base

<LoggedInAtrribute(Requiered:=False)>
Public Class DEXController
    Inherits BaseController

    Private requieredLabels = {"IdentificationName", "IdentificationPhone", "IdentificationMail", "IdentificationCompany", "IdentificationPwd", "IdentificationRepeatPwd", "IdentificationBtnNext",
        "btnRecoverComplaint", "roprintComplaintMessages", "recoverComplaint", "recoverComplaintDesc", "recoverComplaintID", "SendComplaint", "SendComplaintDesc", "IdentificationTitle", "PasswordInfo", "PasswordInfoDesc",
        "ComplaintBody", "ComplaintTitle", "ComplaintResponse", "ComplaintResponseDesc", "ComplaintGuidDesc", "CompanyLocation", "closeSessionError", "complaintGuidCopied", "complaintGuidNotCopied",
        "ComplaintValidationError", "ComplaintMsgSaveError", "ComplaintPwdValidationError", "ComplaintPrivacyRequiered", "ComplaintAnonymous", "ComplaintIdentified", "RecoverComplaintInfoRequiered",
        "ComplaintEmailValidationError", "ComplaintPhoneValidationError", "closeDEX", "copyGUID", "LostDataWarning", "ComplaintPwdRepeatError", "AlreadyLoggedIn", "NoLicense", "DEXSessionExpired",
        "MissingMandatoryData", "closeSessionDEX", "goToHome", "MessageTooLarge", "AcceptPrivacyPopup", "ClosePrivacyPopup", "ReadAndAccept", "privacyPolicyCTA", "privacyPolicyCTAEmpty"
    }

    Private requieredMessageLabels = {"sendButton", "conversationStatus", "ongoing", "closed", "dismissed", "statePending", "stateOngoing", "stateClosed", "stateDismissed",
        "SendPlaceHolder", "sendedAt", "by", "at", "emptylist", "roPrintComplaint"}

    Function Index(id As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "DEX", requieredLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "DEX", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        Dim cInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(id)
        Robotics.Web.Base.HelperWeb.EraseCookie("roDEXauth")
        ViewData("CompanyName") = String.Empty
        ViewData("CompanyGUID") = id

        If (WLHelperWeb.CurrentPassport Is Nothing AndAlso cInfo IsNot Nothing) OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing AndAlso Robotics.Azure.RoAzureSupport.GetCompanyName() = cInfo.code) Then
            Dim bSetCompanyInfo As Boolean = Robotics.Azure.RoAzureSupport.GetCompanyName().ToLower <> cInfo.code.ToLower
            Dim oComplaintChannel As roChannel = Nothing
            Try
                Web.HttpContext.Current.Session("DexAnonymous") = True
                If bSetCompanyInfo Then
                    Web.HttpContext.Current.Session("roMultiCompanyId") = cInfo.code
                    Global_asax.ReloadSharedData()
                End If

                oComplaintChannel = API.ChannelsServiceMethods.GetComplaintChannel(Nothing)
            Catch ex As Exception
                oComplaintChannel = Nothing
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DEXController.Index::", ex)
            Finally
                If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Web.HttpContext.Current.Session("DexAnonymous") = Nothing
            End Try

            If oComplaintChannel IsNot Nothing Then
                ViewData("CompanyName") = cInfo.name
                ViewData("PrivacyPolicyText") = oComplaintChannel.PrivacyPolicy
                Return View("DEX")
            Else
                Return View("NoSession")
            End If
        Else
            If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing Then
                Return View("InvalidateSession")
            Else
                Return View("NoSession")
            End If
        End If

    End Function

    Function Logout(id As String) As JsonResult
        Me.InitializeBase(Nothing, Nothing, "DEX", requieredLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "DEX", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        If Robotics.Web.Base.WLHelperWeb.CurrentPassport IsNot Nothing Then
            WLHelperWeb.SignOut(Nothing, WLHelperWeb.CurrentPassport)
        End If

        Return Json(True)
    End Function

    <HttpPost>
    Function ValidateData(id As String, key As String, email As String, phone As String) As JsonResult
        Me.InitializeBase(Nothing, Nothing, "DEX", requieredLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "DEX", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        Dim upper As New System.Text.RegularExpressions.Regex("[A-Z]")
        Dim lower As New System.Text.RegularExpressions.Regex("[a-z]")
        Dim number As New System.Text.RegularExpressions.Regex("[0-9]")
        Dim special As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]")

        If Not (key.Length >= 10 AndAlso upper.Matches(key).Count > 0 AndAlso lower.Matches(key).Count > 0 AndAlso number.Matches(key).Count > 0 AndAlso special.Matches(key).Count > 0) Then
            Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#ComplaintPwdValidationError"))
        End If

        Dim regex As New Regex("^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$")
        If email.Length > 0 AndAlso Not regex.IsMatch(email) Then
            Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#ComplaintEmailValidationError"))
        End If

        regex = New Regex("^(\+\d{9,}|\d{9,})$")
        If phone.Length > 0 AndAlso Not regex.IsMatch(phone) Then
            Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#ComplaintPhoneValidationError"))
        End If

        Return Json(True)

    End Function

    <HttpPost>
    Function Send(id As String, dexMessage As roMessage) As JsonResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc") _
                          .AddAdditionalLanguage("DEX", requieredLabels, "LiveChannels")

        Dim cInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(id)
        Dim companyName As String = RoAzureSupport.GetCompanyName()
        If (WLHelperWeb.CurrentPassport Is Nothing AndAlso cInfo IsNot Nothing) OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing AndAlso companyName.ToLower = cInfo.code.ToLower) Then
            Dim bSetCompanyInfo As Boolean = companyName.ToLower <> cInfo.code.ToLower
            Dim oResult As Object
            Try
                Web.HttpContext.Current.Session("DexAnonymous") = True
                If bSetCompanyInfo Then
                    Web.HttpContext.Current.Session("roMultiCompanyId") = cInfo.code
                    Global_asax.ReloadSharedData()
                End If

                Dim oComplaintChannel As roChannel = API.ChannelsServiceMethods.GetComplaintChannel(Nothing, False)
                If oComplaintChannel IsNot Nothing Then
                    dexMessage.Conversation.Channel = oComplaintChannel
                    dexMessage.Conversation.CreatedBy = -1
                    dexMessage.CreatedBy = -1
                    dexMessage.CreatedOn = Now
                    dexMessage.Conversation.CreatedOn = Now
                    dexMessage.IsResponse = False

                    If API.ChannelsServiceMethods.CreateConversation(Nothing, dexMessage, True) Then
                        oResult = dexMessage
                    Else
                        oResult = API.ChannelsServiceMethods.LastConversationErrorText
                    End If
                Else
                    oResult = API.ChannelsServiceMethods.LastChannelErrorText
                End If
            Catch ex As Exception
                oResult = "Unknown exception"
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DEXController.Send::", ex)
            Finally
                If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Web.HttpContext.Current.Session("DexAnonymous") = Nothing
            End Try

            Return Json(oResult)
        Else
            Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#NoLicense"))
        End If

    End Function

    <HttpPost>
    Function ContentMessages(id As String, ByVal complaintReference As String, ByVal password As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc") _
                          .AddAdditionalLanguage("DEX", requieredLabels, "LiveChannels")
        If Not ReCaptchaValidator.Validate(roTypes.Any2String(HttpContext.Request.Form("g-recaptcha-response"))) Then
            ViewData("ErrorMsg") = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#DEXSessionExpired")
            Return View("NoSession")
        End If

        If complaintReference = String.Empty OrElse password = String.Empty Then
            ViewData("ErrorMsg") = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#MissingMandatoryData")
            Return View("NoSession")
        End If

        Dim cInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(id)
        Dim companyName As String = RoAzureSupport.GetCompanyName()
        Dim oMessages As Generic.List(Of roMessage) = Nothing

        If (WLHelperWeb.CurrentPassport Is Nothing AndAlso cInfo IsNot Nothing) OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing AndAlso companyName.ToLower = cInfo.code.ToLower) Then
            Dim bSetCompanyInfo As Boolean = companyName.ToLower <> cInfo.code.ToLower

            Try
                Web.HttpContext.Current.Session("DexAnonymous") = True
                If bSetCompanyInfo Then
                    Web.HttpContext.Current.Session("roMultiCompanyId") = cInfo.code
                    Global_asax.ReloadSharedData()
                End If

                If API.ChannelsServiceMethods.GetComplaintChannel(Nothing) IsNot Nothing Then
                    ViewData("CompanyName") = cInfo.name
                    oMessages = API.ChannelsServiceMethods.GetAllComplaintMessages(roTypes.Any2String(complaintReference), password, -1, Nothing, True)

                    If oMessages IsNot Nothing AndAlso oMessages.Count > 0 Then
                        ViewData("ConversationMessages") = oMessages
                        ViewData("Conversation") = oMessages(0).Conversation
                        ViewData("ExternAccess") = True
                        Robotics.Web.Base.HelperWeb.CreateCookie("roDEXauth", Robotics.VTBase.CryptographyHelper.EncryptWithSHA256($"{id}#{complaintReference}#XAISLU".ToUpper()))
                    Else
                        ViewData("ErrorMsg") = API.ChannelsServiceMethods.LastMessageErrorText
                    End If
                Else
                    ViewData("ErrorMsg") = API.ChannelsServiceMethods.LastChannelErrorText
                End If
            Catch ex As Exception
                oMessages = Nothing
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DEXController.ContentMessages::", ex)
            Finally
                If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Web.HttpContext.Current.Session("DexAnonymous") = Nothing
            End Try

            Return View(If(oMessages IsNot Nothing AndAlso oMessages.Count > 0, "DEXContent", "NoSession"))
        Else
            ViewData("ErrorMsg") = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#NoLicense")
            Return View("NoSession")
        End If

    End Function

    <HttpPost>
    Public Function AddMessage(id As String, ByVal idComplaint As Integer, ByVal Message As String) As JsonResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc") _
                          .AddAdditionalLanguage("DEX", requieredLabels, "LiveChannels")

        Dim roDEXauth As String = Robotics.Web.Base.HelperWeb.GetCookie("roDEXauth")

        If roDEXauth = String.Empty Then Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#DEXSessionExpired"))

        Dim cInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(id)
        Dim companyName As String = RoAzureSupport.GetCompanyName()
        If (WLHelperWeb.CurrentPassport Is Nothing AndAlso cInfo IsNot Nothing) OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing AndAlso companyName.ToLower = cInfo.code.ToLower) Then
            Dim bSetCompanyInfo As Boolean = companyName.ToLower <> cInfo.code.ToLower
            Dim oResult As Object

            Try
                Web.HttpContext.Current.Session("DexAnonymous") = True
                If bSetCompanyInfo Then
                    Web.HttpContext.Current.Session("roMultiCompanyId") = cInfo.code
                    Global_asax.ReloadSharedData()
                End If

                Dim oComplaintChannel As roChannel = API.ChannelsServiceMethods.GetComplaintChannel(Nothing, False)
                If (oComplaintChannel IsNot Nothing) Then
                    Dim complaint As roConversation = API.ChannelsServiceMethods.GetConversationById(idComplaint, Nothing, -1, False)
                    If complaint IsNot Nothing AndAlso complaint.Id > 0 Then
                        complaint.Channel = oComplaintChannel
                        Dim newMessage As New roMessage With {
                            .Conversation = complaint,
                            .CreatedBy = -1,
                            .CreatedOn = Date.Now,
                            .Body = Message,
                            .IsResponse = False
                        }

                        If roDEXauth = Robotics.VTBase.CryptographyHelper.EncryptWithSHA256($"{id}#{complaint.ReferenceNumber}#XAISLU".ToUpper()) Then
                            If API.ChannelsServiceMethods.CreateMessage(Nothing, newMessage, True) Then
                                oResult = True
                            Else
                                oResult = API.ChannelsServiceMethods.LastMessageErrorText
                            End If
                        Else
                            oResult = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#DEXSessionExpired")
                        End If
                    Else
                        oResult = API.ChannelsServiceMethods.LastConversationErrorText
                    End If
                Else
                    oResult = API.ChannelsServiceMethods.LastChannelErrorText
                End If
            Catch ex As Exception
                oResult = "Unknown exception"
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DEXController.AddMessage::", ex)
            Finally
                If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Web.HttpContext.Current.Session("DexAnonymous") = Nothing
            End Try

            Return Json(oResult)
        Else
            Return Json(ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#NoLicense"))
        End If

    End Function

    <HttpPost>
    Function Reload(id As String, ByVal complaintReference As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc") _
                          .AddAdditionalLanguage("DEX", requieredLabels, "LiveChannels")
        Dim roDEXauth As String = Robotics.Web.Base.HelperWeb.GetCookie("roDEXauth")

        If roDEXauth <> Robotics.VTBase.CryptographyHelper.EncryptWithSHA256($"{id}#{complaintReference}#XAISLU".ToUpper()) Then
            ViewData("ErrorMsg") = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#DEXSessionExpired")
            Return View("NoSession")
        End If

        Dim cInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(id)
        Dim companyName As String = RoAzureSupport.GetCompanyName()
        Dim oMessages As Generic.List(Of roMessage) = Nothing

        If (WLHelperWeb.CurrentPassport Is Nothing AndAlso cInfo IsNot Nothing) OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso cInfo IsNot Nothing AndAlso companyName.ToLower = cInfo.code.ToLower) Then
            Dim bSetCompanyInfo As Boolean = companyName.ToLower <> cInfo.code.ToLower

            Try
                Web.HttpContext.Current.Session("DexAnonymous") = True
                If bSetCompanyInfo Then
                    Web.HttpContext.Current.Session("roMultiCompanyId") = cInfo.code
                    Global_asax.ReloadSharedData()
                End If

                If API.ChannelsServiceMethods.GetComplaintChannel(Nothing) IsNot Nothing Then
                    ViewData("CompanyName") = cInfo.name
                    oMessages = API.ChannelsServiceMethods.ReloadComplaintMessages(complaintReference, roDEXauth, -1, Nothing, False)
                    ViewData("ErrorMsg") = API.ChannelsServiceMethods.LastMessageErrorText
                Else
                    ViewData("ErrorMsg") = API.ChannelsServiceMethods.LastChannelErrorText
                End If
            Catch ex As Exception
                oMessages = Nothing
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DEXController.Reload::", ex)
            Finally
                If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Web.HttpContext.Current.Session("DexAnonymous") = Nothing
            End Try

            Return View(If(oMessages IsNot Nothing AndAlso oMessages.Count > 0, "_MessagesList", "NoSession"), oMessages)
        Else
            ViewData("ErrorMsg") = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)("DEX#NoLicense")
            Return View("NoSession")
        End If

    End Function

End Class