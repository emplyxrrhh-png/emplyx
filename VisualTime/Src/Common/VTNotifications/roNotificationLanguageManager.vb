Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Notifications

    Public Class roNotificationLanguageManager

#Region "Declarations - Constructor"

        Private oState As roNotificationState

        Private eNotificationType As eNotificationType
        Private iScenarioID As Integer
        Private eScope As MessageScope

        Public Sub New()
            Me.oState = New roNotificationState

        End Sub

#End Region

        ''' <summary>
        ''' Carga plantilla de documento
        ''' </summary>
        ''' <param name="notificationType"></param>
        ''' <param name="strLanguageKey"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadNotificationLanguage(ByVal notificationType As eNotificationType, ByVal strLanguageKey As String, Optional ByVal bAudit As Boolean = False) As roNotificationLanguage
            Dim bolRet As roNotificationLanguage = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError

                bolRet = New roNotificationLanguage
                bolRet.NotificationType = notificationType
                bolRet.LanguageKey = strLanguageKey

                Dim strSQL As String = "@SELECT# * FROM NotificationMessageParameters inner join sysroNotificationTypes on IDNotificationType = sysroNotificationTypes.ID WHERE IDNotificationType = " & CInt(notificationType).ToString & " order by Scenario, Scope"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                bolRet.Scenarios = LoadScenariosFromStorage(strLanguageKey, tb).ToArray
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::LoadNotificationLanguage")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::LoadNotificationLanguage")
            End Try

            Return bolRet
        End Function

        Private Function LoadScenariosFromStorage(strLanguageKey As String, tb As DataTable) As List(Of NotificationLanguageScenario)
            Dim oRet As New List(Of NotificationLanguageScenario)
            Try
                Dim iCurrentScenario As Integer = -1
                Dim iNextScenario As Integer = -1

                Dim customFileBytes As Byte() = Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer." & strLanguageKey.ToUpper & ".CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)

                Dim oDefaultLanguage = New roLanguage()
                oDefaultLanguage.SetLanguageReference("ProcessNotificationServer", strLanguageKey)

                Dim oLanguage = New roLanguageLocal()
                oLanguage.SetLanguageReference("ProcessNotificationServer", strLanguageKey)
                oLanguage.LoadFromByteArray(customFileBytes)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim currentScenario As NotificationLanguageScenario = Nothing
                    For Each oRow In tb.Rows
                        iNextScenario = roTypes.Any2Integer(oRow("Scenario"))

                        If iCurrentScenario <> iNextScenario OrElse iCurrentScenario = -1 Then
                            If iCurrentScenario <> -1 AndAlso currentScenario IsNot Nothing Then
                                oRet.Add(currentScenario)
                            End If

                            currentScenario = New NotificationLanguageScenario
                            currentScenario.IDScenario = iNextScenario
                            currentScenario.NotificationLanguageKey = roTypes.Any2String(oRow("NotificationLanguageKey"))

                            currentScenario.Name = oDefaultLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & ".Scenario", "Notification")

                            Dim scopeRows As DataRow() = tb.Select("Scenario= " & iNextScenario & " AND Scope = '" & MessageScope.Subject.ToString() & "'")
                            If scopeRows.Length > 0 Then
                                currentScenario.Subject = oLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Subject", "Notification")

                                If currentScenario.Subject = "NotFound" Then
                                    currentScenario.Subject = oDefaultLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Subject", "Notification")
                                End If

                                Dim tmpParameterList As New Generic.List(Of NotificationLanguageParam)
                                For Each oScopeRow In scopeRows

                                    Dim oParam As New NotificationLanguageParam() With {
                                        .IDParameter = roTypes.Any2Integer(oScopeRow("Parameter")),
                                        .ParameterLanguageKey = roTypes.Any2String(oScopeRow("ParameterLanguageKey")),
                                        .Name = oLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                        }
                                    If oParam.Name = "NotFound" Then
                                        oParam.Name = oDefaultLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                    End If

                                    tmpParameterList.Add(oParam)
                                Next
                                currentScenario.SubjectParameters = tmpParameterList.ToArray

                                For Each item In currentScenario.SubjectParameters
                                    currentScenario.Subject = currentScenario.Subject.Replace("${" & item.IDParameter & "}", "{" & item.Name & "}")
                                Next
                            Else
                                currentScenario.Subject = String.Empty
                                currentScenario.SubjectParameters = {}
                            End If

                            scopeRows = tb.Select("Scenario= " & iNextScenario & " AND Scope = '" & MessageScope.Body.ToString() & "'")
                            If scopeRows.Length > 0 Then
                                currentScenario.Body = oLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Body", "Notification")

                                If currentScenario.Body = "NotFound" Then
                                    currentScenario.Body = oDefaultLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Body", "Notification")
                                End If

                                Dim tmpParameterList As New Generic.List(Of NotificationLanguageParam)
                                For Each oScopeRow In scopeRows
                                    Dim oParam As New NotificationLanguageParam() With {
                                        .IDParameter = roTypes.Any2Integer(oScopeRow("Parameter")),
                                        .ParameterLanguageKey = roTypes.Any2String(oScopeRow("ParameterLanguageKey")),
                                        .Name = oLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                    }

                                    If oParam.Name = "NotFound" Then
                                        oParam.Name = oDefaultLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                    End If

                                    tmpParameterList.Add(oParam)
                                Next
                                currentScenario.BodyParameters = tmpParameterList.ToArray

                                For Each item In currentScenario.BodyParameters
                                    currentScenario.Body = currentScenario.Body.Replace("${" & item.IDParameter & "}", "{" & item.Name & "}")
                                Next
                            Else
                                currentScenario.Body = String.Empty
                            End If

                            iCurrentScenario = iNextScenario
                        End If
                    Next

                    If currentScenario IsNot Nothing Then oRet.Add(currentScenario)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::LoadScenariosFromFile")
            End Try

            Return oRet
        End Function

        Private Function LoadScenariosFromFile(strLanguageKey As String, tb As DataTable) As List(Of NotificationLanguageScenario)
            Dim oRet As New List(Of NotificationLanguageScenario)
            Try
                Dim iCurrentScenario As Integer = -1
                Dim iNextScenario As Integer = -1

                Dim oLanguage = New roLanguageLocal()
                oLanguage.SetLanguageReference("ProcessNotificationServer", strLanguageKey)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim currentScenario As NotificationLanguageScenario = Nothing
                    For Each oRow In tb.Rows
                        iNextScenario = roTypes.Any2Integer(oRow("Scenario"))

                        If iCurrentScenario <> iNextScenario OrElse iCurrentScenario = -1 Then
                            If iCurrentScenario <> -1 AndAlso currentScenario IsNot Nothing Then
                                oRet.Add(currentScenario)
                            End If

                            currentScenario = New NotificationLanguageScenario
                            currentScenario.IDScenario = iNextScenario
                            currentScenario.NotificationLanguageKey = roTypes.Any2String(oRow("NotificationLanguageKey"))

                            currentScenario.Name = oLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & ".Scenario", "Notification")

                            Dim scopeRows As DataRow() = tb.Select("Scenario= " & iNextScenario & " AND Scope = '" & MessageScope.Subject.ToString() & "'")
                            If scopeRows.Length > 0 Then
                                currentScenario.Subject = oLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Subject", "Notification")

                                Dim tmpParameterList As New Generic.List(Of NotificationLanguageParam)
                                For Each oScopeRow In scopeRows
                                    tmpParameterList.Add(New NotificationLanguageParam() With {
                                        .IDParameter = roTypes.Any2Integer(oScopeRow("Parameter")),
                                        .ParameterLanguageKey = roTypes.Any2String(oScopeRow("ParameterLanguageKey")),
                                        .Name = oLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                                         })
                                Next
                                currentScenario.SubjectParameters = tmpParameterList.ToArray

                                For Each item In currentScenario.SubjectParameters
                                    currentScenario.Subject = currentScenario.Subject.Replace("${" & item.IDParameter & "}", "{" & item.Name & "}")
                                Next
                            Else
                                currentScenario.Subject = String.Empty
                                currentScenario.SubjectParameters = {}
                            End If

                            scopeRows = tb.Select("Scenario= " & iNextScenario & " AND Scope = '" & MessageScope.Body.ToString() & "'")
                            If scopeRows.Length > 0 Then
                                currentScenario.Body = oLanguage.TranslateRawText("Notification." & currentScenario.NotificationLanguageKey & ".Body", "Notification")

                                Dim tmpParameterList As New Generic.List(Of NotificationLanguageParam)
                                For Each oScopeRow In scopeRows
                                    tmpParameterList.Add(New NotificationLanguageParam() With {
                                        .IDParameter = roTypes.Any2Integer(oScopeRow("Parameter")),
                                        .ParameterLanguageKey = roTypes.Any2String(oScopeRow("ParameterLanguageKey")),
                                        .Name = oLanguage.Translate("Notification." & currentScenario.NotificationLanguageKey & "." & oScopeRow("Scope") & ".Parameters." & oScopeRow("Parameter"), "Notification", False)
                                                         })
                                Next
                                currentScenario.BodyParameters = tmpParameterList.ToArray

                                For Each item In currentScenario.BodyParameters
                                    currentScenario.Body = currentScenario.Body.Replace("${" & item.IDParameter & "}", "{" & item.Name & "}")
                                Next
                            Else
                                currentScenario.Body = String.Empty
                            End If

                            iCurrentScenario = iNextScenario
                        End If
                    Next

                    If currentScenario IsNot Nothing Then oRet.Add(currentScenario)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::LoadScenariosFromFile")
            End Try

            Return oRet
        End Function

        Public Function LoadCustomizableNotifications() As roNotificationType()
            Dim bolRet As roNotificationType() = {}

            Try
                oState.Result = DocumentResultEnum.NoError

                Dim tmpList As New Generic.List(Of roNotificationType)

                Dim strSQL As String = "@SELECT# distinct IDNotificationType, sysroNotificationTypes.Name, sysroNotificationTypes.OnlySystem FROM NotificationMessageParameters inner join sysroNotificationTypes on IDNotificationType = sysroNotificationTypes.ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow In tb.Rows
                        tmpList.Add(New roNotificationType() With {.Id = roTypes.Any2Integer(oRow("IDNotificationType")), .Name = roTypes.Any2String(oRow("Name")), .System = roTypes.Any2Integer(oRow("OnlySystem"))})
                    Next
                End If

                bolRet = tmpList.ToArray()
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::SaveNotificationLanguage")
            End Try

            Return bolRet
        End Function

        Public Function SaveNotificationLanguage(ByVal oNotificationMessage As roNotificationLanguage, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                oState.Result = DocumentResultEnum.NoError

                bolRet = SaveScenariosToStorage(oNotificationMessage)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::SaveNotificationLanguage")
            End Try

            Return bolRet
        End Function

        Private Function SaveScenariosToStorage(oNotificationMessage As roNotificationLanguage) As Boolean
            Dim bret As Boolean = True
            Try

                Dim keysToSave As New Dictionary(Of String, String)

                For Each oScenario In oNotificationMessage.Scenarios
                    For Each item In oScenario.SubjectParameters
                        oScenario.Subject = oScenario.Subject.Replace("{" & item.Name & "}", "${" & item.IDParameter & "}")
                    Next
                    keysToSave.Add("Notification." & oScenario.NotificationLanguageKey & ".Subject.roText", oScenario.Subject)
                    For Each item In oScenario.BodyParameters
                        'Al copiar y pegar desde algo externo (notepad, excel etc), se guardan saltos de linea y se corta el texto despues
                        oScenario.Body = oScenario.Body.Replace(vbCr, "").Replace(vbLf, "")
                        oScenario.Body = oScenario.Body.Replace("{" & item.Name & "}", "${" & item.IDParameter & "}")
                    Next

                    keysToSave.Add("Notification." & oScenario.NotificationLanguageKey & ".Body.roText", oScenario.Body)
                Next

                Dim customFileBytes As Byte() = Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer." & oNotificationMessage.LanguageKey.ToUpper & ".CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)

                Dim oLanguage = New roLanguageLocal()
                customFileBytes = oLanguage.ProcessChanges(customFileBytes, keysToSave)

                Robotics.Azure.RoAzureSupport.SaveFileOnAzure(customFileBytes, "customFiles/ProcessNotificationServer." & oNotificationMessage.LanguageKey.ToUpper & ".CUST.LNG", roLiveQueueTypes.documents)
            Catch ex As Exception
                bret = False
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::SaveScenariosToFile")
            End Try

            Return bret
        End Function

        Private Function SaveScenariosToFile(oNotificationMessage As roNotificationLanguage) As Boolean
            Dim bret As Boolean = True
            Try
                Dim oLanguage = New roLanguageLocal()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oNotificationMessage.LanguageKey & ".CUST")

                For Each oScenario In oNotificationMessage.Scenarios
                    For Each item In oScenario.SubjectParameters
                        oScenario.Subject = oScenario.Subject.Replace("{" & item.Name & "}", "${" & item.IDParameter & "}")
                    Next

                    oLanguage.SaveLanguage("ProcessNotificationServer", oNotificationMessage.LanguageKey & ".CUST", "Notification." & oScenario.NotificationLanguageKey & ".Subject.roText", oScenario.Subject)

                    For Each item In oScenario.BodyParameters
                        oScenario.Body = oScenario.Body.Replace(vbCr, "").Replace(vbLf, "")
                        oScenario.Body = oScenario.Body.Replace("{" & item.Name & "}", "${" & item.IDParameter & "}")
                    Next

                    oLanguage.SaveLanguage("ProcessNotificationServer", oNotificationMessage.LanguageKey & ".CUST", "Notification." & oScenario.NotificationLanguageKey & ".Body.roText", oScenario.Body)
                Next
            Catch ex As Exception
                bret = False
                oState.UpdateStateInfo(ex, "roNotificationLanguageManager::SaveScenariosToFile")
            End Try

            Return bret
        End Function

    End Class

End Namespace