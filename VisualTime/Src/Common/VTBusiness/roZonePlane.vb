Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Zone

    <DataContract()>
    Public Class roZonePlane

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private intID As Integer
        Private strName As String

        Private oPlaneImage As Byte()

        Public Sub New()
            Me.oState = New roZoneState()
            Me.intID = -1
            ReDim Me.oPlaneImage(-1)
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roZoneState,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            ReDim Me.oPlaneImage(-1)
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roZoneState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roZoneState)
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
        Public Property PlaneImage() As Byte()
            Get
                Return Me.oPlaneImage
            End Get
            Set(ByVal value As Byte())
                Me.oPlaneImage = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonePlanes " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    If Not IsDBNull(oRow("PlaneImage")) Then
                        Dim bits As Byte() = CType(oRow("PlaneImage"), Byte())
                        Me.oPlaneImage = bits
                    Else
                        Me.oPlaneImage = Nothing
                    End If
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tZonePlane, Me.strName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roZonePlane::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roZonePlane::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True
            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("ZonePlanes")
                Dim strSQL As String = "@SELECT# * FROM ZonePlanes WHERE ID = " & Me.intID.ToString
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
                If Me.oPlaneImage IsNot Nothing AndAlso Me.oPlaneImage.Length > 0 Then
                    oRow("PlaneImage") = Me.oPlaneImage
                Else
                    oRow("PlaneImage") = DBNull.Value
                End If

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

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
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tZonePlane, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZonePlane::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZonePlane::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Try
                'Comprovem si existeix algun terminal amb la zona associada
                Dim dTblTermReader As DataTable = CreateDataTable("@SELECT# * from Zones Where IDPlane = " & Me.intID.ToString)
                If dTblTermReader.Rows.Count > 0 Then
                    oState.Result = DTOs.ZoneResultEnum.ZonePlaneInZone
                    bolRet = False
                End If

                If bolRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM ZonePlanes WHERE ID = " & Me.intID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tZonePlane, Me.strName, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZonePlane::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZonePlane::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente codigo zona a usar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM ZonePlanes "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roZonePlane::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZonePlane::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetZonePlanesList(ByVal strOrderBy As String, ByVal _State As roZoneState,
                                                    Optional ByVal bAudit As Boolean = False) As Generic.List(Of roZonePlane)

            Dim oRet As New Generic.List(Of roZonePlane)

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonePlanes ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oZonePlane As roZonePlane = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oZonePlane = New roZonePlane(oRow("ID"), _State, False)
                        oRet.Add(oZonePlane)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetZonePlanesDataTable(ByVal strOrderBy As String, ByVal _State As roZoneState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM ZonePlanes ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function GetZonesFromPlane(ByVal _IDZonePlane As Integer, ByVal _State As roZoneState,
                                                 Optional ByVal bAudit As Boolean = False) As Generic.List(Of roZone)

            Dim oRet As New Generic.List(Of roZone)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# ID FROM Zones WHERE IDPlane = " & _IDZonePlane.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each dRow As DataRow In tb.Rows
                        oRet.Add(New roZone(dRow("ID"), _State, bAudit))
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesFromPlane")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZonePlane::GetZonesFromPlane")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace