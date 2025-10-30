Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Incidence

Public Class IncidenceMethods

    ''' <summary>
    ''' Recupera lista de incidencias
    ''' </summary>
    ''' <param name="strWhere">Parametro SQL Where</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (sysroDailyIncidencesTypes)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetIncidences(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roIncidenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim oIncidences As New roIncidenceList(bState)
        Dim ds As New DataSet
        ds.Tables.Add(oIncidences.GetIncidences(strWhere))

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera lista de incidencias
    ''' </summary>
    ''' <param name="strWhere">Parametro SQL Where</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (sysroDailyIncidencesDescription)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetIncidencesDescription(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roIncidenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim oIncidences As New roIncidenceList(bState)
        Dim ds As New DataSet
        ds.Tables.Add(oIncidences.GetIncidencesDescription(strWhere))

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class