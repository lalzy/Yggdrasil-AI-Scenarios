// WorldInstanceService.cs

using Yggdrasil.Data;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;

namespace Yggdrasil.Services.Instances;

public class WorldInstanceService (AppDbContext db){
    private readonly DbSet<WorldTemplate> _template = db.Set<WorldTemplate>();
    private readonly DbSet<WorldInstance> _world = db.Set<WorldInstance>();
    
    /// <summary>Create a world instance</summary>
    /// <param name="request">The WorldInstance's data</param>
    /// <returns>The new created WorldInstance</returns>
    public WorldInstance? Create(Guid ID){
        var template = _template.Include(w => w.Narrator).ThenInclude(n => n.Connection).FirstOrDefault(w => w.ID == ID);
        if(template == null) return null;
        
        var entity = template.Patch<WorldInstance>();
        entity.ID = Guid.NewGuid();
        entity.Narrator = template.Narrator;
        
        _world.Add(entity);
        db.SaveChanges();
        
        return entity;
    }

    /// <summary>Get a requested WorldInstance</summary>
    /// <param name="ID">ID of the WorldInstance to get</param>
    /// <returns>The requested WorldInstance</returns>
    public WorldInstance? Get(Guid ID){
        return _world.Include(w => w.Conversations).Include(w => w.Narrator).ThenInclude(n => n.Connection).FirstOrDefault(w => w.ID == ID);
    }


    /// <summary>Get all WorldInstances</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated WorldInstance records</returns>
    public List<WorldInstance> GetAll(int? count=null, int page=1, string sortOrder=SortOrder.NameAsc){
        IQueryable<WorldInstance> querry = _world.Include(w => w.Conversations).Include(w => w.Narrator).ThenInclude(n => n.Connection).OrderBy(sortOrder);
        if(count != null){
            querry = querry.Skip(--page * count.Value)
                .Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested WorldInstance</summary>
    /// <param name="ID">ID of the WorldInstance to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated WorldInstance</returns>
    public WorldInstance? Update(Guid ID, WorldDTO.Update request){
        var entity = _world.Include(w => w.Conversations).Include(w => w.Narrator).ThenInclude(n => n.Connection).FirstOrDefault(w => w.ID == ID);

        if(entity == null) return null;
        
        request.Patch(entity);
        if(request.NarratorID != null){
            var narrator = db.Set<Narrator>().Include(n => n.Connection).FirstOrDefault(n => n.ID == request.NarratorID.Value);
            if(narrator == null)
                throw new KeyNotFoundException(WorldTemplateService.CreateKeyNotFoundString(request.NarratorID.Value));
            entity.Narrator = narrator;
        }
        db.SaveChanges();
        return entity;
    }

    /// <summary>Delete a worldInstance</summary>
    /// <param name="ID">ID of the worldInstance to delete</param>
    public void Delete(Guid ID){
        var entity = _world.Find(ID);
        if(entity == null) return;
        
        _world.Remove(entity);
        db.SaveChanges();
    }
}
