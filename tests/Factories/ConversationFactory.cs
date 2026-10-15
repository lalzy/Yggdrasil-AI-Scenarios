// ConversationFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public static class ConversationFactory{
    public static Conversation Create(AppDbContext db, WorldInstance worldInstance, List<Chat>? chats = null){
        chats ??= [];
        var entity = new AutoFaker<Conversation>().RuleFor(x => x.ID, _ => Guid.Empty)
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.World, _ => worldInstance)
            .RuleFor(x => x.Chats, _ => chats).Generate();
        db.Set<Conversation>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
