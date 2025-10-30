Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink.DataLink

Public Class DatalinkMethods

    Public Shared Function GetDatalinkGuides(ByVal oState As roWsState) As roDatalinkGuideListResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkManager(bState)
        Dim oGuides = oDataLinkManager.GetDataLinkGuides()

        'crear el response genérico
        Dim genericResponse As New roDatalinkGuideListResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Guides = oGuides.ToArray
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetDatalinkGuide(ByVal eConcept As roDatalinkConcept, ByVal oState As roWsState) As roDatalinkGuideResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkManager(bState)
        Dim oGuide = oDataLinkManager.Load(eConcept)

        'crear el response genérico
        Dim genericResponse As New roDatalinkGuideResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Guide = oGuide
        genericResponse.oState = newGState

        Return genericResponse

    End Function

    Public Shared Function SaveDatalinkGuide(ByVal oGuide As roDatalinkGuide, ByVal oState As roWsState) As roStandarResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkManager(bState)
        Dim bResult = oDataLinkManager.Save(oGuide)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Result = bResult
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetExportTemplateBytes(ByVal idExportGuide As Integer, ByVal idTemplate As Integer, ByVal oState As roWsState) As roDatalinkTemplateBytesResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkExportManager(bState)
        Dim bResult As Byte() = {}

        bResult = oDataLinkManager.GetExportTemplateBytes(idExportGuide, idTemplate)

        'crear el response genérico
        Dim genericResponse As New roDatalinkTemplateBytesResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Content = bResult
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveExportTemplateBytes(ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer, ByVal oState As roWsState) As roStandarResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkExportManager(bState)
        Dim bResult As Boolean = False

        bResult = oDataLinkManager.SaveExportTemplateBytes(bTemplateContent, idExportGuide, idTemplate)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Result = bResult
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function DuplicateExportTemplateBytes(ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer, ByVal newTemplateName As String, ByVal oState As roWsState) As roStandarResponse

        Dim bState = New roDataLinkGuideState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDataLinkManager As New roDataLinkExportManager(bState)
        Dim bResult As Boolean = False

        bResult = oDataLinkManager.DuplicateExportTemplateBytes(bTemplateContent, idExportGuide, idTemplate, newTemplateName)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse()
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDataLinkManager.State, newGState)

        genericResponse.Result = bResult
        genericResponse.oState = newGState

        Return genericResponse
    End Function

End Class