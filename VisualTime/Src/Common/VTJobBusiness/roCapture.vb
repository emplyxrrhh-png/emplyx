Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roCapture

#Region "Declarations - Constructor"

    Private oLog As roLog

    Private lngID As Long
    Private xDateTime As DateTime
    Private oCapture As Image

    Public Sub New(ByVal _ID As Long, ByVal _Log As roLog)

        Me.lngID = _ID

        Me.Load()

    End Sub

#End Region

#Region "Properties"

    Public Property ID() As Long
        Get
            Return Me.lngID
        End Get
        Set(ByVal value As Long)
            Me.lngID = value
        End Set
    End Property

    Public Property DateTime() As DateTime
        Get
            Return Me.xDateTime
        End Get
        Set(ByVal value As DateTime)
            Me.xDateTime = value
        End Set
    End Property

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

    Public Sub Load()

        Try

            Dim strSQL As String = "@SELECT# * FROM Captures WHERE [ID] = " & Me.lngID.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                Dim oRow As DataRow = tb.Rows(0)

                Me.xDateTime = oRow("DateTime")

                If Not IsDBNull(oRow("Capture")) Then
                    Dim bImage As Byte() = CType(oRow("Capture"), Byte())
                    Dim ms As MemoryStream = New MemoryStream(bImage)
                    Me.oCapture = CType(Image.FromStream(ms), Bitmap)
                End If

            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCapture::Load :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCapture::Load :", ex)
        Finally

        End Try

    End Sub

    Public Function Save() As Boolean

        Dim bolRet As Boolean = False

        Try

            ' Eliminar imágenes antiguas
            Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
            Dim intDays As Integer = oSettings.GetVTSetting(eKeys.AuditDays)
            Dim xDate As DateTime = Now
            xDate = xDate.AddDays(-intDays)

            Dim strSQL As String
            strSQL = "@UPDATE# AccessMoves SET IDCapture = NULL " &
                     "WHERE IDCapture IN (@SELECT# Captures.ID FROM Captures WHERE DateTime < " & Any2Time(xDate).SQLDateTime & ")"
            ExecuteSql(strSQL)
            strSQL = "@UPDATE# InvalidAccessMoves SET IDCapture = NULL " &
                     "WHERE IDCapture IN (@SELECT# Captures.ID FROM Captures WHERE DateTime < " & Any2Time(xDate).SQLDateTime & ")"
            ExecuteSql(strSQL)
            strSQL = "@UPDATE# InvalidMoves SET IDCapture = NULL " &
                     "WHERE IDCapture IN (@SELECT# Captures.ID FROM Captures WHERE DateTime < " & Any2Time(xDate).SQLDateTime & ")"
            ExecuteSql(strSQL)
            strSQL = "@DELETE# FROM Captures " &
                     "WHERE DateTime < " & Any2Time(xDate).SQLDateTime
            ExecuteSql(strSQL)

            Dim tbCaptures As New DataTable("Captures")
            strSQL = "@SELECT# * FROM Captures WHERE ID = " & Me.lngID.ToString
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbCaptures)

            Dim oRow As DataRow = Nothing
            Dim bolNewRow As Boolean = False
            If tbCaptures.Rows.Count = 0 Then
                oRow = tbCaptures.NewRow
                bolNewRow = True
                Me.lngID = Me.GetNextID()
                oRow("ID") = Me.lngID
            Else
                oRow = tbCaptures.Rows(0)
            End If

            oRow("DateTime") = Me.xDateTime

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
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCapture::Save :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCapture::Save :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Private Function GetNextID() As Integer

        Dim lngRet As Long = 1

        Try

            Dim strSQL As String = "@SELECT# MAX(ID) FROM Captures"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item(0)) Then
                    lngRet = CLng(tb.Rows(0).Item(0)) + 1
                End If
            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCapture::GetNextID :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCapture::GetNextID :", ex)
        Finally

        End Try

        Return lngRet
    End Function

#End Region

End Class