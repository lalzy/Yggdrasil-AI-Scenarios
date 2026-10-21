// CharaterDTO.cs

using Yggdrasil.Models.ValueObjects;
using System.ComponentModel.DataAnnotations;
using Yggdrasil.Models.Entities;

namespace Yggdrasil.Models.DTO;

public class CharacterDTO{
    public record Request(
        [Required] Agent Agent,
        [Required] Profile Profile,
        LLMConnection? Connection = null
    );
    public record Update(
        Agent? A = null,
        Profile? P = null,
        LLMConnection? Connection = null
    );
}
