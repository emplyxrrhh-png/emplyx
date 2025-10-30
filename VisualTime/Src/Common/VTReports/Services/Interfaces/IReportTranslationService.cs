using DevExpress.XtraReports.UI;

namespace ReportGenerator.Services
{
    public interface IReportTranslationService
    {
        void setTranslateLanguage(string language, int passportId);

        void unsetTranslateLanguage();

        void TranslateXtraReport(XtraReport report, string reportName, int passportId);
    }
}