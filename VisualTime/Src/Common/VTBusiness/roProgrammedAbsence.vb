Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Absence

    <DataContract()>
    <Serializable()>
    Public Class roProgrammedAbsence
        Inherits Forecast.roForecast

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roProgrammedAbsenceState

        Private iIdAbsence As Nullable(Of Integer)
        Private intIDEmployee As Nullable(Of Integer)
        Private xBeginDate As Nullable(Of DateTime)
        Private intIDCause As Nullable(Of Integer)
        Private xFinishDate As Nullable(Of DateTime)
        Private intMaxLastingDays As Integer
        Private strDescription As String
        Private xRelapsedDate As Nullable(Of Date)
        Private intRequestId As Nullable(Of Integer)
        Private intIDEmployeeOriginal As Nullable(Of Integer)
        Private xBeginDateOriginal As Nullable(Of DateTime)
        Private xFinishDateOriginal As Nullable(Of DateTime)
        Private intMaxLastingDaysOriginal As Nullable(Of Integer)
        Private bolAutomaticClosed As Nullable(Of Boolean)
        Private CustomizationCode As String = String.Empty
        Private Client As String = String.Empty

        Public Sub New()
            Me.oState = New roProgrammedAbsenceState
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _State As roProgrammedAbsenceState)
            Me.oState = _State
            Me.IDEmployee = _IDEmployee
            Me.BeginDate = _BeginDate
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _CustomizationCode As String, ByVal _Client As String, ByVal _State As roProgrammedAbsenceState)
            Me.oState = _State
            Me.IDEmployee = _IDEmployee
            Me.BeginDate = _BeginDate
            Me.CustomizationCode = _CustomizationCode
            Me.Client = _Client
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roProgrammedAbsenceState)
            Me.oState = _State
            Load(_ID)
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Overrides Property IDCause() As Nullable(Of Integer)
            Get
                Return intIDCause
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property RequestId() As Nullable(Of Integer)
            Get
                Return intRequestId
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intRequestId = value
            End Set
        End Property
        <DataMember()>
        Public Property IDEmployee() As Nullable(Of Integer)
            Get
                Return intIDEmployee
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property IdAbsence() As Nullable(Of Integer)
            Get
                Return iIdAbsence
            End Get
            Set(ByVal value As Nullable(Of Integer))
                iIdAbsence = value
            End Set
        End Property
        <DataMember()>
        Public Overrides Property BeginDate() As Nullable(Of DateTime)
            Get
                Return xBeginDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xBeginDate = value
            End Set
        End Property
        <DataMember()>
        Public Overrides Property FinishDate() As Nullable(Of DateTime)
            Get
                Return xFinishDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xFinishDate = value
            End Set
        End Property
        <DataMember()>
        Public Overrides Property RealFinishDate() As DateTime
            Get
                If Me.FinishDate.HasValue Then
                    Return Me.FinishDate.Value
                Else
                    Return Me.BeginDate.Value.AddDays(Me.MaxLastingDays - 1)
                End If
            End Get
            Set(value As DateTime)

            End Set

        End Property
        <DataMember()>
        Public Overrides Property MaxLastingDays() As Integer
            Get
                Return intMaxLastingDays
            End Get
            Set(ByVal value As Integer)
                intMaxLastingDays = value
            End Set
        End Property
        <DataMember()>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property
        <IgnoreDataMember()>
        Public Property State() As roProgrammedAbsenceState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roProgrammedAbsenceState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDEmployeeOriginal() As Nullable(Of Integer)
            Get
                Return intIDEmployeeOriginal
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDEmployeeOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginDateOriginal() As Nullable(Of DateTime)
            Get
                Return xBeginDateOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If (value IsNot Nothing) Then
                    xBeginDateOriginal = value
                Else
                    xBeginDateOriginal = BeginDate
                End If
            End Set
        End Property
        <DataMember()>
        Public Property FinishDateOriginal() As Nullable(Of DateTime)
            Get
                Return xFinishDateOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If (value IsNot Nothing) Then
                    xFinishDateOriginal = value
                Else
                    xFinishDateOriginal = FinishDate
                End If
            End Set
        End Property
        <DataMember()>
        Public Property MaxLastingDaysOriginal() As Integer
            Get
                Return intMaxLastingDaysOriginal
            End Get
            Set(ByVal value As Integer)
                intMaxLastingDaysOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property RealOriginalFinishDate() As DateTime
            Get
                If Me.FinishDateOriginal.HasValue Then
                    Return Me.FinishDateOriginal.Value
                Else
                    Return Me.BeginDateOriginal.Value.AddDays(Me.MaxLastingDaysOriginal - 1)
                End If
            End Get
            Set(value As DateTime)

            End Set
        End Property
        <DataMember()>
        Public Property AutomaticClosed() As Nullable(Of Boolean)
            Get
                Return bolAutomaticClosed
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolAutomaticClosed = value
            End Set
        End Property
        <DataMember()>
        Public Property RelapsedDate() As Nullable(Of Date)
            Get
                Return xRelapsedDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                xRelapsedDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roProgrammedAbsenceState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

        Private Function SQLWhere() As String

            Dim strRet As String = ""
            strRet = "ProgrammedAbsences.IDEmployee = "
            If Me.IDEmployeeOriginal.HasValue Then
                strRet &= Me.IDEmployeeOriginal.Value & " AND "
            Else
                strRet &= Me.IDEmployee.Value & " AND "
            End If
            strRet &= "ProgrammedAbsences.BeginDate = "
            If Me.BeginDateOriginal.HasValue Then
                strRet &= roTypes.Any2Time(Me.BeginDateOriginal.Value).SQLSmallDateTime
            Else
                strRet &= roTypes.Any2Time(Me.BeginDate.Value).SQLSmallDateTime
            End If

            Return strRet

        End Function

        Public Overrides Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Me.IDEmployeeOriginal = Me.IDEmployee
            Me.BeginDateOriginal = Me.BeginDate

            Me.IDCause = Nothing
            Me.FinishDate = Nothing
            Me.MaxLastingDays = 0
            Me.Description = ""
            Me.FinishDateOriginal = Nothing
            Me.MaxLastingDaysOriginal = 0
            Me.RelapsedDate = Nothing
            Me.IdAbsence = -1

            If Me.IDEmployee.HasValue And Me.BeginDate.HasValue Then
                Try

                    Dim strSQL As String = "@SELECT# * FROM ProgrammedAbsences " &
                                           "WHERE " & Me.SQLWhere()
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Me.IdAbsence = roTypes.Any2Integer(tb.Rows(0)("AbsenceID"))

                        If Not IsDBNull(tb.Rows(0)("IDCause")) Then Me.IDCause = CInt(tb.Rows(0)("IDCause"))
                        If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                            Me.FinishDate = tb.Rows(0)("FinishDate")
                            Me.FinishDateOriginal = tb.Rows(0)("FinishDate")
                        End If

                        If Not IsDBNull(tb.Rows(0)("MaxLastingDays")) Then
                            Me.MaxLastingDays = tb.Rows(0)("MaxLastingDays")
                            Me.MaxLastingDaysOriginal = tb.Rows(0)("MaxLastingDays")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Description")) Then Me.Description = tb.Rows(0)("Description")
                        If Not IsDBNull(tb.Rows(0)("AutomaticClosed")) Then Me.AutomaticClosed = tb.Rows(0)("AutomaticClosed")
                        If Not IsDBNull(tb.Rows(0)("RelapsedDate")) Then Me.RelapsedDate = tb.Rows(0)("RelapsedDate")
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", Me.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{BeginDate}", Me.BeginDate, "", 1)
                        If Me.FinishDate.HasValue Then
                            oState.AddAuditParameter(tbParameters, "{FinishDate}", Me.FinishDate, "", 1)
                        End If

                        Dim oEmpState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.oState, oEmpState)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedAbsence, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbParameters, -1)
                    End If

                    bolRet = True
                Catch ex As Data.Common.DbException
                    oState.UpdateStateInfo(ex, "roProgrammedAbsence::Load")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roProgrammedAbsence::Load")
                End Try

            End If

            Return bolRet

        End Function

        Public Overloads Function Load(_ID As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Me.IDCause = Nothing
            Me.FinishDate = Nothing
            Me.MaxLastingDays = 0
            Me.Description = ""
            Me.FinishDateOriginal = Nothing
            Me.MaxLastingDaysOriginal = 0
            Me.RelapsedDate = Nothing

            If _ID > 0 Then
                Me.IdAbsence = _ID

                Try

                    Dim strSQL As String = "@SELECT# * FROM ProgrammedAbsences " &
                                           "WHERE AbsenceID = " & _ID.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Me.IDEmployee = tb.Rows(0)("IDEmployee")
                        Me.IDEmployeeOriginal = Me.IDEmployee
                        Me.BeginDate = tb.Rows(0)("BeginDate")
                        If Not IsDBNull(tb.Rows(0)("RequestId")) Then Me.RequestId = CInt(tb.Rows(0)("RequestId"))
                        Me.BeginDateOriginal = Me.BeginDate
                        If Not IsDBNull(tb.Rows(0)("IDCause")) Then Me.IDCause = CInt(tb.Rows(0)("IDCause"))
                        If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                            Me.FinishDate = tb.Rows(0)("FinishDate")
                            Me.FinishDateOriginal = tb.Rows(0)("FinishDate")
                        End If

                        If Not IsDBNull(tb.Rows(0)("MaxLastingDays")) Then
                            Me.MaxLastingDays = tb.Rows(0)("MaxLastingDays")
                            Me.MaxLastingDaysOriginal = tb.Rows(0)("MaxLastingDays")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Description")) Then Me.Description = tb.Rows(0)("Description")
                        If Not IsDBNull(tb.Rows(0)("AutomaticClosed")) Then Me.AutomaticClosed = tb.Rows(0)("AutomaticClosed")
                        If Not IsDBNull(tb.Rows(0)("RelapsedDate")) Then Me.RelapsedDate = tb.Rows(0)("RelapsedDate")
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", Me.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{BeginDate}", Me.BeginDate, "", 1)
                        If Me.FinishDate.HasValue Then
                            oState.AddAuditParameter(tbParameters, "{FinishDate}", Me.FinishDate, "", 1)
                        End If

                        Dim oEmpState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.oState, oEmpState)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedAbsence, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbParameters, -1)
                    End If

                    bolRet = True
                Catch ex As Data.Common.DbException
                    oState.UpdateStateInfo(ex, "roProgrammedAbsence::Load")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roProgrammedAbsence::Load")
                End Try

            End If

            Return bolRet

        End Function

        Public Overrides Function Save(Optional ByVal bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean
            Dim bolRet As Boolean = False

            Dim bolIsNew As Boolean = False
            Dim bolIsExported As Boolean = False
            Dim strVPACleanLog As String = String.Empty
            Dim lDeletedForecast As New List(Of roProgrammedAbsence)

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = ProgrammedAbsencesResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                bolRet = ValidateProgrammedAbsence(Me, Me.State)

                If bolRet OrElse ((Me.State.Result = ProgrammedAbsencesResultEnum.AnotherExistInDateInterval OrElse Me.State.Result = ProgrammedAbsencesResultEnum.AnotherOvertimeExistInDate) AndAlso Me.CustomizationCode = "VPA" AndAlso Me.Client.ToUpper = "DATAIMPORT") Then

                    If Me.CustomizationCode = "VPA" AndAlso Me.Client.ToUpper = "DATAIMPORT" Then
                        ' Miro si en el periodo de la ausencia hay vacaciones (la funcion ValidateProgrammedAbsence no lo hace)
                        Dim bCheckOnlyHolidays As Boolean = bolRet
                        ' APV: Si hay alguna previsión o vacaciones solapada, lo arreglo. Esta solicitud debe entrar Si o Si
                        bolRet = CleanOverlappings(Me, bCheckOnlyHolidays, strVPACleanLog, lDeletedForecast)
                        If bolRet AndAlso strVPACleanLog.Length > 0 Then
                            Me.State.ErrorText = strVPACleanLog
                        End If
                    End If

                    If bolRet Then

                        bolRet = False

                        Dim tb As New DataTable("ProgrammedAbsences")
                        Dim strSQL As String = "@SELECT# * FROM ProgrammedAbsences WHERE " & Me.SQLWhere()
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim _BeginDate As Date = Me.BeginDate.Value
                        Dim _FinishDate As Date
                        If Me.FinishDate.HasValue Then
                            _FinishDate = Me.FinishDate.Value
                        Else
                            _FinishDate = DateAdd(DateInterval.Day, Me.MaxLastingDays - 1, Me.BeginDate.Value)
                        End If

                        Dim _ActualFinishDate As Date = _FinishDate

                        Dim _IDCauseOld As Integer = Me.IDCause.Value

                        Dim _FreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee.Value, False, Me.oState)

                        Dim bolLaunchUpdate As Boolean = True

                        Dim oAuditDataOld As DataRow = Nothing
                        Dim oAuditDataNew As DataRow = Nothing

                        Dim _FinishDateOld As Date

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            bolIsNew = True
                        Else
                            oRow = tb.Rows(0)
                            oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                            Dim _BeginDateOld As Date = oRow("BeginDate")
                            If Not IsDBNull(oRow("FinishDate")) Then
                                _FinishDateOld = oRow("FinishDate")
                            Else
                                _FinishDateOld = DateAdd(DateInterval.Day, oRow("MaxLastingDays") - 1, _BeginDateOld)
                            End If

                            _IDCauseOld = oRow("IDCause")

                            'Si todos los datos coinciden, no lanzamos recalculo
                            If _BeginDate = _BeginDateOld And _FinishDate = _FinishDateOld And _IDCauseOld = Me.IDCause.Value Then
                                bolLaunchUpdate = False
                            End If

                            _BeginDate = IIf(_BeginDateOld < _BeginDate, _BeginDateOld, _BeginDate)
                            _FinishDate = IIf(_FinishDateOld > _FinishDate, _FinishDateOld, _FinishDate)

                            'Fecha de congelación
                            _BeginDate = IIf(_BeginDate <= _FreezeDate, _FreezeDate, _BeginDate)
                            _BeginDateOld = IIf(_BeginDateOld <= _FreezeDate, _FreezeDate, _BeginDateOld)

                            ' APV
                            If Me.CustomizationCode = "VPA" AndAlso Me.Client.ToUpper = "DATAIMPORT" Then
                                Try
                                    Dim oExistingAbsence As New roProgrammedAbsence(oRow("AbsenceID"), oState)
                                    If strVPACleanLog.Length = 0 Then
                                        strVPACleanLog = Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.1", "", "Para poder importar la previsión <b>") & Me.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.2", "", "</b> se realizaron las siguientes modificaciones:<br> <br>" & vbCrLf & vbCrLf)
                                    End If
                                    If _IDCauseOld <> Me.IDCause Then
                                        strVPACleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceCauseModified.1", "", "- Modificar el motivo de la previsión <b> ") & oExistingAbsence.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceCauseModified.2", "", "</b><br>" & vbCrLf)
                                    Else
                                        strVPACleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceEndModified.1", "", "- Modificar fecha de finalización de la previsión <b> ") & oExistingAbsence.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceEndModified.2", "", "</b><br>" & vbCrLf)
                                    End If
                                    If strVPACleanLog.Length > 0 Then
                                        strVPACleanLog = "#APV*" & strVPACleanLog & "<br><br>"
                                        Me.State.ErrorText = strVPACleanLog
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                        End If

                        oRow("IDCause") = Me.IDCause.Value
                        If Me.RequestId.HasValue Then
                            oRow("RequestId") = Me.RequestId.Value
                        Else
                            oRow("RequestId") = DBNull.Value
                        End If

                        oRow("IDEmployee") = Me.IDEmployee.Value
                        oRow("BeginDate") = Me.BeginDate.Value
                        If Me.FinishDate.HasValue Then
                            oRow("FinishDate") = Me.FinishDate.Value
                        Else
                            oRow("FinishDate") = DBNull.Value
                        End If
                        oRow("MaxLastingDays") = Me.MaxLastingDays
                        oRow("Description") = Me.Description

                        If Me.RelapsedDate.HasValue Then
                            oRow("RelapsedDate") = Me.RelapsedDate
                        Else
                            oRow("RelapsedDate") = DBNull.Value
                        End If

                        If IsDBNull(oRow("IsExported")) Then oRow("IsExported") = False

                        'Si se han realizado cambios, actualizamos
                        If bolLaunchUpdate Then
                            oRow("AutomaticClosed") = 0
                            oRow("IsExported") = False
                        End If

                        oRow("Timestamp") = Now

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        bolRet = True

                        ' Recupero el ID asignado al fichaje para guardar la foto, si aplica
                        If bolIsNew Then
                            Dim tmpId As DataTable = CreateDataTable("@SELECT# TOP 1 [AbsenceID] FROM ProgrammedAbsences WHERE IDEmployee=" & Me.intIDEmployee.ToString & " AND IDCause= " & Me.intIDCause.ToString & " ORDER BY [AbsenceID] DESC")

                            If tmpId IsNot Nothing AndAlso tmpId.Rows.Count = 1 Then
                                Me.iIdAbsence = tmpId.Rows(0)("AbsenceID")
                                oRow("AbsenceID") = Me.iIdAbsence

                                ' Si se creó una nueva, y se borró una existente que tenía documentación asociada, se asocia la documentación existente para la antigua, a la nueva
                                If lDeletedForecast.Count > 0 Then
                                    For Each oDeletedForecast As roProgrammedAbsence In lDeletedForecast
                                        strSQL = "@UPDATE# Documents SET IdDaysAbsence = " & Me.iIdAbsence.ToString & " WHERE IdDaysAbsence = " & oDeletedForecast.IdAbsence.ToString & " AND IdEmployee = " & Me.IDEmployee.ToString
                                        bolRet = ExecuteSql(strSQL)
                                        If Not bolRet Then
                                            Me.State.ErrorText = String.Empty
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        ' TODO: Si se cambió alguna fecha de la ausencia, y tenemos documentos asociados, los guardamos para que se actualice estado si corresponde

                        ' Auditamos
                        If bolRet And bAudit Then
                            oAuditDataNew = oRow
                            Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                            Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                            Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                            Dim oEmpState As New Employee.roEmployeeState()
                            roBusinessState.CopyTo(Me.oState, oEmpState)

                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tProgrammedAbsence, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbAuditParameters, -1)
                        End If

                        ' Notificamos cambio al servidor
                        If bolRet And bolLaunchUpdate Then

                            'Actualiza la table de DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "Date >= " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND Date <= " & roTypes.Any2Time(_FinishDate).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Elimina las Causas
                            strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & Me.IDEmployee.Value & " AND Date >= " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " &
                                     "Date <= " & roTypes.Any2Time(_FinishDate).SQLSmallDateTime & " AND IDCause = " & _IDCauseOld
                            ExecuteSqlWithoutTimeOut(strSQL)

                            If _IDCauseOld <> Me.IDCause.Value Then
                                strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                               "Date >= " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND Date <= " & roTypes.Any2Time(_FinishDate).SQLSmallDateTime & " AND " &
                                               "IDCause = " & Me.IDCause.Value
                                ExecuteSqlWithoutTimeOut(strSQL)
                            End If

                            ' Crea la tarea de Ausencias programadas
                            If CreateTask Then
                                Dim oContext As New roCollection
                                oContext.Add("User.ID", Me.IDEmployee.Value)
                                oContext.Add("Date", Now.Date)
                                VTBase.Extensions.roConnector.InitTask(TasksType.MOVES, oContext)
                            End If

                            ' Especial TorrasPapel
                            Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                            If roTypes.Any2String(customization) = "SARROT" Then
                                ' Marco para envío a Meta4. Sea de cuando sea la ausencia, marco el día de hoy para que se envíe ya. De todos modos, se envían todas desde cero ...
                                strSQL = "@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET SENDABSENCE=0 WHERE IDEMPLOYEE=" & Me.IDEmployee.Value & " And DATE=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime 'getdate()"
                                ExecuteSql(strSQL)
                            End If

                            ' Especial UPF
                            If roTypes.Any2String(customization) = "UEPMOP" Then
                                ' Marco para recalcuo el dia uno del año en curso a partir de la fecha de inicio de la previsión, y si el año es anterior el del año pasado y el actual
                                ' Para recalcular los valores iniciales de vacaciones y asuntos propios en caso necesario
                                RecalculateStartUpValues_UPF(Me.IDEmployee.Value, _BeginDate, _FreezeDate)
                            End If

                            ' Borramos notificaciones de empleado ausente dentro del periodo de la ausencia prevista
                            strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (15,13)) AND Key1Numeric = " & Me.IDEmployee.Value & " AND Key3DateTime >= " & roTypes.Any2Time(_BeginDate).SQLDateTime & " AND Key3DateTime <= " & roTypes.Any2Time(_FinishDate.AddDays(1).AddMinutes(-1)).SQLDateTime
                            ExecuteSqlWithoutTimeOut(strSQL)

                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence:Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence:Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function CleanOverlappings(oProgrammedAbsence As roProgrammedAbsence, bCheckOnlyHolidays As Boolean, ByRef strCleanLog As String, ByRef lDeletedForecast As List(Of roProgrammedAbsence)) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim queryDateStart As String = roTypes.Any2Time(oProgrammedAbsence.BeginDate.Value).SQLSmallDateTime()
                Dim queryDateEnd As String = roTypes.Any2Time(oProgrammedAbsence.RealFinishDate).SQLSmallDateTime()

                Dim strSQL As String
                Dim tb As DataTable

                Dim dForecastStart As Date = Date.MinValue
                Dim dForecastEnd As Date = Date.MinValue
                Dim iForecastIdEmployee As Integer = -1

                Dim oForecast As Forecast.roForecast = Nothing

                Dim oDeletedForecast As roProgrammedAbsence = Nothing

                If Not bCheckOnlyHolidays Then

                    '1.- Ausencias programadas por días y por horas, y horas de exceso
                    strSQL = "@SELECT# 'D' as Type, IDEmployee, BeginDate, ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) as EndDate, AbsenceID as ID from ProgrammedAbsences " &
                         " WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value &
                         " AND AbsenceID <> " & oProgrammedAbsence.IdAbsence.ToString &
                         " AND (BeginDate <= " & queryDateEnd & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & ")" &
                         " UNION " &
                         " @SELECT# 'H' as Type, IDEmployee, Date as BeginDate, IsNULL(FinishDate,Date) as EndDate, AbsenceID as ID from ProgrammedCauses " &
                         " WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value &
                         " AND (Date <= " & queryDateEnd & " AND IsNULL(FinishDate,Date) >= " & queryDateStart & ")" &
                         " UNION " &
                         " @SELECT# 'O' as Type, IDEmployee, BeginDate, EndDate, ID from ProgrammedOvertimes " &
                         " WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value &
                         " AND (BeginDate <= " & queryDateEnd & " AND EndDate >= " & queryDateStart & ")"

                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        strCleanLog = Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.1", "", "Para poder importar la previsión <b>") & oProgrammedAbsence.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.2", "", "</b> se realizaron las siguientes modificaciones:<br> <br>" & vbCrLf & vbCrLf)
                        For Each oRow As DataRow In tb.Rows
                            Dim sType As String = String.Empty
                            Dim iID As Long

                            dForecastStart = roTypes.Any2DateTime(oRow("BeginDate"))
                            dForecastEnd = roTypes.Any2DateTime(oRow("EndDate"))
                            iForecastIdEmployee = roTypes.Any2Integer(oRow("IDEmployee"))
                            sType = roTypes.Any2String(oRow("Type"))
                            iID = roTypes.Any2Long(oRow("ID"))

                            Select Case sType.ToUpper
                                Case "D"
                                    oForecast = New roProgrammedAbsence(IDEmployee, dForecastStart, New roProgrammedAbsenceState(Me.State.IDPassport))
                                Case "H"
                                    oForecast = New Incidence.roProgrammedCause(IDEmployee, dForecastStart, Nothing, New Incidence.roProgrammedCauseState(Me.State.IDPassport))
                                Case "O"
                                    oForecast = New Forecast.roProgrammedOvertimeProxy(iID, IDEmployee, dForecastStart)
                            End Select
                            oForecast.Load()

                            If oForecast.BeginDate.Value >= oProgrammedAbsence.BeginDate AndAlso oForecast.RealFinishDate <= oProgrammedAbsence.RealFinishDate Then
                                ' Se solapa completamente. La borro.
                                ' ---->--E--<-----
                                ' ->-----N-----<--
                                If oForecast.IDCause = oProgrammedAbsence.IDCause AndAlso TypeOf oForecast Is roProgrammedAbsence Then
                                    ' Se mantiene la documentación de la antigua previsión, en la nueva
                                    oDeletedForecast = New roProgrammedAbsence
                                    oDeletedForecast = oForecast.SurfaceCopy
                                    lDeletedForecast.Add(oDeletedForecast)
                                End If
                                ' Elimino la existente
                                bolRet = oForecast.Delete()
                                If bolRet Then
                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.1", "", "- Eliminar previsión <b>") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.2", "", "</b><br>" & vbCrLf)
                                Else
                                    Exit For
                                End If
                            ElseIf oForecast.BeginDate.Value >= oProgrammedAbsence.BeginDate AndAlso oForecast.RealFinishDate >= oProgrammedAbsence.RealFinishDate Then
                                ' Modifico la fecha inicial al día después del final de la nueva ausencia
                                ' ----->----E----<----
                                ' ->-----N----<-------
                                If oForecast.IDCause <> oProgrammedAbsence.IDCause OrElse TypeOf oForecast IsNot roProgrammedAbsence Then
                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceBeginModified.1", "", "- Modificar fecha de inico de previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceBeginModified.2", "", "</b><br>" & vbCrLf)
                                    oForecast.BeginDate = oProgrammedAbsence.RealFinishDate.AddDays(1)
                                    bolRet = oForecast.Save()
                                    If Not bolRet Then
                                        strCleanLog = String.Empty
                                        Exit For
                                    End If
                                Else
                                    ' Misma justificación en ausencia por días. Borro la ausencia existente y modifico los límites de la nueva
                                    ' Se mantiene la documentación de la antigua previsión, en la nueva
                                    oDeletedForecast = New roProgrammedAbsence
                                    oDeletedForecast = oForecast.SurfaceCopy
                                    lDeletedForecast.Add(oDeletedForecast)

                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.1", "", "- Eliminar la previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.2", "", "</b><br>" & vbCrLf)
                                    bolRet = oForecast.Delete()
                                    If bolRet Then
                                        Me.FinishDate = oForecast.RealFinishDate
                                        Me.MaxLastingDays = oForecast.MaxLastingDays
                                    Else
                                        Exit For
                                    End If
                                End If
                            ElseIf oForecast.BeginDate.Value <= oProgrammedAbsence.BeginDate AndAlso oForecast.RealFinishDate <= oProgrammedAbsence.RealFinishDate Then
                                ' Modifico la fecha final al día anterior al inicio de la nueva ausencia
                                ' --->----E----<----
                                ' ------->----N----<--
                                If oForecast.IDCause <> oProgrammedAbsence.IDCause OrElse TypeOf oForecast IsNot roProgrammedAbsence Then
                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceEndModified.1", "", "- Modificar fecha de finalización de previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceEndModified.2", "", "</b><br>" & vbCrLf)
                                    If oForecast.FinishDate.HasValue Then
                                        oForecast.FinishDate = oProgrammedAbsence.BeginDate.Value.AddDays(-1)
                                    Else
                                        oForecast.MaxLastingDays = DateDiff(DateInterval.Day, oForecast.BeginDate.Value, oProgrammedAbsence.BeginDate.Value)
                                    End If
                                    bolRet = oForecast.Save()
                                    If Not bolRet Then
                                        strCleanLog = String.Empty
                                        Exit For
                                    End If
                                Else
                                    ' Misma justificación en ausencia por días. Borro la ausencia existente y modifico los límites de la nueva
                                    ' Se mantiene la documentación de la antigua previsión, en la nueva
                                    oDeletedForecast = New roProgrammedAbsence
                                    oDeletedForecast = oForecast.SurfaceCopy
                                    lDeletedForecast.Add(oDeletedForecast)

                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.1", "", "- Eliminar la previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.2", "", "</b><br>" & vbCrLf)
                                    bolRet = oForecast.Delete()
                                    If bolRet Then
                                        Me.BeginDate = oForecast.BeginDate.Value
                                    Else
                                        strCleanLog = String.Empty
                                        Exit For
                                    End If
                                End If
                            ElseIf oForecast.BeginDate.Value < oProgrammedAbsence.BeginDate AndAlso oForecast.RealFinishDate > oProgrammedAbsence.RealFinishDate Then
                                ' Hay que partir la existente en dos, que no solapen con la nueva
                                ' ->-----E-----<----
                                ' ---->--N--<-------
                                ' 1.- Creo una nueva que empiece el día después de que finalice la nueva, y acabe cuando acababa la original
                                Dim iOriginalForecastId As Integer = -1
                                Dim iNewForecastId As Integer = -1
                                If oForecast.IDCause <> oProgrammedAbsence.IDCause OrElse TypeOf oForecast IsNot roProgrammedAbsence Then
                                    Dim oNewForecast As Forecast.roForecast = Nothing
                                    bolRet = True
                                    If TypeOf oForecast Is roProgrammedAbsence Then
                                        oNewForecast = New roProgrammedAbsence
                                        oNewForecast = oForecast.SurfaceCopy
                                        iOriginalForecastId = CType(oForecast, roProgrammedAbsence).IdAbsence
                                    ElseIf TypeOf oForecast Is Incidence.roProgrammedCause Then
                                        oNewForecast = New Incidence.roProgrammedCause
                                        oNewForecast = oForecast.SurfaceCopy
                                        CType(oNewForecast, Incidence.roProgrammedCause).AbsenceID = -1
                                        iOriginalForecastId = CType(oForecast, Incidence.roProgrammedCause).AbsenceID
                                    ElseIf TypeOf oForecast Is Forecast.roProgrammedOvertimeProxy Then
                                        oNewForecast = New Forecast.roProgrammedOvertimeProxy(iID, IDEmployee, dForecastStart)
                                        oNewForecast.Load()
                                        CType(oNewForecast, Forecast.roProgrammedOvertimeProxy).ID = -1
                                        iOriginalForecastId = CType(oForecast, Forecast.roProgrammedOvertimeProxy).ID
                                        Dim oForecastAux As New Forecast.roProgrammedOvertimeProxy(iID, IDEmployee, dForecastStart)
                                        oForecastAux.Load()
                                        bolRet = oForecastAux.Delete()
                                        CType(oForecast, Forecast.roProgrammedOvertimeProxy).ID = -1
                                    Else
                                        bolRet = False
                                        ' Tipo desconocido
                                        strCleanLog = String.Empty
                                        Exit For
                                    End If
                                    If bolRet Then
                                        oNewForecast.BeginDate = oProgrammedAbsence.RealFinishDate.AddDays(1)
                                        bolRet = oNewForecast.Save()
                                        If bolRet Then
                                            strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceSplit.1", "", "- Dividir en dos la previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceSplit.2", "", "</b><br>" & vbCrLf)
                                            ' 2.- La primera la finalizo el día antes de que empiece la nueva
                                            If oForecast.FinishDate.HasValue Then
                                                oForecast.FinishDate = oProgrammedAbsence.BeginDate.Value.AddDays(-1)
                                            Else
                                                oForecast.MaxLastingDays = DateDiff(DateInterval.Day, oForecast.BeginDate.Value, oProgrammedAbsence.BeginDate.Value)
                                            End If
                                            bolRet = oForecast.Save()
                                            If Not bolRet Then
                                                strCleanLog = String.Empty
                                                Exit For
                                            End If
                                            ' Si cambió el ID de la ausencia original, actualizo posibles documentos que hayan quedado mal asignados
                                            If TypeOf oForecast Is roProgrammedAbsence Then
                                                iNewForecastId = CType(oForecast, roProgrammedAbsence).IdAbsence
                                            ElseIf TypeOf oForecast Is Incidence.roProgrammedCause Then
                                                iNewForecastId = CType(oForecast, Incidence.roProgrammedCause).AbsenceID
                                            ElseIf TypeOf oForecast Is Forecast.roProgrammedOvertimeProxy Then
                                                iNewForecastId = CType(oForecast, Forecast.roProgrammedOvertimeProxy).ID
                                            End If
                                            If iOriginalForecastId <> iNewForecastId AndAlso (TypeOf oForecast Is roProgrammedAbsence Or TypeOf oForecast Is Incidence.roProgrammedCause) Then
                                                If TypeOf oForecast Is roProgrammedAbsence Then
                                                    strSQL = "@UPDATE# Documents SET IdDaysAbsence = " & iNewForecastId.ToString & " WHERE IdDaysAbsence = " & iOriginalForecastId.ToString & " AND IdEmployee = " & Me.IDEmployee.ToString
                                                ElseIf TypeOf oForecast Is Incidence.roProgrammedCause Then
                                                    strSQL = "@UPDATE# Documents SET IdHoursAbsence = " & iNewForecastId.ToString & " WHERE IdHoursAbsence = " & iOriginalForecastId.ToString & " AND IdEmployee = " & Me.IDEmployee.ToString
                                                End If
                                                bolRet = ExecuteSql(strSQL)
                                                If Not bolRet Then
                                                    strCleanLog = String.Empty
                                                    Exit For
                                                End If
                                            End If
                                        Else
                                            Exit For
                                        End If
                                    Else
                                        Exit For
                                    End If
                                Else
                                    ' Misma justificación en ausencia por días. Borro la ausencia existente y modifico los límites de la nueva
                                    ' Se mantiene la documentación de la antigua previsión, en la nueva
                                    oDeletedForecast = New roProgrammedAbsence
                                    oDeletedForecast = oForecast.SurfaceCopy
                                    lDeletedForecast.Add(oDeletedForecast)

                                    strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.1", "", "- Eliminar la previsión <b> ") & oForecast.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDeleted.2", "", "</b><br>" & vbCrLf)
                                    bolRet = oForecast.Delete()
                                    If bolRet Then
                                        Me.BeginDate = oProgrammedAbsence.BeginDate
                                        Me.FinishDate = oProgrammedAbsence.RealFinishDate
                                        Me.MaxLastingDays = oProgrammedAbsence.MaxLastingDays
                                    Else
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If

                '2.- Horarios de Vacaciones
                If bolRet Then
                    strSQL = "@SELECT# * FROM DailySchedule " &
                         " WHERE IDShift1 IN (@SELECT# Id FROM Shifts WHERE ShiftType = 2) " &
                         " AND Date BETWEEN " & queryDateStart & " and " & queryDateEnd &
                         " AND IDEmployee = " & oProgrammedAbsence.IDEmployee.Value
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        If strCleanLog.Length = 0 Then
                            strCleanLog = Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.1", "", "Para poder importar la previsión <b>") & oProgrammedAbsence.ToString() & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.AbsenceDetail.2", "", "</b> se realizaron las siguientes modificaciones:<br><br>" & vbCrLf & vbCrLf)
                        End If
                        Dim sDetail As String = String.Empty
                        For Each oRow As DataRow In tb.Rows
                            sDetail &= roTypes.Any2DateTime(oRow("Date")).ToShortDateString & ", "
                        Next
                        If sDetail.EndsWith(", ") Then sDetail = sDetail.Substring(0, sDetail.Length - 2)
                        '0.- Elimino
                        strCleanLog &= Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.DeletedHolidays.1", "", "- Eliminar días planificados como vacaciones: <b>") & sDetail & Me.oState.Language.TranslateWithDefault("VAPAbsenceImport.DeletedHolidays.2", "", "</b><br>" & vbCrLf)
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IDPreviousShift = IDShift1, IDShift1 = IDShiftBase, IDShiftBase = NULL, IsHolidays = 0, Status = 0, [GUID] = '' " &
                             " WHERE IDShift1 IN (@select# id from shifts where shifttype = 2) " &
                             " AND Date BETWEEN " & queryDateStart & " and " & queryDateEnd &
                             " AND IDEmployee = " & oProgrammedAbsence.IDEmployee.Value
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then
                            strCleanLog = String.Empty
                        End If
                    Else
                        bolRet = True
                    End If
                End If
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try
            If strCleanLog.Length > 0 Then strCleanLog = "#APV*" & strCleanLog & "<br><br>"
            Return bolRet
        End Function

        Public Overrides Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Dim strSql As String = "@DELETE# FROM ProgrammedAbsences WHERE " & Me.SQLWhere()
                'bolRet = ExecuteSql(strSql)

                Dim tb As New DataTable("ProgrammedAbsences")
                Dim strSQL As String = "@SELECT# * FROM ProgrammedAbsences WHERE " & Me.SQLWhere()
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows.Count > 0 Then

                    If Not IsDBNull(tb.Rows(0).Item("IDCause")) Then Me.IDCause = CInt(tb.Rows(0).Item("IDCause"))
                    If Not IsDBNull(tb.Rows(0).Item("FinishDate")) Then Me.FinishDate = tb.Rows(0).Item("FinishDate")
                    If Not IsDBNull(tb.Rows(0).Item("MaxLastingDays")) Then Me.MaxLastingDays = tb.Rows(0).Item("MaxLastingDays")
                    If Not IsDBNull(tb.Rows(0).Item("AbsenceID")) Then Me.IdAbsence = CInt(tb.Rows(0).Item("AbsenceID"))

                    tb.Rows(0).Delete()

                    da.Update(tb)

                    bolRet = True
                End If

                'Comprovem si es troba en periode de congelació
                Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee.Value, False, Me.State)
                If bolRet Then
                    If Me.BeginDate.Value <= freezeDate Then
                        Me.State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                        bolRet = False
                    End If

                    If Me.RealFinishDate <= freezeDate Then
                        Me.State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    ' En el caso de que tenga el parametro avanzado de no permitir gestionar justificaciones que
                    ' no esten dentro de sus grupos de negocio, lo validamos
                    If roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("BusinessGroup.ApplyNotAllowedModifyCause", New AdvancedParameter.roAdvancedParameterState).Value) = "1" Then
                        Dim oCause As New Cause.roCause(-1, New Cause.roCauseState(Me.State.IDPassport))
                        Dim tbCauses As DataTable = oCause.Causes("", True)

                        If tbCauses IsNot Nothing Then
                            Dim oRows() As DataRow = tbCauses.Select("ID = " & Me.IDCause)
                            If oRows IsNot Nothing AndAlso oRows.Length = 0 Then
                                Me.State.Result = ProgrammedAbsencesResultEnum.NotAllowedCause
                                bolRet = False
                            End If
                        End If
                    End If
                End If

                If bolRet Then
                    ' Borramos la causa de la ausencia dentro de los limites de la ausencia
                    strSQL = "@DELETE# DailyCauses " &
                             "WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                   "IDCause = " & Me.IDCause.Value & " AND " &
                                   "Date >= " & roTypes.Any2Time(Me.BeginDate.Value).SQLSmallDateTime() & " AND " &
                                   "Date <= " & roTypes.Any2Time(Me.RealFinishDate).SQLSmallDateTime() & " "
                    bolRet = ExecuteSql(strSQL)

                    ' Actualizamos los datos de las dotaciones en caso necesario
                    Dim dtActualDate As Date
                    dtActualDate = Me.BeginDate.Value
                    While dtActualDate <= Me.RealFinishDate
                        Dim bolNotify As Boolean = False
                        If dtActualDate = Me.RealFinishDate Then bolNotify = True
                        Dim oSchedulerState As New Scheduler.roSchedulerState
                        roBusinessState.CopyTo(oState, oSchedulerState)
                        bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_All, oSchedulerState, Me.IDEmployee.Value, , dtActualDate, bolNotify)
                        roBusinessState.CopyTo(oSchedulerState, oState)
                        dtActualDate = dtActualDate.AddDays(1)
                    End While

                End If

                'If bolRet AndAlso Me.IdAbsence.HasValue AndAlso Me.IdAbsence.Value > 0 Then
                '    ' Borramos los documentos de la ausencia
                '    strSQL = "@DELETE# Documents WHERE IdEmployee =  " & Me.IDEmployee.Value & " and IdDaysAbsence = " & Me.IdAbsence.Value
                '    bolRet = ExecuteSql(strSQL)
                'End If

                If bolRet Then
                    ' Notificamos el borrado de la ausencia
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                             "SET Status = 45, [GUID] = '' " &
                             "WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                   "Date >= " & roTypes.Any2Time(Me.BeginDate.Value).SQLSmallDateTime() & " AND " &
                                   "Date <= " & roTypes.Any2Time(Me.RealFinishDate).SQLSmallDateTime()

                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                ' ESPECIAL: TorrasPapel / UPF
                If bolRet Then
                    Try
                        Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                        If roTypes.Any2String(customization) = "SARROT" Then
                            ' Marco para envío a Meta4. Sea de cuando sea la ausencia, marco el día de hoy para que se envíe ya. De todos modos, se envían todas desde cero ...
                            strSQL = "@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET SENDABSENCE=0 WHERE IDEMPLOYEE=" & Me.IDEmployee.Value & " And DATE=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime 'getdate()"
                            ExecuteSql(strSQL)
                        End If

                        ' Especial UPF
                        If roTypes.Any2String(customization) = "UEPMOP" Then
                            ' Marco para recalcuo el dia uno del año en curso a partir de la fecha de inicio de la previsión, y si el año es anterior el del año pasado y el actual
                            ' Para recalcular los valores iniciales de vacaciones y asuntos propios en caso necesario
                            RecalculateStartUpValues_UPF(Me.IDEmployee.Value, Me.BeginDate.Value, freezeDate)
                        End If
                    Catch ex As Exception
                    End Try
                End If

                ' Si es preciso, borro tareas de notificación de ausencias
                If bolRet Then
                    Try
                        strSQL = "@DELETE# sysroNotificationTasks WHERE Key1Numeric = " & Me.IDEmployee.Value & " AND Key5Numeric = " & Me.IdAbsence & " AND IDNotification = 701 AND Parameters LIKE 'DAYS%'"
                        bolRet = ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try
                End If

                If bolRet Then
                    Try
                        strSQL = "@INSERT# INTO DeletedProgrammedAbsences VALUES (" & Me.IDEmployee.Value & "," & Me.IdAbsence & "," & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                        bolRet = ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim oEmpState As New Employee.roEmployeeState()
                    roBusinessState.CopyTo(Me.oState, oEmpState)

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProgrammedAbsence, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), Nothing, -1)
                End If

                Dim oContext As New roCollection
                oContext.Add("User.ID", Me.IDEmployee.Value)
                oContext.Add("Date", Now.Date)
                roConnector.InitTask(TasksType.MOVES, oContext)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetLastValidEnddate(ByVal _IDEmployee As Integer, ByVal dFromDate As Date, ByVal iMaxDays As Integer, ByVal _State As roProgrammedAbsenceState) As Date

            Dim dRet As Date = Nothing
            Try

                Dim strSQL As String
                Dim strFromDate As String = roTypes.Any2Time(dFromDate).SQLSmallDateTime

                strSQL = "@SELECT# MIN(tmp.SDate) AS EndDate FROM ( " &
                            "@SELECT# MIN(BeginDate) AS SDate FROM ProgrammedAbsences WHERE IDEmployee = " & _IDEmployee & " AND BeginDate BETWEEN " & strFromDate & " AND DATEADD(day," & iMaxDays.ToString & "," & strFromDate & ") " &
                            "UNION " &
                            "@SELECT# MIN(Date) AS SDate FROM ProgrammedCauses WHERE IDEmployee = " & _IDEmployee & " AND Date BETWEEN " & strFromDate & " AND DATEADD(day," & iMaxDays.ToString & "," & strFromDate & ") " &
                            "UNION " &
                            "@SELECT# MIN(BeginDate) AS SDate FROM ProgrammedOvertimes WHERE IDEmployee = " & _IDEmployee & " AND BeginDate BETWEEN " & strFromDate & " AND DATEADD(day," & iMaxDays.ToString & "," & strFromDate & ") " &
                            "UNION " &
                            "@SELECT# MIN(Date) AS SDate FROM ProgrammedHolidays WHERE  IDEmployee = " & _IDEmployee & " AND Date BETWEEN " & strFromDate & " AND DATEADD(day," & iMaxDays.ToString & "," & strFromDate & ") " &
                            "UNION " &
                            "@SELECT# DATEADD(day,1,MIN(EndDate)) AS SDate FROM EmployeeContracts WHERE IDEmployee = " & _IDEmployee & " AND  EndDate BETWEEN " & strFromDate & " AND DATEADD(day," & iMaxDays.ToString & "," & strFromDate & "))tmp "

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 AndAlso Not IsDBNull(tb.Rows(0)("EndDate")) Then
                    Dim oRow As DataRow = tb.Rows(0)
                    dRet = oRow("EndDate")
                    dRet = dRet.AddDays(-1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetLastValidEnddate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetLastValidEnddate")
            Finally

            End Try

            Return dRet
        End Function

        ''' <summary>
        ''' Devuelve las bajas del empleado
        ''' </summary>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="showAll"></param>
        ''' <param name="dateStart"></param>
        ''' <param name="dateEnd"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveConnection"></param>
        ''' <returns></returns>
        Public Shared Function GetLeaves(ByVal _IDEmployee As Integer, ByVal showAll As Boolean, ByVal dateStart As String, ByVal dateEnd As String, ByVal _State As roProgrammedAbsenceState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# AbsenceID, ProgrammedAbsences.IDCause, IDEmployee, BeginDate, FinishDate, MaxLastingDays, ProgrammedAbsences.Description AS Description, RelapsedDate, Causes.Name AS CauseName, CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate " &
                        "From ProgrammedAbsences " &
                        "Left Join Causes On Causes.ID = ProgrammedAbsences.IDCause  " &
                        "LEFT JOIN (@SELECT# row_number() over (partition by IdCause order by IdDocument desc) As 'RowNumber', IDCause, IDDocument from CausesDocumentsTracking) CDT on CDT.IDCause = ProgrammedAbsences.IDCause  " &
                        "Left Join DocumentTemplates On DocumentTemplates.Id = CDT.IDDocument " &
                        "WHERE IDEmployee = " & _IDEmployee & " and RowNumber = 1 and DocumentTemplates.Scope = " & DTOs.DocumentScope.LeaveOrPermission

                If Not showAll Then
                    strSQL = strSQL & " And BeginDate BETWEEN " & roTypes.Any2Time(dateStart).SQLSmallDateTime & " And " & roTypes.Any2Time(dateEnd).SQLSmallDateTime
                End If

                strSQL = strSQL & " ORDER BY BeginDate DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence:: GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedAbsences(ByVal _IDEmployee As Integer, ByVal _State As roProgrammedAbsenceState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, IDEmployee, BeginDate, FinishDate, MaxLastingDays, ProgrammedAbsences.Description, RelapsedDate, " &
                                "Causes.Name, " &
                                "CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedAbsences.AbsenceID  AND adf.forecasttype='days') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered " &
                         "FROM ProgrammedAbsences " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee & " " &
                         "ORDER BY BeginDate DESC"

                oRet = CreateDataTable(strSQL, )

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedAbsences(ByVal _IDEmployee As Integer, ByVal strWhere As String, ByVal _State As roProgrammedAbsenceState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, IDEmployee, BeginDate, FinishDate, MaxLastingDays, ProgrammedAbsences.Description, RelapsedDate, " &
                                "Causes.Name, " &
                                "CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate " &
                         "FROM ProgrammedAbsences " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee
                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY BeginDate DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetMyTeamAbsencesBetweenDates(ByVal idemployee As Integer, ByVal beginDate As Date, ByVal endDate As Date, ByVal _State As roProgrammedAbsenceState) As DataTable
            ' Obtiene las ausencias que hay en el periodo
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String

                Dim strFromDate As String = roTypes.Any2Time(beginDate).SQLSmallDateTime
                Dim strToDate As String = roTypes.Any2Time(endDate).SQLSmallDateTime

                strSQL = "@select# * from GetMyTeamAbsencesBetweenDates (" & strFromDate & ", " & strToDate & ", " & idemployee & ")"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetMyTeamAbsencesBetweenDates")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetMyTeamAbsencesBetweenDates")
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetProgrammedAbsences(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _State As roProgrammedAbsenceState) As DataTable
            ' Obtiene las ausencias que hay en el periodo
            Dim oRet As DataTable = Nothing
            Try

                ' Si es para todos los empleados, recupero el campo de ficha usado como identificador único
                Dim strUniqueidentifierField As String = String.Empty
                If _IDEmployee = -1 Then
                    strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value)
                End If

                Dim strSQL As String

                If _IDEmployee > 0 Then
                    strSQL = "@SELECT# IDCause, ProgrammedAbsences.IDEmployee, BeginDate, FinishDate, MaxLastingDays, ProgrammedAbsences.Description, RelapsedDate, " &
                                "'' AS NIF, " &
                                "'' AS IdImport, " &
                                "Causes.Name, " &
                                "CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedAbsences.AbsenceID  AND adf.forecasttype='days') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, AbsenceID, " &
                                "Timestamp, " &
                                "'Day' AS Type,  " &
                                "'CRU' AS Action  " &
                         "FROM ProgrammedAbsences " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee & " AND " &
                               "((BeginDate BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                                "((CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                                "(BeginDate < " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " &
                                 "(CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) > " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ")) " &
                         "ORDER BY BeginDate DESC"
                Else
                    strSQL = "@SELECT# IDCause, ProgrammedAbsences.IDEmployee, BeginDate, FinishDate, MaxLastingDays, ProgrammedAbsences.Description, RelapsedDate, " &
                                    "NifTable.Value AS NIF, " &
                                    "IdTable.Value AS IdImport, " &
                                    "Causes.Name, " &
                                    "CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate, " &
                                    "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                    "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedAbsences.AbsenceID  AND adf.forecasttype='days') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, AbsenceID, " &
                                    "Timestamp, " &
                                    "'Day' AS Type,  " &
                                    "'CRU' AS Action  " &
                             "FROM ProgrammedAbsences " &
                                        "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                                        "LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = programmedabsences.IDEmployee AND NifTable.Date < GETDATE() " &
                                        "LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = programmedabsences.IDEmployee AND IdTable.Date < GETDATE()" &
                         "WHERE " &
                            "((BeginDate BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                            "((CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                            "(BeginDate < " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " &
                            "(CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) > " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ")) " &
                            " 	   AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) " &
                            "      AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)" &
                         "ORDER BY BeginDate DESC"
                End If

                oRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsences")
            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedAbsencesByTimeStamp(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _State As roProgrammedAbsenceState) As DataTable
            ' Obtiene las ausencias que hay en el periodo
            Dim oRet As DataTable = Nothing

            Try

                Dim strUniqueidentifierField As String = String.Empty
                If _IDEmployee = -1 Then
                    strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                End If

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, ProgrammedAbsences.IDEmployee, " &
                                "Convert(varchar, NifTable.Value) As NIF, " &
                                "Convert(varchar, IdTable.Value) As IdImport, " &
                                "BeginDate, FinishDate, MaxLastingDays, Convert(varchar, ProgrammedAbsences.Description) As Description, RelapsedDate, " &
                                "Causes.Name, " &
                                "CASE WHEN FinishDate Is NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedAbsences.AbsenceID  And adf.forecasttype='days') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, AbsenceID, " &
                                "Timestamp, " &
                                "'Day' AS Type,  " &
                                "'CRU' AS Action  " &
                             " FROM ProgrammedAbsences " &
                                "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                                "LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = programmedabsences.IDEmployee AND NifTable.Date < GETDATE() " &
                                "Left JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = programmedabsences.IDEmployee AND IdTable.Date < GETDATE() "
                If _IDEmployee > 0 Then
                    strSQL = strSQL & " WHERE ProgrammedAbsences.idEmployee = " & _IDEmployee & " AND "
                Else
                    strSQL = strSQL & " WHERE "
                End If
                strSQL = strSQL & " Timestamp BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime &
                                " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) " &
                                " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1) " &
                            " UNION " &
                             "@SELECT# NULL as idcause, " &
                             "DeletedProgrammedAbsences.IDEmployee,  " &
                             "Convert(varchar, NifTable.Value) As NIF, " &
                             "Convert(varchar, IdTable.Value) As IdImport, " &
                             "NULL AS begindate, " &
                             "NULL As finishdate, " &
                             "NULL AS maxlastingdays, " &
                             "NULL AS description, " &
                             "NULL AS relapseddate, " &
                             "NULL AS NAME, " &
                             "NULL AS RealFinishDate, " &
                             "NULL AS HasDocuments, " &
                             "NULL As DocumentsDelivered, " &
                             "AbsenceID, " &
                             "Timestamp, " &
                             "'Day' AS Type,  " &
                             "'D' AS Action  " &
                             "FROM DeletedProgrammedAbsences " &
                             "LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = DeletedProgrammedAbsences.IDEmployee AND NifTable.Date < GETDATE() " &
                             "Left JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = DeletedProgrammedAbsences.IDEmployee AND IdTable.Date < GETDATE() "
                If _IDEmployee > 0 Then
                    strSQL = strSQL & " WHERE DeletedProgrammedAbsences.IdEmployee = " & _IDEmployee & " AND "
                Else
                    strSQL = strSQL & " WHERE "
                End If
                strSQL = strSQL & " Timestamp BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime &
                            " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) " &
                            " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)" &
                         "ORDER BY BeginDate DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsencesByTimeStamp")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsencesByTimeStamp")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedAbsence(ByVal _IDEmployee As Integer, ByVal _BeginDate As DateTime, ByVal _State As roProgrammedAbsenceState, Optional ByVal bolAudit As Boolean = True) As roProgrammedAbsence

            Dim oRet As roProgrammedAbsence = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, IDEmployee, BeginDate " &
                         "FROM ProgrammedAbsences " &
                         "WHERE IDEmployee = " & _IDEmployee & " AND " &
                               "BeginDate = " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime()
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    oRet = New roProgrammedAbsence(tb.Rows(0).Item("IDEmployee"), tb.Rows(0).Item("BeginDate"), _State)
                    oRet.Load(bolAudit)

                End If

                ''If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                ''    ' Auditamos consulta masiva
                ''    Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                ''    Dim strAuditName As String = ""
                ''    For Each oRow As DataRow In oRet.Rows
                ''        strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                ''    Next
                ''    Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                ''    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                ''End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsence")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedAbsence")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function ValidateProgrammedAbsence(ByVal oProgrammedAbsence As roProgrammedAbsence, ByVal _State As roProgrammedAbsenceState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim queryDateStart As String = roTypes.Any2Time(oProgrammedAbsence.BeginDate.Value).SQLSmallDateTime()
                Dim queryDateEnd As String = roTypes.Any2Time(oProgrammedAbsence.RealFinishDate).SQLSmallDateTime()

                Dim strSQL As String

                If oProgrammedAbsence.FinishDate.HasValue AndAlso oProgrammedAbsence.BeginDate.Value > oProgrammedAbsence.FinishDate.Value Then
                    _State.Result = ProgrammedAbsencesResultEnum.InvalidDateInterval
                ElseIf Not oProgrammedAbsence.FinishDate.HasValue AndAlso oProgrammedAbsence.MaxLastingDays <= 0 Then
                    _State.Result = ProgrammedAbsencesResultEnum.InvalidDateInterval
                Else
                    strSQL = "@SELECT# * from ProgrammedAbsences " &
                             "WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND " &
                                   "(" &
                                    "(BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & ") " &
                                   "OR " &
                                    "(BeginDate <= " & queryDateEnd & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") " &
                                   ")"

                    If oProgrammedAbsence.IDEmployeeOriginal.HasValue Then
                        strSQL &= " AND CONVERT(varchar, IDEmployee) + '*' + CONVERT(varchar, BeginDate, 103) <> '" & oProgrammedAbsence.IDEmployeeOriginal.Value & "*" & Format(oProgrammedAbsence.BeginDateOriginal.Value, "dd/MM/yyyy") & "'"
                    End If

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.AnotherExistInDateInterval
                        Else
                            bolRet = True
                        End If

                    End If

                End If

                If bolRet Then

                    strSQL = "@SELECT# * from ProgrammedAbsences WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND "

                    strSQL &= " ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) "

                    If oProgrammedAbsence.IDEmployeeOriginal.HasValue Then
                        strSQL &= " AND CONVERT(varchar, IDEmployee) + '*' + CONVERT(varchar, BeginDate, 103) <> '" & oProgrammedAbsence.IDEmployeeOriginal.Value & "*" & Format(oProgrammedAbsence.BeginDateOriginal.Value, "dd/MM/yyyy") & "'"
                    End If

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.AnotherExistInDateInterval
                            bolRet = False
                        Else
                            bolRet = True
                        End If

                    End If

                End If

                'Comprovem si la ausencia es troba en periode de congelació
                If bolRet Then
                    Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedAbsence.IDEmployee.Value, False, _State)
                    Dim _IDCauseOld As Integer = oProgrammedAbsence.IDCause

                    'Si existeix la ausencia i es una modificació
                    If oProgrammedAbsence.BeginDateOriginal.HasValue Then
                        'Recuperem la justificacio antiga per fer comprovacions
                        Dim tb As New DataTable("ProgrammedAbsences")
                        strSQL = "@SELECT# * FROM ProgrammedAbsences WHERE "
                        strSQL &= "ProgrammedAbsences.IDEmployee = "
                        If oProgrammedAbsence.IDEmployeeOriginal.HasValue Then
                            strSQL &= oProgrammedAbsence.IDEmployeeOriginal.Value & " AND "
                        Else
                            strSQL &= oProgrammedAbsence.IDEmployee.Value & " AND "
                        End If
                        strSQL &= "ProgrammedAbsences.BeginDate = "
                        If oProgrammedAbsence.BeginDateOriginal.HasValue Then
                            strSQL &= roTypes.Any2Time(oProgrammedAbsence.BeginDateOriginal.Value).SQLSmallDateTime
                        Else
                            strSQL &= roTypes.Any2Time(oProgrammedAbsence.BeginDate.Value).SQLSmallDateTime
                        End If

                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        If tb.Rows.Count > 0 Then
                            _IDCauseOld = tb.Rows(0)("IDCause")
                        End If

                        If oProgrammedAbsence.BeginDateOriginal = oProgrammedAbsence.BeginDate And
                        oProgrammedAbsence.RealFinishDate = oProgrammedAbsence.RealOriginalFinishDate Then
                            'Si les dates coincideixen, i es troba en periode de congelacio, comprovem que no
                            's'hagi modificat la justificació
                            If oProgrammedAbsence.BeginDate.Value <= freezeDate Then
                                If oProgrammedAbsence.IDCause <> _IDCauseOld Then
                                    _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                                    bolRet = False
                                End If

                            End If
                        Else
                            If oProgrammedAbsence.BeginDateOriginal <> oProgrammedAbsence.BeginDate And
                                oProgrammedAbsence.BeginDate.Value <= freezeDate Then
                                _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                                bolRet = False
                            ElseIf oProgrammedAbsence.RealOriginalFinishDate <> oProgrammedAbsence.RealFinishDate And
                                oProgrammedAbsence.RealFinishDate <= freezeDate Then
                                _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                                bolRet = False
                            ElseIf oProgrammedAbsence.BeginDate.Value <= freezeDate And oProgrammedAbsence.IDCause <> _IDCauseOld Then
                                _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                                bolRet = False
                            End If
                        End If

                        'If bolRet Then
                        '    ' Comprobamos datos de seguimiento en el caso que sea una modificación
                        '    Dim oLicense As New roServerLicense
                        '    If oLicense.FeatureIsInstalled("Feature\Absences") Then
                        '        Dim strSQLAux As String = String.Empty
                        '        Dim idEmployee As Integer = 0

                        '        If oProgrammedAbsence.IDEmployeeOriginal.HasValue Then
                        '            idEmployee = oProgrammedAbsence.IDEmployeeOriginal
                        '        Else
                        '            idEmployee = oProgrammedAbsence.IDEmployee
                        '        End If

                        '        strSQLAux = "@SELECT# COUNT(*) FROM AbsenceTracking WHERE TypeAbsence = 0 AND IDEmployee=" & idEmployee.ToString & " AND IDCause=" & _IDCauseOld & " AND Date=" & roTypes.Any2Time(oProgrammedAbsence.BeginDateOriginal.Value).SQLSmallDateTime

                        '        Dim intCountTrackDays As Integer = roTypes.Any2Integer(ExecuteScalar(strSQLAux, oBaseConnection))
                        '        If intCountTrackDays > 0 Then
                        '            If oProgrammedAbsence.IDCause <> _IDCauseOld Then
                        '                ' Si se cambia la justificacion y tiene registros de seguimiento no se deja cambiar
                        '                _State.Result = ProgrammedAbsencesResultEnum.ExistTrackingDays
                        '                bolRet = False
                        '            End If

                        '            If bolRet Then
                        '                If oProgrammedAbsence.BeginDateOriginal <> oProgrammedAbsence.BeginDate Then
                        '                    ' Si se cambia la fecha de inicio y tiene registros de seguimiento no se deja cambiar
                        '                    _State.Result = ProgrammedAbsencesResultEnum.ExistTrackingDays
                        '                    bolRet = False
                        '                End If
                        '            End If

                        '            If bolRet Then
                        '                If oProgrammedAbsence.RealOriginalFinishDate <> oProgrammedAbsence.RealFinishDate Then
                        '                    ' Si se cambia la fecha de finalización
                        '                    ' y Si existen registros de seguimiento ya entregados >= a la nueva fecha de finalización no se deja cambiar
                        '                    Dim intCountTrackDaysWithDeliveredDays As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM AbsenceTracking WHERE TypeAbsence = 0 AND IDEmployee=" & oProgrammedAbsence.IDEmployeeOriginal & " AND IDCause=" & _IDCauseOld & " AND Date=" & roTypes.Any2Time(oProgrammedAbsence.BeginDateOriginal).SQLSmallDateTime & " AND DeliveryDate IS not null AND TrackDay >=" & roTypes.Any2Time(oProgrammedAbsence.RealFinishDate).SQLSmallDateTime, oBaseConnection))
                        '                    If intCountTrackDaysWithDeliveredDays > 0 Then
                        '                        _State.Result = ProgrammedAbsencesResultEnum.ExistTrackingDays
                        '                        bolRet = False
                        '                    End If
                        '                End If
                        '            End If
                        '        End If
                        '    End If
                        'End If
                    Else
                        'Si es una nova ausencia
                        If oProgrammedAbsence.BeginDate.Value <= freezeDate Then
                            _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                            bolRet = False
                        End If

                        If oProgrammedAbsence.RealFinishDate <= freezeDate Then
                            _State.Result = ProgrammedAbsencesResultEnum.InFreezeDate
                            bolRet = False
                        End If
                    End If
                End If

                'Comprovem si la ausencia es troba dins el periode de contracte.
                ' por indicaciones de consultoria, solo se debe tener en cuenta que el inicio este dentro de un contrato y que el final tambien , pero no
                ' necesariamente en el mismo contrato
                If bolRet Then
                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE " &
                             "BeginDate <= " & roTypes.Any2Time(oProgrammedAbsence.BeginDate.Value).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(oProgrammedAbsence.BeginDate.Value).SQLSmallDateTime() & " AND " &
                             "IDEmployee = " & oProgrammedAbsence.IDEmployee
                    Dim dTblC As DataTable = CreateDataTable(strSQL)
                    If dTblC Is Nothing OrElse dTblC.Rows.Count = 0 Then
                        _State.Result = ProgrammedAbsencesResultEnum.DateOutOfContract
                        bolRet = False
                    End If

                    If bolRet Then
                        strSQL = "@SELECT# * FROM EmployeeContracts WHERE " &
                             "BeginDate <= " & roTypes.Any2Time(oProgrammedAbsence.RealFinishDate).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(oProgrammedAbsence.RealFinishDate).SQLSmallDateTime() & " AND " &
                             "IDEmployee = " & oProgrammedAbsence.IDEmployee
                        dTblC = CreateDataTable(strSQL)
                        If dTblC Is Nothing OrElse dTblC.Rows.Count = 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.DateOutOfContract
                            bolRet = False
                        End If
                    End If
                End If

                ' Comprobamos que no existan fichajes en la fecha de inicio de la ausencia
                ' Se deshabilita esta validacion por indicaciones de consultoria

                If bolRet Then
                    ' Comprobamos que no existan incidencias previstas en ese periodo
                    strSQL = "@SELECT# * from ProgrammedCauses " &
                             "WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND "

                    strSQL &= " ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) "

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.AnotherExistInDateInterval
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If

                End If

                If bolRet Then
                    ' Verificamos si existe una prevision de vacaciones por horas en el mismo periodo
                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                 "WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND "
                    strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.AnotherHolidayExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If

                End If

                If bolRet Then
                    ' Comprobamos que no existan previsiones de exceso  en ese periodo
                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                             "WHERE IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND "
                    strSQL &= "  (  (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (EndDate >= " & queryDateStart & " AND EndDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDateStart & " AND EndDate >= " & queryDateEnd & ") )  "
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedAbsencesResultEnum.AnotherOvertimeExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If

                End If

                If bolRet Then
                    Dim oAdvParamAPV As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                    Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("VTLive.Absences.ValidateHolidayOnDate", New AdvancedParameter.roAdvancedParameterState())
                    If roTypes.Any2String(oAdvParamAPV.Value).ToUpper <> "VPA" And roTypes.Any2Boolean(oAdvParam.Value) Then
                        ' Verificamos que la previsión no se solape con un horario de vacaciones planificado
                        strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & oProgrammedAbsence.IDEmployee.Value & " AND "
                        strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND ( isnull(IsHolidays,0) = 1 ) "
                        Dim tb As DataTable = CreateDataTable(strSQL)
                        If tb IsNot Nothing Then
                            If tb.Rows.Count > 0 Then
                                _State.Result = ProgrammedAbsencesResultEnum.AnotherHolidayExistInDate
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::ValidateProgrammedAbsence")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::ValidateProgrammedAbsence")
            End Try

            Return bolRet

        End Function

        'Private Shared Function GetFirstDate() As Date
        '    Dim xRet As Date = New Date(1900, 1, 1)
        '    Dim oParameters As New roParameters("OPTIONS")
        '    Dim oParam As Object = oParameters.Parameter(roParameters.Parameters.FirstDate)

        '    If oParam IsNot Nothing AndAlso IsDate(oParam) Then
        '        xRet = CDate(oParam)
        '    End If

        '    Return xRet
        'End Function

        Public Shared Function GetProgrammedCauses(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _State As roProgrammedAbsenceState) As DataTable
            ' Obtiene las ausencias que hay en el periodo
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                'strSQL = "@SELECT# * FROM ProgrammedCauses " & _
                '           "WHERE IDEmployee = " & _IDEmployee & " AND Date>=" & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & _
                '                 "Date<=" & roTypes.Any2Time(_EndDate).SQLSmallDateTime

                strSQL = "@SELECT# ProgrammedCauses.*, Causes.Name as Name FROM ProgrammedCauses LEFT JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                           "WHERE IDEmployee = " & _IDEmployee

                oRet = CreateDataTable(strSQL, )

                ''If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                ''    ' Auditamos consulta masiva
                ''    Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                ''    Dim strAuditName As String = ""
                ''    For Each oRow As DataRow In oRet.Rows
                ''        strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                ''    Next
                ''    Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                ''    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                ''End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedCauses")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedAbsence::GetProgrammedCauses")
            Finally

            End Try

            Return oRet

        End Function

        Public Overrides Function ToString() As String
            Dim oRet As String = String.Empty
            Try
                oRet = "Ausencia por días del empleado " & Employee.roEmployee.GetEmployee(Me.IDEmployee, Nothing).Name & " desde el día " & Me.BeginDate.Value.ToShortDateString & " hasta el día " & Me.RealFinishDate.ToShortDateString & " por " & Cause.roCause.GetCauseNameByID(Me.IDCause)
            Catch ex As Exception
                oRet = "Error recuperando descripción de ausencia por horas"
            End Try
            Return oRet
        End Function

        Private Function RecalculateStartUpValues_UPF(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _FreezeDate As Date) As Boolean

            Dim oRet As Boolean = False

            Try
                Dim _BeginDateUPF As Date = _BeginDate
                Dim _FinishDateUPF As Date = _BeginDate
                Dim bolApplyUPF As Boolean = True
                ' Si la fecha de inicia de la PA, es del año actual , marcamos solo el 1 de el año actual
                ' Si la fecha es del año pasado , marcamos el 1 de enero del año pasado y el actual
                If _BeginDateUPF.Year = Now.Date.Year Then
                    _BeginDateUPF = New Date(Now.Year, 1, 1)
                    _FinishDateUPF = _BeginDateUPF
                ElseIf _BeginDateUPF.Year < Now.Date.Year Then
                    _BeginDateUPF = New Date(_BeginDateUPF.Year, 1, 1)
                    _FinishDateUPF = New Date(_BeginDateUPF.Year + 1, 1, 1)
                Else
                    bolApplyUPF = False
                End If

                If bolApplyUPF Then
                    Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date = " & roTypes.Any2Time(_BeginDateUPF).SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(_FinishDateUPF).SQLSmallDateTime & ")" & " AND " &
                                     " Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime
                    ExecuteSqlWithoutTimeOut(strSQL)
                End If

                oRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence::RecalculateStartUpValues_UPF")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedAbsence::RecalculateStartUpValues_UPF")
            End Try

            Return oRet

        End Function

        Public Overrides Function SurfaceCopy() As Object
            Return Me.MemberwiseClone
        End Function

#End Region

#End Region

    End Class

End Namespace