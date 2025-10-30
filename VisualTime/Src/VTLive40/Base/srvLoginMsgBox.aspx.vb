Imports Robotics.Web.Base

Partial Class Base_srvLoginMsgBox
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Select Case Request("action")
                Case "AlertLOPD"
                    AlertLOPD()
                Case "AlertPASSWORDEXPIRED"
                    AlertPASSWORDEXPIRED()
                Case "AlertLICENSEEXCEED"
                    AlertLicenseExceeded()
                Case "AlertENS"
                    AlertENS()
            End Select
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    Private Sub AlertLicenseExceeded()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Option1Text = Me.Language.Keyword("Button.Accept")
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Visible = False
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate("LicenseExceeded.Alert.Title", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate("LicenseExceeded.Alert.Description", Me.DefaultScope)

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = Me.ResolveUrl("~/Base/Images/MessageFrame/Alert32.png")

            Me.MsgBoxContent.btnOption1_Click = " HideMsgBoxForm(); return false;"
            ''Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AlertLOPD()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Option1Text = Me.Language.Keyword("Button.Accept")
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Visible = False
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate("LOPD.Alert.Title", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate("LOPD.Alert.Description", Me.DefaultScope)

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = Me.ResolveUrl("~/Base/Images/MessageFrame/Alert32.png")

            Me.MsgBoxContent.btnOption1_Click = "ShowChangePwd(false); HideMsgBoxForm(); return false;"
            ''Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AlertPASSWORDEXPIRED()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Option1Text = Me.Language.Keyword("Button.Accept")
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Visible = False
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate("PasswordExpired.Alert.Title", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate("PasswordExpired.Alert.Description", Me.DefaultScope)

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = Me.ResolveUrl("~/Base/Images/MessageFrame/Alert32.png")

            Me.MsgBoxContent.btnOption1_Click = "ShowChangePwd(false); HideMsgBoxForm(); return false;"
            ''Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AlertENS()
        If (HttpContext.Current.Session("ShowLegalText") IsNot Nothing AndAlso HttpContext.Current.Session("ShowLegalText").Equals(True)) Then
            Try
                Me.MsgBoxContent.Option1Text = Me.Language.Keyword("Button.Accept")
                Me.MsgBoxContent.Option1Description = ""
                Me.MsgBoxContent.Option1Visible = True
                Me.MsgBoxContent.Option2Visible = False
                Me.MsgBoxContent.Option3Visible = False

                Me.MsgBoxContent.Title = Me.Language.Translate("ENS.Alert.Title", Me.DefaultScope)
                Me.MsgBoxContent.TitleDescription = Me.Language.Translate("ENS.Alert.Description", Me.DefaultScope)

                Me.MsgBoxContent.AlertVisible = False

                Me.MsgBoxContent.IconUrl = Me.ResolveUrl("~/Base/Images/MessageFrame/Alert32.png")

                Me.MsgBoxContent.btnOption1_Click = "HideMsgBoxForm(); return false;"

                HttpContext.Current.Session("ShowLegalText.VTLive") = False
            Catch ex As Exception
            End Try
        End If
    End Sub

End Class