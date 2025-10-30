Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

<DataContract>
Public Class EmployeeGroup

#Region "Declarations - constructor"

    Private intIDGroup As Integer
    Private intIDParentGroup As Nullable(Of Integer)
    Private strName As String
    Private intMaxConfigurable As Nullable(Of Integer)
    Private intObjectValue As Nullable(Of Integer)
    Private intInheritedValue As Nullable(Of Integer)
    Private intEditedValue As Nullable(Of Integer)
    Private intChildsValue As Nullable(Of Integer)
    Private intChildsMaxConfigurable As Nullable(Of Integer)

    Private oPermissions As Generic.List(Of Permission)

    Public Sub New()

    End Sub

    Public Sub New(ByVal row As EmployeeGroupsDataSet.GroupsRow, ByVal oState As roSecurityState)

        'Me.SetLanguage(oState.IDPassport)

        Me.intIDGroup = row.IDGroup
        If Not row.IsIDParentGroupNull Then Me.intIDParentGroup = row.IDParentGroup
        Me.strName = row.Name
        If Not row.IsMaxConfigurableNull Then Me.intMaxConfigurable = row.MaxConfigurable
        If Not row.IsObjectValueNull Then Me.intObjectValue = row.ObjectValue
        If Not row.IsInheritedValueNull Then Me.intInheritedValue = row.InheritedValue
        If Not row.IsEditedValueNull Then Me.intEditedValue = row.EditedValue
        If Not row.IsChildsValueNull Then Me.intChildsValue = row.ChildsValue
        If Not row.IsChildsMaxConfigurableNull Then Me.intChildsMaxConfigurable = row.ChildsMaxConfigurable

        ' Obtenemos la lista de posibles permisos a aplicar al grupo
        oPermissions = New Generic.List(Of Permission)

        oPermissions.Add(Permission.Write)
        oPermissions.Add(Permission.Read)
        oPermissions.Add(Permission.None)

        If row.MaxConfigurable < Permission.Write Then oPermissions.Remove(Permission.Write)
        If row.MaxConfigurable < Permission.Read Then oPermissions.Remove(Permission.Read)

    End Sub

#End Region

#Region "Properties"

    <DataMember>
    Public Property IDGroup() As Integer
        Get
            Return Me.intIDGroup
        End Get
        Set(ByVal value As Integer)
            Me.intIDGroup = value
        End Set
    End Property

    <DataMember>
    Public Property IDParentGroup() As Nullable(Of Integer)
        Get
            Return Me.intIDParentGroup
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intIDParentGroup = value
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
    Public Property ChildsValue() As Nullable(Of Integer)
        Get
            Return Me.intChildsValue
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intChildsValue = value
        End Set
    End Property

    <DataMember>
    Public Property ChildsMaxConfigurable() As Nullable(Of Integer)
        Get
            Return Me.intChildsMaxConfigurable
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intChildsMaxConfigurable = value
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

End Class