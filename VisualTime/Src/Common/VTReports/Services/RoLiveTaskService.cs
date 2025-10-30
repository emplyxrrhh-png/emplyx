using ReportGenerator.Domain;
using ReportGenerator.Repositories;
using Robotics.Base;
using System;

namespace ReportGenerator.Services
{
    public class RoLiveTaskService
    {
        private readonly IRoTaskRepository repo;
        private readonly IReportExecutionRepository repExeRepo;
        private readonly IReportGenerationService reportGenerationService;
        private readonly IReportStorageService reportStorageService;

        public RoLiveTaskService(IRoTaskRepository repo, IReportExecutionRepository repExeRepo, IReportStorageService reportStorageService, IReportGenerationService reportGenerationService)
        {
            this.reportGenerationService = reportGenerationService ?? throw new ArgumentNullException(nameof(reportGenerationService));
            this.reportStorageService = reportStorageService ?? throw new ArgumentNullException(nameof(reportStorageService));
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.repExeRepo = repExeRepo ?? throw new ArgumentNullException(nameof(repExeRepo));
        }

        //public void ExecuteTask(int id)
        //{
        //    var task = StartTask(id);
        //    try
        //    {
        //        var taskParameters = task.GetParameters();
        //        var layoutID = int.Parse((string)taskParameters["LayoutId"]);
        //        var memoryFileReport = reportGenerationService.getReportPDF(layoutID);
        //        var reportFile = new ReportExecution
        //        {
        //            Binary = memoryFileReport.binary,
        //            ExecutionTime = DateTime.Now,
        //            LayoutID = layoutID,
        //            PassportID = task.IDPassport,
        //        };
        //        reportStorageService.Save(reportFile);
        //        task.Status = 2;
        //        task.Progress = 100;
        //        task.EndDate = DateTime.Now;
        //    }
        //    catch (Exception e)
        //    {
        //        task.ErrorCode = e.Message;
        //        task.Status = 3;
        //    }
        //    finally
        //    {
        //        repo.UpdateLifeTask(task);
        //    }
        //}

        public void ExecuteTask(int id)
        {
            var task = repo.GetLifeTask(id);
            var taskParameters = task.GetParameters();

            var layoutID = int.Parse((string)taskParameters["LayoutId"]);

            var reportExecution = new ReportExecution
            {
                Binary = { },
                ExecutionDate = DateTime.Now,
                LayoutID = layoutID,
                PassportID = task.IDPassport,
                Status = 1
            };

            StartTask(ref task, ref reportExecution);

            try
            {

                var memoryFileReport = reportGenerationService.getReportPDF(layoutID);

                reportExecution.Binary = memoryFileReport.binary;
                reportExecution.ExecutionDate = DateTime.Now;
                reportExecution.Status = 2;

                reportStorageService.Save(reportExecution);
                task.Status = 2;
                task.Progress = 100;
                task.EndDate = DateTime.Now;
            }
            catch (Exception e)
            {
                task.ErrorCode = e.Message;
                task.Status = 3;
                reportExecution.Status = 3;
            }
            finally
            {
                repExeRepo.Update(reportExecution);
                repo.UpdateLifeTask(task);
            }
        }

        public void StartTask(ref roTask task, ref ReportExecution reportExecution)
        {
            //La marcamos como empezada
            task.Status = 1;
            repo.UpdateLifeTask(task);
            repExeRepo.Insert(reportExecution);
            task.ExecutionDate = DateTime.Now;
        }
    }
}