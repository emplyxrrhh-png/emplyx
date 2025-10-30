Imports System.Net
Imports System.Web.Hosting

Public Class KeepAliveProcess
    Implements IProcessHostPreloadClient

    Public Shared Function preloadWarmup()
        Dim tmp As New KeepAliveProcess
        tmp.Preload({})
        Return True

    End Function

    Private Sub Preload(parameters() As String) Implements IProcessHostPreloadClient.Preload

        ExecuteWarmUp()

        'Dim executeThread As New Threading.Thread(AddressOf ExecuteWarmUp)
        'executeThread.Start(HttpContext.Current)

        'Dim executeThreadSites As New Threading.Thread(AddressOf PreloadSites)
        'executeThreadSites.Start()

    End Sub

    Private Shared Sub PreloadSites()
        Dim serverURL As String = String.Empty

        serverURL = "https://vtlive.visualtime.net/"

#If DEBUG Then
        serverURL = "http://localhost:8025/"
#End If

        Try
            Dim MyRssRequest As HttpWebRequest = HttpWebRequest.Create(serverURL & "Employees/Employees.aspx")
            Dim MyRssResponse As HttpWebResponse = MyRssRequest.GetResponse()
            MyRssRequest = HttpWebRequest.Create(serverURL & "Employees/Handlers/srvEmployees.ashx?action=getBarButtons")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Employees/Groups.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Employees/Handlers/srvGroups.ashx?action=getBarButtons")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Scheduler/Calendar.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Scheduler/MovesNew.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Requests/Requests.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Requests/Handlers/srvRequests.ashx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Security/Supervisors.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Security/Handlers/srvSupervisorsV3.ashx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Alerts/Alerts.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Alerts/AlertsDetail.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Alerts/TasksQueue.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "DataLink/DataLink.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "DataLink/DataLinkBusiness.aspx")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Start")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Genius")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "OnBoarding")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "DocumentaryManagement")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Communique")
            MyRssResponse = MyRssRequest.GetResponse()

            MyRssRequest = HttpWebRequest.Create(serverURL & "Report")
            MyRssResponse = MyRssRequest.GetResponse()
        Catch ex As Exception

        End Try

    End Sub

    Private Shared Sub ExecuteWarmUp()

        PreloadSites()
    End Sub

End Class