Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase.roTypes

Namespace VTVisits

    Public Class roNotification

        Public Enum ActionType
            New_ = 1
            Update_ = 2
            Delete_ = 3
        End Enum

        Public Shared Function SendMail(ByVal Type As ActionType, ByVal IDEmployee As Integer, ByVal BeginDate As DateTime, ByVal id As String, ByVal url As String) As Boolean
            Try
                Dim sSQL As String = ""
                Dim tip As Integer = Type
                Dim idpas As Integer

                If CurrentOptions.notification = 1 Then
                    ' Notificar al responsable de la visita, es decir, al empleado que la recibe
                    idpas = Any2Integer(ExecuteScalar("@SELECT# id from sysroPassports where idemployee=" + IDEmployee.ToString))
                    sSQL = "@INSERT# INTO [dbo].[sysroNotificationTasks]"
                    sSQL += "([IDNotification],[Key1Numeric],[Key2Numeric],[Key3DateTime],[Parameters])"
                    sSQL += "VALUES(4501," + idpas.ToString + ", " + tip.ToString + ", " + Any2Time(BeginDate).SQLDateTime + ", "
                    sSQL += "'" + url + "/visits/#/plani/scheduled/visit/" + id + "')"
                ElseIf CurrentOptions.notification = 2 Then
                    ' Notificar al usuario que realiza la acción, es decir, al passport que hizo login en Visits
                    sSQL = "@INSERT# INTO [dbo].[sysroNotificationTasks]"
                    sSQL += "([IDNotification],[Key1Numeric],[Key2Numeric],[Key3DateTime],[Parameters])"
                    sSQL += "VALUES(4501," + IDEmployee.ToString + ", " + tip.ToString + ", " + Any2Time(BeginDate).SQLDateTime + ", "
                    sSQL += "'" + url + "/visits/#/plani/scheduled/visit/" + id + "')"
                End If

                If sSQL.Length > 0 Then ExecuteSql(sSQL)
            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::ronotification::sendmail::" + ex.Message)
                Return False
            End Try
            Return True
        End Function

    End Class

End Namespace