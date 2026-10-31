// PersonaDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class PersonaDTO{
    public record Request(
        [Required] string DisplayName,
        [Required] ProfileDTO.Request Profile,
        string? Description = null
    );

    public record Update(
        string? DisplayName = null,
        ProfileDTO.Request? Profile = null,
        string? Description = null
    );
}
