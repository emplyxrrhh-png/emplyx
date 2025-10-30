function InitConvertControls(srcLanguageExt, CurrentExtDatePicketFormat, CurrentExtDatePicketStartDay) {
    try {        
        if (Ext != null) {
            SetExtLanguage(srcLanguageExt);
            // Asignamos formato fecha a controles DatePicker 
            DatePickerInit(CurrentExtDatePicketFormat, CurrentExtDatePicketStartDay);
            //Inicializamos los tips de la librería Ext
            Ext.QuickTips.init();        
            // turn on validation errors beside the field globally
            Ext.form.Field.prototype.msgTarget = 'qtip';        
            // Convertimos controles (DatePicker, ...) del div de contenido 
            //ConvertControls(ConvertControlsDiv);
        }
    } catch (ex) { /*alert('SetConvertControls ' + ex);*/ }
}

// Establecer el archivo de idioma indicado a la librería Ext
function SetExtLanguage(srcLanguageExt) {
    try {
        var head = Ext.fly(document.getElementsByTagName('head')[0]); 
        Ext.DomHelper.append(head, {
                             tag: 'script', 
                             type: 'text/javascript',
                             src: srcLanguageExt
        });
    } catch (ex) { }
}

function ConvertControls(ContainerID) {

    var Container = document.getElementById(ContainerID);
    if (ContainerID != '' && ContainerID != null) {
        Container = document.getElementById(ContainerID);
    }
    else {
        Container = document;
    }
    
    arrInputs = new Array();
    arrInputs = Container.getElementsByTagName('input');
    var nTotalFields = arrInputs.length;
    
    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        switch (arrInputs[n].getAttribute('ConvertControl')) {
        case 'DatePicker': {                            
                DatePickerApply(arrInputs[n]);                
                break; }
        case 'TextField': {
                TextFieldApply(arrInputs[n]);
                break; }   
        case 'TextArea': {
                TextAreaApply(arrInputs[n]);
                break; }
        case 'NumberField': {                
                NumberFieldApply(arrInputs[n]);
                break; }   
        }

    }

    arrInputs = new Array();
    arrInputs = Container.getElementsByTagName('textarea');
    var nTotalFields = arrInputs.length;

    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        switch (arrInputs[n].getAttribute('ConvertControl')) {
            case 'TextArea': 
                {
                    TextAreaApply(arrInputs[n]);
                    break;
                }
        }

    }
    

}

function CheckConvertControls(ContainerID) {
   
    var Container = document.getElementById(ContainerID);
    if (ContainerID != '' && ContainerID != null) {
        Container = document.getElementById(ContainerID);
    }
    else {
        Container = document;
    }
    
    arrInputs = new Array();
    arrInputs = Container.getElementsByTagName('input');
    var nTotalFields = arrInputs.length;
    
    var postvars = '';
    for(n=0;n<nTotalFields;n++){
        
        if (arrInputs[n].getAttribute('ConvertControl') != null &&
            arrInputs[n].getAttribute('ConvertControl') != '') {

            /*
            if (arrInputs[n].getAttribute('class').indexOf('x-form-invalid') > -1 ) {
                return false;
            }
            */
            if (arrInputs[n].getAttribute("Converted") == "true") { 
                
                var oField = Ext.getCmp(arrInputs[n].getAttribute("ConvertedId"));
                if (oField != null) {                    
                    oField.validate();
                    if (oField.isValid(false) == false) return false;                    
                }
                
            }
            
        }
                
    }
    
    return true;
    
}

var DatePickerFormat = 'd/m/Y';
function DatePickerInit(format, startDay) {    
    // Establece el formato de fecha
    DatePickerFormat = format;
    // Establece el día de inicio de la semana
    Ext.DatePicker.prototype.startDay = startDay;
}

function DatePickerApply(Control) {
    try {
        var obj;
        var _allowBlank, _value, _disabled, _visible, _onChange;
        
        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _allowBlank = obj.getAttribute('CCallowBlank');
        _value = obj.getAttribute('value');

        _disabled = "false";
        if (obj.attributes.getNamedItem("disabled") != null) {
            _disabled = obj.getAttribute("disabled").toString();
        }
        
        if (_disabled == "disabled") { _disabled = "true"; } 
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");
        
        if (obj.getAttribute("Converted") != "true") { 
           
            var Params = 'format: DatePickerFormat,';
            if(_allowBlank != null){ Params += "allowBlank:" + _allowBlank + ","; }
            if(_value != null){ Params += "value:'" + _value + "',"; }            
            if(_disabled != null && _disabled != ""){ Params += "disabled:" + _disabled + ","; }            
            if (Params.length > 0) Params = Params.substr(0,Params.length -1);
                    
            var oParams;
            eval('oParams = {' + Params + '};');

            oField = new Ext.form.DateField(oParams);
            oField.applyTo(obj.id);
                        
            if (_onChange != null) {
                oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField);
            }

            if (_visible == "false") oField.hide();
            
            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());            
            
        }
        else {
        
            oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
            if (oField != null) {
                        
                switch (_disabled) {
                    case 'true': {  if (oField.disabled == false) { oField.disable();}                
                                    break; }
                    case 'false': { if (oField.disabled == true) { oField.enable();}
                                    break; }
                    case 'disabled': { if (oField.disabled == true) { oField.disable(); }
                            break; }
                }

                switch (_visible) {
                    case 'true': {  if (oField.hidden == true) { oField.show();}                
                                    break; }
                    case 'false': { if (oField.hidden == false) { oField.hide();}                
                                    break; }
                }
                
                oField.validate();
                                                
            }
                        
        }
            
    } catch (ex) {
        alert('DatePickerApply' + ex);
    }
    
}

function TextFieldApply(Control){
    try {

        var obj;
        var _allowBlank, _blankText, _disabled, _inputType, _maxLength, _minLength, _maxLengthText, _minLengthText, _value, _visible, _time, _mask, _regex, _onChange;

        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _disabled = obj.getAttribute('disabled');
        if (_disabled == "disabled") {  _disabled = "true"; }
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");        
        
        if (obj.getAttribute("Converted") != "true") { 

            _allowBlank = obj.getAttribute('CCallowBlank');
            _blankText = obj.getAttribute('CCblankText');            
            _inputType = obj.getAttribute('CCinputType');
            _maxLength = obj.getAttribute('CCmaxLength');
            _minLength = obj.getAttribute('CCminLength');
            _maxLengthText = obj.getAttribute('CCmaxLengthText');
            _minLengthText = obj.getAttribute('CCminLengthText');
            _value = obj.getAttribute('value');            
            _time = obj.getAttribute('CCtime');
            _mask = obj.getAttribute('CCmask');
            _regex = obj.getAttribute('CCregex');
            
            var Params = '';
            if(_allowBlank != null){ Params += "allowBlank:" + _allowBlank + ","; }
            if(_blankText != null){ Params += "blankText:'" + _blankText + "',"; }
            if (_disabled != null && _disabled != "") { Params += "disabled:" + _disabled + ","; }            
            if(_inputType != null){ Params += "inputType:'" + _inputType + "',"; }    
            if(_maxLength != null){ Params += "maxLength:" + _maxLength + ","; obj.setAttribute('maxlength', _maxLength); }    
            if(_minLength != null){ Params += "minLength:" + _minLength + ","; }    
            if(_maxLengthText != null){ Params += "maxLengthText:'" + _maxLengthText + "',"; }    
            if(_minLengthText != null){ Params += "minLengthText:'" + _minLengthText + "',"; }    
            if(_value != null){ Params += "value:'',"; }            
            if(_time != null && _time == "true"){ 
                _mask = /[\d:]/i;
                _regex = /^([0-1]?[0-9]|2[0-3]):([0-5][0-9])$/;                
                obj.setAttribute('maxlength', '5');
            }            
            if(_mask != null){ Params += "maskRe:" + _mask + ","; }            
            if(_regex != null){ Params += "regex:" + _regex + ","; }            
            
            if (Params.length > 0) Params = Params.substr(0,Params.length -1);
            
            var oParams;
            eval("oParams = {" + Params + "};");
            //Tema parseig dels caracters " ' "
            if (_value != null) { oParams.value = _value; }

            oField = new Ext.form.TextField(oParams);
            oField.applyTo(obj.id);

            if (_onChange != null) {
                oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField);
            }

            if (_visible == "false") oField.hide();
            
            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());
        
        }
        else {
        
            oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
            if (oField != null) {

                switch (_disabled) {
                    case 'true': 
                        {
                            if (oField.disabled == false) { oField.disable(); }
                            break;
                        }
                    case 'false': 
                        {
                            if (oField.disabled == true) { oField.enable(); }
                            break;
                        }
                    case 'disabled': 
                        {
                            if (oField.disabled == true) { oField.disable(); }
                            break;
                        }
                }
                
                switch (_visible) {
                    case 'true': {  if (oField.hidden == true) { oField.show();}                
                                    break; }
                    case 'false': { if (oField.hidden == false) { oField.hide();}                
                                    break; }
                }                    
                
                oField.validate();
                
            }
                                    
        }
        
                          
    } catch (ex) {
        alert('TextFieldApply ' + ex);
    }   
    
}

function TextAreaApply(Control){
    try {
        var obj;
        var _allowBlank, _blankText, _disabled, _inputType, _maxLength, _minLength, _maxLengthText, _minLengthText, _value, _visible, _onChange;

        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _disabled = obj.getAttribute("disabled");
        if (_disabled == "disabled") { _disabled = "true"; }
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");        
        
        if (obj.getAttribute("Converted") != "true") { 

            _allowBlank = obj.getAttribute("CCallowBlank");
            _blankText = obj.getAttribute("CCblankText");            
            _inputType = obj.getAttribute("CCinputType");
            _maxLength = obj.getAttribute("CCmaxLength");
            _minLength = obj.getAttribute("CCminLength");
            _maxLengthText = obj.getAttribute("CCmaxLengthText");
            _minLengthText = obj.getAttribute("CCminLengthText");
            _value = obj.getAttribute("value");
            
            var Params = '';
            if(_allowBlank != null){ Params += "allowBlank:" + _allowBlank + ","; }
            if(_blankText != null){ Params += "blankText:'" + _blankText + "',"; }
            if (_disabled != null && _disabled != "") { Params += "disabled:" + _disabled + ","; }            
            if(_inputType != null){ Params += "inputType:'" + _inputType + "',"; }    
            if(_maxLength != null){ Params += 'maxLength:' + _maxLength + ','; obj.setAttribute('maxlength', _maxLength); }    
            if(_minLength != null){ Params += 'minLength:' + _minLength + ','; }    
            if(_maxLengthText != null){ Params += "maxLengthText:'" + _maxLengthText + "',"; }    
            if(_minLengthText != null){ Params += "minLengthText:'" + _minLengthText + "',"; }    
            if(_value != null){ Params += "value:'',"; }            
            if (Params.length > 0) Params = Params.substr(0,Params.length -1);
            
            var oParams;
            eval('oParams = {' + Params + '};');
            if (_value != null) { oParams.value = _value; }
            
            oField = new Ext.form.TextArea(oParams);
            oField.applyTo(obj.id);

            if (_onChange != null) {
                oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField);
            }

            if (_visible == "false") oField.hide();
            
            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());            
        
        }
        else {
        
            oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
            if (oField != null) {

                switch (_disabled) {
                    case 'true': 
                        {
                            if (oField.disabled == false) { oField.disable(); }
                            break;
                        }
                    case 'false': 
                        {
                            if (oField.disabled == true) { oField.enable(); }
                            break;
                        }
                    case 'disabled': 
                        {
                            if (oField.disabled == true) { oField.disable(); }
                            break;
                        }
                }
                switch (_visible) {
                    case 'true': {  if (oField.hidden == true) { oField.show();}                
                                    break; }
                    case 'false': { if (oField.hidden == false) { oField.hide();}                
                                    break; }
                }
                
                oField.validate();
                
            }
                        
        }
                  
    } catch (ex) {
        alert('TextAreaApply ' + ex);
    }   
    
}

function NumberFieldApply(Control){
    try {

        var obj;
        var _allowBlank, _blankText, _disabled, _maxValue, _minValue, _maxValueText, _minValueText, _allowDecimals, _allowNegative, _decimalPrecision, _decimalSeparator, _value, _visible, _onChange, _maxLength, _minLength, _maxLengthText, _minLengthText;

        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _disabled = obj.getAttribute('disabled');
        if (_disabled == "disabled") { _disabled = "true"; }
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");        
        
        if (obj.getAttribute("Converted") != "true") { 

            _allowBlank = obj.getAttribute('CCallowBlank');
            _blankText = obj.getAttribute('CCblankText');            
            _maxValue = obj.getAttribute('CCmaxValue');
            _minValue = obj.getAttribute('CCminValue');
            _maxValueText = obj.getAttribute('CCmaxValueText');
            _minValueText = obj.getAttribute('CCminValueText');
            _allowDecimals = obj.getAttribute('CCallowDecimals');
            _allowNegative = obj.getAttribute('CCallowNegative');
            _decimalPrecision = obj.getAttribute('CCdecimalPrecision');
            _decimalSeparator = obj.getAttribute('CCdecimalSeparator');
            _maxLength = obj.getAttribute("CCmaxLength");
            _minLength = obj.getAttribute("CCminLength");
            _maxLengthText = obj.getAttribute("CCmaxLengthText");
            _minLengthText = obj.getAttribute("CCminLengthText");            
            _value = obj.getAttribute('value');

            var Params = '';
            if(_allowBlank != null){ Params += "allowBlank:" + _allowBlank + ","; }
            if(_blankText != null){ Params += "blankText:'" + _blankText + "',"; }    
            if(_disabled != null && _disabled != ""){ Params += "disabled:" + _disabled + ","; }            
            if(_maxValue != null){ Params += 'maxValue:' + _maxValue + ','; }    
            if(_minValue != null){ Params += 'minValue:' + _minValue + ','; }            
            if(_maxValueText != null){ Params += "maxValueText:'" + _maxValueText + "',"; }    
            if(_minValueText != null){ Params += "minValueText:'" + _minValueText + "',"; }    
            if(_allowDecimals != null){ Params += "allowDecimals:" + _allowDecimals + ","; }    
            if(_allowNegative != null){ Params += "allowNegative:" + _allowNegative + ","; }    
            if(_decimalPrecision != null){ Params += "decimalPrecision:" + _decimalPrecision + ","; }    
            if(_decimalSeparator != null){ Params += "decimalSeparator:'" + _decimalSeparator + "',"; }    
            if(_maxLength != null){ Params += 'maxLength:' + _maxLength + ','; obj.setAttribute('maxlength', _maxLength); }    
            if(_minLength != null){ Params += 'minLength:' + _minLength + ','; }    
            if(_maxLengthText != null){ Params += "maxLengthText:'" + _maxLengthText + "',"; }    
            if(_minLengthText != null){ Params += "minLengthText:'" + _minLengthText + "',"; }                
            if(_value != null){ Params += "value:'" + _value + "',"; }            
            if (Params.length > 0) Params = Params.substr(0,Params.length -1);
            
            var oParams;
            eval('oParams = {' + Params + '};');
            
            oField = new Ext.form.NumberField(oParams);                     
            oField.applyTo(obj.id);

            if (_onChange != null) {
                oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField);
            }

            if (_visible == "false") oField.hide();
            
            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());            
        
        }
        else {
        
            oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
            if (oField != null) {

                switch (_disabled) {
                    case 'true': 
                        {
                            if (oField.disabled == false) { oField.disable(); }
                            break;
                        }
                    case 'false': 
                        {
                            if (oField.disabled == true) { oField.enable(); }
                            break;
                        }
                    case 'disabled': 
                        {
                            if (oField.disabled == true) { oField.disable(); }
                            break;
                        }
                }
                
                switch (_visible) {
                    case 'true': {  if (oField.hidden == true) { oField.show();}                
                                    break; }
                    case 'false': { if (oField.hidden == false) { oField.hide();}                
                                    break; }
                }
                
                oField.validate();
                
            }
                        
        }
                  
    } catch (ex) {
        alert('NumberFieldApply ' + ex);
    }   
    
}


function onEnterPress(e, funct) {
    try {
        var keynum;
        var keychar;
        var numcheck;

        if (window.event) // IE
        {
            keynum = e.keyCode;
        }
        else if (e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which;
        }

        /*keychar = String.fromCharCode(keynum);
        numcheck = /\d/;*/

        if (e.keyCode == 13) {
            if (funct != "") {
                eval(funct);
            }
            return false;
        }

        //return !numcheck.test(keychar);
        return true;

    } catch (e) { alert(e); }
}
      