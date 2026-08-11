// Records.cs

using Yggdrasil.Models;

namespace Yggdrasil.DTO;

public record WorldSummary(Guid world_ID, string Name, string Description);
public record CharacterSummary(Guid charactter_ID, string Name, string Description, string Gender);
public record PersonaSummary(Guid persona_ID, string Name, string Description, string Gender);
public record ChatMessageSummary(Guid ID, Guid conversation_ID, RoleType Role, string Content, DateTime TimeStamp);
