Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataLink

    Public Class roCausesImport
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

#Region "23 - IMPORTAR JUSTIFICACIONES EXCEL"

        Public Function ImportDailyCausesExcel() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then

                    'Obtenemos la fecha de congelación
                    Dim freezeDate As Date = New Date(1900, 1, 1)

                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(DailyCauseExcelColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(DailyCauseExcelColumns)).Length - 1) As String

                    Dim intNewDailyCauses As Integer = 0
                    Dim ContractAnt As String = ""

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer
                        Dim intEndLine As Integer
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then
                            Dim InvalidColumn As String = ""

                            'Contar número de columnas
                            Dim FieldKey As String = ""
                            Dim intColumns As Integer = Me.ValidarColumnasDailyCauses(ColumnsPos, InvalidColumn, FieldKey)
                            If intColumns > 0 Then

                                Try
                                    bolRet = True

                                    Dim oContractState = New Contract.roContractState()
                                    roBusinessState.CopyTo(Me.State, oContractState)

                                    Dim oDailyCauseState = New Cause.roCauseState()
                                    roBusinessState.CopyTo(Me.State, oDailyCauseState)

                                    Dim bolNotifyChanges As Boolean = False
                                    Dim strEmployees As String = ""
                                    Dim FieldName As String = ""

                                    ' Si la clave es un campo de la ficha , obtenemos el nombre del campo
                                    If FieldKey = "UserField" Then
                                        If GetCellValue(0, ColumnsPos(DailyCauseExcelColumns.Contract)).Length > 4 Then
                                            FieldName = GetCellValue(0, ColumnsPos(DailyCauseExcelColumns.Contract)).Substring(4)
                                        End If
                                    End If

                                    ' Recorremos toda la hoja excel
                                    For intRow As Integer = intBeginLine To intEndLine
                                        bolRet = False
                                        'Obtenemos los datos del puesto a partir del excel
                                        If Me.GetDataExcelDailyCauses(intRow, intColumns, ColumnsPos, ColumnsVal) Then
                                            Dim valueDate As Date = roTypes.Any2DateTime(ColumnsVal(DailyCauseExcelColumns.CauseDate))
                                            Dim valueContract As String = ColumnsVal(DailyCauseExcelColumns.Contract)

                                            'Get contract
                                            Dim oContract As Contract.roContract = Nothing

                                            ' Si la clave es un campo de la ficha , primero obtenemos el id del usuario y luego el contrato
                                            If FieldKey = "UserField" AndAlso FieldName.Length > 0 Then
                                                Dim tbEmpUserField As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(FieldName, valueContract, Now, New UserFields.roUserFieldState)
                                                If tbEmpUserField IsNot Nothing AndAlso tbEmpUserField.Rows.Count = 1 Then
                                                    Dim idEmployee = tbEmpUserField.Rows(0)("idemployee")
                                                    oContract = Contract.roContract.GetContractInDate(idEmployee, valueDate, oContractState, False)
                                                End If
                                            Else
                                                ' Sino, busca el empleado por el contrato
                                                oContract = New Contract.roContract(oContractState, valueContract)
                                                oContract.Load()
                                            End If
                                            bolRet = CreateDailyCause(msgLog, freezeDate, valueDate, ColumnsVal, oContract, intNewDailyCauses, oContractState, oDailyCauseState, bolNotifyChanges, FieldName, intRow)
                                        End If
                                    Next

                                    If bolNotifyChanges Then
                                        Extensions.roConnector.InitTask(TasksType.DAILYCAUSES)
                                    End If

                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewDailyCauses", "") & ": " & intNewDailyCauses.ToString & Environment.NewLine
                                Catch ex As Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportDailyCausesExcel")
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
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportDailyCausesExcel")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Protected Friend Function CreateDailyCause(ByRef msgLog As String, ByRef freezeDate As Date, valueDate As Date, ColumnsVal() As String, oContract As Contract.roContract, ByRef intNewDailyCauses As Integer, ByRef oContractState As roContractState, oDailyCauseState As roCauseState, ByRef bolNotifyChanges As Boolean, FieldName As String, intRow As Integer)
            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
            Dim bolRet = False
            Try
                If valueDate < DateTime.Now.Date.AddDays(1) Then
                    Dim idEmployee As Integer = 0
                    Dim oEmployeeState As New Employee.roEmployeeState

                    Dim loadedContract = oContract IsNot Nothing

                    If loadedContract Then
                        freezeDate = roBusinessSupport.GetEmployeeLockDatetoApply(oContract.IDEmployee, False, Me.State)
                    End If
                    If loadedContract And freezeDate < valueDate Then

                        ' Si existe el contrato.
                        If loadedContract AndAlso oContract.BeginDate <= valueDate AndAlso oContract.EndDate >= valueDate Then

                            Dim oCauseID As Integer = Cause.roCause.GetCauseByShortName(roTypes.Any2String(ColumnsVal(DailyCauseExcelColumns.Cause)), oDailyCauseState)

                            If oCauseID > 0 Then
                                Dim oDailyCause As New Cause.roDailyCause()
                                oDailyCause.LoadWithParams(oContract.IDEmployee, valueDate, oCauseID, True)

                                Dim strMinutes As String = roTypes.Any2String(ColumnsVal(DailyCauseExcelColumns.Value))
                                If Not strMinutes.Contains("-") AndAlso IsNumeric(strMinutes) Then

                                    oDailyCause.Value = roTypes.Any2Double(strMinutes) / 60
                                    bolRet = oDailyCause.Save(, False)

                                    If Not bolRet Then
                                        Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidRegister", "") & " " & intRow & vbNewLine
                                    Else
                                        intNewDailyCauses = intNewDailyCauses + 1
                                        bolNotifyChanges = True

                                        Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status=65, [GUID] = '' WHERE IDEmployee = " & oContract.IDEmployee & " AND Date=" & roTypes.Any2Time(valueDate).SQLSmallDateTime
                                        If Not ExecuteSql(strSQL) Then
                                            Me.State.Result = DataLinkResultEnum.CalculateError
                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.DailyProcessStartError", "") & " " & vbNewLine
                                        End If

                                    End If
                                Else
                                    Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidValueOnRegister", "") & " " & intRow & vbNewLine
                                End If
                            Else
                                Me.State.Result = DataLinkResultEnum.InvalidCause
                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidCauseOnRegister", "") & " " & intRow & vbNewLine
                            End If
                        Else
                            Me.State.Result = DataLinkResultEnum.InvalidContract
                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidContractOnRegister", "") & " " & intRow & vbNewLine
                        End If
                    Else
                        If Not loadedContract Then
                            Me.State.Result = DataLinkResultEnum.NoContracts
                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoSuchContract", "") & " " & intRow & vbNewLine
                        Else
                            Me.State.Result = DataLinkResultEnum.FreezeDateException
                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.FreezeDateOnRegister", "") & " " & intRow & vbNewLine
                        End If
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.FutureDate
                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.FutureDateOnRegister", "") & " " & intRow & vbNewLine
                End If
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportDailyCausesExcel")
                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Protected Friend Function DeleteDailyCause(ByRef msgLog As String, ByRef freezeDate As Date, valueDate As Date, ColumnsVal() As String, oContract As Contract.roContract, ByRef intNewDailyCauses As Integer, ByRef oContractState As roContractState, oDailyCauseState As roCauseState, ByRef bolNotifyChanges As Boolean, FieldName As String, intRow As Integer)
            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
            Dim bolRet = False
            Try
                If valueDate < DateTime.Now.Date.AddDays(1) Then
                    Dim idEmployee As Integer = 0
                    Dim oEmployeeState As New Employee.roEmployeeState

                    Dim loadedContract = oContract IsNot Nothing

                    If loadedContract Then
                        freezeDate = roBusinessSupport.GetEmployeeLockDatetoApply(oContract.IDEmployee, False, Me.State)
                    End If
                    If loadedContract And freezeDate < valueDate Then

                        ' Si existe el contrato.
                        If loadedContract AndAlso oContract.BeginDate <= valueDate AndAlso oContract.EndDate >= valueDate Then

                            Dim oCauseID As Integer = Cause.roCause.GetCauseByShortName(roTypes.Any2String(ColumnsVal(DailyCauseExcelColumns.Cause)), oDailyCauseState)

                            If oCauseID > 0 Then
                                Dim oDailyCause As New Cause.roDailyCause()
                                Dim existsManualCause = oDailyCause.LoadWithParams(oContract.IDEmployee, valueDate, oCauseID, True, False)

                                If Not existsManualCause Then
                                    Me.State.Result = DataLinkResultEnum.ManualCauseNotExists
                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.ManualCauseNotExists", "") & " " & vbNewLine
                                Else
                                    bolRet = oDailyCause.Delete()
                                    If Not bolRet Then
                                        Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidRegister", "") & " " & intRow & vbNewLine
                                    Else
                                        intNewDailyCauses = intNewDailyCauses + 1
                                        bolNotifyChanges = True

                                        Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status=65, [GUID] = '' WHERE IDEmployee = " & oContract.IDEmployee & " AND Date=" & roTypes.Any2Time(valueDate).SQLSmallDateTime
                                        If Not ExecuteSql(strSQL) Then
                                            Me.State.Result = DataLinkResultEnum.CalculateError
                                            msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.DailyProcessStartError", "") & " " & vbNewLine
                                        End If

                                    End If
                                End If
                            Else
                                Me.State.Result = DataLinkResultEnum.InvalidContract
                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidContractOnRegister", "") & " " & intRow & vbNewLine
                            End If
                        Else
                            If Not loadedContract Then
                                Me.State.Result = DataLinkResultEnum.NoContracts
                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoSuchContract", "") & " " & intRow & vbNewLine
                            Else
                                Me.State.Result = DataLinkResultEnum.FreezeDateException
                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.FreezeDateOnRegister", "") & " " & intRow & vbNewLine
                            End If
                        End If
                    Else
                        Me.State.Result = DataLinkResultEnum.FutureDate
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.FutureDateOnRegister", "") & " " & intRow & vbNewLine
                    End If
                End If
            Catch ex As Exception
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportDailyCausesExcel")
                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Private Function ValidarColumnasDailyCauses(ByRef ColumnsPos() As Integer, ByRef InvalidColumn As String, ByRef FieldKey As String) As Integer
            Dim intNumColumnas As Integer = -1
            Dim intCol As Integer = 0
            Dim bolIsValid As Boolean = True
            Dim Columna As String

            InvalidColumn = ""

            ' Inicializa columnas
            For intCol = 0 To ColumnsPos.Length - 1
                ColumnsPos(intCol) = -1
            Next

            ' Descompone columnas
            intCol = 0
            Columna = GetCellValueWithoutFormat(0, intCol).ToUpper
            FieldKey = "Contract"
            Do While Columna <> "" And bolIsValid
                Select Case Columna.ToUpper
                    Case "FECHA" : ColumnsPos(DailyCauseExcelColumns.CauseDate) = intCol
                    Case "CONTRATO" : ColumnsPos(DailyCauseExcelColumns.Contract) = intCol
                    Case "JUSTIFICACION" : ColumnsPos(DailyCauseExcelColumns.Cause) = intCol
                    Case "VALOR" : ColumnsPos(DailyCauseExcelColumns.Value) = intCol
                    Case Else
                        If Columna.StartsWith("USR_") Then
                            ColumnsPos(DailyCauseExcelColumns.Contract) = intCol
                            FieldKey = "UserField"
                        Else
                            bolIsValid = False
                        End If
                End Select

                If bolIsValid Then
                    intCol += 1
                    Columna = GetCellValueWithoutFormat(0, intCol)
                Else
                    InvalidColumn = Columna
                End If
            Loop

            If bolIsValid Then
                If Not (ColumnsPos(DailyCausesColumns.Contract) = -1 Or ColumnsPos(DailyCauseExcelColumns.CauseDate) = -1 Or
                       ColumnsPos(DailyCauseExcelColumns.Cause) = -1 Or ColumnsPos(DailyCauseExcelColumns.Value)) = -1 Then
                    intNumColumnas = intCol
                Else
                    InvalidColumn = "Required Fields"
                End If
            End If

            Return intNumColumnas
        End Function

        Private Function GetDataExcelDailyCauses(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Count - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelDailyCauses")
            End Try

            Return bolRet

        End Function

#End Region

    End Class
End Namespace