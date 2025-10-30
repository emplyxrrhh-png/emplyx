Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields

Public Class UserFieldMethods

    Public Shared Function GetUserField(ByVal strFieldName As String, ByVal _Type As Types, ByVal _bCheckUsedInProcess As Boolean, ByVal oState As roWsState,
                             ByVal bAudit As Boolean) As roGenericVtResponse(Of roUserField)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roUserField)
        oResult.Value = New roUserField(bState, strFieldName, _Type, _bCheckUsedInProcess, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveUserField(ByVal oUserField As roUserField, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oUserField.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteUserField(ByVal oUserField As roUserField, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oUserField.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteUserFieldByName(ByVal strUserFieldName As String, ByVal _Type As Types, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oUserField As New roUserField(bState, strUserFieldName, _Type, False, False)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oUserField.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal bolCheckInProcess As Boolean, ByVal oState As roWsState, Optional ByVal bolIsEmergencyReport As Boolean = False) As roGenericVtResponse(Of DataSet)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roUserField.GetUserFields(_Type, bState, strWhere,, bolCheckInProcess, bolIsEmergencyReport)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTaskFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roUserField.GetTaskFields(_Type, bState, strWhere)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTaskField(ByVal intID As Integer, ByVal oState As roWsState,
                                 ByVal bAudit As Boolean) As roGenericVtResponse(Of roTaskFieldDefinition)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTaskFieldDefinition)
        oResult.Value = New roTaskFieldDefinition(bState, intID, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskField(ByVal oTaskFieldDefinition As roTaskFieldDefinition, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oTaskFieldDefinition.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveUserFields(ByVal dsUserFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        If dsUserFields IsNot Nothing AndAlso dsUserFields.Tables.Count > 0 Then
            Dim tb As DataTable = dsUserFields.Tables(0)
            oResult.Value = roUserField.SaveUserFields(tb, bState, bAudit)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskFields(ByVal dsTaskFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        If dsTaskFields IsNot Nothing AndAlso dsTaskFields.Tables.Count > 0 Then
            Dim tb As DataTable = dsTaskFields.Tables(0)
            oResult.Value = roTaskFieldDefinition.SaveTaskFields(tb, bState, bAudit)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldValues(ByVal strUserField As String, ByVal _Type As Types, ByVal xDate As Date, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roUserField.GetUserFieldValues(strUserField, _Type, xDate, strParent, strSeparator, strFieldWhere, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeesFromUserFieldWithType(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal strFieldWhere As String, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEmployeeUserField.GetEmployeesFromUserFieldWithType(strUserField, strUserFieldValue, xDate, strFieldWhere, Feature, Type, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetCategories(ByVal bolOnlyUsed As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String))

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of String))
        oResult.Value = roUserField.GetCategories(bolOnlyUsed, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldsList(ByVal _Type As Types, ByVal bolOnlyUsed As Boolean, ByVal bolCheckInProcess As Boolean, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roUserField))

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roUserField))
        oResult.Value = roUserField.GetUserFieldsList(_Type, bolCheckInProcess, bState, IIf(bolOnlyUsed, "Used = 1", ""), bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CanUserFieldApplyUniqueConstraint(ByVal userFieldName As String, ByVal userfieldId As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roUserField.CanUserFieldApplyUniqueConstraint(userFieldName, userfieldId, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SetUniqueConstraintToUserField(ByVal userFieldName As String, ByVal userfieldId As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roUserField.SetUniqueConstraintToUserField(userFieldName, userfieldId, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldConditionFilter(ByVal oCondition As roUserFieldCondition, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        If oCondition.UserField IsNot Nothing Then
            oCondition.UserField.Load(False)
        End If

        oResult.Value = oCondition.GetFilter(-1)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldConditionFilterGlobal(ByVal FilterList As Generic.List(Of roUserFieldCondition), ByVal FilterListCondition As Generic.List(Of String), ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        oResult.Value = roUserFieldCondition.GetUserFieldConditionFilterGlobal(FilterList, FilterListCondition, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetUserFieldConditionsXml(ByVal oConditions As Generic.List(Of roUserFieldCondition), ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        oResult.Value = roUserFieldCondition.GetXml(oConditions)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function LoadUserFieldConditionsFromXml(ByVal strXml As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roUserFieldCondition))

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roUserFieldCondition))
        oResult.Value = roUserFieldCondition.LoadFromXml(strXml, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterFieldsDataSet(ByVal _Type As Types, ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roUserField.GetBusinessCenterFields(_Type, bState, strWhere)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterField(ByVal intID As Integer, ByVal oState As roWsState,
                                 ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenterFieldDefinition)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roBusinessCenterFieldDefinition)

        oResult.Value = New roBusinessCenterFieldDefinition(bState, intID, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveBusinessCenterField(ByVal oBusinessCenterFieldDefinition As roBusinessCenterFieldDefinition, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = oBusinessCenterFieldDefinition.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveBusinessCenterFields(ByVal dsBusinessCenterFields As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        If dsBusinessCenterFields IsNot Nothing AndAlso dsBusinessCenterFields.Tables.Count > 0 Then
            Dim tb As DataTable = dsBusinessCenterFields.Tables(0)
            oResult.Value = roBusinessCenterFieldDefinition.SaveBusinessCenterFields(tb, bState, bAudit)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

        Return oResult
    End Function

#Region "tasksTemplateFields"

    Public Shared Function GetTaskTemplateFieldsDataSet(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskFieldTaskTemplate.GetTaskFieldsTaskTemplateDataTable(_IDTaskTemplate, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetProjectTemplateFieldsDataSet(ByVal _IDProject As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskFieldProjectTemplate.GetTaskFieldsProjectTemplateDataTable(_IDProject, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAvailableTaskTemplateFieldsDataSet(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskFieldTaskTemplate.GetAvailableTaskFieldsTaskTemplateDataTable(_IDTaskTemplate, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAvailableProjectTemplateFieldsDataSet(ByVal _IDProject As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskFieldProjectTemplate.GetAvailableTaskFieldsProjectTemplateDataTable(_IDProject, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskTemplateFields(ByVal _IDTaskTemplate As Integer, ByVal _TasktFields As Generic.List(Of roTaskFieldTaskTemplate), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskFieldTaskTemplate.SaveTaskFieldsTaskTemplate(_IDTaskTemplate, _TasktFields, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveProjectTemplateFields(ByVal _IDProject As Integer, ByVal _TasktFields As Generic.List(Of roTaskFieldProjectTemplate), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskFieldProjectTemplate.SaveTaskFieldsProjectTemplate(_IDProject, _TasktFields, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

End Class