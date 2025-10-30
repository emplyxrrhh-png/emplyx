Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class UploadCertificate
    Inherits PageBase

    Public Property IdRelatedObject() As Integer
        Get
            Return ViewState("UploadCertificate_IDEmployee")
        End Get

        Set(ByVal value As Integer)
            ViewState("UploadCertificate_IDEmployee") = value
        End Set
    End Property

    Public Property IdPGPKey() As Integer
        Get
            Return ViewState("UploadCertificate_IdPGPKey")
        End Get

        Set(ByVal value As Integer)
            ViewState("UploadCertificate_IdPGPKey") = value
        End Set
    End Property

    Public Property ControlClientId() As String
        Get
            Return ViewState("UploadCertificate_ControlClientId")
        End Get

        Set(ByVal value As String)
            ViewState("UploadCertificate_ControlClientId") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        InsertExtraJavascript("GridViewHelper", "~/Base/Scripts/GridViewHelper.js", , True)
        InsertExtraJavascript("EmployeeGroup", "~/Employees/Scripts/EmployeeGroups.js")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InsertCssIncludes(Page)
        If Not IsPostBack Then
            Me.ControlClientId = roTypes.Any2String(Request("ClientId"))
            hdnControlCaller.Value = ControlClientId
            If Azure.RoAzureSupport.CheckIfFileExists("pgp.pub", roLiveDatalinkFolders.certificates.ToString, roLiveQueueTypes.datalink) Then
                lblFileUploaded.Visible = True
            Else
                lblFileUploaded.Visible = False
            End If
        End If
    End Sub

    Protected Sub btOK_Click(ByVal source As Object, ByVal e As EventArgs) Handles btOK.Click
        If fUploader.PostedFile IsNot Nothing AndAlso fUploader.PostedFile.ContentLength > 0 Then
            Using stream As IO.Stream = fUploader.PostedFile.InputStream()
                stream.Position = 0
                'Guardamos la clave pública en el contenedor como pgp.pub
                If Not Azure.RoAzureSupport.SaveFileOnCompanyContainer(stream, "pgp.pub", roLiveDatalinkFolders.certificates.ToString, roLiveQueueTypes.datalink, True) Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "UploadCertificate::BtOkClick: Unable to save certificate")
                Else
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeOnOk", "Close_Response();", True)
                End If
            End Using
        End If


    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback

    End Sub

End Class