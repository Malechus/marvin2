

using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace marvin2.Models.PiModels
{
    public class BlockingResponse
    {
        public enum Blocking { enabled, disabled, failed, unknown }
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public Blocking blocking{ get; set; }
        public float? timer{ get; set; }
        public float took{ get; set; }
    }
    
    public class QueryResponse
    {
        public QueryClient[] clients{ get; set; }
        public int total_queries{ get; set; }
        public int blocked_queries{ get; set; }
        public float took{ get; set; }
    }
    
    public class QueryClient
    {
        public string ip{ get; set; }
        public string name{ get; set; }
        public int count{ get; set; }
    }

    public class PiError
    {
        public string key { get; set; }
        public string message { get; set; }
        public string? hint { get; set; }

    }
    
    public class PiUnauthResult
    {
        public PiError error{ get; set; }
        public float took{ get; set; }
    }
    
    public class PiSession
    {
        public bool valid{ get; set; }
        public bool totp{ get; set; }
        public string? sid{ get; set; }
        public string? csrf{ get; set; }
        public int validity{ get; set; }
        public string? message{ get; set; }
    }
    
    public class PiAuth
    {
        public PiSession session{ get; set; }
        public float took{ get; set; }
    }
}