using ReportGenerator.Repositories;

namespace ReportGenerator.Services
{
    public class PassportService : IPassportService
    {
        private IPassportRepository passportRepository;

        #region Constructor

        public PassportService()
        {
            this.passportRepository = new PassportRepository();
        }

        #endregion Constructor

        public string GetUsernameByPassportId(int passportId)
        {
            return (passportRepository.Get(passportId)?.Name.ToString()) ?? "";
        }

        public bool IsPassportIdARoboticsUser(int passportId)
        {
            return Robotics.Security.Base.roPassportManager.IsRoboticsUserOrConsultant(passportId);    
        }
    }
}