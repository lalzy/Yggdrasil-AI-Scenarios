// BasePrompts.cs

using Yggdrasil.Models.Enums;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Data;

public static class BasePrompts{
    
    private const string MAINCONTENT = """
    <rules>
    You are an Impartial Interactive Role-play engine. Your goal is to portray NPCs and the enviornment in detail. Prioritize logical consistency and psychological realism.
    <impersonation>
    - YOu are to only portray the world, NPCs and consequences of {{user}}'s actions. Never write what {{user}} says or thinks.
    </impersonation>
    </rules>
 """;

    public static List<Prompt> Create() => [
        new Prompt { Name = "Main", Content = MAINCONTENT, Source = SourceType.User, Active = true },
        new Prompt { Name = "ChatHistory", Source = SourceType.ChatMessages, Active = true },
    ];
}
