function LengthValidation(s, e) {
    if (s.GetValue() == null || s.GetValue().length == 0 || (s.GetInputElement().maxLength != -1 && s.GetValue().length > s.GetInputElement().maxLength)) {
        e.isValid = false;
    }
}

function DateRequiered(s, e) {
    if (s.GetDate() == null) {
        e.isValid = false;
    }
}

function SelectedItemRequiered(s, e) {
    if (s.GetSelectedItem() == null) {
        e.isValid = false;
    }
}

function HightlightOnGotFocus(s, e) {
    //s.GetMainElement().classList.add('selectedEditor');
}

function FadeOnLostFocus(s, e) {
    //s.GetMainElement().classList.remove('selectedEditor');
}
