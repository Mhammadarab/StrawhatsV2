using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cargohub.models
{
    public class LogEntry
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("performedBy")]
        public string PerformedBy { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("changes")]
        public Changes Changes { get; set; }
    }

    public class Changes
    {
        [JsonProperty("app")]
        public ChangeDetail<string> App { get; set; }

        [JsonProperty("endpointAccess")]
        public Dictionary<string, Dictionary<string, ChangeDetail<bool>>> EndpointAccess { get; set; }
    }

    public class ChangeDetail<T>
    {
        [JsonProperty("oldValue")]
        public T OldValue { get; set; }

        [JsonProperty("newValue")]
        public T NewValue { get; set; }
    }
}