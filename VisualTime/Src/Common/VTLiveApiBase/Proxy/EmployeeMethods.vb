Imports System.Data
Imports Robotics.Azure
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTBusiness.Job
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Security
Imports Robotics.VTBase
Imports ServiceApi

Public Class EmployeeMethods

#Region "Employee"

    ''' <summary>
    ''' Devuelve un dataset con los datos de los empleados de la lista pasada por parámetro<br/>
    ''' Si la lista pasada no contiene ninguna coma se considera que se quiere hacer un where por un valor.
    ''' </summary>
    ''' <param name="List">Lista de códigos de empleado separados por comas (ej: 1,4,5,14,23) o cláusula where.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Employees.*, sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>
    Public Shared Function GetEmployeeFromList(ByVal List As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roBusinessSupport.GetEmployeeFromList(List, bState, "", "", "")

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la información de un empleado.
    ''' </summary>
    ''' <param name="IdEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployee(ByVal IdEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployee)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployee)
        oResult.Value = roEmployee.GetEmployee(IdEmployee, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeName(ByVal IdEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = roBusinessSupport.GetEmployeeName(IdEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el código de contrato indicado.
    ''' </summary>
    ''' <param name="IdContract">Código de contrato.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeeByContract(ByVal IdContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployee)
        oResult.Value = roEmployee.GetEmployeeByContract(IdContract, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el código del empleado con el contrato indicado.
    ''' </summary>
    ''' <param name="IdContract">Código de contrato del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Código del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetIdEmployeeByContract(ByVal IdContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roEmployee.GetIdEmployeeByContract(IdContract, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el nombre indicado.
    ''' </summary>
    ''' <param name="strName">Nombre del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeeByName(ByVal strName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployee)
        oResult.Value = roEmployee.GetEmployeeByName(strName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetEmployeeSelectionPath(ByVal idEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeSelectionPath)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeSelectionPath)
        oResult.Value = roEmployee.GetEmployeeSelectionPath(idEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve el código del empleado con el nombre indicado.
    ''' </summary>
    ''' <param name="strName">Nombre del empleado a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Código interno del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetIdEmployeeByName(ByVal strName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roEmployee.GetIdEmployeeByName(strName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateEmployee(ByVal Employee As roEmployee, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.ValidateEmployee(Employee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveEmployee(ByVal Employee As roEmployee, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployee)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployee)
        If roEmployee.SaveEmployee(Employee, bState) Then
            oResult.Value = Employee
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borra los datos del empleado indicado. Verifica que el empleado no tenga fichajes de producción ni saldos de producción.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False.</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteEmployee(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.DeleteEmployee(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba si un empleado tiene datos en visualtime para decidir si se permite borrar o no.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False.</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function CheckIfEmployeeHasData(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roBusinessSupport.CheckIfEmployeeHasData(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el número de empleados activos a una fecha, utilizando la definición de los contratos.
    ''' </summary>
    ''' <param name="xDate">Fecha a la que se quiere obtener el número de empleados activos.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados activos a una fecha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetActiveEmployeesCount(ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roBusinessSupport.GetActiveEmployeesCount(xDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el número de empleados de tareas activos a una fecha, utilizando la definición de los contratos.
    ''' </summary>
    ''' <param name="xDate">Fecha a la que se quiere obtener el número de empleados activos tipo J.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados activos a una fecha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetActiveEmployeesTaskCount(ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roBusinessSupport.GetActiveEmployeesTaskCount(xDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de empleados en una tabla. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, EmployeeContracts.IDContract, EmployeeContracts.IDCard</returns>
    ''' <remarks></remarks>

    Public Shared Function GetAllEmployees(ByVal strWhere As String, ByVal strFeature As String, ByVal strFeatureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetAllEmployees(strWhere, strFeature, strFeatureType, bState)
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

    Public Shared Function GetEmployees(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployees(strWhere, "", "", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por nombre. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre del empleado.</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where.</param>
    ''' <param name="strFeature">Feature para comprobar seguridad de empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesByName(ByVal strLikeName As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByName(strLikeName, strWhere, strFeature, "U", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por nombre. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strAdvFilter">Filtro avanzado que se aplica para buscar empleados.</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where.</param>
    ''' <param name="strFeature">Feature para comprobar seguridad de empleados.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.* </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesByAdvancedFilter(ByVal strAdvFilter As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByAdvancedFilter(strAdvFilter, strWhere, strFeature, "U", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por código de contrato. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeIDContract">Filtro que se aplica al código de contrato</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, EmployeeContracts.IDContract </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesByIDContract(ByVal strLikeIDContract As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByIDContract(strLikeIDContract, strWhere, strFeature, "U", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por número de tarjeta. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al número de tarjeta</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS IDCard</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesByIDCard(ByVal strLikeName As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByIDCard(strLikeName, strWhere, strFeature, "U", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que cumplen el filtro por matrícula. Opcionalmente se aplica el filtro indicado.
    ''' </summary>
    ''' <param name="strLikePlate">Filtro que se aplica a la matrícula</param>
    ''' <param name="strWhere">Filtro a utilizar cómo cláusula where. Opcional.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS Plate</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesByPlate(ByVal strLikePlate As String, ByVal strWhere As String, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByPlate(strLikePlate, strWhere, strFeature, "U", bState)
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

    ''' <summary>
    ''' Obtiene la lista de empleados que tiene el id.
    ''' </summary>
    ''' <param name="IdEmployee">Id del empleado a buscar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Tabla con las columnas: sysrovwAllEmployeeGroups.*</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesById(ByVal IdEmployee As Integer, ByVal strFeature As String, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesByID(IdEmployee, strFeature, "U", bState)
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

    ''' <summary>
    ''' Genera múltiples empleados.
    ''' </summary>
    ''' <param name="EmployeesData">Información de los empleados: nombre (EmployeeName - String), código tarjeta (IDCard - Long), código contrato (IDContract - String), fecha inicio (BeginDate - DateTime), tipo empleado ('A', 'J') (EmployeeType - String), código grupo (IDGroup - Integer), ficha con trajeta (CardMethod - Boolean), ficha con huella (BiometricMethod - Boolean), combinación métodos (MergeMethod - Integer)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="lstEmployeeNameError">Devuelve la lista de nombre de empleados que no se han podido guardar.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function CreateMultiEmployees(ByVal EmployeesData As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String))
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of String))
        Dim lstErrors As New Generic.List(Of String)
        roEmployee.CreateMultiEmployees(EmployeesData.Tables(0), bState, lstErrors)
        oResult.Value = lstErrors

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un array de bytes del fichero especificado como paramentro.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns>Array de Bytes o nothing si hay algun error</returns>
    ''' <remarks></remarks>

    Public Shared Function GetDocumentToView(ByVal strFileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Byte())
        oResult.Value = roScheduler.GetDocumentToView(strFileName, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la información del empleado con el código de contrato indicado.
    ''' </summary>
    ''' <param name="IdEmployee">Id del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roEmployee' con la información del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeeSummary(ByVal IdEmployee As Integer, ByVal onDate As Date, ByVal accrualSummaryType As SummaryType, ByVal causesSummaryType As SummaryType, ByVal tasksSummaryType As SummaryType, ByVal centersSummaryType As SummaryType,
                                       ByVal requestType As SummaryRequestType, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeSummary)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeSummary)
        oResult.Value = roEmployeeSummaryManager.GetEmployeeSummary(IdEmployee, onDate, accrualSummaryType, causesSummaryType, tasksSummaryType, centersSummaryType, requestType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetCurrentGroupName(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = roBusinessSupport.GetCurrentGroupName(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetCurrentFullGroupName(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = roBusinessSupport.GetCurrentFullGroupName(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la información del grupo actual al que pertenece el empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roMobility' con la información del grupo actual al que pertenece el empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetCurrentMobility(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMobility)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roMobility)
        oResult.Value = roMobility.GetCurrentMobility(IDEmployee, bState)
        If oResult.Value Is Nothing Then
            oResult.Value = roMobility.GetNextMobility(IDEmployee, bState)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la información de la mobilidad de un empleado para un grupo.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="IdGroup">Código grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roMobility' con la información de la mobilidad.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetMobility(ByVal IDEmployee As Integer, ByVal IdGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMobility)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roMobility)
        oResult.Value = roMobility.GetMobility(IDEmployee, IdGroup, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una tabla con la información de la mobilidad de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDGroup, Name, BeginDate, EndDate </returns>
    ''' <remarks></remarks>

    Public Shared Function GetMobilities(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roMobility.GetMobilities(IDEmployee, bState)
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

    ''' <summary>
    ''' Valida la información de la mobilidad para un empleado.
    ''' Verifica que el grupo indicado exista y que el perido de fechas de validez de la mobilidad sea correcto.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="Mobility">Object 'roMobility' con la información de la mobilidad.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Shared Function ValidateMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roMobility.ValidateMobility(IDEmployee, Mobility, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateMobilities(ByVal IDEmployee As Integer, ByVal dsMobilities As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of roMobilityValidation)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roMobilityValidation)
        Dim iInvalidRow As Integer = 0
        Dim bOk As Boolean = roMobility.ValidateMobilities(IDEmployee, dsMobilities, iInvalidRow, bState)
        oResult.Value = New roMobilityValidation() With {.InvalidRow = iInvalidRow, .Mobilities = dsMobilities, .Valid = bOk}

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Añade una mobilidad a la definición de mobilidades de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="Mobility">Objeto 'roMobility' con la información de la mobilidad a añadir.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveMobility(ByVal IDEmployee As Integer, ByVal Mobility As roMobility, ByVal oState As roWsState, ByVal CallBroadcaster As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roMobility.SaveMobility(IDEmployee, Mobility, bState, CallBroadcaster)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveMobilities(ByVal IDEmployee As Integer, ByVal dsMobilities As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roMobility.SaveMobilities(IDEmployee, dsMobilities, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function UpdateEmployeeGroup(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal FromDate As Date, ByVal pCopyPlan As Boolean, ByVal SourceIDEmployee As Integer,
                                            ByVal intShiftType As ActionShiftType, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction,
                                            ByVal _ShiftPermissionAction As ShiftPermissionAction, ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bolRet As Boolean = False

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim datBeginDate As Date
        ' Si nos pasan una fecha de inicio en los parámetros
        If FromDate.ToShortDateString = "01/01/0001" Then
            ' La fecha de final de pertenencia a un grupo es la fecha anterior al movimiento
            datBeginDate = Date.Today
        Else
            datBeginDate = FromDate
        End If

        Dim oContractState As New Contract.roContractState(oState.IDPassport)
        Dim oContract = Contract.roContract.GetContractInDate(IDEmployee, datBeginDate, oContractState, False)

        If oContract IsNot Nothing AndAlso FromDate < oContract.BeginDate Then
            FromDate = oContract.BeginDate
        End If

        If oContract IsNot Nothing Then
            bolRet = roMobility.UpdateEmployeeGroup(IDEmployee, IDGroup, FromDate, bState)
        End If

        If oState.Result = EmployeeResultEnum.NoError Then
            If pCopyPlan Then
                Dim oEndDate As roTime = roScheduler.GetLatestPlanDate(SourceIDEmployee, bState)
                If oState.Result = EmployeeResultEnum.NoError AndAlso oEndDate IsNot Nothing Then
                    Dim datEndDate As Date = oEndDate.Value
                    bolRet = roScheduler.CopyPlan(SourceIDEmployee, IDEmployee, datBeginDate, datEndDate, intShiftType,
                                                        _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, bState, copyHolidays, False)
                End If
            End If
        End If

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeLockDatetoApply(ByVal IDEmployee As Integer, ByVal EmployeeLockDateType As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeLockDateInfo)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeLockDateInfo)

        Dim oRes As New roEmployeeLockDateInfo
        oRes.EmployeeLockDate = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, EmployeeLockDateType, bState, bAudit)
        oRes.EmployeeLockDateType = EmployeeLockDateType

        oResult.Value = oRes

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SaveEmployeeLockDate(ByVal IDEmployee As Integer, ByVal LockDate As Date, ByVal EmployeeLockDateType As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roBusinessSupport.SaveEmployeeLockDate(IDEmployee, LockDate, EmployeeLockDateType, bState, bAudit, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SaveLockDate(ByVal xLockDate As Date, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roBusinessSupport.SaveLockDate(xLockDate, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetEmployeesOnLockDate(ByVal strEmployeeFilter As String, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeesOnLockDate(strEmployeeFilter, xDate, bState)
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

    Public Shared Function GetUserField(ByVal IDEmployee As Integer, ByVal UserFieldName As String, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeUserField)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeUserField)
        oResult.Value = New roEmployeeUserField(IDEmployee, UserFieldName, xDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de valores de la ficha del empleado a una fecha en concreto.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha en la que se quiere obtener los valores</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Una lista de objetos 'roEmployeeUserField' con la información del valor a una fecha de la ficha.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserFields(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roEmployeeUserField))

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roEmployeeUserField))
        oResult.Value = roEmployeeUserField.GetUserFieldsList(IDEmployee, xDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''<WebMethod()> _
    ''Public Shared Function GetUserFieldsList(ByVal oState As roWsState) As roGenericVtResponse(Of roUserFields
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

    Public Shared Function SaveUserField(ByVal IDEmployee As Integer, ByVal UserFieldName As String, ByVal UserFieldDate As Date, ByVal UserFieldValue As Object, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oEmployeeUserField As New roEmployeeUserField(IDEmployee, UserFieldName, UserFieldDate, bState, False)
        oEmployeeUserField.FieldValue = UserFieldValue

        oResult.Value = oEmployeeUserField.Save(, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oEmployeeUserField.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Guarda los valores de la ficha de un empleado.<br/>
    ''' Para cada valor de campo verifica que la fecha no este dentro del periodo de congelación.<br/>
    ''' Para cada campo modificado lanza el recálculo correspondiente en función de si el campo se utiliza en algún proceso.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado.</param>
    ''' <param name="_UserFields">Lista de objetos 'roEmployeeUserField' con la información de los valores de los campos.</param>
    ''' <param name="oState">Información adición de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveUserFields(ByVal _IDEmployee As Integer, ByVal _UserFields As Generic.List(Of roEmployeeUserField), ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployeeUserField.SaveUserFields(_IDEmployee, _UserFields, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene todos los valores de la ficha de un empleado en una fecha.<br/>
    ''' Tiene en cuenta la configuración de seguridad de la ficha del passport actual y el nivel de acceso de cada campo, devolviendo solo los que se tiene permiso.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha de los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: FieldCaption, FieldName, Type, Value, OriginalDate, Value, ValueDateTime, Category, AccessLevel, Description, AccessValidation, History</returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserFieldsDataset(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Data.DataSet)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEmployeeUserField.GetUserFieldsDataTable(IDEmployee, xDate, bState)
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
    ''' Obteien el histórico de valores de un campo de la ficha de un empleado.<br/>
    ''' Tiene en cuenta la configuración de seguridad de la ficha del passport actual y el nivel de acceso de cada campo, devolviendo solo los que se tiene permiso.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="FieldName">Nombre del campo de la ficha.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con los columnas: IDEmployee, FieldName, Date, Value, OriginalDate </returns>
    ''' <remarks></remarks>

    Public Shared Function GetUserFieldHistoryDataset(ByVal IDEmployee As Integer, ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEmployeeUserField.GetUserFieldHistoryDataTable(IDEmployee, FieldName, bState)
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
    ''' Guarda el histórico de valores de un campo de la ficha de un empleado.<br/>
    ''' Se recorren las filas de la tabla pasado y en función del estado de la fila se actualiza el histórico del campo.
    ''' </summary>
    ''' <param name="IDEmployee">Código del empleado.</param>
    ''' <param name="tbHistory">Tabla con la información del histórico de valores del campo de la ficha: DEmployee, FieldName, Date, Value, OriginalDate </param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveUserFieldHistory(ByVal IDEmployee As Integer, ByVal tbHistory As DataTable, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployeeUserField.SaveUserFieldHistory(IDEmployee, tbHistory, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DeleteUserFieldHistory(ByVal IDEmployee As Integer, ByVal _FieldName As String, ByVal _FromDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployeeUserField.DeleteUserFieldHistory(IDEmployee, _FieldName, _FromDate, True, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetEmployeeUserFieldValueAtDate(ByVal _IDEmployee As String, ByVal _FieldName As String, ByVal _Date As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeUserField)
        Dim bState = New roUserFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeUserField)
        oResult.Value = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(_IDEmployee, _FieldName, _Date, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetTerminalMessages(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of System.Data.DataSet)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roTerminalMessage.GetTerminalMessages(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene las definición de un mensaje de terminal de un empleado.
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="TerminalMessage">Mensaje terminal.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roTerminalMessage' con la información del mensaje.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTerminalMessage(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByVal oState As roWsState) As roGenericVtResponse(Of roTerminalMessage)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTerminalMessage)
        oResult.Value = roTerminalMessage.GetTerminalMessage(IDEmployee, TerminalMessage, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DeleteTerminalMessages(ByVal IDEmployee As Integer, ByVal TerminalMessage As String, ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roTerminalMessage.DeleteTerminalMessages(IDEmployee, TerminalMessage, ID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roTerminalMessage.ValidateTerminalMessage(IdEmployee, TerminalMessage, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveTerminalMessage(ByVal IdEmployee As Integer, ByVal TerminalMessage As roTerminalMessage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roTerminalMessage.SaveTerminalMessage(IdEmployee, TerminalMessage, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyPlan(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        Dim oAuxDate As roTime = roScheduler.GetLatestPlanDate(intSourceIDEmployee, bState)
        If oState.Result = EmployeeResultEnum.NoError Then
            If Not oAuxDate Is Nothing Then
                roScheduler.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xBeginPeriod, oAuxDate.Value, intShiftType, _LockedDayAction, _ShiftPermissionAction, _CoverageDayAction,
                                           xDateLocked, bState, copyHolidays, bAudit)
            End If
        End If

        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyShifts(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                   ByVal intShiftType As ActionShiftType,
                                   ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                   ByVal xDateLocked As Date, ByVal oState As roWsState, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xBeginPeriod, xEndPeriod, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction,
                                   xDateLocked, bState, copyHolidays, bAudit)

        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyShiftsPeriod(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBeginPeriod As Date, ByVal xSourceEndPeriod As Date,
                                         ByVal xDestinationBeginPeriod As Date, ByVal intShiftType As ActionShiftType,
                                         ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                         ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xSourceBeginPeriod, xSourceEndPeriod, xDestinationBeginPeriod, intShiftType,
                                   _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, bState)

        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyShiftsPeriodWithEnd(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBeginPeriod As Date,
                                                ByVal xSourceEndPeriod As Date, ByVal xDestinationBeginPeriod As Date, ByVal xDestinationEndPeriod As Date,
                                                ByVal intShiftType As ActionShiftType,
                                                ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xSourceBeginPeriod, xSourceEndPeriod, xDestinationBeginPeriod, intShiftType,
                                   _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, bState, xDestinationEndPeriod, bAudit)

        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyShiftsEmployees(ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date,
                                            ByVal intShiftType As ActionShiftType,
                                            ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                            ByVal intIDEmployeeLocked As Integer, ByVal oState As roWsState,
                                            ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction,
                                   intIDEmployeeLocked, copyHolidays, bState, , , bAudit)

        oEmployeeCopyPlanResult.IDEmployeeLocked = intIDEmployeeLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyShiftsEmployeesWithEnd(ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date,
                                                   ByVal xDestinationEndDate As Date, ByVal intShiftType As ActionShiftType,
                                                   ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                   ByVal intIDEmployeeLocked As Integer, ByVal xDateLocked As Date, ByVal oState As roWsState, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction,
                                   intIDEmployeeLocked, copyHolidays, bState, xDestinationEndDate, xDateLocked, bAudit)

        oEmployeeCopyPlanResult.IDEmployeeLocked = intIDEmployeeLocked
        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyListShifts(ByVal lstShifts As Generic.List(Of String), ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal _
                                       intShiftType As ActionShiftType,
                                       ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                       ByVal xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        roScheduler.CopyPlan(lstShifts, intDestinationIDEmployee, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, xDateLocked, copyHolidays, bState, bAudit, , _ShiftPermissionAction)

        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = (bState.Result = EmployeeResultEnum.NoError)
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyListShiftsEmployees(ByVal lstShifts As Generic.List(Of String), ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date,
                                                ByVal xEndDate As Date, ByVal intShiftType As ActionShiftType,
                                                ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                ByVal xDateLocked As Date, ByVal intIDEmployeeLocked As Integer, ByVal isHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        Dim bRes As Boolean = roScheduler.CopyPlan(lstShifts, lstDestionationIDEmployees, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction,
                                          xDateLocked, intIDEmployeeLocked, isHolidays, bState, bAudit)

        oEmployeeCopyPlanResult.IDEmployeeLocked = intIDEmployeeLocked
        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = bRes
        oResult.Value = oEmployeeCopyPlanResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function LockDaysList(ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roScheduler.LockDaysInPlan(intDestinationIDEmployee, xBeginDate, xEndDate, xLocked, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function LockDaysListEmployees(ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roScheduler.LockDaysInPlan(lstDestionationIDEmployees, xBeginDate, xEndDate, xLocked, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetPlan(ByVal intIDEmployee As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roScheduler.GetPlan(intIDEmployee, xBegin, xEnd, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el estado de calculo del proceso de tareas.<br/>
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Status, IDEmployee "</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskPlan(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roScheduler.GetTaskPlan(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el estado de calculo del proceso de presencia.<br/>
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Status, IDEmployee "</returns>
    ''' <remarks></remarks>

    Public Shared Function GetScheduleStatus(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roScheduler.GetScheduleStatus(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function AssignShift(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal intIDShift1 As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer,
                                    ByVal intIDShift4 As Integer, ByVal xStartShift1 As DateTime, ByVal xStartShift2 As DateTime, ByVal xStartShift3 As DateTime,
                                    ByVal xStartShift4 As DateTime, ByVal intIDAssignment As Integer,
                                    ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                    ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roScheduler.AssignShift(intIDEmployee, xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                intIDAssignment, _LockedDayAction, _CoverageDayAction, bState, , , bAudit, _ShiftPermissionAction)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function AssignAlterShift(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal intIDAlterShift As Integer, ByVal xAlterStartShift As DateTime, ByVal _LockedDayAction As LockedDayAction, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roScheduler.AssignAlterShift(intIDEmployee, xDate, intIDAlterShift, xAlterStartShift, _LockedDayAction, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function AssignWeekShifts(ByVal lstEmployees As ArrayList, ByVal lstWeekShifts As ArrayList, ByVal lstWeekStartShifts As Generic.List(Of DateTime),
                                         ByVal lstWeekAssignments As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                         ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                         ByVal intIDEmployeeLocked As Integer, ByVal xDateLocked As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeCopyPlanResult)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeCopyPlanResult)
        Dim oEmployeeCopyPlanResult As New roEmployeeCopyPlanResult

        Dim bRes As Boolean = roScheduler.AssignWeekShifts(lstEmployees, lstWeekShifts, lstWeekStartShifts, lstWeekAssignments, xBeginDate, xEndDate,
                                                        _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, xDateLocked, bState)

        oEmployeeCopyPlanResult.IDEmployeeLocked = intIDEmployeeLocked
        oEmployeeCopyPlanResult.EmployeeLockDate = xDateLocked
        oEmployeeCopyPlanResult.CopyPlanResult = bRes

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function RemoveShift(ByVal intIDEmployee As Long, ByVal intShiftPosition As Integer, ByVal xDate As Date, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roScheduler.RemoveShift(intShiftPosition, intIDEmployee, xDate, _LockedDayAction, _CoverageDayAction, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el texto del comentario introducido en la pantalla de edición de fichajes para un empleado y fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Texto con el comentario.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetRemarkText(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        Dim strRet As String = ""
        Dim oShiftState As New Shift.roShiftState(bState.IDPassport)
        Dim oDailySchedule As New Shift.roDailySchedule(intIDEmployee, xDate, oShiftState)
        If oDailySchedule.Remarks IsNot Nothing Then
            strRet = oDailySchedule.Remarks.Text
        End If
        oResult.Value = strRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function SetRemarkText(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal strText As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oShiftState As New Shift.roShiftState(bState.IDPassport)
        Dim oDailySchedule As New Shift.roDailySchedule(intIDEmployee, xDate, oShiftState)
        If strText.Trim = String.Empty AndAlso oDailySchedule.Remarks IsNot Nothing Then
            oDailySchedule.Remarks.Delete(True)
            oDailySchedule.Remarks = Nothing
        Else
            Dim remarks As New Shift.roRemark(oShiftState)
            oDailySchedule.Remarks = remarks
            oDailySchedule.Remarks.Text = strText
        End If

        oResult.Value = oDailySchedule.Save()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

#Region "Presence Query Methods"


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

    Public Shared Function GetPresenceStatus(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _LastMoveType As MovementStatus, ByVal _LastMoveDateTime As DateTime, ByVal _LastMove As Move.roMove, ByVal _PresenceMinutes As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roMovePresenceStatus)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roMovePresenceStatus)

        Dim oRet As New roMovePresenceStatus
        oRet.PresenceStatus = roScheduler.GetPresenceStatus(_IDEmployee, _InputDateTime, _LastMoveType, _LastMoveDateTime, _LastMove, _PresenceMinutes, bState)
        oRet.LastMoveDateTime = _LastMoveDateTime
        oRet.LastMoveId = If(_LastMove IsNot Nothing, _LastMove.IDMove, -1)
        oRet.LastMoveType = _LastMoveType
        oRet.PresenceMinutes = _PresenceMinutes

        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetPresenceStatusEx(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _LastPunchType As PunchStatus, ByVal _LastPunchDateTime As DateTime, ByVal _LastPunch As Punch.roPunch, ByVal _PresenceMinutes As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roPunchPresenceStatus)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPunchPresenceStatus)

        Dim oRet As New roPunchPresenceStatus
        oRet.PresenceStatus = roScheduler.GetPresenceStatusEx(_IDEmployee, _InputDateTime, _LastPunchType, _LastPunchDateTime, _LastPunch, _PresenceMinutes, bState)
        oRet.LastMoveDateTime = _LastPunchDateTime
        oRet.LastPunchId = If(_LastPunch IsNot Nothing, _LastPunch.ID, -1)
        oRet.LastMoveType = _LastPunchType
        oRet.PresenceMinutes = _PresenceMinutes

        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetMoves(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As DataSet = Nothing
        Dim tbMoves As DataTable = roPunch.GetMoves(intIDEmployee, xDate, xDate, bolIncludeCaptures, bState)
        If oState.Result = EmployeeResultEnum.NoError AndAlso
           tbMoves IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(tbMoves)
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene los fichajes de presencia de un empleado por una fecha, incluyendo las imagenes en caso necesario.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="bolIncludeCaptures">Para indicar si se incluyen la imágenes de los fichajes si existen.</param>
    ''' <remarks></remarks>

    Public Shared Function GetPunchesPres(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As DataSet = Nothing
        Dim tbPunches As DataTable = roPunch.GetPunchesPres(intIDEmployee, xDate, xDate, bolIncludeCaptures, bState, strFilter)
        If oState.Result = EmployeeResultEnum.NoError AndAlso
           tbPunches IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(tbPunches)
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los fichajes de presencia de un empleado por una fecha, incluyendo las imagenes en caso necesario.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha.</param>
    ''' <param name="xEndDate">Fecha final.</param>
    ''' <param name="bolIncludeCaptures">Para indicar si se incluyen la imágenes de los fichajes si existen.</param>
    ''' <remarks></remarks>

    Public Shared Function GetPunchesPresPeriod(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal xEndDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As DataSet = Nothing
        Dim tbPunches As DataTable = roPunch.GetPunchesPres(intIDEmployee, xDate, xEndDate, bolIncludeCaptures, bState, strFilter)
        If oState.Result = EmployeeResultEnum.NoError AndAlso
           tbPunches IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(tbPunches)
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetIncompleteMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As New DataSet
        Dim tb As DataTable = roPunch.GetIncompletMoves(xBegin, xEnd, intIDGroup, strIDEmployees, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetIncompleteDays(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As New DataSet
        Dim tb As DataTable = roPunch.GetIncompletDays(xBegin, xEnd, intIDGroup, strIDEmployees, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetSuspiciousMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As New DataSet
        Dim tb As DataTable = roPunch.GetSuspiciousMoves(xBegin, xEnd, intIDGroup, strIDEmployees, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetSuspiciousPunches(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As New DataSet
        Dim tb As DataTable = roPunch.GetSuspiciousPunches(xBegin, xEnd, intIDGroup, strIDEmployees, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el último movimiento realizado por un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roMove' con la información del último movimiento del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetLastMove(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Move.roMove)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Move.roMove)

        oResult.Value = roPunch.GetLastMove(intIDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene el último fichaje de presencia realizado por un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código de empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objecto 'roPunch' con la información del último movimiento del empleado.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetLastPunchPres(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Punch.roPunch)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Punch.roPunch)

        oResult.Value = roPunch.GetLastPunchPres(intIDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetEmployeeAssignments(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roEmployeeAssignment))

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roEmployeeAssignment))

        oResult.Value = roEmployeeAssignment.GetEmployeeAssignments(intIDEmployee, bState, False)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los puestos que tiene asignado el empleado y su idoneidad en un dataset.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment, Suitability </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeeAssignmentsDatatable(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roEmployeeAssignment.GetEmployeeAssignmentsDataTable(intIDEmployee, bState, False)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guardamos los puestos asignados al empleado
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="lstEmployeeAssignments">Lista de puestos asignados .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment, Suitability </returns>
    ''' <remarks></remarks>

    Public Shared Function SaveEmployeeAssignments(ByVal intIDEmployee As Integer, ByVal lstEmployeeAssignments As Generic.List(Of roEmployeeAssignment), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployeeAssignment.SaveEmployeeAssignments(intIDEmployee, lstEmployeeAssignments, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetIncidences(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roIncidence.GetIncidences(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los valores de las incidencias generadas de empleados , fechas y tipos de incidencia.
    ''' </summary>
    ''' <param name="intIDEmployees">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, IncidenceValue, DailyCauses.Value, DailyCauses.Manual, DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, BeginTimeOrder </returns>
    ''' <remarks></remarks>

    Public Shared Function GetMassIncidences(ByVal intIDEmployees As String, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strIncidencesType As String, ByVal OnlyNotJustified As Boolean, ByVal strCenters As String, ByVal strCausesFilter As String, ByVal strCausesValueFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roIncidence.GetMassIncidences(intIDEmployees, xBeginDate, xEndDate, strIncidencesType, OnlyNotJustified, strCenters, strCausesFilter, strCausesValueFilter, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los valores de las incidencias generadas de empleados , fechas y tipos de incidencia.
    ''' </summary>
    ''' <param name="intIDEmployees">Código del empleado.</param>
    ''' <param name="xBeginDate">Fecha</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, IncidenceValue, DailyCauses.Value, DailyCauses.Manual, DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, BeginTimeOrder </returns>
    ''' <remarks></remarks>

    Public Shared Function GetMassCenters(ByVal intIDEmployees As String, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strCausesIDs As String, ByVal oState As roWsState, strBusinessCenters As String, Optional directEmployeeIds As String = "-1", Optional directGroupIds As String = "-1") As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roIncidence.GetMassCenters(intIDEmployees, xBeginDate, xEndDate, strCausesIDs, bState, strBusinessCenters, directEmployeeIds, directGroupIds)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetIncompleteIncidences(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roIncidence.GetIncompletIncidences(xBegin, xEnd, intIDGroup, strIDEmployees, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveDailyCauses(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal ds As DataSet, ByVal bolUpdateStatus As Boolean, ByVal oState As roWsState,
                                        ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New Cause.roCauseState()
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim newGState As New roWsState
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bolRet As Boolean = False
        If ds.Tables.Count > 0 Then

            Dim oDailyCauseList As New Cause.roDailyCauseList(bState)
            bolRet = oDailyCauseList.Save(intIDEmployee, xDate, ds.Tables(0), bolUpdateStatus, bAudit)
            If Not bolRet Then
                roWsStateManager.CopyTo(oDailyCauseList.State, newGState)
            Else
                oState.Result = EmployeeResultEnum.NoError
                newGState = oState
            End If
        Else
            oState.Result = EmployeeResultEnum.DailyCausesInvalidData
            newGState = oState
        End If

        oResult.Value = bolRet
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetDailyAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetDailyAccruals(intIDEmployee, xDate, bState, filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los saldos diarios de tareas de un empleado para una fecha.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado.</param>
    ''' <param name="xDate">Fecha de la que se recuperarán los saldos.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: DailyTaskAccruals.IDTask, Tasks.Name, DailyTaskAccruals.Value, ValueFormat. </returns>
    ''' <remarks></remarks>

    Public Shared Function GetDailyTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetDailyTaskAccruals(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetAnualAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetAnualAccruals(intIDEmployee, xDate, bState, , , filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetTaskAccruals(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetMonthAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetMonthAccruals(intIDEmployee, xDate, bState, , filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetWeekAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetWeekAccruals(intIDEmployee, xDate, bState, , filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetContractAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetContractAccruals(intIDEmployee, xDate, bState, , , filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene el valor de los saldos por año laboral de un empleado para una fecha.<br/>
    ''' Utiliza el día definido como inicio de semana para determinar a partir de que fecha tiene que acumular los valores.
    ''' </summary>
    ''' <param name="intIDEmployee">Código empleado.</param>
    ''' <param name="xDate">Fecha hasta la que se acumularán los valores.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDConcept, Concepts.Name, Total, TotalFormat (valor formateado según el tipo del saldo: H o O), Concepts.DefaultQuery (tipo de saldo, Y:anual, M:Mensual, W:Semanal) </returns>
    ''' <remarks></remarks>

    Public Shared Function GetAnnualWorkAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetContractAnnualizedAccruals(intIDEmployee, xDate, bState, , , filterBusinessGroups)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetMonthTaskAccruals(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetMonthTaskAccruals(intIDEmployee, xDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetAccrualsQuery(ByVal _IDEmployee As Integer, ByVal _CurrentDate As Date, ByVal _OnlyViewInTerminals As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = roConcept.GetAccrualsQuery(_IDEmployee, _CurrentDate, _OnlyViewInTerminals, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyAccrualRules(ByVal intIDSourceEmployee As Integer, ByVal intIDDestinationEmployee As Integer, ByVal bolDescartarReglasActuales As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roConcept.CopyAccrualRules(intIDSourceEmployee, intIDDestinationEmployee, bolDescartarReglasActuales, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function CopyConceptAnnualLimits(ByVal intIDEmployeeSource As Integer, ByVal intIDEmployeeDestination As Integer, ByVal bolDiscardAnnualLimits As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roConcept.CopyConceptAnnualLimits(intIDEmployeeSource, intIDEmployeeDestination, bolDiscardAnnualLimits, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

#End Region

#Region "Biometric Data"

    ''' <summary>
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteBiometricData(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.DeleteBiometricData(IDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' </summary>    
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteBiometricDataForAllEmployees(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim certificateManager As New roBioCertificateManager(New roDocumentState(bState.IDPassport))
        Dim companyGUID As String = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName())
        Dim oCompanyInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(companyGUID)

        oResult.Value = (certificateManager.GenerateCertificate(oCompanyInfo) AndAlso roEmployee.DeleteBiometricDataForAllEmployees(bState))

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function DisableBiometricDataForAllEmployees(ByVal disabled As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.DisableBiometricDataForAllEmployees(disabled, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="disabled">Desactivado o activado</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function EnableOrDisableBiometricData(ByVal IDEmployee As Integer, ByVal disabled As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.EnableOrDisableBiometricData(IDEmployee, disabled, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="IDEmployee">Código empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteEmployeeRelatedData(ByVal IDEmployee As Integer, ByVal bRemoveEmployeePhoto As Boolean, ByVal bRemoveBiometricData As Boolean, ByVal bRemoveEmployeePunchImages As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roEmployee.DeleteEmployeeRelatedData(IDEmployee, bRemoveEmployeePhoto, bRemoveBiometricData, bRemoveEmployeePunchImages, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function VacationsResumeQuery(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                             ByVal xVacationsDate As Date, ByVal intDone As Double, ByVal intPending As Double, ByVal intLasting As Double, ByVal intDisponible As Double, ByVal intExpiredDays As Double, ByVal intDaysWithoutEnjoyment As Double,
                                             ByVal oState As roWsState) As roGenericVtResponse(Of roHolidayResumeInfo)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roHolidayResumeInfo)
        Dim oHolidayResumeInfo As New roHolidayResumeInfo

        If roBusinessSupport.VacationsResumeQuery(IDEmployee, IDShift, xCurrentDate, xBeginPeriod, xEndPeriod, xVacationsDate, intDone, intPending, intLasting, intDisponible, bState, intExpiredDays, intDaysWithoutEnjoyment) Then
            oHolidayResumeInfo.BeginPeriod = xBeginPeriod
            oHolidayResumeInfo.EndPeriod = xEndPeriod
            oHolidayResumeInfo.Done = intDone
            oHolidayResumeInfo.Disponible = intDisponible
            oHolidayResumeInfo.Lasting = intLasting
            oHolidayResumeInfo.Pending = intPending
            oHolidayResumeInfo.ExpiredDays = intExpiredDays
            oHolidayResumeInfo.DaysWithoutEnjoyment = intDaysWithoutEnjoyment

        Else
            oHolidayResumeInfo = Nothing
        End If
        oResult.Value = oHolidayResumeInfo

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function ProgrammedHolidaysResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                             ByVal xProgrammedHolidaysDate As Date, ByVal intPending As Double, ByVal intLasting As Double, ByVal intDisponible As Double,
                                             ByVal oState As roWsState) As roGenericVtResponse(Of roHolidayResumeInfo)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roHolidayResumeInfo)
        Dim oHolidayResumeInfo As New roHolidayResumeInfo
        If roBusinessSupport.ProgrammedHolidaysResumeQuery(IDEmployee, IDCause, xCurrentDate, xBeginPeriod, xEndPeriod, xProgrammedHolidaysDate, intPending, intLasting, intDisponible, bState) Then
            oHolidayResumeInfo.BeginPeriod = xBeginPeriod
            oHolidayResumeInfo.EndPeriod = xEndPeriod
            oHolidayResumeInfo.Done = 0
            oHolidayResumeInfo.Disponible = intDisponible
            oHolidayResumeInfo.Lasting = intLasting
            oHolidayResumeInfo.Pending = intPending
        Else
            oHolidayResumeInfo = Nothing
        End If
        oResult.Value = oHolidayResumeInfo

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetTelecommuteStatsAtDate(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roEmployeeTelecommuteAgreementStats)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeTelecommuteAgreementStats)
        oResult.Value = VTEmployees.Employee.roEmployeeTelecommuteAgreement.GetTelecommuteStatsAtDate(IDEmployee, xBeginPeriod, Nothing, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function LaboralDaysInPeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roBusinessSupport.LaboralDaysInPeriod(IDEmployee, xBeginPeriod, xEndPeriod, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetEmployeeStatus(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEmployeeStatus)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEmployeeStatus)
        oResult.Value = New roEmployeeStatus(IDEmployee, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

#End Region

#Region "Forecasts"

    Public Shared Function GetEmployeeForecastsInPeriod(ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessSupport.GetEmployeeForecastsInPeriod(IDEmployee, _BeginDate, bState)
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

    Public Shared Function GetCoverageEmployees(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roScheduler.GetCoverageEmployees(_IDEmployee, _CoverageDate, bState)
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

    Public Shared Function AddEmployeeCoverage(ByVal _IDEmployeeCoverage As Integer, ByVal _IDEmployeeCovered As Integer, ByVal _CoverageDate As Date, ByVal _IDShift As Integer, ByVal _LockedDayActionEmployeeCovered As LockedDayAction, ByVal _LockedDayActionEmployeeCoverage As LockedDayAction, ByVal _IDEmployeeLocked As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroupState As New roAccessGroupState(oState.IDPassport)
        oResult.Value = roScheduler.AddEmployeeCoverage(_IDEmployeeCoverage, _IDEmployeeCovered, _CoverageDate, bState, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _IDEmployeeLocked, _IDShift, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroupState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function RemoveEmployeeCoverage(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal _LockedDayActionEmployeeCovered As LockedDayAction, ByVal _LockedDayActionEmployeeCoverage As LockedDayAction, ByVal _EmployeeType As Integer, ByVal _IDEmployeeLocked As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroupState As New roAccessGroupState(oState.IDPassport)
        oResult.Value = roScheduler.RemoveEmployeeCoverage(_IDEmployee, _CoverageDate, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _EmployeeType, _IDEmployeeLocked, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroupState, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function GetDailyCoverageEmployees(ByVal _IDGroup As Integer, ByVal _IDAssignment As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roScheduler.GetDailyCoverageEmployees(_IDGroup, _IDAssignment, _CoverageDate, bState)
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

#End Region

#Region "Authorizations"

    Public Shared Function AddAccessGroupToEmployee(ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, ByVal bolAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroupState As New roAccessGroupState(oState.IDPassport)
        oResult.Value = roAccessGroup.SaveEmployeeAccessGroup(_IDEmployee, _IDAuthorization, oAccessGroupState, True, , bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroupState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function RemoveAccessGroupToEmployee(ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, ByVal bolAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroupState As New roAccessGroupState(oState.IDPassport)
        oResult.Value = roAccessGroup.RemoveEmployeeAccessGroup(_IDEmployee, _IDAuthorization, oAccessGroupState, True, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroupState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveAccessAuthorizations(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal dsAuthorizations As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEmployeeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroupState As New roAccessGroupState(oState.IDPassport)
        oResult.Value = roAccessGroup.SaveAccessAuthorizations(IDEmployee, IDGroup, dsAuthorizations, oAccessGroupState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroupState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

#End Region

End Class