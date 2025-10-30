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
        Me.txtSerial = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.lblSerial = New System.Windows.Forms.Label()
        Me.btnDecrypt = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtSerial
        '
        Me.txtSerial.Font = New System.Drawing.Font("Microsoft Sans Serif", 30.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSerial.Location = New System.Drawing.Point(43, 65)
        Me.txtSerial.Name = "txtSerial"
        Me.txtSerial.Size = New System.Drawing.Size(406, 53)
        Me.txtSerial.TabIndex = 0
        Me.txtSerial.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(43, 12)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(406, 23)
        Me.btnLoad.TabIndex = 1
        Me.btnLoad.Text = "Cargar BBDD"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'lblSerial
        '
        Me.lblSerial.AutoSize = True
        Me.lblSerial.Location = New System.Drawing.Point(43, 46)
        Me.lblSerial.Name = "lblSerial"
        Me.lblSerial.Size = New System.Drawing.Size(84, 13)
        Me.lblSerial.TabIndex = 2
        Me.lblSerial.Text = "Número de serie"
        '
        'btnDecrypt
        '
        Me.btnDecrypt.Location = New System.Drawing.Point(43, 133)
        Me.btnDecrypt.Name = "btnDecrypt"
        Me.btnDecrypt.Size = New System.Drawing.Size(406, 23)
        Me.btnDecrypt.TabIndex = 3
        Me.btnDecrypt.Text = "Desencriptar"
        Me.btnDecrypt.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 180)
        Me.Controls.Add(Me.btnDecrypt)
        Me.Controls.Add(Me.lblSerial)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.txtSerial)
        Me.Name = "Form1"
        Me.Text = "mx8 database"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtSerial As TextBox
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnLoad As Button
    Friend WithEvents lblSerial As Label
    Friend WithEvents btnDecrypt As Button
End Class
