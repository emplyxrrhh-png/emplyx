Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace VTRules
    Public Class roRulesManager

        Private oState As roRulesState = Nothing

        Public ReadOnly Property State As roRulesState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"
        Public Sub New()
            oState = New roRulesState()
        End Sub

        Public Sub New(ByVal _State As roRulesState)
            oState = _State
        End Sub
#End Region

#Region "DUMMY"
        Public Function GetDummyRulesGroups() As List(Of roRulesGroup)
            Dim sql As String = "@SELECT# DummyData FROM TMPDummyRulesGroups"
            Dim jsonRulesGroups As String = DataLayer.AccessHelper.ExecuteScalar(sql).ToString()

            'Dim rulesGroupsbis As List(Of roRulesGroup) = GetAllRulesGroups(, True, True)

            Dim rulesGroups As List(Of roRulesGroup) = roJSONHelper.Deserialize(Of List(Of roRulesGroup))(jsonRulesGroups)

            'SaveRulesGroups(rulesGroups, True, True)

            Return rulesGroups
        End Function

#End Region

        Public Function GetRulesByShift(ByVal idShift As Integer) As List(Of roRule)
            Throw New NotImplementedException("GetRulesByShiftAndDate")
        End Function
#Region "Métodos públicos"
        ''' <summary>
        ''' Guarda una lista de grupos de reglas, procesando las operaciones CRUD según el EditionStatus
        ''' </summary>
        ''' <param name="rulesGroups">Lista de grupos de reglas a procesar</param>
        ''' <param name="auditGroups">Indica si se debe auditar la operación para los grupos</param>
        ''' <param name="auditRules">Indica si se debe auditar la operación para las reglas</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Public Function SaveRulesGroups(ByRef rulesGroups As List(Of roRulesGroup),
                                      Optional auditGroups As Boolean = False,
                                      Optional auditRules As Boolean = False) As Boolean
            Dim result As Boolean = True
            Dim haveToClose As Boolean = False

            Try
                ' Inicializar transacción
                haveToClose = AccessHelper.StartTransaction()

                ' Procesamiento de los grupos de reglas
                For Each group In rulesGroups
                    ' Procesar el grupo según corresponda
                    If Not ProcessRulesGroup(group, auditGroups) Then
                        result = False
                        Exit For
                    End If

                    ' Si el grupo no está marcado para eliminación, procesamos sus reglas
                    If group.EditionStatus <> ItemEditionStatus.Deleted Then
                        ' Procesar las reglas del grupo
                        For Each rule In group.Rules
                            If Not ProcessRule(rule, group.Id, auditRules) Then
                                result = False
                                Exit For
                            End If

                            ' Si la regla no está marcada para eliminación, procesamos su historial
                            If rule.EditionStatus <> ItemEditionStatus.Deleted AndAlso rule.RuleDefinitions IsNot Nothing Then
                                For Each historyEntry In rule.RuleDefinitions
                                    If Not ProcessRuleHistoryEntry(historyEntry, rule.Id, auditRules) Then
                                        result = False
                                        Exit For
                                    End If
                                Next
                            End If
                        Next
                    End If

                    ' Si ha habido algún error, salimos del bucle
                    If Not result Then Exit For
                Next
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesGroupManager::SaveRulesGroups")
            Finally
                ' Finalizar transacción
                AccessHelper.EndCurrentTransaction(haveToClose, result)
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Obtiene un grupo de reglas por su Id
        ''' </summary>
        ''' <param name="groupId">Id del grupo de reglas</param>
        ''' <param name="loadRules">Indica si se deben cargar las reglas asociadas al grupo</param>
        ''' <param name="loadHistory">Indica si se deben cargar las entradas históricas de las reglas</param>
        ''' <returns>El grupo de reglas si existe, null en caso contrario</returns>
        Public Function GetRulesGroupById(groupId As Integer,
                                        Optional loadRules As Boolean = True,
                                        Optional loadHistory As Boolean = True) As roRulesGroup
            Dim rulesGroup As roRulesGroup = Nothing

            Try
                ' Obtener el grupo
                Dim sqlGroup As String = $"@SELECT# [Id], [Name] FROM [RulesGroups] WHERE [Id] = {groupId}"
                Dim dtGroup As DataTable = AccessHelper.CreateDataTable(sqlGroup)

                If dtGroup IsNot Nothing AndAlso dtGroup.Rows.Count > 0 Then
                    rulesGroup = New roRulesGroup()
                    rulesGroup.Id = roTypes.Any2Integer(dtGroup.Rows(0)("Id"))
                    rulesGroup.Name = roTypes.Any2String(dtGroup.Rows(0)("Name"))
                    rulesGroup.Rules = New List(Of roRule)()

                    ' Cargar reglas si es necesario
                    If loadRules Then
                        rulesGroup.Rules = GetRulesByGroupId(groupId, loadHistory)
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesGroupManager::GetRulesGroupById")
            End Try

            Return rulesGroup
        End Function

        ''' <summary>
        ''' Obtiene todos los grupos de reglas
        ''' </summary>
        ''' <param name="filter">Filtro opcional para los grupos</param>
        ''' <param name="loadRules">Indica si se deben cargar las reglas asociadas a los grupos</param>
        ''' <param name="loadHistory">Indica si se deben cargar las entradas históricas de las reglas</param>
        ''' <returns>Lista de grupos de reglas</returns>
        Public Function GetAllRulesGroups(Optional filter As RulesGroupFilter = Nothing,
                                        Optional loadRules As Boolean = False,
                                        Optional loadHistory As Boolean = False) As List(Of roRulesGroup)
            Dim rulesGroups As New List(Of roRulesGroup)()

            Try
                ' Construir la consulta SQL con filtros opcionales
                Dim whereClause As String = String.Empty
                If filter IsNot Nothing AndAlso Not String.IsNullOrEmpty(filter.Name) Then
                    whereClause = $" WHERE [Name] LIKE '%{filter.Name.Replace("'", "''")}%'"
                End If

                ' Obtener los grupos
                Dim sqlGroups As String = $"@SELECT# [Id], [Name] FROM [RulesGroups]{whereClause} ORDER BY [Name]"
                Dim dtGroups As DataTable = AccessHelper.CreateDataTable(sqlGroups)

                If dtGroups IsNot Nothing AndAlso dtGroups.Rows.Count > 0 Then
                    For Each row As DataRow In dtGroups.Rows
                        Dim group As New roRulesGroup()
                        group.Id = roTypes.Any2Integer(row("Id"))
                        group.Name = roTypes.Any2String(row("Name"))
                        group.Rules = New List(Of roRule)()

                        ' Cargar reglas si es necesario
                        If loadRules Then
                            group.Rules = GetRulesByGroupId(group.Id, loadHistory)
                        End If

                        rulesGroups.Add(group)
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesGroupManager::GetAllRulesGroups")
            End Try

            Return rulesGroups
        End Function

        ''' <summary>
        ''' Elimina un grupo de reglas y todas sus reglas asociadas
        ''' </summary>
        ''' <param name="groupId">Id del grupo a eliminar</param>
        ''' <param name="auditResult">Indica si se debe auditar la operación</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Public Function DeleteRulesGroup(groupId As Integer, Optional auditResult As Boolean = False) As Boolean
            Dim result As Boolean = True
            Dim haveToClose As Boolean = False

            Try
                ' Inicializar transacción
                haveToClose = AccessHelper.StartTransaction()

                ' Obtener el nombre del grupo para auditoría
                Dim groupName As String = String.Empty
                If result Then
                    Dim sqlName As String = $"@SELECT# [Name] FROM [RulesGroups] WHERE [Id] = {groupId}"
                    groupName = roTypes.Any2String(AccessHelper.ExecuteScalar(sqlName))
                End If

                ' Eliminar todas las entradas históricas de las reglas del grupo
                Dim sqlDeleteHistory As String = $"@DELETE# [RulesDefinitions] WHERE [IdRule] IN (SELECT [Id] FROM [Rules] WHERE [GroupId] = {groupId})"
                result = AccessHelper.ExecuteSql(sqlDeleteHistory)

                ' Eliminar todas las reglas del grupo
                If result Then
                    Dim sqlDeleteRules As String = $"@DELETE# [Rules] WHERE [GroupId] = {groupId}"
                    result = AccessHelper.ExecuteSql(sqlDeleteRules)
                End If

                ' Eliminar el grupo
                If result Then
                    Dim sqlDeleteGroup As String = $"@DELETE# [RulesGroups] WHERE [Id] = {groupId}"
                    result = AccessHelper.ExecuteSql(sqlDeleteGroup)
                End If

                ' Auditar la operación si es necesario
                If result AndAlso auditResult AndAlso Not String.IsNullOrEmpty(groupName) Then
                    ' Implementar auditoría
                End If
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesGroupManager::DeleteRulesGroup")
            Finally
                ' Finalizar transacción
                AccessHelper.EndCurrentTransaction(haveToClose, result)
            End Try

            Return result
        End Function
#End Region

#Region "Métodos privados"
        ''' <summary>
        ''' Obtiene las reglas asociadas a un grupo
        ''' </summary>
        ''' <param name="groupId">Id del grupo</param>
        ''' <param name="loadHistory">Indica si se deben cargar las entradas históricas</param>
        ''' <returns>Lista de reglas del grupo</returns>
        Private Function GetRulesByGroupId(groupId As Integer, Optional loadHistory As Boolean = True) As List(Of roRule)
            Dim rules As New List(Of roRule)()

            Try
                ' Obtener las reglas del grupo
                Dim sqlRules As String = $"@SELECT# [Id], [Name], [Description], [Type], [CreatedDate], [ModifiedDate] FROM [Rules] WHERE [GroupId] = {groupId} ORDER BY [Name]"
                Dim dtRules As DataTable = AccessHelper.CreateDataTable(sqlRules)

                If dtRules IsNot Nothing AndAlso dtRules.Rows.Count > 0 Then
                    For Each row As DataRow In dtRules.Rows
                        Dim rule As New roRule()
                        rule.Id = roTypes.Any2Integer(row("Id"))
                        rule.Name = roTypes.Any2String(row("Name"))
                        rule.Description = roTypes.Any2String(row("Description"))
                        rule.Type = CType(roTypes.Any2Integer(row("Type")), eRuleType)
                        rule.GroupId = groupId
                        rule.CreatedDate = roTypes.Any2DateTime(row("CreatedDate"))
                        rule.ModifiedDate = roTypes.Any2DateTime(row("ModifiedDate"))
                        rule.Tags = New List(Of String)()
                        rule.RuleDefinitions = New List(Of roRuleDefinition)()
                        LoadRuleTags(rule)

                        ' Cargar entradas históricas si es necesario
                        If loadHistory Then
                            rule.RuleDefinitions = GetRuleHistoryEntries(rule.Id)
                        End If

                        rules.Add(rule)
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesGroupManager::GetRulesByGroupId")
            End Try

            Return rules
        End Function

        ''' <summary>
        ''' Obtiene las entradas históricas de una regla
        ''' </summary>
        ''' <param name="ruleId">Id de la regla</param>
        ''' <returns>Lista de entradas históricas</returns>
        Private Function GetRuleHistoryEntries(ruleId As Integer) As List(Of roRuleDefinition)
            Dim historyEntries As New List(Of roRuleDefinition)()

            Try
                ' Obtener las entradas históricas
                Dim sqlHistory As String = $"@SELECT# 
                                                [Id], 
                                                [IdRule], 
                                                [Description], 
                                                [XmlDefinition], 
                                                [ModifiedBy], 
                                                [ModifiedDate], 
                                                [ChangeType],
                                                [EffectiveFrom],
                                                [EmployeeContext],
                                                LEAD([EffectiveFrom], 1) OVER(ORDER BY [EffectiveFrom]) AS NextEffectiveFrom
                                            FROM [RulesDefinitions] 
                                            WHERE [IdRule] = {ruleId} 
                                            ORDER BY [EffectiveFrom]"
                Dim dtHistory As DataTable = AccessHelper.CreateDataTable(sqlHistory)

                If dtHistory IsNot Nothing AndAlso dtHistory.Rows.Count > 0 Then
                    For Each row As DataRow In dtHistory.Rows
                        Dim historyEntry As New roRuleDefinition()
                        historyEntry.Id = roTypes.Any2Integer(row("Id"))
                        historyEntry.IdRule = ruleId
                        historyEntry.Description = roTypes.Any2String(row("Description"))
                        historyEntry.XmlDefinition = roTypes.Any2String(row("XmlDefinition"))
                        historyEntry.EmployeeContext = roJSONHelper.Deserialize(Of roSelectorFilter)((row("EmployeeContext")))
                        historyEntry.ModifiedBy = roTypes.Any2String(row("ModifiedBy"))
                        historyEntry.ModifiedDate = roTypes.Any2DateTime(row("ModifiedDate"))
                        historyEntry.EffectiveFrom = roTypes.Any2DateTime(row("EffectiveFrom"))
                        historyEntry.ChangeType = CType(roTypes.Any2Integer(row("ChangeType")), eRuleChangeType)

                        ' Calcular EffectiveUntil usando el valor de NextEffectiveFrom
                        If Not row.IsNull("NextEffectiveFrom") Then
                            Dim nextEffectiveFrom As DateTime = roTypes.Any2DateTime(row("NextEffectiveFrom"))
                            historyEntry.EffectiveUntil = nextEffectiveFrom.AddDays(-1)
                        End If

                        ' Cargar los turnos asociados a esta entrada histórica
                        historyEntry.Shifts = GetShiftsForHistoryEntry(historyEntry.Id)

                        historyEntries.Add(historyEntry)
                    Next

                    ' Establecer EffectiveUntil para cada entrada histórica
                    For i As Integer = 0 To historyEntries.Count - 2
                        historyEntries(i).EffectiveUntil = historyEntries(i + 1).EffectiveFrom.AddDays(-1)
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesGroupManager::GetRuleHistoryEntries")
            End Try

            Return historyEntries
        End Function

        ''' <summary>
        ''' Obtiene los turnos asociados a una entrada histórica
        ''' </summary>
        ''' <param name="historyEntryId">Id de la entrada histórica</param>
        ''' <returns>Lista de turnos asociados</returns>
        Private Function GetShiftsForHistoryEntry(historyEntryId As Integer) As List(Of roShiftItem)
            Dim shifts As New List(Of roShiftItem)()

            Try
                ' Consulta SQL para obtener los turnos de una entrada histórica
                Dim sqlShifts As String = $"@SELECT# 
                                                rs.[IdShift], 
                                                rs.[Order],
                                                s.[ShortName] AS ShiftName,
                                                sg.[Id] AS IdShiftGroup,
                                                sg.[Name] AS ShiftGroupName
                                            FROM [RulesShifts] rs
                                            LEFT JOIN [Shifts] s ON rs.[IdShift] = s.[Id]
                                            LEFT JOIN [ShiftGroups] sg ON s.[IdGroup] = sg.[Id]
                                            WHERE rs.[IdRuleDefinition] = {historyEntryId}
                                            ORDER BY rs.[Order]"

                Dim dtShifts As DataTable = AccessHelper.CreateDataTable(sqlShifts)

                If dtShifts IsNot Nothing AndAlso dtShifts.Rows.Count > 0 Then
                    For Each row As DataRow In dtShifts.Rows
                        Dim shiftItem As New roShiftItem()
                        shiftItem.IdShift = roTypes.Any2Integer(row("IdShift"))
                        shiftItem.ShiftName = roTypes.Any2String(row("ShiftName"))
                        shiftItem.IdShiftGroup = If(row.IsNull("IdShiftGroup"), 0, roTypes.Any2Integer(row("IdShiftGroup")))
                        shiftItem.ShiftGroupName = roTypes.Any2String(row("ShiftGroupName"))
                        shiftItem.Order = roTypes.Any2Integer(row("Order"))
                        shiftItem.EditionStatus = ItemEditionStatus.NotEdited

                        shifts.Add(shiftItem)
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesGroupManager::GetShiftsForHistoryEntry")
            End Try

            Return shifts
        End Function

        ''' <summary>
        ''' Procesa un grupo de reglas según su EditionStatus
        ''' </summary>
        ''' <param name="group">Grupo de reglas a procesar</param>
        ''' <param name="audit">Indica si se debe auditar la operación</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function ProcessRulesGroup(ByRef group As roRulesGroup, Optional auditResult As Boolean = False) As Boolean
            Dim result As Boolean = True

            Try
                Select Case group.EditionStatus
                    Case ItemEditionStatus.New
                        ' Crear nuevo grupo
                        Dim sqlInsert As String = "@INSERT# INTO [RulesGroups] ([Name]) OUTPUT INSERTED.ID VALUES (@Name)"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@Name", CommandParameter.ParameterType.tString, group.Name))

                        Dim newId As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlInsert, parameters))
                        If newId > 0 Then
                            group.Id = newId
                        Else
                            result = False
                        End If

                    Case ItemEditionStatus.Edited
                        ' Actualizar grupo existente
                        Dim sqlUpdate As String = "@UPDATE# [RulesGroups] SET [Name] = @Name WHERE [Id] = @Id"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@Name", CommandParameter.ParameterType.tString, group.Name))
                        parameters.Add(New CommandParameter("@Id", CommandParameter.ParameterType.tInt, group.Id))

                        result = AccessHelper.ExecuteSql(sqlUpdate, parameters)

                    Case ItemEditionStatus.Deleted
                        ' Eliminar grupo
                        result = DeleteRulesGroup(group.Id, auditResult)

                    Case ItemEditionStatus.NotEdited
                        ' No hacer nada
                End Select

                If result AndAlso auditResult Then
                    ' TODO: Implementar auditoría
                End If
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesGroupManager::ProcessRulesGroup")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Procesa una regla según su EditionStatus
        ''' </summary>
        ''' <param name="rule">Regla a procesar</param>
        ''' <param name="groupId">Id del grupo al que pertenece la regla</param>
        ''' <param name="auditResult">Indica si se debe auditar la operación</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function ProcessRule(ByRef rule As roRule, groupId As Integer, Optional auditResult As Boolean = False) As Boolean
            Dim result As Boolean = True

            Try
                Select Case rule.EditionStatus
                    Case ItemEditionStatus.New
                        ' Crear nueva regla
                        Dim sqlInsert As String = "@INSERT# INTO [Rules] ([Name], [Description], [Type], [GroupId], [CreatedDate], [ModifiedDate]) " &
                                                "OUTPUT INSERTED.ID VALUES (@Name, @Description, @Type, @GroupId, @CreatedDate, @ModifiedDate)"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@Name", CommandParameter.ParameterType.tString, rule.Name))
                        parameters.Add(New CommandParameter("@Description", CommandParameter.ParameterType.tString, rule.Description))
                        parameters.Add(New CommandParameter("@Type", CommandParameter.ParameterType.tInt, CInt(rule.Type)))
                        parameters.Add(New CommandParameter("@GroupId", CommandParameter.ParameterType.tInt, groupId))
                        parameters.Add(New CommandParameter("@CreatedDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))
                        parameters.Add(New CommandParameter("@ModifiedDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))

                        Dim newId As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlInsert, parameters))
                        If newId > 0 Then
                            rule.Id = newId
                            rule.GroupId = groupId

                            ' Guardar tags si existen
                            If rule.Tags IsNot Nothing AndAlso rule.Tags.Count > 0 Then
                                result = SaveRuleTags(rule)
                            End If
                        Else
                            result = False
                        End If

                    Case ItemEditionStatus.Edited
                        ' Actualizar regla existente
                        Dim sqlUpdate As String = "@UPDATE# [Rules] SET [Name] = @Name, [Description] = @Description, " &
                                                "[Type] = @Type, [ModifiedDate] = @ModifiedDate WHERE [Id] = @Id"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@Name", CommandParameter.ParameterType.tString, rule.Name))
                        parameters.Add(New CommandParameter("@Description", CommandParameter.ParameterType.tString, rule.Description))
                        parameters.Add(New CommandParameter("@Type", CommandParameter.ParameterType.tInt, CInt(rule.Type)))
                        parameters.Add(New CommandParameter("@ModifiedDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))
                        parameters.Add(New CommandParameter("@Id", CommandParameter.ParameterType.tInt, rule.Id))

                        result = AccessHelper.ExecuteSql(sqlUpdate, parameters)

                        ' Actualizar tags si es necesario
                        If result AndAlso rule.Tags IsNot Nothing Then
                            result = SaveRuleTags(rule)
                        End If
                    Case ItemEditionStatus.Deleted
                        ' Eliminar regla
                        ' Primero eliminar entradas históricas
                        Dim sqlDeleteHistory As String = $"@DELETE# [RulesDefinitions] WHERE [IdRule] = {rule.Id}"
                        result = AccessHelper.ExecuteSql(sqlDeleteHistory)

                        ' Luego eliminar la regla
                        If result Then
                            Dim sqlDeleteRule As String = $"@DELETE# [Rules] WHERE [Id] = {rule.Id}"
                            result = AccessHelper.ExecuteSql(sqlDeleteRule)
                        End If
                    Case ItemEditionStatus.NotEdited
                        ' No hacer nada
                End Select

                If result AndAlso auditResult Then
                    ' TODO: Implementar auditoría
                End If
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesGroupManager::ProcessRule")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Procesa una entrada histórica según su EditionStatus
        ''' </summary>
        ''' <param name="historyEntry">Entrada histórica a procesar</param>
        ''' <param name="ruleId">Id de la regla a la que pertenece la entrada</param>
        ''' <param name="auditResult">Indica si se debe auditar la operación</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function ProcessRuleHistoryEntry(ByRef historyEntry As roRuleDefinition, ruleId As Integer, Optional auditResult As Boolean = False) As Boolean
            Dim result As Boolean = True

            Try
                Select Case historyEntry.EditionStatus
                    Case ItemEditionStatus.New
                        ' Crear nueva entrada histórica
                        Dim sqlInsert As String = "@INSERT# INTO [RulesDefinitions] ([IdRule], [Description], [XmlDefinition], [EmployeeContext], [ModifiedBy], [ModifiedDate], [ChangeType], [EffectiveFrom]) " &
                                                "OUTPUT INSERTED.ID VALUES (@IdRule, @Description, @XmlDefinition, @EmployeeContext, @ModifiedBy, @ModifiedDate, @ChangeType, @EffectiveFrom)"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@IdRule", CommandParameter.ParameterType.tInt, ruleId))
                        parameters.Add(New CommandParameter("@Description", CommandParameter.ParameterType.tString, historyEntry.Description))
                        parameters.Add(New CommandParameter("@XmlDefinition", CommandParameter.ParameterType.tString, historyEntry.XmlDefinition))
                        parameters.Add(New CommandParameter("@EmployeeContext", CommandParameter.ParameterType.tString, roJSONHelper.Serialize(historyEntry.EmployeeContext)))
                        parameters.Add(New CommandParameter("@ModifiedBy", CommandParameter.ParameterType.tString, historyEntry.ModifiedBy))
                        parameters.Add(New CommandParameter("@ModifiedDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))
                        parameters.Add(New CommandParameter("@ChangeType", CommandParameter.ParameterType.tInt, historyEntry.ChangeType))
                        parameters.Add(New CommandParameter("@EffectiveFrom", CommandParameter.ParameterType.tDateTime, historyEntry.EffectiveFrom))

                        Dim newId As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlInsert, parameters))
                        If newId > 0 Then
                            historyEntry.Id = newId
                            historyEntry.IdRule = ruleId

                            ' Procesar la lista de roShiftItem
                            If result Then
                                result = ProcessShiftsForHistoryEntry(historyEntry)
                            End If
                        Else
                            result = False
                        End If

                    Case ItemEditionStatus.Edited
                        ' Actualizar entrada histórica existente
                        Dim sqlUpdate As String = "@UPDATE# [RulesDefinitions] SET [Description] = @Description, [XmlDefinition] = @XmlDefinition, [EmployeeContext] = @EmployeeContext, " &
                                                "[ModifiedBy] = @ModifiedBy, [ModifiedDate] = @ModifiedDate, [ChangeType] = @ChangeType , [EffectiveFrom] = @EffectiveFrom WHERE [Id] = @Id"
                        Dim parameters As New List(Of CommandParameter)()
                        parameters.Add(New CommandParameter("@Description", CommandParameter.ParameterType.tString, historyEntry.Description))
                        parameters.Add(New CommandParameter("@XmlDefinition", CommandParameter.ParameterType.tString, historyEntry.XmlDefinition))
                        parameters.Add(New CommandParameter("@EmployeeContext", CommandParameter.ParameterType.tString, roJSONHelper.Serialize(historyEntry.EmployeeContext)))
                        parameters.Add(New CommandParameter("@ModifiedBy", CommandParameter.ParameterType.tString, historyEntry.ModifiedBy))
                        parameters.Add(New CommandParameter("@ModifiedDate", CommandParameter.ParameterType.tDateTime, DateTime.Now))
                        parameters.Add(New CommandParameter("@ChangeType", CommandParameter.ParameterType.tInt, historyEntry.ChangeType))
                        parameters.Add(New CommandParameter("@EffectiveFrom", CommandParameter.ParameterType.tDateTime, historyEntry.EffectiveFrom))
                        parameters.Add(New CommandParameter("@Id", CommandParameter.ParameterType.tInt, historyEntry.Id))

                        result = AccessHelper.ExecuteSql(sqlUpdate, parameters)

                        ' Procesar la lista de roShiftItem
                        If result Then
                            result = ProcessShiftsForHistoryEntry(historyEntry)
                        End If
                    Case ItemEditionStatus.Deleted
                        ' Procesar la lista de roShiftItem
                        If result Then
                            result = ProcessShiftsForHistoryEntry(historyEntry)
                        End If

                        ' Eliminar entrada histórica
                        Dim sqlDelete As String = $"@DELETE# [RulesDefinitions] WHERE [Id] = {historyEntry.Id}"
                        result = AccessHelper.ExecuteSql(sqlDelete)
                    Case ItemEditionStatus.NotEdited
                        ' No hacer nada
                End Select

                If result AndAlso auditResult Then
                    ' TODO: Implementar auditoría
                End If
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesGroupManager::ProcessRuleHistoryEntry")
            End Try

            Return result
        End Function

        ' Procesa la lista de roShiftItem de una entrada histórica
        Private Function ProcessShiftsForHistoryEntry(historyEntry As roRuleDefinition) As Boolean
            Dim result As Boolean = True
            Try
                If historyEntry.Shifts IsNot Nothing Then
                    For Each shiftItem In historyEntry.Shifts
                        Select Case shiftItem.EditionStatus
                            Case ItemEditionStatus.New
                                result = InsertShiftItem(historyEntry.Id, shiftItem)
                            Case ItemEditionStatus.Edited
                                result = UpdateShiftItem(shiftItem)
                            Case ItemEditionStatus.Deleted
                                result = DeleteShiftItem(shiftItem.IdShift, historyEntry.Id)
                        End Select
                        If Not result Then Exit For
                    Next
                End If
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesManager::ProcessShiftsForHistoryEntry")
            End Try
            Return result
        End Function

        ' Métodos auxiliares para insertar, actualizar y eliminar roShiftItem
        Private Function InsertShiftItem(historyEntryId As Integer, shiftItem As roShiftItem) As Boolean
            Dim sql As String = "@INSERT# INTO [RulesShifts] ([IdRuleDefinition], [IdShift], [Order]) VALUES (@IdRuleDefinition, @IdShift, @Order)"
            Dim parameters As New List(Of CommandParameter) From {
                New CommandParameter("@IdRuleDefinition", CommandParameter.ParameterType.tInt, historyEntryId),
                New CommandParameter("@IdShift", CommandParameter.ParameterType.tInt, shiftItem.IdShift),
                New CommandParameter("@Order", CommandParameter.ParameterType.tInt, shiftItem.Order)
            }
            Return AccessHelper.ExecuteSql(sql, parameters)
        End Function

        Private Function UpdateShiftItem(shiftItem As roShiftItem) As Boolean
            Dim sql As String = "@UPDATE# [RulesShifts] SET [ShiftName]=@ShiftName, [IdShiftGroup]=@IdShiftGroup, [ShiftGroupName]=@ShiftGroupName, [Order]=@Order WHERE [IdShift]=@IdShift"
            Dim parameters As New List(Of CommandParameter) From {
                New CommandParameter("@ShiftName", CommandParameter.ParameterType.tString, shiftItem.ShiftName),
                New CommandParameter("@IdShiftGroup", CommandParameter.ParameterType.tInt, shiftItem.IdShiftGroup),
                New CommandParameter("@ShiftGroupName", CommandParameter.ParameterType.tString, shiftItem.ShiftGroupName),
                New CommandParameter("@Order", CommandParameter.ParameterType.tInt, shiftItem.Order),
                New CommandParameter("@IdShift", CommandParameter.ParameterType.tInt, shiftItem.IdShift)
            }
            Return AccessHelper.ExecuteSql(sql, parameters)
        End Function

        Private Function DeleteShiftItem(idShift As Integer, historyEntryId As Integer) As Boolean
            Dim sql As String = "@DELETE# FROM [RuleShifts] WHERE [IdShift]=@IdShift AND [IdRuleDefinition]=@IdRuleDefinition"
            Dim parameters As New List(Of CommandParameter) From {
                New CommandParameter("@IdShift", CommandParameter.ParameterType.tInt, idShift),
                New CommandParameter("@IdRuleDefinition", CommandParameter.ParameterType.tInt, historyEntryId)
            }
            Return AccessHelper.ExecuteSql(sql, parameters)
        End Function

        ''' <summary>
        ''' Carga los tags de una regla desde la base de datos
        ''' </summary>
        ''' <param name="rule">Regla a la que cargar los tags</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function LoadRuleTags(ByRef rule As roRule) As Boolean
            Dim result As Boolean = True

            Try
                rule.Tags = GetRuleTagsFromDatabase(rule.Id)
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesManager::LoadRuleTags")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Obtiene los tags actuales de una regla desde la base de datos
        ''' </summary>
        ''' <param name="ruleId">Id de la regla</param>
        ''' <returns>Lista de tags asociados a la regla</returns>
        Private Function GetRuleTagsFromDatabase(ByVal ruleId As Integer) As List(Of String)
            Dim tags As New List(Of String)()

            Try
                ' Consulta SQL para obtener los tags actuales
                Dim sql As String = "
                @SELECT# t.Name 
                FROM [dbo].[EntityTags] et 
                INNER JOIN [dbo].[Tags] t ON et.IdTag = t.Id 
                WHERE et.EntityType = 'Rule' AND et.EntityId = @RuleId"

                Dim parameters As New List(Of CommandParameter)()
                parameters.Add(New CommandParameter("@RuleId", CommandParameter.ParameterType.tInt, ruleId))

                Dim dt As DataTable = AccessHelper.CreateDataTable(sql, parameters)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each row As DataRow In dt.Rows
                        tags.Add(roTypes.Any2String(row("Name")))
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesManager::GetRuleTagsFromDatabase")
            End Try

            Return tags
        End Function

        ''' <summary>
        ''' Guarda los tags asociados a una regla
        ''' </summary>
        ''' <param name="rule">Regla cuyos tags se deben guardar</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function SaveRuleTags(ByVal rule As roRule) As Boolean
            Dim result As Boolean = True

            Try
                ' Si no hay tags, no hay nada que hacer
                If rule.Tags Is Nothing Then
                    Return True
                End If

                ' Primero obtenemos los tags actuales para poder comparar y determinar cuáles añadir o eliminar
                Dim currentTags As List(Of String) = GetRuleTagsFromDatabase(rule.Id)

                ' Tags a añadir: los que están en la regla pero no en la base de datos
                Dim tagsToAdd As List(Of String) = rule.Tags.Where(Function(t) Not currentTags.Contains(t)).ToList()

                ' Tags a eliminar: los que están en la base de datos pero no en la regla
                Dim tagsToRemove As List(Of String) = currentTags.Where(Function(t) Not rule.Tags.Contains(t)).ToList()

                ' Eliminar tags que ya no están asociados a la regla
                For Each tagToRemove In tagsToRemove
                    If Not DeleteRuleTag(rule.Id, tagToRemove) Then
                        result = False
                        Exit For
                    End If
                Next

                ' Añadir nuevos tags
                For Each tagToAdd In tagsToAdd
                    If Not AddRuleTag(rule.Id, tagToAdd) Then
                        result = False
                        Exit For
                    End If
                Next
            Catch ex As Exception
                result = False
                oState.UpdateStateInfo(ex, "roRulesManager::SaveRuleTags")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Añade un tag a una regla
        ''' </summary>
        ''' <param name="ruleId">Id de la regla</param>
        ''' <param name="tagName">Nombre del tag</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function AddRuleTag(ByVal ruleId As Integer, ByVal tagName As String) As Boolean
            Dim result As Boolean = False

            Try
                ' Primero verificamos si el tag existe, si no, lo creamos
                Dim tagId As Integer = GetOrCreateTagId(tagName)

                If tagId > 0 Then
                    ' Verificar si ya existe la relación entre la regla y el tag
                    Dim sqlCheck As String = "
                    @SELECT# COUNT(*) 
                    FROM [dbo].[EntityTags] 
                    WHERE EntityType = 'Rule' AND EntityId = @RuleId AND IdTag = @TagId"

                    Dim checkParams As New List(Of CommandParameter)()
                    checkParams.Add(New CommandParameter("@RuleId", CommandParameter.ParameterType.tInt, ruleId))
                    checkParams.Add(New CommandParameter("@TagId", CommandParameter.ParameterType.tInt, tagId))

                    Dim exists As Boolean = (AccessHelper.ExecuteScalar(sqlCheck, checkParams) > 0)

                    If Not exists Then
                        ' Insertar la relación entre la regla y el tag
                        Dim sqlInsert As String = "
                        @INSERT# INTO [dbo].[EntityTags] (IdTag, EntityType, EntityId) 
                        VALUES (@TagId, 'Rule', @RuleId)"

                        Dim insertParams As New List(Of CommandParameter)()
                        insertParams.Add(New CommandParameter("@TagId", CommandParameter.ParameterType.tInt, tagId))
                        insertParams.Add(New CommandParameter("@RuleId", CommandParameter.ParameterType.tInt, ruleId))

                        result = AccessHelper.ExecuteSql(sqlInsert, insertParams)
                    Else
                        ' Ya existe, consideramos que es exitoso
                        result = True
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesManager::AddRuleTag")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Elimina un tag de una regla
        ''' </summary>
        ''' <param name="ruleId">Id de la regla</param>
        ''' <param name="tagName">Nombre del tag</param>
        ''' <returns>True si la operación fue exitosa, False en caso contrario</returns>
        Private Function DeleteRuleTag(ByVal ruleId As Integer, ByVal tagName As String) As Boolean
            Dim result As Boolean = False

            Try
                ' Primero obtenemos el ID del tag
                Dim sqlGetTagId As String = "@SELECT# Id FROM [dbo].[Tags] WHERE Name = @TagName"
                Dim getTagParams As New List(Of CommandParameter)()
                getTagParams.Add(New CommandParameter("@TagName", CommandParameter.ParameterType.tString, tagName))

                Dim tagId As Object = AccessHelper.ExecuteScalar(sqlGetTagId, getTagParams)

                If tagId IsNot Nothing AndAlso roTypes.Any2Integer(tagId) > 0 Then
                    ' Eliminar la relación entre la regla y el tag
                    Dim sqlDelete As String = "
                    @DELETE# FROM [dbo].[EntityTags] 
                    WHERE EntityType = 'Rule' AND EntityId = @RuleId AND IdTag = @TagId"

                    Dim deleteParams As New List(Of CommandParameter)()
                    deleteParams.Add(New CommandParameter("@RuleId", CommandParameter.ParameterType.tInt, ruleId))
                    deleteParams.Add(New CommandParameter("@TagId", CommandParameter.ParameterType.tInt, roTypes.Any2Integer(tagId)))

                    result = AccessHelper.ExecuteSql(sqlDelete, deleteParams)
                Else
                    ' Si el tag no existe, consideramos que es exitoso
                    result = True
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesManager::DeleteRuleTag")
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Obtiene o crea un tag y devuelve su ID
        ''' </summary>
        ''' <param name="tagName">Nombre del tag</param>
        ''' <returns>ID del tag existente o del nuevo tag creado</returns>
        Private Function GetOrCreateTagId(ByVal tagName As String) As Integer
            Dim tagId As Integer = 0

            Try
                ' Primero intentamos obtener el ID si el tag ya existe
                Dim sqlGetId As String = "@SELECT# Id FROM [dbo].[Tags] WHERE Name = @TagName"
                Dim getIdParams As New List(Of CommandParameter)()
                getIdParams.Add(New CommandParameter("@TagName", CommandParameter.ParameterType.tString, tagName))

                Dim idResult As Object = AccessHelper.ExecuteScalar(sqlGetId, getIdParams)

                If idResult IsNot Nothing Then
                    tagId = roTypes.Any2Integer(idResult)
                Else
                    ' Si no existe, creamos el tag
                    Dim sqlInsert As String = "@INSERT# INTO [dbo].[Tags] (Name) OUTPUT INSERTED.ID VALUES (@TagName)"
                    Dim insertParams As New List(Of CommandParameter)()
                    insertParams.Add(New CommandParameter("@TagName", CommandParameter.ParameterType.tString, tagName))

                    Dim newId As Object = AccessHelper.ExecuteScalar(sqlInsert, insertParams)
                    If newId IsNot Nothing Then
                        tagId = roTypes.Any2Integer(newId)
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRulesManager::GetOrCreateTagId")
            End Try

            Return tagId
        End Function
#End Region
    End Class
End Namespace

