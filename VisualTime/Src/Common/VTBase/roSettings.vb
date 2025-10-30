Imports System.Runtime.Serialization
Imports Microsoft.Win32
Imports Robotics.Base.DTOs

<DataContract()>
Public Class roSettings


    Public Structure sSettingKey
        ' Estructura de la Key que se guardará en el array de constantes
        Dim Key As eKeys ' Clave que vincula la enumeración con la ruta
        Dim ApplicationRegSection As String
        Dim RegSection As String
        Dim RegKey As String
        Dim DefaultValue As Object
    End Structure

    Dim oConstArray As ArrayList

    Public Sub New(Optional ByVal _ApplicationRegSection As String = "")
        ' Creo el array de constantes de registro
        oConstArray = New ArrayList

        ' AÑADIR TODAS LAS NUEVAS CLAVES AQUÍ
        AddConst(eKeys.Language, "Paths", "Language", "ESP")
        AddConst(eKeys.System, "Paths", "System", "")
        AddConst(eKeys.Config, "Paths", "Config", "")
        AddConst(eKeys.Reports, "Paths", "ReportsCR", "")
        AddConst(eKeys.Readings, "Paths", "Readings", "")
        AddConst(eKeys.Logs, "Paths", "Logs", "")
        AddConst(eKeys.LogLevel, "Server", "LogLevel", "0")
        AddConst(eKeys.MaxLogDays, "Server", "LogMaxDays", "7")
        AddConst(eKeys.System, "Paths", "Root", "")
        AddConst(eKeys.DefaultLanguage, "", "DefaultLanguage", "ESP")
        AddConst(eKeys.AutomaticBeginJob, "", "AutomaticBeginJob", "False")
        AddConst(eKeys.AuditDays, "Server", "AuditDays", "31")
        AddConst(eKeys.DatabaseServer, "Server", "DatabaseServer", "(local)")
        AddConst(eKeys.Database, "Server", "Database", "VisualTime")
        AddConst(eKeys.DatabaseUser, "Server", "DatabaseUser", "sa")
        AddConst(eKeys.DatabasePwd, "Server", "DatabasePwd", "")
        AddConst(eKeys.ShowTotalPieces, "", "ShowTotalPieces", "False")
        AddConst(eKeys.AutomaticPreparation, "", "AutomaticPreparation", "False")
        AddConst(eKeys.AutomaticFinishJob, "", "AutomaticFinishJob", "False")
        AddConst(eKeys.TimeFormat, "", "TimeFormat", "1")
        AddConst(eKeys.Running, "Server", "Running", "False")
        AddConst(eKeys.PunchPeriodRTIn, "", "PunchPeriodRTIn", 0)
        AddConst(eKeys.PunchPeriodRTOut, "", "PunchPeriodRTOut", 0)
        AddConst(eKeys.PathProcesses, "Paths", "Processes", "")
        AddConst(eKeys.DataLink, "Paths", "DataLink", "")
        AddConst(eKeys.Documents, "Paths", "Documents", "")

        AddConst(eKeys.DatabaseFailover, "Server", "DatabaseFailover", "")

        AddConst(eKeys.DatabaseEnabledPolling, "Server", "DatabaseEnabledPolling", "")
        AddConst(eKeys.DatabaseEnabledPooling, "Server", "DatabaseEnabledPooling", "")
        AddConst(eKeys.DatabaseMaxPoolSize, "Server", "DatabaseMaxPoolSize", "")

        AddConst(eKeys.PathRoot, "Server", "SitePath", "")
    End Sub

    Public Function GetKeys() As ArrayList
        Return oConstArray
    End Function

    Private Sub AddConst(ByVal Key As eKeys, ByVal RegSection As String, ByVal RegKey As String, ByVal DefaultValue As Object)
        ' Crea una nueva entrada en el array de constantes de registro
        Dim oSettingKey As New sSettingKey

        ' Creo el objeto y establezco sus valores
        oSettingKey.Key = Key
        oSettingKey.RegSection = RegSection
        oSettingKey.RegKey = RegKey
        oSettingKey.DefaultValue = DefaultValue

        oConstArray.Add(oSettingKey) ' Añado el nuevo objeto al array
    End Sub

    Public Function GetVTSetting(ByVal key As eKeys) As Object

        ' Funcion que recupera la clave del registro del sistema
        Dim oSettingKey As sSettingKey

        oSettingKey = GetSettingKeyFromArray(key)
        Return oSettingKey.DefaultValue

    End Function

    Public Function SetVTSetting(ByVal key As eKeys, ByVal Value As String) As Boolean
        ' Función que guarda el valor de una clave en registro
        Dim oSettingKey As sSettingKey

        oSettingKey = GetSettingKeyFromArray(key)
        Return True
    End Function

    Private Function GetSettingKeyFromArray(ByVal Key As eKeys) As sSettingKey
        ' Función que busca en los elementos del array de constantes el que tenga el key pasado por parámetro.
        Dim oSettingKey As sSettingKey

        For Each oSettingKey In oConstArray
            If oSettingKey.Key = Key Then
                Return oSettingKey ' Encontrado y salgo de la funcion
            End If
        Next

        ' Como no he encontrado nada inicializo la variable para devolverla en blanco
        oSettingKey = New sSettingKey
        oSettingKey.RegSection = ""
        oSettingKey.RegKey = ""

        Return oSettingKey ' Si no he encontrado nada devuelvo un blanco
    End Function

End Class