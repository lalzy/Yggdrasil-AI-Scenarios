// ConversationService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Extensions;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Services;

public class ConversationService (AppDbContext db){
    private readonly DbSet<Conversation> _conversation = db.Set<Conversation>();

    /// <summary>Create a new Conversation</summary>
    /// <param name="request">The ConversationDTO request</param>
    /// <returns>The created Conversation object</returns>
    public Conversation Create(ConversationDTO.Request request){
        var entity = new Conversation{Title = request.Title, Chats = request.Chats};
        _conversation.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get requested Conversation</summary>
    /// <param name="ID">ID of the conversation to get</param>
    /// <returns>The requested Conversation object</returns>
    public Conversation Get(Guid ID){
        var conv = new Conversation{Title=""};
        return conv;
    }

    /// <summary>Rename the Conversation title</summary>
    /// <param name="ID">ID of the conversation to change</param>
    /// <param name="newName">The new title</param>
    /// <returns>The modified conversation object</returns>
    public Conversation Rename(Guid ID, string newName){
        var conv = new Conversation{Title=""};
        return conv;
    }

    /// <summary>Delete the Conversation</summary>
    /// <param name="ID">ID of the conversation to delete</param>
    public void Delete(Guid ID){}
}
