// NarratorService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;

namespace Yggdrasil.Services.Templates;

public class NarratorService (AppDbContext db){
    private readonly DbSet<Narrator> _narrator = db.Set<Narrator>();

    private string InvalidConnectionError(Guid connectionID){
        return $"Connection with ID '{connectionID}' was not found.";
    }
    
    /// <summary>Create a world template</summary>
    /// <param name="request">The worldTemplate's data</param>
    /// <returns>The new created worldTemplate</returns>
    public Narrator Create(NarratorDTO.Request request){
        var entity = request.Patch<Narrator>();
        // request.Adapt<Narrator>();
        var connection = db.Set<LLMConnection>().Find(request.ConnectionID);
        if(connection == null) throw new KeyNotFoundException(InvalidConnectionError(request.ConnectionID));
        entity.Connection = connection;
        
        _narrator.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get a requested worldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to get</param>
    /// <returns>The requested worldTemplate</returns>
    public Narrator? Get(Guid ID){
        return _narrator.Include(n =>n.Connection).FirstOrDefault(n => n.ID == ID);
    }

    /// <summary>Get all worldTemplates</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated worldTemplate records</returns>
    public List<Narrator> GetAll(int? count=null, int page=1, string sortOrder=NarratorSortOrder.NameAsc){
        IQueryable<Narrator> querry = _narrator.Include(n => n.Connection).OrderBy(sortOrder);

        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Update the requested WorldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated WorldTemplate</returns>
    public Narrator? Update(Guid ID, NarratorDTO.Update request){
        var entity = _narrator.Include(n => n.Connection).FirstOrDefault(n => n.ID == ID);
        if(entity == null) return null;
        
        if(request.ConnectionID != null){
            var connection = db.Set<LLMConnection>().Find(request.ConnectionID.Value);
            if(connection == null) throw new KeyNotFoundException(InvalidConnectionError(request.ConnectionID.Value));
            entity.Connection = connection;
        }
        request.Patch(entity);
        db.SaveChanges();
        
        return entity;
    }

    /// <summary>Delete a worldTemplate</summary>
    /// <param name="ID">ID of the worldTempalte to delete</param>
    public void Delete(Guid ID){
        var entity = _narrator.Find(ID);
        if(entity != null){
            _narrator.Remove(entity);
            db.SaveChanges();
        }
    }
}
