Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class clsPushProtocol
    ' Constantes para protocolo PUSH ver3.2
    Public Const PUSH_SERVER_VER As String = "3.1.2"
    Public Const PUSH_LIB_VER As String = "3.1.2"

    Public Shared Function IsPunch(_Event As String, ByRef _Action As String) As Boolean
        Select Case Any2Integer(_Event)
            Case 0
                _Action = "AV"
                Return True
            Case 21
                'Ejemplo: Se ha fichado en un instante en que el lector no está habilitado. De momento reportamos como AIT
                _Action = "AIT"
                Return True
            Case 22
                'Ejemplo: Empleado con tarjeta asociada, con algún periodo asignado para la puerta por la que intenta acceder, pero ningún periodo comprende la hora actual
                _Action = "AIT"
                Return True
            Case 23
                'Ejemplo: Empleado con tarjeta asociada, pero sin periodos de acceso asignados
                _Action = "AIR"
                Return True
            Case 27
                'Ejemplo: Tarjeta sin asignar
                _Action = "AIC"
                Return True
            Case Else
                Return False
                ' TODO: Categorizar todos los posibles valores del parámetro event
                '                        protected string GetEventType(string eventType)
                '{
                '    int iEvent = Convert.ToInt32(eventType);
                '    string strType = "";
                '                Switch(iEvent)
                '    {
                '        case 0:
                '            strType = "Success";
                '            break;
                '        case 1:
                '            strType = "Normal open Verify";
                '            break;
                '        case 2:
                '            strType = "First Personnel Open";
                '            break;
                '        case 3:
                '            strType = "Multi Personnel Open";
                '            break;
                '        case 4:
                '            strType = "Super password Open";
                '            break;
                '        case 5:
                '            strType = "Normal Open";
                '            break;
                '        case 6:
                '            strType = "Link event";
                '            break;
                '        case 7:
                '            strType = "Cancel alarm";
                '            break;
                '        case 8:
                '            strType = "Remote open door";
                '            break;
                '        case 9:
                '            strType = "Remote close door";
                '            break;
                '        case 10:
                '            strType = "Cancel NO";
                '            break;
                '        case 11:
                '            strType = "Start NO";
                '            break;
                '        case 12:
                '            strType = "Remote open auxiliary";
                '            break;
                '        case 13:
                '            strType = "Remote close auxiliary";
                '            break;
                '        case 14:
                '            strType = "Open with finger";
                '            break;
                '        case 15:
                '            strType = "Open with multi finger";
                '            break;
                '        case 16:
                '            strType = "Door keep open fp";
                '            break;
                '        case 17:
                '            strType = "Card and FP";
                '            break;
                '        case 18:
                '            strType = "First FP";
                '            break;
                '        case 19:
                '            strType = "First Card and FP";
                '            break;
                '        case 20:
                '            strType = "Interval error";
                '            break;
                '        case 21:
                '            strType = "Sleep door error";
                '            break;
                '        case 22:
                '            strType = "Illege time";
                '            break;
                '        case 23:
                '            strType = "Access Denied";
                '            break;
                '        case 24:
                '            strType = "Antipassback";
                '            break;
                '        case 25:
                '            strType = "Multi lock lingate";
                '            break;
                '        case 26:
                '            strType = "Multi card wait";
                '            break;
                '        case 27:
                '            strType = "Verified Failed";
                '            break;
                '        case 28:
                '            strType = "Door contact overtime";
                '            break;
                '        case 29:
                '            strType = "Card overtime";
                '            break;
                '        case 30:
                '            strType = "Password error";
                '            break;
                '        case 31:
                '            strType = "Fp interval error";
                '            break;
                '        case 32:
                '            strType = "Multi fp wait";
                '            break;
                '        case 33:
                '            strType = "Fp overtime";
                '            break;
                '        case 34:
                '            strType = "Unknown Finger";
                '            break;
                '        case 35:
                '            strType = "FP Overtime";
                '            break;
                '        case 36:
                '            strType = "Button overtime";
                '            break;
                '        case 37:
                '            strType = "Keep open error";
                '            break;
                '        case 38:
                '            strType = "Loss card";
                '            break;
                '        case 39:
                '            strType = "Blacklist";
                '            break;
                '        case 40:
                '            strType = "Multi finger failed";
                '            break;
                '        case 41:
                '            strType = "verify type failed";
                '            break;
                '        case 42:
                '            strType = "wiegand error";
                '            break;
                '        case 43:
                '            strType = "reserved 43";
                '            break;
                '        case 44:
                '            strType = "Remote verify failed";
                '            break;
                '        case 45:
                '            strType = "Remote verify timeout";
                '            break;
                '        case 46:
                '            strType = "reserved 46";
                '            break;
                '        case 47:
                '            strType = "Open door failed";
                '            break;
                '        case 48:
                '            strType = "Multi personnel failed";
                '            break;
                '        case 100:
                '            strType = "Antistrip alarm";
                '            break;
                '        case 101:
                '            strType = "Emergency password alarm";
                '            break;
                '        case 102:
                '            strType = "Unexcept door open";
                '            break;
                '        case 103:
                '            strType = "Emergency fp alarm";
                '            break;
                '        case 104:
                '            strType = "Five failed card punch";
                '            break;
                '        case 105:
                '            strType = "Device offline";
                '            break;
                '        case 200:
                '            strType = "Door open";
                '            break;
                '        case 201:
                '            strType = "Door close";
                '            break;
                '        case 202:
                '            strType = "Button open door";
                '            break;
                '        case 203:
                '            strType = "Multi card and fp";
                '            break;
                '        case 204:
                '            strType = "Keep open end";
                '            break;
                '        case 205:
                '            strType = "Remote keep open";
                '            break;
                '        case 206:
                '            strType = "Device Start";
                '            break;
                '        case 207:
                '            strType = "Password open door";
                '            break;
                '        case 208:
                '            strType = "Super user open door";
                '            break;
                '        case 209:
                '            strType = "Door locked";
                '            break;
                '        case 210:
                '            strType = "Fire event";
                '            break;
                '        case 211:
                '            strType = "Super user closed";
                '            break;
                '        case 212:
                '            strType = "Multi card OK";
                '            break;
                '        case 213:
                '            strType = "First card open";
                '            break;
                '        case 214:
                '            strType = "Device online";
                '            break;
                '        case 220:
                '            strType = "Auxin open";
                '            break;
                '        case 221:
                '            strType = "Auxin close";
                '            break;
                '        case 222:
                '            strType = "Remote verify success";
                '            break;
                '        case 223:
                '            strType = "Remote verify";
                '            break;
                '        case 225:
                '            strType = "Auxin normal";
                '            break;
                '        case 226:
                '            strType = "Auxin trigger";
                '            break;
                '        case 227:
                '            strType = "Double open";
                '            break;
                '        case 228:
                '            strType = "Double close";
                '            break;
                '        default:
                '            strType = "unknown error:" + eventType;
                '            break;

                '    }
                '    return strType;
                '}
        End Select
    End Function

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
            Return ((iYear - 2000) * 12 * 31 + (iMonth - 1) * 31 + (iDay - 1)) * 24 * 60 * 60 + (iHour * 60 + iMinute) * 60 + iSecond
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

    ' Parámetros de configuración de centralita
    Public Enum dataTerminalConfigParameters
        IPAddress
        GATEIPAddress
        NetMask
        AntiPassback
        Interlock
        Door1ForcePassWord
        Door2ForcePassWord
        Door3ForcePassWord
        Door4ForcePassWord
        Door1SuperPassWord
        Door2SuperPassWord
        Door3SuperPassWord
        Door4SuperPassWord
        Door1CloseAndLock
        Door2CloseAndLock
        Door3CloseAndLock
        Door4CloseAndLock
        Door1SensorType
        Door2SensorType
        Door3SensorType
        Door4SensorType
        Door1Drivertime
        Door2Drivertime
        Door3Drivertime
        Door4Drivertime
        Door1Detectortime
        Door2Detectortime
        Door3Detectortime
        Door4Detectortime
        Door1VerifyType
        Door2VerifyType
        Door3VerifyType
        Door4VerifyType
        Door1MultiCardOpenDoor
        Door2MultiCardOpenDoor
        Door3MultiCardOpenDoor
        Door4MultiCardOpenDoor
        Door1FirstCardOpenDoor
        Door2FirstCardOpenDoor
        Door3FirstCardOpenDoor
        Door4FirstCardOpenDoor
        Door1ValidTZ
        Door2ValidTZ
        Door3ValidTZ
        Door4ValidTZ
        Door1KeepOpenTimeZone
        Door2KeepOpenTimeZone
        Door3KeepOpenTimeZone
        Door4KeepOpenTimeZone
        Door1Intertime
        Door2Intertime
        Door3Intertime
        Door4Intertime
        WatchDog
        Door4ToDoor2
        BackupTime
        Reboot
        DateTime
        Unknown
        DNSAddress
        WebServerURL
        IsSupportDNS
        WebServerIP
        WebServerPort
    End Enum

    ' Validación de Token
    Public Shared Function GetCommunicationToken(sRegistryCode As String, sSN As String, sOther As String) As String
        Try
            Dim md5 As ZK.MD5.MD5Managed
            md5 = New ZK.MD5.MD5Managed
            md5.Initialize()
            md5.HashCoreEx(System.Text.Encoding.ASCII.GetBytes(sRegistryCode), 0, sRegistryCode.Length)
            md5.HashCoreEx(System.Text.Encoding.ASCII.GetBytes(sSN), 0, sSN.Length)
            md5.HashCoreEx(System.Text.Encoding.ASCII.GetBytes(sOther), 0, sOther.Length)
            md5.HashFinalEx()
            Dim s As New System.Text.StringBuilder()
            For Each b As Byte In md5.Hash
                s.Append(b.ToString("x2").ToLower())
            Next
            Return s.ToString()
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
End Class