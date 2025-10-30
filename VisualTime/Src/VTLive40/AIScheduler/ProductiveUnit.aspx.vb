Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class ProductiveUnit
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="Modes")>
        Public Modes As roProductiveUnitMode()

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private oPermission As Permission
    Private oCostCenterPermission As Boolean

    Private Const EMPTY_WEEK As String = "0000000"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("ProductiveUnit.Definition")
        Me.oCostCenterPermission = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("AIScheduler", "~/AIScheduler/Scripts/AIScheduler.js")
        Me.InsertExtraJavascript("ProductiveUnit", "~/AIScheduler/Scripts/ProductiveUnit.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesProductiveUnit.TreeCaption = Me.Language.Translate("TreeCaptionProductiveUnit", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If

        If AISchedulingServiceMethods.GetProductiveUnits(Me.Page).Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        If Not Me.IsPostBack And Not Me.IsCallback Then
            If oCostCenterPermission Then LoadDefaultCombos(True, oCostCenterPermission)
        End If

        LoadDefaultCombos(False, oCostCenterPermission)
        Me.costCenterRow.Style("display") = ""

    End Sub

    Protected Sub LoadDefaultCombos(ByVal bolReload As Boolean, ByVal oCostcenterPermission As Boolean)

        If oCostcenterPermission Then
            Dim dtCostCeters As DataTable = ViewState("ProductiveUnits_CostCenters")

            If dtCostCeters Is Nothing OrElse bolReload Then
                dtCostCeters = API.TasksServiceMethods.GetBusinessCenters(Me.Page, False)
                ViewState("ProductiveUnits_CostCenters") = dtCostCeters
            End If

            Me.cmbCostCenter.ValueType = GetType(Integer)
            Me.cmbCostCenter.Items.Clear()
            Me.cmbCostCenter.Items.Add(Me.Language.Translate("ProductiveUnit.NoCostCenter", Me.DefaultScope), 0)
            If dtCostCeters IsNot Nothing AndAlso dtCostCeters.Rows.Count > 0 Then
                For Each oRow In dtCostCeters.Rows
                    Me.cmbCostCenter.Items.Add(oRow("Name"), oRow("ID"))
                Next

            End If
        End If

        cmbSummaryPeriod.Items.Clear()
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Anual", DefaultScope), ProductiveUnitSummaryType.Anual)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Mensual", DefaultScope), ProductiveUnitSummaryType.Monthly)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Semanal", DefaultScope), ProductiveUnitSummaryType.Weekly)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Daily", DefaultScope), ProductiveUnitSummaryType.Daily)

        cmbSummaryPeriod.SelectedItem = cmbSummaryPeriod.Items(0)

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETPRODUCTIVEUNIT"
                bRet = LoadProductiveUnit(oParameters.ID, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETPRODUCTIVEUNIT")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            Case "SAVEPRODUCTIVEUNIT"
                bRet = SaveProductiveUnit(oParameters.ID, oParameters.Name, oParameters, responseMessage)

                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVEPRODUCTIVEUNIT")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)

                    ASPxCallbackPanelContenido.JSProperties("cpModes") = roJSONHelper.SerializeNewtonSoft(oParameters.Modes)
                Else
                    bRet = LoadProductiveUnit(oParameters.ID, responseMessage)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETPRODUCTIVEUNIT")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                End If

            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

        ASPxCallbackPanelContenido.JSProperties.Add("cpCanEdit", Me.oPermission > Permission.Read)

    End Sub

    Private Function LoadProductiveUnit(ByRef IdProductiveUnit As Integer, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentProductiveUnit As roProductiveUnit
                If IdProductiveUnit = -1 Then
                    oCurrentProductiveUnit = New roProductiveUnit()
                    oCurrentProductiveUnit.ID = -1
                    oCurrentProductiveUnit.IDCenter = 0
                Else
                    oCurrentProductiveUnit = AISchedulingServiceMethods.GetProductiveUnitById(Me.Page, IdProductiveUnit, True)
                End If

                txtName.Text = oCurrentProductiveUnit.Name
                txtShortName.Text = oCurrentProductiveUnit.ShortName
                txtDescription.Text = oCurrentProductiveUnit.Description
                dxcolor.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentProductiveUnit.Color)

                cmbCostCenter.SelectedItem = cmbCostCenter.Items.FindByValue(oCurrentProductiveUnit.IDCenter)
                If cmbCostCenter.SelectedItem Is Nothing Then cmbCostCenter.SelectedItem = cmbCostCenter.Items.FindByValue(0)

                If oCurrentProductiveUnit.UnitModes Is Nothing Then oCurrentProductiveUnit.UnitModes = {}

                ASPxCallbackPanelContenido.JSProperties.Add("cpModes", roJSONHelper.SerializeNewtonSoft(oCurrentProductiveUnit.UnitModes))

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function SaveProductiveUnit(ByRef IdProductiveUnit As Integer, ByRef ProductiveUnitName As String, ByVal oParameters As ObjectCallbackRequest, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission >= Permission.Write Then

                Dim oCurrentProductiveUnit As roProductiveUnit = AISchedulingServiceMethods.GetProductiveUnitById(Me.Page, IdProductiveUnit, True)
                If Not oCurrentProductiveUnit Is Nothing Then

                    oCurrentProductiveUnit.Color = Drawing.ColorTranslator.ToWin32(dxcolor.Color)
                    oCurrentProductiveUnit.Name = txtName.Text
                    oCurrentProductiveUnit.ShortName = txtShortName.Text
                    oCurrentProductiveUnit.Description = txtDescription.Text
                    oCurrentProductiveUnit.IDCenter = cmbCostCenter.SelectedItem.Value

                    oCurrentProductiveUnit.UnitModes = oParameters.Modes
                    oCurrentProductiveUnit = AISchedulingServiceMethods.SaveProductiveUnit(Me.Page, oCurrentProductiveUnit, True)

                    If oCurrentProductiveUnit.ID <> -1 Then
                        bRet = True
                        Dim treePath As String = "/source/" & oCurrentProductiveUnit.ID
                        HelperWeb.roSelector_SetSelection(oCurrentProductiveUnit.ID.ToString, treePath, "ctl00_contentMainBody_roTreesProductiveUnit")
                    Else
                        ErrorInfo = AISchedulingServiceMethods.LastProductiveErrorText
                    End If
                Else
                    ErrorInfo = AISchedulingServiceMethods.LastProductiveErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

End Class