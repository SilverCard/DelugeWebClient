# DelugeWebClient
Deluge WebUI RPC Client in C# 


#### Quick Sample
```csharp
 var delugeUrl = https://192.168.88.10:8112/json;
 var delugePassword = "123456";

  using (DelugeWebClient client = new DelugeWebClient(delugeUrl))
  {
      await client.LoginAsync(delugePassword);
      var r = await client.GetTorrentsStatusAsync();
      await client.LogoutAsync();
  }
```
