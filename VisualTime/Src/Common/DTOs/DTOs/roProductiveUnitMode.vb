Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa un modo del modo de la unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModeStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de modos de la unidadad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModeListResponse
        <DataMember>
        Public Property ProductiveUnits As roProductiveUnitMode()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una modo de la unidad productiva
    ''' </summary>
    <DataContract>
    Public Class roProductiveUnitModeResponse
        <DataMember>
        Public Property ProductiveUnitMode As roProductiveUnitMode

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roProductiveUnitMode
        Protected lngID As Long
        Protected lngIDProductiveUnit As Long
        Protected strName As String
        Protected strShortName As String
        Protected strDescription As String
        Protected strColor As String
        Protected dblCostValue As Double
        Protected lstUnitModePositions As roProductiveUnitModePosition()
        Protected dblCoverage As Double

        Public Sub New()
            lngID = -1
            lngIDProductiveUnit = -1
            strName = String.Empty
            strShortName = String.Empty
            strColor = "#FFFFFF"
            dblCostValue = 0
            lstUnitModePositions = Nothing
            dblCoverage = 0
        End Sub

        ''' <summary>
        ''' Identificador único del modo de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID As Long
            Get
                Return lngID
            End Get
            Set(value As Long)
                lngID = value
            End Set
        End Property
        ''' <summary>
        ''' Identificador de la unidad productiva a la que está asignado el modo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDProductiveUnit As Long
            Get
                Return lngIDProductiveUnit
            End Get
            Set(value As Long)
                lngIDProductiveUnit = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del modo de la unidad productiva
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
        ''' Nombre abreviado del modo de la unidad productiva
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
        ''' Descripción del modo de la unidad productiva
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
        ''' Color identificativo del modo de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property HtmlColor As String
            Get
                Return strColor
            End Get
            Set(value As String)
                strColor = value
            End Set
        End Property
        ''' <summary>
        ''' Coste del modo
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CostValue As Double
            Get
                Return dblCostValue
            End Get
            Set(value As Double)
                dblCostValue = value
            End Set
        End Property

        ''' <summary>
        ''' Lista de posiciones del modo de la unidad productiva
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property UnitModePositions As roProductiveUnitModePosition()
            Get
                Return lstUnitModePositions
            End Get
            Set(value As roProductiveUnitModePosition())
                lstUnitModePositions = value
            End Set
        End Property

        ''' <summary>
        ''' Cobertura del modo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Coverage As Double
            Get
                Return dblCoverage
            End Get
            Set(value As Double)
                dblCoverage = value
            End Set
        End Property

    End Class

End Namespace