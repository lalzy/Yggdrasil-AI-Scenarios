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

    /// <summary>Create a world template</summary>
    /// <param name="request">The worldTemplate's data</param>
    /// <returns>The new created worldTemplate</returns>
    public Narrator Create(NarratorDTO.Request request){
        var entity = request.Adapt<Narrator>();
        return entity;
    }

    /// <summary>Get a requested worldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to get</param>
    /// <returns>The requested worldTemplate</returns>
    public Narrator? Get(Guid ID){
        return new Narrator{
            DisplayName = "",
            Instruction = "",
            Description = "",
            Connection = new LLMConnection{
                ResponseFormat = "",
                RequestFormat = "",
                URL = "",
                Name = ""
            }
        };
    }

    /// <summary>Get all worldTemplates</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="page">Which page to fetch. This is absolute page and not index</param>
    /// <param name="sortBy">Sort Order</param>
    /// <returns>Paginated worldTemplate records</returns>
    public List<Narrator> GetAll(int? count=null, int page=1, string sortOrder=NarratorSortOrder.NameAsc){
        var entity = new Narrator{
            DisplayName = "",
            Instruction = "",
            Description = "",
            Connection = new LLMConnection{
                ResponseFormat = "",
                RequestFormat = "",
                URL = "",
                Name = ""
            }
        };
        return [entity];
    }

    /// <summary>Update the requested WorldTemplate</summary>
    /// <param name="ID">ID of the worldTemplate to update</param>
    /// <param name="request">The data to change</param>
    /// <returns>Updated WorldTemplate</returns>
    public Narrator? Update(Guid ID, NarratorDTO.Update request){
        return new Narrator{
            DisplayName = "",
            Instruction = "",
            Description = "",
            Connection = new LLMConnection{
                ResponseFormat = "",
                RequestFormat = "",
                URL = "",
                Name = ""
            }
        };
    }

    /// <summary>Delete a worldTemplate</summary>
    /// <param name="ID">ID of the worldTempalte to delete</param>
    public void Delete(Guid ID){
        
    }
}
