<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="indexv2.aspx.vb" Inherits="VTPortalWeb.indexv2" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>VTPortal</title>

    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="msapplication-tap-highlight" content="no" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />

    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/dx.common.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/dx.spa.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/dx.android5.light.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/dx.VTPortal.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/VTPortal.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/survey.min.css") & Me.MasterVersion %>" />
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("css/VTPortal.ie11.css") & Me.MasterVersion %>" />

    <!--[if lte IE 9]>
    <link rel="stylesheet" type="text/css" href="../css/VTPortal.ie9.css<%= "" & Me.MasterVersion %>" />
    <![endif]-->

    <script type="text/javascript" src="../js/jquery-3.7.1.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/knockout-3.4.2.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/survey.jquery.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/emotion-ratings.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/surveyjs-widgets.js<%= "" & Me.MasterVersion %>"></script>
    <!-- Language JS classes-->
    <script type="text/javascript" src="../js/cldr.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/cldr/event.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/cldr/supplemental.min.js<%= "" & Me.MasterVersion %>"></script>

    <script type="text/javascript" src="../js/globalize.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/globalize/message.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/globalize/number.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/globalize/date.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/globalize/currency.min.js<%= "" & Me.MasterVersion %>"></script>

    <script type="text/javascript" src="../js/dx.all.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Language JS classes-->
    <script type="text/javascript" src="../js/plugins/i18next.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/plugins/i18next.ko.min.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Moment JS Libraries -->
    <script language="javascript" type="text/javascript" src="../js/plugins/moment.min.js<%= "" & Me.MasterVersion %>"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/moment-timezone.min.js<%= "" & Me.MasterVersion %>"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/timezone.detect.js<%= "" & Me.MasterVersion %>"></script>

    <script language="javascript" type="text/javascript" src="../js/plugins/aes.js<%= "" & Me.MasterVersion %>"></script>
    <!-- Js ext framework -->
    <script language="javascript" type="text/javascript" src="../js/plugins/sugar.min.js<%= "" & Me.MasterVersion %>"></script>
    <!-- Cookie -->
    <script language="javascript" type="text/javascript" src="../js/plugins/js.cookie.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Robotics JS classes-->
    <script type="text/javascript" src="js/roVTPortal.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/roVTPortalViews.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="../js/ro.validate.url.min.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Gmaps framework -->
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyC5Jcssn8H6tPktYPYwdPP7jwmoDxSkpuE&callback=Function.prototype&loading=async"></script>

    <!-- Application -->
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("index.css") & Me.MasterVersion %>" />
    <script type="text/javascript" src="app.config.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="index.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Layouts -->
    <script type="text/javascript" src="layouts/Empty/EmptyLayout.js<%= "" & Me.MasterVersion %>"></script>
    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("layouts/Empty/EmptyLayout.css") & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="layouts/Empty/EmptyLayout.html<%= "" & Me.MasterVersion %>" />

    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("layouts/Navbar/NavbarLayout.css") & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="layouts/Navbar/NavbarLayout.html<%= "" & Me.MasterVersion %>" />
    <script type="text/javascript" src="layouts/Navbar/NavbarLayout.js<%= "" & Me.MasterVersion %>"></script>

    <link rel="stylesheet" type="text/css" href="<%=Me.ResolveUrl("layouts/Pivot/PivotLayout.css") & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="layouts/Pivot/PivotLayout.html<%= "" & Me.MasterVersion %>" />
    <script type="text/javascript" src="layouts/Pivot/PivotLayout.js<%= "" & Me.MasterVersion %>"></script>

    <!-- Views -->
    <link rel="dx-template" type="text/html" href="views/Login.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/status.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/alerts.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/punches.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/documents.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/loading.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/TimeGate.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/partial/bigUserInfo.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/partial/documentInput.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/partial/statusHeader.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/partial/TaskFieldsInput.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/partial/noPermissions.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/actions/SelectListObject.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/actions/taskSelector.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/actions/RequestFilters.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/actions/DocumentFilters.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/actions/sendDocument.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/actions/punchInfo.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/leaves/leaves.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/leaves/myLeaves.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/leaves/myLeavesAlerts.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/leaves/sendLeaveDocument.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/leaves/newLeave.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/teleworking/teleworking.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/teleworking/myWork.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/teleworking/myWorkDaily.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/teleworking/myWorkRequests.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/scheduler/scheduler.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/myCalendar.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/myAccruals.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/myAuthorizations.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/myRequests.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/dayInfo.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/scheduler/capacityDetail.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/punches/punchManagement.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/punches/myPunches.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/punches/myPunchRequests.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/profile/profile.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/profile/profileConfig.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/profile/profileUserFields.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/myteam/myteam.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/myteam/myTeamAlerts.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/myteam/myTeamAlertsDetail.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/myteam/myTeamDailyRecords.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/myteam/myTeamEmployees.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/myteam/myTeamRequests.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/requests/requestsList.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/approvalHistory.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/userFields.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/forgottenPunch.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/justifyPunch.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/externalWork.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/externalWorkWeekResume.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/telecommute.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/changeShift.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/plannedAbsence.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/plannedCause.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/plannedHoliday.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/cancelHolidays.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/plannedOvertime.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requests/shiftExchange.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/requestsNew/absence.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requestsNew/forgotten.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requestsNew/telecommuting.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requestsNew/changeShiftNew.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requestsNew/cancelHolidaysNew.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/home/summaryHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/notificationsHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/dailyRecordHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/accrualsHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/punchesHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/surveysHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/emptyHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/communiquesHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/channelsHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/telecommutingHome.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/home/webLinksHome.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/communiques/communiqueDetail.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/communiques/communiques.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/channels/myChannels.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/channels/channelConversations.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/channels/conversationMessages.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/channels/addMessage.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/survey/surveyDetail.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/survey/surveys.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/onboarding/onboardingDetail.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/onboarding/onboardings.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/dailyRecord/addDailyRecord.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/requestsNew/dailyRecordDetail.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/dailyRecord/myDailyRecord.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/weblinks/myWebLinks.dxview<%= "" & Me.MasterVersion %>" />

    <link rel="dx-template" type="text/html" href="views/configuration/configuration.dxview<%= "" & Me.MasterVersion %>" />
    <link rel="dx-template" type="text/html" href="views/configuration/sharedPortalConfig.dxview<%= "" & Me.MasterVersion %>" />
</head>
<body>
</body>
</html>