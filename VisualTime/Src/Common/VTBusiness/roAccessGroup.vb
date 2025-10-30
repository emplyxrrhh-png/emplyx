Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Security.Base

Namespace AccessGroup

    <DataContract()>
    Public Class roAccessGroup

#Region "Declarations - Constructor"

        Private oState As roAccessGroupState

        Private intID As Integer
        Private strName As String
        Private strShortName As String

        Private oAccessGroupPermissions As Generic.List(Of roAccessGroupPermission)
        Private oEmployees As Generic.List(Of roEmployeeDescription)
        Private oGroups As Generic.List(Of roGroupDescription)

        Private oAuthorizationDocuments As Generic.List(Of roAuthorizationDocument)

        Public Sub New()
            Me.oState = New roAccessGroupState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAccessGroupState, Optional ByVal bolAudit As Boolean = True, Optional idPassport As Integer = 0)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bolAudit, idPassport)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAccessGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessGroupState)
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
        Public Property AccessGroupPermissions() As Generic.List(Of roAccessGroupPermission)
            Get
                Return Me.oAccessGroupPermissions
            End Get
            Set(ByVal value As Generic.List(Of roAccessGroupPermission))
                Me.oAccessGroupPermissions = value
            End Set
        End Property

        <DataMember()>
        Public Property Employees() As Generic.List(Of roEmployeeDescription)
            Get
                Return Me.oEmployees
            End Get
            Set(ByVal value As Generic.List(Of roEmployeeDescription))
                Me.oEmployees = value
            End Set
        End Property

        <DataMember()>
        Public Property Groups() As Generic.List(Of roGroupDescription)
            Get
                Return Me.oGroups
            End Get
            Set(ByVal value As Generic.List(Of roGroupDescription))
                Me.oGroups = value
            End Set
        End Property

        <DataMember()>
        Public Property AuthorizationDocuments() As Generic.List(Of roAuthorizationDocument)
            Get
                Return Me.oAuthorizationDocuments
            End Get
            Set(ByVal value As Generic.List(Of roAuthorizationDocument))
                Me.oAuthorizationDocuments = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False, Optional idPassport As Integer = 0) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessGroups " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    If Not IsDBNull(oRow("ShortName")) Then Me.strShortName = oRow("ShortName")
                    'Carrega de ZonesException i ZonesInactivity
                    Me.oAccessGroupPermissions = roAccessGroupPermission.GetAccessGroupsPermissionsList(oRow("ID"), oState)
                    Me.oEmployees = roEmployeeDescription.GetEmployeesByAccessGroup(oRow("ID"), oState, idPassport)
                    Me.Groups = roGroupDescription.GetGroupsByAccessGroup(oRow("ID"), oState, idPassport)
                    Me.AuthorizationDocuments = roAuthorizationDocument.GetRequieredDocumentsByAccessGroup(oRow("ID"), oState, idPassport)
                Else
                    Me.oAccessGroupPermissions = Nothing
                    Me.oEmployees = Nothing
                    Me.Groups = Nothing
                    Me.AuthorizationDocuments = Nothing
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAccessGroup, Me.strName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccessGroup::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccessGroup::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal bBypassBroadcaster As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = AccessGroupResultEnum.XSSvalidationError
                    Return False
                End If


                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("AccessGroups")
                Dim strSQL As String = "@SELECT# * FROM AccessGroups WHERE ID = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    Me.ID = GetNextID(Me.oState)
                    oRow = tb.NewRow
                    oRow("ID") = Me.ID
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                If Me.Validate(bolIsNew) Then
                    oRow("Name") = Me.strName
                    oRow("ShortName") = Me.strShortName

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    'Esborrem Daily and Holidays, abans de tornar a afegir
                    Dim DeleteQuerys() As String = {"@DELETE# FROM AccessGroupsPermissions WHERE IDAccessGroup = " & Me.intID.ToString}

                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    'Gravem els AccessGroupPermissions
                    If bolRet Then
                        If Me.oAccessGroupPermissions IsNot Nothing Then
                            If Me.oAccessGroupPermissions.Count > 0 Then
                                For Each oAPD As roAccessGroupPermission In oAccessGroupPermissions
                                    oAPD.IDAccessGroup = Me.ID
                                    bolRet = oAPD.Save()
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If
                    End If

                    'Dim oEmployeeConnector As New Employee.roEmployees
                    'Dim oEmployeeState As New Employee.roEmployeeState()
                    'roBusinessState.CopyTo(Me.oState, oEmployeeState)

                    If bolRet Then
                        ' Miramos si se trata de  una instalación de Accesos en Modo Avanzado
                        Dim pAdvAccessMode As New AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", Nothing)
                        Dim bAdvAccessMode As Boolean = False
                        If pAdvAccessMode IsNot Nothing Then bAdvAccessMode = roTypes.Any2String(pAdvAccessMode.Value) = "1"

                        'Miramos si se guarda las autorizaciones por usuario
                        Dim pAdvAccessGroupMode As New AdvancedParameter.roAdvancedParameter("AccessGroupsMode", Nothing)
                        Dim bAdvAccessGroupMode As Boolean = False
                        If pAdvAccessGroupMode IsNot Nothing Then bAdvAccessGroupMode = pAdvAccessGroupMode.Value.Equals("1")

                        'Obtener lista de empleados actuales antes de comenzar a modificar
                        Dim oldEmployees As DataTable = roBusinessSupport.GetEmployeesOnAccessGroup(Me.ID, String.Empty, "Access", "U", oState)

                        'Obtener lista de grupos actuales antes de comenzar a modificar
                        Dim oldGroups As DataTable = roBusinessSupport.GetGroupsOnAccessGroup(Me.ID, String.Empty, "Access", "U", oState)

                        'Procesar lista de empleados nuevos
                        Dim bolReviewList As Boolean = (Me.Employees IsNot Nothing AndAlso Me.Employees.Count > 0)
                        If bolReviewList Then
                            For Each Item As roEmployeeDescription In Me.Employees
                                bolRet = SaveEmployeeAccessGroup(Item.ID, Me.ID, oState, bAdvAccessMode)
                                If Not bolRet Then Exit For
                            Next
                        End If

                        'Procesa lista de grupos
                        Dim bolGroupsReviewList As Boolean = (Me.Groups IsNot Nothing AndAlso Me.Groups.Count > 0)
                        If bolGroupsReviewList Then
                            ' Primero borro todo y luego añado lo que envía la pantalla
                            bolRet = ExecuteSql("@DELETE# GroupsAccessAuthorization where idauthorization = " + Me.ID.ToString)
                            If bolRet Then
                                For Each Item As roGroupDescription In Me.Groups
                                    bolRet = SaveGroupAccessGroup(Item.ID, Me.ID, oState, bAdvAccessMode)
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        Else
                            ' No hay grupos en esta autorización. Borro los que existan
                            bolRet = ExecuteSql("@DELETE# GroupsAccessAuthorization where idauthorization = " + Me.ID.ToString)
                        End If

                        'Procesar lista de empleados antiguos
                        If bolRet Then
                            If oldEmployees IsNot Nothing AndAlso oldEmployees.Rows.Count > 0 Then
                                Dim intIdEmployee As Integer
                                Dim oEmployeeDescription As roEmployeeDescription
                                For Each dRow As DataRow In oldEmployees.Rows
                                    intIdEmployee = roTypes.Any2Integer(dRow("IdEmployee"))

                                    If bolReviewList Then
                                        oEmployeeDescription = New roEmployeeDescription() : oEmployeeDescription.ID = intIdEmployee
                                        If Not Me.oEmployees.Contains(oEmployeeDescription) Then
                                            'Elimina del grupo de accesos
                                            bolRet = SaveEmployeeAccessGroup(intIdEmployee, Me.ID, oState, bAdvAccessMode, False)
                                        End If
                                    Else
                                        'Elimina del grupo de accesos
                                        bolRet = SaveEmployeeAccessGroup(intIdEmployee, Me.ID, oState, bAdvAccessMode, False)
                                    End If
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If

                        'Procesa lista de autorizaciones
                        Dim bolDocReviewList As Boolean = (Me.AuthorizationDocuments IsNot Nothing AndAlso Me.AuthorizationDocuments.Count > 0)
                        If bolDocReviewList Then
                            ' Primero borro todo y luego añado lo que envía la pantalla
                            bolRet = ExecuteSql("@DELETE# AccessAuthorizationDocumentsTracking where idauthorization = " + Me.ID.ToString)
                            If bolRet Then
                                bolRet = roAuthorizationDocument.SaveDocumentsToAccessGroup(Me.ID, Me.AuthorizationDocuments, Me.State)
                            End If
                        Else
                            ' No hay grupos en esta autorización. Borro los que existan
                            bolRet = ExecuteSql("@DELETE# AccessAuthorizationDocumentsTracking where idauthorization = " + Me.ID.ToString)
                        End If

                        'si tiene que guardar el usuario del grupo de acceso
                        If bolRet AndAlso bAdvAccessGroupMode Then
                            Dim idParentPassport = roPassportManager.GetPassportTicket(oState.IDPassport).IDParentPassport.ToString()
                            strSQL = "IF NOT EXISTS (@SELECT# * FROM sysroPassports_AccessGroup WHERE IDPassport = " & idParentPassport.ToString & " AND IDAccessGroup = " & intID.ToString & ") " &
                                     "BEGIN " &
                                     "@INSERT# INTO sysroPassports_AccessGroup (IDPassport, IDAccessGroup) VALUES(" & idParentPassport.ToString & "," & intID.ToString & ") " &
                                     "END"
                            bolRet = ExecuteSql(strSQL)
                        End If
                    End If
                    'FIN PPR '''''''''''''''''''''''''''''''''''''''''''''''

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessGroup, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet And Not bBypassBroadcaster Then
                        ' Notificamos al servidor que tiene que lanzar el broadcaster
                        ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                        roConnector.InitTask(TasksType.BROADCASTER)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessGroup:: Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessGroup::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveAccessGroupByPassport(ByVal oState As roAccessGroupState, ByVal intIDPassport As Integer, ByVal intAccessGroups() As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos empleados y grupos asignados
                Dim DeleteQuerys As String() = {"@DELETE# FROM sysroPassports_AccessGroup WHERE IDPassport = " & intIDPassport.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete)
                    If Not bolRet Then Exit For
                Next

                'Asignamos las nuevas autorizaciones
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For i = 0 To intAccessGroups.Length - 1
                        bolRet = ExecuteSql("@INSERT# INTO sysroPassports_AccessGroup (IDPassport, IDAccessGroup) VALUES(" & intIDPassport.ToString & "," & intAccessGroups(i).ToString & ")")
                        If Not bolRet Then Exit For
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccessGroup::SaveSccessGroupByPassport")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccessGroup::SaveSccessGroupByPassport")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Employees.Count > 0 OrElse Me.Groups.Count > 0 Then
                    oState.Result = DTOs.AccessGroupResultEnum.NoDeleteforEmployeesAssigned
                    bolRet = False
                End If

                If bolRet Then
                    If roTypes.Any2Double(ExecuteScalar("@SELECT# COUNT(*) FROM EventAccessAuthorization WHERE IDAuthorization=" & Me.ID.ToString)) > 0 Then
                        oState.Result = DTOs.AccessGroupResultEnum.EventSchedulerAssigned
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    Dim DeleteQuerys As String() = {"@DELETE# FROM AccessAuthorizationDocumentsTracking WHERE IDAuthorization = " & Me.intID.ToString,
                                                    "@DELETE# FROM AccessGroupsPermissions WHERE IDAccessGroup = " & Me.intID.ToString,
                                                    "@DELETE# FROM AccessGroups WHERE ID = " & Me.intID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessGroup, Me.strName, Nothing, -1)
                End If

                If bolRet Then
                    ' Notificamos al servidor que tiene que lanzar el broadcaster
                    ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessGroup::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessGroup::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal bolIsNew As Boolean) As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.strShortName.Trim = String.Empty Then
                    Me.oState.Result = DTOs.AccessGroupResultEnum.ShortNameEmpty
                    bolRet = False
                End If

                If Me.strName.Length > 50 Then
                    Me.oState.Result = DTOs.AccessGroupResultEnum.NameToLong
                    bolRet = False
                End If

                If bolRet Then
                    ' Verificamos que el nombre corto de la autorización sea único
                    Dim strSQL As String = "@SELECT# * FROM AccessGroups WHERE ShortName = '" & Me.strShortName & "'"
                    Dim tbAuthorizations As DataTable = CreateDataTable(strSQL)
                    If tbAuthorizations IsNot Nothing Then
                        If bolIsNew Then
                            If tbAuthorizations.Rows.Count > 0 Then
                                Me.oState.Result = DTOs.AccessGroupResultEnum.ShortNameAlreadyExists
                                bolRet = False
                            End If
                        Else
                            For Each oAuthRow As DataRow In tbAuthorizations.Rows
                                If roTypes.Any2Integer(oAuthRow("ID")) <> Me.intID Then
                                    Me.oState.Result = DTOs.AccessGroupResultEnum.ShortNameAlreadyExists
                                    bolRet = False
                                End If
                            Next
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccessGroup::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccessGroup::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente codigo a usar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNextID(ByVal _State As roAccessGroupState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM AccessGroups "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::GetNextID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::GetNextID")
            End Try

            Return intRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function EmptyAccessGroupEmployees(ByVal _IDSourceAccess As Integer, ByVal _State As roAccessGroupState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim oRet As Boolean = Nothing

            Try
                Dim oAccessGroup As New roAccessGroup(_IDSourceAccess, _State, False)

                oAccessGroup.Employees = New Generic.List(Of roEmployeeDescription)
                oAccessGroup.Groups = New Generic.List(Of roGroupDescription)
                oRet = oAccessGroup.Save(bAudit)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::CopyAccess")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::CopyAccess")
            End Try

            Return oRet

        End Function

        Public Shared Function CopyAccess(ByVal _IDSourceAccess As Integer, ByVal _NewName As String, ByVal _State As roAccessGroupState, Optional ByVal bAudit As Boolean = False) As roAccessGroup

            Dim oRet As roAccessGroup = Nothing

            Try
                oRet = New roAccessGroup(_IDSourceAccess, _State, False)

                oRet.ID = -1
                If _NewName = String.Empty Then
                    _State.Language.ClearUserTokens()
                    _State.Language.AddUserToken(oRet.Name)
                    _NewName = _State.Language.Translate("Access.AccessSave.Copy", String.Empty)
                    _State.Language.ClearUserTokens()
                End If
                oRet.Name = "_" & _NewName
                Dim a As New Random()
                oRet.Name = oRet.Name & " " & a.Next(55, 200).ToString
                oRet.ShortName = "000" & GetNextID(_State)
                oRet.ShortName = oRet.ShortName.Substring(oRet.ShortName.Length - 3)
                If Not oRet.Save(bAudit, True) Then
                    oRet = Nothing
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::CopyAccess")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::CopyAccess")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessGroupsList(ByVal _State As roAccessGroupState, Optional ByVal bolAudit As Boolean = True) As Generic.List(Of roAccessGroup)

            Dim oRet As New Generic.List(Of roAccessGroup)

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessGroups Order by Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAccessGroup As roAccessGroup = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAccessGroup = New roAccessGroup(oRow("ID"), _State)
                        oRet.Add(oAccessGroup)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsList")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAuthorizationsByZone(ByVal IDZone As Integer, ByVal _State As roAccessGroupState, Optional bAudit As Boolean = False) As Generic.List(Of roAccessGroup)

            Dim oRet As New Generic.List(Of roAccessGroup)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# AccessGroups.ID, AccessGroups.Name from AccessGroupsPermissions inner join AccessGroups On AccessGroups.ID = AccessGroupsPermissions.IDAccessGroup where AccessGroupsPermissions.IDZone = " & IDZone.ToString
                strSQL &= " ORDER BY NAME"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAuthorization As roAccessGroup = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAuthorization = New roAccessGroup(oRow("ID"), _State, bAudit)
                        oRet.Add(oAuthorization)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetAuthorizationsByZone")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetAuthorizationsByZone")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessGroupsDataTable(ByVal _State As roAccessGroupState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim pAdvAccessGroupMode As New AdvancedParameter.roAdvancedParameter("AccessGroupsMode", Nothing)
                Dim bAdvAccessGroupMode = pAdvAccessGroupMode.Value.Equals("1")
                Dim idPassport = "0"
                If (bAdvAccessGroupMode) Then
                    idPassport = roPassportManager.GetPassportTicket(_State.IDPassport).IDParentPassport.ToString()
                End If

                Dim strSQL As String = "@SELECT# * FROM AccessGroups "
                If (bAdvAccessGroupMode) Then strSQL &= "where id in (@SELECT# IDAccessGroup from sysroPassports_AccessGroup where IDPassport = " & idPassport & " ) "
                strSQL &= "ORDER BY Name "
                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetAccessGroupsByPassport(ByVal _State As roAccessGroupState, ByVal intIDPassport As Integer) As Integer()

            Dim tbRet As DataTable = Nothing
            Dim intRet() As Integer = {0}

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessGroups WHERE ID IN (@SELECT# DISTINCT IDAccessGroup FROM sysroPassports_AccessGroup WHERE IDPassport =" & intIDPassport.ToString() & ") "
                strSQL = strSQL & " Order by Name"
                tbRet = CreateDataTable(strSQL, )

                If tbRet IsNot Nothing Then
                    ReDim intRet(tbRet.Rows.Count - 1)

                    Dim i As Integer = 0
                    For Each oRow As DataRow In tbRet.Rows
                        intRet(i) = oRow("ID")
                        i += 1
                    Next
                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsByPassport")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsByPassport")
            End Try

            Return intRet

        End Function

        Public Shared Function GetAccessGroupsByPassportDataTable(ByVal oState As roAccessGroupState, ByVal intIDPassport As Integer, ByVal validateParent As Boolean) As DataTable

            Dim tbRet As DataTable = Nothing

            Try
                Dim passportId As String = If(validateParent, intIDPassport.ToString(), roPassportManager.GetPassportTicket(intIDPassport).IDParentPassport.ToString())

                Dim strSQL As String = "@SELECT# * FROM AccessGroups WHERE ID IN (@SELECT# DISTINCT IDAccessGroup FROM sysroPassports_AccessGroup WHERE IDPassport =" & passportId & ") "

                strSQL = strSQL & " Order by Name"
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassportDataTable")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassportDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetEmployeeAuthorizations(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByRef _State As roAccessGroupState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim autorizationMode As New AdvancedParameter.roAdvancedParameter("AccessGroupsMode", New AdvancedParameter.roAdvancedParameterState())

                Dim idParentPassport = String.Empty
                If autorizationMode.Value.ToUpper = "1" Then
                    idParentPassport = roPassportManager.GetPassportTicket(_State.IDPassport).IDParentPassport.ToString()
                End If

                Dim strSQL As String = String.Empty

                If IDEmployee > 0 Then
                    strSQL = "@SELECT# IDEmployee,IDAuthorization,GroupPath  FROM sysrovwAccessAuthorizations WHERE IDEmployee = " & IDEmployee
                    If (autorizationMode.Value.Equals("1")) Then strSQL &= " and IDAuthorization in (@SELECT# IDAccessGroup from sysroPassports_AccessGroup where IDPassport = " & idParentPassport & ") "
                    strSQL = strSQL & " UNION @SELECT# " & IDEmployee & ", IDAccessGroup, '0' AS GroupPath FROM Employees WHERE ID = " & IDEmployee & " AND IDAccessGroup IS NOT NULL"
                    If (autorizationMode.Value.Equals("1")) Then strSQL &= " and IDAccessGroup in (@SELECT# IDAccessGroup from sysroPassports_AccessGroup where IDPassport = " & idParentPassport & ")"

                ElseIf IDGroup > 0 Then
                    strSQL = "@SELECT# Path FROM dbo.Groups WHERE ID = " & IDGroup
                    Dim strGroupPath As String = roTypes.Any2String(ExecuteScalar(strSQL))
                    strSQL = "@SELECT# IDGroup AS IDEmployee,IDAuthorization, gr.Path AS GroupPath FROM GroupsAccessAuthorization gaa INNER JOIN Groups gr on gaa.IDGroup = gr.ID WHERE '" + strGroupPath + "' like gr.Path + '\%' OR '" & strGroupPath & "' = gr.Path "
                End If

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupsDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetAccessGroupByShortName(ByVal strShortName As String, ByVal _State As roAccessGroupState) As Integer
            Dim iRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# ID FROM AccessGroups WHERE ShortName = '" & strShortName & "'"

                iRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                iRet = 0
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupByShortName")
            Catch ex As Exception
                iRet = 0
                _State.UpdateStateInfo(ex, "roAccessGroup::GetAccessGroupByShortName")
            End Try

            Return iRet
        End Function

        Public Shared Function SaveEmployeeAccessGroup(ByVal IdEmployee As Integer, ByVal IDAccessGroup As Nullable(Of Integer), ByRef oState As roAccessGroupState,
                                                Optional bAdvancedAccessMode As Boolean = False, Optional StaysInGroup As Boolean = True, Optional bolAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strQuery As String

                ' Actualizamos tabla de empleados (independientemente del modo de accesos, avanzado o clásico, por compatibilidad)
                If IDAccessGroup.HasValue AndAlso StaysInGroup Then
                    strQuery = "@UPDATE# Employees SET IDAccessGroup = " & IDAccessGroup & "  WHERE ID = " & IdEmployee
                Else
                    strQuery = "@UPDATE# Employees SET IDAccessGroup = NULL  WHERE ID = " & IdEmployee
                End If
                bolRet = ExecuteSql(strQuery)

                ' Si es una instalación de accesos en modo avanzado, actualizo las tabla EmployeeAccessAuthorization
                If bAdvancedAccessMode Then
                    If IDAccessGroup.HasValue AndAlso StaysInGroup Then
                        strQuery = "IF NOT EXISTS (@SELECT# * FROM EmployeeAccessAuthorization WHERE IDEmployee = " & IdEmployee.ToString & " AND IDAuthorization = " & IDAccessGroup & ") " &
                                     "BEGIN " &
                                     "@INSERT# INTO EmployeeAccessAuthorization values (" & IdEmployee & "," & IDAccessGroup & ") " &
                                     "END"
                    Else
                        strQuery = "@DELETE# EmployeeAccessAuthorization WHERE IDEmployee = " & IdEmployee & " AND IDAuthorization = " & IDAccessGroup
                    End If
                    bolRet = ExecuteSql(strQuery)

                    If bolRet AndAlso Not (IDAccessGroup.HasValue AndAlso StaysInGroup) Then
                        ' Por compatibilidad, si al empleado le queda asignado algún grupo de accesos, y no hay ninguno en la tabla Employees, lo informo ahora para que tenga uno
                        strQuery = "@UPDATE# Employees SET Employees.IDaccessgroup = EA.IDAuthorization "
                        strQuery &= "FROM Employees "
                        strQuery &= "INNER JOIN (@SELECT# IDAuthorization, IDEmployee FROM  EmployeeAccessAuthorization) EA "
                        strQuery &= "ON Employees.id = EA.IDEmployee "
                        strQuery &= "WHERE Employees.ID = " & IdEmployee.ToString & " And Employees.IDAccessGroup Is NULL "
                        bolRet = ExecuteSql(strQuery)
                    End If
                Else
                    strQuery = "@DELETE# EmployeeAccessAuthorization WHERE IDEmployee = " & IdEmployee & " AND IDAuthorization = " & IDAccessGroup
                    bolRet = ExecuteSql(strQuery)
                End If

                If bolRet AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oEmpName As String = roBusinessSupport.GetEmployeeName(IdEmployee, oState)
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, String.Empty, 1)
                    If IDAccessGroup.HasValue Then oState.AddAuditParameter(tbParameters, "{AccessGroup}", roBusinessSupport.GetAccessGroupName(IDAccessGroup, oState), String.Empty, 1)

                    oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tAccessAuthorization, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveEmployeeAccessGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveEmployeeAccessGroup")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveGroupAccessGroup(ByVal IdGroup As Integer, ByVal IDAccessGroup As Integer, ByRef oState As roAccessGroupState,
                                                    Optional bolAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strQuery As String

                ' Si ya existe no hago nada
                strQuery = "@SELECT# * from GroupsAccessAuthorization where IDAuthorization = " + IDAccessGroup.ToString + " and IDGroup = " + IdGroup.ToString
                Dim dtGroupAut As DataTable = Nothing
                dtGroupAut = CreateDataTable(strQuery)

                If dtGroupAut IsNot Nothing AndAlso dtGroupAut.Rows.Count = 0 Then
                    Dim sGroupPath As String = roTypes.Any2String(ExecuteScalar("@SELECT# path from groups where id = " & IdGroup.ToString))
                    bolRet = sGroupPath.Trim.Length > 0

                    If bolRet Then
                        '0.- Borro los grupos que puedan existir y dependan de este que voy a añadir
                        strQuery = "@DELETE# gamain from GroupsAccessAuthorization gamain " &
                            "inner join GroupsAccessAuthorization gaux on gaux.idgroup = gamain.idgroup " &
                            "left join Groups on Groups.ID = gamain.IDGroup " &
                            "where Groups.Path like '" & sGroupPath & "' + '\%' and gamain.IDAuthorization = " & IDAccessGroup.ToString
                        bolRet = ExecuteSql(strQuery)
                        '1.- Compruebo si hay algún grupo padre del que voy a insertar. Si es así, no hace falta guardar...
                        strQuery = "@SELECT# * from sysrovwAccessAuthorizations svaa inner join Groups gr on svaa.GroupPath = gr.Path and svaa.GroupPath <> '0' " &
                               "where  '" & sGroupPath & "' like gr.path + '\%' and IDAuthorization = " & IDAccessGroup.ToString
                        If ExecuteScalar(strQuery) = 0 Then
                            strQuery = "@INSERT# INTO GroupsAccessAuthorization values (" & IdGroup & "," & IDAccessGroup & ")"
                            bolRet = ExecuteSql(strQuery)
                        End If
                    End If
                Else
                    bolRet = True
                End If

                If bolRet AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oEmpName As String = roBusinessSupport.GetGroupName(IdGroup, oState)
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, String.Empty, 1)
                    oState.AddAuditParameter(tbParameters, "{AccessGroup}", roBusinessSupport.GetAccessGroupName(IDAccessGroup, oState), String.Empty, 1)

                    oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tAccessAuthorization, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveGroupAccessGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveGroupAccessGroup")
            End Try

            Return bolRet

        End Function

        Public Shared Function RemoveEmployeeAccessGroup(ByVal IdEmployee As Integer, ByVal IDAccessGroup As Nullable(Of Integer), ByRef oState As roAccessGroupState,
                                                Optional bAdvancedAccessMode As Boolean = False, Optional bolAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String

                ' Si es una instalación de accesos en modo avanzado, actualizo las tabla EmployeeAccessAuthorization
                If bAdvancedAccessMode Then
                    If IDAccessGroup.HasValue Then
                        strQuery = "@DELETE# EmployeeAccessAuthorization WHERE IDEmployee = " & IdEmployee & " AND IDAuthorization = " & IDAccessGroup.Value
                        bolRet = ExecuteSql(strQuery)

                        If bolRet Then
                            Dim oAccessGroupState As New AccessGroup.roAccessGroupState
                            roBusinessState.CopyTo(oState, oAccessGroupState)

                            Dim dtAuthorizations As DataTable = AccessGroup.roAccessGroup.GetEmployeeAuthorizations(IdEmployee, -1, oAccessGroupState)

                            If dtAuthorizations IsNot Nothing AndAlso dtAuthorizations.Rows.Count > 0 Then
                                ' Por compatibilidad, si al empleado le queda asignado el grupo de accesos que se esta asignando, le guararemos otro de la lista de autorizaciones
                                strQuery = "@UPDATE# Employees SET Employees.IDaccessgroup = EA.IDAuthorization "
                                strQuery &= "FROM Employees "
                                strQuery &= "FULL JOIN (@SELECT# top 1 IDAuthorization, IDEmployee FROM  EmployeeAccessAuthorization where IDEmployee = " & IdEmployee.ToString & " and IDAuthorization <> " & IDAccessGroup & ") EA "
                                strQuery &= "ON Employees.id = EA.IDEmployee "
                                strQuery &= "WHERE Employees.ID = " & IdEmployee.ToString & " And Employees.IDAccessGroup = " & IDAccessGroup.Value
                                bolRet = ExecuteSql(strQuery)
                            Else
                                bAdvancedAccessMode = False
                            End If
                        End If
                    Else
                        bAdvancedAccessMode = False
                    End If
                End If

                If Not bAdvancedAccessMode Then
                    strQuery = "@UPDATE# Employees SET IDAccessGroup = NULL  WHERE ID = " & IdEmployee & " AND IDAccessGroup = " & IDAccessGroup
                    bolRet = ExecuteSql(strQuery)
                End If

                If bolRet AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oEmpName As String = roBusinessSupport.GetEmployeeName(IdEmployee, oState)
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, String.Empty, 1)
                    If IDAccessGroup.HasValue Then oState.AddAuditParameter(tbParameters, "{AccessGroup}", roBusinessSupport.GetAccessGroupName(IDAccessGroup, oState), String.Empty, 1)

                    oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessAuthorization, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeAccessGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeAccessGroup")
            End Try

            Return bolRet

        End Function

        Public Shared Function RemoveAllEmployeeAccessGroups(ByVal IdEmployee As Integer, ByRef oState As roAccessGroupState,
                                                Optional bAdvancedAccessMode As Boolean = False, Optional bolAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String

                ' Si es una instalación de accesos en modo avanzado, actualizo las tabla EmployeeAccessAuthorization
                If bAdvancedAccessMode Then
                    strQuery = "@DELETE# EmployeeAccessAuthorization WHERE IDEmployee = " & IdEmployee
                    bolRet = ExecuteSql(strQuery)
                End If

                strQuery = "@UPDATE# Employees SET IDAccessGroup = NULL  WHERE ID = " & IdEmployee
                bolRet = ExecuteSql(strQuery)

                If bolRet AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oEmpName As String = roBusinessSupport.GetEmployeeName(IdEmployee, oState)
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, String.Empty, 1)
                    oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessAuthorization, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeAccessGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeAccessGroup")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveAccessAuthorizations(ByVal intIDEmployee As Integer, ByVal intIDGroup As Integer, ByVal dsAuthorizations As DataSet, ByRef oState As roAccessGroupState, Optional ByVal bolAudit As Boolean = False) As Boolean

            oState.UpdateStateInfo()

            Dim bolRet As Boolean = True
            Dim strSQL As String
            Dim bHaveToClose As Boolean = False

            Dim idAccessZone As New Generic.List(Of Integer)
            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If bolRet Then

                    If intIDEmployee > 0 Then
                        ' Borrar las autorizaciones actuales actuales
                        strSQL = "@DELETE# FROM EmployeeAccessAuthorization " &
                                 "WHERE IDEmployee = " & intIDEmployee.ToString
                        If Not ExecuteSql(strSQL) Then
                            oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                        End If

                        ' Borrar las autorizacion de employees para compatibilidad
                        strSQL = "@UPDATE# Employees SET IDAccessGroup = NULL  WHERE ID = " & intIDEmployee
                        If Not ExecuteSql(strSQL) Then
                            oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                        End If

                        If oState.Result = DTOs.AccessGroupResultEnum.NoError Then
                            If dsAuthorizations IsNot Nothing AndAlso dsAuthorizations.Tables.Count > 0 Then

                                For Each oRow As DataRow In dsAuthorizations.Tables(0).Rows
                                    If roTypes.Any2String(oRow("GroupPath")) = "0" Then
                                        idAccessZone.Add(roTypes.Any2Integer(oRow("IDAuthorization")))
                                        strSQL = "@INSERT# INTO EmployeeAccessAuthorization(IDEmployee, IDAuthorization) " &
                                                 "VALUES(" & intIDEmployee.ToString & ", " & oRow("IDAuthorization") & ")"
                                        If Not ExecuteSql(strSQL) Then
                                            oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                                            Exit For
                                        End If
                                    End If

                                Next

                                If idAccessZone.Count > 0 Then
                                    strSQL = "@UPDATE# Employees SET IDAccessGroup = " & idAccessZone(0) & "  WHERE ID = " & intIDEmployee
                                    If Not ExecuteSql(strSQL) Then
                                        oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                                    End If
                                End If
                            Else
                                oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                            End If

                        End If
                    ElseIf intIDGroup > 0 Then

                        strSQL = "@SELECT# Path FROM dbo.Groups WHERE ID = " & intIDGroup
                        Dim strGroupPath As String = roTypes.Any2String(ExecuteScalar(strSQL))

                        ' Borrar las autorizaciones actuales actuales
                        strSQL = "@DELETE# FROM GroupsAccessAuthorization " &
                                 "WHERE IDGroup = " & intIDGroup.ToString
                        If Not ExecuteSql(strSQL) Then
                            oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                        End If

                        If dsAuthorizations IsNot Nothing AndAlso dsAuthorizations.Tables.Count > 0 Then
                            For Each oRow As DataRow In dsAuthorizations.Tables(0).Rows

                                If strGroupPath = roTypes.Any2String(oRow("GroupPath")) Then

                                    strSQL = "@INSERT# INTO GroupsAccessAuthorization(IDGroup, IDAuthorization) " &
                                         "VALUES(" & intIDGroup.ToString & ", " & oRow("IDAuthorization") & ")"

                                    If Not ExecuteSql(strSQL) Then
                                        oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                                        Exit For
                                    End If

                                    strSQL = "@DELETE# FROM GroupsAccessAuthorization" &
                                         " WHERE IDAuthorization = " & oRow("IDAuthorization") & " AND IDGroup IN (@SELECT# ID From Groups where Path like '" & strGroupPath & "\%')"

                                    If Not ExecuteSql(strSQL) Then
                                        oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                                        Exit For
                                    End If

                                End If
                            Next
                        Else
                            oState.Result = DTOs.AccessGroupResultEnum.ConnectionError
                        End If

                    End If
                End If

                ' Si se hizo algún cambio llamo a Broadcaster
                If oState.Result = DTOs.AccessGroupResultEnum.NoError Then
                    'TODO: Se debería controlar si se han hecho cambios o no
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If

                If oState.Result = DTOs.AccessGroupResultEnum.NoError AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oEmpName As String = roBusinessSupport.GetEmployeeName(intIDEmployee, oState)
                    Dim accessGroupsNames As String = String.Empty
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, String.Empty, 1)
                    If idAccessZone.Count > 0 Then
                        For Each iAccessGroupID As Integer In idAccessZone
                            accessGroupsNames &= $"{roBusinessSupport.GetAccessGroupName(iAccessGroupID, oState)},"
                        Next
                        If accessGroupsNames.Length > 1 Then accessGroupsNames = accessGroupsNames.Substring(0, accessGroupsNames.Length - 1)
                    End If
                    oState.AddAuditParameter(tbParameters, "{AccessGroup}", accessGroupsNames, String.Empty, 1)

                    oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tAccessAuthorization, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SaveAccessAuthorizations")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SaveAccessAuthorizations")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = DTOs.AccessGroupResultEnum.NoError)
            End Try

            Return (oState.Result = DTOs.AccessGroupResultEnum.NoError)

        End Function

        Public Shared Function UpgradeAccessMode(ByVal oState As roAccessGroupState) As Boolean

            Dim oRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# EmployeeAccessAuthorization;delete GroupsAccessAuthorization;insert into [dbo].[EmployeeAccessAuthorization] @SELECT# Id as idEmployee, IDAccessGroup from employees where IDAccessGroup is not null"
                If ExecuteSql(strSQL) Then
                    oRet = True
                    oState.Result = DTOs.AccessGroupResultEnum.NoError
                Else
                    oState.Result = DTOs.AccessGroupResultEnum.Exception
                End If

                If oState.Result = DTOs.AccessGroupResultEnum.NoError Then
                    oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tAccessMode, String.Empty, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccessGroup::UpgradeAccessMode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccessGroup::UpgradeAccessMode")
            End Try

            Return oRet

        End Function

#End Region

    End Class

    Public Class roEmployeeDescription
        Implements IEquatable(Of roEmployeeDescription)

        Private oState As roAccessGroupState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roAccessGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessGroupState)
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

        Public Sub New()
            Me.oState = New roAccessGroupState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAccessGroupState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# ID,Name FROM Employees WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roEmployeeDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeDescription::Load")
            End Try

            Return bolRet

        End Function

        Public Function Equals1(ByVal other As roEmployeeDescription) As Boolean Implements System.IEquatable(Of roEmployeeDescription).Equals
            Return Me.intID = other.intID
        End Function

#Region "Helper Methods"

        Public Shared Function GetEmployeesByAccessGroup(ByVal IDAccessGroup As Integer, ByVal _State As roAccessGroupState, Optional idPassport As Integer = 0) As Generic.List(Of roEmployeeDescription)

            Dim oRet As New Generic.List(Of roEmployeeDescription)

            Try
                Dim strSQL As String

                If idPassport = 0 Then
                    strSQL = "@SELECT# ID, Name, Type, IDAccessGroup FROM Employees Where IDAccessGroup = " & IDAccessGroup.ToString
                    strSQL &= " UNION "
                    strSQL &= "@SELECT# employees.ID , employees.Name, employees.Type, employees.IDAccessGroup FROM Employees, EmployeeAccessAuthorization WHERE IDAuthorization = " & IDAccessGroup.ToString & " AND employees.ID = EmployeeAccessAuthorization.IDEmployee"
                Else
                    strSQL = "@SELECT# ID, Name, Type, IDAccessGroup "
                    strSQL &= " FROM Employees "
                    strSQL &= " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe ON poe.IDPassport = " & idPassport.ToString & " AND poe.IDEmployee=Employees.ID AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate "
                    strSQL &= " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & idPassport.ToString & " AND pof.IdFeature = 9 AND pof.Permission > 0 "
                    strSQL &= " Where IDAccessGroup = " & IDAccessGroup.ToString
                    strSQL &= " UNION "
                    strSQL &= " @SELECT# employees.ID , employees.Name, employees.Type, employees.IDAccessGroup "
                    strSQL &= " FROM Employees"
                    strSQL &= " INNER JOIN EmployeeAccessAuthorization "
                    strSQL &= " ON employees.ID = EmployeeAccessAuthorization.IDEmployee"
                    strSQL &= " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe ON poe.IDPassport = " & idPassport.ToString & " AND poe.IDEmployee=Employees.ID AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate "
                    strSQL &= " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & idPassport.ToString & " AND pof.IdFeature = 9 AND pof.Permission > 0 "
                    strSQL &= " WHERE IDAuthorization = " & IDAccessGroup.ToString
                End If

                strSQL &= "  ORDER BY NAME"

                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    Dim oEmployeeDesc As roEmployeeDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oEmployeeDesc = New roEmployeeDescription(oRow("ID"), _State)
                        oRet.Add(oEmployeeDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetEmployeesByAccessGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetEmployeesByAccessGroup")
            End Try

            Return oRet

        End Function

#End Region

    End Class

    Public Class roGroupDescription
        Implements IEquatable(Of roGroupDescription)

        Private oState As roAccessGroupState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roAccessGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessGroupState)
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

        Public Sub New()
            Me.oState = New roAccessGroupState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAccessGroupState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID, dbo.GetFullGroupPathName(ID) as Name FROM Groups " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                Else

                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupDescription::Load")
            End Try

            Return bolRet

        End Function

        Public Function Equals1(ByVal other As roGroupDescription) As Boolean Implements System.IEquatable(Of roGroupDescription).Equals
            Return Me.intID = other.intID
        End Function

#Region "Helper Methods"

        Public Shared Function GetGroupsByAccessGroup(ByVal IDAccessGroup As Integer, ByVal _State As roAccessGroupState, Optional idPassport As Integer = 0) As Generic.List(Of roGroupDescription)

            Dim oRet As New Generic.List(Of roGroupDescription)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# gaa.IDGroup, dbo.GetFullGroupPathName(gaa.IDGroup) as Name, gaa.IDAuthorization FROM GroupsAccessAuthorization gaa inner join Groups ON gaa.IDGroup = Groups.ID Where IDAuthorization = " & IDAccessGroup.ToString
                strSQL &= "  ORDER BY NAME"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oEmployeeDesc As roGroupDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oEmployeeDesc = New roGroupDescription(oRow("IDGroup"), _State)
                        oRet.Add(oEmployeeDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetEmployeesByAccessGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeDesctiption::GetEmployeesByAccessGroup")
            End Try

            Return oRet

        End Function

#End Region

    End Class

    Public Class roAuthorizationDocument
        Implements IEquatable(Of roAuthorizationDocument)

        Private oState As roAccessGroupState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roAccessGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessGroupState)
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

        Public Sub New()
            Me.oState = New roAccessGroupState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAccessGroupState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID,Name FROM DocumentTemplates " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                Else

                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAuthorizationDocument::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAuthorizationDocument::Load")
            End Try

            Return bolRet

        End Function

        Public Function Equals1(ByVal other As roAuthorizationDocument) As Boolean Implements System.IEquatable(Of roAuthorizationDocument).Equals
            Return Me.intID = other.intID
        End Function

#Region "Helper Methods"

        Public Shared Function GetRequieredDocumentsByAccessGroup(ByVal IDAccessGroup As Integer, ByVal _State As roAccessGroupState, Optional idPassport As Integer = 0) As Generic.List(Of roAuthorizationDocument)

            Dim oRet As New Generic.List(Of roAuthorizationDocument)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# gaa.IDDocument, DocumentTemplates.Name, gaa.IDAuthorization FROM AccessAuthorizationDocumentsTracking gaa inner join DocumentTemplates ON gaa.IDDocument = DocumentTemplates.ID Where IDAuthorization = " & IDAccessGroup.ToString
                strSQL &= "  ORDER BY NAME"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAuthorizationDesc As roAuthorizationDocument = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAuthorizationDesc = New roAuthorizationDocument(oRow("IDDocument"), _State)
                        oRet.Add(oAuthorizationDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAuthorizationDocument::GetRequieredDocumentsByAccessGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAuthorizationDocument::GetRequieredDocumentsByAccessGroup")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveDocumentsToAccessGroup(ByVal IDAccessGroup As Integer, ByVal oDocs As Generic.List(Of roAuthorizationDocument), ByVal _State As roAccessGroupState) As Boolean

            Dim oRet As Boolean = True

            Try

                Dim strSQL As String = String.Empty
                For Each oRow As roAuthorizationDocument In oDocs
                    strSQL = "@INSERT# INTO AccessAuthorizationDocumentsTracking(IDAuthorization, IDDocument) VALUES (" & IDAccessGroup & "," & oRow.ID & ")"
                    oRet = ExecuteSql(strSQL)
                    If Not oRet Then Exit For
                Next
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAuthorizationDocument::GetRequieredDocumentsByAccessGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAuthorizationDocument::GetRequieredDocumentsByAccessGroup")
            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace