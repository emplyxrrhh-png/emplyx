Imports System.DirectoryServices

Namespace Base



    Public Class AuthenticateAD

        Private _filterAttribute As String
        '

        Public Function AuthenticateByActiveDirectory(ByRef oState As roSecurityState, ByVal Domain As String, ByVal UserName As String, ByVal Password As String) As Boolean

            Dim bRet As Boolean = False

            Try
                Dim strPath As String = String.Empty
                Dim dtConf As DataTable = DataLayer.AccessHelper.CreateDataTable("@SELECT# Value FROM sysroLiveAdvancedParameters WHERE ParameterName='VTLive.AD.URL'")
                If dtConf IsNot Nothing AndAlso dtConf.Rows.Count = 1 AndAlso Robotics.VTBase.roTypes.Any2String(dtConf.Rows(0)("Value")) <> String.Empty Then
                    strPath = VTBase.roTypes.Any2String(dtConf.Rows(0)("Value"))
                Else
                    strPath = "LDAP://" + Domain
                End If

                If Me.IsAuthenticated(oState, strPath, Domain, UserName, Password) Then
                    bRet = True
                End If
            Catch ex As Exception
                bRet = False
                oState.UpdateStateInfo(ex, "AuthenticateAD::AuthenticateByActiveDirectory")
            End Try

            Return bRet

        End Function

        Private Function IsAuthenticated(ByRef oState As roSecurityState, ByVal strPath As String, ByVal domain As String, ByVal username As String, ByVal pwd As String) As Boolean
            Dim bRet As Boolean = False
            Dim domainAndUsername As String = domain & "\" & username
            Dim entry As DirectoryEntry = New DirectoryEntry(strPath, domainAndUsername, pwd)

            Try

                'Bind to the native AdsObject to force authentication.
                Dim obj As Object = entry.NativeObject

                Dim search As DirectorySearcher = New DirectorySearcher(entry)

                search.Filter = "(SAMAccountName=" & username & ")"
                search.PropertiesToLoad.Add("cn")
                Dim result As SearchResult = search.FindOne()

                If result IsNot Nothing Then
                    Return True
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "AuthenticateAD::IsAuthenticated")
            End Try

            Return bRet

        End Function

    End Class

End Namespace