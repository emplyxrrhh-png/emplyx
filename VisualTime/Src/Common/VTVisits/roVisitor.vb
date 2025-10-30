Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase.roTypes

Namespace VTVisits

    <DataContract>
    Public Class roVisitor

        Private sIDVisitor As String
        Private sName As String = ""
        Private bPrivate As Boolean = False
        Private lFields As New List(Of roVisitorField)
        Private lPunches As New List(Of roVisitPunches)
        Private bRequiretFieldsOnCreateVisit As Boolean = False
        Private bRequiretFieldsOnStartVisit As Boolean = False
        Private Shared oState As New roVisitorState
        Private sResult As String = ""
        Private dSearchParams As New Dictionary(Of String, String)

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
        Public Property isprivate As Boolean
            Get
                Return bPrivate
            End Get
            Set(ByVal Value As Boolean)
                bPrivate = Value
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
        Public Property fields As List(Of roVisitorField)
            Get
                Return lFields
            End Get
            Set(value As List(Of roVisitorField))
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
        Public Property requiredfieldsoncreatevisit As Boolean
            Get
                Return bRequiretFieldsOnCreateVisit
            End Get
            Set(value As Boolean)
                bRequiretFieldsOnCreateVisit = value
            End Set
        End Property

        <DataMember()>
        Public Property requiredfieldsonstartvisit As Boolean
            Get
                Return bRequiretFieldsOnStartVisit
            End Get
            Set(value As Boolean)
                bRequiretFieldsOnStartVisit = value
            End Set
        End Property
        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.Result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal id As String, ByVal oPerm As Permission, ByVal _State As roVisitorState,
            Optional ByVal bAudit As Boolean = False, Optional SearchParams As Dictionary(Of String, String) = Nothing)
            sIDVisitor = id
            If Not SearchParams Is Nothing Then dSearchParams = SearchParams
            If Not _State Is Nothing Then oState = _State
            load(oPerm, bAudit)

        End Sub

        Private Function load(ByVal oPerm As Permission, Optional ByVal bAudit As Boolean = False) As Boolean
            Try

                Dim sSQL As String
                Dim tb As DataTable
                If sIDVisitor <> "new" Then
                    sSQL = "@SELECT# * from Visitor where idvisitor='" + sIDVisitor + "'"
                    tb = CreateDataTable(sSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        parseRow(tb.Rows(0))
                    Else
                        oState.Result = roVisitorState.ResultEnum.NotFound
                    End If
                End If

                If oState.Result <> roVisitorState.ResultEnum.NotFound Then

                    'Cargamos los campos personalizados
                    sSQL = "@SELECT# vf.*, isnull(vfv.value,'') as value, vfv.modified as modified, lap.Value as UniqueID "
                    sSQL += " from Visitor_Fields vf left outer join Visitor_Fields_Value vfv"
                    sSQL += "   on vfv.idfield=vf.idfield And vfv.idvisitor='" + sIDVisitor + "'"
                    sSQL += "  left outer join sysroLiveAdvancedParameters lap on vf.IDField=lap.value and ParameterName='vst_visitorUniqueIDField'"
                    sSQL += " where deleted=0 and visible=1 order by vf.position desc"
                    tb = CreateDataTable(sSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        lFields.Clear()
                        Dim fld As roVisitorField
                        For Each row As DataRow In tb.Rows
                            Dim _VisitorState As New roVisitorState(oState.IDPassport)
                            fld = New roVisitorField(_VisitorState)
                            fld.parseRow(row)
                            lFields.Add(fld)
                            If fld.required >= 1 Then
                                If fld.required = 2 And fld.value.Length = 0 Then
                                    bRequiretFieldsOnCreateVisit = True
                                ElseIf fld.required = 3 And fld.askevery = 2 And (fld.value.Length = 0 Or fld.modified.AddMinutes(5).Subtract(Now).TotalMinutes < 0) Then
                                    bRequiretFieldsOnStartVisit = True
                                ElseIf fld.required = 3 And (fld.value.Length = 0 Or fld.modified.AddDays(30).Subtract(Now).TotalDays < 0) Then
                                    bRequiretFieldsOnStartVisit = True
                                End If
                            End If
                        Next
                    End If

                    If Not (dSearchParams.ContainsKey("_loadpunches_") AndAlso Any2Boolean(dSearchParams("_loadpunches_")) = False) Then
                        If oPerm >= Permission.Admin Then
                            'Cargamos los movimientos
                            sSQL = "@SELECT# vvp.idvisit as idvisit, vvp.idvisitor as idvisitor, e.name as name, datepunch, action"
                            sSQL += " from Visit_Visitor_Punch vvp"
                            sSQL += " inner join Visit v on vvp.IDVisit = v.IDVisit "
                            sSQL += " inner join Employees e on v.IDEmployee = e.ID "
                            sSQL += " where vvp.idvisitor='" + sIDVisitor.ToString + "'"

                            tb = CreateDataTable(sSQL)
                            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                                lPunches.Clear()
                                Dim pun As New roVisitPunches
                                For Each row As DataRow In tb.Rows
                                    pun = New roVisitPunches
                                    pun.parseRow(row)
                                    lPunches.Add(pun)
                                Next
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.Result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitor::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("IDVisitor") Then sIDVisitor = Any2String(row.Item("IDVisitor"))
                If row.Table.Columns.Contains("CreatedBy") Then
                    If Any2String(row.Item("CreatedBy")) Is Nothing Or Any2String(row.Item("CreatedBy")) = "" Then
                        isprivate = False
                    Else
                        isprivate = True
                    End If
                End If

                If row.Table.Columns.Contains("Name") Then sName = Any2String(row.Item("Name"))
            Catch ex As Exception

            End Try
        End Sub

        Public Function save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim act As VTBase.Audit.Action
            Dim bRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim createdBy = oState.IDPassport

                Dim sSQL As String
                If ((sIDVisitor = "0" Or sIDVisitor = "" Or sIDVisitor = "new") AndAlso (bPrivate = False)) Then
                    act = VTBase.Audit.Action.aInsert
                    sIDVisitor = Guid.NewGuid().ToString()
                    sSQL = "@INSERT# INTO Visitor (idvisitor, name)"
                    sSQL += " values ('" + sIDVisitor.ToString + "', '" + any2SQLHTML(sName) + "')"

                ElseIf ((sIDVisitor = "0" Or sIDVisitor = "" Or sIDVisitor = "new") AndAlso (bPrivate = True)) Then
                    act = VTBase.Audit.Action.aInsert
                    sIDVisitor = Guid.NewGuid().ToString()
                    sSQL = "@INSERT# INTO Visitor (idvisitor, name, createdby)"
                    sSQL += " values ('" + sIDVisitor.ToString + "', '" + any2SQLHTML(sName) + "', '" + createdBy.ToString + "')"
                Else
                    If bPrivate = False Then
                        act = VTBase.Audit.Action.aUpdate
                        sSQL = "@UPDATE# Visitor set"
                        sSQL += " name='" + any2SQLHTML(sName) + "', CreatedBy=null"
                        sSQL += " where idvisitor='" + sIDVisitor.ToString + "'"
                    Else
                        act = VTBase.Audit.Action.aUpdate
                        sSQL = "@UPDATE# Visitor set"
                        sSQL += " name='" + any2SQLHTML(sName) + "', CreatedBy=" + createdBy.ToString + ""
                        sSQL += " where idvisitor='" + sIDVisitor.ToString + "'"
                    End If

                End If

                ExecuteSql(sSQL)

                For Each fl As roVisitorField In lFields
                    fl.idvisitor = sIDVisitor
                    fl.saveValue(bAudit)
                Next

                Try
                    Dim keys As New List(Of String)
                    Dim vals As New List(Of String)
                    keys.Add("name")
                    vals.Add(sName)
                    roLiveSupport.Audit(act, VTBase.Audit.ObjectType.tVisit, "", keys, vals, Nothing)
                Catch ex As Exception
                End Try
            Catch ex As Exception
                bRet = False
                oState.Result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitor::save::" + ex.Message)
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
            End Try
            Return True
        End Function

        Public Shared Function delete(ByVal _IDVisitor As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Try

                Dim sSQL As String
                sSQL = "@UPDATE# visitor set deleted=1 where idvisitor = '" + _IDVisitor + "'"
                If ExecuteSql(sSQL) Then
                    Try
                        Dim keys As New List(Of String)
                        Dim vals As New List(Of String)
                        keys.Add("idvisitor")
                        vals.Add(_IDVisitor)
                        roLiveSupport.Audit(VTBase.Audit.Action.aDelete, VTBase.Audit.ObjectType.tVisit, "", keys, vals, Nothing)
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As Exception
                oState.Result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitor::delete::" + ex.Message)
            End Try
            Return True
        End Function

        Public Shared Function getIDbyUniqueID(ByVal UniqueID As String) As String
            Dim resp As String = ""

            Try

                Dim sSQL As String = "@SELECT# top 1 v.idvisitor from visitor v"
                sSQL += " inner join Visitor_Fields_Value vfv on v.idvisitor=vfv.idvisitor and vfv.value='" + UniqueID + "'"
                sSQL += " inner join sysroLiveAdvancedParameters lap on vfv.IDField=lap.value and ParameterName='vst_visitorUniqueIDField'"

                Return Any2String(ExecuteScalar(sSQL))
            Catch ex As Exception
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return resp
        End Function

    End Class

    <DataContract()>
    Public Class roVisitorState
        Inherits roBusinessState

        <Flags()>
        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            SqlError
            NotFound
            NotPermissions
            BeginOld
        End Enum

#Region "Declarations - Constructor"

        Private intResult As ResultEnum

        Public Sub New()
            MyBase.New("VTBusiness.Visits", "Visits")
            Me.intResult = ResultEnum.NoError
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
        Public Property Result() As ResultEnum
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
    Public Class roVisitorField

        Private oState As New roVisitorState()
        Private sIDField As String
        Private sIDVisitor As String
        Private sName As String = ""
        Private bVisible As Boolean = True
        Private iRequired As Byte = 0
        Private iAskEvery As Byte = 0
        Private iPosition As Byte = 0
        Private iEdit As Boolean = 0
        Private sValue As String = ""
        Private sValues As String = ""
        Private dModified As DateTime = New DateTime(1970, 1, 1)
        Private sResult As String = "NoError"

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
        Public Property idvisitor() As String
            Get
                Return sIDVisitor
            End Get
            Set(ByVal Value As String)
                sIDVisitor = Value
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
        Public Property edit() As Boolean
            Get
                Return iEdit
            End Get
            Set(ByVal Value As Boolean)
                iEdit = Value
            End Set
        End Property

        <DataMember()>
        Public Property askevery() As Byte
            Get
                Return iAskEvery
            End Get
            Set(ByVal Value As Byte)
                iAskEvery = Value
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
        Public Property modified() As DateTime
            Get
                If dModified.Year < 1970 Then
                    dModified = New DateTime(1970, 1, 1)
                End If
                Return dModified
            End Get
            Set(ByVal Value As DateTime)
                dModified = Value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.Result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roVisitorState)
            If Not _State Is Nothing Then oState = _State
        End Sub

        Public Sub New(ByVal _IDField As String, ByVal _State As roVisitorState)
            sIDField = _IDField
            If Not _State Is Nothing Then oState = _State
            load()
        End Sub

        Private Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try
                If sIDField <> "0" And sIDField <> "" And sIDField <> "new" Then

                    Dim sSQL As String = "@SELECT# * from Visitor_Fields where idfield='" + sIDField + "'"
                    Dim tb As DataTable = CreateDataTable(sSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        parseRow(tb.Rows(0))
                    Else
                        oState.Result = roVisitState.ResultEnum.NotFound
                    End If
                End If
            Catch ex As Exception
                oState.Result = roVisitState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::load::" + ex.Message)
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
                If row.Table.Columns.Contains("Edit") Then iEdit = Any2Boolean(row.Item("Edit"))
                If row.Table.Columns.Contains("Required") Then iRequired = Any2Integer(row.Item("Required"))
                If row.Table.Columns.Contains("AskEvery") Then iAskEvery = Any2Integer(row.Item("AskEvery"))
                If row.Table.Columns.Contains("Position") Then iPosition = Any2Integer(row.Item("Position"))
                If row.Table.Columns.Contains("Value") Then sValue = HTML2String(row.Item("Value")).Trim
                If row.Table.Columns.Contains("Values") Then sValues = HTML2String(row.Item("Values")).Trim
                If row.Table.Columns.Contains("Modified") Then dModified = Any2DateTime(row.Item("Modified"))
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
                    iPosition = Any2Integer(ExecuteScalar("@SELECT# max(position) from Visitor_Fields where deleted=0")) + 1
                    sSQL = " @INSERT# INTO Visitor_Fields (IDField, name, Visible, Required, AskEvery, position, [values], edit)"
                    sSQL += "values('" + sIDField.ToString + "', '" + any2SQLHTML(sName) + "'," + IIf(bVisible, "1", "0") + ", "
                    sSQL += iRequired.ToString + "," + iAskEvery.ToString + "," + iPosition.ToString + ", '" + any2SQLHTML(sValues) + "', " & If(iEdit, "1", "0") & ")"
                Else
                    sSQL = "@UPDATE# Visitor_Fields set"
                    sSQL += " Name='" + any2SQLHTML(sName) + "'"
                    sSQL += " ,Visible=" + IIf(bVisible, "1", "0")
                    sSQL += " ,Required=" + iRequired.ToString
                    sSQL += " ,AskEvery=" + iAskEvery.ToString
                    sSQL += " ,[values]='" + any2SQLHTML(sValues) + "'"
                    sSQL += " ,edit=" + If(iEdit, "1", "0")
                    sSQL += " where IDField='" + sIDField + "'"
                End If
                Return ExecuteSql(sSQL)
            Catch ex As Exception
                'Cerramos conexión si es necesario

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::save::" + ex.Message)
            End Try
            Return True
        End Function

        Public Function saveValue(Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim sSQL As String
                sSQL = " @INSERT# INTO Visitor_Fields_Value (IDField,IDVisitor,Value, Modified)"
                sSQL += "values('" + sIDField.ToString + "', '" + sIDVisitor.ToString + "','" + any2SQLHTML(sValue) + "',getdate())"

                Try
                    ExecuteSql(sSQL)
                Catch ex As Exception
                    sSQL = "@UPDATE# Visitor_Fields_Value set"
                    sSQL += " Value='" + any2SQLHTML(sValue) + "', Modified=getdate()"
                    sSQL += " where IDField='" + sIDField.ToString + "'"
                    sSQL += " and IDVisitor='" + sIDVisitor.ToString + "'"
                    Return ExecuteSql(sSQL)
                End Try
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::savevalue::" + ex.Message)
            End Try
            Return True
        End Function

        Public Shared Function DeleteVisitField(idfield As String) As Boolean
            Try
                Dim sSQL As String

                sSQL = "@UPDATE# Visitor_Fields set deleted=1 where idfield='" + idfield + "'"
                ExecuteSql(sSQL)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::deletevisitfield::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Shared Function upVisitorField(idfield As String) As Boolean
            Try
                Dim sSQL As String

                sSQL = "@SELECT# position from Visitor_Fields where idfield='" + idfield + "'"
                Dim currpos As Integer = ExecuteScalar(sSQL)
                If currpos >= 2 Then
                    sSQL = "@SELECT# max(position) from Visitor_Fields where position<" + currpos.ToString
                    Dim uppos As Integer = ExecuteScalar(sSQL)
                    sSQL = "@UPDATE# Visitor_Fields set position=" + currpos.ToString + " where position=" + uppos.ToString
                    ExecuteSql(sSQL)
                    sSQL = "@UPDATE# Visitor_Fields set position=" + uppos.ToString + " where idfield='" + idfield + "'"
                    ExecuteSql(sSQL)

                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::upvisitorfield::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Shared Function downVisitorField(idfield As String) As Boolean
            Try
                Dim sSQL As String

                sSQL = "@SELECT# position from Visitor_Fields where idfield='" + idfield + "'"
                Dim currpos As Integer = ExecuteScalar(sSQL)
                sSQL = "@SELECT# max(position) from Visitor_Fields"
                Dim maxpos As Integer = ExecuteScalar(sSQL)
                If currpos < maxpos Then
                    sSQL = "@SELECT# min(position) from Visitor_Fields where position>" + currpos.ToString
                    Dim downpos As Integer = ExecuteScalar(sSQL)
                    sSQL = "@UPDATE# Visitor_Fields set position=" + currpos.ToString + " where position=" + downpos.ToString
                    ExecuteSql(sSQL)
                    sSQL = "@UPDATE# Visitor_Fields set position=" + downpos.ToString + " where idfield='" + idfield.ToString + "'"
                    ExecuteSql(sSQL)

                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfield::downvisitorfield::" + ex.Message)

                Return False
            End Try

            Return True
        End Function

        Private Function getNewUniqueID() As String
            Dim id As String = ""
            Try
                Dim exist As Boolean = True
                Dim i As Integer = 0

                id = RandomString(roOptions.VisitorUniqueIDFieldLen, roOptions.UniqueIDType)

                Dim sSQL As String
                Dim tb As New DataTable
                While exist And i < 100
                    sSQL = "@SELECT# * from Visitor_Fields_Value vfv  inner join Visitor v on v.idvisitor=vfv.idvisitor where v.deleted=0 and vfv.idfield='" + roOptions.visitorUniqueIDFieldParam + "'"
                    sSQL += " and value = '" + id + "'"
                    tb = CreateDataTable(sSQL)
                    If tb.Rows.Count = 0 Then
                        exist = False
                    Else
                        id = RandomString(roOptions.VisitorUniqueIDFieldLen, roOptions.UniqueIDType)
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
    Public Class roVisitorList

        Private lVisitor As New List(Of roVisitor)
        Private dFilter As New Dictionary(Of String, String)
        Private oState As New roVisitorState
        Private sResult As String = ""

        Private oPerm As Permission

        <DataMember()>
        Public Property visitors As List(Of roVisitor)
            Get
                Return lVisitor
            End Get
            Set(value As List(Of roVisitor))
                lVisitor = lVisitor
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
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.Result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()
            Me.oPerm = Security.WLHelper.GetFeaturePermission(Nothing, "Visits", "U")
        End Sub

        Public Sub New(ByVal _State As roVisitorState, Optional ByVal bAudit As Boolean = False)
            If Not _State Is Nothing Then oState = _State

            Me.oPerm = Security.WLHelper.GetFeaturePermission(oState.IDPassport, "Visits", "U")

            load(bAudit)
        End Sub

        Public Function load(Optional ByVal bAudit As Boolean = False, Optional _State As roVisitorState = Nothing) As Boolean

            Try
                'Filtro por campos personalizados
                Dim cNamVal As Dictionary(Of String, String) = New Dictionary(Of String, String)
                For Each nam As KeyValuePair(Of String, String) In dFilter
                    If nam.Key(0) <> "_" And nam.Key <> "name" And nam.Key <> "ids" Then
                        If nam.Value.Length > 0 Then
                            cNamVal.Add(nam.Key, nam.Value)
                        End If
                    End If
                Next

                Dim sSQL As String = "@SELECT# v.idvisitor, v.CreatedBy from Visitor v"
                If cNamVal.Count > 0 Then
                    sSQL += " inner join (@SELECT# IDVisitor from [dbo].[Visitor_Fields] vf left outer join [dbo].[Visitor_Fields_Value] vfv on vf.IDField = vfv.IDField where vf.deleted = 0 And visible = 1"
                    For Each nam As KeyValuePair(Of String, String) In cNamVal
                        If nam.Key(0) <> "_" Then sSQL += " and (vfv.IDField='" + nam.Key + "' and vfv.value like '" + any2SQLHTML(nam.Value).Replace("*", "%").Replace("=", "").Replace("'", "") + "')"
                    Next
                    sSQL += ") as vals on v.idvisitor=vals.idvisitor "
                End If

                sSQL += " where deleted=0 and (CreatedBy is null or CreatedBy=" + _State.IDPassport.ToString + ")"

                If dFilter.ContainsKey("name") AndAlso dFilter("name").Length > 0 Then
                    sSQL += " and name like '" + dFilter("name").Replace("*", "%").Replace("=", "").Replace("'", "") + "'"
                End If
                If dFilter.ContainsKey("ids") Then
                    If dFilter("ids").Length = 0 Then Return True
                    sSQL += " and idvisitor in ("
                    Dim dfirst As Boolean = True
                    For Each id As String In dFilter("ids").Split(";")
                        If id.Length > 0 Then
                            sSQL += IIf(Not dfirst, ", ", "") + "'" + id + "'"
                            dfirst = False
                        End If
                    Next
                    sSQL += ")"
                End If
                sSQL += " order by name"

                Dim tmpList As New Generic.List(Of roVisit)

                Dim tb As DataTable = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vss As New roVisitorState

                    loadVisitors(tb, tmpList, vss)

                End If
            Catch ex As Exception
                oState.Result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorlist::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Sub loadVisitors(tb As Object, tmpList As Object, vss As roVisitorState)
            Dim vs As roVisitor
            For i As Integer = 0 To tb.Rows.Count - 1
                vs = New roVisitor(tb.Rows(i).Item("idvisitor"), Me.oPerm, vss, , dFilter)
                lVisitor.Add(vs)
            Next
        End Sub

    End Class

    <DataContract>
    Public Class roVisitorFieldList

        Private lFields As New List(Of roVisitorField)
        Private oState As New roVisitorState
        Private sResult As String = ""

        <DataMember()>
        Public Property fields As List(Of roVisitorField)
            Get
                Return lFields
            End Get
            Set(value As List(Of roVisitorField))
                lFields = lFields
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return IIf(sResult.Length > 0, sResult, oState.Result.ToString)
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roVisitorState,
            Optional ByVal bAudit As Boolean = False)
            If Not _State Is Nothing Then oState = _State
            load(bAudit)

        End Sub

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim sSQL As String = "@SELECT# * from Visitor_Fields where deleted=0 order by position asc "
                lFields.Clear()

                Dim tb As DataTable = CreateDataTable(sSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim vsf As roVisitorField
                    For Each row As DataRow In tb.Rows
                        vsf = New roVisitorField(oState)
                        vsf.parseRow(row)
                        lFields.Add(vsf)
                    Next
                End If
            Catch ex As Exception
                oState.Result = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitorfieldlist::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

    End Class

End Namespace