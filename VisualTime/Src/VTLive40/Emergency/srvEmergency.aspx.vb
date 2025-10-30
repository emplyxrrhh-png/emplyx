Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Emergency_srvEmergency
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case Request("action")
            Case "executeEmergencyReport"
                Me.Controls.Clear()
                ExecuteEmergencyReport(roTypes.Any2String(Request("idReports")), roTypes.Any2String(Request("keyReport")), roTypes.Any2String(Request("sCompany")))
        End Select

    End Sub

    Private Sub ExecuteEmergencyReport(ByVal idReports As String, ByVal keyReport As String, ByVal sCompanyId As String)


        If AuthValidations.GetCompanyWithSessionInitiated() <> String.Empty AndAlso AuthValidations.GetCompanyWithSessionInitiated() <> sCompanyId.ToLower().Trim Then
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("ReportNameEmpty", Me.DefaultScope))
                Response.Write(rError.toJSON)
        Else
            If Not String.IsNullOrEmpty(idReports) AndAlso Not String.IsNullOrEmpty(sCompanyId) Then
                Dim sourceCID As String = roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId"))

                Try
                    HttpContext.Current.Session("roMultiCompanyId") = sCompanyId
                    Dim EmergencyReportKey As String = roTypes.Any2String(API.SecurityServiceMethods.GetEmergencyReportKey(Me))
                    If keyReport = EmergencyReportKey Then
                        Dim bRet As Boolean
                        bRet = API.ReportServiceMethods.ExecuteEmergencyReport(Me.Page, True, idReports) 'Añadimos los ids de los reports seleccionados a ejecutar

                        If bRet Then
                            Dim rOK As New roJSON.JSONError(False, Me.Language.Translate("LaunchedOk", Me.DefaultScope))
                            Response.Write(rOK.toJSON)
                        Else
                            Dim rError As New roJSON.JSONError(True, API.SecurityServiceMethods.LastErrorText)
                            Response.Write(rError.toJSON)
                        End If
                    Else
                        Dim rError As New roJSON.JSONError(True, Me.Language.Translate("IncorrectKey", Me.DefaultScope))
                        Response.Write(rError.toJSON)
                    End If
                Catch ex As Exception
                    'do nothing
                End Try

                HttpContext.Current.Session("roMultiCompanyId") = sourceCID

            Else
                Dim rError As New roJSON.JSONError(True, Me.Language.Translate("ReportNameEmpty", Me.DefaultScope))
                Response.Write(rError.toJSON)
            End If
        End If



    End Sub

End Class