using DapperSRP.Common;
using DapperSRP.Common.Configuration;
using DapperSRP.Service.EmailTemplates.Models;
using DapperSRP.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace DapperSRP.Quartz.Jobs
{
    public class NewProductReportJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<SuperAdminCredentialsConfig> _superAdminCredentials;

        public NewProductReportJob(IServiceScopeFactory serviceScopeFactory, IOptions<SuperAdminCredentialsConfig> superAdminCredentials)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _superAdminCredentials = superAdminCredentials;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            var newProductsCount = await productService.GetNewProductsCountAsync(DateTime.UtcNow.AddDays(-1));

            var emailBody = emailTemplateService.RenderTemplate(EmailTemplateConstants.DailyProductReportEmail, new DailyProductReportEmailModel() { NewProducts = newProductsCount.Result });
            await emailService.SendEmailAsync(_superAdminCredentials.Value.Email, Constants.DailyProductReportJobEmailSubject, emailBody);
        }
    }
}
