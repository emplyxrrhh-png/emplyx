using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReportGenerator.Repositories;
using ReportGenerator.Services;
using Robotics.Azure;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.Base.VTEmployees.Contract;
using Robotics.Base.VTReports.Services;
using Robotics.DataLayer;
using Robotics.Security;
using Robotics.Security.Base;
using Robotics.VTBase;
using Robotics.VTBase.Extensions.VTLiveTasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace ReportGenerator.TaskManager
{
    public class ReportTaskExecution
    {
        private readonly IReportStorageService _reportStorageService;
        private readonly IReportExecutionService _reportExecutionService;
        private readonly IReportGeneratorService _reportGeneratorService;

        public ReportTaskExecution()
        {
            _reportGeneratorService = new ReportGeneratorService();
            _reportStorageService = ReportTaskExecution.CreateReportStorage();
            _reportExecutionService = new ReportExecutionService();

            roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug,
                            "CReportsDXServerNet::Start::ReportTaskExecution(" +
                            System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion +
                            ") inicialized.");
        }

        public ReportTaskExecution(IReportExecutionService reportExecutionService, IReportStorageService reportStorageService, IReportGeneratorService reportGeneratorService)
        {
            _reportExecutionService = reportExecutionService;
            _reportGeneratorService = reportGeneratorService;
            _reportStorageService = reportStorageService;
        }

        private static ReportStorageService CreateReportStorage()
        {
            var oSettings = new roSettings();
            string strPath = roTypes.Any2String(oSettings.GetVTSetting(Robotics.Base.DTOs.eKeys.DataLink));

            var blobRepo = new FileRepository(strPath);
            var executionRepo = new ReportExecutionRepository();

            return new ReportStorageService(blobRepo, executionRepo);
        }

        public BaseTaskResult ExecuteTask(roLiveTask oTask)
        {
            BaseTaskResult bolRet = new BaseTaskResult { Result = true, Description = String.Empty };

            if (oTask.Action == string.Empty) return bolRet;

            String sType = String.Empty;
            sType = oTask.Action.ToUpper();

            switch (sType)
            {
                case "GENERATEREPORTSDXTASKS":
                    bolRet = ReportTaskExecution.ExecuteScheduledTasks();
                    break;

                case "REPORTTASKDX":
                    if (roTypes.Any2Boolean(oTask.Parameters["isPlannedTask"])) bolRet = ExecutePlannedTask(oTask);
                    else bolRet = ExecuteNormalTask(oTask);

                    break;
            }

            return bolRet;
        }

        public static BaseTaskResult ExecuteScheduledTasks()
        {
            BaseTaskResult bolRet = new BaseTaskResult { Result = true, Description = String.Empty };
            try
            {
                var reportPlannedExecutionsService = new ReportPlannedExecutionService();
                List<ReportPlannedExecution> planificationsToExecute = reportPlannedExecutionsService.GetPlanificationsToExecute();

                if (planificationsToExecute != null)
                {
                    foreach (ReportPlannedExecution planification in planificationsToExecute)
                    {
                        var reportExecution = new ReportExecution()
                        {
                            Binary = null,
                            ExecutionDate = DateTime.Now,
                            ReportID = planification.ReportId,
                            PassportID = planification.PassportId,
                            Status = 1
                        };

                        System.Guid guidGenerada;

                        ReportExecutionService reService = new ReportExecutionService();
                        guidGenerada = reService.InsertExecution(reportExecution);

                        Object isPlannedTask = true;
                        Object reportId = planification.ReportId;
                        Object executionTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                        Object guid = guidGenerada.ToString();
                        Object reportParameters = planification.ParametersJson;
                        Object viewFields = planification.ViewFields;
                        Object fileExtension = planification.Format.ToString();
                        Object destination = planification.Destination;
                        Object language = planification.Language;

                        var oParameters = new roCollection();
                        oParameters.Add("isPlannedTask", ref isPlannedTask);
                        oParameters.Add("ReportId", ref reportId);
                        oParameters.Add("ExecutionTime", ref executionTime);
                        oParameters.Add("Guid", ref guid);
                        oParameters.Add("ReportParameters", ref reportParameters);
                        oParameters.Add("ViewFields", ref viewFields);
                        oParameters.Add("fileExtension", ref fileExtension);
                        oParameters.Add("destination", ref destination);
                        oParameters.Add("language", ref language);

                        var scheduledTaskState = new roLiveTaskState(planification.PassportId);
                        if (roLiveTask.CreateLiveTask(roLiveTaskTypes.ReportTaskDX, oParameters, ref scheduledTaskState) != -1)
                        {
                            DateTime? nextExecutionDate = new VisualTimeQueryDecoderService().GetNextExecutionDateFromEncodedQuery(planification.Scheduler, planification.LastExecutionDate == new DateTime(1753, 01, 01) ? null : planification.LastExecutionDate);
                            planification.NextExecutionDate = nextExecutionDate;
                            reportPlannedExecutionsService.UpdatePlannedExecution(planification);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskManager::CheckPlannedTasks :", Ex);
                bolRet.Result = false;
            }

            return bolRet;
        }

        public BaseTaskResult ExecuteNormalTask(roLiveTask oTask)
        {
            BaseTaskResult bolRet = new BaseTaskResult { Result = false, Description = String.Empty };
            try
            {
                int reportId = roTypes.Any2Integer(oTask.Parameters["ReportId"]);
                int taskId = roTypes.Any2Integer(oTask.ID);
                int userPassportId = roTypes.Any2Integer(oTask.IDPassport);
                var executionGuid = Guid.Parse(roTypes.Any2String(oTask.Parameters["Guid"]));
                string reportParameters = roTypes.Any2String(oTask.Parameters["ReportParameters"]);
                DevExpress.Utils.AzureCompatibility.Enable = true;

                Report report = new ReportService().GetDevexpressCompatibleReportById(reportId, true, reportParameters, userPassportId, taskId);
                ReportExecution reportExecution = _reportExecutionService.GetExecution(executionGuid);
                try
                {
                    ReportParameter formatParam = report.ParametersList.Find(x => x.Type == "Robotics.Base.formatSelector");
                    ReportExportExtensions extension = ReportExportExtensions.pdf;
                    if (formatParam != null && !formatParam.Value.Equals("0")) extension = (ReportExportExtensions)formatParam.Value;
                    //Si el listado de parametros contiene un parámetro llamado forceExcel a true, forzamos la ejecución con formato excel
                    ReportParameter forceExcelParam = report.ParametersList.Find(x => x.Name == "forceExcel");
                    if (forceExcelParam != null && (bool)forceExcelParam.Value) extension = ReportExportExtensions.xlsx;

                    reportExecution.Binary = _reportGeneratorService.GenerateReportForSupervisor(report, extension, userPassportId, taskId);
                    reportExecution.Format = extension;
                    reportExecution.ExecutionDate = DateTime.Now;
                    reportExecution.Status = 2;
                    reportExecution.Format = extension;
                    reportExecution.FileLink = _reportStorageService.getFileLink(reportExecution);
                    _reportStorageService.Save(reportExecution);
                }
                catch (Exception ex)
                {
                    reportExecution.Binary = Array.Empty<byte>();
                    reportExecution.Status = 3;
                    _reportStorageService.Save(reportExecution);

                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "ReportTaskExecution::ExecuteNormalTask :", ex);
                }

                if (reportExecution.Status == 2)
                {
                    bolRet.Result = true;
                    bolRet.Description = reportId.ToString();
                }
                else
                {
                    bolRet.Result = false;
                    bolRet.Description = oTask.State.ErrorText;
                }
            }
            catch (Exception Ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::ExecuteNormalTask :", Ex);
            }

            return bolRet;
        }

        private bool IsValidJsonObject(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                return false;

            jsonString = jsonString.Trim();
            if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
            {
                try
                {
                    JObject.Parse(jsonString);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }
            return false;
        }

        public BaseTaskResult ExecutePlannedTask(roLiveTask oTask)
        {
            BaseTaskResult bolRet = new BaseTaskResult { Result = false, Description = String.Empty };
            int passportId = oTask.IDPassport;

            try
            {
                int reportId = roTypes.Any2Integer(oTask.Parameters["ReportId"]);
                int taskId = roTypes.Any2Integer(oTask.ID);
                string reportParameters = roTypes.Any2String(oTask.Parameters["ReportParameters"]);
                //Get actual Report Date
                reportParameters = GetRealReportDate(reportParameters);

                string strDestination = roTypes.Any2String(oTask.Parameters["destination"]);
                string strLanguage = roTypes.Any2String(oTask.Parameters["language"]);
                string strSubject = String.Empty;
                string viewFieldsString = roTypes.Any2String(oTask.Parameters["ViewFields"]);

                if (IsValidJsonObject(viewFieldsString))
                {
                    try
                    {
                        JObject viewFields = JObject.Parse(viewFieldsString);
                        strSubject = viewFields["description"]?.ToString() ?? string.Empty;
                    }
                    catch (JsonReaderException ex)
                    {
                        roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::ExecutePlannedTask: Error parsing ViewFields JSON", ex);
                    }
                }
                else
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roWarning, "ReportTaskExecution::ExecutePlannedTask: ViewFields parameter is not a valid JSON object.");
                }

                var destination = new ReportPlannedDestination();
                if (strDestination != null)
                {
                    destination = (ReportPlannedDestination)roJSONHelper.DeserializeNewtonSoft(strDestination, destination.GetType());

                    var executionGuid = Guid.Parse(roTypes.Any2String(oTask.Parameters["Guid"]));
                    ReportExecution reportExecution = _reportExecutionService.GetExecution(executionGuid);

                    try
                    {
                        Enum.TryParse(roTypes.Any2String(oTask.Parameters["fileExtension"]), out ReportExportExtensions fileExtension);
                        Enum.TryParse(destination.Type, out ReportDestinationType oDestinationType);

                        SmtpClient oSendMail = null;
                        string sourceMail = String.Empty;
                        var oCompanyConfigurationRepository = new roConfigRepository();
                        roAzureConfig emailCache = oCompanyConfigurationRepository.GetConfigParameter(roConfigParameter.email);

                        roMailConfig oConf = (roMailConfig)Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(emailCache.value, typeof(roMailConfig));
                        sourceMail = oConf.MailAccount;
                        oSendMail = Robotics.Mail.SendMail.GetInstance(oConf);

                        if (oDestinationType == ReportDestinationType.supervisors)
                        {

                            int[] idsSupervisors = Array.ConvertAll(destination.Value.Split(','), int.Parse);

                            foreach (int idExecutePassport in idsSupervisors)
                            {
                                try
                                {
                                    roSecurityState secState = new roSecurityState(idExecutePassport);
                                    roPassport passport = roPassportManager.GetPassport(idExecutePassport, LoadType.Passport, ref secState);
                                    if (passport != null && passport.IsActivePassport && passport.IsSupervisor && (!passport.IDEmployee.HasValue || (passport.IDEmployee.HasValue && roPassportManager.GetEmployeeActiveContract(passport).Rows.Count > 0)))
                                    {
                                        Report report = new ReportService().GetDevexpressCompatibleReportById(reportId, true, reportParameters, idExecutePassport, taskId);
                                        ReportParameter formatParam = report.ParametersList.Find(x => x.Type == "Robotics.Base.formatSelector");
                                        fileExtension = ReportExportExtensions.pdf;
                                        if (formatParam != null && !formatParam.Value.Equals("0")) fileExtension = (ReportExportExtensions)formatParam.Value;
                                        //Si el listado de parametros contiene un parámetro llamado forceExcel a true, forzamos la ejecución con formato excel
                                        ReportParameter forceExcelParam = report.ParametersList.Find(x => x.Name == "forceExcel");
                                        if (forceExcelParam != null && (bool)forceExcelParam.Value) fileExtension = ReportExportExtensions.xlsx;

                                        reportExecution.Binary = _reportGeneratorService.GenerateReportForSupervisor(report, fileExtension, idExecutePassport, taskId, strLanguage);
                                        reportExecution.Format = fileExtension;
                                        if (string.IsNullOrEmpty(strSubject))
                                        {
                                            strSubject = report.Name;
                                        }
                                        if (reportExecution.Binary != null && reportExecution.Binary.Length > 0)
                                        {
                                            var languageKey = passport?.Language.Key ?? "ESP";
                                            this.sendMail("", idExecutePassport, LoadType.Passport, strSubject, report.Name, reportExecution.Binary, fileExtension, oSendMail, sourceMail, languageKey);
                                        }
                                    }
                                    else
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "ReportTaskExecution::ExecutePlannedTask : Unexistent destination passport " + idExecutePassport + " or passport belongs to employee supervisor without active contract. Report ID = " + reportId);
                                        bolRet.Description = string.IsNullOrEmpty(bolRet.Description) ? "Supervisor/s ignored: " : bolRet.Description + idExecutePassport + ",";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::ExecutePlannedTask: Exception generating report for supervisor " + idExecutePassport + ". Report ID = " + reportId, ex);
                                    bolRet.Description = string.IsNullOrEmpty(bolRet.Description) ? "Some or all supervisors destinations missing due to exception " : bolRet.Description + " - " + " and some other supervisor destinations missing due to exception ";
                                }
                            }
                        }
                        else if (oDestinationType == ReportDestinationType.empemail || oDestinationType == ReportDestinationType.empdocument)
                        {
                            Report report = new ReportService().GetDevexpressCompatibleReportById(reportId, true, reportParameters, passportId, taskId);
                            fileExtension = ReportExportExtensions.pdf;
                            ReportParameter formatParam = report.ParametersList.Find(x => x.Type == "Robotics.Base.formatSelector");
                            if (formatParam != null && !formatParam.Value.Equals("0")) fileExtension = (ReportExportExtensions)formatParam.Value;
                            //Si el listado de parametros contiene un parámetro llamado forceExcel a true, forzamos la ejecución con formato excel
                            ReportParameter forceExcelParam = report.ParametersList.Find(x => x.Name == "forceExcel");
                            if (forceExcelParam != null && (bool)forceExcelParam.Value) fileExtension = ReportExportExtensions.xlsx;

                            var exportedFilesList = _reportGeneratorService.GenerateReportForEmployee(report, fileExtension, passportId, taskId, strLanguage);
                            if (string.IsNullOrEmpty(strSubject))
                            {
                                strSubject = report.Name;
                            }
                            if (exportedFilesList.Count > 0)
                            {
                                var oContractState = new roContractState(passportId);
                                foreach ((int employeeId, byte[] reportBytes) exportedFileRow in exportedFilesList)
                                {
                                    if (roContract.GetActiveContract(exportedFileRow.employeeId, ref oContractState) != null)
                                    {
                                        if (exportedFileRow.reportBytes != null && exportedFileRow.reportBytes.Length > 0)
                                        {
                                            if (reportExecution.Binary == null)
                                            {
                                                reportExecution.Binary = exportedFileRow.reportBytes;
                                            }

                                            if (oDestinationType == ReportDestinationType.empemail)
                                            {
                                                roSecurityState secState = new roSecurityState(-1);
                                                roPassport passportEmployee = roPassportManager.GetPassport(exportedFileRow.employeeId, LoadType.Employee, ref secState);

                                                var languageKey = passportEmployee?.Language.Key ?? "ESP";
                                                this.sendMail("", exportedFileRow.employeeId, LoadType.Employee, strSubject, report.Name, exportedFileRow.reportBytes, fileExtension, oSendMail, sourceMail, languageKey);
                                            }
                                            else if (oDestinationType == ReportDestinationType.empdocument)
                                            {
                                                this.saveToDocument(exportedFileRow.employeeId, roTypes.Any2Integer(destination.Value), strSubject, report.Name, exportedFileRow.reportBytes, fileExtension, passportId);
                                            }
                                            else
                                            {
                                                roLog.get_GetInstance().logMessage(roLog.EventType.roWarning, "ReportTaskExecution::ExecutePlannedTask : DestinationType:" + oDestinationType + " no es ni empEmail ni empDocument. ReportID:" + reportId);
                                            }
                                        }
                                        else
                                        {
                                            roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "ReportTaskExecution::ExecutePlannedTask : No hay datos a exportar para el empleado con ID:" + exportedFileRow.employeeId + ". ReportID:" + reportId);
                                        }
                                    }
                                    else
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roWarning, "ReportTaskExecution::ExecutePlannedTask : El empleado con ID:" + exportedFileRow.employeeId + " no tiene contrato activo y no se le enviará el informe ID:" + reportId);
                                    }
                                }
                            }
                            else
                            {
                                roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, "ReportTaskExecution::ExecutePlannedTask : exportedFilesList vacío para el empleado con passportId:" + passportId + ". ReportID:" + reportId);
                            }
                        }
                        else if (oDestinationType == ReportDestinationType.email)
                        {
                            Report report = new ReportService().GetDevexpressCompatibleReportById(reportId, true, reportParameters, passportId, taskId);

                            ReportParameter formatParam = report.ParametersList.Find(x => x.Type == "Robotics.Base.formatSelector");
                            fileExtension = ReportExportExtensions.pdf;
                            if (formatParam != null && !formatParam.Value.Equals("0")) fileExtension = (ReportExportExtensions)formatParam.Value;
                            //Si el listado de parametros contiene un parámetro llamado forceExcel a true, forzamos la ejecución con formato excel
                            ReportParameter forceExcelParam = report.ParametersList.Find(x => x.Name == "forceExcel");
                            if (forceExcelParam != null && (bool)forceExcelParam.Value) fileExtension = ReportExportExtensions.xlsx;

                            reportExecution.Binary = _reportGeneratorService.GenerateReport(report, fileExtension, passportId, taskId, strLanguage);
                            reportExecution.Format = fileExtension;
                            if (string.IsNullOrEmpty(strSubject))
                            {
                                strSubject = report.Name;
                            }
                            if (reportExecution.Binary != null && reportExecution.Binary.Length > 0)
                            {
                                this.sendMail((string)destination.Value, -1, LoadType.Employee, strSubject, report.Name, reportExecution.Binary, fileExtension, oSendMail, sourceMail, oTask.State.Language.LanguageKey);
                            }
                        }
                        else
                        {
                            roLog.get_GetInstance().logMessage(roLog.EventType.roWarning, "ReportTaskExecution::ExecutePlannedTask : DestinationType: " + oDestinationType + " no controlado. ReportID:" + reportId);
                        }

                        reportExecution.ExecutionDate = DateTime.Now;
                        reportExecution.Status = 2;
                        reportExecution.FileLink = _reportStorageService.getFileLink(reportExecution);
                        _reportStorageService.Save(reportExecution);
                    }
                    catch (Exception ex)
                    {
                        reportExecution.Binary = Array.Empty<byte>();
                        reportExecution.Status = 3;
                        bolRet.Description = string.IsNullOrEmpty(bolRet.Description) ? "Exception (see detail in Insights) " : "Exception (see detail in Insights) " + " - " + bolRet.Description;
                        _reportStorageService.Save(reportExecution);
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "ReportTaskExecution::ExecutePlannedTask :", ex); 
                    }

                    if (reportExecution.Status == 2)
                    {
                        bolRet.Result = true;
                        bolRet.Description = string.IsNullOrEmpty(bolRet.Description) ? reportId.ToString() : reportId.ToString() + " - " + bolRet.Description;
                    }
                    else
                    {
                        bolRet.Result = false;
                        bolRet.Description = string.IsNullOrEmpty(bolRet.Description) ? oTask.State.ErrorText : bolRet.Description + " - " + oTask.State.ErrorText;
                    }
                }
                else
                {
                    bolRet.Result = false;
                    bolRet.Description = "Incorrect destination";
                }
            }
            catch (Exception Ex)
            {
                // Stop
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::ExecutePlannedTask :", Ex); 
            }

            return bolRet;
        }

        //Obtener la fecha real de un reporte planificado segun hayas seleccionado Mes anterior, actual o siguiente.
        //Si estamos en Diciembre 23 y esta planificado para el mes anterior, damos Noviembre 23, actual Diciembre 23 o siguiente Enero 2024
        private string GetRealReportDate(string reportParameters)
        {
            try
            {
                JArray reportParametersJArray = JArray.Parse(reportParameters);
                JObject plannedDataJObject = null;
                foreach (JObject jsonObject in reportParametersJArray)
                {
                    if (jsonObject["Name"].ToString() == "YearMonth")
                    {
                        plannedDataJObject = jsonObject;
                        break;
                    }
                }
                if (plannedDataJObject != null)
                {
                    if (plannedDataJObject["Value"]["month"].Type == JTokenType.Integer || plannedDataJObject["Value"]["month"].Type == JTokenType.String) //13: CurrentMonth, 14: PreviousMonth, 15: NextMonth
                    {
                        DateTime resultDate = DateTime.Now;
                        switch ((int)plannedDataJObject["Value"]["month"])
                        {
                            case 14:
                                resultDate = resultDate.AddMonths(-1);
                                break;

                            case 15:
                                resultDate = resultDate.AddMonths(1);
                                break;
                        }

                        foreach (JObject jsonObject in reportParametersJArray)
                        {
                            switch (jsonObject["Name"].ToString())
                            {
                                case "YearMonth":
                                    jsonObject["Value"]["year"] = resultDate.Year;
                                    break;

                                case "month":
                                    jsonObject["Value"] = resultDate.Month;
                                    break;

                                case "year":
                                    jsonObject["Value"] = resultDate.Year;
                                    break;
                            }
                        }
                        reportParameters = reportParametersJArray.ToString();
                    }
                }

                return reportParameters;
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::GetRealReportData: ", ex);
                return reportParameters;
            }
        }

        private void sendMail(string strEmails, int subjectId, LoadType loadType, string subject, string fileName, byte[] file, ReportExportExtensions extension, SmtpClient sendMail, String sourceMail, string strLanguage)
        {
            try
            {
                string destination = string.Empty;
                if (strEmails == string.Empty)
                {
                    roSecurityState secState = new roSecurityState(-1);
                    roPassportTicket passport = roPassportManager.GetPassportTicket(subjectId, loadType, ref secState);


                    if (passport.Email != string.Empty & !passport.IDEmployee.HasValue)
                    {
                        destination = passport.Email;
                    }
                    else
                    {
                        var userfieldSQL = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'";
                        string strEmailUsrField = roTypes.Any2String(AccessHelper.ExecuteScalar(userfieldSQL));

                        if (strEmailUsrField.Length > 0 & passport.IDEmployee.HasValue)
                        {
                            // Miramos el valor del campo de la ficha del empleado
                            var strSQL = "@DECLARE# @Date smalldatetime SET @Date = " + roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime() + "  @SELECT# * FROM GetEmployeeUserFieldValue(" + roTypes.Any2String(passport.IDEmployee) + ",'" + strEmailUsrField + "', @Date)";
                            DataTable tbs = AccessHelper.CreateDataTable(strSQL);
                            if (tbs is object && tbs.Rows.Count > 0)
                            {
                                destination = roTypes.Any2String(tbs.Rows[0]["Value"]);
                            }
                        }
                    }
                }
                else
                {
                    destination = strEmails;
                }
                var language = new Robotics.VTBase.roLanguage();
                language.SetLanguageReference("LiveReports", strLanguage);
                var emailBody = language.Translate("Reports.Confidential", "Reports") ?? "";

                if (destination != "") Robotics.Mail.SendMail.SendMail(destination, subject, emailBody, fileName + "." + extension.ToString(), file, sendMail, sourceMail, RoAzureSupport.GetCompanyName(), "iso-8859-8"); //Añadimos éste encoding porque hay problemas con los acentos en reports planificados
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::sendMail :", ex);
            }
        }

        private void saveToDocument(int employeeID, int documentTemplateID, string subject, string fileName, byte[] file, ReportExportExtensions extension, int idPassport)
        {
            try
            {
                var oDocManager = new Robotics.Base.VTDocuments.roDocumentManager(new Robotics.Base.VTDocuments.roDocumentState(idPassport));

                var oDocTemplate = oDocManager.LoadDocumentTemplate(documentTemplateID);

                var newDoc = new roDocument()
                {
                    Id = -1,
                    Title = fileName,
                    IdEmployee = employeeID,
                    IdCompany = -1,
                    DocumentTemplate = oDocTemplate,
                    DocumentType = "." + extension.ToString(),
                    Document = file,
                    DeliveredDate = DateTime.Now,
                    DeliveryChannel = "VisualTime Report Manager",
                    DeliveredBy = "",
                    Status = DocumentStatus.Pending,
                    LastStatusChange = DateTime.Now,
                    BeginDate = new DateTime(1900, 1, 1, 0, 0, 0),
                    EndDate = new DateTime(2079, 1, 1, 0, 0, 0)
                };

                oDocManager.SaveDocument(ref newDoc);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportTaskExecution::saveToDocument :", ex);
            }
        }
    }
}