Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace VTVisits

    <DataContract>
    Public Class roVisit

        Private sIDVisit As String = 0
        Private sIDParentVisit As String = 0
        Private Shared oState As New roVisitState
        Private sName As String = ""
        Private lVisitors As New List(Of roVisitor)
        Private dBeginDate As Date = New Date(1970, 1, 1)
        Private dEndDate As Date = New Date(1970, 1, 1)
        Private dLastOut As Date = New Date(1970, 1, 1)
        Private dLastIn As Date = New Date(1970, 1, 1)
        Private bRepeat As Boolean
        Private sCloneEvery As String = ""
        Private iIDEmployee As Integer = 0
        Private sEmployee As String = ""
        Private lFields As New List(Of roVisitField)
        Private lPunches As New List(Of roVisitPunches)
        Private iCreatedBy As Integer = 0
        Private sCreatedBy As String = ""
        Private iStatus As Integer = 0
        Private dModified As Date = New Date(1970, 1, 1)
        Private bDelete As Boolean = False
        Private iType As Integer
        Private iIDType As String
        Private visittype As String
        Private sResult As String = ""
        Private iModifiedBy As Integer = 0
        Private dSearchParams As New Dictionary(Of String, String)

#Region "Property"

        <DataMember()>
        Public Property idvisit As String
            Get
                Return sIDVisit
            End Get
            Set(value As String)
                sIDVisit = value
            End Set
        End Property

        <DataMember()>
        Public Property idtype As String
            Get
                Return iIDType
            End Get
            Set(value As String)
                iIDType = value
            End Set
        End Property

        <DataMember()>
        Public Property idparentvisit As String
            Get
                Return sIDParentVisit
            End Get
            Set(value As String)
                sIDParentVisit = value
            End Set
        End Property

        <DataMember()>
        Public Property visittypedesc As String
            Get
                Return visittype
            End Get
            Set(value As String)
                visittype = value
            End Set
        End Property

        <DataMember()>
        Public Property name As String
            Get
                Return sName
            End Get
            Set(value As String)
                sName = value
            End Set
        End Property

        <DataMember()>
        Public Property visitors As List(Of roVisitor)
            Get
                Return lVisitors
            End Get
            Set(ByVal value As List(Of roVisitor))
                lVisitors = value
            End Set
        End Property

        <DataMember()>
        Public Property begindate() As Date
            Get
                Return dBeginDate
            End Get
            Set(ByVal Value As Date)
                dBeginDate = Value
            End Set
        End Property

        <DataMember()>
        Public Property enddate() As Date
            Get
                Return dEndDate
            End Get
            Set(ByVal Value As Date)
                dEndDate = Value
            End Set
        End Property

        <DataMember()>
        Public Property lastOutDate() As Date
            Get
                Return dLastOut
            End Get
            Set(ByVal Value As Date)
                dLastOut = Value
            End Set
        End Property

        <DataMember()>
        Public Property lastInDate() As Date
            Get
                Return dLastIn
            End Get
            Set(ByVal Value As Date)
                dLastIn = Value
            End Set
        End Property

        <DataMember()>
        Public Property repeat() As Boolean
            Get
                Return bRepeat
            End Get
            Set(ByVal Value As Boolean)
                bRepeat = Value
            End Set
        End Property

        <DataMember()>
        Public Property cloneevery() As String
            Get
                Return sCloneEvery
            End Get
            Set(ByVal Value As String)
                sCloneEvery = Value
            End Set
        End Property

        <DataMember()>
        Public Property idemployee() As Integer
            Get
                Return iIDEmployee
            End Get
            Set(ByVal Value As Integer)
                iIDEmployee = Value
            End Set
        End Property

        <DataMember()>
        Public Property employee() As String
            Get
                Return sEmployee
            End Get
            Set(ByVal Value As String)
                sEmployee = Value
            End Set
        End Property

        <DataMember()>
        Public Property fields As List(Of roVisitField)
            Get
                Return lFields
            End Get
            Set(value As List(Of roVisitField))
                lFields = value
            End Set
        End Property

        <DataMember()>
        Public Property punches As List(Of roVisitPunches)
            Get
                Return lPunches
            End Get
            Set(value As List(Of roVisitPunches))
                lPunches = value
            End Set
        End Property

        <DataMember()>
        Public Property createdby() As Integer
            Get
                Return iCreatedBy
            End Get
            Set(ByVal Value As Integer)
                iCreatedBy = Value
            End Set
        End Property

        <DataMember()>
        Public Property createdbyname() As String
            Get
                Return sCreatedBy
            End Get
            Set(ByVal Value As String)
                sCreatedBy = Value
            End Set
        End Property

        <DataMember()>
        Public Property status() As Integer
            Get
                Return iStatus
            End Get
            Set(ByVal Value As Integer)
                iStatus = Value
            End Set
        End Property

        <DataMember()>
        Public Property modified() As DateTime
            Get
                Return dModified
            End Get
            Set(ByVal Value As DateTime)
                dModified = Value
            End Set
        End Property

        <DataMember()>
        Public Property deleted() As Boolean
            Get
                Return bDelete
            End Get
            Set(ByVal Value As Boolean)
                bDelete = Value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        <DataMember()>
        Public Property modifiedBy As Integer
            Get
                Return iModifiedBy
            End Get
            Set(value As Integer)
                iModifiedBy = value
            End Set
        End Property

#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal id As String, ByVal idtype As String, ByVal oPerm As Permission, ByVal _State As roVisitState,
                Optional ByVal bAudit As Boolean = False, Optional SearchParams As Dictionary(Of String, String) = Nothing)
            Try
                sIDVisit = id

                iIDType = roTypes.Any2Integer(idtype)
                If Not SearchParams Is Nothing Then dSearchParams = SearchParams
                If Not _State Is Nothing Then oState = _State
                load(oPerm, bAudit, SearchParams)
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception
            End Try

        End Sub

        Private Function load(ByVal oPerm As Permission, Optional ByVal bAudit As Boolean = False, Optional SearchParams As Dictionary(Of String, String) = Nothing) As Boolean
            Try
                If Not SearchParams Is Nothing Then dSearchParams = SearchParams

                Dim sSQL As String
                Dim tb As DataTable
                If sIDVisit <> "new" And sIDVisit <> "0" Then

                    sSQL = "@SELECT# v.*, e1.name as Employee, p.name as CreatedByName, vt.Name as VisitTypeDesc FROM Visit v"
                    sSQL += " left outer join employees e1 on v.idemployee=e1.id"
                    sSQL += " left outer join [sysroPassports] p on v.createdby=p.id"
                    sSQL += " left outer join Visit_Types vt on v.VisitType=vt.IDType"
                    sSQL += " WHERE v.deleted=0 and idvisit = '" + sIDVisit.ToString + "'"
                    tb = CreateDataTable(sSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        parseRow(tb.Rows(0))

                        'Cargamos los visitantes
                        If Not (dSearchParams.ContainsKey("_loadvisitors_") AndAlso Any2Boolean(dSearchParams("_loadvisitors_")) = False) Then
                            sSQL = "@SELECT# v.idvisitor from Visit_Visitor vv inner join Visitor v on vv.idvisitor=v.idvisitor"
                            sSQL += " where vv.idvisit='" + sIDVisit.ToString + "'"
                            tb = CreateDataTable(sSQL)
                            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                                lVisitors.Clear()
                                Dim fld As roVisitor = Nothing
                                Dim std As roVisitorState = Nothing
                                For Each row As DataRow In tb.Rows
                                    fld = New roVisitor(row("idvisitor"), oPerm, std, False, SearchParams)
                                    'fld.parseRow(row)
                                    lVisitors.Add(fld)
                                Next
                            End If
                        End If
                    Else
                        oState.result = roVisitState.ResultEnum.NotFound
                    End If
                End If

                'Cargamos los campos personalizados
                sSQL = "@SELECT# vf.*, isnull(vfv.value,'') as value, lap.Value as UniqueID from Visit_Fields vf "
                sSQL += " left outer join Visit_Fields_Value vfv on vfv.idfield=vf.idfield"
                sSQL += " and vfv.idvisit='" + sIDVisit.ToString + "'"
                sSQL += "  left outer join sysroLiveAdvancedParameters lap on vf.IDField=lap.value and ParameterName='vst_visitUniqueIDField'"
                If (iIDType <> "0" And iIDType <> "") Then
                    sSQL += " where deleted=0 and visible=1 and (vf.VisitType=" + iIDType.ToString + " or vf.VisitType=0 or vf.VisitType is null) order by position asc"
                Else
                    sSQL += " where deleted=0 and visible=1 and (vf.VisitType=0 or vf.VisitType is null) order by position asc"
                End If

                tb = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    lFields.Clear()
                    Dim fld As New roVisitField
                    For Each row As DataRow In tb.Rows
                        fld = New roVisitField
                        fld.parseRow(row)
                        lFields.Add(fld)
                    Next
                End If

                'Cargamos los movimientos
                If Not (dSearchParams.ContainsKey("_loadpunches_") AndAlso Any2Boolean(dSearchParams("_loadpunches_")) = False) Then
                    sSQL = "@SELECT# vvp.idvisit as idvisit, vvp.idvisitor as idvisitor, vr.name as name, datepunch, action"
                    sSQL += " from Visit_Visitor_Punch vvp"
                    sSQL += " inner join Visitor vr on vvp.IDVisitor = vr.IDVisitor "
                    sSQL += " where vvp.idvisit='" + sIDVisit.ToString + "'"

                    tb = CreateDataTable(sSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        lPunches.Clear()
                        Dim pun As New roVisitPunches
                        For Each row As DataRow In tb.Rows
                            pun = New roVisitPunches
                            pun.parseRow(row)
                            lPunches.Add(pun)
                        Next

                        'busco la última entrada para mostrarla en el grid
                        If (lPunches IsNot Nothing AndAlso lPunches.Count > 0) Then
                            If True Then
                                Dim lstPunchVisit = lPunches.Where(Function(lp) lp.action.Equals("IN"))
                                If (lstPunchVisit IsNot Nothing AndAlso lstPunchVisit.Count > 0) Then dLastIn = lstPunchVisit.Max(Function(mp) mp.punchdate)
                            End If
                        End If

                        'busco la última salida para mostrarla en el grid
                        If (lPunches IsNot Nothing AndAlso lPunches.Count > 0) Then
                            If True Then
                                Dim lstPunchVisit = lPunches.Where(Function(lp) lp.action.Equals("OUT"))
                                If (lstPunchVisit IsNot Nothing AndAlso lstPunchVisit.Count > 0) Then
                                    If dLastIn < lstPunchVisit.Max(Function(mp) mp.punchdate) Then dLastOut = lstPunchVisit.Max(Function(mp) mp.punchdate)
                                End If
                            End If
                        End If

                    End If
                End If
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisit::load::" + ex.Message)

                Return False
            End Try
            Return True

        End Function

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("Name") Then sName = HTML2String(row.Item("name"))
                dBeginDate = Any2DateTime(row.Item("BeginDate"))
                dEndDate = Any2DateTime(row.Item("enddate"))
                'Compatibilidad con visitas anteriores, si no tiene id publico se le assigna uno
                If row.Table.Columns.Contains("Repeat") Then bRepeat = Any2Boolean(row.Item("Repeat"))
                If row.Table.Columns.Contains("CloneEvery") Then sCloneEvery = Any2String(row.Item("CloneEvery"))
                If row.Table.Columns.Contains("IDEmployee") Then iIDEmployee = Any2Integer(row.Item("IDEmployee"))
                If row.Table.Columns.Contains("Employee") Then sEmployee = Any2String(row.Item("Employee"))
                If row.Table.Columns.Contains("CreatedBy") Then iCreatedBy = Any2Integer(row.Item("CreatedBy"))
                If row.Table.Columns.Contains("CreatedByName") Then sCreatedBy = Any2String(row.Item("CreatedByName"))
                If row.Table.Columns.Contains("Status") Then iStatus = Any2Integer(row.Item("Status"))
                If row.Table.Columns.Contains("Modified") Then dModified = Any2DateTime(row.Item("Modified"))
                If row.Table.Columns.Contains("Deleted") Then bDelete = Any2Boolean(row.Item("Deleted"))
                If row.Table.Columns.Contains("IDParentVisit") Then sIDParentVisit = Any2String(row.Item("IDParentVisit"))
                If row.Table.Columns.Contains("VisitType") Then iIDType = Any2String(row.Item("VisitType"))
                If row.Table.Columns.Contains("VisitTypeDesc") Then visittype = Any2String(row.Item("VisitTypeDesc"))
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception
            End Try
        End Sub

        Public Function save(ByVal url As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim act As VTBase.Audit.Action
            Dim bSavePunch As Boolean = False
            Dim bRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Finaliza una visita
                If iStatus = 2 Then
                    If bRepeat Then
                        If dEndDate.Subtract(Now).TotalMinutes > 1 Then
                            iStatus = 0
                            bSavePunch = True
                        End If
                    End If
                End If

                If iIDType = Nothing Or iIDType = "-1" Then
                    iIDType = "null"
                End If

                Dim sSQL As String
                Dim iOldStatus As Integer = -1
                If sIDVisit = "0" Or sIDVisit = "" Or sIDVisit = "new" Then
                    act = VTBase.Audit.Action.aInsert
                    sIDVisit = Guid.NewGuid().ToString()
                    sSQL = "@INSERT# INTO Visit ([IDVisit],[Name],[BeginDate],[EndDate],[Repeat],[IDEmployee],[CreatedBy],[Status],[Modified],[CloneEvery],[IDParentVisit], [VisitType])"
                    sSQL += " values ('" + sIDVisit.ToString + "', '" + any2SQLHTML(sName) + "', " + Any2Time(dBeginDate).SQLDateTime + ", " + Any2Time(dEndDate).SQLDateTime + ", "
                    sSQL += If(bRepeat, "1", "0") + ", " + iIDEmployee.ToString + ", " + iCreatedBy.ToString + ", " + iStatus.ToString + ", getdate(), '" + sCloneEvery.ToString + "',"
                    sSQL += If(sCloneEvery.Replace("-", "").Length > 0 AndAlso sIDParentVisit = "0", "'" & sIDVisit.ToString & "'", If(sCloneEvery.Replace("-", "").Length > 0 AndAlso sIDParentVisit.Length > 1, "'" & sIDParentVisit.ToString & "'", "''"))
                    sSQL += ", " + iIDType.ToString + ")"
                Else

                    ' Recupero estado actual para controlar si hay que guardar fichaje o no ...
                    sSQL = "@SELECT# Status from Visit where idVisit = '" + sIDVisit.ToString + "'"
                    iOldStatus = Any2Integer(ExecuteScalar(sSQL))
                    act = VTBase.Audit.Action.aUpdate
                    sSQL = "@UPDATE# Visit set"
                    sSQL += "  [Name]='" + any2SQLHTML(sName) + "'"
                    sSQL += "  ,[BeginDate] = " + Any2Time(dBeginDate).SQLDateTime
                    sSQL += "  ,[EndDate] = " + Any2Time(dEndDate).SQLDateTime
                    sSQL += "  ,[Repeat] =" + If(bRepeat, "1", "0")
                    sSQL += "  ,[IDEmployee] = " + iIDEmployee.ToString
                    sSQL += "  ,[Status] = " + iStatus.ToString
                    sSQL += "  ,[Modified] = getdate()"
                    sSQL += "  ,[CloneEvery] = '" + sCloneEvery.ToString + "'"
                    sSQL += " where idvisit='" + sIDVisit.ToString + "'"
                End If

                ExecuteSql(sSQL)

                For Each fl As roVisitField In lFields
                    fl.idvisit = sIDVisit
                    fl.saveValue(bAudit)
                Next

                sSQL = "@DELETE# FROM Visit_Visitor where idvisit='" + sIDVisit.ToString + "'"
                ExecuteSql(sSQL)

                For Each vstr As roVisitor In lVisitors
                    sSQL = "@INSERT# INTO Visit_Visitor (idvisit, idvisitor)"
                    sSQL += " values('" + sIDVisit.ToString + "', '" + vstr.idvisitor.ToString + "')"
                    ExecuteSql(sSQL)

                    'Generamos el marcaje
                    If ((iStatus = 1 Or iStatus = 2) AndAlso iOldStatus <> iStatus) Or bSavePunch Then
                        sSQL = "@INSERT# INTO Visit_Visitor_Punch (idvisit, idvisitor, DatePunch, Action)"
                        sSQL += " values('" + sIDVisit.ToString + "', '" + vstr.idvisitor.ToString + "',getdate(),'" + IIf(iStatus = 1, "IN", "OUT") + "')"
                        ExecuteSql(sSQL)
                    End If
                Next

                'Clonamos la siguiente visita
                If sCloneEvery.Replace("-", "").Length > 0 And iStatus >= 2 Then
                    Dim vst2 As roVisit
                    vst2 = Me
                    vst2.idparentvisit = sIDParentVisit
                    vst2.idvisit = "new"
                    vst2.status = 0
                    vst2.punches.Clear()
                    Select Case sCloneEvery.Split(";")(0)
                        Case "daily"
                            If sCloneEvery.Split(";")(1) = "a" Then
                                vst2.begindate = vst2.begindate.AddDays(1)
                            Else
                                vst2.begindate = vst2.begindate.AddDays(Any2Integer(sCloneEvery.Split(";")(2)))
                            End If
                        Case "weekly"
                            Dim bexist As Boolean = False
                            For i As Integer = 0 To sCloneEvery.Split(";")(2).Split(",").Count - 1
                                If Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) > Me.begindate.DayOfWeek Then
                                    vst2.begindate = vst2.begindate.AddDays(Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) - Me.begindate.DayOfWeek)
                                    bexist = True
                                    Exit For
                                End If
                                If Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) = 0 Then
                                    vst2.begindate = vst2.begindate.AddDays(7 - Me.begindate.DayOfWeek)
                                    bexist = True
                                    Exit For
                                End If
                            Next
                            If Not bexist Then
                                vst2.begindate = vst2.begindate.AddDays(7 * Any2Integer(sCloneEvery.Split(";")(1)))
                                vst2.begindate = vst2.begindate.AddDays(-1 * (vst2.begindate.DayOfWeek - Any2Integer(sCloneEvery.Split(";")(2).Split(",")(0))))
                            End If
                        Case "monthly"
                            vst2.begindate = New DateTime(Me.begindate.Year, Me.begindate.AddMonths(Any2Integer(sCloneEvery.Split(";")(2))).Month, Any2Integer(sCloneEvery.Split(";")(1)), Me.begindate.Hour, Me.begindate.Minute, 0)
                        Case "yearly"
                            vst2.begindate = New DateTime(Me.begindate.AddYears(1).Year, Any2Integer(sCloneEvery.Split(";")(2)), Any2Integer(sCloneEvery.Split(";")(1)), Me.begindate.Hour, Me.begindate.Minute, 0)
                    End Select
                    If vst2.enddate > vst2.begindate Then
                        ' Sólo creo la visita para el día siguiente si no existe otra ya creada
                        Dim sSQL2 As String = "@SELECT# idvisit from Visit where idparentvisit = '" + sIDParentVisit.ToString + "' and begindate = " + Any2Time(vst2.begindate).SQLDateTime
                        sSQL2 += " and CloneEvery = '" + Me.cloneevery + "'"
                        sSQL2 += " and IDEmployee = " + Me.idemployee.ToString + " "
                        Dim sIDVisit2 As String = Any2String(ExecuteScalar(sSQL2))
                        If sIDVisit2 = "" Then
                            vst2.save(bAudit)
                        End If
                    End If
                End If

                If iStatus = 0 Then 'Solo notificamos cambios, no estados, solo se puede modificar cuando esta planificada.
                    If CurrentOptions(True).notification = 1 Then
                        roNotification.SendMail(IIf(act = VTBase.Audit.Action.aInsert, roNotification.ActionType.New_, roNotification.ActionType.Update_), iIDEmployee, dBeginDate, sIDVisit, url)
                    ElseIf CurrentOptions.notification = 2 Then
                        roNotification.SendMail(IIf(act = VTBase.Audit.Action.aInsert, roNotification.ActionType.New_, roNotification.ActionType.Update_), iModifiedBy, dBeginDate, sIDVisit, url)
                    End If
                End If

                Try
                    Dim keys As New List(Of String)
                    Dim vals As New List(Of String)
                    keys.Add("begindate")
                    vals.Add(dBeginDate.ToString("dd/MM/yy hh:mm"))
                    keys.Add("enddate")
                    vals.Add(dEndDate.ToString("dd/MM/yy hh:mm"))
                    roLiveSupport.Audit(act, VTBase.Audit.ObjectType.tVisit, "", keys, vals, Nothing)
                Catch ex As Exception
                End Try
            Catch ex As Exception
                bRet = False
                oState.result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisit::save::" + ex.Message)
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
            End Try
            Return True
        End Function

        Public Shared Function saveVisitorOnVisit(ByVal IDVisit As String, ByVal IDVisitor As String) As Boolean
            Dim resp As Boolean = False

            Try

                Dim sSQL As String = "@INSERT# INTO Visit_Visitor (idvisit, idvisitor)"
                sSQL += " values('" + IDVisit + "', '" + IDVisitor + "')"
                Return ExecuteSql(sSQL)
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return resp
        End Function

        Public Shared Function getIDbyUniqueID(ByVal UniqueID As String) As String
            Dim resp As String = ""

            Try

                Dim sSQL As String = "@SELECT# top 1 v.idvisit from Visit v"
                sSQL += " inner join Visit_Fields_Value vfv on v.idvisit=vfv.idvisit and vfv.value='" + UniqueID + "'"
                sSQL += " inner join sysroLiveAdvancedParameters lap on vfv.IDField=lap.value and ParameterName='vst_visitUniqueIDField'"
                sSQL += " order by v.BeginDate desc"

                Return Any2String(ExecuteScalar(sSQL))
            Catch ex As Exception
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return resp
        End Function

        Public Function delete(ByVal _IDVisit As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                ExecuteSql("@UPDATE# visit set deleted=1,status = case when status=1 then 2 else status end where idvisit = '" + _IDVisit + "'")

                If CurrentOptions.notification = 1 Then
                    roNotification.SendMail(roNotification.ActionType.Delete_, iIDEmployee, dBeginDate, sIDVisit, "")
                Else
                    roNotification.SendMail(roNotification.ActionType.Delete_, iModifiedBy, dBeginDate, sIDVisit, "")
                End If

                Try
                    Dim keys As New List(Of String)
                    Dim vals As New List(Of String)
                    keys.Add("idvisit")
                    vals.Add(_IDVisit)
                    roLiveSupport.Audit(VTBase.Audit.Action.aDelete, VTBase.Audit.ObjectType.tVisit, "", keys, vals, Nothing)
                Catch ex As Exception
                End Try
            Catch ex As Exception
                oState.result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisit::delete::" + ex.Message)
            End Try

            Return True
        End Function

    End Class

    <DataContract()>
    Public Class roVisitState
        Inherits roBusinessState

        <Flags()>
        Public Enum ResultEnum
            NoError
            NotPermissions
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
            Set(ByVal value As ResultEnum)
                Me.intResult = value
                If Me.intResult <> ResultEnum.NoError And Me.intResult <> ResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

    End Class

    <DataContract>
    Public Class roVisitField

        Private oState As New roVisitState
        Private sIDField As String
        Private sIDVisit As String
        Private sName As String = ""
        Private bVisible As Boolean = True
        Private iRequired As Byte = 0
        Private iPosition As Byte = 0
        Private sValue As String = ""
        Private sValues As String = ""
        Private iEdit As Boolean = False
        Private sResult As String = "NoError"
        Private iVisitType As Integer
        Private sVisitTypeName As String = ""

#Region "Property"

        <DataMember()>
        Public Property idfield() As String
            Get
                Return sIDField
            End Get
            Set(ByVal Value As String)
                sIDField = Value
            End Set
        End Property

        <DataMember()>
        Public Property visittype() As Integer
            Get
                Return iVisitType
            End Get
            Set(ByVal Value As Integer)
                iVisitType = Value
            End Set
        End Property

        <DataMember()>
        Public Property visittypename() As String
            Get
                Return sVisitTypeName
            End Get
            Set(ByVal Value As String)
                sVisitTypeName = Value
            End Set
        End Property

        <DataMember()>
        Public Property name() As String
            Get
                Return sName
            End Get
            Set(ByVal Value As String)
                sName = Value
            End Set
        End Property

        <DataMember()>
        Public Property visible() As Boolean
            Get
                Return bVisible
            End Get
            Set(ByVal Value As Boolean)
                bVisible = Value
            End Set
        End Property

        <DataMember()>
        Public Property required() As Byte
            Get
                Return iRequired
            End Get
            Set(ByVal Value As Byte)
                iRequired = Value
            End Set
        End Property

        <DataMember()>
        Public Property position() As Byte
            Get
                Return iPosition
            End Get
            Set(ByVal Value As Byte)
                iPosition = Value
            End Set
        End Property

        <DataMember()>
        Public Property idvisit() As String
            Get
                Return sIDVisit
            End Get
            Set(ByVal Value As String)
                sIDVisit = Value
            End Set
        End Property

        <DataMember()>
        Public Property value() As String
            Get
                Return sValue
            End Get
            Set(ByVal Value As String)
                sValue = Value
            End Set
        End Property

        <DataMember()>
        Public Property values() As String
            Get
                Return sValues
            End Get
            Set(ByVal Value As String)
                sValues = Value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        <DataMember()>
        Public Property edit() As Boolean
            Get
                Return iEdit
            End Get
            Set(ByVal Value As Boolean)
                iEdit = Value
            End Set
        End Property

#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal _IDField As String, ByVal _State As roVisitState,
                Optional ByVal bAudit As Boolean = False)
            sIDField = _IDField
            load(bAudit)
        End Sub

        Private Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try
                If sIDField <> "" And sIDField <> "0" And sIDField <> "new" Then

                    Dim sSQL As String = "@SELECT# * from Visit_Fields where idfield='" + sIDField.ToString + "'"
                    Dim tb As DataTable = CreateDataTable(sSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        parseRow(tb.Rows(0))
                    Else
                        oState.result = roVisitState.ResultEnum.NotFound
                    End If
                End If
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::load::" + ex.Message)
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return True
        End Function

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("IDField") Then sIDField = Any2String(row.Item("IDField"))
                If row.Table.Columns.Contains("Name") Then sName = HTML2String(Any2String(row.Item("Name")).Trim)
                If row.Table.Columns.Contains("Visible") Then bVisible = Any2Boolean(row.Item("Visible"))
                If row.Table.Columns.Contains("Required") Then iRequired = Any2Integer(row.Item("Required"))
                If row.Table.Columns.Contains("Position") Then iPosition = Any2Integer(row.Item("Position"))
                If row.Table.Columns.Contains("Value") Then sValue = HTML2String(row.Item("Value"))
                If row.Table.Columns.Contains("Values") Then sValues = HTML2String(row.Item("Values"))
                If row.Table.Columns.Contains("Edit") Then iEdit = Any2Boolean(row.Item("Edit"))
                If row.Table.Columns.Contains("VisitType") Then iVisitType = Any2Integer(row.Item("VisitType"))
                If row.Table.Columns.Contains("NameType") Then sVisitTypeName = Any2String(row.Item("NameType"))
                If row.Table.Columns.Contains("UniqueID") Then
                    If Any2String(row.Item("UniqueID")).Length > 0 Then
                        iRequired = 99
                        If sValue.Length = 0 Then
                            sValue = getNewUniqueID()
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function save(Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String
                If sIDField = "0" Or sIDField = "" Or sIDField = "new" Then
                    sIDField = Guid.NewGuid().ToString()
                    iPosition = Any2Integer(ExecuteScalar("@SELECT# max(position) from Visit_Fields where deleted=0")) + 1
                    sSQL = " @INSERT# INTO Visit_Fields (IDField, name, Visible, Required, position, [values], edit, VisitType) "
                    sSQL += "values('" + sIDField.ToString + "', '" + any2SQLHTML(sName) + "'," + IIf(bVisible, "1", "0") + "," + iRequired.ToString + "," + iPosition.ToString + ", '" + any2SQLHTML(sValues)
                    sSQL += "', " & If(iEdit, "1", "0") & ", " + iVisitType.ToString + ")"
                Else
                    sSQL = "@UPDATE# Visit_Fields set"
                    sSQL += " Name='" + any2SQLHTML(sName) + "'"
                    sSQL += " ,Visible=" + IIf(bVisible, "1", "0")
                    sSQL += " ,Required=" + iRequired.ToString
                    sSQL += " ,VisitType=" + iVisitType.ToString
                    sSQL += " ,[Values]='" + any2SQLHTML(sValues) + "'"
                    sSQL += " ,edit=" + If(iEdit, "1", "0")
                    sSQL += " where IDField='" + sIDField + "'"
                End If
                Return ExecuteSql(sSQL)
            Catch ex As Exception
                'Cerramos conexión si es necesario

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::save::" + ex.Message)
            End Try
            Return True

        End Function

        Public Function saveValue(Optional ByVal bAudit As Boolean = False) As Boolean
            Try

                Dim sSQL As String
                sSQL = " @INSERT# INTO Visit_Fields_Value (IDField,IDVisit,Value)"
                sSQL += "values('" + sIDField.ToString + "', '" + sIDVisit.ToString + "','" + any2SQLHTML(sValue) + "')"

                Try
                    ExecuteSql(sSQL)
                Catch ex As Exception
                    sSQL = "@UPDATE# Visit_Fields_Value set"
                    sSQL += " Value='" + any2SQLHTML(sValue) + "'"
                    sSQL += " where IDField='" + sIDField + "'"
                    sSQL += " and IDVisit='" + sIDVisit + "'"
                    Return ExecuteSql(sSQL)

                End Try
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::savevalue::" + ex.Message)
            End Try
            Return True
        End Function

        Public Shared Function DeleteVisitField(idfield As String) As Boolean
            Try
                Dim sSQL As String
                Dim sSQLAux As String

                sSQL = "@UPDATE# Visit_Fields set deleted=1 where idfield='" + idfield + "'"
                ExecuteSql(sSQL)
                sSQLAux = "@SELECT# Name FROM Visit_Fields where idfield='" + idfield + "'"
                sSQL = "@UPDATE# sysroLiveAdvancedParameters set Value = '' where ParameterName like '%vst_%' and Value = (" & sSQLAux & ")"
                ExecuteSql(sSQL)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::deletevisitfield::" + ex.Message)
                Return False
            End Try
            Return True
        End Function

        Public Shared Function upVisitField(idfield As String) As Boolean
            Try
                Dim sSQL As String

                sSQL = "@SELECT# position from Visit_Fields where idfield='" + idfield + "'"
                Dim currpos As Integer = ExecuteScalar(sSQL)
                If currpos >= 2 Then
                    sSQL = "@SELECT# max(position) from Visit_Fields where position<" + currpos.ToString
                    Dim uppos As Integer = ExecuteScalar(sSQL)
                    sSQL = "@UPDATE# Visit_Fields set position=" + currpos.ToString + " where position=" + uppos.ToString
                    ExecuteSql(sSQL)
                    sSQL = "@UPDATE# Visit_Fields set position=" + uppos.ToString + " where idfield='" + idfield + "'"
                    ExecuteSql(sSQL)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::upvisitfield::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Shared Function downVisitField(idfield As String) As Boolean
            Try
                Dim sSQL As String
                sSQL = "@SELECT# position from Visit_Fields where idfield='" + idfield + "'"
                Dim currpos As Integer = ExecuteScalar(sSQL)
                sSQL = "@SELECT# max(position) from Visit_Fields"
                Dim maxpos As Integer = ExecuteScalar(sSQL)
                If currpos < maxpos Then
                    sSQL = "@SELECT# min(position) from Visit_Fields where position>" + currpos.ToString
                    Dim downpos As Integer = ExecuteScalar(sSQL)
                    sSQL = "@UPDATE# Visit_Fields set position=" + currpos.ToString + " where position=" + downpos.ToString
                    ExecuteSql(sSQL)
                    sSQL = "@UPDATE# Visit_Fields set position=" + downpos.ToString + " where idfield='" + idfield + "'"
                    ExecuteSql(sSQL)

                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::downvisitfield::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Private Function getNewUniqueID() As String
            Dim id As String = ""
            Try
                Dim exist As Boolean = True
                Dim i As Integer = 0

                'Dim StrType As eStrType = Any2Integer(GetParameter("Visits.UID.Type"))
                'Dim StrLen As Integer = Any2Integer(GetParameter("Visits.UID.Len"))
                'If StrType > VtVisitsCommon.eStrType.None Then
                id = RandomString(roOptions.visitUniqueIDFieldLen, roOptions.UniqueIDType)

                Dim sSQL As String
                Dim tb As New DataTable
                While exist Or i < 100
                    sSQL = "@SELECT# * from Visit_Fields_Value vfv  inner join Visit v on v.idvisit=vfv.idvisit where v.deleted=0 and vfv.idfield='" + roOptions.visitUniqueIDFieldParam + "'"
                    sSQL += " and value = '" + id + "'"
                    tb = CreateDataTable(sSQL)
                    If tb.Rows.Count = 0 Then
                        exist = False
                    Else
                        id = RandomString(roOptions.visitUniqueIDFieldLen, roOptions.UniqueIDType)
                    End If
                    i += 1 'Nos aseguramos no generar un bucle infinito.
                End While
                'Else
                '    id = ""
                'End If
            Catch ex As Exception
            End Try
            Return id
        End Function

    End Class

    <DataContract>
    Public Class roVisitList

        Private lVisit As New List(Of roVisit)
        Private dFilter As New Dictionary(Of String, String)
        Private dSearchParams As New Dictionary(Of String, String)
        Private oState As New roVisitState
        Private sResult As String = ""
        Private oPerm As Permission

        <DataMember()>
        Public Property visits As List(Of roVisit)
            Get
                Return lVisit
            End Get
            Set(value As List(Of roVisit))
                lVisit = lVisit
            End Set
        End Property

        <DataMember()>
        Public Property filter As Dictionary(Of String, String)
            Get
                Return dFilter
            End Get
            Set(value As Dictionary(Of String, String))
                dFilter = value
            End Set
        End Property

        <DataMember()>
        Public Property searchparams As Dictionary(Of String, String)
            Get
                Return dSearchParams
            End Get
            Set(value As Dictionary(Of String, String))
                dSearchParams = value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        'Public Sub New()
        '    Me.oPerm = Security.WLHelper.GetPermissionOverFeature(Nothing, "Visits", "U")
        'End Sub

        Public Sub New(ByVal IDPassport As Integer, ByVal _State As roVisitState, doLoad As Boolean,
            Optional ByVal bAudit As Boolean = False)
            If Not _State Is Nothing Then oState = _State

            Me.oPerm = Security.WLHelper.GetPermissionOverFeature(IDPassport, "Visits", "U")

            If Me.oPerm = 0 Then
                Me.oPerm = Security.WLHelper.GetPermissionOverFeature(IDPassport, "Visits", "E")
            End If

            If doLoad = True Then
                load(IDPassport, bAudit)
            End If

        End Sub

        Public Function load(ByVal IDPassport As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim bIsSupervisor As Boolean = True
                Dim oPerm As Integer = Security.WLHelper.GetPermissionOverFeature(IDPassport, "Visits", "U")
                If oPerm = 0 Then
                    bIsSupervisor = False
                End If

                'Filtro por campos personalizados
                Dim cNamVal As Dictionary(Of String, String) = New Dictionary(Of String, String)
                For Each nam As KeyValuePair(Of String, String) In dFilter
                    If nam.Key(0) <> "_" And nam.Key <> "status" And nam.Key <> "begindate" And nam.Key <> "enddate" And nam.Key <> "visitor" And nam.Key <> "employee" And nam.Key <> "date" And nam.Key <> "lastIn" And nam.Key <> "lastOut" And nam.Key <> "orderBy" And nam.Key <> "location" Then
                        If nam.Value.Length > 0 Then
                            cNamVal.Add(nam.Key, nam.Value)
                        End If
                    End If
                Next

                'Dim sSQL As String = "@SELECT# distinct top 200 v.idvisit from Visit v"
                Dim sSQL As String = "@SELECT# distinct v.idvisit from Visit v"
                sSQL += " inner join sysrovwVisitDateList vvw on v.idvisit=vvw.idvisit"
                If bIsSupervisor Then
                    sSQL += " inner join [sysrovwCurrentEmployeeGroups] ceg on ceg.idemployee=v.idemployee"
                    sSQL &= " inner join [sysrovwSecurity_PermissionOverEmployees] poe ON poe.IDPassport = " & IDPassport.ToString & " AND poe.IDEmployee=ceg.idemployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate "
                    sSQL &= " inner join [sysrovwSecurity_PermissionOverFeatures] pof ON pof.IDPassport = " & IDPassport.ToString & " AND pof.IdFeature = 31 AND Permission > 0 "
                Else
                    sSQL += " inner join Employees e on e.ID = v.IDEmployee "
                End If

                If Me.oPerm = Permission.Admin Then
                    ' sSQL += " inner join  GetSupervisedEmployeesByPassport(" + IDPassport.ToString + ", 'Visits') sep"
                    'sSQL += " on sep.id=ceg.idemployee"
                ElseIf Me.oPerm = Permission.Write Then
                    sSQL += " inner join  sysroPassports p"
                    If bIsSupervisor Then
                        sSQL += " on p.IDEmployee=ceg.IDEmployee and CurrentEmployee=1 and p.id=" + IDPassport.ToString
                    Else
                        sSQL += " on e.ID = p.IDEmployee and p.id=" + IDPassport.ToString
                    End If
                Else
                    'No devolvemos ninguna visita, no tiene permisos para entrar en visitas
                    Return True
                End If

                If dFilter.ContainsKey("visitor") AndAlso dFilter("visitor").Trim.Length > 0 Then
                    sSQL += " inner join Visit_Visitor vv on v.idvisit=vv.idvisit"
                    sSQL += " inner join Visitor vr on vv.idvisitor=vr.idvisitor"
                End If
                If dFilter.ContainsKey("employee") AndAlso dFilter("employee").Trim.Length > 0 Then
                    sSQL += " inner join Employees e on v.idemployee=e.id"
                End If
                If dFilter.ContainsKey("location") AndAlso dFilter("location").Trim.Length > 0 Then
                    sSQL += " inner join Visit_Fields_Value vfv on v.idvisit=vfv.idvisit and vfv.value='" + dFilter("location") + "'"
                    sSQL += " inner join sysroLiveAdvancedParameters lap on vfv.IDField=lap.value and ParameterName='vst_MultilocationField'"
                End If

                If cNamVal.Count > 0 Then
                    sSQL += " inner join (@SELECT# IDVisit from [dbo].[Visit_Fields] vf left outer join [dbo].[Visit_Fields_Value] vfv on vf.IDField = vfv.IDField where vf.deleted = 0 And visible = 1"
                    For Each nam As KeyValuePair(Of String, String) In cNamVal
                        sSQL += " and (vfv.IDField='" + nam.Key + "' and vfv.value like '" + any2SQLHTML(nam.Value).Replace("*", "%").Replace("=", "").Replace("'", "") + "')"
                    Next
                    sSQL += ") as vals on v.idvisit=vals.idvisit "
                End If

                If Me.oPerm >= Permission.Admin Or Not bIsSupervisor Then

                    If ((dFilter.ContainsKey("lastIn") AndAlso dFilter("lastIn").Trim.Length > 0) OrElse (dFilter.ContainsKey("lastOut") AndAlso dFilter("lastOut").Trim.Length > 0)) Then
                        sSQL += " inner join Visit_Visitor_Punch vv on v.IDVisit = vv.IDVisit "
                    End If
                    sSQL += " where v.deleted=0 "
                    'si existen los dos filtros busco entre fechas

#Region "FILTROS DE FECHAS"

                    'fichajes
                    If ((dFilter.ContainsKey("lastIn") AndAlso dFilter("lastIn").Trim.Length > 0) AndAlso (dFilter.ContainsKey("lastOut") AndAlso dFilter("lastOut").Trim.Length > 0)) Then
                        sSQL += " and ((CONVERT(DATETIME,vv.DatePunch,120) between " & SQLDateTime(Any2Time(dFilter("lastIn")).DateOnly) &
                            " and " & SQLDateTime(Any2Time(dFilter("lastOut")).DateOnly) &
                            " and vv.Action = 'IN') or (CONVERT(DATETIME,vv.DatePunch,120) between " & SQLDateTime(Any2Time(dFilter("lastIn")).DateOnly) &
                            " and " & SQLDateTime(Any2Time(dFilter("lastOut")).DateOnly) & " and vv.Action = 'OUT')) "
                    ElseIf (dFilter.ContainsKey("lastIn") AndAlso dFilter("lastIn").Trim.Length > 0) Then
                        sSQL += " and (CONVERT(DATETIME,vv.DatePunch,120) = " + SQLDateTime(Any2Time(dFilter("lastIn")).DateOnly) & " and vv.Action = 'IN') "
                    ElseIf (dFilter.ContainsKey("lastOut") AndAlso dFilter("lastOut").Trim.Length > 0) Then
                        sSQL += " and (CONVERT(DATETIME,vv.DatePunch,120) = " + SQLDateTime(Any2Time(dFilter("lastOut")).DateOnly) & " and vv.Action = 'OUT') "
                    End If
                    'planificación

                    If (dFilter.ContainsKey("begindate") AndAlso dFilter("begindate").Trim.Length = 0) Then
                        dFilter("begindate") = DateSerial(1900, 1, 1)
                    End If

                    If (dFilter.ContainsKey("enddate") AndAlso dFilter("enddate").Trim.Length = 0) Then
                        dFilter("enddate") = DateSerial(2079, 1, 1)
                    End If

                    If ((dFilter.ContainsKey("begindate") AndAlso dFilter("begindate").Trim.Length > 0) AndAlso (dFilter.ContainsKey("enddate") AndAlso dFilter("enddate").Trim.Length > 0)) Then
                        sSQL += " and ((cBeginDate between " & SQLDateTime(Any2Time(dFilter("begindate")).DateOnly) &
                            " and " & SQLDateTime(Any2Time(dFilter("enddate")).DateOnly) &
                            " ) or (cEndDate between " & SQLDateTime(Any2Time(dFilter("begindate")).DateOnly) &
                            " and " & SQLDateTime(Any2Time(dFilter("enddate")).DateOnly) & "))"
                    End If

#End Region

                    If dFilter.ContainsKey("visitor") AndAlso dFilter("visitor").Trim.Length > 0 Then
                        sSQL += " and vr.name like '%" + any2SQLHTML(dFilter("visitor")).Replace("*", "%").Replace("=", "").Replace("'", "") + "%'"
                    End If
                    If dFilter.ContainsKey("employee") AndAlso dFilter("employee").Trim.Length > 0 Then
                        sSQL += " and e.name like '%" + any2SQLHTML(dFilter("employee")).Replace("*", "%").Replace("=", "").Replace("'", "") + "%'"
                    End If
                End If

                If dFilter.ContainsKey("status") Then
                    sSQL += " and status=" + Any2Integer(dFilter("status")).ToString
                End If
                If Me.oPerm < Permission.Admin And bIsSupervisor Then
                    sSQL += " and (getdate() between dateadd(d,-3,cbegindate) and dateadd(d,3,cenddate))"
                End If
                If dFilter.ContainsKey("date") Then
                    sSQL += " and (cbegindate = " + SQLDateTime(Any2Time(dFilter("date")).DateOnly)
                    sSQL += " or " + SQLDateTime(Any2Time(dFilter("date")).DateOnly) + " between cbegindate and cenddate)"
                End If

                Dim tmpList As New Generic.List(Of roVisit)
                Dim tmpList1 As New Generic.List(Of roVisit)
                Dim tmpList2 As New Generic.List(Of roVisit)
                Dim tmpList3 As New Generic.List(Of roVisit)
                Dim tmpList4 As New Generic.List(Of roVisit)

                Dim tb As DataTable = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vss As New roVisitState
                    Dim visitsCount = tb.Rows.Count
                    loadVisitsPartition(tb, tmpList, vss, 0, visitsCount)
                End If

                Dim orderKey As String = "hour"

                If dFilter.ContainsKey("orderBy") Then orderKey = dFilter("orderBy")

                Select Case orderKey.ToLower
                    Case "hour"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim tmpDate = If(x.lastInDate > x.begindate, x.lastInDate, x.begindate)
                                                            tmpDate = New Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, tmpDate.Hour, tmpDate.Minute, tmpDate.Second)
                                                            Return tmpDate
                                                        End Function))
                    Case "description"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Return x.name
                                                        End Function))
                    Case "visitor"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim visitorsName = ""
                                                            For Each oVistor In x.visitors
                                                                visitorsName = visitorsName & oVistor.name & ","
                                                            Next
                                                            Return visitorsName
                                                        End Function))
                    Case "employee"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Return x.employee
                                                        End Function))
                    Case "userfield"
                        Dim optSt As New roOptionsState
                        Dim joptions As New roOptions(optSt)

                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim oField = x.fields.Find(Function(y) y.name = joptions.cardfield)
                                                            If oField Is Nothing Then
                                                                Return ""
                                                            Else
                                                                Return oField.value
                                                            End If
                                                        End Function))
                End Select
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitlist::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Sub loadVisitsPartition(ByVal tb As Object, ByRef tmpList As Object, vss As roVisitState, iBegin As Integer, iEnd As Integer)
            If (iBegin < iEnd) Then
                For i As Integer = iBegin To (iEnd - 1)
                    Dim vs As roVisit
                    Dim idvisit As String = "new"
                    If tb.Rows.Count > 0 And tb.Rows.Count > i Then
                        idvisit = Any2String(tb.Rows(i).Item("idvisit"))
                    End If
                    vs = New roVisit(idvisit, Nothing, Me.oPerm, vss, , dFilter)
                    tmpList.Add(vs)
                Next
            End If
        End Sub

        Public Function search(ByVal IDPassport As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String = "@SELECT# v.idvisit from Visit v"
                sSQL += " inner join sysrovwVisitDateList vvw on v.idvisit=vvw.idvisit"
                If Me.oPerm = Permission.Admin Then
                    sSQL += " inner join [sysrovwCurrentEmployeeGroups] ceg on ceg.idemployee=v.idemployee"
                    sSQL += " inner join [sysrovwGetPermissionOverEmployee] gpoe on ceg.idemployee= gpoe.EmployeeID and gpoe.PassportID=" + IDPassport.ToString + " and gpoe.EmployeeFeatureID=31 and gpoe.CalculatedPermission > 3"
                ElseIf Me.oPerm = Permission.Write Then
                    sSQL += " inner join [sysrovwCurrentEmployeeGroups] ceg on ceg.idemployee=v.idemployee"
                    sSQL += " inner join  sysroPassports p"
                    sSQL += " on p.IDEmployee=ceg.IDEmployee and CurrentEmployee=1 and p.id=" + IDPassport.ToString
                Else
                    'No devolvemos ninguna visita, no tiene permisos para entrar en visitas
                    Return True
                End If
                Dim filter As String = sSQL

                Select Case dSearchParams("where")
                    Case "visits" 'Buscamos por nombre y campos de una visita
                        sSQL += " where (v.name like '%" + any2SQLHTML(dSearchParams("value")) + "%'"
                        sSQL += " or v.idvisit in ( @SELECT# IDVisit from Visit_Fields_Value where value like '%" + any2SQLHTML(dSearchParams("value")) + "%'))"
                    Case "visitors" 'Buscamos por nombre y campos de un visitante
                        sSQL += " where v.idvisit in ( @SELECT# idvisit"
                        sSQL += " from Visit_Visitor vv where vv.idvisitor in ("
                        sSQL += " @SELECT# visitor.IDVisitor from visitor inner join [dbo].[Visitor_Fields_Value] on Visitor.IDVisitor = Visitor_Fields_Value.IDVisitor where (visitor.Name like '%" & any2SQLHTML(dSearchParams("value")) & "%' or Visitor_Fields_Value.Value like '%" & any2SQLHTML(dSearchParams("value")) & "%')))"
                    Case "employees" 'Buscamos por el nombre de empleado
                        sSQL += " where ceg.EmployeeName like '%" + any2SQLHTML(dSearchParams("value")) + "%'"
                    Case Else 'Buscamos por empleado
                        sSQL += " where (ceg.EmployeeName like '%" + any2SQLHTML(dSearchParams("value")) + "%' "
                        sSQL += " or v.name like '%" + any2SQLHTML(dSearchParams("value")) + "%' "
                        sSQL += " or v.idvisit in (@SELECT#  vfv.IDVisit from [dbo].[Visit_Fields_Value] vfv where vfv.value like '%" + any2SQLHTML(dSearchParams("value")) + "%')"
                        sSQL += " or v.idvisit in (@SELECT# vv.idvisit from Visit_Visitor vv where vv.idvisitor in ("
                        sSQL += " @SELECT# visitor.IDVisitor from visitor inner join [dbo].[Visitor_Fields_Value] on Visitor.IDVisitor = Visitor_Fields_Value.IDVisitor where (visitor.Name like '%" & any2SQLHTML(dSearchParams("value")) & "%' or Visitor_Fields_Value.Value like '%" & any2SQLHTML(dSearchParams("value")) & "%'))))"

                End Select
                sSQL += " and v.deleted=0 "
                sSQL += " and (getdate() between cbegindate and cenddate)"

                Dim tb As DataTable = CreateDataTable(sSQL)

                Dim tmpList As New Generic.List(Of roVisit)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vs As roVisit
                    Dim vss As New roVisitState
                    For Each row As DataRow In tb.Rows
                        vs = New roVisit(Any2String(row.Item("idvisit")), Nothing, Me.oPerm, vss, , dFilter)
                        tmpList.Add(vs)
                    Next
                End If

                Dim orderKey As String = "hour"

                If dSearchParams.ContainsKey("orderBy") Then orderKey = dSearchParams("orderBy")

                Select Case orderKey.ToLower
                    Case "hour"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim tmpDate = If(x.lastInDate > x.begindate, x.lastInDate, x.begindate)
                                                            tmpDate = New Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, tmpDate.Hour, tmpDate.Minute, tmpDate.Second)
                                                            Return tmpDate
                                                        End Function))
                    Case "description"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Return x.name
                                                        End Function))
                    Case "visitor"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim visitorsName = ""
                                                            For Each oVistor In x.visitors
                                                                visitorsName = visitorsName & oVistor.name & ","
                                                            Next
                                                            Return visitorsName
                                                        End Function))
                    Case "employee"
                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Return x.employee
                                                        End Function))
                    Case "userfield"
                        Dim optSt As New roOptionsState
                        Dim joptions As New roOptions(optSt)

                        lVisit.AddRange(tmpList.OrderBy(Function(x)
                                                            Dim oField = x.fields.Find(Function(y) y.name = joptions.cardfield)
                                                            If oField Is Nothing Then
                                                                Return ""
                                                            Else
                                                                Return oField.value
                                                            End If
                                                        End Function))
                End Select
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitlist::search::" + ex.Message)
                Return False
            End Try
            Return True
        End Function

        Public Shared Function lastModification(Optional ByVal filter As Dictionary(Of String, String) = Nothing) As DateTime
            Dim last As DateTime = New Date(1970, 1, 1)
            Try

                Dim sSQL As String = "@SELECT# max(Modified) from Visit "
                If Not filter Is Nothing AndAlso filter.ContainsKey("date") Then
                    sSQL += " where " + SQLDateTime(Any2Time(filter("date")).DateOnly) + " between convert(datetime,convert(varchar(10),begindate,120),120) and dateadd(d,1,convert(datetime,convert(varchar(10),enddate,120),120))"
                    sSQL += " Or " + SQLDateTime(Any2Time(filter("date")).DateOnly) + " between convert(datetime,convert(varchar(10),begindate,120),120) and dateadd(d,1,convert(datetime,convert(varchar(10),begindate,120),120))"
                End If

                last = Robotics.VTBase.roTypes.Any2DateTime(ExecuteScalar(sSQL))
                If last.Year = 1 Then last = DateSerial(1970, 1, 1)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitlist::lastmodification::" + ex.Message)
            End Try
            Return last
        End Function

    End Class

    <DataContract>
    Public Class roVisitFieldList

        Private lFields As New List(Of roVisitField)
        Private oState As New roVisitState
        Private sResult As String = ""

        <DataMember()>
        Public Property fields As List(Of roVisitField)
            Get
                Return lFields
            End Get
            Set(value As List(Of roVisitField))
                lFields = lFields
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roVisitState, Optional ByVal bAudit As Boolean = False)
            If Not _State Is Nothing Then oState = _State
            load(bAudit)

        End Sub

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try
                Dim sSQL As String = "@SELECT# v.*, t.Name as NameType from Visit_Fields v left join Visit_Types t on t.IdType = v.VisitType where v.deleted=0 order by v.position asc "
                lFields.Clear()

                Dim tb As DataTable = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vsf As roVisitField
                    For Each row As DataRow In tb.Rows
                        vsf = New roVisitField
                        vsf.parseRow(row)
                        lFields.Add(vsf)
                    Next
                End If
                Dim vsfTime As roVisitField
                vsfTime = New roVisitField

                vsfTime.idfield = "timeNow"
                vsfTime.value = ""
                vsfTime.name = "Hora de inicio de visita"

                lFields.Add(vsfTime)
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfieldlist::load::" + ex.Message)
                Return False
            End Try

            Return True
        End Function

    End Class

    <DataContract>
    Public Class roVisitPunches

        Private sIDVisit As String = ""
        Private sIDVisitor As String = ""
        Private sVisitorName As String = ""
        Private dPunchDate As DateTime = New DateTime(1970, 1, 1)
        Private sAction As String = ""

        <DataMember()>
        Public Property idvisit As String
            Get
                Return sIDVisit
            End Get
            Set(value As String)
                sIDVisit = value
            End Set
        End Property

        <DataMember()>
        Public Property idvisitor As String
            Get
                Return sIDVisitor
            End Get
            Set(value As String)
                sIDVisitor = value
            End Set
        End Property

        <DataMember()>
        Public Property visitorname As String
            Get
                Return sVisitorName
            End Get
            Set(value As String)
                sVisitorName = value
            End Set
        End Property

        <DataMember()>
        Public Property punchdate As DateTime
            Get
                Return dPunchDate
            End Get
            Set(value As DateTime)
                dPunchDate = value
            End Set
        End Property

        <DataMember()>
        Public Property action As String
            Get
                Return sAction
            End Get
            Set(value As String)
                sAction = value
            End Set
        End Property

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("IDVisitor") Then sIDVisitor = Any2String(row.Item("IDVisitor"))
                If row.Table.Columns.Contains("IDVisit") Then sIDVisit = Any2String(Any2String(row.Item("IDVisit")))
                If row.Table.Columns.Contains("Name") Then sVisitorName = Any2String(row.Item("Name"))
                If row.Table.Columns.Contains("Action") Then sAction = Any2String(row.Item("Action"))
                If row.Table.Columns.Contains("DatePunch") Then dPunchDate = Any2DateTime(row.Item("DatePunch"))
            Catch ex As Exception

            End Try
        End Sub

    End Class

    <DataContract>
    Public Class roVisitType

        Private oState As New roVisitState
        Private iIDType As Integer
        Private sName As String = ""
        Private sResult As String = "NoError"

#Region "Property"

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        <DataMember()>
        Public Property idtype() As Integer
            Get
                Return iIDType
            End Get
            Set(ByVal Value As Integer)
                iIDType = Value
            End Set
        End Property

        <DataMember()>
        Public Property name() As String
            Get
                Return sName
            End Get
            Set(ByVal Value As String)
                sName = Value
            End Set
        End Property

#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal _IDType As Integer, ByVal _State As roVisitState,
                Optional ByVal bAudit As Boolean = False)
            iIDType = _IDType
            load(bAudit)
        End Sub

        Public Shared Function DeleteVisitType(idtype As String) As Boolean
            Try
                Dim sSQL As String

                sSQL = "@DELETE# FROM Visit_Types where idtype=" + idtype + " and idtype NOT IN(@SELECT# VisitType FROM Visit where VisitType is Not null) "

                ExecuteSql(sSQL)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisittype::deletevisittype::" + ex.Message)
                Return False
            End Try
            Return True
        End Function

        Private Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try
                If iIDType <> 0 Then

                    Dim sSQL As String = "@SELECT# * from Visit_Types where IDType='" + iIDType.ToString + "'"
                    Dim tb As DataTable = CreateDataTable(sSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        parseRow(tb.Rows(0))
                    Else
                    End If
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisittype::load::" + ex.Message)
            Finally

            End Try
            Return True
        End Function

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("IDType") Then iIDType = Any2String(row.Item("IDType"))
                If row.Table.Columns.Contains("Name") Then sName = HTML2String(Any2String(row.Item("Name")).Trim)
            Catch ex As Exception

            End Try
        End Sub

        Public Function save(Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String
                If iIDType = 0 Or iIDType = Nothing Then
                    sSQL = " @INSERT# INTO Visit_Types (name) "
                    sSQL += "values( '" + any2SQLHTML(sName) + "')"
                Else
                    sSQL = "@UPDATE# Visit_Types Set"
                    sSQL += " Name='" + any2SQLHTML(sName) + "' where IDType=" + Convert.ToString(iIDType) + ""
                End If
                Return ExecuteSql(sSQL)
            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisittype::save::" + ex.Message)
            End Try
            Return True

        End Function

    End Class

    <DataContract>
    Public Class roVisitTypeList

        Private lTypes As New List(Of roVisitType)
        Private oState As New roVisitState
        Private sResult As String = ""

        <DataMember()>
        Public Property types As List(Of roVisitType)
            Get
                Return lTypes
            End Get
            Set(value As List(Of roVisitType))
                lTypes = lTypes
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roVisitState, Optional ByVal bAudit As Boolean = False)
            If Not _State Is Nothing Then oState = _State
            load(bAudit)
        End Sub

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim sSQL As String = "@SELECT# * from Visit_Types"
                lTypes.Clear()

                Dim tb As DataTable = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vsf As roVisitType
                    For Each row As DataRow In tb.Rows
                        vsf = New roVisitType
                        vsf.parseRow(row)
                        lTypes.Add(vsf)
                    Next
                End If
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisittypelist::load::" + ex.Message)
                Return False
            End Try
            Return True
        End Function

    End Class

End Namespace