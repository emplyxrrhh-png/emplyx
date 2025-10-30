Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Mail
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roEmployeeImport
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

#Region "20- IMPORTAR EMPLEADOS EXCEL"

        Public Function ImportEmployeesExcel() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As String

                    ' Definimos array con los nombres de los campos personalizados
                    Dim ColumnsUsrName(-1) As String

                    ' Definimos variables para guardar los logs

                    Dim intNewEmployees As Integer = 0
                    Dim intUpdateEmployees As Integer = 0
                    Dim intDeletedEmployees As Integer = 0
                    Dim intKOs As Integer = 0

                    Dim intNewGroups As Integer = 0
                    Dim intUpdateGroups As Integer = 0

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer = 0
                        Dim intEndLine As Integer = 0
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then

                            Dim CompositeContractType As Integer = 0
                            Dim EmpresaColumnPosition As Integer = -1
                            Dim bolClonar As Boolean = False
                            Dim InvalidColumn As String = ""
                            Dim IsGroupImport As Boolean = False
                            Dim bolFiels_OnlyUpdate As Boolean = False

                            'Contar número de columnas
                            Dim intColumns As Integer = Me.ValidarColumnasEmployees(False, ColumnsPos, ColumnsUsrName, CompositeContractType, EmpresaColumnPosition, bolClonar, Nothing, Nothing, InvalidColumn, IsGroupImport, bolFiels_OnlyUpdate)
                            If intColumns > 0 Then

                                ReDim ColumnsVal(ColumnsPos.Length - 1)
                                Dim strDeletedEmployeesInfo As String = String.Empty
                                If Not IsGroupImport Then
                                    ' Importamos fichero de usuarios
                                    Try

                                        Dim oEmployeeState As New Employee.roEmployeeState
                                        roBusinessState.CopyTo(Me.State, oEmployeeState)

                                        Dim oUserFieldState As New UserFields.roUserFieldState()
                                        roBusinessState.CopyTo(Me.State, oUserFieldState)

                                        Dim oContractState = New Contract.roContractState()
                                        roBusinessState.CopyTo(Me.State, oContractState)

                                        Dim tbUserFieldList As DataTable = UserFields.roUserField.GetUserFields(Types.EmployeeField, oUserFieldState)
                                        createMandatoryFields(oEmployeeState, oUserFieldState, False)
                                        ' Solo creamos los campos de la ficha en caso necesario
                                        If Not bolFiels_OnlyUpdate Then createExcelUserFields(ColumnsVal, ColumnsUsrName, oEmployeeState, oUserFieldState, False)

                                        bolRet = True

                                        ' Grupos de usuario
                                        Dim dtUserGroups As DataTable = GetUserGroups()

                                        ' Convenios
                                        Dim dtLabAgrees As DataTable = GetLabAgrees()

                                        Dim strMsg As String = ""

                                        ' Determina si se sobrepasaría licencia de máximo número de empleados
                                        bolRet = ImportEmployeesEXCEL_CheckMaxEmployeeNotExceded(intBeginLine, intEndLine, intColumns, ColumnsPos, ColumnsVal, CompositeContractType, bolClonar, EmpresaColumnPosition, RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey, RoboticsExternAccess.EmployeeColumns.DNI, oEmployeeState, strLogEvent)
                                        If bolRet = False Then Exit Try

                                        Dim newCompanyIds As New Generic.List(Of Integer)

                                        'Recorrer Hoja Excel
                                        Dim bErrorExists As Boolean = False
                                        For intRow As Integer = intBeginLine To intEndLine
                                            Dim errorInfo As String = String.Empty
                                            If Me.GetDataExcelEmployees(intRow, intColumns, ColumnsPos, ColumnsVal, CompositeContractType, EmpresaColumnPosition, errorInfo) Then

                                                Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                                                Try
                                                    strMsg = ""
                                                    ' Busca el empleado
                                                    Dim intIDEmployee As Integer = isEmployeeNew(ColumnsVal, RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey, RoboticsExternAccess.EmployeeColumns.DNI, oUserFieldState)
                                                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.RegisterType).ToUpper() = "BORRADO" Then
                                                        errorInfo = Me.State.Language.Translate("Import.LogEvent.DeleteOperation", "") & " " & Me.State.Language.Translate("Import.LogEvent.UpdatedUserFields", "")
                                                        bolRet = False
                                                        If intIDEmployee > 0 Then
                                                            ' Si existe el usuario, borramos el contrato o el usuario en caso que sea el último contrato
                                                            Dim oContract As New Contract.roContract(oContractState, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract))
                                                            If oContract.Load(False) AndAlso oContract.IDEmployee = intIDEmployee Then
                                                                Dim strDeletedDetail As String = ""
                                                                Dim oContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(intIDEmployee, oContractState)
                                                                If oContracts IsNot Nothing AndAlso oContracts.Rows.Count = 1 Then
                                                                    ' Como solo queda un contrato, borramos el usuario
                                                                    bolRet = Employee.roEmployee.DeleteEmployee(intIDEmployee, oEmployeeState, False)
                                                                    strDeletedDetail = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name)
                                                                Else
                                                                    ' En caso contrario, borramos solo el contrato indicado
                                                                    bolRet = oContract.Delete(True)
                                                                    strDeletedDetail = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name) & "(" & oContract.IDContract & ")"
                                                                End If
                                                                If bolRet Then
                                                                    intDeletedEmployees = intDeletedEmployees + 1
                                                                    If strDeletedEmployeesInfo = String.Empty Then
                                                                        strDeletedEmployeesInfo = strDeletedDetail
                                                                    Else
                                                                        strDeletedEmployeesInfo = strDeletedEmployeesInfo & " - " & strDeletedDetail
                                                                    End If
                                                                End If
                                                            Else
                                                                ' el contrato no existe o el contrato no pertenece a ese usuario
                                                                bolRet = False
                                                                strMsg = "Error deleting Employee '" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name) & "-" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "': contract does not exist for the indicated employee"
                                                            End If
                                                        Else
                                                            ' el empleado no existe no borramos nada
                                                            bolRet = False
                                                            strMsg = "Error deleting Employee '" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name) & "': employee does not exist"
                                                        End If
                                                    Else
                                                        If intIDEmployee = -1 Then
                                                            'Nuevo Empleado
                                                            errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "") & " "
                                                            bolRet = Me.NewEmployee(ColumnsVal, ColumnsUsrName, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strMsg, False, bolFiels_OnlyUpdate)
                                                            If bolRet Then intNewEmployees = intNewEmployees + 1
                                                        Else
                                                            errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.UpdateOperation", "") & " "
                                                            bolRet = Me.UpdateEmployee(intIDEmployee, ColumnsVal, ColumnsUsrName, ColumnsPos, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strMsg, False, bolFiels_OnlyUpdate, False)
                                                            If bolRet Then intUpdateEmployees = intUpdateEmployees + 1
                                                        End If
                                                    End If

                                                    If bolRet = False Or Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved Then
                                                        bErrorExists = True
                                                        If Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved Then
                                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterImportedWithoutData", "") & " " & intRow & " " & errorInfo & vbNewLine
                                                        Else
                                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow & " " & errorInfo & vbNewLine
                                                            intKOs = intKOs + 1
                                                        End If
                                                        Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                        msgLog &= strMsg & vbNewLine
                                                    End If
                                                Catch ex As Exception
                                                    Me.State.Result = DataLinkResultEnum.Exception
                                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesExcel")
                                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & " " & errorInfo & vbNewLine & ex.Message & vbNewLine
                                                    bolRet = False
                                                Finally
                                                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                                End Try
                                            Else
                                                bErrorExists = True
                                                Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                                msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & " " & errorInfo & vbNewLine
                                                intKOs += 1
                                            End If
                                        Next

                                        If bErrorExists Then
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                        End If
                                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewEmployees", "") & intNewEmployees.ToString & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateEmployees", "") & intUpdateEmployees.ToString & vbNewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.DeletedEmployees", "") & intDeletedEmployees.ToString & vbNewLine & strDeletedEmployeesInfo & vbNewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.RegistersNotImported", "") & " " & intKOs & vbNewLine

                                        ' Auditamos importación empleados
                                        Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                                        Me.State.AddAuditParameter(tbParameters, "{ImportEmployeesType}", "Excel", "", 1)
                                        Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportEmployees, "", tbParameters, -1) '', oTrans.Connection)

                                        ' Llama al BroadCaster
                                        If intNewEmployees > 0 Or intUpdateEmployees > 0 Or intDeletedEmployees > 0 Then
                                            Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                                        End If
                                    Catch ex As DbException
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    Catch ex As Exception
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    End Try
                                Else
                                    ' Importamos fichero de departamentos
                                    Try
                                        Dim strMsg As String = ""

                                        'Recorrer Hoja Excel
                                        Dim newCompanyIds As New Generic.List(Of Integer)
                                        Dim bErrorExists As Boolean = False
                                        For intRow As Integer = intBeginLine To intEndLine
                                            If Me.GetDataExcelGroups(intRow, intColumns, ColumnsPos, ColumnsVal) Then

                                                Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                                                Try
                                                    strMsg = ""

                                                    ' Busca el grupo
                                                    Dim oGroup As Group.roGroup = Group.roGroup.GetGroupByKey(ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel), New Group.roGroupState(-1))

                                                    If oGroup Is Nothing Then
                                                        'Nuevo grupo
                                                        bolRet = Me.NewGroup(ColumnsVal, newCompanyIds, strMsg)
                                                        If bolRet Then intNewGroups = intNewGroups + 1
                                                    Else
                                                        bolRet = Me.UpdateGroup(oGroup, ColumnsVal, newCompanyIds, strMsg)
                                                        If bolRet Then intUpdateGroups = intUpdateGroups + 1
                                                    End If

                                                    If bolRet = False Then
                                                        bErrorExists = True
                                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow & vbNewLine
                                                        Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                        msgLog &= strMsg & vbNewLine
                                                    End If
                                                Catch ex As Exception
                                                    Me.State.Result = DataLinkResultEnum.Exception
                                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportGroupsExcel")
                                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                                    bolRet = False
                                                Finally
                                                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                                End Try
                                            Else
                                                bErrorExists = True
                                                Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                                msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & vbNewLine
                                                intKOs += 1
                                            End If
                                        Next

                                        If bErrorExists Then
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                        End If
                                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewGroups", "") & intNewGroups.ToString & Environment.NewLine
                                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateGroups", "") & intUpdateGroups.ToString & vbNewLine

                                        ' Auditamos importación empleados
                                        Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                                        Me.State.AddAuditParameter(tbParameters, "{ImportGroupsType}", "Excel", "", 1)
                                        Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportGroups, "", tbParameters, -1)
                                    Catch ex As DbException
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportGroupsExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    Catch ex As Exception
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportGroupsExcel")
                                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    End Try
                                End If
                            Else ' Columnas inválidas
                                Me.State.Result = DataLinkResultEnum.InvalidColumns
                                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidColumns", "") & " '" & InvalidColumn & "'" & vbNewLine
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
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesExcel")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba el log
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function ValidarColumnasEmployees(ByVal bolAnchoFijo As Boolean, ByRef ColumnsPos() As Integer, ByRef ColumnsUsrName() As String, ByRef CompositeContractType As Integer, ByRef EmpresaColumnPosition As Integer, ByRef bolClone As Boolean, ByVal ColumnsInitialPos() As Integer, ByVal ColumnsLenght() As Integer, ByRef InvalidColumn As String, ByRef IsGroupImport As Boolean, ByRef bolFiels_OnlyUpdate As Boolean) As Integer
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
                CompositeContractType = 0
                EmpresaColumnPosition = -1
                IsGroupImport = False

                ' Determina la posición inicial de lectura de la plantilla
                intCol = 0
                IntRow = 0
                If bolAnchoFijo = False Then
                    ' Lee todas las columnas de la plantilla para fichero delimitado por caracter separador o excel
                    Column = GetCellValueWithoutFormat(IntRow, intCol)

                    Do While Column <> String.Empty And bolIsValid
                        bolIsValid = ValidarColumnasEmployees_GetColumnInfo(Column, x, ColumnsPos, ColumnsUsrName, CompositeContractType, EmpresaColumnPosition, bolClone, IsGroupImport, bolFiels_OnlyUpdate)
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
                        bolIsValid = ValidarColumnasEmployees_GetColumnInfo(Column, x, ColumnsPos, ColumnsUsrName, CompositeContractType, EmpresaColumnPosition, bolClone, IsGroupImport, bolFiels_OnlyUpdate)
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

                If bolIsValid Then
                    If Not IsGroupImport Then
                        ' Comprueba columnas obligatorias para importar usuarios
                        If ColumnsPos(RoboticsExternAccess.EmployeeColumns.Contract) >= 0 AndAlso (ColumnsPos(RoboticsExternAccess.EmployeeColumns.DNI) >= 0 OrElse ColumnsPos(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) >= 0) _
                                        AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.BeginDate) >= 0 AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.EndDate) >= 0 AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.Name) >= 0 Then
                            intNumColumnas = x
                        Else
                            InvalidColumn = "Required User Fields"
                        End If
                    Else
                        ' Comprueba columnas obligatorias para importar departamentos
                        If ColumnsPos(RoboticsExternAccess.EmployeeColumns.IDLevel) >= 0 AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.IDParentLevel) >= 0 AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.Name) >= 0 _
                                AndAlso ColumnsPos(RoboticsExternAccess.EmployeeColumns.Contract) = -1 Then
                            intNumColumnas = x
                        Else
                            InvalidColumn = "Required Group Fields"
                        End If

                    End If
                End If
            Catch ex As Exception
                InvalidColumn = "Some kind of error ... "
            End Try

            Return intNumColumnas
        End Function

        Private Function ValidarColumnasEmployees_GetColumnInfo(ByVal Column As String, ByVal intCol As Integer, ByRef ColumnsPos() As Integer, ByRef ColumnsUsrName() As String, ByRef CompositeContractType As Integer, ByRef EmpresaColumnPosition As Integer, ByRef bolClone As Boolean, ByRef IsGroupImport As Boolean, ByRef bolFiels_OnlyUpdate As Boolean) As Boolean
            Dim bolIsValid As Boolean = True

            Dim originalColName As String = Column

            Column = Column.ToString.ToUpper

            Select Case Column
                Case "CONTRATO" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Contract) = intCol
                Case "NIF" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.DNI) = intCol
                Case "FECHA ALTA" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.BeginDate) = intCol
                Case "FECHA BAJA" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.EndDate) = intCol
                Case "NOMBRE" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Name) = intCol
                Case "APELLIDO1" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Surname1) = intCol
                Case "APELLIDO2" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Surname2) = intCol
                Case "TARJETA" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Card) = intCol
                Case "NIVEL0" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level0) = intCol
                Case "NIVEL1" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level1) = intCol
                Case "NIVEL2" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level2) = intCol
                Case "NIVEL3" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level3) = intCol
                Case "NIVEL4" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level4) = intCol
                Case "NIVEL5" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level5) = intCol
                Case "NIVEL6" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level6) = intCol
                Case "NIVEL7" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level7) = intCol
                Case "NIVEL8" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level8) = intCol
                Case "NIVEL9" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level9) = intCol
                Case "NIVEL10" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Level10) = intCol
                Case "FECHANIVEL10" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.DateLevel10) = intCol
                Case "CONTRATO_FECHA ALTA" : CompositeContractType = 1
                Case "EMPRESA_CONTRATO_FECHA ALTA" : CompositeContractType = 2
                Case "CONTRATO_PERIODO" : CompositeContractType = 3
                Case "USUARIO" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Login) = intCol
                Case "CLONAR" : bolClone = True
                Case "CONVENIO" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Convenio) = intCol
                Case "CENTRO_TRABAJO" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.WorkCenter) = intCol
                Case "CONTRATO_MOTIVO_FIN" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.EndContractReason) = intCol
                Case "GRUPOUSUARIOS" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.GrupoUsuarios) = intCol
                Case "IMPORT_KEY" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) = intCol
                Case "AUTORIZACIONES" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Authorizations) = intCol
                Case "PERIODO" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Period) = intCol
                Case "IDIOMA" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.Language) = intCol
                Case "IDNIVEL" : ColumnsPos(RoboticsExternAccess.EmployeeColumns.IDLevel) = intCol
                Case "IDNIVELPADRE"
                    ColumnsPos(RoboticsExternAccess.EmployeeColumns.IDParentLevel) = intCol
                    IsGroupImport = True
                Case "IDENTIFICADOR", "FECHA DE CORTE"
                    bolIsValid = True
                Case "FICHA_SOLOACTUALIZAR"
                    ' Solo se actualizan los campos que ya existen en VT, el resto se ignoran (ni se crean ni se asigna el valor)
                    bolFiels_OnlyUpdate = True
                Case "TIPO DE REGISTRO"
                    ColumnsPos(RoboticsExternAccess.EmployeeColumns.RegisterType) = intCol
                Case Else
                    If Column.StartsWith("USR_") Then
                        ReDim Preserve ColumnsPos(ColumnsPos.Length)

                        ' Si existe el campo empresa nos guardamos la posicion
                        If Column = "USR_EMPRESA" Then
                            EmpresaColumnPosition = ColumnsPos.Length - 1
                        End If

                        ColumnsPos(ColumnsPos.Length - 1) = intCol

                        ' Guardamos el nombre del campo de la ficha
                        ReDim Preserve ColumnsUsrName(ColumnsUsrName.Length)
                        ColumnsUsrName(ColumnsUsrName.Length - 1) = originalColName.Substring(4)
                    Else
                        bolIsValid = False
                    End If
            End Select

            Return bolIsValid
        End Function

        Private Function GetDataExcelEmployees(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String,
                                                   ByRef CompositeContractType As Integer, ByRef EmpresaColumnPosition As Integer, Optional ByRef errorInfo As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate).Length > 0 Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.BeginDate", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate) & " "
                End If

                If (ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey).Length > 0) Then
                    errorInfo = errorInfo & Me.State.Language.Translate("Import.LogEvent.ImportPrimaryKey", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract).Length > 0 Then
                    errorInfo = errorInfo & Me.State.Language.Translate("Import.LogEvent.IDContract", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate).Length > 0 And (ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI).Length > 0 OrElse ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey).Length > 0) And
                           ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name).Length > 0 And ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract).Length > 0 Then

                    If CompositeContractType > 0 Then
                        If CompositeContractType <> 3 Then
                            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                                bolRet = True
                                Dim strBeginDate As String = ""
                                '"yyyy/MM/dd"
                                strBeginDate = Format(CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)), "yyyy/MM/dd")
                                strBeginDate = strBeginDate.Replace("/", "")

                                Select Case CompositeContractType
                                    Case 1 'CONTRATO_FECHA ALTA
                                        ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & strBeginDate
                                    Case 2 '"EMPRESA_CONTRATO_FECHA ALTA"
                                        If EmpresaColumnPosition <> -1 Then
                                            If ColumnsVal(EmpresaColumnPosition).Length > 0 Then
                                                ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(EmpresaColumnPosition) & "." & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & strBeginDate
                                            Else
                                                bolRet = False
                                            End If
                                        Else
                                            bolRet = False
                                        End If
                                End Select
                            Else
                                bolRet = False
                            End If
                        Else
                            bolRet = True
                            ' CONTRATO_PERIODO
                            Try
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Period).Length > 0 Then
                                    ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Period)
                                Else
                                    bolRet = False
                                End If
                            Catch ex As Exception
                                bolRet = False
                            End Try

                        End If
                    Else
                        bolRet = True
                    End If

                End If

                'If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate).Length > 0 Then
                ' ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate) = DateTime.FromOADate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
                ' End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelEmployees")
            End Try

            Return bolRet

        End Function

        Protected Friend Function NewEmployee(ByVal ColumnsVal() As String, ByVal ColumnsUsrName() As String, ByVal tbUserFieldList As DataTable, ByRef newCompanyIDs As Generic.List(Of Integer), ByVal dtUserGroups As DataTable, ByVal dtLabAgree As DataTable, ByRef strMsg As String, ByVal CallBroadcaster As Boolean, ByVal bolFiels_OnlyUpdate As Boolean) As Boolean

            Dim bolRet As Boolean = False
            Dim bolContinue As Boolean = True

            Dim idColumnInitialDate As Short

            Dim invalidDateField As String = String.Empty

            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oContractState = New Contract.roContractState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oUserFIeldState As New UserFields.roUserFieldState
            Dim DateField As Date
            Dim lNullDateStrings As String() = Nothing
            Dim sDateFormat As String = String.Empty

            Dim notSavedUserFields As String = String.Empty

            roBusinessState.CopyTo(Me.State, oUserFIeldState)

            Me.State.UpdateStateInfo()
            strMsg = ""

            'Recuperamos la fecha de congelación
            Dim bContinue As Boolean = True
            Dim xFreezeDate As Date = roParameters.GetFirstDate()

            ' Obtener el nombre completo del empleado
            Dim strName As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Surname1) & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Surname2)
            If strName.Trim <> "" Then
                strName &= ", "
            Else
                strName = ""
            End If

            ' Cargamos las posibles representaciones de la fecha sin informar
            Dim param1 As New AdvancedParameter.roAdvancedParameter("NullDateStrings", New AdvancedParameter.roAdvancedParameterState)
            If Any2String(param1.Value).Trim.Length > 0 Then
                lNullDateStrings = Any2String(param1.Value).Split("@")
            End If

            'Leemos la fecha de inicio de contrato/movilidad
            Dim xBeginDate As Date

            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                xBeginDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
            Else
                bolContinue = False
                Me.State.Result = DataLinkResultEnum.FieldDataIncorrect
                invalidDateField = "Fecha Alta"
            End If
            'End If

            'Leemos la fecha de fin de contrato/movilidad
            Dim xEndDate As Date
            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate)) Then
                xEndDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))
            Else
                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) = String.Empty OrElse (Not lNullDateStrings Is Nothing AndAlso lNullDateStrings.Contains(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))) Then
                    xEndDate = New Date(2079, 1, 1)
                Else
                    bolContinue = False
                    Me.State.Result = DataLinkResultEnum.FieldDataIncorrect
                    invalidDateField = invalidDateField & If(invalidDateField = String.Empty, "", ", ") & "Fecha Baja"
                End If

            End If

            Dim sWorkCenter As String = String.Empty
            sWorkCenter = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.WorkCenter))

            Dim EnabledVTDesktop As Boolean = True
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop) IsNot String.Empty Then
                EnabledVTDesktop = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop))
            End If
            Dim EnabledVTPortal As Boolean = True
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal) IsNot String.Empty Then
                EnabledVTPortal = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal))
            End If
            Dim EnabledVTPortalApp As Boolean = True
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp) IsNot String.Empty Then
                EnabledVTPortalApp = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp))
            End If
            Dim EnabledVTVisits As Boolean = True
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits) IsNot String.Empty Then
                EnabledVTVisits = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits))
            End If
            Dim LoginWithoutContract As Boolean = False
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract) IsNot String.Empty Then
                LoginWithoutContract = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract))
            End If
            Dim UserPhoto As String = ""
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto) IsNot String.Empty Then
                UserPhoto = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto))
            End If
            Dim EnableBiometricData As Boolean = True
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData) IsNot String.Empty Then
                EnableBiometricData = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData))
            End If

            Dim sEndContractReason As String = String.Empty
            sEndContractReason = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndContractReason))

            Dim sPinValue As String = String.Empty
            sPinValue = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Pin))

            If bolContinue Then
                Dim bolIsExpiredContract As Boolean = False
                If xEndDate < Now.Date Then
                    Me.State.Result = DataLinkResultEnum.ExpiredContract
                    bolContinue = False
                End If

                If xBeginDate <= xFreezeDate Then
                    Me.State.Result = DataLinkResultEnum.FreezeDateException
                    bolContinue = False
                End If

                If bolContinue Then

                    strName &= ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name)

                    Dim oEmployee As New Employee.roEmployee

                    oEmployee.ID = -1
                    oEmployee.Name = strName
                    oEmployee.Type = "A"

                    If UserPhoto IsNot Nothing And UserPhoto <> "" Then
                        Try
                            Dim bytes As Byte() = Convert.FromBase64String(UserPhoto)
                            oEmployee.Image = roImagesHelper.ResizeImageIfNeeded(bytes, 200, 200)
                        Catch ex As Exception
                            Me.State.Result = DataLinkResultEnum.InvalidPhotoData
                            Return False
                        End Try
                    End If

                    bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, CallBroadcaster)

                    If bolRet Then
                        bolRet = createNIFIfNecessary(ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                        If bolRet AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) <> String.Empty Then
                            bolRet = CreateImportPrimaryKeyIfNecessary(ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                        End If

                        ' Grabar campos ficha en caso necesario
                        Dim oUserField As UserFields.roUserField = Nothing
                        If bolRet Then
                            Dim strUsrValue As String = ""
                            Dim strUsrName As String = ""
                            Dim intUsrCol As Integer = 0
                            Dim bSaveField As Boolean = True
                            For intCol As Integer = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length To ColumnsVal.Length - 1
                                bSaveField = True
                                strUsrValue = ColumnsVal(intCol)
                                strUsrName = ColumnsUsrName(intUsrCol)
                                If strUsrValue <> "" AndAlso strUsrName <> "" Then
                                    intUsrCol += 1
                                    ' Si no es un campo de la ficha del empleado que define su fecha inicial
                                    If InitialDateField_Is(strUsrName) = False Then
                                        ' Solo generamos/actualizamos el valor en caso necesario
                                        Dim bolApplyChange As Boolean = True
                                        If bolFiels_OnlyUpdate Then
                                            ' Si esta indicada la opcion de solo actualizar campos existentes,
                                            ' en caso que el campo no exista en VT no se debe hacer nada
                                            Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                                            If oFields.Length = 0 Then bolApplyChange = False
                                        End If
                                        If bolApplyChange Then
                                            ' Busca la columna que determina la fecha inicial del campo de usuario
                                            idColumnInitialDate = InitialDateField_GetColumn(strUsrName, ColumnsUsrName)
                                            If idColumnInitialDate <> -1 Then
                                                Dim xHistoryDate As Date = Now.Date

                                                Dim id As Short = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length + idColumnInitialDate
                                                If IsDate(ColumnsVal(id)) Then xHistoryDate = ColumnsVal(id)

                                                If xHistoryDate <= xFreezeDate Then
                                                    notSavedUserFields &= strUsrName & ","
                                                    bSaveField = False
                                                End If
                                            End If
                                            If bSaveField Then
                                                oUserField = New UserFields.roUserField(oUserFIeldState, strUsrName, Types.EmployeeField, False, False)
                                                If oUserField.History Then
                                                    DateField = Now.Date
                                                    If idColumnInitialDate <> -1 Then
                                                        Dim id As Short = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length + idColumnInitialDate
                                                        If IsDate(ColumnsVal(id)) Then DateField = ColumnsVal(id)
                                                    End If
                                                Else
                                                    DateField = New Date(1900, 1, 1)
                                                End If
                                                Dim bolUsedInProcess As Boolean = False
                                                Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                                                If oFields.Length > 0 Then
                                                    If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                                        bolUsedInProcess = True
                                                    End If
                                                End If

                                                'Valido el formato de los campo de tipo periodo de fecha
                                                Dim strUsrValueTmp As String = strUsrValue
                                                Dim ImportUserField As Boolean = True
                                                If oUserField.FieldType = FieldTypes.tDatePeriod Then
                                                    If strUsrValue.Trim.Length > 0 Then
                                                        ' Formato debe ser yyyy/MM/dd*yyyy/MM/dd
                                                        For Each sDate As String In strUsrValue.Split("*")
                                                            If sDate.Trim.Length > 0 AndAlso Not IsDate(sDate) Then
                                                                ImportUserField = False
                                                                notSavedUserFields &= strUsrName & ","
                                                                Exit For
                                                            Else
                                                                If IsDate(sDate) Then strUsrValueTmp = strUsrValueTmp.Replace(sDate, Format(CDate(sDate), "yyyy/MM/dd"))
                                                            End If
                                                        Next
                                                        strUsrValue = strUsrValueTmp
                                                    End If
                                                ElseIf oUserField.FieldType = FieldTypes.tDecimal Then
                                                    If strUsrValue.Trim.Length > 0 Then
                                                        ' Separador decimal debe ser un .
                                                        strUsrValue = strUsrValueTmp.Replace(roConversions.GetDecimalDigitFormat(), ".")
                                                    End If
                                                ElseIf oUserField.FieldType = FieldTypes.tDate Then
                                                    If IsDate(strUsrValueTmp) Then strUsrValue = Format(CDate(strUsrValueTmp), "yyyy/MM/dd")
                                                End If

                                                If ImportUserField Then
                                                    Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, strUsrName, DateField, New UserFields.roUserFieldState)
                                                    oEmployeeUserField.FieldValue = strUsrValue
                                                    If Not oEmployeeUserField.Save(, , bolUsedInProcess) Then
                                                        notSavedUserFields &= strUsrName & ","
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    intUsrCol += 1
                                End If
                            Next
                        End If
                        If Not bolRet AndAlso Me.State.Result = DataLinkResultEnum.NoError Then Me.State.Result = DataLinkResultEnum.InvalidEmployee
                        If bolRet Then
                            Dim intIDGroup As Integer
                            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel).Trim.Length > 0 Then
                                ' Solo asignamos el usuario al grupo en caso que exista dicho grupo, no lo creamos
                                Dim oGroup = Group.roGroup.GetGroupByKey(ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel), New Group.roGroupState(-1))
                                If oGroup IsNot Nothing AndAlso oGroup.ID > 0 Then
                                    intIDGroup = oGroup.ID
                                Else
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidGroup
                                    oGroupState.Result = GroupResultEnum.GroupNotExists
                                    oGroupState.ErrorText = oGroupState.ErrorText & "(" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel) & ")"
                                End If
                            Else
                                ' Creamos los departamentos en caso necesario a partir de los niveles (NIVEL0,NIVEL1, ...)   y asignamos el usuario al grupo
                                Dim invalidLevel As String = String.Empty
                                If ValidateDepartaments(ColumnsVal, invalidLevel, oGroupState) Then
                                    bolRet = CreateDepartaments(ColumnsVal, ColumnsUsrName, oGroupState, intIDGroup, newCompanyIDs)
                                Else
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidGroup
                                    oGroupState.Result = GroupResultEnum.GroupNotExists
                                    oGroupState.ErrorText = oGroupState.ErrorText & "(" & invalidLevel & ")"
                                End If
                            End If

                            If bolRet And intIDGroup <> 0 Then
                                '+++++ MOVILIDAD DEL EMPLEADO ++++

                                ' Insertamos al empleado en la tabla de Empleados y grupos
                                Dim oMobility As New Employee.roMobility
                                With oMobility
                                    .IdGroup = intIDGroup
                                    .BeginDate = xBeginDate
                                    .EndDate = New Date(2079, 1, 1)
                                    .IsTransfer = False
                                End With
                                bolRet = Employee.roMobility.SaveMobility(oEmployee.ID, oMobility, oEmployeeState, CallBroadcaster, True)
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidMovility

                                ' ++++ CONTRATOS ++++
                                If bolRet Then
                                    'Insertamos el empleado en la tabla de contratos
                                    Dim oContract As New Contract.roContract(oContractState, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract))
                                    With oContract
                                        .IDEmployee = oEmployee.ID
                                        .BeginDate = xBeginDate
                                        .EndDate = xEndDate
                                        .Enterprise = sWorkCenter
                                        .IDCard = Nothing
                                        .EndContractReason = sEndContractReason

                                        ' Asigna el convenio
                                        If Not IsNothing(dtLabAgree) And ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) <> "" Then
                                            ' Busca el convenio
                                            Dim rw() As DataRow
                                            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).IndexOf("'") >= 0 Then
                                                ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).Replace("'", "''")
                                            End If
                                            rw = dtLabAgree.Select("Name='" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) & "'")
                                            If rw.Length = 1 Then
                                                Dim lbAgr As New LabAgree.roLabAgree
                                                lbAgr.ID = rw(0)("id")
                                                lbAgr.Load(False)
                                                .LabAgree = lbAgr
                                            Else
                                                bolRet = False
                                                Me.State.Result = DataLinkResultEnum.InvalidLabAgree
                                            End If
                                        End If
                                    End With
                                    If bolRet Then
                                        If Not oContract.Save(, CallBroadcaster) Then
                                            Me.State.Result = DataLinkResultEnum.InvalidContract
                                            bolRet = False
                                        End If
                                    End If
                                Else
                                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                                End If

                                ' Habilitamos el fichaje con tarjeta en caso necesario
                                If bolRet Then
                                    Dim strIDCard As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Card)
                                    If Not mCustomizationCode.ToUpper = "VPA" Then
                                        If strIDCard.Length > 0 Then
                                            If IsNumeric(strIDCard) Then
                                                bolRet = CreateCardAuthenticationMethods(oEmployee, strIDCard)
                                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidCard
                                            Else
                                                Me.State.Result = DataLinkResultEnum.InvalidCard
                                                bolRet = False
                                            End If
                                        End If
                                    Else
                                        ' Personalización. Las tarjetas son Mifare y llegan con código HEXA girado de byte a byte
                                        If strIDCard.Length > 0 Then
                                            If System.Text.RegularExpressions.Regex.IsMatch(strIDCard, "^[0-9a-fA-F]+$") Then
                                                ' 1.- Giro de dos bytes en dos bytes
                                                Dim sRotatedHex As String = String.Empty
                                                sRotatedHex = RotateHex(strIDCard)
                                                ' 2.- Paso a codificación
                                                bolRet = CreateCardAuthenticationMethods(oEmployee, Any2String(Convert.ToInt64(sRotatedHex, 16)))
                                            Else
                                                Me.State.Result = DataLinkResultEnum.InvalidCard
                                                bolRet = False
                                            End If
                                        End If
                                    End If
                                End If

                                If bolRet Then bolRet = CreatePassportIfNeeded(oEmployee)

                                ' Habilitamos el fichaje por huella
                                If bolRet Then
                                    Dim oParameters As New roParameters("OPTIONS", True)

                                    'Si esta activado la biometria globalmente y le pasas por parametro True, creamos biometria
                                    If Not roTypes.Any2Boolean(oParameters.Parameter(Parameters.DisableBiometricData)) AndAlso EnableBiometricData Then
                                        bolRet = CreateBioAuthenticationMethods(oEmployee)
                                    End If
                                End If

                                If bolRet Then
                                    If sPinValue IsNot Nothing And sPinValue <> "" Then
                                        If IsNumeric(sPinValue) AndAlso (sPinValue.Length >= 4 AndAlso sPinValue.Length <= 6) Then
                                            bolRet = CreateOrUpdatePinAuthenticationMethods(oEmployee, sPinValue)
                                        Else
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.InvalidPinLength
                                        End If
                                    End If
                                End If

                                ' Habilitamos el fichaje por login
                                If bolRet Then
                                    Dim strLogin As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Login)
                                    If strLogin.Length > 0 Then
                                        bolRet = CreateLoginAuthenticationMethods(oEmployee, oEmployeeState, strLogin, True)
                                        If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidLogin
                                    End If
                                End If
                                If bolRet Then
                                    Dim strLanguage As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Language)
                                    If strLanguage.Length > 0 Then
                                        bolRet = UpdateEmployeeLanguage(oEmployee, oEmployeeState, strLanguage, True)
                                        If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidLanguage
                                    End If
                                End If
                                If bolRet Then
                                    ' Si se tiene que importar el grupo de usuario lo asigna
                                    ' Si es seguridad modo v2 , esto no aplica
                                    Dim bolApplyUserGroup As Boolean = False
                                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.GrupoUsuarios) <> "" AndAlso bolApplyUserGroup Then

                                    End If
                                End If

                                '+++++ AUTORIZACIONES DE ACCESO ++++
                                If bolRet AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.Authorizations) <> "" Then
                                    If Not CreateAuthorizations(oEmployee.ID, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Authorizations), oEmployeeState) Then
                                        Me.State.Result = DataLinkResultEnum.AuthorizationError
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    ' Planificamos al empleado en caso necesario con la planificacion del empleado plantilla
                                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNICopyPlan) <> "" Then
                                        ' Obtenemos el empleado plantilla
                                        Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNICopyPlan), Now, oUserFIeldState)
                                        Dim intOriginEmployee As Integer = 0
                                        If tbEmployee.Rows.Count > 0 Then
                                            intOriginEmployee = tbEmployee.Rows(0).Item("idemployee")
                                        End If
                                        If intOriginEmployee > 0 AndAlso intOriginEmployee <> oEmployee.ID Then
                                            ' En el caso de encontrar el empleado plantilla
                                            ' copiamos su planificacion al nuevo empleado
                                            Dim intIDPassport As Integer = roConstants.GetSystemUserId()
                                            If intIDPassport > 0 Then
                                                Dim oCalendarState As New roCalendarState(intIDPassport)
                                                Dim oCalendarManager As New roCalendarManager(oCalendarState)
                                                Dim oParameters As New roCollection
                                                oParameters.Add("FromDateDestination", xBeginDate.ToString("yyyy/MM/dd HH:mm")) 'inicio de contrato
                                                oParameters.Add("BeginDateSource", xBeginDate.ToString("yyyy/MM/dd HH:mm")) 'Inicio de contrato

                                                ' Obtenemos el ultimo dia planificado del empleado plantilla
                                                Dim strSQLEndSource As String = "@SELECT# TOP 1 Date from DailySchedule WHERE IDEmployee=" & intOriginEmployee.ToString & " Order by Date desc"
                                                Dim dEndSource As DataTable = CreateDataTable(strSQLEndSource)
                                                Dim xEndSource As Date = xBeginDate
                                                If dEndSource IsNot Nothing AndAlso dEndSource.Rows.Count > 0 Then
                                                    xEndSource = dEndSource.Rows(0)("Date")
                                                    If xEndSource < xBeginDate Then xEndSource = xBeginDate
                                                End If

                                                oParameters.Add("EndDateSource", xEndSource.ToString("yyyy/MM/dd HH:mm")) ' ultimo dia planificado
                                                oParameters.Add("lstOriginEmployees", intOriginEmployee.ToString)
                                                oParameters.Add("lstDestEmployees", oEmployee.ID.ToString)
                                                oParameters.Add("RepeatMode", 0)
                                                oParameters.Add("RepeatModeValue", "1")
                                                oParameters.Add("RepeatStartMode", 0)
                                                oParameters.Add("RepeatStartModeValue", "0")
                                                oParameters.Add("RepeatSkipMode", 0)
                                                oParameters.Add("RepeatSkipTimes", 0)
                                                oParameters.Add("RepeatSkipModeValue", "0")
                                                oParameters.Add("BlockedMode", 0)
                                                oParameters.Add("HolidaysMode", 0)
                                                oParameters.Add("CopyMainShifts", 1)
                                                oParameters.Add("CopyHolidays", 0)
                                                oParameters.Add("LockDestDays", 0)

                                                oCalendarManager.CopyPlanv2(oParameters, oCalendarState, False)

                                            End If
                                        ElseIf intOriginEmployee = oEmployee.ID Then
                                            Me.State.Result = DataLinkResultEnum.NewEmployeeCannotBeSourceOfPlanning
                                            bolRet = False
                                        End If
                                    End If
                                End If
                            End If
                            If Not bolRet Then
                                ' Borrar empleado
                                Dim oDeleteEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
                                If Not Employee.roEmployee.DeleteEmployee(oEmployee.ID, oDeleteEmployeeState, CallBroadcaster) Then
                                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                                End If
                            End If
                        Else
                            Me.State.Result = DataLinkResultEnum.InvalidEmployee
                        End If
                        If bolRet Then
                            Dim oManager As New roPassportManager
                            Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)
                            If oPassport IsNot Nothing Then
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto) IsNot String.Empty Then
                                    oPassport.PhotoRequiered = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto))
                                End If

                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation) IsNot String.Empty Then
                                    oPassport.LocationRequiered = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation))
                                End If

                                oPassport.EnabledVTDesktop = EnabledVTDesktop
                                oPassport.EnabledVTPortal = EnabledVTPortal
                                oPassport.EnabledVTPortalApp = EnabledVTPortalApp
                                oPassport.EnabledVTVisits = EnabledVTVisits
                                oManager.Save(oPassport)
                                AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET LoginWithoutContract=" & IIf(LoginWithoutContract, 1, 0) & "  WHERE ID = " & oPassport.ID)

                                ' Ejecutamos la regla de bot del tipo copiar permisos y funcionalidades en caso que este activa
                                Dim oLicense As New roServerLicense
                                Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")
                                If bolIsInstalledBots Then
                                    Try
                                        'Regla copiar permisos de empleado y campos de la ficha
                                        Dim oBotState As New roBotState(-1)
                                        Dim oBotManager As New roBotManager(oBotState)
                                        Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String)
                                        _oParameters.Add(BotRuleParameterEnum.DestinationEmployee, oEmployee.ID.ToString)
                                        oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyEmployeePermissions, _oParameters)
                                        oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyEmployeeUserFields, _oParameters)
                                    Catch ex As Exception
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CopyPermissions::Creating Employee " & strName & ": Error executing bot", ex)
                                    End Try
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            If Not bolRet Then
                strMsg = "Error Creating Employee " & strName & ": " & Me.State.Result.ToString & " - " & Me.State.ErrorText
                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        If oEmployeeState.Result <> EmployeeResultEnum.NoError Then strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        If oGroupState.Result <> GroupResultEnum.NoError Then strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidContract
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oContractState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidCard
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidLogin
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidMovility
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidUserGroup
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.FieldDataIncorrect
                        strMsg &= "(" & invalidDateField & ")"
                    Case DataLinkResultEnum.Exception
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.ErrorText & ")"
                End Select
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If

            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError

            If bolRet AndAlso notSavedUserFields <> String.Empty Then
                Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved
                Me.State.Language.ClearUserTokens()
                Me.State.Language.AddUserToken(strName)
                Me.State.Language.AddUserToken(If(notSavedUserFields.Trim.EndsWith(","), notSavedUserFields.Trim.Substring(0, notSavedUserFields.Trim.Length - 1), notSavedUserFields.Trim))
                strMsg = Me.State.Language.Translate("rodatalinkimport.newemployee.someuserfieldsignored", "")
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If

            Return bolRet
        End Function

        Protected Friend Function UpdateEmployee(ByVal intIDEmployee As Integer, ByVal ColumnsVal() As String, ByVal ColumnsUsrName() As String, ByVal ColumnsPos() As Integer, ByVal tbUserFieldList As DataTable, ByRef newCompanyIDs As Generic.List(Of Integer), ByVal dtUserGroups As DataTable, ByVal dtLabAgree As DataTable, ByRef strMsg As String, ByVal CallBroadcaster As Boolean, ByVal bolFiels_OnlyUpdate As Boolean, ByVal bProtectDataOnContractUpdate As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim idColumnInitialDate As Short
            Dim invalidDatefield As String = String.Empty
            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oContractState = New Contract.roContractState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Me.State.UpdateStateInfo()
            Dim oUserFIeldState As New UserFields.roUserFieldState
            roBusinessState.CopyTo(Me.State, oUserFIeldState)
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim notSavedUserFields As String = String.Empty
            Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(intIDEmployee, False, Me.State) 'Support.roLiveSupport.GetFreezeDate()
            Dim lNullDateStrings As String() = Nothing
            strMsg = ""

            ' Obtener el nombre completo del empleado
            Dim strName As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Surname1) & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Surname2)
            If strName.Trim <> "" Then
                strName &= ", "
            Else
                strName = ""
            End If
            strName &= ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name)

            Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, oEmployeeState, False)
            If oEmployee IsNot Nothing Then

                oEmployee.Name = strName

                Dim UserPhoto As String = ""
                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto) IsNot String.Empty Then
                    UserPhoto = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.UserPhoto))
                End If

                If UserPhoto IsNot Nothing And UserPhoto <> "" Then
                    Try
                        Dim bytes As Byte() = Convert.FromBase64String(UserPhoto)
                        oEmployee.Image = roImagesHelper.ResizeImageIfNeeded(bytes, 200, 200)
                        CallBroadcaster = True
                    Catch ex As Exception
                        Me.State.Result = DataLinkResultEnum.InvalidPhotoData
                        Return False
                    End Try
                End If

                bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, CallBroadcaster)
                If bolRet Then
                    bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, CallBroadcaster)
                    bolRet = createNIFIfNecessary(ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                    If bolRet AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) <> String.Empty Then
                        bolRet = CreateImportPrimaryKeyIfNecessary(ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                    End If

                    Dim strUsrValue As String = ""
                    Dim strUsrName As String = ""
                    Dim intUsrCol As Integer = 0
                    Dim bSaveField As Boolean = True
                    For intCol As Integer = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length To ColumnsVal.Length - 1
                        bSaveField = True
                        strUsrValue = ColumnsVal(intCol)
                        strUsrName = ColumnsUsrName(intUsrCol)
                        If strUsrValue <> "" AndAlso strUsrName <> "" Then
                            intUsrCol += 1
                            ' Si no es un campo de la ficha del empleado que define su fecha inicial procesa el campo de la ficha
                            If Not InitialDateField_Is(strUsrName) OrElse IsNothing(strUsrName) Then
                                ' Solo generamos/actualizamos el valor en caso necesario
                                Dim bolApplyChange As Boolean = True
                                If bolFiels_OnlyUpdate Then
                                    ' Si esta indicada la opcion de solo actualizar campos existentes,
                                    ' en caso que el campo no exista en VT no se debe hacer nada
                                    Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                                    If oFields.Length = 0 Then bolApplyChange = False
                                End If
                                If bolApplyChange Then
                                    ' Busca la columna que determina la fecha inicial del campo de usuario
                                    idColumnInitialDate = InitialDateField_GetColumn(strUsrName, ColumnsUsrName)
                                    If idColumnInitialDate <> -1 Then
                                        Dim xHistoryDate As Date = Now.Date
                                        Dim id As Short = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length + idColumnInitialDate
                                        If IsDate(ColumnsVal(id)) Then xHistoryDate = ColumnsVal(id)

                                        If xHistoryDate <= xFreezeDate Then
                                            notSavedUserFields &= strUsrName & ","
                                            bSaveField = False
                                        End If
                                    End If

                                    If bSaveField Then
                                        Dim oUserField As New UserFields.roUserField(oUserFIeldState, strUsrName, Types.EmployeeField, False, False)

                                        Dim DateField As Date
                                        Dim ImportUserField As Boolean = True
                                        If oUserField.History Then
                                            DateField = Now.Date
                                            If idColumnInitialDate <> -1 Then
                                                Dim id As Short = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length + idColumnInitialDate
                                                If IsDate(ColumnsVal(id)) Then DateField = ColumnsVal(id)
                                            End If
                                        Else
                                            DateField = New Date(1900, 1, 1)
                                        End If
                                        Dim bolUsedInProcess As Boolean = False
                                        Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                                        If oFields.Length > 0 Then
                                            If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                                bolUsedInProcess = True
                                            End If
                                        End If

                                        Dim strUsrValueTmp As String = strUsrValue
                                        If oUserField.FieldType = FieldTypes.tDatePeriod Then  'Valido el formato de los campo de tipo periodo de fecha
                                            If strUsrValue.Trim.Length > 0 Then
                                                ' Formato debe ser yyyy/MM/dd*yyyy/MM/dd
                                                For Each sDate As String In strUsrValue.Split("*")
                                                    If sDate.Trim.Length > 0 AndAlso Not IsDate(sDate) Then
                                                        ImportUserField = False
                                                        notSavedUserFields &= strUsrName & ","
                                                        Exit For
                                                    Else
                                                        If IsDate(sDate) Then strUsrValueTmp = strUsrValueTmp.Replace(sDate, Format(CDate(sDate), "yyyy/MM/dd"))
                                                    End If
                                                Next
                                                strUsrValue = strUsrValueTmp
                                            End If
                                        ElseIf oUserField.FieldType = FieldTypes.tDecimal Then
                                            If strUsrValue.Trim.Length > 0 Then
                                                ' Separador decimal debe ser un .
                                                strUsrValue = strUsrValueTmp.Replace(roConversions.GetDecimalDigitFormat(), ".")
                                            End If
                                        ElseIf oUserField.FieldType = FieldTypes.tDate Then
                                            If IsDate(strUsrValueTmp) Then strUsrValue = Format(CDate(strUsrValueTmp), "yyyy/MM/dd")
                                        End If
                                        If ImportUserField = True Then
                                            Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, strUsrName, DateField, New UserFields.roUserFieldState)
                                            oEmployeeUserField.FieldValue = strUsrValue
                                            If Not oEmployeeUserField.Save(, , bolUsedInProcess) Then
                                                notSavedUserFields &= strUsrName & ","
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            intUsrCol += 1
                        End If
                    Next

                End If

                If Not bolRet AndAlso Me.State.Result = DataLinkResultEnum.NoError Then Me.State.Result = DataLinkResultEnum.InvalidEmployee

                If bolRet Then
                    Dim bolInFreezePeriod As Boolean = False
                    Dim bolInvalid_change_contract_begin_date As Boolean = False

                    'Leemos la fecha de inicio de contrato/movilidad
                    Dim xBeginDate As Date
                    If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                        xBeginDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
                    Else
                        bolRet = False
                        Me.State.Result = DataLinkResultEnum.FieldDataIncorrect
                        invalidDatefield = "Fecha Alta"
                    End If

                    Dim param1 As New AdvancedParameter.roAdvancedParameter("NullDateStrings", New AdvancedParameter.roAdvancedParameterState)
                    If Any2String(param1.Value).Trim.Length > 0 Then
                        lNullDateStrings = Any2String(param1.Value).Split("@")
                    End If

                    'Leemos la fecha de fin de contrato/movilidad
                    Dim xEndDate As Date
                    If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate)) Then
                        xEndDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))
                    Else
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) = String.Empty OrElse (Not lNullDateStrings Is Nothing AndAlso lNullDateStrings.Contains(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))) Then
                            xEndDate = New Date(2079, 1, 1)
                        Else
                            bolRet = False
                            Me.State.Result = DataLinkResultEnum.FieldDataIncorrect
                            invalidDatefield = invalidDatefield & If(invalidDatefield = String.Empty, "", ", ") & "Fecha Baja"
                        End If

                    End If

                    Dim sWorkCenter As String = "#NOT_UPDATED#"
                    If ColumnsPos(RoboticsExternAccess.EmployeeColumns.WorkCenter) > -1 Then
                        sWorkCenter = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.WorkCenter))
                    End If

                    Dim bPunchWithPhoto As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto) IsNot String.Empty Then
                        bPunchWithPhoto = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithPhoto))
                    End If

                    Dim bPunchWithGeolocation As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation) IsNot String.Empty Then
                        bPunchWithGeolocation = ColumnsVal(RoboticsExternAccess.EmployeeColumns.RequirePunchWithGeolocation)
                    End If

                    Dim EnabledVTDesktop As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop) IsNot String.Empty Then
                        EnabledVTDesktop = ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTDesktop)
                    End If

                    Dim EnabledVTPortal As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal) IsNot String.Empty Then
                        EnabledVTPortal = ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortal)
                    End If

                    Dim EnabledVTPortalApp As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp) IsNot String.Empty Then
                        EnabledVTPortalApp = ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTPortalApp)
                    End If

                    Dim EnabledVTVisits As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits) IsNot String.Empty Then
                        EnabledVTVisits = ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnabledVTVisits)
                    End If

                    Dim LoginWithoutContract As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract) IsNot String.Empty Then
                        LoginWithoutContract = ColumnsVal(RoboticsExternAccess.EmployeeColumns.LoginWithoutContract)
                    End If

                    Dim sEndContractReason As String = "#NOT_UPDATED#"
                    If ColumnsPos(RoboticsExternAccess.EmployeeColumns.EndContractReason) > -1 Then
                        sEndContractReason = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndContractReason))
                    End If

                    Dim EnableBiometricData As Boolean? = Nothing
                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData) IsNot String.Empty Then
                        EnableBiometricData = roTypes.Any2Boolean(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EnableBiometricData))
                    End If

                    Dim sPinValue As String = "#NOT_UPDATED#"
                    If ColumnsPos(RoboticsExternAccess.EmployeeColumns.Pin) > -1 Then
                        sPinValue = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Pin))
                    End If

                    If bolRet Then
                        Dim oContract As New Contract.roContract(oContractState, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract))
                        If oContract.Load() Then

                            Dim bolUpdContract As Boolean = False
                            Dim hasPunchesAfterEndDate = False
                            Dim bolAvoidLooseEmployeeDataAfterContractPeriodModified As Boolean = False

                            ' Existe el contrato. Lo actualizamos
                            With oContract

                                If bProtectDataOnContractUpdate AndAlso (xBeginDate > .BeginDate OrElse xEndDate < .EndDate) Then
                                    ' Vemos si hay fechas del actual contrato que quedan fuera de contrato con el cambio
                                    bolAvoidLooseEmployeeDataAfterContractPeriodModified = Contract.roContract.HasPunchesOutOfContratToBeModified(.IDEmployee, .BeginDate, .EndDate, xBeginDate, xEndDate, oContractState)
                                End If

                                ' En el caso que la fecha de inicio de contrato se modifique verificamos si se puede hacer o no
                                If Not bolAvoidLooseEmployeeDataAfterContractPeriodModified AndAlso .BeginDate <> xBeginDate Then
                                    Dim ChangeContractBeginDate As String = New AdvancedParameter.roAdvancedParameter("VTLive.Datalink.ChangeContractBeginDate", New AdvancedParameter.roAdvancedParameterState).Value
                                    If ChangeContractBeginDate.Length = 0 Then ChangeContractBeginDate = "True"
                                    If Not roTypes.Any2Boolean(ChangeContractBeginDate) Then
                                        ' Si no esta permitido modificar la fecha de inicio de contrato, no permito modificar el contrato
                                        bolInvalid_change_contract_begin_date = True
                                    End If
                                End If

                                If Not bolAvoidLooseEmployeeDataAfterContractPeriodModified AndAlso Not bolInvalid_change_contract_begin_date Then
                                    If .BeginDate <> xBeginDate AndAlso (xBeginDate <= xFreezeDate OrElse .BeginDate <= xFreezeDate) Then
                                        'Me modifican la fecha inicial y esta dentro de la fecha de congelacion
                                        'No permito grabar
                                        bolInFreezePeriod = True
                                    End If

                                    If Not bolInFreezePeriod Then
                                        If .EndDate <> xEndDate AndAlso (xEndDate <= xFreezeDate OrElse .EndDate < xFreezeDate) Then
                                            'Me modifican la fecha final y esta dentro de la fecha de congelacion
                                            'No permito grabar
                                            bolInFreezePeriod = True
                                        End If
                                    End If

                                    If Not bolInFreezePeriod Then
                                        bolUpdContract = (.BeginDate <> xBeginDate OrElse .EndDate <> xEndDate OrElse (sWorkCenter <> "#NOT_UPDATED#" AndAlso .Enterprise <> sWorkCenter))

                                        .BeginDate = xBeginDate
                                        .EndDate = xEndDate
                                        If sWorkCenter.Trim.Length > 0 AndAlso sWorkCenter <> "#NOT_UPDATED#" Then
                                            .Enterprise = sWorkCenter
                                        End If
                                        If sEndContractReason <> "#NOT_UPDATED#" Then
                                            .EndContractReason = sEndContractReason
                                            bolUpdContract = True
                                        End If
                                        ' Asigna el convenio
                                        If Not IsNothing(dtLabAgree) AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) <> "" Then
                                            ' Busca el convenio
                                            Dim rw As DataRow()
                                            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).IndexOf("'") >= 0 Then
                                                ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).Replace("'", "''")
                                            End If
                                            rw = dtLabAgree.Select("Name='" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) & "'")
                                            If rw.Length = 1 Then
                                                Dim lbAgr As New LabAgree.roLabAgree
                                                lbAgr.ID = rw(0)("id")
                                                lbAgr.Load(False)

                                                Dim bolUpdateLabAgree As Boolean = False

                                                If .LabAgree Is Nothing Then
                                                    bolUpdateLabAgree = True
                                                Else
                                                    If lbAgr.ID <> .LabAgree.ID Then
                                                        bolUpdateLabAgree = True
                                                    End If
                                                End If

                                                If bolUpdateLabAgree Then
                                                    If .BeginDate <= xFreezeDate Then
                                                        bolInFreezePeriod = True
                                                        bolUpdContract = False
                                                    Else
                                                        .LabAgree = lbAgr
                                                        bolUpdContract = True
                                                    End If
                                                End If
                                            Else
                                                bolRet = False
                                                Me.State.Result = DataLinkResultEnum.InvalidLabAgree
                                            End If
                                        End If
                                    End If
                                End If
                            End With

                            If bolRet Then
                                If bolInvalid_change_contract_begin_date Then
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.NotAllowedChangeContractBeginDate
                                ElseIf bolAvoidLooseEmployeeDataAfterContractPeriodModified Then
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.ContractDataProtected
                                ElseIf hasPunchesAfterEndDate Then
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.HasPunchesAfterContractEndDate
                                    Me.State.ErrorText = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.HasPunchesAfterEndContractDate", "") & " " & vbNewLine
                                    If Me.IsAutomaticProcess Then
                                        SendEmailPunchesAfterContractEndDate(oEmployee)
                                    End If
                                ElseIf Not bolInFreezePeriod Then
                                    If bolUpdContract Then
                                        bolRet = oContract.Save(, CallBroadcaster)
                                        If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidContract
                                    Else
                                        bolRet = True
                                    End If
                                Else
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.FreezeDateException
                                End If
                            End If
                        Else

                            If xBeginDate <= xFreezeDate Then
                                'Me modifican la fecha inicial y esta dentro de la fecha de congelacion
                                'No permito grabar
                                bolInFreezePeriod = True
                            End If

                            If Not bolInFreezePeriod Then
                                ' No existe el contrato.
                                Dim bolNewContract As Boolean = False

                                Dim hasPunchesAfterEndDate As Boolean = False

                                ' Miramos si hay que cerrar el contrato anterior
                                Dim tbContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(oEmployee.ID, oContractState)
                                If tbContracts IsNot Nothing Then
                                    Dim strSQL As String
                                    Dim oContracts As DataRow() = tbContracts.Select("", "BeginDate DESC")
                                    If oContracts.Length > 0 Then
                                        Dim xNullDate As New Date(2079, 1, 1)
                                        If CDate(oContracts(0).Item("BeginDate")) < xBeginDate Then

                                            If hasPunchesAfterEndDate Then
                                                bolRet = False
                                                Me.State.Result = DataLinkResultEnum.HasPunchesAfterContractEndDate
                                                Me.State.ErrorText = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.HasPunchesAfterEndContractDate", "") & " " & vbNewLine
                                                If Me.IsAutomaticProcess Then
                                                    SendEmailPunchesAfterContractEndDate(oEmployee)
                                                End If
                                            ElseIf CDate(oContracts(0).Item("EndDate")) = xNullDate Or CDate(oContracts(0).Item("EndDate")) >= xBeginDate Then
                                                ' Si actualmente esta de alta, cerramos el contrato con fecha anterior al inicio del nuevo contrato
                                                strSQL = "@UPDATE# EmployeeContracts Set EndDate=" & roTypes.Any2Time(xBeginDate.Date.AddDays(-1)).SQLSmallDateTime & " " &
                                                             "WHERE IDEmployee=" & oEmployee.ID & " AND IDContract like '" & oContracts(0).Item("IDContract") & "'"
                                                bolRet = ExecuteSql(strSQL)
                                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidContract
                                            End If
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    'Insertamos el nuevo contrato
                                    oContract = New Contract.roContract(oContractState, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract))
                                    With oContract
                                        .IDEmployee = oEmployee.ID
                                        .IDCard = Nothing
                                        .BeginDate = xBeginDate
                                        .EndDate = xEndDate

                                        ' Asigna el convenio
                                        If Not IsNothing(dtLabAgree) And ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) <> "" Then
                                            ' Busca el convenio
                                            Dim rw() As DataRow
                                            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).IndexOf("'") >= 0 Then
                                                ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio).Replace("'", "''")
                                            End If
                                            rw = dtLabAgree.Select("Name='" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Convenio) & "'")
                                            If rw.Length = 1 Then
                                                Dim lbAgr As New LabAgree.roLabAgree
                                                lbAgr.ID = rw(0)("id")
                                                lbAgr.Load(False)
                                                .LabAgree = lbAgr
                                            Else
                                                bolRet = False
                                                Me.State.Result = DataLinkResultEnum.InvalidLabAgree
                                            End If
                                        End If
                                    End With

                                    If bolRet Then
                                        bolRet = oContract.Save(, CallBroadcaster)
                                        If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidContract
                                    End If

                                End If
                            Else
                                bolRet = False
                                Me.State.Result = DataLinkResultEnum.FreezeDateException
                            End If

                        End If
                    End If

                    'Comprobamos sysroPassportsAuthenticationMethods
                    If bolRet Then
                        Dim strIDCard As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Card)
                        If Not mCustomizationCode.ToUpper = "VPA" Then
                            If strIDCard.Length > 0 Then
                                If IsNumeric(strIDCard) Then
                                    bolRet = CreateCardAuthenticationMethods(oEmployee, strIDCard)
                                    If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidCard
                                Else
                                    Me.State.Result = DataLinkResultEnum.InvalidCard
                                    bolRet = False
                                End If
                            End If
                        Else
                            ' Personalización. Las tarjetas son Mifare y llegan con código HEXA girado de byte a byte
                            If strIDCard.Length > 0 Then
                                If System.Text.RegularExpressions.Regex.IsMatch(strIDCard, "^[0-9a-fA-F]+$") Then
                                    ' 1.- Giro de dos bytes en dos bytes
                                    Dim sRotatedHex As String = String.Empty
                                    sRotatedHex = RotateHex(strIDCard)
                                    ' 2.- Paso a codificación
                                    bolRet = CreateCardAuthenticationMethods(oEmployee, Any2String(Convert.ToInt64(sRotatedHex, 16)))
                                Else
                                    Me.State.Result = DataLinkResultEnum.InvalidCard
                                    bolRet = False
                                End If
                            End If
                        End If
                    End If

                    'Comprobamos la biometria
                    If bolRet Then
                        Dim oParameters As New roParameters("OPTIONS", True)

                        If EnableBiometricData.HasValue Then
                            Dim oManager As New roPassportManager
                            Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)
                            Dim hasBiometricData = oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Length > 0

                            'Si esta desactivada globalmente no hacemos nada
                            If Not oParameters.Parameter(Parameters.DisableBiometricData) Then
                                'Si tiene datos biometricos los actualizamos, si no los creamos (puede ser que no tenga datos biometricos si creamos un usuario con EnableBiometricData a False)
                                If hasBiometricData Then
                                    bolRet = Employee.roEmployee.EnableOrDisableBiometricData(intIDEmployee, Not EnableBiometricData, oEmployeeState, False)
                                Else
                                    If EnableBiometricData Then
                                        bolRet = CreateBioAuthenticationMethods(oEmployee)
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If bolRet Then
                        If sPinValue.Trim.Length > 0 AndAlso sPinValue <> "#NOT_UPDATED#" Then
                            If IsNumeric(sPinValue) AndAlso (sPinValue = "0" OrElse (sPinValue.Length >= 4 AndAlso sPinValue.Length <= 6)) Then
                                bolRet = CreateOrUpdatePinAuthenticationMethods(oEmployee, sPinValue)
                            Else
                                bolRet = False
                                Me.State.Result = DataLinkResultEnum.InvalidPinLength
                            End If
                        End If
                    End If

                    'Comprobamos el idioma
                    If bolRet Then
                        Dim strLanguage As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Language)
                        If strLanguage.Length > 0 Then
                            bolRet = UpdateEmployeeLanguage(oEmployee, oEmployeeState, strLanguage, False)
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidLanguage
                        End If
                    End If

                    'Comprobamos el login
                    If bolRet Then
                        Dim strLogin As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Login)
                        If strLogin.Length > 0 Then
                            bolRet = CreateLoginAuthenticationMethods(oEmployee, oEmployeeState, strLogin, False)
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidLogin
                        End If
                    End If

                    If bolRet Then
                        'Actualizamos la movilidad en caso de que se utilice la fecha de cambio de grupo
                        If ColumnsPos(RoboticsExternAccess.EmployeeColumns.DateLevel10) > 0 Then
                            ' Tenemos en cuenta si la fecha de movilidad es obligatoria ...
                            If Not bolMobilityDateRequired OrElse ColumnsVal(RoboticsExternAccess.EmployeeColumns.DateLevel10).Length > 0 Then
                                bolRet = False
                                If Not bolMobilityDateRequired OrElse IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.DateLevel10)) Then
                                    Dim xMovilityDate As Date
                                    If Not bolMobilityDateRequired AndAlso Not IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.DateLevel10)) Then
                                        xMovilityDate = Me.dateImportFileCreationDate.Date
                                    Else
                                        xMovilityDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.DateLevel10))
                                    End If

                                    Dim intIDGroup As Integer

                                    If ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel).Trim.Length > 0 Then
                                        ' Solo asignamos el usuario al grupo en caso que exista dicho grupo, no lo creamos
                                        Dim oGroup = Group.roGroup.GetGroupByKey(ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel), oGroupState)
                                        If oGroup IsNot Nothing AndAlso oGroup.ID > 0 Then
                                            bolRet = True
                                            intIDGroup = oGroup.ID
                                        Else
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.InvalidGroup
                                            oGroupState.Result = GroupResultEnum.GroupNotExists
                                            oGroupState.ErrorText = oGroupState.ErrorText & "(" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel) & ")"
                                        End If
                                    Else
                                        Dim invalidLevel As String = String.Empty
                                        If ValidateDepartaments(ColumnsVal, invalidLevel, oGroupState) Then
                                            ' Creamos los departamentos en caso necesario a partir de los niveles (NIVEL0,NIVEL1, ...)   y asignamos el usuario al grupo
                                            bolRet = CreateDepartaments(ColumnsVal, ColumnsUsrName, oGroupState, intIDGroup, newCompanyIDs)
                                        Else
                                            bolRet = False
                                            Me.State.Result = DataLinkResultEnum.InvalidGroup
                                            oGroupState.Result = GroupResultEnum.GroupNotExists
                                            oGroupState.ErrorText = oGroupState.ErrorText & "(" & invalidLevel & ")"
                                        End If
                                    End If

                                    If bolRet And intIDGroup <> 0 Then
                                        ' Obtenemos el grupo actual
                                        Dim intActualIDGroup As Integer
                                        Dim xTime As New roTime(CDate(xMovilityDate).Date)

                                        intActualIDGroup = roTypes.Any2Integer(ExecuteScalar("@SELECT# top 1 IDGroup  from EmployeeGroups WHERE IDEmployee=" & oEmployee.ID & " and BeginDate<=" & xTime.SQLDateTime & " order by BeginDate desc"))
                                        If intActualIDGroup <> intIDGroup Then
                                            ' Verifico que la movilidad no está en periodo de bloqueo
                                            bolRet = False
                                            If xMovilityDate > xFreezeDate Then
                                                ' VALIDAR MOBILIDADES EXISTENTES DEL EMPLEADO JUNTO CON LA NUEVA ************ AR HOTELS
                                                Dim intInvalidRow As Integer
                                                Dim dsMobilities As New DataSet
                                                Dim tbMobilities As DataTable = Employee.roMobility.GetMobilities(oEmployee.ID, oEmployeeState)
                                                Dim oRow As DataRow = tbMobilities.NewRow
                                                oRow("IDGroup") = intIDGroup
                                                oRow("Name") = ""
                                                oRow("BeginDate") = xMovilityDate
                                                'oRow("EndDate") = ?
                                                tbMobilities.Rows.Add(oRow)
                                                dsMobilities.Tables.Add(tbMobilities)
                                                Dim bRetorna As Boolean = ValidateMobilities(oEmployee.ID, dsMobilities, intInvalidRow, oEmployeeState)
                                                If bRetorna Then
                                                    bolRet = Employee.roMobility.UpdateEmployeeGroup(oEmployee.ID, intIDGroup, xMovilityDate, oEmployeeState, CallBroadcaster)
                                                Else
                                                    'comprobar si hay alguna movilidad con la misma fecha que la recibida. En ese caso se reemplaza el grupo para AR Hotels
                                                    bRetorna = ExistDateInMovility(oEmployee.ID, intActualIDGroup, xMovilityDate)
                                                    If bRetorna Then
                                                        bolRet = ReplaceGroupInMobility(oEmployee.ID, intActualIDGroup, intIDGroup, xMovilityDate, dsMobilities)
                                                    Else
                                                        bolRet = False
                                                    End If
                                                End If

                                            End If
                                        Else
                                            ' No ha habido cambio de grupo,
                                            bolRet = True
                                        End If
                                    End If
                                    'End If
                                End If
                            End If
                        End If

                        If Not bolRet Then
                            If (Me.State.Result <> DataLinkResultEnum.InvalidGroup) Then Me.State.Result = DataLinkResultEnum.InvalidMovility
                        End If
                    End If

                    '+++++ AUTORIZACIONES DE ACCESO ++++
                    If bolRet AndAlso ColumnsVal(RoboticsExternAccess.EmployeeColumns.Authorizations) <> "" Then
                        If Not CreateAuthorizations(oEmployee.ID, ColumnsVal(RoboticsExternAccess.EmployeeColumns.Authorizations), oEmployeeState) Then
                            Me.State.Result = DataLinkResultEnum.AuthorizationError
                            bolRet = False
                        End If
                    End If

                    If bolRet Then
                        ' Si se tiene que importar el grupo de usuario lo asigna
                        ' Si es seguridad modo v2 , esto no aplica
                        Dim bolApplyUserGroup As Boolean = False
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.GrupoUsuarios) <> "" AndAlso bolApplyUserGroup Then

                        End If
                    End If

                    ' Actualizamos fichaje por foto y geolocalización, aplicaciones a las que puede acceder y permitir el login sin contracto activo
                    If bolRet Then
                        If bPunchWithPhoto.HasValue OrElse bPunchWithGeolocation.HasValue OrElse EnabledVTDesktop.HasValue OrElse EnabledVTPortal.HasValue OrElse EnabledVTPortalApp.HasValue OrElse EnabledVTVisits.HasValue OrElse LoginWithoutContract.HasValue Then
                            Dim oManager As New roPassportManager
                            Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)
                            If oPassport IsNot Nothing Then
                                Dim hasChanged As Boolean = False
                                If oPassport.PhotoRequiered <> bPunchWithPhoto Then
                                    oPassport.PhotoRequiered = bPunchWithPhoto
                                    hasChanged = True
                                End If
                                If oPassport.LocationRequiered <> bPunchWithGeolocation Then
                                    oPassport.LocationRequiered = bPunchWithGeolocation
                                    hasChanged = True
                                End If
                                If oPassport.EnabledVTDesktop <> EnabledVTDesktop Then
                                    oPassport.EnabledVTDesktop = EnabledVTDesktop
                                    hasChanged = True
                                End If
                                If oPassport.EnabledVTPortal <> EnabledVTPortal Then
                                    oPassport.EnabledVTPortal = EnabledVTPortal
                                    hasChanged = True
                                End If
                                If oPassport.EnabledVTPortalApp <> EnabledVTPortalApp Then
                                    oPassport.EnabledVTPortalApp = EnabledVTPortalApp
                                    hasChanged = True
                                End If
                                If oPassport.EnabledVTVisits <> EnabledVTVisits Then
                                    oPassport.EnabledVTVisits = EnabledVTVisits
                                    hasChanged = True
                                End If
                                If LoginWithoutContract.HasValue Then
                                    AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET LoginWithoutContract=" & IIf(LoginWithoutContract, 1, 0) & "  WHERE ID = " & oPassport.ID)
                                End If
                                If hasChanged Then
                                    oManager.Save(oPassport)
                                End If
                            End If
                        End If
                    End If
                End If
            Else
                Me.State.Result = DataLinkResultEnum.InvalidEmployee
            End If

            If Not bolRet Then
                strMsg = "Error Updating Employee '" & strName & "': " & Me.State.Result.ToString & " - " & Me.State.ErrorText

                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        If oEmployeeState.Result <> EmployeeResultEnum.NoError Then strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidContract
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oContractState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidCard
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidLogin
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidMovility
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidUserGroup
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidLanguage
                        If oContractState.Result <> ContractsResultEnum.NoError Then strMsg &= " (" & oContractState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.Exception
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.ErrorText & ")"
                    Case DataLinkResultEnum.FieldDataIncorrect
                        strMsg &= "(" & invalidDatefield & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        If oGroupState.Result <> GroupResultEnum.NoError Then strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                End Select
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If
            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError
            If bolRet AndAlso notSavedUserFields <> String.Empty Then
                Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved
                Me.State.Language.ClearUserTokens()
                Me.State.Language.AddUserToken(strName)
                Me.State.Language.AddUserToken(If(notSavedUserFields.Trim.EndsWith(","), notSavedUserFields.Trim.Substring(0, notSavedUserFields.Trim.Length - 1), notSavedUserFields.Trim))
                strMsg = Me.State.Language.Translate("rodatalinkimport.updatedemployee.someuserfieldsignored", "")
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If

            Return bolRet

        End Function

        Private Function GetDataExcelGroups(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel).Length > 0 And ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name).Length > 0 Then
                    bolRet = True
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelGroups")
            End Try

            Return bolRet

        End Function

        Private Function NewGroup(ByVal ColumnsVal() As String, ByRef newCompanyIDs As Generic.List(Of Integer), ByRef strMsg As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oGroup As New Group.roGroup
            Dim oParentGroup As Group.roGroup = Nothing
            Dim bolNewCompany As Boolean = False

            Me.State.UpdateStateInfo()
            strMsg = ""

            ' Nombre del grupo
            Dim strName As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name)

            bolRet = True
            ' Obtenemos el id del padre en caso necesario
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDParentLevel).Trim.Length > 0 Then
                oParentGroup = Group.roGroup.GetGroupByKey(ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDParentLevel), New Group.roGroupState(-1))
                If oParentGroup Is Nothing OrElse oParentGroup.ID <= 0 Then
                    ' Si el grupo padre no existe , mostramos error y no seguimos
                    Me.State.Result = DataLinkResultEnum.InvalidParentGroup
                    bolRet = False
                Else
                    If oParentGroup.ID = oGroup.ID Then
                        ' no podemos asignarlo a si mismo
                        Me.State.Result = DataLinkResultEnum.InvalidParentGroup
                        bolRet = False
                    End If
                End If
            End If

            If bolRet Then
                ' Asigamos las propiedades al nuevo grupo
                oGroup.Name = strName
                oGroup.Export = ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel).Trim
                oGroup.DescriptionGroup = ""
                If oParentGroup IsNot Nothing Then
                    oGroup.Path = oParentGroup.Path
                Else
                    bolNewCompany = True
                End If

                ' Guardamos el nuevo grupo
                bolRet = oGroup.Save()
                If Not bolRet Then
                    Me.State.Result = DataLinkResultEnum.InvalidGroup
                Else
                    If bolNewCompany Then
                        If Not newCompanyIDs Is Nothing Then newCompanyIDs.Add(oGroup.ID)
                    End If
                End If
            End If

            If Not bolRet Then
                strMsg = "Error Creating Group " & strName & "(" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDLevel) & ")" & ": (" & Me.State.Result.ToString & " - " & Me.State.ErrorText & ")"
                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidGroup
                        If oGroupState.Result <> GroupResultEnum.NoError Then strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                    Case DataLinkResultEnum.Exception
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.ErrorText & ")"
                End Select

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If

            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError

            Return bolRet
        End Function

        Private Function UpdateGroup(ByVal oGroup As Group.roGroup, ByVal ColumnsVal() As String, ByRef newCompanyIDs As Generic.List(Of Integer), ByRef strMsg As String) As Boolean

            Dim bolRet As Boolean = False
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oParentGroup As Group.roGroup = Nothing
            Dim bolNewCompany As Boolean = False
            Dim sSQL As String = ""
            Dim bolModified As Boolean = False

            Me.State.UpdateStateInfo()
            strMsg = ""

            ' Obtener el nombre del grupo
            Dim strName As String = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name)

            bolRet = True
            ' Obtenemos el id del padre en caso necesario
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDParentLevel).Trim.Length > 0 Then
                oParentGroup = Group.roGroup.GetGroupByKey(ColumnsVal(RoboticsExternAccess.EmployeeColumns.IDParentLevel), oGroupState)
                If oParentGroup Is Nothing OrElse oParentGroup.ID <= 0 Then
                    ' Si el grupo padre no existe , mostramos error y no seguimos
                    Me.State.Result = DataLinkResultEnum.InvalidParentGroup
                    bolRet = False
                Else
                    If oParentGroup.ID = oGroup.ID Then
                        ' no podemos asignarlo a si mismo
                        Me.State.Result = DataLinkResultEnum.InvalidParentGroup
                        bolRet = False
                    End If

                    If oParentGroup.FullGroupName.StartsWith(oGroup.FullGroupName) Then
                        ' No podemos asignarlo a un hijo suyo
                        Me.State.Result = DataLinkResultEnum.InvalidParentGroup
                        bolRet = False
                    End If
                End If
            End If

            If bolRet Then
                ' Actualizamos las propiedades del grupo, en caso necesario
                Dim strPath As String = ""

                ' Si ha cambiado el nombre
                If oGroup.Name <> strName Then
                    bolModified = True
                    oGroup.Name = strName
                End If

                ' Si ha cambiado el padre
                If oParentGroup IsNot Nothing Then
                    strPath = oParentGroup.Path & "\" & oGroup.ID
                Else
                    strPath = oGroup.ID
                End If

                If strPath <> oGroup.Path Then
                    If bolRet Then
                        ' En el caso que el padre haya cambiado, debemos cambiar tambien a todos sus departamentos hijos el path y el fullgroupName
                        sSQL = "@SELECT# ID, Path FROM Groups WHERE Path Like '" & oGroup.Path & "\%'"
                        Dim tb As DataTable = CreateDataTable(sSQL)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            For Each orow As DataRow In tb.Rows
                                Dim strChildPath As String = Any2String(orow("Path"))
                                strChildPath = strChildPath.Replace(oGroup.Path & "\", strPath & "\")

                                ' Actualizamos el path del grupo hijo
                                bolRet = ExecuteSql("@UPDATE# Groups set Path='" & strChildPath & "' WHERE id=" & orow("ID").ToString)

                                'Actualizamos el FullPathName del grupo hijo
                                If bolRet Then bolRet = ExecuteSql("@UPDATE# groups set FullGroupName= dbo.GetFullGroupPathName(id) where id=" & orow("ID").ToString)

                                If Not bolRet Then
                                    Me.State.Result = DataLinkResultEnum.InvalidGroup
                                    Exit For
                                End If
                            Next
                        End If

                        ' Actualizamos el nuevo path del grupo
                        oGroup.Path = strPath
                        If Not oGroup.Path.Contains("\") Then bolNewCompany = True

                        bolModified = True
                    End If
                End If

                ''' NOTA: NO SE HACE NINGUN TRATAMIENTO DE LOS CENTROS DE COSTE PORQUE POR UN LADO NO SE ENVIA ESA INFO Y POR OTRO DEBERIAMOS SABE DESDE
                ''' QUE FECHA SE QUIERE APLICAR ESE NUEVO CENTRO PARA RECALCULAR LOS DIAS A PARTIR DE ESA FECHA, YA QUE SINO DEBERIAMOS RECALCULAR TODO DESDE LA FECHA DE CNGELACION
                ''' Y ES POSIBLE QUE LA FECHA DE CONGELACION NO ESTE BIEN PUESTA PORQUE EN LAS IMPORTACIONES AUTOMATICAS NO TE DARIA TIEMPO A CAMBIARLA ADEMAS EN CADA CASA LA FECHA PODRIA SER DIFERENTE
                ''' Y PARA UN CENTRO SERIA UNA FECHA Y PARA OTRO UNA DIFERENTE

                ' Guardamos los datos del grupo, en cas que se haya modificado algun dato
                If bolRet And bolModified Then bolRet = oGroup.Save()
                If Not bolRet Then
                    Me.State.Result = DataLinkResultEnum.InvalidGroup
                Else
                    If bolNewCompany Then
                        If Not newCompanyIDs Is Nothing Then newCompanyIDs.Add(oGroup.ID)
                    End If
                End If
            End If

            If Not bolRet Then
                strMsg = "Error Updating Group '" & strName & "': " & Me.State.Result.ToString & " - " & Me.State.ErrorText

                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidParentGroup
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.Result.ToString & " - " & Me.State.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.Result.ToString & " - " & Me.State.ErrorText & ")"
                    Case DataLinkResultEnum.Exception
                        If Me.State.Result <> DataLinkResultEnum.NoError Then strMsg &= " (" & Me.State.ErrorText & ")"

                End Select

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
            End If

            If bolRet Then Me.State.Result = DataLinkResultEnum.NoError

            Return bolRet

        End Function

        Private Function SendEmailPunchesAfterContractEndDate(ByRef oEmployee As Employee.roEmployee) As Boolean

            Dim oSendMail As SendMail = Nothing
            Dim oSmtpServer As System.Net.Mail.SmtpClient = Nothing
            Dim strRet As String = String.Empty
            Dim res As Boolean = False
            Dim advParam As AdvancedParameter.roAdvancedParameter = New AdvancedParameter.roAdvancedParameter("Anca.SendEmailPunchesAfterContractEndDate", Nothing)
            Try
                Dim oNotificationItem As New roNotificationItem With {
                                .Type = NotificationItemType.email,
                                                    .Content = String.Empty,
                                                    .Body = Me.State.ErrorText,
                                                    .Destination = IIf(advParam IsNot Nothing, advParam.Value, ""),
                                                    .Subject = String.Format("Incidencia en la importación automática del usuario {0}", oEmployee.Name)
                            }
                Dim bSend As Boolean = Azure.RoAzureSupport.SendTaskToQueue(-1, Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.SendEmail, VTBase.roJSONHelper.SerializeNewtonSoft(oNotificationItem))

                If bSend Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & "-1" & "::SentMessageToQueue")
                    strRet = "OK"
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & "-1" & "::ErrorSendingMessageToQueue")
                    strRet = "KO"
                End If

                res = strRet = "OK"
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CImportData::SendEmailPunchesAfterContractEndDate :", ex)
                res = False
            Finally
                If oSmtpServer IsNot Nothing Then
                    oSmtpServer.Dispose()
                End If
            End Try

            Return res

        End Function

        Protected Friend Function GetUserGroups() As DataTable
            Dim tb As New DataTable

            Try
                Dim strSQL As String = "@SELECT# id, IDParentPassport, Name, '' as xName FROM sysroPassports where GroupType='U' order by name"

                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim DT As New DataTable

                    ' Selecciona todos los registros de sysropassports definidos como grupos
                    For Each rw As DataRow In tb.Rows
                        If Not IsDBNull(rw("idParentPassport")) Then
                            rw("xName") = "\" & GetParentUserGroup(rw("idParentPassport")) & "\" & rw("Name")
                        Else
                            rw("xName") = "\" & rw("Name")
                        End If
                    Next
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetUserGroups")

            End Try

            Return tb

        End Function

        Private Function GetParentUserGroup(ByVal id As Short) As String
            Dim xPath As String = String.Empty

            Try

                Dim strSQL As String = "@SELECT# id, idParentPassport, Name FROM sysroPassports where id=" & id
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("idParentPassport")) Then
                        xPath = GetParentUserGroup(tb.Rows(0)("idParentPassport")) & "\" & tb.Rows(0)("Name")
                    Else
                        xPath = tb.Rows(0)("Name")
                    End If
                Else
                    xPath = ""
                End If

                tb.Dispose()
            Catch
            End Try

            Return xPath

        End Function

        Protected Friend Function GetLabAgrees() As DataTable
            Dim tb As New DataTable

            Try

                Dim strSQL As String = "@SELECT# id, Name FROM LabAgree"
                tb = CreateDataTable(strSQL)
            Catch
            End Try

            Return tb

        End Function

        Private Function CreatePassportIfNeeded(ByRef oEmployee As Employee.roEmployee) As Boolean
            Dim bolRet As Boolean = True

            'Recuperem el passport posem la tarja a sysroPassports_AuthenticationMethods
            Dim oStateSecurity As New roSecurityState

            Try
                Dim oManager As New roPassportManager(oStateSecurity)
                Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                If oPassport Is Nothing Then
                    oPassport = New roPassport()
                    oPassport.IDEmployee = oEmployee.ID
                    oPassport.GroupType = "E"
                    Dim oSettings As New roSettings()
                    Dim langManager As roLanguageManager = New roLanguageManager()
                    Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                    oPassport.Language = defaultLang
                    oPassport.State = 1
                    oPassport.Name = oEmployee.Name

                    bolRet = oManager.Save(oPassport)

                    If Not bolRet AndAlso oManager.State.Result <> SecurityResultEnum.NoError Then
                        Me.State.Result = DataLinkResultEnum.Exception
                        Me.State.ErrorText = oManager.State.ErrorText
                        Me.State.ErrorDetail = oManager.State.ErrorDetail
                    End If
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreatePassportIfNeeded")
            End Try

            Return bolRet

        End Function

        Private Function CreateBioAuthenticationMethods(ByRef oEmployee As Employee.roEmployee) As Boolean
            Dim bolRet As Boolean = True

            Dim oStateSecurity As New roSecurityState

            Try
                Dim oManager As New roPassportManager(oStateSecurity)
                Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                If oPassport Is Nothing Then
                    oPassport = New roPassport()
                    oPassport.IDEmployee = oEmployee.ID
                    oPassport.GroupType = "E"

                    Dim oSettings As New roSettings()
                    Dim langManager As roLanguageManager = New roLanguageManager()
                    Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                    oPassport.Language = defaultLang

                    oPassport.State = 1
                End If

                oPassport.Name = oEmployee.Name

                'Dim oBioRow As Robotics.Security.DataSets.PassportAuthenticationMethodsDataSet.sysroPassports_AuthenticationMethodsRow =
                '        oPassport.AuthenticationMethods().BiometricRow("RXA200", 0)

                Dim oBioRow As roPassportAuthenticationMethodsRow = Nothing

                If oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Length > 0 Then
                    oBioRow = oPassport.AuthenticationMethods.BiometricRows.FirstOrDefault(Function(x) x.Version = "RXA200")
                End If

                If oBioRow Is Nothing Then
                    oBioRow = New roPassportAuthenticationMethodsRow() With {
                            .IDPassport = oPassport.ID,
                            .Method = AuthenticationMethod.Biometry,
                            .Version = "RXA200",
                            .Credential = "",
                            .Password = "",
                            .BiometricID = 0,
                            .BiometricData = {},
                            .TimeStamp = Now,
                            .Enabled = True,
                            .RowState = RowState.UpdateRow
                            }
                    If (oPassport.AuthenticationMethods Is Nothing) Then
                        oPassport.AuthenticationMethods = New roPassportAuthenticationMethods()
                    End If
                    If (oPassport.AuthenticationMethods.BiometricRows Is Nothing) Then
                        oPassport.AuthenticationMethods.BiometricRows = {}
                    End If

                    Dim tmpList As Generic.List(Of roPassportAuthenticationMethodsRow) = oPassport.AuthenticationMethods.BiometricRows.ToList()
                    tmpList.Add(oBioRow)

                    oPassport.AuthenticationMethods.BiometricRows = tmpList.ToArray()
                Else
                    oBioRow.Enabled = True
                    oBioRow.RowState = RowState.UpdateRow
                End If

                oManager.Save(oPassport)

                If oManager.State.Result <> SecurityResultEnum.NoError Then
                    Me.State.Result = DataLinkResultEnum.Exception
                    Me.State.ErrorText = oManager.State.ErrorText
                    Me.State.ErrorDetail = oManager.State.ErrorDetail
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateBioAuthenticationMethods")
            End Try

            Return bolRet

        End Function

        Private Function CreateCardAuthenticationMethods(ByRef oEmployee As Employee.roEmployee, ByVal strIDcard As String) As Boolean
            Dim bolRet As Boolean = True

            'Recuperem el passport posem la tarja a sysroPassports_AuthenticationMethods
            Dim oStateSecurity As New roSecurityState

            Try
                Dim oManager As New roPassportManager(oStateSecurity)
                Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                If oPassport Is Nothing Then
                    oPassport = New roPassport()
                    oPassport.IDEmployee = oEmployee.ID
                    oPassport.GroupType = "E"
                    Dim oSettings As New roSettings()
                    Dim langManager As roLanguageManager = New roLanguageManager()
                    Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                    oPassport.Language = defaultLang
                    oPassport.State = 1
                End If

                oPassport.Name = oEmployee.Name

                ' Verifico que la tarjeta no está ya en uso, y que el empleado tiene permiso para fichar por tarjeta
                Dim oActualPassportWithThisCard As roPassport = Nothing
                oActualPassportWithThisCard = roPassportManager.GetPassportByCredential(strIDcard, AuthenticationMethod.Card, "")
                If oActualPassportWithThisCard IsNot Nothing Then
                    If oEmployee.ID <> oActualPassportWithThisCard.IDEmployee Then bolRet = False
                End If

                If bolRet Then

                    If oPassport.AuthenticationMethods.CardRows() IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Any() Then
                        If strIDcard <> oPassport.AuthenticationMethods.CardRows(0).Credential OrElse Not oPassport.AuthenticationMethods.CardRows(0).Enabled Then
                            If strIDcard = 0 Then
                                If oPassport.AuthenticationMethods.CardRows(0) IsNot Nothing Then
                                    oPassport.AuthenticationMethods.CardRows(0).Credential = ""
                                    oPassport.AuthenticationMethods.CardRows(0).Enabled = False
                                    oPassport.AuthenticationMethods.CardRows(0).RowState = RowState.UpdateRow
                                End If
                            Else
                                oPassport.AuthenticationMethods.CardRows(0).Credential = strIDcard
                                oPassport.AuthenticationMethods.CardRows(0).Enabled = True
                                oPassport.AuthenticationMethods.CardRows(0).RowState = RowState.UpdateRow
                            End If
                        End If
                    Else

                        If strIDcard <> String.Empty Then
                            Dim oCardRow As New roPassportAuthenticationMethodsRow() With {
                                    .IDPassport = oPassport.ID,
                                    .Method = AuthenticationMethod.Card,
                                    .Version = "",
                                    .Credential = strIDcard,
                                    .Password = "",
                                    .BiometricID = 0,
                                    .TimeStamp = Now,
                                    .Enabled = True,
                                    .RowState = RowState.UpdateRow
                                    }

                            If (oPassport.AuthenticationMethods Is Nothing) Then
                                oPassport.AuthenticationMethods = New roPassportAuthenticationMethods()
                            End If
                            If (oPassport.AuthenticationMethods.CardRows Is Nothing) Then
                                oPassport.AuthenticationMethods.CardRows = {}
                            End If

                            Dim tmpList As Generic.List(Of roPassportAuthenticationMethodsRow) = oPassport.AuthenticationMethods.CardRows.ToList()
                            tmpList.Add(oCardRow)

                            oPassport.AuthenticationMethods.CardRows = tmpList.ToArray()
                        End If


                    End If

                    oManager.Save(oPassport)

                    If oManager.State.Result <> SecurityResultEnum.NoError Then
                        Me.State.Result = DataLinkResultEnum.Exception
                        Me.State.ErrorText = oManager.State.ErrorText
                        Me.State.ErrorDetail = oManager.State.ErrorDetail
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateCardAuthenticationMethods")
            End Try

            Return bolRet

        End Function

        Private Function CreateLoginAuthenticationMethods(ByRef oEmployee As Employee.roEmployee, ByRef oEmployeeState As Employee.roEmployeeState, ByVal strLogin As String, ByVal bIsNewEmployee As Boolean) As Boolean
            Dim bolRet As Boolean = True

            'Recuperem el passport posem la tarja a sysroPassports_AuthenticationMethods
            Dim oStateSecurity As New roSecurityState

            Try

                If ValidateUserName(strLogin, IIf(bIsNewEmployee, -1, oEmployee.ID)) Then
                    Dim oManager As New roPassportManager(oStateSecurity)
                    Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                    If oPassport Is Nothing Then
                        oPassport = New roPassport()
                        oPassport.IDEmployee = oEmployee.ID
                        oPassport.GroupType = "E"
                        Dim oSettings As New roSettings()

                        Dim langManager As roLanguageManager = New roLanguageManager()
                        Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                        oPassport.Language = defaultLang

                        oPassport.State = 1
                    End If

                    oPassport.Name = oEmployee.Name


                    If oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow IsNot Nothing Then
                        oPassport.AuthenticationMethods.PasswordRow.Credential = strLogin
                        If strLogin.IndexOf("\") >= 0 Then
                            Dim sPass As String = strLogin.ToLower
                            oPassport.AuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(sPass)
                        End If
                        oPassport.AuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow
                    Else
                        Dim r As New Random()
                        Dim strCode As String = ""
                        For i As Integer = 1 To 10
                            strCode &= CStr(r.Next(0, 9))
                        Next


                        Dim oPasswordRow As roPassportAuthenticationMethodsRow = New roPassportAuthenticationMethodsRow() With {
                                .IDPassport = oPassport.ID,
                                .Method = AuthenticationMethod.Password,
                                .Version = String.Empty,
                                .Credential = strLogin,
                                .InvalidAccessAttemps = 0,
                                .BiometricID = 0,
                                .TimeStamp = Now,
                                .Enabled = True,
                                .RowState = RowState.UpdateRow
                                }

                        If strLogin.IndexOf("\") >= 0 Then
                            oPasswordRow.Password = CryptographyHelper.EncryptWithMD5(strLogin.ToLower())
                        Else
                            oPasswordRow.Password = CryptographyHelper.EncryptWithMD5(strCode)
                            oPasswordRow.LastUpdatePassword = New Date(1900, 1, 1)
                        End If

                        oPassport.AuthenticationMethods.PasswordRow = oPasswordRow

                        ' Envía el email
                        'Si el servicio esta activo enviamos el mail
                        Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))
                        If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1 Then
                            ' Envía el email
                            Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES (1903, " & oPassport.IDEmployee & ", " & oPassport.ID & ",'" & CryptographyHelper.Encrypt(strCode) & "')")
                            Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES (1905, " & oPassport.IDEmployee & ", " & oPassport.ID & ",'" & strLogin.Replace("\", "") & "')")
                        End If
                    End If

                    If oPassport.IDEmployee.HasValue Then 'Integracion con Visitas
                        Dim sPass As String = VTBase.CryptographyHelper.EncryptWithMD5(strLogin.ToLower)

                        oEmployee.WebLogin = strLogin
                        oEmployee.WebPassword = sPass
                        oEmployee.ActiveDirectory = (strLogin.IndexOf("\") >= 0)

                        bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False)

                    End If
                    '============================================

                    oManager.Save(oPassport)

                    If oManager.State.Result <> SecurityResultEnum.NoError Then
                        Me.State.Result = DataLinkResultEnum.Exception
                        Me.State.ErrorText = oManager.State.ErrorText
                        Me.State.ErrorDetail = oManager.State.ErrorDetail
                    End If
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateLoginAuthenticationMethods")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Private Function CreateOrUpdatePinAuthenticationMethods(ByRef oEmployee As Employee.roEmployee, ByVal sPin As String) As Boolean
            Dim bolRet As Boolean = True

            Dim oStateSecurity As New roSecurityState

            Try
                Dim oManager As New roPassportManager(oStateSecurity)
                Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                If oPassport Is Nothing Then
                    oPassport = New roPassport()
                    oPassport.IDEmployee = oEmployee.ID
                    oPassport.GroupType = "E"

                    Dim oSettings As New roSettings()
                    Dim langManager As roLanguageManager = New roLanguageManager()
                    Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                    oPassport.Language = defaultLang

                    oPassport.State = 1
                End If

                oPassport.Name = oEmployee.Name

                Dim oMethod As roPassportAuthenticationMethodsRow = oPassport.AuthenticationMethods.PinRow

                If oMethod IsNot Nothing Then
                    'Actualizamos solo si estamos cambiando el pin del que ya tiene
                    If sPin <> oMethod.Password Then
                        If sPin = 0 Then
                            oMethod.Password = ""
                            oMethod.Enabled = False
                            oMethod.TimeStamp = Now
                            oMethod.RowState = RowState.UpdateRow
                        Else
                            oMethod.Password = sPin
                            oMethod.Enabled = True
                            oMethod.TimeStamp = Now
                            oMethod.RowState = RowState.UpdateRow
                        End If

                    End If
                Else
                    If sPin <> 0 Then
                        oMethod = New roPassportAuthenticationMethodsRow

                        oMethod.IDPassport = oPassport.ID
                        oMethod.Method = AuthenticationMethod.Pin
                        oMethod.Version = ""
                        oMethod.Credential = String.Empty
                        oMethod.Password = sPin
                        oMethod.BiometricID = 0
                        oMethod.Enabled = True

                        oMethod.TimeStamp = Now
                        oMethod.RowState = RowState.NewRow
                        oPassport.AuthenticationMethods.PinRow = oMethod
                    End If

                End If

                oManager.Save(oPassport)

                If oManager.State.Result <> SecurityResultEnum.NoError Then
                    Me.State.Result = DataLinkResultEnum.Exception
                    Me.State.ErrorText = oManager.State.ErrorText
                    Me.State.ErrorDetail = oManager.State.ErrorDetail
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdatePinAuthenticationMethods")
                bolRet = False
            End Try
            Return bolRet
        End Function

        Private Function UpdateEmployeeLanguage(ByRef oEmployee As Employee.roEmployee, ByRef oEmployeeState As Employee.roEmployeeState, ByVal strLanguage As String, ByVal bIsNewEmployee As Boolean) As Boolean
            Dim bolRet As Boolean = True

            Dim oStateSecurity As New roSecurityState

            Try

                If ValidateLanguage(strLanguage) Then

                    Dim langManager As roLanguageManager = New roLanguageManager()
                    Dim oManager As New roPassportManager(oStateSecurity)
                    Dim oPassport As roPassport = oManager.LoadPassport(oEmployee.ID, LoadType.Employee)

                    If oPassport Is Nothing Then
                        oPassport = New roPassport()
                        oPassport.IDEmployee = oEmployee.ID
                        oPassport.GroupType = "E"

                        Dim oSettings As New roSettings()
                        Dim defaultLang As roPassportLanguage = langManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                        oPassport.Language = defaultLang

                        oPassport.State = 1
                    End If

                    oPassport.Name = oEmployee.Name

                    Dim language As roPassportLanguage = langManager.LoadByKey(strLanguage)

                    If language IsNot Nothing Then
                        oPassport.Language = language
                    End If


                    oManager.Save(oPassport)

                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::UpdateEmployeeLanguage")
            End Try

            Return bolRet
        End Function

        Private Function ValidateUserName(ByVal strLogin As String, ByVal IDEmployee As Integer) As Boolean
            Dim bolRet As Boolean = True

            Try
                If IDEmployee = -1 Then
                    Dim strSQL As String = "@SELECT# count(*) FROM sysroPassports_AuthenticationMethods WHERE Credential = '" & strLogin.Trim & "' and Method = 1"

                    Dim oLoginCount As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))
                    bolRet = (oLoginCount = 0)
                Else
                    Dim strSQL As String = "@SELECT# ISNULL(IDEmployee,0) AS IDEmployee FROM sysropassports srp inner join sysroPassports_AuthenticationMethods srpa on srpa.IDPassport = srp.ID WHERE srpa.Credential = '" & strLogin.Trim & "' and srpa.Method = 1"

                    Dim dtUsers As DataTable = CreateDataTable(strSQL)
                    For Each oRow As DataRow In dtUsers.Rows
                        If roTypes.Any2Integer(oRow("IDEmployee")) <> IDEmployee Then
                            bolRet = False
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::validateUserName")
            End Try

            Return bolRet
        End Function

        Private Function ValidateLanguage(ByVal strLanguage As String) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String = "@SELECT# count(*) FROM sysroLanguages WHERE LanguageKey = '" & strLanguage.Trim & "'"

                Dim oLanguageCount As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))
                bolRet = (oLanguageCount = 1)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::validateLanguage")
            End Try

            Return bolRet
        End Function

        Private Function ValidateDepartaments(ByVal ColumnsVal() As String, ByRef invalidLevel As String, ByRef oGroupState As Group.roGroupState) As Boolean
            Dim level As Integer = 1
            For i As Integer = RoboticsExternAccess.EmployeeColumns.Level2 To RoboticsExternAccess.EmployeeColumns.Level10
                If Not String.IsNullOrEmpty(ColumnsVal(i)) AndAlso String.IsNullOrEmpty(ColumnsVal(i - 1)) Then
                    invalidLevel = "Level" & level
                    Return False
                End If
                level = level + 1
            Next
            Return True
        End Function



        Private Function CreateDepartaments(ByVal ColumnsVal() As String, ByVal ColumnsUsrName() As String, ByRef oGroupState As Group.roGroupState, ByRef intIDGroup As Integer, ByRef newCompanyIDs As Generic.List(Of Integer)) As Boolean
            ' Nivel 0
            Dim intIDCompanyLvl0 As Integer

            ' Nivel 1
            Dim intIDEmpresa As Integer
            ' Nivel 2
            Dim intIDCentro As Integer

            Dim intIDNivel3 As Integer
            Dim intIDNivel4 As Integer
            Dim intIDNivel5 As Integer
            Dim intIDNivel6 As Integer
            Dim intIDNivel7 As Integer
            Dim intIDNivel8 As Integer
            Dim intIDNivel9 As Integer

            Dim bolRet As Boolean = True

            ' Si existe la columna de nivel 0 creamos la compañia base
            ' Si esta no existe partimos de la compañia base 1
            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level0).Length > 0 Then
                Dim gState As New Group.roGroupState
                Dim oGroup As Group.roGroup = Group.roGroup.GetCompanyByName(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level0), gState)
                If oGroup Is Nothing Then
                    oGroup = New Group.roGroup(oGroupState)
                    oGroup.ID = -1
                    oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level0)
                    oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                    If oGroup.Save() Then
                        Dim oUserFIeldState As New UserFields.roUserFieldState

                        Dim oUserField As New UserFields.roUserField(oUserFIeldState.IDPassport) ', "NIF", UserFields.Types.EmployeeField, oTrans, False)
                        oUserField.Type = Types.GroupField
                        oUserField.FieldName = "CIF"

                        If Not oUserField.Load(False, False) Then
                            oUserField.FieldName = "CIF"
                            oUserField.Type = Types.GroupField
                            oUserField.FieldType = FieldTypes.tText
                            oUserField.Used = True
                            oUserField.History = False

                            If Not oUserField.Save(False) Then
                                bolRet = False
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                            End If
                        Else
                            bolRet = True
                            oUserField.Used = True
                            If Not oUserField.Save(False) Then
                                bolRet = False
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                            End If

                        End If

                        If bolRet Then
                            bolRet = UserFields.roGroupUserField.SaveUserField(oGroup.ID, "CIF", ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level0), New UserFields.roUserFieldState)
                            intIDCompanyLvl0 = oGroup.ID
                            If Not newCompanyIDs Is Nothing Then newCompanyIDs.Add(oGroup.ID)
                            If Not bolRet Then
                                Me.State.Result = DataLinkResultEnum.InvalidGroup
                                bolRet = False
                            End If
                        End If
                    End If
                Else
                    Dim oUserFIeldState As New UserFields.roUserFieldState

                    Dim oUserField As New UserFields.roUserField(oUserFIeldState.IDPassport) ', "NIF", Types.EmployeeField, oTrans, False)
                    oUserField.Type = Types.GroupField
                    oUserField.FieldName = "CIF"

                    If Not oUserField.Load(False, False) Then
                        oUserField.FieldName = "CIF"
                        oUserField.Type = Types.GroupField
                        oUserField.FieldType = FieldTypes.tText
                        oUserField.Used = True
                        oUserField.History = False

                        If Not oUserField.Save(False) Then
                            bolRet = False
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                        End If

                        If bolRet Then
                            bolRet = UserFields.roGroupUserField.SaveUserField(oGroup.ID, "CIF", ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level0), New UserFields.roUserFieldState)
                            If Not bolRet Then
                                Me.State.Result = DataLinkResultEnum.InvalidGroup
                                bolRet = False
                            End If
                        End If
                    Else
                        bolRet = True
                        oUserField.Used = True
                        If Not oUserField.Save(False) Then
                            bolRet = False
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                        End If
                    End If
                End If

                If bolRet Then
                    intIDCompanyLvl0 = oGroup.ID
                    intIDGroup = oGroup.ID
                End If
            Else
                intIDCompanyLvl0 = 1
                intIDGroup = 1
                ' No se informó nivel 0. Esto no debería pasar si hay varias empresas (nivel 0). Si sólo hay una, no tiene porque ser la 1. La busco ahora
                Dim lCompanies As New List(Of Group.roGroup)
                lCompanies = Group.roGroup.GetCompanies(oGroupState)
                If lCompanies.Count >= 1 Then
                    intIDCompanyLvl0 = lCompanies.First.ID
                    intIDGroup = lCompanies.First.ID
                End If
            End If

            If bolRet Then
                'Creamos la estructura organizativa
                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level1).Length > 0 Then

                    Dim oGroup As Group.roGroup

                    'Comprobamos si esta creado el nivel1
                    oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level1), intIDCompanyLvl0, oGroupState, )
                    If oGroup Is Nothing Then
                        ' Creamos el nivel
                        oGroup = New Group.roGroup(oGroupState)
                        oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level1)
                        oGroup.Path = intIDCompanyLvl0
                        oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111119"
                        bolRet = oGroup.Save()
                        If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                    End If
                    intIDGroup = oGroup.ID
                    intIDEmpresa = oGroup.ID

                    If bolRet Then

                        'Creamos el Nivel2
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level2).Length > 0 Then

                            'Comprobamos si esta creado el nivel2
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level2), intIDCompanyLvl0 & "\" & intIDEmpresa, oGroupState, )
                            If oGroup Is Nothing Then
                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level2)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                            End If
                            intIDGroup = oGroup.ID

                        End If

                    End If

                    intIDCentro = intIDGroup

                    If bolRet Then

                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level3).Length > 0 Then
                            'Comprobamos si esta creado el nivel3
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level3), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro, oGroupState, )

                            If oGroup Is Nothing Then
                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level3)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel3 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level4).Length > 0 Then
                            'Comprobamos si esta creado el nivel4
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level4), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level4)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel4 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level5).Length > 0 Then
                            'Comprobamos si esta creado el nivel5
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level5), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level5)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel5 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level6).Length > 0 Then
                            'Comprobamos si esta creado el nivel6
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level6), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level6)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel6 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level7).Length > 0 Then
                            'Comprobamos si esta creado el nivel7
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level7), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level7)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel7 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level8).Length > 0 Then
                            'Comprobamos si esta creado el nivel8
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level8), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level8)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel8 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level9).Length > 0 Then
                            'Comprobamos si esta creado el nivel9
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level9), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level9)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If

                    intIDNivel9 = intIDGroup

                    If bolRet Then
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level10).Length > 0 Then
                            'Comprobamos si esta creado el nivel10
                            oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level10), intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8 & "\" & intIDNivel9, oGroupState, )

                            If oGroup Is Nothing Then

                                ' Creamos el nivel
                                oGroup = New Group.roGroup(oGroupState)
                                oGroup.Name = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Level10)
                                oGroup.Path = intIDCompanyLvl0 & "\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8 & "\" & intIDNivel9
                                oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                                bolRet = oGroup.Save()
                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                                intIDGroup = oGroup.ID
                            Else
                                intIDGroup = oGroup.ID
                            End If
                        End If
                    End If
                End If

            End If

            Return bolRet
        End Function

        Private Function ExistDateInMovility(ByVal intIDEmployee As Integer, ByVal IdGroup As Integer, ByVal oDate As Object) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim xTime As New roTime(CDate(oDate).Date)

                Dim strSQL As String = "@SELECT# IDGroup FROM EmployeeGroups WHERE IDEmployee = " & intIDEmployee & " AND IDGroup = " & IdGroup & " AND " &
                                           "BeginDate = " & xTime.SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then
                    bolRet = tb.Rows.Count
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ExistDateInMovility")

            End Try

            Return bolRet

        End Function

        Private Function ReplaceGroupInMobility(ByVal intIDEmployee As Integer, ByVal IdGroup As Integer, ByVal IdGroupNew As Integer, ByVal oDate As Object, ByRef dsMobilities As DataSet) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim xTime As New roTime(CDate(oDate).Date)

                ' Aquí debo ver que el penúltimo grupo no sea igual que el nuevo grupo, porque al hacer el UPDATE me quedarían dos movilidades al mismo grupo seguidas.
                Dim iPenultimateGroup As Integer = 0
                If dsMobilities.Tables(0).Rows.Count > 0 Then
                    Dim oMobilityRows() As DataRow = dsMobilities.Tables(0).Select("Name<>''", "BeginDate DESC")
                    If oMobilityRows.Length > 1 Then iPenultimateGroup = oMobilityRows(1)("IDGroup")
                End If
                If iPenultimateGroup <> IdGroupNew Then
                    Dim strSQL As String = "@UPDATE# EmployeeGroups SET IDGroup = " & IdGroupNew & " WHERE (IDEmployee = " & intIDEmployee & ") AND " &
                                           "(IDGroup = " & IdGroup & ") AND (BeginDate = " & xTime.SQLSmallDateTime & ")"
                    bolRet = ExecuteSql(strSQL)
                Else
                    ' No actualizo porque quedarían dos movilidades seguidas al mismo grupo

                    ' Hay que eliminar el ultimo registro existente y no grabar el nuevo grupo, ya que de esa forma
                    ' seguira estando en el grupo asignado previamente(penúltimo)
                    Dim strSQL As String = "@DELETE# FROM EmployeeGroups  WHERE (IDEmployee = " & intIDEmployee & ") AND " &
                                           "(IDGroup = " & IdGroup & ") AND (BeginDate = " & xTime.SQLSmallDateTime & ")"
                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        ' Dejamos abierta la ultima movilidad
                        Dim xEndDate = New Date(2079, 1, 1)
                        strSQL = "@UPDATE# EmployeeGroups SET EndDate=" & Any2Time(xEndDate).SQLSmallDateTime & " WHERE (IDEmployee = " & intIDEmployee & ") AND Begindate = (@SELECT# max(BeginDate) from EmployeeGroups EG WHERE eg.IDEmployee = EmployeeGroups.idemployee  )"
                        bolRet = ExecuteSql(strSQL)

                        ' Actualizo timestamp
                        Extensions.roTimeStamps.UpdateEmployeeTimestamp(intIDEmployee)
                    End If

                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ReplaceGroupInMobility")
            End Try

            Return bolRet

        End Function

        Private Function ValidateMobilities(ByVal IDEmployee As Integer, ByRef dsMobilities As DataSet, ByRef intInvalidRow As Integer,
                                                ByRef oEmployeeState As Employee.roEmployeeState) As Boolean

            intInvalidRow = -1

            Try

                Dim xBeginDate As DateTime
                Dim xLastBeginDate As New Nullable(Of DateTime)
                Dim intLastIDGroup As Integer = -1

                If dsMobilities.Tables(0).Rows.Count > 0 Then
                    Dim oMobilityRows() As DataRow = dsMobilities.Tables(0).Select("", "BeginDate ASC")
                    Dim oRow As DataRow
                    For nRow As Integer = 0 To oMobilityRows.Length - 1

                        oRow = oMobilityRows(nRow)

                        'Si la fecha de inicio no es una fecha correcta, no dejamos actualizar
                        If IsDBNull(oRow("BeginDate")) Then
                            oEmployeeState.Result = EmployeeResultEnum.MobilityBadBeginDate
                            intInvalidRow = nRow
                            Exit For
                        End If

                        xBeginDate = CDate(oRow("BeginDate"))

                        'Si la fecha actual es anterior al inicio del contrato, devolvemos el error
                        If Not EmployeeWithContract(IDEmployee, oEmployeeState, xBeginDate) Then
                            oEmployeeState.Result = EmployeeResultEnum.MobilityInvalidBeginDate
                            intInvalidRow = nRow
                            Exit For
                        End If

                        'Si no tenemos grupo no dejamos actualizar
                        If IsDBNull(oRow("IDGroup")) OrElse oRow("IDGroup") <= 0 Then
                            oEmployeeState.Result = EmployeeResultEnum.MobilityNoGroup
                            intInvalidRow = nRow
                            Exit For
                        End If

                        If Not xLastBeginDate.HasValue Then ' Si és el primer registro
                            'Primer registro, este no puede ser distinto a la primera fecha de contrato
                            Dim xBeginContract As DateTime = FirstContractDate(IDEmployee, oEmployeeState)
                            If oEmployeeState.Result = EmployeeResultEnum.NoError Then
                                If xBeginDate > xBeginContract Then
                                    oEmployeeState.Result = EmployeeResultEnum.MobilityDifferentContractDate
                                    intInvalidRow = nRow
                                    Exit For
                                End If
                            End If
                        Else
                            'Si la fecha de inicio de la fila actual es igual a la fecha de inicio de
                            'la ultima fila, devolvemos el error
                            If xBeginDate = xLastBeginDate.Value Then
                                oEmployeeState.Result = EmployeeResultEnum.MobilityDuplicateStartDate
                                intInvalidRow = nRow
                                Exit For
                            End If
                            'Si el grupo es el mismo que el grupo de la fila anterior, mostramos el error
                            If oRow("IDGroup") = intLastIDGroup Then
                                oEmployeeState.Result = EmployeeResultEnum.MobilityDuplicateGroup
                                intInvalidRow = nRow
                                Exit For
                            End If
                        End If

                        'Si no estamos en la ultima fila
                        If nRow < oMobilityRows.Length - 1 Then
                            Dim oNextRow As DataRow = oMobilityRows(nRow + 1)
                            'Comprueba que la siguiente fecha sea correcta
                            If IsDBNull(oNextRow) Then
                                oEmployeeState.Result = EmployeeResultEnum.MobilityBadBeginDate
                                intInvalidRow = nRow
                                Exit For
                            End If
                            oRow("EndDate") = CDate(oNextRow("BeginDate")).AddDays(-1).Date
                        Else
                            oRow("EndDate") = New Date(2079, 1, 1) ' DBNull.Value
                        End If

                        xLastBeginDate = xBeginDate
                        intLastIDGroup = oRow("IDGroup")

                    Next
                Else
                    oEmployeeState.Result = EmployeeResultEnum.MobilityDifferentContractDate
                End If
            Catch ex As DbException
                oEmployeeState.UpdateStateInfo(ex, "roDataLinkImport::ValidateMobilities")
            Catch Ex As Exception
                oEmployeeState.UpdateStateInfo(Ex, "roDataLinkImport::ValidateMobilities")
            End Try

            Return (oEmployeeState.Result = EmployeeResultEnum.NoError)

        End Function

        Private Function EmployeeWithContract(ByVal intIDEmployee As Integer, ByRef oState As Employee.roEmployeeState, ByVal oDate As Object) As Boolean

            Dim bolRet As Boolean = False

            Me.State.UpdateStateInfo()

            Try

                Dim xTime As New roTime(CDate(oDate).Date)

                Dim strSQL As String = "@SELECT# IDEmployee FROM EmployeeContracts WHERE IDEmployee = " & intIDEmployee & " AND " &
                                           xTime.SQLSmallDateTime & " >= BeginDate And EndDate >= " & xTime.SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    bolRet = (tb.Rows.Count > 0)
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::EmployeeWithContract")
            End Try

            Return bolRet

        End Function

        Private Function FirstContractDate(ByVal intIDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As DateTime

            Dim xRet As DateTime

            Me.State.UpdateStateInfo()

            Try

                Dim strSQL As String = "@SELECT# Min(BeginDate) FROM EmployeeContracts WHERE IDEmployee=" & intIDEmployee.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    If tb.Rows.Count > 0 Then
                        xRet = tb.Rows(0)(0)
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::FirstContractDate")
            End Try

            Return xRet

        End Function

        Private Function CreateAuthorizations(ByVal intIDEmployee As Integer, ByVal authorizations As String, ByRef oState As Employee.roEmployeeState) As Boolean
            Dim bolRet As Boolean = True

            Dim oParam As New AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", New AdvancedParameter.roAdvancedParameterState)
            Dim bolAdvMode As Boolean = roTypes.Any2Integer(oParam.Value) = 1

            If authorizations.ToUpper = "NONE" Or authorizations.ToUpper <> "" Then
                'Quitamos todas las autorizaciones al empleado
                bolRet = AccessGroup.roAccessGroup.RemoveAllEmployeeAccessGroups(intIDEmployee, New AccessGroup.roAccessGroupState(Me.State.IDPassport), bolAdvMode)
            End If

            If authorizations.ToUpper <> "" Then
                Dim oEmpAuthorizations() As String = authorizations.Split(",")
                For Each oNewShortName As String In oEmpAuthorizations
                    Dim iAccessGroupID As Integer = AccessGroup.roAccessGroup.GetAccessGroupByShortName(oNewShortName.Trim, New AccessGroup.roAccessGroupState)
                    If iAccessGroupID > 0 Then
                        bolRet = AccessGroup.roAccessGroup.SaveEmployeeAccessGroup(intIDEmployee, iAccessGroupID, New AccessGroup.roAccessGroupState(Me.State.IDPassport), bolAdvMode)
                        If Not bolRet Then
                            Me.State.Result = DataLinkResultEnum.AuthorizationError
                            Exit For
                        End If
                    End If
                Next

            End If

            Return bolRet
        End Function

        Public Function GetLastEmployeeUserField(ByVal idEmployee As Integer, ByVal EmployeeUserField As String) As Date
            Dim tb As New DataTable
            GetLastEmployeeUserField = New Date(1900, 1, 1)

            Try
                Dim strSQL As String = "@SELECT# top(1) Date FROM EmployeeUserFieldValues " &
                                           "WHERE IDEmployee = " & idEmployee.ToString & " AND " &
                                           "FieldName = '" & EmployeeUserField & "'" &
                                           "Order by Date Desc"

                tb = CreateDataTable(strSQL)
                If Not IsNothing(tb) AndAlso tb.Rows.Count > 0 Then GetLastEmployeeUserField = tb.Rows(0)("Date")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetLastEmployeeUserField")

            End Try

        End Function

        Private Function ImportEmployeesEXCEL_CheckMaxEmployeeNotExceded(ByVal intBeginLine As Integer, ByVal intEndLine As Integer, ByVal intColumns As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String,
                                                 ByVal CompositeContractType As Integer, ByVal bolClonar As Boolean, ByVal EmpresaColumnPosition As Integer,
                                                 ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer, ByRef EmployeeState As Employee.roEmployeeState, ByRef strLogEvent As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim dtEmpUserFields As DataTable = Nothing
                Dim dtEmpContracts As DataTable = Nothing
                Dim newID As Long = 0

                ' Lee empleados y contratos de la base de datos
                ImportEmployees_GetContracts(newID, dtEmpContracts, dtEmpUserFields)
                Dim intIDEmployee As Integer = 0

                ' Recorre el fichero
                For intRow As Integer = intBeginLine To intEndLine
                    Dim errorInfo As String = String.Empty
                    If Me.GetDataExcelEmployees(intRow, intColumns, ColumnsPos, ColumnsVal, CompositeContractType, EmpresaColumnPosition) Then
                        ' Determina si es un empleado nuevo
                        intIDEmployee = ImportEmployees_GetIDEmployee(dtEmpUserFields, bolClonar, iKeyPositionVal, iNifPositionVal, ColumnsVal, EmpresaColumnPosition, EmployeeState)

                        ' Simula el contrato
                        ImportEmployees_SimulateContract(newID, intIDEmployee, dtEmpUserFields, dtEmpContracts, ColumnsVal, iKeyPositionVal, iNifPositionVal)
                    End If
                Next intRow

                ' Comprueba que no se hayan sobrepasado los empleados de la licencia a día de hoy
                bolRet = ImportEmployees_CheckMaxEmployeesNotExceded(dtEmpContracts, strLogEvent)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesEXCEL_CheckMaxEmployeeNotExceded")
                bolRet = True ' Si hay cualquier error no previsto continúa con la importación

            End Try

            Return bolRet
        End Function

#End Region

#Region "20- IMPORTAR EMPLEADOS ASCII"

        Public Function ImportEmployeesAscii(ByVal strSeparator As String) As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKAscii And bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As String

                    'Definimos array con los valores de la posicion inicial de las columnas
                    Dim ColumnsInitialPos(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As Integer

                    'Definimos array con los valores de la longitud de las columnas
                    Dim ColumnsLenght(System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length - 1) As Integer

                    ' Definimos array con los nombres de los campos personalizados
                    Dim ColumnsUsrName(-1) As String

                    ' Definimos variables para guardar los logs
                    Dim intNewEmployees As Integer = 0
                    Dim intUpdateEmployees As Integer = 0
                    Dim intKOs As Integer = 0

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & vbNewLine

                    ' Comprueba el fichero excel de plantilla
                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intLines As Integer = 0
                        intLines = Me.CountLinesAscii()
                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then

                            Dim CompositeContractType As Integer = 0
                            Dim EmpresaColumnPosition As Integer = -1
                            Dim bolClonar As Boolean = False
                            Dim InvalidColumn As String = ""
                            Dim IsGroupImport As Boolean = False
                            Dim bolFiels_OnlyUpdate As Boolean = False

                            'Contar número de columnas
                            Dim intColumns As Integer = Me.ValidarColumnasEmployees((strSeparator = ""), ColumnsPos, ColumnsUsrName, CompositeContractType, EmpresaColumnPosition, bolClonar, ColumnsInitialPos, ColumnsLenght, InvalidColumn, IsGroupImport, bolFiels_OnlyUpdate)
                            If intColumns > 0 Then

                                ReDim ColumnsVal(ColumnsPos.Length - 1)

                                Try

                                    Dim oEmployeeState As New Employee.roEmployeeState
                                    roBusinessState.CopyTo(Me.State, oEmployeeState)

                                    Dim oUserFieldState As New UserFields.roUserFieldState()
                                    roBusinessState.CopyTo(Me.State, oUserFieldState)

                                    Dim tbUserFieldList As DataTable = UserFields.roUserField.GetUserFields(Types.EmployeeField, oUserFieldState)
                                    createMandatoryFields(oEmployeeState, oUserFieldState, False)
                                    createExcelUserFields(ColumnsVal, ColumnsUsrName, oEmployeeState, oUserFieldState, False)

                                    bolRet = True

                                    ' Grupos de usuario
                                    Dim dtUserGroups As DataTable = GetUserGroups()

                                    ' Convenios
                                    Dim dtLabAgrees As DataTable = GetLabAgrees()

                                    Dim strMsg As String = ""

                                    ' Determina si se sobrepasaría licencia de máximo número de empleados
                                    bolRet = ImportEmployeesASCII_CheckMaxEmployeeNotExceded(intLines, intColumns, ColumnsPos, ColumnsVal, strSeparator, CompositeContractType, EmpresaColumnPosition, ColumnsInitialPos, ColumnsLenght, bolClonar, RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey, RoboticsExternAccess.EmployeeColumns.DNI, oEmployeeState, strLogEvent)
                                    If bolRet = False Then Exit Try

                                    'Recorrer Fichero Ascii
                                    Dim strLine As String = ""
                                    Dim intIDEmployee As Integer = 0
                                    Dim errorExists As Boolean = False

                                    Dim newCompanyIds As New Generic.List(Of Integer)

                                    For intRow As Integer = 0 To intLines - 1
                                        Dim errorInfo As String = String.Empty
                                        strLine = Me.oAsciiReader.ReadLine()

                                        If Me.GetDataAsciiEmployees(strLine, intColumns, ColumnsPos, ColumnsVal, strSeparator, CompositeContractType, EmpresaColumnPosition, ColumnsInitialPos, ColumnsLenght, errorInfo) Then

                                            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                                            Try
                                                If Not bolClonar Then
                                                    intIDEmployee = isEmployeeNew(ColumnsVal, RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey, RoboticsExternAccess.EmployeeColumns.DNI, oUserFieldState)
                                                Else
                                                    ' Debemos obtener un empleado con el DNI + Empresa, para saber si existe o no
                                                    Dim strSQL As String
                                                    strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & roTypes.Any2Time(Now).SQLSmallDateTime & " " &
                                                                 "@SELECT# sysrovwAllEmployeeGroups.* " &
                                                                 "FROM sysrovwAllEmployeeGroups, GetAllEmployeeUserFieldValue('EMPRESA', @Date) V , GetAllEmployeeUserFieldValue('NIF', @Date) Z "
                                                    strSQL &= ", Employees "
                                                    strSQL &= "WHERE sysrovwAllEmployeeGroups.IDEmployee = V.idEmployee AND sysrovwAllEmployeeGroups.IDEmployee = Z.idEmployee AND " &
                                                                        "CONVERT(varchar, ISNULL(V.[Value], '')) = '" & ColumnsVal(EmpresaColumnPosition) & "' AND " &
                                                                        "CONVERT(varchar, ISNULL(Z.[Value], '')) = '" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI) & "' "
                                                    Dim tbEmployee As DataTable = CreateDataTable(strSQL)

                                                    If tbEmployee.Rows.Count <= 0 Then
                                                        intIDEmployee = -1
                                                    Else
                                                        intIDEmployee = tbEmployee.Rows(0).Item("IDEmployee")
                                                    End If
                                                End If

                                                strMsg = ""

                                                If intIDEmployee = -1 Then
                                                    ' Nuevo empleado
                                                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "") & " "
                                                    bolRet = Me.NewEmployee(ColumnsVal, ColumnsUsrName, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strMsg, False, False)
                                                    If bolRet Then intNewEmployees = intNewEmployees + 1
                                                Else
                                                    'Actualizar empleado
                                                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.UpdateOperation", "") & " "
                                                    bolRet = Me.UpdateEmployee(intIDEmployee, ColumnsVal, ColumnsUsrName, ColumnsPos, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strMsg, False, False, False)
                                                    If bolRet Then intUpdateEmployees = intUpdateEmployees + 1
                                                End If

                                                If bolRet = False Or Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved Then
                                                    errorExists = True
                                                    If Me.State.Result = DataLinkResultEnum.SomeUserFieldsNotSaved Then
                                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterImportedWithoutData", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine
                                                    Else
                                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine
                                                        intKOs += 1
                                                    End If
                                                    Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                    msgLog &= strMsg & vbNewLine
                                                End If
                                            Catch ex As DbException
                                                Me.State.Result = DataLinkResultEnum.Exception
                                                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesASCII")
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine & ex.Message & vbNewLine
                                                bolRet = False
                                            Catch ex As Exception
                                                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesASCII")
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine & ex.Message & vbNewLine
                                                bolRet = False
                                            Finally
                                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                            End Try
                                        Else
                                            'Verificamos algunos casos en los que no es necesario reportar error (líneas vacías, sólo retornos de carro, sólo tabuladores ...
                                            If strLine.Replace(vbTab, "").Replace(" ", "").Length > 0 Then
                                                Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                                msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine
                                            Else
                                                msgLog &= Me.State.Language.Translate("Import.LogEvent.RegisterWithoutInfo", "") & " " & intRow + 1 & " " & errorInfo & " " & vbNewLine
                                            End If
                                        End If
                                    Next

                                    If errorExists Then
                                        bolRet = False
                                    End If

                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & vbNewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewEmployees", "") & intNewEmployees.ToString & vbNewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateEmployees", "") & intUpdateEmployees.ToString & vbNewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.RegistersNotImported", "") & " " & intKOs & vbNewLine

                                    ' Llama al BroadCaster
                                    'If intNewEmployees > 0 And bolClonar Then
                                    If intNewEmployees > 0 Or intUpdateEmployees > 0 Then
                                        Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                                    End If

                                    ' Auditamos importación empleados
                                    Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                                    Me.State.AddAuditParameter(tbParameters, "{ImportEmployeesType}", "Ascii", "", 1)
                                    Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportEmployees, "", tbParameters, -1)

                                    ' Comprueba si se ha sobrepasado el número máximo de empleados de la licencia
                                    'ImportEmployees_CheckMaxEmployeesNotExcededInFuture()
                                Catch ex As Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeeAscii")
                                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                    bolRet = False
                                End Try
                            Else ' Columnas inválidas
                                Me.State.Result = DataLinkResultEnum.InvalidColumns
                                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidColumns", "") & " '" & InvalidColumn & "'" & vbNewLine
                            End If
                        Else
                            ' No hay registros
                            Me.State.Result = DataLinkResultEnum.NoRegisters
                            strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoRegisters", "") & vbNewLine
                        End If
                    Else ' No hay ningún libro en el fichero excel de definición de columnas
                        Me.State.Result = DataLinkResultEnum.NoSheets
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoSheetsInExcelFile", "") & vbNewLine
                    End If
                Else
                    ' Ficheros incorrectos
                    If bolIsFileOKAscii = False Then
                        Me.State.Result = DataLinkResultEnum.InvalidASCIIFile
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidASCIIFile", "") & vbNewLine
                    ElseIf bolIsFileOKExcel = False Then
                        Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidFormatFile", "") & vbNewLine
                    End If
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesAscii")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba el log
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function GetDataAsciiEmployees(ByVal strData As String, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String,
                                                   ByVal strSeparator As String, ByVal CompositeContractType As Integer, ByVal EmpresaColumnPosition As Integer, ByVal ColumnsInitialPos() As Integer, ByVal ColumnsLenght() As Integer, Optional ByRef errorInfo As String = "") As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                ' Descompone línea ...
                If strSeparator <> "" Then
                    ' según caracter delimitador
                    Dim SplitData() As String = strData.Split(strSeparator)
                    Dim intPos As Integer
                    Dim strValue As String = ""
                    For intColumn As Integer = 0 To ColumnsPos.Length - 1
                        intPos = ColumnsPos(intColumn)

                        If intPos >= 0 Then
                            If SplitData.Length > intPos Then
                                strValue = SplitData(intPos)
                                If strValue IsNot Nothing Then
                                    ColumnsVal(intColumn) = strValue.Trim
                                End If
                            End If
                        End If
                    Next
                Else
                    ' con formato fijo
                    Dim intPos As Integer
                    For intColumn As Integer = 0 To ColumnsPos.Length - 1
                        intPos = ColumnsPos(intColumn)
                        If intPos >= 0 Then
                            ColumnsVal(intColumn) = strData.Substring(ColumnsInitialPos(intPos) - 1, ColumnsLenght(intPos)).Trim
                        End If
                    Next
                End If

                ' Comprueba que los valores obligatorios estén informados
                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate).Length > 0 And (ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI).Length > 0 OrElse ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey).Length > 0) And
                       ColumnsVal(RoboticsExternAccess.EmployeeColumns.Name).Length > 0 And ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract).Length > 0 Then

                    If CompositeContractType > 0 Then
                        If CompositeContractType <> 3 Then
                            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                                bolRet = True
                                Dim strBeginDate As String = ""
                                '"yyyy/MM/dd"
                                strBeginDate = Format(CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)), "yyyy/MM/dd")
                                strBeginDate = strBeginDate.Replace("/", "")
                                Select Case CompositeContractType
                                    Case 1
                                        'CONTRATO_FECHA ALTA
                                        ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & strBeginDate
                                    Case 2
                                        '"EMPRESA_CONTRATO_FECHA ALTA"
                                        If EmpresaColumnPosition <> -1 Then
                                            If ColumnsVal(EmpresaColumnPosition).Length > 0 Then
                                                ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(EmpresaColumnPosition) & "." & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & strBeginDate
                                            Else
                                                bolRet = False
                                            End If
                                        Else
                                            bolRet = False
                                        End If
                                End Select
                            Else
                                bolRet = False
                            End If
                        Else
                            ' CONTRATO_PERIODO
                            bolRet = True
                            Try
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Period).Length > 0 Then
                                    ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "." & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Period)
                                Else
                                    bolRet = False
                                End If
                            Catch ex As Exception
                                bolRet = False
                            End Try
                        End If
                    Else
                        bolRet = True
                    End If

                End If
                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate).Length > 0 Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.BeginDate", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate) & " "
                End If

                If (ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey).Length > 0) Then
                    errorInfo = errorInfo & Me.State.Language.Translate("Import.LogEvent.ImportPrimaryKey", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract).Length > 0 Then
                    errorInfo = errorInfo & Me.State.Language.Translate("Import.LogEvent.IDContract", "") & " " & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & " "
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataAsciiEmployees")
            End Try

            Return bolRet

        End Function

        Private Function ImportEmployeesASCII_CheckMaxEmployeeNotExceded(ByVal intLines As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String,
                                                   ByVal strSeparator As String, ByVal CompositeContractType As Integer, ByVal EmpresaColumnPosition As Integer, ByVal ColumnsInitialPos() As Integer,
                                                   ByVal ColumnsLenght() As Integer, ByVal bolClonar As Boolean, ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer, ByRef EmployeeState As Employee.roEmployeeState, ByRef strLogEvent As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim dtEmpUserFields As DataTable = Nothing
                Dim dtEmpContracts As DataTable = Nothing
                Dim newID As Long = 0

                ' Lee empleados y contratos de la base de datos
                ImportEmployees_GetContracts(newID, dtEmpContracts, dtEmpUserFields)

                Dim strLine As String
                Dim intIDEmployee As Integer = 0

                ' Recorre el fichero
                For i As Integer = 0 To intLines - 1
                    strLine = oAsciiReader.ReadLine()
                    If Me.GetDataAsciiEmployees(strLine, intColumnas, ColumnsPos, ColumnsVal, strSeparator, CompositeContractType, EmpresaColumnPosition, ColumnsInitialPos, ColumnsLenght) Then
                        ' Determina si es un empleado nuevo
                        intIDEmployee = ImportEmployees_GetIDEmployee(dtEmpUserFields, bolClonar, iKeyPositionVal, iNifPositionVal, ColumnsVal, EmpresaColumnPosition, EmployeeState)

                        ' Simula el contrato
                        ImportEmployees_SimulateContract(newID, intIDEmployee, dtEmpUserFields, dtEmpContracts, ColumnsVal, iKeyPositionVal, iNifPositionVal)
                    End If
                Next i

                ' Comprueba que no se hayan sobrepasado los empleados de la licencia a día de hoy
                bolRet = ImportEmployees_CheckMaxEmployeesNotExceded(dtEmpContracts, strLogEvent)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesASCII_CheckMaxEmployeeNotExceded")
                bolRet = True ' Si hay cualquier error no previsto continúa con la importación
            Finally
                If Not IsNothing(oAsciiReader) Then oAsciiReader.Close()
                CheckAsciiFile()
            End Try

            Return bolRet
        End Function

#End Region

#Region "1- IMPORTAR EMPLEADOS SAGE MURANO"

        Public Function ImportEmployeeSageMuranoAscii() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKAscii Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(EmployeeSageMurano)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(EmployeeSageMurano)).Length - 1) As String

                    ' Definimos array con los nombres de los campos personalizados
                    Dim ColumnsUsrName(-1) As String

                    ' Definimos variables para guardar los logs

                    Dim intNewEmployees As Integer = 0
                    Dim intUpdateEmployees As Integer = 0


                    ' Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & vbNewLine

                    ' Contamos el número de lineas
                    Dim intLines As Integer = Me.CountLinesAscii()
                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & (intLines - 1).ToString & vbNewLine
                    If intLines > 1 Then



                        Dim oEmployeeState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.State, oEmployeeState)
                        Dim oUserFIeldState As New UserFields.roUserFieldState
                        roBusinessState.CopyTo(Me.State, oUserFIeldState)


                        Dim tbUserFieldList As DataTable = UserFields.roUserField.GetUserFields(Types.EmployeeField, oUserFIeldState,, )
                        createMandatoryFields(oEmployeeState, oUserFIeldState, True)
                        createExcelUserFields(ColumnsVal, Nothing, oEmployeeState, oUserFIeldState, True)

                        bolRet = True

                        'Recorrer Fichero Ascii
                        Dim strLine As String = ""
                        Dim strMsg As String = ""

                        ' Determina si se sobrepasaría licencia de máximo número de empleados
                        bolRet = ImportEmployeesSAGE_CheckMaxEmployeeNotExceded(intLines, ColumnsVal, RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey, RoboticsExternAccess.EmployeeColumns.DNI, oEmployeeState, strLogEvent)
                        If bolRet = False Then Exit Try

                        'Posicionarse en el segundo registro
                        Me.oAsciiReader.ReadLine()

                        'Recorrer fichero ASCII
                        For intRow As Integer = 1 To intLines - 1
                            strMsg = ""
                            strLine = Me.oAsciiReader.ReadLine()

                            'Obtener datos del Empleado
                            If Me.GetDataAsciiSageMurano(strLine, ColumnsVal) Then

                                Try
                                    ' Busca el empleado
                                    Dim intIDEmployee As Integer = isEmployeeNew(ColumnsVal, EmployeeSageMurano.ImportPrimaryKey, EmployeeSageMurano.DNI, oUserFIeldState)

                                    ' Importa el registro
                                    If intIDEmployee = -1 Then
                                        ' Nuevo empleado
                                        bolRet = Me.NewEmployeeSageMurano(ColumnsVal, strMsg, tbUserFieldList, False)
                                        If bolRet Then intNewEmployees = intNewEmployees + 1
                                    Else
                                        'Actualizar datos del Empleado
                                        bolRet = Me.UpdateEmployeeSageMurano(intIDEmployee, ColumnsVal, strMsg, tbUserFieldList, False)
                                        If bolRet Then intUpdateEmployees = intUpdateEmployees + 1
                                    End If

                                    If bolRet = False Then
                                        Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow & vbNewLine
                                        msgLog &= strMsg & vbNewLine
                                    End If

                                Catch ex As DbException
                                    Me.State.Result = DataLinkResultEnum.Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeeSAGEMURANO")
                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                    bolRet = False
                                Catch ex As Exception
                                    Me.State.Result = DataLinkResultEnum.Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeeSAGEMURANO")
                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                    bolRet = False
                                Finally

                                End Try
                            Else
                                Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & vbNewLine
                            End If
                        Next

                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & vbNewLine
                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewEmployees", "") & intNewEmployees.ToString & vbNewLine
                        strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateEmployees", "") & intUpdateEmployees.ToString & vbNewLine

                        ' Auditamos importación empleados
                        Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                        Me.State.AddAuditParameter(tbParameters, "{ImportEmployeesType}", "Ascii", "", 1)
                        Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportEmployees, "", tbParameters, -1)

                        ' Llama al BroadCaster
                        If intNewEmployees > 0 Or intUpdateEmployees > 0 Then
                            Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                        End If

                        ' Comprueba si se ha sobrepasado el número máximo de empleados de la licencia
                        'ImportEmployees_CheckMaxEmployeesNotExcededInFuture()
                    Else
                        ' No hay registros
                        Me.State.Result = DataLinkResultEnum.NoRegisters
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoRegisters", "") & vbNewLine
                    End If
                    Me.oAsciiReader.Close()
                Else
                    ' Fichero Ascii incorrecto
                    Me.State.Result = DataLinkResultEnum.InvalidASCIIFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidASCIIFile", "") & vbNewLine
                End If

            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeeSageMuranoAscii")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine

            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, , Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function ImportEmployeesSAGE_CheckMaxEmployeeNotExceded(ByVal intLines As Integer, ByRef ColumnsVal() As String,
                                              ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer, ByRef EmployeeState As Employee.roEmployeeState, ByRef strLogEvent As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim dtEmpUserFields As DataTable = Nothing
                Dim dtEmpContracts As DataTable = Nothing
                Dim newID As Long = 0


                ' Lee empleados y contratos de la base de datos
                ImportEmployees_GetContracts(newID, dtEmpContracts, dtEmpUserFields)

                Dim strLine As String
                Dim intIDEmployee As Integer = 0

                strLine = oAsciiReader.ReadLine()

                ' Recorre el fichero
                For i As Integer = 1 To intLines - 1
                    strLine = oAsciiReader.ReadLine()

                    'Obtener datos del Empleado
                    If Me.GetDataAsciiSageMurano(strLine, ColumnsVal) Then
                        ' Determina si es un empleado nuevo
                        intIDEmployee = ImportEmployees_GetIDEmployee(dtEmpUserFields, False, iKeyPositionVal, iNifPositionVal, ColumnsVal, -1, EmployeeState)

                        ' Simula el contrato
                        ImportEmployeesSAGE_SimulateContracts(intIDEmployee, newID, ColumnsVal, dtEmpContracts)
                    End If
                Next i

                ' Comprueba que no se hayan sobrepasado los empleados de la licencia a día de hoy
                bolRet = ImportEmployees_CheckMaxEmployeesNotExceded(dtEmpContracts, strLogEvent)

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployeesSAGE_CheckMaxEmployeeNotExceded")
                bolRet = True ' Si hay cualquier error no previsto continúa con la importación

            Finally
                If Not IsNothing(oAsciiReader) Then oAsciiReader.Close()
                CheckAsciiFile()
            End Try

            Return bolRet
        End Function

        Private Sub ImportEmployeesSAGE_SimulateContracts(ByVal intIDEmployee As Integer, ByRef newID As Integer, ByVal ColumnsVal() As String, ByRef dtEmpContracts As DataTable)
            Dim Contracts(-1) As String
            Dim intPeriods As Integer = 0
            Dim intContracts As Integer = 0

            ' Descompone contratos
            intPeriods = roTypes.StringItemsCount(ColumnsVal(EmployeeSageMurano.ActiveDays), "/")

            If (intPeriods Mod 2) = 0 Then
                intContracts = intPeriods / 2
            Else
                If intPeriods > 1 Then
                    intContracts = Int(intPeriods / 2) + 1
                Else
                    intContracts = 1
                End If
            End If

            ' Asigna fechas de contratos
            Dim ActualContract As Integer = 0
            ReDim Contracts(intContracts - 1)
            For z As Integer = 0 To intPeriods - 1
                If z = 0 Or z Mod 2 = 0 Then
                    Contracts(ActualContract) = roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                    ActualContract = ActualContract + 1
                Else
                    Contracts(ActualContract - 1) = Contracts(ActualContract - 1) & "@" & roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                End If
            Next

            ' Elimina contratos del empleado
            Dim row() As DataRow = dtEmpContracts.Select("idEmployee=" & intIDEmployee)
            For z As Integer = 0 To row.Length - 1
                row(z).Delete()
            Next

            ' Crea los nuevos contratos
            If intIDEmployee = -1 Then
                intIDEmployee = newID
                newID += 1
            End If

            Dim rowEC As DataRow
            For w As Integer = 0 To Contracts.Length - 1
                ' Crea el contrato
                rowEC = dtEmpContracts.NewRow
                rowEC("idEmployee") = intIDEmployee
                rowEC("idContract") = ColumnsVal(EmployeeSageMurano.CompanyCode) & "." & ColumnsVal(EmployeeSageMurano.EmployeeCode) & "." & (w + 1).ToString
                rowEC("BeginDate") = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 0, "@")).Value
                If IsDate(roTypes.String2Item(Contracts(w), 1, "@")) Then
                    rowEC("EndDate") = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 1, "@")).Value
                Else
                    rowEC("EndDate") = "2079/01/01"
                End If

                dtEmpContracts.Rows.Add(rowEC)
            Next

        End Sub

        Private Function GetDataAsciiSageMurano(ByVal strData As String, ByRef ColumnsVal() As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                'Inicializamos los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                'Leemos los datos del empleado
                Dim SplitData() As String = strData.Split(";")

                If SplitData.Length < 19 Then Return False

                If SplitData.Length >= (EmployeeSageMurano.ID) Then
                    ColumnsVal(EmployeeSageMurano.ID) = SplitData(EmployeeSageMurano.ID).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.DNI) Then
                    ColumnsVal(EmployeeSageMurano.DNI) = SplitData(EmployeeSageMurano.DNI).Trim
                    ColumnsVal(EmployeeSageMurano.ImportPrimaryKey) = SplitData(EmployeeSageMurano.DNI).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.LastName) Then
                    ColumnsVal(EmployeeSageMurano.LastName) = SplitData(EmployeeSageMurano.LastName).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.NameEmployee) Then
                    ColumnsVal(EmployeeSageMurano.NameEmployee) = SplitData(EmployeeSageMurano.NameEmployee).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.RegisterSystemDate) Then
                    ColumnsVal(EmployeeSageMurano.RegisterSystemDate) = SplitData(EmployeeSageMurano.RegisterSystemDate).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.ActiveDays) Then
                    ColumnsVal(EmployeeSageMurano.ActiveDays) = SplitData(EmployeeSageMurano.ActiveDays).Trim
                    If Not ColumnsVal(EmployeeSageMurano.ActiveDays).Contains("/") Then
                        ' 2018/07/09 Si solo existe una sola fecha de inicio
                        ' asignamos como fecha de inicio la del campo registersystemdate
                        ' Modificacion realizada bajo peticion de Jordi Casas y relacionada con el cliente thermoeurop
                        ColumnsVal(EmployeeSageMurano.ActiveDays) = ColumnsVal(EmployeeSageMurano.RegisterSystemDate)
                    End If
                End If

                If SplitData.Length >= (EmployeeSageMurano.Departments) Then
                    ColumnsVal(EmployeeSageMurano.Departments) = SplitData(EmployeeSageMurano.Departments).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.EmployeeCode) Then
                    ColumnsVal(EmployeeSageMurano.EmployeeCode) = SplitData(EmployeeSageMurano.EmployeeCode).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.CompanyCode) Then
                    ColumnsVal(EmployeeSageMurano.CompanyCode) = SplitData(EmployeeSageMurano.CompanyCode).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_EMAIL) Then
                    ColumnsVal(EmployeeSageMurano.USR_EMAIL) = SplitData(EmployeeSageMurano.USR_EMAIL).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_SS) Then
                    ColumnsVal(EmployeeSageMurano.USR_SS) = SplitData(EmployeeSageMurano.USR_SS).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_ADDRESS) Then
                    ColumnsVal(EmployeeSageMurano.USR_ADDRESS) = SplitData(EmployeeSageMurano.USR_ADDRESS).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_POSTALCODE) Then
                    ColumnsVal(EmployeeSageMurano.USR_POSTALCODE) = SplitData(EmployeeSageMurano.USR_POSTALCODE).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_TOWN) Then
                    ColumnsVal(EmployeeSageMurano.USR_TOWN) = SplitData(EmployeeSageMurano.USR_TOWN).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_PROVINCE) Then
                    ColumnsVal(EmployeeSageMurano.USR_PROVINCE) = SplitData(EmployeeSageMurano.USR_PROVINCE).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_PERSONALPHONE) Then
                    ColumnsVal(EmployeeSageMurano.USR_PERSONALPHONE) = SplitData(EmployeeSageMurano.USR_PERSONALPHONE).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_PERSONALMOBILE) Then
                    ColumnsVal(EmployeeSageMurano.USR_PERSONALMOBILE) = SplitData(EmployeeSageMurano.USR_PERSONALMOBILE).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_BIRTHDATE) Then
                    ColumnsVal(EmployeeSageMurano.USR_BIRTHDATE) = SplitData(EmployeeSageMurano.USR_BIRTHDATE).Trim
                End If

                If SplitData.Length >= (EmployeeSageMurano.USR_SEX) Then
                    ColumnsVal(EmployeeSageMurano.USR_SEX) = SplitData(EmployeeSageMurano.USR_SEX).Trim
                End If

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ColumnsVal(n).Replace("""", "")
                Next

                If ColumnsVal(EmployeeSageMurano.Departments).Length > 0 Then
                    '“1 – EMPRESA 1/SECCIO A/DEPARTAMENT B”
                    For n As Integer = 0 To roTypes.StringItemsCount(ColumnsVal(EmployeeSageMurano.Departments), "/") - 1
                        Select Case n
                            Case 0 : ColumnsVal(EmployeeSageMurano.Level1) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 1 : ColumnsVal(EmployeeSageMurano.Level2) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 2 : ColumnsVal(EmployeeSageMurano.Level3) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 3 : ColumnsVal(EmployeeSageMurano.Level4) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 4 : ColumnsVal(EmployeeSageMurano.Level5) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 5 : ColumnsVal(EmployeeSageMurano.Level6) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 6 : ColumnsVal(EmployeeSageMurano.Level7) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 7 : ColumnsVal(EmployeeSageMurano.Level8) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 8 : ColumnsVal(EmployeeSageMurano.Level9) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                            Case 9 : ColumnsVal(EmployeeSageMurano.Level10) = roTypes.String2Item(ColumnsVal(EmployeeSageMurano.Departments), n, "/")
                        End Select
                    Next
                End If

                If ColumnsVal(EmployeeSageMurano.EmployeeCode).Length > 0 And (ColumnsVal(EmployeeSageMurano.DNI).Length > 0 Or ColumnsVal(EmployeeSageMurano.ImportPrimaryKey).Length > 0) And
                   ColumnsVal(EmployeeSageMurano.LastName).Length > 0 And ColumnsVal(EmployeeSageMurano.CompanyCode).Length > 0 And
                   ColumnsVal(EmployeeSageMurano.ActiveDays).Length > 0 Then
                    bolRet = True
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataAsciiSageMurano")
            End Try

            Return bolRet

        End Function

        Private Function NewEmployeeSageMurano(ByVal ColumnsVal() As String, ByRef strMsg As String, ByVal tbUserFieldList As DataTable, ByVal CallBroadcaster As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oContractState = New Contract.roContractState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oUserFIeldState As New UserFields.roUserFieldState
            Dim DateField As Date

            roBusinessState.CopyTo(Me.State, oUserFIeldState)

            Me.State.UpdateStateInfo()
            strMsg = ""

            ' Obtiene el nombre completo del empleado
            Dim strName As String = ColumnsVal(EmployeeSageMurano.LastName)
            If strName.Trim <> "" Then
                strName &= ", "
            Else
                strName = ""
            End If

            strName &= ColumnsVal(EmployeeSageMurano.NameEmployee)

            ' Obtiene los contratos
            Dim bolIsExpiredContract As Boolean = False
            Dim Contracts(-1) As String
            Dim intPeriods As Integer = 0
            Dim intContracts As Integer = 0
            intPeriods = roTypes.StringItemsCount(ColumnsVal(EmployeeSageMurano.ActiveDays), "/")

            If (intPeriods Mod 2) = 0 Then
                ' Si todos los contratos estan cerrados
                ' verificamos si el ultimo fin de contrato es anterior a hoy
                ' si es asi no lo importamos, es un empleado dado de baja 
                If IsDate(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), intPeriods - 1, "/")) Then
                    Dim xEndDate As Date = roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), intPeriods - 1, "/")).Value
                    bolIsExpiredContract = (xEndDate < Now.Date)
                End If
                intContracts = intPeriods / 2
            Else
                If intPeriods > 1 Then
                    intContracts = Int(intPeriods / 2) + 1
                Else
                    intContracts = 1
                End If
            End If

            ' Obtenemos el nº de contratos
            Dim ActualContract As Integer = 0
            ReDim Contracts(intContracts - 1)
            For z As Integer = 0 To intPeriods - 1
                If z = 0 Or z Mod 2 = 0 Then
                    Contracts(ActualContract) = roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                    ActualContract = ActualContract + 1
                Else
                    Contracts(ActualContract - 1) = Contracts(ActualContract - 1) & "@" & roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                End If
            Next

            ' Validamos los contratos
            bolRet = Me.ValidateContracts(ColumnsVal, Contracts, intPeriods)

            If Not bolIsExpiredContract And bolRet Then
                Dim oEmployee As New Employee.roEmployee

                oEmployee.ID = -1

                oEmployee.Name = strName

                oEmployee.Type = "A"

                If Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, CallBroadcaster) Then
                    Dim oUserField As UserFields.roUserField = Nothing

                    bolRet = createNIFIfNecessary(ColumnsVal(EmployeeSageMurano.DNI), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)

                    If bolRet AndAlso ColumnsVal(EmployeeSageMurano.ImportPrimaryKey) <> String.Empty Then
                        bolRet = CreateImportPrimaryKeyIfNecessary(ColumnsVal(EmployeeSageMurano.ImportPrimaryKey), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                    End If

                    ' Crea campo codigo empleado murano
                    If bolRet Then
                        oUserField = New UserFields.roUserField(oUserFIeldState, "CODIGO_EMPLEADO", Types.EmployeeField, False, False)

                        If oUserField.History Then
                            DateField = Now.Date
                        Else
                            DateField = New Date(1900, 1, 1)
                        End If

                        Dim bolUsedInProcess As Boolean = False
                        Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='CODIGO_EMPLEADO'")
                        If oFields.Length > 0 Then
                            If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                bolUsedInProcess = True
                            End If
                        End If

                        Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, "CODIGO_EMPLEADO", DateField, New UserFields.roUserFieldState)
                        oEmployeeUserField.FieldValue = ColumnsVal(EmployeeSageMurano.EmployeeCode)
                        bolRet = oEmployeeUserField.Save(,, bolUsedInProcess)
                    End If

                    If bolRet Then
                        oUserField = New UserFields.roUserField(oUserFIeldState, "CODIGO_EMPRESA", Types.EmployeeField, False, False)
                        If oUserField.History Then
                            DateField = Now.Date
                        Else
                            DateField = New Date(1900, 1, 1)
                        End If

                        Dim bolUsedInProcess As Boolean = False
                        Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='CODIGO_EMPRESA'")
                        If oFields.Length > 0 Then
                            If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                bolUsedInProcess = True
                            End If
                        End If

                        Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, "CODIGO_EMPRESA", DateField, New UserFields.roUserFieldState)
                        oEmployeeUserField.FieldValue = ColumnsVal(EmployeeSageMurano.CompanyCode)
                        bolRet = oEmployeeUserField.Save(, bolUsedInProcess)
                    End If


                    If bolRet Then

                        Dim strUsrValue As String = ""
                        Dim strUsrName As String = ""
                        Dim intUsrCol As Integer = 0

                        For intCol As Integer = EmployeeSageMurano.USR_EMAIL To EmployeeSageMurano.USR_SEX
                            strUsrValue = ColumnsVal(intCol)
                            If strUsrValue <> "" Then
                                Select Case intCol
                                    Case EmployeeSageMurano.USR_EMAIL : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_EMAIL", "")
                                    Case EmployeeSageMurano.USR_SS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SS", "")
                                    Case EmployeeSageMurano.USR_ADDRESS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_ADDRESS", "")
                                    Case EmployeeSageMurano.USR_POSTALCODE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_POSTALCODE", "")
                                    Case EmployeeSageMurano.USR_TOWN : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_TOWN", "")
                                    Case EmployeeSageMurano.USR_PROVINCE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PROVINCE", "")
                                    Case EmployeeSageMurano.USR_PERSONALPHONE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALPHONE", "")
                                    Case EmployeeSageMurano.USR_PERSONALMOBILE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALMOBILE", "")
                                    Case EmployeeSageMurano.USR_BIRTHDATE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_USR_BIRTHDATE", "")
                                    Case EmployeeSageMurano.USR_SEX : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SEX", "")
                                End Select

                                oUserField = New UserFields.roUserField(oUserFIeldState, strUsrName, Types.EmployeeField, False, False)

                                If oUserField.History Then
                                    DateField = Now.Date
                                Else
                                    DateField = New Date(1900, 1, 1)
                                End If

                                Dim bolUsedInProcess As Boolean = False
                                Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                                If oFields.Length > 0 Then
                                    If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                        bolUsedInProcess = True
                                    End If
                                End If

                                Dim oEmployeeUserField = New UserFields.roEmployeeUserField(oEmployee.ID, strUsrName, DateField, New UserFields.roUserFieldState)
                                oEmployeeUserField.FieldValue = strUsrValue
                                bolRet = oEmployeeUserField.Save(,, bolUsedInProcess)

                                If Not bolRet Then Exit For
                            End If
                            intUsrCol += 1

                        Next
                    End If

                    If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidEmployee

                    If bolRet Then

                        Dim intIDGroup As Integer

                        bolRet = Me.CreateDepartamentsSageMurano(ColumnsVal, oGroupState, intIDGroup)

                        If bolRet And intIDGroup <> 0 Then

                            Dim xBeginDate As Date
                            If IsDate(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), 0, "/")) Then
                                xBeginDate = roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), 0, "/")).Value
                            Else
                                xBeginDate = Now.Date
                            End If

                            Dim xEndDate As Date
                            xEndDate = New Date(2079, 1, 1)

                            ' Insertamos al empleado en la tabla de Empleados y grupos
                            Dim oMobility As New Employee.roMobility
                            With oMobility
                                .IdGroup = intIDGroup
                                .BeginDate = xBeginDate
                                .EndDate = New Date(2079, 1, 1)
                                .IsTransfer = False
                            End With

                            bolRet = Employee.roMobility.SaveMobility(oEmployee.ID, oMobility, oEmployeeState, CallBroadcaster, True)

                            Dim strIDCard As String = ""

                            If bolRet Then

                                'Insertamos el empleado en la tabla de contratos
                                For w As Integer = 0 To Contracts.Length - 1
                                    Dim strIDContract As String = ""
                                    strIDContract = ColumnsVal(EmployeeSageMurano.CompanyCode) & "." & ColumnsVal(EmployeeSageMurano.EmployeeCode) & "." & (w + 1).ToString

                                    Dim oContract As New Contract.roContract(oContractState, strIDContract)
                                    With oContract
                                        .IDEmployee = oEmployee.ID
                                        .BeginDate = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 0, "@")).Value
                                        If IsDate(roTypes.String2Item(Contracts(w), 1, "@")) Then
                                            .EndDate = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 1, "@")).Value
                                        Else
                                            .EndDate = xEndDate
                                        End If
                                        .IDCard = Nothing
                                    End With
                                    bolRet = oContract.Save(, CallBroadcaster)
                                    If Not bolRet Then
                                        Me.State.Result = DataLinkResultEnum.InvalidContract
                                        Exit For
                                    End If
                                Next

                            Else
                                Me.State.Result = DataLinkResultEnum.InvalidEmployee
                            End If

                            ' Habilitamos el fichaje por huella
                            If bolRet Then
                                bolRet = CreateBioAuthenticationMethods(oEmployee)
                            End If

                        End If

                        If Not bolRet Then
                            ' Borrar empleado
                            Dim oDeleteEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
                            If Not Employee.roEmployee.DeleteEmployee(oEmployee.ID, oDeleteEmployeeState, CallBroadcaster) Then
                                Me.State.Result = DataLinkResultEnum.InvalidEmployee
                            End If
                        End If

                    Else
                        Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    End If
                End If
            Else
                Me.State.Result = DataLinkResultEnum.ExpiredContract
            End If

            If Not bolRet Then
                strMsg = "Error Creating Employee " & strName & ": " & Me.State.Result.ToString & " - " & Me.State.ErrorText
                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidGroup
                        strMsg &= " (" & oGroupState.Result.ToString & " - " & oGroupState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidContract
                        strMsg &= " (" & oContractState.Result.ToString & " - " & oContractState.ErrorText & ")"
                    Case DataLinkResultEnum.Exception
                        strMsg &= " (" & Me.State.ErrorText & ")"
                End Select

                roLog.GetInstance().logMessage(roLog.EventType.roWarning, strMsg)
            End If

            Return bolRet

        End Function

        Private Function UpdateEmployeeSageMurano(ByVal intIDEmployee As Integer, ByVal ColumnsVal() As String, ByRef strMsg As String, ByVal tbUserFieldList As DataTable, ByVal CallBroadcaster As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim DateField As Date

            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oContractState = New Contract.roContractState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
            Dim oGroupState As New Group.roGroupState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)

            Me.State.UpdateStateInfo()
            strMsg = ""

            Dim oUserFIeldState As New UserFields.roUserFieldState
            roBusinessState.CopyTo(Me.State, oUserFIeldState)

            ' Obtener el nombre completo del empleado
            Dim strName As String = ColumnsVal(EmployeeSageMurano.LastName)
            If strName.Trim <> "" Then
                strName &= ", "
            Else
                strName = ""
            End If

            strName &= ColumnsVal(EmployeeSageMurano.NameEmployee)


            Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, oEmployeeState, False)
            If oEmployee IsNot Nothing Then

                oEmployee.Name = strName

                bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, CallBroadcaster)
                If bolRet Then
                    ' Actualizamos los campos de la ficha
                    Dim strUsrValue As String = ""
                    Dim strUsrName As String = ""
                    Dim intUsrCol As Integer = 0
                    For intCol As Integer = EmployeeSageMurano.USR_EMAIL To EmployeeSageMurano.USR_SEX
                        strUsrValue = ColumnsVal(intCol)
                        If strUsrValue <> "" Then
                            Select Case intCol
                                Case EmployeeSageMurano.USR_EMAIL : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_EMAIL", "")
                                Case EmployeeSageMurano.USR_SS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SS", "")
                                Case EmployeeSageMurano.USR_ADDRESS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_ADDRESS", "")
                                Case EmployeeSageMurano.USR_POSTALCODE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_POSTALCODE", "")
                                Case EmployeeSageMurano.USR_TOWN : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_TOWN", "")
                                Case EmployeeSageMurano.USR_PROVINCE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PROVINCE", "")
                                Case EmployeeSageMurano.USR_PERSONALPHONE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALPHONE", "")
                                Case EmployeeSageMurano.USR_PERSONALMOBILE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALMOBILE", "")
                                Case EmployeeSageMurano.USR_BIRTHDATE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_USR_BIRTHDATE", "")
                                Case EmployeeSageMurano.USR_SEX : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SEX", "")
                            End Select

                            Dim oUserField As New UserFields.roUserField(oUserFIeldState, strUsrName, Types.EmployeeField, False, False)

                            If oUserField.History Then
                                DateField = Now.Date
                            Else
                                DateField = New Date(1900, 1, 1)
                            End If

                            Dim bolUsedInProcess As Boolean = False
                            Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & strUsrName.Replace("'", "''") & "'")
                            If oFields.Length > 0 Then
                                If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                                    bolUsedInProcess = True
                                End If
                            End If

                            Dim oEmployeeUserField = New UserFields.roEmployeeUserField(oEmployee.ID, strUsrName, DateField, New UserFields.roUserFieldState)
                            oEmployeeUserField.FieldValue = strUsrValue
                            bolRet = oEmployeeUserField.Save(,, bolUsedInProcess)


                            If Not bolRet Then Exit For
                        End If
                        intUsrCol += 1

                    Next
                End If

                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidEmployee

                bolRet = createNIFIfNecessary(ColumnsVal(EmployeeSageMurano.DNI), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)

                If bolRet AndAlso ColumnsVal(EmployeeSageMurano.ImportPrimaryKey) <> String.Empty Then
                    bolRet = CreateImportPrimaryKeyIfNecessary(ColumnsVal(EmployeeSageMurano.ImportPrimaryKey), oEmployee.ID, oEmployeeState, oUserFIeldState, tbUserFieldList)
                End If

                If bolRet Then
                    ' Crea campo codigo empleado murano
                    Dim oUserField As New UserFields.roUserField(oUserFIeldState, "CODIGO_EMPLEADO", Types.EmployeeField, False, False)
                    If oUserField.History Then
                        DateField = Now.Date
                    Else
                        DateField = New Date(1900, 1, 1)
                    End If

                    Dim bolUsedInProcess As Boolean = False
                    Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='CODIGO_EMPLEADO'")
                    If oFields.Length > 0 Then
                        If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                            bolUsedInProcess = True
                        End If
                    End If

                    Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, "CODIGO_EMPLEADO", DateField, New UserFields.roUserFieldState)
                    oEmployeeUserField.FieldValue = ColumnsVal(EmployeeSageMurano.EmployeeCode)
                    bolRet = oEmployeeUserField.Save(,, bolUsedInProcess)
                End If

                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidEmployee

                If bolRet Then
                    ' Crea campo codigo empresa
                    Dim oUserField As New UserFields.roUserField(oUserFIeldState, "CODIGO_EMPRESA", Types.EmployeeField, False, False)
                    If oUserField.History Then
                        DateField = Now.Date
                    Else
                        DateField = New Date(1900, 1, 1)
                    End If

                    Dim bolUsedInProcess As Boolean = False
                    Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='CODIGO_EMPRESA'")
                    If oFields.Length > 0 Then
                        If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                            bolUsedInProcess = True
                        End If
                    End If

                    Dim oEmployeeUserField As New UserFields.roEmployeeUserField(oEmployee.ID, "CODIGO_EMPRESA", DateField, New UserFields.roUserFieldState)
                    oEmployeeUserField.FieldValue = ColumnsVal(EmployeeSageMurano.CompanyCode)
                    bolRet = oEmployeeUserField.Save(,, bolUsedInProcess)


                End If

                If bolRet Then
                    ' Actualizamos los contratos
                    Dim intPeriods As Integer = 0
                    Dim intContracts As Integer = 0
                    intPeriods = roTypes.StringItemsCount(ColumnsVal(EmployeeSageMurano.ActiveDays), "/")

                    If (intPeriods Mod 2) = 0 Then
                        intContracts = intPeriods / 2
                    Else
                        If intPeriods > 1 Then
                            intContracts = Int(intPeriods / 2) + 1
                        Else
                            intContracts = 1
                        End If
                    End If

                    Dim xEndDate As Date
                    xEndDate = New Date(2079, 1, 1)

                    ' Obtenemos el nº de contratos
                    Dim Contracts(-1) As String
                    Dim ActualContract As Integer = 0
                    ReDim Contracts(intContracts - 1)
                    For z As Integer = 0 To intPeriods - 1
                        If z = 0 Or z Mod 2 = 0 Then
                            Contracts(ActualContract) = roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                            ActualContract = ActualContract + 1
                        Else
                            Contracts(ActualContract - 1) = Contracts(ActualContract - 1) & "@" & roTypes.Any2Time(roTypes.String2Item(ColumnsVal(EmployeeSageMurano.ActiveDays), z, "/")).Value
                        End If
                    Next

                    Dim strSQL As String

                    ' Obtenemos los contratos actuales
                    Dim oContractS = New Contract.roContractState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
                    Dim oOldContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(oEmployee.ID, oContractS)


                    ' Eliminamos los contratos actuales
                    strSQL = "@DELETE# FROM EmployeeContracts WHERE IDEmployee=" & oEmployee.ID
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidContract

                    ' Validamos los contratos
                    If bolRet Then
                        bolRet = Me.ValidateContracts(ColumnsVal, Contracts, intPeriods)
                    End If

                    If bolRet Then
                        'Insertamos los contratos actuales del empleado
                        For w As Integer = 0 To Contracts.Length - 1
                            Dim strIDContract As String = ""
                            Dim intLabAgree As Double = 0
                            Dim wEndDate As Date

                            If IsDate(roTypes.String2Item(Contracts(w), 1, "@")) Then
                                wEndDate = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 1, "@")).Value
                            Else
                                wEndDate = xEndDate
                            End If

                            strIDContract = ColumnsVal(EmployeeSageMurano.CompanyCode) & "." & ColumnsVal(EmployeeSageMurano.EmployeeCode) & "." & (w + 1).ToString

                            ' Si este contrato coincide con alguno de los anteriores , le asignamos el anterior convenio
                            If oOldContracts IsNot Nothing Then
                                Dim oRow As DataRow
                                For Each oRow In oOldContracts.Rows
                                    ' Para cada contrato antiguo , miramos si coincide con alguno de los nuevos
                                    ' y en ese caso le asignamos el mismo convenio
                                    If oRow("IDContract") = strIDContract Then
                                        intLabAgree = roTypes.Any2Double(oRow("IDLabAgree"))
                                    End If
                                Next
                            End If

                            strSQL = "@INSERT# INTO EmployeeContracts (IDEmployee, IDContract, BeginDate, EndDate, IDCard , IDLabAgree) Values(" &
                                     oEmployee.ID & ",'" & strIDContract & "'," & roTypes.Any2Time(roTypes.String2Item(Contracts(w), 0, "@")).SQLSmallDateTime &
                                     "," & roTypes.Any2Time(wEndDate).SQLSmallDateTime & ",0," & IIf(intLabAgree > 0, intLabAgree.ToString, "NULL") & ")"
                            bolRet = ExecuteSql(strSQL)
                            If Not bolRet Then
                                Me.State.Result = DataLinkResultEnum.InvalidContract
                                Exit For
                            End If
                        Next



                        If bolRet Then
                            ' Eliminamos los datos que no esten dentro de los contratos indicados anteriormente
                            bolRet = Contract.roContract.RemoveDaysWithoutContract(oEmployee.ID, oContractState)
                        End If

                    End If

                    If bolRet Then
                        'Actualizamos la movilidad en caso que el grupo actual no sea igual que el que llega en el registro
                        Dim intIDGroup As Integer

                        bolRet = Me.CreateDepartamentsSageMurano(ColumnsVal, oGroupState, intIDGroup)

                        If bolRet And intIDGroup <> 0 Then

                            Dim oActualMobility As New Employee.roMobility
                            oActualMobility = Employee.roMobility.GetCurrentMobility(oEmployee.ID, oEmployeeState)

                            If intIDGroup <> oActualMobility.IdGroup Then
                                If oActualMobility.BeginDate = Now.Date Then
                                    ' Si la fecha de inicio del grupo actual es hoy , modificamos la asignacion de grupo
                                    strSQL = "@UPDATE# EmployeeGroups Set IDGroup=" & intIDGroup & " " &
                                                "WHERE IDEmployee=" & oEmployee.ID & " AND BeginDate= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime
                                    bolRet = ExecuteSql(strSQL)

                                Else
                                    ' Cerramos la asignacion al grupo a fecha de ayer
                                    oActualMobility.EndDate = Now.Date.AddDays(-1)

                                    strSQL = "@UPDATE# EmployeeGroups Set EndDate= " & roTypes.Any2Time(Now.Date.AddDays(-1)).SQLSmallDateTime & " " &
                                                "WHERE IDEmployee=" & oEmployee.ID & " AND BeginDate= " & roTypes.Any2Time(oActualMobility.BeginDate.Date).SQLSmallDateTime
                                    bolRet = ExecuteSql(strSQL)

                                    If bolRet Then
                                        ' Insertamos la nueva mobilidad
                                        Dim xBeginDate As Date
                                        xBeginDate = Now.Date

                                        Dim xMEndDate As Date
                                        xMEndDate = New Date(2079, 1, 1)

                                        strSQL = "@INSERT# INTO EmployeeGroups (IDEmployee, IDGroup, BeginDate, EndDate) VALUES(" & oEmployee.ID & "," &
                                            intIDGroup & "," & roTypes.Any2Time(xBeginDate.Date).SQLSmallDateTime & "," & roTypes.Any2Time(xMEndDate.Date).SQLSmallDateTime & ")"

                                        bolRet = ExecuteSql(strSQL)
                                    End If
                                End If

                                If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            End If
                        End If
                    End If
                End If

            Else
                Me.State.Result = DataLinkResultEnum.InvalidEmployee
            End If

            If Not bolRet Then
                strMsg = "Error Updating Employee " & strName & ": " & Me.State.Result.ToString & " - " & Me.State.ErrorText
                Select Case Me.State.Result
                    Case DataLinkResultEnum.InvalidEmployee
                        strMsg &= " (" & oEmployeeState.Result.ToString & " - " & oEmployeeState.ErrorText & ")"
                    Case DataLinkResultEnum.InvalidContract
                        strMsg &= " (" & oContractState.Result.ToString & " - " & oContractState.ErrorText & ")"
                    Case DataLinkResultEnum.Exception
                        strMsg &= " (" & Me.State.ErrorText & ")"
                End Select
                roLog.GetInstance().logMessage(roLog.EventType.roWarning, strMsg)
            End If

            Return bolRet

        End Function

        Private Function ValidateContracts(ByVal ColumnsVal() As String, ByRef Contracts() As String, ByVal intPeriods As String) As Boolean
            Dim bolRet As Boolean = False
            Dim ActualContract As Integer = 0
            Dim DateContract As Date
            Dim BeginDate As Date
            Dim EndDate As Date
            Dim LastDate As Date

            LastDate = New Date(1900, 1, 1)

            DateContract = New Date(2079, 1, 1)

            If Len(roTypes.Any2String(ColumnsVal(EmployeeSageMurano.ActiveDays))) > 0 Then
                bolRet = True
                For w As Integer = 0 To Contracts.Length - 1
                    ' Validamos que el Empresa/Empleado no exista en ningún contrato
                    Dim strIDContract As String = ""
                    strIDContract = ColumnsVal(EmployeeSageMurano.CompanyCode) & "." & ColumnsVal(EmployeeSageMurano.EmployeeCode) & "."

                    If Len(roTypes.Any2String(ColumnsVal(EmployeeSageMurano.CompanyCode))) = 0 Then
                        bolRet = False
                        Exit For
                    End If

                    If Len(roTypes.Any2String(ColumnsVal(EmployeeSageMurano.EmployeeCode))) = 0 Then
                        bolRet = False
                        Exit For
                    End If

                    If roTypes.Any2Double(ExecuteScalar("@SELECT# count(*) from EmployeeContracts WHERE IDContract like '" & strIDContract & "%'")) > 0 Then
                        bolRet = False
                        Exit For
                    End If

                    ' Validamos las fechas
                    If Not IsDate(roTypes.String2Item(Contracts(w), 0, "@")) Then
                        bolRet = False
                        Exit For
                    End If

                    If Not IsDate(roTypes.String2Item(Contracts(w), 1, "@")) And (Contracts.Length - 1) > w Then
                        bolRet = False
                        Exit For
                    End If

                    BeginDate = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 0, "@")).Value

                    If Not IsDate(roTypes.String2Item(Contracts(w), 1, "@")) Then
                        EndDate = DateContract
                    Else
                        EndDate = roTypes.Any2Time(roTypes.String2Item(Contracts(w), 1, "@")).Value
                    End If

                    If LastDate >= BeginDate Then
                        bolRet = False
                        Exit For
                    End If

                    If BeginDate > EndDate Then
                        bolRet = False
                        Exit For
                    End If

                    If bolRet Then
                        Try
                            If w = 0 AndAlso Len(ColumnsVal(EmployeeSageMurano.RegisterSystemDate)) > 0 Then
                                Dim bolApplyRegisterSystemDate As New AdvancedParameter.roAdvancedParameter("VTLive.Datalink.SageMurano.ApplyRegisterSystemDate", New AdvancedParameter.roAdvancedParameterState)
                                If bolApplyRegisterSystemDate IsNot Nothing AndAlso Any2Boolean(bolApplyRegisterSystemDate.Value) Then
                                    ' Si tenemos activo el parametro avanzado (orden expresa de David, solo activar a clientes que lo pidan o se necesite)
                                    ' En el caso que sea el primer contrato, se debe revisar si la fecha de inicio del contrato es igual o anterior a la fecha de registersystemdate
                                    ' en caso de ser posterior, debemos actulizar la fecha de inicio de contrato con la de registersystemdate

                                    If BeginDate > Any2Time(ColumnsVal(EmployeeSageMurano.RegisterSystemDate)).Value Then
                                        BeginDate = Any2Time(ColumnsVal(EmployeeSageMurano.RegisterSystemDate)).Value
                                        Contracts(w) = BeginDate.ToString("yyyy/MM/dd") & "@" & EndDate.ToString("yyyy/MM/dd")
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    LastDate = EndDate
                Next
            End If

            Return bolRet

        End Function

        Private Function CreateDepartamentsSageMurano(ByVal ColumnsVal() As String, ByRef oGroupState As Group.roGroupState, ByRef intIDGroup As Integer) As Boolean
            ' Nivel 1
            Dim intIDEmpresa As Integer
            ' Nivel 2
            Dim intIDCentro As Integer

            Dim intIDNivel3 As Integer
            Dim intIDNivel4 As Integer
            Dim intIDNivel5 As Integer
            Dim intIDNivel6 As Integer
            Dim intIDNivel7 As Integer
            Dim intIDNivel8 As Integer
            Dim intIDNivel9 As Integer

            Dim bolRet As Boolean = True

            'Creamos la estructura organizativa
            If ColumnsVal(EmployeeSageMurano.Level1).Length > 0 Then

                Dim oGroup As Group.roGroup

                'Comprobamos si esta creado el nivel1
                oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level1), "1", oGroupState, )
                If oGroup Is Nothing Then
                    ' Creamos el nivel
                    oGroup = New Group.roGroup(oGroupState)
                    oGroup.Name = ColumnsVal(EmployeeSageMurano.Level1)
                    oGroup.Path = "1"
                    oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111119"
                    bolRet = oGroup.Save()
                    If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                End If
                intIDGroup = oGroup.ID
                intIDEmpresa = oGroup.ID

                If bolRet Then

                    'Creamos el Nivel2
                    If ColumnsVal(EmployeeSageMurano.Level2).Length > 0 Then

                        'Comprobamos si esta creado el nivel2
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level2), "1\" & intIDEmpresa, oGroupState, )
                        If oGroup Is Nothing Then
                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level2)
                            oGroup.Path = "1\" & intIDEmpresa
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup
                        End If
                        intIDGroup = oGroup.ID

                    End If

                End If

                intIDCentro = intIDGroup

                If bolRet Then

                    If ColumnsVal(EmployeeSageMurano.Level3).Length > 0 Then
                        'Comprobamos si esta creado el nivel3
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level3), "1\" & intIDEmpresa & "\" & intIDCentro, oGroupState, )

                        If oGroup Is Nothing Then
                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level3)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel3 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level4).Length > 0 Then
                        'Comprobamos si esta creado el nivel4
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level4), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level4)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If


                intIDNivel4 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level5).Length > 0 Then
                        'Comprobamos si esta creado el nivel5
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level5), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level5)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel5 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level6).Length > 0 Then
                        'Comprobamos si esta creado el nivel6
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level6), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level6)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel6 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level7).Length > 0 Then
                        'Comprobamos si esta creado el nivel7
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level7), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level7)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel7 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level8).Length > 0 Then
                        'Comprobamos si esta creado el nivel8
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level8), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level8)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel8 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level9).Length > 0 Then
                        'Comprobamos si esta creado el nivel9
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level9), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level9)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

                intIDNivel9 = intIDGroup

                If bolRet Then
                    If ColumnsVal(EmployeeSageMurano.Level10).Length > 0 Then
                        'Comprobamos si esta creado el nivel10
                        oGroup = Group.roGroup.GetGroupByNameInLevel(ColumnsVal(EmployeeSageMurano.Level10), "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8 & "\" & intIDNivel9, oGroupState, )

                        If oGroup Is Nothing Then

                            ' Creamos el nivel
                            oGroup = New Group.roGroup(oGroupState,)
                            oGroup.Name = ColumnsVal(EmployeeSageMurano.Level10)
                            oGroup.Path = "1\" & intIDEmpresa & "\" & intIDCentro & "\" & intIDNivel3 & "\" & intIDNivel4 & "\" & intIDNivel5 & "\" & intIDNivel6 & "\" & intIDNivel7 & "\" & intIDNivel8 & "\" & intIDNivel9
                            oGroup.SecurityFlags = "" '"3100000000111111111111111111111111111111"
                            bolRet = oGroup.Save()
                            If Not bolRet Then Me.State.Result = DataLinkResultEnum.InvalidGroup

                            intIDGroup = oGroup.ID

                        Else
                            intIDGroup = oGroup.ID
                        End If
                    End If
                End If

            Else
                intIDGroup = 1
            End If

            Return bolRet

        End Function

#End Region

#Region "Employee Helper Methods"

        Private Sub ImportEmployees_GetContracts(ByRef newId As Integer, ByRef dtEmpContracts As DataTable, ByRef dtEmpUserFields As DataTable)

            ' Lee el id máximo
            newId = Any2Double(ExecuteScalar("@SELECT# max(ID) from Employees")) + 1

            ' Lee los contratos
            dtEmpContracts = CreateDataTable("@SELECT# idEmployee, idContract, BeginDate, EndDate from EmployeeContracts", "EmployeeContracts")

            ' Lee los campos clave de la ficha
            Dim sSQL As String = "@SELECT# idEmployee, FieldName, Date, Value from EmployeeUserFieldValues where FieldName='NIF'"
            If Me.strImportPrimaryKeyUserField <> "NIF" Then sSQL = sSQL & " OR FieldName='" & Me.strImportPrimaryKeyUserField & "'"
            dtEmpUserFields = CreateDataTable(sSQL, "EmployeeUserFieldValues")
        End Sub

        Private Function ImportEmployees_SimulateContract(ByRef newId As Integer, ByVal intIDEmployee As Integer, ByRef dtEmpUserFields As DataTable, dtEmpContracts As DataTable, ByVal ColumnsVal() As String, ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer) As Boolean
            Dim bolRet As Boolean = True
            Try
                Dim RowUF As DataRow
                Dim RowEC As DataRow
                Dim RowECU() As DataRow

                If intIDEmployee = -1 Then
                    ' Crea el empleado en campo de la ficha
                    RowUF = dtEmpUserFields.NewRow
                    RowUF("idEmployee") = newId
                    newId += 1
                    RowUF("FieldName") = strImportPrimaryKeyUserField
                    If ColumnsVal(iKeyPositionVal) <> String.Empty Then
                        RowUF("Value") = ColumnsVal(iKeyPositionVal)
                    Else
                        RowUF("Value") = ColumnsVal(iNifPositionVal)
                    End If

                    dtEmpUserFields.Rows.Add(RowUF)

                    ' Crea el contrato
                    RowEC = dtEmpContracts.NewRow
                    RowEC("idEmployee") = newId
                    RowEC("idContract") = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract)

                    'Leemos la fecha de inicio de contrato/movilidad
                    If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                        RowEC("BeginDate") = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
                    Else
                        bolRet = False
                    End If

                    'Leemos la fecha de fin de contrato/movilidad
                    If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate)) Then
                        RowEC("EndDate") = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))
                    Else
                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) = String.Empty Then
                            RowEC("EndDate") = New Date(2079, 1, 1)
                        Else
                            bolRet = False
                        End If
                    End If

                    If bolRet Then dtEmpContracts.Rows.Add(RowEC)
                Else
                    ' Actualiza o crea el contrato
                    ' Busca el contrato
                    RowECU = dtEmpContracts.Select("idEmployee=" & intIDEmployee & " and idContract='" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract) & "'")

                    If RowECU.Length > 0 Then

                        If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                            RowECU(0)("BeginDate") = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
                        End If

                        If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate)) Then
                            RowECU(0)("EndDate") = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))
                        Else
                            If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) = String.Empty Then
                                RowECU(0)("EndDate") = New Date(2079, 1, 1)
                            End If
                        End If

                        If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndContractReason).Length > 0 Then
                            RowECU(0)("EndContractReason") = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndContractReason))
                        End If
                    Else
                        ' Crea un contrato nuevo
                        ' Selecciona el último contrato
                        RowECU = dtEmpContracts.Select("idEmployee=" & intIDEmployee, "EndDate desc")
                        If RowECU.Length > 0 Then

                            Dim xBeginDate As Date
                            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate)) Then
                                xBeginDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate))
                            Else
                                bolRet = False
                            End If

                            Dim xEndDate As Date
                            If IsDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate)) Then
                                xEndDate = CDate(ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate))
                            Else
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) = String.Empty Then
                                    xEndDate = New Date(2079, 1, 1)
                                Else
                                    bolRet = False
                                End If
                            End If

                            ' Comprueba que no se cruce el último contrato
                            If bolRet AndAlso RowECU(0)("BeginDate") <= xBeginDate Then
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.BeginDate) < RowECU(0)("EndDate") Then
                                    ' Finaliza el contrato anterior
                                    RowECU(0)("EndDate") = xBeginDate.AddDays(-1)
                                End If

                                ' Crea el nuevo contrato
                                RowEC = dtEmpContracts.NewRow
                                RowEC("idEmployee") = intIDEmployee
                                RowEC("idContract") = ColumnsVal(RoboticsExternAccess.EmployeeColumns.Contract)
                                RowEC("BeginDate") = xBeginDate
                                If ColumnsVal(RoboticsExternAccess.EmployeeColumns.EndDate) <> "" Then
                                    RowEC("EndDate") = xEndDate
                                Else
                                    RowEC("EndDate") = xEndDate
                                End If

                                dtEmpContracts.Rows.Add(RowEC)
                            End If
                        End If
                    End If
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployees_SimulateContract")
            End Try

            Return bolRet
        End Function

        Private Function ImportEmployees_GetIDEmployee(ByRef dtEmpUserFields As DataTable, ByVal bolClonar As Boolean, ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer, ByVal ColumnsVal() As String, ByVal EmpresaColumnPosition As Integer, ByRef oEmployeeState As Employee.roEmployeeState) As Integer
            Dim intidEmployee As Integer = -1

            Try
                If Not bolClonar Then
                    Dim bCheckNkif As Boolean = True
                    Dim RowEmployee() As DataRow = Nothing

                    If ColumnsVal(iKeyPositionVal) <> String.Empty Then
                        RowEmployee = dtEmpUserFields.Select("FieldName='" & Me.strImportPrimaryKeyUserField & "' and Value = '" & ColumnsVal(iKeyPositionVal) & "'")
                        If RowEmployee.Length > 0 Then bCheckNkif = False
                    End If

                    If bCheckNkif Then
                        RowEmployee = dtEmpUserFields.Select("FieldName='NIF' and Value = '" & ColumnsVal(iNifPositionVal) & "'")
                    End If

                    If RowEmployee.Length > 0 Then
                        intidEmployee = RowEmployee(0)("IDEmployee")
                    Else
                        intidEmployee = -1
                    End If
                Else
                    ' Debemos obtener un empleado con el DNI + Empresa, para saber si existe o no
                    Dim strSQL As String
                    strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & roTypes.Any2Time(Now).SQLSmallDateTime & " " &
                             "@SELECT# sysrovwAllEmployeeGroups.* " &
                             "FROM sysrovwAllEmployeeGroups, GetAllEmployeeUserFieldValue('EMPRESA', @Date) V , GetAllEmployeeUserFieldValue('NIF', @Date) Z "
                    strSQL &= ", Employees "
                    strSQL &= "WHERE sysrovwAllEmployeeGroups.IDEmployee = V.idEmployee AND sysrovwAllEmployeeGroups.IDEmployee = Z.idEmployee AND " &
                                    "CONVERT(varchar, ISNULL(V.[Value], '')) = '" & ColumnsVal(EmpresaColumnPosition) & "' AND " &
                                    "CONVERT(varchar, ISNULL(Z.[Value], '')) = '" & ColumnsVal(RoboticsExternAccess.EmployeeColumns.DNI) & "' "
                    Dim tbEmployee As DataTable = CreateDataTable(strSQL)

                    If tbEmployee.Rows.Count > 0 Then
                        intidEmployee = tbEmployee.Rows(0).Item("IDEmployee")
                    End If
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployees_GetIDEmployee")
            End Try

            Return intidEmployee
        End Function

        Private Function ImportEmployees_CheckMaxEmployeesNotExceded(ByVal dtEmployeeContracts As DataTable, ByRef strLogEvent As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim iMaxEmployees As Integer = 0
                Dim iActiveEmployees As Integer = 0

                ' Lee los empleados activos a fecha de hoy
                ImportEmployees_GetInfoMaxEmployees(Now, dtEmployeeContracts, iMaxEmployees, iActiveEmployees)
                If iMaxEmployees < 0 Or iActiveEmployees < 0 Then
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.ErrorReadingLicense", "") & vbNewLine
                    Me.State.Result = DataLinkResultEnum.NumMaxEmployeesExceded
                    Exit Try
                End If

                ' Comprueba si se ha excedido la licencia
                Dim NumEmployeesExceded As Integer = iActiveEmployees - iMaxEmployees
                If NumEmployeesExceded > 0 Then
                    Dim strMsg As String = Me.State.Language.Translate("Import.LogEvent.LicenceseExceded", "") & NumEmployeesExceded & vbNewLine
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, strMsg)
                    strLogEvent = Now.ToString & " --> " & strMsg
                    Me.State.Result = DataLinkResultEnum.NumMaxEmployeesExceded
                    Exit Try
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployees_CheckMaxEmployeesNotExceded")
            End Try

            Return bolRet
        End Function

        Private Function ImportEmployees_GetInfoMaxEmployees(ByVal AtDate As Date, ByVal dtEmployeeContracts As DataTable, ByRef iMaxEmployees As Integer, ByRef iActiveEmployees As Integer) As Boolean
            Dim bolRet As Boolean = False
            iMaxEmployees = -1
            iActiveEmployees = -1

            Try
                ' Lee los empleados de la licencia
                Dim LicenseService As New roServerLicense
                iMaxEmployees = LicenseService.FeatureData("VisualTime Server", "MaxEmployees")

                ' Lee los empleados activos a una fecha
                iActiveEmployees = ImportEmployees_GetActivesEmployees(AtDate, dtEmployeeContracts)

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployees_GetInfoMaxEmployees")
            End Try

            Return bolRet
        End Function

        Private Function ImportEmployees_GetActivesEmployees(ByVal AtDate As Date, dtEmployeeContracts As DataTable) As Integer
            Dim iActiveEmployees As Integer = -1

            Try
                ' Selecciona el número de empleados activos
                Dim sSQL As String = "EndDate>#" & Format$(AtDate, "yyyy/MM/dd") & "# and BeginDate<=#" & Format$(AtDate, "yyyy/MM/dd") & "# "
                Dim row() As DataRow = dtEmployeeContracts.Select(sSQL)
                iActiveEmployees = row.Length
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportEmployees_GetActivesEmployees")
            End Try

            Return iActiveEmployees
        End Function

        Public Function ImportEmployeesSAGE200c_ResetContractsAndFixMobilitiesIfNeeded(ByVal sEmployeeNIF As String, ByVal sUniqueIDEmployee As String, lContracts As List(Of RoboticsExternAccess.roDatalinkEmployeeContract)) As Boolean
            Dim oRet As Boolean = True

            Try
                Me.State.Result = DataLinkResultEnum.InvalidContractHistory

                ' 0.- Vemos si es empleado nuevo a partir de NIF o ImportKey
                Dim intIDEmployee As Integer = isEmployeeNew(sUniqueIDEmployee, sEmployeeNIF, New UserFields.roUserFieldState)

                If intIDEmployee > 0 Then
                    ' 1.- Verificar que la lista de contratos es correcta
                    '     No hay solapamientos
                    '     Todos los contratos tienen convenio, y el convenio existe en VT
                    '     Si no es ok, salimos
                    'Comprobamos si hay contratos

                    Dim sSQL As String = String.Empty
                    Dim lExistincgLabAgreess As New List(Of String)
                    Dim noEndDate As New Date(2079, 1, 1)

                    If lContracts Is Nothing OrElse lContracts.Count = 0 Then
                        ' Salgo
                        oRet = False
                        Me.State.Result = DataLinkResultEnum.NoContracts
                        Return oRet
                    End If

                    If lContracts.FindAll(Function(x) x.LabAgreeName Is Nothing OrElse x.LabAgreeName = String.Empty).Count > 0 Then
                        ' Salgo
                        oRet = False
                        Me.State.Result = DataLinkResultEnum.InvalidLabAgree
                        Return oRet
                    End If

                    ' Cargamos convenios existentes
                    sSQL = "@SELECT# Name FROM LabAgree"
                    Dim tLabAgrees As DataTable = CreateDataTable(sSQL, "Convenios")
                    Dim sLabAgreeName As String = String.Empty

                    If tLabAgrees IsNot Nothing AndAlso tLabAgrees.Rows.Count > 0 Then
                        For Each oRow As DataRow In tLabAgrees.Rows
                            sLabAgreeName = roTypes.Any2String(oRow("Name")).ToUpper
                            If Not lExistincgLabAgreess.Contains(sLabAgreeName) Then
                                lExistincgLabAgreess.Add(sLabAgreeName)
                            End If
                        Next
                    Else
                        oRet = False
                        Me.State.Result = DataLinkResultEnum.NoLabAgreesDefined
                        Return oRet
                    End If

                    ' Validación de contratos
                    If oRet Then
                        For n As Integer = 0 To lContracts.Count - 1
                            ' Primer contrato es ok
                            If n = 0 Then
                                If lContracts(0).EndContractDate < lContracts(0).StartContractDate Then
                                    oRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidContractHistory
                                    Return oRet
                                End If
                                If Not lExistincgLabAgreess.Contains(lContracts(n).LabAgreeName.ToUpper) Then
                                    oRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidContractHistory
                                    Return oRet
                                End If
                            End If

                            ' Solapamientos y convenios
                            If (n + 1) < lContracts.Count Then
                                'Comprobamos si no es el ultimo contrato y la fecha inicio del siguiente contrato es anterior a la fecha fin del contrato
                                If lContracts(n).EndContractDate <> noEndDate And lContracts(n).EndContractDate > lContracts(n + 1).StartContractDate Then
                                    oRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidContractHistory
                                    Return oRet
                                End If

                                If Not lExistincgLabAgreess.Contains(lContracts(n).LabAgreeName.ToUpper) Then
                                    Me.State.Result = DataLinkResultEnum.InvalidContractHistory
                                    Return oRet
                                End If
                            End If
                        Next

                        ' 1.- Borrar todos los contratos del empleado con iIDEmmployee
                        sSQL = "@DELETE# EmployeeContracts WHERE IDEmployee = " & intIDEmployee.ToString
                        oRet = ExecuteSql(sSQL)
                    End If

                    If oRet Then
                        ' Guardamos todos los contratos nuevos TODO: Verificar cómo llega aquí la fecha fin de contrato desde el conector. Si llega a nulo, o nothing, o minvalue, se pone a 1 de 1 de 2079
                        For Each oContract As RoboticsExternAccess.roDatalinkEmployeeContract In lContracts
                            sSQL = "@INSERT# INTO EmployeeContracts (IDEmployee,IDContract,BeginDate,EndDate,IDLabAgree) @SELECT# " & intIDEmployee.ToString & ",'" & oContract.IDContract & "'," & roTypes.Any2Time(oContract.StartContractDate).SQLSmallDateTime & "," & If(oContract.EndContractDate = Date.MinValue, "'20790101'", roTypes.Any2Time(oContract.EndContractDate).SQLSmallDateTime) & ", Id FROM LabAgree WHERE UPPER(Name) = '" & oContract.LabAgreeName.ToUpper & "'"
                            oRet = ExecuteSql(sSQL)
                            If Not oRet Then
                                Me.State.Result = DataLinkResultEnum.InvalidContractHistory
                                Exit For
                            End If
                        Next

                        If oRet Then
                            ' Movilidades
                            Dim lMovilityList As New List(Of RoboticsExternAccess.roDatalinkEmployeeMovility)
                            Dim dFirstContractDate As Date = lContracts.First.StartContractDate
                            Dim bIsMovilityInContract As Boolean = False

                            'Sacamos la lista de movilidades
                            sSQL = "@SELECT# IDGroup, BeginDate, EndDate, IsTransfer from EmployeeGroups " &
                                    "WHERE IDEmployee =  " & intIDEmployee.ToString & " ORDER BY BeginDate ASC"

                            Dim dtMovilities As DataTable = New DataTable
                            Dim oMovility As roDatalinkEmployeeMovility
                            dtMovilities = CreateDataTable(sSQL, "Movilities")

                            If dtMovilities IsNot Nothing AndAlso dtMovilities.Rows.Count > 0 Then
                                For Each movility As DataRow In dtMovilities.Rows
                                    oMovility = New roDatalinkEmployeeMovility
                                    oMovility.StartDate = movility("BeginDate")
                                    oMovility.EndDate = movility("EndDate")
                                    oMovility.IDGroup = movility("IDGroup")
                                    oMovility.IsTransfer = movility("IsTransfer")
                                    lMovilityList.Add(oMovility)
                                Next
                            Else
                                oRet = False
                            End If

                            ' 2.- Ajustar movilidades existentes a los contratos si fuese necesario
                            '     La primera movilidad debe conincidir exactamente con la fecha de inicio del primer contratox
                            '     Coincide la primera movilidad con el inicio de primer contrato?
                            Dim dFirstMovility As Date = lMovilityList.ToList.First.StartDate
                            If dFirstContractDate <> dFirstMovility Then
                                lMovilityList.First.StartDate = dFirstContractDate
                            End If

                            Dim iPos As Integer = 0
                            'Bucle para comprobar si la movilidad esta dentro de algun contrato
                            For Each dMovility As roDatalinkEmployeeMovility In lMovilityList
                                If lContracts.FindAll(Function(x) x.StartContractDate <= dMovility.StartDate AndAlso x.EndContractDate >= dMovility.StartDate).Count = 0 Then
                                    ' Ajustamos
                                    ' 2.1.- Toda fecha de movilidad debe estar dentro de un periodo de contato
                                    ' 2.1.1- Si no lo está, la paso al primer día del siguiente contrato si lo hay. Si no, al último de último contrato
                                    Dim dNextStartDate As Date = Date.MinValue
                                    Dim oNextContract As RoboticsExternAccess.roDatalinkEmployeeContract
                                    Dim oPreviousContract As RoboticsExternAccess.roDatalinkEmployeeContract

                                    oNextContract = lContracts.FirstOrDefault(Function(x) x.StartContractDate > dMovility.StartDate)
                                    If oNextContract IsNot Nothing Then
                                        dMovility.StartDate = oNextContract.StartContractDate
                                        If iPos > 0 Then
                                            lMovilityList.Item(iPos - 1).EndDate = dMovility.StartDate.AddDays(-1)
                                        End If
                                    Else
                                        oPreviousContract = lContracts.Last(Function(x) x.EndContractDate < dMovility.StartDate)
                                        If oPreviousContract IsNot Nothing Then
                                            dMovility.StartDate = oPreviousContract.EndContractDate
                                            If iPos > 0 Then
                                                lMovilityList.Item(iPos - 1).EndDate = dMovility.StartDate.AddDays(-1)
                                            End If
                                        End If
                                    End If
                                End If
                                iPos += 1
                            Next

                            sSQL = "@DELETE# EmployeeGroups WHERE IDEmployee = " & intIDEmployee.ToString
                            oRet = ExecuteSql(sSQL)

                            For Each dMovility In lMovilityList
                                sSQL = "@INSERT# INTO EmployeeGroups (IDEmployee,IDGroup,BeginDate,EndDate,IsTransfer)" &
                                       "VALUES (" & intIDEmployee.ToString & "," & dMovility.IDGroup.ToString & "," & roTypes.Any2Time(dMovility.StartDate).SQLSmallDateTime & "," & roTypes.Any2Time(dMovility.EndDate).SQLDateTime & ",0)"
                                oRet = ExecuteSql(sSQL)
                            Next
                        End If

                        oRet = True
                    End If
                Else
                    ' Es un empledo nuevo. Lo daré de alta con el último contrato de la lista de histórico
                    ' Si vuelve a venir, cuando ya exista, y con el histórico de contratos, entonces se dará de alta el histórico ...
                    oRet = True
                    Me.State.Result = DataLinkResultEnum.NoError
                End If
            Catch ex As Exception
                oRet = False
                Me.State.Result = DataLinkResultEnum.Exception
            End Try

            Return oRet

        End Function

        'Private Function GetChildEmployeeGroups(ByVal row As UsersAdmin.EmployeeGroupsDataSet.GroupsRow, ByVal oState As roSecurityState) As Generic.List(Of UsersAdmin.EmployeeGroup)
        '    Dim lstEmployeeGroups As New Generic.List(Of UsersAdmin.EmployeeGroup)
        '    Dim lstChilds As Generic.List(Of UsersAdmin.EmployeeGroup)
        '    For Each childRow As UsersAdmin.EmployeeGroupsDataSet.GroupsRow In row.GetGroupsRows()
        '        lstEmployeeGroups.Add(New UsersAdmin.EmployeeGroup(childRow, oState))
        '        lstChilds = Me.GetChildEmployeeGroups(childRow, oState)
        '        For Each oEmployeeGroupChild As UsersAdmin.EmployeeGroup In lstChilds
        '            lstEmployeeGroups.Add(oEmployeeGroupChild)
        '        Next
        '    Next
        '    Return lstEmployeeGroups
        'End Function


#End Region

    End Class

End Namespace