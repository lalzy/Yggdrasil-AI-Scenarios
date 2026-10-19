// ConversationFactory.cs

using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class ConversationFactory{
    public static Conversation Create(AppDbContext db, WorldInstance worldInstance, List<Chat>? chats = null){
        chats ??= [];
        var entity = new AutoFaker<Conversation>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.World, _ => worldInstance)
            .RuleFor(x => x.Chats, _ => chats).Generate();
        db.Set<Conversation>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
