// NarratorDTO.cs

using Yggdrasil.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class NarratorDTO{
    public record Request(
        [Required] string DisplayName,
        [Required] string Description,
        [Required] string Instruction,
        [Required] LLMConnection Connection
    );
    public record Update(
        string? DisplayName=null,
        string? Description=null,
        string? Instruction=null,
        LLMConnection? Connection=null
    );
}
