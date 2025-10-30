Public Class roJSON

    Public Enum JSONType
        None_JSON
        Hidden_JSON
        Text_JSON
        Number_JSON
        Date_JSON
        OptionGroup_JSON
        OptionCheck_JSON
        ComboBox_JSON
        CheckBox_JSON
        Radio_JSON
        DatePicker_JSON
        Time_JSON
    End Enum

    Private m_JSONFields As New ArrayList()

    Public Function addField(ByVal oField As String, ByVal oValue As String, ByVal oControlsID() As String, ByVal oType As JSONType, Optional ByVal oList As Generic.List(Of JSONComboItem) = Nothing, Optional ByVal DisableControl As Boolean = False) As Boolean
        Try
            Dim jsF As New jsonField
            jsF.Field = oField
            jsF.Type = oType
            jsF.Value = oValue
            jsF.Lists = oList
            jsF.ControlsID = oControlsID
            jsF.Disable = DisableControl

            m_JSONFields.Add(jsF)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function CreateJSON() As String
        Try

            Dim strJSON As String = ""

            For Each jsF As jsonField In m_JSONFields
                Dim strControls As String = ""
                strJSON &= "{ 'field':'" & jsF.Field.Replace("'", "''") & "',"

                If jsF.Value Is Nothing Then
                    strJSON &= "'value':'',"
                Else
                    If jsF.Type = JSONType.Number_JSON Then
                        strJSON &= "'value':'" & jsF.Value.Replace(",", ".") & "',"
                    Else
                        strJSON &= "'value':'" & jsF.Value.Replace("'", "''") & "',"
                    End If
                End If

                If jsF.ControlsID IsNot Nothing Then
                    For Each strControl As String In jsF.ControlsID
                        strControls &= strControl & ","
                    Next
                    strControls = strControls.Substring(0, Len(strControls) - 1)
                End If
                strJSON &= "'control':'" & strControls.Replace("'", "''") & "',"
                Select Case jsF.Type
                    Case JSONType.None_JSON
                        strJSON &= "'type':''"
                    Case JSONType.Hidden_JSON
                        strJSON &= "'type':'X_HIDDEN'"
                    Case JSONType.Text_JSON
                        strJSON &= "'type':'X_TEXT'"
                    Case JSONType.Number_JSON
                        strJSON &= "'type':'X_NUMBER'"
                    Case JSONType.Date_JSON
                        strJSON &= "'type':'X_DATE'"
                    Case JSONType.OptionGroup_JSON
                        strJSON &= "'type':'X_OPTIONGROUP'"
                    Case JSONType.OptionCheck_JSON
                        strJSON &= "'type':'X_OPTIONCHECK'"
                    Case JSONType.ComboBox_JSON
                        strJSON &= "'type':'X_COMBOBOX'"
                        Dim comboList As String = ""
                        If jsF.Lists IsNot Nothing Then
                            For Each comboS As JSONComboItem In jsF.Lists
                                comboList &= comboS.toJSON & "|*|"
                            Next
                            comboList = comboList.Substring(0, Len(comboList) - 3)
                        End If
                        strJSON &= ",'list':'" & comboList & "'"
                    Case JSONType.CheckBox_JSON
                        strJSON &= "'type':'X_CHECKBOX'"
                    Case JSONType.Radio_JSON
                        strJSON &= "'type':'X_RADIO'"
                    Case JSONType.DatePicker_JSON
                        strJSON &= "'type':'X_DATEPICKER'"
                    Case JSONType.Time_JSON
                        strJSON &= "'type':'X_TIME'"
                End Select
                strJSON &= ",'disable':'" & jsF.Disable.ToString.ToLower & "'"
                strJSON &= "} ,"
            Next
            strJSON = strJSON.Substring(0, Len(strJSON) - 1)
            Return strJSON
        Catch ex As Exception
            Return "ERROR: " & ex.Message.ToString & " " & ex.StackTrace.ToString
        End Try
    End Function

    Private Class jsonField
        Private m_Field As String = ""
        Private m_Type As JSONType = JSONType.Text_JSON
        Private m_Value As String = ""
        Private m_ControlsID() As String = Nothing
        Private m_List As Generic.List(Of JSONComboItem) = Nothing
        Private m_Disable As Boolean = False

        Public Property Field() As String
            Get
                Return m_Field
            End Get
            Set(ByVal value As String)
                m_Field = value
            End Set
        End Property

        Public Property Type() As JSONType
            Get
                Return m_Type
            End Get
            Set(ByVal value As JSONType)
                m_Type = value
            End Set
        End Property

        Public Property [Value]() As String
            Get
                Return m_Value
            End Get
            Set(ByVal value As String)
                m_Value = value
            End Set
        End Property

        Public Property ControlsID() As String()
            Get
                Return m_ControlsID
            End Get
            Set(ByVal value As String())
                m_ControlsID = value
            End Set
        End Property

        Public Property Lists() As Generic.List(Of JSONComboItem)
            Get
                Return m_List
            End Get
            Set(ByVal value As Generic.List(Of JSONComboItem))
                m_List = value
            End Set
        End Property

        Public Property Disable() As Boolean
            Get
                Return m_Disable
            End Get
            Set(ByVal value As Boolean)
                m_Disable = value
            End Set
        End Property
    End Class

    Public Class JSONComboItem
        Private m_Text As String
        Private m_Value As String
        Private m_JSFunction As String

        Public Sub New()
            m_Text = ""
            m_Value = ""
            m_JSFunction = ""
        End Sub

        Public Sub New(ByVal xText As String, ByVal xValue As String, ByVal xJSFunction As String)
            m_Text = xText
            m_Value = xValue
            m_JSFunction = xJSFunction
        End Sub

        Public Property Text() As String
            Get
                Return m_Text
            End Get
            Set(ByVal value As String)
                m_Text = value
            End Set
        End Property
        Public Property [Value]() As String
            Get
                Return m_Value
            End Get
            Set(ByVal value As String)
                m_Value = value
            End Set
        End Property
        Public Property JSFunction() As String
            Get
                Return m_JSFunction
            End Get
            Set(ByVal value As String)
                m_JSFunction = value
            End Set
        End Property

        Public Function toJSON() As String
            Dim strJSON As String = ""

            If m_Value = "" Then
                m_Value = m_Text
            End If
            Return m_Text.Replace("'", "\'") & "~*~" & m_Value.Replace("'", "\'") & "~*~" & m_JSFunction.Replace("'", "\'")
        End Function

    End Class

    Public Class JSONError

        Public Enum ErMessageType
            InlineMsg
            PopupMsg
        End Enum

        Private m_Error As Boolean
        Private m_TypeMsg As ErMessageType
        Private m_ErrorText As String
        Private m_ObjID As String

        Public Sub New()
            m_Error = False
            m_ErrorText = ""
            m_TypeMsg = ErMessageType.PopupMsg
            m_ObjID = ""
        End Sub

        Public Sub New(ByVal xError As Boolean, ByVal xErrorText As String, Optional ByVal xObjID As String = "", Optional ByVal xTypeMsg As ErMessageType = ErMessageType.PopupMsg)
            m_Error = xError
            m_ErrorText = xErrorText
            m_TypeMsg = xTypeMsg
            m_ObjID = xObjID
        End Sub

        Public Property [Error]() As Boolean
            Get
                Return m_Error
            End Get
            Set(ByVal value As Boolean)
                m_Error = value
            End Set
        End Property
        Public Property ErrorText() As String
            Get
                Return m_ErrorText
            End Get
            Set(ByVal value As String)
                m_ErrorText = value
            End Set
        End Property
        Public Property ObjID() As String
            Get
                Return m_ObjID
            End Get
            Set(ByVal value As String)
                m_ObjID = value
            End Set
        End Property
        Public Property TypeMsg() As ErMessageType
            Get
                Return m_TypeMsg
            End Get
            Set(ByVal value As ErMessageType)
                m_TypeMsg = value
            End Set
        End Property

        Public Function toJSON() As String
            Dim strJSON As String = ""
            Dim typMsg As String = "1"
            typMsg = IIf(m_TypeMsg = ErMessageType.InlineMsg, "0", "1")

            Return "{ 'error': '" & m_Error.ToString.ToLower & "', 'tab': '', 'tabContainer': '', 'id': '" & m_ObjID & "', 'msg': '" & m_ErrorText.Replace("'", "´").Replace(vbCrLf, "<br>") & "', 'typemsg':'" & typMsg & "' }"
        End Function

    End Class

End Class