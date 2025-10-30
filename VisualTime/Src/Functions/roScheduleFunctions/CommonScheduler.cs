using Robotics.Base.DTOs;
using Robotics.Base.VTScheduleManager;
using Robotics.VTBase;
using roSchedulerFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roScheduleFunctions
{
    public class CommonScheduler
    {
        protected CommonScheduler()
        {
        }

        public static roCompanyConfiguration[] CheckIfDBNeedsToUpdate(int splitGroupSize)
        {
            bool isEmptyCache = Robotics.DataLayer.roCacheManager.GetInstance.IsCompanyCacheEmpty();

            roCompanyConfiguration[] cachedCompanies = Robotics.DataLayer.roCacheManager.GetInstance.GetCompanies();

            if (isEmptyCache)
            {
                try
                {
                    var manager = new roScheduleManager();
                    manager.UpdateDBTasks(cachedCompanies, Program.DefaultLogLevel, Program.DefaultTraceLevel, splitGroupSize);
                }
                catch (Exception ex)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunDatabaseUpdaterTask::Init::Error checking for DB version::" + ex.Message);
                }
            }

            return cachedCompanies;
        }

        public static bool IsChangeDayRunning()
        {
            DateTime xTime = DateTime.Now;

            if ( xTime.Hour == 0 && xTime.Minute >= 10 && xTime.Minute < 13 ) return true;
            else return false;
        }

        public static bool IsChangeHourRunning()
        {
            DateTime xTime = DateTime.Now;

            if (xTime.Hour != 0 && xTime.Minute == 10 ) return true;
            else return false;
        }

    }
}
