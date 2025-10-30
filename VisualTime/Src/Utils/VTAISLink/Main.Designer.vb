<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim GridFormatRule1 As DevExpress.XtraGrid.GridFormatRule = New DevExpress.XtraGrid.GridFormatRule()
        Dim FormatConditionRuleValue1 As DevExpress.XtraEditors.FormatConditionRuleValue = New DevExpress.XtraEditors.FormatConditionRuleValue()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.DevToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.grpBox = New System.Windows.Forms.GroupBox()
        Me.nudMaxEmployees = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtURL = New System.Windows.Forms.TextBox()
        Me.lblURL = New System.Windows.Forms.Label()
        Me.lblTotalSegs = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnReport = New System.Windows.Forms.Button()
        Me.btnShowResponse = New System.Windows.Forms.Button()
        Me.btnShowRequest = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblSoft = New System.Windows.Forms.Label()
        Me.lblMedium = New System.Windows.Forms.Label()
        Me.lblHard = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblSoftDesc = New System.Windows.Forms.Label()
        Me.lblMediumDesc = New System.Windows.Forms.Label()
        Me.btnSendRequest = New System.Windows.Forms.Button()
        Me.lblTotalCovered = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblTotalToCover = New System.Windows.Forms.Label()
        Me.lblTotalLocations = New System.Windows.Forms.Label()
        Me.lblTotalUsers = New System.Windows.Forms.Label()
        Me.lblTotalDays = New System.Windows.Forms.Label()
        Me.lblDays = New System.Windows.Forms.Label()
        Me.lblPositions = New System.Windows.Forms.Label()
        Me.lblLocations = New System.Windows.Forms.Label()
        Me.lblUsers = New System.Windows.Forms.Label()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.nudMaxCalcMinutes = New System.Windows.Forms.NumericUpDown()
        Me.chkDiscardPreAssigned = New System.Windows.Forms.CheckBox()
        Me.btnOpenAccess = New System.Windows.Forms.Button()
        Me.dtTo = New System.Windows.Forms.DateTimePicker()
        Me.lblPlanTo = New System.Windows.Forms.Label()
        Me.lblPlanFrom = New System.Windows.Forms.Label()
        Me.dtFrom = New System.Windows.Forms.DateTimePicker()
        Me.cmbDataSource = New System.Windows.Forms.ComboBox()
        Me.lblData = New System.Windows.Forms.Label()
        Me.grdCalendar = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.JsonDataSource1 = New DevExpress.DataAccess.Json.JsonDataSource(Me.components)
        Me.MenuStrip1.SuspendLayout()
        Me.grpBox.SuspendLayout()
        CType(Me.nudMaxEmployees, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudMaxCalcMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grdCalendar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DevToolsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(4, 1, 0, 1)
        Me.MenuStrip1.Size = New System.Drawing.Size(1128, 24)
        Me.MenuStrip1.TabIndex = 10
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'DevToolsToolStripMenuItem
        '
        Me.DevToolsToolStripMenuItem.Name = "DevToolsToolStripMenuItem"
        Me.DevToolsToolStripMenuItem.Size = New System.Drawing.Size(67, 22)
        Me.DevToolsToolStripMenuItem.Text = "DevTools"
        '
        'grpBox
        '
        Me.grpBox.Controls.Add(Me.nudMaxEmployees)
        Me.grpBox.Controls.Add(Me.Label4)
        Me.grpBox.Controls.Add(Me.txtURL)
        Me.grpBox.Controls.Add(Me.lblURL)
        Me.grpBox.Controls.Add(Me.lblTotalSegs)
        Me.grpBox.Controls.Add(Me.Label3)
        Me.grpBox.Controls.Add(Me.btnReport)
        Me.grpBox.Controls.Add(Me.btnShowResponse)
        Me.grpBox.Controls.Add(Me.btnShowRequest)
        Me.grpBox.Controls.Add(Me.Label9)
        Me.grpBox.Controls.Add(Me.lblSoft)
        Me.grpBox.Controls.Add(Me.lblMedium)
        Me.grpBox.Controls.Add(Me.lblHard)
        Me.grpBox.Controls.Add(Me.Label6)
        Me.grpBox.Controls.Add(Me.lblSoftDesc)
        Me.grpBox.Controls.Add(Me.lblMediumDesc)
        Me.grpBox.Controls.Add(Me.btnSendRequest)
        Me.grpBox.Controls.Add(Me.lblTotalCovered)
        Me.grpBox.Controls.Add(Me.Label2)
        Me.grpBox.Controls.Add(Me.lblTotalToCover)
        Me.grpBox.Controls.Add(Me.lblTotalLocations)
        Me.grpBox.Controls.Add(Me.lblTotalUsers)
        Me.grpBox.Controls.Add(Me.lblTotalDays)
        Me.grpBox.Controls.Add(Me.lblDays)
        Me.grpBox.Controls.Add(Me.lblPositions)
        Me.grpBox.Controls.Add(Me.lblLocations)
        Me.grpBox.Controls.Add(Me.lblUsers)
        Me.grpBox.Controls.Add(Me.btnLoad)
        Me.grpBox.Controls.Add(Me.Label1)
        Me.grpBox.Controls.Add(Me.nudMaxCalcMinutes)
        Me.grpBox.Controls.Add(Me.chkDiscardPreAssigned)
        Me.grpBox.Controls.Add(Me.btnOpenAccess)
        Me.grpBox.Controls.Add(Me.dtTo)
        Me.grpBox.Controls.Add(Me.lblPlanTo)
        Me.grpBox.Controls.Add(Me.lblPlanFrom)
        Me.grpBox.Controls.Add(Me.dtFrom)
        Me.grpBox.Controls.Add(Me.cmbDataSource)
        Me.grpBox.Controls.Add(Me.lblData)
        Me.grpBox.Location = New System.Drawing.Point(23, 42)
        Me.grpBox.Name = "grpBox"
        Me.grpBox.Size = New System.Drawing.Size(1078, 224)
        Me.grpBox.TabIndex = 11
        Me.grpBox.TabStop = False
        Me.grpBox.Text = "Escenario"
        '
        'nudMaxEmployees
        '
        Me.nudMaxEmployees.Location = New System.Drawing.Point(293, 149)
        Me.nudMaxEmployees.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.nudMaxEmployees.Name = "nudMaxEmployees"
        Me.nudMaxEmployees.Size = New System.Drawing.Size(66, 20)
        Me.nudMaxEmployees.TabIndex = 35
        Me.nudMaxEmployees.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(31, 152)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(238, 13)
        Me.Label4.TabIndex = 34
        Me.Label4.Text = "Total personas a considerar (selección aleatoria):"
        '
        'txtURL
        '
        Me.txtURL.Location = New System.Drawing.Point(136, 22)
        Me.txtURL.Name = "txtURL"
        Me.txtURL.Size = New System.Drawing.Size(273, 20)
        Me.txtURL.TabIndex = 33
        Me.txtURL.Text = "http://84.88.176.103:10003/robotics2/api/solver/"
        '
        'lblURL
        '
        Me.lblURL.AutoSize = True
        Me.lblURL.Location = New System.Drawing.Point(31, 30)
        Me.lblURL.Name = "lblURL"
        Me.lblURL.Size = New System.Drawing.Size(50, 13)
        Me.lblURL.TabIndex = 32
        Me.lblURL.Text = "Motor AI:"
        '
        'lblTotalSegs
        '
        Me.lblTotalSegs.AutoSize = True
        Me.lblTotalSegs.Location = New System.Drawing.Point(1031, 185)
        Me.lblTotalSegs.Name = "lblTotalSegs"
        Me.lblTotalSegs.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalSegs.TabIndex = 31
        Me.lblTotalSegs.Text = "0"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(893, 185)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(132, 13)
        Me.Label3.TabIndex = 30
        Me.Label3.Text = "Tiempo transcurrido (seg.):"
        '
        'btnReport
        '
        Me.btnReport.Enabled = False
        Me.btnReport.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReport.Location = New System.Drawing.Point(956, 22)
        Me.btnReport.Name = "btnReport"
        Me.btnReport.Size = New System.Drawing.Size(90, 81)
        Me.btnReport.TabIndex = 29
        Me.btnReport.Text = "Report"
        Me.btnReport.UseVisualStyleBackColor = True
        '
        'btnShowResponse
        '
        Me.btnShowResponse.Enabled = False
        Me.btnShowResponse.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowResponse.Location = New System.Drawing.Point(853, 65)
        Me.btnShowResponse.Name = "btnShowResponse"
        Me.btnShowResponse.Size = New System.Drawing.Size(97, 37)
        Me.btnShowResponse.TabIndex = 28
        Me.btnShowResponse.Text = "Ver JSON"
        Me.btnShowResponse.UseVisualStyleBackColor = True
        '
        'btnShowRequest
        '
        Me.btnShowRequest.Enabled = False
        Me.btnShowRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowRequest.Location = New System.Drawing.Point(853, 22)
        Me.btnShowRequest.Name = "btnShowRequest"
        Me.btnShowRequest.Size = New System.Drawing.Size(97, 37)
        Me.btnShowRequest.TabIndex = 27
        Me.btnShowRequest.Text = "Ver JSON"
        Me.btnShowRequest.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(776, 135)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(38, 13)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "Score:"
        '
        'lblSoft
        '
        Me.lblSoft.AutoSize = True
        Me.lblSoft.Location = New System.Drawing.Point(860, 185)
        Me.lblSoft.Name = "lblSoft"
        Me.lblSoft.Size = New System.Drawing.Size(13, 13)
        Me.lblSoft.TabIndex = 25
        Me.lblSoft.Text = "0"
        '
        'lblMedium
        '
        Me.lblMedium.AutoSize = True
        Me.lblMedium.Location = New System.Drawing.Point(860, 168)
        Me.lblMedium.Name = "lblMedium"
        Me.lblMedium.Size = New System.Drawing.Size(13, 13)
        Me.lblMedium.TabIndex = 24
        Me.lblMedium.Text = "0"
        '
        'lblHard
        '
        Me.lblHard.AutoSize = True
        Me.lblHard.Location = New System.Drawing.Point(860, 152)
        Me.lblHard.Name = "lblHard"
        Me.lblHard.Size = New System.Drawing.Size(13, 13)
        Me.lblHard.TabIndex = 23
        Me.lblHard.Text = "0"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(800, 152)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(33, 13)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Hard:"
        '
        'lblSoftDesc
        '
        Me.lblSoftDesc.AutoSize = True
        Me.lblSoftDesc.Location = New System.Drawing.Point(800, 185)
        Me.lblSoftDesc.Name = "lblSoftDesc"
        Me.lblSoftDesc.Size = New System.Drawing.Size(29, 13)
        Me.lblSoftDesc.TabIndex = 21
        Me.lblSoftDesc.Text = "Soft:"
        '
        'lblMediumDesc
        '
        Me.lblMediumDesc.AutoSize = True
        Me.lblMediumDesc.Location = New System.Drawing.Point(800, 168)
        Me.lblMediumDesc.Name = "lblMediumDesc"
        Me.lblMediumDesc.Size = New System.Drawing.Size(47, 13)
        Me.lblMediumDesc.TabIndex = 20
        Me.lblMediumDesc.Text = "Medium:"
        '
        'btnSendRequest
        '
        Me.btnSendRequest.Enabled = False
        Me.btnSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSendRequest.Location = New System.Drawing.Point(513, 65)
        Me.btnSendRequest.Name = "btnSendRequest"
        Me.btnSendRequest.Size = New System.Drawing.Size(334, 38)
        Me.btnSendRequest.TabIndex = 6
        Me.btnSendRequest.Text = "Planificar"
        Me.btnSendRequest.UseVisualStyleBackColor = True
        '
        'lblTotalCovered
        '
        Me.lblTotalCovered.AutoSize = True
        Me.lblTotalCovered.Location = New System.Drawing.Point(726, 185)
        Me.lblTotalCovered.Name = "lblTotalCovered"
        Me.lblTotalCovered.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalCovered.TabIndex = 19
        Me.lblTotalCovered.Text = "0"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(717, 185)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(12, 13)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "/"
        '
        'lblTotalToCover
        '
        Me.lblTotalToCover.AutoSize = True
        Me.lblTotalToCover.Location = New System.Drawing.Point(679, 185)
        Me.lblTotalToCover.Name = "lblTotalToCover"
        Me.lblTotalToCover.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalToCover.TabIndex = 17
        Me.lblTotalToCover.Text = "0"
        '
        'lblTotalLocations
        '
        Me.lblTotalLocations.AutoSize = True
        Me.lblTotalLocations.Location = New System.Drawing.Point(679, 168)
        Me.lblTotalLocations.Name = "lblTotalLocations"
        Me.lblTotalLocations.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalLocations.TabIndex = 16
        Me.lblTotalLocations.Text = "0"
        '
        'lblTotalUsers
        '
        Me.lblTotalUsers.AutoSize = True
        Me.lblTotalUsers.Location = New System.Drawing.Point(679, 151)
        Me.lblTotalUsers.Name = "lblTotalUsers"
        Me.lblTotalUsers.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalUsers.TabIndex = 15
        Me.lblTotalUsers.Text = "0"
        '
        'lblTotalDays
        '
        Me.lblTotalDays.AutoSize = True
        Me.lblTotalDays.Location = New System.Drawing.Point(679, 135)
        Me.lblTotalDays.Name = "lblTotalDays"
        Me.lblTotalDays.Size = New System.Drawing.Size(13, 13)
        Me.lblTotalDays.TabIndex = 14
        Me.lblTotalDays.Text = "0"
        '
        'lblDays
        '
        Me.lblDays.AutoSize = True
        Me.lblDays.Location = New System.Drawing.Point(521, 135)
        Me.lblDays.Name = "lblDays"
        Me.lblDays.Size = New System.Drawing.Size(36, 13)
        Me.lblDays.TabIndex = 13
        Me.lblDays.Text = "Días: "
        '
        'lblPositions
        '
        Me.lblPositions.AutoSize = True
        Me.lblPositions.Location = New System.Drawing.Point(521, 185)
        Me.lblPositions.Name = "lblPositions"
        Me.lblPositions.Size = New System.Drawing.Size(156, 13)
        Me.lblPositions.TabIndex = 12
        Me.lblPositions.Text = "Posiciones (a cubrir/cubiertas): "
        '
        'lblLocations
        '
        Me.lblLocations.AutoSize = True
        Me.lblLocations.Location = New System.Drawing.Point(521, 168)
        Me.lblLocations.Name = "lblLocations"
        Me.lblLocations.Size = New System.Drawing.Size(51, 13)
        Me.lblLocations.TabIndex = 11
        Me.lblLocations.Text = "Equipos: "
        '
        'lblUsers
        '
        Me.lblUsers.AutoSize = True
        Me.lblUsers.Location = New System.Drawing.Point(521, 151)
        Me.lblUsers.Name = "lblUsers"
        Me.lblUsers.Size = New System.Drawing.Size(57, 13)
        Me.lblUsers.TabIndex = 10
        Me.lblUsers.Text = "Personas: "
        '
        'btnLoad
        '
        Me.btnLoad.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLoad.Location = New System.Drawing.Point(513, 22)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(334, 37)
        Me.btnLoad.TabIndex = 9
        Me.btnLoad.Text = "Cargar escenario"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(272, 185)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(149, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Tiempo máximo cálculo (seg.):"
        '
        'nudMaxCalcMinutes
        '
        Me.nudMaxCalcMinutes.Location = New System.Drawing.Point(432, 178)
        Me.nudMaxCalcMinutes.Name = "nudMaxCalcMinutes"
        Me.nudMaxCalcMinutes.Size = New System.Drawing.Size(43, 20)
        Me.nudMaxCalcMinutes.TabIndex = 7
        Me.nudMaxCalcMinutes.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'chkDiscardPreAssigned
        '
        Me.chkDiscardPreAssigned.AutoSize = True
        Me.chkDiscardPreAssigned.Checked = True
        Me.chkDiscardPreAssigned.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDiscardPreAssigned.Location = New System.Drawing.Point(11, 184)
        Me.chkDiscardPreAssigned.Name = "chkDiscardPreAssigned"
        Me.chkDiscardPreAssigned.Size = New System.Drawing.Size(191, 17)
        Me.chkDiscardPreAssigned.TabIndex = 6
        Me.chkDiscardPreAssigned.Text = "Descartar posiciones preasignadas"
        Me.chkDiscardPreAssigned.UseVisualStyleBackColor = True
        '
        'btnOpenAccess
        '
        Me.btnOpenAccess.Location = New System.Drawing.Point(424, 51)
        Me.btnOpenAccess.Name = "btnOpenAccess"
        Me.btnOpenAccess.Size = New System.Drawing.Size(52, 21)
        Me.btnOpenAccess.TabIndex = 5
        Me.btnOpenAccess.Text = "Abrir ..."
        Me.btnOpenAccess.UseVisualStyleBackColor = True
        '
        'dtTo
        '
        Me.dtTo.Location = New System.Drawing.Point(136, 117)
        Me.dtTo.MinDate = New Date(2025, 7, 1, 0, 0, 0, 0)
        Me.dtTo.Name = "dtTo"
        Me.dtTo.Size = New System.Drawing.Size(200, 20)
        Me.dtTo.TabIndex = 3
        Me.dtTo.Value = New Date(2025, 7, 1, 0, 0, 0, 0)
        '
        'lblPlanTo
        '
        Me.lblPlanTo.AutoSize = True
        Me.lblPlanTo.Location = New System.Drawing.Point(77, 123)
        Me.lblPlanTo.Name = "lblPlanTo"
        Me.lblPlanTo.Size = New System.Drawing.Size(36, 13)
        Me.lblPlanTo.TabIndex = 4
        Me.lblPlanTo.Text = "hasta:"
        '
        'lblPlanFrom
        '
        Me.lblPlanFrom.AutoSize = True
        Me.lblPlanFrom.Location = New System.Drawing.Point(31, 89)
        Me.lblPlanFrom.Name = "lblPlanFrom"
        Me.lblPlanFrom.Size = New System.Drawing.Size(82, 13)
        Me.lblPlanFrom.TabIndex = 3
        Me.lblPlanFrom.Text = "Planifica desde:"
        '
        'dtFrom
        '
        Me.dtFrom.Location = New System.Drawing.Point(136, 87)
        Me.dtFrom.MinDate = New Date(2025, 7, 1, 0, 0, 0, 0)
        Me.dtFrom.Name = "dtFrom"
        Me.dtFrom.Size = New System.Drawing.Size(200, 20)
        Me.dtFrom.TabIndex = 2
        Me.dtFrom.Value = New Date(2025, 7, 1, 0, 0, 0, 0)
        '
        'cmbDataSource
        '
        Me.cmbDataSource.FormattingEnabled = True
        Me.cmbDataSource.Items.AddRange(New Object() {"dbAISLinkOneMonthOneHundred.accdb", "dbAISLinkOneWeek.accdb", "dbAISLinkOne.accdb", "dbAISLink.accdb"})
        Me.cmbDataSource.Location = New System.Drawing.Point(136, 51)
        Me.cmbDataSource.Name = "cmbDataSource"
        Me.cmbDataSource.Size = New System.Drawing.Size(273, 21)
        Me.cmbDataSource.TabIndex = 1
        '
        'lblData
        '
        Me.lblData.AutoSize = True
        Me.lblData.Location = New System.Drawing.Point(31, 55)
        Me.lblData.Name = "lblData"
        Me.lblData.Size = New System.Drawing.Size(78, 13)
        Me.lblData.TabIndex = 0
        Me.lblData.Text = "Base de datos:"
        '
        'grdCalendar
        '
        Me.grdCalendar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grdCalendar.Location = New System.Drawing.Point(23, 282)
        Me.grdCalendar.MainView = Me.GridView1
        Me.grdCalendar.Name = "grdCalendar"
        Me.grdCalendar.Size = New System.Drawing.Size(1078, 492)
        Me.grdCalendar.TabIndex = 12
        Me.grdCalendar.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        GridFormatRule1.Name = "Format0"
        FormatConditionRuleValue1.Expression = "Contains('', '')"
        GridFormatRule1.Rule = FormatConditionRuleValue1
        Me.GridView1.FormatRules.Add(GridFormatRule1)
        Me.GridView1.GridControl = Me.grdCalendar
        Me.GridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.GridView1.Name = "GridView1"
        Me.GridView1.OptionsCustomization.AllowRowSizing = True
        Me.GridView1.OptionsLayout.Columns.AddNewColumns = False
        Me.GridView1.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full
        Me.GridView1.OptionsView.ColumnAutoWidth = False
        Me.GridView1.OptionsView.RowAutoHeight = True
        Me.GridView1.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.[False]
        Me.GridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.[True]
        Me.GridView1.RowHeight = 20
        '
        'JsonDataSource1
        '
        Me.JsonDataSource1.Name = "JsonDataSource1"
        Me.JsonDataSource1.RootElement = Nothing
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1128, 690)
        Me.Controls.Add(Me.grdCalendar)
        Me.Controls.Add(Me.grpBox)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(1142, 683)
        Me.Name = "Main"
        Me.Text = "Next AI Link"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.grpBox.ResumeLayout(False)
        Me.grpBox.PerformLayout()
        CType(Me.nudMaxEmployees, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudMaxCalcMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grdCalendar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents DevToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents grpBox As GroupBox
    Friend WithEvents cmbDataSource As ComboBox
    Friend WithEvents lblData As Label
    Friend WithEvents btnOpenAccess As Button
    Friend WithEvents dtTo As DateTimePicker
    Friend WithEvents lblPlanTo As Label
    Friend WithEvents lblPlanFrom As Label
    Friend WithEvents dtFrom As DateTimePicker
    Friend WithEvents btnSendRequest As Button
    Friend WithEvents chkDiscardPreAssigned As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents nudMaxCalcMinutes As NumericUpDown
    Friend WithEvents btnLoad As Button
    Friend WithEvents lblTotalCovered As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents lblTotalToCover As Label
    Friend WithEvents lblTotalLocations As Label
    Friend WithEvents lblTotalUsers As Label
    Friend WithEvents lblTotalDays As Label
    Friend WithEvents lblDays As Label
    Friend WithEvents lblPositions As Label
    Friend WithEvents lblLocations As Label
    Friend WithEvents lblUsers As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents lblSoft As Label
    Friend WithEvents lblMedium As Label
    Friend WithEvents lblHard As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents lblSoftDesc As Label
    Friend WithEvents lblMediumDesc As Label
    Friend WithEvents btnShowResponse As Button
    Friend WithEvents btnShowRequest As Button
    Friend WithEvents btnReport As Button
    Friend WithEvents grdCalendar As DevExpress.XtraGrid.GridControl
    Friend WithEvents JsonDataSource1 As DevExpress.DataAccess.Json.JsonDataSource
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents lblTotalSegs As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtURL As TextBox
    Friend WithEvents lblURL As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents nudMaxEmployees As NumericUpDown
End Class
