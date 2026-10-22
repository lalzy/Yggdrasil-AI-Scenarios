// CharacterTemplateService.cs

using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;

namespace Yggdrasil.Services;

public class CharacterInstanceService (AppDbContext db){
    private readonly DbSet<CharacterInstance> _characters = db.Set<CharacterInstance>();
    private readonly DbSet<CharacterTemplate> _templates = db.Set<CharacterTemplate>();
    
    /// <returns>Error message string</returns>
    public static string CreateKeyNotFoundString(Guid narratorID){
        return $"Narrator with ID '{narratorID}' was not found.";
    }
    
    /// <summary>Create a world template</summary>
    /// <param name="request">The CharacterInstance's data</param>
    /// <returns>The new created CharacterInstance</returns>
    public CharacterInstance? Create(Guid ID){
        var template = _templates.Include(c => c.Connection).FirstOrDefault(c => c.ID == ID);
        if(template == null) return null;
        
        var entity = template.Patch<CharacterInstance>();
        entity.ID = Guid.NewGuid();
        entity.Connection = template.Connection;
        
        _characters.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get a requested CharacterInstance</summary>
    /// <param name="ID">ID of the CharacterInstance to get</param>
    /// <returns>The requested CharacterInstance</returns>
    public CharacterInstance? Get(Guid ID){
        return _characters.Include(c => c.Connection).FirstOrDefault(c => c.ID == ID);
    }

    /// <summary>Get all CharacterInstances</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    public List<CharacterInstance> GetAll(int? count=null, int page=1, string sortOrder=CharacterSortOrder.DisplayNameAsc){
        IQueryable<CharacterInstance> querry = _characters.Include(c => c.Connection).OrderBy(sortOrder);
        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested CharacterInstance</summary>
    /// <param name="ID">ID of the CharacterInstance to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated CharacterInstance</returns>
    public CharacterInstance? Update(Guid ID, CharacterDTO.Update request){
        var entity = _characters.Include(c => c.Connection).FirstOrDefault(c => c.ID == ID);
        if(entity == null) return null;
        if(request.ConnectionID != null){
            var connection = db.Set<LLMConnection>().FirstOrDefault(c => c.ID == request.ConnectionID);
            if(connection == null) throw new KeyNotFoundException(CreateKeyNotFoundString(request.ConnectionID.Value));
            entity.Connection = connection;
        }
        
        request.Patch(entity);
        
        db.SaveChanges();
        return entity;
    }


    /// <summary>Delete a CharacterInstance</summary>
    /// <param name="ID">ID of the CharacterInstance to delete</param>
    public void Delete(Guid ID){
        var entity = _characters.FirstOrDefault(c => c.ID == ID);
        if(entity != null){
            _characters.Remove(entity);
            db.SaveChanges();
        }
    }
}
