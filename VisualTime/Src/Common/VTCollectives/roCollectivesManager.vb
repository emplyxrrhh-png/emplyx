Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roBaseState

Namespace VTCollectives


    Public Class roCollectivesManager

        Private oState As roCollectiveState = Nothing

        Public ReadOnly Property State As roCollectiveState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"
        Public Sub New()
            oState = New roCollectiveState()
        End Sub

        Public Sub New(ByVal _State As roCollectiveState)
            oState = _State
        End Sub
#End Region

#Region "Public Methods"

        Public Function GetCollectiveById(idCollective As Integer, Optional loadDefinition As Boolean = True, Optional ByVal auditAction As Boolean = False) As roCollective
            Dim returnCollective As roCollective = Nothing
            Dim sqlCommand As String

            Try

                oState.ClearResult()

                Dim table As DataTable

                sqlCommand = $"@SELECT# Collectives.ID, 
                                       Collectives.Name, 
	                                   Collectives.Description,
                                       Collectives.CreatedBy,
                                       Collectives.CreatedDate,
	                                   CollectivesDefinitions.Id IdDefinition,
	                                   CollectivesDefinitions.Description DefinitionDescription,
	                                   CollectivesDefinitions.Definition, 
	                                   CollectivesDefinitions.Filter,
	                                   CollectivesDefinitions.BeginDate,
	                                   ISNULL(LEAD(CollectivesDefinitions.BeginDate) OVER (PARTITION BY CollectivesDefinitions.IDCollective ORDER BY CollectivesDefinitions.BeginDate) - 1, CONVERT(SMALLDATETIME, '2079-01-01',120)) AS EndDate,
	                                   CollectivesDefinitions.ModifiedBy,
	                                   CollectivesDefinitions.ModifiedDate
                                FROM Collectives
                                INNER JOIN CollectivesDefinitions ON Collectives.id = CollectivesDefinitions.idcollective
                                WHERE Collectives.ID = {idCollective}
                                ORDER BY CollectivesDefinitions.BeginDate ASC"

                table = CreateDataTable(sqlCommand)

                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    Dim collectiveRow As DataRow = table.Rows(0)

                    returnCollective = New roCollective
                    returnCollective.Id = roTypes.Any2Integer(collectiveRow("Id"))
                    returnCollective.Name = roTypes.Any2String(collectiveRow("Name"))
                    returnCollective.Description = roTypes.Any2String(collectiveRow("Description"))
                    returnCollective.HistoryEntries = New List(Of roCollectiveDefinition)
                    returnCollective.CreatedBy = roTypes.Any2String(collectiveRow("CreatedBy"))
                    returnCollective.CreatedDate = roTypes.Any2DateTime(collectiveRow("CreatedDate")).Date

                    For Each row As DataRow In table.Rows
                        Dim idCollectiveDefinition As Integer = roTypes.Any2Integer(row("IdDefinition"))
                        Dim collectiveDefinition As roCollectiveDefinition = New roCollectiveDefinition()

                        collectiveDefinition.Id = idCollectiveDefinition
                        collectiveDefinition.CollectiveId = returnCollective.Id
                        collectiveDefinition.Definition = roTypes.Any2String(row("Definition"))
                        collectiveDefinition.Description = roTypes.Any2String(row("DefinitionDescription"))
                        collectiveDefinition.FilterExpression = roTypes.Any2String(row("Filter"))
                        collectiveDefinition.BeginDate = roTypes.Any2DateTime(row("BeginDate")).Date
                        collectiveDefinition.EndDate = roTypes.Any2DateTime(row("EndDate")).Date
                        collectiveDefinition.ModifiedBy = roTypes.Any2String(row("ModifiedBy"))
                        collectiveDefinition.ModifiedDate = roTypes.Any2DateTime(row("ModifiedDate"))
                        returnCollective.HistoryEntries.Add(collectiveDefinition)
                    Next

                End If

                If (oState.Result = CollectiveResult.NoError) AndAlso auditAction Then
                    Dim collectiveName As String = If(returnCollective IsNot Nothing, returnCollective.Name, "")

                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{CollectiveName}", collectiveName, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCollective, collectiveName, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.LogLevel = roLog.EventType.roError
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectiveById")
                oState.Result = CollectiveResult.ErrorRecoveringCollective
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectiveById")
                oState.Result = CollectiveResult.ErrorRecoveringCollective
            End Try
            Return returnCollective
        End Function

        Public Function GetAllCollectives(Optional loadDefinitions As Boolean = False, Optional ByVal auditAction As Boolean = False) As List(Of roCollective)
            Dim returnCollectives As New List(Of roCollective)

            Try
                oState.ClearResult()

                Dim collective As New roCollective
                Dim table As DataTable

                Dim strSQL As String = $"@SELECT# [Id] FROM [dbo].[Collectives] ORDER BY Name ASC"

                table = CreateDataTable(strSQL)

                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    For Each row As DataRow In table.Rows
                        collective = GetCollectiveById(row("ID"), loadDefinitions, auditAction)
                        If collective IsNot Nothing Then returnCollectives.Add(collective)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetAllCollectives")
                oState.Result = CollectiveResult.ErrorRecoveringAllCollectives
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetAllCollectives")
                oState.Result = CollectiveResult.ErrorRecoveringAllCollectives
            End Try
            Return returnCollectives
        End Function

        Public Function CreateOrUpdateCollective(ByRef collective As roCollective, Optional auditAction As Boolean = False) As Boolean
            Dim returnValue As Boolean = False
            Dim isNewCollective As Boolean = True
            Dim haveToClose As Boolean = False

            Try
                oState.ClearResult()

                Dim supervisorName As String = Me.oState.GetPassport.Name

                If Not ValidateCollective(collective) Then
                    Return False
                End If

                isNewCollective = (collective.Id <= 0)

                ' Verificamos permiso
                Dim featureId As String = "Employees"

                Dim iPermissionOverCollectives As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
                If iPermissionOverCollectives < 6 Then
                    Me.oState.Result = CollectiveResult.NoPermission
                    Return False
                End If

                haveToClose = StartTransaction()

                ' 0.- Procesamiento del Colectivo
                If Not ProcessCollective(collective, isNewCollective, supervisorName, auditAction) Then
                    Return False
                End If

                ' 1.- Procesamiento de las definiciones del colectivo
                If oState.Result = CollectiveResult.NoError AndAlso collective.HistoryEntries IsNot Nothing Then
                    returnValue = ProcessCollectiveDefinitions(collective, supervisorName, auditAction)
                    If Not returnValue Then
                        Return False
                    End If
                End If

                returnValue = True

            Catch ex As DbException
                returnValue = False
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::CreateOrUpdateCollective")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
            Catch ex As Exception
                returnValue = False
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::CreateOrUpdateCollective")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(haveToClose, oState.Result = CollectiveResult.NoError)
            End Try

            Return returnValue
        End Function

        ''' <summary>
        ''' Procesa la creación o actualización del colectivo principal
        ''' </summary>
        Private Function ProcessCollective(ByRef collective As roCollective, ByVal isNewCollective As Boolean, ByVal supervisorName As String, auditAction As Boolean) As Boolean
            Dim sqlCommand As String = String.Empty
            Dim parameters As New List(Of CommandParameter)
            Dim newCollectiveId As Integer

            Try
                ' Procesamiento según el estado de edición
                Select Case collective.EditionStatus
                    Case ItemEditionStatus.New, ItemEditionStatus.Edited
                        ' Colectivo nuevo o editado
                        If isNewCollective OrElse collective.EditionStatus = ItemEditionStatus.New Then
                            ' Nuevo colectivo
                            sqlCommand = $"@INSERT# INTO Collectives ([Name] ,[Description], [CreatedBy], [CreatedDate]) 
                                 OUTPUT INSERTED.ID 
                                 VALUES (@name, @description, @createdBy, @createdDate)"

                            parameters.Add(New CommandParameter("@name", CommandParameter.ParameterType.tString, collective.Name))
                            parameters.Add(New CommandParameter("@description", CommandParameter.ParameterType.tString,
                                  If(collective.Description Is Nothing, String.Empty, roTypes.Any2String(collective.Description))))
                            parameters.Add(New CommandParameter("@createdBy", CommandParameter.ParameterType.tString, supervisorName))
                            parameters.Add(New CommandParameter("@createdDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))

                            newCollectiveId = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                            collective.Id = newCollectiveId
                            collective.EditionStatus = ItemEditionStatus.NotEdited

                        Else
                            ' Actualización de colectivo existente
                            sqlCommand = $"@UPDATE# Collectives SET [Name]=@name, [Description]=@description
                                WHERE [Id] = @id"

                            parameters.Add(New CommandParameter("@name", CommandParameter.ParameterType.tString, collective.Name))
                            parameters.Add(New CommandParameter("@description", CommandParameter.ParameterType.tString,
                                  If(collective.Description Is Nothing, String.Empty, roTypes.Any2String(collective.Description))))
                            parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, collective.Id))

                            AccessHelper.ExecuteSql(sqlCommand, parameters)
                            collective.EditionStatus = ItemEditionStatus.NotEdited
                        End If

                    Case ItemEditionStatus.Deleted
                        ' El colectivo debe ser eliminado
                        Return DeleteCollective(collective, False)
                    Case ItemEditionStatus.NotEdited
                        ' No se requiere ninguna acción para el colectivo
                        Return True
                    Case Else
                        ' Estado desconocido
                        Me.oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
                        Return False
                End Select

                If (oState.Result = CollectiveResult.NoError) AndAlso auditAction Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{CollectiveName}", collective.Name, String.Empty, 1)
                    Me.oState.Audit(If(isNewCollective, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tCollective, collective.Name, tbParameters, -1)
                End If

                Return True

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::ProcessCollective")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Procesa la creación, actualización o eliminación de las definiciones históricas del colectivo
        ''' </summary>
        Private Function ProcessCollectiveDefinitions(ByRef collective As roCollective, ByVal supervisorName As String, auditAction As Boolean) As Boolean
            Dim sqlCommand As String
            Dim parameters As List(Of CommandParameter)
            Dim checkIfUpdateStatusIsNeeded As Boolean = False
            Dim originalDefinition As roCollectiveDefinition = Nothing
            Dim datesAndEmployeesToRecalculate As New Dictionary(Of Date, List(Of List(Of Integer)))

            Try
                ' Procesar las definiciones del historial según su estado de edición
                For Each definition As roCollectiveDefinition In collective.HistoryEntries
                    parameters = New List(Of CommandParameter)

                    ' Asegurarse que la definición tenga asignado el ID correcto del colectivo
                    definition.CollectiveId = collective.Id

                    Select Case definition.EditionStatus
                        Case ItemEditionStatus.New
                            definition.ModifiedBy = supervisorName
                            definition.ModifiedDate = roTypes.UnspecifiedNow()

                            ' Nueva definición histórica
                            sqlCommand = $"@INSERT# INTO CollectivesDefinitions ([IDCollective],
                                                                       [Description],
                                                                       [Definition], 
                                                                       [Filter], 
                                                                       [BeginDate],
                                                                       [ModifiedBy],
                                                                       [ModifiedDate]) 
                                    OUTPUT INSERTED.ID
                                    VALUES (@idcollective, @description, @definition, @filter,
                                    @begindate, @modifiedby, @modifieddate)"
                            parameters.Add(New CommandParameter("@idcollective", CommandParameter.ParameterType.tInt, collective.Id))
                            parameters.Add(New CommandParameter("@description", CommandParameter.ParameterType.tString, definition.Description))
                            parameters.Add(New CommandParameter("@definition", CommandParameter.ParameterType.tString, definition.Definition))
                            parameters.Add(New CommandParameter("@filter", CommandParameter.ParameterType.tString, GetHavingClause(definition.FilterExpression)))
                            parameters.Add(New CommandParameter("@begindate", CommandParameter.ParameterType.tDateTime, definition.BeginDate.Date))
                            parameters.Add(New CommandParameter("@modifiedby", CommandParameter.ParameterType.tString, definition.ModifiedBy))
                            parameters.Add(New CommandParameter("@modifieddate", CommandParameter.ParameterType.tDateTime, definition.ModifiedDate))

                            Dim newId As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                            definition.Id = newId
                            definition.EditionStatus = ItemEditionStatus.NotEdited

                            If auditAction Then
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                roAudit.AddParameter(tbParameters, "{CollectiveName}", collective.Name, String.Empty, 1)
                                roAudit.AddParameter(tbParameters, "{DefinitionDescription}", definition.Description, String.Empty, 1)
                                Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tCollectiveDefinition, "", tbParameters, -1)
                            End If

                        Case ItemEditionStatus.Edited
                            ' Actualizar definición existente
                            ' Primero obtenemos los valores originales de la definición
                            sqlCommand = $"@SELECT# [Description], [Definition], [Filter], [BeginDate] FROM CollectivesDefinitions WHERE [Id] = {definition.Id}"
                            Dim originalData As DataTable = CreateDataTable(sqlCommand)

                            If originalData IsNot Nothing AndAlso originalData.Rows.Count > 0 Then
                                originalDefinition = New roCollectiveDefinition()
                                originalDefinition.Id = definition.Id
                                originalDefinition.Description = roTypes.Any2String(originalData.Rows(0)("Description"))
                                originalDefinition.Definition = roTypes.Any2String(originalData.Rows(0)("Definition"))
                                originalDefinition.FilterExpression = roTypes.Any2String(originalData.Rows(0)("Filter"))
                                originalDefinition.BeginDate = roTypes.Any2DateTime(originalData.Rows(0)("BeginDate")).Date
                            End If

                            sqlCommand = $"@UPDATE# CollectivesDefinitions 
                                   SET   [Description] = @description,
                                         [Definition] = @definition, 
                                         [Filter] = @filter,
                                         [BeginDate] = @begindate,
                                         [ModifiedBy] = @modifiedby,
                                         [ModifiedDate] = @modifieddate
                                   WHERE [Id] = @id"

                            parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, definition.Id))
                            parameters.Add(New CommandParameter("@description", CommandParameter.ParameterType.tString, definition.Description))
                            parameters.Add(New CommandParameter("@definition", CommandParameter.ParameterType.tString, definition.Definition))
                            parameters.Add(New CommandParameter("@filter", CommandParameter.ParameterType.tString, GetHavingClause(definition.FilterExpression)))
                            parameters.Add(New CommandParameter("@begindate", CommandParameter.ParameterType.tDateTime, definition.BeginDate.Date))
                            parameters.Add(New CommandParameter("@modifiedby", CommandParameter.ParameterType.tString, supervisorName))
                            parameters.Add(New CommandParameter("@modifieddate", CommandParameter.ParameterType.tDateTime, DateTime.Now))

                            AccessHelper.ExecuteSql(sqlCommand, parameters)
                            definition.EditionStatus = ItemEditionStatus.NotEdited

                            checkIfUpdateStatusIsNeeded = True

                            If auditAction Then
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                roAudit.AddParameter(tbParameters, "{CollectiveName}", collective.Name, String.Empty, 1)

                                ' Añadir información sobre cambios específicos
                                If originalDefinition IsNot Nothing Then
                                    Dim changeDetails As String = String.Empty
                                    If originalDefinition.Description <> definition.Description Then
                                        changeDetails = $"Description: {originalDefinition.Description} -> {definition.Description} "
                                    End If

                                    If originalDefinition.Definition <> definition.Definition Then
                                        changeDetails = $"{changeDetails} Definition: {originalDefinition.Definition} -> {definition.Definition} "
                                    End If

                                    If originalDefinition.BeginDate <> definition.BeginDate Then
                                        changeDetails = $"{changeDetails} Begin date: {originalDefinition.BeginDate.ToString("yyyy-MM-dd")} -> {definition.BeginDate.ToString("yyyy-MM-dd")} "
                                    End If
                                    If changeDetails.Trim <> String.Empty Then
                                        roAudit.AddParameter(tbParameters, "{ChangeDetails}", changeDetails, String.Empty, 1)
                                    End If
                                End If

                                Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCollectiveDefinition, collective.Name, tbParameters, -1)
                            End If
                        Case ItemEditionStatus.Deleted
                            ' Eliminar definición existente
                            If definition.Id > 0 Then
                                sqlCommand = $"@DELETE# FROM CollectivesDefinitions WHERE Id = @id"
                                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, definition.Id))
                                AccessHelper.ExecuteSql(sqlCommand, parameters)
                            End If

                            checkIfUpdateStatusIsNeeded = True

                            If auditAction Then
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                roAudit.AddParameter(tbParameters, "{CollectiveName}", collective.Name, String.Empty, 1)
                                Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCollectiveDefinition, "", tbParameters, -1)
                            End If
                        Case ItemEditionStatus.NotEdited
                            ' No requiere ninguna acción
                            Continue For
                    End Select

                    If checkIfUpdateStatusIsNeeded AndAlso datesAndEmployeesToRecalculate IsNot Nothing Then
                        UpdateStatusIfNeeded(datesAndEmployeesToRecalculate, originalDefinition, definition)
                    End If
                Next

                If datesAndEmployeesToRecalculate IsNot Nothing AndAlso datesAndEmployeesToRecalculate.Any Then
                    UpdateStatus(datesAndEmployeesToRecalculate)
                End If

                ' Eliminar de la lista las definiciones marcadas para eliminación
                collective.HistoryEntries.RemoveAll(Function(d) d.EditionStatus = ItemEditionStatus.Deleted)

                Return True

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::ProcessCollectiveDefinitions")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
                Return False
            End Try
        End Function

        Private Function UpdateStatusIfNeeded(ByRef datesAndEmployeesToRecalculate As Dictionary(Of Date, List(Of List(Of Integer))), originalDefinition As roCollectiveDefinition, newDefinition As roCollectiveDefinition) As Boolean
            Try

                If datesAndEmployeesToRecalculate IsNot Nothing AndAlso originalDefinition IsNot Nothing AndAlso newDefinition IsNot Nothing Then
                    ' 0 - Verificamos si el cambio en la fecha de inicio requiere recálculo
                    If originalDefinition.BeginDate > Now.Date AndAlso newDefinition.BeginDate > Now.Date Then
                        ' Definición futura, no se requiere recálculo
                        Return False
                    End If

                    ' 1 - Verificamos si ha cambiado la definicion
                    If originalDefinition.Definition = newDefinition.Definition AndAlso originalDefinition.BeginDate = newDefinition.BeginDate Then
                        ' No hay cambios efectivos, no se requiere recálculo
                        Return False
                    End If

                    ' 2 - Verificamos si el colectivo se usa en alguna regla (esta verificación es la última por se la más costosa)
                    '     Si se usa en regla, se requiere recálculo
                    ' TODO: Cargamos todas las reglas y verificamos si usan esl colectivo ... Plantear si en manager de reglas se deberían guardar los colectivos de otro modo (ahora se guarda con la definición del selector universal, que es un JSON)
                    '       Para cada colectivo afectado, y fechas de inicio y fin de la definición se podrían calcular los empleados afectados para limitar el recálculo al máximo.
                    ' Dim oldestDate As Date = whatever
                    ' If Not datesAndEmployeesToRecalculate.ContainsKey(oldestDate) Then
                    '    datesAndEmployeesToRecalculate(oldestDate) = New List(Of List(Of Integer))
                    ' End If
                    ' datesAndEmployeesToRecalculate(oldestDate).Add(New List(Of Integer)({1, 2, 3}))
                End If

                Return True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::UpdateStatusIfNeeded")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
                Return False
            End Try
        End Function

        Private Function UpdateStatus(ByVal datesAndEmployeesToRecalculate As Dictionary(Of Date, List(Of List(Of Integer)))) As Boolean
            Try
                ' 0 - Actualizamos status
                If datesAndEmployeesToRecalculate IsNot Nothing AndAlso datesAndEmployeesToRecalculate.Any Then
                    For Each kvp As KeyValuePair(Of Date, List(Of List(Of Integer))) In datesAndEmployeesToRecalculate
                        Dim sqlCommand As String = $"@UPDATE# DailySchedule SET [Status] = @status WHERE [Status] > @status AND Date BETWEEN @recalcdate AND CAST(GETDATE() AS DATE) AND IDEmployee IN (@employeeids)"
                        Dim parameters As New List(Of CommandParameter)
                        parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tString, 50))
                        parameters.Add(New CommandParameter("@recalcdate", CommandParameter.ParameterType.tDateTime, kvp.Key.Date))
                        parameters.Add(New CommandParameter("@employeeids", CommandParameter.ParameterType.tString, String.Join(",", kvp.Value.SelectMany(Function(x) x).Distinct())))
                        AccessHelper.ExecuteSql(sqlCommand, parameters)
                    Next
                End If

                ' 1 - Auditamos
                '     Hacer cálculo grueso de los días/empleados recalculados para indicarlo en la auditoría (si se quiere auditar)

                Return True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::UpdateStatus")
                oState.Result = CollectiveResult.ErrorCreatingOrUpdatingCollective
                Return False
            End Try
        End Function

        Private Function ValidateCollective(collective As roCollective) As Boolean
            Dim returnValue As Boolean = True
            Dim sqlCommand As String = String.Empty

            Try
                If Not (DataLayer.roSupport.IsXSSSafe($"{collective.Name}") AndAlso DataLayer.roSupport.IsXSSSafe($"{collective.Description}")) Then
                    oState.Result = CollectiveResult.XSSvalidationError
                    Return False
                End If

                If collective.Name.Trim = String.Empty Then
                    Me.oState.Result = CollectiveResult.CollectiveNameCanNotBeEmpty
                    Return False
                End If

                sqlCommand = $"@SELECT# COUNT(*) FROM [dbo].[Collectives] WHERE Id <> {collective.Id} AND Name = '{collective.Name.Replace("'", "''")}'"
                If roTypes.Any2Integer(ExecuteScalar(sqlCommand)) > 0 Then
                    Me.oState.Result = CollectiveResult.NameAlreadyExists
                    Return False
                End If

                ' No existe otra definición con la misma fecha de inicio en el mismo colectivo
                For Each definition As roCollectiveDefinition In collective.HistoryEntries.Where(Function(y) y.EditionStatus <> ItemEditionStatus.Deleted)
                    If collective.HistoryEntries.FindAll(Function(x) x.BeginDate.Date = definition.BeginDate.Date AndAlso x.Id <> definition.Id AndAlso x.EditionStatus <> ItemEditionStatus.Deleted).Any Then
                        Me.oState.Result = CollectiveResult.CollectiveDefinitionOverlapped
                        Return False
                    End If
                Next

            Catch ex As Exception
                returnValue = False
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::ValidateCollective")
                oState.Result = CollectiveResult.Exception
            End Try

            Return returnValue
        End Function

        Public Function ValidateCollectiveDefinition(collectiveDefinition As roCollectiveDefinition) As Boolean
            Dim returnValue As Boolean = True

            Try
                If collectiveDefinition Is Nothing OrElse collectiveDefinition.Definition.Trim = "[]" OrElse collectiveDefinition.FilterExpression.Trim = "null" Then
                    Me.oState.Result = CollectiveResult.CollectiveFilterCanNotBeEmpty
                    Return False
                End If

                If collectiveDefinition.BeginDate < roParameters.GetFirstDate().AddDays(1) Then
                    Me.oState.Result = CollectiveResult.BeginDefinitionInFreezeDate
                    Return False
                End If

                ' No existe otra definición con la misma fecha de inicio
                If collectiveDefinition.CollectiveId > 0 Then
                    Dim collective As roCollective = GetCollectiveById(collectiveDefinition.CollectiveId)
                    If collective IsNot Nothing Then
                        If collective.HistoryEntries.FindAll(Function(x) x.BeginDate.Date = collectiveDefinition.BeginDate.Date AndAlso x.Id <> collectiveDefinition.Id).Any Then
                            Me.oState.Result = CollectiveResult.CollectiveDefinitionOverlapped
                            Return False
                        End If
                    Else
                        Me.oState.Result = CollectiveResult.ErrorValidatingCollectiveDefinition
                        Return False
                    End If
                End If

                If Not DataLayer.roSupport.IsXSSSafe($"{collectiveDefinition.Description}") Then
                    oState.Result = CollectiveResult.XSSvalidationError
                    Return False
                End If

                Dim usedUserFields As New List(Of String)
                Dim havingClause As String = GetHavingClause(collectiveDefinition.FilterExpression, usedUserFields)
                If havingClause = String.Empty Then
                    Me.oState.Result = CollectiveResult.ErorrGeneratingHavingClause
                    Return False
                Else
                    Dim availableUserFields As New List(Of roUserField)
                    availableUserFields = GetEmployeeUserfields()
                    ' Verifico que todos los campos usados están en los disponibles.
                    Dim allUserfieldsAreElegible As Boolean = availableUserFields.Select(Of String)(Function(x) x.FieldName).Intersect(usedUserFields).Count = usedUserFields.Count

                    If Not allUserfieldsAreElegible Then
                        Me.oState.Result = CollectiveResult.NonElegibleUserfieldUsed
                        Return False
                    End If
                End If

            Catch ex As Exception
                returnValue = False
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::ValidateCollectiveDefinition")

                oState.Result = CollectiveResult.Exception
            End Try

            Return returnValue
        End Function

        Public Function DeleteCollective(ByRef oCollective As roCollective, Optional auditAction As Boolean = False) As Boolean
            Dim returnValue As Boolean = False
            Dim sqlCommand As String = String.Empty
            Dim bHaveToClose As Boolean = False

            Try
                If oCollective.Id <= 0 Then Return True

                Dim featureId As String = "Employees"

                Dim iPermissionOverBots As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
                If iPermissionOverBots < 6 Then
                    Me.oState.Result = CollectiveResult.NoPermission
                    Return False
                End If

                oState.Result = CollectiveResult.ErrorDeletingCollective

                ' No puedo borrar si el colectivo está en uso en alguna regla de negocio
                If UsedInProcess(oCollective.Id) Then
                    oState.Result = CollectiveResult.CollectiveUsedInProcess
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                sqlCommand = $"@DELETE# CollectivesDefinitions WHERE IdCollective = {oCollective.Id}"
                returnValue = ExecuteSql(sqlCommand)
                If returnValue Then
                    sqlCommand = $"@DELETE# Collectives WHERE Id = {oCollective.Id}"
                    returnValue = ExecuteSql(sqlCommand)
                End If

                If returnValue AndAlso auditAction Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{CollectiveName}", oCollective.Name, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCollective, oCollective.Name, tbParameters, -1)
                End If

                oState.ClearResult()

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBotManager::DeleteCollective")
                oState.Result = CollectiveResult.ErrorDeletingCollective
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBotManager::DeleteCollective")
                oState.Result = CollectiveResult.ErrorDeletingCollective
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = CollectiveResult.NoError)
            End Try
            Return returnValue
        End Function

        Public Function UsedInProcess(collectiveId As Integer) As Boolean
            Dim returnValue As Boolean = True
            Try

                ' Usado en Visibilidad de horarios
                Dim sql As String = $"@SELECT# VisibilityCriteria FROM Shifts WHERE VisibilityPermissions = 3"
                Dim resultTable As DataTable = CreateDataTable(sql)
                Dim uniqueCollectiveIds As HashSet(Of Integer) = resultTable.DefaultView.Cast(Of DataRowView)().Select(Function(row) roTypes.Any2String(row("VisibilityCriteria"))).Where(Function(criteria) Not String.IsNullOrEmpty(criteria)).SelectMany(Function(criteria) criteria.Split(","c)).Select(Function(idStr) Integer.Parse(idStr.Trim())).ToHashSet()

                returnValue = uniqueCollectiveIds.Contains(collectiveId)

                ' Usado en Notificaciones
                ' TODO ...


            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::UsedInProcess")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::UsedInProcess")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            End Try

            Return returnValue

        End Function

        Public Function GetCollectiveEmployees(collectiveFilterExpression As String, referenceDate As Date, auditAction As Boolean) As List(Of roSelectedEmployee)
            Dim returnCollectiveEmployees As New List(Of roSelectedEmployee)

            Try
                oState.ClearResult()

                Dim employeesTable As New DataTable()

                employeesTable = GetCollectiveEmployeesFromFilter(collectiveFilterExpression, referenceDate)

                If employeesTable IsNot Nothing AndAlso employeesTable.Rows.Count > 0 Then
                    ' Recorro la tabla y lleno la lista de empleados del colectivo
                    returnCollectiveEmployees = New List(Of roSelectedEmployee)
                    For Each employeeRow As DataRow In employeesTable.Rows
                        returnCollectiveEmployees.Add(New roSelectedEmployee With {.IdEmployee = employeeRow("IDEmployee"), .EmployeeName = employeeRow("Name"), .Group = employeeRow("Group")})
                    Next
                End If

                If (oState.Result = CollectiveResult.NoError) AndAlso auditAction Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{CollectiveExpression}", collectiveFilterExpression, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{TotalEmployeesShown}", returnCollectiveEmployees.Count, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCollectiveEmployees, "", tbParameters, -1)
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectivesEmployees")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectivesEmployees")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            End Try
            Return returnCollectiveEmployees
        End Function

        Public Function GetEmployeeUserfields() As List(Of roUserField)
            Dim returnUserFields As New List(Of roUserField)

            Try
                oState.ClearResult()

                Dim userField As New roUserField
                Dim table As DataTable

                ' Devuelvo campos de la ficha en uso y que sean de tipo texto, numérico, fecha, decimal o lista
                Dim strSQL As String = $"@SELECT# ID, FieldName, FieldType, ListValues, ISNULL(Category,'(sin categoría)') Category FROM sysroUserFields 
                                          WHERE Used = 1 AND Type = 0 AND FieldType IN (0,1,2,3,5) AND TRIM(ISNULL(FieldName,'')) <> '' 
                                          ORDER BY FieldName ASC"

                table = CreateDataTable(strSQL)

                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    For Each row As DataRow In table.Rows
                        userField = New roUserField()
                        userField.Id = roTypes.Any2Integer(row("ID"))
                        userField.FieldName = roTypes.Any2String(row("FieldName"))
                        userField.FieldType = roTypes.Any2Integer(row("FieldType"))
                        userField.ListValues = roTypes.Any2String(row("ListValues")).Split("~").ToList
                        returnUserFields.Add(userField)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetEmployeeUserfields")
                oState.Result = CollectiveResult.ErrorRecoveringEmployeeUserfields
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetEmployeeUserfields")
                oState.Result = CollectiveResult.ErrorRecoveringEmployeeUserfields
            End Try
            Return returnUserFields
        End Function

        Public Function GetHavingClause(filterExpression As String, Optional usedUserFields As List(Of String) = Nothing) As String
            Dim returnValue As String = String.Empty

            Try
                If String.IsNullOrEmpty(filterExpression) Then
                    oState.Result = CollectiveResult.ErorrGeneratingHavingClause
                    Return String.Empty
                End If

                ' Diccionario para cachear los tipos de campo
                Dim fieldTypeCache As New Dictionary(Of String, String)

                ' Preprocesamos la expresión de filtro
                Dim preprocessedList As List(Of Object) = ProcessFilterExpression(filterExpression)
                Dim resultString As New StringBuilder()

                For Each obj As Object In preprocessedList
                    If TypeOf obj Is String Then
                        resultString.Append($"{obj}")
                    ElseIf TypeOf obj Is Comparison Then
                        Dim comparison As Comparison = CType(obj, Comparison)
                        Dim havingClause As String = String.Empty

                        ' Añadimos a la lista de campos usados
                        If usedUserFields IsNot Nothing AndAlso Not usedUserFields.Contains(comparison.Operand1) Then
                            usedUserFields.Add(comparison.Operand1)
                        End If

                        ' Buscamos tipo del campo (usando caché)
                        Dim fieldType As String = GetFieldType(comparison.Operand1, fieldTypeCache)

                        If fieldType <> String.Empty Then
                            Select Case fieldType
                                Case "0" ' Texto
                                    havingClause = BuildTextComparison(comparison)
                                Case "1" ' Numérico
                                    havingClause = BuildNumericComparison(comparison)
                                Case "2" ' Fecha
                                    havingClause = BuildDateComparison(comparison)
                                Case "3" ' Decimal
                                    havingClause = BuildDecimalComparison(comparison)
                                Case "5" ' Lista
                                    havingClause = BuildListComparison(comparison)
                                Case Else ' Otros tipos
                                    havingClause = BuildGenericComparison(comparison)
                            End Select

                            resultString.Append(havingClause)
                        Else
                            oState.Result = CollectiveResult.ErrorFilterExpressionContainsInvalidFields
                            resultString = New StringBuilder()
                            Exit For
                        End If
                    End If
                Next

                returnValue = resultString.ToString()

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetHavingClause")
                oState.Result = CollectiveResult.ErorrGeneratingHavingClause
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetHavingClause")
                oState.Result = CollectiveResult.ErorrGeneratingHavingClause
            End Try

            Return returnValue
        End Function

#End Region

#Region "Private methods"
        Private Function GetCollectiveEmployeesFromFilter(collectiveFilterExpression As String, referenceDate As Date) As DataTable
            Dim returnCollectiveEmployees As DataTable = Nothing

            Try
                oState.ClearResult()

                Dim havingClause As String = roTypes.Any2String(collectiveFilterExpression)
                If havingClause.Trim = String.Empty Then havingClause = "1=2"

                ' En las comparaciones con fecha sliding, caso de ejecutarse para fechas diferentes de hoy, se debe sustituir el GETDATE por la fecha de referencia
                havingClause = havingClause.Replace("GETDATE()", roTypes.Any2Time(referenceDate.Date).SQLSmallDateTime)

                Dim command As String = $"@SELECT# 
                                                IDEmployee, 
                                                EmployeeName [Name], 
                                                    CASE
                                                    WHEN FullGroupName IS NULL THEN NULL
                                                    ELSE
			                                            CASE WHEN CHARINDEX('\', FullGroupName) = 0 THEN GroupName
			                                            ELSE
                                                        IIF(
                                                            CHARINDEX('\', FullGroupName) > 0,
                                                            TRIM(RIGHT(FullGroupName, CHARINDEX('\', REVERSE(FullGroupName)) - 1)),
                                                            TRIM(FullGroupName)
                                                        ) +
                                                        ' (' +
                                                        IIF(
                                                            CHARINDEX('\', FullGroupName) > 0,
                                                            TRIM(LEFT(FullGroupName, CHARINDEX('\', FullGroupName) - 1)),
                                                            TRIM(FullGroupName)
                                                        ) +
                                                        ')'
			                                            END
                                                END AS [Group]  
                                          FROM sysrovwCurrentEmployeeGroups 
                                          WHERE IDEmployee IN (
                                                @SELECT# idEmployee
                                                FROM sysrovwEmployeeUserFieldTypifiedValues
                                                WHERE {roTypes.Any2Time(referenceDate.Date).SQLSmallDateTime} BETWEEN StartDate AND EndDate
                                                GROUP BY idEmployee
                                                HAVING {havingClause} 
                                            )
                                          ORDER BY Name ASC"
                returnCollectiveEmployees = AccessHelper.CreateDataTable(command)

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectiveEmployeesFromFilter")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCollectivesManager::GetCollectiveEmployeesFromFilter")
                oState.Result = CollectiveResult.ErrorRecoveringCollectiveEmployees
            End Try

            Return returnCollectiveEmployees
        End Function
#End Region

#Region "Helper Parsing Filter Expression"
        ' Optimización propuesta por Claude Sonnet 3.7 - 100% tests correctos
        ''' <summary>
        ''' Define los estados posibles durante el análisis de la expresión de filtro
        ''' </summary>
        Private Enum ParsingState
            Normal
            InComparison
            InLogicOperator
            ProcessingNegation
        End Enum

        Public Function BuildTextComparison(comparison As Comparison) As String
            Dim valueColumn As String = "ValueText"
            comparison.Operand2 = $"'{comparison.Operand2.Replace("'", "''")}'"

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function BuildNumericComparison(comparison As Comparison) As String
            Dim valueColumn As String = "ValueInt"

            ' Intentar convertir a entero para evitar inyección SQL
            Dim numericValue As Integer
            If Not Integer.TryParse(comparison.Operand2, numericValue) Then
                ' Si no es un número válido, establecer a 0
                numericValue = 0
            End If

            comparison.Operand2 = numericValue.ToString()

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function BuildDecimalComparison(comparison As Comparison) As String
            Dim valueColumn As String = "ValueDecimal"

            ' Intentar convertir a decimal para evitar inyección SQL
            Dim decimalValue As Decimal
            If Not Decimal.TryParse(comparison.Operand2.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), decimalValue) Then
                ' Si no es un decimal válido, establecer a 0
                decimalValue = 0
            End If

            ' Usar invariant culture para asegurar formato correcto
            comparison.Operand2 = decimalValue.ToString(System.Globalization.CultureInfo.InvariantCulture)

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function BuildDateComparison(comparison As Comparison) As String
            ' Si es aniversario, manejarlo como caso especial
            If comparison.ComparisonOperator.ToUpper = "ANNIVERSARY" Then
                Return BuildAnniversaryComparison(comparison)
            End If

            Dim valueColumn As String = "ValueDate"

            ' Procesar timeAgo si aplica
            If comparison.Operand2.Contains("timeAgo") AndAlso comparison.Operand2.Split("*").Count = 3 Then
                comparison.Operand2 = ProcessTimeAgoExpression(comparison.Operand2)
            Else
                Try
                    comparison.Operand2 = roTypes.Any2Time(comparison.Operand2).SQLSmallDateTime
                Catch ex As Exception
                    ' Si hay error de conversión, usar fecha actual
                    comparison.Operand2 = roTypes.Any2Time(DateTime.Now).SQLSmallDateTime
                End Try
            End If

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function BuildAnniversaryComparison(comparison As Comparison) As String
            Dim comparisonText As String = $"MONTH(ValueDate) = MONTH(GETDATE()) AND DAY(ValueDate) = DAY(GETDATE())"
            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {comparisonText} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function ProcessTimeAgoExpression(timeAgoExpression As String) As String
            Dim parts As String() = timeAgoExpression.Split("*")
            Dim timeAgo As Integer = roTypes.Any2Integer(parts(0))
            Dim unit As String = parts(1).ToLower()

            Select Case unit
                Case "days"
                    Return $"DATEADD(day,-{timeAgo}, CAST(GETDATE() AS DATE))"
                Case "weeks"
                    Return $"DATEADD(week,-{timeAgo}, CAST(GETDATE() AS DATE))"
                Case "months"
                    ' Proteger contra fechas anteriores a 1/1/1900
                    Dim maxMonths As Integer = (Now.Year - 1900) * 12 + Now.Month - 1
                    timeAgo = Math.Min(timeAgo, maxMonths)
                    Return $"DATEADD(month,-{timeAgo}, CAST(GETDATE() AS DATE))"
                Case "years"
                    ' Proteger contra fechas anteriores a 1/1/1900
                    Dim maxYears As Integer = Now.Year - 1900
                    timeAgo = Math.Min(timeAgo, maxYears)
                    Return $"DATEADD(year,-{timeAgo}, CAST(GETDATE() AS DATE))"
                Case Else
                    Return $"CAST(GETDATE() AS DATE)" ' Valor por defecto
            End Select
        End Function

        Public Function BuildListComparison(comparison As Comparison) As String
            Dim valueColumn As String = "ValueList"
            comparison.Operand2 = $"'{comparison.Operand2.Replace("'", "''")}'"

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function BuildGenericComparison(comparison As Comparison) As String
            Dim valueColumn As String = "TRY_CAST(RawValue AS NVARCHAR)"
            comparison.Operand2 = $"'{comparison.Operand2.Replace("'", "''")}'"

            Return $"SUM(CASE WHEN fieldname = '{comparison.Operand1}' AND {valueColumn} {comparison.ComparisonOperator} {comparison.Operand2} THEN 1 ELSE 0 END) > 0"
        End Function

        Public Function GetFieldType(fieldName As String, ByRef cache As Dictionary(Of String, String)) As String
            ' Usar caché si el campo ya fue consultado
            If cache IsNot Nothing AndAlso cache.ContainsKey(fieldName) Then
                Return cache(fieldName)
            End If

            ' Consultar el tipo de campo
            Dim sqlCommand As String = $"@SELECT# FieldType FROM sysroUserFields WHERE FieldName = '{fieldName.Replace("'", "''")}'"
            Dim fieldType As String = roTypes.Any2String(ExecuteScalar(sqlCommand))

            ' Almacenar en caché para futuras consultas
            If cache IsNot Nothing AndAlso Not String.IsNullOrEmpty(fieldType) Then
                cache.Add(fieldName, fieldType)
            End If

            Return fieldType
        End Function

        ''' <summary>
        ''' Procesa una expresión de filtro y la convierte en una lista de objetos que representan
        ''' los componentes lógicos y las comparaciones.
        ''' </summary>
        ''' <param name="filterExpression">La expresión de filtro en formato JSON</param>
        ''' <returns>Una lista de objetos que contiene operadores y comparaciones</returns>
        Public Function ProcessFilterExpression(filterExpression As String) As List(Of Object)
            If String.IsNullOrEmpty(filterExpression) Then
                Return New List(Of Object)()
            End If

            Dim result As New List(Of Object)()
            Dim currentComparison As New StringBuilder()
            Dim currentLogicOperator As New StringBuilder()

            Dim state As ParsingState = ParsingState.Normal
            Dim negationFound As Boolean = False

            For i As Integer = 0 To filterExpression.Length - 1
                Dim currentChar As Char = filterExpression(i)

                ' Ignorar saltos de línea
                If currentChar = vbCr OrElse currentChar = vbLf Then
                    Continue For
                End If

                Select Case state
                    Case ParsingState.Normal
                        HandleNormalState(currentChar, i, filterExpression, state, result, negationFound, currentComparison)

                    Case ParsingState.InComparison
                        If currentChar = "]"c Then
                            ' Finalizar comparación y procesarla
                            Dim comparisonParts As String() = currentComparison.ToString().Split(","c)
                            If comparisonParts.Length >= 3 Then
                                Dim parsedComparison As New Comparison(
                                    comparisonParts(0).Replace("""", ""),
                                    comparisonParts(1).Replace("""", ""),
                                    comparisonParts(2).Replace("""", ""))

                                result.Add(ParseComparison(parsedComparison))
                                currentComparison.Clear()
                                state = ParsingState.Normal
                                negationFound = False
                                result.Add(")")
                            End If
                        Else
                            ' Acumular caracteres para la comparación
                            currentComparison.Append(currentChar)
                        End If

                    Case ParsingState.InLogicOperator
                        If currentChar = ","c Then
                            ' Finalizar operador lógico
                            Dim logicOp As String = currentLogicOperator.ToString().Replace("""", "").ToUpper()
                            result.Add($" {logicOp} ")
                            currentLogicOperator.Clear()
                            state = ParsingState.Normal
                        Else
                            ' Acumular caracteres para el operador lógico
                            currentLogicOperator.Append(currentChar)
                        End If

                    Case ParsingState.ProcessingNegation
                        ' Ignorar caracteres específicos durante el procesamiento de negación
                        If Not (currentChar = "!"c OrElse currentChar = ","c OrElse currentChar = """"c) Then
                            state = ParsingState.Normal
                            i -= 1  ' Retroceder para procesar este carácter en el estado normal
                        End If
                        If currentChar = ","c Then
                            state = ParsingState.Normal
                        End If
                End Select
            Next

            Return result
        End Function

        ''' <summary>
        ''' Maneja el estado normal del análisis de la expresión de filtro
        ''' </summary>
        Private Sub HandleNormalState(currentChar As Char, position As Integer, filterExpression As String,
                             ByRef state As ParsingState, ByRef result As List(Of Object),
                             ByRef negationFound As Boolean, ByRef currentComparison As StringBuilder)

            Select Case currentChar
                Case "["c
                    ' Inicio de un grupo o comparación
                    result.Add("(")

                Case "]"c
                    ' Cierre de grupo (sin comparación activa)
                    result.Add(")")

                Case """"c
                    ' Posible inicio de comparación o parte de ella
                    If position > 0 AndAlso filterExpression(position - 1) = "["c Then
                        ' Verificar si es una negación: ["!"]
                        If position + 2 < filterExpression.Length AndAlso
                            filterExpression(position + 1) = "!"c AndAlso
                            filterExpression(position + 2) = """"c Then

                            result.Add("NOT")
                            negationFound = True
                            state = ParsingState.ProcessingNegation
                        Else
                            ' Inicio de una comparación normal
                            state = ParsingState.InComparison
                            currentComparison.Append(currentChar)
                        End If
                    Else
                        ' Parte de otra expresión, simplemente agregar
                        result.Add(currentChar.ToString())
                    End If

                Case ","c
                    ' Posible inicio de un operador lógico
                    If negationFound Then
                        state = ParsingState.ProcessingNegation
                    Else
                        state = ParsingState.InLogicOperator
                    End If

                Case Else
                    ' Para otros caracteres, solo agregarlos si no son espacios en comparación
                    If currentChar <> " "c Then
                        result.Add(currentChar.ToString())
                    End If
            End Select
        End Sub


        Private Function ParseComparison(comparison As Comparison) As Comparison

            If comparison.Operand2.ToUpper = "ANNIVERSARY" Then
                comparison.ComparisonOperator = comparison.Operand2
                comparison.Operand2 = String.Empty
            End If

            Select Case comparison.ComparisonOperator.ToUpper()
                Case "CONTAINS", "NOTCONTAINS"
                    comparison.Operand2 = $"%{comparison.Operand2}%"
                Case "STARTSWITH"
                    comparison.Operand2 = $"{comparison.Operand2}%"
                Case "ENDSWITH"
                    comparison.Operand2 = $"%{comparison.Operand2}"
            End Select

            Select Case comparison.ComparisonOperator.ToUpper()
                Case "CONTAINS", "STARTSWITH", "ENDSWITH"
                    comparison.ComparisonOperator = "LIKE"
                Case "NOTCONTAINS"
                    comparison.ComparisonOperator = "NOT LIKE"
            End Select

            Return comparison

        End Function
#End Region

    End Class


End Namespace
