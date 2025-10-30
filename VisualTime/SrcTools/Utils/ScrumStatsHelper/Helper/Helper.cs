using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumStatsHelper
{
    internal class Helper
    {
        static public Tuple<DateTime, DateTime> GetSprintDates(int currentSprint)
        {
            // Calculo Sprint actual
            // El 33 empezó el 2023-06-28
            DateTime startDate = new DateTime(2023, 06, 28);
            DateTime sprintStart = startDate.AddDays((currentSprint - 33) * 14);
            DateTime sprintEnd = sprintStart.AddDays(13);

            return new Tuple<DateTime, DateTime>(sprintStart, sprintEnd);
        }

        static public int GetSprintNumberOnDate(DateTime date)
        {
            // Calculo Sprint actual
            // El 33 empezó el 2023-06-28
            DateTime startDate = new DateTime(2023, 06, 28);
            int totalDays = (date - startDate).Days;
            int totalSprints = totalDays / 14;
            int currentSprint = totalSprints + 33;
            return currentSprint;
        }
    }
}
