'Imports Robotics.WebServices.CommonService
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions
Imports Robotics.Base.DTOs

<Serializable()>
<DataContract>
Public Class roLanguageLocal
    Private Const DICTIONARY_EXTENSION = ".DIC"
    Private Const LANGUAGE_EXTENSION = ".LNG"
    Private mDictionary As String
    Private oSupport As New roSupport
    Private strLanguageFileReference As String
    Private strLanguageKey As String
    Private oNotTranslatedControlsTypes As New ArrayList
    Private oControlsWithoutTooltip As New ArrayList
    Private aStringTokens As New ArrayList
    Private aStringSystemTokens As New ArrayList ' Tokens propios del sistema
    Private aStringUserTokens As New ArrayList ' Tokens que seran aportados por el programa y que se reemplazaran en las posiscion ${1} a ${9}
    Private oScopesLoadeds As New ArrayList
    Private oTextList As LanguageFile
    Private oTextDictList As LanguageFile
    Private languageCulture As CultureInfo

    Private sCustomFileContent As Boolean
    Private bByteContent As Byte()

    Public Sub New()
        sCustomFileContent = False
        bByteContent = {}
    End Sub

    Public Sub ClearUserTokens()
        aStringUserTokens.Clear()
    End Sub

    Public Sub AddUserToken(ByVal Token As String)
        aStringUserTokens.Add(Token)
    End Sub

    Public Sub SetLanguageReference(ByVal LanguageFileReference As String, ByVal LanguageKey As String)
        If LanguageFileReference <> String.Empty Then strLanguageFileReference = LanguageFileReference

        If LanguageKey <> String.Empty Then
            strLanguageKey = LanguageKey
            Try
                Select Case LanguageKey
                    Case "ESP"
                        languageCulture = New CultureInfo("es-ES")
                    Case "CAT"
                        languageCulture = New CultureInfo("ca-es")
                    Case "ENG"
                        languageCulture = New CultureInfo("en-US")
                    Case "GAL"
                        languageCulture = New CultureInfo("gl-ES")
                    Case "EKR"
                        languageCulture = New CultureInfo("eu-ES")
                    Case "ITA"
                        languageCulture = New CultureInfo("it-IT")
                    Case "FRA"
                        languageCulture = New CultureInfo("fr-FR")
                    Case "CHN"
                        languageCulture = New CultureInfo("zh-Hant")
                    Case "POR"
                        languageCulture = New CultureInfo("pt-PT")
                    Case "SLK"
                        languageCulture = New CultureInfo("sk-SK")
                    Case Else
                        languageCulture = New CultureInfo("es-ES")
                End Select
            Catch ex As Exception
                languageCulture = New CultureInfo("es-ES")
            End Try
        Else
            strLanguageKey = "ESP"
            languageCulture = New CultureInfo("es-ES")
        End If
    End Sub

    Public Sub LoadFromByteArray(ByVal customFileBytes As Byte())
        sCustomFileContent = True

        If customFileBytes IsNot Nothing AndAlso customFileBytes.Length > 0 Then
            bByteContent = customFileBytes
        End If

    End Sub

    Private Sub InitVariables()
        ' Inicializo el array de clases de controles que no debo inspeccionar
        oNotTranslatedControlsTypes.Clear()
        oNotTranslatedControlsTypes.Add("System.Windos.forms.Textbox")
        oNotTranslatedControlsTypes.Add("DevComponents.DotNetBar.Controls.TextBoxX")
        oNotTranslatedControlsTypes.Add("Robotics.BaseControls.roListDataControl")
        oNotTranslatedControlsTypes.Add("Robotics.BaseControls.roNumericControl")
        oNotTranslatedControlsTypes.Add("Robotics.BaseControls.roDateControl")
        oNotTranslatedControlsTypes.Add("EmployeeSelector.EmployeeSelector")

        ' Inicializo el array de controles que no tienen tooltip
        oControlsWithoutTooltip.Clear()
        oControlsWithoutTooltip.Add("DevComponents.DotNetBar.PanelEx")
        oControlsWithoutTooltip.Add("System.Windows.Forms.Label")
        oControlsWithoutTooltip.Add("DevComponents.DotNetBar.Controls.CheckBoxX")
        oControlsWithoutTooltip.Add("DevComponents.DotNetBar.Controls.GroupPanel")
        oControlsWithoutTooltip.Add("DevComponents.DotNetBar.TabControlPanel")
        oControlsWithoutTooltip.Add("DevComponents.DotNetBar.RibbonControl")
        ' Inicializo el array de tokens
        aStringSystemTokens.Clear()
        aStringSystemTokens.Add("${Employees}")
        aStringSystemTokens.Add("${Card}")

    End Sub

    Public Sub ClearLanguageBuffer()
        oScopesLoadeds = Nothing
        oTextList = Nothing
        oTextDictList = Nothing
    End Sub

    Private Function IsScopeLoaded(ByVal Scope As String) As Boolean
        Dim strCurrentScope As String

        For Each strCurrentScope In oScopesLoadeds
            If strCurrentScope = Scope Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function TranslateRawText(ByVal TextKey As String, ByVal Scope As String) As String
        Return Translate(TextKey, Scope, True)
    End Function

    Public Function Translate(ByVal TextKey As String, ByVal Scope As String, Optional ByVal bRawText As Boolean = False) As String
        Dim strTranslation As String

        InitVariables()

        If Not IsScopeLoaded(Scope) Then
            oTextList = Me.GetLanguage(strLanguageFileReference, strLanguageKey, Scope)
            oScopesLoadeds.Add(Scope)
        End If

        ' Traduzco el TEXTO
        strTranslation = FindLanguageText(TextKey & ".roText", "NotFound")
        If strTranslation = "NotFound" Then
            strTranslation = FindLanguageText(TextKey & ".Text", "NotFound")
        End If

        If strTranslation <> "NotFound" Then
            If strTranslation = "(NoEntry)" Then
                strTranslation = "UNDEFINED LANGUAGE:" & TextKey & ".roText"
            End If
            If Not bRawText Then strTranslation = StringParse(strTranslation)
            Return strTranslation
        End If

        Return strTranslation
    End Function

    Public Function TranslateWithDefault(ByVal TextKey As String, ByVal Scope As String, ByVal strDefault As String, Optional ByVal bRawText As Boolean = False) As String
        Dim oRet As String = String.Empty
        oRet = Translate(TextKey, Scope, bRawText)
        If oRet = "NotFound" OrElse oRet.StartsWith("UNDEFINED LANGUAGE:") Then
            oRet = strDefault
        End If
        Return oRet
    End Function

    Private Sub AnalizeControls(ByVal ParentControl As Object, ByVal FormName As String)
        ' Función recursiva que revisa los controles y sus hijos
        Dim oControl As Object = Nothing
        Dim oSubControl As Object

        Select Case ParentControl.GetType.ToString
            Case "DevComponents.DotNetBar.BubbleBarTab" ' Este control requiere un trato especial
                For Each oControl In ParentControl.buttons

                    If Not oNotTranslatedControlsTypes.Contains(oControl.GetType.ToString) Then
                        TranslateControl(oControl, FormName)
                        'AnalizeControls(oControl, FormName) ' No es necesario lanzar otro analize pq estos botones no pueden contener otro control
                    End If

                Next
            Case "DevComponents.DotNetBar.TabItem" ' Este control requiere un trato especial
                TranslateControl(ParentControl, FormName)
            Case "DevComponents.DotNetBar.RibbonControl" ' Este control requiere un trato especial
                ' La ribbon contiene dos listas diferentes de controles
                For Each oControl In ParentControl.Controls ' Analizo la primera lista
                    AnalizeControls(oControl, FormName)
                Next

                For Each oControl In oControl.items ' Analizo la segunda lista
                    TranslateControl(oControl, FormName)
                Next
            Case Else
                For Each oControl In ParentControl.Controls

                    If Not oNotTranslatedControlsTypes.Contains(oControl.GetType.ToString) Then
                        TranslateControl(oControl, FormName)
                        Select Case oControl.GetType.ToString
                            Case "DevComponents.DotNetBar.BubbleBar" ' Este control requiere un trato especial
                                For Each oSubControl In oControl.tabs
                                    AnalizeControls(oSubControl, FormName)
                                Next
                            Case "DevComponents.DotNetBar.TabControl" ' Este control requiere un trato especial
                                For Each oSubControl In oControl.tabs
                                    AnalizeControls(oSubControl, FormName)
                                Next

                                For Each oSubControl In oControl.controls
                                    AnalizeControls(oSubControl, FormName)
                                Next
                            Case Else
                                AnalizeControls(oControl, FormName)
                        End Select
                    End If

                Next

        End Select
    End Sub

    Private Function FindLanguageText(ByVal Key As String, ByVal DefaultValue As String, Optional ByVal FromDic As Boolean = False) As String
        Dim strText As String
        Dim intEqualPos As Integer
        Dim oAuxTextList As LanguageFile

        If FromDic Then
            oAuxTextList = oTextDictList
        Else
            oAuxTextList = oTextList
        End If

        For Each strText In oAuxTextList.TextList
            If strText.ToLower.StartsWith(Key.ToLower) Then
                intEqualPos = InStr(strText, "=", CompareMethod.Text)

                If intEqualPos > 0 Then
                    Return strText.Substring(intEqualPos)
                Else
                    Return DefaultValue
                End If
            End If
        Next

        Return DefaultValue
    End Function

    Private Sub TranslateControl(ByVal oControl As Object, ByVal FormName As String)

        Dim strTranslation As String

        If oControl.Name = "" Then Exit Sub

        If oControl.GetType.ToString <> "DevComponents.DotNetBar.BubbleButton" Then
            ' Traduzco el TEXT
            Try ' Este try es para controlar que no todos los controles tienen la propiedad text
                strTranslation = oControl.Text ' Esta linea solo esta para que si el control no posee la propiedad salte una excepcion y no continue con el resto del codigo
                'strTranslation = oSupport.INIRead(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".Text", "NotFound")
                strTranslation = FindLanguageText(FormName & "." & oControl.Name & ".Text", "NotFound")
                If strTranslation <> "NotFound" Then
                    If strTranslation = "(NoEntry)" Then
                        strTranslation = "UNDEFINED LANGUAGE:" & FormName & "." & oControl.Name & ".Text"
                    End If
                    strTranslation = StringParse(strTranslation)
                    oControl.Text = strTranslation
                Else
                    Try ' Este try es para controlar que no todos los controles tienen la propiedad text
                        'oSupport.INIWrite(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".Text", oControl.Text)
                        If oControl.text = "" Then
                            Me.SaveLanguage(strLanguageFileReference, strLanguageKey, FormName & "." & oControl.Name & ".Text", "(NoEntry)")
                        Else
                            Me.SaveLanguage(strLanguageFileReference, strLanguageKey, FormName & "." & oControl.Name & ".Text", oControl.text)
                        End If
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As Exception
            End Try

            ' Traduzco el tooltip
            If Not oControlsWithoutTooltip.Contains(oControl.GetType.ToString) Then
                Try ' Este try es para controlar que no todos los controles tienen la propiedad text
                    strTranslation = oControl.Tooltip ' Esta linea solo esta para que si el control no posee la propiedad salte una excepcion y no continue con el resto del codigo
                    'strTranslation = oSupport.INIRead(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".Tooltip", "NotFound")
                    strTranslation = FindLanguageText(FormName & "." & oControl.Name & ".Tooltip", "NotFound")
                    If strTranslation <> "NotFound" Then
                        If strTranslation = "(NoEntry)" Then
                            strTranslation = "UNDEFINED LANGUAGE:" & FormName & "." & oControl.Name & ".Tooltip"
                        End If
                        strTranslation = StringParse(strTranslation)
                        oControl.Tooltip = strTranslation
                    Else
                        Try ' Este try es para controlar que no todos los controles tienen la propiedad tooltip
                            'oSupport.INIWrite(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".Tooltip", oControl.tooltip)
                            'Me.SaveLanguage(strLanguageFileReference, strLanguageKey, FormName & "." & oControl.Name & ".Tooltip", "(NoEntry)")
                        Catch ex As Exception
                        End Try

                    End If
                Catch ex As Exception
                End Try
            End If
        Else

            ' Traduzco el tooltiptext (Utilizado solamente por los bubblebarbuttons
            'strTranslation = oSupport.INIRead(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".TooltipText", "NotFound")
            strTranslation = FindLanguageText(FormName & "." & oControl.Name & ".TooltipText", "NotFound")
            If strTranslation <> "NotFound" Then
                If strTranslation = "(NoEntry)" Then
                    strTranslation = "UNDEFINED LANGUAGE:" & FormName & "." & oControl.Name & ".TooltipText"
                End If
                strTranslation = StringParse(strTranslation)
                Try ' Este try es para controlar que no todos los controles tienen la propiedad tooltip
                    oControl.Tooltip = strTranslation
                Catch ex As Exception
                End Try
            Else
                Try ' Este try es para controlar que no todos los controles tienen la propiedad tooltip
                    'oSupport.INIWrite(strLanguagePath & "\" & strFileName, "Language", FormName & "." & oControl.Name & ".Tooltip", oControl.TooltipText)
                    'Me.SaveLanguage(strLanguageFileReference, strLanguageKey, FormName & "." & oControl.Name & ".Tooltip", "(NoEntry)")
                Catch ex As Exception
                End Try

            End If
        End If

    End Sub

    'Private Sub CreateDir(ByVal LanguagePath As String)
    '    ' Funcion que crea el directorio si no existe
    '    If My.Computer.FileSystem.GetDirectoryInfo(LanguagePath).Attributes = -1 Then
    '        MkDir(LanguagePath)
    '    End If
    'End Sub
    Private Function StringParse(ByVal strData As String, Optional ByVal ReplaceDefaultValue As String = "roNoEntry") As String
        '
        ' Modifica el string cambiando los tokens por su valor
        '  Además de los tokens predefinidos ($M,$W, etc), se puede pasar un
        '   diccionario, de forma que reemplaza strings "${key}" por el valor
        '   en el diccionario.
        '
        Dim sAux As String
        Dim sPost As String
        Dim fromToken As String
        Dim toToken As String

        Dim replaceToken As String
        Dim dictionaryToken As String

        Dim bolFirstCharUpper As Boolean

        'Patrones de búsqueda
        Const DICTIONARY_PATTERN = "\${[a-zA-Z0-9.]+}"
        Const TOKEN_PATTERN = "\${[0-9]+}"

        On Error Resume Next

        If strData Is Nothing Or strData = "" Then Return ""

        ' Miro si hay un token al inicio de la cadena para, una vez traducido, poner la primera letra en mayúscula
        If Mid(strData, 1, 2) = "${" Then
            bolFirstCharUpper = True
        Else
            bolFirstCharUpper = False
        End If

        ' Reemplaza tokens fijos
        strData = strData.Replace("$(APPPATH)", Environment.CurrentDirectory)
        strData = strData.Replace("$(WINSYSPATH)", Environment.SystemDirectory)
        strData = strData.Replace("$(WINPATH)", roSupport.GetWindowsDir)

        Dim oSettings As New roSettings
        strData = strData.Replace("$(VTSYSTEMPATH)", oSettings.GetVTSetting(eKeys.System))
        strData = strData.Replace("$(VTCONFIGPATH)", oSettings.GetVTSetting(eKeys.Config))
        strData = strData.Replace("$(VTREPORTSPATH)", oSettings.GetVTSetting(eKeys.Reports))
        strData = strData.Replace("$(VTDATALINKPATH)", oSettings.GetVTSetting(eKeys.DataLink))
        strData = strData.Replace("$(VTREADINGSPATH)", oSettings.GetVTSetting(eKeys.Reports))

        strData = strData.Replace("$(APPNAME)", roSupport.GetExeName)

        strData = strData.Replace("$CRLF", vbCrLf)
        strData = strData.Replace("$CR", vbCr)
        strData = strData.Replace("$LF", vbLf)
        strData = strData.Replace("$NL", Chr(10))
        strData = strData.Replace("$H", Now.Hour)
        strData = strData.Replace("$N", Now.Minute)
        strData = strData.Replace("$S", Now.Second)
        strData = strData.Replace("$YY", Now.Year)
        strData = strData.Replace("$Y", Format$(Year(Now), "yy"))
        strData = strData.Replace("$MMM", MonthName(Now.Month, False))
        strData = strData.Replace("$MM", MonthName(Now.Month, True))
        strData = strData.Replace("$M", Now.Month)
        strData = strData.Replace("$D", Now.Day)
        strData = strData.Replace("$WWW", WeekdayName(Weekday(Now), False))
        strData = strData.Replace("$WW", WeekdayName(Weekday(Now), True))
        strData = strData.Replace("$W", Weekday(Now))

        ' Reemplaza expresiones de usuario por valores de usuarios
        'While InStr(strData, "$REPLACE{") > 0
        '    fromToken = Mid$(strData, InStr(strData, "$REPLACE{") + 9, 9999)
        '    toToken = Mid$(fromToken, InStr(fromToken, "#") + 1, 9999)
        '    fromToken = Mid$(fromToken, 1, InStr(fromToken, "#") - 1)
        '    sAux = Mid$(toToken, 2, 9999)
        '    toToken = Mid$(toToken, 1, InStr(toToken, "#") - 1)
        '    sAux = Mid$(sAux, InStr(sAux, "#") + 1, 9999)
        '    sPost = sAux
        '    sAux = Mid$(sAux, 1, InStr(sAux, "}") - 1)
        '    sPost = Mid$(sPost, InStr(sPost, "}") + 1, 9999)
        '    sAux.Replace(fromToken, toToken)
        '    strData = Mid$(strData, 1, InStr(strData, "$REPLACE{") - 1) & sAux & sPost
        'End While

        ' Reemplaza tokens por valores en el diccionario
        Dim myDictionayEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf TranslateMatch)
        strData = Regex.Replace(strData, DICTIONARY_PATTERN, myDictionayEvaluator)

        ' Reemplazo tokens por valores pasados por parámetro
        aStringTokens = aStringUserTokens
        Dim myTokenEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf TokenMatch)
        strData = Regex.Replace(strData, TOKEN_PATTERN, myTokenEvaluator)

        aStringTokens = aStringUserTokens
        strData = Regex.Replace(strData, TOKEN_PATTERN, myTokenEvaluator)

        ' TODO: Si la primera palabara de la frase viene del diccionario, se deberí poner la primera letra mayúscula
        If bolFirstCharUpper Then
            strData = UCase(Mid(strData, 1, 1)) & Mid(strData, 2)
        End If

        Return strData

    End Function

    Private Function TokenMatch(ByVal m As Match) As String
        '
        ' Delegado que trata cada token de sustituyendolo por lo que se ha pasado por parámetro
        '

        Dim tokenPos As Integer

        tokenPos = m.ToString.Substring(2, m.ToString.Length - 3)
        If Not aStringTokens Is Nothing Then
            If tokenPos <= aStringTokens.Capacity And tokenPos > 0 Then
                Return aStringTokens(tokenPos - 1).ToString
            Else
                Return String.Empty
            End If
        Else
            Return " "
        End If
    End Function

    Private Function TranslateMatch(ByVal m As Match) As String
        '
        ' Delegado que trata cada token de diccionario encontrado en una cadena
        '
        If oTextDictList Is Nothing Then
            oTextDictList = Me.GetLanguage("Dictionary", strLanguageKey & DICTIONARY_EXTENSION, "")
        End If

        Return FindLanguageText(m.ToString.Substring(2, m.ToString.Length - 3), m.ToString, True)
    End Function

    Public Function Keyword(ByVal key As String) As String
        '
        ' Delegado que trata cada token de diccionario encontrado en una cadena
        '
        Dim strRet As String = ""
        If oTextDictList Is Nothing Then
            oTextDictList = Me.GetLanguage("Dictionary", strLanguageKey & DICTIONARY_EXTENSION, "")
        End If

        Dim strKeyDic As String = key
        If key.StartsWith("${") And key.EndsWith("}") Then
            strKeyDic = key.Substring(2, key.Length - 3)
        End If

        strRet = FindLanguageText(strKeyDic, key, True)

        Return strRet

    End Function

    Public Function GetLanguage(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Scope As String) As LanguageFile
        Dim oTextsArray As New LanguageFile
        Dim strLanguagePath As String
        Dim strFile As String
        Dim strCustomFile As String = String.Empty
        Dim strLine As String
        Dim bIsCustomFile As Boolean = False

        If Not sCustomFileContent Then
            ' Recupero la ruta de los ficheros de idioma
            Dim oSetting As New Robotics.VTBase.roSettings
            strLanguagePath = oSetting.GetVTSetting(eKeys.Language)

            bIsCustomFile = LanguageKey.Contains(".CUST")
            If LanguageKey = "" Then LanguageKey = "ESP"
            If LanguageKey = ".LNG" Then LanguageKey = "ESP.LNG"
            If LanguageKey = ".DIC" Then LanguageKey = "ESP.DIC"
            ' Contruyo el nombre del fichero

            If InStr(LanguageKey, ".LNG") > 0 OrElse InStr(LanguageKey, ".DIC") > 0 Then
                strFile = LanguageReference & "." & LanguageKey ' El languageKey contiene la extension
                If Not bIsCustomFile Then strCustomFile = LanguageReference & "." & LanguageKey.Split(".")(0) & ".CUST." & LanguageKey.Split(".")(1)
            Else
                strFile = LanguageReference & "." & LanguageKey & ".LNG"
                If Not bIsCustomFile Then strCustomFile = LanguageReference & "." & LanguageKey & ".CUST.LNG"
            End If

            If strLanguagePath <> "" Then
                ' Abro el fichero de idioma y lo recorro guardando en el array aquellas cadenas que pertenezcan al Ámbito/Scope

                ' Primero, si existe, el personalizado
                If Not bIsCustomFile AndAlso System.IO.File.Exists(strLanguagePath & "\" & strCustomFile) Then
                    Dim oCustFile As New FileStream(strLanguagePath & "\" & strCustomFile, FileMode.OpenOrCreate, FileAccess.Read)
                    Dim oCurrentCustomEncoding As System.Text.Encoding = roLanguageFileEncoding.DetectTextFileEncoding(strLanguagePath & "\" & strCustomFile)
                    ' El fichero es uno estandar. Cargo si existe el personalizado
                    Dim oSCustomReader As StreamReader = New StreamReader(oCustFile, oCurrentCustomEncoding)
                    Do While Not oSCustomReader.EndOfStream
                        strLine = oSCustomReader.ReadLine()

                        If Scope <> "" Then
                            If strLine.StartsWith(Scope) Then
                                oTextsArray.TextList.Add(strLine)
                            End If
                        Else
                            oTextsArray.TextList.Add(strLine)
                        End If
                    Loop
                    oSCustomReader.Close()
                    oCustFile.Close()
                End If

                Dim oFile As New FileStream(strLanguagePath & "\" & strFile, FileMode.OpenOrCreate, FileAccess.Read)
                Dim oCurrentEncoding As System.Text.Encoding = roLanguageFileEncoding.DetectTextFileEncoding(strLanguagePath & "\" & strFile)
                Dim oSReader As StreamReader = New StreamReader(oFile, oCurrentEncoding)
                Do While Not oSReader.EndOfStream
                    strLine = oSReader.ReadLine()

                    If Scope <> "" Then
                        If strLine.StartsWith(Scope) Then
                            oTextsArray.TextList.Add(strLine)
                        End If
                    Else
                        oTextsArray.TextList.Add(strLine)
                    End If
                Loop

                oSReader.Close()
                oFile.Close()
            Else
                oTextsArray = Nothing
            End If
        Else
            If bByteContent.Length > 0 Then
                Dim oLanguage As New roLanguage
                oLanguage.SetLanguageReference(strLanguageFileReference, strLanguageKey)

                Dim oDic As New Dictionary(Of String, String)

                Dim memStream As New MemoryStream(bByteContent)
                Dim oReader As New StreamReader(memStream)
                While Not oReader.EndOfStream
                    Dim sRead As String = oReader.ReadLine()

                    If sRead.Contains("=") Then
                        Dim langKey As String = sRead.Substring(0, sRead.IndexOf("="))
                        Dim langVal As String = sRead.Substring(sRead.IndexOf("=") + 1)

                        oDic.Add(langKey, langVal)
                    End If

                End While
                oReader.Close()
                memStream.Close()

                For Each oDicKey As String In oDic.Keys
                    oTextsArray.TextList.Add(oDicKey & "=" & oDic(oDicKey))
                Next
            End If

        End If

        Return oTextsArray
    End Function

    Public Function ProcessChanges(ByVal sourceDoc As Byte(), ByVal changes As Dictionary(Of String, String)) As Byte()

        Dim textWriter As New StringBuilder

        If sourceDoc IsNot Nothing AndAlso sourceDoc.Length > 0 Then

            Dim oDic As New Dictionary(Of String, String)

            Dim memStream As New MemoryStream(sourceDoc)
            Dim oReader As New StreamReader(memStream)
            While Not oReader.EndOfStream
                Dim sRead As String = oReader.ReadLine()

                If sRead.Contains("=") Then
                    Dim langKey As String = sRead.Substring(0, sRead.IndexOf("="))
                    Dim langVal As String = sRead.Substring(sRead.IndexOf("=") + 1)

                    oDic.Add(langKey, langVal)
                End If

            End While
            oReader.Close()
            memStream.Close()

            For Each oChangeKey As String In changes.Keys
                If oDic.ContainsKey(oChangeKey) Then
                    oDic(oChangeKey) = changes(oChangeKey)
                Else
                    oDic.Add(oChangeKey, changes(oChangeKey))
                End If
            Next

            textWriter.AppendLine("[Language]")
            For Each oDicKey As String In oDic.Keys
                textWriter.AppendLine(oDicKey & "=" & oDic(oDicKey))
            Next
        Else
            textWriter.AppendLine("[Language]")
            For Each sChange As String In changes.Keys
                textWriter.AppendLine(sChange & "=" & changes(sChange))
            Next
        End If

        Return roTypes.ToBytes(textWriter.ToString, Encoding.UTF8)
    End Function

    Public Sub SaveLanguage(ByVal LanguageReference As String, ByVal LanguageKey As String, ByVal Key As String, ByVal Value As String)
        Dim oTextsArray As New LanguageFile
        Dim strLanguagePath As String
        Dim strFile As String

        ' Descarto cadenas vacías
        If Value = "" Then Exit Sub

        Dim oSetting As New Robotics.VTBase.roSettings
        strLanguagePath = oSetting.GetVTSetting(eKeys.Language)

        ' Contruyo el nombre del fichero
        strFile = LanguageReference & "." & LanguageKey & ".LNG"

        If strLanguagePath <> "" Then
            If Not LanguageKey.Contains(".CUST") Then
                Dim oCurrentEncoding As System.Text.Encoding = roLanguageFileEncoding.DetectTextFileEncoding(strLanguagePath & "\" & strFile)
                Dim oSWriter As New StreamWriter(strLanguagePath & "\" & strFile, True, oCurrentEncoding)
                oSWriter.WriteLine(Key & "=" & Value)
                oSWriter.Close()
            Else
                ' Si es un fichero CUSTOM, puede que la línea ya exista. Debo actualizar y no crear una nueva
                Dim customFile As String = strLanguagePath & "\" & strFile
                If Not System.IO.File.Exists(customFile) Then
                    ' Lo creo y añado la línea
                    Dim oSWriter As New StreamWriter(customFile, True, System.Text.Encoding.UTF8)
                    oSWriter.WriteLine("[Language]")
                    oSWriter.WriteLine(Key & "=" & Value)
                    oSWriter.Close()
                Else
                    ' Lo recorro en busca de la línea
                    Dim oCurrentCustEncoding As System.Text.Encoding = roLanguageFileEncoding.DetectTextFileEncoding(customFile)
                    Dim lines() As String = IO.File.ReadAllLines(customFile)
                    Dim bFound As Boolean = False
                    For i As Integer = 0 To lines.Length - 1
                        If lines(i).StartsWith(Key) Then
                            lines(i) = Key & "=" & Value
                            bFound = True
                            Exit For
                        End If
                    Next
                    If Not bFound Then
                        ReDim Preserve lines(lines.Length)
                        lines(lines.Length - 1) = Key & "=" & Value
                    End If
                    IO.File.WriteAllLines(customFile, lines, oCurrentCustEncoding)
                End If
            End If
        End If
    End Sub

    Public Function GetDecimalDigitFormat() As String
        Dim oInfo As NumberFormatInfo = languageCulture.NumberFormat
        Return oInfo.NumberDecimalSeparator
    End Function

    Public Function GetShortDateFormat() As String
        Dim oInfo As DateTimeFormatInfo = languageCulture.DateTimeFormat
        Return oInfo.ShortDatePattern
    End Function

    Public Function GetShortDateSeparator() As String
        Dim oInfo As DateTimeFormatInfo = languageCulture.DateTimeFormat
        Return oInfo.DateSeparator
    End Function

    Public Function GetShortTimeFormat() As String
        Dim oInfo As DateTimeFormatInfo = languageCulture.DateTimeFormat
        Return "HH:mm"
    End Function

    Public Function GetMonthAndDayDateFormat() As String
        Dim oInfo As DateTimeFormatInfo = languageCulture.DateTimeFormat
        Dim strResult As String = oInfo.ShortDatePattern
        strResult = strResult.Replace("yy", "")
        If strResult.StartsWith(oInfo.DateSeparator) Then strResult = strResult.Substring(oInfo.DateSeparator.Length)
        If strResult.EndsWith(oInfo.DateSeparator) Then strResult = strResult.Substring(0, strResult.Length - oInfo.DateSeparator.Length)
        Return strResult
    End Function

    Public Function GetLanguageCulture() As CultureInfo
        Return languageCulture
    End Function

End Class

<DataContract>
Public Class LanguageFile
    Private oTextList As New ArrayList

    <DataMember>
    Public Property TextList() As ArrayList
        Get
            Return oTextList
        End Get
        Set(ByVal value As ArrayList)
            oTextList = value
        End Set
    End Property

End Class