Imports System.IO
Imports System.Math
Imports System.Reflection
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports System.Text.RegularExpressions
Imports Robotics.Base.DTOs

Public Class roSupport
    '
    ' Clase con funciones de propósito general
    '

    'Tipos de redondeo
    Public Enum eRoundType
        roRoundTop = 1
        roRoundBottom = -1
        roRoundNear = 0
    End Enum

    ' Tipos de Messages
    Public Enum eMessageType
        roInfo = 1
        roCritical = 2
        roError = 3
        roDebug = 4
    End Enum

#Region "Declarations - Constructor"

    Private strActivaLanguage As String

    ' Variables para la generación de IDs
    Private Shared mLastGetIDTimer As Double
    Private Shared mIDNumericSeed As Integer

    Public Sub New(Optional ByVal _ActiveLanguage As String = "ESP")

        Me.strActivaLanguage = _ActiveLanguage

        ' Prepara IDs aleatorios
        Randomize(Now.TimeOfDay.TotalSeconds)
        mIDNumericSeed = Int(10000 * Rnd())
        mLastGetIDTimer = Now.TimeOfDay.TotalSeconds

    End Sub

#End Region

#Region "Properties"

    Public Property ActiveLanguage() As String
        Get
            Return Me.strActivaLanguage
        End Get
        Set(ByVal value As String)
            Me.strActivaLanguage = value
        End Set
    End Property

#End Region

#Region "Methods"

    Public Shared Function Sanitize(text As String) As String
        ' HTML reserved characters replacement
        Dim noHTML As String = Regex.Replace(text, $"<[^>]+>|&nbsp;", "").Trim()
        Dim noHTMLNormalised As String = Regex.Replace(noHTML, "\s{2,}", " ")
        Return noHTMLNormalised
    End Function

    Public Shared Function TruncateScreenText(ByVal text As String, Optional ByVal maxlength As Integer = 30) As String
        If text.Length > maxlength Then
            text = text.Substring(0, maxlength) & "..."
        End If
        Return text
    End Function

    Public Shared Function DeepClone(Of T)(ByRef orig As T) As T

        ' Don't serialize a null object, simply return the default for that object
        If (Object.ReferenceEquals(orig, Nothing)) Then Return Nothing

        Dim formatter As New BinaryFormatter()
        Dim stream As New MemoryStream()

        formatter.Serialize(stream, orig)
        stream.Seek(0, SeekOrigin.Begin)

        Return CType(formatter.Deserialize(stream), T)

    End Function

    Public Shared Function GetWindowsDir() As String
        Dim WinSysPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.System)
        Dim WinPath As String = WinSysPath.Substring(0, WinSysPath.LastIndexOf("\"))

        Return WinPath
    End Function

    Public Shared Function GetExeName() As String

        Return Assembly.GetExecutingAssembly().GetName().Name
    End Function

    Public Shared Function NumRound(ByVal RoundType As eRoundType, ByVal x As Double, ByVal RoundFactor As Long) As Long
        Dim Temp As Double

        Select Case RoundType
            Case eRoundType.roRoundBottom
                'Despues ajustamos el valor entero
                Temp = Int(x)
                Return Temp - Temp Mod RoundFactor

            Case eRoundType.roRoundTop
                Temp = Int(x)
                Temp = (Temp + IIf(x = Temp, 0, 1))
                Return Temp + (RoundFactor - (Temp Mod RoundFactor))

            Case eRoundType.roRoundNear
                Temp = Fix(x + 0.5 * Sign(x))
                ' Tratamos el redondeo de 0.5 de modo especial
                If x - Int(x) = 0.5 Then
                    If Temp / 2 <> Int(Temp / 2) Then ' Si Temp es impar
                        ' Reducimos hacia un numero par
                        Temp = Temp - Sign(x)
                    End If
                End If

                If (Temp Mod RoundFactor) >= RoundFactor / 2 Then
                    Return Temp + (RoundFactor - (Temp Mod RoundFactor))
                ElseIf (Temp Mod RoundFactor) < RoundFactor / 2 Then
                    Return Temp - Temp Mod RoundFactor
                ElseIf (Temp Mod RoundFactor) = RoundFactor / 2 Then
                    Return Temp + (RoundFactor - (Temp Mod RoundFactor))
                Else
                    Return Temp + (RoundFactor - (Temp Mod RoundFactor))
                End If

                'Devolvemos el valor
            Case Else
                Return Int(x)
        End Select

    End Function

    Public Shared Function GetGUID() As String
        '
        ' Crea un GUID
        '
        Try
            Return System.Guid.NewGuid().ToString()
        Catch ex As Exception
            Err.Raise(9123, "roCommon", "GetGUID: Error in CoCreateGUID")
        End Try

        Return String.Empty
    End Function

    Public Shared Function GetSmallID(ByVal Length As Integer) As String
        '
        ' Devuelve un ID del tamaño seleccionado
        '
        Dim s As String

        If Length > 8 Then
            Return Left$(GetGUID, Length)
        Else
            While mLastGetIDTimer = Now.TimeOfDay.TotalSeconds
                System.Threading.Thread.Sleep(100)
            End While
            mLastGetIDTimer = Now.TimeOfDay.TotalSeconds
            s = Format$(Now.TimeOfDay.TotalSeconds * 100, "00000000")
            Return Right$(s, Length)
        End If

    End Function

    Public Shared Function StringEncodeControlChars(ByVal sInput As String) As String
        '
        ' Cambia caracteres de control de un string por tokens.
        '  Llamando de nuevo a la funcion StringDecodeControlChars se obtiene de nuevo el
        '  string original.

        Dim sOutput As String
        Dim I As Integer

        sOutput = sInput
        For I = 1 To 31
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        For I = 60 To 62
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        For I = 123 To 255
            sOutput = sOutput.Replace(Chr(I), "%" & I & "%")
        Next
        Return sOutput

    End Function

    Public Shared Function StringDecodeControlChars(ByVal sInput As String) As String
        '
        ' Descodifica un string codificado previamente con StringEncodeControlChars.
        '

        Dim sOutput As String
        Dim I As Integer

        sOutput = sInput
        For I = 1 To 31
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next
        For I = 60 To 62
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next
        For I = 123 To 255
            sOutput = sOutput.Replace("%" & I & "%", Chr(I))
        Next

        Return sOutput

    End Function

    Public Function INIRead(ByVal Filename As String, ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As String = "", Optional ByVal Tokens As Object = Nothing, Optional ByVal strLicenseContent As Object = Nothing) As String

        If Filename = "remote" AndAlso strLicenseContent <> String.Empty Then
            Try
                Dim ini As New MadMilkman.Ini.IniFile()
                Dim iniContent As Byte() = Encoding.UTF8.GetBytes(strLicenseContent)
                Dim memStream As New MemoryStream(iniContent)
                ini.Load(memStream)

                If ini.Sections(Section) IsNot Nothing AndAlso ini.Sections(Section).Keys(Key) IsNot Nothing Then
                    Return StringParse(ini.Sections(Section).Keys(Key).Value.ToString, Tokens)
                Else
                    Return String.Empty
                End If
            Catch ex As Exception
                Return ""
            End Try
        Else
            Return ""
        End If

        ' Elimina cualquier error
        Err.Clear()

    End Function

    Public Function StringParse(ByVal strData As String, Optional ByVal Tokens As Object = Nothing, Optional ByVal ReplaceDefaultValue As String = "roNoEntry") As String
        '
        ' Modifica el string cambiando los tokens por su valor
        '  Además de los tokens predefinidos ($M,$W, etc), se puede pasar un
        '   diccionario, de forma que reemplaza strings "${key}" por el valor
        '   en el diccionario.
        '
        Dim replaceToken As String
        Dim dictionaryToken As String

        Dim bolFirstCharUpper As Boolean

        If strData Is Nothing OrElse strData = "" Then Return ""

        ' Miro si hay un token al inicio de la cadena para, una vez traducido, poner la primera letra en mayúscula
        If Mid(strData, 1, 2) = "${" Then
            bolFirstCharUpper = True
        Else
            bolFirstCharUpper = False
        End If

        Dim oSettings As New roSettings

        ' Reemplaza tokens fijos
        strData = StringReplace(strData, "$(APPPATH)", Environment.CurrentDirectory)
        strData = StringReplace(strData, "$(WINSYSPATH)", Environment.SystemDirectory)
        strData = StringReplace(strData, "$(WINPATH)", GetWindowsDir)

        strData = StringReplace(strData, "$(VTSYSTEMPATH)", oSettings.GetVTSetting(eKeys.System))
        strData = StringReplace(strData, "$(VTCONFIGPATH)", oSettings.GetVTSetting(eKeys.Config))
        strData = StringReplace(strData, "$(VTREPORTSPATH)", oSettings.GetVTSetting(eKeys.Reports))
        strData = StringReplace(strData, "$(VTDATALINKPATH)", oSettings.GetVTSetting(eKeys.DataLink))
        strData = StringReplace(strData, "$(VTREADINGSPATH)", oSettings.GetVTSetting(eKeys.Readings))

        strData = StringReplace(strData, "$(APPNAME)", GetExeName)
        strData = StringReplace(strData, "$CR", vbCr)
        strData = StringReplace(strData, "$CRLF", vbCrLf)
        strData = StringReplace(strData, "$LF", vbLf)
        strData = StringReplace(strData, "$NL", Chr(10))
        strData = StringReplace(strData, "$H", Hour(Now))
        strData = StringReplace(strData, "$N", Minute(Now))
        strData = StringReplace(strData, "$S", Second(Now))
        strData = StringReplace(strData, "$YY", Year(Now))
        strData = StringReplace(strData, "$Y", Format$(Year(Now), "yy"))
        strData = StringReplace(strData, "$MMM", MonthName(Month(Now), False))
        strData = StringReplace(strData, "$MM", MonthName(Month(Now), True))
        strData = StringReplace(strData, "$M", Now.Month)
        strData = StringReplace(strData, "$D", Now.Day)
        strData = StringReplace(strData, "$WWW", WeekdayName(Weekday(Now), False))
        strData = StringReplace(strData, "$WW", WeekdayName(Weekday(Now), True))
        strData = StringReplace(strData, "$W", Weekday(Now))

        ' Reemplaza tokens por valores en el diccionario
        While InStr(strData, "${") > 0
            ' Obtiene token a reemplazar
            replaceToken = Mid$(strData, InStr(strData, "${") + 2, 9999)
            replaceToken = Left$(replaceToken, InStr(replaceToken, "}") - 1)

            ' Obtiene string a insertar
            dictionaryToken = ""
            If IsNumeric(replaceToken) AndAlso (Tokens IsNot Nothing) Then
                dictionaryToken = Tokens(CLng(replaceToken) - 1)
            End If

            ' Si no está en los argumentos locales, busca token en diccionario
            If dictionaryToken = "" Then dictionaryToken = Keyword(replaceToken)

            ' Inserta string
            'strData = StringReplace(strData, "${" & replaceToken & "}", dictionaryToken)
            strData = strData.Replace("${" & replaceToken & "}", dictionaryToken)

        End While

        ' Si la primera palabra de la frase viene del diccionario, pasa a mayusuculas primera letra
        If bolFirstCharUpper Then ' dictionaryToken <> "" And InStr(strData, dictionaryToken) = 0 Then
            strData = strData.Substring(0, 1).ToUpper & strData.Substring(1)
        End If

        Return strData

    End Function

    Public Function Keyword(ByVal Word As String) As String
        '
        ' Devuelve un valor del diccionario
        '
        'Dim oSettings As New roSettings
        'Dim mKeywordFile As String = oSettings.GetVTSetting(roSettings.eKeys.Language) & "\Dictionary" & Me.strActivaLanguage & ".DIC"
        'Keyword = INIRead(mKeywordFile, "Dictionary", Word, "")
        Dim strRet As String = ""

        Dim oLanguage As New roLanguage
        oLanguage.SetLanguageReference("", Me.strActivaLanguage)
        strRet = oLanguage.Keyword(Word)

        Return strRet

    End Function

    Public Function StringReplace(ByVal sInput As String, ByVal sToken As String, ByVal sNewToken As String) As String

        ' Search&Replace de substrings
        Dim strRet As String = ""
        Dim sOutput As String

        Try

            sOutput = ""
            While InStr(sInput, sToken) > 0
                sOutput = $"{sOutput}{Mid$(sInput, 1, InStr(sInput, sToken) - 1)}{sNewToken}"
                sInput = Mid$(sInput, InStr(sInput, sToken) + Len(sToken), 9999)
            End While
            strRet = sOutput & sInput
        Catch ex As Exception
            strRet = sInput
        End Try

        Return strRet

    End Function

#End Region

#Region "Shared"

    Public Shared Function OlsonTimeZoneToTimeZoneInfo(olsonTimeZoneId As String) As TimeZoneInfo
        Dim olsonWindowsTimes = New Dictionary(Of String, String)() From {
            {"Etc/GMT+12", "Dateline Standard Time"},
            {"Etc/GMT+11", "UTC-11"},
            {"Pacific/Pago_Pago", "UTC-11"},
            {"Pacific/Niue", "UTC-11"},
            {"Pacific/Midway", "UTC-11"},
            {"America/Adak", "Aleutian Standard Time"},
            {"Pacific/Honolulu", "Hawaiian Standard Time"},
            {"Pacific/Rarotonga", "Hawaiian Standard Time"},
            {"Pacific/Tahiti", "Hawaiian Standard Time"},
            {"Pacific/Johnston", "Hawaiian Standard Time"},
            {"Etc/GMT+10", "Hawaiian Standard Time"},
            {"Pacific/Marquesas", "Marquesas Standard Time"},
            {"America/Anchorage", "Alaskan Standard Time"},
            {"America/Juneau", "Alaskan Standard Time"},
            {"America/Metlakatla", "Alaskan Standard Time"},
            {"America/Nome", "Alaskan Standard Time"},
            {"America/Sitka", "Alaskan Standard Time"},
            {"America/Yakutat", "Alaskan Standard Time"},
            {"Etc/GMT+9", "UTC-09"},
            {"Pacific/Gambier", "UTC-09"},
            {"America/Tijuana", "Pacific Standard Time (Mexico)"},
            {"America/Santa_Isabel", "Pacific Standard Time (Mexico)"},
            {"Etc/GMT+8", "UTC-08"},
            {"Pacific/Pitcairn", "UTC-08"},
            {"America/Los_Angeles", "Pacific Standard Time"},
            {"America/Vancouver", "Pacific Standard Time"},
            {"America/Dawson", "Pacific Standard Time"},
            {"America/Whitehorse", "Pacific Standard Time"},
            {"PST8PDT", "Pacific Standard Time"},
            {"America/Phoenix", "US Mountain Standard Time"},
            {"America/Dawson_Creek", "US Mountain Standard Time"},
            {"America/Creston", "US Mountain Standard Time"},
            {"America/Fort_Nelson", "US Mountain Standard Time"},
            {"America/Hermosillo", "US Mountain Standard Time"},
            {"Etc/GMT+7", "US Mountain Standard Time"},
            {"America/Chihuahua", "Mountain Standard Time (Mexico)"},
            {"America/Mazatlan", "Mountain Standard Time (Mexico)"},
            {"America/Denver", "Mountain Standard Time"},
            {"America/Edmonton", "Mountain Standard Time"},
            {"America/Cambridge_Bay", "Mountain Standard Time"},
            {"America/Inuvik", "Mountain Standard Time"},
            {"America/Yellowknife", "Mountain Standard Time"},
            {"America/Ojinaga", "Mountain Standard Time"},
            {"America/Boise", "Mountain Standard Time"},
            {"MST7MDT", "Mountain Standard Time"},
            {"America/Guatemala", "Central America Standard Time"},
            {"America/Belize", "Central America Standard Time"},
            {"America/Costa_Rica", "Central America Standard Time"},
            {"Pacific/Galapagos", "Central America Standard Time"},
            {"America/Tegucigalpa", "Central America Standard Time"},
            {"America/Managua", "Central America Standard Time"},
            {"America/El_Salvador", "Central America Standard Time"},
            {"Etc/GMT+6", "Central America Standard Time"},
            {"America/Chicago", "Central Standard Time"},
            {"America/Winnipeg", "Central Standard Time"},
            {"America/Rainy_River", "Central Standard Time"},
            {"America/Rankin_Inlet", "Central Standard Time"},
            {"America/Resolute", "Central Standard Time"},
            {"America/Matamoros", "Central Standard Time"},
            {"America/Indiana/Knox", "Central Standard Time"},
            {"America/Indiana/Tell_City", "Central Standard Time"},
            {"America/Menominee", "Central Standard Time"},
            {"America/North_Dakota/Beulah", "Central Standard Time"},
            {"America/North_Dakota/Center", "Central Standard Time"},
            {"America/North_Dakota/New_Salem", "Central Standard Time"},
            {"CST6CDT", "Central Standard Time"},
            {"Pacific/Easter", "Easter Island Standard Time"},
            {"America/Mexico_City", "Central Standard Time (Mexico)"},
            {"America/Bahia_Banderas", "Central Standard Time (Mexico)"},
            {"America/Merida", "Central Standard Time (Mexico)"},
            {"America/Monterrey", "Central Standard Time (Mexico)"},
            {"America/Regina", "Canada Central Standard Time"},
            {"America/Swift_Current", "Canada Central Standard Time"},
            {"America/Bogota", "SA Pacific Standard Time"},
            {"America/Rio_Branco", "SA Pacific Standard Time"},
            {"America/Eirunepe", "SA Pacific Standard Time"},
            {"America/Coral_Harbour", "SA Pacific Standard Time"},
            {"America/Guayaquil", "SA Pacific Standard Time"},
            {"America/Jamaica", "SA Pacific Standard Time"},
            {"America/Cayman", "SA Pacific Standard Time"},
            {"America/Panama", "SA Pacific Standard Time"},
            {"America/Lima", "SA Pacific Standard Time"},
            {"Etc/GMT+5", "SA Pacific Standard Time"},
            {"America/Cancun", "Eastern Standard Time (Mexico)"},
            {"America/New_York", "Eastern Standard Time"},
            {"America/Nassau", "Eastern Standard Time"},
            {"America/Toronto", "Eastern Standard Time"},
            {"America/Iqaluit", "Eastern Standard Time"},
            {"America/Montreal", "Eastern Standard Time"},
            {"America/Nipigon", "Eastern Standard Time"},
            {"America/Pangnirtung", "Eastern Standard Time"},
            {"America/Thunder_Bay", "Eastern Standard Time"},
            {"America/Detroit", "Eastern Standard Time"},
            {"America/Indiana/Petersburg", "Eastern Standard Time"},
            {"America/Indiana/Vincennes", "Eastern Standard Time"},
            {"America/Indiana/Winamac", "Eastern Standard Time"},
            {"America/Kentucky/Monticello", "Eastern Standard Time"},
            {"America/Louisville", "Eastern Standard Time"},
            {"EST5EDT", "Eastern Standard Time"},
            {"America/Port-au-Prince", "Haiti Standard Time"},
            {"America/Havana", "Cuba Standard Time"},
            {"America/Indianapolis", "US Eastern Standard Time"},
            {"America/Indiana/Marengo", "US Eastern Standard Time"},
            {"America/Indiana/Vevay", "US Eastern Standard Time"},
            {"America/Asuncion", "Paraguay Standard Time"},
            {"America/Halifax", "Atlantic Standard Time"},
            {"Atlantic/Bermuda", "Atlantic Standard Time"},
            {"America/Glace_Bay", "Atlantic Standard Time"},
            {"America/Goose_Bay", "Atlantic Standard Time"},
            {"America/Moncton", "Atlantic Standard Time"},
            {"America/Thule", "Atlantic Standard Time"},
            {"America/Caracas", "Venezuela Standard Time"},
            {"America/Cuiaba", "Central Brazilian Standard Time"},
            {"America/Campo_Grande", "Central Brazilian Standard Time"},
            {"America/La_Paz", "SA Western Standard Time"},
            {"America/Antigua", "SA Western Standard Time"},
            {"America/Anguilla", "SA Western Standard Time"},
            {"America/Aruba", "SA Western Standard Time"},
            {"America/Barbados", "SA Western Standard Time"},
            {"America/St_Barthelemy", "SA Western Standard Time"},
            {"America/Kralendijk", "SA Western Standard Time"},
            {"America/Manaus", "SA Western Standard Time"},
            {"America/Boa_Vista", "SA Western Standard Time"},
            {"America/Porto_Velho", "SA Western Standard Time"},
            {"America/Blanc-Sablon", "SA Western Standard Time"},
            {"America/Curacao", "SA Western Standard Time"},
            {"America/Dominica", "SA Western Standard Time"},
            {"America/Santo_Domingo", "SA Western Standard Time"},
            {"America/Grenada", "SA Western Standard Time"},
            {"America/Guadeloupe", "SA Western Standard Time"},
            {"America/Guyana", "SA Western Standard Time"},
            {"America/St_Kitts", "SA Western Standard Time"},
            {"America/St_Lucia", "SA Western Standard Time"},
            {"America/Marigot", "SA Western Standard Time"},
            {"America/Martinique", "SA Western Standard Time"},
            {"America/Montserrat", "SA Western Standard Time"},
            {"America/Puerto_Rico", "SA Western Standard Time"},
            {"America/Lower_Princes", "SA Western Standard Time"},
            {"America/Port_of_Spain", "SA Western Standard Time"},
            {"America/St_Vincent", "SA Western Standard Time"},
            {"America/Tortola", "SA Western Standard Time"},
            {"America/St_Thomas", "SA Western Standard Time"},
            {"Etc/GMT+4", "SA Western Standard Time"},
            {"America/Santiago", "Pacific SA Standard Time"},
            {"America/Grand_Turk", "Turks And Caicos Standard Time"},
            {"America/St_Johns", "Newfoundland Standard Time"},
            {"America/Araguaina", "Tocantins Standard Time"},
            {"America/Sao_Paulo", "E. South America Standard Time"},
            {"America/Cayenne", "SA Eastern Standard Time"},
            {"Antarctica/Rothera", "SA Eastern Standard Time"},
            {"America/Fortaleza", "SA Eastern Standard Time"},
            {"America/Belem", "SA Eastern Standard Time"},
            {"America/Maceio", "SA Eastern Standard Time"},
            {"America/Recife", "SA Eastern Standard Time"},
            {"America/Santarem", "SA Eastern Standard Time"},
            {"Atlantic/Stanley", "SA Eastern Standard Time"},
            {"America/Paramaribo", "SA Eastern Standard Time"},
            {"Etc/GMT+3", "SA Eastern Standard Time"},
            {"America/Buenos_Aires", "Argentina Standard Time"},
            {"America/Argentina/La_Rioja", "Argentina Standard Time"},
            {"America/Argentina/Rio_Gallegos", "Argentina Standard Time"},
            {"America/Argentina/Salta", "Argentina Standard Time"},
            {"America/Argentina/San_Juan", "Argentina Standard Time"},
            {"America/Argentina/San_Luis", "Argentina Standard Time"},
            {"America/Argentina/Tucuman", "Argentina Standard Time"},
            {"America/Argentina/Ushuaia", "Argentina Standard Time"},
            {"America/Catamarca", "Argentina Standard Time"},
            {"America/Cordoba", "Argentina Standard Time"},
            {"America/Jujuy", "Argentina Standard Time"},
            {"America/Mendoza", "Argentina Standard Time"},
            {"America/Godthab", "Greenland Standard Time"},
            {"America/Montevideo", "Montevideo Standard Time"},
            {"Antarctica/Palmer", "Magallanes Standard Time"},
            {"America/Punta_Arenas", "Magallanes Standard Time"},
            {"America/Miquelon", "Saint Pierre Standard Time"},
            {"America/Bahia", "Bahia Standard Time"},
            {"America/Noronha", "UTC-02"},
            {"Atlantic/South_Georgia", "UTC-02"},
            {"Etc/GMT+2", "UTC-02"},
            {"Atlantic/Azores", "Azores Standard Time"},
            {"America/Scoresbysund", "Azores Standard Time"},
            {"Atlantic/Cape_Verde", "Cape Verde Standard Time"},
            {"Etc/GMT+1", "Cape Verde Standard Time"},
            {"Etc/GMT", "UTC"},
            {"America/Danmarkshavn", "UTC"},
            {"Etc/GMT Etc/UTC", "UTC"},
            {"Africa/El_Aaiun", "Morocco Standard Time"},
            {"Africa/Casablanca", "Morocco Standard Time"},
            {"Europe/London", "GMT Standard Time"},
            {"Atlantic/Canary", "GMT Standard Time"},
            {"Atlantic/Faeroe", "GMT Standard Time"},
            {"Europe/Guernsey", "GMT Standard Time"},
            {"Europe/Dublin", "GMT Standard Time"},
            {"Europe/Isle_of_Man", "GMT Standard Time"},
            {"Europe/Jersey", "GMT Standard Time"},
            {"Europe/Lisbon", "GMT Standard Time"},
            {"Atlantic/Madeira", "GMT Standard Time"},
            {"Atlantic/Reykjavik", "Greenwich Standard Time"},
            {"Africa/Ouagadougou", "Greenwich Standard Time"},
            {"Africa/Abidjan", "Greenwich Standard Time"},
            {"Africa/Accra", "Greenwich Standard Time"},
            {"Africa/Banjul", "Greenwich Standard Time"},
            {"Africa/Conakry", "Greenwich Standard Time"},
            {"Africa/Bissau", "Greenwich Standard Time"},
            {"Africa/Monrovia", "Greenwich Standard Time"},
            {"Africa/Bamako", "Greenwich Standard Time"},
            {"Africa/Nouakchott", "Greenwich Standard Time"},
            {"Atlantic/St_Helena", "Greenwich Standard Time"},
            {"Africa/Freetown", "Greenwich Standard Time"},
            {"Africa/Dakar", "Greenwich Standard Time"},
            {"Africa/Sao_Tome", "Greenwich Standard Time"},
            {"Africa/Lome", "Greenwich Standard Time"},
            {"Europe/Berlin", "W. Europe Standard Time"},
            {"Europe/Andorra", "W. Europe Standard Time"},
            {"Europe/Vienna", "W. Europe Standard Time"},
            {"Europe/Zurich", "W. Europe Standard Time"},
            {"Europe/Busingen", "W. Europe Standard Time"},
            {"Europe/Gibraltar", "W. Europe Standard Time"},
            {"Europe/Rome", "W. Europe Standard Time"},
            {"Europe/Vaduz", "W. Europe Standard Time"},
            {"Europe/Luxembourg", "W. Europe Standard Time"},
            {"Europe/Monaco", "W. Europe Standard Time"},
            {"Europe/Malta", "W. Europe Standard Time"},
            {"Europe/Amsterdam", "W. Europe Standard Time"},
            {"Europe/Oslo", "W. Europe Standard Time"},
            {"Europe/Stockholm", "W. Europe Standard Time"},
            {"Arctic/Longyearbyen", "W. Europe Standard Time"},
            {"Europe/San_Marino", "W. Europe Standard Time"},
            {"Europe/Vatican", "W. Europe Standard Time"},
            {"Europe/Budapest", "Central Europe Standard Time"},
            {"Europe/Tirane", "Central Europe Standard Time"},
            {"Europe/Prague", "Central Europe Standard Time"},
            {"Europe/Podgorica", "Central Europe Standard Time"},
            {"Europe/Belgrade", "Central Europe Standard Time"},
            {"Europe/Ljubljana", "Central Europe Standard Time"},
            {"Europe/Bratislava", "Central Europe Standard Time"},
            {"Europe/Paris", "Romance Standard Time"},
            {"Europe/Brussels", "Romance Standard Time"},
            {"Europe/Copenhagen", "Romance Standard Time"},
            {"Europe/Madrid", "Romance Standard Time"},
            {"Africa/Ceuta", "Romance Standard Time"},
            {"Europe/Warsaw", "Central European Standard Time"},
            {"Europe/Sarajevo", "Central European Standard Time"},
            {"Europe/Zagreb", "Central European Standard Time"},
            {"Europe/Skopje", "Central European Standard Time"},
            {"Africa/Luanda", "W. Central Africa Standard Time"},
            {"Africa/Porto-Novo", "W. Central Africa Standard Time"},
            {"Africa/Kinshasa", "W. Central Africa Standard Time"},
            {"Africa/Bangui", "W. Central Africa Standard Time"},
            {"Africa/Brazzaville", "W. Central Africa Standard Time"},
            {"Africa/Douala", "W. Central Africa Standard Time"},
            {"Africa/Algiers", "W. Central Africa Standard Time"},
            {"Africa/Libreville", "W. Central Africa Standard Time"},
            {"Africa/Malabo", "W. Central Africa Standard Time"},
            {"Africa/Niamey", "W. Central Africa Standard Time"},
            {"Africa/Lagos", "W. Central Africa Standard Time"},
            {"Africa/Ndjamena", "W. Central Africa Standard Time"},
            {"Africa/Tunis", "W. Central Africa Standard Time"},
            {"Etc/GMT-1", "W. Central Africa Standard Time"},
            {"Africa/Windhoek", "Namibia Standard Time"},
            {"Asia/Amman", "Jordan Standard Time"},
            {"Asia/Nicosia", "GTB Standard Time"},
            {"Europe/Athens", "GTB Standard Time"},
            {"Europe/Bucharest", "GTB Standard Time"},
            {"Asia/Beirut", "Middle East Standard Time"},
            {"Africa/Cairo", "Egypt Standard Time"},
            {"Europe/Chisinau", "E. Europe Standard Time"},
            {"Asia/Damascus", "Syria Standard Time"},
            {"Asia/Hebron", "West Bank Standard Time"},
            {"Asia/Gaza", "West Bank Standard Time"},
            {"Africa/Johannesburg", "South Africa Standard Time"},
            {"Africa/Bujumbura", "South Africa Standard Time"},
            {"Africa/Gaborone", "South Africa Standard Time"},
            {"Africa/Lubumbashi", "South Africa Standard Time"},
            {"Africa/Maseru", "South Africa Standard Time"},
            {"Africa/Blantyre", "South Africa Standard Time"},
            {"Africa/Maputo", "South Africa Standard Time"},
            {"Africa/Kigali", "South Africa Standard Time"},
            {"Africa/Mbabane", "South Africa Standard Time"},
            {"Africa/Lusaka", "South Africa Standard Time"},
            {"Africa/Harare", "South Africa Standard Time"},
            {"Etc/GMT-2", "South Africa Standard Time"},
            {"Europe/Kiev", "FLE Standard Time"},
            {"Europe/Mariehamn", "FLE Standard Time"},
            {"Europe/Sofia", "FLE Standard Time"},
            {"Europe/Tallinn", "FLE Standard Time"},
            {"Europe/Helsinki", "FLE Standard Time"},
            {"Europe/Vilnius", "FLE Standard Time"},
            {"Europe/Riga", "FLE Standard Time"},
            {"Europe/Uzhgorod", "FLE Standard Time"},
            {"Europe/Zaporozhye", "FLE Standard Time"},
            {"Asia/Jerusalem", "Israel Standard Time"},
            {"Europe/Kaliningrad", "Kaliningrad Standard Time"},
            {"Africa/Tripoli", "Libya Standard Time"},
            {"Asia/Baghdad", "Arabic Standard Time"},
            {"Europe/Istanbul", "Turkey Standard Time"},
            {"Asia/Famagusta", "Turkey Standard Time"},
            {"Asia/Riyadh", "Arab Standard Time"},
            {"Asia/Bahrain", "Arab Standard Time"},
            {"Asia/Kuwait", "Arab Standard Time"},
            {"Asia/Qatar", "Arab Standard Time"},
            {"Asia/Aden", "Arab Standard Time"},
            {"Europe/Minsk", "Belarus Standard Time"},
            {"Europe/Moscow", "Russian Standard Time"},
            {"Europe/Kirov", "Russian Standard Time"},
            {"Europe/Volgograd", "Russian Standard Time"},
            {"Europe/Simferopol", "Russian Standard Time"},
            {"Africa/Nairobi", "E. Africa Standard Time"},
            {"Antarctica/Syowa", "E. Africa Standard Time"},
            {"Africa/Djibouti", "E. Africa Standard Time"},
            {"Africa/Asmera", "E. Africa Standard Time"},
            {"Africa/Addis_Ababa", "E. Africa Standard Time"},
            {"Indian/Comoro", "E. Africa Standard Time"},
            {"Indian/Antananarivo", "E. Africa Standard Time"},
            {"Africa/Khartoum", "E. Africa Standard Time"},
            {"Africa/Mogadishu", "E. Africa Standard Time"},
            {"Africa/Juba", "E. Africa Standard Time"},
            {"Africa/Dar_es_Salaam", "E. Africa Standard Time"},
            {"Africa/Kampala", "E. Africa Standard Time"},
            {"Indian/Mayotte", "E. Africa Standard Time"},
            {"Etc/GMT-3", "E. Africa Standard Time"},
            {"Asia/Tehran", "Iran Standard Time"},
            {"Asia/Dubai", "Arabian Standard Time"},
            {"Asia/Muscat", "Arabian Standard Time"},
            {"Etc/GMT-4", "Arabian Standard Time"},
            {"Europe/Astrakhan", "Astrakhan Standard Time"},
            {"Europe/Ulyanovsk", "Astrakhan Standard Time"},
            {"Asia/Baku", "Azerbaijan Standard Time"},
            {"Europe/Samara", "Russia Time Zone 3"},
            {"Indian/Mauritius", "Mauritius Standard Time"},
            {"Indian/Reunion", "Mauritius Standard Time"},
            {"Indian/Mahe", "Mauritius Standard Time"},
            {"Europe/Saratov", "Saratov Standard Time"},
            {"Asia/Tbilisi", "Georgian Standard Time"},
            {"Asia/Yerevan", "Caucasus Standard Time"},
            {"Asia/Kabul", "Afghanistan Standard Time"},
            {"Asia/Tashkent", "West Asia Standard Time"},
            {"Antarctica/Mawson", "West Asia Standard Time"},
            {"Asia/Oral", "West Asia Standard Time"},
            {"Asia/Aqtau", "West Asia Standard Time"},
            {"Asia/Aqtobe", "West Asia Standard Time"},
            {"Asia/Atyrau", "West Asia Standard Time"},
            {"Indian/Maldives", "West Asia Standard Time"},
            {"Indian/Kerguelen", "West Asia Standard Time"},
            {"Asia/Dushanbe", "West Asia Standard Time"},
            {"Asia/Ashgabat", "West Asia Standard Time"},
            {"Asia/Samarkand", "West Asia Standard Time"},
            {"Etc/GMT-5", "West Asia Standard Time"},
            {"Asia/Yekaterinburg", "Ekaterinburg Standard Time"},
            {"Asia/Karachi", "Pakistan Standard Time"},
            {"Asia/Calcutta", "India Standard Time"},
            {"Asia/Colombo", "Sri Lanka Standard Time"},
            {"Asia/Katmandu", "Nepal Standard Time"},
            {"Asia/Almaty", "Central Asia Standard Time"},
            {"Antarctica/Vostok", "Central Asia Standard Time"},
            {"Asia/Urumqi", "Central Asia Standard Time"},
            {"Indian/Chagos", "Central Asia Standard Time"},
            {"Asia/Bishkek", "Central Asia Standard Time"},
            {"Asia/Qyzylorda", "Central Asia Standard Time"},
            {"Etc/GMT-6", "Central Asia Standard Time"},
            {"Asia/Dhaka", "Bangladesh Standard Time"},
            {"Asia/Thimphu", "Bangladesh Standard Time"},
            {"Asia/Omsk", "Omsk Standard Time"},
            {"Asia/Rangoon", "Myanmar Standard Time"},
            {"Indian/Cocos", "Myanmar Standard Time"},
            {"Antarctica/Davis", "SE Asia Standard Time"},
            {"Indian/Christmas", "SE Asia Standard Time"},
            {"Asia/Jakarta", "SE Asia Standard Time"},
            {"Asia/Pontianak", "SE Asia Standard Time"},
            {"Asia/Phnom_Penh", "SE Asia Standard Time"},
            {"Asia/Vientiane", "SE Asia Standard Time"},
            {"Asia/Bangkok", "SE Asia Standard Time"},
            {"Asia/Saigon", "SE Asia Standard Time"},
            {"Etc/GMT-7", "SE Asia Standard Time"},
            {"Asia/Barnaul", "Altai Standard Time"},
            {"Asia/Hovd", "W. Mongolia Standard Time"},
            {"Asia/Krasnoyarsk", "North Asia Standard Time"},
            {"Asia/Novokuznetsk", "North Asia Standard Time"},
            {"Asia/Novosibirsk", "N. Central Asia Standard Time"},
            {"Asia/Tomsk", "Tomsk Standard Time"},
            {"Asia/Shanghai", "China Standard Time"},
            {"Asia/Hong_Kong", "China Standard Time"},
            {"Asia/Macau", "China Standard Time"},
            {"Asia/Irkutsk", "North Asia East Standard Time"},
            {"Asia/Singapore", "Singapore Standard Time"},
            {"Asia/Brunei", "Singapore Standard Time"},
            {"Asia/Makassar", "Singapore Standard Time"},
            {"Asia/Kuala_Lumpur", "Singapore Standard Time"},
            {"Asia/Kuching", "Singapore Standard Time"},
            {"Asia/Manila", "Singapore Standard Time"},
            {"Etc/GMT-8", "Singapore Standard Time"},
            {"Australia/Perth", "W. Australia Standard Time"},
            {"Asia/Taipei", "Taipei Standard Time"},
            {"Asia/Ulaanbaatar", "Ulaanbaatar Standard Time"},
            {"Asia/Choibalsan", "Ulaanbaatar Standard Time"},
            {"Asia/Pyongyang", "North Korea Standard Time"},
            {"Australia/Eucla", "Aus Central W. Standard Time"},
            {"Asia/Chita", "Transbaikal Standard Time"},
            {"Asia/Jayapura", "Tokyo Standard Time"},
            {"Asia/Tokyo", "Tokyo Standard Time"},
            {"Pacific/Palau", "Tokyo Standard Time"},
            {"Asia/Dili", "Tokyo Standard Time"},
            {"Etc/GMT-9", "Tokyo Standard Time"},
            {"Asia/Seoul", "Korea Standard Time"},
            {"Asia/Yakutsk", "Yakutsk Standard Time"},
            {"Asia/Khandyga", "Yakutsk Standard Time"},
            {"Australia/Adelaide", "Cen. Australia Standard Time"},
            {"Australia/Broken_Hill", "Cen. Australia Standard Time"},
            {"Australia/Darwin", "AUS Central Standard Time"},
            {"Australia/Brisbane", "E. Australia Standard Time"},
            {"Australia/Lindeman", "E. Australia Standard Time"},
            {"Australia/Sydney", "AUS Eastern Standard Time"},
            {"Australia/Melbourne", "AUS Eastern Standard Time"},
            {"Antarctica/DumontDUrville", "West Pacific Standard Time"},
            {"Pacific/Truk", "West Pacific Standard Time"},
            {"Pacific/Guam", "West Pacific Standard Time"},
            {"Pacific/Saipan", "West Pacific Standard Time"},
            {"Pacific/Port_Moresby", "West Pacific Standard Time"},
            {"Etc/GMT-10", "West Pacific Standard Time"},
            {"Australia/Hobart", "Tasmania Standard Time"},
            {"Australia/Currie", "Tasmania Standard Time"},
            {"Asia/Vladivostok", "Vladivostok Standard Time"},
            {"Asia/Ust-Nera", "Vladivostok Standard Time"},
            {"Australia/Lord_Howe", "Lord Howe Standard Time"},
            {"Pacific/Bougainville", "Bougainville Standard Time"},
            {"Asia/Srednekolymsk", "Russia Time Zone 10"},
            {"Asia/Magadan", "Magadan Standard Time"},
            {"Pacific/Norfolk", "Norfolk Standard Time"},
            {"Asia/Sakhalin", "Sakhalin Standard Time"},
            {"Antarctica/Casey", "Central Pacific Standard Time"},
            {"Antarctica/Macquarie", "Central Pacific Standard Time"},
            {"Pacific/Ponape", "Central Pacific Standard Time"},
            {"Pacific/Kosrae", "Central Pacific Standard Time"},
            {"Pacific/Noumea", "Central Pacific Standard Time"},
            {"Pacific/Guadalcanal", "Central Pacific Standard Time"},
            {"Pacific/Efate", "Central Pacific Standard Time"},
            {"Etc/GMT-11", "Central Pacific Standard Time"},
            {"Asia/Kamchatka", "Russia Time Zone 11"},
            {"Asia/Anadyr", "Russia Time Zone 11"},
            {"Antarctica/McMurdo", "New Zealand Standard Time"},
            {"Pacific/Auckland", "New Zealand Standard Time"},
            {"Pacific/Tarawa", "UTC+12"},
            {"Pacific/Majuro", "UTC+12"},
            {"Pacific/Kwajalein", "UTC+12"},
            {"Pacific/Nauru", "UTC+12"},
            {"Pacific/Funafuti", "UTC+12"},
            {"Pacific/Wake", "UTC+12"},
            {"Pacific/Wallis", "UTC+12"},
            {"Etc/GMT-12", "UTC+12"},
            {"Pacific/Fiji", "Fiji Standard Time"},
            {"Pacific/Chatham", "Chatham Islands Standard Time"},
            {"Pacific/Enderbury", "UTC+13"},
            {"Pacific/Fakaofo", "UTC+13"},
            {"Etc/GMT-13", "UTC+13"},
            {"Pacific/Tongatapu", "Tonga Standard Time"},
            {"Pacific/Apia", "Samoa Standard Time"},
            {"Pacific/Kiritimati", "Line Islands Standard Time"},
            {"Etc/GMT-14", "Line Islands Standard Time"}
        }

        Dim windowsTimeZoneId = Nothing
        Dim windowsTimeZone = Nothing
        If olsonWindowsTimes.TryGetValue(olsonTimeZoneId, windowsTimeZoneId) Then
            Try
                windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId)
            Catch ex As Exception
                ' No hacemos nada
            End Try
        End If
        Return windowsTimeZone
    End Function

    Public Shared Function DateTimePeriod2String(ByVal eType As TypePeriodEnum, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime) As String
        Dim strDateTimePeriod As String = String.Empty

        strDateTimePeriod = CInt(eType) & "," & xBeginPeriod.ToString("yyyy-MM-dd HH:mm:ss") & "," & xEndPeriod.ToString("yyyy-MM-dd HH:mm:ss")

        Return strDateTimePeriod
    End Function

    Public Shared Function String2DateTime(strDateTimePeriod As String) As roDateTimePeriod
        Dim oDateTimePeriod As New roDateTimePeriod

        oDateTimePeriod.TypePeriod = TypePeriodEnum.PeriodToday
        oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
        oDateTimePeriod.EndDateTimePeriod = #12:00:00 AM#

        If strDateTimePeriod <> String.Empty Then

            Dim oPeriodParms As String() = strDateTimePeriod.Split(",")

            Dim oVal As Integer = roTypes.Any2Integer(oPeriodParms(0))
            Dim oType As TypePeriodEnum = oVal

            If oVal = -1 Then
                'No hacemos nada (Esta seleccionado día de inicio de cálculo)
                oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
                oDateTimePeriod.EndDateTimePeriod = #12:00:00 AM#
            Else

                Dim dtMonth As Date

                Select Case oType
                    Case TypePeriodEnum.PeriodOther
                        oDateTimePeriod.BeginDateTimePeriod = DateTime.ParseExact(oPeriodParms(1), "yyyy-MM-dd", Nothing)
                    Case TypePeriodEnum.PeriodTomorrow
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(1)
                    Case TypePeriodEnum.PeriodToday
                        oDateTimePeriod.BeginDateTimePeriod = Now
                    Case TypePeriodEnum.PeriodYesterday
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(-1)
                    Case TypePeriodEnum.PeriodCurrentWeek
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(1 - Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodLastWeek
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(-6 - Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodCurrentMonth
                        oDateTimePeriod.BeginDateTimePeriod = New Date(Now.Year, Now.Month, 1)
                    Case TypePeriodEnum.PeriodLastMonth
                        dtMonth = Now.AddMonths(-1)
                        oDateTimePeriod.BeginDateTimePeriod = New Date(dtMonth.Year, dtMonth.Month, 1)
                    Case TypePeriodEnum.PeriodCurrentYear
                        'Año actual
                        oDateTimePeriod.BeginDateTimePeriod = New Date(Now.Year, 1, 1)
                    Case TypePeriodEnum.PeriodNextWeek
                        'Semana siguiente
                        Dim today As DateTime = DateTime.Today
                        Dim daysUntilMonday As Integer = (CInt(DayOfWeek.Monday) - CInt(today.DayOfWeek) + 7)
                        Dim nextMonday As DateTime = today.AddDays(daysUntilMonday)

                        oDateTimePeriod.BeginDateTimePeriod = nextMonday
                    Case TypePeriodEnum.PeriodNextMonth
                        'Mes siguiente
                        Dim N = DateTime.Now.AddMonths(1)
                        Dim FirstMonthDay = New DateTime(N.Year, N.Month, 1)

                        oDateTimePeriod.BeginDateTimePeriod = FirstMonthDay
                End Select

                oDateTimePeriod.TypePeriod = oType
                If oType <> 0 Then 'TypePeriodEnum.PeriodOther Then
                    oDateTimePeriod.BeginDateTimePeriod = New DateTime(oDateTimePeriod.BeginDateTimePeriod.Year, oDateTimePeriod.BeginDateTimePeriod.Month, oDateTimePeriod.BeginDateTimePeriod.Day, 0, 0, 0)
                Else
                    oDateTimePeriod.BeginDateTimePeriod = New DateTime(oDateTimePeriod.BeginDateTimePeriod.Year, oDateTimePeriod.BeginDateTimePeriod.Month, oDateTimePeriod.BeginDateTimePeriod.Day, oDateTimePeriod.BeginDateTimePeriod.Hour, oDateTimePeriod.BeginDateTimePeriod.Minute, oDateTimePeriod.BeginDateTimePeriod.Second)
                End If
            End If
        Else
            oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
        End If

        Return oDateTimePeriod
    End Function

    Public Shared Function String2DateTimePeriod(strDateTimePeriod As String) As roDateTimePeriod
        Dim oDateTimePeriod As New roDateTimePeriod

        oDateTimePeriod.TypePeriod = TypePeriodEnum.PeriodToday
        oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
        oDateTimePeriod.EndDateTimePeriod = #12:00:00 AM#

        If strDateTimePeriod <> String.Empty Then

            Dim oPeriodParms As String() = strDateTimePeriod.Split(",")

            Dim oVal As Integer = roTypes.Any2Integer(oPeriodParms(0))
            Dim oType As TypePeriodEnum = oVal

            If oVal = -1 Then
                'No hacemos nada (Esta seleccionado día de inicio de cálculo)
                oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
                oDateTimePeriod.EndDateTimePeriod = #12:00:00 AM#
            Else

                Dim dtMonth As Date

                Select Case oType
                    Case TypePeriodEnum.PeriodOther
                        oDateTimePeriod.BeginDateTimePeriod = DateTime.ParseExact(oPeriodParms(1), "yyyy-MM-dd HH:mm:ss", Nothing)
                        oDateTimePeriod.EndDateTimePeriod = DateTime.ParseExact(oPeriodParms(2), "yyyy-MM-dd HH:mm:ss", Nothing)
                    Case TypePeriodEnum.PeriodTomorrow
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(1)
                        oDateTimePeriod.EndDateTimePeriod = Now.AddDays(1)
                    Case TypePeriodEnum.PeriodToday
                        oDateTimePeriod.BeginDateTimePeriod = Now
                        oDateTimePeriod.EndDateTimePeriod = Now
                    Case TypePeriodEnum.PeriodYesterday
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(-1)
                        oDateTimePeriod.EndDateTimePeriod = Now.AddDays(-1)
                    Case TypePeriodEnum.PeriodCurrentWeek
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(1 - Weekday(Now, vbMonday))
                        oDateTimePeriod.EndDateTimePeriod = Now.AddDays(7 - Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodLastWeek
                        oDateTimePeriod.BeginDateTimePeriod = Now.AddDays(-6 - Weekday(Now, vbMonday))
                        oDateTimePeriod.EndDateTimePeriod = Now.AddDays(-Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodCurrentMonth
                        oDateTimePeriod.BeginDateTimePeriod = New Date(Now.Year, Now.Month, 1)
                        oDateTimePeriod.EndDateTimePeriod = New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day)
                    Case TypePeriodEnum.PeriodLastMonth
                        dtMonth = Now.AddMonths(-1)
                        oDateTimePeriod.BeginDateTimePeriod = New Date(dtMonth.Year, dtMonth.Month, 1)
                        oDateTimePeriod.EndDateTimePeriod = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
                    Case TypePeriodEnum.PeriodCurrentYear
                        'Año actual
                        oDateTimePeriod.BeginDateTimePeriod = New Date(Now.Year, 1, 1)
                        oDateTimePeriod.EndDateTimePeriod = Now
                    Case TypePeriodEnum.PeriodNextWeek
                        'Semana siguiente
                        Dim today As DateTime = DateTime.Today
                        Dim daysUntilMonday As Integer = (CInt(DayOfWeek.Monday) - CInt(today.DayOfWeek) + 7)
                        Dim nextMonday As DateTime = today.AddDays(daysUntilMonday)

                        oDateTimePeriod.BeginDateTimePeriod = nextMonday
                        oDateTimePeriod.EndDateTimePeriod = nextMonday.AddDays(6)
                    Case TypePeriodEnum.PeriodNextMonth
                        'Mes siguiente
                        Dim N = DateTime.Now.AddMonths(1)
                        Dim FirstMonthDay = New DateTime(N.Year, N.Month, 1)
                        Dim LastMonthDay = New DateTime(N.Year, N.Month, DateTime.DaysInMonth(N.Year, N.Month))

                        oDateTimePeriod.BeginDateTimePeriod = FirstMonthDay
                        oDateTimePeriod.EndDateTimePeriod = LastMonthDay
                    Case TypePeriodEnum.PeriodNMonthsAgoFromDay
                        'X meses atrás desde día Y
                        Dim dAuxDate As Date = Date.ParseExact(oPeriodParms(1), "yyyy-MM-dd HH:mm:ss", Nothing)
                        Dim dMonthsAgo As Date = DateTime.Now.AddMonths(-1 * dAuxDate.Month)
                        oDateTimePeriod.BeginDateTimePeriod = DateSerial(dMonthsAgo.Year, dMonthsAgo.Month, dAuxDate.Day)
                        oDateTimePeriod.EndDateTimePeriod = oDateTimePeriod.BeginDateTimePeriod.AddMonths(1).AddDays(-1)
                End Select

                oDateTimePeriod.TypePeriod = oType
                If oType <> 0 Then 'TypePeriodEnum.PeriodOther Then
                    oDateTimePeriod.BeginDateTimePeriod = New DateTime(oDateTimePeriod.BeginDateTimePeriod.Year, oDateTimePeriod.BeginDateTimePeriod.Month, oDateTimePeriod.BeginDateTimePeriod.Day, 0, 0, 0)
                    oDateTimePeriod.EndDateTimePeriod = New DateTime(oDateTimePeriod.EndDateTimePeriod.Year, oDateTimePeriod.EndDateTimePeriod.Month, oDateTimePeriod.EndDateTimePeriod.Day, 23, 59, 59)
                Else
                    oDateTimePeriod.BeginDateTimePeriod = New DateTime(oDateTimePeriod.BeginDateTimePeriod.Year, oDateTimePeriod.BeginDateTimePeriod.Month, oDateTimePeriod.BeginDateTimePeriod.Day, oDateTimePeriod.BeginDateTimePeriod.Hour, oDateTimePeriod.BeginDateTimePeriod.Minute, oDateTimePeriod.BeginDateTimePeriod.Second)
                    oDateTimePeriod.EndDateTimePeriod = New DateTime(oDateTimePeriod.EndDateTimePeriod.Year, oDateTimePeriod.EndDateTimePeriod.Month, oDateTimePeriod.EndDateTimePeriod.Day, oDateTimePeriod.EndDateTimePeriod.Hour, oDateTimePeriod.EndDateTimePeriod.Minute, oDateTimePeriod.EndDateTimePeriod.Second)
                End If
            End If
        Else
            oDateTimePeriod.BeginDateTimePeriod = #12:00:00 AM#
            oDateTimePeriod.EndDateTimePeriod = #12:00:00 AM#
        End If

        Return oDateTimePeriod
    End Function

    Public Shared Function FixedSize(ByVal imgPhoto As Drawing.Image, ByVal Width As Integer, ByVal Height As Integer, Optional ByVal bolPercent As Boolean = True) As Drawing.Image

        Dim sourceWidth As Integer = imgPhoto.Width
        Dim sourceHeight As Integer = imgPhoto.Height
        Dim sourceX As Integer = 0
        Dim sourceY As Integer = 0
        Dim destX As Integer = 0
        Dim destY As Integer = 0

        Dim nPercent As Double = 0
        Dim nPercentW As Double = 0
        Dim nPercentH As Double = 0

        Dim destWidth As Integer
        Dim destHeight As Integer

        If bolPercent Then

            nPercentW = (CType(Width, Double) / CType(sourceWidth, Double))
            nPercentH = (CType(Height, Double) / CType(sourceHeight, Double))

            'if we have to pad the height pad both the top and the bottom
            'with the difference between the scaled height and the desired height
            If (nPercentH < nPercentW) Then
                nPercent = nPercentH
                destX = ((Width - (sourceWidth * nPercent)) / 2)
            Else
                nPercent = nPercentW
                destY = ((Height - (sourceHeight * nPercent)) / 2)
            End If

            destWidth = (sourceWidth * nPercent)
            destHeight = (sourceHeight * nPercent)
        Else

            destWidth = Width
            destHeight = Height
            destX = 0
            destY = 0

        End If

        Dim bmPhoto As Drawing.Bitmap = New Drawing.Bitmap(Width, Height, Drawing.Imaging.PixelFormat.Format24bppRgb)
        'bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution)

        Dim grPhoto As Drawing.Graphics = Drawing.Graphics.FromImage(bmPhoto)
        grPhoto.Clear(Drawing.Color.LightGray)
        'grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic

        grPhoto.DrawImage(imgPhoto,
         New Drawing.Rectangle(destX, destY, destWidth, destHeight),
         New Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
         Drawing.GraphicsUnit.Pixel)

        grPhoto.Dispose()
        Return bmPhoto

    End Function

    Public Shared Function IsLeapYear(iYear As Integer) As Boolean
        Return (iYear Mod 4 = 0 AndAlso iYear Mod 100 <> 0) OrElse iYear Mod 400 = 0
    End Function

#End Region

End Class