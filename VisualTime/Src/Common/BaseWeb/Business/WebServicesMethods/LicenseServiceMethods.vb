Imports Robotics.Base.DTOs
Imports Robotics.Security

Namespace API

    Public NotInheritable Class LicenseServiceMethods

        Public Shared Function FeatureIsInstalled(ByVal strFeature As String, Optional ByVal _passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            If WLHelperWeb.CurrentPassport(True) Is Nothing Then
                bolRet = VTLiveApi.LicenseMethods.FeatureIsInstalled(strFeature).Value
            Else
                If WLHelperWeb.InstalledFeatures.Contains(strFeature) Then
                    bolRet = CType(WLHelperWeb.InstalledFeatures(strFeature), Boolean)
                Else
                    bolRet = VTLiveApi.LicenseMethods.FeatureIsInstalled(strFeature).Value

                    WLHelperWeb.InstalledFeatures.Add(strFeature, bolRet)
                End If
            End If

            Return bolRet

        End Function

        Public Shared Function FeatureData(ByVal strFeature As String, ByVal strVariable As String) As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            Dim key As String = strFeature & "_" & strVariable
            If WLHelperWeb.InstalledFeaturesData.Contains(key) Then
                strRet = CType(WLHelperWeb.InstalledFeaturesData(key), String)
            Else
                strRet = VTLiveApi.LicenseMethods.FeatureData(strFeature, strVariable).Value
                If strRet.Trim <> "" Then
                    WLHelperWeb.InstalledFeaturesData.Add(key, strRet)
                End If
            End If

            Return strRet

        End Function

        Public Shared Function VersionInfo(ByRef currentVersion As String, ByRef currentVersionDate As String, ByRef versionHistory As String()) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            Dim resp As (Boolean, String, String, String())
            resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value

            bolRet = resp.Item1
            currentVersion = resp.Item2
            currentVersionDate = resp.Item3
            versionHistory = resp.Item4

            Return bolRet
        End Function

        Public Shared Function GetLicenseInstalledModulesStatus(ByVal strLicInfoFile As String) As Robotics.Base.DTOs.roLicenseModule()
            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Return VTLiveApi.LicenseMethods.GetLicenseInstalledModulesStatus(strLicInfoFile).Value.ToArray
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Function GetLicenseInstalledSolutionStatus(ByVal strLicInfoFile As String) As Robotics.Base.DTOs.roLicenseSolution()
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Return VTLiveApi.LicenseMethods.GetLicenseInstalledSolutionStatus(strLicInfoFile).Value.ToArray
        End Function

        Public Shared Function GetLicenseMaxConcurrentSessions(ByVal licInfoFile As String) As Integer
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Return VTLiveApi.LicenseMethods.GetLicenseMaxConcurrentSessions(licInfoFile).Value
        End Function

    End Class

End Namespace