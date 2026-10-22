// PromptDTO.cs

using Yggdrasil.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class PromptDTO{
    public record Request(
        [Required] string Name,
        [Required] string Content,
        [Required] SourceType Source,
        bool? Active = null
    );
    public record Update(
        string? Name = null,
        string? Content = null,
        bool? Active = null
    );
}
