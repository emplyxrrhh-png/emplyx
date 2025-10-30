Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Assignment

    <DataContract()>
    Public Class roAssignment

#Region "Declarations - Constructors"

        Private oState As roAssignmentState

        Private intID As Integer
        Private strName As String
        Private strShortName As String
        Private intColor As Integer
        Private strDescription As String
        Private strCostField As String
        Private strExport As String

        Public Sub New()

            Me.oState = New roAssignmentState
            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roAssignmentState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roAssignmentState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember()>
        Public Property State() As roAssignmentState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAssignmentState)
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
        Public Property CostField() As String
            Get
                Return strCostField
            End Get
            Set(ByVal value As String)
                strCostField = value
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
        Public Property Export() As String
            Get
                Return Me.strExport
            End Get
            Set(ByVal value As String)
                Me.strExport = value
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
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Assignments WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.strShortName = Any2String(oRow("ShortName"))
                    Me.intColor = Any2Integer(oRow("Color"))
                    Me.strDescription = Any2String(oRow("Description"))

                    Me.strCostField = Any2String(oRow("CostField"))

                    Me.strExport = Any2String(oRow("Export"))

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAssignment, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAssignment::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAssignment::Load")
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
                Me.intColor = Any2Integer(oRow("Color"))
                Me.strDescription = Any2String(oRow("Description"))

                Me.strCostField = Any2String(oRow("CostField"))
                Me.strExport = Any2String(oRow("Export"))

                bolRet = True

                If _Audit Then
                    ' ***

                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = AssignmentResultEnum.XSSvalidationError
                    Return False
                End If

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Assignment")
                    Dim strSQL As String = "@SELECT# * FROM Assignments " &
                                           "WHERE ID = " & Me.ID.ToString
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
                    oRow("ShortName") = Me.strShortName
                    oRow("Color") = Me.intColor

                    oRow("CostField") = Me.strCostField

                    If Me.strExport.Length = 0 Then Me.strExport = Me.ID
                    oRow("Export") = Me.strExport

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

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
                oState.UpdateStateInfo(ex, "roAssignment::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAssignment::Save")
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
                    oState.Result = DTOs.AssignmentResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El nombre corto no puede estar en blanco
                If Me.ShortName = "" Or Trim(Me.ShortName) = "" Then
                    oState.Result = DTOs.AssignmentResultEnum.ShortNameCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El nombre corto no puede estar en blanco
                If Me.ShortName.Length > 2 Then
                    oState.Result = DTOs.AssignmentResultEnum.ShortNameToLong
                    bolRet = False
                    Return False
                End If

                ' El codigo de equivalencia no puede estar en blanco
                If Me.Export.Length > 5 Then
                    oState.Result = DTOs.AssignmentResultEnum.ShortNameToLong
                    bolRet = False
                    Return False
                End If

                If bolRet And bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Assignments " &
                             "WHERE Name = @AssignmentName AND " &
                                   "ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@AssignmentName", DbType.String, 64)
                    cmd.Parameters("@AssignmentName").Value = Me.Name
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.AssignmentResultEnum.NameAlreadyExist
                        bolRet = False
                    End If

                    If bolRet Then
                        ' Compuebo que el nombre corto y el color no exista
                        tb = New DataTable()
                        strSQL = "@SELECT# * FROM Assignments " &
                                 "WHERE ShortName = @ShortName AND " &
                                 "Color = @Color AND " &
                                       "ID <> " & Me.ID.ToString
                        cmd = CreateCommand(strSQL)
                        AddParameter(cmd, "@ShortName", DbType.String, 64)
                        cmd.Parameters("@ShortName").Value = Me.ShortName
                        AddParameter(cmd, "@Color", DbType.Int32, 0)
                        cmd.Parameters("@Color").Value = Me.intColor

                        da = CreateDataAdapter(cmd, True)
                        tb.Rows.Clear()
                        da.Fill(tb)

                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            oState.Result = DTOs.AssignmentResultEnum.ShortNameAlreadyExist
                            bolRet = False
                        End If
                    End If

                    If bolRet Then
                        ' Compuebo que el codigo de equivalencia no exista
                        tb = New DataTable()
                        strSQL = "@SELECT# * FROM Assignments " &
                             "WHERE Export = @AssignmentExport AND " &
                                   "ID <> " & Me.ID.ToString
                        cmd = CreateCommand(strSQL)
                        AddParameter(cmd, "@AssignmentExport", DbType.String, 64)
                        cmd.Parameters("@AssignmentExport").Value = Me.Export
                        da = CreateDataAdapter(cmd, True)
                        tb.Rows.Clear()
                        da.Fill(tb)

                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            oState.Result = DTOs.AssignmentResultEnum.ExportAlreadyExist
                            bolRet = False
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAssignment::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAssignment::Validate")
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

            Dim strSQL As String = "@SELECT# MAX(ID) FROM Assignments"
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

            Try

                If Not Me.IsUsed() Then

                    bolRet = False

                    'Borramos el puesto
                    Dim DelQuerys() As String = {"@DELETE# FROM Assignments WHERE ID = " & Me.ID.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = DTOs.AssignmentResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = DTOs.AssignmentResultEnum.NoError)

                    If bolRet And bAudit Then
                        '' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAssignment, Me.strName, Nothing, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAssignment::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAssignment::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si el puesto se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim strUsed As String = ""

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado a ningun empleado
                    strSQL = "@SELECT# Employees.Name From Employees, EmployeeAssignments Where Employees.ID = EmployeeAssignments.IDEmployee And EmployeeAssignments.IDAssignment = " & Me.intID
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUsed = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los empleados que lo usan
                            strUsed &= "," & oRow("Name")
                        Next
                        If strUsed <> "" Then strUsed = strUsed.Substring(1)
                        If strUsed <> "" Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(strUsed)
                            oState.Result = DTOs.AssignmentResultEnum.UsedInEmployeeAssignments
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado a ningun horario
                    strSQL = "@SELECT# Shifts.Name From Shifts, ShiftAssignments Where Shifts.ID = ShiftAssignments.IDShift And ShiftAssignments.IDAssignment = " & Me.intID
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUsed = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre de todos los horarios que lo usan
                            strUsed &= "," & oRow("Name")
                        Next
                        If strUsed <> "" Then strUsed = strUsed.Substring(1)
                        If strUsed <> "" Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(strUsed)
                            oState.Result = DTOs.AssignmentResultEnum.UsedInShiftAssignments
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado a ninguna planificacion
                    strSQL = "@SELECT# Employees.Name, DailySchedule.Date From Employees, DailySchedule Where DailySchedule.IDEmployee = Employees.ID And (DailySchedule.IDAssignment = " & Me.intID & " OR DailySchedule.OldIDAssignment = " & Me.intID & ") Order by Name, Date"
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUsed = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre y la fecha  de todos los empleados que lo tienen planificado
                            strUsed &= "," & oRow("Name") & " " & Format(CDate(oRow("Date")), "dd/MM/yyyy")
                        Next
                        If strUsed <> "" Then strUsed = strUsed.Substring(1)
                        If strUsed <> "" Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(strUsed)
                            oState.Result = DTOs.AssignmentResultEnum.UsedInSchedulerAssignments
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado a ninguna dotacion teorica
                    strSQL = "@SELECT# Groups.Name, DailyCoverage.Date From Groups, DailyCoverage Where DailyCoverage.IDGroup = Groups.ID And DailyCoverage.IDAssignment = " & Me.intID & " Order by Name, Date"
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        strUsed = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre y la fecha  de todos los empleados que lo tienen planificado
                            strUsed &= "," & oRow("Name") & " " & Format(CDate(oRow("Date")), "dd/MM/yyyy")
                        Next
                        If strUsed <> "" Then strUsed = strUsed.Substring(1)
                        If strUsed <> "" Then

                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(strUsed)
                            oState.Result = DTOs.AssignmentResultEnum.UsedInCoverageAssignments
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAssignment::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAssignment::IsUsed")
            End Try

            Return bolIsUsed

        End Function

#Region "Helper methods"

        Public Shared Function GetAssignments(ByVal _Order As String, ByVal _State As roAssignmentState, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roAssignment)

            Dim oRet As New Generic.List(Of roAssignment)

            Try

                Dim strSQL As String = "@SELECT# Assignments.* " &
                                       "FROM Assignments "
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roAssignment(oRow, _State, _Audit))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::GetAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::GetAssignments")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAssignmentsDataTable(ByVal _Order As String, ByVal _State As roAssignmentState, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Assignments.* " &
                                       "FROM Assignments "
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                tbRet = CreateDataTable(strSQL, )
                If tbRet IsNot Nothing Then

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::GetAssignmentsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::GetAssignmentsDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de puestos que puede cubrir un empleado por un horario. Los ordena según la cobertura del puesto en el horario.
        ''' </summary>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="_IDShift"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeeAndShiftAssignments(ByVal _IDEmployee As Integer, ByVal _IDShift As Integer, ByVal _State As roAssignmentState, Optional ByVal _Audit As Boolean = True) As Generic.List(Of roAssignment)

            Dim oRet As New Generic.List(Of roAssignment)

            Try

                Dim strSQL As String = "@SELECT# ShiftAssignments.IDAssignment " &
                                       "FROM ShiftAssignments " &
                                       "WHERE ShiftAssignments.IDAssignment IN (@SELECT# EmployeeAssignments.IDAssignment FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = " & _IDEmployee.ToString & ") AND " &
                                             "ShiftAssignments.IDShift = " & _IDShift.ToString & " " &
                                       "ORDER BY ShiftAssignments.Coverage DESC"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roAssignment(CInt(oRow("IDAssignment")), _State, False))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::GetEmployeeAndShiftAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::GetEmployeeAndShiftAssignments")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeAndShiftAssignment(ByVal _IDEmployee As Integer, ByVal _IDShift As Integer, ByVal _IDAssignment As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Assignments.ID , Name, ShortName, Color, Coverage From EmployeeAssignments , ShiftAssignments,Assignments Where Assignments.ID = EmployeeAssignments.IDAssignment and ShiftAssignments.IDAssignment = EmployeeAssignments.IDAssignment and EmployeeAssignments.IDEmployee =" & _IDEmployee.ToString & " AND  EmployeeAssignments.IDAssignment = " & _IDAssignment.ToString & " AND  ShiftAssignments.IDSHift = " & _IDShift.ToString

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                '_State.UpdateStateInfo(ex, "roAssignment::GetEmployeeAndShiftAssignment")
            Catch ex As Exception
                '_State.UpdateStateInfo(ex, "roAssignment::GetEmployeeAndShiftAssignment")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el nombre de un puesto
        ''' </summary>
        ''' <param name="iIDAssignment"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetName(ByVal iIDAssignment As Integer, ByVal _State As roAssignmentState) As String

            Dim sRet As String = ""

            Try

                Dim strSQL As String

                strSQL = "@SELECT# Name FROM ASsignments WHERE ID = " & iIDAssignment.ToString
                sRet = ExecuteScalar(strSQL)
                If sRet Is Nothing Then sRet = "()"
            Catch ex As DbException
                If Not _State Is Nothing Then _State.UpdateStateInfo(ex, "roAssignment::GetName")
            Catch ex As Exception
                If Not _State Is Nothing Then _State.UpdateStateInfo(ex, "roAssignment::GetName")
            Finally

            End Try

            Return sRet

        End Function

#End Region

#End Region

        Public Shared Function GetAssignmentExportKeyById(ByVal iID As Integer) As String

            Dim oRet As String = String.Empty

            Try

                Dim strSQL As String
                If iID < 1 Then Return oRet

                strSQL = "@SELECT# Export FROM Assignments WHERE Id = " & iID.ToString
                oRet = Robotics.VTBase.roTypes.Any2String(ExecuteScalar(strSQL))
            Catch ex As Exception
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetAssignmentIdByExportKey(ByVal sKey As String) As Integer

            Dim oRet As Integer = -1

            Try

                Dim strSQL As String
                If sKey.Trim = String.Empty Then Return oRet

                strSQL = "@SELECT# ID FROM Assignments WHERE Export = '" & sKey & "'"
                oRet = Robotics.VTBase.roTypes.Any2Integer(ExecuteScalar(strSQL))
                If oRet = 0 Then oRet = -1
            Catch ex As Exception
            Finally

            End Try

            Return oRet
        End Function

    End Class

End Namespace