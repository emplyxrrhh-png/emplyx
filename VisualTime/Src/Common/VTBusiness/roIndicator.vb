Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Indicator

    <DataContract()>
    Public Class roIndicator

#Region "Declarations - Constructors"

        Private oState As roIndicatorState
        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private intType As Integer
        Private intIDFirstconcept As Integer
        Private intIdSecondConcept As Integer
        Private dblLimitValue As Nullable(Of Double)
        Private dblDesiredValue As Nullable(Of Double)
        Private strCondition As String
        Private bolAllowNotification As Boolean
        Private intTypeNotification As Integer

        Public Sub New()

            Me.oState = New roIndicatorState
            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roIndicatorState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roIndicatorState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember()>
        Public Property State() As roIndicatorState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roIndicatorState)
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
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property Type() As Integer
            Get
                Return Me.intType
            End Get
            Set(ByVal value As Integer)
                Me.intType = value
            End Set
        End Property
        <DataMember()>
        Public Property IDFirstConcept() As Integer
            Get
                Return Me.intIDFirstconcept
            End Get
            Set(ByVal value As Integer)
                Me.intIDFirstconcept = value
            End Set
        End Property
        <DataMember()>
        Public Property IDSecondConcept() As Integer
            Get
                Return Me.intIdSecondConcept
            End Get
            Set(ByVal value As Integer)
                Me.intIdSecondConcept = value
            End Set
        End Property
        <DataMember()>
        Public Property LimitValue() As Nullable(Of Double)
            Get
                Return Me.dblLimitValue
            End Get
            Set(ByVal value As Nullable(Of Double))
                Me.dblLimitValue = value
            End Set
        End Property
        <DataMember()>
        Public Property DesiredValue() As Nullable(Of Double)
            Get
                Return Me.dblDesiredValue
            End Get
            Set(ByVal value As Nullable(Of Double))
                Me.dblDesiredValue = value
            End Set
        End Property
        <DataMember()>
        Public Property Condition() As String
            Get
                Return Me.strCondition
            End Get
            Set(ByVal value As String)
                Me.strCondition = value
            End Set
        End Property
        <DataMember()>
        Public Property AllowNotification() As Boolean
            Get
                Return Me.bolAllowNotification
            End Get
            Set(ByVal value As Boolean)
                Me.bolAllowNotification = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeNotification() As Integer
            Get
                Return Me.intTypeNotification
            End Get
            Set(ByVal value As Integer)
                Me.intTypeNotification = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Indicators WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intType = Any2Integer(oRow("Type"))
                    Me.strName = Any2String(oRow("Name"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.intIDFirstconcept = Any2Integer(oRow("IDFirstConcept"))
                    Me.intIdSecondConcept = Any2Integer(oRow("IDSecondConcept"))

                    If Not IsDBNull(oRow("DesiredValue")) Then
                        Me.dblDesiredValue = Any2Double(oRow("DesiredValue"))
                    End If

                    If Not IsDBNull(oRow("LimitValue")) Then
                        Me.dblLimitValue = Any2Double(oRow("LimitValue"))
                    End If

                    Me.strCondition = Any2String(oRow("Condition"))
                    Me.bolAllowNotification = Any2Boolean(oRow("AllowNotification"))
                    Me.intTypeNotification = Any2Integer(oRow("TypeNotification"))

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tIndicator, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roIndicator::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roIndicator::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intID = oRow("ID")
                Me.intType = Any2Integer(oRow("Type"))
                Me.strName = Any2String(oRow("Name"))
                Me.strDescription = Any2String(oRow("Description"))
                Me.intIDFirstconcept = Any2Integer(oRow("IDFirstConcept"))
                Me.intIdSecondConcept = Any2Integer(oRow("IDSecondConcept"))

                If Not IsDBNull(oRow("DesiredValue")) Then
                    Me.dblDesiredValue = Any2Double(oRow("DesiredValue"))
                End If

                If Not IsDBNull(oRow("LimitValue")) Then
                    Me.dblLimitValue = Any2Double(oRow("LimitValue"))
                End If

                Me.strCondition = Any2String(oRow("Condition"))
                Me.bolAllowNotification = Any2Boolean(oRow("AllowNotification"))
                Me.intTypeNotification = Any2Integer(oRow("TypeNotification"))

                bolRet = True

                If _Audit Then
                    ' ***
                End If

            End If

            Return bolRet

        End Function

        Public Function Save(ByVal _Type As IndicatorsType, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = IndicatorResultEnum.XSSvalidationError
                    Return False
                End If

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("Indicators")
                    Dim strSQL As String = "@SELECT# * FROM Indicators WHERE ID = " & Me.ID.ToString
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
                    oRow("Type") = CType(_Type, Integer)
                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("IDFirstConcept") = Me.intIDFirstconcept
                    oRow("IDSecondConcept") = Me.intIdSecondConcept
                    oRow("DesiredValue") = Me.dblDesiredValue
                    oRow("LimitValue") = Me.dblLimitValue
                    oRow("Condition") = Me.strCondition
                    oRow("AllowNotification") = Me.bolAllowNotification
                    oRow("TypeNotification") = Me.intTypeNotification

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
                        oState.AddAuditFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tIndicator, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roIndicator::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIndicator::Save")
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
                    oState.Result = DTOs.IndicatorResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El primer saldo no puede ser nulo
                If Me.IDFirstConcept = -1 Then
                    oState.Result = DTOs.IndicatorResultEnum.IDFirstConceptCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El primer saldo no puede ser nulo
                If Me.IDSecondConcept = -1 Then
                    oState.Result = DTOs.IndicatorResultEnum.IDSecondConceptCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El primer saldo no puede ser nulo
                If Me.DesiredValue = -1 Then
                    oState.Result = DTOs.IndicatorResultEnum.DesiredValueCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El primer saldo no puede ser nulo
                If Me.LimitValue = -1 Then
                    oState.Result = DTOs.IndicatorResultEnum.LimitValueCannotBeNull
                    bolRet = False
                    Return False
                End If

                Dim startConcept As Concept.roConcept = New Concept.roConcept()
                Dim endConcept As Concept.roConcept = New Concept.roConcept()

                startConcept.ID = IDFirstConcept
                endConcept.ID = IDSecondConcept

                startConcept.Load(False)
                endConcept.Load(False)

                If startConcept IsNot Nothing And endConcept IsNot Nothing And startConcept.DefaultQuery.Equals(endConcept.DefaultQuery) = False Then
                    oState.Result = DTOs.IndicatorResultEnum.ConceptsTypeDiffer
                    bolRet = False
                    Return False
                End If

                If bolRet And bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM Indicators " &
                             "WHERE Name = @IndicatorName AND " &
                                   "ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IndicatorName", DbType.String, 64)
                    cmd.Parameters("@IndicatorName").Value = Me.Name
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = DTOs.IndicatorResultEnum.NameAlreadyExist
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roIndicator::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIndicator::Validate")
            Finally

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

            Dim strSQL As String = "@SELECT# MAX(ID) FROM Indicators"
            Dim tb As DataTable = CreateDataTable(strSQL, )
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
                    Dim DelQuerys() As String = {"@DELETE# FROM Indicators WHERE ID = " & Me.ID.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = DTOs.IndicatorResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = DTOs.IndicatorResultEnum.NoError)

                    If bolRet And bAudit Then
                        '' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tIndicator, Me.strName, Nothing, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roIndicator::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIndicator::Delete")
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
                    strSQL = "@SELECT# Groups.Name From Groups, GroupIndicators Where Groups.ID = GroupIndicators.IDGroup And GroupIndicators.IDIndicator = " & Me.intID
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
                            oState.Result = DTOs.IndicatorResultEnum.UsedInGroupAssignments
                            oState.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roIndicator::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIndicator::IsUsed")
            End Try

            Return bolIsUsed

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetIndicators(ByVal _Order As String, ByVal _State As roIndicatorState, ByVal _Type As IndicatorsType, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roIndicator)

            Dim oRet As New Generic.List(Of roIndicator)

            Try

                Dim strSQL As String = "@SELECT# Indicators.* " &
                                       "FROM Indicators " &
                                       "WHERE Indicators.Type = " & CType(_Type, Integer).ToString
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roIndicator(oRow, _State, _Audit))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roIndicator::GetIndicators")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roIndicator::GetIndicators")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetIndicatorsDataTable(ByVal _Order As String, ByVal _State As roIndicatorState, ByVal _Type As IndicatorsType, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Indicators.* " &
                                       "FROM Indicators " &
                                       "WHERE Indicators.Type = " & CType(_Type, Integer).ToString
                If _Order <> "" Then strSQL &= " ORDER BY " & _Order
                tbRet = CreateDataTable(strSQL, )
                If tbRet IsNot Nothing Then

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roIndicator::GetIndicatorsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roIndicator::GetIndicatorsDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function CalculateIndicatorValueForGroup(ByVal IDindicator As Integer, ByVal IDGroup As Integer, ByVal InitialDate As String, ByVal EndDate As String, ByVal _State As roIndicatorState, Optional ByVal _Audit As Boolean = False) As Double

            Dim retValue As Double = 0

            Try

                Dim oIndicator As roIndicator
                oIndicator = New roIndicator(IDindicator, _State, _Audit)

                Dim indValue As Double? = roIndicator.GetIndicatorValueByGroup(oIndicator, IDGroup, InitialDate, EndDate)

                If (indValue.HasValue) Then
                    retValue = indValue.Value
                Else
                    retValue = -1
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roIndicator::CalculateIndicatorValue")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roIndicator::CalculateIndicatorValue")
            Finally

            End Try

            Return retValue

        End Function

        Public Shared Function CalculateIndicatorEvolutionForGroup(ByVal IDindicator As Integer, ByVal IDGroup As Integer, ByVal InitialDate As Date, ByVal EndDate As Date, ByVal groupType As IndicatorEvolutionGroupType, ByVal _State As roIndicatorState, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim retTb As New DataTable

            Try

                Dim oIndicator As roIndicator
                oIndicator = New roIndicator(IDindicator, _State, _Audit)

                retTb.Columns.Add("ValueDate", GetType(Date))
                retTb.Columns.Add("Value", GetType(Double))

                Dim iPeriod As Date = InitialDate
                Dim ePeriod As Date = EndDate

                'While (InitialDate > EndDate)

                '    Dim curValue As Double? = roIndicator.GetIndicatorValueByGroup(oIndicator, IDGroup, InitialDate, EndDate)

                '    Dim oNewRow As DataRow = retTb.NewRow
                '    With oNewRow
                '        .Item("ValueDate") = InitialDate
                '        If (curValue.HasValue) Then
                '            .Item("Value") = curValue.Value
                '        Else
                '            .Item("Value") = DBNull.Value
                '        End If
                '    End With
                'End While
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roIndicator::CalculateIndicatorEvolutionForGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roIndicator::CalculateIndicatorEvolutionForGroup")
            Finally

            End Try

            Return retTb

        End Function

        Private Shared Function GetIndicatorValueByGroup(ByVal oIndicator As roIndicator, ByVal IDGroup As Integer, ByVal InitialDate As Date, ByVal EndDate As Date) As Double?
            Dim retValue As Double?
            Dim sSQL As String = "@SELECT# DailyAccruals.IDConcept, SUM(DailyAccruals.Value) AS AccrualValue"
            sSQL = sSQL & " From Concepts, DailyAccruals, sysroEmployeeGroups, EmployeeContracts"
            sSQL = sSQL & " Where Concepts.ID = DailyAccruals.IDConcept AND DailyAccruals.IDEmployee = sysroEmployeeGroups.IDEmployee"
            sSQL = sSQL & " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate"
            sSQL = sSQL & " AND DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND DailyAccruals.Date >= EmployeeContracts.BeginDate"
            sSQL = sSQL & " AND DailyAccruals.Date <= EmployeeContracts.EndDate AND (DailyAccruals.Date BETWEEN " & Any2Time(InitialDate).SQLSmallDateTime & " AND  " & Any2Time(EndDate).SQLSmallDateTime & ")"
            sSQL = sSQL & " AND (Concepts.ID IN (" & oIndicator.IDFirstConcept & "," & oIndicator.IDSecondConcept & ")) AND sysroEmployeeGroups.Path like( "
            sSQL = sSQL & " (@SELECT# Groups.Path from Groups where Groups.ID = " & IDGroup & ") + '%' )"
            sSQL = sSQL & " GROUP BY DailyAccruals.IDConcept "

            Dim initialConceptValue As Double = 0
            Dim endConceptValue As Double = 0
            Dim indicatorValTmp As Double = 0

            Dim dt As DataTable = CreateDataTable(sSQL, )
            If Not dt Is Nothing Then
                For Each cRow As DataRow In dt.Rows
                    If Any2Integer(cRow("IDConcept")) = oIndicator.IDFirstConcept Then
                        initialConceptValue = initialConceptValue + Any2Double(cRow("AccrualValue"))
                    Else
                        endConceptValue = endConceptValue + Any2Double(cRow("AccrualValue"))
                    End If

                Next
            End If

            If endConceptValue > 0 Then
                retValue = (initialConceptValue * 100) / endConceptValue
            Else
                retValue = Nothing
            End If

            Return retValue
        End Function

#End Region

    End Class

End Namespace