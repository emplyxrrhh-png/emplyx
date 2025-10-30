Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTBots

    Public Class roBotManager

        Private oState As roBotState = Nothing

        Public ReadOnly Property State As roBotState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roBotState()
        End Sub

        Public Sub New(ByVal _State As roBotState)
            oState = _State
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Recover Bot information.
        ''' </summary>
        ''' <param name="idBot"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetBotById(idBot As Integer, Optional bLoadRules As Boolean = True, Optional ByVal bAudit As Boolean = False) As roBot
            Dim retBot As roBot = Nothing
            Dim strSQL As String

            Try

                oState.Result = BotResultEnum.NoError

                Dim tb As DataTable

                strSQL = "@SELECT# [Id],[Name],[Type],[Status],[LastExecutionDate] "
                strSQL = strSQL & " FROM [dbo].[Bots] WHERE Id = " & idBot.ToString & " ORDER BY Name ASC"

                tb = CreateDataTable(strSQL)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    retBot = New roBot
                    retBot.Id = roTypes.Any2Integer(oRow("Id"))
                    retBot.Name = roTypes.Any2String(oRow("Name"))
                    retBot.Type = roTypes.Any2Integer(oRow("Type"))
                    retBot.Status = roTypes.Any2Integer(oRow("Status"))
                    retBot.LastExecutionDate = If(IsDBNull(oRow("LastExecutionDate")), New Date(1970, 1, 1), roTypes.Any2DateTime(oRow("LastExecutionDate")))
                End If

                If retBot IsNot Nothing Then
                    retBot.BotRules = New Generic.List(Of roBotRule)
                    '1.- Cargo Reglas
                    strSQL = "@SELECT# [Id], [IDBot], [Type], [Name],[IDTemplate],[Definition],[IsActive] "
                    strSQL = strSQL & " FROM [dbo].[BotRules] WHERE IDBot = " & idBot.ToString & " ORDER BY Name ASC"
                    tb = CreateDataTable(strSQL)

                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim retBotRule As roBotRule = Nothing
                        Dim lstBotRules As New Generic.List(Of roBotRule)
                        For Each orow As DataRow In tb.Rows
                            retBotRule = Nothing
                            Dim oBotRuleManager As roBotRuleManager = Nothing

                            Select Case orow("Type")
                                Case BotRuleTypeEnum.CopyEmployeePermissions
                                    oBotRuleManager = New roBotRule_Copy_Employee_Permissions
                                Case BotRuleTypeEnum.CopySupervisorPermissions
                                    oBotRuleManager = New roBotRule_Copy_Supervisor_Permissions
                                Case BotRuleTypeEnum.CopyEmployeeUserFields
                                    oBotRuleManager = New roBotRule_Copy_EmployeeUserFields
                                Case BotRuleTypeEnum.CopyCenterCostRole
                                    oBotRuleManager = New roBotRule_Copy_CostCenterRole
                            End Select

                            If oBotRuleManager IsNot Nothing Then retBotRule = oBotRuleManager.GetBotRuleByRow(orow)

                            If retBotRule IsNot Nothing Then
                                lstBotRules.Add(retBotRule)
                            End If
                        Next
                        retBot.BotRules = lstBotRules
                    End If
                End If

                If (oState.Result = BotResultEnum.NoError) AndAlso bAudit Then
                    Dim botName As String = If(retBot IsNot Nothing, retBot.Name, "")

                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{BotName}", botName, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tBot, botName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = BotResultEnum.ErrorRecoveringBot
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetBotById")
            Catch ex As Exception
                oState.Result = BotResultEnum.ErrorRecoveringBot
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetBotById")
            End Try
            Return retBot
        End Function

        ''' <summary>
        ''' Recover bot information
        ''' </summary>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAllBots(Optional ByVal bAudit As Boolean = False, Optional bLoadRules As Boolean = True) As List(Of roBot)
            Dim retBots As New List(Of roBot)

            Try
                oState.Result = BotResultEnum.NoError

                Dim oBot As New roBot
                Dim tb As DataTable

                Dim strSQL As String = "@SELECT# [Id]  "
                strSQL = strSQL & " FROM [dbo].[Bots]  ORDER BY Name ASC"

                tb = CreateDataTable(strSQL)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each orow As DataRow In tb.Rows
                        oBot = GetBotById(orow("ID"), bLoadRules, False)
                        If oBot IsNot Nothing Then retBots.Add(oBot)
                    Next
                End If
            Catch ex As DbException
                oState.Result = BotResultEnum.ErrorRecoveringAllBots
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetAllBots")
            Catch ex As Exception
                oState.Result = BotResultEnum.ErrorRecoveringAllBots
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetAllBots")
            Finally

            End Try
            Return retBots
        End Function

        ''' <summary>
        ''' Creates or update a bot.
        ''' </summary>
        ''' <param name="oBot"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CreateOrUpdateBot(ByRef oBot As roBot, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim iNew As Integer
            Dim iOldStatus As Integer

            Try
                oState.Result = BotResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(oBot) Then
                    oState.Result = BotResultEnum.XSSvalidationError
                    Return False
                End If

                bolIsNew = (oBot.Id = 0)

                ' Verificamos permiso
                Dim featureId As String = "Bots.Definition"

                Dim iPermissionOverBots As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
                If iPermissionOverBots < 6 Then
                    Me.oState.Result = BotResultEnum.NoPermission
                    Return False
                End If

                ' Hay que validar que no exista otro bot con el mismo nombre
                Dim sqlCommand As String = String.Empty

                sqlCommand = "@SELECT# count(*) FROM [dbo].[Bots] WHERE Id <> " & oBot.Id.ToString & " AND Name = '" & oBot.Name.Replace("'", "''") & "'"
                If roConversions.Any2Double(ExecuteScalar(sqlCommand)) > 0 Then
                    Me.oState.Result = BotResultEnum.NameAlreadyExists
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim parameters As New List(Of CommandParameter)

                Dim bStatusChanged As Boolean = False

                If Not bolIsNew Then
                    sqlCommand = $"@UPDATE# Bots SET [Name]=@name, [Status]=@status, " &
                                        " [LastExecutionDate]=@lastexecutiondate, [Type]=@type " &
                                        " OUTPUT DELETED.Status" &
                                        " WHERE [Id] = @id"
                Else
                    sqlCommand = $"@INSERT# INTO Bots ([Name] " &
                                    " ,[Status] " &
                                    " ,[LastExecutionDate] " &
                                    " ,[Type] ) " &
                                    " OUTPUT INSERTED.ID " &
                                    " VALUES (@name, @status, " &
                                    " @lastexecutiondate, @type)"
                End If

                parameters.Add(New CommandParameter("@name", CommandParameter.ParameterType.tString, oBot.Name))
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oBot.Status))
                If bolIsNew Then
                    parameters.Add(New CommandParameter("@lastexecutiondate", CommandParameter.ParameterType.tDateTime, DBNull.Value))
                Else
                    parameters.Add(New CommandParameter("@lastexecutiondate", CommandParameter.ParameterType.tDateTime, oBot.LastExecutionDate))
                    parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oBot.Id))
                End If
                parameters.Add(New CommandParameter("@type", CommandParameter.ParameterType.tInt, oBot.Type))

                Try
                    If bolIsNew Then
                        iNew = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    Else
                        iOldStatus = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                        If iOldStatus <> oBot.Status Then bStatusChanged = True
                    End If
                    bolRet = True
                    If bolIsNew Then oBot.Id = iNew
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roBotManager::ErrorCreatingOrUpdatingBot")
                    oState.Result = BotResultEnum.ErrorCreatingOrUpdatingBot
                End Try

                If (oState.Result = BotResultEnum.NoError) Then
                    ' Guardamos las reglas
                    Dim aRulesToKeep As New List(Of Integer)

                    ' Primero borramos las reglas que ya no estan
                    If oBot.BotRules IsNot Nothing AndAlso oBot.BotRules.Count > 0 Then
                        For Each orule As roBotRule In oBot.BotRules
                            aRulesToKeep.Add(orule.Id)
                        Next
                    End If

                    If aRulesToKeep.Count > 0 Then
                        sqlCommand = "@DELETE# BotRules WHERE IdBot = " & oBot.Id.ToString & " AND Id NOT IN (" & String.Join(",", aRulesToKeep) & ")"
                        bolRet = ExecuteSql(sqlCommand)
                    Else
                        sqlCommand = "@DELETE# BotRules WHERE IdBot = " & oBot.Id.ToString
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    ' Guardamos las actuales
                    If bolRet AndAlso oBot.BotRules IsNot Nothing AndAlso oBot.BotRules.Count > 0 Then
                        Dim oBotRuleManager As roBotRuleManager = Nothing
                        For Each orule As roBotRule In oBot.BotRules
                            bolRet = False
                            orule.IdBot = oBot.Id
                            Select Case orule.Type
                                Case BotRuleTypeEnum.CopyEmployeePermissions
                                    oBotRuleManager = New roBotRule_Copy_Employee_Permissions
                                Case BotRuleTypeEnum.CopySupervisorPermissions
                                    oBotRuleManager = New roBotRule_Copy_Supervisor_Permissions
                                Case BotRuleTypeEnum.CopyEmployeeUserFields
                                    oBotRuleManager = New roBotRule_Copy_EmployeeUserFields
                                Case BotRuleTypeEnum.CopyCenterCostRole
                                    oBotRuleManager = New roBotRule_Copy_CostCenterRole
                            End Select

                            If oBotRuleManager IsNot Nothing Then bolRet = oBotRuleManager.CreateOrUpdateBotRule(orule, False)
                            If Not bolRet Then
                                oState.Result = BotResultEnum.ErrorCreatingOrUpdatingBot
                                If oBotRuleManager.State.Result = BotRuleResultEnum.NameAlreadyExists Then
                                    oState.Result = BotResultEnum.BotRuleNameAlreadyExists
                                ElseIf oBotRuleManager.State.Result = BotRuleResultEnum.SameTypeAlreadyExist Then
                                    oState.Result = BotResultEnum.BotRuleSameTypeAlreadyExist
                                End If
                                Exit For
                            End If

                        Next
                    End If
                End If

                If (oState.Result = BotResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{BotName}", oBot.Name, String.Empty, 1)
                    Me.oState.Audit(If(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tBot, oBot.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = BotResultEnum.ErrorCreatingOrUpdatingBot
                Me.oState.UpdateStateInfo(ex, "roBotManager::CreateOrUpdateBot")
            Catch ex As Exception
                oState.Result = BotResultEnum.ErrorCreatingOrUpdatingBot
                Me.oState.UpdateStateInfo(ex, "roBotManager::CreateOrUpdateBot")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = BotResultEnum.NoError)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        '''Delete a bot
        ''' </summary>
        ''' <param name="oBot"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteBot(oBot As roBot, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String = String.Empty
            Dim bHaveToClose As Boolean = False

            Try
                If oBot.Id <= 0 Then Return True

                Dim featureId As String = "Bots.Definition"

                Dim iPermissionOverBots As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
                If iPermissionOverBots < 6 Then
                    Me.oState.Result = BotResultEnum.NoPermission
                    Return False
                End If

                oState.Result = BotResultEnum.ErrorDeletingBot

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sSQL As String = String.Empty
                strSQL = "@DELETE# Bots WHERE Id = " & oBot.Id.ToString
                bolRet = ExecuteSql(strSQL)
                If bolRet Then
                    strSQL = "@DELETE# BotRules WHERE IdBot = " & oBot.Id.ToString
                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{BotName}", oBot.Name, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tBot, oBot.Name, tbParameters, -1)
                End If

                oState.Result = BotResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBotManager::DeleteBot")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBotManager::DeleteBot")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = BotResultEnum.NoError)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Ejecuta las reglas de un tipo concreto
        ''' </summary>
        ''' <param name="oBotRuleType"></param>
        ''' <param name="_oParameters"></param>
        ''' <returns></returns>
        Public Function ExecuteRulesByType(ByVal oBotRuleType As BotRuleTypeEnum, ByVal _oParameters As Dictionary(Of BotRuleParameterEnum, String)) As Boolean
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try

                oState.Result = BotResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oListbots As List(Of roBot) = GetAllBots()
                If oListbots IsNot Nothing AndAlso oListbots.Count > 0 Then
                    For Each oBot As roBot In oListbots
                        For Each _BotRule As roBotRule In oBot.BotRules
                            Dim oBotRule As roBotRuleManager = Nothing

                            If _BotRule.IsActive And _BotRule.Type = oBotRuleType Then
                                bolRet = False
                                Select Case _BotRule.Type
                                    Case BotRuleTypeEnum.CopyEmployeePermissions
                                        oBotRule = New roBotRule_Copy_Employee_Permissions
                                    Case BotRuleTypeEnum.CopySupervisorPermissions
                                        oBotRule = New roBotRule_Copy_Supervisor_Permissions
                                    Case BotRuleTypeEnum.CopyEmployeeUserFields
                                        oBotRule = New roBotRule_Copy_EmployeeUserFields
                                    Case BotRuleTypeEnum.CopyCenterCostRole
                                        oBotRule = New roBotRule_Copy_CostCenterRole
                                End Select

                                If oBotRule IsNot Nothing Then bolRet = oBotRule.ExecuteRule(_BotRule, _oParameters)

                                If Not bolRet Then
                                    oState.Result = BotResultEnum.ErrorExecutingBot
                                    Exit For
                                End If
                            End If
                        Next
                        If oState.Result = BotResultEnum.ErrorExecutingBot Then Exit For
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBotManager::ExecuteRulesByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBotManager::ExecuteRulesByType")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = BotResultEnum.NoError)
            End Try
            Return bolRet
        End Function

        Public Function GetRulesByBotType(ByVal oBotType As BotTypeEnum) As Generic.List(Of BotRuleTypeEnum)
            Dim oList As New Generic.List(Of BotRuleTypeEnum)

            Try

                oState.Result = BotResultEnum.NoError

                Dim oLicense As New roServerLicense
                Dim bolIsInstalledBotsBotsPremium As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")

                Select Case oBotType
                    Case BotTypeEnum.NewEmployee
                        If bolIsInstalledBotsBotsPremium Then
                            oList.Add(BotRuleTypeEnum.CopyEmployeePermissions)
                            oList.Add(BotRuleTypeEnum.CopyEmployeeUserFields)
                        End If
                    Case BotTypeEnum.NewSupervisor
                        If bolIsInstalledBotsBotsPremium Then
                            oList.Add(BotRuleTypeEnum.CopySupervisorPermissions)
                        End If
                    Case BotTypeEnum.NewCostCenter
                        If bolIsInstalledBotsBotsPremium Then oList.Add(BotRuleTypeEnum.CopyCenterCostRole)
                End Select
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetRulesByBotType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBotManager::GetRulesByBotType")
            End Try

            Return oList
        End Function

#End Region

    End Class

End Namespace