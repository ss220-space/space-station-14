using System.Text.Json.Serialization;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.Authorization;

[Serializable]
public sealed class DiscordAuthorization
{
    [JsonPropertyName("discordId")] public string? DiscordId { get; set; } = null;

    [JsonPropertyName("discordName")] public string? DiscordName { get; set; } = null;
}

[Serializable]
public sealed class PlayerData
{
    [JsonPropertyName("userId")] public NetUserId UserId { get; set; }

    [JsonPropertyName("userName")] public string? UserName { get; set; }
}

[Serializable]
public sealed class AuthorizationLink
{
    [JsonPropertyName("authURL")] public string AuthURL { get; set; } = string.Empty;
}

public sealed class MsgOfferAuthorization : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public string Url = "";

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Url = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Url);
    }
}
