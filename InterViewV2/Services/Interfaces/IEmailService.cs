namespace InterViewV2.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string message, IEnumerable<(byte[] file, string name)> attachments = null);
        Task SendEmailAsync(IEnumerable<string> to, string subject, string message, IEnumerable<(byte[] file, string name)> attachments = null);
    }
}
