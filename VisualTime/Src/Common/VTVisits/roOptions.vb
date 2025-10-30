Imports System.Runtime.Serialization
Imports System.Text
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace VTVisits

    <DataContract>
    Public Class roOptions

        Private iNotification As Byte
        Private Const notificationParam = "vst_Notification"

        Private sCardFieldId As String
        Private sCardField As String
        Private Const cardFieldParam = "vst_CardField"

        Private sCardNumberField As String
        Private Const cardNumberFieldParam = "vst_CardNumberField"

        Private sMultilocationField As String
        Private Const multilocationFieldParam = "vst_MultilocationField"

        Private sUniqueIDField As String
        Private Const UniqueIDFieldParam = "vst_UniqueIDField"

        Private sZoneValueField As String
        Private Const zoneValueFieldParam = "vst_ZoneValue"

        Private sAllowVisitFieldModify As String
        Private Const AllowVisitFieldModifyParam = "vst_AllowVisitFieldModify"

        Private sDeleteDataVisitorsField As String
        Private Const deleteDataVisitorsFieldParam = "vst_DeleteDataVisitors"
        Private Const deleteDataVisitorsDefauktValue = "90"

        Private sDocumentNumberField As String
        Private Const documentNumberFieldParam = "vst_DocumentNumberField"

        Private sScanServicePort As String
        Private Const scanServicePortParam = "vst_ScanServicePort"

        Private sScanServiceUser As String
        Private Const scanServiceUserParam = "vst_ScanServiceUser"

        Private sScanServicePwd As String
        Private Const scanServicePwdParam = "vst_ScanServicePwd"

        Private Const FieldParamNotApply = "-1"

        Private sVisitUniqueIDField As String
        Public Const visitUniqueIDFieldParam = "vst_visitUniqueIDField"
        Public Const visitUniqueIDFieldLen = 3

        Private sVisitorUniqueIDField As String
        Public Const visitorUniqueIDFieldParam = "vst_visitorUniqueIDField"
        Public Const VisitorUniqueIDFieldLen = 4

        Public Const UniqueIDType As eStrType = eStrType.Numeric

        Private sResult As String = ""
        Private oState As roOptionsState

        ' valores
        ' 0: Sin notificaciones
        ' 1: Notificar al responsable
        ' 2: Notificar al usuario

        <DataMember()>
        Public Property notification As Byte
            Get
                Return iNotification
            End Get
            Set(value As Byte)
                iNotification = value
            End Set
        End Property

        <DataMember()>
        Public Property cardfield As String
            Get
                Return sCardField
            End Get
            Set(value As String)
                sCardField = value
            End Set
        End Property

        <DataMember()>
        Public Property documentNumberfield As String
            Get
                Return sDocumentNumberField
            End Get
            Set(value As String)
                sDocumentNumberField = value
            End Set
        End Property

        <DataMember()>
        Public Property scanServicePort As String
            Get
                Return sScanServicePort
            End Get
            Set(value As String)
                sScanServicePort = value
            End Set
        End Property

        <DataMember()>
        Public Property scanServiceUser As String
            Get
                Return sScanServiceUser
            End Get
            Set(value As String)
                sScanServiceUser = value
            End Set
        End Property

        <DataMember()>
        Public Property scanServicePwd As String
            Get
                Return sScanServicePwd
            End Get
            Set(value As String)
                sScanServicePwd = value
            End Set
        End Property

        <DataMember()>
        Public Property cardfieldid As String
            Get
                Return sCardFieldId
            End Get
            Set(value As String)
                sCardFieldId = value
            End Set
        End Property

        <DataMember()>
        Public Property cardNumberField As String
            Get
                Return sCardNumberField
            End Get
            Set(value As String)
                sCardNumberField = value
            End Set
        End Property

        <DataMember()>
        Public Property multilocationField As String
            Get
                Return sMultilocationField
            End Get
            Set(value As String)
                sMultilocationField = value
            End Set
        End Property

        <DataMember()>
        Public Property uninqueIDField As String
            Get
                Return sUniqueIDField
            End Get
            Set(value As String)
                sUniqueIDField = value
            End Set
        End Property

        <DataMember()>
        Public Property visitUniqueIDField As String
            Get
                Return sVisitUniqueIDField
            End Get
            Set(value As String)
                sVisitUniqueIDField = value
            End Set
        End Property

        <DataMember()>
        Public Property visitorUniqueIDField As String
            Get
                Return sVisitorUniqueIDField
            End Get
            Set(value As String)
                sVisitorUniqueIDField = value
            End Set
        End Property

        <DataMember()>
        Public Property zoneValueField As String
            Get
                Return sZoneValueField
            End Get
            Set(value As String)
                sZoneValueField = value
            End Set
        End Property

        <DataMember()>
        Public Property allowvisitfieldmodify As String
            Get
                Return sAllowVisitFieldModify
            End Get
            Set(value As String)
                sAllowVisitFieldModify = value
            End Set
        End Property

        <DataMember()>
        Public Property deleteDataVisitorsField As String
            Get
                Return sDeleteDataVisitorsField
            End Get
            Set(value As String)
                sDeleteDataVisitorsField = value
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

        Public Sub New(ByVal _State As roOptionsState, Optional ByVal bAudit As Boolean = False)
            Try
                If Not _State Is Nothing Then oState = _State Else oState = New roOptionsState()
                cardNumberField = FieldParamNotApply
                zoneValueField = FieldParamNotApply
                deleteDataVisitorsField = deleteDataVisitorsDefauktValue
                load(bAudit)
            Catch ex As Exception
                oState.result = roVisitState.ResultEnum.Exception
            End Try

        End Sub

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim parameters = New List(Of CommandParameter) From
            {
                New CommandParameter("@pattern", CommandParameter.ParameterType.tString, "vst_%")
            }

                Dim sSQL = "@SELECT# value,ParameterName from sysroLiveAdvancedParameters where ParameterName like @pattern"

                Dim tb As DataTable = CreateDataTable(sSQL, parameters)

                'rtort - 20160526 - add html encode on options for compare
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each row As DataRow In tb.Rows
                        Select Case row("ParameterName")
                            Case notificationParam
                                iNotification = Any2Integer(row("value"))
                            Case cardFieldParam
                                sCardField = HTML2String(Any2String(row("value")))
                                sCardFieldId = Robotics.VTBase.roTypes.Any2String(ExecuteScalar("@SELECT# IDField from Visit_Fields where Name = '" & sCardField & "'"))
                            Case cardNumberFieldParam
                                sCardNumberField = HTML2String(Any2String(row("value")))
                            Case zoneValueFieldParam
                                zoneValueField = HTML2String(Any2String(row("value")))
                            Case deleteDataVisitorsFieldParam
                                deleteDataVisitorsField = Any2String(row("value"))
                            Case AllowVisitFieldModifyParam
                                sAllowVisitFieldModify = Any2String(row("value"))
                            Case documentNumberFieldParam
                                sDocumentNumberField = Any2String(row("value"))
                            Case scanServicePortParam
                                sScanServicePort = Any2String(row("value"))
                            Case scanServiceUserParam
                                sScanServiceUser = Any2String(row("value"))
                            Case scanServicePwdParam
                                sScanServicePwd = Any2String(row("value"))
                            Case multilocationFieldParam
                                sMultilocationField = Any2String(row("value"))
                            Case UniqueIDFieldParam
                                sUniqueIDField = Any2String(row("value"))
                            Case visitUniqueIDFieldParam
                                sVisitUniqueIDField = Any2String(row("value"))
                            Case visitorUniqueIDFieldParam
                                sVisitorUniqueIDField = Any2String(row("value"))
                        End Select
                    Next
                End If
            Catch ex As Exception
                sResult = roOptionsState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rooptions::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

        Public Function save(values As roOptions, Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                'save options
                If iNotification <> values.notification Then SaveOption(notificationParam, values.notification.ToString)
                If sCardField <> values.cardfield Then SaveOption(cardFieldParam, values.cardfield)
                If sCardNumberField <> values.sCardNumberField Then SaveOption(cardNumberFieldParam, values.sCardNumberField)
                If sZoneValueField <> values.sZoneValueField Then SaveOption(zoneValueFieldParam, values.sZoneValueField)
                If sDeleteDataVisitorsField <> values.sDeleteDataVisitorsField Then SaveOption(deleteDataVisitorsFieldParam, values.sDeleteDataVisitorsField)
                If sAllowVisitFieldModify <> values.sAllowVisitFieldModify Then SaveOption(AllowVisitFieldModifyParam, values.sAllowVisitFieldModify)
                If sDocumentNumberField <> values.sDocumentNumberField Then SaveOption(documentNumberFieldParam, values.sDocumentNumberField)
                If sScanServicePort <> values.sScanServicePort Then SaveOption(scanServicePortParam, values.sScanServicePort)
                If sMultilocationField <> values.sMultilocationField Then SaveOption(multilocationFieldParam, values.multilocationField)
                If sUniqueIDField <> values.sUniqueIDField Then SaveOption(UniqueIDFieldParam, values.uninqueIDField)
                If sVisitUniqueIDField <> values.sVisitUniqueIDField Then SaveOption(visitUniqueIDFieldParam, values.sVisitUniqueIDField)
                If sVisitorUniqueIDField <> values.sVisitorUniqueIDField Then SaveOption(visitorUniqueIDFieldParam, values.sVisitorUniqueIDField)
            Catch ex As Exception
                oState.result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rooptions::save::" + ex.Message)
            End Try

            Return True
        End Function

        Private Function SaveOption(name As String, value As String, Optional bAudit As Boolean = False) As Boolean
            Try

                Dim sSQL As New StringBuilder

                sSQL.Append("MERGE sysroLiveAdvancedParameters AS t ")
                sSQL.Append("USING (@SELECT# top 1 @parametername as ParameterName,@value as Value from sysroLiveAdvancedParameters) AS S ON (t.ParameterName = S.ParameterName) ")
                sSQL.Append("WHEN MATCHED THEN @UPDATE# SET T.Value = S.Value ")
                sSQL.Append("WHEN NOT MATCHED THEN @INSERT# (ParameterName, Value) VALUES (@parametername,@value);")

                Dim parameters = New List(Of CommandParameter) From
            {
                New CommandParameter("@parametername", CommandParameter.ParameterType.tString, name),
                New CommandParameter("@value", CommandParameter.ParameterType.tString, any2SQLHTML(value))
            }

                Try
                    ExecuteSql(sSQL.ToString(), parameters)
                Catch ex As Exception
                Finally
                    sSQL.Clear()
                End Try

                Return True
            Catch ex As Exception
                oState.result = roVisitorState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rooptions::saveoption::" + ex.Message)

                Return False
            End Try
        End Function

    End Class

    <DataContract()>
    Public Class roOptionsState
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
    Public Class roPrintConfig

        Private iid As Int32 = 0
        Private sname As String = ""
        Private svalue As String = ""
        Private sResult As String = ""
        Private sIdVisit As String = ""
        Private oState As roOptionsState

        <DataMember()>
        Public Property id As Int32
            Get
                Return iid
            End Get
            Set(value As Int32)
                iid = value
            End Set
        End Property
        <DataMember()>
        Public Property name As String
            Get
                Return sname
            End Get
            Set(value As String)
                sname = value
            End Set
        End Property
        <DataMember()>
        Public Property idVisit As String
            Get
                Return sIdVisit
            End Get
            Set(value As String)
                sIdVisit = value
            End Set
        End Property

        <DataMember()>
        Public Property value As String
            Get
                Return svalue
            End Get
            Set(value As String)
                svalue = value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roOptionsState,
                Optional ByVal bAudit As Boolean = False)
            load(bAudit)
        End Sub

        Public Function savePrintConfig(values As roPrintConfig, Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                values.value = values.value.Replace("'", "''")

                Dim sSQL As String = "@SELECT# * from Visit_Print_Config"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    sSQL = "@UPDATE# Visit_Print_Config set"
                    sSQL += " Value='" + Any2String(values.value) + "'"
                    ' sSQL += " where ID='" + values.id + "'"
                Else

                    sSQL = " @INSERT# INTO Visit_Print_Config (ID,Value)"
                    sSQL += "values(1, '" + values.value + "')"

                End If

                Try
                    ExecuteSql(sSQL)
                Catch ex As Exception

                End Try
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roOptions::savePrintConfig::" + ex.Message)
            End Try
            Return True
        End Function

        Public Function getPrintConfig(values As roPrintConfig, Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim visit = New roVisit(values.idVisit, Nothing, Nothing, Nothing)

                Dim sSQL As String = "@SELECT# * from Visit_Print_Config"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each row As roVisitor In visit.visitors
                        parseRowForPrint(tb.Rows(0), visit, row)
                    Next row
                Else
                End If

                ' load(oActiveTransaction, bAudit)

                Dim meTemp = Me
            Catch ex As Exception
                'Cerramos conexión si es necesario

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roOptions::savePrintConfig::" + ex.Message)
            End Try
            Return True
        End Function

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String = "@SELECT# * from Visit_Print_Config"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    parseRow(tb.Rows(0))
                Else

                End If
            Catch ex As Exception
                oState.result = roOptionsState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::load::" + ex.Message)
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return True
        End Function

        Public Sub parseRowForPrint(ByVal row As DataRow, ByVal visit As roVisit, ByVal visitor As roVisitor)
            Try

                Dim strSQL = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'vst_visitUniqueIDField'"
                Dim visitID = VTBase.roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL))

                Dim strSQLVisitor = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'vst_visitorUniqueIDField'"
                Dim visitorID = VTBase.roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQLVisitor))

                Dim valuesString = HTML2String(row.Item("Value")).ToString.Replace("{Hora actual}", Date.Now())
                valuesString = valuesString.Replace("{Fecha}", visit.begindate)
                valuesString = valuesString.Replace("{Visitante}", visitor.name)
                valuesString = valuesString.Replace("{Asunto}", visit.name)
                valuesString = valuesString.Replace("{Visitado}", visit.employee)
                valuesString = valuesString.Replace("{ID Visita}", visit.fields.Find(Function(p) p.idfield = visitID).value)
                valuesString = valuesString.Replace("{ID Visitante}", visitor.fields.Find(Function(p) p.idfield = visitorID).value)

                If row.Table.Columns.Contains("Id") Then iid = Any2String(row.Item("Id"))
                If row.Table.Columns.Contains("Name") Then sname = HTML2String(row.Item("Name"))
                If row.Table.Columns.Contains("Value") Then value += "<div style=""border:1px solid black; padding:10px; page-break-before: always;"">" + valuesString + "</div>"
            Catch ex As Exception

            End Try
        End Sub

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("Id") Then iid = Any2String(row.Item("Id"))
                If row.Table.Columns.Contains("Name") Then sname = HTML2String(row.Item("Name"))
                If row.Table.Columns.Contains("Value") Then value += HTML2String(row.Item("Value"))
            Catch ex As Exception

            End Try
        End Sub

    End Class

    <DataContract>
    Public Class roVisitLaws

        Private iid As Int32 = 0
        Private sTitle1 As String = ""
        Private sValue1 As String = ""
        Private sTitle2 As String = ""
        Private sValue2 As String = ""
        Private sResult As String = ""
        Private oState As roOptionsState

        <DataMember()>
        Public Property id As Int32
            Get
                Return iid
            End Get
            Set(value As Int32)
                iid = value
            End Set
        End Property
        <DataMember()>
        Public Property title1 As String
            Get
                Return sTitle1
            End Get
            Set(value As String)
                sTitle1 = value
            End Set
        End Property
        <DataMember()>
        Public Property value1 As String
            Get
                Return sValue1
            End Get
            Set(value As String)
                sValue1 = value
            End Set
        End Property

        <DataMember()>
        Public Property title2 As String
            Get
                Return sTitle2
            End Get
            Set(value As String)
                sTitle2 = value
            End Set
        End Property
        <DataMember()>
        Public Property value2 As String
            Get
                Return sValue2
            End Get
            Set(value As String)
                sValue2 = value
            End Set
        End Property
        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal _State As roOptionsState,
                Optional ByVal bAudit As Boolean = False)
            load(bAudit)
        End Sub

        Public Function saveVisitLaws(values As roVisitLaws, Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                values.value1 = values.value1.Replace("'", "''")
                values.title1 = values.title1.Replace("'", "''")
                values.value2 = values.value2.Replace("'", "''")
                values.title2 = values.title2.Replace("'", "''")

                Dim sSQL As String = "@SELECT# * from Visit_Legal_Texts"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    sSQL = "@UPDATE# Visit_Legal_Texts set"
                    sSQL += " Title1='" + Any2String(values.title1) + "', Value1='" + Any2String(values.value1) + "',  Title2='" + Any2String(values.title2) + "', Value2='" + Any2String(values.value2) + "'"
                    ' sSQL += " where ID='" + values.id + "'"
                Else

                    sSQL = " @INSERT# INTO Visit_Legal_Texts (ID,Title1, Value1, Title2, Value2)"
                    sSQL += "values(1, '" + values.title1 + "', '" + values.value1 + "', '" + values.title2 + "', '" + values.value2 + "')"

                End If

                Try
                    ExecuteSql(sSQL)
                Catch ex As Exception

                End Try
            Catch ex As Exception
                'Cerramos conexión si es necesario

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roOptions::saveVisitLaws::" + ex.Message)
            End Try
            Return True
        End Function

        Public Function getVisitLaws(values As roVisitLaws, Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String = "@SELECT# * from Visit_Legal_Texts"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    'For Each row As roVisitor In visit.visitors
                    '    parseRowForPrint(tb.Rows(0), visit, row)
                    'Next row
                Else
                    oState.result = roOptionsState.ResultEnum.NotFound
                End If

                ' load(oActiveTransaction, bAudit)

                Dim meTemp = Me
            Catch ex As Exception
                'Cerramos conexión si es necesario

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roOptions::saveVisitLaws::" + ex.Message)
            End Try
            Return True
        End Function

        Public Function load(Optional ByVal bAudit As Boolean = False) As Boolean

            Try

                Dim sSQL As String = "@SELECT# * from Visit_Legal_Texts"
                Dim tb As DataTable = CreateDataTable(sSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    parseRow(tb.Rows(0))
                Else

                End If
            Catch ex As Exception
                oState.result = roOptionsState.ResultEnum.SqlError

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::rovisitfield::load::" + ex.Message)
            Finally
                'Cerramos conexión si es necesario

            End Try
            Return True
        End Function

        Public Sub parseRowForPrint(ByVal row As DataRow, ByVal visit As roVisit, ByVal visitor As roVisitor)
            Try

                If row.Table.Columns.Contains("Id") Then iid = Any2String(row.Item("Id"))
                If row.Table.Columns.Contains("Title1") Then title1 = HTML2String(row.Item("Title1"))
                If row.Table.Columns.Contains("Value1") Then value1 = HTML2String(row.Item("Value1"))
                If row.Table.Columns.Contains("Title2") Then title2 = HTML2String(row.Item("Title2"))
                If row.Table.Columns.Contains("Value2") Then value2 = HTML2String(row.Item("Value2"))
            Catch ex As Exception

            End Try
        End Sub

        Public Sub parseRow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("Id") Then iid = Any2String(row.Item("Id"))
                If row.Table.Columns.Contains("Title1") Then title1 = HTML2String(row.Item("Title1"))
                If row.Table.Columns.Contains("Value1") Then value1 = HTML2String(row.Item("Value1"))
                If row.Table.Columns.Contains("Title2") Then title2 = HTML2String(row.Item("Title2"))
                If row.Table.Columns.Contains("Value2") Then value2 = HTML2String(row.Item("Value2"))
            Catch ex As Exception

            End Try
        End Sub

    End Class

End Namespace