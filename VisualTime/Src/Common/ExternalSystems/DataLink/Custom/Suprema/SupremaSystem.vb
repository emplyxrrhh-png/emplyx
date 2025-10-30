Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTUserFields
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports Robotics.ExternalSystems.CTAIMA

Namespace Suprema

    Public Class SupremaSystem
        '
        ' Clase con funciones de propósito general de ValidateID
        '

        Private configParameters As SupremaConfigurationParameters = New SupremaConfigurationParameters
        Private isSupremaSystem As Boolean = False
        Private token As String = ""
        Private punchEventCodes As List(Of Integer)

        Public ReadOnly Property ConfigurationParameters(asCopy As Boolean) As SupremaConfigurationParameters
            Get
                Dim supremaConfig As SupremaConfigurationParameters
                If Not asCopy Then
                    supremaConfig = configParameters
                Else
                    supremaConfig = DeepClone(configParameters)
                    supremaConfig.Password = String.Empty
                End If
                Return supremaConfig
            End Get
        End Property

        Public ReadOnly Property IsEnabled As Boolean
            Get
                Return isSupremaSystem
            End Get
        End Property

        Public ReadOnly Property SupremaPunchEventCodes As List(Of Integer)
            Get
                Return punchEventCodes
            End Get
        End Property

        Public Sub New()
            Try
                Dim oParam As New AdvancedParameter.roAdvancedParameter("VisualTime.Link.Suprema.Enabled", New AdvancedParameter.roAdvancedParameterState())

                If roTypes.Any2Boolean(oParam.Value) Then
                    ' Cargamos la configuración
                    LoadConfigurationParameters()

                    ' Si la configuración es correcta, arrancamos el sistema
                    If configParameters.URL <> String.Empty AndAlso configParameters.Username <> String.Empty AndAlso configParameters.Password <> String.Empty AndAlso configParameters.EmployeeUserfieldId > 0 AndAlso configParameters.StartDate <> String.Empty Then
                        Net.ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf MyCertHandler)
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::Started successfully.")

                        ' Cargamos los códigos asociados a fichajes
                        punchEventCodes = GetSupremaEventCodes()

                        isSupremaSystem = True
                    Else
                        isSupremaSystem = False
                    End If

                    configParameters.IsActive = isSupremaSystem

                    configParameters.LastRun = Extensions.roParameters.GetDateParameter("SupremaLastRun")
                Else
                    isSupremaSystem = False
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::New :", ex)
            End Try

        End Sub

        Public Sub LoadConfigurationParameters()
            Dim sql As String = String.Empty

            sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.UserName'"
            configParameters.Username = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))

            sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.Password'"
            configParameters.Password = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))

            configParameters.HasPassword = (configParameters.Password.Trim.Length > 0)

            sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.URL'"
            configParameters.URL = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))

            Dim employeeUserField As String = ""
            If employeeUserField.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.EmployeeField'"
                employeeUserField = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))

                ' Si es una cadena de texto, sustrituimos por el ID del campo
                If employeeUserField.Length > 0 AndAlso Not IsNumeric(employeeUserField) Then
                    Dim oUserField As roUserField = New roUserField(New roUserFieldState(-1), employeeUserField.Trim, UserFieldsTypes.Types.EmployeeField, False, False)
                    If oUserField.Id <> -1 Then
                        configParameters.EmployeeUserfieldId = oUserField.Id
                        Dim advancedParameter As roAdvancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.EmployeeField", New AdvancedParameter.roAdvancedParameterState(-1))
                        advancedParameter.Value = oUserField.Id.ToString
                        advancedParameter.Save()
                    End If
                Else
                    configParameters.EmployeeUserfieldId = roTypes.Any2Integer(employeeUserField)
                End If
            End If

            sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.InitialLinkDate'"
            configParameters.StartDate = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))

            Dim tmpVal As String() = {}
            sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.CheckPeriod'"
            tmpVal = roTypes.Any2String(AccessHelper.ExecuteScalar(sql)).Split("@")

            If tmpVal.Length = 2 Then
                Dim strHours = tmpVal(0).Split(":")
                If strHours.Length = 2 Then
                    configParameters.DailyInitialTime = New DateTime(1900, 1, 1, roTypes.Any2Integer(tmpVal(0).Split(":")(0)), roTypes.Any2Integer(tmpVal(0).Split(":")(1)), 0, DateTimeKind.Local)
                Else
                    configParameters.DailyInitialTime = New DateTime(1900, 1, 1, 0, 10, 0, DateTimeKind.Local)
                End If
                configParameters.CheckPeriod = roTypes.Any2Integer(tmpVal(1))
                If configParameters.CheckPeriod <= 0 Then configParameters.CheckPeriod = 5
            Else
                configParameters.DailyInitialTime = New DateTime(1900, 1, 1, 0, 10, 0, DateTimeKind.Local)
                configParameters.CheckPeriod = 5
            End If

            configParameters.Timestamp = Now
        End Sub

        Private Function MyCertHandler(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
            Return True
        End Function

        Private Function Login() As Boolean
            Dim bRet As Boolean = True

            Try

                Dim Client As HttpClient = New HttpClient()
                Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")

                Client.BaseAddress = New Uri(configParameters.URL)
                Client.Timeout = New TimeSpan(0, 0, 100)

                Dim json = JsonConvert.SerializeObject(New Suprema.LoginRequest() With {.User = New Suprema.User() With {.login_id = configParameters.Username, .password = configParameters.Password}})
                Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
                ' Subimos el documento
                Dim response = Client.PostAsync("api/login", content).Result

                If response.IsSuccessStatusCode Then
                    Dim OB As Object
                    OB = JsonConvert.DeserializeObject(Of Suprema.LoginResponse)(response.Content.ReadAsStringAsync().Result)

                    For Each oHeader In response.Headers
                        If oHeader.Key = "bs-session-id" Then
                            token = roTypes.Any2String(oHeader.Value(0))
                            Exit For
                        End If
                    Next
                Else
                    bRet = False
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::Login:: Bad Request: Response: " & response.ReasonPhrase)
                End If
            Catch ex As Exception
                bRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::Login :", ex)
            End Try

            Return bRet

        End Function

        Private Function ProcessSupremaDevicesEvents() As Boolean
            Dim processSuccessful As Boolean = True
            Dim updateStamp As Boolean = False
            Try

                Dim oTerminalList As New Robotics.Base.VTBusiness.Terminal.roTerminalList

                Dim dtTerminals As DataTable = oTerminalList.GetTerminals("Type='Suprema'")

                Dim Client As HttpClient = New HttpClient()
                Client.BaseAddress = New Uri(configParameters.URL)
                Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
                Client.DefaultRequestHeaders.Add("bs-session-id", token)
                Client.Timeout = New TimeSpan(0, 0, 100)

                If dtTerminals.Rows.Count = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::No Suprema terminal configured. Cannot import punches!. Bye ... ")
                    Return True
                End If

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Start processing punches comming from Suprema devices")

                For Each oTerminal As DataRow In dtTerminals.Rows
                    Dim cTerminal As roTerminal = Nothing

                    For Each tmpTerminal As roTerminal In oTerminalList.Terminals
                        If tmpTerminal.ID = roTypes.Any2Integer(oTerminal("Id")) Then
                            cTerminal = tmpTerminal
                            Exit For
                        End If
                    Next

                    If cTerminal IsNot Nothing Then
                        Dim initialConnectionStatus As Boolean = (cTerminal.LastStatus = "Ok")
                        Dim deviceConnected As Boolean = (cTerminal.LastStatus = "Ok")

                        Dim checkDatetime As String = configParameters.StartDate
                        If Not IsDBNull(oTerminal("PunchStamp")) AndAlso roTypes.Any2String(oTerminal("PunchStamp")) <> String.Empty Then
                            Try
                                checkDatetime = roTypes.Any2String(oTerminal("PunchStamp"))
                            Catch ex As Exception
                                checkDatetime = configParameters.StartDate
                            End Try
                        End If

                        Dim eventCodesArray As String() = punchEventCodes.Select(Function(code) code.ToString()).ToArray()
                        Dim oCond As Suprema.condition() = {
                            New Suprema.condition() With {.column = "datetime", .[operator] = Suprema.AvailableOperators.Greater, .values = {checkDatetime}},
                            New Suprema.condition() With {.column = "event_type_id.code", .[operator] = Suprema.AvailableOperators.Contains, .values = eventCodesArray},
                            New Suprema.condition() With {.column = "device_id.id", .[operator] = Suprema.AvailableOperators.Equal, .values = {oTerminal("SerialNumber")}}
                        }

                        Dim oOrders As Suprema.order() = {New Suprema.order() With {.column = "datetime", .descending = False}}

                        Dim json = JsonConvert.SerializeObject(New Suprema.EventsRequest() With {.Query = New Suprema.Query() With {.limit = 1000, .conditions = oCond, .orders = oOrders}})
                        Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
                        ' Subimos el documento
                        Dim response = Client.PostAsync("/api/events/search", content).Result

                        If response.IsSuccessStatusCode Then
                            Dim oResult As String = response.Content.ReadAsStringAsync().Result
                            Dim OB As Suprema.SearchResponse = JsonConvert.DeserializeObject(Of Suprema.SearchResponse)(oResult)

                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Asking Suprema API for punches from terminal " & cTerminal.Description & " after " & checkDatetime)

                            If OB.EventCollection IsNot Nothing AndAlso OB.EventCollection.rows IsNot Nothing Then
                                If OB.EventCollection.rows.Any() Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Start processing " & OB.EventCollection.rows.Count.ToString & " punches")

                                    For Each row As Suprema.punch In OB.EventCollection.rows
                                        If row.event_type_id.code = "12800" OrElse row.event_type_id.code = "9216" Then
                                            ' Estos eventos NO son fichajes. De todos modos, no deberíamos pasar por aquí nunca ...
                                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Event ignored: " & PunchToString(oTerminal("SerialNumber"), row) & "  Response: " & row.event_type_id.code)
                                            Continue For
                                        End If

                                        ' Estado de conexión
                                        If row.event_type_id.code = "4095" OrElse row.event_type_id.code = "13056" Then
                                            Select Case row.event_type_id.code
                                                Case "4095"
                                                    ' Terminal desconectado
                                                    deviceConnected = False
                                                Case "13056"
                                                    ' Terminal conectado
                                                    deviceConnected = True
                                            End Select
                                            updateStamp = True
                                        Else
                                            'Lo que he recibido es un fichaje. Por tanto, estaba conectado
                                            deviceConnected = True

                                            Dim iIdEmployee As Integer = -1
                                            If row.user_id IsNot Nothing AndAlso row.user_id.user_id IsNot Nothing AndAlso row.user_id.user_id.Trim <> "" AndAlso roTypes.Any2Long(row.user_id.user_id) > 0 Then
                                                Dim tbEmployees As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldIdAndValue(configParameters.EmployeeUserfieldId, roTypes.Any2String(row.user_id.user_id), Now, New UserFields.roUserFieldState())
                                                If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then iIdEmployee = tbEmployees.Rows(0).Item("idemployee")
                                            Else
                                                ' Si el ID de empleado viene vacío, alerto y salgo. De otro modo, le caerá el fichaje al primer empleado sin campo Id Importación informado
                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "ProcessSupremaDevicesEvents::GetPunches::Punch without user_id. " & PunchToString(oTerminal("SerialNumber"), row) & " .Ignoring ...")
                                                Continue For
                                            End If

                                            Dim idReader As roTerminal.roTerminalReader = cTerminal.ReaderByIndex(0)
                                            Dim oPunch As roPunch = Nothing
                                            Dim oExistingPunch As DataTable

                                            ' Hora UTC del fichaje
                                            Dim utcPunchDatetime = row.datetime
                                            ' Zona horaria
                                            Dim hoursOffset As Integer = Convert.ToInt32(row.timezone.hour)
                                            Dim halfHourOffset As Integer = Convert.ToInt32(row.timezone.half)
                                            Dim negativeOffset As Integer = Convert.ToInt32(row.timezone.negative)
                                            Dim totalOffset As Double = hoursOffset + If(halfHourOffset = 1, 0.5, 0)
                                            If negativeOffset = 1 Then
                                                totalOffset = -totalOffset
                                            End If
                                            ' Ajustar la hora UTC a la hora local
                                            Dim localPunchDatetime As DateTime = utcPunchDatetime.AddHours(totalOffset)
                                            ' Finalmente tengo en cuenta el DST
                                            If Convert.ToInt32(row.is_dst) = 1 Then
                                                localPunchDatetime = localPunchDatetime.AddHours(1)
                                            End If

                                            If iIdEmployee > 0 Then
                                                oPunch = New roPunch With {
                                                            .IDTerminal = cTerminal.ID,
                                                            .IDReader = 1,
                                                            .DateTime = localPunchDatetime,
                                                            .IDEmployee = iIdEmployee,
                                                            .Type = PunchTypeEnum._IN,
                                                            .ActualType = PunchTypeEnum._IN,
                                                            .IDZone = idReader.IDZone,
                                                            .Field1 = "Suprema",
                                                            .Field2 = row.event_type_id.code
                                                            }
                                            Else
                                                oPunch = New roPunch With {
                                                            .IDTerminal = cTerminal.ID,
                                                            .IDReader = 1,
                                                            .DateTime = localPunchDatetime,
                                                            .Type = PunchTypeEnum._IN,
                                                            .ActualType = PunchTypeEnum._IN,
                                                            .IDZone = idReader.IDZone,
                                                            .Field1 = "Suprema",
                                                            .Field2 = row.event_type_id.code,
                                                            .IDEmployee = 0,
                                                            .IDCredential = row.user_id.user_id
                                                            }
                                            End If

                                            Select Case idReader.InteractionAction
                                                Case InteractionAction.S
                                                    oPunch.Type = PunchTypeEnum._OUT
                                                    oPunch.ActualType = PunchTypeEnum._OUT
                                                    oPunch.IDZone = idReader.IDZoneOut
                                                Case InteractionAction.X
                                                    oPunch.Type = PunchTypeEnum._AUTO
                                                    oPunch.ActualType = PunchTypeEnum._AUTO
                                                    oPunch.IDZone = 0
                                            End Select

                                            ' Control de repetidos (para el caso de fichajes no asignados a empleados. El resto ya lo haría el save del ropunch
                                            If oPunch IsNot Nothing Then
                                                Dim strFilter As String = ""
                                                strFilter = "IDEmployee = " & oPunch.IDEmployee & " AND IDCredential = " & oPunch.IDCredential.ToString & " AND DateTime = " + roTypes.Any2Time(oPunch.DateTime).SQLDateTime + " AND Type = " & oPunch.Type & " AND Field1 = 'Suprema' AND IDTerminal = " & oPunch.IDTerminal.ToString
                                                oExistingPunch = VTBusiness.Punch.roPunch.GetPunches(strFilter, Nothing)
                                                If oExistingPunch.Rows.Count > 0 Then
                                                    ' No guardo
                                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Punch duplicated. It won't be saved: " & PunchToString(oTerminal("SerialNumber"), row) & " -- > oPunc" & oPunch.ToString)
                                                    updateStamp = True
                                                Else
                                                    oPunch.Source = PunchSource.Suprema
                                                    updateStamp = oPunch.Save()
                                                End If
                                            End If
                                        End If

                                        If updateStamp Then
                                            cTerminal.UpdatePunchStamp(JsonConvert.SerializeObject(row.datetime).Replace("""", ""))
                                        Else
                                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Could not insert punch:{" & JsonConvert.SerializeObject(row) & "}")
                                            Exit For
                                        End If
                                    Next
                                Else
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::No pending punches to process.")
                                End If
                            End If
                            If initialConnectionStatus <> deviceConnected Then
                                cTerminal.UpdateStatus(deviceConnected, True)
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Bad Request: Response: " & response.ReasonPhrase)
                        End If
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Terminal not found in the list of terminals. Ignoring ...")
                    End If
                Next
                Extensions.roParameters.SaveDateParameter("SupremaLastRun", Now)
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::ProcessSupremaDevicesEvents::Finished. See you in " & configParameters.CheckPeriod.ToString & " minutes!")
            Catch ex As Exception
                Extensions.roParameters.SaveDateParameter("SupremaLastRun", roTypes.CreateDateTime(1900, 1, 1))
                processSuccessful = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::ProcessSupremaDevicesEvents :", ex)
            End Try

            Return processSuccessful

        End Function

        Public Function SyncData() As roDataLinkState
            Dim oDataLinkState As roDataLinkState = New roDataLinkState(-1)

            Try
                oDataLinkState.Result = DataLinkResultEnum.NoError

                If Me.Login() Then
                    If Not Me.ProcessSupremaDevicesEvents() Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::SyncData: Could not import punches")
                        oDataLinkState.Result = DataLinkResultEnum.SupremaErrorGettingPunches
                    Else
                        SetNextScheduleTime()
                    End If
                End If

            Catch ex As Exception
                oDataLinkState.Result = DataLinkResultEnum.Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::SyncData :", ex)
            End Try

            Return oDataLinkState
        End Function

        Public Function GetNextScheduleTime() As DateTime
            Dim nextRun As DateTime
            nextRun = VTBase.Extensions.roParameters.GetDateParameter("SupremaNextRun")
            If nextRun = Date.MinValue Then
                nextRun = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, configParameters.DailyInitialTime.Hour, configParameters.DailyInitialTime.Minute, 0, DateTimeKind.Local)
            End If
            Return nextRun
        End Function

        Public Function SetNextScheduleTime() As Boolean
            Dim actualNextRun As DateTime = GetNextScheduleTime()
            Dim dateNow As DateTime = DateTime.Now
            Dim nextRun As DateTime

            ' Sólo si se ha sobrepasado la  hora de próxima ejecución
            If actualNextRun < dateNow Then
                Dim firstDayRun As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, configParameters.DailyInitialTime.Hour, configParameters.DailyInitialTime.Minute, 0, DateTimeKind.Local)

                If dateNow < firstDayRun Then
                    If firstDayRun.Subtract(dateNow).TotalMinutes > configParameters.CheckPeriod Then
                        nextRun = actualNextRun.AddMinutes(configParameters.CheckPeriod)
                    Else
                        nextRun = firstDayRun
                    End If
                Else
                    Dim nextRegularRun As DateTime = firstDayRun
                    While nextRegularRun < dateNow
                        nextRegularRun = nextRegularRun.AddMinutes(configParameters.CheckPeriod)
                    End While
                    nextRun = nextRegularRun
                End If

                VTBase.Extensions.roParameters.SaveDateParameter("SupremaNextRun", nextRun)

            Else
                Return False
            End If

            Return True
        End Function

        Private Function PunchToString(serialNumber As String, oPunch As Suprema.punch) As String
            Try
                Return "Suprema Terminal SN: " & serialNumber & " Datetime:" & oPunch.datetime.ToLocalTime.ToString & " UserId: " & If(oPunch.user_id Is Nothing OrElse oPunch.user_id.user_id Is Nothing, "Nothing", oPunch.user_id.user_id)
            Catch ex As Exception
                Return "--error parsing punch--"
            End Try
        End Function

        Public Function GetSupremaEventCodes() As List(Of Integer)
            Dim returnList As New List(Of Integer)
            Dim query As String = "@SELECT# code FROM sysroSupremaEventCodes"

            Try
                Using reader As DbDataReader = AccessHelper.CreateDataReader(query)
                    While reader.Read()
                        returnList.Add(reader("code"))
                    End While
                End Using
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::GetSupremaEventCodes :", ex)
            End Try

            Return returnList
        End Function

        Private Function DeepClone(Of T)(ByVal obj As T) As T
            Using ms As New MemoryStream()
                Dim formatter As New BinaryFormatter()
                formatter.Serialize(ms, obj)
                ms.Position = 0
                Return DirectCast(formatter.Deserialize(ms), T)
            End Using
        End Function

    End Class

End Namespace