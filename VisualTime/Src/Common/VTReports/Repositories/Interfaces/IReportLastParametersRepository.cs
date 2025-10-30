namespace ReportGenerator.Repositories
{
    public interface IReportLastParametersRepository
    {
        string Get(int reportId, int passportId);

        bool Insert(int reportId, int passportId, string lastParameters);

        bool Update(int reportId, int passportId, string lastParameters);

        bool Delete(int reportId, int passportId);

        bool DeleteAll(int reportId);
    }
}