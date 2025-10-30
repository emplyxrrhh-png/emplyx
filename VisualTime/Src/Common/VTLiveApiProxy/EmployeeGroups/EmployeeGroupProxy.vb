Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.GroupIndicator
Imports Robotics.Base.VTUserFields.UserFields

Public Class EmployeeGroupProxy
    Implements IEmployeeGroupSvc

    Public Function KeepAlive() As Boolean Implements IEmployeeGroupSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene los grupos definidos.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los grupos devueltos.
    ''' </summary>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
    ''' <remarks></remarks>

    Public Function GetGroups(ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetGroups
        Return EmployeeGroupMethods.GetGroups(Feature, Type, oState)
    End Function


    Public Function GetGroupZones(ByVal IDZone As Integer, ByVal IDZoneWorkingTime As Integer, ByVal IDZoneNonWorkingTime As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Integer, Integer, Boolean)) Implements IEmployeeGroupSvc.GetGroupZones
        Return EmployeeGroupMethods.GetGroupZones(IDZone, IDZoneNonWorkingTime, IDZoneNonWorkingTime, oState)
    End Function


    Public Function GetGroupCenters(ByVal IDGroup As Integer, ByVal IDCenter As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Integer, Boolean)) Implements IEmployeeGroupSvc.GetGroupCenters

        Return EmployeeGroupMethods.GetGroupCenters(IDGroup, IDCenter, oState)
    End Function

    ''' <summary>
    ''' Retorna dataset con los grupos de empleados a los que el Passport NO tiene ningún permiso.<br/>
    ''' Se utiliza para indicar explicitamente en los listados los grupos que no se deben mostrar.
    ''' </summary>
    ''' <param name="Feature">Nombre de la funcionalidad sobre la que se obtendrán los grupos de empleados no permitidos.</param>
    ''' <param name="oState">Información adicional de estado (de donde se obtendrá el IdPassport).</param>
    ''' <returns>Dataset con tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
    ''' <remarks></remarks>

    Public Function GetGroupsNotAllowed(ByVal Feature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetGroupsNotAllowed

        Return EmployeeGroupMethods.GetGroupsNotAllowed(Feature, oState)
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

    Public Function GetChildGroups(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetChildGroups

        Return EmployeeGroupMethods.GetChildGroups(IDGroup, Feature, Type, oState)
    End Function

    ''' <summary>
    ''' Obtiene la información de un grupo.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Function GetGroup(ByVal IDGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roGroup) Implements IEmployeeGroupSvc.GetGroup

        Return EmployeeGroupMethods.GetGroup(IDGroup, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene la definición de un grupo por nombre.<br/>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strGroupName">Nombre del grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición del grupo.</returns>
    ''' <remarks></remarks>

    Public Function GetGroupByName(ByVal strGroupName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup) Implements IEmployeeGroupSvc.GetGroupByName

        Return EmployeeGroupMethods.GetGroupByName(strGroupName, oState)
    End Function

    ''' <summary>
    ''' Obtiene la definición de los grupos que contenga una lista de códigos.
    ''' </summary>
    ''' <param name="List">Lista de códigos de grupo separados por comas.</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
    ''' <remarks></remarks>

    Public Function GetGroupsFromList(ByVal List As String, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetGroupsFromList
        Return EmployeeGroupMethods.GetGroupsFromList(List, Feature, Type, oState)

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

    Public Function SaveGroup(ByVal Group As roGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements IEmployeeGroupSvc.SaveGroup

        Return EmployeeGroupMethods.SaveGroup(Group, oState, bAudit)
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

    Public Function DeleteGroup(ByVal idGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.DeleteGroup

        Return EmployeeGroupMethods.DeleteGroup(idGroup, oState, bAudit)
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

    Public Function GetEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal excludeWithoutContract As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetEmployeesFromGroup

        Return EmployeeGroupMethods.GetEmployeesFromGroup(IDGroup, Feature, Type, excludeWithoutContract, oState)
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

    Public Function GetEmployeesInTransitToTheGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetEmployeesInTransitToTheGroup

        Return EmployeeGroupMethods.GetEmployeesInTransitToTheGroup(IDGroup, Feature, Type, oState)
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

    Public Function GetOldEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal includeWithoutContract As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetOldEmployeesFromGroup

        Return EmployeeGroupMethods.GetOldEmployeesFromGroup(IDGroup, Feature, Type, includeWithoutContract, oState)
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

    Public Function GetEmployeesFromGroupWithType(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal FieldWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetEmployeesFromGroupWithType

        Return EmployeeGroupMethods.GetEmployeesFromGroupWithType(IDGroup, Feature, Type, FieldWhere, oState)
    End Function



    Public Function GetEmployeesFromGroupAll(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetEmployeesFromGroupAll

        Return EmployeeGroupMethods.GetEmployeesFromGroupAll(IDGroup, Feature, Type, oState)
    End Function


    Public Function GetEmployeeListFromGroupsRecursive(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String, ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeGroupSvc.GetEmployeeListFromGroupsRecursive

        Return EmployeeGroupMethods.GetEmployeeListFromGroupsRecursive(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState)
    End Function


    Public Function GetEmployeeListFromGroupsNORecursive(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String, ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeGroupSvc.GetEmployeeListFromGroupsNORecursive

        Return EmployeeGroupMethods.GetEmployeeListFromGroupsNORecursive(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState)
    End Function


    Public Function GetEmployeeListFromGroupsRecursiveInDates(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                  ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState,
                                                                  ByVal DateInf As DateTime, ByVal DateSup As DateTime) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeGroupSvc.GetEmployeeListFromGroupsRecursiveInDates

        Return EmployeeGroupMethods.GetEmployeeListFromGroupsRecursiveInDates(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState, DateInf, DateSup)
    End Function


    Public Function GetEmployeeListFromGroupsNORecursiveInDates(ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                    ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState,
                                                                    ByVal DateInf As DateTime, ByVal DateSup As DateTime) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeGroupSvc.GetEmployeeListFromGroupsNORecursiveInDates

        Return EmployeeGroupMethods.GetEmployeeListFromGroupsNORecursiveInDates(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState, DateInf, DateSup)
    End Function


    ''' <summary>
    ''' Devuelve un dataset con los empledos que pertenecen o han pertenecido al grupo pasado por parámetro.<br/>
    ''' Se da por supuesto que nunca llegara un IDGroup sobre el que no se tengan permisos.<br/>
    ''' Permite indicar la funcionalidad de seguridad del passport para aplicar seguridad y filtrar correctamente los empleados devueltos.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: IDEmployee, Name, CurrentEmployee, Type (4- El empleado es una futura incorporación, 3- El empleado es una baja, 2- Empleado con movilidad, 1- Empleado normal) </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesFromGroupAny(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeGroupSvc.GetEmployeesFromGroupAny

        Return EmployeeGroupMethods.GetEmployeesFromGroupAny(IDGroup, Feature, Type, oState)
    End Function

    ''' <summary>
    ''' Devuelve un lista con los códigos de los grupos padre de la selección indicada.
    ''' </summary>
    ''' <param name="lstGroupsSelection">Lista de códigos de grupo.</param>
    ''' <param name="lstEmployeesSelection">Lista de códigos de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de los códigos de grupo</returns>
    ''' <remarks></remarks>

    Public Function GetGroupSelectionPath(ByVal lstGroupsSelection As Generic.List(Of Integer), ByVal lstEmployeesSelection As Generic.List(Of Integer), ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeGroupSvc.GetGroupSelectionPath

        Return EmployeeGroupMethods.GetGroupSelectionPath(lstGroupsSelection, lstEmployeesSelection, oState)
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

    Public Function GetTree(ByVal strFilterUserFields As String, ByVal strFeature As String, ByVal strType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupTree)) Implements IEmployeeGroupSvc.GetTree

        Return EmployeeGroupMethods.GetTree(strFilterUserFields, strFeature, strType, oState)
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

    Public Function GetUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupUserField) Implements IEmployeeGroupSvc.GetUserField

        Return EmployeeGroupMethods.GetUserField(IDGroup, UserFieldName, oState)
    End Function

    ''' <summary>
    ''' Obtiene todos los valores de los campos de la ficha del grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roGroupUserField' con los valores de los campos de la ficha.</returns>
    ''' <remarks></remarks>

    Public Function GetUserFields(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupUserField)) Implements IEmployeeGroupSvc.GetUserFields

        Return EmployeeGroupMethods.GetUserFields(IDGroup, oState)
    End Function
    '' _
    ''Public Function GetUserFieldsList(ByRef oState As roEmployeeState) As roUserFields
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

    Public Function SaveUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal UserFieldValue As Object, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SaveUserField

        Return EmployeeGroupMethods.SaveUserField(IDGroup, UserFieldName, UserFieldValue, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda los valores de los campos de la ficha del grupo. <br/>
    ''' Si hay la licencia de prevención de riesgos laborales activa, notifica al servidor para que lanze el proceso broadcaster.
    ''' </summary>
    ''' <param name="_IDGroup">Código grupo.</param>
    ''' <param name="_UserFields">Lista de objetos 'roGroupUserField' con los valores de los campos de la ficha a actualizar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveUserFields(ByVal _IDGroup As Integer, ByVal _UserFields As Generic.List(Of roGroupUserField), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SaveUserFields

        Return EmployeeGroupMethods.SaveUserFields(_IDGroup, _UserFields, oState)
    End Function

    ''' <summary>
    ''' Obtiene todos los valores de los campos de la ficha de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: FieldCaption, FieldName, Type, Value, ValueDateTime, Category, AccessLevel, Description, AccessValidation </returns>
    ''' <remarks></remarks>

    Public Function GetUserFieldsDataset(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet) Implements IEmployeeGroupSvc.GetUserFieldsDataset

        Return EmployeeGroupMethods.GetUserFieldsDataset(IDGroup, oState)
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

    Public Function GetGroupByNameWithCompany(ByVal strGroupName As String, ByVal intIDCompany As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup) Implements IEmployeeGroupSvc.GetGroupByNameWithCompany

        Return EmployeeGroupMethods.GetGroupByNameWithCompany(strGroupName, intIDCompany, oState)
    End Function

    ''' <summary>
    ''' Obtiene la definición de la empresa por nombre. <br/>
    ''' * Se audita la consulta.
    ''' </summary>
    ''' <param name="strCompanyName">Nombre empresa.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roGroup' con la definición de la empresa.</returns>
    ''' <remarks></remarks>

    Public Function GetCompanyByName(ByVal strCompanyName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup) Implements IEmployeeGroupSvc.GetCompanyByName

        Return EmployeeGroupMethods.GetCompanyByName(strCompanyName, oState)
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

    Public Function GetGroupByNameInLevel(ByVal strGroupName As String, ByVal strPath As String, ByVal oState As roWsState) As roGenericVtResponse(Of roGroup) Implements IEmployeeGroupSvc.GetGroupByNameInLevel

        Return EmployeeGroupMethods.GetGroupByNameInLevel(strGroupName, strPath, oState)
    End Function

#End Region


#Region "Indicators"

    ''' <summary>
    ''' Obtiene todos los valores de los campos de la ficha del grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roGroupUserField' con los valores de los campos de la ficha.</returns>
    ''' <remarks></remarks>

    Public Function GetGroupIndicators(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGroupIndicator)) Implements IEmployeeGroupSvc.GetGroupIndicators

        Return EmployeeGroupMethods.GetGroupIndicators(IDGroup, oState)
    End Function

    ''' <summary>
    ''' Obtiene todos los indicadores de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: ID, Name, Description</returns>
    ''' <remarks></remarks>

    Public Function GetGroupIndicatorsDataset(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet) Implements IEmployeeGroupSvc.GetGroupIndicatorsDataset
        Return EmployeeGroupMethods.GetGroupIndicatorsDataset(IDGroup, oState)
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

    Public Function SaveGroupIndicators(ByVal _IDGroup As Integer, ByVal _IDsIndicators As Generic.List(Of Integer), ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SaveGroupIndicators

        Return EmployeeGroupMethods.SaveGroupIndicators(_IDGroup, _IDsIndicators, oState, bAudit)
    End Function



    Public Function RegeneratePasswordsToEmployees(ByVal lstGroups As Generic.List(Of Integer), ByVal lstEmployees As Generic.List(Of Integer), ByVal Feature As String,
                                                       ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String)) Implements IEmployeeGroupSvc.RegeneratePasswordsToEmployees

        Return EmployeeGroupMethods.RegeneratePasswordsToEmployees(lstGroups, lstEmployees, Feature, strFilters, strFilterUserFields, oState)
    End Function


    Public Function SetBloquedAccessAppToEmployees(ByVal lstGroups As Generic.List(Of Integer), ByVal lstEmployees As Generic.List(Of Integer), ByVal Feature As String,
                                                 ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SetBloquedAccessAppToEmployees

        Return EmployeeGroupMethods.SetBloquedAccessAppToEmployees(lstGroups, lstEmployees, Feature, strFilters, strFilterUserFields, LockAccess, oState)
    End Function


    Public Function SetAppConfigurationToEmployees(ByVal lstGroups As Generic.List(Of Integer), ByVal lstEmployees As Generic.List(Of Integer), ByVal Feature As String,
                                                 ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByVal oState As roWsState, ByVal FromEmployee As Integer) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SetAppConfigurationToEmployees

        Return EmployeeGroupMethods.SetAppConfigurationToEmployees(lstGroups, lstEmployees, Feature, strFilters, strFilterUserFields, LockAccess, oState, FromEmployee)
    End Function


    Public Function SetPermissionsToEmployees(ByVal lstGroups As Generic.List(Of Integer), ByVal lstEmployees As Generic.List(Of Integer), ByVal Feature As String,
                                                 ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByVal oState As roWsState, ByVal FromEmployee As Integer) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.SetPermissionsToEmployees

        Return EmployeeGroupMethods.SetPermissionsToEmployees(lstGroups, lstEmployees, Feature, strFilters, strFilterUserFields, LockAccess, oState, FromEmployee)
    End Function

#End Region

#Region "SaaS Service Administration Status"

    Public Function ActivateService(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.ActivateService
        Return EmployeeGroupMethods.ActivateService(oState)
    End Function


    Public Function CancelService(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.CancelService
        Return EmployeeGroupMethods.CancelService(oState)
    End Function


    Public Function RegenerateAllPasswords(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeGroupSvc.RegenerateAllPasswords
        Return EmployeeGroupMethods.RegenerateAllPasswords(oState)
    End Function


    Public Function GetServiceStatus(ByVal serviceStatus As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Boolean)) Implements IEmployeeGroupSvc.GetServiceStatus
        Return EmployeeGroupMethods.GetServiceStatus(serviceStatus, oState)
    End Function
#End Region


End Class
