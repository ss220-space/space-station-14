using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Shared.Corvax.CCCVars;
using Content.Shared.SS220.Authorization;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Server.SS220.Authorization;

public sealed class DiscordAuthorizationManager : IAuthorizationManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly INetManager _netManager = default!;

    private readonly HttpClient _httpClient = new();

    private ISawmill _sawmill = default!;
    private string _apiUrl = string.Empty;
    private bool _authEnabled = false;

    public void Initialize()
    {
        _netManager.RegisterNetMessage<MsgOfferAuthorization>();
        _sawmill = Logger.GetSawmill("authorization");
        _cfg.OnValueChanged(CCCVars.AuthorizationApiUrl, s => _apiUrl = s, true);
        _cfg.OnValueChanged(CCCVars.AuthorizationEnabled, s => _authEnabled = s, true);
    }

    public async Task<bool> CheckAuth(IPlayerSession player)
    {
        if (!_authEnabled)
            return true;
        if (string.IsNullOrEmpty(_apiUrl))
            return true;
        var discordAuthorization = await GetDiscordAuthorization(player.Data.UserId);
        if (discordAuthorization == null || discordAuthorization.DiscordId == null)
        {
            var payload = new PlayerData() {
                UserId = player.Data.UserId,
                UserName = player.Data.UserName
            };

            var response = await _httpClient.PostAsync($"{_apiUrl}/{player.Data.UserId.ToString()}",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _sawmill.Error(
                    "Failed to get player authorization URL from API: [{StatusCode}] {Response}",
                    response.StatusCode,
                    errorText);
                return false;
            }

            var url = await response.Content.ReadFromJsonAsync<AuthorizationLink>();
            if (url == null)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _sawmill.Error(
                    "Failed to get player authorization URL from API, null received",
                    response.StatusCode,
                    errorText);
                return false;
            }

            OfferAuthorization(player, url.AuthURL);
            return false;
        }

        return true;
    }

    public async Task<DiscordAuthorization?> GetDiscordAuthorization(NetUserId userId)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return null;

        var url = $"{_apiUrl}/{userId.ToString()}";
        var response = await _httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            _sawmill.Error(
                "Failed to get player discord authorization from API: [{StatusCode}] {Response}",
                response.StatusCode,
                errorText);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<DiscordAuthorization>();
    }

    public void OfferAuthorization(IPlayerSession player, string url)
    {
        var message = new MsgOfferAuthorization
        {
            Url = url
        };
        _netManager.ServerSendMessage(message, player.ConnectedClient);
    }
}
