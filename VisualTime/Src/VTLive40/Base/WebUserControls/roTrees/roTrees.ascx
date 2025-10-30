<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTrees_roTrees" CodeBehind="roTrees.ascx.vb" %>

<table border="0" cellpadding="0" cellspacing="0" width="100%" style="height: 100%;">
    <tr>
        <td id="Tabs" runat="server" valign="top" style="width: 20px; padding-top: 15px;">
            <!-- Tabs -->
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree01" class="tabSel1-active" onkeypress="KeyPressTabTree01(event);"></a></td>
                </tr>
                <tr>
                    <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree02" class="tabSel2" onkeypress="KeyPressTabTree02(event);"></a></td>
                </tr>
                <tr>
                    <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree03" class="tabSel3" onkeypress="KeyPressTabTree03(event);"></a></td>
                </tr>
            </table>
        </td>
        <td valign="top" style="padding-top: 2px; padding-right: 2px;" id="<%= Me.ClientID %>_td">

            <!-- Arbre #1 Normal -->
            <div id="<%= Me.ClientID %>_tree-div" style="display: block; position: relative; overflow: auto; width: 100%; height: 200px; text-align: left; border: solid 1px silver;"></div>

            <!-- Arbre #2 Agrupar Por -->
            <div id="<%= Me.ClientID %>_tree-div-UF" style="display: block; width: 99%; height: 200px; height: 99%; text-align: left; border: solid 1px silver; display: none;">
                <table width="100%" style="height: 100%" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td valign="top" height="50px"><span style="display: block; width: 200px; padding-left: 6px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                            <asp:Label ID="lblAgruparPor" runat="server" Text="Agrupar por:"></asp:Label></span>
                            <!-- Combo Agrupació -->
                            <div style="width: 200px; padding: 2px; text-align: left;">
                                <roWebControls:roComboBox runat="server" ID="cmbAgrupacio" Width="180px" Height="14px" AutoResizeChildsWidth="true" ParentHeight="14px" ParentWidth="170px" ChildsHeight="14px" ChildsWidth="200px" ItemsRunAtServer="false" ChildsVisible="10" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" id="<%= Me.ClientID %>_td_UF" width="100%" height="100%">
                            <div id="<%= Me.ClientID %>_tree-div2" style="position: relative; overflow: auto; width: 99%; height: 99%; text-align: left; border-top: solid 1px #BFBFBF;"></div>
                        </td>
                    </tr>
                </table>
            </div>

            <!-- Arbre #3 Buscar Por -->
            <div id="<%= Me.ClientID %>_tree-div-FF" style="border-top: solid 1px silver; width: 99%; height: 100%; text-align: left; border: solid 1px silver; display: none;">
                <table width="100%" style="height: 99%" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td valign="top" height="26px">
                            <span style="display: block; width: 200px; padding-left: 8px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                                <asp:Label ID="lblFindText" runat="server" Text="Buscar por:"></asp:Label></span>
                            <!-- Combo Agrupació -->
                            &nbsp;&nbsp;<roWebControls:roComboBox runat="server" ID="cmbFieldFind" Width="170px" Height="14px" AutoResizeChildsWidth="true" ParentHeight="14px" ParentWidth="170px" ChildsHeight="14px" ChildsWidth="200px" ItemsRunAtServer="false" ChildsVisible="10" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" height="20px">
                            <div style="width: 200px; padding: 2px; padding-left: 5px; text-align: left;">
                                <input type="text" id="<%= Me.ClientID %>_FieldFindValue" class="textClass" style="padding: 2px; *padding: 0; width: 205px; *width: 180px;"
                                    onkeypress="FieldFindChanged(event,'<%= Me.ClientID %>');" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="100%" style="height: 100%;" id="<%= Me.ClientID %>_td_FF">
                            <div id="<%= Me.ClientID %>_tree-div3" style="overflow: auto; width: 98%; height: 98%; text-align: left; border-top: solid 1px #BFBFBF;"></div>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript" language="javascript">
    eval("var <%= Me.ClientID %>_roTrees;"); //Variable Global i dinamica de l'arbre
    eval("var <%= Me.ClientID %>_Path;"); //Path d'acces

    //declara la clase dels arbres dinamicament i globalment
    //Reposició del Path per crides js
    eval("<%= Me.ClientID %>_Path = '<%= Me.ResolvePath %>';");

    //Constructor de roTrees(Prefixe,Arbre1,MultiSeleccio1, Solsgrups1, funcio1, Arbre2,...)
    var createScript = "<%= Me.ClientID %>_roTrees = new roTrees('<%= Me.ClientID %>',";
    createScript += "<%= Me.Tree1Visible.ToString.ToLower  %>,<%= Me.Tree1MultiSel.ToString.ToLower  %>,<%= Me.Tree1ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree1Function %>','<%= Me.Tree1SelectorPage %>','<%= Me.Tree1ImagePath %>',<%= Me.Tree1EnableDD.ToString.ToLower  %>,'<%= Me.Tree1FunctDD %>',";
    createScript += "<%= Me.Tree2Visible.ToString.ToLower  %>,<%= Me.Tree2MultiSel.ToString.ToLower  %>,<%= Me.Tree2ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree2Function %>','<%= Me.Tree2SelectorPage %>','<%= Me.Tree2ImagePath %>',<%= Me.Tree2EnableDD.ToString.ToLower  %>,'<%= Me.Tree2FunctDD %>',";
    createScript += "<%= Me.Tree3Visible.ToString.ToLower  %>,<%= Me.Tree3MultiSel.ToString.ToLower  %>,<%= Me.Tree3ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree3Function %>','<%= Me.Tree3SelectorPage %>','<%= Me.Tree3ImagePath %>',";
    createScript += "'<%= Me.FeatureAlias %>','<%= Me.FeatureType %>',<%= Me.FirstClick.ToString.ToLower %>);";

    eval(createScript);


    let initTreeSelector_<%= Me.ClientID %> = function () {
        chTree(getActiveTreeType('<%= Me.ClientID %>'), '<%= Me.ClientID %>');
        //Redimensio dels arbres
        eval('<%= Me.ClientID %>_resizeTrees()');
    }


    if (window.addEventListener) {
        window.addEventListener('load', initTreeSelector_<%= Me.ClientID %>)
    } else {
        window.attachEvent('onload', initTreeSelector_<%= Me.ClientID %>)
    }


    

    window.onresize = function () {
        eval('<%= Me.ClientID %>_resizeTrees()');
    }

    function <%= Me.ClientID %>_resizeTrees() {

        //Redimensio arbre #1
        var td1 = document.getElementById('<%= Me.ClientID %>_td');
        var ar1 = document.getElementById('<%= Me.ClientID %>_tree-div');

        //Redimensio arbre #2
        var td2 = document.getElementById('<%= Me.ClientID %>_td_UF');
        var cn2 = document.getElementById('<%= Me.ClientID %>_tree-div-UF');
        var ar2 = document.getElementById('<%= Me.ClientID %>_tree-div2');

        //Redimensio arbre #3
        var td3 = document.getElementById('<%= Me.ClientID %>_td_FF');
        var cn3 = document.getElementById('<%= Me.ClientID %>_tree-div-FF');
        var ar3 = document.getElementById('<%= Me.ClientID %>_tree-div3');

        var oWidth, oHeight;
        var oHeight2, oHeight3;

        //PPR 07/09/2012
        //if(typeof MinMaxSelector == "function"){
        //    var obj = document.getElementById('<%= Me.roMainSelectorID %>_panMaximize')
        //    if(obj != null){
        //        obj.style.height=document.body.offsetHeight - 130 + 'px';
        //    }
        //}

        if (ar1 != null) {
            if (ar1.style.display != 'none') {

                oHeight = (td1.clientHeight - 31);
                oHeight2 = (td1.clientHeight - 50);
                oHeight3 = (td1.clientHeight - 60);
                oWidth = (td1.clientWidth - 4);

                ar1.style.height = '500px';
                ar1.style.width = '100px';
            }
        }

        if (ar2 != null) {
            if (cn2.style.display != 'none') {

                oHeight = (td2.clientHeight + 50);
                oHeight2 = td2.clientHeight;
                oHeight3 = (td2.clientHeight - 50);
                oWidth = (td2.clientWidth - 10);

                ar2.style.height = '500px';
                ar2.style.width = '100px';
            }
        }

        if (ar3 != null) {
            if (cn3.style.display != 'none') {

                oHeight = (td3.clientHeight + 60);
                oHeight2 = (td3.clientHeight + 50);
                oHeight3 = (td3.clientHeight);
                oWidth = (td3.clientWidth - 10);

                ar3.style.height = '500px';
                ar3.style.width = '100px';
            }
        }

        if (ar1 != null) {
            try {
                if (parseFloat(oHeight) > 0) { ar1.style.height = oHeight + "px"; }
            } catch (ex) { }
            try {
                if (parseFloat(oWidth) > 0) { ar1.style.width = oWidth + "px"; }
            } catch (ex) { }
        }

        if (ar2 != null) {
            try {
                if (parseFloat(oHeight2) > 0) { ar2.style.height = oHeight2 + "px"; }
            } catch (ex) { }
            try {
                if (parseFloat(oWidth) > 0) { ar2.style.width = oWidth + "px"; }
            } catch (ex) { }
        }

        if (ar3 != null) {
            try {
                if (parseFloat(oHeight3) > 0) { ar3.style.height = oHeight3 + "px"; }
            } catch (ex) { }
            try {
                if (parseFloat(oWidth) > 0) { ar3.style.width = oWidth + "px"; }
            } catch (ex) { }
        }
    }

    function KeyPressTabTree01(event) {
        if (event.keyCode == 40) {
            chTree('2','<%= Me.ClientID %>');
            var tolo = document.getElementById('<%= Me.ClientID %>' + '_tabTree02');
            tolo.focus();
        }
        event.preventDefault();
    }

    function KeyPressTabTree02(event) {
        if (event.keyCode == 38) {
            chTree('1','<%= Me.ClientID %>');
            var tolo = document.getElementById('<%= Me.ClientID %>' + '_tabTree01');
            tolo.focus();
        }
        else if (event.keyCode == 40) {
            chTree('3','<%= Me.ClientID %>');
            var tolo = document.getElementById('<%= Me.ClientID %>' + '_tabTree03');
            tolo.focus();
        }
        event.preventDefault();
    }

    function KeyPressTabTree03(event) {
        if (event.keyCode == 38) {
            chTree('2','<%= Me.ClientID %>');
            var tolo = document.getElementById('<%= Me.ClientID %>' + '_tabTree02');
            tolo.focus();
        }
        event.preventDefault();
    }
</script>