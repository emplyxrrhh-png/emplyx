Imports System.Data.Common
Imports System.Drawing
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Capture

    <DataContract()>
    Public Class roCapture

#Region "Declarations - Constructor"

        Private oState As roCaptureState

        Private intID As Integer
        Private dtDateTime As DateTime

        Private oImage As Byte()

        Public Sub New()
            Me.oState = New roCaptureState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roCaptureState)
            Me.oState = _State
            Me.intID = _ID
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roCaptureState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roCaptureState)
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
        Public Property [DateTime]() As DateTime
            Get
                Return Me.dtDateTime
            End Get
            Set(ByVal value As DateTime)
                Me.dtDateTime = value
            End Set
        End Property
        <DataMember()>
        Public Property Capture() As Byte()
            Get
                Return Me.oImage
            End Get
            Set(ByVal value As Byte())
                Me.oImage = value
            End Set
        End Property

        <IgnoreDataMember()>
        Public Property CaptureImage() As Image
            Get
                Dim ms As New System.IO.MemoryStream(Me.oImage)
                Return CType(Image.FromStream(ms), Bitmap)
            End Get
            Set(ByVal value As Image)
                Me.oImage = roTypes.Image2Bytes(value)
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim strSQL As String = "@SELECT# * FROM Captures " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("DateTime")) Then Me.dtDateTime = oRow("DateTime")

                    If Not IsDBNull(oRow("Capture")) Then
                        Dim bits As Byte() = CType(oRow("Capture"), Byte())
                        Me.oImage = bits
                    Else
                        Me.oImage = Nothing
                    End If
                Else

                End If

                bolRet = True

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCapture, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCapture::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCapture::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("Captures")
                Dim strSQL As String = "@SELECT# * FROM Captures WHERE ID = " & Me.intID.ToString
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

                oRow("DateTime") = Me.dtDateTime

                If Me.oImage IsNot Nothing AndAlso Me.oImage.Length > 0 Then
                    oRow("Capture") = Me.oImage
                Else
                    oRow("Capture") = DBNull.Value
                End If

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                oAuditDataNew = oRow

                bolRet = True

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCapture, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCapture::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCapture::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim DeleteQuerys() As String = {"@DELETE# FROM Captures WHERE ID = " & Me.intID.ToString}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCapture, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCapture::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCapture::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente codigo a usar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM Captures"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roCapture::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCapture::GetNextID")
            End Try

            Return intRet

        End Function

#End Region

    End Class

End Namespace