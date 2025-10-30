namespace ReportGenerator.Services
{
    public interface IPassportService
    {
        string GetUsernameByPassportId(int passportId);

        bool IsPassportIdARoboticsUser(int passportId);
    }
}