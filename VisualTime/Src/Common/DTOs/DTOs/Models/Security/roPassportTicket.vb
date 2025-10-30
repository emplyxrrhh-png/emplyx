Imports System.Runtime.Serialization


Namespace Robotics.Base.DTOs


    <DataContract>
    Public Class roPassportTicket

#Region " Declarations / Constructor "

        Public Sub New()
            IDEmployee = Nothing
            Description = String.Empty
            Name = String.Empty
            Email = String.Empty
            EnabledVTDesktop = True
            EnabledVTPortal = True
            EnabledVTPortalApp = True
            EnabledVTVisits = True
        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' Returns the passport's ID.
        ''' </summary>
        <DataMember>
        Public Property ID As Integer

        ''' <summary>
        ''' Returns the passport's IDParentPassport.
        ''' </summary>
        <DataMember>
        Public Property IDParentPassport As Nullable(Of Integer)

        <DataMember>
        Public Property IDUser() As Nullable(Of Integer)

        ''' <summary>
        ''' Returns the passport's name.
        ''' </summary>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Returns the passport's description.
        ''' </summary>
        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Returns the passport's description.
        ''' </summary>
        <DataMember>
        Public Property IsSupervisor As Boolean

        ''' <summary>
        ''' Returns if passport is active.
        ''' </summary>
        <DataMember>
        Public Property IsActivePassport As Boolean

        ''' <summary>
        ''' Returns the passport's email address if it is not a group.
        ''' </summary>
        <DataMember>
        Public Property Email As String

        ''' <summary>
        ''' Returns the ID of the employee associated with the passport, or nothing.
        ''' </summary>
        <DataMember>
        Public Property IDEmployee As Nullable(Of Integer)

        <DataMember>
        Public Property Language As roPassportLanguage

        <DataMember()>
        Public Property EnabledVTDesktop As Boolean

        <DataMember()>
        Public Property EnabledVTPortal As Boolean

        <DataMember()>
        Public Property EnabledVTPortalApp As Boolean

        <DataMember()>
        Public Property EnabledVTVisits As Boolean

        <DataMember()>
        Public Property EnabledVTVisitsApp As Boolean

        <DataMember()>
        Public Property IDGroupFeature As Integer

        <DataMember()>
        Public Property IsBloquedAccessApp As Boolean

        <DataMember()>
        Public Property IsTemporayBloqued As Boolean

        <DataMember()>
        Public Property IsExpiredPwd As Boolean

        <DataMember()>
        Public Property IsSSO As Boolean

        <DataMember()>
        Public Property AuthCredential As String

        <DataMember()>
        Public Property CegidIdCredential As String

        <DataMember()>
        Public ReadOnly Property IsPrivateUser As Boolean
            Get
                Return Me.Description.Contains("@@ROBOTICS@@")
            End Get
        End Property
#End Region


    End Class

End Namespace