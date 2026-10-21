// CharacterTemplateService.cs

using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;

namespace Yggdrasil.Services;

public class CharacterTemplateService (AppDbContext db){
    private readonly DbSet<CharacterTemplate> _characters = db.Set<CharacterTemplate>();
    
    /// <returns>Error message string</returns>
    public static string CreateKeyNotFoundString(Guid narratorID){
        return $"Narrator with ID '{narratorID}' was not found.";
    }
    
    public CharacterTemplate Create(CharacterDTO.Request request){
        var entity = new CharacterTemplate{
            Agent = new Agent{Instruction="", DisplayName = "", Description=""},
            Profile =  new Profile{FirstName="", Gender=""}};
        return entity;
    }

    public CharacterTemplate? Get(Guid ID){
        var entity = new CharacterTemplate{
            Agent = new Agent{Instruction="", DisplayName = "", Description=""},
            Profile =  new Profile{FirstName="", Gender=""}};
        return entity;
    }

    public List<CharacterTemplate> GetAll(int? count=null, int page=1, string sortOrder=SortOrder.IDAsc){
        var entity = new CharacterTemplate{
            Agent = new Agent{Instruction="", DisplayName = "", Description=""},
            Profile =  new Profile{FirstName="", Gender=""}};
        return [entity];
    }

    public CharacterTemplate? Update(Guid ID, CharacterDTO.Update request){
        var entity = new CharacterTemplate{
            Agent = new Agent{Instruction="", DisplayName = "", Description=""},
            Profile =  new Profile{FirstName="", Gender=""}};
        return entity;
    }

    public void Delete(Guid ID){}
}
