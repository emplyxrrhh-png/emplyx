using DevExpress.CodeParser;
using DevExpress.Xpo.Helpers;
using Robotics.Base.VTBusiness.Common;
using Robotics.Base.VTBusiness.Group;
using Robotics.Base.VTBusiness.Support;
using Robotics.Base.VTBusiness.Task;
using Robotics.Base.VTEmployees.Employee;
using Robotics.Security;
using Robotics.VTBase;
using System;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public class VisualTimeQueryDecoderService : IVisualTimeQueryDecoderService
    {
        #region Constructor

        public VisualTimeQueryDecoderService()
        {
        }

        #endregion Constructor

        public IEnumerable<int> GetEmployeesFromEncodedQuery(String parameterValue, int passportId, DateTime? iDate, DateTime? eDate)
        {
            

            
            String[] configuration = (parameterValue ?? String.Empty).Split('@');
            String employeeFilter = configuration[0];
            String filter = (configuration.Length > 1 ? configuration[1] : String.Empty);
            String filterUser = (configuration.Length > 2 ? configuration[2] : String.Empty);


            List<int> employeesList = roSelector.GetEmployeeList(passportId, "Employees", "U", null,
                                                                                employeeFilter, filter, filterUser, false, iDate, eDate);

            return employeesList;
        }

        public IEnumerable<int> GetTasksFromEncodedQuery(String parameterValue)
        {
            //Filtramos las tareas para obtener ids de las tareas a partir de ids y nombres de proyecto.
            return roTask.GetTasksFiltered(parameterValue);
        }

        public DateTime[] GetDateFromEncodedQuery(String parameterValue)
        {
            Robotics.Base.DTOs.roDateTimePeriod datetimeDecodedPeriod = roSupport.String2DateTime(parameterValue);

            return new DateTime[] { datetimeDecodedPeriod.BeginDateTimePeriod, datetimeDecodedPeriod.BeginDateTimePeriod };
        }

        public DateTime[] GetDatePeriodFromEncodedQuery(String parameterValue)
        {
            Robotics.Base.DTOs.roDateTimePeriod datetimeDecodedPeriod = roSupport.String2DateTimePeriod(parameterValue);

            return new DateTime[] { datetimeDecodedPeriod.BeginDateTimePeriod, datetimeDecodedPeriod.EndDateTimePeriod };
        }

        public DateTime? GetNextExecutionDateFromEncodedQuery(String parameterValue, DateTime? lastExecutionDate)
        {
            Exception resultException = null;
            DateTime? nextExecutionDate = roLiveSupport.GetNextRun(
                                            roReportSchedulerScheduleManager.retScheduleString(roReportSchedulerScheduleManager.Load(parameterValue)),
                                            lastExecutionDate,
                                            ref resultException
                                         );

            return nextExecutionDate;
        }
    }
}