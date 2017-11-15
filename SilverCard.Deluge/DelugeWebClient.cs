using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

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


            _httpClient = new HttpClient(_httpClientHandler);
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
            var requestJson = JsonConvert.SerializeObject(webRequest);
            StringContent content = new StringContent(requestJson);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            var responseMessage = await _httpClient.PostAsync(Url, content);
            responseMessage.EnsureSuccessStatusCode();

            var rJson = await responseMessage.Content.ReadAsStringAsync();
            var webResponse = JsonConvert.DeserializeObject<WebResponseMessage<T>>(rJson);

            if(webResponse.Error != null) throw new DelugeWebClientException(webResponse.Error.Message, webResponse.Error.Code);
            if (webResponse.ResponseId != webRequest.RequestId) throw new DelugeWebClientException("Desync.", 0);

            return webResponse.Result;
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
                    if (_httpClientHandler != null) _httpClientHandler.Dispose();
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
