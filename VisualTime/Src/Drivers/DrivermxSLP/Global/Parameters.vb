Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class clsParameters

    Private Shared oParameters As roParameters
    Private Shared oSettings As roSettings
    Private Shared oLicense As roServerLicense
    Private Shared _XMLParams As Dictionary(Of String, String) = New Dictionary(Of String, String)
    Private Shared _CardCode As eCardCode = eCardCode.None
    Private Shared _CardType As eCardType = eCardType.None
    Private Shared _AllowTask As Boolean
    Private Shared _JobTimePermissionType As Integer
    Private Shared _EmployeesJobTimePermission As String = ""
    Private Shared _LastLoad As DateTime = Now

    Public Enum eXMLParams
        OutputFilePath
        OutputFileName
        EntriesMode
        CardType
        PrinterMarginLeft
        PrinterMarginTop
    End Enum

    Public Enum eEntriesMode
        DataBase
        File
        DataBaseFile
    End Enum

    Public Enum eCardCode
        None = -1
        Hex = 0
        Numeric = 1
        Robotics = 2
    End Enum

    Public Enum eCardType
        None = -1
        Unique = 1
        Mifare = 2
        HID = 3
        UniqueNumeric = 4
        MifareNumeric = 5
    End Enum

    Private Shared Sub Load()
        Try
            Dim bForce As Boolean = False
            If Now.Subtract(_LastLoad).TotalSeconds > 90 Then
                bForce = True
                _LastLoad = Now
            End If

            If bForce Or oParameters Is Nothing Then
                oParameters = New roParameters("OPTIONS", False)

                ' Obtener parámetros configuración general

                Dim rd As DbDataReader = Nothing
                Try

                    Dim oParams As roCollection = Nothing
                    rd = CreateDataReader("@SELECT# Data FROM sysroParameters WHERE [ID] = 'OPTIONS'")
                    If rd.Read Then
                        If Not IsDBNull(rd("Data")) Then oParams = New roCollection(roTypes.Any2String(rd("Data")).Trim)
                    End If
                    rd.Close()
                    If oParams IsNot Nothing Then
                        _JobTimePermissionType = IIf(Any2Integer(oParams.Item("JobTimePermissionType")) = 0, 3, roTypes.Any2Integer(oParams.Item("JobTimePermissionType")))
                        _EmployeesJobTimePermission = Any2String(oParams.Item("EmployeesJobTimePermission"))
                    End If
                Catch Ex As DbException
                    gLog.logMessage(VTBase.roLog.EventType.roError, "CTerminalMx::LoadParameters: ", Ex)
                Catch Ex As Exception
                    gLog.logMessage(VTBase.roLog.EventType.roError, "CTerminalMx::LoadParameters: ", Ex)
                Finally
                    If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

                End Try
            End If

            If bForce Or oSettings Is Nothing Then
                oSettings = New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
            End If

            If oLicense Is Nothing Then
                oLicense = New roServerLicense
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Shared ReadOnly Property AllowProductiv() As Boolean
        Get
            Load()
            Return oLicense.FeatureIsInstalled("Feature\Productiv")
        End Get
    End Property

    Public Shared ReadOnly Property EntriesMode() As eEntriesMode
        Get
            Select Case getXMLParams(eXMLParams.EntriesMode)
                Case "0"
                    Return eEntriesMode.DataBase
                Case "1"
                    Return eEntriesMode.File
                Case "2"
                    Return eEntriesMode.DataBaseFile
                Case Else
                    Return eEntriesMode.DataBase
            End Select
        End Get
    End Property

    Public Shared ReadOnly Property SavePunchOnFile() As Boolean
        Get
            Return EntriesMode = eEntriesMode.File Or EntriesMode = eEntriesMode.DataBaseFile
        End Get
    End Property

    Public Shared ReadOnly Property EntriesFilePath() As String
        Get
            Return IO.Path.Combine(getXMLParams(eXMLParams.OutputFilePath), getXMLParams(eXMLParams.OutputFileName))
        End Get
    End Property

    Public Shared ReadOnly Property CardType() As eCardType
        Get
            Return _CardType
        End Get
    End Property

    Public Shared ReadOnly Property CardCode() As eCardCode
        Get
            If _CardCode = eCardCode.None Then
                Try
                    Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                    ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                    If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                        _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                    End If
                    Dim sValue As String = ""
                    sValue = Robotics.VTBase.roTypes.Any2Integer(Microsoft.Win32.Registry.GetValue(_RegistryRoot + "Robotics\VisualTime\Process\CommsNET", "CardType", 2))
                    Select Case sValue
                        Case "0", "Hex"
                            _CardCode = eCardCode.Hex
                        Case "1", "Numeric"
                            _CardCode = eCardCode.Numeric
                        Case "2", "Robotics"
                            _CardCode = eCardCode.Robotics
                        Case Else
                            _CardCode = eCardCode.Robotics
                    End Select
                Catch ex As Exception
                    _CardCode = eCardCode.Robotics
                End Try
            End If
            Return _CardCode

        End Get
    End Property

    Public Shared Sub ParseXMLParams(ByVal strParams As String)
        Try
            Dim key As String = ""
            Dim val As String = ""

            strParams = "OutputFileName=;OutputFilePath=;" + strParams.Replace(Chr(10), "").Replace(Chr(13), "")
            For Each param As String In strParams.Split(";")

                If param.Contains("=") Then
                    key = param.Split("=")(0)
                    val = param.Split("=")(1)

                    If _XMLParams.ContainsKey(key) Then _XMLParams.Remove(key)

                    Select Case key.ToLower
                        Case eXMLParams.OutputFilePath.ToString.ToLower
                            If val = "" Then
                                Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                                ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                                If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                                    _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                                End If
                                Dim oSettings As New roSettings(_RegistryRoot & "Robotics\VisualTime")
                                val = oSettings.GetVTSetting(eKeys.Readings)
                            Else
                                If Not IO.Directory.Exists(val) Then
                                    Try
                                        IO.Directory.CreateDirectory(val)
                                    Catch ex As Exception
                                        gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMX7::ParseXMLParams::Error creating directory:", ex)
                                    End Try
                                End If
                            End If
                        Case eXMLParams.OutputFileName.ToString.ToLower
                            If val = "" Then
                                val = "entries.vtr"
                            End If
                        Case eXMLParams.CardType.ToString.ToLower
                            If val = "" Then
                                _CardType = eCardType.Unique
                            Else
                                Select Case val.ToLower
                                    Case "1", "unique"
                                        _CardType = eCardType.Unique
                                    Case "2", "mifare"
                                        _CardType = eCardType.Mifare
                                    Case "3", "hid"
                                        _CardType = eCardType.HID
                                    Case "4", "uniquenumeric"
                                        _CardType = eCardType.UniqueNumeric
                                    Case "5", "mifarenumeric"
                                        _CardType = eCardType.MifareNumeric
                                    Case Else
                                        _CardType = eCardType.Unique
                                End Select
                            End If
                    End Select
                    If Not _XMLParams.ContainsKey(key) Then _XMLParams.Add(key, val)
                End If
            Next
        Catch ex As Exception
            gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMX7::ParseXMLParams::Error:", ex)
        End Try
    End Sub

    Public Shared Function getXMLParams(ByVal Param As String) As String
        Try
            If _XMLParams.ContainsKey(Param) Then
                Return _XMLParams(Param)
            Else
                Return ""
            End If
        Catch ex As Exception
            gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMX7::getXMLParams::Error:", ex)
            Return ""
        End Try
    End Function

    Public Shared Function getXMLParams(ByVal Param As eXMLParams) As String
        Try
            Return getXMLParams(Param.ToString)
        Catch ex As Exception
            gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMX7::getXMLParams2::Error:", ex)
            Return ""
        End Try
    End Function

    Public Shared Function LoadAdvancedParameter(ByVal strParameterName As String, ByRef strParameterValue As String) As Boolean
        ' Esta función está en VTBusiness a partir de la versión 2014
        Dim bolRet As Boolean = False

        Try

            Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters " &
                                   "WHERE [ParameterName] = '" & strParameterName & "'"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                Dim oRow As DataRow = tb.Rows(0)

                If Not IsDBNull(oRow("ParameterName")) Then strParameterName = oRow("ParameterName")
                If Not IsDBNull(oRow("Value")) Then strParameterValue = oRow("Value")
            Else
                strParameterValue = ""
            End If

            bolRet = True
        Catch ex As Data.Common.DbException
            gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::LoadAdvancedParameter::Load::Error:", ex)
        Catch ex As Exception
            gLog.logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::LoadAdvancedParameter::Load::Error:", ex)
        Finally

        End Try

        Return bolRet

    End Function

End Class