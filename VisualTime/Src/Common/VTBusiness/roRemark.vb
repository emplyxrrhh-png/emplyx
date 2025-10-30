Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Shift

    <XmlType(Namespace:="http://localhost", TypeName:="roShiftRemark")>
    <DataContract>
    Public Class roRemark

#Region "Declarations - Constructors"

        Private oState As roShiftState

        Private intID As Integer
        Private strText As String

        Public Sub New()
            Me.oState = New roShiftState
            Me.intID = -1
            Me.strText = ""
        End Sub

        Public Sub New(ByVal _State As roShiftState)
            Me.oState = _State
            Me.intID = -1
            Me.strText = ""
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roShiftState)
            Me.oState = _State
            Me.ID = _ID
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roShiftState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value

            End Set
        End Property
        <DataMember>
        Public Property Text() As String
            Get
                Return Me.strText
            End Get
            Set(ByVal value As String)
                Me.strText = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim rd As DbDataReader = Nothing

            Try

                Dim strSQL As String = "@SELECT# Text FROM sysroRemarks " &
                                       "WHERE ID = " & Me.ID.ToString

                Dim tbIDs As DataTable = CreateDataTable(strSQL, )
                If tbIDs IsNot Nothing AndAlso tbIDs.Rows.Count > 0 Then
                    Me.strText = tbIDs.Rows(0)("Text")
                End If

                bolRet = True

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Text}", Me.Text, "", 1)

                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tRemark, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRemark:Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRemark:Load")
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal oldRemark As String = Nothing, Optional ByVal dateRemark As String = Nothing, Optional ByVal employeeRemark As String = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Dim rd As DbDataReader = Nothing
            Try

                If Not DataLayer.roSupport.IsXSSSafe(Me.strText) Then
                    oState.Result = ShiftResultEnum.XSSvalidationError
                    Return False
                End If


                Dim tb As New DataTable("sysroRemarks")

                Dim strSQL As String = ""

                If oldRemark IsNot Nothing AndAlso oldRemark <> "" Then
                    strSQL = "@SELECT# * FROM sysroRemarks WHERE ID = " & oldRemark.ToString
                Else
                    strSQL = "@SELECT# * FROM sysroRemarks WHERE ID = " & Me.ID.ToString
                End If

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("ID") = Me.NewID()
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("Text") = Me.strText

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                oAuditDataNew = oRow

                Me.intID = oRow("ID")

                bolRet = True

                ' Auditamos
                If bAudit Then
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()

                    employeeRemark = IIf(employeeRemark = Nothing, "", employeeRemark)
                    dateRemark = IIf(dateRemark = Nothing, "", dateRemark)

                    oState.AddAuditParameter(tbAuditParameters, "{EmployeeName}", employeeRemark, "", 1)
                    oState.AddAuditParameter(tbAuditParameters, "{Date}", dateRemark, "", 1)

                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String = ""
                    If oAuditAction = Audit.Action.aInsert Then
                        strObjectName = oAuditDataNew("Text")
                    Else
                        strObjectName = oAuditDataOld("Text") & " -> " & oAuditDataNew("Text")
                    End If
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tRemark, strObjectName, tbAuditParameters, -1)

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRemark:Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRemark:Save")
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim rd As DbDataReader = Nothing

            Try

                Dim strSQL As String = "@DELETE# FROM sysroRemarks WHERE ID = " & Me.ID.ToString
                bolRet = ExecuteSql(strSQL)

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Text}", Me.Text, "", 1)

                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tRemark, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRemark:Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRemark:Delete")
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

            Return bolRet

        End Function

        Private Function NewID() As Integer

            Dim intRet As Integer = 0

            Dim rd As DbDataReader = Nothing

            Try

                Dim strSQL As String = "@SELECT# MAX(ID) FROM sysroRemarks"

                Dim tbIDs As DataTable = CreateDataTable(strSQL, )
                If tbIDs IsNot Nothing AndAlso tbIDs.Rows.Count > 0 Then
                    If Not IsDBNull(tbIDs.Rows(0)(0)) Then intRet = roTypes.Any2Integer(tbIDs.Rows(0)(0))
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRemark:NewID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRemark:NewID")
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

            Return intRet + 1

        End Function

#End Region

    End Class

End Namespace