Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Camera

    <DataContract()>
    Public Class roCamera

#Region "Declarations - Constructor"

        Private oState As roCameraState

        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private strModel As String
        Private strURL As String

        Public Sub New()
            Me.oState = New roCameraState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roCameraState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roCameraState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roCameraState)
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
        Public Property Model() As String
            Get
                Return Me.strModel
            End Get
            Set(ByVal value As String)
                Me.strModel = value
            End Set
        End Property

        <DataMember()>
        Public Property Url() As String
            Get
                Return Me.strURL
            End Get
            Set(ByVal value As String)
                Me.strURL = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Cameras " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    If Not IsDBNull(oRow("Description")) Then Me.strDescription = oRow("Description")
                    If Not IsDBNull(oRow("Model")) Then Me.strModel = oRow("Model")
                    If Not IsDBNull(oRow("Url")) Then Me.strURL = oRow("Url")
                Else
                    Me.strName = ""
                    Me.strDescription = ""
                    Me.strModel = ""
                    Me.strURL = ""
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCamera, Me.strName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCamera::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCamera::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = CameraResultEnum.XSSvalidationError
                    Return False
                End If

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("Cameras")
                Dim strSQL As String = "@SELECT# * FROM Cameras WHERE ID = " & Me.intID.ToString
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
                oRow("Description") = Me.strDescription
                oRow("Model") = "" 'Me.strModel
                oRow("Url") = Me.strURL

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                oAuditDataNew = oRow
                bolRet = True

                If bolRet And bAudit Then
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
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tCamera, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCamera::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCamera::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                'Comprovem si existeix algun terminal amb la zona associada
                Dim dTblTermReader As DataTable = CreateDataTable("@SELECT# * from TerminalReaders Where IDCamera = " & Me.ID)
                If dTblTermReader.Rows.Count > 0 Then
                    oState.Result = DTOs.CameraResultEnum.CameraInTerminalReaders
                    bolRet = False
                End If

                If bolRet Then
                    'Comprovem si existeix algun periode amb la zona
                    Dim dTblPeriod As DataTable = CreateDataTable("@SELECT# * from Zones Where IDCamera = " & Me.ID)
                    If dTblPeriod.Rows.Count > 0 Then
                        oState.Result = DTOs.CameraResultEnum.CameraInZones
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM Cameras WHERE ID = " & Me.intID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCamera, Me.strName, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCamera::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCamera::Delete")
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
                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM Cameras "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roCamera::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCamera::GetNextID")
            End Try

            Return intRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetCamerasList(ByVal strOrderBy As String, ByVal _State As roCameraState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roCamera)

            Dim oRet As New Generic.List(Of roCamera)

            Try

                Dim strSQL As String = "@SELECT# * FROM Cameras ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL = "Name ASC"
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oCamera As roCamera = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oCamera = New roCamera(oRow("ID"), _State, bAudit)
                        oRet.Add(oCamera)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roCamera::GetCamerasList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCamera::GetCamerasList")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCamerasDataTable(ByVal strOrderBy As String, ByVal _State As roCameraState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM Cameras ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roCamera::GetCamerasDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCamera::GetCamerasDataTable")
            End Try

            Return tbRet

        End Function

        Private Shared Function SearchEmployeeList(ByVal IDEmployee As Integer, ByRef dTblIn As DataTable, ByRef dTblOut As DataTable) As Boolean
            Dim bolRet As Boolean = False
            If dTblIn IsNot Nothing Then
                If dTblIn.Rows.Count > 0 Then

                End If
            End If
            If dTblOut IsNot Nothing Then
                If dTblOut.Rows.Count > 0 Then

                End If
            End If
        End Function

        Public Shared Function GetIDCameraFromReader(ByVal intIDTerminal As Integer, ByVal intIDReader As Integer, ByVal _State As roCameraState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCamera FROM TerminalReaders WHERE IDTerminal = " & intIDTerminal.ToString & " AND ID = " & intIDReader.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    If Not IsDBNull(tb.Rows(0).Item("IDCamera")) Then
                        intRet = tb.Rows(0).Item("IDCamera")
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCamera::GetIDCameraFromReader")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCamera::GetIDCameraFromReader")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function ExitsCamera(ByVal IDCamera As Integer, ByVal _State As roCameraState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String
                If IDCamera = -1 Then
                    strSQL = "@SELECT# TOP 1 ID FROM Cameras"
                Else
                    strSQL = "@SELECT# ID FROM Cameras WHERE ID = " & IDCamera
                End If

                Dim tb As DataTable = CreateDataTable(strSQL)
                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCamera::ExitsCamera")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCamera::ExitsCamera")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace