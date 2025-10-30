Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base

Partial Class WebUserControls_EmployeeType
    Inherits UserControlBase

#Region "Properties"

    ''' <summary>
    ''' Mostrar/Ocultar los tipos desactivados.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowDisabledTypes() As Boolean
        Get
            Dim b As Boolean = ViewState("ShowDisabledTypes")
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowDisabledTypes") = value
        End Set
    End Property

    Public ReadOnly Property chkAttendanceEnabled() As Boolean
        Get
            Return Me.chkAttendance.Enabled
        End Get
    End Property

    Public ReadOnly Property chkJobEnabled() As Boolean
        Get
            Return Me.chkJob.Enabled
        End Get
    End Property

    Public ReadOnly Property chkAccessEnabled() As Boolean
        Get
            Return Me.chkAccess.Enabled
        End Get
    End Property

    Public ReadOnly Property chkExternsEnabled() As Boolean
        Get
            Return Me.chkExterns.Enabled
        End Get
    End Property

    Public ReadOnly Property chkPreventionEnabled() As Boolean
        Get
            Return Me.chkPrevention.Enabled
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Cambiamos el texto del check de accesos según estado
        If chkAccess.Checked Then
            lblAccessDescription.Text = Me.Language.Translate("CheckAccess.CheckedDescription", DefaultScope)
        Else
            lblAccessDescription.Text = Me.Language.Translate("CheckAccess.UncheckedDescription", DefaultScope)
        End If
    End Sub

    Protected Sub chkAttendance_CheckedChanged(ByVal sender As Object) Handles chkAttendance.CheckedChanged
        ' El check de presencia nunca puede estar desmarcado.
        Me.chkAttendance.Checked = True
    End Sub

    Protected Sub ibtJob_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtJob.Click
        Me.chkJob.Checked = True
    End Sub

    Protected Sub ibtAccess_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtAccess.Click
        Me.chkAccess.Checked = True
    End Sub

    Protected Sub ibtExterns_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtExterns.Click
        Me.chkExterns.Checked = True
    End Sub

    Protected Sub ibtPrevention_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtPrevention.Click
        Me.chkPrevention.Checked = True
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Muestra la configuración del tipo de empleado por pantalla
    ''' </summary>
    ''' <param name="oEmployee">Objeto empleado a mostrar</param>
    ''' <remarks></remarks>
    Public Sub LoadData(ByVal oEmployee As roEmployee)

        Dim gVTExpressInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Version\LiveExpress")
        Dim gJobsInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv")
        Dim bolOHPInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\OHP")

        If Not gVTExpressInstalled Then
            'Si no hay produccion
            If Not gJobsInstalled Then
                Me.chkAttendance.Enabled = False
                Me.chkJob.Enabled = False
                Me.chkAccess.Enabled = False ' No dejamos modificar el check de control de accesos. Se marca o desmarca automàticamente en función de si el empleado está en un grupo de accesos o no.
                Me.chkExterns.Enabled = False
                Me.chkPrevention.Enabled = bolOHPInstalled
                Me.chkAttendance.Checked = True
                Me.chkJob.Checked = False
                Me.chkAccess.Checked = oEmployee.AccControlled
                Me.chkExterns.Checked = False
                Me.chkPrevention.Checked = False 'oEmployee.RiskControlled
            Else
                Me.chkAttendance.Enabled = False
                Me.chkJob.Enabled = True
                Me.chkAccess.Enabled = False ' No dejamos modificar el check de control de accesos. Se marca o desmarca automàticamente en función de si el empleado está en un grupo de accesos o no.
                Me.chkExterns.Enabled = False
                Me.chkPrevention.Enabled = bolOHPInstalled
                If oEmployee.Type = "A" Then
                    Me.chkAttendance.Checked = True
                    Me.chkJob.Checked = False
                Else
                    Me.chkAttendance.Checked = True
                    Me.chkJob.Checked = True
                End If
                Me.chkAccess.Checked = oEmployee.AccControlled
                Me.chkExterns.Checked = False
                Me.chkPrevention.Checked = False 'oEmployee.RiskControlled
            End If
        Else
            Me.chkAttendance.Enabled = False
            Me.chkJob.Enabled = False
            Me.chkAccess.Enabled = False
            Me.chkExterns.Enabled = False
            Me.chkPrevention.Enabled = False
            Me.chkAttendance.Checked = True
            Me.chkJob.Checked = False
            Me.chkAccess.Checked = False
            Me.chkExterns.Checked = False
            Me.chkPrevention.Checked = False
        End If

        ' Mostramos/ocultamos tipos desactivados
        Dim chkTypes() As Object = {Me.chkAttendance, Me.chkJob, Me.chkAccess, Me.chkExterns, Me.chkPrevention}
        For Each cnChk As Object In chkTypes
            cnChk.Visible = (cnChk.Enabled Or Me.ShowDisabledTypes)
        Next

    End Sub

    ''' <summary>
    ''' Obtiene la información del tipo de empleado de pantalla y lo actualiza al empleado
    ''' </summary>
    ''' <param name="oEmployee">Objeto empleado a actualizar</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RetrieveData(ByVal oEmployee As roEmployee) As Boolean

        Dim bolRet As Boolean = True

        If Me.chkJob.Checked Then
            oEmployee.Type = "J"
        ElseIf Me.chkAttendance.Checked Then
            oEmployee.Type = "A"
        End If

        oEmployee.RiskControlled = False 'Me.chkPrevention.Checked

        Return bolRet

    End Function

#End Region

End Class