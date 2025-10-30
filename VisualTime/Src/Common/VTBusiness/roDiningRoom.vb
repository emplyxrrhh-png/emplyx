Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DiningRoom

    <DataContract>
    Public Class roDiningRoomTurn

#Region "Declarations - Constructor"

        Private oState As roDiningRoomState
        Private bolNewTurn As Integer

        Private intIDDiningRoom As Integer
        Private intID As Integer
        Private strName As String
        Private strExport As String
        Private oEmployeeSelection As roCollection
        Private dBeginTime As Nullable(Of Date)
        Private dEndTime As Nullable(Of Date)
        Private strDaysOfWeek As String

        Private Const EMPTY_WEEK As String = "0000000"
        Private Const PARAM_USERFIELDS As String = "prmUserFields"

        Public Sub New()
            Me.oState = New roDiningRoomState()
            Me.IDDiningRoom = -1
            Me.ID = -1
            Me.strName = ""
            Me.strDaysOfWeek = EMPTY_WEEK
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roDiningRoomState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.ID = _ID
            Me.IDDiningRoom = -1
            Me.strName = ""
            Me.strDaysOfWeek = EMPTY_WEEK
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        <DataMember>
        Public Property IDDiningRoom() As Integer
            Get
                Return intIDDiningRoom
            End Get
            Set(ByVal value As Integer)
                intIDDiningRoom = value
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
        Public Property Export() As String
            Get
                Return strExport
            End Get
            Set(ByVal value As String)
                strExport = value
            End Set
        End Property

        <IgnoreDataMember()>
        <XmlIgnore()>
        Public Property EmployeeSelection() As roCollection
            Get
                Return Me.oEmployeeSelection
            End Get
            Set(ByVal value As roCollection)
                Me.oEmployeeSelection = value
            End Set
        End Property

        <DataMember>
        Public Property DefinitionXML() As String
            Get
                Dim strXML As String = ""
                If Me.oEmployeeSelection IsNot Nothing Then
                    strXML = Me.oEmployeeSelection.XML()
                End If
                Return strXML
            End Get
            Set(ByVal value As String)
                Me.oEmployeeSelection = New roCollection(value)
            End Set
        End Property

        <DataMember>
        Public Property BeginTime() As Nullable(Of Date)
            Get
                Return dBeginTime
            End Get
            Set(ByVal value As Nullable(Of Date))
                dBeginTime = value
            End Set
        End Property

        <DataMember>
        Public Property EndTime() As Nullable(Of Date)
            Get
                Return dEndTime
            End Get
            Set(ByVal value As Nullable(Of Date))
                dEndTime = value
            End Set
        End Property

        <DataMember>
        Public Property DaysOfWeek() As String
            Get
                Return strDaysOfWeek
            End Get
            Set(ByVal value As String)
                strDaysOfWeek = value
            End Set
        End Property

        <IgnoreDataMember()>
        Public Property State() As roDiningRoomState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDiningRoomState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Shared Function ValidateCorrectWeek(ByVal strWeek As String) As String
            Dim strFin As String = String.Empty
            If Not strWeek Is Nothing AndAlso strWeek.Length > 0 Then
                For n As Integer = 0 To strWeek.Length - 1
                    If strWeek(n) = "0" Or strWeek(n) = "1" Then
                        strFin &= strWeek(n)
                    Else
                        strFin &= "0"
                    End If
                Next
                strFin = (strFin & EMPTY_WEEK).Substring(0, EMPTY_WEEK.Length)
                Return strFin
            Else
                Return EMPTY_WEEK
            End If
        End Function

        Public Sub Load(Optional ByVal bAudit As Boolean = False)
            Dim strQuery As String
            Dim oDataset As Data.DataSet
            Dim oDatareader As Data.Common.DbDataReader

            If Me.ID = -1 Then Exit Sub

            strQuery = "@SELECT# * from DiningRoomTurns WHERE ID = " & Me.ID

            Try
                oDataset = CreateDataSet(strQuery)

                If oDataset IsNot Nothing Then
                    oDatareader = oDataset.CreateDataReader()

                    If oDatareader IsNot Nothing Then
                        If oDatareader.HasRows Then
                            If oDatareader.Read() Then

                                Me.intID = Any2Integer(oDatareader("ID"))
                                Me.intIDDiningRoom = Any2Integer(oDatareader("IDDiningRoom"))
                                Me.strName = Any2String(oDatareader("Name"))
                                If Not IsDBNull(oDatareader("Export")) Then
                                    Me.strExport = oDatareader("Export")
                                End If

                                Me.oEmployeeSelection = New roCollection()
                                Try
                                    If Not IsDBNull(oDatareader("EmployeeSelection")) Then Me.oEmployeeSelection.LoadXMLString(oDatareader("EmployeeSelection"))
                                Catch
                                End Try

                                Me.dBeginTime = oDatareader("BeginTime")
                                Me.dEndTime = oDatareader("EndTime")
                                Me.strDaysOfWeek = ValidateCorrectWeek(Any2String(oDatareader("DaysOfWeek")))

                                ' Auditamos consulta Turno
                                If bAudit Then
                                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                    oState.AddAuditParameter(tbParameters, "{DiningRoom}", Me.Name, "", 1)
                                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDiningRoom, Me.Name, tbParameters, -1)
                                End If

                            End If

                            oDatareader.Close()

                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Load")
            End Try

        End Sub

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = DiningRoomResultEnum.XSSvalidationError
                    Return False
                End If


                If Me.Validate() Then

                    Dim oTurnOld As DataRow = Nothing
                    Dim oTurnNew As DataRow = Nothing
                    Dim strQueryRow As String
                    Dim NewID As Integer

                    Dim intIDTurnOld As Integer = Me.intID

                    strQueryRow = "@SELECT# * FROM DiningRoomTurns WHERE ID = " & Me.intID
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "DiningRoomTurns")
                    If tbAuditOld.Rows.Count = 1 Then oTurnOld = tbAuditOld.Rows(0)

                    ' Si es un Turno nuevo genero el ID
                    If Me.intID = -1 Then

                        Me.bolNewTurn = True
                        NewID = GetNextIDTurn()
                        If NewID = -1 Then
                            Me.oState.Result = DiningRoomResultEnum.ErrorGeneratingNewID
                        End If
                        Me.ID = NewID

                    End If

                    If oState.Result <> DiningRoomResultEnum.NoError Or oState.ErrorNumber <> 0 Then
                        bolRet = Me.SaveData()
                        If Not bolRet Then oState.Result = DiningRoomResultEnum.ErrorSavingData
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos modificación horario
                        strQueryRow = "@SELECT# * FROM DiningRoomTurns WHERE ID = " & Me.intID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "DiningRoomTurns")
                        If tbAuditNew.Rows.Count = 1 Then oTurnNew = tbAuditNew.Rows(0)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditFieldsValues(tbParameters, oTurnNew, oTurnOld)
                        Dim oAuditAction As Audit.Action = IIf(oTurnOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oTurnNew("Name")
                        ElseIf oTurnOld("Name") <> oTurnNew("Name") Then
                            strObjectName = oTurnOld("Name") & " -> " & oTurnNew("Name")
                        Else
                            strObjectName = oTurnNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDiningRoom, strObjectName, tbParameters, -1)

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDiningRoomTurn::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDiningRoomTurn::Save")
            End Try

            Return bolRet

        End Function

        Private Function SaveData() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim tb As New DataTable("DiningRoomTurns")
                Dim strSQL As String = "@SELECT# * FROM DiningRoomTurns WHERE ID = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("ID") = Me.intID
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("IDDiningRoom") = Me.intIDDiningRoom
                oRow("Name") = Me.strName
                If Me.strExport <> "" Then
                    oRow("Export") = Me.strExport
                Else
                    oRow("Export") = DBNull.Value
                End If
                oRow("EmployeeSelection") = Me.oEmployeeSelection.XML()
                oRow("BeginTime") = Me.dBeginTime
                oRow("EndTime") = Me.dEndTime
                oRow("DaysOfWeek") = Me.strDaysOfWeek

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::SaveData")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        '''  Valida que todos los campos tengan valores correctos
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                'Comprobar que el nombre del turno no esté en uso
                'If Me.intID = -1 Then
                If ExistName() Then
                    Me.oState.Result = DiningRoomResultEnum.DiningRoomNameAlreadyExist
                    bolRet = False
                End If
                'End If

                'Comprobar que el id del comedor del turno tenga un valor
                If bolRet AndAlso (intIDDiningRoom <= 0 Or intIDDiningRoom > Integer.MaxValue) Then
                    Me.oState.Result = DiningRoomResultEnum.IncorrectValue
                    bolRet = False
                End If

                'Comprobar que el nombre del turno tenga un nombre
                If bolRet AndAlso strName.Trim = String.Empty Then
                    Me.oState.Result = DiningRoomResultEnum.IncorrectValue
                    bolRet = False
                End If

                If bolRet Then
                    '===================================================
                    Dim oAuxEmployeeSelection As New roCollection(Me.DefinitionXML)
                    If oAuxEmployeeSelection.Count > 0 Then

                        Dim strValue As String
                        strValue = oEmployeeSelection.Item(PARAM_USERFIELDS, roCollection.roSearchMode.roByKey)
                        If strValue <> String.Empty Then
                            If strValue.Contains("~") Then
                                Try
                                    Dim strFieldName As String = strValue.Substring(0, strValue.IndexOf("~"))
                                    Dim strFieldValue As String = strValue.Substring(strValue.IndexOf("~") + 1, strValue.Length() - strValue.IndexOf("~") - 1)
                                    If strFieldName = String.Empty Or strFieldValue = String.Empty Then
                                        Me.oState.Result = DiningRoomResultEnum.UserFieldEmpty
                                        bolRet = False
                                    End If
                                Catch
                                    Me.oState.Result = DiningRoomResultEnum.UserFieldEmpty
                                    bolRet = False
                                End Try
                            Else
                                Me.oState.Result = DiningRoomResultEnum.UserFieldEmpty
                                bolRet = False
                            End If
                        End If
                    End If
                End If
                '=====================================================

                'Comprobar que tenga un valor
                If bolRet AndAlso Not dBeginTime.HasValue Then
                    Me.oState.Result = DiningRoomResultEnum.IncorrectValue
                    bolRet = False
                End If

                'Comprobar que tenga un valor
                If bolRet AndAlso Not dEndTime.HasValue Then
                    Me.oState.Result = DiningRoomResultEnum.IncorrectValue
                    bolRet = False
                End If

                'Comprobar que la hora final sea mayor que la hora inicial
                If bolRet AndAlso Any2Time(dBeginTime.Value).VBNumericValue > Any2Time(dEndTime.Value).VBNumericValue Then
                    Me.oState.Result = DiningRoomResultEnum.InvalidPeriod
                    bolRet = False
                End If

                'Comprobar que haya marcado algún día de la semama

                If bolRet Then
                    strDaysOfWeek = ValidateCorrectWeek(strDaysOfWeek)
                    If ValidateCorrectWeek(strDaysOfWeek) = EMPTY_WEEK Then
                        Me.oState.Result = DiningRoomResultEnum.DaysOfWeekEmpty
                        bolRet = False
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function GetNextIDTurn() As Integer
            Dim intRet As Integer = 1

            Try

                Dim strSql As String = "@SELECT# Max(ID) as counter From DiningRoomTurns"
                Dim tb As DataTable = CreateDataTable(strSql)

                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Counter")) Then
                        intRet = tb.Rows(0)("Counter") + 1
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::GetNextTurnID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::GetNextTurnID")
            End Try

            Return intRet

        End Function

        Private Function ExistName() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ID FROM DiningRoomTurns WHERE Name = '" & Me.strName & "' AND ID <> " & Me.intID
                Dim tb As DataTable = CreateDataTable(strSQL)
                bolRet = (tb.Rows.Count > 0)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::ExistName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::ExistName")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSql As String = "@DELETE# DiningRoomTurns WHERE ID= " & intID
                bolRet = ExecuteSql(strSql)

                If bolRet And bAudit Then
                    ' Auditamos borrado Turno
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{DiningRoom}", Me.Name, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDiningRoom, Me.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::Delete")
            End Try

            Return bolRet

        End Function

        Public Function GetDiningRoomTurns(ByVal intIDDiningRoom As Integer) As DataTable

            Dim tb As DataTable = Nothing

            oState.UpdateStateInfo()

            Try
                Dim strSQL As String = "@SELECT# * FROM DiningRoomTurns "

                If intIDDiningRoom <> -1 Then
                    strSQL &= " WHERE IDDiningRoom = " & intIDDiningRoom & " "
                End If

                strSQL &= " ORDER BY Name"

                tb = CreateDataTable(strSQL, "DiningRoomTurns")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurns::Turns")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurns::Turns")
            End Try

            Return tb

        End Function

#End Region

#Region "Helper Methods"

        ''' <summary>
        ''' Crea una copia de un turno existente.
        ''' </summary>
        ''' <param name="_IDSourceTurn">Código del Turno del que se quiere realizar la copia</param>
        ''' <param name="_NewName">Nombre del nuevo Turno creado. Si no se informa, se utiliza el tag de idioma 'DiningRoomTurns.DiningRoomTurnsSave.Copy' para generar el nuevo nombre (copia de ...).</param>
        ''' <param name="_State"></param>
        ''' <returns>Nuevo Turno creado</returns>
        ''' <remarks></remarks>
        Public Shared Function CopyDiningRoomTurn(ByVal _IDSourceTurn As Integer, ByVal _NewName As String, ByVal _State As roDiningRoomState, Optional ByVal bAudit As Boolean = False) As roDiningRoomTurn

            Dim oRet As roDiningRoomTurn = Nothing

            Try

                oRet = New roDiningRoomTurn(_IDSourceTurn, _State, False)

                oRet.ID = -1
                If _NewName = "" Then
                    _State.Language.ClearUserTokens()
                    _State.Language.AddUserToken(oRet.Name)
                    _NewName = _State.Language.Translate("DiningRoomTurns.DiningRoomTurnsSave.Copy", "")
                    _State.Language.ClearUserTokens()
                End If
                oRet.Name = _NewName

                If Not oRet.Save(bAudit) Then
                    oRet = Nothing
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDiningRoomTurn::CopyShift")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDiningRoomTurn::CopyShift")
            End Try

            Return oRet

        End Function

        Public Shared Function ExitsDiningRoomTurn(ByVal IdDiningRoomTurn As Integer, ByRef _State As roDiningRoomState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String
                If IdDiningRoomTurn = -1 Then
                    strSQL = "@SELECT# TOP 1 ID FROM DiningRoomTurns"
                Else
                    strSQL = "@SELECT# ID FROM DiningRoomTurns WHERE ID = " & IdDiningRoomTurn
                End If

                Dim tb As DataTable = CreateDataTable(strSQL)
                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDiningRoomTurn::ExitsDiningRoomTurn")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDiningRoomTurn::ExitsDiningRoomTurn")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract>
    Public Class roDiningRoom

#Region "Declarations - Constructor"

        Private oState As roDiningRoomState

        Private intID As Integer
        Private strName As String

        Private _Audit As Boolean = False

        Public Sub New()
            Me.oState = New roDiningRoomState
            Me.ID = 0
            Me.Name = String.Empty
        End Sub

        Public Sub New(ByVal _State As roDiningRoomState)
            Me.oState = _State
            Me.ID = 0
            Me.Name = String.Empty
        End Sub

        Public Sub New(ByVal _IDDiningRoom As Integer, ByVal _State As roDiningRoomState, Optional ByVal bAudit As Boolean = False)
            Me._Audit = bAudit
            Me.oState = _State
            Me.ID = _IDDiningRoom
            Me.Name = String.Empty
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                Me.Load(_Audit)
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

        <IgnoreDataMember>
        Public Property State() As roDiningRoomState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDiningRoomState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            If Me.intID <= 0 Then

                Me.strName = ""
            Else

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM DiningRooms WHERE [ID] = " & Me.ID.ToString)
                    If tb.Rows.Count > 0 Then

                        Me.strName = Any2String(tb.Rows(0).Item("Name"))

                        ' Auditamos consulta comedor
                        If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{DiningRoom}", Me.Name, "", 1)
                            oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDiningRoom, Me.Name, tbParameters, -1)
                        End If

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roDiningRoom::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roDiningRoom::Load")
                End Try

            End If

        End Sub

        Public Function ValidateDiningRoom() As Boolean

            ' El nombre no puede estar en blanco
            If Me.Name = "" Then
                oState.Result = DiningRoomResultEnum.DiningRoomNameRequired
                Return False
            End If

            ' El nombre no puede existir en la bdd para otro comedor
            Dim strQuery As String
            Dim oDataSet As System.Data.DataSet

            strQuery = "@SELECT# Name From DiningRooms WHERE ID <> " & Me.ID & " AND Name = '" & Me.Name.Replace("'", "''") & "'"
            oDataSet = CreateDataSet(strQuery)
            If oDataSet.CreateDataReader.HasRows = True Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = DiningRoomResultEnum.DiningRoomNameAlreadyExist
                Return False
            End If

            Return True

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = DiningRoomResultEnum.XSSvalidationError
                    Return False
                End If

                If ValidateDiningRoom() Then

                    Dim strQueryRow As String = ""
                    Dim oDiningRoomOld As DataRow = Nothing
                    Dim oDiningRoomNew As DataRow = Nothing

                    strQueryRow = "@SELECT# ID, Name FROM DiningRoom WHERE ID = " & Me.intID
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "DiningRooms")
                    If tbAuditOld.Rows.Count = 1 Then oDiningRoomOld = tbAuditOld.Rows(0)

                    Dim tbDiningRooms As New DataTable("DiningRooms")
                    Dim strSQL As String = "@SELECT# * FROM DiningRoom WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbDiningRooms)

                    Dim oRow As DataRow = Nothing
                    If Me.intID <= 0 Then
                        oRow = tbDiningRooms.NewRow
                        oRow("ID") = Me.GetNextIDDiningRoom()
                    ElseIf tbDiningRooms.Rows.Count = 1 Then
                        oRow = tbDiningRooms.Rows(0)
                    End If

                    oRow("Name") = Me.strName

                    If Me.intID <= 0 Then
                        tbDiningRooms.Rows.Add(oRow)
                    End If

                    da.Update(tbDiningRooms)

                    If Me.intID <= 0 Then
                        Me.intID = oRow("ID")
                    End If

                    If bAudit Then
                        strQueryRow = "@SELECT# ID, Name FROM DiningRooms WHERE ID = " & Me.intID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "DiningRooms")
                        If tbAuditNew.Rows.Count = 1 Then oDiningRoomNew = tbAuditNew.Rows(0)

                        ' Insertar registro auditoria
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Dim oAuditAction As Audit.Action = IIf(oDiningRoomOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        oState.AddAuditFieldsValues(tbParameters, oDiningRoomNew, oDiningRoomOld)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oDiningRoomNew("Name")
                        ElseIf oDiningRoomOld("Name") <> oDiningRoomNew("Name") Then
                            strObjectName = oDiningRoomOld("Name") & " -> " & oDiningRoomNew("Name")
                        Else
                            strObjectName = oDiningRoomNew("Name")
                        End If
                        oState.Audit(oAuditAction, Audit.ObjectType.tDiningRoom, strObjectName, tbParameters, -1)
                    End If

                    bolRet = True

                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roDiningRoom::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDiningRoom::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Me.oState.UpdateStateInfo()

            Try

                Dim dSet As DataSet = roDiningRoom.GetTurnsFromDiningRoom(Me.ID, oState)
                If dSet IsNot Nothing Then
                    If dSet.Tables(0).Rows.Count = 0 Then
                        Dim strSql As String = "@DELETE# DiningRooms WHERE ID = " & Me.ID
                        bolRet = ExecuteSql(strSql)

                        If bolRet And bAudit Then
                            ' Auditamos borrado horario
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{DiningRoom}", Me.Name, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDiningRoom, Me.Name, tbParameters, -1)
                        End If
                    Else
                        oState.Result = DiningRoomResultEnum.DiningRoomNotEmpty
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDiningRoom::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoom::Delete")
            End Try

            Return bolRet

        End Function

        Private Function GetNextIDDiningRoom() As Integer

            ' Busca el siguiente ID de comedor.
            Dim intRet As Integer = 1

            Dim strQuery As String = " @SELECT# Max(ID) as Contador From DiningRooms "
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item("Contador")) Then
                    intRet = tb.Rows(0).Item("Contador") + 1
                End If
            End If

            Return intRet

        End Function

        ''' <summary>
        ''' Devuelve un dataset con los turnos que pertenecen al comedor pasado por parámetro
        ''' </summary>
        ''' <param name="intIDDiningRoom">ID del comedor a recuperar los turnos</param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTurnsFromDiningRoom(ByVal intIDDiningRoom As Integer, ByRef oState As roDiningRoomState) As System.Data.DataSet
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# * from DiningRoomsTurns WHERE IDDiningRoom = " & intIDDiningRoom & " ORDER BY Name"
                oRet = CreateDataSet(strQuery)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoom::GetTurnsFromDiningRoom")
            End Try

            Return oRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un dataset con los comedores disponibles
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDiningRooms(ByRef oState As roDiningRoomState) As System.Data.DataSet
            Dim oDataset As System.Data.DataSet

            Try
                Dim strQuery As String = "@SELECT# * From DiningRooms"
                oDataset = CreateDataSet(strQuery, "DiningRooms")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoom::GetDiningRooms")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoom::GetDiningRooms")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

#End Region

    End Class

End Namespace