<%@ WebHandler Language="VB" Class="srvManagePunches" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports System.Data

Public Class srvManagePunches
    Inherits handlerBase

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Response.ContentType = "text/plain"
        Select Case Request("action")
            Case "getTabs" ' Retorna la capçalera de la plana
                LoadDataTab()
            Case "uploadFileVTX"
                ProcessUploadedFile()
        End Select
    End Sub

    Private Sub ProcessUploadedFile()
        Try
            For Each filename As String In Request.Files
                Dim file As HttpPostedFile = Request.Files(filename)

                If file Is Nothing Then
                    Throw New IO.FileNotFoundException("No file found")
                End If
                If Not file.FileName.ToLower.EndsWith(".vtx") Then
                    Session("dtImportVtr") = Nothing
                    Throw New Exception("File is not .VTX")
                End If


                Dim sr As New IO.StreamReader(file.InputStream, Encoding.Default)
                ParseDeleteSQL(sr)
                sr.DiscardBufferedData()
                sr.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                ParseImportVTR(sr)
                sr.DiscardBufferedData()
                sr.Close()
            Next

            Response.Clear()
            Response.StatusCode = 200
            Response.Write("Ok")
        Catch Ex As Exception
            Response.StatusCode = 500
            Response.Write(Ex.Message)
        End Try
    End Sub

    Private Sub ParseDeleteSQL(ByRef sr As IO.StreamReader)

        Dim a As String
        Dim idstodelete As String = ""
        Do
            a = sr.ReadLine
            If a IsNot Nothing Then
                If a.Contains("[ORIGINALIDS]=") Then
                    idstodelete = a.Split("=")(1)
                    Exit Do
                End If

            End If
        Loop Until a Is Nothing

        'save the ids for the select
        Session("idsToDelete") = idstodelete




    End Sub

    '.VTR contain such lines: 97;2016/07/04;06:47;153;E;0;
    '.VTX files contain such lines: 97;2016/07/04;06:47;153;E;0;49805;
    Private Sub ParseImportVTR(ByRef sr As IO.StreamReader)

        Dim DX_ID = 1
        Dim dt As DataTable = New DataTable
        dt.Columns.Add(New DataColumn("DX_ID", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Terminal", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Fecha", GetType(DateTime)))
        dt.Columns.Add(New DataColumn("IdEmpleado", GetType(Integer)))
        dt.Columns.Add(New DataColumn("PunchType", GetType(String)))
        dt.Columns.Add(New DataColumn("Cause", GetType(String)))

        Dim line As String = sr.ReadLine
        While line IsNot Nothing
            If Not line.Contains("[ORIGINALIDS]=") Then
                Dim concat = DX_ID & ";" & line.Substring(0, GetNthIndex(line, ";", 2)) & " " & line.Substring(GetNthIndex(line, ";", 2) + 1) 'joins Fecha y Hora into one 
                dt.Rows.Add(concat.Split(";").Take(6).ToArray)
                DX_ID += 1
            End If
            line = sr.ReadLine

        End While

        Session("dtImportVtr") = dt 'this dt will feed the DevExpress grid
    End Sub

    Public Function GetNthIndex(s As String, t As Char, n As Integer) As Integer
        Dim count As Integer = 0
        For i As Integer = 0 To s.Length - 1
            If s(i) = t Then
                count += 1
                If count = n Then
                    Return i
                End If
            End If
        Next
        Return -1
    End Function

    Private Sub LoadDataTab()
        Try
            'Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, Request("ID"), False)
            'If oEmployee Is Nothing Then Exit Sub

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))


            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/SDK.png"
            img.Height = 80
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameTerminal"" class=""NameText"">" & "Gestión de fichajes" & " </span></div>" 'TODO: translations

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(intActiveTab))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)

        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Genera els botons de la dreta (General, ...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateTabs(ByVal intActiveTab As Integer) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}
        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_ImportPunches", Me.Language.Translate("tabImport", Me.DefaultScope), "bTab")

        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        '================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell
        hCellGen.Attributes("valign") = "top"

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        '================================



        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        'oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs(0);")

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"


        Return hTableGen ' Retorna el HTMLTable

    End Function

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function



End Class