using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverCard.Deluge
{
    public class DelugeConfig
    {
        [JsonProperty(PropertyName = "max_download_speed")]
        public double MaxDownloadSpeed { get; set; }

        [JsonProperty(PropertyName = "max_upload_speed")]
        public double MaxUploadSpeed { get; set; }

        [JsonProperty(PropertyName = "torrentfiles_location")]
        public String TorrentFilesLocation { get; set; }

        [JsonProperty(PropertyName = "move_completed_path")]
        public String MoveCompletedPath { get; set; }

        [JsonProperty(PropertyName = "max_connections")]
        public int MaxConnections { get; set; }
    }
}
