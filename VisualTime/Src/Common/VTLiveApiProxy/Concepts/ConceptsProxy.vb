Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.DataLayer.AccessHelper

Public Class ConceptsProxy
    Implements IConceptsSvc

    Public Function KeepAlive() As Boolean Implements IConceptsSvc.KeepAlive
        Return True
    End Function

#Region "Concepts"
    ''' <summary>
    ''' Obtiene el saldo con el ID indicado
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo a obtener</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un saldo (roConcept)</returns>
    ''' <remarks></remarks>
    Public Function GetConceptByID(ByVal IDConcept As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConcept) Implements IConceptsSvc.GetConceptByID

        Return ConceptsMethods.GetConceptByID(IDConcept, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene el saldo con el Nombre corto indicado
    ''' </summary>
    ''' <param name="ShortName">Nombre corto del saldo que buscamos</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un saldo (roConcept)</returns>
    ''' <remarks></remarks>
    Public Function GetConceptByShortName(ByVal ShortName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConcept) Implements IConceptsSvc.GetConceptByShortName

        Return ConceptsMethods.GetConceptByShortName(ShortName, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene el Grupo de saldos con el ID indicado
    ''' </summary>
    ''' <param name="IDConceptGroup">ID del grupo de saldos a obtener</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un grupo de saldos (roConceptGroup)</returns>
    ''' <remarks></remarks>
    Public Function GetConceptGroupByID(ByVal IDConceptGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConceptGroup) Implements IConceptsSvc.GetConceptGroupByID

        Return ConceptsMethods.GetConceptGroupByID(IDConceptGroup, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todos los saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los saldos (Concepts) ordenados por nombre</returns>
    ''' <remarks></remarks>
    Public Function GetConcepts(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetConcepts

        Return ConceptsMethods.GetConcepts(oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los saldos que se pueden utilizar en los horarios de vacaciones
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los saldos (Concepts) ordenados por nombre que se pueden utilizar en los horarios de vacaciones</returns>
    ''' <remarks></remarks>
    Public Function GetHolidaysConcepts(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetHolidaysConcepts

        Return ConceptsMethods.GetHolidaysConcepts(oState)
    End Function



    ''' <summary>
    ''' Devuelve un dataset con el ID y Name de todos los saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un dataset con el ID y Name de todos los saldos ordenados por nombre</returns>
    ''' <remarks></remarks>
    Public Function GetConceptList(ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetConceptList

        Return ConceptsMethods.GetConceptList(oState, filterBusinessGroups)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con el id y nombre de todas las notificaciones
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConceptDataset(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetConceptDataset

        Return ConceptsMethods.GetConceptDataset(oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todos los grupos de saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los Grupos de saldos (sysroReportGroups)</returns>
    ''' <remarks></remarks>
    Public Function GetConceptGroups(ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetConceptGroups

        Return ConceptsMethods.GetConceptGroups(filterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    Public Function GetBusinessGroupFromConceptGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetBusinessGroupFromConceptGroups

        Return ConceptsMethods.GetBusinessGroupFromConceptGroups(oState)
    End Function

    ''' <summary>
    ''' Valida los datos del saldo.<br/>
    ''' Comprueba que:<br/>
    ''' - El nombre no puede estar en blanco<br/>
    ''' - El nombre no exista<br/>
    ''' - El nombre abreviado no exista<br/>
    ''' - Intervalo de fechas (BeginDate / FinishDate) sea correcto<br/>
    ''' - Que no haya mas de 4 saldos por terminal (ViewInTerminals)<br/>
    ''' - Campo Userfield informado<br/>
    ''' - La composición del acumulado
    ''' </summary>
    ''' <param name="oConcept">Saldo a validar (roConcept)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido validar.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function ValidateConcept(ByVal oConcept As roConcept, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.ValidateConcept

        Return ConceptsMethods.ValidateConcept(oConcept, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos del saldo. Si és nuevo, se actualiza el ID del saldo pasado.<br/>
    ''' Se ejecuta ValidateConcept()
    ''' </summary>
    ''' <param name="oConcept">Saldo a guardar (roConcept)</param>
    ''' <param name="ClosingDate">Fecha de cierre para generar el saldo obsoleto y guardar el nuevo.</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el saldo.</returns>
    ''' <remarks></remarks>
    Public Function SaveConcept(ByVal oConcept As roConcept, ByVal ClosingDate As Nullable(Of Date), ByVal DefinitionHasChanged As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (Boolean, roConcept)) Implements IConceptsSvc.SaveConcept

        Return ConceptsMethods.SaveConcept(oConcept, ClosingDate, DefinitionHasChanged, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda los datos del grupo de saldos. Si és nuevo, se actualiza el ID del saldo pasado.<br/>
    ''' Se ejecuta ValidateConceptGroup()
    ''' </summary>
    ''' <param name="oConceptGroup">Grupo de saldo a guardar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function SaveConceptGroup(ByVal oConceptGroup As roConceptGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (Boolean, roConceptGroup)) Implements IConceptsSvc.SaveConceptGroup

        Return ConceptsMethods.SaveConceptGroup(oConceptGroup, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina el saldo con el ID indicado<br/>
    ''' Comprueba:<br/>
    ''' - Si esta en uso<br/>
    ''' Realiza:<br/>
    ''' - Borra de DailyAccruals<br/>
    ''' - Borra de EmployeeConceptCarryOvers<br/>
    ''' - Borra de EmployeeConceptAnnualLimits<br/>
    ''' - Borra de Concepts<br/>
    ''' Ejecuta Task.CONCEPTS
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteConcept(ByVal IDConcept As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteConcept

        Return ConceptsMethods.DeleteConcept(IDConcept, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borra el grupo de saldos con el ID indicado<br/>
    ''' Comprueba: <br/>
    ''' - Si esta en uso<br/>
    ''' Realiza: <br/>
    ''' - Borra de sysroReportGroupConcepts<br/>
    ''' - Borra de sysroReportGroups
    ''' </summary>
    ''' <param name="IDConceptGroup">ID del grupo de saldos</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteConceptGroup(ByVal IDConceptGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteConceptGroup

        Return ConceptsMethods.DeleteConceptGroup(IDConceptGroup, oState, bAudit)
    End Function

    ''' <summary>
    ''' Indica si el saldo está en uso.
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo a comprobar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si el saldo se encuentra en uso.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function ConceptIsUsed(ByVal IDConcept As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.ConceptIsUsed

        Return ConceptsMethods.ConceptIsUsed(IDConcept, oState)
    End Function

    ''' <summary>
    ''' Borrar los valores iniciales de un saldo<br/>
    ''' Realiza:<br/>
    ''' - Borra los registros de las tablas EmployeeConceptCarryOvers y EmployeeConceptAnnualLimits que contengan el código del saldo.
    ''' </summary>
    ''' <param name="_IDConcept">Código del saldo</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteStartupValues(ByVal _IDConcept As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteStartupValues

        Return ConceptsMethods.DeleteStartupValues(_IDConcept, oState)
    End Function

    ''' <summary>
    ''' Devuelve la fecha más antigua calculada de un saldo. Si no hay ninguna fecha calculada, devuelve nothing.
    ''' </summary>
    ''' <param name="_IDConcept">Código del saldo</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function GetConceptOldestDate(ByVal _IDConcept As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Nullable(Of Date)) Implements IConceptsSvc.GetConceptOldestDate

        Return ConceptsMethods.GetConceptOldestDate(_IDConcept, oState)
    End Function

    ''' <summary>
    ''' Devuelve las justificaciones de un saldo
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo</param>
    ''' <returns>DataSet (ConceptCauses, Causes) ordenado por Causes.Name</returns>
    ''' <remarks></remarks>

    Public Function GetConceptsCauses(ByVal IDConcept As Integer) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetConceptsCauses

        Return ConceptsMethods.GetConceptsCauses(IDConcept)
    End Function


    ''' <summary>
    ''' Recupera los datos de una Regla de saldo por ID
    ''' </summary>
    ''' <param name="IdAccrualsRule">ID de regla de saldo</param>
    ''' <returns>Regla de Saldo (roAccrualsRule)</returns>
    ''' <remarks></remarks>

    Public Function GetAccrualRuleByID(ByVal IdAccrualsRule As Integer) As roGenericVtResponse(Of roAccrualsRule) Implements IConceptsSvc.GetAccrualRuleByID

        Return ConceptsMethods.GetAccrualRuleByID(IdAccrualsRule)
    End Function

    ''' <summary>
    ''' Recupera un dataset con la lista de Reglas de saldos
    ''' </summary>
    ''' <returns>DataSet (AccrualsConcepts) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Function GetAccrualsRules() As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetAccrualsRules

        Return ConceptsMethods.GetAccrualsRules()
    End Function

    ''' <summary>
    ''' Recupera el siguiente codigo de Regla de saldo
    ''' </summary>
    ''' <returns>Siguiente ID de la reglad de saldo</returns>
    ''' <remarks></remarks>

    Public Function GetNextIDAccrualsRule() As roGenericVtResponse(Of Integer) Implements IConceptsSvc.GetNextIDAccrualsRule

        Return ConceptsMethods.GetNextIDAccrualsRule()
    End Function

    ''' <summary>
    ''' Guarda una regla de saldo
    ''' </summary>
    ''' <param name="oAccrualsRule">Regla de saldo a guardar (roAccrualsRule)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SaveAccrualsRule(ByVal oAccrualsRule As roAccrualsRule) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.SaveAccrualsRule

        Return ConceptsMethods.SaveAccrualsRule(oAccrualsRule)
    End Function

    ''' <summary>
    ''' Eliminar regla de saldos (No implementada)
    ''' </summary>
    ''' <param name="IDAccrualsRule">ID de saldo a eliminar</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteAccrualsRule(ByVal IDAccrualsRule As Integer) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteAccrualsRule

        Return ConceptsMethods.DeleteAccrualsRule(IDAccrualsRule)
    End Function

    ''' <summary>
    '''  Devuelve los datos de un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">ID de grupo de saldos</param>
    ''' <returns>Devuelve un grupo de saldo (roReportGroup)</returns>
    ''' <remarks></remarks>

    Public Function GetReportGroup(ByVal IDReportGroup As Integer) As roGenericVtResponse(Of roReportGroup) Implements IConceptsSvc.GetReportGroup

        Return ConceptsMethods.GetReportGroup(IDReportGroup)
    End Function

    ''' <summary>
    ''' Devuelve una lista de los grupos de saldos existentes
    ''' </summary>
    ''' <returns>Devuelve un DataSet (sysroReportGroups) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Function GetReportGroups(ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetReportGroups

        Return ConceptsMethods.GetReportGroups(filterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Devuelve una lista de los saldos que componen un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">ID de grupo de saldo</param>
    ''' <returns>DataSet (sysroReportGroupConcepts, Concepts) ordenado por Position</returns>
    ''' <remarks></remarks>

    Public Function GetReportGroupsConcepts(ByVal IDReportGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IConceptsSvc.GetReportGroupsConcepts

        Return ConceptsMethods.GetReportGroupsConcepts(IDReportGroup, oState)
    End Function

    ''' <summary>
    ''' Guarda un grupo de saldos
    ''' </summary>
    ''' <param name="oReportGroup">Grupo de saldo a guardar (roReportGroup)</param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Function SaveReportGroup(ByVal oReportGroup As roReportGroup) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.SaveReportGroup

        Return ConceptsMethods.SaveReportGroup(oReportGroup)
    End Function

    ''' <summary>
    ''' Guarda los saldos pertenecientes a un Grupo de saldos
    ''' </summary>
    ''' <param name="oConcepts"></param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Function AddReportGroupConcepts(ByVal oConcepts As ArrayList) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.AddReportGroupConcepts

        Return ConceptsMethods.AddReportGroupConcepts(oConcepts)
    End Function

    ''' <summary>
    ''' Elimina un grupo de saldos
    ''' </summary>
    ''' <param name="IDREportGroup"></param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Function DeleteReportGroup(ByVal IDREportGroup As Integer) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteReportGroup

        Return ConceptsMethods.DeleteReportGroup(IDREportGroup)
    End Function
    ''' <summary>
    ''' Borra los saldos de un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">Id del grupo de saldos</param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Function DeleteReportGroupsConcepts(ByVal IDReportGroup As Integer) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.DeleteReportGroupsConcepts
        Return ConceptsMethods.DeleteReportGroupsConcepts(IDReportGroup)
    End Function

    ''' <summary>
    ''' Modifica la posición de un saldo dentro de un grupo de saldos.
    ''' </summary>
    ''' <param name="IDReportGroup">Código del grupo de saldos</param>
    ''' <param name="IDConcept">Código del saldo</param>
    ''' <param name="bolUp">True para subir. False para bajar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function SetReportGroupConceptPosition(ByVal IDReportGroup As Integer, ByVal IDConcept As Integer, ByVal bolUp As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IConceptsSvc.SetReportGroupConceptPosition
        Return ConceptsMethods.SetReportGroupConceptPosition(IDReportGroup, IDConcept, bolUp, oState)
    End Function
#End Region

End Class
