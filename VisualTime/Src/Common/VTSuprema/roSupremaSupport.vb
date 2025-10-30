Imports System.Net.Http
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roSupremaSupport
    '
    ' Clase con funciones de propósito general de ValidateID
    '

    Private Shared _mInstance As roSupremaSupport = Nothing
    Private oLog As New roLog("Suprema")

    Private isSupremaSystem As Boolean = False

    Private sUserName As String = ""
    Private sPassword As String = ""
    Private sURLApi As String = "" ' --https://pre-vidsignercloud.validatedid.com/api/
    Private sToken As String = ""

    Private sInitialLinkDate As String = ""

    Private xInitialHour As New DateTime(1900, 1, 1, 0, 0, 0)
    Private iCheckPeriod As Integer = -1

    Private strEmployeeUserField As String = String.Empty

    Public Shared ReadOnly Property GetInstance(Optional ByVal bForceNew As Boolean = False) As roSupremaSupport
        Get
            If bForceNew Then
                Return New roSupremaSupport()
            Else
                If (_mInstance Is Nothing) Then
                    _mInstance = New roSupremaSupport()
                End If
                Return _mInstance
            End If
        End Get
    End Property

    Public ReadOnly Property IsEnabled As Boolean
        Get
            Return isSupremaSystem
        End Get
    End Property

#Region "Declarations - Constructor"

    Public Sub New()
        Try
            Dim sql As String = ""
            If sUserName.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.UserName'"
                sUserName = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If sPassword.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.Password'"
                sPassword = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If sURLApi.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.URL'"
                sURLApi = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If strEmployeeUserField.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.EmployeeField'"
                strEmployeeUserField = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If sInitialLinkDate.Length = 0 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.InitialLinkDate'"
                sInitialLinkDate = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            Dim tmpVal As String() = {}

            If iCheckPeriod = -1 Then
                sql = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VisualTime.Link.Suprema.CheckPeriod'"
                tmpVal = roTypes.Any2String(AccessHelper.ExecuteScalar(sql)).Split("@")
            End If

            If tmpVal.Length = 2 Then
                Dim strHours = tmpVal(0).Split(":")
                If strHours.Length = 2 Then
                    xInitialHour = New DateTime(1900, 1, 1, roTypes.Any2Integer(tmpVal(0).Split(":")(0)), roTypes.Any2Integer(tmpVal(0).Split(":")(1)), 0)
                Else
                    xInitialHour = New DateTime(1900, 1, 1, 0, 10, 0)
                End If

                iCheckPeriod = roTypes.Any2Integer(tmpVal(1))

                If iCheckPeriod <= 0 Then iCheckPeriod = 5
            Else
                xInitialHour = New DateTime(1900, 1, 1, 0, 10, 0)
                iCheckPeriod = 30
            End If

            If sURLApi <> String.Empty AndAlso sUserName <> String.Empty AndAlso sPassword <> String.Empty AndAlso strEmployeeUserField <> String.Empty AndAlso sInitialLinkDate <> String.Empty Then
                Net.ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf MyCertHandler)
                oLog.logMessage(roLog.EventType.roDebug, "roSupremaSupport::Started successfully.")
                isSupremaSystem = True
            Else
                oLog.logMessage(roLog.EventType.roDebug, "roSupremaSupport::Not Started, missing parameters configuration")
                isSupremaSystem = False
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::GetConfiguration :", ex)
        End Try

    End Sub

    Private Function MyCertHandler(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
        Return True
    End Function

#End Region

    Private Function Login(Optional ByVal oConnection As roBaseConnection = Nothing) As Boolean
        Dim bRet As Boolean = True

        Try

            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")

            Client.BaseAddress = New Uri(sURLApi)
            Client.Timeout = New TimeSpan(0, 0, 100)

            Dim json = JsonConvert.SerializeObject(New Suprema.LoginRequest() With {.User = New Suprema.User() With {.login_id = sUserName, .password = sPassword}})
            Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
            ' Subimos el documento
            Dim response = Client.PostAsync("api/login", content).Result

            If response.IsSuccessStatusCode Then
                Dim OB As Object
                OB = JsonConvert.DeserializeObject(Of Suprema.LoginResponse)(response.Content.ReadAsStringAsync().Result)

                For Each oHeader In response.Headers
                    If oHeader.Key = "bs-session-id" Then
                        sToken = roTypes.Any2String(oHeader.Value(0))
                        Exit For
                    End If
                Next
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::Login:: Bad Request: Response: " & response.ReasonPhrase)
            End If
        Catch ex As Exception
            bRet = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::Login :", ex)
        Finally

        End Try

        Return bRet

    End Function

    Private Function GetPunches(Optional ByVal oConnection As roBaseConnection = Nothing) As Boolean

        Dim oReturn As Boolean = True
        Try

            Dim oTerminalList As New Robotics.Base.VTBusiness.Terminal.roTerminalList

            Dim dtTerminals As DataTable = oTerminalList.GetTerminals("Type='Virtual'")

            Dim Client As HttpClient = New HttpClient()
            Client.BaseAddress = New Uri(sURLApi)
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.DefaultRequestHeaders.Add("bs-session-id", sToken)
            Client.Timeout = New TimeSpan(0, 0, 100)

            If dtTerminals.Rows.Count = 0 Then
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::No virtual terminal configured. Cannot import punches!. Bye ... ")
                Return True
            End If

            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Start processing punches comming from Suprema devices")

            For Each oTerminal As DataRow In dtTerminals.Rows
                Dim cTerminal As roTerminal = Nothing

                For Each tmpTerminal As roTerminal In oTerminalList.Terminals
                    If tmpTerminal.ID = roTypes.Any2Integer(oTerminal("Id")) Then
                        cTerminal = tmpTerminal
                        Exit For
                    End If
                Next

                Dim checkDatetime As String = sInitialLinkDate
                If Not IsDBNull(oTerminal("PunchStamp")) AndAlso roTypes.Any2String(oTerminal("PunchStamp")) <> String.Empty Then
                    Try
                        checkDatetime = roTypes.Any2String(oTerminal("PunchStamp"))
                    Catch ex As Exception
                        checkDatetime = sInitialLinkDate
                    End Try
                End If

                'TODO: La comparación Greater podría eliminar un resultado si hay dos en el mismo instante de dos empleados diferentes, y uno de los dos no se recupera en la llamada (remoto)
                'Dim oCond As Suprema.condition() = {
                '        New Suprema.condition() With {.column = "datetime", .[operator] = Suprema.AvailableOperators.Greater, .values = {checkDatetime}},
                '        New Suprema.condition() With {.column = "event_type_id.code", .[operator] = Suprema.AvailableOperators.Not_Equal, .values = {"12800", "9216"}},
                '        New Suprema.condition() With {.column = "device_id.id", .[operator] = Suprema.AvailableOperators.Equal, .values = {oTerminal("Location")}}
                '    }

                Dim oCond As Suprema.condition() = {
                        New Suprema.condition() With {.column = "datetime", .[operator] = Suprema.AvailableOperators.Greater, .values = {checkDatetime}},
                        New Suprema.condition() With {.column = "event_type_id.code", .[operator] = Suprema.AvailableOperators.Equal, .values = {"4102", "4103", "4104", "4105", "4106", "4107", "4112", "4113", "4114", "4115", "4118", "4119", "4120", "4121", "4122", "4123", "4128", "4129", "4097", "4098", "4099", "4100", "4101"}},
                        New Suprema.condition() With {.column = "device_id.id", .[operator] = Suprema.AvailableOperators.Equal, .values = {oTerminal("Location")}}
                    }

                ' Posiblemente haya que añadir los codes de identify (los que haya arriba son los de verify)
                ' Son estos "4866","4867","4868","4869","4870","4871","4872"

                ' Otra opción de consulta sería usar operador betweeen
                'New Suprema.condition() With {.column = "event_type_id.code", .[operator] = Suprema.AvailableOperators.Between, .values = {"4097","4129"}},

                Dim oOrders As Suprema.order() = {New Suprema.order() With {.column = "datetime", .descending = False}}

                Dim json = JsonConvert.SerializeObject(New Suprema.EventsRequest() With {.Query = New Suprema.Query() With {.limit = 1000, .conditions = oCond, .orders = oOrders}})
                Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
                ' Subimos el documento
                Dim response = Client.PostAsync("/api/events/search", content).Result

                If response.IsSuccessStatusCode Then
                    Dim oResult As String = response.Content.ReadAsStringAsync().Result
                    Dim OB As Suprema.SearchResponse = JsonConvert.DeserializeObject(Of Suprema.SearchResponse)(oResult)

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Asking Suprema API for punches from terminal " & cTerminal.Description & " after " & checkDatetime)

                    If OB.EventCollection IsNot Nothing AndAlso OB.EventCollection.rows IsNot Nothing Then
                        If OB.EventCollection.rows.Count > 0 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Start processing " & OB.EventCollection.rows.Count.ToString & " punches")

                            For Each row As Suprema.punch In OB.EventCollection.rows
                                If row.event_type_id.code = "12800" OrElse row.event_type_id.code = "9216" Then
                                    ' Estos eventos NO son fichajes. De todos modos, no deberíamos pasar por aquí nunca ...
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::Event::Event ignored: " & PunchToString(oTerminal("Location"), row) & "  Response: " & row.event_type_id.code)
                                    Continue For
                                End If

                                Dim iIdEmployee As Integer = -1
                                If row.user_id IsNot Nothing AndAlso row.user_id.user_id IsNot Nothing AndAlso row.user_id.user_id.Trim <> "" AndAlso roTypes.Any2Long(row.user_id.user_id) > 0 Then
                                    Dim tbEmployees As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(Me.strEmployeeUserField, roTypes.Any2String(row.user_id.user_id), Now, New UserFields.roUserFieldState())
                                    If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then iIdEmployee = tbEmployees.Rows(0).Item("idemployee")
                                Else
                                    ' Si el ID de empleado viene vacío, alerto y salgo. De otro modo, le caerá el fichaje al primer empleado sin campo Id Importación informado
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Punch without user_id. " & PunchToString(oTerminal("Location"), row) & " .Ignoring ...")
                                    Continue For
                                End If

                                If cTerminal IsNot Nothing Then
                                    Dim idReader As roTerminal.roTerminalReader = cTerminal.ReaderByIndex(0)
                                    Dim oPunch As roPunch = Nothing
                                    Dim oExistingPunch As DataTable
                                    If iIdEmployee > 0 Then
                                        oPunch = New roPunch With {
                                        .IDTerminal = cTerminal.ID,
                                        .IDReader = 1,
                                        .DateTime = row.datetime.ToLocalTime,
                                        .IDEmployee = iIdEmployee,
                                        .Type = IIf(idReader.InteractionAction = InteractionAction.E, PunchTypeEnum._IN, PunchTypeEnum._OUT),
                                        .ActualType = IIf(idReader.InteractionAction = InteractionAction.E, PunchTypeEnum._IN, PunchTypeEnum._OUT),
                                        .IDZone = IIf(idReader.InteractionAction = InteractionAction.E, idReader.IDZone, idReader.IDZoneOut),
                                        .Field1 = "Suprema"
                                    }
                                    Else
                                        oPunch = New roPunch With {
                                        .IDTerminal = cTerminal.ID,
                                        .IDReader = 1,
                                        .DateTime = row.datetime.ToLocalTime,
                                        .Type = IIf(idReader.InteractionAction = InteractionAction.E, PunchTypeEnum._IN, PunchTypeEnum._OUT),
                                        .ActualType = IIf(idReader.InteractionAction = InteractionAction.E, PunchTypeEnum._IN, PunchTypeEnum._OUT),
                                        .IDZone = IIf(idReader.InteractionAction = InteractionAction.E, idReader.IDZone, idReader.IDZoneOut),
                                        .Field1 = "Suprema",
                                        .IDEmployee = 0,
                                        .IDCredential = row.user_id.user_id
                                    }
                                    End If

                                    ' Control de repetidos (para el caso de fichajes no asignados a empleados. El resto ya lo haría el save del ropunch
                                    If oPunch IsNot Nothing Then
                                        Dim strFilter As String = ""
                                        strFilter = "IDEmployee = " & oPunch.IDEmployee & " AND IDCredential = " & oPunch.IDCredential.ToString & " AND DateTime = " + roTypes.Any2Time(oPunch.DateTime).SQLDateTime + " AND Type = " & oPunch.Type & " AND Field1 = 'Suprema' AND IDTerminal = " & oPunch.IDTerminal.ToString
                                        oExistingPunch = VTBusiness.Punch.roPunch.GetPunches(strFilter, Nothing)
                                        If oExistingPunch.Rows.Count > 0 Then
                                            ' No guardo
                                            oLog.logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Punch duplicated. It won't be saved: " & PunchToString(oTerminal("Location"), row) & " -- > oPunc" & oPunch.ToString)
                                            oReturn = True
                                        Else
                                            oPunch.Source = PunchSource.CustomImport
                                            oReturn = oPunch.Save()
                                        End If
                                    End If

                                    If oReturn Then
                                        Dim updateTerminalSQL = "@UPDATE# Terminals SET PunchStamp = '" & JsonConvert.SerializeObject(row.datetime).Replace("""", "") & "' WHERE ID = " & cTerminal.ID
                                        DataLayer.AccessHelper.ExecuteSql(updateTerminalSQL)
                                    Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Could not insert punch:{" & JsonConvert.SerializeObject(row) & "}")
                                        Exit For
                                    End If

                                End If

                            Next
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::No pending punches to process.")
                        End If
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Bad Request: Response: " & response.ReasonPhrase)
                End If
            Next
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::GetPunches::Finished. See you in " & iCheckPeriod.ToString & " minutes!")
        Catch ex As Exception
            oReturn = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::GetPunches :", ex)
        Finally

        End Try

        Return oReturn

    End Function

    Public Function SyncData() As Boolean
        Dim bRet As Boolean = True

        Try

            If Me.Login() Then
                If Not Me.GetPunches() Then roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roSupremaSupport::SyncData:Could not import punches")
            End If
        Catch ex As Exception
            bRet = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupremaSupport::SyncData :", ex)
        End Try

        Return bRet
    End Function

    Public Function GetNextScheduleTime() As DateTime
        Dim oNextRun As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, xInitialHour.Hour, xInitialHour.Minute, 0)

        While oNextRun < DateTime.Now
            oNextRun = oNextRun.AddMinutes(iCheckPeriod)
        End While

        Return oNextRun
    End Function

    Private Function PunchToString(idTerminal As Integer, oPunch As Suprema.punch) As String
        Try
            Return "TerminalId: " & idTerminal.ToString & " Datetime:" & oPunch.datetime.ToLocalTime.ToString & " UserId: " & If(oPunch.user_id Is Nothing OrElse oPunch.user_id.user_id Is Nothing, "Nothing", oPunch.user_id.user_id)
        Catch ex As Exception
            Return "--error parsing punch--"
        End Try
    End Function

End Class