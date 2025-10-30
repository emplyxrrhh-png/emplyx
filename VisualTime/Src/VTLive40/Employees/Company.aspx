<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Company" Title="${Employees} y ${Groups}" EnableEventValidation="false" CodeBehind="Company.aspx.vb" %>

<%@ Register Src="~/Employees/WebUserControls/DocumentManagment.ascx" TagPrefix="roForms" TagName="DocumentManagment" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentPendingManagment.ascx" TagPrefix="roForms" TagName="DocumentPendingManagment" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
        }

        function redirectToCompanySection(s, type) {
            if (type != 'email') top.reenviaFrame(s.cpDestinationURL, '', '', '');
        }
    </script>

    <style>
        .CompanySectionDescAndBtn {
            height: 260px;
            min-height: 260px;
            max-height: 260px;
        }
        .btnDisabled {
            opacity: 0.6 !important;
            cursor: not-allowed !important;
            pointer-events: none !important;
        }
        p[id$="txtDisabledCollectives"]  {
            text-align: center;
            color: #d76d1d;
            font-weight: bold;
        }

        @media (max-width: 1450px) {
            #CompanyGroupsSection .CompanySectionDescription{
                font-size: 12px;
            }
            #CompanyGroupsSection .CompanySectionDescription.Desc1{
                margin-bottom: 0;
            }
            #CompanyGroupsSection .CompanySectionDescription.Desc2{
                margin-top: 0;
            }
            #divMainBody {
                min-height: auto !important;
                padding-bottom: 20px;
            }
            #divTabData {
                height: fit-content !important;
            }
        }
        
    </style>

    <div id="divMainBody">

        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divCompany" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img src="Images/Company80.png" height="50px" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <div class="NameText">
                        <span id="readOnlyNameCompany"><%=Me.Language.Translate("CaptionGrid", Me.DefaultScope)%></span>
                    </div>
                    <div class="DescriptionText">
                        <span id="readOnlyDescritionCompany"><%=Me.Language.Translate("DescritionCompany", Me.DefaultScope)%></span>
                    </div>
                </div>
                <div id="tbButtons" runat="server" class="blackRibbonButtons" style="padding-top: 25px">
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="display: flex; flex-flow: row nowrap; justify-content: space-between; box-sizing: border-box; height: 95%;">
                    <div class="descriptionCompanyDiv" style="" id="companyGroupsDiv" runat="server">
                        <div style="display: flex;flex-direction: column;height: 100%;justify-content: space-between;">
                            <div class="CompanySectionTitle">
                                <span><%=Me.Language.Translate("Company.GroupsAndCollectives", Me.DefaultScope)%></span>
                            </div>

                            <div class="CompanySectionGroupImage">
                            </div>
                            <div class="CompanySectionDescAndBtn" style="display:flex; margin-bottom: 10%; flex: 1;">
                                <div style="float: left;width: calc(50%);display: flex;flex-flow: column;justify-content: space-between;height: auto;align-items: center;">
                                    <div>
                                        <div class="CompanySectionMiddleTitle" style="line-height: 35px;">
                                                <span><%=Me.Language.Translate("Company.CollectivesTitle", Me.DefaultScope)%></span>
                                            </div>
                                        <div class="CompanySectionDescription">
                                            <span><%=Me.Language.Translate("Company.CollectivesDesc", Me.DefaultScope)%></span>
                                        </div>
                                    
                                    </div>
                                    <div class="CompanyActionLink">
                                        <p id="txtDisabledCollectives" runat="server"></p>
                                        <dx:aspxbutton id="btnGoToCollectives" runat="server" autopostback="False" causesvalidation="False" text="Acceder a colectivos" tooltip="" hoverstyle-cssclass="btnFlat-hover btnFlatBlack-hover" cssclass="btnFlat btnFlatBlack">
                                            <clientsideevents click="function(s,e){redirectToCompanySection(s,'collectives');}" />
                                        </dx:aspxbutton>
                                    </div>
                                </div>
                                <div style="float: left;width: calc(50%);display: flex;flex-flow: column;justify-content: space-between;height: auto;align-items: center;">
                                    <div id="CompanyGroupsSection">
                                        <div class="CompanySectionMiddleTitle" style="line-height: 35px;">
                                                <span><%=Me.Language.Translate("Company.Groups", Me.DefaultScope)%></span>
                                            </div>
                                        <div class="CompanySectionDescription Desc1">
                                            <span><%=Me.Language.Translate("Company.GroupsDescription", Me.DefaultScope)%></span>
                                        </div>
                                        <div class="CompanySectionDescription Desc2">
                                            <span><%=Me.Language.Translate("Company.GroupsDescription2", Me.DefaultScope)%></span>
                                        </div>
                                    </div>
                                    <div class="CompanyActionLink">
                                        <dx:aspxbutton id="btnGoToGroups" runat="server" autopostback="False" causesvalidation="False" text="Acceder a grupos" tooltip="" hoverstyle-cssclass="btnFlat-hover btnFlatBlack-hover" cssclass="btnFlat btnFlatBlack">
                                            <clientsideevents click="function(s,e){redirectToCompanySection(s,'groups');}" />
                                        </dx:aspxbutton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="descriptionCompanyDiv" style="display: none; height: auto !important;" id="companySecurityModeV3" runat="server">
                        <div style="display: flex;flex-direction: column;height: 100%;justify-content: space-between;">
                            <div class="CompanySectionTitle">
                                <span><%=Me.Language.Translate("Company.SupervisorsAndRoles", Me.DefaultScope)%></span>
                            </div>

                            <div class="CompanySectionSupervisorsImage">
                            </div>
                            <!--div class="CompanySectionMiddleTitle" style="line-height: 35px;">
                                        <span>&nbsp;</span>
                                    </div-->

                            <div class="CompanySectionDescAndBtn" style="display:flex; margin-bottom: 10%; flex: 1;">
                                <div style="float: left;width: calc(50%);display: flex;flex-flow: column;justify-content: space-between;height: auto;align-items: center;">
                                    <div >
                                        <div class="CompanySectionMiddleTitle">
                                            <span><%=Me.Language.Translate("Company.Supervisors", Me.DefaultScope)%></span>
                                        </div>
                                        <div class="CompanySectionDescription">
                                            <span><%=Me.Language.Translate("Company.SupervisorsDescription", Me.DefaultScope)%></span>
                                        </div>
                                    </div>
                                    <div class="CompanyActionMiddleLink">
                                        <dx:aspxbutton id="btnGoToSupervisorsV3" runat="server" autopostback="False" causesvalidation="False" text="Acceder a supervisores" tooltip="" hoverstyle-cssclass="btnFlat-hover btnFlatBlack-hover" cssclass="btnFlat btnFlatBlack">
                                            <clientsideevents click="function(s,e){redirectToCompanySection(s,'passports');}" />
                                        </dx:aspxbutton>
                                    </div>
                                </div>
                                <div style="float: left;width: calc(50%);display: flex;flex-flow: column;justify-content: space-between;height: auto;align-items: center;">
                                    <div style="">
                                        <div class="CompanySectionMiddleTitle">
                                            <span><%=Me.Language.Translate("Company.SecurityFunction", Me.DefaultScope)%></span>
                                        </div>
                                        <div class="CompanySectionDescription">
                                            <span><%=Me.Language.Translate("Company.SecurityFunctionDescription", Me.DefaultScope)%></span>
                                        </div>
                                    </div>
                                    <div class="CompanyActionMiddleLink">
                                        <dx:aspxbutton id="btnGoToRolesV3" runat="server" autopostback="False" causesvalidation="False" text="Acceder a roles" tooltip="" hoverstyle-cssclass="btnFlat-hover btnFlatBlack-hover" cssclass="btnFlat btnFlatBlack">
                                            <clientsideevents click="function(s,e){redirectToCompanySection(s,'roles');}" />
                                        </dx:aspxbutton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function resizeFrames() {
            var divMainBodyHeight = $("#divMainBody").outerHeight(true);
            var divHeight = 0;
            if (divMainBodyHeight < 525) {
                divHeight = 525 - $("#divTabInfo").outerHeight(true);
            }
            else {
                divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
            }

            $("#divTabData").height(divHeight - 10);
        }

        window.onresize = function () {
            resizeFrames();
        }
    </script>
</asp:Content>