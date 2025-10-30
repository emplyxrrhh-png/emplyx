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
        Me.txtBody = New System.Windows.Forms.TextBox
        Me.txtAttach = New System.Windows.Forms.TextBox
        Me.txtSubject = New System.Windows.Forms.TextBox
        Me.txtTo = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.lblCorreo = New System.Windows.Forms.Label
        Me.lblAsunto = New System.Windows.Forms.Label
        Me.lblDescripcion = New System.Windows.Forms.Label
        Me.lblAdjunto = New System.Windows.Forms.Label
        Me.Button2 = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblDebug = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'txtBody
        '
        Me.txtBody.Location = New System.Drawing.Point(84, 57)
        Me.txtBody.Multiline = True
        Me.txtBody.Name = "txtBody"
        Me.txtBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtBody.Size = New System.Drawing.Size(229, 110)
        Me.txtBody.TabIndex = 9
        Me.txtBody.Text = "Prueba body"
        '
        'txtAttach
        '
        Me.txtAttach.Location = New System.Drawing.Point(84, 191)
        Me.txtAttach.Name = "txtAttach"
        Me.txtAttach.Size = New System.Drawing.Size(229, 20)
        Me.txtAttach.TabIndex = 8
        '
        'txtSubject
        '
        Me.txtSubject.Location = New System.Drawing.Point(84, 31)
        Me.txtSubject.Name = "txtSubject"
        Me.txtSubject.Size = New System.Drawing.Size(229, 20)
        Me.txtSubject.TabIndex = 7
        Me.txtSubject.Text = "Prueba asunto"
        '
        'txtTo
        '
        Me.txtTo.Location = New System.Drawing.Point(84, 5)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.Size = New System.Drawing.Size(229, 20)
        Me.txtTo.TabIndex = 6
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(36, 116)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(16, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Enviar"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'lblCorreo
        '
        Me.lblCorreo.AutoSize = True
        Me.lblCorreo.Location = New System.Drawing.Point(11, 9)
        Me.lblCorreo.Name = "lblCorreo"
        Me.lblCorreo.Size = New System.Drawing.Size(41, 13)
        Me.lblCorreo.TabIndex = 10
        Me.lblCorreo.Text = "Correo:"
        '
        'lblAsunto
        '
        Me.lblAsunto.AutoSize = True
        Me.lblAsunto.Location = New System.Drawing.Point(11, 34)
        Me.lblAsunto.Name = "lblAsunto"
        Me.lblAsunto.Size = New System.Drawing.Size(43, 13)
        Me.lblAsunto.TabIndex = 11
        Me.lblAsunto.Text = "Asunto:"
        '
        'lblDescripcion
        '
        Me.lblDescripcion.AutoSize = True
        Me.lblDescripcion.Location = New System.Drawing.Point(12, 60)
        Me.lblDescripcion.Name = "lblDescripcion"
        Me.lblDescripcion.Size = New System.Drawing.Size(66, 13)
        Me.lblDescripcion.TabIndex = 12
        Me.lblDescripcion.Text = "Descripción:"
        '
        'lblAdjunto
        '
        Me.lblAdjunto.AutoSize = True
        Me.lblAdjunto.Location = New System.Drawing.Point(12, 191)
        Me.lblAdjunto.Name = "lblAdjunto"
        Me.lblAdjunto.Size = New System.Drawing.Size(46, 13)
        Me.lblAdjunto.TabIndex = 13
        Me.lblAdjunto.Text = "Adjunto:"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(12, 217)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(301, 28)
        Me.Button2.TabIndex = 14
        Me.Button2.Text = "Enviar"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 248)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(196, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Puede especificar a varios Destinatarios"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 265)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(221, 13)
        Me.Label2.TabIndex = 16
        Me.Label2.Text = "si los separa con el carácter ;  <puntoycoma>"
        '
        'lblDebug
        '
        Me.lblDebug.AutoSize = True
        Me.lblDebug.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lblDebug.Location = New System.Drawing.Point(287, 286)
        Me.lblDebug.Name = "lblDebug"
        Me.lblDebug.Size = New System.Drawing.Size(39, 13)
        Me.lblDebug.TabIndex = 17
        Me.lblDebug.Text = "Debug"
        Me.lblDebug.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(326, 299)
        Me.Controls.Add(Me.lblDebug)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.lblAdjunto)
        Me.Controls.Add(Me.lblDescripcion)
        Me.Controls.Add(Me.lblAsunto)
        Me.Controls.Add(Me.lblCorreo)
        Me.Controls.Add(Me.txtBody)
        Me.Controls.Add(Me.txtAttach)
        Me.Controls.Add(Me.txtSubject)
        Me.Controls.Add(Me.txtTo)
        Me.Controls.Add(Me.Button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Testing Mail"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtBody As System.Windows.Forms.TextBox
    Friend WithEvents txtAttach As System.Windows.Forms.TextBox
    Friend WithEvents txtSubject As System.Windows.Forms.TextBox
    Friend WithEvents txtTo As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents lblCorreo As System.Windows.Forms.Label
    Friend WithEvents lblAsunto As System.Windows.Forms.Label
    Friend WithEvents lblDescripcion As System.Windows.Forms.Label
    Friend WithEvents lblAdjunto As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDebug As System.Windows.Forms.Label

End Class
