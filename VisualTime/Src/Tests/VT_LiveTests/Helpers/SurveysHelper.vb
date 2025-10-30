Imports Robotics
Imports Robotics.Base.DTOs

Public Class SurveysHelper

    Public Property SurveyListLoaded As Boolean = False
    Public Property SurveyResponsesLoaded As Boolean = False

    Public Sub New()
    End Sub

    Public Function SetAvailableSurveys(idPassport As Integer, passportSurveys As Integer(), otherSurveys As Integer())

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.GetAllSurveysPageBaseInt32 =
            Function(reference, idSupervisor)
                SurveyListLoaded = True
                Dim oLst As New Generic.List(Of roSurvey)

                For Each idSurvey As Integer In passportSurveys
                    oLst.Add(New Base.DTOs.roSurvey With {.Id = idSurvey, .CreatedBy = idPassport, .CurrentEmployeeResponses = {}})
                Next

                Return oLst.ToArray()
            End Function

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.GetSurveyInt32PageBaseBoolean =
            Function(idSurvey, reference, bAudit)

                If passportSurveys.Contains(idSurvey) Then
                    Return New Base.DTOs.roSurvey With {.Id = idSurvey, .CreatedBy = idPassport, .CurrentEmployeeResponses = {}}
                Else
                    If otherSurveys.Contains(idSurvey) Then
                        Return New Base.DTOs.roSurvey With {.Id = idSurvey, .CreatedBy = idPassport + 1, .CurrentEmployeeResponses = {}}
                    Else

                        Return New Base.DTOs.roSurvey With {.Id = 0, .CreatedBy = 0, .CurrentEmployeeResponses = {}}
                    End If
                End If

            End Function

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.GetSurveyResponsesInt32PageBaseBoolean =
           Function(idSurvey, reference, audit)
               SurveyResponsesLoaded = True
               Dim oResponse As New roSurveyResponses
               Dim oLst As New Generic.List(Of String)
               For i = 0 To 10
                   oLst.Add("Response" & i)
               Next
               oResponse.Data = oLst.ToArray()
               oResponse.ResultCount = oResponse.Data.Length
               Return oResponse
           End Function

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.GetSurveyResponsesByIdEmployeeInt32Int32ArrayPageBaseBoolean =
           Function(idSurvey, idEmployees, reference, audit)
               SurveyResponsesLoaded = True
               Dim oResponse As New roSurveyResponses
               Dim oLst As New Generic.List(Of String)
               For Each idEmployee In idEmployees
                   oLst.Add("Response" & idEmployee)
               Next
               oResponse.Data = oLst.ToArray()
               oResponse.ResultCount = oResponse.Data.Length
               Return oResponse
           End Function

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.CreateOrUpdateSurveyPageBaseroSurveyRefBoolean =
           Function(reference, ByRef oSurvey, bAudit)
               If passportSurveys.Contains(oSurvey.Id) OrElse oSurvey.Id = 0 Then
                   Return True
               Else
                   Return False
               End If
           End Function

        Robotics.Web.Base.API.Fakes.ShimSurveyServiceMethods.DeleteSurveyroSurveyPageBaseBoolean =
           Function(oSurvey, reference, bAudit)
               If passportSurveys.Contains(oSurvey.Id) Then
                   Return True
               Else
                   Return False
               End If
           End Function

    End Function

End Class