Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs.UserFieldsTypes

    Public Enum FieldTypes
        <EnumMember> tText = 0
        <EnumMember> tNumeric = 1
        <EnumMember> tDate = 2
        <EnumMember> tDecimal = 3
        <EnumMember> tTime = 4
        <EnumMember> tList = 5
        <EnumMember> tDatePeriod = 6
        <EnumMember> tTimePeriod = 7
        <EnumMember> tLink = 8
        <EnumMember> tDocument = 9
    End Enum

    Public Enum AccessLevels
        <EnumMember> aLow
        <EnumMember> aMedium
        <EnumMember> aHigh
    End Enum

    Public Enum ConvertibleValues
        <EnumMember> Mobile 'sysroMobile - numeric
        <EnumMember> Gender 'sysroGender - list
        <EnumMember> ProfessionalCategory ' sysroProfessionalCategory - list
        <EnumMember> QuoteGroup 'sysroQuoteGroup - list
        <EnumMember> Position 'sysroPosition - text
        <EnumMember> TotalSalary 'sysroTotalSalary - decimal
        <EnumMember> BaseSalary 'sysroBaseSalary - decimal
        <EnumMember> SalarySupp 'sysroSalarySupp - decimal
        <EnumMember> ExtraSalary 'sysroExtraSalary - decimal
        <EnumMember> EarningsOvertime 'sysroEarningsOverTime - decimal
    End Enum

    Public Enum Types
        <EnumMember> EmployeeField
        <EnumMember> GroupField
        <EnumMember> TaskField
        <EnumMember> TaskEmployeeField
    End Enum

    Public Enum AccessValidation
        <EnumMember> None
        <EnumMember> Required
        <EnumMember> Warning
    End Enum

    Public Enum CompareType
        <EnumMember> Equal
        <EnumMember> Minor
        <EnumMember> MinorEqual
        <EnumMember> Major
        <EnumMember> MajorEqual
        <EnumMember> Distinct
        <EnumMember> Contains
        <EnumMember> NotContains
        <EnumMember> StartWith
        <EnumMember> EndWidth
    End Enum

    Public Enum CompareValueType
        <EnumMember> DirectValue
        <EnumMember> CurrentDate
        ' ...
    End Enum

End Namespace