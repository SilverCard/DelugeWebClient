using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.IO;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SilverCard.Deluge.Test")]

namespace SilverCard.Deluge
{
    public class DelugeWebClient : IDisposable
    {
        private HttpClientHandler _httpClientHandler;
        private HttpClient _httpClient;
        private int _RequestId;
        public String Url { get; private set; }

        public DelugeWebClient(String url)
        {
            _httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            
            _httpClient = new HttpClient(_httpClientHandler, true);
            _RequestId = 1;

            Url = url;
        }

        public async Task LoginAsync(String password)
        {
            var result = await SendRequestAsync<Boolean>("auth.login", password);
            if (!result) throw new AuthenticationException("Failed to login.");            
        }

        public Task<Boolean> AuthCheckSessionAsync()
        {
            return SendRequestAsync<Boolean>("auth.check_session");
        }

        public async Task LogoutAsync()
        {
            var result = await SendRequestAsync<Boolean>("auth.delete_session");
            if (!result) throw new DelugeWebClientException("Failed to delete session.", 0);
        }

        public Task<String> AddTorrentMagnetAsync(String uri, TorrentOptions options = null)
        {
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentException(nameof(uri));         
            var req = CreateRequest("core.add_torrent_magnet", uri, options);
            req.NullValueHandling = NullValueHandling.Ignore;
            return SendRequestAsync<String>(req);
        }

        internal Task<String> AddTorrentFile(string file, TorrentOptions options = null)
        {
            if (String.IsNullOrWhiteSpace(file)) throw new ArgumentException(nameof(file));
            if(!File.Exists(file)) throw new ArgumentException(nameof(file));
            string filename = Path.GetFileName(file); 
            string base64 = Convert.ToBase64String(File.ReadAllBytes(file));
            var req = CreateRequest("core.add_torrent_file", filename, base64, options);
            req.NullValueHandling = NullValueHandling.Ignore;
            return SendRequestAsync<String>(req);
        }

        public Task<Boolean> RemoveTorrentAsync(String torrentId, Boolean removeData = false)
        {
            return SendRequestAsync<Boolean>("core.remove_torrent", torrentId, removeData);
        }

        public async Task<List<TorrentStatus>> GetTorrentsStatusAsync()
        {
            var emptyFilterDict = new Dictionary<string, string>();
            var keys = Utils.GetAllJsonPropertyFromType(typeof(TorrentStatus));
            Dictionary<String, TorrentStatus> result = await SendRequestAsync<Dictionary<String, TorrentStatus>>("core.get_torrents_status", emptyFilterDict, keys);
            return result.Values.ToList();
        }

        

        public async Task<SessionStatus> GetSessionStatusAsync()
        {
            var keys = Utils.GetAllJsonPropertyFromType(typeof(SessionStatus));
            var result = await SendRequestAsync<SessionStatus>("core.get_session_status", keys);
            return result;
        }

        public Task<DelugeConfig> GetConfigAsync()
        {        
            return SendRequestAsync<DelugeConfig>("core.get_config");
        }

        private Task<T> SendRequestAsync<T>(string method, params object[] parameters) 
        {
            return SendRequestAsync<T>(CreateRequest(method, parameters));
        }

        private async Task<T> SendRequestAsync<T>(WebRequestMessage webRequest) 
        {
            var requestJson = JsonConvert.SerializeObject(webRequest, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = webRequest.NullValueHandling
            });

            var responseJson = await PostJson(requestJson);
            var webResponse = JsonConvert.DeserializeObject<WebResponseMessage<T>>(responseJson);

            if(webResponse.Error != null) throw new DelugeWebClientException(webResponse.Error.Message, webResponse.Error.Code);
            if (webResponse.ResponseId != webRequest.RequestId) throw new DelugeWebClientException("Desync.", 0);

            return webResponse.Result;
        }

        private async Task<String> PostJson(String json)
        {
            StringContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var responseMessage = await _httpClient.PostAsync(Url, content);
            responseMessage.EnsureSuccessStatusCode();

            var responseJson = await responseMessage.Content.ReadAsStringAsync();
            return responseJson;
        }

        private WebRequestMessage CreateRequest(string method, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(method)) throw new ArgumentException(nameof(method));
            return new WebRequestMessage(_RequestId++, method, parameters);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_httpClient != null) _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DelugeWebClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
