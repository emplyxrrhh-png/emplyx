using System.Collections.Generic;
using VTReports.Domain;

namespace Robotics.Base.VTReports.Services
{
    internal interface IReportService
    {
        Report GetReportById(int reportId, int passportId, roReportsState oState = null, bool bAudit = false);

        Report GetReportByName(string reportName, int passportId, ReportPermissionTypes action = ReportPermissionTypes.Update, roReportsState oState = null, bool bAudit = false);

        Report GetDevexpressCompatibleReportById(int reportId, bool isSupervisorReport, string reportParameters, int passportId, int taskId);

        List<Report> GetReportsByPassportIdSimplified(int passportId, roReportsState oState = null, bool bAudit = false);

        int? GetReportCreatorPassportId(int reportId);

        bool InsertReport(Report report, roReportsState oState = null, bool bAudit = false);

        bool UpdateReport(Report report, roReportsState oState = null, bool bAudit = false);

        bool UpdateReportCategories(int reportId, List<(int, int)> reportCategories, int passportId, roReportsState oState = null, bool bAudit = false);

        bool DeleteReport(int reportId, int passportId, roReportsState oState = null, bool bAudit = false);

        bool CopyReport(int reportId, string newReportName, int passportId, roReportsState oState = null, bool bAudit = false);
    }
}