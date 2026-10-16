// WorldDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class WorldDTO{
    public record Request(
        [Required] string Name,
        [Required] string Description,
        string? OpeningMessage
    );
    public record Edit(
        string? name,
        string? Description
    );
}
