Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports SwaggerWcf.Attributes

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum EmployeeColumns
        Contract
        DNI
        BeginDate
        EndDate
        Name
        Surname1
        Surname2
        Card
        Level0
        Level1
        Level2
        Level3
        Level4
        Level5
        Level6
        Level7
        Level8
        Level9
        Level10
        DateLevel10
        Convenio
        GrupoUsuarios
        Login
        ImportPrimaryKey
        Authorizations
        Period
        DNICopyPlan
        WorkCenter
        Language
        IDLevel
        IDParentLevel
        RegisterType
        EndContractReason
        RequirePunchWithPhoto
        RequirePunchWithGeolocation
        EnabledVTDesktop
        EnabledVTPortal
        EnabledVTPortalApp
        EnabledVTVisits
        LoginWithoutContract
        UserPhoto
        EnableBiometricData
        Pin
    End Enum

    <DataContract>
    Public Class roDatalinkEmployeeUserFieldValue

        Public Sub New()
            UserFieldName = String.Empty
            UserFieldValue = String.Empty

            UserFieldValueDate = New Date(1970, 1, 1, 0, 0, 0, 0)
        End Sub

        <SwaggerWcfProperty(Required:=True, Default:="NIF", Description:="Name of the field of the HR file")>
        <DataMember>
        Public Property UserFieldName As String

        <SwaggerWcfProperty(Required:=True, Default:="123456A", Description:="Value of the field of the HR file.
                                                                                FieldTypes.tText ==> Text con valor
                                                                                FieldTypes.tNumeric ==> Text solo númerico
                                                                                FieldTypes.tDate ==> Texto con fecha en formto yyyy/MM/dd
                                                                                FieldTypes.tDecimal ==> Texto con númerico y separador decimales '.' ej. 32323.3
                                                                                FieldTypes.tList ==> Text con valor
                                                                                FieldTypes.tDatePeriod ==> Fecha inicial y fecha final en formato: yyyy/MM/dd*yyyy/MM/dd
                                                                                FieldTypes.tTimePeriod ==> Hora inicial y hora final en formato: HH:mm*HH:mm
                                                                                FieldTypes.tTime ==> Texto con valor HHHH:mm (p.ej. 125:30 //125 horas, 30 min)")>
        <DataMember>
        Public Property UserFieldValue As String

        <SwaggerWcfProperty(Required:=True, Default:="/Date(1609455600000)/", Description:="Date Effect of the value of the field of the HR file")>
        <DataMember>
        Public Property UserFieldValueDate As Date

    End Class

    Public Interface IDatalinkEmployee

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsUsrName As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandarEmployeeResponse

        <DataMember>
        <Description("Result code that indicates the request status")>
        Public Property ResultCode As ReturnCode

        <DataMember>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Result from Rest api request")>
        <Description("Result from Rest api request")>
        Public Property ResultDetails As String

        <DataMember>
        <SwaggerWcfProperty(Required:=True, Default:="", Description:="Result user with updated or created data")>
        <Description("Result user with updated or created data")>
        Public Property ResultEmployee As roEmployee

    End Class

    <DataContract>
    Public Class roDatalinkEmployeeContract

        <DataMember>
        Public Property StartContractDate As Date

        <DataMember>
        Public Property EndContractDate As Date

        <DataMember>
        Public Property IDContract As String

        <DataMember>
        Public Property LabAgreeName As String

    End Class

    <DataContract>
    Public Class roDatalinkEmployeeMovility

        <DataMember>
        Public Property StartDate As Date

        <DataMember>
        Public Property EndDate As Date

        <DataMember>
        Public Property IDGroup As Integer

        <DataMember>
        Public Property IsTransfer As Integer

    End Class

    <DataContract>
    Public Class roDatalinkEmployeeContractsHistory

        <DataMember>
        Public Property Contracts As roDatalinkEmployeeContract()

    End Class

    Public Class roDatalinkStandardPhoto

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey

        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF

        <DataMember>
        Public Property PhotoData As String ' Photo on base64 format

    End Class

End Namespace