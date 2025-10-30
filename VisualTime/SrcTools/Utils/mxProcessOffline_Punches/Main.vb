Module Main

    Sub Main()
        Dim TerminalID As Integer
        Dim lstError As String = ""
        If My.Application.CommandLineArgs.Count > 0 Then
            TerminalID = My.Application.CommandLineArgs(0)
            If TerminalID > 0 Then
                Dim oUBSLogic As Robotics.Comms.DriverMx8p.BusinesProtocol.USBLogicOfflineMX7p = New Robotics.Comms.DriverMx8p.BusinesProtocol.USBLogicOfflineMX7p
                oUBSLogic.ProcessDatabase(TerminalID, lstError)
                oUBSLogic = Nothing
            End If
        End If

    End Sub

End Module
