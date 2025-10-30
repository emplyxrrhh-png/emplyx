<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roUserCtlFieldCriteriaV2" CodeBehind="roUserCtlFieldCriteriaV2.ascx.vb" %>

<script type="text/javascript">

    var <%= Me.Prefix%>_selected1Index = -1;
    var <%= Me.Prefix%>_selected2Index = -1;
    var <%= Me.Prefix%>_selected3Index = -1;

    function setroUserCtlFieldCriteriaEnabled(Prefix, status) {
        var cmb1Client = null;
        var cmb2Client = null;
        var cmb3Client = null;
        var cmb4Client = null;

        eval("cmb1Client = " + Prefix + "_cmbCriteria1Client");
        eval("cmb2Client = " + Prefix + "_cmbCriteria2Client");
        eval("cmb3Client = " + Prefix + "_cmbCriteria3Client");
        eval("cmb4Client = " + Prefix + "_cmbVisibilityValueClient");

        cmb1Client.SetEnabled(status);
        cmb2Client.SetEnabled(status);
        cmb3Client.SetEnabled(status);
        cmb4Client.SetEnabled(status);
    }

    function setSelectedIndexes(Prefix, indexes) {
        var cmb1Client = null;
        var cmb2Client = null;
        var cmb3Client = null;
        var cmb4Client = null;

        var curIndexes = indexes.split(",");

        eval("cmb1Client = " + Prefix + "_cmbCriteria1Client");
        eval("cmb2Client = " + Prefix + "_cmbCriteria2Client");
        eval("cmb3Client = " + Prefix + "_cmbCriteria3Client");
        eval("cmb4Client = " + Prefix + "_cmbVisibilityValueClient");

        eval(Prefix + "_selected1Index = " + curIndexes[0] + ";");
        eval(Prefix + "_selected2Index = " + curIndexes[1] + ";");
        eval(Prefix + "_selected3Index = " + curIndexes[2] + ";");

        cmb1Client.SetSelectedIndex(curIndexes[0]);
        cmb2Client.SetSelectedIndex(curIndexes[1]);
        cmb3Client.SetSelectedIndex(curIndexes[2]);
        if (curIndexes[3] != "") cmb4Client.SetSelectedIndex(curIndexes[3]);

    }

    function resetUserCriteriaControl(Prefix) {
        var cmb1Client = null;
        var cmb2Client = null;
        var cmb3Client = null;
        var cmb4Client = null;

        eval("cmb1Client = " + Prefix + "_cmbCriteria1Client");
        eval("cmb2Client = " + Prefix + "_cmbCriteria2Client");
        eval("cmb3Client = " + Prefix + "_cmbCriteria3Client");
        eval("cmb4Client = " + Prefix + "_cmbVisibilityValueClient");
        cmb1Client.SetValue("");
        cmb2Client.ClearItems();
        cmb3Client.ClearItems();
        cmb4Client.ClearItems();

        var panValCmb = document.getElementById(Prefix + "_panVValueComboBox");
        var panValMsk = document.getElementById(Prefix + "_panVValueMaskTextBox");
        var panValMskTime = document.getElementById(Prefix + "_panVValueMaskTextBoxTime");
        var panValTxt = document.getElementById(Prefix + "_panVValueTextBox");
        var panValPeriod = document.getElementById(Prefix + "_panVValuePeriods");
        var panValTimePeriod = document.getElementById(Prefix + "_panVValueTimePeriods");

        panValCmb.style.display = 'none';
        panValMsk.style.display = 'none';
        panValMskTime.style.display = 'none';
        panValTxt.style.display = 'none';
        panValPeriod.style.display = 'none';
        panValTimePeriod.style.display = 'none';
    }

    function hiddenPans(Prefix) {

        var ctrlPrefix = Prefix;

        var panValCmb = document.getElementById(ctrlPrefix + "_panVValueComboBox");
        var panValMsk = document.getElementById(ctrlPrefix + "_panVValueMaskTextBox");
        var panValMskTime = document.getElementById(ctrlPrefix + "_panVValueMaskTextBoxTime");
        var panValTxt = document.getElementById(ctrlPrefix + "_panVValueTextBox");
        var panValPeriod = document.getElementById(ctrlPrefix + "_panVValuePeriods");
        var panValTimePeriod = document.getElementById(ctrlPrefix + "_panVValueTimePeriods");

        panValCmb.style.display = 'none';
        panValMsk.style.display = 'none';
        panValMskTime.style.display = 'none';
        panValTxt.style.display = 'none';
        panValPeriod.style.display = 'none';
        panValTimePeriod.style.display = 'none';
    }

    function chkComboField(s, e, Prefix) {

        var ctrlPrefix = Prefix;

        var cmb1Client = s;
        var cmb2Client = null;
        var cmb3Client = null;
        var cmb4Client = null;

        eval("cmb2Client = " + ctrlPrefix + "_cmbCriteria2Client");
        eval("cmb3Client = " + ctrlPrefix + "_cmbCriteria3Client");
        eval("cmb4Client = " + ctrlPrefix + "_cmbVisibilityValueClient");
        cmb4Client.ClearItems();

        eval(Prefix + "_selected1Index = " + s.GetSelectedIndex());
        eval(Prefix + "_selected2Index = " + -1);
        eval(Prefix + "_selected3Index = " + -1);

        hiddenPans(Prefix);
        var cmbValue = cmb1Client.GetValue().substring(cmb1Client.GetValue().indexOf("*|*") + 3);
        cmb2Client.ClearItems();
        //text
        if (cmbValue == "0") {
            cmb2Client.AddItem("<%= Me.CriteriaEquals%>", 'Equals');
            cmb2Client.AddItem("<%= Me.CriteriaDifferent%>", 'Different');
            cmb2Client.AddItem("<%= Me.CriteriaStartsWith%>", 'StartsWith');
            cmb2Client.AddItem("<%= Me.CriteriaContains%>", 'Contains');
        }
        //Numeric (1), Data (2), Decimal (3), Hora (4)
        else if (cmbValue == "1" || cmbValue == "2" || cmbValue == "3" || cmbValue == "4") {
            cmb2Client.AddItem("<%= Me.CriteriaEquals%>", 'Equals');
            cmb2Client.AddItem("<%= Me.CriteriaMajor%>", 'Major');
            cmb2Client.AddItem("<%= Me.CriteriaMajorOrEquals%>", 'MajorOrEquals');
            cmb2Client.AddItem("<%= Me.CriteriaMinor%>", 'Minor');
            cmb2Client.AddItem("<%= Me.CriteriaMinorOrEquals%>", 'MinorOrEquals');
            cmb2Client.AddItem("<%= Me.CriteriaDifferent%>", 'Different');
        }
        //Llista de valors
        else if (cmbValue == "5") {
            cmb2Client.AddItem("<%= Me.CriteriaContains%>", 'Contains');
            cmb2Client.AddItem("<%= Me.CriteriaNoContains%>", 'NoContains');
        }
        //Periodes de data / hora
        else if (cmbValue == "6" || cmbValue == "7") {
            cmb2Client.AddItem("<%= Me.CriteriaEquals%>", 'Equals');
            cmb2Client.AddItem("<%= Me.CriteriaContains%>", 'Contains');
        }

        cmb3Client.ClearItems();

        //Texte(0), Numeric(1), Decimal(3)
        if (cmbValue == "0" || cmbValue == "1" || cmbValue == "3") {
            cmb3Client.AddItem("<%= Me.CriteriaTheValue%>", 'TheValue');
        }
        //Data (2)
        else if (cmbValue == "2") {
            cmb3Client.AddItem("<%= Me.CriteriaTheDate%>", 'TheDate');
        }
        //Hora (4)
        else if (cmbValue == "4") {
            cmb3Client.AddItem("<%= Me.CriteriaTheTime%>", 'TheTime');
        }
        //Llista de valors
        else if (cmbValue == "5") {
            cmb3Client.AddItem("<%= Me.CriteriaTheValue%>", 'TheValue');
        }
        //Periodes de data
        else if (cmbValue == "6") {
            cmb3Client.AddItem("<%= Me.CriteriaThePeriod%>", 'ThePeriod');
            cmb3Client.AddItem("<%= Me.CriteriaTheDate%>", 'TheDate');
        }
        //Periodes de hora
        else if (cmbValue == "7") {
            cmb3Client.AddItem("<%= Me.CriteriaThePeriod%>", 'ThePeriod');
            cmb3Client.AddItem("<%= Me.CriteriaTheTime%>", 'TheTime');
        }

        if (cmbValue == "5") {
            loadComboVisibilityParms(Prefix);
        }

        try {
            valueChanged();
        }
        catch (err) {

        }
    }

    function checkCombo2(s, e, Prefix) {
        hiddenPans(Prefix);

        var ctrlPrefix = Prefix;

        var cmb1Client = null;
        var cmb3Client = null;

        var sIndex1 = -1;
        var sIndex3 = -1;
        eval(Prefix + "_selected2Index = " + s.GetSelectedIndex() + ";");
        eval("sIndex1 = " + Prefix + "_selected1Index;");
        eval("sIndex3 = " + Prefix + "_selected3Index;");

        eval("cmb1Client = " + ctrlPrefix + "_cmbCriteria1Client");
        eval("cmb3Client = " + ctrlPrefix + "_cmbCriteria3Client");

        if (sIndex1 > -1 && sIndex3 > -1) {
            checkCombo3(s, e, false, Prefix);
        }

        try {
            valueChanged();
        }
        catch (err) {

        }
    }

    function checkCombo3(s, e, update, Prefix) {

        hiddenPans(Prefix);
        var ctrlPrefix = Prefix;

        var cmb1Client = null;
        var cmb3Client = null;

        var sIndex1 = -1;
        var sIndex3 = -1;
        eval("sIndex1 = " + Prefix + "_selected1Index;");
        eval("sIndex3 = " + Prefix + "_selected3Index;");

        if (update) {
            eval(Prefix + "_selected3Index = " + s.GetSelectedIndex() + ";");
            eval("sIndex3 = " + Prefix + "_selected3Index;");
        }

        eval("cmb1Client = " + ctrlPrefix + "_cmbCriteria1Client");
        eval("cmb3Client = " + ctrlPrefix + "_cmbCriteria3Client");

        var val1 = cmb1Client.GetItem(sIndex1).value.substring(cmb1Client.GetItem(sIndex1).value.indexOf("*|*") + 3);
        var val2 = cmb3Client.GetItem(sIndex3).value;

        var panValCmb = document.getElementById(ctrlPrefix + "_panVValueComboBox");
        var panValMsk = document.getElementById(ctrlPrefix + "_panVValueMaskTextBox");
        var panValMskTime = document.getElementById(ctrlPrefix + "_panVValueMaskTextBoxTime");
        var panValTxt = document.getElementById(ctrlPrefix + "_panVValueTextBox");
        var panValPeriod = document.getElementById(ctrlPrefix + "_panVValuePeriods");
        var panValTimePeriod = document.getElementById(ctrlPrefix + "_panVValueTimePeriods");

        //Texte(0),Numeric(1),Decimal(3)
        if (val1 == "0" || val1 == "1" || val1 == "3") {
            panValTxt.style.display = '';
        } else if (val1 == "2") { //Data
            panValMsk.style.display = '';
        } else if (val1 == "4") { //Hora
            panValMskTime.style.display = '';
        } else if (val1 == "5") { //Llista de valors
            panValCmb.style.display = '';
        } else if (val1 == "6") { //Periodes de data
            if (val2 == "TheDate") {
                panValMsk.style.display = "";
            }
            else if (val2 == "ThePeriod") {
                panValPeriod.style.display = "";
            }
        } else if (val1 == "7") {
            if (val2 == "TheTime") {
                panValMskTime.style.display = "";
            } else if (val2 == "ThePeriod") {
                panValTimePeriod.style.display = "";
            }
        } else {
            panValTxt.style.display = '';
        }

        try {
            valueChanged();
        }
        catch (err) {

        }
    }

    function loadComboVisibilityParms(Prefix) {
        var cmb1Client = null;
        var cmb4Client = null;

        var ctrlPrefix = Prefix;

        eval("cmb1Client = " + ctrlPrefix + "_cmbCriteria1Client");
        eval("cmb4Client = " + ctrlPrefix + "_cmbVisibilityValueClient");

        var sIndex1 = "";
        eval("sIndex1 = " + Prefix + "_selected1Index;")

        var val1 = cmb1Client.GetItem(sIndex1).value.substring(0, cmb1Client.GetItem(sIndex1).value.indexOf("*|*"));

        cmb4Client.PerformCallback("LOADFIELDS#" + val1);
        //var stamp = '&StampParam=' + new Date().getMilliseconds();
        //var ajax = nuevoAjax();
        //ajax.open("GET", "../Base/WebUserControls/handlers/srvUserFields.ashx?action=getUserFieldValues&FIELD_NAME=" + val1 + stamp, true);

        //ajax.onreadystatechange = function () {
        //    if (ajax.readyState == 4) {
        //        var values = ajax.responseText.split("*");
        //        for (var index = 0; index < values.length; index++) {
        //            cmb4Client.AddItem(values[index], values[index]);
        //        }
        //    }
        //}

        //ajax.send(null);
    }

    function selectFieldListValue(s, Prefix) {
        //var sIndex = s.GetSelectedIndex();

        //var cmb4Client = null;
        //eval("cmb4Client = " + controlPrefix + "_cmbVisibilityValueClient");

        //cmb4Client.SetSelectedItem(cmb4Client.GetItem(sIndex));

        try {
            valueChanged();
        }
        catch (err) {

        }
    }

    function valueChanged() {
        try {
            hasChanges(true, false);
        }
        catch (err) {

        }
    }
</script>

<table border="0" style="padding: 20px,0px,20px,20px; padding-top: 5px; text-align: center">
    <tr>
        <td style="text-align: right; width: 100px; max-width: 100px">
            <asp:Label ID="lblCriteria1" runat="server" Text="Campo de la ficha" />
        </td>
        <td colspan="2" style="text-align: left; padding-left: 5px">
            <dx:ASPxComboBox ID="cmbCriteria1" runat="server" Width="200px" ClientInstanceName="cmbCriteria1Client" EnableCallbackMode="true" EnableSynchronization="True">
                <ClientSideEvents SelectedIndexChanged="function(s,e){valueChanged();}" />
            </dx:ASPxComboBox>
        </td>
    </tr>
    <tr>
        <td style="text-align: right; width: 100px; max-width: 100px">
            <asp:Label ID="lblCriteria2" runat="server" Text="Condición" />
        </td>
        <td style="text-align: left; padding-left: 5px; width: 200px; max-width: 200px">
            <dx:ASPxComboBox ID="cmbCriteria2" runat="server" Width="200px" ClientInstanceName="cmbCriteria2Client" EnableCallbackMode="true" EnableSynchronization="True">
                <ClientSideEvents SelectedIndexChanged="function(s,e){checkCombo2(s,e);valueChanged();}" />
            </dx:ASPxComboBox>
        </td>
        <td style="text-align: left; padding-left: 5px; width: 200px; max-width: 200px">
            <dx:ASPxComboBox ID="cmbCriteria3" runat="server" Width="200px" ClientInstanceName="cmbCriteria3Client" EnableCallbackMode="true" EnableSynchronization="True">
                <ClientSideEvents SelectedIndexChanged="function(s,e){checkCombo3(s,e,true);valueChanged();}" />
            </dx:ASPxComboBox>
        </td>
    </tr>
    <tr id="trVisibilityCriteria">
        <td style="text-align: right; width: 100px; max-width: 100px">
            <asp:Label ID="lblValue" runat="server" Text="Valor" />
        </td>
        <td colspan="2" style="text-align: left; padding-left: 5px">
            <table style="border-spacing: 0px">
                <tr id="panVValueTextBox" runat="server" style="display: none;">
                    <td>
                        <dx:ASPxTextBox ID="txtVisibilityValue" runat="server" Width="300">
                            <ClientSideEvents TextChanged="function(s,e){ valueChanged() }" />
                        </dx:ASPxTextBox>
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 0px">
                <tr id="panVValueMaskTextBox" runat="server" style="display: none;">
                    <td>
                        <dx:ASPxDateEdit ID="mskVisibilityValueDate" PopupVerticalAlign="WindowCenter" runat="server" Width="105" AllowNull="false">
                            <ClientSideEvents DateChanged="function(s,e){ valueChanged() }" />
                        </dx:ASPxDateEdit>
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 0px">
                <tr id="panVValueMaskTextBoxTime" runat="server" style="display: none;">
                    <td>
                        <dx:ASPxTextBox ID="mskVisibilityValueTime" runat="server" Width="70">
                            <MaskSettings Mask="<0000..9999>:<00..59>" IncludeLiterals="None" />
                            <ClientSideEvents TextChanged="function(s,e){ valueChanged() }" />
                        </dx:ASPxTextBox>
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 0px">
                <tr id="panVValueComboBox" runat="server" style="display: none;">
                    <td>
                        <dx:ASPxComboBox ID="cmbVisibilityValue" Width="200px" runat="server" ClientInstanceName="cmbVisibilityValueClient" EnableSynchronization="True" EnableCallbackMode="true">
                            <ClientSideEvents SelectedIndexChanged="function(s,e){selectFieldListValue(s);valueChanged();}" />
                        </dx:ASPxComboBox>
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 0px">
                <tr id="panVValuePeriods" runat="server" style="display: none;">
                    <td>
                        <table>
                            <tr>
                                <td>De</td>
                                <td>
                                    <dx:ASPxDateEdit ID="mskDatePeriod1" PopupVerticalAlign="WindowCenter" runat="server" Width="105" AllowNull="false">
                                        <ClientSideEvents DateChanged="function(s,e){ valueChanged() }" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>a</td>
                                <td>
                                    <dx:ASPxDateEdit ID="mskDatePeriod2" PopupVerticalAlign="WindowCenter" runat="server" Width="105" AllowNull="false">
                                        <ClientSideEvents DateChanged="function(s,e){ valueChanged() }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 0px">
                <tr id="panVValueTimePeriods" runat="server" style="display: none;">
                    <td>
                        <table>
                            <tr>
                                <td>De</td>
                                <td>
                                    <dx:ASPxTextBox ID="mskTimePeriod1" runat="server" Width="70">
                                        <MaskSettings Mask="<0000..9999>:<00..59>" IncludeLiterals="None" />
                                        <ClientSideEvents TextChanged="function(s,e){ valueChanged() }" />
                                    </dx:ASPxTextBox>
                                </td>
                                <td>a</td>
                                <td>
                                    <dx:ASPxTextBox ID="mskTimePeriod2" runat="server" Width="70">
                                        <MaskSettings Mask="<0000..9999>:<00..59>" IncludeLiterals="None" />
                                        <ClientSideEvents TextChanged="function(s,e){ valueChanged() }" />
                                    </dx:ASPxTextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>