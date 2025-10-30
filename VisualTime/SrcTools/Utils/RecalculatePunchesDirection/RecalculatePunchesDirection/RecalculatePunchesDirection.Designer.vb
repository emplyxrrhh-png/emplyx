<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RecalculatePunchesDirection
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RecalculatePunchesDirection))
        Me.dtFirsDate = New System.Windows.Forms.DateTimePicker()
        Me.dtLastDate = New System.Windows.Forms.DateTimePicker()
        Me.txtEmployeeIDs = New System.Windows.Forms.TextBox()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'dtFirsDate
        '
        Me.dtFirsDate.Location = New System.Drawing.Point(20, 111)
        Me.dtFirsDate.Name = "dtFirsDate"
        Me.dtFirsDate.Size = New System.Drawing.Size(200, 20)
        Me.dtFirsDate.TabIndex = 0
        Me.dtFirsDate.Value = New Date(2017, 5, 22, 0, 0, 0, 0)
        '
        'dtLastDate
        '
        Me.dtLastDate.Location = New System.Drawing.Point(323, 111)
        Me.dtLastDate.Name = "dtLastDate"
        Me.dtLastDate.Size = New System.Drawing.Size(200, 20)
        Me.dtLastDate.TabIndex = 1
        Me.dtLastDate.Value = New Date(2018, 2, 28, 0, 0, 0, 0)
        '
        'txtEmployeeIDs
        '
        Me.txtEmployeeIDs.Location = New System.Drawing.Point(20, 49)
        Me.txtEmployeeIDs.Name = "txtEmployeeIDs"
        Me.txtEmployeeIDs.Size = New System.Drawing.Size(503, 20)
        Me.txtEmployeeIDs.TabIndex = 2
        Me.txtEmployeeIDs.Text = "2,3,4,5,6,7,8,9,10,11,12,13,14,15,16"
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(438, 169)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(75, 23)
        Me.cmdOK.TabIndex = 3
        Me.cmdOK.Text = "Aplicar"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(334, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Ids de empleados separados por coma (o * para todos los empleados)"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 92)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Fecha inicio"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(320, 92)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Fecha final"
        '
        'RecalculatePunchesDirection
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(538, 215)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.txtEmployeeIDs)
        Me.Controls.Add(Me.dtLastDate)
        Me.Controls.Add(Me.dtFirsDate)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "RecalculatePunchesDirection"
        Me.Text = "Recálculo de sentido de fichajes automáticos"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dtFirsDate As DateTimePicker
    Friend WithEvents dtLastDate As DateTimePicker
    Friend WithEvents txtEmployeeIDs As TextBox
    Friend WithEvents cmdOK As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
End Class
