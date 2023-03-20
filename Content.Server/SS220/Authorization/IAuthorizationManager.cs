using System.Threading.Tasks;
using Content.Shared.SS220.Authorization;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server.SS220.Authorization;

public interface IAuthorizationManager
{
    void Initialize();

    public Task<bool> CheckAuth(IPlayerSession player);

    public Task<DiscordAuthorization?> GetDiscordAuthorization(NetUserId userId);

    public void OfferAuthorization(IPlayerSession player, string url);
}
