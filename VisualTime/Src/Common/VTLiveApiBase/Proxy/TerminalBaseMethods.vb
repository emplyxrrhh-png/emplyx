Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.ExternalSystems.Suprema
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class TerminalBaseMethods

    Public Shared Function GetTerminalsListStatus(ByVal strWhere As String, ByVal liteCharge As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim dt As DataTable = roTerminalList.LoadStatusData(strWhere, liteCharge, bState)
        If dt IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(dt)
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTerminals(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of roTerminalList)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTerminalList)
        Dim oTerminals As New roTerminalList(bState)
        oTerminals.LoadData(strWhere, bState)
        oResult.Value = oTerminals

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTerminalsDataSet(ByVal strWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        ds.Tables.Add(roTerminalList.GetTerminalsLive(strWhere, bState))
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTerminals(ByVal dsTerminals As DataSet, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False
        If dsTerminals.Tables.Contains("Terminals") Then
            Dim oTerminals As New roTerminalList(bState)
            bolRet = oTerminals.Save(dsTerminals.Tables("Terminals"), bAudit)
        End If
        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTerminal(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTerminal)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTerminal)
        oResult.Value = New roTerminal(intID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTerminalName(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = roTerminal.GetTerminalName(intID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveNFCReader(ByVal terminalID As Integer, ByVal readerid As Integer, ByVal idzone As Integer, ByVal nfc As String, ByVal idmode As Integer, ByVal description As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roTerminal.SaveNFCReader(terminalID, readerid, idzone, nfc, idmode, description, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (roTerminal, Boolean))
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (roTerminal, Boolean))

        oTerminal.State = bState
        Dim bRes As Boolean = oTerminal.Save(bAudit)

        oResult.Value = (oTerminal, bRes)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function RegisterMxcTerminal(ByVal intID As Integer, ByVal strRegistrationCode As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oTerminal As New roTerminal(intID, bState, bAudit)
        Dim bSaved = oTerminal.SaveTerminalResgistrationCode(strRegistrationCode, bAudit)

        oResult.Value = bSaved

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda la configuración de una sirena
    ''' </summary>
    ''' <param name="oTerminalSiren"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SaveTerminalSiren(ByVal oTerminalSiren As roTerminalSiren, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oTerminalSiren.State = bState
        Dim bSaved = oTerminalSiren.Save(bAudit)

        oResult.Value = bSaved

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda la configuración de un lector
    ''' </summary>
    ''' <param name="oReader">Lector a guardar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SaveTerminalReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oReader.State = bState
        Dim bSaved = oReader.Save(, bAudit)

        oResult.Value = bSaved

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borra un terminal
    ''' </summary>
    ''' <param name="oTerminal">Temrinal a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oTerminal.State = bState
        Dim bDelete = oTerminal.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borra un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminalById(ByVal intIDTerminal As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oTerminal As New roTerminal(intIDTerminal, bState, False)
        Dim bDelete = oTerminal.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borra una sirena de un terminal
    ''' </summary>
    ''' <param name="oTerminalSiren">Sirena a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminalSiren(ByVal oTerminalSiren As roTerminalSiren, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oTerminalSiren.State = bState
        Dim bDelete = oTerminalSiren.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borra una sirena de un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal</param>
    ''' <param name="intID">Código de la sirena a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminalSirenById(ByVal intIDTerminal As Integer, ByVal intID As Integer, ByVal oState As roWsState,
                                            ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oSiren As New roTerminalSiren(intIDTerminal, intID, bState, False)
        Dim bDelete = oSiren.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borrar un lector de un terminal
    ''' </summary>
    ''' <param name="oReader">Lector a borrar</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminalReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState,
                                         ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oReader.State = bState
        Dim bDelete = oReader.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Borrar un lector de un terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código de terminal</param>
    ''' <param name="intID">Código de lector</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteTerminalReaderById(ByVal intIDTerminal As Integer, ByVal intID As Integer, ByVal oState As roWsState,
                                             ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oReader As New roTerminal.roTerminalReader(intIDTerminal, intID, bState, False)
        Dim bDelete = oReader.Delete(bAudit)

        oResult.Value = bDelete

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprobación Núm. de Serie del Terminal
    ''' </summary>
    ''' <param name="Serial">Núm. de Serie VisualTime</param>
    ''' <param name="SNTerminal">Núm. de Serie del Terminal</param>
    ''' <returns>True si el núm. de serie es correcto</returns>
    ''' <remarks></remarks>

    Public Shared Function CheckTerminalSerialNum(ByVal Serial As String, ByVal SNTerminal As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oTerminal As New roTerminal(bState)
        Dim bolRes = oTerminal.CheckTerminalSerialNum(Serial, SNTerminal)

        oResult.Value = bolRes

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Añade un TerminalReader a un Terminal
    ''' </summary>
    ''' <param name="oTerminal">roTerminal al que añadir el roTerminalReader</param>
    ''' <param name="oTerminalReader">TerminalReader a crear / actualizar</param>
    ''' <param name="oState">Control de errores</param>
    ''' <returns>Devuelve TRUE si consigue añadirlo / actualizarlo</returns>
    ''' <remarks></remarks>

    Public Shared Function AddTerminalReader(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean))
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean))

        oTerminal.State = bState
        oTerminalReader.State = bState
        Dim bRes As Boolean = oTerminal.AddReader(oTerminalReader)

        oResult.Value = (oTerminal, oTerminalReader, bRes)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina un TerminalReader de un Terminal
    ''' </summary>
    ''' <param name="oTerminal">roTerminal al que eliminar el roTerminalReader</param>
    ''' <param name="oTerminalReader">TerminalReader a eliminar</param>
    ''' <param name="ostate">Control de errores</param>
    ''' <returns>Devuelve TRUE si consigue eliminarlo</returns>
    ''' <remarks></remarks>

    Public Shared Function RemoveTerminalReader(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean))
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean))

        oTerminal.State = bState
        oTerminalReader.State = bState
        Dim bRes As Boolean = oTerminal.RemoveReader(oTerminalReader)

        oResult.Value = (oTerminal, oTerminalReader, bRes)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera el siguiente número de Id. de Terminal
    ''' </summary>
    ''' <param name="oState">Control de Errores</param>
    ''' <returns>Devuelve el siguiente Id. del Terminal. En caso de error, devuelve -1</returns>
    ''' <remarks></remarks>

    Public Shared Function RetrieveTerminalNextID(ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roTerminal.RetrieveTerminalNextID(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera el siguiente número de Id. de Reader del Terminal especificado
    ''' </summary>
    ''' <param name="IDTerminal">Id del Terminal a recuperar el siguiente Reader</param>
    ''' <param name="oState">Control de Errores</param>
    ''' <returns>Devuelve el siguiente Id. del Reader. En caso de error, devuelve -1</returns>
    ''' <remarks></remarks>

    Public Shared Function RetrieveTerminalReaderNextID(ByVal IDTerminal As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roTerminal.RetrieveTerminalReaderNextID(IDTerminal, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function ValidateTerminal(ByVal oTerminal As roTerminal, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oTerminal.State = bState
        oResult.Value = oTerminal.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function ValidateReader(ByVal oReader As roTerminal.roTerminalReader, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oReader.State = bState
        oResult.Value = oReader.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTerminalReadersTemplates(ByVal _Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTerminal.roTerminalReader.GetTemplates(_Type, bState)
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
    ''' Devuelve los diferentes tipos de terminal
    ''' </summary>
    ''' <param name="_TypeDirection">'Local' o 'Remote'. En blanco devuelve ambos</param>
    ''' <param name="oState"></param>
    ''' <returns>Un datatable con los campos Type y Direction</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTerminalTypes(ByVal _TypeDirection As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTerminal.GetTerminalTypes(_TypeDirection, bState)
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
    ''' Devuelve la fecha/hora del servidor aplicando la diferencia horaria que tenga configurada el terminal
    ''' </summary>
    ''' <param name="intIDTerminal">Código del terminal</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetCurrentDateTime(ByVal intIDTerminal As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DateTime)
        Dim oTerminal As New roTerminal(intIDTerminal, bState)
        oResult.Value = oTerminal.GetCurrentDateTime

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una lista con la definición de las zonas horarias que se pueden configurar a un terminal.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetTimeZones(ByVal _TerminalType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTerminalTimeZone))

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roTerminalTimeZone))
        oResult.Value = roTerminalTimeZone.GetTerminalTimeZones(_TerminalType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetEmployeeTerminals(ByVal _IDEmployee As Integer, ByVal _TerminalType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTerminal))

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roTerminal))
        oResult.Value = roTerminal.GetEmployeeTerminals(_IDEmployee, _TerminalType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function RegisterTerminalOnMT(strCompanyName As String, strSerialNumber As String, strTerminalType As String, oState As roWsState) As roGenericVtResponse(Of String)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        Dim oConf As New Robotics.Azure.roAzureTerminalRepository
        oResult.Value = oConf.AddTerminalToCompany(strSerialNumber, strCompanyName, strTerminalType)

        If oResult.Value.ToUpper <> "OK" Then
            bState.Result = TerminalBaseResultEnum.Exception
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function ImportVTC(strFileContents As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try

            Dim tmpRes As Boolean = True
            Dim oSupport As New Robotics.VTBase.roSupport
            Dim actualLine As Integer = 1
            Dim lineContent As String = oSupport.INIRead("remote", "Data", "1",,, strFileContents)

            While lineContent <> String.Empty AndAlso tmpRes
                Dim cardDesc As String() = lineContent.Split(",")

                Dim strSQL = "merge CardAliases with(HOLDLOCK) as target " &
                                " using (values ('" & cardDesc(1) & "')) " &
                                " as source (RealValue) " &
                                " on target.IDCard = " & cardDesc(0) & " " &
                                " when matched then " &
                                " @UPDATE# " &
                                " set target.Realvalue = source.RealValue" &
                                " when not matched then " &
                                " @INSERT# (IDCard,RealValue) " &
                                " values (" & cardDesc(0) & ",source.RealValue);"

                tmpRes = Robotics.DataLayer.AccessHelper.ExecuteSql(strSQL)

                actualLine += 1
                lineContent = oSupport.INIRead("remote", "Data", actualLine.ToString,,, strFileContents)
            End While

            oResult.Value = True
        Catch ex As Exception
            oResult.Value = False
            bState.Result = TerminalBaseResultEnum.SqlError
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveMx9Parameter(idTerminal As Integer, strParameterName As String, strParameterValue As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try

            Dim strSQL As String = "@INSERT# [dbo].[TerminalsSyncTasks] ([IDTerminal], [Task], [IDEmployee], [IDFinger], [DeleteOnConfirm], [TaskDate], [Parameter1], [Parameter2], [TaskData], [TaskSent], [TaskRetries]) " &
                                        "VALUES (" & idTerminal.ToString & ", N'setterminalconfig', 0, 0, 0, CAST(N'2019-11-28T13:16:31.337' AS DateTime), 0, 0, N'<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd""> " &
                                        "<TerminalConfig> " &
                                        "<Name>mx9DB:" & strParameterName & "</Name> " &
                                        "<Value>" & strParameterValue & "</Value> " &
                                        "</TerminalConfig> " &
                                        "</LocalDataSet>', NULL, NULL) "

            Dim oTerminal As New roTerminal(idTerminal, New roTerminalState())

            If oTerminal.Type.ToLower = "mx9" Then
                oResult.Value = Robotics.DataLayer.AccessHelper.ExecuteSql(strSQL)
            Else
                oResult.Value = False
                bState.Result = TerminalBaseResultEnum.TerminalDoesNotExists
            End If
        Catch ex As Exception
            oResult.Value = False
            bState.Result = TerminalBaseResultEnum.SqlError
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetSupremaConfiguration(ByVal oState As roWsState) As roGenericVtResponse(Of SupremaConfigurationParameters)

        Dim bState = New roTerminalState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of SupremaConfigurationParameters)

        Dim supremaSystem As SupremaSystem = New SupremaSystem
        supremaSystem.LoadConfigurationParameters()
        oResult.Value = supremaSystem.ConfigurationParameters(True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveSupremaConfiguration(ByVal supremaConfiguration As SupremaConfigurationParameters, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roTerminalState(-1)
        Dim bolRet As Boolean = True
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim advancedParameter As roAdvancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.URL", New AdvancedParameter.roAdvancedParameterState(-1))
        advancedParameter.Value = supremaConfiguration.URL
        bolRet = advancedParameter.Save()

        advancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.UserName", New AdvancedParameter.roAdvancedParameterState(-1))
        advancedParameter.Value = supremaConfiguration.Username
        bolRet = bolRet AndAlso advancedParameter.Save()

        If supremaConfiguration.Password.Trim.Length > 0 Then
            advancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.Password", New AdvancedParameter.roAdvancedParameterState(-1))
            advancedParameter.Value = supremaConfiguration.Password
            bolRet = bolRet AndAlso advancedParameter.Save()
        End If

        advancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.EmployeeField", New AdvancedParameter.roAdvancedParameterState(-1))
        advancedParameter.Value = supremaConfiguration.EmployeeUserfieldId
        bolRet = bolRet AndAlso advancedParameter.Save()

        advancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.InitialLinkDate", New AdvancedParameter.roAdvancedParameterState(-1))
        advancedParameter.Value = supremaConfiguration.StartDate
        bolRet = bolRet AndAlso advancedParameter.Save()

        Dim enabled As Boolean = (supremaConfiguration.URL.Trim.Length > 0)
        advancedParameter = New roAdvancedParameter("VisualTime.Link.Suprema.Enabled", New AdvancedParameter.roAdvancedParameterState(-1))
        advancedParameter.Value = enabled.ToString.ToLower
        bolRet = bolRet AndAlso advancedParameter.Save()

        ' Marcamos actualización para que se recargue la caché (TODO control de caché)
        roParameters.SaveDateParameter("SupremaLastCacheUpdate", Now)

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class