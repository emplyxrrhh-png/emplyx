Imports System.Runtime.Serialization

<DataContract()>
Public Class JSONFieldItem

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
        'State (Disableds)
        StateText_JSON
        StateNumber_JSON
        StateDate_JSON
        StateOptionGroup_JSON
        StateOptionCheck_JSON
        StateComboBox_JSON
        StateCheckBox_JSON
        StateRadio_JSON
        StateDatePicker_JSON
        StateTime_JSON
    End Enum

    Private m_Field As String = ""
    Private m_Type As JSONType = JSONType.Text_JSON
    Private m_Value As String = ""
    Private m_ControlsID() As String = Nothing
    Private m_List As Generic.List(Of JSONComboFieldItem) = Nothing
    Private m_Disable As Boolean = False

    Public Sub New()
        m_Field = ""
        m_Type = JSONType.Text_JSON
        m_Value = ""
        m_ControlsID = Nothing
        m_List = Nothing
        m_Disable = False
    End Sub

    Public Sub New(ByVal xField As String, ByVal xValue As String, ByVal xControlsID() As String, ByVal xType As JSONType, ByVal xList As Generic.List(Of JSONComboFieldItem), ByVal xDisabled As Boolean)
        m_Field = xField
        m_Type = xType
        m_Value = xValue
        m_ControlsID = xControlsID
        m_List = xList
        m_Disable = xDisabled
    End Sub

    <DataMember(Name:="field")>
    Public Property Field() As String
        Get
            Return m_Field
        End Get
        Set(ByVal value As String)
            m_Field = value
        End Set
    End Property

    <DataMember(Name:="typeid")>
    Public Property Type() As JSONType
        Get
            Return m_Type
        End Get
        Set(ByVal value As JSONType)
            m_Type = value
        End Set
    End Property

    <DataMember(Name:="type")>
    Public Property TypeStr() As String
        Get
            Select Case m_Type
                Case JSONType.None_JSON
                    Return ""
                Case JSONType.Hidden_JSON
                    Return "X_HIDDEN"
                Case JSONType.Text_JSON
                    Return "X_TEXT"
                Case JSONType.Number_JSON
                    Return "X_NUMBER"
                Case JSONType.Date_JSON
                    Return "X_DATE"
                Case JSONType.OptionGroup_JSON
                    Return "X_OPTIONGROUP"
                Case JSONType.OptionCheck_JSON
                    Return "X_OPTIONCHECK"
                Case JSONType.ComboBox_JSON
                    Return "X_COMBOBOX"
                Case JSONType.CheckBox_JSON
                    Return "X_CHECKBOX"
                Case JSONType.Radio_JSON
                    Return "X_RADIO"
                Case JSONType.DatePicker_JSON
                    Return "X_DATEPICKER"
                Case JSONType.Time_JSON
                    Return "X_TIME"
                    'Estats -----------------------------------------------------
                Case JSONType.StateText_JSON
                    Return "X_TEXT_STATE"
                Case JSONType.StateNumber_JSON
                    Return "X_NUMBER_STATE"
                Case JSONType.StateDate_JSON
                    Return "X_DATE_STATE"
                Case JSONType.StateOptionGroup_JSON
                    Return "X_OPTIONGROUP_STATE"
                Case JSONType.StateOptionCheck_JSON
                    Return "X_OPTIONCHECK_STATE"
                Case JSONType.StateComboBox_JSON
                    Return "X_COMBOBOX_STATE"
                Case JSONType.StateCheckBox_JSON
                    Return "X_CHECKBOX_STATE"
                Case JSONType.StateRadio_JSON
                    Return "X_RADIO_STATE"
                Case JSONType.StateDatePicker_JSON
                    Return "X_DATEPICKER_STATE"
                Case JSONType.StateTime_JSON
                    Return "X_TIME_STATE"
                Case Else
                    Return ""
            End Select
        End Get
        Set(ByVal value As String)
        End Set
    End Property

    <DataMember(Name:="value")>
    Public Property [Value]() As String
        Get
            Return m_Value
        End Get
        Set(ByVal value As String)
            m_Value = value
        End Set
    End Property

    <DataMember(Name:="control")>
    Public Property ControlsID() As String()
        Get
            Return m_ControlsID
        End Get
        Set(ByVal value As String())
            m_ControlsID = value
        End Set
    End Property

    <DataMember(Name:="list")>
    Public Property Lists() As Generic.List(Of JSONComboFieldItem)
        Get
            Return m_List
        End Get
        Set(ByVal value As Generic.List(Of JSONComboFieldItem))
            m_List = value
        End Set
    End Property

    <DataMember(Name:="disable")>
    Public Property Disable() As Boolean
        Get
            Return m_Disable
        End Get
        Set(ByVal value As Boolean)
            m_Disable = value
        End Set
    End Property
End Class