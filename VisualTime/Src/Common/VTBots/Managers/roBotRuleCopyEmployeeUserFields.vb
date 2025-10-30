Imports System.Data.Common
Imports System.Runtime.Remoting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase

Public Class roBotRule_Copy_EmployeeUserFields
    Inherits roBotRuleManager

    Private idEmployeeSource As Integer = -1

    Public Sub New()
        MyBase.New(BotRuleTypeEnum.CopyEmployeeUserFields)
    End Sub

    Protected Overrides Function GetIdSourceEmployee() As Integer
        If idEmployeeSource = -1 Then
            idEmployeeSource = _oBotRule.IDTemplate
        End If
        Return idEmployeeSource
    End Function

    Protected Overrides Function GetIdSourcePassport() As Integer
        Return -1
    End Function
    Protected Overrides Function GetSourceIds() As Integer()
        Return Nothing
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
        Try
            oState.Result = BotRuleResultEnum.ErrorExecutingBotRule

            If Me.oParameters IsNot Nothing AndAlso Me.oParameters.ContainsKey(BotRuleParameterEnum.DestinationEmployee) AndAlso GetIdSourceEmployee() > 0 Then
                Dim idDestinationEmployee = roTypes.Any2Integer(Me.oParameters(BotRuleParameterEnum.DestinationEmployee))
                bRet = roBotActions.SetEmployeeUserFields(idDestinationEmployee, "Employees", GetIdSourceEmployee, oState)
                If bRet Then oState.Result = BotRuleResultEnum.NoError
            End If
        Catch ex As Exception
            bRet = False
            oState.Result = BotRuleResultEnum.ErrorExecutingBotRule
            oState.UpdateStateInfo(ex, "roBotRuleManager::ExecuteRuleSpecific")
        End Try
        Return bRet

    End Function

End Class