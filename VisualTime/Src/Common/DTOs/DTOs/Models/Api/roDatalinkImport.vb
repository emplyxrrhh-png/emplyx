Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink

    <DataContract>
    Public Enum eCompositeContractType
        <EnumMember> None = 0
        <EnumMember> ContractAndDate = 1
        <EnumMember> CompanyContractAndDate = 2
        <EnumMember> ContractAndPeriod = 3
    End Enum

    Public Enum eImportType
        IsExcelType
        IsAsciiType
        IsXMLType
        IsCustom
    End Enum

    Public Enum DailyCausesColumns
        Contract
        CauseDate
        Cause
        Value
    End Enum

    Public Enum TaskColumns
        Center
        Project
        Task
        Description
        ShortName
        Status
        Priority
        Tags
        BarCode
        BeginDate
        EndDate
        Duration
        CloseType
        CloseValue
        ActivationType
        ActivationValue
        ColaborationType
        AutorizedType
        AutorizedValue
        AtributesAssigned
        Atribute_1
        Atribute_2
        Atribute_3
        Atribute_4
        Atribute_5
        Atribute_6
    End Enum

    Public Enum EmployeeSageMurano
        ID
        DNI
        LastName
        NameEmployee
        RegisterSystemDate
        ActiveDays
        Departments
        EmployeeCode
        CompanyCode
        USR_EMAIL
        USR_SS
        USR_ADDRESS
        USR_POSTALCODE
        USR_TOWN
        USR_PROVINCE
        USR_PERSONALPHONE
        USR_PERSONALMOBILE
        USR_BIRTHDATE
        USR_SEX
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
        ImportPrimaryKey
    End Enum

    Public Enum BusinessCenterEnum
        Name
        Status
        ZonesAreControlled
        ZonesAssigned
        EmployesAreControlled
        EmployeesAssigned
        USR_Field1
        USR_Field2
        USR_Field3
        USR_Field4
        USR_Field5
    End Enum

End Namespace