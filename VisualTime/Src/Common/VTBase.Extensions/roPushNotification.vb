Imports Robotics.Base.DTOs

Public Class roPushNotification

    Public Shared Function SendNotificationPushToPassport(idUser As Long, ByVal passportType As LoadType, title As String, content As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oNotificationItem As roNotificationItem = Nothing

            Dim strSql As String = ""

            If passportType = LoadType.Passport Then
                strSql = "@SELECT# Token from sysroPassports_DeviceTokens WHERE IdPassport = " & idUser 'where DateADD(day,5,RegistrationDate) > getdate()"
            ElseIf passportType = LoadType.Employee Then
                strSql = "@SELECT# Token from sysroPassports_DeviceTokens WHERE (@SELECT# COUNT(*) FROM EMPLOYEECONTRACTS EC WHERE EC.IDEMPLOYEE = " & idUser & " AND (EC.BEGINDATE <= GETDATE() AND (EC.ENDDATE IS NULL OR EC.ENDDATE > GETDATE()))) > 0 AND IdPassport =  (@SELECT# ID from sysropassports where IDEmployee = " & idUser & " ) "  'where DateADD(day,5,RegistrationDate) > getdate()"
            End If

            Dim dtDevices As DataTable = DataLayer.AccessHelper.CreateDataTable(strSql)

            If dtDevices IsNot Nothing AndAlso dtDevices.Rows.Count > 0 Then
                For Each oRow As DataRow In dtDevices.Rows

                    oNotificationItem = New roNotificationItem() With {
                            .Type = NotificationItemType.push,
                            .Content = String.Empty,
                            .Body = content,
                            .Destination = roTypes.Any2String(oRow("Token")),
                            .Subject = title,
                            .CompanyId = Azure.RoAzureSupport.GetCompanyName()
                        }

                    bRet = (bRet AndAlso Azure.RoAzureSupport.SendTaskToQueue(1, Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.SendPushNotification, VTBase.roJSONHelper.SerializeNewtonSoft(oNotificationItem)))
                Next
            End If
        Catch ex As Exception
            bRet = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::SendNotificationPushToPassport:", ex)
        End Try

        Return bRet
    End Function

End Class