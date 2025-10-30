Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class CauseSelectorData
    Inherits NoCachePageBase

#Region "Declarations"

    Private strAction As String = "TreeData"
    Private strIconCauses As String = "Cause_16.png"
    Private strImagesPath As String = ""

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Obtengo el parámetro de la acción a realizar
        Dim Action As String = Request.Params("action")
        If Action IsNot Nothing Then
            Me.strAction = Action
        End If

        Dim ImagesPath As String = Request.Params("ImagesPath")
        If ImagesPath IsNot Nothing Then
            Me.strImagesPath = ImagesPath
        Else
            Me.strImagesPath = "../../images/CausesSelector"
        End If
        If Not Me.strImagesPath.EndsWith("/") Then Me.strImagesPath &= "/"

        Select Case Me.strAction
            Case "TreeData" ' Obtiene los nodos del árbol del nivel indicado (strParent)

                Dim strArrayNodes As String = LoadCausesTree()

                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write(strArrayNodes)
            Case Else
                Me.Response.Clear()
                Me.Response.ContentType = "text/html"
                Me.Response.Write("/source/")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Function LoadCausesTree() As String
        Dim strArrayNodes As String = String.Empty
        Dim dTbl As DataTable = CausesServiceMethods.GetCauses(Me.Page)
        If dTbl.Rows.Count > 0 Then
            For Each dRow As DataRow In dTbl.Rows
                strArrayNodes &= "{ 'id':'" & dRow("ID") & "', 'text':'" & dRow("Name").Replace("'", "&#39;") & "', " &
                                 "'leaf': true, " &
                                 "'icon': '" & Me.strImagesPath & strIconCauses & "'"
                strArrayNodes &= "},"
            Next
        End If

        If strArrayNodes <> "" Then
            strArrayNodes = "[" & strArrayNodes.Substring(0, strArrayNodes.Length - 1) & "]"
        End If
        Return strArrayNodes

    End Function

#End Region

End Class