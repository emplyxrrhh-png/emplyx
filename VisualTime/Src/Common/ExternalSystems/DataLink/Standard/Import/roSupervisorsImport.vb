Imports System.Data.Common
Imports System.IO
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace DataLink

    Public Class roSupervisorsImport
        Inherits roDataLinkImport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Sub New(ByVal importType As eImportType, ByVal oImportFile As Byte(), Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(importType, oImportFile, state)
        End Sub

        Public Sub New(ByVal importType As eImportType, ByVal fileNameAsciiOExcelOXML As String, Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(importType, fileNameAsciiOExcelOXML, state)
        End Sub

        Public Sub New(ByVal fileNameAscii As String, ByVal fileNameExcel As String, Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(fileNameAscii, fileNameExcel, state)
        End Sub

#Region "27- IMPORTAR SUPERVISORES EXCEL"

        Public Function ImportSupervisorsExcel() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Dim oUserFieldState As New UserFields.roUserFieldState()
            roBusinessState.CopyTo(Me.State, oUserFieldState)

            Dim oGroupState As New Group.roGroupState()
            roBusinessState.CopyTo(Me.State, oGroupState)

            Dim oEmployeeState As New Employee.roEmployeeState()
            roBusinessState.CopyTo(Me.State, oEmployeeState)

            Try
                If Me.bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(RoboticsExternAccess.SupervisorColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(RoboticsExternAccess.SupervisorColumns)).Length - 1) As String

                    ' Definimos variables para guardar los logs

                    Dim intNewSupervisors As Integer = 0
                    Dim intUpdateSupervisors As Integer = 0
                    Dim intKOs As Integer = 0
                    Dim intRegister2Process As Integer = 0

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer = 0
                        Dim intEndLine As Integer = 0
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        If intLines > 0 Then

                            Dim InvalidColumn As String = ""

                            Dim roleConfigurationAdvancedParameter = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("Custom.PNET.SupervisorImport", New AdvancedParameter.roAdvancedParameterState, ).Value)
                            Dim importRoleId = ""
                            Dim importResponsableType = ""
                            Dim bState = New roGroupFeatureState()
                            Dim VTroles = roGroupFeatureManager.GetGroupFeaturesList(bState)
                            If VTroles IsNot Nothing And VTroles.Length > 0 Then
                                If roleConfigurationAdvancedParameter IsNot Nothing AndAlso roleConfigurationAdvancedParameter.IndexOf("-") > 0 Then
                                    importResponsableType = roleConfigurationAdvancedParameter.Split("-")(0)
                                    importRoleId = VTroles.Where(Function(role) role.Name = roleConfigurationAdvancedParameter.Split("-")(1)).FirstOrDefault().ID
                                    bolRet = True
                                End If
                            End If

                            Dim importCategoriesInfo = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("Custom.PNET.SupervisorImportCategoriesInfo", New AdvancedParameter.roAdvancedParameterState, ).Value)
                            If importCategoriesInfo Is Nothing OrElse importCategoriesInfo.Trim().Length = 0 Then
                                bolRet = False
                            End If

                            If bolRet Then
                                'Contar número de columnas
                                Dim intColumns As Integer = Me.ValidarColumnasSupervisors(False, ColumnsPos, Nothing, Nothing, InvalidColumn)
                                If intColumns > 0 Then

                                    ReDim ColumnsVal(ColumnsPos.Length - 1)
                                    ' Importamos fichero de supervisores
                                    Try


                                        Dim strMsg As String = ""

                                        'Recorrer Hoja Excel
                                        Dim bErrorExists As Boolean = False
                                        Dim supervisorsGroups As DataTable = New DataTable()
                                        supervisorsGroups.Columns.Add("ResponsibleId", GetType(String))
                                        supervisorsGroups.Columns.Add("GroupName", GetType(String))
                                        supervisorsGroups.Columns.Add("ExcelRow", GetType(Integer))
                                        For intRow As Integer = intBeginLine To intEndLine
                                            If Me.GetDataExcelSupervisors(intRow, ColumnsPos, ColumnsVal, importResponsableType) AndAlso ColumnsVal(SupervisorColumns.ResponsibleId) IsNot Nothing Then
                                                Dim newRow As DataRow = supervisorsGroups.NewRow()
                                                newRow("ResponsibleId") = ColumnsVal(SupervisorColumns.ResponsibleId)
                                                newRow("GroupName") = ColumnsVal(SupervisorColumns.GroupName)
                                                newRow("ExcelRow") = intRow
                                                supervisorsGroups.Rows.Add(newRow)

                                            End If
                                        Next
                                        intRegister2Process = supervisorsGroups.Rows.Count
                                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intRegister2Process.ToString & Environment.NewLine
                                        Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                                        Dim processedSupervisorsPassports As List(Of Integer) = New List(Of Integer)
                                        Dim processedSupervisors As List(Of String) = New List(Of String)
                                        For Each row As DataRow In supervisorsGroups.Rows

                                            Try
                                                Dim excelGroupsString = From supervisor As DataRow In supervisorsGroups.AsEnumerable()
                                                                        Where supervisor.Field(Of String)("ResponsibleId") = row("ResponsibleId")
                                                                        Select supervisor.Field(Of String)("GroupName")
                                                If (Not processedSupervisors.Contains(row("ResponsibleId"))) Then
                                                    processedSupervisors.Add(row("ResponsibleId"))
                                                    strMsg = ""
                                                    ' Busca el supervisor
                                                    Dim dtPassportInfo As DataTable = getPassportByUserField(row("ResponsibleId"), oUserFieldState)

                                                    If dtPassportInfo IsNot Nothing AndAlso dtPassportInfo.Rows.Count = 1 Then
                                                        Dim intIDEmployee As Integer = dtPassportInfo.Rows(0).Item("idemployee")

                                                        Dim employeePassport As roPassport = roPassportManager.GetPassport(intIDEmployee, LoadType.Employee)

                                                        Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, oEmployeeState)

                                                        Dim excelGroups As roPassportGroups = New roPassportGroups()
                                                        If employeePassport IsNot Nothing Then
                                                            excelGroups.idPassport = employeePassport.ID
                                                        End If
                                                        excelGroups.GroupRows = New roPassportGroupRow() {}
                                                        For Each excelGroup As String In excelGroupsString
                                                            Dim group As roPassportGroupRow = New roPassportGroupRow()
                                                            If employeePassport IsNot Nothing Then
                                                                group.IDPassport = employeePassport.ID
                                                            End If
                                                            Dim oGroup As VTBusiness.Group.roGroup = VTBusiness.Group.roGroup.GetGroupByName(excelGroup, oGroupState)
                                                            If oGroup IsNot Nothing Then
                                                                group.IDGroup = oGroup.ID
                                                            Else
                                                                Me.State.Result = DataLinkResultEnum.InvalidGroup
                                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NonExistentGroupOnregister", "") & " " & row("ExcelRow") & vbNewLine
                                                                intKOs = intKOs + 1
                                                                bErrorExists = True
                                                            End If
                                                            ReDim Preserve excelGroups.GroupRows(excelGroups.GroupRows.Length)
                                                            excelGroups.GroupRows(excelGroups.GroupRows.Length - 1) = group
                                                        Next

                                                        ' Comprueba si es nuevo o actualización

                                                        If employeePassport Is Nothing OrElse Not employeePassport.IsSupervisor Then
                                                            'Nuevo Supervisor
                                                            Dim intNewPassportID As Integer = -1
                                                            bolRet = Me.NewSupervisor(oEmployee, employeePassport, excelGroups, strMsg, False, intNewPassportID, importRoleId, importCategoriesInfo)
                                                            processedSupervisorsPassports.Add(intNewPassportID)
                                                            If bolRet Then intNewSupervisors = intNewSupervisors + 1
                                                        Else
                                                            Dim bolUpdate As Boolean = False
                                                            If employeePassport.IDGroupFeature = importRoleId Then
                                                                processedSupervisorsPassports.Add(employeePassport.ID)
                                                                If employeePassport.Groups IsNot Nothing AndAlso employeePassport.Groups.GroupRows IsNot Nothing Then
                                                                    If excelGroups.GroupRows.Length <> employeePassport.Groups.GroupRows.Length Then
                                                                        bolUpdate = True
                                                                    Else
                                                                        Dim excelGroupsList As List(Of Integer) = excelGroups.GroupRows.Select(Function(groupRow) groupRow.IDGroup).ToList()
                                                                        Dim employeeGroupsList As List(Of Integer) = employeePassport.Groups.GroupRows.Select(Function(groupRow) groupRow.IDGroup).ToList()
                                                                        excelGroupsList.Sort()
                                                                        employeeGroupsList.Sort()

                                                                        If excelGroupsList.SequenceEqual(employeeGroupsList) Then
                                                                            bolUpdate = False
                                                                        Else
                                                                            bolUpdate = True
                                                                        End If
                                                                    End If
                                                                Else
                                                                    bolUpdate = True
                                                                End If
                                                            Else
                                                                'El rol no es válido
                                                                Me.State.Result = DataLinkResultEnum.InvalidRole
                                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidUserRole", "") & " " & row("ExcelRow") & vbNewLine
                                                                intKOs = intKOs + excelGroupsString.Count
                                                                bErrorExists = True
                                                            End If

                                                            If bolUpdate Then
                                                                bolRet = Me.UpdateSupervisor(employeePassport, excelGroups, strMsg, False)
                                                                If bolRet Then intUpdateSupervisors = intUpdateSupervisors + 1
                                                            End If
                                                        End If
                                                    Else
                                                        'No existe el empleado
                                                        Me.State.Result = DataLinkResultEnum.InvalidEmployee
                                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NonExistentEmployeeOnregister", "") & " " & row("ExcelRow") & vbNewLine
                                                        intKOs = intKOs + excelGroupsString.Count
                                                        bErrorExists = True
                                                    End If
                                                End If
                                            Catch ex As Exception
                                                Me.State.Result = DataLinkResultEnum.Exception
                                                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportSupervisorsExcel")
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & row("ExcelRow") & vbNewLine & ex.Message & vbNewLine
                                                bolRet = False
                                            Finally
                                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                            End Try
                                        Next

                                        'Supervisores con rol gestionado
                                        Dim oPassportManager As roPassportManager = New roPassportManager()
                                        Dim oPassportsIds As List(Of Integer) = oPassportManager.GetPassportsByRole(importRoleId)

                                        Dim idPassports2RemoveGroups As IEnumerable(Of Integer) = oPassportsIds.Except(processedSupervisorsPassports)

                                        For Each passport2Remove As Integer In idPassports2RemoveGroups
                                            Dim employeePassport As roPassport = roPassportManager.GetPassport(passport2Remove, LoadType.Passport)
                                            If employeePassport IsNot Nothing AndAlso employeePassport.Groups IsNot Nothing AndAlso employeePassport.Groups.GroupRows IsNot Nothing AndAlso employeePassport.Groups.GroupRows.Length > 0 Then
                                                bolRet = Me.UpdateSupervisor(employeePassport, Nothing, strMsg, False)
                                                If bolRet Then intUpdateSupervisors = intUpdateSupervisors + 1
                                            End If
                                        Next

                                        If bErrorExists Then
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                        End If
                                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewSupervisors", "") & intNewSupervisors.ToString & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateSupervisors", "") & intUpdateSupervisors.ToString & vbNewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.RegistersNotImported", "") & " " & intKOs & vbNewLine

                                        ' Auditamos importación supervisores
                                        Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                                        Me.State.AddAuditParameter(tbParameters, "{ImportSupervisorsType}", "Excel", "", 1)
                                        Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportEmployees, "", tbParameters, -1) '', oTrans.Connection)

                                    Catch ex As DbException
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportSupervisorsExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    Catch ex As Exception
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportSupervisorsExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    End Try
                                Else ' Columnas inválidas
                                    Me.State.Result = DataLinkResultEnum.InvalidColumns
                                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidColumns", "") & " '" & InvalidColumn & "'" & vbNewLine
                                End If
                            Else
                                Me.State.Result = DataLinkResultEnum.MandatoryParameterNotDefined
                                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.MandatoryParameterNotDefined", "") & vbNewLine
                            End If
                        Else ' No has registros
                            Me.State.Result = DataLinkResultEnum.NoRegisters
                            strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoRegisters", "") & vbNewLine
                        End If
                    Else ' No hay ningún libro en el fichero excel
                        Me.State.Result = DataLinkResultEnum.NoSheets
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoSheetsInExcelFile", "") & vbNewLine
                    End If
                Else
                    ' Fichero Excel inválido
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportSupervisorsExcel")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba el log
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function ValidarColumnasSupervisors(ByVal bolAnchoFijo As Boolean, ByRef ColumnsPos() As Integer, ByVal ColumnsInitialPos() As Integer, ByVal ColumnsLenght() As Integer, ByRef InvalidColumn As String) As Integer
            Dim intNumColumnas As Integer = -1
            Dim bolIsValid As Boolean = True
            Dim intCol As Integer = 0
            Dim IntRow As Integer = 0
            Dim Column As String = ""
            Dim Size As Integer = 0
            Dim x As Integer = 0
            Dim l As Integer = 1

            Try

                ' Inicializa columnas
                For intCol = 0 To ColumnsPos.Length - 1
                    ColumnsPos(intCol) = -1
                Next

                InvalidColumn = ""

                ' Determina la posición inicial de lectura de la plantilla
                intCol = 0
                IntRow = 0
                If bolAnchoFijo = False Then
                    ' Lee todas las columnas de la plantilla para fichero delimitado por caracter separador o excel
                    Column = GetCellValueWithoutFormat(IntRow, intCol)

                    Do While Column <> String.Empty And bolIsValid
                        bolIsValid = ValidarColumnasSupervisors_GetColumnInfo(Column, x, ColumnsPos)
                        If bolIsValid Then
                            intCol += 1
                        Else
                            InvalidColumn = Column
                        End If

                        x += 1

                        Column = GetCellValueWithoutFormat(IntRow, intCol)
                    Loop
                Else
                    IntRow += 1

                    ' Lee todas las columnas de la plantilla para fichero de ancho fijo
                    Column = GetCellValueWithoutFormat(IntRow, intCol)

                    Do While Column <> String.Empty And bolIsValid
                        bolIsValid = ValidarColumnasSupervisors_GetColumnInfo(Column, x, ColumnsPos)
                        If bolIsValid Then
                            Size = roConversions.Any2Double(GetCellValueWithoutFormat(IntRow, intCol + 1))
                            ColumnsInitialPos(x) = l
                            ColumnsLenght(x) = Size
                            l = l + Size

                            bolIsValid = (Size <> 0)
                        Else
                            InvalidColumn = Column
                        End If

                        If bolIsValid Then IntRow += 1
                        x += 1

                        Column = GetCellValueWithoutFormat(IntRow, intCol)
                    Loop
                End If

            Catch ex As Exception
                InvalidColumn = "Some kind of error ... "
            End Try

            intNumColumnas = x

            Return intNumColumnas
        End Function

        Private Function ValidarColumnasSupervisors_GetColumnInfo(ByVal Column As String, ByVal intCol As Integer, ByRef ColumnsPos() As Integer) As Boolean
            Dim bolIsValid As Boolean = True

            Dim originalColName As String = Column

            Column = Column.ToString.ToUpper

            Select Case Column
                Case "NOMBRE UNIDAD ORGANIZATIVA" : ColumnsPos(RoboticsExternAccess.SupervisorColumns.GroupName) = intCol
                Case "TIPO RESPONSABLE" : ColumnsPos(RoboticsExternAccess.SupervisorColumns.ResponsibleType) = intCol
                Case "ID RESPONSABLE" : ColumnsPos(RoboticsExternAccess.SupervisorColumns.ResponsibleId) = intCol
            End Select

            Return bolIsValid
        End Function

        Private Function GetDataExcelSupervisors(ByVal intRow As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String, ByVal responsableTypeParameterValue As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                If ColumnsVal(SupervisorColumns.ResponsibleType).Length > 0 And roTypes.Any2String(ColumnsVal(SupervisorColumns.ResponsibleType)) = responsableTypeParameterValue Then
                    bolRet = True
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelSupervisors")
            End Try

            Return bolRet

        End Function

        Public Function getPassportByUserField(ByVal strPNUserFieldValue As String, ByRef oEmployeeState As UserFields.roUserFieldState) As DataTable
            Dim tbSupervisor As DataTable = Nothing

            If strPNUserFieldValue <> String.Empty Then
                tbSupervisor = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(Me.strPNResponsableIDUserField, strPNUserFieldValue, Now, oEmployeeState)

            End If

            If tbSupervisor IsNot Nothing AndAlso tbSupervisor.Rows.Count > 0 Then Return tbSupervisor Else Return Nothing

        End Function

        Private Function NewSupervisor(ByVal oEmployee As Robotics.Base.VTEmployees.Employee.roEmployee, ByVal oEmployeePassport As roPassport, ByVal oGroups As roPassportGroups, ByRef strMsg As String, ByVal CallBroadcaster As Boolean, ByRef newPassportID As Integer, ByVal importRoleId As Integer, ByVal importCategoriesInfo As String) As Boolean
            Dim bolRet As Boolean = False
            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Me.State.UpdateStateInfo()
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oSecurityState As New roSecurityState(Me.State.IDPassport, Me.State.Context)
            strMsg = ""
            Dim strErrorInfo As String = ""
            Dim oFeatures As UsersAdmin.Feature() = Nothing

            Dim oNewPassport As New roPassport

            Dim oParentPassport As roPassport = Nothing

            oNewPassport.Name = oEmployee.Name
            oNewPassport.Description = ""
            oNewPassport.IDGroupFeature = importRoleId

            oNewPassport.GroupType = ""


            oNewPassport.IDEmployee = oEmployee.ID

            If oEmployeePassport IsNot Nothing Then
                oNewPassport.Language = oEmployeePassport.Language
            Else
                oNewPassport.Language = New roPassportLanguage()
            End If
            oNewPassport.State = 1
            oNewPassport.IsSupervisor = True

            If oEmployeePassport IsNot Nothing Then
                oNewPassport.EnabledVTDesktop = oEmployeePassport.EnabledVTDesktop
                oNewPassport.EnabledVTPortal = oEmployeePassport.EnabledVTPortal
                oNewPassport.EnabledVTPortalApp = oEmployeePassport.EnabledVTPortalApp
                oNewPassport.EnabledVTVisits = oEmployeePassport.EnabledVTVisits
                oNewPassport.EnabledVTVisitsApp = oEmployeePassport.EnabledVTVisitsApp

                oNewPassport.LocationRequiered = oEmployeePassport.LocationRequiered
                oNewPassport.PhotoRequiered = oEmployeePassport.PhotoRequiered
            Else
                oNewPassport.EnabledVTDesktop = True
                oNewPassport.EnabledVTPortal = True
                oNewPassport.EnabledVTPortalApp = True
                oNewPassport.EnabledVTVisits = True
                oNewPassport.EnabledVTVisitsApp = False

                oNewPassport.LocationRequiered = True
                oNewPassport.PhotoRequiered = True
            End If

            If strErrorInfo = "" Then
                ' Cargamos información de los métodos de identificación
                If oEmployeePassport IsNot Nothing AndAlso oEmployeePassport.AuthenticationMethods IsNot Nothing Then

                    oNewPassport.AuthenticationMethods = oEmployeePassport.AuthenticationMethods

                    If oNewPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                        For Each oBio In oNewPassport.AuthenticationMethods.BiometricRows
                            oBio.IDPassport = oNewPassport.ID
                            oBio.RowState = RowState.UpdateRow
                        Next
                    End If

                    If oNewPassport.AuthenticationMethods.PasswordRow IsNot Nothing Then
                        oNewPassport.AuthenticationMethods.PasswordRow.IDPassport = oNewPassport.ID
                        oNewPassport.AuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow
                    End If

                    If oNewPassport.AuthenticationMethods.CardRows IsNot Nothing Then
                        For Each oCard In oNewPassport.AuthenticationMethods.CardRows
                            oCard.IDPassport = oNewPassport.ID
                            oCard.RowState = RowState.UpdateRow
                        Next

                    End If

                End If

                oFeatures = UsersAdmin.Business.FeaturesBusiness.GetFeaturesFromPassportAll(oEmployeePassport.ID, "E", oSecurityState).ToArray()

                Dim oPassportManager As New roPassportManager

                Dim bolDelete As Boolean = oPassportManager.Delete(oEmployeePassport, True)

                oNewPassport.IsSupervisor = True


                bolRet = oPassportManager.Save(oNewPassport, False)

                newPassportID = oNewPassport.ID

                If strErrorInfo = "" Then
                    ' Guardamos el passport


                    oGroups.idPassport = newPassportID

                    For Each oGroup As roPassportGroupRow In oGroups.GroupRows
                        oGroup.IDPassport = newPassportID
                    Next

                    oNewPassport.Groups = oGroups

                    Dim passportCategoryRows As New List(Of roPassportCategoryRow)

                    Using jsonReader As JsonReader = New JsonTextReader(New StringReader(importCategoriesInfo))
                        While jsonReader.Read() AndAlso jsonReader.TokenType <> JsonToken.StartArray
                        End While

                        If jsonReader.TokenType = JsonToken.StartArray Then

                            jsonReader.Read()

                            While jsonReader.TokenType <> JsonToken.EndArray
                                Dim row As New roPassportCategoryRow()

                                While jsonReader.Read() AndAlso jsonReader.TokenType <> JsonToken.EndObject
                                    If jsonReader.TokenType = JsonToken.PropertyName Then
                                        Dim propertyName As String = jsonReader.Value.ToString()

                                        Select Case propertyName
                                            Case "IDCategory"
                                                jsonReader.Read()
                                                row.IDCategory = CType(jsonReader.Value, Integer)
                                            Case "LevelOfAuthority"
                                                jsonReader.Read()
                                                row.LevelOfAuthority = CType(jsonReader.Value, Byte)
                                            Case "ShowFromLevel"
                                                jsonReader.Read()
                                                row.ShowFromLevel = CType(jsonReader.Value, Byte)
                                        End Select
                                    End If
                                End While
                                row.IDPassport = oNewPassport.ID
                                passportCategoryRows.Add(row)

                                ' Avanzar al siguiente elemento del arreglo JSON
                                jsonReader.Read()
                            End While

                        End If
                    End Using
                    oNewPassport.Categories = New roPassportCategories()
                    oNewPassport.Categories.CategoryRows = passportCategoryRows.ToArray()
                    oNewPassport.Categories.idPassport = oNewPassport.ID

                    bolRet = oPassportManager.Save(oNewPassport, False)


                    ' Ejecutamos la regla de bot del tipo copiar permisos y funcionalidades en caso que este activa
                    Dim oLicense As New roServerLicense
                    Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")
                    If bolIsInstalledBots Then
                        Try
                            Dim oBotState As New roBotState(-1)
                            Dim oBotManager As New roBotManager(oBotState)
                            Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String)
                            _oParameters.Add(BotRuleParameterEnum.DestinationSupervisor, oNewPassport.ID.ToString)
                            oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopySupervisorPermissions, _oParameters)
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "Creating new passport " & oNewPassport.Name & ": Error executing bot", ex)
                        End Try
                    End If

                    If oFeatures IsNot Nothing AndAlso oFeatures.Length > 0 AndAlso oNewPassport.IDEmployee.HasValue AndAlso oNewPassport.IDEmployee > 0 Then
                        Dim oFeaturesChanged As List(Of Robotics.UsersAdmin.Feature) = Nothing
                        For Each oFeature As Robotics.UsersAdmin.Feature In oFeatures
                            Dim oPerm As Permission = Permission.None

                            Select Case oFeature.ObjectValue
                                Case 0
                                    oPerm = Permission.None
                                Case 3
                                    oPerm = Permission.Read
                                Case 6
                                    oPerm = Permission.Write
                                Case 9
                                    oPerm = Permission.Admin
                                Case Else
                                    oPerm = Permission.None
                            End Select

                            If oPerm <> Permission.None Then UsersAdmin.Business.FeaturesBusiness.SetFeaturePermission(oNewPassport.ID, oFeature.ID, "E", oPerm, oFeaturesChanged, oSecurityState)
                        Next
                    End If
                End If
            End If


            If Not bolRet Then
                strMsg = "Error Updating Employee '" & oNewPassport.Name & "': " & Me.State.Result.ToString & " - " & Me.State.ErrorText

                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        If oEmployeeState.Result <> EmployeeResultEnum.NoError Then strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        If oGroupState.Result <> GroupResultEnum.NoError Then strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                End Select
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If
            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError

            Return bolRet

        End Function
        Private Function UpdateSupervisor(ByVal oPassport As roPassport, ByVal oGroups As roPassportGroups, ByRef strMsg As String, ByVal CallBroadcaster As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Me.State.UpdateStateInfo()
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            strMsg = ""

            Dim oPassportManager As New roPassportManager

            oPassport.Groups = oGroups

            bolRet = oPassportManager.Save(oPassport, False)


            If Not bolRet Then
                strMsg = "Error Updating Employee '" & oPassport.Name & "': " & Me.State.Result.ToString & " - " & Me.State.ErrorText

                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        If oEmployeeState.Result <> EmployeeResultEnum.NoError Then strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        If oGroupState.Result <> GroupResultEnum.NoError Then strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                End Select
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If
            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError

            Return bolRet

        End Function

#End Region

    End Class
End Namespace