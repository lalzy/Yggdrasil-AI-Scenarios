// SourceTypes.cs

namespace Yggdrasil.Models.Enums;

/// <summary>Used when building the Prompt to send to the LLM. Reads the user's made
/// SystemPrompt and inject the relevant sources where the source-types exist
/// </summary>
public enum SourceType {
    FreeText, World, Narration, Characters, CharacterAgent, Persona, ChatMessages
}
