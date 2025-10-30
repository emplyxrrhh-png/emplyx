Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Web
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Task

    <DataContract()>
    Public Class roTask

#Region "Declarations - Constructors"

        Private oState As roTaskState

        Private intID As Integer
        Private intIDCenter As Integer
        Private strName As String
        Private strBarcode As String
        Private strShortName As String
        Private strDescription As String
        Private strTimeRemarks As String
        Private intIDEmployeeUpdateStatus As Nullable(Of Integer)
        Private xUpdateStatusDate As Nullable(Of Date)
        Private intStatus As TaskStatusEnum
        Private intColor As Integer
        Private strProject As String
        Private strTag As String
        Private intPriority As Integer
        Private intIDPassport As Integer
        Private xExpectedStartDate As Nullable(Of Date)
        Private xExpectedEndDate As Nullable(Of Date)
        Private xStartDate As Nullable(Of Date)
        Private xEndDate As Nullable(Of Date)
        Private lngInitialTime As Double
        Private lngTimeChangedRequirements As Double
        Private lngForecastErrorTime As Double
        Private lngNonProductiveTimeIncidence As Double
        Private lngEmployeeTime As Double
        Private lngTeamTime As Double
        Private lngMaterialTime As Double
        Private lngOtherTime As Double

        Private intTypeCollaboration As TaskTypeCollaborationEnum

        Private intModeCollaboration As TaskModeCollaborationEnum

        Private intTypeActivation As TaskTypeActivationEnum

        Private intActivationTask As Integer
        Private xActivationDate As Nullable(Of Date)

        Private intTypeClosing As TaskTypeClosingEnum

        Private xClosingDate As Nullable(Of Date)
        Private intTypeAuthorization As TaskTypeAuthorizationEnum

        Private oEmployees As Generic.List(Of roEmployeeTaskDescription)
        Private oGroups As Generic.List(Of roGroupTaskDescription)

        Private mTaskNotRequiresApprovalToBeCompleted As Boolean = False

        Public Sub New()

            Me.oState = New roTaskState
            Me.intID = -1

            Me.intModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
            Me.intTypeActivation = TaskTypeActivationEnum._ALWAYS
            Me.intTypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
            Me.intTypeClosing = TaskTypeClosingEnum._UNDEFINED
            Me.intTypeCollaboration = TaskTypeCollaborationEnum._ANY
            Me.LoadAdvancedParameters()

        End Sub

        Public Sub New(ByVal _ID As Long, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID
            Me.LoadAdvancedParameters()
            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.LoadAdvancedParameters()
            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember()>
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
        Public Property IDCenter() As Integer
            Get
                Return Me.intIDCenter
            End Get
            Set(ByVal value As Integer)
                Me.intIDCenter = value
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
        Public Property BarCode() As String
            Get
                Return Me.strBarcode
            End Get
            Set(ByVal value As String)
                Me.strBarcode = value
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
        Public Property TimeRemarks() As String
            Get
                Return Me.strTimeRemarks
            End Get
            Set(ByVal value As String)
                Me.strTimeRemarks = value
            End Set
        End Property

        <DataMember()>
        Public Property Status() As TaskStatusEnum
            Get
                Return Me.intStatus
            End Get
            Set(ByVal value As TaskStatusEnum)
                Me.intStatus = value
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
        Public Property Project() As String
            Get
                Return Me.strProject
            End Get
            Set(ByVal value As String)
                Me.strProject = value
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
        Public Property UpdateStatusDate() As Nullable(Of DateTime)
            Get
                Return Me.xUpdateStatusDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xUpdateStatusDate = value
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
        Public Property StartDate() As Nullable(Of DateTime)
            Get
                Return Me.xStartDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xStartDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate() As Nullable(Of DateTime)
            Get
                Return Me.xEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xEndDate = value
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
        Public Property TimeChangedRequirements() As Double
            Get
                Return Me.lngTimeChangedRequirements
            End Get
            Set(ByVal value As Double)
                Me.lngTimeChangedRequirements = value
            End Set
        End Property
        <DataMember()>
        Public Property ForecastErrorTime() As Double
            Get
                Return Me.lngForecastErrorTime
            End Get
            Set(ByVal value As Double)
                Me.lngForecastErrorTime = value
            End Set
        End Property
        <DataMember()>
        Public Property NonProductiveTimeIncidence() As Double
            Get
                Return Me.lngNonProductiveTimeIncidence
            End Get
            Set(ByVal value As Double)
                Me.lngNonProductiveTimeIncidence = value
            End Set
        End Property
        <DataMember()>
        Public Property EmployeeTime() As Double
            Get
                Return Me.lngEmployeeTime
            End Get
            Set(ByVal value As Double)
                Me.lngEmployeeTime = value
            End Set
        End Property
        <DataMember()>
        Public Property TeamTime() As Double
            Get
                Return Me.lngTeamTime
            End Get
            Set(ByVal value As Double)
                Me.lngTeamTime = value
            End Set
        End Property
        <DataMember()>
        Public Property MaterialTime() As Double
            Get
                Return Me.lngMaterialTime
            End Get
            Set(ByVal value As Double)
                Me.lngMaterialTime = value
            End Set
        End Property
        <DataMember()>
        Public Property OtherTime() As Double
            Get
                Return Me.lngOtherTime
            End Get
            Set(ByVal value As Double)
                Me.lngOtherTime = value
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
        Public Property Employees() As Generic.List(Of roEmployeeTaskDescription)
            Get
                Return Me.oEmployees
            End Get
            Set(ByVal value As Generic.List(Of roEmployeeTaskDescription))
                Me.oEmployees = value
            End Set
        End Property
        <DataMember()>
        Public Property Groups() As Generic.List(Of roGroupTaskDescription)
            Get
                Return Me.oGroups
            End Get
            Set(ByVal value As Generic.List(Of roGroupTaskDescription))
                Me.oGroups = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployeeUpdateStatus() As Nullable(Of Integer)
            Get
                Return Me.intIDEmployeeUpdateStatus
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDEmployeeUpdateStatus = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Sub LoadAdvancedParameters()
            Try
                mTaskNotRequiresApprovalToBeCompleted = (roTypes.Any2Integer(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("Tasks.TaskNotRequiresApprovalToBeCompleted", Nothing)) = 1)
            Catch ex As Exception
                mTaskNotRequiresApprovalToBeCompleted = False
            End Try
        End Sub

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Tasks WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intIDCenter = Any2Integer(oRow("IDCenter"))
                    Me.strName = Any2String(oRow("Name"))
                    If Me.intID = 0 Then
                        If Me.State IsNot Nothing Then
                            Me.strName = Me.State.Language.Translate("Tasks.WhithoutTask.Name", "")
                        Else
                            Me.strName = "Sin tarea"
                        End If

                    End If

                    Me.strBarcode = Any2String(oRow("BarCode"))
                    Me.strShortName = Any2String(oRow("ShortName"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.intStatus = Any2Integer(oRow("Status"))
                    Me.intColor = Any2Integer(oRow("Color"))
                    Me.strProject = Any2String(oRow("Project"))
                    Me.strTag = Any2String(oRow("Tag"))
                    Me.intPriority = Any2Integer(oRow("Priority"))
                    Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                    If Not IsDBNull(oRow("ExpectedStartDate")) Then
                        Me.xExpectedStartDate = oRow("ExpectedStartDate")
                    Else
                        Me.xExpectedStartDate = Nothing
                    End If

                    If Not IsDBNull(oRow("UpdateStatusDate")) Then
                        Me.xUpdateStatusDate = oRow("UpdateStatusDate")
                    Else
                        Me.xUpdateStatusDate = Nothing
                    End If

                    If Not IsDBNull(oRow("ExpectedEndDate")) Then
                        Me.xExpectedEndDate = oRow("ExpectedEndDate")
                    Else
                        Me.xExpectedEndDate = Nothing
                    End If
                    If Not IsDBNull(oRow("StartDate")) Then
                        Me.xStartDate = oRow("StartDate")
                    Else
                        Me.xStartDate = Nothing
                    End If
                    If Not IsDBNull(oRow("EndDate")) Then
                        Me.xEndDate = oRow("EndDate")
                    Else
                        Me.xEndDate = Nothing
                    End If

                    Me.lngInitialTime = Any2Double(oRow("InitialTime"))

                    Me.lngTimeChangedRequirements = Any2Double(oRow("TimeChangedRequirements"))
                    Me.lngForecastErrorTime = Any2Double(oRow("ForecastErrorTime"))
                    Me.lngNonProductiveTimeIncidence = Any2Double(oRow("NonProductiveTimeIncidence"))
                    Me.lngEmployeeTime = Any2Double(oRow("EmployeeTime"))
                    Me.lngTeamTime = Any2Double(oRow("TeamTime"))
                    Me.lngMaterialTime = Any2Double(oRow("MaterialTime"))
                    Me.lngOtherTime = Any2Double(oRow("OtherTime"))

                    Me.strTimeRemarks = Any2String(oRow("TimeRemarks"))

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

                    If Not IsDBNull(oRow("IDEmployeeUpdateStatus")) Then
                        Me.intIDEmployeeUpdateStatus = Any2Integer(oRow("IDEmployeeUpdateStatus"))
                    Else
                        Me.intIDEmployeeUpdateStatus = Nothing
                    End If

                    Me.intTypeAuthorization = Any2Integer(oRow("TypeAuthorization"))

                    Dim oEmployeeTaskState As New roEmployeeTaskState
                    Me.oEmployees = roEmployeeTaskDescription.GetEmployeesByTask(oRow("ID"), oEmployeeTaskState)

                    Dim oGroupTaskState As New roGroupTaskState
                    Me.oGroups = roGroupTaskDescription.GetGroupsByTask(oRow("ID"), oGroupTaskState)

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTask, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTask::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTask::Load")
            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intID = oRow("ID")
                Me.intIDCenter = Any2Integer(oRow("IDCenter"))
                Me.strName = Any2String(oRow("Name"))
                Me.strBarcode = Any2String(oRow("BarCode"))
                Me.strShortName = Any2String(oRow("ShortName"))
                Me.strDescription = Any2String(oRow("Description"))
                Me.intStatus = Any2Integer(oRow("Status"))
                Me.intColor = Any2Integer(oRow("Color"))
                Me.strProject = Any2String(oRow("Project"))
                Me.strTag = Any2String(oRow("Tag"))
                Me.intPriority = Any2Integer(oRow("Priority"))
                Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                If Not IsDBNull(oRow("ExpectedStartDate")) Then
                    Me.xExpectedStartDate = oRow("ExpectedStartDate")
                Else
                    Me.xExpectedStartDate = Nothing
                End If

                If Not IsDBNull(oRow("UpdateStatusDate")) Then
                    Me.xUpdateStatusDate = oRow("UpdateStatusDate")
                Else
                    Me.xUpdateStatusDate = Nothing
                End If

                If Not IsDBNull(oRow("ExpectedEndDate")) Then
                    Me.xExpectedEndDate = oRow("ExpectedEndDate")
                Else
                    Me.xExpectedEndDate = Nothing
                End If
                If Not IsDBNull(oRow("StartDate")) Then
                    Me.xStartDate = oRow("StartDate")
                Else
                    Me.xStartDate = Nothing
                End If
                If Not IsDBNull(oRow("EndDate")) Then
                    Me.xEndDate = oRow("EndDate")
                Else
                    Me.xEndDate = Nothing
                End If

                Me.lngInitialTime = Any2Double(oRow("InitialTime"))

                Me.lngTimeChangedRequirements = Any2Double(oRow("TimeChangedRequirements"))
                Me.lngForecastErrorTime = Any2Double(oRow("ForecastErrorTime"))
                Me.lngNonProductiveTimeIncidence = Any2Double(oRow("NonProductiveTimeIncidence"))
                Me.lngEmployeeTime = Any2Double(oRow("EmployeeTime"))
                Me.lngTeamTime = Any2Double(oRow("TeamTime"))
                Me.lngMaterialTime = Any2Double(oRow("MaterialTime"))
                Me.lngOtherTime = Any2Double(oRow("OtherTime"))

                Me.strTimeRemarks = Any2String(oRow("TimeRemarks"))

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

                If Not IsDBNull(oRow("IDEmployeeUpdateStatus")) Then
                    Me.intIDEmployeeUpdateStatus = Any2Integer(oRow("IDEmployeeUpdateStatus"))
                Else
                    Me.intIDEmployeeUpdateStatus = Nothing
                End If

                Me.intTypeAuthorization = Any2Integer(oRow("TypeAuthorization"))

                Dim oEmployeeTaskState As New roEmployeeTaskState
                Me.oEmployees = roEmployeeTaskDescription.GetEmployeesByTask(oRow("ID"), oEmployeeTaskState)

                Dim oGroupTaskState As New roGroupTaskState
                Me.oGroups = roGroupTaskDescription.GetGroupsByTask(oRow("ID"), oGroupTaskState)

                bolRet = True

                If _Audit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTask, Me.strName, tbParameters, -1)
                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal bCheckEmployeesWorkingInTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim NewID As Integer

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = TaskResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate(, bCheckEmployeesWorkingInTask) Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Task")
                    Dim strSQL As String = "@SELECT# * FROM Tasks " &
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

                    oRow("IDCenter") = Me.intIDCenter
                    oRow("Name") = Me.strName.Replace(vbCrLf, "")
                    oRow("ShortName") = Me.strShortName
                    oRow("Description") = Me.strDescription
                    If mTaskNotRequiresApprovalToBeCompleted AndAlso Me.intStatus = TaskStatusEnum._PENDING Then Me.intStatus = TaskStatusEnum._COMPLETED
                    oRow("Status") = Me.intStatus
                    oRow("Color") = Me.intColor
                    oRow("Project") = Me.strProject.Replace(vbCrLf, "")
                    oRow("Tag") = Me.strTag
                    oRow("Priority") = Me.intPriority
                    oRow("IDPassport") = Me.intIDPassport

                    oRow("BarCode") = Me.strBarcode

                    If Me.xExpectedStartDate.HasValue Then
                        oRow("ExpectedStartDate") = Me.xExpectedStartDate.Value
                    Else
                        oRow("ExpectedStartDate") = DBNull.Value
                    End If

                    If Me.xUpdateStatusDate.HasValue Then
                        oRow("UpdateStatusDate") = Me.xUpdateStatusDate.Value
                    Else
                        oRow("UpdateStatusDate") = DBNull.Value
                    End If

                    If Me.xExpectedEndDate.HasValue Then
                        oRow("ExpectedEndDate") = Me.xExpectedEndDate.Value
                    Else
                        oRow("ExpectedEndDate") = DBNull.Value
                    End If

                    If Me.xStartDate.HasValue Then
                        oRow("StartDate") = Me.xStartDate.Value
                    Else
                        oRow("StartDate") = DBNull.Value
                    End If

                    oRow("EndDate") = DBNull.Value

                    If Me.intStatus = TaskStatusEnum._ON Then
                        oRow("EndDate") = DBNull.Value
                    Else
                        If IsDBNull(oRow("EndDate")) Then
                            oRow("EndDate") = Any2Time(Now).Value
                        End If
                    End If

                    If Me.intIDEmployeeUpdateStatus.HasValue Then
                        oRow("IDEmployeeUpdateStatus") = Me.intIDEmployeeUpdateStatus.Value
                    Else
                        oRow("IDEmployeeUpdateStatus") = DBNull.Value
                    End If

                    oRow("InitialTime") = Me.lngInitialTime
                    oRow("TimeChangedRequirements") = Me.lngTimeChangedRequirements
                    oRow("ForecastErrorTime") = Me.lngForecastErrorTime
                    oRow("NonProductiveTimeIncidence") = Me.lngNonProductiveTimeIncidence
                    oRow("EmployeeTime") = Me.lngEmployeeTime
                    oRow("TeamTime") = Me.lngTeamTime
                    oRow("MaterialTime") = Me.lngMaterialTime
                    oRow("OtherTime") = Me.lngOtherTime

                    oRow("TimeRemarks") = Me.strTimeRemarks

                    oRow("Status") = Me.intStatus

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
                    Dim DeleteQuerys() As String = {"@DELETE# FROM EmployeeTasks WHERE IDTask = " & Me.intID.ToString, "@DELETE# FROM GroupTasks WHERE IDTask = " & Me.intID.ToString}
                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    'Asignamos los empleados y grupos en caso necesario
                    If bolRet AndAlso Me.intTypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES Then
                        If Me.oEmployees IsNot Nothing AndAlso Me.oEmployees.Count > 0 Then
                            For Each oEmp As roEmployeeTaskDescription In oEmployees
                                bolRet = ExecuteSql("@INSERT# INTO EmployeeTasks (IDEmployee, IDTask) VALUES(" & oEmp.ID.ToString & "," & Me.intID.ToString & ")")
                                If Not bolRet Then Exit For
                            Next
                        End If

                        'Asignamos los nuevos grupos
                        If bolRet AndAlso Me.oGroups IsNot Nothing AndAlso Me.oGroups.Count > 0 Then
                            For Each oGro As roGroupTaskDescription In oGroups
                                bolRet = ExecuteSql("@INSERT# INTO GroupTasks (IDGroup, IDTask) VALUES(" & oGro.ID.ToString & "," & Me.intID.ToString & ")")
                                If Not bolRet Then Exit For
                            Next
                        End If
                    End If

                    If bolRet AndAlso bAudit Then
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
                        Me.oState.Audit(oAuditAction, Audit.ObjectType.tTask, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTask::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True, Optional ByVal bCheckEmployeesWorkingInTask As Boolean = True) As Boolean

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
                ' El centro no puede estar en blanco
                If Me.IDCenter = 0 AndAlso Me.ID <> 0 Then
                    oState.Result = TaskResultEnum.CenterCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' Compuebo que el CB no exista
                If Me.BarCode.Length > 0 Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Tasks " &
                             "WHERE BarCode = @BarCode AND " &
                                " ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@BarCode", DbType.String, 64)
                    cmd.Parameters("@BarCode").Value = Me.BarCode

                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = TaskResultEnum.BarCodeAlreadyExist
                        bolRet = False
                    End If

                End If

                If bolRet And bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Tasks " &
                             "WHERE Name = @TaskName AND " &
                                " Project = @ProjectName AND " &
                                " ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@TaskName", DbType.String, 64)
                    cmd.Parameters("@TaskName").Value = Me.Name

                    AddParameter(cmd, "@ProjectName", DbType.String, 64)
                    cmd.Parameters("@ProjectName").Value = Me.Project

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
                    '' Compuebo que el nombre corto y el color no exista
                    'tb = New DataTable()
                    'strSQL = "@SELECT# * FROM Tasks " & _
                    '         "WHERE ShortName = @ShortName AND " & _
                    '         "Project = @ProjectName AND " & _
                    '               "ID <> " & Me.ID.ToString
                    'cmd = CreateCommand(strSQL)
                    'AddParameter(cmd, "@ShortName", DbType.String, 64)
                    'cmd.Parameters("@ShortName").Value = Me.ShortName
                    'AddParameter(cmd, "@ProjectName", DbType.String, 64)
                    'cmd.Parameters("@ProjectName").Value = Me.Project

                    'da = CreateDataAdapter(cmd, True)
                    'tb.Rows.Clear()
                    'da.Fill(tb)

                    'If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    '    oState.Result = TaskResultEnum.ShortNameAlreadyExist
                    '    bolRet = False
                    'End If
                    'End If
                End If

                If bolRet Then
                    'If (Me.InitialTime + Me.EmployeeTime + Me.MaterialTime + Me.NonProductiveTimeIncidence + Me.OtherTime + Me.TeamTime + Me.TimeChangedRequirements + Me.ForecastErrorTime) < 0 Then
                    If Me.InitialTime < 0 Then
                        oState.Result = TaskResultEnum.NegativeTime
                        bolRet = False
                    End If
                End If

                If bolRet And (Me.intStatus = TaskStatusEnum._CANCELED Or Me.intStatus = TaskStatusEnum._COMPLETED) And bCheckEmployeesWorkingInTask Then
                    ' Verificamos en el caso que se finalice la tarea o que se cancele
                    ' que no exista ningun empleado trabajando en ella
                    Dim oEmployeesonTask As DataTable = GetEmployeesWorkingInTaskDatatable(Me.intID, oState)
                    If oEmployeesonTask IsNot Nothing And oEmployeesonTask.Rows.Count > 0 Then
                        Dim strret As String = ""
                        For Each oRowAux As DataRow In oEmployeesonTask.Rows
                            strret = strret & oRowAux("Name") & ","
                        Next

                        If strret.Length > 0 Then
                            strret = strret.Substring(0, strret.Length - 1)
                        End If
                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(strret)
                        oState.Result = TaskResultEnum.WorkingInTask
                        oState.Language.ClearUserTokens()

                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTask::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::Validate")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra la tarea siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim sSql As String = "@DELETE# FROM Tasks WHERE ID = " & Me.ID.ToString
            Dim bolRet As Boolean = False
            Dim strArray As New ArrayList
            Dim strSql As String
            Dim lPunchesWithPhotoToDelete As List(Of Integer) = New List(Of Integer)

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oEmployeesonTask As DataTable = GetEmployeesWorkedOnTaskDatatable(Me.intID, oState)
                If oEmployeesonTask IsNot Nothing And oEmployeesonTask.Rows.Count > 0 Then
                    Dim strret As String = ""
                    For Each oRowAux As DataRow In oEmployeesonTask.Rows
                        strret = strret & oRowAux("Name") & ","
                    Next

                    If strret.Length > 0 Then
                        strret = strret.Substring(0, strret.Length - 1)
                    End If
                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(strret)
                    oState.Result = TaskResultEnum.WorkingInTask
                    oState.Language.ClearUserTokens()

                    bolRet = False
                    Return False
                End If

                strArray.Add("@DELETE# FROM EmployeeTasks WHERE IDTask = " & Me.ID.ToString)
                strArray.Add("@DELETE# FROM AlertsTask WHERE IDTask = " & Me.ID.ToString)
                strArray.Add("@DELETE# FROM GroupTasks WHERE IDTask = " & Me.ID.ToString)
                strArray.Add("@DELETE# FROM DailyTaskAccruals WHERE IDTask = " & Me.ID.ToString)
                strArray.Add("@DELETE# FROM FieldsTask WHERE IDTask = " & Me.ID.ToString)
                strArray.Add("@DELETE# FROM PunchesCaptures " _
                                & " WHERE IDPunch IN (@SELECT# id from Punches where ActualType = " & PunchTypeEnum._TASK & " AND TypeData= " & Me.ID.ToString & ")")
                strArray.Add("@DELETE# FROM Punches WHERE ActualType = " & PunchTypeEnum._TASK & " AND TypeData= " & Me.ID.ToString)
                strArray.Add("@DELETE# Tasks WHERE ID= " & intID)

                bolRet = True

                Dim strSQLAux As String = "@SELECT# DISTINCT IDEMPLOYEE, SHIFTDATE FROM PUNCHES WHERE ACTUALTYPE = " & PunchTypeEnum._TASK & " AND TypeData =" & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQLAux)
                If tb IsNot Nothing And tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        bolRet = ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET TaskStatus=0 WHERE IDEmployee=" & oRow("IDEmployee").ToString & " AND Date =" & Any2Time(oRow("ShiftDate")).SQLSmallDateTime)
                        If Not bolRet Then Exit For
                    Next
                End If

                ' Guardo id de los fichajes que voy a borrar si tienen fotos en azure, para borrarlas después de confirmar la transacción
                Dim strSQLPunchesWithPhoto As String = String.Empty
                strSQLPunchesWithPhoto = "@SELECT# ID FROM Punches WHERE PhotoOnAzure = 1 AND ActualType = " & PunchTypeEnum._TASK & " AND TypeData= " & Me.ID.ToString
                Dim tbPunchWithPhoto As DataTable = CreateDataTable(strSQLPunchesWithPhoto)
                If tbPunchWithPhoto IsNot Nothing AndAlso tbPunchWithPhoto.Rows.Count > 0 Then
                    For Each oPunchRow As DataRow In tbPunchWithPhoto.Rows
                        lPunchesWithPhotoToDelete.Add(roTypes.Any2Integer("ID"))
                    Next
                End If

                If bolRet Then
                    For Each strSql In strArray
                        bolRet = ExecuteSql(strSql)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    roConnector.InitTask(TasksType.TASKS)
                End If

                If bolRet And bAudit Then
                    ' Auditamos borrado de tarea
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{TaskName}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTask, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTask::DeleteTask")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::DeleteTask")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                ' Borrado de fotos de Azure
                If lPunchesWithPhotoToDelete IsNot Nothing AndAlso lPunchesWithPhotoToDelete.Count > 0 Then
                    For Each idPunch As Integer In lPunchesWithPhotoToDelete
                        Azure.RoAzureSupport.DeletePunchPhotoFile(roTypes.Any2Integer(idPunch))
                    Next
                End If

            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = -1

            Try

                Dim strSql As String = "@SELECT# Max(ID) as counter From Tasks"
                Dim tb As DataTable = CreateDataTable(strSql)

                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Counter")) Then
                        intRet = tb.Rows(0)("Counter") + 1
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTask::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTask::GetNextID")
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
                oState.UpdateStateInfo(ex, "roTask::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetTaskNameById(ByVal iTaskID As Integer, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False) As String
            Dim bolRet As String = String.Empty

            Try

                Dim strSQL As String = "@SELECT# Name FROM Tasks WHERE ID = " & iTaskID
                Dim strName As Object = ExecuteScalar(strSQL)
                If Not IsDBNull(strName) Then
                    bolRet = Any2String(strName)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::Load")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::Load")
            Finally

            End Try

            Return bolRet
        End Function



        Public Shared Function GetTasksByName(ByVal strLikeName As String, ByVal _State As roTaskState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# TOP 200 Tasks.ID, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name  " &
                         " FROM Tasks "

                If strLikeName.Length > 0 Then
                    strSQL &= "  WHERE (isnull(Tasks.Project,'') + ' ' + Tasks.Name) LIKE '" & Replace(strLikeName, "'", "''") & "' "
                End If

                strSQL &= " ORDER BY Tasks.Project, Tasks.Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTasksByProject(ByVal strLikeName As String, ByVal _State As roTaskState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * FROM Tasks "

                If strLikeName.Length > 0 Then
                    strSQL &= "  WHERE Project LIKE '" & Replace(strLikeName, "'", "''") & "' "
                End If

                strSQL &= " ORDER BY ID"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProjectsByName(ByVal strLikeName As String, ByVal _State As roTaskState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# DISTINCT(Project) as Name FROM Tasks "

                If strLikeName.Length > 0 Then
                    strSQL &= "  WHERE Project LIKE '" & Replace(strLikeName, "'", "''") & "' "
                End If

                strSQL &= " ORDER BY Project DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTasksByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetLastTasksByEmployee(ByVal intIDEmployee As Integer, ByVal _State As roTaskState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# TOP 500 Tasks.ID, Tasks.Name, Tasks.Project  " &
                         " FROM Tasks " &
                        "  WHERE ID IN(@SELECT# DISTINCT TypeData FROM Punches WHERE IDEmployee =" & intIDEmployee.ToString & " AND ActualType=" & PunchTypeEnum._TASK & " AND ShiftDate <=" & Any2Time(Now.Date).SQLSmallDateTime & " AND ShiftDate >=" & Any2Time(Now.Date).Add(-2, "m").SQLSmallDateTime & ")"
                strSQL &= "ORDER BY Tasks.Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetLastTasksByEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetLastTasksByEmployee")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetStatusTask(ByVal _IDTask As Integer, ByVal _State As roTaskState) As Double

            Dim oRet As Double = 0

            Try

                Dim strSQL As String
                strSQL = "@SELECT# CONVERT(numeric(25,4),ISNULL(SUM(DailyTaskAccruals.Value), 0)) as Value " &
                         " FROM DailyTaskAccruals  " &
                               " WHERE DailyTaskAccruals.IDTask = " & _IDTask.ToString

                ' Tiempo total trabajado en la tarea
                Dim TotalWork As Double = 0
                TotalWork = Any2Double(ExecuteScalar(strSQL))

                ' Tiempo total teorico
                Dim TotalTeoricTime As Double = 0
                strSQL = "@SELECT# InitialTime, TimeChangedRequirements, ForecastErrorTime, NonProductiveTimeIncidence, EmployeeTime, TeamTime, MaterialTime, OtherTime " &
                         " FROM Tasks  " &
                               " WHERE ID = " & _IDTask.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    TotalTeoricTime = Any2Double(oRow("InitialTime"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("TimeChangedRequirements"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("ForecastErrorTime"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("NonProductiveTimeIncidence"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("EmployeeTime"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("TeamTime"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("MaterialTime"))
                    TotalTeoricTime = TotalTeoricTime + Any2Double(oRow("OtherTime"))
                End If

                oRet = IIf(TotalTeoricTime > 0, Math.Round((TotalWork / TotalTeoricTime) * 100, 0), 0)
                If oRet > 100 Then oRet = 100
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetWorkedTimeInTask(ByVal _IDTask As Integer, ByVal _State As roTaskState) As Double

            Dim oRet As Double = 0

            Try

                Dim strSQL As String
                strSQL = "@SELECT# CONVERT(numeric(25,4),ISNULL(SUM(DailyTaskAccruals.Value), 0)) as Value " &
                         " FROM DailyTaskAccruals  " &
                               " WHERE DailyTaskAccruals.IDTask = " & _IDTask.ToString

                ' Tiempo total trabajado en la tarea
                oRet = Any2Double(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetNextTaskPunchDate(ByVal _State As roTaskState, ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime) As DateTime

            Dim nextDate As DateTime

            Try

                Dim strSQL As String
                strSQL = "@SELECT# TOP 1 Date as Value " &
                         " FROM DailyTaskAccruals  " &
                               " WHERE DailyTaskAccruals.IDTask = " & _IDTask.ToString

                If _IDEmployee > 0 Then
                    strSQL = strSQL & " AND DailyTaskAccruals.IDEmployee=" & _IDEmployee
                End If

                strSQL = strSQL & " AND DailyTaskAccruals.Date >" & Any2Time(_Date).SQLDateTime

                strSQL = strSQL & " ORDER BY DailyTaskAccruals.Date ASC"

                ' Tiempo total trabajado en la tarea
                Dim oDate As Object = ExecuteScalar(strSQL)
                If (oDate IsNot Nothing) Then
                    nextDate = Any2DateTime(oDate)
                Else
                    nextDate = _Date
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
                nextDate = DateTime.Now
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
                nextDate = DateTime.Now
            Finally

            End Try

            Return nextDate

        End Function

        Public Shared Function GetAntTaskPunchDate(ByVal _State As roTaskState, ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime) As DateTime

            Dim nextDate As DateTime

            Try

                Dim strSQL As String
                strSQL = "@SELECT# TOP 1 Date as Value " &
                         " FROM DailyTaskAccruals  " &
                               " WHERE DailyTaskAccruals.IDTask = " & _IDTask.ToString

                If _IDEmployee > 0 Then
                    strSQL = strSQL & " AND DailyTaskAccruals.IDEmployee=" & _IDEmployee
                End If

                strSQL = strSQL & " AND DailyTaskAccruals.Date <" & Any2Time(_Date).SQLDateTime

                strSQL = strSQL & " ORDER BY DailyTaskAccruals.Date DESC"

                ' Tiempo total trabajado en la tarea
                ' Tiempo total trabajado en la tarea
                Dim oDate As Object = ExecuteScalar(strSQL)
                If (oDate IsNot Nothing) Then
                    nextDate = Any2DateTime(oDate)
                Else
                    nextDate = _Date
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
                nextDate = DateTime.Now
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetStatusTask")
                nextDate = DateTime.Now
            Finally

            End Try

            Return nextDate

        End Function

        Public Shared Function GetLastTaskByEmployee(ByVal intIDEmployee As Integer, ByVal _State As roTaskState) As roTask

            Dim oRet As New roTask

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * FROM Tasks WHERE ID = " &
                        " (@SELECT# TOP 1 TypeData FROM Punches WHERE IDEmployee =" & intIDEmployee.ToString & " AND ActualType=" _
                        & PunchTypeEnum._TASK & " AND ShiftDate <=" & Any2Time(Now.Date).SQLSmallDateTime & " ORDER BY DateTime DESC,ID DESC)"

                Dim dt As DataTable = CreateDataTable(strSQL, )

                If dt.Rows.Count = 1 Then
                    oRet.LoadFromRow(dt.Rows(0), False)
                Else
                    oRet.ID = 0
                    oRet.Load(False)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetLastTaskByEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetLastTaskByEmployee")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTasks(ByVal _Order As String, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roTask)

            Dim oRet As New Generic.List(Of roTask)

            Try

                Dim strSQL As String = "@SELECT# Tasks.* " &
                                       "FROM Tasks "
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roTask(oRow, _State, _Audit))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTasks")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTasks")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskByBarCode(ByVal _BarCode As String, ByVal _State As roTaskState) As roTask

            Dim oRet As roTask = Nothing

            Try

                Dim strSQL As String = "@SELECT# Tasks.ID " &
                                       "FROM Tasks WHERE BarCode ='" & _BarCode.Replace("'", "''") & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing And tb.Rows.Count > 0 Then
                    oRet = New roTask(roTypes.Any2Long(tb.Rows(0).Item(0)), _State)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTaskByBarCode")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTaskByBarCode")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTasksDataTable(ByVal _Order As String, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Tasks.* " &
                                       "FROM Tasks "
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                tbRet = CreateDataTable(strSQL, )
                If tbRet IsNot Nothing Then

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTasksDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTasksDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function GetStatistics(ByVal _IDTask As Integer, ByVal _ViewType As TaskStatisticsViewEnum, ByVal _ViewGroupBy As TaskStatisticsGroupByEnum, ByVal _State As roTaskState, Optional ByVal _BeginDate As Date = Nothing, Optional ByVal _EndDate As Date = Nothing) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                If _BeginDate = Nothing Then
                    _BeginDate = Now.Date
                End If

                If _EndDate = Nothing Then
                    _EndDate = Now.Date
                End If

                Dim strSQL As String = ""

                Select Case _ViewType
                    Case TaskStatisticsViewEnum._WorkingTime
                        Select Case _ViewGroupBy
                            Case TaskStatisticsGroupByEnum._ByGroup
                                strSQL = "@SELECT# FullGroupName as Concept,   isnull(convert(numeric(25, 6),SUM(Value)), 0)  as Total " &
                                            "FROM Employees INNER JOIN DailyTaskAccruals on DailyTaskAccruals.IDEmployee = Employees.ID INNER JOIN Tasks on Tasks.ID = DailyTaskAccruals.IDTask INNER JOIN sysroEmployeeGroups on sysroEmployeeGroups.IDEmployee=DailyTaskAccruals.IDEmployee " &
                                                " WHERE DailyTaskAccruals.IDTask = " & _IDTask & " AND DailyTaskAccruals.Date Between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate " &
                                                    " GROUP BY FullGroupName "
                            Case TaskStatisticsGroupByEnum._ByEmployee
                                strSQL = "@SELECT# Employees.Name as Concept ,   isnull(convert(numeric(25, 6),SUM(Value)), 0)  as Total, Employees.ID as IDConcept " &
                                            "FROM Employees INNER JOIN DailyTaskAccruals on DailyTaskAccruals.IDEmployee = Employees.ID INNER JOIN Tasks on Tasks.ID = DailyTaskAccruals.IDTask INNER JOIN sysroEmployeeGroups on sysroEmployeeGroups.IDEmployee=DailyTaskAccruals.IDEmployee " &
                                                " WHERE DailyTaskAccruals.IDTask = " & _IDTask & " AND DailyTaskAccruals.Date Between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate " &
                                                    " GROUP BY Employees.Name, Employees.ID"
                            Case TaskStatisticsGroupByEnum._ByDate
                                strSQL = "@SELECT# DailyTaskAccruals.Date as Concept,   isnull(convert(numeric(25, 6),SUM(Value)), 0)  as Total " &
                                            "FROM Employees INNER JOIN DailyTaskAccruals on DailyTaskAccruals.IDEmployee = Employees.ID INNER JOIN Tasks on Tasks.ID = DailyTaskAccruals.IDTask INNER JOIN sysroEmployeeGroups on sysroEmployeeGroups.IDEmployee=DailyTaskAccruals.IDEmployee " &
                                                " WHERE DailyTaskAccruals.IDTask = " & _IDTask & " AND DailyTaskAccruals.Date Between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate " &
                                                " AND DailyTaskAccruals.Date Between  " & Any2Time(_BeginDate).SQLSmallDateTime & " AND " & Any2Time(_EndDate).SQLSmallDateTime &
                                                    " GROUP BY DailyTaskAccruals.Date "
                        End Select
                    Case TaskStatisticsViewEnum._Diversions
                        strSQL = "@SELECT# 1, InitialTime  FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 2, TimeChangedRequirements FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 3, ForecastErrorTime FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 4, NonProductiveTimeIncidence FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 5, EmployeeTime FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 6, TeamTime FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 7, MaterialTime FROM Tasks WHERE ID = " & _IDTask &
                            " UNION @SELECT# 8, OtherTime FROM Tasks WHERE ID = " & _IDTask
                End Select

                Dim cmd As DbCommand = CreateCommand(strSQL)

                oRet = New DataTable("TaskStatistics")

                Dim da As DbDataAdapter = CreateDataAdapter(cmd)
                da.Fill(oRet)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetStatistics")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetStatistics")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTasksFiltered(ByVal tasksList As String) As List(Of Integer)

            Dim oRet As List(Of Integer) = New List(Of Integer)

            Try
                Dim oIDs = tasksList.Split(",")
                Dim strIDs As List(Of String) = New List(Of String)
                For Each sID As String In oIDs
                    strIDs.Add("'" + HttpUtility.UrlDecode(sID).ToString() + "'")
                Next

                Dim strSQL As String
                strSQL = "@SELECT# ID FROM Tasks WHERE cast(ID as nvarchar) IN (" & Join(strIDs.ToArray(), ",") & ") or Project IN (" & Join(strIDs.ToArray(), ",") & ")"

                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each Row In tb.Rows
                    oRet.Add(CInt(Row.Item("ID")))
                Next
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Crea una copia de una Tarea existente.
        ''' </summary>
        ''' <param name="_IDSourceTask">Código de tarea del que se quiere realizar la copia</param>
        ''' <param name="_NewName">Nombre de la nueva Tarea creado. Si no se informa, se utiliza el tag de idioma 'Tasks.TaskSave.Copy' para generar el nuevo nombre (copia de ...).</param>
        ''' <param name="_State"></param>
        ''' <returns>Nueva tarea creada</returns>
        ''' <remarks></remarks>
        Public Shared Function CopyTask(ByVal _IDSourceTask As Integer, ByVal _NewName As String, ByVal _State As roTaskState, Optional ByVal bAudit As Boolean = False) As Integer

            Dim oRet As Integer = -1
            Dim oTask As roTask = Nothing

            Try

                oTask = New roTask(_IDSourceTask, _State, False)

                oTask.ID = -1
                If _NewName = "" Then
                    _State.Language.ClearUserTokens()
                    _State.Language.AddUserToken(oTask.Name)
                    _NewName = _State.Language.Translate("Tasks.TaskSave.Copy", "")
                    _State.Language.ClearUserTokens()
                End If
                oTask.Project = "___________"
                oTask.StartDate = Nothing
                oTask.EndDate = Nothing
                oTask.Color = 0
                oTask.ExpectedStartDate = Nothing
                oTask.ExpectedEndDate = Nothing
                oTask.EmployeeTime = 0
                oTask.ForecastErrorTime = 0
                oTask.MaterialTime = 0
                oTask.OtherTime = 0
                oTask.Status = TaskStatusEnum._ON
                oTask.TeamTime = 0
                oTask.NonProductiveTimeIncidence = 0
                oTask.TimeChangedRequirements = 0
                oTask.TimeRemarks = ""

                If oTask.Save(bAudit) Then
                    oRet = oTask.ID
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::CopyTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::CopyTask")
            Finally
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el nombre de una tarea a partir de su ID.
        ''' </summary>
        ''' <param name="_IDTask">Código de tarea del que se quiere realizar la copia</param>
        ''' <param name="_State"></param>
        ''' <returns>Nombre de la tarea</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskName(ByVal _IDTask As Integer, ByVal _State As roTaskState) As String

            Dim oRet As String = ""

            Try

                Dim strSQL As String = "@SELECT# (isnull(Tasks.Project,'') + ' ' + Tasks.Name) as Name   " &
                                       "FROM Tasks WHERE ID=" & _IDTask.ToString

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

        Public Shared Function GetTaskDescription(ByVal strLikeName As String, ByVal _State As roTaskState) As String

            Dim oRet As String = ""

            Try

                Dim strSQL As String = "@SELECT# (Tasks.Description) as Description   " &
                                       "FROM Tasks WHERE (Tasks.Name) LIKE '" & Replace(strLikeName, "'", "''") & "' "

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    oRet = Any2String(oRow("Description"))
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetTaskDescription")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetTaskDescription")
            Finally

            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Guarda los valores introducidos a partir de un fichaje de Terminal, los campos
        ''' que se tengan que guardar en los campos de la ficha de la tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <param name="Field1">Campo1</param>
        ''' <param name="Field2">Campo1</param>
        '''<returns>Si se han guardado correctamente los datos</returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTaskFieldsFromPunch(ByVal IDTask As Integer, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String, ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double, ByVal _State As roTaskState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String = ""

                bolRet = True

                If Field1.Length > 0 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field1='" & Field1.Replace("'", "''") & "' WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If

                If Field2.Length > 0 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field2='" & Field2.Replace("'", "''") & "' WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If

                If Field3.Length > 0 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field3='" & Field3.Replace("'", "''") & "' WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If

                If Field4 <> -1 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field4=" & Field4.ToString().Replace(".", ",") & " WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If

                If Field5 <> -1 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field5=" & Field5.ToString().Replace(".", ",") & " WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If

                If Field6 <> -1 AndAlso bolRet Then
                    strSQL = "@UPDATE# Tasks Set Field6=" & Field6.ToString().Replace(".", ",") & " WHERE ID=" & IDTask
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roTask::SaveTaskFieldsFromPunch")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roTask::SaveTaskFieldsFromPunch")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve el nº de empleados asignados a una tarea o los empleados seleccionados a partir de los id's de grupo y empleado
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <param name="sEmployees">ID's de los empleados</param>
        ''' <param name="sGroups">ID's de los grupos</param>
        '''<returns>Nº de empleados asignados a la tarea o seleccionados</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesFromTask(ByVal IDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As Double
            Dim oRet As Double = 0

            Try

                Dim strIDGroups As String = ""
                Dim strPath As String = ""
                Dim strEmployees As String = ""
                Dim i As Integer
                Dim oTask As New roTask

                If IDTask > 0 Then
                    oTask.ID = IDTask
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
        ''' Obtiene el nº de empleados trabajando en una tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <returns>Obtiene el nº de empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTask(ByVal IDTask As Integer, ByRef _State As roTaskState) As Double
            Dim oRet As Double = 0

            Try

                Dim strSQL As String
                'strSQL = "@SELECT# count(*) From punches p inner Join(@SELECT# idemployee, max(DateTime) as datetime From punches Where Type = 4 Group By idemployee) tmp "
                'strSQL = strSQL & "On p.idemployee=tmp.idemployee And p.datetime=tmp.datetime And p.type = 4 where typedata = " & IDTask.ToString

                strSQL = "@SELECT# COUNT(*) FROM ( " &
                         "@SELECT# ROW_NUMBER() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC, ID DESC) fila, TypeData FROM PUNCHES " &
                         "WHERE Type = 4 " &
                         ") aux WHERE aux.fila = 1 AND  aux.typedata = " & IDTask.ToString

                oRet = ExecuteScalar(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTask")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el nº de empleados trabajando en una tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <returns>Obtiene el nº de empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTaskOld(ByVal IDTask As Integer, ByRef _State As roTaskState) As Double
            Dim oRet As Double = 0
            Dim oTable As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Employees.ID,Employees.Name," &
                                         " (@SELECT# TOP 1 Typedata  FROM Punches WHERE Punches.IDEmployee = Employees.ID and Punches.Type = 4 ORDER BY DateTime DESC, ID DESC) AS 'LastPunchTask'" &
                                         " FROM EMPLOYEES WHERE Employees.Type ='J'"

                oTable = CreateDataTable(strSQL, )
                If oTable IsNot Nothing Then
                    If oTable.Rows.Count > 0 Then
                        For Each dRow As DataRow In oTable.Rows
                            If Any2Integer(dRow("LastPunchTask")) <> IDTask Then
                                dRow.Delete()
                            End If
                        Next
                        oTable.AcceptChanges()
                    End If

                    If oTable IsNot Nothing Then oRet = oTable.Rows.Count
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTask")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el listado de empleados que han trabajando en una tarea en los dos ultimos dias
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Obtiene el listado de empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkedInTaskList(ByVal IDTask As Integer, ByRef _State As roTaskState) As DataTable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# taskStatus.* FROM (" &
                                        "@SELECT# Employees.ID,Employees.Name, MAX(tPunches.DateTime) AS PunchDateTime FROM Employees inner join (" &
                                        "@SELECT# ID, IDEmployee, DateTime FROM Punches WHERE Punches.Type = 4 and Punches.TypeData = " & IDTask & ") tPunches " &
                                        " ON employees.ID = tPunches.IDEmployee and Employees.Type ='J' and tPunches.DateTime > DATEADD(d,-7,getdate()) " &
                                        " GROUP BY Employees.ID,Employees.Name " &
                                        ")taskStatus " &
                                        " INNER JOIN EmployeeStatus ON taskStatus.ID = EmployeeStatus.IDEmployee " &
                                        " WHERE taskStatus.PunchDateTime is not null AND EmployeeStatus.LastPunch >= DATEADD(day, - 7, GETDATE()) " &
                                        " AND taskStatus.ID not in (" &
                                        " @SELECT# DISTINCT taskStatus.ID from (" &
                                        " @SELECT# Employees.ID,Employees.Name, " &
                                        " (@SELECT# TOP 1 DateTime FROM Punches WHERE Punches.IDEmployee = Employees.ID and Punches.Type = 4 " &
                                        " AND Punches.TypeData = " & IDTask & " ORDER BY DateTime DESC, ID DESC) AS 'PunchDateTime' FROM EMPLOYEES " &
                                        " WHERE Employees.Type ='J') taskStatus" &
                                        " INNER JOIN EmployeeStatus ON taskStatus.ID = EmployeeStatus.IDEmployee " &
                                        " WHERE taskStatus.PunchDateTime is not null AND EmployeeStatus.LastPunch >= DATEADD(day, - 4, GETDATE()) " &
                                        " AND taskStatus.PunchDateTime = (@SELECT# TOP 1 DateTime FROM Punches WHERE Punches.IDEmployee = taskStatus.ID and Punches.Type = 4 ORDER BY DateTime DESC, ID DESC ))"

                'SELECT TOP 1 DateTime FROM Punches WHERE Punches.IDEmployee = taskStatus.ID and Punches.Type = 4 ORDER BY DateTime DESC, ID DESC )

                ' " ) and tPunches.DateTime > DATEADD(d,-2,getdate()) "
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkedInTaskList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkedInTaskList")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el listadp de empleados trabajando en una tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Obtiene el listado de empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTaskList(ByVal IDTask As Integer, ByRef _State As roTaskState) As DataTable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# taskStatus.* from (" &
                                        "@SELECT# Employees.ID,Employees.Name, " &
                                        "(@SELECT# TOP 1 DateTime FROM Punches WHERE Punches.IDEmployee = Employees.ID and Punches.Type = 4 " &
                                        " AND Punches.TypeData = " & IDTask & " ORDER BY DateTime DESC, ID DESC) AS 'PunchDateTime' FROM EMPLOYEES " &
                                        " WHERE Employees.Type ='J') taskStatus" &
                                        " INNER JOIN EmployeeStatus ON taskStatus.ID = EmployeeStatus.IDEmployee " &
                                        " WHERE taskStatus.PunchDateTime is not null AND EmployeeStatus.LastPunch >= DATEADD(day, - 4, GETDATE()) " &
                                        " AND taskStatus.PunchDateTime = (@SELECT# TOP 1 DateTime FROM Punches WHERE Punches.IDEmployee = taskStatus.ID and Punches.Type = 4 ORDER BY DateTime DESC, ID DESC )"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTaskList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTaskList")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los empleados trabajando en una tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Datatable con los empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTaskDatatable(ByVal IDTask As Integer, ByRef _State As roTaskState) As DataTable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# id, name, LastPunchTask from (@SELECT# ROW_NUMBER() OVER(PARTITION BY punches.idemployee ORDER BY idemployee desc , datetime DESC) AS RowNumber,  Employees.ID as 'id',Employees.Name as 'NAME', punches.TypeData as LastPunchTask from employees inner join punches on punches.IDEmployee = employees.id where employees.Type='J' and punches.Type=4) a where a.RowNumber=1 "

                'Dim strSQL As String = "@SELECT# Employees.ID,Employees.Name," &
                '                         " (@SELECT# TOP 1 Typedata  FROM Punches WHERE Punches.IDEmployee = Employees.ID and Punches.Type = 4 ORDER BY DateTime DESC, ID DESC) AS 'LastPunchTask'" &
                '                            " FROM EMPLOYEES WHERE Employees.Type ='J'"

                oRet = CreateDataTable(strSQL, )
                If oRet IsNot Nothing Then
                    If oRet.Rows.Count > 0 Then
                        For Each dRow As DataRow In oRet.Rows
                            If Any2Integer(dRow("LastPunchTask")) <> IDTask Then
                                dRow.Delete()
                            End If
                        Next
                        oRet.AcceptChanges()
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTaskDatatable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkingInTaskDatatable")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los empleados que han trabajado en una tarea en algun momento
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Datatable con los empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkedOnTaskDatatable(ByVal IDTask As Integer, ByRef _State As roTaskState) As DataTable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Employees.ID,Employees.Name" &
                                            " FROM EMPLOYEES WHERE Employees.Type ='J' AND Employees.ID IN(@SELECT# DISTINCT IDEMPLOYEE FROM Punches WHERE ActualType = " & PunchTypeEnum._TASK & " AND TypeData= " & IDTask.ToString & ")"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkedOnTaskDatatable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::GetEmployeesWorkedOnTaskDatatable")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Marca la tarea como completada
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <param name="strError">Si no ha podido completarla, deja como error </param>
        '''<returns>Datatable con los empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function CompleteTask(ByVal IDTask As Integer, ByRef strError As String, ByRef _State As roTaskState) As Boolean
            Dim oRet As Boolean = True

            Try

                Dim strSQL As String = "@UPDATE# dbo.Tasks SET Status=" & TaskStatusEnum._COMPLETED & ",EndDate=" & Any2Time(Now).SQLSmallDateTime & "  WHERE ID=" & IDTask
                If Not ExecuteSql(strSQL) Then
                    oRet = False
                    strError = GetTaskNameById(IDTask, _State)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::CompleteTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::CompleteTask")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Marca la tarea como completada
        ''' </summary>
        ''' <param name="strProject">Nombre del proyecto para cerrar</param>
        ''' <param name="strError">Deja como error el nombre de las tareas que no se han podido cerrar separadas por comas </param>
        '''<returns>Datatable con los empleados trabajando en una tarea </returns>
        ''' <remarks></remarks>
        Public Shared Function CompleteTaskFromProject(ByVal strProject As String, ByRef strError As String, ByRef _State As roTaskState) As Boolean
            Dim oRet As Boolean = True

            Try

                Dim dtTasks As DataTable = roTask.GetTasksByProject(strProject, _State)

                If dtTasks IsNot Nothing AndAlso dtTasks.Rows.Count > 0 Then
                    Dim tmpRet As Boolean = True
                    Dim tmpError As String = String.Empty
                    For Each oRow As DataRow In dtTasks.Rows
                        tmpRet = CompleteTask(oRow("ID"), tmpError, _State)
                        If Not tmpRet Then strError = strError & tmpError & ","
                    Next
                End If

                If strError <> String.Empty Then
                    oRet = False
                    strError = strError.Substring(0, strError.Length - 1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTask::CompleteTaskFromProject")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTask::CompleteTaskFromProject")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roEmployeeTaskDescription
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
                oState.UpdateStateInfo(ex, "roEmployeeTaskDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTaskDescription::Load")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper Methods"

        Public Shared Function GetEmployeesByTask(ByVal IDTask As Integer, ByVal _State As roEmployeeTaskState) As Generic.List(Of roEmployeeTaskDescription)

            Dim oRet As New Generic.List(Of roEmployeeTaskDescription)

            Try

                Dim strSQL As String = "@SELECT# IDEmployee FROM EmployeeTasks, Employees Where Employees.ID = EmployeeTasks.IDEmployee AND  IDTask = " & IDTask & " Order by Employees.Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oEmployeeDesc As roEmployeeTaskDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oEmployeeDesc = New roEmployeeTaskDescription(oRow("IDEmployee"), _State)
                        oRet.Add(oEmployeeDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roEmployeeTaskDesctiption::GetEmployeesByTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeTaskDesctiption::GetEmployeesByTask")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roGroupTaskDescription
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
                oState.UpdateStateInfo(ex, "roGroupTaskDescription::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupTaskDescription::Load")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper Methods"

        Public Shared Function GetGroupsByTask(ByVal IDTask As Integer, ByVal _State As roGroupTaskState) As Generic.List(Of roGroupTaskDescription)

            Dim oRet As New Generic.List(Of roGroupTaskDescription)

            Try

                Dim strSQL As String = "@SELECT# IDGroup FROM GroupTasks,Groups Where Groups.ID = GroupTasks.IDGroup AND  IDTask = " & IDTask & " Order by Groups.Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oGroupDesc As roGroupTaskDescription = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oGroupDesc = New roGroupTaskDescription(oRow("IDGroup"), _State)
                        oRet.Add(oGroupDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roGroupTaskDesctiption::GetGroupsByTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupTaskDesctiption::GetGroupsByTask")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roTaskAlert

#Region "Declarations - Constructor"

        Private oState As roTaskState

        Private lngID As Long
        Private intIDTask As Integer
        Private intIDEmployee As Integer
        Private xDateTime As Nullable(Of DateTime)
        Private strComment As String
        Private strEmployeeName As String
        Private bolIsReaded As Boolean
        Private intIDPassportReaded As Integer

        Public Sub New()
            Me.oState = New roTaskState()
            Me.lngID = 0
            Me.Load()
        End Sub

        Public Sub New(ByVal _ID As Long, ByVal _State As roTaskState)
            Me.oState = _State
            Me.lngID = _ID
            Me.Load()

        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Long
            Get
                Return Me.lngID
            End Get
            Set(ByVal value As Long)
                Me.lngID = value
                Me.Load()
            End Set
        End Property
        <DataMember()>
        Public Property IDTask() As Integer
            Get
                Return Me.intIDTask
            End Get
            Set(ByVal value As Integer)
                Me.intIDTask = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property
        <DataMember()>
        Public Property DateTime() As Nullable(Of DateTime)
            Get
                Return Me.xDateTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xDateTime = value
            End Set
        End Property
        <DataMember()>
        Public Property Comment() As String
            Get
                Return Me.strComment
            End Get
            Set(ByVal value As String)
                Me.strComment = value
            End Set
        End Property
        <DataMember()>
        Public Property EmployeeName() As String
            Get
                Return Me.strEmployeeName
            End Get
            Set(ByVal value As String)
                Me.strEmployeeName = value
            End Set
        End Property

        <DataMember()>
        Public Property IsReaded() As Nullable(Of Boolean)
            Get
                Return Me.bolIsReaded
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                If value.HasValue Then
                    Me.bolIsReaded = value.Value
                Else
                    Me.bolIsReaded = False
                End If
            End Set
        End Property
        <DataMember()>
        Public Property IDPassportReaded() As Integer
            Get
                Return Me.intIDPassportReaded
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassportReaded = value
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

        Public Function Load(Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            If Me.lngID <= 0 Then
                Me.xDateTime = Now.Date
                Me.bolIsReaded = False
                Me.intIDPassportReaded = 0
                Me.intIDEmployee = 0
                Me.intIDTask = 0
                Me.strComment = ""
                Me.strComment = ""
                Me.strEmployeeName = ""
            Else

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM AlertsTask WHERE [ID] = " & Me.ID.ToString, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim oRow As DataRow = tb.Rows(0)

                        If Not IsDBNull(oRow("IDTask")) Then
                            Me.intIDTask = oRow("IDTask")
                        Else
                            Me.intIDTask = 0
                        End If

                        If Not IsDBNull(oRow("IDEmployee")) Then
                            Me.intIDEmployee = oRow("IDEmployee")
                            Me.strEmployeeName = Any2String(ExecuteScalar("@SELECT# name from Employees where id=" & Me.intIDEmployee))
                        Else
                            Me.intIDEmployee = 0
                        End If

                        Me.xDateTime = oRow("DateTime")

                        If Not IsDBNull(oRow("Comment")) Then
                            Me.strComment = oRow("Comment")
                        Else
                            Me.strComment = ""
                        End If

                        If Not IsDBNull(oRow("IsReaded")) Then
                            Me.bolIsReaded = oRow("IsReaded")
                        Else
                            Me.bolIsReaded = False
                        End If

                        If Not IsDBNull(oRow("IDPassportReaded")) Then
                            Me.intIDPassportReaded = oRow("IDPassportReaded")
                        Else
                            Me.intIDPassportReaded = 0
                        End If
                    End If

                    bolRet = True
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roTaskAlert::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roTaskAlert::Load")
                Finally

                End Try

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim tb As New DataTable("AlertsTask")
                Dim strSQL As String = "@SELECT# * FROM AlertsTask WHERE [ID] = " & Me.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                End If

                If Me.ID <> -1 Then
                    oRow("ID") = Me.ID
                End If

                oRow("IDTask") = Me.IDTask
                oRow("IDEmployee") = Me.IDEmployee
                oRow("DateTime") = Me.DateTime
                oRow("Comment") = Me.Comment
                oRow("IsReaded") = Me.IsReaded
                oRow("IDPassportReaded") = Me.IDPassportReaded

                If tb.Rows.Count <= 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTask::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::Save")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetTaskAlertsDataTable(ByVal IDTask As Integer, ByVal _State As roTaskState) As DataTable
            ' Devuelvo todas las Alertas de una tarea en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskAlertsTask As Generic.List(Of roTaskAlert) = roTaskAlert.GetTaskAlertsList(IDTask, _State)
            If _State.Result = TaskResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    oRet.Columns.Add(New DataColumn("ID", GetType(Long)))

                    oRet.Columns.Add(New DataColumn("IDTask", GetType(Integer)))

                    oRet.Columns.Add(New DataColumn("IDEmployee", GetType(Integer)))

                    oRet.Columns.Add(New DataColumn("EmployeeName", GetType(String)))

                    oRet.Columns.Add(New DataColumn("DateTime", GetType(DateTime)))

                    oRet.Columns.Add(New DataColumn("Comment", GetType(String)))

                    oRet.Columns.Add(New DataColumn("IsReaded", GetType(Boolean)))

                    oRet.Columns.Add(New DataColumn("IDPassportReaded", GetType(Integer)))

                    If TaskAlertsTask IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskAlertTask As roTaskAlert In TaskAlertsTask

                            oNewRow = oRet.NewRow

                            oNewRow("ID") = oTaskAlertTask.ID
                            oNewRow("IDTask") = oTaskAlertTask.IDTask
                            oNewRow("IDEmployee") = oTaskAlertTask.IDEmployee
                            oNewRow("EmployeeName") = oTaskAlertTask.EmployeeName
                            oNewRow("DateTime") = oTaskAlertTask.DateTime
                            oNewRow("Comment") = oTaskAlertTask.Comment
                            oNewRow("IsReaded") = oTaskAlertTask.IsReaded
                            oNewRow("IDPassportReaded") = oTaskAlertTask.IDPassportReaded
                            oRet.Rows.Add(oNewRow)
                        Next
                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldTask::GetTaskAlertsDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldTask::GetTaskAlertsDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetTaskAlertDataTable(ByVal _IDAlert As Integer, ByVal _State As roTaskState) As DataTable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Employees.Name as EmployeeName, AlertsTask.ID, AlertsTask.IDTask, AlertsTask.IDEmployee, AlertsTask.DateTime, AlertsTask.Comment, AlertsTask.IsReaded, AlertsTask.IDPassportReaded FROM AlertsTask, Employees " &
                                       " WHERE AlertsTask.ID = " & _IDAlert

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskAlertDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskAlertDataTable")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskAlertsList(ByVal _IDTask As Integer, ByVal _State As roTaskState) As Generic.List(Of roTaskAlert)

            Dim oRet As New Generic.List(Of roTaskAlert)

            Try

                Dim strSQL As String = "@SELECT# Employees.Name as EmployeeName, AlertsTask.ID, AlertsTask.IDTask, AlertsTask.IDEmployee, AlertsTask.DateTime, AlertsTask.Comment, AlertsTask.IsReaded, AlertsTask.IDPassportReaded FROM AlertsTask, Employees " &
                                       " WHERE AlertsTask.IDEmployee = Employees.ID AND  IDTask = " & _IDTask & " Order by AlertsTask.IsReaded asc , AlertsTask.DateTime desc"

                Dim tbAlerts As DataTable = CreateDataTable(strSQL, )

                If tbAlerts IsNot Nothing AndAlso tbAlerts.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbAlerts.Rows
                        oRet.Add(New roTaskAlert(oRow("ID"), _State))
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskAlertsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskAlertsList")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la alerta introducida a partir de un fichaje
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <param name="IDEmployee">ID del empleado</param>
        ''' <param name="xDateTime">Fecha hora de la alerta</param>
        ''' <param name="Comment">Anotacion del empleado</param>
        '''<returns>Si se han guardado correctamente los datos</returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTaskAlertsFromPunch(ByVal IDTask As Integer, ByVal IDEmployee As Integer, ByVal xDateTime As DateTime, ByVal Comment As String, ByVal _State As roTaskState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                strSQL = "@INSERT# INTO AlertsTask (IDTask, IDEmployee,DateTime, Comment) VALUES (" & IDTask & "," & IDEmployee & "," & Any2Time(xDateTime).SQLDateTime & ",'" & Comment.Replace("'", "''") & "')"
                bolRet = ExecuteSql(strSQL)

                bolRet = True
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roTask::SaveTaskAlertsFromPunch")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roTask::SaveTaskAlertsFromPunch")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskAlerts(ByVal _IDTask As Integer, ByVal _TaskAlertsTask As Generic.List(Of roTaskAlert), ByRef _State As roTaskState) As Boolean

            Dim bolRet As Boolean = False
            Dim bolRes As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String = ""

                strSQL = "@SELECT# * FROM AlertsTask WHERE IDTask= " & _IDTask
                ' Obtenemos las alertas de la tarea
                Dim ActualTaskAlertsTask As New Generic.List(Of roTaskAlert)
                Dim tbAlerts As DataTable = CreateDataTable(strSQL)

                If tbAlerts IsNot Nothing AndAlso tbAlerts.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAlerts.Rows
                        ActualTaskAlertsTask.Add(New roTaskAlert(oRow("ID"), _State))
                    Next
                End If

                bolRet = True

                ' Miramos que alertas hay que eliminar
                For Each oTaskAlertTask As roTaskAlert In ActualTaskAlertsTask
                    Dim bolExistAlert As Boolean = False
                    For Each oActualTaskFieldTask As roTaskAlert In _TaskAlertsTask
                        If oActualTaskFieldTask.ID = oTaskAlertTask.ID Then
                            bolExistAlert = True
                            Exit For
                        End If
                    Next

                    bolRes = True
                    If Not bolExistAlert Then
                        ' Eliminamos la alerta asignada a la tarea
                        strSQL = "@DELETE# FROM AlertsTask WHERE ID="
                        strSQL = strSQL & oTaskAlertTask.ID
                        bolRes = ExecuteSql(strSQL)
                    End If

                    If Not bolRes Then
                        bolRet = bolRes
                        Exit For
                    End If
                Next

                If bolRet Then
                    ' Asignamos los nuevos valores a las alertas
                    bolRes = True
                    For Each oActualTaskAlertTask As roTaskAlert In _TaskAlertsTask

                        strSQL = "@UPDATE# AlertsTask SET IsReaded=" & IIf(oActualTaskAlertTask.IsReaded, 1, 0) & " , IDPassportReaded=" & oActualTaskAlertTask.IDPassportReaded & "   WHERE ID=" & oActualTaskAlertTask.ID
                        bolRes = ExecuteSql(strSQL)

                        If Not bolRes Then
                            bolRet = bolRes
                            Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Auditamos modificacion asignacion de alertas
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = ""

                    Dim strTaskName As String = Any2String(ExecuteScalar("@SELECT# name from tasks where id=" & _IDTask))

                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{Name}", strTaskName, "", 1)
                    _State.Audit(Audit.Action.aUpdate, Audit.ObjectType.tTask, strTaskName, tbParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::SaveTaskAlerts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::SaveTaskAlerts")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class

    <DataContract()>
    Public Class roTaskAssignment

#Region "Declarations - Constructors"

        Private oState As roTaskState

        Private intIDTask As Integer
        Private intIDAssignment As Integer

        Public Sub New()

            Me.oState = New roTaskState
            Me.intIDTask = -1
            Me.intIDAssignment = -1

        End Sub

        Public Sub New(ByVal _IDTask As Integer, ByVal _IDAssignment As Integer, ByVal _State As roTaskState)

            Me.oState = _State

            Me.intIDTask = _IDTask
            Me.intIDAssignment = _IDAssignment

            Me.Load()

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDTask() As Integer
            Get
                Return Me.intIDTask
            End Get
            Set(ByVal value As Integer)
                Me.intIDTask = value
            End Set
        End Property
        <DataMember()>
        Public Property IDAssignment() As Integer
            Get
                Return Me.intIDAssignment
            End Get
            Set(ByVal value As Integer)
                Me.intIDAssignment = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM TaskAssignments " &
                                       "WHERE IDTask = " & Me.intIDTask.ToString & " AND " &
                                             "IDAssignment = " & Me.intIDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    ' TODO: auditar EmployeeAssignment
                    ' Auditar lectura
                    ''If _Audit Then
                    ''    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    ''    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDailyCoverageAssignment, Me.AuditObjectName(), tbParameters, -1)
                    ''End If

                    bolRet = True

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTaskAssignment::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAssignment::Load")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskAssignments(ByVal _IDTask As Integer, ByVal _TaskAssignments As Generic.List(Of roTaskAssignment), ByVal _State As roTaskState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try

                If ValidateTaskAssignments(_IDTask, _TaskAssignments, _State) Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    ' Obtenemos los puestos asignados actualmente
                    Dim lstOldDailyAssignments As Generic.List(Of roTaskAssignment) = roTaskAssignment.GetTaskAssignments(_IDTask, _State)

                    Dim IDAssignmentsSaved As New Generic.List(Of Integer)
                    If _TaskAssignments IsNot Nothing AndAlso _TaskAssignments.Count > 0 Then
                        For Each oTaskAssignment As roTaskAssignment In _TaskAssignments
                            oTaskAssignment.oState = _State
                            oTaskAssignment.IDTask = _IDTask
                            bolRet = oTaskAssignment.SaveTaskAssignment(bAudit, False)
                            If Not bolRet Then Exit For
                            IDAssignmentsSaved.Add(oTaskAssignment.IDAssignment)
                        Next
                    Else
                        bolRet = True
                    End If

                    If bolRet Then
                        ' Borramos los puestos asignados de la tabla que no esten en la lista
                        For Each oTaskAssignment As roTaskAssignment In lstOldDailyAssignments
                            If ExistTaskAssignmentInList(_TaskAssignments, oTaskAssignment) Is Nothing Then
                                bolRet = oTaskAssignment.Delete(bAudit)
                                If Not bolRet Then Exit For
                            End If
                        Next
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignment::SaveTaskAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignment::SaveTaskAssignments")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function SaveTaskAssignment(Optional ByVal _Audit As Boolean = False, Optional ByVal _ExecuteTask As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            Dim bolChangeData As Boolean = False
            Try

                If Me.Validate() Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldTaskAssignment As roTaskAssignment = Nothing

                    Dim tb As New DataTable("TaskAssignments")
                    Dim strSQL As String = "@SELECT# * FROM TaskAssignments " &
                                           "WHERE IDTask = " & Me.intIDTask.ToString & " AND " &
                                                 "IDAssignment = " & Me.intIDAssignment.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDTask") = Me.intIDTask
                        oRow("IDAssignment") = Me.intIDAssignment
                        bolChangeData = True
                    Else
                        oOldTaskAssignment = New roTaskAssignment(Me.intIDTask, Me.intIDAssignment, Me.oState)
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    'If bolRet And _Audit And bolChangeData Then
                    '    bolRet = False
                    '    ' Auditamos
                    '    Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    '    Audit.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    '    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    '    Dim strObjectName As String

                    '    Dim strTask As String = Any2String(ExecuteScalar("@SELECT# Name FROM Tasks where id=" & Me.intIDTask.ToString))
                    '    Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & oAuditDataNew("IDAssignment")))

                    '    strObjectName = strTask & ": " & strAssignment
                    '    'bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTaskAssignment, strObjectName, tbAuditParameters, -1)

                    'End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskAssignment::SaveTaskAssignment")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAssignment::SaveTaskAssignment")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskAssignment::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAssignment::Validate")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateTaskAssignments(ByVal _IDTask As Integer, ByVal _TaskAssignments As Generic.List(Of roTaskAssignment), ByVal _State As roTaskState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oList As New Generic.List(Of roTaskAssignment)

                ' Verificamos que la lista de puestos sea correcta.
                bolRet = True
                For Each oTaskAssignment As roTaskAssignment In _TaskAssignments
                    If ExistTaskAssignmentInList(oList, oTaskAssignment) IsNot Nothing Then
                        bolRet = False
                        _State.Result = TaskResultEnum.AssignmentRepited
                        Exit For
                    Else
                        oList.Add(oTaskAssignment)
                    End If
                Next

                ' Verificamos que los puestos no esten ya asignados a otra tarea
                If bolRet Then
                    For Each oTaskAssignment As roTaskAssignment In _TaskAssignments
                        If ExistTaskAssignmentInOtherTask(oTaskAssignment, _State) Then
                            bolRet = False
                            _State.Result = TaskResultEnum.AssignmentInOtherTask
                            Exit For
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignments::ValidateTaskAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignments::ValidateTaskAssignments")
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistTaskAssignmentInOtherTask(ByVal oTaskAssignment As roTaskAssignment, ByVal _State As roTaskState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM TaskAssignments " &
                                       "WHERE TaskAssignments.IDTask <> " & oTaskAssignment.IDTask.ToString & " AND " &
                                             "TaskAssignments.IDAssignment = " & oTaskAssignment.IDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                bolRet = (tb IsNot Nothing AndAlso tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignment::ExistTaskAssignmentInOtherTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignment::ExistTaskAssignmentInOtherTask")
            Finally

            End Try

            Return bolRet

        End Function

        Private Shared Function ExistTaskAssignmentInList(ByVal lstTaskAssignments As Generic.List(Of roTaskAssignment), ByVal oTaskAssignment As roTaskAssignment) As roTaskAssignment

            Dim oRet As roTaskAssignment = Nothing

            If lstTaskAssignments IsNot Nothing Then

                For Each oItem As roTaskAssignment In lstTaskAssignments
                    If oItem.IDTask = oTaskAssignment.IDTask And
                       oItem.IDAssignment = oTaskAssignment.IDAssignment Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

        Public Function Delete(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strTask As String = Any2String(ExecuteScalar("@SELECT# Name FROM Tasks where id=" & Me.intIDTask.ToString))
                Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & Me.intIDAssignment.ToString))

                Dim DelQuerys() As String = {"@DELETE# FROM TaskAssignments WHERE IDTask = " & Me.intIDTask.ToString & " AND " &
                                                                             "IDAssignment = " & Me.intIDAssignment.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = TaskResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = TaskResultEnum.NoError)

                If bolRet And _Audit Then
                    ' Auditamos
                    'bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTaskAssignment, strtask & ": " & strAssignment, Nothing, -1, oTrans.Connection)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTaskAssignment::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAssignment::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetTaskAssignmentsDataTable(ByVal _IDTask As Integer, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = True) As System.Data.DataTable
            ' Recupera la lista de Puestos de la tarea en un datatable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Assignments.Name, TaskAssignments.IDAssignment, TaskAssignments.IDTask  " &
                                       "FROM TaskAssignments INNER JOIN Assignments " &
                                                "ON TaskAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE TaskAssignments.IDTask = " & _IDTask.ToString & " " &
                                       "ORDER BY Assignments.Name"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    If _Audit Then
                        ' Auditamos
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignment::GetTaskAssignmentsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignment::GetTaskAssignmentsDataTable")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskAssignments(ByVal _IDTask As Integer, ByVal _State As roTaskState, Optional ByVal _Audit As Boolean = True) As Generic.List(Of roTaskAssignment)

            Dim oRet As New Generic.List(Of roTaskAssignment)

            Try

                Dim strSQL As String = "@SELECT# TaskAssignments.* " &
                                       "FROM TaskAssignments INNER JOIN Assignments " &
                                                "ON TaskAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE TaskAssignments.IDTask = " & _IDTask.ToString & " " &
                                       "ORDER BY Assignments.Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roTaskAssignment(oRow("IDTask"), oRow("IDAssignment"), _State))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignment::GetTaskAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignment::GetTaskAssignments")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function ExistTaskAssignment(ByVal _IDTask As Integer, ByVal _IDAssignment As Integer, ByVal _State As roTaskState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM TaskAssignments " &
                                       "WHERE TaskAssignments.IDTask = " & _IDTask.ToString & " AND " &
                                             "TaskAssignments.IDAssignment = " & _IDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                bolRet = (tb IsNot Nothing AndAlso tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskAssignment::ExistTaskAssignment")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskAssignment::ExistTaskAssignment")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace