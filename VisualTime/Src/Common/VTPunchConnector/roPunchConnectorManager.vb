Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTPunchConnector

    Public Class roPunchConnectorManager

        Private oState As roPunchConnectorState = Nothing

        Public ReadOnly Property State As roPunchConnectorState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roPunchConnectorState()
        End Sub

        Public Sub New(ByVal _State As roPunchConnectorState)
            oState = _State
        End Sub

#End Region

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bRet As Boolean = False

            ' Ejecutamos tarea indicada
            Try

                Dim oParam As New roParameters("OPTIONS", False)
                Dim strTemplateFileName As String = roTypes.Any2String(oParam.Parameter(Parameters.ConnectorReadingsName))
                Dim strSourceFileName As String = roTypes.Any2String(oParam.Parameter(Parameters.ConnectorSourceName))

                If strTemplateFileName <> String.Empty AndAlso strSourceFileName <> String.Empty Then

                    ' Aquí debo coger todos los ficheros del contenedor que cumplan con la máscara, y no sólo uno como hasta ahora
                    Dim lFilesToImport As New List(Of String)
                    Dim arrFilenameAux As String() = strSourceFileName.Split(".")
                    Dim strName As String = arrFilenameAux(0)
                    Dim strExtension As String = arrFilenameAux(1)
                    Dim strCompany As String = Azure.RoAzureSupport.GetCompanyName
                    lFilesToImport = Azure.RoAzureSupport.ListFiles(strName, strExtension, roLiveQueueTypes.datalink, roLiveDatalinkFolders.punches.ToString, True, strCompany)

                    If lFilesToImport.Any() Then
                        ' Fichero de esquema
                        Dim oImportSchemaFile As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strTemplateFileName, roLiveDatalinkFolders.connector.ToString, roLiveQueueTypes.datalink, False)
                        If oImportSchemaFile Is Nothing OrElse oImportSchemaFile.Length = 0 Then
                            oImportSchemaFile = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure(strTemplateFileName, DTOs.roLiveQueueTypes.datalink, False)
                        End If

                        For Each strFileName As String In lFilesToImport
                            Dim oImportFile As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strFileName, roLiveDatalinkFolders.punches.ToString, roLiveQueueTypes.datalink, False)

                            If oImportFile IsNot Nothing AndAlso oImportSchemaFile IsNot Nothing AndAlso oImportFile.Length > 0 AndAlso oImportSchemaFile.Length > 0 Then
                                Azure.RoAzureSupport.RenameFileInCompanyContainer(strFileName, "bck/" & strFileName & "." & Now.ToString("yyyyMMddHHmmss") & ".bck", roLiveDatalinkFolders.punches.ToString, roLiveQueueTypes.datalink)

                                Dim oPunchLines As Generic.List(Of String) = Me.GetPunchImportLines(oImportFile)
                                Dim dicDictionary As New Dictionary(Of String, String)
                                Dim oSchema As Generic.List(Of roConnectorParameter) = Me.GetSchemaDescription(oImportSchemaFile, dicDictionary)

                                Dim bSaved As Boolean = True

                                For Each oEntryLine As String In oPunchLines
                                    bSaved = Me.SaveEntry(oEntryLine, oSchema, strFileName, dicDictionary)
                                Next
                            Else
                                'No se ha encontrado plantilla o fichero de importacion
                            End If
                        Next

                        'Importamos por si existieran fichajes en la tabla entries aunque no haya ficheros
                        bRet = Me.CreatePunchesFromEntries()
                    End If
                Else
                    'Connector no configurado. No hacemos nada
                    bRet = True
                End If


            Catch Ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunchConnectorManager::ExecuteTask :", Ex)
            End Try

            Return New BaseTaskResult With {.Result = bRet, .Description = String.Empty}
        End Function

#Region "MovesNET Methods"

        Public Function CreatePunchesFromEntries() As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String = String.Empty

                Dim xFreezingDate As Date = New Date(1900, 1, 1)
                Dim oParameters As New roParameters("OPTIONS", True)
                If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing Then
                    If IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                        xFreezingDate = oParameters.Parameter(Parameters.FirstDate)
                    End If
                End If

                Dim customization As String = String.Empty
                Try
                    Dim advParFiat As String = New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState).Value.ToUpper
                    customization = advParFiat.ToUpper
                Catch ex As Exception
                    customization = String.Empty
                End Try

                strSQL = "@SELECT# * FROM Entries WHERE DATETIME >= " & roTypes.Any2Time(xFreezingDate).SQLDateTime & " ORDER BY DATETIME ASC, ID ASC "

                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        ' Procesamos el registro
                        If CreatePunchFromEntry(oRow, customization) Then
                            ' Si todo ha ido bien eliminados el registro
                            Dim strSQLDel As String = "@DELETE# FROM Entries WHERE ID = " & oRow("ID")
                            ExecuteSql(strSQLDel)

                            ' Eliminamos la imagen del registro si existe
                            strSQLDel = "@DELETE# FROM EntriesCaptures WHERE IDEntry = " & oRow("ID")
                            ExecuteSql(strSQLDel)
                        End If
                    Next
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::ImportFromEntries::Fatal error::", ex)
            Finally

            End Try

            Return bolRet
        End Function

        Private Function CreatePunchFromEntry(ByVal oRow As DataRow, ByVal strCustomization As String) As Boolean
            '
            ' Procesa el registro de la tabla ENTRIES
            '
            Dim EmployeeID As Double = 0
            Dim CauseID As Long = 0
            Dim TerminalType As String = ""
            Dim ReaderType As String = ""
            Dim oPunch As New Punch.roPunch
            Dim strType As String = ""
            Dim bolRet As Boolean = True

            Try

                ' Obtenenemos el tipo de fichaje
                strType = roTypes.Any2String(oRow("Type"))

                Select Case strType.ToUpper
                    Case "E"
                        oPunch.Type = PunchTypeEnum._IN
                    Case "S"
                        oPunch.Type = PunchTypeEnum._OUT
                    Case "A"
                        oPunch.Type = PunchTypeEnum._AV
                    Case "X"
                        oPunch.Type = PunchTypeEnum._AUTO
                    Case "L"
                        oPunch.Type = PunchTypeEnum._L
                    Case "D"
                        oPunch.Type = PunchTypeEnum._DR
                    Case "I"
                        oPunch.Type = PunchTypeEnum._AI
                        oPunch.InvalidType = InvalidTypeEnum.NTIME_
                    Case Else
                        ' Tipo desconocido
                        bolRet = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::ExecuteBatch: Unknown punch type " & strType & " for punch: Credential= " & oRow("IDCard") & " DatetTime= " & oRow("DateTime").ToString)
                End Select

                If bolRet Then
                    'Obtenemos la resta de datos del fichaje
                    oPunch.DateTime = oRow("DateTime")
                    oPunch.TypeData = oRow("IDCause")
                    oPunch.IDTerminal = oRow("IDReader")
                    oPunch.IDReader = oRow("Rdr")
                    oPunch.IDCredential = oRow("IDCard")

                    Dim strPunchForLog As String
                    strPunchForLog = "Credential = " & oRow("IDCard") & " DateTime = " & oRow("DateTime").ToString & " Cause = " & oRow("IDCause").ToString & " Terminal = " & oRow("IDReader").ToString

                    ' Obtenemos la foto en caso que exista
                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM EntriesCaptures WHERE IDEntry = " & oRow("ID"))
                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                        If Not IsDBNull(tb.Rows(0).Item("Capture")) Then
                            Dim bImage As Byte() = CType(tb.Rows(0).Item("Capture"), Byte())
                            If bImage.Length > 0 Then
                                Dim ms As MemoryStream = New MemoryStream(bImage)
                                oPunch.Capture = CType(Image.FromStream(ms), Bitmap)
                            End If
                        End If

                    End If

                    If oPunch.IDTerminal = 97 Or oPunch.IDTerminal = 98 Then
                        ' Es un fichaje proveniente de terminal web
                        TerminalType = "Web"
                        EmployeeID = oRow("IdCard")
                        oPunch.IDZone = 0
                    Else
                        ' Es un terminal físico
                        TerminalType = roTypes.Any2String(ExecuteScalar("@SELECT# Type From Terminals Where ID=" & oPunch.IDTerminal))

                        If InStr(1, TerminalType, "CipherLab") > 0 Then
                            If roTypes.Any2Double(oPunch.IDReader) = 1 Then
                                oPunch.IDReader = 2
                            ElseIf roTypes.Any2Double(oPunch.IDReader) = 2 Then
                                oPunch.IDReader = 1
                            End If
                        End If

                        oPunch.IDZone = roTypes.Any2Double(ExecuteScalar("@SELECT# IDZone FROM TerminalReaders WHERE ID= " & oPunch.IDReader & " AND IDTerminal = " & oPunch.IDTerminal))

                        ReaderType = roTypes.Any2String(ExecuteScalar("@SELECT# Type From TerminalReaders Where IDTerminal=" & oPunch.IDTerminal & " and id=" & oPunch.IDReader))

                        If InStr(UCase(TerminalType), "MX7") > 0 OrElse InStr(UCase(TerminalType), "MX8") > 0 Then
                            ' Si es versión mx7 antiguo (v 1.x.x.x)
                            ' Si es un terminal MX7 independientemente del lector, siempre devuelve el identificador del empleado
                            EmployeeID = roTypes.Any2Double(ExecuteScalar("@SELECT# id from employees where id=" & oRow("IdCard")))
                            If EmployeeID = 0 Then
                                ' Es Live. Miro en passports
                                EmployeeID = GetEmployeeIDFromCardIDLive(oRow("IdCard"), ReaderType, roTypes.Any2Time(oRow("DateTime")).DateOnly)
                            End If
                        ElseIf ReaderType = "BIO" Then
                            'Si es un lector biometric buscamos el id de empleado con la huella
                            EmployeeID = roTypes.Any2Double(ExecuteScalar("@SELECT# id from employees where biometricid=" & oRow("IdCard")))
                            If EmployeeID = 0 Then
                                'Si es una papelera puede que fichen con tarjeta.
                                If roTypes.Any2Double(ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue) = " & oRow("IdCard"))) = 0 Then
                                    oPunch.IDCredential = oRow("IdCard")
                                Else
                                    oPunch.IDCredential = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue) = " & oRow("IdCard"))
                                End If
                                'Miramos si el ID de tarjeta del marcaje se corresponde con una tarjeta de algun empleado
                                EmployeeID = GetEmployeeIDFromCardID(oPunch.IDCredential, roTypes.Any2Time(oRow("DateTime")).DateOnly)
                            End If
                        ElseIf ReaderType = "SX" Or ReaderType = "RX" Or ReaderType = "RXF" Or ReaderType = "MAT" OrElse (UCase(TerminalType) = "RXC" Or UCase(TerminalType) = "RXCE" Or UCase(TerminalType) = "RXCEP" Or UCase(TerminalType) = "RXCP" Or UCase(TerminalType) = "RXF") Then
                            ' Los terminales SX y RX/F envían directamente el identificador de empleado
                            EmployeeID = oRow("IdCard")
                        Else
                            ' Para versiones Live (con uso de passports)
                            If oRow.Table.Columns.Contains("UsesIdEmployee") AndAlso Not IsDBNull(oRow("UsesIdEmployee")) AndAlso oRow("UsesIdEmployee") Then
                                ' IDCard es un identificador de empleado
                                EmployeeID = oRow("IdCard")
                            Else
                                EmployeeID = GetEmployeeIDFromCardIDLive(oRow("IdCard"), ReaderType, roTypes.Any2Time(oRow("DateTime")).DateOnly)
                            End If
                        End If
                    End If

                    ' Selecciona el Modo del terminal
                    Dim Mode As String = roTypes.Any2String(ExecuteScalar("@SELECT# MODE FROM TerminalReaders WHERE ID= " & oPunch.IDReader & " AND IDTerminal=" & oPunch.IDTerminal))

                    ' Comprueba si el terminal tiene asociado un centro de coste directo
                    Dim idCostCenter As Integer = roTypes.Any2Double(ExecuteScalar("@SELECT# idCostCenter FROM TerminalReaders WHERE Mode in ('CO') and ID= " & oPunch.IDReader & " AND IDTerminal = " & oPunch.IDTerminal))
                    If idCostCenter > 0 Then
                        oPunch.Type = PunchTypeEnum._CENTER
                        oPunch.TypeData = idCostCenter
                    Else
                        ' Si el terminal es sólo centro de costes y no ha fichado ningún centro
                        If Mode = "CO" And roTypes.Any2Double(oPunch.TypeData) < 800 Then
                            oPunch.Type = PunchTypeEnum._CENTER
                            oPunch.TypeData = 0
                        Else
                            ' Comprueba el tipo de justificacion fichada
                            If roTypes.Any2Double(oPunch.TypeData) > 0 Then
                                If roTypes.Any2Double(oPunch.TypeData) < 800 Or (Mode <> "CO" And Mode <> "TACO") Then
                                    ' Justificacion de presencia
                                    If Not TerminalType = "Web" Then
                                        ' Las entradas que vengan de Web (webTerminal,...) vienen con el Id de la justificación, luego no necesito recuperar dicho id
                                        ' Si no, recupero el Id de la justificación a partir de su código de terminal
                                        CauseID = GetCauseIDFromKeyCode(oPunch.TypeData)
                                    Else
                                        CauseID = oPunch.TypeData
                                    End If
                                    If CauseID = -1 Then
                                        'La causa no está dada de alta
                                        oPunch.TypeData = 0
                                    Else
                                        oPunch.TypeData = CauseID
                                    End If
                                Else
                                    Dim oTable As DataTable
                                    Dim oCostCencerState As New BusinessCenter.roBusinessCenterState
                                    Dim CenterIsAvailble As Boolean = False
                                    Dim idCenter As Integer = 0

                                    ' Justificacion interpretada como centro de coste
                                    oPunch.Type = PunchTypeEnum._CENTER

                                    ' Busca el id del centro
                                    idCenter = roTypes.Any2Double(ExecuteScalar("@SELECT# id FROM BusinessCenters WHERE Description Like '%#Terminal=" & oPunch.TypeData & "#%'"))

                                    ' Lee los centros de coste en los que puede fichar el empleado
                                    oTable = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oCostCencerState, EmployeeID, False)
                                    If oTable.Rows.Count > 0 And idCenter > 0 Then
                                        ' El centro fichado lo busca en los centros en los que puede fichar el empleado
                                        Dim row() As DataRow = oTable.Select("idCenter=" & idCenter)

                                        ' Si no lo ha encontrado mira si puede fichar en el centro al que pertenece su grupo
                                        If row.Length = 0 Then
                                            oTable = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oCostCencerState, EmployeeID, True)
                                            If oTable.Rows.Count > 0 Then
                                                row = oTable.Select("idCenter=" & idCenter)
                                                CenterIsAvailble = (row.Length > 0)
                                            End If
                                        Else
                                            CenterIsAvailble = True
                                        End If
                                    End If

                                    ' Si tiene permiso para fichar el centro busca su id
                                    If CenterIsAvailble Then
                                        oPunch.TypeData = idCenter
                                    Else
                                        oPunch.TypeData = 0
                                    End If
                                End If
                            End If
                        End If
                    End If

                    'Miramos si existe mas de un empleado con el EmployeeID recuperado
                    If roTypes.Any2Double(ExecuteScalar("@SELECT# count(*) FROM Employees WHERE Id = " & EmployeeID)) <> 1 Then
                        ' Marco la entrada como inválida
                        ' No tenemos un ID de empleado correcto
                        EmployeeID = 0
                    End If

                    ' Aisgnamos el empleado al fichaje
                    oPunch.IDEmployee = EmployeeID

                    ' Descartamos fichajes duplicados
                    If PunchDuplicated(oPunch) Then
                        ' Fichaje ya existe en la tabla Punches
                        bolRet = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::ExecuteBatch: Punch already exists. Will not be saved: " & strPunchForLog)
                        Dim strSQLDel As String = "@DELETE# FROM Entries WHERE ID = " & oRow("ID")
                        ExecuteSql(strSQLDel)
                    End If

                    If bolRet AndAlso strCustomization = "TAIF" Then
                        bolRet = bolPuncheOK(oPunch)

                        If Not bolRet Then
                            ' Si el registro no es OK lo borra
                            Dim strSQLDel As String = "@DELETE# FROM Entries WHERE ID = " & oRow("ID")
                            ExecuteSql(strSQLDel)
                        End If
                    End If

                    If bolRet Then
                        ' Guardamos el fichaje
                        oPunch.Source = PunchSource.Collector
                        If oPunch.Save() Then
                            bolRet = True
                            ' Si el terminal tiene el modo de "fichaje de tarea automatico"
                            ' y es una entrada de presencia
                            If oPunch.ActualType.HasValue Then
                                If oPunch.ActualType = PunchTypeEnum._IN And oPunch.IDEmployee > 0 Then
                                    If roTypes.Any2String(ExecuteScalar("@SELECT# isnull(Description, '') FROM Terminals WHERE ID = " & oPunch.IDTerminal)).ToString.ToUpper.Contains("#AUTOMATICTASK") Then
                                        ' Creamos el fichaje de tarea a partir del puesto asignado en la planificacion
                                        ' en caso que sea un empleado de tipo tareas
                                        If roTypes.Any2String(ExecuteScalar("@SELECT# isnull(Type, '') FROM Employees WHERE ID = " & oPunch.IDEmployee)) = "J" Then
                                            CreateAutomaticTaskPunch(oPunch)
                                        End If
                                    End If
                                End If
                            End If

                            ' En cliente los días con fichajes importados no se calculaban. Forzamos ...
                            Try
                                If oPunch.IDEmployee > 0 Then
                                    Dim xDailyDate As DateTime
                                    If oPunch.ShiftDate.HasValue Then
                                        xDailyDate = oPunch.ShiftDate.Value
                                    ElseIf oPunch.DateTime.HasValue Then
                                        xDailyDate = oPunch.DateTime
                                    End If
                                    Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 0 WHERE IDEmployee = " & oPunch.IDEmployee & " AND Date = " & roTypes.Any2Time(xDailyDate.Date).SQLSmallDateTime
                                    ExecuteSql(strSQL)
                                End If
                            Catch ex As Exception
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::ExecuteBatch: Unable to set Status on slow system", ex)
                            End Try
                        Else
                            bolRet = False
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::ExecuteBatch: Unable to save punch: " & strPunchForLog)
                        End If
                    End If
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::ExecuteBatch:Exception trying to save punch", Ex)
            End Try

            Return bolRet
        End Function

        Private Function PunchDuplicated(ByVal oPunch As Punch.roPunch) As Boolean
            Dim bolRet As Boolean = False
            Dim tbPunches As DataTable
            Dim oState As New Punch.roPunchState
            Try
                tbPunches = Punch.roPunch.GetPunches("IDEmployee = " & oPunch.IDEmployee.ToString & " AND " &
                                                                      "DateTime = " & roTypes.Any2Time(oPunch.DateTime).SQLDateTime & " AND  " &
                                                                      "idTerminal=" & oPunch.IDTerminal.ToString & " and " &
                                                                      "Type=" & oPunch.Type, oState)
                If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                    bolRet = True
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::PunchDuplicated:", ex)
            End Try
            Return bolRet
        End Function

        Private Function bolPuncheOK(ByVal oPunch As Punch.roPunch) As Boolean
            Dim bolRet As Boolean = True

            Try
                Dim intPunchPeriodRTIn As Integer = 0
                Dim oParameters As New roParameters("OPTIONS")
                Dim _State As New Punch.roPunchState
                Dim _punch As Punch.roPunch

                ' Comprueba si el fichaje está duplicado
                Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & oPunch.IDEmployee.ToString & " AND " &
                                                                      "DateTime = " & roTypes.Any2Time(oPunch.DateTime).SQLDateTime & " AND  " &
                                                                      "idTerminal=" & oPunch.IDTerminal.ToString & " and " &
                                                                      "Type=" & oPunch.Type, _State)
                If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::bolPuncheOK: Punch is duplicated:(" & oPunch.IDEmployee.ToString & "," & roTypes.Any2Time(oPunch.DateTime).SQLDateTime & "," & oPunch.IDTerminal.ToString & "," & oPunch.Type & ")")
                    bolRet = False
                    Exit Try
                End If

                ' Obtengo configuración tiempo mínimo entre una entrada-salida y una salida-entrada
                intPunchPeriodRTIn = roTypes.Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTIn))

                ' Obtener información del último fichaje de presencia del empleado
                Dim oLastPunchType As PunchStatus = PunchStatus.Indet_
                Dim xLastPunchDateTime As DateTime
                Dim lngLastPunchID As Long = -1

                _punch = New Punch.roPunch(oPunch.IDEmployee, -1, _State)
                _punch.GetLastPunchPres(oLastPunchType, xLastPunchDateTime, lngLastPunchID)
                _punch.ID = lngLastPunchID
                _punch.Load()

                ' Detecta fichaje repetido
                If oLastPunchType = PunchStatus.In_ Or oLastPunchType = PunchStatus.Out_ Then
                    Dim Seconds As Long = DateDiff(DateInterval.Second, xLastPunchDateTime, CDate(oPunch.DateTime))
                    If (_punch.IDTerminal = oPunch.IDTerminal And Seconds > 0 And Seconds <= intPunchPeriodRTIn * 60) Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::bolPuncheOK: Punch is too close to another:(" & oPunch.IDEmployee.ToString & "," & roTypes.Any2Time(oPunch.DateTime).SQLDateTime & "," & oPunch.IDTerminal.ToString & "," & oPunch.Type & ")")
                        bolRet = False
                        Exit Try
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::Fiat_IsPuncheOK :", ex)
            End Try

            Return bolRet
        End Function

        Private Function CreateAutomaticTaskPunch(ByVal oPunch As Punch.roPunch) As Boolean
            '
            ' Creamos un inico de tarea que corresponda con el puesto asignado planificado
            '
            Dim ssql As String = ""
            Dim bolRet As Boolean = False
            Dim oPunchTask As New Punch.roPunch
            Dim PunchTimeOnly As DateTime
            Dim ShiftDate As DateTime
            Dim IDTask As Double = 0

            Try

                ' Obtenemos la fecha del fichaje
                ShiftDate = New Date(oPunch.DateTime.Value.Year, oPunch.DateTime.Value.Month, oPunch.DateTime.Value.Day)

                ' Si el fichaje es anterior a las 06:00
                ' miramos si el dia anterior es nocturno
                ' en ese caso tenemos que mirar el puesto asignado al dia anterior no al actual
                PunchTimeOnly = New Date(1, 1, 1, oPunch.DateTime.Value.Hour, oPunch.DateTime.Value.Minute, 0)
                Dim LimitTime As DateTime
                LimitTime = New Date(1, 1, 1, 6, 0, 0)
                If PunchTimeOnly < LimitTime Then
                    ' Miramos si el dia anterior tiene horario nocturno
                    Dim IDShift As Double = roTypes.Any2Double(ExecuteScalar("@SELECT# isnull(IDShiftUsed, 0) FROM DailySchedule WHERE IDEmployee = " & oPunch.IDEmployee & " AND Date = " & roTypes.Any2Time(DateAdd(DateInterval.Day, -1, ShiftDate)).SQLSmallDateTime))
                    If IDShift > 0 Then
                        Dim ShiftState As New Shift.roShiftState
                        Dim oShift As New Shift.roShift(IDShift, ShiftState)
                        Dim EndLimit As DateTime
                        EndLimit = New Date(oShift.EndLimit.Year, oShift.EndLimit.Month, oShift.EndLimit.Day)
                        Dim EndLimitNight As DateTime
                        EndLimitNight = New Date(1899, 12, 31)
                        If EndLimitNight = EndLimit Then
                            ShiftDate = DateAdd(DateInterval.Day, -1, ShiftDate)
                        End If
                    End If
                End If

                ' Obtenemos el puesto planificado para ese dia
                Dim IDAssignment As Double = roTypes.Any2Double(ExecuteScalar("@SELECT# isnull(IDAssignment, 0) FROM DailySchedule WHERE IDEmployee = " & oPunch.IDEmployee & " AND Date = " & roTypes.Any2Time(ShiftDate).SQLSmallDateTime))

                If IDAssignment > 0 Then
                    ' Obtenemos la tarea a la que está asignado el puesto
                    IDTask = roTypes.Any2Double(ExecuteScalar("@SELECT# IDTask FROM TaskAssignments WHERE IDAssignment = " & IDAssignment))
                End If

                ' Creamos el fichaje de tareas
                oPunchTask.IDEmployee = oPunch.IDEmployee
                oPunchTask.Type = PunchTypeEnum._TASK
                oPunchTask.ActualType = PunchTypeEnum._TASK
                oPunchTask.ShiftDate = ShiftDate
                oPunchTask.DateTime = oPunch.DateTime
                oPunchTask.IDReader = oPunch.IDReader
                oPunchTask.IDTerminal = oPunch.IDTerminal
                oPunchTask.TypeData = IDTask
                oPunchTask.Source = PunchSource.Collector

                If oPunchTask.Save(False, False, False) Then
                    bolRet = True
                End If
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::CreateAutomaticTaskPunch :", Ex)
            End Try

            Return bolRet

        End Function

        Private Function GetEmployeeIDFromCardID(ByVal CardID As String, ByVal Day As Date) As Long
            '
            ' Recupera el ID de un empleado pasandole el ID de Tarjeta
            '  Si no encuentra el empleado especificado devuelve -1
            '
            Dim ssql As String = ""
            Dim aDate As New roTime
            Dim bolRet As Long = 0

            Try
                GetEmployeeIDFromCardID = 0
                aDate.Value = Day

                'Creamos la sentencia SQL
                ssql = "@SELECT# IDEmployee FROM EmployeeContracts WHERE IdCard = " & CardID
                ssql = ssql & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime

                bolRet = roTypes.Any2Long(ExecuteScalar(ssql))

                If bolRet = 0 Then
                    ' Puede ocurrir que en la EmployeeCOntracts hayan asignado una targeta desde un terminal que recogiera más bytes
                    ' Para descartarlo miro con un like (no se hace en una sola instrucción porque el like penaliza)
                    ssql = "@SELECT# IDEmployee FROM EmployeeContracts WHERE CONVERT(nvarchar(50),Idcard) LIKE '%" & CardID & "'"
                    ssql = ssql & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime

                    bolRet = roTypes.Any2Long(ExecuteScalar(ssql))
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::GetEmployeeIDFromCardID :", Ex)
            End Try

            Return bolRet

        End Function

        Private Function GetCauseIDFromKeyCode(ByVal aIncidenceKeyCode As String) As Long
            '
            'Devuelve el identificador de causa desde su código de teclado
            '
            Dim ssql As String = ""
            Dim retValue As String = ""
            Dim bolRet As Long = 0

            Try

                If aIncidenceKeyCode = 0 Then
                    'En caso de encontrarnos sin incidencia, sale directamente
                    bolRet = 0
                    Return bolRet
                End If

                'Consulta a que causa correponde
                ssql = "@SELECT# ID From Causes Where ReaderInputCode = " & aIncidenceKeyCode & " AND AllowInputFromReader=1"
                retValue = roTypes.Any2String(ExecuteScalar(ssql))

                If Len(retValue) > 0 Then
                    bolRet = roTypes.Any2Long(retValue)
                Else
                    bolRet = -1
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::GetCauseIDFromKeyCode :", Ex)
            End Try

            Return bolRet

        End Function

        Private Function GetEmployeeIDFromCardIDLive(ByVal CardID As String, ByVal TerminalType As String, ByVal Day As Date) As Long
            '
            '  Versión LIVE: Recupera el ID de un empleado pasandole el ID de Tarjeta
            '  Si no encuentra el empleado especificado devuelve -1
            '
            Dim strCard As String = ""
            Dim retValue As String = ""
            Dim CardString As String
            strCard = CardID 'Código interno tranformado
            Dim bolCardAliases As Boolean
            Dim ssql As String
            Dim bolRet As Long = 0

            Try

                retValue = roTypes.Any2String(ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'"))

                If Len(retValue) > 0 Then
                    CardString = CStr(retValue)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                ssql = "@SELECT# Employees.ID " &
                       "FROM Employees " &
                            "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee AND " &
                                        " (BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, " & roTypes.Any2Time(Day.Date).SQLSmallDateTime & ", 102), 102)) AND " &
                                        " (EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, " & roTypes.Any2Time(Day.Date).SQLSmallDateTime & ", 102), 102))" &
                            "INNER JOIN sysroPassports ON Employees.ID = sysroPassports.IDEmployee " &
                            "INNER JOIN sysroPassports_AuthenticationMethods ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                        "WHERE sysroPassports_AuthenticationMethods.Enabled=1 AND sysroPassports_AuthenticationMethods.Method = 3 AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "

                If bolCardAliases Then
                    ssql = ssql + "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                Else
                    If UCase(TerminalType) = "VR" Then ' Si el terminal es virtual convierte Credential a tipo Integer numerico 00021 y 21
                        ssql = ssql + "Credential <>'' and CONVERT(decimal(18,0),sysroPassports_AuthenticationMethods.Credential)=" & CardString
                    Else
                        ssql = ssql + "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CardString & "'"
                    End If
                End If

                bolRet = roTypes.Any2Long(ExecuteScalar(ssql))

                If bolRet = 0 Then
                    ssql = "@SELECT# Employees.ID " &
                             "FROM Employees " &
                                    "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee AND " &
                                        " (BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, " & roTypes.Any2Time(Day.Date).SQLSmallDateTime & ", 102), 102)) AND " &
                                        " (EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, " & roTypes.Any2Time(Day.Date).SQLSmallDateTime & ", 102), 102))" &
                                    "INNER JOIN sysroPassports ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                             "WHERE sysroPassports_AuthenticationMethods.Enabled=1 AND sysroPassports_AuthenticationMethods.Method = 3 AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "

                    If UCase(TerminalType) = "VR" Then ' Si el terminal es virtual convierte Credential a tipo Integer numerico 00021 y 21
                        ssql = ssql + "Credential <>'' and CONVERT(decimal(18,0),sysroPassports_AuthenticationMethods.Credential)=" & roTypes.Any2String(roTypes.Any2Long(strCard))
                    Else
                        ssql = ssql + "sysroPassports_AuthenticationMethods.Credential LIKE '%" & roTypes.Any2String(roTypes.Any2Long(strCard)) & "'"
                    End If

                    bolRet = roTypes.Any2Double(ExecuteScalar(ssql))
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPunchConnectorManager::GetEmployeeIDFromCardIDLive :", Ex)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Connector Methods"

        Private Function GetPunchImportLines(ByVal oFileContent As Byte()) As Generic.List(Of String)
            Dim oPunchLines As New Generic.List(Of String)

            Try
                Dim memStream As New MemoryStream(oFileContent)
                Dim oReader As New StreamReader(memStream)
                While Not oReader.EndOfStream
                    oPunchLines.Add(oReader.ReadLine())
                End While
                oReader.Close()
                memStream.Close()
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunchConnectorManager::GetPunchImportLines::Unhandled exception::", ex)
            End Try

            Return oPunchLines
        End Function

        Private Function GetSchemaDescription(ByVal oFileContent As Byte(), ByRef dicDictionary As Dictionary(Of String, String)) As Generic.List(Of roConnectorParameter)
            Dim oSchema As New Generic.List(Of roConnectorParameter)

            Try
                Dim oSchemaLines As New Generic.List(Of String)

                Dim memStream As New MemoryStream(oFileContent)
                Dim oReader As New StreamReader(memStream)
                While Not oReader.EndOfStream
                    oSchemaLines.Add(oReader.ReadLine())
                End While
                oReader.Close()
                memStream.Close()

                Dim cParam As New roConnectorParameter
                For Each oSchemaLine As String In oSchemaLines
                    If oSchemaLine.StartsWith(";") Then Continue For

                    If oSchemaLine.Trim = String.Empty Then
                        If cParam.Type <> roConnectorParameterType.Unknown Then
                            oSchema.Add(cParam)
                            cParam = New roConnectorParameter()
                        End If
                    Else
                        If oSchemaLine.StartsWith("[") Then
                            If oSchemaLine.Contains("DB") Then
                                cParam.Type = roConnectorParameterType.DBField
                            ElseIf oSchemaLine.Contains("Dictionary") Then
                                cParam.Type = roConnectorParameterType.Dictionary
                            Else
                                cParam.Type = roConnectorParameterType.Field
                                cParam.Name = oSchemaLine.Replace("[", "").Replace("]", "")
                            End If
                        Else

                            If oSchemaLine.Contains("=") Then
                                Dim sCommand As String = oSchemaLine.Substring(0, oSchemaLine.IndexOf("="))

                                If cParam.Type = roConnectorParameterType.Dictionary AndAlso oSchemaLine.Split("=").Count = 2 Then
                                    If Not dicDictionary.ContainsKey(oSchemaLine.Split("=")(0)) Then
                                        dicDictionary.Add(oSchemaLine.Split("=")(0), oSchemaLine.Split("=")(1))
                                    End If
                                Else
                                    Select Case sCommand.ToUpper
                                        Case "SPLITBY"
                                            cParam.SplitBy = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "SPLITINDEX"
                                            cParam.SplitPosition = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "NAME"
                                            cParam.Name = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "VALUE"
                                            cParam.Value = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "VALIDATE"
                                            cParam.Validate = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "CONDITION"
                                            cParam.Condition = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "BEGINPOS"
                                            cParam.BeginPos = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                        Case "LEN"
                                            cParam.Len = oSchemaLine.Substring(oSchemaLine.IndexOf("=") + 1)
                                    End Select
                                End If
                            End If

                        End If

                    End If
                Next
                If cParam.Type <> roConnectorParameterType.Unknown Then
                    oSchema.Add(cParam)
                    cParam = New roConnectorParameter()
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunchConnectorManager::GetSchemaDescription::Unhandled exception::", ex)
            End Try

            Return oSchema
        End Function

        Private Function SaveEntry(ByVal oEntryLine As String, ByVal oSchema As Generic.List(Of roConnectorParameter), ByVal strTemplateFileName As String, ByVal dicDictionary As Dictionary(Of String, String)) As Boolean
            Dim oEntryFields As New Generic.List(Of roConnectorParameter)

            Dim bSaved As Boolean = True
            Try
                Dim sSql As String = String.Empty

                Dim bEntryIsValid As Boolean = True
                Dim eInvalidReason As DTOs.InvalidEntryReason
                Dim entryLineSerialized As String = String.Empty

                Try

                    Dim dbFields As Generic.List(Of roConnectorParameter) = oSchema.FindAll(Function(x) x.Type = roConnectorParameterType.DBField)
                    Dim fields As Generic.List(Of roConnectorParameter) = oSchema.FindAll(Function(x) x.Type = roConnectorParameterType.Field)

                    For Each pField As roConnectorParameter In fields
                        If pField.BeginPos IsNot Nothing AndAlso pField.Len IsNot Nothing Then
                            pField.CalculatedValue = oEntryLine.Substring(pField.BeginPos - 1, pField.Len)
                        Else
                            pField.CalculatedValue = oEntryLine.Split(pField.SplitBy)(pField.SplitPosition)
                        End If
                    Next

                    entryLineSerialized = roJSONHelper.Serialize(fields)

                    Dim sTerminalType As String = String.Empty
                    For Each oField As roConnectorParameter In dbFields
                        If oField.Value.ToLower.StartsWith("fields(""") Then
                            Dim fieldName As String = oField.Value.ToLower.Replace("fields(""", "").Replace(""")", "")

                            oEntryFields.Add(New roConnectorParameter() With {
                                   .Type = roConnectorParameterType.DBField,
                                   .Name = oField.Name,
                                   .CalculatedValue = fields.Find(Function(x) x.Name.ToLower = fieldName.ToLower).CalculatedValue
                                })

                        ElseIf oField.Value.ToLower.StartsWith("value=") Then
                            Dim func As String = oField.Value.ToLower.Replace("value=", "").Trim
                            Dim iTerminalId As Integer = -1

                            If oField.Name = "IDReader" Then

                                Dim calcValueParts As String() = func.Split("&")

                                Dim fValue As String = String.Empty
                                For Each sPart As String In calcValueParts
                                    Dim cPart As String = sPart.Trim()

                                    If cPart.StartsWith("fields(") Then
                                        cPart = cPart.Replace("fields(", "").Replace(")", "").Replace("""", "")
                                        fValue = fValue & fields.Find(Function(x) x.Name.ToLower = cPart).CalculatedValue
                                    Else
                                        fValue = fValue & cPart.Replace("""", "")
                                    End If
                                Next

                                If fValue.StartsWith("virtual") Then
                                    If func.Length = 7 Then
                                        iTerminalId = roTypes.Any2Integer(ExecuteScalar("@select# MAX(ID) from terminals where UPPER(type) = 'VIRTUAL'"))
                                        sTerminalType = roTypes.Any2String(ExecuteScalar("@select# Mode from TerminalReaders where idTerminal =" & iTerminalId))
                                    Else
                                        Dim locationString As String = fValue.Substring(7)
                                        Dim locationStringAux As String = locationString
                                        Dim locationInt As Integer
                                        If Integer.TryParse(locationString, locationInt) Then
                                            locationInt = CInt(locationString)
                                        Else
                                            locationInt = -1
                                        End If
                                        If locationInt > 0 Then locationStringAux = roTypes.Any2String(locationInt)
                                        If locationString = locationStringAux Then
                                            iTerminalId = roTypes.Any2Integer(ExecuteScalar($"@SELECT# MAX(ID) FROM Terminals WHERE UPPER(Type) = 'VIRTUAL' AND Location = '{locationString}'"))
                                        Else
                                            iTerminalId = roTypes.Any2Integer(ExecuteScalar($"@SELECT# MAX(ID) FROM Terminals WHERE UPPER(Type) = 'VIRTUAL' AND (Location = '{locationString}' OR Location = '{locationStringAux}')"))
                                        End If

                                        sTerminalType = roTypes.Any2String(ExecuteScalar("@select# Mode from TerminalReaders where idTerminal =" & iTerminalId))
                                        End If

                                        If iTerminalId > 0 Then
                                        oEntryFields.Add(New roConnectorParameter() With {
                                        .Type = roConnectorParameterType.DBField,
                                        .Name = oField.Name,
                                        .CalculatedValue = iTerminalId.ToString
                                    })
                                    Else
                                        bEntryIsValid = False
                                        eInvalidReason = InvalidEntryReason.InvalidVirtualTerminal
                                    End If
                                End If
                            Else
                                oEntryFields.Add(New roConnectorParameter() With {
                                       .Type = roConnectorParameterType.DBField,
                                       .Name = oField.Name,
                                       .CalculatedValue = func.Replace("""", "")
                                   })
                            End If
                        ElseIf oField.Value.ToLower.StartsWith("cdate(") Then
                            Dim func As String = oField.Value.ToLower.Replace("cdate(", "").Trim

                            If oField.Name = "DateTime" Then
                                Dim calcValueParts As String() = func.Replace("cdate(", "").Replace(")", "").Split("&")

                                Dim fValue As String = String.Empty
                                For Each sPart As String In calcValueParts
                                    Dim cPart As String = sPart.Trim().Replace("""", "")

                                    If cPart.ToLower.StartsWith("$field") Then
                                        cPart = cPart.Replace("$", "")
                                        fValue = fValue & fields.Find(Function(x) x.Name.ToLower = cPart.ToLower).CalculatedValue
                                    Else

                                        Dim rPart As String = cPart.Replace("""", "")

                                        If rPart.ToLower.StartsWith("mid(") Then

                                            Dim substringParts As String() = rPart.ToLower.Replace("mid(", "").Replace(")", "").Split(",")

                                            If substringParts.Length = 3 Then
                                                Dim sIndex As Integer = roTypes.Any2Integer(substringParts(1)) - 1
                                                Dim eIndex As Integer = roTypes.Any2Integer(substringParts(2))

                                                Dim sMidfield As String = substringParts(0).Replace("$", "").Replace("""", "")
                                                Dim midPart As String = fields.Find(Function(x) x.Name.ToLower = sMidfield.ToLower).CalculatedValue

                                                fValue = fValue & midPart.Substring(sIndex, eIndex)
                                            Else
                                                'No se parsea la fecha
                                            End If
                                        Else
                                            fValue = fValue & rPart.Replace("""", "")
                                        End If
                                    End If
                                Next

                                oEntryFields.Add(New roConnectorParameter() With {
                                       .Type = roConnectorParameterType.DBField,
                                       .Name = oField.Name,
                                       .CalculatedValue = fValue
                                   })

                            End If
                        ElseIf oField.Value.ToLower.StartsWith("mid(") Then
                            If oField.Name = "IDCard" Then
                                Dim fValue As String = String.Empty
                                Dim substringParts As String() = oField.Value.ToLower.Replace("mid(", "").Replace(")", "").Split(",")

                                If substringParts.Length = 2 OrElse substringParts.Length = 3 Then
                                    Dim sIndex As Integer = -1
                                    Dim eIndex As Integer = -1

                                    sIndex = roTypes.Any2Integer(substringParts(1)) - 1
                                    If substringParts.Length = 3 Then
                                        eIndex = roTypes.Any2Integer(substringParts(2))
                                    End If

                                    Dim sMidfield As String = substringParts(0).Replace("$", "").Replace("""", "")
                                    Dim midPart As String = fields.Find(Function(x) x.Name.ToLower = sMidfield.ToLower).CalculatedValue

                                    If substringParts.Length = 3 Then
                                        fValue = fValue & midPart.Substring(sIndex, eIndex)
                                    Else
                                        fValue = fValue & midPart.Substring(sIndex)
                                    End If
                                Else
                                    eInvalidReason = InvalidEntryReason.InvalidFormat
                                    bEntryIsValid = False
                                End If

                                oEntryFields.Add(New roConnectorParameter() With {
                                       .Type = roConnectorParameterType.DBField,
                                       .Name = oField.Name,
                                       .CalculatedValue = fValue
                                   })

                            End If
                        Else
                            oEntryFields.Add(New roConnectorParameter() With {
                               .Type = roConnectorParameterType.DBField,
                               .Name = oField.Name,
                               .CalculatedValue = oField.Value
                           })

                        End If

                    Next

                    Dim bIDCardIsIDEmployee As Boolean = False
                    Dim idCard As String = String.Empty
                    Dim oCardParam As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "idcard")

                    If oCardParam Is Nothing Then
                        Dim sUSRFieldName As String
                        Dim sUSRFieldValue As String

                        Dim oNifField As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "nif")
                        Dim oNifFieldName As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "nif_fieldname")

                        Dim oUSRField As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "usr_fieldvalue")
                        Dim oUSRFieldName As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "usr_fieldname")

                        Dim oUsesIDEmployee As roConnectorParameter = oEntryFields.Find(Function(x) x.Name.ToLower = "usesidemployee")

                        Dim sLogFieldValue As String = String.Empty
                        If (oNifField IsNot Nothing AndAlso oNifFieldName IsNot Nothing) OrElse (oUSRFieldName IsNot Nothing) Then
                            If oUSRFieldName IsNot Nothing Then
                                ' Identificamos por campo de la ficha. Recupero cual ...
                                sUSRFieldName = roTypes.Any2String(oUSRFieldName.CalculatedValue)
                                sUSRFieldValue = roTypes.Any2String(oUSRField.CalculatedValue)
                            Else
                                ' Identificamos por NIF. Aunque es un campo de la ficha, lo mantengo así por compatibilidad
                                sUSRFieldName = oNifFieldName.CalculatedValue
                                sUSRFieldValue = oNifField.CalculatedValue
                            End If

                            If oUsesIDEmployee IsNot Nothing Then
                                If roTypes.Any2Long(oUsesIDEmployee.CalculatedValue) <> 0 Then
                                    bIDCardIsIDEmployee = True
                                    oUsesIDEmployee.CalculatedValue = 1
                                Else
                                    oUsesIDEmployee.CalculatedValue = 0
                                End If
                            End If

                            If bIDCardIsIDEmployee Then
                                ' Se identificará al empleado por su id de empleado y no por su tarjeta.
                                sSql = "@SELECT# top(1) IDEmployee FROM dbo.EmployeeUserFieldValues where FieldName='" & sUSRFieldName & "' and substring(value,1,20)='" & sUSRFieldValue & "' "
                                idCard = roTypes.Any2String(ExecuteScalar(sSql))
                                If idCard = "0" OrElse idCard = "" Then
                                    roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::SaveEntry: Unknow NIF:'" & sUSRFieldValue & "'")
                                    eInvalidReason = InvalidEntryReason.UnknownEmployee
                                    bEntryIsValid = False
                                End If
                            Else
                                sSql = "@SELECT# top(1) dbo.sysroPassports_AuthenticationMethods.Credential " &
                                        "FROM dbo.EmployeeUserFieldValues INNER JOIN " &
                                        "dbo.sysroPassports ON dbo.EmployeeUserFieldValues.IDEmployee = dbo.sysroPassports.IDEmployee INNER JOIN " &
                                        "dbo.sysroPassports_AuthenticationMethods ON dbo.sysroPassports.ID = dbo.sysroPassports_AuthenticationMethods.IDPassport " &
                                        "where FieldName='" & sUSRFieldName & "' and substring(value,1,20)='" & sUSRFieldValue & "' " &
                                        "and dbo.sysroPassports_AuthenticationMethods.Method = 3"

                                idCard = roTypes.Any2String(ExecuteScalar(sSql))

                                ' Mira si está en el card aliases
                                Dim IDCardAliases = roTypes.Any2String(ExecuteScalar("@SELECT# RealValue FROM CardAliases WHERE CONVERT(VARCHAR,idCard) = '" & idCard & "'"))
                                If IDCardAliases <> "" Then idCard = IDCardAliases
                                If idCard = "0" OrElse idCard = "" Then
                                    roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roPunchConnectorManager::SaveEntry: Unknow NIF:'" & sUSRFieldValue & "'")
                                    eInvalidReason = InvalidEntryReason.UnknownEmployee
                                    bEntryIsValid = False
                                End If
                            End If

                            oEntryFields.Add(New roConnectorParameter() With {
                                                   .Type = roConnectorParameterType.DBField,
                                                   .Name = "idCard",
                                                   .CalculatedValue = idCard
                                               })
                        End If
                    End If

                    ' Verficio que no sea fichaje futuro
                    If Now.Date.Subtract(roTypes.Any2DateTime(oEntryFields.Find(Function(x) x.Name.ToLower = "datetime").CalculatedValue)).Days < 0 Then
                        bEntryIsValid = False
                        eInvalidReason = InvalidEntryReason.FuturePunch
                    End If
                Catch ex As Exception
                    bEntryIsValid = False
                    eInvalidReason = InvalidEntryReason.InvalidFormat
                End Try

                If bEntryIsValid AndAlso oEntryFields IsNot Nothing Then

                    Dim oEntry As New VTBusiness.Move.roEntry
                    Try

                        oEntry.IDReader = roTypes.Any2Integer(oEntryFields.Find(Function(x) x.Name.ToLower = "idreader").CalculatedValue)
                        oEntry._DateTime = roTypes.Any2DateTime(oEntryFields.Find(Function(x) x.Name.ToLower = "datetime").CalculatedValue)
                        oEntry.IDCard = roTypes.Any2Long(oEntryFields.Find(Function(x) x.Name.ToLower = "idcard").CalculatedValue)
                        If dicDictionary.Count > 0 Then
                            oEntry.Type = dicDictionary(roTypes.Any2String(oEntryFields.Find(Function(x) x.Name.ToLower = "type").CalculatedValue).Trim)
                        Else
                            oEntry.Type = roTypes.Any2String(oEntryFields.Find(Function(x) x.Name.ToLower = "type").CalculatedValue).Trim
                        End If

                        If oEntryFields.Find(Function(x) x.Name.ToLower = "idcause") IsNot Nothing Then
                            oEntry.IDCause = roTypes.Any2Integer(oEntryFields.Find(Function(x) x.Name.ToLower = "idcause").CalculatedValue)
                        Else
                            oEntry.IDCause = 0
                        End If

                        oEntry.Rdr = roTypes.Any2Integer(oEntryFields.Find(Function(x) x.Name.ToLower = "rdr").CalculatedValue)

                        If oEntryFields.Find(Function(x) x.Name.ToLower = "usesidemployee") IsNot Nothing Then
                            oEntry.UsesIdEmployee = roTypes.Any2Boolean(oEntryFields.Find(Function(x) x.Name.ToLower = "usesidemployee").CalculatedValue)
                        Else
                            oEntry.UsesIdEmployee = False
                        End If

                        oEntry.InvalidRead = False

                        bSaved = oEntry.Save()

                        If Not bSaved Then
                            bEntryIsValid = False
                            eInvalidReason = InvalidEntryReason.Unknown
                        End If
                    Catch ex As Exception
                        bEntryIsValid = False
                        eInvalidReason = InvalidEntryReason.Unknown
                    End Try
                End If

                If Not bEntryIsValid Then
                    roLog.GetInstance.logMessage(roLog.EventType.roDebug, $"roPunchConnectorManager::SaveEntry:Invalid entry: {oEntryLine} {entryLineSerialized}")
                    Dim oLanguage As New roLanguage
                    oLanguage.SetLanguageReference("ProcessCollector", "ESP")
                    Dim strError As String = oLanguage.Translate("Process." & eInvalidReason.ToString & ".Text", "Collector")
                    sSql = "@INSERT# INTO InvalidEntries ([Processed],[RawData],[Behavior],[CreatedOn],[ErrorText]) VALUES ('N','" & oEntryLine & "','Behavior " & strTemplateFileName.Split(".")(0) & "', GETDATE(), '" & strError & "')"
                    ExecuteSql(sSql)
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunchConnectorManager::SaveEntry::Unhandled exception::", ex)
                bSaved = False
            End Try

            Return bSaved
        End Function

#End Region

    End Class

End Namespace