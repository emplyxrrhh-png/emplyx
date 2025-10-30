Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_BreakStart_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.BreakStart, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, True, True, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Employees WHERE ID = " & _oNotificationTask.Key1Numeric)),
                Format$(_oNotificationTask.key3Datetime, "HH:mm")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.BreakStart.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.BreakStart.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBreakStart::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            'Obtenemos todos los ids de los shifts de hoy y ayer sin repetidos que tengan descansos con la notificación activa
            'y que algun employee con estos shifts haya tenido un fichaje de entrada
            SQL = "@SELECT# distinct(IDShiftUsed), ds.IDEmployee, ds.Date, ds.LayersDefinition, Shifts.StartLimit  " &
                        "FROM DailySchedule ds " &
                        "INNER JOIN sysroShiftsLayers sl on sl.IDShift = ds.IDShiftUsed " &
                        "INNER JOIN punches p on p.IDEmployee = ds.IDEmployee " &
                        "INNER JOIN shifts on shifts.id = ds.IDShiftUsed " &
                        "WHERE Date >=@Date " &
                        "AND sl.IDType = 1200 " &
                        "AND p.ActualType = 1 " &
                        "AND p.DateTime >=@Date " &
                        "AND sl.Definition Like '%<Item key=""NotificationForUser"" type=""8"">true</Item>%' ORDER BY Date ASC"

            SQL = Strings.Replace(SQL, "@Date", roTypes.Any2Time(DateTime.Now.Date).Add(-1, "d").SQLSmallDateTime)

            Dim tZoneInfo As TimeZoneInfo = Nothing
            Dim dtShifts As DataTable = AccessHelper.CreateDataTable(SQL)
            If dtShifts IsNot Nothing AndAlso dtShifts.Rows.Count > 0 Then
                For Each drShift As DataRow In dtShifts.Rows 'Por cada shift
                    tZoneInfo = Nothing


                    Dim oShiftState As New Shift.roShiftState
                    Dim oShift As Shift.roShift = New Shift.roShift(drShift("IDShiftUsed"), oShiftState)
                    Dim dShiftDate As Date = roTypes.Any2DateTime(drShift("Date"))

                    For Each shiftlayer As Shift.roShiftLayer In oShift.Layers
                        ' Sólo trato los descansos
                        If shiftlayer.LayerType = roLayerTypes.roLTBreak Then

                            Dim oData = New roCollection(shiftlayer.DataStoredXML)
                            Dim notifyEmployee As Boolean = roTypes.Any2Boolean(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.NotificationForUser)))

                            If notifyEmployee Then
                                ' Recuperamos la información del descanso
                                Dim beginBreakDate As Date = roTypes.Any2DateTime(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.Begin)))
                                Dim finishBreakDate As Date = roTypes.Any2DateTime(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.Finish)))
                                Dim oLayersDefinition As New roCollection(roTypes.Any2String(drShift("LayersDefinition")))

                                ' Tiempos de cortesía
                                Dim courtesyTimeBefore As TimeSpan = TimeSpan.Parse(roTypes.Any2String(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.NotificationForUserBeforeTime))))
                                'Dim courtesyTimeAfter As TimeSpan = TimeSpan.Parse(roTypes.Any2String(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.NotificationForUserAfterTime))))

                                'Si existe el LayerDefinition es que es un descanso con tiempos relativos (In Nil we trust)
                                If oLayersDefinition.Exists("TotalLayers") Then
                                    'Aunque haya más franjas rígidas, las horas de los descansos siempre se aplican desde el inicio de la primera franja
                                    If oLayersDefinition.Exists("LayerFloatingBeginTime_1") Then
                                        'La hora inicial correcta está en el dailyshcedule
                                        Dim beginMandatoryLayer As DateTime = roTypes.Any2DateTime(oLayersDefinition("LayerFloatingBeginTime_1"))

                                        'Solo modificamos la hora inicial y final del descanso en caso que el layer tenga las propiedades RealBegin y RealFinish
                                        'En ese caso el Begin y el Finish seran horas relativas y no absolutas
                                        If oData.Exists(shiftlayer.XmlKey(roXmlLayerKeys.RealBegin)) Then
                                            'Para conseguir la hora inicial del descanso le añadimos la hora incial del descanso relativa a la hora inicial del dailyschedule
                                            beginBreakDate = beginMandatoryLayer
                                            beginBreakDate = beginBreakDate.Add(roTypes.Any2DateTime(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.Begin))).TimeOfDay)
                                        End If

                                        If oData.Exists(shiftlayer.XmlKey(roXmlLayerKeys.RealFinish)) Then
                                            'Para conseguir la hora final del descanso le añadimos la hora final del descanso relativa a la hora inicial del dailyschedule
                                            finishBreakDate = beginMandatoryLayer
                                            finishBreakDate = finishBreakDate.Add(roTypes.Any2DateTime(oData.Item(shiftlayer.XmlKey(roXmlLayerKeys.Finish))).TimeOfDay)
                                        End If
                                    End If
                                End If

                                ' Hora teórica de inicio del horario
                                Dim workStartTime As Date = oShift.StartLimit

                                ' Referenciamos todas las horas al día del horario
                                Dim beginRealBreakDate As Date
                                Dim beginBreakTimeToNotify As Date
                                Dim finishRealBreakDate As Date
                                Dim nowTime As Date = DateTime.Now

                                beginRealBreakDate = roShift.GetLocalizedShiftTime(beginBreakDate, dShiftDate)
                                finishRealBreakDate = roShift.GetLocalizedShiftTime(finishBreakDate, dShiftDate)
                                workStartTime = roShift.GetLocalizedShiftTime(workStartTime, dShiftDate)
                                beginBreakTimeToNotify = beginRealBreakDate

                                Dim beginRealBreakDateWithTimeZone As Date
                                Dim finishRealBreakDateWithTimeZone As Date
                                beginRealBreakDateWithTimeZone = roNotificationHelper.GetServerTime(roTypes.Any2Integer(drShift("IDEmployee")), beginRealBreakDate, tZoneInfo)
                                finishRealBreakDateWithTimeZone = roNotificationHelper.GetServerTime(roTypes.Any2Integer(drShift("IDEmployee")), finishRealBreakDate, tZoneInfo)

                                Dim breakStarted As Boolean = beginRealBreakDateWithTimeZone.Add(courtesyTimeBefore) <= nowTime
                                'Dim breakFinished As Boolean = finishRealBreakDate.Add(courtesyTimeAfter) <= nowTime

                                SQL = "@SELECT# count(id) FROM sysroNotificationTasks WHERE IDNotification=@idNotification AND Key1Numeric=@idEmployee AND Key3DateTime=@beginBreakDate"
                                SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(drShift("IDEmployee")))
                                SQL = Strings.Replace(SQL, "@beginBreakDate", roTypes.Any2Time(beginBreakTimeToNotify).SQLDateTime)

                                Dim notificationDoesNotExists As Boolean = AccessHelper.ExecuteScalar(SQL) = 0

                                ' Sólo notifico si no lo hice antes, y el descanso ya ha empezado (y transcurrido la cortesía de inicio) y no ha terminado
                                If notificationDoesNotExists AndAlso breakStarted Then 'AndAlso Not breakFinished Then
                                    ' Fichajes a considerar ...
                                    SQL = "@SELECT# DateTime, ActualType " &
                                                "FROM Punches " &
                                                "WHERE IDEmployee=@idEmployee " &
                                                "AND ActualType in(1, 2) " &
                                                "AND ShiftDate>=@Date " &
                                                "ORDER BY DateTime asc"

                                    SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(drShift("IDEmployee")))
                                    SQL = Strings.Replace(SQL, "@Date", roTypes.Any2Time(DateTime.Now.Date).Add(-1, "d").SQLSmallDateTime)

                                    Dim dtPunches As DataTable = AccessHelper.CreateDataTable(SQL)
                                    bRet = roNotificationHelper.CreateBreakNotification(dtPunches, courtesyTimeBefore, beginRealBreakDate, beginRealBreakDate, eNotificationType.BreakStart, roTypes.Any2Integer(drShift("IDEmployee")), roTypes.Any2Integer(ads("ID")), beginBreakTimeToNotify, workStartTime)
                                End If
                            End If
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBreakStart::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class