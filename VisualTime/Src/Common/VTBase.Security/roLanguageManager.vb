Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security.Base
Imports Robotics.VTBase

<Serializable()>
<DataContract>
Public Class roLanguageManager

#Region "Declarations - Constructor"

    Private oState As roPassportState = Nothing

    Public ReadOnly Property State As roPassportState
        Get
            Return oState
        End Get
    End Property

    Public Sub New()
        Me.oState = New roPassportState()
    End Sub

    Public Sub New(ByVal state As roPassportState)
        Me.oState = state
    End Sub

#End Region

#Region "Methods"

    Public Function LoadById(ByVal id As Integer) As roPassportLanguage
        Dim oLang As roPassportLanguage = Nothing
        Try

            oLang = LoadData(roCacheManager.GetInstance().GetLocaleById(id))

        Catch ex As DbException
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLanguage::LoadById::" & ex.Message, ex)
        End Try
        Return oLang
    End Function

    Public Function LoadByKey(key As String) As roPassportLanguage
        Dim oLang As roPassportLanguage = Nothing
        Try

            oLang = LoadData(roCacheManager.GetInstance().GetLocaleByKey(key))

        Catch ex As DbException
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLanguage::LoadById::" & ex.Message, ex)
        End Try
        Return oLang
    End Function

    Public Function LoadLanguages() As roPassportLanguage()
        Dim oLang As New Generic.List(Of roPassportLanguage)
        Try

            Dim strSQL As String = $"@SELECT# * FROM sysroLanguages"

            Dim oLangDT As roAzureLocale() = roCacheManager.GetInstance().GetLocales()
            If oLangDT IsNot Nothing AndAlso oLangDT.Length > 0 Then

                For Each oRow As roAzureLocale In oLangDT
                    oLang.Add(LoadData(oRow))
                Next
            End If
        Catch ex As DbException
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLanguage::LoadById::" & ex.Message, ex)
        End Try
        Return oLang.OrderBy(Function(lang) lang.ID).ToArray
    End Function

#End Region

#Region "Private"
    Private Function LoadData(oRow As roAzureLocale) As roPassportLanguage
        Dim oLang As roPassportLanguage
        If oRow IsNot Nothing Then
            oLang = New roPassportLanguage() With {
                .ID = oRow.id,
                .Key = oRow.key,
                .Culture = oRow.culture,
                .ParametersXml = oRow.parameters
            }
            Dim tmpLang As New VTBase.roLanguage
            tmpLang.SetLanguageReference("LivePortal", oLang.Key)
            oLang.Installed = roTypes.Any2Boolean(tmpLang.Translate("installed", ""))
        Else
            oLang = New roPassportLanguage()
        End If

        Return oLang
    End Function


#End Region

End Class