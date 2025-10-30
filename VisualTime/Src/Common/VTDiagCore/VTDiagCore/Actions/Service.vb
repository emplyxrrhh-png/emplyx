Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.VTBase

Namespace VTDiagCore.ActionsService

    Public Class Service

        Public Sub ChangeVTLiveApiHTTPSEnabled()
            Dim oSettings As New roSettings
            Dim pathRoot = oSettings.GetVTSetting(eKeys.PathRoot)
            Dim webConfigPath = Path.Combine(pathRoot, "VTLiveApi", "web.config")
            Dim webConfig = XDocument.Load(webConfigPath)
            webConfig.Element("configuration").Element("system.serviceModel").Element("behaviors").Element("serviceBehaviors").Element("behavior").Element("serviceMetadata").Attribute("httpsGetEnabled").Value = IIf(IsVTLiveApiHTTPSEnabled(), "false", "true")
            webConfig.Save(webConfigPath)
        End Sub

        Public Function IsVTLiveApiHTTPSEnabled() As Boolean
            Dim oSettings As New roSettings
            Dim pathRoot = oSettings.GetVTSetting(eKeys.PathRoot)
            Dim webConfigPath = Path.Combine(pathRoot, "VTLiveApi", "web.config")
            Dim webConfig = XDocument.Load(webConfigPath)
            Return webConfig.Element("configuration").Element("system.serviceModel").Element("behaviors").Element("serviceBehaviors").Element("behavior").Element("serviceMetadata").Attribute("httpsGetEnabled").Value = "true"
        End Function

        Public Sub DeleteTerminalFolder(terminalId As Integer)
            roTerminal.ResetTerminalSyncData(terminalId)
        End Sub

    End Class

End Namespace