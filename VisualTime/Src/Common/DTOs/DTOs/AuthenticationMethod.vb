Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Represents valid authentication methods.
    ''' </summary>
    '''
    <DataContract>
    Public Enum AuthenticationMethod
        <Description("Password")>
        <EnumMember> Password = 1
        <Description("Integrated Security")>
        <EnumMember> IntegratedSecurity = 2
        <Description("Card")>
        <EnumMember> Card = 3
        <Description("Biometry")>
        <EnumMember> Biometry = 4
        <Description("Pin")>
        <EnumMember> Pin = 5
        <Description("Vehicle Plate")>
        <EnumMember> Plate = 6
        <Description("NFC")>
        <EnumMember> NFC = 7

    End Enum

    Public Enum AuthenticationMethodMerge
        <EnumMember> CardAndBiometry = 1     ' Tarjeta y huella, o solo tarjeta si no hay lector de huella
        <EnumMember> BiometryElseCard = 2    ' Huella o tarjeta si no hay lector de huella
        <EnumMember> CardAndPin = 3          ' Tarjeta y pin, o solo tarjeta si no se puede introducir el pin
    End Enum

End Namespace