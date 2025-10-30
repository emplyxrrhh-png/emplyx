Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DocumentAbsence

    <DataContract()>
    Public Class roDocumentAbsence

#Region "Declarations - Constructor"

        Private oState As roDocumentAbsenceState

        Private intID As Integer
        Private strName As String
        Private strShortName As String
        Private strDescription As String
        Private strRememberText As String
        Private lstAdvices As Generic.List(Of roDocumentAbsenceAdvice)

        Public Sub New()
            Me.oState = New roDocumentAbsenceState()
            Me.intID = -1
            Me.strName = String.Empty
            Me.strShortName = String.Empty
            Me.strDescription = String.Empty
            Me.strRememberText = String.Empty
            Me.lstAdvices = Nothing
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roDocumentAbsenceState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.strName = String.Empty
            Me.strShortName = String.Empty
            Me.strDescription = String.Empty
            Me.strRememberText = String.Empty
            Me.lstAdvices = Nothing
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roDocumentAbsenceState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDocumentAbsenceState)
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
        Public Property RememberText() As String
            Get
                Return Me.strRememberText
            End Get
            Set(ByVal value As String)
                Me.strRememberText = value
            End Set
        End Property
        <DataMember()>
        Public Property Advices() As Generic.List(Of roDocumentAbsenceAdvice)
            Get
                Return Me.lstAdvices
            End Get
            Set(ByVal value As Generic.List(Of roDocumentAbsenceAdvice))
                Me.lstAdvices = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM DocumentsAbsences WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    If Not IsDBNull(oRow("ShortName")) Then Me.strShortName = oRow("ShortName")
                    If Not IsDBNull(oRow("Description")) Then Me.strDescription = oRow("Description")
                    If Not IsDBNull(oRow("RememberText")) Then Me.strRememberText = oRow("RememberText")
                    Me.lstAdvices = DocumentAbsence.roDocumentAbsenceAdvice.GetAdvicesByIdDocumentAbsence(Me.intID, oState)
                Else
                    Me.strName = String.Empty
                    Me.strShortName = String.Empty
                    Me.strDescription = String.Empty
                    Me.strRememberText = String.Empty
                    Me.lstAdvices = New Generic.List(Of roDocumentAbsenceAdvice)()
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tDocumentAbsence, Me.strName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDocumentAbsence::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentAbsence::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function ValidateDocument() As Boolean

            Dim strQuery As String
            Dim oDataSet As System.Data.DataSet

            ' El nombre no puede estar en blanco
            If Me.Name = "" Then
                oState.Result = DTOs.DocumentAbsenceResultEnum.InvalidName
                Return False
            End If

            ' El nombre corto no puede estar en blanco
            If Me.ShortName = "" Then
                oState.Result = DTOs.DocumentAbsenceResultEnum.InvalidShortName
                Return False
            End If

            ' El nombre no puede existir en la bdd para otro documento
            strQuery = " @SELECT# * From DocumentsAbsences "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " And name = '" & Me.Name.Replace("'", "''") & "' "

            oDataSet = CreateDataSet(strQuery)
            If oDataSet.CreateDataReader.HasRows = True Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = DTOs.DocumentAbsenceResultEnum.NameAlreadyExist
                Return False
            End If

            ' El nombre corto no puede existir en la bdd para otro documento
            strQuery = " @SELECT# * From DocumentsAbsences "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " AND ShortName = '" & Me.ShortName.Replace("'", "''") & "' "

            oDataSet = CreateDataSet(strQuery)
            If oDataSet.CreateDataReader.HasRows = True Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = DTOs.DocumentAbsenceResultEnum.ShortNameAlreadeyExist
                Return False
            End If

            Return True

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateDocument() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim bolIsNew As Boolean = False

                    Dim tb As New DataTable("DocumentAbsences")
                    Dim strSQL As String = "@SELECT# * FROM DocumentsAbsences WHERE ID = " & Me.intID.ToString
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
                    oRow("ShortName") = Me.strShortName
                    oRow("Description") = Me.strDescription
                    oRow("RememberText") = Me.strRememberText

                    'Documents
                    If Me.lstAdvices IsNot Nothing AndAlso Me.lstAdvices.Count > 0 Then

                        If bolIsNew Then
                            For Each advice As roDocumentAbsenceAdvice In Me.lstAdvices
                                advice.IDDocumentAbsence = Me.intID
                            Next
                        End If

                        roDocumentAbsenceAdvice.SaveAdvicesByIdDocumentAbsence(Me.intID, Me.lstAdvices, oState)
                    Else
                        roDocumentAbsenceAdvice.DeleteAdvicesByIdDocumentAbsence(Me.intID, oState)
                    End If

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDocumentAbsence, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            If Not Me.IsUsed() Then
                Try
                    Dim DeleteQuerys() As String = {"@DELETE# DocumentsAbsencesAdvices WHERE IDDocumentAbsence = " & Me.ID, "@DELETE# FROM DocumentsAbsences WHERE ID = " & Me.intID}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDocumentAbsence, Me.strName, Nothing, -1)
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::Delete")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::Delete")
                End Try
            Else
                bolRet = False
            End If

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

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM DocumentsAbsences "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsence::GetNextID")
            End Try

            Return intRet

        End Function

        Public Function IsUsed() As Boolean
            Dim bolRet As Boolean = False

            Dim strErrorMessage As String

            ' Miramos si la causa esta siendo usada en algun horario
            strErrorMessage = CauseUsedInAbsenceTracking()
            If strErrorMessage <> "" Then
                oState.Language.ClearUserTokens()
                'oState.Language.AddUserToken(strErrorMessage)
                oState.Result = DTOs.DocumentAbsenceResultEnum.DocumentUsedInAbsenceTracking
                oState.Language.ClearUserTokens()
                bolRet = True
            End If

            Return bolRet

        End Function

        Public Function CauseUsedInAbsenceTracking() As String
            ' Funcion que devuelve los registros de seguimimiento que usan un documento
            Dim strRet As String = ""

            Dim strQuery As String
            Dim odataset As Data.DataSet
            Dim oDataView As System.Data.DataView
            Dim DataRowView As Data.DataRowView

            strQuery = "@SELECT# AbsenceTracking.ID "
            strQuery = strQuery & " FROM AbsenceTracking"
            strQuery = strQuery & " Where IDDocument= " & Me.ID

            Try
                odataset = CreateDataSet(strQuery)
                oDataView = New Data.DataView(odataset.Tables(0))

                For Each DataRowView In oDataView
                    strRet = strRet & DataRowView("ID") & vbCrLf
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInAbsenceTracking")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCause::CauseUsedInAbsenceTracking")
            End Try

            Return strRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetDocumentAbsencesDataTable(ByVal strOrderBy As String, ByVal _State As roDocumentAbsenceState) As DataTable

            Dim tbRet As DataTable = New DataTable()

            Try

                Dim strSQL As String = "@SELECT# * FROM DocumentsAbsences ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roDocumentAbsence::GetDocumentAbsencesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDocumentAbsence::GetDocumentAbsencesDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function ExitsDocumentAbsence(ByVal IDDocumentAbsence As Integer, ByVal _State As roDocumentAbsenceState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String
                If IDDocumentAbsence = -1 Then
                    strSQL = "@SELECT# TOP 1 ID FROM DocumentsAbsences"
                Else
                    strSQL = "@SELECT# ID FROM DocumentsAbsences WHERE ID = " & IDDocumentAbsence
                End If

                Dim tb As DataTable = CreateDataTable(strSQL)
                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDocumentAbsence::ExitsDocumentAbsence")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDocumentAbsence::ExitsDocumentAbsence")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace