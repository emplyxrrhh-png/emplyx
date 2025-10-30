Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Security.Base

Public Class BusinessHelper

    Public Sub New()
    End Sub

    Public Sub AdvancedParameterStub(ByVal parameters As Dictionary(Of String, String))
        Robotics.Base.VTBusiness.Common.AdvancedParameter.Fakes.ShimroAdvancedParameter.ConstructorStringroAdvancedParameterStateBoolean =
                        Function(ByVal advancedparameter As roAdvancedParameter, ByVal _Name As String, ByVal _State As roAdvancedParameterState, ByVal bAudit As Boolean)

                            If parameters.ContainsKey(_Name) Then
                                advancedparameter.Value = parameters(_Name)
                            Else
                                advancedparameter.Value = ""
                            End If

                            Return advancedparameter
                        End Function
    End Sub


    Public Function GetEmployeeLockDatetoApplyStub(freezeDate As Date)
        Base.VTBusiness.Common.Fakes.ShimroBusinessSupport.GetEmployeeLockDatetoApplyInt32BooleanRefroBusinessStateRefBoolean =
                        Function(idEmployee, ByRef bCheckLock, ByRef oState, bLock)
                            Return freezeDate
                        End Function
    End Function

End Class