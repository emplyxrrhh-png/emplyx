Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink


    Public Class ProfileExportPunchesEx

#Region "Declarations - Constructor"

        Private mEmployeeTempTableName As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody
        Private mdaDinners As DbDataAdapter = Nothing
        Private mIntervalMinutes As Integer = 0
        Public mClearContractEndDate As Boolean = False

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState

        Public Sub New(ByVal employeeTempTableName As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal IntervalMinutes As Integer, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeTempTableName = employeeTempTableName
            mBeginDay = BeginDay
            mIntervalMinutes = IntervalMinutes
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal employeeTempTableName As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeTempTableName = employeeTempTableName
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

                ' Crea el dataadapter de contratos
                Dim daEmployeeContracts As DbDataAdapter = CreateDataAdapter_EmployeeContracts(mClearContractEndDate)

                ' Crea el fichero de salida
                If Not Me.Profile.FileOpen() Then Exit Try
                bolCloseFile = True

                ' Selecciona los fichajes
                Dim sSQL As String =
                    "@SELECT# dbo.Punches.ID, dbo.Punches.IDEmployee, dbo.Punches.Type, dbo.Punches.ActualType, dbo.Punches.ShiftDate, dbo.Punches.DateTime,
					CASE WHEN dbo.Punches.IDTerminal > 0 THEN dbo.Punches.IDTerminal ELSE NULL END IDTerminal , dbo.Punches.TypeData, dbo.Terminals.Description as TerminalDescription, dbo.Terminals.Location as TerminalLocation,
					dbo.Causes.Export, dbo.Causes.ShortName, dbo.DailySchedule.IDShiftUsed, dbo.Shifts.Export ExportShift,
                    ISNULL(Causes.Name,'') CauseName, sysroPassports.Name as Supervisor, ISNULL(Punches.CreatedOn,'') DateCreated, ISNULL(Punches.TimeStamp,NULL) DateModified,
                    CASE WHEN Punches.IDTerminal > 0 THEN Terminals.Type ELSE CASE WHEN Punches.IDPassport > 0 THEN 'Supervisor' ELSE '?' END END Origen, ISNULL(punches.Location,'') GPS,
                    dbo.Punches.InTelecommute, dbo.Punches.WorkCenter, IsNotReliable, Source
					FROM dbo.Punches"

                If mEmployeeTempTableName.Length > 0 Then
                    sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = dbo.Punches.IDEmployee "
                End If

                sSQL &= " LEFT JOIN dbo.DailySchedule ON  dbo.Punches.IDEmployee = dbo.DailySchedule.IDEmployee and dbo.Punches.ShiftDate = dbo.DailySchedule.Date
					LEFT OUTER JOIN dbo.Causes ON dbo.Punches.TypeData = dbo.Causes.ID  AND dbo.Punches.TypeData > 0
					LEFT OUTER JOIN dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID
					left join dbo.Shifts on dbo.DailySchedule.IDShiftUsed = dbo.Shifts.id
                    LEFT JOIN sysroPassports ON sysroPassports.ID = Punches.IDPassport
					WHERE Punches.idEmployee>0 And (dbo.Punches.ActualType In (1, 2))  "

                'If mEmployeesFilter.Length > 0 Then
                '    sSQL += " And dbo.punches.idEmployee In (" & Me.mEmployeesFilter & ")"
                'End If

                sSQL += " And dbo.Punches.ShiftDate between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " And " & roTypes.Any2Time(Me.mEndDate).SQLDateTime

                sSQL += " ORDER BY datetime, dbo.punches.idEmployee"

                Dim dtPunches As DataTable = CreateDataTableWithoutTimeouts(sSQL,, "Punches")
                Dim idEmployeeAnt As Integer = 0

                ' Crea el dataadapter de empleados
                Dim daEmployees As DbDataAdapter = CreateDataAdapter_Employees()

                Dim dtEmployeeContracts As New DataTable
                Dim dtEmployees As New DataTable
                Dim dtShifts As New DataTable
                Dim bolLoadContracts As Boolean = False

                ' Verifica si tiene que cargar los contratos
                Dim i As Integer
                For i = 0 To Me.Profile.Fields.Count - 1
                    If InStr(Profile.Fields(i).Source.ToUpper, "CONTRATO") > 0 Then
                        bolLoadContracts = True
                        Exit For
                    End If
                Next i

                'Verifica si debemos cargar la información de los horarios - JP
                Dim bolLoadShiftsDefinition = Profile.Fields.Any(Function(f) f.Source.ToUpper().Equals("EQUIVALENCIA"))
                Dim lstShifts As New List(Of Shift.roShift)
                'Si tenemos que cargar la información, buscamos todos sus horarios para cargar las definiciones - JP
                If (bolLoadShiftsDefinition) Then
                    'Recuperamos los distintos horarios que han utilizado los empleados -
                    Dim dtDistinctShifIds = dtPunches.DefaultView.ToTable(True, "IDShiftUsed")
                    For Each shiftRow As DataRow In dtDistinctShifIds.Rows
                        If Not IsDBNull(shiftRow("IdShiftUsed")) Then
                            Dim oshiftState As New Shift.roShiftState
                            Dim oShift As New Shift.roShift(shiftRow("IDShiftUsed"), oshiftState)
                            'Valido si tiene franjas rigidas
                            Dim shiftLayersMandatory = oShift.Layers.Cast(Of Shift.roShiftLayer)().ToList() _
                                                .Any(Function(l) l.LayerType.Equals(roLayerTypes.roLTMandatory))
                            If (shiftLayersMandatory) Then
                                ' Revisamos que las franjas rígidas sean estáticas
                                If oShift.ShiftType = ShiftType.Normal Then
                                    Dim FloatingBeginUpTo = oShift.Layers.Cast(Of Shift.roShiftLayer)().ToList() _
                                                .Any(Function(l) l.Data.Exists("FloatingBeginUpTo"))
                                    Dim FloatingFinishMinutes = oShift.Layers.Cast(Of Shift.roShiftLayer)().ToList() _
                                                .Any(Function(l) l.Data.Exists("FloatingFinishMinutes"))

                                    If Not FloatingBeginUpTo And Not FloatingFinishMinutes Then lstShifts.Add(oShift)
                                End If
                            End If
                        End If
                    Next

                End If

                ' Para cada punche
                For Each Row As DataRow In dtPunches.Rows
                    ' Si cambia el empleado carga los datos de la ficha y de empleado
                    If idEmployeeAnt <> Row("IdEmployee") Then
                        ' Lee los datos del empleado
                        dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), Me.mEndDate, Me.oState)
                        idEmployeeAnt = Row("IdEmployee")
                    End If

                    daEmployees.SelectCommand.Parameters("@idEmployee").Value = Row("idEmployee")
                    daEmployees.SelectCommand.Parameters("@Date").Value = Row("ShiftDate")
                    dtEmployees.Rows.Clear()
                    daEmployees.Fill(dtEmployees)

                    ' Lee los datos del contrato
                    If bolLoadContracts Then
                        daEmployeeContracts.SelectCommand.Parameters("@idEmployee").Value = Row("IdEmployee")
                        daEmployeeContracts.SelectCommand.Parameters("@Date").Value = Row("ShiftDate")
                        dtEmployeeContracts.Rows.Clear()
                        daEmployeeContracts.Fill(dtEmployeeContracts)
                    End If

                    ' Carga datos
                    If Not LoadInfo(dtPunches, Row, dtEmpUsrFields, dtEmployees, dtEmployeeContracts) Then Exit For
                    ' Crea la línea
                    'bolRet = CreateLine(Row)
                    bolRet = Me.Profile.CreateLine()

                Next
                'expoerta los datos de los horarios
                If (bolLoadShiftsDefinition) Then
                    If (lstShifts.Count > 0) Then
                        CreateShiftSheet("Horarios")
                        CreateShiftInfo(lstShifts)
                    End If
                End If

                bolRet = True

                dtEmployees.Dispose()
                dtEmployeeContracts.Dispose()
                dtPunches.Dispose()
                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPunchesEx:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function CreateLine(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                'For i As Integer = 0 To Me.Profile.Fields.Count - 1
                '    Select Case Profile.Fields(i).Source.ToUpper
                '        'Case "FECHA_ASIGNADA" : Profile.Fields(i).Value = cRow("ShiftDate")
                '        'Case "FECHA" : Profile.Fields(i).Value = Any2Time(cRow("DateTime")).DateOnly
                '        'Case "HORA" : Profile.Fields(i).Value = cRow("DateTime")
                '        'Case "TIPO_FICHAJE" : Profile.Fields(i).Value = cRow("Type")
                '        'Case "TIPO_FICHAJE_CALCULADO"
                '        '    Profile.Fields(i).Value = cRow("ActualType")
                '        '    Profile.Fields(i).GetValueFromList = True
                '        'Case "TERMINAL_ID" : Profile.Fields(i).Value = cRow("idTerminal")
                '        'Case "TERMINAL_DESCRIPCION", "TERMINAL_DESCRIPCIÓN" : Profile.Fields(i).Value = Any2String(cRow("TerminalDescription"))
                '        'Case "TERMINAL_LOCALIZACION", "TERMINAL_LOCALIZACIÓN" : Profile.Fields(i).Value = Any2String(cRow("TerminalLocation"))
                '        'Case "JUSTIFICACION_EXPORTARCOMO", "JUSTIFICACIÓN_EXPORTARCOMO" : Profile.Fields(i).Value = Any2String(cRow("Export"))
                '        'Case "JUSTIFICACION_NC", "JUSTIFICACIÓN_NC" : Profile.Fields(i).Value = Any2String(cRow("ShortName"))
                '        'Case "ID", "ID_FICHAJE", "ID FICHAJE" : Profile.Fields(i).Value = Any2String(cRow("ID"))

                '    End Select
                'Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPunchesEx:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function CreateShiftSheet(name As String) As Boolean
            Return Profile.CreateSheet(name)
        End Function

        Private Function CreateShiftInfo(lstShifts As List(Of Shift.roShift)) As Boolean
            Return Profile.CreateShiftLines(lstShifts)
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPunchesEx:CreateDataAdapter_Employees")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_EmployeeContracts(Optional bClearEndContractDate As Boolean = False) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String
                If Not bClearEndContractDate Then
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPunchesEx:CreateDataAdapter_EmployeeContracts")
            End Try

            Return da
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtEmployee As DataTable, ByVal dtEmployeeContracts As DataTable) As Boolean
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
                        Case "FECHA_ASIGNADA" : Profile.Fields(i).Value = row("ShiftDate")
                        Case "FECHA" : Profile.Fields(i).Value = Any2Time(row("DateTime")).DateOnly
                        Case "HORA" : Profile.Fields(i).Value = row("DateTime")
                        Case "TIPO_FICHAJE" : Profile.Fields(i).Value = row("Type")
                        Case "TIPO_FICHAJE_CALCULADO"
                            Profile.Fields(i).Value = row("ActualType")
                            Profile.Fields(i).GetValueFromList = True
                        Case "TERMINAL_ID" : Profile.Fields(i).Value = row("idTerminal")
                        Case "TERMINAL_DESCRIPCION", "TERMINAL_DESCRIPCIÓN" : Profile.Fields(i).Value = Any2String(row("TerminalDescription"))
                        Case "TERMINAL_LOCALIZACION", "TERMINAL_LOCALIZACIÓN" : Profile.Fields(i).Value = Any2String(row("TerminalLocation"))
                        Case "JUSTIFICACION_EXPORTARCOMO", "JUSTIFICACIÓN_EXPORTARCOMO" : Profile.Fields(i).Value = Any2String(row("Export"))
                        Case "JUSTIFICACION_NC", "JUSTIFICACIÓN_NC" : Profile.Fields(i).Value = Any2String(row("ShortName"))
                        Case "JUSTIFICACION_NOMBRE", "JUSTIFICACIÓN_NOMBRE" : Profile.Fields(i).Value = Any2String(row("CauseName"))
                        Case "SUPERVISOR" : Profile.Fields(i).Value = Any2String(row("Supervisor"))
                        Case "CREADO_FECHA" : Profile.Fields(i).Value = Any2String(row("DateCreated"))
                        Case "MODIFICADO_FECHA" : Profile.Fields(i).Value = Any2String(row("DateModified"))
                        Case "GEOLOCALIZACIÓN", "GEOLOCALIZACION" : Profile.Fields(i).Value = Any2String(row("GPS"))
                        Case "ID", "ID_FICHAJE", "ID FICHAJE" : Profile.Fields(i).Value = Any2String(row("ID"))
                        Case "CENTRO_TRABAJO" : Profile.Fields(i).Value = Any2String(row("WorkCenter"))
                        Case "TELETRABAJO" : Profile.Fields(i).Value = If(Any2Boolean(row("InTelecommute")), "X", "")
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate
                        Case "FIABILIDAD"
                            Dim strReliability As String = "High"
                            If Any2Boolean(row("IsNotReliable")) Then
                                strReliability = "Low"
                                Profile.Fields(i).Value = "BAJA"
                            ElseIf Any2Integer(row("Source")) = PunchSource.Request Then
                                strReliability = "Medium"
                                Profile.Fields(i).Value = "MEDIA"
                            Else
                                strReliability = "High"
                                Profile.Fields(i).Value = "ALTA"
                            End If
                            Profile.Fields(i).Value = oState.Language.Translate("roDataLinkExport.ProfileExportPunchesEx.Reliability." & strReliability, "")
                        Case "ORIGEN"
                            Try
                                Profile.Fields(i).Value = If(IsDBNull(row("Source")), "", oState.Language.Translate("roDataLinkExport.ProfileExportPunchesEx.Source." & [Enum].GetName(GetType(PunchSource), row("Source")), ""))
                            Catch ex As Exception
                                Profile.Fields(i).Value = String.Empty
                            End Try
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

                        Case "EMPLEADO_IDENTIFICADOR_INTERNO"
                            Profile.Fields(i).Value = idEmpleado

                        Case "TARJETA"
                            Dim oStateSecurity As New roSecurityState
                            Dim oPassport As roPassport = roPassportManager.GetPassport(idEmpleado, LoadType.Employee, oStateSecurity)
                            If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Length > 0 Then
                                Profile.Fields(i).Value = oPassport.AuthenticationMethods.CardRows(0).Credential
                            End If
                        Case "EQUIVALENCIA"
                            Profile.Fields(i).Value = row("ExportShift")
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
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Profile.Fields(i).Value = "RBS not supported"
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBSB_") Then ' Robotics script base
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(5, Profile.Fields(i).Source.Length - 5)
                                    Profile.Fields(i).Value = "RBS not supported"
                                End If
                            End If
                    End Select
                Next

                bolRet = True

                dt.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportPunchesEx:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Function LoadShiftInfo(dtShiftIds As DataTable) As Boolean

            Return False
        End Function

#End Region

    End Class

End Namespace
