Imports System.Data.Common
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports System.Web
Imports Newtonsoft.Json.Linq
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace CTAIMA

    Public Class CTAIMASystem
        Private Shared _mInstance As Hashtable = Nothing

        Private oLog As New roLog("CTAIMA")

        Private isCTAIMASystem As Boolean = False

        Private _strCTAIMAUserName As String = ""
        Private _strCTAIMAPwd As String = ""
        Private _strCTAIMAWSurl As String = ""

        Private _strCTAIMAUserFieldName As String = ""

        Private _strCTAIMAInitialHour As New DateTime(1900, 1, 1, 0, 0, 0)
        Private _strCTAIMACheckPeriod As Integer

        Private _isPULogEnabled As Boolean = False

        Private _mustLaunchBroadcaster As Boolean = False

#Region "Properties"

        Public ReadOnly Property IsEnabled As Boolean
            Get
                Return isCTAIMASystem
            End Get
        End Property

#End Region

        Public Sub New()
            Try

                Dim oParam As New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaUserLinkEnabled).ToString, New AdvancedParameter.roAdvancedParameterState())

                If roTypes.Any2String(oParam.Value) = "1" Then
                    isCTAIMASystem = True
                Else
                    isCTAIMASystem = False
                End If

                If isCTAIMASystem Then

                    oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaUserName).ToString, New AdvancedParameter.roAdvancedParameterState())
                    _strCTAIMAUserName = roTypes.Any2String(oParam.Value)

                    oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaPassword).ToString, New AdvancedParameter.roAdvancedParameterState())
                    _strCTAIMAPwd = roTypes.Any2String(oParam.Value)

                    oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaWsURL).ToString, New AdvancedParameter.roAdvancedParameterState())
                    _strCTAIMAWSurl = roTypes.Any2String(oParam.Value)

                    oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaPRLUserField).ToString, New AdvancedParameter.roAdvancedParameterState())
                    _strCTAIMAUserFieldName = roTypes.Any2String(oParam.Value)

                    oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaCheckPeriod).ToString, New AdvancedParameter.roAdvancedParameterState())
                    Dim tmpVal As String() = roTypes.Any2String(oParam.Value).Split("@")

                    _strCTAIMAInitialHour = New DateTime(1900, 1, 1, roTypes.Any2Integer(tmpVal(0).Split(":")(0)), roTypes.Any2Integer(tmpVal(0).Split(":")(1)), 0)
                    _strCTAIMACheckPeriod = roTypes.Any2Integer(tmpVal(1))

                    Try
                        oParam = New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaDebug).ToString, New AdvancedParameter.roAdvancedParameterState())
                        _isPULogEnabled = (roTypes.Any2Integer(oParam.Value) = 1)
                    Catch ex As Exception
                        _isPULogEnabled = False
                    End Try
                End If
            Catch ex As Exception

            End Try

        End Sub

        Public Function SyncData() As roDataLinkState
            Dim oDataLinkState As roDataLinkState = New roDataLinkState(-1)
            oDataLinkState.Result = DataLinkResultEnum.NoError
            'Enviamos lista de fichajes pendientes de la tabla de sincornización de plusultra
            Try
                'Dim wsPU As New CT_EstadoTrabajadoresPortTypeClient("BasicHttpBinding_ICT_EstadoTrabajadoresPortType1", Me._strCTAIMAWSurl)

                Dim wsPU As New BasicHttpBinding_ICT_EstadoTrabajadoresPortType()
                wsPU.Url = Me._strCTAIMAWSurl

                wsPU.Timeout = 180000

                SyncCTAIMAprl(wsPU)
            Catch ex As Exception
                oDataLinkState.Result = DataLinkResultEnum.CTAIMAError
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncData::Unknown error calling webservice::" & ex.Message)
            End Try

            Return oDataLinkState
        End Function

        Public Function SyncDataRestful() As roDataLinkState
            Dim bolRet As Boolean = True
            Dim oAuditParam As New Generic.List(Of String)
            Dim oAuditValues As New Generic.List(Of String)

            oAuditParam.Add("{sImportType}")
            oAuditValues.Add("Employees PRL permission")
            Dim oDataLinkState As roDataLinkState = New roDataLinkState(-1)
            oDataLinkState.Result = DataLinkResultEnum.NoError

            Try
                Dim tenantId = New AdvancedParameter.roAdvancedParameter("CTaimaTenantId", New AdvancedParameter.roAdvancedParameterState).Value
                Dim apiVersion = New AdvancedParameter.roAdvancedParameter("CTaimaApiVersion", New AdvancedParameter.roAdvancedParameterState).Value
                Dim ocpApimSubscriptionKey = New AdvancedParameter.roAdvancedParameter("CTaimaOcmApimSubscriptionKey", New AdvancedParameter.roAdvancedParameterState).Value

                Dim tb As DataTable = roBusinessSupport.GetEmployees(String.Empty, "", "", New roEmployeeState(-1))

                If tb IsNot Nothing Then

                    For Each row As DataRow In tb.Rows
                        Try

                            Dim dni As String = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("IDEmployee"), "NIF", Date.Now, New UserFields.roUserFieldState).FieldValue
                            Dim centerId As String = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("IDEmployee"), "CTaimaCenterId", Date.Now, New UserFields.roUserFieldState)?.FieldValue
                            Dim requieresUpdate As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("IDEmployee"), "CTaimaPRLUserField", Date.Now, New UserFields.roUserFieldState(-1))

                            If (Not IsNothing(dni) AndAlso Not dni.Equals(String.Empty)) AndAlso (Not IsNothing(centerId) AndAlso Not centerId.Equals(String.Empty)) Then
                                ' If (dni = "26206374J") Then
                                If requieresUpdate Is Nothing OrElse (requieresUpdate IsNot Nothing AndAlso requieresUpdate.FieldValue = "") Then
                                    'Request parameters
                                    Dim queryString = HttpUtility.ParseQueryString(String.Empty)
                                    queryString.Item("dni") = dni
                                    queryString.Item("centerId") = centerId
                                    Dim Uri = "https://ctaima.azure-api.net/api/ctaimacae/access-control/employees-accomplishment-states?" & queryString.ToString()

                                    Using client As New HttpClient()
                                        'Request headers
                                        client.DefaultRequestHeaders.Add("tenantId", tenantId)
                                        client.DefaultRequestHeaders.Add("api-version", apiVersion)
                                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey)

                                        Dim response = client.GetAsync(Uri).Result
                                        Dim responseString As String = response.Content.ReadAsStringAsync().Result

                                        If (response.IsSuccessStatusCode) Then

                                            Dim isAuthorized As Boolean = False

                                            Dim deserializedCtaima As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString)
                                            If deserializedCtaima IsNot Nothing AndAlso deserializedCtaima.Count > 0 Then
                                                Dim i As Integer = 0
                                                Dim dueDate As DateTime = Nothing
                                                While isAuthorized = False AndAlso i < deserializedCtaima.Count
                                                    dueDate = roTypes.Any2DateTime(deserializedCtaima(i)("dueDate").Value)

                                                    If dueDate >= Date.Now.Date Then
                                                        isAuthorized = True
                                                    End If
                                                    i = i + 1
                                                End While

                                                UpdateUserFieldRestful(dni, isAuthorized)

                                                oAuditParam.Add("{sObjectId}")
                                                oAuditValues.Add($"Employee {dni}")
                                                'oAuditParam.Add($"Employee {dni} OK")
                                                'oAuditValues.Add(reason)
                                                oAuditParam.Add("{sReason}")
                                                oAuditValues.Add("SUCCESSED " & dueDate.ToString)

                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CCTAIMA::SyncData::SyncDataRestful::Request result is successful for employee with ID " & row("IDEmployee"))
                                            Else
                                                oAuditParam.Add("{sObjectId}")
                                                oAuditValues.Add($"Employee {dni}")
                                                oAuditParam.Add("{sReason}")
                                                oAuditValues.Add("FAILED " & responseString)
                                            End If
                                        Else
                                            oAuditParam.Add("{sObjectId}")
                                            oAuditValues.Add($"Employee {dni}")
                                            oAuditParam.Add("{sReason}")
                                            oAuditValues.Add("FAILED " & responseString)
                                            'roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncData::SyncDataRestful::Request result not success for ID " & row("IDEmployee") & ": " & response.ToString)
                                        End If
                                    End Using
                                End If
                                ' End If
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncDataRestful::ErrorRetrievingEmployeeAccessInfo::" & ex.Message)
                        End Try
                    Next

                End If

                If Me._mustLaunchBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
            Catch ex As Exception
                bolRet = False
                oDataLinkState.Result = DataLinkResultEnum.CTAIMAError
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncData::Unknown error calling webservice::" & ex.Message)
            End Try

            Base.VTBusiness.Support.roLiveSupport.Audit(Audit.Action.aExecuted,
                                                        Robotics.VTBase.Audit.ObjectType.tDatalinkWS,
                                                        $"Ctaima PRL Synchronization: {If(bolRet, "SUCCESSED", "FAILED")}",
                                                        oAuditParam,
                                                        oAuditValues,
                                                        New Base.VTBusiness.AuditState.wscAuditState(-1))

            Return oDataLinkState
        End Function

        Public Function SendDataRestful() As roDataLinkState
            Dim bolRet As Boolean = True
            Dim oAuditParam As New Generic.List(Of String)
            Dim oAuditValues As New Generic.List(Of String)

            oAuditParam.Add("{sImportType}")
            oAuditValues.Add("Punches synchronization")

            Dim oDataLinkState As roDataLinkState = New roDataLinkState(-1)
            oDataLinkState.Result = DataLinkResultEnum.NoError

            'Enviamos los fichajes a Ctaima
            Try
                Dim tenantId = New AdvancedParameter.roAdvancedParameter("CTaimaTenantId", New AdvancedParameter.roAdvancedParameterState).Value
                Dim apiVersion = New AdvancedParameter.roAdvancedParameter("CTaimaApiVersion", New AdvancedParameter.roAdvancedParameterState).Value
                Dim ocpApimSubscriptionKey = New AdvancedParameter.roAdvancedParameter("CTaimaOcmApimSubscriptionKey", New AdvancedParameter.roAdvancedParameterState).Value

                Dim sql As String = "@SELECT#
	                                (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues AS EUV WHERE EUV.IDEmployee = P.IDEmployee AND EUV.FieldName = 'NIF' ORDER BY EUV.Date DESC) AS Dni,
	                                (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues AS EUV WHERE EUV.IDEmployee = P.IDEmployee AND EUV.FieldName = 'CIF' ORDER BY EUV.Date DESC) AS Cif,
                                    (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues AS EUV WHERE EUV.IDEmployee = P.IDEmployee AND EUV.FieldName = 'CTaimaCenterId' ORDER BY EUV.Date DESC) AS CTaimaCenterId,
	                                P.ID,
	                                P.IDEmployee,
	                                P.DateTime,
	                                CASE
		                                WHEN P.Type = 5 THEN
			                                CASE
				                                WHEN Z.IsWorkingZone = 1 THEN 0
				                                WHEN Z.IsWorkingZone = 0 THEN 1
			                                END
		                                WHEN P.Type = 7 THEN
                                            CASE
                                                  WHEN P.ActualType = 1 THEN 0
                                                  WHEN P.ActualType = 2 THEN 1
                                            END
	                                END AS Type,
	                                P.Exported
                                    FROM Punches AS P
                                    LEFT JOIN Zones AS Z ON P.IDZone = Z.ID
                                    WHERE P.Type IN (5, 7) AND P.Exported = 0
                                    ORDER BY IDEmployee"
                Dim exportedIdsList As String = String.Empty

                Dim tb As DataTable = AccessHelper.CreateDataTable(sql)

                For Each row As DataRow In tb.Rows
                    Try
                        Dim dni As String = roTypes.Any2String(row("Dni"))
                        Dim cif As String = roTypes.Any2String(row("Cif"))
                        Dim centerId As String = roTypes.Any2String(row("CTaimaCenterId"))
                        Dim type As Integer = roTypes.Any2Integer(row("Type"))
                        Dim punchDate As Date = roTypes.Any2Time(row("DateTime")).ValueDateTime

                        If Not dni.Equals(String.Empty) And Not cif.Equals(String.Empty) And Not centerId.Equals(String.Empty) Then
                            Using client As New HttpClient()
                                'Request headers
                                client.DefaultRequestHeaders.Add("tenantId", tenantId)
                                client.DefaultRequestHeaders.Add("api-version", apiVersion)
                                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey)

                                Dim Uri = "https://ctaima.azure-api.net/api/ctaimacae/in-out-registrations/ou/" & centerId

                                Dim body As String = "{" &
                                                        """Cif"": """ & cif & """," &
                                                        """ResourceType"":  ""0""," &
                                                        """ResourceValue"": """ & dni & """," &
                                                        """inOutType"": """ & type & """," &
                                                        """inOutDate"": """ & punchDate.Year & "-" & punchDate.Month & "-" & punchDate.Day & " " & punchDate.TimeOfDay.ToString & """," &
                                                        """Comments"": """"" &
                                                    "}"

                                Dim byteData As Byte() = Encoding.UTF8.GetBytes(body)

                                Using content As New ByteArrayContent(byteData)
                                    content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                                    Dim response = client.PostAsync(Uri, content).Result
                                    Dim responseString As String = response.Content.ReadAsStringAsync().Result

                                    If response.IsSuccessStatusCode Then
                                        exportedIdsList &= $"{row("ID")},"

                                        oAuditParam.Add("{sObjectId}")
                                        oAuditValues.Add($"Employee {dni} : Date {punchDate} : Type {type}")
                                        oAuditParam.Add("{sReason}")
                                        oAuditValues.Add("SUCCESSED")

                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CCTAIMA::SyncData::SendDataRestful::Request result is successful for employee with ID " & row("ID"))
                                    Else
                                        oAuditParam.Add("{sObjectId}")
                                        oAuditValues.Add($"Employee {dni} : Date {punchDate} Type {If(type = 1, "IN", "OUT")}")
                                        oAuditParam.Add("{sReason}")
                                        oAuditValues.Add("FAILED " & responseString)

                                        roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncData::SendDataRestful::Request result not success " & response.ToString & " " & body)
                                    End If
                                End Using
                            End Using
                        End If
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SendDataRestful::::" & ex.Message)
                    End Try
                Next

                If Not exportedIdsList.Equals(String.Empty) Then
                    exportedIdsList = exportedIdsList.Substring(0, exportedIdsList.Length - 1)

                    bolRet = AccessHelper.ExecuteSql($"@UPDATE# Punches SET Exported = 1 WHERE ID IN ({exportedIdsList})")
                End If

                Base.VTBusiness.Support.roLiveSupport.Audit(Audit.Action.aExecuted,
                                                        Robotics.VTBase.Audit.ObjectType.tDatalinkWS,
                                                        $"Ctaima Punches Register Synchronization: {If(bolRet, "SUCCESSED", "FAILED")}",
                                                        oAuditParam,
                                                        oAuditValues,
                                                        New Base.VTBusiness.AuditState.wscAuditState(-1))
            Catch ex As Exception
                oDataLinkState.Result = DataLinkResultEnum.CTAIMAError
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::SyncData::Unknown error calling webservice::" & ex.Message)
            End Try

            Return oDataLinkState
        End Function

        Public Function GetNextScheduleTime() As DateTime
            Dim nextExecutionParam As New AdvancedParameter.roAdvancedParameter((Core.DTOs.AdvancedParameterType.CTaimaNextExecution).ToString, New AdvancedParameter.roAdvancedParameterState())
            If Not nextExecutionParam.Exists Then
                Dim oNextRun As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _strCTAIMAInitialHour.Hour, _strCTAIMAInitialHour.Minute, 0)

                While oNextRun < DateTime.Now
                    oNextRun = oNextRun.AddMinutes(_strCTAIMACheckPeriod)
                End While
                nextExecutionParam.Value = oNextRun.ToString
                nextExecutionParam.Save()
                Return oNextRun
            Else
                If roTypes.Any2DateTime(nextExecutionParam.Value) >= DateTime.Now Then
                    Return roTypes.Any2DateTime(nextExecutionParam.Value)
                Else
                    Dim oNextRun As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _strCTAIMAInitialHour.Hour, _strCTAIMAInitialHour.Minute, 0)

                    While oNextRun < DateTime.Now
                        oNextRun = oNextRun.AddMinutes(_strCTAIMACheckPeriod)
                    End While
                    Dim currentExecutionTime As DateTime = roTypes.Any2DateTime(nextExecutionParam.Value)
                    nextExecutionParam.Value = oNextRun.ToString
                    nextExecutionParam.Save()
                    Return roTypes.Any2DateTime(currentExecutionTime)
                End If

            End If

        End Function

        Private Sub SyncCTAIMAprl(ByVal wsPU As BasicHttpBinding_ICT_EstadoTrabajadoresPortType)

            Dim oSol As New datosSolicitudType
            oSol.userId = Me._strCTAIMAUserName
            oSol.userClave = Me._strCTAIMAPwd

            Dim oStatus() As ArrayOfCT_EstadoTrabajadoresTypeTrabajadoresTrabajadorTrabajador = {}
            Dim bRes As bapiret2Type = wsPU.CT_EstadoTrabajadoresOperation(oSol, oStatus)

            Dim bLaunchBroadcaster As Boolean = False
            Dim bolRet As Boolean = True
            If oStatus IsNot Nothing AndAlso oStatus.Length > 0 Then
                For Each oEmployee As CTAIMA.ArrayOfCT_EstadoTrabajadoresTypeTrabajadoresTrabajadorTrabajador In oStatus
                    bolRet = UpdateUserField(oEmployee)
                    If bolRet AndAlso Not bLaunchBroadcaster Then bLaunchBroadcaster = True
                Next

                If bLaunchBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
            End If
        End Sub

        Private Function UpdateUserFieldRestful(ByVal dni As String, isAuthorized As Boolean) As Boolean
            Dim bolRet As Boolean = True
            Dim authorized As Char = IIf(isAuthorized, "S", "N")
            Dim sOldvalue As String = String.Empty

            Try

                Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", dni, Now, New UserFields.roUserFieldState)
                Dim idEmployee As Integer = -1

                If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then
                    idEmployee = tbEmployee.Rows(0).Item("IDEmployee")
                Else
                    bolRet = False
                    If _isPULogEnabled Then roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Unknown employee with NIF::" & dni)
                End If

                If bolRet Then
                    Dim bEndDate As DateTime = New DateTime(1900, 1, 1, 0, 0, 0)

                    Dim oEmployeeState As New Employee.roEmployeeState(-1)

                    Dim oCurrentUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "CTaimaPRLUserField", bEndDate, New VTUserFields.UserFields.roUserFieldState(-1), False)

                    If oCurrentUserField Is Nothing OrElse (oCurrentUserField IsNot Nothing AndAlso oCurrentUserField.FieldValue = "") Then
                        Dim oEmployeeUserField As New UserFields.roEmployeeUserField(idEmployee, Me._strCTAIMAUserFieldName, bEndDate, New UserFields.roUserFieldState)
                        sOldvalue = roTypes.Any2String(oEmployeeUserField.FieldValue)
                        oEmployeeUserField.FieldValue = authorized
                        bolRet = oEmployeeUserField.Save()

                        Me._mustLaunchBroadcaster = True

                        If _isPULogEnabled Then
                            If bolRet Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CCTAIMA::UpdateUserField::Employee with NIF(" & dni & ") updated to '" & authorized & "'. Previous value was '" & sOldvalue & "'")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Error updating employee with NIF(" & dni & ") userfield to value::'" & authorized & "'")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Unknown error updating user::" & ex.Message)
            End Try

            Return bolRet
        End Function

        Private Function UpdateUserField(ByVal oEmployee As ArrayOfCT_EstadoTrabajadoresTypeTrabajadoresTrabajadorTrabajador) As Boolean
            Dim bolRet As Boolean = True

            Dim wsResultcode As Integer = Core.DTOs.ReturnCode._UnknownError
            Dim curEmployeeID As Integer = -1

            Dim sOldvalue As String = String.Empty

            Try

                Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", oEmployee.nif, Now, New UserFields.roUserFieldState)
                Dim idEmployee As Integer = -1

                If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then
                    idEmployee = tbEmployee.Rows(0).Item("IDEmployee")
                Else
                    bolRet = False
                    If _isPULogEnabled Then roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Unknown employee with NIF::" & oEmployee.nif)
                End If

                If bolRet Then
                    Dim bCanEnter As Boolean = True
                    Dim bEndDate As DateTime = New DateTime(1900, 1, 1, 0, 0, 0)

                    For Each oEndDate In oEmployee.fechascortescentros
                        'bEndDate = roTypes.Any2DateTime(oEndDate.fechaCorte).Date

                        If roTypes.Any2DateTime(oEndDate.fechaCorte).Date < DateTime.Now.Date Then
                            bCanEnter = False
                            Exit For
                        End If
                    Next

                    Dim oEmployeeState As New Employee.roEmployeeState(-1)

                    Dim oCurrentUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, Me._strCTAIMAUserFieldName, bEndDate, New VTUserFields.UserFields.roUserFieldState(-1), False)

                    If oCurrentUserField Is Nothing OrElse (oCurrentUserField IsNot Nothing AndAlso oCurrentUserField.FieldValue <> IIf(bCanEnter, "S", "N")) Then
                        Dim oEmployeeUserField As New UserFields.roEmployeeUserField(idEmployee, Me._strCTAIMAUserFieldName, bEndDate, New UserFields.roUserFieldState)
                        sOldvalue = roTypes.Any2String(oEmployeeUserField.FieldValue)
                        oEmployeeUserField.FieldValue = IIf(bCanEnter, "S", "N")
                        bolRet = oEmployeeUserField.Save()

                        If _isPULogEnabled Then
                            If bolRet Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CCTAIMA::UpdateUserField::Employee with NIF(" & oEmployee.nif & ") updated to '" & IIf(bCanEnter, "S", "N") & "'. Previous value was '" & sOldvalue & "'")
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Error updating employee with NIF(" & oEmployee.nif & ") userfield to value::'" & IIf(bCanEnter, "S", "N") & "'")
                            End If
                        End If
                    End If

                End If
            Catch ex As Exception

                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CCTAIMA::UpdateUserField::Unknown error updating user::" & ex.Message)
            End Try

            Return bolRet
        End Function

    End Class

End Namespace