Imports System.IO
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase.roTypes

<DataContract()>
<Serializable>
Public Class roVTLicense
    Private _iMaxExmployees As Long
    Private _iMaxJobExmployees As Long
    Private _bIsProductiv As Boolean
    Private _xExpirationDate As DateTime
    Private _oServerLicensceStatus As roServerLicense.roLicenseStatus
    Private _Edition As roServerLicense.roVisualTimeEdition

    Public Sub New()
        _iMaxExmployees = 0
        _iMaxJobExmployees = 0
        _bIsProductiv = False
        _xExpirationDate = roTypes.CreateDateTime(2079, 1, 1, 0, 0, 0)
        _oServerLicensceStatus = roServerLicense.roLicenseStatus.roLicense_Inactive
        _Edition = roServerLicense.roVisualTimeEdition.NotSet
    End Sub

    <DataMember()>
    Public Property MaxExmployees As Long
        Get
            Return _iMaxExmployees
        End Get
        Set(value As Long)
            _iMaxExmployees = value
        End Set
    End Property

    <DataMember()>
    Public Property MaxJobExmployees As Long
        Get
            Return _iMaxJobExmployees
        End Get
        Set(value As Long)
            _iMaxJobExmployees = value
        End Set
    End Property

    <DataMember()>
    Public Property IsProductiv As Boolean
        Get
            Return _bIsProductiv
        End Get
        Set(value As Boolean)
            _bIsProductiv = value
        End Set
    End Property

    <DataMember()>
    Public Property ExpirationDate As DateTime
        Get
            Return _xExpirationDate
        End Get
        Set(value As DateTime)
            _xExpirationDate = value
        End Set
    End Property

    <DataMember()>
    Public Property ServerLicensceStatus As roServerLicense.roLicenseStatus
        Get
            Return _oServerLicensceStatus
        End Get
        Set(value As roServerLicense.roLicenseStatus)
            _oServerLicensceStatus = value
        End Set
    End Property

    <DataMember()>
    Public Property Edition As roServerLicense.roVisualTimeEdition
        Get
            Return _Edition
        End Get
        Set(value As roServerLicense.roVisualTimeEdition)
            _Edition = value
        End Set
    End Property
End Class

<DataContract>
Public Class roServerLicense

#Region "Declarations - Constructor"

    Private oSupport As roSupport

    Public Enum roLicenseStatus
        <EnumMember> roLicense_Invalid = 0
        <EnumMember> roLicense_Inactive = 11
        <EnumMember> roLicense_Active = 12
        <EnumMember> roLicense_ClientCopy = 15
        <EnumMember> roLicense_Demo_Invalid = 20
        <EnumMember> roLicense_Demo_Valid = 21
        <EnumMember> roLicense_Expired = 91
    End Enum

    Public Enum roVisualTimeEdition
        <EnumMember> NotSet = 0
        <EnumMember> Starter = 1
        <EnumMember> One = 2
        <EnumMember> Advanced = 3
        <EnumMember> Premium = 4
    End Enum

    Private mLicenseFile As String
    Private mLicenseContent As String
    Private mConn As Object
    Private bIsPod As Boolean = False

    Private dDemoMediaLastCheck As Double

    Public Sub New()

        Me.oSupport = New roSupport
        Me.mLicenseContent = String.Empty

        If roConstants.IsMultitenantServiceEnabled() Then
            Me.mLicenseFile = "remote" 'System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) & "\" & LICENSE_FILE
            Me.mLicenseContent = roTypes.Any2String(Threading.Thread.GetDomain.GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString & "_license"))
        Else
            Try
                Dim strCompany As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))
                'DEBUG HA EN LOCAL
                'If strCompany = String.Empty Then
                '    strCompany = Robotics.Azure.RoAzureSupport.GetCompanyName
                'End If

                Me.mLicenseFile = "remote"
                Me.mLicenseContent = String.Empty

                Dim oConfig As roCompanyConfiguration = DataLayer.roCacheManager.GetInstance.GetCompany(strCompany)
                If oConfig Is Nothing Then
                    Dim oCompanyConfiguration As New Azure.roCompanyConfigurationRepository
                    oConfig = oCompanyConfiguration.GetCompanyConfiguration(strCompany)

                    If oConfig IsNot Nothing AndAlso oConfig.companyname <> String.Empty Then

                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roServerLicense::License for company " & strCompany & " requested from cosmos")
                        DataLayer.roCacheManager.GetInstance.UpdateCompanyCache(oConfig)
                        Me.mLicenseContent = oConfig.license
                    End If
                Else
                    Me.mLicenseContent = oConfig.license
                End If
            Catch ex As Exception
                Me.mLicenseFile = String.Empty
                Me.mLicenseContent = String.Empty
            End Try

        End If

    End Sub

#End Region

#Region "Methods"

    Public Function Status(Optional ByVal LogHandle As roLog = Nothing, Optional ByVal AlternateLicenseFile As String = "") As roLicenseStatus
        Return roLicenseStatus.roLicense_Active
    End Function

    Public Function FeatureIsInstalled(ByVal Feature As String) As Boolean
        Dim isInstalled As Boolean = False

        Dim FeatureGroup As String
        Dim FeatureItem As String

        If Len(Feature) = 0 Then
            isInstalled = True
        Else
            If InStr(Feature, "\") > 0 Then
                FeatureGroup = String2Item(Feature, 0, "\")
                FeatureItem = roConversions.String2RestItems(Feature, 0, "\")
                If UCase$(Any2String(FeatureData(FeatureGroup, FeatureItem))) = "INSTALLED" Then
                    isInstalled = True
                End If
            Else
                isInstalled = (UCase$(Any2String(FeatureData(Feature, "Available"))) = "TRUE")
            End If
        End If

        Return isInstalled
    End Function

    Public Function FeatureData(ByVal Feature As String, ByVal Variable As String, Optional ByVal LogHandle As roLog = Nothing) As String
        Return Me.oSupport.INIRead(mLicenseFile, Feature, Variable, "",, Me.mLicenseContent)
    End Function

#End Region

End Class