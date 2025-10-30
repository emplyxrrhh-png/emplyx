Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields

Public Class UserFieldProxy
    Implements IUserFieldSvc

    Public Function KeepAlive() As Boolean Implements IUserFieldSvc.KeepAlive
        Return True
    End Function


    Public Function GetUserField(ByVal strFieldName As String, ByVal _Type As Types, ByVal _bCheckUsedInProcess As Boolean, ByVal oState As roWsState,
                             ByVal bAudit As Boolean) As roGenericVtResponse(Of roUserField) Implements IUserFieldSvc.GetUserField
        Return UserFieldMethods.GetUserField(strFieldName, _Type, _bCheckUsedInProcess, oState, bAudit)
    End Function


    Public Function SaveUserField(ByVal oUserField As roUserField, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveUserField

        Return UserFieldMethods.SaveUserField(oUserField, oState, bAudit)

    End Function


    Public Function DeleteUserField(ByVal oUserField As roUserField, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.DeleteUserField
        Return UserFieldMethods.DeleteUserField(oUserField, oState, bAudit)
    End Function


    Public Function DeleteUserFieldByName(ByVal strUserFieldName As String, ByVal _Type As Types, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.DeleteUserFieldByName

        Return UserFieldMethods.DeleteUserFieldByName(strUserFieldName, _Type, oState, bAudit)
    End Function


    Public Function GetUserFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal bolCheckInProcess As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetUserFieldsDataSet
        Return UserFieldMethods.GetUserFieldsDataSet(_Type, strWhere, bolCheckInProcess, oState)
    End Function


    Public Function GetTaskFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetTaskFieldsDataSet
        Return UserFieldMethods.GetTaskFieldsDataSet(_Type, strWhere, oState)
    End Function


    Public Function GetTaskField(ByVal intID As Integer, ByVal oState As roWsState,
                                 ByVal bAudit As Boolean) As roGenericVtResponse(Of roTaskFieldDefinition) Implements IUserFieldSvc.GetTaskField

        Return UserFieldMethods.GetTaskField(intID, oState, bAudit)

    End Function


    Public Function SaveTaskField(ByVal oTaskFieldDefinition As roTaskFieldDefinition, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveTaskField
        Return UserFieldMethods.SaveTaskField(oTaskFieldDefinition, oState, bAudit)
    End Function



    Public Function SaveUserFields(ByVal dsUserFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveUserFields

        Return UserFieldMethods.SaveUserFields(dsUserFields, oState, bAudit)
    End Function

    Public Function SaveTaskFields(ByVal dsTaskFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveTaskFields
        Return UserFieldMethods.SaveTaskFields(dsTaskFields, oState, bAudit)
    End Function





    Public Function GetUserFieldValues(ByVal strUserField As String, ByVal _Type As Types, ByVal xDate As Date, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetUserFieldValues
        Return UserFieldMethods.GetUserFieldValues(strUserField, _Type, xDate, strParent, strSeparator, strFieldWhere, oState)
    End Function


    Public Function GetEmployeesFromUserFieldWithType(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal strFieldWhere As String, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetEmployeesFromUserFieldWithType
        Return UserFieldMethods.GetEmployeesFromUserFieldWithType(strUserField, strUserFieldValue, xDate, strFieldWhere, Feature, Type, oState)

    End Function


    Public Function GetCategories(ByVal bolOnlyUsed As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String)) Implements IUserFieldSvc.GetCategories
        Return UserFieldMethods.GetCategories(bolOnlyUsed, oState)
    End Function


    Public Function GetUserFieldsList(ByVal _Type As Types, ByVal bolOnlyUsed As Boolean, ByVal bolCheckInProcess As Boolean, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roUserField)) Implements IUserFieldSvc.GetUserFieldsList
        Return UserFieldMethods.GetUserFieldsList(_Type, bolOnlyUsed, bolCheckInProcess, oState, bolAudit)
    End Function


    Public Function GetUserFieldConditionFilter(ByVal oCondition As roUserFieldCondition, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IUserFieldSvc.GetUserFieldConditionFilter
        Return UserFieldMethods.GetUserFieldConditionFilter(oCondition, oState)
    End Function


    Public Function GetUserFieldConditionFilterGlobal(ByVal FilterList As Generic.List(Of roUserFieldCondition), ByVal FilterListCondition As Generic.List(Of String), ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IUserFieldSvc.GetUserFieldConditionFilterGlobal
        Return UserFieldMethods.GetUserFieldConditionFilterGlobal(FilterList, FilterListCondition, oState)
    End Function


    Public Function GetUserFieldConditionsXml(ByVal oConditions As Generic.List(Of roUserFieldCondition), ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IUserFieldSvc.GetUserFieldConditionsXml
        Return UserFieldMethods.GetUserFieldConditionsXml(oConditions, oState)
    End Function


    Public Function LoadUserFieldConditionsFromXml(ByVal strXml As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roUserFieldCondition)) Implements IUserFieldSvc.LoadUserFieldConditionsFromXml
        Return UserFieldMethods.LoadUserFieldConditionsFromXml(strXml, oState)
    End Function

    Public Function GetBusinessCenterFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetBusinessCenterFieldsDataSet
        Return UserFieldMethods.GetBusinessCenterFieldsDataSet(_Type, strWhere, oState)
    End Function


    Public Function GetBusinessCenterField(ByVal intID As Integer, ByVal oState As roWsState,
                                 ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenterFieldDefinition) Implements IUserFieldSvc.GetBusinessCenterField

        Return UserFieldMethods.GetBusinessCenterField(intID, oState, bAudit)

    End Function


    Public Function SaveBusinessCenterField(ByVal oBusinessCenterFieldDefinition As roBusinessCenterFieldDefinition, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveBusinessCenterField

        Return UserFieldMethods.SaveBusinessCenterField(oBusinessCenterFieldDefinition, oState, bAudit)
    End Function



    Public Function SaveBusinessCenterFields(ByVal dsBusinessCenterFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveBusinessCenterFields
        Return UserFieldMethods.SaveBusinessCenterFields(dsBusinessCenterFields, oState, bAudit)
    End Function


#Region "tasksTemplateFields"

    Public Function GetTaskTemplateFieldsDataSet(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetTaskTemplateFieldsDataSet

        Return UserFieldMethods.GetTaskTemplateFieldsDataSet(_IDTaskTemplate, oState)
    End Function


    Public Function GetProjectTemplateFieldsDataSet(ByVal _IDProject As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetProjectTemplateFieldsDataSet
        Return UserFieldMethods.GetProjectTemplateFieldsDataSet(_IDProject, oState)
    End Function

    Public Function GetAvailableTaskTemplateFieldsDataSet(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetAvailableTaskTemplateFieldsDataSet
        Return UserFieldMethods.GetAvailableTaskTemplateFieldsDataSet(_IDTaskTemplate, oState)
    End Function


    Public Function GetAvailableProjectTemplateFieldsDataSet(ByVal _IDProject As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserFieldSvc.GetAvailableProjectTemplateFieldsDataSet
        Return UserFieldMethods.GetAvailableProjectTemplateFieldsDataSet(_IDProject, oState)
    End Function


    Public Function SaveTaskTemplateFields(ByVal _IDTaskTemplate As Integer, ByVal _TasktFields As Generic.List(Of roTaskFieldTaskTemplate), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveTaskTemplateFields
        Return UserFieldMethods.SaveTaskTemplateFields(_IDTaskTemplate, _TasktFields, oState)
    End Function


    Public Function SaveProjectTemplateFields(ByVal _IDProject As Integer, ByVal _TasktFields As Generic.List(Of roTaskFieldProjectTemplate), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserFieldSvc.SaveProjectTemplateFields
        Return UserFieldMethods.SaveProjectTemplateFields(_IDProject, _TasktFields, oState)
    End Function
#End Region

End Class
