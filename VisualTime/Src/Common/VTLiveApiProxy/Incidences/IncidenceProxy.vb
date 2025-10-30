Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Incidence

Public Class IncidenceProxy
    Implements IIncidenceSvc

    Public Function KeepAlive() As Boolean Implements IIncidenceSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Recupera lista de incidencias
    ''' </summary>
    ''' <param name="strWhere">Parametro SQL Where</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (sysroDailyIncidencesTypes)</returns>
    ''' <remarks></remarks>
    Public Function GetIncidences(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IIncidenceSvc.GetIncidences
        Return IncidenceMethods.GetIncidences(strWhere, oState)
    End Function


End Class
