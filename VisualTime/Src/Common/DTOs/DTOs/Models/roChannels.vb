Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ChannelStatusEnum
        <EnumMember()> Draft
        <EnumMember()> Published
    End Enum

    <DataContract>
    Public Enum ConversationStatusEnum
        <EnumMember()> Pending
        <EnumMember()> Ongoing
        <EnumMember()> Dismissed
        <EnumMember()> Closed
    End Enum

    <DataContract>
    Public Enum ConversationComplexity
        <EnumMember()> Normal
        <EnumMember()> High
    End Enum

    <DataContract>
    Public Enum MessageStatusEnum
        <EnumMember()> Unread
        <EnumMember()> Read
    End Enum

    <DataContract>
    <Serializable>
    Public Class roChannel

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property CreatedBy As Integer

        <DataMember>
        Public Property CreatedByName As String

        <DataMember>
        Public Property CreatedOn As Date

        <DataMember>
        Public Property ModifiedBy As Integer

        <DataMember>
        Public Property ModifiedByName As String

        <DataMember>
        Public Property ModifiedOn As Date

        <DataMember>
        Public Property SubscribedSupervisors As Integer()

        <DataMember>
        Public Property Employees As Integer()

        <DataMember>
        Public Property Groups As Integer()

        <DataMember>
        Public Property PublishedOn As DateTime

        <DataMember>
        Public Property Title As String

        <DataMember>
        Public Property AllowAnonymous As Boolean

        <DataMember>
        Public Property ReceiptAcknowledgment As Boolean

        <DataMember>
        Public Property Deleted As Boolean

        <DataMember>
        Public Property IsComplaintChannel As Boolean

        <DataMember>
        Public Property DeletedBy As Integer

        <DataMember>
        Public Property DeletedByName As String

        <DataMember>
        Public Property DeletedOn As Date

        <DataMember>
        Public Property NewMessages As Integer

        <DataMember>
        Public Property OpenConversations As Integer

        <DataMember>
        Public Property Status As ChannelStatusEnum

        <DataMember>
        Public Property PrivacyPolicy As String

    End Class

    <DataContract>
    <Serializable>
    Public Class roConversation
        Implements ICloneable

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property ReferenceNumber As String

        <DataMember>
        Public Property Channel As roChannel

        <DataMember>
        Public Property CreatedBy As Integer

        <DataMember>
        Public Property CreatedByName As String

        <DataMember>
        Public Property CreatedOn As Date

        <DataMember>
        Public Property Title As String

        <DataMember>
        Public Property IsAnonymous As Boolean

        <DataMember>
        Public Property NewMessages As Integer

        <DataMember>
        Public Property Status As ConversationStatusEnum

        <DataMember>
        Public Property LastStatusChangeBy As Integer

        <DataMember>
        Public Property LastStatusChangeByName As String

        <DataMember>
        Public Property LastStatusChangeOn As Date

        <DataMember>
        Public Property LastMessageHint As String

        <DataMember>
        Public Property LastMessageTimestamp As Date

        <DataMember>
        Public Property Password As String

        <DataMember>
        Public Property ExtraData As roConversationExtraData

        <DataMember>
        Public Property Complexity As ConversationComplexity

        Public Function Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

    <DataContract>
    <Serializable>
    Public Class roConversationExtraData

        <DataMember>
        Public Property FullName As String

        <DataMember>
        Public Property Phone As String

        <DataMember>
        Public Property Mail As String

        <DataMember>
        Public Property Customer As String

        <DataMember>
        Public Property Other As String

    End Class

    <DataContract>
    <Serializable>
    Public Class roMessage
        Implements ICloneable

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Conversation As roConversation

        <DataMember>
        Public Property Body As String

        <DataMember>
        Public Property IsAnonymous As Boolean

        <DataMember>
        Public Property IsResponse As Boolean

        <DataMember>
        Public Property CreatedBy As Integer

        <DataMember>
        Public Property CreatedByName As String

        <DataMember>
        Public Property CreatedOn As Date

        <DataMember>
        Public Property Status As MessageStatusEnum

        Public Function Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

End Namespace