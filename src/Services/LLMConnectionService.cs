// LLMConnectionService.cs

using Yggdrasil.Constants;
using Yggdrasil.Models;
using Yggdrasil.Util;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Extensions;

namespace Yggdrasil.Services;

public class LLMConnectionService (AppDbContext db){
    private readonly AppDbContext _db = db;

    public ServiceResult<List<LLMConnectionSummary>> GetAll(int? count=null){
        var query = _db.Set<LLMConnection>().Where(c => c.Name != null).Select(c => new LLMConnectionSummary(c.ID, c.Name, c.URL, c.APIType));

        if(count.HasValue){
            if(count < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(count.Value);
        }
        
        return new(query.ToList());
    }

    public ServiceResult<LLMConnection> GetOne(Guid connection_ID){
        var connection = _db.Set<LLMConnection>().FirstOrDefault(c => c.ID == connection_ID);

        if(connection == null) throw new KeyNotFoundException(ErrorMessages.CONNECTION_NOT_FOUND);

        return new(connection);
    }

    public ServiceResult<LLMConnection> Create(LLMConnectionRequest request){
        LLMConnection connection = request.ConvertModelToDTO<LLMConnection>();

        connection = _db.Set<LLMConnection>().Add(connection).Entity;
        _db.SaveChanges();
        return new(connection);
    }
}
