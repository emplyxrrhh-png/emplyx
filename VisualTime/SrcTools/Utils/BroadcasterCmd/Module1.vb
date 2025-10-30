Imports Robotics.Base.VTBroadcasterCore
Imports Robotics.DataLayer

Module Module1

    Sub Main()
        Dim _terminalParam As String
        Dim _idTerminal As Integer
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_RequestGUID", Guid.NewGuid.ToString())

        If My.Application.CommandLineArgs.Count = 1 Then
            _terminalParam = My.Application.CommandLineArgs(0)
            If Integer.TryParse(_terminalParam, _idTerminal) Then
                Dim tbTerminals As DataTable = AccessHelper.CreateDataTable("@select# * from Terminals where id = " & _idTerminal.ToString)
                If Not tbTerminals Is Nothing AndAlso tbTerminals.Rows.Count > 0 Then
                    Console.WriteLine("Generando tareas para terminal " & _idTerminal.ToString & " ...")
                    Dim dStartTime As Date = Now
                    Dim oBroadcasterManager As BroadcasterManager = New BroadcasterManager()
                    oBroadcasterManager.RunBroadcaster(_idTerminal)
                    Dim dElapsedSpan As TimeSpan = Now.Subtract(dStartTime)
                    Dim elapsedTime As String = String.Format("{0} horas {1} minutos {2} segundos", dElapsedSpan.Hours, dElapsedSpan.Minutes, dElapsedSpan.Seconds)
                    Console.WriteLine("... finalizado en " & elapsedTime & " !!!!")
                Else
                    Console.WriteLine("No hay ningún terminal con id " & _idTerminal.ToString & " en la base de datos :-(")
                End If
            Else
                Console.WriteLine(_terminalParam.ToString & " no parece un id de terminal :-(")
            End If
        Else
                If My.Application.CommandLineArgs.Count = 0 Then
                Console.WriteLine("Debes informar el id del terminal")
            Else
                Console.WriteLine("Demasiados parámetros. Sólo acepto el id del terminal a programar")
            End If
        End If
    End Sub

End Module
