using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using Quartz;

namespace DapperSRP.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            using var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            return job ?? throw new Exception($"Job of type {bundle.JobDetail.JobType} could not be resolved.");
        }

        public void ReturnJob(IJob job) { }
    }
}
