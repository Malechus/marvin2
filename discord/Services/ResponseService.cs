using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Selects randomized response strings for the bot.
    /// Reads arrays/sections from the provided <see cref="IConfigurationRoot"/> (for example,
    /// the "Greetings" and "Responses" sections in appsettings.json) and uses an injected
    /// <see cref="Random"/> to pick an entry.
    /// </summary>
    public class ResponseService
    {
        private readonly Random _random;
        private readonly IConfigurationRoot _config;
        
        /// <summary>
        /// Initializes a new instance of <see cref="ResponseService"/>.
        /// </summary>
        /// <param name="random">A shared <see cref="Random"/> instance used to pick items.</param>
        /// <param name="config">Configuration root to read greeting and response sections from.</param>
        public ResponseService(Random random, IConfigurationRoot config)
        {
            _random = random;
            _config = config;
        }

        public string BuildGreeting(){
            string hello = GetRandomGreeting();
            string version = GetVersionInfo();
            //string test = GetSelfTest();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(hello);
            sb.AppendLine(version);

            return sb.ToString();
        }
        
        /// <summary>
        /// Retrieves a random greeting string from the "Greetings" configuration section.
        /// </summary>
        /// <returns>
        /// A single greeting string selected at random from the "Greetings" section.
        /// If the section is empty, this method may throw an exception due to indexing on an empty list.
        /// </returns>
        public string GetRandomGreeting()
        {
            var greetingsSection = _config.GetSection("Greetings").GetChildren();
            var greetings = greetingsSection.Select(s => s.Value).ToList();
            int maxVal = greetings.Count;
            return greetings[_random.Next(0, maxVal)];
        }

	private string GetVersionInfo(){
		string versionInfo = "Discord Version: " + _config["Version:DiscordVersion"] + " | " + "Unified Version: " + _config["Version:UnifiedVersion"] + " | " + _config["Version:VersionName"];
		return versionInfo;
	}

	private string GetSelfTest(){
            //TODO: Add self test
            throw new Exception("pending automation");
            return "pending";

	}
        
        /// <summary>
        /// Retrieves a random response string from the "Responses" configuration section.
        /// </summary>
        /// <returns>
        /// A single response string selected at random from the "Responses" section.
        /// If the section is empty, this method may throw an exception due to indexing on an empty list.
        /// </returns>
        public string GetRandomResponse()
        {
            var responseSection = _config.GetSection("Responses").GetChildren();
            var responses = responseSection.Select(s => s.Value).ToList();
            int maxVal = responses.Count();
            return responses[_random.Next(0, maxVal)];
        }
    }
}
