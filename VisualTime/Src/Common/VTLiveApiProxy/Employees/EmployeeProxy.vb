Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Security
Imports Robotics.VTBase

Public Class EmployeeProxy
    Implements IEmployeeSvc

    Public Function KeepAlive() As Boolean Implements IEmployeeSvc.KeepAlive
        Return True
    End Function


#Region "Employee"

    ''' <summary>
    ''' Devuelve un dataset con los datos de los empleados de la lista pasada por parámetro<br/>        
    ''' Si la lista pasada no contiene ninguna coma se considera que se quiere hacer un where por un valor.
    ''' </summary>
    ''' <param name="List">Lista de códigos de empleado separados por comas (ej: 1,4,5,14,23) o cláusula where.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Employees.*, sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>
    Public Function GetEmployeeFromList(ByVal List As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeeFromList
        Return EmployeeMethods.GetEmployeeFromList(List, oState)
    End Function


    Public Function GetEmployeesNotAllowed(ByVal IdPassport As Integer, ByVal idApplication As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IEmployeeSvc.GetEmployeesNotAllowed
        Return EmployeeMethods.GetEmployeesNotAllowed(IdPassport, idApplication, oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los empleados de la lista pasada por parámetro y que esten dentro de la funcionalidad de seguridad del pasaporte activo. <br/>
    ''' Si la lista pasada no contiene ninguna coma se considera que se quiere hacer un where por un valor.        
    ''' </summary>
    ''' <param name="strListIDs">Lista de códigos de empleado separados por comas (ej: 1,4,5,14,23) o cláusula where.</param>
    ''' <param name="strFeature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="strType">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Employees.*, sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>
    Public Function GetEmployeeFromListWithType(ByVal strListIDs As String, ByVal strFeature As String, ByVal strType As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeeFromListWithType
        Return EmployeeMethods.GetEmployeeFromListWithType(strListIDs, strFeature, strType, oState)
    End Function

    ''' <summary>
    ''' Devuelve la información de un empleado.
    ''' </summary>
    ''' <param name="IdEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetEmployee(ByVal IdEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployee) Implements IEmployeeSvc.GetEmployee
        Return EmployeeMethods.GetEmployee(IdEmployee, oState, bAudit)
    End Function


    Public Function GetEmployeeName(ByVal IdEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IEmployeeSvc.GetEmployeeName
        Return EmployeeMethods.GetEmployeeName(IdEmployee, oState)
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el código de contrato indicado.
    ''' </summary>
    ''' <param name="IdContract">Código de contrato.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeByContract(ByVal IdContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee) Implements IEmployeeSvc.GetEmployeeByContract
        Return EmployeeMethods.GetEmployeeByContract(IdContract, oState)
    End Function

    ''' <summary>
    ''' Devuelve el código del empleado con el contrato indicado.
    ''' </summary>
    ''' <param name="IdContract">Código de contrato del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Código del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetIdEmployeeByContract(ByVal IdContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IEmployeeSvc.GetIdEmployeeByContract
        Return EmployeeMethods.GetIdEmployeeByContract(IdContract, oState)
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el nombre indicado.
    ''' </summary>
    ''' <param name="strName">Nombre del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeByName(ByVal strName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee) Implements IEmployeeSvc.GetEmployeeByName
        Return EmployeeMethods.GetEmployeeByName(strName, oState)
    End Function

    ''' <summary>
    ''' Devuelve el código del empleado con el nombre indicado.
    ''' </summary>
    ''' <param name="strName">Nombre del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Código interno del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetIdEmployeeByName(ByVal strName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IEmployeeSvc.GetIdEmployeeByName
        Return EmployeeMethods.GetIdEmployeeByName(strName, oState)
    End Function

    ''' <summary>
    ''' Comprueba los datos del empleado:<br/>
    ''' - Verifica que el código no exista<br/>
    ''' - Que tenga nombre informado<br/>
    ''' - Que sea de tipo 'A' o 'J'<br/>
    ''' - Si tiene asignado un grupo de accesos que sea un grupo correcto.     
    ''' </summary>
    ''' <param name="Employee">Objeto 'roEmployee' con la información del empleado a validar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False </returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function ValidateEmployee(ByVal Employee As roEmployee, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.ValidateEmployee
        Return EmployeeMethods.ValidateEmployee(Employee, oState)
    End Function

    ''' <summary>
    ''' Guarda la información del empleado en la base de datos, verificando la información previamente ('ValidateEmployee'). <br/>
    ''' Si el empleado ya existe actualiza su información.<br/>
    ''' Genera o actualiza el passaport asociado al empleado y genera la información de auditoría correspondiente.
    ''' </summary>
    ''' <param name="Employee">Información del empleado a grabar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveEmployee(ByVal Employee As roEmployee, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee) Implements IEmployeeSvc.SaveEmployee
        Return EmployeeMethods.SaveEmployee(Employee, oState)
    End Function

    ''' <summary>
    ''' Borra los datos del empleado indicado. Verifica que el empleado no tenga fichajes de producción ni saldos de producción.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False.</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteEmployee(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.DeleteEmployee

        Return EmployeeMethods.DeleteEmployee(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Comprueba si un empleado tiene datos en visualtime para decidir si se permite borrar o no.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False.</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CheckIfEmployeeHasData(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.CheckIfEmployeeHasData
        Return EmployeeMethods.CheckIfEmployeeHasData(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Devuelve el número de empleados activos a una fecha, utilizando la definición de los contratos.         
    ''' </summary>
    ''' <param name="xDate">Fecha a la que se quiere obtener el número de empleados activos.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados activos a una fecha.</returns>
    ''' <remarks></remarks>

    Public Function GetActiveEmployeesCount(ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IEmployeeSvc.GetActiveEmployeesCount
        Return EmployeeMethods.GetActiveEmployeesCount(xDate, oState)
    End Function

    ''' <summary>
    ''' Devuelve el número de empleados de tareas activos a una fecha, utilizando la definición de los contratos.         
    ''' </summary>
    ''' <param name="xDate">Fecha a la que se quiere obtener el número de empleados activos tipo J.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados activos a una fecha.</returns>
    ''' <remarks></remarks>

    Public Function GetActiveEmployeesTaskCount(ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IEmployeeSvc.GetActiveEmployeesTaskCount
        Return EmployeeMethods.GetActiveEmployeesTaskCount(xDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados en una tabla. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, EmployeeContracts.IDContract, EmployeeContracts.IDCard</returns>
    ''' <remarks></remarks>

    Public Function GetEmployees(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployees
        Return EmployeeMethods.GetEmployees(strWhere, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por nombre. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre del empleado.</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where.</param>
    ''' <param name="strFeature">Feature para comprobar seguridad de empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesByName(ByVal strLikeName As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesByName
        Return EmployeeMethods.GetEmployeesByName(strLikeName, strWhere, strFeature, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por nombre. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strAdvFilter">Filtro avanzado que se aplica para buscar empleados.</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where.</param>
    ''' <param name="strFeature">Feature para comprobar seguridad de empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesByAdvancedFilter(ByVal strAdvFilter As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesByAdvancedFilter
        Return EmployeeMethods.GetEmployeesByAdvancedFilter(strAdvFilter, strWhere, strFeature, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por código de contrato. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeIDContract">Filtro que se aplica al código de contrato</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, EmployeeContracts.IDContract </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesByIDContract(ByVal strLikeIDContract As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesByIDContract
        Return EmployeeMethods.GetEmployeesByIDContract(strLikeIDContract, strWhere, strFeature, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por número de tarjeta. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al número de tarjeta</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS IDCard</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesByIDCard(ByVal strLikeName As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesByIDCard
        Return EmployeeMethods.GetEmployeesByIDCard(strLikeName, strWhere, strFeature, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por matrícula. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikePlate">Filtro que se aplica a la matrícula</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS Plate</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesByPlate(ByVal strLikePlate As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesByPlate
        Return EmployeeMethods.GetEmployeesByPlate(strLikePlate, strWhere, strFeature, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados que tiene el id.
    ''' </summary>
    ''' <param name="IdEmployee">Id del empleado a buscar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesById(ByVal IdEmployee As Integer, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesById
        Return EmployeeMethods.GetEmployeesById(IdEmployee, strFeature, oState)
    End Function

    ''' <summary>
    ''' Genera múltiples empleados.
    ''' </summary>
    ''' <param name="EmployeesData">Información de los empleados: nombre (EmployeeName - String), código tarjeta (IDCard - Long), código contrato (IDContract - String), fecha inicio (BeginDate - DateTime), tipo empleado ('A', 'J') (EmployeeType - String), código grupo (IDGroup - Integer), ficha con trajeta (CardMethod - Boolean), ficha con huella (BiometricMethod - Boolean), combinación métodos (MergeMethod - Integer)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="lstEmployeeNameError">Devuelve la lista de nombre de empleados que no se han podido guardar.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CreateMultiEmployees(ByVal EmployeesData As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String)) Implements IEmployeeSvc.CreateMultiEmployees
        Return EmployeeMethods.CreateMultiEmployees(EmployeesData, oState)
    End Function


    ''' <summary>
    ''' Devuelve un array de bytes del fichero especificado como paramentro.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns>Array de Bytes o nothing si hay algun error</returns>
    ''' <remarks></remarks>

    Public Function GetDocumentToView(ByVal strFileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IEmployeeSvc.GetDocumentToView
        Return EmployeeMethods.GetDocumentToView(strFileName, oState)
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el código de contrato indicado.
    ''' </summary>
    ''' <param name="IdEmployee">Id del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeSummary(ByVal IdEmployee As Integer, ByVal onDate As Date, ByVal accrualSummaryType As SummaryType, ByVal causesSummaryType As SummaryType, ByVal tasksSummaryType As SummaryType, ByVal centersSummaryType As SummaryType,
                                       ByVal requestType As SummaryRequestType, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeSummary) Implements IEmployeeSvc.GetEmployeeSummary

        Return EmployeeMethods.GetEmployeeSummary(IdEmployee, onDate, accrualSummaryType, causesSummaryType, tasksSummaryType, centersSummaryType, requestType, oState)
    End Function
#End Region

#Region "Mobility"

    ''' <summary>
    ''' Obtiene el nombre del completo del grupo al que pertenece el empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Cadena con el nombre completo del grupo al que pertenece el empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetCurrentFullGroupName(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IEmployeeSvc.GetCurrentFullGroupName
        Return EmployeeMethods.GetCurrentFullGroupName(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene la información del grupo actual al que pertenece el empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roMobility' con la información del grupo actual al que pertenece el empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetCurrentMobility(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMobility) Implements IEmployeeSvc.GetCurrentMobility
        Return EmployeeMethods.GetCurrentMobility(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene la información de la mobilidad de un empleado para un grupo.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="IdGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roMobility' con la información de la mobilidad.</returns>
    ''' <remarks></remarks>

    Public Function GetMobility(ByVal IDEmployee As Integer, ByVal IdGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMobility) Implements IEmployeeSvc.GetMobility
        Return EmployeeMethods.GetMobility(IDEmployee, IdGroup, oState)
    End Function

    ''' <summary>
    ''' Devuelve una tabla con la información de la mobilidad de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDGroup, Name, BeginDate, EndDate </returns>
    ''' <remarks></remarks>

    Public Function GetMobilities(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetMobilities
        Return EmployeeMethods.GetMobilities(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Valida la información de la mobilidad para un empleado. 
    ''' Verifica que el grupo indicado exista y que el perido de fechas de validez de la mobilidad sea correcto.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="Mobility">Object 'roMobility' con la información de la mobilidad.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function ValidateMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.ValidateMobility
        Return EmployeeMethods.ValidateMobility(IDEmployee, Mobility, oState)
    End Function

    ''' <summary>
    ''' Valida la información de las mobilidades para un empleado.
    ''' Para cada mobilidad verifica que:
    ''' - La fecha de inicio no sea anterior al inicio de contrato del empleado
    ''' - Es el primer registro de la mobilidad, debe ser igual a la fecha de inicio del contrato
    ''' - El grupo sea correcto
    ''' - No se repita el mismo grupo en dos o más registros consecutivos
    ''' Para cada mobilidad informa la fecha final.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="dsMobilities">Tabla con la información de las mobilidades a verificar: IDGroup, BeginDate, EndDate (la fecha final la informa la propia función)</param>
    ''' <param name="intInvalidRow">Devuelve el ínidice de la fila errónea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function ValidateMobilities(ByVal IDEmployee As Integer, ByVal dsMobilities As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of roMobilityValidation) Implements IEmployeeSvc.ValidateMobilities
        Return EmployeeMethods.ValidateMobilities(IDEmployee, dsMobilities, oState)
    End Function

    ''' <summary>
    ''' Añade una mobilidad a la definición de mobilidades de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="Mobility">Objeto 'roMobility' con la información de la mobilidad a añadir.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByVal oState As roWsState, ByVal CallBroadcaster As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveMobility
        Return EmployeeMethods.SaveMobility(IDEmployee, Mobility, oState, CallBroadcaster)
    End Function

    ''' <summary>
    ''' Guarda la definición de mobilidades para un empleado. 
    ''' Sustituye la información de mobilidades actual.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="dsMobilities">Tabla con las mobilidades del empleado a guardar: IDGroup, BeginDate, EndDate </param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveMobilities(ByVal IDEmployee As Integer, ByVal dsMobilities As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveMobilities
        Return EmployeeMethods.SaveMobilities(IDEmployee, dsMobilities, oState)
    End Function


    Public Function UpdateEmployeeGroup(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal FromDate As Date, ByVal pCopyPlan As Boolean, ByVal SourceIDEmployee As Integer,
                                            ByVal intShiftType As ActionShiftType, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction,
                                            ByVal _ShiftPermissionAction As ShiftPermissionAction, ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.UpdateEmployeeGroup

        Return EmployeeMethods.UpdateEmployeeGroup(IDEmployee, IDGroup, FromDate, pCopyPlan, SourceIDEmployee, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState)
    End Function


    Public Function GetEmployeeLockDatetoApply(ByVal IDEmployee As Integer, ByVal EmployeeLockDateType As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeLockDateInfo) Implements IEmployeeSvc.GetEmployeeLockDatetoApply
        Return EmployeeMethods.GetEmployeeLockDatetoApply(IDEmployee, EmployeeLockDateType, oState, bAudit)
    End Function

    Public Function SaveEmployeeLockDate(ByVal IDEmployee As Integer, ByVal LockDate As Date, ByVal EmployeeLockDateType As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveEmployeeLockDate
        Return EmployeeMethods.SaveEmployeeLockDate(IDEmployee, LockDate, EmployeeLockDateType, oState, bAudit)
    End Function


    Public Function SaveLockDate(ByVal xLockDate As Date, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveLockDate
        Return EmployeeMethods.SaveLockDate(xLockDate, oState, bAudit)
    End Function


    Public Function GetEmployeesOnLockDate(ByVal strEmployeeFilter As String, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetEmployeesOnLockDate
        Return EmployeeMethods.GetEmployeesOnLockDate(strEmployeeFilter, xDate, oState)
    End Function

#End Region

#Region "UserFields"

    ''' <summary>
    ''' Obtiene el valor de un campo de la ficha de un empleado a un fecha en concreto. 
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="UserFieldName">Nombre del campo de la ficha.</param>
    ''' <param name="xDate">Fecha en la que se quiere obtener el valor.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Un objeto 'roEmployeeUserField' con la información del valor a una fecha del campo de la ficha.</returns>
    ''' <remarks></remarks>

    Public Function GetUserField(ByVal IDEmployee As Integer, ByVal UserFieldName As String, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeUserField) Implements IEmployeeSvc.GetUserField
        Return EmployeeMethods.GetUserField(IDEmployee, UserFieldName, xDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de valores de campos de la ficha del empleado a una fecha en concreto.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha en la que se quiere obtener los valores</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Una lista de objetos 'roEmployeeUserField' con la información del valor a una fecha de los campos de la ficha.</returns>
    ''' <remarks></remarks>

    Public Function GetUserFields(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roEmployeeUserField)) Implements IEmployeeSvc.GetUserFields
        Return EmployeeMethods.GetUserFields(IDEmployee, xDate, oState)
    End Function
    ''<WebMethod()> _
    ''Public Function GetUserFieldsList(ByVal oState As roWsState) As roGenericVtResponse(Of roUserFields
    ''    If oState Is Nothing Then oState = New roEmployeeState(-1, Me.Context)
    ''    oState.UpdateStateInfo(Me.Context)
    ''    Dim oUserFields As roUserFields = oEmployeeConector.GetUserFieldsList(oState)
    ''    Return oUserFields
    ''End Function

    ''' <summary>
    ''' Guarda el valor de un campo de la ficha del empleado para una fecha. <br/>
    ''' Verifica que la fecha no este dentro del priodo de congelación.<br/>
    ''' Lanza el proceso de recálculo correspondiente en función de si el campo se utiliza en algún proceso.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="UserFieldName">Nombre del campo de la ficha.</param>
    ''' <param name="UserFieldDate">Fecha en la que tiene valor el campo de la ficha.</param>
    ''' <param name="UserFieldValue">Valor del campo de la ficha.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveUserField(ByVal IDEmployee As Integer, ByVal UserFieldName As String, ByVal UserFieldDate As Date, ByVal UserFieldValue As Object, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveUserField
        Return EmployeeMethods.SaveUserField(IDEmployee, UserFieldName, UserFieldDate, UserFieldValue, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda los valores de los campos de la ficha de un empleado.<br/>
    ''' Para cada valor de campo verifica que la fecha no este dentro del periodo de congelación.<br/>
    ''' Para cada campo modificado lanza el recálculo correspondiente en función de si el campo se utiliza en algún proceso.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado.</param>
    ''' <param name="_UserFields">Lista de objetos 'roEmployeeUserField' con la información de los valores de los campos.</param>
    ''' <param name="oState">Información adición de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveUserFields(ByVal _IDEmployee As Integer, ByVal _UserFields As Generic.List(Of roEmployeeUserField), ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveUserFields
        Return EmployeeMethods.SaveUserFields(_IDEmployee, _UserFields, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene todos los valores de los campos de la ficha de un empleado en una fecha.<br/>
    ''' Tiene en cuenta la configuración de seguridad de los campos de la ficha del passport actual y el nivel de acceso de cada campo, devolviendo solo los que se tiene permiso.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha de los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: FieldCaption, FieldName, Type, Value, OriginalDate, Value, ValueDateTime, Category, AccessLevel, Description, AccessValidation, History</returns>
    ''' <remarks></remarks>

    Public Function GetUserFieldsDataset(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet) Implements IEmployeeSvc.GetUserFieldsDataset
        Return EmployeeMethods.GetUserFieldsDataset(IDEmployee, xDate, oState)
    End Function

    ''' <summary>
    ''' Obteien el histórico de valores de un campo de la ficha de un empleado.<br/>
    ''' Tiene en cuenta la configuración de seguridad de los campos de la ficha del passport actual y el nivel de acceso de cada campo, devolviendo solo los que se tiene permiso.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="FieldName">Nombre del campo de la ficha.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con los columnas: IDEmployee, FieldName, Date, Value, OriginalDate </returns>
    ''' <remarks></remarks>

    Public Function GetUserFieldHistoryDataset(ByVal IDEmployee As Integer, ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetUserFieldHistoryDataset
        Return EmployeeMethods.GetUserFieldHistoryDataset(IDEmployee, FieldName, oState)
    End Function

    ''' <summary>
    ''' Guarda el histórico de valores de un campo de la ficha de un empleado.<br/>
    ''' Se recorren las filas de la tabla pasado y en función del estado de la fila se actualiza el histórico del campo.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="tbHistory">Tabla con la información del histórico de valores del campo de la ficha: DEmployee, FieldName, Date, Value, OriginalDate </param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveUserFieldHistory(ByVal IDEmployee As Integer, ByVal tbHistory As DataTable, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveUserFieldHistory
        Return EmployeeMethods.SaveUserFieldHistory(IDEmployee, tbHistory, oState)
    End Function

    ''' <summary>
    ''' Elimina el histórico de valores de un campo de la ficha de empleado a partir de una fecha.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="_FieldName">Nombre del campo de la ficha.</param>
    ''' <param name="_FromDate">Fecha a partir de la que se quiere borrar el histórico. Esta fecha se incluye en el borrado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteUserFieldHistory(ByVal IDEmployee As Integer, ByVal _FieldName As String, ByVal _FromDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.DeleteUserFieldHistory
        Return EmployeeMethods.DeleteUserFieldHistory(IDEmployee, _FieldName, _FromDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene el valor de un campo de la ficha para un empleado vigente a una fecha en concreto.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado.</param>
    ''' <param name="_FieldName">Nombre del campo de la ficha.</param>
    ''' <param name="_Date">Fecha en la que es vigente el valor.</param>
    ''' <param name="oState"></param>        
    ''' <returns>Objeto 'roEmployeeUserField' con el valor del campo de la ficha.</returns>
    ''' <remarks></remarks>        

    Public Function GetEmployeeUserFieldValueAtDate(ByVal _IDEmployee As String, ByVal _FieldName As String, ByVal _Date As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeUserField) Implements IEmployeeSvc.GetEmployeeUserFieldValueAtDate
        Return EmployeeMethods.GetEmployeeUserFieldValueAtDate(_IDEmployee, _FieldName, _Date, oState)
    End Function

#End Region

#Region "TerminalMessages"

    ''' <summary>
    ''' Obtiene los mensajes a mostrar por los terminales de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: IDEmployee, Message, Schedule, LastTimeShown, ForAllEmployees </returns>
    ''' <remarks></remarks>

    Public Function GetTerminalMessages(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet) Implements IEmployeeSvc.GetTerminalMessages
        Return EmployeeMethods.GetTerminalMessages(IDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene las definición de un mensaje de terminal de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="TerminalMessage">Mensaje terminal.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roTerminalMessage' con la información del mensaje.</returns>
    ''' <remarks></remarks>

    Public Function GetTerminalMessage(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByVal oState As roWsState) As roGenericVtResponse(Of roTerminalMessage) Implements IEmployeeSvc.GetTerminalMessage
        Return EmployeeMethods.GetTerminalMessage(IDEmployee, TerminalMessage, oState)
    End Function

    ''' <summary>
    ''' Borra los datos de un mensaje de terminal para un empleado.<br/>
    ''' Si el mensaje esta marcado como "Para Todos Los Empleados" no se tiene en cuenta el código de empleado y se borran ese mensaje para todos los empleados.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="TerminalMessage">Mensaje terminal</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteTerminalMessages(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.DeleteTerminalMessages
        Return EmployeeMethods.DeleteTerminalMessages(IDEmployee, TerminalMessage, ID, oState)
    End Function

    ''' <summary>
    ''' Valida la definición de un mensaje de terminal para un empleado.<br/>
    ''' Verifica que el mensaje tenga asignada una programación.
    ''' </summary>
    ''' <param name="IdEmployee">Código empleado.</param>
    ''' <param name="TerminalMessage">Definición mensaje terminal.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function ValidateTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.ValidateTerminalMessage
        Return EmployeeMethods.ValidateTerminalMessage(IdEmployee, TerminalMessage, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos del mensaje de terminal.<br/>
    ''' Si esta marcado ForAllEmployees se genera una copia de ese mensaje para todos los empleados.
    ''' </summary>
    ''' <param name="IdEmployee">Código empleado.</param>
    ''' <param name="TerminalMessage">Definición mensaje terminal.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveTerminalMessage
        Return EmployeeMethods.SaveTerminalMessage(IdEmployee, TerminalMessage, oState)
    End Function

#End Region

#Region "Schedule"

    ''' <summary>
    ''' Copia la planificación de horarios de un empleado a otro, indicando una fecha inicio a partir de la que se copiará.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
    ''' </summary>
    ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
    ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
    ''' <param name="xBeginPeriod">Fecha inicial de la planificación.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    '''  </param>
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="copyHolidays">Indica si se deben copiar los dias de vacaciones.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyPlan(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyPlan

        Return EmployeeMethods.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xBeginPeriod, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState, bAudit)
    End Function

    ''' <summary>
    ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
    ''' </summary>
    ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
    ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
    ''' <param name="xBeginPeriod">Fecha inicio del periodo a copiar.</param>
    ''' <param name="xEndPeriod">Fecha fin del periodo a copiar.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    '''  </param>
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyShifts(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                   ByVal intShiftType As ActionShiftType,
                                   ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                   ByVal xDateLocked As Date, ByVal oState As roWsState, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyShifts

        Return EmployeeMethods.CopyShifts(intSourceIDEmployee, intDestinationIDEmployee, xBeginPeriod, xEndPeriod, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, oState, copyHolidays, bAudit)
    End Function



    ''' <summary>
    ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo origen y un fecha destino.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
    ''' </summary>
    ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
    ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
    ''' <param name="xSourceBeginPeriod">Fecha inicio del periodo a copiar.</param>
    ''' <param name="xSourceEndPeriod">Fecha fin del periodo a copiar.</param>
    ''' <param name="xDestinationBeginPeriod">Fecha inicio del periodo al que se copiará.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    '''  </param>
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyShiftsPeriod(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBeginPeriod As Date, ByVal xSourceEndPeriod As Date,
                                         ByVal xDestinationBeginPeriod As Date, ByVal intShiftType As ActionShiftType,
                                         ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                         ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyShiftsPeriod

        Return EmployeeMethods.CopyShiftsPeriod(intSourceIDEmployee, intDestinationIDEmployee, xSourceBeginPeriod, xSourceEndPeriod, xDestinationBeginPeriod, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState)
    End Function

    ''' <summary>
    ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo origen y un periodo destino.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
    ''' </summary>
    ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
    ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
    ''' <param name="xSourceBeginPeriod">Fecha inicio del periodo a copiar.</param>
    ''' <param name="xSourceEndPeriod">Fecha fin del periodo a copiar.</param>
    ''' <param name="xDestinationBeginPeriod">Fecha inicio del periodo al que se copiará.</param>
    ''' <param name="xDestinationEndPeriod">Fecha fin del periodo al que se copiará.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    '''  </param>
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyShiftsPeriodWithEnd(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBeginPeriod As Date,
                                                ByVal xSourceEndPeriod As Date, ByVal xDestinationBeginPeriod As Date, ByVal xDestinationEndPeriod As Date,
                                                ByVal intShiftType As ActionShiftType,
                                                ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyShiftsPeriodWithEnd

        Return EmployeeMethods.CopyShiftsPeriodWithEnd(intSourceIDEmployee, intDestinationIDEmployee, xSourceBeginPeriod, xSourceEndPeriod, xDestinationBeginPeriod, xDestinationEndPeriod, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState, bAudit)
    End Function

    ''' <summary>
    ''' Copia la planificación de horarios de unos empleados a otros. Indicando una fecha origen y otra destino.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
    ''' </summary>
    ''' <param name="SourceIDEmployees">Lista de códigos de los empleados origen.</param>
    ''' <param name="DestinationIDEmployees">Lista de códigos de los empleados destino.</param>
    ''' <param name="xSourceDate">Fecha origen.</param>
    ''' <param name="xDestinationDate">Fecha destino.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>     
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyShiftsEmployees(ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date,
                                            ByVal intShiftType As ActionShiftType,
                                            ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                            ByVal intIDEmployeeLocked As Integer, ByVal oState As roWsState,
                                            ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyShiftsEmployees

        Return EmployeeMethods.CopyShiftsEmployees(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, oState, copyHolidays, bAudit)
    End Function

    ''' <summary>
    ''' Copia la planificación de horarios de unos empleados a otros, indicando una fecha origen y un periodo destino. Los horarios de la fecha origen se repetiran en el periodo destino.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
    ''' </summary>
    ''' <param name="SourceIDEmployees">Lista de códigos de los empleados origen.</param>
    ''' <param name="DestinationIDEmployees">Lista de códigos de los empleados destino.</param>
    ''' <param name="xSourceDate">Fecha origen.</param>
    ''' <param name="xDestinationDate">Fecha inicio destino.</param>
    ''' <param name="xDestinationEndDate">Fecha fin destino.</param>
    ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>   
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar.</param>
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar para el empleado bloqueado (intIDEmployeeLocked).</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyShiftsEmployeesWithEnd(ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date,
                                                   ByVal xDestinationEndDate As Date, ByVal intShiftType As ActionShiftType,
                                                   ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                   ByVal intIDEmployeeLocked As Integer, ByVal xDateLocked As Date, ByVal oState As roWsState, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyShiftsEmployeesWithEnd

        Return EmployeeMethods.CopyShiftsEmployeesWithEnd(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, xDestinationEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, xDateLocked, oState, copyHolidays, bAudit)
    End Function

    ''' <summary>
    ''' Planifica la lista de horarios a un empleado entre fechas.<br></br>
    ''' Si es necesario, la lista de horarios se asigna de forma cíclica.<br></br>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
    ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.
    ''' </summary>
    ''' <param name="lstShifts">Lista de horarios (IDShift1*IDShift2*IDShift3*IDShift4).</param>
    ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
    ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
    ''' <param name="xEndDate">Fecha final de planificación.</param>
    ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function CopyListShifts(ByVal lstShifts As Generic.List(Of String), ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal _
                                       intShiftType As ActionShiftType,
                                       ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                       ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyListShifts

        Return EmployeeMethods.CopyListShifts(lstShifts, intDestinationIDEmployee, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState, bAudit)
    End Function



    ''' <summary>
    ''' Planifica la lista de horarios a varios empleado entre fechas.<br></br>
    ''' La lista de horarios corresponde a los horarios a asignar para una misma fecha para los distintos empleados. Si es necesario, la lista de horarios se asigna de forma cíclica para una misma fecha. Si hay varios días a planificar, se repite la lista para cada día.<br></br>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
    ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.<br></br>
    ''' La forma de asignar los horarios es la siguiente: por cada fecha del periodo se recorre todos los empleados a planificar y les asigna el/los horarios correspondientes.
    ''' </summary>
    ''' <param name="lstShifts">Lista de horarios (IDShift1*IDShift2*IDShift3*IDShift4).</param>
    ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a planificar.</param>
    ''' <param name="xBeginDate">Fecha inicio de planificación.</param>
    ''' <param name="xEndDate">Fecha fin de planificación.</param>
    ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>  
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
    ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function CopyListShiftsEmployees(ByVal lstShifts As Generic.List(Of String), ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date,
                                                ByVal xEndDate As Date, ByVal intShiftType As ActionShiftType,
                                                ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                ByVal xDateLocked As Date, ByVal intIDEmployeeLocked As Integer, ByVal isHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.CopyListShiftsEmployees

        Return EmployeeMethods.CopyListShiftsEmployees(lstShifts, lstDestionationIDEmployees, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, intIDEmployeeLocked, isHolidays, oState, bAudit)
    End Function

    ''' <summary>
    ''' Bloquea/desbloquea los días informados a un empleado entre fechas.<br></br>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' </summary>
    ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
    ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
    ''' <param name="xEndDate">Fecha final de planificación.</param>
    ''' <param name="xLocked">Bloquear y desbloquear días</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function LockDaysList(ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.LockDaysList
        Return EmployeeMethods.LockDaysList(intDestinationIDEmployee, xBeginDate, xEndDate, xLocked, oState)
    End Function

    ''' <summary>
    ''' Bloquea/desbloquea los días informados a un empleado entre fechas.<br></br>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
    ''' </summary>
    ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a planificar.</param>
    ''' <param name="xBeginDate">Fecha inicio de planificación.</param>
    ''' <param name="xEndDate">Fecha fin de planificación.</param>
    ''' <param name="xLocked">Bloqueado</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function LockDaysListEmployees(ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.LockDaysListEmployees

        Return EmployeeMethods.LockDaysListEmployees(lstDestionationIDEmployees, xBeginDate, xEndDate, xLocked, oState)
    End Function


    ''' <summary>
    ''' Obtiene la planificación de horarios de un empleado para un periodo.<br/>
    ''' Verifica que el passport actual tenga permisos (Calendar.Scheduler) sobre el empleado seleccionado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xBegin">Fecha inicio del periodo.</param>
    ''' <param name="xEnd">Fecha fin del periodo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Name1, Name2, Name3, Name4, Date, ShortName1, ShortName2, ShortName3, ShortName4, PrimaryColor, UsedColor, Status, IDShift1, IDShift2, IDShift3, IDShift4, IDEmployee "</returns>
    ''' <remarks></remarks>

    Public Function GetPlan(ByVal intIDEmployee As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetPlan

        Return EmployeeMethods.GetPlan(intIDEmployee, xBegin, xEnd, oState)
    End Function
    ''' <summary>
    ''' Obtiene el estado de calculo del proceso de tareas.<br/>
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Status, IDEmployee "</returns>
    ''' <remarks></remarks>

    Public Function GetTaskPlan(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetTaskPlan
        Return EmployeeMethods.GetTaskPlan(intIDEmployee, xDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene el estado de calculo del proceso de presencia.<br/>
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Status, IDEmployee "</returns>
    ''' <remarks></remarks>

    Public Function GetScheduleStatus(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetScheduleStatus
        Return EmployeeMethods.GetScheduleStatus(intIDEmployee, xDate, oState)
    End Function





    ''' <summary>
    ''' Planifica un horario (y sus alternativos si es necesario) a un empleado para una fecha.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha a planificar.</param>
    ''' <param name="intIDShift1">Código del horario principal. Si se quiere eliminar el horario informar -1.</param>
    ''' <param name="intIDShift2">Código del primer horario alternativo. Si no quiere modificar el horario alternativo informar 0. Si se quiere eliminar el horario informar -1.</param>
    ''' <param name="intIDShift3">Código del segundo horario alternativo. Si no quiere modificar el horario alternativo informar 0. Si se quiere eliminar el horario informar -1.</param>
    ''' <param name="intIDShift4">Código del tercer horario alternativo. Si no quiere modificar el horario alternativo informar 0. Si se quiere eliminar el horario informar -1.</param>
    ''' <param name="xStartShift1"></param>
    ''' <param name="xStartShift2"></param>
    ''' <param name="xStartShift3"></param>
    ''' <param name="xStartShift4"></param>
    ''' <param name="intIDAssignment">Código del puesto a asignar. Si se quiere eliminar el puesto informar -1.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>    
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function AssignShift(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal intIDShift1 As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer,
                                    ByVal intIDShift4 As Integer, ByVal xStartShift1 As DateTime, ByVal xStartShift2 As DateTime, ByVal xStartShift3 As DateTime,
                                    ByVal xStartShift4 As DateTime, ByVal intIDAssignment As Integer,
                                    ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                    ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.AssignShift

        Return EmployeeMethods.AssignShift(intIDEmployee, xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                    _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, oState, bAudit)
    End Function

    ''' <summary>
    ''' Asigna el horario como el primer alternativo no informado del empleado y fecha. <br/>
    ''' Si no hay horario principal informado o todos los alternativos ya están informados devuelve 'false'.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.        
    ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha a planificar.</param>
    ''' <param name="intIDAlterShift">Código del horario alternativo.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>               
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function AssignAlterShift(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal intIDAlterShift As Integer, ByVal xAlterStartShift As DateTime, ByVal _LockedDayAction As LockedDayAction, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.AssignAlterShift
        Return EmployeeMethods.AssignAlterShift(intIDEmployee, xDate, intIDAlterShift, xAlterStartShift, _LockedDayAction, oState)
    End Function

    ''' <summary>
    ''' Asigna una planificación semanal a unos empleados para un periodo.<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que los empleados tengan contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.        
    ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.<br></br>
    ''' La forma de asignar los horarios es la siguiente: por cada fecha del periodo se recorre todos los empleados a planificar y les asigna el/los horarios correspondientes.
    ''' </summary>
    ''' <param name="lstEmployees">Lista de códigos de empleado a planificar.</param>
    ''' <param name="lstWeekShifts">Lista con los códigos de horarios a planificar para una semana (7 valores). De lunes a domingo.</param>
    ''' <param name="xBeginDate">Fecha inicio del periodo a planificar.</param>
    ''' <param name="xEndDate">Fecha fin del periodo a planificar.</param>
    ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
    ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
    ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
    ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
    ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
    ''' </param>  
    ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param> 
    ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.</param>
    ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLocked).</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function AssignWeekShifts(ByVal lstEmployees As ArrayList, ByVal lstWeekShifts As ArrayList, ByVal lstWeekStartShifts As Generic.List(Of DateTime),
                                         ByVal lstWeekAssignments As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                         ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                         ByVal intIDEmployeeLocked As Integer, ByVal xDateLocked As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeCopyPlanResult) Implements IEmployeeSvc.AssignWeekShifts

        Return EmployeeMethods.AssignWeekShifts(lstEmployees, lstWeekShifts, lstWeekStartShifts, lstWeekAssignments, xBeginDate, xEndDate, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, xDateLocked, oState)
    End Function

    ''' <summary>
    ''' Elimina la planificación de un empleado para una fecha, indicando el horario a eliminar (-1 se eliminan todos: el principal y los alternativos).<br/>
    ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
    ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
    ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.        
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="intShiftPosition">Posicición del horario a eliminar: -1. Todos, 1. Horario Principal, 2. Primer horario alternativo, 3. Segundo horario alternativo, 4. Tercer horario alternativo.</param>
    ''' <param name="xDate">Fecha en la que se modificará la planificación.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function RemoveShift(ByVal intIDEmployee As Long, ByVal intShiftPosition As Integer, ByVal xDate As Date, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.RemoveShift
        Return EmployeeMethods.RemoveShift(intIDEmployee, intShiftPosition, xDate, _LockedDayAction, _CoverageDayAction, oState)
    End Function

    ''' <summary>
    ''' Obtiene el texto del comentario introducido en la pantalla de edición de fichajes para un empleado y fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Texto con el comentario.</returns>
    ''' <remarks></remarks>

    Public Function GetRemarkText(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IEmployeeSvc.GetRemarkText
        Return EmployeeMethods.GetRemarkText(intIDEmployee, xDate, oState)
    End Function

    ''' <summary>
    ''' Establece el texto del comentario para un empleado y fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="strText">Texto del comentario a guardar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SetRemarkText(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal strText As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SetRemarkText
        Return EmployeeMethods.SetRemarkText(intIDEmployee, xDate, strText, oState)
    End Function


#End Region

#Region "Authentication Methods"

    ''' <summary>
    ''' Obtiene los métodos de autentificación de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDPassport, Method (1-Password, 2-IntegratedSecurity, 3-Card, 4-Biometry, 5-Pin), version, Credential, Password, StartDate, ExpirationDate, BiometricID, BiometricData, TimeStamp, Enabled </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeAuthenticationMethods(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetEmployeeAuthenticationMethods
        Return EmployeeMethods.GetEmployeeAuthenticationMethods(IDEmployee, oState)
    End Function



#End Region

#Region "Presence Query Methods"

    ''' <summary>
    ''' Obtiene información de detalle de presencia para un empleado y un periodo.<br/>
    ''' Por cada fecha devuelve:<br/>
    ''' - El detalle de fichajes<br/>
    ''' - Los minutos de presencia<br/>
    ''' - Los fichajes emparejados
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha inicio del periodo.</param>
    ''' <param name="xEndDate">Fecha fin del periodo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Date, Moves, PresenceMinutes, MovesPairs </returns>
    ''' <remarks></remarks>

    Public Function GetPresenceDetail(ByVal intIDEmployee As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetPresenceDetail
        Return EmployeeMethods.GetPresenceDetail(intIDEmployee, xBeginDate, xEndDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene los saldos diarios para un empleado y periodo.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha inicio del periodo.</param>
    ''' <param name="xEndDate">Fecha fin del periodo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Total, TotalHM, DailyCauses.Date, Causes.ShortName , Causes.Name </returns>
    ''' <remarks></remarks>

    Public Function GetTotals(ByVal intIDEmployee As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetTotals
        Return EmployeeMethods.GetTotals(intIDEmployee, xBeginDate, xEndDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene el estado de presencia de un empleado en una fecha/hora indicada.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado</param>
    ''' <param name="_InputDateTime">Fecha y hora en la que se obtiene el estado</param>
    ''' <param name="_LastMoveType">Devuelve el tipo del último movimiento de presencia del empleado</param>
    ''' <param name="_LastMoveDateTime">Devuelve la fecha y hora del último movimiento del empleado</param>
    ''' <param name="_LastMove">Devuelve el último movimiento de presencia del empleado</param>
    ''' <param name="oState">Información adicional de estado.</param>        
    ''' <returns>Estado de presencia actual del empleado: - Inside: presente, - Outside: ausente</returns>
    ''' <remarks></remarks>

    Public Function GetPresenceStatus(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _LastMoveType As MovementStatus, ByVal _LastMoveDateTime As DateTime, ByVal _LastMove As Move.roMove, ByVal _PresenceMinutes As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMovePresenceStatus) Implements IEmployeeSvc.GetPresenceStatus
        Return EmployeeMethods.GetPresenceStatus(_IDEmployee, _InputDateTime, _LastMoveType, _LastMoveDateTime, _LastMove, _PresenceMinutes, oState)
    End Function

    ''' <summary>
    ''' Obtiene el estado de presencia de un empleado en una fecha/hora indicada.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado</param>
    ''' <param name="_InputDateTime">Fecha y hora en la que se obtiene el estado</param>
    ''' <param name="_LastPunchType">Devuelve el tipo del último movimiento de presencia del empleado</param>
    ''' <param name="_LastPunchDateTime">Devuelve la fecha y hora del último movimiento del empleado</param>
    ''' <param name="_LastPunch">Devuelve el último fichaje de presencia del empleado</param>
    ''' <param name="oState">Información adicional de estado.</param>        
    ''' <returns>Estado de presencia actual del empleado: - Inside: presente, - Outside: ausente</returns>
    ''' <remarks></remarks>

    Public Function GetPresenceStatusEx(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _LastPunchType As PunchStatus, ByVal _LastPunchDateTime As DateTime, ByVal _LastPunch As Punch.roPunch, ByVal _PresenceMinutes As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roPunchPresenceStatus) Implements IEmployeeSvc.GetPresenceStatusEx
        Return EmployeeMethods.GetPresenceStatusEx(_IDEmployee, _InputDateTime, _LastPunchType, _LastPunchDateTime, _LastPunch, _PresenceMinutes, oState)
    End Function

#End Region

#Region "Moves"

    ''' <summary>
    ''' Obtiene los fichajes de un empleado por una fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="bolIncludeCaptures">Para indicar si se incluyen la imágenes de los fichajes si existen.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>
    ''' Tabla con las columnas: IDEmployee, InDateTime, OutDateTime, InIDReader, OutIDReader, InIDCause, OutIDCause, ShiftDate, ID, InIDZone, OutIDZone, InIDReaderType, OutIDReaderType, IsNotReliableIN, IsNotReliableOUT, 
    ''' Si se incluyen la imágenes también se muestran las columnas: MovesCaptures.InCapture, MovesCaptures.OutCaptures
    ''' Si no se incluyen las imágenes también se muestran las columnas: IsInCapture, IsOutCapture
    ''' </returns>
    ''' <remarks></remarks>

    Public Function GetMoves(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetMoves
        Return EmployeeMethods.GetMoves(intIDEmployee, xDate, bolIncludeCaptures, oState)
    End Function

    ''' <summary>
    ''' Obtiene los fichajes de presencia de un empleado por una fecha, incluyendo las imagenes en caso necesario.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="bolIncludeCaptures">Para indicar si se incluyen la imágenes de los fichajes si existen.</param>
    ''' <remarks></remarks>

    Public Function GetPunchesPres(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetPunchesPres
        Return EmployeeMethods.GetPunchesPres(intIDEmployee, xDate, bolIncludeCaptures, oState, strFilter)
    End Function


    ''' <summary>
    ''' Obtiene los fichajes de presencia de un empleado por una fecha, incluyendo las imagenes en caso necesario.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="xEndDate">Fecha final.</param>
    ''' <param name="bolIncludeCaptures">Para indicar si se incluyen la imágenes de los fichajes si existen.</param>
    ''' <remarks></remarks>

    Public Function GetPunchesPresPeriod(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal xEndDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetPunchesPresPeriod
        Return EmployeeMethods.GetPunchesPresPeriod(intIDEmployee, xDate, xEndDate, bolIncludeCaptures, oState, strFilter)
    End Function

    ''' <summary>
    ''' Obtiene las fechas en las que existen movimientos incompletos en un periodo para un grupo y/o lista de empleados. Si se informa grupo, considera todos los subgrupos.
    ''' </summary>
    ''' <param name="xBegin">Fecha inicio del periodo.</param>
    ''' <param name="xEnd">Fecha fin del periodo.</param>
    ''' <param name="intIDGroup">Código del grupo. Si se informa 0 no filtra por grupo.</param>
    ''' <param name="strIDEmployees">Lista de códigos de empleado separados por comas (ej: 1,4,23). Si se deja en blanco no filtra por empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.Name, IDEmployee, Date, GroupName</returns>
    ''' <remarks></remarks>

    Public Function GetIncompleteMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetIncompleteMoves
        Return EmployeeMethods.GetIncompleteMoves(xBegin, xEnd, intIDGroup, strIDEmployees, oState)
    End Function

    ''' <summary>
    ''' Obtiene las fechas en las que existen dias con fichajes impares en un periodo para un grupo y/o lista de empleados. Si se informa grupo, considera todos los subgrupos.
    ''' </summary>
    ''' <param name="xBegin">Fecha inicio del periodo.</param>
    ''' <param name="xEnd">Fecha fin del periodo.</param>
    ''' <param name="intIDGroup">Código del grupo. Si se informa 0 no filtra por grupo.</param>
    ''' <param name="strIDEmployees">Lista de códigos de empleado separados por comas (ej: 1,4,23). Si se deja en blanco no filtra por empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.Name, IDEmployee, Date, GroupName</returns>
    ''' <remarks></remarks>

    Public Function GetIncompleteDays(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetIncompleteDays
        Return EmployeeMethods.GetIncompleteDays(xBegin, xEnd, intIDGroup, strIDEmployees, oState)
    End Function

    ''' <summary>
    ''' Obtiene las fechas en las que existen movimientos no fiables para un grupo y/o lista de empleados. Si se informa grupo, considera todos los subgrupos.
    ''' </summary>
    ''' <param name="xBegin">Fecha inicio del periodo.</param>
    ''' <param name="xEnd">Fecha fin del periodo.</param>
    ''' <param name="intIDGroup">Código del grupo. Si se informa 0 no filtra por grupo.</param>
    ''' <param name="strIDEmployees">Lista de códigos de empleado separados por comas (ej: 1,4,23). Si se deja en blanco no filtra por empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.Name, IDEmployee, Date, GroupName</returns>
    ''' <remarks></remarks>

    Public Function GetSuspiciousMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetSuspiciousMoves
        Return EmployeeMethods.GetSuspiciousMoves(xBegin, xEnd, intIDGroup, strIDEmployees, oState)
    End Function

    ''' <summary>
    ''' Obtiene las fechas en las que existen fichajes no fiables para un grupo y/o lista de empleados. Si se informa grupo, considera todos los subgrupos.
    ''' </summary>
    ''' <param name="xBegin">Fecha inicio del periodo.</param>
    ''' <param name="xEnd">Fecha fin del periodo.</param>
    ''' <param name="intIDGroup">Código del grupo. Si se informa 0 no filtra por grupo.</param>
    ''' <param name="strIDEmployees">Lista de códigos de empleado separados por comas (ej: 1,4,23). Si se deja en blanco no filtra por empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con columnas: Employees.Name, IDEmployee, Date, GroupName</returns>
    ''' <remarks></remarks>

    Public Function GetSuspiciousPunches(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetSuspiciousPunches
        Return EmployeeMethods.GetSuspiciousPunches(xBegin, xEnd, intIDGroup, strIDEmployees, oState)
    End Function

    ''' <summary>
    ''' Obtiene el último movimiento realizado por un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roMove' con la información del último movimiento del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetLastMove(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Move.roMove) Implements IEmployeeSvc.GetLastMove
        Return EmployeeMethods.GetLastMove(intIDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene el último fichaje de presencia realizado por un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roPunch' con la información del último movimiento del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetLastPunchPres(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Punch.roPunch) Implements IEmployeeSvc.GetLastPunchPres
        Return EmployeeMethods.GetLastPunchPres(intIDEmployee, oState)
    End Function

#End Region

#Region "Assignments"
    ''' <summary>
    ''' Obtiene los puestos que tiene asignado el empleado y su idoneidad.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment, Suitability </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeAssignments(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roEmployeeAssignment)) Implements IEmployeeSvc.GetEmployeeAssignments
        Return EmployeeMethods.GetEmployeeAssignments(intIDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene los puestos que tiene asignado el empleado y su idoneidad en un dataset.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment, Suitability </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeAssignmentsDatatable(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetEmployeeAssignmentsDatatable
        Return EmployeeMethods.GetEmployeeAssignmentsDatatable(intIDEmployee, oState)
    End Function
    ''' <summary>
    ''' Guardamos los puestos asignados al empleado
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="lstEmployeeAssignments">Lista de puestos asignados .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment, Suitability </returns>
    ''' <remarks></remarks>

    Public Function SaveEmployeeAssignments(ByVal intIDEmployee As Integer, ByVal lstEmployeeAssignments As Generic.List(Of roEmployeeAssignment), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveEmployeeAssignments
        Return EmployeeMethods.SaveEmployeeAssignments(intIDEmployee, lstEmployeeAssignments, oState)
    End Function



#End Region

#Region "Incidences"

    ''' <summary>
    ''' Obtiene los valores de las incidencias generadas de un empleado y fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, IncidenceValue, DailyCauses.Value, DailyCauses.Manual, DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, BeginTimeOrder </returns>
    ''' <remarks></remarks>

    Public Function GetIncidences(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetIncidences
        Return EmployeeMethods.GetIncidences(intIDEmployee, xDate, oState)
    End Function


    ''' <summary>
    ''' Obtiene los valores de las incidencias generadas de empleados , fechas y tipos de incidencia.
    ''' </summary>
    ''' <param name="intIDEmployees">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, IncidenceValue, DailyCauses.Value, DailyCauses.Manual, DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, BeginTimeOrder </returns>
    ''' <remarks></remarks>

    Public Function GetMassIncidences(ByVal intIDEmployees As String, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strIncidencesType As String, ByVal OnlyNotJustified As Boolean, ByVal strCenters As String, ByVal strCausesFilter As String, ByVal strCausesValueFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetMassIncidences
        Return EmployeeMethods.GetMassIncidences(intIDEmployees, xBeginDate, xEndDate, strIncidencesType, OnlyNotJustified, strCenters, strCausesFilter, strCausesValueFilter, oState)
    End Function

    ''' <summary>
    ''' Obtiene los valores de las incidencias generadas de empleados , fechas y tipos de incidencia.
    ''' </summary>
    ''' <param name="intIDEmployees">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, IncidenceValue, DailyCauses.Value, DailyCauses.Manual, DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, BeginTimeOrder </returns>
    ''' <remarks></remarks>

    Public Function GetMassCenters(ByVal intIDEmployees As String, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strCausesIDs As String, ByVal oState As roWsState, strBusinessCenters As String) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetMassCenters
        Return EmployeeMethods.GetMassCenters(intIDEmployees, xBeginDate, xEndDate, strCausesIDs, oState, strBusinessCenters)
    End Function



    ''' <summary>
    ''' Obtiene los valores de las incidencias no justificadas de un periodo, indicando un código de grupo y/o una lista de empleados.
    ''' </summary>
    ''' <param name="xBegin">Fecha inicio periodo.</param>
    ''' <param name="xEnd">Fecha fin periodo.</param>
    ''' <param name="intIDGroup">Código del grupo. Si se informa 0 no filtra por grupo.</param>
    ''' <param name="strIDEmployees">Lista de códigos de empleado separados por comas (ej: 1,4,23). Si se deja en blanco no filtra por empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Employees.Name, DailyCauses.IDEmployee, DailyCauses.Date, GroupName </returns>
    ''' <remarks></remarks>

    Public Function GetIncompleteIncidences(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetIncompleteIncidences
        Return EmployeeMethods.GetIncompleteIncidences(xBegin, xEnd, intIDGroup, strIDEmployees, oState)
    End Function

    ''' <summary>
    ''' Actualiza las justificaciones para un empleado y fecha.<br/>
    ''' Notifica al servidor el cambio para que se recalcule la información necesaria (DailySchedule.Status=65).
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha en la que se actualizarán las justificaciones.</param>
    ''' <param name="ds">DataSet con una tabla con columnas: IDEmployee, Date, IDRelatedIncidence, Value, Manual, CauseUser, CauseUserType, IsNotReliable </param>
    ''' <param name="bolUpdateStatus">Para indicar si se quiere notificar el cambio al servidor para que recalcule los datos necesarios.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveDailyCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal ds As DataSet, ByVal bolUpdateStatus As Boolean, ByVal oState As roWsState,
                                        ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveDailyCauses

        Return EmployeeMethods.SaveDailyCauses(intIDEmployee, xDate, ds, bolUpdateStatus, oState, bAudit)
    End Function

#End Region

#Region "Accruals"

    ''' <summary>
    ''' Obtiene los saldos diarios de un empleado para una fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha de la que se recuperarán los saldos.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: DailyAccruals.IDConcept, Concepts.Name, DailyAccruals.Value, Concepts.IDType, DailyAccruals.CarryOver, ValueFormat (valor formateado según el tipo del saldo: H o O). </returns>
    ''' <remarks></remarks>

    Public Function GetDailyAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetDailyAccruals
        Return EmployeeMethods.GetDailyAccruals(intIDEmployee, xDate, filterBusinessGroups, oState)
    End Function
    ''' <summary>
    ''' Obtiene los saldos diarios de tareas de un empleado para una fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha de la que se recuperarán los saldos.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: DailyTaskAccruals.IDTask, Tasks.Name, DailyTaskAccruals.Value, ValueFormat. </returns>
    ''' <remarks></remarks>

    Public Function GetDailyTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetDailyTaskAccruals
        Return EmployeeMethods.GetDailyTaskAccruals(intIDEmployee, xDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene el valor de los saldos anuales de un empleado para un fecha.<br/>
    ''' Utiliza el mes definido como inicio de periodo anual para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDConcept, Concepts.Name, Total, Concepts.IDType, TotalFormat (valor formateado según el tipo del saldo: H o O), Concepts.DefaultQuery (tipo de saldo, Y:anual, M:Mensual) </returns>
    ''' <remarks></remarks>

    Public Function GetAnualAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetAnualAccruals
        Return EmployeeMethods.GetAnualAccruals(intIDEmployee, xDate, filterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Obtiene el valor de los saldos totales de tareas de un empleado para un fecha.<br/>
    ''' Utiliza el mes definido como inicio de periodo anual para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDTask, Task.Name, Total,  TotalFormat  </returns>
    ''' <remarks></remarks>

    Public Function GetTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetTaskAccruals
        Return EmployeeMethods.GetTaskAccruals(intIDEmployee, xDate, oState)
    End Function

    ''' <summary>
    ''' Obtiene el valor de los saldos mensuales de un empleado para una fecha.<br/>
    ''' Utiliza el día definido como inicio de periodo mensual para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDConcept, Concepts.Name, Total, TotalFormat (valor formateado según el tipo del saldo: H o O), Concepts.DefaultQuery (tipo de saldo, Y:anual, M:Mensual) </returns>
    ''' <remarks></remarks>

    Public Function GetMonthAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetMonthAccruals
        Return EmployeeMethods.GetMonthAccruals(intIDEmployee, xDate, oState, filterBusinessGroups)
    End Function


    ''' <summary>
    ''' Obtiene el valor de los saldos semanales de un empleado para una fecha.<br/>
    ''' Utiliza el día definido como inicio de semana para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDConcept, Concepts.Name, Total, TotalFormat (valor formateado según el tipo del saldo: H o O), Concepts.DefaultQuery (tipo de saldo, Y:anual, M:Mensual, W:Semanal) </returns>
    ''' <remarks></remarks>

    Public Function GetWeekAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetWeekAccruals
        Return EmployeeMethods.GetWeekAccruals(intIDEmployee, xDate, oState, filterBusinessGroups)
    End Function

    ''' <summary>
    ''' Obtiene el valor de los saldos por contrato de un empleado para una fecha.<br/>
    ''' Utiliza el día definido como inicio de semana para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDConcept, Concepts.Name, Total, TotalFormat (valor formateado según el tipo del saldo: H o O), Concepts.DefaultQuery (tipo de saldo, Y:anual, M:Mensual, W:Semanal) </returns>
    ''' <remarks></remarks>

    Public Function GetContractAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetContractAccruals
        Return EmployeeMethods.GetContractAccruals(intIDEmployee, xDate, oState, filterBusinessGroups)
    End Function


    ''' <summary>
    ''' Obtiene el valor de los saldos de tareas mensuales de un empleado para una fecha.<br/>
    ''' Utiliza el día definido como inicio de periodo mensual para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores de tareas.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDTask, Task.Name, Total, TotalFormat  </returns>
    ''' <remarks></remarks>

    Public Function GetMonthTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetMonthTaskAccruals
        Return EmployeeMethods.GetMonthTaskAccruals(intIDEmployee, xDate, oState)
    End Function


    ''' <summary>
    ''' Obtiene los saldos mensuales y anuales de un empleado y una fecha.
    ''' </summary>
    ''' <param name="_IDEmployee">Código empleado</param>
    ''' <param name="_CurrentDate">Fecha actual</param>
    ''' <param name="_OnlyViewInTerminals">Mostrar sólo los saldos que se tienen que mostrar en los terminales</param>
    ''' <param name="oState"></param>        
    ''' <returns>Tabla con las columnas: IDConcept, Name, Total, IDType, TotalFormat (valor total del saldo formateado en función del tipo de saldo), DefaultQuery (tipo de saldo, Y:anual, M:Mensual)</returns>
    ''' <remarks></remarks>

    Public Function GetAccrualsQuery(ByVal _IDEmployee As Integer, ByVal _CurrentDate As Date, ByVal _OnlyViewInTerminals As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetAccrualsQuery
        Return EmployeeMethods.GetAccrualsQuery(_IDEmployee, _CurrentDate, _OnlyViewInTerminals, oState)
    End Function

    ''' <summary>
    ''' Copia las reglas de saldos de un empleado a otro. Opcionalmente se puede informar si substituir las reglas actuales o no.
    ''' </summary>
    ''' <param name="intIDSourceEmployee">Código del empleado origen.</param>
    ''' <param name="intIDDestinationEmployee">Código del empleado destino.</param>
    ''' <param name="bolDescartarReglasActuales">Indica si substituir las reglas actuales del empleado destino.</param>
    ''' <param name="oState">Información adicional de estado.</param>        
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyAccrualRules(ByVal intIDSourceEmployee As Integer, ByVal intIDDestinationEmployee As Integer, ByVal bolDescartarReglasActuales As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.CopyAccrualRules
        Return EmployeeMethods.CopyAccrualRules(intIDSourceEmployee, intIDDestinationEmployee, bolDescartarReglasActuales, oState)
    End Function

    ''' <summary>
    ''' Copia los valores de los límites anuales de un empleado a otro. Opcionalmente se puede informar si substituir los valores coincidentes o no.
    ''' </summary>
    ''' <param name="intIDEmployeeSource">Código del empleado origen.</param>
    ''' <param name="intIDEmployeeDestination">Código del empleado destino.</param>
    ''' <param name="bolDiscardAnnualLimits">Para indicar si se tiene que sustituir los límites coincidentes. True: indica que se tienen que sustituir.</param>
    ''' <param name="oState">Información adicional de estado.</param>        
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function CopyConceptAnnualLimits(ByVal intIDEmployeeSource As Integer, ByVal intIDEmployeeDestination As Integer, ByVal bolDiscardAnnualLimits As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.CopyConceptAnnualLimits
        Return EmployeeMethods.CopyConceptAnnualLimits(intIDEmployeeSource, intIDEmployeeDestination, bolDiscardAnnualLimits, oState)
    End Function

#End Region

#Region "Biometric Data"

    ''' <summary>
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteBiometricData(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.DeleteBiometricData
        Return EmployeeMethods.DeleteBiometricData(IDEmployee, oState)
    End Function


    ''' <summary>
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteEmployeeRelatedData(ByVal IDEmployee As Integer, ByVal bRemoveEmployeePhoto As Boolean, ByVal bRemoveBiometricData As Boolean, ByVal bRemoveEmployeePunchImages As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.DeleteEmployeeRelatedData
        Return EmployeeMethods.DeleteEmployeeRelatedData(IDEmployee, bRemoveEmployeePhoto, bRemoveBiometricData, bRemoveEmployeePunchImages, oState)
    End Function

#End Region

#Region "Querys"

    ''' <summary>
    ''' Resumen de vacaciones
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xCurrentDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="xVacationsDate">Fecha del día de vacaciones solicitada</param>
    ''' <param name="intDone">Devuelve núm. de días ya disfrutados</param>
    ''' <param name="intPending">Devuelve número de días solicitados pendientes de procesa</param>
    ''' <param name="intLasting">Devuelve el número de días aprobados pendientes de disfrutar</param>
    ''' <param name="intDisponible">Devuelve los días disponibles de vacaciones</param>
    ''' <param name="oState"></param>        
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>

    Public Function VacationsResumeQuery(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                             ByVal xVacationsDate As Date, ByVal intDone As Integer, ByVal intPending As Integer, ByVal intLasting As Integer, ByVal intDisponible As Double,
                                             ByVal oState As roWsState) As roGenericVtResponse(Of roHolidayResumeInfo) Implements IEmployeeSvc.VacationsResumeQuery
        Return EmployeeMethods.VacationsResumeQuery(IDEmployee, IDShift, xCurrentDate, xBeginPeriod, xEndPeriod, xVacationsDate, intDone, intPending, intLasting, intDisponible, oState)
    End Function


    ''' <summary>
    ''' Resumen de vacaciones/permisos por horas
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xCurrentDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="xProgrammedHolidaysDate">Fecha del día de vacaciones solicitada</param>
    ''' <param name="intPending">Devuelve número de días solicitados pendientes de procesa</param>
    ''' <param name="intLasting">Devuelve el número de días aprobados pendientes de disfrutar</param>
    ''' <param name="intDisponible">Devuelve los días disponibles de vacaciones</param>
    ''' <param name="oState"></param>        
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>

    Public Function ProgrammedHolidaysResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                             ByVal xProgrammedHolidaysDate As Date, ByVal intPending As Double, ByVal intLasting As Double, ByVal intDisponible As Double,
                                             ByVal oState As roWsState) As roGenericVtResponse(Of roHolidayResumeInfo) Implements IEmployeeSvc.ProgrammedHolidaysResumeQuery
        Return EmployeeMethods.ProgrammedHolidaysResumeQuery(IDEmployee, IDCause, xCurrentDate, xBeginPeriod, xEndPeriod, xProgrammedHolidaysDate, intPending, intLasting, intDisponible, oState)
    End Function




    ''' <summary>
    ''' Dias laborables del periodo indicado
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="oState"></param>        
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>

    Public Function LaboralDaysInPeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IEmployeeSvc.LaboralDaysInPeriod
        Return EmployeeMethods.LaboralDaysInPeriod(IDEmployee, xBeginPeriod, xEndPeriod, oState)
    End Function



#End Region

#Region "Employee Status"

    ''' <summary>
    ''' Devuelve la información del estado actual de un empleado.
    ''' </summary>
    ''' <param name="IdEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployeeStatus' con la información de estado del empleado.</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeStatus(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeStatus) Implements IEmployeeSvc.GetEmployeeStatus
        Return EmployeeMethods.GetEmployeeStatus(IDEmployee, oState, bAudit)
    End Function

#End Region


#Region "Forecasts"

    Public Function GetEmployeeForecastsInPeriod(ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetEmployeeForecastsInPeriod
        Return EmployeeMethods.GetEmployeeForecastsInPeriod(IDEmployee, _BeginDate, oState)
    End Function
#End Region

#Region "Coverages"

    ''' <summary>
    ''' Devuelve la lista de posibles empleados que pueden realizar una cobertura del empleado para una fecha en concreto.
    ''' </summary>
    ''' <param name="_IDEmployee"></param>
    ''' <param name="_CoverageDate"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetCoverageEmployees(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetCoverageEmployees
        Return EmployeeMethods.GetCoverageEmployees(_IDEmployee, _CoverageDate, oState)
    End Function

    ''' <summary>
    ''' Define una cobertura para un empleado y fecha en concreto.
    ''' </summary>
    ''' <param name="_IDEmployeeCoverage">Código del empleado que realizará la cobertura.</param>
    ''' <param name="_IDEmployeeCovered">Código del empleado a cubrir.</param>
    ''' <param name="_CoverageDate">Fecha de la cobertura</param>
    ''' <param name="_IDShift">Código del horario a utilizar para el empleado que realizará la cobertura. Si se quiere utilizar el que tiene actualmente planificado, indicar -1.</param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function AddEmployeeCoverage(ByVal _IDEmployeeCoverage As Integer, ByVal _IDEmployeeCovered As Integer, ByVal _CoverageDate As Date, ByVal _IDShift As Integer, ByVal _LockedDayActionEmployeeCovered As LockedDayAction, ByVal _LockedDayActionEmployeeCoverage As LockedDayAction, ByVal _IDEmployeeLocked As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.AddEmployeeCoverage
        Return EmployeeMethods.AddEmployeeCoverage(_IDEmployeeCoverage, _IDEmployeeCovered, _CoverageDate, _IDShift, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _IDEmployeeLocked, _Audit, oState)
    End Function

    ''' <summary>
    ''' Elimina una cobertura para un empleado y fecha.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado al que se le quiere eliminar la cobertura (puede ser tanto el empleado que realiza la cobertura cómo el empleado que se está cubriendo).</param>
    ''' <param name="_CoverageDate">Fecha de la cobertura</param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function RemoveEmployeeCoverage(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal _LockedDayActionEmployeeCovered As LockedDayAction, ByVal _LockedDayActionEmployeeCoverage As LockedDayAction, ByVal _EmployeeType As Integer, ByVal _IDEmployeeLocked As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.RemoveEmployeeCoverage
        Return EmployeeMethods.RemoveEmployeeCoverage(_IDEmployee, _CoverageDate, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _EmployeeType, _IDEmployeeLocked, _Audit, oState)
    End Function

    ''' <summary>
    ''' Devuelve la lista de posibles empleados que pueden realizar un puesto para poder planificar una dotación para un grupo, puesto y fecha concretos.
    ''' </summary>
    ''' <param name="_IDGroup"></param>
    ''' <param name="_IDAssignment"></param>
    ''' <param name="_CoverageDate"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetDailyCoverageEmployees(ByVal _IDGroup As Integer, ByVal _IDAssignment As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEmployeeSvc.GetDailyCoverageEmployees
        Return EmployeeMethods.GetDailyCoverageEmployees(_IDGroup, _IDAssignment, _CoverageDate, oState)
    End Function

#End Region


#Region "Authorizations"

    Public Function AddAccessGroupToEmployee(ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, ByVal bolAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.AddAccessGroupToEmployee
        Return EmployeeMethods.AddAccessGroupToEmployee(_IDEmployee, _IDAuthorization, bolAudit, oState)
    End Function


    Public Function RemoveAccessGroupToEmployee(ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, ByVal bolAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.RemoveAccessGroupToEmployee
        Return EmployeeMethods.RemoveAccessGroupToEmployee(_IDEmployee, _IDAuthorization, bolAudit, oState)
    End Function


    Public Function SaveAccessAuthorizations(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal dsAuthorizations As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IEmployeeSvc.SaveAccessAuthorizations
        Return EmployeeMethods.SaveAccessAuthorizations(IDEmployee, IDGroup, dsAuthorizations, oState)
    End Function
#End Region

#Region "Resume"

#End Region

End Class
