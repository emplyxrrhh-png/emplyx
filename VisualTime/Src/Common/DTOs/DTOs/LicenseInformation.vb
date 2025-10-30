Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roLicenseSolution
        Protected _isAvailable As Boolean
        Protected _isCorrect As Boolean
        Protected _languageTag As String
        Protected _missingModules As String

        Private _licenseEmployees As Long
        Private _availableLicenseEmployees As Long

        Public Sub New()
            _isAvailable = False
            _isCorrect = False
            _languageTag = String.Empty
            _missingModules = String.Empty
            _licenseEmployees = 0
            _availableLicenseEmployees = 0
        End Sub

        <DataMember()>
        Public Property IsAvailable As Boolean
            Get
                Return _isAvailable
            End Get
            Set(value As Boolean)
                _isAvailable = value
            End Set
        End Property

        <DataMember()>
        Public Property IsCorrect As Boolean
            Get
                Return _isCorrect
            End Get
            Set(value As Boolean)
                _isCorrect = value
            End Set
        End Property

        <DataMember()>
        Public Property LanguageTag As String
            Get
                Return _languageTag
            End Get
            Set(value As String)
                _languageTag = value
            End Set
        End Property

        <DataMember()>
        Public Property MissingModules As String
            Get
                Return _missingModules
            End Get
            Set(value As String)
                _missingModules = value
            End Set
        End Property

        <DataMember()>
        Public Property LicenseEmployees As Long
            Get
                Return _licenseEmployees
            End Get
            Set(value As Long)
                _licenseEmployees = value
            End Set
        End Property

        <DataMember()>
        Public Property AvailableLicenseEmployees As Long
            Get
                Return _availableLicenseEmployees
            End Get
            Set(value As Long)
                _availableLicenseEmployees = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roLicenseModule
        Protected _isAvailable As Boolean
        Protected _isCorrect As Boolean
        Protected _languageTag As String
        Protected _missingModules As String

        Public Sub New()
            _isAvailable = False
            _isCorrect = False
            _languageTag = String.Empty
            _missingModules = String.Empty
        End Sub

        <DataMember()>
        Public Property IsAvailable As Boolean
            Get
                Return _isAvailable
            End Get
            Set(value As Boolean)
                _isAvailable = value
            End Set
        End Property

        <DataMember()>
        Public Property IsCorrect As Boolean
            Get
                Return _isCorrect
            End Get
            Set(value As Boolean)
                _isCorrect = value
            End Set
        End Property

        <DataMember()>
        Public Property LanguageTag As String
            Get
                Return _languageTag
            End Get
            Set(value As String)
                _languageTag = value
            End Set
        End Property

        <DataMember()>
        Public Property MissingModules As String
            Get
                Return _missingModules
            End Get
            Set(value As String)
                _missingModules = value
            End Set
        End Property

    End Class

End Namespace