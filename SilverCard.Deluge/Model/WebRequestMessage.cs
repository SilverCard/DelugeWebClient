﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SilverCard.Deluge
{
    internal class WebRequestMessage
    {
        [JsonProperty(PropertyName = "id")]
        public int RequestId { get; set; }

        [JsonProperty(PropertyName = "method")]
        public String Method { get; set; }

        [JsonProperty(PropertyName = "params")]
        public List<Object> Params { get; set; }

        [JsonIgnore]
        public NullValueHandling NullValueHandling { get; set; }

        public WebRequestMessage(int requestId, String method, params object[] parameters)
        {
            RequestId = requestId;
            Method = method;
            Params = new List<Object>();

            if (parameters != null) Params.AddRange(parameters);

            NullValueHandling = NullValueHandling.Include;
        }
    }
}
