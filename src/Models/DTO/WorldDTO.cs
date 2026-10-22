// WorldDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class WorldDTO{
    public record Request(
        [Required] string Name,
        [Required] string Description,
        [Required] string Scenario,
        [Required] Guid NarratorID,
        List<string>? OpeningMessages,
        List<string>? Tags
    );
    
    public record Update(
        string? Name = null,
        string? Description = null,
        string? Scenario = null,
        Guid? NarratorID = null,
        List<string>? OpeningMessages = null,
        List<string>? Tags = null
    );
}
