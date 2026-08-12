// ConversationRequest.cs

using System.ComponentModel.DataAnnotations;
using Yggdrasil.Models;

namespace Yggdrasil.DTO;

public class ConversationRequest{
    [Required]
    public Guid World_ID { get; set; }
    [Required]
    public required string Title { get; set; }
}
