
using Microsoft.Extensions.DependencyInjection;
using HeartbeatBot.Job.Services.Messages;
using HeartbeatBot.Job.Services.OutboxMessages;

namespace HeartbeatBot.Job
{
    public class OutBoxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OutBoxWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxMessageService>();
                    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    var selectedMessageList = await outboxService.GetAllAsync();
                    foreach (var item in selectedMessageList)
                    {
                        await messageService.SendStatusMessage(item.Application.ApplicationName, item.Message);
                       await outboxService.ChangeStatus(item.Id);
                    }
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
