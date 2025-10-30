Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Public Class LicenseProxy
    Implements ILicenseSvc

    Public Function KeepAlive() As Boolean Implements ILicenseSvc.KeepAlive
        Return True
    End Function

    Public Function FeatureIsInstalled(ByVal Feature As String) As roGenericVtResponse(Of Boolean) Implements ILicenseSvc.FeatureIsInstalled
        Return LicenseMethods.FeatureIsInstalled(Feature)
    End Function

    Public Function FeatureData(ByVal Feature As String, ByVal Variable As String) As roGenericVtResponse(Of String) Implements ILicenseSvc.FeatureData
        Return LicenseMethods.FeatureData(Feature, Variable)
    End Function

    Public Function VersionInfo(ByVal _Current As String, ByVal _Type As String) As roGenericVtResponse(Of (Boolean, String, String)) Implements ILicenseSvc.VersionInfo
        Return LicenseMethods.VersionInfo(_Current, _Type)
    End Function

    Public Function CheckLivePortal() As roGenericVtResponse(Of Integer) Implements ILicenseSvc.CheckLivePortal
        Return LicenseMethods.CheckLivePortal()
    End Function

    Public Function GetLicenseInstalledSolutionStatus() As roGenericVtResponse(Of Generic.List(Of roLicenseSolution)) Implements ILicenseSvc.GetLicenseInstalledSolutionStatus
        Return LicenseMethods.GetLicenseInstalledSolutionStatus()
    End Function

    Public Function GetLicenseInstalledModulesStatus() As roGenericVtResponse(Of Generic.List(Of roLicenseModule)) Implements ILicenseSvc.GetLicenseInstalledModulesStatus
        Return LicenseMethods.GetLicenseInstalledModulesStatus()
    End Function

    Public Function GetLicenseMaxConcurrentSessions() As roGenericVtResponse(Of Integer) Implements ILicenseSvc.GetLicenseMaxConcurrentSessions
        Return LicenseMethods.GetLicenseMaxConcurrentSessions()
    End Function

End Class
