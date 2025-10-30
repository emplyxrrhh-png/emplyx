Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_NewTerminalWizard
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Terminals.Definition", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        Me.TerminalSN.Text = Request("TerminalSN")
        Me.TerminalModel.Text = Request("Type")
        Me.TerminalIP.Text = Request("IP")

        If Not Me.IsPostBack Then

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text
            Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
            Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
            Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text

            Me.intActivePage = 0
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
            If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
            If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
            If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4

        End If

        Me.optTerminalGroup.addOPanel(Me.optNewTerminal)
        Me.optTerminalGroup.addOPanel(Me.optReplaceTerminal)

        'Carrega dels Terminals per reemplaç...
        LoadTerminalsInTable()

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        Dim intOldPage As Integer
        If Me.CheckPage(Me.intActivePage) Then
            If Me.intActivePage = 2 And Me.optNewTerminal.Checked = True Then
                intOldPage = Me.intActivePage
                Me.intActivePage = 4
            Else
                intOldPage = Me.intActivePage
                Me.intActivePage += 1
            End If

            Me.PageChange(intOldPage, Me.intActivePage)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage

        If Me.intActivePage = 4 And Me.optNewTerminal.Checked = True Then
            Me.intActivePage = 2
        Else
            Me.intActivePage -= 1
        End If

        Me.PageChange(intOldPage, Me.intActivePage)

    End Sub

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim bolSaved As Boolean = False
            Dim strErrorInfo As String = ""

            'Generem el Terminal
            If Me.optNewTerminal.Checked = True Then 'Nou Terminal
                Dim intNewID As Integer = API.TerminalServiceMethods.RetrieveTerminalNextID(Me.Page)
                If intNewID = -1 Then 'Si no es recupera correctament el ID...
                    strErrorInfo = Me.Language.Translate("End.ErrorInNextID", Me.DefaultScope)
                Else ' Grabem el nou Terminal i el Reader
                    'Dim oTerminal As New TerminalService.roTerminal()
                    'oTerminal.ID = -1
                    'oTerminal.Description = Me.txtNameTerminal.Text
                    'oTerminal.Type = Me.TerminalModel.Text
                    'oTerminal.Behavior = ""
                    'oTerminal.Location = Me.TerminalIP.Text
                    'oTerminal.SupportedModes = "E,S,ES,XI"
                    'oTerminal.SupportedOutputs = "1"
                    'oTerminal.SupportedSirens = ""
                    'oTerminal.SirensOutput = "0"
                    'oTerminal.Other = Me.TerminalSN.Text & ";" & Me.txtRegister.Text
                    'oTerminal.RigidMode = "0"
                    'oTerminal.AllowAntiPassBack = False
                    'oTerminal.IsDifferentZoneTime = False
                    'oTerminal.ZoneTime = 0
                    'oTerminal.AllowCustomButton = False
                    'oTerminal.CustomOutput = 0
                    'oTerminal.CustomDuration = 0
                    'oTerminal.CustomField = 0
                    'oTerminal.CustomFieldValue = "0"
                    'oTerminal.AllowMoveReason = True

                    ''Creem el Reader
                    'Dim oTerminalReader As New TerminalService.roTerminalReader()
                    'oTerminalReader.IDTerminal = oTerminal.ID
                    'oTerminalReader.ID = 1
                    'oTerminalReader.Description = "Reader 1"
                    ''TODO: De moment, sense opcio de cambiar la funcionalitat del terminal
                    'oTerminalReader.Mode = "XI"
                    'oTerminalReader.Duration = 0
                    'oTerminalReader.RequestPin = 0
                    'oTerminalReader.AccessKey = 0
                    'oTerminalReader.Type = "RX"

                    Dim oTerminal As New roTerminal()
                    oTerminal.ID = -1
                    oTerminal.Description = Me.txtNameTerminal.Text
                    oTerminal.Type = Me.TerminalModel.Text
                    oTerminal.Behavior = ""
                    oTerminal.Location = Me.TerminalIP.Text
                    oTerminal.SupportedModes = "TA,ACC,ACCTA"
                    oTerminal.SupportedOutputs = "1"
                    oTerminal.SupportedSirens = "1"
                    oTerminal.SirensOutput = "0"
                    oTerminal.Other = CInt(Me.TerminalSN.Text) & ";" & Me.txtRegister.Text
                    oTerminal.RigidMode = "0"
                    oTerminal.AllowAntiPassBack = False
                    oTerminal.IsDifferentZoneTime = False
                    oTerminal.ZoneTime = 0
                    oTerminal.AllowCustomButton = False
                    oTerminal.CustomOutput = 0
                    oTerminal.CustomDuration = 0
                    oTerminal.CustomField = 0
                    oTerminal.CustomFieldValue = "0"
                    oTerminal.AllowMoveReason = False

                    'Creem el Reader
                    Dim oTerminalReader As New roTerminal.roTerminalReader()
                    oTerminalReader.IDTerminal = oTerminal.ID
                    oTerminalReader.ID = 1
                    oTerminalReader.Description = "Reader 1"
                    oTerminalReader.ScopeMode = DTOs.ScopeMode.TA
                    oTerminalReader.UseDispKey = 1
                    oTerminalReader.InteractionAction = DTOs.InteractionAction.X
                    oTerminalReader.ValidationMode = DTOs.ValidationMode.Local
                    oTerminalReader.OHP = False
                    oTerminalReader.Duration = 0
                    oTerminalReader.Output = 0
                    oTerminalReader.InvalidOutput = 0
                    oTerminalReader.RequestPin = 0
                    oTerminalReader.AccessKey = 0
                    oTerminalReader.Type = "RX"

                    If oTerminal.Readers Is Nothing Then oTerminal.Readers = New ArrayList()
                    oTerminal.Readers.Add(oTerminalReader)

                    'If API.TerminalServiceMethods.AddTerminalReader(Me.Page, oTerminal, oTerminalReader) = False Then
                    '    strErrorInfo = API.TerminalServiceMethods.oState.ErrorDetail
                    'End If

                    If API.TerminalServiceMethods.SaveTerminal(Me.Page, oTerminal, True) = False Then
                        'strErrorInfo = API.TerminalServiceMethods.oState.ErrorDetail
                        strErrorInfo = Me.Language.Translate("End.ErrorInSaveTerminal", Me.DefaultScope)
                    Else
                        bolSaved = True
                    End If

                End If
            Else 'Reemplaç del terminal
                Dim oTerminal As roTerminal = API.TerminalServiceMethods.GetTerminal(Me.Page, Me.TermToReplaceID.Value, False)
                oTerminal.Other = Me.TerminalSN.Text & ";" & Me.txtRegister.Text

                If API.TerminalServiceMethods.SaveTerminal(Me.Page, oTerminal, True) = False Then
                    'strErrorInfo = API.TerminalServiceMethods.oState.ErrorDetail
                    strErrorInfo = Me.Language.Translate("End.ErrorInSaveTerminal", Me.DefaultScope)
                Else
                    bolSaved = True
                End If

            End If

            If bolSaved = True Then
                API.UserTaskServiceMethods.DeleteUserTaskById(Me.Page, "USERTASK:\\TERMINAL_UNRECOGNIZED_" & Me.TerminalSN.Text, True)
                If roWsUserManagement.SessionObject.States.UsertaskState.Result <> UserTaskResultEnum.NoError Then
                    'Ha hagut un error en l'eliminació de la tasca...
                    strErrorInfo = Me.Language.Translate("End.ErrorinDeleteUserTask", Me.DefaultScope) & "<br>" & roWsUserManagement.SessionObject.States.UsertaskState.ErrorText
                    bolSaved = False
                End If
            End If

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewTerminalWelcome1.Text", Me.DefaultScope)
            If bolSaved Then

                Me.MustRefresh = "9"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.NewTerminalWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewTerminalWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(4, 0)

        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        If strButtonKey = "MaxEmployeesAcceptKey" Then

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewEmployeeWelcome1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("MaximumEmployeeReached.Message", Me.DefaultScope)
            Me.lblWelcome2.ForeColor = Drawing.Color.Red
            Me.lblWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(4, 0)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1 ' Pantalla Num. de Serie

                Dim isSerialCorrect As Boolean = TerminalServiceMethods.CheckTerminalSerialNum(Me.Page, Me.txtRegister.Text, Me.TerminalSN.Text)

                If isSerialCorrect = False Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.InvalidSerialNum", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case 2 ' Nou / Reemplaç terminal

                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case 3 ' Seleccio Reemplaç terminal (sols si es reemplaça a Pas 2)

                'Comprobar que s'ha seleccionat un terminal per reemplaçar
                If TermToReplaceID.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.NoneTermReplace", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case 4 ' Nom del Nou Terminal (sols si es nou a Pas 2)

                If txtNameTerminal.Text = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page4.NoneTerminalDescription", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep4Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 2
            Case 3
                If TermToReplaceID.Value <> "" Then
                    Dim oTerminal As roTerminal = API.TerminalServiceMethods.GetTerminal(Me.Page, TermToReplaceID.Value, False)
                    If oTerminal IsNot Nothing Then txtNameTerminal.Text = oTerminal.Description
                End If
            Case 5
        End Select

        Select Case intActivePage
            Case 1
                txtRegister.Focus()
            Case 3
            Case 4
                txtNameTerminal.Focus()
            Case 5
        End Select

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If intOldPage = 4 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 4, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 4, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Function ExistsContractID(ByVal strContractID As String) As Boolean

        Dim bolRet As Boolean = False

        Dim oContract As roContract = API.ContractsServiceMethods.GetContract(Nothing, strContractID, False)
        bolRet = (oContract IsNot Nothing AndAlso API.ContractsServiceMethods.LastError.Result = ContractsResultEnum.NoError)

        Return bolRet

    End Function

    ''' <summary>
    ''' Crea un DataGrid (HTMLTable) amb els Terminals que es poden reemplaçar
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadTerminalsInTable()
        Try
            Dim dTable As DataTable = API.TerminalServiceMethods.GetTerminalsDataSet(Me.Page, "Type = '" & Replace(Me.TerminalModel.Text, "'", "''") & "'")
            If dTable Is Nothing Then Exit Sub 'Si no hi han registres, sortim de la funcio

            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            hTable.ID = "htTableTerm"
            hTable.Border = 0

            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Width = "30px"
            hTCell.Attributes("style") = "border-right: 0; width: 30px"
            hTCell.InnerHtml = "Id"
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Width = "200px"
            hTCell.InnerHtml = "Descripción"
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Width = "100px"
            hTCell.InnerHtml = "Modelo"
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Width = "100px"
            hTCell.InnerHtml = "Dirección IP"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                hTRow.ID = "htRow_" & n
                hTRow.Attributes("onmouseover") = "javascript: rowOver('" & hTRow.ClientID & "');"
                hTRow.Attributes("onmouseout") = "javascript: rowOut('" & hTRow.ClientID & "');"
                hTRow.Attributes("onclick") = "javascript: rowClick('" & hTRow.ClientID & "','" & dTable.Rows(n)("Id") & "','" & hTable.ClientID & "');"

                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    If dTable.Columns(y).ColumnName.ToUpper <> "ID" And
                    dTable.Columns(y).ColumnName.ToUpper <> "DESCRIPTION" And
                    dTable.Columns(y).ColumnName.ToUpper <> "TYPE" And
                    dTable.Columns(y).ColumnName.ToUpper <> "LOCATION" Then Continue For
                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    If Me.TermToReplaceID.Value <> "" Then
                        If dTable.Rows(n)("Id") = Me.TermToReplaceID.Value Then
                            hTCell.Attributes("class") &= " gridRowSelected"
                        End If
                    End If

                    hTCell.Style("padding") = "3px"
                    hTCell.InnerText = dTable.Rows(n)(y).ToString

                    If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)
                Next
                hTable.Rows.Add(hTRow)
            Next

            Me.grdTerminales.Controls.Add(hTable)
        Catch ex As Exception
            Dim hTableError As New HtmlTable
            Dim hTableCellError As New HtmlTableCell
            Dim hTableRowError As New HtmlTableRow
            hTableCellError.InnerHtml = ex.Message.ToString
            hTableRowError.Cells.Add(hTableCellError)
            hTableError.Rows.Add(hTableRowError)
            Me.grdTerminales.Controls.Add(hTableError)
        End Try
    End Sub

#End Region

End Class