Imports System.Runtime.Serialization

<DataContract()>
Public Class JSONGenericControl

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
    Private m_onChange As String = ""

    Public Sub New()
        m_Field = ""
        m_Type = JSONType.Text_JSON
        m_Value = ""
        m_ControlsID = Nothing
        m_List = Nothing
        m_Disable = False
    End Sub

    Public Sub New(ByVal xField As String, ByVal xValue As String, ByVal xControlsID() As String, ByVal xType As JSONType, ByVal xList As Generic.List(Of JSONComboFieldItem), ByVal xDisabled As Boolean, ByVal xOnChange As String)
        m_Field = xField
        m_Type = xType
        m_Value = xValue
        m_ControlsID = xControlsID
        m_List = xList
        m_Disable = xDisabled
        m_onChange = xOnChange
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

    <DataMember(Name:="onchange")>
    Public Property OnChange() As Boolean
        Get
            Return m_onChange
        End Get
        Set(ByVal value As Boolean)
            m_onChange = value
        End Set
    End Property
End Class

<DataContract()>
Public Class JSONCheckBoxSelector
    Private strTitle As String = ""
    Private oRows As New Generic.List(Of JSONCheckBoxSelectorRow)
    Private strOnClick As String = ""

    <DataMember(Name:="title")>
    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property

    <DataMember(Name:="rows")>
    Public Property Rows() As Generic.List(Of JSONCheckBoxSelectorRow)
        Get
            Return oRows
        End Get
        Set(ByVal value As Generic.List(Of JSONCheckBoxSelectorRow))
            oRows = value
        End Set
    End Property

    <DataMember(Name:="globalclick")>
    Public Property OnClick() As String
        Get
            Return strOnClick
        End Get
        Set(ByVal value As String)
            strOnClick = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONCheckBoxSelectorRow
    Private strDescription As String = ""
    Private strValue As String = ""
    Private bolChecked As Boolean = False
    Private strOnClick As String = ""

    <DataMember(Name:="description")>
    Public Property Description() As String
        Get
            Return strDescription
        End Get
        Set(ByVal value As String)
            strDescription = value
        End Set
    End Property

    <DataMember(Name:="value")>
    Public Property [Value]() As String
        Get
            Return strValue
        End Get
        Set(ByVal value As String)
            strValue = value
        End Set
    End Property

    <DataMember(Name:="checked")>
    Public Property Checked() As Boolean
        Get
            Return bolChecked
        End Get
        Set(ByVal value As Boolean)
            bolChecked = value
        End Set
    End Property

    <DataMember(Name:="onclick")>
    Public Property OnClick() As Boolean
        Get
            Return bolChecked
        End Get
        Set(ByVal value As Boolean)
            bolChecked = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONTextBox

    Public Enum JSONTextBoxType
        Text_JSON
        Number_JSON
        Date_JSON
        DatePicker_JSON
    End Enum

    Private m_Field As String = ""
    Private m_Type As JSONTextBoxType = JSONTextBoxType.Text_JSON
    Private m_Value As String = ""
    Private m_ControlsID() As String = Nothing
    Private m_List As Generic.List(Of JSONComboFieldItem) = Nothing
    Private m_Disable As Boolean = False
    Private m_onChange As String = ""

    Public Sub New()
        m_Field = ""
        m_Type = JSONTextBoxType.Text_JSON
        m_Value = ""
        m_ControlsID = Nothing
        m_List = Nothing
        m_Disable = False
    End Sub

    Public Sub New(ByVal xField As String, ByVal xValue As String, ByVal xControlsID() As String, ByVal xType As JSONTextBoxType, ByVal xList As Generic.List(Of JSONComboFieldItem), ByVal xDisabled As Boolean, ByVal xOnChange As String)
        m_Field = xField
        m_Type = xType
        m_Value = xValue
        m_ControlsID = xControlsID
        m_List = xList
        m_Disable = xDisabled
        m_onChange = xOnChange
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
    Public Property Type() As JSONTextBoxType
        Get
            Return m_Type
        End Get
        Set(ByVal value As JSONTextBoxType)
            m_Type = value
        End Set
    End Property

    <DataMember(Name:="type")>
    Public Property TypeStr() As String
        Get
            Return m_Type.ToString
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

    <DataMember(Name:="disable")>
    Public Property Disable() As Boolean
        Get
            Return m_Disable
        End Get
        Set(ByVal value As Boolean)
            m_Disable = value
        End Set
    End Property

    <DataMember(Name:="onchange")>
    Public Property OnChange() As Boolean
        Get
            Return m_onChange
        End Get
        Set(ByVal value As Boolean)
            m_onChange = value
        End Set
    End Property
End Class

<DataContract()>
Public Class JSONGrid

End Class

<DataContract()>
Public Class JSONRoundButton

    Public Enum JSONRBColors
        Black
        Gold
        Green
        NavyBlue
        Orange
        Red
        Silver
        SteelBlue
        Yellow
    End Enum

    Public Enum JSONRBStates
        Accept
        Accept2
        ArrowRec
        Cancel
        Cancel2
        Clock
        Denied
        Enter
        EnterIncidence
        Exclamation
        [Exit]
        ExitIncidence
        Home
        [Next]
        Previous
        Question
        RoundBottom
        RoundLeft
        RoundRight
        RoundTop
        Search
        World
    End Enum

    Private strTargetID As String = ""
    Private strText As String = "Button"
    Private mColor As JSONRBColors = JSONRBColors.SteelBlue
    Private mState As JSONRBStates = JSONRBStates.Accept
    Private strOnClick As String = ""
    Private bolDisable As Boolean = False

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Crea un valor para RoundButton
    ''' </summary>
    ''' <param name="_TargetID">ID Cliente que recibira las modificaciones</param>
    ''' <param name="_Text">Texto a mostrar</param>
    ''' <param name="_Color">Color</param>
    ''' <param name="_icoState">Icono de estado</param>
    ''' <param name="_onClick">Evento javascript cliente que se ejecutará al hacer lcick</param>
    ''' <param name="_Disable">Deshabilitado</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _TargetID As String,
                   ByVal _Text As String,
                   Optional ByVal _Color As JSONRBColors = JSONRBColors.SteelBlue,
                   Optional ByVal _icoState As JSONRBStates = JSONRBStates.Accept,
                   Optional ByVal _onClick As String = "",
                   Optional ByVal _Disable As Boolean = False)

        strTargetID = _TargetID
        strText = _Text
        mColor = _Color
        mState = _icoState
        strOnClick = _onClick
        bolDisable = _Disable

    End Sub

    <DataMember(Name:="targetid")>
    Public Property TargetID() As String
        Get
            Return strTargetID
        End Get
        Set(ByVal value As String)
            strTargetID = value
        End Set
    End Property

    <DataMember(Name:="text")>
    Public Property Text() As String
        Get
            Return strText
        End Get
        Set(ByVal value As String)
            strText = value
        End Set
    End Property

    <DataMember(Name:="colorid")>
    Public Property Color() As JSONRBColors
        Get
            Return mColor
        End Get
        Set(ByVal value As JSONRBColors)
            mColor = value
        End Set
    End Property

    <DataMember(Name:="color")>
    Public ReadOnly Property ColorStr() As String
        Get
            Return mColor.ToString
        End Get
    End Property

    <DataMember(Name:="icostateid")>
    Public Property IcoState() As JSONRBStates
        Get
            Return mState
        End Get
        Set(ByVal value As JSONRBStates)
            mState = value
        End Set
    End Property

    <DataMember(Name:="icostate")>
    Public ReadOnly Property IcoStateStr() As String
        Get
            Return mState.ToString
        End Get
    End Property

    <DataMember(Name:="onclick")>
    Public Property OnClick() As String
        Get
            Return strOnClick
        End Get
        Set(ByVal value As String)
            strOnClick = value
        End Set
    End Property

    <DataMember(Name:="disable")>
    Public Property Disable() As Boolean
        Get
            Return bolDisable
        End Get
        Set(ByVal value As Boolean)
            bolDisable = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONRoundToolbar
    Private mRoundButton1 As JSONRoundButton = Nothing
    Private mRoundButton2 As JSONRoundButton = Nothing
    Private mRoundButton3 As JSONRoundButton = Nothing
    Private mRoundButton4 As JSONRoundButton = Nothing

    Private mOverrideButton1 As Boolean = False
    Private mOverrideButton2 As Boolean = False
    Private mOverrideButton3 As Boolean = False
    Private mOverrideButton4 As Boolean = False

    Public Sub New()
    End Sub

    <DataMember(Name:="overridebutton1")>
    Public Property OverrideButton1() As Boolean
        Get
            Return mOverrideButton1
        End Get
        Set(ByVal value As Boolean)
            mOverrideButton1 = value
        End Set
    End Property

    <DataMember(Name:="overridebutton2")>
    Public Property OverrideButton2() As Boolean
        Get
            Return mOverrideButton2
        End Get
        Set(ByVal value As Boolean)
            mOverrideButton2 = value
        End Set
    End Property

    <DataMember(Name:="overridebutton3")>
    Public Property OverrideButton3() As Boolean
        Get
            Return mOverrideButton3
        End Get
        Set(ByVal value As Boolean)
            mOverrideButton3 = value
        End Set
    End Property

    <DataMember(Name:="overridebutton4")>
    Public Property OverrideButton4() As Boolean
        Get
            Return mOverrideButton4
        End Get
        Set(ByVal value As Boolean)
            mOverrideButton4 = value
        End Set
    End Property

    <DataMember(Name:="roundbutton1")>
    Public Property RoundButton1() As JSONRoundButton
        Get
            Return mRoundButton1
        End Get
        Set(ByVal value As JSONRoundButton)
            mRoundButton1 = value
        End Set
    End Property

    <DataMember(Name:="roundbutton2")>
    Public Property RoundButton2() As JSONRoundButton
        Get
            Return mRoundButton2
        End Get
        Set(ByVal value As JSONRoundButton)
            mRoundButton2 = value
        End Set
    End Property

    <DataMember(Name:="roundbutton3")>
    Public Property RoundButton3() As JSONRoundButton
        Get
            Return mRoundButton3
        End Get
        Set(ByVal value As JSONRoundButton)
            mRoundButton3 = value
        End Set
    End Property

    <DataMember(Name:="roundbutton4")>
    Public Property RoundButton4() As JSONRoundButton
        Get
            Return mRoundButton4
        End Get
        Set(ByVal value As JSONRoundButton)
            mRoundButton4 = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONPhotoframe

    Public Enum JSONBallStatus
        Inside
        OutSide
    End Enum

    Private mStatus As JSONBallStatus = JSONBallStatus.OutSide

    ''' <summary>
    ''' Devuelve un objeto Photoframe de LivePortal con el estado correspondiente
    ''' </summary>
    ''' <param name="_Status"></param>
    ''' <remarks></remarks>
    Public Sub New(Optional ByVal _Status As JSONBallStatus = JSONBallStatus.OutSide)
        mStatus = _Status
    End Sub

    Public Property Status() As JSONBallStatus
        Get
            Return mStatus
        End Get
        Set(ByVal value As JSONBallStatus)
            mStatus = value
        End Set
    End Property
End Class

<DataContract()>
Public Class JSONRibbon

    Public Enum JSONRibbonStatus
        Inside
        OutSide
    End Enum

    Private strRibbonUserName As String = ""
    Private strRibbonMsg As String = ""
    Private mRibbonStatus As JSONRibbonStatus = JSONRibbonStatus.OutSide

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Crea un objeto Ribbon para control de estado de LivePortal
    ''' </summary>
    ''' <param name="_UserName">Usuario de LivePortal (Nombre y apellidos)</param>
    ''' <param name="_Msg">Mensaje de estado (Entró..., Salió..., etc.)</param>
    ''' <param name="_Status">Presente, ausente, etc.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _UserName As String, ByVal _Msg As String, ByVal _Status As JSONRibbonStatus)
        strRibbonUserName = _UserName
        strRibbonMsg = _Msg
        mRibbonStatus = _Status
    End Sub

    <DataMember(Name:="ribbonusername")>
    Public Property UserName() As String
        Get
            Return strRibbonUserName
        End Get
        Set(ByVal value As String)
            strRibbonUserName = value
        End Set
    End Property

    <DataMember(Name:="ribbonmessage")>
    Public Property Message() As String
        Get
            Return strRibbonMsg
        End Get
        Set(ByVal value As String)
            strRibbonMsg = value
        End Set
    End Property

    <DataMember(Name:="ribbonstatus")>
    Public Property Status() As JSONRibbonStatus
        Get
            Return mRibbonStatus
        End Get
        Set(ByVal value As JSONRibbonStatus)
            mRibbonStatus = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONEmployeeStatus

    Private strHour As String = "00"
    Private strMinute As String = "00"
    Private dDateServer As Date = System.DateTime.Now
    Private strDateText As String = ""
    Private mPhotoframe As JSONPhotoframe = Nothing
    Private mRibbon As JSONRibbon = Nothing

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Crea un objeto EmployeeStatus para cargar la cabecera de LivePortal
    ''' </summary>
    ''' <param name="_Hour">Hora (00)</param>
    ''' <param name="_Minute">Minutos (00)</param>
    ''' <param name="_DateServer">Fecha del servidor</param>
    ''' <param name="_DateText">Texto de la fecha (Miercoles, 22 de abril, etc.)</param>
    ''' <param name="_Photoframe">Estado de la fotografia</param>
    ''' <param name="_Ribbon">Ribbon con estado, nombre usuario, etc.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _Hour As String, ByVal _Minute As String, ByVal _DateServer As Date, ByVal _DateText As String, Optional ByVal _Photoframe As JSONPhotoframe = Nothing, Optional ByVal _Ribbon As JSONRibbon = Nothing)
        strHour = _Hour
        strMinute = _Minute
        dDateServer = _DateServer
        strDateText = _DateText
        mPhotoframe = _Photoframe
        mRibbon = _Ribbon
    End Sub

    <DataMember(Name:="serverhour")>
    Public Property Hours() As String
        Get
            Return strHour
        End Get
        Set(ByVal value As String)
            strHour = value
        End Set
    End Property

    <DataMember(Name:="serverminute")>
    Public Property Minutes() As String
        Get
            Return strMinute
        End Get
        Set(ByVal value As String)
            strMinute = value
        End Set
    End Property

    <DataMember(Name:="serverdate")>
    Public Property DateServer() As Date
        Get
            Return dDateServer
        End Get
        Set(ByVal value As Date)
            dDateServer = value
        End Set
    End Property

    <DataMember(Name:="serverdatetext")>
    Public Property DateText() As String
        Get
            Return strDateText
        End Get
        Set(ByVal value As String)
            strDateText = value
        End Set
    End Property

    <DataMember(Name:="photoframe")>
    Public Property Photoframe() As JSONPhotoframe
        Get
            Return mPhotoframe
        End Get
        Set(ByVal value As JSONPhotoframe)
            mPhotoframe = value
        End Set
    End Property

    <DataMember(Name:="ribbon")>
    Public Property Ribbon() As JSONRibbon
        Get
            Return mRibbon
        End Get
        Set(ByVal value As JSONRibbon)
            mRibbon = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONMainMenu
    Private strObjPrefix As String
    Private mMainMenuItems As Generic.List(Of JSONMainMenuItem)

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Crea un valor para MainMenu
    ''' </summary>
    ''' <param name="_MainMenuItems">MenuItems</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal objPrefix As String, ByVal _MainMenuItems As Generic.List(Of JSONMainMenuItem))
        mMainMenuItems = _MainMenuItems
    End Sub

    <DataMember(Name:="objprefix")>
    Public Property ObjPrefix() As String
        Get
            Return strObjPrefix
        End Get
        Set(ByVal value As String)
            strObjPrefix = value
        End Set
    End Property

    <DataMember(Name:="menuitems")>
    Public Property MenuItems() As Generic.List(Of JSONMainMenuItem)
        Get
            Return mMainMenuItems
        End Get
        Set(ByVal value As Generic.List(Of JSONMainMenuItem))
            mMainMenuItems = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONMainMenuItem

    Private strPath As String
    Private strText As String
    Private strImgUrl As String
    Private strOnClick As String
    Private strUrlRedirect As String
    Private mSubMenuItems As Generic.List(Of JSONMainMenuItem)
    Private strInfo As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _Path As String, ByVal _Text As String, ByVal _ImgUrl As String, ByVal _OnClick As String, ByVal _UrlRedirect As String,
                    ByVal _SubMenuItems As Generic.List(Of JSONMainMenuItem))
        strPath = _Path
        strText = _Text
        strImgUrl = _ImgUrl
        strOnClick = _OnClick
        strUrlRedirect = _UrlRedirect
        mSubMenuItems = _SubMenuItems
        strInfo = ""
    End Sub

    <DataMember(Name:="path")>
    Public Property Path() As String
        Get
            Return strPath
        End Get
        Set(ByVal value As String)
            strPath = value
        End Set
    End Property

    <DataMember(Name:="text")>
    Public Property Text() As String
        Get
            Return strText
        End Get
        Set(ByVal value As String)
            strText = value
        End Set
    End Property

    <DataMember(Name:="imgurl")>
    Public Property ImgUrl() As String
        Get
            Return strImgUrl
        End Get
        Set(ByVal value As String)
            strImgUrl = value
        End Set
    End Property

    <DataMember(Name:="onclick")>
    Public Property OnClick() As String
        Get
            Return strOnClick
        End Get
        Set(ByVal value As String)
            strOnClick = value
        End Set
    End Property

    <DataMember(Name:="urlredirect")>
    Public Property UrlRedirect() As String
        Get
            Return strUrlRedirect
        End Get
        Set(ByVal value As String)
            strUrlRedirect = value
        End Set
    End Property

    <DataMember(Name:="menuitems")>
    Public Property MenuItems() As Generic.List(Of JSONMainMenuItem)
        Get
            Return mSubMenuItems
        End Get
        Set(ByVal value As Generic.List(Of JSONMainMenuItem))
            mSubMenuItems = value
        End Set
    End Property

    <DataMember(Name:="info")>
    Public Property Info() As String
        Get
            Return strInfo
        End Get
        Set(ByVal value As String)
            strInfo = value
        End Set
    End Property

End Class