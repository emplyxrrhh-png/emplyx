Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink
    Public Class ProfileExportPlanning

#Region "Declarations - Constructor"

        Private mEmployeesFilter As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody

        Private mPeriodPattern As String = String.Empty

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState

        Private oLog As New roLog("ProfileExportPlanning")

        Public Sub New(ByVal EmployeesFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeesFilter = EmployeesFilter
            mBeginDay = BeginDay
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal EmployeesFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

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

        Public Property PeriodPattern() As String
            Get
                Return Me.mPeriodPattern
            End Get

            Set(ByVal value As String)
                Me.mPeriodPattern = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function ExportProfile(ByRef msgLog As String, ByRef _state As roDataLinkState) As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try
                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then
                    ' Si me pasaron una definición de periodo de datos, la interpreto ahora ....
                    If Me.PeriodPattern.Trim.Length > 0 AndAlso (Me.PeriodPattern.IndexOf(",") > 0 OrElse Me.PeriodPattern.IndexOf(".") > 0) Then
                        roDataLinkExport.GetExportPeriodFromPattern(Me.PeriodPattern.Replace(".", ","), Now, mBeginDate, mEndDate)
                    Else
                        ' Exportación automática
                        Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                        If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                        Me.mEndDate = Me.mBeginDate.AddMonths(1)
                        Me.mEndDate = Me.mEndDate.AddDays(-1)
                    End If
                End If

                ' Crea el fichero de salida
                If Not Me.Profile.FileOpen() Then Exit Try
                bolCloseFile = True

                ' Selecciona los registros
                Dim dtReg As DataTable = CreateDataTable(SelectRegisters, "Registries")
                Dim dtContracts As New DataTable
                Dim dtGroups As New DataTable
                Dim dtMobilities As New DataTable
                Dim dtEmpUserFieldValues As New DataTable
                Dim dtSchedule As New DataTable

                Dim importKeyFieldName As String = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value)

                ' Para cada registro
                Dim n, iTotalEmployees As Integer
                n = 0
                iTotalEmployees = dtReg.Rows.Count

                For Each Row As DataRow In dtReg.Rows

                    n += 1
                    oLog.logMessage(roLog.EventType.roDebug, "Exporting planning for employee number " & n.ToString & " out of " & iTotalEmployees.ToString)

                    ' Carga los contratos del empleado
                    dtContracts = DataLinkDynamicCode.CreateDataTable_Contracts(Row("ID"), Me.oState)

                    ' Carga los niveles a los que pertenece el empleado a fecha de hoy
                    dtGroups = DataLinkDynamicCode.CreateDataTable_Groups(Row("ID"), Me.oState)

                    ' Carga el histórico de movilidades del empleado
                    dtMobilities = DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(Row("ID"), Me.oState)

                    ' Carga la ficha del empleado
                    dtEmpUserFieldValues = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("ID"), Me.mEndDate, Me.oState)

                    ' Carga planificación del empleado para el periodo
                    dtSchedule = VTBusiness.Scheduler.roScheduler.GetPlan(Row("id"), mBeginDate, mEndDate, New Employee.roEmployeeState,, True)

                    ' Carga los datos de empleados, tareas y campos de la ficha
                    If Not LoadInfo(dtReg, Row, dtEmpUserFieldValues, dtContracts, dtGroups, dtMobilities, dtSchedule, importKeyFieldName) Then Exit For
                Next

                dtContracts.Dispose()
                dtGroups.Dispose()
                dtEmpUserFieldValues.Dispose()
                dtEmpUserFieldValues.Dispose()
                dtSchedule.Dispose()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function SelectRegisters() As String
            Dim sSQL As String = ""

            Try
                sSQL = "@SELECT# * FROM Employees "
                If Me.mEmployeesFilter <> "" Then sSQL &= " WHERE ID in (" & Me.mEmployeesFilter & ") "
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:FieldTypeExists")
            End Try

            Return bolExists
        End Function

        Private Function CreateLine(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Graba la línea
                bolRet = Me.Profile.CreateLine

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal oEmployee As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtContracts As DataTable, ByVal dtGroups As DataTable, ByVal dtMobilities As DataTable, ByVal dtEmpSchedule As DataTable, ByVal strImportKeyFieldName As String) As Boolean
            Dim bolRet As Boolean = False
            Dim i As Integer = 0

            Try

                ' Creo matriz de roturas. Todos los días de la exportación, inicio y fin incluidos.
                Dim oBreaksDateList As New List(Of DateTime)
                Dim bFillGaps As Boolean = False

                'Añadimos la fecha de inicio y final de la exportación
                For iDay As Integer = 0 To mEndDate.Subtract(mBeginDate).Days
                    oBreaksDateList.Add(mBeginDate.Date.AddDays(iDay))
                Next

                ' 2.- Roturas por Campos de la ficha
                ' Consideraré sólo los campos de la ficha que se hayan incluido en la exportación
                Dim dtEmployeeUserFieldValuesHistory As DataTable
                Dim lUsr As New List(Of String)
                For i = 0 To Me.Profile.Fields.Count - 1
                    If Me.Profile.Fields(i).Source.StartsWith("USR_") Then
                        lUsr.Add(Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4))
                    Else
                        ' Si se incluyo la clave de importación, la añado también
                        If Me.Profile.Fields(i).Source.ToUpper = "PRIMARYKEY" AndAlso strImportKeyFieldName.Length > 0 Then lUsr.Add(strImportKeyFieldName)
                    End If
                Next
                Dim sFilter As String = String.Empty
                For Each sFieldName As String In lUsr
                    If sFilter <> String.Empty Then sFilter = sFilter & ","
                    sFilter = sFilter & "'" & sFieldName & "'"
                Next
                dtEmployeeUserFieldValuesHistory = DataLinkDynamicCode.CreateDataTable_EmployeeUserFieldValuesHistory(oEmployee("ID"), sFilter, oState)

                ' 4.- Ahora a paritr de la lista de fechas, creo la matriz de roturas
                Dim dtBreaks As New DataTable
                dtBreaks = GetBreaksDateTable(oBreaksDateList)

                ' 5.- Planificación
                Dim oScheduleDataView As New DataView
                If dtEmpSchedule IsNot Nothing AndAlso dtEmpSchedule.Rows.Count > 0 Then
                    oScheduleDataView = New System.Data.DataView(dtEmpSchedule)
                    oScheduleDataView.Sort = "Date ASC"
                End If

                ' 6.- Cargo información e imprimo lineas
                Dim dtGroupsOnDate As New DataTable
                Dim oContractRow As DataRow = Nothing
                Dim oMobilityRow As DataRow = Nothing
                Dim oUserFieldValueOnDateRow As DataRow = Nothing
                Dim sUsrFldName As String

                For Each oBreakRow In dtBreaks.Rows

                    ' Recupero contrato en función del periodo de rotura
                    oContractRow = GetContractOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtContracts)

                    ' Sólo muestro líneas si en el periodo hay contrato
                    If Not oContractRow Is Nothing Then

                        ' Recupero información de departamento en la fecha del periodo
                        dtGroupsOnDate = DataLinkDynamicCode.CreateDataTable_GroupsOnDate(oEmployee("ID"), oBreakRow("EndDate"), oState)

                        ' Recupero información del grupo
                        oMobilityRow = GetMobilityOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtMobilities)

                        ' Para cada columna
                        Dim sTotal As Single = 0
                        For i = 0 To Me.Profile.Fields.Count - 1
                            Me.Profile.Fields(i).Value = ""
                            Select Case Me.Profile.Fields(i).Source.ToUpper
                                Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                                    Me.Profile.Fields(i).Value = mBeginDate
                                Case "FECHADETALLE"
                                    Me.Profile.Fields(i).Value = oBreakRow("BeginDate")
                                Case "FECHAINICIOMOVILIDAD"
                                    Me.Profile.Fields(i).Value = oMobilityRow("BeginDate")
                                Case "FECHAFINMOVILIDAD"
                                    Dim objDate As Date = roTypes.Any2DateTime(oMobilityRow("EndDate"))
                                    If objDate.Year = 2079 AndAlso objDate.Month = 1 And objDate.Day = 1 Then
                                        Me.Profile.Fields(i).Value = ""
                                    Else
                                        Me.Profile.Fields(i).Value = oMobilityRow("EndDate")
                                    End If
                                Case "GRUPO"
                                    Me.Profile.Fields(i).Value = oMobilityRow("Name")
                                Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                                    Me.Profile.Fields(i).Value = mEndDate
                                Case "NOMBRE"
                                    Me.Profile.Fields(i).Value = oEmployee("Name")
                                Case "CONTRATO"
                                    Me.Profile.Fields(i).Value = oContractRow("IDContract")
                                Case "FECHAALTA"
                                    Me.Profile.Fields(i).Value = oContractRow("BeginDate")
                                Case "FECHABAJA"
                                    Dim objDate As Date = roTypes.Any2DateTime(oContractRow("EndDate"))
                                    If objDate.Year = 2079 AndAlso objDate.Month = 1 And objDate.Day = 1 Then
                                        Me.Profile.Fields(i).Value = ""
                                    Else
                                        Me.Profile.Fields(i).Value = oContractRow("EndDate")
                                    End If
                                Case "PRIMARYKEY"
                                    oUserFieldValueOnDateRow = GetUserFieldValueOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), strImportKeyFieldName, dtEmployeeUserFieldValuesHistory)
                                    If Not oUserFieldValueOnDateRow Is Nothing AndAlso Not IsDBNull(oUserFieldValueOnDateRow("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(oUserFieldValueOnDateRow("Value"), oUserFieldValueOnDateRow("FieldType"))
                                Case "CONVENIO"
                                    Me.Profile.Fields(i).Value = oContractRow("LabAgreeName")
                                Case "[TOTAL]"
                                    Me.Profile.Fields(i).Value = sTotal
                                Case Else
                                    ' Determina el tipo de campo
                                    If Me.Profile.Fields(i).Source.ToUpper.StartsWith("HORARIO_ID_") Then
                                        Dim sShiftID As String = String.Empty
                                        sShiftID = Me.Profile.Fields(i).Source.Substring(11, Me.Profile.Fields(i).Source.Length - 11)
                                        If oScheduleDataView IsNot Nothing AndAlso oScheduleDataView.Table IsNot Nothing AndAlso oScheduleDataView.ToTable.Rows.Count > 0 Then
                                            Dim oScheduleRow As DataRow
                                            oScheduleDataView.RowFilter = "Date = '" & Format(oBreakRow("BeginDate"), "yyyy/MM/dd") & "'"
                                            If oScheduleDataView.ToTable.Rows.Count > 0 Then
                                                oScheduleRow = oScheduleDataView.ToTable.Rows(0)
                                                ' Si el día está calculado, me quedo con ShiftUsed. Si no, con el Shift1
                                                If Not IsDBNull(oScheduleRow("IDShiftUsed")) Then
                                                    If oScheduleRow("IDShiftUsed") = sShiftID Then
                                                        Profile.Fields(i).Value = oScheduleRow("ExpectedWorkingHoursUsedShift")
                                                    Else
                                                        Profile.Fields(i).Value = 0
                                                    End If
                                                Else
                                                    If oScheduleRow("IDShift1") = sShiftID Then
                                                        Profile.Fields(i).Value = oScheduleRow("ExpectedWorkingHours1")
                                                    Else
                                                        Profile.Fields(i).Value = 0
                                                    End If
                                                End If
                                                sTotal = sTotal + roTypes.Any2Double(Profile.Fields(i).Value)
                                            End If
                                        End If
                                    ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("USR_") Then
                                        sUsrFldName = Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4)
                                        oUserFieldValueOnDateRow = GetUserFieldValueOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), sUsrFldName, dtEmployeeUserFieldValuesHistory)
                                        If Not oUserFieldValueOnDateRow Is Nothing AndAlso Not IsDBNull(oUserFieldValueOnDateRow("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(oUserFieldValueOnDateRow("Value"), oUserFieldValueOnDateRow("FieldType"))
                                    ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("DAT_") Then
                                        sUsrFldName = Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4)
                                        oUserFieldValueOnDateRow = GetUserFieldValueOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), sUsrFldName, dtEmployeeUserFieldValuesHistory)
                                        If Not oUserFieldValueOnDateRow Is Nothing Then
                                            Dim objDate As Date = roTypes.Any2DateTime(oUserFieldValueOnDateRow("Date"))
                                            If objDate.Year = 2079 AndAlso objDate.Month = 1 And objDate.Day = 1 Then
                                                Me.Profile.Fields(i).Value = ""
                                            Else
                                                Me.Profile.Fields(i).Value = oUserFieldValueOnDateRow("Date")
                                            End If
                                        End If
                                    ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("LITERAL_") Then
                                        Me.Profile.Fields(i).Value = Me.Profile.Fields(i).Source.Substring(8, Me.Profile.Fields(i).Source.Length - 8)
                                    ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("NIVEL") Then
                                        Dim iLevel As Integer = roTypes.Any2Integer(Me.Profile.Fields(i).Source.Substring(5, Me.Profile.Fields(i).Source.Length - 5))
                                        If dtGroupsOnDate.Rows.Count >= (iLevel + 1) Then
                                            Me.Profile.Fields(i).Value = dtGroupsOnDate.Rows(iLevel)("Name")
                                        Else
                                            Me.Profile.Fields(i).Value = ""
                                        End If
                                        ' Se añade la descripción del grupo
                                    ElseIf Profile.Fields(i).Source.ToUpper.StartsWith("DESCRIPCIONNIVEL") Then
                                        Dim iLevel As Integer = roTypes.Any2Integer(Me.Profile.Fields(i).Source.Substring(16, Me.Profile.Fields(i).Source.Length - 16))
                                        If dtGroupsOnDate.Rows.Count >= (iLevel + 1) Then
                                            Profile.Fields(i).Value = dtGroupsOnDate.Rows(iLevel)("DescriptionGroup")
                                        Else
                                            Profile.Fields(i).Value = ""
                                        End If
                                    ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                        Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                        Profile.Fields(i).Value = "RBS not supported"
                                    End If
                            End Select

                        Next
                        CreateLine(oEmployee)
                    End If
                Next

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Sub AddPeriodToBreaksTable(dBeginDate As DateTime, dEndDate As DateTime, dScopeBeginDate As DateTime, dScopeEndDate As DateTime, bIsFirstPeriod As Boolean, bIsLastPeriod As Boolean, ByRef oBreaksDateList As List(Of DateTime))
            Try

                ' Si el primer periodo es posterior al inicio de periodo de exportación, creo un periodo para completar
                If bIsFirstPeriod AndAlso dBeginDate > dScopeBeginDate AndAlso dBeginDate <= dScopeEndDate Then
                    oBreaksDateList.Add(dScopeBeginDate)
                    oBreaksDateList.Add(dBeginDate.AddDays(-1).AddHours(1))
                End If

                ' Si el último periodo finaliza antes que el periodo de exportación, creo un periodo para completar
                If bIsLastPeriod AndAlso dEndDate < dScopeEndDate AndAlso dEndDate >= dScopeBeginDate Then
                    oBreaksDateList.Add(dEndDate.AddDays(1))
                    oBreaksDateList.Add(dScopeEndDate.AddHours(1))
                End If

                If dBeginDate >= dScopeBeginDate AndAlso dEndDate <= dScopeEndDate Then
                    ' Periodo  contenido completamente en periodo de exportación
                    oBreaksDateList.Add(dBeginDate)
                    oBreaksDateList.Add(dEndDate.AddHours(1))
                ElseIf dEndDate < dScopeBeginDate OrElse dBeginDate > dScopeEndDate Then
                    ' Periodo fuera de periodo de exportación. No hago nada
                ElseIf dBeginDate < dScopeBeginDate AndAlso dEndDate <= dScopeEndDate Then
                    ' Periodo empieza antes de periodo de exportación, y acaba dentro
                    oBreaksDateList.Add(dScopeBeginDate)
                    oBreaksDateList.Add(dEndDate.AddHours(1))
                ElseIf dBeginDate < dScopeBeginDate AndAlso dEndDate > dScopeEndDate Then
                    ' Periodo empieza antes de periodo de exportación, y acaba después.
                    ' No hace falta añadir nada, porque añadiría exactamente el periodo de exportación
                ElseIf dBeginDate > dScopeBeginDate AndAlso dEndDate >= dScopeEndDate Then
                    ' Periodo empieza dentro del periodo de exportación, y acaba fuera
                    oBreaksDateList.Add(dBeginDate)
                    oBreaksDateList.Add(dScopeEndDate.AddHours(1))
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:AddPeriodToBreaksTable")
            End Try

        End Sub

        Private Function GetBreaksDateTable(oDateList As List(Of DateTime)) As DataTable
            Dim dtBreaks As DataTable = Nothing
            Try
                Dim oBreaksRow As DataRow
                dtBreaks = CreateDataTable("@SELECT# getdate() as BeginDate, getdate() as EndDate")
                dtBreaks.Rows.Clear()
                oDateList.Sort()

                Dim dDate As DateTime

                For n = 0 To oDateList.Count - 1
                    oBreaksRow = dtBreaks.NewRow
                    dDate = oDateList(n)
                    oBreaksRow("BeginDate") = dDate
                    oBreaksRow("EndDate") = dDate
                    dtBreaks.Rows.Add(oBreaksRow)
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetBreaksDateTable")
            End Try
            Return dtBreaks
        End Function

        Private Function GetContractOnDate(dStartDate As DateTime, dEndDate As DateTime, dtContracts As DataTable) As DataRow
            Dim oRet As DataRow = Nothing
            Try
                For Each oContractRow As DataRow In dtContracts.Rows
                    If oContractRow("BeginDate") <= dStartDate AndAlso oContractRow("EndDate") >= dEndDate Then
                        oRet = oContractRow
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetContractOnDate")
            End Try
            Return oRet
        End Function

        Private Function GetLastContractOnPeriod(dStartDate As DateTime, dEndDate As DateTime, dtContracts As DataTable) As DataRow
            Dim oContractRow As DataRow = Nothing
            Try

                If dtContracts.Rows.Count > 0 Then
                    ' Si me pasaron una fecha distinta de la actual, devuelvo el contrato activo a esa fecha, o el último que tuvo activo en el periodo

                    Dim oDataView As System.Data.DataView = New System.Data.DataView(dtContracts)
                    oDataView.RowFilter = "BeginDate <= '" & Format(dEndDate, "yyyy/MM/dd") & "'"
                    oDataView.Sort = "BeginDate DESC"
                    If oDataView.Count > 0 Then
                        oContractRow = oDataView.ToTable.Rows(0)
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetLastContractOnPeriod")
            End Try
            Return oContractRow
        End Function

        Private Function GetUserFieldValueOnDate(dStartDate As DateTime, dEndDate As DateTime, sFieldName As String, dtEmpUsrFieldHistory As DataTable) As DataRow
            Dim oRet As DataRow = Nothing
            Try
                For Each oEmpUsrFieldHistoryRow As DataRow In dtEmpUsrFieldHistory.Rows
                    'If oEmpUsrFieldHistoryRow("BeginDate") <= dStartDate AndAlso oEmpUsrFieldHistoryRow("EndDate") >= dEndDate AndAlso oEmpUsrFieldHistoryRow("FieldName") = sFieldName Then
                    If oEmpUsrFieldHistoryRow("EndDate") >= dEndDate AndAlso oEmpUsrFieldHistoryRow("BeginDate") <= dEndDate AndAlso roTypes.Any2String(oEmpUsrFieldHistoryRow("FieldName")).Trim.ToUpper = sFieldName.Trim.ToUpper Then
                        oRet = oEmpUsrFieldHistoryRow
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetUserFieldValueOnDate")
            End Try
            Return oRet
        End Function

        Private Function GetMobilityOnDate(dStartDate As DateTime, dEndDate As DateTime, dtGroups As DataTable) As DataRow
            Dim oRet As DataRow = Nothing
            Try
                For Each oMobilityRow As DataRow In dtGroups.Rows
                    If oMobilityRow("BeginDate") <= dStartDate.Date AndAlso oMobilityRow("EndDate") >= dEndDate.Date Then
                        oRet = oMobilityRow
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetMobilityOnDate")
            End Try
            Return oRet
        End Function

        Private Function GetLastmobilityOnPeriod(dStartDate As DateTime, dEndDate As DateTime, dtMobilities As DataTable) As DataRow
            Dim oMobilityRow As DataRow = Nothing
            Try

                If dtMobilities.Rows.Count > 0 Then
                    ' Si me pasaron una fecha distinta de la actual, devuelvo la movilidad activo a esa fecha, o el último que tuvo activo en el periodo

                    Dim oDataView As System.Data.DataView = New System.Data.DataView(dtMobilities)
                    oDataView.RowFilter = "BeginDate <= '" & Format(dEndDate, "yyyy/MM/dd") & "'"
                    oDataView.Sort = "BeginDate DESC"
                    If oDataView.Count > 0 Then
                        oMobilityRow = oDataView.ToTable.Rows(0)
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPlanning:GetLastmobilityOnPeriod")
            End Try
            Return oMobilityRow
        End Function

#End Region

    End Class

End Namespace