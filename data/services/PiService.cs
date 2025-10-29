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
    /// <summary>
    /// Client for interacting with a locally hosted Pi-hole API.
    /// Encapsulates authentication, request headers, and convenience methods for
    /// querying blocking status and top clients. This service uses a custom
    /// HttpClientHandler that accepts a self-signed certificate; that handler is
    /// intentionally unsafe and should only be used for local Pi-hole communication.
    /// </summary>
    public class PiService
    {
        /// <summary>
        /// HttpClientHandler configured to accept the Pi-hole's self-signed certificate.
        /// NOTE: This is unsafe for general use and is limited to local Pi-hole API calls.
        /// </summary>
        private static HttpClientHandler _handler = new HttpClientHandler()
        {
            //Using this handler to accept the self signed SSL cert from the PH API, which is locally hosted. This method is unsafe, so this HttpClient
            //is only used for queries to the local PH API. A different HttpClient must be used for any other use case in this application.
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        /// <summary>
        /// The configured HttpClient used to make API calls to the Pi-hole.
        /// </summary>
        private readonly HttpClient _client = new HttpClient(_handler);

        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Constructs a new <see cref="PiService"/> instance and performs authentication
        /// against the Pi-hole API using the API key found in configuration.
        /// The constructor sets up the client's BaseAddress, authenticates to obtain a session id,
        /// and configures required default request headers.
        /// </summary>
        /// <param name="config">Configuration root containing Pi-hole settings (PiHole:BaseAddress and PiHole:APIKey).</param>
        /// <exception cref="Exception">Thrown when the authentication POST does not return HTTP 200 OK.</exception>
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
        
        /// <summary>
        /// Synchronously returns whether the Pi-hole is currently blocking queries.
        /// </summary>
        /// <returns><c>true</c> if blocking is enabled; otherwise <c>false</c>.</returns>
        public bool IsBlocking()
        {
            return isBlockingAsync().Result;
        }
        
        /// <summary>
        /// Asynchronously queries the Pi-hole blocking status endpoint.
        /// </summary>
        /// <returns>A task that resolves to <c>true</c> when blocking is enabled; otherwise <c>false</c>.</returns>
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
        
        /// <summary>
        /// Returns the top clients by query count.
        /// </summary>
        /// <returns>List of top <see cref="QueryClient"/> entries ordered by query count.</returns>
        public List<QueryClient> GetTopClients()
        {
            return getTopClientsAsync(false).Result;
        }
        
        /// <summary>
        /// Returns the top clients that have blocked queries.
        /// </summary>
        /// <returns>List of top <see cref="QueryClient"/> entries filtered to blocked queries and ordered by count.</returns>
        public List<QueryClient> GetTopBlockedClients()
        {
            return getTopClientsAsync(true).Result;
        }
        
        /// <summary>
        /// Internal async implementation that queries the Pi-hole top clients endpoint.
        /// </summary>
        /// <param name="blocked">If <c>true</c>, only blocked-client statistics are returned; otherwise all clients are returned.</param>
        /// <returns>A task that resolves to a list of <see cref="QueryClient"/> ordered descending by count.</returns>
        /// <exception cref="Exception">Thrown when the API returns an unauthorized response or the response cannot be deserialized.</exception>
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

            List<QueryClient> queryClients = result.clients.ToList().OrderByDescending(c => c.count).ToList();

            return queryClients;
        }       
        
        
        
    }
    
    
}