Public Class DailyCausesHelper
    Public Property SaveDailyCauseCalled As Boolean = False
    Public Property ManualDailyCause As Boolean = False
    Public Property DeleteDailyCauseCalled As Boolean = False
    Public Property CheckUser As Boolean = True

    Function SaveDailyCauseSpy()
        Robotics.Base.VTBusiness.Cause.Fakes.ShimroDailyCause.AllInstances.SaveBooleanBooleanString = Function(a, b, c, d)
                                                                                                          SaveDailyCauseCalled = True
                                                                                                          ManualDailyCause = a.Manual
                                                                                                          Return True
                                                                                                      End Function
    End Function

    Function DeleteDailyCauseSpy()
        Robotics.Base.VTBusiness.Cause.Fakes.ShimroDailyCause.AllInstances.Delete = Function(a)
                                                                                        DeleteDailyCauseCalled = True
                                                                                        ManualDailyCause = a.Manual
                                                                                        Return True
                                                                                    End Function
    End Function

    Function GetCauseByShortName()
        Robotics.Base.VTBusiness.Cause.Fakes.ShimroCause.GetCauseByShortNameStringroCauseState = Function(a, b)
                                                                                                     Return 1
                                                                                                 End Function
    End Function

    Function LoadWithParamsSpy(ByVal existingCauseValue As Integer)
        Robotics.Base.VTBusiness.Cause.Fakes.ShimroDailyCause.AllInstances.LoadWithParamsInt32DateTimeInt32BooleanBoolean = Function(a, b, c, d, e, f)
                                                                                                                                a.Manual = d
                                                                                                                                CheckUser = f
                                                                                                                                If existingCauseValue >= 0 Then
                                                                                                                                    a.Value = existingCauseValue
                                                                                                                                    Return True
                                                                                                                                Else
                                                                                                                                    Return False
                                                                                                                                End If
                                                                                                                            End Function
    End Function

End Class
