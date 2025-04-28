using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ElasticsearchChecker
{
    private static readonly string elasticsearchUrl = "http://localhost:9200";

    public static async Task<bool> PingElasticsearch()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(elasticsearchUrl);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
