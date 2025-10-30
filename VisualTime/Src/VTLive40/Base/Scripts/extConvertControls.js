var createVTypeFunct = false;

function InitConvertControls(srcLanguageExt, CurrentExtDatePicketFormat, CurrentExtDatePicketStartDay) {
    try {
        if (Ext != null) {

            //Deshabilitado porque lo hacemos desde pagebase _ini con un RegisterClientScriptInclude
            //anulado---> SetExtLanguage(srcLanguageExt);

            // Asignamos formato fecha a controles DatePicker
            DatePickerInit(CurrentExtDatePicketFormat, CurrentExtDatePicketStartDay);

            //Inicializamos los tips de la librería Ext
            Ext.QuickTips.init();

            // turn on validation errors beside the field globally
            Ext.form.Field.prototype.msgTarget = 'qtip';

        }
    } catch (ex) { showError("extConvertControls:InitConvertControls", ex); }
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
    } catch (ex) { showError("extConvertControls:SetExtLanguage", ex); }
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
        case 'TimeField': {
                TimeFieldApply(arrInputs[n]);
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

    arrInputs = new Array();
    arrInputs = Container.getElementsByTagName('div');

    var nTotalFields = arrInputs.length;

    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        switch (arrInputs[n].getAttribute('ConvertControl')) {
            case 'Slider':
                {
                    SliderApply(arrInputs[n]);
                    break;
                }
        }

    }

}

function CheckConvertControls(ContainerID, OnlyCheckVisibleControls) {
   
    var Container = document.getElementById(ContainerID);
    if (ContainerID != '' && ContainerID != null) {
        Container = document.getElementById(ContainerID);
    }
    else {
        Container = document;
    }

    if (Container == null) return true;
    
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
                    var bolValidate = true;
                    
                    if (OnlyCheckVisibleControls == null) OnlyCheckVisibleControls = false;
                    if (OnlyCheckVisibleControls == false || (OnlyCheckVisibleControls == true && oField.hidden == false)) {
                        bolValidate = true;
                    }
                    else {
                        bolValidate = false;
                    }                                        
                    
                    if (bolValidate == true) {
                        oField.validate();
                        if (oField.isValid(false) == false) return false;                    
                    }
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
        // Add the additional 'advanced' VTypes
        if (!createVTypeFunct) {
            Ext.apply(Ext.form.VTypes, {
            daterange: function(val, field) {
                
                    var date = field.parseDate(val);
                    var start;
                    var end;

                    if (!date) {
                        dependency.setMaxValue();
                        dependency.setMinValue();
                        return;
                    }

                    //end field
                    if (field.startDateField) {
                        start = Ext.getCmp(field.startDateField);
                        if (start != null) {
                            if (start.value == "" || start.value == null) {
                                field.dateRangeMin = null;
                                field.setMinValue(null);
                                field.dateRangeMax = null;
                                field.setMaxValue(null);
                            }
                        }
                    }

                    //start field
                    if (field.endDateField) {
                        end = Ext.getCmp(field.endDateField);
                        if (end != null) {
                            if (end.value == "" || end.value == null) {
                                field.dateRangeMin = null;
                                field.setMinValue(null);
                                field.dateRangeMax = null;
                                field.setMaxValue(null);
                            }
                        }
                    }                    

                    if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
                        start = Ext.getCmp(field.startDateField);
                        if (start != null) {
                            start.setMaxValue(date);
                            this.dateRangeMax = date;
                            start.validate();
                        }
                    }
                    else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
                        var end = Ext.getCmp(field.endDateField);
                        if (end != null) {
                            end.setMinValue(date);
                            this.dateRangeMin = date;
                            end.validate();
                        }
                    }
                    /*
                    * Always return true since we're only using this vtype to set the
                    * min/max allowed values (these are tested for after the vtype test)
                    */
                    return true;
                }
            });
            createVTypeFunct = true;
        }
        
        var obj;
        var _allowBlank, _value, _disabled, _visible, _onChange;
        
        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _allowBlank = obj.getAttribute('CCallowBlank');
        
        _value = obj.value;

        _disabled = "false";
        if (obj.attributes.getNamedItem("disabled") != null) {
            _disabled = obj.getAttribute("disabled").toString();
        }

        if (obj.disabled) { _disabled = "true"; }
        
        if (_disabled == "disabled") { _disabled = "true"; } 
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");
        
        if (obj.getAttribute("Converted") != "true") {
            var Params = 'format: DatePickerFormat,';
            Params += "id:'" + obj.id + "',";
            Params += "name:'',";
            Params += "value:'" + _value + "',";
            Params += "disabled:" + _disabled + ",";
            if(_allowBlank != null){ Params += "allowBlank:" + _allowBlank + ","; }
            if(_value != null){ Params += "value:'" + _value + "',"; }
            if (obj.getAttribute("CCvtype") != null) { Params += "vtype:'" + obj.getAttribute("CCvtype") + "',"; }
            if (obj.getAttribute("CCendDateField") != null) { Params += "endDateField:'" + obj.getAttribute("CCendDateField") + "',"; }
            if (obj.getAttribute("CCstartDateField") != null) { Params += "startDateField:'" + obj.getAttribute("CCstartDateField") + "',"; }
            //if (_disabled != null && _disabled != "") { Params += "disabled:" + _disabled + ","; }

            Params += "width:92,";  //<--ppr para evitar campos con width=0
            
            if (Params.length > 0) Params = Params.substr(0, Params.length - 1);

            var oParams;
            eval('oParams = {' + Params + '};');

            oField = new Ext.form.DateField(oParams);
            oField.applyToMarkup(obj.id);
            
            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
            }
            
            oField.setValue(_value);
            if (_visible == "false") oField.hide();
            
            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());
            
            oField.validate();
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

                oField.setValue(_value);
                oField.validate();
                                                
            }
                        
        }
            
    } catch (ex) {
        showError('extConvertControls:DatePickerApply', ex);
    }
    
}

function TimeFieldApply(Control) {
    /*new Ext.form.TimeField(
    { id: "TimeField2",
    renderTo: "TimeField2_Container",
    hiddenName: "TimeField2_Value",
    valueField: "text",
    minValue: "9:00",
    maxValue: "18:00",
    increment: 30, 
    format: "G:i"
    }
    );*/
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
            var Params = 'format: "G:i", minValue: "0:00", maxValue: "23:59", increment: 30, valueField: "text",';
            if (_allowBlank != null) { Params += "allowBlank:" + _allowBlank + ","; }
            if (_value != null) { Params += "value:'" + _value + "',"; }
            if (_disabled != null && _disabled != "") { Params += "disabled:" + _disabled + ","; }

            Params += "width:60,";  //<--ppr para evitar campos con width=0

            if (Params.length > 0) Params = Params.substr(0, Params.length - 1);
            
            var oParams;
            eval('oParams = {' + Params + '};');

            oField = new Ext.form.TimeField(oParams);
            oField.applyToMarkup(obj.id);

            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
            }
            
            if (_visible == "false") oField.hide();

            obj.setAttribute("Converted", "true");
            obj.setAttribute("ConvertedId", oField.getId());

        } else {

            oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
            if (oField != null) {

                switch (_disabled) {
                    case 'true':
                            if (oField.disabled == false) { oField.disable(); }
                            break;
                    case 'false':
                            if (oField.disabled == true) { oField.enable(); }
                            break;
                    case 'disabled':
                            if (oField.disabled == true) { oField.disable(); }
                            break;
                }

                switch (_visible) {
                    case 'true':
                            if (oField.hidden == true) { oField.show(); }
                            break;
                    case 'false':
                            if (oField.hidden == false) { oField.hide(); }
                            break;
                }
                oField.validate();
            }

        }

    } catch (ex) {
        showError('extConvertControls:TimeFieldApply', ex);
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
                //_regex = /^([0-9]?[0-9]?[0-9]):([0-5][0-9])$/;                
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
            oField.applyToMarkup(obj.id);

            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
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
        showError('extConvertControls:TextFieldApply', ex);
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
            if (_value != null) { Params += "value:'',"; }

//            if (obj.getAttribute("onchange") != null) {
//                var strOnChange = obj.getAttribute("onchange");
//                obj.removeAttribute("onchange");
//                Params += "applyTo: '" + obj.id + "',";
//                Params += "id: '" + obj.id + "',";
//                Params += "name: 'nm_" + obj.id + "',";
//                Params += "listeners: { change: function() { " + strOnChange + "; }},";
//            }

            if (Params.length > 0) Params = Params.substr(0, Params.length - 1);
            var oParams;
            eval('oParams = {' + Params + '};');
            if (_value != null) { oParams.value = _value; }
            
            oField = new Ext.form.TextArea(oParams);
            oField.applyToMarkup(obj.id);
            
            //oField.validate();
            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
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
        showError('extConvertControls:TextAreaApply', ex);
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
            oField.applyToMarkup(obj.id);

            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange")); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
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
        showError('extConvertControls:NumberFieldApply', ex);
    }   
    
}

function SliderApply(Control) {

    /* new Ext.Slider({
    renderTo: 'divSliderLevelOfAuthority',
    width: 214,
    increment: 1,
    minValue: 1,
    maxValue: 10,
    plugins: new Ext.ux.SliderTip()
    });*/

    try {

        var obj;
        var _width, _increment, _minValue, _maxValue, _disabled, _value, _visible, _onChange;

        if (typeof Control == "string") {
            obj = document.getElementById(Control);
        } else {
            obj = Control;
        }

        var oField;

        _disabled = obj.getAttribute('disabled');
        if (_disabled == "disabled" || _disabled == "true") { _disabled = true; } else { _disabled = false; }
        _visible = obj.getAttribute("CCvisible");
        _onChange = obj.getAttribute("CConchange");

        if (obj.getAttribute("Converted") != "true") {

            _width = obj.getAttribute('CCwidth');
            _increment = obj.getAttribute('CCincrement');
            _minValue = obj.getAttribute('CCminValue');
            _maxValue = obj.getAttribute('CCmaxValue');
            _value = obj.getAttribute('CCvalue');

            var Params = '';
            if (_width == null) { _width = 194; }
            if (_increment == null) { _increment = 1; }
            if (_disabled == null) { _disabled = false; }
            if (_maxValue == null) { _maxValue = 10; }
            if (_minValue == null) { _minValue = 1; }
            if (_value == null) { _value = 1; }
                        
           /* var oParams;
            eval('oParams = {' + Params + '};');*/

            oField = new Ext.Slider({
            renderTo: obj.id,
            width: parseInt(_width),
            increment: parseInt(_increment),
            minValue: parseInt(_minValue),
            maxValue: parseInt(_maxValue),
            disabled: _disabled, 
            value: parseInt(_value),
            plugins: new Ext.ux.SliderTip()
            });

            if (_onChange != null) {
                if (!oField.hasListener('change')) {
                    oField.on('change', function() { eval(obj.getAttribute("CConchange") + '(this, this.value);'); }, oField, { normalized: false });
                }
            } else {
                if (!oField.hasListener('change')) {
                    if (obj.getAttribute("onchange") != null) {
                        var strOnChange = obj.getAttribute("onchange");
                        oField.on('change', function() { eval(strOnChange + '(this, this.value);'); }, oField, { normalized: false });
                        obj.removeAttribute("onchange");
                    }
                }
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
                    case 'true':
                        {
                            if (oField.hidden == true) { oField.show(); }
                            break;
                        }
                    case 'false':
                        {
                            if (oField.hidden == false) { oField.hide(); }
                            break;
                        }
                }

                oField.validate();

            }

        }

    } catch (ex) {
        alert('SliderApply ' + ex);
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

    } catch (e) { showError("extConvertControls:onEnterPress", e); }
}

function StringEncodeControlChars(sInput) {
    //Cambia caracteres de control de un string por tokens.
    //Llamando de nuevo a la funcion StringDecodeControlChars se obtiene de nuevo el
    //string original.

    var sOutput = new String(); //string
    var i = 0; //Integer
    sOutput = sInput;

    for (i = 1; i <= 31; i++) {
        var re = new RegExp('[' + String.fromCharCode(i) + ']', "g");
        sOutput = sOutput.replace(re, '%' + i + '%');
    }
    for (i = 60; i <= 62; i++) {
        var re = new RegExp('[' + String.fromCharCode(i) + ']', "g");
        sOutput = sOutput.replace(re, '%' + i + '%');
    }
    for (i = 123; i <= 255; i++) {
        var re = new RegExp('[' + String.fromCharCode(i) + ']', "g");
        sOutput = sOutput.replace(re, '%' + i + '%');
    }

    var remas = new RegExp('[+]', "g");
    sOutput = sOutput.replace(remas, ' ');

    return sOutput;
}
