using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using marvin2.Models.PiModels;
using System.Text;

namespace data.Services
{
    public class PiService : IPiService
    {
        private readonly IConfigurationRoot _config;
        private static HttpClientHandler _handler = new HttpClientHandler()
        {
            //Using this handler to accept the self signed SSL cert from the PH API, which is locally hosted. This method is unsafe, so this HttpClient
            //is only used for queries to the local PH API. A different HttpClient must be used for any other use case in this application.
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        private readonly HttpClient _client = new HttpClient(_handler);
        
        public PiService(IConfigurationRoot config)
        {
            _config = config;
            _client.BaseAddress = new Uri(_config["PiHole:BaseAddress"]);
            
            StringContent stringContent = new(
                JsonSerializer.Serialize(new
                {
                    password = _config["PiHole:APIKey"]
                }),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage message = _client.PostAsync("auth", stringContent).Result;
            if (message.StatusCode != HttpStatusCode.OK) throw new Exception("API Auth Failure");
            string content = message.Content.ReadAsStringAsync().Result;
            PiAuth result = JsonSerializer.Deserialize<PiAuth>(content);
            _client.DefaultRequestHeaders.Add("sid", result.session.sid);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public bool IsBlocking()
        {
            return isBlockingAsync().Result;
        }
        
        private async Task<bool> isBlockingAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("dns/blocking");
            string content = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BlockingResponse>(content);
            
            if(result.blocking == BlockingResponse.Blocking.enabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public List<QueryClient> GetTopClients()
        {
            return getTopClientsAsync(false).Result;
        }
        
        public List<QueryClient> GetTopBlockedClients()
        {
            return getTopClientsAsync(true).Result;
        }
        
        private async Task<List<QueryClient>> getTopClientsAsync(bool blocked)
        {
            string urlPart = "";
            if(blocked)
            {
                urlPart = "stats/top_clients?blocked=true&count=10";
            }
            else
            {
                urlPart = "stats/top_clients?count=10";
            }
            
            HttpResponseMessage response = await _client.GetAsync(urlPart);
            string content = await response.Content.ReadAsStringAsync();
            
            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var unauthResult = JsonSerializer.Deserialize<PiUnauthResult>(content);
                throw new Exception(unauthResult.error.message);
            }
            
            var result = JsonSerializer.Deserialize<QueryResponse>(content);

            if (result is null) throw new Exception("API query failed.");

            List<QueryClient> queryClients = result.clients.ToList();
            
            QuerySorter sorter = new QuerySorter();
            queryClients.Sort(sorter);

            return queryClients;
        }       
        
        
        
    }
    
    
}