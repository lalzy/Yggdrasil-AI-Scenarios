// AgentDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class AgentDTO{
    public record Request(
        [Required] string DisplayName,
        [Required] string Description,
        [Required] List<PromptDTO.Request> Prompts
    );
    public record Update(
        string? DisplayName=null,
        string? Description=null,
        List<PromptDTO.Request>? Prompts = null
    );
}
