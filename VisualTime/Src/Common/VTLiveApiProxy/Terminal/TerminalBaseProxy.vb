Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Terminal

Public Class TerminalBaseProxy
    Implements ITerminalBaseSvc

    Public Function KeepAlive() As Boolean Implements ITerminalBaseSvc.KeepAlive
        Return True
    End Function


    Public Function GetTerminalsListStatus(ByVal strWhere As String, ByVal liteCharge As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITerminalBaseSvc.GetTerminalsListStatus

        Return TerminalBaseMethods.GetTerminalsListStatus(strWhere, liteCharge, oState)
    End Function


    Public Function GetTerminals(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of roTerminalList) Implements ITerminalBaseSvc.GetTerminals
        Return TerminalBaseMethods.GetTerminals(strWhere, oState)
    End Function


    Public Function GetTerminalsDataSet(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITerminalBaseSvc.GetTerminalsDataSet
        Return TerminalBaseMethods.GetTerminalsDataSet(strWhere, oState)
    End Function


    Public Function SaveTerminals(ByVal dsTerminals As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.SaveTerminals
        Return TerminalBaseMethods.SaveTerminals(dsTerminals, oState, bAudit)
    End Function


    Public Function GetTerminal(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTerminal) Implements ITerminalBaseSvc.GetTerminal
        Return TerminalBaseMethods.GetTerminal(intID, oState, bAudit)
    End Function


    Public Function GetTerminalName(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ITerminalBaseSvc.GetTerminalName

        Return TerminalBaseMethods.GetTerminalName(intID, oState)
    End Function

    Public Function SaveNFCReader(ByVal terminalID As Integer, ByVal readerid As Integer, ByVal idzone As Integer, ByVal nfc As String, ByVal idmode As Integer, ByVal description As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.SaveNFCReader
        Return TerminalBaseMethods.SaveNFCReader(terminalID, readerid, idzone, nfc, idmode, description, oState, bAudit)
    End Function

    Public Function SaveTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (roTerminal, Boolean)) Implements ITerminalBaseSvc.SaveTerminal
        Return TerminalBaseMethods.SaveTerminal(oTerminal, oState, bAudit)
    End Function


    Public Function RegisterMxcTerminal(ByVal intID As Integer, ByVal strRegistrationCode As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.RegisterMxcTerminal
        Return TerminalBaseMethods.RegisterMxcTerminal(intID, strRegistrationCode, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda la configuración de una sirena
    ''' </summary>
    ''' <param name="oTerminalSiren"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SaveTerminalSiren(ByVal oTerminalSiren As roTerminalSiren, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.SaveTerminalSiren
        Return TerminalBaseMethods.SaveTerminalSiren(oTerminalSiren, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda la configuración de un lector
    ''' </summary>
    ''' <param name="oReader">Lector a guardar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SaveTerminalReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.SaveTerminalReader
        Return TerminalBaseMethods.SaveTerminalReader(oReader, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borra un terminal
    ''' </summary>
    ''' <param name="oTerminal">Temrinal a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminal
        Return TerminalBaseMethods.DeleteTerminal(oTerminal, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borra un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminalById(ByVal intIDTerminal As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminalById
        Return TerminalBaseMethods.DeleteTerminalById(intIDTerminal, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borra una sirena de un terminal
    ''' </summary>
    ''' <param name="oTerminalSiren">Sirena a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminalSiren(ByVal oTerminalSiren As roTerminalSiren, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminalSiren
        Return TerminalBaseMethods.DeleteTerminalSiren(oTerminalSiren, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borra una sirena de un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal</param>
    ''' <param name="intID">Código de la sirena a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminalSirenById(ByVal intIDTerminal As Integer, ByVal intID As Integer, ByVal oState As roWsState,
                                            ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminalSirenById

        Return TerminalBaseMethods.DeleteTerminalSirenById(intIDTerminal, intID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borrar un lector de un terminal
    ''' </summary>
    ''' <param name="oReader">Lector a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminalReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState,
                                         ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminalReader

        Return TerminalBaseMethods.DeleteTerminalReader(oReader, oState, bAudit)
    End Function

    ''' <summary>
    ''' Borrar un lector de un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código de terminal</param>
    ''' <param name="intID">Código de lector</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteTerminalReaderById(ByVal intIDTerminal As Integer, ByVal intID As Integer, ByVal oState As roWsState,
                                             ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.DeleteTerminalReaderById

        Return TerminalBaseMethods.DeleteTerminalReaderById(intIDTerminal, intID, oState, bAudit)

    End Function

    ''' <summary>
    ''' Comprobación Núm. de Serie del Terminal
    ''' </summary>
    ''' <param name="Serial">Núm. de Serie VisualTime</param>
    ''' <param name="SNTerminal">Núm. de Serie del Terminal</param>
    ''' <returns>True si el núm. de serie es correcto</returns>
    ''' <remarks></remarks>

    Public Function CheckTerminalSerialNum(ByVal Serial As String, ByVal SNTerminal As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.CheckTerminalSerialNum
        Return TerminalBaseMethods.CheckTerminalSerialNum(Serial, SNTerminal, oState)
    End Function

    ''' <summary>
    ''' Añade un TerminalReader a un Terminal
    ''' </summary>
    ''' <param name="oTerminal">roTerminal al que añadir el roTerminalReader</param>
    ''' <param name="oTerminalReader">TerminalReader a crear / actualizar</param>
    ''' <param name="oState">Control de errores</param>
    ''' <returns>Devuelve TRUE si consigue añadirlo / actualizarlo</returns>
    ''' <remarks></remarks>

    Public Function AddTerminalReader(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean)) Implements ITerminalBaseSvc.AddTerminalReader
        Return TerminalBaseMethods.AddTerminalReader(oTerminal, oTerminalReader, oState)
    End Function

    ''' <summary>
    ''' Elimina un TerminalReader de un Terminal
    ''' </summary>
    ''' <param name="oTerminal">roTerminal al que eliminar el roTerminalReader</param>
    ''' <param name="oTerminalReader">TerminalReader a eliminar</param>
    ''' <param name="ostate">Control de errores</param>
    ''' <returns>Devuelve TRUE si consigue eliminarlo</returns>
    ''' <remarks></remarks>

    Public Function RemoveTerminalReader(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean)) Implements ITerminalBaseSvc.RemoveTerminalReader
        Return TerminalBaseMethods.RemoveTerminalReader(oTerminal, oTerminalReader, oState)
    End Function

    ''' <summary>
    ''' Recupera el siguiente número de Id. de Terminal
    ''' </summary>
    ''' <param name="oState">Control de Errores</param>
    ''' <returns>Devuelve el siguiente Id. del Terminal. En caso de error, devuelve -1</returns>
    ''' <remarks></remarks>

    Public Function RetrieveTerminalNextID(ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ITerminalBaseSvc.RetrieveTerminalNextID
        Return TerminalBaseMethods.RetrieveTerminalNextID(oState)
    End Function

    ''' <summary>
    ''' Recupera el siguiente número de Id. de Reader del Terminal especificado
    ''' </summary>
    ''' <param name="IDTerminal">Id del Terminal a recuperar el siguiente Reader</param>
    ''' <param name="oState">Control de Errores</param>
    ''' <returns>Devuelve el siguiente Id. del Reader. En caso de error, devuelve -1</returns>
    ''' <remarks></remarks>

    Public Function RetrieveTerminalReaderNextID(ByVal IDTerminal As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ITerminalBaseSvc.RetrieveTerminalReaderNextID
        Return TerminalBaseMethods.RetrieveTerminalReaderNextID(IDTerminal, oState)

    End Function


    Public Function ValidateTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.ValidateTerminal
        Return TerminalBaseMethods.ValidateTerminal(oTerminal, oState)
    End Function


    Public Function ValidateReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITerminalBaseSvc.ValidateReader
        Return TerminalBaseMethods.ValidateReader(oReader, oState)
    End Function


    Public Function GetTerminalReadersTemplates(ByVal _Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITerminalBaseSvc.GetTerminalReadersTemplates
        Return TerminalBaseMethods.GetTerminalReadersTemplates(_Type, oState)
    End Function

    ''' <summary>
    ''' Devuelve los diferentes tipos de terminal
    ''' </summary>
    ''' <param name="_TypeDirection">'Local' o 'Remote'. En blanco devuelve ambos</param>
    ''' <param name="oState"></param>
    ''' <returns>Un datatable con los campos Type y Direction</returns>
    ''' <remarks></remarks>

    Public Function GetTerminalTypes(ByVal _TypeDirection As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITerminalBaseSvc.GetTerminalTypes
        Return TerminalBaseMethods.GetTerminalTypes(_TypeDirection, oState)
    End Function

    ''' <summary>
    ''' Devuelve la fecha/hora del servidor aplicando la diferencia horaria que tenga configurada el terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetCurrentDateTime(ByVal intIDTerminal As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime) Implements ITerminalBaseSvc.GetCurrentDateTime

        Return TerminalBaseMethods.GetCurrentDateTime(intIDTerminal, oState)

    End Function

    ''' <summary>
    ''' Devuelve una lista con la definición de las zonas horarias que se pueden configurar a un terminal.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetTimeZones(ByVal _TerminalType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTerminalTimeZone)) Implements ITerminalBaseSvc.GetTimeZones

        Return TerminalBaseMethods.GetTimeZones(_TerminalType, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de terminales de un tipo por los que puede fichar un empleado. <br></br>
    ''' Tiene en cuenta la configuración de limitación de empleados.<br></br>
    ''' No tiene en cuenta la configuración de accesos.
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado</param>
    ''' <param name="_TerminalType">Tipo de terminal</param>
    ''' <param name="oState"></param>        
    ''' <returns>Lista de terminales por los que puede fichar</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeTerminals(ByVal _IDEmployee As Integer, ByVal _TerminalType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTerminal)) Implements ITerminalBaseSvc.GetEmployeeTerminals
        Return TerminalBaseMethods.GetEmployeeTerminals(_IDEmployee, _TerminalType, oState)
    End Function
End Class
