using System;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public interface IVisualTimeQueryDecoderService
    {
        IEnumerable<int> GetEmployeesFromEncodedQuery(String parameterValue, int passportId, DateTime? iDate, DateTime? eDate);

        DateTime[] GetDateFromEncodedQuery(String parameterValue);

        DateTime[] GetDatePeriodFromEncodedQuery(String parameterValue);

        DateTime? GetNextExecutionDateFromEncodedQuery(String parameterValue, DateTime? lastExecutionDate);

        IEnumerable<int> GetTasksFromEncodedQuery(String parameterValue);
    }
}