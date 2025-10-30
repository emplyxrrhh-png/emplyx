Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DocumentAbsence

    <DataContract()>
    Public Class roDocumentAbsenceAdvice
        Implements IEquatable(Of roDocumentAbsenceAdvice)

#Region "Declarations - Constructor"

        Private oState As roDocumentAbsenceState

        Private intID As Integer
        Private intIDDocumentAbsence As Integer
        Private strName As String
        Private strAdvice As String

        Public Sub New()
            Me.oState = New roDocumentAbsenceState
            Me.intID = -1
            Me.intIDDocumentAbsence = 0
            Me.strName = String.Empty
            Me.strAdvice = String.Empty
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal IDDocumentAbsence As Integer, ByVal Name As String, ByVal Advice As String)
            Me.intID = ID
            Me.intIDDocumentAbsence = IDDocumentAbsence
            Me.strName = Name
            Me.strAdvice = Advice
        End Sub

        Public Sub New(ByVal ID As Integer)
            Me.oState = New roDocumentAbsenceState
            Me.intID = ID
            Me.Load()
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal _State As roDocumentAbsenceState)
            Me.oState = _State
            Me.intID = ID
            Me.Load()
        End Sub

#End Region

#Region "Properties"

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
        Public Property IDDocumentAbsence() As Integer
            Get
                Return Me.intIDDocumentAbsence
            End Get
            Set(ByVal value As Integer)
                Me.intIDDocumentAbsence = value
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
        Public Property Advice() As String
            Get
                Return Me.strAdvice
            End Get
            Set(ByVal value As String)
                Me.strAdvice = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean
            Dim bRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM DocumentsAbsencesAdvices WHERE ID = " & Me.intID
                Dim tb As DataTable = CreateDataTable(strSQL, "")
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intID = roTypes.Any2Integer(oRow("ID"))
                    Me.intIDDocumentAbsence = roTypes.Any2Integer(oRow("IDDocumentAbsence"))
                    Me.strName = roTypes.Any2String(oRow("Name"))
                    Me.strAdvice = roTypes.Any2String(oRow("Advice"))

                End If

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdviceDocument::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdviceDocument::Load")
            Finally

            End Try

            Return bRet

        End Function

        Public Function Save() As Boolean
            Dim bRet As Boolean = False

            Try

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable()
                Dim strSQL As String = "@SELECT# * FROM DocumentsAbsencesAdvices WHERE ID = " & Me.intID
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("IDDocumentAbsence") = Me.intIDDocumentAbsence
                oRow("Name") = Me.strName
                oRow("Advice") = Me.strAdvice

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::Save")
            End Try

            Return bRet

        End Function

        Public Function Delete() As Boolean
            Dim bRet As Boolean = True

            Try
                Dim SQL As String = "@DELETE# FROM DocumentsAbsencesAdvices WHERE ID = " & Me.intID
                bRet = ExecuteSql(SQL)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::Delete")
            End Try

            Return bRet

        End Function

        Public Function Equals1(ByVal other As roDocumentAbsenceAdvice) As Boolean Implements System.IEquatable(Of roDocumentAbsenceAdvice).Equals
            Return Me.intID = other.intID
        End Function

#End Region

#Region "Helper Methods"

        Public Shared Function GetAdvicesByIdDocumentAbsence(ByVal IDDocumentAbsence As Integer, ByRef oState As roDocumentAbsenceState) As Generic.List(Of roDocumentAbsenceAdvice)
            Dim lst As New Generic.List(Of roDocumentAbsenceAdvice)

            Try

                Dim oDocumentAbsenceAdvice As roDocumentAbsenceAdvice

                Dim SQL As String = "@SELECT# * FROM DocumentsAbsencesAdvices WHERE IDDocumentAbsence = " & IDDocumentAbsence
                Dim tb As DataTable = CreateDataTable(SQL, )
                For Each oRow As DataRow In tb.Rows
                    oDocumentAbsenceAdvice = New roDocumentAbsenceAdvice(roTypes.Any2Integer(oRow("ID")), roTypes.Any2Integer(oRow("IDDocumentAbsence")),
                                                                         roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("Advice")))
                    lst.Add(oDocumentAbsenceAdvice)
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::GetAdvicesByIdDocumentAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::GetAdvicesByIdDocumentAbsence")
            Finally

            End Try

            Return lst

        End Function

        Public Shared Function SaveAdvicesByIdDocumentAbsence(ByVal IdDocumentAbsence As Integer, ByRef lstDocuments As Generic.List(Of roDocumentAbsenceAdvice), ByRef oState As roDocumentAbsenceState) As Boolean
            Dim bRet As Boolean = False

            Try
                'Obtener registros de documents antes de comenzar a modificar
                Dim SQL As String = "@SELECT# * FROM DocumentsAbsencesAdvices WHERE IdDocumentAbsence = " & IdDocumentAbsence
                Dim tbOldDocuments As DataTable = CreateDataTable(SQL)

                'Procesar lista de documentos nuevos
                Dim bolReviewList As Boolean = (lstDocuments IsNot Nothing AndAlso lstDocuments.Count > 0)
                If bolReviewList Then
                    For Each oDocumentAbsenceAdvice As roDocumentAbsenceAdvice In lstDocuments
                        bRet = oDocumentAbsenceAdvice.Save()
                        If Not bRet Then Exit For
                    Next
                End If

                'Procesar lista de empleados antiguos
                If tbOldDocuments IsNot Nothing AndAlso tbOldDocuments.Rows.Count > 0 Then
                    Dim oDocumentAbsenceAdvice As roDocumentAbsenceAdvice
                    For Each oRow As DataRow In tbOldDocuments.Rows

                        oDocumentAbsenceAdvice = New roDocumentAbsenceAdvice(roTypes.Any2Integer(oRow("ID")), roTypes.Any2Integer(oRow("IDDocumentAbsence")), roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("Advice")))

                        If bolReviewList Then
                            If Not lstDocuments.Contains(oDocumentAbsenceAdvice) Then
                                bRet = oDocumentAbsenceAdvice.Delete()
                            End If
                        Else
                            bRet = oDocumentAbsenceAdvice.Delete()
                        End If
                        If Not bRet Then Exit For
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::SaveAdvicesByIdDocumentAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::SaveAdvicesByIdDocumentAbsence")
            End Try

            Return bRet

        End Function

        Public Shared Function DeleteAdvicesByIdDocumentAbsence(ByVal IdDocumentAbsence As Integer, ByRef oState As roDocumentAbsenceState) As Boolean
            Dim bRet As Boolean = False

            Try

                Dim SQL As String = "@DELETE# FROM DocumentsAbsencesAdvices WHERE IdDocumentAbsence = " & IdDocumentAbsence
                bRet = ExecuteSql(SQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::DeleteAdvicesByIdDocumentAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentAbsenceAdvice::DeleteAdvicesByIdDocumentAbsence")
            Finally

            End Try

            Return bRet

        End Function

#End Region

    End Class

End Namespace