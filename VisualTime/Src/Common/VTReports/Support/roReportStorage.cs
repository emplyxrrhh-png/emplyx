using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.XtraReports.UI;
using ReportGenerator.Services;
using Robotics.Base;
using Robotics.Base.VTReports.Services;
using System.Data.SqlClient;
using System.IO;

namespace VTReports.Support
{
    public class roReportStorage : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        private int passportId;

        private IReportService reportService;

        public roReportStorage(int passportId)
        {
            this.passportId = passportId;

            this.reportService = new ReportService();
        }

        public override byte[] GetData(string url)
        {
            // Get the report data from the storage.
            Report reportLayout = reportService.GetReportByName(url, this.passportId, ReportPermissionTypes.Execute, new Domain.roReportsState(passportId), false);

            XtraReport report = XtraReport.FromStream(new MemoryStream(reportLayout.LayoutXMLBinary));

            if (report.DataSource != null)
            {
                var connectionInfo = new SqlConnectionStringBuilder(Robotics.DataLayer.AccessHelper.GetConectionString());
                var connectionParameters = new MsSqlConnectionParameters(connectionInfo.DataSource, connectionInfo.InitialCatalog, connectionInfo.UserID, connectionInfo.Password, MsSqlAuthorizationType.SqlServer);
                ((DevExpress.DataAccess.Sql.SqlDataSource)report.DataSource).ConnectionParameters = connectionParameters;
            }

            new ReportTranslationService().TranslateXtraReport(report, url, this.passportId);

            reportLayout.LayoutXMLBinary = GetCustomData(report);

            return reportLayout.LayoutXMLBinary;
        }

        private byte[] GetCustomData(XtraReport report)
        {
            byte[] data;

            using (var ms = new MemoryStream())
            {
                report.SaveLayoutToXml(ms);
                data = ms.GetBuffer();
            }

            return data;
        }
    }
}