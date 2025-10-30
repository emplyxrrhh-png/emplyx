using DapperExtensions;
using ReportGenerator.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReportGenerator.Repositories
{
    public class roTaskRepository : IRoTaskRepository
    {
        private readonly DatabaseConectionInfo _conectionInfo;
        private readonly string ReportTaskAction = Robotics.Base.VTLiveTasks.roLiveTask.roLiveTaskTypes.ReportTaskDX.ToString().ToUpper();

        public roTaskRepository(DatabaseConectionInfo conectionInfo)
        {
            _conectionInfo = conectionInfo ?? throw new ArgumentNullException(nameof(conectionInfo));
        }

        public IEnumerable<roTask> GetAllReportTasks()
        {
            using (IDbConnection db = new SqlConnection(_conectionInfo.GetConnectionSrtring()))
            {
                var predicate = Predicates.Field<roTask>(f => f.Action, Operator.Eq, ReportTaskAction);
                return db.GetList<roTask>(predicate);
            }
        }
        public roTask GetLifeTask(int id)
        {
            using (IDbConnection db = new SqlConnection(_conectionInfo.GetConnectionSrtring()))
            {
                return db.Get<roTask>(id);
            }
        }
        public int InsertLiveTask(roTask task)
        {
            using (IDbConnection db = new SqlConnection(_conectionInfo.GetConnectionSrtring()))
            {
                return db.Insert(task);
            }
        }

        public void UpdateLifeTask(roTask task)
        {
            using (IDbConnection db = new SqlConnection(_conectionInfo.GetConnectionSrtring()))
            {
                db.Update(task);
            }
        }
    }
}