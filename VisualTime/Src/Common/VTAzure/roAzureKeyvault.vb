Imports System.Drawing
Imports System.Linq
Imports System.Runtime.Caching
Imports Azure
Imports Azure.Identity
Imports Azure.Security
Imports Azure.Security.KeyVault.Secrets
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Enum roKeyvaultParameter
    roVisualtimePGPPassphrase
    roVisualtimeDBUsername
    roVisualtimeDBPassword
End Enum

Public Class roAzureKeyvault

    Private Shared memoryCache As MemoryCache = MemoryCache.Default
    Private Shared lockobject As New Object

    Sub New()

    End Sub


    Public Shared Sub RebootCache()
        SyncLock lockobject

            Dim items As Array
            items = System.Enum.GetNames(GetType(roKeyvaultParameter))
            Dim item As String
            For Each item In items
                If memoryCache.Contains("keyvault_" & item) Then memoryCache.Remove("keyvault_" & item)
            Next

        End SyncLock
    End Sub

    Public Function GeKeyVaultKey(ByVal eParameter As roKeyvaultParameter) As roAzureConfig

        Dim oRet As roAzureConfig = Nothing
        Try

            If memoryCache.Contains("keyvault_" & eParameter.ToString()) Then
                oRet = CType(memoryCache("keyvault_" & eParameter.ToString()), roAzureConfig)
            Else
                Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.keyvalut)
                If oConfigValue.value <> String.Empty Then
                    Dim oKeyValutConfig As roKeyVaultConfig = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigValue.value, GetType(roKeyVaultConfig))
                    If oKeyValutConfig.uri <> String.Empty Then
                        Dim secretClient As New SecretClient(New Uri(oKeyValutConfig.uri), New DefaultAzureCredential())
                        Dim secretValue As NullableResponse(Of KeyVaultSecret) = Nothing
                        Try
                            secretValue = secretClient.GetSecret(eParameter.ToString())
                            If secretValue.HasValue Then
                                oRet = New roAzureConfig() With {
                                    .id = eParameter.ToString(),
                                    .value = secretValue.Value.Value
                                }

                                memoryCache.Set("keyvault_" & eParameter.ToString, oRet, DateTimeOffset.Now.AddDays(1))
                            End If
                        Catch keyVaultEx As RequestFailedException
                            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "roAzureKeyvault::GeKeyVaultKey::Key not found", keyVaultEx)
                        End Try
                    End If
                End If
            End If

        Catch ex As Exception
            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "roAzureKeyvault::GeKeyVaultKey::", ex)
        End Try

        Return oRet

    End Function


    Public Function GeCompanyDexKey(ByVal companyID As String, ByVal bInitNewKey As Boolean) As String

        Dim oRet As String = String.Empty

        Try
            Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.keyvalut)

            If oConfigValue.value <> String.Empty Then

                Dim oKeyValutConfig As roKeyVaultConfig = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigValue.value, GetType(roKeyVaultConfig))
                If oKeyValutConfig.uri <> String.Empty Then
                    Dim secretClient As New SecretClient(New Uri(oKeyValutConfig.uri), New DefaultAzureCredential())
                    Dim secretValue As NullableResponse(Of KeyVaultSecret) = Nothing
                    Try
                        secretValue = secretClient.GetSecret(companyID)
                        If secretValue.HasValue Then
                            oRet = secretValue.Value.Value
                        End If
                    Catch keyVaultEx As RequestFailedException
                        If bInitNewKey AndAlso keyVaultEx.ErrorCode = "SecretNotFound" Then
                            oRet = HashCheckSum.CalculateString(Guid.NewGuid().ToString(), Algorithm.SHA256)
                            secretClient.SetSecret(New KeyVaultSecret(companyID, oRet))
                        End If
                    End Try
                End If

            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureKeyvault::GetCompanySecret::", ex)
            oRet = String.Empty
        End Try

        Return oRet

    End Function

End Class