// WorldTemplateService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;

namespace Yggdrasil.Services.Templates;

public class WorldTemplateService (AppDbContext db){
    private readonly DbSet<WorldTemplate> _world = db.Set<WorldTemplate>();

    /// <summary>Create a world template</summary>
    /// <param name="request">The worldTemplate's data</param>
    /// <returns>The new created worldTemplate</returns>
    public WorldTemplate Create(WorldDTO.Request request){
        var entity = request.Patch<WorldTemplate>();
        _world.Add(entity);
        db.SaveChanges();
        
        return entity;
    }

    /// <summary>Get a requested worldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to get</param>
    /// <returns>The requested worldTemplate</returns>
    public WorldTemplate? Get(Guid ID){
        return _world.Find(ID)!;
    }

    /// <summary>Get all worldTemplates</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated worldTemplate records</returns>
    public List<WorldTemplate> GetAll(int? count=null, int page=1, string sortOrder=WorldSortOrder.NameAsc){
        IQueryable<WorldTemplate> querry = _world.OrderBy(sortOrder);
        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested WorldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated WorldTemplate</returns>
    public WorldTemplate? Update(Guid ID, WorldDTO.Update request){
        var entity = _world.Find(ID);
        
        request.Patch(entity);
        
        db.SaveChanges();
        
        return entity;
    }

    /// <summary>Delete a worldTemplate</summary>
    /// <param name="ID">ID of the worldTempalte to delete</param>
    public void Delete(Guid ID){
        var entity = _world.Find(ID);
        if(entity == null) return;
        
        _world.Remove(entity);
        db.SaveChanges();
    }
}
