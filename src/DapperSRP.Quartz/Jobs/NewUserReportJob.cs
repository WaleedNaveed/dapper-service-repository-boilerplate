using DapperSRP.Common;
using DapperSRP.Common.Configuration;
using DapperSRP.Service.EmailTemplates.Models;
using DapperSRP.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace DapperSRP.Quartz.Jobs
{
    public class NewUserReportJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<SuperAdminCredentialsConfig> _superAdminCredentials;

        public NewUserReportJob(IServiceScopeFactory serviceScopeFactory, IOptions<SuperAdminCredentialsConfig> superAdminCredentials)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _superAdminCredentials = superAdminCredentials;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            var newUsersCount = await userService.GetNewUsersCountAsync(DateTime.UtcNow.AddDays(-1));

            var emailBody = emailTemplateService.RenderTemplate(EmailTemplateConstants.DailyUserReportEmail, new DailyUserReportEmailModel() { NewUsers = newUsersCount.Result });
            await emailService.SendEmailAsync(_superAdminCredentials.Value.Email, Constants.DailyUserReportJobEmailSubject, emailBody);
        }
    }
}
