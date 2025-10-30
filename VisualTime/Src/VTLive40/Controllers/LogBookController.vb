Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports System.Windows.Input
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports VTLive40.CardsTree.Model
Imports VTLive40.Controllers.Base
Imports WLHelperWeb = Robotics.Web.Base.WLHelperWeb

<PermissionsAtrribute(FeatureAlias:="Employees.Complaints", Permission:=Permission.Admin)>
<LoggedInAtrribute(Requiered:=True)>
Public Class LogBookController
    Inherits BaseController

    Private requieredMessageLabels = {"sendButton", "conversationStatus", "ongoing", "closed", "dismissed", "statePending", "stateOngoing", "stateClosed", "stateDismissed", "SendPlaceHolder", "sendedAt", "by", "at", "emptylist"}
    Private requieredConvLabels = {"roComplaintId", "roComplaintIdDesc", "roShowComplaint", "roLogBookTitle", "roLogBookInfo", "roPrintComplaint"}

    Function Index() As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Conversations", requieredConvLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        Return View("index")
    End Function

    <HttpGet>
    Public Function ShowComplaint(ByVal idComplaint As String) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Messages", requieredMessageLabels, "LiveChannels") _
                          .SetViewInfo("LiveChannels", "Channels", "Title", "Title", "Base/Images/StartMenuIcos/Channels.png", "TitleDesc")

        Dim oMessages = API.LogBookServiceMehods.GetComplaintLog(idComplaint, Nothing, True)
        ViewData("ConversationMessages") = oMessages

        Return View("_ComplaintLog")

    End Function

End Class