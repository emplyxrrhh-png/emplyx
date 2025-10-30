Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Move

    <DataContract()>
    Public Class roEntry

#Region "Declarations - Constructor"

        Private oState As roMoveState

        Private intID As Integer
        Private xDateTime As DateTime
        Private lngIDCard As Long
        Private intIDReader As Integer
        Private strType As Char
        Private intIDCause As Integer
        Private bolInvalidRead As Boolean
        Private bolUsesIdEmployee As Boolean
        Private bRdr As Nullable(Of Byte)
        Private oCapture As Image

        Public Sub New()
            Me.oState = New roMoveState(-1)
            Me.ID = 0
        End Sub

        Public Sub New(ByVal _State As roMoveState)
            Me.oState = _State
            Me.ID = 0
        End Sub

        Public Sub New(ByVal _IDEntrie As Integer, ByVal _State As roMoveState)
            Me.oState = _State
            Me.intID = _IDEntrie
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roMoveState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roMoveState)
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
                Me.Load()
            End Set
        End Property
        <DataMember()>
        Public Property _DateTime() As DateTime
            Get
                Return Me.xDateTime
            End Get
            Set(ByVal value As DateTime)
                Me.xDateTime = value
            End Set
        End Property
        <DataMember()>
        Public Property IDCard() As Long
            Get
                Return Me.lngIDCard
            End Get
            Set(ByVal value As Long)
                Me.lngIDCard = value
            End Set
        End Property
        <DataMember()>
        Public Property IDReader() As Integer
            Get
                Return Me.intIDReader
            End Get
            Set(ByVal value As Integer)
                Me.intIDReader = value
            End Set
        End Property
        <DataMember()>
        Public Property Type() As Char
            Get
                Return Me.strType
            End Get
            Set(ByVal value As Char)
                Me.strType = value
            End Set
        End Property
        <DataMember()>
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property
        <DataMember()>
        Public Property UsesIdEmployee() As Boolean
            Get
                Return Me.bolUsesIdEmployee
            End Get
            Set(ByVal value As Boolean)
                Me.bolUsesIdEmployee = value
            End Set
        End Property
        <DataMember()>
        Public Property InvalidRead() As Boolean
            Get
                Return Me.bolInvalidRead
            End Get
            Set(ByVal value As Boolean)
                Me.bolInvalidRead = value
            End Set
        End Property

        <DataMember()>
        Public Property Rdr() As Nullable(Of Byte)
            Get
                Return Me.bRdr
            End Get
            Set(ByVal value As Nullable(Of Byte))
                Me.bRdr = value
            End Set
        End Property

        <XmlIgnore()>
        Public Property Capture() As Image
            Get
                Return Me.oCapture
            End Get
            Set(ByVal value As Image)
                Me.oCapture = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            If Me.intID <= 0 Then

                Me.bRdr = Nothing
                Me.oCapture = Nothing
            Else
                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM Entries WHERE [ID] = " & Me.ID.ToString)
                    If tb.Rows.Count > 0 Then

                        With tb.Rows(0)
                            Me.xDateTime = .Item("DateTime")
                            Me.lngIDCard = .Item("IDCard")
                            Me.intIDReader = .Item("IDReader")
                            Me.strType = .Item("Type")
                            Me.intIDCause = .Item("IDCause")
                            Me.bolInvalidRead = .Item("InvalidRead")
                            Me.bolUsesIdEmployee = .Item("UsesIdEmployee")
                            If Not IsDBNull(.Item("Rdr")) Then
                                Me.bRdr = .Item("Rdr")
                            Else
                                Me.bRdr = Nothing
                            End If

                        End With

                        Me.oCapture = Nothing

                    End If
                    tb = CreateDataTable("@SELECT# * FROM EntriesCaptures WHERE IDEntry = " & Me.intID.ToString)
                    If tb.Rows.Count = 1 Then

                        If Not IsDBNull(tb.Rows(0).Item("Capture")) Then
                            Dim bImage As Byte() = CType(tb.Rows(0).Item("Capture"), Byte())
                            Dim ms As MemoryStream = New MemoryStream(bImage)
                            Me.oCapture = CType(Image.FromStream(ms), Bitmap)
                        End If

                    End If

                    bolRet = True

                    ' Auditar lectura
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tEntry, "", tbParameters, -1)
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roEntry::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roEntry::Load")
                Finally

                End Try

            End If

            Return bolRet

        End Function

        Public Function Save() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim tbEntrie As New DataTable("Entries")
                Dim strSQL As String = "@SELECT# * FROM Entries WHERE [ID] = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbEntrie)

                Dim oRow As DataRow = Nothing
                If tbEntrie.Rows.Count = 0 Then
                    oRow = tbEntrie.NewRow
                    tbEntrie.Rows.Add(oRow)
                ElseIf tbEntrie.Rows.Count = 1 Then
                    oRow = tbEntrie.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("DateTime") = Me.xDateTime
                oRow("IDCard") = Me.lngIDCard
                oRow("IDReader") = Me.intIDReader
                oRow("Type") = Me.strType
                oRow("IDCause") = Me.intIDCause
                oRow("InvalidRead") = Me.bolInvalidRead
                oRow("UsesIdEmployee") = Me.bolUsesIdEmployee

                If Me.bRdr.HasValue Then
                    oRow("Rdr") = Me.bRdr.Value
                Else
                    oRow("Rdr") = DBNull.Value
                End If

                da.Update(tbEntrie)

                If Me.intID <= 0 Then
                    Dim tb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM Entries " &
                                                          "ORDER BY [ID] DESC")
                    If tb.Rows.Count = 1 Then
                        Me.intID = tb.Rows(0).Item("ID")
                    End If
                End If

                ' Grabar imagen
                ''bolRet = Me.SaveCapture(oCn)

                bolRet = True

                oAuditDataNew = oRow

                If bolRet Then
                    bolRet = False
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String = ""
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tEntry, strObjectName, tbAuditParameters, -1)
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roEntry::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEntry::Save")
            End Try

            Return bolRet

        End Function

        Private Function SaveCapture() As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Eliminar imágenes antiguas
                ''Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                ''Dim intDays As Integer = oSettings.GetVTSetting(eKeys.AuditDays)
                ''Dim xDate As DateTime = Now
                ''xDate = xDate.AddDays(-intDays)
                ''Dim strSQL As String = _
                ''    "@DELETE# FROM MovesCaptures " & _
                ''    "WHERE IDMove IN (@SELECT# [ID] FROM Moves " & _
                ''                     "WHERE ISNULL(InDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & " AND " & _
                ''                           "ISNULL(OutDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & ")"
                ''ExecuteSql(strSQL)

                Dim tbCaptures As New DataTable("EntriesCaptures")
                Dim strSQL As String = "@SELECT# * FROM EntriesCaptures WHERE IDEntry = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbCaptures)

                Dim oRow As DataRow = Nothing
                Dim bolNewRow As Boolean = False
                If tbCaptures.Rows.Count = 0 Then
                    oRow = tbCaptures.NewRow
                    bolNewRow = True
                    oRow("IDEntry") = Me.intID
                Else
                    oRow = tbCaptures.Rows(0)
                End If

                If Me.oCapture IsNot Nothing Then
                    oRow("Capture") = roTypes.Image2Bytes(Me.oCapture)
                Else
                    oRow("Capture") = DBNull.Value
                End If

                If bolNewRow Then
                    tbCaptures.Rows.Add(oRow)
                End If

                da.Update(tbCaptures)

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roEntry::SaveCapture")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEntry::SaveCapture")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sSql As String = "@DELETE# FROM EntriesCaptures WHERE IDEntry = " & Me.intID.ToString
                bolRet = ExecuteSql(sSql)

                If bolRet Then

                    sSql = "@DELETE# FROM Entries WHERE [ID] = " & Me.intID.ToString
                    bolRet = ExecuteSql(sSql)
                    If bolRet Then
                        Me.intID = 0
                    End If

                End If

                If bolRet Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEntry, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEntry::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEntry::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetInvalidEntries(ByVal _State As roMoveState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM Entries " &
                                       "WHERE InvalidRead = 1 " &
                                       "ORDER BY [ID]"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roMoves::GetInvalidEntries")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roMove::GetInvalidEntries")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveEntries(ByVal tbEntries As DataTable, ByVal _State As roMoveState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oEntry As roEntry
                Dim bolSaved As Boolean = True

                bolRet = True

                For Each oRow As DataRow In tbEntries.Rows

                    bolSaved = True

                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified

                            oEntry = New roEntry(roTypes.Any2Integer(oRow("ID")), _State)
                            With oEntry
                                ._DateTime = oRow("DateTime")
                                .IDCard = oRow("IDCard")
                                .IDReader = oRow("IDReader")
                                .Type = oRow("Type")
                                .IDCause = oRow("IDCause")
                                .InvalidRead = oRow("InvalidRead")
                                .Rdr = IIf(Not IsDBNull(oRow("Rdr")), oRow("Rdr"), Nothing)

                            End With
                            bolRet = oEntry.Save()

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                            oEntry = New roEntry(oRow("ID"), _State)
                            bolRet = oEntry.Delete()

                        Case Else
                            bolRet = True
                            bolSaved = False

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If

                Next

                If bolRet Then
                    roConnector.InitTask(TasksType.ENTRIES)
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roEntry::SaveEntries")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roEntry::SaveEntries")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace