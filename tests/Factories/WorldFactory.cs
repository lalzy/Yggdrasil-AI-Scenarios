// WorldFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public static class WorldFactory{
    private static AutoFaker<T> BaseRules<T>(this AutoFaker<T> faker) where T : WorldTemplate{
        faker
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.ID, _ => Guid.Empty);
        return faker;
    }

    public static WorldTemplate Create(AppDbContext db){
        var entity = new AutoFaker<WorldTemplate>()
            .BaseRules()
            .Generate();
        db.Set<WorldTemplate>().Add(entity);
        db.SaveChanges();
        return entity;
    }

    public static WorldInstance CreateInstance(AppDbContext db){
        var entity = new AutoFaker<WorldInstance>()
            .BaseRules()
            .RuleFor(x => x.Conversations, _ => [])
            .RuleFor(x => x.LastUsed, _ => DateTime.Now)
            .Generate();
        db.Set<WorldInstance>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
