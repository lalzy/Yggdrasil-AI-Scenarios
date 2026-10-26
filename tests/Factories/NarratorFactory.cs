// NarratorFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Tests.Factories;

public static class NarratorFactory{
    public static Narrator Create(AppDbContext db, LLMConnection? connection=null, List<Prompt>? prompts = null){
        connection ??= LLMConnectionFactory.Create(db);
        var entity = new AutoFaker<Narrator>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.Connection, _ => connection)
            .Generate();
        entity.Agent.Prompts = prompts ?? [];
        db.Set<Narrator>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
