using HeartbeatBot.Job;
using HeartbeatBot.Job.Services.HealtChecks;
using HeartbeatBot.Job.Services.Messages;
using HeartbeatBot.Job.Services.OutboxMessages;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net;
using System.Net.Sockets;

var builder = Host.CreateApplicationBuilder(args);
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
    TimeSpan.FromSeconds(7),
    Polly.Timeout.TimeoutStrategy.Pessimistic
);


var timeoutPolicyTcp = Policy.TimeoutAsync(TimeSpan.FromSeconds(3), TimeoutStrategy.Pessimistic);
var retryPolicyTcp = Policy
    .Handle<SocketException>()   
    .Or<TaskCanceledException>()  
    .Or<TimeoutRejectedException>() 
    .WaitAndRetryAsync(5, retryAttempt =>
    {
        Console.WriteLine($"TCP Retry attempt {retryAttempt}...");
        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
    });

builder.Services.AddSingleton(timeoutPolicyTcp);
builder.Services.AddSingleton(retryPolicyTcp);
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<HttpRequestException>()
    .Or<TaskCanceledException>()
    .Or<TimeoutException>()
    .OrResult(response =>
        response.StatusCode == HttpStatusCode.NotFound ||   // 404
        response.StatusCode == HttpStatusCode.BadRequest || // 400
        response.StatusCode == HttpStatusCode.Forbidden ||  // 403
        response.StatusCode == HttpStatusCode.Unauthorized || // 401
        response.StatusCode == HttpStatusCode.TooManyRequests || // 429
        (int)response.StatusCode >= 500 
    )
    .WaitAndRetryAsync(5, retryAttempt =>
    {
        var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1));
        Console.WriteLine($"⚠️ HTTP Retry {retryAttempt}: Retrying after {delay.TotalSeconds} seconds.");
        return delay;
    },
    (result, timeSpan, retryCount, context) =>
    {
        if (result.Exception != null)
        {
            Console.WriteLine($"❌ Request failed on attempt {retryCount}. Retrying in {timeSpan.TotalSeconds} seconds. Error: {result.Exception.Message}");
        }
        else
        {
            Console.WriteLine($"❌ HTTP {result.Result.StatusCode}: Retrying attempt {retryCount} in {timeSpan.TotalSeconds} seconds.");
        }
    });



builder.Services.AddHttpClient("Pinger")
    .AddPolicyHandler(timeoutPolicy)
    .AddPolicyHandler(retryPolicy);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<OutBoxWorker>();
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<IOutboxMessageService, OutboxMessageService>();
builder.Services.AddScoped<IHealtCheckService, HealtCheckService>();
builder.Services.AddScoped<IMessageService, MessageService>();

var host = builder.Build();
host.Run();
