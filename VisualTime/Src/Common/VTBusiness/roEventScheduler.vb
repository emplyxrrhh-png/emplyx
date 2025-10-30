Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace EventScheduler

    <DataContract()>
    Public Class roEventScheduler

#Region "Declarations - Constructor"

        Private oState As roEventSchedulerState

        Private intID As Integer
        Private strName As String = ""
        Private strDescription As String = ""
        Private strShortName As String = ""
        Private xDate As Date
        Private xDateEnd As Date
        Private xMainDate As Date
        Private oAuthorizations As Generic.List(Of roEventAccessAuthorization)

        Public Sub New()

            Me.oState = New roEventSchedulerState(-1)

            Me.intID = -1

        End Sub

        Public Sub New(ByVal _State As roEventSchedulerState)

            Me.oState = _State

            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roEventSchedulerState, Optional ByVal bAudit As Boolean = False)

            Me.oState = _State

            Me.intID = _ID

            Me.Load(bAudit)

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Description As String, ByVal _State As roEventSchedulerState)

            Me.oState = _State

            Me.intID = _ID
            Me.strName = _Name
            Me.strDescription = _Description

        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Authorizations() As Generic.List(Of roEventAccessAuthorization)
            Get
                Return Me.oAuthorizations
            End Get
            Set(ByVal value As Generic.List(Of roEventAccessAuthorization))
                Me.oAuthorizations = value
            End Set
        End Property

        <IgnoreDataMember()>
        Public Property State() As roEventSchedulerState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roEventSchedulerState)
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
        Public Property ShortName() As String
            Get
                Return Me.strShortName
            End Get
            Set(ByVal value As String)
                Me.strShortName = value
            End Set
        End Property
        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property
        <DataMember()>
        Public Property EventDate() As Date
            Get
                Return Me.xDate
            End Get
            Set(ByVal value As Date)
                Me.xDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EventDateEnd() As Date
            Get
                Return Me.xDateEnd
            End Get
            Set(ByVal value As Date)
                Me.xDateEnd = value
            End Set
        End Property

        <DataMember()>
        Public Property EventMainDate() As Date
            Get
                Return Me.xMainDate
            End Get
            Set(ByVal value As Date)
                Me.xMainDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM EventsScheduler " &
                                       "WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = oRow("Name")
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.strShortName = Any2String(oRow("ShortName"))
                    Me.xDate = oRow("Date")
                    Me.xDateEnd = oRow("EndDate")
                    Me.xMainDate = oRow("MainDate")

                    Me.oAuthorizations = roEventAccessAuthorization.GetAuthorizationsList(oRow("ID"), oState)
                Else
                    Me.oAuthorizations = Nothing
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tEventScheduler, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roEvent::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True
            Dim sSQL As String = ""

            Try

                If Me.strName = "" Then
                    bolRet = False
                    Me.oState.Result = EventSchedulerResultEnum.InvalidName
                Else
                    ' miramos si existe otro evento con el mismo nombre
                    sSQL = "@SELECT# count(*) FROM EventsScheduler WHERE ID <> " & Me.intID.ToString & " AND Name ='" & Me.strName & "'"
                    If Any2Double(ExecuteScalar(sSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = EventSchedulerResultEnum.DuplicateName
                    End If

                End If

                If Me.strShortName = "" Then
                    bolRet = False
                    Me.oState.Result = EventSchedulerResultEnum.InvalidShortName
                Else
                    ' miramos si existe otro evento con el mismo nombre corto
                    sSQL = "@SELECT# count(*) FROM EventsScheduler WHERE ID <> " & Me.intID.ToString & " AND ShortName ='" & Me.strShortName & "'"
                    If Any2Double(ExecuteScalar(sSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = EventSchedulerResultEnum.DuplicateShortName
                    End If

                End If

                If Me.xDate = Nothing OrElse Me.xDateEnd = Nothing OrElse Me.xMainDate = Nothing Then
                    bolRet = False
                    Me.oState.Result = EventSchedulerResultEnum.InvalidDate
                Else
                    ' miramos si existe otro evento en la misma fecha
                    sSQL = "@SELECT# count(*) FROM EventsScheduler WHERE ID <> " & Me.intID.ToString & " AND EndDate >= " & Any2Time(xDate).SQLSmallDateTime & " AND Date <= " & Any2Time(xDateEnd).SQLSmallDateTime
                    If Any2Double(ExecuteScalar(sSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = EventSchedulerResultEnum.DuplicateDate
                    End If
                End If

                If Me.xDateEnd < Me.xDate OrElse Me.xMainDate < Me.xDate OrElse Me.xMainDate > Me.xDateEnd Then
                    bolRet = False
                    Me.oState.Result = EventSchedulerResultEnum.InvalidDate
                End If

                If Me.oAuthorizations Is Nothing Then
                    bolRet = False
                    Me.oState.Result = EventSchedulerResultEnum.EmptyAuthorizations
                Else
                    If Me.oAuthorizations.Count = 0 Then
                        bolRet = False
                        Me.oState.Result = EventSchedulerResultEnum.EmptyAuthorizations
                    Else
                        For Each oAuth As roEventAccessAuthorization In Me.oAuthorizations
                            If oAuth.AuthorizationDate = Nothing OrElse oAuth.AuthorizationDate < xDate OrElse oAuth.AuthorizationDate > xDateEnd Then
                                bolRet = False
                                Me.oState.Result = EventSchedulerResultEnum.InvalidAuthorizationDate
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEvent::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = EventSchedulerResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim bolNewEvent As Boolean = False
                    Dim xOldDate As Date = Nothing

                    Dim tb As New DataTable("TimeZones")
                    Dim strSQL As String = "@SELECT# * FROM EventsScheduler " &
                                           "WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        Me.intID = Me.GetNextID()
                        oRow("ID") = Me.intID
                        bolNewEvent = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("ShortName") = Me.strShortName

                    If Not bolNewEvent Then
                        xOldDate = oRow("Date")
                    End If
                    oRow("Date") = Me.xDate
                    oRow("EndDate") = Me.xDateEnd
                    oRow("MainDate") = Me.xMainDate

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow
                    bolRet = True

                    If bolRet Then
                        Dim DeleteQuerys() As String = {"@DELETE# FROM EventAccessAuthorization WHERE IDEvent = " & Me.intID.ToString}
                        For Each strSQLDelete As String In DeleteQuerys
                            bolRet = ExecuteSql(strSQLDelete)
                            If Not bolRet Then Exit For
                        Next

                        If Me.oAuthorizations IsNot Nothing Then
                            If Me.oAuthorizations.Count > 0 Then
                                For Each oAPD As roEventAccessAuthorization In oAuthorizations
                                    oAPD.IDEvent = Me.ID
                                    bolRet = oAPD.Save()
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If
                    End If

                    If bolRet And bAudit Then
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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tEventScheduler, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet Then
                        Dim bolBroad As Boolean = False
                        ' Si la fecha del evento es de hoy o de ayer
                        If Me.xDate = Now.Date Or Me.xDate = Now.Date.AddDays(-1) Then
                            bolBroad = True
                        End If

                        If Not bolBroad And Not bolNewEvent Then
                            If xOldDate = Now.Date Or xOldDate = Now.Date.AddDays(-1) Then
                                bolBroad = True
                            End If
                        End If

                        ' Notificamos al servidor que tiene que lanzar el broadcaster
                        If bolBroad Then
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEvent::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then

                    Dim DeleteQuerys() As String = {"@DELETE# FROM EventAccessAuthorization WHERE IDEvent = " & Me.intID.ToString}
                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    If bolRet Then
                        Dim strSQL As String = "@DELETE# FROM EventsScheduler WHERE ID = " & Me.intID.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If

                    If bolRet Then
                        If Me.xDate.Date = Now.Date Or Me.xDate.Date = Now.Date.AddDays(-1) Then
                            ' Si la fecha del evento es hoy o ayer
                            ' Notificamos al servidor que tiene que lanzar el broadcaster
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEventScheduler, Me.strName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEvent::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function IsUsed() As Boolean
            Dim bolRet As Boolean = False

            Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEvent::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::IsUsed")
            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = -1

            Try
                Dim strSQL As String = "@SELECT# MAX(ID) as MaxID FROM EventsScheduler"
                Dim oRet As Object = ExecuteScalar(strSQL)

                If IsDBNull(oRet) Then
                    intRet = 1
                Else
                    intRet = CInt(oRet) + 1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEvent::GetNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEvent::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetEventsScheduler(ByVal _State As roEventSchedulerState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roEventScheduler)

            Dim oRet As New Generic.List(Of roEventScheduler)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM EventsScheduler Order by ID asc"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roEventScheduler(oRow("ID"), _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEvent::GetEvents")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEvent::GetEvents")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEventsSchedulerByYear(ByVal Year As Integer, ByVal _State As roEventSchedulerState, Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM EventsScheduler where year(date)=" & Year & " Order by Date desc"
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEvent::GetEventsSchedulerByYear")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEvent::GetEventsSchedulerByYear")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEventsSchedulerByName(ByVal Name As String, ByVal _State As roEventSchedulerState, Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM EventsScheduler where Name Like '" & Name & "' Order by Date desc"
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEvent::GetEventsSchedulerByName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEvent::GetEventsSchedulerByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function CopyEvent(ByVal _IDSourceEvent As Integer, ByVal _NewName As String, ByVal _NewDate As Date, ByVal _State As roEventSchedulerState, Optional ByVal bAudit As Boolean = False) As roEventScheduler

            Dim oRet As roEventScheduler = Nothing

            Try
                oRet = New roEventScheduler(_IDSourceEvent, _State, False)

                oRet.ID = -1
                If _NewName = "" Then
                    _State.Language.ClearUserTokens()
                    _State.Language.AddUserToken(oRet.Name)
                    _NewName = _State.Language.Translate("Events.EventSave.Copy", "")
                    _State.Language.ClearUserTokens()
                End If
                oRet.Name = "_" & _NewName
                Dim a As New Random()
                oRet.ShortName = a.Next(100, 9999).ToString
                Dim b As New Random()
                Dim iOffsetDays As Integer = 1
                iOffsetDays = CDate(_NewDate).Subtract(oRet.EventDate).TotalDays
                oRet.EventDate = oRet.EventDate.AddDays(iOffsetDays)
                oRet.EventDateEnd = oRet.EventDateEnd.AddDays(iOffsetDays)
                oRet.EventMainDate = oRet.EventMainDate.AddDays(iOffsetDays)
                oRet.Name = oRet.Name & " " & Format(oRet.EventDate, "ddMMyyyy") & " " & oRet.ShortName

                ' Por último ajustamos las fechas de las distintas autorizaciones
                For Each oAuth As roEventAccessAuthorization In oRet.Authorizations
                    oAuth.AuthorizationDate = oAuth.AuthorizationDate.AddDays(iOffsetDays)
                Next

                If Not oRet.Save(bAudit) Then
                    oRet = Nothing
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEvent::CopyEvent")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEvent::CopyEvent")
            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

    <DataContract()>
    Public Class roEventAccessAuthorization

#Region "Declarations - Constructor"

        Private oState As roEventSchedulerState

        Private intIDEvent As Integer
        Private intIDAuthorization As Integer
        Private dDate As Date

        Public Sub New()
            Me.oState = New roEventSchedulerState()
            Me.intIDEvent = -1
        End Sub

        Public Sub New(ByVal _IDEvent As Integer, ByVal _IDAuthorization As Integer, ByVal _State As roEventSchedulerState)
            Me.oState = _State
            Me.intIDEvent = _IDEvent
            Me.intIDAuthorization = _IDAuthorization
        End Sub

        Public Sub New(ByVal _IDEvent As Integer, ByVal _IDAuthorization As Integer, ByVal _dDate As Date, ByVal _State As roEventSchedulerState)
            Me.oState = _State
            Me.intIDEvent = _IDEvent
            Me.intIDAuthorization = _IDAuthorization
            Me.dDate = _dDate
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roEventSchedulerState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roEventSchedulerState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDEvent() As Integer
            Get
                Return Me.intIDEvent
            End Get
            Set(ByVal value As Integer)
                Me.intIDEvent = value
            End Set
        End Property
        <DataMember()>
        Public Property IDAuthorization() As Integer
            Get
                Return Me.intIDAuthorization
            End Get
            Set(ByVal value As Integer)
                Me.intIDAuthorization = value
            End Set
        End Property
        <DataMember()>
        Public Property AuthorizationDate() As Date
            Get
                Return Me.dDate
            End Get
            Set(ByVal value As Date)
                Me.dDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("EventAccessAuthorization")
                Dim strSQL As String = "@SELECT# * FROM EventAccessAuthorization WHERE IDEvent = " & Me.intIDEvent.ToString & " And IDAuthorization = " & Me.intIDAuthorization & " And Date = " & roTypes.Any2Time(Me.dDate).SQLSmallDateTime
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDEvent") = Me.IDEvent
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("IDAuthorization") = Me.intIDAuthorization

                oRow("Date") = Me.dDate

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)
                bolRet = True

                If bolRet And bAudit Then
                    oAuditDataNew = oRow

                    bolRet = False
                    ' Auditamos
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Audit.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    'Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    'bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessGroupPermission, "", tbAuditParameters, -1, oTrans.Connection)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEventAccessAuthorization::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEventAccessAuthorization::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim DeleteQuerys() As String = {"@DELETE# FROM EventAccessAuthorization WHERE IDEvent = " & Me.intIDEvent.ToString & " And IDAuthorization = " & Me.intIDAuthorization & " And Date = " & roTypes.Any2Time(Me.dDate).SQLSmallDateTime}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    'bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessGroupPermission, "", Nothing, -1, oTrans.Connection)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEventAccessAuthorization::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEventAccessAuthorization::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetAuthorizationsList(ByVal _ID As Integer, ByVal _State As roEventSchedulerState,
                                                      Optional ByVal bAudit As Boolean = False) As Generic.List(Of roEventAccessAuthorization)

            Dim oRet As New Generic.List(Of roEventAccessAuthorization)

            Try

                Dim strSQL As String = "@SELECT# * FROM EventAccessAuthorization Where IDEvent = " & _ID

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oEventAccessAuthorization As roEventAccessAuthorization = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oEventAccessAuthorization = New roEventAccessAuthorization(oRow("IDEvent"), oRow("IDAuthorization"), oRow("Date"), _State)
                        oRet.Add(oEventAccessAuthorization)
                    Next
                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEventAccessAuthorization::GetAuthorizationsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEventAccessAuthorization::GetAuthorizationsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAuthorizationsDataTable(ByVal _ID As Integer, ByVal _State As roEventSchedulerState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# EventAccessAuthorization.* FROM EventAccessAuthorization, AccessGroups Where AccessGroups.ID = EventAccessAuthorization.IDAuthorization AND IDEvent = " & _ID & " Order by AccessGroups.Name"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEventAccessAuthorization::GetAuthorizationsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEventAccessAuthorization::GetAuthorizationsDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace