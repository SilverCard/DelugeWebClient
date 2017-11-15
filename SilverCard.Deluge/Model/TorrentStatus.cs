using Newtonsoft.Json;
using System;

namespace SilverCard.Deluge
{
    public class TorrentStatus
    {
        [JsonProperty(PropertyName = "comment")]
        public String Comment { get; set; }

        [JsonProperty(PropertyName = "is_seed")]
        public Boolean IsSeed { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public String Hash { get; set; }

        [JsonProperty(PropertyName = "paused")]
        public Boolean Paused { get; set; }

        [JsonProperty(PropertyName = "ratio")]
        public double Ratio { get; set; }
        
        [JsonProperty(PropertyName = "message")]
        public String Message { get; set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
