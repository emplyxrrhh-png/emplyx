Imports System.Security.Claims
Imports System.Web
Imports Microsoft.Owin
Imports Robotics.Base
Imports Robotics.VTBase

Public Class CommonClaim
    Public Shared Function GetClaimValue(oClaims As IEnumerable(Of Claim), claimName As String) As String
        Dim claimValue As String = String.Empty

        Try
            Dim oClaim As Claim = oClaims.First(Function(x) x.Type = claimName)

            If oClaim IsNot Nothing Then
                claimValue = oClaim.Value
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"CommonClaim::GetClaimValue::{claimName}::User identifier claim not founed")
                claimValue = String.Empty
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CommonClaim::GetAuthenticationClaim::Exception searching for claim")
            claimValue = String.Empty
        End Try

        Return claimValue
    End Function


    Public Shared Function GetAuthenticationClaim(authenticationType As String, ssoConfigVersion As Integer, owinContext As IOwinContext) As String
        Dim strUserName As String = String.Empty

        Try
            Dim oParamSSOType As String = authenticationType.Trim.ToUpper

            Dim sCookieName As String = "ExternalCookie"

            If ssoConfigVersion = 2 Then
                sCookieName = "clientscheme_cegidid"
                If Not oParamSSOType.ToUpper().StartsWith("CEGIDID") Then sCookieName = "clientscheme_" & HttpContext.Current.Session("roMultiCompanyId")
            End If

            Dim oClaims As IEnumerable(Of Claim) = owinContext.Authentication.AuthenticateAsync(sCookieName).Result.Identity.Claims

            Try
                If roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("PrintAvailableClaims")) Then
                    Dim availableClaims As String = "Claims disponibles:"
                    For Each claim As Claim In oClaims
                        availableClaims = $"{availableClaims} {claim.Type}({claim.Value}),"
                    Next

                    roLog.GetInstance().logMessage(roLog.EventType.roError, $"CommonClaim::PrintAvailableClaimsWithValues::{availableClaims}")
                End If
            Catch ex As Exception
                'do nothing
            End Try


            Try
                Dim oClaim As Claim = Nothing
                Dim claimName As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VisualTime.SSO.ClaimName"))

                Select Case oParamSSOType
                    Case "AAD", "OKTA"
                        claimName = If(String.IsNullOrEmpty(claimName), "preferred_username", claimName)
                        oClaim = oClaims.First(Function(x) String.Equals(x.Type, claimName, StringComparison.OrdinalIgnoreCase))
                    Case "ADFS", "SAML"
                        claimName = If(String.IsNullOrEmpty(claimName), "/NAMEIDENTIFIER", claimName)
                        oClaim = oClaims.First(Function(x) x.Type.EndsWith(claimName, StringComparison.OrdinalIgnoreCase))

                End Select

                If oClaim IsNot Nothing Then
                    strUserName = oClaim.Value
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, $"CommonClaim::GetAuthenticationClaim::{oParamSSOType}::User identifier claim {claimName} not found")
                    strUserName = String.Empty
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CommonClaim::GetAuthenticationClaim::Exception searching for claim")
                strUserName = String.Empty
            End Try

            If strUserName Is Nothing Then strUserName = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CommonClaim::GetAuthenticationClaim::", ex)
            strUserName = ""
        End Try

        Return strUserName.Trim
    End Function

End Class