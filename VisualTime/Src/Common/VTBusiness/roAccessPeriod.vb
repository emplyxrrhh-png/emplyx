Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace AccessPeriod

    <DataContract()>
    Public Class roAccessPeriod

#Region "Declarations - Constructor"

        Private oState As roAccessPeriodState

        Private intID As Integer
        Private strName As String

        Private oAccessPeriodDaily As Generic.List(Of roAccessPeriodDaily)
        Private oAccessPeriodHolidays As Generic.List(Of roAccessPeriodHolidays)

        Public Sub New()
            Me.oState = New roAccessPeriodState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAccessPeriodState, Optional ByVal bolAudit As Boolean = True)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bolAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAccessPeriodState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessPeriodState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember()>
        Public Property AccessPeriodDaily() As Generic.List(Of roAccessPeriodDaily)
            Get
                Return Me.oAccessPeriodDaily
            End Get
            Set(ByVal value As Generic.List(Of roAccessPeriodDaily))
                Me.oAccessPeriodDaily = value
            End Set
        End Property

        <DataMember()>
        Public Property AccessPeriodHolidays() As Generic.List(Of roAccessPeriodHolidays)
            Get
                Return Me.oAccessPeriodHolidays
            End Get
            Set(ByVal value As Generic.List(Of roAccessPeriodHolidays))
                Me.oAccessPeriodHolidays = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriods " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    'Carrega de ZonesException i ZonesInactivity
                    Me.oAccessPeriodDaily = roAccessPeriodDaily.GetAccessPeriodsDailyList(oRow("ID"), oState)
                    Me.oAccessPeriodHolidays = roAccessPeriodHolidays.GetAccessPeriodHolidaysList(oRow("ID"), oState)
                Else
                    Me.oAccessPeriodDaily = Nothing
                    Me.oAccessPeriodHolidays = Nothing
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAccessPeriod, Me.strName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccessPeriod::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccessPeriod::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function ValidateAccessPeriod() As Boolean

            Dim strQuery As String
            Dim oDataSet As System.Data.DataSet

            ' El nombre no puede estar en blanco
            If Me.Name = "" Then
                oState.Result = DTOs.AccessPeriodResultEnum.InvalidName
                Return False
            End If

            ' El nombre no puede existir en la bdd para otra justificación
            strQuery = " @SELECT# * From AccessPeriods "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " And name = '" & Me.Name.Replace("'", "''") & "' "

            oDataSet = CreateDataSet(strQuery)
            If oDataSet.CreateDataReader.HasRows = True Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = DTOs.AccessPeriodResultEnum.NameAlreadyExist
                Return False
            End If

            'Buscamos si existen dias duplicados
            If SearchSameDayOfWeek() Then
                oState.Result = DTOs.AccessPeriodResultEnum.DuplicatedPeriodsDays
                Return False
            End If

            'Buscamos si existen dias duplicados
            If Me.SearchSameHoliday Then
                oState.Result = DTOs.AccessPeriodResultEnum.DuplicatedPeriodsDays
                Return False
            End If

            Return True

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = AccessPeriodResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.ValidateAccessPeriod Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim bolIsNew As Boolean = False

                    Dim tb As New DataTable("AccessPeriods")
                    Dim strSQL As String = "@SELECT# * FROM AccessPeriods WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        Me.ID = Me.GetNextID()
                        oRow = tb.NewRow
                        oRow("ID") = Me.ID
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    ' Obtenemos los periodos actuales para poder verificar si se han borrado.
                    Dim tbPeriodHolidays As DataTable = CreateDataTable("@SELECT# * FROM AccessPeriodHolidays WHERE IDAccessPeriod = " & Me.intID.ToString)
                    Dim tbPeriodDaily As DataTable = CreateDataTable("@SELECT# * FROM AccessPeriodDaily WHERE IDAccessPeriod = " & Me.intID.ToString)

                    'Esborrem Daily and Holidays, abans de tornar a afegir
                    Dim DeleteQuerys() As String = {"@DELETE# FROM AccessPeriodHolidays WHERE IDAccessPeriod = " & Me.intID.ToString,
                                                    "@DELETE# FROM AccessPeriodDaily WHERE IDAccessPeriod = " & Me.intID.ToString}

                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    'Gravem els AccessPeriodDaily
                    If Me.oAccessPeriodDaily IsNot Nothing Then
                        If Me.oAccessPeriodDaily.Count > 0 Then
                            For Each oAPD As roAccessPeriodDaily In oAccessPeriodDaily
                                oAPD.IDAccessPeriod = Me.ID
                                bolRet = oAPD.Save(False)
                                If Not bolRet Then Exit For
                            Next
                        End If
                    End If

                    'Gravem les AccessPeriodholidays
                    If Me.oAccessPeriodHolidays IsNot Nothing Then
                        If Me.oAccessPeriodHolidays.Count > 0 Then
                            For Each oAPH As roAccessPeriodHolidays In oAccessPeriodHolidays
                                oAPH.IDAccessPeriod = Me.ID
                                bolRet = oAPH.Save(False)
                                If Not bolRet Then Exit For
                            Next
                        End If
                    End If

                    If bolRet And bAudit Then
                        oAuditDataNew = oRow

                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessPeriod, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet Then
                        Dim bolBroadcaster As Boolean = False
                        ' Verificamos si se han modificado datos
                        Dim intCount As Integer = 0
                        If Me.oAccessPeriodHolidays IsNot Nothing Then intCount = Me.oAccessPeriodHolidays.Count
                        If tbPeriodHolidays.Rows.Count <> intCount Then
                            bolBroadcaster = True
                        Else
                            Dim oRows() As DataRow
                            For Each oAPH As roAccessPeriodHolidays In Me.oAccessPeriodHolidays
                                oRows = tbPeriodHolidays.Select("Day = " & oAPH.Day & " AND Month = " & oAPH.Month & " AND BeginTime = '" & Format(oAPH.BeginTime, "yyyy/MM/dd") & "' AND EndTime = '" & Format(oAPH.EndTime, "yyyy/MM/dd") & "'")
                                If oRows.Length = 0 Then
                                    bolBroadcaster = True
                                    Exit For
                                End If
                            Next
                        End If
                        If Not bolBroadcaster Then
                            intCount = 0
                            If Me.oAccessPeriodDaily IsNot Nothing Then intCount = Me.oAccessPeriodDaily.Count
                            If tbPeriodDaily.Rows.Count <> intCount Then
                                bolBroadcaster = True
                            Else
                                Dim oRows() As DataRow
                                For Each oAPD As roAccessPeriodDaily In Me.oAccessPeriodDaily
                                    oRows = tbPeriodDaily.Select("DayofWeek = " & oAPD.DayofWeek & " AND BeginTime = '" & Format(oAPD.BeginTime, "yyyy/MM/dd") & "' AND EndTime = '" & Format(oAPD.EndTime, "yyyy/MM/dd") & "'")
                                    If oRows.Length = 0 Then
                                        bolBroadcaster = True
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                        If bolBroadcaster Then
                            ' Notificamos al servidor que tiene que lanzar el broadcaster
                            ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Busca si hay algún día de la semana coincidente.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SearchSameDayOfWeek() As Boolean
            Dim bolRet As Boolean = False

            Dim oACCP As roAccessPeriodDaily
            Dim oACCPSeach As roAccessPeriodDaily

            Try

                For nACCP As Integer = 0 To Me.oAccessPeriodDaily.Count - 1
                    oACCP = Me.oAccessPeriodDaily(nACCP)
                    For nACCP2 As Integer = 0 To Me.oAccessPeriodDaily.Count - 1
                        If nACCP = nACCP2 Then Continue For
                        oACCPSeach = Me.oAccessPeriodDaily(nACCP2)
                        If oACCP.DayofWeek = oACCPSeach.DayofWeek Then
                            If oACCP.BeginTime >= oACCPSeach.BeginTime And oACCP.EndTime <= oACCPSeach.EndTime Then
                                bolRet = True
                                Exit For
                            End If
                            If oACCP.EndTime >= oACCPSeach.BeginTime And oACCP.EndTime <= oACCPSeach.EndTime Then
                                bolRet = True
                                Exit For
                            End If
                            If oACCPSeach.BeginTime >= oACCP.BeginTime And oACCPSeach.BeginTime <= oACCP.EndTime Then
                                bolRet = True
                                Exit For
                            End If
                            If oACCPSeach.EndTime >= oACCP.BeginTime And oACCPSeach.EndTime <= oACCP.EndTime Then
                                bolRet = True
                                Exit For
                            End If
                            If oACCP.BeginTime = "00:00" And oACCP.BeginTime = "00:00" Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCPSeach.BeginTime = "0:00:00" And oACCPSeach.EndTime = "0:00:00" Then
                                bolRet = True
                                Exit For
                            End If
                        End If

                    Next
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::SearchSameDayOfWeek")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Busca si hay algún festivo coincidente.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SearchSameHoliday() As Boolean
            Dim bolRet As Boolean = False

            Dim oACCP As roAccessPeriodHolidays
            Dim oACCPSeach As roAccessPeriodHolidays

            Try

                For nACCP As Integer = 0 To Me.oAccessPeriodHolidays.Count - 1
                    oACCP = Me.oAccessPeriodHolidays(nACCP)
                    For nACCP2 As Integer = 0 To Me.oAccessPeriodHolidays.Count - 1
                        If nACCP = nACCP2 Then Continue For
                        oACCPSeach = Me.oAccessPeriodHolidays(nACCP2)

                        If oACCP.Day = oACCPSeach.Day And oACCP.Month = oACCPSeach.Month Then
                            If oACCP.BeginTime >= oACCPSeach.BeginTime And oACCP.BeginTime <= oACCPSeach.EndTime Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCP.EndTime >= oACCPSeach.BeginTime And oACCP.EndTime <= oACCPSeach.EndTime Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCPSeach.BeginTime >= oACCP.BeginTime And oACCPSeach.BeginTime <= oACCP.EndTime Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCPSeach.EndTime >= oACCP.BeginTime And oACCPSeach.EndTime <= oACCP.EndTime Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCP.BeginTime.Hour = 0 And oACCP.BeginTime.Minute = 0 And oACCP.EndTime.Hour = 0 And oACCP.EndTime.Minute = 0 Then
                                bolRet = True
                                Exit For
                            End If

                            If oACCPSeach.BeginTime.Hour = 0 And oACCPSeach.BeginTime.Minute = 0 And oACCPSeach.EndTime.Hour = 0 And oACCPSeach.EndTime.Minute = 0 Then
                                bolRet = True
                                Exit For
                            End If

                        End If
                    Next
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::SearchSameHoliday")
            End Try

            Return bolRet
        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQLAP As String = "@SELECT# * FROM AccessGroupsPermissions Where IDAccessPeriod = " & Me.intID
                Dim tb As DataTable = CreateDataTable(strSQLAP)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oState.Result = DTOs.AccessPeriodResultEnum.AccessPeriodInUse
                    bolRet = False
                End If

                If bolRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM AccessPeriodHolidays WHERE IDAccessPeriod = " & Me.intID.ToString,
                                                    "@DELETE# FROM AccessPeriodDaily WHERE IDAccessPeriod = " & Me.intID.ToString,
                                                    "@DELETE# FROM AccessPeriods WHERE ID = " & Me.intID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessPeriod, Me.strName, Nothing, -1)
                End If

                If bolRet Then
                    ' Notificamos al servidor que tiene que lanzar el broadcaster
                    ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente codigo a usar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Try
                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM AccessPeriods "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessPeriod::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetAccessPeriodList(ByVal _State As roAccessPeriodState, Optional ByVal bolAudit As Boolean = True) As Generic.List(Of roAccessPeriod)

            Dim oRet As New Generic.List(Of roAccessPeriod)

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriod Order by Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAccessPeriod As roAccessPeriod = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAccessPeriod = New roAccessPeriod(oRow("ID"), _State, bolAudit)
                        oRet.Add(oAccessPeriod)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriod::GetAccessPeriodList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriod::GetAccessPeriodList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessPeriodDataTable(ByVal _State As roAccessPeriodState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessPeriods ORDER BY Name"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessPeriod::GetAccessPeriodDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessPeriod::GetAccessPeriodDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace