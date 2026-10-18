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

    public WorldTemplate Create(WorldTemplateDTO.Request request){
        var entity = request.Patch<WorldTemplate>();
        _world.Add(entity);
        db.SaveChanges();
        
        return entity;
    }

    public WorldTemplate? Get(Guid ID){
        return _world.Find(ID)!;
    }

    public WorldTemplate Update(Guid ID, WorldTemplateDTO.Update request){
        var entity = _world.Find(ID);
        var entry = _world.Entry(entity);

        foreach(var property in request.GetType().GetProperties()){
            var value = property.GetValue(request);
            if(value != null){
                entry.Property(property.Name).CurrentValue = value;
            }
        }
        
        db.SaveChanges();
        
        return entity!;
    }

    public void Delete(Guid ID){}
}
