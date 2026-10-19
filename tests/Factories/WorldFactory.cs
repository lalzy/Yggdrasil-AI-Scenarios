// WorldFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class WorldFactory{
    public static WorldTemplate Create(AppDbContext db, Narrator? narrator=null){
        narrator ??= NarratorFactory.Create(db);
        
        var entity = new AutoFaker<WorldTemplate>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.Narrator, _ => narrator)
            .Generate();
        db.Set<WorldTemplate>().Add(entity);
        db.SaveChanges();
        return entity;
    }

    public static WorldInstance CreateInstance(AppDbContext db, Narrator? narrator=null){
        narrator ??= NarratorFactory.Create(db);
        
        var entity = new AutoFaker<WorldInstance>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.Conversations, _ => [])
            .RuleFor(x => x.LastUsed, _ => DateTime.Now)
            .RuleFor(x => x.Narrator, _ => narrator)
            .Generate();
        db.Set<WorldInstance>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
