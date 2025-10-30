Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Concept

    <DataContract>
    Public Class roConceptGroup

#Region "Declarations - Constructor"

        Private oState As roConceptState

        Private intID As Integer
        Private strName As String = ""
        Private strBusinessCenter As String
        Private oConcepts As New Generic.List(Of roConcept)

        Public Sub New()
            Me.oState = New roConceptState
            Me.ID = -1
        End Sub

        Public Sub New(ByVal _IDConceptGroup As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = True)
            Me.oState = _State
            Me.ID = _IDConceptGroup
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roConceptState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roConceptState)
                Me.oState = value
                If Me.oConcepts IsNot Nothing Then
                    For Each oConcept As roConcept In oConcepts
                        oConcept.State = Me.oState
                    Next
                End If
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                'If Me.oConcepts IsNot Nothing Then
                '    For Each oConcept As roConcept In Me.oConcepts
                '        oConcept.ID = intID
                '    Next
                'End If
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember>
        Public Property BusinessCenter() As String
            Get
                Return strBusinessCenter
            End Get
            Set(ByVal value As String)
                strBusinessCenter = value
            End Set
        End Property

        <DataMember>
        Public Property Concepts() As Generic.List(Of roConcept)
            Get
                Return oConcepts
            End Get
            Set(ByVal value As Generic.List(Of roConcept))
                oConcepts = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroReportGroups WHERE ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))

                    If Not IsDBNull(oRow("BusinessGroup")) Then
                        Me.strBusinessCenter = oRow("BusinessGroup")
                    End If

                    Me.oConcepts = roConceptGroup.GetConceptsforGroup(Me.ID, Me.oState, False)

                    bolRet = True

                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tConceptGroup, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::Load")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nuevo acumulado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM sysroReportGroups"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try
                Dim strSQL As String
                Dim tb As DataTable
                Dim cmd As DbCommand
                Dim da As DbDataAdapter

                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = ConceptResultEnum.GroupNameCannotBeNull
                    bolRet = False
                    Return False
                End If

                If bolRet Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM sysroReportGroups " &
                             "WHERE Name = @ConceptGroupName AND " &
                                   "ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@ConceptGroupName", DbType.String, 64)
                    cmd.Parameters("@ConceptGroupName").Value = Me.Name
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = ConceptResultEnum.GroupNameAlreadyExist
                        bolRet = False
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = ConceptResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("sysroReportGroups")
                    Dim strSQL As String = "@SELECT# * FROM sysroReportGroups " &
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

                    oRow("Name") = Me.strName

                    If Me.strBusinessCenter <> String.Empty Then
                        oRow("BusinessGroup") = Me.strBusinessCenter
                    Else
                        oRow("BusinessGroup") = DBNull.Value
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    If Me.ID <= 0 Then
                        Dim tmpTb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM sysroReportGroups ORDER BY [ID] DESC")
                        If tmpTb IsNot Nothing AndAlso tmpTb.Rows.Count = 1 Then
                            Me.ID = tmpTb.Rows(0)("ID")
                        End If
                    End If

                    bolRet = True

                    ' Actualizamos la composición del grupo de acumulados
                    bolRet = SaveConceptsforGroup(Me.oState)

                    oAuditDataNew = oRow

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tConceptGroup, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra el grupo de acumulado.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then
                    bolRet = False
                    oState.Result = ConceptResultEnum.NoError

                    Dim DelQuerys() As String = {"@DELETE# FROM sysroReportGroupConcepts WHERE IDReportGroup = " & Me.ID.ToString,
                                                 "@DELETE# FROM sysroReportGroups WHERE ID = " & Me.ID.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = ConceptResultEnum.SqlError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = ConceptResultEnum.NoError)

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tConceptGroup, Me.strName, Nothing, -1)
                    End If
                Else
                    oState.Result = ConceptResultEnum.NotEmptyConcepts
                    bolRet = (oState.Result = ConceptResultEnum.NoError)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si el group de acumulados se encuentra vacío
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean
            Dim bolIsUsed As Boolean = False

            Try
                Dim strSQL As String
                Dim tb As Integer = 0
                Dim strUseConcept As String = ""

                ' Reglas de acumulados
                ' Verifica que no tenga acumulados asignados
                strSQL = "@SELECT# Count(*) FROM sysroReportGroupConcepts AS a INNER JOIN Concepts AS b ON (a.IDConcept = b.ID) Where a.IDReportGroup = " & Me.ID
                tb = ExecuteScalar(strSQL)

                If tb > 0 Then
                    bolIsUsed = True
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::IsUsed")
            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Modifica la posición del acumulado indicado.
        ''' </summary>
        ''' <param name="IDConcept">Código del acumulado</param>
        ''' <param name="bolUp">True para subir una posición. False para bajar una posición</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetConceptPosition(ByVal IDConcept As Integer, ByVal bolUp As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.oConcepts IsNot Nothing Then

                    Dim strSQL1 As String
                    Dim strSQL2 As String
                    If bolUp Then
                        strSQL1 = "@UPDATE# sysroReportGroupConcepts " &
                                  "SET Position = Position + 1 " &
                                  "WHERE IDReportGroup = " & Me.ID.ToString & " AND " &
                                        "Position = (@SELECT# g.Position - 1 FROM sysroReportGroupConcepts g " &
                                        "WHERE g.IDReportGroup = sysroReportGroupConcepts.IDReportGroup  AND IDConcept = " & IDConcept.ToString & " AND ISNULL(Position, 0) > 1)"

                        strSQL2 = "@UPDATE# sysroReportGroupConcepts " &
                                  "SET Position = Position - 1 " &
                                  "WHERE IDReportGroup = " & Me.ID.ToString & " AND " &
                                        "IDConcept = " & IDConcept.ToString & " AND " &
                                        "ISNULL(Position, 0) > 1"
                    Else
                        strSQL1 = "@UPDATE#  sysroReportGroupConcepts " &
                                  "SET Position = Position - 1 " &
                                  "WHERE IDReportGroup = " & Me.ID.ToString & " AND " &
                                        "Position = (@SELECT# g.Position + 1 FROM sysroReportGroupConcepts g " &
                                        "WHERE g.IDReportGroup = sysroReportGroupConcepts.IDReportGroup AND g.IDConcept = " & IDConcept.ToString & " AND ISNULL(Position, 0) < (@SELECT# COUNT(*) FROM sysroReportGroupConcepts g2 WHERE g2.IDReportGroup = g.IDReportGroup))"

                        strSQL2 = "@UPDATE# sysroReportGroupConcepts " &
                                  "SET Position = Position + 1 " &
                                  "WHERE IDReportGroup = " & Me.ID.ToString & " AND " &
                                        "IDConcept = " & IDConcept.ToString & " AND " &
                                        "ISNULL(Position, 0) < (@SELECT# COUNT(*) FROM sysroReportGroupConcepts g " &
                                          "WHERE g.IDReportGroup = sysroReportGroupConcepts.IDReportGroup)"
                    End If
                    bolRet = ExecuteSql(strSQL1)
                    If bolRet Then bolRet = ExecuteSql(strSQL2)

                    If bolRet Then
                        Me.oConcepts = roConceptGroup.GetConceptsforGroup(Me.ID, Me.oState)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptGroup::SetConceptPosition")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::SetConceptPosition")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function SaveConceptsforGroup(ByVal _State As roConceptState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strDeleteSQL As String = "@DELETE# FROM sysroReportGroupConcepts Where IDReportGroup = " & Me.ID

                bolRet = ExecuteSql(strDeleteSQL)

                'Gravem els acumulats amb l'ordre de gravació
                If bolRet Then
                    Dim intPos As Integer = 1
                    Dim strInsertSQL As String = ""
                    If oConcepts IsNot Nothing AndAlso oConcepts.Count > 0 Then
                        For Each oConcept As roConcept In oConcepts
                            strInsertSQL = "@INSERT# INTO sysroReportGroupConcepts (IDReportGroup, IDConcept, Position) Values(" & Me.ID & "," & oConcept.ID & "," & intPos & ")"
                            bolRet = ExecuteSql(strInsertSQL)
                            If bolRet = False Then Exit For
                            intPos += 1
                        Next
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptGroup::SaveConceptsforGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptGroup::SaveConceptsforGroup")
            End Try

            Return bolRet
        End Function

#Region "Helper methods"

        Public Shared Function GetConceptGroups(ByRef _State As roConceptState, ByVal filterBusinessGroups As Boolean) As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strBusiness As String = roConceptGroup.GetBusinessGroupList(_State)

                Dim strSQL As String = "@SELECT# * from sysroReportGroups "

                If filterBusinessGroups Then
                    If strBusiness <> String.Empty Then
                        strSQL &= " WHERE (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                    End If
                End If

                strSQL &= "Order By Name"

                dRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptGroups")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptGroups")
            Finally

            End Try

            Return dRet
        End Function

        Public Shared Function GetConceptIDsByBusinessGroupsPermissions(ByRef _State As roConceptState) As String
            Dim strFilter As String = "-1,"

            Try

                Dim strIDs As String = "-1"

                Dim dtFilter As DataTable = Concept.roConceptGroup.GetConceptGroups(_State, True)

                If dtFilter IsNot Nothing AndAlso dtFilter.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtFilter.Rows
                        strIDs &= "," & oRow("ID")
                    Next
                End If

                Dim strSQL As String = "@SELECT# IDConcept from sysroReportGroupConcepts where IDReportGroup IN(" & strIDs & ") UNION @SELECT# ID from concepts where ID not in(@SELECT# IDConcept from sysroReportGroupConcepts)"

                Dim tbTemp As DataTable = CreateDataTable(strSQL, )

                If tbTemp IsNot Nothing AndAlso tbTemp.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbTemp.Rows
                        strFilter &= oRow(0) & ","
                    Next
                    strFilter = strFilter.Substring(0, strFilter.Length - 1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptGroups")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptGroups")
            Finally

            End Try

            Return strFilter
        End Function

        Public Shared Function GetConceptsforGroupDatatable(ByVal IdConceptGroup As Integer, ByRef _State As roConceptState, Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from sysroReportGroupConcepts AS a INNER JOIN Concepts AS b ON (a.IDConcept = b.ID) Where IDReportGroup = " & IdConceptGroup & " Order By Position"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptsforGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptsforGroup")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptsforGroup(ByVal IdConceptGroup As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roConcept)

            Dim oRet As New Generic.List(Of roConcept)

            Dim dTbl As DataTable

            Try

                Dim strSQL As String = "@SELECT# * from sysroReportGroupConcepts AS a INNER JOIN Concepts AS b ON (a.IDConcept = b.ID) Where IDReportGroup = " & IdConceptGroup & " Order By Position"

                dTbl = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each dRow As DataRow In dTbl.Rows
                        oRet.Add(New roConcept(dRow("IDConcept"), _State, bAudit))
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptsforGroup")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptGroup::GetConceptsforGroup")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un datatable con los BusinessGroup de los grupos de horarios
        ''' </summary>
        Public Shared Function GetBusinessGroupFromConceptGroups(ByRef oState As roConceptState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroup FROM sysroReportGroups GROUP BY BusinessGroup HAVING (BusinessGroup <> '')"
                oRet = CreateDataSet(strQuery)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::GetBusinessGroupFromCauseGroups")
            End Try

            Return oRet

        End Function

        Private Shared Function GetBusinessGroupList(ByVal oState As roConceptState) As String
            Dim strRet As String = String.Empty

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ID IN(@SELECT# isnull(IDGroupFeature,-1) from sysroPassports WHERE id = " & oState.IDPassport & " ) "
                Dim dtBusinessGroups As System.Data.DataTable = CreateDataTable(strQuery)
                If dtBusinessGroups IsNot Nothing And dtBusinessGroups.Rows.Count > 0 Then

                    Dim arrAux() As String = roTypes.Any2String(dtBusinessGroups.Rows(0)("BusinessGroupList")).Split(";")
                    For Each item As String In arrAux
                        If item.Trim() <> String.Empty Then
                            strRet &= "'" & item.Trim().Replace("'", "''") & "',"
                        End If
                    Next
                    If strRet.Length > 0 Then strRet = strRet.Substring(0, strRet.Length() - 1)

                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptGroup::GetBusinessGroupList")
            End Try

            Return strRet

        End Function

#End Region

#End Region

    End Class

End Namespace