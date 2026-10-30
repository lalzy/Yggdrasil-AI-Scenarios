// PersonaFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class PersonaFactory{
    public static PersonaTemplate Create(AppDbContext db){
        var entity = new AutoFaker<PersonaTemplate>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .Generate();
        db.Set<PersonaTemplate>().Add(entity);
        db.SaveChanges();

        return entity;
    }
}
