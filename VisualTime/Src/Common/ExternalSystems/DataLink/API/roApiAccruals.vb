Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roApiAccruals
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

        Public Function GetAccruals(ByVal oAccrualCriteria As RoboticsExternAccess.IDatalinkAccrualCriteria, ByRef lAccruals As Generic.List(Of RoboticsExternAccess.roDatalinkStandarAccrual), ByRef strErrorMsg As String, ByRef iReturnCode As Integer, Optional ByVal bolToDate As Boolean = False) As Boolean
        Dim bolRet As Boolean = False

        Try
            Dim ColumnsVal As String() = {}
            Dim ColumnsPos As Integer() = {}
            Dim bForAllEmployees As Boolean = False
            Dim bForOneEmployee As Boolean = False

            lAccruals = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarAccrual)
            bolRet = oAccrualCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

            bForAllEmployees = (roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(AbsencesCriteriaAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

            Dim strUniqueidentifierField As String = String.Empty

            If bolRet Then
                Dim idEmployee As Integer = 0
                Dim lstEmployees As String = String.Empty
                ' En el caso que el identificador de usuario tenga el caracter ";" debemos obtener cada uno de los identificadores por separado
                ' ya que nos viene una lista de identificadores de usuarios
                If ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey).Contains(";") Then
                    Dim tmplst As String() = ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey).Split(";")
                    For Each employee As String In tmplst
                        If employee.Length > 0 Then
                            ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey) = employee
                                idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, AccrualsCriteriaAsciiColumns.ImportPrimaryKey, AccrualsCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                                If idEmployee > 0 Then
                                lstEmployees += "," & idEmployee.ToString
                            Else
                                strErrorMsg += "," & ColumnsVal(AccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                            End If
                        End If
                    Next
                    If strErrorMsg.Length > 0 Then strErrorMsg = strErrorMsg.Substring(1)
                    If lstEmployees.Length > 0 Then lstEmployees = lstEmployees.Substring(1)

                Else

                        idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, AccrualsCriteriaAsciiColumns.ImportPrimaryKey, AccrualsCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                        If idEmployee > 0 Then
                        lstEmployees = idEmployee.ToString
                        bForOneEmployee = True
                    End If
                End If

                If lstEmployees.Length > 0 OrElse bForAllEmployees Then

                    Dim beginDate As Date = roTypes.Any2DateTime(ColumnsVal(AccrualsCriteriaAsciiColumns.BeginPeriod))
                    Dim endDate As Date = roTypes.Any2DateTime(ColumnsVal(AccrualsCriteriaAsciiColumns.EndPeriod))

                    If Not bForOneEmployee Then
                        strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                    End If

                    ' Obtenemos los saldos que debemos exportar
                    Dim sSQL As String = String.Empty
                    If Not bolToDate Then
                        If lstEmployees.Length > 0 AndAlso bForOneEmployee Then
                            sSQL = "@SELECT# IDConcept , convert(numeric(18,6), sum(Value)) as total , ShortName, Name, Export, Date "
                            sSQL += " From DailyAccruals, Concepts "
                            sSQL += " Where IDEmployee in( " & lstEmployees.ToString & ") "
                            sSQL += " and Concepts.ID = DailyAccruals.IDConcept"
                            sSQL += " And Concepts.Export is not null and Concepts.Export <> '0' "
                            sSQL += " and Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and Date <=" & Any2Time(endDate).SQLSmallDateTime
                            sSQL += " Group By IDConcept, ShortName, Name, Export, Date"
                            sSQL += " Order By IDConcept, Date"
                        Else
                            sSQL = "@SELECT# IDConcept, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, dailyaccruals.idemployee, convert(numeric(18,6), sum(dailyaccruals.Value)) as total , ShortName, Name, Export, dailyaccruals.Date "
                            sSQL += " From DailyAccruals INNER JOIN Concepts ON concepts.id = dailyaccruals.idconcept "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = dailyaccruals.IDEmployee AND NifTable.Date < GETDATE() "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = dailyaccruals.IDEmployee AND IdTable.Date < GETDATE()"
                            sSQL += " Where "
                            If Not bForAllEmployees Then
                                sSQL += " Dailyaccruals.IDEmployee in( " & lstEmployees.ToString & ") AND "
                            End If
                            sSQL += " Concepts.Export is not null and Concepts.Export <> '0' "
                            sSQL += " and dailyaccruals.Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and dailyaccruals.Date <=" & Any2Time(endDate).SQLSmallDateTime
                            sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                            sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                            sSQL += " Group By dailyaccruals.idemployee, CONVERT(VARCHAR,NifTable.Value), CONVERT(VARCHAR,IdTable.Value), IDConcept, ShortName, Name, Export, dailyaccruals.Date"
                            sSQL += " Order By dailyaccruals.idemployee, IDConcept, Date"
                        End If

                        Dim tbAccruals As DataTable = CreateDataTableWithoutTimeouts(sSQL)

                        If tbAccruals IsNot Nothing AndAlso tbAccruals.Rows.Count > 0 Then
                            For Each oRow As DataRow In tbAccruals.Rows
                                Dim oDatalinkStandarAccrual As New RoboticsExternAccess.roDatalinkStandarAccrual
                                If Not bForOneEmployee Then
                                    oDatalinkStandarAccrual.NifEmpleado = Any2String(oRow("NIF"))
                                    oDatalinkStandarAccrual.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                Else
                                    oDatalinkStandarAccrual.NifEmpleado = ColumnsVal(RoboticsExternAccess.AccrualsCriteriaAsciiColumns.NIF)
                                    oDatalinkStandarAccrual.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.AccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                                End If
                                oDatalinkStandarAccrual.AccrualExportKey = Any2String(oRow("Export"))
                                oDatalinkStandarAccrual.AccrualShortName = Any2String(oRow("ShortName"))
                                oDatalinkStandarAccrual.AccrualDate = oRow("Date")
                                oDatalinkStandarAccrual.AccrualValue = Any2Double(oRow("total"))

                                ' Añadimos el acumulado diario a la lista
                                lAccruals.Add(oDatalinkStandarAccrual)
                            Next
                        End If

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        If lstEmployees.Length > 0 AndAlso bForOneEmployee Then
                            sSQL = "@SELECT# ID as IDConcept, Name as ConceptName, ShortName, IDType as Type, DefaultQuery, Concepts.Export FROM Concepts WHERE Concepts.Export is not null and Concepts.Export <> '0'"
                            sSQL += " Order By Concepts.ID"
                        Else
                            sSQL = "@SELECT# Convert(VARCHAR, NifTable.Value) As NIF, CONVERT(VARCHAR,IdTable.Value) As IdImport, employees.ID As EmployeeID, employees.Name As EmployeeName,  Concepts.ID As IDConcept, Concepts.Name As ConceptName, Concepts.ShortName, IDType As Type, DefaultQuery, Concepts.Export FROM Concepts "
                            sSQL += " CROSS Join  employees "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = employees.ID AND NifTable.Date < GETDATE() "
                            sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = employees.ID AND IdTable.Date < GETDATE()"
                            sSQL += " WHERE Concepts.Export Is Not null And Concepts.Export <> '0'"
                            If Not bForAllEmployees Then
                                If lstEmployees.Length = 0 Then lstEmployees = "-1"
                                sSQL += " AND employees.ID in( " & lstEmployees.ToString & ")  "
                            End If
                            sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                            sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                            sSQL += " Order By employees.ID, Concepts.ID"

                        End If

                        Dim dt As DataTable = CreateDataTableWithoutTimeouts(sSQL)

                        Dim value As Double
                        Dim oConcept As Concept.roConcept
                        Dim oConceptState As New Concept.roConceptState(-1)
                        Dim oParams As New roParameters("OPTIONS", True)

                        For Each oRow In dt.Rows
                            oConcept = New Concept.roConcept
                            oConcept.ID = oRow("IDConcept")
                            oConcept.DefaultQuery = Any2String(oRow("DefaultQuery"))
                            Dim oDatalinkStandarAccrual As New RoboticsExternAccess.roDatalinkStandarAccrual
                            If bForOneEmployee Then
                                oDatalinkStandarAccrual.NifEmpleado = ColumnsVal(RoboticsExternAccess.AccrualsCriteriaAsciiColumns.NIF)
                                oDatalinkStandarAccrual.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.AccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                            Else
                                oDatalinkStandarAccrual.NifEmpleado = Any2String(oRow("NIF"))
                                oDatalinkStandarAccrual.UniqueEmployeeID = Any2String(oRow("IdImport"))
                                idEmployee = Any2Integer(oRow("EmployeeID"))
                            End If

                            value = Concept.roConcept.GetAccrualValueOnDate(idEmployee, oParams, roTypes.Any2DateTime(ColumnsVal(AccrualsCriteriaAsciiColumns.AtDate)), False, oConcept, Nothing)

                            oDatalinkStandarAccrual.AccrualExportKey = Any2String(oRow("Export"))
                            oDatalinkStandarAccrual.AccrualShortName = Any2String(oRow("ShortName"))
                            oDatalinkStandarAccrual.AccrualDate = roTypes.Any2DateTime(ColumnsVal(AccrualsCriteriaAsciiColumns.AtDate))
                            oDatalinkStandarAccrual.AccrualValue = value

                            ' Añadimos el acumulado a fecha a la lista
                            lAccruals.Add(oDatalinkStandarAccrual)
                        Next

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    End If
                Else
                    bolRet = False
                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                End If
            Else
                Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                strErrorMsg = "Invalid accrual object"
            End If
        Catch ex As Exception
            Me.State.UpdateStateInfo(ex, "roDataLinkImport:: GetAccruals")
            bolRet = False
        Finally

        End Try

        Return bolRet
    End Function

    Public Function GetTaskAccruals(ByVal oAccrualCriteria As RoboticsExternAccess.IDatalinkTaskAccrualCriteria, ByRef lAccruals As Generic.List(Of RoboticsExternAccess.roDatalinkStandarTaskAccrual), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
        Dim bolRet As Boolean = False

        Try
            Dim ColumnsVal As String() = {}
            Dim ColumnsPos As Integer() = {}
            Dim bForAllEmployees As Boolean = False

            lAccruals = New Generic.List(Of RoboticsExternAccess.roDatalinkStandarTaskAccrual)
            bolRet = oAccrualCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

            bForAllEmployees = (roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

            Dim strUniqueidentifierField As String = String.Empty

            If bolRet Then
                    Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey, TaskAccrualsCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                    If idEmployee > 0 OrElse bForAllEmployees Then

                    Dim beginDate As Date = roTypes.Any2DateTime(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.BeginPeriod))
                    Dim endDate As Date = roTypes.Any2DateTime(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.EndPeriod))

                    If bForAllEmployees Then
                        strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                    End If

                    ' Obtenemos los saldos que debemos exportar
                    Dim sSQL As String = String.Empty
                    If idEmployee > 0 Then

                        sSQL = "@SELECT# DailyTaskAccruals.IDTask, Tasks.Project,  Tasks.Name,DailyTaskAccruals.Date , ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total " &
                                     "FROM DailyTaskAccruals WITH (NOLOCK) INNER JOIN Tasks WITH (NOLOCK) " &
                                     "ON DailyTaskAccruals.IDTask = Tasks.ID " &
                                     " WHERE DailyTaskAccruals.IDEmployee = " & idEmployee.ToString & "  "

                        If roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Project)).Trim <> String.Empty Then sSQL += " And Project like '" & roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Project)).Trim.Replace("'", "''") & "'"
                        If roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Task)).Trim <> String.Empty Then sSQL += " And Name like '" & roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Task)).Trim.Replace("'", "''") & "'"

                        sSQL += " AND DailyTaskAccruals.Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and DailyTaskAccruals.Date <=" & Any2Time(endDate).SQLSmallDateTime
                        sSQL &= " GROUP By DailyTaskAccruals.IDTask, Tasks.Project, Tasks.Name, DailyTaskAccruals.Date "
                        sSQL += " Order By Date, IDTask "
                    Else
                        sSQL = "@SELECT# IDTask, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, DailyTaskAccruals.idemployee, convert(numeric(18,6), sum(DailyTaskAccruals.Value)) as total , Project, Name,  DailyTaskAccruals.Date "
                        sSQL += " From DailyTaskAccruals WITH (NOLOCK) INNER JOIN Tasks WITH (NOLOCK) ON Tasks.id = DailyTaskAccruals.idTask "
                        sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = DailyTaskAccruals.IDEmployee AND NifTable.Date < GETDATE() "
                        sSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = DailyTaskAccruals.IDEmployee AND IdTable.Date < GETDATE()"
                        sSQL += " Where "
                        sSQL += "  DailyTaskAccruals.Date >=" & Any2Time(beginDate).SQLSmallDateTime & " and DailyTaskAccruals.Date <=" & Any2Time(endDate).SQLSmallDateTime
                        If roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Project)).Trim <> String.Empty Then sSQL += " And Project like '" & roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Project)).Trim.Replace("'", "''") & "'"
                        If roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Task)).Trim <> String.Empty Then sSQL += " And Name like '" & roTypes.Any2String(ColumnsVal(TaskAccrualsCriteriaAsciiColumns.Task)).Trim.Replace("'", "''") & "'"
                        sSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                        sSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                        sSQL += " Group By DailyTaskAccruals.idemployee, CONVERT(VARCHAR,NifTable.Value), CONVERT(VARCHAR,IdTable.Value), IDTask, Project, Name,  DailyTaskAccruals.Date"
                        sSQL += " Order By DailyTaskAccruals.idemployee, Date, IDTask"
                    End If

                    Dim tbAccruals As DataTable = CreateDataTable(sSQL)

                    If tbAccruals IsNot Nothing AndAlso tbAccruals.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbAccruals.Rows
                            Dim oDatalinkStandarAccrual As New RoboticsExternAccess.roDatalinkStandarTaskAccrual
                            If bForAllEmployees Then
                                oDatalinkStandarAccrual.NifEmpleado = Any2String(oRow("NIF"))
                                oDatalinkStandarAccrual.UniqueEmployeeID = Any2String(oRow("IdImport"))
                            Else
                                oDatalinkStandarAccrual.NifEmpleado = ColumnsVal(RoboticsExternAccess.TaskAccrualsCriteriaAsciiColumns.NIF)
                                oDatalinkStandarAccrual.UniqueEmployeeID = ColumnsVal(RoboticsExternAccess.TaskAccrualsCriteriaAsciiColumns.ImportPrimaryKey)
                            End If
                            oDatalinkStandarAccrual.Project = Any2String(oRow("Project"))
                            oDatalinkStandarAccrual.Task = Any2String(oRow("Name"))
                            oDatalinkStandarAccrual.AccrualDate = oRow("Date")
                            oDatalinkStandarAccrual.AccrualValue = Any2Double(oRow("total"))

                            ' Añadimos el acumulado diario a la lista
                            lAccruals.Add(oDatalinkStandarAccrual)
                        Next
                    End If

                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Else
                    bolRet = False
                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                End If
            Else
                Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                strErrorMsg = "Invalid accrual object"
            End If
        Catch ex As Exception
            Me.State.UpdateStateInfo(ex, "roDataLinkImport:: GetTaskAccruals")
            bolRet = False
        Finally

        End Try

        Return bolRet
    End Function

End Class

End Namespace