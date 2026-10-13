// ConversationDTO.cs

using Yggdrasil.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class ConversationDTO{
    public record Request(
        List<Chat> Chats,
        [Required] string Title
    );
}
