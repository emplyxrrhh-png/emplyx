Imports Robotics.Base.DTOs
Imports Robotics
Imports Robotics.Security.Base

Public Class ConceptsHelper

    Public Property VacationsResumeQueryCalled4CurrentDate As Boolean = False
    Public Property VacationsResumeQueryCalled4EndOfLastYear As Boolean = False
    Public Property VacationsResumeQueryCalled4EndOfLastPeriod As Boolean = False

    Function FillMockWithAccrualsByType(rowsNum As Integer, type As SummaryType, DefaultQuery As String, Optional WorkPeriod As String = "")
        Dim oRet As New System.Data.DataTable
        oRet.Columns.Add("IDConcept")
        oRet.Columns.Add("Name")
        oRet.Columns.Add("Total")
        oRet.Columns.Add("IDType")
        oRet.Columns.Add("TotalFormat")
        oRet.Columns.Add("DefaultQuery")
        oRet.Columns.Add("MaxValue")
        oRet.Columns.Add("ShortName")
        oRet.Columns.Add("YearWorkPeriod")
        For i = 0 To rowsNum - 1
            oRet.Rows.Add(1, "Saldo" + i.ToString, 0, type, 0, DefaultQuery, 0, "12" + i.ToString, WorkPeriod)
        Next
        Return oRet
    End Function

    Function GetAnnualWorkAccrualsStub(Optional oTable As DataTable = Nothing)
        Robotics.Base.VTBusiness.Concept.Fakes.ShimroConcept.GetContractAnnualizedAccrualsInt32DateTimeroBusinessStateRefBooleanInt32BooleanBooleanBoolean = Function(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal _OnlyViewInTerminals As Boolean,
                                                   ByVal _intIDConcept As Integer, ByVal filterBusinessGroups As Boolean, ByVal bolAddMaxValue As Boolean, ByVal bIncludeZeroes As Boolean)
                                                                                                                                                                 Dim stubDataTable As New System.Data.DataTable
                                                                                                                                                                 If oTable Is Nothing Then
                                                                                                                                                                     stubDataTable.Columns.Add("IDConcept")
                                                                                                                                                                     stubDataTable.Columns.Add("Name")
                                                                                                                                                                     stubDataTable.Columns.Add("Total")
                                                                                                                                                                     stubDataTable.Columns.Add("IDType")
                                                                                                                                                                     stubDataTable.Columns.Add("TotalFormat")
                                                                                                                                                                     stubDataTable.Columns.Add("DefaultQuery")
                                                                                                                                                                     stubDataTable.Columns.Add("MaxValue")
                                                                                                                                                                     stubDataTable.Columns.Add("ShortName")
                                                                                                                                                                     stubDataTable.Columns.Add("YearWorkPeriod")
                                                                                                                                                                     stubDataTable.Rows.Add(1, "Año laboral", 0, SummaryType.ContractAnnualized, 0, "L", 0, "AYW", "(05/03/2023 - 31/07/2023)")
                                                                                                                                                                 Else
                                                                                                                                                                     stubDataTable = oTable
                                                                                                                                                                 End If

                                                                                                                                                                 Return stubDataTable
                                                                                                                                                             End Function
    End Function

    Function GetMonthAccrualsStub(Optional oTable As DataTable = Nothing)
        Robotics.Base.VTBusiness.Concept.Fakes.ShimroConcept.GetMonthAccrualsInt32DateTimeroBusinessStateRefBooleanBooleanBooleanBooleanBoolean = Function(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal _OnlyViewInTerminals As Boolean,
                                                ByVal filterBusinessGroups As Boolean, ByVal bolAddMaxValue As Boolean, ByVal Last As Boolean, ByVal bIncludeZeroes As Boolean)
                                                                                                                                                      Dim stubDataTable As New System.Data.DataTable
                                                                                                                                                      If oTable Is Nothing Then
                                                                                                                                                          stubDataTable.Columns.Add("IDConcept")
                                                                                                                                                          stubDataTable.Columns.Add("Name")
                                                                                                                                                          stubDataTable.Columns.Add("Total")
                                                                                                                                                          stubDataTable.Columns.Add("IDType")
                                                                                                                                                          stubDataTable.Columns.Add("TotalFormat")
                                                                                                                                                          stubDataTable.Columns.Add("DefaultQuery")
                                                                                                                                                          stubDataTable.Columns.Add("MaxValue")
                                                                                                                                                          stubDataTable.Columns.Add("ShortName")
                                                                                                                                                          stubDataTable.Columns.Add("YearWorkPeriod")
                                                                                                                                                          stubDataTable.Rows.Add(1, "Mensual", 0, SummaryType.Mensual, 0, "M", 0, "AM1", "")
                                                                                                                                                          stubDataTable.Rows.Add(1, "Horas mensual", 0, SummaryType.Mensual, 0, "M", 0, "AM2", "")
                                                                                                                                                      Else
                                                                                                                                                          stubDataTable = oTable
                                                                                                                                                      End If

                                                                                                                                                      Return stubDataTable
                                                                                                                                                  End Function
    End Function

    Function GetAnnualAccrualsStub(Optional oTable As DataTable = Nothing)
        Robotics.Base.VTBusiness.Concept.Fakes.ShimroConcept.GetAnualAccrualsInt32DateTimeroBusinessStateRefBooleanInt32BooleanBooleanBooleanBoolean = Function(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal _OnlyViewInTerminals As Boolean,
                                                   ByVal _intIDConcept As Integer, ByVal filterBusinessGroups As Boolean, ByVal bolAddMaxValue As Boolean, ByVal Last As Boolean, ByVal bIncludeZeroes As Boolean)
                                                                                                                                                           Dim stubDataTable As New System.Data.DataTable
                                                                                                                                                           If oTable Is Nothing Then
                                                                                                                                                               stubDataTable.Columns.Add("IDConcept")
                                                                                                                                                               stubDataTable.Columns.Add("Name")
                                                                                                                                                               stubDataTable.Columns.Add("Total")
                                                                                                                                                               stubDataTable.Columns.Add("IDType")
                                                                                                                                                               stubDataTable.Columns.Add("TotalFormat")
                                                                                                                                                               stubDataTable.Columns.Add("DefaultQuery")
                                                                                                                                                               stubDataTable.Columns.Add("MaxValue")
                                                                                                                                                               stubDataTable.Columns.Add("ShortName")
                                                                                                                                                               stubDataTable.Columns.Add("YearWorkPeriod")
                                                                                                                                                               stubDataTable.Rows.Add(1, "Año", 0, SummaryType.Anual, 0, "Y", 0, "AY1", "")
                                                                                                                                                               stubDataTable.Rows.Add(1, "Horas anual", 0, SummaryType.Anual, 0, "Y", 0, "AY2", "")
                                                                                                                                                           Else
                                                                                                                                                               stubDataTable = oTable
                                                                                                                                                           End If

                                                                                                                                                           Return stubDataTable
                                                                                                                                                       End Function
    End Function

    Function GetWeekAccrualsStub(Optional oTable As DataTable = Nothing)
        Robotics.Base.VTBusiness.Concept.Fakes.ShimroConcept.GetWeekAccrualsInt32DateTimeroBusinessStateRefBooleanBooleanBooleanBoolean = Function(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal _OnlyViewInTerminals As Boolean,
                                               ByVal filterBusinessGroups As Boolean, ByVal bolAddMaxValue As Boolean, ByVal bIncludeZeroes As Boolean)
                                                                                                                                              Dim stubDataTable As New System.Data.DataTable
                                                                                                                                              If oTable Is Nothing Then
                                                                                                                                                  stubDataTable.Columns.Add("IDConcept")
                                                                                                                                                  stubDataTable.Columns.Add("Name")
                                                                                                                                                  stubDataTable.Columns.Add("Total")
                                                                                                                                                  stubDataTable.Columns.Add("IDType")
                                                                                                                                                  stubDataTable.Columns.Add("TotalFormat")
                                                                                                                                                  stubDataTable.Columns.Add("DefaultQuery")
                                                                                                                                                  stubDataTable.Columns.Add("MaxValue")
                                                                                                                                                  stubDataTable.Columns.Add("ShortName")
                                                                                                                                                  stubDataTable.Columns.Add("YearWorkPeriod")
                                                                                                                                                  stubDataTable.Rows.Add(1, "Semanal", 0, SummaryType.Semanal, 0, "W", 0, "AW1", "")
                                                                                                                                                  stubDataTable.Rows.Add(1, "Horas semanal", 0, SummaryType.Semanal, 0, "W", 0, "AW2", "")
                                                                                                                                              Else
                                                                                                                                                  stubDataTable = oTable
                                                                                                                                              End If

                                                                                                                                              Return stubDataTable
                                                                                                                                          End Function
    End Function

    Function GetContractAccrualsStub(Optional oTable As DataTable = Nothing)
        Robotics.Base.VTBusiness.Concept.Fakes.ShimroConcept.GetContractAccrualsInt32DateTimeroBusinessStateRefBooleanInt32BooleanBooleanBoolean = Function(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState, ByVal _OnlyViewInTerminals As Boolean,
                                                   ByVal _intIDConcept As Integer, ByVal filterBusinessGroups As Boolean, ByVal bolAddMaxValue As Boolean, ByVal bIncludeZeroes As Boolean)
                                                                                                                                                       Dim stubDataTable As New System.Data.DataTable
                                                                                                                                                       If oTable Is Nothing Then
                                                                                                                                                           stubDataTable.Columns.Add("IDConcept")
                                                                                                                                                           stubDataTable.Columns.Add("Name")
                                                                                                                                                           stubDataTable.Columns.Add("Total")
                                                                                                                                                           stubDataTable.Columns.Add("IDType")
                                                                                                                                                           stubDataTable.Columns.Add("TotalFormat")
                                                                                                                                                           stubDataTable.Columns.Add("DefaultQuery")
                                                                                                                                                           stubDataTable.Columns.Add("MaxValue")
                                                                                                                                                           stubDataTable.Columns.Add("ShortName")
                                                                                                                                                           stubDataTable.Columns.Add("YearWorkPeriod")
                                                                                                                                                           stubDataTable.Rows.Add(1, "Saldo1", 0, SummaryType.Contrato, 0, "C", 0, "ACW", "")
                                                                                                                                                           stubDataTable.Rows.Add(1, "Saldo2", 0, SummaryType.Contrato, 0, "C", 0, "ACN", "")
                                                                                                                                                       Else
                                                                                                                                                           stubDataTable = oTable
                                                                                                                                                       End If

                                                                                                                                                       Return stubDataTable
                                                                                                                                                   End Function
    End Function

    Function VacationsResumeQuerySpy()
        Base.VTBusiness.Common.Fakes.ShimroBusinessSupport.VacationsResumeQueryInt32Int32DateTimeDateTimeRefDateTimeRefDateTimeDoubleRefDoubleRefDoubleRefDoubleRefroBusinessStateDoubleRefDoubleRefStringListOfDateTime =
                    Function(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xReferenceDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime, ByVal xVacationsDate As DateTime, ByRef intDone As Double, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double, ByVal _State As roBusinessState, ByRef intExpiredDays As Double, ByRef intDaysWithoutEnjoyment As Double, ByVal IDContract As String, ByVal lstRequestDates As List(Of Date))
                        If xReferenceDate.Date = Date.Now.Date Then
                            VacationsResumeQueryCalled4CurrentDate = True
                        Else
                            If xReferenceDate.Date = New Date(Date.Now.AddYears(-1).Year, 12, 31) Then
                                VacationsResumeQueryCalled4EndOfLastYear = True
                            Else
                                If xReferenceDate.Date = New DateTime(2023, 2, 11) Then
                                    VacationsResumeQueryCalled4EndOfLastPeriod = True
                                End If
                            End If
                        End If
                    End Function
    End Function

End Class