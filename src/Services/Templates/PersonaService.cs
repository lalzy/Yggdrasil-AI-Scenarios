// PersonaService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Enums.SortOrders;
using Mapster;
using System.Linq.Dynamic.Core;

namespace Yggdrasil.Services.Templates;

public class PersonaService (AppDbContext db){
    private readonly DbSet<PersonaTemplate> _templates = db.Set<PersonaTemplate>();

    /// <summary>Create a PersonaTemplate</summary>
    /// <param name="request">The PersonaTemplate's data</param>
    /// <returns>The new created PersonaTemplate</returns>
    public PersonaTemplate? Create(PersonaDTO.Request request){
        var entity = request.Adapt<PersonaTemplate>();
        _templates.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get a requested PersonaTemplate</summary>
    /// <param name="ID">ID of the PersonaTemplate to get</param>
    /// <returns>The requested PersonaTemplate</returns>
    public PersonaTemplate? Get(Guid ID){
        return _templates.Find(ID);
    }

    /// <summary>Get all PersonaTemplate</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated PersonaTemplate records</returns>
    public List<PersonaTemplate> GetAll(int? count=null, int page=1, string sortOrder=PersonaSortOrder.DisplayNameAsc){
        IQueryable<PersonaTemplate> querry = _templates.OrderBy(sortOrder);
        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested PersonaTemplate</summary>
    /// <param name="ID">ID of the PersonaTemplate to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated PersonaTemplate</returns>
    public PersonaTemplate? Update(Guid ID, PersonaDTO.Update request){
        return null;
    }

    /// <summary>Delete a PersonaTemplate</summary>
    /// <param name="ID">ID of the PersonaTemplate to delete</param>
    public void Delete(Guid ID){}
}
