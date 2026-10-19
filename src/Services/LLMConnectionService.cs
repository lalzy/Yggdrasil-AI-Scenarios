// LLMConnectionService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Entities;
using System.Linq.Dynamic.Core;


namespace Yggdrasil.Services;

public class LLMConnectionService (AppDbContext db){
    public LLMConnection Create(LLMConnectionDTO.Request request){
        var entity = new LLMConnection{
            ResponseFormat = "",
            RequestFormat = "",
            URL = "",
            Name = ""
        };
        return entity;
    }

    public LLMConnection? Get(Guid ID){
        var entity = new LLMConnection{
            ResponseFormat = "",
            RequestFormat = "",
            URL = "",
            Name = ""
        };
        return entity;
    }

    public List<LLMConnection> GetAll(int? count=null, int page=1, string sortOrder=null){
        var entity = new LLMConnection{
            ResponseFormat = "",
            RequestFormat = "",
            URL = "",
            Name = ""
        };
        return [entity];
        
    }

    public LLMConnection? Update(Guid ID, LLMConnectionDTO.Update request){
        var entity = new LLMConnection{
            ResponseFormat = "",
            RequestFormat = "",
            URL = "",
            Name = ""
        };
        return entity;
    }

    public void Delete(Guid ID){}
}
