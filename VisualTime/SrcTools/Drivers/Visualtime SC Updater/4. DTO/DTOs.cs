using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SCServiceUpdater
{
    public enum ProxyRole
    {
        Terminal,
        Server
    }

    public class ProxyConfiguration
    {
        public string? version { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProxyRole role { get; set; }
        public string? apiUrl { get; set; }
        [JsonIgnore]
        public string? updateApiUrl { get; set; }
        public string? apiToken { get; set; }

        public string? customerId { get; set; }

        public bool? emergency { get; set; }

        public string? diagnosticsPort { get; set; }

        public int? checkEndpointsEvery { get; set; }
    }

    public class ApiResponse
    {
        public Updater? updater { get; set; }
        public bool hasVersion { get; set; }
    }

    public class Updater
    {
        public string? fileName { get; set; }
        public string? fileVersion { get; set; }
        public byte[]? content { get; set; }
    }

}
