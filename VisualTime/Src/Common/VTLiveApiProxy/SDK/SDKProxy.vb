Imports Robotics.Base.VTSDK
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.DTOs

''' <summary>
''' Servicio web para la consulta e inserción de fichajes de VisualTime
''' </summary>
Public Class SDKProxy
    Implements ISDKsvc

    Public Function KeepAlive() As Boolean Implements ISDKsvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Se requiere haber iniciado sesión con un usuario de visualtime válido
    ''' Esta función nos dará un listado de fichajes cumpliendo las condiciones especificadas en el parametro WHERE
    ''' Esta funcionalidad requiere de la licencia de SDK de VisualTime
    ''' </summary>
    ''' <param name="strWhere">Cadena en formato SQL que especifica las condiciones en que queremos obtener los fichajes. P.E. (Date ='20170101')</param>
    ''' <param name="bAudit">Indica si se debe auditar la consulta en el entorno de Visualtime</param>
    ''' <param name="oState">Variable en la que indicamos la identidad del usuario que esta realizando la petición y la localización desde donde se utiliza</param>
    ''' <returns>Obtenemos un objeto el listado de fichajes y un objecto roWsState con el estado de la operación.</returns>
    Function GetPunchesWhere(ByVal strWhere As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roSDKPunchList Implements ISDKsvc.GetPunches
        Return SDKMethods.GetPunchesWhere(strWhere, bAudit, oState)
    End Function

    ''' <summary>
    ''' Se requiere haber iniciado sesión con un usuario de visualtime válido
    ''' Esta función insertará el listado de fichajes indicado en VisualTime. También la podemos utilizar para borra fichajes indicando los IDs(estos los podemos consultar en la primera función)
    ''' Esta funcionalidad requiere de la licencia de SDK de VisualTime
    ''' </summary>
    ''' <param name="punches">Lista de fichajes que se van a insertar en visualtime</param>
    ''' <param name="DeletePunchesIDs">ID's de los fichajes que se deben borrar separados por comas</param>
    ''' <param name="bAudit">Indica si se debe auditar la consulta en el entorno de Visualtime</param>
    ''' <param name="oState">Variable en la que indicamos la identidad del usuario que esta realizando la petición y la localización desde donde se utiliza</param>
    ''' <returns>Obtenemos un objeto roWsState con el estado de la operación.</returns>
    Function ProcessPunches(ByVal punches As List(Of roSDKPunch), ByVal DeletePunchesIDs As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roSDKGenericResponse Implements ISDKsvc.ProcessPunches
        Return SDKMethods.ProcessPunches(punches, DeletePunchesIDs, bAudit, oState)
    End Function


End Class