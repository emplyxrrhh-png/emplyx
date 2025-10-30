<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roPathWay" CodeBehind="roPathWay.ascx.vb" %>

<div>
    <div class="RoundCornerFrame roundCorner PathWayLastRemember">
        <table border="0">
            <tr>
                <td>
                    <a href="javascript: void(0);" runat="server" id="buttonIcoHome" class="buttonIcoHome" onclick="menuSelected = '';reenviaFrame('Start','','','');">
                        <asp:Label ID="lblMainMenuTitle" runat="server" Text="Principal"></asp:Label>&nbsp;&gt;&nbsp;
                    </a>
                </td>
                <td style="white-space: nowrap;">
                    <span id="lblPathWay" style="white-space: nowrap; display: block; margin-right: 10px; width: 150px;"></span>
                </td>
                <td style="white-space: nowrap;">
                    <ul>
                        <li style="display: inline;"><span class="buttonIcoHistory" style="width: 16px;"></span></li>
                        <li style="display: inline;">
                            <asp:Label ID="lblLastVisited" runat="server" Text="Últimos visitados:" Font-Bold="true"></asp:Label>&nbsp;</li>
                    </ul>
                </td>

                <td style="white-space: nowrap;">
                    <span id="LastVisitedPan" style="width: 300px;"></span>
                </td>
            </tr>
        </table>
    </div>

    <div class="PathWayAdvices" style="">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <div id="divInfoUserTasks" style="visibility: hidden; display: none; cursor: pointer; color: Red; white-space: nowrap; display: inline; padding-right: 5px;"
                        onmouseover="showUserTasksList();" onmouseout="hideUserTasksList();">
                        <%=Me.Language.Translate("divInfoUserTasks", DefaultScope)%>
                    </div>
                    <div id="divInfoUserTasksIdle" style="visibility: hidden; display: none; cursor: pointer; color: Green; white-space: nowrap; display: inline; padding-right: 5px;"
                        onmouseover="showUserTasksList();" onmouseout="hideUserTasksList();">
                        <%=Me.Language.Translate("divInfoUserTasksIdle", DefaultScope)%>
                    </div>
                </td>
                <td>
                    <img id="imgUserTasks" src="Base/Images/TasksAlertAnim.gif" style="display: none; cursor: pointer;" onmouseover="showUserTasksList(this);" onmouseout="hideUserTasksList();" />
                    <img id="imgUserTasksIdle" src="Base/Images/TasksIdle.png" style="display: none; cursor: pointer;" onmouseover="showUserTasksList(this);" onmouseout="hideUserTasksList();" />
                </td>
            </tr>
        </table>
    </div>
</div>