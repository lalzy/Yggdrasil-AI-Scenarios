// WorldInstanceService.cs

using Yggdrasil.Data;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;

namespace Yggdrasil.Services.Instances;

public class WorldInstanceService (AppDbContext db){
    private readonly DbSet<WorldInstance> _world = db.Set<WorldInstance>();
    
    /// <summary>Create a world instance</summary>
    /// <param name="request">The WorldInstance's data</param>
    /// <returns>The new created WorldInstance</returns>
    public WorldInstance Create(Guid ID){
        var entity = new WorldInstance { Name = "", Description = "", Scenario="" };
        // Get WorldTemplate by ID. Create a new Intance object from the template and return it.
        return entity;
    }

    /// <summary>Get a requested WorldInstance</summary>
    /// <param name="ID">ID of the WorldInstance to get</param>
    /// <returns>The requested WorldInstance</returns>
    public WorldInstance? Get(Guid ID){
        var entity = new WorldInstance { Name = "", Description = "", Scenario="" };
        return entity;
    }


    /// <summary>Get all WorldInstances</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated WorldInstance records</returns>
    public List<WorldInstance> GetAll(int? count=null, int page=1, string sortOrder=SortOrder.NameAsc){
        var entity = new WorldInstance { Name = "", Description = "", Scenario="" };
        return [entity];
    }

    /// <summary>Update the requested WorldInstance</summary>
    /// <param name="ID">ID of the WorldInstance to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated WorldInstance</returns>
    public WorldInstance Update(Guid ID, WorldDTO.Update request){
        var entity = new WorldInstance { Name = "", Description = "", Scenario="" };
        return entity;
    }

    /// <summary>Delete a worldInstance</summary>
    /// <param name="ID">ID of the worldInstance to delete</param>
    public void Delete(Guid ID){
        
    }
}
