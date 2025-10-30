Imports System.Net
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports VTLive40.CardsTree.Model
Imports VTLive40.Controllers.Base
Imports WLHelperWeb = Robotics.Web.Base.WLHelperWeb

<PermissionsAtrribute(FeatureAlias:="Employees.Channels,Employees.Complaint", Permission:=Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class ChannelsController
    Inherits BaseController

    Private requieredLabels = {"roChannelNew", "roChannelStatus", "roChannelsTitle", "roChannelInfo", "roChannelCreatedOn", "roChannelNewMessages", "roChannelName", "roChannelLoading", "roChannelDelete", "roChannelDeleteTitle", "roChannelStatusResume", "roChannelPendingConversations",
        "roChannelConfiguration", "roChannelConversations", "roChannelGeneral", "roChannelDestinationDesc", "roChannelDestination", "roChannelAnonymous", "roChannelSimple", "roChannelAdvanced", "roChannelNo", "roChannelYes", "roChannelPublished", "roChannelTitleDesc",
        "roNoData", "roChannelSupervisors", "roChannelSupervisorsDesc", "roChannelReceiptAknowledgment", "complaint1", "complaint2", "complaint3", "complaint4", "complaint5", "roChannelCancelPublish", "validateComplaintTitle", "roRestrictChannel", "roChannelDEXAccept"}

    Private requieredConvLabels = {"searchText", "treeCaption", "emptylist"}
    Private requieredMessageLabels = {"sendButton", "conversationStatus", "ongoing", "closed", "dismissed", "statePending", "stateOngoing", "stateClosed", "stateDismissed", "SendPlaceHolder", "sendedAt", "by", "at", "conversationComplexity", "highComplexity", "lowComplexity", "roPrintComplaint"}

    Function Index() As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Channels", requieredLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")
        If Robotics.Web.Base.WLHelperWeb.CurrentPassport IsNot Nothing Then

            Dim oLicSupport As New roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

            ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")
            ViewBag.PermissionOverEmployees = CInt(API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Employees.Channels", "U")) 'Convert.ToInt32(WLHelper.GetSupervisorPermissionOverFeature(Robotics.Web.Base.WLHelperWeb.CurrentPassportID, 1850))
            ViewBag.IdPassport = Robotics.Web.Base.WLHelperWeb.CurrentPassportID

            Try
                LoadInitialData()
            Catch ex As Exception
            End Try
            Return View("Channels")
        Else
            Return View("NoSession")
        End If
    End Function

    Private Function LoadInitialData() As Boolean
        Try
            Dim availableSupervisors As New List(Of roAutocompleteItemModel)

            availableSupervisors.AddRange(API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing).Select(Function(user) New roAutocompleteItemModel() With {
                                                        .ID = user.ID,
                                                        .Name = user.Name}))
            ViewBag.Supervisors = availableSupervisors
            Dim sLic As New roServerLicense
            ViewBag.HasComplaintsPermission = sLic.FeatureIsInstalled("Feature\Complaints")
            ViewBag.HasChannelsPermission = sLic.FeatureIsInstalled("Feature\Channels")
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Function CreateChannel() As ActionResult
        Return View("CreateChannel")
    End Function

    <HttpGet>
    Public Function GetChannels(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        Dim oChannels() As roChannel
        Dim oEmpState As New Employee.roEmployeeState(-1)

        oChannels = API.ChannelsServiceMethods.GetAllChannels(Nothing)

        Dim result = DataSourceLoader.Load(oChannels, loadOptions)
        Dim resultJson = JsonConvert.SerializeObject(result)

        Return Content(resultJson, "application/json")
    End Function

    <HttpPost>
    Public Function GetChannel(ByVal idChannel As String) As JsonResult

        Dim oChannel As roChannel = API.ChannelsServiceMethods.GetChannel(roTypes.Any2Integer(idChannel), Nothing)

        If oChannel IsNot Nothing AndAlso oChannel.Id > 0 Then
            Return Json(oChannel)
        Else
            Return Json(roWsUserManagement.SessionObject().States.ChannelState.ErrorText)
        End If

    End Function

    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Employees.Channels,Employees.Complaint", Permission:=Permission.Write)>
    Public Function InsertChannel(ByVal Id As Integer, ByVal Employees As Integer(), ByVal Groups As Integer(), Title As String, ByVal Anonymous As String, ByVal Automatic As String, ByVal SubscribedSupervisors As Integer(), Optional ByVal CurrentStatus As ChannelStatusEnum = ChannelStatusEnum.Draft) As JsonResult

        Dim bRes As Boolean = True
        Dim totalEmployees As New List(Of Integer)
        Dim totalGroups As New List(Of Integer)
        Dim resultChannel As roChannel = Nothing

        If (Id <> 0 AndAlso (CurrentStatus = ChannelStatusEnum.Draft)) Then

            Dim existingChannel As roChannel = API.ChannelsServiceMethods.GetChannel(Id, Nothing)
            If existingChannel IsNot Nothing AndAlso existingChannel.Id > 0 Then
                existingChannel.Title = Title
                existingChannel.AllowAnonymous = Anonymous
                existingChannel.Employees = Employees
                existingChannel.Groups = Groups
                existingChannel.ModifiedBy = Robotics.Web.Base.WLHelperWeb.CurrentPassport.ID
                existingChannel.ModifiedByName = Robotics.Web.Base.WLHelperWeb.CurrentPassport.Name
                existingChannel.ModifiedOn = Date.Now
                existingChannel.SubscribedSupervisors = SubscribedSupervisors
                existingChannel.ReceiptAcknowledgment = Automatic

                bRes = API.ChannelsServiceMethods.CreateOrUpdateChannel(Nothing, existingChannel, True)
            Else
                bRes = False
            End If

            If bRes Then resultChannel = existingChannel
        Else
            Dim newChannel As New roChannel With {
                .Id = Id,
                .CreatedBy = Robotics.Web.Base.WLHelperWeb.CurrentPassport.ID,
                .Title = Title,
                .Employees = Employees,
                .Groups = Groups,
                .AllowAnonymous = Anonymous,
                .SubscribedSupervisors = SubscribedSupervisors,
                .ReceiptAcknowledgment = Automatic
            }
            If Id = 0 Then newChannel.CreatedOn = Date.Now

            bRes = API.ChannelsServiceMethods.CreateOrUpdateChannel(Nothing, newChannel, True)
            If bRes Then resultChannel = newChannel
        End If

        If bRes Then
            Return Json(resultChannel)
        Else
            Return Json(roWsUserManagement.SessionObject().States.ChannelState.ErrorText)
        End If

    End Function

    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Employees.Channels,Employees.Complaint", Permission:=Permission.Write)>
    Public Function SaveComplaintChannel(ByVal Id As Integer, PrivacyPolicy As String) As JsonResult

        Dim bRes As Boolean = True
        Dim resultChannel As roChannel = Nothing

        If (Id <> 0) Then

            Dim existingChannel As roChannel = API.ChannelsServiceMethods.GetChannel(Id, Nothing)
            If existingChannel IsNot Nothing AndAlso existingChannel.Id > 0 Then
                existingChannel.PrivacyPolicy = PrivacyPolicy
                bRes = API.ChannelsServiceMethods.CreateOrUpdateChannel(Nothing, existingChannel, True)
            Else
                bRes = False
            End If

            If bRes Then resultChannel = existingChannel
        Else
            bRes = False
        End If

        If bRes Then
            Return Json(resultChannel)
        Else
            Return Json(roWsUserManagement.SessionObject().States.ChannelState.ErrorText)
        End If

    End Function

    <HttpPost>
    Public Function PublishChannel(ByVal Id As Integer) As JsonResult
        Dim oChannel As roChannel = API.ChannelsServiceMethods.GetChannel(Id, Nothing)

        If oChannel IsNot Nothing AndAlso oChannel.Id > 0 Then
            oChannel.Status = ChannelStatusEnum.Published
            oChannel.PublishedOn = Date.Now

            If API.ChannelsServiceMethods.UpdateChannelStatus(Nothing, oChannel, Nothing) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Public Function RestrictChannel(ByVal Id As Integer) As JsonResult
        Dim oChannel As roChannel = API.ChannelsServiceMethods.GetChannel(Id, Nothing)

        If oChannel IsNot Nothing AndAlso oChannel.Id > 0 AndAlso Not oChannel.IsComplaintChannel Then
            oChannel.Status = ChannelStatusEnum.Draft
            oChannel.PublishedOn = Date.Now

            If API.ChannelsServiceMethods.UpdateChannelStatus(Nothing, oChannel, Nothing) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpDelete>
    <PermissionsAtrribute(FeatureAlias:="Employees.Channels", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Public Function DeleteChannel(ByVal key As Integer) As ActionResult
        Dim tmpChannel As roChannel = API.ChannelsServiceMethods.GetChannel(key, Nothing)

        If tmpChannel IsNot Nothing AndAlso tmpChannel.Id > 0 AndAlso Not tmpChannel.IsComplaintChannel Then
            If tmpChannel.CreatedBy = WLHelperWeb.CurrentPassportID OrElse CInt(API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Employees.Channels", "U")) = 9 Then
                If API.ChannelsServiceMethods.DeleteChannel(tmpChannel, Nothing, True) Then
                    Return New HttpStatusCodeResult(HttpStatusCode.OK)
                End If
            End If

            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpGet>
    Public Function GetChannelConversations(ByVal idChannel As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Conversations", requieredConvLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        BuildCardViewFromConversations(idChannel)

        ViewData("SelectedCardId") = idChannel

        Return View("ChannelConversations")
    End Function

    Private Sub BuildCardViewFromConversations(idChannel As String)
        Dim oCards As New List(Of Card)
        Dim tmpChannel As roChannel = API.ChannelsServiceMethods.GetChannel(idChannel, Nothing)

        If tmpChannel IsNot Nothing AndAlso tmpChannel.Id > 0 Then
            Dim oConversations As Generic.List(Of roConversation) = API.ChannelsServiceMethods.GetChannelConversations(roTypes.Any2Integer(idChannel), Nothing)

            If oConversations IsNot Nothing AndAlso oConversations.Count > 0 Then
                Dim oLng As New roLanguage
                oLng.SetLanguageReference("LiveChannels", WLHelperWeb.CurrentLanguage)

                For Each oConv As roConversation In oConversations
                    Dim sStatus As String = String.Empty
                    Select Case oConv.Status
                        Case ConversationStatusEnum.Closed
                            sStatus = oLng.Translate("stateClosed", "messages")
                        Case ConversationStatusEnum.Dismissed
                            sStatus = oLng.Translate("stateDismissed", "messages")
                        Case ConversationStatusEnum.Ongoing
                            sStatus = oLng.Translate("stateOngoing", "messages")
                        Case ConversationStatusEnum.Pending
                            sStatus = oLng.Translate("statePending", "messages")
                    End Select

                    oCards.Add(New Card() With {
                        .Id = oConv.Id,
                        .Name = oConv.Title,
                        .Description = oConv.ReferenceNumber & " - " & oConv.CreatedByName,'oConv.LastMessageHint,
                        .CreatedOn = oConv.LastMessageTimestamp,
                        .Type = "",
                        .Icon = IIf(oConv.CreatedBy = -1, Url.Content("~/Base/Images/StartMenuIcos/Channels.png"), "/Employee/GetEmployeePhoto/" & oConv.CreatedBy),
                        .Filterfield = $"c{oConv.Id}",
                        .Filterfield2 = oConv.CreatedByName,
                        .Badge = oConv.NewMessages,
                        .Status = CInt(oConv.Status),
                        .StatusText = sStatus
                    })
                Next

            End If
            ViewData("ChannelName") = tmpChannel.Title
            ViewData("ChannelConversation") = oCards
        Else
            ViewData("ChannelName") = String.Empty
            ViewData("ChannelConversation") = oCards
        End If
    End Sub

    Public Function ConversationsCardView(idChannel As String) As ActionResult

        BuildCardViewFromConversations(idChannel)
        Return PartialView("_ConversationsCardView")

    End Function

    <HttpGet>
    Public Function GetConversationMessages(ByVal idConversation As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        Dim oConversation As roConversation = API.ChannelsServiceMethods.GetConversationById(roTypes.Any2Integer(idConversation), Nothing,, False)
        Dim oMessages As Generic.List(Of roMessage) = API.ChannelsServiceMethods.GetConversationMessages(roTypes.Any2Integer(idConversation), Nothing, True)

        API.ChannelsServiceMethods.SetConversationMessagesReaded(roTypes.Any2Integer(idConversation), Nothing)

        ViewData("Conversation") = oConversation
        ViewData("ConversationMessages") = oMessages

        Return View("_ConversationMessages")
    End Function

    <HttpPost>
    Public Function SetConversationStatus(ByVal IdConversation As Integer, ByVal Status As Integer) As JsonResult
        Dim conversation As roConversation = API.ChannelsServiceMethods.GetConversationById(IdConversation, Nothing,, False)

        If conversation IsNot Nothing AndAlso conversation.Id > 0 Then
            conversation.Status = Status

            If API.ChannelsServiceMethods.ChangeConversationState(Nothing, conversation, True) Then
                Return Json(True)
            Else
                Return Json(API.ChannelsServiceMethods.LastConversationErrorText)
            End If
        Else
            Return Json(API.ChannelsServiceMethods.LastConversationErrorText)
        End If

    End Function

    <HttpPost>
    Public Function SetConversationComplexity(ByVal IdConversation As Integer, ByVal Complexity As Integer) As JsonResult
        Dim conversation As roConversation = API.ChannelsServiceMethods.GetConversationById(IdConversation, Nothing,, False)

        If conversation IsNot Nothing AndAlso conversation.Id > 0 Then
            conversation.Complexity = Complexity

            If API.ChannelsServiceMethods.ChangeComplaintComplexity(Nothing, conversation, True) Then
                Return Json(True)
            Else
                Return Json(API.ChannelsServiceMethods.LastConversationErrorText)
            End If
        Else
            Return Json(API.ChannelsServiceMethods.LastConversationErrorText)
        End If

    End Function

    <HttpPost>
    Public Function AddMessage(ByVal IdConversation As Integer, ByVal Message As String) As JsonResult
        Dim conversation As roConversation = API.ChannelsServiceMethods.GetConversationById(IdConversation, Nothing,, False)

        If conversation IsNot Nothing AndAlso conversation.Id > 0 Then
            Dim newMessage As New roMessage
            newMessage.Conversation = conversation
            newMessage.CreatedBy = WLHelperWeb.CurrentPassportID
            newMessage.CreatedOn = Date.Now
            newMessage.Body = Message
            newMessage.IsResponse = True

            If API.ChannelsServiceMethods.CreateMessage(Nothing, newMessage, True) Then
                Return Json(True)
            Else
                Return Json(API.ChannelsServiceMethods.LastMessageErrorText)
            End If
        Else
            Return Json(API.ChannelsServiceMethods.LastMessageErrorText)
        End If

    End Function

    Public Function DEXUseAndConditions() As ActionResult
        Return View("_DEXConditionsAndUse")
    End Function

    Public Function UseAndConditionsPDF() As FileResult
        Dim bytes As Byte() = API.LiveTasksServiceMethods.GetCommonTemplateFile(Nothing, "dexConditions.pdf")
        Return File(bytes, "application/pdf")
    End Function

End Class