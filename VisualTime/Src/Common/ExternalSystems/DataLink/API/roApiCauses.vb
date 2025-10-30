Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roApiCauses
        Inherits roDataLinkApi


        Protected ReadOnly Property ImportEngine As roCausesImport
            Get
                Return CType(Me.oDataImport, roCausesImport)
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roCausesImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub


        Public Function CreateOrUpdateDailyCause(ByVal oDailyCauseData As RoboticsExternAccess.IDatalinkDailyCause, ByRef strErrorMsg As String,
                                       Optional ByVal bCallBroadcaster As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Me.State.Result = DataLinkResultEnum.Exception
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                bolRet = oDailyCauseData.GetDailyCauseColumnsDefinition(ColumnsVal, ColumnsPos)
                Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RoboticsExternAccess.DailyCauseExcelColumns.ImportPrimaryKey, RoboticsExternAccess.DailyCauseExcelColumns.NIF, New UserFields.roUserFieldState)

                If Not bolRet Then
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid employee object"
                ElseIf idEmployee < 0 Then
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                Else
                    Dim oContractState = New Contract.roContractState()
                    roBusinessState.CopyTo(Me.State, oContractState)

                    Dim oDailyCauseState = New Cause.roCauseState()
                    roBusinessState.CopyTo(Me.State, oDailyCauseState)


                    Dim freezeDate As Date = New Date(1900, 1, 1)
                    Dim msgLog As String = ""
                    Dim bolNotifyChanges As Boolean = False
                    Dim strEmployees As String = ""
                    Dim FieldName As String = ""
                    Dim intNewDailyCauses As Integer = 0

                    Dim valueDate As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseExcelColumns.CauseDate))
                    'En la API obtenemos el contrato por IDEmployee y fecha del contrato
                    Dim oContract = Contract.roContract.GetContractInDate(idEmployee, valueDate, oContractState, False)

                    'Create/Update/Delete DailyCause
                    bolRet = Me.ImportEngine.CreateDailyCause(msgLog, freezeDate, valueDate, ColumnsVal, oContract, intNewDailyCauses, oContractState, oDailyCauseState, bolNotifyChanges, FieldName, 0)
                    Me.State.Result = Me.ImportEngine.State.Result
                    If bolRet Then
                        If bCallBroadcaster And bolNotifyChanges Then
                            Extensions.roConnector.InitTask(TasksType.DAILYCAUSES)
                        End If
                        Me.State.Result = DataLinkResultEnum.NoError
                    End If
                End If
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateDailyCause")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function DeleteDailyCause(ByVal oDailyCauseData As RoboticsExternAccess.IDatalinkDailyCause, ByRef strErrorMsg As String,
                                       Optional ByVal bCallBroadcaster As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Me.State.Result = DataLinkResultEnum.Exception
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                bolRet = oDailyCauseData.GetDailyCauseColumnsDefinition(ColumnsVal, ColumnsPos)
                Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RoboticsExternAccess.DailyCauseExcelColumns.ImportPrimaryKey, RoboticsExternAccess.DailyCauseExcelColumns.NIF, New UserFields.roUserFieldState)

                If Not bolRet Then
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid employee object"
                ElseIf idEmployee < 0 Then
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                Else
                    Dim oContractState = New Contract.roContractState()
                    roBusinessState.CopyTo(Me.State, oContractState)

                    Dim oDailyCauseState = New Cause.roCauseState()
                    roBusinessState.CopyTo(Me.State, oDailyCauseState)

                    Dim freezeDate As Date = New Date(1900, 1, 1)
                    Dim msgLog As String = ""
                    Dim bolNotifyChanges As Boolean = False
                    Dim strEmployees As String = ""
                    Dim FieldName As String = ""
                    Dim intNewDailyCauses As Integer = 0

                    Dim valueDate As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseExcelColumns.CauseDate))
                    'En la API obtenemos el contrato por IDEmployee y fecha del contrato
                    Dim oContract = Contract.roContract.GetContractInDate(idEmployee, valueDate, oContractState, False)

                    'Delete DailyCause
                    bolRet = Me.ImportEngine.DeleteDailyCause(msgLog, freezeDate, valueDate, ColumnsVal, oContract, intNewDailyCauses, oContractState, oDailyCauseState, bolNotifyChanges, FieldName, 0)
                    Me.State.Result = Me.ImportEngine.State.Result
                    If bolRet Then
                        If bCallBroadcaster And bolNotifyChanges Then
                            Extensions.roConnector.InitTask(TasksType.DAILYCAUSES)
                        End If
                        Me.State.Result = DataLinkResultEnum.NoError
                    End If
                End If
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::DeleteDailyCause")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function GetCauses(ByVal oCauseCriteria As RoboticsExternAccess.IDatalinkDailyCauseCriteria, ByRef lCauses As Generic.List(Of RoboticsExternAccess.roDatalinkStandarDailyCause), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}
                Dim bForAllEmployees As Boolean = False
                Dim bForOneEmployee As Boolean = False
                Dim bolTimeStamp As Boolean = False

                lCauses = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarDailyCause)
                bolRet = oCauseCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

                bForAllEmployees = (roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

                Dim strUniqueidentifierField As String = String.Empty

                If bolRet Then
                    Dim idEmployee As Integer = 0
                    Dim lstEmployees As String = String.Empty
                    ' En el caso que el identificador de usuario tenga el caracter ";" debemos obtener cada uno de los identificadores por separado
                    ' ya que nos viene una lista de identificadores de usuarios
                    If ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey).Contains(";") Then
                        Dim tmplst As String() = ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey).Split(";")
                        For Each employee As String In tmplst
                            If employee.Length > 0 Then
                                ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey) = employee
                                idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DailyCauseAsciiColumns.ImportPrimaryKey, DailyCauseAsciiColumns.NIF, New UserFields.roUserFieldState)
                                If idEmployee > 0 Then
                                    lstEmployees += "," & idEmployee.ToString
                                Else
                                    strErrorMsg += "," & ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey)
                                End If
                            End If
                        Next
                        If strErrorMsg.Length > 0 Then strErrorMsg = strErrorMsg.Substring(1)
                        If lstEmployees.Length > 0 Then lstEmployees = lstEmployees.Substring(1)

                    Else

                        idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DailyCauseAsciiColumns.ImportPrimaryKey, DailyCauseAsciiColumns.NIF, New UserFields.roUserFieldState)
                        If idEmployee > 0 Then
                            lstEmployees = idEmployee.ToString
                            bForOneEmployee = True
                        End If
                    End If

                    If lstEmployees.Length > 0 OrElse bForAllEmployees Then

                        Dim beginDate As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseAsciiColumns.BeginPeriod))
                        Dim endDate As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseAsciiColumns.EndPeriod))

                        If Not bForOneEmployee Then
                            strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                        End If

                        bolTimeStamp = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.DailyCauseAsciiColumns.ShowChangesInPeriod))

                        ' Obtenemos las justificaciones que debemos exportar
                        Dim sSQL As String = String.Empty
                        If lstEmployees.Length > 0 AndAlso bForOneEmployee Then
                            sSQL = "@SELECT# IDCause , convert(numeric(18,6), DailyCauses.Value) as total , ShortName, Name, DailyCauses.Date, DailyCauses.Manual, IDRelatedIncidence, DailyCauses.IDEmployee, Export "
                            sSQL += " From DailyCauses WITH (NOLOCK) Inner Join Causes WITH (NOLOCK) ON DailyCauses.IDCause = Causes.ID "
                            If bolTimeStamp Then sSQL += " Inner Join DailySchedule WITH (NOLOCK) on DailySchedule.Date = DailyCauses.Date and DailySchedule.IdEmployee = DailyCauses.IDEmployee "
                            sSQL += " Where DailyCauses.IDEmployee in( " & lstEmployees.ToString & ") "
                            If bolTimeStamp Then
                                sSQL += " and DailySchedule.TimestampEngine >= " & Any2Time(beginDate).SQLSmallDateTime & " and DailySchedule.TimestampEngine <=" & Any2Time(endDate).SQLSmallDateTime
                            Else
                                sSQL += " and DailyCauses.Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and DailyCauses.Date <=" & Any2Time(endDate).SQLSmallDateTime
                            End If
                            sSQL += " Order By DailyCauses.Date, IDCause "
                        Else
                            sSQL = "@SELECT# IDCause, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, dailycauses.idemployee, convert(numeric(18,6), dailycauses.Value) as total , ShortName, Name, dailycauses.Date, Manual, IDRelatedIncidence, Export "
                            sSQL += " From DailyCauses WITH (NOLOCK) INNER JOIN Causes WITH (NOLOCK) ON causes.id = dailycauses.idcause "
                            If bolTimeStamp Then sSQL += " Inner Join DailySchedule WITH (NOLOCK) on DailySchedule.Date = DailyCauses.Date and DailySchedule.IdEmployee = DailyCauses.IDEmployee "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = dailycauses.IDEmployee AND NifTable.Date < GETDATE() "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = dailycauses.IDEmployee AND IdTable.Date < GETDATE()"
                            sSQL += " Where 1=1  "
                            If Not bForAllEmployees Then
                                sSQL += " AND Dailycauses.IDEmployee in( " & lstEmployees.ToString & ")  "
                            End If
                            If bolTimeStamp Then
                                sSQL += " and DailySchedule.TimestampEngine >= " & Any2Time(beginDate).SQLSmallDateTime & " and DailySchedule.TimestampEngine <=" & Any2Time(endDate).SQLSmallDateTime
                            Else
                                sSQL += " and dailycauses.Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and dailycauses.Date <=" & Any2Time(endDate).SQLSmallDateTime
                            End If
                            sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                            sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                            sSQL += " Order By dailycauses.idemployee, DailyCauses.Date, IDCause"
                        End If

                        Dim tbCauses As DataTable = CreateDataTableWithoutTimeouts(sSQL)

                        If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                            For Each oRow As DataRow In tbCauses.Rows
                                Dim oDatalinkStandarCause As New RoboticsExternAccess.roDatalinkStandarDailyCause
                                If Not bForOneEmployee Then
                                    oDatalinkStandarCause.NifEmpleado = Any2String(oRow("NIF"))
                                    oDatalinkStandarCause.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                Else
                                    oDatalinkStandarCause.NifEmpleado = ColumnsVal(RoboticsExternAccess.DailyCauseAsciiColumns.NIF)
                                    oDatalinkStandarCause.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.DailyCauseAsciiColumns.ImportPrimaryKey)
                                End If
                                oDatalinkStandarCause.CauseShortName = Any2String(oRow("ShortName"))
                                oDatalinkStandarCause.CauseDate = oRow("Date")
                                Dim causeValue As Double = Any2Double(oRow("total")) * 60
                                oDatalinkStandarCause.CauseValue = Math.Round(causeValue)
                                oDatalinkStandarCause.Incidence = Any2Double(oRow("IDRelatedIncidence"))
                                oDatalinkStandarCause.Manual = Any2Boolean(oRow("Manual"))
                                oDatalinkStandarCause.CauseEquivalenceCode = Any2String(oRow("Export"))
                                oDatalinkStandarCause.IncidenceData = Nothing

                                If roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.AddRelatedIncidence)).Trim = "1" Then
                                    ' Si es necesario añadimos la info de la incidencia relacionada
                                    oDatalinkStandarCause.IncidenceData = New roDatalinkStandarDailyIncidence
                                    If oDatalinkStandarCause.Incidence > 0 Then
                                        sSQL = "@SELECT# DailyIncidences.ID AS IDIncidence, Date, IDType, IDZone, convert(numeric(18,6),Value) AS Total , BeginTime, EndTime , TimeZones.Name as ZoneName FROM DailyIncidences WITH (NOLOCK) INNER JOIN TimeZones WITH (NOLOCK) ON TimeZones.id = DailyIncidences.IDZone  "
                                        sSQL += " WHERE DailyIncidences.IDEmployee= " & Any2Double(oRow("IDEmployee")).ToString
                                        sSQL += " AND DailyIncidences.Date= " & Any2Time(oRow("Date")).SQLSmallDateTime
                                        sSQL += " AND DailyIncidences.ID= " & Any2Double(oRow("IDRelatedIncidence")).ToString
                                        Dim tbIncidence As DataTable = CreateDataTableWithoutTimeouts(sSQL)
                                        If tbIncidence IsNot Nothing AndAlso tbIncidence.Rows.Count = 1 Then
                                            oDatalinkStandarCause.IncidenceData.Incidence = Any2Double(tbIncidence.Rows(0)("IDIncidence"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceDate = tbIncidence.Rows(0)("Date")
                                            oDatalinkStandarCause.IncidenceData.IncidenceType = Any2Integer(tbIncidence.Rows(0)("IDType"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceBeginTime = tbIncidence.Rows(0)("BeginTime")
                                            oDatalinkStandarCause.IncidenceData.IncidenceEndTime = tbIncidence.Rows(0)("EndTime")
                                            oDatalinkStandarCause.IncidenceData.IncidenceZone = Any2String(tbIncidence.Rows(0)("ZoneName"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceValue = Math.Round(Any2Double(tbIncidence.Rows(0)("total")) * 60)
                                        End If
                                    End If


                                End If
                                lCauses.Add(oDatalinkStandarCause)
                            Next
                        End If

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        bolRet = False
                        Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid cause object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport:: GetCauses")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        Public Function GetCausesByTimestamp(ByVal oCauseCriteria As RoboticsExternAccess.IDatalinkDailyCauseCriteria, ByRef lCauses As Generic.List(Of RoboticsExternAccess.roDatalinkStandarDailyCause), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}
                Dim bForAllEmployees As Boolean = False
                Dim bForOneEmployee As Boolean = False

                lCauses = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarDailyCause)
                bolRet = oCauseCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

                bForAllEmployees = (roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

                Dim strUniqueidentifierField As String = String.Empty

                If bolRet Then
                    Dim idEmployee As Integer = 0
                    Dim lstEmployees As String = String.Empty
                    ' En el caso que el identificador de usuario tenga el caracter ";" debemos obtener cada uno de los identificadores por separado
                    ' ya que nos viene una lista de identificadores de usuarios
                    If ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey).Contains(";") Then
                        Dim tmplst As String() = ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey).Split(";")
                        For Each employee As String In tmplst
                            If employee.Length > 0 Then
                                ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey) = employee
                                idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DailyCauseAsciiColumns.ImportPrimaryKey, DailyCauseAsciiColumns.NIF, New UserFields.roUserFieldState)
                                If idEmployee > 0 Then
                                    lstEmployees += "," & idEmployee.ToString
                                Else
                                    strErrorMsg += "," & ColumnsVal(DailyCauseAsciiColumns.ImportPrimaryKey)
                                End If
                            End If
                        Next
                        If strErrorMsg.Length > 0 Then strErrorMsg = strErrorMsg.Substring(1)
                        If lstEmployees.Length > 0 Then lstEmployees = lstEmployees.Substring(1)

                    Else

                        idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DailyCauseAsciiColumns.ImportPrimaryKey, DailyCauseAsciiColumns.NIF, New UserFields.roUserFieldState)
                        If idEmployee > 0 Then
                            lstEmployees = idEmployee.ToString
                            bForOneEmployee = True
                        End If
                    End If

                    If lstEmployees.Length > 0 OrElse bForAllEmployees Then

                        Dim timestamp As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseAsciiColumns.Timestamp))

                        If Not bForOneEmployee Then
                            strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                        End If


                        ' Obtenemos las justificaciones que debemos exportar
                        Dim sSQL As String = String.Empty
                        If lstEmployees.Length > 0 AndAlso bForOneEmployee Then
                            sSQL = "@SELECT# IDCause , convert(numeric(18,6), Value) as total , ShortName, Name, DailyCauses.Date, Manual, IDRelatedIncidence, dailycauses.idemployee, Export "
                            sSQL += " From DailyCauses WITH (NOLOCK) INNER JOIN Causes WITH (NOLOCK) ON causes.id = dailycauses.idcause  "
                            sSQL += " Inner Join DailySchedule WITH (NOLOCK) on DailySchedule.Date = DailyCauses.Date and DailySchedule.IdEmployee = DailyCauses.IDEmployee "
                            sSQL += " Where DailyCauses.IDEmployee in( " & lstEmployees.ToString & ") "
                            sSQL += " and Causes.ID = DailyCauses.IDCause"
                            sSQL += " and DailySchedule.TimestampEngine >= " & Any2Time(timestamp).SQLSmallDateTime
                            'sSQL += " Group By IDCause, ShortName, Name, DailyCauses.Date, Manual, IDRelatedIncidence"
                            sSQL += " Order By dailycauses.Date, dailycauses.IDCause "
                        Else
                            sSQL = "@SELECT# IDCause, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, dailycauses.idemployee, convert(numeric(18,6), dailycauses.Value) as total , ShortName, Name, dailycauses.Date, Manual, IDRelatedIncidence, Export "
                            sSQL += " From DailyCauses WITH (NOLOCK) INNER JOIN Causes WITH (NOLOCK) ON causes.id = dailycauses.idcause "
                            sSQL += " Inner Join DailySchedule WITH (NOLOCK) on DailySchedule.Date = DailyCauses.Date and DailySchedule.IdEmployee = DailyCauses.IDEmployee "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = dailycauses.IDEmployee AND NifTable.Date < GETDATE() "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = dailycauses.IDEmployee AND IdTable.Date < GETDATE()"
                            sSQL += " Where "
                            If Not bForAllEmployees Then
                                sSQL += " Dailycauses.IDEmployee in( " & lstEmployees.ToString & ") AND "
                            End If
                            sSQL += " DailySchedule.TimestampEngine >= " & Any2Time(timestamp).SQLSmallDateTime
                            sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                            sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                            'sSQL += " Group By dailycauses.idemployee, CONVERT(VARCHAR,NifTable.Value), CONVERT(VARCHAR,IdTable.Value), IDCause, ShortName, Name, dailycauses.Date, Manual, IDRelatedIncidence"
                            sSQL += " Order By dailycauses.idemployee, dailycauses.Date, dailycauses.IDCause"
                        End If

                        Dim tbCauses As DataTable = CreateDataTableWithoutTimeouts(sSQL)

                        If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                            For Each oRow As DataRow In tbCauses.Rows
                                Dim oDatalinkStandarCause As New RoboticsExternAccess.roDatalinkStandarDailyCause
                                If Not bForOneEmployee Then
                                    oDatalinkStandarCause.NifEmpleado = Any2String(oRow("NIF"))
                                    oDatalinkStandarCause.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                Else
                                    oDatalinkStandarCause.NifEmpleado = ColumnsVal(RoboticsExternAccess.DailyCauseAsciiColumns.NIF)
                                    oDatalinkStandarCause.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.DailyCauseAsciiColumns.ImportPrimaryKey)
                                End If
                                oDatalinkStandarCause.CauseShortName = Any2String(oRow("ShortName"))
                                oDatalinkStandarCause.CauseDate = oRow("Date")
                                Dim causeValue As Double = Any2Double(oRow("total")) * 60
                                oDatalinkStandarCause.CauseValue = Math.Round(causeValue)
                                oDatalinkStandarCause.Incidence = Any2Double(oRow("IDRelatedIncidence"))
                                oDatalinkStandarCause.Manual = Any2Boolean(oRow("Manual"))
                                oDatalinkStandarCause.CauseEquivalenceCode = Any2String(oRow("Export"))
                                oDatalinkStandarCause.IncidenceData = Nothing

                                If roTypes.Any2String(ColumnsVal(DailyCauseAsciiColumns.AddRelatedIncidence)).Trim = "1" Then
                                    ' Si es necesario añadimos la info de la incidencia relacionada
                                    oDatalinkStandarCause.IncidenceData = New roDatalinkStandarDailyIncidence
                                    If oDatalinkStandarCause.Incidence > 0 Then
                                        sSQL = "@SELECT# DailyIncidences.ID AS IDIncidence, Date, IDType, IDZone, convert(numeric(18,6),Value) AS Total , BeginTime, EndTime , TimeZones.Name as ZoneName FROM DailyIncidences WITH (NOLOCK) INNER JOIN TimeZones WITH (NOLOCK) ON TimeZones.id = DailyIncidences.IDZone  "
                                        sSQL += " WHERE DailyIncidences.IDEmployee= " & Any2Double(oRow("IDEmployee")).ToString
                                        sSQL += " AND DailyIncidences.Date= " & Any2Time(oRow("Date")).SQLSmallDateTime
                                        sSQL += " AND DailyIncidences.ID= " & Any2Double(oRow("IDRelatedIncidence")).ToString
                                        Dim tbIncidence As DataTable = CreateDataTableWithoutTimeouts(sSQL)
                                        If tbIncidence IsNot Nothing AndAlso tbIncidence.Rows.Count = 1 Then
                                            oDatalinkStandarCause.IncidenceData.Incidence = Any2Double(tbIncidence.Rows(0)("IDIncidence"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceDate = tbIncidence.Rows(0)("Date")
                                            oDatalinkStandarCause.IncidenceData.IncidenceType = Any2Integer(tbIncidence.Rows(0)("IDType"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceBeginTime = tbIncidence.Rows(0)("BeginTime")
                                            oDatalinkStandarCause.IncidenceData.IncidenceEndTime = tbIncidence.Rows(0)("EndTime")
                                            oDatalinkStandarCause.IncidenceData.IncidenceZone = Any2String(tbIncidence.Rows(0)("ZoneName"))
                                            oDatalinkStandarCause.IncidenceData.IncidenceValue = Math.Round(Any2Double(tbIncidence.Rows(0)("total")) * 60)
                                        End If
                                    End If
                                End If

                                lCauses.Add(oDatalinkStandarCause)
                            Next
                        End If

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        bolRet = False
                        Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid cause object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport:: GetCausesByTimestamp")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function



    End Class

End Namespace