Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Employee

    <DataContract>
    <Serializable>
    Public Class roMobility

        Private IntIdGroup As Integer
        Private StrName As String
        Private DatBeginDate As DateTime
        Private DatEndDate As Date
        Private BolIsTransfer As Boolean

#Region "Properties"

        <DataMember>
        Public Property IdGroup() As Integer
            Get
                Return IntIdGroup
            End Get
            Set(ByVal value As Integer)
                IntIdGroup = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return StrName
            End Get
            Set(ByVal value As String)
                StrName = value
            End Set
        End Property

        <DataMember>
        Public Property BeginDate() As DateTime
            Get
                Return DatBeginDate
            End Get
            Set(ByVal value As DateTime)
                DatBeginDate = value
            End Set
        End Property

        <DataMember>
        Public Property EndDate() As DateTime
            Get
                Return DatEndDate
            End Get
            Set(ByVal value As DateTime)
                DatEndDate = value
            End Set
        End Property

        <DataMember>
        Public Property IsTransfer() As Boolean
            Get
                Return BolIsTransfer
            End Get
            Set(ByVal value As Boolean)
                BolIsTransfer = value
            End Set
        End Property

#End Region

#Region "Helpers"

        Public Shared Function GenerateNotificationsForEmployeeMobility(ByVal IdEmployee As Integer, ByVal BeginDate As DateTime, ByVal IdGroup As Nullable(Of Integer), ByRef oState As roEmployeeState) As Boolean

            Dim bRet As Boolean = False
            Dim strSQL As String = String.Empty

            Try

                If IdGroup IsNot Nothing AndAlso BeginDate.Date = DateTime.Now.Date Then
                    ' Si es una movilidad nueva y empieza hoy mismo, genero la notificación especifica
                    strSQL = "@SELECT# ID, ShowOnDesktop FROM Notifications Where Activated=1 AND IDType = 44 "

                    Dim dtNotifications As DataTable = CreateDataTable(strSQL)
                    Dim sFiredDate As String = "NULL"
                    If dtNotifications IsNot Nothing AndAlso dtNotifications.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtNotifications.Rows
                            If roTypes.Any2Boolean(oRow("ShowOnDesktop")) = True Then
                                sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                            Else
                                sFiredDate = "NULL"
                            End If

                            strSQL = "@SELECT# COUNT(*) " &
                                " From sysroNotificationTasks " &
                                " Where sysroNotificationTasks.Key1Numeric = " & IdEmployee &
                                " AND sysroNotificationTasks.Key3Datetime  = " & roTypes.Any2Time(BeginDate).SQLDateTime &
                                " AND sysroNotificationTasks.Key2Numeric = " & roTypes.Any2Double(IdGroup.Value) &
                                " AND IDNotification=" & roTypes.Any2Double(oRow("ID"))

                            If roTypes.Any2Integer(ExecuteScalar(strSQL)) = 0 Then
                                strSQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric, FiredDate) VALUES (" &
                                    oRow("ID") & "," & IdEmployee & "," & roTypes.Any2Time(BeginDate).SQLDateTime & "," & roTypes.Any2Double(IdGroup.Value) & ", " & sFiredDate & ")"

                                ExecuteScalar(strSQL)
                            End If
                        Next
                    End If
                Else
                    If IdGroup Is Nothing Then
                        ' Si no tengo grupo genero la notificación de movilidades modificadas
                        strSQL = "@SELECT# ID, ShowOnDesktop  FROM Notifications Where Activated=1 AND IDType = 43"
                        Dim dtNotifications As DataTable = CreateDataTable(strSQL)
                        Dim sFiredDate As String = "NULL"
                        If dtNotifications IsNot Nothing AndAlso dtNotifications.Rows.Count > 0 Then
                            For Each oRow As DataRow In dtNotifications.Rows
                                If roTypes.Any2Boolean(oRow("ShowOnDesktop")) = True Then
                                    sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                Else
                                    sFiredDate = "NULL"
                                End If

                                strSQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate) VALUES (" &
                                        oRow("ID") & "," & IdEmployee & "," & roTypes.Any2Time(BeginDate).SQLDateTime & ", " & sFiredDate & ")"

                                ExecuteScalar(strSQL)
                            Next
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GenerateNotificationsForEmployeeMobility")
                oState.Result = EmployeeResultEnum.ConnectionError
            End Try

            Return bRet

        End Function

        Public Shared Function GetCurrentMobility(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState) As roMobility
            ' Devuelve un una clase con los datos del grupo actual del empleado
            Dim oCurrentMobility As roMobility = Nothing

            Try

                oState.UpdateStateInfo()

                Dim strSQL As String
                strSQL = "@SELECT# IDGroup, Groups.Name, BeginDate, EndDate, IsTransfer " &
                       "FROM EmployeeGroups " &
                                "INNER JOIN Groups ON Groups.ID = EmployeeGroups.IDGroup " &
                       "WHERE EmployeeGroups.Begindate <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                             "EmployeeGroups.Enddate >= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                             "EmployeeGroups.IDEmployee = " & IDEmployee
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    If tb.Rows.Count > 0 Then
                        oCurrentMobility = New roMobility
                        oCurrentMobility.IdGroup = tb.Rows(0)("IdGroup")
                        oCurrentMobility.Name = tb.Rows(0)("Name")
                        oCurrentMobility.BeginDate = tb.Rows(0)("BeginDate")
                        oCurrentMobility.EndDate = tb.Rows(0)("EndDate")
                        oCurrentMobility.IsTransfer = tb.Rows(0)("IsTransfer")
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetCurrentMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetCurrentMobility")
            Finally

            End Try

            Return oCurrentMobility

        End Function

        Public Shared Function GetNextMobility(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState) As roMobility
            ' Devuelve un una clase con los datos del grupo actual del empleado
            Dim oCurrentMobility As roMobility = Nothing

            Try

                oState.UpdateStateInfo()

                Dim strSQL As String
                strSQL = "@SELECT# top(1) IDGroup, Groups.Name, BeginDate, EndDate, IsTransfer " &
                     "FROM EmployeeGroups " &
                     "  INNER JOIN Groups ON Groups.ID = EmployeeGroups.IDGroup " &
                     "WHERE EmployeeGroups.Begindate >= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime &
                     " AND EmployeeGroups.IDEmployee = " & IDEmployee & " " &
                     "ORDER BY BeginDate "

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    If tb.Rows.Count > 0 Then
                        oCurrentMobility = New roMobility
                        oCurrentMobility.IdGroup = tb.Rows(0)("IdGroup")
                        oCurrentMobility.Name = tb.Rows(0)("Name")
                        oCurrentMobility.BeginDate = tb.Rows(0)("BeginDate")
                        oCurrentMobility.EndDate = tb.Rows(0)("EndDate")
                        oCurrentMobility.IsTransfer = tb.Rows(0)("IsTransfer")
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetNextMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetNextMobility")
            Finally

            End Try

            Return oCurrentMobility

        End Function

        Public Shared Function GetMobility(ByVal IDEmployee As Integer, ByVal IdGroup As Integer, ByRef oState As roEmployeeState) As roMobility
            ' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro
            Dim strQuery As String
            Dim oCurrentMobility As roMobility = Nothing
            Dim oDataSet As System.Data.DataSet
            Dim oDatareader As System.Data.Common.DbDataReader

            oState.UpdateStateInfo()

            Try

                strQuery = "@SELECT# IDGroup, Groups.Name, BeginDate, EndDate, IsTransfer "
                strQuery = strQuery & " From EmployeeGroups "
                strQuery = strQuery & " Inner Join Groups On "
                strQuery = strQuery & " Groups.ID = EmployeeGroups.IDGroup "
                strQuery = strQuery & " And Groups.ID = " & IdGroup
                strQuery = strQuery & " Where "
                strQuery = strQuery & " EmployeeGroups.IDEmployee = " & IDEmployee
                strQuery = strQuery & " Order By BeginDate Desc "

                oDataSet = CreateDataSet(strQuery) ' Ejecuto la sql

                If oDataSet IsNot Nothing Then

                    oDatareader = oDataSet.CreateDataReader

                    If oDatareader IsNot Nothing Then
                        oDatareader.Read()
                        If oDatareader.HasRows Then
                            oCurrentMobility = New roMobility
                            oCurrentMobility.IdGroup = oDatareader("IdGroup")
                            oCurrentMobility.Name = oDatareader("Name")
                            oCurrentMobility.BeginDate = oDatareader("BeginDate")
                            oCurrentMobility.EndDate = oDatareader("EndDate")
                            oCurrentMobility.IsTransfer = oDatareader("IsTransfer")
                        End If
                    End If

                    oDatareader.Close() ' Cierro el DataReader

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMobility")
            End Try

            Return oCurrentMobility

        End Function

        Public Shared Function GetMobilities(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState) As System.Data.DataTable
            ' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro
            Dim strQuery As String
            Dim oDataTable As System.Data.DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                strQuery = "@SELECT# IDGroup, Groups.Name, BeginDate, EndDate, IsTransfer, Groups.FullGroupName "
                strQuery = strQuery & " From EmployeeGroups "
                strQuery = strQuery & " Inner Join Groups On "
                strQuery = strQuery & " Groups.ID = EmployeeGroups.IDGroup "
                strQuery = strQuery & " Where "
                strQuery = strQuery & " EmployeeGroups.IDEmployee = " & IDEmployee
                strQuery = strQuery & " Order By BeginDate "

                oDataTable = CreateDataTable(strQuery, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMobilities")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMobilities")
            Finally

            End Try

            Return oDataTable

        End Function

        Public Shared Function ValidateMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByRef oState As roEmployeeState) As Boolean
            ' Valida los datos de la estructura de movilidad (el valor nombre no se mira)
            Dim strQuery As String
            Dim oDatatable As System.Data.DataTable

            oState.UpdateStateInfo()

            ' El grupo tiene que existir
            If Mobility.IdGroup > 0 Then

                strQuery = "@SELECT# * From Groups"
                strQuery = strQuery & " Where Id = " & Mobility.IdGroup

                Try

                    oDatatable = CreateDataTable(strQuery, )
                    If Not oDatatable.CreateDataReader.HasRows Then
                        oState.Result = EmployeeResultEnum.IDGroupNotExist
                    End If
                Catch ex As DbException
                    oState.UpdateStateInfo(ex, "roEmployees::ValidateMobility")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roEmployees::ValidateMobility")
                Finally

                End Try
            Else
                oState.Result = EmployeeResultEnum.InvalidIDGroup
            End If

            If oState.Result = EmployeeResultEnum.NoError Then
                ' La fecha inicio tiene que ser menor que la de fin
                If Mobility.BeginDate > Mobility.EndDate Then
                    oState.Result = EmployeeResultEnum.InvalidDateInterval
                End If
            End If

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Public Shared Function ValidateMobilities(ByVal IDEmployee As Integer, ByRef dsMobilities As DataSet, ByRef intInvalidRow As Integer, ByRef oState As roEmployeeState) As Boolean
            ' Valida los datos de dataset (el valor nombre no se mira)
            ' Campos tabla 0 del dataset: IDGroup, BeginDate, EndDate
            ' Modifica la columna 'EndDate' con el valor correcto

            oState.UpdateStateInfo()

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
                            oState.Result = EmployeeResultEnum.MobilityBadBeginDate
                            intInvalidRow = nRow
                            Exit For
                        End If

                        xBeginDate = CDate(oRow("BeginDate"))

                        'Si la fecha actual es anterior al inicio del contrato, devolvemos el error
                        If Not roBusinessSupport.EmployeeWithContract(IDEmployee, oState, xBeginDate) Then
                            oState.Result = EmployeeResultEnum.MobilityInvalidBeginDate
                            intInvalidRow = nRow
                            Exit For
                        End If

                        'Si no tenemos grupo no dejamos actualizar
                        If IsDBNull(oRow("IDGroup")) OrElse oRow("IDGroup") <= 0 Then
                            oState.Result = EmployeeResultEnum.MobilityNoGroup
                            intInvalidRow = nRow
                            Exit For
                        End If

                        If Not xLastBeginDate.HasValue Then ' Si és el primer registro
                            'Primer registro, este no puede ser distinto a la primera fecha de contrato
                            Dim xBeginContract As DateTime = roBusinessSupport.FirstContractDate(IDEmployee, oState)
                            If oState.Result = EmployeeResultEnum.NoError Then
                                If xBeginDate > xBeginContract Then
                                    oState.Result = EmployeeResultEnum.MobilityDifferentContractDate
                                    intInvalidRow = nRow
                                    Exit For
                                End If
                            End If
                        Else
                            'Si la fecha de inicio de la fila actual es igual a la fecha de inicio de
                            'la ultima fila, devolvemos el error
                            If xBeginDate = xLastBeginDate.Value Then
                                oState.Result = EmployeeResultEnum.MobilityDuplicateStartDate
                                intInvalidRow = nRow
                                Exit For
                            End If
                            'Si el grupo es el mismo que el grupo de la fila anterior, mostramos el error
                            If oRow("IDGroup") = intLastIDGroup Then
                                oState.Result = EmployeeResultEnum.MobilityDuplicateGroup
                                intInvalidRow = nRow
                                Exit For
                            End If
                        End If

                        'Si no estamos en la ultima fila
                        If nRow < oMobilityRows.Length - 1 Then
                            Dim oNextRow As DataRow = oMobilityRows(nRow + 1)
                            'Comprueba que la siguiente fecha sea correcta
                            If IsDBNull(oNextRow) Then
                                oState.Result = EmployeeResultEnum.MobilityBadBeginDate
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
                    oState.Result = EmployeeResultEnum.MobilityDifferentContractDate
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::ValidateMobilities")
            Catch Ex As Exception
                oState.UpdateStateInfo(Ex, "roEmployees::ValidateMobilities")
            End Try

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Public Shared Function SaveMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByRef oState As roEmployeeState, ByVal CallBroadcaster As Boolean, Optional ByVal bIsNewEmployee As Boolean = False) As Boolean
            Dim oDataTable As DataTable
            Dim oDatarow As DataRow
            Dim AuxMobility As roMobility
            Dim bolAllOK As Boolean

            oState.UpdateStateInfo()

            bolAllOK = True

            ' Inicializo la fecha de fin a 01/01/2079
            Mobility.EndDate = CDate("01/01/2079")

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Recupero las mobilidades de este empleado
                oDataTable = GetMobilities(IDEmployee, oState)
                If oState.Result = EmployeeResultEnum.NoError Then

                    If oDataTable IsNot Nothing Then

                        ' Busco los registros hasta encontrar una fecha fin igual o mayor que la fecha de inicio que me han pasado por parametro
                        For Each oDatarow In oDataTable.Rows
                            If oDatarow("EndDate") >= Mobility.BeginDate Then

                                AuxMobility = New roMobility
                                AuxMobility.BeginDate = oDatarow("BeginDate")
                                AuxMobility.EndDate = Mobility.BeginDate.AddDays(-1)
                                AuxMobility.IdGroup = oDatarow("idGroup")
                                AuxMobility.Name = oDatarow("Name")
                                AuxMobility.IsTransfer = oDatarow("IsTransfer")

                                SaveSingleMobility(IDEmployee, AuxMobility, oState, CallBroadcaster)

                                bolAllOK = (oState.Result = EmployeeResultEnum.NoError)

                            ElseIf oDatarow("BeginDate") <= Mobility.EndDate Then
                                Mobility.EndDate = CDate(oDatarow("BeginDate")).AddDays(-1)
                            End If
                        Next

                    End If

                    If bolAllOK Then
                        If Not bIsNewEmployee Then roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, Mobility.BeginDate, Mobility.IdGroup, oState)
                        SaveSingleMobility(IDEmployee, Mobility, oState, CallBroadcaster)
                        bolAllOK = (oState.Result = EmployeeResultEnum.NoError)
                    End If

                    If bolAllOK Then
                        If Not bIsNewEmployee Then roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, DateTime.Now, Nothing, oState)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveMobility")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, True)
            End Try

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Public Shared Function SaveSingleMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByRef oState As roEmployeeState, ByVal CallBroadcaster As Boolean) As Boolean
            ' Guarda los datos de movilidad
            Dim strQuery As String
            Dim oDataTable As System.Data.DataTable

            ' Miramos si hay la licencia de prevención de riesgos laborales
            Dim oServerLicense As New roServerLicense
            Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")
            ' Obtenemos la mobilidad actual
            Dim oCurrenttMobOld As roMobility = roMobility.GetCurrentMobility(IDEmployee, oState)

            oState.UpdateStateInfo()

            ValidateMobility(IDEmployee, Mobility, oState)

            If oState.Result = EmployeeResultEnum.NoError Then

                Dim strQueryRow As String = ""
                Dim oEmployeeGroupOld As DataRow = Nothing
                Dim oEmployeeGroupNew As DataRow = Nothing

                ' Obtengo los datos actuales de la mobilidad
                strQueryRow = "@SELECT# EmployeeGroups.*, Employees.Name AS EmployeeName, Groups.Name as GroupName, Groups.Path  " &
                          "FROM EmployeeGroups INNER JOIN Employees " &
                                    "ON EmployeeGroups.IDEmployee = Employees.[ID] " &
                                    "INNER JOIN Groups " &
                                    "ON EmployeeGroups.IDGroup = Groups.[ID] " &
                          "WHERE EmployeeGroups.IDEmployee = " & IDEmployee & " AND " &
                                "EmployeeGroups.IDGroup = " & Mobility.IdGroup & " AND " &
                                "EmployeeGroups.BeginDate = " & roTypes.Any2Time(Mobility.BeginDate).SQLSmallDateTime
                Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "EmployeeGroups")
                If tbAuditOld.Rows.Count = 1 Then oEmployeeGroupOld = tbAuditOld.Rows(0)

                ' Monto una sql para saber si el registro existe
                strQuery = " @SELECT# * from EmployeeGroups "
                strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                strQuery = strQuery & " And IDGroup = " & Mobility.IdGroup
                strQuery = strQuery & " And BeginDate = " & roTypes.Any2Time(Mobility.BeginDate).SQLSmallDateTime & " "

                oDataTable = CreateDataTable(strQuery)

                Dim bolNew As Boolean = Not oDataTable.CreateDataReader.HasRows()

                If Not bolNew Then
                    ' Es un update
                    strQuery = " @UPDATE# EmployeeGroups "
                    strQuery = strQuery & " Set EndDate = " & roTypes.Any2Time(Mobility.EndDate).SQLSmallDateTime & " "
                    strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                    strQuery = strQuery & " And IDGroup = " & Mobility.IdGroup
                    strQuery = strQuery & " And BeginDate = " & roTypes.Any2Time(Mobility.BeginDate).SQLSmallDateTime & " "
                    strQuery = strQuery & " And IsTransfer = " & IIf(Mobility.IsTransfer, "1", "0") & " "
                Else
                    ' Es un insert
                    strQuery = " @INSERT# INTO EmployeeGroups "
                    strQuery = strQuery & " (IDEmployee, IdGroup, "
                    strQuery = strQuery & " BeginDate, EndDate, IsTransfer ) "
                    strQuery = strQuery & " Values "
                    strQuery = strQuery & " ( " & IDEmployee & " , " & Mobility.IdGroup & ", "
                    strQuery = strQuery & " " & roTypes.Any2Time(Mobility.BeginDate).SQLSmallDateTime & " , " & roTypes.Any2Time(Mobility.EndDate).SQLSmallDateTime & " , " & IIf(Mobility.IsTransfer, "1", "0") & ") "
                End If

                If Not ExecuteSql(strQuery) Then
                    oState.Result = EmployeeResultEnum.ConnectionError
                End If

                If oState.Result = EmployeeResultEnum.NoError Then
                    roTimeStamps.UpdateEmployeeTimestamp(IDEmployee)
                    strQueryRow = "@SELECT# EmployeeGroups.*, Employees.Name AS EmployeeName, Groups.Name as GroupName, Groups.Path " &
                              "FROM EmployeeGroups INNER JOIN Employees " &
                                        "ON EmployeeGroups.IDEmployee = Employees.[ID] " &
                                        "INNER JOIN Groups " &
                                        "ON EmployeeGroups.IDGroup = Groups.[ID] " &
                              "WHERE EmployeeGroups.IDEmployee = " & IDEmployee & " AND " &
                                    "EmployeeGroups.IDGroup = " & Mobility.IdGroup & " AND " &
                                    "EmployeeGroups.BeginDate = " & roTypes.Any2Time(Mobility.BeginDate).SQLSmallDateTime
                    Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "EmployeeGroups")
                    If tbAuditNew.Rows.Count = 1 Then oEmployeeGroupNew = tbAuditNew.Rows(0)

                    ' Insertar registro auditoria
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditFieldsValues(tbParameters, oEmployeeGroupNew, oEmployeeGroupOld)
                    Dim oAuditAction As Audit.Action = IIf(oEmployeeGroupOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String
                    If oAuditAction = Audit.Action.aInsert Then
                        strObjectName = oEmployeeGroupNew("GroupName")
                    ElseIf oEmployeeGroupOld("GroupName") <> oEmployeeGroupNew("GroupName") Then
                        strObjectName = oEmployeeGroupOld("GroupName") & " -> " & oEmployeeGroupOld("GroupName")
                    Else
                        strObjectName = oEmployeeGroupNew("GroupName")
                    End If
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmployeeGroupNew("EmployeeName"), "", 1)
                    oState.Audit(oAuditAction, Audit.ObjectType.tEmployeeGroup,
                             strObjectName & " (" & oEmployeeGroupNew("EmployeeName") & ")",
                             tbParameters, -1)

                    ' Miramos si hemos de lanzar el broadcaster para regenerar los ficheros de prevención (OHP)
                    If bolOHPLicense Then

                        ' Obtenemos la mobilidad actual
                        Dim oCurrenttMobNew As roMobility = roMobility.GetCurrentMobility(IDEmployee, oState)

                        Dim bolBroadcaster As Boolean = False
                        ' Miramos si es necesario ejecutar el broadcaster
                        If oCurrenttMobOld Is Nothing Then
                            bolBroadcaster = True
                        ElseIf oCurrenttMobOld.IdGroup <> oCurrenttMobNew.IdGroup Then
                            ' Solo lanzamos el broadcaster si ha cambiado la empresa
                            If roBusinessSupport.GetGroupPath(oCurrenttMobOld.IdGroup, oState).Split("\")(0) <> roBusinessSupport.GetGroupPath(oCurrenttMobNew.IdGroup, oState).Split("\")(0) Then
                                bolBroadcaster = True
                            End If
                        End If

                        If bolBroadcaster And CallBroadcaster Then
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If

                    End If

                End If
            End If

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Public Shared Function SaveMobilities(ByVal intIDEmployee As Integer, ByVal dsMobilities As DataSet, ByRef oState As roEmployeeState, Optional ByVal bolAudit As Boolean = False) As Boolean

            oState.UpdateStateInfo()

            Dim bolRet As Boolean = True
            Dim strSQL As String

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos la mobilidad actual
                Dim tbMobilitiesOld As DataTable = CreateDataTable("@SELECT# * FROM EmployeeGroups WHERE IDEmployee = " & intIDEmployee.ToString & " ORDER BY BeginDate")

                If dsMobilities IsNot Nothing AndAlso dsMobilities.Tables.Count > 0 AndAlso dsMobilities.Tables(0).Rows.Count < 1 Then
                    bolRet = True
                    oState.Result = EmployeeResultEnum.LastMobilidityDeleteError
                End If

                If bolRet Then
                    ' Borrar las mobilidades actuales
                    strSQL = "@DELETE# FROM EmployeeGroups " &
                         "WHERE IDEmployee = " & intIDEmployee.ToString
                    If Not ExecuteSql(strSQL) Then
                        oState.Result = EmployeeResultEnum.ConnectionError
                    End If

                    If oState.Result = EmployeeResultEnum.NoError Then

                        If dsMobilities IsNot Nothing AndAlso dsMobilities.Tables.Count > 0 Then

                            Dim moveToday As Boolean = False
                            Dim moveGroup As Integer = -1

                            Dim oCount As Integer = dsMobilities.Tables(0).Rows.Count
                            Dim iIndex As Integer = 1
                            For Each oRow As DataRow In dsMobilities.Tables(0).Select("", "BeginDate ASC")

                                If roTypes.Any2DateTime(oRow("BeginDate")).Date = DateTime.Now.Date Then
                                    moveToday = True
                                    moveGroup = roTypes.Any2Integer(oRow("IDGroup"))
                                End If

                                'Si es la última movilidad quitamos la fecha de fin, ya que sino podemos perder el empleado en el árbol
                                If iIndex = oCount Then
                                    strSQL = "@INSERT# INTO EmployeeGroups(IDEmployee, IDGroup, BeginDate, EndDate, IsTransfer) " &
                                     "VALUES(" & intIDEmployee.ToString & ", " & oRow("IDGroup") & ", " &
                                             roTypes.Any2Time(oRow("BeginDate")).SQLSmallDateTime & ", '20790101', " & oRow("IsTransfer") & ")"
                                Else
                                    strSQL = "@INSERT# INTO EmployeeGroups(IDEmployee, IDGroup, BeginDate, EndDate, IsTransfer) " &
                                     "VALUES(" & intIDEmployee.ToString & ", " & oRow("IDGroup") & ", " &
                                             roTypes.Any2Time(oRow("BeginDate")).SQLSmallDateTime & ", " & roTypes.Any2Time(oRow("EndDate")).SQLSmallDateTime & ", " & oRow("IsTransfer") & ")"
                                End If

                                If Not ExecuteSql(strSQL) Then
                                    oState.Result = EmployeeResultEnum.ConnectionError
                                    Exit For
                                End If
                                iIndex = iIndex + 1
                            Next

                            If oState.Result = EmployeeResultEnum.NoError Then roMobility.GenerateNotificationsForEmployeeMobility(intIDEmployee, DateTime.Now, Nothing, oState)
                            If oState.Result = EmployeeResultEnum.NoError AndAlso moveToday Then roMobility.GenerateNotificationsForEmployeeMobility(intIDEmployee, DateTime.Now.Date, moveGroup, oState)

                            If oState.Result = EmployeeResultEnum.NoError AndAlso bolAudit Then
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                Dim empName As String = roBusinessSupport.GetEmployeeName(intIDEmployee, oState)
                                Dim oCurrentMobility As roMobility = GetCurrentMobility(intIDEmployee, oState)
                                Dim actualGroup As String = ""
                                If oCurrentMobility IsNot Nothing AndAlso oCurrentMobility.IdGroup > 0 Then
                                    actualGroup = oCurrentMobility.Name
                                End If

                                oState.AddAuditParameter(tbParameters, "{EmployeeName}", empName, "", 1)
                                oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tEmployeeGroup, "(" & actualGroup & ")" & empName, tbParameters, -1)
                            End If
                        Else
                            oState.Result = EmployeeResultEnum.MobilityInvalidData
                        End If

                    End If

                    If oState.Result = EmployeeResultEnum.NoError Then
                        roMobility.RecalculateCostCenters(intIDEmployee, tbMobilitiesOld, oState)
                        roTimeStamps.UpdateEmployeeTimestamp(intIDEmployee)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveMobilities")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveMobilities")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Public Shared Function UpdateEmployeeGroup(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal FromDate As Date, ByRef oState As roEmployeeState, Optional ByRef bLaunchBroadcaser As Boolean = True) As Boolean

            Dim oMobility As roMobility
            Dim bolProcessResult As Boolean = False
            Dim datBeginDate As Date

            oState.UpdateStateInfo()

            ' Miramos si hay la licencia de prevención de riesgos laborales
            Dim oServerLicense As New roServerLicense
            Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\OHP")

            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

            ' Si nos pasan una fecha de inicio en los parámetros
            If FromDate.ToString(oState.Language.GetShortDateFormat) = "01/01/0001" Then
                ' La fecha de final de pertenencia a un grupo es la fecha anterior al movimiento
                datBeginDate = Date.Today
            Else
                datBeginDate = FromDate
            End If

            ' Verificamos que la fecha de movilidad no esté dentro del periodo de bloqueo del empleado
            If roTypes.Any2Time(ExecuteScalar("@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = " & IDEmployee.ToString)).Value >= datBeginDate Then
                oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                Return False
                Exit Function
            End If

            ' Primero miramos si el empleado tiene previsto moverse
            Dim bolIsMoving As Boolean = IsMovingEmployee(IDEmployee, oState)
            If oState.Result = EmployeeResultEnum.NoError Then

                ' Obtenemos la mobilidad actual
                Dim oCurrenttMobOld As roMobility = roMobility.GetCurrentMobility(IDEmployee, oState)

                Dim tbMobilitiesOld As DataTable = CreateDataTable("@SELECT# * FROM EmployeeGroups WHERE IDEmployee = " & IDEmployee.ToString & " ORDER BY BeginDate")

                If bolIsMoving Then
                    ' El movimiento actual debe sustituir al movimiento previsto anterior
                    oMobility = GetCurrentMobility(IDEmployee, oState)

                    ' Si no hay movimiento actual, busca el siguiente
                    If oMobility Is Nothing Then oMobility = GetNextMobility(IDEmployee, oState)

                    If oState.Result = EmployeeResultEnum.NoError Then

                        If oMobility IsNot Nothing Then
                            If oMobility.IdGroup = IDGroup Then 'Queremos cancelar el movimiento programado
                                bolProcessResult = AddNewMobility(IDEmployee, IDGroup, FromDate, oState)
                            Else
                                bolProcessResult = AddNewMobility(IDEmployee, IDGroup, FromDate, oState)
                            End If
                        End If

                    End If
                Else
                    bolProcessResult = AddNewMobility(IDEmployee, IDGroup, FromDate, oState)
                End If

                If bolProcessResult Then

                    ' Obtenemos la mobilidad actual
                    Dim oCurrenttMobNew As roMobility = roMobility.GetCurrentMobility(IDEmployee, oState)

                    ' Timestamps
                    If oCurrenttMobOld IsNot Nothing AndAlso oCurrenttMobOld.IdGroup <> IDGroup Then
                        VTBase.Extensions.roTimeStamps.UpdateEmployeeTimestamp(IDEmployee)
                    End If

                    ' Miramos si hemos de lanzar el broadcaster para regenerar los ficheros de prevención (OHP)
                    If bolOHPLicense Then
                        If bLaunchBroadcaser Then
                            Dim bolBroadcaster As Boolean = False
                            ' Miramos si es necesario ejecutar el broadcaster
                            If oCurrenttMobOld Is Nothing Then
                                bolBroadcaster = True
                            ElseIf oCurrenttMobOld.IdGroup <> oCurrenttMobNew.IdGroup Then
                                ' Solo lanzamos el broadcaster si ha cambiado la empresa
                                If roBusinessSupport.GetGroupPath(oCurrenttMobOld.IdGroup, oState).Split("\")(0) <> roBusinessSupport.GetGroupPath(oCurrenttMobNew.IdGroup, oState).Split("\")(0) Then
                                    bolBroadcaster = True
                                End If
                            End If

                            If bolBroadcaster Then
                                roConnector.InitTask(TasksType.BROADCASTER)
                            End If
                        End If
                    End If

                    'Me.RecalculateDailyCoverages(IDEmployee, tbMobilitiesOld, oState, oTransaction)
                    roMobility.RecalculateCostCenters(IDEmployee, tbMobilitiesOld, oState)
                End If

            End If

            Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolProcessResult)

            Return (oState.Result = EmployeeResultEnum.NoError And bolProcessResult)

        End Function

        Public Shared Function AddNewMobility(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal FromDate As Date, ByRef oState As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            oState.UpdateStateInfo()

            Try
                Dim xBeginDate As Date
                Dim xEndDate As Date = New Date(2079, 1, 1)

                ' Si nos pasan una fecha de inicio en los parámetros
                If FromDate.ToString(oState.Language.GetShortDateFormat) = "01/01/0001" Then
                    ' La fecha de final de pertenencia a un grupo es la fecha anterior al movimiento
                    xBeginDate = Now.Date
                Else
                    xBeginDate = FromDate.Date
                End If

                Dim bolAddMobility As Boolean = True

                Dim strSQL As String

                ' Buscamos la movilidad que contenga la fecha de inicio
                strSQL = "@SELECT# * FROM EmployeeGroups " &
                     "WHERE IDEmployee = " & IDEmployee.ToString & " AND " &
                           "BeginDate <= " & roTypes.Any2Time(xBeginDate).SQLSmallDateTime & " AND " &
                           "EndDate >= " & roTypes.Any2Time(xBeginDate).SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    If tb.Rows(0).Item("IDGroup") <> IDGroup Then

                        xEndDate = tb.Rows(0).Item("EndDate")

                        If tb.Rows(0).Item("BeginDate") < xBeginDate Then

                            strSQL = "@UPDATE# EmployeeGroups " &
                                 "SET EndDate = " & roTypes.Any2Time(xBeginDate.AddDays(-1)).SQLSmallDateTime & " " &
                                 "WHERE IDEmployee = " & IDEmployee.ToString & " AND " &
                                       "IDGroup = " & tb.Rows(0).Item("IDGroup") & " AND " &
                                       "BeginDate = " & roTypes.Any2Time(tb.Rows(0).Item("BeginDate")).SQLSmallDateTime
                            ExecuteSql(strSQL)
                        Else

                            strSQL = "@DELETE# FROM EmployeeGroups " &
                                 "WHERE IDEmployee = " & IDEmployee.ToString & " AND " &
                                       "IDGroup = " & tb.Rows(0).Item("IDGroup") & " AND " &
                                       "BeginDate = " & roTypes.Any2Time(tb.Rows(0).Item("BeginDate")).SQLSmallDateTime
                            ExecuteSql(strSQL)

                        End If
                    Else

                        bolAddMobility = False

                    End If

                End If

                If bolAddMobility Then
                    strSQL = "@INSERT# INTO EmployeeGroups(IDEmployee, IDGroup, BeginDate, EndDate, IsTransfer) " &
                             "VALUES (" & IDEmployee.ToString & ", " & IDGroup.ToString & ", " & roTypes.Any2Time(xBeginDate).SQLSmallDateTime & ", " & roTypes.Any2Time(xEndDate).SQLSmallDateTime & ",0)"
                    bolRet = ExecuteSql(strSQL)
                Else
                    bolRet = True
                End If

                If bolRet Then

                    ' Verificamos que la movilidad anterior y posterior no sea del mismo grupo.
                    Dim tbMobility As New DataTable("EmployeeGroups")
                    strSQL = "@SELECT# idEmployee, idGroup, Begindate, EndDate FROM EmployeeGroups WHERE IDEmployee = " & IDEmployee.ToString & " ORDER BY BeginDate ASC"
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbMobility)

                    If tbMobility IsNot Nothing Then

                        Dim intIDGroupAnt As Integer = -1
                        Dim xBeginDateAnt As Date

                        Dim oRow As DataRow = Nothing

                        For n As Integer = 0 To tbMobility.Rows.Count - 1
                            oRow = tbMobility.Rows(n)
                            If intIDGroupAnt = oRow("IDGroup") Then
                                tbMobility.Rows(n - 1).Delete()
                                oRow("BeginDate") = xBeginDateAnt
                            End If
                            intIDGroupAnt = oRow("IDGroup")
                            xBeginDateAnt = oRow("BeginDate")
                        Next

                        da.Update(tbMobility)

                    End If

                    If oState.Result = EmployeeResultEnum.NoError Then roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, DateTime.Now, Nothing, oState)
                    If oState.Result = EmployeeResultEnum.NoError AndAlso FromDate.Date = DateTime.Now.Date Then roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, DateTime.Now.Date, IDGroup, oState)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::AddNewMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::AddNewMobility")
            End Try

            Return bolRet

        End Function

        Public Shared Function CancelFutureMobility(ByVal IDEmployee As Integer, ByVal idGroup As Integer, ByVal FromDate As Date, ByRef oState As roEmployeeState) As Boolean
            Dim bolRet As Boolean = False
            Dim strQuery As String
            Dim oDataSet As DataSet
            Dim datStartDate As Date

            oState.UpdateStateInfo()

            Try

                strQuery = "@SELECT# BeginDate From EmployeeGroups "
                strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                strQuery = strQuery & " And EndDate = '01/01/2079' "

                oDataSet = CreateDataSet(strQuery)

                If oDataSet IsNot Nothing Then
                    If oDataSet.Tables(0).Rows.Count > 0 Then
                        datStartDate = oDataSet.Tables(0).Rows(0)("BeginDate") ' Guardamos la fecha de inicio del registro que borramos

                        strQuery = "@DELETE# EmployeeGroups "
                        strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                        strQuery = strQuery & " And EndDate = '01/01/2079' "

                        If Not ExecuteSql(strQuery) Then ' Borro el registro
                            ' Actualizamos el movimiento anterior para quitarle la fecha final

                            strQuery = " @UPDATE# EmployeeGroups "
                            strQuery = strQuery & " Set EndDate = '01/01/2079' "
                            strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                            strQuery = strQuery & " And EndDate = " & roTypes.SQLSmallDateTime(datStartDate.AddDays(-1))

                            If Not ExecuteSql(strQuery) Then ' Borro el registro
                                bolRet = True
                            Else
                                bolRet = False
                                oState.Result = EmployeeResultEnum.ConnectionError
                            End If
                        Else
                            bolRet = False
                            oState.Result = EmployeeResultEnum.ConnectionError
                        End If

                        If bolRet Then
                            roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, DateTime.Now, Nothing, oState)
                        End If

                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CancelFutureMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CancelFutureMobility")
            End Try

            Return bolRet

        End Function

        Public Shared Function ReplaceFutureMobility(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal FromDate As Date, ByRef oState As roEmployeeState) As Boolean
            Dim bolRet As Boolean = False
            Dim strQuery As String
            Dim oDataSet As DataSet
            Dim datStartDate As Date
            Dim datBeginDate As Date

            oState.UpdateStateInfo()

            Try

                ' Si nos pasan una fecha de inicio en los parámetros
                If FromDate.ToString(oState.Language.GetShortDateFormat) = "01/01/0001" Then
                    ' La fecha de final de pertenencia a un grupo es la fecha anterior al movimiento
                    datBeginDate = Date.Today
                Else
                    datBeginDate = FromDate
                End If

                strQuery = " @SELECT# BeginDate From EmployeeGroups "
                strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                strQuery = strQuery & " And EndDate = '01/01/2079' "

                oDataSet = CreateDataSet(strQuery)

                If oDataSet IsNot Nothing Then
                    If oDataSet.Tables(0).Rows.Count > 0 Then
                        datStartDate = oDataSet.Tables(0).Rows(0)("BeginDate") ' Guardamos la fecha de inicio del registro que borramos

                        strQuery = " @UPDATE# EmployeeGroups "
                        strQuery = strQuery & " Set BeginDate = '" & datBeginDate & "' "
                        strQuery = strQuery & ", IDGroup = " & IDGroup
                        strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                        strQuery = strQuery & " And EndDate = '01/01/2079' "

                        If Not ExecuteSql(strQuery) Then ' Borro el registro
                            strQuery = " @UPDATE# EmployeeGroups "
                            strQuery = strQuery & " Set EndDate = " & roTypes.SQLSmallDateTime(datBeginDate.AddDays(-1))
                            strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                            strQuery = strQuery & " And EndDate = " & roTypes.SQLSmallDateTime(datStartDate.AddDays(-1))

                            If Not ExecuteSql(strQuery) Then ' Borro el registro
                                bolRet = True
                            Else
                                bolRet = False
                                oState.Result = EmployeeResultEnum.ConnectionError
                            End If
                        Else
                            bolRet = False
                            oState.Result = EmployeeResultEnum.ConnectionError
                        End If

                        If bolRet Then
                            roMobility.GenerateNotificationsForEmployeeMobility(IDEmployee, DateTime.Now, Nothing, oState)
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::ReplaceFutureMobility")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::ReplaceFutureMobility")
            End Try

            Return bolRet

        End Function

        Public Shared Function IsMovingEmployee(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState) As Boolean
            Dim bolRet As Boolean = False
            Dim iNumEmp As Integer = 0

            oState.UpdateStateInfo()

            Try
                Dim strQuery As String = "@SELECT# Count(IDEmployee) as Counter From EmployeeGroups " &
                       "where IDEmployee = " & IDEmployee & " And BeginDate > " & roTypes.SQLSmallDateTime(Date.Today)

                iNumEmp = roTypes.Any2Double(ExecuteScalar(strQuery))

                bolRet = (iNumEmp > 0)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::IsMovingEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::IsMovingEmployee")
            End Try

            Return bolRet

        End Function

        Public Shared Function RecalculateCostCenters(ByVal _IDEmployee As Integer, ByVal tbMobilitiesOld As DataTable, ByVal _State As roEmployeeState) As Boolean
            '
            ' Recalculamos los centros de coste de las fechas de movilidades que se han modificado
            '

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim oLicense As New roServerLicense
                If oLicense.FeatureIsInstalled("Feature\CostControl") Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    'Dim xFreezingDate As Date = New Date(1900, 1, 1)
                    'Dim oParameters As New roParameters("OPTIONS", True)
                    'If oParameters.Parameter(roParameters.Parameters.FirstDate) IsNot Nothing Then
                    '    If IsDate(oParameters.Parameter(roParameters.Parameters.FirstDate)) Then
                    '        xFreezingDate = oParameters.Parameter(roParameters.Parameters.FirstDate)
                    '    End If
                    'End If

                    Dim tbMobilitiesNew As DataTable = CreateDataTable("@SELECT# * FROM EmployeeGroups WHERE IDEmployee = " & _IDEmployee.ToString & " ORDER BY BeginDate")
                    If tbMobilitiesOld IsNot Nothing AndAlso tbMobilitiesNew IsNot Nothing Then

                        Dim oRowsSelect() As DataRow
                        Dim lstUpdates As New Generic.List(Of String)

                        For Each oRowNew As DataRow In tbMobilitiesNew.Rows
                            If oRowNew.RowState <> DataRowState.Deleted And oRowNew.RowState <> DataRowState.Detached Then
                                ' Miramos si la mobilidad existía anteriormente
                                oRowsSelect = tbMobilitiesOld.Select("BeginDate = '" & Format(oRowNew("BeginDate"), "yyyy/MM/dd") & "' AND " &
                                                                     "EndDate = '" & Format(oRowNew("EndDate"), "yyyy/MM/dd") & "' AND " &
                                                                     "IDGroup = " & oRowNew("IDGroup"))
                                If oRowsSelect.Length = 0 Then ' Si no existía, marcamos para el recálculo.
                                    lstUpdates.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' " &
                                                   "WHERE IDEmployee = " & _IDEmployee & " AND " &
                                                         " Date >= " & roTypes.Any2Time(oRowNew("BeginDate")).SQLSmallDateTime & " AND " &
                                                         " Date <= " & roTypes.Any2Time(oRowNew("EndDate")).SQLSmallDateTime & " AND " &
                                                         " Status > 45 AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)")
                                End If
                            End If
                        Next

                        For Each oRowOld As DataRow In tbMobilitiesOld.Rows
                            ' Miramos si la mobilidad existía anteriormente
                            oRowsSelect = tbMobilitiesNew.Select("BeginDate = '" & Format(oRowOld("BeginDate"), "yyyy/MM/dd") & "' AND " &
                                                                 "EndDate = '" & Format(oRowOld("EndDate"), "yyyy/MM/dd") & "' AND " &
                                                                 "IDGroup = " & oRowOld("IDGroup"))
                            If oRowsSelect.Length = 0 Then ' Si no existía, marcamos para el recálculo.
                                lstUpdates.Add("@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = ''  " &
                                               "WHERE IDEmployee = " & _IDEmployee & " AND " &
                                                     " Date >= " & roTypes.Any2Time(oRowOld("BeginDate")).SQLSmallDateTime & " AND " &
                                                     " Date <= " & roTypes.Any2Time(oRowOld("EndDate")).SQLSmallDateTime & " AND " &
                                                     " Status > 45 AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)")
                            End If
                        Next

                        If lstUpdates.Count > 0 Then

                            For Each strSQL As String In lstUpdates
                                bolRet = ExecuteSql(strSQL)
                                If Not bolRet Then Exit For
                            Next

                            If bolRet Then
                                roConnector.InitTask(TasksType.MOVES)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::RecalculateCostCenters")
                bolRet = False
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::RecalculateCostCenters")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function RecalculateDailyCoverages(ByVal _IDEmployee As Integer, ByVal tbMobilitiesOld As DataTable, ByVal _State As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim oLicense As New roServerLicense
                If oLicense.FeatureIsInstalled("Feature\HRScheduling") Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim tbMobilitiesNew As DataTable = CreateDataTable("@SELECT# * FROM EmployeeGroups WHERE IDEmployee = " & _IDEmployee.ToString & " ORDER BY BeginDate")
                    If tbMobilitiesOld IsNot Nothing AndAlso tbMobilitiesNew IsNot Nothing Then

                        Dim oRowsSelect() As DataRow
                        Dim lstUpdates As New Generic.List(Of String)

                        For Each oRowNew As DataRow In tbMobilitiesNew.Rows
                            If oRowNew.RowState <> DataRowState.Deleted And oRowNew.RowState <> DataRowState.Detached Then
                                ' Miramos si la mobilidad existía anteriormente
                                oRowsSelect = tbMobilitiesOld.Select("BeginDate = '" & Format(oRowNew("BeginDate"), "yyyy/MM/dd") & "' AND " &
                                                                     "EndDate = '" & Format(oRowNew("EndDate"), "yyyy/MM/dd") & "' AND " &
                                                                     "IDGroup = " & oRowNew("IDGroup"))
                                If oRowsSelect.Length = 0 Then ' Si no existía, marcamos para el recálculo.
                                    lstUpdates.Add("@UPDATE# DailyCoverage SET PlannedStatus = 0, ActualStatus = 0 " &
                                                   "WHERE IDGroup = " & oRowNew("IDGroup") & " AND " &
                                                         "Date >= " & roTypes.Any2Time(oRowNew("BeginDate")).SQLSmallDateTime & " AND " &
                                                         "Date <= " & roTypes.Any2Time(oRowNew("EndDate")).SQLSmallDateTime)
                                End If
                            End If
                        Next

                        For Each oRowOld As DataRow In tbMobilitiesOld.Rows
                            ' Miramos si la mobilidad existía anteriormente
                            oRowsSelect = tbMobilitiesNew.Select("BeginDate = '" & Format(oRowOld("BeginDate"), "yyyy/MM/dd") & "' AND " &
                                                                 "EndDate = '" & Format(oRowOld("EndDate"), "yyyy/MM/dd") & "' AND " &
                                                                 "IDGroup = " & oRowOld("IDGroup"))
                            If oRowsSelect.Length = 0 Then ' Si no existía, marcamos para el recálculo.
                                lstUpdates.Add("@UPDATE# DailyCoverage SET PlannedStatus = 0, ActualStatus = 0 " &
                                               "WHERE IDGroup = " & oRowOld("IDGroup") & " AND " &
                                                     "Date >= " & roTypes.Any2Time(oRowOld("BeginDate")).SQLSmallDateTime & " AND " &
                                                     "Date <= " & roTypes.Any2Time(oRowOld("EndDate")).SQLSmallDateTime)
                            End If
                        Next

                        If lstUpdates.Count > 0 Then

                            For Each strSQL As String In lstUpdates
                                bolRet = ExecuteSql(strSQL)
                                If Not bolRet Then Exit For
                            Next

                            If bolRet Then
                                Dim oParamsAux As New roCollection
                                oParamsAux.Add("Command", "UPDATE_PLANNED")
                                roConnector.InitTask(TasksType.HRSCHEDULER, oParamsAux)
                                oParamsAux = New roCollection
                                oParamsAux.Add("Command", "UPDATE_ACTUAL")
                                roConnector.InitTask(TasksType.HRSCHEDULER, oParamsAux)
                            End If

                        End If

                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::RecalculateDailyCoverages")
                bolRet = False
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::RecalculateDailyCoverages")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    Public Class roEmployeeMobility
        Implements IEquatable(Of Integer)

        Private intIDGroup As Integer
        Private datBeginDate As DateTime
        Private datEndDate As DateTime

        Public Sub New()
            Me.intIDGroup = -1
        End Sub

        Public Sub New(ByVal _IdGroup As Integer, ByVal _BeginDate As DateTime, ByVal _EndDate As DateTime)
            Me.intIDGroup = _IdGroup
            Me.datBeginDate = _BeginDate
            Me.datEndDate = _EndDate
        End Sub

        Public Property IDGroup() As Integer
            Get
                Return Me.intIDGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDGroup = value
            End Set
        End Property

        Public Property BeginDate() As DateTime
            Get
                Return Me.datBeginDate
            End Get
            Set(ByVal value As DateTime)
                Me.datBeginDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return Me.datEndDate
            End Get
            Set(ByVal value As DateTime)
                Me.datEndDate = value
            End Set
        End Property

        Public Function Equals1(ByVal _IdGroup As Integer) As Boolean Implements System.IEquatable(Of Integer).Equals
            Return Me.intIDGroup = _IdGroup
        End Function

    End Class

End Namespace