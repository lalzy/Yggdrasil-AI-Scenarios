// ConversationDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class ConversationDTO{
    public record Request(
        [Required] string Title,
        [Required] Guid WorldID
    );
}
