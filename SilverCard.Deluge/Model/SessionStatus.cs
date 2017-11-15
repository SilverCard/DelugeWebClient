using Newtonsoft.Json;

namespace SilverCard.Deluge
{
    //Gets the session status values for ‘keys’, these keys are taking from libtorrent’s session status.
    //http://www.rasterbar.com/products/libtorrent/manual.html#status

    public class SessionStatus
    {
        [JsonProperty(PropertyName = "upload_rate")]
        public int UploadRate { get; set; }

        [JsonProperty(PropertyName = "download_rate")]
        public int DownloadRate { get; set; }

        [JsonProperty(PropertyName = "num_peers")]
        public int NumPeers { get; set; }
    }
}
