using Robotics.Base;

namespace ReportGenerator.Services
{
    public interface IReportPermissionService
    {
        bool GetPermissionOverReportAction(int passportId, int reportId, ReportPermissionTypes action);
        bool GetPermissionOverReportAction(int passportId, Report report, ReportPermissionTypes action);
        bool GetGeneralReportPermission(int passportId, ReportPermissionTypes action);

        ReportPermissions GetPermissionsOverReport(Report report, int passportId);
    }
}