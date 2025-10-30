Imports System.ComponentModel
Imports System.Globalization
Imports System.Runtime.Serialization
Imports SwaggerWcf.Attributes

Namespace Robotics.VTBase

    <DataContract(Name:="roWCFDate")>
    <Description("Structure that represents a date on API")>
    <SwaggerWcfDefinition(ExternalDocsDescription:="Structure that represents a date on API")>
    Public Class roWCFDate
        Protected Shared DateTimeFormat As String = "yyyy-MM-dd HH:mm:ss zz"

        <SwaggerWcfProperty(Required:=True, Default:="", Description:="String date representation on an specific format(yyyy-MM-dd HH:mm:ss zz)")>
        <DataMember>
        Public Property Data As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal data As String)
            data = data
        End Sub

        Public Sub New(ByVal dDate As DateTime)
            Data = dDate.ToString(DateTimeFormat)
        End Sub

        Public Sub New(ByVal dDate As DateTime?)
            If dDate.HasValue Then
                Data = dDate.Value.ToString(DateTimeFormat)
            End If
        End Sub

        Protected ReadOnly Property HasDate As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(Data)
            End Get
        End Property

        Public Function GetDate() As DateTime
            Try
                Return DateTime.ParseExact(Data, DateTimeFormat, CultureInfo.CurrentCulture)
            Catch
                Return New DateTime()
            End Try
        End Function

    End Class

End Namespace