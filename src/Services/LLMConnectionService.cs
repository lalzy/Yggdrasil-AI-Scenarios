// LLMConnectionService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;


namespace Yggdrasil.Services;

public class LLMConnectionService (AppDbContext db){
    private readonly DbSet<LLMConnection> _LLMConnection = db.Set<LLMConnection>();
    
    public LLMConnection Create(LLMConnectionDTO.Request request){
        var entity = request.Patch<LLMConnection>();
        _LLMConnection.Add(entity);
        db.SaveChanges();
        
        return entity;
    }

    public LLMConnection? Get(Guid ID){
        var entity = _LLMConnection.Find(ID);
        return entity;
    }
    
    public List<LLMConnection> GetAll(int? count=null, int page=1, string sortOrder=LLMConnectionSortOrder.IDAsc){
        IQueryable<LLMConnection> querry = _LLMConnection.OrderBy(sortOrder);

        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        
        return querry.ToList();
        
    }


    public LLMConnection? Update(Guid ID, LLMConnectionDTO.Update request){
        var entity = _LLMConnection.Find(ID);
        request.Patch(entity);
        db.SaveChanges();
        
        return entity;
    }

    public void Delete(Guid ID){
        var entity = _LLMConnection.Find(ID);
        if(entity == null) return;
        
        _LLMConnection.Remove(entity);
        db.SaveChanges();
    }
}
