Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roConstants

<DataContract()>
Public Class roParameters

#Region "Declarations - Constructor"

    Private NameParams() As String = {"StopComms", "MaxMovementHours", "FirstDate", "FunctionsTerminal",
                                      "MonthPeriod", "YearPeriod", "JobCounter", "NumMonthsAccess", "LastDateAccess",
                                      "MailAccount", "MailServer", "ServerPort", "HideDisabledFeatures",
                                      "PunchPeriodRTIn", "PunchPeriodRTOut", "EmployeePassportsIDParent",
                                      "SessionTimeout", "MailUser", "MailPWD", "MailAuthentication", "MailDomain",
                                      "UseSSL", "RequiredForbiddenPunch", "EmployeeFieldCost", "EvacuationPointUsrField",
                                      "SendPunchesEvery", "SendPunchesConnector", "SendPunchesExportFile", "SendPunchesSeparator",
                                      "SendPunchesEmployeeIdentifier", "SendPunchesIP", "SendPunchesPort", "CommsOffLine",
                                      "CommsOffLineDate", "TimeFormat", "EmergencyReportActive", "EmergencyReportKey", "EmailUsrField",
                                      "ConnectorDefaultSource", "ConnectorSourceName", "ConnectorReadingsName", "SendPunchesExportLocation",
                                      "PhotosKeepPeriod", "CloseDateAlert", "CloseDateAlertPeriod", "WeekPeriod", "DocumentsKeepPeriod", "InitialNotifierDate",
                                      "ActivatedMinBreakTime", "MinBreakTimeField", "MinBreakTimeFieldValue", "MinBreakTimeIDCause", "MinBreakTime", "BiometricDataKeepPeriod", "BlockUser", "BlockUserPeriod", "ExternAccessIPs", "DisableBiometricData"}

    Private strID As String

    Private oParameters As roCollection

    Public Sub New()

        Me.oParameters = New roCollection()

    End Sub

    Public Sub New(ByVal _ID As String, Optional ByVal bolSetDefaultValues As Boolean = True)

        Me.strID = _ID
        Me.oParameters = New roCollection()

        Me.Load(bolSetDefaultValues)

    End Sub

#End Region

#Region "properties"

    <XmlIgnore()>
    Public Property Parameter(ByVal _Parameter As Parameters) As Object
        Get
            Return Me.oParameters.Item(Me.NameParams(_Parameter))
        End Get
        Set(ByVal value As Object)
            If value IsNot Nothing Then
                Me.oParameters.Item(Me.NameParams(_Parameter)) = value
            Else
                Me.oParameters.Remove(Me.NameParams(_Parameter))
            End If
        End Set
    End Property

    <XmlIgnore()>
    Public Property Parameters_() As roCollection
        Get
            Return Me.oParameters
        End Get
        Set(ByVal value As roCollection)
            Me.oParameters = value
        End Set
    End Property
    <DataMember()>
    Public Property ParametersXML() As String
        Get
            Dim strXML As String = ""
            If Me.oParameters IsNot Nothing Then
                strXML = Me.oParameters.XML()
            End If
            Return strXML
        End Get
        Set(ByVal value As String)
            Me.oParameters = New roCollection(value)
        End Set
    End Property
    ''<XmlArray("ParametersList"), XmlArrayItem("String", GetType(String))> _
    ''Public Property ParametersList() As ArrayList
    ''    Get
    ''        Return Me.lstParameters
    ''    End Get
    ''    Set(ByVal value As ArrayList)
    ''        Me.lstParameters = value
    ''    End Set
    ''End Property
    <DataMember()>
    Public Property ID() As String
        Get
            Return Me.strID
        End Get
        Set(ByVal value As String)
            Me.strID = value
        End Set
    End Property
    <DataMember()>
    Public Property ParametersNames() As String()
        Get
            Return Me.NameParams
        End Get
        Set(ByVal value As String())
            Me.NameParams = value
        End Set
    End Property

#End Region

#Region "Methods"

    Private Sub Load(Optional ByVal bolSetDefaultValues As Boolean = True)

        Dim strSQL As String = "@SELECT# Data FROM sysroParameters WHERE [ID] = '" & Me.strID & "'"

        Dim tb As DataTable = CreateDataTable(strSQL, )
        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
            Me.oParameters = New roCollection(roTypes.Any2String(tb.Rows(0).Item("Data")).Trim)

            Try
                If Me.oParameters IsNot Nothing Then

                    'Recover en el moment de Encrypt info base de dades del correu.
                    Dim strMailServer As String = roTypes.Any2String(Me.oParameters.Item("MailServer"))
                    Dim strMailPwd As String = roTypes.Any2String(Me.oParameters.Item("MailPWD"))

                    Dim bSave As Boolean = False

                    If strMailServer <> String.Empty Then
                        Me.oParameters.Item("MailServer") = String.Empty
                        bSave = True
                    End If

                    If strMailPwd <> String.Empty Then
                        Me.oParameters.Item("MailPWD") = String.Empty
                        bSave = True
                    End If

                    If bSave Then Me.Save()

                    If Me.strID = "OPTIONS" Then
                        ' Obtenemos el valor del parametro FirstDate de la tabla correspondiente
                        strSQL = "@SELECT# isnull(Value,convert(smalldatetime, '1900/01/01', 120)) FROM sysroDateParameters WHERE ParameterName ='FirstDate'"
                        Dim strFirstDate As String = roTypes.Any2String(ExecuteScalar(strSQL))
                        If strFirstDate.Length > 0 Then
                            Dim xFirstDate As Date = roTypes.Any2Time(strFirstDate).Value

                            If xFirstDate <> CDate("1900/01/01") Then
                                ' Si tiene valor lo asignamos a la colección
                                Me.oParameters.Item("FirstDate") = xFirstDate
                            Else
                                ' En caso contrario, si no tiene valor, asignamos el valor de sysroParameters a  sysroDateParameters
                                If Me.oParameters.Exists("FirstDate") AndAlso IsDate(Me.oParameters("FirstDate")) Then
                                    strSQL = "@UPDATE# sysroDateParameters SET Value=" & roTypes.Any2Time(CDate(Me.oParameters("FirstDate"))).SQLSmallDateTime & " WHERE ParameterName ='FirstDate'"
                                    ExecuteSql(strSQL)
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
            End Try
        End If

        If bolSetDefaultValues Then Me.SetDefaultValues()

    End Sub

    Public Function Save(Optional ByVal bolUpdateCollection As Boolean = False, Optional ByVal bAudit As Boolean = False, Optional ByVal oState As roState = Nothing) As Boolean
        Dim bolRet As Boolean = False

        Dim oAuditDataOld As DataRow = Nothing
        Dim oAuditDataNew As DataRow = Nothing

        Dim tb As New DataTable("Parameters")
        Dim strSQL As String = "@SELECT# * FROM sysroParameters WHERE [ID] = '" & Me.strID & "'"
        Dim cmd As DbCommand = CreateCommand(strSQL)
        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
        da.Fill(tb)


        Dim xml As String = Me.oParameters.XML()

        If tb.Rows.Count = 1 AndAlso DataLayer.roSupport.IsXSSSafe(xml) Then

            Dim bUpdateCache As Boolean = False

            If roTypes.Any2String(tb.Rows(0).Item("Data")).Trim() <> xml Then bUpdateCache = True


            oAuditDataOld = roAudit.CloneRow(tb.Rows(0))
            tb.Rows(0).Item("Data") = xml
            da.Update(tb)
            oAuditDataNew = roAudit.CloneRow(tb.Rows(0))
            bolRet = True


            If bUpdateCache Then
                DataLayer.roCacheManager.GetInstance().UpdateParamCache()
            End If
        End If

        If bolRet AndAlso bAudit AndAlso oState IsNot Nothing Then
            bolRet = False
            ' Auditamos
            Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
            roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
            Dim oAuditAction As VTBase.Audit.Action = IIf(oAuditDataOld Is Nothing, VTBase.Audit.Action.aInsert, VTBase.Audit.Action.aUpdate)
            Dim strObjectName As String = ""

            bolRet = oState.Audit(oState.IDPassport, oAuditAction, VTBase.Audit.ObjectType.tParameters, strObjectName, tbAuditParameters, -1)
        End If

        Return bolRet

    End Function

    Private Sub SetDefaultValues()

        If Me.oParameters IsNot Nothing Then

            If Me.Parameter(Parameters.MonthPeriod) Is Nothing Then Me.Parameter(Parameters.MonthPeriod) = 1
            If Me.Parameter(Parameters.YearPeriod) Is Nothing Then Me.Parameter(Parameters.YearPeriod) = 1
            If Me.Parameter(Parameters.WeekPeriod) Is Nothing Then Me.Parameter(Parameters.WeekPeriod) = 1
            If Me.Parameter(Parameters.MovMaxHours) Is Nothing Then Me.Parameter(Parameters.MovMaxHours) = "12:00"
            If Me.Parameter(Parameters.PunchPeriodRTIn) Is Nothing Then Me.Parameter(Parameters.PunchPeriodRTIn) = 2
            If Me.Parameter(Parameters.PunchPeriodRTOut) Is Nothing Then Me.Parameter(Parameters.PunchPeriodRTOut) = 3
            If Me.Parameter(Parameters.SessionTimeout) Is Nothing Then Me.Parameter(Parameters.SessionTimeout) = 5
            If Me.Parameter(Parameters.TimeFormat) Is Nothing Then Me.Parameter(Parameters.TimeFormat) = "1"

        End If

    End Sub

#Region "Helper methods"

    Public Shared Function GetFirstDate() As Date

        Dim xRet As Date = New Date(1900, 1, 1)

        Try

            Dim oParameters As New roParameters("OPTIONS", True)
            Dim oParam As Object = oParameters.Parameter(Parameters.FirstDate)

            If oParam IsNot Nothing AndAlso IsDate(oParam) Then
                xRet = CDate(oParam)
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::GetFirstDate::", ex)
        Finally

        End Try

        Return xRet
    End Function

    ''' <summary>
    ''' Obtiene la fecha del inicio de año en función de la configuración del día y més configurado como inicio de año.
    ''' </summary>
    ''' <param name="intYear">Año del que se quiere recuperar la fecha de inicio.</param>
    ''' <param name="oActiveTransaction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetBeginYearDate(ByVal intYear As Integer) As Date
        Dim xBeginPeriod As DateTime = New Date(1900, 1, 1)
        Try

            Dim oParams As New roParameters("OPTIONS", True)
            Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
            Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
            If intMonthIniDay = 0 Then intMonthIniDay = 1
            If intYearIniMonth = 0 Then intYearIniMonth = 1

            xBeginPeriod = New DateTime(intYear, intYearIniMonth, intMonthIniDay)
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::GetBeginYearDate::", ex)
        End Try

        Return xBeginPeriod

    End Function

    ''' <summary>
    ''' Devuelve el número de días laborales que hay en el periodo indicado (se incluye fecha de inicio y fecha de fin)
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Núm. de días laborales que hay en el periodo indicado</returns>
    ''' <remarks></remarks>

    ''' <summary>
    ''' Recupera el periodo según los parametros de VisualTime y la fecha pasada
    ''' </summary>
    ''' <param name="xCurrentDate">Fecha para recuperar el periodo (según el año)</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function BeginYearPeriod(ByVal xCurrentDate As DateTime) As DateTime
        Dim xBeginPeriod As DateTime

        Try
            Dim oParameters As New roParameters("OPTIONS", True)
            Dim intMonthIniDay As Integer = oParameters.Parameter(Parameters.MonthPeriod)
            Dim intYearIniMonth As Integer = oParameters.Parameter(Parameters.YearPeriod)

            If xCurrentDate.Month > intYearIniMonth Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            ElseIf xCurrentDate.Month = intYearIniMonth And xCurrentDate.Day >= intMonthIniDay Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            Else
                xBeginPeriod = New DateTime(xCurrentDate.Year - 1, intYearIniMonth, intMonthIniDay)
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::BeginYearPeriod::", ex)
        End Try

        Return xBeginPeriod
    End Function

    ''' <summary>
    ''' Recupera el periodo según los parametros de VisualTime y la fecha pasada (1 año natural)
    ''' </summary>
    ''' <param name="xCurrentDate">Fecha para recuperar el periodo</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EndYearPeriod(ByVal xCurrentDate As DateTime) As DateTime
        Dim xEndPeriod As DateTime
        Try

            Dim oParameters As New roParameters("OPTIONS", True)
            Dim intMonthIniDay As Integer = oParameters.Parameter(Parameters.MonthPeriod)
            Dim intYearIniMonth As Integer = oParameters.Parameter(Parameters.YearPeriod)

            Dim xBeginPeriod As DateTime
            If xCurrentDate.Month > intYearIniMonth Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            ElseIf xCurrentDate.Month = intYearIniMonth And xCurrentDate.Day >= intMonthIniDay Then
                xBeginPeriod = New DateTime(xCurrentDate.Year, intYearIniMonth, intMonthIniDay)
            Else
                xBeginPeriod = New DateTime(xCurrentDate.Year - 1, intYearIniMonth, intMonthIniDay)
            End If
            xEndPeriod = xBeginPeriod.AddYears(1).AddDays(-1)
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::EndYearPeriod::", ex)
        End Try

        Return xEndPeriod
    End Function

    Public Shared Function GetDateParameter(ByVal parameterName As String) As Date
        Dim strSQL As String
        Dim valueDate As DateTime = Nothing
        Try
            strSQL = $"@SELECT# Value FROM sysroDateParameters WHERE [ParameterName] = '{parameterName}'"
            valueDate = ExecuteScalar(strSQL)
            If valueDate = Nothing Then valueDate = Date.MinValue
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::GetDateParameter::", ex)
        End Try
        Return valueDate
    End Function

    Public Shared Function SaveDateParameter(ByVal parameterName As String, valueDate As DateTime) As Boolean
        Dim strSQL As String
        Dim returnValue As Boolean = False
        Try
            strSQL = $"IF EXISTS (@SELECT# 1 FROM sysroDateParameters WHERE ParameterName = '{parameterName}')" &
            $"              BEGIN " &
            $"                  @UPDATE# sysroDateParameters SET Value = {roTypes.Any2Time(valueDate).SQLSmallDateTime}" &
            $"                  WHERE ParameterName = '{parameterName}';" &
            $"              END" &
            $"         ELSE" &
            $"              BEGIN" &
            $"                  @INSERT# INTO sysroDateParameters (ParameterName, Value)" &
            $"                  VALUES('{parameterName}', {roTypes.Any2Time(valueDate).SQLSmallDateTime});" &
            $"              END"

            returnValue = ExecuteSql(strSQL)
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roParameters::SaveDateParameter::", ex)
        End Try
        Return returnValue
    End Function

#End Region

#End Region

End Class