Imports Robotics.Base.VTBusiness.EventScheduler
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_EventSelectorData
    Inherits NoCachePageBase

    <Runtime.Serialization.DataContract()>
    Private Class roTreeEventItem

        <Runtime.Serialization.DataMember(Name:="id")>
        Public Id As String

        <Runtime.Serialization.DataMember(Name:="type")>
        Public TypeItem As String

        <Runtime.Serialization.DataMember(Name:="icon")>
        Public Icon As String

        <Runtime.Serialization.DataMember(Name:="name")>
        Public Name As String

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strAction As String = roTypes.Any2String(Request.Params("action"))
        If strAction <> String.Empty Then

            Select Case strAction
                Case "FieldFindData"

                    LoadEventsSearch()

                Case "LoadInitialData"

                    LoadInitialData()

                Case Else
                    Me.Response.Clear()
                    Me.Response.ContentType = "text/html"
                    Me.Response.Write("/source/")

            End Select
        Else
            Me.Response.Clear()
            Me.Response.ContentType = "text/html"
            Me.Response.Write("")
        End If

    End Sub

    Private Sub LoadInitialData()

        Dim strEventValues As String = roTypes.Any2String(Context.Request("EventValues"))
        'Dim strProjectValues As String = roTypes.Any2String(Context.Request("ProjectValues"))

        Dim oTreeEventItem As roTreeEventItem = Nothing
        Dim ListaItems As New Generic.List(Of roTreeEventItem)()

        Dim strAux As String = String.Empty

        If strEventValues <> String.Empty Then
            Dim tmpLista() As String = strEventValues.Split(",")
            For Each strID As String In tmpLista
                Dim oEventScheduler As roEventScheduler = EventSchedulerMethods.GetEventScheduler(Me, Val(strID), False)
                If oEventScheduler IsNot Nothing AndAlso oEventScheduler.Name <> String.Empty Then
                    oTreeEventItem = New roTreeEventItem()
                    oTreeEventItem.Id = FilterSpecialChars(oEventScheduler.ID)
                    oTreeEventItem.Name = FilterSpecialChars(oEventScheduler.Name)
                    oTreeEventItem.Icon = "<img src=\'" & ResolveUrl("~/Base/Images/EventSelector/Event16.png") & "\'/>"
                    oTreeEventItem.TypeItem = "event"
                    ListaItems.Add(oTreeEventItem)
                End If
            Next
        End If

        If ListaItems.Count > 0 Then
            For Each item As roTreeEventItem In ListaItems

                strAux &= "{fields:[{ field: 'id', value: '" & item.Id & "' }, " &
                                   "{ field: 'type', value: '" & item.TypeItem & "' }, " &
                                   "{ field: 'icon', value: '" & item.Icon & "' }, " &
                                   "{ field: 'name', value: '" & item.Name.Replace("'", "\'") & "'}]},"
            Next
            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strAux = "[" & strAux & "]" & ",{msg:''}"

            Context.Response.Write(strAux)
        Else
            Me.Response.Clear()
            Me.Response.ContentType = "text/html"
            Me.Response.Write("")
        End If

    End Sub

    Private Sub LoadEventsSearch()

        Dim strFieldFindColumn As String = ""
        Dim strFieldFindValue As String = ""
        Dim strArrayNodes As String = ""
        Dim strImagePath As String = ResolveUrl("~/Base/Images/EventSelector/")

        Dim FieldFind As String = roTypes.Any2String(Request.Params("FieldFindColumn"))
        If FieldFind <> String.Empty Then
            strFieldFindColumn = FieldFind
        End If

        FieldFind = roTypes.Any2String(Request.Params("FieldFindValue"))
        If FieldFind <> String.Empty Then
            strFieldFindValue = FieldFind
        End If

        Dim tbItems As DataTable = Nothing

        Dim strTextoABuscar As String = strFieldFindValue.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
        If Not strTextoABuscar.StartsWith("%") Then strTextoABuscar = "%" & strTextoABuscar
        If Not strTextoABuscar.EndsWith("%") Then strTextoABuscar = strTextoABuscar & "%"

        Select Case strFieldFindColumn.ToLower
            Case "year"
                If IsNumeric(strFieldFindValue) Then
                    tbItems = EventSchedulerMethods.GetEventsSchedulerByYear(Nothing, strFieldFindValue, False)
                End If

            Case "event"
                tbItems = EventSchedulerMethods.GetEventsSchedulerByName(Nothing, strTextoABuscar, False)

            Case Else
                Dim strWhere As String = String.Empty
                If strFieldFindColumn <> "" Then
                    strWhere = strFieldFindColumn & " LIKE '" & strTextoABuscar
                End If
                tbItems = EventSchedulerMethods.GetEventsSchedulerByYear(Nothing, strTextoABuscar, False)
        End Select

        If Not tbItems Is Nothing AndAlso tbItems.Rows.Count > 0 Then

            Dim oDataView As System.Data.DataView = New Data.DataView(tbItems)
            oDataView.Sort = "Date Desc"

            If strFieldFindColumn.ToLower = "event" Then

                For Each oDataviewRow As Data.DataRowView In oDataView
                    strArrayNodes &= "{ 'id':'" & oDataviewRow("ID") & "', 'text':'" &
                                      Me.FilterSpecialChars(roTypes.Any2String(oDataviewRow("Name"))) & "', 'leaf':true, 'icon': '" & strImagePath & "Event16.png" & "'},"
                Next
            Else

                For Each oDataviewRow As Data.DataRowView In oDataView
                    strArrayNodes &= "{ 'id':'" & oDataviewRow("ID") & "', 'text':'" &
                                      Me.FilterSpecialChars(roTypes.Any2String(oDataviewRow("Name"))) & "', 'leaf':true, 'icon': '" & strImagePath & "Event16.png" & "'},"
                Next
            End If

        End If

        If strArrayNodes <> "" Then
            strArrayNodes = "[" & strArrayNodes.Substring(0, strArrayNodes.Length - 1) & "]"
        End If

        Me.Response.Clear()
        Me.Response.ContentType = "text/html"
        If strArrayNodes = "" Then strArrayNodes = "[]"
        Me.Response.Write(strArrayNodes)

    End Sub

    Private Function FilterSpecialChars(ByVal strValue As String) As String
        Return strValue.Replace("'", "&#39;").Replace("\", "&#92;")
    End Function

End Class