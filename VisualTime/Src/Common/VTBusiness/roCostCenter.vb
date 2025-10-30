Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.DataLayer.AccessHelper

Namespace CostCenter

    <DataContract()>
    Public Class roCostCenter

#Region "Declarations - Constructor"

        Private oState As roCostCenterState

        Private intID As Integer
        Private strName As String

        Public Sub New()
            Me.oState = New roCostCenterState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roCostCenterState)
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

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Dim tb As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM CostCenters " &
                                       "WHERE [ID] = " & Me.intID.ToString
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Me.strName = tb.Rows(0)("Name")
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCostCenter::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCostCenter::Load")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roCostCenterList

#Region "Declarations - Constructors"

        Private oState As roCostCenterState

        Private Items As ArrayList

        Public Sub New()
            Me.oState = New roCostCenterState()
            Me.Items = New ArrayList
        End Sub

        Public Sub New(ByVal _State As roCostCenterState)
            Me.oState = _State
            Me.Items = New ArrayList
        End Sub

#End Region

#Region "Properties "

        <DataMember()>
        <XmlArray("CostCenters"), XmlArrayItem("roCostCenter", GetType(roCostCenter))>
        Public Property CostCenters() As ArrayList
            Get
                Return Me.Items
            End Get
            Set(ByVal value As ArrayList)
                Me.Items = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function LoadData() As Boolean

            Dim bolRet As Boolean = False

            Me.Items = New ArrayList

            Dim tb As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM CostCenters "

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Me.Items.Add(New roCostCenter(oRow("ID"), Me.oState))
                    Next
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCostCenterList::LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCostCenterList::LoadData")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function GetCostCenters() As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM CostCenters "
                oRet = CreateDataTable(strSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCostCenterList::GetCostCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCostCenterList::GetCostCenters")
            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace