var currentDexView = null;
var dexPath = null;
var bSendingMessage = false;

window.addEventListener("pageshow", function (event) {
    var historyTraversal = event.persisted ||
        (typeof window.performance != "undefined" &&
            window.performance.navigation.type === 2);
    if (historyTraversal) {
        // Handle page restore.
        window.location.reload(true);
    }
});

$(() => {
    const itentificationOptions = [
        { id: 0, text: jsLabels["DEX#ComplaintAnonymous"] },
        { id: 1, text: jsLabels["DEX#ComplaintIdentified"] }
    ];

    if ($('#identificationOptions').length > 0) {
        $('#IdentificationPwd').dxTextBox('instance').option('value', '');
        $('#IdentificationRepeatPwd').dxTextBox('instance').option('value', '');

        $('#identificationOptions').dxRadioGroup({
            items: itentificationOptions,
            valueExpr: 'id',
            displayExpr: 'text',
            onValueChanged(e) {
                if (e.value === 0) {
                    $('#identificationData').hide();
                    $('#IdentificationName').dxTextBox('instance').option('value', '');
                    $('#IdentificationMail').dxTextBox('instance').option('value', '');
                    $('#IdentificationPhone').dxTextBox('instance').option('value', '');
                    $('#IdentificationCompany').dxTextBox('instance').option('value', '');
                } else {
                    $('#identificationData').show();
                }
            },
        }).dxRadioGroup('instance').option('value', 0);
    }

    $('#privacyPolicyCTA').off("click");
    $('#privacyPolicyCTA').on("click", function (e) {
        $("#privacyPolicyPopup").dxPopup("instance").show();
    });

    $('#privacyPolicyPopup').dxPopup('instance').option('toolbarItems', [{
        widget: 'dxButton',
        toolbar: 'bottom',
        location: 'after',
        options: {
            icon: 'check',
            text: jsLabels["DEX#AcceptPrivacyPopup"],
            onClick() {
                $('#Privacy').dxCheckBox('instance').option('value', true);
                $('#privacyPolicyPopup').dxPopup('instance').hide();
            },
        },
    }, {
        widget: 'dxButton',
        toolbar: 'bottom',
        location: 'after',
        options: {
            icon: 'close',
            text: jsLabels["DEX#ClosePrivacyPopup"],
            onClick() {
                $('#privacyPolicyPopup').dxPopup('instance').hide();
            },
        },
    }]);
});

function btnRecoverComplaintOnClick() {
    grecaptcha.execute();
}

function onDexValidationSubmit(token) {
    let ref = $('#complaintReference').dxTextBox('instance').option('value');
    let pRef = $('#password').dxTextBox('instance').option('value');

    if (ref == null || ref.length == 0 || pRef == null || pRef.length == 0) {
        DevExpress.ui.notify(jsLabels["DEX#MissingMandatoryData"], "error", 5000);
    } else {
        if (token != null && token.length > 0) {
            document.forms[0].submit();
        }
    }
}

function initDEX() {
    let dexPathArray = window.location.pathname.split('/');
    if (dexPathArray.length > 2) dexPath = dexPathArray[2];

    document.title = "cegid Visualtime";

    $('#sendDEX').show();
    $('#sendDEX1').show();
    $('#sendDEX2').hide();
    $('#sendDEXResult').hide();
}

function goToHome() {
    document.location = '/DEX/' + dexPath;
}

function invalidateCurrentSession() {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'DEX/' + dexPath + '/Logout',
        dataType: "json",
        data: { id: dexPath },
        success: function (e) {
            if (typeof e == 'string') {
                DevExpress.ui.notify(e, "error", 5000);
            } else {
                location.reload();
            }
        },
        error: function (e) {
            DevExpress.ui.notify(jsLabels["DEX#closeSessionError"], "error", 5000);
        }
    });
}

function btnCopyClipboard(s, e) {
    navigator.clipboard.writeText(currentDexView.Conversation.ReferenceNumber)
        .then(() => {
            // Alert the copied text
            DevExpress.ui.notify(jsLabels["DEX#complaintGuidCopied"], "success", 2000);
        })
        .catch((error) => {
            // Handle error
            DevExpress.ui.notify(jsLabels["DEX#complaintGuidNotCopied"], "error", 2000);
        });
}

function btnCloseDEX(s, e) {
    window.location.replace(dexPath);
}

function btnNextDEXClick() {
    if ($('#Privacy').dxCheckBox('instance').option('value')) {
        if ($('#IdentificationPwd').dxTextBox('instance').option('value') == $('#IdentificationRepeatPwd').dxTextBox('instance').option('value')) {
            $.ajax({
                type: "POST",
                url: BASE_URL + 'DEX/' + dexPath + '/ValidateData',
                dataType: "json",
                data: { id: dexPath, key: $('#IdentificationPwd').dxTextBox('instance').option('value'), email: $('#IdentificationMail').dxTextBox('instance').option('value'), phone: $('#IdentificationPhone').dxTextBox('instance').option('value') },
                success: function (e) {
                    if (typeof e == 'string') {
                        DevExpress.ui.notify(e, "error", 2000);

                        if (jsLabels["DEX#ComplaintPwdValidationError"] == e) {
                            $('#IdentificationPwd').dxTextBox('instance').focus();
                        } else if (jsLabels["DEX#ComplaintEmailValidationError"] == e) {
                            $('#IdentificationMail').dxTextBox('instance').focus();
                        } else if (jsLabels["DEX#ComplaintPhoneValidationError"] == e) {
                            $('#IdentificationPhone').dxTextBox('instance').focus();
                        }
                    } else {
                        //guardamos datos del primer bloque
                        currentDexView = {
                            Body: '',
                            IsResponse: false,
                            IsAnonymous: true,
                            CreatedBy: 0,
                            CreatedOn: new Date(),
                            Conversation: {
                                Title: '',
                                IsAnonymous: true,
                                Password: $('#IdentificationPwd').dxTextBox('instance').option('value'),
                                CreatedOn: new Date(),
                                ExtraData: {
                                    FullName: $('#IdentificationName').dxTextBox('instance').option('value'),
                                    Mail: $('#IdentificationMail').dxTextBox('instance').option('value'),
                                    Phone: $('#IdentificationPhone').dxTextBox('instance').option('value'),
                                    Customer: $('#IdentificationCompany').dxTextBox('instance').option('value'),
                                    Other: ''
                                }
                            }
                        };
                        //ocultamos primer bloque
                        $('#sendDEX1').hide();
                        //mostramos segundo bloque
                        $('#sendDEX2').show();

                        $('#DEXMessage').dxTextArea('instance').option('value', '');
                        $('#DEXTitle').dxTextBox('instance').option('value', '');
                        $('#DEXTitle').dxTextBox('instance').focus();
                    }
                },
                error: function (e) {
                    DevExpress.ui.notify(jsLabels["DEX#ComplaintPwdValidationError"], "error", 2000);
                }
            });
        } else {
            DevExpress.ui.notify(jsLabels["DEX#ComplaintPwdRepeatError"], "error", 2000);
        }
    } else {
        DevExpress.ui.notify(jsLabels["DEX#ComplaintPrivacyRequiered"], "error", 2000);
    }
}

function btnSaveDEX() {
    currentDexView.Body = $('#DEXMessage').dxTextArea('instance').option('value');
    currentDexView.Conversation.Title = $('#DEXTitle').dxTextBox('instance').option('value');

    if (currentDexView.Body.length < 6000) {
        if (currentDexView.Body.length == 0 || currentDexView.Conversation.Title.length == 0) {
            DevExpress.ui.notify(jsLabels["DEX#ComplaintValidationError"], "error", 2000);
        } else {
            $.ajax({
                type: "POST",
                url: BASE_URL + 'DEX/' + dexPath + '/Send',
                dataType: "json",
                data: { id: dexPath, dexMessage: currentDexView },
                success: function (e) {
                    if (typeof e == 'string') {
                        DevExpress.ui.notify(e, "error", 5000);
                    } else {
                        currentDexView = e;
                        $('#sendDEX').hide();
                        $('#sendDEX1').show();
                        $('#sendDEX2').hide();
                        $('#sendDEXResult').show();
                        $('#DEXIdentifier').dxTextBox('instance').option('value', currentDexView.Conversation.ReferenceNumber);
                    }
                },
                error: function (e) { DevExpress.ui.notify(jsLabels["DEX#ComplaintMsgSaveError"], "error", 2000); }
            });
        }
    }
    else {
        DevExpress.ui.notify(jsLabels["DEX#MessageTooLarge"], "error", 5000);
    }
}

function createNewMessage() {
    let sMessage = $("#newMsgText").dxTextArea("instance").option("value");
    let iComplaint = selectedComplaint;
    if (sMessage.length < 6000) {
        if (!bSendingMessage) {
            if (sMessage != null && sMessage.trim().length > 0) {
                bSendingMessage = true;
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'DEX/' + dexPath + '/AddMessage',
                    dataType: "json",
                    data: { IdComplaint: iComplaint, Message: sMessage },
                    success: function (e) {
                        bSendingMessage = false;
                        if (typeof e == 'string') {
                            DevExpress.ui.notify(e, "error", 5000);
                        } else {
                            $('#newMsgText').dxTextArea('instance').option('value', '');
                            $("#divMessagesList").load('Reload', { id: dexPath, complaintReference: selectedReference }, function () {
                                $('#divMessagesList').scrollTop($('#divMessagesList')[0].scrollHeight);
                            });
                        }
                    },
                    error: function (e) {
                        bSendingMessage = false;
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelUnkown'), "error", 5000);
                    }
                });
            }
            else {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelEmptyMessage'), "error", 3000);
            }
        }
    }
    else {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roMessageTooLarge'), "error", 5000);
    }
}

function changeMsgState(s, e) {
}

function calcMessageWidth() {
    return document.documentElement.clientWidth - "1%";
}

function printComplaintMessages() {
    var $print = $("#divMessagesList")
        .clone()
        .addClass('print')
        .prependTo('body');
    window.print();
    $print.remove();
}