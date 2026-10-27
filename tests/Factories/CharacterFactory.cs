// CharacterFactory.cs

using AutoBogus;
using Yggdrasil.Data;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class CharacterFactory{

    // Shared rules between template and instance
    private static AutoFaker<T> SharedRules<T>(this AutoFaker<T> faker) where T : class
    {
        return faker.IDRule().CreateAndUpdateAtRule();
    }
    
    public static CharacterTemplate Create(AppDbContext db){
        var entity = new AutoFaker<CharacterTemplate>()
            .SharedRules()
            .RuleFor(x => x.Worlds, [])
            .Generate();

        db.Set<CharacterTemplate>().Add(entity);
        db.SaveChanges();
        return entity;
    }
    
    public static CharacterInstance CreateInstance(AppDbContext db, WorldInstance? world = null, List<Prompt>? prompts=null){
        var entity = new AutoFaker<CharacterInstance>()
            .SharedRules()
            .RuleFor(x => x.World, world ?? null)
            .Generate();
        entity.Agent.Prompts = prompts ?? [];
        db.Set<CharacterInstance>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
