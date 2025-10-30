using ReportGenerator.Domain;
using System.Collections.Generic;

namespace ReportGenerator.Repositories
{
    public interface IRoTaskRepository
    {
        roTask GetLifeTask(int id);

        void UpdateLifeTask(roTask task);

        IEnumerable<roTask> GetAllReportTasks();
    }
}