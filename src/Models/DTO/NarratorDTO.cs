// NarratorDTO.cs

using Yggdrasil.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class NarratorDTO{
    public record Request(
        [Required] AgentDTO.Request Agent,
        [Required] Guid ConnectionID
    );
    public record Update(
        AgentDTO.Update? Agent = null,
        Guid? ConnectionID=null
    );
}
