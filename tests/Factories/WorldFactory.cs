// WorldFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public static class WorldFactory{
    private static AutoFaker<T> BaseRules<T>(this AutoFaker<T> faker) where T : World{
        faker
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.LastUsed, _ => DateTime.UtcNow)
            .RuleFor(x => x.ID, _ => Guid.Empty);
        return faker;
    }

    public static World Create(AppDbContext db){
        var entity = new AutoFaker<World>()
            .BaseRules()
            .Generate();
        db.Set<World>().Add(entity);
        db.SaveChanges();
        return entity;
    }

    public static WorldInstance CreateInstance(AppDbContext db){
        var entity = new AutoFaker<WorldInstance>()
            .BaseRules()
            .RuleFor(x => x.Conversations, _ => [])
            .Generate();
        db.Set<WorldInstance>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
