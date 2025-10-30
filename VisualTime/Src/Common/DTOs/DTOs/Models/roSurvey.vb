Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum SurveyStatusEnum
        <EnumMember()> Draft
        <EnumMember()> Online
        <EnumMember()> Expired
        <EnumMember()> Cancelled
    End Enum

    <DataContract>
    <Serializable>
    Public Class roSurvey
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property CreatedBy As Integer
        <DataMember>
        Public Property CreatedByName As String
        <DataMember>
        Public Property CreatedOn As Date
        <DataMember>
        Public Property ModifiedOn As Date
        <DataMember>
        Public Property Employees As Integer()
        <DataMember>
        Public Property Groups As Integer()
        <DataMember>
        Public Property SentOn As DateTime
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property Content As String
        <DataMember>
        Public Property IsMandatory As Boolean
        <DataMember>
        Public Property ResponseMaxPercentage As Integer
        <DataMember>
        Public Property ExpirationDate As DateTime
        <DataMember>
        Public Property Status As SurveyStatusEnum
        <DataMember>
        Public Property CurrentPercentage As Integer
        <DataMember>
        Public Property Progress As String
        <DataMember>
        Public Property Remaining As String
        <DataMember>
        Public Property CurrentEmployeeResponses As roSurveyEmployee()
        <DataMember>
        Public Property Anonymous As Boolean
        <DataMember>
        Public Property AdvancedMode As Boolean
    End Class

    <DataContract>
    Public Class roSurveyForPortal

        Public Sub New()
            Survey = New roSurvey
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Survey As roSurvey

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roSurveyResponsePortal

        Public Sub New()
            Result = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyResponse
        <DataMember>
        Public Property IdSurvey As Integer
        <DataMember>
        Public Property IdEmployee As Integer
        <DataMember>
        Public Property ResponseData As String
        <DataMember>
        Public Property Timestamp As DateTime
    End Class

    <DataContract()>
    Public Class EmployeeSelector
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public EmployeeName As String
        <DataMember()>
        Public Image As String
    End Class

    <DataContract()>
    Public Class GroupSelector
        <DataMember()>
        Public IdGroup As Integer
        <DataMember()>
        Public Name As String

        Public Sub New(iidgroup As Integer, fullgroup As String)
            'By Xavi Iglesias
            IdGroup = iidgroup
            Dim arrayGroups As String()
            If fullgroup Is Nothing Then
                Name = ""
            Else
                arrayGroups = fullgroup.Split("\")
                If arrayGroups.Length > 1 Then
                    Name = arrayGroups(arrayGroups.Length - 1).Trim & " (" & arrayGroups(arrayGroups.Length - 2).Trim & ")"
                Else
                    Name = arrayGroups(arrayGroups.Length - 1).Trim
                End If
            End If

        End Sub

    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyTemplateInfo
        <DataMember>
        Public Property name As String
        <DataMember>
        Public Property json As String
    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyTemplates
        <DataMember>
        Public Property Templates As roSurveyTemplate()
    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyTemplate
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property DefaultTitle As String
        <DataMember>
        Public Property Key As String
        <DataMember>
        Public Property Content As String
    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyResponses
        <DataMember>
        Public Property ResultCount As Integer
        <DataMember>
        Public Property Data As String()
    End Class

    <DataContract>
    <Serializable>
    Public Class roSurveyEmployee
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property EmployeeName As String

        Public Sub New(idEmployee As Integer, sName As String)
            Id = idEmployee
            EmployeeName = sName
        End Sub

    End Class

End Namespace