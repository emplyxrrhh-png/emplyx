Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Robotics.VTBase

Public Class CommonAuth

    Public Shared Function RedirectToLoginIfNecesary(companyCode As String, authenticationType As String, owinContext As IOwinContext, returnURI As String) As Boolean
        Dim bolRet As Boolean = True

        Try
            authenticationType = authenticationType.Trim.ToUpper

            If authenticationType.Equals("AAD") Then
                owinContext.Authentication.Challenge(
                    New AuthenticationProperties With {.RedirectUri = returnURI},
                    ("clientscheme_" & companyCode.Trim.ToLower)
                )
            ElseIf authenticationType.Equals("OKTA") Then
                owinContext.Authentication.Challenge(
                    New AuthenticationProperties With {.RedirectUri = returnURI},
                    ("clientscheme_" & companyCode.Trim.ToLower)
                )
            ElseIf authenticationType.Equals("ADFS") Then
                owinContext.Authentication.Challenge(
                    New AuthenticationProperties With {.RedirectUri = returnURI},
                    ("clientscheme_" & companyCode.Trim.ToLower)
                )
            ElseIf authenticationType.Equals("SAML") Then
                owinContext.Authentication.Challenge(
                    New AuthenticationProperties With {.RedirectUri = returnURI},
                    ("clientscheme_" & companyCode.Trim.ToLower)
                )
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CommonClaim::RedirectToLoginIfNecesary::", ex)
            bolRet = False
        End Try

        Return bolRet
    End Function

End Class