// WorldTemplateService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using Mapster;

namespace Yggdrasil.Services;

public class WorldTemplateService (AppDbContext db){
    private readonly DbSet<WorldTemplate> _world = db.Set<WorldTemplate>();

    public WorldTemplate Create(WorldDTO.Request request){
        var entity = request.Adapt<WorldTemplate>();// new WorldTemplate
        if(request.OpeningMessage != null)
            entity.OpeningMessages.Add(request.OpeningMessage);
        _world.Add(entity);
        db.SaveChanges();
        
        return entity;
    }

    public WorldTemplate Get(Guid ID){
        var entity = new WorldTemplate
        {
            Name = "",
            Description = "",
        };
        return entity;
    }

    public WorldTemplate Edit(Guid ID, WorldDTO.Edit request){
        var entity = new WorldTemplate
        {
            Name = "",
            Description = "",
        };
        return entity;
    }

    public void Delete(Guid ID){}
}
