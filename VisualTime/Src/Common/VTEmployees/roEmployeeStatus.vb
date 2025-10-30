Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roTypes

Namespace Employee

    <DataContract>
    <Serializable>
    Public Class roEmployeeStatus

#Region "Declarations - Constructors"

        <NonSerialized()>
        Private oState As roEmployeeState

        Private intIDEmployee As Integer
        Private xLastPunch As Nullable(Of DateTime)
        Private intIDCause As Integer
        Private strType As String
        Private bolIsPresent As Boolean
        Private xShiftDate As Nullable(Of DateTime)
        Private xBeginMandatory As Nullable(Of DateTime)
        Private bolVerified As Boolean

        Public Sub New()

            Me.oState = New roEmployeeState
            Me.intIDEmployee = -1

        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _State As roEmployeeState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intIDEmployee = _IDEmployee

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roEmployeeState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property IDEmplotee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property LastPunch() As Nullable(Of DateTime)
            Get
                Return Me.xLastPunch
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xLastPunch = value
            End Set
        End Property

        <DataMember>
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property Type_() As String
            Get
                Return Me.strType
            End Get
            Set(ByVal value As String)
                Me.strType = value
            End Set
        End Property

        <DataMember>
        Public Property IsPresent() As Boolean
            Get
                Return Me.bolIsPresent
            End Get
            Set(ByVal value As Boolean)
                Me.bolIsPresent = value
            End Set
        End Property

        <DataMember>
        Public Property ShiftDate() As Nullable(Of DateTime)
            Get
                Return Me.xShiftDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xShiftDate = value
            End Set
        End Property

        <DataMember>
        Public Property BeginMandatory() As Nullable(Of DateTime)
            Get
                Return Me.xBeginMandatory
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xBeginMandatory = value
            End Set
        End Property

        <DataMember>
        Public Property Verified() As Boolean
            Get
                Return Me.bolVerified
            End Get
            Set(ByVal value As Boolean)
                Me.bolVerified = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# * FROM EmployeeStatus WHERE IDEmployee = " & Me.intIDEmployee.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("LastPunch")) Then Me.xLastPunch = oRow("LastPunch")
                    Me.intIDCause = Any2Integer(oRow("IDCause"))
                    Me.strType = Any2String(oRow("Type"))
                    Me.bolIsPresent = Any2Boolean(oRow("IsPresent"))
                    If Not IsDBNull(oRow("ShiftDate")) Then Me.xShiftDate = oRow("ShiftDate")
                    If Not IsDBNull(oRow("BeginMandatory")) Then Me.xBeginMandatory = oRow("BeginMandatory")
                    Me.bolVerified = Any2Boolean(oRow("Verified"))

                    bolRet = True

                    If _Audit Then
                        ' ***

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEmployeeStatus::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEmployeeStatus::Load")
            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intIDEmployee = oRow("IDEmployee")
                If Not IsDBNull(oRow("LastPunch")) Then Me.xLastPunch = oRow("LastPunch")
                Me.intIDCause = Any2Integer(oRow("IDCause"))
                Me.strType = Any2String(oRow("Type"))
                Me.bolIsPresent = Any2Boolean(oRow("IsPresent"))
                If Not IsDBNull(oRow("ShiftDate")) Then Me.xShiftDate = oRow("ShiftDate")
                If Not IsDBNull(oRow("BeginMandatory")) Then Me.xBeginMandatory = oRow("BeginMandatory")
                Me.bolVerified = Any2Boolean(oRow("Verified"))

                bolRet = True

                If _Audit Then
                    ' ***

                End If

            End If

            Return bolRet

        End Function

#End Region

    End Class

End Namespace