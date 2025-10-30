Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTWebLinks

    Public Class roWebLinksManager
        Private oState As roWebLinksManagerState = Nothing

        Public ReadOnly Property State As roWebLinksManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roWebLinksManagerState()
        End Sub

        Public Sub New(ByVal _State As roWebLinksManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"
        Public Shared Function GetAllWebLinks(ByVal oState As roWebLinksManagerState) As List(Of roWebLink)

            Dim ret As New List(Of roWebLink)

            Try

                Dim strSQL As String = "@SELECT# ID, Title, Description, Url, LinkCaption, Position, ShowOnLiveDashboard, ShowOnPortalDashboard, ShowOnPortalMenu FROM WebLinks order by Position"

                Dim tbres As DataTable = AccessHelper.CreateDataTable(strSQL)
                If tbres IsNot Nothing AndAlso tbres.Rows.Count > 0 Then
                    For Each dr As DataRow In tbres.Rows
                        Dim webLink As New roWebLink
                        webLink.ID = dr("ID")
                        webLink.Title = dr("Title")
                        webLink.Description = dr("Description")
                        webLink.URL = dr("Url")
                        webLink.LinkCaption = dr("LinkCaption")
                        webLink.Position = dr("Position")
                        webLink.ShowOnLiveDashboard = dr("ShowOnLiveDashboard")
                        webLink.ShowOnPortalDashboard = dr("ShowOnPortalDashboard")
                        webLink.ShowOnPortal = dr("ShowOnPortalMenu")
                        ret.Add(webLink)
                    Next
                End If

#If DEBUG Then
                ' Si la lista está vacía, se añaden datos de prueba
                If Not ret.Any() Then
                    Dim webLink As New roWebLink
                    webLink.ID = 1
                    webLink.Title = "Google"
                    webLink.Description = "Motor de búsqueda"
                    webLink.URL = "http://www.google.com"
                    webLink.LinkCaption = "Ir a Google"
                    webLink.Position = 1
                    webLink.ShowOnLiveDashboard = True
                    webLink.ShowOnPortalDashboard = True
                    webLink.ShowOnPortal = True
                    ret.Add(webLink)

                    webLink = New roWebLink
                    webLink.ID = 2
                    webLink.Title = "Yahoo"
                    webLink.Description = "Motor de búsqueda"
                    webLink.URL = "http://www.yahoo.com"
                    webLink.LinkCaption = "Ir a Yahoo"
                    webLink.Position = 2
                    webLink.ShowOnLiveDashboard = True
                    webLink.ShowOnPortalDashboard = True
                    webLink.ShowOnPortal = True
                    ret.Add(webLink)

                    webLink = New roWebLink
                    webLink.ID = 3
                    webLink.Title = "Bing"
                    webLink.Description = "Motor de búsqueda"
                    webLink.URL = "http://www.bing.com"
                    webLink.LinkCaption = "Ir a Bing"
                    webLink.Position = 3
                    webLink.ShowOnLiveDashboard = True
                    webLink.ShowOnPortalDashboard = True
                    webLink.ShowOnPortal = True
                    ret.Add(webLink)
                End If
#End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roWebLinksManager::GetAllWebLinks")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roWebLinksManager::GetAllWebLinks")
            End Try

            Return ret

        End Function

        Public Shared Function CreateOrUpdateWebLink(ByRef webLink As roWebLink, ByVal oState As roWebLinksManagerState) As Integer
            Dim bolRet As Integer = 0
            Try
                Dim isnew As Boolean = True
                Dim tb As New DataTable("WebLinks")
                Dim sqlQuery As String = $"@SELECT# * FROM WebLinks WHERE Id = {webLink.ID}"
                Dim cmd As DbCommand = CreateCommand(sqlQuery)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim linkrow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    linkrow = tb.NewRow
                Else
                    linkrow = tb.Rows(0)
                    isnew = False
                End If

                linkrow("Title") = webLink.Title
                linkrow("Description") = webLink.Description
                linkrow("Url") = webLink.URL
                linkrow("LinkCaption") = webLink.LinkCaption
                linkrow("Position") = webLink.Position
                linkrow("ShowOnLiveDashboard") = webLink.ShowOnLiveDashboard
                linkrow("ShowOnPortalDashboard") = webLink.ShowOnPortalDashboard
                linkrow("ShowOnPortalMenu") = webLink.ShowOnPortal

                If isnew Then tb.Rows.Add(linkrow)
                da.Update(tb)

                If isnew Then
                    'Si es un nuevo registro, devolvemos el ID generado
                    sqlQuery = "@SELECT# @@IDENTITY"
                    bolRet = Convert.ToInt32(ExecuteScalar(sqlQuery))
                Else
                    'Si es una modificación, devolvemos el ID existente
                    bolRet = webLink.ID
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roWebLinksManager::CreateOrUpdateWebLink")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roWebLinksManager::CreateOrUpdateWebLink")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteWebLink(ByRef webLink As roWebLink, ByVal oState As roWebLinksManagerState) As Boolean
            Dim bolRet As Boolean = False
            Try
                Dim sqlQuery As String = $"@DELETE# WebLinks WHERE Id = {webLink.ID}"
                bolRet = ExecuteSql(sqlQuery)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roWebLinksManager::DeleteWebLink")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roWebLinksManager::DeleteWebLink")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace