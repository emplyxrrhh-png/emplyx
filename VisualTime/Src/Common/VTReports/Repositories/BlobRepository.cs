using Robotics.Azure;
using Robotics.Base;
using Robotics.Base.DTOs;
using System.IO;

namespace ReportGenerator.Repositories
{
    public class BlobRepository : IBlobRepository
    {
        private readonly string companyid;

        public BlobRepository(string companyID)
        {
            companyid = companyID;
        }

        public void DeleteBlob(ReportExecution reportFile)
        {
            RoAzureSupport.DeleteFileFromBlob(reportFile.BlobLink, roLiveQueueTypes.reports, companyid);
        }

        public void SaveBlob(ReportExecution reportFile)
        {
            using (var stream = new MemoryStream(reportFile.Binary))

                RoAzureSupport.UploadStream2Blob(stream, reportFile.BlobLink, roLiveQueueTypes.reports, companyid);
        }

        public string GetFileLink(ReportExecution reportFile)
        {
            return reportFile.BlobLink;
        }
    }

    public class FileRepository : IBlobRepository
    {
        private readonly string _path;

        public FileRepository(string path)
        {
            _path = path;
        }

        public void DeleteBlob(ReportExecution reportFile)
        {
            var fullFileName = System.IO.Path.Combine(_path, System.IO.Path.GetFileName(reportFile.BlobLink));
            System.IO.File.Delete(fullFileName);
        }

        public void SaveBlob(ReportExecution reportFile)
        {
            using (var stream = new MemoryStream(reportFile.Binary))
            {
                var fullFileName = System.IO.Path.Combine(_path, System.IO.Path.GetFileName(reportFile.BlobLink));

                using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        public string GetFileLink(ReportExecution reportFile)
        {
            return System.IO.Path.Combine(_path, System.IO.Path.GetFileName(reportFile.BlobLink));
        }
    }
}