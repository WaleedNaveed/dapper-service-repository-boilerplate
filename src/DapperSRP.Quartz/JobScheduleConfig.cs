using DapperSRP.Quartz.Jobs;

namespace DapperSRP.Quartz
{
    public static class JobScheduleConfig
    {
        public static readonly Dictionary<Type, string> JobSchedules = new()
        {
            { typeof(NewUserReportJob), "0 0 23 * * ?" },  // Runs at 11:00 PM daily
            { typeof(NewProductReportJob), "0 15 23 * * ?" } // Runs at 11:15 PM daily
        };
    }
}
