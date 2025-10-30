Public Class VTSInitTask

    Private mLastError As String

#Region " Connect "
    Public Function ConnectToServer(Optional ByRef pstrWorkingStationName As String = "") As Object
        Dim vtdConv As Object
        Dim mRoRegistry As Object
        Dim ServerStationName As String = String.Empty
        Dim mRoSupport As Object

        Dim rObject As Object = Nothing

        Try
            Const HKLM = &H80000002
            Const SERVER_LOCAL = "(LOCAL)"
            Const VTSERVER_CLASS = "VisualTimeServer.Server"

            mLastError = "CreateObject(roConversions)" + vbNewLine

            Try
                ' Obtiene equipo servidor
                vtdConv = CreateObject("VTDTypes.roConversions")
                mRoRegistry = CreateObject("VTMain.roRegistry")
                ServerStationName = vtdConv.Any2String(mRoRegistry.RegValue(HKLM, "SOFTWARE\Robotics\VisualTime\Server", "Server"))
            Catch ex As Exception
                mLastError += "Error creating roConversions (" & ex.Message & ") " + vbNewLine
            End Try

            Try
                mLastError += "CreateObject(roSupport)" + vbNewLine
                ' Traduce servidores locales
                mRoSupport = CreateObject("VTMain.roSupport")
                pstrWorkingStationName = mRoSupport.GetWorkstationName()
                If UCase$(ServerStationName) = SERVER_LOCAL Or UCase$(ServerStationName) = UCase$(pstrWorkingStationName) Then ServerStationName = ""

            Catch ex As Exception
                mLastError += "Error creating roSupport (" & ex.Message & ") " + vbNewLine
            End Try

            Try
                mLastError += "CreateObject(VTSERVER)[" + ServerStationName + "]" + vbNewLine
                'comprobamos si tenemos conexión al Servidor de Visual Time.
                If ServerStationName = "" Then
                    rObject = CreateObject(VTSERVER_CLASS)
                Else
                    rObject = CreateObject(VTSERVER_CLASS, ServerStationName)
                End If
            Catch ex As Exception
                mLastError += "Error creating VTServer (" & ex.Message & ") " + vbNewLine
            End Try

        Catch ex As Exception
            rObject = Nothing
        Finally
            mRoSupport = Nothing
            mRoRegistry = Nothing
            vtdConv = Nothing
        End Try

        Return rObject
    End Function

    Private Function ConnectarTot(ByRef pobjServer As Object, ByRef pobjSession As Object) As Object
        'Conectamos con el servidor de visual time.
        Dim oConnector As Object = Nothing
        Dim strWorkingStationName As String = String.Empty
        Try

            pobjServer = ConnectToServer(strWorkingStationName)
            'Obrir la sessió
            pobjSession = OpenSession(pobjServer, strWorkingStationName)
            'Obrir la connexió
            oConnector = CreateObject("VTConnector.roConnector")
            oConnector.Connect(pobjServer, pobjSession("ID"))
        Catch ex As Exception
            oConnector = Nothing
        End Try
        Return oConnector
    End Function

    Private Function OpenSession(ByRef pobjServer As Object, Optional ByRef pstrWorkStationName As String = "") As Object
        Const roLoginSystemUser = "(VisualTime Module)"
        Const roLoginSystemPassword = "(98hrjin20fg8)"

        Dim oSessionObject As Object = Nothing
        Try

            'Creamos los objectos
            oSessionObject = CreateObject("VTDTypes.roCollection")
            ' Inicia sesion en servidor
            oSessionObject.LoadXMLString(pobjServer.OpenSessionEx(roLoginSystemUser, roLoginSystemPassword, pstrWorkStationName))
        Catch ex As Exception
            oSessionObject = Nothing
        End Try

        Return oSessionObject
    End Function

    Private Sub DesconectarTot(ByRef pobjServer As Object, ByRef pobjSession As Object, ByRef pobjConnect As Object)
        Try

            'Descargamos los objectos
            pobjConnect.Server.CloseSession(pobjSession("ID"))
            pobjConnect.Disconnect() : pobjConnect.unload()
            pobjConnect = Nothing : pobjSession = Nothing : pobjServer = Nothing
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region " Init Task "
    Public Function InitTask_DailyCauses() As Boolean
        Return InitTask("TABLE:\\DAILYCAUSES")
    End Function

    Public Function InitTask_DailyMoves() As Boolean
        Return InitTask("TABLE:\\MOVES")
    End Function

    Public Function InitTask_DailySchedule() As Boolean
        Return InitTask("TABLE:\\DAILYSCHEDULE")
    End Function

    Public Function InitTask_ProgrammedAbsences() As Boolean
        Return InitTask("TABLE:\\PROGRAMMEDABSENCES")
    End Function

    Public Function InitTask_Entries() As Boolean
        Return InitTask("TABLE:\\ENTRIES")
    End Function

    Public Function InitTask_EmployeeJobTime() As Boolean
        Return InitTask("TABLE:\\EMPLOYEEJOBTIME")
    End Function

    Public Function InitTask(ByVal pstrTriggerToExecute As String) As Boolean
        Dim objServer As Object = Nothing, objSession As Object = Nothing, objConnect As Object = Nothing
        InitTask = False
        Try
            objConnect = ConnectarTot(objServer, objSession)
            'Mandamos la tarea
            objConnect.Context("DataOp") = pstrTriggerToExecute
            Return True
        Catch ex As Exception
            Return True
        Finally
            DesconectarTot(objServer, objSession, objConnect)
        End Try
    End Function
#End Region

    Public Function ConnectionString() As String
        Dim objServer As Object = Nothing, objSession As Object = Nothing, objConnect As Object = Nothing
        ConnectionString = ""
        Try
            objConnect = ConnectarTot(objServer, objSession)
            'Demanar paràmetre de conexió a base de dades
            ConnectionString = objConnect.Context("ADOConnectionString")
        Catch ex As Exception
        Finally
            DesconectarTot(objServer, objSession, objConnect)
        End Try
    End Function

    Public ReadOnly Property LastError() As String
        Get
            Return mLastError
        End Get
    End Property
End Class
