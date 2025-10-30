Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class DocumentDTO
        <DataMember>
        Public Property FileName As String
        <DataMember>
        Public Property DocContent As String
        <DataMember>
        Public Property Signers As SignerDTO()
        <DataMember>
        Public Property OrderedSignatures As Boolean
        <DataMember>
        Public Property AdditionalData As String
        <DataMember>
        Public Property DocusignInfo As DocuSignDTO
        <DataMember>
        Public Property NotificationURL As String
        <DataMember>
        Public Property Tag As String
        <DataMember>
        Public Property ExpirationDate As String
        <DataMember>
        Public Property IssuerName As String
    End Class

    <DataContract>
    Public Class DocumentVID_BasicData
        <DataMember>
        Public Property Email As String
        <DataMember>
        Public Property NIF As String
        <DataMember>
        Public Property EmployeeName As String
        <DataMember>
        Public Property PhoneNumber As String
        <DataMember>
        Public Property IssuerName As String
        <DataMember>
        Public Property FileName As String
        <DataMember>
        Public Property FileBytes As Byte()
        <DataMember>
        Public Property GUID As String
        <DataMember>
        Public Property SignerGUI As String
        <DataMember>
        Public Property SignatureViDRemoteURL As String
    End Class

    <DataContract>
    Public Class DocumentVID_PostDocResult
        <DataMember>
        Public Property GUID As String
        <DataMember>
        Public Property SignerGUI As String
        <DataMember>
        Public Property SignatureViDRemoteURL As String
        <DataMember>
        Public Property IsValid As Boolean
    End Class

    <DataContract>
    Public Class DocGuiDTO
        <DataMember>
        Public Property DocGUI As String
    End Class

    <DataContract>
    Public Class SignatureViDRemoteURLDTO
        <DataMember>
        Public Property SignatureViDRemoteURL As String
    End Class

    <DataContract>
    Public Class SignedDocumentReportDTO
        <DataMember>
        Public Property DocContent As String
    End Class

    <DataContract>
    Public Class SignerDTO
        <DataMember>
        Public Property NotificationEmailMessage As SignatureNotificationEmailDTO
        <DataMember>
        Public Property Reminder As SignatureReminderDTO
        <DataMember>
        Public Property PhoneNumber As String
        <DataMember>
        Public Property Language As String
        <DataMember>
        Public Property SignatureType As String
        <DataMember>
        Public Property Form As FormDTO
        <DataMember>
        Public Property isDigitalIdUser As Boolean
        <DataMember>
        Public Property UserNotices As UserNoticeDTO()
        <DataMember>
        Public Property eMail As String
        <DataMember>
        Public Property SignerGUI As String
        <DataMember>
        Public Property PictureURI As String
        <DataMember>
        Public Property DocusignSigner As Boolean
        <DataMember>
        Public Property Visible As VisibleDTO
        <DataMember>
        Public Property DeviceName As String
        <DataMember>
        Public Property NumberID As String
        <DataMember>
        Public Property TypeOfID As String
        <DataMember>
        Public Property SignerName As String
        <DataMember>
        Public Property sendSignedDoc As Boolean
        <DataMember>
        Public Property StampCertificate As StampCertificateDTO
        <DataMember>
        Public Property SkipSignatureEmail As Boolean
    End Class

    <DataContract>
    Public Class SignedDocumentDTO
        <DataMember>
        Public Property FileName As String
        <DataMember>
        Public Property DocContent As String
        <DataMember>
        Public Property AdditionalData As String

    End Class

    <DataContract>
    Public Class SignatureNotificationEmailDTO
        <DataMember>
        Public Property eMailBody As String
        <DataMember>
        Public Property eMailSubject As String
    End Class

    <DataContract>
    Public Class SignatureReminderDTO
        <DataMember>
        Public Property Frequency As Integer
        <DataMember>
        Public Property MaxRetries As Integer
    End Class

    <XmlInclude(GetType(UserNoticeRadioButtonSingleChoiceDTO))>
    <XmlInclude(GetType(UserNoticeCheckboxDTO))>
    <DataContract>
    Public Class UserNoticeDTO
        <DataMember>
        Public UserNoticeName As String
        <DataMember>
        Public UserNoticeObject As Object
        <DataMember>
        Public Property UserNoticeType As TypeOfUserNotice
        <DataMember>
        Public Property PosX As Integer
        <DataMember>
        Public Property PosY As Integer
        <DataMember>
        Public Property SizeX As Integer
        <DataMember>
        Public Property Page As Integer

        <DataContract>
        Public Enum TypeOfUserNotice
            RadioButtonSingleChoice = 0
            CheckBox = 1
        End Enum

    End Class

    <DataContract>
    Public Class VisibleDTO
        <DataMember>
        Public Property Page As Integer
        <DataMember>
        Public Property PosX As Integer
        <DataMember>
        Public Property PosY As Integer
        <DataMember>
        Public Property SizeX As Integer
        <DataMember>
        Public Property SizeY As Integer
        <DataMember>
        Public Property SignatureField As String
        <DataMember>
        Public Property Anchor As String
    End Class

    <DataContract>
    Public Class StampCertificateDTO
        <DataMember>
        Public Property CertificateGUI As String
        <DataMember>
        Public Property Pin As String
        <DataMember>
        Public Property Image As String
    End Class

    <DataContract>
    Public Class DocuSignDTO
        <DataMember>
        Public Property SenderAccountName As String
        <DataMember>
        Public Property SenderAccountPass As String
        <DataMember>
        Public Property Tittle As String
    End Class

    <DataContract>
    Public Class UserNoticeRadioButtonSingleChoiceDTO
        <DataMember>
        Public Property Choice As String()
    End Class

    <DataContract>
    Public Class UserNoticeCheckboxDTO
        <DataMember>
        Public Property Text As String
    End Class

    <DataContract>
    Public Class FormDTO
        <DataMember>
        Public Layout As FormLayoutDTO
        <DataMember>
        Public Sections As List(Of FormSectionDTO)
    End Class

    <DataContract>
    Public Class FormLayoutDTO
        <DataMember>
        Public Property FontFamily As String
        <DataMember>
        Public Property FontSize As String
    End Class

    <DataContract>
    Public Class FormSectionDTO
        <DataMember>
        Public Property Groups As List(Of FormGroupDTO)
    End Class

    <DataContract>
    Public Class FormGroupDTO
        <DataMember>
        Public Property Title As FormTitleDTO
        <DataMember>
        Public Property RadioButtons As List(Of FormRadioButtonDTO)
        <DataMember>
        Public Property CheckBoxes As List(Of FormCheckBoxDTO)
        <DataMember>
        Public Property TextBoxes As List(Of FormTextBoxDTO)
    End Class

    <DataContract>
    Public Class FormTitleDTO
        <DataMember>
        Public Property Text As String
        <DataMember>
        Public Property Position As FormPositionDTO
    End Class

    <DataContract>
    Public Class FormPositionDTO
        <DataMember>
        Public Property PosX As Integer
        <DataMember>
        Public Property PosY As Integer
        <DataMember>
        Public Property SizeX As Integer
        <DataMember>
        Public Property Page As Integer
        <DataMember>
        Public Property Anchor As String
    End Class

    <DataContract>
    Public Class FormRadioButtonDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As FormTitleDTO
        <DataMember>
        Public Property Choices As List(Of FormRadioButtonChoiceDTO)
        <DataMember>
        Public Property Condition As FormConditionDTO
        <DataMember>
        Public Property SelectedChoice As Integer
    End Class

    <DataContract>
    Public Class FormRadioButtonChoiceDTO
        <DataMember>
        Public Property Title As FormTitleDTO
    End Class

    <DataContract>
    Public Class FormConditionDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Result As Integer
    End Class

    <DataContract>
    Public Class FormCheckBoxDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As FormTitleDTO
        <DataMember>
        Public Property Condition As FormConditionDTO
        <DataMember>
        Public Property Response As Boolean
    End Class

    <DataContract>
    Public Class FormTextBoxDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As FormTitleDTO
        <DataMember>
        Public Property MaxLines As Integer
        <DataMember>
        Public Property Condition As FormConditionDTO
        <DataMember>
        Public Property Response As FormTitleDTO
    End Class

    <DataContract>
    Public Class DocumentInfoDTO
        <DataMember>
        Public Property FileName As String
        <DataMember>
        Public Property DocGUI As String
        <DataMember>
        Public Property DocStatus As String
        <DataMember>
        Public Property Downloaded As String
        <DataMember>
        Public Property AdditionalData As String
        <DataMember>
        Public Property Signers As SignerInfoDTO()
    End Class

    <DataContract>
    Public Class SignerInfoDTO
        <DataMember>
        Public Property SignerGUI As String
        <DataMember>
        Public Property SignerName As String
        <DataMember>
        Public Property SignatureStatus As String
        <DataMember>
        Public Property RejectionReason As String
        <DataMember>
        Public Property TypeOfID As String
        <DataMember>
        Public Property NumberID As String
        <DataMember>
        Public Property UserNoticesInfo As UserNoticeInfoDTO()
        <DataMember>
        Public Property FormInfo As FormInfoDTO()
        <DataMember>
        Public Property OperationTime As String
    End Class

    <DataContract>
    Public Class UserNoticeInfoDTO
        <DataMember>
        Public Property UserNoticeName As String
        <DataMember>
        Public Property UserNoticeObject As String
        <DataMember>
        Public Property UserNoticeType As String
    End Class

    <DataContract>
    Public Class FormInfoDTO
        <DataMember>
        Public Property Sections As FormSectionInfoDTO()
    End Class

    <DataContract>
    Public Class FormSectionInfoDTO
        <DataMember>
        Public Property Groups As FormGroupInfoDTO()
    End Class

    <DataContract>
    Public Class FormGroupInfoDTO
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property RadioButtons As FormRadioButtonInfoDTO()
        <DataMember>
        Public Property CheckBoxes As FormCheckBoxInfoDTO()
        <DataMember>
        Public Property TextBoxes As FormTextBoxInfoDTO()
    End Class

    <DataContract>
    Public Class FormRadioButtonInfoDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property Response As String
    End Class

    <DataContract>
    Public Class FormCheckBoxInfoDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property Response As String
    End Class

    <DataContract>
    Public Class FormTextBoxInfoDTO
        <DataMember>
        Public Property Id As String
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property Response As String
    End Class

End Namespace