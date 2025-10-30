using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robotics.Base;
using Robotics.Web.Base.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Web.VTLiveMvC.Models
{
    internal class ReportParameters
    {
        #region Constants

        private const string MYENTERPRISE_ZONE = "Mi Empresa";
        private const string UNESPECIFIED_ZONE = "255";
        private const string templateNameSelectorUniversal = "selectorUniversal";
        private const string templateNameSelectorConcepts = "selectorConcepts";
        private const string templateNameSelectorCauses = "selectorCauses";
        private const string templateNameSelectorViewFormat = "selectorViewFormat";
        private const string templateNameSelectorAccessType = "selectorAccessType";
        private const string templateNameSelectorProfileTypes = "selectorProfileTypes";
        private const string templateNameSelectorCausesRegistroJL = "selectorCausesRegistroJL";
        private const string templateNameSelectorConceptsRegistroJL = "selectorConceptsRegistroJL";
        private const string templateNameSelectorYearAndMonth = "selectorYearAndMonth";
        private const string templateNameSelectorBetweenYearAndMonth = "selectorBetweenYearAndMonth";
        private const string templateNameSelectorFormat = "selectorFormat";
        private const string templateNameSelectorFilterValues = "selectorFilterValues";
        private const string templateNameSelectorEmployees = "selectorEmployees";
        private const string templateNameSelectorTimePeriod = "selectorPeriodTime";
        private const string templateNameSelectorDate = "selectorDate";
        private const string templateNameSelectorString = "selectorString";
        private const string templateNameSelectorBoolean = "selectorBoolean";
        private const string templateNameSelectorNumber = "selectorNumber";
        private const string templateNameSelectorGuid = "selectorGuid";
        private const string templateNameSelectorTasks = "selectorTasks";
        private const string templateNameSelectorProjectsVSL = "selectorProjectsVSL";
        private const string templateNameHidden = "hidden";

        #endregion Constants

        public List<ReportParameter> GetReportParameters(string parametersJson)
        {
            List<ReportParameter> auxliarParametersList = DeserializeJsonParameters(parametersJson ?? String.Empty);

            foreach (ReportParameter auxiliarReportParameter in auxliarParametersList)
            {
                RetrieveParameterTemplate(auxiliarReportParameter);
                CastParameter(auxiliarReportParameter);
            }

            return auxliarParametersList;
        }

        public List<object> GetGenericParameterSelectorOptions(string parameterType, bool isEmergencyReport)
        {
            //IDictionary<int, string> genericParameterOptionsDictionary = new Dictionary<int, String>();
            List<object> genericParameterOptionsList = new List<object>();

            switch (parameterType)
            {
                case "Robotics.Base.userFieldsSelectorRadioBtn": // campos de la ficha
                case "Robotics.Base.userFieldsSelector": // campos de la ficha
                    foreach (System.Data.DataRow item in UserFieldServiceMethods.GetUserFields(null, Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField, "Used = 1", false, isEmergencyReport).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["fieldName"].ToString() });
                    }
                    break;

                case "Robotics.Base.conceptGroupsSelector": //grupos de saldos
                    foreach (System.Data.DataRow item in ConceptsServiceMethods.GetConceptsGroups(null, true).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;

                case "Robotics.Base.conceptsSelector":  //saldos
                    foreach (System.Data.DataRow item in ConceptsServiceMethods.GetConcepts(null).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;

                case "Robotics.Base.groupConceptsSelector":
                    int value;
                    string title;
                    ArrayList tmpArrayGroupSelected = new ArrayList();
                    // id grupo saldo + nombregrupo saldo saldo
                    foreach (System.Data.DataRow item in ConceptsServiceMethods.GetConceptsGroups(null, true).Rows)
                    {
                        value = Int32.Parse(item["ID"].ToString());
                        title = item["Name"].ToString();
                        tmpArrayGroupSelected = new ArrayList();

                        foreach (System.Data.DataRow item2 in ConceptsServiceMethods.GetReportGroupConcepts(null, value).Rows)
                        {
                            tmpArrayGroupSelected.Add(item2["ID"]);
                        }

                        StringBuilder csvList = new StringBuilder();
                        int i = 0;
                        foreach (var list in tmpArrayGroupSelected)
                        {
                            csvList.Append(string.Join(",", list));
                            if (i < tmpArrayGroupSelected.Count - 1) csvList.Append(",");
                            i++;
                        }
                        genericParameterOptionsList.Add(new { value = csvList.ToString(), title = item["Name"].ToString() });
                    }

                    foreach (System.Data.DataRow item in ConceptsServiceMethods.GetReportGroupConcepts(null, 0).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;

                case "Robotics.Base.causesSelector"://justificaciones
                    foreach (System.Data.DataRow item in CausesServiceMethods.GetCauses(null, String.Empty, true).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;

                case "Robotics.Base.incidencesSelector"://incidencias
                    foreach (System.Data.DataRow item in IncidenceServiceMethods.GetIncidencesDescription(null, " IDIncidence > 0 ").Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["IDIncidence"].ToString(), title = item["Description"].ToString() });
                    }
                    break;

                case "Robotics.Base.viewAndFormatSelector": // Visualizar
                    break;

                case "Robotics.Base.accessTypeSelector": // Accesos validos o inválidos
                    break;

                case "Robotics.Base.filterProfileTypesSelector": // Perfiles de filtro
                    break;

                case "Robotics.Base.filterSelectorCausesRegistroJL": // Causes registro jornada laboral
                    break;

                case "Robotics.Base.filterSelectorConceptsRegistroJL": // Concepts registro jornada laboral
                    break;

                case "Robotics.Base.yearAndMonthSelector": // Mes y año
                    break;

                case "Robotics.Base.betweenYearAndMonthSelector": // Rango Mes y año
                    break;

                case "Robotics.Base.formatSelector": // Formato
                    break;

                case "Robotics.Base.tasksSelector": // Tasks
                    break;

                case "Robotics.Base.filterValuesSelector"://Filtro por valores
                    foreach (System.Data.DataRow item in UserFieldServiceMethods.GetUserFields(null, Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField, "Used = 1", false).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["fieldName"].ToString() });
                    }
                    break;

                case "Robotics.Base.shiftsSelector": //horarios
                    foreach (System.Data.DataRow item in ShiftServiceMethods.GetShifts(null).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;
                case "Robotics.Base.holidaysSelector": //horarios
                    foreach (System.Data.DataRow item in ShiftServiceMethods.GetHolidayShifts(null).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                    }
                    break;

                case "Robotics.Base.terminalsSelector": //terminales
                    foreach (System.Data.DataRow item in TerminalServiceMethods.GetTerminalsDataSet(null).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Description"].ToString() });
                    }
                    break;

                case "Robotics.Base.zonesSelector": //zonas
                    foreach (System.Data.DataRow item in ZoneServiceMethods.GetZones(null).Rows)
                    {
                        if (item["id"].ToString() != UNESPECIFIED_ZONE && item["Name"].ToString() != MYENTERPRISE_ZONE)
                        {
                            genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["Name"].ToString() });
                        }
                    }
                    break;
                case "Robotics.Base.userFieldsNumberSelector": // campos de la ficha con valor numérico/decimal/hora
                    string strWhere = "Used = 1 AND FieldType IN (1,3,4)"; //1-Numérico, 3-Decimal, 4-Hora
                    foreach (System.Data.DataRow item in UserFieldServiceMethods.GetUserFields(null, Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField, strWhere, false, isEmergencyReport).Rows)
                    {
                        genericParameterOptionsList.Add(new { value = item["ID"].ToString(), title = item["fieldName"].ToString() });
                    }
                    break;
                case "Robotics.Base.projectsVSLSelector": // Projects VSL
                    break;

                default:
                    throw new Exception();
            }

            return genericParameterOptionsList;
        }

        private void CastParameter(ReportParameter parameter)
        {
            switch (parameter.Type)
            {
                case "Robotics.Base.userFieldsSelectorRadioBtn":    // Campos de la ficha
                case "Robotics.Base.userFieldsSelector":    // Campos de la ficha
                case "Robotics.Base.conceptGroupsSelector": // Grupos de saldos
                case "Robotics.Base.conceptsSelector":      // Saldos
                case "Robotics.Base.causesSelector":        // Justificaciones
                case "Robotics.Base.incidencesSelector":    // Incidencias
                case "Robotics.Base.formatSelector":        // Formato
                case "Robotics.Base.viewAndFormatSelector": // Visualización
                case "Robotics.Base.accessTypeSelector": // Accesos validos o inválidos
                case "Robotics.Base.filterProfileTypesSelector": // Perfiles de filtro
                case "Robotics.Base.filterSelectorCausesRegistroJL": // Causes registro jornada laboral
                case "Robotics.Base.filterSelectorConceptsRegistroJL": // Concepts registro jornada laboral
                case "Robotics.Base.yearAndMonthSelector": // Mes y año
                case "Robotics.Base.betweenYearAndMonthSelector": // Rango Mes y año
                case "Robotics.Base.filterValuesSelector":  // Filtro por valores
                case "Robotics.Base.shiftsSelector":        // Horarios
                case "Robotics.Base.holidaysSelector":      // Vacaciones
                case "Robotics.Base.terminalsSelector":     // Terminales
                case "Robotics.Base.tasksSelector":         // Tasks
                case "Robotics.Base.zonesSelector":         // Zonas
                case "Robotics.Base.employeesSelector":     // Empleados
                case "Robotics.Base.passportIdentifier":    // Identificador de usuario
                case "Robotics.Base.conceptIdentifier":     // Identificador de saldo
                case "Robotics.Base.causeIdentifier":       // Identificador de justificación
                case "Robotics.Base.incidenceIdentifier":   // Identificador de incidencia
                case "Robotics.Base.userFieldIdentifier":   // Identificador de ficha
                case "Robotics.Base.taskIdentifier":        // (system) TaskId
                case "Robotics.Base.projectsVSLSelector":   // Projects VSL
                case "Robotics.Base.functionCall":          // (system) TmpTable
                    CastParameterToCompatibleType(parameter);
                    break;

                default:
                    CastParameterToOriginalType(parameter);
                    break;
            }
        }

        private void CastParameterToCompatibleType(ReportParameter parameter)
        {
            switch (parameter.Type)
            {
                case "Robotics.Base.userFieldsSelectorRadioBtn":
                case "Robotics.Base.userFieldsSelector":
                case "Robotics.Base.conceptGroupsSelector"://Grupos de saldos
                case "Robotics.Base.conceptsSelector":      // Saldos
                case "Robotics.Base.causesSelector":        // justificaciones
                case "Robotics.Base.incidencesSelector":        // incidencias
                case "Robotics.Base.viewAndFormatSelector": // Visualización y formato
                case "Robotics.Base.accessTypeSelector": // Accesos validos o inválidos
                case "Robotics.Base.filterProfileTypesSelector": // Perfiles de filtro
                case "Robotics.Base.filterSelectorCausesRegistroJL": // Causes registro jornada laboral
                case "Robotics.Base.filterSelectorConceptsRegistroJL": // Concepts registro jornada laboral
                case "Robotics.Base.yearAndMonthSelector": // Mes y año
                case "Robotics.Base.betweenYearAndMonthSelector": // Rango Mes y año
                case "Robotics.Base.formatSelector": // Visualización y formato
                case "Robotics.Base.filterValuesSelector":  // Filtrar por valores
                case "Robotics.Base.shiftsSelector":
                case "Robotics.Base.holidaysSelector":
                case "Robotics.Base.terminalsSelector":
                case "Robotics.Base.tasksSelector":
                case "Robotics.Base.zonesSelector":
                case "Robotics.Base.projectsVSLSelector":   // Projects VSL
                case "Robotics.Base.employeesSelector":
                    parameter.Value = String.Empty;
                    break;

                case "Robotics.Base.taskIdentifier":
                case "Robotics.Base.passportIdentifier":  // Identificador de usuario
                case "Robotics.Base.conceptIdentifier":   // Identificador de saldo
                case "Robotics.Base.causeIdentifier":  // Identificador de justificación
                case "Robotics.Base.incidenceIdentifier":  // Identificador de incidencia
                case "Robotics.Base.userFieldIdentifier":   // Identificador de ficha
                    parameter.Value = 0;
                    break;

                case "Robotics.Base.functionCall":
                    parameter.Value = parameter.Name;
                    break;

                default:
                    throw new Exception();
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
                switch (parameter.Type)
                {
                    case "System.Guid":
                        parameter.Value = Guid.Parse(parameter.Value.ToString());
                        break;

                    case "System.DateTime":
                        if (parameter.Value.GetType() == typeof(JObject))
                        {
                            JObject auxiliarValue = (JObject)parameter.Value;
                            parameter.Value = ""; //DevExpress.XtraReports.Parameters.Range.Create<DateTime>((DateTime)auxiliarValue["Start"], (DateTime)auxiliarValue["End"]);
                        }
                        else
                        {
                            parameter.Value = Convert.ChangeType(DateTime.Now.Date, Type.GetType(parameter.Type));
                        }
                        break;

                    default:
                        parameter.Value = Convert.ChangeType(parameter.Value, Type.GetType(parameter.Type));
                        break;
                }
            }
        }

        private List<ReportParameter> DeserializeJsonParameters(string parametersJson)
        {
            return JsonConvert.DeserializeObject<List<ReportParameter>>(parametersJson) ?? new List<ReportParameter>();
        }

        private void RetrieveParameterTemplate(ReportParameter parameter)
        {
            switch (parameter.Type)
            {
                case "System.Int16":    // número entero
                case "System.Int32":    // número entero largo
                case "System.Int64":    // número entero de 64 bits
                case "System.Single":   // número simple
                case "System.Double":   // número doble
                case "System.Decimal":  // número decimal
                    parameter.TemplateName = templateNameSelectorNumber;
                    break;

                case "System.DateTime": // fecha hora
                    if (parameter.IsRangeValue)
                    { parameter.TemplateName = templateNameSelectorTimePeriod; }
                    else
                    { parameter.TemplateName = templateNameSelectorDate; }
                    break;

                case "System.String": // cadena
                    parameter.TemplateName = templateNameSelectorString;
                    break;

                case "System.Boolean": //lógico
                    parameter.TemplateName = templateNameSelectorBoolean;
                    break;

                case "System.Guid":     //guid
                    parameter.TemplateName = templateNameSelectorGuid;
                    break;

                case "Robotics.Base.employeesSelector":  //empleados
                    parameter.TemplateName = templateNameSelectorEmployees;
                    break;

                case "Robotics.Base.taskIdentifier":     //(system) TaskId
                case "Robotics.Base.passportIdentifier": // Identificador de usuario
                case "Robotics.Base.conceptIdentifier":  // Identificador de saldo
                case "Robotics.Base.causeIdentifier":  // Identificador de justificación
                case "Robotics.Base.incidenceIdentifier":  // Identificador de incidencia
                case "Robotics.Base.userFieldIdentifier":   // Identificador de ficha
                case "Robotics.Base.functionCall":       //TmpTable
                    parameter.TemplateName = templateNameHidden;
                    break;
                case "Robotics.Base.userFieldsSelectorRadioBtn": //campos de la ficha
                case "Robotics.Base.userFieldsSelector": //campos de la ficha
                case "Robotics.Base.conceptGroupsSelector": // grupos de saldos
                case "Robotics.Base.incidencesSelector":     // incidencias
                case "Robotics.Base.shiftsSelector":     // horarios
                case "Robotics.Base.holidaysSelector":     // horarios
                case "Robotics.Base.terminalsSelector":  // terminales
                case "Robotics.Base.zonesSelector":      // zonas
                    parameter.TemplateName = templateNameSelectorUniversal;
                    break;

                case "Robotics.Base.tasksSelector":   //tasks ------------------------
                    parameter.TemplateName = templateNameSelectorTasks;
                    break;

                case "Robotics.Base.conceptsSelector":   //saldos ------------------------
                    parameter.TemplateName = templateNameSelectorConcepts;
                    break;

                case "Robotics.Base.causesSelector":   //causes ------------------------
                    parameter.TemplateName = templateNameSelectorCauses;
                    break;

                case "Robotics.Base.formatSelector": // visualización y formato
                    parameter.TemplateName = templateNameSelectorFormat;
                    break;

                case "Robotics.Base.viewAndFormatSelector": // visualización y formato
                    parameter.TemplateName = templateNameSelectorViewFormat;
                    break;

                case "Robotics.Base.accessTypeSelector": // tipo de acceso
                    parameter.TemplateName = templateNameSelectorAccessType;
                    break;

                case "Robotics.Base.filterProfileTypesSelector": // perfiles de filtro
                    parameter.TemplateName = templateNameSelectorProfileTypes;
                    break;

                case "Robotics.Base.filterSelectorCausesRegistroJL": // selector justif. registro jornada laboral
                    parameter.TemplateName = templateNameSelectorCausesRegistroJL;
                    break;

                case "Robotics.Base.filterSelectorConceptsRegistroJL": // selector saldos registro jornada laboral
                    parameter.TemplateName = templateNameSelectorConceptsRegistroJL;
                    break;

                case "Robotics.Base.yearAndMonthSelector": // Mes y año
                    parameter.TemplateName = templateNameSelectorYearAndMonth;
                    break;

                case "Robotics.Base.betweenYearAndMonthSelector": // Rango Mes y año
                    parameter.TemplateName = templateNameSelectorBetweenYearAndMonth;
                    break;

                case "Robotics.Base.filterValuesSelector": // Filtrar por valores
                    parameter.TemplateName = templateNameSelectorFilterValues;
                    break;

                case "Robotics.Base.projectsVSLSelector": // Projects VSL
                    parameter.TemplateName = templateNameSelectorProjectsVSL;
                    break;

                default:
                    throw new Exception("Tipo de parámetro desconocido");
            }

            if (parameter.IsHidden)
            {
                parameter.TemplateName = templateNameHidden;
            }
        }
    }
}