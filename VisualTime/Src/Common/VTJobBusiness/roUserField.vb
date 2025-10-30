Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace UserField

    Public Enum FieldTypes
        tText = 0
        tNumeric = 1
        tDate = 2
        tDecimal = 3
        tTime = 4
        tList = 5
        tDatePeriod = 6
        tTimePeriod = 7
    End Enum

    Public Enum AccessLevels
        aLow
        aMedium
        aHigh
        'aPublic
        'aRestricted
        'aConfidential
        'aPrivate
    End Enum

    Public Enum Types
        EmployeeField
        GroupField
    End Enum

    Public Enum AccessValidation
        None
        Required
        Warning
    End Enum

    Public Enum CompareType
        Equal
        Minor
        MinorEqual
        Major
        MajorEqual
        Distinct
        Contains
        NotContains
        StartWith
        EndWidth
    End Enum

    Public Enum CompareValueType
        DirectValue
        CurrentDate
        ' ...
    End Enum

    Public Class roUserFieldCondition

#Region "Declarations - Constructor"

        Private strUserFieldName As String
        Private oUserFieldDataType As FieldTypes
        Private oUserFieldType As Types
        Private oCompare As CompareType
        Private oValueType As CompareValueType
        Private strValue As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal oCondition As roCollection)

            If oCondition IsNot Nothing Then

                Me.strUserFieldName = Any2String(oCondition.Item("UserFieldName"))
                Me.oUserFieldDataType = Any2Integer(oCondition.Item("UserFieldDataType"))
                Me.oUserFieldType = Any2Integer(oCondition.Item("UserFieldType"))
                Me.oCompare = Any2Integer(oCondition.Item("Compare"))
                Me.oValueType = Any2Integer(oCondition.Item("ValueType"))
                Me.strValue = Any2String(oCondition.Item("Value"))

            End If

        End Sub

#End Region

#Region "Properties"

        Public Property UserFieldName() As String
            Get
                Return Me.strUserFieldName
            End Get
            Set(ByVal value As String)
                Me.strUserFieldName = value
            End Set
        End Property

        Public Property Compare() As CompareType
            Get
                Return Me.oCompare
            End Get
            Set(ByVal value As CompareType)
                Me.oCompare = value
            End Set
        End Property

        Public Property ValueType() As CompareValueType
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As CompareValueType)
                Me.oValueType = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return Me.strValue
            End Get
            Set(ByVal value As String)
                Me.strValue = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oCondition As New roCollection

            oCondition.Add("UserFieldName", Me.strUserFieldName)
            oCondition.Add("UserFieldDataType", Me.oUserFieldDataType)
            oCondition.Add("UserFieldType", Me.oUserFieldType)
            oCondition.Add("Compare", Me.oCompare)
            oCondition.Add("ValueType", Me.oValueType)
            oCondition.Add("Value", Me.strValue)

            Return oCondition.XML

        End Function

        Public Function GetFilter() As String

            Dim strRet As String = ""
            Dim _FieldName As String = Me.strUserFieldName.Replace("'", "''") '  "[USR_" & Me.oUserField.FieldName & "]"
            Dim _Value As String = Me.strValue

            Select Case Me.oUserFieldDataType
                Case FieldTypes.tText, FieldTypes.tList
                    _Value = Me.strValue
                    Select Case Me.oCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) = '" & _Value & "'"
                        Case CompareType.Minor
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) < '" & _Value & "'"
                        Case CompareType.MinorEqual
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) <= '" & _Value & "'"
                        Case CompareType.Major
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) > '" & _Value & "'"
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) >= '" & _Value & "'"
                        Case CompareType.Distinct
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) <> '" & _Value & "'"
                        Case CompareType.Contains
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '%" & _Value & "%'"
                        Case CompareType.NotContains
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) NOT LIKE '%" & _Value & "%'"
                        Case CompareType.StartWith
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '" & _Value & "%'"
                        Case CompareType.EndWidth
                            strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '%" & _Value & "'"
                    End Select

                Case FieldTypes.tNumeric
                    _Value = Me.strValue
                    If IsNumeric(_Value) Then
                        _Value = CLng(_Value)
                        Select Case Me.oCompare
                            Case CompareType.Equal
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) = " & _Value
                            Case CompareType.Minor
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) < " & _Value
                            Case CompareType.MinorEqual
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) <= " & _Value
                            Case CompareType.Major
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) > " & _Value
                            Case CompareType.MajorEqual
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) >= " & _Value
                            Case CompareType.Distinct
                                strRet = "CONVERT(int, CONVERT(varchar, [Value])) <> " & _Value
                        End Select
                    End If

                Case FieldTypes.tDecimal
                    _Value = Me.strValue
                    If IsNumeric(_Value) Then
                        _Value = CStr(CDbl(_Value)).Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Select Case Me.oCompare
                            Case CompareType.Equal
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) = " & _Value
                            Case CompareType.Minor
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) < " & _Value
                            Case CompareType.MinorEqual
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) <= " & _Value
                            Case CompareType.Major
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) > " & _Value
                            Case CompareType.MajorEqual
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) >= " & _Value
                            Case CompareType.Distinct
                                strRet = "CONVERT(Numeric(16,6), CONVERT(varchar, [Value])) <> " & _Value
                        End Select
                    End If

                Case FieldTypes.tDate
                    _Value = "CONVERT(smalldatetime, '" & Me.strValue & "', 120)"
                    Select Case Me.oCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) = " & _Value
                        Case CompareType.Minor
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) < " & _Value
                        Case CompareType.MinorEqual
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) <= " & _Value
                        Case CompareType.Major
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) > " & _Value
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) >= " & _Value
                        Case CompareType.Distinct
                            strRet = "CONVERT(smalldatetime, CONVERT(varchar, [Value]), 120) <> " & _Value
                    End Select

                Case FieldTypes.tTime
                    _Value = "CONVERT(float, '" & CStr(roConversions.ConvertTimeToHours(Me.strValue)).Replace(roConversions.GetDecimalDigitFormat(), ".") & "')"
                    Select Case Me.oCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(float, ISNULL([Value],'')) = " & _Value
                        Case CompareType.Minor
                            strRet = "CONVERT(float, ISNULL([Value],'')) < " & _Value
                        Case CompareType.MinorEqual
                            strRet = "CONVERT(float, ISNULL([Value],'')) <= " & _Value
                        Case CompareType.Major
                            strRet = "CONVERT(float, ISNULL([Value],'')) > " & _Value
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(float, ISNULL([Value],'')) >= " & _Value
                        Case CompareType.Distinct
                            strRet = "CONVERT(float, ISNULL([Value],'')) <> " & _Value
                    End Select

                    ''_Value = "CONVERT(datetime, '1900/01/01 " & Me.strValue & "', 120)"
                    ''Select Case Me.oCompare
                    ''    Case CompareType.Equal
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) = " & _Value
                    ''    Case CompareType.Minor
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) < " & _Value
                    ''    Case CompareType.MinorEqual
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) <= " & _Value
                    ''    Case CompareType.Major
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) > " & _Value
                    ''    Case CompareType.MajorEqual
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) >= " & _Value
                    ''    Case CompareType.Distinct
                    ''        strRet = "CONVERT(datetime, CONVERT(varchar, [Value]), 120) <> " & _Value
                    ''End Select

                Case FieldTypes.tDatePeriod

                    Dim _ValueBegin As String = Me.strValue.Split("*")(0)
                    If _ValueBegin = "" Then _ValueBegin = "1900/01/01"
                    Dim _ValueEnd As String = ""
                    If Me.strValue.Split("*").Length > 1 Then _ValueEnd = Me.strValue.Split("*")(1)
                    If _ValueEnd = "" Then _ValueEnd = "2079/01/01"
                    _Value = Me.strValue
                    If _Value = "" Then _Value = "1900/01/01"

                    Select Case Me.oCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(varchar, [Value]) = '" & Me.strValue & "'"

                        Case CompareType.Distinct
                            strRet = "CONVERT(varchar, [Value]) <> '" & Me.strValue & "'"
                        Case CompareType.Contains
                            If Me.strValue.Contains("*") Then
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _ValueBegin & "', 120) AND " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) >= CONVERT(smalldatetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _Value & "', 120) AND " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) >= CONVERT(smalldatetime, '" & _Value & "', 120)"
                            End If
                        Case CompareType.NotContains
                            If Me.strValue.Contains("*") Then
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _ValueBegin & "', 120) OR " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) < CONVERT(smalldatetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _Value & "', 120) OR " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) < CONVERT(smalldatetime, '" & _Value & "', 120)"
                            End If
                    End Select

                Case FieldTypes.tTimePeriod

                    Dim _ValueBegin As String = Me.strValue.Split("*")(0)
                    If _ValueBegin = "" Then _ValueBegin = "1900/01/01 00:00:00"
                    Dim _ValueEnd As String = ""
                    If Me.strValue.Split("*").Length > 1 Then _ValueEnd = Me.strValue.Split("*")(1)
                    If _ValueEnd = "" Then _ValueEnd = "2079/01/01 00:00:00"
                    _Value = Me.strValue
                    If _Value = "" Then _Value = "1900/01/01 00:00:00"

                    Select Case Me.oCompare
                        Case CompareType.Equal
                            strRet = _FieldName & " = '" & Me.strValue & "'"
                        Case CompareType.Distinct
                            strRet = _FieldName & " <> '" & Me.strValue & "'"
                        Case CompareType.Contains
                            If Me.strValue.Contains("*") Then
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) <= CONVERT(datetime, '" & _ValueBegin & "', 120) AND " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) >= CONVERT(datetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _Value & "', 120) AND " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) >= CONVERT(datetime, '" & _Value & "', 120)"
                            End If
                        Case CompareType.NotContains
                            If Me.strValue.Contains("*") Then
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _ValueBegin & "', 120) OR " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) < CONVERT(datetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _Value & "', 120) OR " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(varchar, [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(varchar, [Value]),'*'))+1, LEN(ISNULL(CONVERT(varchar, [Value]),'*'))) END, 120) < CONVERT(datetime, '" & _Value & "', 120)"
                            End If
                    End Select

            End Select

            If strRet <> "" Then
                strRet = "Employees.ID IN (@SELECT# IDEmployee " &
                              "FROM [dbo].[GetAllEmployeeUserFieldValue]('" & _FieldName & "', '" & Format(Now.Date, "dd/MM/yyyy") & "') " &
                              "WHERE " & strRet & ")"
            End If
            ''strFilter &= "Employees.ID IN (@SELECT# IDEmployee " & _
            ''                              "FROM [dbo].[GetAllEmployeeUserFieldValue]('" & strField & "', '" & Format(Now.Date, "dd/MM/yyyy") & "' " & _
            ''                              "WHERE "

            Return strRet

        End Function

        Public Function CompareString() As String
            Dim strRet As String = ""
            Select Case Me.oCompare
                Case CompareType.Equal : strRet = "="
                Case CompareType.Minor : strRet = "<"
                Case CompareType.MinorEqual : strRet = "<="
                Case CompareType.Major : strRet = ">"
                Case CompareType.MajorEqual : strRet = ">="
                Case CompareType.Distinct : strRet = "<>"
                Case CompareType.Contains : strRet = "LIKE"
                Case CompareType.NotContains : strRet = "NOT LIKE"
                Case CompareType.StartWith : strRet = "LIKE"
                Case CompareType.EndWidth : strRet = "LIKE"
            End Select
            Return strRet
        End Function

#Region "Helper methods"

        Public Shared Function GetXml(ByVal UserFieldConditions As Generic.List(Of roUserFieldCondition)) As String

            Dim oConditions As New roCollection

            oConditions.Add("TotalConditions", UserFieldConditions.Count)

            Dim n As Integer = 1
            For Each oCondition As roUserFieldCondition In UserFieldConditions
                oConditions.Add("Condition" & n.ToString, New roCollection(oCondition.GetXml))
                n += 1
            Next

            Return oConditions.XML

        End Function

        Public Shared Function LoadFromXml(ByVal strXml As String) As Generic.List(Of roUserFieldCondition)

            Dim oRet As New Generic.List(Of roUserFieldCondition)

            If strXml <> "" Then

                Dim oConditions As New roCollection(strXml)

                Dim n As Integer = 1
                Dim oConditionNode As roCollection = oConditions.Node("Condition" & n.ToString)
                While oConditionNode IsNot Nothing
                    oRet.Add(New roUserFieldCondition(oConditionNode))
                    n += 1
                    oConditionNode = oConditions.Node("Condition" & n.ToString)
                End While

            End If

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace