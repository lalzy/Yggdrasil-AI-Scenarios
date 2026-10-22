// ProfileDTO.cs

using Yggdrasil.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class ProfileDTO{
    public record Request(
        [Required] string FirstName,
        [Required] string Gender,
        string? LastName = null,
        string? Pronouns = null,
        string? Apperance = null,
        string? Occupation = null,
        string? Race = null,
        string? Personality = null,
        string? Background = null
    );
    public record Update(
        string? FirstName = null,
        string? Gender = null,
        string? LastName = null,
        string? Pronouns = null,
        string? Apperance = null,
        string? Occupation = null,
        string? Race = null,
        string? Personality = null,
        string? Background = null
    );
}
