// PlayerFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class PlayerFactory{
    public static PlayerInstance Create(AppDbContext db){
        var entity = new AutoFaker<PlayerInstance>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .Generate();

        db.Set<PlayerInstance>().Add(entity);
        db.SaveChanges();

        return entity;
    }
}
