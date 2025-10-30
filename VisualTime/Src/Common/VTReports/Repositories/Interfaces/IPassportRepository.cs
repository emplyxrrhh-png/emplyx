using Robotics.Base;

namespace ReportGenerator.Repositories
{
    public interface IPassportRepository
    {
        ReportPassport Get(int passportId);
    }
}