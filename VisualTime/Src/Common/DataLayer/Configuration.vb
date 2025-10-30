Imports System.Reflection
Imports System.Web.Compilation
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

''' <summary>
''' Exposes configuration values defined in web.config.
''' </summary>
Public NotInheritable Class Configuration

    Private Sub New()
    End Sub

    Public Shared ReadOnly RootUrl As String = ""
    Public Shared ReadOnly DataProvider As ProviderType = DirectCast([Enum].Parse(GetType(ProviderType), "Sql"), ProviderType)

End Class