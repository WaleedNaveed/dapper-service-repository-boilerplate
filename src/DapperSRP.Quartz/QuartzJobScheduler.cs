using Quartz;
using DapperSRP.Quartz;

namespace DapperSRP.Jobs
{
    public class QuartzJobScheduler
    {
        private readonly IScheduler _scheduler;

        public QuartzJobScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task ScheduleAllJobs()
        {
            foreach (var jobSchedule in JobScheduleConfig.JobSchedules)
            {
                var jobKey = new JobKey(jobSchedule.Key.Name);

                var job = JobBuilder.Create(jobSchedule.Key)
                                    .WithIdentity(jobKey)
                                    .Build();

                var trigger = TriggerBuilder.Create()
                                            .WithIdentity($"{jobSchedule.Key.Name}_Trigger")
                                            .WithCronSchedule(jobSchedule.Value)
                                            .ForJob(jobKey)
                                            .Build();

                await _scheduler.ScheduleJob(job, trigger);
            }

            await _scheduler.Start();
        }
    }
}
