Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.Extensions

<DataContract>
Public Class roTerminalMessage

    Private StrMessage As String
    Private StrSchedule As String
    Private DatLastTimeShown As System.DateTime
    Private IntForAllEmployees As Integer

#Region "Properties"

    <DataMember>
    Public Property Message() As String
        Get
            Return StrMessage
        End Get
        Set(ByVal value As String)
            StrMessage = value
        End Set
    End Property

    <DataMember>
    Public Property Schedule() As String
        Get
            Return StrSchedule
        End Get
        Set(ByVal value As String)
            StrSchedule = value
        End Set
    End Property

    <DataMember>
    Public Property LastTimeShown() As DateTime
        Get
            Return DatLastTimeShown
        End Get
        Set(ByVal value As DateTime)
            DatLastTimeShown = value
        End Set
    End Property

    <DataMember>
    Public Property ForAllEmployees() As Integer
        Get
            Return IntForAllEmployees
        End Get
        Set(ByVal value As Integer)
            IntForAllEmployees = value
        End Set
    End Property

#End Region

#Region "TerminalMessages"

    Public Shared Function GetTerminalMessages(ByVal IDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As System.Data.DataSet
        ' Devuelve un dataset con los grupos disponibles
        Dim strQuery As String
        Dim oDataset As System.Data.DataSet = Nothing

        oState.UpdateStateInfo()

        strQuery = "@SELECT# * From EmployeeTerminalMessages "
        strQuery = strQuery & " Where IDEmployee = " & IDEmployee

        Try
            oDataset = CreateDataSet(strQuery)
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessages")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessages")
        End Try

        Return oDataset

    End Function

    Public Shared Function PendingTerminalMessages(ByVal IDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As Boolean
        ' Devuelve si hay mensajes pendientes de mostrar al empleado
        Dim strQuery As String
        Dim oDataTable As System.Data.DataTable = Nothing

        Try

            oState.UpdateStateInfo()
            strQuery = "@SELECT# * From EmployeeTerminalMessages Where lastTimeShown is null AND IDEmployee = " & IDEmployee
            oDataTable = CreateDataTable(strQuery)

            If oDataTable.Rows.Count > 0 Then
                Return True
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessages")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessages")
        Finally

        End Try

        Return False

    End Function

    Public Shared Function GetTerminalMessage(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByRef oState As Employee.roEmployeeState) As roTerminalMessage
        ' Devuelve los datos de un mensaje de terminal
        Dim strQuery As String
        Dim oDataset As System.Data.DataSet
        Dim oDatareader As System.Data.Common.DbDataReader
        Dim oTerminalMessage As roTerminalMessage = Nothing

        oState.UpdateStateInfo()

        strQuery = "@SELECT# * From EmployeeTerminalMessages "
        strQuery = strQuery & " Where IDEmployee = " & IDEmployee
        strQuery = strQuery & " And Message = '" & TerminalMessage & "' "
        strQuery = strQuery & " And LastTimeShown IS NULL "

        Try
            oDataset = CreateDataSet(strQuery) ' Ejecuto la sql
            oDatareader = oDataset.CreateDataReader

            If oDatareader IsNot Nothing Then
                oDatareader.Read()
                If oDatareader.HasRows Then
                    oTerminalMessage = New roTerminalMessage
                    oTerminalMessage.Message = oDatareader("Message")
                    oTerminalMessage.Schedule = oDatareader("Schedule")
                    If Not IsDBNull(oDatareader("LastTimeShown")) Then
                        oTerminalMessage.LastTimeShown = oDatareader("LastTimeShown")
                    End If
                    oTerminalMessage.ForAllEmployees = oDatareader("ForAllEmployees")
                End If
            End If

            oDatareader.Close() ' Cierro el DataReader
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessage")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessage")
        End Try

        Return oTerminalMessage

    End Function

    Public Shared Function GetTerminalMessageByID(ByVal ID As Integer, ByRef oState As Employee.roEmployeeState) As roTerminalMessage
        ' Devuelve los datos de un mensaje de terminal
        Dim strQuery As String
        Dim oDataset As System.Data.DataSet
        Dim oDatareader As System.Data.Common.DbDataReader
        Dim oTerminalMessage As roTerminalMessage = Nothing

        oState.UpdateStateInfo()

        strQuery = "@SELECT# * From EmployeeTerminalMessages Where ID = " & ID.ToString

        Try
            oDataset = CreateDataSet(strQuery) ' Ejecuto la sql
            oDatareader = oDataset.CreateDataReader

            If oDatareader IsNot Nothing Then
                oDatareader.Read()
                If oDatareader.HasRows Then
                    oTerminalMessage = New roTerminalMessage
                    oTerminalMessage.Message = oDatareader("Message")
                    oTerminalMessage.Schedule = oDatareader("Schedule")
                    If Not IsDBNull(oDatareader("LastTimeShown")) Then
                        oTerminalMessage.LastTimeShown = oDatareader("LastTimeShown")
                    End If
                    oTerminalMessage.ForAllEmployees = oDatareader("ForAllEmployees")
                End If
            End If

            oDatareader.Close() ' Cierro el DataReader
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessageByID")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployees::GetTerminalMessageByID")
        End Try

        Return oTerminalMessage

    End Function

    Public Shared Function DeleteTerminalMessages(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByVal ID As Integer, ByRef oState As Employee.roEmployeeState) As Boolean
        ' Borra los datos de un mensaje de terminal.
        ' Si el mensaje esta marcado como "Para Todos Los Empleados" no se tiene en cuenta
        ' el IDEmployee y se borran ese mensaje para todos los empleados.
        Dim strQuery As String = String.Empty
        Dim oTerminalMessage As roTerminalMessage

        oState.UpdateStateInfo()

        Try

            If ID > 0 Then
                oTerminalMessage = GetTerminalMessageByID(ID, oState)
                strQuery = "@DELETE# FROM EmployeeTerminalMessages where id = " & ID.ToString
            Else
                oTerminalMessage = GetTerminalMessage(IDEmployee, TerminalMessage, oState)

                If oState.Result = EmployeeResultEnum.NoError Then

                    If oTerminalMessage IsNot Nothing Then

                        strQuery = "@DELETE# FROM EmployeeTerminalMessages "
                        If oTerminalMessage.ForAllEmployees = 0 Then ' Si el mensaje es solo para este empleado
                            strQuery = strQuery & " Where IDEmployee = " & IDEmployee
                            strQuery = strQuery & " And Message = '" & TerminalMessage.Replace("'", "''") & "' "
                        Else
                            strQuery = strQuery & " Where Message = '" & TerminalMessage.Replace("'", "''") & "' "
                        End If
                    End If
                End If
            End If

            If ExecuteSql(strQuery) Then
                oState.Result = EmployeeResultEnum.NoError
                ' Si el mensaje no se ha mostrado nunca, lanzo broadcaster por si no queda ninguno sin mostrar (el empleado debe marcarse como offline en el terminal)
                If oTerminalMessage.LastTimeShown.Year = 1 Then
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Else
                oState.Result = EmployeeResultEnum.ConnectionError
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployees::DeleteTerminalMessages")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployees::DeleteTerminalMessages")
        End Try

        Return (oState.Result = EmployeeResultEnum.NoError)

    End Function

    Public Shared Function ValidateTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByRef oState As Employee.roEmployeeState) As Boolean
        ' Valida los datos del mensaje de terminal
        oState.UpdateStateInfo()

        If TerminalMessage.Schedule = "" Then
            oState.Result = EmployeeResultEnum.InvalidSchedule
        End If

        Return (oState.Result = EmployeeResultEnum.NoError)

    End Function

    Public Shared Function SaveTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByRef oState As Employee.roEmployeeState) As Boolean
        ' Guarda los datos del mensaje de terminal
        ' Si esta marcado ForAllEmployees se genera una copia de ese mensaje para todos los empleados
        Dim strQuery As String
        Dim oDataset As System.Data.DataSet
        Dim oDataReader As System.Data.Common.DbDataReader = Nothing

        oState.UpdateStateInfo()

        If ValidateTerminalMessage(IdEmployee, TerminalMessage, oState) Then

            Try

                Dim oTerminalMessage As roTerminalMessage = GetTerminalMessage(IdEmployee, TerminalMessage.Message, oState)

                If (oState.Result = EmployeeResultEnum.NoError) Then

                    If oTerminalMessage Is Nothing Then

                        ' El mensaje no existe, hago un insert
                        strQuery = " @SELECT# * From Employees " ' Monto una query que me devolvera los usuarios a los que hay que enviar el mensaje
                        If TerminalMessage.ForAllEmployees = 0 Then
                            strQuery = strQuery & " Where ID = " & IdEmployee
                        End If

                        oDataset = CreateDataSet(strQuery) ' Cargo todos los empleados a los que se enviara el mensaje
                        oDataReader = oDataset.CreateDataReader

                        Do While oDataReader.Read()
                            strQuery = " @INSERT# INTO EmployeeTerminalMessages "
                            strQuery = strQuery & " ( IdEmployee, Message, "
                            strQuery = strQuery & " Schedule, ForAllEmployees) "
                            strQuery = strQuery & " Values "
                            strQuery = strQuery & " ( " & oDataReader("ID") & ", '" & TerminalMessage.Message.Replace("'", "''") & "' , "
                            strQuery = strQuery & " '" & TerminalMessage.Schedule & "' ," & TerminalMessage.ForAllEmployees & ")"
                            If ExecuteSql(strQuery) Then
                                oState.Result = EmployeeResultEnum.NoError
                            Else
                                oState.Result = EmployeeResultEnum.ConnectionError
                            End If
                        Loop
                        oDataReader.Close()
                    Else

                        ' El mensaje existe, hago un update
                        strQuery = " @UPDATE# EmployeeTerminalMessages "
                        strQuery = strQuery & " set Schedule = '" & TerminalMessage.Schedule & "' "
                        strQuery = strQuery & " , ForAllEmployees = " & TerminalMessage.ForAllEmployees
                        strQuery = strQuery & " Where IDEmployee = " & IdEmployee
                        strQuery = strQuery & " And Message = '" & TerminalMessage.Message.Replace("'", "''") & "' "

                        If ExecuteSql(strQuery) Then
                            oState.Result = EmployeeResultEnum.NoError
                        Else
                            oState.Result = EmployeeResultEnum.ConnectionError
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveTerminalMessage")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveTerminalMessage")
            Finally
                If oDataReader IsNot Nothing AndAlso Not oDataReader.IsClosed Then oDataReader.Close()
            End Try

        End If

        Return (oState.Result = EmployeeResultEnum.NoError)

    End Function

#End Region

End Class