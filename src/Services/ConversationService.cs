// ConversationService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
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
        var world = db.Set<WorldInstance>().Find(request.WorldID);
        var entity = new Conversation{Title = request.Title, World=world!};
        entity.World = world;
        _conversation.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get requested Conversation</summary>
    /// <param name="ID">ID of the conversation to get</param>
    /// <returns>The requested Conversation object</returns>
    public Conversation Get(Guid ID){
        return _conversation.Include(c => c.Chats).Include(c => c.World).FirstOrDefault(c => c.ID == ID)!;
    }

    /// <summary>Rename the Conversation title</summary>
    /// <param name="ID">ID of the conversation to change</param>
    /// <param name="newName">The new title</param>
    /// <returns>The modified conversation object</returns>
    public Conversation Rename(Guid ID, string newName){
        var entity = _conversation.Include(c => c.World).FirstOrDefault(c => c.ID == ID)!;
        entity.Title = newName;
        db.SaveChanges();
        return entity;
    }

    /// <summary>Delete the Conversation</summary>
    /// <param name="ID">ID of the conversation to delete</param>
    public void Delete(Guid ID){
        var entity = _conversation.Find(ID)!;
        if(entity == null) return;
        _conversation.Remove(entity);
        db.SaveChanges();
    }

    // Remove Chats after stopAtID
    private List<Chat> GetChatsForEntity(List<Chat> chats, Guid? stopAtID){
        if(stopAtID != null){
            var index = chats.ToList().FindIndex(c => c.ID == stopAtID);
            if(index >= 0) chats = chats.Take(index + 1).ToList();
        }
        return chats;
    }
    
    /// <summary>Duplicate a conversation up until <paramref name="StopAtID"/></summary>
    /// <param name="ID">The conversation ID to duplicate</param>
    /// <param name="StopAtID">Chat ID where duplication should stop.</param>
    /// <returns>The new duplicated chat</returns>
    public Conversation? Duplicate(Guid ID, Guid? stopAtID=null){
        var entity = _conversation.AsNoTracking().Include(c => c.Chats).Include(c => c.World).FirstOrDefault(c => c.ID == ID);
        if(entity == null) return null;

        entity.Chats = GetChatsForEntity(entity.Chats, stopAtID);

        // Set new IDs
        entity.ID = Guid.NewGuid();
        foreach(var chat in entity.Chats){
            chat.ID = Guid.NewGuid();
        }
        
        _conversation.Add(entity);
        db.SaveChanges();
        return entity;
    }
}
