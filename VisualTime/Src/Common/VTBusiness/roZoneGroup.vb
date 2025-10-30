Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper

Namespace Zone

    <DataContract()>
    Public Class roZoneGroup

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private intIDImage As Integer

        Public Sub New()
            Me.oState = New roZoneState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roZoneState)
            Me.oState = _State
            Me.ID = _ID
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
                Me.Load()
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property IDImage() As Integer
            Get
                Return Me.intIDImage
            End Get
            Set(ByVal value As Integer)
                Me.intIDImage = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Dim tb As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM ZoneGroups " &
                                       "WHERE [ID] = " & Me.intID.ToString
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("Name")) Then Me.strName = tb.Rows(0)("Name")
                    If Not IsDBNull(tb.Rows(0)("Description")) Then Me.strDescription = tb.Rows(0)("Description")
                    If Not IsDBNull(tb.Rows(0)("IDImage")) Then Me.intIDImage = tb.Rows(0)("IDImage")
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roZoneGroup::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roZoneGroup::Load")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace