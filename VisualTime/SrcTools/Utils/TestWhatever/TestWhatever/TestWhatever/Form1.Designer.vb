<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.btnReprocessInvalidEntries = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dtpDate = New System.Windows.Forms.DateTimePicker()
        Me.txtZKDatetime = New System.Windows.Forms.TextBox()
        Me.btnZKDatetimeFormat = New System.Windows.Forms.Button()
        Me.btnTimeZoneAndDST = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtMin = New System.Windows.Forms.TextBox()
        Me.chkBucle = New System.Windows.Forms.CheckBox()
        Me.Button15 = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cbAction = New System.Windows.Forms.ComboBox()
        Me.chkCheckAutorizaciones = New System.Windows.Forms.CheckBox()
        Me.chkPRL = New System.Windows.Forms.CheckBox()
        Me.chkTimeZones = New System.Windows.Forms.CheckBox()
        Me.btnCheck = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbReaders = New System.Windows.Forms.ComboBox()
        Me.cbTerminal = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbEmployees = New System.Windows.Forms.ComboBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.txtTaskId = New System.Windows.Forms.TextBox()
        Me.lblImportExport = New System.Windows.Forms.Label()
        Me.btnImportExportTask = New System.Windows.Forms.Button()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.btnSavePunchPhoto = New System.Windows.Forms.Button()
        Me.Button14 = New System.Windows.Forms.Button()
        Me.txtFilename = New System.Windows.Forms.TextBox()
        Me.Button13 = New System.Windows.Forms.Button()
        Me.Button12 = New System.Windows.Forms.Button()
        Me.Button11 = New System.Windows.Forms.Button()
        Me.Button10 = New System.Windows.Forms.Button()
        Me.Button9 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.txtSerial = New System.Windows.Forms.TextBox()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.Button20 = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.txtIDZone = New System.Windows.Forms.TextBox()
        Me.Button19 = New System.Windows.Forms.Button()
        Me.txtInjection = New System.Windows.Forms.TextBox()
        Me.btnCheckInjection = New System.Windows.Forms.Button()
        Me.dtpicker = New System.Windows.Forms.DateTimePicker()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.btnSendEmployee = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.chkActive = New System.Windows.Forms.CheckBox()
        Me.txtEmployeeName = New System.Windows.Forms.TextBox()
        Me.txtEmployeeIdentifier = New System.Windows.Forms.TextBox()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.txtIdToDoListToGet = New System.Windows.Forms.TextBox()
        Me.Button18 = New System.Windows.Forms.Button()
        Me.txtTargetListIdToClone = New System.Windows.Forms.TextBox()
        Me.txtSourceListIdToClone = New System.Windows.Forms.TextBox()
        Me.Button17 = New System.Windows.Forms.Button()
        Me.txtTargetEmployeeId = New System.Windows.Forms.TextBox()
        Me.txtSourceListId = New System.Windows.Forms.TextBox()
        Me.Button16 = New System.Windows.Forms.Button()
        Me.txtIdTask = New System.Windows.Forms.TextBox()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.txtListId = New System.Windows.Forms.TextBox()
        Me.txtIdEmployee = New System.Windows.Forms.TextBox()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnAddToDoList = New System.Windows.Forms.Button()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.cbTCType = New System.Windows.Forms.ComboBox()
        Me.dpDateTC = New System.Windows.Forms.DateTimePicker()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtIDEmp = New System.Windows.Forms.TextBox()
        Me.btnCheckTC = New System.Windows.Forms.Button()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.btnAusentes = New System.Windows.Forms.Button()
        Me.btnPresentes = New System.Windows.Forms.Button()
        Me.cbZones = New System.Windows.Forms.ComboBox()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.btnSendNotifications = New System.Windows.Forms.Button()
        Me.btnCreateNotifications = New System.Windows.Forms.Button()
        Me.gbConnection = New System.Windows.Forms.GroupBox()
        Me.txtLocalConnection = New System.Windows.Forms.TextBox()
        Me.cbCompanies = New System.Windows.Forms.ComboBox()
        Me.rbAzureConnection = New System.Windows.Forms.RadioButton()
        Me.rbLocalConnection = New System.Windows.Forms.RadioButton()
        Me.Button21 = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.TabPage7.SuspendLayout()
        Me.TabPage8.SuspendLayout()
        Me.TabPage9.SuspendLayout()
        Me.TabPage10.SuspendLayout()
        Me.gbConnection.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnReprocessInvalidEntries
        '
        Me.btnReprocessInvalidEntries.Location = New System.Drawing.Point(20, 26)
        Me.btnReprocessInvalidEntries.Name = "btnReprocessInvalidEntries"
        Me.btnReprocessInvalidEntries.Size = New System.Drawing.Size(899, 89)
        Me.btnReprocessInvalidEntries.TabIndex = 0
        Me.btnReprocessInvalidEntries.Text = "Reprocesa InvalidEntries"
        Me.btnReprocessInvalidEntries.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage7)
        Me.TabControl1.Controls.Add(Me.TabPage8)
        Me.TabControl1.Controls.Add(Me.TabPage9)
        Me.TabControl1.Controls.Add(Me.TabPage10)
        Me.TabControl1.Location = New System.Drawing.Point(35, 129)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(958, 392)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.btnReprocessInvalidEntries)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(950, 366)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Invalid Entries"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.dtpDate)
        Me.TabPage2.Controls.Add(Me.txtZKDatetime)
        Me.TabPage2.Controls.Add(Me.btnZKDatetimeFormat)
        Me.TabPage2.Controls.Add(Me.btnTimeZoneAndDST)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.txtMin)
        Me.TabPage2.Controls.Add(Me.chkBucle)
        Me.TabPage2.Controls.Add(Me.Button15)
        Me.TabPage2.Controls.Add(Me.Label4)
        Me.TabPage2.Controls.Add(Me.cbAction)
        Me.TabPage2.Controls.Add(Me.chkCheckAutorizaciones)
        Me.TabPage2.Controls.Add(Me.chkPRL)
        Me.TabPage2.Controls.Add(Me.chkTimeZones)
        Me.TabPage2.Controls.Add(Me.btnCheck)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Controls.Add(Me.cbReaders)
        Me.TabPage2.Controls.Add(Me.cbTerminal)
        Me.TabPage2.Controls.Add(Me.Label2)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.cbEmployees)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(950, 366)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Terminales"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(293, 333)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(130, 13)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Mes / Dia      Hora:Minuto"
        '
        'dtpDate
        '
        Me.dtpDate.CustomFormat = "MM / dd      HH:mm"
        Me.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpDate.Location = New System.Drawing.Point(429, 330)
        Me.dtpDate.Name = "dtpDate"
        Me.dtpDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpDate.TabIndex = 21
        '
        'txtZKDatetime
        '
        Me.txtZKDatetime.Location = New System.Drawing.Point(834, 331)
        Me.txtZKDatetime.Name = "txtZKDatetime"
        Me.txtZKDatetime.Size = New System.Drawing.Size(98, 20)
        Me.txtZKDatetime.TabIndex = 20
        '
        'btnZKDatetimeFormat
        '
        Me.btnZKDatetimeFormat.Location = New System.Drawing.Point(565, 328)
        Me.btnZKDatetimeFormat.Name = "btnZKDatetimeFormat"
        Me.btnZKDatetimeFormat.Size = New System.Drawing.Size(246, 23)
        Me.btnZKDatetimeFormat.TabIndex = 19
        Me.btnZKDatetimeFormat.Text = ">> Formato de fecha loquísimo ZK >>"
        Me.btnZKDatetimeFormat.UseVisualStyleBackColor = True
        '
        'btnTimeZoneAndDST
        '
        Me.btnTimeZoneAndDST.Location = New System.Drawing.Point(29, 328)
        Me.btnTimeZoneAndDST.Name = "btnTimeZoneAndDST"
        Me.btnTimeZoneAndDST.Size = New System.Drawing.Size(238, 22)
        Me.btnTimeZoneAndDST.TabIndex = 18
        Me.btnTimeZoneAndDST.Text = "Zona horaria ZK y DST ..."
        Me.btnTimeZoneAndDST.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(891, 250)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 13)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = "segundos"
        '
        'txtMin
        '
        Me.txtMin.Location = New System.Drawing.Point(855, 243)
        Me.txtMin.Name = "txtMin"
        Me.txtMin.Size = New System.Drawing.Size(30, 20)
        Me.txtMin.TabIndex = 16
        Me.txtMin.Text = "90"
        '
        'chkBucle
        '
        Me.chkBucle.AutoSize = True
        Me.chkBucle.Location = New System.Drawing.Point(764, 246)
        Me.chkBucle.Name = "chkBucle"
        Me.chkBucle.Size = New System.Drawing.Size(98, 17)
        Me.chkBucle.TabIndex = 15
        Me.chkBucle.Text = "En bucle cada "
        Me.chkBucle.UseVisualStyleBackColor = True
        '
        'Button15
        '
        Me.Button15.Location = New System.Drawing.Point(29, 211)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(729, 91)
        Me.Button15.TabIndex = 14
        Me.Button15.Text = "Programar Terminal"
        Me.Button15.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(163, 86)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Tipo de fichaje"
        '
        'cbAction
        '
        Me.cbAction.FormattingEnabled = True
        Me.cbAction.Items.AddRange(New Object() {"Presencia", "Accesos", "Centros", "Tareas"})
        Me.cbAction.Location = New System.Drawing.Point(246, 78)
        Me.cbAction.Name = "cbAction"
        Me.cbAction.Size = New System.Drawing.Size(111, 21)
        Me.cbAction.TabIndex = 12
        '
        'chkCheckAutorizaciones
        '
        Me.chkCheckAutorizaciones.AutoSize = True
        Me.chkCheckAutorizaciones.Location = New System.Drawing.Point(295, 155)
        Me.chkCheckAutorizaciones.Name = "chkCheckAutorizaciones"
        Me.chkCheckAutorizaciones.Size = New System.Drawing.Size(178, 17)
        Me.chkCheckAutorizaciones.TabIndex = 11
        Me.chkCheckAutorizaciones.Text = "Considera Autorizaciones Si o Si"
        Me.chkCheckAutorizaciones.UseVisualStyleBackColor = True
        '
        'chkPRL
        '
        Me.chkPRL.AutoSize = True
        Me.chkPRL.Location = New System.Drawing.Point(180, 155)
        Me.chkPRL.Name = "chkPRL"
        Me.chkPRL.Size = New System.Drawing.Size(97, 17)
        Me.chkPRL.TabIndex = 10
        Me.chkPRL.Text = "Considera PRL"
        Me.chkPRL.UseVisualStyleBackColor = True
        '
        'chkTimeZones
        '
        Me.chkTimeZones.AutoSize = True
        Me.chkTimeZones.Location = New System.Drawing.Point(29, 155)
        Me.chkTimeZones.Name = "chkTimeZones"
        Me.chkTimeZones.Size = New System.Drawing.Size(115, 17)
        Me.chkTimeZones.TabIndex = 9
        Me.chkTimeZones.Text = "Considera Horarios"
        Me.chkTimeZones.UseVisualStyleBackColor = True
        '
        'btnCheck
        '
        Me.btnCheck.Location = New System.Drawing.Point(537, 78)
        Me.btnCheck.Name = "btnCheck"
        Me.btnCheck.Size = New System.Drawing.Size(395, 94)
        Me.btnCheck.TabIndex = 8
        Me.btnCheck.Text = "Verificar Permiso de Empleado por Lector"
        Me.btnCheck.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(44, 86)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(37, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Lector"
        '
        'cbReaders
        '
        Me.cbReaders.FormattingEnabled = True
        Me.cbReaders.Items.AddRange(New Object() {"1"})
        Me.cbReaders.Location = New System.Drawing.Point(89, 78)
        Me.cbReaders.Name = "cbReaders"
        Me.cbReaders.Size = New System.Drawing.Size(44, 21)
        Me.cbReaders.TabIndex = 6
        '
        'cbTerminal
        '
        Me.cbTerminal.FormattingEnabled = True
        Me.cbTerminal.Location = New System.Drawing.Point(108, 29)
        Me.cbTerminal.Name = "cbTerminal"
        Me.cbTerminal.Size = New System.Drawing.Size(824, 21)
        Me.cbTerminal.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(34, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(47, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Terminal"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(29, 124)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(54, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Empleado"
        '
        'cbEmployees
        '
        Me.cbEmployees.FormattingEnabled = True
        Me.cbEmployees.Location = New System.Drawing.Point(89, 117)
        Me.cbEmployees.Name = "cbEmployees"
        Me.cbEmployees.Size = New System.Drawing.Size(384, 21)
        Me.cbEmployees.TabIndex = 2
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.txtTaskId)
        Me.TabPage3.Controls.Add(Me.lblImportExport)
        Me.TabPage3.Controls.Add(Me.btnImportExportTask)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(950, 366)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Enlace de datos SaaS"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'txtTaskId
        '
        Me.txtTaskId.Location = New System.Drawing.Point(642, 23)
        Me.txtTaskId.Name = "txtTaskId"
        Me.txtTaskId.Size = New System.Drawing.Size(56, 20)
        Me.txtTaskId.TabIndex = 2
        Me.txtTaskId.Text = "243682"
        '
        'lblImportExport
        '
        Me.lblImportExport.AutoSize = True
        Me.lblImportExport.Location = New System.Drawing.Point(33, 23)
        Me.lblImportExport.Name = "lblImportExport"
        Me.lblImportExport.Size = New System.Drawing.Size(570, 13)
        Me.lblImportExport.TabIndex = 1
        Me.lblImportExport.Text = "Debe existir una tarea IMPORT o EXPORT pendiente (estado 0) en la base de datos s" &
    "eleccionada, con el siguiente ID "
        '
        'btnImportExportTask
        '
        Me.btnImportExportTask.Location = New System.Drawing.Point(36, 58)
        Me.btnImportExportTask.Name = "btnImportExportTask"
        Me.btnImportExportTask.Size = New System.Drawing.Size(892, 33)
        Me.btnImportExportTask.TabIndex = 0
        Me.btnImportExportTask.Text = "Ejecuta tarea (si existe y está en estado 0)"
        Me.btnImportExportTask.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.Button21)
        Me.TabPage6.Controls.Add(Me.btnSavePunchPhoto)
        Me.TabPage6.Controls.Add(Me.Button14)
        Me.TabPage6.Controls.Add(Me.txtFilename)
        Me.TabPage6.Controls.Add(Me.Button13)
        Me.TabPage6.Controls.Add(Me.Button12)
        Me.TabPage6.Controls.Add(Me.Button11)
        Me.TabPage6.Controls.Add(Me.Button10)
        Me.TabPage6.Controls.Add(Me.Button9)
        Me.TabPage6.Controls.Add(Me.Button8)
        Me.TabPage6.Controls.Add(Me.Button7)
        Me.TabPage6.Controls.Add(Me.txtSerial)
        Me.TabPage6.Controls.Add(Me.Button6)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(950, 366)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Azure"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'btnSavePunchPhoto
        '
        Me.btnSavePunchPhoto.Location = New System.Drawing.Point(386, 18)
        Me.btnSavePunchPhoto.Name = "btnSavePunchPhoto"
        Me.btnSavePunchPhoto.Size = New System.Drawing.Size(216, 23)
        Me.btnSavePunchPhoto.TabIndex = 11
        Me.btnSavePunchPhoto.Text = "Guarda foto terminal"
        Me.btnSavePunchPhoto.UseVisualStyleBackColor = True
        '
        'Button14
        '
        Me.Button14.Location = New System.Drawing.Point(643, 285)
        Me.Button14.Name = "Button14"
        Me.Button14.Size = New System.Drawing.Size(216, 23)
        Me.Button14.TabIndex = 10
        Me.Button14.Text = "Borra"
        Me.Button14.UseVisualStyleBackColor = True
        '
        'txtFilename
        '
        Me.txtFilename.BackColor = System.Drawing.SystemColors.Window
        Me.txtFilename.Location = New System.Drawing.Point(643, 201)
        Me.txtFilename.Name = "txtFilename"
        Me.txtFilename.Size = New System.Drawing.Size(216, 20)
        Me.txtFilename.TabIndex = 9
        Me.txtFilename.Text = "ImportEmployees.xlsx"
        Me.txtFilename.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Button13
        '
        Me.Button13.Location = New System.Drawing.Point(643, 256)
        Me.Button13.Name = "Button13"
        Me.Button13.Size = New System.Drawing.Size(216, 23)
        Me.Button13.TabIndex = 8
        Me.Button13.Text = "Marca finalizado"
        Me.Button13.UseVisualStyleBackColor = True
        '
        'Button12
        '
        Me.Button12.Location = New System.Drawing.Point(643, 227)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(216, 23)
        Me.Button12.TabIndex = 7
        Me.Button12.Text = "Marca en proceso"
        Me.Button12.UseVisualStyleBackColor = True
        '
        'Button11
        '
        Me.Button11.Location = New System.Drawing.Point(643, 103)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(216, 23)
        Me.Button11.TabIndex = 6
        Me.Button11.Text = "Listado de ficheros"
        Me.Button11.UseVisualStyleBackColor = True
        '
        'Button10
        '
        Me.Button10.Location = New System.Drawing.Point(386, 227)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(216, 23)
        Me.Button10.TabIndex = 5
        Me.Button10.Text = "Descarga fichero en carpeta v2"
        Me.Button10.UseVisualStyleBackColor = True
        '
        'Button9
        '
        Me.Button9.Location = New System.Drawing.Point(386, 159)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(216, 23)
        Me.Button9.TabIndex = 4
        Me.Button9.Text = "Descarga fichero en carpeta"
        Me.Button9.UseVisualStyleBackColor = True
        '
        'Button8
        '
        Me.Button8.Location = New System.Drawing.Point(386, 103)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(216, 23)
        Me.Button8.TabIndex = 3
        Me.Button8.Text = "Descarga fichero cliente"
        Me.Button8.UseVisualStyleBackColor = True
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(30, 104)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(143, 23)
        Me.Button7.TabIndex = 2
        Me.Button7.Text = "Button7"
        Me.Button7.UseVisualStyleBackColor = True
        '
        'txtSerial
        '
        Me.txtSerial.BackColor = System.Drawing.SystemColors.Window
        Me.txtSerial.Location = New System.Drawing.Point(30, 35)
        Me.txtSerial.Name = "txtSerial"
        Me.txtSerial.Size = New System.Drawing.Size(143, 20)
        Me.txtSerial.TabIndex = 1
        Me.txtSerial.Text = "BOCK192561229"
        Me.txtSerial.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(30, 61)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(143, 23)
        Me.Button6.TabIndex = 0
        Me.Button6.Text = "Busca empresa por SN"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.Button20)
        Me.TabPage4.Controls.Add(Me.TextBox1)
        Me.TabPage4.Controls.Add(Me.txtIDZone)
        Me.TabPage4.Controls.Add(Me.Button19)
        Me.TabPage4.Controls.Add(Me.txtInjection)
        Me.TabPage4.Controls.Add(Me.btnCheckInjection)
        Me.TabPage4.Controls.Add(Me.dtpicker)
        Me.TabPage4.Controls.Add(Me.Button1)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(950, 366)
        Me.TabPage4.TabIndex = 6
        Me.TabPage4.Text = "Cajón desastre"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'Button20
        '
        Me.Button20.Location = New System.Drawing.Point(535, 125)
        Me.Button20.Name = "Button20"
        Me.Button20.Size = New System.Drawing.Size(75, 23)
        Me.Button20.TabIndex = 28
        Me.Button20.Text = "Button20"
        Me.Button20.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(366, 128)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(100, 20)
        Me.TextBox1.TabIndex = 27
        '
        'txtIDZone
        '
        Me.txtIDZone.Enabled = False
        Me.txtIDZone.Location = New System.Drawing.Point(125, 141)
        Me.txtIDZone.Name = "txtIDZone"
        Me.txtIDZone.Size = New System.Drawing.Size(59, 20)
        Me.txtIDZone.TabIndex = 26
        Me.txtIDZone.Text = "2"
        '
        'Button19
        '
        Me.Button19.Location = New System.Drawing.Point(32, 141)
        Me.Button19.Name = "Button19"
        Me.Button19.Size = New System.Drawing.Size(75, 23)
        Me.Button19.TabIndex = 25
        Me.Button19.Text = "Empleados en zona "
        Me.Button19.UseVisualStyleBackColor = True
        '
        'txtInjection
        '
        Me.txtInjection.Location = New System.Drawing.Point(310, 19)
        Me.txtInjection.Name = "txtInjection"
        Me.txtInjection.Size = New System.Drawing.Size(556, 20)
        Me.txtInjection.TabIndex = 24
        Me.txtInjection.Text = "Hola me llamo Walter Espinoza y altero al personal"
        '
        'btnCheckInjection
        '
        Me.btnCheckInjection.Location = New System.Drawing.Point(310, 52)
        Me.btnCheckInjection.Name = "btnCheckInjection"
        Me.btnCheckInjection.Size = New System.Drawing.Size(219, 23)
        Me.btnCheckInjection.TabIndex = 23
        Me.btnCheckInjection.Text = "Comprobar inyección"
        Me.btnCheckInjection.UseVisualStyleBackColor = True
        '
        'dtpicker
        '
        Me.dtpicker.CustomFormat = "MM / dd      HH:mm:ss"
        Me.dtpicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpicker.Location = New System.Drawing.Point(32, 19)
        Me.dtpicker.Name = "dtpicker"
        Me.dtpicker.Size = New System.Drawing.Size(191, 20)
        Me.dtpicker.TabIndex = 22
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(32, 54)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Timespan"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.btnSendEmployee)
        Me.TabPage5.Controls.Add(Me.Label8)
        Me.TabPage5.Controls.Add(Me.Label7)
        Me.TabPage5.Controls.Add(Me.chkActive)
        Me.TabPage5.Controls.Add(Me.txtEmployeeName)
        Me.TabPage5.Controls.Add(Me.txtEmployeeIdentifier)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(950, 366)
        Me.TabPage5.TabIndex = 7
        Me.TabPage5.Text = "FCB FijosDisc"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'btnSendEmployee
        '
        Me.btnSendEmployee.Location = New System.Drawing.Point(352, 28)
        Me.btnSendEmployee.Name = "btnSendEmployee"
        Me.btnSendEmployee.Size = New System.Drawing.Size(205, 72)
        Me.btnSendEmployee.TabIndex = 56
        Me.btnSendEmployee.Text = "Enviar empleado"
        Me.btnSendEmployee.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(37, 63)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(108, 13)
        Me.Label8.TabIndex = 55
        Me.Label8.Text = "Nombre de empleado"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(23, 37)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(129, 13)
        Me.Label7.TabIndex = 54
        Me.Label7.Text = "Identificador de empleado"
        '
        'chkActive
        '
        Me.chkActive.AutoSize = True
        Me.chkActive.Location = New System.Drawing.Point(158, 83)
        Me.chkActive.Name = "chkActive"
        Me.chkActive.Size = New System.Drawing.Size(56, 17)
        Me.chkActive.TabIndex = 53
        Me.chkActive.Text = "Activo"
        Me.chkActive.UseVisualStyleBackColor = True
        '
        'txtEmployeeName
        '
        Me.txtEmployeeName.Location = New System.Drawing.Point(158, 56)
        Me.txtEmployeeName.Name = "txtEmployeeName"
        Me.txtEmployeeName.Size = New System.Drawing.Size(100, 20)
        Me.txtEmployeeName.TabIndex = 52
        '
        'txtEmployeeIdentifier
        '
        Me.txtEmployeeIdentifier.Location = New System.Drawing.Point(158, 30)
        Me.txtEmployeeIdentifier.Name = "txtEmployeeIdentifier"
        Me.txtEmployeeIdentifier.Size = New System.Drawing.Size(100, 20)
        Me.txtEmployeeIdentifier.TabIndex = 51
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.txtIdToDoListToGet)
        Me.TabPage7.Controls.Add(Me.Button18)
        Me.TabPage7.Controls.Add(Me.txtTargetListIdToClone)
        Me.TabPage7.Controls.Add(Me.txtSourceListIdToClone)
        Me.TabPage7.Controls.Add(Me.Button17)
        Me.TabPage7.Controls.Add(Me.txtTargetEmployeeId)
        Me.TabPage7.Controls.Add(Me.txtSourceListId)
        Me.TabPage7.Controls.Add(Me.Button16)
        Me.TabPage7.Controls.Add(Me.txtIdTask)
        Me.TabPage7.Controls.Add(Me.Button5)
        Me.TabPage7.Controls.Add(Me.Button4)
        Me.TabPage7.Controls.Add(Me.Button3)
        Me.TabPage7.Controls.Add(Me.Button2)
        Me.TabPage7.Controls.Add(Me.txtListId)
        Me.TabPage7.Controls.Add(Me.txtIdEmployee)
        Me.TabPage7.Controls.Add(Me.btnDelete)
        Me.TabPage7.Controls.Add(Me.btnAddToDoList)
        Me.TabPage7.Location = New System.Drawing.Point(4, 22)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage7.Size = New System.Drawing.Size(950, 366)
        Me.TabPage7.TabIndex = 8
        Me.TabPage7.Text = "ToDo Lists"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'txtIdToDoListToGet
        '
        Me.txtIdToDoListToGet.Location = New System.Drawing.Point(517, 94)
        Me.txtIdToDoListToGet.Name = "txtIdToDoListToGet"
        Me.txtIdToDoListToGet.Size = New System.Drawing.Size(36, 20)
        Me.txtIdToDoListToGet.TabIndex = 18
        Me.txtIdToDoListToGet.Text = "12"
        '
        'Button18
        '
        Me.Button18.Location = New System.Drawing.Point(370, 93)
        Me.Button18.Name = "Button18"
        Me.Button18.Size = New System.Drawing.Size(116, 20)
        Me.Button18.TabIndex = 17
        Me.Button18.Text = "Obtener lista"
        Me.Button18.UseVisualStyleBackColor = True
        '
        'txtTargetListIdToClone
        '
        Me.txtTargetListIdToClone.Location = New System.Drawing.Point(611, 215)
        Me.txtTargetListIdToClone.Name = "txtTargetListIdToClone"
        Me.txtTargetListIdToClone.Size = New System.Drawing.Size(36, 20)
        Me.txtTargetListIdToClone.TabIndex = 16
        Me.txtTargetListIdToClone.Text = "12"
        '
        'txtSourceListIdToClone
        '
        Me.txtSourceListIdToClone.Location = New System.Drawing.Point(517, 216)
        Me.txtSourceListIdToClone.Name = "txtSourceListIdToClone"
        Me.txtSourceListIdToClone.Size = New System.Drawing.Size(36, 20)
        Me.txtSourceListIdToClone.TabIndex = 15
        Me.txtSourceListIdToClone.Text = "17"
        '
        'Button17
        '
        Me.Button17.Location = New System.Drawing.Point(370, 215)
        Me.Button17.Name = "Button17"
        Me.Button17.Size = New System.Drawing.Size(116, 20)
        Me.Button17.TabIndex = 14
        Me.Button17.Text = "Clonar tareas lista"
        Me.Button17.UseVisualStyleBackColor = True
        '
        'txtTargetEmployeeId
        '
        Me.txtTargetEmployeeId.Location = New System.Drawing.Point(611, 173)
        Me.txtTargetEmployeeId.Name = "txtTargetEmployeeId"
        Me.txtTargetEmployeeId.Size = New System.Drawing.Size(36, 20)
        Me.txtTargetEmployeeId.TabIndex = 13
        Me.txtTargetEmployeeId.Text = "12"
        '
        'txtSourceListId
        '
        Me.txtSourceListId.Location = New System.Drawing.Point(517, 174)
        Me.txtSourceListId.Name = "txtSourceListId"
        Me.txtSourceListId.Size = New System.Drawing.Size(36, 20)
        Me.txtSourceListId.TabIndex = 12
        Me.txtSourceListId.Text = "17"
        '
        'Button16
        '
        Me.Button16.Location = New System.Drawing.Point(411, 173)
        Me.Button16.Name = "Button16"
        Me.Button16.Size = New System.Drawing.Size(75, 20)
        Me.Button16.TabIndex = 11
        Me.Button16.Text = "Clonar lista"
        Me.Button16.UseVisualStyleBackColor = True
        '
        'txtIdTask
        '
        Me.txtIdTask.Location = New System.Drawing.Point(517, 52)
        Me.txtIdTask.Name = "txtIdTask"
        Me.txtIdTask.Size = New System.Drawing.Size(36, 20)
        Me.txtIdTask.TabIndex = 10
        Me.txtIdTask.Text = "12"
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(370, 51)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(116, 20)
        Me.Button5.TabIndex = 9
        Me.Button5.Text = "Obtener tarea"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(42, 307)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(156, 20)
        Me.Button4.TabIndex = 8
        Me.Button4.Text = "Modifica 1 borra el resto"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(42, 261)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(118, 20)
        Me.Button3.TabIndex = 7
        Me.Button3.Text = "Elimina todo menos 1"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(42, 214)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 20)
        Me.Button2.TabIndex = 6
        Me.Button2.Text = "Añade a lista"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'txtListId
        '
        Me.txtListId.Location = New System.Drawing.Point(204, 215)
        Me.txtListId.Name = "txtListId"
        Me.txtListId.Size = New System.Drawing.Size(36, 20)
        Me.txtListId.TabIndex = 5
        Me.txtListId.Text = "1"
        '
        'txtIdEmployee
        '
        Me.txtIdEmployee.Location = New System.Drawing.Point(153, 51)
        Me.txtIdEmployee.Name = "txtIdEmployee"
        Me.txtIdEmployee.Size = New System.Drawing.Size(36, 20)
        Me.txtIdEmployee.TabIndex = 4
        Me.txtIdEmployee.Text = "12"
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(42, 173)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(75, 20)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Actualiza lista"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnAddToDoList
        '
        Me.btnAddToDoList.Location = New System.Drawing.Point(42, 51)
        Me.btnAddToDoList.Name = "btnAddToDoList"
        Me.btnAddToDoList.Size = New System.Drawing.Size(75, 20)
        Me.btnAddToDoList.TabIndex = 0
        Me.btnAddToDoList.Text = "Crear lista"
        Me.btnAddToDoList.UseVisualStyleBackColor = True
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.cbTCType)
        Me.TabPage8.Controls.Add(Me.dpDateTC)
        Me.TabPage8.Controls.Add(Me.Label9)
        Me.TabPage8.Controls.Add(Me.txtIDEmp)
        Me.TabPage8.Controls.Add(Me.btnCheckTC)
        Me.TabPage8.Location = New System.Drawing.Point(4, 22)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(950, 366)
        Me.TabPage8.TabIndex = 9
        Me.TabPage8.Text = "Teletrabajo"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'cbTCType
        '
        Me.cbTCType.FormattingEnabled = True
        Me.cbTCType.Items.AddRange(New Object() {"Teletrabajo", "Presencial"})
        Me.cbTCType.Location = New System.Drawing.Point(382, 66)
        Me.cbTCType.Name = "cbTCType"
        Me.cbTCType.Size = New System.Drawing.Size(121, 21)
        Me.cbTCType.TabIndex = 58
        Me.cbTCType.Text = "Teletrabajo"
        '
        'dpDateTC
        '
        Me.dpDateTC.CustomFormat = "dd / MM / yyyy"
        Me.dpDateTC.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dpDateTC.Location = New System.Drawing.Point(237, 69)
        Me.dpDateTC.Name = "dpDateTC"
        Me.dpDateTC.Size = New System.Drawing.Size(109, 20)
        Me.dpDateTC.TabIndex = 57
        Me.dpDateTC.Value = New Date(2022, 7, 22, 14, 35, 39, 0)
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(36, 76)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(129, 13)
        Me.Label9.TabIndex = 56
        Me.Label9.Text = "Identificador de empleado"
        '
        'txtIDEmp
        '
        Me.txtIDEmp.Location = New System.Drawing.Point(171, 69)
        Me.txtIDEmp.Name = "txtIDEmp"
        Me.txtIDEmp.Size = New System.Drawing.Size(32, 20)
        Me.txtIDEmp.TabIndex = 55
        Me.txtIDEmp.Text = "471"
        '
        'btnCheckTC
        '
        Me.btnCheckTC.Location = New System.Drawing.Point(544, 66)
        Me.btnCheckTC.Name = "btnCheckTC"
        Me.btnCheckTC.Size = New System.Drawing.Size(75, 23)
        Me.btnCheckTC.TabIndex = 0
        Me.btnCheckTC.Text = "Check"
        Me.btnCheckTC.UseVisualStyleBackColor = True
        '
        'TabPage9
        '
        Me.TabPage9.Controls.Add(Me.btnAusentes)
        Me.TabPage9.Controls.Add(Me.btnPresentes)
        Me.TabPage9.Controls.Add(Me.cbZones)
        Me.TabPage9.Location = New System.Drawing.Point(4, 22)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage9.Size = New System.Drawing.Size(950, 366)
        Me.TabPage9.TabIndex = 10
        Me.TabPage9.Text = "Estado de Zonas"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'btnAusentes
        '
        Me.btnAusentes.Location = New System.Drawing.Point(20, 112)
        Me.btnAusentes.Name = "btnAusentes"
        Me.btnAusentes.Size = New System.Drawing.Size(157, 23)
        Me.btnAusentes.TabIndex = 8
        Me.btnAusentes.Text = "Salieron en la última hora"
        Me.btnAusentes.UseVisualStyleBackColor = True
        '
        'btnPresentes
        '
        Me.btnPresentes.Location = New System.Drawing.Point(20, 65)
        Me.btnPresentes.Name = "btnPresentes"
        Me.btnPresentes.Size = New System.Drawing.Size(157, 23)
        Me.btnPresentes.TabIndex = 7
        Me.btnPresentes.Text = "Presentes"
        Me.btnPresentes.UseVisualStyleBackColor = True
        '
        'cbZones
        '
        Me.cbZones.FormattingEnabled = True
        Me.cbZones.Location = New System.Drawing.Point(20, 25)
        Me.cbZones.Name = "cbZones"
        Me.cbZones.Size = New System.Drawing.Size(534, 21)
        Me.cbZones.TabIndex = 6
        '
        'TabPage10
        '
        Me.TabPage10.Controls.Add(Me.btnSendNotifications)
        Me.TabPage10.Controls.Add(Me.btnCreateNotifications)
        Me.TabPage10.Location = New System.Drawing.Point(4, 22)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage10.Size = New System.Drawing.Size(950, 366)
        Me.TabPage10.TabIndex = 11
        Me.TabPage10.Text = "Notificaciones"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'btnSendNotifications
        '
        Me.btnSendNotifications.Location = New System.Drawing.Point(15, 51)
        Me.btnSendNotifications.Name = "btnSendNotifications"
        Me.btnSendNotifications.Size = New System.Drawing.Size(145, 23)
        Me.btnSendNotifications.TabIndex = 1
        Me.btnSendNotifications.Text = "Enviar notificaciones"
        Me.btnSendNotifications.UseVisualStyleBackColor = True
        '
        'btnCreateNotifications
        '
        Me.btnCreateNotifications.Location = New System.Drawing.Point(15, 22)
        Me.btnCreateNotifications.Name = "btnCreateNotifications"
        Me.btnCreateNotifications.Size = New System.Drawing.Size(145, 23)
        Me.btnCreateNotifications.TabIndex = 0
        Me.btnCreateNotifications.Text = "Generar notificaciones"
        Me.btnCreateNotifications.UseVisualStyleBackColor = True
        '
        'gbConnection
        '
        Me.gbConnection.Controls.Add(Me.txtLocalConnection)
        Me.gbConnection.Controls.Add(Me.cbCompanies)
        Me.gbConnection.Controls.Add(Me.rbAzureConnection)
        Me.gbConnection.Controls.Add(Me.rbLocalConnection)
        Me.gbConnection.Location = New System.Drawing.Point(39, 29)
        Me.gbConnection.Name = "gbConnection"
        Me.gbConnection.Size = New System.Drawing.Size(950, 81)
        Me.gbConnection.TabIndex = 3
        Me.gbConnection.TabStop = False
        Me.gbConnection.Text = "Conexión"
        '
        'txtLocalConnection
        '
        Me.txtLocalConnection.Enabled = False
        Me.txtLocalConnection.Location = New System.Drawing.Point(18, 43)
        Me.txtLocalConnection.Name = "txtLocalConnection"
        Me.txtLocalConnection.Size = New System.Drawing.Size(926, 20)
        Me.txtLocalConnection.TabIndex = 6
        '
        'cbCompanies
        '
        Me.cbCompanies.FormattingEnabled = True
        Me.cbCompanies.Location = New System.Drawing.Point(166, 15)
        Me.cbCompanies.Name = "cbCompanies"
        Me.cbCompanies.Size = New System.Drawing.Size(191, 21)
        Me.cbCompanies.TabIndex = 5
        '
        'rbAzureConnection
        '
        Me.rbAzureConnection.AutoSize = True
        Me.rbAzureConnection.Location = New System.Drawing.Point(108, 20)
        Me.rbAzureConnection.Name = "rbAzureConnection"
        Me.rbAzureConnection.Size = New System.Drawing.Size(52, 17)
        Me.rbAzureConnection.TabIndex = 4
        Me.rbAzureConnection.Text = "Azure"
        Me.rbAzureConnection.UseVisualStyleBackColor = True
        '
        'rbLocalConnection
        '
        Me.rbLocalConnection.AutoSize = True
        Me.rbLocalConnection.Checked = True
        Me.rbLocalConnection.Location = New System.Drawing.Point(20, 19)
        Me.rbLocalConnection.Name = "rbLocalConnection"
        Me.rbLocalConnection.Size = New System.Drawing.Size(51, 17)
        Me.rbLocalConnection.TabIndex = 3
        Me.rbLocalConnection.TabStop = True
        Me.rbLocalConnection.Text = "Local"
        Me.rbLocalConnection.UseVisualStyleBackColor = True
        '
        'Button21
        '
        Me.Button21.Location = New System.Drawing.Point(386, 47)
        Me.Button21.Name = "Button21"
        Me.Button21.Size = New System.Drawing.Size(216, 23)
        Me.Button21.TabIndex = 12
        Me.Button21.Text = "Recupero foto terminal"
        Me.Button21.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1026, 551)
        Me.Controls.Add(Me.gbConnection)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "TestWhatever"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.TabPage7.ResumeLayout(False)
        Me.TabPage7.PerformLayout()
        Me.TabPage8.ResumeLayout(False)
        Me.TabPage8.PerformLayout()
        Me.TabPage9.ResumeLayout(False)
        Me.TabPage10.ResumeLayout(False)
        Me.gbConnection.ResumeLayout(False)
        Me.gbConnection.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnReprocessInvalidEntries As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents btnCheck As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents cbReaders As ComboBox
    Friend WithEvents cbTerminal As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cbEmployees As ComboBox
    Friend WithEvents chkPRL As CheckBox
    Friend WithEvents chkTimeZones As CheckBox
    Friend WithEvents Label4 As Label
    Friend WithEvents cbAction As ComboBox
    Friend WithEvents chkCheckAutorizaciones As CheckBox
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents btnImportExportTask As Button
    Friend WithEvents TabPage6 As TabPage
    Friend WithEvents txtSerial As TextBox
    Friend WithEvents Button6 As Button
    Friend WithEvents Button7 As Button
    Friend WithEvents Button8 As Button
    Friend WithEvents Button9 As Button
    Friend WithEvents Button10 As Button
    Friend WithEvents Button11 As Button
    Friend WithEvents txtFilename As TextBox
    Friend WithEvents Button13 As Button
    Friend WithEvents Button12 As Button
    Friend WithEvents Button14 As Button
    Friend WithEvents Button15 As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents txtMin As TextBox
    Friend WithEvents chkBucle As CheckBox
    Friend WithEvents gbConnection As GroupBox
    Friend WithEvents rbAzureConnection As RadioButton
    Friend WithEvents rbLocalConnection As RadioButton
    Friend WithEvents txtLocalConnection As TextBox
    Friend WithEvents cbCompanies As ComboBox
    Friend WithEvents txtTaskId As TextBox
    Friend WithEvents lblImportExport As Label
    Friend WithEvents btnTimeZoneAndDST As Button
    Friend WithEvents txtZKDatetime As TextBox
    Friend WithEvents btnZKDatetimeFormat As Button
    Friend WithEvents dtpDate As DateTimePicker
    Friend WithEvents Label6 As Label
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents Button1 As Button
    Friend WithEvents dtpicker As DateTimePicker
    Friend WithEvents txtInjection As TextBox
    Friend WithEvents btnCheckInjection As Button
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents btnSendEmployee As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents chkActive As CheckBox
    Friend WithEvents txtEmployeeName As TextBox
    Friend WithEvents txtEmployeeIdentifier As TextBox
    Friend WithEvents TabPage7 As TabPage
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnAddToDoList As Button
    Friend WithEvents txtIdEmployee As TextBox
    Friend WithEvents txtListId As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents txtIdTask As TextBox
    Friend WithEvents Button5 As Button
    Friend WithEvents txtTargetEmployeeId As TextBox
    Friend WithEvents txtSourceListId As TextBox
    Friend WithEvents Button16 As Button
    Friend WithEvents txtTargetListIdToClone As TextBox
    Friend WithEvents txtSourceListIdToClone As TextBox
    Friend WithEvents Button17 As Button
    Friend WithEvents txtIdToDoListToGet As TextBox
    Friend WithEvents Button18 As Button
    Friend WithEvents txtIDZone As TextBox
    Friend WithEvents Button19 As Button
    Friend WithEvents Button20 As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TabPage8 As TabPage
    Friend WithEvents dpDateTC As DateTimePicker
    Friend WithEvents Label9 As Label
    Friend WithEvents txtIDEmp As TextBox
    Friend WithEvents btnCheckTC As Button
    Friend WithEvents cbTCType As ComboBox
    Friend WithEvents TabPage9 As TabPage
    Friend WithEvents cbZones As ComboBox
    Friend WithEvents btnAusentes As Button
    Friend WithEvents btnPresentes As Button
    Friend WithEvents TabPage10 As TabPage
    Friend WithEvents btnCreateNotifications As Button
    Friend WithEvents btnSendNotifications As Button
    Friend WithEvents btnSavePunchPhoto As Button
    Friend WithEvents Button21 As Button
End Class
