// Profile.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace Yggdrasil.Models.ValueObjects;

[ComplexType]
public class Profile{
    public required string FirstName {get; set;} = "";
    public string? LastName {get; set;}
    public required string Gender { get;set; }

    /// <summary>Pronouns combat gender confusion</summary>
    public string? Pronouns { get; set; }
    public string? Apperance { get; set; }
    public string? Occupation { get; set; }
    public string? Race { get; set; }
    public string? Personality { get; set; }
    public string? Background { get; set; }
}
