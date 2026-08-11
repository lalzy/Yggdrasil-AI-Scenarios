// ChatMessageFactory.cs

using Yggdrasil.Models;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Factories;

class ChatMessageFactory{
    public static ChatMessage Create(AppDbContext db, Guid? conversation_ID=null, DateTime? timeStamp=null){
        var message = AutoFaker.Generate<ChatMessage>();
        if(conversation_ID.HasValue) message.Conversation_ID = conversation_ID.Value;

        if(timeStamp.HasValue) message.TimeStamp = timeStamp.Value;

        message = db.Set<ChatMessage>().Add(message).Entity;
        db.SaveChanges();
        
        return message;
    }

    public static ChatMessage Create(DatabaseFixture fixture, Guid? conversation_ID = null, DateTime? timeStamp=null){
        return Create(fixture.CreateContext(), conversation_ID, timeStamp); 
    }
}
