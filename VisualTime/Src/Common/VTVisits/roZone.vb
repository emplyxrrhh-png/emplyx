Imports System.Runtime.Serialization
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase.roTypes

Namespace VTVisits

    ''' <summary>
    ''' Class that contents zone item
    ''' </summary>
    <DataContract>
    Public Class roZoneItem
        Dim _zoneId As Int32 = 0
        ''' <summary>
        ''' zone id
        ''' </summary>
        ''' <returns>returns id</returns>
        <DataMember()>
        Public Property zoneId As Int32
            Get
                Return _zoneId
            End Get
            Set(value As Int32)
                _zoneId = value
            End Set
        End Property
        Dim _name As String = String.Empty
        ''' <summary>
        ''' zone name
        ''' </summary>
        ''' <returns>returns the name</returns>
        <DataMember()>
        Public Property zoneName As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        ''' <summary>
        ''' Parse Datarow on properties zone
        ''' </summary>
        Public Sub parseRow()

        End Sub

        Public Sub New()

        End Sub

        Public Sub New(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("ID") Then zoneId = Any2Integer(row.Item("ID"))
                If row.Table.Columns.Contains("Name") Then zoneName = Any2String(row.Item("Name"))
            Catch ex As Exception

            End Try
        End Sub

    End Class

    ''' <summary>
    ''' Class container with all zone items
    ''' </summary>
    <DataContract>
    Public Class roZoneList
        Private sResult As String = "NoError"
        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Dim _zoneList As List(Of roZoneItem)
        <DataMember()>
        Public Property zoneList As List(Of roZoneItem)
            Get
                Return _zoneList
            End Get
            Set(value As List(Of roZoneItem))
                _zoneList = value
            End Set
        End Property

        Public Sub New()
            _zoneList = New List(Of roZoneItem)()
        End Sub

        ''' <summary>
        ''' Parse Datarow on list zone
        ''' </summary>
        ''' <param name="dataRows">datarow collection to parse</param>
        Public Sub parseRows(ByVal dataRows As DataRowCollection)
            For Each row As DataRow In dataRows
                _zoneList.Add(New roZoneItem(row))
            Next
        End Sub

    End Class

    ''' <summary>
    ''' Zone class
    ''' </summary>
    <DataContract>
    Public Class roZone

        Private oState As roZoneState
        Private _zoneList As roZoneList
        <DataMember()>
        Public Property Zone As roZoneList
            Get
                Return _zoneList
            End Get
            Set(value As roZoneList)
                _zoneList = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roZoneState, Optional ByVal bAudit As Boolean = False)
            Try
                If Not _State Is Nothing Then oState = _State Else oState = New roZoneState()
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception
            End Try

        End Sub

        ''' <summary>
        ''' Return zone by working type
        ''' </summary>
        ''' <param name="workingZone">working typw to filter</param>
        ''' <param name="oActiveTransaction">check if query belongs to transaction</param>
        ''' <param name="bAudit">check if audit operation</param>
        ''' <returns></returns>
        Public Function FindAllByWorkingZone(ByVal workingZone As Int32, Optional ByVal bAudit As Boolean = False) As Boolean
            _zoneList = New roZoneList
            Try

                Dim parameters = New List(Of CommandParameter) From
                {
                    New CommandParameter("@workingZone", CommandParameter.ParameterType.tInt, workingZone)
                }

                Dim sSQL = "@SELECT# ID,Name from Zones where IsWorkingZone=@workingZone"

                Dim tb As DataTable = CreateDataTable(sSQL, parameters)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    _zoneList.parseRows(tb.Rows)
                End If
            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rozone::zoneListByWorkingOut::" + ex.Message)

                _zoneList.result = roOptionsState.ResultEnum.Exception
                Return False
            End Try
            Return True
        End Function

    End Class

    ''' <summary>
    ''' Zone State
    ''' </summary>
    <DataContract()>
    Public Class roZoneState
        Inherits roBusinessState

        <Flags()>
        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            SqlError
            NotFound
        End Enum

#Region "Declarations - Constructor"

        Private intResult As ResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Visits", "Visits")
            Me.intResult = ResultEnum.NoError
            'MyBase.LastAccessUpdate = New Date(1970, 1, 1)
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("VTBusiness.Visits", "Visits", _IDPassport)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("VTBusiness.Visits", "Visits", _IDPassport, _Context)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visits", "Visits", _IDPassport, , _ClientAddress)
            Me.intResult = ResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("VTBusiness.Visits", "Visits", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property result() As ResultEnum
            Get
                Return Me.intResult
            End Get
            Set
                Me.intResult = Value
                If Me.intResult <> ResultEnum.NoError And Me.intResult <> ResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

    End Class

End Namespace