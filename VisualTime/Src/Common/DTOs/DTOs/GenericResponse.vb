Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<DataContract>
Public Class roGenericResponse(Of T)
    <DataMember>
    Public Property Value As T
    <DataMember>
    Public Property Status As Integer
End Class

<DataContract>
Public Class roGenericVtResponse(Of T)
    <DataMember>
    Public Property Value As T
    <DataMember>
    Public Property Status As roWsState
End Class

<DataContract(Name:="Requests")>
<SwaggerWcfDefinition(ExternalDocsDescription:="Information related to the requests of a user.")>
Public Class roWSResponse(Of T)
    <DataMember>
    <Description("Result from Rest api request")>
    <SwaggerWcfProperty(Required:=True, Default:="", Description:="Result from Rest api request")>
    Public Property Value As T
    <DataMember>
    <Description("Result code that indicates the request status")>
    Public Property Status As Core.DTOs.ReturnCode
    <DataMember>
    <Description("Additional info related to result code")>
    <SwaggerWcfProperty(Required:=True, Default:="", Description:="Additional info related to result code")>
    Public Property Text As String
    <DataMember>
    <Description("Version of the api used")>
    <SwaggerWcfProperty(Required:=True, Default:="0", Description:="Version of the api used")>
    Public Property ApiVersion As String

End Class