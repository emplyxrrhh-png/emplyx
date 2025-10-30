Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_EditObjectPopup
    Inherits PageBase

    Private strObjectType As String = ""
    Private strObjectID As String = ""

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            strObjectType = roTypes.Any2String(Me.Request("ObjectType"))
            strObjectID = roTypes.Any2String(Me.Request("ObjectID"))

            Select Case strObjectType
                Case "Concept"
                    Me.Img1.Src = Me.ResolveUrl("~/Concepts/Images/Acumulados80.png")
                    Me.lblDescription2.Text = API.ConceptsServiceMethods.GetConceptByID(Me.Page, roTypes.Any2Integer(strObjectID), False).Name
                Case "ConceptGroup"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/ConceptGroups.png")
                    Me.lblDescription2.Text = API.ConceptsServiceMethods.GetConceptGroupByID(Me.Page, roTypes.Any2Integer(strObjectID), False).Name
            End Select

            Me.newObjectName.Text = Me.lblDescription2.Text
            Me.lblTitle.Text = Me.Language.Translate("Title." & strObjectType, Me.DefaultScope)
            Me.lblDescription1.Text = Me.Language.Translate("Description" & strObjectType, Me.DefaultScope)
            Me.Img1.Width = 48
            Me.Img1.Height = 48

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "lblError_Client.SetVisible(false);", True)
        End If

    End Sub

    'Protected Sub btnAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click
    '    If txtCaptcha.Value = c1.Text & c2.Text & c3.Text & c4.Text Then

    '    End If
    'End Sub

End Class