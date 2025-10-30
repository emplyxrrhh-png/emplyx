using Robotics.Base;

namespace ReportGenerator.Repositories
{
    public interface IBlobRepository
    {
        void DeleteBlob(ReportExecution reportFile);

        void SaveBlob(ReportExecution reportFile);

        string GetFileLink(ReportExecution reportFile);
    }
}