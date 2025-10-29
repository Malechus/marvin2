using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace marvin2.Models.PiModels
{
    /// <summary>
    /// Represents the response for the Pi-hole blocking status endpoint.
    /// Contains the current blocking state, an optional timer value, and the request duration.
    /// </summary>
    public class BlockingResponse
    {
        /// <summary>
        /// Enumeration of known blocking states returned by the Pi-hole API.
        /// </summary>
        public enum Blocking { enabled, disabled, failed, unknown }

        /// <summary>
        /// The current blocking state as reported by the Pi-hole API.
        /// Uses a string enum converter to (de)serialize textual enum values.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public Blocking blocking{ get; set; }

        /// <summary>
        /// Optional timer value (in seconds) associated with the blocking state, if present.
        /// </summary>
        public float? timer{ get; set; }

        /// <summary>
        /// Time taken by the API to fulfill the request (in seconds).
        /// </summary>
        public float took{ get; set; }
    }
    
    /// <summary>
    /// Represents the response for a query summary endpoint.
    /// Contains a list of clients, total query counts, blocked query counts, and request duration.
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// Array of client summaries included in the response.
        /// </summary>
        public QueryClient[] clients{ get; set; }

        /// <summary>
        /// Total number of queries in the reported interval.
        /// </summary>
        public int total_queries{ get; set; }

        /// <summary>
        /// Number of queries that were blocked in the reported interval.
        /// </summary>
        public int blocked_queries{ get; set; }

        /// <summary>
        /// Time taken by the API to fulfill the request (in seconds).
        /// </summary>
        public float took{ get; set; }
    }
    
    /// <summary>
    /// Represents a client entry in query responses (IP, resolved name, and query count).
    /// </summary>
    public class QueryClient
    {
        /// <summary>
        /// IP address of the client.
        /// </summary>
        public string ip{ get; set; }

        /// <summary>
        /// Resolved name for the client (may be empty or identical to IP).
        /// </summary>
        public string name{ get; set; }

        /// <summary>
        /// Number of queries attributed to this client.
        /// </summary>
        public int count{ get; set; }
    }

    /// <summary>
    /// Represents an error object returned by the Pi-hole API.
    /// </summary>
    public class PiError
    {
        /// <summary>
        /// Machine-readable error key.
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Optional hint providing additional context or remediation steps.
        /// </summary>
        public string? hint { get; set; }

    }
    
    /// <summary>
    /// Represents the shape of an unauthorized result returned by certain Pi-hole endpoints.
    /// Contains an error object and request duration.
    /// </summary>
    public class PiUnauthResult
    {
        /// <summary>
        /// Error payload describing the authorization failure.
        /// </summary>
        public PiError error{ get; set; }

        /// <summary>
        /// Time taken by the API to process the request (in seconds).
        /// </summary>
        public float took{ get; set; }
    }
    
    /// <summary>
    /// Represents a Pi-hole authentication session payload.
    /// Includes session validity, TOTP flag, session identifiers, and optional messages.
    /// </summary>
    public class PiSession
    {
        /// <summary>
        /// Whether the session is valid.
        /// </summary>
        public bool valid{ get; set; }

        /// <summary>
        /// Whether TOTP (two-factor) is required for the session.
        /// </summary>
        public bool totp{ get; set; }

        /// <summary>
        /// Session identifier string, if provided.
        /// </summary>
        public string? sid{ get; set; }

        /// <summary>
        /// CSRF token value associated with the session, if provided.
        /// </summary>
        public string? csrf{ get; set; }

        /// <summary>
        /// Session validity duration (in seconds).
        /// </summary>
        public int validity{ get; set; }

        /// <summary>
        /// Optional message returned with the session payload (e.g., error or informational text).
        /// </summary>
        public string? message{ get; set; }
    }
    
    /// <summary>
    /// Represents the authentication response returned by the Pi-hole API when requesting a session.
    /// Contains the session object and the request duration.
    /// </summary>
    public class PiAuth
    {
        /// <summary>
        /// The session object describing authentication result details.
        /// </summary>
        public PiSession session{ get; set; }

        /// <summary>
        /// Time taken by the API to fulfill the authentication request (in seconds).
        /// </summary>
        public float took{ get; set; }
    }
}