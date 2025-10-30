Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Cause

    Public Class roCauseDocument
        Implements IEquatable(Of roCauseDocument)

#Region "Declarations - Constructor"

        Private oState As roCauseState

        Private intID As Integer
        Private intIDCause As Integer
        Private intIDLabAgree As Integer
        Private intIDDocument As Integer
        Private strLabAgreeName As String
        Private strDocumentName As String
        Private eTypeRequest As TypeRequestEnum
        Private intNumberItems As Integer
        Private intNumberItems2 As Integer
        Private intFlexibleWhen As Integer
        Private intFlexibleWhen2 As Integer

        Public Sub New()
            Me.oState = New roCauseState
            Me.intID = -1
            Me.intIDCause = 0
            Me.intIDLabAgree = 0
            Me.intIDDocument = 0
            Me.strLabAgreeName = String.Empty
            Me.strDocumentName = String.Empty
            Me.eTypeRequest = TypeRequestEnum.AtBegin
            Me.intNumberItems = -1
            Me.intNumberItems2 = -1
            Me.intFlexibleWhen = -1
            Me.intFlexibleWhen2 = -1
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal IDCause As Integer, ByVal IDLabAgree As Integer, ByVal IDDocument As Integer,
                       ByVal LabAgreeName As String, ByVal DocumentName As String, ByVal strXmlParameters As String)
            Me.oState = New roCauseState
            Me.intID = ID
            Me.intIDCause = IDCause
            Me.intIDLabAgree = IDLabAgree
            Me.intIDDocument = IDDocument
            Me.strLabAgreeName = LabAgreeName
            Me.strDocumentName = DocumentName
            Try
                Dim oCollection As New roCollection(strXmlParameters)
                Me.eTypeRequest = oCollection("TypeRequest")
                Me.intNumberItems = oCollection("NumberItems")
                Me.intNumberItems2 = oCollection("NumberItems2")
                Me.intFlexibleWhen = oCollection("FlexibleWhen")
                Me.intFlexibleWhen2 = oCollection("FlexibleWhen2")
            Catch
                Me.eTypeRequest = TypeRequestEnum.AtBegin
                Me.intNumberItems = -1
                Me.intNumberItems2 = -1
                Me.intFlexibleWhen = -1
                Me.intFlexibleWhen2 = -1
            End Try
        End Sub

#End Region

#Region "Properties"

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
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property IDLabAgree() As Integer
            Get
                Return Me.intIDLabAgree
            End Get
            Set(ByVal value As Integer)
                Me.intIDLabAgree = value
            End Set
        End Property

        <DataMember>
        Public Property IDdocument() As Integer
            Get
                Return Me.intIDDocument
            End Get
            Set(ByVal value As Integer)
                Me.intIDDocument = value
            End Set
        End Property

        <DataMember>
        Public Property TypeRequest() As TypeRequestEnum
            Get
                Return Me.eTypeRequest
            End Get
            Set(ByVal value As TypeRequestEnum)
                Me.eTypeRequest = value
            End Set
        End Property

        <DataMember>
        Public Property NumberItems() As Integer
            Get
                Return Me.intNumberItems
            End Get
            Set(ByVal value As Integer)
                Me.intNumberItems = value
            End Set
        End Property

        <DataMember>
        Public Property NumberItems2() As Integer
            Get
                Return Me.intNumberItems2
            End Get
            Set(ByVal value As Integer)
                Me.intNumberItems2 = value
            End Set
        End Property

        <DataMember>
        Public Property FlexibleWhen() As Integer
            Get
                Return Me.intFlexibleWhen
            End Get
            Set(ByVal value As Integer)
                Me.intFlexibleWhen = value
            End Set
        End Property

        <DataMember>
        Public Property FlexibleWhen2() As Integer
            Get
                Return Me.intFlexibleWhen2
            End Get
            Set(ByVal value As Integer)
                Me.intFlexibleWhen2 = value
            End Set
        End Property

        <DataMember>
        Public Property LabAgreeName() As String
            Get
                Return Me.strLabAgreeName
            End Get
            Set(ByVal value As String)
                Me.strLabAgreeName = value
            End Set
        End Property

        <DataMember>
        Public Property DocumentName() As String
            Get
                Return Me.strDocumentName
            End Get
            Set(ByVal value As String)
                Me.strDocumentName = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Save() As Boolean
            Dim bRet As Boolean = False

            Try
                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable()
                Dim strSQL As String = "@SELECT# * FROM CausesDocumentsTracking WHERE ID = " & Me.intID
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

                oRow("IDCause") = Me.intIDCause
                oRow("IDLabAgree") = Me.intIDLabAgree
                oRow("IDDocument") = Me.intIDDocument

                Dim strSQL2 As String

                Select Case Me.eTypeRequest
                    Case TypeRequestEnum.AtBegin
                        oRow("DaysAfterAbsenceBegin") = 0
                        oRow("DaysAfterAbsenceEnd") = DBNull.Value
                        oRow("Every") = DBNull.Value
                        strSQL2 = "@UPDATE# DocumentTemplates set DefaultExpiration = '0', LeaveDocType = " & Base.DTOs.LeaveDocumentType.LeaveReport & " where id = " & Me.intIDDocument & " and scope = 3"
                        bRet = ExecuteSql(strSQL2)
                    Case TypeRequestEnum.AtEnd
                        oRow("DaysAfterAbsenceBegin") = DBNull.Value
                        oRow("DaysAfterAbsenceEnd") = 0
                        oRow("Every") = DBNull.Value
                        strSQL2 = "@UPDATE# DocumentTemplates set DefaultExpiration = '0', LeaveDocType = " & Base.DTOs.LeaveDocumentType.ReturnReport & " where id = " & Me.intIDDocument & " and scope = 3"
                        bRet = ExecuteSql(strSQL2)
                    Case TypeRequestEnum.EveryFlexible1
                        Select Case FlexibleWhen
                            Case 0 'X días desde el principio
                                oRow("DaysAfterAbsenceBegin") = Me.intNumberItems
                                oRow("DaysAfterAbsenceEnd") = DBNull.Value
                                oRow("Every") = DBNull.Value
                                strSQL2 = "@UPDATE# DocumentTemplates set DefaultExpiration = '0', LeaveDocType = " & Base.DTOs.LeaveDocumentType.LeaveReport & " where id = " & Me.intIDDocument & " and scope = 3"
                                bRet = ExecuteSql(strSQL2)
                            Case 1 'X días desde el final
                                oRow("DaysAfterAbsenceBegin") = DBNull.Value
                                oRow("DaysAfterAbsenceEnd") = Me.intNumberItems
                                oRow("Every") = DBNull.Value
                                strSQL2 = "@UPDATE# DocumentTemplates set DefaultExpiration = '0', LeaveDocType = " & Base.DTOs.LeaveDocumentType.ReturnReport & " where id = " & Me.intIDDocument & " and scope = 3"
                                bRet = ExecuteSql(strSQL2)
                        End Select
                    Case TypeRequestEnum.EveryFlexible2
                        ' Documentos periódicos -> Partes de confirmación
                        oRow("DaysAfterAbsenceBegin") = Me.intNumberItems2
                        oRow("DaysAfterAbsenceEnd") = DBNull.Value
                        oRow("Every") = Me.intNumberItems.ToString()
                        ' Guardo la periodicidad en la definición de la plantilla
                        strSQL2 = "@UPDATE# DocumentTemplates set DefaultExpiration = '1@" & Me.intNumberItems.ToString & "', LeaveDocType = " & Base.DTOs.LeaveDocumentType.ConfirmationReport & " where id = " & Me.intIDDocument & " and scope = 3"
                        bRet = ExecuteSql(strSQL2)
                End Select

                If bRet Then
                    Dim oCollection As New roCollection()
                    oCollection.Add("TypeRequest", Me.eTypeRequest)
                    oCollection.Add("NumberItems", Me.intNumberItems)
                    oCollection.Add("NumberItems2", Me.intNumberItems2)
                    oCollection.Add("FlexibleWhen", Me.intFlexibleWhen)
                    oCollection.Add("FlexibleWhen2", Me.intFlexibleWhen2)

                    oRow("Parameters") = oCollection.XML()

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::Save")
            End Try

            Return bRet

        End Function

        Public Function Delete() As Boolean
            Dim bRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim SQL As String = "@DELETE# FROM CausesDocumentsTracking WHERE ID = " & Me.intID
                bRet = ExecuteSql(SQL)

                If bRet Then
                    bRet = UpdateLeaveDocumentTypeIfNeeded()
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
            End Try

            Return bRet

        End Function

        Public Function Equals1(ByVal other As roCauseDocument) As Boolean Implements System.IEquatable(Of roCauseDocument).Equals
            Return Me.intID = other.intID
        End Function

        Private Function UpdateLeaveDocumentTypeIfNeeded() As Boolean
            Dim bRet As Boolean = True

            Try
                Dim SQL As String = "@SELECT# count(*) FROM CausesDocumentsTracking WHERE ID <> " & Me.intID & " AND IDDocument = " & Me.IDdocument.ToString
                If ExecuteScalar(SQL) = 0 Then
                    SQL = "@UPDATE# documenttemplates set leavedoctype = " & DTOs.LeaveDocumentType.NotDefined & " where id = " & Me.IDdocument.ToString & " and scope = " & DTOs.DocumentScope.LeaveOrPermission
                    bRet = ExecuteSql(SQL)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::UpdateLeaveDocumentTypeIfNeeded")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCauseDocument::UpdateLeaveDocumentTypeIfNeeded")
            End Try

            Return bRet
        End Function

#End Region

#Region "Helper Methods"

        Public Shared Function GetDocumentsByIdCause(ByVal IDCause As Integer, ByRef oState As roCauseState) As Generic.List(Of roCauseDocument)
            Dim lst As New Generic.List(Of roCauseDocument)

            Try

                Dim oCauseDocument As roCauseDocument

                Dim SQL As String = "@SELECT# CausesDocumentsTracking.ID, CausesDocumentsTracking.IDCause, CausesDocumentsTracking.IDLabAgree, CausesDocumentsTracking.IDDocument, CausesDocumentsTracking.Parameters,  " &
                                    "DocumentTemplates.Name AS DocumentName, LabAgree.Name AS LabAgreeName FROM CausesDocumentsTracking LEFT OUTER JOIN DocumentTemplates ON  " &
                                    "CausesDocumentsTracking.IDDocument = DocumentTemplates.ID LEFT OUTER JOIN LabAgree ON CausesDocumentsTracking.IDLabAgree = LabAgree.ID WHERE IDCause = " & IDCause &
                                    "ORDER BY DocumentTemplates.Name, LabAgree.Name "

                Dim tb As DataTable = CreateDataTable(SQL, "Docs")
                For Each oRow As DataRow In tb.Rows
                    oCauseDocument = New roCauseDocument(roTypes.Any2Integer(oRow("ID")), roTypes.Any2Integer(oRow("IDCause")), roTypes.Any2Integer(oRow("IDLabAgree")),
                                                         roTypes.Any2Integer(oRow("IDDocument")), roTypes.Any2String(oRow("LabAgreeName")), roTypes.Any2String(oRow("DocumentName")),
                                                         roTypes.Any2String(oRow("Parameters")))
                    lst.Add(oCauseDocument)
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseDocument::GetDocumentsByIdCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseDocument::GetDocumentsByIdCause")
            Finally

            End Try

            Return lst

        End Function

        Public Shared Function SaveDocumentsByIdCause(ByVal IdCause As Integer, ByRef lstDocuments As Generic.List(Of roCauseDocument), ByRef oState As roCauseState) As Boolean
            Dim bRet As Boolean = False

            Try

                'Obtener registros de documents antes de comenzar a modificar
                Dim SQL As String = "@SELECT# * FROM CausesDocumentsTracking WHERE IDCause = " & IdCause
                Dim tbOldDocuments As DataTable = CreateDataTable(SQL)

                'Procesar lista de documentos nuevos
                Dim bolReviewList As Boolean = (lstDocuments IsNot Nothing AndAlso lstDocuments.Count > 0)
                If bolReviewList Then
                    For Each oCauseDocument As roCauseDocument In lstDocuments
                        bRet = oCauseDocument.Save()
                        If Not bRet Then Exit For
                    Next
                End If

                'Procesar lista de empleados antiguos
                If tbOldDocuments IsNot Nothing AndAlso tbOldDocuments.Rows.Count > 0 Then
                    Dim oCauseDocument As roCauseDocument
                    For Each oRow As DataRow In tbOldDocuments.Rows

                        oCauseDocument = New roCauseDocument(roTypes.Any2Integer(oRow("ID")), roTypes.Any2Integer(oRow("IDCause")), roTypes.Any2Integer(oRow("IDLabAgree")),
                                                             roTypes.Any2Integer(oRow("IDDocument")), "", "", roTypes.Any2String(oRow("Parameters")))

                        If bolReviewList Then
                            If Not lstDocuments.Contains(oCauseDocument) Then
                                bRet = oCauseDocument.Delete()
                            End If
                        Else
                            bRet = oCauseDocument.Delete()
                        End If
                        If Not bRet Then Exit For
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCauseDocument::SaveDocumentsByIdCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseDocument::SaveDocumentsByIdCause")
            End Try

            Return bRet

        End Function

        Public Shared Function DeleteDocumentsByIdCause(ByVal IDCause As Integer, ByRef oState As roCauseState) As Boolean
            Dim bRet As Boolean = False

            Try

                Dim lst As New Generic.List(Of roCauseDocument)
                lst = GetDocumentsByIdCause(IDCause, oState)
                For Each oCauseDocument As roCauseDocument In lst
                    bRet = oCauseDocument.Delete()
                    If Not bRet Then Exit For
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCauseDocument::DeleteDocumentsByIdCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseDocument::DeleteDocumentsByIdCause")
            Finally

            End Try

            Return bRet

        End Function

#End Region

    End Class

End Namespace