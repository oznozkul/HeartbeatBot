
namespace HeartbeatBot.Job.Services.HealtChecks
{
    internal interface IHealtCheckService
    {
        Task<bool> PingAsync(string url);
        Task<(bool isSuccess, string description)> CallWebhookAsync(string webhookUrl);
        Task<bool> PingTcpAsync(string address);
        Task<(bool isSuccess, string description)> CallWebhookOutBoxMessageAsync(string webhookUrl);
    }
}