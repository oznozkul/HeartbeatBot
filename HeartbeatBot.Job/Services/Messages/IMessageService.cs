
namespace HeartbeatBot.Job.Services.Messages
{
    public interface IMessageService
    {
        Task SendTelegramMessage(string message);
        Task SendStatusMessage(string appName, bool isRunning);
        Task SendStatusMessage(string appName, string errorDescription);
    }
}