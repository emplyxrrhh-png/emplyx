Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Logging
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs

Public Class roJSONHelper

    Shared Function Serialize(ByVal obj As Object) As String
        Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType())
        Dim ms As MemoryStream = New MemoryStream()
        serializer.WriteObject(ms, obj)
        Dim retVal As String = Encoding.UTF8.GetString(ms.ToArray())
        Return retVal
    End Function

    Shared Function Deserialize(ByVal json As String, ByVal oType As System.Type) As Object
        Dim obj As Object = Activator.CreateInstance(Of Object)()
        ''Dim ms As MemoryStream = New MemoryStream(Encoding.Unicode.GetBytes(json))
        Dim ms As MemoryStream = New MemoryStream(Encoding.Unicode.GetBytes(json))
        'Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType())
        Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(oType)
        obj = CObj(serializer.ReadObject(ms))
        ms.Close()
        Return obj
    End Function

    Shared Function Deserialize(Of T)(ByVal json As String) As T
        Return JsonConvert.DeserializeObject(Of T)(json)
    End Function

    Shared Function DeserializeNewtonSoft(ByVal json As String, ByVal oType As System.Type) As Object

        json = json.Replace("1999-12-29", "1899-12-29")
        json = json.Replace("1999-12-30", "1899-12-30")
        json = json.Replace("1999-12-31", "1899-12-31")

        Dim obj As Object = Activator.CreateInstance(Of Object)()
        Dim textReader As TextReader = New StringReader(json)
        Dim serializer As New JsonSerializer

        serializer.NullValueHandling = NullValueHandling.Ignore
        serializer.MissingMemberHandling = MissingMemberHandling.Ignore

        obj = serializer.Deserialize(textReader, oType)

        Return obj
    End Function

    Shared Function SerializeNewtonSoft(ByVal obj As Object) As String

        Dim serializer As New JsonSerializer
        serializer.NullValueHandling = NullValueHandling.Ignore
        serializer.MissingMemberHandling = MissingMemberHandling.Ignore

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)

        Dim jsonWriter As New JsonTextWriter(sw)

        serializer.Serialize(jsonWriter, obj)

        Dim Json As String = sw.ToString()
        Json = Json.Replace("1899-12-29", "1999-12-29")
        Json = Json.Replace("1899-12-30", "1999-12-30")
        Json = Json.Replace("1899-12-31", "1999-12-31")

        Return Json
    End Function

#Region "Genius"

    Public Shared Function ToGeniusJSONDefinitionString(ByVal oTable As DataTable, ByVal oTranlateHash As Generic.List(Of roLayoutDescription), ByVal userFieldTypes As Base.DTOs.UserFieldProperties()) As String
        Dim strJSON As String = "{"

        For Each oProperty As DataColumn In oTable.Columns

            Dim propertyType As String = String.Empty

            '"string","number" ,"month" ,"weekday","date","date string","year/month/day","year/quarter/month/day","time","datetime","id","property"
            Select Case oProperty.DataType
                Case GetType(Integer), GetType(Nullable(Of Integer)), GetType(Short), GetType(Nullable(Of Short)), GetType(Decimal), GetType(Nullable(Of Decimal)), GetType(Byte), GetType(Nullable(Of Byte))
                    propertyType = "number"
                    If oProperty.ColumnName = "DayOfWeek" OrElse oProperty.ColumnName.Contains("HHMM") OrElse oProperty.ColumnName.Contains("HH:MM") OrElse oProperty.ColumnName.Contains("IDEmployee") OrElse oProperty.ColumnName.Contains("IDGroup") OrElse oProperty.ColumnName.Contains("IDContract") OrElse oProperty.ColumnName.Contains("IDAssignment") OrElse oProperty.ColumnName.Contains("IDProductiveUnit") Then
                        propertyType = "string"
                    ElseIf oProperty.ColumnName = "Mes" OrElse oProperty.ColumnName = "Month" Then
                        propertyType = "month"
                    End If
                Case GetType(Date), GetType(Nullable(Of Date))
                    If oProperty.ColumnName.EndsWith("_ToDateString") Then
                        propertyType = "date string"
                    Else
                        If oProperty.ColumnName.EndsWith("_ToTime") Then
                            propertyType = "datetime"
                        Else
                            propertyType = "date"
                        End If
                    End If
                Case Else
                    propertyType = "string"

            End Select

            Select Case oProperty.ColumnName.ToUpper
                Case "INVALIDTYPE", "REQUESTSSTATUS", "REQUESTTYPE", "IDPASSPORT", "PUNCHDIRECTION", "METHOD", "VERSION", "CENTERNAME"
                    propertyType = "string"
            End Select

            If oProperty.ColumnName.ToLower.StartsWith("userfield") Then
                Dim index As Integer = roTypes.Any2Integer(oProperty.ColumnName.ToLower.Replace("userfield", ""))

                If index <= userFieldTypes.Length Then
                    Dim fieldType As Base.DTOs.UserFieldsTypes.FieldTypes = userFieldTypes(index - 1).Type

                    Select Case fieldType
                        Case Base.DTOs.UserFieldsTypes.FieldTypes.tDate
                            If oProperty.ColumnName.EndsWith("_ToDateString") Then
                                propertyType = "date string"
                            Else
                                propertyType = "date"
                            End If
                        Case Base.DTOs.UserFieldsTypes.FieldTypes.tDecimal, Base.DTOs.UserFieldsTypes.FieldTypes.tNumeric
                            propertyType = "number"
                        Case Else
                            propertyType = "string"
                    End Select
                Else
                    propertyType = ""
                End If
            ElseIf oProperty.ColumnName.ToLower.StartsWith("sysro") Then
                Dim columnName As String = oProperty.ColumnName.ToLower.Split("_")(0)

                Select Case columnName
                    Case "sysroBirthdate"
                        propertyType = "date string"
                    Case "sysromobile", "sysrogender", "sysroprofessionalcategory", "sysroquotegroup", "sysroposition"
                        propertyType = "string"
                    Case "sysrototalsalary", "sysrobasesalary", "sysrosalarysupp", "sysroextrasalary", "sysroearningsovertime"
                        propertyType = "number"
                    Case Else
                        propertyType = "string"
                End Select
            ElseIf oProperty.ColumnName.ToLower.StartsWith("costcentertotalcost") Then
                propertyType = "number"
            End If

            'If oTranlateHash IsNot Nothing Then

            Dim oColDefinition As roLayoutDescription = oTranlateHash.Find(Function(x) x.ColumnName = oProperty.ColumnName)

            If oColDefinition IsNot Nothing AndAlso propertyType <> String.Empty Then
                strJSON &= """" & oColDefinition.Id & """: {""type"":""" & propertyType & """,""caption"":""" & oColDefinition.Caption & """},"
            End If
        Next

        strJSON &= "}"
        strJSON = strJSON.Replace(",}", "}")

        Return strJSON
    End Function

    Public Shared Function ToGeniusJSONString(ByVal oRow As DataRow, ByVal oTranlateHash As Generic.List(Of roLayoutDescription), ByVal userFieldTypes() As Base.DTOs.UserFieldProperties, dicKeywords As Generic.Dictionary(Of String, String), dicInvalidDescription As Generic.Dictionary(Of Integer, String),
                                              dicMethodDescription As Generic.Dictionary(Of Integer, String), dicPassportName As Generic.Dictionary(Of Integer, String), dicPunchDirectionDescription As Generic.Dictionary(Of Integer, String),
                                              dicStatusDescription As Generic.Dictionary(Of Integer, String), dicTypeDescription As Generic.Dictionary(Of Integer, String), dicVersionDescription As Generic.Dictionary(Of String, String), dicPassportUserFieldPermission As Generic.Dictionary(Of Integer, Base.DTOs.Permission), ByVal columnDefinition As DataColumnCollection, Optional dicDayOfWeekDescription As Generic.Dictionary(Of Integer, String) = Nothing, Optional dicReliableDescription As Generic.Dictionary(Of Integer, String) = Nothing, Optional dicMonthDescription As Generic.Dictionary(Of Integer, String) = Nothing, Optional dicUnespecifiedZoneDescription As Generic.Dictionary(Of String, String) = Nothing, Optional BIExecutionName As String = Nothing) As String
        Dim strJSON As String
        Try
            strJSON = "{"

            For Each element As roLayoutDescription In oTranlateHash
                If Not IsDBNull(oRow(element.ColumnName)) Then
                    If roTypes.Any2String(oRow(element.ColumnName)) <> String.Empty OrElse element.ToString.ToUpper = "CENTERNAME" Then
                        Dim translateObj As Object = oRow(element.ColumnName)

                        Dim sElementSearch As String = element.ColumnName

                        If sElementSearch.ToUpper.Contains("(HH:MM)_TOHOURS") Then
                            sElementSearch = "(HH:MM)_TOHOURS"
                        End If

                        Try
                            Select Case sElementSearch.ToUpper
                                Case "INVALIDTYPE"
                                    If dicInvalidDescription IsNot Nothing Then translateObj = dicInvalidDescription(roTypes.Any2Integer(translateObj))
                                Case "REQUESTSSTATUS"
                                    If dicStatusDescription IsNot Nothing Then translateObj = dicStatusDescription(roTypes.Any2Integer(translateObj))
                                Case "REQUESTTYPE"
                                    If dicTypeDescription IsNot Nothing Then translateObj = dicTypeDescription(roTypes.Any2Integer(translateObj))
                                Case "IDPASSPORT"
                                    If dicPassportName IsNot Nothing Then translateObj = dicPassportName(roTypes.Any2Integer(translateObj))
                                Case "PUNCHDIRECTION"
                                    If dicPunchDirectionDescription IsNot Nothing Then translateObj = dicPunchDirectionDescription(roTypes.Any2Integer(translateObj))
                                Case "METHOD"
                                    If dicMethodDescription IsNot Nothing Then translateObj = dicMethodDescription(roTypes.Any2Integer(translateObj))
                                Case "VERSION"
                                    If dicVersionDescription IsNot Nothing Then translateObj = dicVersionDescription(roTypes.Any2String(translateObj))
                                Case "CENTERNAME"
                                    If dicKeywords IsNot Nothing AndAlso roTypes.Any2String(translateObj) = String.Empty Then translateObj = dicKeywords("NoCenter")
                                Case "COSTCENTERTOTALCOST"
                                    translateObj = GetCostFromObject(oRow)
                                Case "DAYOFWEEK"
                                    If dicDayOfWeekDescription IsNot Nothing Then translateObj = dicDayOfWeekDescription(roTypes.Any2Integer(translateObj))
                                Case "MES"
                                    If dicMonthDescription IsNot Nothing Then translateObj = dicMonthDescription(roTypes.Any2Integer(translateObj))
                                Case "MONTH"
                                    If dicMonthDescription IsNot Nothing Then translateObj = dicMonthDescription(roTypes.Any2Integer(translateObj))
                                Case "VALUEHHMM_TOHOURS"
                                    translateObj = VTBase.roConversions.ConvertHoursToTime(roTypes.Any2Double(translateObj))
                                Case "POSITIVEVALUEHHMM_TOHOURS"
                                    translateObj = VTBase.roConversions.ConvertHoursToTime(roTypes.Any2Double(translateObj))
                                Case "CAUSEVALUEHHMM"
                                    translateObj = VTBase.roConversions.ConvertHoursToTime(roTypes.Any2Double(translateObj))
                                Case "(HH:MM)_TOHOURS"
                                    translateObj = VTBase.roConversions.ConvertHoursToTime(roTypes.Any2Double(translateObj))
                                Case "ISNOTRELIABLE"
                                    If dicReliableDescription IsNot Nothing Then translateObj = dicReliableDescription(Convert.ToInt32(translateObj))
                                Case "ZONENAME"
                                    If translateObj IsNot Nothing AndAlso translateObj = "Sin especificar" Then translateObj = dicUnespecifiedZoneDescription(roTypes.Any2String(translateObj))
                                Case "REALVALUE"
                                    If translateObj IsNot Nothing Then translateObj = DecodeRoboticsCard(translateObj, 20)
                            End Select
                        Catch ex As Exception
                            translateObj = oRow(element.ColumnName)
                        End Try

                        If columnDefinition(element.ColumnName).DataType = GetType(Integer) OrElse columnDefinition(element.ColumnName).DataType = GetType(Nullable(Of Integer)) OrElse columnDefinition(element.ColumnName).DataType = GetType(Short) OrElse columnDefinition(element.ColumnName).DataType = GetType(Nullable(Of Short)) OrElse columnDefinition(element.ColumnName).DataType = GetType(Decimal) OrElse columnDefinition(element.ColumnName).DataType = GetType(Nullable(Of Decimal)) OrElse columnDefinition(element.ColumnName).DataType = GetType(Byte) OrElse columnDefinition(element.ColumnName).DataType = GetType(Nullable(Of Byte)) Then
                            Dim result As Double
                            If Double.TryParse(translateObj, result) AndAlso (roTypes.Any2String(translateObj).Contains(".") OrElse roTypes.Any2String(translateObj).Contains(",")) Then
                                translateObj = roTypes.Any2Double(roTypes.Any2String(translateObj).Replace(".", roConversions.GetDecimalDigitFormat)).ToString().Replace(".", roConversions.GetDecimalDigitFormat)
                            End If
                        End If
                        Dim oJsonVal = roJSONHelper.SerializeNewtonSoft(translateObj)

                        If element.ColumnName.ToLower.StartsWith("userfield") Then
                            Dim index As Integer = roTypes.Any2Integer(element.ColumnName.ToLower.Replace("userfield", ""))

                            If index <= userFieldTypes.Length Then
                                Dim fieldType As Base.DTOs.UserFieldsTypes.FieldTypes = userFieldTypes(index - 1).Type

                                If dicPassportUserFieldPermission(userFieldTypes(index - 1).RequieredFeature) > Base.DTOs.Permission.None Then
                                    Select Case fieldType
                                        Case Base.DTOs.UserFieldsTypes.FieldTypes.tDate
                                            If roTypes.Any2String(translateObj) = String.Empty Then
                                                oJsonVal = roJSONHelper.SerializeNewtonSoft("")
                                            Else
                                                oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2DateTime(translateObj))
                                            End If
                                        Case Base.DTOs.UserFieldsTypes.FieldTypes.tDecimal, Base.DTOs.UserFieldsTypes.FieldTypes.tNumeric
                                            oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2Double(roTypes.Any2String(translateObj).Replace(".", roConversions.GetDecimalDigitFormat)).ToString().Replace(".", roConversions.GetDecimalDigitFormat))
                                        Case Else
                                            oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2String(translateObj))
                                    End Select
                                Else
                                    oJsonVal = roJSONHelper.SerializeNewtonSoft("")
                                End If
                            Else
                                oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2String(translateObj))
                            End If
                        ElseIf element.ColumnName.ToLower.StartsWith("sysro") Then
                            Dim columnName As String = element.ColumnName.ToLower.Split("_")(0)

                            Select Case columnName
                                Case "sysromobile", "sysrogender", "sysroprofessionalcategory", "sysroquotegroup", "sysroposition"
                                    oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2String(translateObj))
                                Case "sysrototalsalary", "sysrobasesalary", "sysrosalarysupp", "sysroextrasalary", "sysroearningsoverTime"
                                    oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2Double(roTypes.Any2String(translateObj).Replace(".", roConversions.GetDecimalDigitFormat)))
                                Case Else
                                    oJsonVal = roJSONHelper.SerializeNewtonSoft(roTypes.Any2String(translateObj))
                            End Select
                        End If

                        If BIExecutionName Is Nothing Then
                            If Not (oJsonVal.Contains("2079-01-01T00:00:00") OrElse oJsonVal.Contains("1900-01-01T00:00:00")) Then strJSON &= """" & element.Id & """:" & oJsonVal & ","
                        Else
                            If Not (oJsonVal.Contains("2079-01-01T00:00:00") OrElse oJsonVal.Contains("1900-01-01T00:00:00")) Then strJSON &= """" & element.ColumnName & """:" & oJsonVal & ","
                        End If
                        'If Not oJsonVal.Contains("2079-01-01T00:00:00") Then strJSON &= """" & oTranlateHash(element) & """:" & oJsonVal & ","
                    End If
                End If

            Next

            strJSON &= "}"
            strJSON = strJSON.Replace(",}", "}")
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "Genius::Error generating JSON register::", ex)
            strJSON = String.Empty
        End Try

        Return strJSON
    End Function

#End Region

#Region "Analytics V1 with genius"

    Public Shared Function ToGeniusJSONDefinitionString(ByVal oAnalityc As Robotics.Base.DTOs.Analytics_Base, ByVal oTranlateHash As Hashtable) As String
        Dim strJSON As String = "{"

        Dim actualType As Type = oAnalityc.GetType()

        For Each oProperty In actualType.GetProperties

            Dim propertyType As String = String.Empty

            Select Case oProperty.PropertyType()
                Case GetType(Integer), GetType(Nullable(Of Integer)), GetType(Short), GetType(Nullable(Of Short)), GetType(Decimal), GetType(Nullable(Of Decimal)), GetType(Byte), GetType(Nullable(Of Byte))
                    propertyType = "number"
                Case GetType(Date), GetType(Nullable(Of Date))
                    propertyType = "date"
                Case Else
                    propertyType = "string"

            End Select

            If oTranlateHash IsNot Nothing Then
                If oTranlateHash(oProperty.Name) <> String.Empty Then strJSON &= """" & oProperty.Name & """: {""type"":""" & propertyType & """,""caption"":""" & oTranlateHash(oProperty.Name) & """},"
            Else
                strJSON &= """" & oProperty.Name & """: {""type"":""" & propertyType & """,""caption"":""" & oProperty.Name & """},"
            End If

        Next

        strJSON &= "}"
        strJSON = strJSON.Replace(",}", "}")

        Return strJSON
    End Function

    Public Shared Function ToGeniusJSONString(ByVal oAnalityc As Robotics.Base.DTOs.Analytics_Base, ByVal oTranlateHash As Hashtable, ByVal userFieldNames As String()) As String

        Dim serializer As New JsonSerializer
        serializer.NullValueHandling = NullValueHandling.Ignore
        serializer.MissingMemberHandling = MissingMemberHandling.Ignore

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)

        Dim jsonWriter As New JsonTextWriter(sw)

        serializer.Serialize(jsonWriter, oAnalityc)
        Dim strJSON As String = sw.ToString()

        Dim actualType As Type = oAnalityc.GetType()

        For Each oProperty In actualType.GetProperties
            strJSON = strJSON.Replace("""" & oProperty.Name & """:"""",", "")
            strJSON = strJSON.Replace("""" & oProperty.Name & """:""""", "")
            strJSON = strJSON.Replace("""" & oProperty.Name & """:""2079-01-01T00:00:00"",", "")
            strJSON = strJSON.Replace("""" & oProperty.Name & """:""2079-01-01T00:00:00""", "")

            strJSON = strJSON.Replace(",}", "}")
        Next

        If oTranlateHash IsNot Nothing Then
            Dim index As Integer = userFieldNames.Length

            While index <= 10
                strJSON = strJSON.Replace("""UserField" & index.ToString & """:""""", "")
                index += 1
            End While

        End If

        Return strJSON
    End Function

#End Region

    Private Shared Function GetCostFromObject(oAnalyticObj As DataRow) As Decimal

        Dim xValue As Decimal = 0
        Try
            xValue = oAnalyticObj("Value")

            Dim decimalFormat As String = roConversions.GetDecimalDigitFormat

            If Not IsDBNull(oAnalyticObj("DefaultCenter")) AndAlso roTypes.Any2Integer(oAnalyticObj("DefaultCenter")) = True Then
                If roTypes.Any2String(oAnalyticObj("Cost")) <> String.Empty Then
                    xValue = xValue * roTypes.Any2Double(roTypes.Any2String(oAnalyticObj("Cost")).Replace(".", decimalFormat))
                Else
                    xValue = 0
                End If
            Else
                If roTypes.Any2String(oAnalyticObj("PVP")) <> String.Empty Then
                    xValue = xValue * roTypes.Any2Double(roTypes.Any2String(oAnalyticObj("PVP")).Replace(".", decimalFormat))
                Else
                    xValue = 0
                End If

            End If

            If Not IsDBNull(oAnalyticObj("CostFactor")) Then
                xValue = xValue * roTypes.Any2Double(oAnalyticObj("CostFactor"))
            End If
        Catch ex As Exception
            xValue = 0
        End Try

        Return xValue
    End Function

    Private Shared Function DecodeRoboticsCard(ByVal idCard As String, Optional ByVal maxLen As Byte = 16) As Long
        Dim sIDCard As String = ""
        Try
            Dim stmp As String

            If idCard.Trim <> "" Then
                If idCard.Length > maxLen Then
                    stmp = Right(idCard, maxLen)
                Else
                    stmp = idCard
                End If
                While stmp.Length >= 2
                    If Convert.ToString(Integer.Parse(Right(stmp, 2)), 16).Length > 1 Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "Genius::DecodeRoboticsCard::Warning: IDCard " & idCard & " is invalid.")
                        Return 0
                    End If
                    sIDCard = Convert.ToString(Integer.Parse(Right(stmp, 2)), 16) & sIDCard
                    stmp = stmp.Substring(0, stmp.Length - 2)
                End While
                If stmp.Length > 0 Then sIDCard = Convert.ToString(Integer.Parse(stmp), 16) & sIDCard
                sIDCard = Convert.ToInt64(sIDCard, 16).ToString
            End If

            Return Robotics.VTBase.roTypes.Any2Long(sIDCard)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "Genius::DecodeRoboticsCard::Error:(IDCard:" & idCard & "):", ex)
            Return 0
        End Try
    End Function

End Class