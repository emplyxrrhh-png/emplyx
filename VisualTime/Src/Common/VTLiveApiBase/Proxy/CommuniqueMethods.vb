Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCommuniques

Public Class CommuniqueMethods

    Public Shared Function CreateOrUpdateCommunique(ByVal oCommunique As roCommunique, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueResponse
        Dim genericResponse As New roCommuniqueResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        If oCommuniqueManager.CreateOrUpdateCommunique(oCommunique, bAudit) Then
            genericResponse.Communique = oCommunique
        End If

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCommuniquesByCreator(ByVal oState As roWsState, Optional creatorId As Integer = 0) As roCommuniqueListResponse
        Dim genericResponse As New roCommuniqueListResponse
        'Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        Dim oCommuniques As Generic.List(Of roCommunique) = oCommuniqueManager.GetCommuniquesByCreator(creatorId)

        ' Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.Communiques = If(oCommuniques IsNot Nothing, oCommuniques.ToArray, {})
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetAllCommuniques(ByVal oState As roWsState, Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommuniqueListResponse
        Dim genericResponse As New roCommuniqueListResponse
        'Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        Dim oCommuniques As Generic.List(Of roCommunique) = oCommuniqueManager.GetAllCommuniques(idEmployee, bAudit)

        ' Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.Communiques = If(oCommuniques IsNot Nothing, oCommuniques.ToArray, {})
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function DeleteCommunique(ByVal idCommunique As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse
        Dim genericResponse As New roCommuniqueStandarResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        genericResponse.Result = oCommuniqueManager.DeleteCommunique(idCommunique, bAudit)

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetCommuniqueWithStatistics(ByVal idCommunique As Integer, ByVal oState As roWsState, Optional ByVal idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommuniqueStatusResponse
        Dim genericResponse As New roCommuniqueStatusResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        genericResponse.Status = oCommuniqueManager.GetCommuniqueWithStatistics(idCommunique, bAudit)

        If idEmployee > 0 Then
            genericResponse.Status.EmployeeCommuniqueStatus = {genericResponse.Status.EmployeeCommuniqueStatus.ToList.Find(Function(x) x.IdEmployee = idEmployee)}
        End If

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetAllEmployeeCommuniquesWithStatus(ByVal idEmployee As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roEmployeeCommuniquesStatusResponse
        Dim genericResponse As New roEmployeeCommuniquesStatusResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        genericResponse.Status = oCommuniqueManager.GetAllEmployeeCommuniquesWithStatus(idEmployee, bAudit).ToArray

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SetCommuniqueRead(ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse
        Dim genericResponse As New roCommuniqueStandarResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        genericResponse.Result = oCommuniqueManager.SetCommuniqueRead(idCommunique, idEmployee, bAudit)

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function AnswerCommunique(ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal sAnswer As String, ByVal oState As roWsState, Optional ByVal bAudit As Boolean = False) As roCommuniqueStandarResponse
        Dim genericResponse As New roCommuniqueStandarResponse
        ' Cambio mi state genérico a un estado especifico
        Dim bState = New roCommuniqueState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCommuniqueManager As New roCommuniqueManager(bState)
        genericResponse.Result = oCommuniqueManager.AnswerCommunique(idCommunique, idEmployee, sAnswer, bAudit)

        'Crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCommuniqueManager.State, newGState)
        genericResponse.oState = newGState

        Return genericResponse
    End Function

End Class