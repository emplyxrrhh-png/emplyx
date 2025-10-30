'------------------------------------------------------------------------------
' <generado automáticamente>
'     Este código fue generado por una herramienta.
'
'     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
'     se vuelve a generar el código. 
' </generado automáticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class roCalendar

    '''<summary>
    '''Control hdnCalendarWorkMode.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents hdnCalendarWorkMode As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Control hdnClientInstanceName.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents hdnClientInstanceName As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Control PerformActionCallback.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents PerformActionCallback As Global.DevExpress.Web.ASPxCallback

    '''<summary>
    '''Control ASPxRoCalendarCallback.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents ASPxRoCalendarCallback As Global.DevExpress.Web.ASPxCallback

    '''<summary>
    '''Control hdnCalendarConfig.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents hdnCalendarConfig As Global.DevExpress.Web.ASPxHiddenField

    '''<summary>
    '''Control hdnHolidayShiftPeriodicity.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents hdnHolidayShiftPeriodicity As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Control roCalendarRender.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents roCalendarRender As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Control dlgAdvCopy.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgAdvCopy As Global.VTLive40.WebUserForms_frmAdvCopy

    '''<summary>
    '''Control dlgStarter.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgStarter As Global.VTLive40.frmStarterShift

    '''<summary>
    '''Control dlgComplementary.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgComplementary As Global.VTLive40.frmComplementary

    '''<summary>
    '''Control dlgCopy.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgCopy As Global.VTLive40.WebUserForms_frmCopyOptions

    '''<summary>
    '''Control dlgError.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgError As Global.VTLive40.WebUserForms_frmErrorDetail

    '''<summary>
    '''Control dlgImportError.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgImportError As Global.VTLive40.WebUserForms_frmImportDetail

    '''<summary>
    '''Control dlgImport.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgImport As Global.VTLive40.WebUserForms_frmImportExcel

    '''<summary>
    '''Control dlgFilterCalendar.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgFilterCalendar As Global.VTLive40.WebUserForms_frmFilterCalendar

    '''<summary>
    '''Control dlgAssignments.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgAssignments As Global.VTLive40.frmAssignments

    '''<summary>
    '''Control dlgSortCalendar.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgSortCalendar As Global.VTLive40.WebUserForms_frmSortCalendar

    '''<summary>
    '''Control dlgShiftSelector.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgShiftSelector As Global.VTLive40.frmShiftSelector

    '''<summary>
    '''Control dlgProductiveUnitSelector.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgProductiveUnitSelector As Global.VTLive40.frmProductiveUnitSelector

    '''<summary>
    '''Control dlgBudgetAddEmployees.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgBudgetAddEmployees As Global.VTLive40.frmBudgetAddEmployees

    '''<summary>
    '''Control dlgEditDailyView.
    '''</summary>
    '''<remarks>
    '''Campo generado automáticamente.
    '''Para modificarlo, mueva la declaración del campo del archivo del diseñador al archivo de código subyacente.
    '''</remarks>
    Protected WithEvents dlgEditDailyView As Global.VTLive40.WebUserForms_frmEditDailyView
End Class
