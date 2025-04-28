# HeartbeatBot

HeartbeatBot is a Telegram bot developed in C# that monitors the health status of services and sends notifications through Telegram.
This project is ideal for those who want a fast and practical solution to ensure their servers or services are always up and running.

## Features

- Periodically performs health checks on specified URLs or service endpoints.
- Sends instant Telegram notifications when a service goes down or comes back up.
- Configurable check intervals (e.g., every 5 minutes).
- Simple and quick setup.
- Supports multiple services.
- Built with modern and robust C# and .NET architecture.

## Use Cases

- Monitoring websites for uptime.
- Tracking API service availability.
- Building your own server/infrastructure monitoring system.

2. Install the required NuGet packages:
    - `Microsoft.Extensions.Hosting`
    - `Microsoft.Extensions.DependencyInjection`
    - `System.Net.Http`

3. Create an `appsettings.json` file and configure it like this:
    ```json
    {
      "TelegramBotToken": "YOUR_BOT_TOKEN",
      "ChatId": "YOUR_CHAT_ID",
      "HealthCheckUrls": [
        "https://example.com",
        "https://another-service.com/health"
      ],
      "CheckIntervalInSeconds": 300
    }
    ```

4. Run the project:
    ```bash
    dotnet run
    ```

## Structure

- `HealthCheckerService`: Sends HTTP requests to the specified URLs to check their health status.
- `TelegramNotifier`: Sends messages to Telegram when a service status changes.
- `Program.cs`: Starts services and configures dependency injection.

## Development

Potential improvements:
- Add advanced notification messages (e.g., include response times).
- Implement logging (e.g., to a file or database).
- Add manual health check triggers via Telegram bot commands.
- Define custom alert thresholds for each service.

## Contribution

If you want to contribute:
- Fork the repository.
- Make your changes.
- Submit a Pull Request.

