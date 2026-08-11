// ConversationFactory.cs

using Yggdrasil.Models;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

public class ConversationFactory(){
    public static Conversation Create(AppDbContext db, Guid? world_ID = null){
        var conversation = AutoFaker.Generate<Conversation>();
        if (world_ID.HasValue) conversation.World_ID = world_ID.Value;

        db.Set<Conversation>().Add(conversation);
        db.SaveChanges();
        return conversation;
    }

    public static Conversation Create(DatabaseFixture fixture, Guid? world_ID=null){
        return Create(fixture.CreateContext(), world_ID);
    }
}
