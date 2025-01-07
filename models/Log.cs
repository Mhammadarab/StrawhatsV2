using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cargohub.models
{
    public class LoggingEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public string ApiKey { get; set; }
        public Changes Changes { get; set; }
    }

    public class Changes
    {
        public ChangeDetail<string> App { get; set; }
        public Dictionary<string, Dictionary<string, ChangeDetail<bool>>> EndpointAccess { get; set; }
    }

    public class ChangeDetail<T>
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }
}