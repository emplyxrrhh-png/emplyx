Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class roEmployee

#Region "Declarations - Constructor"

    'Variables locales
    Private mvarIDCard As String
    Private mvarID As Long
    Private mvarEmployeeName As String
    Private mvarEmployeeType As String
    Private mvarEmployeeImage As System.Drawing.Image
    Private mvarIDAccessGroup As Object
    Private mvarStatus As String
    Private mvarLastMoveDateTime As New roTime
    Private mvarLastMoveID As Double
    Private bolAllowQuerys As Boolean
    Private bolAllowQueryAccruals As Boolean
    Private bolAllowQueryShifts As Boolean
    Private bolAllowQueryMoves As Boolean
    Private bolAllowRequest As Boolean ' AllowRequestLeave
    Private bolAllowRequestVacations As Boolean ' AllowRequestShift
    Private intHolidaysShift As Integer  'HolidaysShift
    Private bolAllowRequestExternalJob As Boolean ' AllowRequestExternalJob

    Private intMonthIniDay As Integer
    Private intYearIniMonth As Integer

    Private oTerminalQueryPeriod As ArrayList

    Private bolMachinesController As Boolean

    Private oEmployeeData As DataRow = Nothing

    Private bolLiveInstalled As Boolean = False

    Public Sub New()

        ' Miramos licencia para saber si és una versión live o no.
        Dim oLicense As New roServerLicense
        Me.bolLiveInstalled = (oLicense.FeatureIsInstalled("Version\LiveExpress") Or oLicense.FeatureIsInstalled("Version\Live"))

    End Sub

#End Region

#Region "Properties"

    Public Property LastMoveID() As Double
        Get
            Return mvarLastMoveID
        End Get
        Set(ByVal value As Double)
            mvarLastMoveID = value
        End Set
    End Property

    Public Property LastMoveDateTime() As roTime
        Get
            Return mvarLastMoveDateTime
        End Get
        Set(ByVal value As roTime)
            mvarLastMoveDateTime = value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return mvarStatus
        End Get
        Set(ByVal value As String)
            mvarStatus = value
        End Set
    End Property

    Public ReadOnly Property IDAccessGroup() As Object
        Get
            Return mvarIDAccessGroup
        End Get
    End Property

    Public ReadOnly Property EmployeeType() As String
        Get
            Return mvarEmployeeType
        End Get
    End Property

    Public ReadOnly Property EmployeeName() As String
        Get
            Return mvarEmployeeName
        End Get
    End Property

    Public ReadOnly Property EmployeeImage() As System.Drawing.Image
        Get
            Return Me.mvarEmployeeImage
        End Get
    End Property

    Public Property ID() As Long
        Get
            Return mvarID
        End Get
        Set(ByVal value As Long)
            mvarID = value
        End Set
    End Property

    Public Property IDCard() As String
        Get
            Return mvarIDCard
        End Get
        Set(ByVal value As String)
            mvarIDCard = value
        End Set
    End Property

    Public ReadOnly Property BeginYearPeriod(ByVal xCurrentDate As DateTime) As DateTime
        Get
            Dim xBeginPeriod As DateTime
            If xCurrentDate.Month > intYearIniMonth Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            ElseIf xCurrentDate.Month = intYearIniMonth And xCurrentDate.Day >= intMonthIniDay Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            Else
                xBeginPeriod = New DateTime(xCurrentDate.Year - 1, intYearIniMonth, intMonthIniDay)
            End If
            Return xBeginPeriod
        End Get
    End Property

    Public ReadOnly Property EndYearPeriod(ByVal xCurrentDate As DateTime) As DateTime
        Get
            Dim xBeginPeriod As DateTime
            Dim xEndPeriod As DateTime
            If xCurrentDate.Month > intYearIniMonth Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            ElseIf xCurrentDate.Month = intYearIniMonth And xCurrentDate.Day >= intMonthIniDay Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            Else
                xBeginPeriod = New DateTime(xCurrentDate.Year - 1, intYearIniMonth, intMonthIniDay)
            End If
            xEndPeriod = xBeginPeriod.AddYears(1).AddDays(-1)
            Return xEndPeriod
        End Get
    End Property


#End Region

#Region "Methods"

    Public Function Load(ByRef LogHandle As roLog) As Boolean
        '
        ' Carga datos del Empleado de un mvarID.
        '
        Dim bolRet As Boolean = False
        Dim sSQL As String

        Try

            Me.oEmployeeData = Nothing

            If mvarID <> 0 Then

                sSQL = "@SELECT# * FROM Employees WHERE ID = " & mvarID

                Dim tbEmployeeData = CreateDataTable(sSQL)
                If tbEmployeeData.Rows.Count = 1 Then

                    Me.oEmployeeData = tbEmployeeData.Rows(0)

                    'Carga Nombre o Alias
                    mvarEmployeeName = Any2String(oEmployeeData("Alias"))
                    If Len(mvarEmployeeName) = 0 Then mvarEmployeeName = Any2String(oEmployeeData("Name"))

                    'Carga Tipo de Empleado
                    mvarEmployeeType = Any2String(oEmployeeData("Type"))

                    'Carga Grupo de Acceso
                    mvarIDAccessGroup = Any2String(oEmployeeData("IDAccessGroup"))

                    ' Carga imagen empleado
                    If Not IsDBNull(oEmployeeData("Image")) Then
                        Try
                            Dim bImage As Byte() = CType(oEmployeeData("Image"), Byte())
                            Dim ms As MemoryStream = New MemoryStream(bImage)
                            Me.mvarEmployeeImage = CType(Image.FromStream(ms), Bitmap)
                        Catch ex As Exception
                            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::Load : error on load employee image, Employee '" & mvarID & "', error: ", ex)
                        End Try
                    Else
                        Me.mvarEmployeeImage = My.Resources.Employee256
                    End If

                    Me.bolAllowQuerys = Any2Boolean(oEmployeeData("AllowQueriesOnTerminal"))
                    Me.bolAllowQueryAccruals = Any2Boolean(oEmployeeData("AllowQueryAccrualsOnTerminal"))
                    Me.bolAllowQueryShifts = Any2Boolean(oEmployeeData("AllowQueryShiftsOnTerminal"))
                    Me.bolAllowQueryMoves = Any2Boolean(oEmployeeData("AllowQueryMovesOnTerminal"))

                    Me.bolAllowRequest = Any2Boolean(oEmployeeData("AllowRequestLeave"))
                    Me.bolAllowRequestVacations = Any2Boolean(oEmployeeData("AllowRequestLeave"))
                    Me.intHolidaysShift = Any2Integer(oEmployeeData("HolidaysShift"))
                    Me.bolAllowRequestExternalJob = Any2Boolean(oEmployeeData("AllowRequestExternalJob"))

                    Dim strPeriods As String = Any2String(oEmployeeData("TerminalQueryPeriod"))
                    Me.oTerminalQueryPeriod = New ArrayList
                    If strPeriods <> "" Then
                        Dim strBeginTime As String
                        Dim strEndTime As String
                        For n As Integer = 0 To strPeriods.Split(";"c).Length - 1
                            strBeginTime = strPeriods.Split(";"c)(n).Split("-")(0)
                            strEndTime = strPeriods.Split(";"c)(n).Split("-")(1)
                            Me.oTerminalQueryPeriod.Add(strBeginTime & "*" & strEndTime)
                        Next
                    End If

                    Me.bolMachinesController = Any2Boolean(oEmployeeData("MachinesController"))

                    ' Obtener paràmetros de configuración VT
                    Dim oParameters As New roParameters("OPTIONS", False)
                    intMonthIniDay = oParameters.Parameter(Parameters.MonthPeriod)
                    intYearIniMonth = oParameters.Parameter(Parameters.YearPeriod)

                    'Carga Fecha/Hora último movimiento, su estado
                    'y el ID del movimiento
                    mvarLastMoveDateTime = Nothing 'GetLastMoveDateTime(mvarID, mvarStatus, mvarLastMoveID)

                    bolRet = True
                Else
                    bolRet = False
                End If
            Else
                bolRet = False
            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "Unexpected error on LoadFromActiveDataset, Employee '" & mvarID & "', error: ", ex)
            mvarID = 0
            bolRet = False
        End Try

        Return bolRet

    End Function


    Public Function GetEmployeeMessage(ByRef LogHandle As roLog) As String
        '
        ' Devuelve el siguiente mensaje a mostrar para este empleado, o "" si no hay.
        '
        Dim strMessage As String = ""
        Dim Params() As String
        Dim cmd As DbCommand
        Dim da As DbDataAdapter
        Dim tb As New DataTable

        Try

            ' Mira si hay algun mensaje
            cmd = CreateCommand("@SELECT# * FROM EmployeeTerminalMessages WHERE IDEmployee=" & mvarID)
            da = CreateDataAdapter(cmd, True)
            da.Fill(tb)

            For Each oRow As DataRow In tb.Rows

                ' Mira si este mensaje debe mostrarse
                If GetNextEmployeeMessage_MustShow(oRow("Schedule"), oRow("LastTimeShown")) Then
                    ' Este mensaje debe mostrarse, actualiza registro y muestra
                    strMessage = oRow("Message")

                    ' Obtiene parametros
                    Params = Split(oRow("Schedule"), "@")

                    ' Mira el tipo de schedule
                    If Params(0) = "1" Then  ' El mensaje solo se muestra una vez, proceder al borrado
                        oRow.Delete()
                    Else
                        oRow("LastTimeShown") = Any2Time(Now).Value
                        'da.Update(tb)
                        'Exit For
                    End If

                    da.Update(tb)
                    Exit For

                End If

            Next

            da.Dispose()
            cmd.Dispose()
            tb.Dispose()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "VtJobBusiness::GetEmployeeMessage: Unexpected error querying messages for employee " & mvarID & ", ignoring possible messages.", ex)
        End Try

        Return strMessage

    End Function

    Public Function GetCardIDV1FromEmployeeID(ByRef LogHandle As roLog, ByVal IDEmployee As Integer) As String

        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As String = ""
        Dim sContr As String = ""
        Dim sAlias As String = ""

        Try

            aDate.Value = Now.Date

            If IDEmployee <> "0" Then

                If Not Me.bolLiveInstalled Then

                    'Buscamos en el contrato
                    sSQL = "@SELECT# CONVERT(varchar, IDCard) FROM EmployeeContracts WHERE "
                    sSQL += "idemployee = " + IDEmployee.ToString
                    sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime
                    sContr = Any2String(ExecuteScalar(sSQL))

                    'Miramos si existe un aliases
                    sAlias = roTypes.Any2String(ExecuteScalar("@SELECT# RealValue FROM CardAliases WHERE IDCard = '" & sContr & "'"))
                    If sAlias.Length > 0 Then
                        Value = sAlias
                    Else
                        Value = sContr
                    End If
                Else

                    sSQL = "@SELECT# sysroPassports_AuthenticationMethods.Credential " &
                             "FROM Employees " &
                                    "INNER JOIN sysroPassports " &
                                    "ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods " &
                                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                             "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                    "Employees.ID=" + IDEmployee.ToString
                    sContr = Any2String(ExecuteScalar(sSQL))
                    'Miramos si existe un aliases
                    sAlias = roTypes.Any2String(ExecuteScalar("@SELECT# RealValue FROM CardAliases WHERE IDCard = '" & sContr & "'"))
                    If sAlias.Length > 0 Then
                        Value = sAlias
                    Else
                        Value = sContr
                    End If

                End If

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "VTJobBusiness::GetCardIDV1FromEmployeeID: Employee=" & IDEmployee.ToString, ex)
            Value = ""
        End Try

        Return Value.TrimStart("0")

    End Function

    Public Function GetEmployeeIDFromCardID(ByRef LogHandle As roLog, ByVal CardID As String) As Long
        '
        ' Recupera el ID de un empleado pasandole el ID de Tarjeta
        '  Si no encuentra el empleado especificado devuelve -1
        '
        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As Object
        Dim CardString As String

        Try

            GetEmployeeIDFromCardID = -1

            aDate.Value = Now.Date

            ' Convertir código tarjeta
            CardID = Me.DataReader2VTValue(CardID)

            If CardID <> "0" Then

                ' Si la tarjeta tiene un alias, obtiene ahora
                'Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & CardID)
                Dim strCard As String = CardID
                Dim bolCardAliases As Boolean
                If strCard.Length Mod 4 > 0 Then strCard = strCard.PadLeft(strCard.Length + (4 - (strCard.Length Mod 4)), "0")
                Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'")
                If Value IsNot Nothing Then
                    CardString = CStr(Value)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                If Not Me.bolLiveInstalled Then

                    'Creamos la sentencia SQL
                    sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                    If bolCardAliases Then
                        sSQL &= "IdCard = " & CardString
                    Else
                        sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & CLng(strCard) & "'"
                    End If
                    sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime

                    GetEmployeeIDFromCardID = Any2Double(ExecuteScalar(sSQL))

                    If GetEmployeeIDFromCardID = 0 Then
                        sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                        sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & CLng(strCard) & "'"
                        sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime
                        GetEmployeeIDFromCardID = Any2Double(ExecuteScalar(sSQL))
                    End If
                Else

                    sSQL = "@SELECT# Employees.ID " &
                             "FROM Employees " &
                                    "INNER JOIN sysroPassports " &
                                    "ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods " &
                                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                             "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "
                    If bolCardAliases Then
                        sSQL &= "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                    Else
                        sSQL &= "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'"
                    End If
                    GetEmployeeIDFromCardID = Any2Double(ExecuteScalar(sSQL))

                    If GetEmployeeIDFromCardID = 0 Then
                        sSQL = "@SELECT# Employees.ID " &
                                 "FROM Employees " &
                                        "INNER JOIN sysroPassports " &
                                        "ON Employees.ID = sysroPassports.IDEmployee " &
                                        "INNER JOIN sysroPassports_AuthenticationMethods " &
                                        "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                 "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                       "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                       "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                       "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                       "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'"
                        GetEmployeeIDFromCardID = Any2Double(ExecuteScalar(sSQL))
                    End If

                End If

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "VTJobBusiness::GetEmployeeIDFromCardID: CardID=" & CardID, ex)
            GetEmployeeIDFromCardID = -1
        End Try

    End Function

    Public Function GetEmployeeIDFromCardIDHex(ByRef LogHandle As roLog, ByVal CardID As String) As Long
        Dim sSQL As String
        Dim Value As Object
        Dim aDate As New roTime
        Dim CardString As String
        Dim lngRet As Long = -1

        Try
            If CardID <> "0" Then
                ' Si la tarjeta tiene un alias, obtiene ahora
                aDate.Value = Now.Date
                Dim strCard As String = CardID
                Dim bolCardAliases As Boolean
                If strCard.Length.ToString Mod 2 > 0 Then strCard = strCard.PadLeft(strCard.Length.ToString + (2 - (strCard.Length.ToString Mod 2)), "0")
                Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE dbo.fn_HexToIntnt(right(dbo.fn_decToBase(RealValue,16)," + strCard.Length.ToString + ")) = dbo.fn_HexToIntnt('" & strCard & "')")
                If Value IsNot Nothing Then
                    CardString = CStr(Value)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                If Not Me.bolLiveInstalled Then

                    'Creamos la sentencia SQL
                    sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                    If bolCardAliases Then
                        sSQL &= "IdCard = " & CardString
                    Else
                        sSQL &= "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(IDCard,16)," + strCard.Length.ToString + "))  = dbo.fn_HexToIntnt('" & strCard & "')"
                    End If
                    sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime

                    lngRet = Any2Double(ExecuteScalar(sSQL))

                    If lngRet = 0 Then
                        sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                        sSQL &= "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(IDCard,16)," + strCard.Length.ToString + "))  = dbo.fn_HexToIntnt('" & strCard & "')"
                        sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime
                        lngRet = Any2Double(ExecuteScalar(sSQL))
                    End If
                Else

                    sSQL = "@SELECT# TOP 1 Employees.ID " &
                             "FROM Employees " &
                                    "INNER JOIN sysroPassports " &
                                    "ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods " &
                                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                    "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                             "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                   aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "
                    If bolCardAliases Then
                        sSQL &= "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                    Else
                        sSQL &= "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(sysroPassports_AuthenticationMethods.Credential,16)," + strCard.Length.ToString + "))= dbo.fn_HexToIntnt('" & strCard & "')"
                    End If
                    sSQL &= " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                    lngRet = Any2Double(ExecuteScalar(sSQL))

                    If lngRet = 0 Then
                        sSQL = "@SELECT# TOP 1 Employees.ID " &
                                 "FROM Employees " &
                                        "INNER JOIN sysroPassports " &
                                        "ON Employees.ID = sysroPassports.IDEmployee " &
                                        "INNER JOIN sysroPassports_AuthenticationMethods " &
                                        "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                        "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                                 "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                       "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                       "sysroPassports_AuthenticationMethods.Enabled = 1  AND " &
                                       "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                       aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                       "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(sysroPassports_AuthenticationMethods.Credential,16)," + strCard.Length.ToString + ")) = dbo.fn_HexToIntnt('" & strCard & "')" &
                                       " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                        lngRet = Any2Double(ExecuteScalar(sSQL))
                    End If

                End If

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "VTJobBusiness::GetEmployeeIDFromCardIDHex: CardID=" & CardID, ex)
            lngRet = -1
        End Try

        Return lngRet

    End Function

    ''' <summary>
    ''' Recupera el ID del empleado correspondiente a la tarjeta informada. No hace ninguna conversión del código de tarjeta.
    ''' Tiene en cuenta la longitud del código para determinar si tiene que comprar el valor con la base de datos utilizando un LIKE o no.
    ''' </summary>
    ''' <param name="ActiveConnection"></param>
    ''' <param name="LogHandle"></param>
    ''' <param name="CardID"></param>
    ''' <returns>El código del empleado. Si no existe, devuelve -1.</returns>
    ''' <remarks></remarks>
    Public Function GetEmployeeIDFromCardIDV1(ByRef LogHandle As roLog, ByVal CardID As String) As Long

        Dim lngRet As Long = -1
        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As Object
        Dim CardString As String

        Try

            aDate.Value = Now.Date

            If CardID <> "0" And CardID.Length > 0 Then

                If Not IsNumeric(CardID) Then
                    CardID = Convert.ToUInt64(CardID, 16)
                End If

                ' Si la tarjeta tiene un alias, obtiene ahora
                'Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & CardID)
                Dim strCard As String = CardID
                Dim bolCardAliases As Boolean
                'If strCard.Length Mod 4 > 0 Then strCard = strCard.PadLeft(strCard.Length + (4 - (strCard.Length Mod 4)), "0")
                Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'")
                If Value IsNot Nothing Then
                    CardString = CStr(Value)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                If Not Me.bolLiveInstalled Then

                    'Creamos la sentencia SQL
                    sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                    If bolCardAliases Then
                        sSQL &= "IdCard = " & CardString
                    Else
                        sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & CLng(strCard) & "'"
                    End If
                    sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime

                    lngRet = Any2Double(ExecuteScalar(sSQL))

                    If lngRet = 0 Then
                        sSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE "
                        sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & CLng(strCard) & "'"
                        sSQL = sSQL & " AND " & aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime
                        lngRet = Any2Double(ExecuteScalar(sSQL))
                    End If
                Else

                    sSQL = "@SELECT# TOP 1 Employees.ID " &
                             "FROM Employees " &
                                    "INNER JOIN sysroPassports " &
                                    "ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods " &
                                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                    "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                             "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                   aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "
                    If bolCardAliases Then
                        sSQL &= "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                    Else
                        sSQL &= "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'"
                    End If
                    sSQL &= " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                    lngRet = Any2Double(ExecuteScalar(sSQL))

                    If lngRet = 0 Then
                        sSQL = "@SELECT# TOP 1 Employees.ID " &
                                 "FROM Employees " &
                                        "INNER JOIN sysroPassports " &
                                        "ON Employees.ID = sysroPassports.IDEmployee " &
                                        "INNER JOIN sysroPassports_AuthenticationMethods " &
                                        "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                        "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                                 "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                       "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                       "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                       "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                       aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                       "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'" &
                                       " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                        lngRet = Any2Double(ExecuteScalar(sSQL))
                    End If

                End If

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "VTJobBusiness::GetEmployeeIDFromCardID: CardID=" & CardID, ex)
            lngRet = -1
        End Try

        Return lngRet

    End Function

    Public Function GetCardIDFromCard(ByVal strCard As String, ByRef LogHandle As roLog) As Long
        '
        ' Recupera el IDCard asignado a una lectura de tarjeta en la tabla CardAliases
        '  Si no encuentra el IDCard especificado devuelve la conversión de la lectura
        '
        Dim Value As Object
        Dim lngCard As Long

        Try

            ' Convertir código tarjeta
            lngCard = Me.DataReader2VTValue(strCard)

            ' Si la tarjeta tiene un alias, obtiene ahora
            Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & lngCard)
            If Value IsNot Nothing Then
                lngCard = Value
            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "Unexpected error on GetCardIDFromCard, Card '" & strCard & "', error: ", ex)
        Finally

        End Try

        Return lngCard

    End Function

    Public Function GetCardIDFromCardV1(ByVal strCard As String, ByRef LogHandle As roLog) As Long
        '
        ' Recupera el IDCard asignado a una lectura de tarjeta en la tabla CardAliases
        '  Si no encuentra el IDCard especificado devuelve la conversión de la lectura
        '
        Dim Value As Object
        Dim lngCard As Long

        Try

            If Not IsNumeric(strCard) Then
                lngCard = Convert.ToUInt64(strCard, 16)
            Else
                lngCard = strCard
            End If

            ' Si la tarjeta tiene un alias, obtiene ahora
            Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & lngCard)
            If Value IsNot Nothing Then
                lngCard = Value
            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "Unexpected error on GetCardIDFromCard, Card '" & strCard & "', error: ", ex)
        Finally

        End Try

        Return lngCard

    End Function



    Private Sub UpdateDailySchedule(ByRef ServerConn As roConnector, ByVal EmployeeID As Long, ByVal MoveDate As Object)
        'Avisamos que se han modificados datos para volver a calcular
        '
        '
        Dim cmd As DbCommand
        Dim da As DbDataAdapter
        Dim tb As New DataTable

        cmd = CreateCommand("@SELECT# * FROM DailySchedule WHERE IDEmployee=" & EmployeeID & " AND Date=" & Any2Time(MoveDate).SQLSmallDateTime)
        da = CreateDataAdapter(cmd, True)
        da.Fill(tb)

        Dim oRow As DataRow
        If tb.Rows.Count = 0 Then
            oRow = tb.NewRow
            oRow("IDEmployee") = EmployeeID
            oRow("Date") = MoveDate
        Else
            oRow = tb.Rows(0)
        End If

        ' Actualiza status
        oRow("Status") = 0
        oRow("JobStatus") = 0
        oRow("GUID") = ""

        If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)

        ' Guarda cambios
        da.Update(tb)
        da.Dispose()
        cmd.Dispose()
        tb.Dispose()

        ' Notifica que hemos modificado la tabla Moves para que se calcule de nuevo
        ''ServerConn.Context(roVarDataOp) = roTableObject & ":\\MOVES"

    End Sub

    Private Function GetNextEmployeeMessage_MustShow(ByVal Schedule As String, ByVal LastTimeShown As Object) As Boolean
        '
        ' Determina si debemos mostrar un mensaje o no
        '
        Dim Params() As String
        Dim MonthDiff As Integer

        Try

            ' Obtiene parametros
            Params = Split(Schedule, "@")

            ' Mira el tipo de schedule
            Select Case Params(0)
                Case "1" ' Una vez. No tiene en cuenta la fecha, se mostrará en el próximo marcaje
                    If IsDBNull(LastTimeShown) Then GetNextEmployeeMessage_MustShow = True

                Case "D" ' Cada X dias
                    If IsDBNull(LastTimeShown) Then
                        GetNextEmployeeMessage_MustShow = True
                    ElseIf DateDiff("d", LastTimeShown, Now) >= Any2Long(Params(1)) Then
                        GetNextEmployeeMessage_MustShow = True
                    End If

                Case "S" ' Cada cierto dia de la semana
                    If Weekday(Now, vbMonday) = Any2Long(Params(1)) Then
                        If IsDBNull(LastTimeShown) Then
                            GetNextEmployeeMessage_MustShow = True
                        ElseIf Any2Time(LastTimeShown).DateOnly <> Any2Time(Now).DateOnly Then
                            GetNextEmployeeMessage_MustShow = True
                        End If
                    End If

                Case "M" ' Cada cierto dia del mes
                    If Params(1) = "DM" Then
                        ' Dia numero tal cada tantos meses
                        If Day(Now) = Any2Long(Params(2)) Then
                            If IsDBNull(LastTimeShown) Then
                                ' Primera vez
                                GetNextEmployeeMessage_MustShow = True
                            Else
                                ' Otras veces, mira que hayan pasado n meses
                                MonthDiff = Month(Now) - Month(LastTimeShown) + 12
                                If MonthDiff < 0 Then MonthDiff = MonthDiff + 12
                                If MonthDiff Mod Any2Long(Params(3)) = 0 Then
                                    If Any2Time(LastTimeShown).DateOnly <> Any2Time(Now).DateOnly Then
                                        GetNextEmployeeMessage_MustShow = True
                                    End If
                                End If
                            End If
                        End If
                    Else
                        ' Cada dia de la semana tal cada cuantos meses
                        ' NO IMPLEMENTADO
                    End If

                Case "A" ' Cada cierto dia del año
                    If Day(Now) = Any2Long(Params(1)) And Month(Now) = Any2Long(Params(2)) Then
                        If IsDBNull(LastTimeShown) Then
                            GetNextEmployeeMessage_MustShow = True
                        ElseIf Any2Time(LastTimeShown).DateOnly <> Any2Time(Now).DateOnly Then
                            GetNextEmployeeMessage_MustShow = True
                        End If
                    End If

                Case Else
                    GetNextEmployeeMessage_MustShow = False
            End Select
        Catch ex As Exception
            GetNextEmployeeMessage_MustShow = False
        End Try

    End Function

    Public Function UpdateBeginDateByOrder(ByVal IDJob As Double, ByRef LogHandle As roLog, ByVal PunchTime As roTime) As Boolean
        '
        'Actualiza la fecha de incio de las Ordenes que contienen
        'a la fase fichada

        Dim Values As Integer
        Dim IDOrder As String
        Dim SelValues As Integer
        Dim Ok As Boolean
        Dim cmd As DbCommand
        Dim da As DbDataAdapter
        Dim ads As New DataTable
        Dim Order As String
        Dim sSQL As String
        Dim i As Integer
        Dim Job As String

        Try

            UpdateBeginDateByOrder = True

            Job = Any2String(ExecuteScalar("@SELECT# Path FROM Jobs WHERE ID=" & IDJob))
            Values = StringItemsCount(Job, "\")
            IDOrder = ""
            For i = 0 To Values - 2
                If i = 0 Then
                    IDOrder = String2Item(Job, i, "\")
                Else
                    IDOrder = IDOrder & "\" & String2Item(Job, i, "\")
                End If
            Next i

            sSQL = "@SELECT# ID,BeginDate FROM Orders WHERE LEN(ID) <=" & Len(IDOrder) & " AND ID Like '" & String2Item(Job, 0, "\") & "%' AND LEN(id) >=" & Len(String2Item(Job, 0, "\")) & " Order by ID desc"
            cmd = CreateCommand(sSQL)
            da = CreateDataAdapter(cmd, True)
            da.Fill(ads)
            For Each oRow As DataRow In ads.Rows

                SelValues = StringItemsCount(oRow("ID"), "\")
                Ok = True
                For i = 0 To SelValues - 1
                    If Not (String2Item(IDOrder, i, "\") = String2Item(oRow("ID"), i, "\")) Then
                        Ok = False
                    End If
                Next i
                If Ok Then
                    Order = Any2String(oRow("ID"))
                    If Not IsDBNull(oRow("BeginDate")) Then
                        If Any2Time(oRow("BeginDate")).DateOnly > PunchTime.DateOnly Then
                            oRow("BeginDate") = PunchTime.DateOnly
                        End If
                    Else
                        oRow("BeginDate") = PunchTime.DateOnly
                    End If
                End If

            Next

            da.Update(ads)
            da.Dispose()
            cmd.Dispose()
            ads.Dispose()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "Error en UpdateBeginDateByOrder. Error:", ex)
            UpdateBeginDateByOrder = False
        End Try

    End Function

    Public Function GetLastIDJob() As Long
        '
        'Retorna el último trabajo donde se ha estado sin incidencia
        '

        Dim sSQL As String
        Dim ads As DbDataReader

        sSQL = "@SELECT# top 1 * FROM EmployeeJobMoves WHERE IDEmployee = " & mvarID
        sSQL = sSQL & " AND InDateTime is Not Null AND OutDateTime is Not Null"
        '    sSQL = sSQL & " AND IDIncidence = 0"
        sSQL = sSQL & " Order by OutDateTime DESC,ID DESC "

        GetLastIDJob = 0

        ads = CreateDataReader(sSQL)
        If ads.Read Then
            GetLastIDJob = Any2Double(ads.Item("IDJob"))
            ads.Close()
            ads = Nothing
            Exit Function
        End If
        ads.Close()
        ads = Nothing

    End Function

    Public Function InQueryTime(ByVal xCurrentDateTime As DateTime, ByVal LogHandle As roLog) As Boolean

        Dim bolRet As Boolean = False

        Dim strBeginHour As String
        Dim strBeginMin As String
        Dim strEndHour As String
        Dim strEndMin As String
        Dim xBegin As DateTime
        Dim xEnd As DateTime

        If Me.oTerminalQueryPeriod.Count > 0 Then

            For Each strPeriod As String In Me.oTerminalQueryPeriod

                strBeginHour = strPeriod.Split("*"c)(0).Substring(0, 2)
                strBeginMin = strPeriod.Split("*"c)(0).Substring(3, 2)
                strEndHour = strPeriod.Split("*"c)(1).Substring(0, 2)
                strEndMin = strPeriod.Split("*"c)(0).Substring(3, 2)

                xBegin = New DateTime(xCurrentDateTime.Year, xCurrentDateTime.Month, xCurrentDateTime.Day, strBeginHour, strBeginMin, 0)
                xEnd = New DateTime(xCurrentDateTime.Year, xCurrentDateTime.Month, xCurrentDateTime.Day, strEndHour, strEndMin, 59)

                bolRet = (DateDiff(DateInterval.Minute, xBegin, xCurrentDateTime) >= 0 And
                          DateDiff(DateInterval.Minute, xCurrentDateTime, xEnd) >= 0)

                If bolRet Then Exit For

            Next
        Else
            bolRet = True
        End If

        Return bolRet

    End Function

#Region "Accruals methods"


#End Region

#Region "Shifts methods"



#End Region

#Region "Vacation methods"

    Public Function VacationsMonthsQuery(ByVal xCurrentDate As DateTime, ByVal LogHandle As roLog) As DataTable

        Dim tbVacations As DataTable = Nothing

        Dim rd As DbDataReader = Nothing
        Try

            ' Determinar inicio i final del periodo anual
            Dim xBeginPeriod As DateTime = Me.BeginYearPeriod(xCurrentDate)
            Dim xEndPeriod As DateTime = Me.EndYearPeriod(xCurrentDate)

            Dim strSQL As String = "@SELECT# DATEPART(year, DailySchedule.[Date]) AS 'Year', DATEPART(month, DailySchedule.[Date]) AS 'Month' " &
                                   "FROM DailySchedule " &
                                   "WHERE DailySchedule.IDEmployee = " & Me.ID.ToString & " AND " &
                                         "DailySchedule.[Date] between CONVERT(smalldatetime, '" & Format(xBeginPeriod, "dd/MM/yyyy") & "', 103) AND " &
                                                                      "CONVERT(smalldatetime, '" & Format(xEndPeriod, "dd/MM/yyyy") & "', 103) " &
                                   "GROUP BY DATEPART(year, DailySchedule.[Date]), DATEPART(month, DailySchedule.[Date])"
            tbVacations = CreateDataTable(strSQL, )
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::VacationsMonthsQuery:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return tbVacations

    End Function

    Public Function VacationsResumeQuery(ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                         ByRef intDone As Integer, ByRef intPending As Integer, ByRef intLasting As Integer, ByRef intDisponible As Integer,
                                         ByVal LogHandle As roLog) As Boolean

        Dim bolRet As Boolean = False

        Dim rd As DbDataReader = Nothing
        Try

            ' Obtener paràmetros de configuración VT
            'Dim intMonthIniDay As Integer = 1
            'Dim intYearIniMonth As Integer = 1
            'rd = CreateDataReader("@SELECT# Data FROM sysroParameters WHERE [ID] = 'OPTIONS'")
            'If rd.Read Then
            '    Dim oCollection As New roCollection(rd("Data"))
            '    intMonthIniDay = oCollection.KeyValue("MonthPeriod")
            '    intYearIniMonth = oCollection.KeyValue("YearPeriod")
            'End If
            'rd.Close()

            ' Determinar inicio i final del periodo anual
            xBeginPeriod = Me.BeginYearPeriod(xCurrentDate)
            xEndPeriod = Me.EndYearPeriod(xCurrentDate)
            'If xCurrentDate.Month > intYearIniMonth Then
            '    xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            'ElseIf xCurrentDate.Month = intYearIniMonth And xCurrentDate.Day >= intMonthIniDay Then
            '    xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            'Else
            '    xBeginPeriod = New DateTime(xCurrentDate.Year - 1, intYearIniMonth, intMonthIniDay)
            'End If
            'xEndPeriod = xBeginPeriod.AddYears(1).AddDays(-1)

            ' Obtener número días ya disfrutados
            intDone = Me.GetAlreadyTakenHollidays(xCurrentDate, xBeginPeriod, LogHandle)

            ' Obtener número de días solicitados pendientes de procesar
            intPending = Me.GetPendingApprovalHollidays(xBeginPeriod, xEndPeriod, LogHandle)

            ' Obtener el número de días aprobados pendientes de disfrutar
            intLasting = Me.GetApprovedButNotTakenHolidays(xCurrentDate, xEndPeriod, LogHandle)

            ' Obtener los días disponibles de vacaciones
            Dim bolAreWorkingDays As Boolean
            intDisponible = Me.GetDisponibleHolidays(xBeginPeriod, xEndPeriod, bolAreWorkingDays, LogHandle)

            bolRet = True
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::VacationsResumeQuery:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return bolRet

    End Function

    Public Function GetAlreadyTakenHollidays(ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime, ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String
            strSQL = "@SELECT# COUNT(*) " &
                     "FROM DailySchedule INNER JOIN Employees " &
                                "ON DailySchedule.IDEmployee = Employees.[ID] " &
                     "WHERE DailySchedule.IDEmployee = " & Me.ID.ToString & " AND " &
                           "DailySchedule.IDShiftUsed = Employees.HolidaysShift AND " &
                           "DailySchedule.[Date] BETWEEN " &
                                "CONVERT(smalldatetime, '" & Format(xBeginPeriod, "dd/MM/yyyy") & "', 103) AND " &
                                "CONVERT(smalldatetime, '" & Format(xCurrentDate, "dd/MM/yyyy") & "', 103)"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                intRet = Any2Integer(rd(0))
            End If
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::GetAlreadyTakenHollidays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function GetPendingApprovalHollidays(ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime, ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String
            strSQL = "@SELECT# * " &
                     "FROM WtRequest INNER JOIN Employees " &
                                "ON WtRequest.IDEmployee = Employees.[ID] " &
                            "INNER JOIN Shifts " &
                                "ON Employees.HolidaysShift = Shifts.[ID] " &
                     "WHERE WtRequest.IDEmployee = " & Me.ID.ToString & " AND " &
                           "WtRequest.ShiftId = Employees.HolidaysShift AND " &
                           "WtRequest.RequestType=2 AND " &
                           "NOT (WtRequest.Status = 1 AND WtRequest.StatusLevel = 1)"
            rd = CreateDataReader(strSQL)
            Dim xRequestBegin As DateTime
            Dim xRequestEnd As DateTime
            Dim bolAreWorkingDays As Boolean
            While rd.Read
                If Not IsDBNull(rd("DateBegin")) And Not IsDBNull(rd("DateEnd")) Then
                    xRequestBegin = rd("DateBegin")
                    xRequestEnd = rd("DateEnd")
                    bolAreWorkingDays = Any2Boolean(rd("AreWorkingDays"))
                    If xRequestBegin >= xBeginPeriod And xRequestEnd <= xEndPeriod Then
                        ' Si el período solicitado está completamente dentro del año en curso, cuento todos los días
                        If Not bolAreWorkingDays Then ' Días naturales
                            intRet += xRequestEnd.Subtract(xRequestBegin).Days + 1
                        Else ' Días laborables
                            intRet += Me.LaboralDaysInPeriod(xRequestBegin, xRequestEnd, LogHandle)
                        End If
                    Else
                        If (xRequestBegin >= xBeginPeriod And xRequestBegin <= xEndPeriod) And xRequestEnd > xEndPeriod Then
                            ' La fecha final de la solicitud es posterior al final de año
                            If Not bolAreWorkingDays Then ' Días naturales
                                intRet += xEndPeriod.Subtract(xRequestBegin).Days + 1
                            Else ' Días laborables
                                intRet += Me.LaboralDaysInPeriod(xRequestBegin, xEndPeriod, LogHandle)
                            End If
                        Else
                            If xRequestBegin < xBeginPeriod And (xRequestEnd <= xEndPeriod And xRequestEnd >= xBeginPeriod) Then
                                ' La fecha inicial de la solicitud es anterior al inicio de año
                                If Not bolAreWorkingDays Then ' Días naturales
                                    intRet += xRequestEnd.Subtract(xBeginPeriod).Days + 1
                                Else ' Días laborables
                                    intRet += Me.LaboralDaysInPeriod(xBeginPeriod, xRequestEnd, LogHandle)
                                End If
                            Else
                                If xRequestBegin < xBeginPeriod And xRequestEnd > xEndPeriod Then
                                    ' La solicitud excede el año actual por el inicio y por el fin
                                    If Not bolAreWorkingDays Then ' Días naturales
                                        intRet += xEndPeriod.Subtract(xBeginPeriod).Days + 1
                                    Else ' Días laborables
                                        intRet += Me.LaboralDaysInPeriod(xBeginPeriod, xEndPeriod, LogHandle)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End While
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::GetPendingApprovalHollidays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function GetApprovedButNotTakenHolidays(ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime, ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String
            strSQL = "@SELECT# COUNT(*) " &
                     "FROM DailySchedule INNER JOIN Employees " &
                            "ON DailySchedule.IDEmployee = Employees.[ID] " &
                     "WHERE DailySchedule.IDEmployee = " & Me.ID.ToString & " AND " &
                           "(DailySchedule.IDShift1 = Employees.HolidaysShift OR " &
                            "DailySchedule.IDShift2 = Employees.HolidaysShift OR " &
                            "DailySchedule.IDShift3 = Employees.HolidaysShift OR " &
                            "DailySchedule.IDShift4 = Employees.HolidaysShift) AND " &
                           "DailySchedule.[Date] BETWEEN " &
                                "CONVERT(smalldatetime, '" & Format(xCurrentDate.AddDays(1).Date, "dd/MM/yyyy") & "', 103) AND " &
                                "CONVERT(smalldatetime, '" & Format(xEndPeriod.Date, "dd/MM/yyyy") & "', 103)"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                intRet = Any2Integer(rd(0))
            End If
            rd.Close()

            ''"LEFT JOIN Shifts Shifts1 ON DailySchedule.IDShift1 = Shifts1.[ID] " & _
            ''"LEFT JOIN Shifts Shifts2 ON DailySchedule.IDShift2 = Shifts2.[ID] " & _
            ''"LEFT JOIN Shifts Shifts3 ON DailySchedule.IDShift3 = Shifts3.[ID] " & _
            ''"LEFT JOIN Shifts Shifts4 ON DailySchedule.IDShift4 = Shifts4.[ID] " & _
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::GetApprovedButNotTakenHolidays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function GetDisponibleHolidays(ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime, ByRef bolAreWorkingDays As Boolean, ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String
            strSQL = "@SELECT# SUM(DailyAccruals.Value) " &
                     "FROM DailyAccruals INNER JOIN Concepts " &
                            "ON DailyAccruals.IDConcept = Concepts.[ID] " &
                            "INNER JOIN Shifts ON Concepts.[ID] = Shifts.IDConceptBalance " &
                     "WHERE Shifts.[ID] = " & Me.intHolidaysShift.ToString & " AND " &
                           "DailyAccruals.IDEmployee = " & Me.ID.ToString & " AND " &
                           "DailyAccruals.[Date] BETWEEN " &
                                Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                Any2Time(xEndPeriod.Date).SQLSmallDateTime
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                intRet = Any2Integer(rd(0))
            End If
            rd.Close()

            strSQL = "@SELECT# AreWorkingDays " &
                     "FROM Shifts " &
                     "WHERE [ID] = " & Me.intHolidaysShift.ToString
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                bolAreWorkingDays = Any2Boolean(rd(0))
            End If
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::GetDisponibleHolidays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function LaboralDaysInPeriod(ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime, ByVal LogHandle As roLog) As Integer
        '
        ' Devuelve el número de días laborales que hay en el periodo indicado (se incluye fecha de inici y fecha de fin)
        '
        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String = "@SELECT# COUNT(*) " &
                                   "FROM DailySchedule LEFT JOIN Shifts Shifts1 ON DailySchedule.IDShift1 = Shifts1.[ID] " &
                                                      "LEFT JOIN Shifts Shifts2 ON DailySchedule.IDShift2 = Shifts2.[ID] " &
                                                      "LEFT JOIN Shifts Shifts3 ON DailySchedule.IDShift3 = Shifts3.[ID] " &
                                                      "LEFT JOIN Shifts Shifts4 ON DailySchedule.IDShift4 = Shifts4.[ID] " &
                                   "WHERE DailySchedule.IDEmployee = " & Me.ID.ToString & " AND " &
                                         "DailySchedule.[Date] BETWEEN CONVERT(smalldatetime, '" & Format(xBeginPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                                                      "CONVERT(smalldatetime, '" & Format(xEndPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                         "(Shifts1.ExpectedWorkingHours <> 0 OR Shifts2.ExpectedWorkingHours <> 0 OR Shifts3.ExpectedWorkingHours <> 0 OR Shifts4.ExpectedWorkingHours <> 0)"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                intRet = Any2Integer(rd(0))
            End If
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::LaboralDaysInPeriod:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function ValidateHolidaysRequest(ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByRef intResponse As Integer, ByVal LogHandle As roLog) As Boolean

        Dim bolRet As Boolean = False

        Dim rd As DbDataReader = Nothing
        Try

            bolRet = True

            intResponse = 0

            ' Determinar si ya existe una solicitud generada entre las fechas indicadas
            Dim strSQL As String = "@SELECT# * " &
                                   "FROM WtRequest INNER JOIN Employees " &
                                            "ON WtRequest.IDEmployee = Employees.[ID] " &
                                            "INNER JOIN Shifts " &
                                            "ON Employees.HolidaysShift = Shifts.[ID] " &
                                   "WHERE WtRequest.IDEmployee = " & Me.ID.ToString & " AND " &
                                         "WtRequest.ShiftId = Employees.HolidaysShift AND " &
                                         "WtRequest.RequestType=2 AND " &
                                         "(WtRequest.DateBegin BETWEEN " &
                                          Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date.AddDays(1)).SQLSmallDateTime & " OR " &
                                          "WtRequest.DateEnd BETWEEN " &
                                          Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date.AddDays(1)).SQLSmallDateTime & ")"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                bolRet = False
                intResponse = 1
            End If
            rd.Close()

            If bolRet Then

                ' Determinar inicio i final del periodo anual
                Dim xBeginPeriod As DateTime = Me.BeginYearPeriod(xBegin.Date)
                Dim xEndPeriod As DateTime = Me.EndYearPeriod(xBegin.Date)

                ' Obtener los días disponibles
                Dim bolAreWorkingDays As Boolean = False
                Dim intDisponible As Integer = Me.GetDisponibleHolidays(xBeginPeriod, xEndPeriod, bolAreWorkingDays, LogHandle) -
                                               Me.GetPendingApprovalHollidays(xBeginPeriod, xEndPeriod, LogHandle)

                Dim intRequestDays As Integer = 0
                If bolAreWorkingDays Then ' Días laborables
                    intRequestDays = Me.LaboralDaysInPeriod(xBegin, xEnd, LogHandle)
                Else ' Días naturales
                    intRequestDays = DateDiff(DateInterval.Day, xBegin, xEnd) + 1
                End If

                bolRet = (intRequestDays <= intDisponible)

                If Not bolRet Then intResponse = 2

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::ValidateHolidaysRequest:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return bolRet

    End Function

#End Region

#Region "Requests methods"

    Public Function RequestMinimumDays(ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 7

        Dim rd As DbDataReader = Nothing
        Try

            rd = CreateDataReader("@SELECT# Data FROM sysroParameters WHERE [ID] = 'OPTIONS'")
            If rd.Read Then
                Dim oCollection As New roCollection(rd("Data"))
                Dim strDays As String = oCollection.Item("RequestMinimumDays")
                If strDays <> "" AndAlso IsNumeric(strDays) Then
                    intRet = Val(strDays)
                End If
            End If
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::RequestMinimumDays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function CreateRequest(ByVal intRequestType As Integer, ByVal xRequestDate As DateTime, ByVal strDescription As String,
                                  ByVal xDateBegin As DateTime, ByVal xDateEnd As DateTime, ByVal intIDShift As Integer,
                                  ByVal intIDCause As Nullable(Of Integer), ByVal intInAction As Nullable(Of Integer), ByVal intHours As Nullable(Of Integer), ByVal intIDEmployee2 As Nullable(Of Integer),
                                  ByVal LogHandle As roLog) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim cmd As DbCommand
            Dim da As DbDataAdapter
            Dim tb As New DataTable

            Dim strSQL As String = "@SELECT# * FROM WtRequest WHERE IdRequest = -1"

            cmd = CreateCommand(strSQL)
            da = CreateDataAdapter(cmd, True)
            da.Fill(tb)

            Dim oRequest As DataRow = tb.NewRow
            oRequest("IdEmployee") = Me.ID
            oRequest("RequestType") = intRequestType
            oRequest("RequestDate") = xRequestDate
            oRequest("Description") = strDescription
            oRequest("Status") = 0
            oRequest("StatusLevel") = 0
            oRequest("StatusDateTime") = xRequestDate
            oRequest("StatusUserId") = DBNull.Value
            oRequest("DateBegin") = xDateBegin
            oRequest("DateEnd") = xDateEnd
            oRequest("ShiftId") = intIDShift
            If intIDCause.HasValue Then oRequest("Cause") = intIDCause.Value
            If intInAction.HasValue Then oRequest("InAction") = intInAction.Value
            If intHours.HasValue Then oRequest("Hours") = intHours.Value
            If intIDEmployee2.HasValue Then oRequest("IdEmployee2") = intIDEmployee2.Value

            tb.Rows.Add(oRequest)

            da.Update(tb)

            bolRet = True
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::VacationsMonthsQuery:", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Public Function PendingRequestCount(ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            rd = CreateDataReader("@SELECT# ISNULL(COUNT(*),0) Value " &
                                  "FROM wtRequest " &
                                  "WHERE (Status=0 OR (Status=1 AND StatusLevel>1)) " &
                                  "AND IDEmployee = " & Me.ID.ToString)

            If rd.Read Then
                intRet = Any2Integer(rd(0))
            End If
            rd.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::RequestMinimumDays:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

#End Region

#Region "Presence query methods"

    Public Function PresenceMinutes(ByVal xDate As DateTime, ByVal LogHandle As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String = "@SELECT# * " &
                                   "FROM Moves " &
                                   "WHERE IDEmployee = " & Me.ID.ToString & " AND " &
                                         "ShiftDate = CONVERT(smalldatetime, '" & Format(xDate, "dd/MM/yyyy") & "', 103) " &
                                   "ORDER BY [ID]"
            rd = CreateDataReader(strSQL)
            While rd.Read
                If Not IsDBNull(rd("InDateTime")) And Not IsDBNull(rd("OutDateTime")) Then
                    intRet += DateDiff(DateInterval.Minute, rd("InDateTime"), rd("OutDateTime"))
                End If

            End While
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::PresenceMinutes:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function PresenceMinutes(ByVal xDate As DateTime, ByVal LogHandle As roLog, ByVal UseCurrentDate As Boolean) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String = "@SELECT# * " &
                                   "FROM Moves " &
                                   "WHERE IDEmployee = " & Me.ID.ToString & " AND " &
                                         "ShiftDate = CONVERT(smalldatetime, '" & Format(xDate, "dd/MM/yyyy") & "', 103) " &
                                   "ORDER BY [ID]"
            rd = CreateDataReader(strSQL)
            While rd.Read
                If Not IsDBNull(rd("InDateTime")) And Not IsDBNull(rd("OutDateTime")) Then
                    intRet += DateDiff(DateInterval.Minute, rd("InDateTime"), rd("OutDateTime"))
                ElseIf Not IsDBNull(rd("InDateTime")) And UseCurrentDate Then
                    intRet += DateDiff(DateInterval.Minute, rd("InDateTime"), xDate)
                End If

            End While
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::PresenceMinutes:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return intRet

    End Function

    Public Function PresenceDetail(ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal LogHandle As roLog) As DataTable

        Dim tb As New DataTable("PresenceDetail")
        tb.Columns.Add(New DataColumn("Date", GetType(DateTime)))
        tb.Columns.Add(New DataColumn("Moves", GetType(String)))
        tb.Columns.Add(New DataColumn("PresenceMinutes", GetType(Integer)))
        tb.Columns.Add(New DataColumn("MovesPairs", GetType(Integer)))

        Dim rd As DbDataReader = Nothing
        Try

            Dim oRow As DataRow

            Dim strSQL As String = "@SELECT# DailySchedule.[Date], Moves.* " &
                                   "FROM DailySchedule LEFT JOIN Moves " &
                                            "ON DailySchedule.[Date] = Moves.ShiftDate AND " &
                                               "DailySchedule.IDEmployee = Moves.IDEmployee " &
                                   "WHERE DailySchedule.IDEmployee = " & Me.ID.ToString & " AND " &
                                         "DailySchedule.[Date] between CONVERT(smalldatetime, '" & Format(xBeginDate, "dd/MM/yyyy") & "', 103) AND " &
                                                                      "CONVERT(smalldatetime, '" & Format(xEndDate, "dd/MM/yyyy") & "', 103) " &
                                   "ORDER BY DailySchedule.[Date], Moves.InDateTime"
            rd = CreateDataReader(strSQL)

            Dim xDate As Nullable(Of DateTime) = Nothing
            Dim strMoves As String = ""
            Dim strMoveIn As String
            Dim strMoveOut As String
            Dim intMovesPairs As Integer = 0
            Dim n As Integer = 1
            While rd.Read

                If xDate.HasValue AndAlso xDate.Value <> rd("Date") Then
                    If strMoves <> "" Then
                        If strMoves.Substring(strMoves.Length - 2, 2) = vbCrLf Then
                            strMoves = strMoves.Substring(0, strMoves.Length - 2)
                        End If
                    End If
                    oRow = tb.NewRow
                    oRow("Date") = xDate.Value
                    oRow("Moves") = strMoves
                    oRow("PresenceMinutes") = Me.PresenceMinutes(xDate.Value, LogHandle)
                    oRow("MovesPairs") = intMovesPairs
                    tb.Rows.Add(oRow)
                    strMoves = ""
                    intMovesPairs = 0
                    n = 1
                End If

                xDate = rd("Date")
                If Not IsDBNull(rd("InDateTime")) Then
                    strMoveIn = Format(rd("InDateTime"), "HH:mm")
                Else
                    strMoveIn = "__:__"
                End If
                If Not IsDBNull(rd("OutDateTime")) Then
                    strMoveOut = Format(rd("OutDateTime"), "HH:mm")
                Else
                    strMoveOut = "__:__"
                End If
                If strMoveIn <> "__:__" Or strMoveOut <> "__:__" Then
                    strMoves &= strMoveIn & "-" & strMoveOut & " "
                    intMovesPairs += 1
                    If n = 2 Then
                        strMoves &= vbCrLf
                        n = 0
                    End If
                    n += 1
                End If

            End While
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::PresenceMinutes:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return tb

    End Function

#End Region

#Region "ExternalWork methods"

    Public Function CreateExternalWork(ByVal xDate As DateTime, ByVal dblValue As Double, ByRef bolRepited As Boolean,
                                       ByVal LogHandle As roLog) As Boolean

        Dim bolRet As Boolean = False

        Dim rd As DbDataReader = Nothing
        Try

            ' Obtener la justificación horas trabajadas (HTR)
            Dim intIDCause As Integer = 1
            ''rd = CreateDataReader("@SELECT# [ID] FROM Causes WHERE ShortName = 'HTR'")
            ''If rd.Read Then
            ''    intIDCause = rd("ID")
            ''End If
            ''rd.Close()

            Dim cmd As DbCommand
            Dim da As DbDataAdapter
            Dim tb As New DataTable

            Dim strSQL As String = "@SELECT# * FROM DailyCauses " &
                                   "WHERE IDEmployee = " & Me.ID.ToString & " AND " &
                                         "[Date] = CONVERT(smalldatetime, '" & Format(xDate, "dd/MM/yyyy") & "', 103) AND " &
                                         "IDRelatedIncidence = 0 AND IDCause = " & intIDCause.ToString

            cmd = CreateCommand(strSQL)
            da = CreateDataAdapter(cmd, True)
            da.Fill(tb)

            If tb.Rows.Count = 0 Then

                Dim oRequest As DataRow = tb.NewRow
                oRequest("IdEmployee") = Me.ID
                oRequest("Date") = xDate
                oRequest("IDRelatedIncidence") = 0
                oRequest("IDCause") = intIDCause
                oRequest("Value") = dblValue
                oRequest("Manual") = 1
                oRequest("CauseUser") = DBNull.Value
                oRequest("CauseUserType") = DBNull.Value
                oRequest("AccrualsRules") = 0
                oRequest("DailyRule") = 0
                oRequest("AccruedRule") = 0
                oRequest("IsNotReliable") = 1

                tb.Rows.Add(oRequest)

                da.Update(tb)

                bolRepited = False
            Else
                bolRepited = True
            End If

            bolRet = True
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::CreateExternalWork:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return bolRet

    End Function

#End Region

#Region "Cards conversion methods"

    Public Function DataReader2VTValue(ByVal strData As String) As Long
        '
        ' Convierte el código en hexadecimal que obtenemos del lector de poximidad a el código almacenado a VT
        '
        Dim lngValue As Long = Convert.ToUInt64(strData, 16)

        strData = lngValue.ToString

        ' Solo se aplica la transformación si el código en decimal tiene más de 10 dígitos
        'If strData.Length > 10 Then

        strData = Binary(strData, 2)
        If strData.Length Mod 4 > 0 Then
            strData = strData.PadLeft(strData.Length + (4 - (strData.Length Mod 4)), "0")
        End If

        Dim strVTValue As String = ""
        For i As Integer = 0 To strData.Length - 1 Step 4
            strVTValue &= CStr(Deciml(strData.Substring(i, 4), 2)).PadLeft(2, "0")
        Next

        lngValue = CLng(strVTValue)

        'End If

        Return lngValue

    End Function

    Public Function VTValue2DataReader(ByVal lngVTValue As Long) As String

        Dim RealValueDecimal As String
        Dim RealValue As String = lngVTValue.ToString

        RealValueDecimal = ""
        Dim str As String
        For i As Integer = 1 To Len(RealValue) Step 2
            str = "0000" & Binary(Mid$(RealValue, i, 2), 2)
            RealValueDecimal &= str.Substring(str.Length - 4, 4)
        Next i

        Return Deciml(RealValueDecimal, 2)

    End Function

    Private Function Binary(ByVal InptD As Object, ByVal BaseD As Object) As String

        Dim g As Object

        Try
            Binary = ""
            g = InptD
            Do
                Binary = (g Mod BaseD) & Binary
                g = g \ BaseD
            Loop Until g = 0

            Exit Function
        Catch
            Binary = ""
        End Try

    End Function

    Private Function Deciml(ByVal InptB As Object, ByVal BaseB As Object) As String

        Dim b As Object
        Dim E As Object
        Dim F As Object
        Dim A As Object
        Dim C As Object
        Dim D As Object

        Try
            b = InptB
            E = 0
            F = 0

            'loop
            Do
                A = CStr(b).Substring(CStr(b).Length - 1, 1) ' Right(b, 1)
                b = CStr(b).Substring(0, CStr(b).Length - 1) ' Left(b, Len(b) - 1)

                C = BaseB ^ F
                D = A * C
                E = E + D

                F = F + 1 'counter
            Loop Until b = ""
            'end loop

            Deciml = E

            Exit Function
        Catch
            Deciml = ""
        End Try

    End Function

#End Region

#Region "Jobs methods"

    Public Function HasEndJobPermission(ByVal intPermissionType As Integer, ByVal strEmployeesJobTimePermission As String,
                                        ByVal LogHandle As roLog) As Boolean
        '
        ' Obtenemos si el empleado tiene permisos para cerrar la fase
        '
        Dim bolRet As Boolean

        Try

            Select Case intPermissionType
                Case 3 : bolRet = True
                Case 2
                    If ExecuteScalar("@SELECT# COUNT(*) FROM Employees WHERE [ID]=" & Me.mvarID.ToString & " AND [ID] IN " & strEmployeesJobTimePermission) > 0 Then
                        bolRet = True
                    Else
                        bolRet = False
                    End If
                Case 1 : bolRet = False
                Case Else : bolRet = True
            End Select
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::CreateExternalWork:", ex)
        Finally

        End Try

        Return bolRet

    End Function

#End Region

#Region "Contracts methods"

    Public Function GetActiveContractID(ByVal LogHandle As roLog) As String

        Dim strRet As String = ""

        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String = " @SELECT# IDContract From EmployeeContracts " &
                                   " Where BeginDate <= " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                                   " And EndDate >= " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                                   " And IDEmployee = " & Me.ID.ToString

            Dim tb As DataTable = CreateDataTable(strSQL, )
            Dim oDataReader As DbDataReader = tb.CreateDataReader()

            If oDataReader.HasRows Then
                If oDataReader.Read() Then
                    strRet = oDataReader("IDContract")
                End If
            End If
            oDataReader.Close()
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roEmployee::GetActiveContractID:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return strRet

    End Function

#End Region

#End Region

End Class