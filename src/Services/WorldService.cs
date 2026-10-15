// WorldService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using Mapster;

namespace Yggdrasil.Services;

public class WorldService (AppDbContext db){
    private readonly DbSet<World> _world = db.Set<World>();

    public World Create(WorldDTO.Request request){
        var entity = new World
        {
            Description = "",
        };
        return entity;
    }

    public World Get(Guid ID){
        var entity = new World
        {
            Description = "",
        };
        return entity;
    }

    public World Edit(Guid ID, WorldDTO.Edit request){
        var entity = new World
        {
            Description = "",
        };
        return entity;
    }

    public void Delete(Guid ID){}
}
