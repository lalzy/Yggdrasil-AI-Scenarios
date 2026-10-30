// PlayerService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Enums.SortOrders;

namespace Yggdrasil.Services.Instances;

public class PlayerService (AppDbContext db){
    private readonly DbSet<PlayerInstance> _players = db.Set<PlayerInstance>();

    /// <summary>Create a player instance</summary>
    /// <param name="PersonaTemplateID">The personaTemplate to build the player instance from</param>
    /// <returns>The new created PlayerInstance</returns>
    public PlayerInstance? Create(Guid PersonaTemplateID){
        return null;
    }

    /// <summary>Get a requested PlayerInstance</summary>
    /// <param name="ID">ID of the PlayerInstance to get</param>
    /// <returns>The requested PlayerInstance</returns>
    public PlayerInstance? Get(Guid ID){
        return null;
    }

    /// <summary>Get all PlayerInstance</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated PlayerInstance records</returns>
    public List<PlayerInstance> GetAll(int? count = null, int page=1, string sortOrder=SortOrder.IDAsc){
        return [];
    }
    
    /// <summary>Update the requested PlayerInstance</summary>
    /// <param name="ID">ID of the PlayerInstance to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated PlayerInstance</returns>
    public PlayerInstance? Update(Guid ID, PlayerDTO.Update request){
        return null;
    }

    /// <summary>Delete a PlayerInstance</summary>
    /// <param name="ID">ID of the PlayerInstance to delete</param>
    public void Delete(Guid ID){}
}
