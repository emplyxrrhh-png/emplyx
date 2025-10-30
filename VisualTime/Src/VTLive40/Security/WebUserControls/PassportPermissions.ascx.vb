Imports Robotics.Base.DTOs
Imports Robotics.UsersAdmin
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

Partial Class Security_WebUserControls_PassportPermissions
    Inherits UserControlBase

    Private oUserAdmSecurityPermission As Permission

#Region "Properties"

    Public WriteOnly Property TableHeight() As String
        Set(ByVal value As String)
            Me.divPassportPermissionsTable.Style.Add("height", value)
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim cacheControl As New NoCachePageBase
        cacheControl.InsertExtraJavascript("Features", "~/Security/Scripts/Features.js", Me.Page)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        oUserAdmSecurityPermission = Me.GetFeaturePermission("Administration.Security")
    End Sub

#End Region

#Region "Methods"

    Public Sub LoadData(ByVal _IDPassport As Integer, ByVal _PermissionsType As String)

        Me.divPassportPermissionsTable.Controls.Add(Me.CreateFeaturesTable(_IDPassport, _PermissionsType))

    End Sub

    Private Function CreateFeaturesTable(ByVal idPassport As Integer, ByVal type As String) As HtmlTable

        Dim hTable As New HtmlTable
        Dim hTRow As HtmlTableRow
        Dim hTCell As HtmlTableCell

        With hTable
            .Border = 0
            .CellPadding = 0
            .CellSpacing = 0
            .Attributes("class") = "FeaturesTableStyle GridFeatures"
        End With

        ' Añadimos fila nombres columnas
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader FeaturesTableStyle-cellheader-noend"
        'hTCell.Attributes("style") = "border-right: 0;"
        hTCell.InnerHtml = Me.Language.Translate("Functionalties.Columns.Name", Me.DefaultScope) ' "Funcionalidad"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader"
        hTCell.Attributes("style") = "text-align: right;"
        hTCell.InnerHtml = Me.Language.Translate("Functionalties.Columns.Permission", Me.DefaultScope) '"Permiso"
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim oFeaturesAll() As Feature = Nothing

        Dim oFeatureRows As Generic.List(Of HtmlTableRow) = Me.GetFeatures(oFeaturesAll, idPassport, Nothing, type, 1)
        If oFeatureRows IsNot Nothing Then
            For Each oRow As HtmlTableRow In oFeatureRows
                hTable.Rows.Add(oRow)
            Next
        End If

        Return hTable

    End Function

    Private Function GetFeatures(ByVal oFeaturesAll() As Feature, ByVal idPassport As Integer, ByVal idParentFeature As Nullable(Of Integer), ByVal type As String, ByVal intLevel As Integer) As Generic.List(Of HtmlTableRow)

        Dim oFeatureRows As New Generic.List(Of HtmlTableRow)
        Dim oLicSupport As New roServerLicense()
        Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

        If oFeaturesAll Is Nothing Then
            ' Obtenemos la lista de todas la funcionalidades
            oFeaturesAll = API.UserAdminServiceMethods.GetFeaturesFromPassportAll(Me.Page, idPassport, type)
        End If

        If oFeaturesAll IsNot Nothing Then

            ' Obtenemos las funcionalidades del nivel actual (idParentfeature)
            Dim oFeatures As New Generic.List(Of Feature)
            For Each oItem As Feature In oFeaturesAll

                If Not idParentFeature.HasValue Then
                    If Not oItem.IDParent.HasValue Then oFeatures.Add(oItem)
                Else
                    If oItem.IDParent.HasValue AndAlso oItem.IDParent = idParentFeature Then

                        If oItem.Alias = "Punches.DailyRecord" AndAlso dailyRecordInstalled = False Then
                        Else
                            oFeatures.Add(oItem)
                        End If

                    End If
                End If
            Next

            Dim hTRow As HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim divFeature As HtmlGenericControl
            Dim divAnchorFeature As HtmlGenericControl
            Dim divOpenFeature As HtmlGenericControl
            Dim divInfoFeature As HtmlGenericControl
            Dim aAnchor As HtmlAnchor
            Dim aAnchorInfo As HtmlAnchor
            Dim iHtmlImg As HtmlImage

            ' Obtenemos traducción tooltip botón mostrar información
            Dim strFeatureInformationButton As String = Me.Language.Translate("Feature.Information.Button", Me.DefaultScope)

            ' En el caso que estemos en seguridad v2
            ' debemos dejar mostrar las funcionalidades de tipo E con el maximo de permisos
            ' si tener en cuenta el grupo de usuario
            Dim intMaxConfigurable As Integer = -1
            intMaxConfigurable = 9

            For Each oFeature As Feature In oFeatures
                If oFeature.Type = "E" Then
                    If intMaxConfigurable = 9 Then
                        oFeature.MaxConfigurable = 9
                    End If
                End If

                If oFeature.MaxConfigurable > Permission.None Then

                    ' Pinta la fila con el nombre de la categoría actual y sus permisos
                    hTRow = New HtmlTableRow
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " " & "FeaturesTableStyle-noendcellLevel" & intLevel

                    aAnchor = New HtmlAnchor
                    With aAnchor
                        .HRef = "javascript:void(0);"
                        .Style("width") = "100%"
                        .Attributes("class") = "FeatureAnchor"
                        If oFeature.IsGroup Then
                            .Attributes("onclick") = "ShowHideFeatureChilds('" & oFeature.ID & "', '" & Me.ClientID & "_');"
                        Else
                            .Style("cursor") = "default"
                        End If
                        .InnerHtml = oFeature.Name ' Me.Language.Translate("Feature." & oFeature.Alias & ".Name", Me.DefaultScope)
                    End With

                    iHtmlImg = New HtmlImage
                    With iHtmlImg
                        .Alt = ""
                        .ID = "aFeatureOpenImg" & oFeature.ID
                        If oFeature.IsGroup Then
                            .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/tree/elbow-minus-nl.gif")
                            .Attributes("onclick") = "ShowHideFeatureChilds('" & oFeature.ID & "', '" & Me.ClientID & "_');"
                            .Style("cursor") = "pointer"
                        Else
                            .Style("min-width") = "16px"
                            .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/s.gif")
                        End If
                    End With

                    divFeature = New HtmlGenericControl("div")
                    divFeature.Style("width") = "100%"

                    divOpenFeature = New HtmlGenericControl("div")
                    divOpenFeature.Style("float") = "left"
                    divOpenFeature.Style("text-align") = "left"
                    divOpenFeature.Controls.Add(iHtmlImg)

                    divAnchorFeature = New HtmlGenericControl("div")
                    divAnchorFeature.Style("float") = "left"
                    divAnchorFeature.Style("text-align") = "left"
                    divAnchorFeature.Style("margin-top") = "2px"
                    divAnchorFeature.Controls.Add(aAnchor)

                    divInfoFeature = New HtmlGenericControl("div")
                    divInfoFeature.Style("float") = "right"
                    divInfoFeature.Style("text-align") = "right"
                    aAnchorInfo = New HtmlAnchor
                    With aAnchorInfo
                        .ID = "aFeatureInfo" & oFeature.ID
                        .HRef = "javascript:void(0);"
                        .Attributes("class") = "FeatureInfoAnchor"
                        .Attributes("onclick") = "ShowHideFeatureInfo(this, '" & oFeature.ID & "', '" & Me.ClientID & "_');"
                        .Title = strFeatureInformationButton
                    End With
                    divInfoFeature.Controls.Add(aAnchorInfo)

                    divFeature.Controls.Add(divOpenFeature)
                    divFeature.Controls.Add(divAnchorFeature)
                    divFeature.Controls.Add(divInfoFeature)
                    hTCell.Controls.Add(divFeature)

                    hTRow.Cells.Add(hTCell)
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " FeaturesTableStyle-cellPermissions"
                    hTCell.Attributes("align") = "right"
                    hTCell.Style("padding") = "3px"
                    hTCell.Controls.Add(Me.CreateFeaturePermissionsTable(idPassport, oFeature))

                    If oUserAdmSecurityPermission <= Permission.Read Then
                        hTCell.Attributes("class") &= " disabled"
                    End If
                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                    ' Pinta fila con la información de la funcionalidad
                    hTRow = New HtmlTableRow
                    hTRow.ID = "rowFeatureInfo" & oFeature.ID
                    hTRow.Style("display") = "none"
                    hTCell = New HtmlTableCell
                    hTCell.Style("padding-left") = (5 * 10) & "px"
                    hTCell.Controls.Add(Me.CreateFeatureInfoTable(oFeature))
                    hTRow.Cells.Add(hTCell)
                    hTCell = New HtmlTableCell
                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                End If

                If oFeature.IsGroup Then

                    Dim hChildsTable As New HtmlTable
                    With hChildsTable
                        .ID = "tbFeatureChilds" & oFeature.ID
                        .Border = 0
                        .CellPadding = 0
                        .CellSpacing = 0
                        .Attributes("class") = "FeaturesTableStyle GridFeatureChilds"
                    End With
                    Dim oChildFeatureRows As Generic.List(Of HtmlTableRow) = Me.GetFeatures(oFeaturesAll, idPassport, oFeature.ID, type, intLevel + 1)
                    If oChildFeatureRows IsNot Nothing Then
                        For Each oRow As HtmlTableRow In oChildFeatureRows
                            hChildsTable.Rows.Add(oRow)
                        Next
                    End If

                    hTRow = New HtmlTableRow
                    hTRow.ID = "rowFeatureChilds" & oFeature.ID
                    hTCell = New HtmlTableCell
                    hTCell.ColSpan = 2
                    hTCell.Style("padding-left") = "15px" '(intLevel * 10) & "px"
                    hTCell.Controls.Add(hChildsTable)
                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                End If

            Next

        End If

        Return oFeatureRows

    End Function

    Private Function CreateFeaturePermissionsTable(ByVal idPassport As Integer, ByVal oFeature As Feature) As HtmlTable

        Dim oRet As New HtmlTable

        ' Obtenemos traducciones tooltis botones
        Static strSetDefaultPermissionButton As String = Me.Language.Translate("SetDefaultPermission.Button", Me.DefaultScope)
        Static strPermissionOverEmployeesButton As String = Me.Language.Translate("PermissionOverEmployees.Button", Me.DefaultScope)

        Dim oFeaturePermissions() As Permission = oFeature.Permissions.ToArray ' API.UserAdminServiceMethods.GetFeaturePermissions(Me, idPassport, oFeature.ID)
        If oFeaturePermissions IsNot Nothing Then

            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim aPermission As HtmlAnchor

            With oRet
                .Border = 0
                .CellPadding = 0
                .CellSpacing = 0
                .Attributes("class") = "GridStFeaturesTableStyleyle GridPermissions"
                .Align = "right"
            End With

            For Each oPermission As Permission In oFeaturePermissions

                hTCell = New HtmlTableCell

                aPermission = New HtmlAnchor
                aPermission.ID = "aFeaturePermission" & oPermission.ToString & "_" & oFeature.ID
                aPermission.HRef = "javascript:void(0);"
                aPermission.Attributes("class") = "Permission" & oPermission.ToString
                If oFeature.EditedValue = oFeature.InheritedValue Then
                    aPermission.Attributes("class") &= " Permission" & oPermission.ToString & "Inherited"
                End If
                If oFeature.EditedValue = CInt(oPermission) Then
                    aPermission.Attributes("class") &= " PermissionPressed"
                Else
                    aPermission.Attributes("class") &= " PermissionUnPressed"
                End If

                If oUserAdmSecurityPermission > Permission.Read Then
                    aPermission.Attributes("onclick") = "UpdFeaturePermission(this, '" & idPassport & "', '" & oFeature.ID & "', '" & oFeature.Type & "', '" & oPermission.ToString & "', '" & Me.ClientID & "_', '" & Me.ResolveUrl("~/Security") & "/');"
                End If
                hTCell.Controls.Add(aPermission)
                hTRow.Cells.Add(hTCell)

            Next

            hTCell = New HtmlTableCell
            Dim aSetDefault As New HtmlAnchor
            With aSetDefault
                .HRef = "javascript:void(0);"
                .Attributes("class") = "PermissionDefault"
                .Title = strSetDefaultPermissionButton

                If oUserAdmSecurityPermission > Permission.Read Then
                    .Attributes("onclick") = "UpdDefaultFeaturePermission(this, '" & idPassport & "', '" & oFeature.ID & "', '" & oFeature.Type & "', '" & Me.ClientID & "_', '" & Me.ResolveUrl("~/Security") & "/');"
                End If
            End With
            hTCell.Controls.Add(aSetDefault)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            Dim aEmployees As New HtmlAnchor
            With aEmployees
                .HRef = "javascript:void(0);"
                .Attributes("class") = "PermissionEmployeesButton"
                If oFeature.AppHasPermissionsOverEmployees Then
                    .Attributes.Add("onclick", "ShowEmployeePermissions('" & idPassport & "', '" & oFeature.ID & "', '" & oFeature.Name & "');")
                Else
                    .Attributes("class") &= " PermissionEmployeesButtonDisabled"
                End If
                .Title = strPermissionOverEmployeesButton
            End With
            hTCell.Controls.Add(aEmployees)
            hTRow.Cells.Add(hTCell)

            oRet.Rows.Add(hTRow)

        End If

        Return oRet

    End Function

    Private Function CreateFeatureInfoTable(ByVal oFeature As Feature) As HtmlTable

        Dim hInfo As New HtmlTable

        With hInfo
            .ID = "tbFeatureInfo" & oFeature.ID
            .Border = 0
            .CellPadding = 0
            .CellSpacing = 0
            .Attributes("class") = "FeatureInfoTable"
            '.Style("display") = "none"
        End With
        Dim hTRow As New HtmlTableRow
        Dim hTCell As New HtmlTableCell
        hTCell.InnerHtml = oFeature.Description
        hTRow.Cells.Add(hTCell)

        hInfo.Rows.Add(hTRow)

        Return hInfo

    End Function

#End Region

End Class