using Content.Server.SS220.Authorization;
using Content.Shared.Administration;
using Content.Shared.GameTicking;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands
{
    [AnyCommand]
    sealed class ObserveCommand : IConsoleCommand
    {
        [Dependency] private readonly IAuthorizationManager _authManager = default!;

        public string Command => "observe";
        public string Description => "";
        public string Help => "";

        public async void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not IPlayerSession player)
            {
                return;
            }

            var ticker = EntitySystem.Get<GameTicker>();

            if (ticker.RunLevel == GameRunLevel.PreRoundLobby)
            {
                shell.WriteError("Wait until the round starts.");
                return;
            }

            if (ticker.PlayerGameStatuses.TryGetValue(player.UserId, out var status) &&
                status != PlayerGameStatus.JoinedGame)
            {
                var authValid = await _authManager.CheckAuth(player);
                if (authValid)
                {
                    ticker.MakeObserve(player);
                }
            }
            else
            {
                shell.WriteError($"{player.Name} is not in the lobby.   This incident will be reported.");
            }
        }
    }
}
