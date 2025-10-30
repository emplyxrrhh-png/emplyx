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
        Me.txtCardFromCommunications = New System.Windows.Forms.TextBox()
        Me.txtCardFromBBDD = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtBytesTerminalReader = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbTerminalCodification = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmbDriverCodification = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtToVTDatabase = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtToTerminal = New System.Windows.Forms.TextBox()
        Me.btnGetCardForBBDD = New System.Windows.Forms.Button()
        Me.btnGetCarForTerminal = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cmbCardType = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'txtCardFromCommunications
        '
        Me.txtCardFromCommunications.Location = New System.Drawing.Point(247, 195)
        Me.txtCardFromCommunications.Name = "txtCardFromCommunications"
        Me.txtCardFromCommunications.Size = New System.Drawing.Size(303, 20)
        Me.txtCardFromCommunications.TabIndex = 0
        '
        'txtCardFromBBDD
        '
        Me.txtCardFromBBDD.Location = New System.Drawing.Point(247, 326)
        Me.txtCardFromBBDD.Name = "txtCardFromBBDD"
        Me.txtCardFromBBDD.Size = New System.Drawing.Size(303, 20)
        Me.txtCardFromBBDD.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(33, 202)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(178, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Tarjeta recibida por comunicaciones"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(33, 333)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(182, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Tarjeta recuperada de base de datos"
        '
        'txtBytesTerminalReader
        '
        Me.txtBytesTerminalReader.Location = New System.Drawing.Point(247, 35)
        Me.txtBytesTerminalReader.MaxLength = 1
        Me.txtBytesTerminalReader.Name = "txtBytesTerminalReader"
        Me.txtBytesTerminalReader.Size = New System.Drawing.Size(19, 20)
        Me.txtBytesTerminalReader.TabIndex = 4
        Me.txtBytesTerminalReader.Text = "3"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(37, 42)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Bytes leídos por lector"
        '
        'cmbTerminalCodification
        '
        Me.cmbTerminalCodification.FormattingEnabled = True
        Me.cmbTerminalCodification.Items.AddRange(New Object() {"Ninguna", "Hexadecimal", "Robotics"})
        Me.cmbTerminalCodification.Location = New System.Drawing.Point(247, 72)
        Me.cmbTerminalCodification.Name = "cmbTerminalCodification"
        Me.cmbTerminalCodification.Size = New System.Drawing.Size(121, 21)
        Me.cmbTerminalCodification.TabIndex = 6
        Me.cmbTerminalCodification.Text = "Ninguna"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(37, 80)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(171, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Codificación de tarjetas en terminal"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(37, 107)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(165, 13)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Codificación de tarjetas en BBDD"
        '
        'cmbDriverCodification
        '
        Me.cmbDriverCodification.FormattingEnabled = True
        Me.cmbDriverCodification.Items.AddRange(New Object() {"Ninguna", "Hexadecimal", "Robotics", "Octal"})
        Me.cmbDriverCodification.Location = New System.Drawing.Point(247, 99)
        Me.cmbDriverCodification.Name = "cmbDriverCodification"
        Me.cmbDriverCodification.Size = New System.Drawing.Size(121, 21)
        Me.cmbDriverCodification.TabIndex = 8
        Me.cmbDriverCodification.Text = "Robotics"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(33, 260)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(136, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Tarjeta a guardar en BBDD"
        '
        'txtToVTDatabase
        '
        Me.txtToVTDatabase.Location = New System.Drawing.Point(247, 253)
        Me.txtToVTDatabase.Name = "txtToVTDatabase"
        Me.txtToVTDatabase.ReadOnly = True
        Me.txtToVTDatabase.Size = New System.Drawing.Size(303, 20)
        Me.txtToVTDatabase.TabIndex = 10
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(33, 389)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(129, 13)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "Tarjeta a enviar a terminal"
        '
        'txtToTerminal
        '
        Me.txtToTerminal.Location = New System.Drawing.Point(247, 382)
        Me.txtToTerminal.Name = "txtToTerminal"
        Me.txtToTerminal.ReadOnly = True
        Me.txtToTerminal.Size = New System.Drawing.Size(303, 20)
        Me.txtToTerminal.TabIndex = 12
        '
        'btnGetCardForBBDD
        '
        Me.btnGetCardForBBDD.Location = New System.Drawing.Point(315, 221)
        Me.btnGetCardForBBDD.Name = "btnGetCardForBBDD"
        Me.btnGetCardForBBDD.Size = New System.Drawing.Size(139, 24)
        Me.btnGetCardForBBDD.TabIndex = 14
        Me.btnGetCardForBBDD.Text = "v"
        Me.btnGetCardForBBDD.UseVisualStyleBackColor = True
        '
        'btnGetCarForTerminal
        '
        Me.btnGetCarForTerminal.Location = New System.Drawing.Point(315, 352)
        Me.btnGetCarForTerminal.Name = "btnGetCarForTerminal"
        Me.btnGetCarForTerminal.Size = New System.Drawing.Size(139, 24)
        Me.btnGetCarForTerminal.TabIndex = 15
        Me.btnGetCarForTerminal.Text = "v"
        Me.btnGetCarForTerminal.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(37, 134)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(72, 13)
        Me.Label8.TabIndex = 17
        Me.Label8.Text = "Tipo de lector"
        '
        'cmbCardType
        '
        Me.cmbCardType.FormattingEnabled = True
        Me.cmbCardType.Items.AddRange(New Object() {"HID", "MiFare", "Unique"})
        Me.cmbCardType.Location = New System.Drawing.Point(247, 126)
        Me.cmbCardType.Name = "cmbCardType"
        Me.cmbCardType.Size = New System.Drawing.Size(121, 21)
        Me.cmbCardType.TabIndex = 16
        Me.cmbCardType.Text = "Unique"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(948, 445)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.cmbCardType)
        Me.Controls.Add(Me.btnGetCarForTerminal)
        Me.Controls.Add(Me.btnGetCardForBBDD)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtToTerminal)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtToVTDatabase)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cmbDriverCodification)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cmbTerminalCodification)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtBytesTerminalReader)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtCardFromBBDD)
        Me.Controls.Add(Me.txtCardFromCommunications)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtCardFromCommunications As System.Windows.Forms.TextBox
    Friend WithEvents txtCardFromBBDD As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtBytesTerminalReader As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbTerminalCodification As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cmbDriverCodification As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtToVTDatabase As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtToTerminal As System.Windows.Forms.TextBox
    Friend WithEvents btnGetCardForBBDD As System.Windows.Forms.Button
    Friend WithEvents btnGetCarForTerminal As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbCardType As System.Windows.Forms.ComboBox

End Class
