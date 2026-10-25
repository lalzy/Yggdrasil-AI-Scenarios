// SettingsFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class SettingsFactory{
    public static Settings Create(AppDbContext db){
        var entity = new AutoFaker<Settings>()
            .IDRule().Generate();
        
        db.Set<Settings>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
