Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper

Namespace Zone

    <DataContract()>
    Public Class roZoneWorkCenter

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private strName As String

        Public Sub New()
            Me.oState = New roZoneState()
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roZoneState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roZoneState)
                Me.oState = value
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

#End Region

#Region "Methods"

#Region "Helper methods"

        Public Shared Function GetZoneWorkCentersDataTable(ByVal strOrderBy As String, ByVal _State As roZoneState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# distinct(enterprise) as Name from EmployeeContracts where enterprise is not null and enterprise <> '' ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "enterprise ASC"
                End If

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roZoneWorkCenter::GetZonesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZoneWorkCenter::GetZonesDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace