using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HeartbeatBot.Job.Models;
using HeartbeatBot.Job.Models.Enums;
using HeartbeatBot.Job.Services.HealtChecks;
using HeartbeatBot.Job.Services.Messages;
using HeartbeatBot.Job.Services.OutboxMessages;

namespace HeartbeatBot.Job
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var appService = scope.ServiceProvider.GetRequiredService<IAppService>();
                    var healtCheckService = scope.ServiceProvider.GetRequiredService<IHealtCheckService>();
                    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxMessageService>();
                    var apps = appService.GetAllApps().Where(x => !x.IsActive);

                    foreach (var item in apps)
                    {
                        switch (item.HealtCheckType)
                        {
                            case HealtCheckType.Ping:
                                if (!await healtCheckService.PingAsync(item.Url))
                                {
                                    if (!item.IsLock)
                                    {
                                        await messageService.SendStatusMessage(item.ApplicationName, false);
                                        item.IsLock = true;
                                        appService.UpdateApp(item.Id, item);
                                    }
                                }
                                else if (item.IsLock)
                                {
                                    await messageService.SendStatusMessage(item.ApplicationName, true);
                                    item.IsLock = false;
                                    appService.UpdateApp(item.Id, item);
                                }
                                break;
                            case HealtCheckType.WebHookPing:
                                var (isAlive, description) = await healtCheckService.CallWebhookAsync(item.Url);

                                if (!isAlive)
                                {
                                    if (!item.IsLock)
                                    {
                                        await messageService.SendStatusMessage($"{item.ApplicationName} - {description}", false);
                                        item.IsLock = true;
                                        appService.UpdateApp(item.Id, item);
                                    }
                                }
                                else if (item.IsLock)
                                {
                                    await messageService.SendStatusMessage($"{item.ApplicationName} - {description}", true);
                                    item.IsLock = false;
                                    appService.UpdateApp(item.Id, item);
                                }
                                break;
                            case HealtCheckType.TcpPing:
                                if (!await healtCheckService.PingTcpAsync(item.Url))
                                {
                                    if (!item.IsLock)
                                    {
                                        await messageService.SendStatusMessage(item.ApplicationName, false);
                                        item.IsLock = true;
                                        appService.UpdateApp(item.Id, item);
                                    }
                                }
                                else if (item.IsLock)
                                {
                                    await messageService.SendStatusMessage(item.ApplicationName, true);
                                    item.IsLock = false;
                                    appService.UpdateApp(item.Id, item);
                                }
                                break;
                            case HealtCheckType.WebHookOutBoxMessage:
                                var (isAliveOutBox, descriptionOutBox) = await healtCheckService.CallWebhookOutBoxMessageAsync(item.Url);
                                if (!string.IsNullOrEmpty(descriptionOutBox))
                                {
                                    if (!item.IsLock)
                                    {
                                        await outboxService.CreateAsync(new OutboxMessage()
                                        {
                                            ApplicationId = item.Id,
                                            CreatedAt = DateTime.UtcNow,
                                            IsSent = false,
                                            Message = $"{item.ApplicationName} - {descriptionOutBox}"
                                        });
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
                await Task.Delay(5000, stoppingToken);

            }
        }


    }
}
