Imports System.IO
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.VTBase
Imports VTLiveApi

Public Class CommonProxy
    Implements ICommonSvc

    Public Function KeepAlive() As Boolean Implements ICommonSvc.KeepAlive
        Return True
    End Function

    Public Function GetLanguage(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Scope As String) As roGenericVtResponse(Of LanguageFile) Implements ICommonSvc.GetLanguage
        Return CommonMethods.GetLanguage(LanguageReference, LanguageKey, Scope)
    End Function


    'Public Function GetLanguageOld(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Scope As String) As roGenericVtResponse(Of LanguageFile) Implements ICommonSvc.GetLanguageOld
    '    Return CommonMethods.GetLanguageOld(LanguageReference, LanguageKey, Scope)
    'End Function


    Public Sub SaveLanguage(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Key As String, ByVal Value As String) Implements ICommonSvc.SaveLanguage
        CommonMethods.SaveLanguage(LanguageReference, LanguageKey, Key, Value)
    End Sub


    'Public Sub SaveLanguageOLD(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Key As String, ByVal Value As String) Implements ICommonSvc.SaveLanguageOLD
    '    CommonMethods.SaveLanguageOLD(LanguageReference, LanguageKey, Key, Value)

    'End Sub


    Public Function DefaultLanguage() As roGenericVtResponse(Of String) Implements ICommonSvc.DefaultLanguage
        Return CommonMethods.DefaultLanguage()
    End Function


    Public Function GetAdvancedParameter(ByVal parameterName As String, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAdvancedParameter) Implements ICommonSvc.GetAdvancedParameter
        Return CommonMethods.GetAdvancedParameter(parameterName, oState, bolAudit)
    End Function


    Public Function GetAdvancedParameterList(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAdvancedParameter)) Implements ICommonSvc.GetAdvancedParameterList
        Return CommonMethods.GetAdvancedParameterList(oState, bolAudit)
    End Function


    Public Function GetAdvancedParameterDataTable(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of DataTable) Implements ICommonSvc.GetAdvancedParameterDataTable
        Return CommonMethods.GetAdvancedParameterDataTable(oState, bolAudit)
    End Function


    '////byref oAdvancedParameter
    Public Function SaveAdvancedParameter(ByVal oAdvancedParameter As roAdvancedParameter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ICommonSvc.SaveAdvancedParameter
        Return CommonMethods.SaveAdvancedParameter(oAdvancedParameter, oState, bAudit)
    End Function

    Public Function GetVisualTimeEdition() As roGenericVtResponse(Of String) Implements ICommonSvc.GetVisualTimeEdition
        Return CommonMethods.GetVisualTimeEdition()
    End Function
End Class
