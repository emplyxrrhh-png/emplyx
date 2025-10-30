'Imports Robotics.WebServices.CommonService
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports Robotics.Base.DTOs

<Serializable()>
<DataContract>
Public Class roLanguage
    Protected strLanguageFileReference As String
    Protected strLanguageKey As String

    Protected oTranslatedProperties As Generic.List(Of String) 'ArrayList
    Protected oNotTranslatedControlsTypes As New ArrayList
    Protected oControlsWithoutTooltip As New ArrayList
    Protected oControlsWithoutText As New ArrayList
    Protected oControlsWithoutDescription As New ArrayList

    Protected aStringTokens As New ArrayList
    Protected aStringSystemTokens As New ArrayList ' Tokens propios del sistema
    Protected aStringUserTokens As New ArrayList ' Tokens que seran aportados por el programa y que se reemplazaran en las posiscion ${1} a ${9}

    Private languageCulture As CultureInfo

    Public Sub New()
    End Sub

    Public Sub ClearUserTokens()
        aStringUserTokens.Clear()
    End Sub

    Public Sub AddUserToken(ByVal Token As String)
        aStringUserTokens.Add(Token)
    End Sub

    Public ReadOnly Property LanguageKey As String
        Get
            Return strLanguageKey
        End Get
    End Property

    Public Shared Function GetLanguageFromKey(ByVal LanguageKey As String) As String
        Dim strLanguageKey As String = "es-ES"
        If LanguageKey <> String.Empty Then
            Try
                Select Case LanguageKey
                    Case "ESP"
                        strLanguageKey = "es-ES"
                    Case "CAT"
                        strLanguageKey = "ca-ES"
                    Case "ENG"
                        strLanguageKey = "en-US"
                    Case "GAL"
                        strLanguageKey = "gl-ES"
                    Case "EKR"
                        strLanguageKey = "eu-ES"
                    Case "ITA"
                        strLanguageKey = "it-IT"
                    Case "FRA"
                        strLanguageKey = "fr-FR"
                    Case "CHN"
                        strLanguageKey = "zh-Hant"
                    Case "POR"
                        strLanguageKey = "pt-PT"
                    Case "SLK"
                        strLanguageKey = "sk-SK"
                    Case Else
                        strLanguageKey = "es-ES"
                End Select
            Catch ex As Exception
                strLanguageKey = "es-ES"
            End Try
        Else
            strLanguageKey = "es-ES"
        End If

        Return strLanguageKey
    End Function

    Public Function GetLanguageKey() As String
        If languageCulture IsNot Nothing Then
            Return languageCulture.Name
        Else
            Return "es-ES"
        End If
    End Function

    Public Shared Function GetLanguageCulture(ByVal LanguageKey As String) As CultureInfo
        Dim cCulture As CultureInfo = Nothing
        If LanguageKey <> String.Empty Then
            Try
                Select Case LanguageKey
                    Case "ESP"
                        cCulture = New CultureInfo("es-ES")
                    Case "CAT"
                        cCulture = New CultureInfo("ca-ES")
                    Case "ENG"
                        cCulture = New CultureInfo("en-US")
                    Case "GAL"
                        cCulture = New CultureInfo("gl-ES")
                    Case "EKR"
                        cCulture = New CultureInfo("eu-ES")
                    Case "ITA"
                        cCulture = New CultureInfo("it-IT")
                    Case "FRA"
                        cCulture = New CultureInfo("fr-FR")
                    Case "CHN"
                        cCulture = New CultureInfo("zh-Hant")
                    Case "POR"
                        cCulture = New CultureInfo("pt-PT")
                    Case "SLK"
                        cCulture = New CultureInfo("sk-SK")
                    Case Else
                        cCulture = New CultureInfo("es-ES")
                End Select
            Catch ex As Exception
                cCulture = New CultureInfo("es-ES")
            End Try
        Else
            cCulture = New CultureInfo("es-ES")
        End If

        Return cCulture
    End Function

    Public Sub SetLanguageReference(ByVal LanguageFileReference As String, ByVal LanguageKey As String)
        If LanguageFileReference <> String.Empty Then strLanguageFileReference = LanguageFileReference

        If LanguageKey <> String.Empty Then
            strLanguageKey = LanguageKey
            Try
                Select Case LanguageKey
                    Case "ESP"
                        languageCulture = New CultureInfo("es-ES")
                    Case "CAT"
                        languageCulture = New CultureInfo("ca-ES")
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

        Threading.Thread.CurrentThread.CurrentCulture = languageCulture
        Threading.Thread.CurrentThread.CurrentUICulture = languageCulture
    End Sub

    Protected Overridable Sub InitVariables()
        ' Inicializo el array de clases de controles que no debo inspeccionar
        oNotTranslatedControlsTypes.Clear()

        ' Inicializo el array de controles que no tienen tooltip
        oControlsWithoutTooltip.Clear()

        ' Inicializo el array de tokens
        aStringSystemTokens.Clear()

    End Sub

    Public Function TranslateRawText(ByVal TextKey As String, ByVal Scope As String) As String
        Return Translate(TextKey, Scope, True)
    End Function

    Public Function TranslateDictionary(ByVal TextKey As String, ByVal Scope As String, Optional ByVal bRawText As Boolean = False) As String
        Try
            Threading.Thread.CurrentThread.CurrentCulture = languageCulture
            Threading.Thread.CurrentThread.CurrentUICulture = languageCulture
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo("es-ES")
            Threading.Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-ES")
        End Try

        Dim strTranslation As String
        Dim rm As New VTResources.roResourceManager
        Dim strSearchKey As String = String.Empty

        If Not TextKey.StartsWith(Scope & ".") Then
            strSearchKey = Scope
        End If

        If strSearchKey = String.Empty Then
            strSearchKey = TextKey
        Else
            strSearchKey = strSearchKey & "." & TextKey
        End If

        If Not strSearchKey.EndsWith(".rotext") Then
            strSearchKey = strSearchKey & ".rotext"
        End If

        strTranslation = rm.GetString("Dictionary", strSearchKey.ToLower, "")

        If strTranslation <> "NotFound" Then
            If strTranslation = "(NoEntry)" Then
                strTranslation = "UNDEFINED LANGUAGE:" & TextKey & ".roText"
            End If
            If Not bRawText Then strTranslation = StringParse(strTranslation)
            Return strTranslation
        End If

        Return strTranslation
    End Function

    Public Function Translate(ByVal TextKey As String, ByVal Scope As String, Optional ByVal bRawText As Boolean = False, Optional ByVal sDefaultText As String = "") As String
        Try
            Threading.Thread.CurrentThread.CurrentCulture = languageCulture
            Threading.Thread.CurrentThread.CurrentUICulture = languageCulture
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo("es-ES")
            Threading.Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-ES")
        End Try

        Dim strTranslation As String
        Dim rm As New VTResources.roResourceManager
        Dim strSearchKey As String = String.Empty

        If Not TextKey.StartsWith(Scope & ".") Then
            strSearchKey = Scope
        End If

        If strSearchKey = String.Empty Then
            strSearchKey = TextKey
        Else
            strSearchKey = strSearchKey & "." & TextKey
        End If

        If Not strSearchKey.EndsWith(".rotext") Then
            strSearchKey = strSearchKey & ".rotext"
        End If

        strTranslation = rm.GetString(strLanguageFileReference, strSearchKey.ToLower, sDefaultText)

        If strTranslation <> "NotFound" Then
            If strTranslation = "(NoEntry)" Then
                strTranslation = "UNDEFINED LANGUAGE:" & TextKey & ".roText"
            End If
            If Not bRawText Then strTranslation = StringParse(strTranslation)
            Return strTranslation
        Else
            If TextKey = "reportdescription" Then
                strTranslation = ""
            End If
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

    Public Function TranslateDicTokens(ByVal strData As String) As String
        Dim bolFirstCharUpper As Boolean
        'Patrones de búsqueda
        Const DICTIONARY_PATTERN = "\${[a-zA-Z][a-zA-Z0-9.]+}"


        If strData Is Nothing OrElse strData = "" Then Return ""

        If Mid(strData, 1, 2) = "${" Then
            bolFirstCharUpper = True
        Else
            bolFirstCharUpper = False
        End If

        Dim myDictionayEvaluator As New MatchEvaluator(AddressOf TranslateMatch)
        strData = Regex.Replace(strData, DICTIONARY_PATTERN, myDictionayEvaluator)

        If bolFirstCharUpper Then
            strData = UCase(Mid(strData, 1, 1)) & Mid(strData, 2)
        End If

        Return strData

    End Function

    Private Function TranslateMatch(ByVal m As Match) As String
        '
        ' Delegado que trata cada token de diccionario encontrado en una cadena
        '
        Return TranslateDictionary(m.ToString.Substring(2, m.ToString.Length - 3), String.Empty, True)
    End Function

    Protected Function StringParse(ByVal strData As String, Optional ByVal ReplaceDefaultValue As String = "roNoEntry") As String
        Dim bolFirstCharUpper As Boolean

        'Patrones de búsqueda
        Const TOKEN_PATTERN = "\${[0-9]+}"


        If strData Is Nothing OrElse strData = "" Then Return ""

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

        ' Reemplazo tokens por valores pasados por parámetro
        aStringTokens = aStringUserTokens
        Dim myTokenEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf TokenMatch)
        strData = Regex.Replace(strData, TOKEN_PATTERN, myTokenEvaluator)

        aStringTokens = aStringUserTokens
        strData = Regex.Replace(strData, TOKEN_PATTERN, myTokenEvaluator)

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
        If aStringTokens IsNot Nothing Then
            If tokenPos <= aStringTokens.Capacity AndAlso tokenPos > 0 AndAlso aStringTokens.Count > 0 AndAlso aStringTokens(tokenPos - 1) IsNot Nothing Then
                Return aStringTokens(tokenPos - 1).ToString
            Else
                Return String.Empty
            End If
        Else
            Return " "
        End If
    End Function

    Public Function Keyword(ByVal key As String) As String
        Return Me.TranslateDictionary(key, "")
    End Function

    Public Function KeywordJavaScript(ByVal key As String) As String
        Return Me.Keyword(key).Replace("'", "\'")
    End Function

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