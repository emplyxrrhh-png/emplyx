Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract,
        KnownType(GetType(roDocumentsRequest))>
    Public Class roRequestTypes
        <DataMember>
        Public Property RequestType As Object
    End Class

    <DataContract, KnownType(GetType(roWsState))>
    Public Class roDocumentsRequest
        <DataMember>
        Public Property idActivity As Integer
        <DataMember>
        Public Property idDocument As Integer
        <DataMember>
        Public Property idCompany As Integer
        <DataMember>
        Public Property idContract As String
        <DataMember>
        Public Property idEmployee As Integer
        <DataMember>
        Public Property bAudit As Boolean
        <DataMember>
        Public Property idActivities() As Integer()
        <DataMember>
        Public Property lstContracts() As String()
        <DataMember>
        Public Property oState As roWsState
        <DataMember>
        Public Property oDocumentTemplate As roDocumentTemplate
        <DataMember>
        Public Property oDocument As roDocument
        <DataMember>
        Public Property docType As DocumentType
    End Class

End Namespace