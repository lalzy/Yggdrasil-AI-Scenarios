// WorldFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public static class WorldFactory{
    public static World Create(AppDbContext db){
        var entity = new AutoFaker<World>()
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.LastUsed, _ => DateTime.UtcNow)
            .RuleFor(x => x.ID, _ => Guid.Empty)
            .RuleFor(x => x.Conversations, _ => [])
            .Generate();
        db.Set<World>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
