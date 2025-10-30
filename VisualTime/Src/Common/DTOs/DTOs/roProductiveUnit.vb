Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ProductiveUnitSummaryType
        <EnumMember> Anual
        <EnumMember> Monthly
        <EnumMember> Daily
        <EnumMember> Weekly
    End Enum

    ''' <summary>
    ''' Representa una unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de unidades productivas
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitListResponse
        <DataMember>
        Public Property ProductiveUnits As roProductiveUnit()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitResponse
        <DataMember>
        Public Property ProductiveUnit As roProductiveUnit

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un resumen de la unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitSummaryResponse
        <DataMember>
        Public Property ProductiveUnitSummary As roProductiveUnitSummary

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roProductiveUnit
        Protected intID As Integer
        Protected strName As String
        Protected strShortName As String
        Protected strDescription As String
        Protected intColor As Integer
        Protected intIDCenter As Integer
        Protected lstUnitModes As roProductiveUnitMode()

        Public Sub New()
            intID = -1
            strName = String.Empty
            strShortName = String.Empty
            intColor = 0
            lstUnitModes = Nothing
            intIDCenter = 0
        End Sub

        ''' <summary>
        ''' Identificador único de las unidades productivas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID As Integer
            Get
                Return intID
            End Get
            Set(value As Integer)
                intID = value
            End Set
        End Property
        ''' <summary>
        ''' Nombre de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String
            Get
                Return strName
            End Get
            Set(value As String)
                strName = value
            End Set
        End Property
        ''' <summary>
        ''' Nombre abreviado de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShortName As String
            Get
                Return strShortName
            End Get
            Set(value As String)
                strShortName = value
            End Set
        End Property
        ''' <summary>
        ''' Descripción de la unidad productiva
        ''' </summary>
        <DataMember()>
        Public Property Description As String
            Get
                Return strDescription
            End Get
            Set(value As String)
                strDescription = value
            End Set
        End Property
        ''' <summary>
        ''' Color identificativo de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Color As Integer
            Get
                Return intColor
            End Get
            Set(value As Integer)
                intColor = value
            End Set
        End Property
        ''' <summary>
        ''' Lista de modos de la unidad productiva
        ''' </summary>
        <DataMember()>
        Public Property UnitModes As roProductiveUnitMode()
            Get
                Return lstUnitModes
            End Get
            Set(value As roProductiveUnitMode())
                lstUnitModes = value
            End Set
        End Property

        ''' <summary>
        ''' Centro de coste asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDCenter As Integer
            Get
                Return intIDCenter
            End Get
            Set(value As Integer)
                intIDCenter = value
            End Set
        End Property
    End Class

    <DataContract>
    Public Class roProductiveUnitSummary

        Public Sub New()
            ProductiveUnitSummary_ModeDetail = {}
        End Sub

        <DataMember>
        Public Property ProductiveUnitSummary_ModeDetail As roProductiveUnitSummary_ModeDetail()
    End Class

    <DataContract>
    Public Class roProductiveUnitSummary_ModeDetail
        <DataMember>
        Public Property IDMode As Integer

        <DataMember>
        Public Property IDProductiveUnit As Integer

        <DataMember>
        Public Property ModeName As String

        <DataMember>
        Public Property Color As Integer

        <DataMember>
        Public Property Quantity As Double
    End Class

End Namespace