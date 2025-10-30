Imports Robotics.Base.DTOs

Public Interface IroTerminalRepository

    Function GetTerminals() As roTerminalRegister()

    Function GetTerminalCompanyConfiguration(strSerialNumber As String) As roTerminalRegister

    Function AddTerminalToCompany(TerminalSerialNumber As String, CompanyName As String, TerminalModel As String) As String

    Function UpdateTerminalToCompany(TerminalSerialNumber As String, CompanyName As String, enabled As Boolean) As String

    Function DeleteTerminalFromCompany(TerminalSerialNumber As String, CompanyName As String) As String

End Interface