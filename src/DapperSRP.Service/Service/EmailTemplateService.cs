using DapperSRP.Common;
using DapperSRP.Service.Interface;

namespace DapperSRP.Service.Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string RenderTemplate<T>(string templateName, T model)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EmailTemplateConstants.BaseFolder, $"{templateName}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Email template not found: {filePath}");
            }

            string templateContent = File.ReadAllText(filePath);

            // Manually replace placeholders in the template with model data
            foreach (var prop in typeof(T).GetProperties())
            {
                string placeholder = $"{{{prop.Name}}}";
                string value = prop.GetValue(model)?.ToString() ?? string.Empty;
                templateContent = templateContent.Replace(placeholder, value);
            }

            return templateContent;
        }
    }
}
