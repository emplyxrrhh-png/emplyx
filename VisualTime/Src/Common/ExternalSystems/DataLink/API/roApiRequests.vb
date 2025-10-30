Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roApiRequests
        Inherits roDataLinkApi

        Protected ReadOnly Property ImportEngine As roEmployeeImport
            Get
                Return CType(Me.oDataImport, roEmployeeImport)
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roEmployeeImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub

        Public Function GetRequests(ByVal oRequestCriteria As RoboticsExternAccess.IDatalinkRequestCriteria, ByRef lRequests As Generic.List(Of RoboticsExternAccess.roDatalinkStandarRequest), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}
                Dim bForAllEmployees As Boolean = False
                Dim oRequestState As roRequestState = New roRequestState()

                lRequests = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarRequest)
                bolRet = oRequestCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

                bForAllEmployees = (roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

                Dim strUniqueidentifierField As String = String.Empty

                If bolRet Then
                    Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RequestCriteriaAsciiColumns.ImportPrimaryKey, RequestCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                    If idEmployee > 0 OrElse bForAllEmployees Then

                        Dim beginDate As Date = roTypes.Any2DateTime(ColumnsVal(RequestCriteriaAsciiColumns.BeginPeriod))
                        Dim endDate As Date = roTypes.Any2DateTime(ColumnsVal(RequestCriteriaAsciiColumns.EndPeriod))

                        If bForAllEmployees Then
                            strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                        End If

                        ' Obtenemos las solicitudes que debemos exportar
                        Dim sSQL As String = String.Empty
                        If idEmployee > 0 Then
                            sSQL = "@SELECT# r.ID, r.requestType, r.RequestDate, r.Status, r.Date1, r.Date2, r.IDCause, r.IDShift, r.Comments, r.FieldName, r.FieldValue, r.Hours,(@SELECT# top 1 Value from EmployeeUserFieldValues where IDEmployee = r.IDEmployeeExchange and EmployeeUserFieldValues.Date < GETDATE() AND FieldName = 'Id importación' order by Date desc) AS IdImportExchange, r.IDEmployeeExchange, r.StartShift, r.FromTime, r.ToTime, r.IDCenter " &
                                     "FROM Requests r WITH (NOLOCK) " &
                                     " WHERE r.IDEmployee = " & idEmployee.ToString & "  "

                            If roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.Type)).Trim <> String.Empty Then sSQL += " And RequestType like '" & roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.Type)).Trim.Replace("'", "''") & "'"

                            sSQL += " AND r.RequestDate >=" & Any2Time(beginDate).SQLSmallDateTime & " and r.RequestDate <=" & Any2Time(endDate).SQLSmallDateTime
                            sSQL += " Order By RequestDate "
                        Else
                            sSQL = "@SELECT# r.ID, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, r.idemployee, r.requestType, r.RequestDate, r.Status, r.Date1, r.Date2, r.IDCause, r.IDShift, r.Comments, r.FieldName, r.FieldValue, r.Hours,(@SELECT# top 1 Value from EmployeeUserFieldValues where IDEmployee = r.IDEmployeeExchange and EmployeeUserFieldValues.Date < GETDATE() AND FieldName = 'Id importación' order by Date desc) AS IdImportExchange, r.IDEmployeeExchange, r.StartShift, r.FromTime, r.ToTime, r.IDCenter "
                            sSQL += " From Requests r WITH (NOLOCK) "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = r.IDEmployee AND NifTable.Date < GETDATE() "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = r.IDEmployee AND IdTable.Date < GETDATE()"
                            sSQL += " Where "
                            sSQL += "  r.RequestDate >=" & Any2Time(beginDate).SQLSmallDateTime & " and r.RequestDate <=" & Any2Time(endDate).SQLSmallDateTime
                            If roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.Type)).Trim <> String.Empty Then sSQL += " And RequestType like '" & roTypes.Any2String(ColumnsVal(RequestCriteriaAsciiColumns.Type)).Trim.Replace("'", "''") & "'"
                            sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                            sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                            sSQL += " Order By r.idemployee, r.RequestDate"
                        End If

                        Dim tbRequests As DataTable = CreateDataTable(sSQL)

                        If tbRequests IsNot Nothing AndAlso tbRequests.Rows.Count > 0 Then
                            For Each oRow As DataRow In tbRequests.Rows
                                Dim oDatalinkStandarRequest As New RoboticsExternAccess.roDatalinkStandarRequest
                                If bForAllEmployees Then
                                    oDatalinkStandarRequest.NifEmpleado = Any2String(oRow("NIF"))
                                    oDatalinkStandarRequest.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                Else
                                    oDatalinkStandarRequest.NifEmpleado = ColumnsVal(RoboticsExternAccess.RequestCriteriaAsciiColumns.NIF)
                                    oDatalinkStandarRequest.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.RequestCriteriaAsciiColumns.ImportPrimaryKey)
                                End If
                                'Asignamos valor a todos los parametros de oDatalinkStandarRequest
                                oDatalinkStandarRequest.RequestType = Any2String(oRow("requestType"))
                                oDatalinkStandarRequest.RequestDate = Any2DateTime(oRow("RequestDate"))
                                oDatalinkStandarRequest.Status = Any2String(oRow("Status"))
                                oDatalinkStandarRequest.Date1 = Any2DateTime(oRow("Date1"))
                                oDatalinkStandarRequest.Date2 = Any2DateTime(oRow("Date2"))
                                oDatalinkStandarRequest.IDCause = Any2Integer(oRow("IDCause"))
                                oDatalinkStandarRequest.IDShift = Any2Integer(oRow("IDShift"))
                                oDatalinkStandarRequest.Comments = Any2String(oRow("Comments"))
                                oDatalinkStandarRequest.FieldName = Any2String(oRow("FieldName"))
                                oDatalinkStandarRequest.FieldValue = Any2String(oRow("FieldValue"))
                                oDatalinkStandarRequest.Hours = Any2Double(oRow("Hours"))
                                oDatalinkStandarRequest.IDEmployeeExchange = Any2Integer(oRow("IdImportExchange")) 'Asignamos el id importación del empleado
                                oDatalinkStandarRequest.StartShift = Any2DateTime(oRow("StartShift"))
                                oDatalinkStandarRequest.FromTime = Any2DateTime(oRow("FromTime"))
                                oDatalinkStandarRequest.ToTime = Any2DateTime(oRow("ToTime"))
                                oDatalinkStandarRequest.IDCenter = Any2Integer(oRow("IDCenter"))

                                Dim oReqDays As List(Of roRequestDay) = roRequestDay.GetRequestDays(Any2Integer(oRow("ID")), oRequestState)
                                oDatalinkStandarRequest.HolidaysDays = oReqDays.ConvertAll(AddressOf XRequestDaysToStandardConverter).ToList

                                Dim oReqApprovals As List(Of roRequestApproval) = roRequestApproval.GetRequestApprovals(Any2Integer(oRow("ID")), oRequestState)
                                oDatalinkStandarRequest.Approvals = oReqApprovals.ConvertAll(AddressOf XRequestApprovalsToStandardConverter).ToList

                                ' Añadimos la solicitud a la lista
                                lRequests.Add(oDatalinkStandarRequest)
                            Next
                        End If

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        bolRet = False
                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid request object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport:: GetRequests")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Private Function XRequestDaysToStandardConverter(oRequestDays As VTRequests.Requests.roRequestDay) As roRequestDayStandard
            Dim oRet As roRequestDayStandard
            Try
                oRet = New roRequestDayStandard()
                oRet.RequestDate = New Robotics.VTBase.roWCFDate(oRequestDays.RequestDate)
                If oRequestDays.AllDay IsNot Nothing Then oRet.AllDay = oRequestDays.AllDay
                If oRequestDays.FromTime IsNot Nothing Then oRet.FromTime = New Robotics.VTBase.roWCFDate(oRequestDays.FromTime)
                If oRequestDays.ToTime IsNot Nothing Then oRet.ToTime = New Robotics.VTBase.roWCFDate(oRequestDays.ToTime)
                If oRequestDays.Duration IsNot Nothing Then oRet.Duration = oRequestDays.Duration
                If oRequestDays.ActualType IsNot Nothing Then oRet.ActualType = oRequestDays.ActualType
                If oRequestDays.IDCause IsNot Nothing Then oRet.IDCause = oRequestDays.IDCause
                If oRequestDays.Comments IsNot Nothing Then oRet.Comments = oRequestDays.Comments
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

        Private Function XRequestApprovalsToStandardConverter(oRequestApprovals As VTRequests.Requests.roRequestApproval) As roRequestApprovalStandard
            Dim oRet As roRequestApprovalStandard
            Try
                oRet = New roRequestApprovalStandard()
                oRet.ApprovalDateTime = New roWCFDate(oRequestApprovals.ApprovalDateTime)
                oRet.Comments = oRequestApprovals.Comments
                oRet.IDPassport = oRequestApprovals.IDPassport
                oRet.Status = oRequestApprovals.Status
                oRet.StatusLevel = oRequestApprovals.StatusLevel
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

    End Class
End Namespace
