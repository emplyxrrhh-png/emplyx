Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Public Class GeniusHelper

    Public Property ViewSaved As Boolean = False

    Public Sub New()
    End Sub

    Public Function SetAvailableGeniusViews(idPassport As Integer, passportViews As Integer(), otherViews As Integer(), commonViews As Integer())

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByIdPageBaseInt32Boolean =
            Function(reference, idView, bAudit)

                If passportViews.Contains(idView) Then
                    Return New Base.DTOs.roGeniusView With {.Id = idView, .IdPassport = idPassport}
                Else
                    If commonViews.Contains(idView) Then
                        Return New Base.DTOs.roGeniusView With {.Id = idView, .IdPassport = 0}
                    Else
                        If otherViews.Contains(idView) Then
                            Return New Base.DTOs.roGeniusView With {.Id = idView, .IdPassport = idPassport + 1}
                        Else
                            Return New Base.DTOs.roGeniusView With {.Id = 0, .IdPassport = 0}
                        End If

                    End If
                End If

            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.ExecuteGeniusViewPageBaseroGeniusView =
            Function(reference As PageBase, geniusView As roGeniusView)
                If passportViews.Contains(geniusView.Id) Then
                    Return New roGeniusExecution() With {
                    .IdGeniusView = geniusView.Id
                    }
                Else
                    Return Nothing
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.ShareGeniusViewPageBaseRefroGeniusViewInt32Array =
            Function(ByRef reference As PageBase, geniusView As roGeniusView, identity As Integer, users As Array)
                If passportViews.Contains(geniusView.Id) Then
                    Return True
                Else
                    Return Nothing
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.DeleteGeniusViewPageBaseroGeniusView =
            Function(ByVal reference As PageBase, geniusView As roGeniusView)
                If passportViews.Contains(geniusView.Id) Then
                    Return True
                Else
                    Return False
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.UpdateGeniusViewLayoutPageBaseroGeniusExecution =
            Function(ByVal reference As PageBase, geniusView As roGeniusExecution)
                If passportViews.Contains(geniusView.IdGeniusView) Then
                    Return True
                Else
                    Return False
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.SaveGeniusViewPageBaseRefroGeniusView =
            Function(ByRef reference As PageBase, geniusView As roGeniusView)
                If passportViews.Contains(geniusView.Id) Then
                    Return True
                Else
                    Return False
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByTaskPageBaseInt32Boolean =
            Function(reference As PageBase, idTask As Boolean, audit As Boolean)
                If passportViews.Contains(idTask) Then
                    Return New Base.DTOs.roGeniusView With {.Id = idTask, .IdPassport = idPassport}
                Else
                    If commonViews.Contains(idTask) Then
                        Return New Base.DTOs.roGeniusView With {.Id = idTask, .IdPassport = 0}
                    Else
                        If otherViews.Contains(idTask) Then
                            Return New Base.DTOs.roGeniusView With {.Id = idTask, .IdPassport = idPassport + 1}
                        Else
                            Return New Base.DTOs.roGeniusView With {.Id = 0, .IdPassport = 0}
                        End If

                    End If
                End If
            End Function

    End Function

    Public Function SetViewExecutions(idView As Integer, excutionId As Integer, bFinished As Boolean)
        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusExecutionWithSasKeyByIdPageBaseInt32 =
           Function(ByVal reference As PageBase, idExecution As Integer)

               Dim oView As New roGeniusExecution With {
                    .AzureSaSKey = "key",
                    .Id = excutionId,
                    .IdGeniusView = idView,
                    .FileLink = If(bFinished, "filelink", "Error")
               }

               Return oView

           End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusExecutionByIdPageBaseInt32 =
           Function(ByVal reference As PageBase, idExecution As Integer)

               Dim oView As New roGeniusExecution With {
                    .AzureSaSKey = "key",
                    .Id = excutionId,
                    .IdGeniusView = idView,
                    .FileLink = If(bFinished, "filelink", "Error")
               }

               Return oView

           End Function

    End Function

    Public Function SetPlanningListForGeniusViews(identity As Integer, passportViews As Integer(), planningsPerView As Integer(), otherViews As Integer(), commonViews As Integer())

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetUserGeniusPlanificationsPageBaseInt32RefInt32Ref =
                Function(reference, ByRef id, ByRef idPassport)
                    If passportViews.Contains(id) Then

                        Dim pView As Integer = passportViews.ToList.IndexOf(id)
                        Dim viewPlanningCout As Integer = planningsPerView(pView)

                        Dim oLst As New List(Of roGeniusScheduler)

                        For i = 0 To viewPlanningCout - 1
                            oLst.Add(New roGeniusScheduler With {
                                     .ID = id + (i * 10),
                                     .IDGeniusView = id,
                                     .IDPassport = identity
                                     })
                        Next

                        Return oLst
                    Else
                        If Not commonViews.Contains(id) AndAlso otherViews.Contains(id) Then
                            Dim oLst As New List(Of roGeniusScheduler)

                            oLst.Add(New roGeniusScheduler With {
                                     .ID = id,
                                     .IDGeniusView = id,
                                     .IDPassport = identity + 1
                                     })

                            Return oLst
                        Else
                            Return New List(Of roGeniusScheduler)
                        End If
                    End If
                End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusPlanificationByIdPageBaseInt32Boolean =
                Function(reference, id, bolAudit)

                    Dim planningIds As New Generic.List(Of Integer)

                    For Each idView In passportViews
                        Dim pView As Integer = passportViews.ToList.IndexOf(idView)
                        Dim viewPlanningCout As Integer = planningsPerView(pView)

                        For i = 0 To viewPlanningCout - 1
                            planningIds.Add(idView + (i * 10))
                        Next
                    Next

                    If planningIds.Contains(id) Then
                        Return New roGeniusScheduler() With {
                        .ID = id,
                        .IDPassport = identity
                    }
                    Else
                        Return New roGeniusScheduler() With {
                        .ID = id,
                        .IDPassport = (identity + 1)
                        }
                    End If

                End Function

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.DeleteGeniusPlanificationPageBaseroGeniusScheduler =
            Function(reference, geniusScheduler)
                If geniusScheduler.IDPassport = identity Then
                    Return True
                Else
                    Return False
                End If
            End Function
    End Function

    Public Function SaveGeniusViewSpy()

        Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.SaveGeniusViewPageBaseRefroGeniusView =
            Function(ByRef reference As PageBase, geniusView As roGeniusView)
                ViewSaved = True
            End Function

    End Function

End Class