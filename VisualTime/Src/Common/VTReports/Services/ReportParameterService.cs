using DevExpress.XtraReports.UI;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReportGenerator.Repositories;
using Robotics.Base;
using Robotics.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using Robotics.Base.VTSelectorManager;
using DevExpress.DataProcessing;
using Robotics.VTBase;

namespace ReportGenerator.Services
{
    public class ReportParameterService : IReportParameterService
    {
        private IReportLastParametersRepository lastParametersRepository;
        private IVisualTimeQueryDecoderService visualTimeQueryDecoderService;

        #region Constructor

        public ReportParameterService()
        {
            this.visualTimeQueryDecoderService = new VisualTimeQueryDecoderService();
            this.lastParametersRepository = new ReportLastParametersRepository();
            this.visualTimeQueryDecoderService = new VisualTimeQueryDecoderService();
        }

        #endregion Constructor

        public List<ReportParameter> GetDevexpressCompatibleReportParameters(string parametersJson, int passportId, int taskId)
        {
            List<ReportParameter> auxliarParametersList = DeserializeJsonParameters(parametersJson);

            SetNonUserDefinedValues(auxliarParametersList, passportId, taskId);

            foreach (ReportParameter auxiliarReportParameter in auxliarParametersList)
            {
                CastDevexpressCompatibleParameter(auxiliarReportParameter, passportId, auxliarParametersList);
            }

            return auxliarParametersList;
        }

        public string GetLastParametersFromReport(int reportId, int passportId)
        {
            return lastParametersRepository.Get(reportId, passportId);
        }

        public void SetParameterValuesToXtraReport(Report report, XtraReport xtraReport, int taskId)
        {
            List<ReportParameter> originalParametersList = DeserializeJsonParameters(report.ParametersJson);

            HandleFunctionCallsFromParameters(report.ParametersList, originalParametersList, taskId);

            foreach (ReportParameter parameter in report.ParametersList)
            {
                xtraReport.Parameters[parameter.Name].MultiValue = parameter.IsMultiValue;
                xtraReport.Parameters[parameter.Name].Value = parameter.Value;
            }
        }

        public void SetParameterValuesToXtraReportEmployee(Report report, XtraReport xtraReport, int employeeId, int taskId)
        {
            List<ReportParameter> originalParametersList = DeserializeJsonParameters(report.ParametersJson);

            foreach (ReportParameter param in report.ParametersList)
            {
                if (param.Type.Equals("Robotics.Base.employeesSelector"))
                {
                    employeesSelector[] emp = { new employeesSelector { Value = employeeId.ToString() } };
                    param.Value = emp;
                }
            }

            HandleFunctionCallsFromParameters(report.ParametersList, originalParametersList, taskId);

            foreach (ReportParameter parameter in report.ParametersList)
            {
                xtraReport.Parameters[parameter.Name].MultiValue = parameter.IsMultiValue;
                xtraReport.Parameters[parameter.Name].Value = parameter.Value;
            }
        }

        public bool SaveLastParameters(int reportId, int passportId, string lastParameters)
        {
            if (lastParametersRepository.Get(reportId, passportId) != null)
            {
                return lastParametersRepository.Update(reportId, passportId, lastParameters);
            }
            else
            {
                return lastParametersRepository.Insert(reportId, passportId, lastParameters);
            }
        }

        public bool DeleteAllLastParameters(int reportId)
        {
            return lastParametersRepository.DeleteAll(reportId);
        }

        public bool DeleteRowsFromTemporalTables(IEnumerable<ReportParameter> parametersList, int taskId)
        {
            bool result = true;

            try
            {
                foreach (ReportParameter parameter in parametersList.Where(x => x.Type.Equals("Robotics.Base.functionCall")))
                {
                    new Support.TemporalTablesManager().FlushTemporalRows(parameter.Value.ToString(), taskId);
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        #region Private Methods (Helpers)

        private void CastDevexpressCompatibleParameter(ReportParameter parameter, int passportId, List<ReportParameter> auxliarParametersList)
        {
            JArray auxiliarArray = null;
            Object[] parameterValuesArray = null;

            switch (parameter.Type)
            {
                case "Robotics.Base.userFieldsSelectorRadioBtn": //Campos de la ficha
                case "Robotics.Base.userFieldsSelector": //Campos de la ficha
                    //try { auxiliarArray = (JArray)parameter.Value; } catch { auxiliarArray = new JArray(); }
                    //parameterValuesArray = auxiliarArray.Select(x => new userFieldsSelector { Value = x.ToString() }).ToArray();
                    //parameter.Value = (parameter.IsMultiValue ? parameterValuesArray : parameterValuesArray.FirstOrDefault());
                    break;

                case "Robotics.Base.formatSelector": //Campos de la ficha
                    try
                    {
                        parameter.Value = Robotics.VTBase.roTypes.Any2Integer(((JObject)parameter.Value)["format"].ToString());
                    }
                    catch
                    {
                        parameter.Value = "0";
                    }
                    break;
                
                case "Robotics.Base.viewAndFormatSelector": //Campos de la ficha
                    try
                    {
                        bool showDetail = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["ckDetail"]);
                        bool showTotals = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["ckTotal"]);
                        bool showChart = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["ckShowChart"]);
                        parameter.Value = (showDetail ? "1" : "0") + "," + (showTotals ? "1" : "0") + "," + (showChart ? "1" : "0");
                    }
                    catch
                    {
                        parameter.Value = "1,1";
                    }
                    break;

                case "Robotics.Base.accessTypeSelector": //Tipo de acceso
                    try
                    {
                        bool showValids = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["ckValids"]);
                        bool showInvalids = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["ckInvalids"]);
                        parameter.Value = (showValids ? "1" : "0") + "," + (showInvalids ? "1" : "0");
                    }
                    catch
                    {
                        parameter.Value = "1,0";
                    }
                    break;

                case "Robotics.Base.filterProfileTypesSelector": //Saldos, justificaciones o Incidencias
                    try
                    {
                        if( ((JObject)parameter.Value)["rangoCriterio"].GetType() == typeof(JObject))
                        {
                            ((JObject)parameter.Value)["rangoCriterio"] = ((JObject)parameter.Value)["rangoCriterio"]?["value"]?? "diariamente";
                        }
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "1"; //Mostramos saldos por defecto
                    }
                    break;

                case "Robotics.Base.filterSelectorCausesRegistroJL": //Justificaciones Registro jornada laboral
                    try
                    {
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "1";
                    }
                    break;

                case "Robotics.Base.filterSelectorConceptsRegistroJL": //Saldos Registro jornada laboral
                    try
                    {
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "1";
                    }
                    break;

                case "Robotics.Base.yearAndMonthSelector": //Mes y año
                    //IMPORTANTE: Necesitamos crear 2 parametros extras en el informe con nombre: "Year" y "Month", para darles valor automáticamente con el valor seleccionado en éste selector
                    try
                    {
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "2022,01"; //Mostramos valores por defecto
                    }
                    break;

                case "Robotics.Base.betweenYearAndMonthSelector": //Rango Mes y año
                                                                  //IMPORTANTE: Necesitamos crear 4 parametros extras en el informe con nombre: "YearStart", "MonthStart", "YearEnd" y "MonthEnd" para darles valor automáticamente con el valor seleccionado en éste selector
                    try
                    {
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "2022,01-2022,04"; //Mostramos valores por defecto
                    }
                    break;

                case "Robotics.Base.filterValuesSelector": //Campos de la ficha
                    try
                    {
                        bool optionFilterValues = Robotics.VTBase.roTypes.Any2Boolean(((JObject)parameter.Value)["optionFilterValues"]);
                        string typeSince = Robotics.VTBase.roTypes.Any2String(((JObject)parameter.Value)["typeFilterSince"]);
                        string typeTo = Robotics.VTBase.roTypes.Any2String(((JObject)parameter.Value)["typeFilterTo"]);
                        string valueSince = Robotics.VTBase.roTypes.Any2String(((JObject)parameter.Value)["valueSince"]);
                        string valueTo = Robotics.VTBase.roTypes.Any2String(((JObject)parameter.Value)["valueTo"]);

                        if (!optionFilterValues)
                        {
                            parameter.Value = "filter,0, ,0, ,0";
                        }
                        else
                        {
                            if (typeSince == "text")
                            {
                                Array valueSinceHHmm = valueSince.Split(':');
                                valueSince = Convert.ToInt32(((string[])valueSinceHHmm)[0]) * 60 + Convert.ToInt32(((string[])valueSinceHHmm)[1]).ToString();
                            }
                            if (typeTo == "text")
                            {
                                Array valueToHHmm = valueTo.Split(':');
                                valueTo = Convert.ToInt32(((string[])valueToHHmm)[0]) * 60 + Convert.ToInt32(((string[])valueToHHmm)[1]).ToString();
                            }

                            parameter.Value = "filter,1," + typeSince + "," + valueSince + "," + typeTo + "," + valueTo;
                        }
                    }
                    catch
                    {
                        parameter.Value = "1,1";
                    }
                    break;

                case "Robotics.Base.conceptGroupsSelector": // GRUPO DE SALDOS -----------------------------------------------
                    break;

                case "Robotics.Base.conceptsSelector": //SALDOS ---------------------------------------------------------------
                    break;

                case "Robotics.Base.causesSelector": //JUSTIFICACIONES -------------------------------------------------------
                    break;

                case "Robotics.Base.incidencesSelector": //INCIDENCIAS -------------------------------------------------------
                    try { auxiliarArray = (JArray)parameter.Value; } catch { auxiliarArray = new JArray(); }
                    parameterValuesArray = auxiliarArray.Select(x => new incidencesSelector { Value = x.ToString() }).ToArray();
                    parameter.Value = (parameter.IsMultiValue ? parameterValuesArray : parameterValuesArray.FirstOrDefault());
                    break;

                case "Robotics.Base.shiftsSelector": //Horarios ---------------------                    
                    break;

                case "Robotics.Base.holidaysSelector": //Horarios de vacacuibes
                    break;

                case "Robotics.Base.terminalsSelector": // Terminales -------------
                    try { auxiliarArray = (JArray)parameter.Value; } catch { auxiliarArray = new JArray(); }
                    parameterValuesArray = auxiliarArray.Select(x => new terminalsSelector { Value = x.ToString() }).ToArray();
                    parameter.Value = (parameter.IsMultiValue ? parameterValuesArray : parameterValuesArray.FirstOrDefault());
                    break;

                case "Robotics.Base.tasksSelector": // Tasks -------------
                    //Llamar a función que devuelva string con ids de tareas y le pasamos tareas + nombres proyectos
                    try { auxiliarArray = (JArray)parameter.Value; } catch { auxiliarArray = new JArray(); }
                    parameterValuesArray = this.visualTimeQueryDecoderService.GetTasksFromEncodedQuery(parameter.Value.ToString() ?? String.Empty).Select(x => new tasksSelector { Value = x.ToString() }).ToArray();
                    parameter.Value = String.Join(",", parameterValuesArray);
                    break;

                case "Robotics.Base.zonesSelector": // Zonas ------------------------------
                    //try { auxiliarArray = (JArray)parameter.Value; } catch { auxiliarArray = new JArray(); }
                    //parameterValuesArray = auxiliarArray.Select(x => new zonesSelector { Value = x.ToString() }).ToArray();
                    //parameter.Value = (parameter.IsMultiValue ? parameterValuesArray : parameterValuesArray.FirstOrDefault());
                    break;

                case "Robotics.Base.employeesSelector": // Empleados -------------------------
                   

                    //if(parameter.Description == "v2")
                    //{
                    //    string[] selectorParams = parameter.Value.ToString().Split('@');

                    //    parameter.Value = GetWhereWithoutPermissions(selectorParams[0], selectorParams[1], selectorParams[2], false).ToUpper().Replace("@SELECT#", "SELECT");
                    //}
                    //else
                    //{
                    bool isEmployeeBetweenDates = auxliarParametersList.Find(param => param.Name.Equals("EmployeesBetweenDates") && param.Value.ToString().Equals("1")) != null;
                    ReportParameter paramDateRange = auxliarParametersList.Find(param => param.Type.Equals("System.DateTime") && param.IsRangeValue);
                    DateTime? startDatePeriod = null;
                    DateTime? endDatePeriod = null;

                    if (isEmployeeBetweenDates && paramDateRange != null)
                    {
                        DateTime[] auxiliarDatePeriod = this.visualTimeQueryDecoderService.GetDatePeriodFromEncodedQuery(paramDateRange.Value.ToString() ?? String.Empty);
                        startDatePeriod = auxiliarDatePeriod[0];
                        endDatePeriod = auxiliarDatePeriod[1];
                    }

                    parameterValuesArray = this.visualTimeQueryDecoderService.GetEmployeesFromEncodedQuery(parameter.Value.ToString() ?? String.Empty, passportId, startDatePeriod, endDatePeriod)
                                                                                    .Select(x => new employeesSelector { Value = x.ToString() })
                                                                                    .ToArray();
                    parameter.Value = (parameter.IsMultiValue ? parameterValuesArray : parameterValuesArray.FirstOrDefault());
                    //}
                    break;

                case "System.DateTime": // Fecha Hora -------------------------------------
                    if (parameter.IsRangeValue)
                    {
                        DateTime[] auxiliarDatePeriod = this.visualTimeQueryDecoderService.GetDatePeriodFromEncodedQuery(parameter.Value.ToString() ?? String.Empty);
                        parameter.IsMultiValue = false;
                        parameter.Value = DevExpress.XtraReports.Parameters.Range.Create<DateTime>(auxiliarDatePeriod[0], auxiliarDatePeriod[1]);
                    }
                    else
                    { CastParameterToOriginalType(parameter); }
                    break;

                case "Robotics.Base.taskIdentifier": //(system) TaskId
                    parameter.IsMultiValue = false;
                    break;

                case "Robotics.Base.passportIdentifier": // Identificador de usuario
                    parameter.IsMultiValue = false;
                    break;

                case "Robotics.Base.conceptIdentifier":     // Identificador de saldo
                    parameter.IsMultiValue = false;
                    parameter.Value = Robotics.VTBase.roTypes.Any2String(parameter.Value);
                    break;

                case "Robotics.Base.causeIdentifier":     // Identificador de justificacion
                    parameter.IsMultiValue = false;
                    parameter.Value = Robotics.VTBase.roTypes.Any2String(parameter.Value);
                    break;

                case "Robotics.Base.incidenceIdentifier":     // Identificador de incidencia
                    parameter.IsMultiValue = false;
                    parameter.Value = Robotics.VTBase.roTypes.Any2String(parameter.Value);
                    break;

                case "Robotics.Base.userFieldIdentifier":     // Identificador de saldo
                    parameter.IsMultiValue = false;
                    parameter.Value = Robotics.VTBase.roTypes.Any2String(parameter.Value);
                    break;

                case "Robotics.Base.functionCall":    //TmpTable
                    parameter.Value = Robotics.VTBase.roTypes.Any2String(parameter.Name);
                    break;

                case "Robotics.Base.projectsVSLSelector": //Projectos VSL
                    //IMPORTANTE: Necesitamos crear 2 parametros extras en el informe con nombre: "StartProject" y "EndProject", para darles valor automáticamente con el valor seleccionado en este selector
                    try
                    {
                        parameter.Value = ((JObject)parameter.Value);
                    }
                    catch
                    {
                        parameter.Value = "0,1"; //Mostramos valores por defecto
                    }
                    break;

                default:
                    CastParameterToOriginalType(parameter);
                    break;
            }
        }

        private void CastParameterToOriginalType(ReportParameter parameter)
        {
            if (parameter.IsMultiValue)
            {
                JArray auxiliarArray = (JArray)parameter.Value;
                parameter.Value = auxiliarArray.ToObject(Type.GetType(String.Concat(parameter.Type, "[]")));
            }
            else
            {
                try
                {
                    switch (parameter.Type)
                    {
                        case "System.Guid":
                            parameter.Value = Guid.Parse(parameter.Value.ToString());
                            break;

                        case "System.DateTime":
                            if (parameter.Value.ToString().Contains(","))
                            {
                                DateTime[] auxiliarDatePeriod = this.visualTimeQueryDecoderService.GetDateFromEncodedQuery(parameter.Value.ToString() ?? String.Empty);
                                parameter.Value = auxiliarDatePeriod[0];
                            }
                            else
                            {
                                parameter.Value = Convert.ChangeType(parameter.Value, Type.GetType(parameter.Type));
                            }
                            break;
                        case "System.String":
                            if(parameter.Name == "PTRangoCriterio" && parameter.Value.GetType() == typeof(JObject))
                            {
                                parameter.Value = Convert.ChangeType(((JObject)parameter.Value)["value"] ?? "diariamente", Type.GetType(parameter.Type));
                            }
                            else if (parameter.Name == "PTConcepts")
                            {
                                string sValue = roTypes.Any2String(parameter.Value);
                                if (sValue.Contains(",") || (!sValue.Contains(",") && roTypes.Any2Integer(sValue) > 0 ))
                                {
                                    parameter.Value = Convert.ChangeType(parameter.Value, Type.GetType(parameter.Type));
                                }
                                else
                                {
                                    parameter.Value = "";
                                }
                            } 
                            else
                            {
                                parameter.Value = Convert.ChangeType(parameter.Value, Type.GetType(parameter.Type));
                            }
                            break;
                        
                        default:
                                parameter.Value = Convert.ChangeType(parameter.Value, Type.GetType(parameter.Type)); 
                            break;
                    }
                }
                catch
                {
                    //Do nothing
                }
            }
        }

        private List<ReportParameter> DeserializeJsonParameters(string parametersJson)
        {
            return JsonConvert.DeserializeObject<List<ReportParameter>>(parametersJson) ?? new List<ReportParameter>();
        }

        private void HandleFunctionCallsFromParameters(IEnumerable<ReportParameter> parametersList, IEnumerable<ReportParameter> originalParametersList, int taskId)
        {
            foreach (ReportParameter parameter in parametersList.Where(x => x.Type.Equals("Robotics.Base.functionCall")))
            {
                if (parameter.IsMultiValue)
                {
                    JArray auxiliarArray = (JArray)parameter.Value;
                    auxiliarArray.ToList()
                                 .ForEach(x => new Support.TemporalTablesManager().ExecuteTask(x.ToString(), taskId, parametersList.ToList(), originalParametersList.ToList()));
                }
                else
                {
                    new Support.TemporalTablesManager().ExecuteTask(parameter.Value.ToString(), taskId, parametersList.ToList(), originalParametersList.ToList());
                }
            }
        }

        private void SetNonUserDefinedValues(List<ReportParameter> parametersList, int passportId, int taskId)
        {
            foreach (ReportParameter parameter in parametersList)
            {
                switch (parameter.Type)
                {
                    case "Robotics.Base.passportIdentifier": // Identificador de usuario
                        parameter.Value = passportId;
                        break;

                    case "Robotics.Base.taskIdentifier":
                        parameter.Value = taskId;
                        break;
                }
            }
        }

        #endregion Private Methods (Helpers)
    }
}