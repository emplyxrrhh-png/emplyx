Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink


    Public Class roCostCentersImport
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


#Region "26 - IMPORTAR CENTROS DE COSTE"

        Public Function ImportBusinessCenter(ByVal idImport As Integer) As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(BusinessCenterEnum)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(BusinessCenterEnum)).Length - 1) As String

                    Dim intNew As Integer = 0
                    Dim intUpd As Integer = 0
                    Dim dtCCUsrFields As DataTable = CreateDataTable("@SELECT# id, Name from sysroFieldsBusinessCenters", "CCUsrFields")

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer
                        Dim intEndLine As Integer
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        Dim bolIsNew As Boolean = False

                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then
                            Dim InvalidColumn As String = ""

                            'Contar número de columnas
                            Dim intColumns As Integer = Me.ValidarColumnasBusinessCenter(ColumnsPos, dtCCUsrFields, InvalidColumn)
                            If intColumns > 0 Then

                                Try
                                    bolRet = True

                                    ' Centro de coste
                                    Dim oCenterState = New BusinessCenter.roBusinessCenterState()
                                    roBusinessState.CopyTo(Me.State, oCenterState)

                                    Dim oCenter As New BusinessCenter.roBusinessCenter()

                                    ' Recorremos toda la hoja excel
                                    For intRow As Integer = intBeginLine To intEndLine
                                        bolRet = False

                                        'Obtenemos los datos a partir del excel
                                        If Me.GetDataExcelBusinessCenter(intRow, intColumns, ColumnsPos, ColumnsVal) Then
                                            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                                            Try
                                                ' Carga el centro
                                                oCenter = New BusinessCenter.roBusinessCenter()
                                                bolRet = oCenter.GetBusinessCenterByName(ColumnsVal(BusinessCenterEnum.Name))
                                                If bolRet = True Then
                                                    ' Comprueba si existe el centro
                                                    If oCenter.ID = -1 Then
                                                        oCenter.Name = ColumnsVal(BusinessCenterEnum.Name)
                                                        bolIsNew = True
                                                    Else
                                                        bolIsNew = False

                                                        ' Borra todos los empleados cedidos
                                                        bolRet = oCenter.DeleteAllEmployeesAssigned()
                                                    End If

                                                    ' Asigna valores
                                                    If ColumnsVal(BusinessCenterEnum.Status) <> "" Then oCenter.Status = Any2Double(ColumnsVal(BusinessCenterEnum.Status))
                                                    If ColumnsVal(BusinessCenterEnum.USR_Field1) <> "" Then oCenter.Field1 = ColumnsVal(BusinessCenterEnum.USR_Field1)
                                                    If ColumnsVal(BusinessCenterEnum.USR_Field2) <> "" Then oCenter.Field2 = ColumnsVal(BusinessCenterEnum.USR_Field2)
                                                    If ColumnsVal(BusinessCenterEnum.USR_Field3) <> "" Then oCenter.Field3 = ColumnsVal(BusinessCenterEnum.USR_Field3)
                                                    If ColumnsVal(BusinessCenterEnum.USR_Field4) <> "" Then oCenter.Field4 = ColumnsVal(BusinessCenterEnum.USR_Field4)
                                                    If ColumnsVal(BusinessCenterEnum.USR_Field5) <> "" Then oCenter.Field5 = ColumnsVal(BusinessCenterEnum.USR_Field5)

                                                    ' Trata Zonas asignadas
                                                    If ColumnsVal(BusinessCenterEnum.ZonesAreControlled) <> "" Then oCenter.AuthorizationMode = Any2Double(ColumnsVal(BusinessCenterEnum.ZonesAreControlled))
                                                    If oCenter.AuthorizationMode = 1 And ColumnsVal(BusinessCenterEnum.ZonesAssigned) <> "" Then
                                                        If BusinessCenterValidateZones(ColumnsVal(BusinessCenterEnum.ZonesAssigned), intRow, msgLog, oCenter.Zones) = False Then
                                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorReadingZoneOnRegister", "") & " " & intRow & vbNewLine
                                                        End If
                                                    End If

                                                    ' Trata Empleados asignados
                                                    If Any2Double(ColumnsVal(BusinessCenterEnum.EmployesAreControlled)) = 1 And ColumnsVal(BusinessCenterEnum.EmployeesAssigned) <> "" Then
                                                        If BusinessCenterValidateEmployees(oCenter.ID, ColumnsVal(BusinessCenterEnum.EmployeesAssigned), intRow, msgLog) = False Then
                                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorReadingEmployeeOnRegister", "") & " " & intRow & vbNewLine
                                                        End If
                                                    End If

                                                    ' Graba los datos
                                                    If bolRet = True Then bolRet = oCenter.Save()
                                                    If bolRet Then
                                                        Dim oLicense As New roServerLicense
                                                        Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")
                                                        If bolIsInstalledBots Then
                                                            Try
                                                                Dim oBotState As New roBotState(-1)
                                                                Dim oBotManager As New roBotManager(oBotState)
                                                                Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String) From {
                                                                    {BotRuleParameterEnum.DestinationCostCenter, oCenter.ID.ToString}
                                                                }
                                                                oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyCenterCostRole, _oParameters)
                                                            Catch ex As Exception
                                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CopyCenterCostrules::Creating CostCenter " & oCenter.Name & ": Error executing bot", ex)
                                                            End Try
                                                        End If
                                                    End If
                                                Else
                                                    Me.State.Result = DataLinkResultEnum.Exception
                                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorReadingBusinessCenterOnRegister", "") & " " & intRow & vbNewLine
                                                End If
                                            Catch ex As Exception
                                                Me.State.Result = DataLinkResultEnum.Exception
                                                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportBusinessCenter")
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                                bolRet = False
                                            Finally

                                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

                                                If bolIsNew = True Then
                                                    intNew += 1
                                                Else
                                                    intUpd += 1
                                                End If
                                            End Try
                                        Else
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RequiredFieldNotInformed", "") & " " & intRow & vbNewLine
                                        End If
                                    Next

                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NewBusinessCenter", "") & ": " & intNew.ToString & Environment.NewLine
                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UpdatedBusinessCenter", "") & ": " & intUpd.ToString & Environment.NewLine

                                    ' Si se hizo algún cambio llamo a Broadcaster
                                    If intUpd Or intNew Then Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                                Catch ex As Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportBusinessCenterExcel")
                                    Me.State.Result = DataLinkResultEnum.Exception
                                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                    bolRet = False
                                End Try
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
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportBusinessCenter")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function ValidarColumnasBusinessCenter(ByRef ColumnsPos() As Integer, ByVal dtCCUsrFields As DataTable, ByRef InvalidColumn As String) As Integer
            Dim intNumColumnas As Integer = -1
            Dim intCol As Integer = 0
            Dim bolIsValid As Boolean = True
            Dim Column As String

            InvalidColumn = ""

            ' Inicializa columnas
            For intCol = 0 To ColumnsPos.Length - 1
                ColumnsPos(intCol) = -1
            Next

            ' Descompone columnas
            intCol = 0
            Column = GetCellValueWithoutFormat(0, intCol).ToUpper
            Do While Column <> "" And bolIsValid
                Select Case Column.ToUpper
                    Case "NOMBRE" : ColumnsPos(BusinessCenterEnum.Name) = intCol
                    Case "ESTADO" : ColumnsPos(BusinessCenterEnum.Status) = intCol
                    Case "CONTROL_ZONAS" : ColumnsPos(BusinessCenterEnum.ZonesAreControlled) = intCol
                    Case "ZONAS" : ColumnsPos(BusinessCenterEnum.ZonesAssigned) = intCol
                    Case "CONTROL_CESIONES" : ColumnsPos(BusinessCenterEnum.EmployesAreControlled) = intCol
                    Case "CESIONES" : ColumnsPos(BusinessCenterEnum.EmployeesAssigned) = intCol
                    Case Else
                        If Column.StartsWith("USR_") Then
                            ' Determina el id del campo de la ficha
                            Dim row() As DataRow = dtCCUsrFields.Select("Name = '" & Column.Substring(4) & "'")
                            If row.Length > 0 Then
                                ColumnsPos(BusinessCenterEnum.USR_Field1 + row(0)("id") - 1) = intCol
                            Else
                                bolIsValid = False
                            End If
                        Else
                            bolIsValid = False
                        End If
                End Select

                If bolIsValid Then
                    intCol += 1
                    Column = GetCellValueWithoutFormat(0, intCol)
                Else
                    InvalidColumn = Column
                End If
            Loop

            If bolIsValid Then
                If Not (ColumnsPos(BusinessCenterEnum.Name)) = -1 Then
                    intNumColumnas = intCol
                Else
                    InvalidColumn = "Required Fields"
                End If
            End If

            Return intNumColumnas
        End Function

        Private Function GetDataExcelBusinessCenter(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Count - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                If ColumnsVal(BusinessCenterEnum.Name).Length > 0 Then
                    bolRet = True
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelBusinessCenter")
            End Try

            Return bolRet

        End Function

        Private Function BusinessCenterValidateZones(ByVal ZonesAssigned As String, ByVal IntRow As Integer, ByRef MsgLog As String, ByRef LstZones As Generic.List(Of BusinessCenter.roBusinessCenterZone)) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Trata Zonas asignadas
                If ZonesAssigned <> "" Then
                    Dim oZone As New Zone.roZone()
                    Dim oCenterZone As New BusinessCenter.roBusinessCenterZone()
                    Dim oCenterState = New BusinessCenter.roBusinessCenterState()

                    Dim str() As String = ZonesAssigned.Split(";")
                    LstZones = New Generic.List(Of BusinessCenter.roBusinessCenterZone)

                    ' Comprueba si existe la zona
                    For i As Integer = 0 To str.Length - 1
                        oZone = New Zone.roZone()
                        If oZone.GetZoneByName(str(i)) = False Then
                            Me.State.Result = DataLinkResultEnum.Exception
                            MsgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorReadingZoneOnRegister", "") & " " & IntRow & vbNewLine
                            Exit For
                        End If

                        If oZone.ID <> -1 Then
                            oCenterZone = New BusinessCenter.roBusinessCenterZone()
                            oCenterZone.ID = oZone.ID
                            oCenterZone.Name = oZone.Name
                            LstZones.Add(oCenterZone)
                        Else
                            oCenterState.Result = DTOs.BusinessCenterResultEnum.NonExistentZone
                            Me.State.Language.ClearUserTokens()
                            Me.State.Language.AddUserToken("'" & str(i) & "'")
                            MsgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NonExistentZoneOnregister", "") & " " & IntRow & vbNewLine
                        End If
                    Next
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::BusinessCenterValidateZones")

            End Try

            Return bolRet
        End Function

        Private Function BusinessCenterValidateEmployees(ByVal idCenter As Integer, ByVal EmployeesAssigned As String, ByVal intRow As Integer, ByRef msgLog As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim str() As String = EmployeesAssigned.Split(";")
                Dim emp() As String
                Dim oEmployeeState As Employee.roEmployeeState
                Dim oCenterZone As New BusinessCenter.roBusinessCenterZone()
                Dim oCenterState = New BusinessCenter.roBusinessCenterState()

                ' Tabla de EmployeeCenters
                Dim tbEmployees As New DataTable("EmployeeCenters")
                tbEmployees.Columns.Add("idEmployee", GetType(Integer))
                tbEmployees.Columns.Add("idCenter", GetType(Integer))
                tbEmployees.Columns.Add("BeginDate", GetType(DateTime))
                tbEmployees.Columns.Add("EndDate", GetType(DateTime))

                Dim dsEmployees As New DataSet("EmployeeCentersDS")
                dsEmployees.Tables.Add(tbEmployees)

                ' Comprueba si existe el empleado
                For i As Integer = 0 To str.Length - 1
                    emp = str(i).Split("#")

                    If emp.Length <> 3 Then
                        ' Formato incorrecto
                        oCenterState.Result = DTOs.BusinessCenterResultEnum.InvalidFormat
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnregister", "") & " " & intRow & vbNewLine
                    Else
                        Dim tbEmpUserField As New DataTable

                        ' Busca el empleado
                        oEmployeeState = New Employee.roEmployeeState
                        tbEmpUserField = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", emp(0), Now, New UserFields.roUserFieldState)

                        If tbEmpUserField.Rows.Count > 0 Then
                            ' Comprueba las fechas
                            If Not IsDate(emp(1)) Or Not IsDate(emp(2)) Then
                                ' Fechas incorrectas
                                oCenterState.Result = DTOs.BusinessCenterResultEnum.InvalidDate
                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidDateOnregister", "") & " " & intRow & vbNewLine
                            Else
                                If CDate(emp(1)) > CDate(emp(2)) Then
                                    ' Fechas incorrectas
                                    oCenterState.Result = DTOs.BusinessCenterResultEnum.InvalidDate
                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidDateOnregister", "") & " " & intRow & vbNewLine
                                Else
                                    ' Añade el empleado cedido
                                    Dim oState As New BusinessCenter.roBusinessCenterState

                                    tbEmployees.Rows.Clear()
                                    tbEmployees.Rows.Add({tbEmpUserField.Rows(0)("idemployee"), idCenter, emp(1), emp(2)})
                                    If BusinessCenter.roBusinessCenter.SaveEmployeeCenters(tbEmpUserField.Rows(0)("idemployee"), dsEmployees, oState, , False, False) = False Then
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorSavingEmployeesOnRegister", "") & " " & intRow & vbNewLine
                                    End If
                                End If
                            End If
                        Else
                            ' No existe el empleado
                            oCenterState.Result = DTOs.BusinessCenterResultEnum.NonExistentEmployee
                            Me.State.Language.ClearUserTokens()
                            Me.State.Language.AddUserToken("'" & emp(0) & "'")
                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NonExistentEmployeeOnregister", "") & " " & intRow & vbNewLine
                        End If
                    End If
                Next

                bolRet = True
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::BusinessCenterValidateEmployees")
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace