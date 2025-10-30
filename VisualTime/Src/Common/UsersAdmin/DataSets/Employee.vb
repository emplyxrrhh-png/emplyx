Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base

<DataContract>
Public Class Employee

#Region "Declarations - constructor"

    Private intIDEmployee As Integer
    Private strName As String
    Private intObjectValue As Nullable(Of Integer)
    Private intInheritedValue As Nullable(Of Integer)
    Private intInheritedGroupValue As Nullable(Of Integer)
    Private intEditedValue As Nullable(Of Integer)

    Private oPermissions As Generic.List(Of Permission)

    Public Sub New()

    End Sub

    Public Sub New(ByVal row As EmployeesDataSet.EmployeesRow, ByVal oState As roSecurityState)

        'Me.SetLanguage(oState.IDPassport)

        Me.intIDEmployee = row.IDEmployee
        Me.strName = row.Name
        If Not row.IsObjectValueNull Then Me.intObjectValue = row.ObjectValue
        If Not row.IsInheritedValueNull Then Me.intInheritedValue = row.InheritedValue
        If Not row.IsInheritedGroupValueNull Then Me.intInheritedGroupValue = row.InheritedGroupValue
        If Not row.IsEditedValueNull Then Me.intEditedValue = row.EditedValue

        ' Obtenemos la lista de posibles permisos a aplicar al grupo
        oPermissions = New Generic.List(Of Permission)

        oPermissions.Add(Permission.Write)
        oPermissions.Add(Permission.Read)
        oPermissions.Add(Permission.None)

    End Sub

#End Region

#Region "Properties"

    <DataMember>
    Public Property IDEmployee() As Integer
        Get
            Return Me.intIDEmployee
        End Get
        Set(ByVal value As Integer)
            Me.intIDEmployee = value
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
    Public Property InheritedGroupValue() As Nullable(Of Integer)
        Get
            Return Me.intInheritedGroupValue
        End Get
        Set(ByVal value As Nullable(Of Integer))
            Me.intInheritedGroupValue = value
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