Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Requests_WebUserControls_frmRequestResume
    Inherits Web.UI.UserControl

    Public Sub SetRequestData(ByVal oRequestData As DataRow, ByVal oTypeRow As DataRow, ByVal oRequestPermission As Permission, ByVal strIdTableRow As String)

        Dim oLanguage As New roLanguageWeb
        WLHelperWeb.SetLanguage(oLanguage, Me.LanguageFile)

        Dim oMsgParams As New Generic.List(Of String)

        ' Obtenemos el nivel de mando del supervisor
        Dim intLevelOfAuthority As Integer = 10
        intLevelOfAuthority = 0
        If WLHelperWeb.CurrentPassport.ID > 0 Then
            If oRequestData.Table.Columns.Contains("NextlevelOfAuthorityRequired") AndAlso oRequestData("NextlevelOfAuthorityRequired") > 0 Then
                intLevelOfAuthority = oRequestData("NextlevelOfAuthorityRequired")
            Else
                intLevelOfAuthority = SecurityV3ServiceMethods.GetPassportLevelOfAuthority(Nothing, WLHelperWeb.CurrentPassport.ID, oRequestData("RequestType"), IIf(IsDBNull(oRequestData("IDCause")), 0, oRequestData("IDCause")), oRequestData("ID"))
            End If
        End If

        'Insertamos la imagen del empleado que solicita
        Me.imgEmployee.Src = Me.ResolveUrl("../../Employees/loadimage.aspx?IdEmployee=" & oRequestData("IDEmployee") & "&NewParam=" & Now.TimeOfDay.Seconds.ToString)
        Me.imgEmployee.Height = 48

        ' Nombre empleado
        oMsgParams = New Generic.List(Of String)
        oMsgParams.Add(roTypes.Any2String(oRequestData("EmployeeName")))
        oMsgParams.Add(roTypes.Any2String(oRequestData("GroupName")))
        Me.lblEmployeeName.Text = oLanguage.Translate("Request.EmployeeInfo", Me.DefaultScope, oMsgParams)

        ' Tipo solicitud
        Dim strImg As String = "../Images/RequestTypes/20/"
        ' Determinamos el nombre de la imagen del tipo de solicitud
        If oTypeRow IsNot Nothing Then
            strImg &= oTypeRow("ElementName") & ".png"
            Me.lblRequestType.Text = oTypeRow("ElementDesc")
        Else
            strImg &= "UnknownType.png"
            Me.lblRequestType.Text = String.Empty
        End If
        Me.imgRequestType.Src = strImg

        ' Resumen solicitud
        Me.lblRequestInfo.Text = roTypes.Any2String(oRequestData("RequestInfo"))

        ' Fecha solicitud
        oMsgParams = New Generic.List(Of String)

        Dim oReqDate As Date = roTypes.Any2DateTime(oRequestData("RequestDate"))
        Dim strDate As String = Format(oReqDate, HelperWeb.GetShortDateFormat) & " " & Format(oReqDate, HelperWeb.GetShortTimeFormat)

        oMsgParams.Add(strDate)
        Me.lblRequestDate.Text = oLanguage.Translate("Request.DateInfo", Me.DefaultScope, oMsgParams)

        Dim bolApproveEnabled As Boolean = False
        Dim bolRefuseEnabled As Boolean = False
        If oRequestData("Status") = eRequestStatus.Pending AndAlso oRequestPermission > Permission.Read Then
            If roTypes.Any2Integer(oRequestData("StatusLevel")) <= intLevelOfAuthority Then
                ' Si esta pendiente y el nivel de mando de la solicitud es igual o inferior, quiere decir que
                ' se ha creado directamente en este nivel para ser aprobado por un superior,
                ' entonces si tienes el mismo nivel no puedes gestionarla
                bolApproveEnabled = False
                bolRefuseEnabled = False
            Else
                bolApproveEnabled = True
                bolRefuseEnabled = True
            End If
        ElseIf oRequestData("Status") = eRequestStatus.OnGoing Then

            If oRequestData("RequestType") = eRequestType.ExchangeShiftBetweenEmployees AndAlso oRequestPermission > Permission.Read AndAlso roTypes.Any2Integer(oRequestData("StatusLevel")) = 0 Then
                bolApproveEnabled = True
                bolRefuseEnabled = True
            Else
                If oRequestData("StatusLevel") > intLevelOfAuthority AndAlso oRequestPermission > Permission.Read Then
                    bolApproveEnabled = True
                    bolRefuseEnabled = True
                Else ' Una vez aprobada o denegada ya no se puede modificar (aunque sea el mismo supervisor)
                    bolApproveEnabled = False
                    bolRefuseEnabled = False
                End If
            End If
        Else
            ' Si la última acción ha sido denegarla por mi mismo, la puedo modificar *** esta opción se ha desactivado ***
            bolApproveEnabled = False ' (oRequestData("StatusLevel") = intLevelOfAuthority And Any2Integer(oRequestData("LastRequestApprovalIDPassport")) = WLHelperWeb.CurrentPassport.ID)
            bolRefuseEnabled = False
        End If

        'Las solicitudes de intercambio de turnos entre empleados nunca se pueden aprobar desde visualtime live
        If roTypes.Any2Integer(oRequestData("RequestType")) = eRequestType.ExchangeShiftBetweenEmployees AndAlso oRequestData("Status") = eRequestStatus.Pending Then
            bolApproveEnabled = False
            bolRefuseEnabled = False
        End If

        'Validación de aprobaciones y rechazos automáticos por el usuario de sistema
        If roTypes.Any2Boolean(oRequestData("AutomaticValidation")) Then
            If oRequestData("Status") = eRequestStatus.Denied Then
                bolApproveEnabled = True
            Else
                bolApproveEnabled = False
                bolRefuseEnabled = False
            End If

        End If

        'Insertamos la imagen para aprobar la solicitud
        Me.divApproveRequest.Attributes("class") = "buttonApproveRequest" & IIf(Not bolApproveEnabled, "Disabled", "")
        If bolApproveEnabled Then
            Me.divApproveRequest.Attributes("style") = "cursor: pointer; height: 20px;"
            Me.divApproveRequest.Attributes("onclick") = "approveRequest('" & oRequestData("ID") & "', '" & strIdTableRow & "',''); return false;"
            Me.divApproveRequest.Attributes("title") = oLanguage.Translate("Action.Approve.Title", Me.DefaultScope)
        Else
            Me.divApproveRequest.Attributes("style") = "height: 20px;"
        End If

        'Insertamos la imagen para denegar la solicitud
        Me.divRefuseRequest.Attributes("class") = "buttonRefuseRequest" & IIf(Not bolRefuseEnabled, "Disabled", "")
        If bolRefuseEnabled Then
            Me.divRefuseRequest.Attributes("style") = "cursor: pointer; height: 20px;"
            Me.divRefuseRequest.Attributes("onclick") = "refuseRequest('" & oRequestData("ID") & "', '" & strIdTableRow & "','')"
            Me.divRefuseRequest.Attributes("title") = oLanguage.Translate("Action.Refuse.Title", Me.DefaultScope)
        Else
            Me.divRefuseRequest.Attributes("style") = "height: 20px;"
        End If

        ' Días de antigüedad
        oMsgParams = New Generic.List(Of String)
        Dim intFromDays As Integer = Math.Abs(DateDiff(DateInterval.Day, Now.Date, CDate(oRequestData("RequestDate")).Date))
        oMsgParams.Add(intFromDays)
        Me.lblFromDays.Text = oLanguage.Translate("Request.FromDaysInfo", Me.DefaultScope, oMsgParams)
        Me.lblFromDays.ToolTip = oLanguage.Translate("Request.FromDaysInfo.ToolTip", Me.DefaultScope)

        ' Días desde último cambio
        oMsgParams = New Generic.List(Of String)
        Dim xLastApprovalDate As Date = oRequestData("RequestDate")
        If Not IsDBNull(oRequestData("LastRequestApprovalDate")) Then xLastApprovalDate = oRequestData("LastRequestApprovalDate")
        Dim intLastApprovalDays As Integer = Math.Abs(DateDiff(DateInterval.Day, Now.Date, xLastApprovalDate.Date))
        oMsgParams.Add(intLastApprovalDays)
        Me.lblLastApprovalDays.Text = oLanguage.Translate("Request.LastApprovalDaysInfo", Me.DefaultScope, oMsgParams)
        Me.lblLastApprovalDays.ToolTip = oLanguage.Translate("Request.LastApprovalDaysInfo.ToolTip", Me.DefaultScope)

        If oRequestData("Status") <> eRequestStatus.Pending Then
            oMsgParams = New Generic.List(Of String)

            If (oRequestData("Status") <> eRequestStatus.Canceled) Then
                If roTypes.Any2String(oRequestData("LastRequestApprovalPassportName")) <> "" Then
                    oMsgParams.Add(roTypes.Any2String(oRequestData("LastRequestApprovalPassportName")))
                Else
                    oMsgParams.Add(" -Desconocido- ")
                End If
            Else
                oMsgParams.Add(roTypes.Any2String(oRequestData("EmployeeName")))
            End If

            If oRequestData("Status") = eRequestStatus.OnGoing Or oRequestData("Status") = eRequestStatus.Accepted Then
                Me.lblLastAction.Text = oLanguage.Translate("Request.LastActionInfo.Accepted.Passport", Me.DefaultScope, oMsgParams)
            ElseIf oRequestData("Status") = eRequestStatus.Denied Then
                Me.lblLastAction.Text = oLanguage.Translate("Request.LastActionInfo.Denied.Passport", Me.DefaultScope, oMsgParams)
            ElseIf oRequestData("Status") = eRequestStatus.Canceled Then
                Me.lblLastAction.Text = oLanguage.Translate("Request.LastActionInfo.Canceled.Passport", Me.DefaultScope, oMsgParams)
            End If
        End If

    End Sub

    Private Function LanguageFile() As String

        Dim strLanguageFile As String
        If Me.TemplateControl.AppRelativeTemplateSourceDirectory.StartsWith("~/Base/") Then
            strLanguageFile = ConfigurationManager.AppSettings("LanguageBaseFile")
        Else
            strLanguageFile = ConfigurationManager.AppSettings("LanguageFile")
        End If
        Return strLanguageFile

    End Function

    Private ReadOnly Property DefaultScope() As String
        Get
            Dim strScope As String
            If Me.TemplateControl IsNot Nothing Then
                strScope = System.IO.Path.GetFileNameWithoutExtension(Me.TemplateControl.AppRelativeVirtualPath)
            Else
                strScope = Me.ID
            End If
            Return strScope
        End Get
    End Property

End Class