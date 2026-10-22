// CharacterFactory.cs

using AutoBogus;
using Yggdrasil.Data;
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
            .Generate();

        db.Set<CharacterTemplate>().Add(entity);
        db.SaveChanges();
        return entity;
    }
    
    public static CharacterInstance CreateInstance(AppDbContext db){
        var entity = new AutoFaker<CharacterInstance>()
            .SharedRules()
            .Generate();

        db.Set<CharacterInstance>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
