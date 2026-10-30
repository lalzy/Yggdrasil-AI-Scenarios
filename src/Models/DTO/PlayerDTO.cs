// PlayerDTO.cs

using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.DTO;

public class PlayerDTO{
    public record Update(
        string? DisplayName = null,
        string? Description = null,
        Profile Profile = null
    );
}
