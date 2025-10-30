Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports DocumentFormat.OpenXml.Office2010.CustomUI
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace BusinessCenter

    <DataContract()>
    Public Class roBusinessCenter

#Region "Declarations - Constructors"

        Private oState As roBusinessCenterState

        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private strField1 As String
        Private strField2 As String
        Private strField3 As String
        Private strField4 As String
        Private strField5 As String
        Private intStatus As Integer
        Private intAuthorizationMode As Integer
        Private oZones As Generic.List(Of roBusinessCenterZone)

        Public Sub New()

            Me.oState = New roBusinessCenterState
            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roBusinessCenterState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID

            Me.Load(_Audit)

        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Zones() As Generic.List(Of roBusinessCenterZone)
            Get
                Return Me.oZones
            End Get
            Set(ByVal value As Generic.List(Of roBusinessCenterZone))
                Me.oZones = value
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember()>
        Public Property State() As roBusinessCenterState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roBusinessCenterState)
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
        Public Property Status() As Integer
            Get
                Return Me.intStatus
            End Get
            Set(ByVal value As Integer)
                Me.intStatus = value
            End Set
        End Property
        <DataMember()>
        Public Property AuthorizationMode() As Integer
            Get
                Return Me.intAuthorizationMode
            End Get
            Set(ByVal value As Integer)
                Me.intAuthorizationMode = value
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
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property
        <DataMember()>
        Public Property Field1() As String
            Get
                Return Me.strField1
            End Get
            Set(ByVal value As String)
                Me.strField1 = value
            End Set
        End Property
        <DataMember()>
        Public Property Field2() As String
            Get
                Return Me.strField2
            End Get
            Set(ByVal value As String)
                Me.strField2 = value
            End Set
        End Property
        <DataMember()>
        Public Property Field3() As String
            Get
                Return Me.strField3
            End Get
            Set(ByVal value As String)
                Me.strField3 = value
            End Set
        End Property
        <DataMember()>
        Public Property Field4() As String
            Get
                Return Me.strField4
            End Get
            Set(ByVal value As String)
                Me.strField4 = value
            End Set
        End Property
        <DataMember()>
        Public Property Field5() As String
            Get
                Return Me.strField5
            End Get
            Set(ByVal value As String)
                Me.strField5 = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM BusinessCenters WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.strField1 = Any2String(oRow("Field1"))
                    Me.strField2 = Any2String(oRow("Field2"))
                    Me.strField3 = Any2String(oRow("Field3"))
                    Me.strField4 = Any2String(oRow("Field4"))
                    Me.strField5 = Any2String(oRow("Field5"))

                    Me.intStatus = IIf(IsDBNull(oRow("Status")), 1, oRow("Status"))

                    Me.intAuthorizationMode = IIf(IsDBNull(oRow("AuthorizationMode")), 0, oRow("AuthorizationMode"))

                    Dim oBusinessCenterZoneState As New roBusinessCenterZoneState
                    Me.oZones = roBusinessCenterZone.GetZonesByBusinessCenter(oRow("ID"), oBusinessCenterZoneState)

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAssignment, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBusinessCenter::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBusinessCenter::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function GetBusinessCenterByName(ByVal BusinessCenterName As String, Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# id FROM BusinessCenters WHERE Name = '" & BusinessCenterName & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Me.ID = Any2String(oRow("ID"))
                    bolRet = Me.Load()

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAssignment, Me.strName, tbParameters, -1)
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByName")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByName")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = BusinessCenterResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("BusinessCenter")
                    Dim strSQL As String = "@SELECT# * FROM BusinessCenters WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        Me.ID = oRow("ID")
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("Field1") = Me.strField1
                    oRow("Field2") = Me.strField2
                    oRow("Field3") = Me.strField3
                    oRow("Field4") = Me.strField4
                    oRow("Field5") = Me.strField5
                    oRow("Status") = Me.intStatus
                    oRow("AuthorizationMode") = Me.intAuthorizationMode

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    'Borramos zonas asignadas
                    Dim DeleteQuerys() As String = {"@DELETE# FROM BusinessCenterZones WHERE IDCenter = " & Me.intID.ToString}
                    For Each strSQLDelete As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQLDelete)
                        If Not bolRet Then Exit For
                    Next

                    'Asignamos los empleados y grupos en caso necesario
                    If bolRet And Me.intAuthorizationMode = 1 Then
                        If Me.oZones IsNot Nothing Then
                            If Me.oZones.Count > 0 Then
                                For Each oZon As roBusinessCenterZone In oZones
                                    bolRet = ExecuteSql("@INSERT# INTO BusinessCenterZones (IDZone, IDCenter) VALUES(" & oZon.ID.ToString & "," & Me.intID.ToString & ")")
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If
                    End If

                    bolRet = True

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAssignment, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::Save")
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
                    oState.Result = DTOs.BusinessCenterResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                If bolRet And bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM BusinessCenters WHERE Name = @AssignmentName AND ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@AssignmentName", DbType.String, 64)
                    cmd.Parameters("@AssignmentName").Value = Me.Name
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.NameAlreadyExist
                        bolRet = False
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::Validate")
            End Try

            Return bolRet

        End Function

        Public Function DeleteAllEmployeesAssigned() As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM EmployeeCenters WHERE IDCenter = " & Me.ID.ToString
                ExecuteSql(strSQL)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::DeleteAllEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::DeleteAllEmployees")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nuevo puesto
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM BusinessCenters"
            Dim tb As DataTable = CreateDataTable(strSQL)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        ''' <summary>
        ''' Borra el puesto siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                bolRet = True

                Dim strSQL As String
                Dim tb As DataTable
                Dim cmd As DbCommand
                Dim da As DbDataAdapter

                ' Verificamos que no este asignado a ningun grupo de usuarios no sea el de Robotics
                tb = New DataTable()
                strSQL = "@SELECT# * FROM sysroPassports_Centers WHERE IDCenter = @IDCenter AND IDPassport NOT IN(@SELECT# ID FROM sysroPassports WHERE Description like '%@@ROBOTICS@@%')"
                cmd = CreateCommand(strSQL)
                AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                cmd.Parameters("@IDCenter").Value = Me.ID
                da = CreateDataAdapter(cmd, True)
                tb.Rows.Clear()
                da.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oState.Result = DTOs.BusinessCenterResultEnum.UsedOnPassports
                    bolRet = False
                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Tasks WHERE IDCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnTasks
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Groups WHERE IDCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnGroups
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Punches WHERE Type = 13 and TypeData= @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnPunches
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM DailyCauses WHERE IDCenter= @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnDailyCauses
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM EmployeeCenters WHERE IDCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnEmployees
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Shifts WHERE IDCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnShifts
                        bolRet = False
                    End If

                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM ProductiveUnits WHERE IDCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnProductiveUnit
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM TerminalReaders WHERE IDCostCenter = @IDCenter "
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDCenter", DbType.Int32, 0)
                    cmd.Parameters("@IDCenter").Value = Me.ID
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.BusinessCenterResultEnum.UsedOnTerminals
                        bolRet = False
                    End If
                End If


                If bolRet Then
                    'Borramos el centro
                    Dim DelQuerys() As String = {"@DELETE# FROM sysroPassports_Centers WHERE IDCenter = " & Me.ID.ToString & " AND IDPassport IN(@SELECT# ID FROM sysroPassports WHERE Description like '%@@ROBOTICS@@%')",
                                                "@DELETE# FROM BusinessCenterZones WHERE IDCenter = " & Me.ID.ToString,
                                                "@DELETE# FROM BusinessCenters WHERE ID = " & Me.ID.ToString}

                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = DTOs.BusinessCenterResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = DTOs.BusinessCenterResultEnum.NoError)
                End If

                If bolRet And bAudit Then
                    '' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAssignment, Me.strName, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetBusinessCenters(ByVal oState As roBusinessCenterState, Optional bolCheckStatus As Boolean = False) As System.Data.DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM BusinessCenters "

                If bolCheckStatus Then
                    strSQL = strSQL & " WHERE Status=1 "
                End If
                strSQL = strSQL & " ORDER BY Name"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function GetBusinessCentersByFilter(oState As roBusinessCenterState, strFilter As String) As System.Data.DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSql As String = "@SELECT# * FROM BusinessCenters "
                strSql = strSql & " WHERE " & strFilter
                strSql = strSql & " ORDER BY Name"

                tbRet = CreateDataTable(strSql, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function SaveEmployeeCenters(ByVal intIDEmployee As Integer, ByVal dsCenters As DataSet, ByRef oState As roBusinessCenterState, Optional ByVal bolAudit As Boolean = False, Optional ByVal bolCallBroadcaster As Boolean = True, Optional bolDeletePreviousCenters As Boolean = True) As Boolean

            oState.UpdateStateInfo()

            Dim bolRet As Boolean = True
            Dim bolBroadcast As Boolean = False
            Dim strSQL As String

            Dim idCenter As New Generic.List(Of Integer)
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If bolRet Then
                    ' Borrar los centros de coste actuales
                    If bolDeletePreviousCenters Then
                        strSQL = "@DELETE# FROM EmployeeCenters " &
                                 "WHERE IDEmployee = " & intIDEmployee.ToString
                        If Not ExecuteSql(strSQL) Then
                            oState.Result = DTOs.BusinessCenterResultEnum.ConnectionError
                        End If
                    End If

                    If oState.Result = DTOs.BusinessCenterResultEnum.NoError Then
                        If dsCenters IsNot Nothing AndAlso dsCenters.Tables.Count > 0 Then

                            For Each oRow As DataRow In dsCenters.Tables(0).Rows
                                idCenter.Add(roTypes.Any2Integer(oRow("IDCenter")))
                                strSQL = "@INSERT# INTO EmployeeCenters(IDEmployee, IDCenter, BeginDate, EndDate) " &
                                         "VALUES(" & intIDEmployee.ToString & ", " & oRow("IDCenter") & "," & Any2Time(oRow("BeginDate")).SQLSmallDateTime & "," & Any2Time(oRow("EndDate")).SQLSmallDateTime & ")"
                                If Not ExecuteSql(strSQL) Then
                                    oState.Result = DTOs.BusinessCenterResultEnum.ConnectionError
                                    Exit For
                                End If
                            Next
                        Else
                            oState.Result = DTOs.BusinessCenterResultEnum.Exception
                        End If

                    End If
                End If

                ' Si se hizo algún cambio llamo a Broadcaster
                If oState.Result = DTOs.BusinessCenterResultEnum.NoError And bolCallBroadcaster Then
                    'TODO: Se debería controlar si se han hecho cambios o no
                    If bolCallBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
                End If

                If oState.Result = DTOs.BusinessCenterResultEnum.NoError AndAlso bolAudit Then
                    ' Insertar registro auditoria
                    Dim oStateEmployee As New Employee.roEmployeeState
                    oStateEmployee.IDPassport = oState.IDPassport

                    Dim oEmpName As String = roBusinessSupport.GetEmployeeName(intIDEmployee, oStateEmployee)
                    Dim CentersNames As String = ""
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmpName, "", 1)
                    If idCenter.Count > 0 Then
                        For Each iCenterID As Integer In idCenter
                            Dim oCenter As New roBusinessCenter(iCenterID, oState)
                            CentersNames = CentersNames & oCenter.Name & ","
                        Next
                        If CentersNames.Length > 1 Then CentersNames = CentersNames.Substring(0, CentersNames.Length - 1)
                    End If
                    oState.AddAuditParameter(tbParameters, "{Centers}", CentersNames, "", 1)

                    oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tEmployeeCenters, oEmpName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::SaveEmployeeCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::SaveEmployeeCenters")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = DTOs.BusinessCenterResultEnum.NoError)
            End Try

            Return (oState.Result = DTOs.BusinessCenterResultEnum.NoError)

        End Function

        Public Shared Function GetAvailableBusinessCentersDataTable(ByVal oState As roBusinessCenterState, ByVal intIDEmployee As Integer, Optional ByVal xDate As Date = Nothing, Optional ByVal bolCheckPassportPermission As Boolean = False) As System.Data.DataTable
            Dim oTable As DataTable = Nothing
            Try
                ' Si no tiene centros de coste asignados, no puede fichar aunque su grupo tenga uno por defecto ...
                If BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oState, intIDEmployee, False, True,, xDate, bolCheckPassportPermission).Rows.Count > 0 Then
                    ' Si tiene centros de coste asignados, puede fichar por estos o por el asignado a por el que su grupo tenga por defecto ...
                    oTable = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(oState, intIDEmployee, True, True,, xDate, bolCheckPassportPermission)
                End If
            Catch ex As Exception
                Return Nothing
            End Try

            Return oTable
        End Function

        Public Shared Function GetEmployeeWorkingCostCenter(ByVal oState As roBusinessCenterState, ByVal intIDEmployee As Integer, ByRef idCostCenter As Integer, Optional ByRef _BusinessCenter As roBusinessCenter = Nothing) As String
            Dim strWorkingCostCenter As String = String.Empty

            Try

                Dim oParameter As New AdvancedParameter.roAdvancedParameter("CostControlMode", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport))
                Dim mode As Integer = -1

                If oParameter.Value <> "" Then mode = roTypes.Any2Integer(oParameter.Value)
                Dim oLastPunch As DataTable = Nothing

                If mode <= 0 Then
                    oLastPunch = Punch.roPunch.GetLastPunchDataTable(New Punch.roPunchState(-1), DateTime.Now.Date, DateTime.Now.Date, , DateTime.Now, intIDEmployee, , , , , , PunchTypeEnum._CENTER)
                Else
                    oLastPunch = Punch.roPunch.GetLastPunchDataTable(New Punch.roPunchState(-1), , DateTime.Now.Date, , DateTime.Now, intIDEmployee, , , , , , PunchTypeEnum._CENTER)
                End If

                Dim iCenter As Integer = -1

                If oLastPunch IsNot Nothing AndAlso oLastPunch.Rows.Count > 0 Then
                    iCenter = oLastPunch.Rows(0)("TypeData")
                Else
                    iCenter = GetEmployeeDefaultBusinessCenter(oState, intIDEmployee, Now.Date)
                End If

                If iCenter > 0 Then
                    idCostCenter = iCenter
                    Dim strSQL As String = "@SELECT# Name FROM BusinessCenters WHERE ID = " & iCenter
                    strWorkingCostCenter = roTypes.Any2String(ExecuteScalar(strSQL))
                    If _BusinessCenter IsNot Nothing Then
                        _BusinessCenter = New roBusinessCenter(idCostCenter, oState, False)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenters")
            Finally

            End Try

            Return strWorkingCostCenter
        End Function

        Public Shared Function GetEmployeeBusinessCentersDataTable(ByVal oState As roBusinessCenterState, ByVal intIDEmployee As Integer, Optional bolDefaultCenter As Boolean = True, Optional ByVal bolOnlyActiveCenters As Boolean = False, Optional intIDZone As Integer = -1, Optional ByVal xDate As Date = Nothing, Optional ByVal bolCheckPassportPermission As Boolean = False) As System.Data.DataTable
            Dim tbRet As DataTable = Nothing

            Try

                If xDate = Nothing Then
                    xDate = Date.Now.Date
                End If

                Dim strSQL As String = "@SELECT# EmployeeCenters.*, BusinessCenters.Name FROM EmployeeCenters, BusinessCenters WHERE BusinessCenters.ID = EmployeeCenters.IDCenter AND EmployeeCenters.IDEmployee =" & intIDEmployee.ToString
                If bolOnlyActiveCenters Then
                    strSQL = strSQL & " AND BusinessCenters.Status = 1 AND EmployeeCenters.BeginDate <= " & Any2Time(xDate).SQLSmallDateTime & "  AND EmployeeCenters.EndDate >= " & Any2Time(xDate).SQLSmallDateTime
                End If

                If intIDZone > 0 Then
                    strSQL = strSQL & " AND (BusinessCenters.AuthorizationMode = 0 OR BusinessCenters.ID IN(@SELECT# distinct IDCenter FROM BusinessCenterZones WHERE BusinessCenterZones.IDZone =" & intIDZone.ToString & ") )"
                End If

                If bolCheckPassportPermission Then
                    If oState.IDPassport > 0 Then
                        strSQL = strSQL & " AND (BusinessCenters.ID IN(@SELECT# DISTINCT IDCostCenter FROM sysrovwSecurity_PermissionOverCostCenters c WITH (nolock) WHERE c.IDPassport =" & oState.IDPassport.ToString & "))"
                    End If
                End If

                strSQL = strSQL & " Order By BusinessCenters.Name"
                tbRet = CreateDataTable(strSQL, )

                If bolDefaultCenter Then
                    Dim intIDefaultCenter = GetEmployeeDefaultBusinessCenter(oState, intIDEmployee, xDate)

                    If intIDefaultCenter > 0 Then
                        Dim oCenter As New roBusinessCenter(intIDefaultCenter, oState)
                        Dim bolretValid As Boolean = True
                        ' Revisamos si el centro esta activo y si esta en la zona
                        strSQL = "@SELECT# count(BusinessCenters.ID) as total FROM BusinessCenters WHERE BusinessCenters.ID=" & oCenter.ID.ToString
                        If bolOnlyActiveCenters Then
                            strSQL = strSQL & " AND BusinessCenters.Status = 1 "
                        End If

                        If intIDZone > 0 Then
                            strSQL = strSQL & " AND (BusinessCenters.AuthorizationMode = 0 OR BusinessCenters.ID IN(@SELECT# distinct IDCenter FROM BusinessCenterZones WHERE BusinessCenterZones.IDZone =" & intIDZone.ToString & ") )"
                        End If

                        If bolOnlyActiveCenters Or intIDZone > 0 Then
                            If Any2Double(ExecuteScalar(strSQL, )) = 0 Then
                                bolretValid = False
                            End If
                        End If

                        If bolretValid Then
                            Dim oRow As DataRow
                            If tbRet Is Nothing Or tbRet.Rows.Count = 0 Then
                                oRow = tbRet.NewRow
                                oRow("IDEmployee") = intIDEmployee
                                oRow("IDCenter") = intIDefaultCenter
                                oRow("Name") = oCenter.Name
                                tbRet.Rows.Add(oRow)
                            Else
                                If tbRet.Select("IDCenter=" & intIDefaultCenter).Length = 0 Then
                                    oRow = tbRet.NewRow
                                    oRow("IDEmployee") = intIDEmployee
                                    oRow("IDCenter") = intIDefaultCenter
                                    oRow("Name") = oCenter.Name
                                    tbRet.Rows.Add(oRow)
                                End If
                            End If
                            tbRet.AcceptChanges()
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetEmployeeBusinessCentersDataTable")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetEmployeeBusinessCentersDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function GetBusinessCenterByPassportDataTable(ByVal oState As roBusinessCenterState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As System.Data.DataTable

            Dim tbRet As DataTable = Nothing

            Try
                Dim strSQL As String = "@SELECT# * FROM BusinessCenters WHERE ID IN (@SELECT# DISTINCT IDCostCenter FROM sysrovwSecurity_PermissionOverCostCenters c WITH (nolock) WHERE c.IDPassport =" & intIDPassport.ToString & ") "

                If bolCheckStatus Then
                    strSQL = strSQL & " AND Status=1"
                End If
                strSQL = strSQL & " Order by Name"
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassportDataTable")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassportDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetEmployeeDefaultBusinessCenter(ByVal oState As roBusinessCenterState, ByVal intIDEmployee As Integer, ByVal xDate As Date) As Integer
            Dim intIDCenter As Integer = 0

            Dim sSQL As String
            Dim IDCenter As Integer
            Dim IDGroup As Double
            Dim i As Integer

            Try

                intIDCenter = 0

                ' Obtenemos el grupo y el centro de coste al que pertenece en la fecha indicada el empleado
                sSQL = "@SELECT# isnull(IDCenter, 0) as DefaultCenter , Path, ID FROM Groups WHERE ID IN(@SELECT# IDGROUP FROM EmployeeGroups WHERE IDEmployee = " & intIDEmployee & " AND BeginDate <= " & Any2Time(xDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xDate).SQLSmallDateTime & ")"
                Dim tbCenters As DataTable = CreateDataTable(sSQL, )
                If tbCenters IsNot Nothing Then
                    If tbCenters.Rows.Count > 0 Then
                        IDCenter = Any2Double(tbCenters.Rows(0)("DefaultCenter"))

                        i = StringItemsCount(Any2String(tbCenters.Rows(0)("Path")), "\") - 1
                        ' Si el grupo actual no tiene centro asignado recorremos todos los grupos padres
                        ' hasta encontrar el primero que tenga asignado uno
                        If IDCenter = 0 And i > 0 Then
                            While i >= 0
                                IDGroup = Any2Double(String2Item(Any2String(tbCenters.Rows(0)("path")), i - 1, "\"))
                                IDCenter = Any2Integer(ExecuteScalar("@SELECT# isnull(IDCenter, 0) as DefaultCenter FROM Groups WHERE ID=" & IDGroup))
                                If IDCenter > 0 Then
                                    i = -1
                                End If
                                i = i - 1
                            End While
                        End If
                    End If
                    intIDCenter = IDCenter
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetEmployeeDefaultBusinessCenter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetEmployeeDefaultBusinessCenter")
            Finally

            End Try

            Return intIDCenter

        End Function

        Public Shared Function GetBusinessCenterByPassport(ByVal oState As roBusinessCenterState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As Integer()
            Dim tb As DataTable = Nothing
            Dim intRet() As Integer = {0}

            Try

                Dim strSQL As String = "@SELECT# ID FROM BusinessCenters WHERE ID IN (@SELECT# DISTINCT IDCostCenter FROM sysrovwSecurity_PermissionOverCostCenters c WITH (nolock) WHERE c.IDPassport =" & intIDPassport.ToString & ") "
                If bolCheckStatus Then
                    strSQL = strSQL & " AND Status=1"
                End If

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    ReDim intRet(tb.Rows.Count - 1)

                    Dim i As Integer = 0
                    For Each oRow As DataRow In tb.Rows
                        intRet(i) = oRow("ID")
                        i += 1
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassport")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterByPassport")
            End Try

            Return intRet

        End Function

        Public Shared Function SaveBusinessCenterByPassport(ByVal oState As roBusinessCenterState, ByVal intIDPassport As Integer, ByVal intBusinessCenter() As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos empleados y grupos asignados
                Dim DeleteQuerys() As String = {"@DELETE# FROM sysroPassports_Centers WHERE IDPassport = " & intIDPassport.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete)
                    If Not bolRet Then Exit For
                Next

                'Asignamos los nuevos centros de coste
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For i = 0 To intBusinessCenter.Length - 1
                        bolRet = ExecuteSql("@INSERT# INTO sysroPassports_Centers (IDPassport, IDCenter) VALUES(" & intIDPassport.ToString & "," & intBusinessCenter(i).ToString & ")")
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::SaveBusinessCenterByPassport")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::SaveBusinessCenterByPassport")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function
#End Region

#Region "Bussiness Centers Accrued Values Helper"

        Public Shared Function GetContractAnnualizedBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                         Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As Generic.List(Of DateTime) = Nothing
                lstDates = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, intIDEmployee, xDate, roContractState)

                Dim xBeginPeriod As DateTime = lstDates(0)
                xDate = lstDates(1)

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xBeginPeriod, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetAnualBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                         Optional idCause As Integer = 0, Optional ByVal Last As Boolean = False) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1
                If intYearIniMonth = 0 Then intYearIniMonth = 1

                Dim xBeginPeriod As Date
                If xDate.Month > intYearIniMonth Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                ElseIf xDate.Month = intYearIniMonth And xDate.Day >= intMonthIniDay Then
                    xBeginPeriod = New DateTime(xDate.Year, intYearIniMonth, intMonthIniDay)
                Else
                    xBeginPeriod = New DateTime(xDate.Year - 1, intYearIniMonth, intMonthIniDay)
                End If

                If (Last = True) Then
                    xBeginPeriod = xBeginPeriod.AddYears(-1)
                    xDate = xBeginPeriod.AddYears(1).AddDays(-1)
                End If

                Dim roContractState = New Contract.roContractState()

                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xBeginPeriod, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetAnualBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetMonthBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                         Optional idCause As Integer = 0, Optional ByVal Last As Boolean = False) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1

                Dim xBeginPeriod As DateTime
                If xDate.Day > intMonthIniDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    xBeginPeriod = New Date(xDate.Year, xDate.Month, intMonthIniDay)
                ElseIf xDate.Day < intMonthIniDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    xBeginPeriod = New Date(xDate.AddMonths(-1).Year, xDate.AddMonths(-1).Month, intMonthIniDay)
                Else
                    'Si es el mismo dia
                    xBeginPeriod = xDate
                End If

                If (Last = True) Then
                    xBeginPeriod = xBeginPeriod.AddMonths(-1)
                    xDate = xBeginPeriod.AddMonths(1).AddDays(-1)
                End If

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xBeginPeriod, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMonthBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetContractBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                            Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim xBeginPeriod As DateTime
                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                Else
                    xBeginPeriod = New DateTime(1900, 1, 1, 0, 0, 0)
                    xDate = New DateTime(1900, 1, 1, 0, 0, 0)
                End If

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xBeginPeriod, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetContractBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetContractBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetWeekBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                        Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intWeekIniDay As Integer = oParams.Parameter(Parameters.WeekPeriod)
                If intWeekIniDay = 0 Then intWeekIniDay = 1

                Dim xBeginPeriod As DateTime
                Dim iDayOfWeek As Integer = xDate.DayOfWeek
                If iDayOfWeek = 0 Then iDayOfWeek = 7
                If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7
                xBeginPeriod = xDate.AddDays(intWeekIniDay - iDayOfWeek)

                Dim roContractState = New Contract.roContractState()
                roBusinessState.CopyTo(oState, roContractState)
                Dim lstDates As List(Of Date) = roBusinessSupport.GetDatesOfContractInDate(intIDEmployee, xDate, roContractState)
                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                    If xDate > lstDates(1) Then xDate = lstDates(1)
                End If

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xBeginPeriod, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetWeekBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Public Shared Function GetDailyBussinessCenters(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState,
                                                         Optional idCause As Integer = 0) As DataTable
            Dim tb As DataTable = Nothing

            Try
                Dim myDate = New DateTime(xDate.Year, xDate.Month, xDate.Day, 0, 0, 0, 0)

                Dim strSQL As String = GetBusinessCenterSummaryData(intIDEmployee, idCause, xDate)

                tb = CreateDataTable(strSQL, )
                For Each oRow As DataRow In tb.Rows
                    If Any2Boolean(oRow("DayType")) Or Any2Boolean(oRow("CustomType")) Then
                        ' Si la justificacion es de dia o personalizada
                        oRow("TotalFormat") = Format(CDbl(oRow("Total")), "##0.000")
                    Else
                        ' Si la justificacion es de tiempo
                        Try
                            oRow("TotalFormat") = roConversions.ConvertHoursToTime(CDbl(oRow("Total")))
                        Catch ex As Exception
                            oRow("TotalFormat") = "00:00"
                        End Try
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyBussinessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetDailyBussinessCenters")
            Finally

            End Try

            Return tb
        End Function

        Private Shared Function GetBusinessCenterSummaryData(intIDEmployee As Integer, idCause As Integer, xBeginPeriod As Date, Optional xDate As Date = Nothing) As String
            Dim strSQL As String = "@SELECT#     dbo.Causes.Name AS CauseName, dbo.Causes.WorkingType, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,
   				                            isnull(dbo.BusinessCenters.Name, '') AS CenterName, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,
				                            sum(dbo.DailyCauses.Value) AS Total, '' AS TotalFormat,  isnull(dbo.Causes.DayType, 0) as DayType, isnull(dbo.Causes.CustomType, 0) as CustomType
                            FROM         dbo.sysroEmployeeGroups with (nolock)
                                                INNER JOIN dbo.Causes with (nolock)
                                                INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND cast(dbo.DailyCauses.Date as date) between cast(dbo.sysroEmployeeGroups.BeginDate as date) AND cast(dbo.sysroEmployeeGroups.EndDate as date)
   				                            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND cast(dbo.DailyCauses.Date as date) >= cast(dbo.EmployeeContracts.BeginDate as date) AND cast(dbo.DailyCauses.Date as date) <= cast(dbo.EmployeeContracts.EndDate as date)
   				                            LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID
                            where   dbo.DailyCauses.IDEmployee = " & intIDEmployee.ToString
            If xDate.Equals(Nothing) Then
                strSQL &= " and dbo.dailycauses.date = " & Any2Time(xBeginPeriod).SQLSmallDateTime
            Else
                strSQL &= " and dbo.dailycauses.date between " & Any2Time(xBeginPeriod).SQLSmallDateTime & " and " & Any2Time(xDate).SQLSmallDateTime
            End If

            If idCause > 0 Then
                strSQL &= " AND dbo.Causes.ID = " & idCause.ToString
            End If

            strSQL &= " GROUP BY dbo.Causes.Name, dbo.Causes.WorkingType, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)), isnull(dbo.BusinessCenters.ID, 0), isnull(dbo.BusinessCenters.Name, ''),
		                    dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter, dbo.Causes.DayType, dbo.Causes.CustomType "

            Return strSQL
        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roBusinessCenterZone
        Private oState As roBusinessCenterZoneState

        Private intID As Integer
        Private strName As String

        <IgnoreDataMember()>
        Public Property State() As roBusinessCenterZoneState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roBusinessCenterZoneState)
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
            Me.oState = New roBusinessCenterZoneState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roBusinessCenterZoneState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID,Name FROM Zones " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                Else

                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBusinessCenterZone::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenterZone::Load")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper Methods"

        Public Shared Function GetZonesByBusinessCenter(ByVal IDCenter As Integer, ByVal _State As roBusinessCenterZoneState) As Generic.List(Of roBusinessCenterZone)

            Dim oRet As New Generic.List(Of roBusinessCenterZone)

            Try

                Dim strSQL As String = "@SELECT# IDZone FROM BusinessCenterZones, Zones Where Zones.ID = BusinessCenterZones.IDZone AND  IDCenter = " & IDCenter & " Order by Zones.Name"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oZoneDesc As roBusinessCenterZone = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oZoneDesc = New roBusinessCenterZone(oRow("IDZone"), _State)
                        oRet.Add(oZoneDesc)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roBusinessCenterZone::GetZonesByBusinessCenter")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roBusinessCenterZone::GetZonesByBusinessCenter")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessCenterZones(ByVal IDCenter As Integer, ByVal oState As roBusinessCenterState) As System.Data.DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# IDZone, Zones.Name as ZoneName FROM BusinessCenterZones, Zones Where Zones.ID = BusinessCenterZones.IDZone AND  IDCenter = " & IDCenter & " Order by Zones.Name"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterZones")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBusinessCenter::GetBusinessCenterZones")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

    End Class

    Public Class roBusinessCenterZoneState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BusinessCenterZoneResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.BusinessCenter", "TasksService")
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.BusinessCenter", "TasksService", _IDPassport)
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.BusinessCenter", "TasksService", _IDPassport, _Context)
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.BusinessCenter", "TasksService", _IDPassport, , _ClientAddress)
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.BusinessCenter", "TasksService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BusinessCenterZoneResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BusinessCenterZoneResultEnum)
                Me.intResult = value
                If Me.intResult <> BusinessCenterZoneResultEnum.NoError And Me.intResult <> BusinessCenterZoneResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BusinessCenterZoneResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BusinessCenterZoneResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BusinessCenterZoneResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace