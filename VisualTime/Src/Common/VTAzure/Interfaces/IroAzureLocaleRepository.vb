Imports Robotics.Base.DTOs

Public Interface IroAzureLocaleRepository

    Function GetLocaleByKey(key As String) As roAzureLocale

    Function GetLocaleById(id As Integer) As roAzureLocale

    Function GetLocales() As roAzureLocale()

End Interface