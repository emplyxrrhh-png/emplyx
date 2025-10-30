Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class ExportsSelectorData
    Inherits PageBase

#Region "Declarations"

    Private strAction As String = "TreeData"

    Private intIDParent As Nullable(Of Integer) = Nothing
    Private strIconExportASCII As String = "ExportsASCII_16.png"
    Private strIconExportExcel As String = "ExportsExcel_16.png"
    Private strIconExportXML As String = "ExportsXML_16.png"
    Private strIconExportDirect As String = "ExportDirect_16.png"

    Private strArrayNodes As String = ""

    Private bolOnlyGroups As Boolean = False
    Private bolMultiSelect As Boolean = True
    Private strImagesPath As String = ""

    Private bolFilterConceptEmployee As Boolean = True

    Private lstSelection As New ArrayList

    Private wSepChar As String = Chr(94) & Chr(124) & Chr(94)

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Controls.Clear()

        ' Obtengo el parámetro de la acción a realizar
        Dim Action As String = Request.Params("action")
        If Action IsNot Nothing Then
            Me.strAction = Action
        End If

        ' Lectura parámetros página
        Dim OnlyGroups As String = Request.Params("OnlyGroups")
        If OnlyGroups IsNot Nothing AndAlso OnlyGroups.Length = 1 Then
            Me.bolOnlyGroups = (OnlyGroups = "1")
        End If

        Dim MultiSelect As String = Request.Params("MultiSelect")
        If MultiSelect IsNot Nothing AndAlso MultiSelect.Length = 1 Then
            Me.bolMultiSelect = (MultiSelect = "1")
        End If

        Dim ImagesPath As String = Request.Params("ImagesPath")
        If ImagesPath IsNot Nothing Then
            Me.strImagesPath = ImagesPath
        Else
            Me.strImagesPath = "../../images/ExportsSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        If Me.Request("node") IsNot Nothing AndAlso Me.Request("node") <> "source" AndAlso Me.Request("node") <> "" AndAlso IsNumeric(Me.Request("node")) Then
            Me.intIDParent = CInt(Me.Request("node"))
        End If

        Dim Filters As String = Request.Params("Filters")
        If Filters <> "" Then
            If Filters.Substring(0, 1) = "1" Then Me.bolFilterConceptEmployee = True Else Me.bolFilterConceptEmployee = False
        End If

        If Session("ExportsSelector_Selection") IsNot Nothing Then
            Dim strSelection As String = Session("ExportsSelector_Selection")
            If strSelection <> "" Then
                For Each s As String In strSelection.Split(",")
                    Me.lstSelection.Add(s.Trim)
                Next
            Else
                Me.lstSelection.Clear()
            End If
        End If

        Select Case Me.strAction
            Case "TreeData" ' Obtiene los nodos del árbol del nivel indicado (strParent)
                LoadExportsTreeStdXML()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write(Me.strArrayNodes)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    'SI ES UNA LIVE STANDARD
    Private Sub LoadExportsTreeStdXML()

        Dim strIco As String = strIconExportXML
        Dim strDesc As String = Me.Language.Translate("ExportStandardXML", DefaultScope)

        If Robotics.VTBase.roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CheckLicenseLimits")) Then
            If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then
                If GetFeaturePermission("Employees.DataLink.Exports.StdEmployees") >= Permission.Read Then
                    strArrayNodes &= "{ 'id':'0', 'text':'" & strDesc.Replace("'", "&#39;") & "', " &
                                            "'leaf': true, " &
                                            "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"
                End If
            End If

            ' Obtenemos las plantillas
            Dim myList As String() = API.DataLinkServiceMethods.GetTemplatesExcel(Me.Page)

            Dim nNumNodo As Integer = 1

            If Not myList Is Nothing Then

                strIco = strIconExportExcel
                For Each aElement As Object In myList
                    Dim splitSep() As String = {Me.wSepChar}
                    Dim cads As String() = aElement.ToString.Split(splitSep, StringSplitOptions.None)

                    strDesc = cads(1).Replace("'", "&#39;") 'nombre de la plantilla a mostrar

                    strArrayNodes &= "{ 'id':'" & nNumNodo & "', 'text':'" & strDesc & "', " &
                                            "'leaf': true, 'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"

                    nNumNodo = nNumNodo + 1

                Next

            End If

            ' Añadimos las plantillas de Sage-Murano sólo si tiene la licencia necesaria
            If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then
                Dim bolDatalinkSageInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\SageLogicControl")
                If bolDatalinkSageInstalled AndAlso GetFeaturePermission("Employees.DataLink.Exports.Sage") >= Permission.Read Then

                    ' Diario
                    strDesc = Me.Language.Translate("ExportSageMurano", DefaultScope)
                    strIco = strIconExportASCII
                    strArrayNodes &= "{ 'id':'8000', 'text':'" & strDesc.Replace("'", "&#39;") & "', " &
                                    "'leaf': true, " &
                                    "'icon': '" & Me.strImagesPath & strIco & "'"
                    strArrayNodes &= "},"

                    'Se comenta debido a que sage no cumple el estandar de robotics pactado con ellos. Hasta nueva orden queda comentado

                    ' Periodico
                    'strDesc = Me.Language.Translate("ExportSageMuranoP", DefaultScope)
                    'strIco = strIconExportASCII
                    'strArrayNodes &= "{ 'id':'8001', 'text':'" & strDesc.Replace("'", "&#39;") & "', " & _
                    '                "'leaf': true, " & _
                    '                "'icon': '" & Me.strImagesPath & strIco & "'"
                    'strArrayNodes &= "},"
                End If
            End If

            'EXPORTACIONES DIRECTAS. EL SERVIDOR DEVUELVE UN XLS SIN NECESIDAD DE DEFINIR PLANTLLA EN LA CARPETA DATALINK
            'Exportación de saldos diarios
            'Exportación de saldos periódicos
            'Exportación de calendario
            'Exportación de vacaciones

            '======================================
            'EXPORTACIONES DIRECTAS (ExportGuides)
            '======================================
            Dim aux As String = ""
            Dim dt As DataTable = API.DataLinkServiceMethods.GetExports(Me)
            Dim rows() As DataRow = dt.Select("Version < 2", "ProfileType,Name")

            For Each row As DataRow In rows
                aux = "{"
                aux &= "'id':'" & row("id").ToString & "', "
                aux &= "'text':'" & row("Name").ToString.Replace("'", "&#39;") & "', "
                aux &= "'leaf': true, " & "'icon': '" & Me.strImagesPath & strIconExportDirect & "'},"
                strArrayNodes &= aux
            Next

            'Agregar la lista de ficheros a la sesion
            Session.Remove("Export_TemplatesExcel")
            Session.Add("Export_TemplatesExcel", myList)

            If Me.strArrayNodes <> "" Then
                Me.strArrayNodes = "[" & Me.strArrayNodes.Substring(0, Me.strArrayNodes.Length - 1) & "]"
            End If
        End If

    End Sub

#End Region

End Class