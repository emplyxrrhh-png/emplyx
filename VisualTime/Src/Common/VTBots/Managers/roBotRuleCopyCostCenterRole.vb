Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCostCenter.CostCenter
Imports Robotics.VTBase

Public Class roBotRule_Copy_CostCenterRole
    Inherits roBotRuleManager

    Private idEmployeeSource As Integer = -1

    Private idSourceIds As Integer()

    Public Sub New()
        MyBase.New(BotRuleTypeEnum.CopyCenterCostRole)
    End Sub

    Protected Overrides Function GetIdSourceEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdSourcePassport() As Integer
        Return -1
    End Function
    Protected Overrides Function GetSourceIds() As Integer()
        If idSourceIds Is Nothing OrElse idSourceIds.Length = 0 Then
            idSourceIds = New Integer(_oBotRule.Definition.SourceIds.Length - 1) {}
            For i As Integer = 0 To _oBotRule.Definition.SourceIds.Length - 1
                idSourceIds(i) = _oBotRule.Definition.SourceIds(i)
            Next
        End If
        Return idSourceIds
    End Function

    Protected Overrides Function GetBotRuleByRowSpecific(ByVal oBotRuleRow As DataRow) As Boolean
        Dim bRet As Boolean = True

        Try
            _oBotRule.IDTemplate = roTypes.Any2Integer(oBotRuleRow("IDTemplate"))
        Catch ex As Exception
            bRet = False
            oState.Result = BotRuleResultEnum.ErrorRecoveringBotRule
            oState.UpdateStateInfo(ex, "roBotRuleManager::GetBotRuleByRowSpecific")
        End Try
        Return bRet

    End Function

    Protected Overrides Function ExecuteRuleSpecific() As Boolean
        Dim bRet As Boolean = False
        Dim bHaveToClose As Boolean = False

        Try
            bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
            oState.Result = BotRuleResultEnum.ErrorExecutingBotRule
            If Me.oParameters IsNot Nothing AndAlso Me.oParameters.ContainsKey(BotRuleParameterEnum.DestinationCostCenter) AndAlso GetSourceIds().Length > 0 Then
                Dim idDestinationCostCenter = roTypes.Any2Integer(Me.oParameters(BotRuleParameterEnum.DestinationCostCenter))
                Dim oCostCenterManagerState As New roCostCenterManagerState(oState.IDPassport)
                'Recogemos todos los Centros de Coste actuales del rol
                For Each sourceId In GetSourceIds()
                    Dim businessCenters As New List(Of Integer)
                    Dim tbBusinessCenter As DataTable = roCostCenterManager.GetBusinessCenterBySecurityGroupDataTable(oCostCenterManagerState, sourceId, False)
                    If tbBusinessCenter IsNot Nothing AndAlso tbBusinessCenter.Rows.Count > 0 Then
                        For Each row As DataRow In tbBusinessCenter.Rows
                            businessCenters.Add(Convert.ToInt32(row("ID")))
                        Next
                    End If

                    'Añadimos el nuestro si no estaba en la lista
                    If Not businessCenters.Contains(idDestinationCostCenter) Then
                        businessCenters.Add(idDestinationCostCenter)
                    End If
                    bRet = roCostCenterManager.SaveBusinessCenterBySecurityGroup(oCostCenterManagerState, sourceId, businessCenters.ToArray, False)
                    If Not bRet Then
                        Exit For
                    End If
                Next

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
                If bRet Then
                    oState.Result = BotRuleResultEnum.NoError
                End If
            End If
        Catch ex As Exception
            bRet = False
            oState.Result = BotRuleResultEnum.ErrorExecutingBotRule
            oState.UpdateStateInfo(ex, "roBotRuleManager::ExecuteRuleSpecific")
        End Try
        Return bRet

    End Function

End Class