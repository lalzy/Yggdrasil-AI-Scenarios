// ChatFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class ChatFactory{
    public static Chat Create(AppDbContext db, Conversation conversation){
        var entity = new AutoFaker<Chat>()
            .IDRule()
            .RuleFor(x => x.TimeStamp, _ => DateTime.UtcNow)
            .RuleFor(x => x.Conversation, _ => conversation)
            .Generate();
        db.Set<Chat>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
