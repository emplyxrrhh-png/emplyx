Imports System.Data.Common
Imports System.Runtime.Remoting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base
Imports Robotics.VTBase

Public Class roBotRule_Copy_Supervisor_Permissions
    Inherits roBotRuleManager

    Private idPassportSource As Integer = -1

    Public Sub New()
        MyBase.New(BotRuleTypeEnum.CopySupervisorPermissions)
    End Sub

    Protected Overrides Function GetIdSourceEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdSourcePassport() As Integer
        If idPassportSource = -1 Then
            idPassportSource = _oBotRule.IDTemplate
        End If
        Return idPassportSource
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

            If Me.oParameters IsNot Nothing AndAlso Me.oParameters.ContainsKey(BotRuleParameterEnum.DestinationSupervisor) AndAlso GetIdSourcePassport() > 0 Then
                Dim lstPassports As New Generic.List(Of Integer)
                lstPassports.Add(roTypes.Any2Integer(Me.oParameters(BotRuleParameterEnum.DestinationSupervisor)))
                bRet = roPassportManager.CopySupervisorProperties(GetIdSourcePassport, lstPassports.ToArray, True, True, True, True, True, True, New roPassportState(-1))

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