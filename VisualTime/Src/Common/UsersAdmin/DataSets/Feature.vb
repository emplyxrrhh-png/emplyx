Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

<DataContract>
Public Class Feature

#Region "Declarations - Constructor"

    Private intID As Integer
    Private intIDParent As Nullable(Of Integer)
    Private strAlias As String
    Private strName As String
    Private strType As String
    Private bolIsGroup As Boolean
    Private strPermissionTypes As String
    Private intMaxConfigurable As Nullable(Of Integer)
    Private intObjectValue As Nullable(Of Integer)
    Private intInheritedValue As Nullable(Of Integer)
    Private intEditedValue As Nullable(Of Integer)
    Private strDescription As String
    Private bolAppHasPermissionsOverEmployees As Boolean

    Private oPermissions As Generic.List(Of Permission)

    Private oLanguage As roLanguage = Nothing

    Public Sub New()

    End Sub

    Public Sub New(ByVal row As FeaturesDataSet.FeaturesRow, ByVal oState As roSecurityState)

        Me.SetLanguage(oState.IDPassport)

        Me.intID = row.ID
        If Not row.IsIDParentNull Then Me.intIDParent = row.IDParent
        Me.strAlias = row._Alias
        Dim strLanguageKey As String = "Features." & CStr(IIf(row.Type = "U", "", row.Type & ".")) & row._Alias
        Me.strName = Me.oLanguage.Translate(strLanguageKey & ".Name", "")

        If Me.strName.ToUpper.Contains("UNDEFINED LANGUAGE") Or Me.strName.ToUpper.Contains("NOENTRY") Or Me.strName.ToUpper.Contains("NOTFOUND") Then
            Me.strName = row.Name
        End If

        Me.strDescription = Me.oLanguage.Translate(strLanguageKey & ".Description", "")

        If Me.strDescription.ToUpper.Contains("UNDEFINED LANGUAGE") Or Me.strDescription.ToUpper.Contains("NOENTRY") Or Me.strDescription.ToUpper.Contains("NOTFOUND") Then
            Me.strDescription = row.Name
        End If

        Me.strType = row.Type
        Me.bolIsGroup = row.IsGroup
        Me.strPermissionTypes = row.PermissionTypes
        If Not row.IsMaxConfigurableNull Then Me.intMaxConfigurable = row.MaxConfigurable
        If Not row.IsObjectValueNull Then Me.intObjectValue = row.ObjectValue
        If Not row.IsInheritedValueNull Then Me.intInheritedValue = row.InheritedValue
        If Not row.IsEditedValueNull Then Me.intEditedValue = row.EditedValue
        'If Not row.IsDescriptionNull Then Me.strDescription = row.Description
        If Not row.IsAppHasPermissionsOverEmployeesNull Then Me.bolAppHasPermissionsOverEmployees = row.AppHasPermissionsOverEmployees

        ' Obtenemos la lista de posibles permisos a aplicar a la funcionalidad
        oPermissions = New Generic.List(Of Permission)

        oPermissions.Add(Permission.Admin)
        oPermissions.Add(Permission.Write)
        oPermissions.Add(Permission.Read)
        oPermissions.Add(Permission.None)

        If row.MaxConfigurable < Permission.Admin Or Not row.PermissionTypes.Contains("A") Then oPermissions.Remove(Permission.Admin)
        If row.MaxConfigurable < Permission.Write Or Not row.PermissionTypes.Contains("W") Then oPermissions.Remove(Permission.Write)
        If row.MaxConfigurable < Permission.Read Or Not row.PermissionTypes.Contains("R") Then oPermissions.Remove(Permission.Read)

    End Sub

#End Region

#Region "Properties"

    <DataMember>
    Public Property ID() As Integer
        Get
            Return Me.intID
        End Get
        Set(ByVal value As Integer)
            Me.intID = value
        End Set
    End Property

    <DataMember>
    Public Property IDParent() As Nullable(Of Integer)
        Get
            Return Me.intIDParent
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intIDParent = value
        End Set
    End Property

    <DataMember>
    Public Property [Alias]() As String
        Get
            Return Me.strAlias
        End Get
        Set(ByVal value As String)
            Me.strAlias = value
        End Set
    End Property

    <DataMember>
    Public Property Name() As String
        Get
            Return Me.strName
        End Get
        Set(ByVal value As String)
            Me.strName = value
        End Set
    End Property

    <DataMember>
    Public Property Type() As String
        Get
            Return Me.strType
        End Get
        Set(ByVal value As String)
            Me.strType = value
        End Set
    End Property

    <DataMember>
    Public Property IsGroup() As Boolean
        Get
            Return Me.bolIsGroup
        End Get
        Set(ByVal value As Boolean)
            Me.bolIsGroup = value
        End Set
    End Property

    <DataMember>
    Public Property PermissionTypes() As String
        Get
            Return Me.strPermissionTypes
        End Get
        Set(ByVal value As String)
            Me.strPermissionTypes = value
        End Set
    End Property

    <DataMember>
    Public Property MaxConfigurable() As Nullable(Of Integer)
        Get
            Return Me.intMaxConfigurable
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intMaxConfigurable = value
        End Set
    End Property

    <DataMember>
    Public Property ObjectValue() As Nullable(Of Integer)
        Get
            Return Me.intObjectValue
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intObjectValue = value
        End Set
    End Property

    <DataMember>
    Public Property InheritedValue() As Nullable(Of Integer)
        Get
            Return Me.intInheritedValue
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intInheritedValue = value
        End Set
    End Property

    <DataMember>
    Public Property EditedValue() As Nullable(Of Integer)
        Get
            Return Me.intEditedValue
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intEditedValue = value
        End Set
    End Property

    <DataMember>
    Public Property Description() As String
        Get
            Return Me.strDescription
        End Get
        Set(ByVal value As String)
            Me.strDescription = value
        End Set
    End Property

    <DataMember>
    Public Property AppHasPermissionsOverEmployees() As Boolean
        Get
            Return Me.bolAppHasPermissionsOverEmployees
        End Get
        Set(ByVal value As Boolean)
            Me.bolAppHasPermissionsOverEmployees = value
        End Set
    End Property

    <DataMember>
    Public Property Permissions() As Generic.List(Of Permission)
        Get
            Return Me.oPermissions
        End Get
        Set(ByVal value As Generic.List(Of Permission))
            Me.oPermissions = value
        End Set
    End Property

#End Region

#Region "Methods"

    Private Sub SetLanguage(ByVal idPassport As Integer)

        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguage()
            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(idPassport, LoadType.Passport)
            Dim strLanguageKey As String
            If oPassport IsNot Nothing Then
                strLanguageKey = oPassport.Language.Key
            Else
                Dim oSettings As New roSettings()
                strLanguageKey = CStr(oSettings.GetVTSetting(eKeys.DefaultLanguage))
            End If
            Me.oLanguage.SetLanguageReference("UserAdminService", strLanguageKey)
        End If

    End Sub


    Private Function IsUsersAdminForCurrentUser(ByVal row As FeaturesDataSet.FeaturesRow, ByVal idPassport As Integer, ByVal idCurrentPassport As Integer) As Boolean

        Dim bolRet As Boolean = False

        ' Make sure we don't remove UsersAdmin for current user or for a parent group.
        If row IsNot Nothing AndAlso row._Alias = UsersAdmin.Business.Features.UsersAdmin Then
            Dim oManager As New roPassportManager
            Dim User As roPassportTicket = roPassportManager.GetPassportTicket(idCurrentPassport, LoadType.Passport)
            If User.ID = idPassport OrElse oManager.GetParents(User.ID).Contains(idPassport) Then
                bolRet = True
            End If
        End If

        Return bolRet

    End Function

#End Region

End Class