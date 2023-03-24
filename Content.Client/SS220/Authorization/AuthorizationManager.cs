using Content.Shared.SS220.Authorization;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Network;

namespace Content.Client.SS220.Authorization;

public sealed class AuthorizationManager
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;

    private string _url = "";
    private AuthPopup? _activePopup;

    public void Initialize()
    {
        _netManager.RegisterNetMessage<MsgOfferAuthorization>(OnShouldOfferAuthorization);
    }

    private void OnShouldOfferAuthorization(MsgOfferAuthorization message)
    {
        _url = message.Url;
        ShowOffer();
    }

    private void ShowOffer()
    {
        if (_activePopup != null)
            return;

        _activePopup = new AuthPopup();

        _activePopup.OnQuitPressed += OnQuitPressed;
        _activePopup.OnAcceptPressed += OnAcceptPressed;
        _activePopup.URLText = _url;
        _userInterfaceManager.WindowRoot.AddChild(_activePopup);
        LayoutContainer.SetAnchorPreset(_activePopup, LayoutContainer.LayoutPreset.Wide);
    }

    private void OnQuitPressed()
    {
        _consoleHost.ExecuteCommand("quit");
    }

    private void OnAcceptPressed()
    {
        IoCManager.Resolve<IUriOpener>().OpenUri(_url);
        _activePopup?.Orphan();
        _activePopup = null;
    }
}
