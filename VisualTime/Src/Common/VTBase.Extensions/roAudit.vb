Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.Audit

<DataContract()>
Public Class roAudit

#Region "Declarations - Constructor"

    Private intID As Integer
    Private strUserName As String
    Private xDate As DateTime
    Private oAction As Action
    Private oObjectType As ObjectType
    Private strObjectName As String = ""
    Private tbParameters As DataTable
    Private intSessionID As Integer
    Private strClientLocation As String

    Private strLanguageFileReference As String

    Private oLanguage As roLanguage

    Public Sub New()
        Me.oLanguage = New roLanguage
        Me.intID = -1
        Me.xDate = Now
        Me.oAction = Action.aNone
        Me.oObjectType = ObjectType.tNone
        Me.strLanguageFileReference = String.Empty
        Me.strObjectName = ""
    End Sub

    Public Sub New(ByVal _HttpRequest As System.Web.HttpRequest, ByVal _UserName As String,
                       Optional ByVal _Action As Action = Action.aNone, Optional ByVal _ObjectType As ObjectType = ObjectType.tNone, Optional ByVal _ObjectName As String = "",
                       Optional ByVal _Parameters As DataTable = Nothing,
                       Optional ByVal _SessionID As Integer = -1)

        Me.oLanguage = New roLanguage
        Me.strClientLocation = Me.GetClientAddress(_HttpRequest)
        Me.strUserName = _UserName
        Me.intID = -1
        Me.xDate = Now
        Me.oAction = _Action
        Me.oObjectType = _ObjectType
        Me.strObjectName = _ObjectName
        Me.tbParameters = _Parameters
        Me.intSessionID = _SessionID
        Me.strLanguageFileReference = String.Empty
    End Sub

    Public Sub New(ByVal _ClientLocation As String, ByVal _UserName As String,
               Optional ByVal _Action As Action = Action.aNone, Optional ByVal _ObjectType As ObjectType = ObjectType.tNone, Optional ByVal _ObjectName As String = "",
               Optional ByVal _Parameters As DataTable = Nothing,
               Optional ByVal _SessionID As Integer = -1)

        Me.oLanguage = New roLanguage
        Me.strClientLocation = _ClientLocation
        Me.strUserName = _UserName
        Me.intID = -1
        Me.xDate = Now
        Me.oAction = _Action
        Me.oObjectType = _ObjectType
        Me.strObjectName = _ObjectName
        Me.tbParameters = _Parameters
        Me.intSessionID = _SessionID
        Me.strLanguageFileReference = String.Empty
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
        End Set
    End Property
    <DataMember()>
    Public Property UserName() As String
        Get
            Return Me.strUserName
        End Get
        Set(ByVal value As String)
            Me.strUserName = value
        End Set
    End Property
    <DataMember()>
    Public Property _Date() As DateTime
        Get
            Return Me.xDate
        End Get
        Set(ByVal value As DateTime)
            Me.xDate = value
        End Set
    End Property
    <DataMember()>
    Public Property Action() As Action
        Get
            Return Me.oAction
        End Get
        Set(ByVal value As Action)
            Me.oAction = value
        End Set
    End Property
    <DataMember()>
    Public Property ObjectType() As ObjectType
        Get
            Return Me.oObjectType
        End Get
        Set(ByVal value As ObjectType)
            Me.oObjectType = value
        End Set
    End Property
    <DataMember()>
    Public Property ObjectName() As String
        Get
            Return Me.strObjectName
        End Get
        Set(ByVal value As String)
            Me.strObjectName = value
        End Set
    End Property
    <DataMember()>
    Public Property Parameters() As DataTable
        Get
            Return Me.tbParameters
        End Get
        Set(ByVal value As DataTable)
            Me.tbParameters = value
        End Set
    End Property
    <DataMember()>
    Public Property SessionID() As Integer
        Get
            Return Me.intSessionID
        End Get
        Set(ByVal value As Integer)
            Me.intSessionID = value
        End Set
    End Property
    <DataMember()>
    Public Property ClientLocation() As String
        Get
            Return Me.strClientLocation
        End Get
        Set(ByVal value As String)
            Me.strClientLocation = value
        End Set
    End Property
    <DataMember()>
    Public ReadOnly Property MessageKey() As String
        Get
            Return "Audit.Message." & System.Enum.GetName(ObjectType.GetType, Me.oObjectType) & "." &
                                          System.Enum.GetName(Action.GetType, Me.oAction)
        End Get
    End Property

#End Region

#Region "Methods"

    Public Function SaveAudit() As Boolean
        Dim bolRet As Boolean = False

        If Me.Parameters IsNot Nothing AndAlso Me.Parameters.Rows.Count > 0 Then
            For Each oRow In Me.Parameters.Rows
                oRow("ParamValue") = roTypes.Any2String(oRow("ParamValue"), 30)
            Next
        End If

        Dim strAuditLine As String = roJSONHelper.SerializeNewtonSoft(Me)
        Return Robotics.Azure.RoAzureSupport.AddAuditLineToTable(strAuditLine, Me.xDate)

        Return bolRet
    End Function

    Public Function GetMessage(ByVal strLanguageKey As String) As String

        If Me.strLanguageFileReference = String.Empty OrElse Me.strLanguageFileReference <> strLanguageKey Then
            Me.strLanguageFileReference = strLanguageKey
            Me.oLanguage.SetLanguageReference("LiveAudit", strLanguageKey)
        End If

        Dim strMessage As String = oLanguage.Translate(Me.MessageKey, "Audit")

        Const TOKEN_PATTERN = "\$@[a-zA-Z.]+@"

        ' Reemplazo tokens por valores pasados por parámetro
        Dim myTokenEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf TokenMatch)
        strMessage = Regex.Replace(strMessage, TOKEN_PATTERN, myTokenEvaluator)

        'aStringTokens = aStringUserTokens
        'strData = Regex.Replace(strData, TOKEN_PATTERN, myTokenEvaluator)

        Return strMessage

    End Function

    Private Function TokenMatch(ByVal m As Match) As String
        '
        ' Delegado que trata cada token de sustituyendolo por lo que se ha pasado por parámetro
        '

        Return Me.ResolveParameter("{" & m.Value.Substring(2, m.Value.Length - 3) & "}", "", Me.oLanguage)

        'Dim tokenPos As Integer
        '
        'tokenPos = m.ToString.Substring(2, m.ToString.Length - 3)
        'If Not aStringTokens Is Nothing Then
        '    If tokenPos <= aStringTokens.Capacity And tokenPos > 0 Then
        '        Return aStringTokens(tokenPos - 1).ToString
        '    Else
        '        Return String.Empty
        '    End If
        'Else
        '    Return " "
        'End If
    End Function

    Private Function ResolveParameter(ByVal strName As String, ByVal strParentName As String, ByVal oLanguage As roLanguage) As String

        Dim strRet As String = ""

        If Me.tbParameters IsNot Nothing AndAlso Me.tbParameters.Rows.Count > 0 Then

            Dim strParamName As String
            'Dim strChildParamName As String
            Dim strChilds As String = ""
            Dim oParams As DataRow() = Me.tbParameters.Select("ParamName = '" & strName & "' AND ParamParent = '" & strParentName & "'", "Priority")
            Dim oChildParams As DataRow()
            For Each oParam As DataRow In oParams

                'If strRet <> "" Then strRet &= vbCrLf

                strParamName = oParam("ParamName")
                If Not strParamName.StartsWith("{") Then
                    If strParamName.StartsWith("?") Then
                        strRet &= oLanguage.Translate("Audit.Parameters." & strParamName.Substring(1), "Audit") & ":"
                    Else
                        strRet &= strParamName & ":"
                    End If
                End If
                strRet &= oParam("ParamValue")

                oChildParams = Me.tbParameters.Select("ParamParent = '" & strParamName & "'", "Priority")
                If oChildParams.Length > 0 Then
                    If strRet <> "" Then strRet = vbCrLf & strRet & vbCrLf
                    strChilds = ""
                    Dim oChild As DataRow
                    Dim strValue As String = ""
                    For nChild As Integer = 0 To oChildParams.Length - 1
                        oChild = oChildParams(nChild)
                        ''If nChild > 0 Then strChilds &= " -> "
                        strValue = Me.ResolveParameter(oChild("ParamName"), oChild("ParamParent"), oLanguage)
                        If nChild > 0 Then
                            If Not strValue.StartsWith(vbCrLf) Then
                                strChilds &= " -> "
                            End If
                        End If
                        strChilds &= strValue
                    Next
                    'For Each oChild As DataRow In oChildParams
                    '    strChilds &= " -> "
                    '    strChilds &= Me.ResolveParameter(oChild("ParamName"), oChild("ParamParent"), oLanguage)
                    'Next
                    While strChilds.EndsWith(" -> ")
                        strChilds = strChilds.Substring(0, strChilds.Length - 4)
                    End While
                    'If strChilds.EndsWith(" -> ") Then strChilds = strChilds.Substring(0, strChilds.Length - 4)
                    If strChilds <> "" Then
                        'strRet &= vbCrLf & strChilds
                        strRet &= strChilds
                    End If
                End If

            Next

            If strParentName = "" AndAlso strRet.StartsWith(vbCrLf) Then
                strRet = strRet.Substring(2)
            End If

        End If

        Return strRet

    End Function

    Private Function GetClientAddress(ByVal oRequest As System.Web.HttpRequest) As String

        Dim strRet As String = ""

        ' See if the address has been forwarded through a proxy server:
        strRet = oRequest.ServerVariables("HTTP_X_FORWARDED_FOR")
        ' If not, then get the real remote address:
        If strRet = String.Empty Then
            strRet = oRequest.ServerVariables("REMOTE_ADDR")
        End If

        Return strRet

    End Function

#Region "Helper methods"

    Public Shared Function CreateParametersTable() As DataTable
        Dim tb As New DataTable("MessageParameters")
        tb.Columns.Add(New DataColumn("ParamName", GetType(String)))
        tb.Columns.Add(New DataColumn("ParamValue", GetType(String)))
        tb.Columns.Add(New DataColumn("ParamParent", GetType(String)))
        tb.Columns.Add(New DataColumn("Priority", GetType(Integer)))
        Return tb
    End Function

    Public Shared Sub AddParameter(ByVal tbParameters As DataTable, ByVal strName As String, ByVal strValue As String, ByVal strParent As String, ByVal intPriority As Integer)
        Dim oNew As DataRow = tbParameters.NewRow
        oNew("ParamName") = strName
        oNew("ParamValue") = roTypes.Any2String(strValue, 4) 'cortamos la longitud máxima de la descripción del parámetro a 4kb
        oNew("ParamParent") = strParent
        oNew("Priority") = intPriority
        tbParameters.Rows.Add(oNew)
    End Sub

    Public Shared Sub AddFieldsValues(ByVal tb As DataTable, ByVal oRow As DataRow, Optional ByVal oRowOld As DataRow = Nothing)

        Dim intPriority As Integer = 1

        AddParameter(tb, "{FieldsValues}", "", "", intPriority) : intPriority += 1

        Dim bolIsUpdate As Boolean = (oRowOld IsNot Nothing)
        Dim strValue As String
        Dim strOldValue As String
        Dim strFieldName As String

        For Each oColumn As DataColumn In oRow.Table.Columns

            strFieldName = "?" & oRow.Table.TableName & "." & oColumn.ColumnName

            If Not bolIsUpdate Then
                If Not IsDBNull(oRow(oColumn.ColumnName)) Then
                    AddParameter(tb, strFieldName, oRow(oColumn.ColumnName), "{FieldsValues}", intPriority) : intPriority += 1
                End If
            Else
                If Not IsDBNull(oRow(oColumn.ColumnName)) Then
                    strValue = roTypes.Any2String(oRow(oColumn.ColumnName))
                Else
                    strValue = ""
                End If
                If Not IsDBNull(oRowOld(oColumn.ColumnName)) Then
                    strOldValue = roTypes.Any2String(oRowOld(oColumn.ColumnName))
                Else
                    strOldValue = ""
                End If
                If strValue <> strOldValue Then

                    If strOldValue.Length > 100 Then strOldValue = $"{strOldValue.Substring(0, 95)}..."
                    If strValue.Length > 100 Then strValue = $"{strValue.Substring(0, 95)}..."

                    AddParameter(tb, strFieldName, "", "{FieldsValues}", intPriority) : intPriority += 1
                    AddParameter(tb, "{OldValue}", strOldValue, strFieldName, intPriority) : intPriority += 1
                    AddParameter(tb, "{NewValue}", strValue, strFieldName, intPriority) : intPriority += 1
                End If
            End If

        Next

    End Sub

    Public Shared Function CloneRow(ByVal oRow As DataRow) As DataRow
        Dim oNewRow As DataRow = oRow.Table.NewRow
        For n As Integer = 0 To oNewRow.Table.Columns.Count - 1
            oNewRow(n) = oRow(n)
        Next
        Return oNewRow
    End Function

    Public Shared Function GetAudit(ByVal strLanguageKey As String,
                                        ByVal _BeginDate As DateTime, ByVal _EndDate As DateTime,
                                        Optional ByVal _UserName As String = "",
                                        Optional ByVal _ActionID As Action = Action.aNone,
                                        Optional ByVal _ObjectType As ObjectType = ObjectType.tNone,
                                        Optional ByVal _ClientLocation As String = "",
                                        Optional ByVal _PageNumber As Integer = -1,
                                        Optional ByVal _PageRows As Integer = -1,
                                        Optional ByVal _OrderField As String = "",
                                        Optional ByVal _OrderAsc As Boolean = True,
                                        Optional ByRef _PagesCount As Integer = -1) As DataTable

        Return GetAuditFromAzure(strLanguageKey, _BeginDate, _EndDate)

    End Function

    Private Shared Function GetAuditFromAzure(strLanguageKey As String, _BeginDate As Date, _EndDate As Date) As DataTable

        Dim tbRet As New DataTable
        tbRet.Columns.Add("ActionID", GetType(Integer))
        tbRet.Columns.Add("ActionDesc", GetType(String))
        tbRet.Columns.Add("Date", GetType(Date))
        tbRet.Columns.Add("PassportName", GetType(String))
        tbRet.Columns.Add("ElementID", GetType(Integer))
        tbRet.Columns.Add("ElementDesc", GetType(String))
        tbRet.Columns.Add("ElementName", GetType(String))
        tbRet.Columns.Add("MessageParameters", GetType(String))
        tbRet.Columns.Add("Message", GetType(String))
        tbRet.Columns.Add("ClientLocation", GetType(String))
        tbRet.Columns.Add("ID", GetType(Integer))

        tbRet.AcceptChanges()

        Try
            Dim lstAuditActions As Generic.List(Of roAudit) = roAudit.GetAzureAudit(_BeginDate, _EndDate)
            Dim oLang As New roLanguage
            oLang.SetLanguageReference("LiveAudit", strLanguageKey)

            Dim oAudit As roAudit = Nothing
            Dim curIndex As Integer = 0
            For Each cAudit As roAudit In lstAuditActions
                Dim oRow As DataRow = tbRet.NewRow()

                oRow("ID") = curIndex

                oRow("ActionID") = cAudit.Action
                oRow("ActionDesc") = oLang.Translate("Audit.Action." & System.Enum.GetName(GetType(Action), cAudit.Action), "Audit")
                oRow("Date") = cAudit._Date
                oRow("PassportName") = cAudit.UserName
                oRow("ElementID") = cAudit.ObjectType
                oRow("ElementDesc") = oLang.Translate("Audit.ObjectType." & System.Enum.GetName(GetType(ObjectType), cAudit.ObjectType), "Audit")
                oRow("ElementName") = cAudit.ObjectName

                oRow("MessageParameters") = roConversions.XmlSerialize(cAudit.Parameters, GetType(DataTable)).Replace("'", "''")
                oRow("Message") = cAudit.GetMessage(strLanguageKey).Replace("<", "&lt;")
                oRow("ClientLocation") = cAudit.ClientLocation
                tbRet.Rows.Add(oRow)
                curIndex = curIndex + 1
            Next
        Catch ex As Exception
            tbRet = Nothing
        End Try

        Return tbRet
    End Function

    Public Shared Function GetAzureAudit(_BeginDate As Date, _EndDate As Date) As Generic.List(Of roAudit)

        Dim lstAuditActions As New Generic.List(Of roAudit)

        Try
            Dim auditLines As String() = Azure.RoAzureSupport.GetAzureAuditBetweenDates(_BeginDate, _EndDate)

            For Each line In auditLines
                If line <> String.Empty Then lstAuditActions.Add(roJSONHelper.DeserializeNewtonSoft(line, GetType(roAudit)))
            Next
        Catch ex As Exception
            lstAuditActions = New List(Of roAudit)
        End Try

        Return lstAuditActions
    End Function

    Public Shared Function ExportAuditToAzure(dt As DataTable) As Integer
        Dim oRet As Integer = 0

        Try
            Dim strLines As New Generic.List(Of String)
            Dim xDates As New Generic.List(Of DateTime)

            For Each oRow As DataRow In dt.Rows
                Dim tbParameters As DataTable = Nothing
                If Not IsDBNull(oRow("MessageParameters")) AndAlso oRow("MessageParameters") <> "" Then
                    tbParameters = roConversions.XmlDeserialize(oRow("MessageParameters"), GetType(DataTable))
                End If

                Dim oAudit As roAudit = New roAudit(CStr(oRow("ClientLocation")), CStr(oRow("PassportName")), oRow("ActionID"), oRow("ElementID"), oRow("ElementName"), tbParameters, roTypes.Any2Integer(oRow("SessionID")))
                oAudit.xDate = roTypes.Any2DateTime(oRow("Date"))

                strLines.Add(roJSONHelper.SerializeNewtonSoft(oAudit))
                xDates.Add(oAudit.xDate)

                If roTypes.Any2Integer(oRow("ID")) > oRet Then
                    oRet = roTypes.Any2Integer(oRow("ID"))
                End If
            Next

            Dim bInsert As Boolean = False

            bInsert = Robotics.Azure.RoAzureSupport.AddAuditLineToTableBatch(strLines.ToArray, xDates.ToArray)

            If Not bInsert Then oRet = 0
        Catch ex As Exception
            oRet = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAudit::ExportAuditToAzure :", ex)
        End Try

        Return oRet
    End Function

    Public Shared Function GetAuditActions(ByVal strLanguageKey As String) As DataTable

        Dim tbRet As DataTable = Nothing

        Dim oLang As New roLanguage
        oLang.SetLanguageReference("LiveAudit", strLanguageKey)

        tbRet = New DataTable("AuditActions")
        tbRet.Columns.Add(New DataColumn("ActionID", GetType(Integer)))
        tbRet.Columns.Add(New DataColumn("ActionDesc", GetType(String)))

        Dim oRow As DataRow
        For Each oAction As Action In System.Enum.GetValues(GetType(Action))
            oRow = tbRet.NewRow
            oRow("ActionID") = oAction
            oRow("ActionDesc") = oLang.Translate("Audit.Action." & System.Enum.GetName(GetType(Action), oAction), "Audit")
            tbRet.Rows.Add(oRow)
        Next

        Return tbRet

    End Function

    Public Shared Function GetAuditObjectTypes(ByVal strLanguageKey As String) As DataTable

        Dim tbRet As DataTable = Nothing

        Dim oLang As New roLanguage
        oLang.SetLanguageReference("LiveAudit", strLanguageKey)

        tbRet = New DataTable("AuditObjectTypes")
        tbRet.Columns.Add(New DataColumn("ElementID", GetType(Integer)))
        tbRet.Columns.Add(New DataColumn("ElementDesc", GetType(String)))

        Dim oRow As DataRow
        For Each oObjectType As ObjectType In System.Enum.GetValues(GetType(ObjectType))
            oRow = tbRet.NewRow
            oRow("ElementID") = oObjectType
            oRow("ElementDesc") = oLang.Translate("Audit.ObjectType." & System.Enum.GetName(GetType(ObjectType), oObjectType), "Audit")
            tbRet.Rows.Add(oRow)
        Next

        Return tbRet

    End Function

#End Region

#End Region

End Class