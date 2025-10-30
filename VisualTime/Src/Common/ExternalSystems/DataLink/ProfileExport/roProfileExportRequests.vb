Imports System.Data.Common
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink
    Public Class ProfileExportRequests

#Region "Declarations - Constructor"

        Private mEmployeesFilter As String = ""
        Private mCausesFilter As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody
        Private mdaDinners As DbDataAdapter = Nothing
        Private mIntervalMinutes As Integer = 0

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState

        Public Sub New(ByVal EmployeesFilter As String, ByVal CausesFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal IntervalMinutes As Integer, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6
            Me.mCausesFilter = CausesFilter

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeesFilter = EmployeesFilter
            mBeginDay = BeginDay
            mIntervalMinutes = IntervalMinutes
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

        Public Sub New(ByVal EmployeesFilter As String, ByVal CausesFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6
            Me.mCausesFilter = CausesFilter

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeesFilter = EmployeesFilter
            mBeginDate = BeginDate
            mEndDate = EndDate
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal NewValue As roDataLinkState)

            End Set
        End Property

        Public Property Profile() As ProfileExportBody
            Get
                Return Me.mBody
            End Get
            Set(ByVal value As ProfileExportBody)
                Me.mBody = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function ExportProfile() As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try
                ' Crea el dataadapter de empleados
                Dim dtEmpUsrFields As DataTable = Nothing
                Dim mbolGroupByDate As Boolean = Me.Profile.FieldExists("Fecha")

                ' Crea el dataadapter de contratos
                Dim daEmployeeContracts As DbDataAdapter = CreateDataAdapter_EmployeeContracts(True)

                ' Crea el fichero de salida
                If Not Me.Profile.FileOpen() Then Exit Try
                bolCloseFile = True

                ' Selecciona las justificaciones diarias
                Dim sSQL As String = String.Empty
                sSQL = "@SELECT# Requests.IDEmployee, " &
                    "CONVERT(date,Requests.RequestDate) RequestDate, " &
                    "sysroRequestType.Type, " &
                    "Requests.Status, " &
                    "Approvals.IDPassport SupervisorPassport, " &
                    "sysropassports.Name SupervisorName, " &
                    "Approvals.Comments ApprovalComments, " &
                    "ISNULL(Causes.Name,'') CauseName, " &
                    "ISNULL(Shifts.Name,'') ShiftName, " &
                    "'@NumDays' as NumDays, " &
                    "Date1 DateBegin, " &
                    "Date2 DateEnd, " &
                    "FromTime, " &
                    "ToTime, " &
                    "Hours TotalHours, " &
                    "Requests.Comments EmployeeComments " &
                    "FROM Requests " &
                    "INNER JOIN sysroRequestType ON sysroRequestType.IdType = Requests.RequestType " &
                    "LEFT JOIN Causes ON Causes.ID = Requests.IDCause " &
                    "LEFT JOIN Shifts ON Shifts.ID = Requests.IDShift " &
                    "LEFT JOIN (@SELECT# row_number() over (partition by IdRequest ORDER BY IdRequest ASC, DateTime DESC) As 'RowNumber1', * from RequestsApprovals) Approvals ON Approvals.IDRequest =Requests.ID and (RowNumber1 = 1 OR RowNumber1 IS NULL) " &
                    "LEFT JOIN sysroPassports ON Approvals.IDPassport = sysroPassports.id " &
                    "WHERE CONVERT(date,Requests.RequestDate) BETWEEN " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " and " & roTypes.Any2Time(Me.mEndDate).SQLDateTime &
                    " AND Requests.IDEmployee IN (" & Me.mEmployeesFilter & ")"

                Dim dtRequests As DataTable = CreateDataTableWithoutTimeouts(sSQL, , "Requests")
                Dim idEmployeeAnt As Integer = 0

                ' Crea el dataadapter de empleados
                Dim daEmployees As DbDataAdapter = CreateDataAdapter_Employees()

                Dim dtEmployeeContracts As New DataTable
                Dim dtEmployees As New DataTable
                Dim bolLoadContracts As Boolean = False

                ' Verifica si tiene que cargar los contratos
                Dim i As Integer
                For i = 0 To Me.Profile.Fields.Count - 1
                    If InStr(Profile.Fields(i).Source.ToUpper, "CONTRATO") > 0 Then
                        bolLoadContracts = True
                        Exit For
                    End If
                Next i

                ' Para cada punche
                For Each Row As DataRow In dtRequests.Rows
                    ' Si cambia el empleado carga los datos de la ficha y de empleado
                    If idEmployeeAnt <> Row("IdEmployee") Then
                        ' Lee los datos del empleado
                        dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), Me.mEndDate, Me.oState)
                        idEmployeeAnt = Row("IdEmployee")
                    End If

                    daEmployees.SelectCommand.Parameters("@idEmployee").Value = Row("idEmployee")
                    If mbolGroupByDate Then
                        daEmployees.SelectCommand.Parameters("@Date").Value = Row("Date")
                    Else
                        daEmployees.SelectCommand.Parameters("@Date").Value = Me.mEndDate
                    End If

                    dtEmployees.Rows.Clear()
                    daEmployees.Fill(dtEmployees)

                    ' Lee los datos del contrato
                    If bolLoadContracts Then
                        daEmployeeContracts.SelectCommand.Parameters("@idEmployee").Value = Row("IdEmployee")
                        If mbolGroupByDate Then
                            daEmployeeContracts.SelectCommand.Parameters("@Date").Value = Row("Date")
                        Else
                            daEmployeeContracts.SelectCommand.Parameters("@Date").Value = Me.mEndDate
                        End If

                        dtEmployeeContracts.Rows.Clear()
                        daEmployeeContracts.Fill(dtEmployeeContracts)
                    End If

                    ' Carga datos
                    If Not LoadInfo(Row, dtEmpUsrFields, dtEmployees, dtEmployeeContracts) Then Exit For

                    ' Crea la línea
                    bolRet = CreateLine(Row)
                Next

                bolRet = True

                dtEmployees.Dispose()
                dtEmployeeContracts.Dispose()
                dtRequests.Dispose()
                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportRequests:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function CreateLine(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA" : Profile.Fields(i).Value = cRow("Date")
                        Case "VALOR" : Profile.Fields(i).Value = cRow("Total")
                        Case "NC" : Profile.Fields(i).Value = Any2String(cRow("ShortName"))
                        Case "EXPORTARCOMO" : Profile.Fields(i).Value = Any2String(cRow("Export"))
                    End Select
                Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDailyCauses:CreateLine")
            End Try

            Return bolRet
        End Function

        Private Function CreateDataAdapter_Employees() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# IDEmployee, Name AS EmployeeName, GroupName, FullGroupName from Employees " &
                            "INNER JOIN sysroEmployeeGroups " &
                            " ON Employees.ID = sysroEmployeeGroups.IDEmployee " &
                            " WHERE Employees.ID = @idEmployee " &
                            " AND BeginDate <= @date and EndDate >= @date"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@date", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDailyCauses:CreateDataAdapter_Employees")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_EmployeeContracts(Optional bClearEndDate As Boolean = False) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String
                If Not bClearEndDate Then
                    strSQL =
                "@SELECT# top(1) IDContract, BeginDate, enddate from EmployeeContracts " &
                "where idEmployee=@idEmployee and @date between BeginDate and EndDate"
                Else
                    strSQL =
                "@SELECT# top(1) IDContract, BeginDate, CASE WHEN DATEPART(YEAR,EmployeeContracts.EndDate) = 2079 THEN NULL ELSE EmployeeContracts.EndDate END  enddate from EmployeeContracts " &
                "where idEmployee=@idEmployee and @date between BeginDate and EndDate"
                End If

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@date", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDailyCauses:CreateDataAdapter_EmployeeContracts")
            End Try

            Return da
        End Function

        Private Function LoadInfo(ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtEmployee As DataTable, ByVal dtEmployeeContracts As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim bolGroup1 As Boolean = False
                Dim i As Integer = 0
                Dim n As Integer = 0

                Dim dt As New DataTable

                ' Determina el empleado
                Dim idEmpleado As Long = row("idEmployee")

                ' Para cada columna
                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""

                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate
                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Profile.Fields(i).Value = mEndDate
                        Case "GRUPO"
                            If dtEmployee.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployee.Rows(0)("GroupName")
                        Case "GRUPO_COMPLETO"
                            If dtEmployee.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployee.Rows(0)("FullGroupName")
                        Case "NOMBRE"
                            If dtEmployee.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployee.Rows(0)("EmployeeName")
                        Case "CONTRATO"
                            If dtEmployeeContracts.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployeeContracts.Rows(0)("idContract")
                        Case "CONTRATO_FECHA_INICIO"
                            If dtEmployeeContracts.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployeeContracts.Rows(0)("BeginDate")
                        Case "CONTRATO_FECHA_FINAL"
                            If dtEmployeeContracts.Rows.Count > 0 Then Profile.Fields(i).Value = dtEmployeeContracts.Rows(0)("EndDate")
                        Case "SOLICITUD_FECHA"
                            Profile.Fields(i).Value = row("RequestDate")
                        Case "SOLICITUD_TIPO"
                            Profile.Fields(i).Value = oState.Language.TranslateWithDefault("roDataLinkExport.ProfileExportRequests.RequestType." & row("Type"), "", row("Type"))
                        Case "SOLICITUD_ESTADO"
                            Dim sStatus As String = roTypes.Any2String(row("Status"))
                            Profile.Fields(i).Value = oState.Language.Translate("roDataLinkExport.ProfileExportRequests.RequestStatus." & sStatus, "", sStatus)
                        Case "SOLICITUD_MOTIVO"
                            Profile.Fields(i).Value = row("CauseName")
                        Case "SOLICITUD_HORARIO"
                            Profile.Fields(i).Value = row("ShiftName")
                        Case "SOLICITUD_TOTAL_DIAS"
                            Profile.Fields(i).Value = "PENDIENTE CALCULO"
                        Case "SOLICITUD_FECHA_INICIO"
                            Profile.Fields(i).Value = row("DateBegin")
                        Case "SOLICITUD_HORA_INICIO"
                            Profile.Fields(i).Value = row("FromTime")
                        Case "SOLICITUD_FECHA_FINAL"
                            Profile.Fields(i).Value = row("DateEnd")
                        Case "SOLICITUD_HORA_FINAL"
                            Profile.Fields(i).Value = row("ToTime")
                        Case "SOLICITUD_SUPERVISOR"
                            Profile.Fields(i).Value = row("SupervisorName")
                        Case "SOLICITUD_PENDIENTE_SUPERVISOR"
                            Profile.Fields(i).Value = "PENDIENTE CALCULO"
                        Case "SOLICITUD_COMENTARIOS_EMPLEADO"
                            Profile.Fields(i).Value = row("EmployeeComments")
                        Case "SOLICITUD_COMENTARIOS_SUPERVISOR"
                            Profile.Fields(i).Value = row("ApprovalComments")
                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Profile.Fields(i).Source.ToUpper, "USR_") Then
                                ' Lee el dato del campo de la ficha
                                If dtEmpUsrFields.Rows.Count > 0 Then
                                    Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Dim r() As DataRow = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                    If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                End If
                            Else
                                ' Literal
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                    Profile.Fields(i).Value = Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8)
                                End If
                            End If
                    End Select
                Next

                bolRet = True

                dt.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::LoadInfo:LoadInfo")
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace