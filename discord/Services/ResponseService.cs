

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace marvin2.discord.Services
{
    public class ResponseService
    {
        private readonly Random _random;
        private readonly IConfigurationRoot _config;
        
        public ResponseService(Random random, IConfigurationRoot config)
        {
            _random = random;
            _config = config;
        }
        
        public string GetRandomGreeting()
        {
            var greetingsSection = _config.GetSection("Greetings").GetChildren();
            var greetings = greetingsSection.Select(s => s.Value).ToList();
            int maxVal = greetings.Count;
            return greetings[_random.Next(0, maxVal)];
        }
        
        public string GetRandomResponse()
        {
            var responseSection = _config.GetSection("Responses").GetChildren();
            var responses = responseSection.Select(s => s.Value).ToList();
            int maxVal = responses.Count();
            return responses[_random.Next(0, maxVal)];
        }
    }
}