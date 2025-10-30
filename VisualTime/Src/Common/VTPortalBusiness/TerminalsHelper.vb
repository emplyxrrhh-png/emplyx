Imports System.Security
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal
    Public Class TerminalsHelper
        Public Shared Function EnableTimegate(ByVal serialNumber As String, ByVal name As String, ByVal apkVersion As String, ByVal terminalstate As roTerminalState) As roGenericResponse(Of Timegate)
            Dim tInfo As New roGenericResponse(Of Timegate)

            Try
                tInfo.Status = ErrorCodes.GENERAL_ERROR

                Dim systemState As New roSecurityState()
                Dim langKey As String = systemState.GetLanguageKey()

                Dim terminal As Terminal.roTerminal = New Terminal.roTerminal()
                terminal.SerialNumber = serialNumber
                terminal.FirmVersion = apkVersion
                If name IsNot Nothing AndAlso name <> "" Then
                    terminal.Description = name
                Else
                    terminal.Description = "Dispositivo portal compartido"
                End If
                terminal.Type = "Time Gate"
                terminal.SupportedModes = "TA,EIP"
                terminal.CustomDuration = 10
                terminal.Location = terminalstate.ClientAddress.Split("#")(0).Split(":")(0)
                Dim reader As VTBusiness.Terminal.roTerminal.roTerminalReader = New VTBusiness.Terminal.roTerminal.roTerminalReader()
                reader.Description = "Reader 1"
                reader.ID = 1
                reader.Mode = "EIP"
                reader.ValidationMode = ValidationMode.Server
                terminal.Readers.Add(reader)

                Dim idMode As Integer = CInt(FieldTypes.tNumeric)
                Dim oParameterCustomFieldID As New AdvancedParameter.roAdvancedParameter("Timegate.Identification.CustomUserFieldId", New AdvancedParameter.roAdvancedParameterState)
                Dim timegateIdMode As String = roTypes.Any2String(oParameterCustomFieldID.Value)
                If timegateIdMode <> String.Empty Then
                    Dim timegateConf As TimegateConfiguration = roJSONHelper.DeserializeNewtonSoft(timegateIdMode, GetType(TimegateConfiguration))

                    If timegateConf IsNot Nothing AndAlso timegateConf.CustomUserFieldEnabled Then
                        Dim idUserFieldUsed As Integer = roUserField.GetUserFieldType(timegateConf.UserFieldId, New roUserFieldState())
                        If idUserFieldUsed > -1 Then idMode = idUserFieldUsed
                    End If
                End If

                Dim oParameter As New AdvancedParameter.roAdvancedParameter("Timegate.Configuration.Background", New AdvancedParameter.roAdvancedParameterState)
                Dim timeGateMD5Background = ""
                If oParameter.Value IsNot Nothing AndAlso oParameter.Value <> "" Then
                    timeGateMD5Background = CryptographyHelper.EncryptWithMD5(oParameter.Value)
                End If

                If terminal.Save(True) Then
                    tInfo.Status = ErrorCodes.OK
                    tInfo.Value = New Timegate With {
                        .Id = terminal.ID,
                        .Name = terminal.Description,
                        .Firmware = terminal.FirmVersion,
                        .SerialNumber = terminal.SerialNumber,
                        .ScreenTimeout = terminal.CustomDuration,
                        .LastConnection = If(terminal.LastAction.HasValue, terminal.LastAction, roTypes.CreateDateTime(1970, 1, 1)),
                        .Behaviour = terminal.Readers(0).Mode,
                        .InZone = roTypes.Any2Integer(terminal.Readers(0).IdZone),
                        .OutZone = roTypes.Any2Integer(terminal.Readers(0).IdZoneOut),
                        .Language = langKey,
                        .IDmode = idMode,
                        .BackgroundMD5 = timeGateMD5Background
                    }
                End If



            Catch ex As Exception
                tInfo.Value = Nothing
                tInfo.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TerminalsHelper::EnableTimeGate")
            End Try

            Return tInfo
        End Function

        Public Shared Function GetTimeGateBackgroundConfiguration(ByVal serialNumber As String, ByVal terminalstate As roTerminalState) As roGenericResponse(Of String)
            Dim ret As roGenericResponse(Of String) = New roGenericResponse(Of String)
            Try
                Dim oParameter As New AdvancedParameter.roAdvancedParameter("Timegate.Configuration.Background", New AdvancedParameter.roAdvancedParameterState)
                Dim timeGateBackground = ""
                If oParameter.Value IsNot Nothing AndAlso oParameter.Value <> "" Then
                    timeGateBackground = roTypes.Any2String(oParameter.Value)
                End If


                ret.Status = ErrorCodes.OK
                ret.Value = timeGateBackground


            Catch ex As Exception
                ret.Value = Nothing
                ret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TerminalsHelper::GetTimeGateBackgroundConfiguration")
            End Try

            Return ret
        End Function

        Public Shared Function GetTimeGateConfiguration(ByVal serialNumber As String, ByVal terminalstate As roTerminalState) As roGenericResponse(Of Timegate)
            Dim ret As roGenericResponse(Of Timegate) = New roGenericResponse(Of Timegate)
            Try
                Dim oTerminal As roTerminal = roTerminal.GetTerminalBySerialNumber(serialNumber, terminalstate)
                Dim oParameter As New AdvancedParameter.roAdvancedParameter("Timegate.Configuration.Background", New AdvancedParameter.roAdvancedParameterState)
                Dim timeGateMD5Background = ""
                If oParameter.Value IsNot Nothing AndAlso oParameter.Value <> "" Then
                    timeGateMD5Background = CryptographyHelper.EncryptWithMD5(oParameter.Value)
                End If

                If oTerminal IsNot Nothing Then
                    Dim systemState As New roSecurityState()
                    Dim langKey As String = systemState.GetLanguageKey()


                    Dim idMode As Integer = CInt(FieldTypes.tNumeric)
                    Dim oParameterCustomFieldID As New AdvancedParameter.roAdvancedParameter("Timegate.Identification.CustomUserFieldId", New AdvancedParameter.roAdvancedParameterState)
                    Dim timegateIdMode As String = roTypes.Any2String(oParameterCustomFieldID.Value)
                    If timegateIdMode <> String.Empty Then
                        Dim timegateConf As TimegateConfiguration = roJSONHelper.DeserializeNewtonSoft(timegateIdMode, GetType(TimegateConfiguration))

                        If timegateConf IsNot Nothing AndAlso timegateConf.CustomUserFieldEnabled Then
                            Dim idUserFieldUsed As Integer = roUserField.GetUserFieldType(timegateConf.UserFieldId, New roUserFieldState())
                            If idUserFieldUsed > -1 Then idMode = idUserFieldUsed
                        End If
                    End If

                    ret.Status = ErrorCodes.OK
                    ret.Value = New Timegate With {
                        .Id = oTerminal.ID,
                        .Name = oTerminal.Description,
                        .Firmware = oTerminal.FirmVersion,
                        .SerialNumber = oTerminal.SerialNumber,
                        .ScreenTimeout = oTerminal.CustomDuration,
                        .LastConnection = If(oTerminal.LastAction.HasValue, oTerminal.LastAction, roTypes.CreateDateTime(1970, 1, 1)),
                        .Behaviour = oTerminal.Readers(0).Mode,
                        .InZone = roTypes.Any2Integer(oTerminal.Readers(0).IdZone),
                        .OutZone = roTypes.Any2Integer(oTerminal.Readers(0).IdZoneOut),
                        .Language = langKey,
                        .IDmode = idMode,
                        .BackgroundMD5 = timeGateMD5Background
                    }

                Else
                    ret.Value = Nothing
                    ret.Status = ErrorCodes.NOT_FOUND
                End If

            Catch ex As Exception
                ret.Value = Nothing
                ret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TerminalsHelper::GetTimeGateConfiguration")
            End Try

            Return ret
        End Function
        Public Shared Function DisableTimegate(ByVal serialNumber As String) As StdResponse
            Dim bDeleted As New StdResponse

            Try

                bDeleted.Result = True
                bDeleted.Status = ErrorCodes.OK
                Dim oTerminal As roTerminal = New roTerminal
                Dim terminalState As New roTerminalState()
                oTerminal = Terminal.roTerminal.GetTerminalBySerialNumber(serialNumber, terminalState)
                bDeleted.Result = oTerminal.Delete()

            Catch ex As Exception
                bDeleted.Result = False
                bDeleted.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TerminalsHelper::DisableSharedPortal")
            End Try

            Return bDeleted
        End Function

        Public Shared Function SetTimeGateStatus(ByVal serialNumber As String, ByVal appVersion As String, ByVal terminalstate As roTerminalState) As Boolean
            Dim ret As Boolean = False
            Try
                Dim terminal As roTerminal = roTerminal.GetTerminalBySerialNumber(serialNumber, terminalstate)
                If terminal IsNot Nothing Then
                    terminal.LastAction = DateTime.Now.ToString()
                    terminal.FirmVersion = appVersion
                    terminal.UpdateStatus(True)
                    ret = terminal.Save()
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TerminalsHelper::SetTimeGateStatus")
            End Try

            Return ret
        End Function
    End Class
End Namespace
