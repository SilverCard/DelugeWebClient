using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Reflection;

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
            string[] l = File.ReadAllLines(@"D:\delugeurl.txt");

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

        [TestMethod]
        public async Task GetTorrentsStatusAsync_Test()
        {
            using (DelugeWebClient client = new DelugeWebClient(DelugeUrl))
            {
                await client.LoginAsync(DelugePassword);
                var r = await client.GetTorrentsStatusAsync();
                await client.LogoutAsync();
            }

        }

        [TestMethod]
        public async Task AddRemoveTorrentMagnet_Test()
        {
            using (DelugeWebClient client = new DelugeWebClient(DelugeUrl))
            {
                await client.LoginAsync(DelugePassword);
                var torrentId = await client.AddTorrentMagnetAsync("magnet:?xt=urn:btih:30987c19cf0eae3cf47766f387c621fa78a58ab9&dn=debian-9.2.1-amd64-netinst.iso", new TorrentOptions() { MoveCompletedPath = "/etc/linux-iso" });
                Thread.Sleep(1000);
                var r2 = await client.RemoveTorrentAsync(torrentId, true);
                Assert.IsTrue(r2);
                await client.LogoutAsync();
            }

        }

        [TestMethod]
        public async Task AddRemoveTorrentByFile_Test()
        {
            using (DelugeWebClient client = new DelugeWebClient(DelugeUrl))
            {
                await client.LoginAsync(DelugePassword);
                string torrentFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test.torrent");
                var torrentId = await client.AddTorrentFile(torrentFile, new TorrentOptions() { MoveCompletedPath = "/etc/linux-iso" });
                Thread.Sleep(1000);
                var r2 = await client.RemoveTorrentAsync(torrentId, true);
                Assert.IsTrue(r2);
                await client.LogoutAsync();
            }

        }
    }
}
