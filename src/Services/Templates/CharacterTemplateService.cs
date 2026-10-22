// CharacterTemplateService.cs

using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;
using Yggdrasil.Utils;


namespace Yggdrasil.Services;

public class CharacterTemplateService (AppDbContext db){
    private readonly DbSet<CharacterTemplate> _characters = db.Set<CharacterTemplate>();
    
    /// <summary>Create a world template</summary>
    /// <param name="request">The CharacterTemplate's data</param>
    /// <returns>The new created CharacterTemplate</returns>
    public CharacterTemplate Create(CharacterDTO.Request request){
        var entity = request.Adapt<CharacterTemplate>();

        if(request.ConnectionID != null){
            var connection = db.Set<LLMConnection>().Find(request.ConnectionID.Value);
            if(connection == null) throw new KeyNotFoundException(UniversalErrors.CreateKeyNotFoundString("Connection", request.ConnectionID.Value));
            entity.Connection = connection;
        }

        _characters.Add(entity);
        
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get a requested CharacterTemplate</summary>
    /// <param name="ID">ID of the CharacterTemplate to get</param>
    /// <returns>The requested CharacterTemplate</returns>
    public CharacterTemplate? Get(Guid ID){
        return _characters.Include(c => c.Connection).FirstOrDefault(c => c.ID == ID);
    }

    /// <summary>Get all CharacterTemplates</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    public List<CharacterTemplate> GetAll(int? count=null, int page=1, string sortOrder=CharacterSortOrder.DisplayNameAsc){
        IQueryable<CharacterTemplate> querry = _characters.Include(c => c.Connection).OrderBy(sortOrder);
        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested CharacterTemplate</summary>
    /// <param name="ID">ID of the CharacterTemplate to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated CharacterTemplate</returns>
    public CharacterTemplate? Update(Guid ID, CharacterDTO.Update request){
        var entity = _characters.Include(c => c.Connection).FirstOrDefault(c => c.ID == ID);
        if(entity == null) return null;
        if(request.ConnectionID != null){
            var connection = db.Set<LLMConnection>().FirstOrDefault(c => c.ID == request.ConnectionID);
            if(connection == null) throw new KeyNotFoundException(UniversalErrors.CreateKeyNotFoundString("Connection", request.ConnectionID.Value));
            entity.Connection = connection;
        }
        
        request.Patch(entity);
        
        db.SaveChanges();
        return entity;
    }


    /// <summary>Delete a CharacterTemplate</summary>
    /// <param name="ID">ID of the CharacterTemplate to delete</param>
    public void Delete(Guid ID){
        var entity = _characters.FirstOrDefault(c => c.ID == ID);
        if(entity != null){
            _characters.Remove(entity);
            db.SaveChanges();
        }
    }
}
