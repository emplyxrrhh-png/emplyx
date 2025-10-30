Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink.DataLink

Public Class DataLinkBaseMethods

    ''' <summary>
    ''' Devuelve un dataset con una guia de importacion concreta
    ''' </summary>
    ''' <param name="_ID"></param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetImportGuide(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roImportGuide)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roImportGuide)
        oResult.Value = New roImportGuide(_ID, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todas las guias de importación
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetImports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roGuide.GetImports(bState)
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
    ''' Devuelve un dataset con todas las guias de exportación
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetExports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roGuide.GetExports(bState)
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
    ''' Obtiene la guia de extraccion  con el ID indicado
    ''' </summary>
    ''' <param name="IDGuide"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetGuideByID(ByVal IDGuide As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGuide)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGuide)
        oResult.Value = New roGuide(IDGuide, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una lista generica de cadenas con todas las plantillas Excel de exportacion de datos existentes en la carpeta configurada
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>ArrayList of elements with format: [NameToShow]#[TemplateFilePath]</returns>
    ''' <remarks></remarks>
    Public Shared Function GetTemplatesExcel(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String))

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of String))
        Dim oDataLink As New roDataLink(bState)
        Dim myList As Generic.List(Of String) = oDataLink.GetTemplatesExcel()

        oResult.Value = myList

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba si ya se ha exportado el periodo indicado
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>Retorna Boolean indicando si el periodo ya se ha exportado</returns>
    ''' <remarks></remarks>
    Public Shared Function ExistsExportPeriod(ByVal BeginPeriod As Date, ByVal EndPeriod As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roDataLink.ExistsExportPeriod(BeginPeriod, EndPeriod, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Guarda los datos de la guia de importación. Si és nuevo, se actualiza el ID de la guia pasado.<br/>
    ''' </summary>
    ''' <param name="oImportGuide">Puesto a guardar (roImportGuide)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar la guia.</returns>
    ''' <remarks></remarks>
    Public Shared Function SaveImportGuide(ByVal oImportGuide As roImportGuide, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oImportGuide.State = bState

        oResult.Value = oImportGuide.Save(bAudit)

        bState = oImportGuide.State 'OJO

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetExportTemplates(ByVal oState As roWsState) As roGenericVtResponse(Of Byte())

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Byte())

        oResult.Value = roImportGuide.GetExportTemplates(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos de la guia de exportacion. Si és nuevo, se actualiza el ID de la guia pasado.<br/>
    ''' </summary>
    ''' <param name="oExportGuide">Puesto a guardar (roExportGuide)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar la guia.</returns>
    ''' <remarks></remarks>
    Public Shared Function SaveExportGuide(ByVal oExportGuide As roExportGuide, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oExportGuide.State = bState '<-- Vigilar si funciona, en principi pel que he vist sí

        oResult.Value = oExportGuide.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetNextExportGuideId(ByVal iMinRange As Integer, ByVal iMaxRange As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roExportGuide.GetNextExportGuideId(iMinRange, iMaxRange, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve todas las plantillas disponibles de la carpeta datalink del tipo solicitado
    ''' </summary>
    ''' <param name="ProfileMask"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetTemplatesByProfileMask(ByVal ProfileMask As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataTable)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataTable)
        Dim tb As DataTable = Nothing
        Dim roEG As New roExportGuide()
        tb = roEG.GetTemplatesByProfileMask(ProfileMask, oState.IDPassport)
        If tb Is Nothing Then
            oResult.Value = New DataTable()
        Else
            oResult.Value = tb

        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con una guia de importacion concreta
    ''' </summary>
    ''' <param name="_ID"></param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetExportGuide(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roExportGuide)
        Dim oResult As New roGenericVtResponse(Of roExportGuide)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDataLinkState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roExportGuide(_ID, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

End Class