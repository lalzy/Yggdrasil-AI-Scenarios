// WorldDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class WorldTemplateDTO{
    public record Request(
        [Required] string Name,
        [Required] string Description,
        List<string>? OpeningMessages,
        List<string>? Tags
    );
    public record Update(
        string? Name = null,
        string? Description = null,
        List<string>? OpeningMessages = null,
        List<string>? Tags = null
    );
}
