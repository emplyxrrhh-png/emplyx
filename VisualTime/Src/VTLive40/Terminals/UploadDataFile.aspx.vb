Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class UploadDataFile
    Inherits PageBase

    Public Property CurrentTerminalID As Integer
        Get
            Return Session("Terminal_CurrentTerminalID")
        End Get
        Set(value As Integer)
            Session("Terminal_CurrentTerminalID") = value
        End Set
    End Property

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack AndAlso Not IsCallback Then

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "window.parent.parent.showLoader(false);", True)
        End If

    End Sub

    Protected Sub uploadClick(ByVal sender As Object, ByVal args As System.EventArgs)
        'Miramos si nos han pasado un fichero
        If fUploader.HasFile Then
            'Comprobamos que el fichero que nos envian tiene el nombre correcto
            If fUploader.FileName = "Terminal" + CurrentTerminalID.ToString.PadLeft(3, "0") + ".data" Then
                Dim sTerminalPath As String
                Dim oSettings As New Robotics.VTBase.roSettings
                Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                    _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                End If
                oSettings = New Robotics.VTBase.roSettings(_RegistryRoot & "Robotics\VisualTime")

                sTerminalPath = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & CurrentTerminalID.ToString
                sTerminalPath = IO.Path.Combine(sTerminalPath, "USB")
                sTerminalPath = IO.Path.Combine(sTerminalPath, "Terminal" + CurrentTerminalID.ToString.PadLeft(3, "0") + ".data")

                'Si aun existe el fichero es que no se ha procesado, no se puede sustituir.
                If IO.File.Exists(sTerminalPath) Then
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshGrid", "window.parent.showLoader(false);window.parent.PopupTerminalFile_Client.Hide();window.parent.showErrorPopup('UploadDataFile.Error.Title', 'error', 'UploadDataFile.Error.Description.InvalidFilExist','', 'UploadDataFile.Error.OK', 'UploadDataFile.Error.OKDesc', '');", True)
                Else
                    'Fichero almacenado
                    fUploader.PostedFile.SaveAs(sTerminalPath)

                    Dim lstFileParameterNames As New List(Of String)
                    Dim lstFileParameterValues As New List(Of String)
                    lstFileParameterNames.Add("{Path}")
                    lstFileParameterValues.Add(sTerminalPath)
                    lstFileParameterNames.Add("{FileName}")
                    lstFileParameterValues.Add(IO.Path.GetFileName(sTerminalPath))
                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tUploadFile, IO.Path.GetFileName(sTerminalPath), lstFileParameterNames, lstFileParameterValues, Me.Page)

                    Robotics.Web.Base.API.ConnectorServiceMethods.LaunchBroadcasterForTerminalTask(Nothing, CurrentTerminalID, "USB")
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshGrid", "window.parent.showLoader(false);window.parent.PopupTerminalFile_Client.Hide();window.parent.showErrorPopup('UploadDataFile.Info.Title', 'info', 'UploadDataFile.Info.description.FileCopyOK','', 'UploadDataFile.Info.OK', 'UploadDataFile.Info.OKDesc', '');", True)
                End If
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshGrid", "window.parent.showLoader(false);window.parent.PopupTerminalFile_Client.Hide();window.parent.showErrorPopup('UploadDataFile.Error.Title', 'error', 'UploadDataFile.Error.Description.InvalidFileName','', 'UploadDataFile.Error.OK', 'UploadDataFile.Error.OKDesc', '');", True)

            End If
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshGrid", "window.parent.showLoader(false);window.parent.PopupTerminalFile_Client.Hide();window.parent.showErrorPopup('UploadDataFile.Error.Title', 'error', 'UploadDataFile.Error.Description.InvalidFile','', 'UploadDataFile.Error.OK', 'UploadDataFile.Error.OKDesc', '');", True)

        End If

    End Sub

End Class