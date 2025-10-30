<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DevTools
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DevTools))
        Me.lblElapsed = New System.Windows.Forms.Label()
        Me.txtResponse = New System.Windows.Forms.TextBox()
        Me.btnCheckRequest = New System.Windows.Forms.Button()
        Me.txtRequest = New System.Windows.Forms.TextBox()
        Me.txtID = New System.Windows.Forms.TextBox()
        Me.btnRequest = New System.Windows.Forms.Button()
        Me.btnTestRequest = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblElapsed
        '
        Me.lblElapsed.AutoSize = True
        Me.lblElapsed.Location = New System.Drawing.Point(241, 244)
        Me.lblElapsed.Name = "lblElapsed"
        Me.lblElapsed.Size = New System.Drawing.Size(0, 13)
        Me.lblElapsed.TabIndex = 16
        '
        'txtResponse
        '
        Me.txtResponse.Location = New System.Drawing.Point(38, 264)
        Me.txtResponse.MaxLength = 100000
        Me.txtResponse.Multiline = True
        Me.txtResponse.Name = "txtResponse"
        Me.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtResponse.Size = New System.Drawing.Size(725, 179)
        Me.txtResponse.TabIndex = 15
        '
        'btnCheckRequest
        '
        Me.btnCheckRequest.Location = New System.Drawing.Point(38, 235)
        Me.btnCheckRequest.Name = "btnCheckRequest"
        Me.btnCheckRequest.Size = New System.Drawing.Size(196, 23)
        Me.btnCheckRequest.TabIndex = 14
        Me.btnCheckRequest.Text = "Respuesta"
        Me.btnCheckRequest.UseVisualStyleBackColor = True
        '
        'txtRequest
        '
        Me.txtRequest.Location = New System.Drawing.Point(38, 37)
        Me.txtRequest.MaxLength = 100000
        Me.txtRequest.Multiline = True
        Me.txtRequest.Name = "txtRequest"
        Me.txtRequest.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtRequest.Size = New System.Drawing.Size(725, 163)
        Me.txtRequest.TabIndex = 13
        Me.txtRequest.Text = resources.GetString("txtRequest.Text")
        '
        'txtID
        '
        Me.txtID.Location = New System.Drawing.Point(240, 206)
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(44, 20)
        Me.txtID.TabIndex = 12
        '
        'btnRequest
        '
        Me.btnRequest.Location = New System.Drawing.Point(38, 206)
        Me.btnRequest.Name = "btnRequest"
        Me.btnRequest.Size = New System.Drawing.Size(196, 23)
        Me.btnRequest.TabIndex = 11
        Me.btnRequest.Text = "Llamada"
        Me.btnRequest.UseVisualStyleBackColor = True
        '
        'btnTestRequest
        '
        Me.btnTestRequest.Location = New System.Drawing.Point(38, 8)
        Me.btnTestRequest.Name = "btnTestRequest"
        Me.btnTestRequest.Size = New System.Drawing.Size(231, 23)
        Me.btnTestRequest.TabIndex = 10
        Me.btnTestRequest.Text = "Genera Request"
        Me.btnTestRequest.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(354, 8)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(231, 23)
        Me.Button1.TabIndex = 17
        Me.Button1.Text = "Genera Request"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'DevTools
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 465)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblElapsed)
        Me.Controls.Add(Me.txtResponse)
        Me.Controls.Add(Me.btnCheckRequest)
        Me.Controls.Add(Me.txtRequest)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.btnRequest)
        Me.Controls.Add(Me.btnTestRequest)
        Me.Name = "DevTools"
        Me.Text = "DevTools"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblElapsed As Label
    Friend WithEvents txtResponse As TextBox
    Friend WithEvents btnCheckRequest As Button
    Friend WithEvents txtRequest As TextBox
    Friend WithEvents txtID As TextBox
    Friend WithEvents btnRequest As Button
    Friend WithEvents btnTestRequest As Button
    Friend WithEvents Button1 As Button
End Class
