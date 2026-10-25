// Character.cs

using Yggdrasil.Models.Abstracts;
namespace Yggdrasil.Models.Entities.Templates;

public class CharacterTemplate : CharacterBase{
    public List<WorldTemplate> Worlds { get; set; } = [];
}
