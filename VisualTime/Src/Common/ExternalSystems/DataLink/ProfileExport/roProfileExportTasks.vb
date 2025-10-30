Imports System.Data.Common
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class TupleList(Of T1, T2)
        Inherits List(Of Tuple(Of T1, T2))

        Public Overloads Sub Add(item As T1, item2 As T2)
            Add(New Tuple(Of T1, T2)(item, item2))
        End Sub

    End Class


    Public Class ProfileExportTasks

#Region "Declarations - Constructor"

        Private mEmployeeFilterTable As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState
        Private taskFields As List(Of Tuple(Of String, String))

        Public Sub New(ByVal tmpEmployeeFilterTable As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mBeginDay = BeginDay
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
            CreateTaskFields()
        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal tmpEmployeeFilterTable As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mBeginDate = BeginDate
            mEndDate = EndDate
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
            CreateTaskFields()
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

        Public Property LstFilters() As List(Of String)

#End Region

#Region "Methods"

        Private Enum TaskExportType
            SumByTasks
            SumByTasksDate
            SumByEmployee
            SumByEmployeeDate
            SumByEmployeeTask
            SumByEmployeeTaskDate
            EmployeeTaskDatePartial
        End Enum

        Private Sub CreateTaskFields()
            taskFields = New TupleList(Of String, String)() From {
                                {"TA_NOMBRE", "Name"},
                                {"TA_NOMBRECORTO", "ShortName"},
                                {"TA_DESCRIPCION", "Description"},
                                {"TA_DESCRIPCIÓN", "Description"},
                                {"TA_PROYECTO", "Project"},
                                {"TA_TAG", "Tag"},
                                {"TA_CODIGOBARRAS", "BarCode"},
                                {"TA_FIELD1", "Field1"},
                                {"TA_FIELD2", "Field2"},
                                {"TA_FIELD3", "Field3"},
                                {"TA_FECHAINICIOESTIMADA", "ExpectedStartDate"},
                                {"TA_FECHAFINALESTIMADA", "ExpectedEndDate"},
                                {"TA_FECHAINICIOREAL", "StartDate"},
                                {"TA_FECHAFINALREAL", "EndDate"},
                                {"TA_STATUS", "Status"},
                                {"TA_PRIORIDAD", "Priority"},
                                {"TA_TIEMPOPREVISTO", "InitialTime"},
                                {"TA_TIEMPOINCIDENCIANOPRODUCTIVA", "NonProductiveTimeIncidence"},
                                {"TA_TIEMPOEMPLEADOS", "EmployeeTime"},
                                {"TA_TIEMPOEQUIPO", "TeamTime"},
                                {"TA_TIEMPOMATERIAL", "MaterialTime"},
                                {"TA_TIEMPOOTRO", "OtherTime"},
                                {"TA_FIELD4", "Field4"},
                                {"TA_FIELD5", "Field5"},
                                {"TA_FIELD6", "Field6"}
                            }
        End Sub

        Public Function ExportProfile(ByRef msgLog As String, ByRef _state As roDataLinkState) As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try
                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then
                    ' Importación automática
                    Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                    If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                    Me.mEndDate = Me.mBeginDate.AddMonths(1)
                    Me.mEndDate = Me.mEndDate.AddDays(-1)
                End If

                ' Crea el dataadapter de tareas
                Dim daTasks As DbDataAdapter = CreateDataAdapter_Tasks()

                ' Crea el dataadapter de empleados
                Dim daEmployees As DbDataAdapter = CreateDataAdapter_Employees()

                ' Crea el fichero de salida
                If Not Me.Profile.FileOpen() Then Exit Try
                bolCloseFile = True

                ' Determina el tipo de exportación
                Dim TypeExport As TaskExportType = -1
                Dim bolExportEmployee As Boolean = (Me.Profile.FieldExists("Emp_Nombre") Or Me.Profile.FieldExists("Emp_Grupo") Or Me.Profile.FieldExists("Emp_GrupoCompleto") Or FieldTypeExists("USR_"))
                Dim bolExportTask As Boolean = FieldTypeExists("TA_")
                Dim bolExportDate As Boolean = Me.Profile.FieldExists("AC_Fecha")
                Dim bolExportPartial As Boolean = Me.Profile.FieldExists("AC_Parcial")

                Dim bolExportCC As Boolean = FieldTypeExists("CC_")

                If Not bolExportEmployee AndAlso bolExportTask AndAlso Not bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByTasks
                If Not bolExportEmployee AndAlso bolExportTask AndAlso bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByTasksDate
                If bolExportEmployee AndAlso Not bolExportTask AndAlso Not bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByEmployee
                If bolExportEmployee AndAlso Not bolExportTask AndAlso bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByEmployeeDate
                If bolExportEmployee AndAlso bolExportTask AndAlso Not bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByEmployeeTask
                If bolExportEmployee AndAlso bolExportTask AndAlso bolExportDate AndAlso Not bolExportPartial Then TypeExport = TaskExportType.SumByEmployeeTaskDate
                If bolExportEmployee AndAlso bolExportTask AndAlso bolExportDate AndAlso bolExportPartial Then TypeExport = TaskExportType.EmployeeTaskDatePartial

                ' Si no es una exportación reconocida sale
                If TypeExport = -1 Then
                    msgLog = _state.Language.Translate("ResultEnum.ErrorInInfoProfile", "") & vbNewLine
                    Exit Try
                End If

                ' Selecciona los registros
                Dim dtReg As DataTable = CreateDataTable(SelectRegisters(TypeExport), "Registries")
                Dim dtEmployees As New DataTable
                Dim dtTasks As New DataTable
                Dim EmployeeAnt As Integer = 0
                Dim TaskAnt As Integer = 0
                Dim dtEmpUsrFields As DataTable = Nothing

                ' Para cada registro
                For Each Row As DataRow In dtReg.Rows
                    ' Carga empleados
                    If bolExportEmployee Then
                        If EmployeeAnt <> Row("idEmployee") Then
                            daEmployees.SelectCommand.Parameters("@idEmployee").Value = Row("idEmployee")
                            dtEmployees.Rows.Clear()
                            daEmployees.Fill(dtEmployees)

                            EmployeeAnt = Row("idEmployee")

                            ' Carga los campos e la ficha
                            dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), Me.mEndDate, Me.oState)
                        End If
                    End If

                    ' Carga Tareas o centros de coste
                    If bolExportTask OrElse bolExportCC Then

                        If TaskAnt <> Row("idTask") Then
                            daTasks.SelectCommand.Parameters("@idTask").Value = Row("idTask")
                            dtTasks.Rows.Clear()
                            daTasks.Fill(dtTasks)

                            TaskAnt = Row("idTask")
                        End If
                    End If

                    ' Carga los datos de empleados, tareas y campos de la ficha
                    If Not LoadInfo(dtReg, dtEmpUsrFields, dtEmployees, dtTasks) Then Exit For

                    ' Crea la línea
                    CreateLine(Row)
                Next

                dtEmployees.Dispose()
                dtTasks.Dispose()
                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()

                'Se deja un mensaje informativo dentro del archivo ascii
                If Profile.MemoryStreamWriter IsNot Nothing AndAlso Profile.MemoryStreamWriter.Length = 0 Then
                    Dim textErrror As Byte() = System.Text.Encoding.Unicode.GetBytes(Me.oState.Language.Translate("roDataLinkExport.ExportProfile.NoInfo", ""))
                    Profile.MemoryStreamWriter.Write(textErrror, 0, textErrror.Length)
                End If

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function SelectRegisters(ByVal TypeExport As TaskExportType) As String
            Dim sSQL As String = ""

            Try
                Dim strTaskFilter = String.Empty
                If LstFilters IsNot Nothing AndAlso LstFilters.Count > 0 Then
                    For Each filter As String In LstFilters
                        If (filter.Contains("convert")) Then
                            strTaskFilter = strTaskFilter & " AND " & filter.Replace("{0}", "Tasks.")
                        Else
                            strTaskFilter = strTaskFilter & " AND Tasks." & filter
                        End If

                    Next
                End If

                Select Case TypeExport

                    Case TaskExportType.SumByTasks
                        sSQL = "@SELECT# id as IdTask from Tasks " &
                            "where StartDate between " & Any2Time(mBeginDate).SQLDateTime & " and " & Any2Time(Me.mEndDate).SQLDateTime & strTaskFilter

                    Case TaskExportType.SumByTasksDate
                        sSQL = "@SELECT# AC.idtask, AC.date, sum(AC.value) as Value, sum(AC.field4) as field4, sum(AC.field5) as field5, sum(AC.field6) as field6 "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " from DailyTaskAccruals AC"
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL += "  where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " group by AC.idtask,AC.date"
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " order by AC.idtask,AC.date "

                    Case TaskExportType.SumByEmployee
                        sSQL = "@SELECT# AC.idemployee, sum(AC.value) as Value, sum(AC.field4) as field4, sum(AC.field5) as field5, sum(AC.field6) as field6 "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " from DailyTaskAccruals AC"
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL += "  where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " group by idemployee "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " order by AC.idEmployee "

                    Case TaskExportType.SumByEmployeeDate
                        sSQL = "@SELECT# AC.idemployee, AC.date, sum(AC.value) as Value, sum(AC.field4) as field4, sum(AC.field5) as field5, sum(AC.field6) as field6 "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " from DailyTaskAccruals AC"
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL += "  where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " group by AC.idemployee,AC.date"
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " order by AC.idEmployee,AC.date "

                    Case TaskExportType.SumByEmployeeTask
                        sSQL = "@SELECT# AC.idemployee, AC.idtask, sum(AC.value) as Value, sum(AC.field4) as field4, sum(AC.field5) as field5, sum(AC.field6) as field6 "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " from DailyTaskAccruals AC"
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL += "  where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " group by AC.idemployee,AC.idtask"
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " order by AC.idEmployee "

                    Case TaskExportType.SumByEmployeeTaskDate
                        sSQL = "@SELECT# AC.idemployee, AC.idtask, AC.date, sum(AC.value) as Value, sum(AC.field4) as field4, sum(AC.field5) as field5, sum(AC.field6) as field6 "
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " from DailyTaskAccruals AC"
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL += "  where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " group by AC.idemployee,AC.idtask,AC.date"
                        If Me.Profile.FieldExists("AC_Field1") Then sSQL += ", AC.Field1"
                        If Me.Profile.FieldExists("AC_Field2") Then sSQL += ", AC.Field2"
                        If Me.Profile.FieldExists("AC_Field3") Then sSQL += ", AC.Field3"
                        sSQL += " order by AC.idEmployee,AC.date "

                    Case TaskExportType.EmployeeTaskDatePartial
                        sSQL = "@SELECT# * from DailyTaskAccruals AC "
                        If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = AC.IDEmployee "
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & " INNER JOIN Tasks on AC.IDTask = Tasks.ID "
                        sSQL = sSQL & " where AC.date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime
                        If (Not String.IsNullOrEmpty(strTaskFilter)) Then sSQL = sSQL & strTaskFilter
                        sSQL += " order by idEmployee,date "

                End Select
            Catch ex As Exception

            End Try

            Return sSQL
        End Function

        Private Function FieldTypeExists(ByVal FieldType As String) As Boolean
            Dim bolExists As Boolean = False

            Try
                FieldType = FieldType.ToUpper

                ' Crea la cabecera del excel
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    If InStr(1, Me.Profile.Fields(i).Source.ToUpper, FieldType) = 1 Then
                        bolExists = True
                        Exit For
                    End If
                Next i
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:FieldTypeExists")
            End Try

            Return bolExists
        End Function

        Private Function CreateLine(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "AC_FECHA" : Profile.Fields(i).Value = cRow("Date")
                        Case "AC_PARCIAL" : Profile.Fields(i).Value = cRow("idPart")
                        Case "AC_TIEMPO" : Profile.Fields(i).Value = cRow("Value")
                        Case "AC_FIELD1" : Profile.Fields(i).Value = Any2String(cRow("Field1"))
                        Case "AC_FIELD2" : Profile.Fields(i).Value = Any2String(cRow("Field2"))
                        Case "AC_FIELD3" : Profile.Fields(i).Value = Any2String(cRow("Field3"))
                        Case "AC_FIELD4" : Profile.Fields(i).Value = Any2Double(cRow("Field4"))
                        Case "AC_FIELD5" : Profile.Fields(i).Value = Any2Double(cRow("Field5"))
                        Case "AC_FIELD6" : Profile.Fields(i).Value = Any2Double(cRow("Field6"))
                    End Select
                Next i

                ' Graba la línea
                bolRet = Me.Profile.CreateLine

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function CreateDataAdapter_Tasks() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try

                Dim strSQL As String =
                    "@SELECT# tasks.*, BusinessCenters.Name CC_Name, BusinessCenters.Description as CC_Description " &
                    "FROM dbo.Tasks INNER JOIN BusinessCenters ON Tasks.IDCenter = dbo.BusinessCenters.ID " &
                    "where Tasks.id=@idTask "
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idTask", DbType.Int32)

                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataAdapter_Tasks")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_Employees() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# sysrovwCurrentEmployeeGroups.IDEmployee, EmployeeName, GroupName, FullGroupName " &
                    "FROM dbo.sysrovwCurrentEmployeeGroups where idEmployee=@idEmployee"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataAdapter_Employees")
            End Try

            Return da
        End Function

        Public Function SetDataFilters(columnName As String, columnValue As String) As String
            Try
                Dim filterValue = String.Empty
                Dim tupleValue = taskFields.FirstOrDefault(Function(f) f.Item1.Equals(columnName.ToUpper.Trim()))
                If (tupleValue IsNot Nothing) Then
                    Select Case columnName.ToUpper.Trim()
                    'String
                        Case "TA_NOMBRE", "TA_FIELD3", "TA_FIELD2", "TA_FIELD1", "TA_CODIGOBARRAS",
                             "TA_TAG", "TA_PROYECTO", "TA_DESCRIPCIÓN", "TA_DESCRIPCION", "TA_NOMBRECORTO"
                            filterValue = tupleValue.Item2 & " like '%" & columnValue & "%'"

                    'Date
                        Case "TA_FECHAINICIOESTIMADA", "TA_FECHAFINALREAL", "TA_FECHAINICIOREAL", "TA_FECHAFINALESTIMADA"
                            Dim dateValues() = columnValue.Split("-")
                            If (dateValues.Count > 1) Then
                                filterValue = "convert(date,{0}" & tupleValue.Item2 & ",120) between " & Any2Time(dateValues(0)).SQLDateTime & " AND " & Any2Time(dateValues(1)).SQLDateTime
                            Else
                                filterValue = "convert(date,{0}" & tupleValue.Item2 & ",120) = " & Any2Time(dateValues(0)).SQLDateTime
                            End If

                    'Entero
                        Case "TA_STATUS", "TA_PRIORIDAD", "TA_TIEMPOPREVISTO", "TA_TIEMPOINCIDENCIANOPRODUCTIVA",
                             "TA_TIEMPOEMPLEADOS", "TA_TIEMPOEQUIPO", "TA_TIEMPOMATERIAL", "TA_TIEMPOOTRO", "TA_FIELD4",
                             "TA_FIELD5", "TA_FIELD6"
                            filterValue = tupleValue.Item2 & " = " & columnValue
                    End Select
                End If
                Return filterValue
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:SetDataFilters")
                Return String.Empty
            End Try
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal dtEmpUsrFields As DataTable, ByVal dtEmployees As DataTable, ByVal dtTasks As DataTable) As Boolean
            Dim bolRet As Boolean = False
            Dim i As Integer = 0

            Try
                Dim rowEmp As DataRow = Nothing
                If Not IsNothing(dtEmployees) AndAlso dtEmployees.Rows.Count > 0 Then rowEmp = dtEmployees.Rows(0)

                Dim rowTask As DataRow = Nothing
                If Not IsNothing(dtTasks) AndAlso dtTasks.Rows.Count > 0 Then rowTask = dtTasks.Rows(0)

                ' Para cada columna
                For i = 0 To Me.Profile.Fields.Count - 1
                    Me.Profile.Fields(i).Value = ""

                    Select Case Me.Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Me.Profile.Fields(i).Value = mBeginDate

                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Me.Profile.Fields(i).Value = mEndDate

                        Case "EMP_GRUPO"
                            If Not IsNothing(rowEmp) Then Me.Profile.Fields(i).Value = rowEmp("GroupName")

                        Case "EMP_GRUPOCOMPLETO"
                            If Not IsNothing(rowEmp) Then Me.Profile.Fields(i).Value = rowEmp("FullGroupName")

                        Case "EMP_NOMBRE"
                            If Not IsNothing(rowEmp) Then Me.Profile.Fields(i).Value = rowEmp("EmployeeName")

                        Case "CC_NOMBRE"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("CC_Name")

                        Case "CC_DESCRIPCION", "CC_DESCRIPCIÓN"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("CC_Description")

                        Case "TA_NOMBRE"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("Name")

                        Case "TA_NOMBRECORTO"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("ShortName")

                        Case "TA_DESCRIPCION", "TA_DESCRIPCIÓN"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("Description")

                        Case "TA_STATUS"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("Status")

                        Case "TA_PROYECTO"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("Project")

                        Case "TA_TAG"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("TAG")

                        Case "TA_PRIORIDAD"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("Priority")

                        Case "TA_FECHAINICIOESTIMADA"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("ExpectedStartDate")

                        Case "TA_FECHAFINALESTIMADA"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("ExpectedEndDate")

                        Case "TA_FECHAINICIOREAL"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("StartDate")

                        Case "TA_FECHAFINALREAL"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("EndDate")

                        Case "TA_TIEMPOPREVISTO"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("InitialTime")

                        Case "TA_TIEMPOINCIDENCIANOPRODUCTIVA"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("NonProductiveTimeIncidence")

                        Case "TA_TIEMPOEMPLEADOS"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("EmployeeTime")

                        Case "TA_TIEMPOEQUIPO"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("TeamTime")

                        Case "TA_TIEMPOMATERIAL"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("MaterialTime")

                        Case "TA_TIEMPOOTRO"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = rowTask("OtherTime")

                        Case "TA_CODIGOBARRAS"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2String(rowTask("BarCode"))

                        Case "TA_FIELD1"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2String(rowTask("Field1"))

                        Case "TA_FIELD2"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2String(rowTask("Field2"))

                        Case "TA_FIELD3"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2String(rowTask("Field3"))

                        Case "TA_FIELD4"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2Double(rowTask("Field4"))

                        Case "TA_FIELD5"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2Double(rowTask("Field5"))
                        Case "TA_FIELD6"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2Double(rowTask("Field6"))
                        Case "TA_FECHACOMPLETADA"
                            If Not IsNothing(rowTask) Then Me.Profile.Fields(i).Value = Any2DateTime(rowTask("UpdateStatusDate"))
                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Me.Profile.Fields(i).Source.ToUpper, "USR_") Then
                                If Not IsNothing(rowEmp) Then
                                    ' Lee el dato del campo de la ficha
                                    If dtEmpUsrFields.Rows.Count > 0 Then
                                        Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                        Dim r() As DataRow = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                        If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                    End If
                                End If
                            Else
                                ' Literal
                                If InStr(1, Me.Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                    Me.Profile.Fields(i).Value = Me.Profile.Fields(i).Source.Substring(8, Me.Profile.Fields(i).Source.Length - 8)
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Profile.Fields(i).Value = "RBS not supported"
                                Else
                                    'Console.Write(Me.Profile.Fields(i).Source.ToUpper & vbNewLine)
                                End If
                            End If
                    End Select
                Next

                ' Si hay saldos de los dos tipos no es correcto porque la plantilla no es correcta
                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:LoadInfo")
            End Try

            Return bolRet
        End Function

#End Region

    End Class


End Namespace