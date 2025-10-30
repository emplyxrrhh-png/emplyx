Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar

Public Class ScheduleRulesProxy
    Implements IScheduleRulesSvc

    Public Function KeepAlive() As Boolean Implements IScheduleRulesSvc.KeepAlive
        Return True
    End Function

    Function GetLabAgreeScheduleRules(ByVal IdLabAgree As Integer, ByVal oState As roWsState) As roScheduleRulesResponse Implements IScheduleRulesSvc.GetLabAgreeScheduleRules
        Return ScheduleRulesMethods.GetLabAgreeScheduleRules(IdLabAgree, oState)
    End Function

    Function GetContractScheduleRules(ByVal IdContract As String, ByVal oState As roWsState) As roScheduleRulesResponse Implements IScheduleRulesSvc.GetContractScheduleRules
        Return ScheduleRulesMethods.GetContractScheduleRules(IdContract, oState)
    End Function

    Function GetEmployeeCurrentScheduleRules(ByVal IdContract As String, ByVal oState As roWsState) As roScheduleRulesResponse Implements IScheduleRulesSvc.GetEmployeeCurrentScheduleRules

        Return ScheduleRulesMethods.GetEmployeeCurrentScheduleRules(IdContract, oState)
    End Function

    Function SaveLabAgreeScheduleRules(ByVal IdLabAgree As Integer, ByVal oScheduleRules As roScheduleRule(), ByVal oState As roWsState) As roScheduleRulesStdResponse Implements IScheduleRulesSvc.SaveLabAgreeScheduleRules

        Return ScheduleRulesMethods.SaveLabAgreeScheduleRules(IdLabAgree, oScheduleRules, oState)
    End Function

    Function SaveContractScheduleRules(ByVal IdContract As String, ByVal oScheduleRules As roScheduleRule(), ByVal oState As roWsState) As roScheduleRulesStdResponse Implements IScheduleRulesSvc.SaveContractScheduleRules

        Return ScheduleRulesMethods.SaveContractScheduleRules(IdContract, oScheduleRules, oState)
    End Function


    Function GetUserScheduleRulesTypes(ByVal oState As roWsState) As roScheduleRulesTypesResponse Implements IScheduleRulesSvc.GetUserScheduleRulesTypes
        Return ScheduleRulesMethods.GetUserScheduleRulesTypes(oState)
    End Function








End Class
