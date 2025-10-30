Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataLink

    Public Class roTasksImport
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

#Region "25- IMPORTAR TAREAS"

        Public Function ImportTasksExcel() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(TaskColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(TaskColumns)).Length - 1) As String

                    ' Definimos variables para guardar los logs
                    Dim intNewTasks As Integer = 0
                    Dim intUpdateTasks As Integer = 0

                    ' Inicio de la importación
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
                            Dim intColumns As Integer = Me.ValidarColumnasTasks(ColumnsPos, InvalidColumn)

                            If intColumns > 0 Then
                                Try
                                    Dim intIDTask As Integer = 0
                                    bolRet = True

                                    Dim oTaskState As New Task.roTaskState
                                    roBusinessState.CopyTo(Me.State, oTaskState)

                                    ' Recorremos toda la hoja excel
                                    For intRow As Integer = intBeginLine To intEndLine

                                        'Obtenemos los datos de la tarea
                                        If Me.GetDataExcelTasks(intRow, intColumns, ColumnsPos, ColumnsVal) Then

                                            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                                            Try
                                                ' Buscamos la tarea
                                                Dim strSQL = "@SELECT# ID FROM Tasks WHERE Name ='" & ColumnsVal(TaskColumns.Task).Replace("'", "''") & "' "
                                                If ColumnsVal(TaskColumns.Project).Length > 0 Then
                                                    strSQL = strSQL & " AND Project='" & ColumnsVal(TaskColumns.Project).Replace("'", "''") & "' "
                                                End If

                                                intIDTask = roTypes.Any2Integer(ExecuteScalar(strSQL))
                                                If intIDTask = 0 Then
                                                    ' Nueva tarea
                                                    bolRet = Me.UpdateTask(True, ColumnsVal, oTaskState)
                                                    If bolRet Then intNewTasks = intNewTasks + 1
                                                Else
                                                    'Tarea existente
                                                    bolRet = Me.UpdateTask(False, ColumnsVal, oTaskState, intIDTask)
                                                    If bolRet Then intUpdateTasks = intUpdateTasks + 1
                                                End If

                                                If bolRet = False Then
                                                    Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow & " (" & oTaskState.ErrorText & ")" & vbNewLine
                                                End If
                                            Catch ex As Exception
                                                Me.State.Result = DataLinkResultEnum.Exception
                                                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportTask")
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                                bolRet = False
                                            Finally
                                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                            End Try
                                        Else
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                            msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & vbNewLine
                                        End If
                                    Next

                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.NewTasks", "") & " " & intNewTasks.ToString & Environment.NewLine
                                    strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.UpdateTasks", "") & " " & intUpdateTasks.ToString & vbNewLine

                                    If bolRet Then
                                        ' Auditamos importación tareas
                                        Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                                        Me.State.AddAuditParameter(tbParameters, "{ImportTasksType}", "Excel", "", 1)
                                        Me.State.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDataLinkImportTask, "", tbParameters, -1) '', oTrans.Connection)
                                    End If
                                Catch ex As Exception
                                    Me.State.Result = DataLinkResultEnum.Exception
                                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportTasksExcel")
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
                    ' Fichero Excel incorrecto
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportTasksExcel")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function ValidarColumnasTasks(ByRef ColumnsPos() As Integer, ByRef InvalidColumn As String) As Integer
            Dim intNumColumnas As Integer = -1
            Dim intCol As Integer
            Dim bolIsValid As Boolean = True
            Dim Columna As String

            InvalidColumn = ""

            ' Inicializa variables
            For intCol = 0 To ColumnsPos.Length - 1
                ColumnsPos(intCol) = -1
            Next

            ' Comprueba columnas
            intCol = 0
            Columna = GetCellValueWithoutFormat(0, intCol).ToUpper
            Do While Columna <> "" And bolIsValid
                Select Case Columna
                    Case "CENTRO" : ColumnsPos(TaskColumns.Center) = intCol
                    Case "PROYECTO" : ColumnsPos(TaskColumns.Project) = intCol
                    Case "TAREA" : ColumnsPos(TaskColumns.Task) = intCol
                    Case "DESCRIPCION" : ColumnsPos(TaskColumns.Description) = intCol
                    Case "ABREVIATURA" : ColumnsPos(TaskColumns.ShortName) = intCol
                    Case "ESTADO" : ColumnsPos(TaskColumns.Status) = intCol
                    Case "PRIORIDAD" : ColumnsPos(TaskColumns.Priority) = intCol
                    Case "TAGS" : ColumnsPos(TaskColumns.Tags) = intCol
                    Case "CODIGO_UNICO" : ColumnsPos(TaskColumns.BarCode) = intCol
                    Case "INICIO_PREVISTO" : ColumnsPos(TaskColumns.BeginDate) = intCol
                    Case "FIN_PREVISTO" : ColumnsPos(TaskColumns.EndDate) = intCol
                    Case "DURACION" : ColumnsPos(TaskColumns.Duration) = intCol
                    Case "TIPO_CIERRE" : ColumnsPos(TaskColumns.CloseType) = intCol
                    Case "VALOR_CIERRE" : ColumnsPos(TaskColumns.CloseValue) = intCol
                    Case "TIPO_ACTIVACION" : ColumnsPos(TaskColumns.ActivationType) = intCol
                    Case "VALOR_ACTIVACION" : ColumnsPos(TaskColumns.ActivationValue) = intCol
                    Case "TIPO_COLABORACION" : ColumnsPos(TaskColumns.ColaborationType) = intCol
                    Case "TIPO_AUTORIZADOS" : ColumnsPos(TaskColumns.AutorizedType) = intCol
                    Case "VALOR_AUTORIZADOS" : ColumnsPos(TaskColumns.AutorizedValue) = intCol
                    Case "ATRIBUTOS_SEL" : ColumnsPos(TaskColumns.AtributesAssigned) = intCol
                    Case "ATRIBUTO_1" : ColumnsPos(TaskColumns.Atribute_1) = intCol
                    Case "ATRIBUTO_2" : ColumnsPos(TaskColumns.Atribute_2) = intCol
                    Case "ATRIBUTO_3" : ColumnsPos(TaskColumns.Atribute_3) = intCol
                    Case "ATRIBUTO_4" : ColumnsPos(TaskColumns.Atribute_4) = intCol
                    Case "ATRIBUTO_5" : ColumnsPos(TaskColumns.Atribute_5) = intCol
                    Case "ATRIBUTO_6" : ColumnsPos(TaskColumns.Atribute_6) = intCol
                    Case Else : bolIsValid = False
                End Select

                If bolIsValid Then
                    intCol += 1
                    Columna = GetCellValueWithoutFormat(0, intCol)
                Else
                    InvalidColumn = Columna
                End If
            Loop

            If bolIsValid Then
                If Not ColumnsPos(TaskColumns.Task) = -1 Then
                    intNumColumnas = intCol
                Else
                    InvalidColumn = "Required Fields"
                End If
            End If

            Return intNumColumnas
        End Function

        Private Function GetDataExcelTasks(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelTasks")
            End Try

            Return bolRet

        End Function

        Private Function UpdateTask(ByVal IsNewTask As Boolean, ByVal ColumnsVal() As String, ByRef oTaskState As Task.roTaskState, Optional ByVal _IDTask As Integer = -1) As Boolean

            Dim bolRet As Boolean = False
            Dim intIDCenter As Integer = 0

            If ColumnsVal(TaskColumns.Center).Length > 0 Then
                intIDCenter = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID  from BusinessCenters WHERE Name='" & ColumnsVal(TaskColumns.Center).Replace("'", "''") & "' "))
                If intIDCenter = 0 Then
                    'Si no existe, creamos el nuevo centro de coste
                    Dim oCenter As New BusinessCenter.roBusinessCenter() With {.ID = -1, .Name = ColumnsVal(TaskColumns.Center), .Status = 1, .AuthorizationMode = 0}
                    If oCenter.Save() Then
                        intIDCenter = oCenter.ID
                    End If
                End If
            Else
                'Debemos crear el centro de coste por defecto en caso necesario
                intIDCenter = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID  from BusinessCenters WHERE Name='General' "))
                If intIDCenter = 0 Then
                    Dim oCenter As New BusinessCenter.roBusinessCenter() With {.ID = -1, .Name = "General", .Status = 1, .AuthorizationMode = 0}
                    If oCenter.Save() Then
                        intIDCenter = oCenter.ID
                    End If
                End If
            End If

            If intIDCenter = 0 Then
                Return False
            End If

            'Si la tarea es nueva pero no esta activa no la importamos
            If ColumnsVal(TaskColumns.Status).Length > 0 And roTypes.Any2Double(ColumnsVal(TaskColumns.Status)) <> 1 And IsNewTask Then
                Return False
            End If

            Dim oTask As New Task.roTask(_IDTask, oTaskState, False)
            If IsNewTask Then oTask.ID = -1
            oTask.Name = ColumnsVal(TaskColumns.Task)
            oTask.Project = ColumnsVal(TaskColumns.Project)
            oTask.IDCenter = intIDCenter
            oTask.Description = ColumnsVal(TaskColumns.Description)
            oTask.ShortName = ColumnsVal(TaskColumns.ShortName)
            If oTask.ShortName.Length = 0 Then oTask.ShortName = ""
            oTask.BarCode = ColumnsVal(TaskColumns.BarCode)

            If IsNewTask Then
                'Solo si la tarea es nueva fuerzo todos los parametros por defecto
                oTask.Status = TaskStatusEnum._ON
                oTask.TypeActivation = TaskTypeActivationEnum._ALWAYS
                oTask.TypeClosing = TaskTypeClosingEnum._UNDEFINED
                oTask.ModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
                oTask.TypeCollaboration = TaskTypeCollaborationEnum._ANY
                oTask.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
                oTask.Groups = New Generic.List(Of Task.roGroupTaskDescription)
                oTask.Employees = New Generic.List(Of Task.roEmployeeTaskDescription)
            Else
                'Si estamos actualizando la tarea mantenemos los parametros antiguos de configuración de inicio de la tarea

                Select Case roTypes.Any2Double(ColumnsVal(TaskColumns.Status))
                    Case 1 : oTask.Status = TaskStatusEnum._ON
                    Case 2 : oTask.Status = TaskStatusEnum._CANCELED
                    Case 3 : oTask.Status = TaskStatusEnum._COMPLETED
                End Select
            End If

            If ColumnsVal(TaskColumns.Priority).Length > 0 Then
                oTask.Priority = IIf(roTypes.Any2Integer(ColumnsVal(TaskColumns.Priority)) > 20, 20, roTypes.Any2Integer(ColumnsVal(TaskColumns.Priority)))
                oTask.Priority = IIf(oTask.Priority < 0, 0, oTask.Priority)
            Else
                oTask.Priority = 0
            End If

            oTask.Tag = ColumnsVal(TaskColumns.Tags)

            Try
                oTask.ExpectedStartDate = DateTime.FromOADate(ColumnsVal(TaskColumns.BeginDate))
            Catch
            End Try

            Try
                oTask.ExpectedEndDate = DateTime.FromOADate(ColumnsVal(TaskColumns.EndDate))
            Catch
            End Try

            oTask.InitialTime = roTypes.Any2Double(ColumnsVal(TaskColumns.Duration))

            If ColumnsVal(TaskColumns.CloseType) <> String.Empty Then
                Dim DateClose As Nullable(Of DateTime)
                Try
                    DateClose = DateTime.FromOADate(ColumnsVal(TaskColumns.CloseValue))
                Catch
                End Try

                If roTypes.Any2Double(ColumnsVal(TaskColumns.CloseType)) = 1 And DateClose.HasValue Then
                    oTask.TypeClosing = TaskTypeClosingEnum._ATDATE
                    oTask.ClosingDate = DateClose
                End If
            End If

            If ColumnsVal(TaskColumns.ActivationType) <> String.Empty Then
                'Si es 0 (allways) ya no hacemos nada y mantendremos el antiguo si estaba configurado
                Select Case roTypes.Any2Double(ColumnsVal(TaskColumns.ActivationType))
                    Case 1
                        If IsDate(ColumnsVal(TaskColumns.ActivationValue)) Then
                            oTask.TypeActivation = TaskTypeActivationEnum._ATDATE
                            oTask.ActivationDate = ColumnsVal(TaskColumns.ActivationValue)
                        End If
                    Case 2, 3
                        Dim intIDTask As Integer = 0
                        Dim strTask As String = "@SELECT# ID FROM Tasks WHERE Name='" & ColumnsVal(TaskColumns.ActivationValue).Replace("'", "''") & "' "
                        If ColumnsVal(TaskColumns.Project).Length > 0 Then
                            strTask = strTask & " AND Project='" & ColumnsVal(TaskColumns.Project).Replace("'", "''") & "' "
                        End If
                        intIDTask = roTypes.Any2Integer(ExecuteScalar(strTask))

                        If intIDTask > 0 Then
                            oTask.TypeActivation = TaskTypeActivationEnum._ATRUNTASK
                            If roTypes.Any2Double(ColumnsVal(TaskColumns.ActivationType)) = 2 Then
                                oTask.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK
                            End If
                            oTask.ActivationTask = intIDTask
                        End If
                End Select
            End If

            If ColumnsVal(TaskColumns.ColaborationType) <> String.Empty Then
                Select Case roTypes.Any2Double(ColumnsVal(TaskColumns.ColaborationType))
                    Case 1
                        oTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME
                        oTask.TypeCollaboration = TaskTypeCollaborationEnum._ANY
                    Case 2
                        oTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME
                        oTask.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST
                End Select
            End If

            If ColumnsVal(TaskColumns.AutorizedType) <> String.Empty Then
                Select Case roTypes.Any2Double(ColumnsVal(TaskColumns.AutorizedType))
                    Case 1 ' Departamentos
                        oTask.Employees = New Generic.List(Of Task.roEmployeeTaskDescription)
                        Dim strGroups = ColumnsVal(TaskColumns.AutorizedValue)
                        For i As Integer = 0 To roTypes.StringItemsCount(strGroups, "@") - 1

                            Dim intGroup As Integer = 0
                            GetDepartament(roTypes.String2Item(strGroups, i, "@"), intGroup)
                            If intGroup > 0 Then
                                Dim oGroupTaskState As New Task.roGroupTaskState
                                Dim newElem As New Task.roGroupTaskDescription(intGroup, oGroupTaskState)
                                oTask.Groups.Add(newElem)
                            End If
                        Next
                        If Not oTask.Groups Is Nothing Then
                            If oTask.Groups.Count > 0 Then
                                oTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
                            End If
                        End If

                    Case 2 'Empleados concretos

                        oTask.Groups = New Generic.List(Of Task.roGroupTaskDescription)
                        Dim strEmployees = ColumnsVal(TaskColumns.AutorizedValue)

                        For i As Integer = 0 To roTypes.StringItemsCount(strEmployees, "@") - 1

                            Dim intEmpl As Integer = 0

                            Dim oEmployeeState As New Employee.roEmployeeState
                            roBusinessState.CopyTo(Me.State, oEmployeeState)

                            Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", roTypes.String2Item(strEmployees, i, "@"), Now, New UserFields.roUserFieldState)
                            If tbEmployee.Rows.Count > 0 Then
                                intEmpl = tbEmployee.Rows(0).Item("idemployee")
                            End If
                            If intEmpl > 0 Then
                                Dim newElem As Task.roEmployeeTaskDescription
                                Dim oEmployeeTaskState As New Task.roEmployeeTaskState

                                newElem = New Task.roEmployeeTaskDescription(intEmpl, oEmployeeTaskState)
                                oTask.Employees.Add(newElem)
                            End If
                        Next
                        If Not oTask.Employees Is Nothing Then
                            If oTask.Employees.Count > 0 Then
                                oTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
                            End If
                        End If

                End Select
            End If

            Dim bolSaved As Boolean = False

            If oTask.Save() Then

                bolSaved = True

                If ColumnsVal(TaskColumns.AtributesAssigned).Length > 0 Then
                    ColumnsVal(TaskColumns.AtributesAssigned) = Replace(ColumnsVal(TaskColumns.AtributesAssigned), ".", ",")

                    Dim AtributesAssigned() As String = ColumnsVal(TaskColumns.AtributesAssigned).Split(",")
                    Dim NumAtributesAssigned As Integer = AtributesAssigned.Length
                    If NumAtributesAssigned > 6 Then NumAtributesAssigned = 6

                    Dim taskFieldsUsedIDs As New Generic.List(Of Integer)
                    Dim taskFieldsUsedValues As New Generic.List(Of String)
                    Dim n As Integer = 0

                    For x As Integer = 0 To NumAtributesAssigned - 1
                        n = AtributesAssigned(x)

                        If n <= 6 Then
                            taskFieldsUsedIDs.Add(n)

                            Select Case n
                                Case 1 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_1))
                                Case 2 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_2))
                                Case 3 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_3))
                                Case 4 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_4))
                                Case 5 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_5))
                                Case 6 : taskFieldsUsedValues.Add(ColumnsVal(TaskColumns.Atribute_6))
                            End Select
                        End If
                    Next

                    Dim lstTaskField As New Generic.List(Of UserFields.roTaskField)

                    Dim oUserFieldState As New UserFields.roUserFieldState()
                    roBusinessState.CopyTo(Me.State, oUserFieldState)
                    Dim tbFields As Data.DataTable = UserFields.roUserField.GetTaskFields(Types.TaskField, oUserFieldState)

                    For index As Integer = 0 To taskFieldsUsedIDs.Count - 1
                        For Each rw As Data.DataRow In tbFields.Rows
                            If (roTypes.Any2Integer(rw("ID")) = taskFieldsUsedIDs(index)) Then
                                Dim oTaskField As New UserFields.roTaskField() With {.IDTask = oTask.ID, .IDField = rw("ID"), .Type = rw("Type"), .FieldName = rw("Name"), .Action = rw("Action"), .FieldValue = taskFieldsUsedValues(index)}
                                lstTaskField.Add(oTaskField)
                            End If
                        Next
                    Next

                    Dim oTaskFieldState As New UserFields.roTaskFieldState()
                    roBusinessState.CopyTo(Me.State, oTaskFieldState)
                    If UserFields.roTaskField.SaveTaskFields(oTask.ID, lstTaskField, oTaskFieldState) = False Then
                        bolSaved = False
                    End If
                End If

                bolRet = bolSaved

            End If

            Return bolRet

        End Function

        Private Sub GetDepartament(ByVal strPathDepartament As String, ByRef intIDGroup As Integer)
            Dim strPath As String = String.Empty
            Dim oGroup As Group.roGroup
            Dim oGroupState As New Group.roGroupState
            roBusinessState.CopyTo(Me.State, oGroupState)

            For i As Integer = 0 To roTypes.StringItemsCount(strPathDepartament, "\") - 1
                If i = 0 And roTypes.String2Item(strPathDepartament, i, "\").Length > 0 Then
                    oGroup = Group.roGroup.GetCompanyByName(roTypes.String2Item(strPathDepartament, i, "\"), oGroupState)
                    If oGroup Is Nothing Then
                        strPath = ""
                        intIDGroup = 0
                        Exit For
                    Else
                        strPath = oGroup.ID
                        intIDGroup = oGroup.ID
                    End If
                Else
                    oGroup = Group.roGroup.GetGroupByNameInLevel(roTypes.String2Item(strPathDepartament, i, "\"), strPath, oGroupState)
                    If oGroup Is Nothing Then
                        strPath = ""
                        intIDGroup = 0
                        Exit For
                    Else
                        strPath = oGroup.Path
                        intIDGroup = oGroup.ID
                    End If
                End If
            Next
        End Sub

#End Region
    End Class

End Namespace