// ChatFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public static class ChatFactory{
    public static Chat Create(AppDbContext db, Conversation conversation){
        var entity = new AutoFaker<Chat>().RuleFor(x => x.ID, _ => Guid.Empty).RuleFor(x => x.Conversation, _ => conversation).Generate();
        db.Set<Chat>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
