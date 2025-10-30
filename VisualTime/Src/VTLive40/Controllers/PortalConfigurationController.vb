Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Robotics
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.Web.Base

<LoggedInAtrribute(Requiered:=True)>
Public Class PortalConfigurationController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    Function Index() As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")
            Try
                'GetPortalConfiguration()
                LoadViewBags()
            Catch ex As Exception
            End Try
            Return View("PortalConfiguration")
        Else
            Return View("NoSession")
        End If
    End Function

    Private Sub LoadViewBags()

        Dim positions As New List(Of roPosition)

        Dim left As New roPosition
        left.Id = 1
        left.Name = GetServerLanguage().Translate("roPortalConfigurationLeft", "")
        positions.Add(left)
        Dim center As New roPosition
        center.Id = 2
        center.Name = GetServerLanguage().Translate("roPortalConfigurationCenter", "")
        positions.Add(center)
        Dim right As New roPosition
        right.Id = 3
        right.Name = GetServerLanguage().Translate("roPortalConfigurationRight", "")
        positions.Add(right)
        Dim cover As New roPosition
        cover.Id = 4
        cover.Name = GetServerLanguage().Translate("roPortalConfigurationCover", "")
        positions.Add(cover)

        ViewBag.Positions = positions

        ViewBag.DailyRecord = HelperSession.GetFeatureIsInstalledFromApplication("Feature\DailyRecord")

        LoadGeolocalizationTypes()

        Dim oParam = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.ZoneRestrictedByIP", False)
        ViewBag.ZoneRestrictedByIP = roTypes.Any2Boolean(oParam.Value)

        Dim oParam2 = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.HeaderConfigurationEnabled", False)
        ViewBag.HeaderConfigurationEnabled = roTypes.Any2Boolean(oParam2.Value)


        Dim tgUserFields As List(Of roUserField) = API.UserFieldServiceMethods.GetUserFieldList(Nothing, UserFieldsTypes.Types.EmployeeField, True, False, False)

        Dim tgFields As New Generic.List(Of SelectField)
        If tgUserFields IsNot Nothing AndAlso tgUserFields.Count > 0 Then
            For Each userField As roUserField In tgUserFields
                If userField.Used AndAlso userField.Unique Then
                    tgFields.Add(New SelectField() With {
                        .FieldName = userField.FieldName,
                        .FieldValue = userField.Id
                                 })
                End If
            Next
        End If
        ViewBag.TimegateUserfields = tgFields

    End Sub

    Private Sub LoadGeolocalizationTypes()
        Dim geolocalizationTypes As New List(Of roGeolocalizationType)

        Dim withoutGeolocalization As New roGeolocalizationType
        withoutGeolocalization.Id = 1
        withoutGeolocalization.Name = GetServerLanguage().Translate("roPortalGeolocalizationConfigurationWithout", "PortalConfiguration")
        geolocalizationTypes.Add(withoutGeolocalization)
        Dim withGeolocalization As New roGeolocalizationType
        withGeolocalization.Id = 2
        withGeolocalization.Name = GetServerLanguage().Translate("roPortalGeolocalizationConfigurationWith", "PortalConfiguration")
        geolocalizationTypes.Add(withGeolocalization)
        Dim userGeolocalization As New roGeolocalizationType
        userGeolocalization.Id = 3
        userGeolocalization.Name = GetServerLanguage().Translate("roPortalGeolocalizationConfigurationUser", "PortalConfiguration")
        geolocalizationTypes.Add(userGeolocalization)

        ViewBag.GeolocalizationTypes = geolocalizationTypes
    End Sub

    <HttpGet>
    Public Function GetPortalConfiguration() As ActionResult
        Dim advancedParamter As roAdvancedParameter
        Dim oPortalConfiguration As roPortalConfiguration = New roPortalConfiguration()

        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.HeaderConfiguration", False)

        oPortalConfiguration.HeaderConfiguration = roJSONHelper.DeserializeNewtonSoft(advancedParamter.Value, GetType(roPortalHeaderConfiguration))

        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.GeolocalizationConfiguration", False)

        oPortalConfiguration.GeolocalizationConfiguration = roJSONHelper.DeserializeNewtonSoft(advancedParamter.Value, GetType(roPortalGeolocalizationConfiguration))

        If Not advancedParamter.Exists Then
            oPortalConfiguration.GeolocalizationConfiguration = New roPortalGeolocalizationConfiguration()
            oPortalConfiguration.GeolocalizationConfiguration.Geolocalization = 3
        End If

        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.PunchOptions", False)
        If advancedParamter.Value.Trim.Length = 0 Then
            oPortalConfiguration.PunchOptions = New roPortalPunchOptions With {.ZoneRequired = False}
        Else
            oPortalConfiguration.PunchOptions = roJSONHelper.DeserializeNewtonSoft(advancedParamter.Value, GetType(roPortalPunchOptions))
        End If


        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.DailyRecordPattern", False)

        oPortalConfiguration.DailyRecordPattern = roTypes.Any2Boolean(advancedParamter.Value)

        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.GeneralConfiguration", False)

        oPortalConfiguration.GeneralConfiguration = roJSONHelper.DeserializeNewtonSoft(advancedParamter.Value, GetType(roPortalGeneralConfiguration))

        Dim maxNumberOfDaysPastValue As Integer
        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.DailyRecord.MaxNumberOfDaysPast", False)
        If Not Integer.TryParse(roTypes.Any2String(advancedParamter.Value), maxNumberOfDaysPastValue) Then
            maxNumberOfDaysPastValue = -1
        End If

        oPortalConfiguration.DailyRecordMaxDaysOnPast = maxNumberOfDaysPastValue

        If Not advancedParamter.Exists Then
            oPortalConfiguration.GeneralConfiguration = New roPortalGeneralConfiguration()
            oPortalConfiguration.GeneralConfiguration.Impersonate = True
        End If



        Dim resultJson = JsonConvert.SerializeObject(oPortalConfiguration)

        Return Content(resultJson, "application/json")
    End Function

    <HttpPost>
    Public Function SavePortalConfiguration(ByVal Image As String, ByVal Position As String, ByVal Opacity As String, ByVal LeftColor As String, ByVal RightColor As String, ByVal Geolocalization As String, ByVal PunchZoneRequired As String, ByVal DailyRecordPattern As String, ByVal Impersonate As String, ByVal drMaxDaysOnPast As Integer) As JsonResult

        Dim advancedParamter As New roAdvancedParameter

        Dim newPortalConfiguration As New roPortalConfiguration()

        ' Eliminar el prefijo si es necesario
        Dim prefijo As String = "data:image/jpeg;base64,"
        If Image.StartsWith("data:image/jpeg;base64,") Then
            Image = Image.Replace(prefijo, "")
        ElseIf Image.StartsWith("data:image/png;base64,") Then
            prefijo = "data:image/png;base64,"
            Image = Image.Replace(prefijo, "")
        ElseIf Image.StartsWith("data:image/jpg;base64,") Then
            prefijo = "data:image/jpg;base64,"
            Image = Image.Replace(prefijo, "")
        End If
        Dim ImageBytes As Byte() = Convert.FromBase64String(Image)
        newPortalConfiguration.HeaderConfiguration.Image = prefijo & Convert.ToBase64String(roImagesHelper.ResizeImageIfNeeded(ImageBytes, 240, 0))
        newPortalConfiguration.HeaderConfiguration.Position = roTypes.Any2Integer(Position)
        newPortalConfiguration.HeaderConfiguration.Opacity = Opacity
        newPortalConfiguration.HeaderConfiguration.LeftColor = LeftColor
        newPortalConfiguration.HeaderConfiguration.RightColor = RightColor

        advancedParamter.Name = "VTPortal.HeaderConfiguration"
        advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(newPortalConfiguration.HeaderConfiguration)
        Dim hasSameValue As Boolean = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)
        Dim result = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        newPortalConfiguration.GeolocalizationConfiguration.Geolocalization = roTypes.Any2Integer(Geolocalization)

        advancedParamter.Name = "VTPortal.GeolocalizationConfiguration"
        advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(newPortalConfiguration.GeolocalizationConfiguration)

        hasSameValue = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)
        Dim result2 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        newPortalConfiguration.PunchOptions.ZoneRequired = roTypes.Any2Boolean(PunchZoneRequired)

        advancedParamter.Name = "VTPortal.PunchOptions"
        advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(newPortalConfiguration.PunchOptions)

        hasSameValue = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)
        Dim result3 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        advancedParamter.Name = "VTPortal.DailyRecordPattern"
        advancedParamter.Value = DailyRecordPattern

        hasSameValue = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)
        Dim result4 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        newPortalConfiguration.GeneralConfiguration.Impersonate = roTypes.Any2Boolean(Impersonate)

        advancedParamter.Name = "VTPortal.GeneralConfiguration"
        advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(newPortalConfiguration.GeneralConfiguration)

        hasSameValue = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)

        Dim result5 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        advancedParamter.Name = "VTPortal.DailyRecord.MaxNumberOfDaysPast"
        advancedParamter.Value = drMaxDaysOnPast.ToString

        hasSameValue = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)

        Dim result6 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        DataLayer.roCacheManager.GetInstance.UpdateParamCache()

        Return Json(result AndAlso result2 AndAlso result3 AndAlso result4 AndAlso result5)

    End Function

    <HttpPost>
    Public Function SaveTimegateConfiguration(ByVal customUserFieldEnabled As Boolean, userFieldId As Integer?, userFieldName As String, ByVal image As String, ByVal position As String, ByVal opacity As String, ByVal leftColor As String, ByVal rightColor As String) As JsonResult

        Dim timegateConfiguration As New TimegateConfiguration With {
            .CustomUserFieldEnabled = customUserFieldEnabled,
            .UserFieldId = IIf(customUserFieldEnabled, userFieldId, -1)
            }

        ' Comprobamos si el campo seleccionado puede ser único en base a los valores ya asignados.
        Dim saveTgConf As Boolean = True

        If customUserFieldEnabled Then
            saveTgConf = API.UserFieldServiceMethods.SetUniqueConstraintToUserField(Nothing, userFieldName, userFieldId, False)
        End If


        If saveTgConf Then
            Dim advancedParamter As New roAdvancedParameter
            advancedParamter.Name = "Timegate.Identification.CustomUserFieldId"
            advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(timegateConfiguration)
            Dim hasSameValue As Boolean = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value)
            saveTgConf = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)
        Else
            Dim warningMessage = GetServerLanguage().Translate("timegateconfiguration.noneligibleuniqueuserfield", "")
            Return Json(warningMessage)
        End If

        Dim advancedParamterTGBackground As New roAdvancedParameter

        Dim newTimeGateBackground As New TimeGateBackgroundConfiguration()

        ' Eliminar el prefijo si es necesario
        If image IsNot Nothing Then
            Dim prefix As String = "data:image/jpeg;base64,"
            If image.StartsWith("data:image/jpeg;base64,") Then
                image = image.Replace(prefix, "")
            ElseIf image.StartsWith("data:image/png;base64,") Then
                prefix = "data:image/png;base64,"
                image = image.Replace(prefix, "")
            ElseIf image.StartsWith("data:image/jpg;base64,") Then
                prefix = "data:image/jpg;base64,"
                image = image.Replace(prefix, "")
            End If
            Dim imageBytes As Byte() = Convert.FromBase64String(image)
            newTimeGateBackground.Image = prefix & Convert.ToBase64String(roImagesHelper.ResizeImageIfNeeded(imageBytes, 1500, 0))
        End If
        If position IsNot Nothing Then
            newTimeGateBackground.Position = roTypes.Any2Integer(position)
        End If
        If opacity IsNot Nothing Then
            newTimeGateBackground.Opacity = opacity
        End If
        If leftColor IsNot Nothing Then
            newTimeGateBackground.LeftColor = leftColor
        End If
        If rightColor IsNot Nothing Then
            newTimeGateBackground.RightColor = rightColor
        End If
        advancedParamterTGBackground.Name = "Timegate.Configuration.Background"
        advancedParamterTGBackground.Value = roJSONHelper.SerializeNewtonSoft(newTimeGateBackground)
        Dim hasSameBackground As Boolean = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamterTGBackground.Name, False).Value.Equals(advancedParamterTGBackground.Value)
        Dim result2 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamterTGBackground, Not hasSameBackground)
        Return Json(saveTgConf AndAlso result2)
    End Function

    <HttpGet>
    Public Function GetTimegateConfiguration() As ActionResult

        Dim tgConf As TimegateConfiguration
        Dim advancedParamter As New roAdvancedParameter
        advancedParamter.Name = "Timegate.Identification.CustomUserFieldId"
        tgConf = roJSONHelper.DeserializeNewtonSoft(API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value, GetType(TimegateConfiguration))

        Dim timeGateBackgroundConfiguration As TimeGateBackgroundConfiguration = New TimeGateBackgroundConfiguration()

        advancedParamter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "Timegate.Configuration.Background", False)

        timeGateBackgroundConfiguration = roJSONHelper.DeserializeNewtonSoft(advancedParamter.Value, GetType(TimeGateBackgroundConfiguration))

        If tgConf Is Nothing Then

            tgConf = New TimegateConfiguration With {
            .CustomUserFieldEnabled = False,
            .UserFieldId = -1,
            .BackgroundConfiguration = timeGateBackgroundConfiguration
}
        Else
            tgConf.BackgroundConfiguration = timeGateBackgroundConfiguration
        End If

        Return Content(JsonConvert.SerializeObject(tgConf), "application/json")

    End Function

    <HttpPost>
    Public Function RestorePortalConfiguration() As JsonResult

        Dim advancedParamter As New roAdvancedParameter

        advancedParamter.Name = "VTPortal.HeaderConfiguration"
        advancedParamter.Value = ""

        Dim result = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, False)

        advancedParamter = New roAdvancedParameter

        advancedParamter.Name = "VTPortal.GeneralConfiguration"
        'Añadimos valor por defecto a true
        Dim generalConfiguration As New roPortalGeneralConfiguration
        generalConfiguration.Impersonate = True
        advancedParamter.Value = roJSONHelper.SerializeNewtonSoft(generalConfiguration)

        Dim hasSameValue As Boolean = API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals(advancedParamter.Value) Or API.CommonServiceMethods.GetAdvancedParameter(Nothing, advancedParamter.Name, False).Value.Equals("")

        Dim result2 = API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParamter, Not hasSameValue)

        If result AndAlso result2 Then
            Return Json(True)
        Else
            Return Json(False)
        End If

    End Function

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveGUI")
        End If
        Return Me.oLanguage
    End Function

End Class