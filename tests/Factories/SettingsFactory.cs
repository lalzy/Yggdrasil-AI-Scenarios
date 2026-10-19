// SettingsFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class SettingsFactory{
    public static Settings Create(AppDbContext db, SystemPrompt? prompt = null){
        var entity = new AutoFaker<Settings>()
            .IDRule()
            .RuleFor(x => x.DefaultPrompt, _ => prompt ??= SystemPromptFactory.Create(db)).Generate();
        
        db.Set<Settings>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
