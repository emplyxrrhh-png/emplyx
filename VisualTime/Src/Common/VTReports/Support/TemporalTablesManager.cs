using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Diagram.Core.Native;
using DevExpress.Xpo.Helpers;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.Base.VTBusiness.Common;
using Robotics.Base.VTBusiness.Incidence;
using Robotics.Base.VTBusiness.Report;
using Robotics.Base.VTBusiness.Shift;
using Robotics.Base.VTBusiness.Support;
using Robotics.Base.VTEmployees.Contract;
using Robotics.Base.VTEmployees.Employee;
using Robotics.Base.VTUserFields.UserFields;
using Robotics.Security;
using Robotics.Security.Base;
using Robotics.VTBase;
using Robotics.VTBase.Extensions;
using Robotics.VTBase.Extensions.VTLiveTasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static DevExpress.CodeParser.CodeStyle.Formatting.Rules;
using Robotics.DataLayer;
using DevExpress.Xpo;
using Robotics.Base.VTBusiness.Concept;
using Robotics.Base.VTConcepts.Concepts;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using System.Runtime.InteropServices.ComTypes;
using System.Data.SqlClient;
using DevExpress.XtraCharts.Native;
using DevExpress.DataAccess.ObjectBinding;
using Robotics.Base.VTSelectorManager;

namespace ReportGenerator.Support
{
    public class TemporalTablesManager
    {
        public TemporalTablesManager()
        {
        }

        private enum IndicatorPeriodType
        {
            MONTH_A = 0,
            MONTH_1 = 1,
            MONTH_2 = 2,
            Q1 = 3,
            Q2 = 4,
            Q3 = 5,
            Q4 = 6,
            YEAR_A = 7,
            Year_1 = 8
        }

        private enum StatusZoneEnum
        {
            IN_Zone_LessThan48H = 0,
            IN_Zone_MoreThan48H = 1,
            Employee_MustBe = 2,
            Employee_Absent = 3,
            IN_OtherZone_LessThan48H = 4,
            IN_OtherZone_MoreThan48H = 5,
            With_Error = 99
        }

        private Robotics.VTBase.roSupport GetSupport(int passportId, roPassport _Passport = null)
        {
            Robotics.VTBase.roSupport oRet;

            var oPassport = _Passport;
            if (oPassport == null)
            {
                try
                {
                    roSecurityState state = new roSecurityState();
                    oPassport = roPassportManager.GetPassport(passportId, LoadType.Passport, ref state);
                }
                catch (Exception e)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TemporalTablesManager::Error obtainging passport::" + e.Message, e);
                }
            }
            if (oPassport != null)
                oRet = new Robotics.VTBase.roSupport(oPassport.Language.Key);
            else
                oRet = new Robotics.VTBase.roSupport();

            return oRet;
        }

        public void ExecuteTask(string strTask, int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            roTrace.get_GetInstance().InitTraceEvent();

            switch (strTask.ToUpper())
            {
                case "TMPCALENDAR":
                    {
                        CreateTmpCalendar(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPMONTHLYEMPLOYEECALENDAR":
                    {
                        CreateTmpMonthyCalendar(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPMONTHLYEMPLOYEECALENDARV2":
                    {
                        CreateTmpMonthyCalendarV2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPSHifTDEFINITIONS":
                    {
                        CreateTmpShiftDefinitions(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPTOP":
                    {
                        CreateTmpTop(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPEMERGENCY":
                    {
                        if (parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector") && x.Name.Equals("Employees")).FirstOrDefault().Value.ToString().Split('@')[0] != "")
                            CreateTmpEmergency(IDReportTask, parametersList, originalParametersList);
                        else
                            CreateTmpEmergency(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPDAILYPARTIALACCRUALS":
                    {
                        CreateTmpDailyPartialAccruals(IDReportTask, parametersList, originalParametersList, false, false);
                        break;
                    }

                case "TMPDAILYPARTIALACCRUALSWITHZEROS":
                    {
                        CreateTmpDailyPartialAccruals(IDReportTask, parametersList, originalParametersList, true, false);
                        break;
                    }

                case "TMPDAILYPARTIALACCRUALSWITHZEROSUSERFIELD":
                    {
                        CreateTmpDailyPartialAccruals(IDReportTask, parametersList, originalParametersList, false, true);
                        break;
                    }

                case "CREATETMPCONTROLHOLIDAYS":
                    {
                        CreateTmpControlHolidays(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPUSERFIELDS":
                    {
                        CreateTmpUserFields("sysrovwAllEmployeeGroups", IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPUSERFIELDSACCESSGROUPS":
                    {
                        CreateTmpUserFields("sysrovwAllEmployeeAccessGroups", IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPDAILYEMPLOYEEINFO":
                    {
                        CreateTmpUserFields("sysrovwAllEmployeeAccessGroups", IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPWHOSHOULDCOMEANDINST":
                    {
                        CreateTmpWhoShouldComeAndInst(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPHOLIDAYSCONTROLBYCONTRACT":
                    {
                        CreateTmpHolidaysControlByContractEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPPLANNEDHOLIDAYSCONTROLBYCONTRACT":
                    {
                        CreateTmpPlannedHolidaysControlByContractEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPINDICATORSANALYSIS":
                    {
                        CreateTmpIndicatorAnalysisEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPEMERGENCYEX":
                    {
                        //ESTE SE ELIMINA?
                        CreateTmpEmergencyEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPEMERGENCYTOTALS":
                    {
                        CreateTmpEmergencyWithAttendanceByZonesEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPEMERGENCYBASIC":
                    {
                        CreateTmpEmergencyWithAttendanceByZonesBasicEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPCRITERIOSALDOS":
                    {
                        CreateTmpCriterioSaldos(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPCRITERIOCAUSESINCIDENCES":
                    {
                        CreateTmpCriterioCausesIncidences(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPDETAILEDCALENDAREMPLOYEE":
                    {
                        CreateTmpDetailedCalendarEmployeeEx2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPVISITSLOCATION":
                    {
                        CreateTmpVisitsLocationEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPWHOSHOULDBEPRESENTNOW":
                    {
                        CreateTmpWhoShouldBePresentNowEx(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPACCRUALSBYCONTRACT":
                    {
                        CreateTmpAccrualsByContract(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPHOLIDAYSCONTROLBYCONTRACTV2": //Control de días de vacaciones por contrato
                    {
                        CreateTmpHolidaysControlByContractExV2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPPLANNEDHOLIDAYSCONTROLBYCONTRACTV2": //Control de horas de vacaciones por contrato
                    {
                        CreateTmpPlannedHolidaysControlByContractExV2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPACCRUALSBYGROUP":
                    {
                        CreateTmpAccrualsByGroup(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPACCRUALSBYEMPLOYEE":
                    {
                        CreateTmpAccrualsByEmployee(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPBOLSAHORAS":
                    {
                        CreateTmpBolsaHoras(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPROTACION":
                    {
                        CreateTmpRotacion(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPACCRUALSDETAILBYEMPLOYEE":
                    {
                        CreateTmpVM(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMRETRASOS":
                    {
                        CreateTmpRET(IDReportTask, parametersList, originalParametersList);
                        break;
                    }

                case "TMPCN":
                    {
                        CreateTmpCN(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPDETAILEDCALENDAREMPLOYEEV2":
                    {
                        CreateTmpDetailedCalendarEmployeev2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPDAILYCALENDAREMPLOYEEV2":
                    {
                        CreateTmpDailyCalendarEmployeev2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPVSL_ANNUALACCRUALS":
                    {
                        VSL_CreateTmpAccrualsControlByEmployee(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPSCHEDULINGLAYERRESUME":
                    {
                        CreateTmpSchedulingLayerResumev2(IDReportTask, parametersList, originalParametersList);
                        break;
                    }
                case "TMPDETAILEDCALENDAREMPLOYEEPAGOSLEAR":
                    {
                        CreateTmpDetailedCalendarEmployeeLear(IDReportTask, parametersList, originalParametersList);
                        break;
                    }



                default:
                    {
                        throw new Exception("Invalid function call name: " + strTask);
                    }
            }

            roTrace.get_GetInstance().AddTraceEvent($"TMP tables({strTask.ToUpper()}) ready.");
        }

        public bool FlushTemporalRows(string strTask, int IDReportTask)
        {
            bool result = false;
            try
            {
                switch (strTask.ToUpper())
                {
                    case "TMPCALENDAR":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPCALENDAREMPLOYEE WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPMONTHLYEMPLOYEECALENDAR":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPMONTHLYEMPLOYEECALENDAR WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPSHifTDEFINITIONS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPSHifTDEFINITIONS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPTOP":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPTOP WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPEMERGENCY":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPDAILYPARTIALACCRUALS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDailyPartialAccruals WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPDAILYPARTIALACCRUALSWITHZEROS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDailyPartialAccruals WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPDAILYPARTIALACCRUALSWITHZEROSUSERFIELD":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDailyPartialAccruals WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "CREATETMPCONTROLHOLIDAYS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPCONTROLHOLIDAYS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPUSERFIELDS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPUSERFIELDS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPUSERFIELDSACCESSGROUPS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPUSERFIELDS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPDAILYEMPLOYEEINFO":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPUSERFIELDS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPWHOSHOULDCOMEANDINST":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPUSERFIELDS WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPHOLIDAYSCONTROLBYCONTRACT":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }                        
                    case "TMPMONTHLYEMPLOYEECALENDARV2":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# tmpMonthlyEmployeeCalendar WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPPLANNEDHOLIDAYSCONTROLBYCONTRACT":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPINDICATORSANALYSIS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPIndicatorsAnalysis WHERE IDReportTask = " + IDReportTask.ToString());
                            result = result && Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPGroupIndicatorsAnalysis WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPEMERGENCYEX":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPEMERGENCYTOTALS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask.ToString());
                            result = result && Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyTotals WHERE IDReportTask = " + IDReportTask.ToString());
                            result = result && Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyVisits WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPEMERGENCYBASIC":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyBasic WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPCRITERIOSALDOS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPReports_CriterioSaldos WITH (ROWLOCK) WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPCRITERIOCAUSESINCIDENCES":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPReports_CriterioCausesIncidences WITH (ROWLOCK) WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPDETAILEDCALENDAREMPLOYEE":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDetailedCaldendarEmployee WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPVISITSLOCATION":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPVISITSLOCATION WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPWHOSHOULDBEPRESENTNOW":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPWhoShouldComeAndInst WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPACCRUALSBYCONTRACT":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByContract WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPHOLIDAYSCONTROLBYCONTRACTV2":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPHOLIDAYSCONTROLByContract  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPPLANNEDHOLIDAYSCONTROLBYCONTRACTV2":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPHOLIDAYSCONTROLByContract  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPACCRUALSBYGROUP":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByGroup  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPACCRUALSBYEMPLOYEE":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }

                    case "TMPBOLSAHORAS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPROTACION":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPACCRUALSDETAILBYEMPLOYEE":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsDetailByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMRETRASOS":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsDetailByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPCN":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPAccrualsByEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPDETAILEDCALENDAREMPLOYEEV2":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPDetailedCalendarEmployee  WHERE IDReportTask = " + IDReportTask.ToString());
                            if (result) {
                                result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPDetailedCalendarEmployee_DailyIncidences  WHERE IDReportTask = " + IDReportTask.ToString());
                            }
                            break;
                        }
                    case "TMPSCHEDULINGLAYERRESUME":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TmpSchedulingLayerResume  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                    case "TMPDETAILEDCALENDAREMPLOYEEPAGOSLEAR":
                        {
                            result = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPDETAILEDCALENDAREMPLOYEEPAGOSLEAR  WHERE IDReportTask = " + IDReportTask.ToString());
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::FlushTermporalRows::", ex);
            }
            finally
            {
            }
            return result;
        }

        public void CreateTmpCalendar(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            //
            // Llena la tabla temporal con los datos de la consulta
            //

            try
            {
                var Total = default(double);
                var DIA = new double[31];
                var Dias = default(int);
                double TotalHorasAnuales;
                double TotalDiasAnuales;

                var oSupport = GetSupport(int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier") && x.Name.Equals("PassportId")).FirstOrDefault().Value.ToString()));
                var oLanguage = new roLanguage();
                oLanguage.SetLanguageReference("LiveReports", oSupport.ActiveLanguage);

                // Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPCALENDAREMPLOYEE WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string FI = roTypes.Any2Time(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0]).SQLSmallDateTime();
                string FF = roTypes.Any2Time(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[1]).SQLSmallDateTime();

                // Obtenemos los empleados seleccionados con contrato activo en el periodo solicitado
                string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector") && x.Name.Equals("Employees")).FirstOrDefault().Value.ToString().Split('@');
                string strSQL = "@SELECT# * FROM sysrovwAllEmployeeGroups WHERE " + roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]).Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups") + " and idemployee in (@SELECT# idemployee from employeecontracts where (" + "(BeginDate <=" + FI + " and EndDate >=" + FF + ") or	" + "(BeginDate <=" + FF + " and EndDate >=" + FF + ") or " + "(BeginDate <=" + FI + " and EndDate >=" + FI + ") or " + "(BeginDate >=" + FI + " and EndDate <=" + FF + "))) ";

                var tbQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(strSQL);

                var tbTmp = new DataTable("TMPCALENDAREMPLOYEE");
                strSQL = "@SELECT# * FROM TMPCALENDAREMPLOYEE";
                var cmd = Robotics.DataLayer.AccessHelper.CreateCommand(strSQL);
                var da = Robotics.DataLayer.AccessHelper.CreateDataAdapter(cmd, true);
                da.Fill(tbTmp);

                DataRow oNewRow;

                foreach (DataRow oRow in tbQuery.Rows)
                {
                    TotalHorasAnuales = 0;
                    TotalDiasAnuales = 0;

                    for (int x = 1; x <= 12; x++)
                    {
                        // Mes
                        oNewRow = tbTmp.NewRow();
                        {
                            var withBlock = oNewRow;
                            withBlock["IDEmployee"] = oRow["IDEmployee"];
                            withBlock["EMPLEADO"] = oRow["EmployeeName"];
                            withBlock["MES"] = x;
                            withBlock["IDReportTask"] = IDReportTask;
                            switch (x)
                            {
                                case 1:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.January", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 2:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.February", "TmpCalendar");
                                        var xDate = new DateTime(Convert.ToInt32(Strings.Format(Convert.ToDateTime(roTypes.Any2Time(((DateTime[])(parametersList.Where(y => y.Type.Equals("Robotics.Base.datePeriodSelector") && y.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0]).DateOnly()), "yyyy")), 2, 28);
                                        if (xDate.AddDays(1).Month == 2)
                                            withBlock["DIAS"] = 29;
                                        else
                                            withBlock["DIAS"] = 28;
                                        break;
                                    }

                                case 3:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.March", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 4:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.April", "TmpCalendar");
                                        withBlock["DIAS"] = 30;
                                        break;
                                    }

                                case 5:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.May", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 6:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.June", "TmpCalendar");
                                        withBlock["DIAS"] = 30;
                                        break;
                                    }

                                case 7:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.July", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 8:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.August", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 9:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.September", "TmpCalendar");
                                        withBlock["DIAS"] = 30;
                                        break;
                                    }

                                case 10:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.October", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }

                                case 11:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.November", "TmpCalendar");
                                        withBlock["DIAS"] = 30;
                                        break;
                                    }

                                case 12:
                                    {
                                        withBlock["NOMBRE"] = oLanguage.Translate("TmpCalendar.December", "TmpCalendar");
                                        withBlock["DIAS"] = 31;
                                        break;
                                    }
                            }

                            GetPeriodMoves(roTypes.Any2Long(oRow["IDEMPLOYEE"]), Convert.ToString(x), ref Total, ref DIA, ref Dias, parametersList);

                            for (int i = 1; i <= 31; i++)
                                withBlock["horasdia" + Convert.ToString(i)] = DIA[i - 1];
                            withBlock["TOTALHORAS"] = Total;
                            withBlock["TOTALDIAS"] = Dias;
                            TotalHorasAnuales = TotalHorasAnuales + Total;
                            TotalDiasAnuales = TotalDiasAnuales + Dias;

                            if (x == 12)
                            {
                                withBlock["TOTALHORASAnuales"] = TotalHorasAnuales;
                                withBlock["TOTALdiasAnuales"] = TotalDiasAnuales;
                            }
                        }
                        tbTmp.Rows.Add(oNewRow);
                    }
                }

                da.Update(tbTmp);
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpCalendar");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpCalendar");
            }
            finally
            {
            }
        }

        private void GetPeriodMoves(long IDEmployee, string pos, ref double Total, ref double[] DIA, ref int Dias, List<ReportParameter> parametersList)
        {
            //
            //
            //
            int i;
            string sSQL;
            string sSQL2;

            try
            {
                // 'Crea la consulta con los datos a consultar
                sSQL = "@SELECT# idshift1, date, isnull(DailySchedule.ExpectedWorkingHours,Shifts.ExpectedWorkingHours) as ExpectedWorkingHours ";
                sSQL = sSQL + " from DailySchedule inner join Shifts";
                sSQL = sSQL + " on DailySchedule.IDShift1=Shifts.ID";
                sSQL = sSQL + " WHERE IDEmployee=" + Convert.ToString(IDEmployee);
                string StartNaturalDate = ((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0].ToString();
                if (Convert.ToInt32(pos) < 10)
                {
                    sSQL = sSQL + " AND Date>=" + roTypes.Any2Time("01/0" + pos + "/" + Convert.ToString(Convert.ToDateTime(roTypes.Any2Time(StartNaturalDate).DateOnly()).Year)).SQLSmallDateTime();
                    sSQL = sSQL + " AND Date<" + roTypes.Any2Time("01/0" + pos + "/" + Convert.ToString(Convert.ToDateTime(roTypes.Any2Time(StartNaturalDate).DateOnly()).Year)).Add(1, "m").Substract(1, "s").SQLSmallDateTime();
                }
                else
                {
                    sSQL = sSQL + " AND Date>=" + roTypes.Any2Time("01/" + pos + "/" + Convert.ToString(Convert.ToDateTime(roTypes.Any2Time(StartNaturalDate).DateOnly()).Year)).SQLSmallDateTime();
                    sSQL = sSQL + " AND Date<" + roTypes.Any2Time("01/" + pos + "/" + Convert.ToString(Convert.ToDateTime(roTypes.Any2Time(StartNaturalDate).DateOnly()).Year)).Add(1, "m").SQLSmallDateTime();
                }
                sSQL = sSQL + " ORDER BY Date ASC";

                Total = 0;
                Dias = 0;
                object Contract;
                for (i = 1; i <= 31; i++)
                {
                    sSQL2 = "@SELECT# idcontract from employeecontracts";
                    sSQL2 = sSQL2 + " where IDEmployee=" + Convert.ToString(IDEmployee);
                    try
                    {
                        sSQL2 = sSQL2 + " and " + roTypes.Any2Time(Convert.ToString(i) + "/" + pos + "/" + Convert.ToString(Convert.ToDateTime(roTypes.Any2Time(StartNaturalDate).DateOnly()).Year)).SQLSmallDateTime() + " between BeginDate and EndDate";
                        Contract = Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL2);
                    }
                    catch
                    {
                        Contract = null;
                    }
                    if (Strings.Len(roTypes.Any2String(Contract)) > 0)
                        DIA[i - 1] = 0;
                    else
                        DIA[i - 1] = -1;
                }

                var tb = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                DateTime xDate;
                foreach (DataRow oRow in tb.Rows)
                {
                    xDate = Convert.ToDateTime(oRow["Date"]).Date;
                    if (DIA[xDate.Day - 1] >= 0)
                    {
                        // DIA(xDate.Day - 1) = Any2Double(oRow("ExpectedWorkingHours"))
                        // Total = Total + Any2Double(oRow("ExpectedWorkingHours"))
                        // if Any2Double(oRow("ExpectedWorkingHours")) > 0 {
                        // Si no es horario de vacaciones obtenemos las horas teoricas direcamente
                        if (roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# ISNULL(ShiftType, 0) FROM Shifts WHERE ID=" + Convert.ToString(roTypes.Any2Double(oRow["IDShift1"])))) != 2)
                        {
                            DIA[xDate.Day - 1] = roTypes.Any2Double(oRow["ExpectedWorkingHours"]);
                            Total = Total + roTypes.Any2Double(oRow["ExpectedWorkingHours"]);
                        }
                        else
                        {
                            // Obtenemos las horas teoricas del horario base
                            double dblExpectedWorkingHours = 0;
                            dblExpectedWorkingHours = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(isnull(DailySchedule.ExpectedWorkingHours,isnull(Shifts.ExpectedWorkingHours, 0))) as Total FROM DailySchedule, Shifts WHERE IDShift1 = " + Convert.ToString(roTypes.Any2Double(oRow["IDShift1"])) + " AND Shifts.ID = DailySchedule.IDShiftBase AND IDEmployee = " + Convert.ToString(IDEmployee) + " AND Date=" + roTypes.Any2Time(oRow["Date"]).SQLSmallDateTime() + " AND IDShiftBase IS NOT NULL "));
                            DIA[xDate.Day - 1] = dblExpectedWorkingHours;
                        }

                        Dias = Dias + 1;
                    }
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:GetPeriodMoves");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:GetPeriodMoves");
            }
        }

        private void CreateTmpMonthyCalendar(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            /////////////////////////////////////////////
            DateTime startDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime endDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;
            /////////////////////////////////////////////

            roTime Days;

            try
            {
                // Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPMONTHLYEMPLOYEECALENDAR WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");
                Days = new roTime();
                Days = roTypes.Any2Time(startDate);
                while (Days.ValueDateTime <= endDate)
                {
                    // Borramos los registros anteriores
                    Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO TMPMONTHLYEMPLOYEECALENDAR VALUES (" + Days.SQLSmallDateTime() + "," + IDReportTask.ToString() + ")");
                    Days = Days.Add(1, "d");
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpMonthyCalendar");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpMonthyCalendar");
            }
            finally
            {
            }
        }

        //Se usa!
        private void CreateTmpMonthyCalendarV2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            /////////////////////////////////////////////                 
            int monthStart = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("monthStart")).FirstOrDefault().Value)));
            int yearStart = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("yearStart")).FirstOrDefault().Value)));
            int monthEnd = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("monthEnd")).FirstOrDefault().Value)));
            int yearEnd = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("yearEnd")).FirstOrDefault().Value)));
            DateTime startDate = new DateTime(yearStart, monthStart, 1);
            DateTime endDate = new DateTime(yearEnd, monthEnd, DateTime.DaysInMonth(yearEnd, monthEnd));

            /////////////////////////////////////////////

            roTime Days;

            try
            {
                // Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPMONTHLYEMPLOYEECALENDAR WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");
                Days = new roTime();
                Days = roTypes.Any2Time(startDate);
                while (Days.ValueDateTime <= endDate)
                {
                    // Borramos los registros anteriores
                    Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO TMPMONTHLYEMPLOYEECALENDAR VALUES (" + Days.SQLSmallDateTime() + "," + IDReportTask.ToString() + ")");
                    Days = Days.Add(1, "d");
                }
            }
            catch (DbException ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpMonthyCalendarV2::DBEX::", ex);
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpMonthyCalendar");
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpMonthyCalendarV2::", ex);
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpMonthyCalendar");
            }
            finally
            {
            }
        }

        private void CreateTmpShiftDefinitions(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string sSQL;
            string[] iShift;
            int i;
            Robotics.Base.VTBusiness.Shift.roShift oShift;

            try
            {
                iShift = new string[Information.UBound(Strings.Split((parametersList.Where(x => x.Name.Equals("Shift")).FirstOrDefault().Value).ToString(), ",")) + 1];
                iShift = Convert.ToString(((parametersList.Where(x => x.Name.Equals("Shift")).FirstOrDefault().Value))).Split(',');
                sSQL = "@DELETE# FROM TMPSHifTDEFINITIONS WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0";
                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                var loopTo = Information.UBound(iShift);

                for (i = 0; i <= loopTo; i++)
                {
                    oShift = new roShift(Convert.ToInt32(iShift[i]), new Robotics.Base.VTBusiness.Shift.roShiftState(), false);

                    sSQL = "@INSERT# INTO TMPSHifTDEFINITIONS";
                    sSQL = sSQL + " VALUES (" + iShift[i] + ", '" + oShift.GetLayers().Replace("'", "´") + "', '" + oShift.GetZones().Replace("'", "´") + "', '" + oShift.GetRules().Replace("'", "´") + "'," + IDReportTask.ToString() + ")";
                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpShiftDefinitions");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpShiftDefinitions");
            }
            finally
            {
            }
        }

        public void CreateTmpTop(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            // Llena la tabla temporal con los datos de la consulta

            string Concepts = string.Empty;
            var Condition = string.Empty;
            string TypePeriod = string.Empty;
            double ConditionValue;

            try
            {
                // Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM TMPTOP WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Seteamos las variables a utilizar
                Concepts = ((parametersList.Where(x => x.Name.Equals("Concept")).FirstOrDefault().Value)).ToString();

                //Causes = ((parametersList.Where(x => x.Name.Equals("Cause")).FirstOrDefault().Value)).ToString();
                //Incidences = ((parametersList.Where(x => x.Name.Equals("Incidence")).FirstOrDefault().Value)).ToString();
                //TypePeriod = ((parametersList.Where(x => x.Name.Equals("IncPlusAccPeriod")).FirstOrDefault().Value)).ToString();
                //"Daily", "Period"

                TypePeriod = ((parametersList.Where(x => x.Name.Equals("TypePeriod")).FirstOrDefault().Value)).ToString();

                //"More, Less";
                Condition = ((parametersList.Where(x => x.Name.Equals("Condition")).FirstOrDefault().Value)).ToString();
                ConditionValue = roTypes.Any2Double(((parametersList.Where(x => x.Name.Equals("ConditionValue")).FirstOrDefault().Value)).ToString());

                string sSQL = "";
                DataTable aDS = null;
                DataTable bDS = null;
                //DataTable cDS = null;

                // Abrimos la tabla temporal para llenarla de los datos del informe
                bDS = new DataTable("TMPTOP");
                sSQL = "@SELECT# * FROM TMPTOP WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0";
                var cmd = Robotics.DataLayer.AccessHelper.CreateCommand(sSQL);
                var da = Robotics.DataLayer.AccessHelper.CreateDataAdapter(cmd, true);
                da.Fill(bDS);

                // Empleados seleccionados
                string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector") && x.Name.Equals("Employees")).FirstOrDefault().Value.ToString().Split('@');

                // Recogemos el nombre del acumulado
                string ConceptName = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# Name FROM Concepts WHERE ID = " + Concepts));

                string sSQLGroupDaily = "";
                if (TypePeriod == "Daily")
                {
                    sSQL = "@SELECT# ds.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, ds.Date, c.ID IDConcept, c.Name ConceptName, Value Total ";
                    sSQL = sSQL + " FROM DailySchedule ds WITH (NOLOCK)";
                    sSQL = sSQL + " INNER JOIN DailyAccruals da WITH (NOLOCK) ON ds.Date = da.Date AND ds.IDEmployee = da.IDEmployee";
                    sSQL = sSQL + " INNER JOIN Concepts c WITH (NOLOCK) ON c.ID = da.IDConcept";
                    sSQL = sSQL + " INNER JOIN sysrovwAllEmployeeGroups WITH (NOLOCK) ON sysrovwAllEmployeeGroups.IDEmployee = da.IDEmployee";
                    sSQL = sSQL + " WHERE IDConcept IN (" + Concepts + ")";
                    sSQL = sSQL + " AND da.Date BETWEEN " + roTypes.Any2Time(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0]).SQLSmallDateTime() + " AND " + roTypes.Any2Time(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[1]).SQLSmallDateTime();
                    sSQL = sSQL + " AND " + roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]).Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");

                    sSQLGroupDaily = " GROUP BY ds.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, ds.Date, c.ID, c.Name, da.Value";
                    sSQLGroupDaily = sSQLGroupDaily + " ORDER BY ds.Date";
                }

                if (Condition == "Less")
                {
                }

                aDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                if (aDS != null)
                {
                    if (bDS != null)
                    {
                        // Para cada registro encontrado lo introducimos en la tabla temporal
                        DataRow bRow = null;
                        foreach (DataRow aRow in aDS.Rows)
                        {
                            bRow = bDS.NewRow();

                            bRow["IDEmployee"] = roTypes.Any2Double(aRow["IDEmployee"]);
                            bRow["EmployeeName"] = roTypes.Any2String(aRow["EmployeeName"]);
                            bRow["IDReportTask"] = IDReportTask;

                            bRow["IDPrimaryConcept"] = roTypes.Any2Double(Concepts);
                            bRow["ConceptPrimaryName"] = ConceptName;
                            bRow["BeginTime"] = new DateTime(1899, 12, 30, 0, 0, 0);
                            bRow["EndTime"] = new DateTime(1899, 12, 30, 0, 0, 0);

                            bRow["Date"] = roTypes.Any2Time(aRow["Date"]).DateOnly().ToString();
                            bRow["Value"] = roTypes.Any2Double(aRow["Total"]);

                            // Actualizamos la tabla
                            bDS.Rows.Add(bRow);
                        }
                    }
                }

                // Guardamos los cambios
                da.Update(bDS);
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpTop");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpTop");
            }
            finally
            {
            }
        }

        public void CreateTmpEmergency(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            CreateTmpEmergencyWithAttendance(IDReportTask, parametersList, originalParametersList);
        }

        public bool CreateTmpEmergencyWithAttendance(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            bool bolRet = false;
            int IdEmployee;

            try
            {
                var oParams = new roParameters("OPTIONS", true);

                // Obtener el campo que contiene el nombre del campo de la ficha para agrupar
                string UsrField = roTypes.Any2String(oParams.get_Parameter(Parameters.EvacuationPointUsrField));

                // Obtener el campo que contiene el nombre del campo de horas de comprobacion
                // Dim MaxTime As String = roTypes.Any2Time(oParams.Parameter(roParameters.Parameters.EmergencyMaxHours)).Minutes <---en live no existe este parametro falta por agregar
                long MaxTime = 2880;

                roEmployeeUserField fld;
                var oEmployeeState = new roUserFieldState(((int)(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value)));

                // crear datos en la tabla
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // **** EMPLEADOS PRESENTES *********
                string sSQL = "@SELECT# ID FROM Employees INNER JOIN sysrovwCurrentEmployeesPresenceStatusPunches ON sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee = Employees.ID " + "WHERE Status = 'IN' AND DateTime >= DateAdd(n, -" + Convert.ToString(MaxTime) + ", GETDATE()) AND " + "(DateTime > (@SELECT# TOP 1 DateTime FROM sysrovwCurrentEmployeesPresenceStatusPunches srceps WHERE srceps.IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee AND Status= 'OUT' ORDER BY DateTime DESC) OR " + "Status = (@SELECT# TOP 1 Status FROM sysrovwCurrentEmployeesPresenceStatusPunches srceps WHERE srceps.IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee ORDER BY DateTime DESC))";

                // Dim tbAux As New DataTable("Employees")
                // Dim cmd As DbCommand = CreateCommand(sSQL)
                // Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                // da.SelectCommand.CommandTimeout = 0
                // da.Fill(tbAux)

                var tbAux = Robotics.DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(sSQL);

                if (tbAux != null)
                {
                    foreach (DataRow oRow in tbAux.Rows)
                    {
                        IdEmployee = roTypes.Any2Integer(oRow["ID"]);
                        fld = Robotics.Base.VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(Convert.ToString(IdEmployee), UsrField, DateTime.Today, oEmployeeState, false);
                        if (!(fld == null))
                            sSQL = "@INSERT# INTO TMPEmergency VALUES (" + Convert.ToString(IdEmployee) + ",'" + roTypes.Any2String(fld.FieldValue) + "', 'Present'," + IDReportTask.ToString() + ")";
                        else
                            sSQL = "@INSERT# INTO TMPEmergency VALUES (" + Convert.ToString(IdEmployee) + ",'', 'Present'," + IDReportTask.ToString() + ")";
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }
                }

                // **** EMPLEADOS AUSENTES ************
                // sSQL = "@SELECT# ID FROM Employees INNER JOIN sysrovwCurrentEmployeesPresenceStatusPunches ON sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee = Employees.ID WHERE DateTime < DateAdd(n, -" & MaxTime & ", GETDATE()) OR (Status = 'OUT' " +
                // "AND (DateTime > (@SELECT# TOP 1 DateTime FROM sysrovwCurrentEmployeesPresenceStatusPunches srceps WHERE srceps.IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee AND Status= 'IN' ORDER BY DateTime DESC) " +
                // "OR Status = (@SELECT# TOP 1 Status FROM sysrovwCurrentEmployeesPresenceStatusPunches srceps WHERE srceps.IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee ORDER BY DateTime DESC)) " +
                // "OR DateTime IS NULL)"

                sSQL = "@SELECT# DISTINCT Employees.ID FROM Employees INNER JOIN sysrovwCurrentEmployeesPresenceStatusPunches ON sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee = Employees.ID INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " + "WHERE (sysrovwCurrentEmployeesPresenceStatusPunches.DateTime < DATEADD(n, -" + Convert.ToString(MaxTime) + ", GETDATE())) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) OR " + "(sysrovwCurrentEmployeesPresenceStatusPunches.DateTime > (@SELECT# TOP (1) DateTime FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE (IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) AND (Status = 'IN') " + "ORDER BY DateTime DESC)) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') OR " + "(GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') AND " + "(sysrovwCurrentEmployeesPresenceStatusPunches.Status = (@SELECT# TOP (1) Status FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE(IdEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) " + "ORDER BY DateTime DESC)) OR (sysrovwCurrentEmployeesPresenceStatusPunches.DateTime IS NULL) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate)";

                // Dim tbAbsences As New DataTable("EmployeesAbsence")
                // Dim cmdAbsences As DbCommand = CreateCommand(sSQL)
                // Dim daAbsences As DbDataAdapter = CreateDataAdapter(cmdAbsences, True)
                // daAbsences.SelectCommand.CommandTimeout = 0
                // daAbsences.Fill(tbAbsences)

                var tbAbsences = Robotics.DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(sSQL);

                if (tbAbsences != null)
                {
                    foreach (DataRow oRow in tbAbsences.Rows)
                    {
                        IdEmployee = roTypes.Any2Integer(oRow["ID"]);

                        fld = Robotics.Base.VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(Convert.ToString(IdEmployee), UsrField, DateTime.Today, oEmployeeState, false);
                        if (!(fld == null))
                            sSQL = "@INSERT# INTO TMPEmergency VALUES (" + Convert.ToString(IdEmployee) + ",'" + roTypes.Any2String(fld.FieldValue) + "', 'Absent'," + IDReportTask.ToString() + ")";
                        else
                            sSQL = "@INSERT# INTO TMPEmergency VALUES (" + Convert.ToString(IdEmployee) + ",'', 'Absent'," + IDReportTask.ToString() + ")";
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }
                }

                bolRet = true;
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpEmergencyWithAttendance");
            }
            finally
            {
            }

            return bolRet;
        }

        //Se usa
        private void CreateTmpDailyPartialAccruals(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList, bool withZeros, bool useUserField = false)
        {
            ////////////////////////////////////////////////////
            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            DateTime startDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime endDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }
            string empFilter = string.Join(",", empSelected);
            ////////////////////////////////////////////////////

            string sSQL;
            double newEmployee;
            var oldEmployee = default(double);
            DateTime newDate;
            var oldDate = default(DateTime);
            double newGroup;
            var oldGroup = default(double);

            DataTable aDS = null;
            DataTable bDS = null;

            string idContract = string.Empty;

            try
            {
                // Primero creamos el calendario
                CreateTmpMonthyCalendar(IDReportTask, parametersList, originalParametersList);

                // Eliminamos los datos de la tabla temporal para introducir más tarde los nuevos
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDailyPartialAccruals WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Recogemos a los empleados seleccionamos
                sSQL = "@SELECT# DISTINCT(sysroEmployeeGroups.IDEmployee), sysroEmployeeGroups.IDGroup, sysroEmployeeGroups.FullGroupName, Employees.Name EmployeeName, sysroEmployeeGroups.GroupName FROM sysroEmployeeGroups";
                sSQL = sSQL + " INNER JOIN Employees ON Employees.ID = sysroEmployeeGroups.IDEmployee";
                if (IDPassport > 0 && empFilter != "")
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And cast(" + roTypes.Any2Time(startDate).SQLSmallDateTime() + " as date) between cast(poe.BeginDate as date) And cast(poe.EndDate as date) " +
                        " AND poe.IDEmployee IN (" + empFilter + ") " +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1 ";
                }
                sSQL = sSQL + " WHERE " + roTypes.Any2Time(startDate).SQLSmallDateTime() + " <= sysroEmployeeGroups.EndDate";
                sSQL = sSQL + " AND " + roTypes.Any2Time(endDate).SQLSmallDateTime() + " >= sysroEmployeeGroups.BeginDate";
                if (empFilter == "")
                {
                    sSQL = sSQL + " AND sysroEmployeeGroups.IDEmployee IN(-1) ";
                }
                sSQL = sSQL + " order by Employees.Name";

                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                foreach (DataRow bRow in bDS.Rows)
                {
                    // Preparamos la consulta
                    sSQL = GetSSqlTmpDailyPartialAccruals(bRow, withZeros, useUserField, IDReportTask, parametersList);
                    aDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                    if (aDS.Rows.Count == 0)
                    {
                        sSQL = "@INSERT# INTO TMPDailyPartialAccruals (FullGroupName, IDGroup, GroupName, IDEmployee, EmployeeName, Acum1, idContract,IDReportTask, IDReportGroup, Position)";
                        sSQL = sSQL + " VALUES ('" + Strings.Replace(roTypes.Any2String(bRow["FullGroupName"]), "'", "''") + "',";
                        sSQL = sSQL + Convert.ToString(roTypes.Any2Double(bRow["IDGroup"])) + ",'" + Strings.Replace(roTypes.Any2String(bRow["GroupName"]), "'", "''") + "',";
                        sSQL = sSQL + Convert.ToString(roTypes.Any2Double(bRow["IDEmployee"])) + ",'" + Strings.Replace(roTypes.Any2String(bRow["EmployeeName"]), "'", "''") + "',";
                        sSQL = sSQL + "0, '" + Strings.Replace(idContract, "'", "''") + "'," + IDReportTask.ToString() + ", " + int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptGroupsSelector")).FirstOrDefault()?.Value.ToString()) + ",1)";
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }
                    string idConceptsAnt = "";
                    string strIdConcepts = "";
                    foreach (DataRow aRow in aDS.Rows)
                    {
                        if (string.IsNullOrEmpty(idConceptsAnt))
                        {
                            strIdConcepts = Convert.ToString(aRow["idconcept"] + "@") + aRow["position"];
                            idConceptsAnt = aRow["idconcept"].ToString();
                        }
                        else if (!idConceptsAnt.Equals(aRow["idconcept"]))
                        {
                            if (!strIdConcepts.ToString().Contains(aRow["idconcept"].ToString()))
                            {
                                strIdConcepts = Convert.ToString(strIdConcepts + " - " + aRow["idconcept"] + "@") + aRow["position"];
                                string sSQLAux = "@UPDATE# TMPDailyPartialAccruals SET idConcepts = '" + strIdConcepts + "' WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0";
                                if (useUserField)
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLAux);
                            }
                            idConceptsAnt = aRow["idconcept"].ToString();
                        }

                        newEmployee = roTypes.Any2Double(aRow["IDEmployee"]);
                        newDate = Convert.ToDateTime(roTypes.Any2Time(aRow["Days"]).DateOnly());
                        newGroup = roTypes.Any2Double(aRow["IDGroup"]);

                        if (roTypes.Any2Double(aRow["Value"]) != 0 & !withZeros & (newEmployee != oldEmployee | newDate != oldDate | newGroup != oldGroup) | withZeros & (newEmployee != oldEmployee | newDate != oldDate | newGroup != oldGroup))
                        {
                            // Busca el contrato
                            sSQL = "@SELECT# TOP (1) IDContract " + "FROM .EmployeeContracts " + "WHERE (IDEmployee=" + aRow["IDEmployee"] + ") AND (BeginDate <= " + roTypes.Any2Time(aRow["Days"]).SQLSmallDateTime() + ") AND " + "(EndDate >= " + roTypes.Any2Time(aRow["Days"]).SQLSmallDateTime() + ") ";
                            idContract = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                            // Si es un nuevo empleado creamos la linia
                            sSQL = "@INSERT# INTO TMPDailyPartialAccruals (FullGroupName, IDGroup, GroupName, Position, IDReportGroup,";
                            sSQL = sSQL + " TotalValueGroup, Path, IDEmployee, EmployeeName, TotalValueEmployee, Day, ShiftName,";
                            sSQL = sSQL + " ExpectedWorkingHours, Acum" + Convert.ToString(roTypes.Any2Double(aRow["Position"])) + ", idContract, IDReportTask " + (!useUserField ? "" : ",userFieldValue,idConcepts") + ")";
                            sSQL = sSQL + " VALUES ('" + Strings.Replace(roTypes.Any2String(aRow["FullGroupName"]), "'", "''") + "',";
                            sSQL = sSQL + Convert.ToString(roTypes.Any2Double(aRow["IDGroup"])) + ",'" + Strings.Replace(roTypes.Any2String(aRow["GroupName"]), "'", "''") + "',";
                            sSQL = sSQL + Convert.ToString(roTypes.Any2Double(aRow["Position"])) + "," + Convert.ToString(roTypes.Any2Double(aRow["IDReportGroup"])) + ",";
                            sSQL = sSQL + Strings.Replace(Convert.ToString(roTypes.Any2Double(aRow["TotalValueGroup"])), ",", ".") + ",'" + roTypes.Any2String(aRow["Path"]) + "',";
                            sSQL = sSQL + Convert.ToString(roTypes.Any2Double(aRow["IDEmployee"])) + ",'" + Strings.Replace(roTypes.Any2String(aRow["Name"]), "'", "''") + "',";
                            sSQL = sSQL + Strings.Replace(Convert.ToString(roTypes.Any2Double(aRow["TotalValueEmployee"])), ",", ".") + "," + roTypes.Any2Time(aRow["Days"]).SQLSmallDateTime() + ",";
                            sSQL = sSQL + "'" + Strings.Replace(roTypes.Any2String(aRow["ShiftName"]), "'", "''") + "'," + Strings.Replace(Convert.ToString(roTypes.Any2Double(aRow["ExpectedWorkingHours"])), ",", ".") + ",";
                            sSQL = sSQL + Strings.Replace(Convert.ToString(roTypes.Any2Double(aRow["value"])), ",", ".") + ", '" + Strings.Replace(idContract, "'", "''") + "'," + IDReportTask.ToString();
                            if (useUserField)
                            {
                                if (aRow["FieldValue"] != null && !string.IsNullOrEmpty(roTypes.Any2String(aRow["FieldValue"])))
                                    sSQL = sSQL + "," + roTypes.Any2String(aRow["FieldValue"]);
                                else
                                    sSQL = sSQL + ",''";

                                sSQL = sSQL + ",'" + strIdConcepts + "'";
                            }
                            sSQL = sSQL + ")";
                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                        else if (roTypes.Any2Double(aRow["Value"]) != 0 & !withZeros | withZeros)
                        {
                            // Si ya existe actualizamos la linia solo si tiene un valor distinto a 0
                            sSQL = "@UPDATE# TMPDailyPartialAccruals SET Acum" + Convert.ToString(roTypes.Any2Double(aRow["Position"])) + " = " + Strings.Replace(Convert.ToString(roTypes.Any2Double(aRow["value"])), ",", ".") + " WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Long(aRow["IDEmployee"])) + " AND Day = " + roTypes.Any2Time(aRow["Days"]).SQLSmallDateTime() + " and IDReportTask=" + IDReportTask.ToString();

                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }

                        // Actualizamos las variables
                        oldGroup = newGroup;
                        oldEmployee = newEmployee;
                        oldDate = newDate;
                    }
                }
            }
            catch (Exception ex)
            {

                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpDailyPartialAccruals::", ex);
            }
            finally
            {
            }
        }

        private string GetSSqlTmpDailyPartialAccruals(DataRow bRow, bool withZeros, bool useUserField, int IDReportTask, List<ReportParameter> parametersList)
        {
            ////////////////////////////////////////////////////
            DateTime startDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime endDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;
            int conceptGroup = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptGroupsSelector")).FirstOrDefault()?.Value.ToString());
            ////////////////////////////////////////////////////

            string sSQL;
            var StartDateSQLConvert = roTypes.Any2Time(startDate).SQLSmallDateTime();
            var EndDateSQLConvert = roTypes.Any2Time(endDate).SQLSmallDateTime();
            //!useUserField ? parametersList.Where(x => x.Name.Equals("Concept")).FirstOrDefault().Value : "@SELECT# idconcept from sysroReportGroupConcepts where IDReportGroup = " + parametersList.Where(x => x.Name.Equals("ConceptGroups")).FirstOrDefault().Value;
            var Concept = "@SELECT# idconcept from sysroReportGroupConcepts where IDReportGroup = " + conceptGroup;
            var sSQLAux = ",(@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " + Convert.ToString(roTypes.Any2Double(bRow["IDEmployee"])) + @" AND
                            EmployeeUserFieldValues.FieldName = '" + "19 jornada"//int.Parse(((Robotics.Base.conceptGroupsSelector[])parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptGroupsSelector")).FirstOrDefault().Value).FirstOrDefault().Value)
                            + @"' AND EmployeeUserFieldValues.Date <= tmp.Days
                            ORDER BY EmployeeUserFieldValues.Date DESC) FieldValue";

            sSQL = "@SELECT# sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.idgroup, sysroEmployeeGroups.groupname,position,";
            sSQL = sSQL + " IDReportGroup, isnull(abs(totalvalue),0) as TotalValueGroup, sysroEmployeeGroups.path, employees.name,";
            sSQL = sSQL + " isnull(abs(totalemp),0) as TotalValueEmployee, tmp.idemployee, tmp.days, shifts.name as shiftname,";
            sSQL = sSQL + " (case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end) as ExpectedWorkingHours, tmp.idconcept, concepts.name as conceptname, concepts.shortname,";
            sSQL = sSQL + " isnull(dailyaccruals.Value, 0) As Value ";
            sSQL = sSQL + (!useUserField ? " FROM" : sSQLAux + " FROM");
            sSQL = sSQL + " (@SELECT# id as idemployee, idconcept, days, Position,IDReportGroup";
            sSQL = sSQL + " from tmpMonthlyEmployeeCalendar WITH (NOLOCK), sysroreportgroupconcepts WITH (NOLOCK), employees WITH (NOLOCK)";
            sSQL = Convert.ToString(sSQL + " where TMPMONTHLYEMPLOYEECALENDAR.IDReportTask = " + IDReportTask.ToString() + " AND days between " + StartDateSQLConvert + " and ") + EndDateSQLConvert;
            sSQL = sSQL + " AND IDConcept IN (" + Concept + ")) tmp";
            sSQL = sSQL + " left outer join dailyaccruals WITH (NOLOCK) on dailyaccruals.idconcept = tmp.idconcept";
            sSQL = sSQL + " and tmp.days = dailyaccruals.date and tmp.idemployee = dailyaccruals.idemployee";
            sSQL = sSQL + " inner join employees WITH (NOLOCK) on tmp.idemployee=employees.id";
            sSQL = sSQL + " inner join concepts WITH (NOLOCK) on tmp.idconcept = concepts.id";
            sSQL = sSQL + " inner join sysroEmployeeGroups WITH (NOLOCK) on sysroEmployeeGroups.IDemployee=tmp.idemployee";
            sSQL = sSQL + " AND tmp.days between sysroEmployeeGroups.begindate and sysroEmployeeGroups.enddate";
            sSQL = sSQL + " left outer join dailyschedule WITH (NOLOCK) on tmp.days = dailyschedule.date";
            sSQL = sSQL + " and tmp.idemployee = dailyschedule.idemployee";
            sSQL = sSQL + " left join shifts WITH (NOLOCK) on dailyschedule.idshift1 = shifts.id";
            sSQL = sSQL + " left outer join (@SELECT# isnull(sum(abs(value)),0) as totalvalue, aeg.idgroup as idgroup";
            sSQL = sSQL + " from dailyaccruals da inner join sysroEmployeeGroups aeg on da.idemployee=aeg.idemployee";
            sSQL = Convert.ToString(sSQL + " where date between " + StartDateSQLConvert + " and ") + EndDateSQLConvert;
            sSQL = sSQL + " and idconcept in (" + Concept + ") group by aeg.idgroup) totalvaluegroup";
            sSQL = sSQL + " on sysroEmployeeGroups.idgroup=totalvaluegroup.idgroup";
            sSQL = sSQL + " left join (@SELECT# isnull(sum(abs(value)),0) as totalemp, idemployee, date";
            sSQL = Convert.ToString(sSQL + " From dailyaccruals where date between " + StartDateSQLConvert + " and ") + EndDateSQLConvert;
            sSQL = sSQL + " and idconcept in (" + Concept + ")";
            sSQL = sSQL + " group by idemployee, date) totalvalueemployee on totalvalueemployee.idemployee=tmp.idemployee and totalvalueemployee.date=tmp.days";
            sSQL = sSQL + " WHERE sysroEmployeeGroups.IDEmployee = " + Convert.ToString(roTypes.Any2Double(bRow["IDEmployee"])) + " AND sysroEmployeeGroups.IDGroup = " + Convert.ToString(roTypes.Any2Double(bRow["IDGroup"])) + " AND " + StartDateSQLConvert + " <= sysroEmployeeGroups.EndDate";
            sSQL = sSQL + " AND " + EndDateSQLConvert + " >= sysroEmployeeGroups.BeginDate";
            sSQL = sSQL + " AND Position > 0 AND IDReportGroup = " + conceptGroup;
            if (!withZeros)
                sSQL = sSQL + " AND TotalEmp <> 0 AND TotalValue <> 0 AND Value <> 0";

            sSQL = sSQL + " ORDER BY sysroEmployeeGroups.GroupName, Employees.Name, tmp.days, Position";
            return sSQL;
        }

        private void CreateTmpControlHolidays(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            // Llenamos la tabla del informe de control de vacaciones

            string sSQL;
            DataTable aDS = null;
            DataTable bDS = null;
            //DataRow aRow = null;
            DataRow bRow = null;

            double StartupValue;
            double CarryOver;
            string ConceptRule;
            int CauseRule;
            string Concepts = string.Empty;
            string Contract;
            string AuxConcepts;
            double StartupBalance;
            int EnjoyedDays;
            int CurrentBalance;
            int DaysProvided;
            int EndingBalance;
            string oldDate;
            string tmpDate;
            string newDate;
            string LineEnjoyed;
            string LineProvided;
            string Shifts = string.Empty;

            try
            {
                // Borramos los datos que puedan haber
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPCONTROLHOLIDAYS WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Buscamos todos los horarios que tienen informado el VAC=TRUE en las propiedades avanzadas
                sSQL = "@SELECT# * FROM Shifts WHERE IDConceptBalance > 0 AND ShiftType=2";
                aDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                foreach (DataRow ARow in aDS.Rows)
                {
                    // Creamos una cadena con todos los conceptosPara cada horario buscamos su concepto asociado
                    Concepts = Concepts + roTypes.Any2String(ARow["IDConceptBalance"]) + ",";
                    Shifts = Shifts + roTypes.Any2String(ARow["ID"]) + ",";
                }

                string[] empFilter = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector") && x.Name.Equals("Employees")).FirstOrDefault().Value.ToString().Split('@');

                // Recortamos la ultima coma
                if (Strings.Len(Concepts) > 0)
                {
                    Concepts = Strings.Left(Concepts, Strings.Len(Concepts) - 1);

                    if (Strings.Len(Shifts) > 0)
                        Shifts = Strings.Left(Shifts, Strings.Len(Shifts) - 1);

                    // Buscamos los empleados seleccionados
                    sSQL = "@SELECT# * FROM sysrovwAllEmployeeGroups WHERE " + roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]).Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");
                    aDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                    foreach (DataRow ARow in aDS.Rows)
                    {
                        // Inicializamos las variables
                        StartupValue = 0;
                        CarryOver = 0;
                        ConceptRule = "";
                        CauseRule = 0;
                        StartupBalance = 0;
                        EnjoyedDays = 0;
                        CurrentBalance = 0;
                        DaysProvided = 0;
                        EndingBalance = 0;

                        // ***************** Buscamos el valor iniciales de los acumulados seleccionados **************
                        sSQL = "@SELECT# ISNULL(SUM(StartupValue),0) Value FROM EmployeeConceptAnnualLimits";
                        sSQL = sSQL + " WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"])) + " ";
                        sSQL = sSQL + " AND IDConcept IN (" + Concepts + ") AND IDYear='" + Convert.ToString(DateAndTime.Year(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0].ToString()))) + "'";
                        StartupValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                        // *********************** Buscamos el valor de arrastre si tiene *****************************
                        sSQL = "@SELECT# IDAccrualsRules FROM EmployeeAccrualsRules WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                        bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        AuxConcepts = "," + Concepts + ",";

                        foreach (DataRow BRow in bDS.Rows)
                        {
                            ConceptRule = "," + roTypes.Any2String(GetValueFromAccrualRule(roTypes.Any2Double(BRow["IDAccrualsRules"]), "MainAccrual")) + ",";

                            // Buscamos si el acumulado de la regla coincide con el del horario
                            if (Strings.InStr(AuxConcepts, ConceptRule) > 0)
                            {
                                CauseRule = Convert.ToInt32(roTypes.Any2Double(GetValueFromAccrualRule(roTypes.Any2Double(BRow["IDAccrualsRules"]), "DestiAccrual")));

                                // Buscamos el valor de arrastre
                                sSQL = "@SELECT# ISNULL(Value,0) FROM DailyCauses WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                                sSQL = sSQL + " AND Date = " + roTypes.Any2Time(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0].ToString())).SQLSmallDateTime();
                                sSQL = sSQL + " AND IDCause = " + Convert.ToString(CauseRule) + " AND AccrualsRules=1";
                                CarryOver = CarryOver + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                            }
                        }

                        // ******************************** Calculamos el saldo inicial *******************************
                        StartupBalance = StartupValue + CarryOver;

                        // ************************* Buscamos el total de dias disfrutados ***********************
                        newDate = "";
                        tmpDate = "";
                        oldDate = "";
                        LineEnjoyed = "";

                        // 1.- Buscamos el valor total de dias disfrutados
                        sSQL = "@SELECT# ISNULL(COUNT(*),0) FROM DailySchedule INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShiftUsed";
                        sSQL = sSQL + " WHERE Date BETWEEN " + roTypes.Any2Time(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0].ToString())).SQLSmallDateTime();
                        sSQL = sSQL + " AND " + roTypes.Any2Time(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[1].ToString())).SQLSmallDateTime();
                        sSQL = sSQL + " AND IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                        sSQL = sSQL + " AND Shifts.ID IN (" + Shifts + ")";
                        EnjoyedDays = Convert.ToInt32(roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)));

                        // 2.- Buscamos los dias disfrutados
                        sSQL = "@SELECT# Date FROM DailySchedule INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShiftUsed";
                        sSQL = sSQL + " WHERE Date BETWEEN " + roTypes.Any2Time(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[0].ToString())).SQLSmallDateTime();
                        sSQL = sSQL + " AND " + roTypes.Any2Time(DateAndTime.DateValue(((DateTime[])(parametersList.Where(x => x.Type.Equals("Robotics.Base.datePeriodSelector") && x.Name.Equals("PeriodDate")).FirstOrDefault().Value))[1].ToString())).SQLSmallDateTime();
                        sSQL = sSQL + " AND IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                        sSQL = sSQL + " AND Shifts.ID IN (" + Shifts + ")";
                        sSQL = sSQL + " ORDER BY Date";
                        bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        for (int iRow = 0, loopTo = bDS.Rows.Count - 1; iRow <= loopTo; iRow++)
                        {
                            bRow = bDS.Rows[iRow];

                            newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();

                            // La primera vez no entramos
                            if (Strings.Len(oldDate) > 0)
                            {
                                if (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, oldDate))
                                {
                                    while (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                    {
                                        tmpDate = newDate;
                                        if (iRow < bDS.Rows.Count - 1)
                                            newDate = roTypes.Any2Time(bDS.Rows[iRow + 1]["Date"]).DateOnly().ToString();
                                    }
                                    LineEnjoyed = LineEnjoyed + oldDate + " - " + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;
                                    newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                }
                                else
                                {
                                    oldDate = newDate;
                                    tmpDate = newDate;

                                    // Puede ser que su siguiente sea consecutivo
                                    if (iRow < bDS.Rows.Count - 1)
                                    {
                                        iRow += 1;
                                        bRow = bDS.Rows[iRow];
                                    }

                                    newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    if (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                    {
                                        while (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                        {
                                            tmpDate = newDate;

                                            if (iRow < bDS.Rows.Count - 1)
                                            {
                                                iRow += 1;
                                                bRow = bDS.Rows[iRow];
                                            }

                                            newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                        }

                                        LineEnjoyed = LineEnjoyed + oldDate + " - " + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;

                                        iRow -= 1;
                                        bRow = bDS.Rows[iRow];

                                        newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    }
                                    else
                                    {
                                        LineEnjoyed = LineEnjoyed + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;

                                        iRow -= 1;
                                        bRow = bDS.Rows[iRow];

                                        newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    }
                                }
                            }

                            oldDate = newDate;
                            tmpDate = newDate;
                        }

                        // ******************************* Buscamos el saldo actual ******************************
                        CurrentBalance = Convert.ToInt32(StartupBalance - EnjoyedDays);

                        // ******************************* Buscamos los dias previstos ***************************
                        // 1.- Buscamos el total de dias previstos
                        sSQL = "@SELECT# ISNULL(COUNT(Date),0) Value FROM DailySchedule";
                        sSQL = sSQL + " INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1";
                        sSQL = sSQL + " WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                        sSQL = sSQL + " AND Date > GETDATE()";
                        // sSQL = sSQL & " AND IDConceptBalance IN (" & Concepts & ")"
                        sSQL = sSQL + " AND Shifts.ID IN (" + Shifts + ")";
                        DaysProvided = Convert.ToInt32(roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)));

                        // 2.- Buscamos los dias previstos
                        sSQL = "@SELECT# Date FROM DailySchedule";
                        sSQL = sSQL + " INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1";
                        sSQL = sSQL + " WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"]));
                        sSQL = sSQL + " AND Date > GETDATE()";
                        // sSQL = sSQL & " AND IDConceptBalance IN (" & Concepts & ")"
                        sSQL = sSQL + " AND Shifts.ID IN (" + Shifts + ")";
                        sSQL = sSQL + " ORDER BY Date ASC";
                        bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        newDate = "";
                        tmpDate = "";
                        oldDate = "";
                        LineProvided = "";

                        for (int iRow = 0, loopTo1 = bDS.Rows.Count - 1; iRow <= loopTo1; iRow++)
                        {
                            bRow = bDS.Rows[iRow];

                            newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();

                            // La primera vez no entramos
                            if (Strings.Len(oldDate) > 0)
                            {
                                if (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, oldDate))
                                {
                                    while (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                    {
                                        tmpDate = newDate;

                                        iRow += 1;
                                        if (iRow < bDS.Rows.Count - 1)
                                        {
                                            bRow = bDS.Rows[iRow];
                                            newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                        }
                                    }
                                    LineProvided = LineProvided + oldDate + " - " + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;
                                    iRow -= 1;
                                    bRow = bDS.Rows[iRow];
                                    newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                }
                                else
                                {
                                    oldDate = newDate;
                                    tmpDate = newDate;

                                    // Puede ser que su siguiente sea consecutivo
                                    if (iRow < bDS.Rows.Count - 1)
                                    {
                                        iRow += 1;
                                        bRow = bDS.Rows[iRow];
                                    }

                                    newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    if (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                    {
                                        while (Convert.ToDateTime(newDate) == DateAndTime.DateAdd("d", 1, tmpDate))
                                        {
                                            tmpDate = newDate;

                                            iRow += 1;
                                            if (iRow < bDS.Rows.Count - 1)
                                            {
                                                bRow = bDS.Rows[iRow];
                                                newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                            }
                                        }
                                        LineProvided = LineProvided + oldDate + " - " + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;

                                        iRow -= 1;
                                        bRow = bDS.Rows[iRow];

                                        newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    }
                                    else
                                    {
                                        LineProvided = LineProvided + tmpDate + Microsoft.VisualBasic.Constants.vbNewLine;

                                        iRow -= 1;
                                        bRow = bDS.Rows[iRow];

                                        newDate = roTypes.Any2Time(bRow["Date"]).DateOnly().ToString();
                                    }
                                }
                            }

                            oldDate = newDate;
                            tmpDate = newDate;
                        }

                        // *************************** Buscamos el saldo final previsto ***************************
                        EndingBalance = CurrentBalance - DaysProvided;

                        // Buscamos el contrato
                        Contract = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# TOP 1 IDContract FROM EmployeeContracts WHERE IDEmployee = " + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"])) + " ORDER BY BeginDate DESC"));

                        // *************************** INSERTAMOS TODOS LOS DATOS EN LA TABLA TEMPORAL
                        sSQL = "@INSERT# INTO TMPCONTROLHOLIDAYS VALUES ('" + Strings.Replace(roTypes.Any2String(ARow["FullGroupName"]), "'", "''") + "','";
                        sSQL = sSQL + Strings.Replace(roTypes.Any2String(ARow["GroupName"]), "'", "''") + "'," + Convert.ToString(roTypes.Any2Double(ARow["IDEmployee"])) + ",'";
                        sSQL = sSQL + Strings.Replace(roTypes.Any2String(ARow["EmployeeName"]), "'", "''") + "'," + Strings.Replace(Convert.ToString(roTypes.Any2Double(StartupValue)), ",", ".") + "," + Strings.Replace(Convert.ToString(roTypes.Any2Double(CarryOver)), ",", ".") + ",";
                        sSQL = sSQL + Strings.Replace(Convert.ToString(roTypes.Any2Double(StartupBalance)), ",", ".") + "," + Strings.Replace(Convert.ToString(roTypes.Any2Double(EnjoyedDays)), ",", ".") + ",'" + LineEnjoyed + "'," + Strings.Replace(Convert.ToString(roTypes.Any2Double(CurrentBalance)), ",", ".") + ",";
                        sSQL = sSQL + Strings.Replace(Convert.ToString(roTypes.Any2Double(DaysProvided)), ",", ".") + ",'" + LineProvided + "'," + Strings.Replace(Convert.ToString(roTypes.Any2Double(EndingBalance)), ",", ".") + ",'" + Contract + "'," + IDReportTask.ToString() + ")";
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpControlHolidays");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpControlHolidays");
            }
            finally
            {
            }
        }

        private string GetValueFromAccrualRule(double IDAccrualRule, string parameter)
        {
            // Obtenemos un parametro de la tabla sysroparameters

            string strRet = "";

            string sSQL;
            object sXML;
            var Result = new roCollection();

            try
            {
                sSQL = "@SELECT# Definition FROM AccrualsRules WHERE IDAccrualsRule = " + Convert.ToString(IDAccrualRule);
                sXML = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                Result.LoadXMLString(sXML.ToString());
                strRet = Result[parameter].ToString();
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:GetValueFromAccrualRule");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:GetValueFromAccrualRule");
            }
            finally
            {
            }

            return strRet;
        }

        /// <summary>
        /// Crea tabla temporal con empleados y campos de la ficha
        /// </summary>
        /// <remarks></remarks>
        private void CreateTmpUserFields(string TableName, int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            try
            {
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPUSERFIELDS WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string sSQL = "";

                switch (TableName.ToUpper())
                {
                    case "SYSROVWALLEMPLOYEEGROUPS":
                        {
                            string[] empFilter = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector") && x.Name.Equals("Employees")).FirstOrDefault().Value.ToString().Split('@');

                            sSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee FROM Employees";
                            sSQL = sSQL + " INNER JOIN sysrovwAllEmployeeGroups ON sysrovwAllEmployeeGroups.IDEmployee = Employees.ID";
                            sSQL = sSQL + " WHERE " + roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]).Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");
                            break;
                        }

                    case "SYSROVWALLEMPLOYEEACCESSGROUPS":
                        {
                            sSQL = "@SELECT# EmployeeId as IDEmployee FROM sysrovwAllEmployeeAccessGroups ";
                            sSQL = sSQL + " WHERE " + "AccessGroupID in (" + parametersList.Where(x => x.Name.Equals("AccessGroups")).FirstOrDefault().Value + ")";
                            break;
                        }
                }

                var aDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                var oEmployeeState = new roEmployeeState();
                //roBusinessState.CopyTo(oState, oEmployeeState);

                foreach (DataRow oRow in aDS.Rows)
                {
                    if (Strings.Len(parametersList.Where(x => x.Name.Equals("SpecialUserFields")).FirstOrDefault().Value) > 0)
                    {
                        var oUserFields = Robotics.Base.VTUserFields.UserFields.roEmployeeUserField.GetUserFieldsList(roTypes.Any2Integer(oRow["IDEmployee"]), DateAndTime.Now.Date, new Robotics.Base.VTUserFields.UserFields.roUserFieldState(int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier") && x.Name.Equals("PassportId")).FirstOrDefault().Value.ToString())));
                        foreach (Robotics.Base.VTUserFields.UserFields.roEmployeeUserField oUserField in oUserFields)
                        {
                            if ((oUserField.FieldName.ToUpper() ?? "") == (Convert.ToString(parametersList.Where(x => x.Name.Equals("SpecialUserFields")).FirstOrDefault().Value).ToUpper() ?? ""))
                            {
                                sSQL = "@INSERT# INTO TMPUSERFIELDS VALUES (" + Convert.ToString(roTypes.Any2Integer(oRow["IDEmployee"])) + ",'" + oUserField.FieldValue + "'," + IDReportTask.ToString() + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);

                                break;
                            }
                        }
                    }
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpUserFields");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:CreateTmpUserFields");
            }
            finally
            {
            }
        }

        private void CreateTmpHolidaysControlByContractEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            ////////////////////////////////////////////////////
            DateTime startDate = (DateTime)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value;
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string Employees = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);
            ////////////////////////////////////////////////////

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;
            DataTable sConcept;
            DateTime DateIniPeriode;
            DateTime DateFinPeriode;
            DateTime DateInifilter;
            DateTime DateFinFilter;

            double StartupValue;
            double CarryOver;
            double EnjoyedDays;
            double CurrentBalance;
            double DaysProvided;
            double EndingBalance;

            int StartDateMonth;
            int StartDateDay;
            //roTime StartDateDayM;
            //roTime StartDateDayY;
            //CRUFLCOM.Visits oVisit = new CRUFLCOM.Visits();
            DataTable bDS;

            object sXML;
            roCollection Result = new roCollection();
            double IdConcept;
            double IDCause;

            try
            {
                //Recupero el mes de inicio de a�o
                StartDateMonth = roTypes.Any2Integer(GetValueFromSysroParameters("roParYearPeriod"));
                //Si no existe el par�metro roParYearPeriod, lo creo a 1 (enero)
                if (StartDateMonth == 0)
                {
                    StartDateMonth = 1;
                }

                // Recupero el dia de inicio de mes
                StartDateDay = roTypes.Any2Integer(GetValueFromSysroParameters("roParMonthPeriod"));

                if (StartDateDay == 0)
                {
                    StartDateDay = 1;
                }

                DateIniPeriode = new DateTime(startDate.Year, StartDateMonth, StartDateDay);
                DateFinPeriode = roTypes.Any2Time(DateIniPeriode).Add(1, "yyyy").Add(-1, "d").ValueDateTime;

                //Borramos los registros anteriores
                //MTH: No borramos datas para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                // cerrada por parentesis, asi que, recortamos el ultimo parentesis de la consulta para poder anidar una clausula AND nueva.
                sSQL = "@SELECT# sysrovwAllEmployeeGroups.groupname, sysrovwAllEmployeeGroups.Fullgroupname, sysrovwAllEmployeeGroups.idemployee, sysrovwAllEmployeeGroups.idGroup, " +
                       "sysrovwAllEmployeeGroups.EmployeeName, EmployeeContracts.IDContract, EmployeeContracts.BeginDate AS BeginDateContract, " +
                       "dbo.EmployeeContracts.EndDate AS EndDateContract " +
                       "FROM dbo.EmployeeContracts INNER JOIN " +
                       "dbo.Employees ON dbo.EmployeeContracts.IDEmployee = dbo.Employees.ID INNER JOIN " +
                       "dbo.sysrovwAllEmployeeGroups ON dbo.Employees.ID = dbo.sysrovwAllEmployeeGroups.IDEmployee ";
                sSQL = sSQL + " where " + Employees.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups") + " and EmployeeContracts.BeginDate<=" + roTypes.Any2Time(DateFinPeriode).SQLSmallDateTime() + " and EmployeeContracts.EndDate>=" + roTypes.Any2Time(DateIniPeriode).SQLSmallDateTime();
                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Determina fecha inicial y final de b�squeda
                        DateInifilter = (DateIniPeriode <= sQueryDataRow.Field<DateTime>("BeginDateContract") ? sQueryDataRow.Field<DateTime>("BeginDateContract") : DateIniPeriode);
                        DateFinFilter = (DateFinPeriode >= sQueryDataRow.Field<DateTime>("EndDateContract") ? sQueryDataRow.Field<DateTime>("EndDateContract") : DateFinPeriode);

                        // Para cada empleado obtenemos los saldos de vacaciones
                        sSQL = "@SELECT# ID, Name FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Shifts WHERE IDConceptBalance > 0 AND ShiftType=2)";
                        sConcept = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (sConcept != null)
                        {
                            foreach (DataRow sConceptDataRow in sConcept.Rows)
                            {
                                // Valor inicial
                                StartupValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND StartupValue=1  AND CarryOver=1 AND IDConcept = " + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime()));

                                // Arrastre a�o anterior
                                CarryOver = 0;

                                // Obtenemos las reglas de convenio que utilicen este saldo y buscamos el valor de arrastre para la fecha
                                // inicial del a�o
                                //sSQL = "@SELECT# Definition FROM AccrualsRules "
                                sSQL = "@SELECT# Definition FROM AccrualsRules inner join LabAgreeAccrualsRules on AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules";
                                sSQL = sSQL + " where LabAgreeAccrualsRules.IDLabAgree in (@select# IDLabAgree from EmployeeContracts where BeginDate <=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and EndDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + ")";
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        sXML = roTypes.Any2String(bDSDataRow.Field<string>("Definition"));
                                        Result.LoadXMLString(sXML.ToString());
                                        IdConcept = roTypes.Any2Double(Result["MainAccrual"]);
                                        IDCause = roTypes.Any2Double(Result["DestiAccrual"]);
                                        //Err.Clear
                                        //On Error GoTo Catch

                                        if (IdConcept == roTypes.Any2Double(sConceptDataRow["ID"]))
                                        {
                                            //Buscamos el valor de arrastre
                                            sSQL = "@SELECT# sum(ISNULL(Value,0)) as total FROM DailyCauses WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee"));
                                            sSQL = sSQL + " AND Date = " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime();
                                            sSQL = sSQL + " AND IDCause = " + IDCause + " AND AccrualsRules=1";
                                            CarryOver = CarryOver + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                        }
                                        //DoEvents
                                        //if Not bDS.MoveNext { GoTo finally
                                    }
                                }

                                bDS.Dispose();

                                //                        // Dias disfrutados
                                EnjoyedDays = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# COUNT(*) FROM DailySchedule INNER JOIN Employees ON DailySchedule.IDEmployee = Employees.[ID] INNER JOIN Shifts ON DailySchedule.IDShiftUsed = Shifts.[ID] WHERE DailySchedule.IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND Shifts.ID IN(@SELECT# ID FROM SHifTS WHERE ShiftType=2 and IDConceptBalance =" + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + ") " + "  AND DailySchedule.[Date] BETWEEN " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND " + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE <=" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                //                        // Saldo actual
                                CurrentBalance = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND IDConcept = " + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE <=" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                //                        // Dias planificados futuros
                                DaysProvided = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# COUNT(*) FROM DailySchedule INNER JOIN Employees ON DailySchedule.IDEmployee = Employees.[ID] INNER JOIN Shifts ON DailySchedule.IDShift1= Shifts.[ID] WHERE DailySchedule.IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND Shifts.ID IN(@SELECT# ID FROM SHifTS WHERE ShiftType=2 and IDConceptBalance =" + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + ") " + "  AND DailySchedule.[Date] BETWEEN " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND " + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE >" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                //                        // Saldo restante previsto
                                EndingBalance = CurrentBalance - DaysProvided;

                                if (StartupValue != 0 || EnjoyedDays != 0 || CurrentBalance != 0 || DaysProvided != 0 || roTypes.Any2Boolean(CarryOver))
                                {
                                    sSQL = "@INSERT# INTO TMPHOLIDAYSCONTROLByContract (IDEmployee, IDConcept, StartupValue, CarryOver, EnjoyedDays, CurrentBalance, DaysProvided, EndingBalance, idContract, IDReportTask) VALUES (";
                                    sSQL = sSQL + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + "," + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + "," + StartupValue.ToString().Replace(",", ".") + "," + CarryOver.ToString().Replace(",", ".") + "," + EnjoyedDays.ToString().Replace(",", ".") + "," + CurrentBalance.ToString().Replace(",", ".") + "," + DaysProvided.ToString().Replace(",", ".") + "," + EndingBalance.ToString().Replace(",", ".") + ",'" + sQueryDataRow.Field<string>("idContract") + "'," + IDReportTask + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContract: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        //Se usa!
        private void CreateTmpHolidaysControlByContractExV2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = (DateTime)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;
            DataTable sConcept;
            DateTime DateIniPeriode;
            DateTime DateFinPeriode;
            DateTime DateInifilter;
            DateTime DateFinFilter;

            double StartupValue;
            double CarryOver;

            int StartDateMonth;
            int StartDateDay;
            DataTable bDS;

            object sXML;
            roCollection Result = new roCollection();
            double IdConcept;
            double IDCause;

            try
            {
                roParameters oParameters;
                oParameters = new roParameters("OPTIONS", true);

                //Recupero el mes de inicio de a�o

                StartDateMonth = roTypes.Any2Integer(oParameters.get_Parameter(Parameters.YearPeriod));
                //Si no existe el par�metro roParYearPeriod, lo creo a 1 (enero)
                if (StartDateMonth == 0)
                {
                    StartDateMonth = 1;
                }

                // Recupero el dia de inicio de mes
                StartDateDay = roTypes.Any2Integer(oParameters.get_Parameter(Parameters.MonthPeriod));
                if (StartDateDay == 0)
                {
                    StartDateDay = 1;
                }

                DateIniPeriode = new DateTime(StartDate.Year, StartDateMonth, StartDateDay);
                DateFinPeriode = roTypes.Any2Time(DateIniPeriode).Add(1, "yyyy").Add(-1, "d").ValueDateTime;

                //Borramos los registros anteriores
                //MTH: No borramos datas para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);
                if (empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                // cerrada por parentesis, asi que, recortamos el ultimo parentesis de la consulta para poder anidar una clausula AND nueva.
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.groupname, sysrovwCurrentEmployeeGroups.Fullgroupname, sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.idGroup, " +
                       "sysrovwCurrentEmployeeGroups.EmployeeName, EmployeeContracts.IDContract, EmployeeContracts.BeginDate AS BeginDateContract, " +
                       "dbo.EmployeeContracts.EndDate AS EndDateContract " +
                       "FROM dbo.EmployeeContracts ";

                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = EmployeeContracts.IDEmployee And " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " between poe.BeginDate And poe.EndDate " +
                        " AND poe.IDEmployee IN (" + empFilter + ") " +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1 ";
                }
                sSQL = sSQL + "INNER JOIN dbo.Employees ON dbo.EmployeeContracts.IDEmployee = dbo.Employees.ID INNER JOIN " +
                       "dbo.sysrovwCurrentEmployeeGroups ON dbo.Employees.ID = dbo.sysrovwCurrentEmployeeGroups.IDEmployee ";
                sSQL = sSQL + " WHERE ";

                if (empSelected == null)
                {
                    sSQL = sSQL + "sysrovwCurrentEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + "EmployeeContracts.BeginDate <= " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND EmployeeContracts.EndDate >= " + roTypes.Any2Time(StartDate).SQLSmallDateTime();
                //Esto lo dejamos preparado por si hacemos un selector de mostrar contrato vigente a fecha seleccionada o contratos dentro del periodo
                //sSQL = sSQL + " AND EmployeeContracts.BeginDate <= " + roTypes.Any2Time(DateFinPeriode.Date).SQLSmallDateTime() + " AND EmployeeContracts.EndDate >= " + roTypes.Any2Time(DateIniPeriode.Date).SQLSmallDateTime();

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Determina fecha inicial y final de b�squeda
                        DateInifilter = (DateIniPeriode <= sQueryDataRow.Field<DateTime>("BeginDateContract") ? sQueryDataRow.Field<DateTime>("BeginDateContract") : DateIniPeriode);
                        DateFinFilter = (DateFinPeriode >= sQueryDataRow.Field<DateTime>("EndDateContract") ? sQueryDataRow.Field<DateTime>("EndDateContract") : DateFinPeriode);

                        // Para cada empleado obtenemos los saldos de vacaciones que no estén caducados en el año en curso
                        sSQL = "@SELECT# ID, Name, DefaultQuery FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Shifts WHERE IDConceptBalance > 0 AND ShiftType=2) AND (Concepts.FinishDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + ")";
                        sConcept = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (sConcept != null)
                        {
                            foreach (DataRow sConceptDataRow in sConcept.Rows)
                            {

                                // Valor inicial
                                StartupValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow["IDEmployee"]) + " AND StartupValue=1  AND CarryOver=1 AND IDConcept = " + roTypes.Any2Double(sConceptDataRow["ID"]) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime()));

                                // Arrastre a�o anterior
                                CarryOver = 0;

                                // Obtenemos las reglas de convenio que utilicen este saldo y buscamos el valor de arrastre para la fecha
                                // inicial del periodo

                                sSQL = "@SELECT# Definition FROM AccrualsRules inner join LabAgreeAccrualsRules on AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules";
                                sSQL = sSQL + " where LabAgreeAccrualsRules.IDLabAgree in (@select# IDLabAgree from EmployeeContracts where BeginDate <=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and EndDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + ")";
                                sSQL = sSQL + " AND LabAgreeAccrualsRules.BeginDate <=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and LabAgreeAccrualsRules.EndDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime();
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        try
                                        {
                                            sXML = roTypes.Any2String(bDSDataRow.Field<string>("Definition"));
                                            Result.LoadXMLString(sXML.ToString());
                                            IdConcept = roTypes.Any2Double(Result["MainAccrual"]);
                                            IDCause = roTypes.Any2Double(Result["DestiAccrual"]);
                                            //Err.Clear
                                            //On Error GoTo Catch

                                            if (IdConcept == roTypes.Any2Double(sConceptDataRow["ID"]))
                                            {
                                                //Buscamos el valor de arrastre
                                                sSQL = "@SELECT# sum(ISNULL(Value,0)) as total FROM DailyCauses WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["IDEmployee"]);
                                                sSQL = sSQL + " AND Date = " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime();
                                                sSQL = sSQL + " AND IDCause = " + IDCause + " AND AccrualsRules=1";
                                                CarryOver = CarryOver + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
                                        }
                                    }
                                }

                                bDS.Dispose();

                                // Obtenemos el primero horario de vacaciones que tiene asignado el saldo
                                sSQL = "@SELECT# TOP 1 ID FROM Shifts WHERE IDConceptBalance = " + roTypes.Any2Double(sConceptDataRow["ID"]) + " AND ShiftType = 2 ";
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                var _State = new roEmployeeState(-1);
                                double intResult = 0; // Final previsto
                                double intDone = 0; // Obtener número días ya disfrutados
                                double intPending = 0; // Número de días solicitados pendientes de procesar
                                double intLasting = 0; // Número de días aprobados pendientes de disfrutar
                                double intDisponible = 0; // Número de días disponibles de vacaciones
                                double intDaysWithoutEnjoyment = 0;
                                double intExpiredDays = 0;


                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        DateTime xbeginperiod = DateInifilter;
                                        DateTime xendperiod = DateFinFilter;

                                        //si elegimos una fecha con año actual, el currentDate es día de hoy
                                        if (StartDate.Year == DateTime.Now.Year)
                                        {
                                            StartDate = new DateTime(StartDate.Year, DateTime.Now.Month, DateTime.Now.Day);
                                        }
                                        else if (StartDate.Year < DateTime.Now.Year)
                                        {
                                            //en caso de que sea año anterior al actual, el current Date debe ser último día del año seleccionado
                                            StartDate = new DateTime(StartDate.Year, 12, 31);
                                        }

                                        roBusinessSupport.VacationsResumeQuery(roTypes.Any2Integer(sQueryDataRow["IDEmployee"]), roTypes.Any2Integer(bDSDataRow["ID"]), StartDate, ref xbeginperiod, ref xendperiod, DateInifilter, ref intDone, ref intPending, ref intLasting, ref intDisponible, _State, ref intExpiredDays, ref intDaysWithoutEnjoyment, roTypes.Any2String(sQueryDataRow["IDContract"]));
                                    }
                                }

                                bDS.Dispose();

                                intResult = intDisponible - (intLasting + intPending);

                                if (StartupValue != 0 || intDone != 0 || intDisponible != 0 || intLasting != 0 || intPending != 0 || roTypes.Any2Boolean(CarryOver))
                                {
                                    //No insertamos los registros que sean de tipo Año laboral
                                    if (roTypes.Any2String(sConceptDataRow["DefaultQuery"]) != "L")
                                    {
                                        sSQL = "@INSERT# INTO TMPHOLIDAYSCONTROLByContract (IDEmployee, IDConcept, StartupValue, CarryOver, EnjoyedDays, CurrentBalance, DaysProvided, EndingBalance, idContract, IDReportTask, DaysPendingApproval) VALUES (";
                                        sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["IDEmployee"]) + "," + roTypes.Any2Double(sConceptDataRow["ID"]) + "," + StartupValue.ToString().Replace(",", ".") + "," + CarryOver.ToString().Replace(",", ".") + "," + intDone.ToString().Replace(",", ".") + "," + intDisponible.ToString().Replace(",", ".") + "," + intLasting.ToString().Replace(",", ".") + "," + intResult.ToString().Replace(",", ".") + ",'" + sQueryDataRow.Field<string>("idContract") + "'," + IDReportTask + "," + intPending.ToString().Replace(",", ".") + ")";
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpHolidaysControlByContractExV2::", ex);
            }
            finally
            {
            }
        }
        //Se usa!
        private void CreateTmpPlannedHolidaysControlByContractExV2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = (DateTime)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;
            DataTable sConcept;
            DateTime DateIniPeriode;
            DateTime DateFinPeriode;
            DateTime DateInifilter;
            DateTime DateFinFilter;

            double StartupValue;
            double CarryOver;

            int StartDateMonth;
            int StartDateDay;
            DataTable bDS;

            object sXML;
            roCollection Result = new roCollection();
            double IdConcept;
            double IDCause;

            try
            {
                roParameters oParameters;
                oParameters = new roParameters("OPTIONS", true);

                //Recupero el mes de inicio de a�o

                StartDateMonth = roTypes.Any2Integer(oParameters.get_Parameter(Parameters.YearPeriod));
                //Si no existe el par�metro roParYearPeriod, lo creo a 1 (enero)
                if (StartDateMonth == 0)
                {
                    StartDateMonth = 1;
                }

                // Recupero el dia de inicio de mes
                StartDateDay = roTypes.Any2Integer(oParameters.get_Parameter(Parameters.MonthPeriod));
                if (StartDateDay == 0)
                {
                    StartDateDay = 1;
                }

                DateIniPeriode = new DateTime(StartDate.Year, StartDateMonth, StartDateDay);
                DateFinPeriode = roTypes.Any2Time(DateIniPeriode).Add(1, "yyyy").Add(-1, "d").ValueDateTime;

                //Borramos los registros anteriores
                //MTH: No borramos datas para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);
                if (empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                // cerrada por parentesis, asi que, recortamos el ultimo parentesis de la consulta para poder anidar una clausula AND nueva.
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.groupname, sysrovwCurrentEmployeeGroups.Fullgroupname, sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.idGroup, " +
                       "sysrovwCurrentEmployeeGroups.EmployeeName, EmployeeContracts.IDContract, EmployeeContracts.BeginDate AS BeginDateContract, " +
                       "dbo.EmployeeContracts.EndDate AS EndDateContract " +
                       "FROM dbo.EmployeeContracts ";
                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = EmployeeContracts.IDEmployee And " + roTypes.Any2Time(DateIniPeriode.Date).SQLSmallDateTime() + " between poe.BeginDate And poe.EndDate " +
                        " AND poe.IDEmployee IN (" + empFilter + ") " +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1 ";
                }
                sSQL = sSQL + "INNER JOIN dbo.Employees ON dbo.EmployeeContracts.IDEmployee = dbo.Employees.ID INNER JOIN " +
                       "dbo.sysrovwCurrentEmployeeGroups ON dbo.Employees.ID = dbo.sysrovwCurrentEmployeeGroups.IDEmployee ";
                sSQL = sSQL + " WHERE ";

                if (empSelected == null)
                {
                    sSQL = sSQL + "sysrovwCurrentEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + "EmployeeContracts.BeginDate <= " + roTypes.Any2Time(DateFinPeriode.Date).SQLSmallDateTime() + " AND EmployeeContracts.EndDate >= " + roTypes.Any2Time(DateIniPeriode.Date).SQLSmallDateTime();

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Determina fecha inicial y final de b�squeda
                        DateInifilter = (DateIniPeriode <= sQueryDataRow.Field<DateTime>("BeginDateContract") ? sQueryDataRow.Field<DateTime>("BeginDateContract") : DateIniPeriode);
                        DateFinFilter = (DateFinPeriode >= sQueryDataRow.Field<DateTime>("EndDateContract") ? sQueryDataRow.Field<DateTime>("EndDateContract") : DateFinPeriode);

                        // Para cada empleado obtenemos los saldos de vacaciones por horas
                        sSQL = "@SELECT# ID, Name FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Causes WHERE IDConceptBalance > 0 AND IsHoliday = 1)";
                        sConcept = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (sConcept != null)
                        {
                            foreach (DataRow sConceptDataRow in sConcept.Rows)
                            {
                                // Valor inicial
                                StartupValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow["IDEmployee"]) + " AND StartupValue=1  AND CarryOver=1 AND IDConcept = " + roTypes.Any2Double(sConceptDataRow["ID"]) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime()));

                                // Arrastre a�o anterior
                                CarryOver = 0;

                                // Obtenemos las reglas de convenio que utilicen este saldo y buscamos el valor de arrastre para la fecha
                                // inicial del a�o
                                //sSQL = "@SELECT# Definition FROM AccrualsRules "
                                sSQL = "@SELECT# Definition FROM AccrualsRules inner join LabAgreeAccrualsRules on AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules";
                                sSQL = sSQL + " where LabAgreeAccrualsRules.IDLabAgree in (@select# IDLabAgree from EmployeeContracts where BeginDate <=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and EndDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + ")";
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        try
                                        {
                                            sXML = roTypes.Any2String(bDSDataRow.Field<string>("Definition"));
                                            Result.LoadXMLString(sXML.ToString());
                                            IdConcept = roTypes.Any2Double(Result["MainAccrual"]);
                                            IDCause = roTypes.Any2Double(Result["DestiAccrual"]);
                                            //Err.Clear
                                            //On Error GoTo Catch

                                            if (IdConcept == roTypes.Any2Double(sConceptDataRow["ID"]))
                                            {
                                                //Buscamos el valor de arrastre
                                                sSQL = "@SELECT# sum(ISNULL(Value,0)) as total FROM DailyCauses WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["IDEmployee"]);
                                                sSQL = sSQL + " AND Date = " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime();
                                                sSQL = sSQL + " AND IDCause = " + IDCause + " AND AccrualsRules=1";
                                                CarryOver = CarryOver + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
                                        }
                                    }
                                }

                                bDS.Dispose();

                                // Obtenemos la primera justificación de vacaciones por horas que tiene asignado el saldo
                                sSQL = "@SELECT# TOP 1 ID FROM Causes WHERE IDConceptBalance = " + roTypes.Any2Double(sConceptDataRow["ID"]);
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                var _State = new roEmployeeState(-1);
                                double intResult = 0; // Final previsto
                                double intDone = 0; // Obtener número días ya disfrutados
                                double intPending = 0; // Número de días solicitados pendientes de procesar
                                double intLasting = 0; // Número de días aprobados pendientes de disfrutar
                                double intDisponible = 0; // Número de días disponibles de vacaciones

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        double intMaxValue = 0;

                                        DateTime xbeginperiod = DateInifilter;
                                        DateTime xendperiod = DateFinFilter;

                                        //si elegimos una fecha con año actual, el currentDate es día de hoy
                                        if (StartDate.Year == DateTime.Now.Year)
                                        {
                                            StartDate = new DateTime(StartDate.Year, DateTime.Now.Month, DateTime.Now.Day);
                                        }
                                        else if (StartDate.Year < DateTime.Now.Year)
                                        {
                                            //en caso de que sea año anterior al actual, el current Date debe ser último día del año seleccionado
                                            StartDate = new DateTime(StartDate.Year, 12, 31);
                                        }


                                        roBusinessSupport.ProgrammedHolidaysResumeQuery(roTypes.Any2Integer(sQueryDataRow["IDEmployee"]), roTypes.Any2Integer(bDSDataRow["ID"]), StartDate, ref xbeginperiod, ref xendperiod, DateInifilter, ref intPending, ref intLasting, ref intDisponible, _State, ref intMaxValue);
                                    }
                                }

                                bDS.Dispose();

                                intResult = intDisponible - (intLasting + intPending);

                                if (StartupValue != 0 || intDone != 0 || intDisponible != 0 || intLasting != 0 || intPending != 0 || roTypes.Any2Boolean(CarryOver))
                                {
                                    sSQL = "@INSERT# INTO TMPHOLIDAYSCONTROLByContract (IDEmployee, IDConcept, StartupValue, CarryOver, EnjoyedDays, CurrentBalance, DaysProvided, EndingBalance, idContract, IDReportTask, DaysPendingApproval) VALUES (";
                                    sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["IDEmployee"]) + "," + roTypes.Any2Double(sConceptDataRow["ID"]) + "," + StartupValue.ToString().Replace(",", ".") + "," + CarryOver.ToString().Replace(",", ".") + "," + intDone.ToString().Replace(",", ".") + "," + intDisponible.ToString().Replace(",", ".") + "," + intLasting.ToString().Replace(",", ".") + "," + intResult.ToString().Replace(",", ".") + ",'" + sQueryDataRow.Field<string>("idContract") + "'," + IDReportTask + "," + intPending.ToString().Replace(",", ".") + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpPlannedHolidaysControlByContractExV2::", ex);
            }
            finally
            {
            }
        }

        private void CreateTmpAccrualsByGroup(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByGroup WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                // cerrada por parentesis, asi que, recortamos el ultimo parentesis de la consulta para poder anidar una clausula AND nueva.
                sSQL = "@SELECT# dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\\') + ' - ' + dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\\') + ' - ' + dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\\')  as Level3,  sysroEmployeeGroups.idemployee,sysroEmployeeGroups.idgroup " +
                       " FROM dbo.EmployeeContracts INNER JOIN " +
                       " dbo.sysroEmployeeGroups ON dbo.EmployeeContracts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysroEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysroEmployeeGroups.IDEmployee IN(-1)";
                }

                sSQL = sSQL + " AND EmployeeContracts.BeginDate <= " + roTypes.Any2Time(EndDate.Date).SQLSmallDateTime() + " AND EmployeeContracts.EndDate >= " + roTypes.Any2Time(StartDate.Date).SQLSmallDateTime();
                sSQL = sSQL + " AND " + roTypes.Any2Time(StartDate.Date).SQLSmallDateTime() + "<= sysroEmployeeGroups.EndDate AND  " + roTypes.Any2Time(EndDate.Date).SQLSmallDateTime() + " >=  sysroEmployeeGroups.BeginDate ";
                sSQL = sSQL + " AND len(dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\\')) > 0 ";

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysroEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\\') + ' - ' + dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\\') + ' - ' + dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\\')  ,sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IDGroup";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    double intIDGroup = 0;

                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Para cada empleado obtenemos su nivel 3 y si no existe lo añadimos a la tabla temporal
                        // en caso que exsita añadimos 1 al contador de empleados
                        sSQL = "@SELECT# isnull(FullGroupName,'') FROM TMPAccrualsByGroup WHERE FullGroupName like '" + roTypes.Any2String(sQueryDataRow["Level3"]).Replace("'", "''") + "' AND IDReportTask = " + IDReportTask.ToString();
                        if (roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) == "")
                        {
                            intIDGroup = intIDGroup + 1;

                            sSQL = "@INSERT# INTO TMPAccrualsByGroup (IDGroup, FullGroupName, TotalEmployees,IDReportTask) VALUES (";
                            sSQL = sSQL + intIDGroup.ToString() + ",'" + roTypes.Any2String(sQueryDataRow["Level3"]).Replace("'", "''") + "',0," + IDReportTask.ToString() + ")";
                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                        //else
                        //{
                        //    sSQL = "@UPDATE# TMPAccrualsByGroup Set TotalEmployees= TotalEmployees + 1 WHERE FullGroupName like '" + roTypes.Any2String(sQueryDataRow["Level3"]).Replace("'", "''") + "' AND IDReportTask = " + IDReportTask.ToString();
                        //    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);

                        //}

                        string ShortName = "Teo";
                        double Value = 0;

                        string sSQLUpdate = "";
                        for (int x = 1; x <= 10; x++)
                        {
                            switch (x)
                            {
                                case 1:
                                    {
                                        ShortName = "Ben";
                                        break;
                                    }
                                case 2:
                                    {
                                        ShortName = "Med";
                                        break;
                                    }
                                case 3:
                                    {
                                        ShortName = "RET";
                                        break;
                                    }
                                case 4:
                                    {
                                        ShortName = "IND";
                                        break;
                                    }
                                case 5:
                                    {
                                        ShortName = "PER";
                                        break;
                                    }
                                case 6:
                                    {
                                        ShortName = "Bac";
                                        break;
                                    }
                                case 7:
                                    {
                                        ShortName = "Bma";
                                        break;
                                    }
                                case 8:
                                    {
                                        ShortName = "FSI";
                                        break;
                                    }
                                case 9:
                                    {
                                        ShortName = "Teo";
                                        break;
                                    }
                                case 10:
                                    {
                                        ShortName = "TAB";
                                        break;
                                    }
                            }

                            sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                            sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                            sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                            sSQL = sSQL + " INNER JOIN sysroEmployeeGroups sreg ON sreg.IDEmployee = e.ID";
                            sSQL = sSQL + " INNER JOIN Groups g ON g.id = sreg.IDGroup";
                            sSQL = sSQL + " INNER JOIN EmployeeContracts ec ON ec.idemployee = e.id";
                            sSQL = sSQL + " WHERE sreg.IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                            sSQL = sSQL + " AND c.ShortName = '" + ShortName + "'";
                            sSQL = sSQL + " AND sreg.idgroup = " + roTypes.Any2Double(sQueryDataRow["idgroup"]);
                            sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                            sSQL = sSQL + " AND da.Date >= sreg.BeginDate AND da.Date <= sreg.EndDate";
                            sSQL = sSQL + " AND da.Date BETWEEN ec.BeginDate AND ec.EndDate";
                            Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                            if (x == 1)
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByGroup Set TotalEmployees= isnull(TotalEmployees,0) + 1, Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE FullGroupName like '" + roTypes.Any2String(sQueryDataRow["Level3"]).Replace("'", "''") + "' AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                            else
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByGroup Set Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE FullGroupName like '" + roTypes.Any2String(sQueryDataRow["Level3"]).Replace("'", "''") + "' AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpAccrualsByEmployee(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        string ShortName = "Teo";
                        double Value = 0;

                        string sSQLUpdate = "";
                        for (int x = 1; x <= 12; x++)
                        {
                            switch (x)
                            {
                                case 1:
                                    {
                                        ShortName = "Ben";
                                        break;
                                    }
                                case 2:
                                    {
                                        ShortName = "Med";
                                        break;
                                    }
                                case 3:
                                    {
                                        ShortName = "RET";
                                        break;
                                    }
                                case 4:
                                    {
                                        ShortName = "IND";
                                        break;
                                    }
                                case 5:
                                    {
                                        ShortName = "PER";
                                        break;
                                    }
                                case 6:
                                    {
                                        ShortName = "Bac";
                                        break;
                                    }
                                case 7:
                                    {
                                        ShortName = "Bma";
                                        break;
                                    }
                                case 8:
                                    {
                                        ShortName = "FSI";
                                        break;
                                    }
                                case 9:
                                    {
                                        ShortName = "Teo";
                                        break;
                                    }
                                case 10:
                                    {
                                        ShortName = "TAB";
                                        break;
                                    }
                                case 11:
                                    {
                                        ShortName = "Htr";
                                        break;
                                    }
                                case 12:
                                    {
                                        ShortName = "Htr";
                                        break;
                                    }
                            }

                            sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                            sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                            sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                            sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                            sSQL = sSQL + " AND c.ShortName = '" + ShortName + "'";
                            sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                            Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                            if (x == 11 && Value != 0.0)
                            {
                                //- IT Value1
                                //- VISMED Valuie 2
                                //- RETAUS Valkue 3
                                //- INDISP Value 4
                                //- PERM  Value 5
                                sSQL = "@SELECT# isnull(value1,0) + isnull(value2,0) + isnull(value3,0) + isnull(value4,0) + isnull(value5,0)  FROM TMPAccrualsByEmployee ";
                                sSQL = sSQL + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) / Value;
                            }

                            if (x == 12)
                            {
                                //  (Htr -
                                //- IT Value1
                                //- VISMED Value 2
                                //- RETAUS Valkue 3
                                //- INDISP Value 4
                                //- PERM  Value 5) / Teo
                                sSQL = "@SELECT# isnull(value1,0) + isnull(value2,0) + isnull(value3,0) + isnull(value4,0) + isnull(value5,0)  FROM TMPAccrualsByEmployee ";
                                sSQL = sSQL + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Value = Value - roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                sSQL = "@SELECT# isnull(value9,0) FROM TMPAccrualsByEmployee ";
                                sSQL = sSQL + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                double Teo = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                if (Teo != 0.0)
                                {
                                    Value = Value / Teo;
                                }
                                else
                                {
                                    Value = 0.0;
                                }
                            }

                            if (x == 1)
                            {
                                sSQL = "@INSERT# INTO TMPAccrualsByEmployee (IDEmployee, FullGroupName, DNI,EmployeeName,Value1,IDReportTask) VALUES (";
                                sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByEmployee Set Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpCN(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        string ShortName = "PZO";
                        double Value = 0;

                        string sSQLUpdate = "";
                        for (int x = 1; x <= 8; x++)
                        {
                            switch (x)
                            {
                                case 1:
                                    {
                                        ShortName = "PZO";
                                        break;
                                    }
                                case 2:
                                    {
                                        ShortName = "PZA";
                                        break;
                                    }
                                case 3:
                                    {
                                        ShortName = "PTU";
                                        break;
                                    }
                                case 4:
                                    {
                                        ShortName = "PNO";
                                        break;
                                    }
                                case 5:
                                    {
                                        ShortName = "PLE";
                                        break;
                                    }
                                case 6:
                                    {
                                        ShortName = "PFS";
                                        break;
                                    }
                                case 7:
                                    {
                                        ShortName = "PFE";
                                        break;
                                    }
                                case 8:
                                    {
                                        ShortName = "ADO";
                                        break;
                                    }
                            }

                            sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                            sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                            sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                            sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                            sSQL = sSQL + " AND c.ShortName = '" + ShortName + "'";
                            sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                            Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                            if (x == 1)
                            {
                                sSQL = "@INSERT# INTO TMPAccrualsByEmployee (IDEmployee, FullGroupName, DNI,EmployeeName,Value1,IDReportTask) VALUES (";
                                sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByEmployee Set Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        //Se usa
        private void CreateTmpDetailedCalendarEmployeev2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Eliminamos registros vacíos del array IDConcept
            string[] IDConcept = parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptsSelector")).FirstOrDefault().Value.ToString().Split(',').Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            DataTable dsInfo;

            DataTable dsPunches;

            Dictionary<string, List<string>> dicInserts = new Dictionary<string, List<string>>();

            DateTime StartWorkDate;
            DateTime EndWorkDate;

            string strInsert;
            string strValues;
            string strFields;

            double[] ConceptValues;
            long IndexConcepts;
            int IndexConcept;


            string commaSeparatedIDEmployees;

            try
            {
                AccessHelper.ExecuteSql($"@DELETE# TMPDetailedCalendarEmployee WHERE IDReportTask = {IDReportTask}");
                AccessHelper.ExecuteSql($"@DELETE# TMPDetailedCalendarEmployee_DailyIncidences  WHERE IDReportTask = {IDReportTask}");

                string empFilter = string.Join(",", empSelected);
                if( empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IdGroup, Employees.Name AS EmployeeName, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,'NIF', getdate()),'') as FieldValue FROM sysroEmployeeGroups inner join Employees on employees.ID = sysroEmployeeGroups.IDEmployee ";
                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) between cast(poe.BeginDate as date) And cast(poe.EndDate as date) " +
                        " AND poe.IDEmployee IN (" + empFilter + ") " +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1 ";
                }
                if (empSelected == null)
                {
                    sSQL = sSQL + "WHERE sysroEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + " ORDER BY Employees.Name, sysroEmployeeGroups.BeginDate ";
                sSQL = sSQL.Replace("sysrovwAllEmployeeGroups", "sysroEmployeeGroups");

                roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2(1):: " + sSQL);
                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    commaSeparatedIDEmployees = string.Join(",", sQuery.AsEnumerable().Where(row => !row.IsNull("IDEmployee")).Select(row => Robotics.VTBase.roTypes.Any2String(row.Field<int>("IDEmployee"))));

                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        if (((DateTime)sQueryDataRow["BeginDate"] >= StartDate && (DateTime)sQueryDataRow["BeginDate"] <= EndDate) ||
                                   ((DateTime)sQueryDataRow["EndDate"] >= StartDate && (DateTime)sQueryDataRow["EndDate"] <= EndDate) ||
                                            ((DateTime)sQueryDataRow["BeginDate"] <= StartDate && (DateTime)sQueryDataRow["EndDate"] >= EndDate))
                        {
                            if ((DateTime)sQueryDataRow["BeginDate"] < StartDate)
                            {
                                StartWorkDate = StartDate;
                            }
                            else
                            {
                                StartWorkDate = (DateTime)sQueryDataRow["BeginDate"];
                            }

                            if ((DateTime)sQueryDataRow["EndDate"] > EndDate)
                            {
                                EndWorkDate = EndDate;
                            }
                            else
                            {
                                EndWorkDate = (DateTime)sQueryDataRow["EndDate"];
                            }

                            // Selecciona datos del empleado
                            sSQL = "@select# DATES.Date, EC.idEmployee, EC.idContract, ISNULL(DS.IDShiftUsed, ISNULL(DS.IDShift1, 0)) AS IDShift " +
                                           "from dbo.ExplodeDates('" + StartWorkDate.ToString("yyyyMMdd") + "','" + EndWorkDate.ToString("yyyyMMdd") + "') as DATES " +
                                           "    Left JOIN EmployeeContracts EC on EC.idEmployee=" + sQueryDataRow["IDEmployee"] + " and cast(DATES.Date as date) between cast(BeginDate as date) and cast(EndDate as date) " +
                                           "    Left JOIN DailySchedule DS on DS.IDEmployee=EC.idEmployee and cast(Dates.Date as date)=cast(DS.date as date) " +
                                           "where not EC.idContract is null " +
                                           "order by ec.idContract,dates.date";

                            roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2(2):: " + sSQL);
                            dsInfo = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                            if (dsInfo != null)
                            {
                                foreach (DataRow sQueryDataInfo in dsInfo.Rows)
                                {
                                    // Si el empleado en esa fecha tiene contrato
                                    if (!roTypes.Any2String(sQueryDataInfo["idcontract"]).Equals(String.Empty))
                                    {
                                        strValues = roTypes.Any2String(IDReportTask);
                                        strFields = "IDReportTask";

                                        strValues = strValues + "," + sQueryDataRow["IDGroup"];
                                        strFields = strFields + ",IDGroup";

                                        strValues = strValues + "," + sQueryDataRow["IDEmployee"];
                                        strFields = strFields + ",IDEmployee";

                                        strValues = strValues + "," + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLSmallDateTime();
                                        strFields = strFields + ",DatePlanification";

                                        strValues = strValues + ",'" + sQueryDataRow["FullGroupName"].ToString().Replace("'", "''") + "' ";
                                        strFields = strFields + ",GroupName";

                                        strValues = strValues + ",'" + (sQueryDataRow["EmployeeName"].ToString().Replace("'", "''") + "'");
                                        strFields = strFields + ",EmployeeName";

                                        // OBTENER SALDOS
                                        if (IDConcept.Length > 0)
                                        {
                                            IndexConcept = 1;

                                            ConceptValues = GetAccrualsFromListv2(roTypes.Any2Integer(sQueryDataRow["IDEmployee"]), string.Join(",", IDConcept), (DateTime)sQueryDataInfo["Date"], (DateTime)sQueryDataInfo["Date"], 15);

                                            for (IndexConcepts = 0; IndexConcepts < IDConcept.Length && IndexConcept <= 15; IndexConcepts++)
                                            {
                                                strValues = strValues + "," + IDConcept[IndexConcepts];
                                                strFields = strFields + "," + "IDConcept" + IndexConcept;

                                                strValues = strValues + ",'" + IDConcept[IndexConcepts] + "'";
                                                strFields = strFields + "," + "ConceptName" + IndexConcept;

                                                strValues = strValues + ",'H'";
                                                strFields = strFields + "," + "FormatConcept" + IndexConcept;

                                                strValues = strValues + "," + ConceptValues[IndexConcept - 1].ToString().Replace(",", ".");
                                                strFields = strFields + "," + "ValueConcept" + IndexConcept;

                                                IndexConcept = IndexConcept + 1;
                                            }
                                        }

                                        //OBTENER PUNCHES

                                        sSQL = "@SELECT# distinct TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                               "IDEmployee = " + sQueryDataRow["IDEmployee"] + " AND cast(ShiftDate as date)= cast(" + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLDateTime() + " as date) AND ActualType = 1 " +
                                               "UNION ALL " +
                                               "@SELECT# distinct TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                               "IDEmployee = " + sQueryDataRow["IDEmployee"] + " AND cast(ShiftDate as date)= cast(" + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLDateTime() + " as date) AND ActualType = 2 " +
                                               "ORDER BY ActualType, DateTime";

                                        roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2(3):: " + sSQL);

                                        dsPunches = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                        if (dsPunches != null)
                                        {
                                            foreach (DataRow sQueryPunches in dsPunches.Rows)
                                            {
                                                if (sQueryPunches["DateTime"] != null)
                                                {
                                                    if (roTypes.Any2String(sQueryPunches["ActualType"]) == "1")
                                                    {
                                                        strValues = strValues + "," + roTypes.Any2Time(sQueryPunches["DateTime"]).SQLSmallDateTime();
                                                        strFields = strFields + "," + "PuncheIn" + sQueryPunches["Rank"];
                                                    }
                                                    else
                                                    {
                                                        strValues = strValues + "," + roTypes.Any2Time(sQueryPunches["DateTime"]).SQLSmallDateTime();
                                                        strFields = strFields + "," + "PuncheOut" + sQueryPunches["Rank"];
                                                    }
                                                }
                                            }
                                        }

                                        //OBTENER HORARIO
                                        if (sQueryDataInfo["IDSHift"] != null)
                                        {
                                            strValues = strValues + "," + sQueryDataInfo["IDSHift"];
                                            strFields = strFields + "," + "IDShift";
                                        }

                                        //INCIDENCIA NO JUSTIFICADA
                                        strValues = strValues + ",1";
                                        strFields = strFields + "," + "Cause";

                                        //////// -----------------------------------ONLINE MODE------------------------------------------------------------
                                        ////////strInsert = "@insert# into TMPDetailedCalendarEmployee ( " + strFields + ") Values (";
                                        ////////strInsert = strInsert + strValues + ")";
                                        ////////Robotics.DataLayer.AccessHelper.ExecuteSql(strInsert);
                                        //////// -----------------------------------ONLINE MODE------------------------------------------------------------


                                        //////// -----------------------------------BATCH MODE------------------------------------------------------------
                                        strInsert = "@insert# into TMPDetailedCalendarEmployee ( " + strFields + ") Values ";

                                        if (!dicInserts.ContainsKey(strInsert))
                                        {
                                            dicInserts.Add(strInsert, new List<string>());
                                        }
                                        dicInserts[strInsert].Add(strValues);
                                    }
                                }
                            }
                        }
                    }
                    if (dicInserts.Any())
                    {
                        for (int i = 0; i < dicInserts.Count; i++)
                        {
                            List<string> values = dicInserts.ElementAt(i).Value;
                            if (values.Any())
                            {
                                int batchSize = 1000;
                                for (int j = 0; j < values.Count; j += batchSize)
                                {
                                    string insertSql = dicInserts.ElementAt(i).Key;
                                    // Toma un subconjunto de hasta 500 elementos
                                    var batchValues = values.Skip(j).Take(batchSize);
                                    foreach (var value in batchValues)
                                    {
                                        insertSql += $"({value}),";
                                    }

                                    // Elimina la última coma
                                    insertSql = insertSql.Substring(0, insertSql.Length - 1);

                                    // Ejecuta el SQL para el bloque actual
                                    try
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2(4):: " + insertSql);
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(insertSql);
                                    }
                                    catch (Exception ex)
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2::", ex);
                                        //////// -----------------------------------BATCH MODE------------------------------------------------------------

                                    }
                                }
                            }
                        }
                    }
                    DbCommand command = AccessHelper.CreateCommand("Report_Informe_Detallado_Usuario");
                    command.CommandType = CommandType.StoredProcedure;

                    AccessHelper.AddParameter(command, "@p_begin", DbType.Date).Value = StartDate;
                    AccessHelper.AddParameter(command, "@p_end", DbType.Date).Value = EndDate;
                    AccessHelper.AddParameter(command, "@IdReportTask", DbType.Int64).Value = IDReportTask;
                    AccessHelper.AddParameter(command, "@IDsEmp", DbType.String, 100000).Value = commaSeparatedIDEmployees;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeev2::", ex);
            }

        }

        //Se usa!
        private bool CreateTmpSchedulingLayerResumev2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }
            if (empSelected.Length == 0) empSelected = new string[] { "-1" };


            int monthStart = Convert.ToInt16(parametersList.Where(x => x.Name.Equals("monthStart")).FirstOrDefault().Value);
            int yearStart = Convert.ToInt16(parametersList.Where(x => x.Name.Equals("yearStart")).FirstOrDefault().Value);
            int monthEnd = Convert.ToInt16(parametersList.Where(x => x.Name.Equals("monthEnd")).FirstOrDefault().Value);
            int yearEnd = Convert.ToInt16(parametersList.Where(x => x.Name.Equals("yearEnd")).FirstOrDefault().Value);
            DateTime StartDate = new DateTime(yearStart, monthStart, 1);
            DateTime EndDate = new DateTime(yearEnd, monthEnd, DateTime.DaysInMonth(yearEnd, monthEnd));

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;

            //Parámetros estáticos definidos en el report original
            string layer0_Definition = "Mañana@=06:00@=14:00";
            string layer1_Definition = "Mediodia@=14:00@=17:00";
            string layer2_Definition = "Tarde@=17:00@+06:00";
            double Layer_Stretch = 2;

            int IDGroup;
            var roGroups = new DataTable();
            var roEmployees = new DataTable();
            var roSchedule = new DataTable();
            string sEmployeeFilter = "";
            double[] layer0 = new double[31];
            double[] layer1 = new double[31];
            double[] layer2 = new double[31];

            long lastDate = -1;
            double currentDate;
            long lastEmployee = -1;
            long currentEmployee;
            long lastShift = -1;
            long currentShift;
            string[] shiftLayerDefinitions = new string[0];
            string validLayers;
            
            long lngPosition;
            DateTime lastDateInsert = DateTime.MinValue;

            try
            {

                //Borramos los registros anteriores
                AccessHelper.ExecuteSql($"@DELETE# TmpSchedulingLayerResume WHERE IDReportTask = {IDReportTask} OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);

                //Obtenemos el listado de grupos para el informe
                sSQL = "@SELECT# distinct sysrovwAllEmployeeGroups.IDGroup FROM sysrovwAllEmployeeGroups";
                sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                    " AND poe.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee And convert(date, getdate()) between poe.BeginDate And poe.EndDate " +
                    " AND poe.IDEmployee IN (" + empFilter + ") and poe.IdFeature = 1 and poe.FeaturePermission > 1 ";

                roGroups = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if(roGroups.Rows.Count == 0) return false;

                // Parseamos las franjas configuradas
                var layer0Period = new roTimePeriod();
                var layer1Period = new roTimePeriod();
                var layer2Period = new roTimePeriod();
                if (!ParseLayers(layer0_Definition, layer1_Definition, layer2_Definition, ref layer0Period, ref layer1Period, ref layer2Period))
                {
                    return false;
                }

                foreach (DataRow groupsRow in roGroups.Rows)
                {
                    IDGroup = Convert.ToInt32(groupsRow["IDGroup"]);

                    //Inicializamos los arrays de datos
                    Array.Clear(layer0, 0, layer0.Length);
                    Array.Clear(layer1, 0, layer1.Length);
                    Array.Clear(layer2, 0, layer2.Length);

                    //Obtenemos todos los empleados planificados para el grupo en question.
                    sEmployeeFilter = "";
                    sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, " +
                   "(CASE WHEN cast(sysroEmployeeGroups.BeginDate as date) >=  cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) THEN sysroEmployeeGroups.BeginDate ELSE  cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) END) AS ReallyStart, " +
                   "(CASE WHEN cast(sysroEmployeeGroups.EndDate as date) >  cast(" + roTypes.Any2Time(EndDate).SQLSmallDateTime() + " as date) THEN  cast(" + roTypes.Any2Time(EndDate).SQLSmallDateTime() + " as date) ELSE sysroEmployeeGroups.EndDate END) AS ReallyEnd " +
                   "FROM sysroEmployeeGroups " +
                    " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                    " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And convert(date, getdate()) between poe.BeginDate And poe.EndDate " +
                    " AND poe.IDEmployee IN (" + empFilter + ") and poe.IdFeature = 1 and poe.FeaturePermission > 1 " +
                    "WHERE IDGroup = " + IDGroup +
                   " AND ((cast(sysroEmployeeGroups.BeginDate as date) BETWEEN  cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) AND  cast(" + roTypes.Any2Time(EndDate).SQLSmallDateTime() + " as date)) " +
                   "OR (cast(sysroEmployeeGroups.BeginDate as date) <=  cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) AND cast(sysroEmployeeGroups.EndDate as date) >=  cast(" + roTypes.Any2Time(EndDate).SQLSmallDateTime() + " as date)) " +
                   "OR (cast(sysroEmployeeGroups.EndDate as date) BETWEEN  cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) AND  cast(" + roTypes.Any2Time(EndDate).SQLSmallDateTime() + " as date)))";


                    roEmployees = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                    if (roEmployees.Rows.Count == 0) return false;

                    foreach (DataRow employeeRow in roEmployees.Rows)
                    {
                        if(sEmployeeFilter != "") sEmployeeFilter += " OR ";
                        sEmployeeFilter += "(sysrovwAllEmployeeGroups.IDEmployee = " + employeeRow["IDEmployee"] + " AND DailySchedule.Date >= " + roTypes.Any2Time(employeeRow["ReallyStart"]).SQLSmallDateTime() + " AND DailySchedule.Date <= " + roTypes.Any2Time(employeeRow["ReallyEnd"]).SQLSmallDateTime() + ")";
                    }

                    if (sEmployeeFilter != "")
                    {
                        //Obtenemos la planificación de los empleados para el dia y grupo
                        sSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, DailySchedule.Date, ISNULL(DailySchedule.IDShiftUsed, DailySchedule.IDShift1) AS ShiftUsed, sl.Definition " +
                    " FROM sysrovwAllEmployeeGroups";
                        sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                            " AND poe.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee And convert(date, getdate()) between poe.BeginDate And poe.EndDate " +
                            " AND poe.IDEmployee IN (" + empFilter + ") and poe.IdFeature = 1 and poe.FeaturePermission > 1 ";
                        sSQL += " INNER JOIN DailySchedule ON sysrovwAllEmployeeGroups.IDEmployee = DailySchedule.IDEmployee" +
                        " INNER JOIN sysroShiftsLayers sl ON ISNULL(DailySchedule.IDShiftUsed, DailySchedule.IDShift1) = sl.IDShift and sl.IDType = 1100 " +
                        " WHERE (" + sEmployeeFilter + ") " +
                         "order by DailySchedule.Date ASC, sysrovwAllEmployeeGroups.IDEmployee ASC";

                        roSchedule = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (roSchedule.Rows.Count == 0) return false;
                        lastDate = -1;
                        lastShift = -1;
                        lastEmployee = -1;
                        shiftLayerDefinitions = new string[0];

                        foreach (DataRow scheduleRow in roSchedule.Rows)
                        {
                            currentEmployee = Convert.ToInt32(scheduleRow["IDEmployee"]);
                            currentDate = Convert.ToDateTime(scheduleRow["Date"]).Day;
                            currentShift = Convert.ToInt32(scheduleRow["ShiftUsed"]);

                            if (lastEmployee != -1 && lastDate != -1 && (lastEmployee != currentEmployee || lastDate != currentDate))
                            {
                                //Si no es el primer registro y cambia de empleado o dia guardo el registro
                                validLayers = GetLayersFromShift(shiftLayerDefinitions, layer0Period, layer1Period, layer2Period, Layer_Stretch);
                                if (validLayers.Contains("0"))
                                {
                                    layer0[lastDate] += 1;
                                }
                                if (validLayers.Contains("1"))
                                {
                                    layer1[lastDate] += 1;
                                }
                                if (validLayers.Contains("2"))
                                {
                                    layer2[lastDate] += 1;
                                }

                                //Reiniciamos el registro de franjas
                                shiftLayerDefinitions = new string[0];
                            }

                            //Si la fecha actual es mas pequeña que la fecha anterior se ha cambiado de mes y debemos insertar el registro en base de datos
                            if (currentDate < lastDate)
                            {
                                sSQL = "@INSERT# INTO TmpSchedulingLayerResume VALUES(" + IDGroup + "," + roTypes.Any2Time(lastDateInsert).SQLSmallDateTime();

                                for (lngPosition = 0; lngPosition < layer0.Length; lngPosition++)
                                {
                                    sSQL += "," + layer0[lngPosition];
                                }

                                for (lngPosition = 0; lngPosition < layer1.Length; lngPosition++)
                                {
                                    sSQL += "," + layer1[lngPosition];
                                }

                                for (lngPosition = 0; lngPosition < layer2.Length; lngPosition++)
                                {
                                    sSQL += "," + layer2[lngPosition];
                                }

                                sSQL += "," + IDReportTask + ")";

                                // Inicializamos los arrays de datos
                                for (int i = 0; i < 31; i++)
                                {
                                    layer0[i] = 0;
                                    layer1[i] = 0;
                                    layer2[i] = 0;
                                }

                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }


                            // Si es el mismo empleado, horario y día, es una nueva franja y redimensiono el array para guardarla
                            if (lastEmployee == currentEmployee && lastShift == currentShift && lastDate == currentDate)
                            {
                                Array.Resize(ref shiftLayerDefinitions, shiftLayerDefinitions.Length + 1);
                            }

                            // Asignamos el valor a la última posición del array
                            if(shiftLayerDefinitions.Length == 0) Array.Resize(ref shiftLayerDefinitions, shiftLayerDefinitions.Length + 1);
                            shiftLayerDefinitions[shiftLayerDefinitions.Length - 1] = roTypes.Any2String(scheduleRow["Definition"]);

                            // Actualizamos las variables de control
                            lastDate = roTypes.Any2Long(currentDate)-1;
                            lastShift = currentShift;
                            lastEmployee = currentEmployee;
                            lastDateInsert = roTypes.Any2DateTime(scheduleRow["Date"]);
                        }

                        //Solo si había registros inicialmente para el grupo ...
                        if (lastDateInsert.Year != 1899)
                        {
                            validLayers = GetLayersFromShift(shiftLayerDefinitions, layer0Period, layer1Period, layer2Period, Layer_Stretch);

                            if (validLayers.Contains("0"))
                            {
                                layer0[lastDate] = layer0[lastDate] + 1;
                            }

                            if (validLayers.Contains("1"))
                            {
                                layer1[lastDate] = layer1[lastDate] + 1;
                            }

                            if (validLayers.Contains("2"))
                            {
                                layer2[lastDate] = layer2[lastDate] + 1;
                            }

                            sSQL = "@INSERT# INTO TmpSchedulingLayerResume VALUES(" + IDGroup + "," + roTypes.Any2Time(lastDateInsert).SQLSmallDateTime();

                            for (lngPosition = 0; lngPosition < layer0.Length; lngPosition++)
                            {
                                sSQL += "," + layer0[lngPosition];
                            }

                            for (lngPosition = 0; lngPosition < layer1.Length; lngPosition++)
                            {
                                sSQL += "," + layer1[lngPosition];
                            }

                            for (lngPosition = 0; lngPosition < layer2.Length; lngPosition++)
                            {
                                sSQL += "," + layer2[lngPosition];
                            }

                            sSQL += "," + IDReportTask + ")";

                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                        

                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpSchedulingLayerResumev2::", ex);
            }
            return false;
        }

        private void CreateTmpDetailedCalendarEmployeeLear(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {                                    
            var dsPunches = new DataTable();
            DataTable dsInfo = new DataTable();
            DataTable sQuery;

            DateTime StartWorkDate;
            DateTime EndWorkDate;

            string sSQL;            
            string strConceptList;
            byte IndexConcept;            

            double[] ConceptValues;                                                                           

            string strInsert;
            string strValues;
            string strFields;
            double Rendimiento;
            double Paro;
            double HorasrendimientoCien;            

            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            string commaSeparatedIDEmployees;

            try
            {                             
                //Borramos los registros anteriores
                AccessHelper.ExecuteSql($"@DELETE# TMPDetailedCalendarEmployeePagosLear WHERE IDReportTask = {IDReportTask} OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);
                if (empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IdGroup, Employees.Name AS EmployeeName, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,'NIF', getdate()),'') as FieldValue FROM sysroEmployeeGroups inner join Employees on employees.ID = sysroEmployeeGroups.IDEmployee ";
                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And cast(" + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " as date) between cast(poe.BeginDate as date) And cast(poe.EndDate as date) " +
                        " AND poe.IDEmployee IN (" + empFilter + ") " +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1 ";
                }
                if (empSelected == null)
                {
                    sSQL = sSQL + "WHERE sysroEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + " ORDER BY Employees.Name, sysroEmployeeGroups.BeginDate ";
                sSQL = sSQL.Replace("sysrovwAllEmployeeGroups", "sysroEmployeeGroups");

                roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeevLear:: " + sSQL);
                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    commaSeparatedIDEmployees = string.Join(",", sQuery.AsEnumerable().Where(row => !row.IsNull("IDEmployee")).Select(row => Robotics.VTBase.roTypes.Any2String(row.Field<int>("IDEmployee"))));

                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        StartWorkDate = StartDate;
                        EndWorkDate = EndDate;

                        // Selecciona datos del empleado
                        sSQL = "@select# DATES.Date, EC.idEmployee, EC.idContract, ISNULL(DS.IDShiftUsed, DS.IDShift1) AS IDShift, " +
                               " (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " + sQueryDataRow["IdEmployee"] + " AND " +
                               " EmployeeUserFieldValues.FieldName = 'Horas paro' AND EmployeeUserFieldValues.Date <= DATES.Date " +
                               " ORDER BY EmployeeUserFieldValues.Date DESC) HorasParo, " +
                               " (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " + sQueryDataRow["IdEmployee"] + " AND " +
                               " EmployeeUserFieldValues.FieldName = '% Rto' AND EmployeeUserFieldValues.Date <= DATES.Date " +
                               " ORDER BY EmployeeUserFieldValues.Date DESC) Rendimiento, " +
                               " (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " + sQueryDataRow["IdEmployee"] + " AND " +
                               " EmployeeUserFieldValues.FieldName = '% Estimulo' AND EmployeeUserFieldValues.Date <= DATES.Date " +
                               " ORDER BY EmployeeUserFieldValues.Date DESC) Estimulo " +
                               "from dbo.ExplodeDates('" + StartWorkDate.ToString("yyyyMMdd") + "','" + EndWorkDate.ToString("yyyyMMdd") + "') as DATES " +
                               " Left JOIN EmployeeContracts EC on EC.idEmployee=" + sQueryDataRow["IdEmployee"] + " and DATES.Date between BeginDate and EndDate " +
                               " Left JOIN DailySchedule DS on DS.IDEmployee=EC.idEmployee and Dates.Date=DS.date " +
                               "where not EC.idContract is null " +
                               "order by ec.idContract,dates.date OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))";
                        //DoEvents();

                        roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeevLear:: " + sSQL);
                        dsInfo = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        //DoEvents();

                        if (dsInfo != null)
                        {
                            foreach (DataRow sQueryDataInfo in dsInfo.Rows)
                            {
                                // para cada dia
                                // Si el empleado en esa fecha tiene contrato
                                if (roTypes.Any2String(sQueryDataInfo[2]) != "")
                                {
                                    strValues = roTypes.Any2String(IDReportTask);
                                    strFields = "idReportTask";

                                    strValues += ",0";
                                    strFields += ",IDGroup";

                                    strValues += "," + sQueryDataInfo[1];
                                    strFields += ",IDEmployee";

                                    strValues += "," + roTypes.Any2Time(sQueryDataInfo[0]).SQLSmallDateTime();
                                    strFields += ",DatePlanification";

                                    strValues += ",'Grupo' ";
                                    strFields += ",GroupName";

                                    strValues += ",'" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'";
                                    strFields += ",EmployeeName";

                                    // OBTENER SALDOS
                                    strConceptList = "'HTr','55','chx','ENF','PRE'";
                                    ConceptValues = GetAccrualsFromListLear(roTypes.Any2Integer(sQueryDataInfo[1]), strConceptList, roTypes.Any2DateTime(sQueryDataInfo[0]), roTypes.Any2DateTime(sQueryDataInfo[0]), 5);
                                    //DoEvents();

                                    // Horas trabajadas
                                    IndexConcept = 1;
                                    strValues += "," + ConceptValues[0].ToString().Replace(",", ".");
                                    strFields += "," + "ValueConcept" + IndexConcept;

                                    // Otros () --> Grupo de saldos OTROS (obtener el nombre del primero que salga)
                                    sSQL = "@SELECT# top 1 ISNULL(NAME, '') FROM CONCEPTS WHERE ID IN(@SELECT# TOP 1 DAILYACCRUALS.IDConcept FROM DAILYACCRUALS WHERE IDEmployee= " + sQueryDataRow["IdEmployee"] + " AND Date=" + roTypes.Any2Time(sQueryDataInfo[0]).SQLSmallDateTime() + " AND IDConcept IN(@SELECT# ID FROM CONCEPTS WHERE ID IN(@SELECT# IDCONCEPT FROM sysroReportGroupConcepts WHERE IDReportGroup in(@SELECT# ID FROM sysroReportGroups WHERE Name LIKE 'OTROS'))) ORDER BY DAILYACCRUALS.VALUE DESC) ";
                                    strValues += ",'" + roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)).Replace("'", "''") + "'";
                                    strFields += "," + "Otros";

                                    // Horas extras n
                                    IndexConcept = 2;
                                    strValues += "," + ConceptValues[1].ToString().Replace(",", ".");
                                    strFields += "," + "ValueConcept" + IndexConcept;

                                    // Horas extras e
                                    IndexConcept = 3;
                                    strValues += "," + ConceptValues[2].ToString().Replace(",", ".");
                                    strFields += "," + "ValueConcept" + IndexConcept;

                                    // Horas extras nocturnas y festivas
                                    IndexConcept = 4;
                                    strValues += "," + ConceptValues[3].ToString().Replace(",", ".");
                                    strFields += "," + "ValueConcept" + IndexConcept;

                                    // Horas presencia
                                    IndexConcept = 5;
                                    strValues += "," + ConceptValues[4].ToString().Replace(",", ".");
                                    strFields += "," + "ValueConcept" + IndexConcept;

                                    // % Rendimiento (campo de la ficha) (140)
                                    System.Globalization.NumberFormatInfo oInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                            Rendimiento = roTypes.Any2Double(roTypes.Any2String(sQueryDataInfo[5]).Replace(".", oInfo.CurrencyDecimalSeparator));
                                    strValues += "," + Rendimiento.ToString().Replace(",", ".");
                                    strFields += "," + "Rendimiento";

                                    // Horas de paro (campo de la ficha) (0,34)
                                    if (ConceptValues[4] < 8)
                                    {
                                        Paro = 0;
                                    }
                                    else
                                    {
                                        Paro = roTypes.Any2Double(roTypes.Any2String(sQueryDataInfo[4]).Replace(".", oInfo.CurrencyDecimalSeparator));
                                    }
                                    strValues += "," + Paro.ToString().Replace(",", ".");
                                    strFields += "," + "Paro";

                                    // % Estimulo
                                    strValues += "," + roTypes.Any2Double(roTypes.Any2String(sQueryDataInfo[6]).Replace(".", oInfo.CurrencyDecimalSeparator)).ToString().Replace(",", ".");
                                    strFields += "," + "Estimulo";

                                    // Horas rendimiento 100% (Horas presencia - horas paro) * rendimiento
                                    HorasrendimientoCien = ConceptValues[4] - Paro;
                                    if (Rendimiento > 0)
                                    {
                                        HorasrendimientoCien = HorasrendimientoCien * (Rendimiento / 100);
                                    }
                                    else
                                    {
                                        HorasrendimientoCien = 0;
                                    }
                                    strValues += "," + HorasrendimientoCien.ToString().Replace(",", ".");
                                    strFields += "," + "HorasCien";

                                    // Absentismos --> Grupo de saldos ABSENTISMO --> Obtener nombre corto saldo y valor de los 3 primeros
                                    // Index 6, 7 , 8
                                    sSQL = "@SELECT# TOP 3 Concepts.SHORTNAME , ISNULL(SUM(DAILYACCRUALS.VALUE), 0) FROM DAILYACCRUALS, Concepts WHERE IDEmployee= " + sQueryDataRow["IdEmployee"] + " AND Date=" + roTypes.Any2Time(sQueryDataInfo[0]).SQLSmallDateTime() + " AND IDConcept IN(@SELECT# ID FROM CONCEPTS WHERE ID IN(@SELECT# IDCONCEPT FROM sysroReportGroupConcepts WHERE IDReportGroup in(@SELECT# ID FROM sysroReportGroups WHERE Name LIKE 'ABSENTISMO'))) and DAILYACCRUALS.IDConcept = Concepts.ID GROUP BY Concepts.SHORTNAME ORDER BY SUM(DAILYACCRUALS.VALUE) DESC";
                                    dsPunches = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                                    if (dsPunches != null)
                                    {
                                        foreach (DataRow punchInfo in dsPunches.Rows)
                                        {
                                            IndexConcept++;
                                            strValues += "," + punchInfo[1].ToString().Replace(",", ".");
                                            strFields += "," + "ValueConcept" + IndexConcept;
                                            strValues += ",'" + punchInfo[0].ToString().Replace("'", "''") + "'";
                                            strFields += "," + "ConceptName" + IndexConcept;
                                        }
                                    }

                                    // OBTENER HORARIO
                                    if (sQueryDataInfo[3] != null)
                                    {
                                        strValues += "," + roTypes.Any2Integer(sQueryDataInfo[3]);
                                        strFields += "," + "IDShift";
                                    }

                                    strInsert = "@insert# into TMPDetailedCalendarEmployeePagosLear ( " + strFields + ") Values (";
                                    strInsert += strValues + ")";
                                    // Ejecuta el SQL para el bloque actual
                                    try
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeevLear:: " + strInsert);
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(strInsert);
                                    }
                                    catch (Exception ex)
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeevLear::", ex);

                                    }
                                }
                                //DoEvents();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpDetailedCalendarEmployeevLear::Error ", ex);                                
            }            
        }


        private bool ParseLayers(string layer0_Definition, string layer1_Definition, string layer2_Definition, ref roTimePeriod layer0Period, ref roTimePeriod layer1Period, ref roTimePeriod layer2Period)
        {
            string[] layer0Definition;
            string[] layer1Definition;
            string[] layer2Definition;

            try
            {
                // Parseamos la definición de franjas
                layer0Definition = layer0_Definition.Split('@');
                layer1Definition = layer1_Definition.Split('@');
                layer2Definition = layer2_Definition.Split('@');

                if (layer0Definition[1].Contains("-"))
                {
                    layer0Period.Begin.Value = roTypes.Any2Time("1899/12/29 " + layer0Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer0Definition[1].Contains("="))
                {
                    layer0Period.Begin.Value = roTypes.Any2Time("1899/12/30 " + layer0Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer0Definition[1].Contains("+"))
                {
                    layer0Period.Begin.Value = roTypes.Any2Time("1899/12/31 " + layer0Definition[1].Substring(1, 5) + ":00").Value;
                }

                if (layer0Definition[2].Contains("-"))
                {
                    layer0Period.Finish.Value = roTypes.Any2Time("1899/12/29 " + layer0Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer0Definition[2].Contains("="))
                {
                    layer0Period.Finish.Value = roTypes.Any2Time("1899/12/30 " + layer0Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer0Definition[2].Contains("+"))
                {
                    layer0Period.Finish.Value = roTypes.Any2Time("1899/12/31 " + layer0Definition[2].Substring(1, 5) + ":00").Value;
                }

                if (layer1Definition[1].Contains("-"))
                {
                    layer1Period.Begin.Value = roTypes.Any2Time("1899/12/29 " + layer1Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer1Definition[1].Contains("="))
                {
                    layer1Period.Begin.Value = roTypes.Any2Time("1899/12/30 " + layer1Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer1Definition[1].Contains("+"))
                {
                    layer1Period.Begin.Value = roTypes.Any2Time("1899/12/31 " + layer1Definition[1].Substring(1, 5) + ":00").Value;
                }

                if (layer1Definition[2].Contains("-"))
                {
                    layer1Period.Finish.Value = roTypes.Any2Time("1899/12/29 " + layer1Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer1Definition[2].Contains("="))
                {
                    layer1Period.Finish.Value = roTypes.Any2Time("1899/12/30 " + layer1Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer1Definition[2].Contains("+"))
                {
                    layer1Period.Finish.Value = roTypes.Any2Time("1899/12/31 " + layer1Definition[2].Substring(1, 5) + ":00").Value;
                }

                if (layer2Definition[1].Contains("-"))
                {
                    layer2Period.Begin.Value = roTypes.Any2Time("1899/12/29 " + layer2Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer2Definition[1].Contains("="))
                {
                    layer2Period.Begin.Value = roTypes.Any2Time("1899/12/30 " + layer2Definition[1].Substring(1, 5) + ":00").Value;
                }
                else if (layer2Definition[1].Contains("+"))
                {
                    layer2Period.Begin.Value = roTypes.Any2Time("1899/12/31 " + layer2Definition[1].Substring(1, 5) + ":00").Value;
                }

                if (layer2Definition[2].Contains("-"))
                {
                    layer2Period.Finish.Value = roTypes.Any2Time("1899/12/29 " + layer2Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer2Definition[2].Contains("="))
                {
                    layer2Period.Finish.Value = roTypes.Any2Time("1899/12/30 " + layer2Definition[2].Substring(1, 5) + ":00").Value;
                }
                else if (layer2Definition[2].Contains("+"))
                {
                    layer2Period.Finish.Value = roTypes.Any2Time("1899/12/31 " + layer2Definition[2].Substring(1, 5) + ":00").Value;
                }

                return true;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::ParseLayers::", ex);
                return false;
            }
        }

        private string GetLayersFromShift(string[] layerDefinition, roTimePeriod layer0Period, roTimePeriod layer1Period, roTimePeriod layer2Period, double Layer_Stretch)
        {
            string validLayers = "";
            double layer0Stretch = 0;
            double layer1Stretch = 0;
            double layer2Stretch = 0;

            try
            {
                foreach (var definition in layerDefinition)
                {
                    var layerPeriod = new roTimePeriod();
                    var shiftDefinition = new roCollection();
                    shiftDefinition.LoadXMLString(definition);

                    // Inicio franja rijida
                    if (shiftDefinition.Exists("FloatingBeginUpto"))
                    {
                        layerPeriod.Begin.Value = roTypes.Any2Time(shiftDefinition["FloatingBeginUpto"]).Value;
                    }
                    else
                    {
                        layerPeriod.Begin.Value =  roTypes.Any2Time(shiftDefinition["Begin"]).Value;
                    }

                    // Fin franja rijida
                    if (shiftDefinition.Exists("FloatingFinishMinutes"))
                    {
                        layerPeriod.Finish.Value = layerPeriod.Begin.Add(roTypes.Any2Long(shiftDefinition["FloatingFinishMinutes"]), "n").Value;
                    }
                    else
                    {
                        layerPeriod.Finish.Value =  roTypes.Any2Time(shiftDefinition["Finish"]).Value;
                    }

                    // Comparamos con layer0
                    var mergePeriod = new roTimePeriod
                    {
                        Begin = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Begin.Value), roTypes.Any2DateTime(layer0Period.Begin.Value)) > 0 ? layerPeriod.Begin.Value : layer0Period.Begin.Value },
                        Finish = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Finish.Value), roTypes.Any2DateTime(layer0Period.Finish.Value)) < 0 ? layerPeriod.Finish.Value : layer0Period.Finish.Value }
                    };

                    var intervalDuration = roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Finish.Value)) - roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Begin.Value));
                    if (intervalDuration > 0)
                    {
                        layer0Stretch += intervalDuration;
                    }

                    // Comparamos con layer1
                    mergePeriod = new roTimePeriod
                    {
                        Begin = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Begin.Value), roTypes.Any2DateTime(layer1Period.Begin.Value)) > 0 ? layerPeriod.Begin.Value : layer1Period.Begin.Value },
                        Finish = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Finish.Value), roTypes.Any2DateTime(layer1Period.Finish.Value)) < 0 ? layerPeriod.Finish.Value : layer1Period.Finish.Value }
                    };

                    intervalDuration = roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Finish.Value)) - roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Begin.Value));
                    if (intervalDuration > 0)
                    {
                        layer1Stretch += intervalDuration;
                    }

                    // Comparamos con layer2
                    mergePeriod = new roTimePeriod
                    {
                        Begin = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Begin.Value), roTypes.Any2DateTime(layer2Period.Begin.Value)) > 0 ? layerPeriod.Begin.Value : layer2Period.Begin.Value },
                        Finish = { Value = DateTime.Compare(roTypes.Any2DateTime(layerPeriod.Finish.Value), roTypes.Any2DateTime(layer2Period.Finish.Value)) < 0 ? layerPeriod.Finish.Value : layer2Period.Finish.Value }
                    };

                    intervalDuration = roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Finish.Value)) - roTypes.DateTime2Double(roTypes.Any2DateTime(mergePeriod.Begin.Value));
                    if (intervalDuration > 0)
                    {
                        layer2Stretch += intervalDuration;
                    }
                }

                if (layer0Stretch >= Layer_Stretch)
                {
                    validLayers += "0";
                }

                if (layer1Stretch >= Layer_Stretch)
                {
                    validLayers += "1";
                }

                if (layer2Stretch >= Layer_Stretch)
                {
                    validLayers += "2";
                }

                return validLayers;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::GetLayersFromShift::", ex);
                return "";
            }
        }

        private void CreateTmpDailyCalendarEmployeev2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {

            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            string[] IDConcept = parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptsSelector")).FirstOrDefault().Value.ToString().Split(',');
            string[] IdUserFields = parametersList.Where(x => x.Type.Equals("Robotics.Base.userFieldsSelector")).FirstOrDefault().Value.ToString().Split(',');

            string IdUserField = "";
            if (IdUserFields.Length > 0)
                IdUserField = IdUserFields.First();

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            DataTable dsInfo;

            roCollection Result = new roCollection();

            DateTime StartWorkDate;
            DateTime EndWorkDate;

            string strInsert;
            string strValues;
            string strFields;

            double[] ConceptValues;
            int IndexConcepts;
            int IndexConcept;

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDailyCalendarEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);
                if (empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                string strEmployeesAndGroupsSelected = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString();
                string strEmployeesSelectedDirect = "-1";
                string strEmployeesSelectedByGroup = empFilter;
                if (strEmployeesAndGroupsSelected != null && strEmployeesAndGroupsSelected != "" && strEmployeesAndGroupsSelected.Contains("B"))
                {
                    string[] employeesSelectedDirect = strEmployeesAndGroupsSelected.Split('@')[0].Split(',');
                    employeesSelectedDirect = employeesSelectedDirect.Where(x => x.StartsWith("B")).ToArray();
                    foreach (string employee in employeesSelectedDirect)
                    {
                        if (strEmployeesSelectedDirect == "")
                            strEmployeesSelectedDirect = employee.Replace("B", "");
                        else
                            strEmployeesSelectedDirect = strEmployeesSelectedDirect + "," + employee.Replace("B", "");
                    }
                    string[] employeesSelectedByGroup = empSelected.Where(x => !strEmployeesSelectedDirect.Contains(x)).ToArray();
                    if (employeesSelectedByGroup != null && employeesSelectedByGroup.Length > 0)
                        strEmployeesSelectedByGroup = string.Join(",", employeesSelectedByGroup);
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IdGroup, Employees.Name AS EmployeeName, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,'NIF', getdate()),'') as FieldValue, isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,(@SELECT# FIELDNAME FROM sysroUserFields WHERE ID = '" + IdUserField + "'), getdate()),'') as UserFieldValue, (@SELECT# FIELDNAME FROM sysroUserFields WHERE ID = '" + IdUserField + "') as UserFieldName FROM sysroEmployeeGroups inner join Employees on employees.ID = sysroEmployeeGroups.IDEmployee ";
                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between poe.BeginDate And poe.EndDate " +
                        " AND (poe.IDEmployee IN (" + strEmployeesSelectedDirect + ") OR (poe.IDEmployee IN (" + strEmployeesSelectedByGroup + ") and cast(GETDATE() as date) between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate))" +
                        " AND poe.IdFeature = 1 AND poe.FeaturePermission > 1";
                }
                if (empSelected == null)
                {
                    sSQL = sSQL + "WHERE sysroEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + " ORDER BY Employees.Name, sysroEmployeeGroups.BeginDate ";
                sSQL = sSQL.Replace("sysrovwAllEmployeeGroups", "sysroEmployeeGroups");

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        if (((DateTime)sQueryDataRow["BeginDate"] >= StartDate && (DateTime)sQueryDataRow["BeginDate"] <= EndDate) ||
                                   ((DateTime)sQueryDataRow["EndDate"] >= StartDate && (DateTime)sQueryDataRow["EndDate"] <= EndDate) ||
                                            ((DateTime)sQueryDataRow["BeginDate"] <= StartDate && (DateTime)sQueryDataRow["EndDate"] >= EndDate))
                        {
                            if ((DateTime)sQueryDataRow["BeginDate"] < StartDate)
                            {
                                StartWorkDate = StartDate;
                            }
                            else
                            {
                                StartWorkDate = (DateTime)sQueryDataRow["BeginDate"];
                            }

                            if ((DateTime)sQueryDataRow["EndDate"] > EndDate)
                            {
                                EndWorkDate = EndDate;
                            }
                            else
                            {
                                EndWorkDate = (DateTime)sQueryDataRow["EndDate"];
                            }

                            // Selecciona datos del empleado
                            sSQL = "@select# DATES.Date, EC.idEmployee, EC.idContract, ISNULL(DS.IDShiftUsed, ISNULL(DS.IDShift1, 0)) AS IDShift " +
                                           "from dbo.ExplodeDates('" + StartWorkDate.ToString("yyyyMMdd") + "','" + EndWorkDate.ToString("yyyyMMdd") + "') as DATES " +
                                           "    Left JOIN EmployeeContracts EC on EC.idEmployee=" + sQueryDataRow["IDEmployee"] + " and DATES.Date between BeginDate and EndDate " +
                                           "    Left JOIN DailySchedule DS on DS.IDEmployee=EC.idEmployee and Dates.Date=DS.date " +
                                           "where not EC.idContract is null " +
                                           "order by ec.idContract,dates.date";

                            dsInfo = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                            if (dsInfo != null)
                            {
                                foreach (DataRow sQueryDataInfo in dsInfo.Rows)
                                {
                                    // Si el empleado en esa fecha tiene contrato
                                    if (!roTypes.Any2String(sQueryDataInfo["idcontract"]).Equals(String.Empty))
                                    {
                                        strValues = roTypes.Any2String(IDReportTask);
                                        strFields = "IDReportTask";

                                        strValues = strValues + "," + sQueryDataRow["IDGroup"];
                                        strFields = strFields + ",IDGroup";

                                        strValues = strValues + "," + sQueryDataRow["IDEmployee"];
                                        strFields = strFields + ",IDEmployee";

                                        strValues = strValues + "," + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLSmallDateTime();
                                        strFields = strFields + ",DatePlanification";

                                        strValues = strValues + ",'" + sQueryDataRow["FullGroupName"].ToString().Replace("'", "''") + "' ";
                                        strFields = strFields + ",GroupName";

                                        strValues = strValues + ",'" + sQueryDataRow["EmployeeName"].ToString().Replace("'", "''") + "' ";
                                        strFields = strFields + ",EmployeeName";

                                        strValues = strValues + ",'" + (sQueryDataRow["UserFieldValue"].ToString().Replace("'", "''") + " " + roTypes.Any2String(sQueryDataRow["FieldValue"]).Replace("'", "''") + "'");
                                        strFields = strFields + ",UserFieldValue";

                                        strValues = strValues + ",'" + (sQueryDataRow["UserFieldName"].ToString().Replace("'", "''") + " " + roTypes.Any2String(sQueryDataRow["FieldValue"]).Replace("'", "''") + "'");
                                        strFields = strFields + ",UserFieldName";

                                        IndexConcept = 1;

                                        ConceptValues = GetAccrualsFromListv2(roTypes.Any2Integer(sQueryDataRow["IDEmployee"]), string.Join(",", IDConcept), (DateTime)sQueryDataInfo["Date"], (DateTime)sQueryDataInfo["Date"], 10);


                                        for (IndexConcepts = 0; IndexConcepts < IDConcept.Length && IndexConcept <= 10; IndexConcepts++)
                                        {
                                            strValues = strValues + "," + IDConcept[IndexConcepts];
                                            strFields = strFields + "," + "IDConcept" + IndexConcept;

                                            strValues = strValues + ",'" + IDConcept[IndexConcepts] + "'";
                                            strFields = strFields + "," + "ConceptName" + IndexConcept;

                                            strValues = strValues + ",'H'";
                                            strFields = strFields + "," + "FormatConcept" + IndexConcept;

                                            strValues = strValues + "," + ConceptValues[IndexConcept - 1].ToString().Replace(",", ".");
                                            strFields = strFields + "," + "ValueConcept" + IndexConcept;

                                            IndexConcept = IndexConcept + 1;
                                        }

                                        //OBTENER PUNCHES
                                        sSQL = "@SELECT# distinct TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                               "IDEmployee = " + sQueryDataRow["IDEmployee"] + " AND ShiftDate= " + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLDateTime() + " AND ActualType = 1 " +
                                               "UNION ALL " +
                                               "@SELECT# distinct TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                               "IDEmployee = " + sQueryDataRow["IDEmployee"] + " AND ShiftDate= " + roTypes.Any2Time(sQueryDataInfo["Date"]).SQLDateTime() + " AND ActualType = 2 " +
                                               "ORDER BY ActualType, DateTime";

                                        //OBTENER HORARIO
                                        if (sQueryDataInfo["IDSHift"] != null)
                                        {
                                            strValues = strValues + "," + sQueryDataInfo["IDSHift"];
                                            strFields = strFields + "," + "IDShift";
                                        }

                                        //INCIDENCIA NO JUSTIFICADA
                                        strValues = strValues + ",1";
                                        strFields = strFields + "," + "Cause";

                                        strInsert = "@insert# into TMPDailyCalendarEmployee ( " + strFields + ") Values (";
                                        strInsert = strInsert + strValues + ")";
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(strInsert);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void VSL_CreateTmpAccrualsControlByEmployee(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {

            int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            int monthStart = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("monthStart")).FirstOrDefault().Value)));
            int yearStart = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("yearStart")).FirstOrDefault().Value)));
            int monthEnd = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("monthEnd")).FirstOrDefault().Value)));
            int yearEnd = Convert.ToInt16(((parametersList.Where(x => x.Name.Equals("yearEnd")).FirstOrDefault().Value)));
            DateTime startDate = new DateTime(yearStart, monthStart, 1);
            DateTime endDate = new DateTime(yearEnd, monthEnd, DateTime.DaysInMonth(yearEnd, monthEnd));                                  

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            DataTable dsInfo;

            roCollection Result = new roCollection();

            DateTime StartWorkDate;
            DateTime EndWorkDate;

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# tmpVSL_AnnualAccruals WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string empFilter = string.Join(",", empSelected);
                if (empFilter.Length == 0)
                {
                    empFilter = "-1";
                }

                string strEmployeesAndGroupsSelected = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString();
                string strEmployeesSelectedDirect = "-1";
                string strEmployeesSelectedByGroup = empFilter;
                if (strEmployeesAndGroupsSelected != null && strEmployeesAndGroupsSelected != "" && strEmployeesAndGroupsSelected.Contains("B"))
                {
                    string[] employeesSelectedDirect = strEmployeesAndGroupsSelected.Split('@')[0].Split(',');
                    employeesSelectedDirect = employeesSelectedDirect.Where(x => x.StartsWith("B")).ToArray();
                    foreach (string employee in employeesSelectedDirect)
                    {
                        if (strEmployeesSelectedDirect == "")
                            strEmployeesSelectedDirect = employee.Replace("B", "");
                        else
                            strEmployeesSelectedDirect = strEmployeesSelectedDirect + "," + employee.Replace("B", "");
                    }
                    string[] employeesSelectedByGroup = empSelected.Where(x => !strEmployeesSelectedDirect.Contains(x)).ToArray();
                    if (employeesSelectedByGroup != null && employeesSelectedByGroup.Length > 0)
                        strEmployeesSelectedByGroup = string.Join(",", employeesSelectedByGroup);
                }

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IdGroup, Employees.Name AS EmployeeName, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,'NIF', getdate()),'') as FieldValue, EmployeeContracts.IDContract FROM sysroEmployeeGroups inner join Employees on employees.ID = sysroEmployeeGroups.IDEmployee ";
                if (IDPassport > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) ON poe.IdPassport = " + IDPassport +
                        " AND poe.IDEmployee = sysroEmployeeGroups.IDEmployee And DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between poe.BeginDate And poe.EndDate " +
                        " AND (poe.IDEmployee IN (" + strEmployeesSelectedDirect + ") OR (poe.IDEmployee IN (" + strEmployeesSelectedByGroup + ") and cast(GETDATE() as date) between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate))";                        
                }
                sSQL += " inner join EmployeeContracts on EmployeeContracts.IDEmployee = Employees.ID and EmployeeContracts.BeginDate<='" + endDate.ToString("yyyyMMdd") + "' and EmployeeContracts.EndDate>='" + startDate.ToString("yyyyMMdd") + "'";
                if (empSelected == null)
                {
                    sSQL = sSQL + "WHERE sysroEmployeeGroups.IDEmployee IN(-1) AND ";
                }

                sSQL = sSQL + " ORDER BY Employees.Name, sysroEmployeeGroups.BeginDate ";
                sSQL = sSQL.Replace("sysrovwAllEmployeeGroups", "sysroEmployeeGroups");

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        if (((DateTime)sQueryDataRow["BeginDate"] >= startDate && (DateTime)sQueryDataRow["BeginDate"] <= endDate) ||
                                   ((DateTime)sQueryDataRow["EndDate"] >= startDate && (DateTime)sQueryDataRow["EndDate"] <= endDate) ||
                                            ((DateTime)sQueryDataRow["BeginDate"] <= startDate && (DateTime)sQueryDataRow["EndDate"] >= endDate))
                        {
                            if ((DateTime)sQueryDataRow["BeginDate"] < startDate)
                            {
                                StartWorkDate = startDate;
                            }
                            else
                            {
                                StartWorkDate = (DateTime)sQueryDataRow["BeginDate"];
                            }

                            if ((DateTime)sQueryDataRow["EndDate"] > endDate)
                            {
                                EndWorkDate = endDate;
                            }
                            else
                            {
                                EndWorkDate = (DateTime)sQueryDataRow["EndDate"];
                            }

                            // Selecciona datos del empleado                            
                            sSQL = "@SELECT# idConcept, Month(Date) as Mes, Sum(Value) as Total " +
                       "FROM dbo.DailyAccruals INNER JOIN dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID " +
                       "WHERE IDEmployee=" + sQueryDataRow["IDEmployee"] + " AND Date >= '" + StartWorkDate.ToString("yyyyMMdd") + "' AND Date <='" + EndWorkDate.ToString("yyyyMMdd") + "' AND Concepts.IsAbsentiism <> 0 AND StartupValue=0  AND CarryOver=0 " +
                       "GROUP BY dbo.DailyAccruals.IDConcept, MONTH(dbo.DailyAccruals.Date) " +
                       "ORDER BY dbo.DailyAccruals.IDConcept";
                            dsInfo = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                            int idSaldoAnt = 0;
                            double[] AccrualsByMonth = new double[12];
                            if (dsInfo != null)
                            {
                                
                                for (int i = 1; i < 12; i++)
                                {
                                    AccrualsByMonth[i] = 0;
                                }
                    
                                foreach (DataRow sQueryDataInfo in dsInfo.Rows)
                                {
                                    if (idSaldoAnt == 0)
                                        idSaldoAnt = roTypes.Any2Integer(sQueryDataInfo["idConcept"]);


                        // Si cambia el saldo inserta registro
                        if (idSaldoAnt != roTypes.Any2Integer(sQueryDataInfo["idConcept"])) {
                                        // Inserta el registro
                                        VSL_CreateTmpAccrualsControlByEmployee_InsertRegistry(roTypes.Any2Long(sQueryDataRow["IDEmployee"]), roTypes.Any2String(sQueryDataRow["idContract"]), StartWorkDate, EndWorkDate, idSaldoAnt, ref AccrualsByMonth, IDReportTask);

                                        idSaldoAnt = roTypes.Any2Integer(sQueryDataInfo["idConcept"]);
                            
                                            for (int i = 1; i < 12; i++)
                                        {
                                            AccrualsByMonth[i] = 0;
                                        }                            
                            
                        }
                                    AccrualsByMonth[roTypes.Any2Integer(sQueryDataInfo["Mes"]) - 1] = roTypes.Any2Double(sQueryDataInfo["Total"]);                        
                                }
                                VSL_CreateTmpAccrualsControlByEmployee_InsertRegistry(roTypes.Any2Long(sQueryDataRow["IDEmployee"]), roTypes.Any2String(sQueryDataRow["idContract"]), StartWorkDate, EndWorkDate, idSaldoAnt, ref AccrualsByMonth, IDReportTask);                                
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        public double GetUserFieldValueDouble(string FieldName, double IDEmployee, DateTime xDate)
        {
            string sSQL = "@select# top(1) substring(Value,1,15) as FieldValue from EmployeeUserFieldValues where idEmployee=" + IDEmployee + " and fieldName='" + FieldName + "' and " +
                          " date <= '" + roTypes.Any2DateTime(xDate).ToString("yyyyMMdd") + "'" +
                          " ORDER BY EmployeeUserFieldValues.Date DESC ";

            var tmpValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
            return tmpValue;
        }

        private double GetInitialAccrual(string AccrualShortName, double IDEmployee, DateTime DateIni, DateTime DateFin)
        {
            string sSQL = "@select# Value from DailyAccruals inner join Concepts " +
                          " on Concepts.ID = DailyAccruals.IDConcept" +
                          " where IDEmployee = " + IDEmployee + " AND Concepts.ShortName = '" + AccrualShortName + "' " +
                          " AND (date between '" + roTypes.Any2DateTime(DateIni).ToString("yyyyMMdd") + "' and '" + roTypes.Any2DateTime(DateFin).ToString("yyyyMMdd") + "') and startupvalue=1 and carryover=1";

            var tmpValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
            return tmpValue;
        }

        private double GetAccrual(string AccrualShortName, double IDEmployee, DateTime DateIni, DateTime DateFin)
        {
            string sSQL = "@select# Sum(Value) from DailyAccruals inner join Concepts " +
                          " on Concepts.ID = DailyAccruals.IDConcept" +
                          " where IDEmployee = " + IDEmployee + " AND Concepts.ShortName = '" + AccrualShortName + "' " +
                          " AND (date between '" + roTypes.Any2DateTime(DateIni).ToString("yyyyMMdd") + "' and '" + roTypes.Any2DateTime(DateFin).ToString("yyyyMMdd") + "') and startupvalue=0 and carryover=0";

            var tmpValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
            return tmpValue;
        }



        private bool VSL_CreateTmpAccrualsControlByEmployee_InsertRegistry(long IDEmployee, string IdContract, DateTime DateIni, DateTime DateFin, int IdConcept, ref double[] AccrualsByMonth, int idReportTask)
        {
            int i;
            string sSQL;

            double Holidays;
            double Antiquity;
            double HolidaysDays;
            double CurrentHolidays;
            double OldHolidays;
            double OldAntiquity;
            double HorasConvenioArrastradasAñoAnterior;
            double ValorInicialHorasConvenioAñoActual;
            double HorasConvenioAñoAnterior;
            double HorasConvenio;

            try
            {
                DateTime DateIniYear = new DateTime(DateIni.Year, 1, 1);
                if (DateIni > DateIniYear) DateIniYear = DateIni;
                DateTime DateFinYear = new DateTime(DateIni.Year, 12, 31);
                if (DateFinYear > DateFin) DateFinYear = DateFin;

                Holidays = GetUserFieldValueDouble("Vacaciones anuales", IDEmployee, DateFin);
                Antiquity = GetInitialAccrual("ANC", IDEmployee, DateIniYear, DateFinYear);
                CurrentHolidays = Holidays - GetAccrual("VAC", IDEmployee, DateIni, DateFin);
                OldHolidays = GetAccrual("VAN", IDEmployee, DateIni, DateFin);
                OldAntiquity = GetAccrual("ANT", IDEmployee, DateIni, DateFin);
                HorasConvenioArrastradasAñoAnterior = GetAccrual("SHC", IDEmployee, DateIni, DateFin);
                ValorInicialHorasConvenioAñoActual = GetInitialAccrual("SHC", IDEmployee, DateIniYear, DateFinYear);
                HorasConvenioAñoAnterior = HorasConvenioArrastradasAñoAnterior + ValorInicialHorasConvenioAñoActual;
                HorasConvenio = GetAccrual("CON", IDEmployee, DateIni, DateFin);
                HolidaysDays = Holidays + Antiquity + OldHolidays;

                // Inserta el registro
                sSQL = "@INSERT# INTO tmpVSL_AnnualAccruals (IDEmployee, idContract, [Year], IDConcept, Holidays, Antiquity, HolidaysDays, CurrentHolidays, OldHolidays, OldAntiquity, " +
                       "HorasConvenioArrastradasAñoAnterior, ValorInicialHorasConvenioAñoActual, HorasConvenioAñoAnterior, HorasConvenio, IdReportTask";

                for (i = 1; i <= 12; i++)
                {
                    sSQL += ", Month_" + i;
                }

                sSQL += ") VALUES (";
                sSQL += IDEmployee + ",'" + IdContract + "', " + DateFin.Year + "," + IdConcept + "," + Holidays.ToString().Replace(",", ".") + "," + Antiquity.ToString().Replace(",", ".");
                sSQL += "," + HolidaysDays.ToString().Replace(",", ".") + "," + CurrentHolidays.ToString().Replace(",", ".") + "," + OldHolidays.ToString().Replace(",", ".");
                sSQL += "," + OldAntiquity.ToString().Replace(",", ".") + "," + HorasConvenioArrastradasAñoAnterior.ToString().Replace(",", ".") + "," + ValorInicialHorasConvenioAñoActual.ToString().Replace(",", ".");
                sSQL += "," + HorasConvenioAñoAnterior.ToString().Replace(",", ".") + "," + HorasConvenio.ToString().Replace(",", ".");
                sSQL += "," + idReportTask.ToString();
                for (i = 0; i < 12; i++)
                {
                    sSQL += "," + AccrualsByMonth[i].ToString().Replace(",", ".");
                }
                sSQL += ")";                
                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);

                return true;
            }
            catch (Exception)
            {
//                LogDailyMessage(roError, "Calendar::VSL_CreateTmpAccrualsControlByEmployee_InsertRegistry: Error " + ex.Message);
                return false;
            }
        }



        private void CreateTmpBolsaHoras(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        string ShortName = "Teo";
                        double Value = 0;

                        string sSQLUpdate = "";
                        for (int x = 1; x <= 5; x++)
                        {
                            switch (x)
                            {
                                case 1:
                                    {
                                        ShortName = "PAS";
                                        break;
                                    }
                                case 2:
                                    {
                                        ShortName = "UPU";
                                        break;
                                    }
                                case 3:
                                    {
                                        ShortName = "DPU";
                                        break;
                                    }
                                case 4:
                                    {
                                        ShortName = "ARE";
                                        break;
                                    }
                                case 5:
                                    {
                                        ShortName = "Sho";
                                        break;
                                    }
                            }

                            sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                            sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                            sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                            sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                            sSQL = sSQL + " AND c.ShortName = '" + ShortName + "'";
                            sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                            Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                            Value = Math.Round(Value, 2);

                            if (x == 1)
                            {
                                sSQL = "@INSERT# INTO TMPAccrualsByEmployee (IDEmployee, FullGroupName, DNI,EmployeeName,Value1,IDReportTask) VALUES (";
                                sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByEmployee Set Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpRotacion(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        string ShortName = "Teo";
                        double Value = 0;

                        string sSQLUpdate = "";
                        for (int x = 1; x <= 5; x++)
                        {
                            switch (x)
                            {
                                case 1:
                                    {
                                        ShortName = "TMA";
                                        break;
                                    }
                                case 2:
                                    {
                                        ShortName = "TTA";
                                        break;
                                    }
                                case 3:
                                    {
                                        ShortName = "TNO";
                                        break;
                                    }
                                case 4:
                                    {
                                        ShortName = "TCO";
                                        break;
                                    }
                                case 5:
                                    {
                                        ShortName = "TTO";
                                        break;
                                    }
                            }

                            sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                            sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                            sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                            sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                            sSQL = sSQL + " AND c.ShortName = '" + ShortName + "'";
                            sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                            Value = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                            Value = Math.Round(Value, 2);

                            if (x == 1)
                            {
                                sSQL = "@INSERT# INTO TMPAccrualsByEmployee (IDEmployee, FullGroupName, DNI,EmployeeName,Value1,IDReportTask) VALUES (";
                                sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                sSQLUpdate = "@UPDATE# TMPAccrualsByEmployee Set Value" + x.ToString() + "= isnull(Value" + x.ToString() + ",0) + " + Value.ToString().Replace(",", ".") + " WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow["idemployee"]) + " AND IDReportTask = " + IDReportTask.ToString();
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQLUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpVM(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsDetailByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Obtenemos las diferentes fechas que contienen alguno de los saldos a mostrar en el informe
                        sSQL = "@SELECT# Distinct da.Date FROM DailyAccruals da";
                        sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                        sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                        sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                        sSQL = sSQL + " AND c.ShortName IN ('Med','IND','AVM')";
                        sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                        sSQL = sSQL + " AND da.Value <> 0 Order by da.Date";

                        DataTable sSubQuery;
                        sSubQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                        if (sSubQuery != null)
                        {
                            foreach (DataRow sQuerySubDataRow in sSubQuery.Rows)
                            {
                                double Value1 = 0;
                                double Value2 = 0;
                                double Value3 = 0;

                                // Para cada dia obtenemos los valores de cada saldo
                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'Med'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value1 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value1 = Math.Round(Value1, 2);

                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'IND'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value2 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value2 = Math.Round(Value2, 2);

                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'AVM'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value3 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value3 = Math.Round(Value3, 2);

                                if (Value1 != 0 || Value2 != 0 || Value3 != 0)
                                {
                                    // Insertamos los valores en la tabla temporal
                                    sSQL = "@INSERT# INTO TMPAccrualsDetailByEmployee (IDEmployee, Date, FullGroupName, DNI,EmployeeName,Value1, Value2, Value3,IDReportTask) VALUES (";
                                    sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + "," + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime() + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value1.ToString().Replace(",", ".") + "," + Value2.ToString().Replace(",", ".") + "," + Value3.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpRET(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;

            roCollection Result = new roCollection();

            try
            {
                //Borramos los registros anteriores
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsDetailByEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                sSQL = "@SELECT# sysrovwCurrentEmployeeGroups.idemployee, sysrovwCurrentEmployeeGroups.EmployeeName,  (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS NIF " +
                       " , (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee and FieldName = 'UNIDAD ORGANIZATIVA SAP' ORDER BY EmployeeUserFieldValues.Date DESC) AS UO  FROM dbo.sysrovwCurrentEmployeeGroups ";
                sSQL = sSQL + " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " GROUP BY sysrovwCurrentEmployeeGroups.idemployee,sysrovwCurrentEmployeeGroups.EmployeeName";

                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Obtenemos las diferentes fechas que contienen alguno de los saldos a mostrar en el informe
                        sSQL = "@SELECT# Distinct da.Date FROM DailyAccruals da";
                        sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                        sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                        sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                        sSQL = sSQL + " AND c.ShortName IN ('RET','PPA','EFA')";
                        sSQL = sSQL + " AND da.date BETWEEN " + roTypes.Any2Time(StartDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(EndDate).SQLSmallDateTime();
                        sSQL = sSQL + " AND da.Value <> 0 Order by da.Date";

                        DataTable sSubQuery;
                        sSubQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                        if (sSubQuery != null)
                        {
                            foreach (DataRow sQuerySubDataRow in sSubQuery.Rows)
                            {
                                double Value1 = 0;
                                double Value2 = 0;
                                double Value3 = 0;

                                // Para cada dia obtenemos los valores de cada saldo
                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'RET'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value1 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value1 = Math.Round(Value1, 2);

                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'PPA'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value2 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value2 = Math.Round(Value2, 2);

                                sSQL = "@SELECT# SUM(isnull(value,0)) FROM DailyAccruals da";
                                sSQL = sSQL + " INNER JOIN Employees e ON e.ID = da.IDEmployee";
                                sSQL = sSQL + " INNER JOIN Concepts c ON c.ID = da.IDConcept";
                                sSQL = sSQL + " WHERE e.ID = " + roTypes.Any2Double(sQueryDataRow["idemployee"]);
                                sSQL = sSQL + " AND c.ShortName = 'EFA'";
                                sSQL = sSQL + " AND da.date = " + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime();
                                Value3 = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));

                                Value3 = Math.Round(Value3, 2);

                                if (Value1 != 0 || Value2 != 0 || Value3 != 0)
                                {
                                    // Insertamos los valores en la tabla temporal
                                    sSQL = "@INSERT# INTO TMPAccrualsDetailByEmployee (IDEmployee, Date, FullGroupName, DNI,EmployeeName,Value1, Value2, Value3,IDReportTask) VALUES (";
                                    sSQL = sSQL + roTypes.Any2Double(sQueryDataRow["idemployee"]) + "," + roTypes.Any2Time(sQuerySubDataRow["Date"]).SQLSmallDateTime() + ",'" + roTypes.Any2String(sQueryDataRow["UO"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["NIF"]).Replace("'", "''") + "','" + roTypes.Any2String(sQueryDataRow["EmployeeName"]).Replace("'", "''") + "'," + Value1.ToString().Replace(",", ".") + "," + Value2.ToString().Replace(",", ".") + "," + Value3.ToString().Replace(",", ".") + "," + IDReportTask.ToString() + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpHolidaysControlByContractExv2: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private void CreateTmpPlannedHolidaysControlByContractEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            ////////////////////////////////////////////////////
            DateTime startDate = (DateTime)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value;
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string Employees = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);
            ////////////////////////////////////////////////////

            //Llena la tabla temporal con los datos de la consulta
            string sSQL = String.Empty;
            DataTable sQuery;
            DataTable sConcept;
            DateTime DateIniPeriode;
            DateTime DateFinPeriode;
            DateTime DateInifilter;
            DateTime DateFinFilter;

            double StartupValue;
            double CarryOver;
            double EnjoyedDays;
            double CurrentBalance;
            double HoursProvided;
            double DaysProvided;
            double EndingBalance;

            int StartDateMonth;
            int StartDateDay;
            DataTable bDS;

            object sXML;
            roCollection Result = new roCollection();
            double IdConcept;
            double IDCause;

            DataTable sProgrammedHolidays;

            try
            {
                //Recupero el mes de inicio de a�o
                StartDateMonth = roTypes.Any2Integer(GetValueFromSysroParameters("roParYearPeriod"));
                //Si no existe el par�metro roParYearPeriod, lo creo a 1 (enero)
                if (StartDateMonth == 0)
                {
                    StartDateMonth = 1;
                }

                // Recupero el dia de inicio de mes
                StartDateDay = roTypes.Any2Integer(GetValueFromSysroParameters("roParMonthPeriod"));

                if (StartDateDay == 0)
                {
                    StartDateDay = 1;
                }

                DateIniPeriode = new DateTime(startDate.Year, StartDateMonth, StartDateDay);
                DateFinPeriode = roTypes.Any2Time(DateIniPeriode).Add(1, "yyyy").Add(-1, "d").ValueDateTime;

                //Borramos los registros anteriores
                //MTH: No borramos datas para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPHOLIDAYSCONTROLByContract WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                // Obtenemos los empleados seleccionados, en los parametros nos llega la sentencia SQL
                // cerrada por parentesis, asi que, recortamos el ultimo parentesis de la consulta para poder anidar una clausula AND nueva.
                sSQL = "@SELECT# sysrovwAllEmployeeGroups.groupname, sysrovwAllEmployeeGroups.Fullgroupname, sysrovwAllEmployeeGroups.idemployee, sysrovwAllEmployeeGroups.idGroup, " +
                       "sysrovwAllEmployeeGroups.EmployeeName, EmployeeContracts.IDContract, EmployeeContracts.BeginDate AS BeginDateContract, " +
                       "dbo.EmployeeContracts.EndDate AS EndDateContract " +
                       "FROM dbo.EmployeeContracts INNER JOIN " +
                       "dbo.Employees ON dbo.EmployeeContracts.IDEmployee = dbo.Employees.ID INNER JOIN " +
                       "dbo.sysrovwAllEmployeeGroups ON dbo.Employees.ID = dbo.sysrovwAllEmployeeGroups.IDEmployee ";
                sSQL = sSQL + " where " + Employees.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups") + " and EmployeeContracts.BeginDate<=" + roTypes.Any2Time(DateFinPeriode).SQLSmallDateTime() + " and EmployeeContracts.EndDate>=" + roTypes.Any2Time(DateIniPeriode).SQLSmallDateTime();
                sQuery = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (sQuery != null)
                {
                    foreach (DataRow sQueryDataRow in sQuery.Rows)
                    {
                        // Determina fecha inicial y final de b�squeda
                        DateInifilter = (DateIniPeriode <= sQueryDataRow.Field<DateTime>("BeginDateContract") ? sQueryDataRow.Field<DateTime>("BeginDateContract") : DateIniPeriode);
                        DateFinFilter = (DateFinPeriode >= sQueryDataRow.Field<DateTime>("EndDateContract") ? sQueryDataRow.Field<DateTime>("EndDateContract") : DateFinPeriode);

                        // Para cada empleado obtenemos los saldos de vacaciones
                        sSQL = "@SELECT# ID, Name FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Causes WHERE IDConceptBalance > 0)";
                        sConcept = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (sConcept != null)
                        {
                            foreach (DataRow sConceptDataRow in sConcept.Rows)
                            {
                                // Valor inicial
                                StartupValue = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND StartupValue=1  AND CarryOver=1 AND IDConcept = " + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime()));

                                // Arrastre a�o anterior
                                CarryOver = 0;

                                // Obtenemos las reglas de convenio que utilicen este saldo y buscamos el valor de arrastre para la fecha
                                // inicial del a�o
                                //sSQL = "@SELECT# Definition FROM AccrualsRules "
                                sSQL = "@SELECT# Definition FROM AccrualsRules inner join LabAgreeAccrualsRules on AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules";
                                sSQL = sSQL + " where LabAgreeAccrualsRules.IDLabAgree in (@select# IDLabAgree from EmployeeContracts where BeginDate <=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and EndDate >=" + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " and IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + ")";
                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSDataRow in bDS.Rows)
                                    {
                                        sXML = roTypes.Any2String(bDSDataRow.Field<string>("Definition"));
                                        Result.LoadXMLString(sXML.ToString());
                                        IdConcept = roTypes.Any2Double(Result["MainAccrual"]);
                                        IDCause = roTypes.Any2Double(Result["DestiAccrual"]);
                                        //Err.Clear
                                        //On Error GoTo Catch

                                        if (IdConcept == roTypes.Any2Double(sConceptDataRow["ID"]))
                                        {
                                            //Buscamos el valor de arrastre
                                            sSQL = "@SELECT# sum(ISNULL(Value,0)) as total FROM DailyCauses WHERE IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee"));
                                            sSQL = sSQL + " AND Date = " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime();
                                            sSQL = sSQL + " AND IDCause = " + IDCause + " AND AccrualsRules=1";
                                            CarryOver = CarryOver + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                        }
                                        //DoEvents
                                        //if Not bDS.MoveNext { GoTo finally
                                    }
                                }

                                bDS.Dispose();

                                //                        // Horas disfrutadas
                                EnjoyedDays = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# COUNT(*) FROM DailySchedule INNER JOIN Employees ON DailySchedule.IDEmployee = Employees.[ID] INNER JOIN Shifts ON DailySchedule.IDShiftUsed = Shifts.[ID] WHERE DailySchedule.IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND Shifts.ID IN(@SELECT# ID FROM SHifTS WHERE ShiftType=2 and IDConceptBalance =" + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + ") " + "  AND DailySchedule.[Date] BETWEEN " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND " + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE <=" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                //                        // Saldo actual
                                CurrentBalance = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyAccruals WHERE IDEmployee=" + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND IDConcept = " + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + " AND Date >= " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND Date <=" + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE <=" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                //                        // Horas planificadas futuras
                                DaysProvided = roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# COUNT(*) FROM DailySchedule INNER JOIN Employees ON DailySchedule.IDEmployee = Employees.[ID] INNER JOIN Shifts ON DailySchedule.IDShift1= Shifts.[ID] WHERE DailySchedule.IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND Shifts.ID IN(@SELECT# ID FROM SHifTS WHERE ShiftType=2 and IDConceptBalance =" + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + ") " + "  AND DailySchedule.[Date] BETWEEN " + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND " + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime() + " and dATE >" + roTypes.Any2Time(startDate).SQLSmallDateTime()));

                                HoursProvided = 0;
                                sSQL = "@select# * FROM ProgrammedHolidays INNER JOIN Employees ";
                                sSQL = sSQL + "ON ProgrammedHolidays.IDEmployee = Employees.[ID] ";
                                sSQL = sSQL + "WHERE ProgrammedHolidays.IDEmployee = " + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + " AND ";
                                sSQL = sSQL + "ProgrammedHolidays.[Date] BETWEEN ";
                                sSQL = sSQL + roTypes.Any2Time(DateInifilter).SQLSmallDateTime() + " AND ";
                                sSQL = sSQL + roTypes.Any2Time(DateFinFilter).SQLSmallDateTime();
                                sSQL = sSQL + " AND DATE >" + roTypes.Any2Time(startDate).SQLSmallDateTime();
                                sSQL = sSQL + " AND IDCause IN(@select# ID FROM Causes WHERE IDConceptBalance =" + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + ")";

                                sProgrammedHolidays = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                                if (sProgrammedHolidays != null)
                                {
                                    foreach (DataRow sProgrammedHolidaysDataRow in sProgrammedHolidays.Rows)
                                    {
                                        if (roTypes.Any2Boolean(sProgrammedHolidaysDataRow.Field<bool>("AllDay")))
                                        {
                                            // Si es all day, obtenemos las horas teoricas del horario
                                            sSQL = "@select# (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" + roTypes.Any2Double(sProgrammedHolidaysDataRow.Field<int>("IDEmployee"));
                                            sSQL = sSQL + " AND S.ID = D.IDShift1 ";
                                            sSQL = sSQL + " AND D.Date = " + roTypes.Any2Time(sProgrammedHolidaysDataRow.Field<bool>("Date")).SQLSmallDateTime();
                                            HoursProvided = HoursProvided + roTypes.Any2Double(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                                        }
                                        else
                                        {
                                            // Obtenemos la duracion
                                            HoursProvided = HoursProvided + roTypes.Any2Double(sProgrammedHolidaysDataRow.Field<bool>("Duration"));
                                        }
                                    }
                                }
                                sProgrammedHolidays.Dispose();

                                //                        // Saldo restante previsto
                                EndingBalance = CurrentBalance - DaysProvided;

                                if (StartupValue != 0 || EnjoyedDays != 0 || CurrentBalance != 0 || DaysProvided != 0 || roTypes.Any2Boolean(CarryOver))
                                {
                                    sSQL = "@INSERT# INTO TMPHOLIDAYSCONTROLByContract (IDEmployee, IDConcept, StartupValue, CarryOver, EnjoyedDays, CurrentBalance, DaysProvided, EndingBalance, idContract, IDReportTask) VALUES (";
                                    sSQL = sSQL + roTypes.Any2Double(sQueryDataRow.Field<int>("IDEmployee")) + "," + roTypes.Any2Double(sConceptDataRow.Field<Int16>("ID")) + "," + StartupValue.ToString().Replace(",", ".") + "," + CarryOver.ToString().Replace(",", ".") + "," + EnjoyedDays.ToString().Replace(",", ".") + "," + CurrentBalance.ToString().Replace(",", ".") + "," + DaysProvided.ToString().Replace(",", ".") + "," + EndingBalance.ToString().Replace(",", ".") + ",'" + sQueryDataRow.Field<string>("idContract") + "'," + IDReportTask + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Calendar::CreateTmpPlannedHolidaysControlByContract: Error " & Err & ":" & Err.Description
            }
            finally
            {
            }
        }

        private bool CreateTmpIndicatorAnalysisEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            //ByVal InitialDate As Date, ByVal Groups As String, ByVal ShowDetails As Boolean, ByVal AllGroups As Boolean, ByVal IDReportTask As Integer
            ////////////////////////////////////////////////////
            DateTime initialDate = (DateTime)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value;
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);
            bool showDetails = Boolean.Parse(parametersList.Where(x => x.Type.Equals("System.Boolean") && x.Name.Equals("showDetails")).FirstOrDefault().Value.ToString());

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");

            bool allGroups = Boolean.Parse(parametersList.Where(x => x.Type.Equals("System.Boolean") && x.Name.Equals("allGroups")).FirstOrDefault().Value.ToString());
            ////////////////////////////////////////////////////

            string sSQL;
            DataTable ads;

            // Variables de la tabla temporal
            string MONTH_A = "";
            string MONTH_1 = "";
            string MONTH_2 = "";
            string Q1 = "";
            string Q2 = "";
            string Q3 = "";
            string Q4 = "";
            string Year = "";
            string Year_1 = "";

            string queryStartDate = "";
            string queryFinishDate = "";

            int needToCheck;

            try
            {
                if (groups.Contains("AND ( (not (sysrovwAllEmployeeGroups.CurrentEmployee=0)"))
                {
                    groups = groups.Substring(1, groups.IndexOf("AND ( (not (sysrovwAllEmployeeGroups.CurrentEmployee=0)") - 1) + ")";
                }

                groups = groups.Replace("sysrovwAllEmployeeGroups.IDEmployee", "Groups.ID");
                groups = groups.Replace("sysroEmployeeGroups.IDEmployee", "Groups.ID");
                groups = groups.Replace("sysroEmployeeGroups", "Groups");
                groups = groups.Replace("sysrovwAllEmployeeGroups", "Groups");
                groups = groups.Replace("IDGroup", "ID");

                //    'Eliminamos los posibles datos que puedan haber
                //    'MTH: No borramos datos para las impresiones multihilo
                sSQL = "@DELETE# FROM TMPIndicatorsAnalysis WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0";
                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);

                sSQL = "@DELETE# FROM TMPGroupIndicatorsAnalysis WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0";
                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);

                //    ' Obtenemos todos los indicadores definidos para los grupos seleccionados
                sSQL = "@SELECT# IDGroup, IDIndicator, IDFirstConcept, IDSecondConcept, LimitValue, DesiredValue, Condition, Groups.Name FROM GroupIndicators";
                sSQL = sSQL + " INNER JOIN Indicators ON GroupIndicators.IDIndicator = Indicators.ID";
                sSQL = sSQL + " INNER JOIN Groups ON GroupIndicators.IDGroup = Groups.ID";
                sSQL = sSQL + " Where GroupIndicators.IDGroup IN(@select# Distinct ID from Groups WHERE " + groups + ")";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        needToCheck = 1;
                        //Calculamos el valor del mes actual
                        queryStartDate = roTypes.Any2Time(initialDate).Add(1, "d").Substract(int.Parse(Strings.Format(roTypes.Any2Time(initialDate).DateOnly(), "dd")), "d").Value.ToString();
                        queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                        //'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Month_A:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                        CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (showDetails || allGroups), MONTH_A, IndicatorPeriodType.MONTH_A, IDReportTask);

                        //' Valores del mes -1
                        queryStartDate = roTypes.Any2Time(queryStartDate).Substract(1, "m").Value.ToString();
                        queryFinishDate = roTypes.Any2Time(queryStartDate).Add(1, "m").Substract(1, "s").Value.ToString();

                        //            'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Month_1:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                        CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), MONTH_1, IndicatorPeriodType.MONTH_1, IDReportTask);

                        //            ' Valores del mes -2
                        queryStartDate = roTypes.Any2Time(queryStartDate).Substract(1, "m").Value.ToString();
                        queryFinishDate = roTypes.Any2Time(queryStartDate).Add(1, "m").Substract(1, "s").Value.ToString();

                        //            'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Month_2:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                        CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), MONTH_2, IndicatorPeriodType.MONTH_2, IDReportTask);

                        int currentMonth;
                        currentMonth = int.Parse(Strings.Format(roTypes.Any2Time(initialDate).DateOnly(), "MM"));

                        //            'Valores del Q1
                        if (currentMonth > 3)
                        {
                            queryStartDate = roTypes.Any2Time(queryStartDate).Substract(int.Parse(Strings.Format(roTypes.Any2Time(queryStartDate).DateOnly(), "MM")) - 1, "m").Value.ToString();
                            queryFinishDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Substract(1, "s").Value.ToString();

                            //                'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q1:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                            CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q1, IndicatorPeriodType.Q1, IDReportTask);
                        }
                        else
                        {
                            queryStartDate = roTypes.Any2Time(queryStartDate).Substract(int.Parse(Strings.Format(roTypes.Any2Time(queryStartDate).DateOnly(), "MM")) - 1, "m").Value.ToString();
                            queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                            //                'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q1:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                            CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q1, IndicatorPeriodType.Q1, IDReportTask);
                            needToCheck = 0;
                        }

                        //            'Valores del Q2
                        if (currentMonth > 6)
                        {
                            queryStartDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Value.ToString();
                            queryFinishDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Substract(1, "s").Value.ToString();

                            //                'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q2:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                            CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q2, IndicatorPeriodType.Q2, IDReportTask);
                        }
                        else
                        {
                            if (needToCheck == 1)
                            {
                                queryStartDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Value.ToString();
                                queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                                //                    'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q2:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                                CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q2, IndicatorPeriodType.Q2, IDReportTask);
                                needToCheck = 0;
                            }
                            else
                            {
                                Q2 = "NULL";
                            }
                        }

                        //            'Valores del Q3
                        if (currentMonth > 9)
                        {
                            queryStartDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Value.ToString();
                            queryFinishDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Substract(1, "s").Value.ToString();

                            //                'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q3:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                            CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q3, IndicatorPeriodType.Q3, IDReportTask);
                        }
                        else
                        {
                            if (needToCheck == 1)
                            {
                                queryStartDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Value.ToString();
                                queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                                //                    'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q3:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                                CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q3, IndicatorPeriodType.Q3, IDReportTask);
                                needToCheck = 0;
                            }
                            else
                            {
                                Q3 = "Null";
                            }
                        }

                        //            'Valores del Q4
                        if (needToCheck == 1)
                        {
                            queryStartDate = roTypes.Any2Time(queryStartDate).Add(3, "m").Value.ToString();
                            queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                            //                'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Q4:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                            CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Q4, IndicatorPeriodType.Q4, IDReportTask);
                        }
                        else
                        {
                            Q4 = "Null";
                        }

                        //            'Valores del Year
                        queryStartDate = roTypes.Any2Time(initialDate).Substract(int.Parse(Strings.Format(roTypes.Any2Time(initialDate).DateOnly(), "MM")) - 1, "m").Substract(int.Parse(Strings.Format(roTypes.Any2Time(initialDate).DateOnly(), "dd")) - 1, "d").Value.ToString();
                        queryFinishDate = roTypes.Any2Time(initialDate).Add(1, "d").Value.ToString();

                        //            'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:Year_A:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate

                        CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Year, IndicatorPeriodType.YEAR_A, IDReportTask);

                        //            'Valores del Year - 1
                        queryStartDate = roTypes.Any2Time(queryStartDate).Substract(1, "yyyy").Value.ToString();
                        queryFinishDate = roTypes.Any2Time(queryStartDate).Add(1, "yyyy").Substract(1, "s").Value.ToString();

                        //            'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:SelectIndicatorGroups:YEAR_1:initialDate:" & queryStartDate & ", finishDate:" & queryFinishDate
                        CalculateIndicatorValue(adsDataRow, queryStartDate, queryFinishDate, (false || allGroups), Year_1, IndicatorPeriodType.Year_1, IDReportTask);

                        //            'Insertamos el registro resultante
                        sSQL = "@INSERT# INTO TmpIndicatorsAnalysis (IDGroup, IDIndicator, MONTH_A";
                        sSQL = sSQL + ", MONTH_1,MONTH_2, Q1, Q2, Q3, Q4, Year_A, Year_1, IDReportTask)";

                        sSQL = sSQL + " Values (" + adsDataRow["IDGroup"] + "," + adsDataRow["IDIndicator"] + ",";
                        sSQL = sSQL + MONTH_A + "," + MONTH_1 + "," + MONTH_2 + "," + Q1 + "," + Q2 + "," + Q3 + "," + Q4 + "," + Year + "," + Year_1 + "," + IDReportTask + ")";

                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }
                    ads.Dispose();
                    return true;
                }
            }
            catch (Exception)
            {
                //    LogDailyMessage roError, "Indicators::CreateTmpIndicatorAnalysis:Error:" & Err.Description
            }

            return false;
        }

        private void CalculateIndicatorValue(DataRow indicatorDS, string InitialDate, string EndDate, bool calculateGroupDetails, string IndicatorValue, IndicatorPeriodType periodType, int IDReportTask)
        {
            string sSQL;
            double initialConceptValue;
            double endConceptValue;
            double indicatorValTmp;

            try
            {
                initialConceptValue = 0;
                endConceptValue = 0;
                indicatorValTmp = 0;

                sSQL = "@SELECT# DailyAccruals.IDConcept, SUM(DailyAccruals.Value) AS AccrualValue";
                sSQL = sSQL + " From Concepts, DailyAccruals, sysroEmployeeGroups, EmployeeContracts";
                sSQL = sSQL + " Where Concepts.ID = DailyAccruals.IDConcept AND DailyAccruals.IDEmployee = sysroEmployeeGroups.IDEmployee";
                sSQL = sSQL + " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate";
                sSQL = sSQL + " AND DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND DailyAccruals.Date >= EmployeeContracts.BeginDate";
                sSQL = sSQL + " AND DailyAccruals.Date <= EmployeeContracts.EndDate AND (DailyAccruals.Date BETWEEN " + roTypes.Any2Time(InitialDate).SQLSmallDateTime() + " AND  " + roTypes.Any2Time(EndDate).SQLSmallDateTime() + ")";
                sSQL = sSQL + " AND (Concepts.ID IN (" + indicatorDS.Field<int>("IDFirstConcept") + "," + indicatorDS.Field<int>("IDSecondConcept") + ")) AND sysroEmployeeGroups.Path like( ";
                sSQL = sSQL + " (@SELECT# Groups.Path from Groups where Groups.ID = " + indicatorDS.Field<int>("IDGroup") + ") + '%' )";
                sSQL = sSQL + " GROUP BY DailyAccruals.IDConcept ";

                DataTable accrualDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (accrualDS != null)
                {
                    foreach (DataRow adsDataRow in accrualDS.Rows)
                    {
                        //            ' Acumulamos para la tabla indicatorAnalisys
                        if (roTypes.Any2Long(adsDataRow.Field<int>("IDConcept")) == roTypes.Any2Long(indicatorDS.Field<int>("IDFirstConcept")))
                        {
                            initialConceptValue = initialConceptValue + adsDataRow.Field<int>("AccrualValue");
                        }
                        else
                        {
                            endConceptValue = endConceptValue + adsDataRow.Field<int>("AccrualValue");
                        }
                    }

                    accrualDS.Dispose();

                    if (endConceptValue > 0)
                    {
                        indicatorValTmp = (initialConceptValue * 100) / endConceptValue;
                        IndicatorValue = roTypes.Any2Double(indicatorValTmp).ToString().Replace(",", ".");
                    }
                    else
                    {
                        IndicatorValue = "NULL";
                    }

                    if (calculateGroupDetails)
                    {
                        CalculateMonthGroupDetail(indicatorDS, InitialDate, EndDate, initialConceptValue, endConceptValue, periodType, IDReportTask);
                    }
                }
            }
            catch (Exception)
            {
                //    LogDailyMessage roError, "Indicators::CalculateIndicatorValue:Error:" & Err.Description
            }
        }

        private void CalculateMonthGroupDetail(DataRow indicatorDS, string InitialDate, string EndDate, double InitialIndicatorValue, double EndIndicatorValue, IndicatorPeriodType periodType, int IDReportTask)
        {
            string sSQL;
            string eSQL;
            string iSQL;
            string updateVar = "";
            string updatePercentage = "";
            DataTable accrualDS;
            DataTable accrualToDS1;
            DataTable accrualToDS;

            try
            {
                sSQL = "@SELECT# sysroEmployeeGroups.IDGroup, sysroEmployeeGroups.GroupName, sysroEmployeeGroups.Path, DailyAccruals.IDConcept, SUM(DailyAccruals.Value) AS AccrualValue";
                sSQL = sSQL + " From Concepts, DailyAccruals, sysroEmployeeGroups, EmployeeContracts";
                sSQL = sSQL + " Where Concepts.ID = DailyAccruals.IDConcept AND DailyAccruals.IDEmployee = sysroEmployeeGroups.IDEmployee";
                sSQL = sSQL + " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate";
                sSQL = sSQL + " AND DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND DailyAccruals.Date >= EmployeeContracts.BeginDate";
                sSQL = sSQL + " AND DailyAccruals.Date <= EmployeeContracts.EndDate AND (DailyAccruals.Date BETWEEN " + roTypes.Any2Time(InitialDate).SQLSmallDateTime() + " AND  " + roTypes.Any2Time(EndDate).SQLSmallDateTime() + ")";
                sSQL = sSQL + " AND (Concepts.ID IN (" + indicatorDS.Field<int>("IDSecondConcept") + ")) AND sysroEmployeeGroups.Path like( ";
                sSQL = sSQL + " (@SELECT# Groups.Path from Groups where Groups.ID = " + indicatorDS.Field<int>("IDGroup") + ") + '%' )";
                sSQL = sSQL + " GROUP BY sysroEmployeeGroups.Path, sysroEmployeeGroups.IDGroup,sysroEmployeeGroups.GroupName, DailyAccruals.IDConcept ";

                accrualDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (accrualDS != null)
                {
                    if (accrualDS.Rows.Count == 0)
                    {
                        eSQL = "@SELECT# SUM(DailyAccruals.Value) AS AccrualValue";
                        eSQL = eSQL + " From Concepts, DailyAccruals, sysroEmployeeGroups, EmployeeContracts";
                        eSQL = eSQL + " Where Concepts.ID = DailyAccruals.IDConcept AND DailyAccruals.IDEmployee = sysroEmployeeGroups.IDEmployee";
                        eSQL = eSQL + " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate";
                        eSQL = eSQL + " AND DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND DailyAccruals.Date >= EmployeeContracts.BeginDate";
                        eSQL = eSQL + " AND DailyAccruals.Date <= EmployeeContracts.EndDate AND (DailyAccruals.Date BETWEEN " + roTypes.Any2Time(InitialDate).SQLSmallDateTime() + " AND  " + roTypes.Any2Time(EndDate).SQLSmallDateTime() + ")";
                        eSQL = eSQL + " AND Concepts.ID = " + indicatorDS.Field<int>("IDFirstConcept") + " AND sysroEmployeeGroups.IDGroup =" + "0";

                        double endValue1 = 0;

                        accrualToDS1 = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (accrualToDS1 != null)
                        {
                            foreach (DataRow accrualToDS1DataRow in accrualToDS1.Rows)
                            {
                                endValue1 = endValue1 + roTypes.Any2Double(accrualToDS1DataRow.Field<double>("AccrualValue"));
                            }
                        }

                        accrualToDS1.Dispose();

                        string updateVal;
                        string updateValGroup;
                        updateVal = "NULL";
                        updateValGroup = "NULL";

                        if (endValue1 > 0)
                        {
                            updateVal = "0";
                            updateValGroup = "0";
                        }

                        if (periodType == IndicatorPeriodType.MONTH_A)
                        {
                            //                ' Insertamos el registro para la tabla groupindicatoranalisys
                            iSQL = "@INSERT# INTO TmpGroupIndicatorsAnalysis (IDGroup, IDIndicator, GroupPath, IDGroupParent, GroupValue, GroupPercentage, IDReportTask)";
                            iSQL = iSQL + " Values (" + roTypes.Any2Long("0") + "," + roTypes.Any2Long(indicatorDS.Field<int>("IDIndicator")) + ",'" + roTypes.Any2String("") + "'," + roTypes.Any2Long(indicatorDS.Field<int>("IDGroup"));
                            iSQL = iSQL + "," + updateVal + "," + updateValGroup + "," + IDReportTask + ")";
                            //               'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:InsertGroupSQL:" & iSQL
                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                        else
                        {
                            switch (periodType)
                            {
                                case TemporalTablesManager.IndicatorPeriodType.MONTH_A:
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.MONTH_1:
                                    updateVar = "MONTH_1";
                                    updatePercentage = "MONTH_1_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.MONTH_2:
                                    updateVar = "MONTH_2";
                                    updatePercentage = "MONTH_2_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.Q1:
                                    updateVar = "Q1";
                                    updatePercentage = "Q1_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.Q2:
                                    updateVar = "Q2";
                                    updatePercentage = "Q2_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.Q3:
                                    updateVar = "Q3";
                                    updatePercentage = "Q3_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.Q4:
                                    updateVar = "Q4";
                                    updatePercentage = "Q4_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.YEAR_A:
                                    updateVar = "YEAR_A";
                                    updatePercentage = "YEAR_A_Percentage";
                                    break;

                                case TemporalTablesManager.IndicatorPeriodType.Year_1:
                                    updateVar = "YEAR_1";
                                    updatePercentage = "YEAR_1_Percentage";
                                    break;

                                default:
                                    break;
                            }

                            iSQL = "@Update# TMPGroupIndicatorsAnalysis SET " + updateVar + " = " + updateVal;
                            iSQL = iSQL + " ," + updatePercentage + " = " + updateValGroup;
                            iSQL = iSQL + " WHERE IDGroup = " + roTypes.Any2Long("0") + " AND IDIndicator = " + roTypes.Any2Long(indicatorDS.Field<int>("IDIndicator")) + " AND IDGroupParent = " + roTypes.Any2Long(indicatorDS.Field<int>("IDGroup"));
                            //'LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:UpdateGroupSQL:" & iSQL
                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                    }
                    else
                    {
                        foreach (DataRow accrualDSDataRow in accrualDS.Rows)
                        {
                            eSQL = "@SELECT# SUM(DailyAccruals.Value) AS AccrualValue";
                            eSQL = eSQL + " From Concepts, DailyAccruals, sysroEmployeeGroups, EmployeeContracts";
                            eSQL = eSQL + " Where Concepts.ID = DailyAccruals.IDConcept AND DailyAccruals.IDEmployee = sysroEmployeeGroups.IDEmployee";
                            eSQL = eSQL + " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate";
                            eSQL = eSQL + " AND DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND DailyAccruals.Date >= EmployeeContracts.BeginDate";
                            eSQL = eSQL + " AND DailyAccruals.Date <= EmployeeContracts.EndDate AND (DailyAccruals.Date BETWEEN " + roTypes.Any2Time(InitialDate).SQLSmallDateTime() + " AND  " + roTypes.Any2Time(EndDate).SQLSmallDateTime() + ")";
                            eSQL = eSQL + " AND Concepts.ID = " + indicatorDS.Field<int>("IDFirstConcept") + " AND sysroEmployeeGroups.IDGroup =" + roTypes.Any2Long(accrualDSDataRow.Field<int>("IDGroup"));

                            double endValue;

                            accrualToDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                            endValue = 0;

                            if (accrualToDS != null)
                            {
                                foreach (DataRow accrualToDSDataRow in accrualToDS.Rows)
                                {
                                    endValue = endValue + roTypes.Any2Double(accrualToDSDataRow.Field<double>("AccrualValue"));
                                }
                            }

                            accrualToDS.Dispose();

                            double groupValue;
                            double groupPercentage;

                            if (roTypes.Any2Double(accrualDSDataRow.Field<double>("AccrualValue")) == 0)
                            {
                                groupValue = 0;
                                groupPercentage = 0;
                            }
                            else
                            {
                                groupValue = (endValue * 100) / accrualDSDataRow.Field<double>("AccrualValue");

                                if (InitialIndicatorValue > 0)
                                {
                                    groupPercentage = (endValue * 100) / InitialIndicatorValue;
                                }
                                else
                                {
                                    groupPercentage = 0;
                                }
                            }

                            if (periodType == IndicatorPeriodType.MONTH_A)
                            {
                                //' Insertamos el registro para la tabla groupindicatoranalisys
                                iSQL = "@INSERT# INTO TmpGroupIndicatorsAnalysis (IDGroup, IDIndicator, GroupPath, IDGroupParent, GroupValue, GroupPercentage, IDReportTask)";
                                iSQL = iSQL + " Values (" + roTypes.Any2Long(accrualDSDataRow.Field<int>("IDGroup")) + "," + roTypes.Any2Long(indicatorDS.Field<int>("IDIndicator")) + ",'" + roTypes.Any2String(accrualDSDataRow.Field<string>("Path")) + "'," + roTypes.Any2Long(indicatorDS.Field<int>("IDGroup"));
                                iSQL = iSQL + "," + roTypes.Any2Double(groupValue).ToString().Replace(",", ".") + "," + roTypes.Any2Double(groupPercentage).ToString().Replace(",", ".") + "," + IDReportTask + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                switch (periodType)
                                {
                                    case TemporalTablesManager.IndicatorPeriodType.MONTH_A:
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.MONTH_1:
                                        updateVar = "MONTH_1";
                                        updatePercentage = "MONTH_1_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.MONTH_2:
                                        updateVar = "MONTH_2";
                                        updatePercentage = "MONTH_2_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.Q1:
                                        updateVar = "Q1";
                                        updatePercentage = "Q1_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.Q2:
                                        updateVar = "Q2";
                                        updatePercentage = "Q2_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.Q3:
                                        updateVar = "Q3";
                                        updatePercentage = "Q3_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.Q4:
                                        updateVar = "Q4";
                                        updatePercentage = "Q4_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.YEAR_A:
                                        updateVar = "YEAR_A";
                                        updatePercentage = "YEAR_A_Percentage";
                                        break;

                                    case TemporalTablesManager.IndicatorPeriodType.Year_1:
                                        updateVar = "YEAR_1";
                                        updatePercentage = "YEAR_1_Percentage";
                                        break;

                                    default:
                                        break;
                                }

                                iSQL = "@Update# TMPGroupIndicatorsAnalysis SET " + updateVar + " = " + roTypes.Any2Double(groupValue).ToString().Replace(",", ".");
                                iSQL = iSQL + " ," + updatePercentage + " = " + roTypes.Any2Double(groupPercentage).ToString().Replace(",", ".");
                                iSQL = iSQL + " WHERE IDGroup = " + roTypes.Any2Long(accrualDSDataRow.Field<int>("IDGroup")) + " AND IDIndicator = " + roTypes.Any2Long(indicatorDS.Field<int>("IDIndicator")) + " AND IDGroupParent = " + roTypes.Any2Long(indicatorDS.Field<int>("IDGroup"));
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                        }
                    }
                }

                accrualDS.Dispose();
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Indicators::CalculateMonthGroupDetail:Error:" & Err.Description
            }
            finally
            {
            }
        }

        public bool CreateTmpEmergencyWithAttendanceByZonesEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            //ByVal Zones As String, ByVal IDReportTask As Integer
            string zones = "";

            bool bolRet;
            int IDEmployee;
            string sSQL;
            DataTable ads;
            string fld;
            long MaxTime;
            string UsrField;
            string PE;

            try
            {
                //'Recogemos el campo que contiene el nombre del campo de la ficha para agrupar
                UsrField = GetValueFromSysroParameters("roParEvacuationPointUsrField");

                //'Obtener el campo que contiene el nombre del campo de horas de comprobacion
                //'Dim MaxTime As String = roTypes.Any2Time(oParams.Parameter(roParameters.Parameters.EmergencyMaxHours)).Minutes <---en live no existe este parametro falta por agregar
                MaxTime = 2880;

                //'**** SELECCIONA LOS PUNTOS DE ENCUENTRO de las zonas seleccionadas ****
                PE = GetEvacuationPoints(zones);

                //'crear datos en la tabla
                //'MTH: No borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyTotals WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyVisits WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //'**** CREA EL PUNTO DE EVACUACION EN BLANCO A TODOS AQUELLOS EMPLEADOS QUE NO LO TENGAN ****'
                sSQL = "@SELECT# id from employees where id not in (@SELECT# idemployee from EmployeeUserFieldValues where FieldName='" + UsrField + "')";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# into EmployeeUserFieldValues (idEmployee, Date, FieldName, Value) values (" + adsDataRow["id"] + ", convert(smalldatetime,'1900/01/01',120), '" + UsrField + "','')");
                    }
                    ads.Dispose();
                }

                //'**** EMPLEADOS QUE ESTAN EN LAS ZONAS SELECCIONADAS CON CONTRATO*********
                sSQL = "@SELECT# * FROM sysrovwCurrentEmployeesZoneStatus " +
                    "WHERE IdZone in (" + zones + ") " +
                    " and   (IDEmployee IN (@SELECT# TOP (1) IDEmployee " +
                                           "From dbo.EmployeeContracts " +
                                           "WHERE (dbo.sysrovwCurrentEmployeesZoneStatus.IDEmployee = IDEmployee) AND (GETDATE() BETWEEN BeginDate AND EndDate))) ";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        IDEmployee = int.Parse(adsDataRow["IDEmployee"].ToString());
                        fld = GetFieldValue(DateTime.Now, IDEmployee, UsrField);

                        if ((DateTime.Now - (DateTime)adsDataRow["datetime"]).TotalMinutes <= MaxTime)
                        {
                            //' ESTA EN LAS ZONAS DESDE HACE MENOS DE 48 HORAS
                            sSQL = "@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[State],[IDReportTask]) VALUES (" + IDEmployee + ",'" + fld + "', '" + StatusZoneEnum.IN_Zone_LessThan48H + "'," + IDReportTask + ")";
                        }
                        else
                        {
                            //' ESTA EN LAS ZONAS DESDE HACE MAS DE 48 HORAS
                            sSQL = "@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[State],[IDReportTask]) VALUES (" + IDEmployee + ",'" + fld + "', '" + StatusZoneEnum.IN_Zone_MoreThan48H + "'," + IDReportTask + ")";
                        }

                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }

                    ads.Dispose();
                }

                //'**** EMPLEADOS AUSENTES ************
                //' Se muestran todos los empleados de los puntos de encuentro en el que hay gente presente aunque no son de las zonas seleccionadas
                //'sSql = "@SELECT# DISTINCT Employees.ID,dbo.sysrovwCurrentEmployeesPresenceStatusPunches.Status, dbo.sysrovwCurrentEmployeesPresenceStatusPunches.DateTime " +
                //'    "FROM Employees INNER JOIN sysrovwCurrentEmployeesPresenceStatusPunches ON sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee = Employees.ID INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " +
                //'    "WHERE (Employees.ID not in (@SELECT# idEmployee from TMPEmergency) " +
                //'    " and ((Employees.ID in (@SELECT# idemployee FROM EmployeeUserFieldValues WHERE FIELDNAME='" & UsrField & "' AND SUBSTRING(VALUE,1,50) in (" & PE & "))) " +
                //'    " OR Employees.ID in (@SELECT# idemployee FROM EmployeeUserFieldValues WHERE FIELDNAME='" & UsrField & "' AND SUBSTRING(VALUE,1,50) in (@SELECT# DISTINCT USERFIELDVALUE FROM TMPEmergency)))) " +
                //'    " and ((GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) OR " +
                //'    "(sysrovwCurrentEmployeesPresenceStatusPunches.DateTime > (@SELECT# TOP (1) DateTime FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE (IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) AND (Status = 'IN') " +
                //'    "ORDER BY DateTime DESC)) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') OR " +
                //'    "(GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') AND " +
                //'    "(sysrovwCurrentEmployeesPresenceStatusPunches.Status = (@SELECT# TOP (1) Status FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE(IdEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) " +
                //'    "ORDER BY DateTime DESC)) OR (sysrovwCurrentEmployeesPresenceStatusPunches.DateTime IS NULL) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate)) "

                //' Se muestran solo los empleados de los puntos de encuentro de las zonas seleccionadas
                sSQL = "@DECLARE# @Date smalldatetime SET @Date =getdate() ";
                sSQL = sSQL + "@SELECT# DISTINCT Employees.ID,dbo.sysrovwCurrentEmployeesPresenceStatusPunches.Status, dbo.sysrovwCurrentEmployeesPresenceStatusPunches.DateTime " +
                    "FROM Employees INNER JOIN sysrovwCurrentEmployeesPresenceStatusPunches ON sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee = Employees.ID INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " +
                    "WHERE (Employees.ID not in (@SELECT# idEmployee from TMPEmergency) " +
                    " and ((Employees.ID in (@SELECT# idemployee FROM GetAllEmployeeAllUserFieldValue(@Date) WHERE FieldName='" + UsrField + "' AND Value in (" + PE + "))) " +
                    " )) " +
                    " and ((GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) OR " +
                    "(sysrovwCurrentEmployeesPresenceStatusPunches.DateTime > (@SELECT# TOP (1) DateTime FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE (IDEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) AND (Status = 'IN') " +
                    "ORDER BY DateTime DESC, id Desc)) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') OR " +
                    "(GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate) AND (sysrovwCurrentEmployeesPresenceStatusPunches.Status = 'OUT') AND " +
                    "(sysrovwCurrentEmployeesPresenceStatusPunches.Status = (@SELECT# TOP (1) Status FROM sysrovwCurrentEmployeesPresenceStatusPunches AS srceps WHERE(IdEmployee = sysrovwCurrentEmployeesPresenceStatusPunches.IDEmployee) " +
                    "ORDER BY DateTime DESC, id Desc)) OR (sysrovwCurrentEmployeesPresenceStatusPunches.DateTime IS NULL) AND (GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate)) ";

                StatusZoneEnum EmployeeStatus;
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        EmployeeStatus = StatusZoneEnum.With_Error;
                        IDEmployee = int.Parse(adsDataRow["ID"].ToString());
                        fld = GetFieldValue(DateTime.Now, IDEmployee, UsrField);

                        if (adsDataRow["Status"].Equals("IN"))
                        {
                            if ((DateTime.Now - (DateTime)adsDataRow["datetime"]).TotalMinutes <= MaxTime)
                            {
                                //' ESTA EN OTRAS ZONAS DESDE HACE MENOS DE 48 HORAS
                                EmployeeStatus = StatusZoneEnum.IN_OtherZone_LessThan48H;
                            }
                            else
                            {
                                //' ESTA EN OTRAS ZONAS DESDE HACE MAS DE 48 HORAS
                                //'EmployeeStatus = IN_OtherZone_MoreThan48H
                                EmployeeStatus = StatusZoneEnum.IN_OtherZone_LessThan48H; //' de momento, en el informe no se diferencian los que est�n en otra zona y llevan m�s o menos de 48 horas
                            }
                        }
                        else
                        {
                            if (EmployeeShouldComeIn(IDEmployee))
                            {
                                //'**** DEBERIA HABER VENIDO ****
                                EmployeeStatus = StatusZoneEnum.Employee_MustBe;
                            }
                            else
                            {
                                //'**** AUSENTES ****
                                EmployeeStatus = StatusZoneEnum.Employee_Absent;
                            }
                        }

                        //' Inserta el registro
                        Robotics.DataLayer.AccessHelper.CreateDataTable("@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[State],[IDReportTask]) VALUES (" + IDEmployee + ",'" + fld + "', '" + EmployeeStatus + "'," + IDReportTask + ")");
                    }

                    ads.Dispose();
                }

                //' Se muestran solo los empleados de los puntos de encuentro de las zonas seleccionadas
                sSQL = "@DECLARE# @Date smalldatetime SET @Date =getdate() ";
                sSQL = sSQL + " @SELECT#  GEUF.idemployee, Value " +
                              " FROM dbo.EmployeeContracts  inner join GetAllEmployeeAllUserFieldValue(@Date) GEUF on EmployeeContracts.idemployee=GEUF.idemployee " +
                              " where FieldName = '" + UsrField + "' and Value in (" + PE + ") " +
                              " AND (GETDATE() BETWEEN dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate) " +
                              " AND GEUF.IDEMPLOYEE not in (@SELECT# idemployee from TMPEmergency ) ";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //' Comprueba si tiene movimientos
                        sSQL = "@SELECT# idemployee FROM sysrovwCurrentEmployeesPresenceStatusPunches WHERE IDEmployee = " + adsDataRow["idEmployee"];
                        if (Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL).ToString() == "0")
                        {
                            //' Inserta el registro como ausente
                            sSQL = "@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[State],[IDReportTask]) VALUES (" + adsDataRow["IDEmployee"] + ",'" + adsDataRow["Value"] + "', '" + StatusZoneEnum.Employee_Absent + "'," + IDReportTask + ")";
                        }
                        else
                        {
                            //' Inserta el registro como error
                            sSQL = "@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[State],[IDReportTask]) VALUES (" + adsDataRow["IDEmployee"] + ",'" + adsDataRow["Value"] + "', '" + StatusZoneEnum.With_Error + "'," + IDReportTask + ")";
                        }

                        Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                    }

                    ads.Dispose();
                }

                //' Visitas presentes con empleados de los puntos asignados
                sSQL = "@declare# @date smalldatetime SET @Date =getdate() ";

                sSQL = sSQL + " @insert# into TMPEmergencyVisitsZones @select# DISTINCT 0, v.BeginDate, vi.IDVisitor, v.IDEmployee, EUFV.value," + IDReportTask + " " +
                      "from [dbo].[Visit] v " +
                      "    inner join [dbo].[Visit_Visitor] vv on v.IDVisit = vv.IDVisit " +
                      "    inner join [dbo].[Visitor] vi on vv.IDVisitor = vi.IDVisitor " +
                      "    inner join GetAllEmployeeUserFieldValue('" + UsrField + "',@Date) EUFV on EUFV.idEmployee =v.IDEmployee " +
                      "Where BeginDate >= DateAdd(n, -2880, GETDATE()) and v.Status = 1 ";

                //'sSQL = sSQL & " @INSERT# into TMPEmergencyVisits @SELECT# visitplanid,begintime,vp.VisitorId, vp.EmpVisitedId,EUFV.value," & IDReportTask & " " +
                //'      "from visitmoves vm " +
                //'      "    inner join visitplan vp on vm.VisitPlanId =vp.ID " +
                //'      "    inner join visitors v on v.ID=vp.VisitorId " +
                //'      "    inner join GetAllEmployeeUserFieldValue('" & UsrField & "',@Date) EUFV on EUFV.idEmployee =vp.EmpVisitedId " +
                //'      "Where EndTime Is Null AND Value in (" & PE & ") and BeginTime >= DateAdd(n, -2880, GETDATE()) "
                Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                //' Calcula totales del informe de emergencia
                int Presents;
                int Visits;
                int LastEntry24;
                int CouldBePresent;
                int Absents;
                int inOtherZones;

                sSQL = "@select# distinct(userfieldvalue) as MP from TMPEmergency as MeettingPoint ";
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //' Lee totales por punto de encuentro
                        Presents = GetEmployeesNumberByLocationEx(adsDataRow["MP"].ToString(), "0", IDReportTask);
                        Visits = GetVisitsCountInEmergencyZonesEx(adsDataRow["MP"].ToString(), IDReportTask);
                        LastEntry24 = GetEmployeesNumberByLocationEx(adsDataRow["MP"].ToString(), "1", IDReportTask);
                        CouldBePresent = GetEmployeesNumberByLocationEx(adsDataRow["MP"].ToString(), "2", IDReportTask);
                        Absents = GetEmployeesNumberByLocationEx(adsDataRow["MP"].ToString(), "3", IDReportTask);
                        inOtherZones = GetEmployeesNumberByLocationEx(adsDataRow["MP"].ToString(), "4", IDReportTask);

                        //' Inserta el registro
                        sSQL = "@INSERT# INTO TMPEmergencyTotals (MeetingPoint,PresentEmployees,PresentVisits,LastEntry24,CouldBePresents,Absents,InOtherZones,IDReportTask) " +
                               "  values ('" + adsDataRow["MP"] + "'," + Presents + "," + Visits + "," + LastEntry24 + "," + CouldBePresent + "," + Absents + "," + inOtherZones + "," + IDReportTask + ")";

                        Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                    }

                    ads.Dispose();
                }

                bolRet = true;
            }
            catch (Exception)
            {
                bolRet = false;
            }
            finally
            {
            }

            return bolRet;
        }

        // Realizado por Victor Mañas 2020/09/22
        private string GetFieldName(DateTime mDate, long IDEmployee, string mFieldID)
        {
            string result = "";
            //'
            //' Obtenemos el Nombre de un campo de la ficha del empleado pasando el ID de dicho campo.
            //'
            string cSQL;
            DataTable ads;
            try
            {
                cSQL = "@SELECT#  FieldName from sysroUserFields where ID=" + mFieldID;
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(cSQL);
                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        result = roTypes.Any2String(adsDataRow["FieldName"]);
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        private string GetFieldValue(DateTime mDate, long IDEmployee, string mFieldName)
        {
            string result = "";
            //'
            //' Obtenemos el valor de un campo en una fecha concreta
            //'
            string cSQL;
            DataTable ads;
            double mType = -1;

            try
            {
                result = "";

                cSQL = "@SELECT# (@SELECT# TOP 1 [Value]" +
                        "From EmployeeUserFieldValues " +
                        "WHERE EmployeeUserFieldValues.IDEmployee = " + IDEmployee + " AND " +
                              " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND " +
                              " EmployeeUserFieldValues.Date <= " + roTypes.Any2Time(mDate).SQLSmallDateTime() +
                        "ORDER BY EmployeeUserFieldValues.Date DESC) as Value , " +
                        " ISNULL((@SELECT# TOP 1 [Date] " +
                               " From EmployeeUserFieldValues " +
                               " WHERE EmployeeUserFieldValues.IDEmployee = " + IDEmployee + " AND " +
                                     " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND " +
                                     " EmployeeUserFieldValues.Date <= " + roTypes.Any2Time(mDate).SQLSmallDateTime() +
                               " ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120)) as Date, FieldType" +
                                "    From sysroUserFields " +
                                    "WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND " +
                                        " ((sysroUserFields.FieldName = '" + mFieldName.Replace("'", "''") + "') OR (Alias = '" + mFieldName.Replace("'", "''") + "'))";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(cSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        result = roTypes.Any2String(adsDataRow["Value"]);
                        mType = roTypes.Any2Double(adsDataRow["FieldType"]);
                    }
                }

                switch (mType)
                {
                    case 1: //numérico
                    case 3: //decimal
                    case 4: //time
                        System.Globalization.NumberFormatInfo oInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                        result = result.Replace(".", oInfo.CurrencyDecimalSeparator);
                        break;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        private bool EmployeeShouldComeIn(int IDEmployee)
        {
            DataTable tbEmployee;
            bool ShouldComeIn = false;

            try
            {
                string sSQL;

                //' Selecciona si el empleado deber�a haber venido
                sSQL = "@select# isPresent from EmployeeStatus where idEmployee=" + IDEmployee + " and BeginMandatory<=GetDate()";
                tbEmployee = new DataTable();
                tbEmployee = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (tbEmployee != null)
                {
                    ShouldComeIn = !roTypes.Any2Boolean(tbEmployee.Rows[0]["isPresent"]);
                    tbEmployee.Dispose();
                }
            }
            catch (Exception)
            {
                //LogDailyMessage roError, "Visits::EmployeeShouldComeIn:Error:" & Err.Description
            }
            finally
            {
            }

            return ShouldComeIn;
        }

        private int GetEmployeesNumberByLocationEx(string location, string State, int IDReportTask)
        {
            string sSQL;

            try
            {
                sSQL = "@SELECT# count(*) from TMPEmergency where UserFieldValue='" + location + "' and state='" + State + "' AND IDReportTask=" + IDReportTask;
                return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
            }
            catch
            {
                //LogDailyMessage roError, "Visits::GetEmployeesNumberByLocationEx:Error:" & Err.Description
            }

            return 0;
        }

        private int GetVisitsCountInEmergencyZonesEx(string location, int IDReportTask)
        {
            string sSQL;

            try
            {
                sSQL = "@select# count(*) from TMPEmergencyVisitsZones " +
                       "where MeettingPoint='" + location + "' AND IDReportTask=" + IDReportTask;
                return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
            }
            catch
            {
                //LogDailyMessage roError, "Visits::GetVisitsCountInEmergencyZonesEx:Error:" & Err.Description
            }

            return 0;
        }

        private string GetEvacuationPoints(string Zones)
        {
            string sSQL;
            DataTable ads;
            string PE = "";
            string[] PEs;
            string aux;

            try
            {
                sSQL = "@SELECT# Name, description FROM Zones " +
                    "WHERE Id in (" + Zones + ") and Description like '%#PE=%'";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    PE = "''";
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        aux = adsDataRow["Description"].ToString().Split(new String[] { "#PE=" }, StringSplitOptions.None)[1];

                        //' busca el final #
                        aux = aux.Split(new string[] { "#" }, StringSplitOptions.None)[0];

                        //' Descompone las zonas
                        PEs = aux.Split(new string[] { ";" }, StringSplitOptions.None);

                        for (int i = 0; i <= Microsoft.VisualBasic.Information.UBound(PEs); i++)
                        {
                            if (PE != "")
                                PE = PE + ",";
                            PE = PE + "'" + PEs[i] + "'";
                        }
                    }

                    ads.Dispose();
                }
            }
            catch (Exception)
            {
                PE = "";
            }
            finally
            {
            }

            return PE;
        }

        //Se usa!
        private bool CreateTmpEmergencyWithAttendanceByZonesBasicEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            String Zones = roTypes.Any2String(originalParametersList.Where(x => x.Type.Equals("Robotics.Base.zonesSelector")).FirstOrDefault().Value);

            bool bolRet = false;
            int IDEmployee;
            int IDZone;
            int ZoneIsNotReliable;
            int IsGeolocated;
            string sSQL;
            DataTable ads;
            string fld;
            string UsrField;
            string TerminalDescription;
            string Name;
            string VisitorName;
            DateTime Datetime;

            bool bInsert = true;

            try
            {
                //Borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergencyBasic WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //Recogemos el campo que contiene el nombre del campo de la ficha de punto de evacuación para agrupar por el ...
                UsrField = GetValueFromSysroParameters("EvacuationPointUsrField");

                //**** CREA EL PUNTO DE EVACUACION EN BLANCO A TODOS AQUELLOS EMPLEADOS QUE NO LO TENGAN ****'
                sSQL = "@SELECT# id from employees where id not in (@select# idemployee from EmployeeUserFieldValues where FieldName='" + UsrField + "')";
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# into EmployeeUserFieldValues (idEmployee, Date, FieldName, Value) values (" + adsDataRow["id"] + ", convert(smalldatetime,'1900/01/01',120), '" + UsrField + "','')");
                    }

                    ads.Dispose();
                }

                //**** EMPLEADOS CUYO ÚLTIMO FICHAJE, DE CUALQUIER TIPO MENOS ACCESO INVÁLIDO, SEA A UNA DE LAS ZONAS SELECCIONADAS EN EL PERFIL DEL INFORME, Y QUE HAGA MENOS DE 48 HORAS*********
                sSQL = "@SELECT# s.*, t.Description as TerminalDescription, t.Type AS TerminalType, tr.IDZone AS IDZoneIn, tr.IDZoneOut  " +
                       "FROM sysrovwCurrentEmployeesZoneStatus s " +
                       "LEFT JOIN Terminals t ON s.IDReader = t.ID " +
                       "LEFT JOIN TerminalReaders tr ON tr.IDTerminal = t.Id AND tr.ID = 1 " +
                       "WHERE s.IdZone IN (" + Zones + ") " +
                       "AND DateTime >= DATEADD(dd,-2,getdate())";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        IDEmployee = roTypes.Any2Integer(adsDataRow["IDEmployee"]);
                        IDZone = roTypes.Any2Integer(adsDataRow["IDZone"]);
                        ZoneIsNotReliable = (adsDataRow["ZoneIsNotReliable"].Equals(true)) ? 1 : 0;
                        IsGeolocated = (roTypes.Any2String(adsDataRow["Location"]).Length > 0) ? 1 : 0;
                        fld = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, UsrField)).Replace("'", "''").Replace("--", "_");
                        Name = roTypes.Any2String(adsDataRow.Field<string>("EmployeeName")).Replace("'", "''").Replace("--", "_");
                        Datetime = adsDataRow.Field<DateTime>("Datetime");
                        if (adsDataRow["TerminalDescription"] != null)

                        {
                            TerminalDescription = adsDataRow["TerminalDescription"].ToString();
                        }
                        else
                        {
                            TerminalDescription = "";
                        }

                        string ValorDefecto = "";
                        string Valor1 = "";
                        string Valor2 = "";
                        string Valor3 = "";
                        string Valor4 = "";

                        foreach (ReportParameter PL in parametersList)
                        {
                            if (PL.Description == "Campo1")
                            {
                                try
                                {
                                    String[] fields = roTypes.Any2String(originalParametersList.Where(x => x.Type.Equals("Robotics.Base.userFieldsSelector")).FirstOrDefault().Value).Split(',');

                                    if (fields.Length > 0)
                                    {
                                        Valor1 = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, GetFieldName(DateTime.Now, IDEmployee, fields[0]))).Replace("'", "''").Replace("--", "_");
                                    }
                                    if (fields.Length > 1)
                                    {
                                        Valor2 = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, GetFieldName(DateTime.Now, IDEmployee, fields[1]))).Replace("'", "''").Replace("--", "_");
                                    }
                                    if (fields.Length > 2)
                                    {
                                        Valor3 = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, GetFieldName(DateTime.Now, IDEmployee, fields[2]))).Replace("'", "''").Replace("--", "_");
                                    }
                                    if (fields.Length > 3)
                                    {
                                        Valor4 = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, GetFieldName(DateTime.Now, IDEmployee, fields[3]))).Replace("'", "''").Replace("--", "_");
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                        }
                        ValorDefecto = roTypes.Any2String(GetFieldValue(DateTime.Now, IDEmployee, "sysroEmergencyNumber")).Replace("'", "''").Replace("--", "_");
                        if (Valor1 == "") { Valor1 = "null"; } else { Valor1 = "'" + Valor1 + "'"; }
                        if (Valor2 == "") { Valor2 = "null"; } else { Valor2 = "'" + Valor2 + "'"; }
                        if (Valor3 == "") { Valor3 = "null"; } else { Valor3 = "'" + Valor3 + "'"; }
                        if (Valor4 == "") { Valor4 = "null"; } else { Valor4 = "'" + Valor4 + "'"; }
                        if (ValorDefecto == "") { ValorDefecto = "null"; } else { ValorDefecto = "'" + ValorDefecto + "'"; }

                        if (Strings.InStr(1, TerminalDescription, "##") > 0)
                        {
                            TerminalDescription = Strings.Left(TerminalDescription, Strings.InStr(1, TerminalDescription, "##") - 1);
                        }

                        bInsert = true;

                        if (roTypes.Any2String(adsDataRow["TerminalType"]).ToUpper() == "LIVEPORTAL")
                        {
                            //Para fichajes desde el Portal, o desde un Time Gate en modo Portal si no se asignaron zonas, miro si es una entrada o una salida. Si es salida, sólo lo muestro si han pasado menos de 15 minutos
                            double daysDifftotmin = ((TimeSpan)(DateTime.Now - Datetime)).TotalMinutes;
                            if (roTypes.Any2Integer(adsDataRow["ActualType"]) == 2 && daysDifftotmin > 15)
                            {
                                bInsert = false;
                            }
                        }

                        if (roTypes.Any2String(adsDataRow["TerminalType"]).ToUpper() == "TIME GATE")
                        {
                            //Para fichajes desde el Portal, o desde un Time Gate en modo Portal si no se asignaron zonas, miro si es una entrada o una salida. Si es salida, sólo lo muestro si han pasado menos de 15 minutos
                            double daysDifftotmin = ((TimeSpan)(DateTime.Now - Datetime)).TotalMinutes;
                            if (roTypes.Any2Integer(adsDataRow["ActualType"]) == 2 && daysDifftotmin > 15 && roTypes.Any2Integer(adsDataRow["IDZoneIn"]) == 0)
                            {
                                bInsert = false;
                            }
                            if (roTypes.Any2Integer(adsDataRow["IDZoneIn"]) != 0) IsGeolocated = 0;
                        }

                        //Creamos el registro
                        if (bInsert)
                        {
                            sSQL = "@INSERT# INTO TMPEmergencyBasic (IDEmployee,Name,TerminalDescription,EmergencyPoint,DateTime,EmpOrVisit,IDReportTask,EmergencyNumber,UserFieldValue,UserFieldValue2,UserFieldValue3,UserFieldValue4,IDZone,ZoneIsNotReliable,IsGeolocated) VALUES " +
                              "(" + IDEmployee + ",'" + Name + "','" + TerminalDescription + "','" + fld + "',convert(datetime,'" + Datetime + "',104),'Employee', " + IDReportTask + "" +
                              ", " + ValorDefecto + ", " + Valor1 + ", " + Valor2 + ", " + Valor3 + ", " + Valor4 + ", " + IDZone + ", " + ZoneIsNotReliable + ", " + IsGeolocated + ")";

                            Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                        }
                    }

                    if (ads.Rows.Count == 0)
                    {
                        // No hay empleados. Insertamos un registro "mágico" para que el subinforme de visitas tenga el IDReportTask.
                        // Si alguien aprende de informes, esto no haría falta si se puediese pasar al subinforme el parámetro ?IDReportTask en lugar de la columna IDReportTask de la tabla  TMPEmergencyBasic ...
                        sSQL = "@INSERT# INTO TMPEmergencyBasic (IDEmployee,Name,EmpOrVisit, IDReportTask) VALUES " +
                               "(0,'','Employee', " + IDReportTask + ")";
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }

                    ads.Dispose();
                }

                //**** VISITAS DE EMPLEADOS QUE ESTAN EN CURSO, ESTÉ EL EMEPLADO VISITADO EN ZONA DE EMERGENCIA O NO*********
                sSQL = $@"WITH RankedVisitsPunches AS (
	                    @SELECT#  ROW_NUMBER() OVER(PARTITION BY IDVisit ORDER BY DatePunch DESC) AS VisitorPunchOrder,
			                    IDVisit,
	                            DatePunch
	                    FROM Visit_Visitor_Punch
                    ),  RankedEmployeePunches AS (
	                    @SELECT#	ROW_NUMBER() OVER(PARTITION BY Employees.ID ORDER BY Employees.ID ASC, DateTime DESC) AS EmployeePunchOrder,
								Punches.DateTime,
			                    Employees.Id AS IDEmployee,
			                    Employees.Name AS EmployeeName,
			                    Punches.IDZone,
			                    ZoneIsNotReliable,
			                    Punches.Location,
								Terminals.Type AS TerminalType,
								TerminalReaders.IDZone AS IDZoneIn,
								TerminalReaders.IDZoneOut AS IDZoneOut
	                    FROM	Employees
	                    LEFT JOIN Punches ON Employees.Id = Punches.IDEmployee AND ShiftDate = CONVERT(DATE, GETDATE()) AND ActualType <> 6 AND IDZone <> 0 AND IDZone IS NOT NULL
						LEFT JOIN Terminals ON Punches.IDTerminal = Terminals.ID
						LEFT JOIN TerminalReaders ON TerminalReaders.IDTerminal = Terminals.id AND TerminalReaders.ID = 1
                    ), ElegibleEmployees AS (
						SELECT DISTINCT(IdEmployee)
						FROM Punches
						WHERE IdZone IN ({Zones}) AND DateTime >= DATEADD(dd,-90,getdate()) AND ActualType <> 6
					)

                    @SELECT# DISTINCT Visitor.Name AS VisitorName, 
				                    Visit.IDEmployee AS IDVisitedEmployee, 
				                    RankedEmployeePunches.EmployeeName,
				                    RankedEmployeePunches.IDZone,
				                    RankedEmployeePunches.ZoneIsNotReliable,
				                    RankedEmployeePunches.Location,
				                    RankedVisitsPunches.DatePunch,
									RankedEmployeePunches.TerminalType,
									RankedEmployeePunches.IDZoneIn,
									RankedEmployeePunches.IDZoneOut
                    FROM Visit 
                    INNER JOIN Visit_Visitor VisitVisitor on VisitVisitor.IDVisit = Visit.IDVisit  
                    INNER JOIN Visitor on Visitor.IDVisitor = VisitVisitor.IDVisitor  
                    INNER JOIN RankedVisitsPunches  on RankedVisitsPunches.IDVisit = Visit.IDVisit AND RankedVisitsPunches.DatePunch >= DATEADD(dd,-2,getdate()) AND RankedVisitsPunches.VisitorPunchOrder = 1
					INNER JOIN ElegibleEmployees ON ElegibleEmployees.IDEmployee = Visit.IDEmployee
                    LEFT  JOIN RankedEmployeePunches ON RankedEmployeePunches.IDEmployee = Visit.IDEmployee AND RankedEmployeePunches.EmployeePunchOrder = 1
                    WHERE Visit.Status = 1";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //   inserto al visitante. Si no, el visitante no tiene que mostrarse ¿o si?
                        VisitorName = adsDataRow.Field<string>("VisitorName");
                        IDEmployee = roTypes.Any2Integer(adsDataRow["IDVisitedEmployee"]);
                        Name = roTypes.Any2String(adsDataRow.Field<string>("EmployeeName")).Replace("'", "''").Replace("--", "_");
                        IDZone = roTypes.Any2Integer(adsDataRow["IDZone"]);
                        Datetime = adsDataRow.Field<DateTime>("DatePunch");
                        ZoneIsNotReliable = (adsDataRow["ZoneIsNotReliable"].Equals(true)) ? 1 : 0;
                        IsGeolocated = (roTypes.Any2String(adsDataRow["Location"]).Length > 0) ? 1 : 0;

                        if (roTypes.Any2String(adsDataRow["TerminalType"]).ToUpper() == "TIME GATE" && roTypes.Any2Integer(adsDataRow["IDZoneIn"]) != 0) IsGeolocated = 0;

                        //Creamos el registro
                        sSQL = "@INSERT# INTO TMPEmergencyBasic (IDEmployee,Name,TerminalDescription,UserFieldValue,DateTime,EmpOrVisit, EmployeeVisited, IDReportTask, IDZone, ZoneIsNotReliable,IsGeolocated) VALUES " +
                        "(" + IDEmployee + ",'" + VisitorName + "',' ',' ',convert(datetime,'" + Datetime + "',104),'Visit','" + Name + "', " + IDReportTask + ", " + IDZone + ", " + ZoneIsNotReliable + ", " + IsGeolocated + ")";

                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }

                    ads.Dispose();
                }

                bolRet = true;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpEmergencyWithAttendanceByZonesBasicEx::", ex);
            }

            return bolRet;
        }

        //Se usa!
        private bool CreateTmpCriterioSaldos(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string tipo_criterio = parametersList.Where(x => x.Name.Equals("PTTipoCriterio")).FirstOrDefault().Value.ToString();
            //no continuamos si el usuario no ha seleccionado la opción de saldos
            if (tipo_criterio != "1") return false;

            int passportId = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());
            //int IdConceptGroup = int.Parse(((Robotics.Base.conceptGroupsSelector[])parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptGroupsSelector")).FirstOrDefault().Value).FirstOrDefault().Value);            

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }
            if (empSelected.Length == 0) empSelected = new string[] { "-1" };

            //string saldos = parametersList.Where(x => x.Name.Equals("Saldos")).FirstOrDefault().Value.ToString();
            object profileTypes = ((object)parametersList.Where(x => x.Name.Equals("ProfileTypes")).FirstOrDefault().Value); //1=saldos; 2=causes; 3=incidences; 4=causes&incidences
            string saldos;
            try
            {
                var concepts = ((JObject)profileTypes)["selectConcepts"];
                bool hasValueProperty = concepts.Count() > 0 && concepts[0].Type == JTokenType.Object && concepts[0]["value"] != null;

                //Antiguamente venia un objeto que tenia value, ahora lo hemos modificado para que venga el valor directamente
                if (hasValueProperty)
                {
                    saldos = String.Join(",", concepts.Select(o => o["value"].ToString()).ToArray());
                }
                else
                {
                    saldos = String.Join(",", concepts.Select(o => o.ToString()).ToArray());
                }
            }
            catch
            {
                saldos = "";
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;
            string concept1 = parametersList.Where(x => x.Name.Equals("concept1")).FirstOrDefault()?.Value?.ToString() ?? "-1";
            string concept2 = parametersList.Where(x => x.Name.Equals("concept2")).FirstOrDefault()?.Value?.ToString() ?? "-1";
            string concept3 = parametersList.Where(x => x.Name.Equals("concept3")).FirstOrDefault()?.Value?.ToString() ?? "-1";
            string concept4 = parametersList.Where(x => x.Name.Equals("concept4")).FirstOrDefault()?.Value?.ToString() ?? "-1";
            string concept5 = parametersList.Where(x => x.Name.Equals("concept5")).FirstOrDefault()?.Value?.ToString() ?? "-1";
            string concept6 = parametersList.Where(x => x.Name.Equals("concept6")).FirstOrDefault()?.Value?.ToString() ?? "-1";

            string criterio = parametersList.FirstOrDefault(x => x.Name == "PTCriterio")?.Value?.ToString() ?? "mayor";
            string tipoValorCriterio = parametersList.FirstOrDefault(x => x.Name == "PTTipoValorCriterio")?.Value?.ToString() ?? "valor";
            string rango_criterio = parametersList.FirstOrDefault(x => x.Name == "PTRangoCriterio")?.Value?.ToString() ?? "diariamente";
            string str_valor_criterio = parametersList.FirstOrDefault(x => x.Name == "PTValorCriterio")?.Value?.ToString() ?? "0";
            string valor_criterio = Robotics.VTBase.roConversions.ConvertTimeToHours(str_valor_criterio).ToString().Replace(',', '.');
            string valorFicha_criterio = parametersList.FirstOrDefault(x => x.Name == "PTValorFichaCriterio")?.Value?.ToString() ?? "";

            //TODO: Seleccionar el nuevo filtro de campo ficha si estuviera seleccionado.
            criterio = (criterio == "mayor") ? ">=" : "<=";

            string sSQL;

            bool bolRet = false;            
            DataTable ads;
                                                            
            string Concept1 = "0";
            string Concept2 = "0";
            string Concept3 = "0";
            string Concept4 = "0";
            string Concept5 = "0";
            string Concept6 = "0";            

            try
            {
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPReports_CriterioSaldos WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");
                

                AccessHelper.BulkCopyIdList("EmployeeIds", empSelected);

                string strAuxValue = valor_criterio;
                if (tipoValorCriterio == "campo")
                {
                    strAuxValue = "cast((@SELECT# top 1 Value from EmployeeUserFieldValues with (nolock) where IDEmployee = emp.ID And FieldName = (@SELECT# FieldName from sysroUserFields Where ID = " + valorFicha_criterio + ") and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc ) as nvarchar) ";
                }

                sSQL = "@SELECT# * FROM (" +
                 "@SELECT# emp.ID AS EmployeeID, emp.Name As EmployeeName, " +
                 "ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, cs.ID as ConceptID, ";
                if (rango_criterio == "diariamente")
                {
                    sSQL += "dsc.Date, ds.Value, (@SELECT# CAST(Text as nvarchar(max)) from sysroRemarks where ID = dsc.Remarks) as Remarks, '' AS ValorCriterio ";
                }
                else
                {
                    sSQL += "NULL as Date, sum(ds.Value) as Value, '' as Remarks, " + strAuxValue + " AS ValorCriterio ";
                }
                sSQL += "FROM DailySchedule dsc with (nolock) inner join Employees emp with (nolock) on emp.ID = dsc.IDEmployee ";
                string join = (criterio.ToString() == "<=") ? "left join" : "inner join"; //si tenemos que mostrar valores 0, añadimos en el informe los días dentro del rango de fechas que no tengan dato
                sSQL += join + " DailyAccruals ds with (nolock) on ds.Date = dsc.Date and dsc.IDEmployee = ds.IDEmployee " +
                "and ds.IDConcept IN (" + saldos + ") ";
                if (rango_criterio == "diariamente") sSQL += "and ds.Value " + criterio + " " + strAuxValue + " ";
                sSQL += " inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = dsc.IDEmployee " +
                "and dsc.Date between ec.BeginDate and ec.EndDate ";

                if (passportId > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                        " AND poe.IDEmployee = emp.ID And ds.Date between poe.BeginDate And poe.EndDate " +
                        " and poe.IdFeature = 1 and poe.FeaturePermission > 1 ";
                    sSQL += " INNER JOIN #EmployeeIds ON #EmployeeIds.Id = poe.IDEmployee ";
                }

                sSQL += "inner join EmployeeGroups eg with (nolock) on eg.IDEmployee = dsc.IDEmployee " +
                "and dsc.Date between eg.BeginDate and eg.EndDate " +
                "inner join Groups g with (nolock) on eg.IDGroup = g.ID " +
                "left join Concepts cs with (nolock) on ds.IDConcept = cs.ID " +
                "left join sysroRemarks as r with (nolock) on dsc.Remarks = r.ID " +
                "where dsc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' ";

                sSQL += "group by emp.ID, emp.Name, ec.IDContract, g.Id, g.Name, g.FullGroupName, cs.ID";
                if (rango_criterio == "diariamente")
                {
                    sSQL += ", ds.Value, dsc.Date, Remarks) t PIVOT(MAX(Value) ";
                }
                else
                {
                    sSQL += " HAVING SUM(ds.value) " + criterio + " " + strAuxValue + ") t PIVOT(SUM(Value) ";
                }
                sSQL += " FOR ConceptID IN([" + concept1 + "]";
                if (concept2 != null && concept2 != "-1") sSQL += ", [" + concept2 + "]";
                if (concept3 != null && concept3 != "-1") sSQL += ", [" + concept3 + "]";
                if (concept4 != null && concept4 != "-1") sSQL += ", [" + concept4 + "]";
                if (concept5 != null && concept5 != "-1") sSQL += ", [" + concept5 + "]";
                if (concept6 != null && concept6 != "-1") sSQL += ", [" + concept6 + "]";
                sSQL += ")) AS pivot_table order by GroupName, Date;";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    DataTable proxyTable = AccessHelper.CreateDataTable("@SELECT# TOP 1 * FROM TMPReports_CriterioSaldos WHERE 1 = 2 ");
                    foreach (DataRow adsDataRow in ads.Rows)
                    {                        
                        Concept1 = (roTypes.Any2String(adsDataRow[concept1]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept1]);
                        if (concept2 != null && concept2 != "-1")
                        {
                            Concept2 = (roTypes.Any2String(adsDataRow[concept2]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept2]);
                        }
                        if (concept3 != null && concept3 != "-1")
                        {
                            Concept3 = (roTypes.Any2String(adsDataRow[concept3]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept3]);
                        }
                        if (concept4 != null && concept4 != "-1")
                        {
                            Concept4 = (roTypes.Any2String(adsDataRow[concept4]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept4]);
                        }
                        if (concept5 != null && concept5 != "-1")
                        {
                            Concept5 = (roTypes.Any2String(adsDataRow[concept5]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept5]);
                        }
                        if (concept6 != null && concept6 != "-1")
                        {
                            Concept6 = (roTypes.Any2String(adsDataRow[concept6]) == "") ? "0" : roTypes.Any2String(adsDataRow[concept6]);
                        }
                        string ValorCriterio = (roTypes.Any2String(adsDataRow["ValorCriterio"]) == "") ? "0" : roTypes.Any2String(adsDataRow["ValorCriterio"]).Replace(".", ",");

                        if (rango_criterio != "diariamente")
                        {
                            if (criterio.ToString() == "<=")
                            {
                                Concept1 = (roTypes.Any2Double(Concept1) <= roTypes.Any2Double(ValorCriterio)) ? Concept1 : "0";
                                Concept2 = (roTypes.Any2Double(Concept2) <= roTypes.Any2Double(ValorCriterio)) ? Concept2 : "0";
                                Concept3 = (roTypes.Any2Double(Concept3) <= roTypes.Any2Double(ValorCriterio)) ? Concept3 : "0";
                                Concept4 = (roTypes.Any2Double(Concept4) <= roTypes.Any2Double(ValorCriterio)) ? Concept4 : "0";
                                Concept5 = (roTypes.Any2Double(Concept5) <= roTypes.Any2Double(ValorCriterio)) ? Concept5 : "0";
                                Concept6 = (roTypes.Any2Double(Concept6) <= roTypes.Any2Double(ValorCriterio)) ? Concept6 : "0";
                            }
                            else
                            {
                                Concept1 = (roTypes.Any2Double(Concept1) >= roTypes.Any2Double(ValorCriterio)) ? Concept1 : "0";
                                Concept2 = (roTypes.Any2Double(Concept2) >= roTypes.Any2Double(ValorCriterio)) ? Concept2 : "0";
                                Concept3 = (roTypes.Any2Double(Concept3) >= roTypes.Any2Double(ValorCriterio)) ? Concept3 : "0";
                                Concept4 = (roTypes.Any2Double(Concept4) >= roTypes.Any2Double(ValorCriterio)) ? Concept4 : "0";
                                Concept5 = (roTypes.Any2Double(Concept5) >= roTypes.Any2Double(ValorCriterio)) ? Concept5 : "0";
                                Concept6 = (roTypes.Any2Double(Concept6) >= roTypes.Any2Double(ValorCriterio)) ? Concept6 : "0";
                            }
                        }                        
                                                
                            //Añado un registro a proxyTable
                            DataRow newRow = proxyTable.NewRow();
                            newRow["EmployeeID"] = adsDataRow["EmployeeID"];
                            newRow["EmployeeName"] = adsDataRow.Field<string>("EmployeeName");
                            newRow["Contract"] = adsDataRow.Field<string>("Contract");
                            newRow["IDGroup"] = adsDataRow["IDGroup"];
                            newRow["GroupName"] = adsDataRow.Field<string>("GroupName");
                            newRow["Ruta"] = adsDataRow.Field<string>("Ruta");
                            newRow["Concept1"] = Concept1;
                            newRow["Concept2"] = Concept2;
                            newRow["Concept3"] = Concept3;
                            newRow["Concept4"] = Concept4;
                            newRow["Concept5"] = Concept5;
                            newRow["Concept6"] = Concept6;
                            newRow["Remarks"] = adsDataRow.Field<string>("Remarks");
                            newRow["IDReportTask"] = IDReportTask;


                            if (rango_criterio == "diariamente")
                            {                                
                                newRow["Date"] = adsDataRow["Date"];
                            }

                            proxyTable.Rows.Add(newRow);                                                                        
                    }

                    AccessHelper.BulkCopy("TMPReports_CriterioSaldos", proxyTable);

                    ads.Dispose();
                }

                bolRet = true;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpCriterioSaldos::", ex);
            }

            return bolRet;
        }
        //Se usa!
        private bool CreateTmpCriterioCausesIncidences(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string tipo_criterio = parametersList.Where(x => x.Name.Equals("PTTipoCriterio")).FirstOrDefault().Value.ToString();
            //no continuamos si el usuario no ha seleccionado justificaciones o incidencias; tipo_criterio == 2 (justificaciones), 3 (incidencias), 4 (justificaciones e incidencias)
            if (!(tipo_criterio == "2" || tipo_criterio == "3" || tipo_criterio == "4")) return false;

            int passportId = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }
            if (empSelected.Length == 0) empSelected = new string[] { "-1" };

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;


            string criterio = parametersList.FirstOrDefault(x => x.Name == "PTCriterio")?.Value?.ToString() ?? "mayor";
            string tipoValorCriterio = parametersList.FirstOrDefault(x => x.Name == "PTTipoValorCriterio")?.Value?.ToString() ?? "valor";
            string rango_criterio = parametersList.FirstOrDefault(x => x.Name == "PTRangoCriterio")?.Value?.ToString() ?? "diariamente";
            string str_valor_criterio = parametersList.FirstOrDefault(x => x.Name == "PTValorCriterio")?.Value?.ToString() ?? "0";
            string valor_criterio = Robotics.VTBase.roConversions.ConvertTimeToHours(str_valor_criterio).ToString().Replace(',', '.');
            string valorFicha_criterio = parametersList.FirstOrDefault(x => x.Name == "PTValorFichaCriterio")?.Value?.ToString() ?? "";

            //Tener en cuenta el nuevo selector campo de la ficha en caso de estar seleccionado
            object profileTypes = ((object)parametersList.Where(x => x.Name.Equals("ProfileTypes")).FirstOrDefault().Value); //1=saldos; 2=causes; 3=incidences; 4=causes&incidences
            string incidences;
            string causes;
            try
            {
                var tmpCauses = ((JObject)profileTypes)["selectCauses"];
                bool hasValueProperty = tmpCauses.Count() > 0 && tmpCauses[0].Type == JTokenType.Object && tmpCauses[0]["value"] != null;

                //Antiguamente venia un objeto que tenia value, ahora lo hemos modificado para que venga el valor directamente
                if (hasValueProperty)
                {
                    causes = String.Join(",", tmpCauses.Select(o => o["value"].ToString()).ToArray());
                }
                else
                {
                    causes = String.Join(",", tmpCauses.Select(o => o.ToString()).ToArray());
                }
            }
            catch
            {
                causes = "";
            }

            try
            {
                var tmpIncidences = ((JObject)profileTypes)["selectIncidences"];
                bool hasValueProperty = tmpIncidences.Count() > 0 && tmpIncidences[0].Type == JTokenType.Object && tmpIncidences[0]["value"] != null;

                //Antiguamente venia un objeto que tenia value, ahora lo hemos modificado para que venga el valor directamente
                if (hasValueProperty)
                {
                    incidences = String.Join(",", tmpIncidences.Select(o => o["value"].ToString()).ToArray());
                }
                else
                {
                    incidences = String.Join(",", tmpIncidences.Select(o => o.ToString()).ToArray());
                }
            }
            catch
            {
                incidences = "";
            }

            criterio = (criterio == "mayor") ? ">=" : "<=";

            string sSQL;

            bool bolRet = false;
            DataTable ads;

            String DateStart;
            String DateEnd;

            try
            {
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPReports_CriterioCausesIncidences WITH (ROWLOCK) WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                string strAuxValue = valor_criterio;
                if (tipoValorCriterio == "campo")
                {
                    strAuxValue = "cast((@SELECT# top 1 Value from EmployeeUserFieldValues with (nolock) where IDEmployee = emp.ID And FieldName = (@SELECT# FieldName from sysroUserFields Where ID = " + valorFicha_criterio + ") and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc ) as nvarchar) ";
                }                


                AccessHelper.BulkCopyIdList("EmployeeIds", empSelected);

                sSQL = "";
                //Primero llenamos las filas que no tienen valor en cada día de la fecha seleccionada, por cada tipo de justificacion
                if (criterio == "<=" && causes != "" && tipo_criterio == "2")
                {
                    foreach (string idCause in causes.Split(','))
                    {
                        sSQL += "@select# emp.ID as EmployeeID, emp.Name as EmployeeName, " +
                            "ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, NULL as Remarks, ";
                        if (rango_criterio == "diariamente")
                        {
                            sSQL += "dsc.Date, NULL as Incidence, NULL as IDIncidence," +
                            "(@SELECT# Causes.Name from Causes where Causes.ID = " + idCause + ") as Cause, " + idCause + " as ID, NULL as BeginTime, NULL as EndTime, 0 as Value ";
                        }
                        else
                        {
                            sSQL += "NULL as Incidence, (@SELECT# Causes.Name from Causes where Causes.ID = " + idCause + ") as Cause, 0 as Value ";
                        }
                        sSQL += "from DailySchedule as dsc with (nolock) " +
                            "left join DailyIncidences as di with (nolock) on di.IDEmployee = dsc.IDEmployee and di.Date = dsc.Date " +
                            "left JOIN sysroDailyIncidencesDescription as did with (nolock) ON di.IDType = did.IDIncidence " +
                            "left JOIN DailyCauses as dc with (nolock) ON di.ID = dc.IDRelatedIncidence and dsc.IDEmployee = dc.IDEmployee and dsc.Date = dc.Date " +
                            "LEFT JOIN Causes as c with (nolock) ON dc.IDCause = c.ID " +
                            "INNER JOIN Employees emp with (nolock) on emp.ID = dsc.IDEmployee ";

                        if (passportId > 0)
                        {
                            sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                                " AND poe.IDEmployee = dsc.IDEmployee And dsc.Date between poe.BeginDate And poe.EndDate " + //" AND poe.IDEmployee IN (" + empFilter + ") and poe.IdFeature = 1 and poe.FeaturePermission > 1 ";
                                " AND poe.IdFeature = 1 and poe.FeaturePermission > 1";
                            sSQL += " INNER JOIN #EmployeeIds ON #EmployeeIds.Id = poe.IDEmployee ";
                        }

                        sSQL += "inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = dsc.IDEmployee " +
                            "and dsc.Date between ec.BeginDate and ec.EndDate " +
                            "inner join EmployeeGroups eg with (nolock) on eg.IDEmployee = dsc.IDEmployee " +
                            "and dsc.Date between eg.BeginDate and eg.EndDate " +
                            "inner join Groups g with (nolock) on eg.IDGroup = g.ID " +
                            "left join sysroRemarks as r with (nolock) on dsc.Remarks = r.ID " +
                            "where dsc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' ";

                        if (rango_criterio == "diariamente")
                        {
                            sSQL += "AND dsc.Date NOT IN (@SELECT# sdc.Date from DailyCauses sdc with (nolock) WHERE sdc.IDEmployee = dsc.IDEmployee AND sdc.IDCause = " + idCause + ") UNION ";
                        }
                        else
                        {
                            sSQL += "AND dsc.IDEmployee NOT IN (@SELECT# sdc.IDEmployee from DailyCauses sdc with (nolock) INNER JOIN #EmployeeIds ON #EmployeeIds.Id = sdc.IDEmployee WHERE sdc.IDCause = " + idCause + " and sdc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' GROUP BY sdc.IDEmployee HAVING SUM(sdc.Value) <= " + strAuxValue + " or SUM(sdc.Value) >= " + strAuxValue + ") UNION ";
                        }
                    }
                }
                if (criterio == "<=" && incidences != "" && tipo_criterio == "3")
                {
                    foreach (string idIncidence in incidences.Split(','))
                    {
                        sSQL += "@select# emp.ID as EmployeeID, emp.Name as EmployeeName, " +
                            "ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, NULL as Remarks, ";
                        if (rango_criterio == "diariamente")
                        {
                            sSQL += "dsc.Date, (@SELECT# dids.Description from sysroDailyIncidencesDescription as dids where dids.IDIncidence = " + idIncidence + ") as Incidence, " + idIncidence + " as IDIncidence," +
                            "NULL as Cause, NULL as ID, NULL as BeginTime, NULL as EndTime, 0 as Value ";
                        }
                        else
                        {
                            sSQL += "(@SELECT# dids.Description from sysroDailyIncidencesDescription as dids where dids.IDIncidence = " + idIncidence + ") as Incidence, NULL as Cause, 0 as Value ";
                        }
                        sSQL += "from DailySchedule as dsc with (nolock) " +
                            "left join DailyIncidences as di with (nolock) on di.IDEmployee = dsc.IDEmployee and di.Date = dsc.Date " +
                            "left JOIN sysroDailyIncidencesDescription as did with (nolock) ON di.IDType = did.IDIncidence " +
                            "left JOIN DailyCauses as dc with (nolock) ON di.ID = dc.IDRelatedIncidence and dsc.IDEmployee = dc.IDEmployee and dsc.Date = dc.Date " +
                            "LEFT JOIN Causes as c with (nolock) ON dc.IDCause = c.ID " +
                            "INNER JOIN Employees emp with (nolock) on emp.ID = dsc.IDEmployee ";

                        if (passportId > 0)
                        {
                            sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                                " AND poe.IDEmployee = dsc.IDEmployee And dsc.Date between poe.BeginDate And poe.EndDate "; // " AND poe.IDEmployee IN (" + empFilter + ") ";
                            sSQL += " INNER JOIN #EmployeeIds ON #EmployeeIds.Id = poe.IDEmployee ";
                        }

                        sSQL += "inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = dsc.IDEmployee " +
                            "and dsc.Date between ec.BeginDate and ec.EndDate " +
                            "inner join EmployeeGroups eg with (nolock) on eg.IDEmployee = dsc.IDEmployee " +
                            "and dsc.Date between eg.BeginDate and eg.EndDate " +
                            "inner join Groups g with (nolock) on eg.IDGroup = g.ID " +
                            "left join sysroRemarks as r with (nolock) on dsc.Remarks = r.ID " +
                            "where dsc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' ";

                        if (rango_criterio == "diariamente")
                        {
                            sSQL += "AND dsc.Date NOT IN (@SELECT# sdi.Date from DailyIncidences sdi with (nolock) WHERE sdi.IDEmployee = dsc.IDEmployee AND sdi.IDType = " + idIncidence + " and sdi.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "') UNION ";
                        }
                        else
                        {
                            sSQL += "AND dsc.IDEmployee NOT IN (@SELECT# sdc.IDEmployee from DailyIncidences sdi with (nolock) INNER JOIN DailyCauses as sdc with (nolock) ON sdi.ID = sdc.IDRelatedIncidence and sdi.IDEmployee = sdc.IDEmployee and sdi.Date = sdc.Date INNER JOIN #EmployeeIds ON #EmployeeIds.Id = sdc.IDEmployee WHERE sdi.IDType = " + idIncidence + " and sdc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' GROUP BY sdc.IDEmployee, sdi.IDType, sdc.IDCause HAVING SUM(sdc.Value) <= " + strAuxValue + " or SUM(sdc.Value) >= " + strAuxValue + ") UNION ";
                        }
                    }
                }
                if (criterio == "<=" && incidences != "" && causes != "" && tipo_criterio == "4")
                {
                    sSQL += "@select# emp.ID as EmployeeID, emp.Name as EmployeeName, " +
                            "ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, NULL as Remarks, ";
                    if (rango_criterio == "diariamente")
                    {
                        sSQL += "dsc.Date, (@SELECT# dids.Description from sysroDailyIncidencesDescription as dids with (nolock) where dids.IDIncidence = " + incidences + ") as Incidence, " + incidences + " as IDIncidence," +
                        "(@SELECT# Causes.Name from Causes with (nolock) where Causes.ID = " + causes + ") as Cause, " + causes + " as ID, NULL as BeginTime, NULL as EndTime, 0 as Value ";
                    }
                    else
                    {
                        sSQL += "(@SELECT# dids.Description from sysroDailyIncidencesDescription as dids with (nolock) where dids.IDIncidence = " + incidences + ") as Incidence, (@SELECT# Causes.Name from Causes where Causes.ID = " + causes + ") as Cause, 0 as Value ";
                    }
                    sSQL += "from DailySchedule as dsc with (nolock) " +
                        "left join DailyIncidences as di with (nolock) on di.IDEmployee = dsc.IDEmployee and di.Date = dsc.Date " +
                        "left JOIN sysroDailyIncidencesDescription as did with (nolock) ON di.IDType = did.IDIncidence " +
                        "left JOIN DailyCauses as dc with (nolock) ON di.ID = dc.IDRelatedIncidence and dsc.IDEmployee = dc.IDEmployee and dsc.Date = dc.Date " +
                        "LEFT JOIN Causes as c with (nolock) ON dc.IDCause = c.ID " +
                        "INNER JOIN Employees emp with (nolock) on emp.ID = dsc.IDEmployee ";

                    if (passportId > 0)
                    {
                        sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                            " AND poe.IDEmployee = dsc.IDEmployee And dsc.Date between poe.BeginDate And poe.EndDate "; //" AND poe.IDEmployee IN (" + empFilter + ") ";
                        sSQL += " INNER JOIN #EmployeeIds ON #EmployeeIds.Id = poe.IDEmployee ";
                    }

                    sSQL += "inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = dsc.IDEmployee " +
                        "and dsc.Date between ec.BeginDate and ec.EndDate " +
                        "inner join EmployeeGroups eg with (nolock) on eg.IDEmployee = dsc.IDEmployee " +
                        "and dsc.Date between eg.BeginDate and eg.EndDate " +
                        "inner join Groups g with (nolock) on eg.IDGroup = g.ID " +
                        "left join sysroRemarks as r with (nolock) on dsc.Remarks = r.ID " +
                        "where dsc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' ";

                    if (rango_criterio == "diariamente")
                    {
                        sSQL += "AND dsc.Date NOT IN (@SELECT# sdc.Date from DailyCauses sdc with (nolock) JOIN DailyIncidences as sdi with (nolock) ON sdi.ID = sdc.IDRelatedIncidence and sdi.IDEmployee = sdc.IDEmployee and sdi.Date = sdc.Date WHERE sdc.IDEmployee = dsc.IDEmployee and sdi.IDEmployee = dsc.IDEmployee AND sdc.IDCause = " + causes + " and sdi.IDType = " + incidences + " and sdc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' and (sdi.Value <= " + strAuxValue + " or sdi.Value >= " + strAuxValue + ") GROUP BY sdc.Date) UNION ";
                    }
                    else
                    {
                        sSQL += "AND dsc.IDEmployee NOT IN (@SELECT# sdc.IDEmployee from DailyIncidences sdi with (nolock) INNER JOIN DailyCauses as sdc with (nolock) ON sdi.ID = sdc.IDRelatedIncidence and sdi.IDEmployee = sdc.IDEmployee and sdi.Date = sdc.Date INNER JOIN #EmployeeIds ON #EmployeeIds.Id = sdc.IDEmployee WHERE sdi.IDType = " + incidences + " and sdc.IDCause = " + causes + " and sdc.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' GROUP BY sdc.IDEmployee, sdi.IDType, sdc.IDCause HAVING SUM(sdc.Value) <= " + strAuxValue + " or SUM(sdc.Value) >= " + strAuxValue + ") UNION ";
                    }
                }

                sSQL += "@select# emp.ID as EmployeeID, emp.Name as EmployeeName, " +
                    "ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, ";

                if (rango_criterio == "diariamente")
                {
                    sSQL += "(@SELECT# CAST(Text as nvarchar(max)) from sysroRemarks where ID = dsc.Remarks) as Remarks, di.Date, did.Description as Incidence, did.IDIncidence, c.Name as Cause, c.ID, di.BeginTime, di.EndTime, dc.Value ";
                }
                else
                {
                    sSQL += "NULL As Remarks, did.Description as Incidence, c.Name as Cause, SUM(dc.Value) as Value ";
                }

                sSQL += "from DailyIncidences as di with (nolock) " +
                        "INNER JOIN sysroDailyIncidencesDescription as did with (nolock) ON di.IDType = did.IDIncidence " +
                        "INNER JOIN DailyCauses as dc with (nolock) ON di.ID = dc.IDRelatedIncidence and di.IDEmployee = dc.IDEmployee and di.Date = dc.Date " +
                        "RIGHT JOIN Causes as c with (nolock) ON dc.IDCause = c.ID " +
                        "INNER JOIN Employees emp with (nolock) on emp.ID = di.IDEmployee ";

                if (passportId > 0)
                {
                    sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                        " AND poe.IDEmployee = di.IDEmployee And di.Date between poe.BeginDate And poe.EndDate ";
                    sSQL += " INNER JOIN #EmployeeIds ON #EmployeeIds.Id = poe.IDEmployee ";
                }

                sSQL += "inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = di.IDEmployee " +
                        "and di.Date between ec.BeginDate and ec.EndDate " +
                        "inner join EmployeeGroups eg with (nolock) on eg.IDEmployee = di.IDEmployee " +
                        "and di.Date between eg.BeginDate and eg.EndDate " +
                        "inner join Groups g with (nolock) on eg.IDGroup = g.ID " +
                        "left join DailySchedule as dsc with (nolock) on di.IDEmployee = dsc.IDEmployee and di.Date = dsc.Date " +
                        "left join sysroRemarks as r with (nolock) on dsc.Remarks = r.ID " +
                        "where di.Date between '" + StartDate.ToString("yyyyMMdd") + "' and '" + EndDate.ToString("yyyyMMdd") + "' ";

                if (tipo_criterio == "2" || tipo_criterio == "4") sSQL += "and c.ID in (" + causes + ") ";
                if (tipo_criterio == "3" || tipo_criterio == "4") sSQL += "and did.IDIncidence in (" + incidences + ") ";

                if (rango_criterio == "diariamente")
                {
                    sSQL += "and dc.Value " + criterio + " " + strAuxValue +
                        " order by g.FullGroupName, dsc.Date asc";
                }
                else
                {
                    sSQL += "GROUP BY emp.ID, emp.Name, ec.IDContract, g.Id, g.Name, g.FullGroupName, did.Description, c.Name " +
                        "HAVING SUM(dc.Value) " + criterio + " " + strAuxValue;
                }

                ads = Robotics.DataLayer.AccessHelper.CreateDataTableWithoutTimeouts(sSQL);

                if (ads != null)
                {
                    DataTable proxyTable = AccessHelper.CreateDataTable("@SELECT# TOP 1 * FROM TMPReports_CriterioCausesIncidences WHERE 1 = 2 ");
                    foreach (DataRow adsDataRow in ads.Rows)
                    {

                        //Añado un registro a proxyTable
                        DataRow newRow = proxyTable.NewRow();
                        newRow["EmployeeID"] = adsDataRow["EmployeeID"];
                        newRow["EmployeeName"] = adsDataRow.Field<string>("EmployeeName");
                        newRow["Contract"] = adsDataRow.Field<string>("Contract");
                        newRow["IDGroup"] = adsDataRow["IDGroup"];
                        newRow["GroupName"] = adsDataRow.Field<string>("GroupName");
                        newRow["Ruta"] = adsDataRow.Field<string>("Ruta");
                        newRow["Incidence"] = adsDataRow.Field<string>("Incidence");
                        newRow["Cause"] = adsDataRow.Field<string>("Cause");
                        newRow["Value"] = (roTypes.Any2String(adsDataRow["Value"]) == "") ? 0 : adsDataRow["Value"];
                        newRow["Remarks"] = adsDataRow.Field<string>("Remarks");
                        newRow["IDReportTask"] = IDReportTask;


                        if (rango_criterio == "diariamente")
                        {
                            DateStart = (roTypes.Any2String(adsDataRow["BeginTime"]).Split(' ').Length > 1) ? roTypes.Any2String(adsDataRow["BeginTime"]).Split(' ')[1] : roTypes.Any2String(adsDataRow["BeginTime"]).Split(' ')[0];
                            DateEnd = (roTypes.Any2String(adsDataRow["EndTime"]).Split(' ').Length > 1) ? roTypes.Any2String(adsDataRow["EndTime"]).Split(' ')[1] : roTypes.Any2String(adsDataRow["EndTime"]).Split(' ')[0];

                            newRow["StartDate"] = DateStart;
                            newRow["EndDate"] = DateEnd;
                            newRow["Date"] = adsDataRow["Date"];
                        }

                        proxyTable.Rows.Add(newRow);
                    }

                    AccessHelper.BulkCopy("TMPReports_CriterioCausesIncidences", proxyTable);

                    ads.Dispose();
                }

                bolRet = true;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpCriterioCausesIncidences::", ex);
            }

            return bolRet;
        }

        private bool CreateTmpDetailedCalendarEmployeeEx2(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);
            int passportId = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());
            int IdConceptGroup = int.Parse(((Robotics.Base.conceptGroupsSelector[])parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptGroupsSelector")).FirstOrDefault().Value).FirstOrDefault().Value);
            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");

            DataTable dsEmployees;
            DataTable dsConcepts;
            DataTable dsPunches;
            DataTable dsInfo;

            DateTime StartWorkDate;
            DateTime EndWorkDate;

            string sSQL;
            bool ConceptsEnabled;
            string strConceptList;
            int IndexConcept;

            double[] ConceptValues;

            DataRow[] DBArrayEmployees = new DataRow[] { };
            long IndexEmployees;

            DataRow[] DBArrayConcepts = new DataRow[] { };
            long IndexConcepts;

            DataRow[] DBArrayInfo = new DataRow[] { };
            long IndexInfo;

            DataRow[] DBArrayPunches = new DataRow[] { };
            long IndexPunches;

            string strInsert;
            string strValues;
            string strFields;

            try
            {
                if (groups == String.Empty) return true;

                if (roTypes.Any2String(Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Robotics\\VisualTime\\Server", "ExistReportData", "False")).Equals("1") && IDReportTask == 0) return true;

                //VACIAR TABLA TEMPORAL
                //MTH: No borramos datas para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPDetailedCalendarEmployee WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //Obtener empleados seleccionados. En los parametros llega sentencia SQL cerrada por parentesis. Recortamos ultimo parentesis para anidar AND nuevo.
                sSQL = "@SELECT# sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IdGroup, Employees.Name AS EmployeeName, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , isnull(dbo.GetValueFromEmployeeUserFieldValues(sysroEmployeeGroups.IDEmployee,'NIF', getdate()),'') as FieldValue FROM sysroEmployeeGroups inner join Employees on employees.ID = sysroEmployeeGroups.IDEmployee and " + groups;
                sSQL = Strings.Left(sSQL, Strings.Len(sSQL) - 1) + " AND Employees.ID=sysroEmployeeGroups.IDEmployee) ";

                if (passportId > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + passportId + ",sysroEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " ORDER BY Employees.Name, sysroEmployeeGroups.BeginDate ";

                sSQL = sSQL.Replace("sysrovwAllEmployeeGroups", "sysroEmployeeGroups");

                dsEmployees = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (dsEmployees != null)
                {
                    DBArrayEmployees = dsEmployees.AsEnumerable().ToArray();
                    dsEmployees.Dispose();
                }

                //If dsDetailed.OpenRecordset("SELECT TOP 1 * FROM TMPDetailedCalendarEmployee", conn, adOpenDynamic, adLockOptimistic) Then

                ConceptsEnabled = false;
                strConceptList = String.Empty;

                if (IdConceptGroup > 0)
                {
                    sSQL = "@SELECT# TOP (8) Concepts.ID, Concepts.ShortName, Concepts.IDType FROM sysroReportGroupConcepts INNER JOIN Concepts ON sysroReportGroupConcepts.IDConcept = Concepts.ID " +
                           "WHERE (sysroReportGroupConcepts.IDReportGroup = " + IdConceptGroup + ") ORDER BY sysroReportGroupConcepts.Position";
                }

                dsConcepts = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (dsConcepts != null)
                {
                    DBArrayConcepts = new DataRow[] { };
                    DBArrayConcepts = dsConcepts.AsEnumerable().ToArray();
                    dsConcepts.Dispose();

                    for (IndexConcepts = 0; IndexConcepts < DBArrayConcepts.Length; IndexConcepts++)
                    {
                        ConceptsEnabled = true;
                        strConceptList = strConceptList + DBArrayConcepts[IndexConcepts].ItemArray[0] + ",";
                    }

                    strConceptList = Strings.Left(strConceptList, Strings.Len(strConceptList) - 1);
                }

                for (IndexEmployees = 0; IndexEmployees < DBArrayEmployees.Length; IndexEmployees++)
                {
                    if (((DateTime)DBArrayEmployees[IndexEmployees][4] >= StartDate && (DateTime)DBArrayEmployees[IndexEmployees][4] <= EndDate) ||
                       ((DateTime)DBArrayEmployees[IndexEmployees][5] >= StartDate && (DateTime)DBArrayEmployees[IndexEmployees][5] <= EndDate) ||
                       ((DateTime)DBArrayEmployees[IndexEmployees][4] <= StartDate && (DateTime)DBArrayEmployees[IndexEmployees][5] >= EndDate))
                    {
                        if ((DateTime)DBArrayEmployees[IndexEmployees][4] < StartDate)
                        {
                            StartWorkDate = StartDate;
                        }
                        else
                        {
                            StartWorkDate = (DateTime)DBArrayEmployees[IndexEmployees][4];
                        }

                        if ((DateTime)DBArrayEmployees[IndexEmployees][5] > EndDate)
                        {
                            EndWorkDate = EndDate;
                        }
                        else
                        {
                            EndWorkDate = (DateTime)DBArrayEmployees[IndexEmployees][5];
                        }

                        //                ' Selecciona datos del empleado
                        sSQL = "@select# DATES.Date, EC.idEmployee, EC.idContract, ISNULL(DS.IDShiftUsed, ISNULL(DS.IDShift1, 0)) AS IDShift, " +
                                       "(@SELECT# TOP(1) 1 From DailyCauses DC WHERE DC.IDCause = 0 AND  DC.IDEmployee = EC.idEmployee and dc.date=dates.date) as CausesNotJustified " +
                                       "from dbo.ExplodeDates('" + StartWorkDate.ToString("yyyyMMdd") + "','" + EndWorkDate.ToString("yyyyMMdd") + "') as DATES " +
                                       "    Left JOIN EmployeeContracts EC on EC.idEmployee=" + DBArrayEmployees[IndexEmployees][0] + " and DATES.Date between BeginDate and EndDate " +
                                       "    Left JOIN DailySchedule DS on DS.IDEmployee=EC.idEmployee and Dates.Date=DS.date " +
                                       "where not EC.idContract is null " +
                                       "order by ec.idContract,dates.date";
                        //                DoEvents

                        dsInfo = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (dsInfo != null)
                        {
                            DBArrayInfo = dsInfo.AsEnumerable().ToArray();
                            dsInfo.Dispose();
                        }

                        for (IndexInfo = 0; IndexInfo < DBArrayInfo.Length; IndexInfo++)
                        {
                            //                    ' Si el empleado en esa fecha tiene contrato
                            if (!roTypes.Any2String(DBArrayInfo[IndexInfo][2]).Equals(String.Empty))
                            {
                                //                       'dsDetailed.AddNew
                                //                     'dsDetailed.Collect("IDReportTask") = IDReportTask
                                strValues = roTypes.Any2String(IDReportTask);
                                strFields = "IDReportTask";

                                //                        'dsDetailed.Collect("IDGroup") = DBArrayEmployees(1, IndexEmployees)
                                strValues = strValues + "," + DBArrayEmployees[IndexEmployees][1];
                                strFields = strFields + ",IDGroup";

                                //                        'dsDetailed.Collect("IDEmployee") = DBArrayEmployees(0, IndexEmployees)
                                strValues = strValues + "," + DBArrayEmployees[IndexEmployees][0];
                                strFields = strFields + ",IDEmployee";

                                //                        'dsDetailed.Collect("DatePlanification") = DBArrayInfo(0, IndexInfo)
                                strValues = strValues + "," + roTypes.Any2Time(DBArrayInfo[IndexInfo][0]).SQLSmallDateTime();
                                strFields = strFields + ",DatePlanification";

                                //                        'dsDetailed.Collect("GroupName") = DBArrayEmployees(3, IndexEmployees)
                                strValues = strValues + ",'" + DBArrayEmployees[IndexEmployees][3].ToString().Replace("'", "''") + "' ";
                                strFields = strFields + ",GroupName";

                                //                        'dsDetailed.Collect("EmployeeName") = DBArrayEmployees(2, IndexEmployees) & " " & Any2String(DBArrayEmployees(6, IndexEmployees))
                                strValues = strValues + ",'" + (DBArrayEmployees[IndexEmployees][2] + " " + roTypes.Any2String(DBArrayEmployees[IndexEmployees][6])).Replace("'", "''") + "'";
                                strFields = strFields + ",EmployeeName";

                                //                        'OBTENER SALDOS
                                if (ConceptsEnabled == true)
                                {
                                    IndexConcept = 1;

                                    ConceptValues = GetAccrualsFromList(roTypes.Any2Integer(DBArrayEmployees[IndexEmployees][0]), strConceptList, DBArrayInfo[IndexInfo][0].ToString(), DBArrayInfo[IndexInfo][0].ToString());

                                    for (IndexConcepts = 0; IndexConcepts < DBArrayConcepts.Length; IndexConcepts++)
                                    {
                                        //                                'dsDetailed.Collect("IDConcept" & IndexConcept) = DBArrayConcepts(0, IndexConcepts)

                                        strValues = strValues + "," + DBArrayConcepts[IndexConcepts][0];
                                        strFields = strFields + "," + "IDConcept" + IndexConcept;

                                        //                                'dsDetailed.Collect("ConceptName" & IndexConcept) = DBArrayConcepts(1, IndexConcepts)
                                        strValues = strValues + ",'" + DBArrayConcepts[IndexConcepts][1].ToString().Replace("'", "''") + "'";
                                        strFields = strFields + "," + "ConceptName" + IndexConcept;

                                        //                                'dsDetailed.Collect("FormatConcept" & IndexConcept) = DBArrayConcepts(2, IndexConcepts)
                                        strValues = strValues + ",'" + DBArrayConcepts[IndexConcepts][2].ToString().Replace("'", "''") + "'";
                                        strFields = strFields + "," + "FormatConcept" + IndexConcept;

                                        //                                'dsDetailed.Collect("ValueConcept" & IndexConcept) = ConceptValues(IndexConcept - 1)
                                        strValues = strValues + "," + ConceptValues[IndexConcept - 1].ToString().Replace(",", ".");
                                        strFields = strFields + "," + "ValueConcept" + IndexConcept;

                                        IndexConcept = IndexConcept + 1;
                                        //                                DoEvents
                                    }
                                }

                                //                        'OBTENER PUNCHES

                                sSQL = "@SELECT# TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                       "IDEmployee = " + DBArrayEmployees[IndexEmployees][0] + " AND ShiftDate= " + roTypes.Any2Time(DBArrayInfo[IndexInfo][0]).SQLDateTime() + " AND ActualType = 1 " +
                                       "UNION ALL " +
                                       "@SELECT# TOP 4 RANK() OVER (ORDER BY DateTime) AS Rank, ActualType, DateTime from Punches WHERE " +
                                       "IDEmployee = " + DBArrayEmployees[IndexEmployees][0] + " AND ShiftDate= " + roTypes.Any2Time(DBArrayInfo[IndexInfo][0]).SQLDateTime() + " AND ActualType = 2 " +
                                       "ORDER BY ActualType, DateTime";

                                dsPunches = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (dsPunches != null)
                                {
                                    DBArrayPunches = dsPunches.AsEnumerable().ToArray();
                                    dsPunches.Dispose();
                                }

                                for (IndexPunches = 0; IndexPunches < DBArrayPunches.Length; IndexPunches++)
                                {
                                    if (DBArrayPunches[IndexPunches][2] != null)
                                    {
                                        if (roTypes.Any2String(DBArrayPunches[IndexPunches][1]) == "1")
                                        {
                                            //                                    'dsDetailed.Collect("PuncheIn" & DBArrayPunches(0, IndexPunches)) = DBArrayPunches(2, IndexPunches)
                                            strValues = strValues + "," + roTypes.Any2Time(DBArrayPunches[IndexPunches][2]).SQLSmallDateTime();
                                            strFields = strFields + "," + "PuncheIn" + DBArrayPunches[IndexPunches][0];
                                        }
                                        else
                                        {
                                            //                                    'dsDetailed.Collect("PuncheOut" & DBArrayPunches(0, IndexPunches)) = DBArrayPunches(2, IndexPunches)
                                            strValues = strValues + "," + roTypes.Any2Time(DBArrayPunches[IndexPunches][2]).SQLSmallDateTime();
                                            strFields = strFields + "," + "PuncheOut" + DBArrayPunches[IndexPunches][0];
                                        }
                                    }
                                    //                            DoEvents
                                }

                                //                        'OBTENER HORARIO
                                if (DBArrayInfo[IndexInfo][3] != null)
                                {
                                    //                            'dsDetailed.Collect("IDShift") = DBArrayInfo(3, IndexInfo)
                                    strValues = strValues + "," + DBArrayInfo[IndexInfo][3];
                                    strFields = strFields + "," + "IDShift";
                                }

                                //                        'INCIDENCIA NO JUSTIFICADA
                                if (DBArrayInfo[IndexInfo][4] != null)
                                {
                                    //dsDetailed["Cause"] = 1;
                                    strValues = strValues + ",1";
                                    strFields = strFields + "," + "Cause";
                                }
                                strInsert = "@insert# into TMPDetailedCalendarEmployee ( " + strFields + ") Values (";
                                strInsert = strInsert + strValues + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(strInsert);

                                //                        'dsDetailed.Update
                            }
                            //                    DoEvents
                        }
                    }
                }

                //    'End If

                //    CreateTmpDetailedCalendarEmployeeEx2 = True
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
            }

            return true;
        }

        private double[] GetAccrualsFromList(int IDEmployee, string strConceptList, string BeginDate, string FinishDate)
        {
            string sSQL;
            DataTable dsConcepts = new DataTable();
            int i;
            string[] arrConceptList;
            double[] ConceptValues = new double[] { };
            int MaxIndex;

            DataRow[] DBArrayConcepts = new DataRow[] { };
            int IndexConcepts;

            try
            {
                arrConceptList = strConceptList.Split(',');
                MaxIndex = arrConceptList.Length;
                Array.Resize(ref ConceptValues, MaxIndex);

                for (i = 0; i < MaxIndex; i++)
                {
                    ConceptValues[i] = 0;
                }

                if (FinishDate == BeginDate)
                {
                    FinishDate = roTypes.Any2Time(BeginDate).Add(1, "d").Substract(1, "s").Value.ToString();
                }

                sSQL = "@SELECT# IdConcept, SUM(value) AS TotalValue from DailyAccruals WHERE IDEmployee = " + roTypes.Any2String(IDEmployee) + " AND " +
                       "IDConcept IN (" + strConceptList + ") AND " +
                       "Date BETWEEN " + roTypes.Any2Time(BeginDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(FinishDate).SQLSmallDateTime() + " GROUP BY IdConcept";

                dsConcepts = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (dsConcepts != null)
                {
                    //dsConcepts.Rows.CopyTo(DBArrayConcepts, 0);
                    DBArrayConcepts = dsConcepts.AsEnumerable().ToArray();
                    dsConcepts.Dispose();
                }

                for (IndexConcepts = 0; IndexConcepts < DBArrayConcepts.Length; IndexConcepts++)
                {
                    for (i = 0; i < MaxIndex; i++)
                    {
                        if (roTypes.Any2String(DBArrayConcepts[IndexConcepts][0]) == arrConceptList[i])
                        {
                            ConceptValues[i] = Double.Parse(DBArrayConcepts[IndexConcepts][1].ToString());
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }

            return ConceptValues;
        }

        private double[] GetAccrualsFromListv2(int IDEmployee, string strConceptList, DateTime BeginDate, DateTime FinishDate, int maxLimit)
        {
            string sSQL;
            DataTable dsConcepts = new DataTable();
            int i;
            string[] arrConceptList;
            double[] ConceptValues = new double[] { };
            int MaxIndex;

            DataRow[] DBArrayConcepts = new DataRow[] { };
            int IndexConcepts;

            try
            {
                arrConceptList = strConceptList.Split(',');
                MaxIndex = arrConceptList.Length;
                if (MaxIndex > maxLimit)
                {
                    MaxIndex = maxLimit;
                }

                Array.Resize(ref ConceptValues, MaxIndex);

                for (i = 0; i < MaxIndex; i++)
                {
                    ConceptValues[i] = 0;
                }

                sSQL = "@SELECT# IdConcept, SUM(value) AS TotalValue from DailyAccruals WHERE IDEmployee = " + roTypes.Any2String(IDEmployee) + " AND " +
"IDConcept IN (" + strConceptList + ") AND " +
"Date BETWEEN " + roTypes.Any2Time(BeginDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(FinishDate).SQLSmallDateTime() + " GROUP BY IdConcept";

                dsConcepts = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::GetAccrualsFromListv2:: " + sSQL);

                if (dsConcepts != null)
                {
                    //dsConcepts.Rows.CopyTo(DBArrayConcepts, 0);
                    DBArrayConcepts = dsConcepts.AsEnumerable().ToArray();
                    dsConcepts.Dispose();
                }

                for (IndexConcepts = 0; IndexConcepts < DBArrayConcepts.Length; IndexConcepts++)
                {
                    for (i = 0; i < MaxIndex; i++)
                    {
                        if (roTypes.Any2String(DBArrayConcepts[IndexConcepts][0]) == arrConceptList[i])
                        {
                            ConceptValues[i] = roTypes.Any2Double(DBArrayConcepts[IndexConcepts][1]);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::GetAccrualsFromListv2::", ex);
            }
            finally
            {
            }

            return ConceptValues;
        }        


        private bool CreateTmpEmergencyEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            //'Rellenamos la tabla temporal llamada TMPEmergency solo con accesos
            //ByVal Employees, ByVal IDReportTask As Integer
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string employees = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            if (!employees.Equals(String.Empty)) employees = employees.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");

            string Value;
            int IDEmployee;
            string sSQL;
            DataTable ads;
            string UsrField;
            long MaxTime;
            bool IsLive;

            try
            {
                var oServerLicense = new roServerLicense();
                string versionCode = "";

                if (oServerLicense.FeatureIsInstalled("Version\\LiveExpress"))
                {
                    versionCode = "EXPRESS";
                }
                else
                {
                    if (oServerLicense.FeatureIsInstalled("Feature\\ONE"))
                    {
                        versionCode = "ONE";
                    }
                    else
                    {
                        versionCode = "LIVE";
                    }
                }

                IsLive = versionCode.Equals("LIVE");

                //'MTH: No borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPEmergency WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //'Recogemos el par�metro de tiempo m�ximo
                MaxTime = GetEmergencyTimeInMinutesEx(2880);

                //'Recogemos el campo que contiene el nombre del campo de la ficha para agrupar
                UsrField = GetValueFromSysroParameters("roParEvacuationPointUsrField");

                if (UsrField != null && !UsrField.Equals(String.Empty))
                {
                    //    'Lanzamos la consulta para obtener el empleado y el valor de ese campo de la ficha
                    sSQL = "@SELECT# ID, ISNULL([USR_" + UsrField + "],'') AS UserField FROM Employees";
                }
                else
                {
                    sSQL = "@SELECT# ID, '' AS UserField FROM Employees";
                }

                if (!IsLive)
                {
                    sSQL = sSQL + " INNER JOIN sysrovwAllEmployeeGroups ON sysrovwAllEmployeeGroups.IDEmployee = Employees.ID";
                    sSQL = sSQL + " WHERE " + employees + " AND EXISTS (@SELECT# * FROM AccessMoves";
                    sSQL = sSQL + " WHERE AccessMoves.IDEmployee = Employees.ID AND DateTime >= DateAdd(n, -" + MaxTime + ", GETDATE())";
                }
                else
                {
                    sSQL = sSQL + " INNER JOIN sysrovwAllEmployeeGroups ON sysrovwAllEmployeeGroups.IDEmployee = Employees.ID";
                    sSQL = sSQL + " WHERE " + employees + " AND EXISTS(@SELECT# * FROM Punches";
                    sSQL = sSQL + " WHERE Punches.IDEmployee = Employees.ID AND DateTime >= DateAdd(n, -" + MaxTime + ", GETDATE()) AND Type IN(5,7))";
                }

                //LogDailyMessage roDebug, "Visits::CreateTmpEmergency:SQL: (" & sSQL & ")"

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //        'Recogemos los datos
                        IDEmployee = roTypes.Any2Integer(adsDataRow["ID"]);
                        Value = roTypes.Any2String(adsDataRow["UserField"]);

                        //        'Creamos el registro                                  //Aqui falta State
                        sSQL = "@INSERT# INTO TMPEmergency([IDEmployee],[UserFieldValue],[IDReportTask]) VALUES (" + IDEmployee + ",'" + Value + "'" + "," + IDReportTask + ")";

                        //        'Ejecutamos la actualizaci�n de la tabla
                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                    }

                    ads.Dispose();
                }

                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        private long GetEmergencyTimeInMinutesEx(long DefaultMinutes)
        {
            long response = 0;
            try
            {
                string strValue;
                strValue = GetValueFromSysroParameters("roParEmergencyMaxHours");
                if (strValue != null)
                {
                    response = roTypes.Any2Time(GetValueFromSysroParameters("roParEmergencyMaxHours")).Minutes();
                    if (response <= 0)
                    {
                        response = DefaultMinutes;
                    }
                    else
                    {
                        response = DefaultMinutes;
                    }
                }
            }
            catch (Exception)
            {
                response = DefaultMinutes;
            }

            return response;
        }

        private string GetValueFromSysroParameters(string Parameter)
        {
            string res;
            string sSQL;
            object sXML;
            roCollection Result = new roCollection();

            try
            {
                sSQL = "@select# Data from sysroparameters where ID='OPTIONS'";
                sXML = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL));
                Result.LoadXMLString(sXML.ToString().Trim());

                if (Result[Parameter] != null)
                {
                    res = Result[Parameter].ToString();
                }
                else
                {
                    res = "";
                }
            }
            catch (Exception)
            {
                res = "";
            }
            finally
            {
            }

            return res;
        }

        private bool CreateTmpWhoShouldBePresentNowEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);
            groups = groups.Replace("sysroEmployeeGroups", "sysrovwAllEmployeeGroups");

            string sSQL;
            DataTable ads;
            DataTable bds;
            int IDEmployee;
            bool ExistsRecord;

            try
            {
                //'Eliminamos los posibles datos que puedan haber
                //'MTH: No borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPWhoShouldComeAndInst WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //'Realizamos la consulta para buscar a todos los empleados afectados

                //' Empleados ausentes y tenian que venir por el horario que tieien seleccionado
                sSQL = "@select# sysrovwAllEmployeeGroups.IDEmployee" +
                " FROM sysrovwAllEmployeeGroups" +
                " INNER JOIN DailySchedule ON sysrovwAllEmployeeGroups.IDEmployee = DailySchedule.IDEmployee" +
                " INNER JOIN Shifts ON DailySchedule.IDShift1 = Shifts.ID" +
                " WHERE DailySchedule.Date = " + roTypes.Any2Time(roTypes.Any2Time(DateTime.Now).DateOnly()).SQLSmallDateTime() +
                " AND sysrovwAllEmployeeGroups.CurrentEmployee = 1 AND Shifts.ExpectedWorkingHours > 0" +
                " AND sysrovwAllEmployeeGroups.IDEmployee IN (@select# distinct idemployee from EmployeeStatus where IsPresent = 0)" +
                " AND " + groups;

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //'Recuperamos al empleado
                        ExistsRecord = false;
                        IDEmployee = roTypes.Any2Integer(adsDataRow["IDEmployee"]);

                        //'0.- DETERMINAMOS A QUÉ DIA PERTENECE EL EMPLEADO AHORA y SI TIENE QUE VENIR
                        DateTime DateWork;
                        bool MustWork;
                        DateWork = roTypes.Any2Time(DateTime.Now).ValueDateTime;
                        MustWork = EmployeeMustWorkingAtDay(IDEmployee, DateWork, DateTime.Now);
                        if (MustWork == false)
                        {
                            DateWork = DateAndTime.DateAdd("d", -1, DateWork);
                            MustWork = EmployeeMustWorkingAtDay(IDEmployee, DateWork, DateTime.Now);
                        }

                        if (MustWork)
                        {
                            //'1.- COMPROBAMOS SI TIENE UNA AUSENCIA PROLONGADA
                            sSQL = "@SELECT# S.Name Horario, S.StartLimit Inicio, C.Name Justificacion FROM ProgrammedAbsences PA " +
                                   "INNER JOIN Causes C ON C.ID = PA.IDCause INNER JOIN DailySchedule DS ON DS.IDEmployee = PA.IDEmployee " +
                                   "AND DS.Date = DATEADD(dd,0,datediff(dd,0," + roTypes.Any2Time(roTypes.Any2Time(DateWork).DateOnly()).SQLSmallDateTime() + ")) INNER JOIN Shifts S ON S.ID = DS.IDShift1 WHERE " +
                                   "DATEADD(dd,0,datediff(dd,0," + roTypes.Any2Time(roTypes.Any2Time(DateWork).DateOnly()).SQLSmallDateTime() + ")) BETWEEN BeginDate AND ISNULL(FinishDate,dateadd(d,PA.maxlastingdays-1,BeginDate)) " +
                                   "AND PA.IDEmployee = " + IDEmployee;

                            bds = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                            if (bds != null)
                            {
                                foreach (DataRow bdsDataRow in bds.Rows)
                                {
                                    ExistsRecord = true;

                                    sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, idReportTask) VALUES (" + IDEmployee + ",'";
                                    sSQL = sSQL + Strings.Replace(roTypes.Any2String(bdsDataRow["Horario"]), "'", "''", 1) + "','" + roTypes.Any2String(bdsDataRow["Inicio"]) + "','" + roTypes.Any2String(bdsDataRow["Justificacion"]) + "', 'A', NULL," + IDReportTask + ")";
                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }

                            bds.Dispose();

                            //'Solo en el caso que no hayamos introducido antes sus datos
                            if (!ExistsRecord)
                            {
                                //'2.- COMPROBAMOS SI TIENE UNA INCIDENCIA PREVISTA
                                sSQL = "@SELECT# S.Name Horario, S.StartLimit Inicio, C.Name Justificacion, Duration FROM ProgrammedCauses PC " +
                                       "INNER JOIN Causes C ON C.ID = PC.IDCause INNER JOIN DailySchedule DS ON DS.IDEmployee = PC.IDEmployee AND " +
                                       "DS.Date = DATEADD(dd,0,datediff(dd,0," + roTypes.Any2Time(roTypes.Any2Time(DateWork).DateOnly()).SQLSmallDateTime() + ")) INNER JOIN Shifts S ON S.ID = DS.IDShift1 WHERE " +
                                       "PC.Date = DateAdd(dd, 0, DateDiff(dd, 0, " + roTypes.Any2Time(roTypes.Any2Time(DateWork).DateOnly()).SQLSmallDateTime() + ")) And PC.IDEmployee = " + IDEmployee;

                                bds = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bds != null)
                                {
                                    foreach (DataRow bdsDataRow in bds.Rows)
                                    {
                                        ExistsRecord = true;

                                        sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, idReportTask) VALUES (" + IDEmployee + ",'";
                                        sSQL = sSQL + Strings.Replace(roTypes.Any2String(bdsDataRow["Horario"]), "'", "''", 1) + "','" + roTypes.Any2String(bdsDataRow["Inicio"]) + "','" + roTypes.Any2String(bdsDataRow["Justificacion"]) + "', 'I','" + roTypes.Any2String(bdsDataRow["Duration"]) + ", " + IDReportTask + "')";
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                    }
                                }

                                bds.Dispose();

                                //'Solo sino hemos insertado ya al empleado
                                if (!ExistsRecord)
                                {
                                    //'3.- COMPROBAMOS SI TIENE QUE VENIR A TRABAJAR
                                    sSQL = "@SELECT# VW.GroupName, VW.IDEmployee, VW.EmployeeName, S.Name Horario, ES.Type, ES.LastPunch, ES.IDCause, C.Name as CauseName, CASE WHEN IsFloating =1 then CONVERT(VARCHAR,DS.StartShiftUsed, 108) ELSE CONVERT(VARCHAR,s.StartLimit, 108) END AS Inicio" +
                                    " FROM DailySchedule DS" +
                                    " INNER JOIN sysrovwAllEmployeeGroups VW ON VW.IDEmployee = DS.IDEmployee" +
                                    " INNER JOIN Shifts S on DS.IDShift1 = S.ID" +
                                    " INNER JOIN EmployeeStatus ES ON ES.IDEmployee = DS.IDEmployee" +
                                    " INNER JOIN Causes C ON C.ID = ES.IDCause" +
                                    " Where ES.IsPresent = 0" +
                                    " and ES.idEmployee=" + IDEmployee +
                                    " and DS.Date=CONVERT(SMALLDATETIME,'" + roTypes.Any2Time(DateWork).ValueDateTime.ToString("yyyy/MM/dd") + "',120)";

                                    String Cause = "";
                                    String AbsenceType = "";
                                    Cause = "''";

                                    bds = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                    if (bds != null)
                                    {
                                        foreach (DataRow bdsDataRow in bds.Rows)
                                        {
                                            bool IsRigidLayer;

                                            //' Comprueba si está en una franja rigida ahora
                                            IsRigidLayer = EmployeeIsInRigidLayer(IDEmployee, DateTime.Now.ToString("yyyy/MM/dd"), DateTime.Now.ToString("HH:mm"));

                                            if (IsRigidLayer)
                                            {
                                                ExistsRecord = true;

                                                if (bdsDataRow["Type"].ToString() == "S")
                                                {
                                                    if (!bdsDataRow["IDCause"].ToString().Equals("0"))
                                                    {
                                                        AbsenceType = "F";
                                                        Cause = "'" + bdsDataRow["CauseName"] + "'";
                                                    }
                                                    else
                                                    {
                                                        if (bdsDataRow["LastPunch"] != null)
                                                        {
                                                            AbsenceType = "S";
                                                            Cause = "'" + roTypes.Any2DateTime(bdsDataRow["LastPunch"]).ToString("dd/MM/yyyy HH:mm") + "'";
                                                        }
                                                        else
                                                        {
                                                            AbsenceType = "E";
                                                            Cause = "NULL";
                                                        }
                                                    }
                                                }

                                                sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, idReportTask) VALUES (" + IDEmployee + ",'";
                                                sSQL = sSQL + Strings.Replace(roTypes.Any2String(bdsDataRow["Horario"]), "'", "''", 1) + "','" + roTypes.Any2String(bdsDataRow["Inicio"]) + "'," + Cause + ", '" + AbsenceType + "', NULL, " + IDReportTask + ")";
                                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                            }
                                        }
                                    }

                                    bds.Dispose();
                                }
                            }
                        }
                    }
                }

                //'Si llegamos aqui es que todo a ido bien
                return true;
            }
            catch (Exception)
            {
            }
            finally
            {
            }

            return false;
        }

        private bool CreateTmpAccrualsByContract(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            string[] empFilter = originalParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value.ToString().Split('@');
            string groups = roSelectorManager.GetWhereWithoutPermissions(empFilter[0], empFilter[1], empFilter[2]);

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }

            DateTime StartDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).Start;
            DateTime EndDate = ((DevExpress.XtraReports.Parameters.Range<System.DateTime>)parametersList.Where(x => x.Type.Equals("System.DateTime")).FirstOrDefault().Value).End;
            int concepts1 = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept1")).FirstOrDefault().Value.ToString());
            int concepts2 = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept2")).FirstOrDefault().Value.ToString());
            int concepts3 = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept3")).FirstOrDefault().Value.ToString());
            int concepts4 = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept4")).FirstOrDefault().Value.ToString());
            int concepts5 = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept5")).FirstOrDefault().Value.ToString());
            int concepts6 = 0; // int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept6")).FirstOrDefault().Value.ToString());
            int concepts7 = 0; // int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept7")).FirstOrDefault().Value.ToString());
            int concepts8 = 0; //int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept8")).FirstOrDefault().Value.ToString());
            int concepts9 = 0; //int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.conceptIdentifier")).Where(z => z.Name.Equals("concept9")).FirstOrDefault().Value.ToString());

            string idconcepts = concepts1 + "," + concepts2 + "," + concepts3 + "," + concepts4 + "," + concepts5 + "," + concepts6 + "," + concepts7 + "," + concepts8 + "," + concepts9;

            if (!groups.Equals(String.Empty)) groups = groups.Replace("sysroEmployeeGroups", "sysrovwCurrentEmployeeGroups");

            string sSQL;
            DataTable ads;
            DataTable bds;
            int IDEmployee;
            string NumContrato;
            string Monthdate;
            string strValue;
            int IDConcept;

            bool ExistsRecord;

            try
            {
                //Eliminamos los posibles datos que puedan haber
                //'MTH: No borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPAccrualsByContract WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //Realizamos la consulta para obtener los datos
                sSQL = "@SELECT#  sysrovwCurrentEmployeeGroups.IDEmployee,sysroDailyAccrualsByContract.NumContrato,  sysroDailyAccrualsByContract.IDConcept, " +
                " convert(smalldatetime, convert(char(4), datepart(year, sysroDailyAccrualsByContract.Date)) + '/' + replace(convert(char(2), datepart(month, sysroDailyAccrualsByContract.Date)), ' ', '') + '/01', 120) as monthdate, " +
                " SUM(sysroDailyAccrualsByContract.Value) as Value " +
                " FROM sysroDailyAccrualsByContract  WITH(NOLOCK) " +
                " INNER JOIN sysrovwCurrentEmployeeGroups WITH(NOLOCK) ON sysrovwCurrentEmployeeGroups.IDemployee = sysroDailyAccrualsByContract.idemployee " +
                " WHERE (" + groups + ") ";

                if (empSelected != null && empSelected.Length > 0)
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN( " + String.Join(",", empSelected) + ")";
                }
                else
                {
                    sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.IDEmployee IN(-1)";
                }

                sSQL = sSQL + " AND sysrovwCurrentEmployeeGroups.CurrentEmployee = 1" +
                " AND sysroDailyAccrualsByContract.Date >= " + roTypes.Any2Time(StartDate.Date).SQLSmallDateTime() + " AND sysroDailyAccrualsByContract.Date <= " + roTypes.Any2Time(EndDate.Date).SQLSmallDateTime() +
                " AND sysroDailyAccrualsByContract.IDConcept in( " + idconcepts + ")";

                int IDPassport = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

                if (IDPassport > 0)
                {
                    sSQL = sSQL + " AND (@Select# dbo.WebLogin_GetPermissionOverEmployee(" + IDPassport.ToString() + ",sysrovwCurrentEmployeeGroups.IDEmployee,1,0,1,getdate())) > 0  ";
                }

                sSQL = sSQL + " Group by sysrovwCurrentEmployeeGroups.idemployee, NumContrato, IDConcept, convert(smalldatetime, convert(char(4), datepart(year, sysroDailyAccrualsByContract.Date)) + '/' + replace(convert(char(2), datepart(month, sysroDailyAccrualsByContract.Date)), ' ', '') + '/01', 120) ";
                sSQL = sSQL + " Order by idemployee, NumContrato, monthdate, IDConcept";

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        IDEmployee = roTypes.Any2Integer(adsDataRow["IDEmployee"]);
                        NumContrato = roTypes.Any2String(adsDataRow["NumContrato"]).Replace("'", "''");
                        Monthdate = roTypes.Any2Time(adsDataRow["monthdate"]).SQLSmallDateTime();
                        IDConcept = roTypes.Any2Integer(adsDataRow["IDConcept"]);
                        strValue = Strings.Replace(Convert.ToString(roTypes.Any2Double(adsDataRow["Value"])), ",", ".");

                        string strConcept = "";
                        if (concepts1 == IDConcept)
                        {
                            strConcept = "Value1";
                        }
                        if (concepts2 == IDConcept)
                        {
                            strConcept = "Value2";
                        }
                        if (concepts3 == IDConcept)
                        {
                            strConcept = "Value3";
                        }
                        if (concepts4 == IDConcept)
                        {
                            strConcept = "Value4";
                        }
                        if (concepts5 == IDConcept)
                        {
                            strConcept = "Value5";
                        }
                        if (concepts6 == IDConcept)
                        {
                            strConcept = "Value6";
                        }
                        if (concepts7 == IDConcept)
                        {
                            strConcept = "Value7";
                        }
                        if (concepts8 == IDConcept)
                        {
                            strConcept = "Value8";
                        }
                        if (concepts9 == IDConcept)
                        {
                            strConcept = "Value9";
                        }

                        // Verificamos si ya existe un registro para el empleado/contrato/mes
                        // si no existe lo creamos

                        if (strConcept.Length > 0)
                        {
                            sSQL = "@SELECT# * FROM TMPAccrualsByContract WHERE IDEmployee=" + IDEmployee + " AND IDContract like '" + NumContrato + "' ";
                            sSQL = sSQL + " AND ConceptDate = " + Monthdate;
                            sSQL = sSQL + " AND IDReportTask = " + IDReportTask;
                            bds = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);
                            ExistsRecord = false;
                            if (bds != null)
                            {
                                if (bds.Rows.Count > 0)
                                {
                                    ExistsRecord = true;
                                }
                            }
                            bds.Dispose();

                            if (ExistsRecord)
                            {
                                // Actualizamos el registro con el valor del saldo
                                sSQL = "@UPDATE# TMPAccrualsByContract SET " + strConcept + "= " + strValue + " WHERE IDEmployee=" + IDEmployee;
                                sSQL = sSQL + " AND IDContract like '" + NumContrato + "' AND ConceptDate =" + Monthdate + "  AND idReportTask=" + IDReportTask;
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                            else
                            {
                                // Creamos un registro con el valor del saldo
                                sSQL = "@INSERT# INTO TMPAccrualsByContract (IDEmployee, IDContract, ConceptDate, " + strConcept + ", idReportTask) VALUES (" + IDEmployee + ",'";
                                sSQL = sSQL + NumContrato + "'," + Monthdate + " ," + strValue + "," + IDReportTask + ")";
                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                        }
                    }
                }

                //'Si llegamos aqui es que todo a ido bien
                return true;
            }
            catch (Exception)
            {
            }
            finally
            {
            }

            return false;
        }

        private bool EmployeeIsInRigidLayer(int IDEmployee, string ShiftDate, String IniTime)
        {
            //'Obtenemos si algun empleado tiene franjas rígidas entre dos horas

            string sSQL;
            string sTmp;
            DataTable Layers = new DataTable();
            DataTable ads = new DataTable();
            roCollection mCollection = new roCollection();
            roCollection mParentCollection = new roCollection();
            DateTime BeginTime;
            DateTime BeginTimeValue;
            DateTime FinishTimeValue;
            bool bolOK = false;
            bool EmployeeIsInRigidLayer = false;

            try
            {
                //' Busca el horario del empleado en el dia solicitado
                sSQL = "@SELECT# idShift1 FROM DailySchedule WHERE Date = " + roTypes.Any2Time(ShiftDate).SQLSmallDateTime() + " AND IDEmployee = " + IDEmployee;

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        bolOK = !(adsDataRow["idShift1"] == null);
                        if (bolOK)
                        {
                            //' Busca las franjas rigidas del horario
                            BeginTime = roTypes.Any2Time(IniTime).ValueDateTime; //TIMEONLY

                            Layers = Robotics.DataLayer.AccessHelper.CreateDataTable("@SELECT# * FROM sysroShiftsLayers WHERE IDSHIFT = " + adsDataRow["IDShift1"] + " AND IDType = 1100 ORDER BY ID");

                            foreach (DataRow layerRow in Layers.Rows)
                            {
                                mCollection.LoadXMLString(layerRow["Definition"].ToString());
                                sTmp = "@SELECT# [Definition] from sysroShiftsLayers where IDShift = " + adsDataRow["IDShift1"];
                                sTmp = sTmp + " and ID=" + layerRow["ID"];
                                sTmp = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sTmp));

                                if (sTmp.Length > 0)
                                {
                                    mParentCollection.LoadXMLString(sTmp);
                                    BeginTimeValue = roTypes.Any2Time(mCollection["Begin"]).ValueDateTime; //TIMEONLY
                                    FinishTimeValue = roTypes.Any2Time(mCollection["Finish"]).ValueDateTime; //TIMEONLY

                                    if (BeginTimeValue < FinishTimeValue)
                                    {
                                        //' Franja mismo dia
                                        if (BeginTime >= BeginTimeValue && BeginTime <= FinishTimeValue)
                                        {
                                            EmployeeIsInRigidLayer = true;
                                            ads.Dispose();
                                            return EmployeeIsInRigidLayer;
                                        }
                                        else
                                        {
                                            //' Franja dos días
                                            if (BeginTime >= BeginTimeValue || BeginTime <= FinishTimeValue)
                                            {
                                                EmployeeIsInRigidLayer = true;
                                                ads.Dispose();
                                                return EmployeeIsInRigidLayer;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    mParentCollection.Clear();
                                }
                            }

                            mCollection.Clear();
                            mParentCollection.Clear();
                        }

                        Layers.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                EmployeeIsInRigidLayer = true;
            }
            finally
            {
            }

            return EmployeeIsInRigidLayer;
        }

        private bool EmployeeMustWorkingAtDay(int IDEmployee, DateTime mDay, DateTime mDateTime)
        {
            //'Obtenemos si algun empleado tiene franjas rígidas entre dos horas

            String sSQL = "";
            DataTable Shift;
            bool MustWorking = false;

            try
            {
                DateTime BeginTimeValue;
                DateTime EndTimeValue;
                DateTime day;
                DateTime time;

                day = Convert.ToDateTime(roTypes.Any2Time(mDay).DateOnly());
                time = Convert.ToDateTime(roTypes.Any2Time(mDateTime).TimeOnly());

                MustWorking = false;

                //' Busca el horario del empleado en el dia solicitado
                sSQL = "@SELECT# dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.Shifts.ID, dbo.Shifts.StartLimit, dbo.Shifts.EndLimit " +
                       " FROM dbo.DailySchedule INNER JOIN dbo.Shifts ON dbo.DailySchedule.IDShift1 = dbo.Shifts.ID " +
                       " WHERE Date = '" + roTypes.Any2Time(mDay).DateOnly() + "' AND IDEmployee = " + IDEmployee;

                Shift = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (Shift != null)
                {
                    foreach (DataRow shiftDataRow in Shift.Rows)
                    {
                        //' Determina las fechas de inicio y final
                        BeginTimeValue = Convert.ToDateTime(day.ToString("dd/MM/yyyy") + " " + roTypes.Any2Time(shiftDataRow["StartLimit"]).ValueDateTime.ToString("hh:mm:ss"));
                        EndTimeValue = Convert.ToDateTime(day.ToString("dd/MM/yyyy") + " " + roTypes.Any2Time(shiftDataRow["EndLimit"]).ValueDateTime.ToString("HH:mm:ss"));

                        //' Horario nocturno
                        if (DateAndTime.DateDiff("d", shiftDataRow["StartLimit"], shiftDataRow["EndLimit"]) == 1)
                        {
                            EndTimeValue = DateAndTime.DateAdd("d", 1, EndTimeValue);
                        }

                        MustWorking = (mDateTime >= BeginTimeValue && mDateTime <= EndTimeValue);
                    }
                }

                Shift.Dispose();
            }
            catch (Exception)
            { }
            finally
            {
            }

            return MustWorking;
        }

        //Se usa!
        private bool CreateTmpWhoShouldComeAndInst(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            int passportId = int.Parse(parametersList.Where(x => x.Type.Equals("Robotics.Base.passportIdentifier")).FirstOrDefault().Value.ToString());

            object stremployee = parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault();
            string[] empSelected = null;
            if (stremployee != null)
            {
                empSelected = Array.ConvertAll((object[])parametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector")).FirstOrDefault().Value, Convert.ToString);
            }
            if (empSelected.Length == 0) empSelected = new string[] { "-1" };

            string sSQL;
            DataTable ads;
            DataTable bDS;
            double IDEmployee;
            bool ExistsRecord;

            try
            {
                //Eliminamos los posibles datos que puedan haber
                //MTH: No borramos datos para las impresiones multihilo
                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPWhoShouldComeAndInst WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");


                string empFilter = string.Join(",", empSelected);

                //Realizamos la consulta para buscar a todos los empleados afectados

                //Empleados ausentes y tenian que venir por el horario que tieien seleccionado

                sSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee" +
                " FROM sysrovwAllEmployeeGroups";
                sSQL += " INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe WITH (NOLOCK) ON poe.IdPassport = " + passportId +
                    " AND poe.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee And convert(date, getdate()) between poe.BeginDate And poe.EndDate " +
                    " AND poe.IDEmployee IN (" + empFilter + ") and poe.IdFeature = 1 and poe.FeaturePermission > 1 ";
                sSQL += " INNER JOIN DailySchedule ON sysrovwAllEmployeeGroups.IDEmployee = DailySchedule.IDEmployee" +
                " INNER JOIN Shifts ON DailySchedule.IDShift1 = Shifts.ID" +
                " WHERE DailySchedule.Date = " + roTypes.Any2Time(roTypes.Any2Time(DateTime.Now).DateOnly()).SQLSmallDateTime() +
                " AND sysrovwAllEmployeeGroups.CurrentEmployee = 1 AND Shifts.ExpectedWorkingHours > 0" +
                " AND sysrovwAllEmployeeGroups.IDEmployee IN (@SELECT# distinct idemployee from EmployeeStatus where IsPresent = 0 and BeginMandatory < " + roTypes.Any2Time(DateTime.Now).SQLSmallDateTime() + ")" +
                " AND DailySchedule.IDEmployee NOT IN (@SELECT# IDEmployee FROM sysrovwHoursAbsences HA where cast(HA.Date as date) = cast(GETDATE() as date) and cast(GETDATE() as time) between cast(HA.BeginTime as time) and cast(HA.EndTime as time))";

                var oServerLicense = new roServerLicense();
                bool dailyRecordInstalled = oServerLicense.FeatureIsInstalled("Feature\\DailyRecord");
                string sqlDailyRecordWhere = "";
                if (dailyRecordInstalled)
                {
                    sqlDailyRecordWhere = " AND DailySchedule.IDEmployee NOT IN (@SELECT# IdEmployee FROM sysrovwSecurity_PermissionOverFeatures WHERE FeatureAlias = 'Punches.DailyRecord' And FeatureType = 'E') ";
                }
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable($"{sSQL} {sqlDailyRecordWhere} ");

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        //Recuperamos al empleado
                        ExistsRecord = false;
                        IDEmployee = roTypes.Any2Double(adsDataRow["IDEmployee"]);

                        //'1.- COMPROBAMOS SI TIENE UNA AUSENCIA PROLONGADA
                        //'sSQL = "SELECT S.Name Horario, S.StartLimit Inicio, C.Name Justificacion FROM ProgrammedAbsences PA"
                        //'sSQL = sSQL & " INNER JOIN Causes C ON C.ID = PA.IDCause"
                        //'sSQL = sSQL & " INNER JOIN DailySchedule DS ON DS.IDEmployee = PA.IDEmployee"
                        //'sSQL = sSQL & " INNER JOIN Shifts S ON S.ID = DS.IDShift1"
                        //'sSQL = sSQL & " WHERE convert(SMALLDATETIME,GETDATE(),103) BETWEEN BeginDate AND ISNULL(FinishDate,dateadd(d,maxlastingdays-1,BeginDate))"
                        //'sSQL = sSQL & " AND PA.IDEmployee = " & IDEmployee
                        //'sSQL = sSQL & " AND CONVERT(VarChar, DS.Date, 103) = CONVERT(VarChar,GETDATE(),103)"

                        sSQL = "@SELECT# S.Name Horario, S.StartLimit Inicio, C.Name Justificacion FROM ProgrammedAbsences PA " +
                               "INNER JOIN Causes C ON C.ID = PA.IDCause INNER JOIN DailySchedule DS ON DS.IDEmployee = PA.IDEmployee " +
                               "AND DS.Date = DATEADD(dd,0,datediff(dd,0,getdate())) INNER JOIN Shifts S ON S.ID = DS.IDShift1 WHERE " +
                               "DATEADD(dd,0,datediff(dd,0,getdate())) BETWEEN BeginDate AND ISNULL(FinishDate,dateadd(d,PA.maxlastingdays-1,BeginDate)) " +
                               "AND PA.IDEmployee = " + IDEmployee;

                        bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                        if (bDS != null)
                        {
                            foreach (DataRow bDSdatarow in bDS.Rows)
                            {
                                ExistsRecord = true;

                                sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, IDReportTask) VALUES (" + IDEmployee + ",'" +
                                roTypes.Any2String(bDSdatarow["Horario"]).Replace("'", "''") + "','" + roTypes.Any2String(bDSdatarow["Inicio"]) + "','" + roTypes.Any2String(bDSdatarow["Justificacion"]) + "', 'A', NULL," + IDReportTask + ")";

                                Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                            }
                        }
                        bDS.Dispose();

                        //Solo en el caso que no hayamos introducido antes sus datos
                        if (!ExistsRecord)
                        {
                            //'2.- COMPROBAMOS SI TIENE UNA INCIDENCIA PREVISTA
                            //'sSQL = "SELECT S.Name Horario, S.StartLimit Inicio, C.Name Justificacion, Duration FROM ProgrammedCauses PC"
                            //'sSQL = sSQL & " INNER JOIN Causes C ON C.ID = PC.IDCause"
                            //'sSQL = sSQL & " INNER JOIN DailySchedule DS ON DS.IDEmployee = PC.IDEmployee"
                            //'sSQL = sSQL & " INNER JOIN Shifts S ON S.ID = DS.IDShift1"
                            //'sSQL = sSQL & " WHERE CONVERT(VarChar,GETDATE(),103) = CONVERT(VarChar,PC.Date,103) AND PC.IDEmployee = " & IDEmployee
                            //'sSQL = sSQL & " AND CONVERT(VarChar, DS.Date, 103) = CONVERT(VarChar,GETDATE(),103)"

                            sSQL = "@SELECT# S.Name Horario, S.StartLimit Inicio, C.Name Justificacion, Duration FROM ProgrammedCauses PC " +
                                   "INNER JOIN Causes C ON C.ID = PC.IDCause INNER JOIN DailySchedule DS ON DS.IDEmployee = PC.IDEmployee AND " +
                                   "DS.Date = DATEADD(dd,0,datediff(dd,0,getdate())) INNER JOIN Shifts S ON S.ID = DS.IDShift1 WHERE " +
                                   "PC.Date = DateAdd(dd, 0, DateDiff(dd, 0, getdate())) And PC.IDEmployee = " + IDEmployee;

                            bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                            if (bDS != null)
                            {
                                foreach (DataRow bDSdatarow in bDS.Rows)
                                {
                                    ExistsRecord = true;

                                    sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, IDReportTask) VALUES (" + IDEmployee + ",'" +
                                    roTypes.Any2String(bDSdatarow["Horario"]).Replace("'", "''") + "','" + roTypes.Any2String(bDSdatarow["Inicio"]) + "','" + roTypes.Any2String(bDSdatarow["Justificacion"]) + "', 'I','" + roTypes.Any2String(bDSdatarow["Duration"]) + "', " + IDReportTask + ")";

                                    Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                }
                            }
                            bDS.Dispose();

                            //Solo si no hemos insertado ya al empleado
                            if (!ExistsRecord)
                            {
                                //3.- COMPROBAMOS SI TIENE QUE VENIR A TRABAJAR
                                sSQL = "@SELECT# VW.GroupName, VW.IDEmployee, VW.EmployeeName, S.Name Horario, CASE WHEN IsFloating =1 then CONVERT(VARCHAR,DS.StartShiftUsed, 108) ELSE CONVERT(VARCHAR,s.StartLimit, 108) END AS Inicio" +
                                " FROM DailySchedule DS" +
                                " INNER JOIN sysrovwAllEmployeeGroups VW ON VW.IDEmployee = DS.IDEmployee" +
                                " INNER JOIN Shifts S on DS.IDShift1 = S.ID" +
                                " WHERE CONVERT(VarChar, DS.Date, 103) = CONVERT(VarChar,GETDATE(),103)" +
                                " AND DS.IDEmployee NOT IN (@SELECT# IDEmployee FROM Punches M WHERE ActualType = 1 AND DATEDIFF(DAY,DateTime,GETDATE())=0 and M.IDEmployee = " + IDEmployee + ")" +
                                " AND DS.IDEmployee NOT IN (@SELECT# IDEmployee FROM ProgrammedAbsences PA WHERE GETDATE() BETWEEN BeginDate AND ISNULL(FinishDate,dateadd(d,maxlastingdays-1,BeginDate)))" +
                                " AND DS.IDEmployee NOT IN (@SELECT# IDEmployee FROM ProgrammedHolidays PH where cast(PH.Date as date) = cast(GETDATE() as date) and (AllDay = 1 or cast(GETDATE() as time) between cast(BeginTime as time) and cast(EndTime as time)))" +
                                " AND DS.IDEmployee NOT IN (@SELECT# IDEmployee FROM sysrovwHoursAbsences HA where cast(HA.Date as date) = cast(GETDATE() as date) and cast(GETDATE() as time) between cast(BeginTime as time) and cast(EndTime as time))" +
                                " AND (CONVERT(SMALLDATETIME,CONVERT(VARCHAR,GETDATE(),108),108) >= CASE WHEN IsFloating = 1 then CONVERT(SMALLDATETIME,CONVERT(VARCHAR,DS.StartShiftUsed,108),108) Else CONVERT(SMALLDATETIME,CONVERT(VARCHAR,S.StartLimit,108),108) End)" +
                                " AND CONVERT(VARCHAR,DS.Date,103) = CONVERT(VARCHAR,GETDATE(),103) AND DS.IDEmployee IN (" + IDEmployee + ")";

                                bDS = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                                if (bDS != null)
                                {
                                    foreach (DataRow bDSdatarow in bDS.Rows)
                                    {
                                        ExistsRecord = true;

                                        sSQL = "@INSERT# INTO TMPWhoShouldComeAndInst (IDEmployee, Shifts, Start, Cause, AbsenceType, Duration, IDReportTask) VALUES (" + IDEmployee + ",'" +
                                        roTypes.Any2String(bDSdatarow["Horario"]).Replace("'", "''") + "','" + roTypes.Any2String(bDSdatarow["Inicio"]) + "',NULL, 'E', NULL," + IDReportTask + ")";
                                        Robotics.DataLayer.AccessHelper.ExecuteSql(sSQL);
                                    }
                                }
                            }
                            bDS.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpWhoShouldComeAndInst::", ex);
                return false;
            }
            finally
            {
            }

            return true;
        }

        private double[] GetAccrualsFromListLear(int IDEmployee, string strConceptList, DateTime BeginDate, DateTime FinishDate, int maxLimit)
        {
            string sSQL;
            DataTable dsConcepts = new DataTable();
            int i;
            string[] arrConceptList;
            double[] ConceptValues = new double[] { };
            int MaxIndex;

            DataRow[] DBArrayConcepts = new DataRow[] { };
            int IndexConcepts;

            try
            {
                arrConceptList = strConceptList.Split(',');
                MaxIndex = arrConceptList.Length;
                if (MaxIndex > maxLimit)
                {
                    MaxIndex = maxLimit;
                }

                Array.Resize(ref ConceptValues, MaxIndex);

                for (i = 0; i < MaxIndex; i++)
                {
                    ConceptValues[i] = 0;
                }

                sSQL = "@SELECT# c.shortname, SUM(value) AS TotalValue from DailyAccruals as da inner join concepts as c on c.id = da.idconcept WHERE IDEmployee = " + roTypes.Any2String(IDEmployee) + " AND " +
"c.shortname IN (" + strConceptList + ") AND " +
"Date BETWEEN " + roTypes.Any2Time(BeginDate).SQLSmallDateTime() + " AND " + roTypes.Any2Time(FinishDate).SQLSmallDateTime() + " GROUP BY shortname";

                dsConcepts = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "TermporalTablesManager::GetAccrualsFromListv2:: " + sSQL);

                if (dsConcepts != null)
                {
                    //dsConcepts.Rows.CopyTo(DBArrayConcepts, 0);
                    DBArrayConcepts = dsConcepts.AsEnumerable().ToArray();
                    dsConcepts.Dispose();
                }

                for (IndexConcepts = 0; IndexConcepts < DBArrayConcepts.Length; IndexConcepts++)
                {
                    for (i = 0; i < MaxIndex; i++)
                    {
                        if ("'" + roTypes.Any2String(DBArrayConcepts[IndexConcepts][0]) + "'" == arrConceptList[i])
                        {
                            ConceptValues[i] = roTypes.Any2Double(DBArrayConcepts[IndexConcepts][1]);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::GetAccrualsFromListv2::", ex);
            }
            finally
            {
            }

            return ConceptValues;
        }


        // Eliminamos tarea de informe
        public bool DeleteTaskReport(long _ID, roReportState _State)
        {
            bool bolRet = false;

            try
            {
                string strTempFileName = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar("@SELECT# ISNULL(UploadFile,'') FROM sysroReportTasks WHERE ID=" + _ID.ToString()));

                // Borramos la tarea del informe
                bolRet = Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# sysroReportTasks WHERE ID=" + _ID.ToString());
                if (bolRet)
                {
                    if (!string.IsNullOrEmpty(strTempFileName))
                    {
                        // Borrar fichero temporal
                        try
                        {
                            File.Delete(strTempFileName);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (DbException)
            {
                //oState.UpdateStateInfo(ex, "roReport:DeleteTaskReport");
            }
            catch (Exception)
            {
                //oState.UpdateStateInfo(ex, "roReport:DeleteTaskReport");
            }
            finally
            {
            }

            return bolRet;
        }

        // Obtenemos el estado de la tarea del informe
        public bool GetStatusTaskReport(long _ID, ref int _Status, ref string _UploadFile, roReportState _State)
        {
            bool bolRet = false;

            try
            {
                string strSQL = "@SELECT# isnull(Status, 0) as Status , isnull(UploadFile, '') as  UploadFile FROM sysroReportTasks WHERE [ID] = " + _ID.ToString();

                var tb = Robotics.DataLayer.AccessHelper.CreateDataTable(strSQL);
                if (tb != null && tb.Rows.Count > 0)
                {
                    _Status = Convert.ToInt32(tb.Rows[0]["Status"]);
                    _UploadFile = tb.Rows[0]["UploadFile"].ToString();
                }
                else
                {
                    _Status = -1;
                    _UploadFile = "";
                }

                bolRet = true;
            }
            catch (DbException ex)
            {
                _State.UpdateStateInfo(ex, "roReport:GetStatusTaskReport");
            }
            catch (Exception ex)
            {
                _State.UpdateStateInfo(ex, "roReport:GetStatusTaskReport");
            }
            finally
            {
            }

            return bolRet;
        }

        // Creamos una tarea para el servidor de informes
        // y retorna el ID de la tarea generada
        public long CreateTaskReport(int _IDPassport, string _ReportName, string _FileName, string _Parameters, string _Culture, int _ExportFormatType, roReportState _State)
        {
            int lngID = -1;

            try
            {
                var tbTask = new DataTable("sysroReportTasks");
                string strSQL = "@SELECT# * FROM sysroReportTasks WHERE [ID] = -1";
                var cmd = Robotics.DataLayer.AccessHelper.CreateCommand(strSQL);
                var da = Robotics.DataLayer.AccessHelper.CreateDataAdapter(cmd, true);
                da.Fill(tbTask);

                DataRow oRow = null;
                oRow = tbTask.NewRow();

                oRow["IDPassport"] = _IDPassport;
                oRow["ReportName"] = _ReportName;
                oRow["FileName"] = _FileName;
                oRow["Status"] = 0;
                oRow["Parameters"] = _Parameters;
                oRow["ExportFormatType"] = _ExportFormatType;
                oRow["Culture"] = _Culture;
                oRow["TimeStamp"] = DateTime.Now;

                tbTask.Rows.Add(oRow);

                da.Update(tbTask);

                var tb = Robotics.DataLayer.AccessHelper.CreateDataTable("@SELECT# TOP 1 [ID] FROM sysroReportTasks " + "ORDER BY [ID] DESC");
                if (tb != null && tb.Rows.Count == 1)
                {
                    lngID = Convert.ToInt32(tb.Rows[0]["ID"]);

                    var oTaskState = new roLiveTaskState(-1);
                    //roBusinessState.CopyTo(oState, oTaskState);
                    if (!roLiveTask.CreateReportTask(lngID, ref oTaskState))
                    {
                        Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# FROM sysroReportTasks WHERE ID =" + Convert.ToString(lngID));
                        lngID = -1;
                    }
                }
            }
            catch (DbException ex)
            {
                _State.UpdateStateInfo(ex, "roReport:CreateTaskReport");
            }
            catch (Exception ex)
            {
                _State.UpdateStateInfo(ex, "roReport:CreateTaskReport");
            }
            finally
            {
            }

            return lngID;
        }

        //Se usa!
        public bool CreateTmpVisitsLocationEx(int IDReportTask, List<ReportParameter> parametersList, List<ReportParameter> originalParametersList)
        {
            bool bolRet;
            string parameterLocation;
            string visitFieldLocation;
            string sSQL, sSQLAux, ordenacion;
            string visitField1, visitFieldValue1, visitField2, visitFieldValue2, visitorField1, visitorValue1, visitorField2, visitorValue2, visitorField3, visitorValue3, visitorField4, visitorValue4;
            string visitorName, EmployeeName, location, EndDate;
            string Datetime = "";
            int idVisit, idVisitor, IDEmployee;
            DataTable ads, adsVisit, adsVisitFields;
            visitField1 = "";
            visitFieldValue1 = "";
            visitField2 = "";
            visitFieldValue2 = "";
            visitorField1 = "";
            visitorValue1 = "";
            visitorField2 = "";
            visitorValue2 = "";
            visitorField3 = "";
            visitorValue3 = "";
            visitorField4 = "";
            visitorValue4 = "";

            bolRet = false;

            try
            {
                //'crear datos en la tabla
                //'MTH: No borramos datos para las impresiones multihilo

                Robotics.DataLayer.AccessHelper.ExecuteSql("@DELETE# TMPVisitsLocation WHERE IDReportTask = " + IDReportTask + " OR IDReportTask = 0");

                //'Recogemos el parametro que marca la afrupacion para Localizacion
                parameterLocation = GetValueFromSysroLiveAdvancedParameters("roAdParLocation");
                //'Miramos el valor del campo de la ficha de Localizacion
                visitFieldLocation = GetValueFromVisitFields(parameterLocation);

                if (parameterLocation.Equals(String.Empty) || visitFieldLocation.Equals(String.Empty))
                {
                    sSQL = "@select# vi.IDVisitor, vi.Name VisitorName, e.ID, e.Name EmployeeName, v.IDVisit, v.status,'SIN ASIGNAR' AS 'Location',v.BeginDate, " + IDReportTask + " " +
                    "from [dbo].[Visit] v " +
                    "left join [dbo].[Visit_Visitor] vv on v.IDVisit = vv.IDVisit " +
                    "left join [dbo].[Visitor] vi on vv.IDVisitor = vi.IDVisitor " +
                    "join [dbo].[Employees] e on v.IDEmployee = e.ID " +
                    "where v.Status = 1";
                }
                else
                {
                    sSQL = "@select# vi.IDVisitor, vi.Name VisitorName, e.ID, e.Name EmployeeName, v.IDVisit, v.status," +
                           " case WHEN (@select# Value from [Visit_Fields] vf  left join Visit_Fields_Value vfv on vf.IDField = vfv.IDField where vf.Name = '" + parameterLocation + "' and vfv.IDVisit = v.IDVisit) IS NULL " +
                           "     then 'SIN ASIGNAR' " +
                           "     ELSE (@select# Value from [Visit_Fields] vf  left join Visit_Fields_Value vfv on vf.IDField = vfv.IDField where vf.Name = '" + parameterLocation + "' and vfv.IDVisit = v.IDVisit) END AS 'Location', " +
                           " v.BeginDate , " + IDReportTask + " " +
                           " from [dbo].[Visit] v " +
                           " left join [dbo].[Visit_Visitor] vv on v.IDVisit = vv.IDVisit " +
                           " left join [dbo].[Visitor] vi on vv.IDVisitor = vi.IDVisitor " +
                           " join [dbo].[Employees] e on v.IDEmployee = e.ID " +
                           " where v.Status = 1";
                }

                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQL);

                if (ads != null)
                {
                    foreach (DataRow adsDataRow in ads.Rows)
                    {
                        visitorName = roTypes.Any2String(adsDataRow["VisitorName"]);

                        idVisitor = roTypes.Any2Integer(adsDataRow["IDVisitor"]);
                        sSQLAux = "@Select# DatePunch, Action from [Visit_Visitor_Punch] where IDVisit = '" + adsDataRow["IDVisit"] + "' and IDVisitor = '" + adsDataRow["IDVisitor"] + "' ";

                        adsVisit = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQLAux);

                        if (adsVisit != null)
                        {
                            foreach (DataRow adsVisitDataRow in adsVisit.Rows)
                            {
                                if (roTypes.Any2String(adsVisitDataRow["Action"]).Equals("IN"))
                                {
                                    Datetime = roTypes.Any2Time(adsVisitDataRow["DatePunch"]).SQLSmallDateTime();
                                }
                            }

                            adsVisit.Dispose();
                        }

                        EndDate = "NULL";

                        IDEmployee = roTypes.Any2Integer(adsDataRow["ID"]);
                        EmployeeName = roTypes.Any2String(adsDataRow["EmployeeName"]);
                        idVisit = roTypes.Any2Integer(adsDataRow["IDVisit"]);
                        idVisitor = roTypes.Any2Integer(adsDataRow["IDVisitor"]);
                        location = roTypes.Any2String(adsDataRow["Location"]);

                        sSQLAux = "@select# '' as IDVisitor, vf.Name, vfv.Value, vf.position, 'A' as Ordenacion, ROW_NUMBER() OVER(ORDER BY position) Pos from [dbo].[Visit] v " +
                                  " left join [Visit_Fields_Value] vfv on v.IDVisit = vfv.IDVisit " +
                                  " left join [Visit_Fields] vf on vfv.IDField = vf.IDField " +
                                  " where v.IDVisit = '" + adsDataRow["IDVisit"] + "' and vf.Name is not null  " +
                                  " UNION " +
                                  " @select# vv.IDVisitor, vf.Name, vfv.Value, vf.position, 'B' as Ordenacion, vf.Pos from [Visit] v " +
                                  " inner join [dbo].[Visit_Visitor] vv on v.IDVisit = vv.IDVisit " +
                                  " left join [Visitor_Fields_Value] vfv on vv.IDVisitor = vfv.IDVisitor " +
                                  " left join (@select# *, ROW_NUMBER() OVER(ORDER BY position) Pos from  [Visitor_Fields]) vf on vfv.IDField = vf.IDField  " +
                                  " where v.IDVisit = '" + adsDataRow["IDVisit"] + "' and vf.Name is not null  " +
                                  " order by Ordenacion, vf.position";

                        visitFieldValue1 = "";
                        visitFieldValue2 = "";
                        visitorValue1 = "";
                        visitorValue2 = "";
                        visitorValue3 = "";
                        visitorValue4 = "";

                        adsVisitFields = Robotics.DataLayer.AccessHelper.CreateDataTable(sSQLAux);

                        if (adsVisitFields != null)
                        {
                            foreach (DataRow adsVisitFieldsDataRow in adsVisitFields.Rows)
                            {
                                ordenacion = roTypes.Any2String(adsVisitFieldsDataRow["Ordenacion"]);
                                if (Strings.UCase(ordenacion).Equals("A"))
                                {
                                    switch (roTypes.Any2Integer(adsVisitFieldsDataRow["Pos"]))
                                    {
                                        case 1:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty))
                                            {
                                                visitField1 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                            }
                                            visitFieldValue1 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            break;

                                        case 2:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty))
                                            {
                                                visitField2 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                            }

                                            visitFieldValue2 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (roTypes.Any2Long(adsVisitFieldsDataRow["Pos"]))
                                    {
                                        case 1:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty) && roTypes.Any2String(adsVisitFieldsDataRow["IDVisitor"]).Equals(idVisitor))
                                            {
                                                visitorField1 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                                visitorValue1 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            }
                                            break;

                                        case 2:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty) && roTypes.Any2String(adsVisitFieldsDataRow["IDVisitor"]).Equals(idVisitor))
                                            {
                                                visitorField2 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                                visitorValue2 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            }
                                            break;

                                        case 3:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty) && roTypes.Any2String(adsVisitFieldsDataRow["IDVisitor"]).Equals(idVisitor))
                                            {
                                                visitorField3 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                                visitorValue3 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            }
                                            break;

                                        case 4:
                                            if (!roTypes.Any2String(adsVisitFieldsDataRow["Name"]).Equals(String.Empty) && roTypes.Any2String(adsVisitFieldsDataRow["IDVisitor"]).Equals(idVisitor))
                                            {
                                                visitorField4 = roTypes.Any2String(adsVisitFieldsDataRow["Name"]);
                                                visitorValue4 = roTypes.Any2String(adsVisitFieldsDataRow["Value"]);
                                            }
                                            break;
                                    }
                                }
                            }

                            adsVisitFields.Dispose();
                        }

                        Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO TMPVisitsLocation VALUES ('" + idVisitor + "','" + visitorName + "'," + IDEmployee + ", " +
                                      "'" + EmployeeName + "', '" + idVisit + "', '" + location + "'," + Datetime + "," + IDReportTask + ", " +
                                      " '" + visitField1 + "', '" + visitFieldValue1 + "','" + visitField2 + "', '" + visitFieldValue2 + "', " +
                                      " '" + visitorField1 + "','" + visitorValue1 + "', '" + visitorField2 + "','" + visitorValue2 + "','" + visitorField3 + "','" + visitorValue3 + "', '" + visitorField4 + "','" + visitorValue4 + "', " + EndDate + ")");
                    }
                    Robotics.DataLayer.AccessHelper.ExecuteSql("@UPDATE# TMPVisitsLocation SET VisitFieldName1 = '" + visitField1 + "'," +
                                          " VisitFieldName2 = '" + visitField2 + "'," +
                                          " VisitorFieldName1 = '" + visitorField1 + "'," +
                                          " VisitorFieldName2 = '" + visitorField2 + "'," +
                                          " VisitorFieldName3 = '" + visitorField3 + "'," +
                                          " VisitorFieldName4 = '" + visitorField4 + "'");

                    ads.Dispose();
                }

                bolRet = true;
            }
            catch (Exception ex) {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CreateTmpVisitsLocationEx::", ex);
            }
            finally
            {
            }
            return bolRet;
        }

        public String GetValueFromSysroLiveAdvancedParameters(String parameter)
        {
            String sSql;
            DataTable ads;
            String value = "";

            try
            {
                ;

                sSql = "@SELECT# Value FROM sysroLiveAdvancedParameters WHERE ParameterName = '" + parameter + "'";
                value = "";
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSql);

                if (ads != null)
                {
                    foreach (DataRow adsdatarow in ads.Rows)
                    {
                        value = roTypes.Any2String(adsdatarow["Value"]);
                    }

                    ads.Dispose();
                }
            }
            catch (Exception) { }
            finally
            {
            }

            return value;
        }

        public String GetValueFromVisitFields(string name)
        {
            String sSql;
            DataTable ads;
            String value = "";

            try
            {
                ;

                sSql = "@SELECT# name from Visit_Fields where name = '" + name + "' and visible = 1";
                value = "";
                ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSql);

                if (ads != null)
                {
                    foreach (DataRow adsdatarow in ads.Rows)
                    {
                        value = roTypes.Any2String(adsdatarow["Name"]);
                    }
                    ads.Dispose();
                }
            }
            catch (Exception) { }
            finally
            {
            }

            return value;
        }
    }

    public class CF_ConvertHoursToTime : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                if (operands[1] == null)
                {
                    operands[1] = -1;
                }
                var timeFormat = roTypes.Any2String(Robotics.DataLayer.roCacheManager.GetInstance.GetParametersCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), Parameters.TimeFormat));

                Double hours = Convert.ToDouble(operands[0]);
                Double IDConcept = Convert.ToDouble(Convert.ToString(operands[1]));
                //Mostramos por defecto horas en formato decimal
                String strRet = String.Format("{0:0.00}", hours); 

                string format = string.Empty;
                if (operands.Length > 2) { 
                    format = roTypes.Any2String(operands[2]);
                }

                if (timeFormat == "1" || timeFormat == "") //Si está seleccionado formato horas y minutos, convertimos el valor
                {
                    if (IDConcept > 0)
                    {
                        if (string.IsNullOrEmpty(format))
                        {
                            roConceptEngine concept = roCacheManager.GetInstance.GetConceptCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), Convert.ToInt16(IDConcept));
                            if (concept == null)
                            {
                                roConceptManager oConceptManager = new roConceptManager(new roConceptManagerState(-1));
                                concept = oConceptManager.Load(Convert.ToInt16(IDConcept));
                                roCacheManager.GetInstance.UpdateConceptCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), concept);
                            }
                            if (concept != null)
                            {
                                format = concept.IDType;
                            }
                        }
                        switch (format)
                        {
                            case "H":
                                strRet = roConversions.ConvertHoursToTime(hours);
                                break;
                            case "O":
                                strRet = hours.ToString("###0.00");
                                break;
                            default:
                                strRet = Robotics.Base.VTBusiness.Concept.roConcept.TimeFormat(hours, IDConcept);
                                break;
                        }
                    }
                    else 
                    {
                        strRet = roConversions.ConvertHoursToTime(hours);
                    }
                }
                return strRet;
            }
            catch (Exception) { return ""; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "TimeFormat"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_ConvertWinColorToHex : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                return Robotics.Base.VTBusiness.Concept.roConcept.ColorHexFormat(Convert.ToInt32(operands[0]));
            }
            catch (Exception) { return ""; }
            finally
            {
            }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "ColorHexFormat"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetAccrualValueonDate : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                //GetAccrualValueonDate( [IDEmployee] , [IDConcept] , [Date] )
                if (operands[1] == null)
                {
                    operands[1] = -1;
                }

                string strwhere = "";
                return Robotics.Base.VTBusiness.Concept.roConcept.GetAccrualValueOnDateDX(roTypes.Any2Integer(operands[0]), Convert.ToDateTime(operands[2]), Convert.ToInt32(Convert.ToString(operands[1])), strwhere);
            }
            catch (Exception) { return 0; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetAccrualValueonDate"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetExpectedWorkingHoursHolidays : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                //(IDEmployee, IDShift, StartDate, EndDate) return Double
                return Robotics.Base.VTBusiness.Concept.roConcept.GetExpectedWorkingHoursHolidaysDX(roTypes.Any2Integer(operands[0]), Convert.ToInt32(Convert.ToString(operands[1])), Convert.ToDateTime(operands[2]), Convert.ToDateTime(operands[3]));
            }
            catch (Exception) { return 0; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetExpectedWorkingHoursHolidays"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetBeginHourByShift : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                //(IDEmployee,IDShift,DayDate,Layer) return String
                return Robotics.Base.VTBusiness.Shift.roShift.GetBeginHourByShiftDX(roTypes.Any2Integer(operands[0]), Convert.ToInt32(Convert.ToString(operands[1])), Convert.ToString(operands[2]), roTypes.Any2Integer(operands[3]));
            }
            catch (Exception) { return 0; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetBeginHourByShift"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetAccrualValueByDefaultQuery : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                string strwhere = "";
                if (operands[1] == null)
                {
                    operands[1] = -1;
                }
                return Robotics.Base.VTBusiness.Concept.roConcept.GetAccrualValueByDefaultQueryDX(roTypes.Any2Integer(operands[0]), Convert.ToDateTime(operands[2]), Convert.ToInt32(Convert.ToString(operands[1])), strwhere);
            }
            catch (Exception) { return 0; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetAccrualValueByDefaultQuery"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetBradfordFactor : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                if (operands[0] == null)
                {
                    operands[0] = -1;
                }
                return Robotics.Base.VTBusiness.Incidence.roIncidence.GetBradfordFactorDX(roTypes.Any2Integer(operands[0]));
            }
            catch { return 0; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetBradfordFactor"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetDirectSupervisors : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                if (operands[0] == null || operands[1] == null) //IDRequest, IDEmployee
                {
                    operands[0] = -1;
                    operands[1] = -1;
                }

                string sSql = "@SELECT# dbo.GetDirectSupervisorsByRequestAndEmployee(" + operands[0] + ", " + operands[1] + ") tmp";
                DataTable ads = Robotics.DataLayer.AccessHelper.CreateDataTable(sSql);
                string strRet = "";

                if (ads != null)
                {
                    foreach (DataRow adsdatarow in ads.Rows)
                    {
                        strRet = roTypes.Any2String(adsdatarow["tmp"]);
                    }
                    ads.Dispose();
                }

                return strRet;
            }
            catch { return "-"; }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetDirectSupervisors"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetCompanyName : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                var companyName = Robotics.DataLayer.roCacheManager.GetInstance.GetCompanyGUID(Robotics.Azure.RoAzureSupport.GetCompanyName());
                var companyInfo = new roCacheManager().GetCompanyInfo(companyName);
                if (companyInfo != null)
                {
                    return companyInfo.name;
                }
                return "";
            }
            catch (Exception ex) {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CF_GetCompanyName::", ex);
                return ""; }
            finally
            {
            }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetCompanyName"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }

    public class CF_GetLocalDayOfWeek : ICustomFunctionOperator
    {
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            try
            {
                DateTime date = new DateTime(int.Parse(operands[0].ToString().Split('-')[2]), int.Parse(operands[0].ToString().Split('-')[1]), int.Parse(operands[0].ToString().Split('-')[0]));                            

                return ((int)date.DayOfWeek);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "TermporalTablesManager::CF_GetCompanyName::", ex);
                return "";
            }
            finally
            {
            }
        }

        string ICustomFunctionOperator.Name
        {
            get { return "GetLocalDayOfWeek"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(int);
        }

        public Type ResultType(params Type[] operands)
        {
            throw new NotImplementedException();
        }
    }
}