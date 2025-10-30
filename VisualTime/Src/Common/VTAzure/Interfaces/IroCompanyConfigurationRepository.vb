Imports Robotics.Base.DTOs

Public Interface IroCompanyConfigurationRepository

    Function GetCompanyConfiguration(IdCompany As String) As roCompanyConfiguration

    Function GetCompanies() As roCompanyConfiguration()

End Interface