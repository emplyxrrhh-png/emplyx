Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCommuniques

Public Class CommuniqueProxy
    Implements ICommuniquesSvc

    Public Function KeepAlive() As Boolean Implements ICommuniquesSvc.KeepAlive
        Return True
    End Function

    Public Function CreateOrUpdateCommunique(ByVal oCommunique As roCommunique, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueResponse Implements ICommuniquesSvc.CreateOrUpdateCommunique
        Return CommuniqueMethods.CreateOrUpdateCommunique(oCommunique, oState, bAudit)
    End Function

    Public Function GetAllCommuniques(ByVal oState As roWsState, Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommuniqueListResponse Implements ICommuniquesSvc.GetAllCommuniques
        Return CommuniqueMethods.GetAllCommuniques(oState, idEmployee, bAudit)
    End Function

    Public Function DeleteCommunique(ByVal idCommunique As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse Implements ICommuniquesSvc.DeleteCommunique
        Return CommuniqueMethods.DeleteCommunique(idCommunique, oState, bAudit)
    End Function

    Public Function GetCommuniqueStatus(ByVal idCommunique As Integer, ByVal oState As roWsState, Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommuniqueStatusResponse Implements ICommuniquesSvc.GetCommuniqueWithStatistics
        Return CommuniqueMethods.GetCommuniqueWithStatistics(idCommunique, oState, , bAudit)
    End Function

    Public Function GetAllEmployeeCommuniquesWithStatus(ByVal idEmployee As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roEmployeeCommuniquesStatusResponse Implements ICommuniquesSvc.GetAllEmployeeCommuniquesWithStatus
        Return CommuniqueMethods.GetAllEmployeeCommuniquesWithStatus(idEmployee, oState, bAudit)
    End Function

    Public Function SetCommuniqueRead(ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse Implements ICommuniquesSvc.SetCommuniqueRead
        Return CommuniqueMethods.SetCommuniqueRead(idCommunique, idEmployee, oState, bAudit)
    End Function

    Public Function AnswerCommunique(ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal sAnswer As String, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse Implements ICommuniquesSvc.AnswerCommunique
        Return CommuniqueMethods.AnswerCommunique(idCommunique, idEmployee, sAnswer, oState, bAudit)
    End Function

    Public Function GetCommuniqueForEmployee(idCommunique As Integer, idEmployee As Integer, oState As roWsState, Optional bAudit As Boolean = False) As roCommuniqueStatusResponse Implements ICommuniquesSvc.GetCommuniqueForEmployee
        Return CommuniqueMethods.GetCommuniqueWithStatistics(idCommunique, oState, idEmployee, bAudit)
    End Function

End Class
