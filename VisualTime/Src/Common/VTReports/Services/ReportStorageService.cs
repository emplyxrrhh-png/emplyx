using ReportGenerator.Repositories;
using Robotics.Base;
using Robotics.VTBase;
using System;

namespace ReportGenerator.Services
{
    public class ReportStorageService : IReportStorageService
    {
        private readonly IBlobRepository _blobRepository;
        private readonly IReportExecutionRepository _reportExecutionRepository;

        /// <summary>
        /// Creates a ReportStorageService
        /// throws null exception if it's parameters are null
        /// </summary>
        /// <param name="blobRepository"></param>
        /// <param name="reportFileRepository"></param>
        public ReportStorageService(IBlobRepository blobRepository, IReportExecutionRepository reportFileRepository)
        {
            if (blobRepository != null)
            {
                this._blobRepository = blobRepository;
            }
            else
            {
                throw new ArgumentNullException(nameof(blobRepository));
            }

            if (reportFileRepository != null)
            {
                this._reportExecutionRepository = reportFileRepository;
            }
            else
            {
                throw new ArgumentNullException(nameof(reportFileRepository));
            }
        }

        public Guid Save(ReportExecution reportFile)
        {
            Guid generatedGuid = Guid.Empty;

            try
            {
                _blobRepository.SaveBlob(reportFile);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportStorageService::Save:: Error saving on blobRepository. ", ex);
                throw new Exception("ReportStorageService::Save:: Error saving on blobRepository. ", ex);
            }

            try
            {
                if (reportFile.Guid == Guid.Empty)
                {
                    generatedGuid = _reportExecutionRepository.Insert(reportFile);
                }
                else
                {
                    _reportExecutionRepository.Update(reportFile);
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportStorageService::Save:: Error saving on reportFileRepository. Attempting to delete blob.", ex);
                try
                {
                    _blobRepository.DeleteBlob(reportFile);
                }
                catch (Exception deleteEx)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportStorageService::Save:: Error deleting blob after failed reportFileRepository operation.", deleteEx);
                    throw new Exception("ReportStorageService::Save:: Error deleting blob after failed reportFileRepository operation", deleteEx);
                }
                throw new Exception("ReportStorageService::Save:: Error saving on reportFileRepository", ex);
            }

            return generatedGuid;
        }

        public string getFileLink(ReportExecution reportFile)
        {
            var fileLink = string.Empty;
            try
            {
                fileLink = _blobRepository.GetFileLink(reportFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving on blobRepository", ex);
            }

            return fileLink;
        }
    }
}