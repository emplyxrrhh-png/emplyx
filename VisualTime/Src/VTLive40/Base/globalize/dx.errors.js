(function () {

    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.JSErrors");

}());

Robotics.Client.JSErrors.isShowingPopup = false;

Robotics.Client.JSErrors.JSErrorTypes = {
    roJsSuccess: 0,
    roJsError: 1,
    roJsWarning: 2,
    roJsInfo: 3
};

Robotics.Client.JSErrors.showDxErrorPopup = function (s, e) {
    if (typeof s.cpJsErrorScript != 'undefined' && s.cpJsErrorScript != '') {
        try {
            eval(s.cpJsErrorScript);
        } catch (e) { }
        
    }
}

Robotics.Client.JSErrors.createEmptyButton = function () {
    return { text: '', textkey: '', desc: '', desckey: '', script:''};
}

Robotics.Client.JSErrors.showJSerrorPopup = function(typeIcon, errorCode, title, description, button1, button2, button3, button4) {
    var oError = {
        MsgType: typeIcon,
        TitleText: title.text,
        TitleKey: title.key,
        ReturnCode: errorCode,
        DescriptionText: description.text,
        DescriptionKey: description.key,
        Button1Text: button1.text,
        Button1TextKey: button1.textkey,
        Button1Description: button1.desc,
        Button1DescriptionKey: button1.desckey,
        Button1Script: button1.script,
        Button2Text: button2.text,
        Button2TextKey: button2.textkey,
        Button2Description: button2.desc,
        Button2DescriptionKey: button2.desckey,
        Button2Script: button2.script,
        Button3Text: button3.text,
        Button3TextKey: button3.textkey,
        Button3Description: button3.desc,
        Button3DescriptionKey: button3.desckey,
        Button3Script: button3.script,
        Button4Text: button4.text,
        Button4TextKey: button4.textkey,
        Button4Description: button4.desc,
        Button4DescriptionKey: button4.desckey,
        Button4Script: button4.script
    };

    if (!Robotics.Client.JSErrors.isShowingPopup) {
        Robotics.Client.JSErrors.isShowingPopup = true;
        Robotics.Client.JSErrors.showJSerror(JSON.stringify(oError), '');
    }
}

Robotics.Client.JSErrors.showJSerror = function(errorObj, errorMsg, stateObj) {

    loadJSErrorLanguages(function () {

        var oError = JSON.parse(errorObj);
        

        var errorText = "";

        if (oError.DescriptionKey != "" || oError.DescriptionText != "") {
            if (oError.ReturnCode != "") errorText = oError.ReturnCode + ": " + (oError.DescriptionKey != '' ? Globalize.formatMessage(oError.DescriptionKey) : oError.DescriptionText);
            else errorText = (oError.DescriptionKey != '' ? Globalize.formatMessage(oError.DescriptionKey) : oError.DescriptionText)
        } else {
            if (errorMsg == "" && typeof stateObj != 'undefined') {
                var oState = JSON.parse(stateObj);

                errorText = (typeof oState.errorTextField == 'undefined' ? oState.ErrorText : (oState.errorTextField == null ? '' : oState.errorTextField));
                if (oState.ReturnCode != "") errorText = (typeof oState.returnCodeField == 'undefined' ? oState.ReturnCode : (oState.returnCodeField == null ? '' : oState.returnCodeField)) + ": " + errorText;
            } else {
                errorText = errorMsg;
            }
        }


        var title = "";
        if (oError.TitleKey != "") title = Globalize.formatMessage(oError.TitleKey);
        else title = oError.TitleText;

        var iconType = oError.MsgType;

        var customMessageDiv = $('<div>').attr('class', 'roPopupMsg');
        customMessageDiv.append($('<div>').attr('class', 'roPopupIcon ro-icon-errorType-' + iconType));
        customMessageDiv.append($('<div>').attr('class', 'roPopupText').html(errorText));

        var heightDiv = $('<div>').attr('style', 'clear:both');


        var computedButtons = [];

        if (oError.Button1Text != "" || oError.Button1TextKey != "") {
            computedButtons.push(
                {
                    text: (oError.Button1TextKey != "" ? Globalize.formatMessage(oError.Button1TextKey) : oError.Button1Text),
                    hint: (oError.Button1DescriptionKey != "" ? Globalize.formatMessage(oError.Button1DescriptionKey) : oError.Button1Description),
                    onClick: function () { return 'roButton1' }
                }
            );
        }

        if (oError.Button2Text != "" || oError.Button2TextKey != "") {
            computedButtons.push(
                {
                    text: (oError.Button2TextKey != "" ? Globalize.formatMessage(oError.Button2TextKey) : oError.Button2Text),
                    hint: (oError.Button2DescriptionKey != "" ? Globalize.formatMessage(oError.Button2DescriptionKey) : oError.Button2Description),
                    onClick: function () { return 'roButton2' }
                }
            );
        }

        if (oError.Button3Text != "" || oError.Button3TextKey != "") {
            computedButtons.push(
                {
                    text: (oError.Button3TextKey != "" ? Globalize.formatMessage(oError.Button3TextKey) : oError.Button3Text),
                    hint: (oError.Button3DescriptionKey != "" ? Globalize.formatMessage(oError.Button3DescriptionKey) : oError.Button3Description),
                    onClick: function () { return 'roButton3' }
                }
            );
        }

        if (oError.Button4Text != "" || oError.Button4TextKey != "") {
            computedButtons.push(
                {
                    text: (oError.Button4TextKey != "" ? Globalize.formatMessage(oError.Button4TextKey) : oError.Button4Text),
                    hint: (oError.Button4DescriptionKey != "" ? Globalize.formatMessage(oError.Button4DescriptionKey) : oError.Button4Description),
                    onClick: function () { return 'roButton4' }
                }
            );
        }

        var customDialog = DevExpress.ui.dialog.custom({
            title: title,
            messageHtml: $('<div>').append(customMessageDiv, heightDiv).html(),
            css: 'roCustomDialog',
            buttons: computedButtons
        });


        customDialog.show().done(function (dialogResult) {
            switch (dialogResult) {
                case 'roButton1':
                    if (typeof oError.Button1Script != 'undefined' && oError.Button1Script != null && oError.Button1Script != "") eval(oError.Button1Script);
                    break;
                case 'roButton2':
                    if (typeof oError.Button2Script != 'undefined' && oError.Button2Script != null && oError.Button2Script != "") eval(oError.Button2Script);
                    break;
                case 'roButton3':
                    if (typeof oError.Button3Script != 'undefined' && oError.Button3Script != null && oError.Button3Script != "") eval(oError.Button3Script);
                    break;
                case 'roButton4':
                    if (typeof oError.Button4Script != 'undefined' && oError.Button4Script != null && oError.Button4Script != "") eval(oError.Button4Script);
                    break;
            }
            Robotics.Client.JSErrors.isShowingPopup = false;
            customDialog.hide();
        });

        //DevExpress.ui.dialog.alert(errorText, Globalize.formatMessage('roAlert'));
    });

    
}




