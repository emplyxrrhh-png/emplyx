Imports System.ComponentModel
Imports System.Globalization
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs

Namespace Robotics.Base

    <DataContract>
    Public Class Report

        <DataMember>
        Public Property Id() As Integer
        <DataMember>
        Public Property CreatorPassportId() As Integer?
        <DataMember>
        Public Property Name() As String
        <DataMember>
        Public Property DisplayName() As String
        <DataMember>
        Public Property Description() As String
        <DataMember>
        Public Property CreationDate() As DateTime
        <DataMember>
        Public Property IsEmergencyReport() As Boolean
        <DataMember>
        Public Property LayoutXMLBinary() As Byte()
        <DataMember>
        Public Property LayoutPreviewXMLBinary() As Byte()

        <DataMember>
        Public Property Permissions As ReportPermissions
        <DataMember(Order:=0)>
        Public Property ParametersList As List(Of ReportParameter)
        <DataMember(Order:=1)>
        Public Property CategoriesList As List(Of roSecurityCategory)
        <DataMember(Order:=3)>
        Public Property CategoriesArray As roSecurityCategory()
            Get
                Return CategoriesList?.ToArray()
            End Get
            Set()
            End Set
        End Property
        <DataMember(Order:=2)>
        Public Property ExecutionsList As List(Of ReportExecution)
        <DataMember>
        Public Property PlannedExecutionsList As List(Of ReportPlannedExecution)

        <DataMember>
        Public Property CreatorName As String
        <DataMember>
        Public Property LastExecution As ReportExecution

        <DataMember>
        Public Property HasExecutionsPlanned As Boolean

        <DataMember>
        Public Property LastParameters As String
        <DataMember>
        Public Property ParametersJson As String

        <DataMember>
        Public Property RequieredFeature As String
        <DataMember>
        Public Property CategoriesDescription As String
        <DataMember>
        Public Property RequiredFunctionalities As String
    End Class

#Region "Report Complementary Classes"

    <DataContract>
    Public Class ReportCategory

        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property MotherCategoryId

    End Class

    <DataContract>
    <KnownType(GetType(ReportExportExtensions))>
    Public Class ReportExecution

        <DataMember>
        Public Property Guid As Guid
        <DataMember>
        Public Property Status As Integer
        <DataMember>
        Public Property ExecutionDate As DateTime?
        <DataMember>
        Public Property ReportID As Integer
        <DataMember>
        Public Property FileLink As String
        <DataMember>
        Public Property PassportID As Integer
        <DataMember>
        Public Property Binary As Byte()
        <DataMember>
        Public Property Format As ReportExportExtensions

        <DataMember>
        Public Property BlobLink As String
            Get
                Return ReportID & "_" & Guid.ToString
            End Get
            Set(value As String)
            End Set
        End Property
        <DataMember(Order:=0)>
        Public Property Extension As String
            Get
                Return [Enum].GetName(GetType(ReportExportExtensions), Me.Format)
            End Get
            Set(value As String)
            End Set
        End Property

        Public Sub New()
            Guid = Guid.NewGuid()
        End Sub

    End Class

    <DataContract>
    <KnownType(GetType(ReportExportExtensions))>
    Public Class ReportPlannedExecution

        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property ReportId As Integer
        <DataMember>
        Public Property PassportId As Integer
        <DataMember>
        Public Property Language As String
        <DataMember>
        Public Property Scheduler As String
        <DataMember>
        Public Property Destination As String
        <DataMember>
        Public Property Format As ReportExportExtensions
        <DataMember>
        Public Property ParametersList As List(Of ReportParameter)
        <DataMember>
        Public Property NextExecutionDate As DateTime?
        <DataMember>
        Public Property LastExecutionDate As DateTime?
        <DataMember>
        Public Property RegisteredPlannedExecutionDate As DateTime

        <DataMember>
        Public Property CreatorName As String
        <DataMember>
        Public Property ParametersJson As String
        <DataMember>
        Public Property ViewFields As String

        Public Class ReportPlannedExecutionComparer
            Implements IEqualityComparer(Of ReportPlannedExecution)

            Public Shadows Function Equals(firstPlannedExecution As ReportPlannedExecution, secondPlannedExecution As ReportPlannedExecution) As Boolean Implements IEqualityComparer(Of ReportPlannedExecution).Equals
                Return (firstPlannedExecution.Id = secondPlannedExecution.Id)
            End Function

            Public Shadows Function GetHashCode(plannedExecution As ReportPlannedExecution) As Integer Implements IEqualityComparer(Of ReportPlannedExecution).GetHashCode
                Return plannedExecution.Id
            End Function

        End Class

    End Class

    <DataContract()>
    Public Class ReportPassport
        Public Property Name As String
        Public Property Description As String
    End Class

    <DataContract>
    Public Enum ReportDestinationType
        <EnumMember> printer
        <EnumMember> email
        <EnumMember> location
        <EnumMember> empdocument
        <EnumMember> empemail
        <EnumMember> emproute
        <EnumMember> supervisors
    End Enum

    <DataContract()>
    Public Class ReportPlannedDestination
        <DataMember(Name:="type")>
        Public Property Type As String

        <DataMember(Name:="value")>
        Public Value As String
    End Class

    <DataContract>
    <KnownType(GetType(String()))>
    <KnownType(GetType(DateTime()))>
    <KnownType(GetType(Int16()))>
    <KnownType(GetType(Int32()))>
    <KnownType(GetType(Int64()))>
    <KnownType(GetType(Single()))>
    <KnownType(GetType(Double()))>
    <KnownType(GetType(Decimal()))>
    <KnownType(GetType(Guid()))>
    <KnownType(GetType(employeesSelector()))>
    Public Class ReportParameter

        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property Description As String
        <DataMember>
        Public Property IsHidden As Boolean
        <DataMember>
        Public Property IsMultiValue As Boolean
        <DataMember>
        Public Property IsRangeValue As Boolean
        <DataMember>
        Public Property Type As String
        <DataMember>
        Public Property Value As Object

        <DataMember>
        Public Property TemplateName As String

    End Class

    <DataContract>
    Public Enum ReportPermissionTypes
        <EnumMember> Create
        <EnumMember> Read
        <EnumMember> Update
        <EnumMember> Delete
        <EnumMember> Execute
        <EnumMember> Schedule
        <EnumMember> DeleteExecutions
        <EnumMember> CreateSchedules
        <EnumMember> UpdateSchedules
        <EnumMember> DeleteSchedules
        <EnumMember> CopyReport
    End Enum

    <DataContract>
    Public Class ReportPermissions
        <DataMember>
        Public Property Edit As Boolean '= UPDATE
        <DataMember>
        Public Property Execute As Boolean
        <DataMember>
        Public Property Remove As Boolean '=DELETE

    End Class

#End Region

#Region "Auxiliar Devexpress Compatibility"

    <DataContract>
    Public Enum ReportExportExtensions
        <EnumMember> pdf
        <EnumMember> xlsx
        <EnumMember> xls
        <EnumMember> csv
        <EnumMember> docx
        <EnumMember> text
        <EnumMember> mail
        <EnumMember> image
        <EnumMember> mht
        <EnumMember> rtf
    End Enum

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(EmployeesSelectorTypeConverter))>
    Public Class employeesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class EmployeesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, employeesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New employeesSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(UserFieldsSelectorTypeConverter))>
    Public Class userFieldsSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class UserFieldsSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, userFieldsSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New userFieldsSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class


    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(UserFieldsSelectorRadioBtnTypeConverter))>
    Public Class userFieldsSelectorRadioBtn
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class UserFieldsSelectorRadioBtnTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, userFieldsSelectorRadioBtn).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New userFieldsSelectorRadioBtn With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(ConceptGroupsSelectorTypeConverter))>
    Public Class conceptGroupsSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class ConceptGroupsSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, conceptGroupsSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New conceptGroupsSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(ConceptSelectorTypeConverter))>
    Public Class conceptsSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class ConceptSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, conceptsSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(CausesSelectorTypeConverter))>
    Public Class causesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class CausesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, causesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(IncidencesSelectorTypeConverter))>
    Public Class incidencesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class IncidencesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, incidencesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New incidencesSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ VISUALIZACIÓN Y FORMATO____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(viewAndFormatSelectorTypeConverter))>
    Public Class viewAndFormatSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class viewAndFormatSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, viewAndFormatSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New viewAndFormatSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ TIPO DE ACCESO____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(accessTypeSelectorTypeConverter))>
    Public Class accessTypeSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class accessTypeSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, accessTypeSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New accessTypeSelector With {.value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ PERFILES DE FILTRO____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(filterProfileTypesSelectorTypeConverter))>
    Public Class filterProfileTypesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class filterProfileTypesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, filterProfileTypesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New filterProfileTypesSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ SELECTOR JUSTIFICACIONES EN REGISTRO JORNADA LABORAL____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(filterSelectorCausesRegistroJLTypeConverter))>
    Public Class filterSelectorCausesRegistroJL
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class filterSelectorCausesRegistroJLTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, filterSelectorCausesRegistroJL).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New filterSelectorCausesRegistroJL With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ SELECTOR SALDOS EN REGISTRO JORNADA LABORAL____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(filterSelectorConceptsRegistroJLTypeConverter))>
    Public Class filterSelectorConceptsRegistroJL
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class filterSelectorConceptsRegistroJLTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, filterSelectorConceptsRegistroJL).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New filterSelectorConceptsRegistroJL With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ AÑO Y MES____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(yearAndMonthSelectorTypeConverter))>
    Public Class yearAndMonthSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class yearAndMonthSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, yearAndMonthSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New yearAndMonthSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ RANGO FECHAS AÑO Y MES____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(betweenYearAndMonthSelectorTypeConverter))>
    Public Class betweenYearAndMonthSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class betweenYearAndMonthSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, betweenYearAndMonthSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New betweenYearAndMonthSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ FORMATO____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(formatSelectorTypeConverter))>
    Public Class formatSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class formatSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, formatSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New formatSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' VISUALIZACIÓN Y FORMATO ]____________________________________

    ''[ FILTRAR POR VALORES____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(filterValuesSelectorTypeConverter))>
    Public Class filterValuesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class filterValuesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, filterValuesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New filterValuesSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' FILTRAR POR VALORES ]____________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(ShiftsSelectorTypeConverter))>
    Public Class shiftsSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class ShiftsSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, shiftsSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New shiftsSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class


    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(HolidaysSelectorTypeConverter))>
    Public Class holidaysSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class HolidaysSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, holidaysSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New holidaysSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class


    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(TerminalsSelectorTypeConverter))>
    Public Class terminalsSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class TerminalsSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, terminalsSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New terminalsSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(TasksSelectorTypeConverter))>
    Public Class tasksSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class TasksSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, tasksSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New tasksSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(ZonesSelectorTypeConverter))>
    Public Class zonesSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class ZonesSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, zonesSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New zonesSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''[ PROJECTS VSL____________________________________
    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(projectsVSLSelectorTypeConverter))>
    Public Class projectsVSLSelector
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class projectsVSLSelectorTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, projectsVSLSelector).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New projectsVSLSelector With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(PassportIdentifierTypeConverter))>
    Public Class passportIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class PassportIdentifierTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, passportIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New passportIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' [ IDENTIFICADORES DE SALDO _________________________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(conceptIdentifierTypeConverter))>
    Public Class conceptIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class conceptIdentifierTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, conceptIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New conceptIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' [ IDENTIFICADORES DE SALDO _________________________________________________

    '' [ IDENTIFICADORES DE JUSTIFICACIÓN _________________________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(causeIdentifierTypeConverter))>
    Public Class causeIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class causeIdentifierTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, causeIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New causeIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' [ IDENTIFICADORES DE JUSTIFICACION _________________________________________________

    '' [ IDENTIFICADORES DE INCIDENCIA _________________________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(incidenceIdentifierTypeConverter))>
    Public Class incidenceIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class incidenceIdentifierTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, incidenceIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New incidenceIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' [ IDENTIFICADORES DE INCIDENCIA _________________________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(userFieldIdentifierConverter))>
    Public Class userFieldIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class userFieldIdentifierConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, userFieldIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New userFieldIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    '' [ IDENTIFICADORES DE SALDO _________________________________________________

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(TaskIdentifierTypeConverter))>
    Public Class taskIdentifier
        Public Property Value() As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class TaskIdentifierTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, taskIdentifier).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New taskIdentifier With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

    ''' <summary>
    ''' This class is used as a helper for the Devexpress parameters conversion.
    ''' DO NOT DELETE.
    ''' </summary>
    <DataContract>
    <TypeConverter(GetType(FunctionCallTypeConverter))>
    Public Class functionCall
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    'Use this class to display parameter values on document pages.
    Public Class FunctionCallTypeConverter
        Inherits TypeConverter

        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            If destinationType Is GetType(String) Then
                Return DirectCast(value, functionCall).Value
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            Return destinationType Is GetType(String) OrElse MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            Return sourceType Is GetType(String) OrElse MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            Dim valueString = TryCast(value, String)
            If valueString IsNot Nothing Then
                Return New functionCall With {.Value = valueString}
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

    End Class

#End Region

End Namespace