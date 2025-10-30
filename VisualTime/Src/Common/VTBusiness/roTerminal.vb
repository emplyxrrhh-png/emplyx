Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Terminal

    <DataContract,
        KnownType(GetType(roTerminal.roTerminalReader))>
    Public Class roTerminal

#Region "Declarations - Constructor"

        Private oState As roTerminalState

        Private intID As Integer
        Private oData As DataSet

        Private hasSirens As String

        Private _Readers As ArrayList
        Private oSirens As Generic.List(Of roTerminalSiren)

        Private dLastUpdateLastUpdate As DateTime = Now
        Private dLastUpdateLastAction As DateTime = Now
        Private bLiveInstalled As Boolean = False
        Private sTerminalPath As String = ""
        Private _AdvancedParameters As New Dictionary(Of String, String)
        Private terminalEnabled As Boolean = True
        Private terminalDeleted As Boolean = False

        Public Sub New()
            Me.oState = New roTerminalState
            ' Valores por defecto
            Me.intID = -1
            Me.oData = Nothing
            _Readers = New ArrayList
            Me.oSirens = New Generic.List(Of roTerminalSiren)
            Me.hasSirens = ""
            Load(, False)
        End Sub

        Public Sub New(ByVal _State As roTerminalState, Optional ByVal bLiteCharge As Boolean = False)
            Me.oState = _State
            ' Valores por defecto
            Me.intID = -1
            Me.oData = Nothing
            _Readers = New ArrayList
            Me.oSirens = New Generic.List(Of roTerminalSiren)
            Me.hasSirens = ""
            Load(False, False, bLiteCharge)
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roTerminalState, Optional ByVal bAudit As Boolean = False, Optional ByVal bLiteCharge As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            _Readers = New ArrayList
            Me.oSirens = New Generic.List(Of roTerminalSiren)
            Me.hasSirens = ""
            Me.Load(, bAudit, bLiteCharge)
        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roTerminalState, Optional ByVal bAudit As Boolean = False, Optional ByVal bLiteCharge As Boolean = False)

            Me.oState = _State

            If Not IsDBNull(_Row("ID")) Then
                Me.intID = _Row("ID")
            Else
                Me.intID = -1
            End If

            _Readers = New ArrayList
            Me.oSirens = New Generic.List(Of roTerminalSiren)

            If Me.Load(, bAudit, bLiteCharge) Then

                Dim oTerminalData As DataRow
                If Me.Data.Tables("Terminals").Rows.Count = 0 Then
                    oTerminalData = Me.Data.Tables("Terminals").NewRow
                Else
                    oTerminalData = Me.Data.Tables("Terminals").Rows(0)
                End If
                Me.LoadRow(_Row, oTerminalData)

            End If

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roTerminalState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTerminalState)
                Me.oState = value
                If Me._Readers IsNot Nothing Then
                    For Each oReader As roTerminalReader In Me._Readers
                        oReader.State = Me.oState
                    Next
                End If
                If Me.oSirens IsNot Nothing Then
                    For Each oSiren As roTerminalSiren In Me.oSirens
                        oSiren.State = Me.oState
                    Next
                End If
            End Set
        End Property

        <DataMember(Order:=0)>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
                If Me._Readers IsNot Nothing Then
                    ' Informamos del id del terminal a todos los lectores
                    For Each oReader As roTerminalReader In Me._Readers
                        oReader.IDTerminal = Me.intID
                    Next
                End If
                If Me.oSirens IsNot Nothing Then
                    ' Informamos del id del terminal a todas las sirenas
                    For Each oSiren As roTerminalSiren In Me.oSirens
                        oSiren.IDTerminal = Me.intID
                    Next
                End If
            End Set
        End Property

        <XmlArray("Readers"), XmlArrayItem("roTerminalReader", GetType(roTerminalReader))>
        <DataMember>
        Public Property Readers() As ArrayList
            Get
                Return Me._Readers
            End Get
            Set(ByVal value As ArrayList)
                Me._Readers = value
            End Set
        End Property

        <DataMember>
        Public Property Data() As DataSet
            Get
                Return Me.oData
            End Get
            Set(ByVal value As DataSet)
                Me.oData = value
            End Set
        End Property

        <DataMember>
        Public Property Description() As String
            Get
                Return Any2String(ReturnField("Description"))
            End Get
            Set(ByVal value As String)
                PutValue("Description", value)
            End Set
        End Property

        <DataMember>
        Public Property Type() As String
            Get
                Return Any2String(ReturnField("Type"))
            End Get
            Set(ByVal value As String)
                PutValue("Type", value)
            End Set
        End Property

        <DataMember>
        Public Property Behavior() As String
            Get
                Return Any2String(ReturnField("Behavior"))
            End Get
            Set(ByVal value As String)
                PutValue("Behavior", value)
            End Set
        End Property

        <DataMember>
        Public Property Location() As String
            Get
                Return Any2String(ReturnField("Location"))
            End Get
            Set(ByVal value As String)
                PutValue("Location", value)
            End Set
        End Property

        <DataMember>
        Public Property LastUpdate() As Nullable(Of DateTime)
            Get
                If ReturnField("LastUpdate") Is System.DBNull.Value Then
                    Dim lAction As Nullable(Of DateTime)
                    Return lAction
                Else
                    Return ReturnField("LastUpdate")
                End If
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue Then
                    PutValue("LastUpdate", value.Value)
                Else
                    PutValue("LastUpdate", DBNull.Value)
                End If
            End Set
        End Property

        <DataMember>
        Public Property LastStatus() As String
            Get
                Dim strLastStatus As String = Any2String(ReturnField("LastStatus"))
                Return strLastStatus
            End Get
            Set(ByVal value As String)
                PutValue("LastStatus", value)
            End Set
        End Property

        <DataMember>
        Public Property SupportedModes() As String
            Get
                Return Any2String(ReturnField("SupportedModes"))
            End Get
            Set(ByVal value As String)
                PutValue("SupportedModes", value)
            End Set
        End Property

        <DataMember>
        Public Property SupportedOutputs() As String
            Get
                Return Any2String(ReturnField("SupportedOutputs"))
            End Get
            Set(ByVal value As String)
                PutValue("SupportedOutputs", value)
            End Set
        End Property

        <DataMember>
        Public Property FirmVersion() As String
            Get
                Return Any2String(ReturnField("FirmVersion"))
            End Get
            Set(ByVal value As String)
                PutValue("FirmVersion", value)
            End Set
        End Property

        <DataMember>
        Public Property SupportedSirens() As String
            Get
                Return Me.hasSirens
            End Get
            Set(ByVal value As String)
                'PutValue("SupportedSirens", value)
            End Set
        End Property

        <DataMember>
        Public Property SirensOutput() As Integer
            Get
                Return Any2Integer(ReturnField("SirensOutput"))
            End Get
            Set(ByVal value As Integer)
                PutValue("SirensOutput", value)
            End Set
        End Property

        <DataMember>
        Public Property Functions() As String
            Get
                Return Any2String(ReturnField("Functions"))
            End Get
            Set(ByVal value As String)
                PutValue("Functions", value)
            End Set
        End Property

        <DataMember>
        Public Property LastAction() As Nullable(Of DateTime)
            Get
                If ReturnField("LastAction") Is System.DBNull.Value Then
                    Dim lAction As Nullable(Of DateTime)
                    Return lAction
                Else
                    Return ReturnField("LastAction")
                End If
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue Then
                    PutValue("LastAction", value.Value)
                Else
                    PutValue("LastAction", DBNull.Value)
                End If
            End Set
        End Property

        <DataMember>
        Public Property Other() As String
            Get
                Return Any2String(ReturnField("Other"))
            End Get
            Set(ByVal value As String)
                PutValue("Other", value)
            End Set
        End Property

        <DataMember>
        Public Property SecurityOptionsDefinition() As String
            Get
                Return Any2String(ReturnField("SecurityOptions"))
            End Get
            Set(ByVal value As String)
                PutValue("SecurityOptions", value)
            End Set
        End Property

        <DataMember>
        Public Property Model() As String
            Get
                Return Any2String(ReturnField("Model"))
            End Get
            Set(ByVal value As String)
                PutValue("Model", value)
            End Set
        End Property

        <DataMember>
        Public Property SerialNumber() As String
            Get
                Return Any2String(ReturnField("SerialNumber"))
            End Get
            Set(ByVal value As String)
                PutValue("SerialNumber", value)
            End Set
        End Property

        <DataMember>
        Public Property RegistrationCode() As String
            Get
                Return Any2String(ReturnField("RegistrationCode"))
            End Get
            Set(ByVal value As String)
                PutValue("RegistrationCode", value)
            End Set
        End Property

        <DataMember>
        Public Property RigidMode() As Integer
            Get
                Return Any2Integer(ReturnField("RigidMode"))
            End Get
            Set(ByVal value As Integer)
                PutValue("RigidMode", value)
            End Set
        End Property

        <DataMember>
        Public Property skin() As Nullable(Of Integer)
            Get
                Return Any2Integer(ReturnField("skin"))
            End Get
            Set(ByVal value As Nullable(Of Integer))
                PutValue("skin", value)
            End Set
        End Property

        <DataMember(Order:=3)>
        Public Property AllowAntiPassBack() As Boolean
            Get
                Return Any2Boolean(ReturnField("AllowAntiPassBack"))
            End Get
            Set(ByVal value As Boolean)
                PutValue("AllowAntiPassBack", value)
            End Set
        End Property

        <DataMember>
        Public Property IsDifferentZoneTime() As Boolean
            Get
                Return Any2Boolean(ReturnField("IsDifferentZoneTime"))
            End Get
            Set(ByVal value As Boolean)
                PutValue("IsDifferentZoneTime", value)
            End Set
        End Property

        <DataMember>
        Public Property ZoneTime() As Integer
            Get
                Return Any2Integer(ReturnField("ZoneTime"))
            End Get
            Set(ByVal value As Integer)
                PutValue("ZoneTime", value)
            End Set
        End Property

        <DataMember>
        Public Property TimeZoneName() As String
            Get
                Dim strRet As String = Any2String(ReturnField("TimeZoneName"))
                If strRet = "" Then
                    ' Obtiene la zona horaria local
                    strRet = TimeZoneInfo.Local.Id
                End If
                Return strRet
            End Get
            Set(ByVal value As String)
                PutValue("TimeZoneName", value)
            End Set
        End Property

        <DataMember(Order:=4)>
        Public Property AutoDaylight() As Boolean
            Get
                Return Any2Boolean(ReturnField("AutoDaylight"))
            End Get
            Set(ByVal value As Boolean)
                PutValue("AutoDaylight", value)
            End Set
        End Property

        <DataMember>
        Public Property AllowCustomButton() As Boolean
            Get
                Return Any2Boolean(ReturnField("AllowCustomButton"))
            End Get
            Set(ByVal value As Boolean)
                PutValue("AllowCustomButton", value)
            End Set
        End Property

        <DataMember>
        Public Property CustomLabel() As String
            Get
                Return Any2String(ReturnField("CustomLabel"))
            End Get
            Set(ByVal value As String)
                PutValue("CustomLabel", value)
            End Set
        End Property

        <DataMember>
        Public Property CustomOutput() As Integer
            Get
                Return Any2Integer(ReturnField("CustomOutput"))
            End Get
            Set(ByVal value As Integer)
                PutValue("CustomOutput", value)
            End Set
        End Property

        <DataMember>
        Public Property CustomDuration() As Integer
            Get
                Return Any2Integer(ReturnField("CustomDuration"))
            End Get
            Set(ByVal value As Integer)
                PutValue("CustomDuration", value)
            End Set
        End Property

        <DataMember>
        Public Property CustomField() As String
            Get
                Return Any2String(ReturnField("CustomField"))
            End Get
            Set(ByVal value As String)
                PutValue("CustomField", value)
            End Set
        End Property

        <DataMember>
        Public Property CustomFieldValue() As String
            Get
                Return Any2String(ReturnField("CustomFieldValue"))
            End Get
            Set(ByVal value As String)
                PutValue("CustomFieldValue", value)
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property ConfData() As String
            Get
                Return Any2String(ReturnField("ConfData"))
            End Get
            Set(ByVal value As String)
                PutValue("ConfData", value)
            End Set
        End Property

        <DataMember(Order:=2)>
        Public Property ConfigurationTable() As DataSet
            Get
                Dim ds As DataSet = Nothing
                Dim tbConfig As DataTable = Me.GetConfiguration()
                If tbConfig.DataSet Is Nothing Then
                    ds = New DataSet
                    ds.Tables.Add(tbConfig)
                Else
                    ds = tbConfig.DataSet
                End If
                Return ds
            End Get
            Set(ByVal value As DataSet)
                If value IsNot Nothing AndAlso value.Tables.Count > 0 Then
                    Me.SetConfiguration(value.Tables(0))
                End If
            End Set
        End Property

        <DataMember>
        Public Property AllowMoveReason() As Boolean
            Get
                Return Any2Boolean(ReturnField("AllowMoveReason"))
            End Get
            Set(ByVal value As Boolean)
                PutValue("AllowMoveReason", value)
            End Set
        End Property

        <IgnoreDataMember>
        Public ReadOnly Property Path(Optional bCreateNewFolderIfNotExists As Boolean = True) As String
            Get
                If Not IO.Directory.Exists(sTerminalPath) AndAlso bCreateNewFolderIfNotExists Then
                    IO.Directory.CreateDirectory(sTerminalPath)
                End If
                If (sTerminalPath.Contains("Terminal-1") OrElse sTerminalPath.Contains("Terminal0")) AndAlso Me.ID > 0 Then
                    ' Actualizo Path
                    Dim oSettings As New roSettings
                    Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                    If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                        _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                    End If
                    sTerminalPath = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & Me.ID.ToString
                End If
                Return sTerminalPath
            End Get
        End Property

        <IgnoreDataMember>
        Public Property ReaderByIndex(ByVal _Index As Integer) As roTerminalReader
            Get
                Return LoadReaderByIndex(_Index)
            End Get
            Set(ByVal value As roTerminalReader)

            End Set
        End Property

        <IgnoreDataMember>
        Public Property ReaderByID(ByVal _ID As Integer) As roTerminalReader
            Get
                For Each reader As roTerminalReader In _Readers
                    If reader.ID = _ID Then Return reader
                Next
                Return Nothing
            End Get
            Set(ByVal value As roTerminalReader)
                For Each reader As roTerminalReader In _Readers
                    If reader.ID = _ID Then
                        reader = value
                        Exit For
                    End If
                Next
            End Set
        End Property

        <IgnoreDataMember>
        Public Property AdvancedParameters As Dictionary(Of String, String)
            Get
                Return _AdvancedParameters
            End Get
            Set(value As Dictionary(Of String, String))
                _AdvancedParameters = value
            End Set
        End Property

        <IgnoreDataMember>
        Public Property ReadersCount() As Integer
            Get
                Dim intCount As Integer = 0
                If oData.Tables.Contains("TerminalReaders") Then
                    intCount = oData.Tables("TerminalReaders").Rows.Count
                End If
                Return intCount
            End Get
            Set(value As Integer)

            End Set
        End Property

        <DataMember>
        Public Property Sirens() As Generic.List(Of roTerminalSiren)
            Get
                Return Me.oSirens
            End Get
            Set(ByVal value As Generic.List(Of roTerminalSiren))
                Me.oSirens = value
            End Set
        End Property

        <DataMember>
        Public Property Enabled As Boolean
            Get
                Return terminalEnabled
            End Get
            Set(ByVal value As Boolean)
                Me.terminalEnabled = value
            End Set
        End Property

        <DataMember>
        Public Property Deleted As Boolean
            Get
                Return terminalDeleted
            End Get
            Set(ByVal value As Boolean)
                Me.terminalDeleted = value
            End Set
        End Property
#End Region

#Region "Methods"

        Public Shared Function SaveNFCReader(ByVal terminalID As Integer, ByVal readerid As Integer, ByVal idzone As Integer, ByVal nfc As String, ByVal idmode As Integer, ByVal description As String, ByVal oState As roTerminalState, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim strSQL As String
            Try

                bolRet = True

                If readerid <> 0 Then
                    strSQL = "@UPDATE# TerminalReaders SET idzone = '" & idzone & "', nfc='" & nfc & "', description='" & description & "' WHERE idterminal = '" & terminalID & "' AND ID = " & readerid
                Else

                    strSQL = "@insert# into TerminalReaders (IDTerminal, ID, Description, IDZone, PictureX, PictureY, Mode, Output, Duration, RequestPin, AccessKey, type, IDActivity, InteractionMode, InteractionAction, InvalidOutput, InvalidDuration, CustomButtons,UseDispKey,OHP,ValidationMode,InteractiveConfig,IDCamera,IDZoneOut,PictureXOut,PictureYOut,IDCameraOut,IDCostCenter ,NFC) values " &
                        "('" & terminalID & "', (@select# cASE WHEN max(ID) is null THEN 1 ELSE max(ID) +1 end from TerminalReaders where IDTerminal='" & terminalID & "'), '" & description & "', '" & idzone & "', NULL, NULL, 'TA', 0, 0, 0, 0, 'RX', NULL, 'Blind', 'X', 0, 0, null, 1, 0, 'Local', NULL, NULL, NULL, NULL, NULL, NULL, NULL, '" & nfc & "')"
                End If

                bolRet = ExecuteScalar(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalReader::SaveNFCReaders")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalReader::SaveNFCReaders")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente número de Id. de Terminal
        ''' </summary>
        ''' <param name="oActiveConnection">Conexión establecida</param>
        ''' <returns>Devuelve el núm. siguiente de terminal (inferior a 90), en caso de error -1</returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveTerminalNextID(ByVal _State As roTerminalState) As Integer
            Dim intRet As Integer = -1

            Try

                Dim tb As New DataTable("Terminals")
                Dim strSQL As String = "@SELECT# MAX(ID) + 1 as NextID FROM Terminals"

                If Not ExecuteScalar(strSQL) Is System.DBNull.Value Then
                    intRet = ExecuteScalar(strSQL)
                Else
                    intRet = 1
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminal:RetrieveTerminalNextID")
                intRet = -1
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminal:RetrieveTerminalNextID")
                intRet = -1
            Finally

            End Try
            Return intRet
        End Function

        Public Function Load(Optional ByVal bolAddNewTerminal As Boolean = True, Optional ByVal bAudit As Boolean = False, Optional ByVal bLiteCharge As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bLegacyRestrictionModeAllowed As Boolean = False

            Try
                roTrace.GetInstance.InitTraceEvent()
                Me.oData = New DataSet()

                Dim strSQL As String = "@SELECT# * FROM Terminals " &
                                       "WHERE ID = " & Me.ID
                Dim tbTerminal As DataTable = CreateDataTable(strSQL, "Terminals")
                strSQL = "@SELECT# * FROM TerminalReaders " &
                         "WHERE IDTerminal = " & Me.ID & " " &
                         "ORDER BY ID"
                Dim tbReaders As DataTable = CreateDataTable(strSQL, "TerminalReaders")

                ' Miro debe estar habilitado el modo de restricción de presencia antiguo (descripción de grupos y/o lista de empleados a nivel de lector)
                Try
                    strSQL = "@SELECT# SUM(total) total from (@SELECT# COUNT(*) total from TerminalReaderEmployees UNION @SELECT# COUNT(*) total from Groups where DescriptionGroup like '%TERMINAL=%') aux"
                    Dim tbControl As DataTable = Nothing
                    tbControl = CreateDataTable(strSQL, "Control")
                    If tbControl IsNot Nothing AndAlso tbControl.Rows.Count > 0 Then
                        bLegacyRestrictionModeAllowed = Any2Boolean(tbControl.Rows(0).Item("total"))
                    End If
                Catch ex As Exception
                End Try

                strSQL = "@SELECT# TerminalReaderEmployees.IDTerminal, TerminalReaderEmployees.IDReader, TerminalReaderEmployees.IDEmployee FROM TerminalReaderEmployees " &
                         "INNER JOIN Employees ON TerminalReaderEmployees.IDEmployee = Employees.ID WHERE (TerminalReaderEmployees.IDTerminal = " & Me.ID & ") ORDER BY Employees.Name"

                Dim tbReaderEmployees As DataTable = CreateDataTable(strSQL, "TerminalReaderEmployees")

                'Si no existeix el Terminal, creem el DataRow
                If tbTerminal.Rows.Count = 0 Then
                    If bolAddNewTerminal OrElse Me.ID = -1 Then
                        Dim dRow As DataRow = tbTerminal.NewRow
                        If Me.ID <> -1 Then
                            dRow("ID") = Me.ID
                        End If
                        tbTerminal.Rows.Add(dRow)
                        bolRet = True
                    Else
                        bolRet = False
                    End If
                Else
                    bolRet = True
                End If

                If bolRet Then
                    Me.oData.Tables.Clear()
                    'Afegim al DataSet
                    Me.oData.Tables.Add(tbTerminal)
                    Me.oData.Tables.Add(tbReaders)

                    'Carreguem els readers
                    Me._Readers.Clear()
                    Dim oReader As roTerminalReader = Nothing
                    For Each tbRow As DataRow In tbReaders.Rows
                        oReader = New roTerminalReader(tbRow, tbReaderEmployees.Select("IDReader = " & tbRow("ID")), Me.oState, bLegacyRestrictionModeAllowed)
                        _Readers.Add(oReader)

                        If (Me.hasSirens = "") Then
                            strSQL = "@SELECT# SupportedSirens FROM sysroReaderTemplates WHERE type = '" & tbTerminal.Rows(0).Item("Type") & "'"  ' "AND ScopeMode = '" & tbRow.Item("Mode") & "'"
                            Dim sirenEnabled As DataTable = CreateDataTable(strSQL, "")

                            If sirenEnabled IsNot Nothing AndAlso sirenEnabled.Rows.Count > 0 Then
                                For Each oSirenRow As DataRow In sirenEnabled.Rows
                                    If oSirenRow("SupportedSirens") IsNot DBNull.Value AndAlso oSirenRow("SupportedSirens") <> "" Then
                                        Me.hasSirens = oSirenRow("SupportedSirens")
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                    Next

                    Me.oSirens = roTerminalSiren.GetTerminalSirens(Me.ID, Me.oState)

                    If Me.ID = -1 Then
                        Me.Enabled = True
                    Else
                        Me.Enabled = roTypes.Any2Boolean(tbTerminal.Rows(0).Item("Enabled"))
                    End If

                    bolRet = True

                    ' Carga de parámetros avanzados
                    Try
                        ' Comedor
                        Dim pAdvParameter As AdvancedParameter.roAdvancedParameter
                        If Me.ReaderByID(1) IsNot Nothing AndAlso Me.ReaderByID(1).Mode.IndexOf("DIN") > -1 Then
                            ' Guardar salida en fichaje de acceso a comedor
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("AttOutOnDinnerIn", Nothing, False)
                            If pAdvParameter IsNot Nothing Then
                                Me._AdvancedParameters.Add("AttOutOnDinnerIn", roTypes.Any2String(pAdvParameter.Value))
                            End If

                            ' Múltiples fichajes en mismo turno de comedor
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("AllowMultipleDinnerInTurn", Nothing, False)
                            If pAdvParameter IsNot Nothing Then
                                Me._AdvancedParameters.Add("AllowMultipleDinnerInTurn", roTypes.Any2String(pAdvParameter.Value))
                            End If

                            ' Tiquet de comedor en fichero
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("DinnerPrintOnFile", Nothing, False)
                            If pAdvParameter IsNot Nothing Then
                                Me._AdvancedParameters.Add("DinnerPrintOnFile", roTypes.Any2String(pAdvParameter.Value))
                            End If
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("DinnerPrintCustomDesign", Nothing, False)
                            If pAdvParameter IsNot Nothing Then
                                Me._AdvancedParameters.Add("DinnerPrintCustomDesign", roTypes.Any2String(pAdvParameter.Value))
                            End If
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("AllowDiningTurnSelection", Nothing, False)
                            If pAdvParameter IsNot Nothing Then
                                Me._AdvancedParameters.Add("AllowDiningTurnSelection", roTypes.Any2String(pAdvParameter.Value))
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Description}", Me.Description, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTerminal, Me.Description, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:Load:Terminal " & Me.ID.ToString)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:Load:Terminal " & Me.ID.ToString)
            Finally
                roTrace.GetInstance.AddTraceEvent("Business terminal loaded")
            End Try

            Return bolRet

        End Function

        Private Function LoadRow(ByVal _SourceRow As DataRow, ByVal _DestRow As DataRow) As Boolean

            Dim bolRet As Boolean = False

            Try
                For nCol As Integer = 0 To _SourceRow.Table.Columns.Count - 1
                    _DestRow(nCol) = _SourceRow(nCol)
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:LoadRow")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:LoadRow")
            End Try

            Return bolRet

        End Function

        Private Function CheckIPNum(ByVal strIP As String) As Boolean
            Dim bolRet As Boolean = True
            Try
                If Not IsNumeric(strIP) Then
                    bolRet = False
                Else
                    If CInt(strIP) < 0 Or CInt(strIP) > 255 Then
                        bolRet = False
                    End If
                End If
            Catch ex As Exception
                bolRet = False
            End Try
            Return bolRet
        End Function

        Private Function CheckIPPort(ByVal strPort As String) As Boolean
            Dim bolRet As Boolean = True
            Try
                If Not IsNumeric(strPort) Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
            End Try
            Return bolRet
        End Function

        Public Function SaveTerminalResgistrationCode(ByVal strRegistrationCode As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String
                bolRet = True

                ' En caso de estar regsitrando un terminal rx1, verifico que si tengo terminales mx8PLUS o LITE, estos tienen una versión suficiente de firmware (2.9.4.2 o superior)
                Dim bIncompatibleMx8Firmware As Boolean = False
                If Me.Type.ToUpper = "RX1" Then
                    Dim tbAux As New DataTable
                    strSQL = "@SELECT# FirmVersion, * from terminals where type = 'mx8'"
                    tbAux = CreateDataTable(strSQL)
                    Dim sFirmVersion As String = String.Empty
                    Dim v1 As Integer = 0
                    Dim v2 As Integer = 0
                    Dim v3 As Integer = 0
                    Dim v4 As Integer = 0
                    Dim iversion As Integer = 0
                    Try
                        If tbAux.Rows.Count > 0 Then
                            ' Hay algún mx8. Debe ser compatible
                            For Each oAuxRow As DataRow In tbAux.Rows
                                sFirmVersion = roTypes.Any2String(oAuxRow("FirmVersion"))
                                If sFirmVersion.Trim = "" Then
                                    bIncompatibleMx8Firmware = True
                                    Exit For
                                Else
                                    v1 = sFirmVersion.Split(".")(0)
                                    v2 = sFirmVersion.Split(".")(1)
                                    v3 = sFirmVersion.Split(".")(2)
                                    v4 = sFirmVersion.Split(".")(3)
                                    iversion = v1 * 1000 + v2 * 100 + v3 * 10 + v4
                                    If iversion < 2942 Then
                                        bIncompatibleMx8Firmware = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        bIncompatibleMx8Firmware = True
                    End Try

                    If bIncompatibleMx8Firmware Then
                        Me.oState.ReturnCode = "5-T100-001"
                        Me.oState.Result = TerminalBaseResultEnum.IncompatibleTerminalsExists
                        Return False
                    End If
                End If

                strSQL = "@UPDATE# Terminals SET RegistrationCode = '" & strRegistrationCode & "' WHERE [Type] = '" & Me.Type.ToUpper & "' AND ID = " & Me.ID.ToString
                If Not CheckTerminalSerialNum(strRegistrationCode, Me.SerialNumber) OrElse Not ExecuteSql(strSQL) Then
                    Me.oState.Result = TerminalBaseResultEnum.InvalidRegistrationCode
                    bolRet = False
                Else
                    'eliminamos la alerta de usuario
                    Dim oUserTaskState As New UserTask.roUserTaskState
                    roBusinessState.CopyTo(Me.oState, oUserTaskState)
                    Dim oTask As New UserTask.roUserTask("USERTASK:\\MXC_NOTREGISTERED" & Me.ID, oUserTaskState)
                    oTask.Delete()
                End If

                If bolRet And bAudit Then
                    bolRet = False

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Terminals")
                    strSQL = "@SELECT# * FROM Terminals WHERE ID = " & Me.ID
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    If tb.Rows.Count > 0 Then
                        oAuditDataNew = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oAuditDataNew)

                        oAuditDataNew("RegistrationCode") = strRegistrationCode

                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)

                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tTerminal, Me.Description, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:SaveTerminalResgistrationCode")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:SaveTerminalResgistrationCode")
            End Try

            Return bolRet
        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Dim strSQL As String

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = SecurityResultEnum.XSSvalidationError
                    Return False
                End If

                bolRet = True

                ' Verificamos que sólo haya definido un terminal de tipo 'Portal del empleado' LIVEPORTAL
                If Me.Type.ToUpper = "LIVEPORTAL" Then
                    strSQL = $"@SELECT# * FROM Terminals WHERE [Type] = '{Me.Type.ToUpper}' AND ID <> {Me.ID.ToString} AND ISNULL([Other], '') <> 'APP' AND (Deleted = 0 OR Deleted IS NULL)"
                    Dim tbLivePortal As DataTable = CreateDataTable(strSQL)
                    If tbLivePortal IsNot Nothing AndAlso tbLivePortal.Rows.Count > 0 Then
                        Me.oState.Result = TerminalBaseResultEnum.LivePortalTerminalAlreadyExist
                        bolRet = False
                    End If
                End If

                'VALIDAR LA LOCATION
                If Me.Location <> "" Then
                    Dim strIP1 As String = "", strIP2 As String = "", strIP3 As String = "", strIP4 As String = ""
                    If Me.Location.Split(".").Length = 4 Then

                        'comprobar si tiene puerto
                        If Me.Location.IndexOf(":") <> -1 Then
                            If Me.Location.LastIndexOf(".") < Me.Location.IndexOf(":") Then
                                Dim strPort As String = ""
                                strIP1 = Me.Location.Split(".")(0)
                                strIP2 = Me.Location.Split(".")(1)
                                strIP3 = Me.Location.Split(".")(2)
                                strIP4 = Me.Location.Split(".")(3)
                                strPort = strIP4.Substring(strIP4.IndexOf(":") + 1)
                                strIP4 = strIP4.Remove(strIP4.IndexOf(":"))

                                If Not CheckIPNum(strIP1) Or Not CheckIPNum(strIP2) Or Not CheckIPNum(strIP3) Or Not CheckIPNum(strIP4) Or Not CheckIPPort(strPort) Then
                                    Me.oState.Result = TerminalBaseResultEnum.InvalidConfiguration
                                    bolRet = False
                                End If
                            Else
                                Me.oState.Result = TerminalBaseResultEnum.InvalidConfiguration
                                bolRet = False
                            End If
                        Else
                            strIP1 = Me.Location.Split(".")(0)
                            strIP2 = Me.Location.Split(".")(1)
                            strIP3 = Me.Location.Split(".")(2)
                            strIP4 = Me.Location.Split(".")(3)
                            If Not CheckIPNum(strIP1) OrElse Not CheckIPNum(strIP2) OrElse Not CheckIPNum(strIP3) OrElse Not CheckIPNum(strIP4) Then
                                Me.oState.Result = TerminalBaseResultEnum.InvalidConfiguration
                                bolRet = False
                            End If

                        End If

                    End If

                End If

                If bolRet Then

                    bolRet = False

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Terminals")
                    strSQL = "@SELECT# * FROM Terminals WHERE ID = " & Me.ID
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    Me.LoadRow(Me.oData.Tables("Terminals").Rows(0), oRow)

                    If oRow("ID") Is System.DBNull.Value OrElse oRow("ID") = -1 Then 'Si es null o no s'ha insertat ID
                        Me.ID = roTerminal.RetrieveTerminalNextID(oState)
                        oRow("ID") = Me.ID
                    End If

                    If bolIsNew OrElse (Not oRow("Enabled") AndAlso Me.Enabled) Then
                        ' Marco en Cosmos que el terminal está activo
                        Dim oRepo As New Azure.roAzureTerminalRepository
                        oRepo.UpdateTerminalToCompany(Me.SerialNumber, Azure.RoAzureSupport.GetCompanyName(), Enabled)
                    ElseIf oRow("Enabled") AndAlso Not Me.Enabled Then
                        ' Marco en Cosmos que el terminal no está activo, y elimino toda información de sincronización
                        roTerminal.ResetTerminalSyncData(Me.ID)
                        Dim oRepo As New Azure.roAzureTerminalRepository
                        oRepo.UpdateTerminalToCompany(Me.SerialNumber, Azure.RoAzureSupport.GetCompanyName(), Enabled)
                    End If

                    oRow("Enabled") = Me.Enabled
                    oRow("Deleted") = Me.Deleted
                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If

                    da.Update(tb)

                    'Si acabo de crear el Portal del Empleado, creo también el de sistema para fichajes desde la APP
                    If bolIsNew AndAlso Me.Type.ToUpper = "LIVEPORTAL" Then
                        'Hago una copia de la oRow original
                        Dim oRowApp As DataRow = tb.NewRow
                        Me.LoadRow(oRow, oRowApp)
                        oRowApp("Description") = $"{Me.Description} APP"
                        oRowApp("Other") = $"APP"
                        oRowApp("ID") = roTerminal.RetrieveTerminalNextID(oState)
                        tb.Rows.Add(oRowApp)
                        da.Update(tb)
                    ElseIf Not bolIsNew AndAlso Me.Type.ToUpper = "LIVEPORTAL" Then
                        'Si existe un Portal APK, le cambio el nombre para que coincida con el del Portal del empleado
                        strSQL = $"@UPDATE# Terminals SET Description = '{oRow("Description")} APP' WHERE [Type] = 'LIVEPORTAL' AND [Other] = 'APP'"
                        ExecuteSql(strSQL)
                    End If

                    'Creem / Actualitzem els TerminalReaders
                    If Me.Type.ToUpper <> "NFC" Then
                        bolRet = roTerminalReader.SaveTerminalReaders(Me.ID, Me._Readers, Me.oState, False, bolIsNew)
                        If bolRet Then
                            bolRet = roTerminalSiren.SaveTerminalSirens(Me.ID, Me.oSirens, Me.oState, False)
                        End If
                    Else
                        bolRet = True
                    End If
                    oAuditDataNew = oRow

                    If Me.Type.ToUpper <> "NFC" Then
                        ' Si es nuevo, borro posibles datos que existiesen en el sistema para un terminal con el mismo ID (por ejemplo, poque hubiese existido, y se hubiese borrado, dejando carpeta readings, posibles tareas ...)
                        If bolIsNew Then
                            ' Borro carpeta de readings y VTTD
                            Try
                                roTerminal.ResetTerminalSyncData(Me.ID)
                            Catch ex As Exception
                            End Try
                        End If
                    End If

                    If bolRet AndAlso bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Description")
                        Else
                            strObjectName = oAuditDataOld("Description") & " -> " & oAuditDataNew("Description")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTerminal, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet AndAlso Me.Type.ToUpper <> "TIME GATE" AndAlso Me.Type <> "LivePortal" AndAlso Me.Type <> "NFC" AndAlso Me.Type <> "Virtual" AndAlso Me.Type <> "Suprema" AndAlso Me.Enabled Then
                        ' Notificamos al servidor que tiene que lanzar el broadcaster
                        Dim oParamsAux = New roCollection
                        oParamsAux.Add("IDTerminal", Me.ID)
                        roConnector.InitTask(TasksType.BROADCASTER, oParamsAux)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                bolRet = True

                If bolRet AndAlso Me._Readers IsNot Nothing Then
                    For Each oReader As roTerminalReader In Me._Readers
                        bolRet = oReader.Validate(Me)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet AndAlso Me.oSirens IsNot Nothing Then
                    For Each oSiren As roTerminalSiren In Me.oSirens
                        bolRet = oSiren.Validate()
                        If Not bolRet Then Exit For
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminal::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminal::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sSql As String = "@DELETE# FROM TerminalReaderEmployees WHERE IDTerminal = " & Me.ID
                bolRet = ExecuteSql(sSql)

                If bolRet Then
                    sSql = "@DELETE# FROM TerminalReaders WHERE IDTerminal = " & Me.ID
                    bolRet = ExecuteSql(sSql)
                End If

                If bolRet Then
                    sSql = "@DELETE# FROM TerminalSirens WHERE IDTerminal = " & Me.ID
                    bolRet = ExecuteSql(sSql)
                End If

                If bolRet Then
                    sSql = "@DELETE# FROM sysroUserTasks where ID = '" & UserTask.roUserTask.roUserTaskObject & ":\\TERMINALDISCONNECTED" & Me.ID & "'"
                    bolRet = ExecuteSql(sSql)
                End If

                If bolRet Then
                    sSql = "@DELETE# FROM TerminalsSyncTasks where IDTerminal = " & Me.ID
                    bolRet = ExecuteSql(sSql)
                End If

                If bolRet Then
                    sSql = "@UPDATE# Terminals SET Deleted = 1, Enabled = 0, SerialNumber = '' WHERE ID = " & Me.ID
                    bolRet = ExecuteSql(sSql)
                    'Si he borrado el Portal del empleado, borro también el de sistema para fichajes desde la APP
                    If Me.Type.ToUpper = "LIVEPORTAL" Then
                        sSql = "@UPDATE# Terminals SET Deleted = 1, Enabled = 0, SerialNumber = '' WHERE UPPER(TYPE) = 'LIVEPORTAL' AND ISNULL(Other,'') = 'APP'"
                        bolRet = ExecuteSql(sSql)
                    End If
                End If

                If bolRet Then
                    sSql = "@DELETE# FROM sysroUserTasks where ID = '" & UserTask.roUserTask.roUserTaskObject & ":\\TERMINAL_TYPEMISMATCH_" & Me.ID & "' OR ID LIKE '" & UserTask.roUserTask.roUserTaskObject & ":\\%_NOTREGISTERED" & Me.ID & "'"
                    bolRet = ExecuteSql(sSql)
                End If

                If bolRet Then
                    ' Borro carpeta de readings y VTTD
                    Try
                        roTerminal.ResetTerminalSyncData(Me.ID)
                    Catch ex As Exception
                        'do nothing
                    End Try
                End If

                Dim oRepo As New Azure.roAzureTerminalRepository
                oRepo.DeleteTerminalFromCompany(Me.SerialNumber, Azure.RoAzureSupport.GetCompanyName())

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTerminal, Me.Description, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Sub UpdateStatus(ByVal idTerminal As Integer, ByVal Connected As Boolean)

            Try

                Dim sSQL As String = "@UPDATE# Terminals"
                sSQL += " SET LastStatus='" + IIf(Connected, "Ok", "NotConnected") + "'"
                sSQL += " WHERE [ID] = " + idTerminal.ToString

                ExecuteSql(sSQL)
            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:UpdateStatus::", ex)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:UpdateStatus::", ex)
            Finally

            End Try

        End Sub

        Public Shared Sub UpdateLocation(ByVal idTerminal As Integer, ByVal sLocation As String)

            Try

                Dim sSQL As String = "@UPDATE# Terminals"
                sSQL += " SET Location='" + sLocation + "'"
                sSQL += " WHERE [ID] = " + idTerminal

                ExecuteSql(sSQL)
            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:UpdateLocation::", ex)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:UpdateLocation::", ex)
            Finally

            End Try

        End Sub

        ''' <summary>
        ''' Actualiza el estado del terminal
        ''' </summary>
        ''' <param name="Connected">Conectado o no conectado</param>
        ''' <remarks></remarks>
        Public Sub UpdateStatus(ByVal Connected As Boolean, Optional bUpdateLastAction As Boolean = False)

            Try

                Dim sSQL As String = "@UPDATE# Terminals"
                If bUpdateLastAction AndAlso Connected Then
                    sSQL += " SET LastStatus='" + IIf(Connected, "Ok", "NotConnected") + "', LastAction = getdate() "
                Else
                    sSQL += " SET LastStatus='" + IIf(Connected, "Ok", "NotConnected") + "'"
                End If
                sSQL += " WHERE [ID] = " + Me.ID.ToString

                ExecuteSql(sSQL)

                If Connected Then
                    Me.LastStatus = "Ok"
                Else
                    Me.LastStatus = "NotConnected"
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateStatus")
            Finally

            End Try

        End Sub

        Public Sub UpdateFirmVersion(ByVal sVersion As String)

            Try

                Dim sSQL As String = "@UPDATE# Terminals"
                sSQL += " SET FirmVersion='" + sVersion + "'"
                sSQL += " WHERE [ID] = " + Me.ID.ToString

                ExecuteSql(sSQL)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateFirmVersion")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateFirmVersion")
            Finally

            End Try

        End Sub

        Public Sub UpdateModel(ByVal sModel As String)

            Try
                Dim sSQL As String = "@UPDATE# Terminals set Model = '" + sModel + "' WHERE ID = " + Me.ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateModel")
            End Try

        End Sub

        Public Sub UpdatePunchStamp(ByVal punchStamp As String)

            Try
                Dim sSQL As String = $"@UPDATE# Terminals set PunchStamp = '{punchStamp}' WHERE ID = {Me.ID}"
                ExecuteSql(sSQL)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdatePunchStamp")
            End Try

        End Sub

        Public Sub UpdateOther(ByVal sOther As String)

            Try

                Dim sSQL As String = "@UPDATE# Terminals set Other = '" + sOther + "' WHERE ID = " + Me.ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateOther")
            Finally

            End Try

        End Sub

        Public Sub UpdateLocation(ByVal sLocation As String)

            Try

                Dim sSQL As String = "@UPDATE# Terminals"
                sSQL += " SET Location='" + sLocation + "'"
                sSQL += " WHERE [ID] = " + Me.ID.ToString

                ExecuteSql(sSQL)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLocation")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLocation")
            Finally

            End Try

        End Sub

        ''' <summary>
        ''' Actualiza la fecha de la última configuración del terminal
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UpdateLastUpdate(Optional bUpdateLastAction As Boolean = False)

            Try

                If Math.Abs(dLastUpdateLastUpdate.Subtract(Now).TotalMinutes) > 1 Then
                    Dim sSQL As String = "@UPDATE# Terminals SET LastUpdate=getdate() WHERE [ID] = " + Me.ID.ToString
                    If bUpdateLastAction Then
                        sSQL = "@UPDATE# Terminals SET LastUpdate=getdate(), LastAction = getdate() WHERE [ID] = " + Me.ID.ToString
                    End If
                    ExecuteSql(sSQL)
                    dLastUpdateLastUpdate = Now
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLastUpdate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLastUpdate")
            Finally

            End Try

        End Sub

        ''' <summary>
        ''' Actualiza flag para que los clientes actualicen la información
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub SetForceConfig(iID As Integer, _State As roTerminalState)
            Try
                ExecuteSql("@UPDATE# Terminals SET ForceConfig=1 WHERE [ID] = " + iID.ToString)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminal:SetForceConfig")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminal:SetForceConfig")
            End Try
        End Sub

        ''' <summary>
        ''' Resetea flag para que los clientes actualicen la información
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub ClearForceConfig(iID As Integer, _State As roTerminalState)
            Try
                ExecuteSql("@UPDATE# Terminals SET ForceConfig=0 WHERE [ID] = " + iID.ToString)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminal:ClearForceConfig")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminal:ClearForceConfig")
            End Try
        End Sub

        ''' <summary>
        ''' Actualiza la fecha de la última acción del terminal
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UpdateLastAction()
            Try

                If Math.Abs(dLastUpdateLastAction.Subtract(Now).TotalMinutes) > 1 Then
                    ExecuteSql("@UPDATE# Terminals SET LastAction=getdate(), LastStatus = 'Ok' WHERE [ID] = " + Me.ID.ToString)
                    dLastUpdateLastAction = Now
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLastAction")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:UpdateLastAction")
            End Try

        End Sub

        ''' <summary>
        ''' Retorna la cantidad de tareas de configuración pendientes
        ''' </summary>
        ''' <returns>Cantidad de tareas pendientes</returns>
        ''' <remarks></remarks>
        Public Function getTaskPendingCount() As Integer
            Try
                Dim sSQL As String = ""
                sSQL = "@SELECT# isnull(count(*),0) from TerminalsSyncTasks where IDTerminal = " + Me.ID.ToString
                Return ExecuteScalar(sSQL)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminal:getTaskPendingCount")
                Return 0
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:getTaskPendingCount")
                Return 0
            End Try
        End Function

        ''' <summary>
        ''' Retorna el valor del camp
        ''' </summary>
        ''' <param name="FieldName">Nom del camp de la base de dades</param>
        ''' <returns>Valor del camp</returns>
        ''' <remarks></remarks>
        Public Function ReturnField(ByVal FieldName As String) As Object
            Try
                If Not Me.oData.Tables.Contains("Terminals") Then Return Nothing
                If Me.oData.Tables("Terminals").Rows.Count = 0 Then Return Nothing
                If Me.oData.Tables("Terminals").Columns.Contains(FieldName) = False Then Return Nothing

                Return Me.oData.Tables("Terminals").Rows(0).Item(FieldName)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::ReturnField")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Posa el valor a la Taula Terminals
        ''' </summary>
        ''' <param name="FieldName"></param>
        ''' <param name="FieldValue"></param>
        ''' <remarks></remarks>
        Public Sub PutValue(ByVal FieldName As String, ByVal FieldValue As Object)
            Try
                If Not Me.oData.Tables.Contains("Terminals") Then Exit Sub
                If Me.oData.Tables("Terminals").Rows.Count = 0 Then Exit Sub
                If Me.oData.Tables("Terminals").Columns.Contains(FieldName) = False Then Exit Sub

                If FieldValue Is Nothing Then
                    Me.oData.Tables("Terminals").Rows(0).Item(FieldName) = DBNull.Value
                ElseIf FieldValue Is DBNull.Value Then
                    Me.oData.Tables("Terminals").Rows(0).Item(FieldName) = DBNull.Value
                Else
                    Me.oData.Tables("Terminals").Rows(0).Item(FieldName) = FieldValue
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::PutValue")
            End Try
        End Sub

        ''' <summary>
        ''' Carrega la clase roTerminalReader per index
        ''' </summary>
        ''' <param name="_Index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadReaderByIndex(ByVal _Index As Integer) As roTerminalReader
            Try
                'Dim roTR As New roTerminalReader()

                'If Not Me.oData.Tables.Contains("TerminalReaders") Then Return Nothing
                'If Me.oData.Tables("TerminalReaders").Rows.Count - 1 < _Index Then Return Nothing

                'roTR.LoadByRow(Me.oData.Tables("TerminalReaders").Rows(_Index))
                If _Readers.Count > 0 And _Index < _Readers.Count Then
                    Return _Readers.Item(_Index)
                Else
                    Return Nothing
                End If
                'Return roTR
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::LoadReaderByIndex")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Afegeix un Reader
        ''' </summary>
        ''' <param name="oTerminalReader">TerminalReader</param>
        ''' <param name="oActiveConnection">Conexió activa</param>
        ''' <returns>Retorna true si s'ha pogut afegir</returns>
        ''' <remarks></remarks>
        Public Function AddReader(ByRef oTerminalReader As roTerminalReader) As Boolean
            Try
                _Readers.Add(oTerminalReader)
                Return True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::AddReader")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Elimina un Reader
        ''' </summary>
        ''' <param name="oTerminalReader">TerminalReader per Eliminar</param>
        ''' <param name="oActiveConnection">Conexió activa</param>
        ''' <returns>Retorna TRUE si s'ha pogut eliminar</returns>
        ''' <remarks></remarks>
        Public Function RemoveReader(ByRef oTerminalReader As roTerminalReader) As Boolean
            Dim bolRemove As Boolean = False
            Try
                For n As Integer = 0 To _Readers.Count - 1
                    Dim oReader As roTerminalReader = _Readers.Item(n)
                    If oReader.ID = oTerminalReader.ID Then
                        _Readers.RemoveAt(n)
                        bolRemove = True
                        Exit For
                    End If
                Next
                Return bolRemove
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal::RemoveReader")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Comprobació del núm. de serie del Terminal
        ''' </summary>
        ''' <param name="Serial">Núm. de Serie del Terminal</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckTerminalSerialNum(ByVal Serial As String, ByVal SNTerminal As String) As Boolean
            'Recuperem el Valor necesari per calcular el núm. de serie.

            Dim bRet As Boolean = False

            Try
                Dim LicenseService As New roServerLicense
                Dim ProductSerialNum As String = String.Empty
                Dim ClientSN As String = String.Empty
                ProductSerialNum = LicenseService.FeatureData("License", "ProductSerialNum")

                If ProductSerialNum.Split("-").Count > 1 Then
                    ClientSN = ProductSerialNum.Split("-")(1)
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminal:CheckTerminalSerialNum:Unexpected ProductSerialNum found -> " & ProductSerialNum & ". Format should be XXXXX-YYYY")
                    Return False
                End If

                Dim hashStr As String = Robotics.VTBase.HashCheckSum.CalculateString(ClientSN & SNTerminal, Algorithm.MD5)
                Dim resolverSN As String = hashStr.Chars(0) & hashStr.Chars(4) & hashStr.Chars(8) & hashStr.Chars(12) & hashStr.Chars(16) & hashStr.Chars(20) &
                                                      hashStr.Chars(24) & hashStr.Chars(28) & hashStr.Chars(29) & hashStr.Chars(30) & hashStr.Chars(31)

                bRet = (resolverSN = Serial)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:CheckTerminalSerialNum")
                bRet = False
            End Try

            Return bRet
        End Function

        Public Function GetTerminalRegistrationCode(strSN As String) As String
            Try
                Dim LicenseService As New roServerLicense
                Dim ClientSN As String = LicenseService.FeatureData("License", "ProductSerialNum").Split("-")(1)

                Dim hashStr As String = Robotics.VTBase.HashCheckSum.CalculateString(ClientSN & strSN, Algorithm.MD5)
                Dim resolverSN As String = hashStr.Chars(0) & hashStr.Chars(4) & hashStr.Chars(8) & hashStr.Chars(12) & hashStr.Chars(16) & hashStr.Chars(20) &
                                                      hashStr.Chars(24) & hashStr.Chars(28) & hashStr.Chars(29) & hashStr.Chars(30) & hashStr.Chars(31)
                Return resolverSN
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:GetTerminalRegistrationCode")
                Return String.Empty
            End Try
        End Function

        ''' <summary>
        ''' Comprobació del núm. de serie del Terminal
        ''' </summary>
        ''' <param name="Serial">Núm. de Serie del Terminal</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTerminalSecurityToken(ByVal authToken As String) As String
            'Recuperem el Valor necesari per calcular el núm. de serie.
            Try
                Dim LicenseService As New roServerLicense
                Dim ClientSN As String = LicenseService.FeatureData("License", "ProductSerialNum").Split("-")(1)

                Dim hashStr As String = Robotics.VTBase.HashCheckSum.CalculateString(ClientSN & "#" & Me.ID & "#" & authToken, Algorithm.MD5)
                Dim resolverSN As String = hashStr.Chars(0) & hashStr.Chars(4) & hashStr.Chars(8) & hashStr.Chars(12) & hashStr.Chars(16) & hashStr.Chars(20) &
                                                      hashStr.Chars(24) & hashStr.Chars(28) & hashStr.Chars(29) & hashStr.Chars(30) & hashStr.Chars(31)

                Return resolverSN
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminal:CheckTerminalSerialNum")
                Return String.Empty
            End Try
        End Function

        Private Function GetConfiguration() As DataTable

            Dim tbRet As New DataTable("Configuration")
            tbRet.Columns.Add(New DataColumn("Name", GetType(String)))
            tbRet.Columns.Add(New DataColumn("Type", GetType(Integer)))
            tbRet.Columns.Add(New DataColumn("Value", GetType(String)))

            Dim oConfiguration As New roCollection(Me.ConfData())
            If oConfiguration.Count > 0 Then

                Dim n As Integer = 1
                Dim oNode As Object = Nothing
                Try
                    oNode = oConfiguration.Item(n, roCollection.roSearchMode.roByIndex)
                Catch ex As Exception
                    oNode = Nothing
                End Try
                Dim oRow As DataRow = Nothing
                While oNode IsNot Nothing

                    oRow = tbRet.NewRow

                    oRow("Name") = oConfiguration.Key(n)
                    oRow("Type") = VarType(oNode)
                    oRow("Value") = Any2String(oNode)

                    tbRet.Rows.Add(oRow)

                    n += 1
                    Try
                        If n <= oConfiguration.Count Then
                            oNode = oConfiguration.Item(n, roCollection.roSearchMode.roByIndex)
                        Else
                            oNode = Nothing
                        End If
                    Catch ex As Exception
                        oNode = Nothing
                    End Try

                End While

            End If

            ' '' Definimos las columnas de la tabla
            ''Dim n As Integer = 1
            ''Dim oNode As Object = oConfiguration.Item(n, roCollection.roSearchMode.roByIndex)
            ''While oNode IsNot Nothing

            ''    Dim oColumnType As System.Type = GetType(String)

            ''    Select Case VarType(oNode)
            ''        Case vbInteger, vbLong, vbSingle, vbDouble, vbCurrency, vbDate, vbString, vbVariant, vbBoolean, vbDecimal, vbByte
            ''            oColumnType = GetType(Integer)
            ''        Case vbLong
            ''            oColumnType = GetType(Long)
            ''        Case vbDouble
            ''            oColumnType = GetType(Double)
            ''        Case vbCurrency
            ''            oColumnType = GetType(Double)
            ''        Case vbString
            ''            oColumnType = GetType(String)
            ''        Case vbBoolean
            ''            oColumnType = GetType(Boolean)
            ''        Case vbDecimal
            ''            oColumnType = GetType(Decimal)
            ''        Case vbByte
            ''            oColumnType = GetType(Byte)
            ''        Case vbDate
            ''            oColumnType = GetType(Date)

            ''        Case vbObject
            ''            '
            ''            'Err.Raise(9083, "roCollection", "Unsupported type (" & VarType(Node.Item(Index, roSearchMode.roByIndex)) & ") for XML export.")
            ''            '

            ''    End Select

            ''    tbRet.Columns.Add(New DataColumn(oConfiguration.NodeKey(n), oColumnType))

            ''    n += 1
            ''    oNode = oConfiguration.Item(n, roCollection.roSearchMode.roByIndex)
            ''End While

            ' '' Llenamos la tabla con
            ''Dim oRow As DataRow = tbRet.NewRow

            ''For Each oColumn As DataColumn In tbRet.Columns
            ''    oRow(oColumn.ColumnName) = oConfiguration.Item(oColumn.ColumnName, roCollection.roSearchMode.roByKey)
            ''Next

            Return tbRet

        End Function

        Public Sub SetConfiguration(ByVal tbConfiguration As DataTable)

            Dim oConfiguration As New roCollection

            For Each oRow As DataRow In tbConfiguration.Rows
                If Not IsDBNull(oRow("Name")) And Not IsDBNull(oRow("Type")) Then
                    Dim oValue As Object = Nothing
                    If Not IsDBNull(oRow("Value")) Then oValue = oRow("Value")
                    Select Case CType(oRow("Type"), VariantType)
                        Case vbInteger
                            oConfiguration.Add(oRow("Name"), CType(oValue, Integer))
                        Case vbLong
                            oConfiguration.Add(oRow("Name"), CType(oValue, Integer))
                        Case vbSingle
                            oConfiguration.Add(oRow("Name"), CType(oValue, Single))
                        Case vbDouble
                            oConfiguration.Add(oRow("Name"), CType(oValue, Double))
                        Case vbCurrency
                            oConfiguration.Add(oRow("Name"), CType(oValue, Decimal))
                        Case vbDate
                            oConfiguration.Add(oRow("Name"), CType(oValue, Date))
                        Case vbVariant
                            oConfiguration.Add(oRow("Name"), CType(oValue, Object))
                        Case vbBoolean
                            oConfiguration.Add(oRow("Name"), CType(oValue, Boolean))
                        Case vbDecimal
                            oConfiguration.Add(oRow("Name"), CType(oValue, Decimal))
                        Case vbByte
                            oConfiguration.Add(oRow("Name"), CType(oValue, Byte))
                        Case vbString
                            oConfiguration.Add(oRow("Name"), CType(oValue, String))
                        Case Else
                            oConfiguration.Add(oRow("Name"), Nothing)
                    End Select

                End If
            Next

            Me.ConfData = oConfiguration.XML()

        End Sub

        ''' <summary>
        ''' Devuelve la fecha/hora del servidor aplicando la diferencia horaria que tenga configurada el terminal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCurrentDateTime() As DateTime

            Dim xRet As DateTime

            ' Miramos si el terminal tiene una zona horaria distinta a la del servidor
            If Me.IsDifferentZoneTime Then

                Try

                    xRet = Now

                    ' Obtenemos información de la zona horaria del terminal
                    Dim oTerminalTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Me.TimeZoneName)

                    ' Combertimos la fecha actual del servidor a la zona horaria del terminal
                    xRet = TimeZoneInfo.ConvertTime(xRet, TimeZoneInfo.Local, oTerminalTimeZone)

                    If oTerminalTimeZone.SupportsDaylightSavingTime Then ' Si la zona horaria tiene horario de verano

                        ' Si el terminal no tiene que cambiar automáticamente la hora según el horario de verano y
                        'estamos en zona de verano en la zona horaria del terminal
                        If Not Me.AutoDaylight AndAlso oTerminalTimeZone.IsDaylightSavingTime(xRet) Then
                            ' Restamos la diferencia del cambio de verano a la fecha
                            For Each oRule As TimeZoneInfo.AdjustmentRule In oTerminalTimeZone.GetAdjustmentRules()
                                If xRet >= oRule.DateStart And xRet <= oRule.DateEnd Then
                                    xRet = xRet.AddMinutes(oRule.DaylightDelta.TotalMinutes)
                                End If
                            Next
                        End If

                    End If
                Catch ex As TimeZoneNotFoundException
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:GetCurrentDateTime: TimeZone not found '" & Me.TimeZoneName & "'", ex)
                Catch ex As Exception

                End Try
            Else
                xRet = Now
            End If

            Return xRet

        End Function

        ''' <summary>
        ''' Devuelve la fecha/hora del servidor aplicando la diferencia horaria que tenga configurada el terminal
        ''' </summary>
        ''' <param name="bIsInDST">Ahora la zona horaria del terminal se encuentra en horario de verano</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCurrentDateTimeEx(ByRef bIsInDST As Boolean) As DateTime

            Dim xRet As DateTime

            ' Miramos si el terminal tiene una zona horaria distinta a la del servidor
            If Me.IsDifferentZoneTime Then

                Try

                    xRet = Now

                    ' Obtenemos información de la zona horaria del terminal
                    Dim oTerminalTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Me.TimeZoneName)

                    ' Combertimos la fecha actual del servidor a la zona horaria del terminal
                    xRet = TimeZoneInfo.ConvertTime(xRet, TimeZoneInfo.Local, oTerminalTimeZone)
                    ' Devuelvo si el terminal está en DST
                    bIsInDST = oTerminalTimeZone.IsDaylightSavingTime(xRet)

                    If oTerminalTimeZone.SupportsDaylightSavingTime Then ' Si la zona horaria tiene horario de verano

                        ' Si el terminal no tiene que cambiar automáticamente la hora según el horario de verano y
                        'estamos en zona de verano en la zona horaria del terminal
                        If Not Me.AutoDaylight AndAlso oTerminalTimeZone.IsDaylightSavingTime(xRet) Then
                            ' Restamos la diferencia del cambio de verano a la fecha
                            For Each oRule As TimeZoneInfo.AdjustmentRule In oTerminalTimeZone.GetAdjustmentRules()
                                If xRet >= oRule.DateStart And xRet <= oRule.DateEnd Then
                                    xRet = xRet.AddMinutes(oRule.DaylightDelta.TotalMinutes)
                                End If
                            Next
                        End If

                    End If
                Catch ex As TimeZoneNotFoundException
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminal:GetCurrentDateTime: TimeZone not found '" & Me.TimeZoneName & "'", ex)
                Catch ex As Exception

                End Try
            Else
                xRet = Now
                bIsInDST = TimeZoneInfo.Local.IsDaylightSavingTime(xRet)
            End If

            Return xRet

        End Function

        Public Overrides Function ToString() As String
            Dim oRet As String = String.Empty
            Try
                oRet = "Terminal " & Me.ID & ":" & Me.Type & ":" & Me.Location
            Catch ex As Exception
                oRet = "roTerminal::Error recovering terminal information for logs"
            End Try
            Return oRet
        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roTerminalState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

#End Region

#Region "Helper Methods"

        ''' <summary>
        ''' Recupera de la tabla sysroTerminalReaderTemplates los tipos posibles de terminales
        ''' </summary>
        ''' <param name="_TypeDirection">'Local' o 'Remote'. Dejar en Blanco para recuperar todos los tipos</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>DataTable con el campo Type y Direction</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTerminalTypes(ByVal _TypeDirection As String, ByVal _State As roTerminalState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# DISTINCT(TYPE), DIRECTION FROM sysroReaderTemplates"
                If _TypeDirection <> "" Then strSQL &= " WHERE Direction = @TypeDirection"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                If _TypeDirection <> "" Then
                    AddParameter(cmd, "@TypeDirection", DbType.String, 50)
                    cmd.Parameters("@TypeDirection").Value = _TypeDirection
                End If

                oRet = New DataTable("sysroReaderTemplates")

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.Fill(oRet)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminal::GetTerminalTypes")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminal::GetTerminalTypes")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de terminales de un tipo por los que puede fichar un empleado. <br></br>
        ''' Tiene en cuenta la configuración de limitación de empleados.<br></br>
        ''' No tiene en cuenta la configuración de accesos.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_TerminalType">Tipo de terminal</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>Lista de terminales por los que puede fichar</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeeTerminals(ByVal _IDEmployee As Integer, ByVal _TerminalType As String, ByVal _State As roTerminalState) As Generic.List(Of roTerminal)

            Dim oRet As New Generic.List(Of roTerminal)

            Try
                Dim strSQL As String =
                $"@SELECT# * FROM Terminals 
                  WHERE type = @TerminalType AND ISNULL(Other,'') <> 'APP' AND 
                  (Deleted = 0 OR Deleted IS NULL) AND
                  (ISNULL((@SELECT# COUNT(*) FROM TerminalReaderEmployees TRE1 WHERE TRE1.IDTerminal = Terminals.ID),0) = 0 OR 
                   ISNULL((@SELECT# COUNT(*) FROM TerminalReaderEmployees TRE2 WHERE TRE2.IDTerminal = Terminals.ID AND TRE2.IDEmployee = {_IDEmployee}),0) = 1)"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@TerminalType", DbType.String, 50)
                cmd.Parameters("@TerminalType").Value = _TerminalType

                Dim tbTerminals As New DataTable("Terminals")

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbTerminals)

                For Each oRow As DataRow In tbTerminals.Rows
                    oRet.Add(New roTerminal(oRow, _State, False))
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminal::GetEmployeeTerminals")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminal::GetEmployeeTerminals")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTerminalName(ByVal IDTerminal As Integer, ByVal oState As roTerminalState) As String

            Dim strRet As String = String.Empty

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Description FROM Terminals WHERE ID = " & IDTerminal
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Description")) Then strRet = tb.Rows(0)("Description")
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminal::GetEmployeeTerminals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminal::GetEmployeeTerminals")
            Finally

            End Try

            Return strRet

        End Function

        Public Shared Function GetTerminalReaderZoneByID(ByVal IDTerminal As Integer, ByVal IDReader As Integer, bAttOutZone As Boolean, Optional ByVal oState As roTerminalState = Nothing) As Integer

            Dim intRet As Integer = -1

            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# IDZone, IDZoneOut, Mode, InteractionAction FROM TerminalReaders WHERE ID= " & IDReader.ToString & " AND IDTerminal = " & IDTerminal.ToString &
                                                          "", )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    If Any2String(tb.Rows(0)("Mode")).IndexOf("ACCTA") > -1 AndAlso Any2String(tb.Rows(0)("InteractionAction")) = "ES" Then
                        ' Fichaje de Control Horario en termina con comportamiento de Accesos Integrados con Presencia en 2 sentidos. La zona de presencia la marca la zona IDZoneOut
                        intRet = Any2Integer(tb.Rows(0)("IDZoneOut"))
                        Return intRet
                    End If

                    If Not bAttOutZone Then
                        intRet = Any2Integer(tb.Rows(0)("IDZone"))
                    Else
                        ' Devuelvo la zona asiganada a las salidas de presencia
                        intRet = Any2Integer(tb.Rows(0)("IDZoneOut"))
                    End If
                End If
            Catch ex As DbException
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderZoneByID")
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderZoneByID")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetTerminalReaderCount(ByVal IDTerminal As Integer, ByRef FirstReaderID As Integer, Optional ByVal oState As roTerminalState = Nothing) As Integer

            Dim intRet As Integer = 0
            FirstReaderID = -1

            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# ID FROM TerminalReaders WHERE IDTERMINAL= " & IDTerminal.ToString & " ORDER BY ID ASC", )
                If tb IsNot Nothing Then
                    intRet = tb.Rows.Count
                    FirstReaderID = Any2Integer(tb.Rows(0)("ID"))
                End If
            Catch ex As DbException
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderCount")
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderCount")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetTerminalReaderMode(ByVal IDTerminal As Integer, ByVal IDReader As Integer, Optional ByVal oState As roTerminalState = Nothing) As String

            Dim strRet As String = ""

            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# Mode FROM TerminalReaders WHERE ID= " & IDReader.ToString & " AND IDTerminal = " & IDTerminal.ToString &
                                                          "", )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    strRet = Any2String(tb.Rows(0)("Mode"))
                End If
            Catch ex As DbException
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderMode")
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalReaderMode")
            Finally

            End Try

            Return strRet

        End Function

        Public Shared Function GetTerminalType(iIDTerminal As Integer, Optional ByVal oState As roTerminalState = Nothing) As String
            Dim oRet As String = String.Empty

            Dim sSQL As String = String.Empty
            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# type from terminals where id=" & iIDTerminal.ToString, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    oRet = Any2String(tb.Rows(0)("type"))
                End If
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalType")
            Finally

            End Try
            Return oRet
        End Function

        Public Shared Function GetTerminalTimeZone(iIDTerminal As Integer, Optional ByVal oState As roTerminalState = Nothing) As String
            Dim oRet As String = String.Empty

            Dim sSQL As String = String.Empty
            Try

                sSQL = "@SELECT# TimeZoneName from terminals where id=" & iIDTerminal.ToString
                oRet = roTypes.Any2String(ExecuteScalar(sSQL))
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::GetTerminalTimeZone")
            Finally

            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Se borran tablas de sincronización por si el terminal las usa
        ''' </summary>
        ''' <param name="iIDTerminal"></param>
        ''' <param name="oCn"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        Public Shared Function ResetTerminalSyncData(iIDTerminal As Integer, Optional ByVal oState As roTerminalState = Nothing) As Boolean
            Dim bolRet As Boolean = True
            Dim sSQL As String = String.Empty

            Try
                Dim oSqlArray As New Generic.List(Of String)
                Dim strSQL As String

                oSqlArray.Add("TerminalsSyncBiometricData")
                oSqlArray.Add("TerminalsSyncCardsData")
                oSqlArray.Add("TerminalsSyncDocumentsData")
                oSqlArray.Add("TerminalsSyncEmployeesData")
                oSqlArray.Add("TerminalsSyncGroupsData")
                oSqlArray.Add("TerminalsSyncConfigData")
                oSqlArray.Add("TerminalsSyncSirensData")
                oSqlArray.Add("TerminalsSyncAccessData")
                oSqlArray.Add("TerminalsSyncTimeZonesData")
                oSqlArray.Add("TerminalsSyncCausesData")
                oSqlArray.Add("TerminalsSyncPushTimeZonesData")
                oSqlArray.Add("TerminalsSyncPushEmployeeTimeZonesData")
                oSqlArray.Add("@DELETE# TerminalsSyncTasks WHERE IDTerminal = " & iIDTerminal)

                Dim bResult As Boolean = False
                For Each strSQL In oSqlArray

                    If Not strSQL.ToUpper.Contains("DELETE") Then
                        strSQL = "@DELETE# " & strSQL & " Where TerminalId = " & iIDTerminal
                    End If

                    bResult = ExecuteSqlWithoutTimeOut(strSQL)

                    bolRet = bolRet AndAlso bResult
                Next
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roTerminal::ResetTerminalSyncData")
            End Try
            Return bolRet
        End Function

#End Region

        ''' <summary>
        ''' Clase TerminalReader
        ''' </summary>
        ''' <remarks></remarks>
        <DataContract>
        Public Class roTerminalReader

#Region "Declarations - Constructors"

            Private oState As roTerminalState

            Private intIDTerminal As Integer
            Private intID As Integer
            Private strDescription As String

            Private strMode As String = String.Empty
            Private oScopeMode As ScopeMode
            Private bolOHP As Boolean ' Prevención de riesgos laborales

            Private bolUseDispKey As Boolean
            Private oInteractionMode As InteractionMode
            Private oInteractionAction As Nullable(Of InteractionAction)
            Private oInteractiveConfig As roInteractiveConfig
            Private oValidationMode As ValidationMode

            ' Límite empleados
            Private lstEmployeesLimit As New Generic.List(Of Integer)

            Private intIDZone As Nullable(Of Integer)
            Private dblPictureX As Nullable(Of Double)
            Private dblPictureY As Nullable(Of Double)

            ' Configuración relés
            Private intOutput As Nullable(Of Integer)
            Private intDuration As Integer
            Private intInvalidOutput As Nullable(Of Integer)
            Private intInvalidDuration As Integer
            Private lstCustomButtons As New Generic.List(Of roCustomButton)

            Private intRequestPin As Integer
            Private intAccessKey As Integer
            Private strType As String

            Private intIDActivity As Nullable(Of Integer)

            Private intIDCamera As Nullable(Of Integer)

            Private dLastEmployeeListOnMemoryUpdate As DateTime = New DateTime(1970, 1, 1)
            Private dLastPersistedEmployeeListUpdate As DateTime = New DateTime(1970, 1, 1)
            Private dLastPersistedEmployeeListCheck As DateTime = New DateTime(1970, 1, 1)
            Private lEmployeesUnautorizedOHP As List(Of Integer) = New List(Of Integer)
            Private lEmployeePermitList As List(Of Integer) = New List(Of Integer)
            Private lEmployeesUnautorizedField As List(Of Integer) = New List(Of Integer)
            Private lEmployeePermitByGroupList As List(Of Integer) = New List(Of Integer)
            Private lEmployeeAccessGroupAndContract As List(Of Integer) = New List(Of Integer)
            Private bolEmployeePermitByGroupActive As Boolean = False
            Private bolModeRestrictively As Boolean = False
            Private sTerminalPath As String = ""
            Private bEmployeePermitedListEmployeeMode As Boolean = False

            Private intIDZoneOut As Nullable(Of Integer)
            Private dblPictureXOut As Nullable(Of Double)
            Private dblPictureYOut As Nullable(Of Double)
            Private intIDCameraOut As Nullable(Of Integer)

            Private intIDCostCener As Nullable(Of Integer)

            Private bLegacyRestrictionModeAllowed As Boolean = False
            Private bAccessLicenseInstalled As Boolean = False
            Private bCheckAccessAuthorization As Boolean = False

            Private sNfcTag As String = String.Empty

            ''' <summary>
            ''' Genera un nou Reader en blanc
            ''' </summary>
            ''' <remarks></remarks>
            Public Sub New()
                Me.New(-1, New roTerminalState)
            End Sub

            ''' <summary>
            ''' Carreguem el roTerminal desde un TableRow
            ''' </summary>
            ''' <param name="Row">Row amb els camps de roTerminal</param>
            ''' <param name="_State">Control d'errors</param>
            ''' <remarks></remarks>
            Public Sub New(ByVal Row As DataRow, ByVal RowsEmployees() As DataRow, ByVal _State As roTerminalState, Optional bLegacyRestrictionModeAllowed As Boolean = False)
                oState = _State
                Me.LoadByRow(Row, RowsEmployees, bLegacyRestrictionModeAllowed)
            End Sub

            ''' <summary>
            ''' Genera un nou Reader amb Id de terminal Asignat, i busca el primer reader o genera un nou (ReaderID = 0)
            ''' </summary>
            ''' <param name="_IDTerminal">Terminal al que pertany el lector</param>
            ''' <param name="_State">Control d'errors</param>
            ''' <remarks></remarks>
            Public Sub New(ByVal _IDTerminal As Integer, ByVal _State As roTerminalState, Optional ByVal bAudit As Boolean = False)
                Me.New(_IDTerminal, -1, _State, bAudit)
            End Sub

            ''' <summary>
            ''' Genera un nou Reader amb el terminal especificat i ID Reader, si no existeix el crea
            ''' </summary>
            ''' <param name="_IDTerminal"></param>
            ''' <param name="_IDReader"></param>
            ''' <param name="_State"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _State As roTerminalState, Optional ByVal bAudit As Boolean = False)
                oState = _State
                If _IDTerminal <> -1 Then
                    Me.intIDTerminal = _IDTerminal
                    If _IDReader <> -1 Then
                        Me.ID = _IDReader
                    End If
                    Load(bAudit)
                End If
            End Sub

#End Region

#Region "Properties"

            <IgnoreDataMember>
            Public Property State() As roTerminalState
                Get
                    Return Me.oState
                End Get
                Set(ByVal value As roTerminalState)
                    Me.oState = value
                End Set
            End Property

            <DataMember>
            Public Property IDTerminal() As Integer
                Get
                    Return Me.intIDTerminal
                End Get
                Set(ByVal value As Integer)
                    Me.intIDTerminal = value
                End Set
            End Property

            <DataMember>
            Public Property ID() As Integer
                Get
                    Return Any2Integer(intID)
                End Get
                Set(ByVal value As Integer)
                    intID = value
                End Set
            End Property

            <DataMember>
            Public Property Description() As String
                Get
                    Return Any2String(strDescription)
                End Get
                Set(ByVal value As String)
                    strDescription = value
                End Set
            End Property

            <IgnoreDataMember>
            Public Property Mode() As String
                Get
                    Return Any2String(Me.strMode)
                End Get
                Set(ByVal value As String)
                    Me.strMode = value.ToUpper
                    If Me.strMode = "" Then
                        Me.oScopeMode = ScopeMode.UNDEFINED
                    Else
                        Try
                            Me.oScopeMode = System.Enum.Parse(Me.oScopeMode.GetType, Me.strMode.Replace("+", ""), True)
                        Catch ex As Exception
                            Me.oScopeMode = ScopeMode.UNDEFINED
                        End Try
                    End If
                End Set
            End Property

            <DataMember>
            Public Property ScopeMode() As ScopeMode
                Get
                    Return Me.oScopeMode
                End Get
                Set(ByVal value As ScopeMode)
                    Me.oScopeMode = value
                    If Me.oScopeMode = ScopeMode.TAJOB Then
                        Me.strMode = "TA+JOB"
                    ElseIf Me.oScopeMode = ScopeMode.UNDEFINED Then
                        Me.strMode = ""
                    Else
                        Me.strMode = System.Enum.GetName(Me.oScopeMode.GetType, Me.oScopeMode)
                    End If
                End Set
            End Property

            <IgnoreDataMember>
            Public ReadOnly Property HasAccess() As Boolean
                Get
                    Select Case Me.oScopeMode
                        Case ScopeMode.ACC, ScopeMode.ACCEIP, ScopeMode.ACCTA, ScopeMode.ACCTAEIP, ScopeMode.TAACC
                            Return True
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            <IgnoreDataMember>
            Public ReadOnly Property HasJob() As Boolean
                Get
                    Select Case Me.oScopeMode
                        Case ScopeMode.JOB, ScopeMode.TAJOB, ScopeMode.TAJOBEIP
                            Return True
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            <IgnoreDataMember>
            Public ReadOnly Property HasTask() As Boolean
                Get
                    Select Case Me.oScopeMode
                        Case ScopeMode.TSK, ScopeMode.TATSK, ScopeMode.TATSKEIP
                            Return True
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            <DataMember>
            Public Property OHP() As Boolean
                Get
                    Return Me.bolOHP
                End Get
                Set(ByVal value As Boolean)
                    Me.bolOHP = value
                End Set
            End Property

            <DataMember>
            Public Property UseDispKey() As Boolean
                Get
                    Return Me.bolUseDispKey
                End Get
                Set(ByVal value As Boolean)
                    Me.bolUseDispKey = value
                End Set
            End Property

            <DataMember>
            Public Property InteractionMode() As InteractionMode
                Get
                    Return Me.oInteractionMode
                End Get
                Set(ByVal value As InteractionMode)
                    Me.oInteractionMode = value
                End Set
            End Property

            <DataMember>
            Public Property InteractionAction() As Nullable(Of InteractionAction)
                Get
                    Return Me.oInteractionAction
                End Get
                Set(ByVal value As Nullable(Of InteractionAction))
                    Me.oInteractionAction = value
                End Set
            End Property

            <DataMember>
            Public Property InteractiveConfig() As roInteractiveConfig
                Get
                    Return Me.oInteractiveConfig
                End Get
                Set(ByVal value As roInteractiveConfig)
                    Me.oInteractiveConfig = value
                End Set
            End Property

            <DataMember>
            Public Property ValidationMode() As ValidationMode
                Get
                    Return Me.oValidationMode
                End Get
                Set(ByVal value As ValidationMode)
                    Me.oValidationMode = value
                End Set
            End Property

            <DataMember>
            Public Property IDZone() As Nullable(Of Integer)
                Get
                    Return intIDZone
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intIDZone = value
                End Set
            End Property

            <DataMember>
            Public Property PictureX() As Nullable(Of Double)
                Get
                    Return dblPictureX
                End Get
                Set(ByVal value As Nullable(Of Double))
                    dblPictureX = value
                End Set
            End Property

            <DataMember>
            Public Property PictureY() As Nullable(Of Double)
                Get
                    Return dblPictureY
                End Get
                Set(ByVal value As Nullable(Of Double))
                    dblPictureY = value
                End Set
            End Property

            <DataMember>
            Public Property EmployeesLimit() As Generic.List(Of Integer)
                Get
                    Return Me.lstEmployeesLimit
                End Get
                Set(ByVal value As Generic.List(Of Integer))
                    Me.lstEmployeesLimit = value
                End Set
            End Property

            <DataMember>
            Public Property Output() As Nullable(Of Integer)
                Get
                    Return Any2Integer(intOutput)
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intOutput = value
                End Set
            End Property

            <DataMember>
            Public Property Duration() As Integer
                Get
                    Return Any2Integer(intDuration)
                End Get
                Set(ByVal value As Integer)
                    intDuration = value
                End Set
            End Property

            <DataMember>
            Public Property InvalidOutput() As Nullable(Of Integer)
                Get
                    Return Any2Integer(Me.intInvalidOutput)
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    Me.intInvalidOutput = value
                End Set
            End Property

            <DataMember>
            Public Property InvalidDuration() As Integer
                Get
                    Return Any2Integer(Me.intInvalidDuration)
                End Get
                Set(ByVal value As Integer)
                    Me.intInvalidDuration = value
                End Set
            End Property

            <DataMember>
            Public Property CustomButtons() As Generic.List(Of roCustomButton)
                Get
                    Return Me.lstCustomButtons
                End Get
                Set(ByVal value As Generic.List(Of roCustomButton))
                    Me.lstCustomButtons = value
                End Set
            End Property

            <DataMember>
            Public Property RequestPin() As Integer
                Get
                    Return Any2Integer(intRequestPin)
                End Get
                Set(ByVal value As Integer)
                    intRequestPin = value
                End Set
            End Property

            <DataMember>
            Public Property AccessKey() As Integer
                Get
                    Return Any2Integer(intAccessKey)
                End Get
                Set(ByVal value As Integer)
                    intAccessKey = value
                End Set
            End Property

            <DataMember>
            Public Property Type() As String
                Get
                    Return Any2String(strType)
                End Get
                Set(ByVal value As String)
                    strType = value
                End Set
            End Property

            <DataMember>
            Public Property IDActivity() As Nullable(Of Integer)
                Get
                    Return Me.intIDActivity
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    Me.intIDActivity = value
                End Set
            End Property

            <DataMember>
            Public Property IDCamera() As Nullable(Of Integer)
                Get
                    Return intIDCamera
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intIDCamera = value
                End Set
            End Property

            <DataMember>
            Public Property IDZoneOut() As Nullable(Of Integer)
                Get
                    Return intIDZoneOut
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intIDZoneOut = value
                End Set
            End Property

            <DataMember>
            Public Property PictureXOut() As Nullable(Of Double)
                Get
                    Return dblPictureXOut
                End Get
                Set(ByVal value As Nullable(Of Double))
                    dblPictureXOut = value
                End Set
            End Property

            <DataMember>
            Public Property PictureYOut() As Nullable(Of Double)
                Get
                    Return dblPictureYOut
                End Get
                Set(ByVal value As Nullable(Of Double))
                    dblPictureYOut = value
                End Set
            End Property

            <DataMember>
            Public Property IDCameraOut() As Nullable(Of Integer)
                Get
                    Return intIDCameraOut
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intIDCameraOut = value
                End Set
            End Property

            <DataMember>
            Public Property IDCostCenter() As Nullable(Of Integer)
                Get
                    Return intIDCostCener
                End Get
                Set(ByVal value As Nullable(Of Integer))
                    intIDCostCener = value
                End Set
            End Property

            <DataMember>
            Public Property LegacyRestrictionModeAllowed As Boolean
                Get
                    Return bLegacyRestrictionModeAllowed
                End Get
                Set(value As Boolean)

                End Set
            End Property

            <IgnoreDataMember>
            Public ReadOnly Property CheckAccessAuthorizationOnNoAccessReaders As Boolean
                Get
                    Dim oRet As Boolean = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "CheckAccessAuthorizationOnNoAccessReaders"))
                    Return (oRet AndAlso Not LegacyRestrictionModeAllowed AndAlso bAccessLicenseInstalled)
                End Get
            End Property

            <DataMember>
            Public Property CheckAPB As Boolean

            <DataMember>
            Public Property APBControlledZones As String

            <DataMember>
            Public Property APBControlledMinutes As Integer

            <DataMember>
            Public Property APBCourtesyTime As Integer

            <DataMember>
            Public Property NfcTagValue() As String
                Get
                    Return Any2String(sNfcTag)
                End Get
                Set(ByVal value As String)
                    sNfcTag = value
                End Set
            End Property

#End Region

#Region "Methods"

            ''' <summary>
            ''' Carrega la clase segons dades pasades al New
            ''' </summary>
            ''' <remarks></remarks>
            Private Function Load(Optional ByVal bolAudit As Boolean = True) As Boolean
                Dim bolRet As Boolean = False

                Try
                    Try
                        'Comprobamos si estamos en VTLive o VTPro
                        Dim oLicense As New roServerLicense
                        bAccessLicenseInstalled = oLicense.FeatureIsInstalled("Forms\Access")
                    Catch ex As Exception
                    End Try

                    Try
                        'Obtenemos el path del terminal
                        ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                        Dim oSettings As New roSettings
                        Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                        If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                            _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                        End If
                        oSettings = New roSettings(_RegistryRoot & "Robotics\VisualTime")

                        sTerminalPath = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & Me.IDTerminal.ToString
                    Catch ex As Exception
                    End Try

                    Dim strSQL As String = ""
                    Dim tbReaders As DataTable = Nothing
                    Dim tbReadersEmployees As DataTable = Nothing
                    If Me.IDTerminal <> -1 And Me.ID <> -1 Then
                        strSQL = "@SELECT# * FROM TerminalReaders " &
                                               "WHERE IDTerminal = " & Me.IDTerminal & " And ID = " & Me.ID
                        tbReaders = CreateDataTable(strSQL, "TerminalReaders")
                    ElseIf Me.IDTerminal <> -1 And Me.ID = -1 Then
                        strSQL = "@SELECT# * FROM TerminalReaders " &
                                               "WHERE IDTerminal = " & Me.IDTerminal
                        tbReaders = CreateDataTable(strSQL, "TerminalReaders")
                    End If

                    If tbReaders IsNot Nothing Then
                        strSQL = "@SELECT# * FROM TerminalReaderEmployees " &
                                               "WHERE IDTerminal = " & Me.IDTerminal
                        tbReadersEmployees = CreateDataTable(strSQL, "TerminalReaderEmployees")

                        If tbReaders.Rows.Count > 0 Then
                            Me.LoadByRow(tbReaders.Rows(0), tbReadersEmployees.Select("IDReader = " & tbReaders.Rows(0).Item("ID")))
                        End If
                    Else
                        ' Auditar lectura (si recuperamos terminalreader)
                        If bolAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{Description}", Me.Description, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTerminalReader, Me.Description, tbParameters, -1)
                        End If
                    End If

                    ' Miro debe estar habilitado el modo de restricción de presencia antiguo (descripción de grupos y/o lista de empleados a nivel de lector)
                    Try
                        strSQL = "@SELECT# SUM(total) total from (@SELECT# COUNT(*) total from TerminalReaderEmployees UNION @SELECT# COUNT(*) total from Groups where DescriptionGroup like '%TERMINAL=%') aux"
                        Dim tbControl As DataTable = Nothing
                        tbControl = CreateDataTable(strSQL, "Control")
                        If Not tbControl Is Nothing AndAlso tbControl.Rows.Count > 0 Then
                            bLegacyRestrictionModeAllowed = Any2Boolean(tbControl.Rows(0).Item("total"))
                        End If
                    Catch ex As Exception
                    End Try

                    bolRet = True
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:Load")
                Finally

                End Try

                Return bolRet

            End Function

            ''' <summary>
            ''' Carrega la clase via DataRow
            ''' </summary>
            ''' <param name="dRow">DataRow per omplir la clase (Deu tenir definits tots els camps)</param>
            ''' <remarks></remarks>
            Private Sub LoadByRow(ByVal dRow As DataRow, ByVal dRowsEmployees() As DataRow, Optional _LegacyRestrictionModeAllowed As Boolean = False)
                Try

                    Me.IDTerminal = Any2Integer(dRow("IDTerminal"))
                    Me.ID = Any2Integer(dRow("ID"))
                    Me.Description = Any2String(dRow("Description"))

                    Dim oLicense As New roServerLicense
                    Try
                        bAccessLicenseInstalled = oLicense.FeatureIsInstalled("Forms\Access")
                        bLegacyRestrictionModeAllowed = _LegacyRestrictionModeAllowed
                    Catch ex As Exception
                    End Try

                    Try
                        'Obtenemos el path del terminal
                        ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                        Dim oSettings As New roSettings
                        Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                        If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                            _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                        End If
                        oSettings = New roSettings(_RegistryRoot & "Robotics\VisualTime")

                        sTerminalPath = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & Me.IDTerminal.ToString
                    Catch ex As Exception
                    End Try

                    Me.Mode = Any2String(dRow("Mode"))

                    Me.UseDispKey = Any2Boolean(dRow("UseDispKey"))

                    Me.InteractionMode = InteractionMode.Interactive
                    If Not IsDBNull(dRow("InteractionMode")) Then
                        Try
                            Me.InteractionMode = System.Enum.Parse(Me.InteractionMode.GetType, Any2String(dRow("InteractionMode")), True)
                        Catch
                            Me.InteractionMode = InteractionMode.Interactive
                        End Try
                    End If

                    Me.InteractionAction = Nothing
                    If Not IsDBNull(dRow("InteractionAction")) AndAlso Any2String(dRow("InteractionAction")) <> "" Then
                        Try
                            Me.InteractionAction = System.Enum.Parse(GetType(InteractionAction), Any2String(dRow("InteractionAction")), True)
                        Catch
                            Me.InteractionAction = Nothing
                        End Try
                    Else
                    End If

                    If Not IsDBNull(dRow("InteractiveConfig")) Then
                        Me.InteractiveConfig = New roInteractiveConfig(New roCollection(Any2String(dRow("InteractiveConfig"))), Me.oState)
                    Else
                        Me.InteractiveConfig = New roInteractiveConfig(New roCollection(), Me.oState)
                    End If

                    Me.ValidationMode = ValidationMode.ServerLocal
                    If Not IsDBNull(dRow("ValidationMode")) Then
                        Try
                            Me.ValidationMode = System.Enum.Parse(Me.ValidationMode.GetType, Any2String(dRow("ValidationMode")), True)
                        Catch
                            Me.ValidationMode = ValidationMode.ServerLocal
                        End Try
                    End If

                    Me.OHP = (oLicense.FeatureIsInstalled("Forms\Access") AndAlso oLicense.FeatureIsInstalled("Feature\OHP") AndAlso Me.Mode.Contains("ACC")) 'Any2Boolean(dRow("OHP"))
                    If Not IsDBNull(dRow("IDActivity")) Then
                        Me.IDActivity = Any2Integer(dRow("IDActivity"))
                    Else
                        Me.IDActivity = Nothing
                    End If

                    If dRow("IDZone") IsNot System.DBNull.Value Then Me.IDZone = Any2Integer(dRow("IDZone"))
                    If dRow("PictureX") IsNot System.DBNull.Value Then Me.PictureX = Any2Double(dRow("PictureX"))
                    If dRow("PictureY") IsNot System.DBNull.Value Then Me.PictureY = Any2Double(dRow("PictureY"))

                    Me.lstEmployeesLimit = New Generic.List(Of Integer)
                    If dRowsEmployees IsNot Nothing Then
                        For Each oRow As DataRow In dRowsEmployees
                            Me.lstEmployeesLimit.Add(oRow("IDEmployee"))
                        Next
                    End If

                    ' Configuración relés
                    If Not IsDBNull(dRow("Output")) Then
                        Me.Output = Any2Integer(dRow("Output"))
                    Else
                        Me.Output = Nothing
                    End If
                    Me.Duration = Any2Integer(dRow("Duration"))
                    If Not IsDBNull(dRow("InvalidOutput")) Then
                        Me.InvalidOutput = Any2Integer(dRow("InvalidOutput"))
                    Else
                        Me.InvalidOutput = Nothing
                    End If
                    Me.InvalidDuration = Any2Integer(dRow("InvalidDuration"))
                    Me.CustomButtons = roCustomButton.LoadFromXml(Any2String(dRow("CustomButtons")), Me.oState)

                    Me.RequestPin = Any2Integer(dRow("RequestPin"))
                    Me.AccessKey = Any2Integer(dRow("AccessKey"))
                    Me.Type = Any2String(dRow("Type"))

                    If dRow("IDCamera") IsNot System.DBNull.Value Then Me.IDCamera = dRow("IDCamera")
                    If dRow("IDZoneOut") IsNot System.DBNull.Value Then Me.IDZoneOut = Any2Integer(dRow("IDZoneOut"))
                    If dRow("PictureXOut") IsNot System.DBNull.Value Then Me.PictureXOut = Any2Double(dRow("PictureXOut"))
                    If dRow("PictureYOut") IsNot System.DBNull.Value Then Me.PictureYOut = Any2Double(dRow("PictureYOut"))
                    If dRow("IDCameraOut") IsNot System.DBNull.Value Then Me.IDCameraOut = dRow("IDCameraOut")

                    If dRow("IDCostCenter") IsNot System.DBNull.Value Then Me.IDCostCenter = Any2Integer(dRow("IDCostCenter"))

                    ' APB y NFC
                    Try
                        If Me.Mode.IndexOf("ACC") > -1 Then
                            ' Aquí verifico si la zona que controla tiene activado control de antipassback

                            ' El APB se aplica por grupos de zona. Una zona está en un único grupo
                            ' La cadena que contiene las zonas controladas de APB tiene el siguiente formato:
                            '           grupo1@grupo2@grupo3
                            ' y cada grupo tiene el formato id1,id2,id3,...
                            ' Ejemplo: 1,2@3,4@5,6

                            Dim sAPBZoneGroups As String = ""
                            Dim pAdvParameter As AdvancedParameter.roAdvancedParameter
                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("APBControlledZones", Nothing)
                            If Not pAdvParameter Is Nothing Then sAPBZoneGroups = roTypes.Any2String(pAdvParameter.Value)

                            ' Si hay varios grupos, los recorro en busca de uno que contenga la zona asignada al terminal
                            For i = 1 To sAPBZoneGroups.Split("@").Count
                                If System.Array.IndexOf(sAPBZoneGroups.Split("@")(i - 1).Split(","), roTypes.Any2String(Me.IDZone)) > -1 Then
                                    Me.APBControlledZones = sAPBZoneGroups.Split("@")(i - 1)
                                    Me.CheckAPB = True
                                    Exit For
                                End If
                            Next

                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("APBControlledMinutes", Nothing)
                            If Not pAdvParameter Is Nothing Then Me.APBControlledMinutes = roTypes.Any2Integer(pAdvParameter.Value)
                            If Me.APBControlledMinutes = 0 Then Me.APBControlledMinutes = 480

                            pAdvParameter = New AdvancedParameter.roAdvancedParameter("APBCourtesyTime", Nothing)
                            If Not pAdvParameter Is Nothing Then Me.APBCourtesyTime = roTypes.Any2String(pAdvParameter.Value)
                        Else
                            Me.CheckAPB = False
                        End If

                        Me.NfcTagValue = roTypes.Any2String(dRow("NFC"))
                    Catch ex As Exception
                    End Try
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:LoadByRow")
                End Try

            End Sub


            <OnDeserializing>
            Private Sub OnDeserialize(pp As StreamingContext)
                If Me.oState Is Nothing Then
                    Me.oState = New roTerminalState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
                End If
                If Me.strMode Is Nothing Then Me.strMode = String.Empty
            End Sub

            Public Function Save(Optional ByVal oTerminal As roTerminal = Nothing, Optional ByVal bolAudit As Boolean = True, Optional bolIsNewTerminal As Boolean = False) As Boolean
                Dim bolRet As Boolean = False

                Dim bHaveToClose As Boolean = False
                Try
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    If Me.Validate(oTerminal, bolIsNewTerminal) Then
                        Dim oAuditDataOld As DataRow = Nothing
                        Dim oAuditDataNew As DataRow = Nothing

                        Dim tb As New DataTable("TerminalReaders")

                        Dim strSQL As String = "@SELECT# * FROM TerminalReaders WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND ID = " & Me.ID.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            oRow("IDTerminal") = Me.IDTerminal
                            Me.intID = Me.GetNextID()
                            oRow("ID") = Me.intID
                        Else
                            oRow = tb.Rows(0)
                            oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        End If

                        'Carreguem / actualitzem el registre

                        oRow("Mode") = Me.Mode
                        oRow("UseDispKey") = Me.UseDispKey
                        oRow("InteractionMode") = System.Enum.GetName(Me.InteractionMode.GetType, Me.InteractionMode)
                        If Me.InteractionAction.HasValue Then
                            oRow("InteractionAction") = System.Enum.GetName(Me.InteractionAction.GetType, Me.InteractionAction)
                        Else
                            oRow("InteractionAction") = System.DBNull.Value
                        End If
                        If Me.oInteractiveConfig IsNot Nothing Then
                            oRow("InteractiveConfig") = Me.oInteractiveConfig.GetXml()
                        Else
                            oRow("InteractiveConfig") = DBNull.Value
                        End If
                        oRow("ValidationMode") = System.Enum.GetName(Me.ValidationMode.GetType, Me.ValidationMode)
                        oRow("OHP") = Me.OHP

                        If Me.IDActivity.HasValue Then
                            oRow("IDActivity") = Me.IDActivity.Value
                        Else
                            oRow("IDActivity") = DBNull.Value
                        End If

                        oRow("Description") = Me.Description
                        If Me.IDZone.HasValue Then
                            oRow("IDZone") = Me.IDZone
                        Else
                            oRow("IDZone") = System.DBNull.Value
                        End If

                        If Me.PictureX.HasValue Then
                            oRow("PictureX") = Me.PictureX
                        Else
                            oRow("PictureX") = System.DBNull.Value
                        End If
                        If Me.PictureY.HasValue Then
                            oRow("PictureY") = Me.PictureY
                        Else
                            oRow("PictureY") = System.DBNull.Value
                        End If

                        ' Actualizamos la configuración de relés
                        If Me.Output.HasValue Then
                            oRow("Output") = Me.Output
                        Else
                            oRow("Output") = System.DBNull.Value
                        End If
                        oRow("Duration") = Me.Duration
                        If Me.InvalidOutput.HasValue Then
                            oRow("InvalidOutput") = Me.InvalidOutput
                        Else
                            oRow("InvalidOutput") = System.DBNull.Value
                        End If
                        oRow("InvalidDuration") = Me.InvalidDuration
                        oRow("CustomButtons") = roCustomButton.GetXml(Me.lstCustomButtons)

                        oRow("RequestPin") = Me.RequestPin
                        oRow("AccessKey") = Me.AccessKey
                        oRow("Type") = Me.Type

                        If Me.intIDCamera.HasValue Then
                            oRow("IDCamera") = Me.intIDCamera
                        Else
                            oRow("IDCamera") = System.DBNull.Value
                        End If

                        If Me.IDZoneOut.HasValue Then
                            oRow("IDZoneOut") = Me.IDZoneOut
                        Else
                            oRow("IDZoneOut") = System.DBNull.Value
                        End If

                        If Me.IDCostCenter.HasValue Then
                            oRow("IDCostCenter") = Me.IDCostCenter
                        Else
                            oRow("IDCostCenter") = System.DBNull.Value
                        End If

                        If Me.PictureXOut.HasValue Then
                            oRow("PictureXOut") = Me.PictureXOut
                        Else
                            oRow("PictureXOut") = System.DBNull.Value
                        End If
                        If Me.PictureYOut.HasValue Then
                            oRow("PictureYOut") = Me.PictureYOut
                        Else
                            oRow("PictureYOut") = System.DBNull.Value
                        End If

                        If Me.intIDCameraOut.HasValue Then
                            oRow("IDCameraOut") = Me.intIDCameraOut
                        Else
                            oRow("IDCameraOut") = System.DBNull.Value
                        End If

                        oRow("NFC") = Me.NfcTagValue

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If

                        da.Update(tb)

                        bolRet = Me.SaveEmployeesLimit()

                        If bolRet Then
                            If Me.oInteractiveConfig IsNot Nothing AndAlso Me.oInteractiveConfig.PrinterName = "cloud" Then
                                Dim oAdvParam As New Robotics.Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameter("DinnerPrintOnFile", New AdvancedParameter.roAdvancedParameterState())
                                oAdvParam.Value = "*cloud"
                                bolRet = oAdvParam.Save()
                            Else
                                If oAuditDataOld IsNot Nothing AndAlso Not IsDBNull(oAuditDataOld("InteractiveConfig")) Then
                                    Dim oOldInteractiveConfig As New roInteractiveConfig(New roCollection(Any2String(oAuditDataOld("InteractiveConfig"))), Me.oState)
                                    If oOldInteractiveConfig.PrinterName <> String.Empty Then
                                        Dim oAdvParam As New Robotics.Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameter("DinnerPrintOnFile", New AdvancedParameter.roAdvancedParameterState())
                                        oAdvParam.Value = ""
                                        bolRet = oAdvParam.Save()
                                    End If
                                End If
                            End If
                        End If

                        oAuditDataNew = oRow

                        If bolRet And bolAudit Then
                            bolRet = False
                            ' Auditamos
                            Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                            Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                            Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                            Dim strObjectName As String
                            If oAuditAction = Audit.Action.aInsert Then
                                strObjectName = oAuditDataNew("Description")
                            Else
                                strObjectName = oAuditDataOld("Description") & " -> " & oAuditDataNew("Description")
                            End If
                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTerminalReader, strObjectName, tbAuditParameters, -1)
                        End If

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:Save")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:Save")
                Finally
                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End Try

                Return bolRet

            End Function

            ''' <summary>
            ''' Recupera el siguiente número de Id. de Reader para el Terminal
            ''' </summary>
            ''' <param name="oActiveTransaction">Transacción actual (opcional)</param>
            ''' <returns>Devuelve el núm. siguiente de terminal reader, en caso de error -1</returns>
            ''' <remarks></remarks>
            Private Function GetNextID() As Integer

                Dim intRet As Integer = -1
                Try

                    Dim tb As New DataTable("TerminalReaders")
                    Dim strSQL As String = "@SELECT# MAX(ID) as NextID FROM TerminalReaders WHERE IDTerminal = " & Me.IDTerminal.ToString

                    intRet = Any2Integer(ExecuteScalar(strSQL)) + 1
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:GetNextID")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:GetNextID")
                End Try

                Return intRet

            End Function

            Private Function SaveEmployeesLimit() As Boolean

                Dim bolRet As Boolean = False

                Try

                    Dim strSQL As String = "@DELETE# FROM TerminalReaderEmployees WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND IDReader = " & Me.ID.ToString
                    bolRet = ExecuteSql(strSQL)

                    If bolRet AndAlso Me.lstEmployeesLimit IsNot Nothing AndAlso Me.lstEmployeesLimit.Count > 0 Then

                        bolRet = False

                        Dim tbEmployees As New DataTable

                        strSQL = "@SELECT# * FROM TerminalReaderEmployees WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND IDReader = " & Me.ID.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tbEmployees)

                        Dim oRow As DataRow = Nothing
                        For Each intIDEmployee As Integer In Me.lstEmployeesLimit
                            oRow = tbEmployees.NewRow
                            oRow("IDTerminal") = Me.IDTerminal
                            oRow("IDReader") = Me.ID
                            oRow("IDEmployee") = intIDEmployee
                            tbEmployees.Rows.Add(oRow)
                        Next

                        da.Update(tbEmployees)

                        bolRet = True

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:SaveEmployeesLimit")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader:SaveEmployeesLimit")
                Finally

                End Try

                Return bolRet

            End Function

            Public Function Validate(Optional ByVal oTerminal As roTerminal = Nothing, Optional bolIsNewTerminal As Boolean = False) As Boolean

                Dim bolRet As Boolean = True

                Try

                    ' Verificamos si la configuración del lector es correcta según la table 'sysroReaderTemplates
                    If oTerminal Is Nothing Then oTerminal = New roTerminal(Me.intIDTerminal, Me.oState)

                    Dim strSQL As String = "@SELECT# * FROM sysroReaderTemplates " &
                                           "WHERE type = '" & oTerminal.Type & "' AND IDReader = " & Me.ID.ToString

                    strSQL &= " AND (ScopeMode = '" & Me.Mode & "' OR ScopeMode LIKE '%," & Me.Mode & ",%' OR ScopeMode LIKE '" & Me.Mode & ",%' OR ScopeMode LIKE '%," & Me.Mode & "')"
                    'strSQL &= " AND (UseDispKey LIKE '%" & IIf(Me.UseDispKey, "1", "0") & "%' OR UseDispKey IS NULL)"
                    strSQL &= " AND (OHP LIKE '%" & IIf(Me.OHP, "1", "0") & "%' OR OHP IS NULL)"
                    Dim strValidationMode As String = System.Enum.GetName(Me.ValidationMode.GetType, Me.ValidationMode)
                    strSQL &= " AND (ValidationMode = '" & strValidationMode & "' OR ValidationMode LIKE '%," & strValidationMode & ",%' OR ValidationMode LIKE '" & strValidationMode & ",%' OR ValidationMode LIKE '%," & strValidationMode & "' OR ValidationMode IS NULL)"

                    strSQL &= " AND (EmployeesLimit LIKE '%" & IIf(Me.EmployeesLimit IsNot Nothing AndAlso Me.EmployeesLimit.Count > 0, "1", "0") & "%' OR EmployeesLimit IS NULL)"
                    strSQL &= " AND (Output LIKE '%" & IIf(Me.Output.HasValue AndAlso Me.Output.Value > 0, "1", "0") & "%' OR Output IS NULL)"
                    strSQL &= " AND (InvalidOutput LIKE '%" & IIf(Me.InvalidOutput.HasValue AndAlso Me.InvalidOutput.Value > 0, "1", "0") & "%' OR InvalidOutput IS NULL)"

                    strSQL &= " AND (CustomButtons LIKE '%" & IIf(Me.CustomButtons IsNot Nothing AndAlso Me.CustomButtons.Count > 0, "1", "0") & "%' OR CustomButtons IS NULL)"

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        bolRet = (tb.Rows.Count > 0)
                    Else
                        bolRet = False
                    End If

                    'Esto puede fallar si no se creó el ostate (cosa que no debería pasar, pero pasa ...)
                    If Me.oState Is Nothing Then Me.oState = New roTerminalState

                    If Not bolRet Then Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidConfiguration

                    If bolRet AndAlso oTerminal.Type.ToUpper <> "VIRTUAL" AndAlso oTerminal.Type.ToUpper <> "MASTERASP" AndAlso Me.Mode.Contains("ACC") AndAlso Me.Output.HasValue AndAlso Me.Output.Value <= 0 Then
                        Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidConfiguration
                        bolRet = False
                    End If

                    ' Verificamos que la configuración de relés sea correcta
                    If bolRet AndAlso Me.Output.HasValue AndAlso Me.Output.Value > 0 AndAlso Me.Duration <= 0 Then
                        Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidOutputDuration
                        bolRet = False
                    End If
                    If bolRet AndAlso Me.InvalidOutput.HasValue AndAlso Me.InvalidOutput.Value > 0 AndAlso Me.InvalidDuration <= 0 Then
                        Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidInvalidOutputDuration
                        bolRet = False
                    End If

                    If bolRet AndAlso Me.CustomButtons IsNot Nothing Then
                        bolRet = roCustomButton.Validate(Me.CustomButtons)
                        If Not bolRet Then Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidCustomButtonConfiguration
                    End If

                    'Ask for Zone if it's MasterASP and you are modifying it
                    If bolRet AndAlso Me.Mode.Contains("ACC") AndAlso Not Me.intIDZone.HasValue AndAlso Not (oTerminal.Type = "masterASP" AndAlso bolIsNewTerminal) Then
                        Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidAccessZone
                        bolRet = False
                    End If

                    If Not bolIsNewTerminal Then
                        If bolRet AndAlso oTerminal.Type <> "LivePortal" AndAlso Not Me.Mode.Contains("ACC") AndAlso
                            (Me.InteractionAction = DTOs.InteractionAction.X OrElse Me.InteractionAction = DTOs.InteractionAction.ES OrElse Me.InteractionAction = DTOs.InteractionAction.E) AndAlso
                            (Not Me.intIDZone.HasValue) Then
                            If oTerminal.Type.ToUpper <> "TIME GATE" OrElse (oTerminal.Type.ToUpper = "TIME GATE" AndAlso Me.Mode = "TA") Then
                                Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidAccessZone
                                bolRet = False
                            End If
                        End If

                        If bolRet AndAlso oTerminal.Type.ToUpper() = "TIME GATE" AndAlso Me.Mode = "CO" Then
                            If Not Me.intIDZone.HasValue Then
                                Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidAccessZone
                                bolRet = False
                            End If

                            If bolRet AndAlso Not Me.intIDCostCener.HasValue Then
                                Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidCostCenter
                                bolRet = False
                            End If
                        End If

                        If bolRet AndAlso oTerminal.Type <> "LivePortal" AndAlso Not Me.Mode.Contains("ACC") AndAlso
                            (Me.InteractionAction = DTOs.InteractionAction.X OrElse Me.InteractionAction = DTOs.InteractionAction.ES OrElse Me.InteractionAction = DTOs.InteractionAction.S) AndAlso
                            (Not Me.intIDZoneOut.HasValue) Then
                            If oTerminal.Type.ToUpper <> "TIME GATE" OrElse (oTerminal.Type.ToUpper = "TIME GATE" AndAlso Me.Mode = "TA") Then
                                Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidAccessZoneOut
                                bolRet = False
                            End If
                        End If

                        ' Timegate, si en modo portal defino una zona, debo definir las dos
                        If bolRet AndAlso oTerminal.Type.ToUpper = "TIME GATE" AndAlso Me.Mode = "EIP" Then
                            If Me.intIDZone.HasValue <> Me.intIDZoneOut.HasValue Then
                                ' Deben estar informadas las dos zonas (entrada/salida)
                                Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidZonesForTimegate
                                bolRet = False
                            ElseIf Me.intIDZone.HasValue Then
                                Dim oZoneIn, oZoneOut As roZone
                                'La zona de entradas debe ser de presencia
                                oZoneIn = New roZone(Me.intIDZone.Value, New roZoneState(-1))
                                'Y la de salida debe ser de ausencia
                                oZoneOut = New roZone(Me.intIDZoneOut.Value, New roZoneState(-1))
                                If Not oZoneIn.IsWorkingZone OrElse oZoneOut.IsWorkingZone Then
                                    Me.oState.Result = TerminalBaseResultEnum.Reader_InvalidZonesForTimegate
                                    bolRet = False
                                End If
                            End If

                        End If
                    End If
                Catch ex As DbException
                    oState.UpdateStateInfo(ex, "roTerminalReader::Validate")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roTerminalReader::Validate")
                Finally

                End Try

                Return bolRet

            End Function

            Public Function Delete(Optional ByVal bolAudit As Boolean = True) As Boolean

                Dim bolRet As Boolean = False

                Dim bHaveToClose As Boolean = False
                Try
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim Querys() As String = {"@DELETE# FROM TerminalReaderEmployees " &
                                              "WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND " &
                                                    "IDReader = " & Me.ID.ToString,
                                              "@DELETE# FROM TerminalReaders " &
                                              "WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND " &
                                                    "ID = " & Me.ID.ToString}
                    For Each strSQL As String In Querys
                        ExecuteSql(strSQL)
                    Next

                    bolRet = True

                    If bolRet And bolAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTerminalReader, Me.Description, Nothing, -1)
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader::Delete")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReader::Delete")
                Finally
                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End Try

                Return bolRet

            End Function

#Region "Helper methods"

            Public Shared Function GetTemplates(ByVal _TerminalType As String, ByVal _State As roTerminalState, Optional ByVal _IDReader As Integer = -1) As DataTable

                Dim oRet As DataTable = Nothing

                Try

                    Dim strSQL As String = "@SELECT# * FROM sysroReaderTemplates"
                    If _TerminalType <> "" Then strSQL &= " WHERE Type = @TerminalType"
                    If _IDReader <> -1 Then strSQL &= " AND IDReader = " & _IDReader.ToString

                    Dim cmd As DbCommand = CreateCommand(strSQL)

                    If _TerminalType <> "" Then
                        AddParameter(cmd, "@TerminalType", DbType.String, 50)
                        cmd.Parameters("@TerminalType").Value = _TerminalType
                    End If

                    oRet = New DataTable("sysroReaderTemplates")

                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(oRet)
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTerminalReader::GetTemplates")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTerminalReader::GetTemplates")
                Finally

                End Try

                Return oRet

            End Function

            ''' <summary>
            ''' Guarda la configuración de lectores de un terminal.
            ''' </summary>
            ''' <param name="_IDTerminal">Código terminal</param>
            ''' <param name="_Readers">Lista de lectores. Si es nothing se borran todos los lectores.</param>
            ''' <param name="_State"></param>
            ''' <param name="oActiveTransaction"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            '''
            Public Shared Function SaveTerminalReaders(ByVal _IDTerminal As Integer, ByVal _Readers As ArrayList, ByVal _State As roTerminalState,
                                                        Optional ByVal bAudit As Boolean = False, Optional bolIsNewTerminal As Boolean = False) As Boolean

                Dim bolRet As Boolean = False

                Dim bHaveToClose As Boolean = False
                Try
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim ExcludeList As New Generic.List(Of Integer)

                    bolRet = True

                    If _Readers IsNot Nothing AndAlso _Readers.Count > 0 Then
                        For Each oReader As roTerminalReader In _Readers
                            bolRet = oReader.Save(, bAudit, bolIsNewTerminal)
                            ExcludeList.Add(oReader.ID)
                            If Not bolRet Then Exit For
                        Next
                    End If

                    If bolRet Then
                        Dim strSQL As String = "@DELETE# FROM TerminalReaders WHERE IDTerminal = " & _IDTerminal.ToString
                        If ExcludeList.Count > 0 Then
                            For Each intID As Integer In ExcludeList
                                strSQL &= " AND ID <> " & intID.ToString
                            Next
                        End If
                        bolRet = ExecuteSql(strSQL)
                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTerminalReader::SaveTerminalReaders")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTerminalReader::SaveTerminalReaders")
                Finally
                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End Try

                Return bolRet

            End Function

#End Region

#End Region

#Region "Class roCustomButton"

            <DataContract>
            Public Class roCustomButton

#Region "Declarations - Constructor"

                Private strLabel As String
                Private intOutput As Integer
                Private intDuration As Integer

                Private lstConditions As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)

                Private bolOnlyEntries As Boolean

                Public Sub New()
                    Me.lstConditions = New Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
                End Sub

                Public Sub New(ByVal colCustomButton As roCollection, ByVal _State As roTerminalState)

                    Me.lstConditions = New Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)

                    If colCustomButton IsNot Nothing Then

                        Me.strLabel = Any2String(colCustomButton.Item("Label"))
                        Me.intOutput = Any2Integer(colCustomButton.Item("Output"))
                        Me.intDuration = Any2Integer(colCustomButton.Item("Duration"))

                        Dim oConditionsNode As roCollection = colCustomButton.Node("Conditions")
                        If oConditionsNode IsNot Nothing Then
                            Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(_State.IDPassport, _State.Context, _State.ClientAddress)
                            Me.lstConditions = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(oConditionsNode.XML(), oUserFieldState)
                        End If

                        Me.bolOnlyEntries = Any2Boolean(colCustomButton.Item("OnlyEntries"))

                    End If

                End Sub

#End Region

#Region "Properties"

                <DataMember>
                Public Property Label() As String
                    Get
                        Return Me.strLabel
                    End Get
                    Set(ByVal value As String)
                        Me.strLabel = value
                    End Set
                End Property

                <DataMember>
                Public Property Output() As Integer
                    Get
                        Return Me.intOutput
                    End Get
                    Set(ByVal value As Integer)
                        Me.intOutput = value
                    End Set
                End Property

                <DataMember>
                Public Property Duration() As Integer
                    Get
                        Return Me.intDuration
                    End Get
                    Set(ByVal value As Integer)
                        Me.intDuration = value
                    End Set
                End Property

                <DataMember>
                Public Property Conditions() As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition)
                    Get
                        Return Me.lstConditions
                    End Get
                    Set(ByVal value As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition))
                        Me.lstConditions = value
                    End Set
                End Property

                <DataMember>
                Public Property OnlyEntries() As Boolean
                    Get
                        Return Me.bolOnlyEntries
                    End Get
                    Set(ByVal value As Boolean)
                        Me.bolOnlyEntries = value
                    End Set
                End Property

#End Region

#Region "Methods"

                Public Function GetXml() As String

                    Dim oCustomButton As New roCollection

                    oCustomButton.Add("Label", Me.strLabel)
                    oCustomButton.Add("Output", Me.intOutput)
                    oCustomButton.Add("Duration", Me.intDuration)
                    oCustomButton.Add("Conditions", New roCollection(VTUserFields.UserFields.roUserFieldCondition.GetXml(Me.lstConditions)))
                    oCustomButton.Add("OnlyEntries", Me.bolOnlyEntries)

                    Return oCustomButton.XML

                End Function

                Public Function Validate() As Boolean

                    Dim bolRet As Boolean = True

                    If Me.Label = "" Then
                        bolRet = False
                    ElseIf Me.Output <= 0 Then
                        bolRet = False
                    ElseIf Me.Duration <= 0 Then
                        bolRet = False
                        'ElseIf Me.Conditions.Count = 0 Then
                        '    bolRet = False
                    End If

                    Return bolRet

                End Function

#Region "Helper methods"

                Public Shared Function GetXml(ByVal CustomButtons As Generic.List(Of roCustomButton)) As String

                    Dim oCustomButtons As New roCollection

                    If CustomButtons IsNot Nothing Then

                        oCustomButtons.Add("TotalCustomButtons", CustomButtons.Count)

                        Dim n As Integer = 1
                        For Each oButton As roCustomButton In CustomButtons
                            oCustomButtons.Add("CustomButton" & n.ToString, New roCollection(oButton.GetXml))
                            n += 1
                        Next
                    Else
                        oCustomButtons.Add("TotalCustomButtons", 0)
                    End If

                    Return oCustomButtons.XML

                End Function

                Public Shared Function LoadFromXml(ByVal strXml As String, ByVal oState As roTerminalState) As Generic.List(Of roCustomButton)

                    Dim oRet As New Generic.List(Of roCustomButton)

                    If strXml <> "" Then

                        Dim oCustomButtons As New roCollection(strXml)

                        Dim n As Integer = 1
                        Dim oButtonNode As roCollection = oCustomButtons.Node("CustomButton" & n.ToString)
                        While oButtonNode IsNot Nothing
                            oRet.Add(New roCustomButton(oButtonNode, oState))
                            n += 1
                            oButtonNode = oCustomButtons.Node("CustomButton" & n.ToString)
                        End While

                    End If

                    Return oRet

                End Function

                Public Shared Function Validate(ByVal CustomButtons As Generic.List(Of roCustomButton)) As Boolean

                    Dim bolRet As Boolean = True

                    For Each oCustom As roCustomButton In CustomButtons
                        bolRet = oCustom.Validate
                        If Not bolRet Then Exit For
                    Next

                    Return bolRet

                End Function

#End Region

#End Region

            End Class

#End Region

#Region "Class roInteractiveConfig"

            <DataContract>
            Public Class roInteractiveConfig

#Region "Declarations - Constructor"

                Private bolQueries As Boolean           ' Se permite realizar consultas
                Private bolRequests As Boolean          ' Se permite realizar sloicitudes

                Private intPunchPeriodRTIn As Integer  ' Tiempo mínimo permitido entre una entrada y una salida
                Private intPunchPeriodRTOut As Integer ' Tiempo mínimo permitido entre una salida y una entrada

                Private strPrinterName As String 'Impresora para imprimir ticket de comedor

                Private strPrinterText As String 'Impresora para imprimir ticket de comedor

                Public Sub New()

                End Sub

                Public Sub New(ByVal colConfig As roCollection, ByVal _State As roTerminalState)

                    Dim oParameters As New roParameters
                    Me.intPunchPeriodRTIn = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTIn))
                    Me.intPunchPeriodRTOut = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTOut))

                    Me.strPrinterName = String.Empty
                    Me.strPrinterText = String.Empty

                    If colConfig IsNot Nothing Then

                        Me.bolQueries = Any2Boolean(colConfig.Item("Queries"))
                        Me.bolRequests = Any2Boolean(colConfig.Item("Requests"))

                        If colConfig.Item("PunchPeriodRTIn") IsNot Nothing Then
                            Me.intPunchPeriodRTIn = Any2Integer(colConfig.Item("PunchPeriodRTIn"))
                        End If

                        If colConfig.Item("PunchPeriodRTOut") IsNot Nothing Then
                            Me.intPunchPeriodRTOut = Any2Integer(colConfig.Item("PunchPeriodRTOut"))
                        End If

                        If colConfig.Item("PrinterName") IsNot Nothing Then
                            Me.strPrinterName = Any2String(colConfig.Item("PrinterName"))
                        End If

                        If colConfig.Item("PrinterText") IsNot Nothing Then
                            Me.strPrinterText = Any2String(colConfig.Item("PrinterText"))
                        End If
                    End If

                End Sub

#End Region

#Region "Properties"

                <DataMember>
                Public Property Queries() As Boolean
                    Get
                        Return Me.bolQueries
                    End Get
                    Set(ByVal value As Boolean)
                        Me.bolQueries = value
                    End Set
                End Property

                <DataMember>
                Public Property Requests() As Boolean
                    Get
                        Return Me.bolRequests
                    End Get
                    Set(ByVal value As Boolean)
                        Me.bolRequests = value
                    End Set
                End Property

                <DataMember>
                Public Property PunchPeriodRTIn() As Integer
                    Get
                        Return Me.intPunchPeriodRTIn
                    End Get
                    Set(ByVal value As Integer)
                        Me.intPunchPeriodRTIn = value
                    End Set
                End Property

                <DataMember>
                Public Property PunchPeriodRTOut() As Integer
                    Get
                        Return Me.intPunchPeriodRTOut
                    End Get
                    Set(ByVal value As Integer)
                        Me.intPunchPeriodRTOut = value
                    End Set
                End Property

                <DataMember>
                Public Property PrinterName() As String
                    Get
                        Return Me.strPrinterName
                    End Get
                    Set(ByVal value As String)
                        Me.strPrinterName = value
                    End Set
                End Property

                <DataMember>
                Public Property PrinterText() As String
                    Get
                        Return Me.strPrinterText
                    End Get
                    Set(ByVal value As String)
                        Me.strPrinterText = value
                    End Set
                End Property

#End Region

#Region "Methods"

                Public Function GetXml() As String

                    Dim oConfig As New roCollection

                    oConfig.Add("Queries", Me.bolQueries)
                    oConfig.Add("Requests", Me.bolRequests)
                    oConfig.Add("PunchPeriodRTIn", Me.intPunchPeriodRTIn)
                    oConfig.Add("PunchPeriodRTOut", Me.intPunchPeriodRTOut)

                    oConfig.Add("PrinterName", Me.strPrinterName)
                    oConfig.Add("PrinterText", Me.strPrinterText)

                    Return oConfig.XML

                End Function

                Public Function Validate() As Boolean

                    Dim bolRet As Boolean = True

                    Return bolRet

                End Function

#End Region

            End Class

#End Region

#Region "Empleados pemitidos"

            Public Function GetEmployeesHasLimitedPermitList() As List(Of Integer)
                Dim Ret As Boolean = False
                Dim count As Integer = 0
                Dim lst As List(Of Integer) = New List(Of Integer)

                Try
                    Dim sSql As String
                    sSql = " @SELECT# count(*) from TerminalReaderEmployees WHERE IDTerminal = " + Me.intIDTerminal.ToString
                    count = roTypes.Any2Integer(ExecuteScalar(sSql))
                    'Si hay limite de empleados buscamos si tiene permiso
                    If count > 0 Then
                        sSql = "@SELECT# distinct IDEmployee FROM TerminalReaderEmployees WHERE IDTerminal = " + Me.intIDTerminal.ToString
                        Dim dt As DataTable = CreateDataTable(sSql)
                        For i As Integer = 0 To dt.Rows.Count - 1
                            lst.Add(dt.Rows(i).Item(0))
                        Next
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetEmployeesHasLimitedPermitList")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetEmployeesHasLimitedPermitList")
                End Try
                Return lst
            End Function

            Public Function GetEmployeesUnautorizedField(Optional iIDEmployee As Integer = -1) As List(Of Integer)
                Dim oLst As List(Of Integer) = New List(Of Integer)
                Try
                    Dim oDatatable As DataTable = Nothing
                    Dim sSQL As String = ""

                    Dim oParam As New AdvancedParameter.roAdvancedParameter("CTaimaUserLinkEnabled", New AdvancedParameter.roAdvancedParameterState)
                    Dim strUserFieldRestriction As String = "Entra"

                    Dim oCTAIMAEnabled As Boolean = roTypes.Any2Boolean(oParam.Value)
                    If oCTAIMAEnabled Then strUserFieldRestriction = roTypes.Any2String((New AdvancedParameter.roAdvancedParameter("CTaimaPRLUserField", New AdvancedParameter.roAdvancedParameterState)).Value)

                    ' Miramos si en la ficha de empleado existe un campo que se llame "Entra"
                    sSQL = "@SELECT# * from sysroUserFields where FieldName = '" & strUserFieldRestriction & "' and Used = 1"
                    oDatatable = CreateDataTable(sSQL)
                    If oDatatable.Rows.Count > 0 Then
                        oDatatable = Nothing

                        sSQL = "@SELECT# idemployee, convert(varchar(20), date,120) +';'+ convert(varchar(1),value) as val"
                        sSQL += " from EmployeeUserFieldValues"
                        sSQL += " where FieldName='" & strUserFieldRestriction & "' "
                        sSQL += " and date < getdate()"
                        If iIDEmployee > 0 Then
                            sSQL += " and IDEmployee = " & iIDEmployee.ToString
                        End If
                        sSQL += " order by 1,2 desc"
                        Dim lstEmployees As List(Of Integer) = New List(Of Integer)
                        oDatatable = CreateDataTable(sSQL, )
                        For Each oRow As DataRow In oDatatable.Rows
                            If Not lstEmployees.Contains(roTypes.Any2Integer(oRow.Item(0))) Then
                                If roTypes.Any2String(oRow.Item("val")).Split(";")(1).Trim.ToUpper = "N" Then
                                    oLst.Add(roTypes.Any2Integer(oRow.Item(0)))
                                End If
                                lstEmployees.Add(roTypes.Any2Integer(oRow.Item(0)))
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetEmployeesUnautorizedField")
                End Try

                Return oLst
            End Function

            Private Function GetGroupsPemits() As String

                Dim sGroups As String = ""

                Try
                    Dim dt As DataTable = New DataTable
                    Dim cGroups As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)
                    Dim sDsc As String = ""
                    Dim sSQL As String = "@SELECT# id, DescriptionGroup, path from groups order by len(path), path"
                    Dim mCurrentNum As String = ""

                    dt = CreateDataTable(sSQL)

                    bolEmployeePermitByGroupActive = False

                    For Each row As DataRow In dt.Rows
                        sDsc = roTypes.Any2String(row.Item("DescriptionGroup"))

                        'Miramos si contiene definiciónde terminales
                        If sDsc.ToUpper.IndexOf("TERMINAL=") >= 0 Then
                            bolEmployeePermitByGroupActive = True
                            Dim sNum As String
                            Try
                                sNum = sDsc.Substring(sDsc.ToUpper.IndexOf("TERMINAL=") + 9, sDsc.IndexOf(";", sDsc.ToUpper.IndexOf("TERMINAL=")) - (sDsc.ToUpper.IndexOf("TERMINAL=") + 9))
                            Catch ex As Exception
                                sNum = ""
                            End Try

                            If sNum.Length > 0 Then
                                cGroups.Add(row.Item("id"), sNum)
                            Else
                                Dim sPath As String = roTypes.Any2String(row.Item("path"))
                                If sPath.Split("\").Length > 1 Then
                                    cGroups.Add(row.Item("id"), cGroups(roTypes.Any2Integer(sPath.Split("\").GetValue(sPath.Split("\").Length - 2))))
                                Else
                                    cGroups.Add(row.Item("id"), "")
                                End If
                            End If
                        Else
                            Dim sPath As String = roTypes.Any2String(row.Item("path"))
                            If sPath.Split("\").Length > 1 Then
                                cGroups.Add(row.Item("id"), cGroups(roTypes.Any2Integer(sPath.Split("\").GetValue(sPath.Split("\").Length - 2))))
                            Else
                                cGroups.Add(row.Item("id"), "")
                            End If
                        End If

                    Next

                    For Each item As KeyValuePair(Of Integer, String) In cGroups

                        If item.Value = "" Or ("," + item.Value + ",").IndexOf("," + Me.intIDTerminal.ToString + ",") >= 0 Then
                            sGroups += IIf(sGroups.Length > 0, ",", "") + item.Key.ToString
                        End If

                    Next

                    If sGroups.Length = 0 Then sGroups = "0"
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetGroupsPemits")
                End Try

                Return sGroups

            End Function

            Public Function GetListOfEmployeePermitByGroup() As List(Of Integer)

                Dim lEmploye As List(Of Integer) = New List(Of Integer)
                Try

                    Dim dt As DataTable
                    Dim sGroup As String = GetGroupsPemits()
                    Dim sSQL As String = "@SELECT# idemployee from sysrovwCurrentEmployeeGroups"
                    If sGroup.Length > 0 Then sSQL += " where idgroup in (" + sGroup + ")"
                    dt = CreateDataTable(sSQL, )

                    For Each row As DataRow In dt.Rows
                        lEmploye.Add(row.Item("idemployee"))
                    Next
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetListOfEmployeePermitByGroup")
                End Try

                Return lEmploye

            End Function

            Public Function GetListOfEmployeeAccessGroupAndContract(Optional ByVal bAdvancedAccessMode As Boolean = False, Optional bChekTimeZones As Boolean = False, Optional iIDEmployee As Integer = -1, Optional ByRef sValidationResult As String = "AV") As List(Of Integer)
                Dim lEmploye As List(Of Integer) = New List(Of Integer)
                Try
                    Dim sSql As String
                    If Not bAdvancedAccessMode Then
                        sSql = " @SELECT# distinct dbo.Employees.ID as EmployeeID"
                        sSql += " FROM Employees WITH (NOLOCK)"
                        sSql += " INNER JOIN EmployeeContracts WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID"
                        sSql += " 		AND  dbo.EmployeeContracts.BeginDate < GETDATE() AND DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate "
                        If Me.HasAccess OrElse CheckAccessAuthorizationOnNoAccessReaders Then
                            sSql += " INNER JOIN AccessGroupsPermissions WITH (NOLOCK) ON Employees.IDAccessGroup=AccessGroupsPermissions.IDAccessGroup"
                            sSql += " INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AccessGroupsPermissions.IDZone AND TerminalReaders.IDZone > -1 "
                            sSql += " 		AND TerminalReaders.IDTerminal=" + intIDTerminal.ToString + " AND TerminalReaders.ID=" + intID.ToString
                        End If
                    Else
                        If Not bChekTimeZones Then
                            sSql = " @SELECT# distinct dbo.Employees.ID as EmployeeID"
                        Else
                            sSql = " @SELECT# * FROM ( "
                            sSql += " @SELECT# ROW_NUMBER() OVER(PARTITION BY Employees.ID, AccPeriods.GroupID ORDER BY AccPeriods.Holiday DESC) AS RowNumber, dbo.Employees.ID as EmployeeID, AccPeriods.GroupID , AccPeriods.Holiday, AccPeriods.Day, AccPeriods.DayofWeek, AccPeriods.Month,  AccPeriods.BeginTime, AccPeriods.EndTime "
                        End If
                        sSql += " FROM Employees WITH (NOLOCK)"
                        sSql += " INNER JOIN EmployeeContracts WITH (NOLOCK) On EmployeeContracts.IDEmployee = Employees.ID"
                        sSql += " 		And  dbo.EmployeeContracts.BeginDate < GETDATE() And DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate "
                        If Me.HasAccess OrElse CheckAccessAuthorizationOnNoAccessReaders Then
                            sSql += "LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) On sysrovwAccessAuthorizations.IDEmployee = Employees.ID "
                            sSql += "INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP On sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                            sSql += " INNER JOIN TerminalReaders WITH (NOLOCK) On TerminalReaders.IDZone=AGP.IDZone AND TerminalReaders.IDZone > -1 "
                            sSql += " 		And TerminalReaders.IDTerminal=" + intIDTerminal.ToString + " And TerminalReaders.ID=" + intID.ToString
                        End If

                        If bChekTimeZones Then
                            sSql += " INNER JOIN ("
                            sSql += "@SELECT# TR.ID As ReaderID, AG.ID As GroupID, 0 As Day, 0 As Month, AGP.ID As PeriodID, AGPD.DayOfWeek, AGPD.BeginTime, AGPD.EndTime, 0 As Holiday "
                            sSql += " FROM dbo.TerminalReaders TR WITH (NOLOCK) "
                            sSql += " INNER JOIN dbo.Zones Z WITH (NOLOCK) On TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ On Z.ID = AGPZ.IDZone "
                            sSql += " INNER Join dbo.AccessGroups AG WITH (NOLOCK) On AGPZ.IDAccessGroup = AG.ID "
                            sSql += " INNER Join dbo.AccessPeriodDaily AGPD WITH (NOLOCK) On AGPZ.IDAccessPeriod = AGPD.IDAccessPeriod "
                            sSql += " INNER Join dbo.AccessPeriods AGP WITH (NOLOCK) On AGP.ID = AGPZ.IDAccessPeriod "
                            sSql += " Where TR.IDTerminal = " & intIDTerminal.ToString & " And AGPD.DayOfWeek < 8 "
                            sSql += " UNION "
                            sSql += " @SELECT# TR.ID As ReaderID, AG.ID As GroupID, AGPH.Day, AGPH.Month, AGP.ID As PeriodID, -1 As DayOfWeek, AGPH.BeginTime, AGPH.EndTime, 1 as HoliDay "
                            sSql += " FROM dbo.TerminalReaders TR WITH (NOLOCK) "
                            sSql += " INNER Join dbo.Zones Z WITH (NOLOCK) ON TR.IDZone = Z.ID "
                            sSql += " INNER Join dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON Z.ID = AGPZ.IDZone "
                            sSql += " INNER Join dbo.AccessGroups AG WITH (NOLOCK) ON AGPZ.IDAccessGroup = AG.ID "
                            sSql += " INNER Join dbo.AccessPeriodHolidays AGPH WITH (NOLOCK) ON AGPZ.IDAccessPeriod = AGPH.IDAccessPeriod "
                            sSql += " INNER Join dbo.AccessPeriods AGP WITH (NOLOCK) ON AGP.ID = AGPZ.IDAccessPeriod "
                            sSql += " Where TR.IDTerminal = " & intIDTerminal.ToString & " And agph.Month <> 0"
                            sSql += " ) AccPeriods "
                            sSql += " 		On AccPeriods.ReaderID=" & intID.ToString & " And AccPeriods.GroupID = sysrovwAccessAuthorizations.IDAuthorization "
                            'sSql += " 		   and (AccPeriods.DayOfWeek = case DATEPART(weekday,getdate()) WHEN 1 THEN 7 ELSE DATEPART(weekday,getdate()) - 1 END OR (DATEPART(month,getdate()) = AccPeriods.Month AND DATEPART(day,getdate()) = AccPeriods.Day))"
                            sSql += " 		   and (AccPeriods.DayOfWeek = (((DatePart(WEEKDAY, getdate()) + @@DATEFIRST + 6 - 1 ) % 7) + 1) OR (DATEPART(month,getdate()) = AccPeriods.Month AND DATEPART(day,getdate()) = AccPeriods.Day))"
                            sSql += " ) as TMP "
                            sSql += " WHERE TMP.RowNumber = 1 "
                            sSql += " And CONVERT(time,getdate(), 14) between CONVERT(time,TMP.begintime, 14) and CONVERT(time,TMP.endtime, 14) "
                            sSql += " And TMP.EmployeeID = " & iIDEmployee.ToString
                        End If
                    End If

                    Dim dt As DataTable = CreateDataTable(sSql)

                    If Not bChekTimeZones Then
                        For Each row As DataRow In dt.Rows
                            lEmploye.Add(row.Item(0))
                        Next
                    Else
                        ' Hago dos listas. La de empleados que
                        For Each row As DataRow In dt.Rows
                            If Not lEmploye.Contains(row("EmployeeID")) Then
                                If row("Holiday") = 0 Then
                                    lEmploye.Add(row("EmployeeID"))
                                End If
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetListOfEmployeeAccessGroup")
                End Try

                Return lEmploye

            End Function

            Public Function GetEmployeeWithOHPDocFaults(Optional iIDEmployee As Integer = -1) As List(Of Integer)
                Dim lst As List(Of Integer) = New List(Of Integer)
                Dim strQuery As String = String.Empty

                Try
                    Dim tbRet As New DataTable
                    strQuery = "@SELECT# distinct idemployee from sysrovwEmployeePRLDocumentaionFaults where idzone = " & Me.intIDZone & " and accessvalidation = " & DTOs.DocumentAccessValidation.AccessDenied
                    If iIDEmployee <> -1 Then
                        strQuery += " and IDEmployee = " & iIDEmployee
                    End If
                    tbRet = CreateDataTable(strQuery)
                    If Not tbRet Is Nothing AndAlso tbRet.Rows.Count > 0 Then
                        For Each row As DataRow In tbRet.Rows
                            If Not lst.Contains(Any2Integer(row.Item("IDEmployee"))) Then
                                lst.Add(Any2Integer(row.Item("IDEmployee")))
                            End If
                        Next
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetEmployeeWithOHPDocFaults")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:GetEmployeeWithOHPDocFaults")
                End Try
                Return lst
            End Function

            Public Function GetEmployeeWithOHPExpiredLegacy() As List(Of Integer)
                Dim lst As List(Of Integer) = New List(Of Integer)

                Try
                    Dim tbRet As New DataTable
                    Dim Command As DbCommand = CreateCommand("TerminalDocuments")
                    Command.CommandType = CommandType.StoredProcedure
                    AddParameter(Command, "@idTerminal", DbType.Int32).Value = Me.intIDTerminal
                    AddParameter(Command, "@date", DbType.DateTime).Value = Now.Date
                    AddParameter(Command, "@OnlyExpired", DbType.Boolean).Value = True
                    Dim Adapter As DbDataAdapter = CreateDataAdapter(Command)
                    Adapter.Fill(tbRet)
                    For Each row As DataRow In tbRet.Rows
                        If Any2Integer(row.Item("AccessValidation")) = 1 And Any2Integer(row.Item("IDReader")) = Me.intID Then
                            If Not lst.Contains(Any2Integer(row.Item("IDEmployee"))) Then
                                lst.Add(Any2Integer(row.Item("IDEmployee")))
                            End If
                        End If
                    Next
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:LoadEmployeeWithOHPExpired")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:LoadEmployeeWithOHPExpired")
                End Try
                Return lst
            End Function

            Private Sub LoadEmployeePermit(Optional ByVal bAdvancedAccessMode As Boolean = False, Optional bCheckTimeZone As Boolean = False, Optional iIDEmployee As Integer = -1, Optional ByRef sValidationResult As String = "AV")

                Try

                    'Cargamos los empleados permitidos en la lista de la pantalla de terminales
                    If LegacyRestrictionModeAllowed Then lEmployeePermitList = GetEmployeesHasLimitedPermitList()

                    ' Obtenemos los empleados que en el campo de la ficha 'Entra' tienen un N
                    lEmployeesUnautorizedField = GetEmployeesUnautorizedField(IIf(bCheckTimeZone, iIDEmployee, -1))

                    'Cargamos los empleados con documentos caducados si tenemos PRL
                    lEmployeesUnautorizedOHP.Clear()
                    If bolOHP Then lEmployeesUnautorizedOHP = GetEmployeeWithOHPDocFaults(IIf(bCheckTimeZone, iIDEmployee, -1))

                    ' Limitación de empleados según terminales asignados a los grupos de empleados
                    If LegacyRestrictionModeAllowed Then lEmployeePermitByGroupList = GetListOfEmployeePermitByGroup()

                    'Si hay accesos cargamos los empleados permitidos por accesos
                    lEmployeeAccessGroupAndContract = GetListOfEmployeeAccessGroupAndContract(bAdvancedAccessMode, bCheckTimeZone, iIDEmployee, sValidationResult)

                    If LegacyRestrictionModeAllowed Then
                        Dim parameterName = "Robotics\VisualTime\Server\RestrictedPresenceActived"
                        Dim _registryRoot = "HKEY_LOCAL_MACHINE\Software\"

                        Dim _isUsingWindowsRegistry = roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("BroadcasterUseWindowsRegistry", New AdvancedParameter.roAdvancedParameterState).Value)

                        Dim advancedParameter As AdvancedParameter.roAdvancedParameter = New AdvancedParameter.roAdvancedParameter("Server.RestrictedPresenceActived", New AdvancedParameter.roAdvancedParameterState)

                        If Not advancedParameter.Exists Then
                            ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                            _registryRoot = IIf(Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing,
                            "HKEY_LOCAL_MACHINE\Software\Wow6432node\",
                            "HKEY_LOCAL_MACHINE\Software\")

                            advancedParameter.Value = Microsoft.Win32.Registry.GetValue(_registryRoot, parameterName, String.Empty)
                            advancedParameter.Save()
                        End If

                        Dim parameterValue = If(_isUsingWindowsRegistry,
                             Microsoft.Win32.Registry.GetValue(_registryRoot, parameterName, String.Empty),
                             roTypes.Any2String(New AdvancedParameter.roAdvancedParameter(parameterName, New AdvancedParameter.roAdvancedParameterState).Value))

                        bolModeRestrictively = roTypes.Any2Boolean(parameterValue)
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:LoadEmployeePermit")
                End Try
            End Sub

            Public Function EmployeePermit(ByVal IDEmployee As Integer, ByVal CheckOHP As Boolean, Optional bIsAccessWithTimeZoneCheck As Boolean = False, Optional ByRef sValidationResult As String = "AV") As Boolean
                Dim bAdvancedAccessMode As Boolean = False
                Dim pAdvancedAccessMode As New AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", New AdvancedParameter.roAdvancedParameterState())
                bAdvancedAccessMode = roTypes.Any2String(pAdvancedAccessMode.Value) = "1"
                Return EmployeePermit(IDEmployee, CheckOHP, bAdvancedAccessMode, bIsAccessWithTimeZoneCheck, sValidationResult)
            End Function

            Public Function EmployeePermit(ByVal IDEmployee As Integer, ByVal CheckOHP As Boolean, ByVal bAdvancedAccessMode As Boolean, Optional bIsAccessWithTimeZoneCheck As Boolean = False, Optional ByRef sValidationResult As String = "AV") As Boolean

                'Si es un segundo lector sin configurar (sólo ocurre con centraliras mxC) nadie puede entrar, no está configurado y nadie tiene accesos ...
                If Me.ID > 1 AndAlso Me.Mode.Trim = "" AndAlso roTypes.Any2Long(Me.intIDZone) = 0 Then Return False

                Try
                    ' Vemos cuándo se produjo el último cambio en configuración de empleados, por si hay que recargar las listas.
                    ' Hay que evitar mirar cada vez la última actualización de datos en tabla de empleados. Cuando se a esta función se llega desde Broadcaster, pueden haber miles de llamadas muy seguidas, y en paralelo, lo que provoca una carga innecesaria en las tablas de sincronización
                    If dLastPersistedEmployeeListUpdate.Year = 1970 OrElse Math.Abs(Now.Subtract(dLastPersistedEmployeeListCheck).TotalSeconds) > 30 Then
                        Dim sql As String = "@SELECT# * FROM TerminalsSyncEmployeesData WITH (NOLOCK) WHERE TerminalId = " & IDTerminal
                        Dim tb As DataTable = AccessHelper.CreateDataTable(sql)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            dLastPersistedEmployeeListUpdate = roTypes.Any2DateTime(tb.Rows(0)("TimeStamp"))
                        Else
                            dLastPersistedEmployeeListUpdate = Now
                        End If
                        dLastPersistedEmployeeListCheck = Now
                    End If

                    If Math.Abs(dLastEmployeeListOnMemoryUpdate.Subtract(dLastPersistedEmployeeListUpdate).TotalSeconds) > 5 OrElse bIsAccessWithTimeZoneCheck OrElse bEmployeePermitedListEmployeeMode Then
                        LoadEmployeePermit(bAdvancedAccessMode, bIsAccessWithTimeZoneCheck, IDEmployee, sValidationResult)
                        dLastEmployeeListOnMemoryUpdate = dLastPersistedEmployeeListUpdate
                        bEmployeePermitedListEmployeeMode = bIsAccessWithTimeZoneCheck '(si he mirado a nivel de empleado, marco para que la siguiente se recalcule la lista de empleados)
                    End If

                    'Miramos si hay restricción por autorizaciones de accesos
                    If Not lEmployeeAccessGroupAndContract.Contains(IDEmployee) Then
                        Return False
                    End If

                    'Miramos si hay restricción por el tema de la ficha del empleado
                    If lEmployeesUnautorizedField.Count > 0 AndAlso lEmployeesUnautorizedField.Contains(IDEmployee) Then
                        Return False
                    End If

                    'Si hay licencia de accesos y PRL, y el lector controla accesos ...
                    If bolOHP Then
                        ' Si debo verificar PRL (porque el terminal no es compatible en modo offline con PRL)
                        If CheckOHP Then
                            If lEmployeesUnautorizedOHP.Count > 0 Then
                                If lEmployeesUnautorizedOHP.Contains(IDEmployee) Then
                                    Return False
                                End If
                            End If
                        End If
                    End If

                    'Solo miramos los grupos de empleados y la restricción por terminal si no tiene accesos
                    If Not HasAccess AndAlso LegacyRestrictionModeAllowed Then
                        'Modo Legacy o restricción mediante autorizaciones de acceso
                        If LegacyRestrictionModeAllowed Then
                            If bolModeRestrictively Then
                                'Modo restrictivo, ha de existir en las dos listas

                                'Miramos si hay restricción por el filtro de la pantalla de terminales
                                If lEmployeePermitList.Count > 0 Then
                                    If Not lEmployeePermitList.Contains(IDEmployee) Then
                                        Return False
                                    End If
                                End If
                                'Miramos si hay restricciones por el grupo de empleados.
                                If bolEmployeePermitByGroupActive Then
                                    If Not lEmployeePermitByGroupList.Contains(IDEmployee) Then
                                        Return False
                                    End If
                                End If
                            Else
                                'Modo no restrictivo, si existe en una de las listas es correcto

                                'Miramos si hay restricciones por el grupo de empleados.
                                If bolEmployeePermitByGroupActive Then
                                    If lEmployeePermitByGroupList.Contains(IDEmployee) Then
                                        Return True
                                    Else
                                        'Miramos si hay permitido por el filtro de la pantalla de terminales
                                        If lEmployeePermitList.Count > 0 AndAlso lEmployeePermitList.Contains(IDEmployee) Then
                                            Return True
                                        End If
                                        Return False
                                    End If
                                End If
                            End If
                        Else
                            ' Restricción mediante autorizaciones de acceso.
                            ' Ya se aplicó esta restricción al cargar la lista lEmployeeAccessGroupAndContract
                        End If
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTerminalReaders:EmployeePermit")
                Finally

                End Try

                Return True

            End Function

        End Class

#End Region

        ''' <summary>
        ''' Recupera el siguiente número de Id. de Reader para el Terminal
        ''' </summary>
        ''' <param name="oActiveTransaction">Transacción actual (opcional)</param>
        ''' <returns>Devuelve el núm. siguiente de terminal reader, en caso de error -1</returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveTerminalReaderNextID(ByVal _IDTerminal As Integer, ByVal _State As roTerminalState) As Integer

            Dim intRet As Integer = -1

            Try

                Dim tb As New DataTable("TerminalReaders")
                Dim strSQL As String = "@SELECT# MAX(ID) as NextID FROM TerminalReaders WHERE IDTerminal = " & _IDTerminal.ToString

                intRet = Any2Integer(ExecuteScalar(strSQL)) + 1
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminalReader:GetNextID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminalReader:GetNextID")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetTerminalBySerialNumber(ByVal strSerialNumber As String, ByVal oState As roTerminalState) As roTerminal

            Dim strRet As roTerminal = Nothing

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID FROM Terminals WHERE SerialNumber = '" & strSerialNumber & "'"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("ID")) Then strRet = New roTerminal(roTypes.Any2Integer(tb.Rows(0)("ID")), oState, False, True)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminal::GetTerminalBySerialNumber")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminal::GetTerminalBySerialNumber")
            End Try

            Return strRet

        End Function

        Public Shared Function GetRegistratrionState(ByVal strSerialNumber As String, ByVal oState As roTerminalState) As TerminalRegitrationState

            Dim response As TerminalRegitrationState = TerminalRegitrationState.DoesNotExists

            Try

                Dim sql As String = $"@SELECT# Mode FROM TerminalReaders 
                                         INNER JOIN Terminals ON Terminals.Id = TerminalReaders.IDTerminal 
                                         WHERE Terminals.SerialNumber = '{strSerialNumber}'"
                Dim table As DataTable = CreateDataTable(sql)

                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    Dim mode As String = roTypes.Any2String(table.Rows(0)("Mode"))
                    response = TerminalRegitrationState.ExistsButNotConfigured
                    If mode.Length > 0 Then
                        response = TerminalRegitrationState.ExistsAndConfigured
                    End If
                Else
                    response = TerminalRegitrationState.DoesNotExists
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminal::IsTerminalConfigured")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminal::IsTerminalConfigured")
            End Try

            Return response

        End Function

        Public Shared Function GetForceConfig(ByVal strSerialNumber As String, ByVal oState As roTerminalState) As Boolean

            Dim bRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ForceConfig FROM Terminals WHERE SerialNumber = '" & strSerialNumber & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    If Not IsDBNull(oRow("ForceConfig")) Then bRet = roTypes.Any2Boolean(oRow("ForceConfig"))
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminal::GetForceConfig")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminal::GetForceConfig")
            Finally

            End Try

            Return bRet

        End Function

        Public Shared Function ValidateTerminalSecurityConnection(ByVal authToken As String, ByVal curTerminal As roTerminal, ByRef timeZone As TimeZoneInfo, ByRef bTerminalRegistered As Boolean, ByRef strTerminalSecurityToken As String, ByRef oState As roTerminalState, dLastConfigLoad As DateTime, ByRef bConfigReloaded As Boolean) As roTerminal
            Dim oRet As roTerminal = Nothing

            Try

                Dim sessionContext As String = String.Empty
                Dim oTermState As New roTerminalState(1)

                bConfigReloaded = False

                If curTerminal IsNot Nothing AndAlso curTerminal.SerialNumber = authToken Then
                    oRet = curTerminal
                    ' Si ya tengo cargada la información del terminal, miro si debo recargarla (lo miro sólo cada X minutos)
                    If roTerminal.GetForceConfig(curTerminal.SerialNumber, oTermState) Then
                        oRet = roTerminal.GetTerminalBySerialNumber(authToken, oState)
                        roTerminal.ClearForceConfig(curTerminal.ID, oTermState)
                        bConfigReloaded = True
                    End If
                Else
                    oRet = roTerminal.GetTerminalBySerialNumber(authToken, oState)
                    bConfigReloaded = True
                End If

                If oRet IsNot Nothing Then
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(oRet.TimeZoneName)

                    strTerminalSecurityToken = oRet.GetTerminalSecurityToken(authToken)
                    bTerminalRegistered = oRet.CheckTerminalSerialNum(oRet.RegistrationCode, oRet.SerialNumber)

                    If Not bTerminalRegistered Then
                        oState.Result = TerminalBaseResultEnum.InvalidRegistrationCode
                    End If
                Else
                    If oState.Result = TerminalBaseResultEnum.NoError Then
                        oState.Result = TerminalBaseResultEnum.TerminalDoesNotExists
                    Else
                        Return Nothing
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            End Try

            Return oRet
        End Function

    End Class

    <DataContract,
        KnownType(GetType(roTerminal))>
    Public Class roTerminalList

#Region "Declarations - Constructors"

        Private oState As roTerminalState

        Private Items As ArrayList

        Public Sub New()

            Me.oState = New roTerminalState
            Me.Items = New ArrayList

        End Sub

        Public Sub New(ByVal _State As roTerminalState)

            Me.oState = _State
            Me.Items = New ArrayList

        End Sub

#End Region

#Region "Properties"

        <XmlArray("Terminals"), XmlArrayItem("roTerminal", GetType(roTerminal))>
        <DataMember>
        Public Property Terminals() As ArrayList
            Get
                Return Me.Items
            End Get
            Set(ByVal value As ArrayList)
                Me.Items = value
            End Set
        End Property

        <IgnoreDataMember>
        Public ReadOnly Property State() As roTerminalState
            Get
                Return Me.oState
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bAudit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Try
                For Each oTerminal As roTerminal In Me.Terminals
                    bolRet = oTerminal.Save(bAudit)
                    If Not bolRet Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roTerminalList::Save")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal tbTerminals As DataTable, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oTerminal As roTerminal
                Dim bolSaved As Boolean = True

                For Each oRow As DataRow In tbTerminals.Rows

                    bolSaved = True
                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified
                            oTerminal = New roTerminal(oRow, Me.oState)
                            bolRet = oTerminal.Save(bAudit)

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cambiar el estado de la fila para poder leer sus datos
                            oTerminal = New roTerminal(CInt(oRow("ID")), Me.oState)
                            bolRet = oTerminal.Delete(bAudit)

                        Case Else
                            bolRet = True
                            bolSaved = False

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If

                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roTerminalList::Save")
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal ds As DataSet, ByRef oState As roTerminalState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ds.Tables.Contains("Terminals") Then

                    Dim tb As DataTable = ds.Tables("Terminals")
                    Dim oTerminal As roTerminal

                    For Each oRow As DataRow In tb.Rows
                        oTerminal = New roTerminal(Any2Integer(oRow("ID")), oState)
                        With oTerminal
                            'If Not IsDBNull(oRow("Value")) Then .Value = oRow("Value")
                            'If Not IsDBNull(oRow("Manual")) Then .Manual = oRow("Manual")
                            'If Not IsDBNull(oRow("CauseUser")) Then .CauseUser = oRow("CauseUser")
                            'If Not IsDBNull(oRow("CauseUserType")) Then .CauseUserType = oRow("CauseUserType")
                            'If Not IsDBNull(oRow("IsNotReliable")) Then .IsNotReliable = oRow("IsNotReliable")
                        End With
                        Me.Terminals.Add(oTerminal)
                    Next

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalList:LoadData")
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal strWhere As String, ByRef oState As roTerminalState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# [ID] FROM Terminals "
                If strWhere <> "" Then
                    strSQL &= "WHERE " & strWhere & " "
                    strSQL &= " AND (UPPER(Type) <> 'LIVEPORTAL' OR ISNULL(Other,'') <> 'APP') "
                    strSQL &= " AND (Deleted = 0 OR Deleted IS NULL) "
                Else
                    strSQL &= " WHERE (UPPER(Type) <> 'LIVEPORTAL' OR ISNULL(Other,'') <> 'APP') "
                    strSQL &= " AND (Deleted = 0 OR Deleted IS NULL) "
                End If
                strSQL &= "ORDER BY Terminals.Enabled DESC, Terminals.Description ASC"

                Dim tb As DataTable = CreateDataTable(strSQL, "Terminals")

                Dim oTerminal As roTerminal
                For Each oRow As DataRow In tb.Rows
                    oTerminal = New roTerminal(Any2Integer(oRow("ID")), oState)
                    Me.Terminals.Add(oTerminal)
                Next

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalList:LoadData")
            End Try

            Return bolRet

        End Function

        Public Function GetTerminals(Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM Terminals "
                If strWhere <> "" Then strSQL &= "WHERE " & strWhere & " "
                strSQL &= "ORDER BY Terminals.ID"

                oRet = CreateDataTable(strSQL, "Terminals")

                For Each dRow As DataRow In oRet.Rows
                    Dim term As New roTerminal(CInt(dRow("ID")), oState)
                    Terminals.Add(term)
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalList:GetTerminals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalList:GetTerminals")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetTerminalsLive(ByVal strWhere As String, ByRef oState As roTerminalState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM Terminals "
                If strWhere <> "" Then strSQL &= "WHERE " & strWhere & " "
                strSQL &= "ORDER BY Terminals.ID"

                oRet = CreateDataTable(strSQL, "Terminals")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalList:GetTerminals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalList:GetTerminals")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function LoadStatusData(ByVal strWhere As String, ByVal liteCharge As Boolean, ByRef oState As roTerminalState) As DataTable

            Dim bolRet As DataTable = Nothing

            Try
                Dim strSQL As String = ""
                If Not liteCharge Then
                    strSQL = "@SELECT# t.ID, t.Enabled, t.Description, t.Type, ISNULL(t.LastStatus,'') as LastStatus, t.LastUpdate, t.Location, tr.ID as IdReader, tr.Mode as Mode, tr.IDCamera as IDCamera , ISNULL(pCount.PunchCount,0) AS punchCountReader, tName.Name  As LastEmp, ISNULL(t.FirmVersion,'') as FirmVersion, (@SELECT# isnull(count(*),0) from TerminalsSyncTasks where IDTerminal = t.ID) as PendingTasks, ISNULL(t.SerialNumber,'') as SerialNumber" &
                        " from Terminals t full outer join TerminalREaders tr on t.ID = tr.IDTerminal" &
                        " left outer join (@SELECT# IDTerminal, IDReader, count(*) as PunchCount from Punches where ShiftDate = getdate() group by IDTerminal, IDReader) pCount on pcount.IDTerminal = tr.IDTerminal AND pcount.IDReader = tr.ID" &
                        " left outer join (@SELECT# ID, Name from Employees) tName ON tName.ID = (@SELECT# top 1 IDEmployee from punches where IDTerminal = tr.IDTerminal AND IdReader = tr.ID order by ID DESC)"
                Else
                    strSQL = "@SELECT# t.ID, t.Enabled, t.Description, t.Type, ISNULL(t.LastStatus,'') as LastStatus, t.LastUpdate, t.Location, tr.ID as IdReader, tr.Mode as Mode, tr.IDCamera as IDCamera, ISNULL(t.FirmVersion,'') as FirmVersion, (@SELECT# isnull(count(*),0) from TerminalsSyncTasks where IDTerminal = t.ID) as PendingTasks, ISNULL(t.SerialNumber,'') as SerialNumber" &
                        " from Terminals t full outer join TerminalREaders tr on t.ID = tr.IDTerminal"
                End If

                If strWhere <> String.Empty Then
                    strSQL = strSQL & " WHERE " & strWhere
                End If

                strSQL = strSQL & " order by t.Enabled DESC, t.Description ASC, tr.id "

                bolRet = CreateDataTable(strSQL, "Terminals")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalList:LoadStatusData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalList:LoadStatusData")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace