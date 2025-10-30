Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.DataLayer.AccessHelper

Public MustInherit Class roBotRuleManager
    Implements IDisposable

    Private disposedValue As Boolean

    Protected ReadOnly Property BotRuleType As BotRuleTypeEnum
    Protected _oBotRule As roBotRule = Nothing
    Protected oState As roBotRuleState = Nothing
    Protected oParameters As New Dictionary(Of BotRuleParameterEnum, String)

    Protected MustOverride Function GetIdSourceEmployee() As Integer

    Protected MustOverride Function GetIdSourcePassport() As Integer
    Protected MustOverride Function GetSourceIds() As Integer()

    Sub New(ByVal _BotRuleType As BotRuleTypeEnum)
        Me.BotRuleType = _BotRuleType
        oState = New roBotRuleState()
    End Sub

    Sub New(ByVal _BotRuleType As BotRuleTypeEnum, ByVal _State As roBotRuleState)
        Me.BotRuleType = _BotRuleType
        oState = _State
    End Sub

    Public ReadOnly Property State As roBotRuleState
        Get
            Return oState
        End Get
    End Property

    Public Function GetBotRuleByRow(ByVal oBotRuleRow As DataRow, Optional bAudit As Boolean = False) As roBotRule
        Dim bLoaded As Boolean = False
        Try
            oState.Result = BotRuleResultEnum.NoError

            _oBotRule = New roBotRule With {
            .Id = roTypes.Any2Integer(oBotRuleRow("ID")),
            .IdBot = roTypes.Any2Integer(oBotRuleRow("IdBot")),
            .Type = roTypes.Any2Integer(oBotRuleRow("Type")),
            .Name = roTypes.Any2String(oBotRuleRow("Name")),
            .Definition = If(IsDBNull(oBotRuleRow("Definition")), Nothing, roJSONHelper.Deserialize(oBotRuleRow("Definition"), GetType(roBotConfig))),
            .IsActive = roTypes.Any2Boolean(oBotRuleRow("IsActive"))
            }

            bLoaded = GetBotRuleByRowSpecific(oBotRuleRow)

            If (oState.Result = BotRuleResultEnum.NoError) AndAlso bAudit Then
                Dim botRuleName As String = If(_oBotRule IsNot Nothing, _oBotRule.Name, "")

                ' Auditamos
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Extensions.roAudit.AddParameter(tbParameters, "{BotRuleName}", botRuleName, String.Empty, 1)
                Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tBot, botRuleName, tbParameters, -1)
            End If
        Catch ex As Exception
            oState.Result = BotRuleResultEnum.ErrorRecoveringBotRule
            Me.oState.UpdateStateInfo(ex, "roBotRuleManager::GetBotRuleByRow")
        End Try

        Return _oBotRule
    End Function

    Public Function ExecuteRule(ByVal oBotRule As roBotRule, ByVal _oParameters As Dictionary(Of BotRuleParameterEnum, String)) As Boolean
        Dim bolret As Boolean = False
        Try
            Me.oParameters = _oParameters
            Me._oBotRule = oBotRule

            bolret = ExecuteRuleSpecific()

            If bolret Then
                Dim botRuleName As String = If(_oBotRule IsNot Nothing, _oBotRule.Name, "")

                ' Auditamos
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Extensions.roAudit.AddParameter(tbParameters, "{BotRuleName}", botRuleName, String.Empty, 1)
                Me.oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tBot, botRuleName, tbParameters, -1)

            End If

            ' Actualizamos la fecha de ejecución y el estado del bot
            Dim oBotManager As New roBotManager()
            Dim oBot As New roBot
            oBot = oBotManager.GetBotById(oBotRule.IdBot)
            If oBot IsNot Nothing AndAlso oBot.Id > 0 Then
                oBot.LastExecutionDate = Now
                oBot.Status = IIf(bolret, BotStatusEnum.Correct, BotStatusEnum.Failed)
                oBotManager.CreateOrUpdateBot(oBot)
            End If
        Catch ex As Exception
            bolret = False
            oState.Result = BotRuleResultEnum.ErrorExecutingBotRule
            Me.oState.UpdateStateInfo(ex, "roBotRuleManager::ExecuteRule")
        End Try

        Return bolret
    End Function

    Protected Overridable Function ExecuteRuleSpecific() As Boolean
        Return True
    End Function

    Protected Overridable Function GetBotRuleByRowSpecific(ByVal oBotRuleRow As DataRow) As Boolean
        Return True
    End Function

    Public Function CreateOrUpdateBotRule(ByRef oBotRule As roBotRule, Optional ByVal bAudit As Boolean = False) As Boolean
        Dim bolRet As Boolean = False
        Dim bolIsNew As Boolean = True
        Dim iNew As Integer
        Dim iOldStatus As Integer

        Try
            oState.Result = BotRuleResultEnum.NoError

            If Not DataLayer.roSupport.IsXSSSafe(oBotRule) Then
                oState.Result = BotRuleResultEnum.XSSvalidationError
                Return False
            End If


            bolIsNew = (oBotRule.Id = 0)

            ' Verificamos permiso
            Dim featureId As String = "Bots.Definition"

            Dim iPermissionOverBots As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
            If iPermissionOverBots < 6 Then
                Me.oState.Result = BotRuleResultEnum.NoPermission
                Return False
            End If

            ' Hay que validar que no exista otro bot con el mismo nombre
            Dim sqlCommand As String = String.Empty

            sqlCommand = "@SELECT# count(*) FROM [dbo].[BotRules] WHERE Id <> " & oBotRule.Id.ToString & " AND Name = '" & oBotRule.Name.Replace("'", "''") & "'"
            If roConversions.Any2Double(ExecuteScalar(sqlCommand)) > 0 Then
                Me.oState.Result = BotRuleResultEnum.NameAlreadyExists
                Return False
            End If

            sqlCommand = "@SELECT# count(*) FROM [dbo].[BotRules] WHERE Id <> " & oBotRule.Id.ToString & " AND Type = " & oBotRule.Type
            If roConversions.Any2Double(ExecuteScalar(sqlCommand)) > 0 Then
                Me.oState.Result = BotRuleResultEnum.SameTypeAlreadyExist
                Return False
            End If

            Dim parameters As New List(Of CommandParameter)

            Dim bStatusChanged As Boolean = False

            If Not bolIsNew Then
                sqlCommand = $"@UPDATE# BotRules SET [Name]=@name, [IDBot]=@idbot, " &
                                        " [IDTemplate]=@idtemplate, [Definition]=@definition, [Type]=@type , [IsActive]=@isactive " &
                                        " OUTPUT DELETED.idbot" &
                                        " WHERE [Id] = @id"
            Else
                sqlCommand = $"@INSERT# INTO BotRules ([Name] " &
                                    " ,[IDBot] " &
                                    " ,[Type] " &
                                    " ,[IDTemplate] " &
                                    " ,[Definition] " &
                                    " ,[IsActive] ) " &
                                    " OUTPUT INSERTED.ID " &
                                    " VALUES (@name, @idbot, " &
                                    " @type,@idtemplate,@definition,@isactive )"
            End If

            parameters.Add(New CommandParameter("@name", CommandParameter.ParameterType.tString, oBotRule.Name))
            parameters.Add(New CommandParameter("@idbot", CommandParameter.ParameterType.tInt, oBotRule.IdBot))
            parameters.Add(New CommandParameter("@idtemplate", CommandParameter.ParameterType.tInt, oBotRule.IDTemplate))
            parameters.Add(New CommandParameter("@definition", CommandParameter.ParameterType.tString, If(oBotRule.Definition IsNot Nothing, roJSONHelper.Serialize(oBotRule.Definition), DBNull.Value)))
            parameters.Add(New CommandParameter("@isactive", CommandParameter.ParameterType.tBoolean, oBotRule.IsActive))
            parameters.Add(New CommandParameter("@type", CommandParameter.ParameterType.tInt, oBotRule.Type))
            If Not bolIsNew Then
                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oBotRule.Id))
            End If

            Try
                If bolIsNew Then
                    iNew = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                Else
                    iOldStatus = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                End If
                bolRet = True
                If bolIsNew Then oBotRule.Id = iNew
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBotRuleManager::CreateOrUpdateBotRule")
                oState.Result = BotRuleResultEnum.ErrorCreatingOrUpdatingBotRule
            End Try

            If (oState.Result = BotRuleResultEnum.NoError) Then
                bolRet = CreateOrUpdateBotRuleSpecific(oBotRule)
            End If

            If (oState.Result = BotRuleResultEnum.NoError) AndAlso bAudit Then
                ' Auditamos
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Me.oState.Audit(If(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tBotRule, oBotRule.Name, tbParameters, -1)
            End If
        Catch ex As DbException
            oState.Result = BotRuleResultEnum.ErrorCreatingOrUpdatingBotRule
            Me.oState.UpdateStateInfo(ex, "roBotRuleManager::CreateOrUpdateBotRule")
        Catch ex As Exception
            oState.Result = BotRuleResultEnum.ErrorCreatingOrUpdatingBotRule
            Me.oState.UpdateStateInfo(ex, "roBotRuleManager::CreateOrUpdateBotRule")
        Finally
        End Try
        Return bolRet
    End Function

    Protected Overridable Function CreateOrUpdateBotRuleSpecific(ByRef oBotRule As roBotRule) As Boolean
        Return True
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then

        End If
        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

End Class