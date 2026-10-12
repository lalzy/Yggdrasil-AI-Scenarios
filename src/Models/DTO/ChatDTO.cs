// ChatDTO.cs

using System.ComponentModel.DataAnnotations;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.DTO;

public class ChatDTO{
    public record Request(
        [Required] string Content,
        [Required] ChatRole Role
    );
}
