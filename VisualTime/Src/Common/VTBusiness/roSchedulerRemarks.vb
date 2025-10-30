Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Scheduler

    Public Enum RemarkCompare
        <EnumMember> Equal
        <EnumMember> Minor
        <EnumMember> MinorEqual
        <EnumMember> Major
        <EnumMember> MajorEqual
        <EnumMember> Distinct
    End Enum

    ''' <summary>
    ''' Configuración de los resaltes (filtros establecidos (Lista de roRemarks))
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Class roSchedulerRemarks

#Region "Declarations - Constructor"

        Private oState As roSchedulerState

        Private intIDPassport As Integer
        Private oRemarks As Generic.List(Of roCalendarRemark)

        Public Sub New()
            Me.oState = New roSchedulerState
            Me.intIDPassport = -1
            Me.oRemarks = New Generic.List(Of roCalendarRemark)
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _State As roSchedulerState)
            Me.oState = _State
            Me.intIDPassport = _IDPassport
            Me.oRemarks = New Generic.List(Of roCalendarRemark)
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roSchedulerState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roSchedulerState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDPassport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property
        <DataMember()>
        Public Property Remarks() As Generic.List(Of roCalendarRemark)
            Get
                Return Me.oRemarks
            End Get
            Set(ByVal value As Generic.List(Of roCalendarRemark))
                Me.oRemarks = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            '
            '

            Try

                'If oActiveTransaction IsNot Nothing Then
                '    oCn = oActiveTransaction.Connection
                '    bolCloseCn = False
                'Else
                '    oCn = CreateConnection()
                '    bolCloseCn = True
                'End If

                Dim oContext As CContext = Nothing
                Try
                    oContext = WLHelper.GetContext(Me.intIDPassport)
                Catch
                    oContext = Nothing
                    Me.oState.Result = SchedulerResultEnum.InvalidIDPassport
                End Try

                If oContext IsNot Nothing Then

                    Me.oRemarks = New Generic.List(Of roCalendarRemark)

                    Dim oConf As New roCollection(oContext.ConfXml)

                    Dim oRemarks As roCollection = oConf.Node("SchedulerRemarks")
                    If oRemarks IsNot Nothing Then

                        Dim intTotalRemarks As Integer = oRemarks.Item("TotalRemarks")
                        Dim strRemark As String
                        Dim RemarkConfig() As String
                        For n As Integer = 0 To intTotalRemarks - 1
                            strRemark = Any2String(oRemarks.Item("Remark" & n.ToString))
                            RemarkConfig = strRemark.Split(";")
                            If RemarkConfig.Count = 4 Then
                                Me.oRemarks.Add(New roCalendarRemark(RemarkConfig(0), RemarkConfig(1), RemarkConfig(2), RemarkConfig(3)))
                            End If
                        Next

                    End If

                    bolRet = True

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Load")
            Finally
                '
            End Try

            Return bolRet

        End Function

        Public Function Save() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oContext As CContext = Nothing
                Try
                    oContext = WLHelper.GetContext(Me.intIDPassport)
                Catch
                    oContext = Nothing
                    Me.oState.Result = SchedulerResultEnum.InvalidIDPassport
                End Try

                If oContext IsNot Nothing Then

                    Dim RemarksConfig As New roCollection

                    RemarksConfig.Add("TotalRemarks", Me.oRemarks.Count)
                    For n As Integer = 0 To Me.oRemarks.Count - 1
                        RemarksConfig.Add("Remark" & n.ToString, Me.oRemarks(n).IDCause.ToString & ";" & Me.oRemarks(n).Compare & ";" & Format(Me.oRemarks(n).Value, "HH:mm") & ";" & Me.oRemarks(n).Color.ToString)
                    Next

                    Dim oConf As New roCollection(oContext.ConfXml)
                    oConf.Remove("SchedulerRemarks")
                    oConf.Add("SchedulerRemarks", RemarksConfig)

                    oContext.ConfXml = oConf.XML

                    WLHelper.SetContext(Me.intIDPassport, oContext)

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Save")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oContext As CContext = Nothing
                Try
                    oContext = WLHelper.GetContext(Me.intIDPassport)
                Catch
                    oContext = Nothing
                    Me.oState.Result = SchedulerResultEnum.InvalidIDPassport
                End Try

                If oContext IsNot Nothing Then

                    Dim oConf As New roCollection(oContext.ConfXml)
                    oConf.Remove("SchedulerRemarks")

                    WLHelper.SetContext(Me.intIDPassport, oContext)

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSchedulerRemarks::Delete")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetCalendarSchedulerRemarks(ByVal _IDEmployee As Integer, ByVal xDate As Date, ByVal remarks As Scheduler.roSchedulerRemarks, ByVal _State As roSchedulerState, ByVal tbDailyCauses1 As DataTable, ByVal tbDailyCauses2 As DataTable, ByVal tbDailyCauses3 As DataTable) As Boolean()

            Dim tbRet() As Boolean = {False, False, False}

            Try

                ' Obtenemos la configuración de resaltes para el pasaporte activo
                Dim oSchedulerRemarks As Scheduler.roSchedulerRemarks = remarks
                If oSchedulerRemarks Is Nothing Then oSchedulerRemarks = New Scheduler.roSchedulerRemarks(_State.IDPassport, _State)

                If oSchedulerRemarks IsNot Nothing AndAlso _State.Result = SchedulerResultEnum.NoError Then
                    Dim oRows() As DataRow = Nothing

                    ' Creamos la tabla y definimos su estructura

                    ' Recorremos todos los resaltes
                    Dim curIndex As Integer = 0
                    For Each oRemark As Scheduler.roCalendarRemark In oSchedulerRemarks.Remarks
                        If curIndex < 3 And oRemark.IDCause >= 0 Then
                            Select Case curIndex
                                Case 0 : If tbDailyCauses1 IsNot Nothing Then oRows = tbDailyCauses1.Select("Date = '" & Format(xDate, "yyyy/MM/dd") & "'")
                                Case 1 : If tbDailyCauses2 IsNot Nothing Then oRows = tbDailyCauses2.Select("Date = '" & Format(xDate, "yyyy/MM/dd") & "'")
                                Case 2 : If tbDailyCauses3 IsNot Nothing Then oRows = tbDailyCauses3.Select("Date = '" & Format(xDate, "yyyy/MM/dd") & "'")
                            End Select

                            If oRows IsNot Nothing AndAlso oRows.Length > 0 Then
                                tbRet(curIndex) = True
                            Else
                                tbRet(curIndex) = False
                            End If
                            curIndex = curIndex + 1
                        End If
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roSchedulerRemarks::GetCalendarSchedulerRemarks")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roSchedulerRemarks::GetCalendarSchedulerRemarks")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function GetSchedulerRemarks(ByVal _IDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal _State As roSchedulerState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                ' Obtenemos la configuración de resaltes para el pasaporte activo
                Dim oSchedulerRemarks As New Scheduler.roSchedulerRemarks(_State.IDPassport, _State)

                If oSchedulerRemarks IsNot Nothing AndAlso _State.Result = SchedulerResultEnum.NoError Then

                    If xBegin <= xEnd Then ' Verificamos que el periodo de fechas sea correcto.

                        Dim tbDailyCauses As DataTable
                        Dim strSQL As String
                        Dim strCompare As String

                        Dim oNewRow As DataRow
                        Dim oDateRows() As DataRow

                        ' Creamos la tabla y definimos su estructura
                        tbRet = New SchedulerRemarksDataSet.SchedulerRemarksDataTable()
                        'tbRet = New DataTable
                        'Dim oColumn As DataColumn = New DataColumn("Date", GetType(Date)) : oColumn.AllowDBNull = False
                        'tbRet.Columns.Add(oColumn)
                        'oColumn = New DataColumn("Colour", GetType(Integer)) : oColumn.AllowDBNull = True
                        'tbRet.Columns.Add(oColumn)
                        'oColumn = New DataColumn("IDCause", GetType(Integer)) : oColumn.AllowDBNull = True
                        'tbRet.Columns.Add(oColumn)
                        'oColumn = New DataColumn("Compare", GetType(String)) : oColumn.AllowDBNull = True
                        'tbRet.Columns.Add(oColumn)
                        'oColumn = New DataColumn("Value", GetType(DateTime)) : oColumn.AllowDBNull = True
                        'tbRet.Columns.Add(oColumn)

                        ' Recorremos todos los resaltes
                        For Each oRemark As Scheduler.roCalendarRemark In oSchedulerRemarks.Remarks

                            Select Case oRemark.Compare
                                Case Scheduler.RemarkCompare.Equal
                                    strCompare = "="
                                Case Scheduler.RemarkCompare.Minor
                                    strCompare = "<"
                                Case Scheduler.RemarkCompare.MinorEqual
                                    strCompare = "<="
                                Case Scheduler.RemarkCompare.Major
                                    strCompare = ">"
                                Case Scheduler.RemarkCompare.MajorEqual
                                    strCompare = ">="
                                Case Scheduler.RemarkCompare.Distinct
                                    strCompare = "<>"
                                Case Else
                                    strCompare = "="
                            End Select

                            Dim oTotalResult As Decimal = Math.Round(CDate(oRemark.Value).TimeOfDay.TotalHours, 2)
                            strSQL = "@SELECT# Date " &
                                     "FROM DailyCauses " &
                                     "WHERE IDEmployee = " & _IDEmployee.ToString & " AND IDCause = " & oRemark.IDCause & " " &
                                     "GROUP BY Date " &
                                     "HAVING Date BETWEEN " & Any2Time(xBegin).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " AND " &
                                     "CONVERT(numeric(9,2), SUM(Value)) " & strCompare & " CONVERT(numeric(9,2)," & CStr(oTotalResult).Replace(",", ".") & ")"

                            tbDailyCauses = CreateDataTable(strSQL, )
                            If tbDailyCauses IsNot Nothing Then

                                For Each oRow As DataRow In tbDailyCauses.Rows
                                    ' Miramos si ya existe un registro con la misma fecha y si no existe lo añadimos con la configuración del color de resalte correspondiente
                                    oDateRows = tbRet.Select("Date = '" & Format(CDate(oRow("Date")), "yyyy/MM/dd") & "'")
                                    If oDateRows.Length = 0 Then
                                        oNewRow = tbRet.NewRow
                                        oNewRow("Date") = oRow("Date")
                                        oNewRow("Colour") = oRemark.Color
                                        oNewRow("IDCause") = oRemark.IDCause
                                        oNewRow("Compare") = oRemark.Compare
                                        oNewRow("Value") = oRemark.Value
                                        tbRet.Rows.Add(oNewRow)
                                    End If
                                Next

                            End If

                        Next

                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roSchedulerRemarks::GetSchedulerRemarks")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roSchedulerRemarks::GetSchedulerRemarks")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

    End Class

    ''' <summary>
    ''' Filtro de resalte
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <DataContract()>
    <XmlType(Namespace:="http://localhost", TypeName:="roRemark")>
    Public Class roCalendarRemark

#Region "Declarations - Constructors"

        Private intIDCause As Integer
        Private oCompare As RemarkCompare
        Private xValue As DateTime
        Private intColor As Integer

        Public Sub New()

        End Sub

        Public Sub New(ByVal _IDCause As Integer, ByVal _Compare As RemarkCompare, ByVal _Value As DateTime, ByVal _Color As Integer)
            Me.intIDCause = _IDCause
            Me.oCompare = _Compare
            Me.xValue = _Value
            Me.intColor = _Color
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property
        <DataMember()>
        Public Property Compare() As RemarkCompare
            Get
                Return Me.oCompare
            End Get
            Set(ByVal value As RemarkCompare)
                Me.oCompare = value
            End Set
        End Property
        <DataMember()>
        Public Property Value() As DateTime
            Get
                Return Me.xValue
            End Get
            Set(ByVal value As DateTime)
                Me.xValue = value
            End Set
        End Property
        <DataMember()>
        Public Property Color() As Integer
            Get
                Return Me.intColor
            End Get
            Set(ByVal value As Integer)
                Me.intColor = value
            End Set
        End Property

#End Region

    End Class

End Namespace