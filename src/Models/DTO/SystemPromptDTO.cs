// SystemPromptDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class SystemPromptDTO{
    public record Request(
        [Required] string Name
    );
}
