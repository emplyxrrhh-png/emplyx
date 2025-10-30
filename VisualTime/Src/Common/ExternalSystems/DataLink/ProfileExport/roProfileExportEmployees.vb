Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class ProfileExportEmployees

#Region "Declarations - Constructor"

        Private mEmployeeTempTableName As String = ""
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

    Private mBreakByContracts As Boolean = True
    Private mBreakByMobilities As Boolean = True
    Private mBreakByUsrFieldsValues As Boolean = True

    Private oState As roDataLinkState

    Private oLog As New roLog("ProfileExportEmployees")

        Public Sub New(ByVal employeeTempTableName As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
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

    Public Property BreakByContracts As Boolean
        Get
            Return mBreakByContracts
        End Get
        Set(value As Boolean)
            mBreakByContracts = value
        End Set
    End Property

    Public Property BreakByMobilities As Boolean
        Get
            Return mBreakByMobilities
        End Get
        Set(value As Boolean)
            mBreakByMobilities = value
        End Set
    End Property

    Public Property BreakByUsrFieldsValues As Boolean
        Get
            Return mBreakByUsrFieldsValues
        End Get
        Set(value As Boolean)
            mBreakByUsrFieldsValues = value
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
                ' Importación automática
                Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                Me.mEndDate = Me.mBeginDate.AddMonths(1)
                Me.mEndDate = Me.mEndDate.AddDays(-1)
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
            Dim dtLogin As New DataTable
            Dim dtLanguage As New DataTable
            Dim dtAuthorizations As New DataTable

            Dim importKeyFieldName As String = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value)

            ' Para cada registro
            Dim n, iTotalEmployees As Integer
            n = 0
            iTotalEmployees = dtReg.Rows.Count

            ' Para cada registro
            For Each Row As DataRow In dtReg.Rows

                n += 1
                oLog.logMessage(roLog.EventType.roDebug, "Exporting data for employee number " & n.ToString & " out of " & iTotalEmployees.ToString)

                ' Carga los contratos del empleado
                dtContracts = DataLinkDynamicCode.CreateDataTable_Contracts(Row("ID"), Me.oState)

                ' Carga los niveles a los que pertenece el empleado a fecha de hoy
                dtGroups = DataLinkDynamicCode.CreateDataTable_Groups(Row("ID"), Me.oState)

                ' Carga el histórico de movilidades del empleado
                dtMobilities = DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(Row("ID"), Me.oState)

                ' Carga la ficha del empleado
                dtEmpUserFieldValues = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("ID"), Me.mEndDate, Me.oState)

                ' Carga el login del empleado
                dtLogin = DataLinkDynamicCode.CreateDataTable_Login(Row("ID"), Me.oState)

                ' Carga el idioma del empleado
                dtLanguage = DataLinkDynamicCode.CreateDataTable_Language(Row("ID"), Me.oState)

                'Autorizaciones de acceso del empleado
                dtAuthorizations = DataLinkDynamicCode.CreateDataTable_Authorizations(Row("ID"), Me.oState)

                ' Carga los datos de empleados, tareas y campos de la ficha
                If Not LoadInfo(dtReg, Row, dtEmpUserFieldValues, dtContracts, dtGroups, dtMobilities, dtLogin, dtLanguage, dtAuthorizations, importKeyFieldName, Me.mEndDate) Then Exit For

                ' Crea la línea
                'CreateLine(Row)
            Next

            dtContracts.Dispose()
            dtGroups.Dispose()
            dtEmpUserFieldValues.Dispose()

            bolRet = True
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:ExportProfile")
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
                If Me.mEmployeeTempTableName <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = Employees.Id "
            Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "ProfileExportEmployees::SelectRegisters", ex)
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:FieldTypeExists")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:ExportOneConceptByLine")
        End Try

        Return bolRet
    End Function

    Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal oEmployee As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtContracts As DataTable, ByVal dtGroups As DataTable, ByVal dtMobilities As DataTable, ByVal dtLogin As DataTable, ByVal dtLanguage As DataTable, ByVal dtAuthorizations As DataTable, ByVal strImportKeyFieldName As String, AtDate As Date) As Boolean
        Dim bolRet As Boolean = False
        Dim i As Integer = 0

        Try

            ' Si es preciso, creo matriz de roturas. Nos permitirá saber cuántas filas voy a tener por empleado
            ' Inicialmente pongo en una lista todas las fecha de inicio y fin. Para distinguirlas, las fechas de inicio serán a las 00:00, y las de fin a las 01:00
            Dim oBreaksDateList As New List(Of DateTime)
            Dim bFillGaps As Boolean = False

            'Añadimos la fecha de inicio y final de la exportación
            oBreaksDateList.Add(mBeginDate)
            oBreaksDateList.Add(mEndDate.AddHours(1))

            ' 0.- Roturas por Contratos
            If mBreakByContracts AndAlso dtContracts.Rows.Count > 0 Then
                RefreshBreaksDateList(dtContracts, oBreaksDateList, True)
            End If

            '1.- Roturas por Movilidades
            If mBreakByMobilities AndAlso dtMobilities.Rows.Count > 0 Then
                RefreshBreaksDateList(dtMobilities, oBreaksDateList, False)
            End If

            ' 2.- Roturas por Campos de la ficha
            ' Consideraré sólo los campos de la ficha que se hayan incluido en la exportación
            Dim dtEmployeeUserFieldValuesHistory As DataTable

            ' Miro primero si debo cargar TODOS los campos de la ficha
            Dim lUsr As New List(Of String)
            Dim bIncludeAllUserFields As Boolean = False
            For i = 0 To Me.Profile.Fields.Count - 1
                If Me.Profile.Fields(i).Source.StartsWith("USR_*") Then
                    bIncludeAllUserFields = True
                    Exit For
                End If
            Next

            Dim sFilter As String = String.Empty
            Dim lFieldNames As New List(Of String)
            If Not bIncludeAllUserFields Then
                For i = 0 To Me.Profile.Fields.Count - 1
                    If Me.Profile.Fields(i).Source.StartsWith("USR_") Then
                        lUsr.Add(Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4))
                        ' Si el campo tiene histórico, también añado la fecha del valor
                    Else
                        ' Si se incluyo la clave de importación, la añado también
                        If Me.Profile.Fields(i).Source.ToUpper = "PRIMARYKEY" AndAlso strImportKeyFieldName.Length > 0 Then lUsr.Add(strImportKeyFieldName)
                    End If
                Next

                For Each sFieldName As String In lUsr
                    If sFilter <> String.Empty Then sFilter = sFilter & ","
                    sFilter = sFilter & "'" & sFieldName & "'"
                Next
            End If

            dtEmployeeUserFieldValuesHistory = DataLinkDynamicCode.CreateDataTable_EmployeeUserFieldValuesHistory(oEmployee("ID"), sFilter, oState)
            If mBreakByUsrFieldsValues AndAlso dtEmployeeUserFieldValuesHistory.Rows.Count > 0 Then
                RefreshBreaksDateList(dtEmployeeUserFieldValuesHistory, oBreaksDateList, False)
            End If

            ' 4.- Ahora a paritr de la lista de fechas, creo la matriz de roturas
            Dim dtBreaks As New DataTable
            dtBreaks = GetBreaksDateTable(oBreaksDateList)

            ' 5.- Cargo información e imprimo lineas
            Dim dtGroupsOnDate As New DataTable
            Dim oContractRow As DataRow = Nothing
            Dim oMobilityRow As DataRow = Nothing
            Dim oUserFieldValueOnDateRow As DataRow = Nothing
            Dim sUsrFldName As String

            For Each oBreakRow In dtBreaks.Rows

                ' Recupero contrato en función del periodo de rotura
                ' TODO: Si no hay rotura por contrato, devuelvo el último contrato activo en el periodo
                If mBreakByContracts Then
                    oContractRow = GetContractOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtContracts)
                Else
                    oContractRow = GetLastContractOnPeriod(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtContracts)
                End If

                ' Sólo muestro líneas si en el periodo hay contrato
                If Not oContractRow Is Nothing Then

                    ' Recupero información de departamento en la fecha del periodo
                    dtGroupsOnDate = DataLinkDynamicCode.CreateDataTable_GroupsOnDate(oEmployee("ID"), oBreakRow("EndDate"), oState)

                    ' Recupero información del grupo
                    If mBreakByMobilities Then
                        oMobilityRow = GetMobilityOnDate(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtMobilities)
                    Else
                        oMobilityRow = GetLastmobilityOnPeriod(oBreakRow("BeginDate"), oBreakRow("EndDate"), dtMobilities)
                    End If

                    ' Para cada columna
                    For i = 0 To Me.Profile.Fields.Count - 1
                        Me.Profile.Fields(i).Value = ""

                        Select Case Me.Profile.Fields(i).Source.ToUpper
                            Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                                Me.Profile.Fields(i).Value = mBeginDate
                            Case "FECHAINICIOROTURA"
                                Me.Profile.Fields(i).Value = oBreakRow("BeginDate")
                            Case "FECHAFINROTURA"
                                Me.Profile.Fields(i).Value = oBreakRow("EndDate")
                            Case "FECHAINICIOMOVILIDAD"
                                Me.Profile.Fields(i).Value = oMobilityRow("BeginDate")
                            Case "FECHAFINMOVILIDAD"
                                Dim objDate As Date = roTypes.Any2DateTime(oMobilityRow("EndDate"))
                                If objDate.Year = 2079 AndAlso objDate.Month = 1 And objDate.Day = 1 Then
                                    Me.Profile.Fields(i).Value = ""
                                Else
                                    Me.Profile.Fields(i).Value = oMobilityRow("EndDate")
                                End If
                            Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                                Me.Profile.Fields(i).Value = mEndDate
                            Case "NOMBRE"
                                Me.Profile.Fields(i).Value = oEmployee("Name")
                            Case "CONTRATO"
                                Me.Profile.Fields(i).Value = oContractRow("IDContract")
                            Case "CENTRO_TRABAJO"
                                Me.Profile.Fields(i).Value = oContractRow("Enterprise")
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
                            Case "USUARIO"
                                Me.Profile.Fields(i).Value = ""
                                If dtLogin.Rows.Count > 0 Then
                                    If Not IsDBNull(dtLogin.Rows(0)("Login")) Then Me.Profile.Fields(i).Value = dtLogin.Rows(0)("Login")
                                End If
                            Case "IDIOMA"
                                Me.Profile.Fields(i).Value = ""
                                If dtLanguage.Rows.Count > 0 Then
                                    If Not IsDBNull(dtLanguage.Rows(0)("LanguageKey")) Then Me.Profile.Fields(i).Value = dtLanguage.Rows(0)("LanguageKey")
                                End If
                            Case "TARJETA"
                                Me.Profile.Fields(i).Value = ""
                                If dtLogin.Rows.Count > 0 Then
                                    If Not IsDBNull(dtLogin.Rows(0)("Card")) Then Me.Profile.Fields(i).Value = dtLogin.Rows(0)("Card")
                                End If
                            Case "GRUPOUSUARIOS"
                                Me.Profile.Fields(i).Value = ""
                                If dtLogin.Rows.Count > 0 Then
                                    If Not IsDBNull(dtLogin.Rows(0)("Name")) Then Me.Profile.Fields(i).Value = dtLogin.Rows(0)("Name")
                                End If
                            Case "AUTORIZACIONES"
                                Me.Profile.Fields(i).Value = ""
                                If dtAuthorizations.Rows.Count > 0 Then
                                    Dim strTmpAuthorizations As String = String.Empty
                                    For Each oRow As DataRow In dtAuthorizations.Rows
                                        strTmpAuthorizations &= oRow("ShortName") & ","
                                    Next
                                    If strTmpAuthorizations <> String.Empty Then
                                        strTmpAuthorizations = strTmpAuthorizations.Substring(0, strTmpAuthorizations.Length - 1)
                                    End If
                                    Me.Profile.Fields(i).Value = strTmpAuthorizations
                                End If
                            Case Else
                                ' Determina el tipo de campo
                                If Me.Profile.Fields(i).Source.ToUpper.StartsWith("USR_") Then
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
                                ElseIf Profile.Fields(i).Source.ToUpper.Equals("CENTROCOSTE") Then
                                    If dtGroupsOnDate.Rows.Count > 0 Then
                                        Profile.Fields(i).Value = GetBusinessCenterOnLevel(dtGroupsOnDate.Rows.Count - 1, dtGroupsOnDate)
                                    Else
                                        Profile.Fields(i).Value = ""
                                    End If
                                ElseIf Profile.Fields(i).Source.ToUpper.StartsWith("CENTROCOSTENIVEL") Then
                                    Dim iLevel As Integer = roTypes.Any2Integer(Me.Profile.Fields(i).Source.Substring(16, Me.Profile.Fields(i).Source.Length - 16))
                                    Profile.Fields(i).Value = GetBusinessCenterOnLevel(iLevel, dtGroupsOnDate)
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    If mBreakByMobilities OrElse mBreakByContracts OrElse mBreakByUsrFieldsValues Then
                                        ' Con alguna rotura
                                        ' Profile.Fields(i).Value = "RBS not supported"
                                        ' Profile.Fields(i).Value = DataLinkDynamicCode.GetCellValueFromExternalCode(scriptFileName, Profile.Fields, dtRowsToExport, oEmployee, dtEmployeeUserFieldValuesHistory, dtContracts, dtGroupsOnDate,
                                        '                           DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(oEmployee("ID"), oCn, Me.oState), dtAuthorizations, Me.mBeginDate, Me.mEndDate)
                                        Dim cellValueByHead = New DataLink.CellValueFromCode
                                        Profile.Fields(i).Value = cellValueByHead.GetValue(scriptFileName, Profile.Fields, dtRowsToExport, oEmployee, dtEmployeeUserFieldValuesHistory, dtContracts, dtGroupsOnDate,
                                                                   DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(oEmployee("ID"), Me.oState), dtAuthorizations, Me.mBeginDate, Me.mEndDate)
                                    Else
                                        ' Sin roturas. Por compatibilidad dejo la llamada como estaba
                                        ' Profile.Fields(i).Value = "RBS not supported"
                                        ' Profile.Fields(i).Value = DataLinkDynamicCode.GetCellValueFromExternalCode(scriptFileName, Profile.Fields, dtRowsToExport, oEmployee, dtEmpUsrFields, dtContracts, dtGroups,
                                        '                           DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(oEmployee("ID"), oCn, Me.oState), dtAuthorizations, Me.mBeginDate, Me.mEndDate)
                                        Dim cellValueByHead = New DataLink.CellValueFromCode
                                        Profile.Fields(i).Value = cellValueByHead.GetValue(scriptFileName, Profile.Fields, dtRowsToExport, oEmployee, dtEmpUsrFields, dtContracts, dtGroups,
                                                                   DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(oEmployee("ID"), Me.oState), dtAuthorizations, Me.mBeginDate, Me.mEndDate)
                                    End If
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBSB_") Then ' Robotics script base
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(5, Profile.Fields(i).Source.Length - 5)
                                    Profile.Fields(i).Value = "RBS not supported"
                                End If
                        End Select

                    Next
                    CreateLine(oEmployee)
                End If
            Next

            bolRet = True
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:LoadInfo")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:AddPeriodToBreaksTable")
        End Try

    End Sub

    ''' <summary>
    ''' Añade a la lista de roturas las fechas de la tabla proporcionada. La tabla debe obligatoriamente tener una columna BeginDate, y otra EndDate
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <param name="oBreaksDateList"></param>
    Private Sub RefreshBreaksDateList(dTable As DataTable, ByRef oBreaksDateList As List(Of DateTime), bFillgaps As Boolean)
        Try
            Dim oRow As DataRow
            Dim dRowBeginDate, dLastRowBeginDate As DateTime
            Dim dRowEndDate, dLastRowEndDate As DateTime
            Dim oDataView As System.Data.DataView = New System.Data.DataView(dTable)
            oDataView.Sort = "BeginDate ASC"
            bFillgaps = True
            Dim bIsFirstPeriod As Boolean = False
            Dim bIsLastPeriod As Boolean = False
            ' Recorro los contratos
            For i = 0 To oDataView.ToTable.Rows.Count - 1
                oRow = oDataView.ToTable.Rows(i)
                dRowBeginDate = roTypes.Any2DateTime(oRow("BeginDate"))
                dRowEndDate = roTypes.Any2DateTime(oRow("EndDate"))

                If i = 0 Then bIsFirstPeriod = True
                If i = oDataView.ToTable.Rows.Count - 1 Then bIsLastPeriod = True

                AddPeriodToBreaksTable(dRowBeginDate, dRowEndDate, mBeginDate, mEndDate, bIsFirstPeriod, bIsLastPeriod, oBreaksDateList)

                If bFillgaps AndAlso Not bIsFirstPeriod AndAlso Not bIsLastPeriod Then
                    ' Si los diversos contratos no son consecutivos en el tiempo, debo crear periodos de NO contrato
                    If dLastRowEndDate.AddDays(1) <> dRowBeginDate Then
                        AddPeriodToBreaksTable(dLastRowEndDate.AddDays(1), dRowBeginDate.AddDays(-1).AddHours(1), mBeginDate, mEndDate, bIsFirstPeriod, bIsLastPeriod, oBreaksDateList)
                    End If
                End If

                dLastRowBeginDate = dRowBeginDate
                dLastRowEndDate = dRowEndDate
            Next
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:RefreshBreaksTable")
        End Try
    End Sub

    Private Function GetBreaksDateTable(oDateList As List(Of DateTime)) As DataTable
        Dim dtBreaks As DataTable = Nothing
        Try
            Dim oBreaksRow As DataRow
            dtBreaks = CreateDataTable("@SELECT# getdate() as BeginDate, getdate() as EndDate")
            dtBreaks.Rows.Clear()
            oDateList.Sort()

            Dim bLastIsStart As Boolean = True
            Dim dDate As DateTime
            Dim dLastDate As DateTime = DateTime.MinValue
            Dim dStartDate As DateTime
            Dim dEndDate As DateTime

            For n = 0 To oDateList.Count - 1
                oBreaksRow = dtBreaks.NewRow
                dDate = oDateList(n)
                Select Case dDate.Hour
                    Case 0
                        ' Inicio de periodo
                        If dDate <> dLastDate Then dStartDate = dDate
                    Case 1
                        ' Fin de periodo
                        If dDate <> dLastDate Then
                            dEndDate = dDate
                            oBreaksRow("BeginDate") = dStartDate
                            oBreaksRow("EndDate") = dEndDate.AddHours(-1)
                            dtBreaks.Rows.Add(oBreaksRow)
                        End If
                End Select
                dLastDate = dDate
            Next
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetBreaksDateTable")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetContractOnDate")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetLastContractOnPeriod")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetUserFieldValueOnDate")
        End Try
        Return oRet
    End Function

    Private Function GetBusinessCenterOnLevel(iLevel As Integer, dtGroups As DataTable) As String
        Dim oRet As String = String.Empty
        Try
            If dtGroups.Rows.Count >= (iLevel + 1) Then
                Dim sValue As String = String.Empty
                Dim j As Integer = iLevel
                sValue = roTypes.Any2String(dtGroups.Rows(iLevel)("BusinessCenterName"))
                While sValue.Trim = "" AndAlso j > 0
                    j = j - 1
                    sValue = roTypes.Any2String(dtGroups.Rows(j)("BusinessCenterName"))
                    If sValue.Trim <> "" Then Exit While
                End While
                oRet = sValue
            End If
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetUserFieldValueOnDate")
        End Try
        Return oRet
    End Function

    Private Function GetMobilityOnDate(dStartDate As DateTime, dEndDate As DateTime, dtGroups As DataTable) As DataRow
        Dim oRet As DataRow = Nothing
        Try
            For Each oMobilityRow As DataRow In dtGroups.Rows
                If oMobilityRow("BeginDate") <= dStartDate AndAlso oMobilityRow("EndDate") >= dEndDate Then
                    oRet = oMobilityRow
                    Exit For
                End If
            Next
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetMobilityOnDate")
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
            Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetLastmobilityOnPeriod")
        End Try
        Return oMobilityRow
    End Function

#End Region

End Class

End Namespace