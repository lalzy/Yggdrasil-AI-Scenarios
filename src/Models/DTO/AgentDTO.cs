// AgentDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class AgentDTO{
    public record Request(
        [Required] string DisplayName,
        [Required] string Description,
        [Required] string Instruction
    );
    public record Update(
        string? DisplayName=null,
        string? Description=null,
        string? Instruction=null
    );
}
