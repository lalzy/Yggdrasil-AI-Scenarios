// CharaterDTO.cs

using Yggdrasil.Models.ValueObjects;
using System.ComponentModel.DataAnnotations;
using Yggdrasil.Models.Entities;

namespace Yggdrasil.Models.DTO;

public class CharacterDTO{
    public record Request(
        [Required] AgentDTO.Request Agent,
        [Required] ProfileDTO.Request Profile,
        Guid? ConnectionID = null
    );
    public record Update(
        AgentDTO.Update? Agent = null,
        ProfileDTO.Update? Profile = null,
        Guid? ConnectionID = null
    );
}
