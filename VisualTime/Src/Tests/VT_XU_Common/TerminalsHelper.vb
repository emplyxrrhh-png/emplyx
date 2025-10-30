Imports Robotics.Base.VTBusiness.Terminal

Public Class TerminalsHelper
    Public Property setTimeGateStatusCalled As Boolean = False
    Public Property updatedLastAction As Date = Nothing
    Public Property updatedApkVersion As String = ""
    Public Property statusUpdated As Boolean = False

    Public Function GetTimeGateConfiguration(ByVal isOK As Boolean)
        Robotics.Base.VTPortal.VTPortal.Fakes.ShimTerminalsHelper.GetTimeGateConfigurationStringroTerminalState = Function(serialNumber As String, terminalState As roTerminalState) As roGenericResponse(Of Robotics.Base.DTOs.Timegate)
                                                                                                                      If isOK Then
                                                                                                                          Dim response As New roGenericResponse(Of Robotics.Base.DTOs.Timegate)
                                                                                                                          response.Status = 0
                                                                                                                          response.Value = New Robotics.Base.DTOs.Timegate
                                                                                                                          response.Value.Id = 1
                                                                                                                          response.Value.Name = "TimeGate"
                                                                                                                          response.Value.Firmware = "1.0"
                                                                                                                          response.Value.SerialNumber = "123456"
                                                                                                                          response.Value.ScreenTimeout = 10
                                                                                                                          response.Value.LastConnection = Now.Date
                                                                                                                          response.Value.Behaviour = "EIP"
                                                                                                                          Return response
                                                                                                                      Else
                                                                                                                          Dim response As New roGenericResponse(Of Robotics.Base.DTOs.Timegate)
                                                                                                                          response.Status = 1
                                                                                                                          Return response
                                                                                                                      End If
                                                                                                                  End Function
    End Function

    Public Function SetTimeGateStatus()
        Robotics.Base.VTPortal.VTPortal.Fakes.ShimTerminalsHelper.SetTimeGateStatusStringStringroTerminalState = Function(serialNumber As String, status As String, terminalState As roTerminalState) As Boolean
                                                                                                                     setTimeGateStatusCalled = True
                                                                                                                     Return True
                                                                                                                 End Function
    End Function

    Public Function GetTerminalBySerialNumber()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.GetTerminalBySerialNumberStringroTerminalState = Function(serialNumber As String, terminalState As roTerminalState) As roTerminal
                                                                                                                    Dim terminal As roTerminal = New roTerminal
                                                                                                                    terminal.ID = 1
                                                                                                                    terminal.Description = "TimeGate"
                                                                                                                    terminal.FirmVersion = "1.0"
                                                                                                                    terminal.SerialNumber = "123456"
                                                                                                                    terminal.CustomDuration = 10
                                                                                                                    terminal.LastAction = New Date(2024, 10, 10)
                                                                                                                    Return New roTerminal
                                                                                                                End Function

    End Function

    Public Function Save()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.AllInstances.SaveBoolean = Function(instance As roTerminal, saved As Boolean) As Boolean
                                                                                              'updatedLastAction = instance.LastAction
                                                                                              Return True
                                                                                          End Function
    End Function
    Public Function PutValues()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.AllInstances.PutValueStringObject = Function(instance As roTerminal, key As String, value As Object) As Boolean
                                                                                                       If key = "LastAction" Then
                                                                                                           updatedLastAction = value
                                                                                                       Else
                                                                                                           If key = "FirmVersion" Then
                                                                                                               updatedApkVersion = value
                                                                                                           End If
                                                                                                       End If
                                                                                                       Return True
                                                                                                   End Function
    End Function

    Public Function ReturnFields()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.AllInstances.ReturnFieldString = Function(instance As roTerminal, key As String) As Object
                                                                                                    Return Nothing
                                                                                                End Function
    End Function
    Public Function Load()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.AllInstances.LoadBooleanBooleanBoolean = Function(instance As roTerminal, loaded As Boolean, loadRelations As Boolean, loadSyncTasks As Boolean) As Boolean
                                                                                                            Return True
                                                                                                        End Function
    End Function

    Public Function UpdateStatus()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.AllInstances.UpdateStatusBooleanBoolean = Function(instance As roTerminal, updated As Boolean, updateSyncTasks As Boolean) As Boolean
                                                                                                             statusUpdated = True
                                                                                                             Return True
                                                                                                         End Function
    End Function

    Public Function GetCurrentTerminalDatetime()
        Robotics.Base.VTPortal.VTPortal.Fakes.ShimStatusHelper.GetCurrentTerminalDatetimeroTerminalTimeZoneInfo = Function(terminal As roTerminal, timeZone As TimeZoneInfo) As Date
                                                                                                                      Return Now

                                                                                                                  End Function

    End Function

    Public Function GetEmployeeTerminals()
        Robotics.Base.VTBusiness.Terminal.Fakes.ShimroTerminal.GetEmployeeTerminalsInt32StringroTerminalState = Function(idEmployee As Integer, terminalType As String, terminalState As roTerminalState) As Generic.List(Of roTerminal)
                                                                                                                    Dim terminals As New Generic.List(Of roTerminal)
                                                                                                                    Dim terminal As New roTerminal
                                                                                                                    terminal.ID = 1
                                                                                                                    terminal.Description = "TimeGate"
                                                                                                                    terminal.FirmVersion = "1.0"
                                                                                                                    terminal.SerialNumber = "123456"
                                                                                                                    terminal.CustomDuration = 10
                                                                                                                    terminal.LastAction = New Date(2024, 10, 10)
                                                                                                                    terminals.Add(terminal)
                                                                                                                    Return terminals
                                                                                                                End Function
    End Function

End Class

