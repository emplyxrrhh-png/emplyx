Imports System.Globalization
Imports System.Reflection
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class LicenseMethods

    Public Shared Function FeatureIsInstalled(ByVal Feature As String) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oServerLicense As New roServerLicense()

        oResult.Value = oServerLicense.FeatureIsInstalled(Feature)

        Return oResult

    End Function

    Public Shared Function FeatureData(ByVal Feature As String, ByVal Variable As String) As roGenericVtResponse(Of String)

        Dim oResult As New roGenericVtResponse(Of String)
        Dim oServerLicense As New roServerLicense()
        Dim oLog As New roLog("ServerLiveLog", "Base.ServerLicense")

        oResult.Value = oServerLicense.FeatureData(Feature, Variable, oLog)

        Return oResult

    End Function

    Public Shared Function VersionInfo(ByVal _Current As String, ByVal _Type As String) As roGenericVtResponse(Of (Boolean, String, String, String()))

        Dim bolRet As New roGenericVtResponse(Of (Boolean, String, String, String()))
        bolRet.Value = (False, _Current, _Type, {})

        Dim versionHistory As String() = {}
        Dim currentVersionDate As String = ""

        Try

            Dim metadataAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyMetadataAttribute), False)
            For Each attr As AssemblyMetadataAttribute In metadataAttributes
                If attr.Key = "VersionHistory" Then
                    versionHistory = attr.Value.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                ElseIf attr.Key = "VersionDate" Then
                    currentVersionDate = attr.Value
                End If
            Next

            Try
                'Localizo fechas de versiones en función de la configuración de idioma del usuario que solicita la información
                Dim currentCulture As CultureInfo = CultureInfo.CurrentCulture

                If currentCulture.Name <> "es-ES" Then
                    Dim currentDate As Date
                    If Date.TryParseExact(currentVersionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, currentDate) Then
                        currentVersionDate = currentDate.ToString("d", currentCulture)
                    End If

                    For i As Integer = 0 To versionHistory.Length - 1
                        Dim parenthesisIndex As Integer = versionHistory(i).IndexOf("(")

                        If parenthesisIndex > -1 Then
                            Dim version As String = versionHistory(i).Substring(0, parenthesisIndex).Trim()
                            Dim dateText As String = versionHistory(i).Substring(parenthesisIndex + 1).TrimEnd(")"c)

                            ' Intentar parsear la fecha
                            Dim datePart As Date
                            If Date.TryParseExact(dateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datePart) Then
                                ' Convertir la fecha al formato de la cultura configurada
                                Dim dateFormatted As String = datePart.ToString("d", currentCulture)
                                versionHistory(i) = $"{version} ({dateFormatted})"
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                'Do nothing
            End Try

            bolRet.Value = (True, Assembly.GetExecutingAssembly().GetName().Version.ToString(), currentVersionDate, versionHistory)
        Catch ex As System.Exception
            Dim oLog As New roLog("ServerLiveLog", "LicenseService")
            oLog.logMessage(roLog.EventType.roError, "wsLicense::VersionInfo", ex)
        End Try

        Return bolRet

    End Function

    Public Shared Function CheckLivePortal() As roGenericVtResponse(Of Integer)

        Dim bolRet As New roGenericVtResponse(Of Integer)
        bolRet.Value = 0

        Try
            bolRet.Value = roBusinessSupport.CheckLivePortal
        Catch ex As System.Exception
            Dim oLog As New roLog("ServerLiveLog", "CheckLivePortal")
            oLog.logMessage(roLog.EventType.roError, "wsLicense::CheckLivePortal", ex)
        End Try

        Return bolRet

    End Function

    Public Shared Function GetLicenseInstalledSolutionStatus(Optional ByVal strLicInfoFile As String = "") As roGenericVtResponse(Of Generic.List(Of roLicenseSolution))
        Dim bolRet As New roGenericVtResponse(Of Generic.List(Of roLicenseSolution))
        bolRet.Value = Nothing

        Try

            Dim objLic As New Support.roLicenseInfo
            If Not objLic.Load(strLicInfoFile) Then
                bolRet.Value = New Generic.List(Of roLicenseSolution)
            Else
                bolRet.Value = objLic.Solutions
            End If
        Catch ex As Exception
            Dim oLog As New roLog("ServerLiveLog", "GetLicenseInstalledSolutionStatus")
            oLog.logMessage(roLog.EventType.roError, "wsLicense::GetLicenseInstalledSolutionStatus", ex)

        End Try

        Return bolRet

    End Function

    Public Shared Function GetLicenseInstalledModulesStatus(Optional ByVal strLicInfoFile As String = "") As roGenericVtResponse(Of Generic.List(Of roLicenseModule))

        Dim bolRet As New roGenericVtResponse(Of Generic.List(Of roLicenseModule))
        bolRet.Value = Nothing

        Try
            Dim objLic As New Support.roLicenseInfo
            If Not objLic.Load(strLicInfoFile) Then
                bolRet.Value = New Generic.List(Of roLicenseModule)
            Else
                bolRet.Value = objLic.Modules
            End If
        Catch ex As Exception
            Dim oLog As New roLog("ServerLiveLog", "GetLicenseInstalledModulesStatus")
            oLog.logMessage(roLog.EventType.roError, "wsLicense::GetLicenseInstalledModulesStatus", ex)
        End Try

        Return bolRet
    End Function

    Public Shared Function GetLicenseMaxConcurrentSessions(ByVal licinfoFile As String) As roGenericVtResponse(Of Integer)

        Dim bolRet As New roGenericVtResponse(Of Integer)
        bolRet.Value = -1

        Try
            Dim objLic As New Support.roLicenseInfo
            If objLic.Load(licinfoFile) Then
                bolRet.Value = objLic.ConcurrentSessions
            Else
                bolRet.Value = -1
            End If
        Catch ex As Exception
            Dim oLog As New roLog("ServerLiveLog", "GetLicenseInstalledModulesStatus")
            oLog.logMessage(roLog.EventType.roError, "wsLicense::GetLicenseInstalledModulesStatus", ex)
        End Try

        Return bolRet

    End Function

End Class