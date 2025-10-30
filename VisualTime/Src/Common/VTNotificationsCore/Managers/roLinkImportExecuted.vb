Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.VTBase

Public Class roNotificationTask_LinkImportExecuted_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.LinkImportExecuted, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return False
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, False, False, False)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem

        Dim strImportGuide As String = String.Empty
        Select Case _oNotificationTask.Key1Numeric
            Case 1
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportEmployees.Name", Nothing, "DataLinkService", oConf.Language)
            Case 2
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportScheduler.Name", Nothing, "DataLinkService", oConf.Language)
            Case 3
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportTeoricCoverage.Name", Nothing, "DataLinkService", oConf.Language)
            Case 4
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportEnterprises.Name", Nothing, "DataLinkService", oConf.Language)
            Case 5
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportTasks.Name", Nothing, "DataLinkService", oConf.Language)
            Case 6
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportAssignments.Name", Nothing, "DataLinkService", oConf.Language)
            Case 7
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportHolidays.Name", Nothing, "DataLinkService", oConf.Language)
            Case 8
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportProgrammedCause.Name", Nothing, "DataLinkService", oConf.Language)
            Case 9
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportProgrammedAbsence.Name", Nothing, "DataLinkService", oConf.Language)
            Case 10
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportProgrammedAbsence.Name", Nothing, "DataLinkService", oConf.Language)
            Case 11
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportDailyCauses.Name", Nothing, "DataLinkService", oConf.Language)
            Case 12
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportWorkSheets.Name", Nothing, "DataLinkService", oConf.Language)
            Case 13
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportBusinessCenterName", Nothing, "DataLinkService", oConf.Language)
            Case 14
                strImportGuide = roNotificationHelper.Message("ImportGuides.ImportPlannigV2.Name", Nothing, "DataLinkService", oConf.Language)
        End Select

        Try
            Dim Params As New ArrayList From {
                strImportGuide,
                roTypes.Any2String(_oNotificationTask.Parameters).Replace(vbCrLf, "<br>")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.DataImportExecuted.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.DataImportExecuted.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email

            ' ESPECIAL APV: Mail con correcciones necesarias para importación de ausencias prolongadas
            If sCustomizationCode = "VPA" Then
                If roTypes.Any2String(_oNotificationTask.NotificationName).ToUpper.EndsWith("(ADV=IMPAUS)") AndAlso True Then
                    If roTypes.Any2Integer(_oNotificationTask.Key1Numeric) = 10 AndAlso Not IsDBNull(_oNotificationTask.Parameters) Then
                        oItem.Subject &= roNotificationHelper.Message("Notification.DataImportExecuted.VAPAutomaticCorrection", Nothing, "DataLinkService", oConf.Language) ', "", ", con incidencias resueltas automáticamente")
                        oItem.Body = roTypes.Any2String(_oNotificationTask.Parameters)
                    End If
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roLinkImportExecuted::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class