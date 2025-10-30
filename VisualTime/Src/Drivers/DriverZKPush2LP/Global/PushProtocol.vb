Imports Robotics.VTBase

Public Class clsPushProtocol
    ' Constantes para protocolo PUSH ver3.2
    Public Const PUSH_SERVER_VER As String = "2.0.1"
    Public Const PUSH_LIB_VER As String = "2.0.0"

    Public Shared Function FormatTimeZoneSlot(dDate1 As Date, date2 As Date) As Integer
        Try
            Return (100 * dDate1.Hour + dDate1.Minute) * 65536 + (100 * date2.Hour + date2.Minute)
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Shared Function ConvertDateTime(dDateTime As Date) As Integer
        Try
            Dim iYear As Integer = dDateTime.Year
            Dim iMonth As Byte = dDateTime.Month
            Dim iDay As Byte = dDateTime.Day
            Dim iHour As Byte = dDateTime.Hour
            Dim iMinute As Byte = dDateTime.Minute
            Dim iSecond As Byte = dDateTime.Second
            Return ((iYear - 2000) * 12 * 31 + (iMonth - 1) * 31 + (iDay - 1)) * 24 * 60 * 60 + iHour * 60 * 60 + iMinute * 60 + iSecond
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Shared Function ZKDateTimeToString(iNum As Integer) As String
        Try
            Dim iSecond As Integer = iNum Mod 60
            Dim iMinute As Integer = ((iNum - iSecond) / 60) Mod 60
            Dim iHour As Integer = ((iNum - iSecond - iMinute) / 3600) Mod 24
            Dim iDay As Integer = (((iNum - iSecond - iMinute - iHour) / 86400) Mod 31)
            Dim iMonth As Integer = (((iNum - iSecond - iMinute - iHour - iDay) / 2678400) Mod 12)
            Dim iYear As Integer = (iNum - iSecond - iMinute - iHour - iDay - iMonth) / 32140800 + 2000

            Return New DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond).ToString
        Catch ex As Exception
            Return "error"
        End Try
    End Function

    Public Shared Function ConvertCard(sCard As String) As String
        Try
            Dim sCardAsHex As String
            Dim res As String = ""
            ' Paso a hexa
            sCardAsHex = Hex(sCard)
            ' Aseguro que tiene un número par de caracteres. Sino, completo con un 0 por la izqda.
            If sCardAsHex.Length Mod 2 <> 0 Then
                sCardAsHex = sCardAsHex.PadLeft(sCardAsHex.Length + 1, "0")
            End If

            ' Giro de izqda a drcha, de byte en bytes (2 caracteres)
            For i As Integer = (sCardAsHex.Length / 2) To 1 Step -1
                res = res + sCardAsHex.Substring(2 * i - 2, 2)
            Next

            res = "[" + res.PadRight(10, "0") + "]"
            Return res
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "clsPushProtocol::ConvertCard:Error: ", ex)
            Return ""
        End Try
    End Function

    ' Tipos de fichaje (no todos disponibles en firmware EU Robotics)
    Public Enum dataPunchStatus
        ClockIn = 0
        ClockOut = 1
        Out = 2
        ReturnFromOut = 3
        ClockInForOvertime = 4
        ClockOutForOvertime = 4
        DeniedInvalidTime = 7
        MealStart = 8
        MealEnd = 9
    End Enum

    Public Enum dataOperations
        PowerOn = 0
        PowerOff = 1
        VerificationFail = 2
        Alarm = 3
        EnterMenu = 4
        ChangeSetting = 5
        AddFinger = 6
        AddPassword = 7
        AddCard = 8
        DelUser = 9
        DelFinger = 10
        DelPassword = 11
        DelCard = 12
        PurgeData = 13
        CreateMFCard = 14
        EnrollMFCard = 15
        RegisterMFCard = 16
        DeleteMFCard = 17
        CleanMFCardContents = 18
        MoveRegistrationDataToCard = 19
        CopyCardDataToAttendanceMachine = 20
        SetTime = 21
        FactorySetting = 22
        DeleteAttRecords = 23
        CleanAdministrationPrivilege = 24
        ChgAccControl = 25
        ChgUserControl = 26
        ChgTimeControl = 27
        ChgUnlocking = 28
        PerformUnlocking = 29
        AddUser = 30
        ModifyFingerPrintAttribute = 31
        DuressAlarm = 32
        DoorBellCall = 33
        AntiPassback = 34
        DeleteAttendancePhoto = 35
        ModifyOtherUserInformation = 36
        Holidays = 37
        RestoreData = 38
        BackupData = 39
        UDiskUpload = 40
        UDiskDownload = 41
        UDiskAttendanceRecordEncryption = 42
        DeleteRecordsAfterDownload = 43
        ExitButton = 53
        DoorSensorClosed = 54
        AlarmEx = 55
        RecoveryParamenters = 56
        RegisterUserPhoto = 68
        ModifyUserName = 70
        ModifyUserPermissions = 71
        ModifyNetworkIP = 76
        ModifyNetworkMask = 77
        ModifyNetworkGateway = 78
        ModifyNetworkDNS = 79
        ModifyConnectionPwd = 80
        ModifyConnectionSettingsID = 81
        ModifyCloudServerAddress = 82
        ModifyCloudServerPort = 83
        ModifyAccessControlRecordSetting = 87
        ModifyFaceParameterIcon = 88
        ModifyFingerprintParameterIcon = 89
        ModifyFingerVeinParameterIcon = 90
        ModifyPalmprintParameterIcon = 91
        UDiskUpgradeIcon = 92
        ModifyRFCardInformation = 100
        EnrollFace = 101
        ModifyPersonnelPermissions = 102
        DeletePersonnelPermissions = 103
        AddPersonelPermissions = 104
        DeleteAccessControlRecords = 105
        DeleteFace = 106
        DeletePersonnelPhoto = 107
        ModifyParameters = 108
        SelectWifiSSID = 109
        ProxEnable = 110
        ProxyIPModification = 111
        ProxyPortModification = 112
        ChangePersonnelPwd = 113
        ModifyFaceInformation = 114
        ChageOperatorPwd = 115
        RestoreAccessControlSettings = 116
        BadOperatorPwdEntered = 117
        OperatorPwdLock = 118
        ModifyLegicCardLength = 120
        RegisterFingerVein = 121
        ModifyFingerVein = 122
        DeleteFingerVein = 123
        RegisterPalmprint = 124
        ModifyPalmprint = 125
        DeletePalmprint = 126
    End Enum

    Public Enum dataAlarmCodes
        Door_Closed_Detected = 50
        Door_Open_Detected = 51
        Terminal_Dismounted = 55
        Out_Button_Pressed = 53
        Accidental_Open_Door = 54
        Invalid_Verification = 58
        Alarm_Cancelled = 65535
    End Enum

    ' Parámetros de configuración de centralita
    Public Enum dataTerminalConfigParameters
        TransactionCount 'The number of current attendance logs
        FPCount 'The number of registered fingerprints
        UserCount 'The number of registered users
        FWVersion 'Firmware version number
        IPAddress 'IP address of the equipment
        NetMask 'Subnet mask of the equipment
        GATEIPAddress 'Gateway address of the equipment
        VOLUME 'Volume
        MAC 'Ethernet MAC address of the equipment.
        CardKey 'Mifare card encryption key
        DeviceID 'Equipment ID number
        LockOn 'Unlocking duration
        AlarmAttLog 'Attendance log alarm
        AlarmReRec 'Minimum repetitive attendance recording interval
        RS232BaudRate 'RS232/RS485 baud rate
        AutoPowerOff 'Automatic power-off time. Format: hour × 256 + minute. The following time setting items all adopt this format.
        AutoPowerOn 'Automatic power-on time
        AutoPowerSuspend 'Automatic standby time
        AutoAlarm1 'to AutoAlarm50 50 automatic timing alarms
        IdlePower 'Idle setting
        IdleMinute 'Idle duration (minute)
        RS232On 'Whether to enable the RS232 connection
        RS485On 'Whether to enable the RS485 connection
        UnlockPerson 'The number of users unlocking the door
        OnlyPINCard 'Only read the Mifare card ID number.
        HiSpeedNet 'Network rate
        Must1To1 'Whether to allow only 1:1 fingerprint matching
        ODD 'Unlock time. If the door still remains open over the due time, an alarm will be generated.
        DSM
        AADelay
        DUHK 'Whether to enable the emergency call function in the case of duress alarm
        DU11 'Duress alarm generated at 1:1 fingerprint matching
        DU1N 'Duress alarm generated at 1:N fingerprint matching
        DUPWD 'Duress alarm generated at password verification
        DUAD 'Duress alarm latency (second)
        LockPWRButton 'Lock the power-off button
        SUN 'Whether to send a broadcast message at equipment power-on to help other computers on the same network find the current equipment
        I1NFrom 'Set a start user number in 1:N fingerprint matching mode
        I1NTo 'Set an end user number in 1:N fingerprint matching mode
        I1H 'Whether to enable the 1:H function
        I1G 'Whether to enable the 1:G function
        KeyPadBeep 'Whether to beep with every keystroke
        WorkCode 'Whether to enable theWorkCode function
        AAVOLUME 'Alarm volume
        DHCP 'Whether to enable the DHCP function
        EnableProxyServer 'Whether to enable the HTTP proxy server
        ProxyServerIP 'IP address of the HTTP proxy server
        ProxyServerPort 'Port of the HTTP proxy server
        PrinterOn 'Whether to enable the printer
        DefaultGroup 'Default group number
        GroupFpLimit 'Limit of the number of fingerprints in each group
        WIFI 'Whether to enableWiFi
        wifidhcp 'Whether to enable the DHCP function of WiFi
        AmPmFormatFunOn 'Whether to display AM/PM on the main interface
        AntiPassbackOn 'Whether to enable the anti-passback function
        MasterSlaveOn 'Whether to enable the master/slave function
        ImeFunOn 'Whether to enable the T9 input method
        WebServerIP 'IP address of the PUSH SDK Web server
        WebServerPort 'Port number of the PUSH SDK Web server
        ApiPort 'Port number of DataAPI SDK
        DelRecord 'The number of history attendance records automatically deleted when the total number of records exceeds the maximum limit.
        Unknown
        AutoOpenRelay
        AutoOpenRelayTimes  'Tiempo de sirena
        FPOpenRelay
        WiegandOpenDoor
        DNS
        WebServerURLModel
        ICLOCKSVRURL
        isSupportAlarmExt
        ExAlarmPort
        FakeFingerFunOn
        IRTempThreshold
        IRTempLowThreshold
        EnalbeIRTempDetection
        EnableNormalIRTempPass
        EnalbeMaskDetection
        EnableWearMaskPass
        BiometricVersion
        FaceCount
        PvCount
        WifiOn
        CMD
        ID
        FILENAME
        Content
    End Enum

End Class

Public Class clsSecurityOptions
    Private _IRTempThreshold As Integer = -1
    Private _IRTempLowThreshold As Integer = -1
    Private _EnalbeIRTempDetection As Integer = -1
    Private _EnableNormalIRTempPass As Integer = -1
    Private _EnalbeMaskDetection As Integer = -1
    Private _EnableWearMaskPass As Integer = -1
    Private _BiometricVersion As String = String.Empty

    Public Property IRTempThreshold As Integer
        Get
            Return _IRTempThreshold
        End Get
        Set(value As Integer)
            _IRTempThreshold = value
        End Set
    End Property

    Public Property IRTempLowThreshold As Integer
        Get
            Return _IRTempLowThreshold
        End Get
        Set(value As Integer)
            _IRTempLowThreshold = value
        End Set
    End Property

    Public Property EnalbeIRTempDetection As Integer
        Get
            Return _EnalbeIRTempDetection
        End Get
        Set(value As Integer)
            _EnalbeIRTempDetection = value
        End Set
    End Property

    Public Property EnableNormalIRTempPass As Integer
        Get
            Return _EnableNormalIRTempPass
        End Get
        Set(value As Integer)
            _EnableNormalIRTempPass = value
        End Set
    End Property

    Public Property EnalbeMaskDetection As Integer
        Get
            Return _EnalbeMaskDetection
        End Get
        Set(value As Integer)
            _EnalbeMaskDetection = value
        End Set
    End Property
    Public Property EnableWearMaskPass As Integer
        Get
            Return _EnableWearMaskPass
        End Get
        Set(value As Integer)
            _EnableWearMaskPass = value
        End Set
    End Property

    Public Property BiometricVersion As String
        Get
            Return _BiometricVersion
        End Get
        Set(value As String)
            _BiometricVersion = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return "EnableIRTempDetection: " & EnalbeIRTempDetection.ToString & "IRTempThreshold: " & IRTempThreshold.ToString & "IRTempLowThreshold: " & IRTempLowThreshold.ToString & "EnableNormalIRTempPass: " & ChineseToRestOfTheWorld(EnableNormalIRTempPass.ToString) & "EnableMaskDetection: " & EnalbeMaskDetection.ToString & "EnableWearMaskPass: " & ChineseToRestOfTheWorld(EnableWearMaskPass.ToString) & "BiometricVersion: " & BiometricVersion.Replace(":", "-")
    End Function

    Private Function ChineseToRestOfTheWorld(strValue As String) As String
        If strValue = "0" Then
            Return "true"
        Else
            Return "false"
        End If
    End Function

End Class