using Polly.Retry;
using Polly.Timeout;
using System.Net.Sockets;
using System.Text.Json;

namespace HeartbeatBot.Job.Services.HealtChecks
{
    public class HealtCheckService : IHealtCheckService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        public HealtCheckService(IHttpClientFactory httpClientFactory, AsyncRetryPolicy retryPolicy, AsyncTimeoutPolicy timeoutPolicy)
        {
            _httpClient = httpClientFactory.CreateClient("Pinger");
            _retryPolicy = retryPolicy;
            _timeoutPolicy = timeoutPolicy;
        }

        public async Task<bool> PingAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _retryPolicy.WrapAsync(_timeoutPolicy)
                    .ExecuteAsync(() => _httpClient.GetAsync(url));

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"❌ Ping başarısız: Ağ bağlantı hatası veya URL erişilemez ({url}).");
                return false;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"⏳ Ping zaman aşımına uğradı: {url} belirlenen sürede yanıt vermedi.");
                return false;
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine($"⏳ Polly zaman aşımı: {url} adresine yapılan isteğin süresi doldu.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Beklenmeyen hata oluştu: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> PingTcpAsync(string address)
        {
            try
            {
                var parts = address.Split(':');
                if (parts.Length != 2 || !int.TryParse(parts[1], out int port))
                {
                    throw new ArgumentException("Geçersiz adres formatı. Format: IP:Port (örn: 127.0.0.1:6379)");
                }

                string host = parts[0];

                return await _retryPolicy.ExecuteAsync(() =>
                    _timeoutPolicy.ExecuteAsync(async () =>
                    {
                        using (var tcpClient = new TcpClient())
                        {
                            await tcpClient.ConnectAsync(host, port);
                            return tcpClient.Connected;
                        }
                    })
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP Ping failed: {ex.Message}");
                return false;
            }
        }
        public async Task<(bool isSuccess, string description)> CallWebhookAsync(string webhookUrl)
        {
            try
            {
                using HttpResponseMessage response = await _retryPolicy.WrapAsync(_timeoutPolicy)
                    .ExecuteAsync(() => _httpClient.GetAsync(webhookUrl));

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }
                else
                {
                    return (false, $"⚠️ Webhook başarısız! Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (false, $"🌐 Ağ hatası! Hata: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "⏳ Timeout! Webhook belirlenen süre içinde yanıt vermedi.");
            }
            catch (TimeoutRejectedException)
            {
                return (false, "⏳ Polly tarafından timeout süresi aşıldı.");
            }
            catch (Exception ex)
            {
                return (false, $"❌ Beklenmeyen hata: {ex.Message}");
            }
        }
        public async Task<(bool isSuccess, string description)> CallWebhookOutBoxMessageAsync(string webhookUrl)
        {
            try
            {
                using HttpResponseMessage response = await _retryPolicy.WrapAsync(_timeoutPolicy)
                .ExecuteAsync(() => _httpClient.GetAsync(webhookUrl));

                if (!response.IsSuccessStatusCode)
                {
                    return (false, string.Empty);
                }


                string jsonResponse = await response.Content.ReadAsStringAsync();

                List<string> encodedJsonList = JsonSerializer.Deserialize<List<string>>(jsonResponse);

                if (encodedJsonList == null || encodedJsonList.Count == 0)
                {
                    return (true, string.Empty);
                }

                List<string> messages = new List<string>();
                foreach (var encodedJson in encodedJsonList)
                {
                    JsonDocument doc = JsonDocument.Parse(encodedJson);
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("Data", out JsonElement dataElement))
                    {
                        if (dataElement.TryGetProperty("Message", out JsonElement messageElement))
                        {
                            messages.Add(messageElement.GetString());
                        }
                    }
                    else if (root.TryGetProperty("Message", out JsonElement messageElement))
                    {
                        messages.Add(messageElement.GetString());
                    }
                }

                return (true, messages.Count > 0 ? string.Join(" | ", messages) : string.Empty);
            }
            catch (Exception ex)
            {
                return (false, string.Empty);
            }
        }


    }
}
