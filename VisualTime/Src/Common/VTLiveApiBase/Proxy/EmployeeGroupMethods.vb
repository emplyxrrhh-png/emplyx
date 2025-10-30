Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.GroupIndicator
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase

Public Class EmployeeGroupMethods

    ''' <summary>
    ''' Obtiene los grupos definidos.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los grupos devueltos.
    ''' </summary>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroups(ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetGroups(Feature, Type, bState)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetGroupZones(ByVal IDZone As Integer, ByVal IDZoneWorkingTime As Integer, ByVal IDZoneNonWorkingTime As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Integer, Integer, Boolean))
        Dim bolResp = False
        Dim bolRet As New roGenericVtResponse(Of (Integer, Integer, Boolean))
        bolRet.Value = (IDZoneWorkingTime, IDZoneNonWorkingTime, bolResp)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        bolResp = roGroup.GetGroupZones(IDZone, IDZoneWorkingTime, IDZoneNonWorkingTime, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        bolRet.Status = newGState

        Return bolRet

    End Function

    Public Shared Function GetGroupCenters(ByVal IDGroup As Integer, ByVal IDCenter As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Integer, Boolean))

        Dim bolResp = False
        Dim bolRet As New roGenericVtResponse(Of (Integer, Boolean))
        bolRet.Value = (IDCenter, bolResp)
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        bolResp = roGroup.GetGroupCenters(IDGroup, IDCenter, bState)
        bolRet.Value = (IDCenter, bolResp)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        bolRet.Status = newGState

        Return bolRet
    End Function

    ''' <summary>
    ''' Obtiene los sub-grupos de un grupo<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los grupos devueltos.
    ''' </summary>
    ''' <param name="IDGroup">ID del grupo a recuperar sub-grupos</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
    ''' <remarks></remarks>

    Public Shared Function GetChildGroups(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetChildGroups(IDGroup, Feature, Type, bState)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la información de un grupo.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroup(ByVal IDGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roGroup)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroup)
        oResult.Value = New roGroup(IDGroup, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene la definición de un grupo por nombre.<br/>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strGroupName">Nombre del grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupByName(ByVal strGroupName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroup)
        oResult.Value = roGroup.GetGroupByName(strGroupName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Guarda la definición de un grupo.<br/>
    ''' Lanza el proceso broadcaster del servidor si hay la licencia de riesgos laborales activa.<br/>
    ''' * Se audita la grabación.
    ''' </summary>
    ''' <param name="Group">Definición del grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveGroup(ByVal Group As roGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim intRet As Integer = -1

        If Group.Save(bAudit, bState) Then
            intRet = Group.ID
        End If

        oResult.Value = intRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina un grupo.<br/>
    ''' Verifica que el grupo no contenga empleados activos ni en tránsito.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="idGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteGroup(ByVal idGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oGroup As New roGroup(idGroup, bState, False)
        oResult.Value = oGroup.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro<br/>
    ''' Se da por supuesto que nunca llegara un IDGroup sobre el que no se tengan permisos.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los empleados devueltos.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas. sysrovwCurrentEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal excludeWithoutContract As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetEmployeesFromGroup(IDGroup, Feature, Type, bState, excludeWithoutContract)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve un dataset con los empledos en tránsito hacia el grupo pasado por parámetro.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los empleados devueltos.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.ID, Employees.Name, Groups.Path as Path, Employees.Type, BeginDate </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesInTransitToTheGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetEmployeesInTransitToTheGroup(IDGroup, Feature, Type, bState)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve un dataset con los empledos que estuvieron en el grupo pasado por parámetro.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los empleados devueltos.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.ID, Employees.Name, Employees.Type, EmployeeGroups.BeginDate, EmployeeGroups.EndDate </returns>
    ''' <remarks></remarks>

    Public Shared Function GetOldEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal includeWithoutContract As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetOldEmployeesFromGroup(IDGroup, Feature, Type, bState, includeWithoutContract)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro.<br/>
    ''' Se da por supuesto que nunca llegara un IDGroup sobre el que no se tengan permisos.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los empleados devueltos.<br/>
    ''' Permite indicar un filtro opcional.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="FieldWhere">
    ''' El formato del parámetro UserFieldWhere tiene que se el de una condición/es SQL:<br/>
    ''' (Name = 'PEPE' or USR_Direccion like 'c%') and [USR_Correo Electrónico] = 'pepe@robotics.es'<br/>
    ''' NOTA: NO PONER AND/OR/WHERE DELANTE DE LA CONDICION, ESO LO PONE LA FUNCION SI LO NECESITA<br/>
    ''' Combinaciones de parámetros:<br/>
    ''' IDGroup = -1 Y FieldWhere != '' - Devuelve toda la lista y el where se aplica a la select en general<br/>
    ''' IDGroup != -1 Y FieldWhere = '' - Devuelve el contenido de sysrovwAllEmployeeGroups para ese grupo<br/>
    ''' IDGroup != -1 Y FieldWhere != '' - Devuelve el contenido de sysrovwAllEmployeeGroups combinado <br/>
    '''                                    con Employees al que se le aplica el where
    ''' </param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesFromGroupWithType(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal FieldWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roGroup.GetEmployeesFromGroupWithType(IDGroup, Feature, Type, FieldWhere, bState)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeListFromGroupsRecursive(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String, ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Integer))


        Dim sEmpFilter As String = Robotics.Security.roSelector.BuildSelectionStringFromIDs({}, arrGroups)
        oResult.Value = Robotics.Security.roSelector.GetEmployeeList(oState.IDPassport, Feature, Type, Nothing,
                                                                                                    sEmpFilter, strFilters, strFilterUserFields, False,
                                                                                                    Nothing, Nothing)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeListFromGroupsNORecursive(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String, ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Integer))


        Dim sEmpFilter As String = Robotics.Security.roSelector.BuildSelectionStringFromIDs({}, arrGroups)
        oResult.Value = Robotics.Security.roSelector.GetEmployeeList(oState.IDPassport, Feature, Type, Nothing,
                                                                                sEmpFilter, strFilters, strFilterUserFields, True,
                                                                                Nothing, Nothing)


        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeListFromGroupsRecursiveInDates(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                  ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState,
                                                                  ByVal DateInf As DateTime, ByVal DateSup As DateTime) As roGenericVtResponse(Of Generic.List(Of Integer))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Integer))

        Dim sEmpFilter As String = Robotics.Security.roSelector.BuildSelectionStringFromIDs({}, arrGroups)
        oResult.Value = Robotics.Security.roSelector.GetEmployeeList(oState.IDPassport, Feature, Type, Nothing,
                                                                                sEmpFilter, strFilters, strFilterUserFields, False,
                                                                                DateInf, DateSup)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetEmployeeListFromGroupsNORecursiveInDates(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                    ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState,
                                                                    ByVal DateInf As DateTime, ByVal DateSup As DateTime) As roGenericVtResponse(Of Generic.List(Of Integer))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Integer))
        Dim sEmpFilter As String = Robotics.Security.roSelector.BuildSelectionStringFromIDs({}, arrGroups)
        oResult.Value = Robotics.Security.roSelector.GetEmployeeList(oState.IDPassport, Feature, Type, Nothing,
                                                                                sEmpFilter, strFilters, strFilterUserFields, True,
                                                                                DateInf, DateSup)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve un lista con los códigos de los grupos padre de la selección indicada.
    ''' </summary>
    ''' <param name="lstGroupsSelection">Lista de códigos de grupo.</param>
    ''' <param name="lstEmployeesSelection">Lista de códigos de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de los códigos de grupo</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupSelectionPath(ByVal lstGroupsSelection As Generic.List(Of Integer), ByVal lstEmployeesSelection As Generic.List(Of Integer), ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Integer))
        oResult.Value = roGroup.GetGroupSelectionPath(lstGroupsSelection, lstEmployeesSelection, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición del árbol de grupos y empleados.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los grupos y empleados devueltos.
    ''' </summary>
    ''' <param name="strFilterUserFields">Filtro sobre los campos de la fiha de los empleados.</param>
    ''' <param name="strFeature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="strType">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roGroupTree' con la definición de la estructura en árbol de grupos y empleados.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTree(ByVal strFilterUserFields As String, ByVal strFeature As String, ByVal strType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupTree))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roGroupTree))
        oResult.Value = roGroupTreeManager.GetTree(strFilterUserFields, strFeature, strType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

#Region "UserFields"

    ''' <summary>
    ''' Obtiene el valor del campo de la ficha del grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="UserFieldName">Nombre campo de la ficha.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroupUserField' con el valor del campo de la ficha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupUserField)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupUserField)
        oResult.Value = roGroupUserField.GetUserField(IDGroup, UserFieldName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene todos los valores de la ficha del grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roGroupUserField' con los valores de la ficha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserFields(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupUserField))

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roGroupUserField))
        oResult.Value = roGroupUserField.GetUserFieldsList(IDGroup, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '' _
    ''Public Shared Function GetUserFieldsList(ByRef oState As roEmployeeState) As roUserFields
    ''    If oState Is Nothing Then oState = New roEmployeeState(-1, Me.Context)
    ''    oState.UpdateStateInfo(Me.Context)
    ''    Dim oUserFields As roUserFields = oEmployeeConector.GetUserFieldsList(oState)
    ''    Return oUserFields
    ''End Function

    ''' <summary>
    ''' Guarda un valor de un campo de la ficha del grupo.<br/>
    ''' Si hay la licencia de prevención de riesgos laborales activa, notifica al servidor para que lanze el proceso broadcaster.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="UserFieldName">nombre campo de la ficha.</param>
    ''' <param name="UserFieldValue">Valor del campo de la ficha.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal UserFieldValue As Object, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroupUserField.SaveUserField(IDGroup, UserFieldName, UserFieldValue, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los valores de la ficha del grupo. <br/>
    ''' Si hay la licencia de prevención de riesgos laborales activa, notifica al servidor para que lanze el proceso broadcaster.
    ''' </summary>
    ''' <param name="_IDGroup">Código grupo.</param>
    ''' <param name="_UserFields">Lista de objetos 'roGroupUserField' con los valores de la ficha a actualizar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveUserFields(ByVal _IDGroup As Integer, ByVal _UserFields As Generic.List(Of roGroupUserField), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroupUserField.SaveUserFields(_IDGroup, _UserFields, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene todos los valores de la ficha de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: FieldCaption, FieldName, Type, Value, ValueDateTime, Category, AccessLevel, Description, AccessValidation </returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserFieldsDataset(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roGroupUserField.GetUserFieldsDataTable(IDGroup, bState)

        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de un grupo en función de su nombre y de la empresa a la que pertenece. <br/>
    ''' Si no existe devuelve Nothing. <br></br>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strGroupName">Nombre grupo.</param>
    ''' <param name="intIDCompany">Código empresa.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupByNameWithCompany(ByVal strGroupName As String, ByVal intIDCompany As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroup)
        oResult.Value = roGroup.GetGroupByName(strGroupName, bState, , intIDCompany & "\%")

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de la empresa por nombre. <br/>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strCompanyName">Nombre empresa.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición de la empresa.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetCompanyByName(ByVal strCompanyName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroup)
        oResult.Value = roGroup.GetCompanyByName(strCompanyName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de un grupo en función de su nombre y que esté en un mismo nivel (mismo path). <br/>
    ''' Si no existe devuelve Nothing. <br></br>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strGroupName">Nombre grupo.</param>
    ''' <param name="strPath">Ruta del nivel.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupByNameInLevel(ByVal strGroupName As String, ByVal strPath As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroup)
        oResult.Value = roGroup.GetGroupByNameInLevel(strGroupName, strPath, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

#Region "Indicators"

    ''' <summary>
    ''' Obtiene todos los valores de la ficha del grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roGroupUserField' con los valores de la ficha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupIndicators(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupIndicator))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roGroupIndicator))
        oResult.Value = roGroupIndicator.GetGroupIndicators(IDGroup, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene todos los indicadores de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: ID, Name, Description</returns>
    ''' <remarks></remarks>

    Public Shared Function GetGroupIndicatorsDataset(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roGroupIndicator.GetIndicatorsDataTable(IDGroup, bState)

        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Guarda los valores los indicadores del grupo. <br/>
    ''' Si requiere la licencia de kpis activa.
    ''' </summary>
    ''' <param name="_IDGroup">Código grupo.</param>
    ''' <param name="_IDsIndicators">Lista de objetos 'Integer' con los identificadores de los indicadores a actualizar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveGroupIndicators(ByVal _IDGroup As Integer, ByVal _IDsIndicators As Generic.List(Of Integer), ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroupIndicator.SaveGroupIndicators(_IDGroup, _IDsIndicators, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

#Region "SaaS Service Administration Status"

    Public Shared Function ActivateService(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroup.ActivateService(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CancelService(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroup.CancelService(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function RegenerateAllPasswords(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roGroup.RegenerateAllPasswords(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetServiceStatus(ByVal serviceStatus As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Boolean))

        Dim bState = New roGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (Boolean, Boolean))
        Dim resResult As Boolean = roGroup.GetServiceStatus(serviceStatus, bState)

        oResult.Value = (serviceStatus, resResult)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

End Class