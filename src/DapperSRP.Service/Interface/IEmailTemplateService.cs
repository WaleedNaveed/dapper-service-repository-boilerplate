namespace DapperSRP.Service.Interface
{
    public interface IEmailTemplateService
    {
        string RenderTemplate<T>(string templateName, T model);
    }
}
