Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Threading
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

'Imports System.IO

''' <summary>
''' Provides methods for common sql data access tasks.
''' </summary>
Friend NotInheritable Class AccessHelperSql

    Const _DBConnectionString As String = "_DBConnectionString"
    Const _ReadDBConnectionString As String = "_ReadDBConnectionString"

    Private Sub New()
    End Sub

    '<TimeToAccquireConnection>
    Public Shared Function CreateCompanyConnection(ByVal companyName As String) As SqlConnection

        ' Connect using sql authentication
        Dim strConnectionstring As String = GetCompanyConnectionString(companyName)

        Dim Conn As New SqlConnection(strConnectionstring)
        Conn.Open()

        If Conn.State = ConnectionState.Open Then
            Dim oCommand As New SqlCommand("SET ARITHABORT ON", Conn)
            oCommand.ExecuteNonQuery()

            Dim cmd As DbCommand = New SqlCommand("SELECT Data from sysroparameters where  ID ='CCode'", Conn, Nothing)
            Dim oCompanyCode As String = String.Empty
            Try
                oCompanyCode = Robotics.VTBase.roTypes.Any2String(cmd.ExecuteScalar())
            Catch
                oCompanyCode = String.Empty
            End Try

            Dim oAzureCompanyCode As String = companyName.Trim.ToLower

            If oCompanyCode.Trim.ToLower <> oAzureCompanyCode Then
                Conn.Close()
                Conn = Nothing

                Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roCritic, "AccessHelperSQL::FatalError::Accessing db:" & oCompanyCode & "::from client:" & oAzureCompanyCode)
                Throw New ConnectionStringException("Invalid connection string. Aborting connection. DB Company::" & oCompanyCode & "::Azure company::" & oAzureCompanyCode)
            End If
        End If


        Return Conn

    End Function

    '<TimeToAccquireConnection>
    Public Shared Function CreateConnection() As SqlConnection
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim bThrowExeception As Boolean = False
        Dim oCompanyCode As String = String.Empty
        Dim oAzureCompanyCode As String = RoAzureSupport.GetCompanyName().ToLower

        ' Connect using sql authentication
        Dim strConnectionstring As String = GetConnectionString()
        Dim Conn As New SqlConnection(strConnectionstring)
        Conn.Open()

        If Conn.State = ConnectionState.Open Then
            Dim oCommand As New SqlCommand("SET ARITHABORT ON", Conn)
            oCommand.ExecuteNonQuery()

            Dim cmd As DbCommand = New SqlCommand("SELECT Data from sysroparameters where  ID ='CCode'", Conn, Nothing)
            Try
                oCompanyCode = Robotics.VTBase.roTypes.Any2String(cmd.ExecuteScalar())
            Catch
                oCompanyCode = String.Empty
            End Try

            If oCompanyCode.Trim.ToLower <> oAzureCompanyCode Then
                Conn.Close()
                Conn = Nothing
                bThrowExeception = True

            End If
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlOpenTime(watch.Elapsed.TotalSeconds)

        If bThrowExeception Then
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roCritic, "AccessHelperSQL::FatalError::Accessing db:" & oCompanyCode & "::from client:" & oAzureCompanyCode)
            Throw New ConnectionStringException("Invalid connection string. Aborting connection. DB Company::" & oCompanyCode & "::Azure company::" & oAzureCompanyCode)
        End If

        Return Conn

    End Function

    '<TimeToAccquireConnection>
    Public Shared Function CreateReadConnection() As SqlConnection
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim bThrowExeception As Boolean = False
        Dim oCompanyCode As String = String.Empty
        Dim oAzureCompanyCode As String = RoAzureSupport.GetCompanyName().ToLower

        ' Connect using sql authentication
        Dim strConnectionstring As String = GetConnectionString(True)
        Dim Conn As New SqlConnection(strConnectionstring)
        Conn.Open()

        If Conn.State = ConnectionState.Open Then
            Dim oCommand As New SqlCommand("SET ARITHABORT ON", Conn)
            oCommand.ExecuteNonQuery()

            Dim cmd As DbCommand = New SqlCommand("SELECT Data from sysroparameters where  ID ='CCode'", Conn, Nothing)
            Try
                oCompanyCode = Robotics.VTBase.roTypes.Any2String(cmd.ExecuteScalar())
            Catch
                oCompanyCode = String.Empty
            End Try

            If oCompanyCode.Trim.ToLower <> oAzureCompanyCode Then
                Conn.Close()
                Conn = Nothing
                bThrowExeception = True
            End If
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlOpenTime(watch.Elapsed.TotalSeconds)

        If bThrowExeception Then
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roCritic, "AccessHelperSQL::FatalError::Accessing db:" & oCompanyCode & "::from client:" & oAzureCompanyCode)
            Throw New ConnectionStringException("Invalid connection string. Aborting connection. DB Company::" & oCompanyCode & "::Azure company::" & oAzureCompanyCode)
        End If

        Return Conn

    End Function

    Public Shared Function GetConnectionString(Optional ByVal bOnlyRead As Boolean = False) As String
        Dim strConnectionstring As String = ""
        Dim strCompany As String = String.Empty
        Try

            If VTBase.roConstants.IsMultitenantServiceEnabled Then
                If bOnlyRead Then
                    strConnectionstring = VTBase.roTypes.Any2String(Thread.GetDomain.GetData(Thread.CurrentThread.ManagedThreadId.ToString & _ReadDBConnectionString))
                Else
                    strConnectionstring = VTBase.roTypes.Any2String(Thread.GetDomain.GetData(Thread.CurrentThread.ManagedThreadId.ToString & _DBConnectionString))
                End If
            Else
                strCompany = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))

                If strCompany = String.Empty Then
                    strCompany = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ClientCompanyId))
                End If

                If strCompany <> String.Empty Then
                    Dim oConfig As roCompanyConfiguration = DataLayer.roCacheManager.GetInstance.GetCompany(strCompany)
                    If oConfig Is Nothing Then

                        Dim oCompanyConfiguration As New Azure.roCompanyConfigurationRepository
                        oConfig = oCompanyConfiguration.GetCompanyConfiguration(strCompany)

                        If oConfig IsNot Nothing AndAlso oConfig.companyname <> String.Empty Then
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roDatalayer::Connection string for company " & strCompany & " requested from cosmos")
                            DataLayer.roCacheManager.GetInstance.UpdateCompanyCache(oConfig)

                            If bOnlyRead Then
                                If oConfig.readdbconnectionstring IsNot Nothing AndAlso oConfig.readdbconnectionstring <> String.Empty Then
                                    strConnectionstring = oConfig.readdbconnectionstring
                                Else
                                    strConnectionstring = oConfig.dbconnectionstring
                                End If
                            Else
                                strConnectionstring = oConfig.dbconnectionstring
                            End If
                        Else
                            strConnectionstring = String.Empty
                        End If
                    Else
                        If bOnlyRead Then
                            If oConfig.readdbconnectionstring IsNot Nothing AndAlso oConfig.readdbconnectionstring <> String.Empty Then
                                strConnectionstring = oConfig.readdbconnectionstring
                            Else
                                strConnectionstring = oConfig.dbconnectionstring
                            End If
                        Else
                            strConnectionstring = oConfig.dbconnectionstring
                        End If
                    End If
                Else
                    strConnectionstring = ""
                End If
            End If

        Catch ex As Exception
            strConnectionstring = ""
        End Try

        strConnectionstring = AccessHelperSql.SetConnectionStringCredentials(strConnectionstring)

        If strConnectionstring = String.Empty Then
            Throw New ConnectionStringException("Empty connection string. Aborting connection. MTEnabled::True::Company::" & strCompany & "::")
        End If

        Return strConnectionstring
    End Function

    Public Shared Function GetCompanyConnectionString(ByVal companyName As String) As String
        Dim strConnectionstring As String = ""
        Try

            If companyName <> String.Empty Then
                Dim oConfig As roCompanyConfiguration = DataLayer.roCacheManager.GetInstance.GetCompany(companyName)
                If oConfig Is Nothing Then

                    Dim oCompanyConfiguration As New Azure.roCompanyConfigurationRepository
                    oConfig = oCompanyConfiguration.GetCompanyConfiguration(companyName)

                    If oConfig IsNot Nothing AndAlso oConfig.companyname <> String.Empty Then
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roDatalayer::Connection string for company " & companyName & " requested from cosmos")
                        DataLayer.roCacheManager.GetInstance.UpdateCompanyCache(oConfig)
                        strConnectionstring = oConfig.dbconnectionstring
                    Else
                        strConnectionstring = String.Empty
                    End If
                Else
                    strConnectionstring = oConfig.dbconnectionstring
                End If
            Else
                strConnectionstring = ""
            End If
        Catch ex As Exception
            strConnectionstring = ""
        End Try

        strConnectionstring = AccessHelperSql.SetConnectionStringCredentials(strConnectionstring)

        If strConnectionstring = String.Empty Then
            Throw New ConnectionStringException("Empty connection string. Aborting connection. GetCompanyConnectionString::Company::" & companyName & "::")
        End If

        Return strConnectionstring
    End Function

    Private Shared Function SetConnectionStringCredentials(sourceConnectionString As String) As String

        Dim finalConnectionstring As String = ""


        Dim cosmosConfig As New roConfigRepository()
        Dim sqlversion As roAzureConfig = cosmosConfig.GetConfigParameter(roConfigParameter.sqlsecurityversion)

        If sqlversion.value = "1" Then
            If sourceConnectionString.Contains("Application Name=VisualTime Live Business") Then
                finalConnectionstring = sourceConnectionString.Replace("Application Name=VisualTime Live Business", "Application Name=" & roConstants.MTApplicationName() & "_" & Environment.MachineName)
            End If
            Return finalConnectionstring
        End If

        Dim dbUsername As String = RoAzureSupport.GetVisualtimeDBUsername()
        Dim dbPassword As String = RoAzureSupport.GetVisualtimeDBPassword()

        If Not String.IsNullOrEmpty(dbUsername) AndAlso Not String.IsNullOrEmpty(dbPassword) AndAlso Not String.IsNullOrEmpty(sourceConnectionString) Then
            Dim connParams As String() = sourceConnectionString.Split(";")

            finalConnectionstring = $"{finalConnectionstring}Application Name={roConstants.MTApplicationName()}_{Environment.MachineName};"
            finalConnectionstring = $"{finalConnectionstring}User ID={dbUsername};"
            finalConnectionstring = $"{finalConnectionstring}Password={dbPassword};"

            For Each sqlParam As String In connParams
                Dim actualParamValues As String() = sqlParam.Split("=")

                Select Case actualParamValues(0).ToUpper
                    Case "APPLICATION NAME"
                        ' Do nothing
                    Case "USER ID"
                        ' Do nothing
                    Case "PASSWORD"
                        ' Do nothing
                    Case Else
                        finalConnectionstring = $"{finalConnectionstring}{sqlParam};"
                End Select
            Next

        End If

        Return finalConnectionstring
    End Function


    Public Shared Function CreateCommand(ByVal commandText As String, ByVal connection As DbConnection, ByVal transaction As DbTransaction) As SqlCommand
        Return New SqlCommand(commandText, DirectCast(connection, SqlConnection), DirectCast(transaction, SqlTransaction))
    End Function

    Public Shared Function CreateDataAdapter(ByVal selectCommand As DbCommand, Optional ByVal lBuilder As Boolean = False) As SqlDataAdapter
        Dim ad As SqlDataAdapter
        ad = New SqlDataAdapter(DirectCast(selectCommand, SqlCommand))
        If lBuilder Then
            Dim cmdB As New SqlCommandBuilder
            cmdB.DataAdapter = ad
            cmdB.ConflictOption = ConflictOption.OverwriteChanges
            ad.InsertCommand = cmdB.GetInsertCommand
            ad.UpdateCommand = cmdB.GetUpdateCommand
            ad.DeleteCommand = cmdB.GetDeleteCommand
        End If
        Return ad
    End Function

    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType, ByVal size As Integer, ByVal direction As ParameterDirection, ByVal sourceColumn As String) As SqlParameter
        Dim Param As New SqlParameter()
        Param.ParameterName = parameterName
        Param.DbType = dataType
        Param.Size = size
        Param.Direction = direction
        Param.SourceColumn = sourceColumn
        command.Parameters.Add(Param)
        Return Param
    End Function

End Class