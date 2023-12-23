﻿// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Robust.Shared.Serialization;

namespace Content.Shared.SS220.Photocopier;

[Serializable, NetSerializable]
public enum PhotocopierVisuals : byte
{
    VisualState,
}

[Serializable, NetSerializable]
public enum PhotocopierVisualLayers : byte
{
    Base,
    Led,
    Top,
    TopPaper,
    PrintAnim
}

[Serializable, NetSerializable]
public enum PhotocopierVisualState : byte
{
    Off,
    Powered,
    Printing,
    Copying,
    OutOfToner
}

[Serializable, NetSerializable]
public sealed class PhotocopierCombinedVisualState : ICloneable
{
    public PhotocopierVisualState State { get; }
    public bool GotItem { get; }
    public bool BurnsButts { get; }
    public bool BurnsButtManually { get; }

    public PhotocopierCombinedVisualState(PhotocopierVisualState state, bool gotItem, bool burnsButts, bool burnsButtManually)
    {
        State = state;
        GotItem = gotItem;
        BurnsButts = burnsButts;
        BurnsButtManually = burnsButtManually;
    }

    public object Clone() => MemberwiseClone();
}
