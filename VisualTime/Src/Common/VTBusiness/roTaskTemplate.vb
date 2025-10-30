Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Task

    <DataContract()>
    Public Class roTaskTemplate

#Region "Declarations - Constructors"

        Private oState As roTaskState

        Private intID As Integer
        Private strName As String
        Private strShortName As String
        Private strDescription As String

        Private intColor As Integer
        Private intIDProject As Integer
        Private strTag As String
        Private intPriority As Integer
        Private intIDPassport As Integer
        Private xExpectedStartDate As Nullable(Of Date)
        Private xExpectedEndDate As Nullable(Of Date)
        Private lngInitialTime As Double
        Private intTypeCollaboration As TaskTypeCollaborationEnum
        Private intModeCollaboration As TaskModeCollaborationEnum
        Private intTypeActivation As TaskTypeActivationEnum

        Private intActivationTask As Integer
        Private xActivationDate As Nullable(Of Date)

        Private intTypeClosing As TaskTypeClosingEnum

        Private xClosingDate As Nullable(Of Date)
        Private intTypeAuthorization As TaskTypeAuthorizationEnum

        Private oEmployees As Generic.List(Of roEmployeeTaskTemplateDescription)
        Private oGroups As Generic.List(Of roGroupTaskTemplateDescription)

        Public Sub New()

            Me.oState = New roTaskState
            Me.intID = -1

            Me.intModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
            Me.intTypeActivation = TaskTypeActivationEnum._ALWAYS
            Me.intTypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
            Me.intTypeClosing = TaskTypeClosingEnum._UNDEFINED
            Me.intTypeCollaboration = TaskTypeCollaborationEnum._ANY

        End Sub

        Public Sub New(ByVal _ID As Long, ByVal _IDProject As Integer, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID
            Me.intIDProject = _IDProject

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        Public Property State() As roTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskState)
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
        Public Property Color() As Integer
            Get
                Return Me.intColor
            End Get
            Set(ByVal value As Integer)
                Me.intColor = value
            End Set
        End Property

        <DataMember()>
        Public Property IDProject() As Integer
            Get
                Return Me.intIDProject
            End Get
            Set(ByVal value As Integer)
                Me.intIDProject = value
            End Set
        End Property
        <DataMember()>
        Public Property Tag() As String
            Get
                Return Me.strTag
            End Get
            Set(ByVal value As String)
                Me.strTag = value
            End Set
        End Property
        <DataMember()>
        Public Property Priority() As Integer
            Get
                Return Me.intPriority
            End Get
            Set(ByVal value As Integer)
                Me.intPriority = value
            End Set
        End Property
        <DataMember()>
        Public Property Passport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property
        <DataMember()>
        Public Property ExpectedStartDate() As Nullable(Of DateTime)
            Get
                Return Me.xExpectedStartDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xExpectedStartDate = value
            End Set
        End Property

        <DataMember()>
        Public Property ExpectedEndDate() As Nullable(Of DateTime)
            Get
                Return Me.xExpectedEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xExpectedEndDate = value
            End Set
        End Property
        <DataMember()>
        Public Property InitialTime() As Double
            Get
                Return Me.lngInitialTime
            End Get
            Set(ByVal value As Double)
                Me.lngInitialTime = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeCollaboration() As TaskTypeCollaborationEnum
            Get
                Return Me.intTypeCollaboration
            End Get
            Set(ByVal value As TaskTypeCollaborationEnum)
                Me.intTypeCollaboration = value
            End Set
        End Property
        <DataMember()>
        Public Property ModeCollaboration() As TaskModeCollaborationEnum
            Get
                Return Me.intModeCollaboration
            End Get
            Set(ByVal value As TaskModeCollaborationEnum)
                Me.intModeCollaboration = value
            End Set
        End Property

        <DataMember()>
        Public Property TypeActivation() As TaskTypeActivationEnum
            Get
                Return Me.intTypeActivation
            End Get
            Set(ByVal value As TaskTypeActivationEnum)
                Me.intTypeActivation = value
            End Set
        End Property
        <DataMember()>
        Public Property ActivationTask() As Integer
            Get
                Return Me.intActivationTask
            End Get
            Set(ByVal value As Integer)
                Me.intActivationTask = value
            End Set
        End Property
        <DataMember()>
        Public Property ActivationDate() As Nullable(Of DateTime)
            Get
                Return Me.xActivationDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xActivationDate = value
            End Set
        End Property

        <DataMember()>
        Public Property TypeClosing() As TaskTypeClosingEnum
            Get
                Return Me.intTypeClosing
            End Get
            Set(ByVal value As TaskTypeClosingEnum)
                Me.intTypeClosing = value
            End Set
        End Property
        <DataMember()>
        Public Property ClosingDate() As Nullable(Of DateTime)
            Get
                Return Me.xClosingDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xClosingDate = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeAuthorization() As TaskTypeAuthorizationEnum
            Get
                Return Me.intTypeAuthorization
            End Get
            Set(ByVal value As TaskTypeAuthorizationEnum)
                Me.intTypeAuthorization = value
            End Set
        End Property
        <DataMember()>
        Public Property Employees() As Generic.List(Of roEmployeeTaskTemplateDescription)
            Get
                Return Me.oEmployees
            End Get
            Set(ByVal value As Generic.List(Of roEmployeeTaskTemplateDescription))
                Me.oEmployees = value
            End Set
        End Property
        <DataMember()>
        Public Property Groups() As Generic.List(Of roGroupTaskTemplateDescription)
            Get
                Return Me.oGroups
            End Get
            Set(ByVal value As Generic.List(Of roGroupTaskTemplateDescription))
                Me.oGroups = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM TaskTemplates WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.strShortName = Any2String(oRow("ShortName"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.intColor = Any2Integer(oRow("Color"))
                    Me.intIDProject = Any2Integer(oRow("IDProject"))
                    Me.strTag = Any2String(oRow("Tag"))
                    Me.intPriority = Any2Integer(oRow("Priority"))
                    Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                    If Not IsDBNull(oRow("ExpectedStartDate")) Then
                        Me.xExpectedStartDate = oRow("ExpectedStartDate")
                    Else
                        Me.xExpectedStartDate = Nothing
                    End If
                    If Not IsDBNull(oRow("ExpectedEndDate")) Then
                        Me.xExpectedEndDate = oRow("ExpectedEndDate")
                    Else
                        Me.xExpectedEndDate = Nothing
                    End If
                    Me.lngInitialTime = Any2Double(oRow("InitialTime"))

                    Me.intTypeCollaboration = Any2Integer(oRow("TypeCollaboration"))

                    Me.intModeCollaboration = Any2Integer(oRow("ModeCollaboration"))

                    Me.intTypeActivation = Any2Integer(oRow("TypeActivation"))
                    Me.intActivationTask = Any2Integer(oRow("ActivationTask"))

                    If Not IsDBNull(oRow("ActivationDate")) Then
                        Me.xActivationDate = oRow("ActivationDate")
                    Else
                        Me.xActivationDate = Nothing
                    End If

                    Me.intTypeClosing = Any2Integer(oRow("TypeClosing"))

                    If Not IsDBNull(oRow("ClosingDate")) Then
                        Me.xClosingDate = oRow("ClosingDate")
                    Else
                        Me.xClosingDate = Nothing
                    End If

                    Me.intTypeAuthorization = Any2Integer(oRow("TypeAuthorization"))

                    Dim oEmployeeTaskState As New roEmployeeTaskState
                    Me.oEmployees = roEmployeeTaskTemplateDescription.GetEmployeesByTask(oRow("ID"), oEmployeeTaskState)

                    Dim oGroupTaskState As New roGroupTaskState
                    Me.oGroups = roGroupTaskTemplateDescription.GetGroupsByTask(oRow("ID"), oGroupTaskState)

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTaskTemplate, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTaskTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTaskTemplate::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intID = oRow("ID")
                Me.strName = Any2String(oRow("Name"))
                Me.strShortName = Any2String(oRow("ShortName"))
                Me.strDescription = Any2String(oRow("Description"))
                Me.intColor = Any2Integer(oRow("Color"))
                Me.intIDProject = Any2String(oRow("IDProject"))
                Me.strTag = Any2String(oRow("Tag"))
                Me.intPriority = Any2Integer(oRow("Priority"))
                Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                If Not IsDBNull(oRow("ExpectedStartDate")) Then
                    Me.xExpectedStartDate = oRow("ExpectedStartDate")
                Else
                    Me.xExpectedStartDate = Nothing
                End If
                If Not IsDBNull(oRow("ExpectedEndDate")) Then
                    Me.xExpectedEndDate = oRow("ExpectedEndDate")
                Else
                    Me.xExpectedEndDate = Nothing
                End If
                Me.lngInitialTime = Any2Double(oRow("InitialTime"))

                Me.intTypeCollaboration = Any2Integer(oRow("TypeCollaboration"))

                Me.intModeCollaboration = Any2Integer(oRow("ModeCollaboration"))

                Me.intTypeActivation = Any2Integer(oRow("TypeActivation"))
                Me.intActivationTask = Any2Integer(oRow("ActivationTask"))

                If Not IsDBNull(oRow("ActivationDate")) Then
                    Me.xActivationDate = oRow("ActivationDate")
                Else
                    Me.xActivationDate = Nothing
                End If

                Me.intTypeClosing = Any2Integer(oRow("TypeClosing"))

                If Not IsDBNull(oRow("ClosingDate")) Then
                    Me.xClosingDate = oRow("ClosingDate")
                Else
                    Me.xClosingDate = Nothing
                End If

                Me.intTypeAuthorization = Any2Integer(oRow("TypeAuthorization"))

                Dim oEmployeeTaskState As New roEmployeeTaskState
                Me.oEmployees = roEmployeeTaskTemplateDescription.GetEmployeesByTask(oRow("ID"), oEmployeeTaskState)

                Dim oGroupTaskState As New roGroupTaskState
                Me.oGroups = roGroupTaskTemplateDescription.GetGroupsByTask(oRow("ID"), oGroupTaskState)

                bolRet = True

                If _Audit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTaskTemplate, Me.strName, tbParameters, -1)
                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim NewID As Integer

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = TaskResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("TaskTemplate")
                    Dim strSQL As String = "@SELECT# * FROM TaskTemplates " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    If intID = -1 Then
                        NewID = GetNextID()
                        If NewID = -1 Then
                            Me.oState.Result = TaskResultEnum.ErrorGeneratingNewID
                        End If
                        Me.ID = NewID
                        oRow("ID") = Me.ID
                    End If

                    oRow("Name") = Me.strName
                    oRow("ShortName") = Me.strShortName
                    oRow("Description") = Me.strDescription
                    oRow("Color") = Me.intColor
                    oRow("IDProject") = Me.intIDProject
                    oRow("Tag") = Me.strTag
                    oRow("Priority") = Me.intPriority
                    oRow("IDPassport") = Me.intIDPassport

                    If Me.xExpectedStartDate.HasValue Then
                        oRow("ExpectedStartDate") = Me.xExpectedStartDate.Value
                    Else
                        oRow("ExpectedStartDate") = DBNull.Value
                    End If

                    If Me.xExpectedEndDate.HasValue Then
                        oRow("ExpectedEndDate") = Me.xExpectedEndDate.Value
                    Else
                        oRow("ExpectedEndDate") = DBNull.Value
                    End If

                    oRow("InitialTime") = Me.InitialTime

                    oRow("TypeCollaboration") = Me.intTypeCollaboration

                    oRow("ModeCollaboration") = Me.intModeCollaboration

                    If Me.intModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME Then
                        oRow("TypeCollaboration") = TaskTypeCollaborationEnum._ANY
                    End If

                    oRow("TypeActivation") = Me.intTypeActivation

                    oRow("ActivationTask") = Me.intActivationTask

                    If Me.xActivationDate.HasValue Then
                        oRow("ActivationDate") = Me.xActivationDate.Value
                    Else
                        oRow("ActivationDate") = DBNull.Value
                    End If

                    oRow("TypeClosing") = Me.intTypeClosing

                    If Me.xClosingDate.HasValue Then
                        oRow("ClosingDate") = Me.xClosingDate.Value
                    Else
                        oRow("ClosingDate") = DBNull.Value
                    End If

                    oRow("TypeAuthorization") = Me.intTypeAuthorization

                    If tb.Rows.Count <= 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    'Borramos empleados y grupos asignados
                    Dim DeleteQuerys() As String = {"@DELETE# FROM EmployeeTaskTemplates WHERE IDTask = " & Me.intID.ToString, "@DELETE# FROM GroupTaskTemplates WHERE IDTask = " & Me.intID.ToString}
                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    'Asignamos los empleados y grupos en caso necesario
                    If bolRet And Me.intTypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES Then
                        If Me.oEmployees IsNot Nothing Then
                            If Me.oEmployees.Count > 0 Then
                                For Each oEmp As roEmployeeTaskTemplateDescription In oEmployees
                                    bolRet = ExecuteSql("@INSERT# INTO EmployeeTaskTemplates (IDEmployee, IDTask) VALUES(" & oEmp.ID.ToString & "," & Me.intID.ToString & ")")
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If

                        'Asignamos los nuevos grupos
                        If bolRet Then
                            If Me.oGroups IsNot Nothing Then
                                If Me.oGroups.Count > 0 Then
                                    For Each oGro As roGroupTaskTemplateDescription In oGroups
                                        bolRet = ExecuteSql("@INSERT# INTO GroupTaskTemplates (IDGroup, IDTask) VALUES(" & oGro.ID.ToString & "," & Me.intID.ToString & ")")
                                        If Not bolRet Then Exit For
                                    Next
                                End If
                            End If
                        End If
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name")
                        End If
                        Me.oState.Audit(oAuditAction, Audit.ObjectType.tTaskTemplate, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskTemplate::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskTemplate::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim cmd As DbCommand
                Dim da As DbDataAdapter

                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = TaskResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El nombre corto no puede estar en blanco
                'If Me.ShortName = "" Or Trim(Me.ShortName) = "" Then
                '    oState.Result = TaskResultEnum.ShortNameCannotBeNull
                '    bolRet = False
                '    Return False
                'End If

                If Me.IDProject <= 0 Then
                    oState.Result = TaskResultEnum.ProjectTemplateNotEmpty
                    bolRet = False
                    Return False
                End If

                If bolRet And bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM TaskTemplates " &
                             "WHERE Name = @TaskName AND " &
                                " IDProject = " & Me.IDProject.ToString &
                                " and ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@TaskName", DbType.String, 64)
                    cmd.Parameters("@TaskName").Value = Me.Name

                    'AddParameter(cmd, "@IDCenter", DbType.Int32, 32)
                    'cmd.Parameters("@IDCenter").Value = Me.IDCenter

                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = TaskResultEnum.NameAlreadyExist
                        bolRet = False
                    End If

                    'If bolRet Then
                    '    ' Compuebo que el nombre corto y el color no exista
                    '    tb = New DataTable()
                    '    strSQL = "@SELECT# * FROM TaskTemplates " & _
                    '             "WHERE ShortName = @ShortName AND " & _
                    '             "IDProject = " & Me.IDProject.ToString & _
                    '             " AND ID <> " & Me.ID.ToString
                    '    cmd = CreateCommand(strSQL)
                    '    AddParameter(cmd, "@ShortName", DbType.String, 64)
                    '    cmd.Parameters("@ShortName").Value = Me.ShortName

                    '    da = CreateDataAdapter(cmd, True)
                    '    tb.Rows.Clear()
                    '    da.Fill(tb)

                    '    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    '        oState.Result = TaskResultEnum.ShortNameAlreadyExist
                    '        bolRet = False
                    '    End If
                    'End If
                End If

                If bolRet Then
                    'If (Me.InitialTime + Me.EmployeeTime + Me.MaterialTime + Me.NonProductiveTimeIncidence + Me.OtherTime + Me.TeamTime + Me.TimeChangedRequirements + Me.ForecastErrorTime) < 0 Then
                    If Me.InitialTime < 0 Then
                        oState.Result = TaskResultEnum.NegativeTime
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskTemplates::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskTemplates::Validate")

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra la tarea siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim sSql As String = "@DELETE# FROM TaskTemplates WHERE ID = " & Me.ID.ToString
            Dim strArray As New ArrayList
            strArray.Add("@DELETE# FROM EmployeeTaskTemplates WHERE IDTask = " & Me.ID.ToString)
            strArray.Add("@DELETE# FROM GroupTaskTemplates WHERE IDTask = " & Me.ID.ToString)
            strArray.Add("@DELETE# FROM FieldsTaskTemplate WHERE IDTaskTemplate = " & Me.ID.ToString)
            strArray.Add("@DELETE# TaskTemplates WHERE ID= " & intID)

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                For Each strSql As String In strArray
                    bolRet = ExecuteSql(strSql)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos borrado de tarea
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{TaskTemplateName}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTaskTemplate, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTaskTemplates::DeleteTask")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskTemplates::DeleteTask")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = 1

            Try

                Dim strSql As String = "@SELECT# Max(ID) as counter From TaskTemplates"
                Dim tb As DataTable = CreateDataTable(strSql)

                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Counter")) Then
                        intRet = tb.Rows(0)("Counter") + 1
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "TaskTemplates::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "TaskTemplates::GetNextID")
            End Try

            Return intRet

        End Function

        ''' <summary>
        ''' Verifica si la tarea se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                Dim strSQL As String = ""
                Dim tb As DataTable = Nothing
                Dim strUsed As String = ""

                'pendiente
                If Not bolIsUsed Then

                End If

                bolIsUsed = False
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskTemplates::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskTemplates::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetTasksByProject(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roTaskTemplate)

            Dim oRet As New Generic.List(Of roTaskTemplate)

            Try

                Dim strSQL As String = "@SELECT# TaskTemplates.* FROM TaskTemplates "
                strSQL &= "  WHERE IDProject= " & _IDProject.ToString
                If _IDsFilter <> "" Then strSQL &= " AND TaskTemplates.ID IN(" & _IDsFilter & ")"
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roTaskTemplate(oRow, _State, _Audit))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskTemplates::GetTasksByProject")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskTemplates::GetTasksByProject")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTasksByProjectDataTable(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TaskTemplates.* FROM TaskTemplates "
                strSQL &= " WHERE IDProject= " & _IDProject.ToString
                If _IDsFilter <> "" Then strSQL &= " AND TaskTemplates.ID IN(" & _IDsFilter & ")"
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                tbRet = CreateDataTable(strSQL, )
                If tbRet IsNot Nothing Then

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskTemplates::GetTasksByProjectDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskTemplates::GetTasksByProjectDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Devuelve el nº de empleados asignados a una tarea o los empleados seleccionados a partir de los id's de grupo y empleado
        ''' </summary>
        ''' <param name="IDTaskTemplate">ID de la tarea</param>
        ''' <param name="sEmployees">ID's de los empleados</param>
        ''' <param name="sGroups">ID's de los grupos</param>
        '''<returns>Nº de empleados asignados a la tarea o seleccionados</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesFromTask(ByVal IDTaskTemplate As Integer, ByVal sEmployees As String, ByVal sGroups As String) As Double
            Dim oRet As Double = 0

            Try

                Dim strIDGroups As String = ""
                Dim strPath As String = ""
                Dim strEmployees As String = ""
                Dim i As Integer
                Dim oTask As New roTaskTemplate

                If IDTaskTemplate > 0 Then
                    oTask.ID = IDTaskTemplate
                    oTask.Load()
                    For i = 0 To oTask.Employees.Count - 1
                        strEmployees = IIf(strEmployees.Length = 0, oTask.Employees(i).ID, strEmployees & "," & oTask.Employees(i).ID)
                    Next

                    For i = 0 To oTask.Groups.Count - 1
                        Dim oGroup As New Group.roGroup
                        oGroup.ID = oTask.Groups(i).ID
                        oGroup.Load()
                        strIDGroups = IIf(strIDGroups.Length = 0, oTask.Groups(i).ID, strIDGroups & "," & oTask.Groups(i).ID)
                        If oGroup.Path.Length > 0 Then
                            strPath = IIf(strPath.Length = 0, "Path like '" & oGroup.Path & "\%'", strPath & " OR Path like '" & oGroup.Path & "\%'")
                        End If
                    Next
                Else
                    strEmployees = sEmployees
                    strIDGroups = sGroups
                    For i = 0 To StringItemsCount(strIDGroups, ",") - 1
                        Dim oGroup As New Group.roGroup
                        oGroup.ID = String2Item(strIDGroups, i, ",")
                        oGroup.Load()
                        If oGroup.Path.Length > 0 Then
                            strPath = IIf(strPath.Length = 0, "Path like '" & oGroup.Path & "\%'", strPath & " OR Path like '" & oGroup.Path & "\%'")
                        End If
                    Next
                End If

                If strPath.Length = 0 Then strPath = "Path ='-1'"
                If strIDGroups.Length = 0 Then strIDGroups = "-1"
                If strEmployees.Length = 0 Then strEmployees = "-1"

                Dim strQuery As String
                strQuery = "@SELECT# COUNT(IDEmployee) FROM sysrovwCurrentEmployeeGroups, Employees "
                strQuery = strQuery & " WHERE sysrovwCurrentEmployeeGroups.IDEmployee = Employees.ID AND (IDGroup IN (" & strIDGroups & ") OR (" & strPath & "))"
                strQuery = strQuery & " AND IDEmployee NOT IN(" & strEmployees & ") AND Type = 'J'"
                oRet = Any2Double(ExecuteScalar(strQuery))

                strQuery = "@SELECT# COUNT(IDEmployee) FROM sysrovwCurrentEmployeeGroups, Employees "
                strQuery = strQuery & " WHERE sysrovwCurrentEmployeeGroups.IDEmployee = Employees.ID  "
                strQuery = strQuery & " AND IDEmployee  IN(" & strEmployees & ") AND Type = 'J'"
                oRet = oRet + Any2Double(ExecuteScalar(strQuery))
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el nombre de una tarea a partir de su ID.
        ''' </summary>
        ''' <param name="_IDTaskTemplate">Código de tarea del que se quiere realizar la copia</param>
        ''' <param name="_State"></param>
        ''' <returns>Nombre de la tarea</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskName(ByVal _IDTaskTemplate As Integer, ByVal _State As roTaskState) As String

            Dim oRet As String = ""

            Try

                Dim strSQL As String = "@SELECT# (TaskTemplates.Name) as Name   " &
                                       "FROM TaskTemplates WHERE ID=" & _IDTaskTemplate.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    oRet = Any2String(oRow("Name"))
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTaskName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTaskName")
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function SaveTasksFromTemplates(ByVal _IDPassport As Integer, ByVal tbTaskTemplates As DataTable, ByVal _State As roTaskState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                tbTaskTemplates.Columns.Add(New DataColumn("IDRealTask", GetType(Integer)))

                Dim oTask As roTask
                Dim bolSaved As Boolean = True

                bolRet = True

                For Each oRow As DataRow In tbTaskTemplates.Rows

                    bolSaved = True

                    oTask = New roTask()
                    oTask.ID = -1

                    oTask.IDCenter = Any2Integer(oRow("IDBusinessCenter"))
                    oTask.Name = Any2String(oRow("Name"))
                    oTask.ShortName = Any2String(oRow("ShortName"))
                    oTask.Description = Any2String(oRow("Description"))
                    oTask.Status = TaskStatusEnum._ON
                    oTask.Color = Any2Integer(oRow("Color"))
                    oTask.Project = Any2String(oRow("ProjectName"))
                    oTask.Tag = Any2String(oRow("Tag"))
                    oTask.Priority = Any2Integer(oRow("Priority"))
                    oTask.Passport = _IDPassport

                    If Not IsDBNull(oRow("InitialDate")) Then
                        oTask.ExpectedStartDate = oRow("InitialDate")
                        Dim hourValues As String() = Any2String(oRow("InitialDateTime")).Split(":")
                        oTask.ExpectedStartDate = oTask.ExpectedStartDate.Value.AddHours(Any2Double(hourValues(0)))
                        oTask.ExpectedStartDate = oTask.ExpectedStartDate.Value.AddMinutes(Any2Double(hourValues(1)))
                    Else
                        oTask.ExpectedStartDate = Nothing
                    End If
                    If Not IsDBNull(oRow("EndDate")) Then
                        oTask.ExpectedEndDate = oRow("EndDate")
                        Dim hourValues As String() = Any2String(oRow("EndDateTime")).Split(":")
                        oTask.ExpectedEndDate = oTask.ExpectedEndDate.Value.AddHours(Any2Double(hourValues(0)))
                        oTask.ExpectedEndDate = oTask.ExpectedEndDate.Value.AddMinutes(Any2Double(hourValues(1)))
                    Else
                        oTask.ExpectedEndDate = Nothing
                    End If

                    If Not IsDBNull(oRow("ActivateByDateDate")) Then
                        oTask.ActivationDate = oRow("ActivateByDateDate")
                        Dim hourValues As String() = Any2String(oRow("ActivateByDateTime")).Split(":")
                        oTask.ActivationDate = oTask.ActivationDate.Value.AddHours(Any2Double(hourValues(0)))
                        oTask.ActivationDate = oTask.ActivationDate.Value.AddMinutes(Any2Double(hourValues(1)))
                    Else
                        oTask.ActivationDate = Nothing
                    End If

                    If Not IsDBNull(oRow("CloseByDateDate")) Then
                        oTask.ClosingDate = oRow("CloseByDateDate")
                        Dim hourValues As String() = Any2String(oRow("CloseByDateTime")).Split(":")
                        oTask.ClosingDate = oTask.ClosingDate.Value.AddHours(Any2Double(hourValues(0)))
                        oTask.ClosingDate = oTask.ClosingDate.Value.AddMinutes(Any2Double(hourValues(1)))
                    Else
                        oTask.ClosingDate = Nothing
                    End If

                    oTask.InitialTime = roConversions.ConvertTimeToHours(Any2String(oRow("InitialDuration")))
                    oTask.TypeCollaboration = Any2Integer(oRow("TypeCollaboration"))
                    oTask.ModeCollaboration = Any2Integer(oRow("ModeCollaboration"))
                    oTask.TypeActivation = Any2Integer(oRow("TypeActivation"))
                    'oTask.ActivationTask = Any2Integer(oRow("IDSelectedTask"))
                    oTask.TypeClosing = Any2Integer(oRow("TypeClosing"))
                    oTask.TypeAuthorization = Any2Integer(oRow("TypeAuthorization"))
                    If Not IsDBNull(oRow("Barcode")) Then oTask.BarCode = oRow("Barcode")

                    Dim oEmployeeTaskState As New roEmployeeTaskState
                    Dim empList As New Generic.List(Of roEmployeeTaskTemplateDescription)
                    empList = roEmployeeTaskTemplateDescription.GetEmployeesByTask(oRow("ID"), oEmployeeTaskState)

                    oTask.Employees = New Generic.List(Of roEmployeeTaskDescription)
                    For Each elem As roEmployeeTaskTemplateDescription In empList
                        Dim newElem As roEmployeeTaskDescription
                        newElem = New roEmployeeTaskDescription(elem.ID, oEmployeeTaskState)
                        oTask.Employees.Add(newElem)
                    Next

                    Dim oGroupTaskState As New roGroupTaskState
                    Dim groupList As New Generic.List(Of roGroupTaskTemplateDescription)
                    groupList = roGroupTaskTemplateDescription.GetGroupsByTask(oRow("ID"), oGroupTaskState)

                    oTask.Groups = New Generic.List(Of roGroupTaskDescription)
                    For Each elem As roGroupTaskTemplateDescription In groupList
                        Dim newElem As roGroupTaskDescription
                        newElem = New roGroupTaskDescription(elem.ID, oGroupTaskState)
                        oTask.Groups.Add(newElem)
                    Next

                    bolRet = oTask.Save(True)
                    If bolRet Then
                        oRow("IDRealTask") = oTask.ID
                    Else
                        roBusinessState.CopyTo(oTask.State, _State)
                        Exit For
                    End If

                Next

                If (bolRet) Then
                    For Each oRow As DataRow In tbTaskTemplates.Rows
                        If (Any2Integer(oRow("IDSelectedTask")) > 0) Then
                            Dim pTaskRow As DataRow() = tbTaskTemplates.Select("ID = " & oRow("IDSelectedTask"))
                            If pTaskRow.Length = 1 Then
                                Dim activateTaskId As Integer = Any2Integer(pTaskRow(0)("IDRealTask"))

                                Dim taskId As Integer = Any2Integer(oRow("IDRealTask"))

                                Dim updateTask As New roTask
                                updateTask.ID = taskId
                                updateTask.Load(False)
                                updateTask.ActivationTask = activateTaskId
                                bolRet = updateTask.Save(False)

                                If Not bolRet Then
                                    roBusinessState.CopyTo(updateTask.State, _State)
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roTaskTemplates::SaveTasksFromTemplates")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roTaskTemplates::SaveTasksFromTemplates")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roEmployeeTaskTemplateDescription
        Private oState As roEmployeeTaskState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roEmployeeTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roEmployeeTaskState)
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
            Me.oState = New roEmployeeTaskState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roEmployeeTaskState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID,Name FROM Employees " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                Else

                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roEmployeeTaskTemplateDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTaskTemplateDescription::Load")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper Methods"

        Public Shared Function GetEmployeesByTask(ByVal IDTask As Integer, ByVal _State As roEmployeeTaskState) As Generic.List(Of roEmployeeTaskTemplateDescription)

            Dim oRet As New Generic.List(Of roEmployeeTaskTemplateDescription)

            Try

                Dim strSQL As String = "@SELECT# IDEmployee FROM EmployeeTaskTemplates, Employees Where Employees.ID = EmployeeTaskTemplates.IDEmployee AND  IDTask = " & IDTask & " Order by Employees.Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oEmployeeDesc As roEmployeeTaskTemplateDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oEmployeeDesc = New roEmployeeTaskTemplateDescription(oRow("IDEmployee"), _State)
                        oRet.Add(oEmployeeDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEmployeeTaskTemplateDesctiption::GetEmployeesByTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeTaskTemplateDesctiption::GetEmployeesByTask")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roGroupTaskTemplateDescription
        Private oState As roGroupTaskState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roGroupTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roGroupTaskState)
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
            Me.oState = New roGroupTaskState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roGroupTaskState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID,Name FROM Groups " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                Else

                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupTaskTemplateDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupTaskTemplateDescription::Load")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper Methods"

        Public Shared Function GetGroupsByTask(ByVal IDTask As Integer, ByVal _State As roGroupTaskState) As Generic.List(Of roGroupTaskTemplateDescription)

            Dim oRet As New Generic.List(Of roGroupTaskTemplateDescription)

            Try

                Dim strSQL As String = "@SELECT# IDGroup FROM GroupTaskTemplates,Groups Where Groups.ID = GroupTaskTemplates.IDGroup AND  IDTask = " & IDTask & " Order by Groups.Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oGroupDesc As roGroupTaskTemplateDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oGroupDesc = New roGroupTaskTemplateDescription(oRow("IDGroup"), _State)
                        oRet.Add(oGroupDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roGroupTaskTemplateDesctiption::GetGroupsByTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupTaskTemplateDesctiption::GetGroupsByTask")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roProjectTemplates

#Region "Declarations - Constructor"

        Private oState As roTaskState

        Private intID As Integer
        Private strProject As String
        Private intIDPassport As Integer
        Private intIDCenter As Integer

        Private _Audit As Boolean = False

        Public Sub New()
            Me.oState = New roTaskState
            Me.strProject = ""
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _State As roTaskState)
            Me.oState = _State
            Me.strProject = ""
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Project As String, ByVal _State As roTaskState, ByVal _IDPassport As Integer, Optional ByVal bAudit As Boolean = False)
            Me._Audit = bAudit
            Me.oState = _State
            Me.ID = _ID
            Me.Project = _Project
            Me.Passport = _IDPassport
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

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
        Public Property Project() As String
            Get
                Return strProject
            End Get
            Set(ByVal value As String)
                strProject = value
            End Set
        End Property
        <DataMember()>
        Public Property Passport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property
        <DataMember()>
        Public Property IDCenter() As Integer
            Get
                Return Me.intIDCenter
            End Get
            Set(ByVal value As Integer)
                Me.intIDCenter = value
            End Set
        End Property
        <IgnoreDataMember()>
        Public Property State() As roTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# * FROM ProjectTemplates WHERE [ID] = " & Me.intID.ToString)
                If tb.Rows.Count > 0 Then

                    Me.strProject = tb.Rows(0).Item("Project")
                    Me.intIDPassport = roTypes.Any2String(tb.Rows(0).Item("IDPassport"))
                    Me.intIDCenter = Any2Integer(tb.Rows(0).Item("IDCenter"))

                    ' Auditamos consulta plantilla de proyecto
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ProjectTemplate}", Me.Project, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProjectTemplate, Me.Project, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProjectTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProjectTemplate::Load")
            End Try

        End Sub

        Public Function ValidateProjectTemplate() As Boolean

            Dim bolRet As Boolean = False

            Try
                ' El nombre no puede estar en blanco
                If Me.Project = "" Then
                    oState.Result = TaskResultEnum.NameCannotBeNull
                    bolRet = False
                ElseIf Me.IDCenter = 0 Then
                    oState.Result = TaskResultEnum.CenterCannotBeNull
                    bolRet = False
                    Return False
                Else
                    Dim strSQL As String = "@SELECT# Project FROM ProjectTemplates WHERE Project = '" & Replace(Me.strProject, "'", "''") & "' AND ID <> " & Me.intID
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    bolRet = (tb.Rows.Count = 0)
                    If Not bolRet Then
                        oState.Result = TaskResultEnum.NameAlreadyExist
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProjectTemplate::ExistName")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strQueryRow As String = ""
                Dim oProjectOld As DataRow = Nothing
                Dim oProjectNew As DataRow = Nothing

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = TaskResultEnum.XSSvalidationError
                    Return False
                End If


                If ValidateProjectTemplate() Then

                    strQueryRow = "@SELECT# Project, IDPassport, ID " &
                                  "FROM ProjectTemplates WHERE [ID] = " & Me.intID.ToString
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "ProjectTemplates")
                    If tbAuditOld.Rows.Count = 1 Then oProjectOld = tbAuditOld.Rows(0)

                    Dim tbGroup As New DataTable("ProjectTemplates")
                    Dim strSQL As String = "@SELECT# * FROM ProjectTemplates WHERE [ID] = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbGroup)

                    Dim oRow As DataRow = Nothing
                    If Me.intID <= 0 Then
                        oRow = tbGroup.NewRow
                        oRow("ID") = Me.GetNextIDProjectTemplate()
                    ElseIf tbGroup.Rows.Count = 1 Then
                        oRow = tbGroup.Rows(0)
                    End If

                    oRow("Project") = Me.strProject
                    oRow("IDPassport") = Me.intIDPassport
                    oRow("IDCenter") = Me.intIDCenter

                    If Me.intID <= 0 Then
                        tbGroup.Rows.Add(oRow)
                    End If

                    da.Update(tbGroup)

                    If Me.intID <= 0 Then
                        Me.intID = oRow("ID")
                    End If

                    If bAudit Then
                        strQueryRow = "@SELECT# ID, Project " &
                                      "FROM ProjectTemplates WHERE [ID] = " & Me.intID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "ProjectTemplates")
                        If tbAuditNew.Rows.Count = 1 Then oProjectNew = tbAuditNew.Rows(0)

                        ' Insertar registro auditoria
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Dim oAuditAction As Audit.Action = IIf(oProjectOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        oState.AddAuditFieldsValues(tbParameters, oProjectNew, oProjectOld)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oProjectNew("Project")
                        ElseIf oProjectOld("Project") <> oProjectNew("Project") Then
                            strObjectName = oProjectOld("Project") & " -> " & oProjectNew("Project")
                        Else
                            strObjectName = oProjectNew("Project")
                        End If
                        oState.Audit(oAuditAction, Audit.ObjectType.tProjectTemplate, strObjectName, tbParameters, -1)
                    End If

                    bolRet = True

                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roProjectTemplate::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProjectTemplate::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Me.oState.UpdateStateInfo()

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim dSet As DataSet = roProjectTemplates.GetTasksFromProject(Me.ID, oState)
                If dSet IsNot Nothing Then
                    If dSet.Tables(0).Rows.Count = 0 Then
                        Dim strSql As String = """"

                        strSql = "@DELETE# FieldsProjectTemplate WHERE IDProjectTemplate = " & Me.ID
                        bolRet = ExecuteSql(strSql)

                        If bolRet Then
                            strSql = "@DELETE# ProjectTemplates WHERE ID = " & Me.ID
                            bolRet = ExecuteSql(strSql)
                        End If

                        If bolRet And bAudit Then
                            ' Auditamos borrado proyecto
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{ProjectTemplateName}", Me.Project, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProjectTemplate, Me.Project, tbParameters, -1)
                        End If
                    Else
                        oState.Result = TaskResultEnum.ProjectTemplateNotEmpty
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roProjectTemplate::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProjectTemplate::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function GetNextIDProjectTemplate() As Integer

            ' Busca el siguiente ID de grupo.
            Dim intRet As Integer = 1

            Dim strQuery As String = " @SELECT# Max(ID) as Contador From ProjectTemplates "
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item("Contador")) Then
                    intRet = tb.Rows(0).Item("Contador") + 1
                End If
            End If

            Return intRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un dataset con los proyectos disponibles
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjects(ByVal oState As roTaskState, ByVal _Order As String) As System.Data.DataSet
            Dim oDataset As System.Data.DataSet

            Try
                Dim strQuery As String = "@SELECT# * From ProjectTemplates"
                If (_Order <> "") Then strQuery = strQuery & " Order by " & _Order
                oDataset = CreateDataSet(strQuery, "ProjectTemplates")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProjectTemplates::GetTasksFromProject")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProjectTemplates::GetTasksFromProject")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        ''' <summary>
        ''' Devuelve un dataset con las tareas que pertenecen al proyecto
        ''' </summary>
        ''' <param name="IDProject">ID del grupo a recuperar las tareas</param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTasksFromProject(ByVal IDProject As Integer, ByRef oState As roTaskState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# * from TaskTemplates "
                strQuery = strQuery & " Where IDProject = " & IDProject
                strQuery = strQuery & " Order By Name "

                oRet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roProjectTemplate::GetTasksFromProject")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProjectTemplate::GetTasksFromProject")
            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace