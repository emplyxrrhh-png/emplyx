Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.DataLayer.AccessHelper

Public Class ConceptsMethods

#Region "Concepts"

    Class ErrorEnumClass
        ' Esta clase sólo sirve para serializar la enumeración
        ' pq no se puede serializar una enumeración directamente (bug)
        Public oEnumError As ConceptResultEnum
    End Class

    ''' <summary>
    ''' Obtiene el saldo con el ID indicado
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo a obtener</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un saldo (roConcept)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptByID(ByVal IDConcept As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConcept)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roConcept)
        oResult.Value = New roConcept(IDConcept, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene el saldo con el Nombre corto indicado
    ''' </summary>
    ''' <param name="ShortName">Nombre corto del saldo que buscamos</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un saldo (roConcept)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptByShortName(ByVal ShortName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConcept)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roConcept)
        oResult.Value = roConcept.GetConceptByShortName(ShortName, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene el Grupo de saldos con el ID indicado
    ''' </summary>
    ''' <param name="IDConceptGroup">ID del grupo de saldos a obtener</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un grupo de saldos (roConceptGroup)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptGroupByID(ByVal IDConceptGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roConceptGroup)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roConceptGroup)
        oResult.Value = New roConceptGroup(IDConceptGroup, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todos los saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los saldos (Concepts) ordenados por nombre</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConcepts(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetConcepts(bState)

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
    ''' Devuelve un dataset con los saldos que se pueden utilizar en los horarios de vacaciones
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los saldos (Concepts) ordenados por nombre que se pueden utilizar en los horarios de vacaciones</returns>
    ''' <remarks></remarks>
    Public Shared Function GetHolidaysConcepts(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetHolidaysConcepts(bState)

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
    ''' Devuelve un dataset con el ID y Name de todos los saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un dataset con el ID y Name de todos los saldos ordenados por nombre</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptList(ByVal oState As roWsState, ByVal filterBusinessGroups As Boolean) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetConceptList(bState, filterBusinessGroups)

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
    ''' Devuelve un dataset con el id y nombre de todas las notificaciones
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptDataset(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetConceptDataset(bState)

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
    ''' Devuelve un dataset con todos los grupos de saldos
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los Grupos de saldos (sysroReportGroups)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConceptGroups(ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConceptGroup.GetConceptGroups(bState, filterBusinessGroups)

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
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    Public Shared Function GetBusinessGroupFromConceptGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roConceptGroup.GetBusinessGroupFromConceptGroups(bState)

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
    Public Shared Function ValidateConcept(ByVal oConcept As roConcept, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oConcept.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function SaveConcept(ByVal oConcept As roConcept, ByVal ClosingDate As Nullable(Of Date), ByVal DefinitionHasChanged As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (Boolean, roConcept))

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        oConcept.State = bState

        Dim oResult As New roGenericVtResponse(Of (Boolean, roConcept))
        Dim bolRet As Boolean
        If ClosingDate.HasValue Then
            bolRet = oConcept.Save(ClosingDate.Value, DefinitionHasChanged, bAudit)
        Else
            bolRet = oConcept.Save(DefinitionHasChanged, , bAudit)
        End If

        oResult.Value = (bolRet, oConcept)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos del grupo de saldos. Si és nuevo, se actualiza el ID del saldo pasado.<br/>
    ''' Se ejecuta ValidateConceptGroup()
    ''' </summary>
    ''' <param name="oConceptGroup">Grupo de saldo a guardar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function SaveConceptGroup(ByVal oConceptGroup As roConceptGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (Boolean, roConceptGroup))

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        oConceptGroup.State = bState

        Dim oResult As New roGenericVtResponse(Of (Boolean, roConceptGroup))
        oResult.Value = (oConceptGroup.Save(bAudit), oConceptGroup)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function DeleteConcept(ByVal IDConcept As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oConcept As New roConcept(IDConcept, bState, False)
        oResult.Value = oConcept.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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
    Public Shared Function DeleteConceptGroup(ByVal IDConceptGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oConceptGroup As New roConceptGroup(IDConceptGroup, bState, False)

        oResult.Value = oConceptGroup.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Indica si el saldo está en uso.
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo a comprobar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si el saldo se encuentra en uso.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function ConceptIsUsed(ByVal IDConcept As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oConcept As New roConcept(IDConcept, bState, False)

        oResult.Value = oConcept.IsUsed()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la fecha más antigua calculada de un saldo. Si no hay ninguna fecha calculada, devuelve nothing.
    ''' </summary>
    ''' <param name="_IDConcept">Código del saldo</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción.</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function GetConceptOldestDate(ByVal _IDConcept As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Nullable(Of Date))

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Nullable(Of Date))

        oResult.Value = roConcept.GetConceptOldestDate(_IDConcept, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve las justificaciones de un saldo
    ''' </summary>
    ''' <param name="IDConcept">ID del saldo</param>
    ''' <returns>DataSet (ConceptCauses, Causes) ordenado por Causes.Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetConceptsCauses(ByVal IDConcept As Integer) As roGenericVtResponse(Of DataSet)

        Dim strQuery As String

        strQuery = "@SELECT# ConceptCauses.IDCause "
        strQuery = strQuery & ", Causes.Name"
        strQuery = strQuery & ", ConceptCauses.HoursFactor"
        strQuery = strQuery & ", ConceptCauses.OccurrencesFactor "
        strQuery = strQuery & " From ConceptCauses "
        strQuery = strQuery & " Inner Join Causes ON "
        strQuery = strQuery & " Causes.ID = ConceptCauses.IDCause "
        strQuery = strQuery & " Where ConceptCauses.IDConcept = " & IDConcept
        strQuery = strQuery & " Order By Causes.Name "

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = CreateDataSet(strQuery)

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Return oResult
    End Function

    ''' <summary>
    ''' Recupera los datos de una Regla de saldo por ID
    ''' </summary>
    ''' <param name="IdAccrualsRule">ID de regla de saldo</param>
    ''' <returns>Regla de Saldo (roAccrualsRule)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetAccrualRuleByID(ByVal IdAccrualsRule As Integer) As roGenericVtResponse(Of roAccrualsRule)
        ' Recupera los datos de una Regla de saldo

        Dim oResult As New roGenericVtResponse(Of roAccrualsRule)

        Dim oAccrualsRule As New roAccrualsRule
        Dim strQuery As String
        Dim oDataSet As DataSet
        Dim oDataView As DataView
        Dim oDataRowView As DataRowView

        strQuery = " @SELECT# * from AccrualsRules Where IdAccrualsRule = " & IdAccrualsRule

        oDataSet = CreateDataSet(strQuery)

        If oDataSet IsNot Nothing Then
            oDataView = New DataView(oDataSet.Tables(0))

            For Each oDataRowView In oDataView
                oAccrualsRule.IdAccrualsRule = oDataRowView("IdAccrualsRule")
                oAccrualsRule.Name = oDataRowView("Name")
                oAccrualsRule.Description = oDataRowView("Description")
                oAccrualsRule.Definition = oDataRowView("Definition")
                oAccrualsRule.Schedule = oDataRowView("Schedule")
                oAccrualsRule.BeginDate = oDataRowView("BeginDate")
                oAccrualsRule.EndDate = oDataRowView("EndDate")
                oAccrualsRule.Priority = oDataRowView("Priority")
                oAccrualsRule.ExecuteFromLastExecDay = oDataRowView("ExecuteFromLastExecDay")
            Next
        Else
            oAccrualsRule = Nothing
        End If

        oResult.Value = oAccrualsRule

        Return oResult
    End Function

    ''' <summary>
    ''' Recupera un dataset con la lista de Reglas de saldos
    ''' </summary>
    ''' <returns>DataSet (AccrualsConcepts) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetAccrualsRules() As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = CreateDataSet("@SELECT# * from AccrualsConcepts Order By Name")

        If ds Is Nothing Then
            ds = New DataSet

        End If

        oResult.Value = ds

        Return oResult
    End Function

    ''' <summary>
    ''' Recupera el siguiente codigo de Regla de saldo
    ''' </summary>
    ''' <returns>Siguiente ID de la reglad de saldo</returns>
    ''' <remarks></remarks>

    Public Shared Function GetNextIDAccrualsRule() As roGenericVtResponse(Of Integer)
        Dim oResult As New roGenericVtResponse(Of Integer)

        Dim strQuery As String
        Dim oDataSet As System.Data.DataSet
        Dim oDataReader As System.Data.Common.DbDataReader
        Dim intNextID As Integer

        intNextID = -1

        strQuery = " @SELECT# Max(IDAccrualsRule) as Contador From AccrualsRules "

        oDataSet = CreateDataSet(strQuery)
        oDataReader = oDataSet.CreateDataReader

        If oDataReader IsNot Nothing Then
            oDataReader.Read()
            If oDataReader.HasRows Then
                intNextID = oDataReader("Contador") + 1
            End If
        End If
        oDataReader.Close()

        oResult.Value = intNextID

        Return oResult
    End Function

    ''' <summary>
    ''' Guarda una regla de saldo
    ''' </summary>
    ''' <param name="oAccrualsRule">Regla de saldo a guardar (roAccrualsRule)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SaveAccrualsRule(ByVal oAccrualsRule As roAccrualsRule) As roGenericVtResponse(Of Boolean)
        ' Guarda los datos de una Regla de saldo
        Dim oResult As New roConceptState
        Dim strQuery As String
        Dim intIdAccrualsRule As Integer
        Dim resp = New roGenericVtResponse(Of Boolean)

        ' si el oConcept.ID es -1 se trata de un nuevo registro
        If oAccrualsRule.IdAccrualsRule = -1 Then
            Dim strError As String = ""

            Do
                Dim oConcept As New roConcept
                oConcept.State = oResult
                intIdAccrualsRule = oConcept.GetNextID()

                strQuery = " @INSERT# INTO AccrualsRules "
                strQuery = strQuery & " ( IDAccrualsRule, Name, Description, "
                strQuery = strQuery & " , Definition, Schedule "
                strQuery = strQuery & " , BeginDate, EndDate "
                strQuery = strQuery & " , Priority, ExecuteFromLastExecDay ) "
                strQuery = strQuery & " Values "
                strQuery = strQuery & " ( " & intIdAccrualsRule
                strQuery = strQuery & " , '" & oAccrualsRule.Name & "' "
                strQuery = strQuery & " , '" & oAccrualsRule.Description & "' "
                strQuery = strQuery & " , '" & oAccrualsRule.Definition & "' "
                strQuery = strQuery & " , '" & oAccrualsRule.Schedule & "' "
                strQuery = strQuery & " , '" & oAccrualsRule.BeginDate & "' "
                strQuery = strQuery & " , '" & oAccrualsRule.EndDate & "' "
                strQuery = strQuery & " , " & oAccrualsRule.Priority
                strQuery = strQuery & " , " & oAccrualsRule.ExecuteFromLastExecDay
                strQuery = strQuery & " ) "

                Try
                    strError = ""
                    If ExecuteSql(strQuery) Then
                        oResult.Result = ConceptResultEnum.NoError
                        oAccrualsRule.IdAccrualsRule = intIdAccrualsRule
                    Else
                        oResult.Result = ConceptResultEnum.SqlError
                    End If
                    oResult.ErrorText = ""
                Catch ex As Exception
                    oResult.Result = ConceptResultEnum.SqlError
                    strError = ex.ToString
                End Try
                'ppr Loop Until InStr(strError, "PRIMARY KEY") = 0 ' Si el error que se ha producido contiene la clave "PRIMARY KEY" es que alguien a usado el codigo que he recuperado y por tanto tengo que volver a intentarlo.
            Loop Until strError.IndexOf("PRIMARY KEY") = 0 ' Si el error que se ha producido contiene la clave "PRIMARY KEY" es que alguien a usado el codigo que he recuperado y por tanto tengo que volver a intentarlo.

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(oResult, newGState)

            resp.Value = True
            resp.Status = newGState
        Else
            strQuery = " @UPDATE# Concepts "
            strQuery = strQuery & " Set Name = '" & oAccrualsRule.Name & "' "
            strQuery = strQuery & " , Description = '" & oAccrualsRule.Description & "' "
            strQuery = strQuery & " , Definition = '" & oAccrualsRule.Definition & "' "
            strQuery = strQuery & " , Schedule = '" & oAccrualsRule.Schedule & "' "
            strQuery = strQuery & " , BeginDate = '" & oAccrualsRule.BeginDate & "' "
            strQuery = strQuery & " , EndDate = '" & oAccrualsRule.EndDate & "' "
            strQuery = strQuery & " , Priority = " & oAccrualsRule.Priority
            strQuery = strQuery & " , ExecuteFromLastExecDay = " & oAccrualsRule.ExecuteFromLastExecDay
            strQuery = strQuery & " where  IdAccrualsRule = " & oAccrualsRule.IdAccrualsRule

            Try
                If ExecuteSql(strQuery) Then
                    oResult.Result = ConceptResultEnum.NoError
                Else
                    oResult.Result = ConceptResultEnum.SqlError
                End If
            Catch ex As Exception
                oResult.Result = ConceptResultEnum.SqlError
            End Try

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(oResult, newGState)

            resp.Value = True
            resp.Status = newGState

        End If

        Return resp
    End Function

    ''' <summary>
    ''' Eliminar regla de saldos (No implementada)
    ''' </summary>
    ''' <param name="IDAccrualsRule">ID de saldo a eliminar</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteAccrualsRule(ByVal IDAccrualsRule As Integer) As roGenericVtResponse(Of Boolean)
        ' Esta funcion queda temporalmente suspendida ya que se tiene que
        ' revisar todo el tema de AccrualsRules
        Return Nothing
    End Function

    ''' <summary>
    '''  Devuelve los datos de un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">ID de grupo de saldos</param>
    ''' <returns>Devuelve un grupo de saldo (roReportGroup)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetReportGroup(ByVal IDReportGroup As Integer) As roGenericVtResponse(Of roReportGroup)

        Dim oResult As New roGenericVtResponse(Of roReportGroup)

        Dim oReportGroup As New roReportGroup
        Dim strQuery As String
        Dim oDataset As DataSet
        Dim oDatareader As Common.DbDataReader

        strQuery = " @SELECT# * From SysRoReportGroups Where ID = " & IDReportGroup

        oDataset = CreateDataSet(strQuery)

        If oDataset IsNot Nothing Then
            oDatareader = oDataset.CreateDataReader(oDataset.Tables(0))

            If oDatareader.HasRows Then
                ' Cargo los datos en la clase
                oReportGroup.ID = oDatareader("ID")
                oReportGroup.Name = oDatareader("Name")

                oDatareader.Close()

                ' Cargo los saldos del grupo en la clase
                strQuery = "@SELECT# * from SysRoReportGroupConcepts Where ID = " & IDReportGroup
                strQuery = strQuery & " Order By Position "
                oDataset = CreateDataSet(strQuery)

                If oDataset IsNot Nothing Then
                    oDatareader = oDataset.CreateDataReader(oDataset.Tables(0))

                    Do While oDatareader.Read
                        Dim oReportGroupConcept As New roReportGroupsConcepts

                        oReportGroupConcept.IDReportGroup = oDatareader("IDReportGroup")
                        oReportGroupConcept.IDConcept = oDatareader("IDConcept")
                        oReportGroupConcept.Position = oDatareader("Position")

                        oReportGroup.Concepts.Add(oReportGroupConcept)
                    Loop
                End If
            End If
        Else
            oReportGroup = Nothing
        End If

        oResult.Value = oReportGroup

        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una lista de los grupos de saldos existentes
    ''' </summary>
    ''' <returns>Devuelve un DataSet (sysroReportGroups) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetReportGroups(ByVal filterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim tb As DataTable = roConceptGroup.GetConceptGroups(bState, filterBusinessGroups)
        Dim ds As DataSet = Nothing

        If oState.Result = ConceptResultEnum.NoError Then
            ds = New DataSet
            ds.Tables.Add(tb)
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una lista de los saldos que componen un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">ID de grupo de saldo</param>
    ''' <returns>DataSet (sysroReportGroupConcepts, Concepts) ordenado por Position</returns>
    ''' <remarks></remarks>

    Public Shared Function GetReportGroupsConcepts(ByVal IDReportGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim tb As DataTable = roConceptGroup.GetConceptsforGroupDatatable(IDReportGroup, bState)
        Dim ds As DataSet = Nothing

        If oState.Result = ConceptResultEnum.NoError Then
            ds = New DataSet
            ds.Tables.Add(tb)
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Guarda un grupo de saldos
    ''' </summary>
    ''' <param name="oReportGroup">Grupo de saldo a guardar (roReportGroup)</param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Shared Function SaveReportGroup(ByVal oReportGroup As roReportGroup) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roConceptState
        Dim strQuery As String
        Dim intIDreportGroup As Integer
        Dim res = New roGenericVtResponse(Of Boolean)

        If oReportGroup.ID = -1 Then
            ' ID = -1 entonces INSERT
            strQuery = " @INSERT# INTO sysroReportGroups "
            strQuery = strQuery & " ( Name ) "
            strQuery = strQuery & " Values "
            strQuery = strQuery & " ( '" & oReportGroup.Name & "' ) "

            Try
                If ExecuteSql(strQuery) Then
                    oResult.Result = ConceptResultEnum.NoError
                    oReportGroup.ID = intIDreportGroup
                Else
                    oResult.Result = ConceptResultEnum.SqlError
                End If
            Catch ex As Exception
                oResult.Result = ConceptResultEnum.SqlError
            End Try
        Else
            ' ID > 0 entonces UPDATE
            strQuery = " @UPDATE# sysroReportGroups "
            strQuery = strQuery & " Set  Name  = '" & oReportGroup.Name & "' "
            strQuery = strQuery & " where ID =" & oReportGroup.ID

            Try
                If ExecuteSql(strQuery) Then
                    oResult.Result = ConceptResultEnum.NoError
                Else
                    oResult.Result = ConceptResultEnum.SqlError
                End If
            Catch ex As Exception
                oResult.Result = ConceptResultEnum.SqlError
            End Try

        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult, newGState)

        res.Value = True
        res.Status = newGState

        Return res
    End Function

    ''' <summary>
    ''' Guarda los saldos pertenecientes a un Grupo de saldos
    ''' </summary>
    ''' <param name="oConcepts"></param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Shared Function AddReportGroupConcepts(ByVal oConcepts As ArrayList) As roGenericVtResponse(Of Boolean)
        '
        Dim oResult As New roConceptState
        Dim strQuery As String
        Dim oDataset As DataSet
        Dim oDataReader As Common.DbDataReader
        Dim oReportGroupConcept As roReportGroupsConcepts
        Dim res As New roGenericVtResponse(Of Boolean)

        For Each oReportGroupConcept In oConcepts
            ' Miro que el saldo no exista ya en la tabla
            strQuery = " @SELECT# * from sysroReportGroupConcepts "
            strQuery = strQuery & " Where IDReportGroup = " & oReportGroupConcept.IDReportGroup
            strQuery = strQuery & " and IDConcept = " & oReportGroupConcept.IDConcept

            oDataset = CreateDataSet(strQuery)
            If oDataset.Tables(0).Rows.Count = 0 Then
                strQuery = " @INSERT# INTO sysroReportGroupConcepts "
                strQuery = strQuery & " (IDReportGroup, IDConcept, Position) "
                strQuery = strQuery & " Values "
                strQuery = strQuery & " ( " & oReportGroupConcept.IDReportGroup
                strQuery = strQuery & " , " & oReportGroupConcept.IDConcept
                strQuery = strQuery & " , " & oReportGroupConcept.Position & ") "

                Try
                    If ExecuteSql(strQuery) Then
                        oResult.Result = ConceptResultEnum.NoError
                    Else
                        oResult.Result = ConceptResultEnum.SqlError
                    End If
                Catch ex As Exception
                    oResult.Result = ConceptResultEnum.SqlError
                End Try
            Else
                oDataReader = oDataset.CreateDataReader(oDataset.Tables(0))
                If oDataReader("Position") <> oReportGroupConcept.Position Then
                    strQuery = " @UPDATE# sysroReportGroupConcepts "
                    strQuery = strQuery & " Set Position = " & oReportGroupConcept.Position
                    strQuery = strQuery & " Where IDReportGroup = " & oReportGroupConcept.IDReportGroup
                    strQuery = strQuery & " and IDConcept = " & oReportGroupConcept.IDConcept

                    Try
                        If ExecuteSql(strQuery) Then
                            oResult.Result = ConceptResultEnum.NoError
                        Else
                            oResult.Result = ConceptResultEnum.SqlError
                        End If
                    Catch ex As Exception
                        oResult.Result = ConceptResultEnum.SqlError
                    End Try

                End If
            End If
        Next

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult, newGState)

        res.Value = IIf(oResult.Result = ConceptResultEnum.NoError, True, False)
        res.Status = newGState

        Return res

    End Function

    ''' <summary>
    ''' Elimina un grupo de saldos
    ''' </summary>
    ''' <param name="IDREportGroup"></param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteReportGroup(ByVal IDREportGroup As Integer) As roGenericVtResponse(Of Boolean)
        ' Borra un grupo de saldos
        Dim strQuery As String
        Dim res As roGenericVtResponse(Of Boolean)
        Dim oResult As New roConceptState

        res = DeleteReportGroupsConcepts(IDREportGroup)

        strQuery = "@DELETE# FROM sysroReportGroups"
        strQuery = strQuery & " Where ID = " & IDREportGroup

        Try
            If ExecuteSql(strQuery) Then
                oResult.Result = ConceptResultEnum.NoError
            Else
                oResult.Result = ConceptResultEnum.SqlError
            End If
        Catch ex As Exception
            oResult.Result = ConceptResultEnum.Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult, newGState)

        res.Value = IIf(oResult.Result = ConceptResultEnum.NoError, True, False)
        res.Status = newGState

        Return res

    End Function

    ''' <summary>
    ''' Borra los saldos de un grupo de saldos
    ''' </summary>
    ''' <param name="IDReportGroup">Id del grupo de saldos</param>
    ''' <returns>Devuelve oState.ResultEnum = NoError si se ha grabado o el error correspondiente</returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteReportGroupsConcepts(ByVal IDReportGroup As Integer) As roGenericVtResponse(Of Boolean)
        ' Borra los saldos de un grupo de saldos
        Dim strQuery As String
        Dim res As New roGenericVtResponse(Of Boolean)
        Dim oResult As New roConceptState

        strQuery = "@DELETE# FROM sysroReportGroupConcepts "
        strQuery = strQuery & " Where IDReportGroup = " & IDReportGroup

        Try
            If ExecuteSql(strQuery) Then
                oResult.Result = ConceptResultEnum.NoError
            Else
                oResult.Result = ConceptResultEnum.SqlError
            End If
        Catch ex As Exception
            oResult.Result = ConceptResultEnum.Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult, newGState)

        res.Value = IIf(oResult.Result = ConceptResultEnum.NoError, True, False)
        res.Status = newGState

        Return res
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

    Public Shared Function SetReportGroupConceptPosition(ByVal IDReportGroup As Integer, ByVal IDConcept As Integer, ByVal bolUp As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oReportGroup As New roConceptGroup(IDReportGroup, bState)

        oResult.Value = oReportGroup.SetConceptPosition(IDConcept, bolUp)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los valores de los saldos de vacaciones para un empleado
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los valores de los saldos de vacaciones</returns>
    ''' <remarks></remarks>
    Public Shared Function GetHolidaysConceptsSummaryByEmployee(ByVal oState As roWsState, ByVal idEmployee As Integer, ByVal idShift As Integer) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetHolidaysConceptsSummaryByEmployee(bState, idEmployee, idShift)

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
    ''' Devuelve un dataset con los movimientos en los saldos de vacaciones
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con los movimientos de los saldos de vacaciones</returns>
    ''' <remarks></remarks>
    Public Shared Function GetHolidaysConceptsDetailByEmployee(ByVal oState As roWsState, ByVal iSelectedShiftID As Integer, ByVal idEmployee As Integer) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roConceptState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roConcept.GetHolidaysConceptsDetailByEmployee(bState, iSelectedShiftID, idEmployee)

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

End Class