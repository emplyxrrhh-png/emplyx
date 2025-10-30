Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink.DataLink

Public Class DatalinkProxy
    Implements IDatalinkSvc

    Public Function KeepAlive() As Boolean Implements IDatalinkSvc.KeepAlive
        Return True
    End Function

    Public Function GetDatalinkGuides(ByVal oState As roWsState) As roDatalinkGuideListResponse Implements IDatalinkSvc.GetDatalinkGuides
        Return DatalinkMethods.GetDatalinkGuides(oState)
    End Function

    Public Function GetDatalinkGuide(ByVal eConcept As roDatalinkConcept, ByVal oState As roWsState) As roDatalinkGuideResponse Implements IDatalinkSvc.GetDatalinkGuide
        Return DatalinkMethods.GetDatalinkGuide(eConcept, oState)
    End Function

    Public Function SaveDatalinkGuide(ByVal oGuide As roDatalinkGuide, ByVal oState As roWsState) As roStandarResponse Implements IDatalinkSvc.SaveDatalinkGuide
        Return DatalinkMethods.SaveDatalinkGuide(oGuide, oState)
    End Function
End Class
