using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;

namespace SilverCard.Deluge.Test
{
    [TestClass]
    public class DelugeWebClient_Test
    {
        public String DelugeUrl { get; private set; }
        public String DelugePassword { get; private set; }

        public DelugeWebClient_Test()
        {
            // Read deluge config from file.
            // delugeurl.txt
            // https://192.168.88.10:8112/json
            // password
            string[] l = File.ReadAllLines(@"D:\Dev\SilverCard.Deluge\delugeurl.txt");

            DelugeUrl = l[0];
            DelugePassword = l[1];
        }

        [TestMethod]
        public async Task GetConfig_Test()
        {
            using (DelugeWebClient client = new DelugeWebClient(DelugeUrl))
            {
                await client.LoginAsync(DelugePassword);
                var r = await client.GetConfigAsync();
                await client.LogoutAsync();
            }        
            
        }

        [TestMethod]
        public async Task GetSessionStatus_Test()
        {
            using (DelugeWebClient client = new DelugeWebClient(DelugeUrl))
            {
                await client.LoginAsync(DelugePassword);
                var r = await client.GetSessionStatusAsync();
                await client.LogoutAsync();
            }

        }

    }
}
