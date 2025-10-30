Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.DataLayer

Public Class AdvancedParametersHelper

    Public Sub New()
    End Sub



    Public Sub AdvancedParameterCacheStub(ByVal parameters As Dictionary(Of String, String))
        Robotics.DataLayer.Fakes.ShimroCacheManager.AllInstances.GetAdvParametersCacheStringString =
                        Function(oCachemanager As roCacheManager, ByVal strCompanyName As String, ByVal key As String) As String
                            Dim sValue As String
                            If parameters.ContainsKey(key) Then
                                sValue = parameters(key)
                            Else
                                sValue = ""
                            End If
                            Return sValue
                        End Function
    End Sub

End Class