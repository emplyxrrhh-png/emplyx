using DevExpress.XtraReports.UI;
using Robotics.Base;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public interface IReportParameterService
    {
        List<ReportParameter> GetDevexpressCompatibleReportParameters(string parametersJson, int passportId, int taskId);

        string GetLastParametersFromReport(int reportId, int passportId);

        void SetParameterValuesToXtraReport(Report report, XtraReport xtraReport, int taskId);

        void SetParameterValuesToXtraReportEmployee(Report report, XtraReport xtraReport, int employeeId, int taskId);

        bool SaveLastParameters(int reportId, int passportId, string lastParameters);

        bool DeleteAllLastParameters(int reportId);

        bool DeleteRowsFromTemporalTables(IEnumerable<ReportParameter> parametersList, int taskId);
    }
}