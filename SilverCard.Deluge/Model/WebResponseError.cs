using Newtonsoft.Json;
using System;

namespace SilverCard.Deluge
{
    internal class WebResponseError
    {
        [JsonProperty(PropertyName = "messag")]
        public String Message { get; set; }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
    }
}
