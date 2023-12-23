using Content.Server.DeviceNetwork.Components;
using Content.Server.EUI;
using Content.Server.Ghost.Components;
using Content.Server.Paper;
using Content.Shared.Eui;
using Content.Shared.Fax;
using Content.Shared.Follower;
using Content.Shared.Ghost;
using Content.Shared.Paper;
using Content.Shared.SS220.Photocopier;

namespace Content.Server.Fax.AdminUI;

public sealed class AdminFaxEui : BaseEui
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    private readonly FaxSystem _faxSystem;
    private readonly FollowerSystem _followerSystem;

    public AdminFaxEui()
    {
        IoCManager.InjectDependencies(this);
        _faxSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<FaxSystem>();
        _followerSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<FollowerSystem>();
    }

    public override void Opened()
    {
        StateDirty();
    }

    public override AdminFaxEuiState GetNewState()
    {
        var faxes = _entityManager.EntityQueryEnumerator<FaxMachineComponent, DeviceNetworkComponent>();
        var entries = new List<AdminFaxEntry>();
        while (faxes.MoveNext(out var uid, out var fax, out var device))
        {
            entries.Add(new AdminFaxEntry(_entityManager.GetNetEntity(uid), fax.FaxName, device.Address));
        }
        return new AdminFaxEuiState(entries);
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case AdminFaxEuiMsg.Follow followData:
            {
                if (Player.AttachedEntity == null ||
                    !_entityManager.HasComponent<GhostComponent>(Player.AttachedEntity.Value))
                    return;

                _followerSystem.StartFollowingEntity(Player.AttachedEntity.Value, _entityManager.GetEntity(followData.TargetFax));
                break;
            }
            case AdminFaxEuiMsg.Send sendData:
            {
                var dataToCopy = new Dictionary<Type, IPhotocopiedComponentData>();
                var paperDataToCopy = new PaperPhotocopiedData()
                {
                    Content = sendData.Content,
                    StampState = sendData.StampState,
                    StampedBy = new() { new StampDisplayInfo { StampedName = sendData.From, StampedColor = sendData.StampColor } }
                };
                dataToCopy.Add(typeof(PaperComponent), paperDataToCopy);

                var metaData = new PhotocopyableMetaData()
                {
                    EntityName = sendData.Title,
                    PrototypeId = "PaperNtFormCc"
                };

                var printout = new FaxPrintout(dataToCopy, metaData);
                _faxSystem.Receive(_entityManager.GetEntity(sendData.Target), printout);
                break;
            }
        }
    }
}
