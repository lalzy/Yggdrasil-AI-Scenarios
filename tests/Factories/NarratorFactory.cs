// NarratorFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class NarratorFactory{
    public static Narrator Create(AppDbContext db, LLMConnection? connection=null){
        connection ??= LLMConnectionFactory.Create(db);
        var entity = new AutoFaker<Narrator>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.Connection, _ => connection)
            .Generate();
        db.Set<Narrator>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
