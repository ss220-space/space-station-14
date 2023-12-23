﻿// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.SS220.CryopodSSD;

public abstract class SharedCryopodSSDSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly StandingStateSystem _standingStateSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CryopodSSDComponent, CanDropTargetEvent>(OnCryopodSSDCanDropTarget);
    }

    /// <summary>
    /// Inserts target inside cryopod
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="target"></param>
    /// <param name="cryopodSsdComponent"></param>
    /// <returns> true if we successfully inserted target inside cryopod, otherwise returns false</returns>
    public bool InsertBody(EntityUid uid, EntityUid target, CryopodSSDComponent cryopodSsdComponent)
    {
        if (cryopodSsdComponent.BodyContainer.ContainedEntity != null)
            return false;

        if (!HasComp<MobStateComponent>(target))
            return false;

        var xform = Transform(target);
        cryopodSsdComponent.BodyContainer.Insert(target, transform: xform, force: true);

        _actionsSystem.AddAction(target, ref cryopodSsdComponent.LeaveActionEntity, cryopodSsdComponent.LeaveAction, uid);
        _standingStateSystem.Stand(target, force: true);

        cryopodSsdComponent.EntityLiedInCryopodTime = _gameTiming.CurTime;

        UpdateAppearance(uid, cryopodSsdComponent);
        return true;
    }

    public void TryEjectBody(EntityUid uid, EntityUid userId, CryopodSSDComponent? cryopodSsdComponent)
    {
        if (!Resolve(uid, ref cryopodSsdComponent))
        {
            return;
        }

        var ejected = EjectBody(uid, cryopodSsdComponent);
        if (ejected != null)
            _adminLogger.Add(LogType.Action, LogImpact.Medium,
                $"{ToPrettyString(ejected.Value)} ejected from {ToPrettyString(uid)} by {ToPrettyString(userId)}");
    }

    public virtual EntityUid? EjectBody(EntityUid uid, CryopodSSDComponent? cryopodSsdComponent)
    {
        if (!Resolve(uid, ref cryopodSsdComponent))
        {
            return null;
        }

        if (cryopodSsdComponent.BodyContainer.ContainedEntity is not { Valid: true } contained)
        {
            return null;
        }

        cryopodSsdComponent.BodyContainer.Remove(contained);

        if (HasComp<KnockedDownComponent>(contained) || _mobStateSystem.IsIncapacitated(contained))
        {
            _standingStateSystem.Down(contained);
        }
        else
        {
            _standingStateSystem.Stand(contained);
        }

        _actionsSystem.RemoveProvidedActions(contained, uid);

        UpdateAppearance(uid, cryopodSsdComponent);
        return contained;
    }

    private void OnCryopodSSDCanDropTarget(EntityUid uid, CryopodSSDComponent component,
        ref CanDropTargetEvent args)
    {
        if (args.Handled)
            return;

        args.CanDrop = HasComp<BodyComponent>(args.Dragged) && _mobStateSystem.IsAlive(args.Dragged);
        args.Handled = true;
    }

    protected void OnComponentInit(EntityUid uid, CryopodSSDComponent cryopodSSDComponent, ComponentInit args)
    {
        cryopodSSDComponent.BodyContainer = _containerSystem.EnsureContainer<ContainerSlot>(uid, "pod-body");
    }

    protected void UpdateAppearance(EntityUid uid, CryopodSSDComponent? cryopodSSD = null,
        AppearanceComponent? appearance = null)
    {
        if (!Resolve(uid, ref cryopodSSD))
        {
            return;
        }

        if (!Resolve(uid, ref appearance))
        {
            return;
        }

        _appearanceSystem.SetData(uid, CryopodSSDComponent.CryopodSSDVisuals.ContainsEntity,
            cryopodSSD.BodyContainer.ContainedEntity is null || _entityManager.IsQueuedForDeletion(cryopodSSD.BodyContainer.ContainedEntity.Value), appearance);
    }

    protected void AddAlternativeVerbs(EntityUid uid, CryopodSSDComponent cryopodSSDComponent,
        GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.User != cryopodSSDComponent.BodyContainer.ContainedEntity)
            return;

        if (cryopodSSDComponent.BodyContainer.ContainedEntity != null)
        {
            args.Verbs.Add(new AlternativeVerb
            {
                Text = Loc.GetString("cryopodSSD-verb-noun-occupant"),
                Category = VerbCategory.Eject,
                Priority = 1,
                Act = () => TryEjectBody(uid, args.User, cryopodSSDComponent)
            });
        }
    }
}

public sealed partial class CryopodSSDLeaveActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class CryopodSSDDragFinished : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class TeleportToCryoFinished : SimpleDoAfterEvent
{
    public NetEntity PortalId { get; private set; }

    public TeleportToCryoFinished(NetEntity portalId)
    {
        PortalId = portalId;
    }
}
