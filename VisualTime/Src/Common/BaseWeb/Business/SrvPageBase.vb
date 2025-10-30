Imports System.Globalization
Imports System.Threading

Public Class SrvPageBase
    Inherits NoCachePageBase

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.applyCulture()

        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)

    End Sub

#End Region

#Region "Culture Methods"

    Public Sub applyCulture()
        Me.InitializeCultures()
    End Sub

    Protected Sub InitializeCultures()

        Dim strCurrentCulture As String = WLHelperWeb.CurrentCulture

        If (Not String.IsNullOrEmpty(strCurrentCulture)) Then

            Thread.CurrentThread.CurrentCulture = New CultureInfo(strCurrentCulture)
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture

        End If

    End Sub

#End Region

End Class