Imports Robotics
Imports Robotics.Base.DTOs

Public Class CommuniquesHelper

    Public Sub New()
    End Sub

    Public Function SetAvailableCommuniques(idPassport As Integer, passportCommuniques As Integer(), otherCommuniques As Integer())

        Robotics.Web.Base.API.Fakes.ShimCommuniqueServiceMethods.AllInstances.GetCommuniqueStatusPageBaseInt32Int32Boolean =
            Function(sm, reference, idCommunique, idEmployee, bAudit)

                If passportCommuniques.Contains(idCommunique) Then
                    Return New roCommuniqueWithStatistics() With {
                    .Communique = New Base.DTOs.roCommunique With {.Id = idCommunique, .CreatedBy = New roPassportWithPhoto() With {.IdPassport = idPassport}},
                    .EmployeeCommuniqueStatus = {}
                    }
                Else
                    If otherCommuniques.Contains(idCommunique) Then
                        Return New roCommuniqueWithStatistics() With {
                            .Communique = New Base.DTOs.roCommunique With {.Id = idCommunique, .CreatedBy = New roPassportWithPhoto() With {.IdPassport = idPassport + 1}},
                            .EmployeeCommuniqueStatus = {}
                            }
                    Else

                        Return New roCommuniqueWithStatistics() With {
                            .Communique = New Base.DTOs.roCommunique With {.Id = -1, .CreatedBy = New roPassportWithPhoto() With {.IdPassport = 0}},
                            .EmployeeCommuniqueStatus = {}
                            }
                    End If
                End If

            End Function

    End Function

End Class