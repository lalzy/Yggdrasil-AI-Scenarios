// ChatMessageRequest.cs

using Yggdrasil.Models;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.DTO;
public class ChatMessageRequest{
    [Required]
    public required Guid Conversation_ID { get; set; }
    [Required]
    public RoleType Role { get; set; }
    [Required]
    public required string Content { get; set; }
}
