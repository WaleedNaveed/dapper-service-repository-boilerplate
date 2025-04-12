using Microsoft.Extensions.DependencyInjection;
using Quartz;
using DapperSRP.Jobs;
using Quartz.Impl;

namespace DapperSRP.Quartz
{
    public static class Startup
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddSingleton<JobFactory>();
            services.AddSingleton<QuartzJobScheduler>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                foreach (var job in JobScheduleConfig.JobSchedules)
                {
                    var jobType = job.Key;
                    var jobKey = new JobKey(jobType.Name);

                    q.AddJob(jobType, jobKey);

                    q.AddTrigger(trigger =>
                    {
                        trigger.ForJob(jobKey)
                               .WithIdentity($"{jobKey.Name}_Trigger")
                               .WithCronSchedule(job.Value);
                    });

                    services.AddTransient(jobType);
                }
            });

            services.AddSingleton(provider =>
            {
                var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
                var scheduler = schedulerFactory.GetScheduler().Result;

                scheduler.JobFactory = provider.GetRequiredService<JobFactory>();

                return scheduler;
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }

        public static async Task UseQuartzSchedulerAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var jobScheduler = scope.ServiceProvider.GetRequiredService<QuartzJobScheduler>();
            await jobScheduler.ScheduleAllJobs();
        }
    }
}
