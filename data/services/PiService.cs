using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace data.Services
{
    public class PiService : IPiService
    {
        private readonly IConfigurationRoot _config;
        private readonly HttpClient _client = new HttpClient();
        
        public PiService(IConfigurationRoot config)
        {
            _config = config;
            _client.BaseAddress = new Uri(_config["PiHole:BaseAddress"]);
            _client.DefaultRequestHeaders.Add("sid", _config["PiHole:APIKey"]);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public async Task<bool> IsBlockingAsync()
        {
            return await isBlockingAsync();
        }
        
        private async Task<bool> isBlockingAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("dns/blocking");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BlockingResponse>(content);
            
            if(result.blocking == BlockingResponse.Blocking.enabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private class BlockingResponse
        {
            public enum Blocking { enabled, disabled, failed, unknown }
            public Blocking blocking{ get; set; }
            public float timer{ get; set; }
            public float took{ get; set; }
        }
    }
    
    
}