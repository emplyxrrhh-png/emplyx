Imports Robotics.VTBase.Extensions

Public Class ParametersHelper

    Public Sub New()
    End Sub

    Public Sub ParameterStub(ByVal parameters As Dictionary(Of String, String))

        Robotics.VTBase.Extensions.Fakes.ShimroParameters.AllInstances.LoadBoolean = Function(ByVal oParams As roParameters, ByVal bAudit As Boolean)
                                                                                         Return True
                                                                                     End Function

        Robotics.VTBase.Extensions.Fakes.ShimroParameters.AllInstances.ParameterGetParameters = Function(oParams As roParameters, eParam As Robotics.Base.DTOs.Parameters)

                                                                                                    If parameters.ContainsKey(eParam.ToString) Then Return parameters(eParam.ToString)

                                                                                                    Return String.Empty

                                                                                                End Function

    End Sub

End Class