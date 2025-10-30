Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Productive_Unit_Under_Coverage_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Productive_Unit_Under_Coverage, sGUID)
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
        Return {}
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Return Nothing
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Cobertura insuficiente en unidad productiva
        '
        Dim SQL As String
        Dim sFiredDate As String
        Dim mCondition As New roCollection
        Dim bolCovert As Boolean
        Dim dtPosition As DataTable
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            ' Revisamos la planificaci�n de las unidades productivas X dias en adelante
            SQL = "@SELECT# ID, IDNode, Date, IDProductiveUnit FROM DailyBudgets WHERE Date >= " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(DateTime.Now.Date).Add(roTypes.Any2Double(mCondition("MaxDays"))).SQLSmallDateTime &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = DailyBudgets.ID " &
                                    " AND DailyBudgets.Date =  sysroNotificationTasks.Key3Datetime " &
                                    " AND DailyBudgets.IDNode = sysroNotificationTasks.Key2Numeric " &
                                    " AND DailyBudgets.IDProductiveUnit = sysroNotificationTasks.Key5Numeric " &
                                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") Order by DailyBudgets.IDNode,DailyBudgets.IDProductiveUnit,DailyBudgets.Date "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    ' Obtenemos las posiciones del dia
                    ' Para cada posicion, obtenemos los empleados asignados que no tengan ausencia prolongada
                    SQL = "@SELECT# p.Quantity, (@SELECT# COUNT(*) AS TotalEmployees FROM DailySchedule WHERE IDDailyBudgetPosition=p.IDDailyBudget AND NOT EXISTS (@SELECT# * FROM ProgrammedAbsences WHERE ProgrammedAbsences.idEmployee = DailySchedule.IDEmployee AND BeginDate <= DailySchedule.Date and  CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END >= DailySchedule.Date ) ) As TotalEmployees FROM DailyBudget_Positions p WHERE p.IDDailyBudget =" & dr("ID")

                    bolCovert = True
                    dtPosition = AccessHelper.CreateDataTable(SQL)
                    If dtPosition IsNot Nothing AndAlso dtPosition.Rows.Count > 0 Then
                        For Each drPos As DataRow In dtPosition.Rows
                            If roTypes.Any2Double(drPos("Quantity")) > roTypes.Any2Double(drPos("TotalEmployees")) Then bolCovert = False
                        Next
                    End If

                    If Not bolCovert Then
                        If roTypes.Any2Boolean(ads("ShowOnDesktop")) = True Then
                            sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                        Else
                            sFiredDate = "NULL"
                        End If

                        SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric, Key5Numeric, FiredDate) VALUES (" &
                                  roTypes.Any2Double(ads("ID")) & "," & dr("ID") & "," & roTypes.Any2Time(dr("Date")).SQLDateTime & "," & roTypes.Any2Double(dr("IDNode")) & "," & dr("IDProductiveUnit") & ", " & sFiredDate & ")"
                        bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                    End If
                Next
            End If

            ' Eliminamos alertas pasadas en caso que existan
            SQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification=" & roTypes.Any2Double(ads("ID")) & " AND Key3DateTime < " & roTypes.Any2Time(DateTime.Now.Date).SQLDateTime
            If AccessHelper.ExecuteSql(SQL) = False Then
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCore::roProductiveUnitUnderCoverage::GenerateNotificationTasks:: @DELETE# Statement failed.")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roProductiveUnitUnderCoverage::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class