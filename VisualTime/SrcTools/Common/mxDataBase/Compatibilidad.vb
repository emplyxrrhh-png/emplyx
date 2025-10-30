Imports System.IO

Public Class clsSystemControl

    Public Shared Sub ControlException(ByVal ex As Exception)

    End Sub

End Class

Public Module mdPublic
    Public Const cfg_id = "id"
    Public Const cfg_autodaylight = "autodaylight"
    Public Const cfg_language = "language"
    Public Const cfg_loglevel = "loglevel"
    Public Const cfg_takephoto = "takephoto"
    Public Const cfg_FingerVerification = "bioverif"
    Public Const cfg_timezone = "timezone"
    Public Const cfg_pwdadmin = "pwdadmin"
    Public Const cfg_pwdbio = "pwdbio"
    Public Const cfg_pwdTechnical = "pwdtechnical"
    Public Const cfg_volumen = "volumen"
    Public Const cfg_defactiveinputs = "defactiveinputs"
    Public Const cfg_defaultpunchmode = "defaultpunchmode"
    Public Const cfg_quickPunch = "quickpunch"
    Public Const cfg_interactivepunch = "interactivepunch"
    Public Const cfg_defaultquickpunch = "defaultquickpunch"
    Public Const cfg_quickPunchModes = "quickpunchmodes"
    Public Const cfg_quickPunchTimeOut = "quickpunchtimeout"
    Public Const cfg_cardtype = "cardtype"
    Public Const cfg_dbversion = "dbversion"
    Public Const cfg_updlog = "udplog"
    Public Const cfg_timetoreboot = "timetoreboot"
    Public Const cfg_biomatch = "biomatch"
    Public Const cfg_rdr1duration = "rdr1duration"
    Public Const cfg_rdr1mode = "rdr1mode"
    Public Const cfg_rdr2duration = "rdr2duration"
    Public Const cfg_rdr2mode = "rdr2mode"

End Module

Public Class clsDebug

    ''' <summary>
    ''' Inicializa las variables por defecto y arranca el hilo para guardar en fichero el log
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    Public Sub Print(ByVal Message As String)
        clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "Debug::" + Message)
    End Sub


    ''' <summary>
    ''' Funciona para guarda en fichero cada 30seg
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveLog()
    End Sub


    ''' <summary>
    ''' Guarda en fichero el log que esta en memoria
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveLog2File(Optional ByVal Backup As Boolean = False)

    End Sub
End Class

Public Class clsSystemHelper

    Public Const AppDirName = "mx7+"
    Private Shared mPathDatabase As String = ""
    Public Const SDPath As String = "\Storage Card\"
    Public Const SDAppPath As String = SDPath + AppDirName + "\"


    Public Shared Function PathDatabaseFilePhoto() As String
        Return IO.Path.Combine(PathDatabase, "mx7photo.DB")
    End Function

    Public Shared Function PathEmployeePhoto() As String
        Return IO.Path.Combine(PathDatabase, "mx7photo.DB")
    End Function

    ''' <summary>
    ''' Devuelve la ruta donde se encuentran las bases de datos
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function PathDatabase() As String

        'Si aun no la hemos usado lo genera

        If mPathDatabase = "" Then

            'Pruebas, guarda bbdd en memoria interna
            mPathDatabase = Path.Combine(SDAppPath, "database")

            'Si el directorio no exite lo genera
            IO.Directory.CreateDirectory(mPathDatabase)

        End If

        'Devuelve el path
        Return mPathDatabase

    End Function
End Class

Public Class clsDisplay
    Public Shared Sub Statustext(ByVal text As String)

    End Sub
End Class

Public Class SystemText
    Public Shared Function getText(ByVal Text As String, ByVal Value As String)
        Return ""
    End Function
End Class

Public Class oa3000API
    Public Const HasAPIWatchDogActive = False
    Public Shared Sub ActiveteAPIWatchDog(ByVal Active As Boolean)

    End Sub
End Class

Public Class clsInputs

    Public Enum eTypeInput
        Nul = 0
        Card = 1
        Bio = 2
        Keyboard = 3
        Scanner = 4
    End Enum
End Class

Public Class clsCardReader

    Public Enum ReaderType
        Hex = 0
        Numeric = 1
        Robotics = 2
    End Enum

    Public Shared Function ConvertCard(ByVal Card As String) As String
        Try

            'Si esta el lector activo devolvemos la ultima tarjeta
            Select Case clsSysConfig.CardType
                Case ReaderType.Numeric
                    Return Long.Parse(Card, Globalization.NumberStyles.HexNumber)
                Case ReaderType.Robotics
                    Return LngCard2VTCard(Card)
                Case ReaderType.Hex
                    Return Card
                Case Else
                    Return LngCard2VTCard(Card)
            End Select

        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardReader::Process:", ex)
            Return ""
        End Try
    End Function

    Public Shared Function LngCard2VTCard(ByVal IDCard As String) As String
        Dim sIDCard As String = ""
        Dim tmp As String = IDCard
        Try

            While tmp.Length > 0
                If Convert.ToInt16(tmp.Substring(0, 1), 16) > 0 Or sIDCard.Length > 0 Then
                    sIDCard += IIf(Convert.ToInt16(tmp.Substring(0, 1), 16) > 9 Or sIDCard.Length = 0, "", "0") + Convert.ToInt16(tmp.Substring(0, 1), 16).ToString
                End If
                tmp = tmp.Substring(1)

            End While

        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGlobal::LngCard2VTCard::Error:(IDCard:" + IDCard.ToString + "):", ex)
        End Try
        Return sIDCard

    End Function
End Class

Public Class clsSysConfig

    Private Shared mCardType As clsCardReader.ReaderType = clsCardReader.ReaderType.Robotics
    Public Shared PreventRepeatedOfflinePunchesPeriod As Integer

    Public Shared Property CardType() As clsCardReader.ReaderType
        Get
            Return mCardType
        End Get
        Set(ByVal value As clsCardReader.ReaderType)
            mCardType = value
        End Set
    End Property

End Class