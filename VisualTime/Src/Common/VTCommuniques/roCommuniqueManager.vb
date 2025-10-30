Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTCommuniques

    Public Class roCommuniqueManager

        Private oState As roCommuniqueState = Nothing

        Public ReadOnly Property State As roCommuniqueState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roCommuniqueState()
        End Sub

        Public Sub New(ByVal _State As roCommuniqueState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        Public Function CreateOrUpdateCommunique(ByRef oCommunique As roCommunique, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Dim bIsNew As Boolean = False
            Dim iCommitedCommuniques As Integer = 0
            Dim oExistingCommunique As roCommunique = Nothing

            Try
                oState.Result = CommuniqueResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(oCommunique) Then
                    oState.Result = CommuniqueResultEnum.XSSvalidationError
                    Return False
                End If

                If ValidateCommunique(oCommunique) Then
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    ' 0.- Guardamos comunicado
                    Dim tb As New DataTable("Communiques")
                    Dim strSQL As String = "@SELECT# * FROM Communiques WHERE Id = " & oCommunique.Id.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If (tb.Rows.Count.Equals(0)) Then
                        bIsNew = True
                        oRow = tb.NewRow
                    Else
                        oRow = tb.Rows(0)
                        oExistingCommunique = LoadCommunique(roTypes.Any2Integer(oRow("Id")), False)
                    End If

                    oRow("IdCompany") = If(oCommunique.IdCompany > 0, oCommunique.IdCompany, System.DBNull.Value)
                    oRow("Subject") = oCommunique.Subject
                    oRow("IdCreatedBy") = Me.oState.IDPassport
                    If bIsNew Then oRow("CreatedOn") = Date.Now
                    oRow("Mandatory") = roTypes.Any2Boolean(oCommunique.MandatoryRead)
                    oRow("Message") = oCommunique.Message
                    oRow("AllowedResponses") = If(oCommunique.AllowedResponses.Length > 0, String.Join("~", oCommunique.AllowedResponses), System.DBNull.Value)
                    oRow("AllowChangeResponse") = roTypes.Any2Boolean(oCommunique.AllowChangeResponse)
                    oRow("ResponseLimitPercentage") = oCommunique.ResponsePercentageLimit
                    oRow("ExpirationDate") = oCommunique.ExpirationDate
                    oRow("PlanificationDate") = oCommunique.PlanificationDate
                    oRow("Status") = oCommunique.Status
                    oRow("Archived") = oCommunique.Archived
                    If oCommunique.Status = CommuniqueStatusEnum.Cancelled Then
                        oRow("Archived") = True
                    End If
                    If Not bIsNew AndAlso oExistingCommunique IsNot Nothing AndAlso oExistingCommunique.Status = CommuniqueStatusEnum.Draft AndAlso oCommunique.Status = CommuniqueStatusEnum.Online Then
                        oRow("SentOn") = Date.Now
                    End If

                    If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                    iCommitedCommuniques = da.Update(tb)
                    If bIsNew Then
                        oCommunique.Id = ExecuteScalar("@SELECT# IDENT_CURRENT('Communiques')")
                    End If

                    bolRet = (iCommitedCommuniques >= 1)

                    If bolRet Then
                        ' 1.- Guardamos empleados destinatarios
                        If Not bIsNew Then
                            strSQL = "@DELETE# CommuniqueEmployees WHERE IdCommunique = " & oCommunique.Id.ToString
                            bolRet = ExecuteSql(strSQL)
                        End If

                        If bolRet AndAlso oCommunique.Employees.Length > 0 Then
                            For Each idEmployee As Integer In oCommunique.Employees
                                strSQL = "@INSERT# INTO CommuniqueEmployees (IdCommunique, IdEmployee) VALUES (" & oCommunique.Id.ToString & "," & idEmployee.ToString & ")"
                                bolRet = ExecuteSql(strSQL)
                                If Not bolRet Then
                                    oState.Result = CommuniqueResultEnum.ErrorSettingEmployees
                                    Exit For
                                End If
                            Next
                        End If

                        ' 2.- Guardamos grupos destino
                        If Not bIsNew Then
                            strSQL = "@DELETE# CommuniqueGroups WHERE IdCommunique = " & oCommunique.Id.ToString
                            bolRet = ExecuteSql(strSQL)
                        End If

                        If bolRet AndAlso oCommunique.Groups.Length > 0 Then
                            For Each idGroup As Integer In oCommunique.Groups
                                strSQL = "@INSERT# INTO CommuniqueGroups (IdCommunique, IdGroup) VALUES (" & oCommunique.Id.ToString & "," & idGroup.ToString & ")"
                                bolRet = ExecuteSql(strSQL)
                                If Not bolRet Then
                                    oState.Result = CommuniqueResultEnum.ErrorSettingGroups
                                    Exit For
                                End If
                            Next
                        End If
                        If oCommunique.Status = CommuniqueStatusEnum.Online AndAlso Not oCommunique.Archived AndAlso bolRet AndAlso (oCommunique.Employees.Length > 0 OrElse oCommunique.Groups.Length > 0) Then
                            If oCommunique.PlanificationDate = DateSerial(2000, 1, 1) Then
                                Dim sSelector As String = roSelector.BuildSelectionStringFromIDs(oCommunique.Employees, oCommunique.Groups)

                                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(Me.State.IDPassport, "Employees", "U", Permission.Read, sSelector, "11110", "", False, Nothing, Nothing)
                                For Each idEmployee As Integer In lstEmployees

                                    VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(idEmployee, LoadType.Employee, Me.oState.Language.Translate("Push.CommuniqueSubject", "", False), Me.oState.Language.Translate("Push.CommuniqueBody", "", False))


                                Next
                            Else
                                strSQL = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 4991"
                                Dim isActive As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                                If isActive Then
                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Parameters, Key3DateTime ) VALUES " &
    "(4991, " & oCommunique.Id & " ,'" & "" & "@" & "" & "'," & roTypes.Any2Time(oCommunique.PlanificationDate).SQLDateTime & ")"
                                    bolRet = ExecuteSql(strSQL)
                                End If
                            End If
                        End If

                        ' 3.- Guardamos documentos en gestor documental
                        If bolRet Then
                            If Not bIsNew Then
                                ' 3.1.- Borro documentos que pudiera tener asociados
                                strSQL = "@DELETE# Documents WHERE IdCommunique = " & oCommunique.Id.ToString
                                bolRet = ExecuteSql(strSQL)
                            End If

                            ' 3.2.- Guardo documentos que pudiera tener asociados
                            If bolRet AndAlso oCommunique.Documents IsNot Nothing AndAlso oCommunique.Documents.Length > 0 Then
                                Dim oDocumentState As New VTDocuments.roDocumentState(Me.oState.IDPassport)
                                Dim oDocumentManager As New VTDocuments.roDocumentManager(oDocumentState)

                                For Each oDocument As DTOs.roDocument In oCommunique.Documents
                                    oDocument.IdCommunique = oCommunique.Id
                                    bolRet = oDocumentManager.SaveDocument(oDocument)
                                    If Not bolRet Then
                                        oState.Result = CommuniqueResultEnum.ErrorSavingDocuments
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If

                    If bolRet AndAlso bAudit Then
                        ' Auditamos
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tCommunique, oCommunique.Subject, tbParameters, -1)
                    End If
                Else
                    oState.Result = CommuniqueResultEnum.SubjectRequired
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::CreateOrUpdateCommunique")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::CreateOrUpdateCommunique")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Public Function DeleteCommunique(ByVal idCommunique As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Dim strSQL As String = String.Empty
            Dim oCommunique As roCommunique = Nothing

            Try
                oState.Result = CommuniqueResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                oCommunique = LoadCommunique(idCommunique, False)

                ' No se puede borrar un comunicado que haya sido leído por algún empleado
                Dim oCommuniqueWithStatistics As roCommuniqueWithStatistics = Nothing
                oCommuniqueWithStatistics = Me.GetCommuniqueWithStatistics(idCommunique)

                If oCommuniqueWithStatistics.EmployeeCommuniqueStatus.ToList.Find(Function(x) x.Read = True) Is Nothing Then
                    ' 1.- Borramos destinatarios
                    strSQL = "@DELETE# CommuniqueEmployees WHERE IdCommunique = " & oCommunique.Id.ToString
                    bolRet = ExecuteSql(strSQL)

                    ' 2.- Borramos grupos destino
                    If bolRet Then
                        strSQL = "@DELETE# CommuniqueGroups WHERE IdCommunique = " & oCommunique.Id.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If

                    ' 3.- Borramos documentos en gestor documental
                    If bolRet Then
                        ' 3.1.- Borro documentos que pudiera tener asociados
                        strSQL = "@DELETE# Documents WHERE IdCommunique = " & oCommunique.Id.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If

                    ' 4.- Borramos Communique
                    If bolRet Then
                        ' 3.1.- Borro documentos que pudiera tener asociados
                        strSQL = "@DELETE# Communiques WHERE Id = " & oCommunique.Id.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If

                    If bolRet AndAlso bAudit Then
                        ' Auditamos
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCommunique, oCommunique.Subject, tbParameters, -1)
                    End If
                Else
                    bolRet = False
                    oState.Result = CommuniqueResultEnum.CanNotDeleteReadCommuniques
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Private Function ValidateCommunique(oCommunique As roCommunique) As Boolean
            Dim oRet As Boolean = False
            Try
                Return Not oCommunique Is Nothing AndAlso oCommunique.Subject.Trim.Length > 0
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::ValidateCommunique")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::ValidateCommunique")
            End Try
            Return oRet
        End Function

        Public Function LoadCommunique(ByVal idCommunique As Integer, Optional ByVal bLoadDocuments As Boolean = True, Optional ByVal bAudit As Boolean = False) As roCommunique
            Dim oCommunique As roCommunique = Nothing

            Dim bolContinue As Boolean = True
            Dim lEmployees As New List(Of Integer)
            Dim lGroups As New List(Of Integer)
            Dim lDocuments As New List(Of DTOs.roDocument)

            Try

                oState.Result = CommuniqueResultEnum.NoError

                Dim tb As DataTable = CreateDataTable("@SELECT# * FROM Communiques WHERE Id = " & idCommunique)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    '0.- Cargamos propiedades comunes
                    With tb.Rows(0)
                        oCommunique = New roCommunique
                        oCommunique.Id = roTypes.Any2Integer(.Item("Id"))
                        oCommunique.IdCompany = roTypes.Any2Integer(.Item("IdCompany"))
                        oCommunique.Subject = roTypes.Any2String(.Item("Subject"))
                        oCommunique.CreatedBy = roPassportManager.GetPassportWithPhoto(.Item("IdCreatedBy"))
                        oCommunique.CreatedOn = .Item("CreatedOn")
                        oCommunique.MandatoryRead = roTypes.Any2Boolean(.Item("Mandatory"))
                        oCommunique.Message = roTypes.Any2String(.Item("Message")).Replace(vbCr, "<br>")
                        oCommunique.AllowChangeResponse = roTypes.Any2Boolean(.Item("AllowChangeResponse"))
                        oCommunique.AllowedResponses = If(IsDBNull(tb.Rows(0).Item("AllowedResponses")), {}, roTypes.Any2String(.Item("AllowedResponses")).Split("~"))
                        oCommunique.ResponsePercentageLimit = roTypes.Any2Integer(.Item("ResponseLimitPercentage"))
                        If Not IsDBNull(.Item("ExpirationDate")) Then oCommunique.ExpirationDate = roTypes.Any2DateTime(.Item("ExpirationDate"))
                        If Not IsDBNull(.Item("PlanificationDate")) Then oCommunique.PlanificationDate = roTypes.Any2DateTime(.Item("PlanificationDate"))
                        If Not IsDBNull(.Item("SentOn")) Then oCommunique.SentOn = roTypes.Any2DateTime(.Item("SentOn"))
                        oCommunique.Status = roTypes.Any2Integer(.Item("Status"))
                        oCommunique.Archived = roTypes.Any2Boolean(.Item("Archived"))
                    End With

                    '1.- Cargo Empleados
                    tb = CreateDataTable("@SELECT# * FROM CommuniqueEmployees WHERE IdCommunique = " & idCommunique)
                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oRow As DataRow In tb.Rows
                            lEmployees.Add(oRow("IdEmployee"))
                        Next
                        oCommunique.Employees = lEmployees.ToArray
                    Else
                        oCommunique.Employees = {}
                    End If

                    '2.- Cargo Grupos
                    tb = CreateDataTable("@SELECT# * FROM CommuniqueGroups WHERE IdCommunique = " & idCommunique)
                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oRow As DataRow In tb.Rows
                            lGroups.Add(oRow("IdGroup"))
                        Next
                        oCommunique.Groups = lGroups.ToArray
                    Else
                        oCommunique.Groups = {}
                    End If

                    '3.- Cargo Documentos
                    tb = CreateDataTable("@SELECT# Id FROM Documents WHERE IdCommunique = " & idCommunique)
                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim oDocument As New DTOs.roDocument
                        Dim oDocumentState As New VTDocuments.roDocumentState(Me.oState.IDPassport)
                        Dim oDocumentManager As New VTDocuments.roDocumentManager(oDocumentState)
                        For Each oRow As DataRow In tb.Rows
                            oDocument = oDocumentManager.LoadDocument(oRow("Id"), False, False)
                            If bLoadDocuments Then
                                Dim docContent As Byte() = {}
                                docContent = oDocumentManager.GetDocumentBytesById(oDocument.Id).DocumentContent
                                oDocument.Weight = docContent.Length

                                oDocument.Document = docContent
                            End If
                            lDocuments.Add(oDocument)
                        Next
                        oCommunique.Documents = lDocuments.ToArray
                    Else
                        oCommunique.Documents = {}
                    End If
                Else
                    oState.Result = CommuniqueResultEnum.ErrorLoadingCommunique
                End If

                If bolContinue AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCommunique, oCommunique.Subject, tbParameters, -1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::LoadCommunique")
            End Try
            Return oCommunique
        End Function

        Public Function GetCommuniquesByCreator(creatorId As Integer) As List(Of roCommunique)
            Dim oCommunique As roCommunique = Nothing

            Dim bolContinue As Boolean = True
            Dim retCommuniques As New List(Of roCommunique)

            Try

                oState.Result = CommuniqueResultEnum.NoError

                Dim tb As DataTable

                tb = CreateDataTable("@SELECT# * FROM Communiques WHERE IdCreatedBy = " & creatorId)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oCommunique = New roCommunique
                        oCommunique = LoadCommunique(roTypes.Any2Integer(oRow("Id")), False)
                        retCommuniques.Add(oCommunique)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetCommuniquesByCreator")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetCommuniquesByCreator")
            Finally

            End Try
            Return retCommuniques
        End Function

        Public Function GetAllCommuniques(Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As List(Of roCommunique)
            Dim oCommunique As roCommunique = Nothing

            Dim bolContinue As Boolean = True
            Dim retCommuniques As New List(Of roCommunique)
            Dim bForEmployee As Boolean = (idEmployee > 0)
            Dim strSQL As String

            Try

                oState.Result = CommuniqueResultEnum.NoError

                Dim tb As DataTable
                If bForEmployee Then
                    strSQL = "@SELECT# Communiques.Id FROM Communiques WITH (NOLOCK) " &
                             " INNER JOIN sysrovwCommuniquesEmployees WITH (NOLOCK) ON Communiques.id = sysrovwCommuniquesEmployees.IdCommunique WHERE IdEmployee = " & idEmployee & " AND ExpirationDate >= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime
                    tb = CreateDataTable(strSQL)
                Else
                    tb = CreateDataTable("@SELECT# * FROM Communiques WITH (NOLOCK) WHERE IdCreatedBy = " & Me.oState.IDPassport)
                End If

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oCommunique = New roCommunique
                        oCommunique = LoadCommunique(roTypes.Any2Integer(oRow("Id")), False)

                        ' Para empleados no muestro comunicados caducados
                        If Not bForEmployee OrElse (oCommunique.Status = CommuniqueStatusEnum.Online) Then
                            retCommuniques.Add(oCommunique)
                        End If
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::LoadCommunique")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::LoadCommunique")
            Finally

            End Try
            Return retCommuniques
        End Function

        Public Function GetCommuniqueWithStatistics(ByVal idCommunique As Integer, Optional ByVal bAudit As Boolean = False) As roCommuniqueWithStatistics
            Dim retCommuniqueStats As New roCommuniqueWithStatistics
            Dim oCommunique As New roCommunique

            Dim lCommuniqueStats As New List(Of roCommuniqueStatusForEmployee)

            Try

                oState.Result = CommuniqueResultEnum.NoError

                oCommunique = LoadCommunique(idCommunique)

                If Not oCommunique Is Nothing Then
                    retCommuniqueStats.Communique = oCommunique
                    Dim oCommuniqueEmployeeStatus As roCommuniqueStatusForEmployee

                    Dim strSQL As String = "@SELECT# * FROM sysrovwCommuniquesStatistics WHERE IdCommunique = " & idCommunique.ToString
                    Dim tbStats As DataTable = CreateDataTable(strSQL)

                    If Not tbStats Is Nothing AndAlso tbStats.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbStats.Rows
                            oCommuniqueEmployeeStatus = New roCommuniqueStatusForEmployee
                            oCommuniqueEmployeeStatus.IdEmployee = oRow("IdEmployee")
                            oCommuniqueEmployeeStatus.EmployeeName = roTypes.Any2String(oRow("Name"))
                            oCommuniqueEmployeeStatus.Read = roTypes.Any2Boolean(oRow("ReadStatus"))
                            If Not IsDBNull(oRow("ReadTimeStamp")) Then oCommuniqueEmployeeStatus.ReadTimeStamp = roTypes.Any2DateTime(oRow("ReadTimeStamp"))
                            oCommuniqueEmployeeStatus.AnswerRequired = roTypes.Any2Boolean(oRow("RequiresResponse"))
                            oCommuniqueEmployeeStatus.Answered = roTypes.Any2Boolean(oRow("ResponseStatus"))
                            oCommuniqueEmployeeStatus.Answer = roTypes.Any2String(oRow("Response"))
                            If Not IsDBNull(oRow("ResponseTimeStamp")) Then oCommuniqueEmployeeStatus.AnswerTimeStamp = roTypes.Any2DateTime(oRow("ResponseTimeStamp"))
                            lCommuniqueStats.Add(oCommuniqueEmployeeStatus)
                        Next
                        retCommuniqueStats.EmployeeCommuniqueStatus = lCommuniqueStats.ToArray
                    Else
                        retCommuniqueStats.EmployeeCommuniqueStatus = {}
                    End If
                Else
                    oState.Result = CommuniqueResultEnum.ErrorLoadingCommunique
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetCommuniqueStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetCommuniqueStatus")
            End Try

            Return retCommuniqueStats

        End Function


        Public Function HasEmployeeCommuniquesWithAlert(ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False, Optional ByVal bLoadContent As Boolean = False) As Boolean

            Try

                oState.Result = CommuniqueResultEnum.NoError
                Dim strSQL As String
                Dim tb As DataTable
                strSQL = "@SELECT# Communiques.Id from Communiques WITH (NOLOCK) " &
                         " INNER JOIN sysrovwCommuniquesEmployees WITH (NOLOCK) ON Communiques.id = sysrovwCommuniquesEmployees.IdCommunique WHERE IdEmployee = " & idEmployee & " AND ExpirationDate >= " & roTypes.Any2Time(Now).SQLSmallDateTime &
                         " AND Communiques.Status = 1 AND Communiques.Archived = 0 AND Communiques.Mandatory = 1" &
                " AND ((PlanificationDate IS NULL) OR (PlanificationDate <= " & roTypes.Any2Time(Now).SQLSmallDateTime & "))" &
                "order by Communiques.SentOn desc"
                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Dim iCommuniqueId As Integer = roTypes.Any2Integer(oRow("Id"))

                        'Cargo estadística del comunicado
                        strSQL = "@SELECT# * FROM sysrovwCommuniquesStatistics WITH (NOLOCK) WHERE IdCommunique = " & iCommuniqueId.ToString & " AND IdEmployee = " & idEmployee.ToString
                        Dim tbStats As DataTable = CreateDataTable(strSQL)
                        If tbStats IsNot Nothing AndAlso tbStats.Rows.Count > 0 Then
                            For Each oStatRow As DataRow In tbStats.Rows
                                If (roTypes.Any2Boolean(oStatRow("RequiresResponse")) AndAlso roTypes.Any2String(oStatRow("Response")) = String.Empty) OrElse Not roTypes.Any2Boolean(oStatRow("ReadStatus")) Then
                                    Return True
                                End If
                            Next
                        End If
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::HasEmployeeCommuniquesWithAlert")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::HasEmployeeCommuniquesWithAlert")
            End Try

            Return False
        End Function

        Public Function GetAllEmployeeCommuniquesWithStatus(ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False, Optional ByVal bLoadContent As Boolean = False) As List(Of roCommuniqueWithStatistics)
            Dim retListCommuniqueStats As New List(Of roCommuniqueWithStatistics)
            Dim retCommuniqueStats As roCommuniqueWithStatistics
            Dim oCommunique As roCommunique
            Dim oCommuniqueEmployeeStatus As roCommuniqueStatusForEmployee

            Try

                oState.Result = CommuniqueResultEnum.NoError

                Dim strSQL As String
                Dim tb As DataTable
                strSQL = "@SELECT# Communiques.Id from Communiques WITH (NOLOCK) " &
                         " INNER JOIN sysrovwCommuniquesEmployees WITH (NOLOCK) ON Communiques.id = sysrovwCommuniquesEmployees.IdCommunique WHERE IdEmployee = " & idEmployee & " AND ExpirationDate >= " & roTypes.Any2Time(Now).SQLSmallDateTime &
                         " AND Communiques.Status = 1 " &
                " AND ((PlanificationDate IS NULL) OR (PlanificationDate <= " & roTypes.Any2Time(Now).SQLSmallDateTime & "))" &
                "order by Communiques.SentOn desc"
                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        retCommuniqueStats = New roCommuniqueWithStatistics
                        oCommunique = New roCommunique
                        oCommunique = LoadCommunique(roTypes.Any2Integer(oRow("Id")), bLoadContent)
                        oCommuniqueEmployeeStatus = New roCommuniqueStatusForEmployee

                        ' Para empleados no muestro comunicados caducados
                        retCommuniqueStats.Communique = oCommunique

                        strSQL = ""
                        If oCommunique IsNot Nothing Then
                            strSQL = "@SELECT# * FROM sysrovwCommuniquesStatistics WITH (NOLOCK) WHERE IdCommunique = " & oCommunique.Id.ToString & " AND IdEmployee = " & idEmployee.ToString
                        End If
                        'Cargo estadística del comunicado
                        Dim tbStats As DataTable = CreateDataTable(strSQL)
                        If tbStats IsNot Nothing AndAlso tbStats.Rows.Count > 0 Then
                            For Each oStatRow As DataRow In tbStats.Rows
                                oCommuniqueEmployeeStatus = New roCommuniqueStatusForEmployee
                                oCommuniqueEmployeeStatus.IdEmployee = oStatRow("IdEmployee")
                                oCommuniqueEmployeeStatus.EmployeeName = roTypes.Any2String(oStatRow("Name"))
                                oCommuniqueEmployeeStatus.Read = roTypes.Any2Boolean(oStatRow("ReadStatus"))
                                If Not IsDBNull(oStatRow("ReadTimeStamp")) Then oCommuniqueEmployeeStatus.ReadTimeStamp = roTypes.Any2DateTime(oStatRow("ReadTimeStamp"))
                                oCommuniqueEmployeeStatus.AnswerRequired = roTypes.Any2Boolean(oStatRow("RequiresResponse"))
                                oCommuniqueEmployeeStatus.Answered = roTypes.Any2Boolean(oStatRow("ResponseStatus"))
                                oCommuniqueEmployeeStatus.Answer = roTypes.Any2String(oStatRow("Response"))
                                If Not IsDBNull(oStatRow("ResponseTimeStamp")) Then oCommuniqueEmployeeStatus.AnswerTimeStamp = roTypes.Any2DateTime(oStatRow("ResponseTimeStamp"))
                                retCommuniqueStats.EmployeeCommuniqueStatus = {oCommuniqueEmployeeStatus}
                            Next
                        Else
                            retCommuniqueStats.EmployeeCommuniqueStatus = {}
                        End If
                        retListCommuniqueStats.Add(retCommuniqueStats)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetAllEmployeeCommuniquesWithStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCommuniqueManager::GetAllEmployeeCommuniquesWithStatus")
            End Try

            Return retListCommuniqueStats

        End Function

        Public Function SetCommuniqueRead(ByVal idCommunique As Integer, ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim iCommitedCommuniques As Integer = 0

            Try

                oState.Result = CommuniqueResultEnum.NoError

                Dim tb As New DataTable("CommuniqueEmployeeStatus")
                Dim strSQL As String = "@SELECT# * FROM CommuniqueEmployeeStatus WHERE IdCommunique = " & idCommunique.ToString & " AND IdEmployee = " & idEmployee.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    oRow = tb.NewRow
                    oRow("IdCommunique") = idCommunique
                    oRow("IdEmployee") = idEmployee
                    oRow("ReadTimeStamp") = Now
                    tb.Rows.Add(oRow)

                    iCommitedCommuniques = da.Update(tb)
                    bolRet = (iCommitedCommuniques >= 1)
                Else
                    ' Ya estaba marcado como leído
                    bolRet = True
                End If

                If Not bolRet Then oState.Result = CommuniqueResultEnum.ErrorSettingReadMark

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Dim oCommunique As New roCommunique
                    oCommunique = LoadCommunique(idCommunique, False)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCommunique, oCommunique.Subject, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SetCommuniqueRead")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SetCommuniqueRead")
            End Try
            Return bolRet
        End Function

        Public Function AnswerCommunique(ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal sAnswer As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim iCommitedCommuniques As Integer = 0
            Dim oCommunique As New roCommunique

            Try
                oState.Result = CommuniqueResultEnum.NoError

                Dim tb As DataTable
                Dim strSQL As String = "@SELECT# * FROM CommuniqueEmployeeStatus WHERE IdCommunique = " & idCommunique.ToString & " AND IdEmployee = " & idEmployee.ToString
                tb = CreateDataTable(strSQL)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    ' Veo si se puede cambiar de opinión
                    oCommunique = LoadCommunique(idCommunique, False)

                    strSQL = "@UPDATE# CommuniqueEmployeeStatus " &
                                 " SET Response = '" & sAnswer & "', ResponseTimeStamp = " & roTypes.Any2Time(Now).SQLSmallDateTime & " WHERE IdCommunique = " & idCommunique.ToString & " AND IdEmployee = " & idEmployee.ToString
                    bolRet = ExecuteSql(strSQL)
                Else
                    Me.oState.Result = CommuniqueResultEnum.UnexpectedCommuniqueStatusForEmployee
                End If

                If Not bolRet Then oState.Result = CommuniqueResultEnum.ErrorSettingAnswer

                If bolRet AndAlso oCommunique IsNot Nothing AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCommunique, oCommunique.Subject, tbParameters, -1)
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::AnswerCommunique")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::AnswerCommunique")
            End Try
            Return bolRet
        End Function

#End Region

    End Class

End Namespace